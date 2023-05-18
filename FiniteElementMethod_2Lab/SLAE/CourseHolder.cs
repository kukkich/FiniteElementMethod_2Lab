using System;

namespace FiniteElementMethod_2Lab.SLAE;

public class CourseHolder
{
    public static void GetInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    public static void WriteSolution(double point, double value)
    {
        Console.WriteLine($"{point} {value:E14}");
    }

    public static void WriteAreaInfo()
    {
        Console.WriteLine("Point not in area");
    }
}