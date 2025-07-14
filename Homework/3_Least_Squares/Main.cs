/* ----------------------------------------------------------------------
 *  Homework ‑ Ordinary Least‑Squares Fit
 *  Main.cs compatible with Mono mcs  (2025‑06‑19)
 * -------------------------------------------------------------------- */
using System;
using System.Globalization;
using System.IO;

class Program
{
    const string OutPath   = "Out.txt";
    const string RawFile   = "Rutherford_and_Soddys_ThX.txt";
    const string LogFile   = "Rutherford_and_Soddys_ThX_log.txt";
    const string CurvesTxt = "bestfit_curves.txt";
    const string DataPlot  = "Rutherford_and_Soddys_ThX.svg";
    const string CurvesSvg = "best_fit_with_changed_coefficients.svg";

    const double Tmodern = 3.6319;   // modern half‑life (days)
    const double TOL     = 1e-12;

    static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (!File.Exists(RawFile)) BuildOriginalDataFile();

        /* ------------- 1.  load and log‑convert data  ------------- */
        Vector t  = Utils.ReadColumn(RawFile, 0);
        Vector y  = Utils.ReadColumn(RawFile, 1);
        Vector dy = Utils.ReadColumn(RawFile, 2);

        Vector lnY  = new Vector(y.Size);
        Vector dlnY = new Vector(y.Size);
        for (int i = 0; i < y.Size; i++)
        {
            lnY[i]  = Math.Log(y[i]);
            dlnY[i] = dy[i] / y[i];
        }
        using (var swLog = new StreamWriter(LogFile, false))
            for (int i = 0; i < t.Size; i++)
                swLog.WriteLine($"{t[i]} {lnY[i]} {dlnY[i]}");

        /* ------------- 2.  least‑squares fit  lnA − λ t ------------- */
        var fs = new Func<double,double>[] { _ => 1.0, z => -z };
        var (c , Cov) = LSFit.Fit(fs, t, lnY, dlnY);      // :contentReference[oaicite:0]{index=0}
        double lnA     = c[0],             lambda   = c[1];
        double dlnA    = Math.Sqrt(Cov[0,0]),
               dlambda = Math.Sqrt(Cov[1,1]);

        double T12  = Math.Log(2)/lambda;
        double dT12 = T12 * dlambda / lambda;
        bool   within = Math.Abs(T12 - Tmodern) < dT12;

        /* ------------- 3.  sample curves for Task C  ------------- */
        Func<double,double> fit    = z => lnA            - lambda            * z;
        Func<double,double> fitLow = z => (lnA-dlnA)     - (lambda+dlambda) * z;
        Func<double,double> fitHi  = z => (lnA+dlnA)     - (lambda-dlambda) * z;

        using (var bf = new StreamWriter(CurvesTxt, false))
            for (double z = 0; z <= 15; z += 0.1)
                bf.WriteLine($"{z} {fit(z)} {fitLow(z)} {fitHi(z)}");

