// --- Small vector/matrix and QR-solver ---
using System;

public class vector {
	public double[] data;
	public int size => data.Length;

	public vector(int n)            { data = new double[n]; }
	public vector(params double[] a){ data = (double[])a.Clone(); }

	public double this[int i] { get=>data[i]; set=>data[i]=value; }

	public vector copy(){ return new vector((double[])data.Clone()); }

	public double dot(vector o){
		double s=0; for(int i=0;i<size;i++) s+=this[i]*o[i]; return s;
	}

	public double norm(){
		double s=0; for(int i=0;i<size;i++) s+=data[i]*data[i];
		return Math.Sqrt(s);
	}

	// --- operators ---
	public static vector operator+(vector a, vector b){
		int n=a.size; var c=new vector(n);
		for(int i=0;i<n;i++) c[i]=a[i]+b[i];
		return c;
	}
	public static vector operator-(vector a, vector b){
		int n=a.size; var c=new vector(n);
		for(int i=0;i<n;i++) c[i]=a[i]-b[i];
		return c;
	}
	public static vector operator-(vector a){
		int n=a.size; var c=new vector(n);
		for(int i=0;i<n;i++) c[i]=-a[i];
		return c;
	}
	public static vector operator*(double s, vector a){
		int n=a.size; var c=new vector(n);
		for(int i=0;i<n;i++) c[i]=s*a[i];
		return c;
	}
	public static vector operator*(vector a, double s) => s*a;
	public static vector operator/(vector a, double s){
		int n=a.size; var c=new vector(n);
		for(int i=0;i<n;i++) c[i]=a[i]/s;
		return c;
	}
}

public class matrix {
	double[,] data;
	public readonly int size1, size2; // rows, cols

	public matrix(int n,int m){ size1=n; size2=m; data=new double[n,m]; }
	public matrix(double[,] a){
		size1=a.GetLength(0); size2=a.GetLength(1);
		data=(double[,])a.Clone();
	}

	public double this[int i,int j]{
		get=>data[i,j];
		set=>data[i,j]=value;
	}

	public static vector operator*(matrix A, vector x){
		int n=A.size1, m=A.size2;
		var y=new vector(n);
		for(int i=0;i<n;i++){
			double s=0; for(int j=0;j<m;j++) s+=A[i,j]*x[j];
			y[i]=s;
		}
		return y;
	}

	public matrix copy(){
		var B=new matrix(size1,size2);
		for(int i=0;i<size1;i++)
			for(int j=0;j<size2;j++) B[i,j]=this[i,j];
		return B;
	}

	public static matrix identity(int n){
		var I=new matrix(n,n);
		for(int i=0;i<n;i++) I[i,i]=1;
		return I;
	}
}

// Classical Gram–Schmidt QR decomposition for (square) systems
public class qrgs {
	public matrix Q,R;

	public qrgs(matrix A){
		int n=A.size1, m=A.size2;
		// forventer her kvadratisk A (n==m); GS virker også for n>=m.
		Q=new matrix(n,m);
		R=new matrix(m,m);
		// Kopier kolonner en ad gangen
		var a_j = new vector(n);
		var q_j = new vector(n);

		for(int j=0;j<m;j++){
			// a_j = A[:,j]
			for(int i=0;i<n;i++) a_j[i]=A[i,j];
			// Orthogonaliser mod tidligere q_k
			for(int k=0;k<j;k++){
				for(int i=0;i<n;i++) q_j[i]=Q[i,k];
				double r_kj = a_j.dot(q_j);
				R[k,j] = r_kj;
				for(int i=0;i<n;i++) a_j[i] -= r_kj*q_j[i];
			}
			// Norm og normalisering
			double r_jj = a_j.norm();
			if(r_jj==0) throw new Exception("qrgs: singular column encountered");
			R[j,j]=r_jj;
			for(int i=0;i<n;i++) Q[i,j]=a_j[i]/r_jj;
		}
	}

	// y = Q^T b  (Q har ortonormale kolonner)
	vector applyQT(vector b){
		int n=Q.size1, m=Q.size2;
		var y=new vector(m);
		for(int j=0;j<m;j++){
			double s=0;
			for(int i=0;i<n;i++) s+=Q[i,j]*b[i];
			y[j]=s;
		}
		return y;
	}

	// Løs Ax=b via QR: A=QR => Rx = Q^T b  (R øvre trekantet)
	public vector solve(vector b){
		int m=R.size2;
		var y = applyQT(b);
		var x = new vector(m);
		for(int i=m-1;i>=0;i--){
			double s=y[i];
			for(int k=i+1;k<m;k++) s-=R[i,k]*x[k];
			x[i]=s/R[i,i];
		}
		return x;
	}

	public static vector solve(matrix A, vector b){
		var qr = new qrgs(A);
		return qr.solve(b);
	}
}
