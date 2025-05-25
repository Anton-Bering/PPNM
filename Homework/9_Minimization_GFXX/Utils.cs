using System;

public static class Utils {
    // Compute the Euclidean norm of a vector
    public static double Norm(double[] v) {
        double sum = 0.0;
        for (int i = 0; i < v.Length; i++) {
            sum += v[i] * v[i];
        }
        return Math.Sqrt(sum);
    }

    // Add two vectors: result = x + scale * dx
    public static double[] Add(double[] x, double[] dx, double scale = 1.0) {
        int n = x.Length;
        double[] z = new double[n];
        for (int i = 0; i < n; i++) {
            z[i] = x[i] + scale * dx[i];
        }
        return z;
    }

    // Numerical gradient of function f at vector x.
    // Uses forward difference by default, or central difference if centralDiff is true.
    public static double[] Gradient(Func<double[], double> f, double[] x, bool centralDiff = false) {
        int n = x.Length;
        double[] grad = new double[n];
        if (!centralDiff) {
            // Forward difference gradient
            double fx = f(x);
            for (int i = 0; i < n; i++) {
                double xi = x[i];
                // Step size: (1 + |x_i|) * 2^(-26)
                double dx = (1 + Math.Abs(xi)) * Math.Pow(2, -26);
                x[i] = xi + dx;
                double fx_dx = f(x);
                grad[i] = (fx_dx - fx) / dx;
                x[i] = xi; // restore original value
            }
        } else {
            // Central difference gradient
            for (int i = 0; i < n; i++) {
                double xi = x[i];
                double dx = (1 + Math.Abs(xi)) * Math.Pow(2, -26);
                x[i] = xi + dx;
                double f_plus = f(x);
                x[i] = xi - dx;
                double f_minus = f(x);
                x[i] = xi; // restore original
                grad[i] = (f_plus - f_minus) / (2 * dx);
            }
        }
        return grad;
    }

    // Numerical Hessian matrix of function f at vector x.
    // Uses forward difference by default, or central difference if centralDiff is true.
    public static double[,] Hessian(Func<double[], double> f, double[] x, bool centralDiff = false) {
        int n = x.Length;
        double[,] H = new double[n, n];
        if (!centralDiff) {
            // Forward difference Hessian
            double[] g = Gradient(f, x, false); // gradient at original x
            for (int j = 0; j < n; j++) {
                double xj = x[j];
                double dx = (1 + Math.Abs(xj)) * Math.Pow(2, -13);
                x[j] = xj + dx;
                double[] g_plus = Gradient(f, x, false);
                x[j] = xj; // restore
                for (int i = 0; i < n; i++) {
                    H[i, j] = (g_plus[i] - g[i]) / dx;
                }
            }
        } else {
            // Central difference Hessian
            for (int j = 0; j < n; j++) {
                double xj = x[j];
                double dx = (1 + Math.Abs(xj)) * Math.Pow(2, -13);
                // Gradient at x + dx_j
                x[j] = xj + dx;
                double[] g_plus = Gradient(f, x, true);
                // Gradient at x - dx_j
                x[j] = xj - dx;
                double[] g_minus = Gradient(f, x, true);
                // Restore x_j
                x[j] = xj;
                // Hessian column j
                for (int i = 0; i < n; i++) {
                    H[i, j] = (g_plus[i] - g_minus[i]) / (2 * dx);
                }
            }
        }
        return H;
    }

    // Solve linear system A * x = b (for x), where A is n×n. Uses Gaussian elimination with partial pivoting.
    public static double[] SolveLinear(double[,] A, double[] b) {
        int n = b.Length;
        // Augmented matrix [A|b]
        double[,] M = new double[n, n + 1];
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                M[i, j] = A[i, j];
            }
            M[i, n] = b[i];
        }
        // Gaussian elimination
        for (int k = 0; k < n; k++) {
            // Partial pivot: find row with max |M[i,k]| for i >= k
            int pivot = k;
            double maxA = Math.Abs(M[k, k]);
            for (int i = k + 1; i < n; i++) {
                double absA = Math.Abs(M[i, k]);
                if (absA > maxA) {
                    pivot = i;
                    maxA = absA;
                }
            }
            if (Math.Abs(M[pivot, k]) < 1e-12) {
                throw new Exception("Matrix is singular or nearly singular.");
            }
            // Swap pivot row with current row k
            if (pivot != k) {
                for (int j = 0; j < n + 1; j++) {
                    double temp = M[k, j];
                    M[k, j] = M[pivot, j];
                    M[pivot, j] = temp;
                }
            }
            // Eliminate below pivot
            for (int i = k + 1; i < n; i++) {
                double factor = M[i, k] / M[k, k];
                for (int j = k; j < n + 1; j++) {
                    M[i, j] -= factor * M[k, j];
                }
            }
        }
        // Back-substitution
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--) {
            double sum = 0.0;
            for (int j = i + 1; j < n; j++) {
                sum += M[i, j] * x[j];
            }
            x[i] = (M[i, n] - sum) / M[i, i];
        }
        return x;
    }

    // Backtracking line search: find step length λ that yields a decrease in f.
    public static double LineSearch(Func<double[], double> f, double[] x, double[] dx) {
        double fx = f(x);
        double lambda = 1.0;
        while (lambda >= 1.0/1024) {
            double[] x_new = Add(x, dx, lambda);
            double f_new = f(x_new);
            if (f_new < fx) break;
            lambda /= 2.0;
        }
        return lambda;
    }
}
