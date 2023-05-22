using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using SharpMath;
using SharpMath.EquationsSystem.Solver;
using SharpMath.Matrices;

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

    public Equation<SymmetricSparseMatrix> Solve()
    {
        var equation = _globalAssembler.BuildEquation();

        var a = equation.Matrix;

        _globalAssembler.ApplyFirstBoundaryConditions(equation, _firstBoundary);

        //.ApplySecondBoundaryConditions(equation, _secondBoundary);
        //     .ApplyThirdBoundaryConditions(equation, _thirdBoundary)

        _SLAEsolver.Solve(equation);


        return equation;
    }
}