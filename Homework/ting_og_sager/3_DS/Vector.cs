using System;

public class Vector
{
    private double[] data;

    public Vector(int size)
    {
        data = new double[size];
    }

    public Vector(double[] data)
    {
        this.data = (double[])data.Clone();
    }

    public int Length => data.Length;

    public double this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public static Vector operator *(Matrix matrix, Vector vector)
    {
        if (matrix.Cols != vector.Length)
            throw new ArgumentException("Matrix columns must match vector length.");

        Vector result = new Vector(matrix.Rows);
        for (int i = 0; i < matrix.Rows; i++)
        {
            double sum = 0.0;
            for (int j = 0; j < matrix.Cols; j++)
            {
                sum += matrix[i, j] * vector[j];
            }
            result[i] = sum;
        }
        return result;
    }
}
