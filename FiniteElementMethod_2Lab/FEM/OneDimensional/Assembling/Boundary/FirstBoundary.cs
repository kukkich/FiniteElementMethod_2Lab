using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;

public readonly struct FirstBoundary
{
    public required int Node { get; init; }
    public required Func<double, double> ValueFromTime { get; init; }

    public FixedValue FromTime(double time)
    {
        return new FixedValue
        {
            Node = Node,
            Value = ValueFromTime(time)
        };
    }
}

public readonly struct FixedValue
{
    public required int Node { get; init; }
    public required double Value { get; init; }
}