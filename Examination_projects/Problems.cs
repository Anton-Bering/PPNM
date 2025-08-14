using System;

public static class Problems {

    public static double Rosenbrock(vector v){
        double x=v[0], y=v[1];
        return Math.Pow(1-x,2) + 100*Math.Pow(y-x*x,2);
    }

    public static double Himmelblau(vector v){
        double x=v[0], y=v[1];
        double t1 = x*x + y - 11;
        double t2 = x + y*y - 7;
        return t1*t1 + t2*t2;
    }

    /* n-D roteret/shiftet kvadratik med kendt minimum */
    public static Func<vector,double> RotatedQuadratic(int n){
        var rnd=new Random(12345);
        double[,] R=new double[n,n];
        // Gram-Schmidt for ortonormale kolonner
        for(int j=0;j<n;j++){
            var col=new vector(n);
            for(int i=0;i<n;i++) col[i]=rnd.NextDouble()-0.5;
            for(int k=0;k<j;k++){
                double dot=0; for(int i=0;i<n;i++) dot+=R[i,k]*col[i];
                for(int i=0;i<n;i++) col[i]-=dot*R[i,k];
            }
            double norm=0; for(int i=0;i<n;i++) norm+=col[i]*col[i];
            norm=Math.Sqrt(norm); if(norm==0) norm=1;
            for(int i=0;i<n;i++) R[i,j]=col[i]/norm;
        }
        double[] D=new double[n]; for(int i=0;i<n;i++) D[i]=1 + 9.0*i/Math.Max(1,n-1);
        var xstar=new vector(n); for(int i=0;i<n;i++) xstar[i]=0.5*i;

        return (vector x) => {
            var y=new vector(n);
            for(int i=0;i<n;i++){
                double s=0; for(int j=0;j<n;j++) s+=R[i,j]*(x[j]-xstar[j]); y[i]=s;
            }
            double val=0; for(int i=0;i<n;i++) val+=D[i]*y[i]*y[i];
            return val;
        };
    }
}
