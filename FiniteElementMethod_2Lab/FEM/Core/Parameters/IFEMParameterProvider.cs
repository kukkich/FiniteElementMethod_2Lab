namespace FiniteElementMethod_2Lab.FEM.Core.Parameters;

public interface IFEMParameterProvider<out T>
{
    public T GetById(int id);
}