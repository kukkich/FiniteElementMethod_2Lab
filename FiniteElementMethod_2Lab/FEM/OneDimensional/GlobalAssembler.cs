using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class GlobalAssembler
{
    private readonly Grid<double> _grid;
    private readonly IFEMParameterProvider<double> _densityFunctionProvider;
    private readonly IFEMParameterProvider<double> _diffusionProvider;
    private readonly ILocalMatrixProvider _massMatrixProvider;
    private readonly ILocalMatrixProvider _stiffnessMatrixProvider;
    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;

    private Equation<SymmetricSparseMatrix> _equation = null!;

    public GlobalAssembler(
        Grid<double> grid,
        IFEMParameterProvider<double> densityFunctionProvider,
        IFEMParameterProvider<double> diffusionProvider,
        ILocalMatrixProvider massMatrixProvider,
        ILocalMatrixProvider stiffnessMatrixProvider,
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixBuilder,
        ILocalAssembler localAssembler,
        IInserter<SymmetricSparseMatrix> inserter
    )
    {
        _grid = grid;
        _densityFunctionProvider = densityFunctionProvider;
        _diffusionProvider = diffusionProvider;
        _massMatrixProvider = massMatrixProvider;
        _stiffnessMatrixProvider = stiffnessMatrixProvider;
        _matrixBuilder = matrixBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public GlobalAssembler BuildEquation()
    {
        SymmetricSparseMatrix globalMatrix = _matrixBuilder.Build(_grid.Elements, _grid.Nodes.Length);
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