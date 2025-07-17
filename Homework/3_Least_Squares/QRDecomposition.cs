using System;

public static class QR
{
    /* -------------------------------------------------------------
     * Classical Gram–Schmidt QR decomposition
     * Returns Q (m×n, orthonormal columns) and R (n×n upper‑triangular)
     * ------------------------------------------------------------- */
    public static (double[,] Q, double[,] R) Decompose(double[,] A)
    {
        int m = A.GetLength(0), n = A.GetLength(1);
        var Q = new double[m, n];
        var R = new double[n, n];

        /* --- copy A into working array Q --- */
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                Q[i, j] = A[i, j];

        for (int k = 0; k < n; k++)
        {
            /* subtract earlier projections */
            for (int j = 0; j < k; j++)
            {
                double r = 0.0;
                for (int i = 0; i < m; i++) r += Q[i, j] * Q[i, k];
                R[j, k] = r;
                for (int i = 0; i < m; i++) Q[i, k] -= r * Q[i, j];
            }

            /* normalise k‑th column */
            double norm = 0.0;
            for (int i = 0; i < m; i++) norm += Q[i, k] * Q[i, k];
            norm = Math.Sqrt(norm);
            if (norm == 0.0) throw new ArgumentException("Linearly dependent columns");

            R[k, k] = norm;
            for (int i = 0; i < m; i++) Q[i, k] /= norm;
        }
        return (Q, R);
    }

    /* -------------------------------------------------------------
     * Solve  R x  =  Qᵀ b  (least‑squares solution to A x ≈ b)
     * ------------------------------------------------------------- */
    public static double[] Solve(double[,] Q, double[,] R, double[] b)
    {
        double[] Qtb = VectorAndMatrix.Multiply(
                           VectorAndMatrix.Transpose(Q), b);

        int n = R.GetLength(1);
        var x = new double[n];

        for (int i = n - 1; i >= 0; i--)
        {
            double s = Qtb[i];
            for (int j = i + 1; j < n; j++) s -= R[i, j] * x[j];
            x[i] = s / R[i, i];
        }
        return x;
    }
}
