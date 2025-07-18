/*  vecotr_and_mactrics.cs
 *  A tiny helper library for basic matrix and vector operations.
 *  NOTE: The file-name keeps the original spelling on purpose.
 */

using System;
using System.Text;

public static class VectorAndMatrix     // class-name is spelled correctly
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

    public static double[,] MultiplyMatrices(double[,] A, double[,] B)
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

    public static bool IsIdentityMatrix(double[,] A, double tol = DefaultTolerance)
    {
        int n = SizeOfSquareMatrix(A);
        var I = IdentityMatrix(n);
        return MatricesEqual(A, I, tol);
    }

    /* ---------- pretty printers ---------- */

    public static string MatrixToString(double[,] A)
    {
        var sb        = new StringBuilder();
        sb.AppendLine();                         // tom linje før
        sb.AppendLine();
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

    public static string VectorToString(double[] v)
    {
        var sb = new StringBuilder();
        foreach (var x in v)
            sb.AppendLine($"{x:0.###}");
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

    public static void CheckIdentityMatrix(double[,] A, string name, double tol = DefaultTolerance)
    {
        Console.WriteLine($"\nTEST: Is {name} an identity matrix (within a tolerance of {tol})?");
        Console.WriteLine(IsIdentityMatrix(A) ? "RESULT: yes.\n" : "RESULT: no.\n");
    }


    

}
