using System;

public class QrDecomposition
{
    public Matrix Q { get; private set; }
    public Matrix R { get; private set; }
    public QrDecomposition(Matrix A)
    {
        int m = A.Rows, n = A.Cols;
        Q = A.T().T(); // Deep copy
        R = new Matrix(n, n);
        for (int i = 0; i < n; i++)
        {
            R[i, i] = 0;
            for (int k = 0; k < m; k++) R[i, i] += Q[k, i] * Q[k, i];
            R[i, i] = Math.Sqrt(R[i, i]);
            for (int k = 0; k < m; k++) Q[k, i] /= R[i, i];
            for (int j = i + 1; j < n; j++)
            {
                R[i, j] = 0;
                for (int k = 0; k < m; k++) R[i, j] += Q[k, i] * Q[k, j];
                for (int k = 0; k < m; k++) Q[k, j] -= Q[k, i] * R[i, j];
            }
        }
    }
    public Vector Solve(Vector b)
    {
        Vector c = Q.T() * b;
        Vector x = new Vector(R.Cols);
        for (int i = R.Cols - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int j = i + 1; j < R.Cols; j++) sum += R[i, j] * x[j];
            x[i] = (c[i] - sum) / R[i, i];
        }
        return x;
    }
}