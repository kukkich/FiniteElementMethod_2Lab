using System;
using System.Diagnostics;

namespace FiniteElementMethod_2Lab.Geometry.Core;

[DebuggerDisplay("Indexes = [{NodeIndexes[0]}, {NodeIndexes[1]}], MaterialId = {MaterialId}")]
public readonly struct Element
{
    public const int NodesOnBound = 1;
    public const int NodesInElement = 2 * NodesOnBound;
    public int[] NodeIndexes { get; }
    public int MaterialId { get; }

    public Element(int[] nodeIndexes, int materialId = 0)
    {
        if (nodeIndexes.Length != NodesInElement)
            throw new ArgumentException(nameof(nodeIndexes));
        
        NodeIndexes = nodeIndexes;
        MaterialId = materialId;
    }

    public int GetBoundNodeIndexes(Bound bound) =>
        bound switch
        {
            Bound.Left => NodeIndexes[0],
            Bound.Right => NodeIndexes[1],
            _ => throw new NotSupportedException()
        };
}