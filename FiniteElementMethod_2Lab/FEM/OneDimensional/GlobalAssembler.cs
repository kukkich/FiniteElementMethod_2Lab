using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class GlobalAssembler
{
    private readonly Grid<double> _grid;
    private readonly IFEMParameterProvider<double> _densityFunctionProvider;
    private readonly IFEMParameterProvider<IFunctionalParameter<double>> _diffusionProvider;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;

    private Equation<SymmetricSparseMatrix> _equation = null!;

    // TODO Имплементировать помеченные интерфейсы 
    public GlobalAssembler(
        Grid<double> grid,
        IFEMParameterProvider<double> densityFunctionProvider, 
        IFEMParameterProvider<IFunctionalParameter<double>> diffusionProvider, // TODO 
        ITemplateMatrixProvider massTemplateProvider, // TODO 
        ITemplateMatrixProvider stiffnessTemplateProvider, // TODO 
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder, 
        ILocalAssembler localAssembler, // TODO 
        IInserter<SymmetricSparseMatrix> inserter 
    )
    {
        _grid = grid;
        _densityFunctionProvider = densityFunctionProvider;
        _diffusionProvider = diffusionProvider;
        _massTemplateProvider = massTemplateProvider;
        _stiffnessTemplateProvider = stiffnessTemplateProvider;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public GlobalAssembler BuildEquation()
    {
        SymmetricSparseMatrix globalMatrix = _matrixPortraitBuilder.Build(_grid.Elements, _grid.Nodes.Length);
        _equation = new Equation<SymmetricSparseMatrix>(
            Matrix: globalMatrix,
            Solution: new Vector(new double[globalMatrix.RowIndexes.Length]),
            RightSide: new Vector(new double[globalMatrix.RowIndexes.Length])
        );

        foreach (var element in _grid.Elements)
        {
            var localMatrix = _localAssembler.AssembleMatrix(element);
            var localRightSide = _localAssembler.AssembleRightSide(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
            _inserter.InsertVector(_equation.RightSide, localRightSide);
        }

        return this;
    }
}