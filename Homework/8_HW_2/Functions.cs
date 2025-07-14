// ---------- Functions.cs ----------
using System;

public static class Functions
{
    // --- One‑ and two‑dimensional test equations -----------------------------
    public static Func<Vec, Vec> Simple1D = (v) =>
    {
        double x = v[0];
        return new Vec(Math.Pow(x, 3) - 2);   // root at ∛2
    };

    public static Func<Vec, Vec> Simple2D = (v) =>
    {
        double x = v[0], y = v[1];
        return new Vec(
            x * x + y * y - 4,   // circle radius 2
            x - y
        );
    };

    // --- Rosenbrock ----------------------------------------------------------
    public static Func<Vec, Vec> RosenbrockGrad = (v) =>
    {
        double x = v[0], y = v[1];
        return new Vec(
            -2 * (1 - x) - 400 * x * (y - x * x),
            200 * (y - x * x)
        );
    };

    // --- Himmelblau ----------------------------------------------------------
    public static Func<Vec, Vec> HimmelblauGrad = (v) =>
    {
        double x = v[0], y = v[1];
        return new Vec(
            4 * (x * x + y - 11) * x + 2 * (x + y * y - 7),
            2 * (x * x + y - 11) + 4 * (x + y * y - 7) * y
        );
    };
}
// ---------------------------------------------------------------------------
