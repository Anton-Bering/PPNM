using System;
using System.IO;
using static System.Math;
using System.Linq;

class Program
{
    static double Fxy(double[] v) => v[0] * v[1];

    static double PeakedFunction(double[] v)
    {
        double x = v[0], y = v[1];
        return 1000.0 * Exp(-100.0 * x - 10.0 * y);
    }

    // -------------------------------// funktioner til Monte Carlo integration

    static void Main()
    {
        Console.WriteLine("------------ TASK A ------------");
        Console.WriteLine("\n------ Calculate two-dimensional integrals with my Monte Carlo routine ------");
        Console.WriteLine("------ Plot the estimated error and the actual error as functions of the number of sampling points------");
        Console.WriteLine("------ And check whether the actual error scales as N^(-1/2) ------");

        // --- Area of Unit Circle ---
        Console.WriteLine("\n--- Area of Unit Circle ---");
        UnitCircleCalculator.CalculateAndSaveResults();

        string circlePath = "Estimate_the_area_of_a_unit_circle.txt";
        if (File.Exists(circlePath))
        {
            string lastLine = File.ReadAllLines(circlePath)
                                   .Reverse()
                                   .First(line => line.Trim().Length > 0 && !line.StartsWith("N"));

            string[] parts = lastLine.Split('\t');
            int lastN = int.Parse(parts[0]);
            double lastEstimate = double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture);

            Console.WriteLine($"\nThe file '{circlePath}' contains the Monte Carlo estimates of the unit circle area.");
            Console.WriteLine($"After {lastN} samples, the estimated area is {lastEstimate:F6}.");
            Console.WriteLine($"The plot 'area_of_a_unit_circle_error.plot' shows the estimated and actual error as a function of N.");
            Console.WriteLine($"As shown in the plot, the error scales approximately as N^(-1/2), as expected from theory.");
        }
        else
        {
            Console.WriteLine("\nError: Estimate_the_area_of_a_unit_circle.txt not found.");
        }

        // --- Gaussian Bell Integral ---
        Console.WriteLine("\n--- Calculating Gaussian Bell Integral ---");
        GaussianBellCalculator.CalculateAndSaveResults();

        string gaussPath = "Estimate_GaussianBell2D.txt";
        if (File.Exists(gaussPath))
        {
            string lastLine_gauss = File.ReadAllLines(gaussPath)
                                        .Reverse()
                                        .FirstOrDefault(line => line.Trim().Length > 0 && !line.StartsWith("N"));

            if (lastLine_gauss != null)
            {
                string[] parts_gauss = lastLine_gauss.Split('\t');
                // Console.WriteLine($"Parsing line: {lastLine_gauss}");

                if (parts_gauss.Length >= 4)
                {
                    bool successN = int.TryParse(parts_gauss[0], out int lastN_gauss);
                    bool successEstimate = double.TryParse(parts_gauss[3], System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double lastEstimate_gauss);

                    if (successN && successEstimate)
                    {
                        Console.WriteLine($"\nThe file '{gaussPath}' contains the Monte Carlo estimates of the area.");
                        Console.WriteLine($"After {lastN_gauss} samples, the estimated area is {lastEstimate_gauss:F6}.");
                        Console.WriteLine($"The plot 'Estimate_GaussianBell2D_error.plot' shows the estimated and actual error as a function of N.");
                        Console.WriteLine($"As shown in the plot, the error scales approximately as N^(-1/2), as expected from theory.");
                    }
                    else
                    {
                        Console.WriteLine("Error: Could not parse values from last data line in 'Estimate_GaussianBell2D.txt'.");
                        Console.WriteLine($" - Parse N success: {successN}");
                        Console.WriteLine($" - Parse estimate success: {successEstimate}");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Last data line does not contain enough columns (expected at least 4).");
                }
            }
            else
            {
                Console.WriteLine("Error: No valid data line found in 'Estimate_GaussianBell2D.txt'.");
            }
        }
        else
        {
            Console.WriteLine("Error: File 'Estimate_GaussianBell2D.txt' not found.");
        }

        Console.WriteLine("\n------ Calculating The Special 3D Integral ------");
        Console.WriteLine("------ The Special 3D integral: ∫₀^π dx/π ∫₀^π dy/π ∫₀^π dz/π [1 - cos(x)cos(y)cos(z)]⁻¹ = Γ(¼)⁴ / (4π³) ≈ 1.3932039296856768591842462603255 ------"); // Integrale
        SpecialIntegralCalculator.CalculateAndSaveResults();
        string specialPath = "Estimate_SpecialIntegral.txt";
        if (File.Exists(specialPath))
        {
            string lastLine_special = File.ReadAllLines(specialPath)
                                        .Reverse()
                                        .FirstOrDefault(line =>
                                        {
                                            string[] parts = line.Split('\t');
                                            return parts.Length >= 4 &&
                                                    double.TryParse(parts[3], System.Globalization.NumberStyles.Float,
                                                        System.Globalization.CultureInfo.InvariantCulture, out _);
                                        });

            if (lastLine_special != null)
            {
                string[] parts_special = lastLine_special.Split('\t');
                int.TryParse(parts_special[0], out int lastN_special);
                double.TryParse(parts_special[3], System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out double result_special);

                Console.WriteLine($"The file '{specialPath}' contains the results for the value of the Special 3D Integral.");
                Console.WriteLine($"After {lastN_special} samples, the estimated value is {result_special:F9}.");
            }
            else
            {
                Console.WriteLine($"Warning: No valid data found in '{specialPath}'.");
            }
        }
        else
        {
            Console.WriteLine($"Error: File '{specialPath}' not found.");
        }

        Console.WriteLine("\n------ Calculating The less sungular 3D Integral ------");
        Console.WriteLine("------ The less sungular 3D integral: ∫₀^π dx ∫₀^π dy ∫₀^π dz cos(x)cos(y)cos(z) /π³ = 8/π³ ≈ 0.2580122754655959134753764215085 ------\n"); // Integrale
        LessSungularIntegralCalculator.CalculateAndSaveResults();
        LessSungularIntegralCalculator.PrintLastEstimateSummary();


        // --- Part B: Quasi vs Pseudo ---
        Console.WriteLine("\n ------------ TASK B ------------");
        Console.WriteLine("\n ------ Compare the scaling of the error with my pseudo-random Monte-Carlo integrator ------");
        Console.WriteLine("Integrand f(x,y) = x*y over [0,1]^2 (true value = 0.250000)");
        Console.WriteLine("{0,8}  {1,14}  {2,14}  {3,14}  {4,14}", "N", "MC est.err", "MC actual err", "QMC est.err", "QMC actual err");
        Console.WriteLine("TJEK 1");
        QuasiVsPseudoComparison.RunComparison();
        Console.WriteLine("TJEK 2");
        Console.WriteLine("\n(Estimated errors: MC uses internal variance; QMC uses difference of two sequences.)\n");
        Console.WriteLine("TJEK 3");
        // --- Part C: Stratified Sampling ---
        Console.WriteLine("Part C: Recursive Stratified Sampling");
        StratifiedSamplingComparison.RunComparison();
    }
}
