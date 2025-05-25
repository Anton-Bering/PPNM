using System;
using System.Text;

public class MainClass {
    static string FormatMatrix(matrix M, int decimals = 3) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < M.size1; i++) {
            for (int j = 0; j < M.size2; j++) {
                sb.Append(string.Format("{0,10:F" + decimals + "} ", M[i, j]));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    static string FormatVector(vector v, int decimals = 3) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < v.size; i++) {
            sb.AppendLine(string.Format("{0,10:F" + decimals + "}", v[i]));
        }
        return sb.ToString();
    }


    public static void Main(string[] args) {
        if (args.Length > 0 && args[0].StartsWith("-size:")) {
            int N = int.Parse(args[0].Substring(6));
            var rnd = new Random(1);
            var A_perf = new matrix(N, N);
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    A_perf[i, j] = rnd.NextDouble();
            var qr = new QR(A_perf);
            return;
        }

        Console.WriteLine("---------- PART A ----------");
        Console.WriteLine("----- Check \"decomp\" -----");

        int n = 7, m = 4;
        var rndA = new Random(1);
        var A = new matrix(n, m);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                A[i, j] = rndA.NextDouble();

        Console.WriteLine($"Random matrix A ({n}x{m}):\n");
        Console.WriteLine(FormatMatrix(A));

        var qrA = new QR(A);
        Console.WriteLine("QR decomposition yielded:");
        Console.WriteLine("Matrix Q:\n");
        Console.WriteLine(FormatMatrix(qrA.Q));

        Console.WriteLine("Matrix R:\n");
        Console.WriteLine(FormatMatrix(qrA.R));

        // Check Q^T * Q = I
        var maxErr = 0.0;
        Console.WriteLine("The product Q^T * Q yields:\n");
        for (int i = 0; i < m; i++) {
            for (int j = 0; j < m; j++) {
                double sum = 0;
                for (int k = 0; k < n; k++)
                    sum += qrA.Q[k, i] * qrA.Q[k, j];
                Console.Write($"{sum,10:G4} ");
                if (i == j) maxErr = Math.Max(maxErr, Math.Abs(sum - 1));
                else maxErr = Math.Max(maxErr, Math.Abs(sum));
            }
            Console.WriteLine();
        }
        Console.WriteLine("\nIs Q^T * Q equal to the identity matrix within a tolerance?");
        Console.WriteLine(maxErr < 1e-12 ? "True\n" : "False\n");

        // Check A = Q*R
        Console.WriteLine("Checking if A = QR. Multiplying Q and R yields:\n");
        var QR = new matrix(n, m);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) {
                double sum = 0;
                for (int k = 0; k < m; k++)
                    sum += qrA.Q[i, k] * qrA.R[k, j];
                QR[i, j] = sum;
            }
        Console.WriteLine(FormatMatrix(QR));
        double qrErr = 0.0;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                qrErr = Math.Max(qrErr, Math.Abs(QR[i, j] - A[i, j]));
        Console.WriteLine("Is A - QR = 0 within a tolerance?");
        Console.WriteLine(qrErr < 1e-12 ? "True\n" : "False\n");
 
        // --- Check solve ---
        Console.WriteLine("----- Check \"solve\" -----");
        int N2 = 4;
        var rndB = new Random(2);
        var A_sq = new matrix(N2, N2);
        var b = new vector(N2);
        for (int i = 0; i < N2; i++) {
            b[i] = rndB.NextDouble();
            for (int j = 0; j < N2; j++)
                A_sq[i, j] = rndB.NextDouble();
        }

        Console.WriteLine($"Random square matrix A ({N2}x{N2}):\n");
        Console.WriteLine(FormatMatrix(A_sq));
        Console.WriteLine($"Random vector b ({N2}):");
        Console.WriteLine(FormatVector(b));

        var qrSolve = new QR(A_sq);
        var x = qrSolve.solve(b);
        Console.WriteLine("Solving QRx = b yields the following x:");
        Console.WriteLine(FormatVector(x));

        Console.WriteLine("Checking if Ax = b. Multiplying A and x yields:");
        var Ax = new vector(N2);
        for (int i = 0; i < N2; i++) {
            double sum = 0;
            for (int j = 0; j < N2; j++)
                sum += A_sq[i, j] * x[j];
            Ax[i] = sum;
        }
        Console.WriteLine(FormatVector(Ax));
        double solveErr = 0;
        for (int i = 0; i < N2; i++)
            solveErr = Math.Max(solveErr, Math.Abs(Ax[i] - b[i]));
        Console.WriteLine("Is A*x - b = 0 within a tolerance?");
        Console.WriteLine(solveErr < 1e-12 ? "True\n" : "False\n");

        // ---------- PART B ----------
        Console.WriteLine("---------- PART B ----------");
        Console.WriteLine("----- Check \"inverse\" -----");

        var rndC = new Random(3);
        var Ainv = new matrix(N2, N2);
        for (int i = 0; i < N2; i++)
            for (int j = 0; j < N2; j++)
                Ainv[i, j] = rndC.NextDouble();

        Console.WriteLine($"Random square matrix A ({N2}x{N2}):\n");
        Console.WriteLine(FormatMatrix(Ainv));
        var qrInv = new QR(Ainv);
        var B = qrInv.inverse();

        Console.WriteLine("Calculated inverse of A using QR method:\n");
        Console.WriteLine(FormatMatrix(B, 4));

        var AB = matrix.Multiply(Ainv, B);
        Console.WriteLine("Checking if A * B = I. Multiplying A and B yields:\n");
        Console.WriteLine(FormatMatrix(AB, 4));

        double maxDiff = 0;
        for (int i = 0; i < N2; i++)
            for (int j = 0; j < N2; j++) {
                double expected = (i == j) ? 1.0 : 0.0;
                maxDiff = Math.Max(maxDiff, Math.Abs(AB[i, j] - expected));
            }
        Console.WriteLine("Is A*B - I = 0 within a tolerance?");
        Console.WriteLine(maxDiff < 1e-12 ? "True\n" : "False\n");

        // ---------- PART C ----------
        Console.WriteLine("---------- PART C ----------");
        Console.WriteLine("Timing data written to timing.txt");
    }
}

