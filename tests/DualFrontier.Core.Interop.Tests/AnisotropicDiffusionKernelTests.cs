using System;
using DualFrontier.Core.Interop.CpuKernels;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class AnisotropicDiffusionKernelTests : IDisposable
{
    private readonly NativeWorld _world = new();
    public void Dispose() => _world.Dispose();

    [Fact]
    public void Run_NullField_Throws()
    {
        Action act = () => AnisotropicDiffusionKernel.Run(null!, AnisotropicDiffusionKernel.Parameters.Default, iterations: 1);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Run_ZeroIterations_NoChange()
    {
        var f = _world.Fields.Register<float>("aniso.noop", 5, 5);
        f.WriteCell(2, 2, 100f);
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                f.SetConductivity(x, y, 1.0f);

        AnisotropicDiffusionKernel.Run(f, AnisotropicDiffusionKernel.Parameters.Default, iterations: 0);

        f.ReadCell(2, 2).Should().Be(100f);
    }

    [Fact]
    public void Run_UniformConductivity_SpreadsRadially()
    {
        var f = _world.Fields.Register<float>("aniso.uniform", 7, 7);
        f.WriteCell(3, 3, 100f);
        for (int y = 0; y < 7; y++)
            for (int x = 0; x < 7; x++)
                f.SetConductivity(x, y, 0.1f);

        var p = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f
        };
        AnisotropicDiffusionKernel.Run(f, p, iterations: 1);

        f.ReadCell(3, 3).Should().BeLessThan(100f);
        f.ReadCell(2, 3).Should().BeApproximately(f.ReadCell(4, 3), 0.0001f);
        f.ReadCell(3, 2).Should().BeApproximately(f.ReadCell(3, 4), 0.0001f);
        f.ReadCell(2, 3).Should().BeApproximately(f.ReadCell(3, 2), 0.0001f);
        f.ReadCell(0, 0).Should().Be(0f);
    }

    [Fact]
    public void Run_WirePath_ChannelsPropagation()
    {
        var f = _world.Fields.Register<float>("aniso.wire", 9, 9);
        f.WriteCell(0, 4, 100f);
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                f.SetConductivity(x, y, y == 4 ? 10.0f : 0.01f);
            }
        }

        var p = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = 0.0f,
            DeltaTime = 0.01f
        };
        AnisotropicDiffusionKernel.Run(f, p, iterations: 3);

        float onWire = f.ReadCell(3, 4);
        float offWire = f.ReadCell(3, 1);

        onWire.Should().BeGreaterThan(0f);
        onWire.Should().BeGreaterThan(offWire * 10f, "wire row channels flow with D=10 vs off-wire D=0.01");
    }

    [Fact]
    public void Run_InsulatorBlocksFlow()
    {
        var f = _world.Fields.Register<float>("aniso.insulator", 9, 9);
        f.WriteCell(0, 4, 100f);
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (x == 4)
                    f.SetConductivity(x, y, 0.0f);
                else
                    f.SetConductivity(x, y, 1.0f);
            }
        }

        var p = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = 0.0f,
            DeltaTime = 0.1f
        };
        AnisotropicDiffusionKernel.Run(f, p, iterations: 10);

        f.ReadCell(0, 4).Should().BeGreaterThan(0f);
        f.ReadCell(5, 4).Should().Be(0f, "insulator column x=4 blocks all flow к x>4");
        f.ReadCell(6, 4).Should().Be(0f);
        f.ReadCell(7, 4).Should().Be(0f);
        f.ReadCell(8, 4).Should().Be(0f);
    }

    [Fact]
    public void Run_DecayOnly_ReducesValue()
    {
        var f = _world.Fields.Register<float>("aniso.decay", 3, 3);
        f.WriteCell(1, 1, 100f);
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
                f.SetConductivity(x, y, 0.0f);

        var p = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = 0.1f,
            DeltaTime = 1.0f
        };
        AnisotropicDiffusionKernel.Run(f, p, iterations: 1);

        f.ReadCell(1, 1).Should().BeApproximately(90f, 0.001f);
        f.ReadCell(0, 0).Should().Be(0f);
    }

    [Fact]
    public void Run_AsymmetricFlowRule_MinConductivity()
    {
        var f = _world.Fields.Register<float>("aniso.asym", 3, 1);
        f.WriteCell(0, 0, 100f);
        f.SetConductivity(0, 0, 10.0f);
        f.SetConductivity(1, 0, 0.1f);
        f.SetConductivity(2, 0, 10.0f);

        var p = new AnisotropicDiffusionKernel.Parameters
        {
            DecayCoefficient = 0.0f,
            DeltaTime = 1.0f
        };
        AnisotropicDiffusionKernel.Run(f, p, iterations: 1);

        f.ReadCell(0, 0).Should().BeApproximately(90f, 0.001f);
        f.ReadCell(1, 0).Should().BeApproximately(10f, 0.001f);
        f.ReadCell(2, 0).Should().Be(0f, "middle cell не propagates to (2,0) в single iteration");
    }
}
