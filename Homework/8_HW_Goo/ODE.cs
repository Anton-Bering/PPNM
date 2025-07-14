using System;
using System.Collections.Generic;
using static System.Math;

public static class ODE {
    public static (List<double>, List<vector>) driver(
        Func<double, vector, vector> f,
        (double, double) interval,
        vector ystart,
        double hstart = 0.01,
        double acc = 1e-3,
        double eps = 1e-3
    ) {
        var (a, b) = interval;
        double x = a;
        vector y = ystart.copy();
        var xlist = new List<double>() { x };
        var ylist = new List<vector>() { y };
        double h = hstart;

        while (x < b) {
            if (x + h > b) h = b - x;
            var (yh, erv) = rkstep45(f, x, y, h);
            double tol = (acc + eps * yh.norm()) * Sqrt(h / (b - a));
            double err = erv.norm();
            if (err <= tol) {
                x += h;
                y = yh;
                xlist.Add(x);
                ylist.Add(y);
            }
            h *= Min(Pow(tol / err, 0.25) * 0.95, 2);
        }
        return (xlist, ylist);
    }

    private static (vector, vector) rkstep45(Func<double, vector, vector> f, double x, vector y, double h) {
        vector k1 = f(x, y);
        vector k2 = f(x + 1.0 / 4 * h, y + 1.0 / 4 * k1 * h);
        vector k3 = f(x + 3.0 / 8 * h, y + (3.0 / 32 * k1 + 9.0 / 32 * k2) * h);
        vector k4 = f(x + 12.0 / 13 * h, y + (1932.0 / 2197 * k1 - 7200.0 / 2197 * k2 + 7296.0 / 2197 * k3) * h);
        vector k5 = f(x + h, y + (439.0 / 216 * k1 - 8 * k2 + 3680.0 / 513 * k3 - 845.0 / 4104 * k4) * h);
        vector k6 = f(x + 1.0 / 2 * h, y + (-8.0 / 27 * k1 + 2 * k2 - 3544.0 / 2565 * k3 + 1859.0 / 4104 * k4 - 11.0 / 40 * k5) * h);

        vector yh = y + (16.0 / 135 * k1 + 6656.0 / 12825 * k3 + 28561.0 / 56430 * k4 - 9.0 / 50 * k5 + 2.0 / 55 * k6) * h;
        vector y_star = y + (25.0 / 216 * k1 + 1408.0 / 2565 * k3 + 2197.0 / 4104 * k4 - 1.0 / 5 * k5) * h;
        vector erv = yh - y_star;
        return (yh, erv);
    }
}
