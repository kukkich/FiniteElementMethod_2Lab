using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface ITemplateMatrixProvider
{
    public StackAllocMatrix GetMatrix(Element element);
}