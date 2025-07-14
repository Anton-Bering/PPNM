public class QRGS {
    public matrix Q, R;

    public QRGS(matrix A) {
        int m = A.size2;
        Q = A.copy();
        R = new matrix(m, m);
        for (int i = 0; i < m; i++) {
            R[i, i] = Q.get_column(i).norm();
            Q.set_column(i, Q.get_column(i) / R[i, i]);
            for (int j = i + 1; j < m; j++) {
                R[i, j] = Q.get_column(i).dot(Q.get_column(j));
                Q.set_column(j, Q.get_column(j) - Q.get_column(i) * R[i, j]);
            }
        }
    }

    public vector solve(vector b) {
        vector x = Q.transpose() * b;
        for (int i = x.size - 1; i >= 0; i--) {
            double sum = 0;
            for (int j = i + 1; j < x.size; j++) {
                sum += R[i, j] * x[j];
            }
            x[i] = (x[i] - sum) / R[i, i];
        }
        return x;
    }
}

public partial class matrix {
    public matrix copy() {
        matrix B = new matrix(size1, size2);
        for (int i = 0; i < size1; i++)
            for (int j = 0; j < size2; j++)
                B[i, j] = this[i, j];
        return B;
    }

    public vector get_column(int j) {
        vector col = new vector(size1);
        for (int i = 0; i < size1; i++) col[i] = this[i, j];
        return col;
    }

    public void set_column(int j, vector col) {
        for (int i = 0; i < size1; i++) this[i, j] = col[i];
    }
}
