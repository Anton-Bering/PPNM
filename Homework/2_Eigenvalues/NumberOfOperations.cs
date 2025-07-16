using System;
using System.Diagnostics;

public static class NumberOfOperations
{
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("n   seconds");
            return;
        }

        int n   = int.Parse(args[0]);

        /* Random symmetric matrix */
        double[,] A = VectorAndMatrix.RandomMatrix(n, n);
        VectorAndMatrix.Symmetrize(A);

        double[]  w = new double[n];
        double[,] V = new double[n, n];

        var watch = Stopwatch.StartNew();
        jacobi.cyclic(A, w, V);
        watch.Stop();

        Console.WriteLine($"{n} {watch.Elapsed.TotalSeconds:F6}");
    }
}
