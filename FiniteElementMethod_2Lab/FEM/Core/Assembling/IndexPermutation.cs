namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public readonly struct IndexPermutation
{
    public int Apply(int index) => _permutation[index];
    public int Length => _permutation.Length;

    private readonly int[] _permutation;

    public IndexPermutation(int[] permutation)
    {
        _permutation = permutation;
    }
}