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
    static void PrintBlock(string title, Func<vector,double> f, vector start, double acc){
        var r1 = QuasiNewton.MinimizeReport(f, start, acc, useSymmetrized:false);
        int fe1 = r1.fevals;

        var r2 = QuasiNewton.MinimizeReport(f, start, acc, useSymmetrized:true);
        int fe2 = r2.fevals;

        Console.WriteLine($"------------ {title} (acc={acc:g}) ------------");
        Console.WriteLine($"start = {start}");
        Console.WriteLine($"method         xmin                              f(xmin)       ||grad||     iters  resets  last_λ   fevals");
        Console.WriteLine($"Broyden     {r1.x_min,-30} {r1.f_min,14:G6} {r1.gradNorm,12:G3} {r1.iterations,7} {r1.resets,7} {r1.last_step,7:G3} {fe1,8}");
        Console.WriteLine($"Sym-Broyden {r2.x_min,-30} {r2.f_min,14:G6} {r2.gradNorm,12:G3} {r2.iterations,7} {r2.resets,7} {r2.last_step,7:G3} {fe2,8}");

        double spIter = (r2.iterations>0) ? (double)r1.iterations/Math.Max(1,r2.iterations) : double.PositiveInfinity;
        double spEval = (fe2>0)           ? (double)fe1/fe2                                 : double.PositiveInfinity;
        Console.WriteLine($"speedup (iters): {spIter:G3},  speedup (fevals): {spEval:G3}");
        Console.WriteLine();
    }

    static void Main(){
        double acc = 1e-6;

        PrintBlock("Rosenbrock", Problems.Rosenbrock, V.vec(-1.2, 1.0), acc);
        PrintBlock("Himmelblau", Problems.Himmelblau, V.vec( 0.0, 0.0), acc);

        var f6 = Problems.RotatedQuadratic(6);
        PrintBlock("RotatedQuadratic6D", f6, V.vec(1, -1, 2, -2, 1.5, 0.5), acc);

        Console.WriteLine("Done.");
    }
}
