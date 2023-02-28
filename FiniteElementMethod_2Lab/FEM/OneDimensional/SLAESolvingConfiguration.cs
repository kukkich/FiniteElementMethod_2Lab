using SharpMath.EquationsSystem.Preconditions;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class SLAESolvingConfiguration
{
    public required IPreconditionerFactory PreconditionFactory { get; init; }
    public required double Precision { get; init; }
    public required int MaxIteration { get; init; }
}