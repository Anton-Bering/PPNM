// ---------- Hydrogen.cs ----------
using System;
using System.IO;

public static class Hydrogen
{
        // ---------- Hydrogen.cs  (new GroundState) ----------------------------------
        // ---------- Hydrogen.cs  (start of GroundState) -----------------------------
    public static (double E0, int steps) GroundState(
            double rMin, double rMax,
            double acc = 1e-6, double eps = 1e-6)
    {
        const double RMAX_LIMIT = 40.0;     // never integrate beyond this
        const double RMAX_STEP  = 4.0;      // enlarge box in 4‑Bohr chunks

        while (true)                         // keep trying until bracketed
        {
            // --- 1. try to bracket in current box ------------------------------
            bool bracketed = Bracket(rMin, rMax,
                                    out double E_lo, out double E_hi,
                                    out double M_lo, out double M_hi);

            if (bracketed)
            {
                // --- 2. bisect inside the bracket ------------------------------
                int it = 0;
                while (Math.Abs(E_hi - E_lo) > acc && it < 100)
                {
                    double E_mid = 0.5*(E_lo + E_hi);
                    double M_mid = Shoot(E_mid, rMin, rMax);

                    if (Math.Sign(M_lo) != Math.Sign(M_mid))
                    {   E_hi = E_mid;  M_hi = M_mid; }
                    else
                    {   E_lo = E_mid;  M_lo = M_mid; }

                    ++it;
                }
                return (0.5*(E_lo + E_hi), it);
            }

            // --- 3. if not bracketed, enlarge the box --------------------------
            rMax += RMAX_STEP;
            if (rMax > RMAX_LIMIT)
                throw new Exception($"Failed to bracket ground‑state energy even at r_max = {rMax}");
        }
    }

    // Helper that does the energy scan in a given box ----------------------------
    private static bool Bracket(double rMin, double rMax,
                                out double E_lo, out double E_hi,
                                out double M_lo, out double M_hi)
    {
        E_lo = -2.0;                M_lo = Shoot(E_lo, rMin, rMax);
        E_hi = E_lo;                M_hi = M_lo;

        const double E_hi_lim = -1e-3;
        const int    Nscan    = 400;
        double step = (E_hi_lim - E_lo) / Nscan;

        for (int i = 1; i <= Nscan; ++i)
        {
            E_hi += step;
            M_hi  = Shoot(E_hi, rMin, rMax);

            if (Math.Sign(M_lo) != Math.Sign(M_hi))
                return true;        // sign change -> bracketed

            E_lo = E_hi;  M_lo = M_hi;
        }
        return false;               // no sign change in this box
    }




    // ---------- Hydrogen.cs  (new Shoot) ----------------------------------------
    private static double Shoot(double E, double rMin, double rMax,
                                string dumpFile = null, int nSteps = 4000)
    {
        //----------------------------------------------------------------------
        // Integrate u'' = -2/r u' + 2(1/r - E) u   with RK4
        //----------------------------------------------------------------------
        double h  = (rMax - rMin) / nSteps;
        double r  = rMin;
        double u  = r;        // u(r) ≈ r   as r→0
        double up = 1.0;      // derivative

        for (int i = 0; i < nSteps; ++i, r += h)
        {
            (double k1, double l1) = RHS(r,     u,            up,           E);
            (double k2, double l2) = RHS(r+h/2, u+h*k1/2,     up+h*l1/2,    E);
            (double k3, double l3) = RHS(r+h/2, u+h*k2/2,     up+h*l2/2,    E);
            (double k4, double l4) = RHS(r+h,   u+h*k3,       up+h*l3,      E);

            u  += h/6*(k1 + 2*k2 + 2*k3 + k4);
            up += h/6*(l1 + 2*l2 + 2*l3 + l4);

            if (dumpFile != null)                           // store profile
                System.IO.File.AppendAllText(dumpFile, $"{r:F6} {u:F8}\n");
        }

        //------------------------------------------------------------------
        // Shooting function  M(E) =  u'/u + k   with  k = √(-2E)
        //------------------------------------------------------------------
        double k = Math.Sqrt(-2*E);           // E < 0  for bound states
        return up/u + k;
    }


    // Helper: RHS of first‑order system --------------------------------------
    private static (double du, double dup) RHS(double r, double u, double up, double E)
    {
        double du   = up;
        double dup  = -2/r * up + 2*(1/r - E) * u;
        return (du, dup);
    }

    // Produce data files for gnuplot -----------------------------------------
    public static void DumpWaveFunctions(double E, double rMin, double rMax)
    {
        string numFile   = "wave_function.dat";
        string anaFile   = "analytic_wf.dat";
        if (File.Exists(numFile)) File.Delete(numFile);
        if (File.Exists(anaFile)) File.Delete(anaFile);

        int N = 1000;
        double h = (rMax - rMin) / N;

        // Numerical – reuse Shoot but record
        Shoot(E, rMin, rMax, numFile, N*4);

        // Analytic –  u_1s(r) = 2·r·exp(‑r)
        for (int i = 0; i <= N; ++i)
        {
            double r = rMin + i * h;
            double u = 2 * r * Math.Exp(-r);
            File.AppendAllText(anaFile, $"{r:F6} {u:F8}\n");
        }
    }
}
// ---------------------------------------------------------------------------
