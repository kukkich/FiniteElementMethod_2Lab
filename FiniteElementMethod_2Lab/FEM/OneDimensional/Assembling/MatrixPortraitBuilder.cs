using FiniteElementMethod_2Lab.FEM.Core;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.Core.Global;
using System.Collections.Generic;
using System.Linq;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;

public class MatrixPortraitBuilder : IMatrixPortraitBuilder<SparseMatrix>
{
    private List<SortedSet<int>> _adjacencyList = null!;

    public SparseMatrix Build(IEnumerable<Element> elements, int nodesCount)
    {
        BuildAdjacencyList(elements, nodesCount);

        var amount = 0;
        var buf = _adjacencyList.Select(nodeSet => amount += nodeSet.Count).ToList();
        buf.Insert(0, 0);

        var rowsIndexes = buf.ToArray();
        var columnsIndexes = _adjacencyList.SelectMany(nodeList => nodeList).ToArray();

        return new SparseMatrix(rowsIndexes, columnsIndexes);
    }

    private void BuildAdjacencyList(IEnumerable<Element> elements, int nodesCount)
    {
        _adjacencyList = new List<SortedSet<int>>(nodesCount);

        for (var i = 0; i < nodesCount; i++)
        {
            _adjacencyList.Add(new SortedSet<int>());
        }

        foreach (var element in elements)
        {
            var nodesIndexes = element.NodeIndexes;

            foreach (var currentNode in nodesIndexes)
            {
                foreach (var nodeIndex in nodesIndexes)
                {
                    if (currentNode > nodeIndex) _adjacencyList[currentNode].Add(nodeIndex);
                }
            }
        }
    }
}