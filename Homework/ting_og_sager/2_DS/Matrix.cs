using System;

public class Matrix
{
    public int Size1 { get; }
    public int Size2 { get; }
    private double[,] data;

    public Matrix(int size1, int size2)
    {
        Size1 = size1;
        Size2 = size2;
        data = new double[size1, size2];
    }

    public double this[int i, int j]
    {
        get => data[i, j];
        set => data[i, j] = value;
    }

    public static Matrix Identity(int n)
    {
        Matrix m = new Matrix(n, n);
        for (int i = 0; i < n; i++)
            m[i, i] = 1.0;
        return m;
    }

    public Matrix Copy()
    {
        Matrix copy = new Matrix(Size1, Size2);
        for (int i = 0; i < Size1; i++)
            for (int j = 0; j < Size2; j++)
                copy[i, j] = this[i, j];
        return copy;
    }
}