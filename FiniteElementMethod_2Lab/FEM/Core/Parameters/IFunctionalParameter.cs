namespace FiniteElementMethod_2Lab.FEM.Core.Parameters;

public interface IFunctionalParameter<in TPoint>
{
    public double Calculate(int nodeIndex);
}