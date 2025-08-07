using System;
using System.Collections.Generic;
using static System.Math;

public static class OdeSolver
{
    public static (List<double>, List<Vector>) Solve(Func<double, Vector, Vector> f, double a, Vector ya, double b, double h = 0.01, double acc = 1e-3, double eps = 1e-3)
    {
        double x = a;
        Vector y = ya.Copy();
        var xlist = new List<double> { x };
        var ylist = new List<Vector> { y };
        while (x < b)
        {
            if (x + h > b) h = b - x;
            var (yh, err) = Step(f, x, y, h);
            double tol = (acc + eps * yh.Norm()) * Sqrt(h / (b - a));
            if (err < tol)
            {
                x += h; y = yh;
                xlist.Add(x); ylist.Add(y);
            }
            h *= Min(Pow(tol / err, 0.25) * 0.95, 2);
        }
        return (xlist, ylist);
    }

    private static (Vector, double) Step(Func<double, Vector, Vector> f, double x, Vector y, double h)
    {
        Vector k0 = f(x, y);
        Vector k1 = f(x + h / 2, y + (h / 2) * k0);
        Vector yh = y + h * k1;
        Vector k_half = f(x + h, yh);
        Vector err = (k1 - k_half) * h;
        return (yh, err.Norm());
    }
}