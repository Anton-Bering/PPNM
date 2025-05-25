using System;
public class Program {
    public static void Main(string[] args) {
        // Default parameters (in case none provided)
        double rmax = 10.0;
        double dr = 0.1;
        // Parse command-line arguments -rmax and -dr
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == "-rmax" && i+1 < args.Length) {
                rmax = double.Parse(args[++i]);
            } else if (args[i] == "-dr" && i+1 < args.Length) {
                dr = double.Parse(args[++i]);
            }
        }
        int npoints = (int)(rmax / dr) - 1;
        if (npoints < 1) {
            Console.Error.WriteLine("Grid size is too small. Choose rmax and dr such that (rmax/dr) - 1 >= 1.");
            return;
        }
        // Prepare radial grid points
        vector r = new vector(npoints);
        for (int i = 0; i < npoints; i++) {
            r[i] = dr * (i + 1);  // r_i = (i+1)*dr
        }
        // Build Hamiltonian matrix H (size npoints x npoints)
        matrix H = new matrix(npoints, npoints);
        double inv_dr2 = 1.0 / (dr * dr);
        // Fill Kinetic energy part K (tridiagonal)
        for (int i = 0; i < npoints - 1; i++) {
            H[i, i]     +=  1.0 * inv_dr2;       // diagonal: 1/Δr^2
            H[i, i + 1] += -0.5 * inv_dr2;       // super-diagonal: -1/(2Δr^2)
            H[i + 1, i] += -0.5 * inv_dr2;       // sub-diagonal (symmetric)
        }
        // Last diagonal element of K:
        H[npoints - 1, npoints - 1] += 1.0 * inv_dr2;
        // Add potential energy W (diagonal: -1/r_i)
        for (int i = 0; i < npoints; i++) {
            H[i, i] += -1.0 / r[i];
        }

        // Diagonalize H using Jacobi cyclic method
        vector w = new vector(npoints);
        matrix V = new matrix(npoints, npoints);
        jacobi.cyclic(H, w, V);
        // Sort eigenvalues and eigenvectors by increasing eigenvalue
        int n = npoints;
        double[] evals = new double[n];
        int[] idx = new int[n];
        for (int i = 0; i < n; i++) { evals[i] = w[i]; idx[i] = i; }
        Array.Sort(evals, idx);  // Sort eigenvalues (and idx) in ascending order
        vector w_sorted = new vector(n);
        matrix V_sorted = new matrix(n, n);
        for (int newi = 0; newi < n; newi++) {
            int oldi = idx[newi];
            w_sorted[newi] = evals[newi];
            for (int row = 0; row < n; row++) {
                V_sorted[row, newi] = V[row, oldi];
            }
        }
        w = w_sorted;
        V = V_sorted;

        // Output eigenvalues (lowest first) in Hartree units
        Console.WriteLine("Eigenvalues (ε, in Hartree):");
        for (int i = 0; i < n; i++) {
            // Only print first several or all? Here we print all for completeness
            Console.WriteLine($"E{i} = {w[i]:F6}");
        }

        // Identify bound-state eigenvalues (negative energies)
        int boundCount = 0;
        while (boundCount < n && w[boundCount] < 0) boundCount++;
        if (boundCount == 0) boundCount = 1; // ensure at least one (ground state)
        if (boundCount > 3) boundCount = 3;  // we'll output up to 3 lowest eigenfunctions

        // Output a few lowest eigenfunctions (normalized radial functions)
        Console.WriteLine("\nRadius r, and lowest eigenfunctions f^(k)(r):");
        // Header line for CSV output
        Console.Write("r");
        for (int k = 0; k < boundCount; k++) {
            Console.Write($", f{k}(r)");
        }
        Console.WriteLine();
        // Each subsequent line: r, f0, f1, f2, ...
        double normConst = 1.0 / Math.Sqrt(dr);
        for (int i = 0; i < npoints; i++) {
            Console.Write($"{r[i]:F3}");
            for (int k = 0; k < boundCount; k++) {
                // f^(k)(r_i) = (1/sqrt(dr)) * V_{ik} 
                double f_ik = normConst * V[i, k];
                Console.Write($", {f_ik:F6}");
            }
            Console.WriteLine();
        }

        // --- Part C: Convergence analysis ---
        Console.WriteLine("\nConvergence study:");
        // 1. Fix r_max and vary dr
        double fix_rmax = rmax;
        Console.WriteLine($"Fixed r_max = {fix_rmax}, varying dr:");
        Console.WriteLine("dr, ground_state_energy");
        double[] drTests = {0.5, 0.2, 0.1, 0.05, 0.02, dr};
        Array.Sort(drTests);
        Array.Reverse(drTests); // sort in descending order (largest dr first)
        double original_rmax = rmax;
        foreach(double dr_val in drTests) {
            if (dr_val <= 0 || dr_val > fix_rmax) continue;
            int Npts = (int)(fix_rmax / dr_val) - 1;
            if (Npts < 1) continue;
            matrix Htemp = new matrix(Npts, Npts);
            // build H for this dr_val
            double invdr2 = 1.0/(dr_val*dr_val);
            for(int i=0;i<Npts-1;i++){
                Htemp[i,i]     +=  1.0*invdr2;
                Htemp[i,i+1]   += -0.5*invdr2;
                Htemp[i+1,i]   += -0.5*invdr2;
            }
            Htemp[Npts-1, Npts-1] += 1.0*invdr2;
            for(int i=0;i<Npts;i++){
                double ri = dr_val*(i+1);
                Htemp[i,i] += -1.0/ri;
            }
            vector wtemp = new vector(Npts);
            matrix Vtemp = new matrix(Npts, Npts);
            jacobi.cyclic(Htemp, wtemp, Vtemp);
            // Find lowest eigenvalue
            double E0 = wtemp[0];
            for(int j=1;j<Npts;j++){ if(wtemp[j] < E0) E0 = wtemp[j]; }
            Console.WriteLine($"{dr_val}, {E0:F6}");
        }
        // 2. Fix dr and vary r_max
        double fix_dr = dr;
        Console.WriteLine($"\nFixed dr = {fix_dr}, varying r_max:");
        Console.WriteLine("r_max, ground_state_energy");
        double[] rmaxTests = {5.0, 10.0, 15.0, rmax};
        Array.Sort(rmaxTests);
        foreach(double rmax_val in rmaxTests) {
            if(rmax_val <= fix_dr) continue;
            int Npts = (int)(rmax_val / fix_dr) - 1;
            if (Npts < 1) continue;
            matrix Htemp = new matrix(Npts, Npts);
            double invdr2 = 1.0/(fix_dr*fix_dr);
            for(int i=0;i<Npts-1;i++){
                Htemp[i,i]   += 1.0*invdr2;
                Htemp[i,i+1] += -0.5*invdr2;
                Htemp[i+1,i] += -0.5*invdr2;
            }
            Htemp[Npts-1,Npts-1] += 1.0*invdr2;
            for(int i=0;i<Npts;i++){
                double ri = fix_dr*(i+1);
                Htemp[i,i] += -1.0/ri;
            }
            vector wtemp = new vector(Npts);
            matrix Vtemp = new matrix(Npts, Npts);
            jacobi.cyclic(Htemp, wtemp, Vtemp);
            double E0 = wtemp[0];
            for(int j=1;j<Npts;j++){ if(wtemp[j] < E0) E0 = wtemp[j]; }
            Console.WriteLine($"{rmax_val}, {E0:F6}");
        }
    }
}
