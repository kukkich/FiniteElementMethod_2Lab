using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;
using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class GlobalAssembler
{
    private static readonly GaussExcluding GaussExcluder;

    static GlobalAssembler()
    {
        GaussExcluder = new GaussExcluding();
    }

    private readonly Grid<double> _grid;
    private readonly IFunctionalParameter<double> _densityFunctionProvider;
    private readonly IFunctionalParameter<double> _diffusionProvider;
    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;

    private Equation<SymmetricSparseMatrix> _equation = null!;

    public GlobalAssembler(
        Grid<double> grid,
        IFunctionalParameter<double> densityFunctionProvider,
        IFunctionalParameter<double> diffusionProvider,
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<SymmetricSparseMatrix> inserter
    )
    {
        _grid = grid;
        _densityFunctionProvider = densityFunctionProvider;
        _diffusionProvider = diffusionProvider;
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public Equation<SymmetricSparseMatrix> BuildEquation()
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

        return _equation;
    }

    public GlobalAssembler ApplyThirdBoundaryConditions(Equation<SymmetricSparseMatrix> equation)
    {
        throw new NotImplementedException();
    }

    public GlobalAssembler ApplySecondBoundaryConditions()
    {
        throw new NotImplementedException();
    }

    public GlobalAssembler ApplyFirstBoundaryConditions(Equation<SymmetricSparseMatrix> equation, FixedValue[] conditions)
    {
        foreach (var valueCondition in conditions)
        {
            GaussExcluder.ExcludeInSymmetric(
                equation,
                row: valueCondition.Node,
                fixedValue: valueCondition.Value
            );
        }

        return this;
    }
}

public class GaussExcluding
{
    public Equation<SymmetricSparseMatrix> ExcludeInSymmetric(Equation<SymmetricSparseMatrix> equation, int row, double fixedValue)
    {
        var matrixSize = equation.Matrix.RowIndexes.Length;

        foreach (var indexValue in equation.Matrix[row])
        {
            equation.RightSide[indexValue.ColumnIndex] -= indexValue.Value * fixedValue;
            indexValue.SetValue(0);
        }

        for (var i = row + 1; i < matrixSize; i++)
        {
            var sparseRow = equation.Matrix[i];
            if (!sparseRow.HasColumn(row))
            {
                continue;
            }

            equation.RightSide[i] -= sparseRow[row] * fixedValue;
            sparseRow[row] = 0;
        }

        equation.Matrix.Diagonal[row] = 1d;
        equation.RightSide[row] = fixedValue;

        return equation;
    }

    public void Exclude(Equation<SparseMatrix> equation, int row, double fixedValue)
    {
        equation.RightSide[row] = fixedValue;
        equation.Matrix.Diagonal[row] = 1d;

        for (var j = equation.Matrix.RowsIndexes[row];
             j < equation.Matrix.RowsIndexes[row + 1];
             j++)
        {
            equation.Matrix.LowerValues[j] = 0d;
        }

        for (var j = row + 1; j < equation.Matrix.CountRows; j++)
        {
            var elementIndex = equation.Matrix[j, row];

            if (elementIndex == -1) continue;
            equation.Matrix.UpperValues[elementIndex] = 0;
        }
    }
}