using System;
using System.IO;
using System.Collections.Generic;
using static System.Math;

class MainProgram {
    static void Main() {
        Console.Error.WriteLine("Starting Task A (Extrema)...");
        TaskA();
        Console.Error.WriteLine("Finished Task A.\n");

        Console.Error.WriteLine("Starting Task B (Hydrogen Atom)... This may take a minute.");
        TaskB();
        Console.Error.WriteLine("Finished Task B.\n");

        Console.Error.WriteLine("Starting Task C (Optimized Newton)...");
        TaskC();
        Console.Error.WriteLine("Finished Task C.");
        Console.Error.WriteLine("Program finished successfully.");
    }

    static void TaskA() {
        // ... (resten af TaskA er uændret)
        Console.WriteLine("------------ TASK A: Newton's method with numerical Jacobian and back-tracking line-search ------------");
        Console.WriteLine("------ Check the root-finding routine using some simple one- and two-dimensional equations ------");

        Console.WriteLine("\n--- Find the extremum(s) of the Rosenbrock's valley function ---");
        Console.WriteLine("The Rosenbrock's valley function is: f(x,y) = (1-x)^2 + 100(y-x^2)^2");
        Func<vector, vector> grad_rosenbrock = (v) => {
            double x = v[0], y = v[1];
            double dfdx = -2 * (1 - x) - 400 * x * (y - x * x);
            double dfdy = 200 * (y - x * x);
            return new vector(dfdx, dfdy);
        };
        vector start_rosenbrock = new vector(0.0, 0.0);
        vector root_rosenbrock = RootFinding.Newton(grad_rosenbrock, start_rosenbrock);
        Console.Write("The extremum(s) is found to be: ");
        root_rosenbrock.print("(x,y) =");

        Console.WriteLine("\n--- Find the minimum(s) of the Himmelblau's function ---");
        Console.WriteLine("The Himmelblau's function is: f(x,y) = (x^2 + y-11)^2 + (x + y^2 -7)^2");
        Func<vector, vector> grad_himmelblau = (v) => {
            double x = v[0], y = v[1];
            double dfdx = 2 * (x * x + y - 11) * (2 * x) + 2 * (x + y * y - 7);
            double dfdy = 2 * (x * x + y - 11) + 2 * (x + y * y - 7) * (2 * y);
            return new vector(dfdx, dfdy);
        };
        vector[] starts_himmelblau = {
            new vector(4, 3), new vector(-3, 3), new vector(-4, -3), new vector(4, -2)
        };
        Console.WriteLine("The minimum(s) are found to be:");
        foreach (var start in starts_himmelblau) {
            vector root = RootFinding.Newton(grad_himmelblau, start);
            root.print($"Start: ({start[0]},{start[1]}) -> Minimum: ", "{0,10:f4} ");
        }
    }

