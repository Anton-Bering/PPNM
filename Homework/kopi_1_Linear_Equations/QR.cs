// QR.cs
using System;
using static mactrics;

public class QR {
    public matrix Q;
    public matrix R;

    /* ---------- constructor: modified Gram-Schmidt ---------- */
    public QR(matrix A) {
        int n = A.size1, m = A.size2;
        if (n < m) throw new ArgumentException("QR decomposition requires n ≥ m");
        Q = A.copy();
        R = new matrix(m, m);

        for (int j = 0; j < m; ++j) {
            for (int i = 0; i < j; ++i) {
                double dot = 0.0;
                for (int k = 0; k < n; ++k) dot += Q[k, i] * Q[k, j];
                R[i, j] = dot;
                for (int k = 0; k < n; ++k) Q[k, j] -= dot * Q[k, i];
            }
            double norm = 0.0;
            for (int k = 0; k < n; ++k) norm += Q[k, j] * Q[k, j];
            norm = Math.Sqrt(norm);
            R[j, j] = norm;
            if (norm == 0) throw new InvalidOperationException("Matrix has linearly dependent or zero column");
            for (int k = 0; k < n; ++k) Q[k, j] /= norm;
        }
    }

    /* ---------- helper to convert legacy matrix ---------- */
    static double[,] MatrixToArray(matrix M) {
        int n = M.size1, m = M.size2;
        var A = new double[n, m];
        for (int i = 0; i < n; ++i)
            for (int j = 0; j < m; ++j)
                A[i, j] = M[i, j];
        return A;
    }

    /* ---------- solve QR x = b ---------- */
    public vector solve(vector b) {
        int n = Q.size1, m = R.size1;
        if (b.size != n) throw new ArgumentException("Vector length must match matrix row count");

        /* use Mactrics for y = Qᵀ b */
        var Qt   = Transpose(MatrixToArray(Q));
        var bCol = new double[n, 1];
        for (int i = 0; i < n; ++i) bCol[i, 0] = b[i];
        var yCol = MultiplyMatrices(Qt, bCol);

        var y = new vector(m);
        for (int i = 0; i < m; ++i) y[i] = yCol[i, 0];

        var x = new vector(m);
        for (int i = m - 1; i >= 0; --i) {
            double sum = y[i];
            for (int j = i + 1; j < m; ++j) sum -= R[i, j] * x[j];
            x[i] = sum / R[i, i];
        }
        return x;
    }

    /* ---------- determinant ---------- */
    public double det() {
        double prod = 1.0;
        for (int i = 0; i < R.size1; ++i) prod *= R[i, i];
        return prod;
    }

    /* ---------- inverse ---------- */
    public matrix inverse() {
        int n = Q.size1;
        if (n != R.size1) throw new InvalidOperationException("Matrix must be square");

        var inv = new matrix(n, n);
        for (int j = 0; j < n; ++j) {
            var e = new vector(n); e[j] = 1.0;
            var x = solve(e);
            for (int i = 0; i < n; ++i) inv[i, j] = x[i];
        }
        return inv;
    }
}
