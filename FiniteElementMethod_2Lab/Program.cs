using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab;

public class Program
{
    public static double[] Time;
    public static Grid<double> Grid;
    public static Func<double, double, double> U;

    static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        var infrastructure = AccuracyTests.GetQuadraticX();

        while (infrastructure.HasNextTime)
        {
            WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);
            infrastructure.NextTimeIteration();
        }
        WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);
        
        Console.WriteLine(infrastructure.IterationsNumber);
    }

    public static void WriteSolution(Vector q, double t)
    {
        var solution = new FiniteElementSolution(Grid, q);

        if (Equals(t, 1d) || Equals(t, 2d) || Equals(t, 3d))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine($"t = {t}");
            Console.ForegroundColor = ConsoleColor.White;

            foreach (var x in new double[] {1/3d})
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                //Console.WriteLine($"{x}");
                Console.ForegroundColor = ConsoleColor.White;
                
                var u = solution.Calculate(x);
                var expected = U(x, t);
                Console.WriteLine($"{u:F15} {expected:F15}");
            }
            
        }
        var step = Grid.Elements.First().Length;
        var start = Grid.Nodes.First();
        
        // Console.ForegroundColor = ConsoleColor.Red;
        // Console.WriteLine($"t = {t}");
        // Console.ForegroundColor = ConsoleColor.White;
        //
        // Console.Write("x:     ");
        // for (int i = 0; i < Grid.Nodes.Length * 2 - 1; i++)
        // {
        //     var node = start + step / 2 * i; 
        //     Console.Write($"{node:F3}  ");
        // }
        //
        // Console.WriteLine();
        // Console.Write("u(x):  ");
        // for (int i = 0; i < Grid.Nodes.Length * 2 - 1; i++)
        // {
        //     var node = start + step / 2 * i;
        //     Console.Write($"{solution.Calculate(node):F3}  ");
        // }
        //
        // Console.WriteLine();
        // Console.Write("expt:  ");
        // for (int i = 0; i < Grid.Nodes.Length * 2 - 1; i++)
        // {
        //     var node = start + step / 2 * i;
        //     Console.Write($"{U(node, t):F3}  ");
        // }
        //
        // Console.WriteLine();
        // Console.ForegroundColor = ConsoleColor.Cyan;
        // Console.Write("diff:  ");
        // for (int i = 0; i < Grid.Nodes.Length * 2 - 1; i++)
        // {
        //     var node = start + step / 2 * i;
        //     Console.Write($"{Math.Abs(U(node, t) - solution.Calculate(node)):F3}  ");
        // }
        // Console.ForegroundColor = ConsoleColor.White;
        //
        //
        //
        // Console.WriteLine();
        // Console.WriteLine();
    }

    public bool Equal(double x, double y)
    {
        return Math.Abs(x - y) <= 1e-14;
    }
}