using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class InternedStringTests
{
    [Fact]
    public void Intern_Roundtrip_ReturnsOriginalContent()
    {
        using var world = new NativeWorld();
        InternedString id = world.InternString("Faction.Settlers");

        id.IsEmpty.Should().BeFalse();
        id.Resolve(world).Should().Be("Faction.Settlers");
    }

    [Fact]
    public void Intern_DedupSameContent_ReturnsSameId()
    {
        using var world = new NativeWorld();
        InternedString a = world.InternString("Recipe.Bread");
        InternedString b = world.InternString("Recipe.Bread");

        a.Should().Be(b);
        a.Id.Should().Be(b.Id);
        a.Generation.Should().Be(b.Generation);
    }

    [Fact]
    public void EmptyContent_ReturnsEmptySentinel()
    {
        using var world = new NativeWorld();
        InternedString id = world.InternString(string.Empty);

        id.IsEmpty.Should().BeTrue();
        id.Resolve(world).Should().BeNull();
    }

    [Fact]
    public void DefaultStruct_IsEmpty()
    {
        InternedString empty = default;
        empty.IsEmpty.Should().BeTrue();
        empty.Id.Should().Be(0u);
    }

    [Fact]
    public void Equality_DifferentIds_NotEqual()
    {
        using var world = new NativeWorld();
        InternedString a = world.InternString("A");
        InternedString b = world.InternString("B");

        (a == b).Should().BeFalse();
        a.Should().NotBe(b);
    }

    [Fact]
    public void CrossModSharing_SameContent_ReturnsSameId()
    {
        using var world = new NativeWorld();
        world.BeginModScope("ModA");
        InternedString a = world.InternString("Shared.Item");
        world.EndModScope("ModA");

        world.BeginModScope("ModB");
        InternedString b = world.InternString("Shared.Item");
        world.EndModScope("ModB");

        a.Id.Should().Be(b.Id);
    }

    [Fact]
    public void ClearModScope_StaleResolution_ReturnsNull()
    {
        using var world = new NativeWorld();
        world.BeginModScope("ModX");
        InternedString staleId = world.InternString("ModXOnly.Recipe");
        world.EndModScope("ModX");

        // Snapshot the (id, gen) pair, then clear the scope. The id is
        // uniquely owned by ModX so the slot is reclaimed and gen bumps.
        world.ClearModScope("ModX");

        staleId.Resolve(world).Should().BeNull();
    }
}
