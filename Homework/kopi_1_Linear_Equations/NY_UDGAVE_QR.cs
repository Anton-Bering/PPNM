using System;
public class QR {
    public matrix Q;
    public matrix R;

    // Perform QR decomposition of A:
    public QR(matrix A) {
        int n = A.size1;
        int m = A.size2;
        if (n < m) {
            throw new ArgumentException("QR decomposition requires n >= m");
        }
        // Copy A into Q and initialize R
        Q = A.copy();
        R = new matrix(m, m);
        // Modified Gram-Schmidt orthogonalization
        for (int j = 0; j < m; j++) {
            // Orthogonalize column j against previous columns
            for (int i = 0; i < j; i++) {
                // Compute projection coefficient (dot product of col_i and col_j)
                double dot = 0;
                for (int k = 0; k < n; k++) {
                    dot += Q[k, i] * Q[k, j];
                }
                R[i, j] = dot;
                // Subtract the projection from column j
                for (int k = 0; k < n; k++) {
                    Q[k, j] -= dot * Q[k, i];
                }
            }
            // Compute norm of the j-th column of Q
            double norm = 0;
            for (int k = 0; k < n; k++) {
                norm += Q[k, j] * Q[k, j];
            }
            norm = Math.Sqrt(norm);
            R[j, j] = norm;
            if (norm == 0) {
                throw new InvalidOperationException("Matrix has linearly dependent or zero column");
            }
            // Normalize the j-th column of Q
            for (int k = 0; k < n; k++) {
                Q[k, j] /= norm;
            }
        }
    }

    // Solve for x in the equation QR*x = b
    public vector solve(vector b) {
        int n = Q.size1;
        int m = R.size1;  // R is m×m
        if (b.size != n) {
            throw new ArgumentException("Vector length must match matrix row count");
        }
        // Compute y = Q^T * b
        vector y = new vector(m);
        for (int i = 0; i < m; i++) {
            double sum = 0;
            for (int k = 0; k < n; k++) {
                sum += Q[k, i] * b[k];
            }
            y[i] = sum;
        }
        // to solve R*x = y
        vector x = new vector(m);
        for (int i = m - 1; i >= 0; i--) {
            double sum = y[i];
            for (int j = i + 1; j < m; j++) {
                sum -= R[i, j] * x[j];
            }
            x[i] = sum / R[i, i];
        }
        return x;
    }

}
