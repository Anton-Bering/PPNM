using System;
using System.Collections.Generic;
using static System.Math;

public class vector {
    private double[] data;
    public int size { get { return data.Length; } }

    // Constructors
    public vector(int n) {
        data = new double[n];
    }
    public vector(double[] arr) {
        data = new double[arr.Length];
        Array.Copy(arr, data, arr.Length);
    }

    // Indexer
    public double this[int i] {
        get { return data[i]; }
        set { data[i] = value; }
    }

    // Norm (Euclidean length)
    public double norm() {
        double sum = 0;
        for (int i = 0; i < data.Length; i++) {
            sum += data[i] * data[i];
        }
        return Sqrt(sum);
    }

    // Deep copy
    public vector copy() {
        vector v = new vector(data.Length);
        Array.Copy(this.data, v.data, data.Length);
        return v;
    }

    // Operator overloads for vector arithmetic
    public static vector operator +(vector v1, vector v2) {
        if (v1.size != v2.size) throw new ArgumentException("Vector sizes must match");
        vector result = new vector(v1.size);
        for (int i = 0; i < v1.size; i++)
            result[i] = v1.data[i] + v2.data[i];
        return result;
    }
    public static vector operator -(vector v1, vector v2) {
        if (v1.size != v2.size) throw new ArgumentException("Vector sizes must match");
        vector result = new vector(v1.size);
        for (int i = 0; i < v1.size; i++)
            result[i] = v1.data[i] - v2.data[i];
        return result;
    }
    public static vector operator *(vector v, double c) {
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++)
            result[i] = v.data[i] * c;
        return result;
    }
    public static vector operator *(double c, vector v) {
        return v * c;
    }

    public override string ToString() {
        return string.Join(" ", data);
    }
}
