using System;
using System.Linq;
using System.Xml.Linq;
using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.OneDimensional;
using FiniteElementMethod_2Lab.Geometry;
using FiniteElementMethod_2Lab.Geometry.Core;
using FiniteElementMethod_2Lab.Geometry.Splitting;
using SharpMath.EquationsSystem.Preconditions.Diagonal;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab;

public class Program
{
    private static double[] Time;
    private static Grid<double> Grid;

    private static Func<double, double, double> U;

    static void Main(string[] args)
    {
        U = (x, t) => 2 - x - x*t;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 2d },
            new IIntervalSplitter[]
            {
                new UniformSplitter(2),
            }
        );

        double[] time = new UniformSplitter(10)
            .EnumerateValues(new Interval(0, 3d))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Time = time;
        Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(2d, 1d, 0d)
            .SetSLAESolver(() => new SLAESolvingConfiguration
            {
                PreconditionFactory = new DiagonalPreconditionerFactory(),
                MaxIteration = 15,
                Precision = 1e-14
            })
            .SetSigma(1)
            .SetDensityFunction((x, t) => -1d * x)
            .SetLambdaBySolutionDependency(u => 1d)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(2, t => U(2, t))
            .Build();

        while (infrastructure.HasNextTime)
        {
            WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);
            infrastructure.NextTimeIteration();
        }
        WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);


        //Console.WriteLine("123");
    }

    public static void WriteSolution(Vector q, double t)
    {

        var solution = new FiniteElementSolution(Grid, q);
        Console.WriteLine($"t = {t}");

        Console.Write("x:    ");
        for (int i = 0; i < 5; i++)
        {
            var node = 0 + 0.5d * i; 
            Console.Write($"{node:F3}  ");
        }

        Console.WriteLine();
        Console.Write("u(x): ");
        for (int i = 0; i < 5; i++)
        {
            var node = 0 + 0.5d * i;
            Console.Write($"{solution.Calculate(node):F3}  ");
        }
        Console.WriteLine();
        Console.WriteLine();
    }
}