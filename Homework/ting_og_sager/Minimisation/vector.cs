using System;
public class vector {
    public double[] data;
    public int size { get => data.Length; }
    public vector(int n) {
        data = new double[n];
    }
    public vector(params double[] entries) {
        data = new double[entries.Length];
        for(int i=0; i<entries.Length; i++) {
            data[i] = entries[i];
        }
    }
    // Indexer
    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }
    // Copy method
    public vector copy() {
        vector v = new vector(size);
        for(int i=0; i<size; i++) v[i] = data[i];
        return v;
    }
    // Norm (Euclidean length)
    public double norm() {
        double sum = 0;
        for(int i=0; i<size; i++) sum += data[i]*data[i];
        return Math.Sqrt(sum);
    }
    // Dot product
    public double dot(vector other) {
        double sum = 0;
        for(int i=0; i<size; i++) sum += this[i]*other[i];
        return sum;
    }
    // Operator overloads for convenience
    public static vector operator+(vector u, vector v) {
        if(u.size != v.size) throw new Exception("Vector sizes do not match");
        vector r = new vector(u.size);
        for(int i=0; i<u.size; i++) r[i] = u[i] + v[i];
        return r;
    }
    public static vector operator-(vector u) {
        vector r = new vector(u.size);
        for(int i=0; i<u.size; i++) r[i] = -u[i];
        return r;
    }
    public static vector operator-(vector u, vector v) {
        if(u.size != v.size) throw new Exception("Vector sizes do not match");
        vector r = new vector(u.size);
        for(int i=0; i<u.size; i++) r[i] = u[i] - v[i];
        return r;
    }
    public static vector operator*(double c, vector v) {
        vector r = new vector(v.size);
        for(int i=0; i<v.size; i++) r[i] = c * v[i];
        return r;
    }
    public static vector operator*(vector v, double c) {
        return c * v;
    }
    // Override ToString for easy display
    public override string ToString() {
        return "(" + string.Join(", ", data) + ")";
    }
}
