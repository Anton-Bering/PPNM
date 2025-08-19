using System;
using static MatrixHelpers;   // only used for DEBUG checks

namespace PPNM.Minimization {

public static class QuasiNewton {

    // Machine epsilon for double. Used to scale finite-difference steps. [p. 101 (PDF 109)]
    const double machine_epsilon = 2.220446049250313e-16; // = 2^-52

    // ------------ Approximation for the gradient (central differences) -------
    static vector Grad(Func<vector,double> f, vector x){
        int n = x.Size;
        var g = new vector(n);
        for(int k=0;k<n;k++){
            double x_k  = x[k];
            double δx_k = Math.Sqrt(machine_epsilon) * (1.0 + Math.Abs(x_k));
            x[k] = x_k + δx_k; double f_plus  = f(x);
            x[k] = x_k - δx_k; double f_minus = f(x);
            g[k] = (f_plus - f_minus) / (2 * δx_k);
            x[k] = x_k;
        }
        return g;
    }

    // ------------ Armijo backtracking line search [p. 100 (PDF 108)] --------
    static double LineSearchArmijo(
        Func<vector,double> f, 
        vector x, 
        double fx, 
        vector g, 
        ref vector Δx,
        double α,
        double λ_minimal)
    {
        double gT__Δx = vector.Dot(g, Δx);
        if (gT__Δx >= 0){
            Δx     = -g;
            gT__Δx = -vector.Dot(g, g);
        }

        double λ = 1.0;
        while (λ >= λ_minimal && f(x + λ*Δx) > fx + α*λ*gT__Δx)
            λ *= 0.5;
        return λ;
    }

    // ------------ Result bundle for reporting --------------------------------
    public struct Result {
        public vector x_min;     
        public double f_min;     
        public double gradNorm;  
        public double last_step; 
        public int iterations;   
        public int resets;       
        public int fevals;       
    }

    public static vector broyden(Func<vector,double> f, vector x, double acc) => MinimizeReport(f,x,acc,useSymmetrized:false).x_min;
    public static vector broyden_sym(Func<vector,double> f, vector x, double acc) => MinimizeReport(f,x,acc,useSymmetrized:true).x_min;

    // ------------ Main driver (one minimization run) -------------------------
    public static Result MinimizeReport(
        Func<vector,double> f, 
        vector x0, 
        double acc, 
        bool useSymmetrized,
        int maxIterations = 10000,
        double α = 1e-4,
        double λ_minimal = 1.0/1024.0)
    {
        int fe = 0;
        Func<vector,double> fcount = z => { fe++; return f(z); };

        // initial state
        int n = x0.Size;
        vector x = x0.Copy();
        double fx = fcount(x);
        vector g  = Grad(fcount, x);

        matrix B = matrix.Eye(n);

        int    iter   = 0;
        int    resets = 0;
        double last_λ = 0;

        for(; iter<maxIterations; iter++){
            if (g.NormInf() <= acc*(1.0 + Math.Abs(fx))) break;

            vector Δx = -(B * g);
            double λ = LineSearchArmijo(fcount, x, fx, g, ref Δx, α, λ_minimal);
            last_λ = λ;

            vector s      = λ*Δx;
            vector x_new  = x + s;
            double fx_new = fcount(x_new);
            vector g_new  = Grad(fcount, x_new);
            vector y      = g_new - g;

            if (λ < λ_minimal){
                x = x_new; fx = fx_new; g = g_new;
                B = matrix.Eye(n);
                resets++;
                #if DEBUG
                CheckIdentityMatrix(B, "B reset (λ < λ_minimal)", tol:1e-12, throwOnFail:false);
                #endif
                continue;
            }

            double sy  = vector.Dot(s,y);
            double tol = 1e-12 * (1.0 + s.Norm()*y.Norm());
            if (Math.Abs(sy) <= tol){
                x = x_new; fx = fx_new; g = g_new;
                B = matrix.Eye(n);
                resets++;
                #if DEBUG
                CheckIdentityMatrix(B, "B reset (curvature guard)", tol:1e-12, throwOnFail:false);
                #endif
                continue;
            }

            BroydenUpdates.Update(ref B, s, y, useSymmetrized);

            // accept move
            x = x_new; fx = fx_new; g = g_new;
        }

        return new Result{
            x_min     = x,
            f_min     = fx,
            gradNorm  = g.Norm(),   
            last_step = last_λ,
            iterations= iter,
            resets    = resets,
            fevals    = fe
        };
    }
}

// -------------------- Update rules ('Broyden's update' and 'symmetrized Broyden's update') --------------------------
public static class BroydenUpdates 
{
    public static void Update(
        ref matrix B, 
        vector s, 
        vector y, 
        bool useSymmetrized, // =false ==> 'Broyden's update' // true ==> 'symmetrized Broyden's update'
        double ε=1e-12)
    {
        double sy = vector.Dot(s,y);

        double tol = ε * (s.Norm() * y.Norm() + 1.0);
        if (Math.Abs(sy) <= tol) return;        
        
        var u = s - (B * y);

        if (useSymmetrized) // symmetrized Broyden's update
        {
            double γ = vector.Dot(u,y) / (2.0 * sy);
            var a = (u - γ*s) / sy;
            B += matrix.Outer(a,s) + matrix.Outer(s,a);
        }
        else // Broyden's update
        {
            B += matrix.Outer(u, s) / sy;            
        }
    }
}


}

