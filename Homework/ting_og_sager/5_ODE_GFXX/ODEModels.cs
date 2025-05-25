using System;

public static class ODEModels {
    // Simple harmonic oscillator: u'' = -u  (y0 = u, y1 = u')
    public static vector SHO(double t, vector y) {
        vector dydt = new vector(2);
        dydt[0] = y[1];
        dydt[1] = -y[0];
        return dydt;
    }

    // Lotka–Volterra predator-prey system (a=1.5, b=1, c=3, d=1):contentReference[oaicite:4]{index=4}
    // y0 = prey, y1 = predator
    public static vector LotkaVolterra(double t, vector y) {
        double a = 1.5, b = 1.0, c = 3.0, d = 1.0;
        double prey = y[0], pred = y[1];
        vector dydt = new vector(2);
        dydt[0] = a * prey - b * prey * pred;
        dydt[1] = -c * pred + d * prey * pred;
        return dydt;
    }

    // Relativistic precession of orbit (system form of u'' + u = 1 + ε u^2)
    // Convert to first-order: y0 = u, y1 = u'; so y0' = y1, y1' = 1 - y0 + ε*y0^2:contentReference[oaicite:5]{index=5}
    public static Func<double, vector, vector> Precession(double eps) {
        return (phi, y) => {
            double u = y[0], up = y[1];
            vector dydφ = new vector(2);
            dydφ[0] = up;
            dydφ[1] = 1 - u + eps * u * u;
            return dydφ;
        };
    }

    // Newtonian three-body problem (planar, m1=m2=m3=1, G=1). 
    // State z = {v1x,v1y, v2x,v2y, v3x,v3y, x1,y1, x2,y2, x3,y3}
    public static vector ThreeBody(double t, vector z) {
        // Unpack positions (r_i) and velocities (v_i)
        double x1 = z[6],  y1 = z[7];
        double x2 = z[8],  y2 = z[9];
        double x3 = z[10], y3 = z[11];
        double vx1 = z[0], vy1 = z[1];
        double vx2 = z[2], vy2 = z[3];
        double vx3 = z[4], vy3 = z[5];
        // Compute pairwise accelerations
        double ax1=0, ay1=0, ax2=0, ay2=0, ax3=0, ay3=0;
        // Force on 1 from 2 and 3
        double dx12 = x2 - x1, dy12 = y2 - y1;
        double r12_sq = dx12*dx12 + dy12*dy12;
        double invr12_3 = 1.0/(r12_sq * Math.Sqrt(r12_sq));
        ax1 += dx12 * invr12_3;
        ay1 += dy12 * invr12_3;
        double dx13 = x3 - x1, dy13 = y3 - y1;
        double r13_sq = dx13*dx13 + dy13*dy13;
        double invr13_3 = 1.0/(r13_sq * Math.Sqrt(r13_sq));
        ax1 += dx13 * invr13_3;
        ay1 += dy13 * invr13_3;
        // Force on 2 from 1 and 3
        double dx21 = x1 - x2, dy21 = y1 - y2;
        double r21_sq = dx21*dx21 + dy21*dy21;
        double invr21_3 = 1.0/(r21_sq * Math.Sqrt(r21_sq));
        ax2 += dx21 * invr21_3;
        ay2 += dy21 * invr21_3;
        double dx23 = x3 - x2, dy23 = y3 - y2;
        double r23_sq = dx23*dx23 + dy23*dy23;
        double invr23_3 = 1.0/(r23_sq * Math.Sqrt(r23_sq));
        ax2 += dx23 * invr23_3;
        ay2 += dy23 * invr23_3;
        // Force on 3 from 1 and 2
        double dx31 = x1 - x3, dy31 = y1 - y3;
        double r31_sq = dx31*dx31 + dy31*dy31;
        double invr31_3 = 1.0/(r31_sq * Math.Sqrt(r31_sq));
        ax3 += dx31 * invr31_3;
        ay3 += dy31 * invr31_3;
        double dx32 = x2 - x3, dy32 = y2 - y3;
        double r32_sq = dx32*dx32 + dy32*dy32;
        double invr32_3 = 1.0/(r32_sq * Math.Sqrt(r32_sq));
        ax3 += dx32 * invr32_3;
        ay3 += dy32 * invr32_3;
        // Assemble z' (derivatives): velocity derivatives = accelerations, position derivatives = velocities
        vector dzdt = new vector(12);
        dzdt[0] = ax1;  dzdt[1] = ay1;
        dzdt[2] = ax2;  dzdt[3] = ay2;
        dzdt[4] = ax3;  dzdt[5] = ay3;
        dzdt[6] = vx1;  dzdt[7] = vy1;
        dzdt[8] = vx2;  dzdt[9] = vy2;
        dzdt[10] = vx3; dzdt[11] = vy3;
        return dzdt;
    }
}
