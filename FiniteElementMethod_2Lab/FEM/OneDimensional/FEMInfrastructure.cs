using SharpMath.EquationsSystem.Solver;
using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.Core;
using SharpMath.Vectors;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.Matrices;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Local;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Global;
using FiniteElementMethod_2Lab.SLAE.Solvers;
using SharpMath;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class FEMInfrastructure
{
    public static double Relaxation { get; set; } = 1d;
    public Vector CurrentSolution => TimeLayersSolution[_currentTimeLayer];
    public Vector PreviousSolution => TimeLayersSolution[_currentTimeLayer - 1];
    public Vector[] TimeLayersSolution { get; private set; }
    public bool HasNextTime => _currentTimeLayer < _timeLayers.Length - 1;
    public double CurrentTime => _timeLayers[_currentTimeLayer];
    public double TimeStep => CurrentTime - _timeLayers[_currentTimeLayer - 1];

    private int _currentTimeLayer = 0;
    public int IterationsNumber { get; private set; }

    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly IMatrixPortraitBuilder<SparseMatrix> _linearMatrixPortraitBuilder;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;
    private readonly IInserter<SparseMatrix> _linearInserter;
    private readonly Grid<double> _grid;
    private readonly TimeRelatedFunctionalProvider _densityFunction;
    private readonly FixedValueProvider _sigma;
    private readonly ConjugateGradientSolver _mcg;
    private readonly LOS _los;
    private readonly double[] _timeLayers;
    private readonly Func<double, double> _lambda;
    private readonly FirstBoundary[] _firstBoundary;


    public FEMInfrastructure(
        IMatrixPortraitBuilder<SymmetricSparseMatrix> matrixPortraitBuilder,
        IMatrixPortraitBuilder<SparseMatrix> linearMatrixPortraitBuilder,
        ITemplateMatrixProvider massTemplateProvider,
        ITemplateMatrixProvider stiffnessTemplateProvider,
        IInserter<SymmetricSparseMatrix> inserter,
        IInserter<SparseMatrix> linearInserter,
        Grid<double> grid,
        TimeRelatedFunctionalProvider densityFunction,
        FixedValueProvider sigma,
        double[] timeLayers,
        ConjugateGradientSolver mcg,
        LOS los,
        Vector initialWeights,
        Func<double, double> lambda,
        FirstBoundary[] firstBoundary
        )
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _linearMatrixPortraitBuilder = linearMatrixPortraitBuilder;
        _massTemplateProvider = massTemplateProvider;
        _stiffnessTemplateProvider = stiffnessTemplateProvider;
        _inserter = inserter;
        _linearInserter = linearInserter;
        _grid = grid;
        _densityFunction = densityFunction;
        _sigma = sigma;
        _timeLayers = timeLayers;
        _mcg = mcg;
        _los = los;
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

        var norm = 1d;
        var i = 0;

        do
        {
            var solver = new FiniteElementSolver(
                GetGlobalAssembler(CurrentSolution),
                _mcg,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equation = solver.Solve();
            LinAl.LinearCombination(
                equation.Solution, CurrentSolution,
                Relaxation, 1d - Relaxation,
                equation.Solution
            );
            TimeLayersSolution[_currentTimeLayer] = equation.Solution;

            var solverNext = new FiniteElementSolver(
                GetGlobalAssembler(equation.Solution),
                _mcg,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equationNext = solverNext.Solve();
  
            var Aq = LinAl.Multiply(equationNext.Matrix, equation.Solution);

            var AqMinusB = LinAl.Subtract(Aq, equationNext.RightSide);

            norm = AqMinusB.Norm / equationNext.RightSide.Norm;
            i++;

        } while (norm > 1e-13 && i < 1000);

        IterationsNumber += i;
    }

    public void NextTimeNewtonIteration()
    {
        _currentTimeLayer++;
        _densityFunction.Time = CurrentTime;
        TimeLayersSolution[_currentTimeLayer] = TimeLayersSolution[_currentTimeLayer - 1];

        var norm = 1d;
        var i = 0;

        do
        {
            var solver = new LinearFiniteSolver(
                GetLinearGlobalAssembler(CurrentSolution),
                _los,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equation = solver.Solve();
            LinAl.LinearCombination(
                equation.Solution, CurrentSolution,
                Relaxation, 1d - Relaxation,
                equation.Solution
            );
            TimeLayersSolution[_currentTimeLayer] = equation.Solution;

            var solverNext = new FiniteElementSolver(
                GetGlobalAssembler(equation.Solution),
                _mcg,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equationNext = solverNext.Solve();

            var Aq = LinAl.Multiply(equationNext.Matrix, equation.Solution);

            var AqMinusB = LinAl.Subtract(Aq, equationNext.RightSide);

            norm = AqMinusB.Norm / equationNext.RightSide.Norm;
            i++;

        } while (norm > 1e-13 && i < 1000);

        IterationsNumber += i;
    }

    private GlobalLinearAssembler GetLinearGlobalAssembler(Vector solution)
    {
        return new GlobalLinearAssembler(
            grid: _grid,
            densityFunctionProvider: _densityFunction,
            diffusionProvider: GetLambda(solution),
            matrixPortraitBuilder: _linearMatrixPortraitBuilder,
            localLinearAssembler: GetLinearLocalAssembler(solution),
            inserter: _linearInserter
        );
    }

    private LocalLinearAssembler GetLinearLocalAssembler(Vector solution)
    {
        return new LocalLinearAssembler(
            lambda: GetLambda(solution),
            massTemplateProvider: _massTemplateProvider,
            stiffnessTemplateProvider: _stiffnessTemplateProvider,
            sigma: _sigma,
            densityFunctionProvider: _densityFunction,
            previousTimeLayerSolution: solution,
            timeStep: CurrentTime - _timeLayers[_currentTimeLayer - 1]
        );
    }

    private GlobalAssembler GetGlobalAssembler(Vector solution)
    {
        return new GlobalAssembler(
            grid: _grid,
            densityFunctionProvider: _densityFunction,
            diffusionProvider: GetLambda(solution),
            matrixPortraitBuilder: _matrixPortraitBuilder,
            localAssembler: GetLocalAssembler(solution),
            inserter: _inserter
        );
    }

    private ILocalAssembler GetLocalAssembler(Vector solution)
    {
        return new LocalAssembler(
            lambda: GetLambda(solution),
            massTemplateProvider: _massTemplateProvider,
            stiffnessTemplateProvider: _stiffnessTemplateProvider,
            sigma: _sigma,
            densityFunctionProvider: _densityFunction,
            previousTimeLayerSolution: solution,
            timeStep: CurrentTime - _timeLayers[_currentTimeLayer - 1]
        );
    }

    private SolutionDependentParameter GetLambda(Vector solution)
    {
        return new SolutionDependentParameter(
            new FiniteElementSolution(_grid, solution),
            _lambda,
            _grid
        );
    }
}