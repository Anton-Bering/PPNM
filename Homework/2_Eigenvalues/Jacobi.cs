using System;

public static class jacobi
{
    /* Apply Jacobi rotation from the right:  A ← A · J(p,q,θ) */
    private static void TimesJ(double[,] A, int p, int q, double theta)
    {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        int n = A.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            double Aip = A[i, p];
            double Aiq = A[i, q];
            A[i, p] =  c * Aip - s * Aiq;
            A[i, q] =  s * Aip + c * Aiq;
        }
    }

    /* Apply Jacobi rotation from the left:  A ← J(p,q,θ)ᵀ · A */
    private static void JTimes(double[,] A, int p, int q, double theta)
    {
        double c = Math.Cos(theta), s = Math.Sin(theta);
        int m = A.GetLength(1);
        for (int j = 0; j < m; j++)
        {
            double Apj = A[p, j];
            double Aqj = A[q, j];
            A[p, j] =  c * Apj + s * Aqj;
            A[q, j] = -s * Apj + c * Aqj;
        }
    }

    /// <summary>
    /// Cyclic Jacobi eigen‑decomposition of a real, symmetric matrix.
    /// On return <paramref name="A"/> is diagonal, <paramref name="w"/>
    /// holds the eigenvalues, and <paramref name="V"/> the eigenvectors
    /// such that  A₀ = V·diag(w)·Vᵀ.
    /// </summary>
    public static void cyclic(double[,] A, double[] w, double[,] V)
    {
        int n = A.GetLength(0);
        if (A.GetLength(1) != n)      throw new ArgumentException("Matrix must be square.");
        if (w.Length != n)            throw new ArgumentException("Eigenvalue vector has wrong length.");
        if (V.GetLength(0) != n || V.GetLength(1) != n)
            throw new ArgumentException("Eigenvector matrix has wrong dimensions.");

        /* V ← I */
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                V[i, j] = (i == j) ? 1.0 : 0.0;

        bool changed;
        do
        {
            changed = false;
            for (int p = 0; p < n - 1; p++)
                for (int q = p + 1; q < n; q++)
                {
                    double app = A[p, p], aqq = A[q, q], apq = A[p, q];
                    if (Math.Abs(apq) < 1e-12) continue;

                    double theta = 0.5 * Math.Atan2(2.0 * apq, aqq - app);
                    double c = Math.Cos(theta), s = Math.Sin(theta);

                    double new_app = c * c * app - 2.0 * s * c * apq + s * s * aqq;
                    double new_aqq = s * s * app + 2.0 * s * c * apq + c * c * aqq;
                    if (Math.Abs(new_app - app) < 1e-14 &&
                        Math.Abs(new_aqq - aqq) < 1e-14) continue;

                    changed = true;
                    TimesJ(A, p, q,  theta);
                    JTimes(A, p, q, -theta);
                    TimesJ(V, p, q,  theta);
                }
        } while (changed);

        for (int i = 0; i < n; i++) w[i] = A[i, i];
    }
}
