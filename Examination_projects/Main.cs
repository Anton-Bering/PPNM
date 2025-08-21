using System;
using PPNM.Minimization;
using System.IO;
using System.Collections.Generic;

class Program {

    // A static scoreboard to track wins across all tests.
    private static Dictionary<string, int> overallScores = new Dictionary<string, int> {
        { "Broyden's update", 0 },
        { "Symmetrized Broyden's update", 0 },
        { "Tie", 0 }
    };

    // Helper method to update the scoreboard after each test.
    private static void TallyOverallWinner(string winner)
    {
        if (overallScores.ContainsKey(winner))
        {
            overallScores[winner]++;
        }
    }

    // A small helper class to define a test problem in a structured way.
    public class TestProblem
    {
        public string Title { get; set; }
        public Func<vector, double> Function { get; set; }
        public vector StartPoint { get; set; }
        public vector ExpectedMinimum { get; set; }
    }
    
    // Runs both minimization methods on a given problem and prints a detailed report and comparison.
    static QuasiNewton.MinimizationOutput[] PrintBlock(
        string title, 
        Func<vector,double> f, 
        vector start, 
        double acc, 
        vector expected_x_min = null)
    {
        var methods = new[]
        {
            new { Name = "Broyden's update", UseSym = false },
            new { Name = "Symmetrized Broyden's update", UseSym = true  }
        };

        var results = new QuasiNewton.MinimizationOutput[methods.Length];
        for (int i = 0; i < methods.Length; i++)
            results[i] = QuasiNewton.MinimizeReport(f, start, acc, useSymmetrized: methods[i].UseSym);

        Console.WriteLine($"------------ {title} ------------\n");
        if (expected_x_min != null) {
            Console.WriteLine($"Expected minimum: {expected_x_min}");
        }
        Console.WriteLine($"Accuracy goal (acc): {acc}");
        Console.WriteLine($"Start point (x_start): {start}\n");

        for (int i = 0; i < methods.Length; i++)
        {
            var r = results[i].Stats;
            Console.WriteLine($"--- Results for {methods[i].Name} ---");
            Console.WriteLine($"Found minimum (x_min):                    {r.x_min}");
            Console.WriteLine($"Value at minimum, f(x_min):               {r.f_min}"); 
            Console.WriteLine($"Gradient norm at x_min:                   {r.gradNorm}  (should be < acc)");
            Console.WriteLine($"Number of iterations:                     {r.iterations}  (lower is better)");
            Console.WriteLine($"Number of resets:                         {r.resets}  (lower is better)");
            Console.WriteLine($"Length of last accepted step (λ_last):    {r.last_step}  (ideally ≈ 1)");
            Console.WriteLine($"Number of evaluations:                    {r.fevals}  (lower is better)\n");
            if (expected_x_min != null)
            {
                bool failed = r.iterations >= 9999; 
                bool agrees = !failed && (r.x_min - expected_x_min).Norm() < acc;
                string YES_or_NO = agrees ? "YES" : "NO";
                Console.WriteLine($"Does the found minimum agree with expectation? {YES_or_NO}.\n");
            }
        }

        // --- Broyden vs Symmetrized Broyden ---
        Console.WriteLine($"--- Broyden's update vs Symmetrized Broyden's update ---\n");

        var rb = results[0].Stats;
        var rs = results[1].Stats;
        string overallWinner = "Tie";

        // Criterion 0: Accuracy (highest priority, only if expected_x_min is provided)
        string accuracyWinner = "N/A"; 
        if (expected_x_min != null)
        {
            Console.WriteLine("0. Accuracy (did the method find the correct minimum?):");
            bool rb_failed = rb.iterations >= 9999;
            bool rs_failed = rs.iterations >= 9999;
            bool rb_correct = !rb_failed && (rb.x_min - expected_x_min).Norm() < acc;
            bool rs_correct = !rs_failed && (rs.x_min - expected_x_min).Norm() < acc;

            if (rb_correct && !rs_correct) {
                accuracyWinner = "Broyden's update";
                Console.WriteLine($"   >> Winner: Broyden. Found the correct minimum where the other failed.\n");
            } else if (!rb_correct && rs_correct) {
                accuracyWinner = "Symmetrized Broyden's update";
                Console.WriteLine($"   >> Winner: Symmetrized Broyden. Found the correct minimum where the other failed.\n");
            } else if (rb_correct && rs_correct) {
                accuracyWinner = "Tie";
                Console.WriteLine($"   >> Tie. Both methods found the correct minimum.\n");
            } else {
                accuracyWinner = "None";
                Console.WriteLine($"   >> None. Neither method found the correct minimum.\n");
            }
        }

        // Criterion 1: Quality of the solution (f_min)
        Console.WriteLine("1. Quality of the found minimum (lower f_min is better):");
        string qualityWinner = "Tie";
        double f_min_diff = Math.Abs(rb.f_min - rs.f_min); 
        if (rs.f_min < rb.f_min && f_min_diff > 1e-9 * (Math.Abs(rb.f_min) + Math.Abs(rs.f_min))) {
            qualityWinner = "Symmetrized Broyden's update";
            Console.WriteLine($"   >> Winner: Symmetrized Broyden. Found a significantly better minimum ({rs.f_min:G5} vs {rb.f_min:G5}).\n");
        } else if (rb.f_min < rs.f_min && f_min_diff > 1e-9 * (Math.Abs(rb.f_min) + Math.Abs(rs.f_min))) {
            qualityWinner = "Broyden's update";
            Console.WriteLine($"   >> Winner: Broyden. Found a significantly better minimum ({rb.f_min:G5} vs {rs.f_min:G5}).\n");
        } else {
            Console.WriteLine($"   >> Tie. Both methods found minima of similar quality.\n");
        }

        // Criterion 2: Efficiency (number of evaluations)
        Console.WriteLine("2. Efficiency (fewer function evaluations is better):");
        string efficiencyWinner = "Tie";
        double spEval = (rs.fevals > 0) ? (double)rb.fevals / rs.fevals : double.PositiveInfinity;
        Console.WriteLine($"   Efficiency Speedup (Broyden Evals / Symmetrized Evals): {spEval:F2}");
        if (spEval > 1.05) { 
            efficiencyWinner = "Symmetrized Broyden's update";
            Console.WriteLine($"   >> Winner: Symmetrized Broyden. Was significantly more efficient.\n");
        } else if (spEval < 0.95) {
            efficiencyWinner = "Broyden's update";
            Console.WriteLine($"   >> Winner: Broyden. Was significantly more efficient ({1/spEval:F2}x).\n");
        } else {
            Console.WriteLine($"   >> Tie. Both methods had a similar efficiency.\n");
        }
        
        // Criterion 3: Robustness (resets and convergence failure)
        Console.WriteLine("3. Robustness (fewer resets and successful convergence indicates stability):");
        string robustnessWinner = "Tie";
        bool rb_failed_robust = rb.iterations >= 9999;
        bool rs_failed_robust = rs.iterations >= 9999;

        if(rb_failed_robust && !rs_failed_robust){
            robustnessWinner = "Symmetrized Broyden's update";
            Console.WriteLine($"   >> Winner: Symmetrized Broyden. Converged where Broyden failed.\n");
        } else if (!rb_failed_robust && rs_failed_robust){
            robustnessWinner = "Broyden's update";
            Console.WriteLine($"   >> Winner: Broyden. Converged where Symmetrized Broyden failed.\n");
        } else if(rs.resets < rb.resets) {
            robustnessWinner = "Symmetrized Broyden's update";
            Console.WriteLine($"   >> Winner: Symmetrized Broyden. Was more stable ({rs.resets} vs {rb.resets} resets).\n");
        } else if (rb.resets < rs.resets) {
            robustnessWinner = "Broyden's update";
            Console.WriteLine($"   >> Winner: Broyden. Was more stable ({rb.resets} vs {rs.resets} resets).\n");
        } else {
            Console.WriteLine($"   >> Tie. Both methods were equally robust.\n");
        }

        // --- Overall Verdict based on prioritized criteria ---
        Console.WriteLine("Overall Verdict:");
        if (accuracyWinner != "N/A" && accuracyWinner != "Tie" && accuracyWinner != "None") {
            Console.WriteLine($"The WINNER is {accuracyWinner}, as it was the only one to find the correct minimum.");
            overallWinner = accuracyWinner;
        } else if (robustnessWinner != "Tie" && (rb_failed_robust || rs_failed_robust)) { // Check for convergence failure
             Console.WriteLine($"The WINNER is {robustnessWinner}, as it solved the problem where the other method failed.");
             overallWinner = robustnessWinner;
        } else if (qualityWinner != "Tie") {
            Console.WriteLine($"The WINNER is {qualityWinner}, as it found a minimum of significantly higher quality.");
            overallWinner = qualityWinner;
        } else if (robustnessWinner != "Tie") {
            Console.WriteLine($"With equal solution quality, {robustnessWinner} wins by being more robust.");
            overallWinner = robustnessWinner;
        } else if (efficiencyWinner != "Tie") {
            Console.WriteLine($"With equal quality and robustness, {efficiencyWinner} wins by being more efficient.");
            overallWinner = efficiencyWinner;
        } else {
            Console.WriteLine("Both methods performed identically on this task.");
            overallWinner = "Tie";
        }
        Console.WriteLine();
        
        TallyOverallWinner(overallWinner);
        
        return results;
    }

