using System;

public partial class matrix {
    public readonly int size1, size2;
    private double[] data;

    public double this[int r, int c] {
        get => data[r * size2 + c];
        set => data[r * size2 + c] = value;
    }

    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[n * m];
    }
    
    public matrix(int n) {
        size1 = n;
        size2 = n;
        data = new double[n * n];
    }

    public static vector operator *(matrix A, vector v) {
        vector r = new vector(A.size1);
        for (int i = 0; i < A.size1; i++) {
            r[i] = 0;
            for (int j = 0; j < A.size2; j++) {
                r[i] += A[i, j] * v[j];
            }
        }
        return r;
    }

    public matrix transpose() {
        matrix T = new matrix(size2, size1);
        for (int i = 0; i < size1; i++) {
            for (int j = 0; j < size2; j++) {
                T[j, i] = this[i, j];
            }
        }
        return T;
    }
}
