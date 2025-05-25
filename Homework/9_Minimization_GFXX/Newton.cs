using System;

public static class Newton {
    // Newton's method for function minimization.
    // f: objective function to minimize.
    // x: initial guess vector.
    // acc: convergence threshold for gradient norm.
    // iterations: output parameter for the number of iterations taken.
    // centralDiff: if true, use central difference for gradient/Hessian; otherwise use forward difference.
    public static double[] Minimize(Func<double[], double> f, double[] initial, double acc, out int iterations, bool centralDiff = false) {
        int maxIter = 1000;
        iterations = 0;
        int n = initial.Length;
        // Clone initial guess to avoid modifying the original array
        double[] x = (double[]) initial.Clone();
        for (int iter = 1; iter <= maxIter; iter++) {
            iterations = iter;
            // Compute gradient at current x
            double[] g = Utils.Gradient(f, x, centralDiff);
            double gradNorm = Utils.Norm(g);
            if (gradNorm < acc) {
                // Converged: gradient norm is below threshold
                break;
            }
            // Compute Hessian at current x
            double[,] H = Utils.Hessian(f, x, centralDiff);
            // Solve H * dx = -g for Newton step (dx)
            double[] negG = new double[n];
            for (int i = 0; i < n; i++) negG[i] = -g[i];
            double[] dx;
            try {
                dx = Utils.SolveLinear(H, negG);
            } catch (Exception) {
                // If Hessian is singular or not solvable, break out of the loop
                break;
            }
            // Backtracking line search to find suitable step length
            double lambda = Utils.LineSearch(f, x, dx);
            // Update x = x + lambda * dx
            for (int i = 0; i < n; i++) {
                x[i] += lambda * dx[i];
            }
        }
        return x;
    }
}
