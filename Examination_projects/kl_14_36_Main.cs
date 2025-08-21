using System;
using PPNM.Minimization;
using System.IO;
using System.Collections.Generic;

static class V {
    public static vector vec(params double[] a){
        var v = new vector(a.Length);
        for(int i=0;i<a.Length;i++) v[i]=a[i];
        return v;
    }
}

class Program {
    // ÆNDRING: Funktionens signatur er ændret til at returnere en array af resultater
    static QuasiNewton.Result[] PrintBlock(
        string title, 
        Func<vector,double> f, 
        vector start, 
        double acc, 
        vector expected_x_min = null)
    {
        var methods = new[]
        {
            new { Name = "Broyden update", UseSym = false },
            new { Name = "Symmetrized Broyden update", UseSym = true  }
        };

        var results = new QuasiNewton.Result[methods.Length];
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
            var r = results[i];
            Console.WriteLine($"--- Results for {methods[i].Name} ---");
            Console.WriteLine($"Found minimum (x_min):                    {r.x_min}");
            // Nogle f(x_min) er ikke 0 (f.eks. Higgs), så kommentaren er gjort mere generel
            Console.WriteLine($"Value at minimum, f(x_min):               {r.f_min}"); 
            Console.WriteLine($"Gradient norm at x_min:                   {r.gradNorm}  (should be < acc)");
            Console.WriteLine($"Number of iterations:                     {r.iterations}  (lower is better)");
            Console.WriteLine($"Number of resets:                         {r.resets}  (lower is better)");
            Console.WriteLine($"Length of last accepted step (λ_last):    {r.last_step}  (ideally ≈ 1)");
            Console.WriteLine($"Number of evaluations:                    {r.fevals}  (lower is better)\n");
            if (expected_x_min != null)
            {
                bool agrees = (r.x_min - expected_x_min).Norm() < acc;
                string YES_or_NO = agrees ? "YES" : "NO";
                Console.WriteLine($"Does the found minimum agree with expectation? {YES_or_NO}.\n");
            }
            

        }


        // Broyden vs Sym-Broyden
        Console.WriteLine($"--- Broyden's update vs Symmetrized Broyden's update ---\n");
        bool sameMinimum = (results[0].x_min - results[1].x_min).Norm() < acc;
        Console.WriteLine($"Did both methods find the same minimum? {(sameMinimum ? "YES" : "NO")}.\n");
        Console.WriteLine($"speedup ≡ Broyden / Sym-Broyden, ( >1 mean Sym-Broyden is more efficient)");
        var rb = results[0];
        var rs = results[1];
        double spIter = (rs.iterations > 0) ? (double)rb.iterations / Math.Max(1, rs.iterations) : double.PositiveInfinity;
        double spEval = (rs.fevals     > 0) ? (double)rb.fevals     / rs.fevals                  : double.PositiveInfinity;
        Console.WriteLine($"Iterations speedup: {spIter}");
        Console.WriteLine($"Evaluations speedup: {spEval}");
        Console.WriteLine();
        
