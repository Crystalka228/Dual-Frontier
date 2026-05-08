using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class SpanLeaseTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    [Fact]
    public void AcquireSpan_on_empty_returns_lease_with_zero_count()
    {
        using var world = new NativeWorld();

        // Force store creation via Add then Remove so the type_id has a store.
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 1 });
        world.RemoveComponent<HealthComponent>(e);

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(0);
        lease.Span.Length.Should().Be(0);
    }

    [Fact]
    public void Span_provides_read_access_to_dense_storage()
    {
        using var world = new NativeWorld();
        EntityId[] entities = new EntityId[5];
        for (int i = 0; i < 5; i++)
        {
            entities[i] = world.CreateEntity();
            world.AddComponent(entities[i],
                new HealthComponent { Current = i * 10, Maximum = 100 });
        }

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(5);
        lease.Span.Length.Should().Be(5);

        int sum = 0;
        for (int i = 0; i < lease.Count; i++)
        {
            sum += lease.Span[i].Current;
        }
        sum.Should().Be(0 + 10 + 20 + 30 + 40);
    }

    [Fact]
    public void Indices_parallel_to_Span()
    {
        using var world = new NativeWorld();
        for (int i = 0; i < 3; i++)
        {
            EntityId e = world.CreateEntity();
            world.AddComponent(e, new HealthComponent { Current = i, Maximum = 100 });
        }

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Indices.Length.Should().Be(lease.Count);
        for (int i = 0; i < lease.Count; i++)
        {
            lease.Indices[i].Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void Mutation_rejected_while_span_active()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        using var lease = world.AcquireSpan<HealthComponent>();

        // CreateEntity itself does not check spans (no component-side mutation),
        // but adding a component while a span is active must be rejected.
        EntityId e2 = world.CreateEntity();
        world.AddComponent(e2, new HealthComponent { Current = 999, Maximum = 100 });

        world.GetComponentCount<HealthComponent>().Should().Be(1);
    }

    [Fact]
    public void Mutation_succeeds_after_lease_disposed()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        lease.Dispose();

        EntityId e2 = world.CreateEntity();
        world.AddComponent(e2, new HealthComponent { Current = 2, Maximum = 100 });

        world.GetComponentCount<HealthComponent>().Should().Be(2);
    }

    [Fact]
    public void Span_throws_after_dispose()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        lease.Dispose();

        Action act = () => { var _ = lease.Span; };

        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Multiple_concurrent_leases_supported()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        using var lease1 = world.AcquireSpan<HealthComponent>();
        using var lease2 = world.AcquireSpan<HealthComponent>();

        lease1.Count.Should().Be(1);
        lease2.Count.Should().Be(1);
    }

    [Fact]
    public void Pairs_yields_entity_component_tuples()
    {
        using var world = new NativeWorld();
        EntityId e1 = world.CreateEntity();
        EntityId e2 = world.CreateEntity();
        EntityId e3 = world.CreateEntity();

        world.AddComponent(e1, new HealthComponent { Current = 10, Maximum = 100 });
        world.AddComponent(e2, new HealthComponent { Current = 20, Maximum = 100 });
        world.AddComponent(e3, new HealthComponent { Current = 30, Maximum = 100 });

        using var lease = world.AcquireSpan<HealthComponent>();

        var collected = new List<(int index, int value)>();
        foreach (var (entity, component) in lease.Pairs)
        {
            collected.Add((entity.Index, component.Current));
        }

        collected.Should().HaveCount(3);
        collected.Should().Contain((e1.Index, 10));
        collected.Should().Contain((e2.Index, 20));
        collected.Should().Contain((e3.Index, 30));
    }
}
