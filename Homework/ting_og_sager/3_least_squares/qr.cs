using System;

public static class QR {
    // QR decomposition using Modified Gram-Schmidt.
    // Returns Q (with orthonormal columns) and R (upper triangular).
    public static (matrix Q, matrix R) decomp(matrix A) {
        int n = A.size1;
        int m = A.size2;
        matrix Q = A.copy();
        matrix R = new matrix(m, m);
        for (int j = 0; j < m; j++) {
            // Compute the norm of column j in Q
            double norm = 0;
            for (int i = 0; i < n; i++) {
                norm += Q[i, j] * Q[i, j];
            }
            norm = Math.Sqrt(norm);
            R[j, j] = norm;
            // Normalize column j of Q
            for (int i = 0; i < n; i++) {
                Q[i, j] /= norm;
            }
            // Orthogonalize remaining columns against column j
            for (int k = j + 1; k < m; k++) {
                double dot = 0;
                for (int i = 0; i < n; i++) {
                    dot += Q[i, j] * Q[i, k];
                }
                R[j, k] = dot;
                for (int i = 0; i < n; i++) {
                    Q[i, k] -= Q[i, j] * R[j, k];
                }
            }
        }
        return (Q, R);
    }

    // Solve A*c = b for c, given QR decomposition of A.
    // Here Q is n×m, R is m×m (for n ≥ m, a least-squares solution if n > m).
    public static vector solve(matrix Q, matrix R, vector b) {
        int n = Q.size1;
        int m = Q.size2;
        // Compute Q^T * b (which is an m-length vector)
        vector x = new vector(m);
        for (int j = 0; j < m; j++) {
            double sum = 0;
            for (int i = 0; i < n; i++) {
                sum += Q[i, j] * b[i];
            }
            x[j] = sum;
        }
        // Back-substitution to solve R * c = x
        vector c = new vector(m);
        for (int i = m - 1; i >= 0; i--) {
            double sum = x[i];
            for (int k = i + 1; k < m; k++) {
                sum -= R[i, k] * c[k];
            }
            c[i] = sum / R[i, i];
        }
        return c;
    }
}
