using System;

public class matrix {
    public readonly int size1, size2;
    private double[] data;

    // Indexer for 2D access (row i, col j)
    public double this[int i, int j] {
        get { return data[i + j * size1]; }
        set { data[i + j * size1] = value; }
    }

    // Constructor for matrix of given size (rows, cols)
    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[n * m];
    }

    // Copy method to duplicate matrix
    public matrix copy() {
        matrix B = new matrix(size1, size2);
        Array.Copy(this.data, B.data, this.data.Length);
        return B;
    }

    // Static method to create identity matrix of size n x n
    public static matrix id(int n) {
        matrix I = new matrix(n, n);
        for (int i = 0; i < n; i++) {
            I[i, i] = 1.0;
        }
        return I;
    }

    // Transpose of a matrix
    public matrix transpose() {
        matrix R = new matrix(size2, size1);
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                R[j, i] = this[i, j];
        return R;
    }

    // Approximate equality check
    public bool approx(matrix B, double tol = 1e-10) {
        if (this.size1 != B.size1 || this.size2 != B.size2) return false;
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                if (Math.Abs(this[i, j] - B[i, j]) > tol) return false;
        return true;
    }

    // Matrix multiplication
    public static matrix operator *(matrix A, matrix B) {
        if (A.size2 != B.size1)
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        matrix result = new matrix(A.size1, B.size2);
        for (int i = 0; i < A.size1; i++)
            for (int j = 0; j < B.size2; j++)
                for (int k = 0; k < A.size2; k++)
                    result[i, j] += A[i, k] * B[k, j];
        return result;
    }
}
