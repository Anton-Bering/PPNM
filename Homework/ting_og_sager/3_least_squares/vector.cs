using System;

public class vector {
    public double[] data;
    public int size => data.Length;            // Number of elements
    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }
    public vector(int n) {
        data = new double[n];
    }
    public vector(double[] source) {
        data = new double[source.Length];
        Array.Copy(source, data, source.Length);
    }
}
