using System;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional.Local;

public class DerivativeCalculator
{
    private const double Delta = 1e-7d;

    public static double CalculateDerivative(Func<double, double> function, double point)
    {
        return (function(point + Delta) - function(point - Delta)) / (2.0 * Delta);
    }
}