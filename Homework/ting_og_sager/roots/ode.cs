using System;
using System.Collections.Generic;

public static class ode {
    // One step of an embedded Runge-Kutta (Euler = order 1, midpoint = order 2)
    public static (vector, vector) rkstep12(Func<double, vector, vector> f, 
                                           double x, vector y, double h) {
        vector k0 = f(x, y);                           // Euler slope
        vector k1 = f(x + h/2, y + k0 * (h/2));        // Midpoint slope
        vector yh = y + k1 * h;                        // higher-order step result
        vector err = (k1 - k0) * h;                    // error estimate = difference
        return (yh, err);
    }

    // Adaptive ODE driver from x=a to x=b with given absolute and relative tolerance
    public static vector driver(Func<double, vector, vector> f, 
                                double a, vector ya, double b, 
                                double acc = 1e-6, double eps = 1e-6, 
                                double h = 0.01, double hmin = 1e-8) {
        if (a >= b) throw new ArgumentException("Start point a must be less than b");
        double x = a;
        vector y = ya.copy();
        // Lists can be used to record the solution if needed (not required for final result)
        // List<double> xlist = new List<double>(); 
        // List<vector> ylist = new List<vector>();
        // xlist.Add(x); ylist.Add(y.copy());

        while (x < b) {
            if (x + h > b) h = b - x;  // last step to hit endpoint exactly
            var (yh, err) = rkstep12(f, x, y, h);
            double errNorm = err.norm();
            double tol = (acc + eps * yh.norm()) * Math.Sqrt(h / (b - a));
            if (errNorm <= tol) {
                // Accept step
                x += h;
                y = yh;
                // xlist.Add(x); ylist.Add(y.copy());
            }
            // Adjust step size for next iteration (increase if small error, decrease if error too large)
            double factor = 0.94 * Math.Pow(tol / (errNorm > 0 ? errNorm : tol), 0.25);
            if (factor > 2.0) factor = 2.0;
            if (errNorm > tol) {
                // Reject step and reduce step size
                h *= Math.Min(factor, 0.5);
                if (h < hmin) {
                    throw new Exception("Step size underflow (h too small)");
                }
                // Try the step again (continue loop without advancing x)
                continue;
            } else {
                // If step accepted, maybe increase step size for efficiency
                if (factor > 1.0) {
                    h *= factor;
                }
            }
        }
        return y;
        // If full solution path is needed:
        // return ylist[ylist.Count - 1];
    }
}
