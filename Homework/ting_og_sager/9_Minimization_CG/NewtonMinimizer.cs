// NewtonMinimizer.cs
using System;

namespace NewtonMinimization
{
    public static class NewtonMinimizer
    {
        public static double[] Minimize(Func<double[], double> f, double[] initial, double tol, out int iterations, int maxIter = 1000)
        {
            int n = initial.Length;
            double[] x = new double[n];
            Array.Copy(initial, x, n);
            double fVal = f(x);

            const double alphaStart = 1.0;
            const double rho = 0.5;
            const double c = 1e-4;
            const double h = 1e-5;

            iterations = 0;
            while (iterations < maxIter)
            {
                // Central difference gradient
                double[] grad = new double[n];
                for (int i = 0; i < n; i++)
                {
                    double orig = x[i];
                    x[i] = orig + h;
                    double fPlus = f(x);
                    x[i] = orig - h;
                    double fMinus = f(x);
                    grad[i] = (fPlus - fMinus) / (2 * h);
                    x[i] = orig;
                }

                double gradNorm = 0.0;
                foreach (double g in grad) gradNorm += g * g;
                gradNorm = Math.Sqrt(gradNorm);
                Console.Error.WriteLine($"Step {iterations}, f(x) = {fVal}, ||grad|| = {gradNorm}");
                if (gradNorm < tol) break;

                // Central difference Hessian
                double[,] Hess = new double[n, n];
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double xi = x[i];
                        double xj = x[j];

                        x[i] = xi + h; x[j] = xj + h; double f1 = f(x);
                        x[i] = xi + h; x[j] = xj - h; double f2 = f(x);
                        x[i] = xi - h; x[j] = xj + h; double f3 = f(x);
                        x[i] = xi - h; x[j] = xj - h; double f4 = f(x);

                        Hess[i, j] = (f1 - f2 - f3 + f4) / (4 * h * h);

                        x[i] = xi;
                        x[j] = xj;
                    }
                }

                // Dæmpning: gør Hessian mere positiv definit
                double lambda = 1e-3;
                for (int i = 0; i < n; i++) Hess[i, i] += lambda;

                double[] p = SolveLinearSystem(Hess, grad, n);
                double gradDotP = 0.0;
                for (int i = 0; i < n; i++) gradDotP += grad[i] * p[i];

                if (gradDotP > 0)
                    for (int i = 0; i < n; i++) p[i] = -grad[i];
                else
                    for (int i = 0; i < n; i++) p[i] = -p[i];

                double alpha = alphaStart;
                while (true)
                {
                    double[] xNew = new double[n];
                    for (int i = 0; i < n; i++) xNew[i] = x[i] + alpha * p[i];
                    double fNew = f(xNew);
                    if (fNew <= fVal + c * alpha * gradDotP)
                    {
                        x = xNew;
                        fVal = fNew;
                        break;
                    }
                    alpha *= rho;
                    if (alpha < 1e-8)
                    {
                        x = xNew;
                        fVal = fNew;
                        break;
                    }
                }

                iterations++;
            }
            return x;
        }

        private static double[] SolveLinearSystem(double[,] H, double[] grad, int n)
        {
            double[] b = new double[n];
            for (int i = 0; i < n; i++) b[i] = -grad[i];

            for (int k = 0; k < n; k++)
            {
                int pivot = k;
                double maxVal = Math.Abs(H[k, k]);
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(H[i, k]) > maxVal)
                    {
                        maxVal = Math.Abs(H[i, k]);
                        pivot = i;
                    }
                }
                if (Math.Abs(H[pivot, k]) < 1e-12)
                    throw new InvalidOperationException("Hessian matrix is singular or ill-conditioned.");

                if (pivot != k)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double temp = H[k, j];
                        H[k, j] = H[pivot, j];
                        H[pivot, j] = temp;
                    }
                    double tempB = b[k];
                    b[k] = b[pivot];
                    b[pivot] = tempB;
                }

                for (int i = k + 1; i < n; i++)
                {
                    double factor = H[i, k] / H[k, k];
                    for (int j = k; j < n; j++)
                        H[i, j] -= factor * H[k, j];
                    b[i] -= factor * b[k];
                }
            }

            double[] p = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0.0;
                for (int j = i + 1; j < n; j++)
                    sum += H[i, j] * p[j];
                p[i] = (b[i] - sum) / H[i, i];
            }
            return p;
        }
    }
}
