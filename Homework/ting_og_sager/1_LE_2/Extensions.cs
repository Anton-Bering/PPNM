using System;
using System.IO;

// Udvidelser til matrix-objekter
public static class MatrixTools {
    // Gør det nemt at printe en matrix
    public static string ToPrettyString(this matrix M) {
        var s = new StringWriter();
        for (int i = 0; i < M.size1; i++) {
            for (int j = 0; j < M.size2; j++) {
                s.Write($"{M[i, j],10:0.###} ");
            }
            s.WriteLine();
        }
        return s.ToString();
    }

    // Sammenlign to matricer med tolerance
    public static bool NearlyEqual(this matrix A, matrix B, double eps = 1e-10) {
        if (A.size1 != B.size1 || A.size2 != B.size2) return false;
        for (int i = 0; i < A.size1; i++)
            for (int j = 0; j < A.size2; j++)
                if (Math.Abs(A[i, j] - B[i, j]) > eps) return false;
        return true;
    }

    // Lav en identitetsmatrix
    public static matrix Identity(int n) {
        var I = new matrix(n, n);
        for (int i = 0; i < n; i++) I[i, i] = 1.0;
        return I;
    }
}

// Udvidelser til vector-objekter
public static class VectorTools {
    // Gør det nemt at printe en vektor
    public static string ToPrettyString(this vector v) {
        var s = new StringWriter();
        for (int i = 0; i < v.size; i++) {
            s.Write($"{v[i],10:0.###} ");
        }
        s.WriteLine();
        return s.ToString();
    }

    // Sammenlign to vektorer med tolerance
    public static bool NearlyEqual(this vector a, vector b, double eps = 1e-10) {
        if (a.size != b.size) return false;
        for (int i = 0; i < a.size; i++)
            if (Math.Abs(a[i] - b[i]) > eps) return false;
        return true;
    }
}
