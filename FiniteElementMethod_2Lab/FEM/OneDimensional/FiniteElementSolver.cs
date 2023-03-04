using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using SharpMath.EquationsSystem.Solver;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class FiniteElementSolver
{
    private readonly GlobalAssembler _globalAssembler;
    private readonly ConjugateGradientSolver _SLAEsolver;
    private readonly FixedValue[] _firstBoundary;

    public FiniteElementSolver(
        GlobalAssembler globalAssembler, 
        ConjugateGradientSolver SLAEsolver,
        FixedValue[] firstBoundary
        )
    {
        _globalAssembler = globalAssembler;
        _SLAEsolver = SLAEsolver;
        _firstBoundary = firstBoundary;
    }

    public Vector Solve()
    {
        var equation = _globalAssembler.BuildEquation();

        var a = equation.Matrix;

        _globalAssembler.ApplyFirstBoundaryConditions(equation, _firstBoundary);

        //.ApplySecondBoundaryConditions(equation, _secondBoundary);
        //     .ApplyThirdBoundaryConditions(equation, _thirdBoundary)

        var solution = _SLAEsolver.Solve(equation);


        return solution;
    }
}