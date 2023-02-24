namespace FiniteElementMethod_2Lab.FEM.Core.Parameters;

public interface IMaterialProvider<out TMaterial>
{
    public TMaterial GetById(int id);
}