using System;
using System.IO;
using static System.Math;

class Program
{
    static Random rng = new Random();

    static double Fxy(double[] v) => v[0] * v[1];

    static double PeakedFunction(double[] v)
    {
        double x = v[0], y = v[1];
        return 1000.0 * Exp(-100.0 * x - 10.0 * y);
    }

    // -------------------------------// funktioner til Monte Carlo integration

    static void Main()
    {
        Console.WriteLine("------------ TASK A ------------\n");
        Console.WriteLine("------ Calculate two-dimensional integrals with my Monte Carlo routine ------");
        Console.WriteLine("------ Plot the estimated error and the actual error as functions of the number of sampling points------");
        Console.WriteLine("------ And check whether the actual error scales as N^(-1/2) ------\n");

        Console.WriteLine("--- Area of Unit Circle ---"); // Kode flyttet til UnitCircleCalculator.cs
        UnitCircleCalculator.CalculateAndSaveResults(); // 
        Console.WriteLine("Data saved to: Estimate_the_area_of_a_unit_circle.txt"); // 

        Console.WriteLine("\n Error scaling plotted in: area_of_a_unit_circle_error.plot \n");

        Console.WriteLine("\n--- Calculating Gaussian Bell Integral ---");
        GaussianBellCalculator.CalculateAndSaveResults(); // Kode flyttet til GaussianBellCalculator.cs

        Console.WriteLine("\n Error scaling plotted in: gaussian_bell_integral_error.plot \n");
        // HERHER Gør så at der komm et plot: gaussian_bell_integral_error.plot

        Console.WriteLine("\n--- Calculating Special 3D Integral ---");
        SpecialIntegralCalculator.CalculateAndSaveResults(); // Kode flyttet til SpecialIntegralCalculator.cs
 


        // Part B: Quasi-Random vs Pseudo-Random
        Console.WriteLine("Part B: Quasi-Random Sequences vs Pseudorandom Monte Carlo");
        Console.WriteLine("Integrand f(x,y) = x*y over [0,1]^2 (true value = 0.250000)");
        Console.WriteLine("{0,8}  {1,14}  {2,14}  {3,14}  {4,14}", "N", "MC est.err", "MC actual err", "QMC est.err", "QMC actual err");

        // Part B
        QuasiVsPseudoComparison.RunComparison(); // Kode flyttet til QuasiVsPseudoComparison.cs

        Console.WriteLine("\n(Estimated errors: MC uses internal variance; QMC uses difference of two sequences.)\n");

        // Part C: Stratified Sampling
        Console.WriteLine("Part C: Recursive Stratified Sampling");

        StratifiedSamplingComparison.RunComparison(); // Kode flyttet til StratifiedSamplingComparison.cs

        
    }
}