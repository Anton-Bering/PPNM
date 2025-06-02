using System;
using System.IO;

public class QuadraticSpline {
    private double[] x;
    private double[] y;
    private double[] b;
    private double[] c;

    public QuadraticSpline(double[] xs, double[] ys) {
        if(xs.Length != ys.Length) 
            throw new ArgumentException("x and y arrays must have same length");
        if(xs.Length < 2) 
            throw new ArgumentException("Need at least two data points for spline");
        int n = xs.Length;

        // Copy input data
        x = new double[n];
        y = new double[n];
        for(int k = 0; k < n; k++) {
            x[k] = xs[k];
            y[k] = ys[k];
        }

        b = new double[n - 1];
        c = new double[n - 1];

        // Precompute interval widths
        double[] h = new double[n - 1];
        for(int i = 0; i < n - 1; i++) {
            h[i] = x[i + 1] - x[i];
            if(!(h[i] > 0)) throw new Exception("QuadraticSpline: x must be strictly increasing");
        }

        // Initial slope for first interval
        b[0] = (y[1] - y[0]) / h[0];

        // Forward recursion to get all b[i]
        for(int i = 0; i < n - 2; i++) {
            b[i + 1] = 2 * (y[i + 2] - y[i + 1]) / h[i + 1] - b[i];
        }

        // Anchor second derivative at first point to zero
        c[0] = 0.0;

        // Compute c[i] for i = 1 .. n-2 using correct formula
        for(int i = 1; i < n - 1; i++) {
            c[i] = (b[i] - b[i - 1]) / (2 * h[i - 1]);
        }

        // Compute c for last interval (i = n-2) using end-point condition (value match)
        int last = n - 2;
        c[last] = (y[n - 1] - y[last] - b[last] * h[last]) / (h[last] * h[last]);
    }

    public double Evaluate(double z) {
        int i = LinearSpline.binsearch(x, z);
        double dx = z - x[i];
        return y[i] + b[i] * dx + c[i] * dx * dx;
    }

    public double Derivative(double z) {
        int i = LinearSpline.binsearch(x, z);
        double dx = z - x[i];
        return b[i] + 2 * c[i] * dx;
    }

    public double Integral(double z) {
        int i = LinearSpline.binsearch(x, z);
        double total = 0.0;

        // Sum full intervals up to interval i-1
        for(int j = 0; j < i; j++) {
            double h = x[j + 1] - x[j];
            total += y[j] * h + b[j] * h * h / 2.0 + c[j] * h * h * h / 3.0;
        }

        // Add partial interval [x_i, z]
        double dx = z - x[i];
        total += y[i] * dx + b[i] * dx * dx / 2.0 + c[i] * dx * dx * dx / 3.0;

        return total;
    }

    public void WriteCoefficientsToFile(string filename) {
        using (StreamWriter writer = new StreamWriter(filename)) {
            writer.WriteLine("# Computed b_i and c_i values from QuadraticSpline class");
            writer.WriteLine("i\tb_i\tc_i");
            for (int i = 0; i < b.Length; i++) {
                writer.WriteLine($"{i}\t{b[i]}\t{c[i]}");
            }
        }
    }
}
