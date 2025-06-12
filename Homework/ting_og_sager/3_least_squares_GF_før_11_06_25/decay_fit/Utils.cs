using System;

public static class Utils {
    // Compute an array of natural logarithms for each element (for transforming y -> ln y)
    public static double[] LogArray(double[] values) {
        double[] logVals = new double[values.Length];
        for (int i = 0; i < values.Length; i++) {
            logVals[i] = Math.Log(values[i]);
        }
        return logVals;
    }

    // Compute uncertainties of log(y) given y and δy: δln(y) = δy / y
    public static double[] LogErrorArray(double[] y, double[] dy) {
        double[] dLog = new double[y.Length];
        for (int i = 0; i < y.Length; i++) {
            dLog[i] = dy[i] / y[i];
        }
        return dLog;
    }

    // Compute half-life and its uncertainty from λ and δλ (propagation: δT = |d(T)/dλ| * δλ = (ln2/λ^2)*δλ)
    public static (double halfLife, double uncertainty) HalfLifeFromLambda(double lambda, double sigmaLambda) {
        double half = Math.Log(2) / lambda;
        double sigmaHalf = Math.Abs(Math.Log(2) / (lambda * lambda)) * sigmaLambda;
        return (half, sigmaHalf);
    }
}
