using System;
using System.Collections.Generic;
using static System.Math;

class QuasiNewton
{
    const double EPS = 1e-6;         // threshold in denominator tests
    const double LAMBDA_MIN = 1.0/1024; // lower bound in back‑tracking

    /* numerical gradient – forward finite difference */
    static vector Gradient(Func<vector,double> f, vector x)
    {
        double fx = f(x);
        var g = new vector(x.size);
        for(int i=0;i<x.size;i++)
        {
            double dx = (Abs(x[i])+1)*Pow(2,-26);
            x[i]+=dx;
            g[i] = (f(x)-fx)/dx;
            x[i]-=dx;
        }
        return g;
    }

    /* rank‑1 outer product helper */
    static void Rank1Add(matrix B, vector u, vector v, double factor=1.0)
    {
        for(int i=0;i<B.size1;i++)
            for(int j=0;j<B.size2;j++)
                B[i,j] += factor*u[i]*v[j];
    }

    public static vector Broyden(Func<vector,double> f, vector x, double acc=1e-3)
    {
        int n = x.size;
        var B = matrix.Identity(n);          // inverse Hessian approximation
        var g = Gradient(f,x);
        var pathX = new List<vector>();
        pathX.Add(x.copy());
        int steps = 0;

        while(g.norm() > acc && steps<5000)
        {
            steps++;
            vector dx = (-1)*(B * g);            // Newton step using current B

            // back‑tracking line search
            double lambda = 1;
            double fx = f(x);
            vector x_new;
            double fx_new;
            while(true)
            {
                x_new = x + lambda*dx;
                fx_new = f(x_new);
                if(fx_new < fx) break;
                lambda/=2;
                if(lambda < LAMBDA_MIN)
                {
                    // accept small step and reset B
                    x_new = x + lambda*dx;
                    B = matrix.Identity(n);
                    break;
                }
            }

            // prepare update vectors
            vector s = x_new - x;
            vector g_new = Gradient(f,x_new);
            vector y = g_new - g;
            double sty = vector.Dot(s,y);
            if(Abs(sty) > EPS)
            {
                // δB = (s-By) s^T / (s^T y)
                vector Bs = B*y;
                vector u = s - Bs;
                Rank1Add(B,u,s,1.0/sty);
            }
            x = x_new;
            g = g_new;
            pathX.Add(x.copy());
        }

        // write optimisation path for plotting
        using(var file = new System.IO.StreamWriter("path_rosenbrock.dat", true))
        {
            foreach(var p in pathX) file.WriteLine($"{p[0]} {p[1]}");
        }

        return x;
    }

    /* Symmetric Broyden update variant */
    public static vector SymBroyden(Func<vector,double> f, vector x, double acc=1e-3)
    {
        int n = x.size;
        var B = matrix.Identity(n);
        var g = Gradient(f,x);
        int steps = 0;
        while(g.norm()>acc && steps<5000)
        {
            steps++;
            vector dx = (-1)*(B*g);
            double lambda=1;
            double fx = f(x);
            vector x_new;
            double fx_new;
            while(true)
            {
                x_new = x + lambda*dx;
                fx_new = f(x_new);
                if(fx_new < fx) break;
                lambda/=2;
                if(lambda < LAMBDA_MIN)
                {
                    x_new = x + lambda*dx;
                    B = matrix.Identity(n);
                    break;
                }
            }
            vector s = x_new - x;
            vector g_new = Gradient(f,x_new);
            vector y = g_new - g;
            double sty = vector.Dot(s,y);
            if(Abs(sty) > EPS)
            {
                vector u = s - B*y;
                double gamma = vector.Dot(u,y)/(2*sty);
                // a = (u - γ s)/(s^T y)
                vector a = (u - s*gamma)*(1.0/sty);
                // δB = a s^T + s a^T
                Rank1Add(B,a,s,1.0);
                Rank1Add(B,s,a,1.0);
            }
            x = x_new;
            g = g_new;
        }
        return x;
    }

    /* ---------- demo ------------ */
    static double Rosenbrock(vector v)
    {
        double x=v[0], y=v[1];
        return Pow(1-x,2)+100*Pow(y- x*x,2);
    }

    static double Himmelblau(vector v)
    {
        double x=v[0], y=v[1];
        return Pow(x*x+y-11,2)+Pow(x+y*y-7,2);
    }

    static void Main()
    {
        // Rosenbrock test
        var start = new vector(-1.2, 1.0);
        var min = Broyden(Rosenbrock, start.copy(), 1e-5);
        Console.WriteLine($"Rosenbrock minimum found at {min.ToPretty()}  f={Rosenbrock(min):g}");

        // Himmelblau test
        var hstart = new vector(0.0, 0.0);
        var hmin = Broyden(Himmelblau, hstart.copy(), 1e-5);
        Console.WriteLine($"Himmelblau minimum at {hmin.ToPretty()}  f={Himmelblau(hmin):g}");

        // compare symmetric update
        var smin = SymBroyden(Rosenbrock, start.copy(),1e-5);
        Console.WriteLine($"Sym‑Broyden (Rosenbrock) → {smin.ToPretty()}  f={Rosenbrock(smin):g}");
    }
}
