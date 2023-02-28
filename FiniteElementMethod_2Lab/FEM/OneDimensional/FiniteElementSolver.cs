using SharpMath.EquationsSystem.Solver;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class FiniteElementSolver
{
    private readonly GlobalAssembler _globalAssembler;
    private readonly ConjugateGradientSolver _SLAEsolver;

    public FiniteElementSolver(GlobalAssembler globalAssembler, ConjugateGradientSolver SLAEsolver)
    {
        _globalAssembler = globalAssembler;
        _SLAEsolver = SLAEsolver;
    }

    public Vector Solve()
    {
        var equation = _globalAssembler.BuildEquation();

        var a = equation.Matrix;

        //_globalAssembler.ApplySecondBoundaryConditions(equation, _secondBoundary)
        //     .ApplyThirdBoundaryConditions(equation, _thirdBoundary)
        //     .ApplyFirstBoundaryConditions(equation, _firstBoundary);

        var solution = _SLAEsolver.Solve(equation);


        return solution;
    }
}