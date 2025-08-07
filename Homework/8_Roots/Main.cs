using System;
using System.IO;
using System.Collections.Generic;
using static System.Math;

public class MainProgram
{
    public delegate Vector RootSolver(Func<Vector, Vector> f, Vector start, double acc = 1e-3);

    public static void Main()
    {
        // TASK A, del 1 (Køres kun 1 gang)
        Console.WriteLine("------------ TASK A: Newton's method with numerical Jacobian and back-tracking line-search ------------\n");

        Console.WriteLine("------ Use your own routines for solving linear systems ------\n");
        Console.WriteLine("--- Solving a linear system Ax=b ---\n");
        
        Matrix A = new Matrix(3, 3);
        A[0, 0] = 4; A[0, 1] = 1; A[0, 2] = 2;
        A[1, 0] = 1; A[1, 1] = 5; A[1, 2] = 3;
        A[2, 0] = 2; A[2, 1] = 3; A[2, 2] = 6;
        Vector b = new Vector(12, 20, 26);
        
        QrDecomposition qrA = new QrDecomposition(A);
        Vector x = qrA.Solve(b);
        
        Console.WriteLine("The system is defined by matrix A and vector b:");
        A.Print("A =\n");
        b.Print("\nb = ");

        Console.WriteLine("\nThe expected solution is (1, 2, 3).");
        
        Console.WriteLine("\nThe solver finds the solution:");
        x.Print("x = ");

        Console.WriteLine("\n------ Test your root-finding routine using one- and two-dimensional equations ------");
        Console.WriteLine("\n--- Finding root for f(x) = (x-5)^2 - 4 ---");
        
        Console.WriteLine("\nThe expected solutions are x=3 and x=7.");

        Console.WriteLine("\nSearching from starting point x=1.0:");
        Func<Vector, Vector> f1d = (v) => new Vector(Pow(v[0] - 5, 2) - 4);
        Vector start1d_1 = new Vector(1.0);
        Vector root1d_1 = RootFinder.Newton(f1d, start1d_1);
        root1d_1.Print("  Root found at x = ");
        Console.WriteLine($"  f({root1d_1[0]:F4}) = {f1d(root1d_1)[0]:F8}");

        Console.WriteLine("\nSearching from starting point x=8.0:");
        Vector start1d_2 = new Vector(8.0);
        Vector root1d_2 = RootFinder.Newton(f1d, start1d_2);
        root1d_2.Print("  Root found at x = ");
        Console.WriteLine($"  f({root1d_2[0]:F4}) = {f1d(root1d_2)[0]:F8}\n");

        Console.WriteLine("\n--- Finding root for f(x,y)=(x+y-5, x^2-y-1) ---\n");
        
        Console.WriteLine("The expected solutions are (x,y)=(2,3) and (x,y)=(-3,8).");
        
        // Finder den 1. rod:
        Console.WriteLine("\nSearching from starting point (1,1):");
        Func<Vector, Vector> f2d = (v) => new Vector(v[0] + v[1] - 5, v[0] * v[0] - v[1] - 1);
        Vector start2d_1 = new Vector(1.0, 1.0);
        Vector root2d_1 = RootFinder.Newton(f2d, start2d_1);
        root2d_1.Print("  Root found at (x,y) = ");
        Console.WriteLine($"  f({root2d_1[0]:F4}, {root2d_1[1]:F4}) = ({f2d(root2d_1)[0]:F8}, {f2d(root2d_1)[1]:F8})");

        // Finder den 2. rod:
        Console.WriteLine("\nSearching from starting point (-2,7):");
        Vector start2d_2 = new Vector(-2.0, 7.0);
        Vector root2d_2 = RootFinder.Newton(f2d, start2d_2);
        root2d_2.Print("  Root found at (x,y) = ");
        Console.WriteLine($"  f({root2d_2[0]:F4}, {root2d_2[1]:F4}) = ({f2d(root2d_2)[0]:F8}, {f2d(root2d_2)[1]:F8})");

        // TASK A del 2 & TASK B del 1 (rund med den 'naïve implementation)
        Console.WriteLine("\n------------ (Here is a small comment related to Task C:                ------------");
        Console.WriteLine("------------  In the analysis below, I use the 'naïve implementation'.) ------------");
        
        RunSolverAnalyses(RootFinder.Newton, true);

        // TASK B del 2: (Køres kun 1 gang)
        Console.WriteLine("\n------ Investigate the convergence of the solution  ------");
        Console.WriteLine("------ (with respect to r_max, r_min, acc, and eps) ------\n");
        RunConvergenceAnalysis();

        // TASK C:
        Console.WriteLine("\n------------ TASK C: Quadratic interpolation line-search ------------\n");
        
        Console.WriteLine("------ Optimize the implementation by only allocating one matrix ------");
        Console.WriteLine("------ in the beginning and then updating it at each step.       ------");
        Console.WriteLine("------ And implement the quadratic interpolation line-search ------\n");

        Console.WriteLine("For this purpose, I have implemented the function NewtonOptimized (in RootFinder.cs),\nwhich is an optimized version of the Newton function.\n");

        Console.WriteLine("To check that the optimizations work, the analyses from Task A and B are run once again,");
        Console.WriteLine("but using the function 'NewtonOptimized' instead of 'Newton'.");
        Console.WriteLine("The results for this are shown below:");
        RunSolverAnalyses(RootFinder.NewtonOptimized, false); // 'false' forhindrer overskrivning af filer
    }

