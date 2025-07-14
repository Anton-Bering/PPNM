using System;

public static class QR
{
    // Returns Q (m×n, with orthonormal columns) and R (n×n upper‑triangular)
    public static (Matrix Q, Matrix R) Decompose(Matrix A)
    {
        int m=A.Rows,n=A.Cols;
        var Q=new Matrix(m,n);
        var R=new Matrix(n,n);

        for(int k=0;k<n;k++)
        {
            // copy column
            for(int i=0;i<m;i++) Q[i,k]=A[i,k];

            // subtract projections
            for(int j=0;j<k;j++)
            {
                double r=0;
                for(int i=0;i<m;i++) r+=Q[i,j]*A[i,k];
                R[j,k]=r;
                for(int i=0;i<m;i++) Q[i,k]-=r*Q[i,j];
            }

            // normalise
            double norm=0;
            for(int i=0;i<m;i++) norm+=Q[i,k]*Q[i,k];
            norm=Math.Sqrt(norm);
            if(norm==0) throw new ArgumentException("Linearly dependent columns");

            R[k,k]=norm;
            for(int i=0;i<m;i++) Q[i,k]/=norm;
        }
        return (Q,R);
    }

    // Solve R x = Qᵀ b  (least squares A x ≈ b)
    public static Vector Solve(Matrix Q,Matrix R,Vector b)
    {
        Vector Qtb = Q.T * b;
        int n=R.Cols;
        var x=new Vector(n);
        for(int i=n-1;i>=0;i--)
        {
            double s=Qtb[i];
            for(int j=i+1;j<n;j++) s-=R[i,j]*x[j];
            x[i]=s/R[i,i];
        }
        return x;
    }
}
