using System;
public static class minimization {
    // Forward finite difference gradient
    public static vector gradientForward(Func<vector,double> f, vector x) {
        int n = x.size;
        vector grad = new vector(n);
        double fx = f(x);
        for(int i = 0; i < n; i++) {
            double xi = x[i];
            double dxi = Math.Abs(xi) * Math.Pow(2, -26);
            if(dxi == 0) dxi = Math.Pow(2, -26);
            x[i] = xi + dxi;
            double fx_i = f(x);
            grad[i] = (fx_i - fx) / dxi;
            x[i] = xi; // restore
        }
        return grad;
    }
    // Central finite difference gradient
    public static vector gradientCentral(Func<vector,double> f, vector x) {
        int n = x.size;
        vector grad = new vector(n);
        for(int i = 0; i < n; i++) {
            double xi = x[i];
            double dxi = Math.Abs(xi) * Math.Pow(2, -26);
            if(dxi == 0) dxi = Math.Pow(2, -26);
            x[i] = xi + dxi;
            double f_plus = f(x);
            x[i] = xi - dxi;
            double f_minus = f(x);
            x[i] = xi; // restore
            grad[i] = (f_plus - f_minus) / (2*dxi);
        }
        return grad;
    }
    // Forward finite difference Hessian (using forward diff gradients)
    public static matrix hessianForward(Func<vector,double> f, vector x) {
        int n = x.size;
        matrix H = new matrix(n, n);
        // gradient at original x
        vector g0 = gradientForward(f, x);
        for(int j = 0; j < n; j++) {
            double xj = x[j];
            double dxj = Math.Abs(xj) * Math.Pow(2, -17);
            if(dxj == 0) dxj = Math.Pow(2, -17);
            x[j] = xj + dxj;
            vector gj = gradientForward(f, x);
            // difference in gradient
            for(int i = 0; i < n; i++) {
                H[i, j] = (gj[i] - g0[i]) / dxj;
            }
            x[j] = xj; // restore
        }
        return H;
    }
    // Central finite difference Hessian (using central diff gradients)
    public static matrix hessianCentral(Func<vector,double> f, vector x) {
        int n = x.size;
        matrix H = new matrix(n, n);
        for(int j = 0; j < n; j++) {
            double xj = x[j];
            double dxj = Math.Abs(xj) * Math.Pow(2, -17);
            if(dxj == 0) dxj = Math.Pow(2, -17);
            x[j] = xj + dxj;
            vector grad_plus = gradientCentral(f, x);
            x[j] = xj - dxj;
            vector grad_minus = gradientCentral(f, x);
            x[j] = xj; // restore
            for(int i = 0; i < n; i++) {
                H[i, j] = (grad_plus[i] - grad_minus[i]) / (2*dxj);
            }
        }
        return H;
    }
    // Newton's method with backtracking line-search
    public static vector newton(Func<vector,double> f, vector start, out int steps, double acc=1e-3, bool useCentral=false, int maxSteps=1000) {
        steps = 0;
        vector x = start.copy();
        while(steps < maxSteps) {
            // compute gradient
            vector g = useCentral ? gradientCentral(f, x) : gradientForward(f, x);
            if(g.norm() < acc) break;
            // compute Hessian
            matrix H = useCentral ? hessianCentral(f, x) : hessianForward(f, x);
            // solve H * dx = -g via QR decomposition
            QR qr = new QR(H);
            vector dx = qr.solve(-g);
            // backtracking line-search
            double fx = f(x);
            double lambda = 1.0;
            while(lambda > 1.0/128) {
                vector x_new = x + lambda * dx;
                double f_new = f(x_new);
                if(f_new < fx) {
                    x = x_new;
                    fx = f_new;
                    break;
                }
                lambda /= 2;
            }
            // if no sufficient decrease found
            if(lambda < 1.0/128 && f(x + lambda*dx) >= fx) {
                // no progress possible (flat or stalled)
                break;
            }
            steps++;
        }
        if(steps == maxSteps) {
            Console.Error.WriteLine("Warning: Newton method did not converge in {0} steps", maxSteps);
        }
        return x;
    }
}
