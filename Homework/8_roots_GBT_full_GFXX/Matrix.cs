using System;
public class matrix {
    private double[,] data;
    public int rows { get; private set; }
    public int cols { get; private set; }

    // Constructors
    public matrix(int n, int m) {
        data = new double[n, m];
        rows = n;
        cols = m;
    }
    public matrix(int n) : this(n, n) { }  // square matrix n x n

    // Indexer for element access
    public double this[int i, int j] {
        get { return data[i, j]; }
        set { data[i, j] = value; }
    }

    // Make a deep copy of the matrix
    public matrix copy() {
        matrix C = new matrix(rows, cols);
        Array.Copy(data, C.data, data.Length);
        return C;
    }
}
