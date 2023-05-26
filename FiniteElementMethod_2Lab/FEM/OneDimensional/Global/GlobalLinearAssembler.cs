using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Local;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;
using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Global;

public class GlobalLinearAssembler
{
    private static readonly GaussExcluding GaussExcluder;

    static GlobalLinearAssembler()
    {
        GaussExcluder = new GaussExcluding();
    }

    private readonly Grid<double> _grid;
    private readonly IFunctionalParameter<double> _densityFunctionProvider;
    private readonly IFunctionalParameter<double> _diffusionProvider;
    private readonly IMatrixPortraitBuilder<SparseMatrix> _matrixPortraitBuilder;
    private readonly LocalLinearAssembler _localLinearAssembler;
    private readonly IInserter<SparseMatrix> _inserter;

    private Equation<SparseMatrix> _equation = null!;

    public GlobalLinearAssembler(
        Grid<double> grid,
        IFunctionalParameter<double> densityFunctionProvider,
        IFunctionalParameter<double> diffusionProvider,
        IMatrixPortraitBuilder<SparseMatrix> matrixPortraitBuilder,
        LocalLinearAssembler localLinearAssembler,
        IInserter<SparseMatrix> inserter
    )
    {
        _grid = grid;
        _densityFunctionProvider = densityFunctionProvider;
        _diffusionProvider = diffusionProvider;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localLinearAssembler = localLinearAssembler;
        _inserter = inserter;
    }

    public Equation<SparseMatrix> BuildEquation()
    {
        var globalMatrix = _matrixPortraitBuilder.Build(_grid.Elements, _grid.Nodes.Length);
        _equation = new Equation<SparseMatrix>(
            Matrix: globalMatrix,
            Solution: new Vector(new double[globalMatrix.CountRows]),
            RightSide: new Vector(new double[globalMatrix.CountRows])
        );

        foreach (var element in _grid.Elements)
        {
            var localMatrix = _localLinearAssembler.AssembleMatrix(element);
            var localRightSide = _localLinearAssembler.AssembleRightSide(element);

            _inserter.InsertMatrix(_equation.Matrix, localMatrix);
            _inserter.InsertVector(_equation.RightSide, localRightSide);
        }

        return _equation;
    }

    public GlobalLinearAssembler ApplyThirdBoundaryConditions(Equation<SymmetricSparseMatrix> equation)
    {
        throw new NotImplementedException();
    }

    public GlobalLinearAssembler ApplySecondBoundaryConditions()
    {
        throw new NotImplementedException();
    }

    public GlobalLinearAssembler ApplyFirstBoundaryConditions(Equation<SparseMatrix> equation, FixedValue[] conditions)
    {
        foreach (var valueCondition in conditions)
        {
            GaussExcluder.Exclude(
                equation,
                row: valueCondition.Node,
                fixedValue: valueCondition.Value
            );
        }

        return this;
    }
}