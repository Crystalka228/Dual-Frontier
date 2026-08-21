using System;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class ComponentTypeRegistryTests
{
    // Placeholder types exercise registration semantics by identity only —
    // the fields are not read in any test.
#pragma warning disable CS0649
    private struct TypeA { public int Value; }
    private struct TypeB { public long Value; }
    private struct TypeC { public byte Value; }
#pragma warning restore CS0649

    [Fact]
    public void Register_returns_sequential_ids_starting_from_1()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint idA = registry.Register<TypeA>();
        uint idB = registry.Register<TypeB>();
        uint idC = registry.Register<TypeC>();

        idA.Should().Be(1);
        idB.Should().Be(2);
        idC.Should().Be(3);
    }

    [Fact]
    public void Register_is_idempotent()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint first = registry.Register<TypeA>();
        uint second = registry.Register<TypeA>();
        uint third = registry.Register<TypeA>();

        first.Should().Be(second).And.Be(third);
    }

    [Fact]
    public void GetId_throws_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        Action act = () => registry.GetId<TypeA>();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not registered*");
    }

    [Fact]
    public void TryGetId_returns_false_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        bool found = registry.TryGetId<TypeA>(out uint id);

        found.Should().BeFalse();
        id.Should().Be(0);
    }

    [Fact]
    public void TryGetId_returns_true_after_register()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint registered = registry.Register<TypeA>();
        bool found = registry.TryGetId<TypeA>(out uint id);

        found.Should().BeTrue();
        id.Should().Be(registered);
    }

    [Fact]
    public void Lookup_returns_identity_for_registered_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint id = registry.Register<TypeA>();
        ComponentIdentity? identity = registry.Lookup(id);

        // The reverse map answers with the owner-scoped identity, not a Type: holding a
        // Type here is exactly what rooted a mod's collectible ALC before ID-A.
        identity.Should().NotBeNull();
        identity!.Value.Owner.Should().Be(ComponentTypeRegistry.KernelOwner);
        identity.Value.TypeFullName.Should().Be(typeof(TypeA).FullName);
    }

    [Fact]
    public void Lookup_returns_null_for_unassigned_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        ComponentIdentity? identity = registry.Lookup(999);

        identity.Should().BeNull();
    }

    [Fact]
    public void Count_reflects_registered_types()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.Count.Should().Be(0);
        registry.Register<TypeA>();
        registry.Count.Should().Be(1);
        registry.Register<TypeB>();
        registry.Count.Should().Be(2);
        registry.Register<TypeA>();  // idempotent
        registry.Count.Should().Be(2);
    }

    [Fact]
    public void IsRegistered_reflects_registration_state()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.IsRegistered<TypeA>().Should().BeFalse();
        registry.Register<TypeA>();
        registry.IsRegistered<TypeA>().Should().BeTrue();
        registry.IsRegistered<TypeB>().Should().BeFalse();
    }
}
