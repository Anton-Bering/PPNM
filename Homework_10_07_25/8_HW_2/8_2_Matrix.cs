// ---------- Matrix.cs ----------
using System;

public class Mat
{
    public double[,] data;
    public int n, m;

    public Mat(int n, int m)
    {
        this.n = n; this.m = m;
        data = new double[n, m];
    }

    public double this[int i, int j]
    {
        get => data[i, j];
        set => data[i, j] = value;
    }

    // Solve A·x = b by Gaussian elimination with partial pivoting -------------
    public Vec Solve(Vec b)
    {
        int N = n;
        var A = (double[,])data.Clone();
        var v = (double[])b.data.Clone();

        for (int k = 0; k < N; ++k)
        {
            // Pivot
            int max = k;
            for (int i = k + 1; i < N; ++i)
                if (Math.Abs(A[i, k]) > Math.Abs(A[max, k])) max = i;

            // Swap rows
            if (max != k)
            {
                for (int j = 0; j < N; ++j)
                {
                    double tmp = A[k, j];
                    A[k, j]     = A[max, j];
                    A[max, j]   = tmp;
                }
                double tmpv = v[k];
                v[k] = v[max];
                v[max] = tmpv;
            }


            // Eliminate
            for (int i = k + 1; i < N; ++i)
            {
                double factor = A[i, k] / A[k, k];
                for (int j = k; j < N; ++j)
                    A[i, j] -= factor * A[k, j];
                v[i] -= factor * v[k];
            }
        }

        // Back‑substitution
        var x = new Vec(N);
        for (int i = N - 1; i >= 0; --i)
        {
            double sum = v[i];
            for (int j = i + 1; j < N; ++j) sum -= A[i, j] * x[j];
            x[i] = sum / A[i, i];
        }
        return x;
    }
}
// ---------------------------------------------------------------------------
