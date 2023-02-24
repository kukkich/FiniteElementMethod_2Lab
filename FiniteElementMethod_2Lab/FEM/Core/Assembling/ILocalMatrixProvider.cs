using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface ILocalMatrixProvider
{
    public StackAllocMatrix GetMatrix(Element element);
}