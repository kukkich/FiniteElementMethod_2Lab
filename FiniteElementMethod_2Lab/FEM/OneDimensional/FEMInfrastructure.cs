using SharpMath.EquationsSystem.Solver;
using System;
using FiniteElementMethod_2Lab.FEM.Core;
using SharpMath.Vectors;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.Matrices;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Local;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class FEMInfrastructure
{
    public Vector CurrentSolution => TimeLayersSolution[_currentTimeLayer];
    public Vector PreviousSolution => TimeLayersSolution[_currentTimeLayer - 1];
    public Vector[] TimeLayersSolution { get; private set; }
    public bool HasNextTime => _currentTimeLayer < _timeLayers.Length - 1; 
    public double CurrentTime => _timeLayers[_currentTimeLayer];
    public double TimeStep => CurrentTime - _timeLayers[_currentTimeLayer - 1];

    private int _currentTimeLayer = 0;


    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;
    private readonly Grid<double> _grid;
    private readonly IFunctionalParameter<double> _densityFunction;
    private readonly FixedValueProvider _sigma;
    private readonly ConjugateGradientSolver _SLAESolver;
    private readonly double[] _timeLayers;
    private readonly Func<double, double> _lambda;

    public FEMInfrastructure(
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder,
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessTemplateProvider,
        IInserter<SymmetricSparseMatrix> inserter,
        Grid<double> grid,
        IFunctionalParameter<double> densityFunction,
        FixedValueProvider sigma,
        double[] timeLayers,
        ConjugateGradientSolver SLAESolver,
        Vector initialWeights,
        Func<double, double> lambda
        )
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _massTemplateProvider = massTemplateProvider;
        _stiffnessTemplateProvider = stiffnessTemplateProvider;
        _inserter = inserter;
        _grid = grid;
        _densityFunction = densityFunction;
        _sigma = sigma;
        _timeLayers = timeLayers;
        _SLAESolver = SLAESolver;
        TimeLayersSolution = new Vector[_timeLayers.Length];
        TimeLayersSolution[0] = initialWeights;
        _lambda = lambda;
    }

    public void NextTimeIteration()
    {
        _currentTimeLayer++;
        TimeLayersSolution[_currentTimeLayer] = TimeLayersSolution[_currentTimeLayer - 1];

        // Сюда поставить цикл
        // Простая итерация/Ньютон
        var solver = new FiniteElementSolver(GetGlobalAssembler(), _SLAESolver);
        var weights = solver.Solve();
        TimeLayersSolution[_currentTimeLayer] = weights;
    }

    private GlobalAssembler GetGlobalAssembler()
    {
        return new GlobalAssembler(
            grid: _grid,
            densityFunctionProvider: _densityFunction,
            diffusionProvider: GetLambda(),
            matrixPortraitBuilder: _matrixPortraitBuilder,
            localAssembler: GetLocalAssembler(),
            inserter: _inserter
        );
    }

    private ILocalAssembler GetLocalAssembler()
    {
        return new LocalAssembler(
            lambda: GetLambda(),
            massTemplateProvider: _massTemplateProvider,
            stiffnessTemplateProvider: _stiffnessTemplateProvider,
            sigma: _sigma,
            densityFunctionProvider: _densityFunction,
            previousTimeLayerSolution: PreviousSolution,
            timeStep: CurrentTime - _timeLayers[_currentTimeLayer - 1]
        );
    }

    private IFunctionalParameter<double> GetLambda()
    {
        return new SolutionDependentParameter(
            new FiniteElementSolution(_grid, CurrentSolution),
            _lambda,
            _grid
        );
    }
}