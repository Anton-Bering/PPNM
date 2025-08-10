using System;
using System.IO;
using System.Collections.Generic;
using static functions;
using static Counter;

class Program
{
	static void Main()
	{
		/* ---------- TASK A ---------- */
		feval = 0;  // Add for task C
		var (xRosFwd,stepsRosFwd) =
			newton.minimize(Rosenbrock,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Forward);
		int evalsRosFwd = feval; // Add for task C

		feval = 0;  // Add for task C
		var (xHimFwd,stepsHimFwd) =
			newton.minimize(Himmelblau,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Forward);
		int evalsHimFwd = feval; // Add for task C

		/* ---------- TASK B -------- */
		var E  = new List<double>();
		var s  = new List<double>();
		var ds = new List<double>();
		string line;
		char[] sep={' ','\t'};
		while((line=Console.In.ReadLine())!=null){
			if(line.Trim().Length==0 || line.Trim()[0]=='#') continue;
			var w = line.Split(sep,StringSplitOptions.RemoveEmptyEntries);
			E .Add(double.Parse(w[0]));
			s .Add(double.Parse(w[1]));
			ds.Add(double.Parse(w[2]));
		}

		Func<vector,double> D = p => Deviation(p,E,s,ds);
		var start = new vector(125.0,2.0,10.0);          
		var (pFit,stepFit) = newton.minimize(D,start,scheme:DiffScheme.Forward);
		double mH = pFit[0], GammaH = pFit[1], AH = pFit[2];

		// skriv Higgs_fit.txt
		double Emin=E[0], Emax=E[E.Count-1];
		WriteFitFile("Higgs_fit.txt",mH,GammaH,AH,Emin,Emax);

		/* ---------- TASK C ------- */
		feval = 0; // Add for at samelinge med task A
		var (xRosCtr,stepsRosCtr) =
			newton.minimize(Rosenbrock,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Central);
		int evalsRosCtr = feval; // Add for at samelinge med task A

		feval = 0; // Add for at samelinge med task A
		var (xHimCtr,stepsHimCtr) =
			newton.minimize(Himmelblau,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Central);
		int evalsHimCtr = feval; // Add for at samelinge med task A

		// SKrive Out.txt:
		using(var OUT = new StreamWriter("Out.txt")){
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
			OUT.WriteLine("------ Fit the Breit-Wigner function to the data ------\n");
			// OUT.WriteLine("The Breit-Wigner function is: F(E|m,Γ,A) = A/[(E-m)^2 + Γ^2/4]");
			// OUT.WriteLine("and the deviation function is: D(m,Γ,A) = Σ_i[ (F(E_i|m,Γ,A)-σ_i)/Δσ_i ]^2\n");

			OUT.WriteLine("--- Determine the mass and the width of the Higgs boson ------\n");
			OUT.WriteLine($"By fitting to the Breit-Wigner function one finds:");
			OUT.WriteLine($"mass  : {mH:f3} GeV");
			OUT.WriteLine($"width : {GammaH:f3} GeV");
			// OUT.WriteLine($"A     : {AH:f3}\n");

			OUT.WriteLine("--- Plot fit together with the experimental data ---\n");
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
			OUT.WriteLine($"    Minimum        	: ({xRosFwd[0]:f6}, {xRosFwd[1]:f6})  vs  ({xRosCtr[0]:f6}, {xRosCtr[1]:f6})");
			OUT.WriteLine($"    Newton steps        : {stepsRosFwd}  vs  {stepsRosCtr}");
			OUT.WriteLine($"    Function evaluations: {evalsRosFwd} vs {evalsRosCtr}");
			OUT.WriteLine($"    Conclusion          : {(evalsRosFwd<evalsRosCtr ? "The forward difference approximations is cheaper" :
			                           evalsRosFwd>evalsRosCtr ? "The central difference approximation is cheaper" : "same cost")}\n");

			OUT.WriteLine("--- Himmelblau's function ---\n");
			OUT.WriteLine($"    Minimum        	: ({xHimFwd[0]:f6}, {xHimFwd[1]:f6})  vs  ({xHimCtr[0]:f6}, {xHimCtr[1]:f6})");
			OUT.WriteLine($"    Newton steps        : {stepsHimFwd}  vs  {stepsHimCtr}");
			OUT.WriteLine($"    Function evaluations: {evalsHimFwd} vs {evalsHimCtr}");
			OUT.WriteLine($"    Conclusion          : {(evalsHimFwd<evalsHimCtr ? "The forward difference approximations is cheaper" :
			                           evalsHimFwd>evalsHimCtr ? "The central difference approximation is cheaper" : "same cost")}\n");
		}

		Console.Error.WriteLine("Finished.  Results written to Out.txt and Higgs_fit.txt");
	}
}
