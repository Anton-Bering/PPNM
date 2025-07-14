using System;
using static System.Math;
public static class RootFind {
    public static vector newton(Func<vector, vector> f, vector start, double acc = 1e-2, vector dx = null) {
        vector x = start.copy();
        int n = x.size;
        // If a δx vector is provided, copy it; otherwise we will compute dynamic δ
        vector delta = dx != null ? dx.copy() : null;
        vector fx = f(x);
        // Allocate Jacobian matrix once
        matrix J = new matrix(n, n);
        const double lambda_min = 1.0 / 64;
        int iterations = 0;
        while (true) {
            double fx_norm = fx.norm();
            if (fx_norm < acc) break;  // convergence achieved
            // Determine δx for numerical Jacobian (if not given, use |x| or 1 times 2^-26)
            if (dx == null) {
                delta = x.map(xi => Abs(xi) > 1 ? Abs(xi) * Pow(2, -26) : Pow(2, -26));
            }
            // Numerical Jacobian via finite differences
            for (int j = 0; j < n; j++) {
                double xj = x[j];
                x[j] = xj + delta[j];
                vector df = f(x) - fx;
                for (int i = 0; i < n; i++) {
                    J[i, j] = df[i] / delta[j];
                }
                x[j] = xj;  // restore x[j]
            }
            // Solve J * Δx = -f(x)  (using QR decomposition)
            var QR = new QRDecomposition(J);
            vector Dx = QR.solve(-1 * fx);
            // If step is negligible (Δx smaller than δx), break to avoid stagnation
            if (Dx.norm() < delta.norm()) break;
            double lambda = 1.0;
            // Compute candidate new point with full step
            vector z = x + Dx;
            vector fz = f(z);
            double fz_norm = fz.norm();
            if (fz_norm < (1 - 0.5) * fx_norm) {
                // Full step yields sufficient decrease, accept it
                x = z;
                fx = fz;
            } else {
                // Try a half step as next attempt
                lambda = 0.5;
                vector z_half = x + lambda * Dx;
                vector f_half_vec = f(z_half);
                double f_half_norm = f_half_vec.norm();
                if (f_half_norm < (1 - 0.25) * fx_norm) {
                    // Half step yields sufficient decrease, accept it
                    x = z_half;
                    fx = f_half_vec;
                } else {
                    // Quadratic interpolation: approximate optimal λ by fitting a parabola to (λ, ||f||)
                    double f0 = fx_norm;
                    double f1 = fz_norm;       // at λ = 1
                    double fh = f_half_norm;   // at λ = 0.5
                    // Compute quadratic coefficients A λ^2 + B λ + C; C = f0, so:
                    double A = -4 * (fh - 0.5 * (f1 + f0));
                    double B = 2 * (fh - f0) - 0.5 * A;
                    double lam_opt = (Abs(A) < 1e-12) ? 0.5 : -B / (2 * A);
                    if (lam_opt < lambda_min) lam_opt = lambda_min;
                    if (lam_opt > 0.5) lam_opt = 0.5;
                    // Evaluate f at the interpolated lambda
                    vector z_opt = x + lam_opt * Dx;
                    vector f_opt_vec = f(z_opt);
                    double f_opt_norm = f_opt_vec.norm();
                    // Choose the step (half vs. interpolated) that gives the larger decrease in ||f||
                    if (f_opt_norm < f_half_norm) {
                        x = z_opt;
                        fx = f_opt_vec;
                    } else {
                        x = z_half;
                        fx = f_half_vec;
                    }
                }
            }
            iterations++;
            if (iterations > 1000) {
                Console.Error.WriteLine("Newton: did not converge within 1000 iterations.");
                break;
            }
        }
        return x;
    }
}
