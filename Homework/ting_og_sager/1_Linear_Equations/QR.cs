using System;
public class QR {
    public matrix Q;
    public matrix R;
    // Perform QR decomposition of matrix A using modified Gram-Schmidt
    public QR(matrix A) {
        int n = A.size1;
        int m = A.size2;
        if (n < m) {
            throw new ArgumentException("QR decomposition requires n >= m (matrix must have >= rows than cols)");
        }
        Q = A.copy();
        R = new matrix(m, m);
        // Modified Gram-Schmidt orthogonalization
        for (int i = 0; i < m; i++) {
            // Subtract projections of previous q-columns from column i
            for (int j = 0; j < i; j++) {
                double dot = 0.0;
                for (int k = 0; k < n; k++) {
                    dot += Q[k, j] * Q[k, i];
                }
                R[j, i] = dot;
                for (int k = 0; k < n; k++) {
                    Q[k, i] -= dot * Q[k, j];
                }
            }
            // Compute norm of the i-th residual vector
            double norm = 0.0;
            for (int k = 0; k < n; k++) {
                norm += Q[k, i] * Q[k, i];
            }
            norm = Math.Sqrt(norm);
            R[i, i] = norm;
            if (norm == 0) {
                throw new InvalidOperationException("Matrix has linearly dependent (singular) columns, QR decomposition failed");
            }
            // Normalize i-th column of Q
            for (int k = 0; k < n; k++) {
                Q[k, i] /= norm;
            }
        }
    }

    // Solve A*x = b using the computed Q and R (for square A or least-squares if A is tall)
    public vector solve(vector b) {
        int n = Q.size1;
        int m = R.size1;  // R is m×m
        if (b.size != n) {
            throw new ArgumentException("Vector length must match the number of rows of A");
        }
        // Compute c = Q^T * b
        vector c = new vector(m);
        for (int j = 0; j < m; j++) {
            double dot = 0.0;
            for (int i = 0; i < n; i++) {
                dot += Q[i, j] * b[i];
            }
            c[j] = dot;
        }
        // Back-substitution: R * x = c
        vector x = new vector(m);
        for (int i = m - 1; i >= 0; i--) {
            double sum = 0.0;
            for (int j = i + 1; j < m; j++) {
                sum += R[i, j] * x[j];
            }
            x[i] = (c[i] - sum) / R[i, i];
        }
        return x;
    }

    // Compute determinant of A (only valid if A is square). det(A) = Π (R[i,i])
    public double det() {
        if (R.size1 != R.size2 || Q.size1 != Q.size2) {
            throw new InvalidOperationException("Determinant is defined only for square matrices");
        }
        double prod = 1.0;
        int n = R.size1;
        for (int i = 0; i < n; i++) {
            prod *= R[i, i];
        }
        return prod;
    }

    // Compute the inverse of A using the QR decomposition (only if A is square)
    public matrix inverse() {
        if (Q.size1 != Q.size2 || R.size1 != R.size2) {
            throw new InvalidOperationException("Inverse is defined only for square matrices");
        }
        int n = Q.size1;
        matrix B = new matrix(n, n);
        // Solve A * x = e_i for each unit vector e_i
        for (int i = 0; i < n; i++) {
            vector e = new vector(n);
            e[i] = 1.0;
            vector x = this.solve(e);
            for (int k = 0; k < n; k++) {
                B[k, i] = x[k];
            }
        }
        return B;
    }
}
