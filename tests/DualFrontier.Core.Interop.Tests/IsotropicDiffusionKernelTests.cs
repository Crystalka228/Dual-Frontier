using System;
using DualFrontier.Core.Interop.CpuKernels;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class IsotropicDiffusionKernelTests : IDisposable
{
    private readonly NativeWorld _world = new();
    public void Dispose() => _world.Dispose();

    [Fact]
    public void EmptyField_RemainsZero()
    {
        var f = _world.Fields.Register<float>("d.empty", 5, 5);
        IsotropicDiffusionKernel.Run(f, IsotropicDiffusionKernel.Parameters.Default, iterations: 5);

        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                f.ReadCell(x, y).Should().Be(0f);
    }

    [Fact]
    public void PointSource_SpreadsToNeighbors()
    {
        var f = _world.Fields.Register<float>("d.spread", 5, 5);
        f.WriteCell(2, 2, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.1f,
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 1);

        f.ReadCell(2, 2).Should().BeLessThan(100f);
        f.ReadCell(1, 2).Should().BeGreaterThan(0f);
        f.ReadCell(3, 2).Should().BeGreaterThan(0f);
        f.ReadCell(2, 1).Should().BeGreaterThan(0f);
        f.ReadCell(2, 3).Should().BeGreaterThan(0f);
        f.ReadCell(1, 1).Should().Be(0f);
    }

    [Fact]
    public void Decay_ReducesValuesOverTime()
    {
        var f = _world.Fields.Register<float>("d.decay", 3, 3);
        f.WriteCell(1, 1, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.0f,
            DecayCoefficient = 0.1f,
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 5);

        f.ReadCell(1, 1).Should().BeLessThan(100f);
        f.ReadCell(1, 1).Should().BeGreaterThan(0f);
    }

    [Fact]
    public void ConservationApproximate_NoDecay_NoBoundaryLoss()
    {
        var f = _world.Fields.Register<float>("d.cons", 5, 5);
        f.WriteCell(2, 2, 100f);

        var p = new IsotropicDiffusionKernel.Parameters
        {
            DiffusionCoefficient = 0.05f,
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f
        };
        IsotropicDiffusionKernel.Run(f, p, iterations: 3);

        float total = 0;
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                total += f.ReadCell(x, y);

        // With reflective boundary (edge cells use self as neighbour),
        // mass is approximately conserved within a small numerical tolerance.
        total.Should().BeApproximately(100f, 1f);
    }
}
