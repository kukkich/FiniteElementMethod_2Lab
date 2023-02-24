using FiniteElementMethod_2Lab.FEM.Core.Parameters;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;

public class MaterialProvider : IMaterialProvider<Material>
{
    private readonly IFEMParameterProvider<SolutionDependentParameter> _lambdaProvider;
    private readonly IFEMParameterProvider<double> _gammaProvider;

    public MaterialProvider(
        IFEMParameterProvider<SolutionDependentParameter> lambdaProvider,
        IFEMParameterProvider<double> gammaProvider
    )
    {
        _lambdaProvider = lambdaProvider;
        _gammaProvider = gammaProvider;
    }

    public Material GetById(int id)
    {
        return new Material(
            _lambdaProvider.GetById(id),
            _gammaProvider.GetById(id)
        );
    }
}