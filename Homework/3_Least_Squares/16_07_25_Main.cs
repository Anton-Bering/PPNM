/* ----------------------------------------------------------------------
 *  Homework – Ordinary Least‑Squares Fit   (updated for VectorAndMatrix)
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
        double[] t  = Utils.ReadColumn(RawFile, 0);
        double[] y  = Utils.ReadColumn(RawFile, 1);
        double[] dy = Utils.ReadColumn(RawFile, 2);

        var lnY  = new double[y.Length];
        var dlnY = new double[y.Length];
        for (int i = 0; i < y.Length; i++)
        {
            lnY[i]  = Math.Log(y[i]);
            dlnY[i] = dy[i] / y[i];
        }
        using (var swLog = new StreamWriter(LogFile, false))
            for (int i = 0; i < t.Length; i++)
                swLog.WriteLine($"{t[i]} {lnY[i]} {dlnY[i]}");

        /* ------------- 2.  least‑squares fit  lnA − λ t ------------- */
        var fs = new Func<double,double>[] { _ => 1.0, z => -z };
        var (c , Cov) = LSFit.Fit(fs, t, lnY, dlnY);

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
        using (var outw = new StreamWriter(OutPath, false))
        {
            /* ========== TASK A summary ========== */
            outw.WriteLine("------------ TASK A: Ordinary least-squares fit by QR-decomposition ------------\n");

            outw.WriteLine("------ Make sure that your QR-decomposition routines work for tall matrices ------\n");

            outw.WriteLine("A tall matrix of size n×m is one where n>m.");
            outw.WriteLine("Below, I check that my QR decomposition routines work for tall matrices:\n");

            outw.WriteLine("1) Generate a random tall (6x4) matrix A:\n");
            double[,] A = VectorAndMatrix.RandomMatrix(6, 4);
            Utils.WriteMatrix(outw, "Matrix A:", A);

            outw.WriteLine("2) Factorize A into QR:\n"); // HERHER: Jeg er ikke sikker på at "Factorize A into QR" er den rette formulering
            var (Q, R) = QR.Decompose(A);
            Utils.WriteMatrix(outw, "Matrix Q:", Q);
            Utils.WriteMatrix(outw, "Matrix R:", R);

            outw.WriteLine("3) Check that R is upper triangular:\n");
            Utils.WriteTest(outw, "Is R upper‑triangular?",
                            VectorAndMatrix.IsUpperTriangular(R, TOL));

            outw.WriteLine("4) Check that QᵀQ ≈ I:\n");
            double[,] QTQ = VectorAndMatrix.Multiply(
                                VectorAndMatrix.Transpose(Q), Q);
            Utils.WriteTest(outw, "Is QᵀQ ≈ I?",
                            VectorAndMatrix.IsIdentityMatrix(QTQ, TOL));

            outw.WriteLine("5) Check that QR ≈ A:\n");
            double[,] diff = SubtractMatrices(
                                 VectorAndMatrix.Multiply(Q, R), A);
            Utils.WriteTest(outw, "Does Q·R reproduce A (‖QR−A‖<1e‑12)?",
                            Frobenius(diff) < TOL);

            outw.WriteLine("\n------ Implement a routine that makes a least-squares fit (using your QR-decomposition routines) ------");
            outw.WriteLine("------ The routine must calculate and return the vector of the best fit coefficients, {c_k}      ------\n");

            // HERHER: ER KOMMET HER TIL
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------ Fit the ThX data with exponential function in the usual logarithmic way ------\n");

            outw.WriteLine("Radioactive decay follows: y(t)=a*exp(-λt)");
            outw.WriteLine("where y is the activity, t is time, a is the activity at t=0, and λ is the decay constant.");
            outw.WriteLine("The uncertainty of the measurement is denoted as δy.");

            outw.WriteLine("\nHERHER: OBS: indset data");
            /* HERHER: Inkluder det her:
            Measured data for ThX:  
            t [days]	: 1,  2,  3, 4, 6, 9,   10,  13,  15
            y [relative units]:	117,100,88,72,53,29.5,25.2,15.2,11.1
            δy:	6,5,4,4,4,3,3,2,2
            */

            outw.WriteLine("\nThe uncertainty of the logarithm is: δln(y)=δy/y");
            outw.WriteLine("HERHER: OBS: Bevis at: δln(y)=δy/y");

            outw.WriteLine("\n------ Plot the experimental data (with error-bars) and your best fit ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------ From your fit find out the half-life time, T_{1/2}=ln(2)/λ, of ThX ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------ Compare your result for ThX with the modern value ------");
            outw.WriteLine("------ (ThX is known today as 224Ra)                     ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------------ TASK B: Uncertainties of the fitting coefficients ------------\n");

            outw.WriteLine("\n------ Modify you least-squares fitting function such that it also calculates  ------");
            outw.WriteLine("------ the covariance matrix and the uncertainties of the fitting coefficients ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------ Estimate the uncertainty of the half-life value for ThX from the given data ------");
            outw.WriteLine("------ does it agree with the modern value within the estimated uncertainty?       ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            outw.WriteLine("\n------------ TASK C: Evaluation of the quality of the uncertainties on the fit coefficients ------------\n");

            outw.WriteLine("\n------ Plot your best fit (where ĉ are the best-fit coefficients)   ------");
            outw.WriteLine("------ together with the fits where you change the fit coefficients ------");
            outw.WriteLine("------ by the estimated uncertainties δc in different combinations  ------\n");
            outw.WriteLine("HERHER: OBS: indset svar");

            // HERHER: KOMMET HER TIL:
            outw.WriteLine("\n---KOMMET HER TIL---KOMMET HER TIL---KOMMET HER TIL---KOMMET HER TIL---KOMMET HER TIL---\n");

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

            /* ---------- detailed diagnostics (updated) ---------- */


            outw.WriteLine("================================ DEL 2 ==========================================\n");
            outw.WriteLine("Below follow the detailed numerical checks required by the assignment.\n");

            outw.WriteLine("\n------------ Detailed decay‑fit output ------------\n");
            outw.WriteLine($"ln(A)  = {lnA:0.######} ± {dlnA:0.######}");
            outw.WriteLine($"λ      = {lambda:0.######} ± {dlambda:0.######}");
            outw.WriteLine($"T½     = {T12:0.####} ± {dT12:0.####} days");
            outw.WriteLine($"Modern value comparison OK?  {(within ? "Yes" : "No")}\n");

            /* --- POINTS from task A, B, and C  --- */
            int HW_POINTS_A = 1;
            int HW_POINTS_B = 1;
            int HW_POINTS_C = 1;

            var prev = Console.Out;
            Console.SetOut(outw);
            HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
            Console.SetOut(prev);
        }
    }

    /* ----------------- helpers for diagnostics ----------------- */
    static double[,] SubtractMatrices(double[,] X, double[,] Y)
    {
        int n = X.GetLength(0), m = X.GetLength(1);
        var Z = new double[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                Z[i, j] = X[i, j] - Y[i, j];
        return Z;
    }

    static double Frobenius(double[,] M)
    {
        double s = 0.0;
        int n = M.GetLength(0), m = M.GetLength(1);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                s += M[i, j] * M[i, j];
        return Math.Sqrt(s);
    }

    /* write the original table if it is missing (unchanged) */
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
