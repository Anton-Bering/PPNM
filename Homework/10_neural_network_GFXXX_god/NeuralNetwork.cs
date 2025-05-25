using System;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork
{
    private int n;           // number of hidden neurons
    private double[] a;      // centers (a_i)
    private double[] b;      // width parameters (b_i)
    private double[] w;      // weights (w_i)

    private static double Activation(double x) => x * Math.Exp(-x * x);
    private static double ActivationPrime(double x) => (1 - 2 * x * x) * Math.Exp(-x * x);
    private static double ActivationDoublePrime(double x) => (4 * x * x * x - 6 * x) * Math.Exp(-x * x);
    private static double ActivationTriple(double x) => (-8 * Math.Pow(x, 4) + 24 * x * x - 6) * Math.Exp(-x * x);
    private static double ActivationIntegral(double x) => 0.5 * (1 - Math.Exp(-x * x));

    public NeuralNetwork(int nHidden, double inputMin = -1.0, double inputMax = 1.0)
    {
        n = nHidden;
        a = new double[n];
        b = new double[n];
        w = new double[n];
        Random rnd = new Random();
        double range = inputMax - inputMin;
        for (int i = 0; i < n; i++)
        {
            a[i] = inputMin + rnd.NextDouble() * range;
            b[i] = 0.1 * range + rnd.NextDouble() * (0.4 * range);
            if (b[i] <= 0) b[i] = 0.1 * range;
            w[i] = (rnd.NextDouble() - 0.5);
        }
    }

    public double Response(double x)
    {
        double sum = 0.0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            sum += w[i] * Activation(u);
        }
        return sum;
    }

    public double Derivative(double x)
    {
        double sum = 0.0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            sum += w[i] * ActivationPrime(u) / b[i];
        }
        return sum;
    }

    public double SecondDerivative(double x)
    {
        double sum = 0.0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            sum += w[i] * ActivationDoublePrime(u) / (b[i] * b[i]);
        }
        return sum;
    }

    public double AntiDerivative(double x)
    {
        double sum = 0.0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            sum += w[i] * b[i] * ActivationIntegral(u);
        }
        return sum;
    }

    public double ComputeCost(double[] xs, double[] ys)
    {
        double total = 0.0;
        for (int k = 0; k < xs.Length; k++)
        {
            double err = Response(xs[k]) - ys[k];
            total += err * err;
        }
        return total;
    }

    public void Train(double[] xs, double[] ys, int epochs, double learningRate)
    {
        int N = xs.Length;
        for (int iter = 0; iter < epochs; iter++)
        {
            double[] grad_a = new double[n];
            double[] grad_b = new double[n];
            double[] grad_w = new double[n];

            for (int k = 0; k < N; k++)
            {
                double x = xs[k];
                double y_target = ys[k];
                double y_pred = 0.0;
                double[] f_u = new double[n];
                double[] fprime_u = new double[n];

                for (int i = 0; i < n; i++)
                {
                    double u = (x - a[i]) / b[i];
                    double f_val = Activation(u);
                    y_pred += w[i] * f_val;
                    f_u[i] = f_val;
                    fprime_u[i] = ActivationPrime(u);
                }

                double error = y_pred - y_target;
                for (int i = 0; i < n; i++)
                {
                    double u = (x - a[i]) / b[i];
                    grad_w[i] += 2 * error * f_u[i];
                    grad_a[i] += 2 * error * (-w[i] * fprime_u[i] / b[i]);
                    grad_b[i] += 2 * error * (-w[i] * u * fprime_u[i] / b[i]);
                }
            }

            for (int i = 0; i < n; i++)
            {
                a[i] -= learningRate * grad_a[i];
                b[i] -= learningRate * grad_b[i];
                w[i] -= learningRate * grad_w[i];
                if (b[i] <= 1e-9) b[i] = 1e-9;
            }
        }
    }

    public double ComputeCostDifferential(Func<double, double, double, double, double> phiFunc,
                                         double aDom, double bDom, double cPoint,
                                         double y_c, double yprime_c,
                                         double alpha, double beta)
    {
        int M = 100;
        double h = (bDom - aDom) / M;
        double integralSum = 0.0;
        for (int j = 0; j < M; j++)
        {
            double x = aDom + (j + 0.5) * h;
            double y = Response(x);
            double y1 = Derivative(x);
            double y2 = SecondDerivative(x);
            double phiVal = phiFunc(y2, y1, y, x);
            integralSum += phiVal * phiVal;
        }
        double integralTerm = (bDom - aDom) * integralSum / M;
        double bcTerm = alpha * Math.Pow(Response(cPoint) - y_c, 2)
                      + beta * Math.Pow(Derivative(cPoint) - yprime_c, 2);
        return integralTerm + bcTerm;
    }

    public void TrainDifferentialEquation(Func<double, double, double, double, double> phiFunc,
                                          double aDom, double bDom, double cPoint,
                                          double y_c, double yprime_c,
                                          double alpha, double beta,
                                          int epochs, double learningRate)
    {
        int M = 100;
        double h = (bDom - aDom) / M;
        List<double> costHistory = new List<double>();

        for (int iter = 0; iter < epochs; iter++)
        {
            double[] grad_a = new double[n];
            double[] grad_b = new double[n];
            double[] grad_w = new double[n];

            for (int j = 0; j < M; j++)
            {
                double x = aDom + (j + 0.5) * h;
                double y = Response(x);
                double y1 = Derivative(x);
                double y2 = SecondDerivative(x);
                double phiVal = phiFunc(y2, y1, y, x);
                if (phiVal == 0.0) continue;

                double eps = 1e-6;
                double phi_y = (phiFunc(y2, y1, y + eps, x) - phiVal) / eps;
                double phi_y1 = (phiFunc(y2, y1 + eps, y, x) - phiVal) / eps;
                double phi_y2 = (phiFunc(y2 + eps, y1, y, x) - phiVal) / eps;

                for (int i = 0; i < n; i++)
                {
                    double u = (x - a[i]) / b[i];
                    double dy_da = -w[i] * ActivationPrime(u) / b[i];
                    double dy_db = -w[i] * u * ActivationPrime(u) / b[i];
                    double dy_dw = Activation(u);

                    double dy1_da = -w[i] * ActivationDoublePrime(u) / (b[i] * b[i]);
                    double dy1_db = -w[i] / (b[i] * b[i]) * (ActivationPrime(u) + u * ActivationDoublePrime(u));
                    double dy1_dw = ActivationPrime(u) / b[i];

                    double dy2_da = -w[i] * ActivationTriple(u) / (b[i] * b[i] * b[i]);
                    double dy2_db = -w[i] / (b[i] * b[i] * b[i]) * (2 * ActivationDoublePrime(u) + u * ActivationTriple(u));
                    double dy2_dw = ActivationDoublePrime(u) / (b[i] * b[i]);

                    double commonFactor = 2 * phiVal;
                    grad_a[i] += commonFactor * (phi_y * dy_da + phi_y1 * dy1_da + phi_y2 * dy2_da);
                    grad_b[i] += commonFactor * (phi_y * dy_db + phi_y1 * dy1_db + phi_y2 * dy2_db);
                    grad_w[i] += commonFactor * (phi_y * dy_dw + phi_y1 * dy1_dw + phi_y2 * dy2_dw);
                }
            }

            double weight = (bDom - aDom) / M;
            for (int i = 0; i < n; i++)
            {
                grad_a[i] *= weight;
                grad_b[i] *= weight;
                grad_w[i] *= weight;
            }

            double bcError_y = Response(cPoint) - y_c;
            double bcError_y1 = Derivative(cPoint) - yprime_c;

            for (int i = 0; i < n; i++)
            {
                double u_c = (cPoint - a[i]) / b[i];
                double dF_da = -w[i] * ActivationPrime(u_c) / b[i];
                double dF_db = -w[i] * u_c * ActivationPrime(u_c) / b[i];
                double dF_dw = Activation(u_c);

                double dF1_da = -w[i] * ActivationDoublePrime(u_c) / (b[i] * b[i]);
                double dF1_db = -w[i] / (b[i] * b[i]) * (ActivationPrime(u_c) + u_c * ActivationDoublePrime(u_c));
                double dF1_dw = ActivationPrime(u_c) / b[i];

                grad_a[i] += 2 * alpha * bcError_y * dF_da + 2 * beta * bcError_y1 * dF1_da;
                grad_b[i] += 2 * alpha * bcError_y * dF_db + 2 * beta * bcError_y1 * dF1_db;
                grad_w[i] += 2 * alpha * bcError_y * dF_dw + 2 * beta * bcError_y1 * dF1_dw;
            }

            for (int i = 0; i < n; i++)
            {
                a[i] -= learningRate * grad_a[i];
                b[i] -= learningRate * grad_b[i];
                w[i] -= learningRate * grad_w[i];
                if (b[i] <= 1e-9) b[i] = 1e-9;
            }

            double currentCost = ComputeCostDifferential(phiFunc, aDom, bDom, cPoint, y_c, yprime_c, alpha, beta);
            costHistory.Add(currentCost);
        }

        using (StreamWriter writer = new StreamWriter("cost_data.txt"))
        {
            for (int i = 0; i < costHistory.Count; i++)
            {
                writer.WriteLine($"{i} {costHistory[i]}");
            }
        }
    }
}
