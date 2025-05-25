using System;

class Program {
    static void Main(string[] args) {
        // Parse optional command-line arguments
        double rmax = 0, dr = 0;
        int sizeN = 0;
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == "-rmax" && i + 1 < args.Length)
                rmax = double.Parse(args[i + 1]);
            if (args[i] == "-dr" && i + 1 < args.Length)
                dr = double.Parse(args[i + 1]);
            if (args[i].StartsWith("-size:"))
                int.TryParse(args[i].Substring(6), out sizeN);
        }

        // ---------- PART C ----------
        if (sizeN > 0) {
            Console.WriteLine("---------- PART C ----------");
            matrix A = new matrix(sizeN, sizeN);
            var rand = new Random(1);
            for (int i = 0; i < sizeN; i++) {
                for (int j = i; j < sizeN; j++) {
                    double x = rand.NextDouble();
                    A[i, j] = A[j, i] = x;
                }
            }
            var (etellerandet_1, etellerandet_2, sweeps) = jacobi.cyclic(A);
            Console.WriteLine($"Matrix size = {sizeN}, Number of Jacobi sweeps = {sweeps}");
            return;
        }

        // ---------- PART B ----------
        if (rmax > 0 && dr > 0) {
            Console.WriteLine("---------- PART B ----------");
            int npoints = (int)(rmax / dr) - 1;
            Console.WriteLine($"Using r_max = {rmax}, dr = {dr} -> npoints = {npoints}");
            vector r = new vector(npoints);
            for (int i = 0; i < npoints; i++) {
                r[i] = dr * (i + 1);
            }

            matrix H = new matrix(npoints, npoints);
            double prefactor = -0.5 / (dr * dr);
            for (int i = 0; i < npoints - 1; i++) {
                H[i, i]     += -2 * prefactor;
                H[i, i + 1] +=  1 * prefactor;
                H[i + 1, i] +=  1 * prefactor;
            }
            H[npoints - 1, npoints - 1] += -2 * prefactor;
            for (int i = 0; i < npoints; i++) {
                H[i, i] += -1.0 / r[i];
            }

            var (w, V, sweeps) = jacobi.cyclic(H);
            Console.WriteLine($"\nNumber of Jacobi sweeps: {sweeps}");

            for (int i = 0; i < w.size; i++) {
                int minIndex = i;
                for (int j = i + 1; j < w.size; j++) {
                    if (w[j] < w[minIndex]) {
                        minIndex = j;
                    }
                }
                if (minIndex != i) {
                    double tmp = w[i];
                    w[i] = w[minIndex];
                    w[minIndex] = tmp;
                    for (int k = 0; k < V.size1; k++) {
                        double tmp2 = V[k, i];
                        V[k, i] = V[k, minIndex];
                        V[k, minIndex] = tmp2;
                    }
                }
            }

            Console.WriteLine("Eigenvalues (lowest first):");
            for (int i = 0; i < w.size; i++) {
                Console.WriteLine($"  E[{i}] = {w[i]:F6}");
            }

            int m = Math.Min(3, w.size);
            Console.WriteLine($"\nRadial grid and first {m} eigenfunctions (columns: r, f0, f1, f2...):");
            for (int j = 0; j < npoints; j++) {
                string line = $"{r[j]:F4}";
                for (int k = 0; k < m; k++) {
                    line += $" {V[j, k]:F6}";
                }
                Console.WriteLine(line);
            }
            return;
        }

        // ---------- PART A ----------
        Console.WriteLine("---------- PART A ----------");
        int N = 5;
        var random = new Random(1);
        matrix A_random = new matrix(N, N);
        for (int i = 0; i < N; i++) {
            for (int j = i; j < N; j++) {
                double x = 10 * (random.NextDouble() - 0.5);
                A_random[i, j] = A_random[j, i] = x;
            }
        }

        Console.WriteLine("Matrix A:");
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                Console.Write($"{A_random[i, j],10:F3}");
            }
            Console.WriteLine();
        }

        var (evals, evecs, sweepCount) = jacobi.cyclic(A_random);
        Console.WriteLine($"\nNumber of Jacobi sweeps: {sweepCount}");

        Console.WriteLine("\nEigenvalues:");
        for (int i = 0; i < evals.size; i++) {
            Console.WriteLine($"  w[{i}] = {evals[i]:F6}");
        }

        // Diagonal matrix D from eigenvalues
        matrix D_diag = new matrix(N, N);
        for (int i = 0; i < N; i++) D_diag[i, i] = evals[i];

        // Compute V^T A V
        matrix VtAV = evecs.transpose() * A_random * evecs;

        Console.WriteLine("\nMatrix V^T A V (should be diagonal with eigenvalues):");
        for (int i = 0; i < N; i++) {
            for (int j = 0; j < N; j++) {
                Console.Write($"{VtAV[i, j],10:F6}");
            }
            Console.WriteLine();
        }

        Console.WriteLine("\nChecking if V^T A V ≈ D:");
        Console.WriteLine(VtAV.approx(D_diag, 1e-6));

        Console.WriteLine("\nChecking if V D V^T ≈ A:");
        matrix VDVt = evecs * D_diag * evecs.transpose();
        Console.WriteLine(VDVt.approx(A_random, 1e-6));

        Console.WriteLine("\nChecking if V^T V ≈ I:");
        matrix VtV = evecs.transpose() * evecs;
        Console.WriteLine(VtV.approx(matrix.id(N)));

        Console.WriteLine("\nChecking if V V^T ≈ I:");
        matrix VVt = evecs * evecs.transpose();
        Console.WriteLine(VVt.approx(matrix.id(N)));
    }
}
