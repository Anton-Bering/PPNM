// ---------- Main.cs ----------
using System;
using static System.Math;
using System.IO;
using static Integration.Adaptive;
using static Integration.ClenshawCurtis;

namespace Integration {
  class MainClass {

    static void Main() {

      Directory.CreateDirectory(".");
      using (var writer = new StreamWriter("Out.txt")) {

        /* ---------------- TASK A ---------------- */
        writer.WriteLine("------------ TASK A: Recursive open 4‑point adaptive integrator ------------\n");
        writer.WriteLine("------ Test the implementation on integrals ------\n");

        TestIntegral(writer,"∫₀¹ √(x) dx",        x => Sqrt(x),        0,1, 2.0/3.0);
        TestIntegral(writer,"∫₀¹ 1/√(x) dx",      x => 1/Sqrt(x),      0,1, 2.0);
        TestIntegral(writer,"∫₀¹ √(1‑x²) dx",     x => Sqrt(1-x*x),    0,1, PI/4);
        TestIntegral(writer,"∫₀¹ ln(x)/√(x) dx",  x => Log(x)/Sqrt(x), 0,1,-4.0);

        /* ---------- plot af selve fejlfunktionen ------------------------ */
        writer.WriteLine("------ Make a plot of the error function and compare with the tabulated values ------\n");
        GenErfTable("error_function.txt", -3, 3, 0.05);
        writer.WriteLine("Data is written to error_function.txt");
        writer.WriteLine("Plot is in error_function.svg\n");

        /* ---------- konvergens‑test for erf(1) -------------------------- */
        writer.WriteLine("------ Calculate erf(1)  ------\n");
        double exactErf1 = 0.84270079294971486934;
        double[] accList = {1e-1,1e-2,1e-3,1e-4,1e-5,1e-6};
        using(var ef = new StreamWriter("erf1.txt")){
          ef.WriteLine("# acc   abs(error)");
          foreach(double acc in accList){
            double val  = Erf(1,acc,0);          // eps = 0
            double diff = Abs(val-exactErf1);
            ef.WriteLine($"{acc:E} {diff:E}");
            writer.WriteLine($"acc = {acc:E}   erf(1) = {val:R}   |Δ| = {diff:E}");
          }
        }
        writer.WriteLine("\n------ Plot the difference between your result ------");
        writer.WriteLine("------ and the exact result as function of acc ------\n");
        
        writer.WriteLine("\nThe plot is written to erf1.svg\n");

        /* ---------------- TASK B ---------------- */
        writer.WriteLine("------------ TASK B: Variable transformation quadratures ------------\n");
        Func<double,double> f1 = z=>1/Sqrt(z);
        Func<double,double> f2 = z=>Log(z)/Sqrt(z);

        Compare(writer,"∫₀¹ 1/√(x) dx",f1,0,1,2.0);
        Compare(writer,"∫₀¹ ln(x)/√(x) dx",f2,0,1,-4.0);

        writer.WriteLine("\n--- Infinite‑limit test: ∫₀^∞ e^(‑x) dx = 1 ---");
        Func<double,double> fInf = z=>Exp(-z);
        var resInf = IntegrateInfinite(fInf,0,double.PositiveInfinity);
        writer.WriteLine($"result = {resInf.val}, calls = {resInf.calls},  exact = 1,  |Δ|={Abs(resInf.val-1):E}\n");

        /* ---------------- TASK C ---------------- */
        writer.WriteLine("------------ TASK C: Error estimate ------------\n");
        writer.WriteLine("Integral                    value        est.err      act.err");

        PrintErr(writer,"∫₀¹ sin(1/x) dx",
                 x=>Sin(1/x),0,1,0.504067061906928);

        // HERHER: Tilføj porintes.
      }
    }

    /* ---------- helper sub‑routines ---------- */

    static void TestIntegral(StreamWriter w,string label,Func<double,double> f,
                             double a,double b,double exact){
      var r  = Integrate(f,a,b);
      bool ok = Abs(r.val-exact) <= 1e-6;

      w.WriteLine($"--- {label} ---\n");
      w.WriteLine($"Exact value: {exact}");
      w.WriteLine($"Value from my implementation: {r.val}");
      w.WriteLine($"Estimated error = {r.err:E}");
      w.WriteLine($"Calls = {r.calls}\n");

      w.WriteLine($"Test: Is the value within the accuracy goals?");
      w.WriteLine($"Result: {(ok? "Yes":"No")}\n");
    }

    /* tabel til egen fejlfunktion */
    static void GenErfTable(string file,double from,double to,double step){
      using (var sw = new StreamWriter(file)) {
        for(double x=from;x<=to+1e-12;x+=step)
          sw.WriteLine($"{x} {Erf(x,1e-6,0)}");
      }
    }

    /* selve erf‑rutinen */
    static double Erf(double z,double acc,double eps){
      if(z<0) return -Erf(-z,acc,eps);
      if(z<=1) return 2/Sqrt(PI)*Integrate(t=>Exp(-t*t),0,z,acc,eps).val;
      return 1-2/Sqrt(PI)*Integrate(t=>{
        double x = z+(1-t)/t;
        return Exp(-x*x)/(t*t);
      },0,1,acc,eps).val;
    }

    /* almindelig vs. Clenshaw‑Curtis sammenligning */
    static void Compare(StreamWriter w,string name,Func<double,double> f,double a,double b,double exact){
      var plain = Integrate(f,a,b);
      var cc    = IntegrateCC(f,a,b);
      w.WriteLine($"--- {name} ---");
      w.WriteLine($"plain  : {plain.val}  calls = {plain.calls}");
      w.WriteLine($"C‑Curt : {cc.val}  calls = {cc.calls}");
      w.WriteLine($"exact  : {exact}\n");
    }

    /* fejl‑estimat – kvalitetscheck */
    static void PrintErr(StreamWriter w,string label,Func<double,double> f,double a,double b,double exact){
      var r = Integrate(f,a,b,1e-6,1e-6);
      w.WriteLine($"{label,-26} {r.val,12:g6}  {r.err:E2}  {Abs(r.val-exact):E2}");
    }
  }
}
