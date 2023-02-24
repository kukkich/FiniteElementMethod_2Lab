using System;
using FiniteElementMethod_2Lab.Geometry;
using FiniteElementMethod_2Lab.Geometry.Splitting;

namespace FiniteElementMethod_2Lab;

public class Program
{
    static void Main(string[] args)
    {
        var splitParameter = new AxisSplitParameter(
            new[] {1d, 5d, 7d},
            new IIntervalSplitter[]
            {
                new UniformSplitter(4),
                new ProportionalSplitter(5, 3d)
            }
        );

        var grid = new OneDimensionalGridBuilder()
            .Build(splitParameter);

        Console.WriteLine("123");
    }
}