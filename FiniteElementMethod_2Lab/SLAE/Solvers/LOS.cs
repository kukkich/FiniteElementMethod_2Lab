using System;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.SLAE.Preconditions;
using SharpMath;
using SharpMath.Vectors;
using UMF3.SLAE;

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
        _r = _luSparse.CalcY(_preconditionMatrix, GlobalVector.Subtract(equation.RightSide, equation.Matrix * equation.Solution));
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
        Console.WriteLine("LOS");

        var residual = Vector.ScalarProduct(_r, _r);
        var residualNext = residual;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residualNext > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var scalarPP = Vector.ScalarProduct(_p, _p);

            var alpha = Vector.ScalarProduct(_p, _r) / scalarPP;

            GlobalVector.Sum(equation.Solution, alpha * _z);

            var rNext = GlobalVector.Subtract(_r, alpha * _p);

            var LAUr = _luSparse.CalcY(_preconditionMatrix, equation.Matrix * _luSparse.CalcX(_preconditionMatrix, rNext));

            var beta = -(Vector.ScalarProduct(_p, LAUr) / scalarPP);

            var zNext = GlobalVector.Sum(_luSparse.CalcX(_preconditionMatrix, rNext), GlobalVector.Multiply(beta, _z));

            var pNext = GlobalVector.Sum(LAUr, GlobalVector.Multiply(beta, _p));

            _r = rNext;
            _z = zNext;
            _p = pNext;

            residualNext = Vector.ScalarProduct(_r, _r) / residual;

            CourseHolder.GetInfo(i, residualNext);
        }

        Console.WriteLine();
    }
}