// ------------------- Bruges til at lave Vecor og metrice genralt -------------------
using System;
using System.Text;

/*======================================================================
    VECTOR class
  ======================================================================*/
public class vector
{
    private readonly double[] v;
    public  double[] Raw => v;

    public vector(int n)                       { v = new double[n]; }
    public vector(params double[] values)      { v = (double[])values.Clone(); }

    public int Size  => v.Length;
    public int size  => v.Length;

    public double this[int i]
    {
        get => v[i];
        set => v[i] = value;
    }

    public vector copy() => new vector(v);
    public vector Copy() => new vector(v);

    public vector map(Func<double,double> f)
    {
        var r = new vector(Size);
        for (int i = 0; i < Size; i++) r[i] = f(v[i]);
        return r;
    }
    public vector Map(Func<double,double> f) => map(f);  // alias

    public static double Dot(vector a, vector b)
    {
        if (a.Size != b.Size) throw new ArgumentException("Dot: length mismatch");
        double s = 0;
        for (int i = 0; i < a.Size; i++) s += a[i] * b[i];
        return s;
    }
    public double Dot(vector other) => Dot(this, other);
    public double norm() => Math.Sqrt(Dot(this, this));
    public double Norm() => norm();                      // alias

    public static vector operator +(vector a, vector b)
    {
        if (a.Size != b.Size) throw new ArgumentException("a+b length mismatch");
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a[i] + b[i];
        return r;
    }
    public static vector operator -(vector a, vector b)
    {
        if (a.Size != b.Size) throw new ArgumentException("a-b length mismatch");
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a[i] - b[i];
        return r;
    }
    public static vector operator *(vector a, double c)
    {
        var r = new vector(a.Size);
        for (int i = 0; i < a.Size; i++) r[i] = a[i] * c;
        return r;
    }
    public static vector operator *(double c, vector a) => a * c;

    public string ToPretty(string fmt = "0.000000")
    {
        var sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < Size; i++)
        {
            sb.Append(v[i].ToString(fmt));
            if (i < Size - 1) sb.Append(", ");
        }
        sb.Append("]");
        return sb.ToString();
    }
    public override string ToString() => ToPretty();
}

/* --- Aliases: --- */
public class Vector : vector
{
    public Vector(int n)        : base(n)     { }
    public Vector(vector v)     : base(v.Raw) { }

    public static Vector operator +(Vector a, Vector b)
        => new Vector(((vector)a) + ((vector)b));

    public static Vector operator -(Vector a, Vector b)
        => new Vector(((vector)a) - ((vector)b));

    public static Vector operator *(Vector a, double c)
        => new Vector(((vector)a) * c);

    public static Vector operator *(double c, Vector a)
        => new Vector(c * ((vector)a));
}
public class Vec : vector { public Vec(int n) : base(n) { } }


/*======================================================================
    MATRIX class
  ======================================================================*/
public class matrix
{
    private readonly double[,] a;
    public  int size1  { get; }
    public  int size2  { get; }
    public  int Rows   => size1;
    public  int Cols   => size2;
    public  int rows    => size1;
    public  int cols    => size2;
    public  int n      => size1;
    public  int m      => size2;

    public matrix(int n,int m) { size1 = n; size2 = m; a = new double[n,m]; }
    public matrix(int n)       : this(n,n) { }

    public double this[int i,int j] { get => a[i,j]; set => a[i,j] = value; }

    public matrix copy() { var C=new matrix(size1,size2); Array.Copy(a,C.a,a.Length); return C; }

    public static matrix Identity(int n)
    {
        var I=new matrix(n);
        for(int i=0;i<n;i++) I[i,i]=1;
        return I;
    }
    public static matrix id(int n) => Identity(n);

    public static matrix Random(int n,int m,Random rng)
    {
        var A=new matrix(n,m);
        for(int i=0;i<n;i++) for(int j=0;j<m;j++) A[i,j]=2*rng.NextDouble()-1;
        return A;
    }

    public matrix T {
        get { var B=new matrix(size2,size1);
              for(int i=0;i<size1;i++) for(int j=0;j<size2;j++) B[j,i]=a[i,j];
              return B;}
    }

    public static matrix Multiply(matrix A, matrix B) => A * B;

