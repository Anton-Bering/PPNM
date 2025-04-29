using System;
using static System.Math;
using static System.Console;
using System.IO;

class main {
    static double erf(double x) {
        if (x < 0) return -erf(-x);
        double[] a = {0.254829592, -0.284496736, 1.421413741, -1.453152027, 1.061405429};
        double t = 1 / (1 + 0.3275911 * x);
        double sum = t*(a[0]+t*(a[1]+t*(a[2]+t*(a[3]+t*a[4]))));
        return 1 - sum * Exp(-x * x);
    }

    static double gamma(double x) {
        if (x < 0) return PI / Sin(PI * x) / gamma(1 - x);
        if (x < 9) return gamma(x + 1) / x;
        double ln = Log(2 * PI) / 2 + (x - 0.5) * Log(x) - x
                    + (1.0 / 12) / x - (1.0 / 360) / Pow(x, 3) + (1.0 / 1260) / Pow(x, 5);
        return Exp(ln);
    }

    static double lngamma(double x) {
        if (x <= 0) throw new ArgumentException("lngamma: x<=0");
        if (x < 9) return lngamma(x + 1) - Log(x);
        return x * Log(x + 1 / (12 * x - 1 / x / 10)) - x + Log(2 * PI / x) / 2;
    }

    static void Main() {
        using (var erfOut = new StreamWriter("erf.txt"))
        using (var gammaOut = new StreamWriter("gamma.txt"))
        using (var lngammaOut = new StreamWriter("lngamma.txt")) {
            for (double x = -3; x <= 3; x += 1.0 / 64) erfOut.WriteLine($"{x} {erf(x)}");
            for (double x = 0.1; x <= 10; x += 1.0 / 32) gammaOut.WriteLine($"{x} {gamma(x)}");
            for (double x = 0.1; x <= 10; x += 1.0 / 32) lngammaOut.WriteLine($"{x} {lngamma(x)}");
        }
    }
}
