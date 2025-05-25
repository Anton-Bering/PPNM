public class vector {
    public double[] data;
    public int size => data.Length;
    // Indexer for accessing elements
    public double this[int i] {
        get => data[i];
        set => data[i] = value;
    }
    // Constructor with given size (initializes all elements to 0)
    public vector(int n) {
        data = new double[n];
    }
    // Constructor from an array of doubles
    public vector(double[] input) {
        data = new double[input.Length];
        input.CopyTo(data, 0);
    }
    // Create a deep copy of the vector
    public vector copy() {
        return new vector((double[])data.Clone());
    }
    // Euclidean norm (length) of the vector
    public double norm() {
        double sum = 0;
        for(int i = 0; i < data.Length; i++) {
            sum += data[i] * data[i];
        }
        return System.Math.Sqrt(sum);
    }
    // Vector addition
    public static vector operator+(vector u, vector v) {
        int n = u.size;
        vector result = new vector(n);
        for(int i = 0; i < n; i++) {
            result[i] = u[i] + v[i];
        }
        return result;
    }
    // Vector subtraction
    public static vector operator-(vector u, vector v) {
        int n = u.size;
        vector result = new vector(n);
        for(int i = 0; i < n; i++) {
            result[i] = u[i] - v[i];
        }
        return result;
    }
    // Scalar multiplication (vector * double)
    public static vector operator*(vector v, double a) {
        int n = v.size;
        vector result = new vector(n);
        for(int i = 0; i < n; i++) {
            result[i] = v[i] * a;
        }
        return result;
    }
    // Scalar multiplication (double * vector)
    public static vector operator*(double a, vector v) {
        return v * a;
    }
    // Scalar division
    public static vector operator/(vector v, double a) {
        return v * (1.0/a);
    }
}
