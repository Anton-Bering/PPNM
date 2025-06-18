using System;
using System.IO;
using System.Collections.Generic;
using static functions;

class Program
{
	static void Main()
	{
		/* ---------- TASK A ---------- */
		var (xRosFwd,stepsRosFwd) =
			newton.minimize(Rosenbrock,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Forward);
		var (xHimFwd,stepsHimFwd) =
			newton.minimize(Himmelblau,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Forward);

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
		var (xRosCtr,stepsRosCtr) =
			newton.minimize(Rosenbrock,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Central);
		var (xHimCtr,stepsHimCtr) =
			newton.minimize(Himmelblau,new vector( 2.0, 2.0 ),
			                scheme:DiffScheme.Central);

		// SKrive Out.txt:
		using(var OUT = new StreamWriter("Out.txt")){
			OUT.WriteLine("------------ TASK A: Newton's method with numerical gradient ------------");
			OUT.WriteLine("------------ And numerical Hessian matrix ------------");
			OUT.WriteLine("------------ And back-tracking linesearch ------------\n");

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
			OUT.WriteLine("------ compare the resulting algorithm with the one in Part A ------\n");

			OUT.WriteLine("--- Rosenbrock's valley function ---\n");
			OUT.WriteLine($"With forward difference (Task A):   minimum at ({xRosFwd[0]:f6}, {xRosFwd[1]:f6}) in {stepsRosFwd} steps");
			OUT.WriteLine($"With central difference (Task C):   minimum at ({xRosCtr[0]:f6}, {xRosCtr[1]:f6}) in {stepsRosCtr} steps\n");

			OUT.WriteLine("--- Himmelblau's function ---\n");
			OUT.WriteLine($"With forward difference (Task A):   minimum at ({xHimFwd[0]:f6}, {xHimFwd[1]:f6}) in {stepsHimFwd} steps");
			OUT.WriteLine($"With central difference (Task C):   minimum at ({xHimCtr[0]:f6}, {xHimCtr[1]:f6}) in {stepsHimCtr} steps\n");
		}

		Console.Error.WriteLine("Finished.  Results written to Out.txt and Higgs_fit.txt");
	}
}
