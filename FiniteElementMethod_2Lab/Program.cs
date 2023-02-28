using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.OneDimensional;
using FiniteElementMethod_2Lab.Geometry;
using FiniteElementMethod_2Lab.Geometry.Core;
using FiniteElementMethod_2Lab.Geometry.Splitting;
using SharpMath.EquationsSystem.Preconditions.Diagonal;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab;

public class Program
{
    static void Main(string[] args)
    {
        var timeSplit = new AxisSplitParameter(
            new[] {0d, 10d},
            new IIntervalSplitter[]
            {
                new UniformSplitter(10),
            });

        var xSplit = new AxisSplitParameter(
            new[] {0d, 1d},
            new IIntervalSplitter[]
            {
                new UniformSplitter(1),
            }
        );

        double[] time = new UniformSplitter(10)
            .EnumerateValues(new Interval(0, 1d))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(2d, 2d)
            .SetSLAESolver(() => new SLAESolvingConfiguration
            {
                PreconditionFactory = new DiagonalPreconditionerFactory(),
                MaxIteration = 15,
                Precision = 1e-14
            })
            .SetSigma(1)
            .SetDensityFunction(x => -2d)
            .SetLambdaBySolutionDependency(u => 1d)
            .Build();

        while (infrastructure.HasNextTime)
        {
            WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);
            infrastructure.NextTimeIteration();
        }
        WriteSolution(infrastructure.CurrentSolution, infrastructure.CurrentTime);


        Console.WriteLine("123");
    }

    public static void WriteSolution(Vector q, double t)
    {
        var constant = q[0];
        var coef = q[1] - q[0];

        Console.WriteLine($"t = {t:F3}    {coef:F3}x + {constant:F3}");
    }
}