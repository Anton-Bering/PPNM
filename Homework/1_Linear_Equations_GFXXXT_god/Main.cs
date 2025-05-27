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

        Console.WriteLine("--- --- --- PART A --- --- ---\n");
        Console.WriteLine("-- -- -- Check that \"decomp\" works as intended -- -- --\n");

        
        int n = 6, m = 4;
        Console.WriteLine($"Generate a random tall ({n}x{m}) matrix A:\n");
        var rndA = new Random(1);
        var A = new matrix(n, m);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                A[i, j] = rndA.NextDouble();
        Console.WriteLine(FormatMatrix(A));

        var qrA = new QR(A);
        Console.WriteLine("Factorize it into QR, its yielded: \n");
        Console.WriteLine("Q:");
        Console.WriteLine(FormatMatrix(qrA.Q));

        Console.WriteLine("\nR:");
        Console.WriteLine(FormatMatrix(qrA.R));

        Console.WriteLine("\nCheck that R is upper triangular: \n");
        bool isUpperTriangular = true;
        for (int i = 1; i < qrA.R.size1; i++) {
            for (int j = 0; j < i; j++) {
                if (Math.Abs(qrA.R[i, j]) > 1e-12) {
                    isUpperTriangular = false;
                    break;
                }
            }
            if (!isUpperTriangular) break;
        }
        Console.WriteLine("\n Is R upper triangular?");
        Console.WriteLine(isUpperTriangular ? "Yes, R is upper triangular\n" : "No, R is not upper triangular\n");
        // Console.WriteLine(isUpperTriangular ? "True\n" : "False\n");

        // Check Q^T * Q = I
        Console.WriteLine("\nCheck that QᵀQ=1 \n");
        var maxErr = 0.0;
        Console.WriteLine("QᵀQ yields:");
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
        Console.WriteLine("\nIs QᵀQ=1 ? (within a tolerance of 1e-12)");
        Console.WriteLine(maxErr < 1e-12 ? "Yes, QᵀQ=1\n" : "No, QᵀQ≠1\n");
        // Console.WriteLine(maxErr < 1e-12 ? "True\n" : "False\n");

        // Check A = Q*R
        Console.WriteLine("Check that QR=A:\n");
        Console.WriteLine("QR yields:");
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
        Console.WriteLine("Is QR=A ? (within a tolerance of 1e-12)");
        Console.WriteLine(qrErr < 1e-12 ? "Yes, QR=A\n" : "No, QR≠A\n");
        // Console.WriteLine(qrErr < 1e-12 ? "True\n" : "False\n");
 
        // --- Check solve ---
        Console.WriteLine("-- -- -- Check that \"solve\" works as intended -- -- --\n");
        int N2 = 4;
        var rndB = new Random(2);
        var A_sq = new matrix(N2, N2);
        var b = new vector(N2);
        for (int i = 0; i < N2; i++) {
            b[i] = rndB.NextDouble();
            for (int j = 0; j < N2; j++)
                A_sq[i, j] = rndB.NextDouble();
        }
        Console.WriteLine($"Generate a random square matrix A ({N2}x{N2}):");
        Console.WriteLine(FormatMatrix(A_sq));
        Console.WriteLine($"\nGenerate a random vector b ({N2}):");
        Console.WriteLine(FormatVector(b));

        Console.WriteLine("\nFactorize A into QR;\n");
        // 
        var qrSolve = new QR(A_sq);
        var x = qrSolve.solve(b);
        Console.WriteLine("Solve QRx=b;\n");
        Console.WriteLine("its yields x=");
        Console.WriteLine(FormatVector(x));

        Console.WriteLine("\nCheck that Ax=b;\n");
        Console.WriteLine("Ax yields:");
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
        Console.WriteLine("Is Ax=b ? (within a tolerance of 1e-12)");
        Console.WriteLine(solveErr < 1e-12 ? "Yes, Ax=b\n" : "No, Ax≠b\n");
        // Console.WriteLine(solveErr < 1e-12 ? "True\n" : "False\n");

        // ---------- PART B ----------
        Console.WriteLine("--- --- --- PART B --- --- ---\n");
        Console.WriteLine("-- -- -- Check that \"inverse\" works as intended -- -- --\n");

        var rndC = new Random(3);
        var Ainv = new matrix(N2, N2);
        for (int i = 0; i < N2; i++)
            for (int j = 0; j < N2; j++)
                Ainv[i, j] = rndC.NextDouble();

        Console.WriteLine($"Generate a random square matrix A ({N2}x{N2}):");
        Console.WriteLine(FormatMatrix(Ainv));
        var qrInv = new QR(Ainv);
        var B = qrInv.inverse();

        Console.WriteLine("Factorize A into QR;\n");
        
        Console.WriteLine("Calculate the inverse B;");
        Console.WriteLine(FormatMatrix(B, 4));

        var AB = matrix.Multiply(Ainv, B);
        Console.WriteLine("Check that AB=I, where I is the identity matrix;\n");
        Console.WriteLine(FormatMatrix(AB, 4));

        double maxDiff = 0;
        for (int i = 0; i < N2; i++)
            for (int j = 0; j < N2; j++) {
                double expected = (i == j) ? 1.0 : 0.0;
                maxDiff = Math.Max(maxDiff, Math.Abs(AB[i, j] - expected));
            }
        Console.WriteLine("Is AB=I ? (within a tolerance of 1e-12)");
        Console.WriteLine(maxDiff < 1e-12 ? "Yes, AB=I\n" : "No, AB≠I\n");
        // Console.WriteLine(maxDiff < 1e-12 ? "True\n" : "False\n");

        // ---------- PART C ----------
        Console.WriteLine("--- --- PART C --- --- ---\n");
        Console.WriteLine("Measure the time it takes to QR-factorize a random NxN matrix as function of N. \n");
        // 
        Console.WriteLine("The time it takes QR-factorize goes like O(N³).");
        Console.WriteLine("The time it takes to QR-factorize a random NxN matrix (as a function of N) is recorded in times.txt and plotted in times_plot.png.");
    }
}

