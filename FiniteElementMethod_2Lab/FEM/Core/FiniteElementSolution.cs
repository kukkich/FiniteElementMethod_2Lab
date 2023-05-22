using System.Linq;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.Core;

public class FiniteElementSolution
{
    private readonly Grid<double> _grid;
    private readonly Vector _functionWeights;

    public FiniteElementSolution(Grid<double> grid, Vector functionWeights)
    {
        _grid = grid;
        _functionWeights = functionWeights;
    }

    public double Calculate(double point)
    {
        var elem = _grid.Elements.First(x => ElementHas(x, point));
        
        var leftIndex = elem.GetBoundNodeIndexes(Bound.Left);
        var rightIndex = elem.GetBoundNodeIndexes(Bound.Right);

        var left = _grid.Nodes[leftIndex];
        var right = _grid.Nodes[rightIndex];


        var qLeft = _functionWeights[leftIndex];
        var qRight = _functionWeights[rightIndex];

        var step = right - left;
        
        var sum = 0d;
        sum += qLeft * (right - point) / step;
        sum += qRight * (point - left) / step;

        return sum;
    }

    private bool ElementHas(Element elem, double point)
    {
        var leftIndex = elem.GetBoundNodeIndexes(Bound.Left);
        var rightIndex = elem.GetBoundNodeIndexes(Bound.Right);

        var left = _grid.Nodes[leftIndex];
        var right = _grid.Nodes[rightIndex];

        return left <= point && point <= right;
    }
}