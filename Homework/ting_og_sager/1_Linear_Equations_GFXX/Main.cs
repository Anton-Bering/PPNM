using System;
using System.Diagnostics;
public class MainClass {
    public static void Main(string[] args) {
        // If a size parameter is provided (Part C performance measurement mode)
        if (args.Length > 0 && args[0].StartsWith("-size:")) {
            int N = int.Parse(args[0].Substring(args[0].IndexOf(':') + 1));
            // Generate a random N×N matrix
            Random rnd = new Random(1);
            matrix A = new matrix(N, N);
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    A[i, j] = rnd.NextDouble();
                }
            }
            // Perform QR factorization (timing will be measured externally)
            QR qr = new QR(A);
            return;  // no output in timing mode
        }

        // Part A: Test QR decomposition on a tall matrix (n > m)
        Console.WriteLine("Testing QR decomposition (Part A) on a tall matrix...");
        int n = 6, m = 4;
        Random rndA = new Random(1);
        matrix A_tall = new matrix(n, m);
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                A_tall[i, j] = rndA.NextDouble();
            }
        }
        QR qrTall = new QR(A_tall);
        matrix Q = qrTall.Q;
        matrix R = qrTall.R;
        // Check orthonormality: Q^T * Q ≈ I_m
        double maxOrthError = 0.0;
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                double expected = (i == j ? 1.0 : 0.0);
                double sum = 0;
                for (int k = 0; k < n; k++) {
                    sum += Q[k, i] * Q[k, j];
                }
                double diff = Math.Abs(sum - expected);
                if (diff > maxOrthError) maxOrthError = diff;
            }
        }
        // Check R is upper triangular (elements below diagonal should be ~0)
        double maxLower = 0.0;
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < i; j++) { // i > j -> below diagonal
                double val = Math.Abs(R[i, j]);
                if (val > maxLower) maxLower = val;
            }
        }
        // Check reconstruction: Q * R ≈ A
        double maxQRdiff = 0.0;
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                double sum = 0;
                for (int k = 0; k < m; k++) {
                    sum += Q[i, k] * R[k, j];
                }
                double diff = Math.Abs(sum - A_tall[i, j]);
                if (diff > maxQRdiff) maxQRdiff = diff;
            }
        }
        Console.WriteLine($"- Q^T * Q orthonormality error (max deviation): {maxOrthError:e2}");
        Console.WriteLine($"- R upper-triangular check (max below-diag element): {maxLower:e2}");
        Console.WriteLine($"- Reconstruction error (max |QR - A|): {maxQRdiff:e2}");

        // Part A (continued): Test solving A*x = b using QR decomposition
        Console.WriteLine("\nTesting solving Ax = b (Part A) on a square system...");
        int N2 = 5;
        Random rndB = new Random(2);
        matrix A_sq = new matrix(N2, N2);
        vector b = new vector(N2);
        for (int i = 0; i < N2; i++) {
            for (int j = 0; j < N2; j++) {
                A_sq[i, j] = rndB.NextDouble();
            }
            b[i] = rndB.NextDouble();
        }
        QR qrSolve = new QR(A_sq);
        vector x = qrSolve.solve(b);
        // Compute A*x and compare with b
        double errorNorm = 0.0;
        for (int i = 0; i < N2; i++) {
            double sum = 0;
            for (int j = 0; j < N2; j++) {
                sum += A_sq[i, j] * x[j];
            }
            double diff = sum - b[i];
            errorNorm += diff * diff;
        }
        errorNorm = Math.Sqrt(errorNorm);
        double detA = qrSolve.det();
        Console.WriteLine($"- Determinant of A (from QR): {detA:e2}");
        Console.WriteLine($"- Solution error ||A*x - b||: {errorNorm:e2}");

        // Part B: Test matrix inverse computation via QR
        Console.WriteLine("\nTesting matrix inverse computation (Part B)...");
        int N3 = 5;
        Random rndC = new Random(3);
        matrix A_inv_test = new matrix(N3, N3);
        for (int i = 0; i < N3; i++) {
            for (int j = 0; j < N3; j++) {
                A_inv_test[i, j] = rndC.NextDouble();
            }
        }
        QR qrInv = new QR(A_inv_test);
        matrix B_inv = qrInv.inverse();
        // Check that A * B_inv ≈ I
        double maxDiffId = 0.0;
        for (int i = 0; i < N3; i++) {
            for (int j = 0; j < N3; j++) {
                double sum = 0;
                for (int k = 0; k < N3; k++) {
                    sum += A_inv_test[i, k] * B_inv[k, j];
                }
                double expected = (i == j ? 1.0 : 0.0);
                double diff = Math.Abs(sum - expected);
                if (diff > maxDiffId) maxDiffId = diff;
            }
        }
        Console.WriteLine($"- Inverse check, max |A * A_inv - I|: {maxDiffId:e2}");

        // Part C: Demonstrate timing for increasing matrix sizes
        Console.WriteLine("\nTiming QR decomposition for increasing N (Part C)...");
        Console.WriteLine("N, Time (ms)");
        for (int N = 100; N <= 200; N += 50) {
            // Generate an N×N random matrix
            matrix A_perf = new matrix(N, N);
            Random rndP = new Random(1);
            for (int i = 0; i < N; i++) {
                for (int j = 0; j < N; j++) {
                    A_perf[i, j] = rndP.NextDouble();
                }
            }
            Stopwatch sw = Stopwatch.StartNew();
            QR qrPerf = new QR(A_perf);
            sw.Stop();
            Console.WriteLine($"{N}, {sw.ElapsedMilliseconds}");
        }
    }
}
