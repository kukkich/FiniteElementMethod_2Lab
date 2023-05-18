using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using SharpMath.Matrices;
using SharpMath;
using SharpMath.Vectors;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Local;

public class LocalLinearAssembler : LocalAssembler
{
    private readonly IFunctionalParameter<double> _lambda;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IAttachedToElementParameterProvider<double> _sigma;
    private readonly IFunctionalParameter<double> _densityFunctionProvider;
    private readonly Vector _previousTimeLayerSolution;
    private readonly double _timeStep;
    public double DampingCoefficient { get; set; } = 1d;

    public LocalLinearAssembler
    (
        IFunctionalParameter<double> lambda,
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessTemplateProvider,
        IAttachedToElementParameterProvider<double> sigma,
        IFunctionalParameter<double> densityFunctionProvider,
        Vector previousTimeLayerSolution, double timeStep
    ) : base(lambda, massTemplateProvider, stiffnessTemplateProvider, sigma, densityFunctionProvider, previousTimeLayerSolution, timeStep)
    { }
    //{
    //    _lambda = lambda;
    //    _massTemplateProvider = massTemplateProvider;
    //    _stiffnessTemplateProvider = stiffnessTemplateProvider;
    //    _sigma = sigma;
    //    _densityFunctionProvider = densityFunctionProvider;
    //    _previousTimeLayerSolution = previousTimeLayerSolution;
    //    _timeStep = timeStep;
    //}

    public new LocalMatrix AssembleMatrix(Element element)
    {
        var matrix = base.AssembleMatrix(element).Matrix.AsMutable();

        for (var j = 0; j < matrix.Rows; j++)
        {
            var derivativeStiffnessMatrix = GetDerivativeStiffnessMatrix(element, _previousTimeLayerSolution[j]);

            for (var i = 0; i < matrix.Columns; i++)
            {
                var sum = 0d;

                for (var r = 0; r < matrix.Columns; r++)
                {
                    sum += derivativeStiffnessMatrix[i, r] * _previousTimeLayerSolution[r];
                }

                matrix[i, j] += DampingCoefficient * sum;
            }
        }

        return new LocalMatrix(
            matrix.AsImmutable(),
            new IndexPermutation(element.NodeIndexes)
        );
    }

    public new LocalVector AssembleRightSide(Element element)
    {
        var vector = GetRightSide(element);

        for (var j = 0; j < vector.Length; j++)
        {
            var derivativeStiffnessMatrix = GetDerivativeStiffnessMatrix(element, _previousTimeLayerSolution[j]);

            for (var i = 0; i < vector.Length; i++)
            {
                for (var r = 0; r < vector.Length; r++)
                {
                    vector[r] += DampingCoefficient * _previousTimeLayerSolution[i] * derivativeStiffnessMatrix[r, j] *
                                 _previousTimeLayerSolution[j];
                }
            }
        }

        return new LocalVector(
            vector,
            new IndexPermutation(element.NodeIndexes)
        );
    }

    private ImmutableMatrix GetDerivativeStiffnessMatrix(Element element, double q)
    {
        var template = _stiffnessTemplateProvider.GetMatrix();

        var lambdaInterpolate = CalculateLambdaInterpolate(q);
        var coefficient = lambdaInterpolate / element.Length;

        return LinAl.Multiply(coefficient, template);
    }

    private double CalculateLambdaInterpolate(double q)
    {
        return 2d * _lambda.CalculateDerivative(q) / 2d;
    }
}