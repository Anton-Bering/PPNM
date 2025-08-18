using System;

namespace PPNM.Minimization {

public static class Problems {
    // Rosenbrock test function (global min at (1,1))
    public static double Rosenbrock(vector v){
        double x = v[0], y = v[1];
        return (1 - x)*(1 - x) + 100*(y - x*x)*(y - x*x);
    }

    // Himmelblau test function (four minima; from (0,0) it goes to (3,2))
    public static double Himmelblau(vector v){
        double x = v[0], y = v[1];
        return (x*x + y - 11)*(x*x + y - 11) + (x + y*y - 7)*(x + y*y - 7);
    }

    // Rotated quadratic in nD
    public static Func<vector,double> RotatedQuadratic(int n, int seed=12345){
        var rng = new Random(seed);

        // build random orthonormal R via Gram-Schmidt
        var R = new matrix(n,n);
        for(int j=0;j<n;j++)
            for(int i=0;i<n;i++)
                R[i,j] = rng.NextDouble() - 0.5;

        // Gram-Schmidt
        for(int j=0;j<n;j++){
            for(int k=0;k<j;k++){
                double dot = 0;
                for(int i=0;i<n;i++) dot += R[i,k]*R[i,j];
                for(int i=0;i<n;i++) R[i,j] -= dot*R[i,k];
            }
            double norm = 0;
            for(int i=0;i<n;i++) norm += R[i,j]*R[i,j];
            norm = Math.Sqrt(norm);
            if(norm==0) norm = 1;
            for(int i=0;i<n;i++) R[i,j] /= norm;
        }

        // diagonal spectrum D from 1 to 10
        var D = new vector(n);
        for(int i=0;i<n;i++) D[i] = 1 + 9.0*i/Math.Max(1,n-1);

        // minimizer x*
        var xstar = new vector(n);
        for(int i=0;i<n;i++) xstar[i] = 0.5*i;

        return (vector x) => {
            var z = x - xstar;            // shift
            var q = R * z;                // rotate
            double sum = 0;
            for(int i=0;i<n;i++) sum += D[i]*q[i]*q[i];
            return sum;
        };
    }
}

}
