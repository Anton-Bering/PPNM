using static System.Console;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

public class data {
    public int a, b;
    public double sum;
}

class main {
    public static void harm(object obj) {
        var arg = (data)obj;
        arg.sum = 0;
        for (int i = arg.a; i < arg.b; i++) arg.sum += 1.0 / i;
    }

    static int Main(string[] args) {
        int nthreads = 1, nterms = (int)1e8;
        foreach (var arg in args) {
            var words = arg.Split(':');
            if (words[0] == "-threads") nthreads = int.Parse(words[1]);
            if (words[0] == "-terms") nterms = (int)float.Parse(words[1]);
        }

        // Manuel
        var parameters = new data[nthreads];
        for (int i = 0; i < nthreads; i++) {
            parameters[i] = new data();
            parameters[i].a = 1 + nterms / nthreads * i;
            parameters[i].b = 1 + nterms / nthreads * (i + 1);
        }
        parameters[nthreads - 1].b = nterms + 1;

        var threads = new Thread[nthreads];
        for (int i = 0; i < nthreads; i++) {
            threads[i] = new Thread(harm);
            threads[i].Start(parameters[i]);
        }

        foreach (var thread in threads) thread.Join();

        double total = 0;
        foreach (var p in parameters) total += p.sum;

        WriteLine($"Manual threading with {nthreads} thread(s): harmonic sum = {total}");

        // Broken parallel methoden
        double brokenSum = 0;
        Parallel.For(1, nterms + 1, i => brokenSum += 1.0 / i);
        WriteLine($"Broken Parallel.For sum: {brokenSum}");

        // Thread-local corect sum
        var local = new ThreadLocal<double>(() => 0.0, true);
        Parallel.For(1, nterms + 1, i => local.Value += 1.0 / i);
        double correct = local.Values.Sum();
        WriteLine($"ThreadLocal Parallel.For sum = {correct}");

        return 0;
    }
}
