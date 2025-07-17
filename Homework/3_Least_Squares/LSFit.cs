using System;

public static class LSFit
{
    /* ---------------------------------------------------------
     * Ordinary least‑squares with uncertainties:
     *   y_i = Σ_k c_k f_k(x_i) ,   σ_i = dy[i]
     * Returns (best‑fit parameter vector c, covariance matrix Cov)
     * --------------------------------------------------------- */
    public static (double[] c, double[,] Cov) Fit(
        Func<double, double>[] fs,
        double[] x, double[] y, double[] dy)
    {
        int n = x.Length;
        int m = fs.Length;

        /* ----------- build design matrix A and weighted RHS b ----------- */
        var A = new double[n, m];
        var b = new double[n];

        for (int i = 0; i < n; i++)
        {
            double w = 1.0 / dy[i];
            b[i] = y[i] * w;
            for (int k = 0; k < m; k++) A[i, k] = w * fs[k](x[i]);
        }

        /* ----------- QR decomposition and normal‑equation solve ---------- */
        var (Q, R) = QR.Decompose(A);
        double[] c = QR.Solve(Q, R, b);

        /* ----------- covariance matrix ---------------------------------- */
        double[] yFit  = Evaluate(fs, c, x);
        double chi2 = 0.0;
        for (int i = 0; i < n; i++)
        {
            double diff = y[i] - yFit[i];
            chi2 += diff * diff / (dy[i] * dy[i]);
        }
        double sigma2 = chi2 / (n - m);          // reduced χ²

        double[,] Rinv = InvertUpper(R);
        double[,] Cov  = ScaleMatrix(
                             VectorAndMatrix.Multiply(Rinv,
                                                       VectorAndMatrix.Transpose(Rinv)),
                             sigma2);
        return (c, Cov);
    }

    public static double[] Evaluate(Func<double, double>[] fs,
                                    double[] c, double[] x)
    {
        int n = x.Length, m = fs.Length;
        var y = new double[n];
        for (int i = 0; i < n; i++)
        {
            double s = 0.0;
            for (int k = 0; k < m; k++) s += c[k] * fs[k](x[i]);
            y[i] = s;
        }
        return y;
    }

    /* ------------- helper: invert upper‑triangular matrix ------------- */
    private static double[,] InvertUpper(double[,] U)
    {
        int n = U.GetLength(0);
        var X = new double[n, n];

        for (int i = n - 1; i >= 0; i--)
        {
            X[i, i] = 1.0 / U[i, i];
            for (int j = i - 1; j >= 0; j--)
            {
                double s = 0.0;
                for (int k = j + 1; k <= i; k++) s += U[j, k] * X[k, i];
                X[j, i] = -s / U[j, j];
            }
        }
        return X;
    }

    /* ------------- helper: scale matrix by scalar --------------------- */
    private static double[,] ScaleMatrix(double[,] A, double factor)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        var B = new double[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                B[i, j] = factor * A[i, j];
        return B;
    }
}
