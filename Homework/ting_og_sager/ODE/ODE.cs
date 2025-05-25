using System;
using System.Collections.Generic;
public static class ODE {
    // (A) Embedded Runge-Kutta stepper of order 1-2 (Euler midpoint method)
    public static (vector, vector) rkstep12(Func<double, vector, vector> f, 
                                           double x, vector y, double h) {
        vector k0 = f(x, y);                    // Euler (1st order estimate)
        vector k1 = f(x + h/2, y + k0 * (h/2));  // Midpoint (2nd order estimate)
        vector yh = y + k1 * h;                 // y(x+h) higher-order result
        vector err = (k1 - k0) * h;             // error estimate = (k1 - k0)*h
        return (yh, err);
    }

    // (C1) Embedded Runge-Kutta stepper of order 2-3 (e.g. Bogacki-Shampine 3(2) method)
    public static (vector, vector) rkstep23(Func<double, vector, vector> f,
                                           double x, vector y, double h) {
        // Stage 1
        vector k1 = f(x, y);
        // Stage 2
        vector k2 = f(x + 0.5 * h, y + 0.5 * k1 * h);
        // Stage 3
        vector k3 = f(x + 0.75 * h, y + 0.75 * k2 * h);
        // 3rd-order (higher order) solution
        vector yh = y + (2.0/9.0 * k1 + 1.0/3.0 * k2 + 4.0/9.0 * k3) * h;
        // 2nd-order (lower order) solution for error estimate
        vector y_low = y + (7.0/24.0 * k1 + 1.0/4.0 * k2 + 1.0/3.0 * k3) * h;
        vector err = yh - y_low;
        return (yh, err);
    }

    // (A) Adaptive step-size driver using an embedded stepper (default is rkstep12)
    public delegate (vector, vector) Stepper(Func<double, vector, vector> f, double x, vector y, double h);
    public static (genlist<double>, genlist<vector>) driver(Func<double, vector, vector> F,
                                                            (double, double) interval,
                                                            vector yinit,
                                                            double h = 0.125, 
                                                            double acc = 0.01, 
                                                            double eps = 0.01,
                                                            Stepper stepper = null) {
        double a = interval.Item1, b = interval.Item2;
        double x = a;
        vector y = yinit.copy();
        if(stepper == null) stepper = rkstep12;           // default stepper is RK12
        var xlist = new genlist<double>(); xlist.add(x);
        var ylist = new genlist<vector>(); ylist.add(y.copy());
        while(true) {
            if(x >= b) break;                            // done
            if(x + h > b) h = b - x;                      // adjust last step to end exactly at b
            var (yh, err) = stepper(F, x, y, h);
            double tol = (acc + eps * yh.norm()) * Math.Sqrt(h / (b - a));
            double err_norm = err.norm();
            if(err_norm <= tol) {
                // Accept step
                x += h; 
                y = yh;
                xlist.add(x);
                ylist.add(y.copy());
            }
            // Adjust step-size for next step
            if(err_norm > 0) {
                double factor = Math.Pow(tol / err_norm, 0.25) * 0.95;
                if(factor > 2) factor = 2;
                h *= factor;
            } else {
                // If error is zero (machine precision reached), increase step
                h *= 2;
            }
        }
        return (xlist, ylist);
    }

    // Binary search to find the interval index i such that x[i] <= z < x[i+1]
    public static int binsearch(genlist<double> xs, double z) {
        if(z < xs[0] || z > xs[xs.Count-1]) 
            throw new Exception("binsearch: z out of bounds");
        int i = 0, j = xs.Count - 1;
        while(j - i > 1) {
            int mid = (i + j) / 2;
            if(z > xs[mid]) i = mid; else j = mid;
        }
        return i;
    }

    // (C2) Construct a quadratic spline interpolation function for a table of points {x_i, y_i}
    public static Func<double, vector> make_qspline(genlist<double> x, genlist<vector> y) {
        int n = x.Count;
        // Spline coefficients b[i] and c[i] for each interval [x[i], x[i+1]]
        vector[] b = new vector[n-1];
        vector[] c = new vector[n-1];
        // Precompute interval lengths and differences Δy
        double[] h = new double[n-1];
        vector[] Delta = new vector[n-1];
        for(int i = 0; i < n-1; i++) {
            h[i] = x[i+1] - x[i];
            Delta[i] = y[i+1] - y[i];
        }
        // Natural end condition: second derivative at last interval = 0
        c[n-2] = new vector(y[0].size);  // c[last] = 0 (vector of zeros)
        for(int comp = 0; comp < y[0].size; comp++) {
            c[n-2][comp] = 0.0;
        }
        // Compute c[i] coefficients backwards for i = n-2, n-3, ..., 0
        for(int j = n-2; j >= 1; j--) {
            // R_j = Δy_j - Δy_{j-1} * (h_j / h_{j-1})
            vector Rj = Delta[j] - Delta[j-1] * (h[j] / h[j-1]);
            // c[j-1] = (R_j - c_j * h_j^2) / (h_{j-1} * h[j])
            c[j-1] = (Rj - c[j] * (h[j] * h[j])) * (1.0 / (h[j-1] * h[j]));
        }
        // Compute b[i] coefficients for each interval
        for(int i = 0; i < n-1; i++) {
            if(c[i] == null) c[i] = new vector(y[0].size);  // ensure not null
            b[i] = (Delta[i] - c[i] * (h[i] * h[i])) * (1.0 / h[i]);
        }
        // Return a function that evaluates the spline s(z)
        return delegate(double z) {
            int i = binsearch(x, z);
            double dx = z - x[i];
            // s_i(z) = y_i + b_i * dx + c_i * dx^2
            return y[i] + (b[i] + c[i] * dx) * dx;
        };
    }

    // (C3) Alternative driver interface: returns a quadratic spline function instead of raw table
    public static Func<double, vector> make_ode_ivp_qspline(Func<double, vector, vector> F,
                                                            (double,double) interval,
                                                            vector y,
                                                            double acc = 0.01,
                                                            double eps = 0.01,
                                                            double hstart = 0.01) {
        var (xlist, ylist) = driver(F, interval, y, hstart, acc, eps);
        return make_qspline(xlist, ylist);
    }
}
