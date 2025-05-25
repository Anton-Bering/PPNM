using System;

public class QR {
    private matrix Q;
    private matrix R;
    public QR(matrix A) {
        int n = A.size1;
        int m = A.size2;
        Q = A.copy();
        R = new matrix(m, m);
        // Modified Gram-Schmidt decomposition
        for (int j = 0; j < m; j++) {
            // Start with original j-th column of A (currently in Q)
            vector v = Q.column(j);
            // Subtract projections onto previously computed Q columns
            for (int i = 0; i < j; i++) {
                vector qi = Q.column(i);
                double Rij = qi.dot(v);
                R[i, j] = Rij;
                v = v - Rij * qi;
            }
            // Compute norm and normalize v to get j-th Q column
            double Rjj = v.norm();
            if (Rjj < 1e-12) 
                throw new Exception("Matrix has (near) linearly dependent columns");
            R[j, j] = Rjj;
            Q.setColumn(j, (1.0 / Rjj) * v);
        }
    }
    public vector solve(vector b) {
        int n = Q.size1;
        int m = Q.size2;
        if (b.size != n) 
            throw new ArgumentException("Vector length must match matrix row count");
        // Compute c = Q^T * b
        vector c = new vector(m);
        for (int i = 0; i < m; i++) {
            vector qi = Q.column(i);
            c[i] = qi.dot(b);
        }
        // Back-substitution to solve R*x = c
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
}
