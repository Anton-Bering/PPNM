using System;
using System.IO;

class Program
{
    static void WriteToCsv(string filename, Vector xValues, Vector yValues)
    {
        using (StreamWriter file = new StreamWriter(filename))
        {
            for (int i = 0; i < xValues.Size; i++)
            {
                file.WriteLine($"{xValues[i]} {yValues[i]}");
            }
        }
    }

    static void SaveEigenfunctions(Matrix eigVecs, double dr, int rmax, int number = 5)
    {
        using (StreamWriter file = new StreamWriter("eigenfunctions.csv"))
        {
            int npoints = (int)(rmax / dr) - 1;
            double Const = 1.0 / Math.Sqrt(dr);

            for (int i = 0; i < npoints; i++)
            {
                double r = dr * (i + 1);
                file.Write($"{r}");
                for (int k = 0; k < number; k++)
                {
                    double normalized = Const * eigVecs.Get(i, k);
                    file.Write($" {normalized}");
                }
                file.WriteLine();
            }
        }
    }

    static Matrix RndSymmetricMatrix(int n)
    {
        Random rand = new Random();
        Matrix A = new Matrix(n, n);
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                double val = rand.NextDouble();
                A.Set(i, j, val);
                if (i != j)
                    A.Set(j, i, val);
            }
        }
        return A;
    }

    static Matrix Hamiltonian(int rmax, double dr)
    {
        int npoints = (int)(rmax / dr) - 1;
        Vector r = new Vector(npoints);
        for (int i = 0; i < npoints; i++)
            r[i] = dr * (i + 1);

        Matrix H = new Matrix(npoints, npoints);
        for (int i = 0; i < npoints - 1; i++)
        {
            H.Set(i, i, -2);
            H.Set(i, i + 1, 1);
            H.Set(i + 1, i, 1);
        }
        H.Set(npoints - 1, npoints - 1, -2);
        H = H * (-0.5 / (dr * dr));

        for (int i = 0; i < npoints; i++)
        {
            H.Set(i, i, H.Get(i, i) - 1.0 / r[i]);
        }
        return H;
    }

    static void Main(string[] args)
    {
        double dr = 0.05;
        int rmax = 25;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dr" && i + 1 < args.Length)
                dr = double.Parse(args[++i]);
            else if (args[i] == "-rmax" && i + 1 < args.Length)
                rmax = int.Parse(args[++i]);
        }

        int m = 5;
        Matrix A = RndSymmetricMatrix(m);
        A.Print("A:");

        var (D, V) = JDWCS.Cyclic(A);
        D.Print("D:");
        V.Print("V:");

        Matrix VT = V.Transpose();
        Matrix VTAV = VT * A * V;
        VTAV.Print("V^T * A * V:");
        Console.WriteLine(Matrix.Compare(VTAV, D)
            ? "V^T * A * V is approximately equal to D."
            : "V^T * A * V is not approximately equal to D.");

        Matrix VDVT = V * D * VT;
        VDVT.Print("V * D * V^T:");
        Console.WriteLine(Matrix.Compare(VDVT, A)
            ? "V * D * V^T is approximately equal to A."
            : "V * D * V^T is not approximately equal to A.");

        Matrix VTV = VT * V;
        VTV.Print("V^T * V:");
        Console.WriteLine(Matrix.Compare(VTV, Matrix.Identity(m))
            ? "V^T * V is approximately equal to I."
            : "V^T * V is not approximately equal to I.");

        Matrix VVT = V * VT;
        VVT.Print("V * V^T:");
        Console.WriteLine(Matrix.Compare(VVT, Matrix.Identity(m))
            ? "V * V^T is approximately equal to I."
            : "V * V^T is not approximately equal to I.");

        Matrix H = Hamiltonian(rmax, dr);
        var (eigVals, eigVecs) = JDWCS.Cyclic(H);
        SaveEigenfunctions(eigVecs, dr, rmax);

        Vector deltaR = new Vector(10);
        for (int i = 0; i < 10; i++)
            deltaR[i] = 0.1 * (i + 1);
        Vector epsilon0DeltaR = new Vector(10);
        for (int i = 0; i < 10; i++)
        {
            Matrix Hi = Hamiltonian(rmax, deltaR[i]);
            var (eval, _) = JDWCS.Cyclic(Hi);
            epsilon0DeltaR[i] = eval.Get(0, 0);
        }
        WriteToCsv("delta_r_vs_epsilon0.csv", deltaR, epsilon0DeltaR);

        Vector rmaxValues = new Vector(10);
        for (int i = 0; i < 10; i++)
            rmaxValues[i] = 1 + i;
        Vector epsilon0Rmax = new Vector(10);
        for (int i = 0; i < 10; i++)
        {
            Matrix Hi = Hamiltonian((int)rmaxValues[i], dr);
            var (eval, _) = JDWCS.Cyclic(Hi);
            epsilon0Rmax[i] = eval.Get(0, 0);
        }
        WriteToCsv("rmax_vs_epsilon0.csv", rmaxValues, epsilon0Rmax);
    }
}
