/* ----------------------------------------------------------------------
 *  Homework – Ordinary Least‑Squares Fit   (updated for 
 * -------------------------------------------------------------------- */
using System;
using System.Globalization;
using System.Diagnostics;   //  HERHER: CG
using System.IO;
using static MatrixHelpers;
using static VectorHelpers;

class Program
{
    const string OutPath   = "Out.txt";
    const string RawFile   = "In_Task_A/Rutherford_and_Soddys_ThX.txt";
    const string LogFile   = "Rutherford_and_Soddys_ThX_log.txt";
    const string CurvesTxt = "bestfit_curves.txt";
    const string DataPlot  = "Rutherford_and_Soddys_ThX.svg";
    const string CurvesSvg = "best_fit_with_changed_coefficients.svg";
    const string FitTxt    = "Fit_the_ThX_data_with_exponential_function.txt";

    const double Tmodern = 3.6313;   // modern half‑life of 224-Ra [days], Kilde: https://www.sciencedirect.com/science/article/abs/pii/S0969804320307107


    static void Main()
    {
        // ------------ TASK A: del 1 ------------
        Console.WriteLine("------------ TASK A: Ordinary least-squares fit by QR-decomposition ------------\n");

        Console.WriteLine("------ Make sure that your QR-decomposition routines work for tall matrices ------");
        Console.WriteLine("------ (A tall matrix of size n×m is one where n>m)                         ------\n");

        Console.WriteLine("Below, I check that my QR decomposition routines work for tall matrices:\n");

        Console.WriteLine("Check part 1) Generate a random tall (6x4) matrix A:\n");
        matrix A = RandomMatrix(6, 4);
        Console.WriteLine(PrintMatrix(A, "A"));

        Console.WriteLine("Check part 2) Decompose A into Q and R:\n");

        // var (Q, R) = QR.Decompose(A);
        var qr = new QR(A);
        matrix Q = qr.Q;
        matrix R = qr.R;
        
        Console.WriteLine(PrintMatrix(Q, "Q"));
        Console.WriteLine(PrintMatrix(R, "R"));

        Console.WriteLine("Check part 3) Check that R is upper triangular:\n");
        CheckUpperTriangular(R, "R");

        Console.WriteLine("\nCheck part 4) Check that QᵀQ ≈ I:\n");
        matrix QTQ = Transpose(Q) * Q;
        CheckIdentityMatrix(QTQ, "QᵀQ");

        Console.WriteLine("\nCheck part 5) Check that QR ≈ A:\n");
        matrix QR_check5 = Q * R;
        Console.WriteLine(PrintMatrix(QR_check5, "QR"));
        CheckMatrixEqual(QR_check5, A, "QR", "A");
        
        // ---------------- TASK A: del 2 ------------
        Console.WriteLine("\n------ Implement a routine that makes a least-squares fit (using your QR-decomposition routines) ------");
        Console.WriteLine("------ The routine must calculate and return the vector of the best fit coefficients, {c_k}      ------\n");

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

        /* ------------- 3a.  write data–vs–fit table (Task A) ------------- */
        using (var ft = new StreamWriter(FitTxt, false))
            for (int i = 0; i < t.Length; i++)
                ft.WriteLine($"{t[i]} {lnY[i]} {fit(t[i])}");

        Console.WriteLine($"Best‑fit coefficients:");
        Console.WriteLine($"   ln(a)  = {lnA:g12}");
        Console.WriteLine($"   λ     = {lambda:g12}");

        Console.WriteLine($"\nFull data‑versus‑fit table written to “{FitTxt}”.\n");

        Console.WriteLine("\n------ Fit the ThX data with exponential function in the usual logarithmic way ------\n");

        Console.WriteLine("Radioactive decay follows: y(t)=a*exp(-λt)");
        Console.WriteLine("where y is the activity, t is time, a is the activity at t=0, and λ is the decay constant.");
        Console.WriteLine("The uncertainty of the measurement is denoted as δy.");

        Console.WriteLine("\nThe uncertainty of the logarithm is: δln(y)=δy/y");

        Console.WriteLine("\n------ Plot the experimental data (with error-bars) and your best fit ------\n");

        Console.WriteLine($"SVG figure saved as “{DataPlot}”.\n");

        Console.WriteLine("\n------ From your fit find out the half-life time, T_{1/2}=ln(2)/λ, of ThX ------\n");
        
        Console.WriteLine($"T_1/2  = {T12:g6} ± {dT12:g6} days");

        Console.WriteLine("\n------ Compare your result for ThX with the modern value ------");
        Console.WriteLine("------ (ThX is known today as 224Ra)                     ------\n");

        Console.WriteLine($"Modern value : {Tmodern} days");
        Console.WriteLine(within
            ? "The modern value lies **within** the 1 σ uncertainty of the fit."
            : "The modern value lies **outside** the 1 σ uncertainty of the fit.");
        Console.WriteLine();

        Console.WriteLine("\n------------ TASK B: Uncertainties of the fitting coefficients ------------\n");

        Console.WriteLine("\n------ Modify you least-squares fitting function such that it also calculates  ------");
        Console.WriteLine("------ the covariance matrix and the uncertainties of the fitting coefficients ------\n");

        Console.WriteLine($"The covariance matrix of (ln a, λ):\n");
        Console.WriteLine(PrintMatrix(Cov));
        Console.WriteLine($"Uncertainty on ln a : {dlnA:g6}");
        Console.WriteLine($"Uncertainty on λ    : {dlambda:g6}\n");

        Console.WriteLine($"Propagated uncertainty on half‑life: ±{dT12:g6} days\n");
        Console.WriteLine(within
            ? "Half‑life agrees with the modern value within its uncertainty.\n"
            : "Half‑life does **not** agree with the modern value within its uncertainty.\n");

        Console.WriteLine("\n------ Estimate the uncertainty of the half-life value for ThX from the given data ------");
        Console.WriteLine("------ does it agree with the modern value within the estimated uncertainty?       ------\n");

        Console.WriteLine("\n------------ TASK C: Evaluation of the quality of the uncertainties on the fit coefficients ------------\n");

        Console.WriteLine("\n------ Plot your best fit, together with the fits where you change the fit coefficients ------");
        Console.WriteLine("------ by the estimated uncertainties δc in different combinations                      ------\n");

        Console.WriteLine($"SVG figure with curves for c ± δc saved as “{CurvesSvg}”.\n");
            
        
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
}
