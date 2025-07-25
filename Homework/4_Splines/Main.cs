using System;
using System.IO;

public class Program {
    public static void Main(string[] args) {
        int n = 10;
        double[] xs = new double[n];
        for (int i = 0; i < n; i++) xs[i] = i;

        double[] yCos = new double[n];
        double[] ySin = new double[n];
        double[] ySqrt = new double[n];
        double[] yQuad = new double[n];
        double[] yLog = new double[n];
        for (int i = 0; i < n; i++) {
            yCos[i] = Math.Cos(xs[i]);
            ySin[i] = Math.Sin(xs[i]);
            ySqrt[i] = Math.Sqrt(xs[i]);
            yQuad[i] = xs[i] * xs[i];
            yLog[i] = Math.Log(xs[i] + 1.0);
        }

        QuadraticSpline qs = new QuadraticSpline(xs, ySin);
        QuadraticSpline qLog = new QuadraticSpline(xs, yLog);
        CubicSpline cs = new CubicSpline(xs, ySqrt);


        // Write quadratic spline coefficients to file for comparison
        qs.WriteCoefficientsToFile("Computed_bi_and_ci_sin.txt");
        qLog.WriteCoefficientsToFile("Computed_bi_and_ci_log.txt");

    

        // --- TASJ A ---
        Console.WriteLine("------------ TASK A ------------");
        Console.WriteLine("\n------ Make some indicative plots to prove the linear spline and the integrator work as intended ------\n");
                    
        Console.WriteLine("--- Take the table: {x_i=0,1,...,9; y_i=cos(x_i)} and plot its linear interpolant ---");
        Console.WriteLine("--- together with interpolant's anti-derivative                                   ---\n");

        Console.WriteLine("The data is in cos.txt and the plot is in cos.svg");
        using (StreamWriter cosWriter = new StreamWriter("Out_Task_A/cos.txt")) {
            cosWriter.WriteLine("# x\tspline(x)\tsplineInt(x)");
            for (double z = 0; z <= 9; z += 0.1) {
                double val = LinearSpline.linterp(xs, yCos, z);
                double integral = LinearSpline.linterpInteg(xs, yCos, z);
                cosWriter.WriteLine($"{z}\t{val}\t{integral}");
            }
        }

        Console.WriteLine("--- Take the table: {x_i=0,1,...,9; y_i=x_i^2} and plot its linear interpolant ---");
        Console.WriteLine("--- together with interpolant's anti-derivative                                ---\n");

        Console.WriteLine("The data is in quad.txt and the plot is in quad.svg");
        using (StreamWriter quadWriter = new StreamWriter("Out_Task_A/quad.txt")) {
            quadWriter.WriteLine("# x\tspline(x)\tsplineInt(x)");
            for (double z = 0; z <= 9; z += 0.1) {
                double val = LinearSpline.linterp(xs, yQuad, z);
                double integral = LinearSpline.linterpInteg(xs, yQuad, z);
                quadWriter.WriteLine($"{z}\t{val}\t{integral}");
            }
        }

        // --- TASK B ---
        Console.WriteLine("\n------------ TASK B ------------");
        
        Console.WriteLine("\n------ Make some indicative plots to prove that the quadratic spline ------");
        Console.WriteLine("------ and the integrator work as intended                           ------\n");

        Console.WriteLine("--- Using {x_i=0,1,...,9; y_i=sin(x_i)} ---\n");
        Console.WriteLine("The data is in sin.txt and the plot is in sin.svg.");
        using (StreamWriter sinWriter = new StreamWriter("Out_Task_B/sin.txt")) {
            sinWriter.WriteLine("# x\tspline(x)\tsplineInt(x)\tsplineDeriv(x)");
            for (double z = 0; z <= 9; z += 0.1) {
                double val = qs.Evaluate(z);
                double integral = qs.Integral(z);
                double deriv = qs.Derivative(z);
                sinWriter.WriteLine($"{z}\t{val}\t{integral}\t{deriv}");
            }
        }

        Console.WriteLine("\n--- Using {x_i=0,1,...,9; y_i=ln(x_i+1)} ---\n");
        Console.WriteLine("The data is in log.txt and the plot is in log.svg.");
        using (StreamWriter logWriter = new StreamWriter("Out_Task_B/log.txt")) {
            logWriter.WriteLine("# x\tspline(x)\tsplineInt(x)\tsplineDeriv(x)");
            for (double z = 0; z <= 9; z += 0.1) {
                double val = qLog.Evaluate(z);
                double integral = qLog.Integral(z);
                double deriv = qLog.Derivative(z);
                logWriter.WriteLine($"{z}\t{val}\t{integral}\t{deriv}");
            }
        }

        
        Console.WriteLine("\n------ Calculate manually the parameters {b_i, c_i} of the corresponding quadratic-splines ------");
        Console.WriteLine("------ And compare the results with the quadratic-spline program ------\n");

        Console.WriteLine("\n For Sin(x):");
        Console.WriteLine("The manually calculated values are in Manually_calculate_bi_and_ci_sin.txt.");
        Console.WriteLine("The program-computed values are in Computed_bi_and_ci_sin.txt.");
        Console.WriteLine("The manual and program-computed values are compared visually in the plot bi_and_ci_comparing.svg.\n");
        
        Console.WriteLine("\n For ln(x+1):");
        Console.WriteLine("The manually calculated values are in Manually_calculate_bi_and_ci_log.txt.");
        Console.WriteLine("The program-computed values are in Computed_bi_and_ci_log.txt.");
        Console.WriteLine("The manual and program-computed values are compared visually in the plot bi_and_ci_comparing_log.svg.\n");


        // --- TASL C ---
        Console.WriteLine("\n------------ TASK C ------------\n");
        
        Console.WriteLine("------- Check that the built-in cubic splines in Gnuplot produce a similar cubic spline to the implementation ------\n");
        Console.WriteLine("The results from my implementation are in sqrt.txt, and the plot is in sqrt.svg.");
        Console.WriteLine("In sqrt_comparing.svg, a comparison is shown between my results (from sqrt.txt) and the built-in Gnuplot spline (using the data in sqrt_data_points_to_Gnuplot.txt).\n");

        using (StreamWriter sqrtWriter = new StreamWriter("Out_Task_C/sqrt.txt")) {
            sqrtWriter.WriteLine("# x\tspline(x)\tsplineInt(x)\tsplineDeriv(x)");
            for (double z = 0; z <= 9; z += 0.1) {
                double val = cs.Evaluate(z);
                double integral = cs.Integral(z);
                double deriv = cs.Derivative(z);
                sqrtWriter.WriteLine($"{z}\t{val}\t{integral}\t{deriv}");
            }
        }

        // --- POINTS from task A, B, and C  ---
        int HW_POINTS_A = 1;
        int HW_POINTS_B = 1;
        int HW_POINTS_C = 1;
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
        
    }
} 