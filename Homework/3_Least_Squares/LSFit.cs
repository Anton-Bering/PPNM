using System;

public static class LSFit
{
    /* ---------------------------------------------------------
     * Ordinary least‑squares with uncertainties:
     *   y_i = Σ_k c_k f_k(x_i)    ,   σ_i = dy[i]
     * Returns best‑fit parameter vector c and covariance matrix
     * --------------------------------------------------------- */
    public static (Vector c, Matrix Cov) Fit(
        Func<double, double>[] fs,
        Vector x, Vector y, Vector dy)
    {
        int n = x.Size;
        int m = fs.Length;

        // Build design matrix A and weighted RHS b
        var A = new Matrix(n, m);
        var b = new Vector(n);

        for (int i = 0; i < n; i++)
        {
            double w = 1.0 / dy[i];
            b[i] = y[i] * w;
            for (int k = 0; k < m; k++) A[i, k] = w * fs[k](x[i]);
        }

        var (Q, R) = QR.Decompose(A);
        Vector c = QR.Solve(Q, R, b);

        /* -------- covariance -------- */
        Vector r = y - Evaluate(fs, c, x);
        double chi2 = 0;
        for (int i = 0; i < n; i++) chi2 += Math.Pow(r[i] / dy[i], 2);
        double sigma2 = chi2 / (n - m);               // reduced χ²

        Matrix Rinv = InvertUpper(R);
        Matrix Cov = sigma2 * (Rinv * Rinv.T);
        return (c, Cov);
    }

    public static Vector Evaluate(Func<double, double>[] fs, Vector c, Vector x)
    {
        var y = new Vector(x.Size);
        for (int i = 0; i < x.Size; i++)
        {
            double s = 0;
            for (int k = 0; k < fs.Length; k++) s += c[k] * fs[k](x[i]);
            y[i] = s;
        }
        return y;
    }

    /* ------------- helper: invert upper‑triangular matrix ------------- */
    private static Matrix InvertUpper(Matrix U)
    {
        int n = U.Rows;
        var X = new Matrix(n, n);

        for (int i = n - 1; i >= 0; i--)
        {
            X[i, i] = 1 / U[i, i];
            for (int j = i - 1; j >= 0; j--)
            {
                double s = 0;
                for (int k = j + 1; k <= i; k++) s += U[j, k] * X[k, i];
                X[j, i] = -s / U[j, j];
            }
        }
        return X;
    }
}