    // The main entry point of the program. Organizes the tests.
    static void Main(){
        double acc = 1e-6;

        RunStandardTests(acc);
        RunComplexProblemTests(acc);
        PrintFinalConclusion();
        
        Console.WriteLine("All tasks completed.");
    }

    // ------------ Test your implementation on some functions with known minima ------------
    private static void RunStandardTests(double acc)
    {
        Console.WriteLine("###################################################################################");
        Console.WriteLine("############ Testing the implementation on functions with known minima ############");
        Console.WriteLine("###################################################################################\n");

        int n_dim = 8;
        vector start_quadratic_nD = new vector(n_dim);
        for(int i=0; i < n_dim; i++) start_quadratic_nD[i] = 1.0;
        vector expected_quadratic_nD = new vector(n_dim);

        vector start_rosenbrock_nD = new vector(n_dim);
        vector expected_rosenbrock_nD = new vector(n_dim);
        for(int i=0; i < n_dim; i++) {
            start_rosenbrock_nD[i] = (i % 2 == 0) ? -1.2 : 1.0;
            expected_rosenbrock_nD[i] = 1.0;
        }

        var testProblems = new List<TestProblem>
        {
            new TestProblem { Title = "Quadratic function", Function = Problems.Quadratic, StartPoint = vector.Create(1.0, 1.0), ExpectedMinimum = vector.Create(0.0, 0.0) },
            new TestProblem { Title = "Rosenbrock's valley function", Function = Problems.Rosenbrock, StartPoint = vector.Create(-1.2, 1.0), ExpectedMinimum = vector.Create(1.0, 1.0) },
            new TestProblem { Title = "Himmelblau's function", Function = Problems.Himmelblau, StartPoint = vector.Create(0.0, 0.0), ExpectedMinimum = vector.Create(3.0, 2.0) },
            new TestProblem { Title = $"Quadratic function in {n_dim}D", Function = Problems.Quadratic_nD, StartPoint = start_quadratic_nD, ExpectedMinimum = expected_quadratic_nD },
            new TestProblem { Title = $"Rosenbrock's valley function in {n_dim}D", Function = Problems.Rosenbrock_nD, StartPoint = start_rosenbrock_nD, ExpectedMinimum = expected_rosenbrock_nD }
        };

        // Run all standard tests in a single loop.
        foreach (var problem in testProblems)
        {
            PrintBlock(problem.Title, problem.Function, problem.StartPoint, acc, problem.ExpectedMinimum);
        }
    }

