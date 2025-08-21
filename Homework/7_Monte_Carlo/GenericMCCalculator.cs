using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public static class GenericMCCalculator
{
    // Denne metode er nu den centrale "motor" for dine beregninger i Del A.
    public static void Run(
        Func<double[], double> f,
        double[] a,
        double[] b,
        double trueValue,
        string dataFile,
        string description,
        int maxN = 10000)
    {
        HashSet<int> existingNs = ReadExistingData(dataFile);

        // Bruger logaritmisk fordelte N-værdier for mere effektive plots
        var allNs = Enumerable.Range(1, 1000) // Kør færre, men bedre fordelte punkter
                              .Select(i => (int)Math.Round(Math.Pow(10, (double)i / 1000 * Math.Log10(maxN))))
                              .Distinct()
                              .Where(n => n > 0)
                              .ToList();

        using (var writer = new StreamWriter(dataFile, append: true))
        {
            if (!existingNs.Any())
                writer.WriteLine("N\tMC est. error\tMC actual error\tValue");

            foreach (int N in allNs)
            {
                if (existingNs.Contains(N)) continue;

                var (result, error) = PlainMC.Integrate(f, a, b, N);
                double actualError = Math.Abs(result - trueValue);

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0}\t{1:E6}\t{2:E6}\t{3:F9}", N, error, actualError, result));

                // Giver en statusopdatering en gang imellem
                if (N % (allNs.Count / 10) == 0 && N > 0)
                     Console.WriteLine($"Processing {description}: N={N}");
            }
        }
        Console.WriteLine($"Finished calculation for {description}. Results saved to {dataFile}.");
    }

    // Denne hjælpe-metode læser eksisterende data for at undgå at regne det samme igen.
    private static HashSet<int> ReadExistingData(string dataFile)
    {
        HashSet<int> existingNs = new HashSet<int>();
        if (File.Exists(dataFile))
        {
            foreach (string line in File.ReadLines(dataFile).Skip(1))
            {
                string[] parts = line.Split('\t');
                if (int.TryParse(parts[0], out int n))
                    existingNs.Add(n);
            }
        }
        return existingNs;
    }
}
