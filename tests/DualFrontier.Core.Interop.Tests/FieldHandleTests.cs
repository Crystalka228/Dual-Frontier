using System;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public sealed class FieldHandleTests : IDisposable
{
    private readonly NativeWorld _world = new();

    public void Dispose() => _world.Dispose();

    [Fact]
    public void ReadCell_DefaultZero()
    {
        var f = _world.Fields.Register<float>("rc.zero", 4, 4);
        f.ReadCell(2, 2).Should().Be(0f);
    }

    [Fact]
    public void WriteCell_Then_ReadCell_RoundTrip()
    {
        var f = _world.Fields.Register<float>("rc.rt", 4, 4);
        f.WriteCell(2, 2, 42.5f);
        f.ReadCell(2, 2).Should().Be(42.5f);
    }

    [Fact]
    public void ReadCell_OutOfBounds_Throws()
    {
        var f = _world.Fields.Register<float>("rc.oob", 3, 3);
        Action a1 = () => f.ReadCell(-1, 0);
        Action a2 = () => f.ReadCell(3, 0);
        Action a3 = () => f.ReadCell(0, 3);
        a1.Should().Throw<FieldOperationFailedException>();
        a2.Should().Throw<FieldOperationFailedException>();
        a3.Should().Throw<FieldOperationFailedException>();
    }

    [Fact]
    public void AcquireSpan_ProvidesReadOnlyView()
    {
        var f = _world.Fields.Register<float>("sp.view", 3, 3);
        f.WriteCell(1, 1, 5f);

        using var lease = f.AcquireSpan();
        lease.Width.Should().Be(3);
        lease.Height.Should().Be(3);
        lease[1, 1].Should().Be(5f);
        lease.Span.Length.Should().Be(9);
    }

    [Fact]
    public void WriteCell_DuringActiveSpan_Throws()
    {
        var f = _world.Fields.Register<float>("sp.reject", 3, 3);
        using var lease = f.AcquireSpan();

        Action act = () => f.WriteCell(0, 0, 1f);
        act.Should().Throw<FieldOperationFailedException>();
    }

    [Fact]
    public void WriteCell_AfterSpanRelease_Succeeds()
    {
        var f = _world.Fields.Register<float>("sp.after", 3, 3);
        using (var lease = f.AcquireSpan()) { /* hold then release */ }

        f.WriteCell(0, 0, 7f);
        f.ReadCell(0, 0).Should().Be(7f);
    }

    [Fact]
    public void Conductivity_DefaultOne()
    {
        var f = _world.Fields.Register<float>("c.def", 3, 3);
        f.GetConductivity(1, 1).Should().Be(1.0f);
    }

    [Fact]
    public void SetConductivity_Then_Get()
    {
        var f = _world.Fields.Register<float>("c.set", 3, 3);
        f.SetConductivity(1, 1, 0.25f);
        f.GetConductivity(1, 1).Should().Be(0.25f);
    }

    [Fact]
    public void StorageFlag_DefaultFalse()
    {
        var f = _world.Fields.Register<float>("s.def", 3, 3);
        f.GetStorageFlag(1, 1).Should().BeFalse();
    }

    [Fact]
    public void StorageFlag_Toggle()
    {
        var f = _world.Fields.Register<float>("s.tog", 3, 3);
        f.SetStorageFlag(1, 1, true);
        f.GetStorageFlag(1, 1).Should().BeTrue();
        f.SetStorageFlag(1, 1, false);
        f.GetStorageFlag(1, 1).Should().BeFalse();
    }

    [Fact]
    public void SwapBuffers_PrimaryBecomesBack()
    {
        var f = _world.Fields.Register<float>("sw.test", 2, 2);
        f.WriteCell(0, 0, 1f);
        f.WriteCell(1, 1, 9f);
        f.SwapBuffers();

        f.ReadCell(0, 0).Should().Be(0f);
        f.ReadCell(1, 1).Should().Be(0f);
    }

    [Fact]
    public void IntField_RoundTrip()
    {
        var f = _world.Fields.Register<int>("i.rt", 3, 3);
        f.WriteCell(1, 1, 12345);
        f.ReadCell(1, 1).Should().Be(12345);
    }
}
