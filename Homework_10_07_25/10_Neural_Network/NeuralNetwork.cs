using System;
using System.Collections.Generic;
using System.IO;

public class NeuralNetwork
{
    private readonly int n;
    private readonly double[] a, b, w;

    private static double φ(double u)  => u * Math.Exp(-u * u);
    private static double φ1(double u) => (1 - 2 * u * u) * Math.Exp(-u * u);
    private static double φ2(double u) => (4 * u * u * u - 6 * u) * Math.Exp(-u * u);
    private static double φ3(double u) => (-8 * Math.Pow(u, 4) + 24 * u * u - 6) * Math.Exp(-u * u);
    private static double Φ(double u)  => 0.5 * (1 - Math.Exp(-u * u));

    public NeuralNetwork(int nHidden, double xMin = -1, double xMax = 1)
    {
        n = nHidden;
        a = new double[n];
        b = new double[n];
        w = new double[n];
        Random rng = new Random();
        double L = xMax - xMin;
        for (int i = 0; i < n; i++)
        {
            a[i] = xMin + rng.NextDouble() * L;
            b[i] = 0.1 * L + rng.NextDouble() * 0.4 * L;
            w[i] = rng.NextDouble() - 0.5;
        }
    }

    public double Response(double x)
    {
        double s = 0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            s += w[i] * φ(u);
        }
        return s;
    }

    public double Derivative(double x)
    {
        double s = 0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            s += w[i] * φ1(u) / b[i];
        }
        return s;
    }

    public double SecondDerivative(double x)
    {
        double s = 0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            s += w[i] * φ2(u) / (b[i] * b[i]);
        }
        return s;
    }

    public double AntiDerivative(double x)
    {
        double s = 0;
        for (int i = 0; i < n; i++)
        {
            double u = (x - a[i]) / b[i];
            s += w[i] * b[i] * Φ(u);
        }
        return s;
    }

    public double ComputeCost(double[] xs, double[] ys)
    {
        double tot = 0;
        for (int k = 0; k < xs.Length; k++)
        {
            double e = Response(xs[k]) - ys[k];
            tot += e * e;
        }
        return tot;
    }

    public void Train(double[] xs, double[] ys, int epochs, double learningRate)
    {
        int M = xs.Length;
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            double[] da = new double[n], db = new double[n], dw = new double[n];
            for (int k = 0; k < M; k++)
            {
                double x = xs[k], t = ys[k];
                double y = Response(x);
                double err = y - t;

                for (int i = 0; i < n; i++)
                {
                    double u = (x - a[i]) / b[i];
                    double φu = φ(u);
                    double φ1u = φ1(u);

                    dw[i] += 2 * err * φu;
                    da[i] += 2 * err * (-w[i] * φ1u / b[i]);
                    db[i] += 2 * err * (-w[i] * u * φ1u / b[i]);
                }
            }
            for (int i = 0; i < n; i++)
            {
                a[i] -= learningRate * da[i];
                b[i] -= learningRate * db[i];
                w[i] -= learningRate * dw[i];
                if (b[i] <= 1e-9) b[i] = 1e-9;
            }
        }
    }

    public double ComputeCostDifferential(
        Func<double, double, double, double, double> Φeq,
        double aDom, double bDom, double c,
        double y_c, double y1_c, double alpha, double beta)
    {
        int M = 100;
        double h = (bDom - aDom) / M;
        double sum = 0;
        for (int j = 0; j < M; j++)
        {
            double x = aDom + (j + 0.5) * h;
            double y = Response(x);
            double y1 = Derivative(x);
            double y2 = SecondDerivative(x);
            double φv = Φeq(y2, y1, y, x);
            sum += φv * φv;
        }
        double intTerm = sum * (bDom - aDom) / M;
        double bcTerm = alpha * Math.Pow(Response(c) - y_c, 2)
                      + beta * Math.Pow(Derivative(c) - y1_c, 2);
        return intTerm + bcTerm;
    }

    public void TrainDifferentialEquation(
        Func<double, double, double, double, double> Φeq,
        double aDom, double bDom, double c,
        double y_c, double y1_c, double alpha, double beta,
        int epochs, double learningRate,
        string costFilename = "differential_equation_cost.txt")
    {
        int M = 100;
        double h = (bDom - aDom) / M;
        var costHistory = new List<double>();

        for (int ep = 0; ep < epochs; ep++)
        {
            double[] da = new double[n], db = new double[n], dw = new double[n];

            for (int j = 0; j < M; j++)
            {
                double x = aDom + (j + 0.5) * h;
                double y = Response(x);
                double y1 = Derivative(x);
                double y2 = SecondDerivative(x);
                double φv = Φeq(y2, y1, y, x);
                if (φv == 0) continue;

                double eps = 1e-6;
                double Φy = (Φeq(y2, y1, y + eps, x) - φv) / eps;
                double Φy1 = (Φeq(y2, y1 + eps, y, x) - φv) / eps;
                double Φy2 = (Φeq(y2 + eps, y1, y, x) - φv) / eps;

                for (int i = 0; i < n; i++)
                {
                    double u = (x - a[i]) / b[i];
                    double φu = φ(u);
                    double φ1u = φ1(u);
                    double φ2u = φ2(u);
                    double φ3u = φ3(u);

                    double dF_da = -w[i] * φ1u / b[i];
                    double dF_db = -w[i] * u * φ1u / b[i];
                    double dF_dw = φu;

                    double dF1_da = -w[i] * φ2u / (b[i] * b[i]);
                    double dF1_db = -w[i] / (b[i] * b[i]) * (φ1u + u * φ2u);
                    double dF1_dw = φ1u / b[i];

                    double dF2_da = -w[i] * φ3u / (b[i] * b[i] * b[i]);
                    double dF2_db = -w[i] / (b[i] * b[i] * b[i]) * (2 * φ2u + u * φ3u);
                    double dF2_dw = φ2u / (b[i] * b[i]);

                    double cf = 2 * φv;
                    da[i] += cf * (Φy * dF_da + Φy1 * dF1_da + Φy2 * dF2_da);
                    db[i] += cf * (Φy * dF_db + Φy1 * dF1_db + Φy2 * dF2_db);
                    dw[i] += cf * (Φy * dF_dw + Φy1 * dF1_dw + Φy2 * dF2_dw);
                }
            }

            double wInt = (bDom - aDom) / M;
            for (int i = 0; i < n; i++)
            {
                da[i] *= wInt; db[i] *= wInt; dw[i] *= wInt;
            }

            double bcE_y = Response(c) - y_c;
            double bcE_y1 = Derivative(c) - y1_c;

            for (int i = 0; i < n; i++)
            {
                double u_c = (c - a[i]) / b[i];
                double φu = φ(u_c);
                double φ1u = φ1(u_c);
                double φ2u = φ2(u_c);

                double dF_da = -w[i] * φ1u / b[i];
                double dF_db = -w[i] * u_c * φ1u / b[i];
                double dF_dw = φu;

                double dF1_da = -w[i] * φ2u / (b[i] * b[i]);
                double dF1_db = -w[i] / (b[i] * b[i]) * (φ1u + u_c * φ2u);
                double dF1_dw = φ1u / b[i];

                da[i] += 2 * (alpha * bcE_y * dF_da + beta * bcE_y1 * dF1_da);
                db[i] += 2 * (alpha * bcE_y * dF_db + beta * bcE_y1 * dF1_db);
                dw[i] += 2 * (alpha * bcE_y * dF_dw + beta * bcE_y1 * dF1_dw);
            }

            for (int i = 0; i < n; i++)
            {
                a[i] -= learningRate * da[i];
                b[i] -= learningRate * db[i];
                w[i] -= learningRate * dw[i];
                if (b[i] <= 1e-9) b[i] = 1e-9;
            }

            costHistory.Add(ComputeCostDifferential(Φeq, aDom, bDom, c, y_c, y1_c, alpha, beta));
        }

        using (var wCost = new StreamWriter(costFilename))
        {
            for (int i = 0; i < costHistory.Count; i++)
                wCost.WriteLine($"{i} {costHistory[i]}");
        }
    }
}