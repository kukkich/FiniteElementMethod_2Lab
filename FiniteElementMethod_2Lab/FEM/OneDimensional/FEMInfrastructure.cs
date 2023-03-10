using SharpMath.EquationsSystem.Solver;
using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.Core;
using SharpMath.Vectors;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.Matrices;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Local;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;
using SharpMath;

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
    private readonly TimeRelatedFunctionalProvider _densityFunction;
    private readonly FixedValueProvider _sigma;
    private readonly ConjugateGradientSolver _SLAESolver;
    private readonly double[] _timeLayers;
    private readonly Func<double, double> _lambda;
    private readonly FirstBoundary[] _firstBoundary;


    public FEMInfrastructure(
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder,
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessTemplateProvider,
        IInserter<SymmetricSparseMatrix> inserter,
        Grid<double> grid,
        TimeRelatedFunctionalProvider densityFunction,
        FixedValueProvider sigma,
        double[] timeLayers,
        ConjugateGradientSolver SLAESolver,
        Vector initialWeights,
        Func<double, double> lambda,
        FirstBoundary[] firstBoundary
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
        _firstBoundary = firstBoundary;
    }

    public void NextTimeIteration()
    {
        _currentTimeLayer++;
        _densityFunction.Time = CurrentTime;
        TimeLayersSolution[_currentTimeLayer] = TimeLayersSolution[_currentTimeLayer - 1];

        // Сюда поставить цикл
        // Простая итерация/Ньютон
        double norm = 1e10;

        do
        {
            var solver = new FiniteElementSolver(
                GetGlobalAssembler(),
                _SLAESolver,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equation = solver.Solve();
            TimeLayersSolution[_currentTimeLayer] = equation.Solution;



            var Aq = LinAl.Multiply(equation.Matrix, equation.Solution);

            var AqMinusB = LinAl.Subtract(Aq, equation.RightSide);

            norm = AqMinusB.Norm / equation.RightSide.Norm;
            //Console.WriteLine($"norm: {norm}");

        } while (norm > 1e-13);

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

    private SolutionDependentParameter GetLambda()
    {
        return new SolutionDependentParameter(
            new FiniteElementSolution(_grid, CurrentSolution),
            _lambda,
            _grid
        );
    }
}