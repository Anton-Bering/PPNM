using System;
using System.Diagnostics;

public static class NumberOfOperations
{
    public static void Main(string[] args)
    {
        /* Udkometer da jeg ikke længer brugre Stopwatch men time.. 
        if (args.Length < 1) 
        {
            Console.WriteLine("n   seconds");
            return;
        }
        */ 

        int n   = int.Parse(args[0]);

        /* Random symmetric matrix */
        double[,] A = VectorAndMatrix.RandomMatrix(n, n);
        VectorAndMatrix.Symmetrize(A);

        double[]  w = new double[n];
        double[,] V = new double[n, n];

        // var watch = Stopwatch.StartNew(); // Udkometer da jeg ikke længer brugre Stopwatch men time..
        jacobi.cyclic(A, w, V);
        // watch.Stop(); // Udkometer da jeg ikke længer brugre Stopwatch men time..

        //Console.WriteLine($"{n} {watch.Elapsed.TotalSeconds:F6}"); // Udkometer da jeg ikke længer brugre Stopwatch men time..
    }
}
