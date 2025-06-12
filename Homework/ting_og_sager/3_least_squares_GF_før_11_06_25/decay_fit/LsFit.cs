using System;

public static class LsFit {
    /// <summary>
    /// Perform a weighted least-squares fit of data (x,y,dy) to a linear combination of functions {f_k}.
    /// Returns a tuple (c, cov) where c[k] are best-fit coefficients and cov is the covariance matrix.
    /// </summary>
    public static (double[] c, double[,] cov) Fit(Func<double, double>[] fs, double[] x, double[] y, double[] dy) {
        int n = x.Length;
        int m = fs.Length;
        // Construct weighted design matrix A (n x m) and weighted vector b (length n)
        double[,] A = new double[n, m];
        double[] b = new double[n];
        for (int i = 0; i < n; i++) {
            double wi = 1.0 / dy[i];            // weight = 1/σ for point i
            b[i] = y[i] * wi;                   // weighted target
            for (int k = 0; k < m; k++) {
                A[i, k] = fs[k](x[i]) * wi;     // A_ik = f_k(x_i)/σ_i
            }
        }
        // QR decomposition of A (n×m) via Gram-Schmidt
        double[,] Q = new double[n, m];
        double[,] R = new double[m, m];
        for (int j = 0; j < m; j++) {
            // Copy column j of A into v
            for (int i = 0; i < n; i++) {
                Q[i, j] = A[i, j];
            }
            // Subtract projections onto previous q-columns
            for (int i = 0; i < j; i++) {
                double dot = 0.0;
                for (int k = 0; k < n; k++) {
                    dot += Q[k, i] * Q[k, j];
                }
                R[i, j] = dot;
                for (int k = 0; k < n; k++) {
                    Q[k, j] -= dot * Q[k, i];
                }
            }
            // Normalize column j
            double norm = 0.0;
            for (int k = 0; k < n; k++) {
                norm += Q[k, j] * Q[k, j];
            }
            norm = Math.Sqrt(norm);
            R[j, j] = norm;
            if (norm < 1e-12) {
                throw new Exception("QR decomposition failed: matrix rank deficient or nearly so.");
            }
            for (int k = 0; k < n; k++) {
                Q[k, j] /= norm;
            }
        }
        // Solve R * c = Q^T * b for coefficients c (back-substitution since R is upper-triangular)
        double[] c = new double[m];
        double[] Qt_b = new double[m];
        for (int j = 0; j < m; j++) {
            double sum = 0.0;
            for (int i = 0; i < n; i++) {
                sum += Q[i, j] * b[i];
            }
            Qt_b[j] = sum;
        }
        for (int i = m - 1; i >= 0; i--) {
            double sum = Qt_b[i];
            for (int j = i + 1; j < m; j++) {
                sum -= R[i, j] * c[j];
            }
            c[i] = sum / R[i, i];
        }
        // Compute covariance matrix: cov = (A^T A)^{-1} = R^{-1} (R^{-1})^T:contentReference[oaicite:11]{index=11}
        double[,] Rinv = new double[m, m];
        // Invert upper-triangular R to get Rinv
        for (int i = 0; i < m; i++) {
            Rinv[i, i] = 1.0 / R[i, i];
            for (int k = i - 1; k >= 0; k--) {
                double sum = 0.0;
                for (int j = k + 1; j <= i; j++) {
                    sum += R[k, j] * Rinv[j, i];
                }
                Rinv[k, i] = -sum / R[k, k];
            }
        }
        // Compute Cov = Rinv * (Rinv)^T
        double[,] cov = new double[m, m];
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                double sum = 0.0;
                for (int k = 0; k < m; k++) {
                    sum += Rinv[i, k] * Rinv[j, k];
                }
                cov[i, j] = sum;
            }
        }
        return (c, cov);
    }
}
