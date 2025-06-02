using System;
using static System.Math;

class Program
{
    // Example integrand for unit circle area: f(x,y) = 1 inside the circle x^2+y^2 <= 1, else 0.
    static double UnitCircleIndicator(double[] v)
    {
        double x = v[0], y = v[1];
        return (x*x + y*y <= 1.0) ? 1.0 : 0.0;
    }

    // Special 3D integrand f(x,y,z) = 1/(π^3 * [1 - cos x cos y cos z]) for x,y,z in [0,π]
    static double SpecialIntegrand(double[] v)
    {
        double A = 1.0 / (PI * PI * PI);
        return A / (1.0 - Cos(v[0]) * Cos(v[1]) * Cos(v[2]));
    }

    // A smooth 2D test integrand for Quasi-Monte Carlo (with known result): f(x,y) = x*y on [0,1]^2
    static double Fxy(double[] v)
    {
        return v[0] * v[1];
    }

    // A peaked 2D function to test stratified sampling: 1000*exp(-100x - 10y) on [0,1]^2
    static double PeakedFunction(double[] v)
    {
        double x = v[0], y = v[1];
        return 1000.0 * Exp(-100.0 * x - 10.0 * y);
    }

    static void Main()
    {
        // Part A: Plain Monte Carlo Integration
        Console.WriteLine("Part A: Plain Monte Carlo Integration");
        // 2D example: Area of unit circle (quarter-circle method or full square)
        double[] a2 = { -1.0, -1.0 };
        double[] b2 = {  1.0,  1.0 };
        double trueArea = PI * 1.0 * 1.0;  // π * r^2, r=1
        Console.WriteLine("Estimating area of unit circle (true value = {0:F6})", trueArea);
        Console.WriteLine("{0,8}  {1,15}  {2,15}  {3,15}", "N", "MC est. error", "MC actual error", "Value");
        int[] Ns = { 1000, 2000, 5000, 10000 };
        foreach (int N in Ns)
        {
            var (res, err) = PlainMC.Integrate(UnitCircleIndicator, a2, b2, N);
            double actualErr = Math.Abs(res - trueArea);
            Console.WriteLine($"{N,8}  {err,15:E6}  {actualErr,15:E6}  {res,15:F6}");
        }
        Console.WriteLine();

        // Special 3D difficult integral
        double[] a3 = { 0.0, 0.0, 0.0 };
        double[] b3 = { PI, PI, PI };
        double analytical = 1.393203929685676859;  // known analytical value Γ(1/4)^4/(4π^3)
        int N3 = 1000000;
        Console.WriteLine("Estimating ∫[0,π]^3 [1 - cos x cos y cos z]^-1 (normalized) ≈ 1.39320393...");
        var (specialRes, specialErr) = PlainMC.Integrate(SpecialIntegrand, a3, b3, N3);
        Console.WriteLine("Monte Carlo result:    {0:F9} ± {1:F9}", specialRes, specialErr);
        Console.WriteLine("Analytical result:     {0:F9}", analytical);
        Console.WriteLine("Absolute difference:   {0:F9}", Math.Abs(specialRes - analytical));
        Console.WriteLine();

        // Part B: Quasi-Random (Low-Discrepancy) vs Plain Monte Carlo
        Console.WriteLine("Part B: Quasi-Random Sequences vs Pseudorandom Monte Carlo");
        Console.WriteLine("Integrand f(x,y) = x*y over [0,1]^2 (true value = 0.250000)");
        Console.WriteLine("{0,8}  {1,14}  {2,14}  {3,14}  {4,14}", "N", "MC est.err", "MC actual err", "QMC est.err", "QMC actual err");
        int[] Nlist = { 1000, 10000, 100000 };
        foreach (int N in Nlist)
        {
            // Plain MC
            var (resR, errR) = PlainMC.Integrate(Fxy, new double[]{0,0}, new double[]{1,1}, N);
            double actualErrR = Math.Abs(resR - 0.25);
            // Quasi MC
            var (resQ, errQ) = QuasiRandomMC.Integrate(Fxy, new double[]{0,0}, new double[]{1,1}, N);
            double actualErrQ = Math.Abs(resQ - 0.25);
            Console.WriteLine($"{N,8}  {errR,14:E6}  {actualErrR,14:E6}  {errQ,14:E6}  {actualErrQ,14:E6}");
        }
        Console.WriteLine("(Estimated errors: MC uses internal variance; QMC uses difference of two sequences.)");
        Console.WriteLine();

        // Part C: Stratified Sampling
        Console.WriteLine("Part C: Recursive Stratified Sampling");
        // Compare plain vs stratified on a challenging 2D integrand
        double[] aP = {0.0, 0.0}, bP = {1.0, 1.0};
        // True value of ∫_0^1 1000 e^{-100x - 10y} dxdy = 1000*(1 - e^-100)/100 * (1 - e^-10)/10
        double trueVal = 1000.0 * (1 - Exp(-100.0)) / 100.0 * (1 - Exp(-10.0)) / 10.0;
        int Np = 10000;
        var (plainRes, plainErr) = PlainMC.Integrate(PeakedFunction, aP, bP, Np);
        var (stratRes, stratErr) = StratifiedMC.Integrate(PeakedFunction, aP, bP, Np);
        Console.WriteLine("Integrating 1000*exp(-100x - 10y) on [0,1]^2 (true value ≈ {0:F6}) with N={1}", trueVal, Np);
        Console.WriteLine("Plain MC:      result = {0:F6},  error estimate = {1:F6},  actual error = {2:F6}", 
                          plainRes, plainErr, Math.Abs(plainRes - trueVal));
        Console.WriteLine("Stratified MC: result = {0:F6},  error estimate = {1:F6},  actual error = {2:F6}", 
                          stratRes, stratErr, Math.Abs(stratRes - trueVal));
    }
}
