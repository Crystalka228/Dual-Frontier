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

        // A lease holds a versions view, but the guard is narrowed to the path
        // that can actually dangle its pointer: growing the entity table
        // (PR #51 review R2). With spare capacity this create writes nothing to
        // the version table and cannot reallocate it, so it SUCCEEDS.
        EntityId e2 = world.CreateEntity();
        e2.Should().NotBe(EntityId.Invalid,
            "creating under a lease is legal while the table has spare capacity");

        // Adding a component while a span is active is rejected as it always was.
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

    /// <summary>
    /// Drives the world to its entity-table growth boundary under a held lease
    /// and RETURNS the refusal. The refusal is the boundary marker, so no test
    /// needs to know the table's capacity; on return the world is parked at the
    /// boundary (a refused create advances nothing) and the caller still holds
    /// its lease. Fails loudly if the guard never arms, so a test can never pass
    /// by silently never reaching the boundary.
    /// </summary>
    private static InvalidOperationException CreateUntilGrowthIsRefused(
        NativeWorld world, int ceiling = 4096)
    {
        for (int i = 0; i < ceiling; i++)
        {
            try
            {
                world.CreateEntity();
            }
            catch (InvalidOperationException refusal)
            {
                return refusal;
            }
        }

        throw new InvalidOperationException(
            $"growth was never refused within {ceiling} creates — the guard is not arming");
    }

    [Fact]
    public void Growing_the_entity_table_is_refused_while_a_lease_is_held()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        using var lease = world.AcquireSpan<HealthComponent>();

        // Loud, not silent (PR #51 review R2): a refused mint used to unpack to
        // EntityId.Invalid and return, so a caller would attach components to the
        // sentinel and lose the spawn without a word.
        InvalidOperationException refusal = CreateUntilGrowthIsRefused(world);

        refusal.Message.Should().Contain("versions view",
            "the refusal must name the window that caused it");
    }

    [Fact]
    public void Dispose_releases_the_versions_view_so_growth_is_legal_again()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        CreateUntilGrowthIsRefused(world);
        lease.Dispose();

        EntityId after = world.CreateEntity();
        after.Should().NotBe(EntityId.Invalid, "growth is legal once the lease is disposed");
        world.IsAlive(after).Should().BeTrue();

        // Dispose is idempotent — a second call must not release the underlying
        // view a second time and re-open the guard for the NEXT lease.
        lease.Dispose();
        using var second = world.AcquireSpan<HealthComponent>();
        CreateUntilGrowthIsRefused(world).Message.Should().Contain("versions view",
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

        // Park the world at its growth boundary so every create below is a
        // growth attempt, which is the only thing the guard fences.
        CreateUntilGrowthIsRefused(world);

        first.Dispose();
        ((Action)(() => world.CreateEntity())).Should().Throw<InvalidOperationException>(
            "the second lease still holds a view — the counter must not have hit zero");

        second.Dispose();
        world.CreateEntity().Should().NotBe(EntityId.Invalid,
            "both views released — growth is legal again");
    }

    // ── PR #51 review R1: the pending-destroy window ─────────────────────────

    [Fact]
    public void Pending_destroy_slot_reads_a_tombstone_and_its_id_is_not_alive()
    {
        using var world = new NativeWorld();
        EntityId doomed = world.CreateEntity();
        world.AddComponent(doomed, new HealthComponent { Current = 7, Maximum = 100 });

        world.DestroyEntity(doomed);   // deliberately NOT flushed

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(1,
            "component storage is reclaimed at flush, so the dead row is still in the span");

        int index = lease.Indices[0];
        lease.Versions[index].Should().BeNegative(
            "a destroyed-but-unflushed slot is tombstoned — it holds no entity at all");

        foreach ((EntityId id, HealthComponent _) in lease.Pairs)
        {
            world.IsAlive(id).Should().BeFalse(
                "the id a span reconstructs for a pending-destroy row must FAIL CLOSED. " +
                "Before the tombstone it was (index, v+1) — the exact pair CreateEntity " +
                "mints on recycle — which IsAlive accepted, so a destroyed entity read " +
                "as alive and the ABA law was void");
        }
    }

    [Fact]
    public void Pending_destroy_id_cannot_be_used_as_a_write_key()
    {
        using var world = new NativeWorld();
        EntityId doomed = world.CreateEntity();
        world.AddComponent(doomed, new HealthComponent { Current = 7, Maximum = 100 });
        world.DestroyEntity(doomed);

        EntityId fromSpan = default;
        using (var lease = world.AcquireSpan<HealthComponent>())
        {
            foreach ((EntityId id, HealthComponent _) in lease.Pairs)
                fromSpan = id;
        }

        int applied;
        using (WriteBatch<HealthComponent> batch = world.BeginBatch<HealthComponent>())
        {
            batch.Update(fromSpan, new HealthComponent { Current = 999, Maximum = 100 })
                 .Should().BeTrue("recording is not applying");
            applied = batch.Flush();
        }

        applied.Should().Be(0,
            "a write keyed on a pending-destroy row must be dropped at flush — the entity " +
            "is dead and the caller is the one who killed it");
    }

    [Fact]
    public void Recycled_slot_never_reuses_a_pair_observable_during_the_pending_window()
    {
        using var world = new NativeWorld();
        EntityId doomed = world.CreateEntity();
        world.AddComponent(doomed, new HealthComponent { Current = 7, Maximum = 100 });
        world.DestroyEntity(doomed);

        EntityId observedWhilePending = default;
        using (var lease = world.AcquireSpan<HealthComponent>())
        {
            foreach ((EntityId id, HealthComponent _) in lease.Pairs)
                observedWhilePending = id;
        }

        world.FlushDestroyedEntities();
        EntityId recycled = world.CreateEntity();

        recycled.Index.Should().Be(doomed.Index, "the slot recycled");
        recycled.Should().NotBe(observedWhilePending,
            "the ABA law (IAC §1 note 1) says a pair is issued at most once per world " +
            "lifetime; an id observable BEFORE its entity exists breaks it just as badly " +
            "as one reissued after");
        recycled.Should().NotBe(doomed);
        world.IsAlive(recycled).Should().BeTrue();
        world.IsAlive(observedWhilePending).Should().BeFalse();
        world.IsAlive(doomed).Should().BeFalse();
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
