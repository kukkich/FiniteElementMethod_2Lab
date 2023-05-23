using System;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.SLAE.Preconditions;
using SharpMath;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.SLAE.Solvers;

public class LOS
{
    private readonly LUPreconditioner _luPreconditioner;
    private readonly LUSparse _luSparse;
    private SparseMatrix _preconditionMatrix;
    private Vector _r;
    private Vector _z;
    private Vector _p;

    public LOS(LUPreconditioner luPreconditioner, LUSparse luSparse)
    {
        _luPreconditioner = luPreconditioner;
        _luSparse = luSparse;
    }

    private void PrepareProcess(Equation<SparseMatrix> equation)
    {
        _preconditionMatrix = _luPreconditioner.Decompose(equation.Matrix);
        var b = equation.Matrix * equation.Solution;
        _r = _luSparse.CalcY(_preconditionMatrix, LinAl.Subtract(equation.RightSide, b, b));
        _z = _luSparse.CalcX(_preconditionMatrix, _r);
        _p = _luSparse.CalcY(_preconditionMatrix, equation.Matrix * _z);
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SparseMatrix> equation)
    {
        //Console.WriteLine("LOS");

        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residualNext > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var scalarPP = Vector.ScalarProduct(_p, _p);

            var alpha = Vector.ScalarProduct(_p, _r) / scalarPP;

            LinAl.Sum(equation.Solution, LinAl.Multiply(alpha, _z), equation.Solution);

            var alphaP = LinAl.Multiply(alpha, _p);
            var rNext = LinAl.Subtract(_r, alphaP, alphaP);

            var LAUr = _luSparse.CalcY(_preconditionMatrix, equation.Matrix * _luSparse.CalcX(_preconditionMatrix, rNext));

            var beta = -(Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var betaZ = LinAl.Multiply(beta, _z);
            var zNext = LinAl.Sum(_luSparse.CalcX(_preconditionMatrix, rNext), betaZ, betaZ);

            var betaP = LinAl.Multiply(beta, _p);
            var pNext = LinAl.Sum(LAUr, betaP, betaP);

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;

            //CourseHolder.GetInfo(i, residualNext);
        }

        //Console.WriteLine();
    }
}