using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class BootstrapTests
{
#pragma warning disable CS0649
    private struct TestComponent
    {
        public int Value;
    }
#pragma warning restore CS0649

    [Fact]
    public void Bootstrap_returns_ready_NativeWorld()
    {
        using var world = Bootstrap.Run();
        world.Should().NotBeNull();
        world.EntityCount.Should().Be(0);
    }

    [Fact]
    public void Bootstrap_world_supports_entity_creation()
    {
        using var world = Bootstrap.Run();
        EntityId entity = world.CreateEntity();

        world.IsAlive(entity).Should().BeTrue();
        world.EntityCount.Should().Be(1);
    }

    [Fact]
    public void Bootstrap_world_supports_components()
    {
        using var world = Bootstrap.Run();
        EntityId entity = world.CreateEntity();

        var comp = new TestComponent { Value = 42 };
        world.AddComponent(entity, comp);

        TestComponent retrieved = world.GetComponent<TestComponent>(entity);
        retrieved.Value.Should().Be(42);
    }

    [Fact]
    public void Bootstrap_with_registry_uses_deterministic_ids()
    {
        // useRegistry: false — this test constructs its own ComponentTypeRegistry
        // to verify deterministic id assignment in isolation. Post-K8.3+K8.4
        // Bootstrap.Run(useRegistry: true) would create world.Registry alongside,
        // and the two registries would race on id allocation against the same
        // native handle (sequential _nextId both starting at 1) — for this
        // determinism test we want a single registry on the handle.
        using var world1 = Bootstrap.Run(useRegistry: false);
        var registry1 = new ComponentTypeRegistry(world1.HandleForInternalUseTest);
        uint id1 = registry1.Register<TestComponent>();

        using var world2 = Bootstrap.Run(useRegistry: false);
        var registry2 = new ComponentTypeRegistry(world2.HandleForInternalUseTest);
        uint id2 = registry2.Register<TestComponent>();

        // Both registries assign the same first id (1) — sequential per K-L4.
        id1.Should().Be(1);
        id2.Should().Be(1);
    }

    [Fact]
    public void Multiple_independent_bootstraps_supported()
    {
        using var world1 = Bootstrap.Run();
        using var world2 = Bootstrap.Run();

        EntityId e1 = world1.CreateEntity();
        EntityId e2 = world2.CreateEntity();

        world1.EntityCount.Should().Be(1);
        world2.EntityCount.Should().Be(1);

        // Engines are isolated — each has its own state.
        e1.Should().NotBe(default(EntityId));
        e2.Should().NotBe(default(EntityId));
    }

    [Fact]
    public void Bootstrapped_world_disposes_cleanly()
    {
        var world = Bootstrap.Run();
        world.Dispose();

        Action act = () => world.CreateEntity();
        act.Should().Throw<ObjectDisposedException>();
    }
}
