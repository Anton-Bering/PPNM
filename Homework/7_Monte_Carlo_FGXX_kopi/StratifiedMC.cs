using System;
using static System.Math;

public static class StratifiedMC
{
    private const int nmin = 16;
    private static Random rng = new Random();

    public static (double result, double error) Integrate(Func<double[], double> f, double[] a, double[] b, int N)
    {
        int dim = a.Length;
        if (N < nmin)
        {
            return PlainMC.Integrate(f, a, b, N);
        }

        int M = Math.Min(N, nmin);
        double[] leftSum = new double[dim], rightSum = new double[dim];
        double[] leftSum2 = new double[dim], rightSum2 = new double[dim];
        int[] leftCount = new int[dim], rightCount = new int[dim];

        double[] x = new double[dim];
        for (int j = 0; j < M; j++)
        {
            for (int k = 0; k < dim; k++)
            {
                double u = rng.NextDouble();
                x[k] = a[k] + u * (b[k] - a[k]);
            }
            double fx = f(x);
            for (int i = 0; i < dim; i++)
            {
                double mid = 0.5 * (a[i] + b[i]);
                if (x[i] <= mid)
                {
                    leftSum[i]  += fx;
                    leftSum2[i] += fx * fx;
                    leftCount[i]++;
                }
                else
                {
                    rightSum[i]  += fx;
                    rightSum2[i] += fx * fx;
                    rightCount[i]++;
                }
            }
        }

        int splitDim = -1;
        double minCombinedVar = double.PositiveInfinity;
        for (int i = 0; i < dim; i++)
        {
            if (leftCount[i] == 0 || rightCount[i] == 0) continue;

            double leftMean = leftSum[i] / leftCount[i];
            double rightMean = rightSum[i] / rightCount[i];
            double leftVar = leftSum2[i] / leftCount[i] - leftMean * leftMean;
            double rightVar = rightSum2[i] / rightCount[i] - rightMean * rightMean;
            double sigmaLeft = Sqrt(Max(0.0, leftVar));
            double sigmaRight = Sqrt(Max(0.0, rightVar));
            double combinedVar = Pow(sigmaLeft + sigmaRight, 2);
            if (combinedVar < minCombinedVar)
            {
                minCombinedVar = combinedVar;
                splitDim = i;
            }
        }

        if (splitDim == -1)
        {
            return PlainMC.Integrate(f, a, b, N);
        }

        double splitMid = 0.5 * (a[splitDim] + b[splitDim]);
        double[] a_left = (double[])a.Clone(), b_left = (double[])b.Clone();
        double[] a_right = (double[])a.Clone(), b_right = (double[])b.Clone();
        b_left[splitDim] = splitMid;
        a_right[splitDim] = splitMid;

        double muLeft = (leftCount[splitDim] > 0) ? leftSum[splitDim] / leftCount[splitDim] : 0.0;
        double muRight = (rightCount[splitDim] > 0) ? rightSum[splitDim] / rightCount[splitDim] : 0.0;
        double varLeft = (leftCount[splitDim] > 0) ? leftSum2[splitDim] / leftCount[splitDim] - muLeft * muLeft : 0.0;
        double varRight = (rightCount[splitDim] > 0) ? rightSum2[splitDim] / rightCount[splitDim] - muRight * muRight : 0.0;
        double sigmaLeftEst = Sqrt(Max(0.0, varLeft));
        double sigmaRightEst = Sqrt(Max(0.0, varRight));

        int N_rem = N - M;
        if (N_rem < 2) return PlainMC.Integrate(f, a, b, N);

        double totalSigma = sigmaLeftEst + sigmaRightEst;
        double fracLeft = (totalSigma > 0) ? (sigmaLeftEst / totalSigma) : 0.5;
        int N_left = (int)Round(N_rem * fracLeft);
        if (N_left < 1) N_left = 1;
        if (N_left > N_rem - 1) N_left = N_rem - 1;
        int N_right = N_rem - N_left;
        if (N_right < 1) { N_right = 1; N_left = N_rem - 1; }

        var (resLeft, errLeft) = Integrate(f, a_left, b_left, N_left);
        var (resRight, errRight) = Integrate(f, a_right, b_right, N_right);
        double result = resLeft + resRight;
        double error = Sqrt(errLeft * errLeft + errRight * errRight);
        return (result, error);
    }
}
