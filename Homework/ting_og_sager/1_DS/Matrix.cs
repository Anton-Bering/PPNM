using System;
using System.Text;

public class matrix {
    public readonly int size1, size2;
    private double[] data;

    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[n * m];
    }

    public double this[int i, int j] {
        get => data[i + j * size1];
        set => data[i + j * size1] = value;
    }

    public matrix copy() {
        matrix copy = new matrix(size1, size2);
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                copy[i, j] = this[i, j];
        return copy;
    }

    public matrix transpose() {
        matrix transposed = new matrix(size2, size1);
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                transposed[j, i] = this[i, j];
        return transposed;
    }

    public static matrix operator *(matrix a, matrix b) {
        if (a.size2 != b.size1)
            throw new ArgumentException("Matrix dimensions do not match for multiplication.");
        matrix result = new matrix(a.size1, b.size2);
        for (int i = 0; i < a.size1; i++)
            for (int k = 0; k < a.size2; k++)
                for (int j = 0; j < b.size2; j++)
                    result[i, j] += a[i, k] * b[k, j];
        return result;
    }

    public static vector operator *(matrix a, vector v) {
        if (a.size2 != v.size)
            throw new ArgumentException("Matrix and vector dimensions do not match.");
        vector result = new vector(a.size1);
        for (int i = 0; i < a.size1; i++)
            for (int j = 0; j < a.size2; j++)
                result[i] += a[i, j] * v[j];
        return result;
    }

    public static matrix identity(int n) {
        matrix I = new matrix(n, n);
        for (int i = 0; i < n; i++)
            I[i, i] = 1.0;
        return I;
    }

    public static matrix random(int rows, int cols, Random rnd) {
        matrix mat = new matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                mat[i, j] = rnd.NextDouble();
        return mat;
    }

    public bool isApprox(matrix other, double tol = 1e-9) {
        if (size1 != other.size1 || size2 != other.size2)
            return false;
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                if (Math.Abs(this[i, j] - other[i, j]) > tol)
                    return false;
        return true;
    }

    public string ToStringFormatted() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < size1; i++) {
            for (int j = 0; j < size2; j++) {
                double val = this[i, j];
                string s = (Math.Abs(val) < 1e-4 && val != 0) ? 
                    val.ToString("0.000e00") : val.ToString("0.000");
                sb.Append($"{s,10} ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}