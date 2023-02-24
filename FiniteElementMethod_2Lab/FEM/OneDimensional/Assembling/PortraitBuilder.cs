using System.Collections.Generic;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Extensions;
using SharpMath.Matrices;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;

public class PortraitBuilder : IMatrixPortraitBuilder<SymmetricSparseMatrix>
{
    private IEnumerable<Element> _elements = null!;
    private int _nodesCount;

    public SymmetricSparseMatrix Build(IEnumerable<Element> elements, int nodesCount)
    {
        _elements = elements;
        _nodesCount = nodesCount;
        var indexesLists = GetLowerNodeIndexesLists();

        var rowIndexes = GetRowIndexes(indexesLists);
        var columnIndexes = GetColumnIndexes(indexesLists).ToArray();

        return new SymmetricSparseMatrix(rowIndexes, columnIndexes);
    }

    private SortedSet<int>[] GetLowerNodeIndexesLists()
    {
        var indexesLists = GetDefaultIndexesLists();

        foreach (var element in _elements)
        {
            var relatedIndexes = element.NodeIndexes;
            foreach (var index in relatedIndexes)
            {
                var lowerIndexes = GetLowerIndexesThan(index, relatedIndexes);
                indexesLists[index].AddRange(lowerIndexes);
            }
        }

        return indexesLists;
    }

    private SortedSet<int>[] GetDefaultIndexesLists()
    {
        var indexesLists = new SortedSet<int>[_nodesCount];
        for (var i = 0; i < indexesLists.Length; i++)
            indexesLists[i] = new SortedSet<int>();

        return indexesLists;
    }

    private static IEnumerable<int> GetLowerIndexesThan(int targetIndex, IEnumerable<int> indexes) =>
        indexes.Where(index => index < targetIndex);

    private static IEnumerable<int> GetRowIndexes(IEnumerable<SortedSet<int>> indexesLists)
    {
        var capacity = 0;
        foreach (var list in indexesLists)
        {
            capacity += list.Count;
            yield return capacity;
        }
    }

    private static IEnumerable<int> GetColumnIndexes(IEnumerable<SortedSet<int>> indexesLists) =>
        indexesLists.SelectMany(list => list);
}