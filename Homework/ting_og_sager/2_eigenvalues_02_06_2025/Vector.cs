// Vector.cs
public class vector {
    public int size;
    private double[] data;
    public vector(int n) {
        size = n;
        data = new double[n];
    }
    public double this[int i] {
        get { return data[i]; }
        set { data[i] = value; }
    }
}
