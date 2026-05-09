using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeCompositeTests
{
    private const uint CompositeId = 0xC10001u;

    [Fact]
    public void Add_GetAt_Roundtrips()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(entity, 42).Should().BeTrue();

        comp.CountFor(entity).Should().Be(1);
        comp.TryGetAt(entity, 0, out int value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void Iterate_PreservesInsertionOrder()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(entity, 10);
        comp.Add(entity, 20);
        comp.Add(entity, 30);

        var buffer = new int[8];
        int n = comp.Iterate(entity, buffer);

        n.Should().Be(3);
        buffer[..n].Should().Equal(new[] { 10, 20, 30 });
    }

    [Fact]
    public void RemoveAt_SwapsWithLast_DoesNotPreserveOrder()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(entity, 1);
        comp.Add(entity, 2);
        comp.Add(entity, 3);
        comp.RemoveAt(entity, 0).Should().BeTrue();

        comp.CountFor(entity).Should().Be(2);
        var buffer = new int[8];
        int n = comp.Iterate(entity, buffer);
        n.Should().Be(2);

        // After remove_at(0), the former tail (3) replaces index 0.
        // Index 1 retains its original value (2).
        buffer[..n].Should().Equal(new[] { 3, 2 });
    }

    [Fact]
    public void RemoveAt_OutOfRange_ReturnsFalse()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(entity, 1);
        comp.RemoveAt(entity, 5).Should().BeFalse();
        comp.CountFor(entity).Should().Be(1);
    }

    [Fact]
    public void MultipleEntities_DataIsolated()
    {
        using var world = new NativeWorld();
        EntityId a = world.CreateEntity();
        EntityId b = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(a, 100);
        comp.Add(a, 200);
        comp.Add(b, 999);

        comp.CountFor(a).Should().Be(2);
        comp.CountFor(b).Should().Be(1);
        comp.TryGetAt(b, 0, out int bValue).Should().BeTrue();
        bValue.Should().Be(999);
    }

    [Fact]
    public void ClearFor_DropsParentEntirely()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var comp = world.GetComposite<int>(CompositeId);

        comp.Add(entity, 1);
        comp.Add(entity, 2);
        comp.ClearFor(entity).Should().BeTrue();

        comp.CountFor(entity).Should().Be(0);
        // Subsequent ClearFor on already-empty parent returns false
        // (nothing was erased).
        comp.ClearFor(entity).Should().BeFalse();
    }

    [Fact]
    public void GetComposite_SameId_ReturnsSameBackingStore()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var first = world.GetComposite<int>(CompositeId);
        var second = world.GetComposite<int>(CompositeId);

        first.Add(entity, 42);
        second.CountFor(entity).Should().Be(1);
    }
}
