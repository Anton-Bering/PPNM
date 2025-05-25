using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeastSquaresFit
{
    class Program
    {
        // Utility function to convert vector to log space
        static Vector LogVector(Vector v)
        {
            Vector result = new Vector(v.Size);
            for (int i = 0; i < v.Size; i++)
            {
                result.Set(i, Math.Log(v.Get(i)));
            }
            return result;
        }

        // Utility function to calculate uncertainties
        static Vector CalcUncertainties(Vector v, Vector dv)
        {
            Vector result = new Vector(v.Size);
            for (int i = 0; i < v.Size; i++)
            {
                result.Set(i, dv.Get(i) / v.Get(i));
            }
            return result;
        }

        static void Main()
        {
            // Declare vectors with sizes
            Vector t = new Vector(9);
            Vector y = new Vector(9);
            Vector dy = new Vector(9);

            // Read data from file
            string path = "datafile.csv";
            if (!File.Exists(path))
            {
                Console.Error.WriteLine("Failed to open datafile.csv");
                return;
            }

            int index = 0;
            foreach (string line in File.ReadLines(path))
            {
                string[] parts = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) continue;

                double time = double.Parse(parts[0]);
                double activity = double.Parse(parts[1]);
                double uncertainty = double.Parse(parts[2]);

                t.Set(index, time);
                y.Set(index, activity);
                dy.Set(index, uncertainty);
                index++;
            }

            // Convert to log space
            Vector lny = LogVector(y);
            Vector dlny = CalcUncertainties(y, dy);

            // Define basis functions
            List<Func<double, double>> fs = new List<Func<double, double>>
            {
                x => 1.0,
                x => -x
            };

            // Perform least squares fit
            (Vector c, Matrix cov) = LSFit.Fit(fs, t, lny, dlny);

            // Print coefficients
            c.Print("c:");

            double unc0 = Math.Sqrt(cov.Get(0, 0));
            double unc1 = Math.Sqrt(cov.Get(1, 1));
            Console.WriteLine($"Uncertainties: {unc0} {unc1}");

            double tau = Math.Log(2) / c.Get(1);
            double dtau = Math.Log(2) / (c.Get(1) * c.Get(1)) * unc1;

            Console.WriteLine($"a = {Math.Exp(c.Get(0))}");
            Console.WriteLine($"tau = {tau} ± {dtau} days, table value: 3.6313(14) days");

            cov.Print("covariance matrix:");
        }
    }
}
