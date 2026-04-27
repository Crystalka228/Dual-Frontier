using DualFrontier.Contracts.Core;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.ECS;

public sealed class EntityIdTests
{
    // ── Equality ────────────────────────────────────────────────────────────

    [Fact]
    public void Equal_when_same_index_and_version()
    {
        var a = new EntityId(3, 1);
        var b = new EntityId(3, 1);
        a.Should().Be(b);
    }

    [Fact]
    public void Not_equal_when_same_index_different_version()
    {
        var live = new EntityId(3, 1);
        var stale = new EntityId(3, 0);
        live.Should().NotBe(stale);
    }

    [Fact]
    public void Not_equal_when_different_index_same_version()
    {
        var a = new EntityId(3, 1);
        var b = new EntityId(7, 1);
        a.Should().NotBe(b);
    }

    // ── HashCode ─────────────────────────────────────────────────────────

    [Fact]
    public void HashCode_equal_for_equal_ids()
    {
        var a = new EntityId(3, 1);
        var b = new EntityId(3, 1);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void HashCode_differs_for_different_version()
    {
        // record struct generates HashCode.Combine(Index, Version) — collision possible
        // but same-slot stale vs live must differ in version → almost certainly different hash
        var live = new EntityId(3, 2);
        var stale = new EntityId(3, 1);
        live.GetHashCode().Should().NotBe(stale.GetHashCode());
    }

    [Fact]
    public void Can_be_used_as_dictionary_key()
    {
        var dict = new Dictionary<EntityId, string>();
        var id = new EntityId(5, 0);
        dict[id] = "pawn";
        dict[id].Should().Be("pawn");
    }

    // ── Invalid / IsValid ────────────────────────────────────────────────

    [Fact]
    public void Default_is_Invalid()
    {
        var defaultId = default(EntityId);
        defaultId.Should().Be(EntityId.Invalid);
    }

    [Fact]
    public void Invalid_IsValid_returns_false()
    {
        EntityId.Invalid.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Zero_index_nonzero_version_is_valid()
    {
        // Index=0 is allowed for real entities; only (0,0) is the sentinel
        var id = new EntityId(0, 1);
        id.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nonzero_index_zero_version_is_valid()
    {
        var id = new EntityId(1, 0);
        id.IsValid.Should().BeTrue();
    }

    // ── Versioning invariant ─────────────────────────────────────────────

    [Fact]
    public void Stale_reference_detected_by_version_comparison()
    {
        // Simulates what World.IsAlive does:
        // after DestroyEntity, the slot's version in the registry increments.
        // A cached id with old version no longer matches → stale.
        var cachedBeforeDestroy = new EntityId(3, 0);
        const int slotVersionAfterDestroy = 1;

        bool isAlive = cachedBeforeDestroy.Version == slotVersionAfterDestroy;
        isAlive.Should().BeFalse("a stale reference must be invalid after the entity is destroyed");
    }

    [Fact]
    public void Fresh_reference_matches_current_slot_version()
    {
        var freshId = new EntityId(3, 1);
        const int slotVersionCurrent = 1;

        bool isAlive = freshId.Version == slotVersionCurrent;
        isAlive.Should().BeTrue();
    }
}
