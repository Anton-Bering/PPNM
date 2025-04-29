using System;
using static System.Math;

public class vec {
    public double x, y, z;

    // Konstruckteres
    public vec() { x = y = z = 0; }
    public vec(double x, double y, double z) { this.x = x; this.y = y; this.z = z; }

    // Matematikeske oprationer
    public static vec operator +(vec u, vec v) => new vec(u.x + v.x, u.y + v.y, u.z + v.z);
    public static vec operator -(vec u) => new vec(-u.x, -u.y, -u.z);
    public static vec operator -(vec u, vec v) => new vec(u.x - v.x, u.y - v.y, u.z - v.z);
    public static vec operator *(vec v, double c) => new vec(c * v.x, c * v.y, c * v.z);
    public static vec operator *(double c, vec v) => v * c;
    public static vec operator /(vec v, double c) => new vec(v.x / c, v.y / c, v.z / c);

    // Dot-produkt og norm
    public double dot(vec other) => x * other.x + y * other.y + z * other.z;
    public static double dot(vec v, vec w) => v.dot(w);
    public double norm() => Sqrt(dot(this));

    // Cross-produkt
    public static vec cross(vec a, vec b) =>
        new vec(a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x);

    public override string ToString() => $"{{ {x}, {y}, {z} }}";

    // print
    public void print(string s = "") => Console.WriteLine($"{s}{this}");

    static bool approx(double a, double b, double acc = 1e-9, double eps = 1e-9) {
        if (Abs(a - b) < acc) return true;
        if (Abs(a - b) < Max(Abs(a), Abs(b)) * eps) return true;
        return false;
    }

    public bool approx(vec other) =>
        approx(this.x, other.x) && approx(this.y, other.y) && approx(this.z, other.z);

    public static bool approx(vec u, vec v) => u.approx(v);
}
