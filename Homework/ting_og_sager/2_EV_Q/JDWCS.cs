using System;

public static class JDWCS
{
    public static void TimesJ(Matrix A, int p, int q, double theta)
    {
        double c = Math.Cos(theta);
        double s = Math.Sin(theta);
        for (int i = 0; i < A.Rows; i++)
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
        for (int j = 0; j < A.Columns; j++)
        {
            double apj = A[p, j];
            double aqj = A[q, j];
            A[p, j] = c * apj - s * aqj;
            A[q, j] = s * apj + c * aqj;
        }
    }

    public static (Matrix D, Matrix V) Cyclic(Matrix A)
    {
        int n = A.Rows;
        Matrix D = A.Copy();
        Matrix V = Matrix.Identity(n);
        bool changed;
        int sweeps = 0;

        do
        {
            sweeps++;
            changed = false;
            for (int q = n - 1; q > 0; q--)
            {
                for (int p = 0; p < q; p++)
                {
                    double dpp = D[p, p];
                    double dqq = D[q, q];
                    double dpq = D[p, q];
                    double phi = 0.5 * Math.Atan2(2 * dpq, dqq - dpp);
                    double c = Math.Cos(phi);
                    double s = Math.Sin(phi);

                    double dpp1 = c * c * dpp - 2 * s * c * dpq + s * s * dqq;
                    double dqq1 = s * s * dpp + 2 * s * c * dpq + c * c * dqq;
                    if (dpp1 != dpp || dqq1 != dqq)
                    {
                        changed = true;
                        D[p, p] = dpp1;
                        D[q, q] = dqq1;
                        D[p, q] = 0.0;
                        D[q, p] = 0.0;

                        for (int i = 0; i < p; i++)
                        {
                            double dip = D[i, p], diq = D[i, q];
                            D[i, p] = c * dip - s * diq;
                            D[i, q] = s * dip + c * diq;
                            D[p, i] = D[i, p];
                            D[q, i] = D[i, q];
                        }
                        for (int i = p + 1; i < q; i++)
                        {
                            double dpi = D[p, i], diq = D[i, q];
                            D[p, i] = c * dpi - s * diq;
                            D[i, q] = s * dpi + c * diq;
                            D[i, p] = D[p, i];
                            D[q, i] = D[i, q];
                        }
                        for (int i = q + 1; i < n; i++)
                        {
                            double dpi = D[p, i], dqi = D[q, i];
                            D[p, i] = c * dpi - s * dqi;
                            D[q, i] = s * dpi + c * dqi;
                            D[i, p] = D[p, i];
                            D[i, q] = D[q, i];
                        }

                        for (int i = 0; i < n; i++)
                        {
                            double vip = V[i, p], viq = V[i, q];
                            V[i, p] = c * vip - s * viq;
                            V[i, q] = s * vip + c * viq;
                        }
                    }
                }
            }
        } while (changed);

        return (D, V);
    }
}
