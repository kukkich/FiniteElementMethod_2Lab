using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public readonly ref struct LocalMatrix
{
    public double this[int x, int y] => _matrix[x, y];
    public IndexPermutation IndexPermutation { get; }

    private readonly StackAllocMatrix _matrix;

    public LocalMatrix(StackAllocMatrix matrix, IndexPermutation permutation, IndexPermutation indexPermutation)
    {
        _matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}