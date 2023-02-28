using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;

public class MassMatrixTemplateProvider : ITemplateMatrixProvider
{
    public ImmutableMatrix GetMatrix()
    {
        var matrix = new ImmutableMatrix(
            new double[,]
            {
                {2, 1},
                {1, 2}
            }
        );

        return matrix;
    }
}