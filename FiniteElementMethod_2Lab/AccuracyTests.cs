using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.OneDimensional;
using FiniteElementMethod_2Lab.Geometry;
using FiniteElementMethod_2Lab.Geometry.Core;
using FiniteElementMethod_2Lab.Geometry.Splitting;
using FiniteElementMethod_2Lab.SLAE.Preconditions;
using FiniteElementMethod_2Lab.SLAE.Solvers;
using SharpMath.EquationsSystem.Preconditions.Diagonal;
using static System.Math;
namespace FiniteElementMethod_2Lab;

public static class AccuracyTests
{
    private static SLAESolvingConfiguration DefaultConfiguration => new SLAESolvingConfiguration
    {
        PreconditionFactory = new DiagonalPreconditionerFactory(),
        MaxIteration = 1500,
        Precision = 1e-15
    };

    public static FEMInfrastructure GetFirstTest()
    {
        Program.U = (x, t) => 2 - x - x * t;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(40)
            .EnumerateValues(new Interval(0, 3d))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => -1d * x - (1 + t) * (1 + t))
            .SetLambdaBySolutionDependency(u => u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    //public static FEMInfrastructure GetNoTimeDependentLinearByX()
    //{
    //    Program.U = (x, t) => 2*x + 1;
    //    var U = Program.U;

    //    var xSplit = new AxisSplitParameter(
    //        new[] { 0d, 1d },
    //        new UniformSplitter(2)
    //    );

    //    double[] time = new UniformSplitter(30)
    //        .EnumerateValues(new Interval(0, 3))
    //        .ToArray();

    //    var grid = new OneDimensionalGridBuilder()
    //        .Build(xSplit);
    //    Program.Time = time;
    //    Program.Grid = grid;

    //    var infrastructure = new FEMInfrastructureBuilder()
    //        .SetGrid(grid)
    //        .SetTimeLayers(time)
    //        .SetInitialWeights(GetInitialWeights(grid, U, time))
    //        .SetSLAESolver(() => DefaultConfiguration)
    //        .SetSigma(1)
    //        .SetDensityFunction((x, t) => 0)
    //        .SetLambdaBySolutionDependency(u => 1)
    //        .SetFirstBoundary(0, t => U(0, t))
    //        .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
    //        .Build();

    //    return infrastructure;
    //}
    public static FEMInfrastructure GetLinearTimeLinearByX()
    {
        Program.U = (x, t) => 2 * x + t;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(30)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => 1)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetQuadraticTimeLinearByX()
    {
        Program.U = (x, t) => 2 * x + t*t;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(30)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => 2*t)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetCubicTimeLinearByX()
    {
        Program.U = (x, t) => 2 * x + t * t * t;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(120)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => 3 * t * t)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetQuadraticX()
    {
        Program.U = (x, t) => x * x;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(3)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => -6 * x * x)
            .SetLambdaBySolutionDependency(u => u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetCubicX()
    {
        Program.U = (x, t) => x * x * x;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(30)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => -6 * x)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetExponential()
    {
        Program.U = (x, t) => Exp(x * (t - 1d));
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(10)
        );

        double[] time = new UniformSplitter(3)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var luPreconditioner = new LUPreconditioner();

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration, luPreconditioner, new LUSparse(luPreconditioner))
            .SetSigma(1)
            .SetDensityFunction((x, t) => U(x, t) * (x + Pow(t-1d, 2)))
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    private static double[] GetInitialWeights(Grid<double> grid, Func<double, double, double> u, double[] time)
    {
        return grid.Nodes.Select(node => u(node, time[0])).ToArray();
    }
}