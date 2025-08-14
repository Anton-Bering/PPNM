using System;
// CHANGED: vi bruger dine helpers/typer
using static MatrixHelpers;

public static class LSFit
{
    /* ---------------------------------------------------------
     * Ordinary least‑squares with uncertainties:
     *   y_i = Σ_k c_k f_k(x_i) ,   σ_i = dy[i]
     * Returns (best‑fit parameter vector c, covariance matrix Cov)
     * --------------------------------------------------------- */
    // CHANGED: c er vector (ikke double[]), Cov er matrix (ikke double[,])
    public static (vector c, matrix Cov) Fit(
        Func<double, double>[] fs,
        double[] x, double[] y, double[] dy)
    {
        int n = x.Length;
        int m = fs.Length;

        /* ----------- build design matrix A and weighted RHS b ----------- */
        // CHANGED: A som matrix, b som vector
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
        // CHANGED: brug din QR.Solve(b) (hvis din metode hedder solve, så skift til qr.solve(b))
        vector c = qr.solve(b);
        matrix Q = qr.Q;
        matrix R = qr.R;

        /* ----------- covariance matrix ---------------------------------- */
        // CHANGED: Evaluate tager vector c og returnerer double[]
        double[] yFit = Evaluate(fs, c, x);
        double chi2 = 0.0;
        for (int i = 0; i < n; i++)
        {
            double diff = y[i] - yFit[i];
            chi2 += diff * diff / (dy[i] * dy[i]);
        }
        double sigma2 = chi2 / (n - m);          // reduced χ²

        // CHANGED: væk med VectorAndMatrix.* — brug matrix-API + Transpose helper
        matrix Rinv = InvertUpper(R);
        matrix Cov  = Rinv * Transpose(Rinv);    // (R^{-1})(R^{-1})^T
        ScaleInPlace(Cov, sigma2);               // Cov *= sigma2

        return (c, Cov);
    }

    // CHANGED: c er vector (ikke double[])
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
    // CHANGED: matrix-version (ikke double[,])
    private static matrix InvertUpper(matrix U)
    {
        int n = U.Rows;
        if (U.Cols != n) throw new ArgumentException("U skal være kvadratisk og øvre trekantet.");
        var X = new matrix(n, n);

        // Løs kolonne for kolonne: U * X[:,j] = e_j (back-substitution)
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

    /* ------------- helper: scale matrix in place ---------------------- */
    // CHANGED: in-place skalering for matrix
    private static void ScaleInPlace(matrix A, double factor)
    {
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                A[i, j] *= factor;
    }
}