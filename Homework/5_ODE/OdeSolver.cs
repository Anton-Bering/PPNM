using System;
using System.Collections.Generic;
using static System.Math;

public static class OdeSolver {
    // Delegate type for an embedded stepper function
    public delegate (vector, vector) StepperFunc(Func<double, vector, vector> f,
                                                double x, vector y, double h);

    // Adaptive step-size driver using an embedded stepper
    public static (List<double>, List<vector>) driver(Func<double, vector, vector> F,
                                                     double a, vector ya, double b,
                                                     double acc = 0.01, double eps = 0.01,
                                                     double h = 0.01, StepperFunc stepper = null) {
        if (a > b) throw new ArgumentException("driver: a > b");
        if (stepper == null) stepper = RKStepper.rkstep12;  // default to RK12
        double x = a;
        vector y = ya.Copy();
        var xlist = new List<double>(); xlist.Add(x);
        var ylist = new List<vector>(); ylist.Add(y);
        while (true) {
            if (x >= b) break;                 // done
            if (x + h > b) h = b - x;          // trim last step to end at b
            var (yh, err) = stepper(F, x, y, h);
            double tol = (acc + eps * yh.Norm()) * Sqrt(h / (b - a));
            double errNorm = err.Norm();
            if (errNorm <= tol) {
                // Accept step
                x += h;
                y = yh;
                xlist.Add(x);
                ylist.Add(y);
            }
            // Adjust step size for next iteration
            if (errNorm > 0) {
                h *= Min(Pow(tol / errNorm, 0.25) * 0.95, 2);
            } else {
                h *= 2;
            }
        }
        return (xlist, ylist);
    }

    // Binary search helper for interpolation
    public static int binsearch(List<double> xs, double z) {
        int i = 0, j = xs.Count - 1;
        if (z < xs[0] || z > xs[j]) throw new ArgumentException("binsearch: z out of range");
        while (j - i > 1) {
            int mid = (i + j) / 2;
            if (xs[mid] <= z) i = mid; else j = mid;
        }
        return i;
    }

    // Construct a quadratic spline interpolation from given (x, y) data
    public static Func<double, vector> make_qspline(List<double> xs, List<vector> ys) {
        int n = xs.Count;
        if (n < 2) throw new ArgumentException("Need >=2 points");
        // Arrays of coefficients b[i], c[i] for each interval
        vector[] b = new vector[n];
        vector[] c = new vector[n];
        int last = n - 2;
        c[last] = new vector(ys[last].Size);  // c_last = 0 vector
        b[last] = (ys[last + 1] - ys[last]) * (1.0 / (xs[last + 1] - xs[last]));
        // Backward recursion for other segments
        for (int i = n - 3; i >= 0; i--) {
            double h_i = xs[i+1] - xs[i];
            b[i] = (ys[i+1] - ys[i]) * (2.0 / h_i) - b[i+1];
            c[i] = (ys[i+1] - ys[i] - b[i] * h_i) * (1.0 / (h_i * h_i));
        }
        // Spline function: returns interpolated y at z
        Func<double, vector> qspline = (double z) => {
            int i = binsearch(xs, z);
            double hz = z - xs[i];
            return ys[i] + b[i] * hz + c[i] * (hz * hz);
        };
        return qspline;
    }

    // Adaptive driver returning a quadratic spline of the solution (instead of raw lists)
    public static Func<double, vector> make_ode_ivp_qspline(Func<double, vector, vector> F,
                                                           double a, vector ya, double b,
                                                           double acc = 0.01, double eps = 0.01,
                                                           double h = 0.01, StepperFunc stepper = null) {
        var (xs, ys) = driver(F, a, ya, b, acc, eps, h, stepper);
        return make_qspline(xs, ys);
    }
}
