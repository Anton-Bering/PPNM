using System;

class Program {
    static void Main() {
        Func<double, string> fmt = (x) => String.Format("{0:F9}", x);

        Console.WriteLine("Part A: Testing basic adaptive integrator on sample integrals.");
        // 1) ∫_0^1 √x dx = 2/3 ≈ 0.666667
        Func<double,double> f1 = x => Math.Sqrt(x);
        double exact1 = 2.0/3.0;
        double err;
        double res1 = AdaptiveIntegrator.Integrate(f1, 0.0, 1.0, 1e-6, 1e-6, out err);
        Console.WriteLine($"∫[0,1] √x dx = {fmt(res1)} \t(exact = {fmt(exact1)}, error = {fmt(res1 - exact1)})");
        Console.WriteLine($"Estimated error = {fmt(err)}");

        // 2) ∫_0^1 x^(-1/2) dx = 2
        Func<double,double> f2 = x => 1.0/Math.Sqrt(x);
        double exact2 = 2.0;
        double res2 = AdaptiveIntegrator.Integrate(f2, 0.0, 1.0, 1e-6, 1e-6, out err);
        Console.WriteLine($"∫[0,1] x^(-1/2) dx = {fmt(res2)} \t(exact = {fmt(exact2)}, error = {fmt(res2 - exact2)})");
        Console.WriteLine($"Estimated error = {fmt(err)}");

        // 3) ∫_0^1 √(1 - x^2) dx = π/4 ≈ 0.785398 (quarter-circle area)
        Func<double,double> f3 = x => Math.Sqrt(1 - x*x);
        double exact3 = Math.PI/4.0;
        double res3 = AdaptiveIntegrator.Integrate(f3, 0.0, 1.0, 1e-6, 1e-6, out err);
        Console.WriteLine($"∫[0,1] √(1-x^2) dx = {fmt(res3)} \t(exact = {fmt(exact3)}, error = {fmt(res3 - exact3)})");
        Console.WriteLine($"Estimated error = {fmt(err)}");

        // 4) ∫_0^1 ln(x)/√x dx = -4
        Func<double,double> f4 = x => Math.Log(x)/Math.Sqrt(x);
        double exact4 = -4.0;
        double res4 = AdaptiveIntegrator.Integrate(f4, 0.0, 1.0, 1e-6, 1e-6, out err);
        Console.WriteLine($"∫[0,1] ln(x)/√x dx = {fmt(res4)} \t(exact = {fmt(exact4)}, error = {fmt(res4 - exact4)})");
        Console.WriteLine($"Estimated error = {fmt(err)}");

        Console.WriteLine();
        // Part A: Implementing the error function erf(z) via integral representation
        Console.WriteLine("Computing error function erf(z) via integration:");
        Func<double,double> integrandExp = x => Math.Exp(-x*x);
        // Compute erf(1) using the definition erf(1) = 2/√π * ∫_0^1 e^{-x^2} dx
        double erfExact = 0.84270079294971486934;  // known exact value of erf(1):contentReference[oaicite:19]{index=19}
        double integral0to1 = AdaptiveIntegrator.Integrate(integrandExp, 0.0, 1.0, 1e-6, 1e-6, out _);
        double erf1 = 2/Math.Sqrt(Math.PI) * integral0to1;
        Console.WriteLine($"erf(1) ≈ {fmt(erf1)} (exact = {erfExact:F18}, abs error = {Math.Abs(erf1 - erfExact):E})");

        // Vary the accuracy goal and see how the result converges to the exact erf(1)
        Console.WriteLine("Accuracy vs requested tolerance for erf(1):");
        Console.WriteLine("acc\t\tResult\t\tError_to_exact");
        double[] accValues = {1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6};
        foreach (double acc in accValues) {
            double integralVal = AdaptiveIntegrator.Integrate(integrandExp, 0.0, 1.0, acc, 0, out _);
            double erfVal = 2/Math.Sqrt(Math.PI) * integralVal;
            double errorToExact = erfVal - erfExact;
            Console.WriteLine($"{acc:E1}\t{erfVal:F9}\t{errorToExact:E}");
        }

        Console.WriteLine();
        // Part B: Clenshaw–Curtis transformation for endpoint singularities
        Console.WriteLine("Part B: Clenshaw-Curtis transformation for endpoint singularities.");
        // Compare integrals with vs. without Clenshaw–Curtis:
        Console.WriteLine("Integral ∫[0,1] x^(-1/2) dx (expected 2):");
        // Without transformation (count function calls)
        int calls = 0;
        Func<double,double> f2count = x => { calls++; return 1.0/Math.Sqrt(x); };
        double resultNoTrans = AdaptiveIntegrator.Integrate(f2count, 0.0, 1.0, 1e-6, 1e-6, out err);
        int callsNoTrans = calls;
        double errorNoTrans = err;
        // With Clenshaw–Curtis transformation
        calls = 0;
        Func<double,double> f2count2 = x => { calls++; return 1.0/Math.Sqrt(x); };
        double resultCC = AdaptiveIntegrator.IntegrateClenshawCurtis(f2count2, 0.0, 1.0, 1e-6, 1e-6);
        int callsCC = calls;
        Console.WriteLine($"Without transform: result = {fmt(resultNoTrans)}, calls = {callsNoTrans}");
        Console.WriteLine($"With Clenshaw-Curtis: result = {fmt(resultCC)}, calls = {callsCC}");
        Console.WriteLine($"Estimated error (no transform) = {fmt(errorNoTrans)}");

        Console.WriteLine("Integral ∫[0,1] ln(x)/√x dx (expected -4):");
        // Without transformation
        calls = 0;
        Func<double,double> f4count = x => { calls++; return Math.Log(x)/Math.Sqrt(x); };
        double resultNoTrans2 = AdaptiveIntegrator.Integrate(f4count, 0.0, 1.0, 1e-6, 1e-6, out err);
        int callsNoTrans2 = calls;
        double errorNoTrans2 = err;
        // With Clenshaw–Curtis
        calls = 0;
        Func<double,double> f4count2 = x => { calls++; return Math.Log(x)/Math.Sqrt(x); };
        double resultCC2 = AdaptiveIntegrator.IntegrateClenshawCurtis(f4count2, 0.0, 1.0, 1e-6, 1e-6);
        int callsCC2 = calls;
        Console.WriteLine($"Without transform: result = {fmt(resultNoTrans2)}, calls = {callsNoTrans2}");
        Console.WriteLine($"With Clenshaw-Curtis: result = {fmt(resultCC2)}, calls = {callsCC2}");
        Console.WriteLine($"Estimated error (no transform) = {fmt(errorNoTrans2)}");

        Console.WriteLine();
        // Part C: Infinite limits and error estimation
        Console.WriteLine("Part C: Handling infinite limits and error estimation.");
        // Test some improper integrals:
        Console.WriteLine("Integral ∫[0,∞) e^(-x) dx (expected 1):");
        Func<double,double> f_exp = x => Math.Exp(-x);
        double res_inf = AdaptiveIntegrator.Integrate(f_exp, 0.0, Double.PositiveInfinity, 1e-6, 1e-6, out err);
        Console.WriteLine($"Result = {fmt(res_inf)}, estimated error = {fmt(err)}, actual error = {fmt(res_inf - 1.0)}");

        Console.WriteLine("Integral ∫[0,∞) e^(-x^2) dx (expected √(π)/2):");
        Func<double,double> f_gauss = x => Math.Exp(-x*x);
        double exact_val = Math.Sqrt(Math.PI)/2;
        double res_inf2 = AdaptiveIntegrator.Integrate(f_gauss, 0.0, Double.PositiveInfinity, 1e-6, 1e-6, out err);
        Console.WriteLine($"Result = {fmt(res_inf2)}, estimated error = {fmt(err)}, actual error = {fmt(res_inf2 - exact_val)}");

        Console.WriteLine("Integral ∫(-∞,∞) e^(-x^2) dx (expected √(π)):");
        double exact_full = Math.Sqrt(Math.PI);
        double res_full = AdaptiveIntegrator.Integrate(f_gauss, Double.NegativeInfinity, Double.PositiveInfinity, 1e-6, 1e-6, out err);
        Console.WriteLine($"Result = {fmt(res_full)}, estimated error = {fmt(err)}, actual error = {fmt(res_full - exact_full)}");

        Console.WriteLine();
        Console.WriteLine("Comparing actual vs estimated errors for a challenging integral:");
        // Example: ∫[0,1] ln(x)/√x dx (singular integrand)
        double res_difficult = AdaptiveIntegrator.Integrate(f4, 0.0, 1.0, 1e-6, 1e-6, out err);
        double actual_err = res_difficult - exact4;
        Console.WriteLine($"ln(x)/√x on [0,1]: result = {fmt(res_difficult)}, est.err = {fmt(err)}, actual err = {fmt(actual_err)}");
    }
}
