using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class GaussianBellCalculator
{
    private static readonly double[] a = { -2.0, -2.0 };
    private static readonly double[] b = { 2.0, 2.0 };
    private static readonly double trueValue = Math.PI; // Integralet over ℝ² er π, men over [-2,2]^2 er det ca. 0.98π
    private const string DataFile = "Estimate_GaussianBell2D.txt";

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

                var (result, error) = PlainMC.Integrate(GaussianBell2D, a, b, N);
                double actualError = Math.Abs(result - trueValue);
                writer.WriteLine($"{N}\t{error:E6}\t{actualError:E6}\t{result:F6}");

                if (N % 1000 == 0)
                    Console.WriteLine($"Processed N={N} for Gaussian Bell");
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

    private static double GaussianBell2D(double[] v)
    {
        double x = v[0], y = v[1];
        return Math.Exp(-(x * x + y * y));
    }
}
