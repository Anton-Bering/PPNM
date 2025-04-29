using static System.Console;
using static System.Math;
using System.Numerics;

class main {
    static bool approx(double a, double b, double acc=1e-9, double eps=1e-9) {
        if (Abs(a - b) <= acc) return true;
        if (Abs(a - b) <= Max(Abs(a), Abs(b)) * eps) return true;
        return false;
    }

    static bool approx(Complex a, Complex b, double acc=1e-9, double eps=1e-9) {
        return approx(a.Real, b.Real, acc, eps) && approx(a.Imaginary, b.Imaginary, acc, eps);
    }

    static void Main() {
        Complex i = Complex.ImaginaryOne;

        Complex sqrtNeg1 = Complex.Sqrt(-1);
        Complex sqrtI = Complex.Sqrt(i);
        Complex expI = Complex.Exp(i);
        Complex expIPI = Complex.Exp(i * PI);
        Complex ipowI = Complex.Pow(i, i);
        Complex lnI = Complex.Log(i);
        Complex sinIPI = Complex.Sin(i * PI);

        // Teoretiske værdier
        Complex expectedSqrtNeg1 = i;
        Complex expectedSqrtI = new Complex(1/Sqrt(2), 1/Sqrt(2));
        Complex expectedExpIPI = -1;
        Complex expectedIPowI = Exp(-PI/2);
        Complex expectedLnI = i * PI / 2;

        WriteLine($"sqrt(-1)         = {sqrtNeg1}, approx? {approx(sqrtNeg1, expectedSqrtNeg1)}");
        WriteLine($"sqrt(i)          = {sqrtI}, approx? {approx(sqrtI, expectedSqrtI)}");
        WriteLine($"exp(i)           = {expI}");
        WriteLine($"exp(iπ)          = {expIPI}, approx? {approx(expIPI, expectedExpIPI)}");
        WriteLine($"i^i              = {ipowI}, approx? {approx(ipowI.Real, expectedIPowI)}");
        WriteLine($"ln(i)            = {lnI}, approx? {approx(lnI, expectedLnI)}");
        WriteLine($"sin(iπ)          = {sinIPI}");

        // Ekxter:
        Complex sinhI = Complex.Sinh(i);
        Complex coshI = Complex.Cosh(i);
        WriteLine($"sinh(i)          = {sinhI}, should be i*sin(1) = {i * Sin(1)}");
        WriteLine($"cosh(i)          = {coshI}, should be cos(1) = {Cos(1)}");
    }
}
