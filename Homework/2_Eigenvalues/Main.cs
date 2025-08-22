using System;
using System.IO;
using System.Linq;
using static MatrixHelpers;
using static VectorHelpers;

public static class Program
{
    public static void Main(string[] args)
    {   
        Console.WriteLine("------------ TASK A: Jacobi diagonalization with cyclic sweeps ------------\n");

        Console.WriteLine("------ Prove that the implementation works as intended ------\n");

        const int sizeA = 5;
        Console.WriteLine($"--- Generate a random symmetric matrix A ({sizeA}x{sizeA}) ---\n");
        matrix A = RandomMatrix(sizeA, sizeA);
        Symmetrize(A); // Gør A symmetrisk
        Console.WriteLine(PrintMatrix(A, "A"));

        Console.WriteLine("\n--- Apply your routine to perform the eigenvalue-decomposition, A=VDVᵀ ---");
        Console.WriteLine("--- (where V is the orthogonal matrix of eigenvectors                  ---");
        Console.WriteLine("--- and D is the diagonal matrix with the corresponding eigenvalues)   ---\n"); 

        /* ---------- Diagonalise ---------- */
        matrix A_orig = A.Copy();                 // keep original for tests
        vector w      = new vector(sizeA);        
        matrix V      = new matrix(sizeA, sizeA); 

        jacobi.cyclic(A, w, V);

        Console.WriteLine("The orthogonal matrix V of eigenvectors:\n");
        Console.WriteLine(PrintMatrix(V, "V"));

        matrix D = DiagonalMatrix(w);
        Console.WriteLine("\nThe diagonal matrix D with the corresponding eigenvalues:\n");
        Console.WriteLine(PrintMatrix(D, "D"));

        Console.WriteLine("\nThe vector w of the eigenvalues:\n");
        Console.WriteLine(PrintVector(w, "w"));

        matrix V_T = V.Transpose();
        Console.WriteLine("\nThe matrix Vᵀ (which is the transpose of V)\n");
        Console.WriteLine(PrintMatrix(V_T, "Vᵀ"));            

        /* ---------- Checks ---------- */

        Console.WriteLine("\n--- Check that VᵀAV≈D ---\n");
        matrix VtAV = V_T * (A_orig * V);
        Console.WriteLine(PrintMatrix(VtAV, "VᵀAV"));
        CheckMatrixEqual(VtAV, D, "VᵀAV", "D");

        Console.WriteLine("\n--- Check that VDVᵀ≈A ---\n");
        matrix VDVt = (V * D) * V_T;
        Console.WriteLine(PrintMatrix(VDVt, "VDVᵀ"));
        CheckMatrixEqual(VDVt, A_orig, "VDVᵀ", "A");

        Console.WriteLine("\n--- Check that VᵀV≈I ---\n");
        matrix VtV = V_T * V;
        Console.WriteLine(PrintMatrix(VtV, "VᵀV"));
        CheckIdentityMatrix(VtV, "VᵀV");

        Console.WriteLine("\n--- Check that VVᵀ≈I ---\n");
        matrix VVt = V * V_T;
        Console.WriteLine(PrintMatrix(VVt, "VVᵀ"));
        CheckIdentityMatrix(VVt, "VVᵀ");

        /* ---------- TASK B  (Hydrogen atom) ---------- */
        Console.WriteLine("\n------------ TASK B: Hydrogen atom, s-wave radial Schrödinger equation on a grid -----------\n");

        Console.WriteLine("------ Calculate numerically the lowest egenvalues and eigenfunctions ------");
        Console.WriteLine("------ of the s-wave states in the hydrogen atom                      ------");
        Console.WriteLine("------ and compare them with the exact results                        ------\n");

        double fixed_rmax = 10.0;
        double fixed_dr   = 0.3;
        int npoints_main  = (int)(fixed_rmax / fixed_dr) - 1;

        matrix H_main  = BuildHamiltonian(npoints_main, fixed_dr);
        vector  w_main  = new vector(npoints_main);
        matrix V_main  = new matrix(npoints_main, npoints_main);
        jacobi.cyclic(H_main, w_main, V_main);

        Console.WriteLine("Numerically calculated lowest eigenvalues:");
        for (int i = 0; i < 5 && i < w_main.Size; i++)
            Console.WriteLine($"ε_{i} = {w_main[i]:F6}");

        /* save eigenfunctions (first 3) */
        double normConst = 1.0 / Math.Sqrt(fixed_dr);
        for (int k = 0; k < 3; k++)
        using (var swNum = new StreamWriter($"numerically_eigenfunctions_n{k + 1}.txt"))
        using (var swAna = new StreamWriter($"analytic_eigenfunctions_n{k + 1}.txt"))
        {
            for (int i = 0; i < npoints_main; i++)
            {
                double r = fixed_dr * (i + 1);
                double f = normConst * V_main[i, k];
                swNum.WriteLine($"{r} {f}");

                double fa;
                switch (k)
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

        // HERHER: TASK C: HerFra: flyt muligvis det her i en selstændig fil, når man skal lave parallelisering.
        Console.WriteLine("\n------ Fix r_max and calculate ε₀ for several different values of Δr ------");
        Console.WriteLine("------ and plot the resulting curve                                  ------\n");
        var drs = Enumerable.Range(1, 100).Select(i => i * 0.01);
        using (var drWriter = new StreamWriter("varying_dr.txt"))
        {
            drWriter.WriteLine("dr, E0");                               
            foreach (double dr in drs)
            {
                int npoints = (int)(fixed_rmax / dr) - 1;
                if (npoints < 1) continue;
                matrix H = BuildHamiltonian(npoints, dr);
                vector ew = new vector(npoints);
                matrix ev = new matrix(npoints, npoints);
                jacobi.cyclic(H, ew, ev);
                // drWriter.WriteLine($"{dr}, {ew.Min()}");
                double Emin = ew[0]; for (int t = 1; t < ew.Size; t++) if (ew[t] < Emin) Emin = ew[t];
                drWriter.WriteLine($"{dr}, {Emin}");

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
            rmWriter.WriteLine("rmax, E0");                         
            foreach (double rmax in rmaxs)
            {
                int npoints = (int)(rmax / fixed_dr) - 1;
                if (npoints < 1) continue;
                matrix H = BuildHamiltonian(npoints, fixed_dr);
                vector ew = new vector(npoints);
                matrix ev = new matrix(npoints, npoints);
                jacobi.cyclic(H, ew, ev);
                // rmWriter.WriteLine($"{rmax}, {ew.Min()}");
                double Emin = ew[0]; for (int t = 1; t < ew.Size; t++) if (ew[t] < Emin) Emin = ew[t];
                rmWriter.WriteLine($"{rmax}, {Emin}");

            }
        }
        
        Console.WriteLine($"\nΔr is fixed to {fixed_dr}.");
        Console.WriteLine("varying_rmax.txt contains the data for the calculated ε₀.");
        Console.WriteLine("varying_rmax.svg is a plot of the resulting curve.");
        // HERHER: TASK C: HerTIL: flyt muligvis det her i en selstændig fil, når man skal lave parallelisering.

        /* ---------- TASK C  (Scaling) ---------- */
        Console.WriteLine("\n------------ TASK C: Scaling and optimization ------------\n");
        
        Console.WriteLine("------ Check that the number of operations for matrix diagonalization scales as O(n³) ------"); 
        Console.WriteLine("------ do the measurements in parallel                                                ------\n");

        Console.WriteLine("The file 'number_of_operations.txt' contains timing data for Jacobi diagonalization of random matrices of size N."); // HERHER: tjek at det sker
        Console.WriteLine("The plot 'number_of_operations.svg' shows the data along with a fitted curve f(N) = a * N³.\n"); // HERHER: Tjek at det faktisk sker

        /* HERHER: OBS: jeg havde det her i en tidlige udgave:
        string timefile = "number_of_operations_time.txt";
        if (File.Exists(timefile)) {
            Console.WriteLine("\nMeasured wall-clock timing of parallel execution (from GNU time):");
            foreach (var line in File.ReadAllLines(timefile))
                Console.WriteLine(line);
        }
        */

        /* ------------ Homework points ------------*/
        int HW_POINTS_A = 1, HW_POINTS_B = 1, HW_POINTS_C = 1; // HERHER: lav om til 1 når du har lavet opgaven færdig
        HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
    
    }

    /* ------------------------------------------------------------------  helpers  ------------------------------------------------------------------ */

    static matrix BuildHamiltonian(int n, double dr)
    {
        var H = new matrix(n, n);
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
