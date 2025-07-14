using System;
public static class LinearSpline {
    // Binary search to find interval index for z
    public static int binsearch(double[] x, double z) {
        if(z < x[0] || z > x[x.Length - 1])
            throw new Exception("binsearch: bad z");
        int i = 0, j = x.Length - 1;
        while(j - i > 1) {
            int mid = (i + j) / 2;
            if(z > x[mid])
                i = mid;
            else
                j = mid;
        }
        return i;
    }

    // Linear spline interpolation at point z
    public static double linterp(double[] x, double[] y, double z) {
        int i = binsearch(x, z);
        double dx = x[i+1] - x[i];
        if(!(dx > 0)) throw new Exception("LinearSpline.linterp: dx=0");
        double dy = y[i+1] - y[i];
        return y[i] + dy/dx * (z - x[i]);
    }

    // Integral of the linear spline from x[0] up to z
    public static double linterpInteg(double[] x, double[] y, double z) {
        int i = binsearch(x, z);
        double sum = 0.0;
        // sum areas of fully covered intervals
        for(int j = 0; j < i; j++) {
            double dx = x[j+1] - x[j];
            double avg_y = (y[j] + y[j+1]) / 2.0;
            sum += avg_y * dx;
        }
        // add partial area from x[i] to z
        double dx_i = z - x[i];
        double slope = (y[i+1] - y[i]) / (x[i+1] - x[i]);
        double y_z = y[i] + slope * dx_i;
        sum += (y[i] + y_z) / 2.0 * dx_i;
        return sum;
    }
}
