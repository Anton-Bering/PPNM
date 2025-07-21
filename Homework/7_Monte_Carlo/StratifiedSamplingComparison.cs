using System;
using System.IO;

public static class StratifiedSamplingComparison
{
    private const string DataFile = "StratifiedSamplingResults.txt";

    public static void RunComparison()
    {
        Console.WriteLine("\n--- Part C: Recursive Stratified Sampling ---");
        
        double[] a = { 0.0, 0.0 };
        double[] b = { 1.0, 1.0 };
        double trueValue = 1000.0 * (1 - Math.Exp(-100.0)) / 100.0 * (1 - Math.Exp(-10.0)) / 10.0;
        int N = 10000;

        var (plainRes, plainErr) = PlainMC.Integrate(PeakedFunction, a, b, N);
        var (stratRes, stratErr) = StratifiedMC.Integrate(PeakedFunction, a, b, N);

        Console.WriteLine("Integrating 1000*exp(-100x - 10y) on [0,1]^2 (true value ≈ {0:F6}) with N={1}", trueValue, N);
        Console.WriteLine("Plain MC:      result = {0:F6},  error estimate = {1:F6},  actual error = {2:F6}",
                          plainRes, plainErr, Math.Abs(plainRes - trueValue));
        Console.WriteLine("Stratified MC: result = {0:F6},  error estimate = {1:F6},  actual error = {2:F6}",
                          stratRes, stratErr, Math.Abs(stratRes - trueValue));

        // Gem til fil
        using (var writer = new StreamWriter(DataFile))
        {
            writer.WriteLine("Method\tResult\tErrorEstimate\tActualError");
            writer.WriteLine($"PlainMC\t{plainRes:F6}\t{plainErr:F6}\t{Math.Abs(plainRes - trueValue):F6}");
            writer.WriteLine($"StratifiedMC\t{stratRes:F6}\t{stratErr:F6}\t{Math.Abs(stratRes - trueValue):F6}");
        }

        Console.WriteLine("\nResults saved to: " + DataFile);
    }

    private static double PeakedFunction(double[] v)
    {
        double x = v[0], y = v[1];
        return 1000.0 * Math.Exp(-100.0 * x - 10.0 * y);
    }
}
