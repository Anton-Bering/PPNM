using System;
using static System.Math;
using System.Collections.Generic;
public static class ODESolver {
    // Return a function representing the ODE system for a given energy E
    public static Func<double, vector, vector> HydrogenODE(double E) {
        // The system: y1 = f, y2 = f'
        // y1' = y2
        // y2' = -2 * (E + 1/r) * y1   (from Schrödinger equation)
        return (double r, vector y) => {
            double f = y[0];
            double fp = y[1];
            double fprime = fp;
            double fpprime = -2 * (E + 1.0 / r) * f;
            return new vector(fprime, fpprime);
        };
    }

    // Adaptive RK4 integration with step-size control
    // Integrate the ODE y' = f(x, y) from x=a to x=b, starting with y(a)=ya.
    // acc = absolute error tolerance, eps = relative error tolerance.
    // Optionally, collects points in rList and fList for output (r vs f(r)).
    public static vector Integrate(Func<double, vector, vector> f, double a, vector ya, double b, double acc, double eps,
                                   List<double> rList = null, List<double> fList = null) {
        double x = a;
        vector y = ya.copy();
        if (rList != null && fList != null) {
            rList.Add(x);
            fList.Add(y[0]);
        }
        double h = (b - a) / 16.0;  // initial step guess
        while (x < b) {
            if (x + h > b) h = b - x;  // do not overshoot end point
            // Single step (full step) with step size h
            vector y_full = StepRK4(f, x, y, h);
            // Two half-steps with step size h/2
            vector y_half = StepRK4(f, x, y, h / 2.0);
            y_half = StepRK4(f, x + h / 2.0, y_half, h / 2.0);
            // Estimate error = difference between two methods
            vector err = y_half - y_full;
            // Compute tolerance for this step (component-wise)
            double maxRelError = 0;
            for (int i = 0; i < y.size; i++) {
                double yi = y_half[i];
                double tol = acc + eps * Abs(yi);
                double err_i = Abs(err[i]);
                // relative error ratio for this component:
                double relError = err_i / tol;
                if (relError > maxRelError) maxRelError = relError;
            }
            if (maxRelError <= 1) {
                // Accept step
                x += h;
                y = y_half;
                if (rList != null && fList != null) {
                    rList.Add(x);
                    fList.Add(y[0]);
                }
            }
            // Adjust step size for next iteration
            // Scale h by factor ~ (1/maxRelError)^(1/4) for RK4, with safety factors
            double factor = 0.95 * Pow(maxRelError > 1e-8 ? 1.0 / maxRelError : 1e8, 0.25);
            if (factor < 0.2) factor = 0.2;
            if (factor > 5) factor = 5;
            h *= factor;
        }
        return y;
    }

    // Perform one Runge-Kutta 4 step (order 4)
    private static vector StepRK4(Func<double, vector, vector> f, double x, vector y, double h) {
        vector k1 = f(x, y);
        vector k2 = f(x + h / 2.0, y + k1 * (h / 2.0));
        vector k3 = f(x + h / 2.0, y + k2 * (h / 2.0));
        vector k4 = f(x + h, y + k3 * h);
        vector y_new = y + (k1 + 2 * k2 + 2 * k3 + k4) * (h / 6.0);
        return y_new;
    }
}
