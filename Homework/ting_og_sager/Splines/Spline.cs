using System;

public class LinearSpline {
    public double[] x, y;
    public LinearSpline(double[] xs, double[] ys) {
        // Store copies of the data arrays
        x = new double[xs.Length];
        y = new double[ys.Length];
        Array.Copy(xs, x, xs.Length);
        Array.Copy(ys, y, ys.Length);
    }
    public double Evaluate(double z) {
        int i = Program.binsearch(x, z);
        double dx = x[i+1] - x[i];
        if (!(dx > 0)) throw new Exception("uups...");
        double dy = y[i+1] - y[i];
        return y[i] + dy/dx * (z - x[i]);
    }
    public double Integral(double z) {
        // Integral of linear spline from x[0] to z
        int j = Program.binsearch(x, z);
        double sum = 0.0;
        // Sum full trapezoids for all intervals completely before z
        for (int i = 0; i < j; i++) {
            double h = x[i+1] - x[i];
            double area = (y[i] + y[i+1]) * h / 2.0;
            sum += area;
        }
        // Add partial trapezoid for interval j from x[j] to z
        double h_j = x[j+1] - x[j];
        if (!(h_j > 0)) throw new Exception("uups...");
        double t = z - x[j]; // interval length from x[j] to z
        double y_z = y[j] + (y[j+1] - y[j]) / h_j * t;  // linear interpolation at z
        double area_j = (y[j] + y_z) * t / 2.0;
        sum += area_j;
        return sum;
    }
}

public class qspline {
    private double[] x, y;     // data points
    private double[] b, c;     // spline coefficients for each interval
    public qspline(double[] xs, double[] ys) {
        // Copy input data
        int n = xs.Length;
        x = new double[n];
        y = new double[n];
        Array.Copy(xs, x, n);
        Array.Copy(ys, y, n);
        if (n < 2) throw new Exception("Need at least two points for spline");
        int m = n - 1;  // number of intervals
        b = new double[m];
        c = new double[m];

        // Precompute interval lengths
        double[] h = new double[m];
        for (int i = 0; i < m; i++) {
            h[i] = x[i+1] - x[i];
            if (!(h[i] > 0)) throw new Exception("xs must be strictly increasing");
        }

        // Allocate c coefficients (note: c[m-1] = 0 for natural boundary)
        c[m-1] = 0.0;
        // Solve for c[0..m-2] by back-substitution (derivative continuity conditions)
        // Equation: h[i]^2 * h[i+1] * c[i] + h[i] * h[i+1]^2 * c[i+1] = (y[i+2]-y[i+1])*h[i] - (y[i+1]-y[i])*h[i+1]
        double[] D = new double[m-1];  // right-hand side differences
        for (int i = 0; i < m-1; i++) {
            D[i] = (y[i+2] - y[i+1]) * h[i] - (y[i+1] - y[i]) * h[i+1];
        }
        // Back-substitution: start from last equation for c[m-2]
        if (m-2 >= 0) {
            c[m-2] = D[m-2] / (h[m-2] * h[m-1] * h[m-2]);  // Actually this formula simplifies because c[m-1]=0
            // The coefficient for c[m-2] (A term) is h[m-2]^2 * h[m-1], 
            // and B term (with c[m-1]) drops out since c[m-1]=0.
            c[m-2] = D[m-2] / (h[m-2] * h[m-2] * h[m-1]);
        }
        for (int i = m - 3; i >= 0; i--) {
            // A_i = h[i]^2 * h[i+1], B_i = h[i] * h[i+1]^2
            double A = h[i] * h[i] * h[i+1];
            double B = h[i] * h[i+1] * h[i+1];
            // Solve: A * c[i] + B * c[i+1] = D[i]  =>  c[i] = (D[i] - B * c[i+1]) / A
            c[i] = (D[i] - B * c[i+1]) / A;
        }

        // Compute b coefficients for each interval
        for (int i = 0; i < m; i++) {
            b[i] = (y[i+1] - y[i]) / h[i] - c[i] * h[i];
        }
    }
    public double evaluate(double z) {
        int i = Program.binsearch(x, z);
        double dx = z - x[i];
        return y[i] + b[i] * dx + c[i] * dx * dx;
    }
    public double derivative(double z) {
        int i = Program.binsearch(x, z);
        double dx = z - x[i];
        // First derivative: b[i] + 2*c[i]*(x - x[i])
        return b[i] + 2 * c[i] * dx;
    }
    public double integral(double z) {
        // Definite integral from x[0] to z of the quadratic spline
        int j = Program.binsearch(x, z);
        double sum = 0.0;
        for (int i = 0; i < j; i++) {
            double h = x[i+1] - x[i];
            // ∫_{x_i}^{x_{i+1}} [y_i + b_i*(x-x_i) + c_i*(x-x_i)^2] dx
            // = y_i*h + b_i*h^2/2 + c_i*h^3/3
            sum += y[i] * h + b[i] * (h * h / 2.0) + c[i] * (h * h * h / 3.0);
        }
        // partial segment j from x[j] to z
        double t = z - x[j];
        sum += y[j] * t + b[j] * (t * t / 2.0) + c[j] * (t * t * t / 3.0);
        return sum;
    }
}

