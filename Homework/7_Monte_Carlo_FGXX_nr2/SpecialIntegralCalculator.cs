using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class SpecialIntegralCalculator
{
    private static readonly double[] a = { 0.0, 0.0, 0.0 };
    private static readonly double[] b = { Math.PI, Math.PI, Math.PI };
    private static readonly double analyticalValue = 1.393203929685676859;
    private const string DataFile = "Estimate_SpecialIntegral.txt";

    public static void CalculateAndSaveResults(int maxN = 100)
    {
        HashSet<int> existingNs = ReadExistingData();
        var allNs = Enumerable.Range(1, maxN).ToList();

        using (var writer = new StreamWriter(DataFile, append: true))
        {
            if (!existingNs.Any())
                writer.WriteLine("N\tMC est. error\tMC actual error\tValue");

            foreach (int N in allNs)
            {
                if (existingNs.Contains(N)) continue;

                var (result, error) = PlainMC.Integrate(SpecialIntegrand, a, b, N);
                double actualError = Math.Abs(result - analyticalValue);
                writer.WriteLine($"{N}\t{error:E6}\t{actualError:E6}\t{result:F9}");

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
        double A = 1.0 / (Math.PI * Math.PI * Math.PI);
        return A / (1.0 - Math.Cos(v[0]) * Math.Cos(v[1]) * Math.Cos(v[2]));
    }
}
