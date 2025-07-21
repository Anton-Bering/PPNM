using System;
using System.IO;
using static System.Math;
using System.Linq;
using System.Globalization;

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
        Console.WriteLine("------ Plot the estimated error and the actual error as functions of the number of sampling points ------");
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

        Console.WriteLine("\n------ Calculating The Special 3D Integral ------\n");
        
        Console.WriteLine("The Special 3D integral: ∫₀^π dx/π ∫₀^π dy/π ∫₀^π dz/π [1 - cos(x)cos(y)cos(z)]⁻¹ = Γ(¼)⁴ / (4π³) ≈ 1.3932039296856768591842462603255\n"); // Integrale
        
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


        // --- TASJ B: 
        Console.WriteLine("\n ------------ TASK B ------------");

        // Estimated the error by using two different sequences:
        Console.WriteLine("\n------ Estimated the error by using two different sequences ------\n");
        Console.WriteLine("Estimate_QMC_vs_MC.txt contains the data for the estimated errors.");
        Console.WriteLine("Estimate_QMC_vs_MC_error.svg is a plot showing the estimated and actual error for MC and QMC as a function of N.");
        Console.WriteLine("The QMC error is estimated as the absolute difference between two independent Halton sequence evaluations.");
        // Data setup
        double[] a2 = { 0.0, 0.0 }, b2 = { 1.0, 1.0 };
        int[] Nlist = { 1000, 10000, 100000 };
        double trueValue = 0.25;
        // Save MC vs QMC results for plotting
        string compFile = "Estimate_QMC_vs_MC.txt";
        using (var writer = new StreamWriter(compFile))
        {
            writer.WriteLine("N\tMC est. err\tMC actual err\tQMC est. err\tQMC actual err");

            foreach (int N in Nlist)
            {
                var (resMC, errMC) = PlainMC.Integrate(Fxy, a2, b2, N);
                var (resQMC, errQMC) = QuasiRandomMC.Integrate(Fxy, a2, b2, N);

                double actualErrMC = Math.Abs(resMC - trueValue);
                double actualErrQMC = Math.Abs(resQMC - trueValue);

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "{0}\t{1:E6}\t{2:E6}\t{3:E6}\t{4:E6}",
                    N, errMC, actualErrMC, errQMC, actualErrQMC));
            }
        }




        // Compare the scaling of the error with your pseudo-random Monte-Carlo integrator:
        Console.WriteLine("\n------ Compare the scaling of the error with my pseudo-random Monte-Carlo integrator ------\n");
        Console.WriteLine("QuasiVsPseudoResults.txt contains the estimated and actual errors for both MC and QMC.");
        Console.WriteLine("Estimate_QMC_vs_MC_error.svg is a log-log plot visualizing the error scaling as a function of N.");
        Console.WriteLine("QMC achieves lower error and faster convergence compared to MC, especially for smaller sample sizes.");
        QuasiVsPseudoComparison.RunComparison();
        
        
        // ------------ TASK C ------------
        Console.WriteLine("\n ------------ TASK C ------------");

        Console.WriteLine("This task implements recursive stratified sampling, where the domain is adaptively subdivided based on estimated sub-variances.");
        Console.WriteLine("The integrator distributes more points where the function varies most, improving accuracy in difficult regions.");
        Console.WriteLine("The method is recursive and stops subdividing when the number of points is below a fixed threshold (nmin).\n");

        double trueVal = 1000.0 * (1 - Math.Exp(-100.0)) / 100.0 * (1 - Math.Exp(-10.0)) / 10.0;
        int[] NlistStrat = { 1000, 2000, 5000, 10000, 20000, 50000 };

        string stratErrFile = "StratifiedSamplingErrors.txt";
        using (var writer = new StreamWriter(stratErrFile))
        {
            writer.WriteLine("N\tPlain_actual_error\tStratified_actual_error");

            foreach (int N in NlistStrat)
            {
                var (plainRes, plainErrDummy) = PlainMC.Integrate(PeakedFunction, new[] { 0.0, 0.0 }, new[] { 1.0, 1.0 }, N);
                var (stratRes, stratErrDummy) = StratifiedMC.Integrate(PeakedFunction, new[] { 0.0, 0.0 }, new[] { 1.0, 1.0 }, N);

                double errPlain = Math.Abs(plainRes - trueVal);
                double errStrat = Math.Abs(stratRes - trueVal);

                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}\t{1:E6}\t{2:E6}", N, errPlain, errStrat));
            }
        }

        Console.WriteLine("StratifiedSamplingErrors.txt contains error data for plain vs stratified MC.");
        Console.WriteLine("StratifiedSamplingError.svg shows the actual error as a function of N in log-log scale.\n");


        StratifiedSamplingComparison.RunComparison();
    }
}
