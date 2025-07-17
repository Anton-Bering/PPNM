using System;
using System.Globalization;
using System.IO;
using System.Linq;

public static class Utils
{
    /* -------------------------------------------------------------
     *  Write a matrix in a neat, readable format to a StreamWriter
     * ------------------------------------------------------------- */

    /* HERHER: OBS: Taget ud
    public static void WriteMatrix(StreamWriter sw, string caption,
                                   double[,] M)
    {
        sw.WriteLine(caption);
        sw.WriteLine(VectorAndMatrix.PrintMatrix(M));
    }
    */

    public static void WriteTest(StreamWriter sw,
                                 string description, bool ok)
        => sw.WriteLine($"TEST: {description}\nRESULT: {(ok ? "Yes" : "No")}\n");

    /* -------------------------------------------------------------
     *  Read column ‘col’ (0‑based) from a whitespace‑separated text
     *  file, ignoring blank lines and lines starting with ‘#’.
     * ------------------------------------------------------------- */
    public static double[] ReadColumn(string path, int col)
    {
        var goodLines = File.ReadLines(path)
                            .Where(l =>
                            {
                                var s = l.Trim();
                                return s.Length != 0 && !s.StartsWith("#");
                            })
                            .ToList();

        var v = new double[goodLines.Count];
        int i = 0;

        foreach (var line in goodLines)
        {
            string[] tok = line.Split((char[])null,
                                      StringSplitOptions.RemoveEmptyEntries);
            if (col >= tok.Length)
                throw new FormatException($"Line lacks column {col}: {line}");

            v[i++] = double.Parse(tok[col], CultureInfo.InvariantCulture);
        }
        return v;
    }
}
