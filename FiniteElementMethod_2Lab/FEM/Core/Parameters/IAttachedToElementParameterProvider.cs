namespace FiniteElementMethod_2Lab.FEM.Core.Parameters;

public interface IAttachedToElementParameterProvider<out T>
{
    public T GetById(int id);
}