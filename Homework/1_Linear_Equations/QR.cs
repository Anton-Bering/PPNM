using System;

public sealed class QR
{
    public matrix Q { get; }      // n×m (orthonormal columns)
    public matrix R { get; }      // m×m (upper‑triangular)

    /*------------------------------------------------------------*/
    public QR(matrix A)
    {
        int n = A.Rows;
        int m = A.Cols;

        if (n < m) throw new ArgumentException("QR decomposition requires n ≥ m");

        Q = A.Copy();                      // work‑copy of A
        R = new matrix(m, m);

        /* modified Gram–Schmidt ---------------------------------*/
        for (int j = 0; j < m; j++) {
            vector v = Q.GetCol(j);

            for (int i = 0; i < j; i++) {
                vector qi = Q.GetCol(i);
                double rij = vector.Dot(qi, v);
                R[i, j] = rij;
                v = v - rij * qi;
            }

            double norm = v.Norm();
            if (norm == 0.0)
                throw new InvalidOperationException("Matrix has linearly dependent (or zero) columns.");

            R[j, j] = norm;
            Q.SetCol(j, v / norm);
        }


    }

    // A x = b
    public vector solve(vector b)
    {
        int n = Q.Rows;
        int m = R.Cols;
        if (b.Size != n) throw new ArgumentException("b.Length must equal A.Rows");

        // y = Q^T b
        var y = new vector(m);
        for (int i = 0; i < m; i++)
            y[i] = vector.Dot(Q.GetCol(i), b);

        // R x = y
        return matrix.BackSubstitute(R, y);
    }

    public double det()
    {
        if (Q.Rows != Q.Cols) throw new InvalidOperationException("Determinant requires a square matrix.");
        int n = R.Cols;
        double p = 1.0;
        for (int i = 0; i < n; i++) p *= R[i, i];
        return p;
    }

    public matrix inverse()
    {
        if (Q.Rows != Q.Cols) throw new InvalidOperationException("Inverse requires a square matrix.");
        int n = Q.Rows;
        var inv = new matrix(n, n);

        for (int j = 0; j < n; j++) {
            var e = new vector(n);
            e[j] = 1.0;
            vector x = solve(e);
            inv.SetCol(j, x);
        }
        return inv;
    }
}