    // ------------ Apply the implementation to more complicated problems ------------
    private static void RunComplexProblemTests(double acc)
    {
        Console.WriteLine("##################################################################################");
        Console.WriteLine("############ Applying the implementation to more complicated problems ############");
        Console.WriteLine("##################################################################################\n");
        RunHiggsFitTest(acc);
        RunAnnTrainingTest(acc);
    }

    // Higgs boson ...
    private static void RunHiggsFitTest(double acc)
    {
        vector higgs_start_params = vector.Create(125, 2, 10);
        
        var higgs_outputs = PrintBlock("Higgs Boson Fit", Problems.HiggsDeviation, higgs_start_params, acc);

        int best_method_index = (higgs_outputs[1].Stats.f_min < higgs_outputs[0].Stats.f_min) ? 1 : 0;
        var best_output = higgs_outputs[best_method_index];
        
        vector higgs_params_fit = best_output.Stats.x_min;
        matrix final_B = best_output.InverseHessian;

        using (var writer = new StreamWriter("higgs_data.txt")) {
            for(int i=0; i < Problems.energy.Length; i++) writer.WriteLine($"{Problems.energy[i]} {Problems.signal[i]} {Problems.error[i]}");
        }
        using (var writer = new StreamWriter("higgs_fit_curve.txt")) {
            double m = higgs_params_fit[0], Gamma = higgs_params_fit[1], A = higgs_params_fit[2];
            for (double E = 100; E <= 160; E += 0.25) {
                double F = A / ((E - m) * (E - m) + Gamma * Gamma / 4.0);
                writer.WriteLine($"{E} {F}");
            }
        }
        Console.WriteLine(">> Generated higgs_data.txt and higgs_fit_curve.txt for plotting.\n");

        try
        {
            double mass_uncertainty = Math.Sqrt(Math.Abs(final_B[0,0]));
            double width_uncertainty = Math.Sqrt(Math.Abs(final_B[1,1]));
            double amp_uncertainty = Math.Sqrt(Math.Abs(final_B[2,2]));

            Console.WriteLine("--- Final Fit Parameters with Estimated Uncertainties ---");
            Console.WriteLine($"(Uncertainties are estimated from the final inverse Hessian approximation)\n");
            Console.WriteLine($"Higgs Mass:      {higgs_params_fit[0]:F3} ± {mass_uncertainty:F3} GeV");
            Console.WriteLine($"Resonance Width: {higgs_params_fit[1]:F3} ± {width_uncertainty:F3}");
            Console.WriteLine($"Scale Factor:    {higgs_params_fit[2]:F2} ± {amp_uncertainty:F2}\n");
        }
        catch (Exception e) {
            Console.WriteLine($"Could not calculate uncertainties: {e.Message}\n");
        }
    }

