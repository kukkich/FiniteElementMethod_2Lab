﻿using System.Collections.Generic;
using FiniteElementMethod_2Lab.Geometry.Splitting;
using System.Linq;
using FiniteElementMethod_2Lab.Geometry.Core;

namespace FiniteElementMethod_2Lab.Geometry;

public class OneDimensionalGridBuilder
{
    public Grid<double> Build(AxisSplitParameter splitParameter)
    {
        var totalNodes = GetTotalNodes(splitParameter);

        var nodes = new double[totalNodes];
        
        var indexes = new Queue<int>();
        var elements = new Element[totalNodes - 1];
        var i = 0;
        
        foreach (var (section, splitter) in splitParameter.SectionWithParameter)
        {
            var values = splitter.EnumerateValues(section);
            if (i > 0)
            {
                values = values.Skip(1);
            }

            foreach (var x in values)
            {
                nodes[i] = x;
                indexes.Enqueue(i);

                if (i > 0)
                {
                    elements[i - 1] = new Element(indexes.ToArray());
                    indexes.Dequeue();
                }

                i++;
            }
        }

        return new Grid<double>(nodes, elements);
    }

    private int GetTotalNodes(AxisSplitParameter splitParameter)
    {
        return splitParameter.Splitters.Sum(x => x.Steps) + 1;
    }
}