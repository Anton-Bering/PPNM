using System;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        double rmax = 30.0; // Øget rmax
        double dr = 0.5;    // Finere gitter

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-rmax") rmax = double.Parse(args[i + 1]);
            else if (args[i] == "-dr") dr = double.Parse(args[i + 1]);
        }

        int npoints = (int)(rmax / dr) - 1;
        Matrix H = new Matrix(npoints, npoints);
        double factor = -0.5 / (dr * dr);

        for (int i = 0; i < npoints; i++)
        {
            H[i, i] = -2 * factor;
            if (i < npoints - 1)
            {
                H[i, i + 1] = factor;
                H[i + 1, i] = factor;
            }
            H[i, i] += -1.0 / (dr * (i + 1)); // -1/r
        }

        var (w, _) = Jacobi.Cyclic(H);

        // Konverter til array og sorter
        double[] eigenvalues = new double[w.Length];
        for (int i = 0; i < w.Length; i++)
            eigenvalues[i] = w[i];
        Array.Sort(eigenvalues);

        Console.WriteLine("Beregnet egenværdier:");
        for (int i = 0; i < 5; i++)
            Console.WriteLine($"ε_{i} = {eigenvalues[i]:F6}");

        Console.WriteLine("\nEksakte egenværdier (Hydrogen):");
        for (int n = 1; n <= 5; n++)
            Console.WriteLine($"n={n}: {-1.0 / (2 * n * n):F6}");
    }
}