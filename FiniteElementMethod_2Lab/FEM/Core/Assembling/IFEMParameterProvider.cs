namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface IFEMParameterProvider<out T>
{
    public T GetByNodeIndex(int nodeIndex);
}