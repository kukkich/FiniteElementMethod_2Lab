using System.Collections.Generic;

namespace FiniteElementMethod_2Lab.FEM.Core.Assembling;

public interface IMatrixPortraitBuilder<out TMatrix>
{
    TMatrix Build(IEnumerable<Element> elements, int nodesCount);
}