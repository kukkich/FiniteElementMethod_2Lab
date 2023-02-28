using FiniteElementMethod_2Lab.FEM.Core.Parameters;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;

public class MaterialProvider : IMaterialProvider<Material>
{
    private readonly IAttachedToElementParameterProvider<SolutionDependentParameter> _lambdaProvider;
    private readonly IAttachedToElementParameterProvider<double> _gammaProvider;

    public MaterialProvider(
        IAttachedToElementParameterProvider<SolutionDependentParameter> lambdaProvider,
        IAttachedToElementParameterProvider<double> gammaProvider
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