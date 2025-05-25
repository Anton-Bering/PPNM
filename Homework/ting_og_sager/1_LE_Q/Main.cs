using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

// Task A: Solving linear equations using QR-decomposition by modified Gram-Schmidt orthogonalization
// Task B: Matrix inverse by Gram-Schmidt QR factorization
// Task C: Operations count for QR-decomposition

class Program
{
    static Random random = new Random();

    // Generates a matrix with random values between 0 and 1
    static Matrix RandomMatrix(int rows, int cols)
    {
        var A = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                A[i, j] = random.NextDouble();
        return A;
    }

    // Generates a vector with random values between 0 and 1
    static Vector RandomVector(int size)
    {
        var v = new Vector(size);
        for (int i = 0; i < size; i++)
            v[i] = random.NextDouble();
        return v;
    }

    // Performs QR factorization and logs timing data to a file
    static void BenchmarkQR(int size, StreamWriter writer)
    {
        Matrix A = RandomMatrix(size, size);
        var stopwatch = Stopwatch.StartNew();
        QR qr = new QR(A);
        stopwatch.Stop();
        writer.WriteLine($"{size} {stopwatch.Elapsed.TotalSeconds}");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("\n TASK A, CHECK 1, Check that your decomp works as intended:");

        // Generate a random non-square matrix A
        int rows = 7;
        int cols = 4;
        Console.WriteLine("\n generate a random tall (n>m) matrix A (of a modest size);");
        Matrix A = RandomMatrix(rows, cols);
        A.Print("A=");

        // Perform QR decomposition
        Console.WriteLine("\n factorize it into QR;");
        QR qr = new QR(A);
        Matrix Q = qr.Q;
        Matrix R = qr.R;
        Q.Print("Q=");
        R.Print("R=");

        // Check if R is upper triangular
        Console.WriteLine("\n check that R is upper triangular;");
        if (R.IsUpTri())
            Console.WriteLine("R is upper triangular");
        else
            Console.WriteLine("R is not upper triangular");

        // Check if Q is orthogonal: Q^T * Q ≈ I
        Console.WriteLine("\n check that Q^T Q = 1;");
        Matrix QT = Q.Transpose();
        Matrix QTQ = QT * Q;
        QTQ.Print("Q^T * Q:");
        Matrix identity = Matrix.Identity(Q.Cols);
        if (Matrix.Compare(QTQ, identity))
            Console.WriteLine("Q^T * Q is the identity matrix.");
        else
            Console.WriteLine("Q^T * Q is not the identity matrix.");

        // Check if A ≈ Q * R
        Console.WriteLine("\n check that QR=A;");
        Matrix QRproduct = Q * R;
        QRproduct.Print("Q * R:");
        if (Matrix.Compare(QRproduct, A))
            Console.WriteLine("Q * R equals A.");
        else
            Console.WriteLine("Q * R does not equal A.");


        Console.WriteLine("\n TASK A, CHECK 2, Check that you solve works as intended:");

        // Generate a random square matrix and vector
        Console.WriteLine("\n generate a random square matrix A (of a modest size);");
        Matrix Asquare = RandomMatrix(cols, cols);
        Asquare.Print("A=");
        Console.WriteLine("\n generate a random vector b (of the same size);");
        Vector b = RandomVector(cols);
        b.Print("b=");

        Console.WriteLine("\n factorize A into QR;");
        QR qr2 = new QR(Asquare);
        Vector x = qr2.Solve(b);
        x.Print("x:");

        Console.WriteLine("\n solve QRx=b");

        Console.WriteLine("\n check that Ax=b;");
        Vector Ax = Asquare * x;
        Ax.Print("A*x=");
        if (Vector.Compare(Ax, b))
            Console.WriteLine("A*x equals b.");
        else
            Console.WriteLine("A*x does not equal b.");

        // TASK B: Matrix inverse by Gram-Schmidt QR factorization
        // Add the function/method "inverse" to your "QRGS" class that, using the calculated Q and R, should calculate the inverse of the matrix A and returns it.
        
        Console.WriteLine("\n TASK B, CHECK, Check that you function works as intended:");

        // Generate a random square matrix A
        Console.WriteLine("\n generate a random square matrix A (of a modest size);");
        Matrix Asquare_taskB = RandomMatrix(cols, cols);
        Asquare_taskB.Print("A=");

        Console.WriteLine("\n factorize A into QR;");

        Console.WriteLine("\n calculate the inverse B;");
        QR qrB = new QR(Asquare_taskB);
        Matrix B = qrB.Inverse();
        B.Print("B=");

        Console.WriteLine("\n check that AB=I, where I is the identity matrix;");
        Matrix AB = Asquare_taskB * B;
        AB.Print("A * B:");
        Matrix identity2 = Matrix.Identity(cols);
        if (Matrix.Compare(AB, identity2))
            Console.WriteLine("A * B equals I.");
        else
            Console.WriteLine("A * B does not equal I.");

    }
}
