using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface ILocalAssembler
{
    public LocalMatrix AssembleMatrix(Element element);
    public LocalVector AssembleRightSide(Element element);
}