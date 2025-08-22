using System;
using System.Collections.Generic;

namespace PPNM.Minimization {

public static class Problems {

    // ------------ Test functions with known minima ------------
    
    // Quadratic function: f(x,y) = x^2 + y^2   
    // known minima: (0, 0)
    public static double Quadratic(vector v){
        double x = v[0], y = v[1];
        return x*x + y*y;
    }
    // Rosenbrock's valley function: f(x,y) = (1-x)^2 + 100*(y-x^2)^2
    // known minima: (1, 1)
    public static double Rosenbrock(vector v){
        double x = v[0], y = v[1];
        return (1 - x)*(1 - x) + 100*(y - x*x)*(y - x*x);
    }
    // Himmelblau's function: f(x,y) = (x^2 + y - 11)^2 + (x + y^2 - 7)^2
    // known minima: (3, 2), og 3 mere
    public static double Himmelblau(vector v){
        double x = v[0], y = v[1];
        return (x*x + y - 11)*(x*x + y - 11) + (x + y*y - 7)*(x + y*y - 7);
    }

    // ------------ Test functions udvidet til nD ------------
    // Note: Of course, the creation of the Quadratic function and Rosenbrock's valley function in nD, 
    // in principle makes the 2D versions unnecessary. However, I have chosen to keep them to be safe and avoid errors.
    
    // Quadratic function nD
    public static double Quadratic_nD(vector v) {
        double sum = 0;
        for (int i = 0; i < v.Size; i++) {
            sum += v[i] * v[i];
        }
        return sum;
    }
    // Rosenbrock's valley function in nD
    public static double Rosenbrock_nD(vector v) {
        double sum = 0;
        for (int i = 0; i < v.Size - 1; i++) {
            double xi   = v[i];
            double xnext = v[i + 1];
            sum += (1 - xi) * (1 - xi) + 100 * (xnext - xi * xi) * (xnext - xi * xi);
        }
        return sum;
    }

    // ------------ More complicated problem ------------

    // ------ Higgs data from CERN (from homework-opgave 9) ------ start ---
    
    // energy E[GeV]
    public static readonly double[] energy = new double[] {
        101, 103, 105, 107, 109, 111, 113, 115, 117, 119, 
        121, 123, 125, 127, 129, 131, 133, 135, 137, 139, 
        141, 143, 145, 147, 149, 151, 153, 155, 157, 159
    };
    // signal σ(E) [certain units]
    public static readonly double[] signal = new double[] {
        -0.25, -0.30, -0.15, -1.71, 0.81, 0.65, -0.91, 0.91, 0.96, -2.52, 
        -1.01, 2.01, 4.83, 4.58, 1.26, 1.01, -1.26, 0.45, 0.15, -0.91, 
        -0.81, -1.41, 1.36, 0.50, -0.45, 1.61, -2.21, -1.86, 1.76, -0.50
    };
    // experimental uncertainty Δσ [same units]
    public static readonly double[] error = new double[] {
        2.0, 2.0, 1.9, 1.9, 1.9, 1.9, 1.9, 1.9, 1.6, 1.6, 
        1.6, 1.6, 1.6, 1.6, 1.3, 1.3, 1.3, 1.3, 1.3, 1.3, 
        1.1, 1.1, 1.1, 1.1, 1.1, 1.1, 1.1, 0.9, 0.9, 0.9
    };

    public static double HiggsDeviation(vector p)
    {
        double m = p[0];    // mass
        double Γ = p[1];    // widths of the resonance
        double A = p[2];    // scale-factor

        double deviation_function = 0;
        for (int i = 0; i < energy.Length; i++)
        {
            double E_i = energy[i];
            double σ_i = signal[i];
            double Δσ_i = error[i];

            // Breit-Wigner function
            double F_i = A / ((E_i - m) * (E_i - m) + Γ * Γ / 4.0);

            // the deviation function
            deviation_function += Math.Pow((F_i - σ_i) / Δσ_i, 2);
        }
        return deviation_function;
    }
    // ------ Higgs data from CERN (from homework-opgave 9) ------ end ---

    // ------ Artificial Neural Network (homework-opgave 10) ------ start ---
    public class Ann 
    {
        private readonly int n; // Antal skjulte neuroner
        private readonly Func<double, double> f = x => x * Math.Exp(-x * x); // Aktivationsfunktion
        private readonly vector p; // Netværksparametre

        public Ann(vector parameters)
        {
            // Hver neuron har 3 parametre (a, b, w)
            if (parameters.Size % 3 != 0)
                throw new ArgumentException("Parameter vector Size must be a multiple of 3.");
            
            this.n = parameters.Size / 3;
            this.p = parameters;
        }

        // Beregner netværkets output F_p(x)
        public double Response(double x)
        {
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                // Hent parametrene for neuron 'i'
                double a_i = p[i * 3 + 0]; // Center
                double b_i = p[i * 3 + 1]; // Skala/bredde
                double w_i = p[i * 3 + 2]; // Vægt

                sum += f((x - a_i) / b_i) * w_i;
            }
            return sum;
        }
    }


}
}