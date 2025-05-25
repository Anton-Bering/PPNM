using System;

public static class QR {
    public static (matrix, matrix) decomp(matrix A) {
        matrix Q = A.copy();
        int m = Q.size2;
        matrix R = new matrix(m, m);

        for (int j = 0; j < m; j++) {
            double norm = 0;
            for (int i = 0; i < Q.size1; i++) norm += Q[i, j] * Q[i, j];
            norm = Math.Sqrt(norm);
            R[j, j] = norm;

            if (R[j, j] == 0) throw new InvalidOperationException("Matrix is rank deficient.");

            for (int i = 0; i < Q.size1; i++) Q[i, j] /= R[j, j];

            for (int k = j + 1; k < m; k++) {
                R[j, k] = 0;
                for (int i = 0; i < Q.size1; i++) R[j, k] += Q[i, j] * Q[i, k];
                for (int i = 0; i < Q.size1; i++) Q[i, k] -= R[j, k] * Q[i, j];
            }
        }
        return (Q, R);
    }

    public static vector solve(matrix Q, matrix R, vector b) {
        vector y = new vector(Q.size2);
        for (int j = 0; j < Q.size2; j++)
            for (int i = 0; i < Q.size1; i++)
                y[j] += Q[i, j] * b[i];

        vector x = new vector(R.size2);
        for (int i = R.size2 - 1; i >= 0; i--) {
            double sum = y[i];
            for (int j = i + 1; j < R.size2; j++) sum -= R[i, j] * x[j];
            x[i] = sum / R[i, i];
        }
        return x;
    }

    public static double det(matrix R) {
        double det = 1.0;
        for (int i = 0; i < R.size1; i++) det *= R[i, i];
        return det;
    }

    public static matrix inverse(matrix Q, matrix R) {
        int n = Q.size1;
        matrix inv = new matrix(n, n);
        for (int i = 0; i < n; i++) {
            vector e = new vector(n);
            e[i] = 1.0;
            vector x = solve(Q, R, e);
            for (int j = 0; j < n; j++) inv[j, i] = x[j];
        }
        return inv;
    }
}