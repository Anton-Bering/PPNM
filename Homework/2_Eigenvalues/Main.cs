using System;
using System.IO;
using System.Linq;

public static class Program
{
    public static void Main(string[] args)
    {   
        Console.WriteLine("------------ TASK A: Jacobi diagonalization with cyclic sweeps ------------\n");

        Console.WriteLine("------ Prove that the implementation works as intended ------\n");

        const int sizeA = 5;
        Console.WriteLine($"--- Generate a random symmetric matrix A ({sizeA}x{sizeA}) ---\n");
        double[,] A = VectorAndMatrix.RandomMatrix(sizeA, sizeA);
        VectorAndMatrix.Symmetrize(A); // Gør A symmetrisk
        Console.WriteLine(VectorAndMatrix.PrintMatrix(A, "A"));

        Console.WriteLine("\n--- Apply your routine to perform the eigenvalue-decomposition, A=VDVᵀ ---");
        Console.WriteLine("--- (where V is the orthogonal matrix of eigenvectors                  ---");
        Console.WriteLine("--- and D is the diagonal matrix with the corresponding eigenvalues)   ---\n"); 

        /* ---------- Diagonalise ---------- */
        double[,] A_orig = (double[,])A.Clone();   // keep original for tests
        double[] w       = new double[sizeA];
        double[,] V      = new double[sizeA, sizeA];

        jacobi.cyclic(A, w, V);

        Console.WriteLine("The orthogonal matrix V of eigenvectors:\n");
        Console.WriteLine(VectorAndMatrix.PrintMatrix(V, "V"));

        double[,] D = VectorAndMatrix.DiagonalMatrix(w);
        Console.WriteLine("\nThe diagonal matrix D with the corresponding eigenvalues:\n");
        Console.WriteLine(VectorAndMatrix.PrintMatrix(D, "D"));

        Console.WriteLine("\nThe vector w of the eigenvalues:\n");
        Console.WriteLine(VectorAndMatrix.PrintVector(w, "w"));

        double[,] V_T = VectorAndMatrix.Transpose(V);
        Console.WriteLine("\nThe matrix Vᵀ (which is the transpose of V)\n");
        Console.WriteLine(VectorAndMatrix.PrintMatrix(V_T, "Vᵀ"));            

        /* ---------- Checks ---------- */

        Console.WriteLine("\n--- Check that VᵀAV≈D ---\n");
        double[,] VtAV = VectorAndMatrix.Multiply(V_T, VectorAndMatrix.Multiply(A_orig, V));
        Console.WriteLine(VectorAndMatrix.PrintMatrix(VtAV, "VᵀAV"));
        VectorAndMatrix.CheckMatrixEqual(VtAV, D, "VᵀAV", "D");

        Console.WriteLine("\n--- Check that VDVᵀ≈A ---\n");
        double[,] VDVt = VectorAndMatrix.Multiply(
                                VectorAndMatrix.Multiply(V, D),
                                V_T);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(VDVt, "VDVᵀ"));
        VectorAndMatrix.CheckMatrixEqual(VDVt, A_orig, "VDVᵀ", "A");

