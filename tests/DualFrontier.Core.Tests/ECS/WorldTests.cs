using Xunit;
using FluentAssertions;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Tests.Fixtures;

namespace DualFrontier.Core.Tests.ECS;

public sealed class WorldTests
{
    // TEST 1 — CreateEntity_returns_valid_entity
    [Fact]
    public void CreateEntity_returns_valid_entity()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        id.IsValid.Should().BeTrue();
        id.Index.Should().BeGreaterThan(0);
    }

    // TEST 2 — CreateEntity_returns_unique_entities
    [Fact]
    public void CreateEntity_returns_unique_entities()
    {
        var world = new ManagedTestWorld();
        var a = world.CreateEntity();
        var b = world.CreateEntity();
        a.Should().NotBe(b);
    }

    // TEST 3 — IsAlive_returns_true_for_live_entity
    [Fact]
    public void IsAlive_returns_true_for_live_entity()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.IsAlive(id).Should().BeTrue();
    }

    // TEST 4 — IsAlive_returns_false_after_destroy
    [Fact]
    public void IsAlive_returns_false_after_destroy()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.DestroyEntity(id);
        world.IsAlive(id).Should().BeFalse();
    }

    // TEST 5 — DestroyEntity_is_noop_for_stale_reference
    [Fact]
    public void DestroyEntity_is_noop_for_stale_reference()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.DestroyEntity(id);
        // Use Action to wrap the call for checking if it throws an exception on stale reference
        Action act = () => world.DestroyEntity(id); 
        act.Should().NotThrow();
    }

    // TEST 6 — FlushDestroyedEntities_recycles_slot
    [Fact]
    public void FlushDestroyedEntities_recycles_slot()
    {
        var world = new ManagedTestWorld();
        var first = world.CreateEntity();
        world.DestroyEntity(first);
        world.FlushDestroyedEntities();
        var recycled = world.CreateEntity();
        // Check if index was reused and version incremented
        recycled.Index.Should().Be(first.Index);
        recycled.Version.Should().Be(first.Version + 1);
    }

    // TEST 7 — AddComponent_and_HasComponent
    [Fact]
    public void AddComponent_and_HasComponent()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent { Value = 42 });
        world.HasComponent<TestComponent>(id).Should().BeTrue();
    }

    // TEST 8 — TryGetComponent_returns_correct_value
    [Fact]
    public void TryGetComponent_returns_correct_value()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent { Value = 99 });
        bool found = world.TryGetComponent<TestComponent>(id, out var comp);
        found.Should().BeTrue();
        comp.Value.Should().Be(99);
    }

    // TEST 9 — FlushDestroyedEntities_removes_components
    [Fact]
    public void FlushDestroyedEntities_removes_components()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent { Value = 1 });
        world.DestroyEntity(id);
        world.FlushDestroyedEntities();
        // Check if the component is gone after flush/destroy cycle
        world.HasComponent<TestComponent>(id).Should().BeFalse();
    }

    // TEST 10 — RemoveComponent_removes_only_that_component
    [Fact]
    public void RemoveComponent_removes_only_that_component()
    {
        var world = new ManagedTestWorld();
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent { Value = 5 });
        world.RemoveComponent<TestComponent>(id);
        world.HasComponent<TestComponent>(id).Should().BeFalse();
    }

    // --- Component Definition (Must be defined outside the class) ---
    internal sealed class TestComponent : IComponent
    {
        public int Value;
    }
}