using System;
using System.Text;

public class MainClass {

    public static void Main(string[] args) {

        Console.WriteLine("------------ TASK A: Solving linear equations using QR-decomposition ------------");
        Console.WriteLine("------------ by modified Gram-Schmidt orthogonalization              ------------\n");
        Console.WriteLine("------ Check that \"decomp\" works as intended ------\n");

        
        int n = 6, m = 4;
        Console.WriteLine($"--- Generate a random tall ({n}x{m}) matrix A ---\n");
        Console.WriteLine($"Matrix A:");
        var A = Mactrics.RandomMatrix(n, m);
        Console.WriteLine(Mactrics.PrintMatrix(A));

        /*
        var qrA = new QR(A);
        Console.WriteLine("--- Factorize A into QR ---\n");
        Console.WriteLine("Matrix Q:");
        var Q = qrA.Q;
        Console.WriteLine(Mactrics.PrintMatrix(Q));

        Console.WriteLine("\nMatrix R:");
        var R = qrA.R;
        Console.WriteLine(Mactrics.PrintMatrix(R));

        Console.WriteLine("\n--- Check that R is upper triangular ---\n");
        Mactrics.CheckUpperTriangular(R,"R")

        // Check Q^T * Q = I
        Console.WriteLine("--- Check that QᵀQ≈I, where I is the identity matrix ---\n");
        Q_transpose = Mactrics.Transpose(Q)
        Q_transpose_times_Q = Mactrics.MultiplyMatrices(Q_transpose, Q)
        Mactrics.CheckIdentityMatrix(Q_transpose_times_Q,"QᵀQ")

        // Check A = Q*R
        Console.WriteLine("--- Check that QR≈A ---\n");
        Console.WriteLine("Matrix QR:");
        var QR = new matrix(n, m);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++) {
                double sum = 0;
                for (int k = 0; k < m; k++)
                    sum += qrA.Q[i, k] * qrA.R[k, j];
                QR[i, j] = sum;
            }
        Console.WriteLine(Mactrics.PrintMatrix(QR));
        double qrErr = 0.0;
        for (int i = 0; i < n; i++)
            for (int j = 0; j < m; j++)
                qrErr = Math.Max(qrErr, Math.Abs(QR[i, j] - A[i, j]));
        Mactrics.CheckIdentityMatrix(QR,"QR")
 
        // --- Check solve ---
        Console.WriteLine("------ Check that \"solve\" works as intended ------\n");

        Console.WriteLine($"--- Generate a random square matrix A ({N2}x{N2})---\n");
        int N2 = 4;
        var A_sq = Mactrics.RandomMatrix(N2, N2);
        Console.WriteLine($"Matrix A:");
        Console.WriteLine(Mactrics.PrintMatrix(A_sq));
        
        Console.WriteLine($"\n--- Generate a random vector b ({N2}) ---\n");
        var b = Mactrics.RandomVector(N2);
        Console.WriteLine($"Vector b:");
        Console.WriteLine(Mactrics.PrintMatrix(b));

        Console.WriteLine("\n--- Factorize A into QR ---\n");
        var qrSolve = new QR(A_sq);
        Console.WriteLine("Matrix Q:");
        Console.WriteLine(Mactrics.PrintMatrix(qrSolve.Q));
        Console.WriteLine("\nMatrix R:");
        Console.WriteLine(Mactrics.PrintMatrix(qrSolve.R));

        var x = qrSolve.solve(b);
        Console.WriteLine("--- Solve QRx=b ---\n");
        Console.WriteLine("Solving QRx=b leads to:");
        Console.WriteLine("\n Vector x:");
        Console.WriteLine(Mactrics.PrintMatrix(x));

        Console.WriteLine("\n--- Check that Ax≈b ---\n");
        Console.WriteLine("Vector Ax:");
        var Ax = Mactrics.MultiplyMatrices(A_sq, x);
        Console.WriteLine(Mactrics.PrintMatrix(Ax));
        CheckMatrixEqual(Ax, b, "Ax", "b");

        // ---------- TASK B ----------
        Console.WriteLine("------------ TASK B: Matrix inverse by Gram-Schmidt QR factorization ------------\n");
        Console.WriteLine("------ Check that \"inverse\" works as intended ------\n");

        var Ainv = Mactrics.IsIdentityMatrix(A);

        Console.WriteLine($"--- Generate a random square matrix A ({N2}x{N2}) ---\n");
        Console.WriteLine($"Matrix A:");
        Console.WriteLine(Mactrics.PrintMatrix(Ainv));
        
        Console.WriteLine("--- Factorize A into QR ---\n");
        var qrInv = new QR(Ainv);
        Console.WriteLine("Matrix Q:");
        Console.WriteLine(Mactrics.PrintMatrix(qrInv.Q));
        Console.WriteLine("\nMatrix R:");
        Console.WriteLine(Mactrics.PrintMatrix(qrInv.R));

        Console.WriteLine("\n--- Calculate B, which is the inverse of A ---\n");
        var B = qrInv.inverse();
        Console.WriteLine("Matrix B:");
        Console.WriteLine(Mactrics.PrintMatrix(B, 4));

        var AB = matrix.Multiply(Ainv, B);
        Console.WriteLine("\n--- Check that AB≈I, where I is the identity matrix ---\n");
        Console.WriteLine("Matrix AB:");
        Console.WriteLine(Mactrics.PrintMatrix(AB, 4));

        double maxDiff = 0;
        for (int i = 0; i < N2; i++)
            for (int j = 0; j < N2; j++) {
                double expected = (i == j) ? 1.0 : 0.0;
                maxDiff = Math.Max(maxDiff, Math.Abs(AB[i, j] - expected));
            }
        Console.WriteLine("\nTEST: Is AB≈I (within a tolerance of 1e-12)?");
        Console.WriteLine(maxDiff < 1e-12 ? "RESULT: Yes, AB≈I.\n" : "RESULT: No, AB≠I.\n");
        // Console.WriteLine(maxDiff < 1e-12 ? "True\n" : "False\n");

        // ---------- TASK C ----------
        Console.WriteLine("\n------------ TASK C: Operations count for QR-decomposition ------------\n");
        Console.WriteLine("------ Measure the time it takes to QR-factorize a random NxN matrix as function of N ------\n");
        
        Console.WriteLine("QR_factorize_time.txt contains the data on how long it takes to QR-factorize a random NxN matrix.");
        Console.WriteLine("QR_factorize_time.svg is a plot showing how long it takes to QR-factorize a random NxN matrix, using the data from QR_factorize_time.txt.");
        Console.WriteLine("The time it takes to QR-factorize grows like O(N³), as shown by the fit in QR_factorize_time.svg.");

        // ---------- Points from task A, B, and C ----------
        int HW_POINTS_A = 1;
        int HW_POINTS_B = 1;
        int HW_POINTS_C = 1;
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);  
        */
    }
}

