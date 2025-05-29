using System.IO;

class Program {
    static void Main() {
        string[] lines = {
            "1\t10",
            "2\t20",
            "3\t30",
            "4\t40",
            "5\t40"
        };
        File.WriteAllLines("test.txt", lines);
    }
}
