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
        for (int i = 0; i < n; i++) {
            yCos[i] = Math.Cos(xs[i]);
            ySin[i] = Math.Sin(xs[i]);
            ySqrt[i] = Math.Sqrt(xs[i]);
        }

        QuadraticSpline qs = new QuadraticSpline(xs, ySin);
        CubicSpline cs = new CubicSpline(xs, ySqrt);

        // Write quadratic spline coefficients to file for comparison
        qs.WriteCoefficientsToFile("Computed_bi_and_ci.txt");

        using (StreamWriter writer = new StreamWriter("Out.txt")) {

            // --- PART A ---
            writer.WriteLine("--- Part A ---");
            writer.WriteLine("-- Make some indicative plots to prove that the linear spline and the integrator work as intended --");
            writer.WriteLine("This is done by taking the table: {x_i=0,1,...,9; y_i=cos(x_i)},");
            writer.WriteLine("the data is in cos.txt and the plot is in cos.png.\n");

            using (StreamWriter cosWriter = new StreamWriter("cos.txt")) {
                cosWriter.WriteLine("# x\tspline(x)\tsplineInt(x)");
                for (double z = 0; z <= 9; z += 0.1) {
                    double val = LinearSpline.linterp(xs, yCos, z);
                    double integral = LinearSpline.linterpInteg(xs, yCos, z);
                    cosWriter.WriteLine($"{z}\t{val}\t{integral}");
                }
            }

            // --- PART B ---
            writer.WriteLine("--- Part B ---");
            writer.WriteLine("-- Make some indicative plots to prove that the quadratic spline and the integrator work as intended.");
            writer.WriteLine("This is done by using {x_i=0,1,...,9; y_i=sin(x_i)}.");
            writer.WriteLine("the data is in sin.txt and the plot is in sin.png.\n");

            using (StreamWriter sinWriter = new StreamWriter("sin.txt")) {
                sinWriter.WriteLine("# x\tspline(x)\tsplineInt(x)\tsplineDeriv(x)");
                for (double z = 0; z <= 9; z += 0.1) {
                    double val = qs.Evaluate(z);
                    double integral = qs.Integral(z);
                    double deriv = qs.Derivative(z);
                    sinWriter.WriteLine($"{z}\t{val}\t{integral}\t{deriv}");
                }
            }

            writer.WriteLine("-- Calculate manually the parameters {b_i, c_i} of the corresponding quadratic-splines, and compare the results with the quadratic-spline program --");
            writer.WriteLine("The manual calculations of the values of b_i and c_i are in Manually_calculate_bi_and_ci.txt.\n");


            // --- PART C ---
            writer.WriteLine("--- Part C ---");
            writer.WriteLine("-- Check that the built-in cubic splines in Gnuplot produce a similar cubic spline to the implementation --");
            writer.WriteLine("The data from my result is in sqrt.txt and the plot is in sqrt.png.");
            writer.WriteLine("In sqrt_comparing.png, a comparison is shown between my results (sqrt.txt and sqrt.png) and the built-in Gnuplot spline.\n");


            using (StreamWriter sqrtWriter = new StreamWriter("sqrt.txt")) {
                sqrtWriter.WriteLine("# x\tspline(x)\tsplineInt(x)\tsplineDeriv(x)");
                for (double z = 0; z <= 9; z += 0.1) {
                    double val = cs.Evaluate(z);
                    double integral = cs.Integral(z);
                    double deriv = cs.Derivative(z);
                    sqrtWriter.WriteLine($"{z}\t{val}\t{integral}\t{deriv}");
                }
            }
        }
    }
} 