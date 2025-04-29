using static System.Console;
using static System.Math;
using static sfuns;

class main {
    static int Main() {
        // Her er del 1:
        WriteLine($"Sqrt(2)     = {Sqrt(2)}");
        WriteLine($"2^(1/5)     = {Pow(2,1.0/5)}");
        WriteLine($"exp(PI)     = {Exp(PI)}");
        WriteLine($"PI^e        = {Pow(PI,E)}");

        // Del 2:
        for (int i = 1; i <= 10; i++) {
            double g = fgamma(i);
            WriteLine($"fgamma({i}) = {g}");
        }

        // Del 3:
        for (int i = 1; i <= 10; i++) {
            double l = lngamma(i);
            WriteLine($"lngamma({i}) = {l}");
        }

        return 0;
    }
}
