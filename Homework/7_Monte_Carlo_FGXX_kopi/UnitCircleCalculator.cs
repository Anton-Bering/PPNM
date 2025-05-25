using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public static class UnitCircleCalculator
{
    private static readonly double[] a = { -1.0, -1.0 };
    private static readonly double[] b = { 1.0, 1.0 };
    private static readonly double trueArea = Math.PI;
    private const string DataFile = "Estimate_the_area_of_a_unit_circle.txt";

    public static void CalculateAndSaveResults(int maxN = 100000)
    {
        // Hent eksisterende N-værdier fra fil
        HashSet<int> existingNs = ReadExistingData();

        // Generer alle N-værdier fra 1 til maxN
        var allNs = Enumerable.Range(1, maxN).ToList();

        using (var writer = new StreamWriter(DataFile, append: true))
        {
            if (!existingNs.Any())
                writer.WriteLine("N\tMC est. error\tMC actual error\tValue");

            foreach (int N in allNs)
            {
                if (existingNs.Contains(N)) continue;

                var (result, error) = PlainMC.Integrate(UnitCircleIndicator, a, b, N);
                double actualError = Math.Abs(result - trueArea);
                writer.WriteLine($"{N}\t{error:E6}\t{actualError:E6}\t{result:F6}");

                if (N % 1000 == 0)
                    Console.WriteLine($"Processed N={N}");
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

    private static double UnitCircleIndicator(double[] v)
    {
        double x = v[0], y = v[1];
        return (x * x + y * y <= 1.0) ? 1.0 : 0.0;
    }
}
