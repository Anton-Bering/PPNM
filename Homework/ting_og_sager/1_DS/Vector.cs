using System;
using System.Text;

public class vector {
    public double[] data;
    public int size => data.Length;

    public vector(int n) {
        data = new double[n];
    }

    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }

    public vector copy() {
        vector copy = new vector(size);
        for (int i = 0; i < size; i++)
            copy[i] = this[i];
        return copy;
    }

    public bool isApprox(vector other, double tol = 1e-9) {
        if (size != other.size)
            return false;
        for (int i = 0; i < size; i++)
            if (Math.Abs(this[i] - other[i]) > tol)
                return false;
        return true;
    }

    public string ToStringFormatted() {
        StringBuilder sb = new StringBuilder();
        foreach (double val in data) {
            string s = (Math.Abs(val) < 1e-4 && val != 0) ? 
                val.ToString("0.000e00") : val.ToString("0.000");
            sb.Append($"{s,10} ");
        }
        return sb.ToString();
    }
}