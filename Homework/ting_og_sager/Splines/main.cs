using System;
using static System.Math;

public class Program {
    // Static methods for linear interpolation (Part A)
    public static int binsearch(double[] x, double z) {
        /* locates the interval for z by bisection */
        if (z < x[0] || z > x[x.Length - 1])
            throw new Exception("binsearch: bad z");
        int i = 0, j = x.Length - 1;
        while (j - i > 1) {
            int mid = (i + j) / 2;
            if (z > x[mid]) i = mid; else j = mid;
        }
        return i;
    }
    public static double linterp(double[] x, double[] y, double z) {
        int i = binsearch(x, z);
        double dx = x[i+1] - x[i];
        if (!(dx > 0)) throw new Exception("uups...");
        double dy = y[i+1] - y[i];
        return y[i] + dy/dx * (z - x[i]);
    }
    public static double linterpInteg(double[] x, double[] y, double z) {
        // Integral of linear interpolant from x[0] to z
        int j = binsearch(x, z);
        double sum = 0.0;
        for (int i = 0; i < j; i++) {
            double h = x[i+1] - x[i];
            double area = (y[i] + y[i+1]) * h / 2.0;
            sum += area;
        }
        double h_j = x[j+1] - x[j];
        if (!(h_j > 0)) throw new Exception("uups...");
        double t = z - x[j];
        double y_z = y[j] + (y[j+1] - y[j]) / h_j * t;
        double area_j = (y[j] + y_z) * t / 2.0;
        sum += area_j;
        return sum;
    }

    public static void Main(string[] args) {
        // Example data for testing:
        // Linear spline test with y = cos(x) for x=0..9
        double[] x_cos = new double[10];
        double[] y_cos = new double[10];
        for (int i = 0; i < 10; i++) {
            x_cos[i] = i;
            y_cos[i] = Cos(x_cos[i]);
        }
        // Quadratic and Cubic spline test with y = sin(x) for x=0..9
        double[] x_sin = new double[10];
        double[] y_sin = new double[10];
        for (int i = 0; i < 10; i++) {
            x_sin[i] = i;
            y_sin[i] = Sin(x_sin[i]);
        }

        // Build splines
        qspline quadSpline = new qspline(x_sin, y_sin);
        cspline cubicSpline = new cspline(x_sin, y_sin);

        // Linear spline interpolation output
        Console.WriteLine("Linear spline interpolation (cosine data):");
        Console.WriteLine(" x       cos(x)     LinearInterp");
        double[] testZ_lin = {0.5, 2.5, 4.5, 6.5, 8.5};
        foreach (double z in testZ_lin) {
            double actual = Cos(z);
            double interp = linterp(x_cos, y_cos, z);
            Console.WriteLine($"{z,4:F1}   {actual,8:F6}   {interp,12:F6}");
        }
        // Linear spline integral vs actual
        double linInteg = linterpInteg(x_cos, y_cos, 9.0);
        double actualCosInteg = Sin(9.0) - Sin(0.0);  // ∫_0^9 cos(x) dx = Sin(9) - Sin(0)
        Console.WriteLine($"\nLinear spline integral [0,9] = {linInteg:F6}");
        Console.WriteLine($"Actual integral of cos(x) [0,9] = {actualCosInteg:F6}");

        // Quadratic vs Cubic spline interpolation output for sine data
        Console.WriteLine("\nQuadratic vs Cubic spline interpolation (sine data):");
        Console.WriteLine(" x       sin(x)    qspline     cspline");
        double[] testZ = {0.5, 2.5, 4.5, 6.5, 8.5};
        foreach (double z in testZ) {
            double actual = Sin(z);
            double qval = quadSpline.evaluate(z);
            double cval = cubicSpline.evaluate(z);
            Console.WriteLine($"{z,4:F1}   {actual,8:F6}   {qval,9:F6}   {cval,9:F6}");
        }

        // First derivative comparison
        Console.WriteLine("\nSpline first derivative vs actual cos(x):");
        Console.WriteLine(" x       cos(x)    qspline'    cspline'");
        double[] testZ_deriv = {2.5, 4.5, 6.5};
        foreach (double z in testZ_deriv) {
            double actualD = Cos(z);
            double qder = quadSpline.derivative(z);
            double cder = cubicSpline.derivative(z);
            Console.WriteLine($"{z,4:F1}   {actualD,8:F6}   {qder,9:F6}   {cder,9:F6}");
        }

        // Second derivative (cubic) comparison
        Console.WriteLine("\nCubic spline second derivative vs actual -sin(x):");
        Console.WriteLine(" x      -sin(x)    cspline''");
        double[] testZ_sec = {2.5, 4.5, 6.5};
        foreach (double z in testZ_sec) {
            double actualSec = -Sin(z);
            double csec = cubicSpline.secondDerivative(z);
            Console.WriteLine($"{z,4:F1}   {actualSec,8:F6}   {csec,10:F6}");
        }

        // Definite integrals from 0 to 9 for sine data
        double quadInteg = quadSpline.integral(9.0);
        double cubicInteg = cubicSpline.integral(9.0);
        double actualSinInteg = -Cos(9.0) + Cos(0.0);  // ∫_0^9 sin(x) dx = 1 - cos(9)
        Console.WriteLine("\nDefinite integrals from 0 to 9 (sine data):");
        Console.WriteLine($"Quadratic spline integral = {quadInteg:F6}");
        Console.WriteLine($"Cubic spline integral     = {cubicInteg:F6}");
        Console.WriteLine($"Actual integral of sin(x) = {actualSinInteg:F6}");
    }
}
