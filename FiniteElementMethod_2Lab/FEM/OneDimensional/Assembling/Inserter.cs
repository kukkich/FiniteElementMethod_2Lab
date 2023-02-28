using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;

public class Inserter : IInserter<SymmetricSparseMatrix>
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

    public void InsertMatrix(SymmetricSparseMatrix matrix, LocalMatrix localMatrix)
    {
        var matrixSize = localMatrix.IndexPermutation.Length;
        for (var i = 0; i < matrixSize; i++)
        {
            var row = localMatrix.IndexPermutation
                .Apply(i);

            for (var j = 0; j < matrixSize; j++)
            {
                var column = localMatrix.IndexPermutation
                    .Apply(j);
                if (column > row) continue;

                matrix[row, column] += localMatrix[i, j];
            }
        }
    }
}