        // ÆNDRING: Returner resultaterne til den, der kalder funktionen
        return results;
    }

    static void Main(){
        double acc = 1e-6;

        // ------------ TASK: Test your implementation on some functions with known minima ------------
        Console.WriteLine("############ Test the implementation on some functions with known minima ############\n");
        
        // Quadratic function
        PrintBlock(
            "Quadratic function", 
            Problems.Quadratic, 
            V.vec(1.0, 1.0), 
            acc, 
            expected_x_min: V.vec(0.0, 0.0));
        // Rosenbrock's valley function
        PrintBlock(
            "Rosenbrock's valley function",
            Problems.Rosenbrock,  
            V.vec(-1.2, 1.0), 
            acc, 
            expected_x_min: V.vec(1.0, 1.0));
        // Quadratic function
        PrintBlock(
            "Himmelblau's function",
            Problems.Himmelblau,  
            V.vec( 0.0, 0.0), 
            acc, 
            expected_x_min: 
            V.vec(3.0, 2.0));

        // ------------ TASK: Test implementation in higher dimensions ------------
        int n_dim = 8;
        Console.WriteLine($"############ Test the implementation in higher dimensions ({n_dim}D) ############\n");

        // Quadratic function in nD
        vector start_quadratic_nD = new vector(n_dim);
        vector expected_quadratic_nD = new vector(n_dim);
        for(int i=0; i < n_dim; i++) {
            start_quadratic_nD[i] = 1.0;
            expected_quadratic_nD[i] = 0.0;
        }
        PrintBlock(
            $"Quadratic function in {n_dim}D", 
            Problems.Quadratic_nD, 
            start_quadratic_nD, 
            acc, 
            expected_x_min: expected_quadratic_nD
        );

        // Rosenbrock's valley function in nD
        vector start_rosenbrock_nD = new vector(n_dim);
        vector expected_rosenbrock_nD = new vector(n_dim);
        for(int i=0; i < n_dim; i++) {
            start_rosenbrock_nD[i] = (i % 2 == 0) ? -1.2 : 1.0;
            expected_rosenbrock_nD[i] = 1.0;
        }

        PrintBlock(
            $"Rosenbrock's valley function in {n_dim}D",
            Problems.Rosenbrock_nD,  
            start_rosenbrock_nD,
            acc,
            expected_x_min: expected_rosenbrock_nD
        );

        // ------------ TASK: Apply your implementation to a more complicated problems ------------
        Console.WriteLine("############ Applying the implementation to a more complicated problems ############\n");

        // Higgs Boson Fit ---------------------------------- start ---
        
        vector higgs_start_params = V.vec(125, 2, 10);
        
        // Kør begge metoder og fang resultaterne
        var higgs_results = PrintBlock(
            "Higgs Boson Fit", 
            Problems.HiggsDeviation, 
            higgs_start_params, 
            acc);

        // plot: Vælg det bedste resultat fra sammenligningen til at lave plottet
        vector higgs_params_fit = (higgs_results[0].f_min < higgs_results[1].f_min) 
                                    ? higgs_results[0].x_min 
                                    : higgs_results[1].x_min;

        using (var writer = new StreamWriter("higgs_data.txt")) {
            for(int i=0; i < Problems.energy.Length; i++)
                writer.WriteLine($"{Problems.energy[i]} {Problems.signal[i]} {Problems.error[i]}");
        }
        using (var writer = new StreamWriter("higgs_fit_curve.txt")) {
            double m = higgs_params_fit[0], Gamma = higgs_params_fit[1], A = higgs_params_fit[2];
            for (double E = 100; E <= 160; E += 0.25) {
                double F = A / ((E - m) * (E - m) + Gamma * Gamma / 4.0);
                writer.WriteLine($"{E} {F}");
            }
        }
        Console.WriteLine(">> Generated higgs_data.txt and higgs_fit_curve.txt for plotting with the best parameters found.\n");
        
        // Higgs Boson Fit -------------------------------------------------- end ---

        // ANN -------------------------------------------------------------- start ---

        // 1. Opsætning (samme som før)
        Func<double, double> targetFunc = x => Math.Cos(5*x-1) * Math.Exp(-x*x);
        int num_neurons = 5;
        Func<vector, double> costFunction = (p) => {
            var ann = new Problems.Ann(p);
            double sumOfSquares = 0;
            for (int k = 0; k < 20; k++) {
                double xi = -1.0 + 2.0 * k / (19);
                sumOfSquares += Math.Pow(ann.Response(xi) - targetFunc(xi), 2);
            }
            return sumOfSquares;
        };

        // 2. Strategi: Kør minimeringen flere gange fra de samme tilfældige startpunkter
        int numberOfTries = 10; // Prøv 10 forskellige startpunkter
        var random = new Random();

        // Generer ét sæt af startpunkter, der skal bruges for BEGGE metoder for en fair sammenligning
        var start_points = new List<vector>();
        for (int i = 0; i < numberOfTries; i++) {
            vector ann_random_start = new vector(num_neurons * 3);
            for(int j = 0; j < num_neurons * 3; j++) {
                ann_random_start[j] = (random.NextDouble() - 0.5) * 4;
            }
            start_points.Add(ann_random_start);
        }

        Console.WriteLine($"------------ Artificial Neural Network Training ------------\n");
        Console.WriteLine($"Attempting {numberOfTries} random starts for each method (Broyden update & Symmetrized Broyden update) to find a good minimum...\n");

        var methods = new[]
        {
            new { Name = "Broyden update", UseSym = false },
            new { Name = "Symmetrized Broyden update", UseSym = true  }
        };

        var best_results = new QuasiNewton.Result[methods.Length];
        var total_iterations = new long[methods.Length];
        var total_fevals = new long[methods.Length];

        // Kør multi-start proceduren for hver metode
        for(int i=0; i < methods.Length; i++)
        {
            var method = methods[i];
            double best_f_min_for_method = double.PositiveInfinity;
            QuasiNewton.Result best_result_for_method = new QuasiNewton.Result();

            Console.WriteLine($"--- Running for: {method.Name} ---");

            foreach(var start_point in start_points)
            {
                var current_result = QuasiNewton.MinimizeReport(costFunction, start_point, acc, useSymmetrized: method.UseSym);
                total_iterations[i] += current_result.iterations;
                total_fevals[i] += current_result.fevals;

                if (current_result.f_min < best_f_min_for_method)
                {
                    best_f_min_for_method = current_result.f_min;
                    best_result_for_method = current_result;
                }
            }
            best_results[i] = best_result_for_method;
            Console.WriteLine($"  >> Best found minimum value f(x_min) for this method: {best_f_min_for_method}\n");
        }


        // 3. Rapportering og sammenligning
        Console.WriteLine("\n--- Best overall results found after all attempts ---");

        for (int i = 0; i < methods.Length; i++)
        {
            var r = best_results[i];
            double avg_iter = (double)total_iterations[i] / numberOfTries;
            double avg_feval = (double)total_fevals[i] / numberOfTries;

            Console.WriteLine($"--- Results for {methods[i].Name} ---");
            Console.WriteLine($"Best found minimum (x_min):               {r.x_min}");
            Console.WriteLine($"Value at best minimum, f(x_min):          {r.f_min}");
            Console.WriteLine($"Gradient norm at best x_min:              {r.gradNorm}");
            Console.WriteLine($"Iterations for the best run:              {r.iterations}");
            Console.WriteLine($"Average iterations over {numberOfTries} runs:    {avg_iter:F2}");
            Console.WriteLine($"Average evaluations over {numberOfTries} runs:   {avg_feval:F2}\n");
        }
        
        // Broyden vs Sym-Broyden for ANN
        Console.WriteLine($"--- Broyden's update vs Symmetrized Broyden's update (for ANN) ---\n");
        // Sammenlign kvaliteten af det bedste fundne minimum
        double f_min_broyden = best_results[0].f_min;
        double f_min_sym = best_results[1].f_min;
        Console.WriteLine($"Quality of best minimum (lower is better):");
        Console.WriteLine($"  Broyden:           {f_min_broyden}");
        Console.WriteLine($"  Symmetrized:       {f_min_sym}");
        Console.WriteLine($"  Sym-Broyden was better by: {f_min_broyden - f_min_sym}\n");

        // Sammenlign gennemsnitlig effektivitet
        Console.WriteLine($"Average efficiency over {numberOfTries} runs (lower is better):");
        double avg_eval_broyden = (double)total_fevals[0] / numberOfTries;
        double avg_eval_sym = (double)total_fevals[1] / numberOfTries;
        Console.WriteLine($"  Avg. Evals Broyden:           {avg_eval_broyden:F2}");
        Console.WriteLine($"  Avg. Evals Symmetrized:       {avg_eval_sym:F2}");
        
        double speedup = avg_eval_broyden / avg_eval_sym;
        Console.WriteLine($"\nEfficiency speedup ≡ AvgEvals(Broyden) / AvgEvals(Sym-Broyden)");
        Console.WriteLine($" ( > 1 means Symmetrized Broyden is more efficient on average )");
        Console.WriteLine($"Speedup: {speedup:F2}\n");


        // 4. plot: Brug parametrene fra det BEDSTE resultat (uanset metode)
        vector ann_params_fit = (best_results[0].f_min < best_results[1].f_min) ? best_results[0].x_min : best_results[1].x_min;
        var trained_ann = new Problems.Ann(ann_params_fit);
        using (var writer = new StreamWriter("ann_approximation.txt")) {
            for (double x = -1.0; x <= 1.0; x += 0.02) {
                writer.WriteLine($"{x} {targetFunc(x)} {trained_ann.Response(x)}");
            }
        }
        Console.WriteLine(">> Generated ann_approximation.txt for plotting with the best overall parameters found.\n");
        // ANN -------------------------------------------------------------- end ---

        Console.WriteLine("Done.");
    }
}