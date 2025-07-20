using System;
using System.Text;

public class MainClass
{
    /*------------------------------------------------------------------*/
    public static void Main(string[] args)
    {

        /* ---------- TIMING MODE (for make out.times.data) ---------- */
        foreach (string a in args)
            if (a.StartsWith("-size:", StringComparison.Ordinal))
            {
                int N = int.Parse(a.Substring(6));

                // Generate a random N×N matrix and QR‑factorise it – nothing else.
                double[,] Random_N_times_N_Matrix = VectorAndMatrix.RandomMatrix(N, N);
                var qr      = new QR(Random_N_times_N_Matrix);

                // IMPORTANT: do **not** print anything – I/O would dominate the timing.
                return;     // leave Main() early, skipping the demo code below
            }
        /* ----------------------------------------------------------- */



        Console.WriteLine("------------ TASK A: Solving linear equations using QR-decomposition ------------");
        Console.WriteLine("------------ by modified Gram-Schmidt orthogonalization              ------------\n");

        Console.WriteLine("------ Check that \"decomp\" works as intended ------\n");

        int n = 6, m = 4;
        Console.WriteLine($"--- Generate a random tall ({n}x{m}) matrix A ---\n");
        double[,] A = VectorAndMatrix.RandomMatrix(n, m);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(A, "A"));

        var qrA = new QR(A);
        Console.WriteLine("--- Factorize A into QR ---\n");
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrA.Q, "Q"));
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrA.R, "R"));

        /* Upper‑triangular test ----------------------------------------*/
        Console.WriteLine("\n--- Check that R is upper triangular ---\n");
        VectorAndMatrix.CheckUpperTriangular(qrA.R, "R");

        /* Qᵀ Q ≈ I test -----------------------------------------------*/
        Console.WriteLine("\n--- Check that QᵀQ ≈ I ---\n");
        double[,] QtQ = VectorAndMatrix.Multiply(
                            VectorAndMatrix.Transpose(qrA.Q), qrA.Q);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(QtQ, "QᵀQ"));
        VectorAndMatrix.CheckIdentityMatrix(QtQ, "QᵀQ");

        /* QR ≈ A test --------------------------------------------------*/
        Console.WriteLine("\n--- Check that QR ≈ A ---\n");
        double[,] QRmat = VectorAndMatrix.Multiply(qrA.Q, qrA.R);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(QRmat, "QR"));
        VectorAndMatrix.CheckMatrixEqual(QRmat, A, "QR", "A");

        /*---------------- CHECK that Solve works -----------------------*/
        Console.WriteLine("\n------ Check that \"solve\" works as intended ------\n");

        int N2 = 4;
        Console.WriteLine($"--- Generate a random square matrix A ({N2}×{N2}) ---\n");
        double[,] A_sq = VectorAndMatrix.RandomMatrix(N2, N2);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(A_sq, "A"));
        Console.WriteLine($"--- Generate a random vector b ({N2}) ---\n");
        double[]  b    = VectorAndMatrix.RandomVector(N2);
        Console.WriteLine(VectorAndMatrix.PrintVector(b, "b"));

        Console.WriteLine($"\n--- Factorize A into QR --- \n");
        var qrSolve = new QR(A_sq);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrSolve.Q, "Q"));
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrSolve.R, "R"));

        Console.WriteLine($"\n--- Solve QRx=b ---\n");
        double[] x = qrSolve.solve(b);
        Console.WriteLine("Solving QRx=b leads to:\n");
        Console.WriteLine(VectorAndMatrix.PrintVector(x, "x"));

        Console.WriteLine("\n--- Check that Ax≈b ---\n");
        double[] Ax = VectorAndMatrix.Multiply(A_sq, x);
        Console.WriteLine(VectorAndMatrix.PrintVector(Ax, "Ax"));
        VectorAndMatrix.CheckVectorEqual(Ax, b, "Ax", "b");

        // ---------- TASK B ----------
        Console.WriteLine("\n------------ TASK B: Matrix inverse by Gram-Schmidt QR factorization ------------\n");
        Console.WriteLine("------ Check that \"inverse\" works as intended ------\n");

        Console.WriteLine($"--- Generate a random square matrix A ({N2}×{N2}) ---\n");
        double[,] A_sq_task_B = VectorAndMatrix.RandomMatrix(N2, N2);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(A_sq_task_B, "A")); 

        Console.WriteLine("\n--- Factorize A into QR ---\n");
        var qrA_sq_task_B = new QR(A_sq_task_B);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrA_sq_task_B.Q, "Q"));
        Console.WriteLine(VectorAndMatrix.PrintMatrix(qrA_sq_task_B.R, "R"));

        Console.WriteLine("\n--- Calculate B, which is the inverse of A ---\n");
        double[,] B = qrA_sq_task_B.inverse();
        Console.WriteLine(VectorAndMatrix.PrintMatrix(B, "B"));

        Console.WriteLine("\n--- Check that AB≈I, where I is the identity matrix ---\n");
        double[,] AB = VectorAndMatrix.Multiply(A_sq_task_B, B);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(AB, "AB"));
        VectorAndMatrix.CheckIdentityMatrix(AB, "AB");

        /*---------------- TASK C: Text‑only summary --------------------*/
        Console.WriteLine("\n------------ TASK C: Operations count for QR-decomposition ------------\n");
        Console.WriteLine("------ Measure the time it takes to QR-factorize a random NxN matrix as function of N ------\n");
        
        Console.WriteLine("out.times.data contains the data on how long it takes to QR-factorize a random NxN matrix.");
        Console.WriteLine("QR_factorize_time.svg is a plot showing how long it takes to QR-factorize a random NxN matrix, using the data from out.times.data.");
        Console.WriteLine("The time it takes to QR-factorize grows like O(N³), as shown by the fit in QR_factorize_time.svg.");


        /* Homework points ---------------------------------------------*/
        int HW_POINTS_A = 1, HW_POINTS_B = 1, HW_POINTS_C = 1;
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
    }
}
