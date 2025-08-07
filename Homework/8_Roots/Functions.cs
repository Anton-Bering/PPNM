using System;
using static System.Math;

public static class Functions
{
    // Rosenbrock's funktion
    public static Vector RosenbrockGradient(Vector v)
    {
        double x = v[0], y = v[1];
        double df_dx = -2 * (1 - x) - 400 * x * (y - x * x);
        double df_dy = 200 * (y - x * x);
        return new Vector(df_dx, df_dy);
    }

    // Himmelblau's funktion
    public static Vector HimmelblauGradient(Vector v)
    {
        double x = v[0], y = v[1];
        double df_dx = 2 * (x * x + y - 11) * (2 * x) + 2 * (x + y * y - 7);
        double df_dy = 2 * (x * x + y - 11) + 2 * (x + y * y - 7) * (2 * y);
        return new Vector(df_dx, df_dy);
    }

    // Schrödinger-ligningen
    public static Func<double, Vector, Vector> SchrodingerEq(double E)
    {
        return (r, y) => new Vector(y[1], -2 * (E + 1 / r) * y[0]);
    }

    // M(E) funktion til shooting method med justerbare parametre
    public static Func<Vector, Vector> Create_M_function(
        double r_max,
        double r_min = 1e-3,
        double acc = 1e-3,
        double eps = 1e-3)
    {
        return (E_vec) =>
        {
            double E = E_vec[0];
            var f_rmin = r_min - r_min * r_min;
            var df_rmin = 1 - 2 * r_min;
            Vector y_start = new Vector(f_rmin, df_rmin);
            var (_, ys) = OdeSolver.Solve(SchrodingerEq(E), r_min, y_start, r_max, acc: acc, eps: eps);
            return new Vector(ys[ys.Count - 1][0]); // Returner f(r_max)
        };
    }
}