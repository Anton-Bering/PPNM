using System;
public class QRDecomposition {
    public matrix Q;
    public matrix R;

    public QRDecomposition(matrix A) {
        int n = A.rows;
        Q = new matrix(n, n);
        R = new matrix(n, n);
        // Perform modified Gram-Schmidt to orthogonalize columns of A
        for (int j = 0; j < n; j++) {
            // Start with column j of A as v
            for (int i = 0; i < n; i++) {
                Q[i, j] = A[i, j];
            }
            // Subtract projections onto previously computed q_k
            for (int k = 0; k < j; k++) {
                double dot = 0;
                for (int i = 0; i < n; i++) {
                    dot += Q[i, k] * Q[i, j];
                }
                R[k, j] = dot;
                for (int i = 0; i < n; i++) {
                    Q[i, j] -= Q[i, k] * dot;
                }
            }
            // Compute norm of the residual vector
            double norm = 0;
            for (int i = 0; i < n; i++) {
                norm += Q[i, j] * Q[i, j];
            }
            norm = Math.Sqrt(norm);
            R[j, j] = norm;
            if (norm == 0) {
                throw new Exception("QR decomposition failed: matrix has linearly dependent columns");
            }
            // Normalize the j-th column of Q
            for (int i = 0; i < n; i++) {
                Q[i, j] /= norm;
            }
        }
    }

    // Solve A*x = b using the QR factors (for square A).
    public vector solve(vector b) {
        int n = b.size;
        // Compute y = Q^T * b
        vector y = new vector(n);
        for (int j = 0; j < n; j++) {
            double dot = 0;
            for (int i = 0; i < n; i++) {
                dot += Q[i, j] * b[i];
            }
            y[j] = dot;
        }
        // Back-substitution to solve R * x = y
        vector x = new vector(n);
        for (int i = n - 1; i >= 0; i--) {
            double sum = 0;
            for (int k = i + 1; k < n; k++) {
                sum += R[i, k] * x[k];
            }
            x[i] = (y[i] - sum) / R[i, i];
        }
        return x;
    }
}
