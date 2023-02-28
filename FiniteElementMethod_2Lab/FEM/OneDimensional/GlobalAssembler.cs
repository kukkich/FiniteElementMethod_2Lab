using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.Matrices;
using SharpMath.Vectors;
using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class GlobalAssembler
{
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

    public GlobalAssembler ApplyThirdBoundaryConditions()
    {
        throw new NotImplementedException();
    }

    public GlobalAssembler ApplySecondBoundaryConditions()
    {
        throw new NotImplementedException();
    }

    public GlobalAssembler ApplyFirstBoundaryConditions()
    {
        throw new NotImplementedException(); 
    }
}