    public static matrix operator +(matrix A,matrix B){
        if(A.size1!=B.size1||A.size2!=B.size2) throw new ArgumentException("A+B dim mismatch");
        var C=new matrix(A.size1,A.size2);
        for(int i=0;i<C.size1;i++) for(int j=0;j<C.size2;j++) C[i,j]=A[i,j]+B[i,j];
        return C;
    }
    public static matrix operator -(matrix A,matrix B){
        if(A.size1!=B.size1||A.size2!=B.size2) throw new ArgumentException("A-B dim mismatch");
        var C=new matrix(A.size1,A.size2);
        for(int i=0;i<C.size1;i++) for(int j=0;j<C.size2;j++) C[i,j]=A[i,j]-B[i,j];
        return C;
    }
    public static matrix operator *(double c,matrix A){
        var B=new matrix(A.size1,A.size2);
        for(int i=0;i<B.size1;i++) for(int j=0;j<B.size2;j++) B[i,j]=c*A[i,j];
        return B;
    }
    public static matrix operator *(matrix A,matrix B){
        if(A.size2!=B.size1) throw new ArgumentException("A*B dim mismatch");
        var C=new matrix(A.size1,B.size2);
        for(int i=0;i<C.size1;i++)
            for(int k=0;k<A.size2;k++){
                double aik=A[i,k];
                for(int j=0;j<C.size2;j++) C[i,j]+=aik*B[k,j];
            }
        return C;
    }
    public static vector operator *(matrix A,vector v){
        if(A.size2!=v.Size) throw new ArgumentException("A*v dim mismatch");
        var r=new vector(A.size1);
        for(int i=0;i<A.size1;i++) for(int j=0;j<A.size2;j++) r[i]+=A[i,j]*v[j];
        return r;
    }

    /* ---------- Gaussian elimination solver ------------- */
    public vector Solve(vector b){
        if(size1!=size2)         throw new ArgumentException("Solve: non‑square");
        if(b.Size!=size1)        throw new ArgumentException("Solve: length mismatch");

        var A = (double[,])a.Clone();
        var v = (double[])b.Raw.Clone();

        int N=size1;
        for(int k=0;k<N;k++){
            int max=k;
            for(int i=k+1;i<N;i++) if(Math.Abs(A[i,k])>Math.Abs(A[max,k])) max=i;

            if(max!=k){
                for(int j=0;j<N;j++) (A[k,j],A[max,j])=(A[max,j],A[k,j]);
                (v[k],v[max])=(v[max],v[k]);
            }

            for(int i=k+1;i<N;i++){
                double factor=A[i,k]/A[k,k];
                for(int j=k;j<N;j++) A[i,j]-=factor*A[k,j];
                v[i]-=factor*v[k];
            }
        }

        var x=new vector(N);
        for(int i=N-1;i>=0;i--){
            double sum=v[i];
            for(int j=i+1;j<N;j++) sum-=A[i,j]*x[j];
            x[i]=sum/A[i,i];
        }
        return x;
    }

    public string ToPretty(string fmt="0.000000"){
        var sb=new StringBuilder();
        for(int i=0;i<size1;i++){
            sb.Append("[ ");
            for(int j=0;j<size2;j++) sb.Append(a[i,j].ToString(fmt).PadLeft(12));
            sb.AppendLine(" ]");
        }
        return sb.ToString();
    }
}

/* ---------- Aliases: ---------- */
public class Matrix : matrix
{
    public Matrix(int n,int m) : base(n,m) { }
    public Matrix(int n)       : base(n)   { }

    public Matrix(matrix A)    : this(A.Rows,A.Cols)
    {
        for(int i=0;i<Rows;i++)
            for(int j=0;j<Cols;j++)
                this[i,j]=A[i,j];
    }

    public static new Matrix Random(int n,int m,Random rng)
    {
        var M=new Matrix(n,m);
        for(int i=0;i<n;i++) for(int j=0;j<m;j++) M[i,j]=2*rng.NextDouble()-1;
        return M;
    }

    public new Matrix T {
        get {
            var B=new Matrix(Cols,Rows);
            for(int i=0;i<Rows;i++)
                for(int j=0;j<Cols;j++)
                    B[j,i]=this[i,j];
            return B;
        }
    }

    public static Matrix operator +(Matrix A, Matrix B)
        => new Matrix(((matrix)A) + ((matrix)B));

    public static Matrix operator -(Matrix A, Matrix B)
        => new Matrix(((matrix)A) - ((matrix)B));

    public static Matrix operator *(double c, Matrix A)
        => new Matrix(c * ((matrix)A));

    public static Matrix operator *(Matrix A, Matrix B)
        => new Matrix(((matrix)A) * ((matrix)B));

    public static Vector operator *(Matrix A, Vector v)
        => new Vector(((matrix)A) * ((vector)v));
}
public class Mat : matrix { public Mat(int n,int m):base(n,m){} public Mat(int n):base(n){} }

