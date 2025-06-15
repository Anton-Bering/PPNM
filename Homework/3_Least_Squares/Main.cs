using System;
using System.Globalization;
using System.IO;

class Program
{
    static readonly string OutPath = "Out.txt";
    static readonly double TOL = 1e-12;

    static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        using (var sw = new StreamWriter(OutPath, false))
        {
            /* ------------------------------------------------- */
            /* -------- TASK A : QR‑decomposition sanity ------- */
            /* ------------------------------------------------- */
            sw.WriteLine("------------ Task A: Ordinary least‑squares fit by QR‑decomposition ------------\n");

            var rng = new Random(1);
            Matrix A = Matrix.Random(6, 4, rng);
            var (Q, R) = QR.Decompose(A);

            sw.WriteLine("------  Make sure that your QR‑decomposition routines work for tall matrices ------\n");
            Utils.WriteMatrix(sw, "Matrix A:", A);
            Utils.WriteMatrix(sw, "Matrix Q:", Q);
            Utils.WriteTest(sw, "Does Q have the same dimension as A (6×4)?",
                            Q.Rows == 6 && Q.Cols == 4);

            Utils.WriteMatrix(sw, "Matrix R:", R);
            Utils.WriteTest(sw, "Is R square (4×4)?", R.Rows == 4 && R.Cols == 4);
            Utils.WriteTest(sw, "Is R upper‑triangular?", IsUpperTriangular(R));

            Matrix QtQ = Q.T * Q;
            Utils.WriteMatrix(sw, "Matrix Q^T Q:", QtQ);
            Utils.WriteTest(sw, "Is Q^T Q ≈ I (|⋅|<1e‑12)?", IsIdentity(QtQ, TOL));

            Matrix QRprod = Q * R;
            Utils.WriteMatrix(sw, "Matrix Q R:", QRprod);
            Utils.WriteTest(sw, "Is Q R ≈ A (|⋅|<1e‑12)?",
                            Frobenius(QRprod - A) < TOL);
            sw.WriteLine();

            /* ------------------------------------------------- */
            /* ---- TASK A : radioactive‑decay least‑squares ---- */
            /* ------------------------------------------------- */
            sw.WriteLine("------ Investigate the law of radioactive decay of 224‑Ra (\"ThX\") ------\n");

            if (!File.Exists("Rutherford_and_Soddys_ThX.txt"))
                BuildOriginalDataFile();

            Vector t = Utils.ReadColumn("Rutherford_and_Soddys_ThX.txt", 0);
            Vector y = Utils.ReadColumn("Rutherford_and_Soddys_ThX.txt", 1);
            Vector dy = Utils.ReadColumn("Rutherford_and_Soddys_ThX.txt", 2);

            Vector lnY = new Vector(y.Size);
            Vector dlnY = new Vector(y.Size);
            for (int i = 0; i < y.Size; i++)
            {
                lnY[i] = Math.Log(y[i]);
                dlnY[i] = dy[i] / y[i];
            }

            using (var f = new StreamWriter("Rutherford_and_Soddys_ThX_log.txt", false))
                for (int i = 0; i < t.Size; i++)
                    f.WriteLine($"{t[i]} {lnY[i]} {dlnY[i]}");

            var fs = new Func<double, double>[] { _ => 1.0, z => -z };   // ln(a) - λ t
            var (c, Cov) = LSFit.Fit(fs, t, lnY, dlnY);
            double lnA = c[0], lambda = c[1];
            double dlnA = Math.Sqrt(Cov[0, 0]), dlambda = Math.Sqrt(Cov[1, 1]);

            double T12 = Math.Log(2) / lambda;
            double dT12 = T12 * dlambda / lambda;
            const double Tmodern = 3.6319;

            double deviation = Math.Abs(T12 - Tmodern) / Tmodern;

            sw.WriteLine($"ln(a) = {lnA:0.######} ± {dlnA:0.######}");
            sw.WriteLine($"λ     = {lambda:0.######} ± {dlambda:0.######}");
            sw.WriteLine($"T_1/2 = ln(2)/λ = {T12:0.####} ± {dT12:0.####} days  (modern value: {Tmodern} days)\n");
            Utils.WriteTest(sw, "Is |T_fit − T_modern|/T_modern < 5 %?", deviation < 0.05);
            sw.WriteLine($"Deviation = {deviation:P2}\n");

            /* ------------------------------------------------- */
            /* -------------------- TASK B --------------------- */
            /* ------------------------------------------------- */
            sw.WriteLine("------------ Task B: Uncertainties of the fitting coefficients ------------\n");
            bool within = Math.Abs(T12 - Tmodern) < dT12;
            Utils.WriteTest(sw, "Is the modern half‑life within the 1σ uncertainty band?", within);
            sw.WriteLine();

            /* ------------------------------------------------- */
            /* -------------------- TASK C --------------------- */
            /* ------------------------------------------------- */
            sw.WriteLine("------------ Task C: Evaluation of the quality of the uncertainties on the fit coefficients ------------\n");
            sw.WriteLine("See best_fit_with_changed_coefficients.svg for the comparison plot.\n");

            // create curves for gnuplot
            Func<double, double> fit   = z => lnA - lambda * z;
            Func<double, double> fitLo = z => (lnA - dlnA) - (lambda + dlambda) * z;
            Func<double, double> fitHi = z => (lnA + dlnA) - (lambda - dlambda) * z;

            using (var bf = new StreamWriter("bestfit_curves.txt", false))
                for (double z = 0; z <= 15; z += 0.1)
                    bf.WriteLine($"{z} {fit(z)} {fitLo(z)} {fitHi(z)}");
        }

        Console.WriteLine("Finished.  Out.txt and all auxiliary files generated.");
    }

    /* ===== helper functions now at class level ===== */

    static bool IsUpperTriangular(Matrix M)
    {
        for (int i = 1; i < M.Rows; i++)
            for (int j = 0; j < i; j++)
                if (Math.Abs(M[i, j]) > TOL) return false;
        return true;
    }

    static bool IsIdentity(Matrix M, double tol)
    {
        if (M.Rows != M.Cols) return false;
        for (int i = 0; i < M.Rows; i++)
            for (int j = 0; j < M.Cols; j++)
            {
                double expected = (i == j) ? 1 : 0;
                if (Math.Abs(M[i, j] - expected) > tol) return false;
            }
        return true;
    }

    static double Frobenius(Matrix M)
    {
        double s = 0;
        for (int i = 0; i < M.Rows; i++)
            for (int j = 0; j < M.Cols; j++)
                s += M[i, j] * M[i, j];
        return Math.Sqrt(s);
    }

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
        File.WriteAllLines("Rutherford_and_Soddys_ThX.txt", raw);
    }
}
