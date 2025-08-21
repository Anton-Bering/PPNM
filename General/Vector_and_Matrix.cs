// OBS: Gamme udgave som ikke bruges mere

/*======================================================================
  VectorAndMatrix.cs  –  v3  (C#-7.3-kompatibel)
  ----------------------------------------------------------------------
  Ét statisk værktøjsbibliotek til alle generelle matrix- og vektor-
  operationer, som bruges i PPNM-hjemmeopgaverne.

  Nye funktioner i v3
    • Symmetrize                 (A ← ½(A + Aᵀ))
    • DiagonalMatrix             (byg  diag(d) )
    • CheckDiagonalMatrix        (to overloads)
  Eksisterende v2-tilføjelser
    • Dot / Norm
    • SolveUpperTriangular
    • RandomMatrix/RandomVector med ekstern Random

  Alt er skrevet med *klassisk* metode-syntaks (ingen C# 8-features),
  så filen kan kompilere med `mcs` (C# 7.3).
 ======================================================================*/
using System;
using System.Text;

/* HERHER ------ Vector-Class tilføget til at bruge i HW5: ------ START */
public class vector {
    private double[] data;

    public int size => data.Length;

    public vector(int n) {
        data = new double[n];
    }

    public vector(double[] arr) {
        data = new double[arr.Length];
        Array.Copy(arr, data, arr.Length);
    }

    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }

    public vector copy() {
        return new vector(data);
    }

    public double norm() {
        double sum = 0;
        foreach (var x in data) sum += x * x;
        return Math.Sqrt(sum);
    }

    public static vector operator +(vector a, vector b) {
        if (a.size != b.size) throw new ArgumentException("vector sizes must match");
        var result = new vector(a.size);
        for (int i = 0; i < a.size; i++) result[i] = a[i] + b[i];
        return result;
    }

    public static vector operator -(vector a, vector b) {
        if (a.size != b.size) throw new ArgumentException("vector sizes must match");
        var result = new vector(a.size);
        for (int i = 0; i < a.size; i++) result[i] = a[i] - b[i];
        return result;
    }

    public static vector operator *(double c, vector v) {
        var result = new vector(v.size);
        for (int i = 0; i < v.size; i++) result[i] = c * v[i];
        return result;
    }

    public static vector operator *(vector v, double c) => c * v;

    public override string ToString() {
        return "[" + string.Join(", ", data) + "]";
    }

    /* HERHER ------ Vector-Class tilføget til at bruge i HW5: ------ END */

    /* HERHER ------ Vector-Class tilføget til at bruge i HW8: ------ START */

    public double[] ToArray() {
        var arr = new double[size];
        Array.Copy(data, arr, size);
        return arr;
    }

    /* HERHER ------ Vector-Class tilføget til at bruge i HW8: ------ END */

}


/*  Ingen namespace for at matche tidligere brug;  ønskes et namespace,
    skal brugskoden naturligvis tilrettes. */
public static class VectorAndMatrix
{
    // tol_upper_limmet = 1e-12; // HERHER: tilføj evetuelt det her.
    // tol_lower_limmet = 1e-5; //HERHER:   tilføj evetuelt det her.
    /* ------------------------------------------------------------------
       1.  Random-generation
       ------------------------------------------------------------------ */
    public static double[,] RandomMatrix(int n, int m)
        => RandomMatrix(n, m, new Random());

    public static double[,] RandomMatrix(int n, int m, Random rng)
    {
        var A = new double[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                A[i, j] = rng.NextDouble();
        return A;
    }

    public static double[] RandomVector(int n)
        => RandomVector(n, new Random());

    public static double[] RandomVector(int n, Random rng)
    {
        var v = new double[n];
        for (int i = 0; i < n; i++) v[i] = rng.NextDouble();
        return v;
    }

    /* ------------------------------------------------------------------
       2.  Pretty-printing
       ------------------------------------------------------------------ */
    public static string PrintMatrix(double[,] M, string name = "", int dec = 5)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Matrix {name}:");
        int n = M.GetLength(0), m = M.GetLength(1);
        string fmt = "{0," + (dec + 8).ToString() + ":F" + dec + "}";

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
                sb.AppendFormat(fmt, M[i, j]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static string PrintVector(double[] v, string name = "", int dec = 5)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Vector {name}:");
        string fmt = "{0," + (dec + 8).ToString() + ":F" + dec + "}";
        foreach (double x in v) sb.AppendFormat(fmt + "\n", x);
        return sb.ToString();
    }

    /* ------------------------------------------------------------------
       3.  Basis-algebra (Transpose, Multiply, Dot, Norm)
       ------------------------------------------------------------------ */
    public static double[,] Transpose(double[,] A)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        var AT = new double[m, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                AT[j, i] = A[i, j];
        return AT;
    }

    public static double[,] Multiply(double[,] A, double[,] B)
    {
        int n = A.GetLength(0),
            m = A.GetLength(1),
            p = B.GetLength(1);
        if (B.GetLength(0) != m)
            throw new ArgumentException("Inner dimensions must match.");

        var C = new double[n, p];
        for (int i = 0; i < n; i++)
            for (int k = 0; k < p; k++)
            {
                double s = 0.0;
                for (int j = 0; j < m; j++) s += A[i, j] * B[j, k];
                C[i, k] = s;
            }
        return C;
    }

    public static double[] Multiply(double[,] A, double[] v)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        if (v.Length != m) throw new ArgumentException("Dimensions mismatch.");

        var w = new double[n];
        for (int i = 0; i < n; i++)
        {
            double s = 0.0;
            for (int j = 0; j < m; j++) s += A[i, j] * v[j];
            w[i] = s;
        }
        return w;
    }

