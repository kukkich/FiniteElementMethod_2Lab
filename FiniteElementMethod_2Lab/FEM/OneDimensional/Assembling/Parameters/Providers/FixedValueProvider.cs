using FiniteElementMethod_2Lab.FEM.Core.Parameters;
using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;

public class FixedValueProvider : IAttachedToElementParameterProvider<double>, IFunctionalParameter<double>
{
    private readonly double _value;

    public FixedValueProvider(double value = 1d)
    {
        _value = value;
    }

    public double GetById(int id)
    {
        return _value;
    }

    public double Calculate(int nodeIndex)
    {
        return _value;
    }

    public double CalculateDerivative(double point)
    {
        throw new NotImplementedException();
    }
}