using FiniteElementMethod_2Lab.FEM.Core.Parameters;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;

public class Material : Material<SolutionDependentParameter, double>
{
    public Material(SolutionDependentParameter lambda, double gamma) 
        : base(lambda, gamma)
        { }
}
    
