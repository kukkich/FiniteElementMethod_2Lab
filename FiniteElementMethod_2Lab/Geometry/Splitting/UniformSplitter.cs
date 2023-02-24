using System.Collections.Generic;
using FiniteElementMethod_2Lab.Geometry.Core;

namespace FiniteElementMethod_2Lab.Geometry.Splitting;

public readonly record struct UniformSplitter(int Steps) : IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length / Steps;

        var stepNumber = 0;
        var value = interval.Begin + stepNumber * step;

        while (interval.Has(value))
        {
            yield return value;
            stepNumber++;
            value = interval.Begin + stepNumber * step;
        }
    }
}