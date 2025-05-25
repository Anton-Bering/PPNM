using System;

public class Matrix
{
    private double[,] data;

    public int Rows => data.GetLength(0);
    public int Cols => data.GetLength(1);

    // ✅ Alias så du kan bruge .Columns som før
    public int Columns => Cols;

    public Matrix(int rows, int cols)
    {
        data = new double[rows, cols];
    }

    // ✅ Indexer med både get og set
    public double this[int i, int j]
    {
        get => data[i, j];
        set => data[i, j] = value;
    }

    // ✅ Brugbar hvis du stadig bruger .Get(i,j)
    public double Get(int i, int j)
    {
        return data[i, j];
    }

    public void Set(int i, int j, double value)
    {
        data[i, j] = value;
    }

    public Matrix Copy()
    {
        var copy = new Matrix(Rows, Cols);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                copy[i, j] = this[i, j];
        return copy;
    }

    public static Matrix Identity(int n)
    {
        var I = new Matrix(n, n);
        for (int i = 0; i < n; i++)
            I[i, i] = 1.0;
        return I;
    }

    public void SetCol(int col, Vector v)
    {
        for (int i = 0; i < Rows; i++)
            data[i, col] = v[i];
    }

    public Vector GetCol(int col)
    {
        var v = new Vector(Rows);
        for (int i = 0; i < Rows; i++)
            v[i] = data[i, col];
        return v;
    }

    public Matrix Transpose()
    {
        var T = new Matrix(Cols, Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                T[j, i] = data[i, j];
        return T;
    }

    public static Matrix operator *(Matrix A, Matrix B)
    {
        if (A.Cols != B.Rows)
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        var result = new Matrix(A.Rows, B.Cols);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < B.Cols; j++)
                for (int k = 0; k < A.Cols; k++)
                    result[i, j] += A[i, k] * B[k, j];
        return result;
    }

    public static Matrix operator *(Matrix A, double scalar)
    {
        var result = new Matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                result[i, j] = A[i, j] * scalar;
        return result;
    }

    public static Vector operator *(Matrix A, Vector v)
    {
        if (A.Cols != v.Size)
            throw new ArgumentException("Matrix and vector dimensions do not match.");
        var result = new Vector(A.Rows);
        for (int i = 0; i < A.Rows; i++)
        {
            double sum = 0;
            for (int j = 0; j < A.Cols; j++)
                sum += A[i, j] * v[j];
            result[i] = sum;
        }
        return result;
    }

    public bool IsUpTri()
    {
        for (int i = 1; i < Rows; i++)
            for (int j = 0; j < i && j < Cols; j++)
                if (Math.Abs(data[i, j]) > 1e-10)
                    return false;
        return true;
    }

    public static bool Compare(Matrix A, Matrix B, double tol = 1e-10)
    {
        if (A.Rows != B.Rows || A.Cols != B.Cols) return false;
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > tol)
                    return false;
        return true;
    }

    public void Print(string label = "")
    {
        if (!string.IsNullOrEmpty(label))
            Console.WriteLine(label);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
                Console.Write($"{data[i, j]:0.0000} ");
            Console.WriteLine();
        }
    }
}
