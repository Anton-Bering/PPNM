using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public static class Utils
{
    /* -------------------------------------------------------------
     *  Skriv en matrix i pænt, læsbart format til en StreamWriter
     * ------------------------------------------------------------- */
    public static void WriteMatrix(StreamWriter sw, string caption, Matrix M)
    {
        sw.WriteLine(caption);
        sw.WriteLine(M.ToPretty());
    }

    public static void WriteTest(StreamWriter sw, string description, bool ok)
        => sw.WriteLine($"TEST: {description}\nRESULT: {(ok ? "Yes" : "No")}\n");

    /* -------------------------------------------------------------
     *  Læs kolonne 'col' (0‑indekseret) fra en ASCII‑fil.
     *  Ignorer:
     *      • tomme linjer
     *      • linjer der starter med  #  (kommentar)
     *  Kolonner separeres af et vilkårligt antal blanks eller tabs.
     * ------------------------------------------------------------- */
    public static Vector ReadColumn(string path, int col)
    {
        var goodLines = File.ReadLines(path)
                            .Where(l =>
                            {
                                var s = l.Trim();
                                return s.Length != 0 && !s.StartsWith("#");
                            })
                            .ToList();

        var v = new Vector(goodLines.Count);
        int i = 0;

        foreach (var line in goodLines)
        {
            string[] tok = line.Split((char[])null,   // splitter på whitespace
                                      StringSplitOptions.RemoveEmptyEntries);
            if (col >= tok.Length)
                throw new FormatException($"Linje har ikke kolonne {col}: {line}");

            v[i++] = double.Parse(tok[col], CultureInfo.InvariantCulture);
        }
        return v;
    }
}
