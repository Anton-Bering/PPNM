using static MatrixTools;
using static VectorTools;

using System;
public static class Program {
    public static void Main(string[] args) {
        // Check for performance measurement mode (part C)
        int perfN = 0;
        foreach (string arg in args) {
            if (arg.StartsWith("-size:")) {
                string num = arg.Substring(6);
                if (Int32.TryParse(num, out int N)) {
                    perfN = N;
                } else {
                    Console.Error.WriteLine("Invalid size parameter.");
                    return;
                }
            }
        }
        if (perfN > 0) {
            int N = perfN;
            var rnd = new Random(1);
            matrix A = new matrix(N, N);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    A[i, j] = rnd.NextDouble();
            QR qr = new QR(A);
            return;
        }

        Console.WriteLine("-- Part A --\n");
        // Part A: QR factorization on a tall matrix
        {
            int n = 8, m = 5;
            var rnd = new Random(1);
            matrix A = new matrix(n, m);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    A[i, j] = rnd.NextDouble();

            QR qr = new QR(A);
            matrix Q = qr.Q;
            matrix R = qr.R;

            Console.WriteLine("1) Check that my decomp works as intended:\n");

            // Udskriv A, Q og R
            Console.WriteLine("1.1) generate a random tall (n>m) matrix A (of a modest size);");
            Console.WriteLine("Matrix A:");
            Console.WriteLine(A.ToPrettyString());

            Console.WriteLine("1.2) factorize it into QR;");
            Console.WriteLine("Matrix Q:");
            Console.WriteLine(Q.ToPrettyString());

            Console.WriteLine("1.3) check that R is upper triangular;");
            Console.WriteLine("Matrix R:");
            Console.WriteLine(R.ToPrettyString());

            Console.WriteLine("1.4) check that Q^TQ=1;");
            // Beregn Q^T Q og sammenlign med I
            matrix QTQ = new matrix(m, m);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < n; k++)
                        QTQ[i, j] += Q[k, i] * Q[k, j];

            Console.WriteLine("Q^T * Q:");
            Console.WriteLine(QTQ.ToPrettyString());

            Console.WriteLine("Q^T * Q ≈ 1 ?");
            Console.WriteLine(QTQ.NearlyEqual(MatrixTools.Identity(m)));

            Console.WriteLine("1.5) check that QR=A;");
            // Beregn og udskriv om QR ≈ A
            matrix A_recon = new matrix(n, m);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    for (int k = 0; k < m; k++)
                        A_recon[i, j] += Q[i, k] * R[k, j];

            Console.WriteLine("QR:");
            Console.WriteLine(A_recon.ToPrettyString());

            Console.WriteLine("A ≈ QR ?");
            Console.WriteLine(A_recon.NearlyEqual(A));
        }

        // Part A: Solve A*x = b
        Console.WriteLine(" ");
        Console.WriteLine("2) Check that you solve works as intended:\n");
        {
            int N = 5;
            var rnd = new Random(2);
            matrix A = new matrix(N, N);
            vector b = new vector(N);
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    A[i, j] = rnd.NextDouble();
                }
                b[i] = rnd.NextDouble();
            }

            QR qr = new QR(A);
            vector x = qr.solve(b);

            vector Ax = new vector(N);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    Ax[i] += A[i, j] * x[j];

            Console.WriteLine("2.1) generate a random square matrix A (of a modest size);");
            Console.WriteLine("A:");
            Console.WriteLine(A.ToPrettyString());
            Console.WriteLine("2.2) generate a random vector b;");
            Console.WriteLine("b:");
            Console.WriteLine(b.ToPrettyString());
            Console.WriteLine("2.4) factorize A into QR;");
            Console.WriteLine("x:");
            Console.WriteLine(x.ToPrettyString());
            Console.WriteLine("2.5) check that Ax=b;");
            Console.WriteLine("A*x:");
            Console.WriteLine(Ax.ToPrettyString());
            Console.WriteLine("A*x ≈ b ?");
            Console.WriteLine(Ax.NearlyEqual(b));
        }

        // Part B: Inverse
        {
            int N = 5;
            var rnd = new Random(3);
            matrix A = new matrix(N, N);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    A[i, j] = rnd.NextDouble();

            QR qr = new QR(A);
            matrix B = qr.inverse();

            matrix AB = new matrix(N, N);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    for (int k = 0; k < N; k++)
                        AB[i, j] += A[i, k] * B[k, j];

            Console.WriteLine("");
            Console.WriteLine("-- Part B --\n");
            Console.WriteLine("1) Check that my function works as intended:\n");
            Console.WriteLine("1.1) generate a random square matrix A (of a modest size);");
            Console.WriteLine("A:");
            Console.WriteLine(A.ToPrettyString());
            Console.WriteLine("1.3) calculate the inverse B;");
            Console.WriteLine("B:");
            Console.WriteLine(B.ToPrettyString());
            Console.WriteLine("1.4) check that AB=I, where I is the identity matrix;");
            Console.WriteLine("A * A⁻¹:");
            Console.WriteLine(AB.ToPrettyString());
            Console.WriteLine("A * A⁻¹ ≈ I ?");
            Console.WriteLine(AB.NearlyEqual(MatrixTools.Identity(N)));
        }
    }
}
