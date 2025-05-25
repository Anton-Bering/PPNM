using System;

// Vector class representing a one-dimensional array of doubles
public class Vector
{
    // Internal storage of vector elements
    private readonly double[] data;

    // Property to get the length of the vector
    public int Length => data.Length;

    // Constructor to initialize the vector with a given size
    public Vector(int size)
    {
        if (size <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
        data = new double[size];
    }

    // Indexer to access or modify elements using [i] syntax
    public double this[int i]
    {
        get => data[i];
        set => data[i] = value;
    }

    // Returns the Euclidean norm (magnitude) of the vector
    public double Norm()
    {
        double sum = 0;
        foreach (var val in data)
            sum += val * val;
        return Math.Sqrt(sum);
    }

    // Computes the dot product between this vector and another
    public double Dot(Vector other)
    {
        if (other.Length != Length)
            throw new ArgumentException("Vectors must have the same length.");
        
        double result = 0;
        for (int i = 0; i < Length; i++)
            result += this[i] * other[i];
        return result;
    }

    // Fills all elements of the vector with the specified value
    public void Fill(double value)
    {
        for (int i = 0; i < Length; i++)
            data[i] = value;
    }

    // Returns a deep copy of the vector
    public Vector Copy()
    {
        var result = new Vector(Length);
        for (int i = 0; i < Length; i++)
            result[i] = this[i];
        return result;
    }

    // Prints the contents of the vector with a label
    public void Print(string label)
    {
        Console.WriteLine($"{label} [{string.Join(", ", data)}]");
    }

    // Adds two vectors element-wise
    public static Vector operator +(Vector a, Vector b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must have the same length.");
        
        var result = new Vector(a.Length);
        for (int i = 0; i < a.Length; i++)
            result[i] = a[i] + b[i];
        return result;
    }

    // Subtracts one vector from another element-wise
    public static Vector operator -(Vector a, Vector b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must have the same length.");
        
        var result = new Vector(a.Length);
        for (int i = 0; i < a.Length; i++)
            result[i] = a[i] - b[i];
        return result;
    }

    // Multiplies each element of the vector by a scalar
    public static Vector operator *(Vector v, double d)
    {
        var result = new Vector(v.Length);
        for (int i = 0; i < v.Length; i++)
            result[i] = v[i] * d;
        return result;
    }

    // Divides each element of the vector by a scalar
    public static Vector operator /(Vector v, double d)
    {
        if (d == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        
        var result = new Vector(v.Length);
        for (int i = 0; i < v.Length; i++)
            result[i] = v[i] / d;
        return result;
    }

    // Compares two vectors element-wise using a tolerance
    public static bool Compare(Vector a, Vector b, double tol = 1e-10)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
            if (Math.Abs(a[i] - b[i]) > tol)
                return false;
        return true;
    }
}
