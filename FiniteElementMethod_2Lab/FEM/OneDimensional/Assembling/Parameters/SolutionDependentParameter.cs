using System;
using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.Geometry;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;

public class SolutionDependentParameter : IFunctionalParameter<double>
{
    private readonly FiniteElementSolution _solution;
    private readonly Func<double, double> _solutionDependency;
    private readonly Grid<double> _grid;

    public SolutionDependentParameter(
        FiniteElementSolution solution, 
        Func<double, double> solutionDependency,
        Grid<double> grid
    )
    {
        _solution = solution;
        _solutionDependency = solutionDependency;
        _grid = grid;
    }

    public double Calculate(int nodeIndex)
    {
        var node = _grid.Nodes[nodeIndex];
        var u = _solution.Calculate(node);
        
        return _solutionDependency(u);
    }

    public double Calculate(double point)
    {
        var u = _solution.Calculate(point);

        return _solutionDependency(u);
    }
}