using System;
using System.IO;

public static class QuasiVsPseudoComparison
{
    private const string DataFile = "QuasiVsPseudoResults.txt";

    public static void RunComparison()
    {
        Console.WriteLine("\n--- Part B: Quasi-Random vs Pseudo-Random Monte Carlo ---");
        Console.WriteLine("Integrand f(x,y) = x*y over [0,1]^2 (true value = 0.250000)");
        Console.WriteLine("{0,8}  {1,14}  {2,14}  {3,14}  {4,14}", 
                          "N", "MC est.err", "MC actual err", "QMC est.err", "QMC actual err");

        int[] Nlist = { 1000, 10000, 100000 };
        using (var writer = new StreamWriter(DataFile))
        {
            writer.WriteLine("N\tMC_est_err\tMC_actual_err\tQMC_est_err\tQMC_actual_err");

            foreach (int N in Nlist)
            {
                var (resR, errR) = PlainMC.Integrate(Fxy, new[] {0.0, 0.0}, new[] {1.0, 1.0}, N);
                double actualErrR = Math.Abs(resR - 0.25);
                var (resQ, errQ) = QuasiRandomMC.Integrate(Fxy, new[] {0.0, 0.0}, new[] {1.0, 1.0}, N);
                double actualErrQ = Math.Abs(resQ - 0.25);

                // Write to console
                Console.WriteLine($"{N,8}  {errR,14:E6}  {actualErrR,14:E6}  {errQ,14:E6}  {actualErrQ,14:E6}");

                // Write to file
                writer.WriteLine($"{N}\t{errR:E6}\t{actualErrR:E6}\t{errQ:E6}\t{actualErrQ:E6}");
            }
        }

        Console.WriteLine("\nResults saved to: " + DataFile);
        Console.WriteLine("(MC uses internal variance; QMC uses difference of two sequences.)");
    }

    private static double Fxy(double[] v) => v[0] * v[1];
}
