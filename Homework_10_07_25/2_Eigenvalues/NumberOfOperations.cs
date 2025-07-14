using System;

public class NumberOfOperations {
    public static void Main(string[] args) {
        if (args.Length < 1) {
            Console.WriteLine("n   seconds");
            return;
        }

        int n = int.Parse(args[0]);
        matrix A = new matrix(n, n);
        Random rand = new Random();

        // Generate a random symmetric matrix
        for (int i = 0; i < n; i++) {
            for (int j = i; j < n; j++) {
                double val = rand.NextDouble();
                A[i, j] = val;
                A[j, i] = val;
            }
        }

        vector w = new vector(n);
        matrix V = new matrix(n, n);

        // Time just the diagonalization
        var watch = System.Diagnostics.Stopwatch.StartNew();
        jacobi.cyclic(A, w, V);
        watch.Stop();

        double seconds = watch.Elapsed.TotalSeconds;
        Console.WriteLine("{0} {1:F6}", n, seconds);
    }
}
