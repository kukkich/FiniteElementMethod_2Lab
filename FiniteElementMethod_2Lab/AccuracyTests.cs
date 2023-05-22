using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.OneDimensional;
using FiniteElementMethod_2Lab.Geometry;
using FiniteElementMethod_2Lab.Geometry.Core;
using FiniteElementMethod_2Lab.Geometry.Splitting;
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -1d * x - (1 + t) * (1 + t))
            .SetLambdaBySolutionDependency(u => u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetNoTimeDependentLinearByX()
    {
        Program.U = (x, t) => 2*x + 1;
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => 0)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
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
            new UniformSplitter(3)
        );

        double[] time = new UniformSplitter(30)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => 3 * t * t)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetExponentialWithProduct() 
    {
        Program.U = (x, t) => Exp(x * (t - 1d));
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(128)
        );

        double[] time = new UniformSplitter(240)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => U(x, t) * (x + Pow(t-1d, 2)))
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXExponentialByTime()
    {
        Program.U = (x, t) => x + Exp(t);
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(2)
        );

        double[] time = new UniformSplitter(240)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => Exp(t))
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetLinearByXLinerByTimeLinearLabmda()
    {
        Program.U = (x, t) => 2*x + t;
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -3d)
            .SetLambdaBySolutionDependency(u => u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXLinerByTimeSquareLabmda()
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -8d*(2*x + t) + 1)
            .SetLambdaBySolutionDependency(u => u*u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXLinerByTimeCubicLabmda()
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -12d * (2 * x + t) + 1)
            .SetLambdaBySolutionDependency(u => u * u * u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXLinerByTimeFourthLabmda()
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -16d * (2 * x + t) + 1d)
            .SetLambdaBySolutionDependency(u => u * u * u * u)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXLinerByTimePeriodicLabmda()
    {
        Program.U = (x, t) => x + t;
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => Sin(x + t) + 1)
            .SetLambdaBySolutionDependency(u => Cos(u))
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }
    public static FEMInfrastructure GetLinearByXLinerByTimeExponentialLabmda()
    {
        Program.U = (x, t) => -x/2d + t;
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

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -1d*Exp(U(x, t))/4d + 1d)
            .SetLambdaBySolutionDependency(u => Exp(u))
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }



    public static FEMInfrastructure GetSquareByXLinearLabmda()
    {
        Program.U = (x, t) => x*x;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 5d },
            new UniformSplitter(5)
        );

        double[] time = new UniformSplitter(1)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -6d*x*x-2d)
            .SetLambdaBySolutionDependency(u => u + 1d)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetSquareByXLinerByTime()
    {
        Program.U = (x, t) => x*x + t;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(8)
        );

        double[] time = new UniformSplitter(30)
            .EnumerateValues(new Interval(0, 3))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => -1)
            .SetLambdaBySolutionDependency(u => 1)
            .SetFirstBoundary(0, t => U(0, t))
            .SetFirstBoundary(grid.Nodes.Length - 1, t => U(grid.Nodes[^1], t))
            .Build();

        return infrastructure;
    }

    public static FEMInfrastructure GetDimasTest()
    {
        Program.U = (x, t) => 2*x + 1;
        var U = Program.U;

        var xSplit = new AxisSplitParameter(
            new[] { 0d, 1d },
            new UniformSplitter(3)
        );

        double[] time = new UniformSplitter(4)
            .EnumerateValues(new Interval(0, 4))
            .ToArray();

        var grid = new OneDimensionalGridBuilder()
            .Build(xSplit);
        Program.Time = time;
        Program.Grid = grid;

        var infrastructure = new FEMInfrastructureBuilder()
            .SetGrid(grid)
            .SetTimeLayers(time)
            .SetInitialWeights(GetInitialWeights(grid, U, time))
            .SetSLAESolver(() => DefaultConfiguration)
            .SetSigma(1)
            .SetDensityFunction((x, t) => 0)
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