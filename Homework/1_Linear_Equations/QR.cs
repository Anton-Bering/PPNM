using System;

/// <summary>
/// QR‑decomposition by modified Gram–Schmidt, rewritten to use the
/// static utility library <c>VectorAndMatrix</c> (double[,] / double[]).
/// </summary>
public sealed class QR
{
    public double[,] Q { get; }   // n×m (orthonormal columns)
    public double[,] R { get; }   // m×m (upper‑triangular)

    /*------------------------------------------------------------*/
    public QR(double[,] A)
    {
        int n = A.GetLength(0);
        int m = A.GetLength(1);
        if (n < m) throw new ArgumentException("QR decomposition requires n ≥ m");

        Q = (double[,])A.Clone();         // work‑copy of A
        R = new double[m, m];

        /* modified Gram–Schmidt ---------------------------------*/
        for (int j = 0; j < m; j++)
        {
            /* subtract projections on earlier orthonormal columns */
            for (int i = 0; i < j; i++)
            {
                double dot = 0.0;
                for (int k = 0; k < n; k++) dot += Q[k, i] * Q[k, j];
                R[i, j] = dot;
                for (int k = 0; k < n; k++) Q[k, j] -= dot * Q[k, i];
            }

            /* normalise current column -------------------------*/
            double norm = 0.0;
            for (int k = 0; k < n; k++) norm += Q[k, j] * Q[k, j];
            norm = Math.Sqrt(norm);
            if (norm == 0.0)
                throw new InvalidOperationException("Matrix has linearly dependent (or zero) columns.");

            R[j, j] = norm;
            for (int k = 0; k < n; k++) Q[k, j] /= norm;
        }
    }

    /*------------------------------------------------------------*/
    /// <summary>Solve <c>A x = b</c> where A == Q R.</summary>
    public double[] solve(double[] b)
    {
        int n = Q.GetLength(0);
        int m = R.GetLength(0);
        if (b.Length != n) throw new ArgumentException("Vector length must equal matrix row count.");

        /* y = Qᴴ b (Q is real orthogonal ⇒ Qᴴ == Qᵀ) */
        var y = new double[m];
        for (int i = 0; i < m; i++)
        {
            double sum = 0.0;
            for (int k = 0; k < n; k++) sum += Q[k, i] * b[k];
            y[i] = sum;
        }

        /* back‑substitution: R x = y ---------------------------*/
        return VectorAndMatrix.SolveUpperTriangular(R, y);
    }

    /*------------------------------------------------------------*/
    public double det()
    {
        /* For square A, det(A) = Π diag(R)  (because det(Q)=±1) */
        int m = R.GetLength(0);
        double p = 1.0;
        for (int i = 0; i < m; i++) p *= R[i, i];
        return p;
    }

    /*------------------------------------------------------------*/
    public double[,] inverse()
    {
        int n = Q.GetLength(0);
        int m = R.GetLength(0);
        if (n != m) throw new InvalidOperationException("Inverse requires a square matrix.");

        var inv = new double[n, n];
        var e   = new double[n];

        /* Solve A x_j = e_j  — one RHS per unit vector ----------*/
        for (int j = 0; j < n; j++)
        {
            Array.Clear(e, 0, n);
            e[j] = 1.0;

            var x = solve(e);
            for (int i = 0; i < n; i++) inv[i, j] = x[i];
        }
        return inv;
    }
}
