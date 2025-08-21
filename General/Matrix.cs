using System;
using System.Text;

public sealed class matrix {
    private readonly double[] data;
    public int Rows { get; }
    public int Cols { get; }

    // --- Constructors ---
    public matrix(int rows, int cols) {
        if (rows < 0 || cols < 0) throw new ArgumentOutOfRangeException("rows/cols must be non-negative");
        Rows = rows; Cols = cols;
        data = new double[rows * cols];
    }

    public matrix(double[,] init) {
        if (init == null) throw new ArgumentNullException(nameof(init));
        Rows = init.GetLength(0);
        Cols = init.GetLength(1);
        data = new double[Rows * Cols];
        int k = 0;
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                data[k++] = init[r, c];
    }

    // --- Indexer (bounds-checked) ---
    public double this[int r, int c] {
        get {
            if ((uint)r >= (uint)Rows || (uint)c >= (uint)Cols)
                throw new IndexOutOfRangeException($"matrix index ({r},{c}) out of [0,{Rows})x[0,{Cols})");
            return data[r * Cols + c];
        }
        set {
            if ((uint)r >= (uint)Rows || (uint)c >= (uint)Cols)
                throw new IndexOutOfRangeException($"matrix index ({r},{c}) out of [0,{Rows})x[0,{Cols})");
            data[r * Cols + c] = value;
        }
    }

    // --- Copy ---
    public matrix Copy() {
        var m = new matrix(Rows, Cols);
        Array.Copy(data, m.data, data.Length);
        return m;
    }

    // --- Identity ---
    public static matrix Eye(int n) {
        var I = new matrix(n, n);
        for (int i = 0; i < n; i++) I[i, i] = 1.0;
        return I;
    }

    // --- Transpose ---
    public matrix Transpose() {
        var T = new matrix(Cols, Rows);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                T[c, r] = this[r, c];
        return T;
    }

    // --- Column/Row get/set ---
    public vector GetCol(int c) {
        if ((uint)c >= (uint)Cols) throw new IndexOutOfRangeException(nameof(c));
        var v = new vector(Rows);
        for (int r = 0; r < Rows; r++) v[r] = this[r, c];
        return v;
    }

    public void SetCol(int c, vector v) {
        if ((uint)c >= (uint)Cols) throw new IndexOutOfRangeException(nameof(c));
        if (v == null || v.Size != Rows) throw new ArgumentException("column size mismatch");
        for (int r = 0; r < Rows; r++) this[r, c] = v[r];
    }

    public vector GetRow(int r) {
        if ((uint)r >= (uint)Rows) throw new IndexOutOfRangeException(nameof(r));
        var v = new vector(Cols);
        for (int c = 0; c < Cols; c++) v[c] = this[r, c];
        return v;
    }

    public void SetRow(int r, vector v) {
        if ((uint)r >= (uint)Rows) throw new IndexOutOfRangeException(nameof(r));
        if (v == null || v.Size != Cols) throw new ArgumentException("row size mismatch");
        for (int c = 0; c < Cols; c++) this[r, c] = v[c];
    }

    // --- Products ---
    public static vector operator *(matrix A, vector x) {
        if (A.Cols != x.Size) throw new ArgumentException("A.Cols must equal x.Size");
        var y = new vector(A.Rows);
        for (int r = 0; r < A.Rows; r++) {
            double s = 0.0;
            int baseIdx = r * A.Cols;
            for (int c = 0; c < A.Cols; c++) s += A.data[baseIdx + c] * x[c];
            y[r] = s;
        }
        return y;
    }

    public static matrix operator *(matrix A, matrix B) {
        if (A.Cols != B.Rows) throw new ArgumentException("A.Cols must equal B.Rows");
        var C = new matrix(A.Rows, B.Cols);
        // i-k-j loop: god cacheadfærd (row-major A, B læses pr. kolonne)
        for (int i = 0; i < A.Rows; i++) {
            for (int k = 0; k < A.Cols; k++) {
                double aik = A[i, k];
                int bBase = k * B.Cols;
                int cBase = i * C.Cols;
                for (int j = 0; j < B.Cols; j++) {
                    C.data[cBase + j] += aik * B.data[bBase + j];
                }
            }
        }
        return C;
    }

    public static matrix operator *(matrix A, double c) {
        var R = new matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.data.Length; i++) R.data[i] = A.data[i] * c;
        return R;
    }
    public static matrix operator *(double c, matrix A) => A * c;

    public static matrix operator /(matrix A, double c) {
        if (c == 0.0) throw new DivideByZeroException();
        var R = new matrix(A.Rows, A.Cols);
        double inv = 1.0 / c;
        for (int i = 0; i < A.data.Length; i++) R.data[i] = A.data[i] * inv;
        return R;
    }

    // --- Addition ---
    public static matrix operator +(matrix A, matrix B) {
        if (A.Rows != B.Rows || A.Cols != B.Cols) throw new ArgumentException("Matrices must have the same dimensions for addition");
        var R = new matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.data.Length; i++) R.data[i] = A.data[i] + B.data[i];
        return R;
    }

    // --- Subtraction ---
    public static matrix operator -(matrix A, matrix B) {
        if (A.Rows != B.Rows || A.Cols != B.Cols) throw new ArgumentException("Matrices must have the same dimensions for subtraction");
        var R = new matrix(A.Rows, A.Cols);
        for (int i = 0; i < A.data.Length; i++) R.data[i] = A.data[i] - B.data[i];
        return R;
    }

    // --- Outer product: u v^T ---
    public static matrix Outer(vector u, vector v) {
        var M = new matrix(u.Size, v.Size);
        for (int i = 0; i < u.Size; i++) {
            double ui = u[i];
            int rowBase = i * M.Cols;
            for (int j = 0; j < v.Size; j++) M.data[rowBase + j] = ui * v[j];
        }
        return M;
    }

    // --- Back-substitution: Rx = y, R øvre trekantet ---
    public static vector BackSubstitute(matrix R, vector y, double eps = 1e-12) {
        if (R == null || y == null) throw new ArgumentNullException();
        if (R.Rows != R.Cols) throw new ArgumentException("R must be square");
        if (R.Rows != y.Size) throw new ArgumentException("dimension mismatch in back-substitution");

        int n = R.Rows;
        var x = new vector(n);
        for (int i = n - 1; i >= 0; i--) {
            double s = y[i];
            for (int j = i + 1; j < n; j++) s -= R[i, j] * x[j];
            double rii = R[i, i];
            if (Math.Abs(rii) < eps)
                throw new InvalidOperationException($"Near-singular R on diagonal at {i}: {rii}");
            x[i] = s / rii;
        }
        return x;
    }

    // --- Formatting ---
    public override string ToString() => ToString("G6", 0);
    public string ToString(string format, int cols = 0) {
        var sb = new StringBuilder();
        for (int r = 0; r < Rows; r++) {
            sb.Append("[ ");
            for (int c = 0; c < Cols; c++) {
                if (cols > 0) sb.AppendFormat("{0," + cols + ":" + format + "}", this[r, c]);
                else sb.AppendFormat("{0:" + format + "}", this[r, c]);
                if (c + 1 < Cols) sb.Append(' ');
            }
            sb.Append(" ]");
            if (r + 1 < Rows) sb.AppendLine();
        }
        return sb.ToString();
    }
}
