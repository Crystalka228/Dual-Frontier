using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
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

        // Since ID-B a lease holds a versions view as well as the component span,
        // so CreateEntity is refused too — creating can grow and therefore
        // REALLOCATE the version table the lease is pointing into. The refusal
        // surfaces as the C ABI's 0 sentinel, i.e. EntityId.Invalid.
        // (Native-side the two guards stay independent: creation under a PLAIN
        // component span is still legal — see scenario_span_lifetime.)
        EntityId e2 = world.CreateEntity();
        e2.Should().Be(EntityId.Invalid, "CreateEntity is refused while a lease is held");

        // And adding a component while a span is active is rejected as it always was.
        world.AddComponent(e2, new HealthComponent { Current = 999, Maximum = 100 });

        world.GetComponentCount<HealthComponent>().Should().Be(1);
    }

    // ── ID-B versions view (К-L22) ───────────────────────────────────────────

    [Fact]
    public void Versions_is_indexed_by_entity_index_not_dense_position()
    {
        using var world = new NativeWorld();

        // Three entities, but only the LAST two carry the component, so dense
        // position and entity index deliberately disagree.
        EntityId skipped = world.CreateEntity();
        EntityId a = world.CreateEntity();
        EntityId b = world.CreateEntity();
        world.AddComponent(a, new HealthComponent { Current = 10, Maximum = 100 });
        world.AddComponent(b, new HealthComponent { Current = 20, Maximum = 100 });

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(2, "only two entities carry the component");
        lease.Versions.Length.Should().BeGreaterThan(lease.Count,
            "Versions is sized by the world's version TABLE, not by the dense span");
        lease.Versions.Length.Should().BeGreaterThan(skipped.Index,
            "every minted index must be addressable in the view, component or not");

        for (int i = 0; i < lease.Count; i++)
        {
            int entityIndex = lease.Indices[i];
            lease.Versions[entityIndex].Should().Be(0,
                "a never-recycled slot carries version 0 — read it at Versions[Indices[i]]");
        }
    }

    [Fact]
    public void Versions_reports_the_true_generation_of_a_recycled_slot()
    {
        using var world = new NativeWorld();

        EntityId stale = world.CreateEntity();
        world.AddComponent(stale, new HealthComponent { Current = 1, Maximum = 100 });
        world.DestroyEntity(stale);
        world.FlushDestroyedEntities();

        EntityId live = world.CreateEntity();
        world.AddComponent(live, new HealthComponent { Current = 2, Maximum = 100 });
        live.Index.Should().Be(stale.Index);

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Versions[live.Index].Should().Be(live.Version,
            "the view must report the generation the live entity actually holds");
        lease.Versions[live.Index].Should().NotBe(stale.Version);
        lease.Versions[live.Index].Should().NotBe(0,
            "version 0 is exactly the value fabrication used to supply here");

        foreach ((EntityId id, HealthComponent _) in lease.Pairs)
            id.Should().Be(live, "Pairs must yield the live id, not a stale look-alike");
    }

    [Fact]
    public void Dispose_releases_the_versions_view_so_creation_is_legal_again()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        world.CreateEntity().Should().Be(EntityId.Invalid, "refused while the lease is held");

        lease.Dispose();

        EntityId after = world.CreateEntity();
        after.Should().NotBe(EntityId.Invalid, "creation is legal once the lease is disposed");
        world.IsAlive(after).Should().BeTrue();

        // Dispose is idempotent — a second call must not release the underlying
        // view a second time and re-open the guard for the NEXT lease.
        lease.Dispose();
        using var second = world.AcquireSpan<HealthComponent>();
        world.CreateEntity().Should().Be(EntityId.Invalid,
            "the guard must re-arm cleanly for a fresh lease after a double Dispose");
    }

    [Fact]
    public void Versions_throws_after_dispose()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        lease.Dispose();

        Action act = () => { var _ = lease.Versions; };

        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Concurrent_leases_each_hold_their_own_versions_view()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var first = world.AcquireSpan<HealthComponent>();
        var second = world.AcquireSpan<HealthComponent>();

        first.Dispose();
        world.CreateEntity().Should().Be(EntityId.Invalid,
            "the second lease still holds a view — the counter must not have hit zero");

        second.Dispose();
        world.CreateEntity().Should().NotBe(EntityId.Invalid,
            "both views released — creation is legal again");
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
