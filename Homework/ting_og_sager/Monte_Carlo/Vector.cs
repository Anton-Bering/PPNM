using System;

public class vector
{
    public double[] data;
    public int size { get => data.Length; }

    public vector(int n)
    {
        data = new double[n];
    }

    public vector(params double[] entries)
    {
        data = new double[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            data[i] = entries[i];
    }

    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }
}
