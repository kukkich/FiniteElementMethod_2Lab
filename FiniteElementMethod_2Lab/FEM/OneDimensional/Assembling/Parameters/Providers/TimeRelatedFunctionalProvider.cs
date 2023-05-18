using System;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.Geometry;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;

public class TimeRelatedFunctionalProvider : IFunctionalParameter<double>
{
    private readonly Grid<double> _grid;
    private readonly Func<double, double, double> _func;
    public double Time { get; set; }

    public TimeRelatedFunctionalProvider(Grid<double> grid, Func<double, double, double> func)
    {
        _grid = grid;
        _func = func;
    }

    public double Calculate(int nodeIndex)
    {
        var node = _grid.Nodes[nodeIndex];
        return _func(node, Time);
    }

    public double CalculateDerivative(double point)
    {
        throw new NotImplementedException();
    }
}