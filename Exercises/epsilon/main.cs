using static System.Console;
using static System.Math;

class main {
    static bool approx(double a, double b, double acc = 1e-9, double eps = 1e-9) {
        if (Abs(b - a) <= acc) return true;
        if (Abs(b - a) <= Max(Abs(a), Abs(b)) * eps) return true;
        return false;
    }

    static int Main() {
        // 1. Max og Min int
        int i = 1;
        while (i + 1 > i) i++;
        WriteLine($"My max int = {i}");
        WriteLine($"System max = {int.MaxValue}");

        i = -1;
        while (i - 1 < i) i--;
        WriteLine($"My min int = {i}");
        WriteLine($"System min = {int.MinValue}");

        // 2. Machine epsilon
        double x = 1.0;
        while (1.0 + x != 1.0) x /= 2.0;
        x *= 2.0;
        WriteLine($"double epsilon ≈ {x}");
        WriteLine($"expected ≈ {Pow(2, -52)}");

        float y = 1.0f;
        while (1.0f + y != 1.0f) y /= 2.0f;
        y *= 2.0f;
        WriteLine($"float epsilon ≈ {y}");
        WriteLine($"expected ≈ {Pow(2, -23)}");

        // 3. tiny og a/b-sammenligning
        double epsilon = Pow(2, -52);
        double tiny = epsilon / 2;
        double a = 1 + tiny + tiny;
        double b = tiny + tiny + 1;
        WriteLine($"a==b? {a == b}");
        WriteLine($"a>1?  {a > 1}");
        WriteLine($"b>1?  {b > 1}");

        // 4. sammenligning af 0.8
        double d1 = 0.1 + 0.1 + 0.1 + 0.1 + 0.1 + 0.1 + 0.1 + 0.1;
        double d2 = 8 * 0.1;
        WriteLine($"d1 = {d1:e15}");
        WriteLine($"d2 = {d2:e15}");
        WriteLine($"d1==d2? => {d1 == d2}");
        WriteLine($"approx(d1,d2)? => {approx(d1,d2)}");

        return 0;
    }
}
