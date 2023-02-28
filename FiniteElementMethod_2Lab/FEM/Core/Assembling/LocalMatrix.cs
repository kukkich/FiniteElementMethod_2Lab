using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public class LocalMatrix
{
    public double this[int x, int y] => _matrix[x, y];
    public IndexPermutation IndexPermutation { get; }

    private readonly ImmutableMatrix _matrix;

    public LocalMatrix(ImmutableMatrix matrix, IndexPermutation indexPermutation)
    {
        _matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}