using System;
using PPNM.Minimization;

static class V {
    public static vector vec(params double[] a){
        var v = new vector(a.Length);
        for(int i=0;i<a.Length;i++) v[i]=a[i];
        return v;
    }
}

class Program {
    static void PrintBlock(string title, Func<vector,double> f, vector start, double acc)
    {
        var methods = new[]
        {
            new { Name = "Broyden update", UseSym = false },
            new { Name = "Symmetrized Broyden update", UseSym = true  }
        };

        var results = new QuasiNewton.Result[methods.Length];
        for (int i = 0; i < methods.Length; i++)
            results[i] = QuasiNewton.MinimizeReport(f, start, acc, useSymmetrized: methods[i].UseSym);

        Console.WriteLine($"------------ Function: {title} (acc={acc:g}) ------------\n");
        Console.WriteLine($"Start point(s): {start}\n");

        for (int i = 0; i < methods.Length; i++)
        {
            var r = results[i];
            Console.WriteLine($"--- Results for {methods[i].Name} ---");
            Console.WriteLine($"Estimated minimizer (x*):   {r.x_min}");
            Console.WriteLine($"Objective at x*:            {r.f_min}");
            Console.WriteLine($"Gradient norm at x*:        {r.gradNorm}");
            Console.WriteLine($"Outer iterations:           {r.iterations}");
            Console.WriteLine($"B resets:                   {r.resets}");
            Console.WriteLine($"Last accepted step (λ):     {r.last_step}");
            Console.WriteLine($"Function evaluations:       {r.fevals}");

            Console.WriteLine();
        }

        // Broyden vs Sym-Broyden
        Console.WriteLine($"--- Broyden vs Sym-Broyden ---");
        Console.WriteLine($"speedup ≡ Broyden / Sym-Broyden, ( >1 mean Sym-Broyden is more efficient)");
        var rb = results[0];
        var rs = results[1];
        double spIter = (rs.iterations > 0) ? (double)rb.iterations / Math.Max(1, rs.iterations) : double.PositiveInfinity;
        double spEval = (rs.fevals     > 0) ? (double)rb.fevals     / rs.fevals                  : double.PositiveInfinity;
        Console.WriteLine($"Iterations speedup: {spIter}");
        Console.WriteLine($"function evaluations speedup: {spEval}");
    }

    static void Main(){
        double acc = 1e-6;

        PrintBlock("Quadratic", Problems.Quadratic, V.vec(1.0, 1.0), acc);
        PrintBlock("Rosenbrock",  Problems.Rosenbrock,  V.vec(-1.2, 1.0), acc);
        PrintBlock("Himmelblau",  Problems.Himmelblau,  V.vec( 0.0, 0.0), acc);

        var f6 = Problems.RotatedQuadratic(6);
        PrintBlock("RotatedQuadratic6D", f6, V.vec(1, -1, 2, -2, 1.5, 0.5), acc);

        Console.WriteLine("Done.");
    }
}
