using System;

public static class jacobi {
    // Apply Jacobi rotation from the right: A <- A * J(p,q,theta)
    public static void timesJ(matrix A, int p, int q, double theta) {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        for (int i = 0; i < A.size1; i++) {
            double Aip = A[i, p];
            double Aiq = A[i, q];
            A[i, p] = c * Aip - s * Aiq;
            A[i, q] = s * Aip + c * Aiq;
        }
    }

    // Apply Jacobi rotation from the left: A <- J(p,q,theta) * A 
    public static void Jtimes(matrix A, int p, int q, double theta) {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        for (int j = 0; j < A.size2; j++) {  // size2 == number of columns
            double Apj = A[p, j];
            double Aqj = A[q, j];
            A[p, j] = c * Apj + s * Aqj;
            A[q, j] = -s * Apj + c * Aqj;
        }
    }

    // Jacobi eigenvalue decomposition with cyclic sweeps.
    // On output: A is diagonalized, w contains eigenvalues, V contains eigenvectors as columns.
    public static void cyclic(matrix A, vector w, matrix V) {
        int n = A.size1;
        // Initialize V as identity matrix
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                V[i, j] = (i == j) ? 1.0 : 0.0;
            }
        }
        bool changed;
        do {
            changed = false;
            // Sweep through all p<q pairs
            for (int p = 0; p < n - 1; p++) {
                for (int q = p + 1; q < n; q++) {
                    double app = A[p, p];
                    double aqq = A[q, q];
                    double apq = A[p, q];
                    // Calculate rotation angle theta
                    double theta = 0.5 * Math.Atan2(2 * apq, aqq - app);
                    double c = Math.Cos(theta), s = Math.Sin(theta);
                    // Compute new diagonal values after rotation (without actually rotating yet)
                    double new_app = c * c * app - 2 * s * c * apq + s * s * aqq;
                    double new_aqq = s * s * app + 2 * s * c * apq + c * c * aqq;
                    // Decide whether to rotate (if diagonal elements will change)
                    if (new_app != app || new_aqq != aqq) {
                        changed = true;
                        // Perform the Jacobi rotation on A and accumulate in V
                        timesJ(A, p, q, theta);       // A = A * J(p,q, theta)
                        Jtimes(A, p, q, -theta);      // A = J(p,q, theta)^T * A
                        timesJ(V, p, q, theta);       // V = V * J(p,q, theta)
                    }
                }
            }
        } while (changed);
        // Extract eigenvalues from diagonalized A
        for (int i = 0; i < n; i++) {
            w[i] = A[i, i];
        }
    }
}
