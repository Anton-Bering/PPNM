using System;
using System.IO;

public class Program {
    public static void Main(string[] args) {
        using (StreamWriter writer = new StreamWriter("Out.txt")) {
            writer.WriteLine("--- --- --- PART A --- --- ---\n");

            writer.WriteLine("-- -- -- Prove that the implementation works as intended -- -- --\n");

            int sizeA = 5;
            Console.WriteLine($"Generate a random symmetric matrix A ({sizeA}x{sizeA}):");
            matrix A = new matrix(sizeA, sizeA);
            Random rnd = new Random(1);
            for (int i = 0; i < sizeA; i++)
            for (int j = i; j < sizeA; j++) {
                double val = Math.Round(rnd.NextDouble(), 3);
                A[i, j] = val;
                A[j, i] = val;
            }
            printMatrix(writer, A);

            writer.WriteLine("\nApply my routine to perform the eigenvalue-decomposition, A=VDVᵀ (where V is the orthogonal matrix of eigenvectors and D is the diagonal matrix with the corresponding eigenvalues).\n");
            writer.WriteLine("Check that VᵀAV=D:\n");
            matrix A_copy = A.copy();
            vector w = new vector(sizeA);
            matrix V = new matrix(sizeA, sizeA);
            jacobi.cyclic(A, w, V);

            matrix VtAV = multiply(transpose(V), multiply(A_copy, V));
            writer.WriteLine("\nVᵀAV yields:");
            printMatrix(writer, VtAV);
            writer.WriteLine("\n Is VᵀAV=D?");
            writer.WriteLine(isDiagonalClose(VtAV, w));

            writer.WriteLine("\nEigenvalue vector w from Jacobi:   " + string.Join(" ", formatVector(w)));

            writer.WriteLine("\nCheck that VDVᵀ=A:\n");
            matrix D = diagonalMatrix(w);
            matrix VDVt = multiply(multiply(V, D), transpose(V));
            writer.WriteLine("\n VDVᵀ yields:");
            printMatrix(writer, VDVt);
            writer.WriteLine("\nIs VDVᵀ=A ?");
            writer.WriteLine(areMatricesClose(A_copy, VDVt));

            writer.WriteLine("\nCheck that VᵀV=I :");
            matrix VtV = multiply(transpose(V), V);
            writer.WriteLine("\n VᵀV yields:");
            printMatrix(writer, VtV);
            writer.WriteLine("\nIs VᵀV=I ?");
            writer.WriteLine(areMatricesClose(VtV, matrix.id(sizeA)));

            writer.WriteLine("\nCheck that VVᵀ=I :\n");
            matrix VVt = multiply(V, transpose(V));
            writer.WriteLine("\n VVᵀ yields:");
            printMatrix(writer, VVt);
            writer.WriteLine("\nIs VVᵀ=I ?");
            writer.WriteLine(areMatricesClose(VVt, matrix.id(sizeA)));

            writer.WriteLine("\n--- --- --- PART B --- --- ---\n");
            double[] drs = {0.5, 0.3, 0.2, 0.1, 0.05, 0.02};
            double[] rmaxs = {5.0, 10.0, 15.0};
            double fixed_rmax = 10;
            double fixed_dr = 0.3;

            int npoints_main = (int)(fixed_rmax / fixed_dr) - 1;
            matrix H_main = buildHamiltonian(npoints_main, fixed_dr);
            vector w_main = new vector(npoints_main);
            matrix V_main = new matrix(npoints_main, npoints_main);
            jacobi.cyclic(H_main, w_main, V_main);
            writer.WriteLine("-- -- -- Calculate numerically the lowest egenvalues -- -- --\n");
            writer.WriteLine("\nThe lowest eigenvalues of the hydrogen atom is:");
            for (int i = 0; i < 5; i++) {
                writer.WriteLine($"ε_{i} = {w_main[i]:F6}");
            }

            writer.WriteLine("-- -- -- Calculate eigenfunctions of the s-wave states in the hydrogen atom -- -- --\n");

            double normConst = 1.0 / Math.Sqrt(fixed_dr);
            for (int k = 0; k < 3; k++) {
                using (StreamWriter sw = new StreamWriter($"radial_n{(k + 1)}.txt")) {
                    for (int i = 0; i < npoints_main; i++) {
                        double r = fixed_dr * (i + 1);
                        double f = normConst * V_main[i, k];
                        sw.WriteLine($"{r} {f}");
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
            writer.WriteLine("The data for the data for the eigenfunctions are in radial_n1.txt, radial_n2.txt, radial_n3.txt.");
            writer.WriteLine("A plot of the eigenfunctions are in radial_wavefunctions.png");


            writer.WriteLine("Convergence data written to varying_dr.txt and varying_rmax.txt.");
            writer.WriteLine("Wavefunctions written to radial_n1.txt, radial_n2.txt, radial_n3.txt.");

            writer.WriteLine("\n--- --- --- PART C --- --- ---\n");
            writer.WriteLine("[Optional optimizations go here if implemented.]");
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
