/*  Main.cs
 *  Simple test-driver for vecotr_and_mactrics.cs
 */

using System;

class MainClass
{


    public static void Main()
    {
        int n = 3;
        int m = 4;

        /* ----- random matrix A ----- */
        var A = VectorAndMatrix.RandomMatrix(n, m);
        Console.WriteLine($"Here is the random matrix {n} x {m} A:{VectorAndMatrix.MatrixToString(A)}");

        /* ----- transpose of A ----- */
        var AT = VectorAndMatrix.Transpose(A);
        Console.WriteLine("The transpose of A is:\n" + VectorAndMatrix.MatrixToString(AT));

        /* ----- identity matrix I ----- */
        var I  = VectorAndMatrix.IdentityMatrix(n);
        Console.WriteLine($"The identity matrix of size {n}:\n{VectorAndMatrix.MatrixToString(I)}");

        /* ----- size of A ----- */
        var (rows, cols) = VectorAndMatrix.SizeOfMatrix(A);
        Console.WriteLine($"The size of A is: {rows} x {cols}");

        /* ----- random square matrix B ----- */
        var B = VectorAndMatrix.RandomMatrix(n, n);
        Console.WriteLine($"Here is the random square matrix B:\n{VectorAndMatrix.MatrixToString(B)}");
        Console.WriteLine($"The size of B is: {VectorAndMatrix.SizeOfSquareMatrix(B)}");

        /* ----- checks on B ----- */
        VectorAndMatrix.CheckUpperTriangular(B, "B");
        VectorAndMatrix.CheckLowerTriangular(B, "B");
        VectorAndMatrix.CheckMatrixEqual(B, A, "B", "A");
        VectorAndMatrix.CheckIdentityMatrix(B, "B");

        /* ----- random vector V ----- */
        var V = VectorAndMatrix.RandomVector(n);
        Console.WriteLine($"Here is the random vector V of size {n}:\n{VectorAndMatrix.VectorToString(V)}");

        var A_times_AT= VectorAndMatrix.MultiplyMatrices(A, AT);
        Console.WriteLine($"The product of A*A^T is:\n{VectorAndMatrix.MatrixToString(A_times_AT)}");
    }
}
