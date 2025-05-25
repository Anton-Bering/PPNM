using System;

public static class jacobi {
    // Multiply matrix A on the right by Jacobi rotation matrix J(p,q,theta)
    public static void timesJ(matrix A, int p, int q, double theta) {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        for (int i = 0; i < A.size1; i++) {
            double Aip = A[i, p];
            double Aiq = A[i, q];
            A[i, p] = c * Aip - s * Aiq;
            A[i, q] = s * Aip + c * Aiq;
        }
    }

    // Multiply matrix A on the left by Jacobi rotation matrix J(p,q,theta)
    public static void Jtimes(matrix A, int p, int q, double theta) {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        for (int j = 0; j < A.size2; j++) {
            double Apj = A[p, j];
            double Aqj = A[q, j];
            A[p, j] = c * Apj + s * Aqj;
            A[q, j] = -s * Apj + c * Aqj;
        }
    }

    // Jacobi eigenvalue decomposition: returns (w, V, sweeps)
    public static (vector, matrix, int) cyclic(matrix M) {
        int n = M.size1;
        matrix A = M.copy();     // copy of M to avoid altering original
        matrix V = matrix.id(n); // initialize V as identity
        vector w = new vector(n);

        int sweeps = 0;          // count number of sweeps
        bool changed;
        do {
            changed = false;
            for (int p = 0; p < n - 1; p++) {
                for (int q = p + 1; q < n; q++) {
                    double app = A[p, p];
                    double aqq = A[q, q];
                    double apq = A[p, q];
                    double theta = 0.5 * Math.Atan2(2 * apq, (aqq - app));
                    double c = Math.Cos(theta);
                    double s = Math.Sin(theta);
                    double new_app = c * c * app - 2 * s * c * apq + s * s * aqq;
                    double new_aqq = s * s * app + 2 * s * c * apq + c * c * aqq;
                    if (new_app != app || new_aqq != aqq) {
                        changed = true;
                        timesJ(A, p, q, theta);
                        Jtimes(A, p, q, -theta);
                        timesJ(V, p, q, theta);
                    }
                }
            }
            sweeps++;
        } while (changed);

        for (int i = 0; i < n; i++) {
            w[i] = A[i, i];
        }

        return (w, V, sweeps);
    }
}
