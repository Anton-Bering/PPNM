// --- very smalle vector/martix and QR-solver ---
using System;

public class vector
{
	public double[] data;
	public int size => data.Length;

	public vector(int n)            { data = new double[n]; }
	public vector(params double[] a){ data = (double[])a.Clone(); }

	public double this[int i] { get=>data[i]; set=>data[i]=value; }

	public double norm()
	{
		double s = 0;
		foreach(var x in data) s+=x*x;
		return Math.Sqrt(s);
	}

	public static vector operator +(vector a, vector b)
	{
		var r = new vector(a.size);
		for(int i=0;i<a.size;i++) r[i]=a[i]+b[i];
		return r;
	}
	public static vector operator -(vector a) { var r=new vector(a.size); for(int i=0;i<a.size;i++) r[i]=-a[i]; return r; }
	public static vector operator -(vector a, vector b) => a+(-b);
	public static vector operator *(double c, vector a)
	{
		var r=new vector(a.size); for(int i=0;i<a.size;i++) r[i]=c*a[i]; return r;
	}
}

public class matrix
{
	public double[,] A;
	public int n => A.GetLength(0);

	public matrix(int n)
	{
		A = new double[n,n];
	}
	public double this[int i,int j] { get=>A[i,j]; set=>A[i,j]=value; }
}


public static class qrgs
{
	public static (matrix R, matrix Q) decomp(matrix A)
	{
		int n=A.n;
		var Q = new matrix(n);
		var R = new matrix(n);

		
		double[] a = new double[n];
		double[] q = new double[n];

		for(int j=0;j<n;j++){
			for(int i=0;i<n;i++) a[i]=A[i,j];

			for(int k=0;k<j;k++){
				for(int i=0;i<n;i++) q[i]=Q[i,k];
				double dot=0; for(int i=0;i<n;i++) dot+=a[i]*q[i];
				R[k,j]=dot;
				for(int i=0;i<n;i++) a[i]-=dot*q[i];
			}
			double norm=0; for(int i=0;i<n;i++) norm+=a[i]*a[i];
			norm=Math.Sqrt(norm);
			R[j,j]=norm;
			for(int i=0;i<n;i++){ Q[i,j]=a[i]/norm; }
		}
		return (R,Q);
	}

	public static vector solve(matrix A, vector b)
	{
		var (R,Q)=decomp(A);
		int n=A.n;
		var y = new vector(n);
		
		for(int j=0;j<n;j++){
			double s=0; for(int i=0;i<n;i++) s+=Q[i,j]*b[i];
			y[j]=s;
		}
		
		var x = new vector(n);
		for(int i=n-1;i>=0;i--){
			double s=y[i];
			for(int k=i+1;k<n;k++) s-=R[i,k]*x[k];
			x[i]=s/R[i,i];
		}
		return x;
	}
}
