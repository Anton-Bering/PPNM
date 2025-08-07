using System;
using static System.Math;

public class Vector
{
    private double[] data;
    public int Size => data.Length;
    public double this[int i] { get => data[i]; set => data[i] = value; }
    public Vector(int n) { data = new double[n]; }
    public Vector(params double[] values) { data = values; }
    public void Print(string s = "") { Console.Write(s); foreach (var val in data) Console.Write($"{val:F8} "); Console.WriteLine(); }
    public Vector Copy() { return new Vector((double[])data.Clone()); }
    public double Norm() { double sumSq = 0; foreach (var val in data) sumSq += val * val; return Sqrt(sumSq); }
    public Vector Map(Func<double, double> f) { Vector res = new Vector(Size); for (int i = 0; i < Size; i++) res[i] = f(this[i]); return res; }
    public static Vector operator +(Vector v, Vector u) { Vector res = new Vector(v.Size); for (int i = 0; i < v.Size; i++) res[i] = v[i] + u[i]; return res; }
    public static Vector operator -(Vector v, Vector u) { Vector res = new Vector(v.Size); for (int i = 0; i < v.Size; i++) res[i] = v[i] - u[i]; return res; }
    public static Vector operator -(Vector v) { Vector res = new Vector(v.Size); for (int i = 0; i < v.Size; i++) res[i] = -v[i]; return res; }
    public static Vector operator *(double a, Vector v) { Vector res = new Vector(v.Size); for (int i = 0; i < v.Size; i++) res[i] = a * v[i]; return res; }
    public static Vector operator *(Vector v, double a) { return a * v; }

    public static double Dot(Vector v, Vector u) { double sum = 0; for (int i = 0; i < v.Size; i++) sum += v[i] * u[i]; return sum; }
}