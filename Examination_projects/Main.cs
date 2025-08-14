using System;
using static PPNM.Minimization.QuasiNewton;

// hjælper: lav vector fra params (din vector har ikke en sådan ctor)
static class V {
    public static vector vec(params double[] a){
        var v = new vector(a.Length);
        for(int i=0;i<a.Length;i++) v[i]=a[i];
        return v;
    }
}

class Program {
    static void RunCase(string name, Func<vector,double> f, vector start, double acc, bool useSR1=false){
        vector xmin = useSR1 ? broyden_sym(f,start,acc) : broyden(f,start,acc);
        double fmin = f(xmin);
        Console.WriteLine($"---- {name} {(useSR1?"(SR1)":"(Broyden)")}");
        Console.WriteLine($"start  = {start}");
        Console.WriteLine($"xmin   = {xmin}");
        Console.WriteLine($"f(xmin)= {fmin:g6}\n");
    }

    static void Main(){
        double acc = 1e-6;

        RunCase("Rosenbrock", Problems.Rosenbrock, V.vec(-1.2, 1.0), acc, useSR1:false);
        RunCase("Rosenbrock", Problems.Rosenbrock, V.vec(-1.2, 1.0), acc, useSR1:true);

        RunCase("Himmelblau", Problems.Himmelblau, V.vec(0.0, 0.0), acc, useSR1:false);
        RunCase("Himmelblau", Problems.Himmelblau, V.vec(0.0, 0.0), acc, useSR1:true);

        var f6 = Problems.RotatedQuadratic(6);
        RunCase("RotatedQuadratic6D", f6, V.vec(1, -1, 2, -2, 1.5, 0.5), acc, useSR1:false);
        RunCase("RotatedQuadratic6D", f6, V.vec(1, -1, 2, -2, 1.5, 0.5), acc, useSR1:true);

        Console.WriteLine("Done.");
    }
}
