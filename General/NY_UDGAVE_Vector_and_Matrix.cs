using System;
using System.Text;


public static class LinAlgUtils
{
    public const double Default_Tolerance = 1e-12;
    public const double Default_ValueMin = 0.0;
    public const double Default_ValueMax = 1.0;

    private static readonly Random _rng = new Random();

    /* ---------------- Gør ting smukke ---------------- */

    static string FormatMatrix(matrix M, int decimals = 3)
    {
        var sb = new StringBuilder();
        string fmt = "{0,10:F" + decimals + "} "; // bredde 10, højrejusteret
        for (int i = 0; i < M.size1; i++)
        {
            for (int j = 0; j < M.size2; j++) sb.AppendFormat(fmt, M[i, j]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    static string FormatVector(vector v, int decimals = 3)
    {
        var sb = new StringBuilder();
        string fmt = "{0,10:F" + decimals + "}"; // bredde 10, højrejusteret
        for (int i = 0; i < v.size; i++) sb.AppendLine(string.Format(fmt, v[i]));
        return sb.ToString();
    }


    /* ---------------- Matrix generators ---------------- */

    public static matrix RandomMatrix(int n, int m,
                                      double valueMin = Default_ValueMin,
                                      double valueMax = Default_ValueMax,
                                      Random rng = null)
    {
        if (rng == null) rng = _rng;
        var A = new matrix(n, m);
        double span = valueMax - valueMin;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                A[i, j] = valueMin + span * rng.NextDouble();
        return A;
    }

    public static matrix IdentityMatrix(int n) => matrix.Identity(n);

    /* ---------------- Size helpers ---------------- */

    public static (int Rows, int Cols) SizeOf(matrix A) => (A.size1, A.size2);

    public static int SizeOfSquareMatrix(matrix A)
    {
        if (A.size1 != A.size2)
            throw new ArgumentException("SizeOfSquareMatrix: the matrix is not square.");
        return A.size1;
    }

    /* ---------------- Basic operations --------------- */

    public static matrix Transpose(matrix A) => A.T;

    public static matrix MultiplyMatrices(matrix A, matrix B)
    {
        if (A.size2 != B.size1)
            throw new ArgumentException("MultiplyMatrices: dimension mismatch.");
        return A * B;
    }

    /* ---------------- Property checks ---------------- */

    private static void PrintCheck(string name, string property, bool result)
    {
        Console.WriteLine($"TEST: Is the matrix {name} {property}?");
        Console.WriteLine($"RESULT: {(result ? "yes" : "no")}.\n");
    }

    public static bool IsUpperTriangular(matrix A, double tol = Default_Tolerance)
    {
        for (int i = 1; i < A.size1; i++)
            for (int j = 0; j < Math.Min(i, A.size2); j++)
                if (Math.Abs(A[i, j]) > tol) return false;
        return true;
    }

    public static bool IsLowerTriangular(matrix A, double tol = Default_Tolerance)
    {
        for (int i = 0; i < A.size1; i++)
            for (int j = i + 1; j < A.size2; j++)
                if (Math.Abs(A[i, j]) > tol) return false;
        return true;
    }

    public static void CheckUpperTriangular(matrix A, string name = "A", double tol = Default_Tolerance)
        => PrintCheck(name, "upper‑triangular", IsUpperTriangular(A, tol));

    public static void CheckLowerTriangular(matrix A, string name = "A", double tol = Default_Tolerance)
        => PrintCheck(name, "lower‑triangular", IsLowerTriangular(A, tol));

    public static bool AreMatricesEqual(matrix A, matrix B, double tol = Default_Tolerance)
    {
        if (A.size1 != B.size1 || A.size2 != B.size2) return false;
        for (int i = 0; i < A.size1; i++)
            for (int j = 0; j < A.size2; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > tol) return false;
        return true;
    }

    public static void CheckMatricesEqual(matrix A, matrix B,
                                          string nameA = "A",
                                          string nameB = "B",
                                          double tol = Default_Tolerance)
        => PrintCheck($"{nameA} = {nameB} (within {tol:E})",
                      string.Empty,
                      AreMatricesEqual(A, B, tol));

    public static void CheckIdentityMatrix(matrix A, string name = "A", double tol = Default_Tolerance)
    {
        int n = SizeOfSquareMatrix(A);
        var I = matrix.Identity(n);
        CheckMatricesEqual(A, I, name, "I", tol);
    }

    /* ---------------- Vectors ---------------- */

    public static vector RandomVector(int n,
                                      double valueMin = Default_ValueMin,
                                      double valueMax = Default_ValueMax,
                                      Random rng = null)
    {
        if (rng == null) rng = _rng;
        var v = new vector(n);
        double span = valueMax - valueMin;
        for (int i = 0; i < n; i++)
            v[i] = valueMin + span * rng.NextDouble();
        return v;
    }
}
