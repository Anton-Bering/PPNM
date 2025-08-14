using static MatrixHelpers;
using static VectorHelpers;
using System;

public class MainClass
{
    public static void Main(string[] args)
    {

        /* ---------- Til Task C: Generate a random N×N matrix and QR‑factorise it ---------- */
        foreach (var a in args)
            if (a.StartsWith("-size:", StringComparison.Ordinal))
            {
                int N = int.Parse(a.Substring(6));
                var Random_N_times_N_Matrix = RandomMatrix(N, N);  
                new QR(Random_N_times_N_Matrix);                   
                return;                     
            }
        /* ----------------------------------------------------------- */

        // ---------- TASK A ----------
        Console.WriteLine("------------ TASK A: Solving linear equations using QR-decomposition ------------");
        Console.WriteLine("------------ by modified Gram-Schmidt orthogonalization              ------------\n");

        /*---------------- CHECK that Decomp works -----------------------*/
        Console.WriteLine("------ Check that \"decomp\" works as intended ------\n");

        int n = 6, m = 4;
        Console.WriteLine($"--- Generate a random tall ({n}x{m}) matrix A ---\n");
        var A = RandomMatrix(n, m);
        Console.WriteLine(PrintMatrix(A, "A"));

        var qrA = new QR(A);
        Console.WriteLine("--- Factorize A into QR ---\n");
        Console.WriteLine(PrintMatrix(qrA.Q, "Q"));
        Console.WriteLine(PrintMatrix(qrA.R, "R"));

        Console.WriteLine("\n--- Check that R is upper triangular ---\n");
        CheckUpperTriangular(qrA.R, "R");

        Console.WriteLine("\n--- Check that QᵀQ ≈ I ---\n");
        var QtQ = qrA.Q.Transpose() * qrA.Q;
        Console.WriteLine(PrintMatrix(QtQ, "QᵀQ"));
        CheckIdentityMatrix(QtQ, "QᵀQ");

        Console.WriteLine("\n--- Check that QR ≈ A ---\n");
        var QRmat = qrA.Q * qrA.R;
        Console.WriteLine(PrintMatrix(QRmat, "QR"));
        CheckMatrixEqual(QRmat, A, "QR", "A");


        /*---------------- CHECK that Solve works -----------------------*/
        Console.WriteLine("\n------ Check that \"solve\" works as intended ------\n");

        int N2 = 4;
        Console.WriteLine($"--- Generate a random square matrix A ({N2}×{N2}) ---\n");
        var A_sq = RandomMatrix(N2, N2);
        Console.WriteLine(PrintMatrix(A_sq, "A"));

        Console.WriteLine($"--- Generate a random vector b ({N2}) ---\n");
        var b = RandomVector(N2);
        Console.WriteLine(PrintVector(b, "b"));

        Console.WriteLine($"\n--- Factorize A into QR --- \n");
        var qrSolve = new QR(A_sq);
        Console.WriteLine(PrintMatrix(qrSolve.Q, "Q"));
        Console.WriteLine(PrintMatrix(qrSolve.R, "R"));

        Console.WriteLine($"\n--- Solve QRx=b ---\n");
        var x = qrSolve.solve(b);
        Console.WriteLine("Solving QRx=b leads to:\n");
        Console.WriteLine(PrintVector(x, "x"));

        Console.WriteLine("\n--- Check that Ax≈b ---\n");
        var Ax = A_sq * x;
        Console.WriteLine(PrintVector(Ax, "Ax"));
        CheckVectorEqual(Ax, b, "Ax", "b");


        // ---------- TASK B ----------
        Console.WriteLine("\n------------ TASK B: Matrix inverse by Gram-Schmidt QR factorization ------------\n");
        
        /*---------------- CHECK that Inverse works -----------------------*/
        Console.WriteLine("------ Check that \"inverse\" works as intended ------\n");

        Console.WriteLine($"--- Generate a random square matrix A ({N2}×{N2}) ---\n");
        var A_sq_task_B = RandomMatrix(N2, N2);
        Console.WriteLine(PrintMatrix(A_sq_task_B, "A")); 

        Console.WriteLine("\n--- Factorize A into QR ---\n");
        var qrA_sq_task_B = new QR(A_sq_task_B);
        Console.WriteLine(PrintMatrix(qrA_sq_task_B.Q, "Q"));
        Console.WriteLine(PrintMatrix(qrA_sq_task_B.R, "R"));

        Console.WriteLine("\n--- Calculate B, which is the inverse of A ---\n");
        var B = qrA_sq_task_B.inverse();
        Console.WriteLine(PrintMatrix(B, "B"));

        Console.WriteLine("\n--- Check that AB≈I, where I is the identity matrix ---\n");
        var AB = A_sq_task_B * B; 
        Console.WriteLine(PrintMatrix(AB, "AB"));
        CheckIdentityMatrix(AB, "AB");


        /*---------------- TASK C: Text‑only summary --------------------*/
        Console.WriteLine("\n------------ TASK C: ------------\n");
        Console.WriteLine("The data and plot for QR-factorizing a random N×N matrix are in the Out_Task_C folder.");

        /* Homework points ---------------------------------------------*/
        int HW_POINTS_A = 1, HW_POINTS_B = 1, HW_POINTS_C = 1;
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
    }
}
