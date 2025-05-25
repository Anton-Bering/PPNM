using System;

public static class LSFit {
    // Perform least-squares fit with given functions and data.
    // Returns (c, Cov) where c is the best-fit coefficient vector and Cov is the covariance matrix of c.
    public static (vector c, matrix Cov) lsfit(Func<double, double>[] fs, vector x, vector y, vector dy) {
        int n = x.size;
        int m = fs.Length;
        if (y.size != n || dy.size != n) {
            throw new ArgumentException("Input vectors x, y, dy must have the same length.");
        }
        // Construct weighted design matrix A (n x m) and vector b (length n)
        matrix A = new matrix(n, m);
        vector b = new vector(n);
        for (int i = 0; i < n; i++) {
            double xi = x[i];
            double yi = y[i];
            double dyi = dy[i];
            if (dyi == 0) throw new ArgumentException($"dy[{i}] is zero, cannot weight data by uncertainty zero.");
            b[i] = yi / dyi;
            for (int k = 0; k < m; k++) {
                A[i, k] = fs[k](xi) / dyi;
            }
        }
        // QR-decomposition of A
        var (Q, R) = QR.decomp(A);
        // Solve for coefficients: A*c ≈ b
        vector c = QR.solve(Q, R, b);
        // Calculate covariance matrix: Cov = (R^{-1} * R^{-T})
        matrix Cov = new matrix(m, m);
        // We find R^{-1} by solving R * invCol = I_col for each column of identity
        matrix Rinv = new matrix(m, m);
        for (int j = 0; j < m; j++) {
            // Solve R * u = e_j (unit vector) for u (back-substitution since R is upper-triangular)
            for (int i = m - 1; i >= 0; i--) {
                double sum = (i == j) ? 1.0 : 0.0;
                for (int k = i + 1; k < m; k++) {
                    sum -= R[i, k] * Rinv[k, j];
                }
                Rinv[i, j] = sum / R[i, i];
            }
        }
        // Now compute Cov = Rinv * (Rinv)^T
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                double sum = 0;
                for (int k = 0; k < m; k++) {
                    sum += Rinv[i, k] * Rinv[j, k];
                }
                Cov[i, j] = sum;
            }
        }
        return (c, Cov);
    }
}