    public static double Dot(double[] a, double[] b)
    {
        if (a.Length != b.Length) throw new ArgumentException("Size mismatch.");
        double s = 0.0;
        for (int i = 0; i < a.Length; i++) s += a[i] * b[i];
        return s;
    }

    public static double Norm(double[] v)
        => Math.Sqrt(Dot(v, v));

    /* ------------------------------------------------------------------
       4.  Triangulære/identitets-tests & determinanter
       ------------------------------------------------------------------ */
    public static bool IsUpperTriangular(double[,] A, double tol = 1e-12)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        for (int i = 1; i < n; i++)
            for (int j = 0; j < Math.Min(i, m); j++)
                if (Math.Abs(A[i, j]) > tol) return false;
        return true;
    }

    public static bool IsIdentityMatrix(double[,] A, double tol = 1e-12)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        if (n != m) return false;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                double expected = (i == j) ? 1.0 : 0.0;
                if (Math.Abs(A[i, j] - expected) > tol) return false;
            }
        return true;
    }

    public static double Determinant(double[,] R)   // kun for øvre-triangulær
    {
        int n = R.GetLength(0), m = R.GetLength(1);
        if (n != m) throw new ArgumentException("Matrix must be square.");

        double prod = 1.0;
        for (int i = 0; i < n; i++) prod *= R[i, i];
        return prod;
    }

    public static double[] SolveUpperTriangular(double[,] R, double[] y)
    {
        int n = R.GetLength(0), m = R.GetLength(1);
        if (n != m || y.Length != n)
            throw new ArgumentException("Dimensions mismatch.");

        var x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double s = y[i];
            for (int j = i + 1; j < n; j++) s -= R[i, j] * x[j];
            x[i] = s / R[i, i];
        }
        return x;
    }

    /* HERHER ------ Vector-Class tilføget til at bruge i HW8: ------ START */

    public static double[,] Outer(vector a, vector b) {
        var A = new double[a.size, b.size];
        for (int i = 0; i < a.size; i++)
            for (int j = 0; j < b.size; j++)
                A[i, j] = a[i] * b[j];
        return A;
    }

    public static vector Multiply(double[,] A, vector v) {
        int n = A.GetLength(0), m = A.GetLength(1);
        if (v.size != m) throw new ArgumentException("Dimensions mismatch.");
        var result = new vector(n);
        for (int i = 0; i < n; i++) {
            double sum = 0;
            for (int j = 0; j < m; j++) sum += A[i, j] * v[j];
            result[i] = sum;
        }
        return result;
    }

    public static double[,] Scale(double[,] A, double s) {
        int n = A.GetLength(0), m = A.GetLength(1);
        var B = new double[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                B[i, j] = s * A[i, j];
        return B;
    }

    public static double[,] Subtract(double[,] A, double[,] B) {
        int n = A.GetLength(0), m = A.GetLength(1);
        if (B.GetLength(0) != n || B.GetLength(1) != m)
            throw new ArgumentException("Dimension mismatch.");
        var C = new double[n, m];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                C[i, j] = A[i, j] - B[i, j];
        return C;
    }

    /* HERHER ------ Vector-Class tilføget til at bruge i HW8: ------ END */

    /* ------------------------------------------------------------------
       5.  Convenience  “Check*”  – skriver resultat til Console
       ------------------------------------------------------------------ */
    public static void CheckUpperTriangular(double[,] A, string name = "A", double tol = 1e-12)
        => Console.WriteLine(IsUpperTriangular(A, tol)
            ? $"TEST: Is {name} upper‑triangular? RESULT: Yes."
            : $"TEST: Is {name} upper‑triangular? RESULT: No.");

    public static void CheckIdentityMatrix(double[,] A, string name = "A", double tol = 1e-12)
        => Console.WriteLine(IsIdentityMatrix(A, tol)
            ? $"TEST: is {name} the identity matrix (within a tolerance of {tol})? \nRESULT: Yes."
            : $"TEST: is {name} the identity matrix (within a tolerance of {tol})? \nRESULT: No.");

    /* udgave 1.0
    public static void CheckMatrixEqual(double[,] A, double[,] B,
                                        string aName = "A", string bName = "B",
                                        double tol = 1e-5) // 1e-12
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        bool ok = true;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > tol) ok = false;

        Console.WriteLine(ok
            ? $"TEST: is {aName}={bName} (within a tolerance of {tol})?\nRESULT: Yes."
            : $"TEST: is {aName}={bName} (within a tolerance of {tol})?\nRESULT: No.");
    }
    */

    // udgave 2.0:
    public static bool CheckMatrixEqual(
        double[,] A,
        double[,] B,
        string aName = "A",
        string bName = "B",
        double tolUpper = 1e-12,
        double tolLower = 1e-5,
        bool verbose = true)
    {
        if (A.GetLength(0) != B.GetLength(0) ||
            A.GetLength(1) != B.GetLength(1))
            throw new ArgumentException("Matrices do not have the same dimensions.");

        // Find the maximum absolute difference
        int rows = A.GetLength(0);
        int cols = A.GetLength(1);
        double maxDiff = 0.0;

        for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
                maxDiff = Math.Max(maxDiff, Math.Abs(A[i, j] - B[i, j]));

        // Escalate the tolerance (×10) until it is large enough or hits tolLower
        double tol = tolUpper;
        while (tol < tolLower && maxDiff > tol)
            tol *= 10.0;

        // Clamp if you want an upper bound
        if (tol > tolLower) tol = tolLower;

        bool isEqual = maxDiff <= tol;

        if (verbose)
        {
            string resultMsg = isEqual
                ? $"Yes  (|maximum difference| = {maxDiff:E3})" // ? $"Yes  (|maximum difference| = {maxDiff:E3} ≤ tolerance  = {tol:E3})"
                : $"No   (|maximum difference| = {maxDiff:E3})"; // : $"No   (|maximum difference| = {maxDiff:E3} > tolerance  = {tol:E3})";

            Console.WriteLine($"TEST: is {aName} = {bName}?\nRESULT: {resultMsg}");
        }

        return isEqual;
    }


    public static void CheckVectorEqual(double[] a, double[] b,
                                        string aName = "a", string bName = "b",
                                        double tol = 1e-12)
    {
        if (a.Length != b.Length)
        {
            Console.WriteLine($"CheckVectorEqual {aName} vs {bName}: size mismatch");
            return;
        }
        bool ok = true;
        for (int i = 0; i < a.Length; i++)
            if (Math.Abs(a[i] - b[i]) > tol) ok = false;

        Console.WriteLine(ok
            ? $"TEST: is {aName}={bName} (within a tolerance of {tol})?\nRESULT: OK"
            : $"TEST: is {aName}={bName} (within a tolerance of {tol})?\nRESULT: No");
    }

    /* ------------------------------------------------------------------
       6.  Nyttige konstruktører
       ------------------------------------------------------------------ */
    public static double[,] IdentityMatrix(int n)
    {
        var I = new double[n, n];
        for (int i = 0; i < n; i++) I[i, i] = 1.0;
        return I;
    }

    public static double[,] DiagonalMatrix(double[] diag)
    {
        int n = diag.Length;
        var D = new double[n, n];
        for (int i = 0; i < n; i++) D[i, i] = diag[i];
        return D;
    }

    /* ------------------------------------------------------------------
       7.  Nyttige generelle in-place operationer
       ------------------------------------------------------------------ */
    public static void Symmetrize(double[,] A)          // A ← ½(A + Aᵀ)
    {
        int n = A.GetLength(0);
        if (A.GetLength(1) != n) throw new ArgumentException("Matrix must be square.");
        for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                double val = 0.5 * (A[i, j] + A[j, i]);
                A[i, j] = A[j, i] = val;
            }
    }

    /* ------------------------------------------------------------------
       8.  CheckDiagonalMatrix (to overloads)
       ------------------------------------------------------------------ */
    public static void CheckDiagonalMatrix(double[,] A, double tol = 1e-6)
    {
        int n = A.GetLength(0), m = A.GetLength(1);
        bool ok = true;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                if (i != j && Math.Abs(A[i, j]) > tol) ok = false;

        Console.WriteLine(ok
            ? "CheckDiagonalMatrix: OK"
            : "CheckDiagonalMatrix: FAILED");
    }

    public static void CheckDiagonalMatrix(double[,] A, double[] diag, double tol = 1e-6)
    {
        int n = diag.Length;
        if (A.GetLength(0) != n || A.GetLength(1) != n)
        {
            Console.WriteLine("CheckDiagonalMatrix vs diag: dimension mismatch");
            return;
        }

        bool ok = true;
        for (int i = 0; i < n; i++)
        {
            if (Math.Abs(A[i, i] - diag[i]) > tol) ok = false;
            for (int j = 0; j < n; j++)
                if (i != j && Math.Abs(A[i, j]) > tol) ok = false;
        }

        Console.WriteLine(ok
            ? "CheckDiagonalMatrix vs diag: OK"
            : "CheckDiagonalMatrix vs diag: FAILED");
    }
}

