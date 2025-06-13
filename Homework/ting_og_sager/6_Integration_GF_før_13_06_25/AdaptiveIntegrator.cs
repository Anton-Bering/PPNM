using System;

class AdaptiveIntegrator {
    // Recursive adaptive integration with open 4-point Newton–Cotes rule (embedded lower-order rule).
    // Handles absolute (acc) and relative (eps) error criteria, reuses function values, 
    // and supports improper integrals via variable transformations.
    public static double Integrate(Func<double,double> f, double a, double b, 
                                   double acc = 1e-6, double eps = 1e-6) {
        double err;
        double result = Integrate(f, a, b, acc, eps, out err);
        return result;
    }

    public static double Integrate(Func<double,double> f, double a, double b, 
                                   double acc, double eps, out double error, 
                                   double f2 = Double.NaN, double f3 = Double.NaN) {
        // Handle infinite limit(s) by transforming to finite interval(s):
        if (Double.IsInfinity(a) || Double.IsInfinity(b)) {
            if (Double.IsNegativeInfinity(a) && Double.IsPositiveInfinity(b)) {
                // Case: a = -∞, b = +∞. Split into two half-infinite integrals at 0.
                double errLeft, errRight;
                double mid = 0.0;
                double resLeft  = Integrate(f, a, mid, acc/2, eps, out errLeft);
                double resRight = Integrate(f, mid, b, acc/2, eps, out errRight);
                error = errLeft + errRight;
                return resLeft + resRight;
            } else if (Double.IsPositiveInfinity(b)) {
                // Case: b = +∞. Substitute x = a + t/(1-t), t ∈ [0,1). dx/dt = 1/(1-t)^2.
                Func<double,double> g = (t) => {
                    double x = a + t/(1 - t);
                    return f(x) * 1.0/((1 - t)*(1 - t));  // f(x) * dx/dt
                };
                double result = Integrate(g, 0.0, 1.0, acc, eps, out error);
                return result;
            } else if (Double.IsNegativeInfinity(a)) {
                // Case: a = -∞. Substitute x = b - t/(1-t), t ∈ [0,1). 
                Func<double,double> g = (t) => {
                    double x = b - t/(1 - t);
                    return f(x) * 1.0/((1 - t)*(1 - t));
                };
                double result = Integrate(g, 0.0, 1.0, acc, eps, out error);
                return result;
            } else {
                // Handle cases where limits are swapped (if a = +∞ and b finite, etc.)
                if (Double.IsPositiveInfinity(a)) {
                    // ∫_b^∞ f(x) dx = -∫_∞^b f(x) dx (swap limits)
                    double res = Integrate(f, b, Double.PositiveInfinity, acc, eps, out error);
                    return -res;
                }
                if (Double.IsNegativeInfinity(b)) {
                    double res = Integrate(f, Double.NegativeInfinity, a, acc, eps, out error);
                    return -res;
                }
            }
        }

        // Finite interval [a, b]: perform adaptive open quadrature
        double h = b - a;
        if (Double.IsNaN(f2)) {
            // First call: compute interior points at 1/3 and 2/3 of the interval
            f2 = f(a + 2*h/6);
            f3 = f(a + 4*h/6);
        }
        // Compute additional points at 1/6 and 5/6 of [a,b]
        double f1 = f(a + 1*h/6);
        double f4 = f(a + 5*h/6);
        // 4-point open rule (higher order) and embedded 2-point rule (lower order)
        double Q = (2*f1 + f2 + f3 + 2*f4) / 6 * h;   // higher-order estimate
        double q = (f1 + f2 + f3 + f4) / 4 * h;       // lower-order estimate
        error = Math.Abs(Q - q);
        if (error <= acc + eps * Math.Abs(Q)) {
            // Acceptable accuracy achieved
            return Q;
        } else {
            // Subdivide interval [a,b] and integrate recursively
            double mid = a + h/2;
            double err1, err2;
            // Reuse points for subintervals:
            // Left [a, mid]: pass f1 (at 1/6) and f2 (at 1/3) as interior reuse points
            double left  = Integrate(f, a, mid, acc/Math.Sqrt(2), eps, out err1, f1, f2);
            // Right [mid, b]: pass f3 (at 2/3) and f4 (at 5/6) as interior reuse points
            double right = Integrate(f, mid, b, acc/Math.Sqrt(2), eps, out err2, f3, f4);
            error = err1 + err2;  // sum error estimates from subintervals
            return left + right;
        }
    }

    public static double IntegrateClenshawCurtis(Func<double,double> f, double a, double b, 
                                                 double acc = 1e-6, double eps = 1e-6) {
        // Apply Clenshaw–Curtis variable transformation to handle endpoint singularities.
        // x = (a+b)/2 + (b-a)/2 * cos(θ), θ∈[0,π]; dx = -(b-a)/2 * sin(θ) dθ.
        double center   = (a + b) / 2;
        double halfWidth = (b - a) / 2;
        // Transformed integrand g(θ) = f(x(θ)) * |dx/dθ| = f((a+b)/2 + (b-a)/2*cosθ) * (b-a)/2 * sinθ.
        Func<double,double> g = (theta) => {
            double x = center + halfWidth * Math.Cos(theta);
            return f(x) * halfWidth * Math.Sin(theta);
        };
        double err;
        double result = Integrate(g, 0.0, Math.PI, acc, eps, out err);
        return result;
    }
}
