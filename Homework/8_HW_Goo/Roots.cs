using System;
using static System.Math;

public static class RootFinding {

    // Part A: Newton's method with simple backtracking
    public static vector Newton(
        Func<vector, vector> f,
        vector start,
        double acc = 1e-3
    ) {
        vector x = start.copy();
        vector fx = f(x), z, fz;
        double lambda_min = 1.0 / 1024;
        int max_iterations = 1000;
        int iterations = 0;

        do {
            if (fx.norm() < acc) break;
            if (iterations++ > max_iterations) {
                Console.Error.WriteLine("Newton: max iterations reached.");
                break;
            }

            matrix J = Jacobian(f, x, fx);
            var QRJ = new QRGS(J);
            vector Dx = QRJ.solve(-fx);

            double lambda = 1.0;
            do {
                z = x + lambda * Dx;
                fz = f(z);
                if (fz.norm() < (1 - lambda / 2) * fx.norm()) break;
                if (lambda < lambda_min) break;
                lambda /= 2;
            } while (true);

            x = z;
            fx = fz;
        } while (true);
        return x;
    }

    // Part C: Optimized Newton's method with quadratic interpolation line search
    public static vector Newton_Optimized(
        Func<vector, vector> f,
        vector start,
        double acc = 1e-3
    ) {
        vector x = start.copy();
        vector fx, z, fz;
        double lambda_min = 1.0 / 1024;
        int max_iterations = 1000;
        int iterations = 0;

        matrix J = new matrix(x.size, x.size);
        fx = f(x);

        do {
            if (fx.norm() < acc) break;
            if (iterations++ > max_iterations) {
                Console.Error.WriteLine("Newton_Optimized: max iterations reached.");
                break;
            }

            Jacobian(f, x, fx, J);
            var QRJ = new QRGS(J);
            vector Dx = QRJ.solve(-fx);

            double lambda = 1.0;
            double fx_norm_sq = fx.norm() * fx.norm();

            while (true) {
                z = x + lambda * Dx;
                fz = f(z);
                if (fz.norm() < (1 - lambda / 2) * fx.norm()) break;
                if (lambda < lambda_min) break;

                // Quadratic interpolation - KORRIGERET FORMEL
                double fz_norm_sq = fz.norm() * fz.norm();
                double lambda_new = lambda * lambda * fx_norm_sq / (2 * (fz_norm_sq - fx_norm_sq + lambda * fx_norm_sq));
                if (lambda_new < 0.1 * lambda) lambda_new = 0.1 * lambda; // Undgå for små skridt
                lambda = lambda_new;

            }

            x = z;
            fx = fz;
        } while (true);
        return x;
    }

    public static matrix Jacobian(Func<vector, vector> f, vector x, vector fx = null) {
        matrix J = new matrix(x.size);
        Jacobian(f, x, fx, J);
        return J;
    }

    public static void Jacobian(Func<vector, vector> f, vector x, vector fx, matrix J) {
        vector dx = x.map(xi => Abs(xi) * Pow(2, -26) + 1e-9); // Added small term to avoid dx=0
        if (fx == null) fx = f(x);
        for (int j = 0; j < x.size; j++) {
            x[j] += dx[j];
            vector df = f(x) - fx;
            for (int i = 0; i < x.size; i++) J[i, j] = df[i] / dx[j];
            x[j] -= dx[j];
        }
    }
}