        Console.WriteLine("\n--- Check that VᵀV≈I ---\n");
        double[,] VtV = VectorAndMatrix.Multiply(V_T, V);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(VtV, "VᵀV"));
        VectorAndMatrix.CheckIdentityMatrix(VtV, "VᵀV");

        Console.WriteLine("\n--- Check that VVᵀ≈I ---\n");
        double[,] VVt = VectorAndMatrix.Multiply(V, V_T);
        Console.WriteLine(VectorAndMatrix.PrintMatrix(VVt, "VVᵀ"));
        VectorAndMatrix.CheckIdentityMatrix(VVt, "VVᵀ");

        /* ---------- TASK B  (Hydrogen atom) ---------- */
        Console.WriteLine("\n------------ TASK B: Hydrogen atom, s-wave radial Schrödinger equation on a grid -----------\n");

        Console.WriteLine("------ Calculate numerically the lowest egenvalues and eigenfunctions ------");
        Console.WriteLine("------ of the s-wave states in the hydrogen atom                      ------");
        Console.WriteLine("------ and compare them with the exact results                        ------\n");

        double fixed_rmax = 10.0;
        double fixed_dr   = 0.3;
        int npoints_main  = (int)(fixed_rmax / fixed_dr) - 1;

        double[,] H_main  = BuildHamiltonian(npoints_main, fixed_dr);
        double[]  w_main  = new double[npoints_main];
        double[,] V_main  = new double[npoints_main, npoints_main];
        jacobi.cyclic(H_main, w_main, V_main);

        Console.WriteLine("Numerically calculated lowest eigenvalues:");
        for (int i = 0; i < 5 && i < w_main.Length; i++)
            Console.WriteLine($"ε_{i} = {w_main[i]:F6}");

        /* save eigenfunctions (first 3) */
        double normConst = 1.0 / Math.Sqrt(fixed_dr);
        for (int k = 0; k < 3; k++)
        using (var swNum = new StreamWriter($"numerically_eigenfunctions_n{k + 1}.txt")) // Skal den her linje ændrer/slettes?
        using (var swAna = new StreamWriter($"analytic_eigenfunctions_n{k + 1}.txt")) // Skal den her linje ændrer/slettes?
        {
            for (int i = 0; i < npoints_main; i++)
            {
                double r = fixed_dr * (i + 1);
                double f = normConst * V_main[i, k];
                swNum.WriteLine($"{r} {f}");

                double fa;
                switch (k)           //  <-- C#‑7.3‑compatible
                {
                    case 0:
                        fa = 2 * r * Math.Exp(-r);
                        break;
                    case 1:
                        fa = -(1.0 / Math.Sqrt(2)) * (1 - r / 2) * r * Math.Exp(-r / 2);
                        break;
                    case 2:
                        fa = (2.0 / (81 * Math.Sqrt(3))) *
                                (27 - 18 * r + 2 * r * r) * r * Math.Exp(-r / 3);
                        break;
                    default:
                        fa = 0.0;
                        break;
                }
                swAna.WriteLine($"{r} {fa}");
            }
        }
        Console.WriteLine("\nnumerically_eigenfunctions_ni.txt (for i = 1, 2, 3) contains the data for the numerically calculated eigenfunctions with n = 1, 2, 3, respectively.");
        Console.WriteLine("eigenfunctions.svg is a plot of the numerically calculated eigenfunctions.");
        
        Console.WriteLine("\nanalytic_eigenfunctions_ni.txt (for i = 1, 2, 3) contains the data for the (exact) analytically computed eigenfunctions with n = 1, 2, 3, respectively.");
        Console.WriteLine("eigenfunctions.svg also contains the exact results for comparison.");


        Console.WriteLine("\n------ Fix r_max and calculate ε₀ for several different values of Δr ------");
        Console.WriteLine("------ and plot the resulting curve                                  ------\n");
        var drs = Enumerable.Range(1, 100).Select(i => i * 0.01);
        using (var drWriter = new StreamWriter("varying_dr.txt"))
        {
            drWriter.WriteLine("dr, E0");                               // CHANGED: correct writer name
            foreach (double dr in drs)
            {
                int npoints = (int)(fixed_rmax / dr) - 1;
                if (npoints < 1) continue;
                double[,] H = BuildHamiltonian(npoints, dr);
                double[] ew = new double[npoints];
                double[,] ev = new double[npoints, npoints];
                jacobi.cyclic(H, ew, ev);
                drWriter.WriteLine($"{dr}, {ew.Min()}");
            }
        }
        
        Console.WriteLine($"r_max is fixed to {fixed_rmax}.");
        Console.WriteLine("varying_dr.txt contains the data for the calculated ε₀.");
        Console.WriteLine("varying_dr.svg is a plot of the resulting curve.");

        Console.WriteLine("\n------ Fix Δr and calculate ε₀ for several different values of r_max ------");
        Console.WriteLine("------ and plot the resulting curve                                  ------\n");
        var rmaxs = Enumerable.Range(8, 33).Select(i => i * 0.5);
        using (var rmWriter = new StreamWriter("varying_rmax.txt"))
        {
            rmWriter.WriteLine("rmax, E0");                             // CHANGED: correct writer name
            foreach (double rmax in rmaxs)
            {
                int npoints = (int)(rmax / fixed_dr) - 1;
                if (npoints < 1) continue;
                double[,] H = BuildHamiltonian(npoints, fixed_dr);
                double[] ew = new double[npoints];
                double[,] ev = new double[npoints, npoints];
                jacobi.cyclic(H, ew, ev);
                rmWriter.WriteLine($"{rmax}, {ew.Min()}");
            }
        }
        
        Console.WriteLine($"\nΔr is fixed to {fixed_dr}.");
        Console.WriteLine("varying_rmax.txt contains the data for the calculated ε₀.");
        Console.WriteLine("varying_rmax.svg is a plot of the resulting curve.");

        /* ---------- TASK C  (Scaling) ---------- */
        Console.WriteLine("\n------------ TASK C: Scaling and optimization ------------\n");
        
        Console.WriteLine("------ Check that the number of operations for matrix diagonalization scales as O(n³) ------"); 
        Console.WriteLine("------ do the measurements in parallel                                                ------\n");

        Console.WriteLine("The file 'number_of_operations.txt' contains timing data for Jacobi diagonalization of random matrices of size N."); // HERHER: tjek at det sker
        Console.WriteLine("The plot 'number_of_operations.svg' shows the data along with a fitted curve f(N) = a * N³.\n"); // HERHER: Tjek at det faktisk sker

        Console.WriteLine("Timing data are written by NumberOfOperations.exe to number_of_operations.txt.");

        /* HERHER: OBS: jeg havde det her i en tidlige udgave:
        string timefile = "number_of_operations_time.txt";
        if (File.Exists(timefile)) {
            Console.WriteLine("\nMeasured wall-clock timing of parallel execution (from GNU time):");
            foreach (var line in File.ReadAllLines(timefile))
                Console.WriteLine(line);
        }
        */

        /* ------------ Homework points ------------*/
        int HW_POINTS_A = 1, HW_POINTS_B = 1, HW_POINTS_C = 1;
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
    
    }

    /* ------------------------------------------------------------------  helpers  ------------------------------------------------------------------ */

    static double[,] BuildHamiltonian(int n, double dr)
    {
        double[,] H = new double[n, n];
        double invdr2 = 1.0 / (dr * dr);

        for (int i = 0; i < n - 1; i++)
        {
            H[i, i]       += invdr2;
            H[i, i + 1]   += -0.5 * invdr2;
            H[i + 1, i]   += -0.5 * invdr2;
        }
        H[n - 1, n - 1] += invdr2;

        for (int i = 0; i < n; i++)
        {
            double ri = dr * (i + 1);
            H[i, i] += -1.0 / ri;
        }
        return H;
    }
}
