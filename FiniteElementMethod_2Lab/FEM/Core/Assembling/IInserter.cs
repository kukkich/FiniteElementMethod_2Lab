using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertVector(Vector vector, LocalVector localVector);
    public void InsertMatrix(TMatrix matrix, LocalMatrix localMatrix);
}