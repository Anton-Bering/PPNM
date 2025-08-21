using System;
using static MatrixHelpers;

public static class LSFit
{
    public static (vector c, matrix Cov) Fit(
        Func<double, double>[] fs,
        double[] x, double[] y, double[] dy)
    {
        int n = x.Length;
        int m = fs.Length;

        matrix A = new matrix(n, m);
        vector b = new vector(n);

        for (int i = 0; i < n; i++)
        {
            double w = 1.0 / dy[i];
            b[i] = y[i] * w;
            for (int k = 0; k < m; k++)
                A[i, k] = w * fs[k](x[i]);
        }

        /* ----------- QR decomposition and normal‑equation solve ---------- */
        var qr = new QR(A);
        vector c = qr.solve(b);
        matrix R = qr.R;

        /* ----------- covariance matrix ---------------------------------- */
        matrix Rinv = InvertUpper(R);
        matrix Cov  = Rinv * Transpose(Rinv);

        return (c, Cov);
    }

    public static double[] Evaluate(Func<double, double>[] fs,
                                    vector c, double[] x)
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
    private static matrix InvertUpper(matrix U)
    {
        int n = U.Rows;
        if (U.Cols != n) throw new ArgumentException("U skal være kvadratisk og øvre trekantet.");
        var X = new matrix(n, n);

        for (int j = 0; j < n; j++)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                double s = (i == j) ? 1.0 : 0.0;
                for (int k = i + 1; k < n; k++) s -= U[i, k] * X[k, j];
                X[i, j] = s / U[i, i];
            }
        }
        return X;
    }
}