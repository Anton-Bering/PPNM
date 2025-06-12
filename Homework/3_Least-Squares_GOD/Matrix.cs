using System;
using System.Text;
using System.Linq;     // needed for a few helper LINQ calls

public class Matrix
{
    public readonly int Rows, Cols;
    private readonly double[,] a;

    public double this[int i, int j]
    {
        get => a[i, j];
        set => a[i, j] = value;
    }

    public Matrix(int m, int n)
    {
        Rows = m;
        Cols = n;
        a = new double[m, n];
    }

    public static Matrix Random(int m, int n, Random rng)
    {
        var A = new Matrix(m, n);
        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                A[i, j] = 2 * rng.NextDouble() - 1;   // uniform in (‑1,1)
        return A;
    }

    /* ------------- basic operations ------------- */

    public Matrix T
    {
        get
        {
            var B = new Matrix(Cols, Rows);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    B[j, i] = a[i, j];
            return B;
        }
    }

    public static Matrix operator *(Matrix A, Matrix B)
    {
        if (A.Cols != B.Rows) throw new ArgumentException("A*B dimension mismatch");
        var C = new Matrix(A.Rows, B.Cols);
        for (int i = 0; i < C.Rows; i++)
            for (int k = 0; k < A.Cols; k++)
            {
                double aik = A[i, k];
                for (int j = 0; j < C.Cols; j++)
                    C[i, j] += aik * B[k, j];
            }
        return C;
    }

    public static Vector operator *(Matrix A, Vector v)
    {
        if (A.Cols != v.Size) throw new ArgumentException("A*v dimension mismatch");
        var r = new Vector(A.Rows);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                r[i] += A[i, j] * v[j];
        return r;
    }

    /* ---- extra operators needed for Mono build ---- */

    public static Matrix operator -(Matrix A, Matrix B)
    {
        if (A.Rows != B.Rows || A.Cols != B.Cols) throw new ArgumentException("A-B dim mismatch");
        var C = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                C[i, j] = A[i, j] - B[i, j];
        return C;
    }

    public static Matrix operator *(double c, Matrix A)
    {
        var B = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                B[i, j] = c * A[i, j];
        return B;
    }

    /* ------------- pretty printer ------------- */

    public string ToPretty(string fmt = "0.0000")
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Rows; i++)
        {
            sb.Append("[ ");
            for (int j = 0; j < Cols; j++)
                sb.Append(a[i, j].ToString(fmt).PadLeft(12));
            sb.AppendLine(" ]");
        }
        return sb.ToString();
    }
}

/* ----------------------------------------------------------------- *
 *                     Simple dense vector class                     *
 * ----------------------------------------------------------------- */

public class Vector
{
    public readonly int Size;
    private readonly double[] v;

    public double this[int i]
    {
        get => v[i];
        set => v[i] = value;
    }

    public Vector(int n)
    {
        Size = n;
        v = new double[n];
    }

    /* arithmetic */

    public static Vector operator +(Vector a, Vector b)
    {
        var r = new Vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a[i] + b[i];
        return r;
    }

    public static Vector operator -(Vector a, Vector b)
    {
        var r = new Vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a[i] - b[i];
        return r;
    }

    public static Vector operator *(double c, Vector a)
    {
        var r = new Vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = c * a[i];
        return r;
    }

    public double Norm()
    {
        double s = 0;
        for (int i = 0; i < Size; i++) s += v[i] * v[i];
        return Math.Sqrt(s);
    }

    public string ToPretty(string fmt = "0.000000")
    {
        var sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < Size; i++)
        {
            sb.Append(v[i].ToString(fmt));
            if (i < Size - 1) sb.Append(", ");
        }
        sb.Append("]");
        return sb.ToString();
    }
}
