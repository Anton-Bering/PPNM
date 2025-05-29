using System;
public class matrix {
    public readonly int size1, size2;
    private double[] data;  // Linear storage in column-major order (size1 = rows, size2 = cols)

    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[size1 * size2];
    }

    // Indexer for matrix elements (zero-based indexing)
    public double this[int i, int j] {
        get { return data[i + j * size1]; }
        set { data[i + j * size1] = value; }
    }

    // Create a deep copy of the matrix
    public matrix copy() {
        matrix copyMat = new matrix(size1, size2);
        Array.Copy(this.data, copyMat.data, this.data.Length);
        return copyMat;
    }

    // (Optional) Multiply two matrices (for convenience in testing)
    public static matrix Multiply(matrix A, matrix B) {
        if (A.size2 != B.size1) throw new ArgumentException("Matrix dimensions do not match for multiplication");
        matrix C = new matrix(A.size1, B.size2);
        for (int i = 0; i < C.size1; i++) {
            for (int j = 0; j < C.size2; j++) {
                double sum = 0;
                for (int k = 0; k < A.size2; k++) {
                    sum += A[i, k] * B[k, j];
                }
                C[i, j] = sum;
            }
        }
        return C;
    }

    // (Optional) Generate an identity matrix of given size
    public static matrix Identity(int n) {
        matrix I = new matrix(n, n);
        for (int i = 0; i < n; i++) {
            I[i, i] = 1.0;
        }
        return I;
    }
}
