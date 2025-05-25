using System;
public class matrix {
    public readonly int size1, size2;
    private double[] data;
    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[size1 * size2];
    }
    public double this[int i, int j] {
        get => data[i + j * size1];
        set => data[i + j * size1] = value;
    }
    public matrix copy() {
        matrix B = new matrix(size1, size2);
        Array.Copy(this.data, B.data, this.data.Length);
        return B;
    }
}
