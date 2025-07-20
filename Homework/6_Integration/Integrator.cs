using System;
using static System.Math;

namespace Integration {

  public static class Adaptive {

    public struct Result { public double val, err; public int calls; }

    /* ------ Til at forhindre uendelig rekursion: ------*/
    public static Result Integrate(Func<double,double> f, double a, double b,
                                   double acc = 1e-6, double eps = 1e-6,
                                   double f2  = double.NaN, double f3 = double.NaN,
                                   int depth  = 0)
    {
      /* ------ Check: for små intervaler eller for dyb rekursion: ------*/
      if (depth > 1000000 || a == b ) {
        Console.WriteLine($"Problem opstår i intergrator.cs. depth={depth}, a={a}, b={b}");
        return new Result { val = 0.0, err = 0.0, calls = 0 };
      }

      /* ------ første funktions‑evalueringer (genbrug af punkter) ------ */
      double h = b - a;
      if (double.IsNaN(f2)) { f2 = f(a + 2 * h / 6); f3 = f(a + 4 * h / 6); }
      double f1 = f(a +     h / 6);
      double f4 = f(a + 5 * h / 6);

      /* ------ håndter eventuelle NaN eller Inf fra integranden ------ */
      if (IsBad(f1) || IsBad(f2) || IsBad(f3) || IsBad(f4)) {
        return new Result { val = 0.0, err = double.PositiveInfinity, calls = 4 };
      }

      /* ------ høj‑ & lav‑ordens kvadraturer ------ */
      double Q = (2 * f1 + f2 + f3 + 2 * f4) / 6 * h;   // høj
      double q = (    f1 + f2 + f3 +     f4) / 4 * h;   // lav
      double err = Abs(Q - q);

      double tol = acc + eps * Abs(Q);
      if (err <= tol) {
        return new Result { val = Q, err = err, calls = 4 };
      }

      /* --- ELSE: del intervallet i to og rekursér --------------------- */
      double acc_half = acc / Sqrt(2.0);
      double mid = (a + b) / 2;

      var left  = Integrate(f, a,  mid, acc_half, eps, f1, f2, depth + 1);
      var right = Integrate(f, mid, b,  acc_half, eps, f3, f4, depth + 1);

      return new Result {
        val   = left.val + right.val,
        err   = Sqrt(left.err * left.err + right.err * right.err),
        calls = left.calls + right.calls + 2     
      };
    }

    /* hjælpe‑funktion: */
    static bool IsBad(double x) => double.IsNaN(x) || double.IsInfinity(x);
  }
}
