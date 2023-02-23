namespace FiniteElementMethod_2Lab.Geometry;

public readonly struct Element
{
    public const int StepsInsideElement = 0;
    public const int NodesOnBound = StepsInsideElement + 1;
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