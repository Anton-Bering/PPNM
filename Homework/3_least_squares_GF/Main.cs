using System;
using ScottPlot;
using ScottPlot.Plottables;

class Program {
    static void Main(string[] args) {
        double[] t = { 1, 2, 3, 4, 6, 9, 10, 13, 15 };
        double[] activity = { 117, 100, 88, 72, 53, 29.5, 25.2, 15.2, 11.1 };
        double[] dActivity = { 6, 5, 4, 4, 4, 3, 3, 2, 2 };

        double[] logY = Utils.LogArray(activity);
        double[] dLogY = Utils.LogErrorArray(activity, dActivity);

        Func<double, double>[] fs = {
            z => 1.0,
            z => z
        };

        var (coeff, cov) = LsFit.Fit(fs, t, logY, dLogY);
        double c0 = coeff[0];
        double c1 = coeff[1];
        double a = Math.Exp(c0);
        double lambda = -c1;

        double sigma_c0 = Math.Sqrt(cov[0, 0]);
        double sigma_c1 = Math.Sqrt(cov[1, 1]);

        double halfLife = Math.Log(2) / lambda;
        double sigma_halfLife = Math.Log(2) * sigma_c1 / (lambda * lambda);

        Console.WriteLine("Least-Squares Fit Results (ThX decay data):");
        Console.WriteLine($"c0 = ln(a) = {c0:F4} ± {sigma_c0:F4}");
        Console.WriteLine($"c1 = slope = {c1:F6} ± {sigma_c1:F6} per day");
        Console.WriteLine($"→ Fitted parameters: a = e^(c0) = {a:F3},  λ = {-c1:F5} per day");
        Console.WriteLine($"Half-life T½ = ln(2)/λ = {halfLife:F3} ± {sigma_halfLife:F3} days");

        double modernHalfLife = 3.6316;
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

        var plt = new ScottPlot.Plot();
        plt.Add.Scatter(t, activity, label: "Data", color: Colors.Orange);
        for (int i = 0; i < t.Length; i++) {
            plt.Add.VerticalLine(t[i], color: Colors.Gray, opacity: 0.2);
            plt.Add.VerticalSpan(t[i] - 0.05, t[i] + 0.05, color: Colors.Transparent);
            plt.Add.HorizontalSpan(activity[i] - dActivity[i], activity[i] + dActivity[i], color: Colors.Transparent);
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
        plt.Add.Scatter(t_fit, y_fit, label: "Best fit", color: Colors.Red);

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
            plt.Add.Scatter(t_fit, y_alt, label: label, color: Colors.Green, lineStyle: LineStyle.Dash);
        }

        plt.Title("Least-Squares Fit of ThX Decay");
        plt.XLabel("Time (days)");
        plt.YLabel("Activity (relative units)");
        plt.Legend();
        plt.SavePng("decay_fit.png", 800, 600);
        Console.WriteLine("\nPlot saved to decay_fit.png");
    }
}
