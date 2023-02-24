using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public readonly ref struct LocalVector
{
    public double this[int x] => _vector[x];
    public IndexPermutation IndexPermutation { get; }

    private readonly Vector _vector;

    public LocalVector(Vector vector, IndexPermutation permutation)
    {
        IndexPermutation = permutation;
        _vector = vector;
    }
}