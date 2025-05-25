using System;

public class Matrix
{
    private double[,] data;

    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        data = new double[rows, cols];
    }

    public Matrix(double[,] data)
    {
        Rows = data.GetLength(0);
        Cols = data.GetLength(1);
        this.data = (double[,])data.Clone();
    }

    public int Rows { get; }
    public int Cols { get; }

    public double this[int row, int col]
    {
        get => data[row, col];
        set => data[row, col] = value;
    }

    public Matrix Transpose()
    {
        Matrix result = new Matrix(Cols, Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[j, i] = data[i, j];
        return result;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Cols != b.Rows)
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");

        Matrix result = new Matrix(a.Rows, b.Cols);
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < b.Cols; j++)
            {
                double sum = 0.0;
                for (int k = 0; k < a.Cols; k++)
                {
                    sum += a[i, k] * b[k, j];
                }
                result[i, j] = sum;
            }
        }
        return result;
    }

    public void DecomposeQR(out Matrix q, out Matrix r)
    {
        int m = Cols;
        int n = Rows;

        q = new Matrix(n, m);
        r = new Matrix(m, m);

        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                q[i, j] = this[i, j];

        for (int k = 0; k < m; k++)
        {
            double norm = 0.0;
            for (int i = 0; i < n; i++)
                norm += q[i, k] * q[i, k];
            norm = Math.Sqrt(norm);

            r[k, k] = norm;

            if (norm == 0.0)
                throw new InvalidOperationException("Matrix is rank deficient.");

            for (int i = 0; i < n; i++)
                q[i, k] /= norm;

            for (int j = k + 1; j < m; j++)
            {
                double dot = 0.0;
                for (int i = 0; i < n; i++)
                    dot += q[i, k] * q[i, j];
                r[k, j] = dot;

                for (int i = 0; i < n; i++)
                    q[i, j] -= q[i, k] * dot;
            }
        }
    }

    public Matrix InvertUpperTriangular()
    {
        if (Rows != Cols)
            throw new InvalidOperationException("Matrix must be square.");

        int m = Rows;
        Matrix inv = new Matrix(m, m);

        for (int j = 0; j < m; j++)
        {
            inv[j, j] = 1.0 / this[j, j];
            for (int i = j - 1; i >= 0; i--)
            {
                double sum = 0.0;
                for (int k = i + 1; k <= j; k++)
                {
                    sum += this[i, k] * inv[k, j];
                }
                inv[i, j] = -sum / this[i, i];
            }
        }

        return inv;
    }

    public Vector SolveUpperTriangular(Vector b)
    {
        if (Rows != Cols)
            throw new InvalidOperationException("Matrix must be square.");
        if (Rows != b.Length)
            throw new ArgumentException("Matrix and vector dimensions do not match.");

        int n = Rows;
        Vector x = new Vector(n);

        for (int i = n - 1; i >= 0; i--)
        {
            double sum = 0.0;
            for (int j = i + 1; j < n; j++)
            {
                sum += this[i, j] * x[j];
            }
            x[i] = (b[i] - sum) / this[i, i];
        }

        return x;
    }
}
