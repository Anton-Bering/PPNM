using System;
using System.Drawing;
using ScottPlot;

class Program {
    static void Main(string[] args) {
        // Part A: Input data (ThX decay experiment)
        double[] t = { 1, 2, 3, 4, 6, 9, 10, 13, 15 };                   // time in days
        double[] activity = { 117, 100, 88, 72, 53, 29.5, 25.2, 15.2, 11.1 }; // activity measurements
        double[] dActivity = { 6, 5, 4, 4, 4, 3, 3, 2, 2 };               // uncertainties (~5% of y)

        // Transform to log scale for exponential fit: ln(y) = ln(a) - λ t
        double[] logY = Utils.LogArray(activity);
        double[] dLogY = Utils.LogErrorArray(activity, dActivity);  // δln(y) = δy/y

        // Basis functions for ln(y) vs t: f0 = 1 (constant), f1 = t (linear term)
        Func<double, double>[] fs = {
            z => 1.0,
            z => z
        };

        // Perform weighted least-squares fit using QR decomposition
        var (coeff, cov) = LsFit.Fit(fs, t, logY, dLogY);
        double c0 = coeff[0];                // c0 = ln(a)
        double c1 = coeff[1];                // c1 = slope = -λ
        double a = Math.Exp(c0);             // fitted parameter a
        double lambda = -c1;                 // decay constant λ

        // Uncertainties of fit coefficients (sqrt of covariance diagonal)
        double sigma_c0 = Math.Sqrt(cov[0, 0]);
        double sigma_c1 = Math.Sqrt(cov[1, 1]);

        // Compute half-life and its uncertainty by error propagation
        double halfLife = Math.Log(2) / lambda;
        double sigma_halfLife = Math.Log(2) * sigma_c1 / (lambda * lambda);

        // Output results to console (redirect to Out.txt via Makefile)
        Console.WriteLine("Least-Squares Fit Results (ThX decay data):");
        Console.WriteLine($"c0 = ln(a) = {c0:F4} ± {sigma_c0:F4}");
        Console.WriteLine($"c1 = slope = {c1:F6} ± {sigma_c1:F6} per day");
        Console.WriteLine($"\u2192 Fitted parameters: a = e^(c0) = {a:F3},  λ = {-c1:F5} per day");
        Console.WriteLine($"Half-life T½ = ln(2)/λ = {halfLife:F3} ± {sigma_halfLife:F3} days");

        double modernHalfLife = 3.6316; // modern T½ of Ra-224 in days
        double diff = Math.Abs(halfLife - modernHalfLife);
        double nsig = diff / sigma_halfLife;
        Console.WriteLine($"Modern T½ ≈ 3.63 days; difference = {diff:F3} days (~{nsig:F1}σ).");
        if (diff <= sigma_halfLife) {
            Console.WriteLine("→ Modern value is within 1σ of our fit result.");
        } else if (diff <= 2 * sigma_halfLife) {
            Console.WriteLine("→ Modern value is within 2σ, but outside 1σ (not a perfect agreement).");
        } else {
            Console.WriteLine("→ Modern value lies outside the 2σ range of our fit (discrepancy).");
        }

        // Part C: Plot data, best fit, and ±σ-perturbed fits
        var plt = new ScottPlot.Plot(600, 400);
        plt.AddScatter(t, activity, markerShape: MarkerShape.filledCircle, color: Color.Orange,
                       markerSize: 5, lineWidth: 0, label: "Data");
        for (int i = 0; i < t.Length; i++) {
            double[] vx = { t[i], t[i] };
            double[] vy = { activity[i] - dActivity[i], activity[i] + dActivity[i] };
            var errLine = plt.AddScatter(vx, vy, color: Color.Orange, lineWidth: 1);
            errLine.MarkerSize = 0;
        }

        int Nplot = 200;
        double xMin = t[0], xMax = t[0];
        for (int i = 1; i < t.Length; i++) {
            if (t[i] < xMin) xMin = t[i];
            if (t[i] > xMax) xMax = t[i];
        }
        double step = (xMax - xMin) / (Nplot - 1);
        double[] t_fit = new double[Nplot];
        double[] y_fit = new double[Nplot];
        for (int i = 0; i < Nplot; i++) {
            double xVal = xMin + i * step;
            t_fit[i] = xVal;
            double logY_pred = c0 + c1 * xVal;
            y_fit[i] = Math.Exp(logY_pred);
        }
        plt.AddScatter(t_fit, y_fit, color: Color.Red, lineWidth: 2, label: "Best fit");

        int[,] combos = { { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
        for (int idx = 0; idx < combos.GetLength(0); idx++) {
            int sign_c0 = combos[idx, 0];
            int sign_c1 = combos[idx, 1];
            double c0_alt = c0 + sign_c0 * sigma_c0;
            double c1_alt = c1 + sign_c1 * sigma_c1;
            double[] y_alt = new double[Nplot];
            for (int i = 0; i < Nplot; i++) {
                double logY_alt = c0_alt + c1_alt * t_fit[i];
                y_alt[i] = Math.Exp(logY_alt);
            }
            string label = $"Fit (c0 {(sign_c0 > 0 ? "+" : "−")}σ, c1 {(sign_c1 > 0 ? "+" : "−")}σ)";
            plt.AddScatter(t_fit, y_alt, color: Color.Green, lineStyle: LineStyle.Dash,
                           lineWidth: 1, label: label);
        }

        plt.Title("Least-Squares Fit of ThX Decay");
        plt.XLabel("Time (days)");
        plt.YLabel("Activity (relative units)");
        plt.Legend();
        plt.SaveFig("decay_fit.png");
        Console.WriteLine("\nPlot saved to decay_fit.png");
    }
}
