using System;
using static System.Math;
public class vector {
    private double[] data;
    public int size { get { return data.Length; } }

    // Constructors
    public vector(int n) {
        data = new double[n];
    }
    public vector(params double[] values) {
        data = new double[values.Length];
        values.CopyTo(data, 0);
    }

    // Indexer for element access
    public double this[int i] {
        get { return data[i]; }
        set { data[i] = value; }
    }

    // Return a copy of this vector
    public vector copy() {
        return fromArray(data);
    }
    private static vector fromArray(double[] values) {
        var v = new vector(values.Length);
        values.CopyTo(v.data, 0);
        return v;
    }

    // Euclidean norm (length) of the vector
    public double norm() {
        double sum = 0;
        foreach (double xi in data) {
            sum += xi * xi;
        }
        return Sqrt(sum);
    }

    // Map: apply a function to each component and return a new vector
    public vector map(Func<double, double> func) {
        vector result = new vector(size);
        for (int i = 0; i < size; i++) {
            result[i] = func(data[i]);
        }
        return result;
    }

    // Vector addition
    public static vector operator+(vector v, vector w) {
        if (v.size != w.size) throw new Exception("Vector sizes must match for addition");
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++) {
            result.data[i] = v.data[i] + w.data[i];
        }
        return result;
    }

    // Vector subtraction
    public static vector operator-(vector v, vector w) {
        if (v.size != w.size) throw new Exception("Vector sizes must match for subtraction");
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++) {
            result.data[i] = v.data[i] - w.data[i];
        }
        return result;
    }

    // Scalar multiplication (vector * scalar)
    public static vector operator*(vector v, double c) {
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++) {
            result.data[i] = v.data[i] * c;
        }
        return result;
    }
    // Scalar multiplication (scalar * vector)
    public static vector operator*(double c, vector v) {
        return v * c;
    }
}
