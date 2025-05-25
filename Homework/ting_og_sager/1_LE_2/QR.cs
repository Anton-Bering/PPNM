using System;
public class QR {
    public matrix Q;
    public matrix R;
    // Perform QR decomposition of matrix A using modified Gram-Schmidt
    public QR(matrix A) {
        int n = A.size1;
        int m = A.size2;
        if (n < m) {
            throw new ArgumentException("QR decomposition requires n >= m");
        }
        Q = A.copy();
        R = new matrix(m, m);
        for (int i = 0; i < m; i++) {
            // v_i = A_i (initially)
            // R_ii = ||v_i||
            double norm = 0;
            for (int k = 0; k < n; k++) {
                norm += Q[k, i] * Q[k, i];
            }
            norm = Math.Sqrt(norm);
            R[i, i] = norm;
            // q_i = v_i / R_ii
            for (int k = 0; k < n; k++) {
                Q[k, i] /= norm;
            }
            // Orthogonalize remaining columns against q_i
            for (int j = i + 1; j < m; j++) {
                double dot = 0;
                for (int k = 0; k < n; k++) {
                    dot += Q[k, i] * Q[k, j];
                }
                R[i, j] = dot;
                for (int k = 0; k < n; k++) {
                    Q[k, j] -= dot * Q[k, i];
                }
            }
        }
    }
    // Solve QR x = b (for square R) by back-substitution
    public vector solve(vector b) {
        int n = Q.size1;
        int m = R.size1;  // R is m×m
        if (b.size != n) 
            throw new ArgumentException("Vector b dimension does not match matrix");
        // Compute Q^T * b
        vector Qtb = new vector(m);
        for (int i = 0; i < m; i++) {
            double dot = 0;
            for (int k = 0; k < n; k++) {
                dot += Q[k, i] * b[k];
            }
            Qtb[i] = dot;
        }
        // Back-substitution in R x = Q^T b
        vector x = new vector(m);
        for (int i = m - 1; i >= 0; i--) {
            double sum = 0;
            for (int j = i + 1; j < m; j++) {
                sum += R[i, j] * x[j];
            }
            x[i] = (Qtb[i] - sum) / R[i, i];
        }
        return x;
    }
    // Compute determinant of original matrix (product of diagonal of R)
    public double det() {
        double product = 1.0;
        int m = R.size1;
        for (int i = 0; i < m; i++) {
            product *= R[i, i];
        }
        return product;
    }
    // Compute inverse of original matrix using QR decomposition
    public matrix inverse() {
        int n = Q.size1;
        int m = R.size1;
        if (n != m) 
            throw new ArgumentException("Matrix must be square to invert");
        matrix B = new matrix(n, n);
        // Solve for each unit vector e_j
        for (int j = 0; j < n; j++) {
            vector e = new vector(n);
            e[j] = 1.0;
            vector x = solve(e);
            for (int i = 0; i < n; i++) {
                B[i, j] = x[i];
            }
        }
        return B;
    }
}
