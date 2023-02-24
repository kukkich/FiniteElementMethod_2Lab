using System.Collections.Generic;
using System.Linq;
using FiniteElementMethod_2Lab.FEM.Core;

namespace FiniteElementMethod_2Lab.Geometry;

public class Grid<TPoint>
{
    public TPoint[] Nodes { get; }
    public Element[] Elements { get; }

    public Grid(
        IEnumerable<TPoint> nodes,
        IEnumerable<Element> elements
    )
    {
        Nodes = nodes.ToArray();
        Elements = elements.ToArray();
    }
}