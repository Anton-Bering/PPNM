using System;
using System.Collections.Generic;
using static System.Math;  // Allows use of Math functions like Cos, Exp without prefix

namespace NeuralNetworkApp {
    /// <summary>
    /// Program entry point. Generates sample data from a target function, 
    /// trains the ANN on this data, and prints the approximation results.
    /// </summary>
    public class Program {
        public static void Main(string[] args) {
            // Define the target function g(x) = cos(5x - 1) * exp(-x^2).
            Func<double, double> targetFunction = x => Cos(5 * x - 1) * Exp(-x * x);

            // Generate training data by sampling g(x) on the interval [-1, 1].
            var xData = new List<double>();
            var yData = new List<double>();
            double step = 0.1;
            for (double x = -1.0; x <= 1.000001; x += step) {
                xData.Add(x);
                yData.Add(targetFunction(x));
            }

            // Initialize an ANN with a chosen number of hidden neurons (e.g., 5).
            int numHiddenNeurons = 5;
            Ann ann = new Ann(numHiddenNeurons);

            // Compute initial mean squared error before training (for comparison).
            double initialError = ann.ComputeTotalError(xData, yData);
            double initialMSE = initialError / xData.Count;
            Console.WriteLine($"Initial MSE (before training): {initialMSE:F6}");

            // Train the network on the dataset.
            ann.Train(xData, yData);

            // Compute error after training.
            double finalError = ann.ComputeTotalError(xData, yData);
            double finalMSE = finalError / xData.Count;
            Console.WriteLine($"Final MSE (after training):   {finalMSE:F6}");

            // Display sample outputs comparing the target function and the network approximation.
            Console.WriteLine("\nSample outputs after training:");
            Console.WriteLine("   x\tg(x) (target)\tF_p(x) (approx)");
            for (double testX = -1.0; testX <= 1.0001; testX += 0.25) {
                double actualY = targetFunction(testX);
                double predictedY = ann.Response(testX);
                Console.WriteLine($"{testX,5:F2}\t{actualY,10:F6}\t{predictedY,13:F6}");
            }
        }
    }
}
