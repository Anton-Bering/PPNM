// --- Newton minimerser ---
using System;

public enum DiffScheme { Forward , Central }

public static class newton
{
	
	static vector gradient(Func<vector,double> f, vector x, DiffScheme scheme)
	{
		int n=x.size;
		var g=new vector(n);
		double fx=f(x);
		for(int i=0;i<n;i++){
			double dx=(1+Math.Abs(x[i]))*Math.Pow(2,-26);
			if(scheme==DiffScheme.Forward){
				x[i]+=dx;
				g[i]=(f(x)-fx)/dx;
				x[i]-=dx;
			}else{
				x[i]+=dx;   double fplus=f(x);
				x[i]-=2*dx; double fminus=f(x);
				x[i]+=dx;   g[i]=(fplus-fminus)/(2*dx);
			}
		}
		return g;
	}

	
	static matrix hessian(Func<vector,double> f,vector x,DiffScheme scheme)
	{
		int n=x.size;
		var H=new matrix(n);
		var g0=gradient(f,x,scheme);
		for(int j=0;j<n;j++){
			double dx=(1+Math.Abs(x[j]))*Math.Pow(2,-13);
			x[j]+=dx;
			var g1=gradient(f,x,scheme);
			x[j]-=dx;
			for(int i=0;i<n;i++) H[i,j]=(g1[i]-g0[i])/dx;
		}
		return H;
	}

	
	public static (vector,int) minimize(
		Func<vector,double> f,
		vector start,
		double acc = 1e-3,
		int    maxsteps = 1000,
		DiffScheme scheme = DiffScheme.Forward )
	{
		var x = new vector(start.data);   // copy
		int step;
		for(step=0; step<maxsteps; step++){
			var g = gradient(f,x,scheme);
			if(g.norm()<acc) break;
			var H = hessian(f,x,scheme);
			var dx = qrgs.solve(H,-1*g);
			double λ = 1.0;
			while(λ>=1.0/1024){
				var xtrial = x + λ*dx;
				if(f(xtrial) < f(x)) { x=xtrial; break; }
				λ/=2;
			}
		}
		return (x,step);
	}
}
