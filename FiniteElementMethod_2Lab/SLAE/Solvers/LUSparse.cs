using FiniteElementMethod_2Lab.FEM.Core.Global;
using FiniteElementMethod_2Lab.SLAE.Preconditions;
using SharpMath;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.SLAE.Solvers;

public class LUSparse
{
    private readonly LUPreconditioner _luPreconditioner;

    public LUSparse(LUPreconditioner luPreconditioner)
    {
        _luPreconditioner = luPreconditioner;
    }

    public Vector Solve(Equation<SparseMatrix> equation)
    {
        var matrix = _luPreconditioner.Decompose(equation.Matrix);
        var y = CalcY(matrix, equation.RightSide);
        var x = CalcX(matrix, y);

        return x;
    }

    public Vector CalcY(SparseMatrix sparseMatrix, Vector b)
    {
        var y = b;

        for (var i = 0; i < sparseMatrix.CountRows; i++)
        {
            var sum = 0.0;
            for (var j = sparseMatrix.RowsIndexes[i]; j < sparseMatrix.RowsIndexes[i + 1]; j++)
            {
                sum += sparseMatrix.LowerValues[j] * y[sparseMatrix.ColumnsIndexes[j]];
            }
            y[i] = (b[i] - sum) / sparseMatrix.Diagonal[i];
        }

        return y;
    }

    public Vector CalcX(SparseMatrix sparseMatrix, Vector y)
    {
        var x = y.Copy();

        for (var i = sparseMatrix.CountRows - 1; i >= 0; i--)
        {
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.UpperValues[j] * x[i];
            }
        }

        return x;
    }
}