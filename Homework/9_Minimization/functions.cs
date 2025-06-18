using System;
using System.Collections.Generic;
using static Counter;

public static class functions
{
	// Rosenbrock 
	public static double Rosenbrock(vector v)
	{
		feval++; // Add for task C
		double x=v[0], y=v[1];
		return (1-x)*(1-x)+100*Math.Pow(y-x*x,2);
	}

	// Himmelblau
	public static double Himmelblau(vector v)
	{
		feval++; // Add for task C
		double x=v[0], y=v[1];
		return Math.Pow(x*x+y-11,2)+Math.Pow(x+y*y-7,2);
	}

	
	public static double BreitWigner(double E,double m,double G,double A)
		=> A/((E-m)*(E-m)+G*G/4);

	
	public static double Deviation(vector p,List<double> E,List<double> s,List<double> ds)
	{
		feval++; // Add for task C
		double m=p[0], G=p[1], A=p[2];
		if(G<=0||A<=0) return 1e300;               
		double sum=0;
		for(int i=0;i<E.Count;i++){
			double F = BreitWigner(E[i],m,G,A);
			sum += Math.Pow((F-s[i])/ds[i],2);
		}
		return sum;
	}

	
	public static void WriteFitFile(string filename,double m,double G,double A,double Emin,double Emax)
	{
		using(var file = new System.IO.StreamWriter(filename)){
			int N=601;
			double step=(Emax-Emin)/(N-1);
			for(int i=0;i<N;i++){
				double E=Emin+i*step;
				file.WriteLine($"{E,8:f3} {BreitWigner(E,m,G,A),12:g6}");
			}
		}
	}
}
