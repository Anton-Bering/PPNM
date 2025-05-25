using System;
using System.Collections.Generic;

public static class Integrator
{
    /// <summary>
    /// Recursive adaptive integrator using an open 4-point Newton-Cotes rule (Simpson-like).
    /// Estimates ∫_a^b f(x) dx to the specified absolute (acc) or relative (eps) accuracy.
    /// Reuses intermediate point evaluations from previous calls (f2, f3) to minimize function calls.
    /// </summary>
    public static double Integrate(Func<double, double> f, double a, double b,
                                   double acc = 1e-3, double eps = 1e-3,
                                   double f2 = Double.NaN, double f3 = Double.NaN)
    {
        // If interval is reversed, swap and integrate with negative sign (for completeness)
        if (a > b)
        {
            return -Integrate(f, b, a, acc, eps);
        }

        double h = b - a;
        // If this is the first call (no previous midpoint evaluations), evaluate f at 1/3 and 2/3 points
        if (Double.IsNaN(f2) || Double.IsNaN(f3))
        {
            f2 = f(a + 2 * h / 6);  // f at 1/3 of the interval
            f3 = f(a + 4 * h / 6);  // f at 2/3 of the interval
        }
        // Evaluate f at 1/6 and 5/6 points of the interval
        double f1 = f(a + h / 6);
        double f4 = f(a + 5 * h / 6);
        // Higher-order (4-point open) quadrature rule estimate
        double Q = (2 * f1 + f2 + f3 + 2 * f4) / 6 * (b - a);
        // Lower-order embedded quadrature rule estimate
        double q = (f1 + f2 + f3 + f4) / 4 * (b - a);
        double error = Math.Abs(Q - q);
        // Accept integral if error within tolerance: tol = acc + eps * |Q|
        double tolerance = acc + eps * Math.Abs(Q);
        if (error <= tolerance)
        {
            return Q;
        }
        else
        {
            // Subdivide interval [a,b] and integrate each part with half the absolute accuracy (acc/√2)
            double mid = (a + b) / 2;
            double acc2 = acc / Math.Sqrt(2);
            // Note: f1 and f2 become the saved midpoint values for the left subinterval,
            //       f3 and f4 become the saved values for the right subinterval.
            double leftIntegral  = Integrate(f, a, mid, acc2, eps,  f1, f2);
            double rightIntegral = Integrate(f, mid, b, acc2, eps, f3, f4);
            return leftIntegral + rightIntegral;
        }
    }

    /// <summary>
    /// Adaptive integrator with Clenshaw–Curtis variable transformation.
    /// Handles endpoint singularities and infinite limits by transforming the integral:
    /// - For finite [a,b]: uses θ = [0, π] with x = (a+b)/2 + (b-a)/2 * cos(θ).
    /// - For infinite limits: uses t = [0,1] with rational transformation:
    ///     if b = +∞, x = a + t/(1-t);
    ///     if a = -∞, x = b - t/(1-t).
    /// This method calls the base Integrate() internally.
    /// </summary>
    public static double IntegrateClenshawCurtis(Func<double, double> f, double a, double b,
                                                 double acc = 1e-3, double eps = 1e-3)
    {
        // Handle infinite integration limits by transforming to a finite interval
        if (Double.IsPositiveInfinity(b) && !Double.IsInfinity(a))
        {
            // Transform x ∈ [a, ∞) to t ∈ [0,1): x = a + t/(1-t), dx = 1/(1-t)^2 dt
            Func<double, double> h = (t) =>
            {
                double x = a + t / (1 - t);
                double dx_dt = 1.0 / Math.Pow(1 - t, 2);
                return f(x) * dx_dt;
            };
            // Integrate h(t) from t=0 to 1 (open integrator will avoid t=1 endpoint)
            return Integrate(h, 0.0, 1.0, acc, eps);
        }
        if (Double.IsNegativeInfinity(a) && !Double.IsInfinity(b))
        {
            // Transform x ∈ (-∞, b] to t ∈ [0,1): x = b - t/(1-t)
            Func<double, double> h = (t) =>
            {
                double x = b - t / (1 - t);
                double dx_dt = 1.0 / Math.Pow(1 - t, 2);
                return f(x) * dx_dt;
            };
            return Integrate(h, 0.0, 1.0, acc, eps);
        }
        if (Double.IsNegativeInfinity(a) && Double.IsPositiveInfinity(b))
        {
            // Split ∫_{-∞}^{∞} f(x) dx into two halves: ∫_{-∞}^{0} + ∫_{0}^{∞}
            // Use half of the absolute tolerance for each half to meet overall accuracy.
            double acc2 = acc / Math.Sqrt(2);
            // Left half: (-∞ to 0]
            Func<double, double> fLeft = (x) => f(x);
            double leftPart = IntegrateClenshawCurtis(fLeft, Double.NegativeInfinity, 0.0, acc2, eps);
            // Right half: [0 to +∞)
            Func<double, double> fRight = (x) => f(x);
            double rightPart = IntegrateClenshawCurtis(fRight, 0.0, Double.PositiveInfinity, acc2, eps);
            return leftPart + rightPart;
        }

        // If [a,b] are finite: apply Clenshaw–Curtis substitution x = (a+b)/2 + (b-a)/2 * cosθ
        double center = (a + b) / 2;
        double halfWidth = (b - a) / 2;
        // Define transformed integrand g(θ) = f(x(θ)) * (dx/dθ). 
        // Here x(θ) = center + halfWidth * cosθ, and dx/dθ = -halfWidth * sinθ.
        // We take absolute value since ∫_0^π sinθ ... covers the interval.
        Func<double, double> g = (theta) =>
        {
            double x = center + halfWidth * Math.Cos(theta);
            double dx_dtheta = halfWidth * Math.Sin(theta);
            return f(x) * dx_dtheta;
        };
        // Integrate g(θ) from θ=0 to θ=π
        return Integrate(g, 0.0, Math.PI, acc, eps);
    }

    /// <summary>
    /// Compute the error function erf(z) using its integral definition:
    /// erf(z) = 2/√π * ∫_0^z e^{-t^2} dt  (for z >= 0;  odd symmetry for z < 0).
    /// Uses adaptive integration to evaluate the integral.
    /// </summary>
    public static double Erf(double z, double acc = 1e-6, double eps = 0)
    {
        if (z < 0)
        {
            // erf(-z) = -erf(z) (odd symmetry)
            return -Erf(-z, acc, eps);
        }
        // Integrate from 0 to z the function e^{-t^2}
        Func<double, double> integrand = (t) => Math.Exp(-t * t);
        double integral = Integrate(integrand, 0.0, z, acc, eps);
        return (2.0 / Math.Sqrt(Math.PI)) * integral;
    }
}
