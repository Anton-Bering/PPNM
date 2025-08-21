// I 'public static class Problems'

// ------------ Find ISCO for et Schwarzschild sort hul ------------
// Minimerer (V_eff''(r))² for at finde r, hvor V_eff''(r) = 0.
public static double IscoSchwarzschild(vector v)
{
    double r = v[0]; // radius
    const double M = 1.0; // mass
    if (r <= 3 * M) return double.PositiveInfinity; // Ugyldigt område (returner en høj værdi)

    double r2 = r * r; // r²
    double r3 = r2 * r; // r³
    double r4 = r3 * r; // r⁴
    double r5 = r4 * r; // r⁵

    // Angular Momentum Squared
    double L2 = (M * r2) / (r - 3*M); // = L²
    
    // V_eff''(r) = -4M/r³ + 6L²/r⁴ - 24ML²/r⁵
    double V_eff_2prime = -4*M/r3 + 6*L2/r4 - 24*M*L2/r5;
    
    // Vi vil finde r, hvor V_eff_double_prime er nul.
    // Det gør vi ved at minimere kvadratet på den.
    return V_eff_2prime * V_eff_2prime;

    // ------------ Find ISCO for et roterende (Kerr) sort hul ------------
    // Minimerer (r² - 6Mr ± 8a√(Mr) - 3a²)²
    // v[0] er radius r, som vi minimerer over.
    // v[1] er den faste spin-parameter 'a' for det sorte hul.
    public static double IscoKerrPrograde(vector v)
    {
        double r = v[0]; // radius
        double a = v[1]; // spin parameter
        const double M = 1.0; // mass

        double value = r * r - 6 * M * r - 8 * a * Math.Sqrt(M * r) + 3 * a * a;
        return value * value;
    }

    public static double IscoKerrRetrograde(vector v)
    {
        double r = v[0];
        double a = v[1];
        const double M = 1.0; // Massen er stadig 1

        // Ligningen for retrograde ISCO
        double value = r * r - 6 * M * r + 8 * a * Math.Sqrt(M * r) + 3 * a * a;
        return value * value;
}
}