using System;

public static class Jacobi
{
    public static void TimesJ(Matrix A, int p, int q, double theta)
    {
        double c = Math.Cos(theta);
        double s = Math.Sin(theta);
        for (int i = 0; i < A.Size1; i++)
        {
            double aip = A[i, p];
            double aiq = A[i, q];
            A[i, p] = c * aip - s * aiq;
            A[i, q] = s * aip + c * aiq;
        }
    }

    public static void JTimes(Matrix A, int p, int q, double theta)
    {
        double c = Math.Cos(theta);
        double s = Math.Sin(theta);
        for (int j = 0; j < A.Size2; j++)
        {
            double apj = A[p, j];
            double aqj = A[q, j];
            A[p, j] = c * apj + s * aqj;
            A[q, j] = (-s) * apj + c * aqj;
        }
    }

    public static (Vector w, Matrix V) Cyclic(Matrix M)
    {
        Matrix A = M.Copy();
        int n = A.Size1;
        Matrix V = Matrix.Identity(n);
        Vector w = new Vector(n);

        bool changed;
        double epsilon = 1e-10;
        int maxIterations = 10000; // Øget antal iterationer
        int iterations = 0;

        do
        {
            changed = false;
            for (int p = 0; p < n - 1; p++)
            {
                for (int q = p + 1; q < n; q++)
                {
                    double apq = A[p, q];
                    if (Math.Abs(apq) < epsilon) continue;

                    double app = A[p, p];
                    double aqq = A[q, q];
                    double theta = 0.5 * Math.Atan2(2 * apq, app - aqq);

                    JTimes(A, p, q, -theta);
                    TimesJ(A, p, q, theta);
                    TimesJ(V, p, q, theta);

                    changed = true;
                }
            }
            iterations++;
            if (iterations >= maxIterations) break;
        } while (changed);

        for (int i = 0; i < n; i++)
            w[i] = A[i, i];

        return (w, V);
    }
}