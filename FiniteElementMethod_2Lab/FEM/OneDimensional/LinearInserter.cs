using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class LinearInserter : IInserter<SparseMatrix>
{
    public void InsertVector(Vector vector, LocalVector localVector)
    {
        var vectorLength = localVector.IndexPermutation.Length;
        for (var i = 0; i < vectorLength; i++)
        {
            var row = localVector.IndexPermutation
                .Apply(i);
            vector[row] += localVector[i];
        }
    }

    public void InsertMatrix(SparseMatrix matrix, LocalMatrix localMatrix)
    {
        var nodesIndexes = localMatrix.IndexPermutation;

        for (var i = 0; i < nodesIndexes.Length; i++)
        {
            for (var j = 0; j < i; j++)
            {
                var elementIndex = matrix[nodesIndexes.Apply(i), nodesIndexes.Apply(j)];

                if (elementIndex == -1) continue;
                matrix.LowerValues[elementIndex] += localMatrix[i, j];
                matrix.UpperValues[elementIndex] += localMatrix[j, i];
            }

            matrix.Diagonal[nodesIndexes.Apply(i)] += localMatrix[i, i];
        }
    }
}