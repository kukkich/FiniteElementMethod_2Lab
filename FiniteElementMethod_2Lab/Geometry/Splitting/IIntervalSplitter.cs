using FiniteElementMethod_2Lab.Geometry.Core;
using System.Collections.Generic;

namespace FiniteElementMethod_2Lab.Geometry.Splitting;

public interface IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}