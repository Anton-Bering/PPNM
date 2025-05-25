using System;
using static System.Math;

public interface IDistribution1D
{
    // Generate a random sample according to the distribution
    double Sample(Random rnd);

    // Probability density function p(x) of the distribution
    double PDF(double x);
}

public class RayleighDistribution : IDistribution1D
{
    // p(x) = 2*x*exp(-x^2) for x >= 0 (Rayleigh distribution with sigma=1/√2)
    public double Sample(Random rnd)
    {
        // Inverse transform sampling: if U ~ Uniform(0,1), then X = sqrt(-ln(U))
        // yields the distribution p(x) = 2x * exp(-x^2)
        double U = rnd.NextDouble();
        return Sqrt(-Log(U));
    }

    public double PDF(double x)
    {
        if (x < 0) return 0.0;
        return 2 * x * Exp(-x * x);
    }
}

public static class MonteCarlo
{
    private static Random rnd = new Random();  // shared RNG for all Monte Carlo sampling

    // Plain Monte Carlo integration over a d-dimensional hyper-rectangle [a,b].
    // Returns (estimate, error) for ∫_a^b f(x) d^d x.
    public static (double result, double error) plainMC(Func<vector, double> f, vector a, vector b, int N)
    {
        int dim = a.size;
        // Compute volume of the integration domain V = ∏_{i=1}^d (b_i - a_i)
        double V = 1.0;
        for (int i = 0; i < dim; i++)
            V *= (b[i] - a[i]);

        double sum = 0.0, sum2 = 0.0;
        vector x = new vector(dim);
        for (int i = 0; i < N; i++)
        {
            // Sample a random point x uniformly in [a,b]^d
            for (int k = 0; k < dim; k++)
                x[k] = a[k] + rnd.NextDouble() * (b[k] - a[k]);
            double fx = f(x);
            sum  += fx;
            sum2 += fx * fx;
        }
        double mean = sum / N;
        double sigma = Sqrt(sum2 / N - mean * mean);  // standard deviation of f over domain
        double result = mean * V;
        double error  = sigma * V / Sqrt(N);
        return (result, error);
    }

    // Convenience overload for one-dimensional plain Monte Carlo integration
    public static (double result, double error) plainMC(Func<double, double> f, double a, double b, int N)
    {
        // Wrap the 1D function into the vector form expected by plainMC
        Func<vector, double> f_vec = (vector x) => f(x[0]);
        var a_vec = new vector(a);
        var b_vec = new vector(b);
        return plainMC(f_vec, a_vec, b_vec, N);
    }

    // Importance sampling Monte Carlo for one-dimensional integrals.
    // Samples from the provided distribution p(x), and estimates ∫ f(x) dx over distribution's support.
    // Returns (estimate, error) for the integral.
    public static (double result, double error) importanceMC(Func<double, double> f, IDistribution1D dist, int N)
    {
        double sum = 0.0, sum2 = 0.0;
        for (int i = 0; i < N; i++)
        {
            // Sample x from the importance distribution p(x)
            double x = dist.Sample(rnd);
            double fx = f(x);
            double w = fx / dist.PDF(x);  // weight = f(x)/p(x)
            sum  += w;
            sum2 += w * w;
        }
        double mean = sum / N;
        double sigma = Sqrt(sum2 / N - mean * mean);
        // Since importance sampling computes the expectation of f/p under p, the mean is the integral
        double result = mean;
        double error  = sigma / Sqrt(N);
        return (result, error);
    }
}
