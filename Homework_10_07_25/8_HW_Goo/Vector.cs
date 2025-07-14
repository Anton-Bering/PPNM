using System;
using static System.Math;

public partial class vector {
    private double[] data;
    public int size => data.Length;

    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }

    public vector(int n) { data = new double[n]; }
    public vector(params double[] list) { data = list; }

    public static implicit operator vector(double[] a) => new vector(a);

    public void print(string s = "", string format = "{0,10:g3} ") {
        Console.Write(s);
        for (int i = 0; i < size; i++) Console.Write(format, this[i]);
        Console.WriteLine();
    }

    public vector copy() {
        return new vector((double[])data.Clone());
    }

    // --- Binære operatorer ---
    public static vector operator +(vector v, vector u) {
        vector r = new vector(v.size);
        for (int i = 0; i < v.size; i++) r[i] = v[i] + u[i];
        return r;
    }

    public static vector operator -(vector v, vector u) {
        vector r = new vector(v.size);
        for (int i = 0; i < v.size; i++) r[i] = v[i] - u[i];
        return r;
    }

    public static vector operator *(vector v, double a) {
        vector r = new vector(v.size);
        for (int i = 0; i < v.size; i++) r[i] = v[i] * a;
        return r;
    }

    public static vector operator *(double a, vector v) => v * a;

    // --- NY OPERATOR TILFØJET ---
    public static vector operator /(vector v, double a) {
        vector r = new vector(v.size);
        for (int i = 0; i < v.size; i++) r[i] = v[i] / a;
        return r;
    }

    // --- NY OPERATOR TILFØJET ---
    public static vector operator -(vector v) {
        vector r = new vector(v.size);
        for (int i = 0; i < v.size; i++) r[i] = -v[i];
        return r;
    }

    public double dot(vector other) {
        double sum = 0;
        for (int i = 0; i < size; i++) sum += this[i] * other[i];
        return sum;
    }

    public double norm() => Sqrt(this.dot(this));

    public vector map(Func<double, double> f) {
        vector v = new vector(this.size);
        for (int i = 0; i < this.size; i++) v[i] = f(this[i]);
        return v;
    }
}
