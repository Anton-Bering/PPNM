using System;
public class Program {
    public static void Main(string[] args) {
        // Prepare test data
        int n = 10;
        double[] xs = new double[n];
        for(int i = 0; i < n; i++) xs[i] = i;
        // Linear spline test with cos(x)
        double[] yLin = new double[n];
        for(int i = 0; i < n; i++) yLin[i] = Math.Cos(xs[i]);
        // Quadratic spline test with sin(x)
        double[] yQuad = new double[n];
        for(int i = 0; i < n; i++) yQuad[i] = Math.Sin(xs[i]);
        // Cubic spline test with sqrt(x)
        double[] yCub = new double[n];
        for(int i = 0; i < n; i++) yCub[i] = Math.Sqrt(xs[i]);

        // Create spline objects
        QuadraticSpline qs = new QuadraticSpline(xs, yQuad);
        CubicSpline cs = new CubicSpline(xs, yCub);
        // Use LinearSpline static methods for linear test

        // Print results to console (to be redirected to Out.txt)
        Console.WriteLine("Linear spline interpolation (y = cos(x))");
        Console.WriteLine("x\tSpline(x)\tSplineInt(x)");
        for(double z = xs[0]; z <= xs[n-1]; z += 0.5) {
            double sVal = LinearSpline.linterp(xs, yLin, z);
            double sInt = LinearSpline.linterpInteg(xs, yLin, z);
            Console.WriteLine($"{z:F1}\t{sVal:F6}\t{sInt:F6}");
        }
        Console.WriteLine();

        Console.WriteLine("Quadratic spline interpolation (y = sin(x))");
        Console.WriteLine("x\tSpline(x)\tSplineInt(x)");
        for(double z = xs[0]; z <= xs[n-1]; z += 0.5) {
            double sVal = qs.Evaluate(z);
            double sInt = qs.Integral(z);
            Console.WriteLine($"{z:F1}\t{sVal:F6}\t{sInt:F6}");
        }
        Console.WriteLine();

        Console.WriteLine("Cubic spline interpolation (y = sqrt(x))");
        Console.WriteLine("x\tSpline(x)\tSplineInt(x)");
        for(double z = xs[0]; z <= xs[n-1]; z += 0.5) {
            double sVal = cs.Evaluate(z);
            double sInt = cs.Integral(z);
            Console.WriteLine($"{z:F1}\t{sVal:F6}\t{sInt:F6}");
        }
    }
}
