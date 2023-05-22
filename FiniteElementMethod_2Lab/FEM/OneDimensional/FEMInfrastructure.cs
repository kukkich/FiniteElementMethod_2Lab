using System;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Local;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath;
using SharpMath.EquationsSystem.Solver;
using SharpMath.Matrices;
using SharpMath.Vectors;

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

    private int _currentTimeLayer;


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
        const int maxIterations = 1000;
        // Сюда поставить цикл
        // Простая итерация/Ньютон
        double norm = 1e10;
        var i = 0;
        do
        {
            i++;

            var solver = new FiniteElementSolver(
                GetGlobalAssembler(CurrentSolution),
                _SLAESolver,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equation = solver.Solve(); // q1
            LinAl.LinearCombination(
                equation.Solution, CurrentSolution,
                Relaxation, 1d - Relaxation,
                equation.Solution
           );
            

            var solverNext = new FiniteElementSolver(
                GetGlobalAssembler(equation.Solution),
                _SLAESolver,
                _firstBoundary.Select(condition => condition.FromTime(CurrentTime))
                    .ToArray()
            );

            var equationNext = solverNext.Solve(); // A(q1), b(q1)
            const double w = 0.8d;
            var qResult = LinAl.LinearCombination(
                equationNext.Solution, equation.Solution,
                w, 1d - w,
                TimeLayersSolution[_currentTimeLayer]
            );
            var Aq = LinAl.Multiply(equationNext.Matrix, qResult);
            
            var AqMinusB = LinAl.Subtract(Aq, equationNext.RightSide);
            
            norm = AqMinusB.Norm / equation.RightSide.Norm;
        } while (norm > 1e-15 && i < maxIterations);
        
        Console.Write($"time: {CurrentTime:F3} exit with norm: {norm:E6} total iterations: ");
        if (i == maxIterations)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.WriteLine($"{i}");
        Console.ResetColor();
        
    }

    private GlobalAssembler GetGlobalAssembler(Vector currentSolution)
    {
        return new GlobalAssembler(
            grid: _grid,
            densityFunctionProvider: _densityFunction,
            diffusionProvider: GetLambda(currentSolution),
            matrixPortraitBuilder: _matrixPortraitBuilder,
            localAssembler: GetLocalAssembler(currentSolution),
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
            previousTimeLayerSolution: PreviousSolution,
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