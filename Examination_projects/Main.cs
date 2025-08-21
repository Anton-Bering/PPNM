using System;
using PPNM.Minimization;
using System.IO;

static class V {
    public static vector vec(params double[] a){
        var v = new vector(a.Length);
        for(int i=0;i<a.Length;i++) v[i]=a[i];
        return v;
    }
}

class Program {
    static void PrintBlock(
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
        Console.WriteLine($"--- Broyden's update vs Symmetrized Broyden's update ---");
        Console.WriteLine($"speedup ≡ Broyden / Sym-Broyden, ( >1 mean Sym-Broyden is more efficient)");
        var rb = results[0];
        var rs = results[1];
        double spIter = (rs.iterations > 0) ? (double)rb.iterations / Math.Max(1, rs.iterations) : double.PositiveInfinity;
        double spEval = (rs.fevals     > 0) ? (double)rb.fevals     / rs.fevals                  : double.PositiveInfinity;
        Console.WriteLine($"Iterations speedup: {spIter}");
        Console.WriteLine($"Evaluations speedup: {spEval}");
        Console.WriteLine();
    }

    static void Main(){
        double acc = 1e-6;

        // ------------ TASK: Test your implementation on some functions with known minima ------------
        Console.WriteLine("------------ Test the implementation on some functions with known minima ------------\n");
        
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
        Console.WriteLine($"------------ Test the implementation in higher dimensions {n_dim} ------------\n");

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
            // Rettet: Tilføjet det kendte minimum
            expected_x_min: expected_rosenbrock_nD
        );

        // ------------ TASK: Apply your implementation to a more complicated problems ------------
        Console.WriteLine("------------ Applying the implementation to a more complicated problems ------------\n");

        // Higgs Boson Fit ---------------------------------- start ---
        Console.WriteLine("I use the data from the Higgs experiment (CERN 2012) we were given in homework exercise 9.");
        Console.WriteLine("The data is fitted to the Breit-Wigner function, by minimizing the deviation function.");
        Console.WriteLine("The first component of the minimum corresponds to the Higgs mass in GeV (the mass should be ~125.3 GeV).");
        Console.WriteLine("The second and third components are the resonance width and the scale factor, respectively, for the given experiment.\n");
        
        // Kør og print den fulde analyse ÉN GANG
        vector higgs_start_params = V.vec(125, 2, 10);
        PrintBlock(
            "Higgs Boson Fit", 
            Problems.HiggsDeviation, 
            higgs_start_params, 
            acc);

        // plot: Kør kun den bedste metode (symmetrisk) ÉN GANG for at få parametrene til plottet
        var higgs_result_sym_broyden = QuasiNewton.MinimizeReport(Problems.HiggsDeviation, higgs_start_params, acc, useSymmetrized: true);
        vector higgs_params_fit = higgs_result_sym_broyden.x_min;

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
        Console.WriteLine(">> Generated higgs_data.txt and higgs_fit_curve.txt for plotting.\n");
        // Higgs Boson Fit -------------------------------------------------- end ---



        // ANN -------------------------------------------------------------- start ---
        Console.WriteLine("This section trains a simple Artificial Neural Network to approximate a function.");
        Console.WriteLine("The problem is complex and can have many local minima. We will try multiple random starts to find a better solution.\n");
        Console.WriteLine("Target function to approximate: g(x) = Cos(5x-1) * Exp(-x²)\n");

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

        // 2. Strategi: Kør minimeringen flere gange med tilfældige startpunkter
        int numberOfTries = 10; // Prøv 10 forskellige startpunkter
        QuasiNewton.Result best_result = new QuasiNewton.Result();
        double best_f_min = double.PositiveInfinity;
        var random = new Random();

        Console.WriteLine($"Attempting {numberOfTries} random starts to find a good minimum...");

        for (int i = 0; i < numberOfTries; i++)
        {
            // Generer en NY tilfældig start-vektor for hver kørsel
            vector ann_random_start = new vector(num_neurons * 3);
            for(int j = 0; j < num_neurons * 3; j++)
            {
                // Tilfældige tal mellem -2 og 2 giver ofte gode resultater
                ann_random_start[j] = (random.NextDouble() - 0.5) * 4;
            }

            // Kør minimeringen med den symmetriske metode (den bedste)
            var current_result = QuasiNewton.MinimizeReport(costFunction, ann_random_start, acc, useSymmetrized: true);

            Console.WriteLine($"  Try {i+1}/{numberOfTries}: Found a minimum with f(x_min) = {current_result.f_min}");

            // Hvis dette resultat er bedre end det bedste, vi har set hidtil, så gem det
            if (current_result.f_min < best_f_min)
            {
                best_f_min = current_result.f_min;
                best_result = current_result;
                Console.WriteLine($"    >> This is the new best result!");
            }
        }

        Console.WriteLine("\n--- Best result found after all attempts ---");
        Console.WriteLine($"Found minimum (x_min):                    {best_result.x_min}");
        Console.WriteLine($"Value at minimum, f(x_min):               {best_result.f_min}");
        Console.WriteLine($"Gradient norm at x_min:                   {best_result.gradNorm}");
        Console.WriteLine($"Number of iterations:                     {best_result.iterations}");
        Console.WriteLine();


        // 3. plot: Brug parametrene fra det BEDSTE resultat
        vector ann_params_fit = best_result.x_min;
        var trained_ann = new Problems.Ann(ann_params_fit);
        using (var writer = new StreamWriter("ann_approximation.txt")) {
            for (double x = -1.0; x <= 1.0; x += 0.02) {
                writer.WriteLine($"{x} {targetFunc(x)} {trained_ann.Response(x)}");
            }
        }
        Console.WriteLine(">> Generated ann_approximation.txt for plotting with the best parameters found.\n");
        // ANN -------------------------------------------------------------- end ---

        Console.WriteLine("Done.");
    }
}