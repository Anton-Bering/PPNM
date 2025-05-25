using System;
public class vector {
    public double[] data;
    public int size {
        get { return data.Length; }
    }

    public double this[int i] {
        get { return data[i]; }
        set { data[i] = value; }
    }

    public vector(int n) {
        data = new double[n];
    }

    // (Optional) Compute dot product of two vectors
    public static double Dot(vector a, vector b) {
        if (a.size != b.size) throw new ArgumentException("Vector sizes do not match");
        double sum = 0;
        for (int i = 0; i < a.size; i++) {
            sum += a[i] * b[i];
        }
        return sum;
    }
}
