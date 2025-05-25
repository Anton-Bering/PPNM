using System;

public static class root {
    // Compute numerical Jacobian matrix J_ij = d f_i / d x_j at point x
    private static matrix jacobian(Func<vector, vector> f, vector x, vector fx = null, vector dx = null) {
        int n = x.size;
        if (fx == null) fx = f(x);
        if (dx == null) {
            // Choose step sizes: dx_i = max(|x_i|, 1) * 2^-26 (approximately sqrt(machine epsilon)
            dx = x.map(xi => Math.Max(Math.Abs(xi), 1.0) * Math.Pow(2.0, -26));
        }
        matrix J = new matrix(n, n);
        for (int j = 0; j < n; j++) {
            double old_xj = x[j];
            x[j] = old_xj + dx[j];             // increment x_j
            vector f_dx = f(x);
            // Compute difference f(x + dx_j*e_j) - f(x)
            vector df = f_dx - fx;
            for (int i = 0; i < n; i++) {
                J[i, j] = df[i] / dx[j];
            }
            x[j] = old_xj;                     // restore x_j
        }
        return J;
    }

    // Newton-Raphson method for solving f(x) = 0 (vector function), with backtracking line-search.
    public static vector newton(Func<vector, vector> f, vector start, double acc = 1e-2, vector dx = null) {
        vector x = start.copy();
        if (dx == null) {
            // Initialize dx for Jacobian if not provided
            dx = x.map(xi => Math.Max(Math.Abs(xi), 1.0) * Math.Pow(2.0, -26));
        }
        vector fx = f(x);
        int iter = 0;
        while (true) {
            if (fx.norm() < acc) break;  // convergence: f(x) is close enough to 0
            matrix J = jacobian(f, x, fx, dx);
            // Solve J * step = -f(x) for Newton step
            QR qrJ = new QR(J);
            vector step = qrJ.solve(-fx);
            // Backtracking line-search for step length
            double lambda = 1.0;
            vector x_new, f_new;
            // Save current x to compute actual step taken
            vector x_old = x.copy();
            while (true) {
                x_new = x_old + lambda * step;
                f_new = f(x_new);
                // Armijo condition: sufficient decrease in ||f||
                if (f_new.norm() < (1 - 0.5 * lambda) * fx.norm()) {
                    break;  // sufficient decrease achieved
                }
                if (lambda < 1.0/64) {
                    // If step gets too small, accept this step (or break out)
                    break;
                }
                lambda /= 2;
            }
            // Update x and f(x)
            x = x_new;
            fx = f_new;
            iter++;
            // Stop if the step size becomes very small relative to dx (no further progress)
            bool smallStep = true;
            for (int i = 0; i < x.size; i++) {
                if (Math.Abs(x_new[i] - x_old[i]) > Math.Abs(dx[i])) {
                    smallStep = false;
                    break;
                }
            }
            if (smallStep) break;
            // (Optional: could add a max iteration guard to avoid infinite loop in pathological cases)
        }
        return x;
    }

    // Bisection method to find a root of a 1D function f in the interval [a,b] 
    // (assumes f(a) and f(b) have opposite signs).
    public static double bisection(Func<double, double> f, double a, double b, double tol = 1e-6) {
        double fa = f(a), fb = f(b);
        if (fa * fb > 0) {
            throw new ArgumentException("Bisection requires f(a) and f(b) to have opposite signs");
        }
        double mid = 0, fmid;
        int maxIter = 1000;
        int iter = 0;
        while ((b - a) > tol && iter < maxIter) {
            mid = (a + b) / 2.0;
            fmid = f(mid);
            if (fmid == 0) {
                // Found exact root
                a = b = mid;
                break;
            }
            // Narrow the bracket to the sub-interval where sign change occurs
            if (fa * fmid < 0) {
                b = mid;
                fb = fmid;
            } else {
                a = mid;
                fa = fmid;
            }
            iter++;
        }
        return (a + b) / 2.0;
    }
}
