using System;
public class vector {
    public double[] data;
    public int size => data.Length;
    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }
    public vector(int n) {
        data = new double[n];
    }
}
