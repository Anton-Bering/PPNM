using System;
public class QR {
    private matrix Q;
    private matrix R;
    public QR(matrix A) {
        int m = A.size1;
        int n = A.size2;
        // Create Q as a copy of A
        Q = A.copy();
        R = new matrix(n, n);
        // Modified Gram-Schmidt orthogonalization
        for (int i = 0; i < n; i++) {
            // Compute norm of column i
            double norm = 0;
            for (int k = 0; k < m; k++) {
                norm += Q[k, i]*Q[k, i];
            }
            norm = Math.Sqrt(norm);
            R[i, i] = norm;
            if (norm == 0) throw new Exception("Matrix has a zero column");
            // Normalize column i of Q
            for (int k = 0; k < m; k++) {
                Q[k, i] /= norm;
            }
            // Orthogonalize remaining columns
            for (int j = i+1; j < n; j++) {
                double dot = 0;
                for (int k = 0; k < m; k++) {
                    dot += Q[k, i]*Q[k, j];
                }
                R[i, j] = dot;
                for (int k = 0; k < m; k++) {
                    Q[k, j] -= Q[k, i]*dot;
                }
            }
        }
    }
    public vector solve(vector b) {
        int n = R.size1;
        // Compute y = Q^T * b
        vector y = new vector(n);
        for (int j = 0; j < n; j++) {
            double dot = 0;
            for (int i = 0; i < Q.size1; i++) {
                dot += Q[i, j]*b[i];
            }
            y[j] = dot;
        }
        // Back-substitution to solve R*x = y
        vector x = new vector(n);
        for (int i = n-1; i >= 0; i--) {
            double sum = 0;
            for (int k = i+1; k < n; k++) {
                sum += R[i, k]*x[k];
            }
            x[i] = (y[i] - sum) / R[i, i];
        }
        return x;
    }
    // (Determinant and inverse methods could be added if needed)
}
