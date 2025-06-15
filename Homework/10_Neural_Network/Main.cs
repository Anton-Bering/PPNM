using System;
using System.IO;

public class Program
{
    public static void Main(string[] args)
    {
        // Part A: Function approximation with ANN
        Console.WriteLine("--- --- Part A: Function Approximation --- ---\n");
        Console.WriteLine("Approximate: g(x)=Cos(5*x-1)*Exp(-x*x)");
        Func<double, double> g = x => Math.Cos(5 * x - 1) * Math.Exp(-x * x);
        int N = 101;
        double[] xs = new double[N];
        double[] ys = new double[N];
        double x_min = -1.0, x_max = 1.0;
        for (int k = 0; k < N; k++)
        {
            double x = x_min + (x_max - x_min) * k / (N - 1);
            xs[k] = x;
            ys[k] = g(x);
        }
        int hiddenCount = 20;
        NeuralNetwork net1 = new NeuralNetwork(hiddenCount, x_min, x_max);
        double initCost = net1.ComputeCost(xs, ys);
        int epochsA = 10000;
        double learnRateA = 0.01;
        net1.Train(xs, ys, epochsA, learnRateA);
        double finalCost = net1.ComputeCost(xs, ys);
        Console.WriteLine($"Initial cost = {initCost:F6}, Final cost = {finalCost:F6}");
        Console.WriteLine("Sample predictions vs actual (after training):");
        Console.WriteLine("    x        F(x) (pred)    g(x) (actual)    error");
        double[] testXs = new double[] { -1.0, -0.5, 0.0, 0.5, 1.0 };
        foreach (double x in testXs)
        {
            double pred = net1.Response(x);
            double actual = g(x);
            double error = pred - actual;
            Console.WriteLine($"{x,6:F2}    {pred,12:F6}    {actual,12:F6}    {error,12:F6}");
        }
        Console.WriteLine();

        using (StreamWriter writer = new StreamWriter("plot_data.txt"))
        {
            int M = 500;
            for (int i = 0; i < M; i++)
            {
                double x = x_min + (x_max - x_min) * i / (M - 1);
                double fx = net1.Response(x);
                double gx = g(x);
                writer.WriteLine($"{x} {gx} {fx}");
            }
        }
        Console.WriteLine("Wrote plot_data.txt");

        Console.WriteLine(" --- --- Part B: Derivatives and Antiderivative --- ---\n");
        double testX = 0.0;
        double netY = net1.Response(testX);
        double netY1 = net1.Derivative(testX);
        double netY2 = net1.SecondDerivative(testX);
        double netInt = net1.AntiDerivative(testX);
        double actY = g(testX);
        Func<double, double> g1 = x => Math.Exp(-x * x) * (-5 * Math.Sin(5 * x - 1) - 2 * x * Math.Cos(5 * x - 1));
        Func<double, double> g2 = x => Math.Exp(-x * x) * ((4 * x * x - 27) * Math.Cos(5 * x - 1) + 20 * x * Math.Sin(5 * x - 1));
        double actY1 = g1(testX);
        double actY2 = g2(testX);
        Console.WriteLine($"At x = {testX:F2}:");
        Console.WriteLine($"  F(x) = {netY:F6},    g(x) = {actY:F6}");
        Console.WriteLine($"  F'(x) = {netY1:F6},   g'(x) = {actY1:F6}");
        Console.WriteLine($"  F''(x) = {netY2:F6},  g''(x) = {actY2:F6}");
        Console.WriteLine($"  ∫F(x) dx (antiderivative) = {netInt:F6}  (constant of integration omitted)");
        Console.WriteLine();

        using (StreamWriter writer = new StreamWriter("derivatives_data.txt"))
        {
            int M = 500;
            for (int i = 0; i < M; i++)
            {
                double x = x_min + (x_max - x_min) * i / (M - 1);
                double gx = g(x);
                double fx = net1.Response(x);
                double g1x = g1(x);
                double f1x = net1.Derivative(x);
                double g2x = g2(x);
                double f2x = net1.SecondDerivative(x);
                writer.WriteLine($"{x} {gx} {fx} {g1x} {f1x} {g2x} {f2x}");
            }
        }
        Console.WriteLine("Wrote derivatives_data.txt");

        Console.WriteLine("--- --- Part C: Solving a Differential Equation with ANN --- ---\n");
        double aDom = 0.0;
        double bDom = Math.PI;
        double cPoint = 0.0;
        double y_c = 0.0;
        double yprime_c = 1.0;
        Func<double, double, double, double, double> phi = (y2, y1, yval, x) => y2 + yval;
        NeuralNetwork net2 = new NeuralNetwork(hiddenCount, aDom, bDom);
        double initCostDE = net2.ComputeCostDifferential(phi, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100.0, beta: 100.0);
        int epochsC = 15000;
        double learnRateC = 0.005;
        net2.TrainDifferentialEquation(phi, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100.0, beta: 100.0, epochs: epochsC, learningRate: learnRateC);
        double finalCostDE = net2.ComputeCostDifferential(phi, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100.0, beta: 100.0);
        Console.WriteLine($"Initial cost = {initCostDE:F6}, Final cost = {finalCostDE:F6}");
        double y_c_pred = net2.Response(cPoint);
        double yprime_c_pred = net2.Derivative(cPoint);
        Console.WriteLine($"Boundary check: F({cPoint}) = {y_c_pred:F6} (target {y_c:F2}),  F'({cPoint}) = {yprime_c_pred:F6} (target {yprime_c:F2})");
        Console.WriteLine("Sample solution values vs actual sin(x):");
        Console.WriteLine("    x        F(x) (pred)    sin(x) (actual)   error");
        double[] testXs2 = new double[] { 0.0, Math.PI / 6, Math.PI / 4, Math.PI / 2, Math.PI };
        foreach (double x in testXs2)
        {
            double y_pred = net2.Response(x);
            double y_act = Math.Sin(x);
            double err = y_pred - y_act;
            Console.WriteLine($"{x,6:F2}    {y_pred,12:F6}    {y_act,12:F6}    {err,12:F6}");
        }

        using (StreamWriter writer = new StreamWriter("solution_data.txt"))
        {
            int M = 500;
            for (int i = 0; i < M; i++)
            {
                double x = aDom + (bDom - aDom) * i / (M - 1);
                double fx = net2.Response(x);
                double gx = Math.Sin(x);
                double f1x = net2.Derivative(x);
                double g1x = Math.Cos(x);
                writer.WriteLine($"{x} {gx} {fx} {g1x} {f1x}");
            }
        }
        Console.WriteLine("Wrote solution_data.txt");

        using (StreamWriter writer = new StreamWriter("residual_data.txt"))
        {
            int M = 500;
            for (int i = 0; i < M; i++)
            {
                double x = aDom + (bDom - aDom) * i / (M - 1);
                double y = net2.Response(x);
                double y2 = net2.SecondDerivative(x);
                double residual = y2 + y;
                writer.WriteLine($"{x} {residual}");
            }
        }
        Console.WriteLine("Wrote residual_data.txt");
    }
}