using static System.Console;
using static System.Math;
using System;

class main {
    static int Main(string[] args) {
        
        bool handled = false;

        foreach (var arg in args) {
            var words = arg.Split(':');
            if (words[0] == "-numbers" && words.Length == 2) {
                var numbers = words[1].Split(',');
                foreach (var number in numbers) {
                    double x = double.Parse(number);
                    WriteLine($"{x} {Sin(x)} {Cos(x)}");
                }
                handled = true;
            }
            if (words[0] == "mode" && words[1] == "stdin") {
                HandleStdin();
                return 0;
            }
        }

        // Hvis ikke handled og contains -input og -output
        string infile = null, outfile = null;
        foreach (var arg in args) {
            var words = arg.Split(':');
            if (words[0] == "-input") infile = words[1];
            if (words[0] == "-output") outfile = words[1];
        }
        if (infile != null && outfile != null) {
            var instream = new System.IO.StreamReader(infile);
            var outstream = new System.IO.StreamWriter(outfile, append: false);
            for (string line = instream.ReadLine(); line != null; line = instream.ReadLine()) {
                double x = double.Parse(line);
                outstream.WriteLine($"{x} {Sin(x)} {Cos(x)}");
            }
            instream.Close();
            outstream.Close();
            return 0;
        }

        if (!handled) {
            Error.WriteLine("Wrong or missing arguments!");
            return 1;
        }

        return 0;
    }

    static void HandleStdin() {
        char[] split_delimiters = { ' ', '\t', '\n' };
        var split_options = StringSplitOptions.RemoveEmptyEntries;
        for (string line = ReadLine(); line != null; line = ReadLine()) {
            var numbers = line.Split(split_delimiters, split_options);
            foreach (var number in numbers) {
                double x = double.Parse(number);
                Error.WriteLine($"{x} {Sin(x)} {Cos(x)}");
            }
        }
    }
}
