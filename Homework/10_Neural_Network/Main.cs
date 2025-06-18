
using System;
using System.IO;
using System.Text;

public class Program
{
    /* -------------- helper: tee Console‑output to Out.txt -------------- */
    private sealed class TeeWriter : TextWriter
    {
        private readonly TextWriter console;
        private readonly StreamWriter file;
        public TeeWriter(string path)
        {
            console = Console.Out;
            file = new StreamWriter(path) { AutoFlush = true };
        }
        public override Encoding Encoding => console.Encoding;
        public override void Write(char value)      { console.Write(value); file.Write(value); }
        public override void Write(string value)    { console.Write(value); file.Write(value); }
        public override void WriteLine(string value){ console.WriteLine(value); file.WriteLine(value); }
        protected override void Dispose(bool d)
        { if (d) file.Dispose(); base.Dispose(d); }
    }

    // To include ∫dx g_e(x):
    static double IntegralOfG(double x)
    {
        // Numerisk integration af g(x) fra -1 til x ved trapezmetoden
        int N = 1000;
        double a = -1.0, b = x;
        double h = (b - a) / N;
        double sum = 0.5 * (Math.Cos(5 * a - 1) * Math.Exp(-a * a) +
                            Math.Cos(5 * b - 1) * Math.Exp(-b * b));
        for (int i = 1; i < N; i++)
        {
            double xi = a + i * h;
            sum += Math.Cos(5 * xi - 1) * Math.Exp(-xi * xi);
        }
        return sum * h;
    }



