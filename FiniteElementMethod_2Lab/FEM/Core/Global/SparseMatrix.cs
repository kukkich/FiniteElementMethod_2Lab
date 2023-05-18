using System;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.Core.Global;

public class SparseMatrix
{
    public double[] Diagonal { get; set; }
    public double[] LowerValues { get; set; }
    public double[] UpperValues { get; set; }
    public int[] RowsIndexes { get; }
    public int[] ColumnsIndexes { get; }

    public int CountRows => Diagonal.Length;
    public int CountColumns => Diagonal.Length;
    public int this[int rowIndex, int columnIndex] =>
        Array.IndexOf(ColumnsIndexes, columnIndex, RowsIndexes[rowIndex],
            RowsIndexes[rowIndex + 1] - RowsIndexes[rowIndex]);

    public SparseMatrix(int[] rowsIndexes, int[] columnsIndexes)
    {
        Diagonal = new double[rowsIndexes.Length - 1];
        LowerValues = new double[rowsIndexes[^1]];
        UpperValues = new double[rowsIndexes[^1]];
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
    }

    public SparseMatrix
    (
        int[] rowsIndexes,
        int[] columnsIndexes,
        double[] diagonal,
        double[] lowerValues,
        double[] upperValues
    )
    {
        RowsIndexes = rowsIndexes;
        ColumnsIndexes = columnsIndexes;
        Diagonal = diagonal;
        LowerValues = lowerValues;
        UpperValues = upperValues;
    }

    public static Vector operator *(SparseMatrix matrix, Vector vector)
    {
        var rowsIndexes = matrix.RowsIndexes;
        var columnsIndexes = matrix.ColumnsIndexes;
        var di = matrix.Diagonal;
        var lowerValues = matrix.LowerValues;
        var upperValues = matrix.UpperValues;

        var result = Vector.Create(matrix.CountRows);

        for (var i = 0; i < matrix.CountRows; i++)
        {
            result[i] += di[i] * vector[i];

            for (var j = rowsIndexes[i]; j < rowsIndexes[i + 1]; j++)
            {
                result[i] += lowerValues[j] * vector[columnsIndexes[j]];
                result[columnsIndexes[j]] += upperValues[j] * vector[i];
            }
        }

        return result;
    }

    public SparseMatrix Clone()
    {
        var rowIndexes = new int[RowsIndexes.Length];
        var columnIndexes = new int[ColumnsIndexes.Length];
        var diagonal = new double[Diagonal.Length];
        var lowerValues = new double[LowerValues.Length];
        var upperValues = new double[UpperValues.Length];

        Array.Copy(RowsIndexes, rowIndexes, RowsIndexes.Length);
        Array.Copy(ColumnsIndexes, columnIndexes, ColumnsIndexes.Length);
        Array.Copy(Diagonal, diagonal, Diagonal.Length);
        Array.Copy(LowerValues, lowerValues, LowerValues.Length);
        Array.Copy(UpperValues, upperValues, UpperValues.Length);

        return new SparseMatrix(rowIndexes, columnIndexes, diagonal, lowerValues, upperValues);
    }
}