    // En samlet funktion der bruges i TASK A og B, samt TASK C:
    private static void RunSolverAnalyses(RootSolver solver, bool writeFiles)
    {
        Console.WriteLine("\n------ Find the extremum(s) of the Rosenbrock's valley function ------\n");
        Console.WriteLine("Expected minimum is (x,y)=(1, 1).\n");
        Vector startR = new Vector(2.0, 2.0);
        Vector rootR = solver(Functions.RosenbrockGradient, startR);
        Console.WriteLine("Rosenbrock minimum found at:");
        rootR.Print("  x = ");
        if (writeFiles)
        {
            File.WriteAllText("Rosenbrock_minimum.txt", $"Start: (2, 2)\nMinimum: ({rootR[0]}, {rootR[1]})");
        }

        Console.WriteLine("\n------ Find the minimum(s) of the Himmelblau's function ------\n");
        Console.WriteLine("Expected minima are (3, 2), (-2.805, 3.131), (-3.779, -3.283), and (3.584, -1.848).\n");
        var himmelblauStarts = new List<Vector>
        {
            new Vector(4.0, 4.0), new Vector(-2.0, 2.0), new Vector(-3.0, -4.0), new Vector(3.0, -3.0)
        };
        foreach (var startH in himmelblauStarts)
        {
            Vector rootH = solver(Functions.HimmelblauGradient, startH);
            rootH.Print($"Minimum found from start point ({startH[0]},{startH[1]}): ");
        }
        if (writeFiles)
        {
            var firstRoot = solver(Functions.HimmelblauGradient, himmelblauStarts[0]);
            File.WriteAllText("Himmelblau_minimum.txt", $"Start: (4, 4)\nMinimum: ({firstRoot[0]}, {firstRoot[1]})");
        }

        if (writeFiles) // gør så at det ikke skrive i TASK C
        {
            Console.WriteLine("\n------------ TASK B: Bound states of hydrogen atom with shooting method for boundary value problems ------------");
        }

        Console.WriteLine("\n------ Find the lowest root, E_0, of the equation M(E)=0 ------\n");
        double r_max_b = 8.0;
        var M_b = Functions.Create_M_function(r_max: r_max_b);
        Vector E_start = new Vector(-0.6);
        Vector E_root = solver(M_b, E_start, acc: 1e-4);
        double E0 = E_root[0];
        Console.WriteLine($"Calculated ground state energy: E0 = {E0:F8} Hartree");

        if (writeFiles)
        {
            Console.WriteLine("\n------ Plot the resulting wave-function and compare with the exact result ------");
            Console.WriteLine("------ (which is: E_0 = -1/2, f_0(r) = r e^(-r)                           ------\n");
            
            File.WriteAllText("Hydrogen_energy.txt", $"E0_calculated = {E0}\nE0_exact = -0.5");
            
            // Data til plot generes kun 1 gang
            double r_min_b = 1e-3;
            Vector y_start = new Vector(r_min_b - r_min_b * r_min_b, 1 - 2 * r_min_b);
            var (rs, ys) = OdeSolver.Solve(Functions.SchrodingerEq(E0), r_min_b, y_start, r_max_b);
            using (var writer = new StreamWriter("Hydrogen_wavefunction.txt"))
            {
                for (int i = 0; i < rs.Count; i++) writer.WriteLine($"{rs[i]} {ys[i][0]}");
            }
            Console.WriteLine("Wavefunction data saved to Hydrogen_wavefunction.txt.");
            Console.WriteLine("Wavefunction plot saved as Hydrogen_wavefunction.svg.");
        }
    }

