using static MatrixTools;
using static VectorTools;

using System;
using System.Diagnostics;
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
            var sw = new Stopwatch();
            sw.Start();
            QR qr = new QR(A);
            sw.Stop();
            double seconds = sw.Elapsed.TotalSeconds;
            Console.WriteLine($"Time for QR factorization (N={N}): {seconds:F6} s");
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
            Console.WriteLine("Matrix A (8x5):\n" + A.ToPrettyString());
            Console.WriteLine("Orthogonal matrix Q (8x5):\n" + Q.ToPrettyString());
            Console.WriteLine("Upper triangular matrix R (5x5):\n" + R.ToPrettyString());

            // Check that R is upper triangular
            bool isTriangular = true;
            for (int i = 0; i < R.size1; i++) {
                for (int j = 0; j < i; j++) {
                    if (R[i, j] != 0) { isTriangular = false; break; }
                }
                if (!isTriangular) break;
            }
            Console.WriteLine($"Matrix R er øvre triangular: {isTriangular}");

            // Check that Q^T Q = I (within tolerance)
            matrix QTQ = Q.transpose() * Q;
            matrix I = MatrixTools.Identity(QTQ.size1);
            bool ortho = QTQ.NearlyEqual(I);
            Console.WriteLine($"Q^T * Q = I (ortogonalitet): {ortho}");

            // Check that QR = A (within tolerance)
            matrix QRprod = Q * R;
            bool reconstruct = QRprod.NearlyEqual(A);
            Console.WriteLine($"Q * R = A (rekonstruktion): {reconstruct}");

            Console.WriteLine("\n");
        }

        Console.WriteLine("-- Part B --\n");
        // Part B: Solve linear system and compute inverse
        {
            int n = 5;
            var rnd = new Random(2);
            // Generate random square matrix A and vector b
            matrix A = new matrix(n, n);
            vector b = new vector(n);
            for (int i = 0; i < n; i++) {
                b[i] = rnd.NextDouble();
                for (int j = 0; j < n; j++) {
                    A[i, j] = rnd.NextDouble();
                }
            }

            // Solve A x = b using QR decomposition
            QR qr = new QR(A);
            vector x = qr.solve(b);

            // Check that A*x == b
            vector Ax = A * x;
            bool solves = Ax.NearlyEqual(b);
            Console.WriteLine($"Løsning af A x = b virker: {solves}");

            // Compute determinant via R
            double detA = qr.det();
            Console.WriteLine($"det(A) = {detA:F6}");

            // Compute inverse of A via QR
            matrix B = qr.inverse();
            // Check that A * B = I
            matrix I = MatrixTools.Identity(n);
            matrix AB = A * B;
            bool invOk = AB.NearlyEqual(I);
            Console.WriteLine($"A^-1 korrekt (A * A^-1 = I): {invOk}");

            Console.WriteLine("\n");
        }
    }
}
