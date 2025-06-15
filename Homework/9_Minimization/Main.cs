using System;
using System.Collections.Generic;

class Program {
    static void Main(string[] args) {
        // Read Higgs data from standard input into lists
        var energy = new List<double>();
        var signal = new List<double>();
        var error  = new List<double>();
        char[] separators = new char[] {' ', '\t'};
        var options = StringSplitOptions.RemoveEmptyEntries;
        while (true) {
            string line = Console.In.ReadLine();
            if (line == null) break;
            string[] words = line.Split(separators, options);
            if (words.Length < 3) continue;
            try {
                energy.Add(double.Parse(words[0]));
                signal.Add(double.Parse(words[1]));
                error.Add(double.Parse(words[2]));
            } catch {
                // ignore lines that cannot be parsed (e.g., headers or invalid lines)
            }
        }
        // Transfer data to Functions static arrays (if any data was read)
        if (energy.Count > 0) {
            Functions.EData = energy.ToArray();
            Functions.SigmaData = signal.ToArray();
            Functions.ErrorData = error.ToArray();
        }
        // Test Newton's method on Rosenbrock function
        double[] rosenInit = new double[] {0.0, 0.0};
        double[] rosenResultForward = Newton.Minimize(Functions.Rosenbrock, rosenInit, 1e-3, out int iterR_forward, false);
        double[] rosenResultCentral = Newton.Minimize(Functions.Rosenbrock, rosenInit, 1e-3, out int iterR_central, true);
        // Test Newton's method on Himmelblau's function
        double[] himmelInit = new double[] {0.0, 0.0};
        double[] himmelResult = Newton.Minimize(Functions.Himmelblau, himmelInit, 1e-3, out int iterH, false);
        // If data was provided, perform Higgs Breit-Wigner fit
        double[] fitResult = null;
        int iterFit = 0;
        if (energy.Count > 0) {
            // initial guess for [m, Γ, A]
            double[] fitInit = new double[] {125.0, 5.0, 10.0};
            fitResult = Newton.Minimize(Functions.Deviation, fitInit, 1e-3, out iterFit, false);
        }
        // Write results to output
        // Rosenbrock results (forward vs central difference)
        Console.WriteLine($"Rosenbrock minimum (forward diff): (x={rosenResultForward[0]:F3}, y={rosenResultForward[1]:F3}), after {iterR_forward} iterations.");
        Console.WriteLine($"Rosenbrock minimum (central diff): (x={rosenResultCentral[0]:F3}, y={rosenResultCentral[1]:F3}), after {iterR_central} iterations.");
        // Himmelblau result
        Console.WriteLine($"Himmelblau minimum: (x={himmelResult[0]:F3}, y={himmelResult[1]:F3}), after {iterH} iterations.");
        // Higgs fit results
        if (fitResult != null) {
            Console.WriteLine($"Higgs fit results: m = {fitResult[0]:F2} GeV, Γ = {fitResult[1]:F2} GeV, A = {fitResult[2]:F2}");
        } else {
            Console.WriteLine("No Higgs data provided. Skipping fit.");
        }
    }
}