    // Funktion til 'Investigate the convergence' (køres kun 1 gang):
    private static void RunConvergenceAnalysis()
    {
        
        using (var writer = new StreamWriter("Hydrogen_convergence_rmax.txt"))
        {
            writer.WriteLine("# r_max, E0");
            for (double r_test = 4; r_test <= 12; r_test += 0.5)
            {
                var M_test = Functions.Create_M_function(r_max: r_test);
                double E_conv = RootFinder.Newton(M_test, new Vector(-0.5), 1e-5)[0];
                writer.WriteLine($"{r_test} {E_conv}");
            }
        }
        Console.WriteLine("Convergence data for r_max saved to Hydrogen_convergence_rmax.txt");

        using (var writer = new StreamWriter("Hydrogen_convergence_rmin.txt"))
        {
            writer.WriteLine("# r_min, E0");
            for (double r_test = 1e-1; r_test >= 1e-4; r_test /= 2)
            {
                var M_test = Functions.Create_M_function(r_max: 8.0, r_min: r_test);
                double E_conv = RootFinder.Newton(M_test, new Vector(-0.5), 1e-5)[0];
                writer.WriteLine($"{r_test} {E_conv}");
            }
        }
        Console.WriteLine("Convergence data for r_min saved to Hydrogen_convergence_rmin.txt");

        using (var writer = new StreamWriter("Hydrogen_convergence_acc.txt"))
        {
            writer.WriteLine("# acc, E0");
            for (double acc_test = 1e-1; acc_test >= 1e-5; acc_test /= 2)
            {
                var M_test = Functions.Create_M_function(r_max: 8.0, acc: acc_test, eps: 1e-3);
                double E_conv = RootFinder.Newton(M_test, new Vector(-0.5), 1e-5)[0];
                writer.WriteLine($"{acc_test} {E_conv}");
            }
        }
        Console.WriteLine("Convergence data for ODE accuracy (acc) saved to Hydrogen_convergence_acc.txt");

        using (var writer = new StreamWriter("Hydrogen_convergence_eps.txt"))
        {
            writer.WriteLine("# eps, E0");
            for (double eps_test = 1e-1; eps_test >= 1e-5; eps_test /= 2)
            {
                var M_test = Functions.Create_M_function(r_max: 8.0, acc: 1e-3, eps: eps_test);
                double E_conv = RootFinder.Newton(M_test, new Vector(-0.5), 1e-5)[0];
                writer.WriteLine($"{eps_test} {E_conv}");
            }
        }
        Console.WriteLine("Convergence data for ODE accuracy (eps) saved to Hydrogen_convergence_eps.txt");
    }
}