using DualFrontier.Components.Building;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// K8.2 v2 Phase 2.B.5 — round-trip and IsFull/TotalQuantity semantics for
/// the post-conversion <see cref="StorageComponent"/> struct + NativeMap +
/// NativeSet shape. Capacity has no implicit default after the conversion;
/// AcceptAll's default is also caller-supplied. Tests cover the construction
/// pattern that production factories must follow.
/// </summary>
public sealed class StorageComponentTests
{
    [Fact]
    public void Default_ItemsInvalid_NotFull_ZeroQuantity()
    {
        StorageComponent component = default;
        component.Items.IsValid.Should().BeFalse();
        component.AllowedItems.IsValid.Should().BeFalse();
        component.IsFull.Should().BeFalse();
        component.TotalQuantity.Should().Be(0);
    }

    [Fact]
    public void RoundTrip_PopulatedItemsResolveToContent()
    {
        using var world = new NativeWorld();
        var component = new StorageComponent
        {
            Capacity = 20,
            Items = world.CreateMap<InternedString, int>(),
            AcceptAll = true,
            AllowedItems = world.CreateSet<InternedString>(),
        };
        InternedString wood = world.InternString("wood");
        InternedString steel = world.InternString("steel");
        component.Items.Set(wood, 50);
        component.Items.Set(steel, 12);

        component.Items.TryGet(wood, out int woodQty).Should().BeTrue();
        woodQty.Should().Be(50);
        component.Items.TryGet(steel, out int steelQty).Should().BeTrue();
        steelQty.Should().Be(12);
        component.TotalQuantity.Should().Be(62);
    }

    [Fact]
    public void IsFull_TriggersAtCapacity()
    {
        using var world = new NativeWorld();
        var component = new StorageComponent
        {
            Capacity = 2,
            Items = world.CreateMap<InternedString, int>(),
        };
        component.IsFull.Should().BeFalse();

        component.Items.Set(world.InternString("a"), 1);
        component.IsFull.Should().BeFalse();

        component.Items.Set(world.InternString("b"), 1);
        component.IsFull.Should().BeTrue("Items.Count == Capacity");
    }

    [Fact]
    public void AllowedItems_NativeSetWhitelist_AddAndContains()
    {
        using var world = new NativeWorld();
        var component = new StorageComponent
        {
            Capacity = 5,
            Items = world.CreateMap<InternedString, int>(),
            AcceptAll = false,
            AllowedItems = world.CreateSet<InternedString>(),
        };
        InternedString steel = world.InternString("steel");
        component.AllowedItems.Add(steel);

        component.AllowedItems.Contains(steel).Should().BeTrue();
        component.AllowedItems.Contains(world.InternString("wood")).Should().BeFalse();
    }

    [Fact]
    public void TotalQuantity_EmptyMap_IsZero()
    {
        using var world = new NativeWorld();
        var component = new StorageComponent
        {
            Capacity = 10,
            Items = world.CreateMap<InternedString, int>(),
        };
        component.TotalQuantity.Should().Be(0);
    }

    [Fact]
    public void DistinctStorages_DoNotShareItemBacking()
    {
        using var world = new NativeWorld();
        var a = new StorageComponent
        {
            Capacity = 5,
            Items = world.CreateMap<InternedString, int>(),
        };
        var b = new StorageComponent
        {
            Capacity = 5,
            Items = world.CreateMap<InternedString, int>(),
        };

        InternedString wood = world.InternString("wood");
        a.Items.Set(wood, 100);
        b.Items.TryGet(wood, out _).Should().BeFalse();
    }
}
