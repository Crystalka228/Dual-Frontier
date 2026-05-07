using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeWorldTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    private struct PositionComponent
    {
        public float X;
        public float Y;
    }

    [Fact]
    public void CreateEntity_returns_valid_alive_entity()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        world.IsAlive(entity).Should().BeTrue();
        world.EntityCount.Should().Be(1);
    }

    [Fact]
    public void DestroyEntity_marks_entity_dead_immediately()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        world.DestroyEntity(entity);

        world.IsAlive(entity).Should().BeFalse();
    }

    [Fact]
    public void Component_added_can_be_retrieved()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        var health = new HealthComponent { Current = 75, Maximum = 100 };
        world.AddComponent(entity, health);

        HealthComponent retrieved = world.GetComponent<HealthComponent>(entity);
        retrieved.Current.Should().Be(75);
        retrieved.Maximum.Should().Be(100);
    }

    [Fact]
    public void HasComponent_reflects_addition_and_removal()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var health = new HealthComponent { Current = 50, Maximum = 100 };

        world.HasComponent<HealthComponent>(entity).Should().BeFalse();

        world.AddComponent(entity, health);
        world.HasComponent<HealthComponent>(entity).Should().BeTrue();

        world.RemoveComponent<HealthComponent>(entity);
        world.HasComponent<HealthComponent>(entity).Should().BeFalse();
    }

    [Fact]
    public void Multiple_component_types_coexist_on_entity()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        world.AddComponent(entity, new HealthComponent { Current = 75, Maximum = 100 });
        world.AddComponent(entity, new PositionComponent { X = 10f, Y = 20f });

        world.HasComponent<HealthComponent>(entity).Should().BeTrue();
        world.HasComponent<PositionComponent>(entity).Should().BeTrue();
        world.GetComponent<HealthComponent>(entity).Current.Should().Be(75);
        world.GetComponent<PositionComponent>(entity).X.Should().Be(10f);
    }

    [Fact]
    public void Destroyed_entity_components_persist_until_flush()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new HealthComponent { Current = 50, Maximum = 100 });

        world.DestroyEntity(entity);
        world.IsAlive(entity).Should().BeFalse();
        world.GetComponentCount<HealthComponent>().Should().Be(1);  // pre-flush

        world.FlushDestroyedEntities();
        world.GetComponentCount<HealthComponent>().Should().Be(0);  // post-flush
    }

    [Fact]
    public void Disposed_world_throws_on_use()
    {
        var world = new NativeWorld();
        world.Dispose();

        Action act = () => world.CreateEntity();

        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void TryGetComponent_returns_false_for_missing()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        bool found = world.TryGetComponent<HealthComponent>(entity, out _);

        found.Should().BeFalse();
    }

    [Fact]
    public void GetComponent_throws_for_missing()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        Action act = () => world.GetComponent<HealthComponent>(entity);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found*");
    }

    [Fact]
    public void EntityCount_decreases_after_destroy()
    {
        using var world = new NativeWorld();
        EntityId a = world.CreateEntity();
        EntityId b = world.CreateEntity();

        world.EntityCount.Should().Be(2);

        world.DestroyEntity(a);
        world.EntityCount.Should().Be(1);

        world.DestroyEntity(b);
        world.EntityCount.Should().Be(0);
    }
}
