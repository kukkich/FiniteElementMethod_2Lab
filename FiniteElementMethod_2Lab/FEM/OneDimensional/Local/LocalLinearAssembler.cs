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

    public new LocalMatrix AssembleMatrix(Element element)
    {
        var matrix = base.AssembleMatrix(element).Matrix.AsMutable();

        for (var j = 0; j < matrix.Rows; j++)
        {
            var derivativeStiffnessMatrix = GetDerivativeStiffnessMatrix(element, PreviousTimeLayerSolution[j]);

            for (var i = 0; i < matrix.Columns; i++)
            {
                var sum = 0d;

                for (var r = 0; r < matrix.Columns; r++)
                {
                    sum += derivativeStiffnessMatrix[i, r] * PreviousTimeLayerSolution[r];
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
            var derivativeStiffnessMatrix = GetDerivativeStiffnessMatrix(element, PreviousTimeLayerSolution[j]);

            for (var i = 0; i < vector.Length; i++)
            {
                for (var r = 0; r < vector.Length; r++)
                {
                    vector[i] += DampingCoefficient * PreviousTimeLayerSolution[r] * derivativeStiffnessMatrix[i, r];
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
        var template = StiffnessTemplateProvider.GetMatrix();

        var lambdaInterpolate = CalculateLambdaInterpolate(q);
        var coefficient = lambdaInterpolate / element.Length;

        return LinAl.Multiply(coefficient, template);
    }

    private double CalculateLambdaInterpolate(double q)
    {
        return 2d * Lambda.CalculateDerivative(q) / 2d;
    }
}