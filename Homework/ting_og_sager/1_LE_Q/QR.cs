using System;

// QR decomposition class for solving linear systems and computing determinant/inverse
public class QR
{
    // Original matrix (copied)
    public Matrix A { get; private set; }

    // Orthogonal matrix Q
    public Matrix Q { get; private set; }

    // Upper triangular matrix R
    public Matrix R { get; private set; }

    // Constructor: performs Gram-Schmidt QR decomposition on input matrix
    public QR(Matrix M)
    {
        A = M.Copy();
        Q = A.Copy();
        R = new Matrix(M.Cols, M.Cols);
        int n = M.Cols;

        // Gram-Schmidt orthogonalization process
        for (int i = 0; i < n; ++i)
        {
            // Compute the norm of column i and normalize
            R[i, i] = Q.GetCol(i).Norm();
            Q.SetCol(i, Q.GetCol(i) / R[i, i]);

            // Remove projection of column i from subsequent columns
            for (int j = i + 1; j < n; ++j)
            {
                R[i, j] = Q.GetCol(i).Dot(Q.GetCol(j));
                Q.SetCol(j, Q.GetCol(j) - Q.GetCol(i) * R[i, j]);
            }
        }
    }

    // Solves Ax = r using QR decomposition (back substitution after projection)
    public Vector Solve(Vector r)
    {
        if (r.Length != Q.Rows)
            throw new ArgumentException("Vector size must match number of matrix rows.");

        // Compute Q^T * r
        Vector y = Q.Transpose() * r;

        // Solve R * x = y using back-substitution
        Vector x = new Vector(R.Cols);
        for (int i = R.Cols - 1; i >= 0; i--)
        {
            double sum = 0;
            for (int k = i + 1; k < R.Cols; k++)
                sum += R[i, k] * x[k];

            x[i] = (y[i] - sum) / R[i, i];
        }

        return x;
    }

    // Computes the absolute value of the determinant of A using R
    public double Det()
    {
        double det = 1.0;
        for (int i = 0; i < R.Rows; ++i)
            det *= R[i, i];
        return Math.Abs(det);
    }

    // Computes the inverse of matrix A using QR and solving A * x = e_i
    public Matrix Inverse()
    {
        if (A.Rows != A.Cols)
            throw new InvalidOperationException("Matrix must be square to compute inverse.");

        int n = A.Cols;
        Matrix inverse = new Matrix(n, n);
        Vector e = new Vector(n);

        for (int i = 0; i < n; ++i)
        {
            // Construct unit vector e_i
            e.Fill(0);
            e[i] = 1;

            // Solve A * x = e_i
            Vector column = Solve(e);

            // Set solution as the i-th column of the inverse
            inverse.SetCol(i, column);
        }

        return inverse;
    }
}
