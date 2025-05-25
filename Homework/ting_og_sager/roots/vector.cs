using System;

public class vector {
    public double[] data;
    public int size => data.Length;

    // Constructor: vector of given length (filled with 0)
    public vector(int n) {
        data = new double[n];
    }

    // Constructor: vector from variable number of elements
    public vector(params double[] entries) {
        data = new double[entries.Length];
        for (int i = 0; i < entries.Length; i++) {
            data[i] = entries[i];
        }
    }

    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }

    public vector copy() {
        return new vector((double[])data.Clone());
    }

    public double norm() {
        double sum = 0;
        for (int i = 0; i < size; i++) {
            sum += data[i] * data[i];
        }
        return Math.Sqrt(sum);
    }

    public double dot(vector other) {
        if (this.size != other.size)
            throw new ArgumentException("Vectors must be same size for dot product");
        double sum = 0;
        for (int i = 0; i < size; i++) {
            sum += this[i] * other[i];
        }
        return sum;
    }

    public vector map(Func<double, double> f) {
        vector result = new vector(size);
        for (int i = 0; i < size; i++) {
            result[i] = f(this[i]);
        }
        return result;
    }

    public override string ToString() {
        return "[" + string.Join(", ", data) + "]";
    }

    // Addition: u + v
    public static vector operator +(vector u, vector v) {
        if (u.size != v.size)
            throw new ArgumentException("Vectors must be same size for addition");
        vector result = new vector(u.size);
        for (int i = 0; i < u.size; i++) {
            result[i] = u[i] + v[i];
        }
        return result;
    }

    // Subtraction: u - v
    public static vector operator -(vector u, vector v) {
        if (u.size != v.size)
            throw new ArgumentException("Vectors must be same size for subtraction");
        vector result = new vector(u.size);
        for (int i = 0; i < u.size; i++) {
            result[i] = u[i] - v[i];
        }
        return result;
    }

    // Negation: -v
    public static vector operator -(vector v) {
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++) {
            result[i] = -v[i];
        }
        return result;
    }

    // Scalar multiplication: v * a
    public static vector operator *(vector v, double a) {
        vector result = new vector(v.size);
        for (int i = 0; i < v.size; i++) {
            result[i] = v[i] * a;
        }
        return result;
    }

    // Scalar multiplication: a * v
    public static vector operator *(double a, vector v) {
        return v * a;
    }
}
