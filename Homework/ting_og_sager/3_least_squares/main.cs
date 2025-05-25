using System;
using System.IO;

class Program {
    static void Main() {
        // Experimental data: time (days), activity, uncertainties
        double[] timesArr = { 1, 2, 3, 4, 6, 9, 10, 13, 15 };
        double[] activityArr = { 117, 100, 88, 72, 53, 29.5, 25.2, 15.2, 11.1 };
        double[] sigmaYArr = { 6, 5, 4, 4, 4, 3, 3, 2, 2 };
        int n = timesArr.Length;

        // Prepare data
        vector t = new vector(timesArr);
        double[] lnY = new double[n];
        for (int i = 0; i < n; i++) lnY[i] = Math.Log(activityArr[i]);
        vector y = new vector(lnY);
        double[] sigmaLnY = new double[n];
        for (int i = 0; i < n; i++) sigmaLnY[i] = sigmaYArr[i] / activityArr[i];
        vector dy = new vector(sigmaLnY);

        // Fit functions
        var fs = new Func<double, double>[] {
            z => 1.0,
            z => z
        };

        // Perform least-squares fit
        var (c, Cov) = LSFit.lsfit(fs, t, y, dy);
        double c0 = c[0], c1 = c[1];
        double dc0 = Math.Sqrt(Cov[0, 0]);
        double dc1 = Math.Sqrt(Cov[1, 1]);

        // Transform back
        double a = Math.Exp(c0);
        double lambda = -c1;
        double da = a * dc0;
        double dLambda = dc1;
        double halfLife = Math.Log(2) / lambda;
        double dHalfLife = Math.Log(2) / (lambda * lambda) * dLambda;

        // Print results
        Console.WriteLine("Fit results for ln(y) = c0 + c1 * t (exponential decay model):");
        Console.WriteLine($"c0 = {c0:F5} ± {dc0:F5}");
        Console.WriteLine($"c1 = {c1:F5} ± {dc1:F5}\n");
        Console.WriteLine($"Derived parameters for y = a * exp(-λ t):");
        Console.WriteLine($"a = e^{{c0}} = {a:F3} ± {da:F3} (initial activity)");
        Console.WriteLine($"λ = -c1 = {lambda:F5} ± {dLambda:F5} (decay constant 1/day)");
        Console.WriteLine($"T_1/2 = ln(2)/λ = {halfLife:F2} ± {dHalfLife:F2} days (half-life)");
        Console.WriteLine($"Modern known T_1/2 ≈ 3.63 days");

        // Export: data.txt
        using (StreamWriter sw = new StreamWriter("data.txt")) {
            for (int i = 0; i < n; i++) {
                sw.WriteLine($"{t[i]} {activityArr[i]} {sigmaYArr[i]}");
            }
        }

        // Fit functions
        Func<double, double> f_fit = x => a * Math.Exp(-lambda * x);

        double c0_upper = c0 + dc0, c0_lower = c0 - dc0;
        double c1_upper = c1 + dc1, c1_lower = c1 - dc1;

        Func<double, double> f_upper = x => Math.Exp(c0_upper + c1_upper * x);
        Func<double, double> f_lower = x => Math.Exp(c0_lower + c1_lower * x);

        int npts = 100;
        double t_min = t[0], t_max = t[t.size - 1];
        double dt = (t_max - t_min) / (npts - 1);

        using (StreamWriter sw = new StreamWriter("fit.txt")) {
            for (int i = 0; i < npts; i++) {
                double x = t_min + i * dt;
                sw.WriteLine($"{x} {f_fit(x)}");
            }
        }

        using (StreamWriter sw = new StreamWriter("fit_upper.txt")) {
            for (int i = 0; i < npts; i++) {
                double x = t_min + i * dt;
                sw.WriteLine($"{x} {f_upper(x)}");
            }
        }

        using (StreamWriter sw = new StreamWriter("fit_lower.txt")) {
            for (int i = 0; i < npts; i++) {
                double x = t_min + i * dt;
                sw.WriteLine($"{x} {f_lower(x)}");
            }
        }
    }
}
