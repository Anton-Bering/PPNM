using System;
using System.Text;

public sealed class vector {
    private readonly double[] data;
    public int Size { get; }

    // --- Constructors ---
    public vector(int n) {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
        Size = n;
        data = new double[n];
    }

    public vector(double[] values, bool copy = true) {
        if (values == null) throw new ArgumentNullException(nameof(values));
        Size = values.Length;
        data = copy ? (double[])values.Clone() : values;
    }

    // --- Indexer (bounds-checked) ---
    public double this[int i] {
        get {
            if ((uint)i >= (uint)Size) throw new IndexOutOfRangeException($"vector index {i} out of [0,{Size})");
            return data[i];
        }
        set {
            if ((uint)i >= (uint)Size) throw new IndexOutOfRangeException($"vector index {i} out of [0,{Size})");
            data[i] = value;
        }
    }

    // --- Utilities ---
    public vector Copy() => new vector(data, copy: true);
    public double[] ToArrayCopy() => (double[])data.Clone();

    // --- Algebra ---
    public static vector operator +(vector a, vector b) {
        if (a.Size != b.Size) throw new ArgumentException("vector sizes must match for addition");
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a.data[i] + b.data[i];
        return r;
    }

    public static vector operator -(vector a, vector b) {
        if (a.Size != b.Size) throw new ArgumentException("vector sizes must match for subtraction");
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a.data[i] - b.data[i];
        return r;
    }

    public static vector operator -(vector a) {
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = -a.data[i];
        return r;
    }

    public static vector operator *(vector a, double c) {
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a.data[i] * c;
        return r;
    }
    public static vector operator *(double c, vector a) => a * c;

    public static vector operator /(vector a, double c) {
        if (c == 0.0) throw new DivideByZeroException();
        var r = new vector(a.Size);
        double inv = 1.0 / c;
        for (int i = 0; i < a.Size; i++) r[i] = a.data[i] * inv;
        return r;
    }

    // --- Dot & Norms ---
    public static double Dot(vector a, vector b) {
        if (a.Size != b.Size) throw new ArgumentException("vector sizes must match for dot product");
        double s = 0.0;
        for (int i = 0; i < a.Size; i++) s += a.data[i] * b.data[i];
        return s;
    }

    public double Norm() {
        double s = 0.0;
        for (int i = 0; i < Size; i++) s += data[i] * data[i];
        return Math.Sqrt(s);
    }

    public double NormInf() {
        double m = 0.0;
        for (int i = 0; i < Size; i++) {
            double v = Math.Abs(data[i]);
            if (v > m) m = v;
        }
        return m;
    }

    public static vector Create(params double[] values) // tilføje til eksamens-prosjekt
    {
        return new vector(values);
    }

    // --- Formatting ---
    public override string ToString() => ToString("G6");
    public string ToString(string format, int cols = 0) {
        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < Size; i++) {
            if (i > 0) sb.Append(' ');
            if (cols > 0) sb.AppendFormat("{0," + cols + ":" + format + "}", data[i]);
            else sb.AppendFormat("{0:" + format + "}", data[i]);
        }
        sb.Append(']');
        return sb.ToString();
    }
}
