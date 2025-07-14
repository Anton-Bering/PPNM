using System;

public static class PlainMC
{
    private static Random rng = new Random();  // shared random number generator

    /// <summary>
    /// Plain Monte Carlo integration over a hyper-rectangle [a,b] in d dimensions.
    /// Returns a tuple (integral, error).
    /// </summary>
    public static (double result, double error) Integrate(Func<double[], double> f, double[] a, double[] b, int N)
    {
        int dim = a.Length;
        // Compute volume of the integration region V = Π_i (b[i]-a[i])
        double V = 1.0;
        for (int i = 0; i < dim; i++)
            V *= (b[i] - a[i]);

        double sum = 0.0, sum2 = 0.0;
        double[] x = new double[dim];
        for (int i = 0; i < N; i++)
        {
            // Sample a random point uniformly in [a,b]
            for (int k = 0; k < dim; k++)
            {
                double u = rng.NextDouble();
                x[k] = a[k] + u * (b[k] - a[k]);
            }
            double fx = f(x);
            sum  += fx;
            sum2 += fx * fx;
        }

        double mean = sum / N;
        // Standard deviation of the mean (sigma/sqrt(N)), using sample variance
        double variance = sum2 / N - mean * mean;
        double sigma = Math.Sqrt(Math.Max(0.0, variance));  // ensure non-negative
        double error = sigma * V / Math.Sqrt(N);
        double result = mean * V;
        return (result, error);
    }
}
