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
        Console.WriteLine("\n------ Calculate integrals with the Monte Carlo routine and plot errors ------");

        // 1) Areal af enhedscirkel
        GenericMCCalculator.Run(
            f: v => (v[0]*v[0] + v[1]*v[1] <= 1) ? 1.0 : 0.0,
            a: new[] { -1.0, -1.0 },
            b: new[] { 1.0, 1.0 },
            trueValue: Math.PI,
            dataFile: "Estimate_the_area_of_a_unit_circle.txt",
            description: "Unit Circle Area"
        );

        // 2) 2D Gauss-integral
        GenericMCCalculator.Run(
            f: v => Math.Exp(-(v[0]*v[0] + v[1]*v[1])),
            a: new[] { -2.0, -2.0 },
            b: new[] { 2.0, 2.0 },
            trueValue: 3.141592653589793 * (1 - Math.Exp(-4)), // Præcis værdi for det afgrænsede integral
            dataFile: "Estimate_GaussianBell2D.txt",
            description: "Gaussian Bell Integral"
        );

        // 3) "Svært" singulært integral
        GenericMCCalculator.Run(
            f: v => {
                double d = 1.0 - Math.Cos(v[0])*Math.Cos(v[1])*Math.Cos(v[2]);
                return (d == 0) ? 0 : 1.0 / (Math.PI*Math.PI*Math.PI * d);
            },
            a: new[] { 0.0, 0.0, 0.0 },
            b: new[] { Math.PI, Math.PI, Math.PI },
            trueValue: 1.393203929685676859,
            dataFile: "Estimate_SpecialIntegral.txt",
            description: "Special 3D Integral"
        );

        // 4) Mindre singulært integral (til test)
        GenericMCCalculator.Run(
            f: v => Math.Cos(v[0])*Math.Cos(v[1])*Math.Cos(v[2]) / (Math.PI*Math.PI*Math.PI),
            a: new[] { 0.0, 0.0, 0.0 },
            b: new[] { Math.PI, Math.PI, Math.PI },
            trueValue: 0.0, // Integralet af cos(x) fra 0 til pi er 0, så hele integralet er 0
            dataFile: "Estimate_LessSungularIntegral.txt",
            description: "Less Singular 3D Integral"
        );

        Console.WriteLine("\nAll calculations for Task A are complete.");
        Console.WriteLine("Data files are generated, and plots can be created using 'make'.");


        // --- TASK B:
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
