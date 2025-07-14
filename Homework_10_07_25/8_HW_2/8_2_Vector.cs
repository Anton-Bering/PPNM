// ---------- Vector.cs ----------
using System;

public class Vec
{
    public double[] data;
    public int Size => data.Length;

    public Vec(int n) => data = new double[n];
    public Vec(params double[] arr) => data = (double[])arr.Clone();

    // Indexer
    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }

    // Basic operations --------------------------------------------------------
    public static Vec operator +(Vec a, Vec b)
    {
        var r = new Vec(a.Size);
        for (int i = 0; i < a.Size; ++i) r[i] = a[i] + b[i];
        return r;
    }
    public static Vec operator -(Vec a, Vec b)
    {
        var r = new Vec(a.Size);
        for (int i = 0; i < a.Size; ++i) r[i] = a[i] - b[i];
        return r;
    }
    public static Vec operator *(double c, Vec a)
    {
        var r = new Vec(a.Size);
        for (int i = 0; i < a.Size; ++i) r[i] = c * a[i];
        return r;
    }
    public double Norm() => Math.Sqrt(Dot(this));

    public double Dot(Vec other)
    {
        double s = 0;
        for (int i = 0; i < Size; ++i) s += this[i] * other[i];
        return s;
    }

    // Clone helper
    public Vec Copy() => new Vec(data);
}
// ---------------------------------------------------------------------------
