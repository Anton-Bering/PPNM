using System;

public static class ODEModels {
    // Simple harmonic oscillator: u'' = -u  (y0 = u, y1 = u')
    public static vector SHO(double t, vector y) {
        vector dydt = new vector(2);
        dydt[0] = y[1];
        dydt[1] = -y[0];
        return dydt;
    }

    // dy/dx = y  => y(x) = exp(x)
    public static vector ExpGrowth(double t, vector y) {
        vector dydt = new vector(1);
        dydt[0] = y[0];
        return dydt;
    }

    // y'' = 6x => y(x) = x^3
    public static vector PolyTest(double t, vector y) {
        vector dydt = new vector(2);
        dydt[0] = y[1];
        dydt[1] = 6 * t;
        return dydt;
    }

    // Lotka–Volterra predator-prey system (a=1.5, b=1, c=3, d=1)
    public static vector LotkaVolterra(double t, vector y) {
        double a = 1.5, b = 1.0, c = 3.0, d = 1.0;
        double prey = y[0], pred = y[1];
        vector dydt = new vector(2);
        dydt[0] = a * prey - b * prey * pred;
        dydt[1] = -c * pred + d * prey * pred;
        return dydt;
    }

    // Relativistic precession of orbit: u'' + u = 1 + eps*u^2
    public static Func<double, vector, vector> Precession(double eps) {
        return (phi, y) => {
            double u = y[0], up = y[1];
            vector dydφ = new vector(2);
            dydφ[0] = up;
            dydφ[1] = 1 - u + eps * u * u;
            return dydφ;
        };
    }

    // Newtonian three-body problem
    public static vector ThreeBody(double t, vector z) {
        double x1 = z[6],  y1 = z[7];
        double x2 = z[8],  y2 = z[9];
        double x3 = z[10], y3 = z[11];
        double vx1 = z[0], vy1 = z[1];
        double vx2 = z[2], vy2 = z[3];
        double vx3 = z[4], vy3 = z[5];

        double ax1=0, ay1=0, ax2=0, ay2=0, ax3=0, ay3=0;

        AddForce(ref ax1, ref ay1, x1, y1, x2, y2);
        AddForce(ref ax1, ref ay1, x1, y1, x3, y3);
        AddForce(ref ax2, ref ay2, x2, y2, x1, y1);
        AddForce(ref ax2, ref ay2, x2, y2, x3, y3);
        AddForce(ref ax3, ref ay3, x3, y3, x1, y1);
        AddForce(ref ax3, ref ay3, x3, y3, x2, y2);

        vector dzdt = new vector(12);
        dzdt[0] = ax1;  dzdt[1] = ay1;
        dzdt[2] = ax2;  dzdt[3] = ay2;
        dzdt[4] = ax3;  dzdt[5] = ay3;
        dzdt[6] = vx1;  dzdt[7] = vy1;
        dzdt[8] = vx2;  dzdt[9] = vy2;
        dzdt[10] = vx3; dzdt[11] = vy3;
        return dzdt;
    }

    private static void AddForce(ref double ax, ref double ay, double xi, double yi, double xj, double yj) {
        double dx = xj - xi, dy = yj - yi;
        double r2 = dx * dx + dy * dy;
        double r3 = Math.Pow(r2, 1.5);
        ax += dx / r3;
        ay += dy / r3;
    }
}