    // AAN
    private static void RunAnnTrainingTest(double acc)
    {
        // 1. Setup
        Func<double, double> targetFunc = x => Math.Cos(5*x-1) * Math.Exp(-x*x);
        int num_neurons = 5;
        Func<vector, double> costFunction = (p) => {
            var ann = new Problems.Ann(p);
            double sumOfSquares = 0;
            for (int k = 0; k < 20; k++) {
                double xi = -1.0 + 2.0 * k / 19.0;
                sumOfSquares += Math.Pow(ann.Response(xi) - targetFunc(xi), 2);
            }
            return sumOfSquares;
        };

        // 2. Strategy
        int numberOfTries = 10;
        int maxIterations = 10000;
        var random = new Random();

        var start_points = new List<vector>();
        for (int i = 0; i < numberOfTries; i++) {
            vector ann_random_start = new vector(num_neurons * 3);
            for(int j = 0; j < num_neurons * 3; j++) {
                ann_random_start[j] = (random.NextDouble() - 0.5) * 4;
            }
            start_points.Add(ann_random_start);
        }

        Console.WriteLine($"------------ Artificial Neural Network Training ------------\n");
        Console.WriteLine($"Attempting {numberOfTries} random starts for each method to find a good minimum...\n");

        var methods = new[] { new { Name = "Broyden's update", UseSym = false }, new { Name = "Symmetrized Broyden's update", UseSym = true } };
        var best_results = new QuasiNewton.MinimizationOutput[methods.Length];
        var total_iterations = new long[methods.Length];
        var total_fevals = new long[methods.Length];
        var success_counts = new int[methods.Length]; 

        for(int i=0; i < methods.Length; i++)
        {
            var method = methods[i];
            double best_f_min_for_method = double.PositiveInfinity;
            QuasiNewton.MinimizationOutput best_output_for_method = new QuasiNewton.MinimizationOutput();
            int successful_runs = 0; 

            Console.WriteLine($"--- Running for: {method.Name} ---");
            foreach(var start_point in start_points)
            {
                var current_result = QuasiNewton.MinimizeReport(costFunction, start_point, acc, useSymmetrized: method.UseSym, maxIterations: maxIterations);
                total_iterations[i] += current_result.Stats.iterations;
                total_fevals[i] += current_result.Stats.fevals;
                
                if(current_result.Stats.iterations < maxIterations) successful_runs++;
                
                if (current_result.Stats.f_min < best_f_min_for_method) {
                    best_f_min_for_method = current_result.Stats.f_min;
                    best_output_for_method = current_result;
                }
            }
            best_results[i] = best_output_for_method;
            success_counts[i] = successful_runs;
            Console.WriteLine($"  >> Best found minimum value f(x_min) for this method: {best_f_min_for_method}\n");
        }

        // 3. Reporting and Comparison
        Console.WriteLine("\n--- Best overall results found after all attempts ---");
        for (int i = 0; i < methods.Length; i++)
        {
            var r = best_results[i].Stats; // Extract stats from the best run's output
            double avg_iter = (double)total_iterations[i] / numberOfTries;
            double avg_feval = (double)total_fevals[i] / numberOfTries;
            Console.WriteLine($"--- Results for {methods[i].Name} ---");
            Console.WriteLine($"Value at best minimum, f(x_min):          {r.f_min}");
            Console.WriteLine($"Average evaluations over {numberOfTries} runs:   {avg_feval:F2}");
            Console.WriteLine($"Success Rate:                             {success_counts[i]}/{numberOfTries} runs converged\n");
        }
        
        Console.WriteLine($"--- Broyden vs Symmetrized Broyden (for ANN) ---\n");
        Console.WriteLine($"Robustness (higher success rate is better):");
        Console.WriteLine($"  Broyden Success Rate:           {success_counts[0]}/{numberOfTries}");
        Console.WriteLine($"  Symmetrized Success Rate:       {success_counts[1]}/{numberOfTries}\n");
        
        double f_min_broyden = best_results[0].Stats.f_min;
        double f_min_sym = best_results[1].Stats.f_min;
        Console.WriteLine($"Quality of best minimum (lower is better):");
        Console.WriteLine($"  Broyden:           {f_min_broyden}");
        Console.WriteLine($"  Symmetrized:       {f_min_sym}\n");
        
        Console.WriteLine($"Average efficiency over {numberOfTries} runs (lower is better):");
        double avg_eval_broyden = (double)total_fevals[0] / numberOfTries;
        double avg_eval_sym = (double)total_fevals[1] / numberOfTries;
        Console.WriteLine($"  Avg. Evals Broyden:           {avg_eval_broyden:F2}");
        Console.WriteLine($"  Avg. Evals Symmetrized:       {avg_eval_sym:F2}\n");
        
        // --- Overall Verdict for ANN Training ---
        Console.WriteLine("Overall Verdict for ANN Training:");
        string annWinner = "Tie";
        int rb_successes = success_counts[0];
        int rs_successes = success_counts[1];

        if (rs_successes > rb_successes && rb_successes < numberOfTries * 0.5) {
             Console.WriteLine("The WINNER is Symmetrized Broyden's update due to its significantly higher success rate, making it more reliable.");
             annWinner = "Symmetrized Broyden's update";
        } else if (rb_successes > rs_successes && rs_successes < numberOfTries * 0.5) {
             Console.WriteLine("The WINNER is Broyden's update due to its significantly higher success rate, making it more reliable.");
             annWinner = "Broyden's update";
        } else if (f_min_sym < f_min_broyden && (f_min_broyden - f_min_sym) > 1e-9 * (f_min_broyden + f_min_sym)) {
             Console.WriteLine("The WINNER is Symmetrized Broyden's update, as it found a solution of significantly higher quality.");
             annWinner = "Symmetrized Broyden's update";
        } else if (f_min_broyden < f_min_sym && (f_min_sym - f_min_broyden) > 1e-9 * (f_min_broyden + f_min_sym)) {
             Console.WriteLine("The WINNER is Broyden's update, as it found a solution of significantly higher quality.");
             annWinner = "Broyden's update";
        } else if (avg_eval_sym < avg_eval_broyden * 0.95) {
             Console.WriteLine("With similar robustness and solution quality, Symmetrized Broyden's update wins by being more efficient on average.");
             annWinner = "Symmetrized Broyden's update";
        } else if (avg_eval_broyden < avg_eval_sym * 0.95) {
             Console.WriteLine("With similar robustness and solution quality, Broyden's update wins by being more efficient on average.");
             annWinner = "Broyden's update";
        } else {
             Console.WriteLine("Both methods showed a comparable performance on this complex task.");
             annWinner = "Tie";
        }
        Console.WriteLine();

        TallyOverallWinner(annWinner);

        // 4. Plotting
        vector ann_params_fit = (best_results[0].Stats.f_min < best_results[1].Stats.f_min) ? best_results[0].Stats.x_min : best_results[1].Stats.x_min;
        var trained_ann = new Problems.Ann(ann_params_fit);
        using (var writer = new StreamWriter("ann_approximation.txt")) {
            for (double x = -1.0; x <= 1.0; x += 0.02) {
                writer.WriteLine($"{x} {targetFunc(x)} {trained_ann.Response(x)}");
            }
        }
        Console.WriteLine(">> Generated ann_approximation.txt for plotting with the best overall parameters found.\n");
    }

