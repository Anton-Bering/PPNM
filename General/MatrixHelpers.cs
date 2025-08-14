using System;
using System.Text;

public static class MatrixHelpers
{
    // Uniform random matrix i [min,max]
    public static matrix RandomMatrix(int rows, int cols, Random rng, double min = -1.0, double max = 1.0)
    {
        if (rows <= 0 || cols <= 0) throw new ArgumentException("rows/cols must be positive.");
        if (rng == null) throw new ArgumentNullException(nameof(rng));
        var A = new matrix(rows, cols);
        double span = max - min;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                A[r, c] = min + span * rng.NextDouble();
        return A;
    }

    // Overload med internt Random
    public static matrix RandomMatrix(int rows, int cols) =>
        RandomMatrix(rows, cols, new Random());

    // R skal være øvre trekantet
    public static bool CheckUpperTriangular(matrix R, string name = "R", double tol = 1e-9, bool throwOnFail = true)
    {
        if (R == null) throw new ArgumentNullException(nameof(R));
        for (int i = 0; i < R.Rows; i++)
            for (int j = 0; j < i; j++)
                if (Math.Abs(R[i, j]) > tol)
                {
                    string msg = $"{name} not upper-triangular at ({i},{j}) = {R[i,j]} (tol={tol}).";
                    if (throwOnFail) throw new Exception(msg);
                    return false;
                }
        Console.WriteLine($"{name} is upper-triangular (tol={tol}).");
        return true;
    }

    // M skal være (n≈I)
    public static bool CheckIdentityMatrix(matrix M, string name = "M", double tol = 1e-6, bool throwOnFail = true)
    {
        if (M == null) throw new ArgumentNullException(nameof(M));
        if (M.Rows != M.Cols)
        {
            string msg = $"{name} not square: {M.Rows}x{M.Cols}.";
            if (throwOnFail) throw new Exception(msg);
            return false;
        }
        for (int i = 0; i < M.Rows; i++)
            for (int j = 0; j < M.Cols; j++)
            {
                double expect = (i == j) ? 1.0 : 0.0;
                if (Math.Abs(M[i, j] - expect) > tol)
                {
                    string msg = $"{name} not identity at ({i},{j}): {M[i,j]} (tol={tol}).";
                    if (throwOnFail) throw new Exception(msg);
                    return false;
                }
            }
        Console.WriteLine($"{name} ≈ I (tol={tol}).");
        return true;
    }

    // A ≈ B
    public static bool CheckMatrixEqual(matrix A, matrix B, string aName = "A", string bName = "B", double tol = 1e-6, bool throwOnFail = true)
    {
        if (A == null || B == null) throw new ArgumentNullException("A/B");
        if (A.Rows != B.Rows || A.Cols != B.Cols)
        {
            string msg = $"{aName} and {bName} dimension mismatch: {A.Rows}x{A.Cols} vs {B.Rows}x{B.Cols}.";
            if (throwOnFail) throw new Exception(msg);
            return false;
        }
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > tol)
                {
                    string msg = $"{aName} ≉ {bName} at ({i},{j}): {A[i,j]} vs {B[i,j]} (tol={tol}).";
                    if (throwOnFail) throw new Exception(msg);
                    return false;
                }
        Console.WriteLine($"{aName} ≈ {bName} (tol={tol}).");
        return true;
    }

    public static void Symmetrize(matrix A) // A ← ½(A + Aᵀ), in-place
    {
        int n = A.Rows;                      
        if (A.Cols != n) throw new ArgumentException("Matrix must be square.");
        for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)
            {
                double v = 0.5 * (A[i, j] + A[j, i]);
                A[i, j] = A[j, i] = v;
            }
    }

    public static matrix DiagonalMatrix(vector diag)
    {
        int n = diag.Size;
        var D = new matrix(n, n);
        for (int i = 0; i < n; i++) 
            D[i, i] = diag[i];
        return D;
    }

    public static matrix Transpose(matrix A)
    {
        if (A == null) throw new ArgumentNullException(nameof(A));
        var AT = new matrix(A.Cols, A.Rows);
        for (int i = 0; i < A.Rows; i++)
            for (int j = 0; j < A.Cols; j++)
                AT[j, i] = A[i, j];
        return AT;
    }



    // Pæn udskrift
    public static string PrintMatrix(matrix M, string name = "", int dec = 5)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Matrix {name}:");
        int n = M.Rows, m = M.Cols;
        string fmt = "{0," + (dec + 8).ToString() + ":F" + dec + "}";

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
                sb.AppendFormat(fmt, M[i, j]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

}
