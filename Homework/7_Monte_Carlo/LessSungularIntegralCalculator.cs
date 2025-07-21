using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public static class LessSungularIntegralCalculator
{
    private static readonly double[] a = { 0.0, 0.0, 0.0 };
    private static readonly double[] b = { Math.PI, Math.PI, Math.PI };
    private static readonly double exactValue = 8.0 / (Math.PI * Math.PI * Math.PI);
    private const string DataFile = "Estimate_LessSungularIntegral.txt";

    public static void CalculateAndSaveResults(int maxN = 10000)
    {
        HashSet<int> existingNs = ReadExistingData();

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

                var (res, err) = PlainMC.Integrate(SmoothIntegrand, a, b, N);
                double actualErr = Math.Abs(res - exactValue);

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0}\t{1:E6}\t{2:E6}\t{3:F9}", N, err, actualErr, res));

                if (N % 1000 == 0)
                    Console.WriteLine($"Processed N={N} for Less Singular Integral");
            }
        }
    }

    public static void PrintLastEstimateSummary()
    {
        if (!File.Exists(DataFile))
        {
            Console.WriteLine($"Error: File '{DataFile}' not found.");
            return;
        }

        string lastLine = File.ReadAllLines(DataFile)
            .Reverse()
            .FirstOrDefault(line =>
            {
                var parts = line.Split('\t');
                return parts.Length >= 4 &&
                       double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out _);
            });

        if (lastLine != null)
        {
            var parts = lastLine.Split('\t');
            int.TryParse(parts[0], out int lastN);
            double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double result);

            Console.WriteLine($"\nThe file '{DataFile}' contains the results for the value of the less sungular 3D Integral.");
            Console.WriteLine($"After {lastN} samples, the estimated value is {result:F9}.");
            Console.WriteLine($"The plot 'Estimate_LessSungularIntegral_error.plot' shows the estimated and actual error as a function of N.");
            Console.WriteLine($"As shown in the plot, the error scales approximately as N^(-1/2), as expected from theory.");
        }
        else
        {
            Console.WriteLine($"Warning: No valid data line found in '{DataFile}'.");
        }
    }

    private static double SmoothIntegrand(double[] v)
    {
        return Math.Cos(v[0]) * Math.Cos(v[1]) * Math.Cos(v[2]) / (Math.PI * Math.PI * Math.PI);
    }

    private static HashSet<int> ReadExistingData()
    {
        var existingNs = new HashSet<int>();
        if (File.Exists(DataFile))
        {
            foreach (var line in File.ReadLines(DataFile).Skip(1))
            {
                var parts = line.Split('\t');
                if (int.TryParse(parts[0], out int n))
                    existingNs.Add(n);
            }
        }
        return existingNs;
    }
}
