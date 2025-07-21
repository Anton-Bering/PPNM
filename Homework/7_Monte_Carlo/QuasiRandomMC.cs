using System;
using System.Collections.Generic;

public static class QuasiRandomMC
{
    // Precomputed primes for Halton sequences
    private static readonly int[] primes = new int[] {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
        31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
        73, 79, 83, 89, 97, 101, 103, 107, 109, 113
    };

    // Halton sequence generator for a given index (1-based) and base.
    // Returns a number in [0,1).
    private static double Halton(int index, int b)
    {
        double f = 1.0;
        double r = 0.0;
        int i = index;
        while (i > 0)
        {
            f /= b;
            r += f * (i % b);
            i /= b;
        }
        return r;
    }

    // MC integration using low-discrepancy (quasi-random) sequences.
    // Uses two different Halton sequences to estimate error.
    public static (double result, double error) Integrate(Func<double[], double> f, double[] a, double[] b, int N)
    {
        int dim = a.Length;
        double V = 1.0;
        for (int i = 0; i < dim; i++)
            V *= (b[i] - a[i]);

        if (dim > primes.Length / 2)
            throw new ArgumentException("Dimension too high for available prime bases.");
        int offset = dim;
        
        double sum1 = 0.0, sum2 = 0.0;
        double[] x = new double[dim];
  
        for (int i = 1; i <= N; i++)
        {
            // Sequence 1 point
            for (int k = 0; k < dim; k++)
            {
                double u = Halton(i, primes[k]);
                x[k] = a[k] + u * (b[k] - a[k]);
            }
            sum1 += f(x);
            // Sequence 2 point
            for (int k = 0; k < dim; k++)
            {
                double u = Halton(i, primes[offset + k]);
                x[k] = a[k] + u * (b[k] - a[k]);
            }
            sum2 += f(x);
        }
        double mean1 = sum1 / N;
        double mean2 = sum2 / N;
        double result1 = mean1 * V;
        double result2 = mean2 * V;
        // Use the average of two sequences as final result
        double result = 0.5 * (result1 + result2);
        // Error estimated by the difference between the two sequence estimates
        double error = Math.Abs(result1 - result2);
        return (result, error);
    }
}
