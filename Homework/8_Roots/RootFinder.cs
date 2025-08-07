using System;
using static System.Math;

public static class RootFinder
{
    // Opgave A: (In the naïve implementation in part A we allocate — at each step — a new matrix to keep the Jacobian)
    public static Vector Newton(Func<Vector, Vector> f, Vector start, double acc = 1e-3)
    {
        Vector x = start.Copy();
        Vector fx = f(x);
        double lambdaMin = 1.0 / 1024.0;
        
        while (fx.Norm() > acc)
        {
            Matrix J = Jacobian(f, x, fx);
            Vector Dx = new QrDecomposition(J).Solve(-fx);
            double lambda = 1.0;
            while (true)
            {
                Vector z = x + lambda * Dx;
                Vector fz = f(z);
                if (fz.Norm() < (1 - lambda / 2) * fx.Norm())
                {
                    x = z; fx = fz; break;
                }
                lambda /= 2;
                if (lambda < lambdaMin)
                {
                    x = z; fx = fz; break;
                }
            }
        }
        return x;
    }

    // Opgave C: (Optimize the implementation by only allocating one matrix in the beginning and then updating it at each step.)
    public static Vector NewtonOptimized(Func<Vector, Vector> f, Vector start, double acc = 1e-3)
    {
        Vector x = start.Copy();
        Vector fx = f(x);
        Matrix J = new Matrix(x.Size);
        
        while (fx.Norm() > acc)
        {
            UpdateJacobian(f, J, x, fx); // Genbrug Jacobian-matrix
            Vector Dx = new QrDecomposition(J).Solve(-fx);

            // (fra bogen, s. 95):
            double lambda = 1.0;
            double phi_0 = 0.5 * fx.Norm() * fx.Norm();

            while (true)
            {
                Vector z = x + lambda * Dx;
                Vector fz = f(z);

                if (fz.Norm() < (1 - lambda / 2) * fx.Norm())
                {
                    x = z;
                    fx = fz;
                    break;
                }

                // Hvis skridtet er for lille...:
                if (lambda < 1.0/1024.0) {
                    x = z;
                    fx = fz;
                    break;
                }

                // Formler fra s. 95, (9.10), (9.11), (9.12)
                double phi_lambda = 0.5 * fz.Norm() * fz.Norm();
                double phi_prime_0 = -2 * phi_0;

                double c = (phi_lambda - phi_0 - phi_prime_0 * lambda) / (lambda * lambda);

                double lambda_next = -phi_prime_0 / (2 * c);
                
                // For at undgå for store skridt...:
                lambda = Min(lambda_next, lambda/2);
            }
        }
        return x;
    }

    private static Matrix Jacobian(Func<Vector, Vector> f, Vector x, Vector fx)
    {
        Matrix J = new Matrix(x.Size);
        UpdateJacobian(f, J, x, fx);
        return J;
    }

    private static void UpdateJacobian(Func<Vector, Vector> f, Matrix J, Vector x, Vector fx)
    {
        Vector dx = x.Map(xi => Abs(xi) * Pow(2, -26) + 1e-8);
        for (int j = 0; j < x.Size; j++)
        {
            x[j] += dx[j];
            Vector df = f(x) - fx;
            for (int i = 0; i < x.Size; i++) J[i, j] = df[i] / dx[j];
            x[j] -= dx[j];
        }
    }
}