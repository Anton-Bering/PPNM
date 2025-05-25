using System;

namespace NewtonMinimization
{
    public static class TestFunctions
    {
        public static double Rosenbrock(double[] vars)
        {
            double x = vars[0];
            double y = vars[1];
            return (1 - x) * (1 - x) + 100 * (y - x * x) * (y - x * x);
        }

        public static double Himmelblau(double[] vars)
        {
            double x = vars[0];
            double y = vars[1];
            return (x * x + y - 11) * (x * x + y - 11) + (x + y * y - 7) * (x + y * y - 7);
        }
    }
}