    public static void Main(string[] args)
    {
        /* ----- redirect alt Console‑output til både skærm & Out.txt ----- */
        using (var tee = new TeeWriter("Out.txt"))
        {
            Console.SetOut(tee);

            /* ------------  PART A ------------ */
            Console.WriteLine("------------ TASK A: Function Approximation ------------\n");
            Console.WriteLine("------ Train the network to approximate some function ------");
            Console.WriteLine("------ And sampled at several points on [-1,1] ------\n");

            Func<double,double> g = x => Math.Cos(5 * x - 1) * Math.Exp(-x * x);
            double x_min = -1.0, x_max = 1.0;
            int N = 101;
            double[] xs = new double[N], ys = new double[N];
            for (int k = 0; k < N; k++)
            {
                double x = x_min + (x_max - x_min) * k / (N - 1);
                xs[k] = x;
                ys[k] = g(x);
            }

            
            NeuralNetwork net1 = new NeuralNetwork(20, x_min, x_max);

            double cost0 = net1.ComputeCost(xs, ys);
            net1.Train(xs, ys, epochs:10_000, learningRate:0.01);
            double cost1 = net1.ComputeCost(xs, ys);

            Console.WriteLine("--- Function to approximate: g(x)=Cos(5*x-1)*Exp(-x*x) ---\n");
            Console.WriteLine($"Initial cost = {cost0:F6}, Final cost = {cost1:F6}");

            Console.WriteLine("\nNetwerk (g_n) vs exact value (g_e):");
            Console.WriteLine("  x:         g_n(x):        g_e(x):      error:");
            int[] idx = { 0, N/4, N/2, 3*N/4, N-1 };
            foreach (int k in idx)
            {
                double x  = xs[k];
                double F  = net1.Response(x);
                double gg = ys[k];
                Console.WriteLine($"{x,6:F2} {F,14:F6} {gg,14:F6} {F-gg,12:F6}");
            }

            using (var w = new StreamWriter("function_g.txt"))
            {
                int M = 500;
                for (int i = 0; i < M; i++)
                {
                    double x = x_min + (x_max - x_min) * i / (M - 1);
                    w.WriteLine($"{x} {g(x)} {net1.Response(x)}");
                }
            }
            Console.WriteLine("\nData saved as function_g.txt");
            Console.WriteLine("Plot saved as function_g.svg");

            /* ------------  PART B ------------ */
            Console.WriteLine("\n------------ TASK B: Derivatives and Anti-derivative ------------\n");
            Console.WriteLine("------ Train the network to give first and second derivatives ------");
            Console.WriteLine("------ And also the anti-derivative ------\n");

            Func<double,double> g1 = x => Math.Exp(-x * x) *
                                          (-5 * Math.Sin(5 * x - 1) - 2 * x * Math.Cos(5 * x - 1));
            Func<double,double> g2 = x => Math.Exp(-x * x) *
                                          ((4 * x * x - 27) * Math.Cos(5 * x - 1) + 20 * x * Math.Sin(5 * x - 1));

            double[] Bx = { -1.0, -0.5, 0.0, 0.5, 1.0 };

            Console.WriteLine("Netwerk (g_n') vs exact value (g_e'):");
            Console.WriteLine("  x:         g_n'(x):       g_e'(x):     error:");
            foreach (double x in Bx)
            {
                double gn1 = net1.Derivative(x);
                double ge1 = g1(x);
                Console.WriteLine($"{x,6:F2} {gn1,14:F6} {ge1,14:F6} {gn1 - ge1,10:F6}");
            }

            Console.WriteLine("\nNetwerk (g_n'') vs exact value (g_e''):");
            Console.WriteLine("  x:         g_n''(x):      g_e''(x):     error:");
            foreach (double x in Bx)
            {
                double gn2 = net1.SecondDerivative(x);
                double ge2 = g2(x);
                Console.WriteLine($"{x,6:F2} {gn2,14:F6} {ge2,14:F6} {gn2 - ge2,10:F6}");
            }

            double shift = IntegralOfG(-1.0) - net1.AntiDerivative(-1.0);
            Console.WriteLine("\nNetwerk (∫dx g_n) vs exact value (∫dx g_e ≈ numerically integrated):");
            Console.WriteLine("  x:         ∫dx g_n(x):        ∫dx g_e(x):      error:");
            foreach (double x in Bx)
            {
                double An = net1.AntiDerivative(x) + shift;
                double Ae = IntegralOfG(x);
                Console.WriteLine($"{x,6:F2} {An,18:F6} {Ae,18:F6} {An - Ae,10:F6}");
            }


            using (var w = new StreamWriter("derivatives_and_anti_derivative_g.txt"))
            {
                int M = 500;
                for (int i = 0; i < M; i++)
                {
                    double x   = x_min + (x_max - x_min) * i / (M - 1);
                    
                    double An = net1.AntiDerivative(x) + shift;
                    double Ae = IntegralOfG(x);
                    w.WriteLine($"{x} {g(x)} {net1.Response(x)} {g1(x)} {net1.Derivative(x)} " +
                                $"{g2(x)} {net1.SecondDerivative(x)} {An} {Ae}");

                }
            }
            Console.WriteLine("\nData saved as derivatives_and_anti_derivative_g.txt");
            Console.WriteLine("Plot saved as first_derivatives_g.svg");
            Console.WriteLine("Plot saved as second_derivatives_g.svg");
            Console.WriteLine("Plot saved as anti_derivative.svg");

            /* ------------  PART C ------------ */
            Console.WriteLine("\n------------ TASK C: Differential Equation Solution Approximation ------------\n");

            Console.WriteLine("------ Solving y'' + y = 0 ------\n");

            Func<double, double, double, double, double> phi1 = (y2, y1, y, x) => y2 + y;
            double aDom = 0.0, bDom = Math.PI;
            double cPoint = 0.0, y_c = 0.0, yprime_c = 1.0;

            NeuralNetwork net2 = new NeuralNetwork(20, aDom, bDom);

            double c0_1 = net2.ComputeCostDifferential(phi1, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100, beta: 100);
            net2.TrainDifferentialEquation(phi1, aDom, bDom, cPoint, y_c, yprime_c, 100, 100, 15_000, 0.005, "differential_equation_cost_phi1.txt");
            double c1_1 = net2.ComputeCostDifferential(phi1, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100, beta: 100);

            Console.WriteLine($"Initial cost = {c0_1:F6}, Final cost = {c1_1:F6}");
            Console.WriteLine($"Boundary check: Φ_n(0) = {net2.Response(0):F6} (target 0.00), Φ_n'(0) = {net2.Derivative(0):F6} (target 1.00)");

            Console.WriteLine("Sample solution values vs sin(x):");
            Console.WriteLine("    x        Φ_n(x)    Φ_e=sin(x)   error");
            double[] sX = { 0.0, Math.PI / 6, Math.PI / 4, Math.PI / 2, Math.PI };
            foreach (double x in sX)
            {
                double F = net2.Response(x);
                double s = Math.Sin(x);
                Console.WriteLine($"{x,6:F2} {F,14:F6} {s,16:F6} {F - s,11:F6}");
            }

            using (var wSol = new StreamWriter("differential_equation_phi1.txt"))
            using (var wRes = new StreamWriter("differential_equation_residual_phi1.txt"))
            {
                int M = 500;
                for (int i = 0; i < M; i++)
                {
                    double x = aDom + (bDom - aDom) * i / (M - 1);
                    double F = net2.Response(x);
                    double F1 = net2.Derivative(x);
                    double s = Math.Sin(x);
                    double s1 = Math.Cos(x);
                    wSol.WriteLine($"{x} {s} {F} {s1} {F1}");

                    double res = phi1(net2.SecondDerivative(x), F1, F, x);
                    wRes.WriteLine($"{x} {res}");
                }
            }
            Console.WriteLine("\nData saved as differential_equation_phi1.txt");
            Console.WriteLine("Data saved as differential_equation_residual_phi1.txt");

            // ----------------------- TASK C.2 -----------------------

            Console.WriteLine("\n------ Solving y'' + sin(x)y' + cos(x)y = x ------\n");

            Func<double, double, double, double, double> phi2 = (y2, y1, y, x) => y2 + Math.Sin(x) * y1 + Math.Cos(x) * y - x;

            NeuralNetwork net3 = new NeuralNetwork(20, aDom, bDom);

            double c0_2 = net3.ComputeCostDifferential(phi2, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100, beta: 100);
            net3.TrainDifferentialEquation(phi2, aDom, bDom, cPoint, y_c, yprime_c, 100, 100, 15_000, 0.005, "differential_equation_cost_phi2.txt");
            double c1_2 = net3.ComputeCostDifferential(phi2, aDom, bDom, cPoint, y_c, yprime_c, alpha: 100, beta: 100);

            Console.WriteLine($"Initial cost = {c0_2:F6}, Final cost = {c1_2:F6}");
            Console.WriteLine($"Boundary check: Φ_n(0) = {net3.Response(0):F6} (target 0.00), Φ_n'(0) = {net3.Derivative(0):F6} (target 1.00)");

            Console.WriteLine("Sample solution values:");
            Console.WriteLine("    x        Φ_n(x)");
            foreach (double x in sX)
            {
                double F = net3.Response(x);
                Console.WriteLine($"{x,6:F2} {F,14:F6}");
            }

            using (var wSol2 = new StreamWriter("differential_equation_phi2.txt"))
            using (var wRes2 = new StreamWriter("differential_equation_residual_phi2.txt"))
            {
                int M = 500;
                for (int i = 0; i < M; i++)
                {
                    double x = aDom + (bDom - aDom) * i / (M - 1);
                    double F = net3.Response(x);
                    double F1 = net3.Derivative(x);
                    wSol2.WriteLine($"{x} {F} {F1}");

                    double res = phi2(net3.SecondDerivative(x), F1, F, x);
                    wRes2.WriteLine($"{x} {res}");
                }
            }
            Console.WriteLine("\nData saved as differential_equation_phi2.txt");
            Console.WriteLine("Data saved as differential_equation_residual_phi2.txt");

            

        }   
    }
}
