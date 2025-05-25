using System;

public class Vector
{
    private double[] data;

    public int Size => data.Length;

    public Vector(int size)
    {
        data = new double[size];
    }

    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }

    public void Set(int i, double value)
    {
        data[i] = value;
    }

    public double Get(int i)
    {
        return data[i];
    }

    public void Print(string label = "")
    {
        if (!string.IsNullOrEmpty(label))
            Console.WriteLine(label);
        for (int i = 0; i < Size; i++)
            Console.WriteLine($"{data[i]:0.0000}");
    }

    public Vector Copy()
    {
        var copy = new Vector(Size);
        for (int i = 0; i < Size; i++)
            copy[i] = this[i];
        return copy;
    }

    public static bool Compare(Vector a, Vector b, double tol = 1e-10)
    {
        if (a.Size != b.Size) return false;
        for (int i = 0; i < a.Size; i++)
            if (Math.Abs(a[i] - b[i]) > tol)
                return false;
        return true;
    }

    public static Vector operator *(Vector v, double scalar)
    {
        var result = new Vector(v.Size);
        for (int i = 0; i < v.Size; i++)
            result[i] = v[i] * scalar;
        return result;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        if (a.Size != b.Size)
            throw new ArgumentException("Vector sizes do not match for addition.");
        var result = new Vector(a.Size);
        for (int i = 0; i < a.Size; i++)
            result[i] = a[i] + b[i];
        return result;
    }
}
