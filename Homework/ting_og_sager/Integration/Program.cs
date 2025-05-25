using System;
using static Integrator;  // Simplify calls to Integrator methods

public class Program
{
    public static void Main(string[] args)
    {
        // Define variables for capturing function call counts
        int calls;

        Console.WriteLine("Adaptive Integration Test Cases (acc=1e-3, eps=1e-3):\n");

        // 1. ∫_0^1 sqrt(x) dx = 2/3 ≈ 0.666667
        calls = 0;
        Func<double, double> f1 = x => { calls++; return Math.Sqrt(x); };
        double result1 = Integrate(f1, 0.0, 1.0);
        Console.WriteLine("∫_0^1 sqrt(x) dx ≈ {0:F6}   (exact = 0.666667, calls = {1})", result1, calls);

        // 2. ∫_0^1 1/sqrt(x) dx = 2
        calls = 0;
        Func<double, double> f2 = x => { calls++; return 1.0/Math.Sqrt(x); };
        double result2 = Integrate(f2, 0.0, 1.0);
        Console.WriteLine("∫_0^1 1/√(x) dx   ≈ {0:F6}   (exact = 2, calls = {1})", result2, calls);

        // 3. ∫_0^1 ln(x)/√(x) dx = -4
        calls = 0;
        Func<double, double> f3 = x => { calls++; return Math.Log(x)/Math.Sqrt(x); };
        double result3 = Integrate(f3, 0.0, 1.0);
        Console.WriteLine("∫_0^1 ln(x)/√(x) dx ≈ {0:F6}   (exact = -4, calls = {1})", result3, calls);

        // 4. ∫_0^1 4√(1 - x^2) dx = π ≈ 3.141593  (area of quarter-circle * 4)
        calls = 0;
        Func<double, double> f4 = x => { calls++; return 4.0 * Math.Sqrt(1 - x * x); };
        double result4 = Integrate(f4, 0.0, 1.0);
        Console.WriteLine("∫_0^1 4√(1-x²) dx ≈ {0:F6}   (exact = 3.141593, calls = {1})", result4, calls);

        Console.WriteLine("\nHandling challenging integrals with Clenshaw–Curtis transform:");
        // Compare performance with and without variable transformation for singular integrals:
        // a) 1/sqrt(x) on [0,1]
        calls = 0;
        Func<double, double> f2a = x => { calls++; return 1.0/Math.Sqrt(x); };
        // reuse result2 from above for no-transform result and calls
        int callsNoTrans = calls; // (calls from previous direct integration stored in callsNoTrans? We'll recompute below for clarity)
        // Actually recompute without transform to get call count:
        callsNoTrans = 0;
        Integrate(x => { callsNoTrans++; return 1.0/Math.Sqrt(x); }, 0.0, 1.0);
        // Now with Clenshaw–Curtis transform
        calls = 0;
        double result2_trans = IntegrateClenshawCurtis(f2a, 0.0, 1.0);
        int callsTrans = calls;
        Console.WriteLine("∫_0^1 1/√(x) dx calls: {0} (no transform) vs {1} (with transform)", callsNoTrans, callsTrans);

        // b) ln(x)/sqrt(x) on [0,1]
        calls = 0;
        Func<double, double> f3a = x => { calls++; return Math.Log(x)/Math.Sqrt(x); };
        // calls without transform (from previous result3 we have calls count)
        callsNoTrans = 0;
        Integrate(x => { callsNoTrans++; return Math.Log(x)/Math.Sqrt(x); }, 0.0, 1.0);
        // with transform
        calls = 0;
        double result3_trans = IntegrateClenshawCurtis(f3a, 0.0, 1.0);
        callsTrans = calls;
        Console.WriteLine("∫_0^1 ln(x)/√(x) dx calls: {0} (no transform) vs {1} (with transform)", callsNoTrans, callsTrans);

        Console.WriteLine("\nInfinite limit integrals with transformation:");
        // 5. ∫_0^∞ e^(-x) dx = 1
        calls = 0;
        Func<double, double> f5 = x => { calls++; return Math.Exp(-x); };
        double result5 = IntegrateClenshawCurtis(f5, 0.0, Double.PositiveInfinity);
        Console.WriteLine("∫_0^∞ e^(-x) dx   ≈ {0:F6}   (exact = 1, calls = {1})", result5, calls);

        // 6. ∫_0^∞ e^(-x^2) dx = √π/2 ≈ 0.8862269
        calls = 0;
        Func<double, double> f6 = x => { calls++; return Math.Exp(-x * x); };
        double result6 = IntegrateClenshawCurtis(f6, 0.0, Double.PositiveInfinity);
        Console.WriteLine("∫_0^∞ e^(-x^2) dx ≈ {0:F6}   (exact ≈ 0.886227, calls = {1})", result6, calls);

        Console.WriteLine("\nError function (erf) computation via integration:");
        // Compute erf(1) (should be ~0.8427007929)
        double erf1 = Integrate(f6, 0.0, 1.0, 1e-6, 0);  // Alternatively, use Integrator.Erf(1.0)
        erf1 = (2.0/Math.Sqrt(Math.PI)) * erf1;
        Console.WriteLine("erf(1) ≈ {0:F9}   (expected ≈ 0.842700793)", erf1);
        // Also demonstrate the Erf function directly
        double erfNeg = Integrator.Erf(-1.0);
        Console.WriteLine("erf(-1) ≈ {0:F9}  (should be -erf(1))", erfNeg);
    }
}
