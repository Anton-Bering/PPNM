// Main.cs – template //

using System;
using System.IO;
using System.Text;
using static System.Math;

namespace HomeworkTemplate
{
    class Program
    {
        static void Main()
        {
            // ────────────────────────── TASK A ──────────────────────────
            using (var outFile = new StreamWriter("Out.txt"))        
            {
                outFile.WriteLine("------------ TASK A: Function Sampling and Plotting ------------\n");

                
                outFile.WriteLine("------ task A.1: Data Generation ------\n");
                outFile.WriteLine("--- task A.1.1: Generate data points for sin(x) ---\n");
                GenerateData("sin_data.txt", Sin);                   
                outFile.WriteLine("Data saved in sin_data.txt");

                outFile.WriteLine("\n--- task A.1.2: Generate data points for cos(x) ---\n");
                GenerateData("cos_data.txt", Cos);
                outFile.WriteLine("Data saved in cos_data.txt\n");

                
                outFile.WriteLine("------ task A.2: Visualization using Gnuplot ------\n");
                outFile.WriteLine("--- task A.2.1: Plot sin(x) from generated data ---\n");
                outFile.WriteLine("sin_plot.svg contains the plot.");
                outFile.WriteLine("\n--- task A.2.2: Plot cos(x) from generated data ---\n");
                outFile.WriteLine("cos_plot.svg contains the plot.");

                // ────────────────────────── TASK B ──────────────────────────
                outFile.WriteLine("\n------------ TASK B: Vector Operations ------------\n");
                outFile.WriteLine("------ task B.1: Working with Vectors ------\n");
                outFile.WriteLine("--- task B.1.1: Create and initialize a vector ---\n");

                vector v = new vector(3);           
                v[0] = 1; v[1] = 2; v[2] = 3;
                outFile.WriteLine("Created vector:\n" + FormatVector(v));

                outFile.WriteLine("--- task B.1.2: Compute the norm of a vector ---\n");
                outFile.WriteLine($"Norm of vector: {v.norm():F3}\n");

                outFile.WriteLine("\n------ task B.2: Empty test ------\n");
                outFile.WriteLine("--- task B.2.1: Empty test ---\n");
                outFile.WriteLine("--- task B.2.2: Empty test ---\n");

                // ────────────────────────── TASK C ──────────────────────────
                outFile.WriteLine("------------ TASK C: Solving Linear Systems with Matrices ------------\n");
                outFile.WriteLine("------ task C.1: Solving a Linear System ------\n");
                outFile.WriteLine("--- task C.1.1: Create and print a 2×2 matrix ---\n");

                matrix A = new matrix(2, 2);
                A[0, 0] = 3; A[0, 1] = 2;
                A[1, 0] = 1; A[1, 1] = 2;
                outFile.WriteLine("Matrix A:\n" + FormatMatrix(A));

                outFile.WriteLine("--- task C.1.2: Solve the system A·x = b using Gaussian elimination ---\n");
                vector b = new vector(2); b[0] = 5; b[1] = 5;
                vector x_vector = A.Solve(b);
                outFile.WriteLine("Solution to A*x = b:\n" + FormatVector(x_vector));

                outFile.WriteLine("------ task C.2: Empty test ------\n");
                outFile.WriteLine("--- task C.2.1: Empty test ---\n");
                outFile.WriteLine("--- task C.2.2: Empty test ---\n");

                
                int HW_POINTS_A = 1;    // If task A is completed HW_POINTS_A=1, else HW_POINTS_A=0
                int HW_POINTS_B = 1;    // If task B is completed HW_POINTS_B=1, else HW_POINTS_B=0
                int HW_POINTS_C = 1;    // If task C is completed HW_POINTS_C=1, else HW_POINTS_C=0
                HW_points.HW_POINTS(HW_POINTS_A, HW_POINTS_B, HW_POINTS_C);
            }   
        }

        static void GenerateData(string filename, Func<double, double> f, double step = 0.1)
        {
            using (var file = new StreamWriter(filename))
            {
                for (double t = 0.0; t <= 2 * PI + 1e-12; t += step)
                    file.WriteLine($"{t} {f(t)}");
            }
        }

        static string FormatVector(vector v, int decimals = 3)
        {
            var sb  = new StringBuilder(v.size * (decimals + 8));
            string fmt = "{0:F" + decimals + "}";
            for (int i = 0; i < v.size; ++i)
                sb.AppendLine(string.Format(fmt, v[i]));
            return sb.ToString();
        }

        static string FormatMatrix(matrix M, int decimals = 3)
        {
            var sb  = new StringBuilder(M.size1 * M.size2 * (decimals + 12));
            string fmt = "{0,10:F" + decimals + "}";
            for (int i = 0; i < M.size1; ++i)
            {
                for (int j = 0; j < M.size2; ++j)
                    sb.Append(string.Format(fmt + " ", M[i, j]));
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
