using System;
public class matrix {
    public readonly int size1, size2;
    private double[] data;
    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[size1 * size2];
    }
    public double this[int i, int j] {
        get => data[i + j * size1];
        set => data[i + j * size1] = value;
    }
    public static matrix operator* (matrix A, matrix B) {
        if (A.size2 != B.size1) 
            throw new ArgumentException("Matrix dimensions mismatch for multiplication");
        matrix C = new matrix(A.size1, B.size2);
        for (int i = 0; i < A.size1; i++) {
            for (int j = 0; j < B.size2; j++) {
                double sum = 0;
                for (int k = 0; k < A.size2; k++) {
                    sum += A[i, k] * B[k, j];
                }
                C[i, j] = sum;
            }
        }
        return C;
    }
    public static vector operator* (matrix A, vector v) {
        if (A.size2 != v.size) 
            throw new ArgumentException("Matrix-vector dimension mismatch");
        vector u = new vector(A.size1);
        for (int i = 0; i < A.size1; i++) {
            double sum = 0;
            for (int j = 0; j < A.size2; j++) {
                sum += A[i, j] * v[j];
            }
            u[i] = sum;
        }
        return u;
    }
    public matrix copy() {
        matrix B = new matrix(size1, size2);
        Array.Copy(this.data, B.data, data.Length);
        return B;
    }
    public matrix transpose() {
        matrix T = new matrix(size2, size1);
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                T[j, i] = this[i, j];
        return T;
    }
}
