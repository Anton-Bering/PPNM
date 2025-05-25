using System;

class Program {
    // Himmelblau's gradient
    static vector HimmelblauGradient(vector v) {
        double x = v[0], y = v[1];
        double f1 = x * x + y - 11;
        double f2 = x + y * y - 7;
        double dfx = 4 * x * f1 + 2 * f2;
        double dfy = 2 * f1 + 4 * y * f2;
        return new vector(new double[] { dfx, dfy });
    }

    // Hydrogen atom shooting method
    static double hydrogenM(double E) {
        double rmin = 1e-3;
        double rmax = 8.0;
        double u0 = rmin - rmin * rmin;
        double w0 = 1.0 - 2.0 * rmin;
        vector y0 = new vector(u0, w0);
        Func<double, vector, vector> schrodinger = (r, y) => {
            double u = y[0], w = y[1];
            double du = w;
            double dw = -(2 * E + 2 / r) * u;
            return new vector(du, dw);
        };
        vector yf = ode.driver(schrodinger, rmin, y0, rmax);
        return yf[0];
    }

    static void Main(string[] args) {
        Console.WriteLine("Root-finding tests:\n");

        // 1D root: f(x) = x² - 4
        Func<vector, vector> f1d = v => new vector(v[0] * v[0] - 4);
        Console.WriteLine("1D test: Solve f(x) = x^2 - 4 = 0");
        Console.WriteLine($"Starting x0 = 1.0  ->  root ≈ {root.newton(f1d, new vector(1.0))[0]:F4}");
        Console.WriteLine($"Starting x0 = -1.0 ->  root ≈ {root.newton(f1d, new vector(-1.0))[0]:F4}");

        // 2D nonlinear system
        Func<vector, vector> f2d = v => {
            double x = v[0], y = v[1];
            return new vector(x + 2 * y - 2, x * x + y - 1);
        };
        Console.WriteLine("\n2D system test:");
        vector s1 = root.newton(f2d, new vector(1.0, 1.0));
        Console.WriteLine($"Start (1,1) -> root ≈ ({s1[0]:F4}, {s1[1]:F4})");
        vector s2 = root.newton(f2d, new vector(0.0, 0.0));
        Console.WriteLine($"Start (0,0) -> root ≈ ({s2[0]:F4}, {s2[1]:F4})");

        // Rosenbrock gradient
        Func<vector, vector> rosenGrad = v => {
            double x = v[0], y = v[1];
            return new vector(2 * (x - 1) - 400 * x * (y - x * x), 200 * (y - x * x));
        };
        Console.WriteLine("\nRosenbrock minimum:");
        vector rmin = root.newton(rosenGrad, new vector(0.0, 0.0), 1e-6);
        Console.WriteLine($"Minimum at ≈ ({rmin[0]:F4}, {rmin[1]:F4})");

        // Himmelblau minima
        Console.WriteLine("\nHimmelblau minima:");
        vector[] starts = {
            new vector(5, 5), new vector(-5, 5),
            new vector(-5, -5), new vector(5, -5)
        };
        int idx = 1;
        foreach (var start in starts) {
            vector min = root.newton(HimmelblauGradient, start, 1e-6);
            Console.WriteLine($"Start #{idx++}: min ≈ ({min[0]:F4}, {min[1]:F4})");
        }

        // Hydrogen atom (fixed bracket)
        Console.WriteLine("\nHydrogen atom ground state (shooting method):");
        double E_low = -0.6;
        double E_high = -0.4;
        double M_low = hydrogenM(E_low);
        double M_high = hydrogenM(E_high);
        if (M_low * M_high > 0) throw new Exception("Failed to bracket the energy root");
        double E0 = root.bisection(hydrogenM, E_low, E_high, 1e-6);
        double check = hydrogenM(E0);
        Console.WriteLine($"Found E0 ≈ {E0:F6} Hartree");
        Console.WriteLine($"Check: f(r_max) at E0 = {check:+0.00e+00;-0.00e+00} (should be ~0)");
        Console.WriteLine("Exact ground state energy = -0.5 Hartree");
    }
}