public class cspline {
    private double[] x, y;       // data points
    private double[] b, c, d;    // spline coefficients for each interval
    public cspline(double[] xs, double[] ys) {
        // Copy input data
        int n = xs.Length;
        if (n < 2) throw new Exception("Need at least two points for spline");
        x = new double[n];
        y = new double[n];
        Array.Copy(xs, x, n);
        Array.Copy(ys, y, n);
        int m = n - 1;
        b = new double[m];
        c = new double[m];
        d = new double[m];

        // Compute interval lengths
        double[] h = new double[m];
        for (int i = 0; i < m; i++) {
            h[i] = x[i+1] - x[i];
            if (!(h[i] > 0)) throw new Exception("xs must be strictly increasing");
        }

        // Solve tridiagonal system for second derivatives (sigma):
        // We solve for p[i] = second derivative at x[i].
        double[] p = new double[n];
        p[0] = 0.0;
        p[n-1] = 0.0;  // natural spline boundary conditions
        if (n > 2) {
            // Setup arrays for Thomas algorithm
            double[] D = new double[n];    // diagonal
            double[] Q = new double[n];    // right-hand side
            double[] B = new double[n];    // temporary for elimination
            // Index 1..n-2 used for interior equations
            D[1] = 2 * (h[0] + h[1]);
            Q[1] = 6 * ((y[2] - y[1]) / h[1] - (y[1] - y[0]) / h[0]);
            for (int i = 2; i <= n-3; i++) {
                D[i] = 2 * (h[i-1] + h[i]);
                Q[i] = 6 * ((y[i+1] - y[i]) / h[i] - (y[i] - y[i-1]) / h[i-1]);
            }
            D[n-2] = 2 * (h[n-3] + h[n-2]);
            Q[n-2] = 6 * ((y[n-1] - y[n-2]) / h[n-2] - (y[n-2] - y[n-3]) / h[n-3]);
            // Forward elimination
            for (int i = 2; i <= n-2; i++) {
                double w = h[i-1] / D[i-1];
                D[i] -= w * h[i-1];
                Q[i] -= w * Q[i-1];
            }
            // Back substitution
            p[n-2] = Q[n-2] / D[n-2];
            for (int i = n-3; i >= 1; --i) {
                p[i] = (Q[i] - h[i] * p[i+1]) / D[i];
            }
            p[0] = 0.0;
            p[n-1] = 0.0;
        }

        // Compute spline coefficients b, c, d for each interval
        for (int i = 0; i < m; i++) {
            // p[i] and p[i+1] are second derivatives at the endpoints of interval i
            // a_i = y[i] (we use y array directly for value)
            // b_i = (y[i+1]-y[i])/h[i] - (2*p[i] + p[i+1])*h[i]/6
            b[i] = (y[i+1] - y[i]) / h[i] - (2 * p[i] + p[i+1]) * h[i] / 6.0;
            // c_i = p[i]/2
            c[i] = p[i] / 2.0;
            // d_i = (p[i+1] - p[i]) / (6*h[i])
            d[i] = (p[i+1] - p[i]) / (6.0 * h[i]);
        }
    }
    public double evaluate(double z) {
        int i = Program.binsearch(x, z);
        double dx = z - x[i];
        // Cubic polynomial: y[i] + b[i]*dx + c[i]*dx^2 + d[i]*dx^3
        return y[i] + b[i] * dx + c[i] * dx * dx + d[i] * dx * dx * dx;
    }
    public double derivative(double z) {
        int i = Program.binsearch(x, z);
        double dx = z - x[i];
        // First derivative: b_i + 2*c_i*dx + 3*d_i*dx^2
        return b[i] + 2 * c[i] * dx + 3 * d[i] * dx * dx;
    }
    public double secondDerivative(double z) {
        int i = Program.binsearch(x, z);
        double dx = z - x[i];
        // Second derivative: 2*c_i + 6*d_i*dx
        return 2 * c[i] + 6 * d[i] * dx;
    }
    public double integral(double z) {
        // Definite integral of spline from x[0] to z
        int j = Program.binsearch(x, z);
        double sum = 0.0;
        for (int i = 0; i < j; i++) {
            double h = x[i+1] - x[i];
            // ∫_{x_i}^{x_{i+1}} [y_i + b_i*(x-x_i) + c_i*(x-x_i)^2 + d_i*(x-x_i)^3] dx
            // = y_i*h + b_i*h^2/2 + c_i*h^3/3 + d_i*h^4/4
            sum += y[i] * h + b[i] * (h * h / 2.0) + c[i] * (h * h * h / 3.0) + d[i] * (h * h * h * h / 4.0);
        }
        double t = z - x[j];
        sum += y[j] * t + b[j] * (t * t / 2.0) + c[j] * (t * t * t / 3.0) + d[j] * (t * t * t * t / 4.0);
        return sum;
    }
}
