using System;
using System.Collections.Generic;
using System.Reflection;
using FiniteElementMethod_2Lab.FEM.Core.Assembling;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Boundary;
using FiniteElementMethod_2Lab.FEM.OneDimensional.Assembling.Parameters.Providers;
using FiniteElementMethod_2Lab.Geometry;
using SharpMath.EquationsSystem.Solver;
using SharpMath.Matrices;
using SharpMath.Vectors;

namespace FiniteElementMethod_2Lab.FEM.OneDimensional;

public class FEMInfrastructureBuilder
{
    private readonly IMatrixPortraitBuilder<SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ITemplateMatrixProvider _massTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessTemplateProvider;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;
    private Func<double, double, double>? _densityFunction;
    private Func<double, double>? _lambda;
    private double? _sigma;
    private Grid<double>? _grid;
    private double[]? _timeLayers;
    private ConjugateGradientSolver? _SLAESolver;
    private Vector? _initialWeights;
    private readonly List<FirstBoundary> _firstBoundary = new();

    public FEMInfrastructureBuilder()
    {
        _matrixPortraitBuilder = new PortraitBuilder();
        _massTemplateProvider = new MassMatrixTemplateProvider();
        _stiffnessTemplateProvider = new StiffnessMatrixTemplateProvider();
        _inserter = new Inserter();
    }

    public FEMInfrastructureBuilder SetSLAESolver(Func<SLAESolvingConfiguration> solverConfiguration)
    {
        var configuration = solverConfiguration();
        _SLAESolver = new ConjugateGradientSolver(
            configuration.PreconditionFactory, 
            configuration.Precision,
            configuration.MaxIteration
            );

        return this;
    }

    public FEMInfrastructureBuilder SetLambdaBySolutionDependency(Func<double, double> lambda)
    {
        _lambda = lambda;
        return this;
    }

    public FEMInfrastructureBuilder SetSigma(double sigma)
    {
        _sigma = sigma;
        return this;
    }

    public FEMInfrastructureBuilder SetDensityFunction(Func<double, double, double> densityFunction)
    {
        _densityFunction = densityFunction;
        return this;
    }

    public FEMInfrastructureBuilder SetGrid(Grid<double> grid)
    {
        _grid = grid;
        return this;
    }

    public FEMInfrastructureBuilder SetTimeLayers(double[] timeLayers)
    {
        _timeLayers = timeLayers;
        return this;
    }

    public FEMInfrastructureBuilder SetInitialWeights(Vector weights)
    {
        _initialWeights = weights;
        return this;
    }
    public FEMInfrastructureBuilder SetInitialWeights(params double[] weights)
    {
        _initialWeights = new Vector(weights);
        return this;
    }

    public FEMInfrastructureBuilder SetFirstBoundary(int nodeIndex, Func<double, double> u)
    {
        _firstBoundary.Add(new FirstBoundary
        {
            Node = nodeIndex,
            ValueFromTime = u
        });

        return this;
    }

    public FEMInfrastructure Build()
    {
        EnsureAllFieldNotNull();
        
        var densityFunction = new TimeRelatedFunctionalProvider(_grid!, _densityFunction!);
        var sigma = new FixedValueProvider((double)_sigma!);

        if (_initialWeights!.Length != _grid!.Nodes.Length)
            throw new InvalidOperationException();

        return new FEMInfrastructure(
            matrixPortraitBuilder: _matrixPortraitBuilder,
            massTemplateProvider: _massTemplateProvider,
            stiffnessTemplateProvider: _stiffnessTemplateProvider,
            inserter: _inserter,
            grid: _grid!,
            densityFunction: densityFunction,
            sigma: sigma,
            timeLayers: _timeLayers!,
            SLAESolver: _SLAESolver!,
            initialWeights: _initialWeights,
            lambda: _lambda!,
            firstBoundary: _firstBoundary.ToArray()
        );
    }


    private void EnsureAllFieldNotNull()
    {
        var type = GetType();
        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (field.GetValue(this) is null)
                throw new InvalidOperationException($"field {field.Name} must be initialized");

        if (_firstBoundary.Count > 2)
            throw new InvalidOperationException();
    }
}