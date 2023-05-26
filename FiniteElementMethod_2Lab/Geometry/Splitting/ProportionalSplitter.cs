using FiniteElementMethod_2Lab.Geometry.Core;
using System;
using System.Collections.Generic;

namespace FiniteElementMethod_2Lab.Geometry.Splitting;

public readonly record struct ProportionalSplitter : IIntervalSplitter
{
    public int Steps { get; }
    public double DischargeRatio { get; }

    private readonly double _lengthCoefficient;

    public ProportionalSplitter(int steps, double dischargeRatio)
    {
        if (Math.Abs(DischargeRatio - 1d) < 1e-15)
            throw new NotSupportedException();

        Steps = steps;
        DischargeRatio = dischargeRatio;
        _lengthCoefficient = (DischargeRatio - 1d) / (Math.Pow(DischargeRatio, Steps) - 1d);
    }

    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length * _lengthCoefficient;

        var stepNumber = 0;
        var value = interval.Begin;

        while (interval.Has(value))
        {
            yield return value;

            value = interval.Begin + step * (Math.Pow(DischargeRatio, stepNumber + 1) - 1d) / (DischargeRatio - 1d);
            stepNumber++;
        }
    }
}