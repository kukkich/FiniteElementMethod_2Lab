using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;

public class StiffnessMatrixTemplateProvider : ITemplateMatrixProvider
{
    public ImmutableMatrix GetMatrix()
    {
        var matrix = new ImmutableMatrix(
            new double[,]
            {
                {1, -1},
                {-1, 1}
            }
        );

        return matrix;
    }
}