using System;

public static class Functions {
    // Rosenbrock's valley function: f(x,y) = (1 - x)^2 + 100*(y - x^2)^2
    public static double Rosenbrock(double[] x) {
        double a = x[0];
        double b = x[1];
        double term1 = 1 - a;
        double term2 = b - a * a;
        return term1 * term1 + 100 * term2 * term2;
    }

    // Himmelblau's function: f(x,y) = (x^2 + y - 11)^2 + (x + y^2 - 7)^2
    public static double Himmelblau(double[] x) {
        double a = x[0];
        double b = x[1];
        double term1 = a * a + b - 11;
        double term2 = a + b * b - 7;
        return term1 * term1 + term2 * term2;
    }

    // Breit-Wigner resonance function: F(E | m, Γ, A) = A / [ (E - m)^2 + (Γ^2)/4 ]
    public static double BreitWigner(double E, double m, double gamma, double A) {
        return A / ( (E - m) * (E - m) + (gamma * gamma) / 4.0 );
    }

    // Data arrays for Higgs fit (to be filled from input in Main)
    public static double[] EData;
    public static double[] SigmaData;
    public static double[] ErrorData;

    // Deviation function for Higgs data fit:
    // D(m, Γ, A) = Σ_i [ (F(E_i | m, Γ, A) - σ_i) / Δσ_i ]^2
    // Parameter vector p = [m, gamma, A]
    public static double Deviation(double[] p) {
        double m = p[0];
        double gamma = p[1];
        double A = p[2];
        double D = 0.0;
        for (int i = 0; i < EData.Length; i++) {
            double Fi = BreitWigner(EData[i], m, gamma, A);
            double diff = (Fi - SigmaData[i]) / ErrorData[i];
            D += diff * diff;
        }
        return D;
    }
}
