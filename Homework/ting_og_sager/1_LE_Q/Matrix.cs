using System;

// Matrix class representing a 2D matrix of doubles
public class Matrix
{
    private readonly double[,] _data;

    // Property for number of rows
    public int Rows => _data.GetLength(0);

    // Property for number of columns
    public int Cols => _data.GetLength(1);

    // Constructor that initializes the matrix with given dimensions
    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentOutOfRangeException("Matrix dimensions must be positive.");
        _data = new double[rows, cols];
    }

    // Indexer to access or assign matrix elements using [i, j] syntax
    public double this[int row, int col]
    {
        get => _data[row, col];
        set => _data[row, col] = value;
    }

    // Creates a deep copy of the matrix
    public Matrix Copy()
    {
        var copy = new Matrix(Rows, Cols);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                copy[i, j] = this[i, j];
        return copy;
    }

    // Creates an identity matrix of size n x n
    public static Matrix Identity(int size)
    {
        var identity = new Matrix(size, size);
        for (int i = 0; i < size; i++)
            identity[i, i] = 1.0;
        return identity;
    }

    // Returns the specified column as a vector
    public Vector GetCol(int col)
    {
        var v = new Vector(Rows);
        for (int i = 0; i < Rows; i++)
            v[i] = this[i, col];
        return v;
    }

    // Sets the specified column using a vector
    public void SetCol(int col, Vector v)
    {
        if (v.Length != Rows)
            throw new ArgumentException("Vector length must match the number of matrix rows.");
        for (int i = 0; i < Rows; i++)
            this[i, col] = v[i];
    }

    // Returns the transpose of the matrix
    public Matrix Transpose()
    {
        var result = new Matrix(Cols, Rows);
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Cols; j++)
                result[j, i] = this[i, j];
        return result;
    }

    // Matrix-matrix multiplication: A * B
    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Cols != b.Rows)
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");

        var result = new Matrix(a.Rows, b.Cols);
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < b.Cols; j++)
                for (int k = 0; k < a.Cols; k++)
                    result[i, j] += a[i, k] * b[k, j];
        return result;
    }

    // Matrix-vector multiplication: A * v
    public static Vector operator *(Matrix a, Vector v)
    {
        if (a.Cols != v.Length)
            throw new ArgumentException("Matrix and vector dimensions do not match.");

        var result = new Vector(a.Rows);
        for (int i = 0; i < a.Rows; i++)
        {
            double sum = 0.0;
            for (int j = 0; j < a.Cols; j++)
                sum += a[i, j] * v[j];
            result[i] = sum;
        }
        return result;
    }

    // Checks if the matrix is upper triangular (all elements below diagonal ≈ 0)
    public bool IsUpTri()
    {
        for (int i = 1; i < Rows; i++)
            for (int j = 0; j < i && j < Cols; j++)
                if (Math.Abs(this[i, j]) > 1e-10)
                    return false;
        return true;
    }

    // Compares two matrices element-wise using a tolerance
    public static bool Compare(Matrix A, Matrix B, double tol = 1e-10)
    {
        if (A.Rows != B.Rows || A.Cols != B.Cols) return false;
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > tol)
                    return false;
        return true;
    }

    // Prints the matrix with a label and formatted numbers
    public void Print(string label)
    {
        Console.WriteLine(label);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
                Console.Write($"{this[i, j]:0.0000} ");
            Console.WriteLine();
        }
    }
}
