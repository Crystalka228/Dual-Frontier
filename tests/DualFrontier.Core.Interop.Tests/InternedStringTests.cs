using System;
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

    [Fact]
    public void EmptyString_RoundTrip_YieldsEmptySentinel()
    {
        using var world = new NativeWorld();

        InternedString empty = world.InternString("");
        empty.IsEmpty.Should().BeTrue("intern of empty content yields the empty sentinel");
        empty.Id.Should().Be(0u);
        empty.Generation.Should().Be(0u);

        empty.Should().Be(default(InternedString));
        (empty == default).Should().BeTrue();

        empty.Resolve(world).Should().BeNull("empty sentinel has no content to resolve");

        int countAfterEmpty = world.StringPoolCount;
        countAfterEmpty.Should().Be(0, "string pool count is unchanged after empty intern");
    }

    [Fact]
    public void EqualsByContent_ReturnsTrueForBothEmpty_RegardlessOfWorld()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString emptyA = worldA.InternString("");
        InternedString emptyB = worldB.InternString("");

        emptyA.EqualsByContent(emptyB, worldA, worldB).Should().BeTrue(
            "two empty InternedStrings are equal by content irrespective of world");
        emptyA.EqualsByContent(emptyA, worldA, worldA).Should().BeTrue(
            "an empty InternedString equals itself by content trivially");
    }

    [Fact]
    public void EqualsByContent_ReturnsFalseWhenOnlyOneSideIsEmpty()
    {
        using var world = new NativeWorld();

        InternedString empty = world.InternString("");
        InternedString nonEmpty = world.InternString("Foo");

        empty.EqualsByContent(nonEmpty, world, world).Should().BeFalse();
        nonEmpty.EqualsByContent(empty, world, world).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_SamePool_FastPathReturnsTrueOnEqualIds()
    {
        using var world = new NativeWorld();

        InternedString a = world.InternString("Foo");
        InternedString b = world.InternString("Foo");

        a.Equals(b).Should().BeTrue("same-pool intern of same content returns equal id pair");
        a.EqualsByContent(b, world, world).Should().BeTrue(
            "EqualsByContent agrees with == on the same-pool fast path");
    }

    [Fact]
    public void EqualsByContent_SamePool_DifferentContentReturnsFalse()
    {
        using var world = new NativeWorld();

        InternedString foo = world.InternString("Foo");
        InternedString bar = world.InternString("Bar");

        foo.EqualsByContent(bar, world, world).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_CrossPool_ReturnsTrueForIdenticalContent()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString fromA = worldA.InternString("SharedContent");
        InternedString fromB = worldB.InternString("SharedContent");

        fromA.EqualsByContent(fromB, worldA, worldB).Should().BeTrue(
            "cross-pool comparison by content returns true when content matches");
    }

    [Fact]
    public void EqualsByContent_CrossPool_ReturnsFalseForDifferentContent()
    {
        using var worldA = new NativeWorld();
        using var worldB = new NativeWorld();

        InternedString fromA = worldA.InternString("ContentA");
        InternedString fromB = worldB.InternString("ContentB");

        fromA.EqualsByContent(fromB, worldA, worldB).Should().BeFalse();
    }

    [Fact]
    public void EqualsByContent_NullWorlds_Throws()
    {
        using var world = new NativeWorld();

        InternedString s = world.InternString("Foo");

        Action callWithNullThis = () => s.EqualsByContent(s, null!, world);
        Action callWithNullOther = () => s.EqualsByContent(s, world, null!);

        callWithNullThis.Should().Throw<ArgumentNullException>().WithParameterName("thisWorld");
        callWithNullOther.Should().Throw<ArgumentNullException>().WithParameterName("otherWorld");
    }

    [Fact]
    public void EqualsByContent_StaleGeneration_ReturnsFalse()
    {
        using var world = new NativeWorld();

        // Intern under ModA scope. The fresh-lookup sanity check stays inside
        // the same scope: re-interning after EndModScope would add the id to
        // the empty/core scope's reference list, pinning it across the
        // subsequent ClearModScope and defeating the reclaim under test.
        world.BeginModScope("ModA");
        InternedString underModA = world.InternString("ModAExclusiveContent");
        InternedString freshLookup = world.InternString("ModAExclusiveContent");
        underModA.Equals(freshLookup).Should().BeTrue(
            "within the same scope, intern of identical content returns the same (id, gen)");
        world.EndModScope("ModA");

        // Clear ModA — the id is uniquely owned by ModA, so the slot is
        // reclaimed and current_generation advances.
        world.ClearModScope("ModA");

        // Re-intern the same content. The reclaimed slot is reused but with
        // the bumped generation tag, so the pre-clear handle is now stale.
        InternedString postClear = world.InternString("ModAExclusiveContent");

        underModA.Equals(postClear).Should().BeFalse(
            "post-clear re-intern advances the generation tag");

        underModA.EqualsByContent(postClear, world, world).Should().BeFalse(
            "stale generation makes content resolution null; comparison is false");
    }
}
