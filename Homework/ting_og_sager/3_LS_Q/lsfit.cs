using System;
using System.Collections.Generic;

namespace LeastSquaresFit {

    public static class LSFit {

        public static (Vector c, Matrix cov) Fit(List<Func<double, double>> fs, Vector x, Vector y, Vector dy) {
            if (x.Size != y.Size || x.Size != dy.Size || y.Size != dy.Size) {
                throw new ArgumentException("x, y, and dy must have the same size");
            }

            int m = x.Size;
            int n = fs.Count;

            Matrix A = new Matrix(m, n);
            Vector b = new Vector(m);

            for (int i = 0; i < m; i++) {
                double xi = x.Get(i);
                double dyi = dy.Get(i);
                b.Set(i, y.Get(i) / dyi);

                for (int k = 0; k < n; k++) {
                    A.Set(i, k, fs[k](xi) / dyi);
                }
            }

            QRGS qrgs = new QRGS(A);
            Matrix Q = qrgs.Q;
            Matrix R = qrgs.R;
            Vector c = qrgs.Solve(b);
            Matrix Rinv = qrgs.Inverse();
            Matrix cov = Rinv * Rinv.Transpose();

            return (c, cov);
        }
    }
}
