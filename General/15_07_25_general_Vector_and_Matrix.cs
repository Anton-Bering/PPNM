// ------------------- Bruges til at lave Vecor og metrice genralt -------------------
using System;
using System.Text;

/*======================================================================
    MATRIX class
  ======================================================================*/
public class VectorAndMatrix
{
    /* ---------- public helpers ---------- */

    public const double DefaultTolerance = 1e-12;
    public const double DefaultValueMin  = 0.0;
    public const double DefaultValueMax  = 1.0;

    /* ----- matrix creation ----- */

    public static double[,] RandomMatrix(int n, int m,
                                         double min = DefaultValueMin,
                                         double max = DefaultValueMax)
    {
        var rng = new Random();
        var A   = new double[n, m];
        for (int i = 0; i < n; ++i)
            for (int j = 0; j < m; ++j)
                A[i, j] = rng.NextDouble() * (max - min) + min;
        return A;
    }

    public static double[,] Transpose(double[,] A)
    {
        int n = A.GetLength(0);
        int m = A.GetLength(1);
        var T = new double[m, n];
        for (int i = 0; i < n; ++i)
            for (int j = 0; j < m; ++j)
                T[j, i] = A[i, j];
        return T;
    }

    public static double[,] Multiply(double[,] A, double[,] B)
    {
        int nA = A.GetLength(0);     // rækker i A
        int p  = A.GetLength(1);     // kolonner i A  (= rækker i B)
        int pB = B.GetLength(0);
        int mB = B.GetLength(1);     // kolonner i B

        if (p != pB)
            throw new ArgumentException(
                $"Cannot multiply matrices.");

        var C = new double[nA, mB];
        for (int i = 0; i < nA; ++i)
            for (int j = 0; j < mB; ++j)
            {
                double sum = 0.0;
                for (int k = 0; k < p; ++k)
                    sum += A[i, k] * B[k, j];
                C[i, j] = sum;
            }
        return C;
    }

        /* ==============================================================
    Matrix × Vector  (n × m) · (m)  →  (n)
    ============================================================== */
    public static double[] Multiply(double[,] A, double[] v)
    {
        int n = A.GetLength(0);          // rows of A
        int m = A.GetLength(1);          // cols of A  (must equal length of v)

        if (v.Length != m)
            throw new ArgumentException(
                $"Cannot multiply: matrix columns ({m}) ≠ vector length ({v.Length}).");

        var res = new double[n];
        for (int i = 0; i < n; ++i) {
            double sum = 0.0;
            for (int k = 0; k < m; ++k)
                sum += A[i, k] * v[k];
            res[i] = sum;
        }
        return res;
    }

    /* ==============================================================
    Vector × Matrix  (n) · (n × m)  →  (m)
    ============================================================== */
    public static double[] Multiply(double[] v, double[,] A)
    {
        int n  = A.GetLength(0);         // rows of A  (must equal length of v)
        int m  = A.GetLength(1);         // cols of A

        if (v.Length != n)
            throw new ArgumentException(
                $"Cannot multiply: vector length ({v.Length}) ≠ matrix rows ({n}).");

        var res = new double[m];
        for (int j = 0; j < m; ++j) {
            double sum = 0.0;
            for (int k = 0; k < n; ++k)
                sum += v[k] * A[k, j];
            res[j] = sum;
        }
        return res;
    }


    public static double Determinant(double[,] A)
    {
        int n = SizeOfSquareMatrix(A);           // throws if A is not square
        var M  = (double[,])A.Clone();           // work on a copy
        double det = 1.0;

        for (int k = 0; k < n; ++k)
        {
            /* --- pivoting --- */
            int pivotRow = k;
            double pivot = Math.Abs(M[k, k]);
            for (int i = k + 1; i < n; ++i)
            {
                double val = Math.Abs(M[i, k]);
                if (val > pivot) { pivot = val; pivotRow = i; }
            }

            if (pivot < DefaultTolerance) return 0.0;          // singular

            if (pivotRow != k)                                 // row-swap ⇒ sign flip
            {
                for (int j = 0; j < n; ++j)
                {
                    double tmp = M[k, j];
                    M[k, j]    = M[pivotRow, j];
                    M[pivotRow, j] = tmp;
                }
                det = -det;
            }

            det *= M[k, k];

            /* --- eliminate below pivot --- */
            for (int i = k + 1; i < n; ++i)
            {
                double factor = M[i, k] / M[k, k];
                for (int j = k + 1; j < n; ++j)
                    M[i, j] -= factor * M[k, j];
            }
        }
        return det;
    }


