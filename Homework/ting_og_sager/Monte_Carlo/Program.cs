using System;
using static System.Math;

class Program
{
    static void Main()
    {
        int N = 100000;  // number of sample points for each Monte Carlo estimation

        // 1) Integral from 0 to 1 of 1/(1+x^2) dx  (expected result = pi/4)
        Func<double, double> f1 = x => 1.0 / (1 + x * x);
        (double res1, double err1) = MonteCarlo.plainMC(f1, 0.0, 1.0, N);
        double exact1 = PI / 4.0;

        // 2) Integral from 0 to infinity of x^2 * exp(-x^2) dx (expected result = sqrt(pi)/4)
        // Plain Monte Carlo: approximate by integrating from 0 to 4 (nearly all contribution in [0,4])
        Func<double, double> f2 = x => x * x * Exp(-x * x);
        (double res2_plain, double err2_plain) = MonteCarlo.plainMC(f2, 0.0, 4.0, N);
        // Importance sampling Monte Carlo with p(x) = 2x*exp(-x^2) (Rayleigh distribution)
        var dist = new RayleighDistribution();
        (double res2_imp, double err2_imp) = MonteCarlo.importanceMC(f2, dist, N);
        double exact2 = Sqrt(PI) / 4.0;

        // 3) Area of unit circle (radius=1).
        // We integrate f(x,y) = 1 inside the circle x^2+y^2<=1 (and 0 outside) over the square [-1,1]×[-1,1].
        // The true area is pi (~3.141593).
        Func<vector, double> f3 = (vector v) =>
        {
            double x = v[0], y = v[1];
            return (x * x + y * y <= 1.0) ? 1.0 : 0.0;
        };
        vector a2 = new vector(-1.0, -1.0);
        vector b2 = new vector( 1.0,  1.0);
        (double res3, double err3) = MonteCarlo.plainMC(f3, a2, b2, N);
        double exact3 = PI * 1.0 * 1.0;  // area = pi*r^2 for r=1

        // Print results with comparisons
        Console.WriteLine("Integral of 1/(1+x^2) from 0 to 1 = {0:F6} ± {1:F6} (exact {2:F6}, N={3})",
                          res1, err1, exact1, N);

        Console.WriteLine("Integral of x^2 * exp(-x^2) from 0 to infinity:");
        Console.WriteLine("  Plain MC (uniform x in [0,4])   = {0:F6} ± {1:F6} (N={2})", 
                          res2_plain, err2_plain, N);
        Console.WriteLine("  Importance sampling (p(x)=2x*exp(-x^2)) = {0:F6} ± {1:F6} (N={2})", 
                          res2_imp, err2_imp, N);
        Console.WriteLine("  Exact result = {0:F6}", exact2);

        Console.WriteLine("Area of unit circle (radius 1) = {0:F6} ± {1:F6} (exact {2:F6}, N={3})",
                          res3, err3, exact3, N);
    }
}
