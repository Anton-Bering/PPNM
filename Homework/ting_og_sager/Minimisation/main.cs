using System;
using System.Collections.Generic;

class Program {
    static void Main() {
        // Testfunktioner
        Func<vector, double> f_rosen = v => {
            double x = v[0], y = v[1];
            return (1 - x)*(1 - x) + 100*(y - x*x)*(y - x*x);
        };

        Func<vector, double> f_himmel = v => {
            double x = v[0], y = v[1];
            return (x*x + y - 11)*(x*x + y - 11) + (x + y*y - 7)*(x + y*y - 7);
        };

        // Del A: Newton-minimering på testfunktioner (fremad)
        vector x0_rosen = new vector(1.2, 1.2); // forbedret startpunkt
        int steps_rosen;
        vector min_rosen = minimization.newton(f_rosen, x0_rosen, out steps_rosen, acc: 1e-5, useCentral: false);
        Console.WriteLine($"Rosenbrock minimum found at {min_rosen} in {steps_rosen} steps, f(min) = {f_rosen(min_rosen):F6}");

        vector x0_himmel = new vector(3.5, 1.5); // nyt startpunkt for Himmelblau
        int steps_himmel;
        vector min_himmel = minimization.newton(f_himmel, x0_himmel, out steps_himmel, acc: 1e-5, useCentral: false);
        Console.WriteLine($"Himmelblau's minimum found at {min_himmel} in {steps_himmel} steps, f(min) = {f_himmel(min_himmel):F6}");

        // Del B: Higgs-data (hvis indlæst fra stdin)
        List<double> energy = new List<double>();
        List<double> signal = new List<double>();
        List<double> error = new List<double>();

        if (Console.IsInputRedirected) {
            string line;
            char[] separators = new char[] { ' ', '\t' };
            while ((line = Console.In.ReadLine()) != null) {
                if (line.Trim().Length == 0) continue;
                string[] words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length < 3) continue;
                energy.Add(double.Parse(words[0]));
                signal.Add(double.Parse(words[1]));
                error.Add(double.Parse(words[2]));
            }
        }

        if (energy.Count > 0) {
            Func<vector, double> D = v => {
                double m = v[0];
                double G = v[1];
                double A = v[2];
                double sum = 0;
                for (int i = 0; i < energy.Count; i++) {
                    double E = energy[i];
                    double s = signal[i];
                    double e = error[i];
                    double model = A / ((E - m)*(E - m) + G*G/4);
                    sum += Math.Pow((model - s)/e, 2);
                }
                return sum;
            };

            vector start = new vector(125.0, 3.0, 10.0);
            int steps_fit;
            vector result = minimization.newton(D, start, out steps_fit, acc: 1e-5, useCentral: false);
            Console.WriteLine($"\nFit result: m = {result[0]:F3} GeV, Γ = {result[1]:F3} GeV, A = {result[2]:F3}");
            Console.WriteLine($"Steps taken: {steps_fit}, Dmin = {D(result):F3}");
        }

        // Del C: Sammenlign fremad og central differens
        int steps_rosen_c;
        vector min_rosen_c = minimization.newton(f_rosen, x0_rosen, out steps_rosen_c, acc: 1e-5, useCentral: true);
        Console.WriteLine($"\nRosenbrock (central diff) minimum at {min_rosen_c} in {steps_rosen_c} steps, f(min) = {f_rosen(min_rosen_c):F6}");

        int steps_himmel_c;
        vector min_himmel_c = minimization.newton(f_himmel, x0_himmel, out steps_himmel_c, acc: 1e-5, useCentral: true);
        Console.WriteLine($"Himmelblau (central diff) minimum at {min_himmel_c} in {steps_himmel_c} steps, f(min) = {f_himmel(min_himmel_c):F6}");

        Console.WriteLine($"\nComparison of steps:");
        Console.WriteLine($"Rosenbrock: forward = {steps_rosen}, central = {steps_rosen_c}");
        Console.WriteLine($"Himmelblau: forward = {steps_himmel}, central = {steps_himmel_c}");
    }
}
