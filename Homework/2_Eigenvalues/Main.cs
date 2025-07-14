
using System;
using System.IO;
using System.Linq;

public class Program {
    public static void Main(string[] args) {
        using (StreamWriter writer = new StreamWriter("Out.txt")) {
            writer.WriteLine("------------ TASK A: Jacobi diagonalization with cyclic sweeps ------------\n");

            writer.WriteLine("------ Prove that the implementation works as intended ------\n");

            int sizeA = 5;
            writer.WriteLine($"--- Generate a random symmetric matrix A ({sizeA}x{sizeA}) ---\n");
            writer.WriteLine("Matrix A:");
            matrix A = new matrix(sizeA, sizeA);
            Random rnd = new Random(1);
            for (int i = 0; i < sizeA; i++)
            for (int j = i; j < sizeA; j++) {
                double val = Math.Round(rnd.NextDouble(), 3);
                A[i, j] = val;
                A[j, i] = val;
            }
            printMatrix(writer, A);

            writer.WriteLine("\n------ After applying your routine to perform the eigenvalue decomposition, A = VDVᵀ ------\n");

            matrix A_copy = A.copy();
            vector w = new vector(sizeA);
            matrix V = new matrix(sizeA, sizeA);
            jacobi.cyclic(A, w, V);

            writer.WriteLine("\nVector w (vector of the eigenvalues):");
            writer.WriteLine(string.Join(" ", formatVector(w)));
            writer.WriteLine("\nMatrix V (matrix of the corresponding eigenvector-columns):");
            printMatrix(writer, V);
            writer.WriteLine("\nMatrix Vᵀ:");
            matrix V_T = transpose(V);
            printMatrix(writer, V_T);
            writer.WriteLine("\nMatrix D (diagonal matrix of eigenvalues):");
            matrix D = diagonalMatrix(w);
            printMatrix(writer, D);


            writer.WriteLine("\n--- Check that VᵀAV≈D ---\n");

            matrix VtAV = multiply(transpose(V), multiply(A_copy, V));
            writer.WriteLine("Matrix VᵀAV:");
            printMatrix(writer, VtAV);
            writer.WriteLine("\nTEST: Is VᵀAV≈D (within a tolerance of 1e-6)?");
            writer.WriteLine(isDiagonalClose(VtAV, w)
                ? "RESULT: Yes, VᵀAV≈D"
                : "RESULT: No, VᵀAV≠D");

            writer.WriteLine("\nEigenvalue vector w from Jacobi:   " + string.Join(" ", formatVector(w)));

            writer.WriteLine("\n--- Check that VDVᵀ≈A ---\n");
            // matrix D = diagonalMatrix(w);
            matrix VDVt = multiply(multiply(V, D), transpose(V));
            writer.WriteLine("Matrix VDVᵀ:");
            printMatrix(writer, VDVt);
            writer.WriteLine("\nTEST: Is VDVᵀ≈A (within a tolerance of 1e-6)?");
            writer.WriteLine(areMatricesClose(A_copy, VDVt)
                ? "RESULT: Yes, VDVᵀ≈A"
                : "RESULT: No, VDVᵀ≠A");

            writer.WriteLine("\n--- Check that VᵀV≈I ---\n");
            matrix VtV = multiply(transpose(V), V);
            writer.WriteLine("Matrix VᵀV:");
            printMatrix(writer, VtV);
            writer.WriteLine("\nTEST: Is VᵀV≈I (within a tolerance of 1e-6)?");
            writer.WriteLine(areMatricesClose(VtV, matrix.id(sizeA))
                ? "RESULT: Yes, VᵀV= I"
                : "RESULT: No, VᵀV≠I");

            writer.WriteLine("\n--- Check that VVᵀ≈I ---\n");
            matrix VVt = multiply(V, transpose(V));
            writer.WriteLine("Matrix VVᵀ:");
            printMatrix(writer, VVt);
            writer.WriteLine("\nTEST: Is VVᵀ≈I (within a tolerance of 1e-6)?");
            writer.WriteLine(areMatricesClose(VVt, matrix.id(sizeA))
                ? "RESULT: Yes, VVᵀ≈I"
                : "RESULT: No, VVᵀ≠I");

            writer.WriteLine("\n------------ TASK B: Hydrogen atom, s-wave radial Schrödinger equation on a grid ------------\n");
            
            writer.WriteLine("------ Calculate numerically the lowest egenvalues and eigenfunctions ------");
            writer.WriteLine("------ of the s-wave states in the hydrogen atom                      ------");
            writer.WriteLine("------ and compare them with the exact results                        ------\n");
            
            double[] drs = Enumerable.Range(1, 100).Select(i => i * 0.01).ToArray();
            double[] rmaxs = Enumerable.Range(8, 33).Select(i => i * 0.5).ToArray();
            double fixed_rmax = 10;
            double fixed_dr = 0.3;

            int npoints_main = (int)(fixed_rmax / fixed_dr) - 1;
            matrix H_main = buildHamiltonian(npoints_main, fixed_dr);
            vector w_main = new vector(npoints_main);
            matrix V_main = new matrix(npoints_main, npoints_main);
            jacobi.cyclic(H_main, w_main, V_main);

            writer.WriteLine("\nNumerically calculated lowest eigenvalues:");
            for (int i = 0; i < 5; i++) {
                writer.WriteLine($"ε_{i} = {w_main[i]:F6}");
            }

            double normConst = 1.0 / Math.Sqrt(fixed_dr);
            for (int k = 0; k < 3; k++) {
                using (StreamWriter sw = new StreamWriter($"numerically_eigenfunctions_n{(k + 1)}.txt"))
                using (StreamWriter sa = new StreamWriter($"analytic_eigenfunctions_n{(k + 1)}.txt")) {
                    for (int i = 0; i < npoints_main; i++) {
                        double r = fixed_dr * (i + 1);
                        double f = normConst * V_main[i, k];
                        sw.WriteLine($"{r} {f}");

                        double fa = 0;
                        if (k == 0)
                            fa = 2 * r * Math.Exp(-r);
                        else if (k == 1)
                            fa = -((1.0 / Math.Sqrt(2)) * (1 - r / 2) * r * Math.Exp(-r / 2));
                        else if (k == 2)
                            fa = (2.0 / (81 * Math.Sqrt(3))) * (27 - 18 * r + 2 * r * r) * r * Math.Exp(-r / 3);

                        sa.WriteLine($"{r} {fa}");
                    }
                }
            }

            using (StreamWriter drWriter = new StreamWriter("varying_dr.txt")) {
                drWriter.WriteLine("dr, E0");
                foreach (double dr in drs) {
                    int npoints = (int)(fixed_rmax / dr) - 1;
                    if (npoints < 1) continue;
                    matrix H = buildHamiltonian(npoints, dr);
                    vector ew = new vector(npoints);
                    matrix evec = new matrix(npoints, npoints);
                    jacobi.cyclic(H, ew, evec);
                    double E0 = findMin(ew);
                    drWriter.WriteLine($"{dr}, {E0}");
                }
            }



            using (StreamWriter rmaxWriter = new StreamWriter("varying_rmax.txt")) {
                rmaxWriter.WriteLine("rmax, E0");
                foreach (double rmax in rmaxs) {
                    int npoints = (int)(rmax / fixed_dr) - 1;
                    if (npoints < 1) continue;
                    matrix H = buildHamiltonian(npoints, fixed_dr);
                    vector ew = new vector(npoints);
                    matrix evec = new matrix(npoints, npoints);
                    jacobi.cyclic(H, ew, evec);
                    double E0 = findMin(ew);
                    rmaxWriter.WriteLine($"{rmax}, {E0}");
                }
            }

            writer.WriteLine("\nnumerically_eigenfunctions_ni.txt (for i = 1, 2, 3) contains the data for the numerically calculated eigenfunctions with n = 1, 2, 3, respectively.");
            writer.WriteLine("eigenfunctions.svg is a plot of the numerically calculated eigenfunctions.");

            writer.WriteLine("\nanalytic_eigenfunctions_ni.txt (for i = 1, 2, 3) contains the data for the (exact) analytically computed eigenfunctions with n = 1, 2, 3, respectively.");
            writer.WriteLine("eigenfunctions.svg also contains the exact results for comparison.");

            writer.WriteLine("\n------ Fix r_max and calculate ε₀ for several different values of Δr ------");
            writer.WriteLine("------ and plot the resulting curve                                  ------\n");

            writer.WriteLine($"r_max is fixed to {fixed_rmax}.");
            writer.WriteLine("varying_dr.txt contains the data for the calculated ε₀.");
            writer.WriteLine("varying_dr.svg is a plot of the resulting curve.");

            writer.WriteLine("\n------ Fix Δr and calculate ε₀ for several different values of r_max ------");
            writer.WriteLine("------ and plot the resulting curve                                  ------\n");

            writer.WriteLine($"\nΔr is fixed to {fixed_dr}.");
            writer.WriteLine("varying_rmax.txt contains the data for the calculated ε₀.");
            writer.WriteLine("varying_rmax.svg is a plot of the resulting curve.");


            writer.WriteLine("\n------------ TASK C: Scaling and optimization ------------\n");
            writer.WriteLine("------ Check that the number of operations for matrix diagonalization scales as O(n³) ------"); 
            writer.WriteLine("------ do the measurements in parallel                                                ------\n");

            writer.WriteLine("The file 'number_of_operations.txt' contains timing data for Jacobi diagonalization of random matrices of size N.");
            writer.WriteLine("The plot 'number_of_operations.svg' shows the data along with a fitted curve f(N) = a * N³.\n");

            string timefile = "number_of_operations_time.txt";
            if (File.Exists(timefile)) {
                writer.WriteLine("\nMeasured wall-clock timing of parallel execution (from GNU time):");
                foreach (var line in File.ReadAllLines(timefile))
                    writer.WriteLine(line);
            }

            // ---------- Points from task A, B, and C ----------
            int HW_POINTS_A = 1;
            int HW_POINTS_B = 1;
            int HW_POINTS_C = 1;
            HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C); 

        }

    }

    static matrix buildHamiltonian(int n, double dr) {
        matrix H = new matrix(n, n);
        double invdr2 = 1.0 / (dr * dr);
        for (int i = 0; i < n - 1; i++) {
            H[i, i] += 1.0 * invdr2;
            H[i, i + 1] += -0.5 * invdr2;
            H[i + 1, i] += -0.5 * invdr2;
        }
        H[n - 1, n - 1] += 1.0 * invdr2;
        for (int i = 0; i < n; i++) {
            double ri = dr * (i + 1);
            H[i, i] += -1.0 / ri;
        }
        return H;
    }

    static double findMin(vector v) {
        double min = v[0];
        for (int i = 1; i < v.size; i++)
            if (v[i] < min) min = v[i];
        return min;
    }

    static void printMatrix(StreamWriter writer, matrix M) {
        for (int i = 0; i < M.size1; i++) {
            for (int j = 0; j < M.size2; j++) {
                writer.Write($"{M[i,j],10:G5} ");
            }
            writer.WriteLine();
        }
    }

    static string[] formatVector(vector v) {
        string[] formatted = new string[v.size];
        for (int i = 0; i < v.size; i++)
            formatted[i] = v[i].ToString("G5").PadLeft(8);
        return formatted;
    }

    static matrix multiply(matrix A, matrix B) {
        int n = A.size1, m = B.size2, p = A.size2;
        matrix C = new matrix(n, m);
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
        for (int k = 0; k < p; k++)
            C[i, j] += A[i, k] * B[k, j];
        return C;
    }

    static matrix transpose(matrix A) {
        matrix T = new matrix(A.size2, A.size1);
        for (int i = 0; i < A.size1; i++)
        for (int j = 0; j < A.size2; j++)
            T[j, i] = A[i, j];
        return T;
    }

    static matrix diagonalMatrix(vector v) {
        matrix D = new matrix(v.size, v.size);
        for (int i = 0; i < v.size; i++) D[i, i] = v[i];
        return D;
    }

    static bool isDiagonalClose(matrix A, vector w, double tol = 1e-6) {
        for (int i = 0; i < w.size; i++)
            if (Math.Abs(A[i, i] - w[i]) > tol) return false;
        return true;
    }

    static bool areMatricesClose(matrix A, matrix B, double tol = 1e-6) {
        for (int i = 0; i < A.size1; i++)
        for (int j = 0; j < A.size2; j++)
            if (Math.Abs(A[i, j] - B[i, j]) > tol) return false;
        return true;
    }
}

