using System;
using System.Diagnostics;

class MainClass {
    static void Main(string[] args) {
        if (args.Length > 0 && args[0].StartsWith("-size:")) {
            int size = int.Parse(args[0].Substring(6));
            TimeDecomp(size);
        } else {
            TestDecomp();
            TestSolve();
            TestInverse();
            Console.WriteLine("---------- PART C ----------");
            Console.WriteLine("Timing data written to timing.txt");
        }
    }

    static void TestDecomp() {
        Console.WriteLine("---------- PART A ----------");
        Console.WriteLine("----- Check \"decomp\" -----");
        var rnd = new Random(1);
        int n = 7, m = 4;
        matrix A = matrix.random(n, m, rnd);

        Console.WriteLine($"Random matrix A ({n}x{m}):\n{A.ToStringFormatted()}");

        var (Q, R) = QR.decomp(A);
        Console.WriteLine("QR decomposition yielded:\nMatrix Q:\n");
        Console.WriteLine(Q.ToStringFormatted());
        Console.WriteLine("\nMatrix R:\n");
        Console.WriteLine(R.ToStringFormatted());

        matrix QtQ = Q.transpose() * Q;
        Console.WriteLine("The product Q^T * Q yields:\n");
        Console.WriteLine(QtQ.ToStringFormatted());

        Console.WriteLine("Is Q^T * Q equal to the identity matrix within a tolerance?");
        Console.WriteLine(QtQ.isApprox(matrix.identity(m), 1e-9) + "\n");

        matrix QRProduct = Q * R;
        Console.WriteLine("Checking if A = QR. Multiplying Q and R yields:\n");
        Console.WriteLine(QRProduct.ToStringFormatted());
        Console.WriteLine("Is A - QR = 0 within a tolerance?");
        Console.WriteLine(QRProduct.isApprox(A, 1e-9) + "\n");
    }

    static void TestSolve() {
        Console.WriteLine("---------- PART B ----------");
        Console.WriteLine("----- Check \"solve\" -----");
        var rnd = new Random(1);
        int n = 4;
        matrix A = matrix.random(n, n, rnd);
        vector b = new vector(n);
        for (int i = 0; i < n; i++) b[i] = rnd.NextDouble();

        Console.WriteLine($"Random square matrix A ({n}x{n}):\n{A.ToStringFormatted()}");
        Console.WriteLine($"\nRandom vector b ({n}):\n{b.ToStringFormatted()}\n");

        var (Q, R) = QR.decomp(A);
        vector x = QR.solve(Q, R, b);
        Console.WriteLine($"Solving QRx = b yields the following x:\n{x.ToStringFormatted()}\n");

        vector Ax = A * x;
        Console.WriteLine("Checking if Ax = b. Multiplying A and x yields:\n");
        Console.WriteLine(Ax.ToStringFormatted());
        Console.WriteLine("\nIs A*x - b = 0 within a tolerance?");
        Console.WriteLine(Ax.isApprox(b, 1e-9) + "\n");
    }

    static void TestInverse() {
        Console.WriteLine("----- Check \"inverse\" -----");
        var rnd = new Random(1);
        int n = 4;
        matrix A = matrix.random(n, n, rnd);

        Console.WriteLine($"Random square matrix A ({n}x{n}):\n{A.ToStringFormatted()}");
        var (Q, R) = QR.decomp(A);
        matrix B = QR.inverse(Q, R);

        Console.WriteLine($"\nCalculated inverse of A using QR method:\n{B.ToStringFormatted()}");
        matrix AB = A * B;
        Console.WriteLine("\nChecking if A * B = I. Multiplying A and B yields:\n");
        Console.WriteLine(AB.ToStringFormatted());
        Console.WriteLine("\nIs A*B - I = 0 within a tolerance?");
        Console.WriteLine(AB.isApprox(matrix.identity(n), 1e-9));
    }

    static void TimeDecomp(int n) {
        var rnd = new Random(1);
        matrix A = matrix.random(n, n, rnd);
        
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var (Q, R) = QR.decomp(A); // Dekomponering sker her
        sw.Stop();
        
        // Brug variablerne til at undgå advarsler
        _ = Q.size1 + R.size1; // Dummy-operation
        
        Console.WriteLine($"{n} {sw.Elapsed.TotalSeconds}");
    }
}