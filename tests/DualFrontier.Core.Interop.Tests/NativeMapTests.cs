using System.Collections.Generic;
using System.Linq;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeMapTests
{
    private const uint MapId = 0xA10001u;

    [Fact]
    public void Set_Get_Roundtrips()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.Set(1u, 100).Should().BeTrue();
        map.Set(2u, 200).Should().BeTrue();

        map.TryGet(1u, out int v1).Should().BeTrue();
        v1.Should().Be(100);
        map.TryGet(2u, out int v2).Should().BeTrue();
        v2.Should().Be(200);
    }

    [Fact]
    public void Set_ExistingKey_Updates_ReturnsFalse()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.Set(1u, 100);
        map.Set(1u, 999).Should().BeFalse();

        map.Count.Should().Be(1);
        map.TryGet(1u, out int v).Should().BeTrue();
        v.Should().Be(999);
    }

    [Fact]
    public void TryGet_MissingKey_ReturnsFalse()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.TryGet(42u, out int v).Should().BeFalse();
        v.Should().Be(0);
    }

    [Fact]
    public void Remove_PresentKey_DropsEntry()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.Set(1u, 10);
        map.Set(2u, 20);
        map.Remove(2u).Should().BeTrue();

        map.Count.Should().Be(1);
        map.TryGet(2u, out _).Should().BeFalse();
    }

    [Fact]
    public void Remove_MissingKey_ReturnsFalse()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.Remove(99u).Should().BeFalse();
    }

    [Fact]
    public void Iterate_ReturnsSortedByKey_RegardlessOfInsertionOrder()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        // Insert in shuffled order; iteration must come back sorted.
        map.Set(5u, 50);
        map.Set(1u, 10);
        map.Set(3u, 30);

        var keys = new uint[8];
        var values = new int[8];
        int n = map.Iterate(keys, values);

        n.Should().Be(3);
        keys.Take(n).Should().Equal(new uint[] { 1u, 3u, 5u });
        values.Take(n).Should().Equal(new int[] { 10, 30, 50 });
    }

    [Fact]
    public void ToList_Equivalence_VsDictionary_OnGetSetRemove()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);
        var dict = new Dictionary<uint, int>();

        // A small workload exercising the same operations against both.
        map.Set(7u, 77); dict[7u] = 77;
        map.Set(2u, 22); dict[2u] = 22;
        map.Set(7u, 777); dict[7u] = 777;
        map.Remove(2u); dict.Remove(2u);
        map.Set(11u, 110); dict[11u] = 110;

        map.Count.Should().Be(dict.Count);
        var snap = map.ToList().ToDictionary(kv => kv.Key, kv => kv.Value);
        snap.Should().BeEquivalentTo(dict);
    }

    [Fact]
    public void Iterate_BufferTooSmall_ReturnsClippedCount()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        for (uint k = 1; k <= 5; k++) map.Set(k, (int)k * 10);

        var keys = new uint[3];
        var values = new int[3];
        int n = map.Iterate(keys, values);

        n.Should().Be(3);
        keys.Should().Equal(new uint[] { 1u, 2u, 3u });
    }

    [Fact]
    public void Clear_DropsAllEntries_ReturnsPreviousCount()
    {
        using var world = new NativeWorld();
        var map = world.GetKeyedMap<uint, int>(MapId);

        map.Set(1u, 1); map.Set(2u, 2); map.Set(3u, 3);
        int prev = map.Clear();

        prev.Should().Be(3);
        map.Count.Should().Be(0);
    }

    [Fact]
    public void GetKeyedMap_SameId_ReturnsSameBackingStore()
    {
        using var world = new NativeWorld();
        var first = world.GetKeyedMap<uint, int>(MapId);
        var second = world.GetKeyedMap<uint, int>(MapId);

        first.Set(42u, 4242);
        second.TryGet(42u, out int v).Should().BeTrue();
        v.Should().Be(4242);
    }
}
