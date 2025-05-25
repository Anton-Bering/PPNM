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
        Console.WriteLine("\nTASK A, CHECK 1, Check that your decomp works as intended:");

        int rows = 7;
        int cols = 4;

        // Generate a random non-square matrix A
        Console.WriteLine("\n generate a random tall (n>m) matrix A (of a modest size);");
        Matrix A = RandomMatrix(rows, cols);
        A.Print("A:");

        // Perform QR decomposition
        Console.WriteLine("\n factorize it into QR;");
        QR qr = new QR(A);
        Matrix Q = qr.Q;
        Matrix R = qr.R;
        Q.Print("Q:");
        R.Print("R:");

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


        Console.WriteLine("\nTASK A, CHECK 2, Check that you solve works as intended:");

        // Generate a random square matrix and vector
        Console.WriteLine("\n generate a random square matrix A (of a modest size);");
        Matrix Asquare = RandomMatrix(cols, cols);
        Asquare.Print("A:");
        Console.WriteLine("\n generate a random vector b (of the same size);");
        Vector b = RandomVector(cols);
        b.Print("b:");

        // Solve Ax = b using QR decomposition
        Console.WriteLine("\n factorize A into QR;");
        QR qr2 = new QR(Asquare);
        Vector x = qr2.Solve(b);
        x.Print("x:");

        // Verify the solution: A * x ≈ b
        Vector Ax = Asquare * x;
        Ax.Print("A * x:");
        if (Vector.Compare(Ax, b))
            Console.WriteLine("A * x equals b.");
        else
            Console.WriteLine("A * x does not equal b.");

        // Repeat to double-check
        Ax = Asquare * x;
        Ax.Print("A * x (again):");
        if (Vector.Compare(Ax, b))
            Console.WriteLine("A * x equals b.");
        else
            Console.WriteLine("A * x does not equal b.");

        Console.WriteLine("\nTask B: Determinant and Inverse");

        // Calculate absolute value of determinant using R from QR
        double detA = qr.Det();
        Console.WriteLine($"abs(det) of A via R: {detA}");

        double detAsquare = qr2.Det();
        Console.WriteLine($"abs(det) of A_square via R: {detAsquare}");

        // Compute inverse of A using QR
        Matrix B = qr2.Inverse();
        B.Print("Inverse B:");

        // Verify A * B ≈ I
        Matrix AB = Asquare * B;
        AB.Print("A * B:");
        Matrix identity2 = Matrix.Identity(cols);
        if (Matrix.Compare(AB, identity2))
            Console.WriteLine("A * B equals I.");
        else
            Console.WriteLine("A * B does not equal I.");

        // Benchmark QR decomposition for increasing matrix sizes
        using (StreamWriter writer = new StreamWriter("benchmark.csv"))
        {
            writer.WriteLine("n, time");
            for (int n = 3; n <= 300; n++)
            {
                BenchmarkQR(n, writer);
            }
        }

        Console.WriteLine("Benchmark complete. Results saved to benchmark.csv");
    }
}
