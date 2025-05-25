using System;

public static class LeastSquares
{
    public static (Vector Coefficients, Matrix Covariance) LsFit(Func<double, double>[] fs, double[] x, double[] y, double[] dy)
    {
        int n = x.Length;
        int m = fs.Length;

        Matrix A = new Matrix(n, m);
        Vector b = new Vector(n);

        for (int i = 0; i < n; i++)
        {
            double sigma = dy[i];
            for (int k = 0; k < m; k++)
            {
                A[i, k] = fs[k](x[i]) / sigma;
            }
            b[i] = y[i] / sigma;
        }

        A.DecomposeQR(out Matrix Q, out Matrix R);

        Vector QTb = Q.Transpose() * b;
        Vector c = R.SolveUpperTriangular(QTb);

        Matrix R_inv = R.InvertUpperTriangular();
        Matrix covariance = R_inv * R_inv.Transpose();

        return (c, covariance);
    }
}
