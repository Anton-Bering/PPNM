using System;
using System.Text;

public static class VectorHelpers
{
    // Uniform random vektor i [min,max]
    public static vector RandomVector(int n, Random rng, double min = -1.0, double max = 1.0)
    {
        if (n <= 0) throw new ArgumentException("n must be positive.");
        if (rng == null) throw new ArgumentNullException(nameof(rng));
        var v = new vector(n);
        double span = max - min;
        for (int i = 0; i < n; i++) v[i] = min + span * rng.NextDouble();
        return v;
    }

    // Overload med internt Random
    public static vector RandomVector(int n) =>
        RandomVector(n, new Random());

    // a ≈ b
    public static bool CheckVectorEqual(vector a, vector b, string aName = "a", string bName = "b", double tol = 1e-6, bool throwOnFail = true)
    {
        if (a == null || b == null) throw new ArgumentNullException("a/b");
        if (a.Size != b.Size)
        {
            string msg = $"{aName} and {bName} length mismatch: {a.Size} vs {b.Size}.";
            if (throwOnFail) throw new Exception(msg);
            return false;
        }
        for (int i = 0; i < a.Size; i++)
            if (Math.Abs(a[i] - b[i]) > tol)
            {
                string msg = $"{aName} ≉ {bName} at [{i}]: {a[i]} vs {b[i]} (tol={tol}).";
                if (throwOnFail) throw new Exception(msg);
                return false;
            }
        Console.WriteLine($"{aName} ≈ {bName} (tol={tol}).");
        return true;
    }

    // Pæn udskrift
    public static string PrintVector(vector v, string name = "", int dec = 5)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Vector {name}:");
        string fmt = "{0," + (dec + 8).ToString() + ":F" + dec + "}";
        for (int i = 0; i < v.Size; i++)
            sb.AppendFormat(fmt + "\n", v[i]);
        return sb.ToString();
    }

}