        /* ------------- 4.  build Out.txt  ------------- */
        // ---- FIX for mcs: classic using‑statement ----
        using (var outw = new StreamWriter(OutPath, false))         // was “using var”
        {                                                           // ‑‑‑ FIX for mcs
            /* ========== TASK A summary ========== */
            outw.WriteLine("------------ TASK A ------------\n");
            outw.WriteLine("--- Fit the data from Rutherford and Soddy with an exponential function ---\n");
            outw.WriteLine($"{RawFile} contains the original experimental data.");
            outw.WriteLine($"{LogFile} contains the logarithmic data used for the fit.");
            outw.WriteLine($"{CurvesTxt} contains the sampled best‑fit curve and ±1σ variations.\n");

            outw.WriteLine("--- Plot the experimental data and your best fit ---\n");
            outw.WriteLine($"{DataPlot} shows the data with error bars and the best‑fit line.\n");

            outw.WriteLine("--- Half‑life of ThX and comparison with modern value ---\n");
            outw.WriteLine($"Half‑life from the fit: {T12:0.####} ± {dT12:0.####} days");
            outw.WriteLine($"Modern recommended value: {Tmodern} days\n");

            /* ========== TASK B summary ========== */
            outw.WriteLine("------------ TASK B ------------\n");
            outw.WriteLine("------ Estimate the uncertainty of the half‑life value for ThX ------\n");
            outw.WriteLine($"1σ uncertainty on the half‑life: ±{dT12:0.####} days\n");

            outw.WriteLine("------ Does the fitted value agree with the modern value within that uncertainty? ------\n");
            outw.WriteLine(within
                ? "Yes – the modern value lies inside the confidence interval.\n"
                : "No  – the modern value lies outside the confidence interval.\n");

            /* ========== TASK C summary ========== */
            outw.WriteLine("------------ TASK C ------------\n");
            outw.WriteLine("------ Plot your best fit together with the fits where you change the fit coefficients ------");
            outw.WriteLine("------ by the estimated uncertainties δc in different combinations                     ------\n");
            outw.WriteLine($"{CurvesTxt} contains the numerical table.");
            outw.WriteLine($"{CurvesSvg} shows the three curves plotted together.\n");

            /* ---------- detailed diagnostics (unchanged) ---------- */
            outw.WriteLine("==========================================================================\n");
            outw.WriteLine("Below follow the detailed numerical checks required by the assignment.\n");

            var rng = new Random(1);
            Matrix A = Matrix.Random(6,4,rng);                    // :contentReference[oaicite:1]{index=1}
            var (Q,R) = QR.Decompose(A);                          // :contentReference[oaicite:2]{index=2}

            Utils.WriteMatrix(outw,"Matrix A:",A);
            Utils.WriteMatrix(outw,"Matrix Q:",Q);
            Utils.WriteMatrix(outw,"Matrix R:",R);

            Utils.WriteTest(outw,"Is R upper‑triangular?",IsUpperTriangular(R));
            Utils.WriteTest(outw,"Is QᵀQ ≈ I?",IsIdentity(Q.T * Q,TOL));
            Utils.WriteTest(outw,"Does Q·R reproduce A (‖QR−A‖<1e‑12)?",
                            Frobenius(Q*R - A) < TOL);

            outw.WriteLine("\n------------ Detailed decay‑fit output ------------\n");
            outw.WriteLine($"ln(A)  = {lnA:0.######} ± {dlnA:0.######}");
            outw.WriteLine($"λ      = {lambda:0.######} ± {dlambda:0.######}");
            outw.WriteLine($"T½     = {T12:0.####} ± {dT12:0.####} days");
            outw.WriteLine($"Modern value comparison OK?  {(within ? "Yes" : "No")}\n");

            // --- POINTS from task A, B, and C  ---
            int HW_POINTS_A = 1;
            int HW_POINTS_B = 1;
            int HW_POINTS_C = 1;
            //
            var prev = Console.Out;           
            Console.SetOut(outw);             
            HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
            Console.SetOut(prev);             


        }   // <‑‑ end using(outw)                            // ‑‑‑ FIX for mcs
    }

    /* ------------ helper functions (unchanged) ------------ */
    static bool IsUpperTriangular(Matrix M)
    {
        for (int i=1;i<M.Rows;i++)
            for (int j=0;j<i;j++)
                if (Math.Abs(M[i,j])>TOL) return false;
        return true;
    }
    static bool IsIdentity(Matrix M,double tol)
    {
        if (M.Rows!=M.Cols) return false;
        for (int i=0;i<M.Rows;i++)
            for (int j=0;j<M.Cols;j++)
            {
                double e=(i==j)?1:0;
                if (Math.Abs(M[i,j]-e)>tol) return false;
            }
        return true;
    }
    static double Frobenius(Matrix M)
    {
        double s=0;
        for (int i=0;i<M.Rows;i++)
            for (int j=0;j<M.Cols;j++)
                s+=M[i,j]*M[i,j];
        return Math.Sqrt(s);
    }

    /* write the original table if it is missing */
    static void BuildOriginalDataFile()
    {
        string[] raw = {
            "# time  activity  dy",
            "1  117   6",
            "2  100   5",
            "3  88    4",
            "4  72    4",
            "6  53    4",
            "9  29.5  3",
            "10 25.2  3",
            "13 15.2  2",
            "15 11.1  2"
        };
        File.WriteAllLines(RawFile, raw);
    }
}
