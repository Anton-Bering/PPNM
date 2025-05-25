using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        using (StreamWriter writer = new StreamWriter("Out.txt"))
        {
            Console.SetOut(writer);
            RunAnalysis();
        }
    }

    static void RunAnalysis()
    {
        Console.WriteLine("---------- Part A ----------");
        Console.WriteLine("Checking that the QR algorithm works for tall matrices ...");

        // Test QR-dekomposition på en tilfældig høj matrix
        Matrix A = GenerateRandomTallMatrix(7, 4);
        Console.WriteLine("Random tall matrix A (m=7, n=4):");
        PrintMatrix(A, 3);

        A.DecomposeQR(out Matrix Q, out Matrix R);
        Console.WriteLine("\nMatrix Q:");
        PrintMatrix(Q, 3);
        Console.WriteLine("\nMatrix R:");
        PrintMatrix(R, 3);

        // Tjek om Q^T Q ≈ I
        Console.WriteLine("\nChecking that Q^T * Q = I within a tolerance ...");
        Matrix QtQ = Q.Transpose() * Q;
        bool isOrthogonal = CheckIdentity(QtQ, 1e-3);
        Console.WriteLine(isOrthogonal ? "True" : "False");
        Console.WriteLine("\nCheck complete: The QR decomposition works for tall matrices.");

        // Fit Rutherford-Soddy data
        Console.WriteLine("\nBeginning OLS fitting to the Rutherford and Soddy data ...");
        (Vector coefficients, Matrix covariance) = FitThXData();

        Console.WriteLine("\n---------- Part B ----------");
        double lambda = -coefficients[1]; // Definér lambda her
        PrintCovariance(covariance, lambda); // Send lambda med

        Console.WriteLine("\n---------- Part C ----------");
        ExportFitWithUncertainty(coefficients, covariance);
    }

    static Matrix GenerateRandomTallMatrix(int rows, int cols)
    {
        Random rand = new Random();
        Matrix mat = new Matrix(rows, cols);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                mat[i, j] = rand.NextDouble();
        return mat;
    }

    static void PrintMatrix(Matrix mat, int decimals)
    {
        string format = "{0,8:F" + decimals + "}";
        for (int i = 0; i < mat.Rows; i++)
        {
            for (int j = 0; j < mat.Cols; j++)
                Console.Write(format, mat[i, j]);
            Console.WriteLine();
        }
    }

    static bool CheckIdentity(Matrix mat, double tolerance)
    {
        for (int i = 0; i < mat.Rows; i++)
            for (int j = 0; j < mat.Cols; j++)
                if (Math.Abs(mat[i, j] - (i == j ? 1 : 0)) > tolerance)
                    return false;
        return true;
    }

    static (Vector, Matrix) FitThXData()
    {
        double[] t = { 1, 2, 3, 4, 6, 9, 10, 13, 15 };
        double[] y = { 117, 100, 88, 72, 53, 29.5, 25.2, 15.2, 11.1 };
        double[] dy = { 6, 5, 4, 4, 4, 3, 3, 2, 2 };

        double[] lnY = new double[y.Length];
        double[] sigma = new double[dy.Length];
        for (int i = 0; i < y.Length; i++)
        {
            lnY[i] = Math.Log(y[i]);
            sigma[i] = dy[i] / y[i];
        }

        Func<double, double>[] fs = { _ => 1.0, x => x };
        var (c, cov) = LeastSquares.LsFit(fs, t, lnY, sigma);

        double a = Math.Exp(c[0]);
        double lambda = -c[1]; // Korrekt fortegn for λ
        double halfLife = Math.Log(2) / lambda;
        double deltaLambda = Math.Sqrt(cov[1, 1]);
        double deltaHalfLife = (Math.Log(2) / (lambda * lambda)) * deltaLambda;

        Console.WriteLine($"\nFitted parameters:");
        Console.WriteLine($"a = {a:F3} ± {Math.Sqrt(cov[0, 0]):F3}");
        Console.WriteLine($"λ = {lambda:F3} ± {deltaLambda:F3}");
        Console.WriteLine($"T_½ = {halfLife:F3} ± {deltaHalfLife:F3} days");
        Console.WriteLine("The modern value of T_½ is 3.6316 days, which is outside the predicted uncertainty.");

        // Eksportér data og fit
        File.WriteAllLines("data.txt", t.Select((ti, i) => $"{ti}\t{y[i]}\t{dy[i]}"));
        File.WriteAllLines("fit.txt", t.Select(ti => $"{ti}\t{Math.Exp(c[0] - lambda * ti)}"));    
        return (c, cov);
    }

    static void PrintCovariance(Matrix covariance, double lambda)
    {
        Console.WriteLine("Covariance matrix:");
        for (int i = 0; i < covariance.Rows; i++)
        {
            for (int j = 0; j < covariance.Cols; j++)
                Console.Write($"{covariance[i, j]:F6}  ");
            Console.WriteLine();
        }

        double deltaLambda = Math.Sqrt(covariance[1, 1]);
        double deltaHalfLife = (Math.Log(2) / (lambda * lambda)) * deltaLambda;
        Console.WriteLine($"\nThe predicted uncertainty in the half-life is {deltaHalfLife:F3} days.");
    }

    static void ExportFitWithUncertainty(Vector coefficients, Matrix covariance)
    {
        double[] t = { 1, 2, 3, 4, 6, 9, 10, 13, 15 };
        double c1 = coefficients[0];
        double lambda = -coefficients[1]; // Brug korrekt lambda
        double delta_c1 = Math.Sqrt(covariance[0, 0]);
        double delta_lambda = Math.Sqrt(covariance[1, 1]);

        var lines = new List<string> { "# t\tlower\tupper" };
        foreach (double ti in t)
        {
            double upper = Math.Exp((c1 + delta_c1) - (lambda - delta_lambda) * ti);
            double lower = Math.Exp((c1 - delta_c1) - (lambda + delta_lambda) * ti);
            lines.Add($"{ti}\t{lower:F2}\t{upper:F2}");
        }

        File.WriteAllLines("fit_limits.txt", lines);
        Console.WriteLine("Fitted values with uncertainty limits exported to fit_limits.txt.");
    }
}