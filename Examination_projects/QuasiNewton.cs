using System;

namespace PPNM.Minimization {

public static class QuasiNewton {

    // ---------- små lokale hjælpere ----------
    static double[,] Copy(double[,] A){
        int n=A.GetLength(0), m=A.GetLength(1);
        var B=new double[n,m]; Array.Copy(A,B,A.Length); return B;
    }
    static void AddInPlace(double[,] A, double[,] B){
        int n=A.GetLength(0), m=A.GetLength(1);
        for(int i=0;i<n;i++) for(int j=0;j<m;j++) A[i,j]+=B[i,j];
    }
    static vector MatVec(double[,] A, vector v){ return VectorAndMatrix.Multiply(A,v); }
    static double DotVec(vector a, vector b){ double s=0; for(int i=0;i<a.size;i++) s+=a[i]*b[i]; return s; }
    static double NormVec(vector a){ return Math.Sqrt(DotVec(a,a)); }
    static double[] VecToArray(vector v){ var a=new double[v.size]; for(int i=0;i<v.size;i++) a[i]=v[i]; return a; }
    static vector ArrayToVec(double[] a){ var v=new vector(a.Length); for(int i=0;i<a.Length;i++) v[i]=a[i]; return v; }

    // ---------- numerisk gradient (central differens) ----------
    static vector Grad(Func<vector,double> f, vector x){
        int n=x.size; var g=new vector(n);
        for(int i=0;i<n;i++){
            double xi=x[i];
            double h = Math.Sqrt(2.220446049250313e-16)*(Math.Abs(xi)+1.0);
            x[i]=xi+h; double fph=f(x);
            x[i]=xi-h; double fmh=f(x);
            g[i]=(fph-fmh)/(2*h);
            x[i]=xi;
        }
        return g;
    }

    // ---------- Armijo backtracking ----------
    static double LineSearch(Func<vector,double> f, vector x, double fx, vector g, vector p){
        double c1=1e-4, tau=0.5, t=1.0;
        double gTp = DotVec(g,p);
        if(gTp>0){ for(int i=0;i<p.size;i++) p[i]=-g[i]; gTp = -NormVec(g)*NormVec(g); }
        var xtrial=x.copy();
        for(int it=0; it<50; it++){
            for(int i=0;i<x.size;i++) xtrial[i]=x[i]+t*p[i];
            if(f(xtrial) <= fx + c1*t*gTp) break;
            t*=tau; if(t<1e-12) break;
        }
        return t;
    }

    // ---------- Broyden/SR1 opdateringer på B (double[,]) ----------
    // Broyden: B <- B + ((y - B s) s^T)/(s^T s)
    static void BroydenUpdate(double[,] B, vector s, vector y){
        var Bs = MatVec(B, s);
        var u  = y - Bs;
        double denom = DotVec(s,s); if(Math.Abs(denom) < 1e-16) return;
        var rank1 = VectorAndMatrix.Outer(u, s);                // u s^T
        AddInPlace(B, VectorAndMatrix.Scale(rank1, 1.0/denom)); // B += (u s^T)/(s^T s)
    }
    // SR1: B <- B + (r r^T)/(r^T s), r = y - B s
    static void SR1Update(double[,] B, vector s, vector y){
        var Bs = MatVec(B, s);
        var r  = y - Bs;
        double denom=0; for(int i=0;i<s.size;i++) denom += r[i]*s[i];
        if(Math.Abs(denom) <= 1e-12) return;
        var rrT = VectorAndMatrix.Outer(r, r);
        AddInPlace(B, VectorAndMatrix.Scale(rrT, 1.0/denom));
    }

    // ---------- løs B p = -g med din QR ----------
    static vector SolveByQR(double[,] B, vector g){
        var qr  = new QR(B);           // din QR-klasse
        var rhs = VecToArray(g);
        for(int i=0;i<rhs.Length;i++) rhs[i]*=-1;   // -g
        var x   = qr.solve(rhs);      // løser B x = -g
        return ArrayToVec(x);
    }

    // ---------- public API ----------
    public static vector broyden(Func<vector,double> f, vector start, double acc){
        return Minimize(f, start, acc, useSR1:false);
    }
    public static vector broyden_sym(Func<vector,double> f, vector start, double acc){
        return Minimize(f, start, acc, useSR1:true);
    }

    static vector Minimize(Func<vector,double> f, vector x0, double acc, bool useSR1){
        int n=x0.size;
        var x  = x0.copy();
        var B  = VectorAndMatrix.IdentityMatrix(n); // double[,]
        double fx = f(x);
        var g  = Grad(f,x);

        int maxit=10000;
        for(int it=0; it<maxit; it++){
            if(NormVec(g) <= acc) break;

            vector p;
            try{ p = SolveByQR(B, g); }
            catch{
                B = VectorAndMatrix.IdentityMatrix(n);
                p = SolveByQR(B, g);
            }
            if(DotVec(p,g) > 0) for(int i=0;i<p.size;i++) p[i] = -g[i];

            double t = LineSearch(f,x,fx,g,p);
            var s    = p.copy(); for(int i=0;i<s.size;i++) s[i]*=t; // s = t*p
            var xnew = x + s;
            double fxnew = f(xnew);
            var gnew = Grad(f,xnew);

            var y = gnew - g;
            if(useSR1) SR1Update(B, s, y);
            else       BroydenUpdate(B, s, y);

            x=xnew; fx=fxnew; g=gnew;

            if(NormVec(s)<1e-14) B = VectorAndMatrix.IdentityMatrix(n);
        }
        return x;
    }
}

}
