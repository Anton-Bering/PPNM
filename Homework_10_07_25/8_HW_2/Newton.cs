// ---------- Newton.cs ----------
using System;

public static class Newton
{
    // ---------- Part A : Newton iteration with numerical Jacobian & back‑tracking ------
    public static Vec Solve(Func<Vec, Vec> F, Vec x0,
                            double acc = 1e-6, double eps = 1e-6,
                            int maxSteps = 999,
                            bool quadraticLineSearch = false)
    {
        int n = x0.Size;
        Vec x = x0.Copy();
        Vec fx;

        for (int iter = 0; iter < maxSteps; ++iter)
        {
            fx = F(x);
            if (fx.Norm() < acc) break;

            Mat J = Jacobian(F, x, eps);
            Vec dx = J.Solve(-1 * fx);

            // Either back‑tracking or quadratic interpolation
            double t = quadraticLineSearch
                       ? QuadLineSearch(F, x, fx, dx)
                       : BackTrack(F, x, fx, dx);

            x += t * dx;
        }
        return x;
    }

    // Numerical Jacobian (forward difference) -----------------------------------------
    private static Mat Jacobian(Func<Vec, Vec> F, Vec x, double eps)
    {
        int n = x.Size;
        Vec fx = F(x);
        var J  = new Mat(n, n);

        for (int i = 0; i < n; ++i)
        {
            Vec xt = x.Copy();
            double dx = eps * Math.Max(1, Math.Abs(x[i]));
            xt[i] += dx;
            Vec dF = F(xt) - fx;
            for (int j = 0; j < n; ++j)
                J[j, i] = dF[j] / dx;
        }
        return J;
    }

    // ---------------- Back‑tracking line‑search ------------------------------
    private static double BackTrack(Func<Vec, Vec> F, Vec x, Vec fx, Vec dx,
                                    double alpha = 1e-4, double beta = 0.5)
    {
        double t = 1.0;
        double fxNorm = fx.Norm();
        while ((F(x + t * dx)).Norm() > (1 - alpha * t) * fxNorm)
            t *= beta;
        return t;
    }

    // ---------- Part C : quadratic interpolation line‑search ------------------
    private static double QuadLineSearch(Func<Vec, Vec> F, Vec x, Vec fx, Vec dx)
    {
        double t = 1.0;
        double fxNorm = fx.Norm();
        double tPrev = 0, fPrev = fxNorm;
        for (int k = 0; k < 10; ++k)
        {
            Vec xt  = x + t * dx;
            double ft = F(xt).Norm();
            if (ft < (1 - 1e-4 * t) * fxNorm) return t;

            // quadratic fit through (0,fxNorm), (tPrev,fPrev), (t,ft)
            double denom = (t * tPrev * (t - tPrev));
            if (Math.Abs(denom) < 1e-12) { t *= 0.5; continue; }
            double a = ( (ft - fxNorm) / (t * t) - (fPrev - fxNorm) / (tPrev * tPrev) ) / (t - tPrev);
            double b = (fPrev - fxNorm) / (tPrev * tPrev) - a * tPrev;
            double tOpt = -b / (2 * a);
            // tOpt = Math.Clamp(tOpt, 0.1 * t, 0.9 * t);  // stay inside
            tOpt = Math.Max(0.1 * t, Math.Min(tOpt, 0.9 * t));

            tPrev = t;  fPrev = ft;  t = tOpt;
        }
        return t;
    }
}
// ---------------------------------------------------------------------------