    static void TaskB() {
        Console.WriteLine("\n------------ Bound states of hydrogen atom with shooting method for boundary value problems ------------");
        
        double r_max_main = 8;
        double r_min = 1e-3;
        double acc = 1e-4, eps = 1e-4;

        Func<vector, vector> M = (v) => {
            double E = v[0];
            Console.Error.WriteLine($"  ... evaluating M(E) for E = {E:f6}");
            Func<double, vector, vector> schrodinger = (r, y) => new vector(y[1], -2 * (E + 1 / r) * y[0]);
            vector y_min = new vector(r_min - r_min * r_min, 1 - 2 * r_min);
            var (_, ylist) = ODE.driver(schrodinger, (r_min, r_max_main), y_min, acc:acc, eps:eps);
            return new vector(ylist[ylist.Count - 1][0]);
        };

        Console.WriteLine($"\n------ Find the lowest root, E_0, of the equation M(E)=0 ------");
        Console.Error.WriteLine("  Finding lowest root E0...");
        Console.WriteLine($"This is done for r_max={r_max_main}, r_min={r_min}, acc={acc}, eps={eps}");
        
        vector E_start = new vector(-1.0);
        vector E0_vec = RootFinding.Newton(M, E_start, acc:1e-4);
        double E0 = E0_vec[0];

        Console.WriteLine("The lowest root is found to be:");
        Console.WriteLine($"E_0 = {E0:f8} Hartree (Exact: -0.5 Hartree)");

        Console.WriteLine("\n------ Plot the resulting wave-function and compare with the exact result ------");
        Console.Error.WriteLine("  Generating plot data...");
        Func<double, vector, vector> schrodinger_final = (r, y) => new vector(y[1], -2 * (E0 + 1 / r) * y[0]);
        vector y_min_final = new vector(r_min - r_min * r_min, 1 - 2 * r_min);
        var (r_vals, f_vals) = ODE.driver(schrodinger_final, (r_min, r_max_main), y_min_final, acc:acc, eps:eps);

        using (var writer = new StreamWriter("wave_function.data")) {
            for (int i = 0; i < r_vals.Count; i++) writer.WriteLine($"{r_vals[i]} {f_vals[i][0]}");
        }
        using (var writer = new StreamWriter("exact_wave_function.data")) {
            for (double r = 0; r <= r_max_main; r += 0.05) writer.WriteLine($"{r} {r * Exp(-r)}");
        }
        Console.WriteLine("wave_function.svg contains a plot of the resulting wave-function along with the exact result for comparison.");

        Console.WriteLine("\n------ Investigate convergence with respect to the r_max ------");
        Console.Error.WriteLine("  Investigating convergence vs r_max...");
        Console.WriteLine("r_max      E_0");
        for (double r_max_conv = 3; r_max_conv <= 10; r_max_conv += 1) {
             Console.Error.Write($"    r_max={r_max_conv}... ");
             Func<vector, vector> M_conv = (v) => {
                double E = v[0];
                Func<double, vector, vector> sch_conv = (r, y) => new vector(y[1], -2 * (E + 1 / r) * y[0]);
                vector y_min_conv = new vector(r_min - r_min * r_min, 1 - 2 * r_min);
                var (_, ylist) = ODE.driver(sch_conv, (r_min, r_max_conv), y_min_conv, acc:acc, eps:eps);
                return new vector(ylist[ylist.Count - 1][0]);
            };
            double E0_conv = RootFinding.Newton(M_conv, new vector(-0.5), acc:1e-4)[0];
            Console.WriteLine($"{r_max_conv,-10:f1} {E0_conv:f8}");
            Console.Error.WriteLine("done.");
        }

        // ... (resten af TaskB og TaskC er uændret, men du kan tilføje flere Error.WriteLine hvis du vil)
        Console.WriteLine("\n------ Investigate convergence with respect to the r_min ------");
        Console.Error.WriteLine("  Investigating convergence vs r_min...");
        Console.WriteLine("r_min      E_0");
        for (double r_min_conv = 0.1; r_min_conv >= 1e-4; r_min_conv /= 2) {
            Console.Error.Write($"    r_min={r_min_conv:g4}... ");
             Func<vector, vector> M_conv = (v) => {
                double E = v[0];
                Func<double, vector, vector> sch_conv = (r, y) => new vector(y[1], -2 * (E + 1 / r) * y[0]);
                vector y_min_conv = new vector(r_min_conv - r_min_conv * r_min_conv, 1 - 2 * r_min_conv);
                var (_, ylist) = ODE.driver(sch_conv, (r_min_conv, r_max_main), y_min_conv, acc:acc, eps:eps);
                return new vector(ylist[ylist.Count - 1][0]);
            };
            double E0_conv = RootFinding.Newton(M_conv, new vector(-0.5), acc:1e-4)[0];
            Console.WriteLine($"{r_min_conv,-10:g4} {E0_conv:f8}");
            Console.Error.WriteLine("done.");
        }
        
        Console.WriteLine("\n------ Investigate convergence with respect to the acc and eps of the ODE solver ------");
        Console.Error.WriteLine("  Investigating convergence vs acc/eps...");
        Console.WriteLine("acc/eps    E_0");
        for (double acc_eps_conv = 1e-2; acc_eps_conv >= 1e-6; acc_eps_conv /= 10) {
            Console.Error.Write($"    acc/eps={acc_eps_conv:g3}... ");
            Func<vector, vector> M_conv = (v) => {
                double E = v[0];
                Func<double, vector, vector> sch_conv = (r, y) => new vector(y[1], -2 * (E + 1 / r) * y[0]);
                vector y_min_conv = new vector(r_min - r_min * r_min, 1 - 2 * r_min);
                var (_, ylist) = ODE.driver(sch_conv, (r_min, r_max_main), y_min_conv, acc:acc_eps_conv, eps:acc_eps_conv);
                return new vector(ylist[ylist.Count - 1][0]);
            };
            double E0_conv = RootFinding.Newton(M_conv, new vector(-0.5), acc:1e-5)[0];
            Console.WriteLine($"{acc_eps_conv,-10:g3} {E0_conv:f8}");
            Console.Error.WriteLine("done.");
        }
    }

    static void TaskC() {
        Console.WriteLine("\n------------ TASK C: Quadratic interpolation line-search ------------");
        Console.WriteLine("\n------ Optimize the implementation by only allocating one matrix in the beginning and then updating it at each step ------");
        Console.WriteLine("The `RootFinding.Newton_Optimized` method and its helper `Jacobian` overload have been implemented.");
        Console.WriteLine("The Jacobian matrix is now allocated once and updated in-place at each iteration, reducing memory allocation overhead.");

        Console.WriteLine("\n------ Implement the quadratic interpolation line-search as described in the book ------");
        Console.WriteLine("The `RootFinding.Newton_Optimized` method now uses a quadratic interpolation line search instead of simple backtracking.");
        Console.WriteLine("This can lead to faster convergence by choosing a more optimal step size `lambda`.");

        Console.WriteLine("\n--- Testing the optimized solver on Rosenbrock's function ---");
        Func<vector, vector> grad_rosenbrock = (v) => {
            double x = v[0], y = v[1];
            double dfdx = -2 * (1 - x) - 400 * x * (y - x * x);
            double dfdy = 200 * (y - x * x);
            return new vector(dfdx, dfdy);
        };
        vector start_rosenbrock = new vector(0.0, 0.0);
        vector root_rosenbrock = RootFinding.Newton_Optimized(grad_rosenbrock, start_rosenbrock);
        Console.Write("The extremum found with the optimized solver is: ");
        root_rosenbrock.print("(x,y) =");
    }
}
