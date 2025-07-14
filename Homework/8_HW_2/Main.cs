// ---------- Main.cs ----------
using System;
using System.IO;

class Program
{
    static void Main()
    {
        using (var writer = new StreamWriter("Out.txt", append:false))
        {
            writer.WriteLine("------------ TASK A: Newton's method with numerical Jacobian and back-tracking line-search ------------\n");

            // --- Simple 1D & 2D checks ---------------------------------------------------
            writer.WriteLine("------ Check the root-finding routine using some simple one- and two-dimensional equations ------\n");

            var root1  = Newton.Solve(Functions.Simple1D, new Vec(1.0));
            var root2  = Newton.Solve(Functions.Simple2D, new Vec(1.5,1.0));

            writer.WriteLine($"Root of x^3 - 2 = 0              : x = {root1[0]:F8}\n");
            writer.WriteLine($"Root of system {{x^2+y^2=4, x=y}} : (x,y) = ({root2[0]:F8}, {root2[1]:F8})\n");

            // --- Rosenbrock extremum -----------------------------------------------------
            writer.WriteLine("\n--- Find the extremum(s) of Rosenbrock's valley function ---\n");
            var rb = Newton.Solve(Functions.RosenbrockGrad, new Vec(-1.2,1.0));
            writer.WriteLine($"The extremum is found at: (x,y) = ({rb[0]:F8}, {rb[1]:F8})");

            // --- Himmelblau minima -------------------------------------------------------
            writer.WriteLine("\n--- Find the minimum(s) of the Himmelblau's function ---\n");
            var hb = Newton.Solve(Functions.HimmelblauGrad, new Vec(3.0,2.0));
            writer.WriteLine($"One minimum is found at: (x,y) = ({hb[0]:F8}, {hb[1]:F8})");

            // --------------------------------------------------------------------------
            writer.WriteLine("\n\n------------ Bound states of hydrogen atom with shooting method for boundary value problems ------------\n");

            double rMin = 1e-4, rMax = 8.0, acc = 1e-6, eps = 1e-6;
            writer.WriteLine($"------ Find the lowest root, E_0, of the equation M(E)=0 ------\n");
            writer.WriteLine($"This is done for r_max = {rMax}\n");

            var (E0, steps) = Hydrogen.GroundState(rMin, rMax, acc, eps);
            writer.WriteLine($"The lowest root is found to be:\nE_0 = {E0:F8}  Hartree   (iterations = {steps})\n");

            // Produce data & plot
            writer.WriteLine("------ Plot the resulting wave-function and compare with the exact result ------\n");
            Hydrogen.DumpWaveFunctions(E0, rMin, rMax);
            writer.WriteLine("wave_function.svg contains a plot of the resulting wave‑function together with the exact 1s curve.\n");

            // Convergence w.r.t. r_max, r_min, acc, eps – small demo -----------------
            writer.WriteLine("------ Investigate convergence with respect to the r_max ------\n");
            writer.WriteLine("r_max    E_0");
            foreach(double rM in new[]{6.0,7.0,8.0,9.0,10.0})
                writer.WriteLine($"{rM,5:F1}  {Hydrogen.GroundState(rMin,rM).E0:F8}");

            writer.WriteLine("\n------ Investigate convergence with respect to the r_min ------\n");
            writer.WriteLine("r_min    E_0");
            foreach(double r0 in new[]{1e-3,5e-4,1e-4})
                writer.WriteLine($"{r0,5:E1}  {Hydrogen.GroundState(r0,rMax).E0:F8}");

            writer.WriteLine("\n------ Investigate convergence with respect to the acc ------\n");
            writer.WriteLine("acc      E_0");
            foreach(double a in new[]{1e-4,1e-6,1e-8})
                writer.WriteLine($"{a,5:E1}  {Hydrogen.GroundState(rMin,rMax,a).E0:F8}");

            writer.WriteLine("\n------ Investigate convergence with respect to the eps ------\n");
            writer.WriteLine("eps      E_0");
            foreach(double e in new[]{1e-4,1e-6,1e-8})
                writer.WriteLine($"{e,5:E1}  {Hydrogen.GroundState(rMin,rMax,acc,e).E0:F8}");

            // --------------------------------------------------------------------------
            writer.WriteLine("\n------------ TASK C: Quadratic interpolation line-search ------------\n");
            writer.WriteLine("Demonstration: re‑optimise Rosenbrock using quadratic line search …");

            var rbFast = Newton.Solve(Functions.RosenbrockGrad, new Vec(-1.2,1.0),
                                    quadraticLineSearch:true);
            writer.WriteLine($"\nOptimised point : ({rbFast[0]:F8}, {rbFast[1]:F8})");

            writer.WriteLine("\n---------- End of automatic report ----------");
        }
    }
}
// ---------------------------------------------------------------------------
