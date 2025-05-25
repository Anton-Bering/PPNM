using System;

public class matrix {
    public readonly int size1, size2;
    private double[] data;
    public matrix(int n, int m) {
        size1 = n;
        size2 = m;
        data = new double[n * m];
    }
    public matrix(int n) : this(n, n) { }  // square matrix constructor
    public double this[int i, int j] {
        get => data[i + j * size1];
        set => data[i + j * size1] = value;
    }
    public matrix copy() {
        matrix m = new matrix(size1, size2);
        Array.Copy(this.data, m.data, this.data.Length);
        return m;
    }
    public vector column(int j) {
        // Return j-th column as a vector
        vector col = new vector(size1);
        for (int i = 0; i < size1; i++) {
            col[i] = this[i, j];
        }
        return col;
    }
    public void setColumn(int j, vector col) {
        if (col.size != size1) 
            throw new ArgumentException("Column size mismatch");
        for (int i = 0; i < size1; i++) {
            this[i, j] = col[i];
        }
    }
}
