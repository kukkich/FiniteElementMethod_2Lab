using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public class LocalMatrix
{
    public double this[int x, int y] => Matrix[x, y];
    public IndexPermutation IndexPermutation { get; }
    public ImmutableMatrix Matrix { get; private set; }

    public LocalMatrix(ImmutableMatrix matrix, IndexPermutation indexPermutation)
    {
        Matrix = matrix;
        IndexPermutation = indexPermutation;
    }
}