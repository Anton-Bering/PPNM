using System;
public class vector {
    public double[] data;
    public int size { get { return data.Length; } }
    // Indexer for access
    public double this[int i] {
        get { return data[i]; }
        set { data[i] = value; }
    }
    // Constructor
    public vector(int n) {
        data = new double[n];
    }
    // Copy constructor
    public vector(vector v) {
        data = new double[v.size];
        for (int i = 0; i < v.size; i++) {
            data[i] = v[i];
        }
    }
    // (Optional) Additional methods could be added if needed.
}
