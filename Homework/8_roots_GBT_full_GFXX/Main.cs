using System;
using static System.Math;
class Program {
    static void Main() {
        // Part A: Newton's Method on Rosenbrock and Himmelblau functions
        Console.WriteLine("Part A: Newton's method results");
        // Rosenbrock's function gradient (analytical)
        Func<vector, vector> rosenGrad = p => {
            double x = p[0], y = p[1];
            return new vector(
                -2 * (1 - x) - 400 * x * (y - x * x),   // df/dx
                 200 * (y - x * x)                      // df/dy
            );
        };
        // Rosenbrock function value (for verification output)
        Func<double, double, double> rosenVal = (x, y) => (1 - x) * (1 - x) + 100 * (y - x * x) * (y - x * x);
        // Himmelblau's function gradient (analytical)
        Func<vector, vector> himmelGrad = p => {
            double x = p[0], y = p[1];
            double A = x * x + y - 11;
            double B = x + y * y - 7;
            // partial derivatives:
            double df_dx = 4 * x * A + 2 * B;
            double df_dy = 2 * A + 4 * y * B;
            return new vector(df_dx, df_dy);
        };
        // Himmelblau function value (for verification output)
        Func<double, double, double> himmelVal = (x, y) => {
            double A = x * x + y - 11;
            double B = x + y * y - 7;
            return A * A + B * B;
        };

        // Solve Rosenbrock gradient=0 from an initial guess
        vector startRosen = new vector(5.0, 5.0);
        vector solRosen = RootFind.newton(rosenGrad, startRosen, 1e-6);
        Console.WriteLine($"Rosenbrock minimum starting from (5,5): x = {solRosen[0]:F6}, y = {solRosen[1]:F6}, f = {rosenVal(solRosen[0], solRosen[1]):F6}");

        // Solve Himmelblau gradient=0 from multiple initial guesses to find all minima
        vector[] starts = {
            new vector(6.0, 6.0),
            new vector(-6.0, 6.0),
            new vector(-6.0, -6.0),
            new vector(6.0, -6.0)
        };
        for (int i = 0; i < starts.Length; i++) {
            vector solHim = RootFind.newton(himmelGrad, starts[i], 1e-6);
            Console.WriteLine($"Himmelblau minimum from ({starts[i][0]},{starts[i][1]}): x = {solHim[0]:F6}, y = {solHim[1]:F6}, f = {himmelVal(solHim[0], solHim[1]):F6}");
        }

        // Part B: Shooting method for Hydrogen atom bound state
        Console.WriteLine("\nPart B: Hydrogen atom bound state via shooting method");
        double rmin = 1e-3;
        double rmax = 8.0;
        // Define M(E) = f_E(r_max) using our ODE solver
        Func<double, double> M = E => {
            vector y0 = new vector(rmin - rmin * rmin, 1 - 2 * rmin);  // initial [f(r_min), f'(r_min)]
            vector y_end = ODESolver.Integrate(ODESolver.HydrogenODE(E), rmin, y0, rmax, 1e-6, 1e-6);
            return y_end[0];  // f(r_max)
        };
        // Bracket the root (E0 ~ -0.5). Start with E_low and E_high such that M(E_low) and M(E_high) have opposite sign.
        double E_low = -1.0;
        double E_high = -0.1;
        double f_low = M(E_low);
        double f_high = M(E_high);
        // Adjust bracket if needed
        int bracketIter = 0;
        while (f_low * f_high > 0 && bracketIter < 20) {
            if (Math.Abs(f_low) < Math.Abs(f_high)) {
                // f_low is smaller in magnitude -> extend E_low further negative
                E_low *= 2;     // make E_low more negative
                f_low = M(E_low);
            } else {
                // extend E_high closer to 0
                E_high *= 0.5;  // make E_high less negative
                f_high = M(E_high);
            }
            bracketIter++;
        }
        if (f_low * f_high > 0) {
            Console.WriteLine("Failed to bracket the energy root - adjust initial guesses.");
        }
        // Bisection to solve M(E)=0
        double E0 = 0;
        double f_mid = 0;
        for (int iter = 0; iter < 60; iter++) {
            E0 = 0.5 * (E_low + E_high);
            f_mid = M(E0);
            if (Math.Abs(f_mid) < 1e-6) break;             // found close to zero
            if (f_mid * f_low < 0) {
                // root lies between E_low and E0
                E_high = E0;
                f_high = f_mid;
            } else {
                // root lies between E0 and E_high
                E_low = E0;
                f_low = f_mid;
            }
            if (Math.Abs(E_high - E_low) < 1e-6) break;    // interval sufficiently small
        }
        Console.WriteLine($"Lowest eigen-energy E0 ≈ {E0:F6} (expected -0.5)");

        // Integrate once more at E0 to get wavefunction values for output
        vector yStart = new vector(rmin - rmin * rmin, 1 - 2 * rmin);
        var rList = new System.Collections.Generic.List<double>();
        var fList = new System.Collections.Generic.List<double>();
        ODESolver.Integrate(ODESolver.HydrogenODE(E0), rmin, yStart, rmax, 1e-6, 1e-6, rList, fList);
        Console.WriteLine("\nr\tf(r)");
        for (int k = 0; k < rList.Count; k++) {
            Console.WriteLine($"{rList[k]:F4}\t{fList[k]:F6}");
        }
    }
}