    // Prints the final vudering af the performance of both methods across all tests.
    private static void PrintFinalConclusion()
    {
        Console.WriteLine("#############################################################################");
        Console.WriteLine("### Is the implementation of the symmetrized Broyden's update better?     ###");
        Console.WriteLine("#############################################################################\n");

        int broydenWins = overallScores["Broyden's update"];
        int symBroydenWins = overallScores["Symmetrized Broyden's update"];
        int ties = overallScores["Tie"];
        int totalTests = broydenWins + symBroydenWins + ties;

        Console.WriteLine($"--- Final Scorecard across {totalTests} tests ---\n");
        Console.WriteLine($"Symmetrized Broyden's update wins: {symBroydenWins}");
        Console.WriteLine($"Broyden's update wins:             {broydenWins}");
        Console.WriteLine($"Ties:                              {ties}\n");

        Console.WriteLine("--- Final Conclusion ---\n");

        if (symBroydenWins == totalTests) {
            Console.WriteLine("YES, the Symmetrized Broyden's update is better for all problems tested!");
        } else if (symBroydenWins > 0 && broydenWins == 0) {
            Console.WriteLine("YES, the Symmetrized Broyden's update is better. It performed better on some problems and was equally good on all others.");
        } else if (symBroydenWins > broydenWins) {
            Console.WriteLine("YES, the Symmetrized Broyden's update is better for most problems, although the standard Broyden's update was better in some cases.");
        } else if (broydenWins == totalTests) {
            Console.WriteLine("NO, surprisingly, the standard Broyden's update is better for all problems tested!");
        } else if (broydenWins > symBroydenWins) {
            Console.WriteLine("NO, surprisingly, the standard Broyden's update is better for most problems, although the Symmetrized Broyden's update was better in some cases.");
        } else if (ties == totalTests) {
            Console.WriteLine("NO, both methods are equally good for all problems tested.");
        } else {
            Console.WriteLine("NO, neither method is definitively better. Overall, both methods are equally good, as they each won the same number of tests.");
        }
        Console.WriteLine();
    }
}