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
    public static vector operator* (double a, vector v) {
        vector u = new vector(v.size);
        for (int i = 0; i < v.size; i++)
            u[i] = a * v[i];
        return u;
    }
    public static vector operator+ (vector u, vector v) {
        if (u.size != v.size) 
            throw new ArgumentException("Vector dimensions mismatch for addition");
        vector w = new vector(u.size);
        for (int i = 0; i < w.size; i++)
            w[i] = u[i] + v[i];
        return w;
    }
    public override string ToString() {
        return string.Join(" ", data);
    }
}