    public static double[,] IdentityMatrix(int n)
    {
        var I = new double[n, n];
        for (int i = 0; i < n; ++i) I[i, i] = 1.0;
        return I;
    }

    /* ----- matrix size ----- */

    public static (int rows, int cols) SizeOfMatrix(double[,] A)
        => (A.GetLength(0), A.GetLength(1));

    public static int SizeOfSquareMatrix(double[,] A)
    {
        var (n, m) = SizeOfMatrix(A);
        if (n == m) return n;
        throw new ArgumentException("Matrix is not square.");
    }

    /* ----- vector creation ----- */

    public static double[] RandomVector(int n,
                                        double min = DefaultValueMin,
                                        double max = DefaultValueMax)
    {
        var mat = RandomMatrix(n, 1, min, max);
        var v   = new double[n];
        for (int i = 0; i < n; ++i) v[i] = mat[i, 0];
        return v;
    }

    /* ---------- property checks ---------- */

    public static bool IsUpperTriangular(double[,] A, double tol = DefaultTolerance)
    {
        int n = A.GetLength(0);
        int m = A.GetLength(1);
        for (int i = 1; i < n; ++i)
            for (int j = 0; j < Math.Min(i, m); ++j)
                if (Math.Abs(A[i, j]) > tol) return false;
        return true;
    }

    public static bool IsLowerTriangular(double[,] A, double tol = DefaultTolerance)
        => IsUpperTriangular(Transpose(A), tol);

    public static bool MatricesEqual(double[,] A, double[,] B,
                                     double tol = DefaultTolerance)
    {
        var (nA, mA) = SizeOfMatrix(A);
        var (nB, mB) = SizeOfMatrix(B);
        if (nA != nB || mA != mB) return false;

        for (int i = 0; i < nA; ++i)
            for (int j = 0; j < mA; ++j)
                if (Math.Abs(A[i, j] - B[i, j]) > tol) return false;
        return true;
    }

    public static bool VectorsEqual(double[] a, double[] b,
                                    double tol = DefaultTolerance)
    {
        if (a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; ++i)
            if (Math.Abs(a[i] - b[i]) > tol) return false;
        return true;
    }

    public static bool IsIdentityMatrix(double[,] A, double tol = DefaultTolerance)
    {
        int n = SizeOfSquareMatrix(A);
        var I = IdentityMatrix(n);
        return MatricesEqual(A, I, tol);
    }

    /* ---------- pretty printers ---------- */

    public static string PrintMatrix(double[,] A)
    {
        var sb        = new StringBuilder();
        sb.AppendLine();                         // tom linje før
        var (n, m)    = SizeOfMatrix(A);
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < m; ++j)
                sb.Append($"{A[i, j]:0.###}\t");
            sb.AppendLine();
        }
        sb.AppendLine();                         // tom linje efter
        return sb.ToString();
    }

    public static string PrintVector(double[] v)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        foreach (var x in v)
            sb.AppendLine($"{x:0.###}");
        sb.AppendLine();
        return sb.ToString();
    }

    /* ---------- console helpers ---------- */

    public static void CheckUpperTriangular(double[,] A, string name)
    {
        Console.WriteLine($"\nTEST: Is the matrix {name} upper triangular?");
        Console.WriteLine(IsUpperTriangular(A) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }

    public static void CheckLowerTriangular(double[,] A, string name)
    {
        Console.WriteLine($"\nTEST: Is the matrix {name} lower triangular?");
        Console.WriteLine(IsLowerTriangular(A) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }

    public static void CheckMatrixEqual(double[,] A, double[,] B,
                                        string nameA, string nameB,
                                        double tol = DefaultTolerance)
    {
        Console.WriteLine($"\nTEST: Is {nameA} = {nameB} (within a tolerance of {tol})?");
        Console.WriteLine(MatricesEqual(A, B, tol) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }

    public static void CheckVectorEqual(double[] a, double[] b,
                                        string nameA, string nameB,
                                        double tol = DefaultTolerance)
    {
        Console.WriteLine($"\nTEST: Is {nameA} = {nameB} (within a tolerance of {tol})?");
        Console.WriteLine(VectorsEqual(a, b, tol) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }


    public static void CheckIdentityMatrix(double[,] A, string name, double tol = DefaultTolerance)
    {
        Console.WriteLine($"\nTEST: Is {name} an identity matrix (within a tolerance of {tol})?");
        Console.WriteLine(IsIdentityMatrix(A) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }
    


}


