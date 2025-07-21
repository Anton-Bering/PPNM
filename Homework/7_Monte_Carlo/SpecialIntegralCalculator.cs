using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public static class SpecialIntegralCalculator
{
    private static readonly double[] a = { 0.0, 0.0, 0.0 };
    private static readonly double[] b = { Math.PI, Math.PI, Math.PI };
    private static readonly double analyticalValue = 1.393203929685676859;
    private const string DataFile = "Estimate_SpecialIntegral.txt";

    public static void CalculateAndSaveResults(int maxN = 100)
    {
        HashSet<int> existingNs = ReadExistingData();

        // Logaritmisk fordelte N-værdier
        var allNs = Enumerable.Range(1, maxN)
                              .Select(i => (int)Math.Round(Math.Pow(10, i * Math.Log10(maxN) / maxN)))
                              .Distinct()
                              .ToList();

        using (var writer = new StreamWriter(DataFile, append: true))
        {
            if (!existingNs.Any())
                writer.WriteLine("N\tMC est. error\tMC actual error\tValue");

            foreach (int N in allNs)
            {
                if (existingNs.Contains(N)) continue;

                var (result, error) = PlainMC.Integrate(SpecialIntegrand, a, b, N);
                double actualError = Math.Abs(result - analyticalValue);

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0}\t{1:E6}\t{2:E6}\t{3:F9}", N, error, actualError, result));

                if (N % 1000 == 0)
                    Console.WriteLine($"Processed N={N} for Special Integral");
            }
        }
    }

    private static HashSet<int> ReadExistingData()
    {
        HashSet<int> existingNs = new HashSet<int>();
        if (File.Exists(DataFile))
        {
            foreach (string line in File.ReadLines(DataFile).Skip(1))
            {
                string[] parts = line.Split('\t');
                if (int.TryParse(parts[0], out int n))
                    existingNs.Add(n);
            }
        }
        return existingNs;
    }

    private static double SpecialIntegrand(double[] v)
    {
        double cosx = Math.Cos(v[0]);
        double cosy = Math.Cos(v[1]);
        double cosz = Math.Cos(v[2]);
        double denom = 1.0 - cosx * cosy * cosz;

        // Undgå division med meget små tal
        if (denom < 1e-10)
            return 0.0;

        double A = 1.0 / (Math.PI * Math.PI * Math.PI);
        return A / denom;
    }
}
