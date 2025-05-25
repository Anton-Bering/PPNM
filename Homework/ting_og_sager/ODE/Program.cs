using System;
using System.IO;
class Program {
    static void Main(string[] args) {
        // Test A3: Simple harmonic oscillator (debugging example u'' = -u)
        Func<double, vector, vector> SHO = (t, Y) => {
            // Y[0] = u, Y[1] = u'
            double u = Y[0];
            double up = Y[1];
            // u'' = -u
            return new vector(new double[]{ up, -u });
        };
        vector y0_sho = new vector(new double[]{ 1.0, 0.0 });  // initial conditions: u(0)=1, u'(0)=0
        var (xs_sho, ys_sho) = ODE.driver(SHO, (0, 2 * Math.PI), y0_sho, h: 0.1, acc: 1e-6, eps: 1e-6);
        vector y_end_sho = ys_sho[ys_sho.Count - 1];
        Console.WriteLine("SHO test: y(2π) = ({0:F6}, {1:F6}) (should be ≈(1, 0))", 
                          y_end_sho[0], y_end_sho[1]);

        // Test A4: Reproduce SciPy odeint example (damped pendulum with friction)
        double b = 0.25, c = 5.0;  // damping and gravity constants (from SciPy example)
        Func<double, vector, vector> pendulum = (t, Y) => {
            double theta = Y[0];
            double omega = Y[1];
            // θ'' + b*θ' + c*sin(θ) = 0  ->  θ'' = -b*θ' - c*sin(θ)
            double theta_dd = -b * omega - c * Math.Sin(theta);
            return new vector(new double[]{ omega, theta_dd });
        };
        vector y0_pend = new vector(new double[]{ Math.PI - 0.1, 0.0 });  // initial θ(0)=π-0.1, ω(0)=0
        Func<double, vector> pendulum_solution = ODE.make_ode_ivp_qspline(pendulum, (0, 10), y0_pend, acc: 1e-5, eps: 1e-5, hstart: 0.01);
        // Write pendulum solution (θ and ω vs t) at 101 points to "pendulum.dat"
        using(StreamWriter sw = new StreamWriter("pendulum.dat")) {
            int N = 100;
            for(int i = 0; i <= N; i++) {
                double t = 10.0 * i / N;
                vector sol = pendulum_solution(t);
                sw.WriteLine("{0:F5} {1:F5} {2:F5}", t, sol[0], sol[1]);
            }
        }
        Console.WriteLine("Pendulum example data written to pendulum.dat");

        // Test B: Relativistic precession of planetary orbit equation
        // u''(φ) + u(φ) = 1 + ε * u(φ)^2, convert to first-order system: y0=u, y1=u' 
        // => y0' = y1, y1' = 1 - y0 + ε*y0^2
        double phi_max = 20 * Math.PI;  // integrate for several rotations (10 rotations)
        double[] epsilons = { 0.0, 0.0, 0.01 };
        string[] orbitFiles = { "orbit_circular.dat", "orbit_elliptical.dat", "orbit_precession.dat" };
        vector[] initialConditions = {
            new vector(new double[]{ 1.0,  0.0  }),   // (ε=0) circular orbit: u(0)=1, u'(0)=0
            new vector(new double[]{ 1.0, -0.5 }),   // (ε=0) elliptical orbit: u(0)=1, u'(0)≈-0.5
            new vector(new double[]{ 1.0, -0.5 })    // (ε≈0.01) relativistic case (same initial u, u')
        };
        for(int s = 0; s < epsilons.Length; s++) {
            double epsilon = epsilons[s];
            vector y0_orbit = initialConditions[s];
            var (phiList, uList) = ODE.driver((phi, Y) => {
                    double u = Y[0], up = Y[1];
                    double upp = 1 - u + epsilon * u * u;
                    return new vector(new double[]{ up, upp });
                },
                (0, phi_max), y0_orbit, h: 1e-3, acc: 1e-6, eps: 1e-6);
            using(StreamWriter sw = new StreamWriter(orbitFiles[s])) {
                for(int i = 0; i < phiList.Count; i++) {
                    sw.WriteLine("{0:F6} {1:F6}", phiList[i], uList[i][0]);
                }
            }
            Console.WriteLine("Orbit scenario {0} data -> {1}", s+1, orbitFiles[s]);
        }

        // Test C1: Verify stepper order by integrating y'' = 2x exactly
        // Convert y''=2x to first-order: Y0=y, Y1=y'; Y0'=Y1, Y1' = 2x
        Func<double, vector, vector> poly2 = (x, Y) => new vector(new double[]{ Y[1], 2 * x });
        vector y0_poly = new vector(new double[]{ 0.0, 0.0 });  // initial y(0)=0, y'(0)=0
        var (xlist_poly, ylist_poly) = ODE.driver(poly2, (0, 10), y0_poly, h: 1e-4, acc: 1e-8, eps: 1e-8, stepper: ODE.rkstep23);
        vector y_end_poly = ylist_poly[ylist_poly.Count - 1];
        double exact_y = Math.Pow(10, 3) / 3.0;
        Console.WriteLine("Polynomial test: y(10) = {0:F6}, exact = {1:F6}", y_end_poly[0], exact_y);
        Console.WriteLine("Steps taken: {0}", xlist_poly.Count - 1);

        // Test C4: Three-body problem (figure-8 orbit) 
        // Initial conditions for the figure-8 orbit (three equal masses) from Chenciner & Montgomery&#8203;:contentReference[oaicite:0]{index=0}
        vector z0 = new vector(new double[] {
             0.4662036850,  0.4323657300,   // v1_x, v1_y
            -0.9324073700, -0.8647314600,   // v2_x, v2_y
             0.4662036850,  0.4323657300,   // v3_x, v3_y
            -0.97000436,    0.24308753,     // r1_x, r1_y
             0.0,           0.0,           // r2_x, r2_y
             0.97000436,   -0.24308753      // r3_x, r3_y
        });
        // Newtonian three-body equations of motion (G=1, m1=m2=m3=1)
        Func<double, vector, vector> threebody = (t, Z) => {
            // positions and velocities from state Z
            vector r1 = new vector(new double[]{ Z[6],  Z[7]  });
            vector r2 = new vector(new double[]{ Z[8],  Z[9]  });
            vector r3 = new vector(new double[]{ Z[10], Z[11] });
            vector v1 = new vector(new double[]{ Z[0],  Z[1]  });
            vector v2 = new vector(new double[]{ Z[2],  Z[3]  });
            vector v3 = new vector(new double[]{ Z[4],  Z[5]  });
            // pairwise distance vectors
            vector r12 = r2 - r1;
            vector r13 = r3 - r1;
            vector r23 = r3 - r2;
            double d12 = r12.norm();
            double d13 = r13.norm();
            double d23 = r23.norm();
            // accelerations (Newton's law)
            vector a1 = r12 * (1.0/(d12*d12*d12)) + r13 * (1.0/(d13*d13*d13));
            vector a2 = (r1 - r2) * (1.0/(d12*d12*d12)) + r23 * (1.0/(d23*d23*d23));
            vector a3 = (r1 - r3) * (1.0/(d13*d13*d13)) + (r2 - r3) * (1.0/(d23*d23*d23));
            // assemble derivative vector Z' = (v1', v2', v3', r1', r2', r3')
            vector dZ = new vector(12);
            // velocity derivatives (accelerations)
            dZ[0] = a1[0];  dZ[1] = a1[1];
            dZ[2] = a2[0];  dZ[3] = a2[1];
            dZ[4] = a3[0];  dZ[5] = a3[1];
            // position derivatives (velocities)
            dZ[6]  = v1[0]; dZ[7]  = v1[1];
            dZ[8]  = v2[0]; dZ[9]  = v2[1];
            dZ[10] = v3[0]; dZ[11] = v3[1];
            return dZ;
        };
        double T = 6.3259 * 3;  // integrate over three periods (T ≈ 6.3259 for one loop)
        var (tlist, zlist) = ODE.driver(threebody, (0, T), z0, h: 1e-3, acc: 1e-5, eps: 1e-5, stepper: ODE.rkstep23);
        // Write trajectory of body1 (x,y over time) to "figure8.dat"
        using(StreamWriter sw = new StreamWriter("figure8.dat")) {
            for(int i = 0; i < tlist.Count; i++) {
                sw.WriteLine("{0:F6} {1:F6}", zlist[i][6], zlist[i][7]);
            }
        }
        Console.WriteLine("Figure-8 three-body data written to figure8.dat");
    }
}
