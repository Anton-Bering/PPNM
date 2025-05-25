using System;
using System.Collections.Generic;

namespace NeuralNetworkApp {
    /// <summary>
    /// Class representing an Artificial Neural Network (ANN) with one input, 
    /// a configurable number of hidden neurons (with Gaussian wavelet activation), 
    /// and one output neuron (summation).
    /// </summary>
    public class Ann {
        // Number of hidden neurons
        private int n;
        // Parameters for each hidden neuron:
        private double[] a; // "a_i" centers (input shift for neuron i)
        private double[] b; // "b_i" scale (width) for neuron i
        private double[] w; // "w_i" weight from neuron i to output

        // Activation function f. Here we use Gaussian wavelet: f(x) = x * exp(-x^2).
        private readonly Func<double, double> f = x => x * Math.Exp(-x * x);

        /// <summary>
        /// Initializes an ANN with a given number of hidden neurons. 
        /// Parameters (a, b, w) are initialized randomly.
        /// </summary>
        /// <param name="numHidden">Number of hidden layer neurons</param>
        public Ann(int numHidden) {
            n = numHidden;
            // Allocate parameter arrays
            a = new double[n];
            b = new double[n];
            w = new double[n];
            // Randomly initialize parameters
            var rand = new Random();
            for (int i = 0; i < n; i++) {
                // Initialize 'a' (center) randomly within [-1, 1]
                a[i] = 2 * rand.NextDouble() - 1;
                // Initialize 'b' (scale) in a reasonable range (e.g., [0.5, 1.5]) to avoid zero or extreme values
                b[i] = 0.5 + rand.NextDouble();
                // Initialize weight 'w' in [-1, 1]
                w[i] = 2 * rand.NextDouble() - 1;
            }
        }

        /// <summary>
        /// Computes the output of the neural network for a given input x.
        /// This is the network response F_p(x) = sum_i [ w_i * f((x - a_i) / b_i) ].
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Network output value for input x</returns>
        public double Response(double x) {
            double sum = 0.0;
            for (int i = 0; i < n; i++) {
                // Hidden neuron i: apply activation to (x - a_i)/b_i, then multiply by weight w_i
                double u = (x - a[i]) / b[i];   // scaled input for neuron i
                double hiddenOut = f(u);        // neuron output using activation function f
                sum += w[i] * hiddenOut;        // accumulate weighted output
            }
            return sum;
        }

        /// <summary>
        /// Train the neural network to fit the given training data {(x_k, y_k)} by adjusting parameters a_i, b_i, w_i.
        /// Uses simple gradient descent to minimize C(p) = sum_k [F_p(x_k) - y_k]^2.
        /// </summary>
        /// <param name="xData">List of input values (training data)</param>
        /// <param name="yData">List of target output values corresponding to xData</param>
        public void Train(List<double> xData, List<double> yData) {
            if (xData.Count != yData.Count || xData.Count == 0) {
                throw new ArgumentException("Training data lists must have the same non-zero length.");
            }
            int dataCount = xData.Count;
            double learningRate = 0.01;    // learning rate for gradient descent
            int maxIterations = 10000;     // number of training iterations (epochs)

            // Gradient descent loop
            for (int iter = 0; iter < maxIterations; iter++) {
                // Initialize gradients for each parameter to 0
                double[] grad_a = new double[n];
                double[] grad_b = new double[n];
                double[] grad_w = new double[n];

                // Compute gradient over all data points
                for (int k = 0; k < dataCount; k++) {
                    double x = xData[k];
                    double yTarget = yData[k];
                    // Compute network output for this data point
                    double output = Response(x);
                    double error = output - yTarget;  // error = F_p(x) - y

                    // Accumulate gradient contributions from this data point for each parameter
                    for (int i = 0; i < n; i++) {
                        double u = (x - a[i]) / b[i];
                        double f_u = f(u);
                        // Derivative f'(u) for f(u) = u * exp(-u^2) is f'(u) = exp(-u^2) * (1 - 2u^2)
                        double f_u_deriv = Math.Exp(-u * u) * (1 - 2 * u * u);

                        // Partial derivatives for each parameter:
                        // ∂C/∂w_i = 2 * error * f(u)
                        grad_w[i] += 2 * error * f_u;
                        // ∂C/∂a_i = 2 * error * w_i * f'(u) * ∂u/∂a_i, with ∂u/∂a_i = -1/b_i
                        grad_a[i] += 2 * error * w[i] * f_u_deriv * (-1.0 / b[i]);
                        // ∂C/∂b_i = 2 * error * w_i * f'(u) * ∂u/∂b_i, with ∂u/∂b_i = -(x - a_i) / b_i^2
                        grad_b[i] += 2 * error * w[i] * f_u_deriv * (-(x - a[i]) / (b[i] * b[i]));
                    }
                }

                // Update each parameter by taking a step opposite to its gradient
                for (int i = 0; i < n; i++) {
                    a[i] -= learningRate * grad_a[i];
                    b[i] -= learningRate * grad_b[i];
                    w[i] -= learningRate * grad_w[i];
                    // Ensure b[i] remains positive and not too close to zero
                    if (b[i] <= 1e-6) {
                        b[i] = 1e-6;
                    }
                }
            }
        }

        /// <summary>
        /// Computes the total squared error (cost) on the given dataset for the current network parameters.
        /// </summary>
        /// <param name="xData">List of input values</param>
        /// <param name="yData">Corresponding list of target outputs</param>
        /// <returns>Total sum of squared errors over the dataset</returns>
        public double ComputeTotalError(List<double> xData, List<double> yData) {
            double totalError = 0.0;
            for (int k = 0; k < xData.Count; k++) {
                double diff = Response(xData[k]) - yData[k];
                totalError += diff * diff;
            }
            return totalError;
        }
    }
}
