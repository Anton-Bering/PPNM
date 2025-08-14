using System;
using System.IO;
using System.Collections.Generic;
using static functions;
using static Counter;

class Program
{
    static void Main()
    {
        /* ---------- TASK A ---------- */
        feval = 0; // tæller funktionskald (se counter.cs)
        var startR = new vector(2.0, 2.0);
        var resRosFwd = Minimizer.Minimize(Rosenbrock, startR, 1e-6, Minimizer.DiffScheme.Forward);
        var xRosFwd = resRosFwd.xMin;
        var stepsRosFwd = resRosFwd.steps;
        int evalsRosFwd = feval;

        feval = 0;
        var startH = new vector(2.0, 2.0);
        var resHimFwd = Minimizer.Minimize(Himmelblau, startH, 1e-6, Minimizer.DiffScheme.Forward);
        var xHimFwd = resHimFwd.xMin;
        var stepsHimFwd = resHimFwd.steps;
        int evalsHimFwd = feval;

        /* ---------- TASK B ---------- */
        var E = new List<double>();
        var s = new List<double>();
        var ds = new List<double>();
        string line;
        char[] sep = { ' ', '\t' };
        while ((line = Console.In.ReadLine()) != null)
        {
            if (line.Trim().Length == 0 || line.Trim()[0] == '#') continue;
            var w = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            E.Add(double.Parse(w[0]));
            s.Add(double.Parse(w[1]));
            ds.Add(double.Parse(w[2]));
        }

        Func<vector, double> D = p => Deviation(p, E, s, ds);
        var p0 = new vector(125.0, 2.0, 10.0);

        feval = 0; // tæller også for Task B
        var resFit = Minimizer.Minimize(D, p0, 1e-6, Minimizer.DiffScheme.Central);
        var pFit = resFit.xMin;
        var stepFit = resFit.steps;
        int evalsFit = feval;

        double mH = pFit[0], GammaH = pFit[1], AH = pFit[2];

        // Skriv Higgs_fit.txt til plot
        double Emin = E[0], Emax = E[E.Count - 1];
        WriteFitFile("Higgs_fit.txt", mH, GammaH, AH, Emin, Emax);

        /* ---------- TASK C ---------- */
        feval = 0;
        var resRosCtr = Minimizer.Minimize(Rosenbrock, startR, 1e-6, Minimizer.DiffScheme.Central);
        var xRosCtr = resRosCtr.xMin;
        var stepsRosCtr = resRosCtr.steps;
        int evalsRosCtr = feval;

        feval = 0;
        var resHimCtr = Minimizer.Minimize(Himmelblau, startH, 1e-6, Minimizer.DiffScheme.Central);
        var xHimCtr = resHimCtr.xMin;
        var stepsHimCtr = resHimCtr.steps;
        int evalsHimCtr = feval;

        // ---------- Skriv Out.txt ----------
        using (var OUT = new StreamWriter("Out.txt"))
        {
            OUT.WriteLine("------------ TASK A: Newton's method with numerical gradient,      ------------");
            OUT.WriteLine("------------ numerical Hessian matrix and back-tracking linesearch ------------\n");

            OUT.WriteLine("------ Find a minimum of the Rosenbrock's valley function ------\n");
            OUT.WriteLine("The Rosenbrock's valley function: f(x,y) = (1-x)^2 + 100(y-x^2)^2\n");
            OUT.WriteLine($"Minimum: (x = {xRosFwd[0]:f6}, y = {xRosFwd[1]:f6})\n");

            OUT.WriteLine("------ Find a minimum of the Himmelblau's function ------\n");
            OUT.WriteLine("The Himmelblau's function: f(x,y) = (x^2+y-11)^2 + (x+y^2-7)^2\n");
            OUT.WriteLine($"Minimum: (x = {xHimFwd[0]:f6}, y = {xHimFwd[1]:f6})\n");

            OUT.WriteLine("------ Record the number of steps it takes for the algorithm to reach the minimum ------\n");
            OUT.WriteLine($"Steps for the Rosenbrock's valley function: {stepsRosFwd}");
            OUT.WriteLine($"Steps for the Himmelblau's function:       {stepsHimFwd}\n");

            OUT.WriteLine("------------ TASK B: Higgs boson discovery ------------\n");
            OUT.WriteLine("cross_section_experiment.txt contain the cross-section data from the experiment involving Higgs.\n");
            OUT.WriteLine("------ Fit the Breit-Wigner function to the data ------");
            OUT.WriteLine("--- Determine the mass and the width of the Higgs boson ---\n");
            OUT.WriteLine("By fitting to the Breit-Wigner function one finds:");
            OUT.WriteLine($"mass  : {mH:f3} GeV");
            OUT.WriteLine($"width : {GammaH:f3} GeV");
            OUT.WriteLine($"A     : {AH:f3}\n");

            OUT.WriteLine("--- Fit diagnostics ---");
            OUT.WriteLine($"Newton steps      : {stepFit}");
            OUT.WriteLine($"Function evals    : {evalsFit}\n");

            OUT.WriteLine("--- Plot fit together with the experimental data ---");
            OUT.WriteLine("Higgs_fit.txt contain the fitting data.");
            OUT.WriteLine("Higgs_fit_and_experimental_data.svg contain the plot of fitting data and the experimental data\n");

            OUT.WriteLine("------------ TASK C: Central instead of forward finite difference approximation for the derivatives ------------\n");

            OUT.WriteLine("------ Find a minimum of the Rosenbrock's valley function ------\n");
            OUT.WriteLine($"Minimum: (x = {xRosCtr[0]:f6}, y = {xRosCtr[1]:f6})\n");

            OUT.WriteLine("------ Find a minimum of the Himmelblau's function ------\n");
            OUT.WriteLine($"Minimum: (x = {xHimCtr[0]:f6}, y = {xHimCtr[1]:f6})\n");

            OUT.WriteLine("------ Record the number of steps it takes for the algorithm to reach the minimum ------\n");
            OUT.WriteLine($"Steps for the Rosenbrock's valley function: {stepsRosCtr}");
            OUT.WriteLine($"Steps for the Himmelblau's function:       {stepsHimCtr}\n");

            OUT.WriteLine("------ compare the resulting algorithm with the one in Part A ------\n");

            OUT.WriteLine("--- Rosenbrock's valley function ---\n");
            OUT.WriteLine($"    Minimum          : ({xRosFwd[0]:f6}, {xRosFwd[1]:f6})  vs  ({xRosCtr[0]:f6}, {xRosCtr[1]:f6})");
            OUT.WriteLine($"    Newton steps     : {stepsRosFwd}  vs  {stepsRosCtr}");
            OUT.WriteLine($"    Function evals   : {evalsRosFwd} vs {evalsRosCtr}");
            OUT.WriteLine($"    Conclusion       : {(evalsRosFwd < evalsRosCtr ? "The forward difference approximation is cheaper" : evalsRosFwd > evalsRosCtr ? "The central difference approximation is cheaper" : "same cost")}\n");

            OUT.WriteLine("--- Himmelblau's function ---\n");
            OUT.WriteLine($"    Minimum          : ({xHimFwd[0]:f6}, {xHimFwd[1]:f6})  vs  ({xHimCtr[0]:f6}, {xHimCtr[1]:f6})");
            OUT.WriteLine($"    Newton steps     : {stepsHimFwd}  vs  {stepsHimCtr}");
            OUT.WriteLine($"    Function evals   : {evalsHimFwd} vs {evalsHimCtr}");
            OUT.WriteLine($"    Conclusion       : {(evalsHimFwd < evalsHimCtr ? "The forward difference approximation is cheaper" : evalsHimFwd > evalsHimCtr ? "The central difference approximation is cheaper" : "same cost")}\n");
        }

        Console.Error.WriteLine("Finished. Results written to Out.txt and Higgs_fit.txt");
    }
}
