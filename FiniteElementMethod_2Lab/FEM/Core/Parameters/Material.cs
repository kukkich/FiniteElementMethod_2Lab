namespace FiniteElementMethod_2Lab.FEM.Core.Parameters;

public class Material<TLambda, TGamma>
{
    public TLambda Lambda { get; }
    public TGamma Gamma { get; }

    public Material(TLambda lambda, TGamma gamma)
    {
        Lambda = lambda;
        Gamma = gamma;
    }
};
