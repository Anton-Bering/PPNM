using static System.Console;
using static vec;

class main {
    static int Main() {
        var a = new vec(1, 2, 3);
        var b = new vec(4, 5, 6);

        a.print("a = ");
        b.print("b = ");

        WriteLine($"a + b = {a + b}");
        WriteLine($"a - b = {a - b}");
        WriteLine($"-a = {-a}");
        WriteLine($"2*a = {2 * a}");
        WriteLine($"a/2 = {a / 2}");
        WriteLine($"dot(a, b) = {vec.dot(a, b)}");
        WriteLine($"norm(a) = {a.norm()}");
        WriteLine($"cross(a, b) = {vec.cross(a, b)}");

        var c = new vec(1.000000001, 2.0, 3.0);
        WriteLine($"approx(a, c)? {vec.approx(a, c)}");

        return 0;
    }
}
