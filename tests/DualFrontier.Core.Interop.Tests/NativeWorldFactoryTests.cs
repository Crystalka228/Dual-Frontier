using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeWorldFactoryTests
{
    [Fact]
    public void AllocateMapId_FirstCall_ReturnsOne()
    {
        using var world = new NativeWorld();
        world.AllocateMapId().Should().Be(1u);
    }

    [Fact]
    public void AllocateMapId_SuccessiveCalls_AreMonotonic()
    {
        using var world = new NativeWorld();
        uint a = world.AllocateMapId();
        uint b = world.AllocateMapId();
        uint c = world.AllocateMapId();
        b.Should().BeGreaterThan(a);
        c.Should().BeGreaterThan(b);
    }

    [Fact]
    public void AllocateMapAndSetIds_Independent_DoNotInterleave()
    {
        using var world = new NativeWorld();
        uint mapA = world.AllocateMapId();
        uint setA = world.AllocateSetId();
        uint mapB = world.AllocateMapId();
        uint setB = world.AllocateSetId();
        mapA.Should().Be(1u);
        setA.Should().Be(1u);
        mapB.Should().Be(2u);
        setB.Should().Be(2u);
    }

    [Fact]
    public void AllocateCompositeId_Independent_FromMapAndSet()
    {
        using var world = new NativeWorld();
        world.AllocateMapId();
        world.AllocateSetId();
        world.AllocateCompositeId().Should().Be(1u);
    }

    [Fact]
    public void CreateMap_ReturnsDistinctStorage_PerCall()
    {
        using var world = new NativeWorld();
        var a = world.CreateMap<uint, int>();
        var b = world.CreateMap<uint, int>();

        a.Set(42u, 100);
        b.TryGet(42u, out _).Should().BeFalse(
            "second CreateMap call returns wrapper over distinct backing storage");
        a.TryGet(42u, out int found).Should().BeTrue();
        found.Should().Be(100);
    }

    [Fact]
    public void CreateSet_ReturnsDistinctStorage_PerCall()
    {
        using var world = new NativeWorld();
        var a = world.CreateSet<uint>();
        var b = world.CreateSet<uint>();

        a.Add(7u);
        b.Contains(7u).Should().BeFalse();
        a.Contains(7u).Should().BeTrue();
    }

    [Fact]
    public void CreateComposite_ReturnsDistinctStorage_PerCall()
    {
        using var world = new NativeWorld();
        var a = world.CreateComposite<int>();
        var b = world.CreateComposite<int>();
        a.CompositeId.Should().NotBe(b.CompositeId);
    }

    [Fact]
    public void CreateMap_WithInternedStringKey_Compiles()
    {
        // Smoke test: confirms NativeMap<InternedString, V> instantiates after
        // IComparable<InternedString> was added. This is the critical surface
        // for StorageComponent.Items in Phase 2.B.5.
        using var world = new NativeWorld();
        var map = world.CreateMap<InternedString, int>();
        InternedString key = world.InternString("wood");
        map.Set(key, 42);
        map.TryGet(key, out int value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void CreateSet_WithInternedStringElement_Compiles()
    {
        using var world = new NativeWorld();
        var set = world.CreateSet<InternedString>();
        InternedString element = world.InternString("steel");
        set.Add(element);
        set.Contains(element).Should().BeTrue();
    }
}
