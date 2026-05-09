using System.Linq;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeSetTests
{
    private const uint SetId = 0xD10001u;

    [Fact]
    public void Add_Contains_Roundtrips()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        set.Add(5).Should().BeTrue();
        set.Add(3).Should().BeTrue();

        set.Contains(5).Should().BeTrue();
        set.Contains(3).Should().BeTrue();
        set.Contains(7).Should().BeFalse();
    }

    [Fact]
    public void Add_Duplicate_ReturnsFalse()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        set.Add(42).Should().BeTrue();
        set.Add(42).Should().BeFalse();
        set.Count.Should().Be(1);
    }

    [Fact]
    public void Remove_PresentElement_DropsIt()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        set.Add(10);
        set.Add(20);
        set.Remove(10).Should().BeTrue();

        set.Contains(10).Should().BeFalse();
        set.Count.Should().Be(1);
    }

    [Fact]
    public void Remove_AbsentElement_ReturnsFalse()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        set.Remove(99).Should().BeFalse();
    }

    [Fact]
    public void Iterate_ReturnsSorted_RegardlessOfInsertionOrder()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        // Insert in shuffled order — iteration must come back sorted.
        set.Add(5);
        set.Add(1);
        set.Add(3);

        var buffer = new int[8];
        int n = set.Iterate(buffer);

        n.Should().Be(3);
        buffer.Take(n).Should().Equal(new[] { 1, 3, 5 });
    }

    [Fact]
    public void Iterate_AfterRemove_RemainsSorted()
    {
        using var world = new NativeWorld();
        var set = world.GetSet<int>(SetId);

        set.Add(1); set.Add(2); set.Add(3); set.Add(4);
        set.Remove(2);

        var buffer = new int[8];
        int n = set.Iterate(buffer);

        n.Should().Be(3);
        buffer.Take(n).Should().Equal(new[] { 1, 3, 4 });
    }
}
