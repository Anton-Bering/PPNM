using System;
using static System.Math;

public static class RKStepper {
    // Embedded RK 1-2 step (Euler + Midpoint)
    public static (vector, vector) rkstep12(Func<double, vector, vector> f,
                                           double x, vector y, double h) {
        vector k0 = f(x, y);                           // Euler slope
        vector k1 = f(x + h/2, y + k0 * (h/2));        // Midpoint slope
        vector yh = y + k1 * h;                        // higher-order estimate y(x+h)
        vector err = (k1 - k0) * h;                    // error estimate (difference)
        return (yh, err);
    }

    // Embedded RK 2-3 step (Bogacki–Shampine method)
    public static (vector, vector) rkstep23(Func<double, vector, vector> f,
                                           double x, vector y, double h) {
        // Stage slopes
        vector k0 = f(x, y);
        vector k1 = f(x + h/2.0, y + k0 * (h/2.0));
        vector k2 = f(x + 3*h/4.0, y + k1 * (3*h/4.0));
        vector k3 = f(x + h, y + k0 * (2.0/9.0 * h) + k1 * (1.0/3.0 * h) + k2 * (4.0/9.0 * h));
        // 3rd-order (high) and 2nd-order (low) estimates
        vector yh = y + k0 * (2.0/9.0 * h) + k1 * (1.0/3.0 * h) + k2 * (4.0/9.0 * h);
        vector yl = y + k0 * (7.0/24.0 * h) + k1 * (1.0/4.0 * h) + k2 * (1.0/3.0 * h) + k3 * (1.0/8.0 * h);
        vector err = yh - yl;  // error estimate
        return (yh, err);
    }
}
