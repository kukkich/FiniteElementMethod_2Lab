using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly IFunctionalParameter<double> _lambda;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IAttachedToElementParameterProvider<double> _sigma;
    private readonly IFunctionalParameter<double> _densityFunctionProvider;
    private readonly Vector _previousTimeLayerSolution;
    private readonly double _timeStep;

    public LocalAssembler(
        IFunctionalParameter<double> lambda,
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessTemplateProvider,
        IAttachedToElementParameterProvider<double> sigma,
        IFunctionalParameter<double> densityFunctionProvider,
        Vector previousTimeLayerSolution,
        double timeStep
    )
    {
        _lambda = lambda;
        _massTemplateProvider = massTemplateProvider;
        _stiffnessTemplateProvider = stiffnessTemplateProvider;
        _sigma = sigma;
        _densityFunctionProvider = densityFunctionProvider;
        _previousTimeLayerSolution = previousTimeLayerSolution;
        _timeStep = timeStep;
    }

    public LocalMatrix AssembleMatrix(Element element)
    {
        var stiffness = GetStiffnessMatrix(element);
        var mass = GetMassMatrix(element);

        var resultMatrix = LinAl.Sum(stiffness, mass).AsImmutable();



        return new LocalMatrix(
            resultMatrix,
            new IndexPermutation(element.NodeIndexes)
        );

    }

    public LocalVector AssembleRightSide(Element element)
    {
        var vector = GetRightSide(element);

        return new LocalVector(
            vector,
            new IndexPermutation(element.NodeIndexes)
        );
    }

    private ImmutableMatrix GetStiffnessMatrix(Element element)
    {
        var template = _stiffnessTemplateProvider.GetMatrix();
        var lambdaLeft = _lambda.Calculate(element.GetBoundNodeIndexes(Bound.Left));
        var lambdaRight = _lambda.Calculate(element.GetBoundNodeIndexes(Bound.Right));

        var coefficient = (lambdaLeft + lambdaRight) / (2d * element.Length);


        return LinAl.Multiply(template, coefficient);
    }

    private ImmutableMatrix GetMassMatrix(Element element)
    {
        var template = _massTemplateProvider.GetMatrix();

        var sigma = _sigma.GetById(element.MaterialId);
        var h = element.Length;

        var coefficient = sigma * h / (6d * _timeStep);

        var result = LinAl.Multiply(template, coefficient);

        return result;
    }

    protected Vector GetRightSide(Element element)
    {
        var template = _massTemplateProvider.GetMatrix();
        var vector = Vector.Create(2, i =>
        {
            var nodeIndex = element.NodeIndexes[i];

            var f = _densityFunctionProvider.Calculate(nodeIndex);

            var sigma = _sigma.GetById(element.MaterialId);
            var q = _previousTimeLayerSolution[nodeIndex];
            var timeImpact = q * sigma / (_timeStep);

            return f + timeImpact;
        });

        var matrix = LinAl.Multiply(template, vector);

        return LinAl.Multiply(element.Length / 6d, matrix);
    }
}