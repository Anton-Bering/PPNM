using System;

public static class Minimizer {
	public enum DiffScheme { Forward, Central }

	// --- offentlige API'er ---
	public static vector Gradient(Func<vector,double> f, vector x, DiffScheme scheme=DiffScheme.Forward, double hScale=1e-6){
		return (scheme==DiffScheme.Central) ? grad_central(f,x,hScale) : grad_forward(f,x,hScale);
	}

	public static matrix Hessian(Func<vector,double> f, vector x, double hScale=1e-4){
		return hess_central(f,x,hScale);
	}

	public static (vector xMin, int steps) Minimize(
		Func<vector,double> f,
		vector x0,
		double acc = 1e-6,
		DiffScheme scheme = DiffScheme.Forward,
		int maxSteps = 1000,
		double hScaleGrad = 1e-6,
		double hScaleHess = 1e-4
	){
		vector x = x0.copy();
		double fx = f(x);
		int steps = 0;

		for(; steps<maxSteps; steps++){
			vector g; matrix H;

			if(scheme==DiffScheme.Central){
				// --- Task C: fælles evalueringer til g og H ---
				(g,H) = grad_hess_central_cached(f,x,hScaleGrad, hScaleHess);
			}else{
				// Forward g (hurtig) + central H (robust)
				g = grad_forward(f,x,hScaleGrad);
				H = hess_central(f,x,hScaleHess);
			}

			double gnorm = g.norm();
			if(gnorm < acc) break;

			symmetrize_inplace(H);

			vector dx;
			try{
				dx = qrgs.solve(H, -1.0*g);
			}catch(Exception){
				dx = -1.0 * g; // fallback
			}

			double lambda = line_search(f, x, fx, g, dx);
			vector xNew = x + lambda*dx;
			double fNew = f(xNew);

			double dxnorm = (lambda*dx).norm();
			double xnorm  = Math.Max(1.0, x.norm());
			if(dxnorm < Math.Sqrt(acc)*xnorm){ x = xNew; fx = fNew; break; }

			x = xNew; fx = fNew;
		}
		return (x, steps);
	}

	// ========== IMPLEMENTATION ==========

	static double hi(double xi, double hScale){
		return hScale * (1.0 + Math.Abs(xi));
	}

	static vector grad_forward(Func<vector,double> f, vector x, double hScale){
		int n = x.size;
		var g = new vector(n);
		double fx = f(x);
		for(int i=0;i<n;i++){
			double hi_i = hi(x[i], hScale);
			double old = x[i];
			x[i] = old + hi_i;
			double fph = f(x);
			x[i] = old;
			g[i] = (fph - fx)/hi_i;
		}
		return g;
	}

	static vector grad_central(Func<vector,double> f, vector x, double hScale){
		int n = x.size;
		var g = new vector(n);
		for(int i=0;i<n;i++){
			double hi_i = hi(x[i], hScale);
			double old = x[i];
			x[i] = old + hi_i; double fph = f(x);
			x[i] = old - hi_i; double fmh = f(x);
			x[i] = old;
			g[i] = (fph - fmh)/(2.0*hi_i);
		}
		return g;
	}

	static matrix hess_central(Func<vector,double> f, vector x, double hScale){
		int n = x.size;
		var H = new matrix(n,n);
		double f0 = f(x);
		double[] h = new double[n];
		double[] f_plus = new double[n];
		double[] f_minus = new double[n];

		for(int i=0;i<n;i++){
			h[i] = hi(x[i], hScale);
			double xi = x[i];
			x[i] = xi + h[i]; f_plus[i]  = f(x);
			x[i] = xi - h[i]; f_minus[i] = f(x);
			x[i] = xi;
		}

		for(int i=0;i<n;i++){
			H[i,i] = (f_plus[i] - 2.0*f0 + f_minus[i])/(h[i]*h[i]);
		}

		for(int i=0;i<n;i++)
		for(int j=i+1;j<n;j++){
			double hi_i = h[i], hj_j = h[j];
			double xi = x[i], xj = x[j];

			x[i]=xi+hi_i; x[j]=xj+hj_j; double fpp = f(x);
			x[i]=xi+hi_i; x[j]=xj-hj_j; double fpm = f(x);
			x[i]=xi-hi_i; x[j]=xj+hj_j; double fmp = f(x);
			x[i]=xi-hi_i; x[j]=xj-hj_j; double fmm = f(x);

			x[i]=xi; x[j]=xj;

			double Hij = (fpp - fpm - fmp + fmm) / (4.0*hi_i*hj_j);
			H[i,j] = Hij;
			H[j,i] = Hij;
		}
		return H;
	}

	// ---- Task C: fælles evalueringer til g og H (central) ----
	static (vector g, matrix H) grad_hess_central_cached(
		Func<vector,double> f,
		vector x,
		double hScaleGrad,
		double hScaleHess
	){
		// Vi bruger samme h for g og H. Du kan vælge at have to skalaer;
		// her bruger vi hScaleHess for begge for at sikre konsistens.
		int n = x.size;
		var g = new vector(n);
		var H = new matrix(n,n);

		double f0 = f(x);
		double[] h = new double[n];
		double[] f_plus = new double[n];
		double[] f_minus = new double[n];

		// Forbered alle 1D skift én gang (genbruges i g og H_ii)
		for(int i=0;i<n;i++){
			h[i] = hi(x[i], hScaleHess);
			double xi = x[i];
			x[i] = xi + h[i]; f_plus[i]  = f(x);
			x[i] = xi - h[i]; f_minus[i] = f(x);
			x[i] = xi;
		}

		// Gradient (central) og diagonaler af H
		for(int i=0;i<n;i++){
			double hi_i = h[i];
			g[i]   = (f_plus[i] - f_minus[i])/(2.0*hi_i);
			H[i,i] = (f_plus[i] - 2.0*f0 + f_minus[i])/(hi_i*hi_i);
		}

		// Off-diagonaler for H
		for(int i=0;i<n;i++)
		for(int j=i+1;j<n;j++){
			double hi_i = h[i], hj_j = h[j];
			double xi = x[i], xj = x[j];

			x[i]=xi+hi_i; x[j]=xj+hj_j; double fpp = f(x);
			x[i]=xi+hi_i; x[j]=xj-hj_j; double fpm = f(x);
			x[i]=xi-hi_i; x[j]=xj+hj_j; double fmp = f(x);
			x[i]=xi-hi_i; x[j]=xj-hj_j; double fmm = f(x);

			x[i]=xi; x[j]=xj;

			double Hij = (fpp - fpm - fmp + fmm) / (4.0*hi_i*hj_j);
			H[i,j] = Hij;
			H[j,i] = Hij;
		}

		return (g,H);
	}

	static void symmetrize_inplace(matrix A){
		int n = A.size1;
		for(int i=0;i<n;i++)
			for(int j=i+1;j<n;j++){
				double s = 0.5*(A[i,j]+A[j,i]);
				A[i,j]=s; A[j,i]=s;
			}
	}

	static double line_search(Func<vector,double> f, vector x, double fx, vector g, vector dx){
		const double alpha = 1e-4;
		const double factor = 0.5;
		double lambda = 1.0;

		double gdotdx = g.dot(dx);
		if(gdotdx > 0){
			for(int i=0;i<dx.size;i++) dx[i] = -g[i];
			gdotdx = -g.norm()*g.norm();
		}

		while(true){
			vector xt = x + lambda*dx;
			double ft = f(xt);
			if(ft <= fx + alpha*lambda*gdotdx) break;
			lambda *= factor;
			if(lambda < 1.0/ (1<<20)) break;
		}
		return lambda;
	}
}
