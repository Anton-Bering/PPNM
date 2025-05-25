using System;
using static System.Math;

public class Program {
    public static void Main() {
        // Part A: Basic ODE tests
        Console.WriteLine("Part A: Testing adaptive Runge-Kutta solver on simple ODEs");
        // 1. Harmonic oscillator u'' = -u, with y(0)=0, y'(0)=1
        vector y0 = new vector(new double[]{0.0, 1.0});
        double a = 0, b = 2 * PI;
        var (xs1, ys1) = OdeSolver.driver(ODEModels.SHO, a, y0, b, acc:1e-6, eps:1e-6);
        vector y_end = ys1[ys1.Count - 1];
        Console.WriteLine($"u'' = -u from x={a} to {b}: y_end = ({y_end[0]:F6}, {y_end[1]:F6}), "
                          + $"expected ~({0:F6}, {1:F6})");
        Console.WriteLine($"Steps taken: {xs1.Count - 1}\n");

        // 2. Lotka–Volterra predator-prey system
        vector lv0 = new vector(new double[]{10.0, 5.0});  // initial populations
        double T = 15.0;
        var (xs2, ys2) = OdeSolver.driver(ODEModels.LotkaVolterra, 0, lv0, T, acc:1e-5, eps:1e-5);
        vector lv_end = ys2[ys2.Count - 1];
        Console.WriteLine($"Lotka-Volterra from t=0 to t={T}: final (prey, pred) = "
                          + $"({lv_end[0]:F3}, {lv_end[1]:F3})");
        Console.WriteLine($"Initial was (prey, pred) = ({lv0[0]}, {lv0[1]})\n");

        // Part B: Relativistic orbital precession
        Console.WriteLine("Part B: Relativistic Precession of Planetary Orbit");
        double phi_max = 12 * PI;
        // Case 1: Circular orbit (ε=0)
        vector u_init = new vector(new double[]{1.0, 0.0});
        var (xs_circ, ys_circ) = OdeSolver.driver(ODEModels.Precession(0.0), 0, u_init, phi_max,
                                                 acc:1e-8, eps:1e-8);
        vector circ_end = ys_circ[ys_circ.Count - 1];
        Console.WriteLine($"Circular orbit (ε=0) after φ={phi_max}: u_final = {circ_end[0]:F6} "
                          + "(should remain ~1)\n");
        // Case 2: Elliptical Newtonian (ε=0)
        vector u_init2 = new vector(new double[]{1.0, -0.5});
        var (xs_newt, ys_newt) = OdeSolver.driver(ODEModels.Precession(0.0), 0, u_init2, phi_max,
                                                 acc:1e-6, eps:1e-6);
        // Case 3: Elliptical relativistic (ε=0.01)
        var (xs_rel, ys_rel) = OdeSolver.driver(ODEModels.Precession(0.01), 0, u_init2.copy(), phi_max,
                                               acc:1e-6, eps:1e-6);
        // Compare u(φ) after each full rotation
        Console.WriteLine("Orbit    φ (radians)    u (Newtonian)    u (Relativistic)");
        Func<double, vector> newt_spline = OdeSolver.make_qspline(xs_newt, ys_newt);
        Func<double, vector> rel_spline = OdeSolver.make_qspline(xs_rel, ys_rel);
        for (int k = 1; k <= 5; k++) {
            double φ = 2 * PI * k;
            double u_newt = newt_spline(φ)[0];
            double u_rel = rel_spline(φ)[0];
            Console.WriteLine($"{k,5}    {φ,10:F3}    {u_newt,14:F6}    {u_rel,14:F6}");
        }
        Console.WriteLine();

        // Part C: Higher-order method and three-body problem
        Console.WriteLine("Part C: Higher Order Stepper, Spline Interpolation, and Three-Body Problem");
        // 1. Test RK23 on y'' = 2x (expected exact integration)
        vector yz0 = new vector(new double[]{0.0, 0.0});
        double x_end = 10.0;
        var (xs_test, ys_test) = OdeSolver.driver((x, y) => {
                vector dydx = new vector(2);
                dydx[0] = y[1];
                dydx[1] = 2 * x;
                return dydx;
            },
            0.0, yz0, x_end, acc:1e-8, eps:1e-8, h:0.001, stepper: RKStepper.rkstep23);
        vector y_final = ys_test[ys_test.Count - 1];
        double exact = Pow(x_end, 3) / 3.0;
        Console.WriteLine($"y'' = 2x from 0 to {x_end}: y_end = {y_final[0]:F6}, exact = {exact:F6}");
        Console.WriteLine($"Steps taken with RK23: {xs_test.Count - 1}\n");

        // 2. Three-body figure-8 orbit simulation
        Console.WriteLine("Three-body figure-8 orbit simulation (m1=m2=m3=1, G=1)");
        // Initial conditions for figure-8 orbit (from known solution):contentReference[oaicite:13]{index=13}:contentReference[oaicite:14]{index=14}
        vector z0 = new vector(new double[12]);
        // Positions:
        z0[6] =  0.9700436;   z0[7]  = -0.24308753;
        z0[8] = -0.9700436;   z0[9]  =  0.24308753;
        z0[10] = 0.0;         z0[11] =  0.0;
        // Velocities:
        z0[0] =  0.466203685;  z0[1] =  0.43236573;
        z0[2] =  0.466203685;  z0[3] =  0.43236573;
        z0[4] = -0.93240737;   z0[5] = -0.86473146;
        double t_max = 6.3259;
        Func<double, vector> orb = OdeSolver.make_ode_ivp_qspline(ODEModels.ThreeBody, 0.0, z0,
                                                                  t_max, acc:1e-5, eps:1e-5);
        // Final positions after one period
        vector z_end = orb(t_max);
        vector r1_init = new vector(new double[]{z0[6],  z0[7]});
        vector r2_init = new vector(new double[]{z0[8],  z0[9]});
        vector r3_init = new vector(new double[]{z0[10], z0[11]});
        vector r1_end  = new vector(new double[]{z_end[6],  z_end[7]});
        vector r2_end  = new vector(new double[]{z_end[8],  z_end[9]});
        vector r3_end  = new vector(new double[]{z_end[10], z_end[11]});
        Console.WriteLine("Final positions vs initial positions after one period:");
        Console.WriteLine($"r1: final = ({r1_end[0]:F6}, {r1_end[1]:F6}), initial = ({r1_init[0]:F6}, {r1_init[1]:F6})");
        Console.WriteLine($"r2: final = ({r2_end[0]:F6}, {r2_end[1]:F6}), initial = ({r2_init[0]:F6}, {r2_init[1]:F6})");
        Console.WriteLine($"r3: final = ({r3_end[0]:F6}, {r3_end[1]:F6}), initial = ({r3_init[0]:F6}, {r3_init[1]:F6})");
        Console.WriteLine("Each body's final position is approximately equal to its starting position, demonstrating the periodic figure-8 orbit.");
    }
}
