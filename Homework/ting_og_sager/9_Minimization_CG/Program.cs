using System;

namespace NewtonMinimization
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] startRosenbrock = { -1.2, 1.0 };
            double[] startHimmelblau = { -2.0, 2.0 };

            int iterRosen;
            double[] resultRosen = NewtonMinimizer.Minimize(TestFunctions.Rosenbrock, startRosenbrock, 1e-6, out iterRosen);
            Console.WriteLine($"Rosenbrock minimized in {iterRosen} steps.");
            Console.WriteLine($"Minimum at (x, y) = ({resultRosen[0]:F6}, {resultRosen[1]:F6}), f = {TestFunctions.Rosenbrock(resultRosen):E}");

            int iterHimmel;
            double[] resultHimmel = NewtonMinimizer.Minimize(TestFunctions.Himmelblau, startHimmelblau, 1e-6, out iterHimmel);
            Console.WriteLine($"Himmelblau minimized in {iterHimmel} steps.");
            Console.WriteLine($"Minimum at (x, y) = ({resultHimmel[0]:F6}, {resultHimmel[1]:F6}), f = {TestFunctions.Himmelblau(resultHimmel):E}");
        }
    }
}