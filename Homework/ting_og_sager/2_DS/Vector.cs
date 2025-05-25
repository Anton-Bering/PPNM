using System;

public class Vector
{
    public int Length { get; }
    private double[] data;

    public Vector(int length)
    {
        Length = length;
        data = new double[length];
    }

    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }
}