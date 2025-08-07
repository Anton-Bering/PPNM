using System;

public class Matrix
{
    private double[,] data;
    public int Rows => data.GetLength(0);
    public int Cols => data.GetLength(1);
    public double this[int r, int c] { get => data[r, c]; set => data[r, c] = value; }
    public Matrix(int n) : this(n, n) { }
    public Matrix(int rows, int cols) { data = new double[rows, cols]; }
    public Matrix T() { Matrix t = new Matrix(Cols, Rows); for (int i = 0; i < Rows; i++) for (int j = 0; j < Cols; j++) t[j, i] = this[i, j]; return t; }
    public static Vector operator *(Matrix A, Vector v) { Vector res = new Vector(A.Rows); for (int i = 0; i < A.Rows; i++) { double sum = 0; for (int j = 0; j < A.Cols; j++) sum += A[i, j] * v[j]; res[i] = sum; } return res; }

    public void Print(string s = "")
    {
        Console.Write(s);
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Console.Write($"{this[i, j],12:F4} ");
            }
            Console.WriteLine();
        }
    }
}