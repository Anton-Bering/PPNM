//  HERHER: indetilvider har jeg ikke bruge det her, men det kommer jeg måske til
using System.Collections.Generic;

public static class AntonFunktioner
{
    // tal(10,20,2) -> [10, 12, 14, 16, 18, 20]
    public static List<int> tal(int n, int m, int i)
    {
        List<int> result = new List<int>();
        for (int x = n; x <= m; x += i)
        {
            result.Add(x);
        }
        return result;
    }
    // .......................................
}
