using System.Collections.Generic;
using FiniteElementMethod_2Lab.Geometry.Core;

namespace FiniteElementMethod_2Lab.Geometry.Splitting;

public interface IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}