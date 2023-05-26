using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Global;
using SharpMath;

namespace FiniteElementMethod_2Lab.SLAE.Solvers;

public class LinearFiniteSolver
{
    private readonly GlobalLinearAssembler _globalAssembler;
    private readonly LOS _SLAEsolver;
    private readonly FixedValue[] _firstBoundary;

    public LinearFiniteSolver(
        GlobalLinearAssembler globalAssembler,
        LOS SLAEsolver,
        FixedValue[] firstBoundary
    )
    {
        _globalAssembler = globalAssembler;
        _SLAEsolver = SLAEsolver;
        _firstBoundary = firstBoundary;
    }

    public Equation<SparseMatrix> Solve()
    {
        var equation = _globalAssembler.BuildEquation();

        _globalAssembler.ApplyFirstBoundaryConditions(equation, _firstBoundary);

        //.ApplySecondBoundaryConditions(equation, _secondBoundary);
        //     .ApplyThirdBoundaryConditions(equation, _thirdBoundary)

        _SLAEsolver.Solve(equation);

        return equation;
    }
}