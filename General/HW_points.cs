// Detter er bare en fil, jeg bruge til at give mig selv points, for de forsklihe HW-opgaver, så jeg har et overblik over det.
using System;

class HW_points
{
    static public void HW_POINTS(int A, int B, int C)
    {
        Console.WriteLine("\n------------ POINTS: received from tasks A, B, and C ------------\n");

        int points_A = 0, points_B = 0, points_C = 0;
        string s_a = "", s_b = "", s_c = "";

        if (A == 1)
        {
            points_A = 6;
            s_a = "🎉";
            Console.WriteLine("Task A completed → 6 points😊");
        }
        else if (A == 0)
        {
            Console.WriteLine("Task A not completed → 0 points🤬");
        }

        if (B == 1)
        {
            points_B = 3;
            s_b = "🎉";
            Console.WriteLine("Task B completed → 3 points😄");
        }
        else if (B == 0)
        {
            Console.WriteLine("Task B not completed → 0 points😭");
        }

        if (C == 1)
        {
            points_C = 1;
            s_c = "🎉";
            Console.WriteLine("Task C completed → 1 point 😎");
        }
        else if (C == 0)
        {
            Console.WriteLine("Task C not completed → 0 points😢");
        }

        Console.WriteLine();
        Console.WriteLine($"In total, this exercise results in {points_A + points_B + points_C} points {s_a}{s_b}{s_c}");
    }


}
