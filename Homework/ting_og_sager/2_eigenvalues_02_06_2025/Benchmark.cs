using System;
using System.Linq;

public class Benchmark {
    public static void Main(string[] args) {
        double dr = 0.1;
        double rmax = 10.0;
        bool varyRmax = false;

        // Parse optional command-line arguments
        for (int i = 0; i < args.Length - 1; i++) {
            if (args[i] == "-dr") dr = double.Parse(args[i + 1]);
            if (args[i] == "-rmax") rmax = double.Parse(args[i + 1]);
        }

        // Check for flag to vary rmax
        if (args.Contains("-varyrmax")) varyRmax = true;

        int npoints = (int)(rmax / dr) - 1;
        if (npoints < 2) {
            Console.WriteLine("{0} NaN", varyRmax ? rmax : dr);
            return;
        }

        matrix H = buildHamiltonian(npoints, dr);
        vector ew = new vector(npoints);
        matrix evec = new matrix(npoints, npoints);
        jacobi.cyclic(H, ew, evec);

        double E0 = findMin(ew);
        Console.WriteLine("{0} {1}", varyRmax ? rmax : dr, E0);
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
}
