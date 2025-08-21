using System;
using System.IO;
using static System.Math;

public class Program {
    public static void Main() {
        // Generer Out.txt struktur
        using (var outtxt = new StreamWriter("Out.txt")) {
            outtxt.WriteLine("------------ TASK A ------------");
            outtxt.WriteLine("\n------ Testing the routines by solving systems of ordinary differential equations (ODE) ------");
            outtxt.WriteLine("\n--- Solving: u''=-u ---");
            outtxt.WriteLine("\nIn sho_data.txt and sho_plot.svg is the data and plot, respectively");
            outtxt.WriteLine("\n--- Solving: dy/dx = y ---");
            outtxt.WriteLine("\nIn exp_data.txt and exp_plot.svg is the data and plot, respectively");
            outtxt.WriteLine("\n--- Solving: y'' = 6x ---");
            outtxt.WriteLine("\nIn poly_data.txt and poly_plot.svg is the data and plot, respectively");
            // 
            // outtxt.WriteLine("For comparison, the plots also contain the analytical solutions.\n");

            outtxt.WriteLine("\n------ Reproduce the example from the scipy.integrate.odeint manual (oscillator with friction) ------");
            outtxt.WriteLine("\nIn oscillator_with_friction.txt and oscillator_with_friction_plot.svg is the data and plot, respectively.");

            outtxt.WriteLine("\n------ Reproduce the example from the the scipy.integrate.solve_ivp manual (Lotka-Volterra system) ------");
            outtxt.WriteLine("\nIn lotka_volterra.txt and lotka_volterra_plot.svg is the data and plot, respectively.");

            outtxt.WriteLine("\n------------ TASK B ------------");
            outtxt.WriteLine("\n------ Integrate this equation with ε=0 and initial conditions u(0)=1, u'(0)=0 : (Newtonian circular motion). ------");
            outtxt.WriteLine("\nThe resulting data is in Newtonian_circular_motion.txt");

            outtxt.WriteLine("\n------ Integrate this equation with ε=0 and initial conditions u(0)=1, u'(0)≈-0.5 : ( Newtonian elliptical motion). ------");
            outtxt.WriteLine("\nThe resulting data is in Newtonian_elliptical_motion.txt");

            outtxt.WriteLine("\n------ Integrate this equation with ε≈0.01 and initial conditions u(0)=1, u'(0)≈-0.5 : (Relativistic precession of a planetary orbit). ------");
            outtxt.WriteLine("\n The resulting data is in Relativistic_motion.txt");

            outtxt.WriteLine("\n------ Plot planetary orbit for Newtonian circular motion, Newtonian elliptical motion, and Relativistic. ------");
            outtxt.WriteLine("\n This plot is in Planetary_orbits_plot.svg");

            outtxt.WriteLine("\n ------------ TASK C ------------");
            outtxt.WriteLine("\n------ Using my numerical ODE integrator reproduce the solution the remarkable stable planar periodic solution to the three-body problem ------");
            outtxt.WriteLine("\nThe reproduced solution data is in three_body_problem.txt and plotted in three_body_problem_plot.svg");
        }

        // Part A: Testing adaptive Runge-Kutta solver on simple ODEs

        // A1. Harmonic oscillator u'' = -u
        vector y0 = new vector(new double[]{0.0, 1.0});
        double a = 0, b = 2 * PI;
        var (xs1, ys1) = OdeSolver.driver(ODEModels.SHO, a, y0, b, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("sho_data.txt")) {
            for (int i = 0; i < xs1.Count; i++) writer.WriteLine($"{xs1[i]} {ys1[i][0]}");
        }

        // A2. dy/dx = y => y = exp(x)
        vector y_exp = new vector(new double[]{1.0});
        var (xs_exp, ys_exp) = OdeSolver.driver(ODEModels.ExpGrowth, 0, y_exp, 2.0, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("exp_data.txt")) {
            for (int i = 0; i < xs_exp.Count; i++) writer.WriteLine($"{xs_exp[i]} {ys_exp[i][0]}");
        }

        // A3. y'' = 6x => y = x^3
        vector y_poly = new vector(new double[]{0.0, 0.0});
        var (xs_poly, ys_poly) = OdeSolver.driver(ODEModels.PolyTest, 0, y_poly, 2.0, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("poly_data.txt")) {
            for (int i = 0; i < xs_poly.Count; i++) writer.WriteLine($"{xs_poly[i]} {ys_poly[i][0]}");
        }

        // A4. Lotka-Volterra system
        Console.WriteLine("In lotka_volterra.txt and lotka_volterra_plot.svg is the data and plot, respectively, from solving: Lotka-Volterra system");
        vector lv0 = new vector(new double[]{10.0, 5.0});
        double T = 15.0;
        var (xs_lv, ys_lv) = OdeSolver.driver(ODEModels.LotkaVolterra, 0, lv0, T, acc:1e-5, eps:1e-5);
        using (var writer = new StreamWriter("lotka_volterra.txt")) {
            for (int i = 0; i < xs_lv.Count; i++) writer.WriteLine($"{xs_lv[i]} {ys_lv[i][0]} {ys_lv[i][1]}");
        }

        // A5. Oscillator with friction: y'' + b y' + k y = 0, rewritten as first-order system
        Console.WriteLine("In oscillator_with_friction.txt and oscillator_with_friction_plot.svg is the data and plot, respectively, from solving: damped oscillator");
        double b_fric = 0.25, k_fric = 5.0;
        Func<double, vector, vector> fricOsc = (t, y) => {
            vector dydt = new vector(2);
            dydt[0] = y[1];
            dydt[1] = -b_fric * y[1] - k_fric * y[0];
            return dydt;
        };
        vector y_fric0 = new vector(new double[]{1.0, 0.0});
        double t_fric_end = 10.0;
        var (xs_fric, ys_fric) = OdeSolver.driver(fricOsc, 0.0, y_fric0, t_fric_end, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("oscillator_with_friction.txt")) {
            for (int i = 0; i < xs_fric.Count; i++) writer.WriteLine($"{xs_fric[i]} {ys_fric[i][0]} {ys_fric[i][1]}");
        }

        // Part B: Save planetary orbit data (Newtonian circular motion, sampled via spline)
        double phi_max = 24 * PI;
        vector u0 = new vector(new double[]{1.0, 0.0});

        // Høj præcision + lille starttrin for stabilitet
        var (xs_nc, ys_nc) = OdeSolver.driver(
            ODEModels.Precession(0.0), 0, u0, phi_max,
            acc: 1e-12, eps: 1e-12, h: 1e-3
        );
        // Brug spline for jævn sampling
        var spline = OdeSolver.make_qspline(xs_nc, ys_nc);
        using (var writer = new StreamWriter("Newtonian_circular_motion.txt")) {
            for (double φ = 0; φ <= phi_max; φ += 0.01)
                writer.WriteLine($"{φ} {spline(φ)[0]}");
        }




        vector u1 = new vector(new double[]{1.0, -0.5});
        var (xs_ne, ys_ne) = OdeSolver.driver(ODEModels.Precession(0.0), 0, u1, phi_max, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("Newtonian_elliptical_motion.txt")) {
            for (int i = 0; i < xs_ne.Count; i++) writer.WriteLine($"{xs_ne[i]} {ys_ne[i][0]}");
        }

        var (xs_rel, ys_rel) = OdeSolver.driver(ODEModels.Precession(0.01), 0, u1.Copy(), phi_max, acc:1e-6, eps:1e-6);
        using (var writer = new StreamWriter("Relativistic_motion.txt")) {
            for (int i = 0; i < xs_rel.Count; i++) writer.WriteLine($"{xs_rel[i]} {ys_rel[i][0]}");
        }

        // Part C: Three-body solution
        vector z0 = new vector(new double[12]);
        z0[6] =  0.9700436; z0[7]  = -0.24308753;
        z0[8] = -0.9700436; z0[9]  =  0.24308753;
        z0[10] = 0.0;        z0[11] =  0.0;
        z0[0] =  0.466203685; z0[1] =  0.43236573;
        z0[2] =  0.466203685; z0[3] =  0.43236573;
        z0[4] = -0.93240737;  z0[5] = -0.86473146;
        double t_max = 6.3259;
        var (xs_tb, ys_tb) = OdeSolver.driver(ODEModels.ThreeBody, 0.0, z0, t_max, acc:1e-5, eps:1e-5);
        using (var writer = new StreamWriter("three_body_problem.txt")) {
            for (int i = 0; i < xs_tb.Count; i++) {
                var y = ys_tb[i];
                writer.WriteLine($"{xs_tb[i]} {y[6]} {y[7]} {y[8]} {y[9]} {y[10]} {y[11]}");
            }
        }
    }
}
