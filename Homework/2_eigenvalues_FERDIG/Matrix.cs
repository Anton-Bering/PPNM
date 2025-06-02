// Matrix.cs
public class matrix {
    public int size1, size2;
    private double[,] data;
    public matrix(int n, int m) {
        size1 = n; size2 = m;
        data = new double[n, m];
    }
    public double this[int i, int j] {
        get { return data[i, j]; }
        set { data[i, j] = value; }
    }
    // Create an n x n identity matrix
    public static matrix id(int n) {
        matrix M = new matrix(n, n);
        for(int i=0; i<n; i++) {
            M[i, i] = 1.0;
        }
        return M;
    }
    // Return a copy of this matrix
    public matrix copy() {
        matrix C = new matrix(size1, size2);
        for(int i=0; i<size1; i++)
            for(int j=0; j<size2; j++)
                C[i, j] = data[i,j];
        return C;
    }
}
