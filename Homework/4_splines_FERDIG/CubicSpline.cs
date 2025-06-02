using System;
public class CubicSpline {
    private double[] x;
    private double[] y;
    private double[] b;
    private double[] c;
    private double[] d;

    public CubicSpline(double[] xs, double[] ys) {
        if(xs.Length != ys.Length) 
            throw new ArgumentException("x and y arrays must have same length");
        if(xs.Length < 2) 
            throw new ArgumentException("Need at least two data points for spline");
        int n = xs.Length;
        x = new double[n];
        y = new double[n];
        for(int k = 0; k < n; k++) {
            x[k] = xs[k];
            y[k] = ys[k];
        }
        b = new double[n-1];
        c = new double[n-1];
        d = new double[n-1];

        if(n == 2) {
            // With two points, the cubic spline is just a straight line
            double h = x[1] - x[0];
            if(!(h > 0)) throw new Exception("CubicSpline: x must be strictly increasing");
            b[0] = (y[1] - y[0]) / h;
            c[0] = 0;
            d[0] = 0;
            return;
        }
        // Compute interval widths
        double[] hArr = new double[n-1];
        for(int i = 0; i < n-1; i++) {
            hArr[i] = x[i+1] - x[i];
            if(!(hArr[i] > 0)) throw new Exception("CubicSpline: x must be strictly increasing");
        }
        // Arrays for tridiagonal system
        double[] lower = new double[n];
        double[] diag = new double[n];
        double[] upper = new double[n];
        double[] rhs = new double[n];
        // Set up equations for i = 1 .. n-2
        for(int i = 1; i < n-1; i++) {
            lower[i] = hArr[i-1];
            diag[i] = 2 * (hArr[i-1] + hArr[i]);
            upper[i] = hArr[i];
            rhs[i] = 6 * ((y[i+1] - y[i]) / hArr[i] - (y[i] - y[i-1]) / hArr[i-1]);
        }
        // Solve tridiagonal system for second derivatives m[i]
        // (Using boundary conditions: m[0] = m[n-1] = 0 for natural spline)
        for(int i = 2; i < n-1; i++) {
            double factor = lower[i] / diag[i-1];
            diag[i] -= factor * upper[i-1];
            rhs[i] -= factor * rhs[i-1];
        }
        double[] m = new double[n];
        m[0] = 0;
        m[n-1] = 0;
        m[n-2] = rhs[n-2] / diag[n-2];
        for(int i = n-3; i >= 1; i--) {
            m[i] = (rhs[i] - upper[i] * m[i+1]) / diag[i];
        }
        // Compute spline coefficients for each interval
        for(int i = 0; i < n-1; i++) {
            double h = hArr[i];
            b[i] = (y[i+1] - y[i]) / h - (2*m[i] + m[i+1]) * h / 6.0;
            c[i] = m[i] / 2.0;
            d[i] = (m[i+1] - m[i]) / (6.0 * h);
        }
    }

    public double Evaluate(double z) {
        int i = LinearSpline.binsearch(x, z);
        double dx = z - x[i];
        return y[i] + b[i]*dx + c[i]*dx*dx + d[i]*dx*dx*dx;
    }

    public double Derivative(double z) {
        int i = LinearSpline.binsearch(x, z);
        double dx = z - x[i];
        return b[i] + 2*c[i]*dx + 3*d[i]*dx*dx;
    }

    public double Integral(double z) {
        int i = LinearSpline.binsearch(x, z);
        double total = 0.0;
        // Sum full intervals
        for(int j = 0; j < i; j++) {
            double h = x[j+1] - x[j];
            // ∫ [y_j + b_j*u + c_j*u^2 + d_j*u^3] from 0 to h (where u = x - x_j)
            total += y[j] * h + b[j] * h*h / 2.0 + c[j] * h*h*h / 3.0 + d[j] * h*h*h*h / 4.0;
        }
        // Add partial interval [x_i, z]
        double dx = z - x[i];
        total += y[i] * dx + b[i] * dx*dx / 2.0 + c[i] * dx*dx*dx / 3.0 + d[i] * dx*dx*dx*dx / 4.0;
        return total;
    }
}
