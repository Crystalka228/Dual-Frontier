using System;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sdk;

/// <summary>Component owned by this test file alone.</summary>
public struct RoundTripComponent : IComponent
{
    public int Kind;
    public long Stamp;
}

/// <summary>
/// W3 defect regression (D-2). The canonical mod loop is: acquire a span, iterate
/// <c>Pairs</c>, and write the entities back through a batch. Before W3 the pair iterators
/// reconstructed <see cref="EntityId"/> with <c>Version=1</c> — a version NO freshly created
/// entity has (versions start at 0 and only grow). A batch keyed on such an id RECORDED
/// successfully and was then rejected by the flush-time version check, silently, so the loop
/// wrote nothing at all.
///
/// <para>
/// Nothing caught it because every existing batch test writes an id obtained from
/// <c>CreateEntity</c> rather than one obtained from a span. These tests close that gap by
/// asserting the ROUND TRIP: the id a span hands back must be usable as a write key.
/// </para>
/// </summary>
[Collection("GameLoopSerial")]
public sealed class SpanWriteRoundTripTests
{
    [Fact]
    public void IdFromSpan_EqualsIdFromCreateEntity_AndIsAlive()
    {
        using var world = new NativeWorld();
        EntityId created = world.CreateEntity();
        world.AddComponent(created, new RoundTripComponent { Kind = 1, Stamp = 10 });

        EntityId fromSpan = default;
        using (SpanLease<RoundTripComponent> lease = world.AcquireSpan<RoundTripComponent>())
        {
            foreach ((EntityId id, RoundTripComponent _) in lease.Pairs)
                fromSpan = id;
        }

        fromSpan.Should().Be(created,
            "an id a span hands back must BE the entity, not a look-alike with a fabricated version");
        world.IsAlive(fromSpan).Should().BeTrue();
    }

    [Fact]
    public void SdkLoop_ReadSpanThenWriteBatch_ActuallyPersists()
    {
        using var world = new NativeWorld();
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        SystemExecutionContext.PushContext(ctx);
        try
        {
            EntityId minted = view.CreateEntity();
            using (WriteScope<RoundTripComponent> add = view.BeginBatch<RoundTripComponent>())
            {
                add.Add(minted, new RoundTripComponent { Kind = 0, Stamp = 0 }).Should().BeTrue();
            }

            // The canonical loop: read the entity back out of a span, then write it.
            EntityId fromSpan = default;
            using (SpanScope<RoundTripComponent> span = view.AcquireSpan<RoundTripComponent>())
            {
                foreach ((EntityId id, RoundTripComponent _) in span.Pairs)
                    fromSpan = id;
            }
            fromSpan.Should().Be(minted, "the SDK span must round-trip the id the SDK minted");

            using (WriteScope<RoundTripComponent> upd = view.BeginBatch<RoundTripComponent>())
            {
                upd.Update(fromSpan, new RoundTripComponent { Kind = 7, Stamp = 300 }).Should().BeTrue();
            }

            RoundTripComponent after = default;
            using (SpanScope<RoundTripComponent> span = view.AcquireSpan<RoundTripComponent>())
            {
                foreach ((EntityId _, RoundTripComponent c) in span.Pairs)
                    after = c;
            }

            after.Stamp.Should().Be(300,
                "the batched write must PERSIST — Update returning true only means recorded, and " +
                "before the D-2 fix the flush dropped it on a version mismatch without a word");
            after.Kind.Should().Be(7);
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }
    }

    [Fact]
    public void WriteBatchOwnSnapshotEnumerator_AlsoRoundTrips()
    {
        using var world = new NativeWorld();
        EntityId created = world.CreateEntity();
        world.AddComponent(created, new RoundTripComponent { Kind = 2, Stamp = 20 });

        using WriteBatch<RoundTripComponent> batch = world.BeginBatch<RoundTripComponent>();

        EntityId seen = default;
        foreach ((EntityId id, RoundTripComponent _) in batch)
            seen = id;

        seen.Should().Be(created,
            "WriteBatch's own snapshot enumerator reconstructs ids the same way and must agree");
    }

    // ── ID-B: the recycled-index round trip (F-59) ───────────────────────────
    //
    // W3 narrowed the defect from "no entity has this version" to "no RECYCLED
    // entity has this version" by aligning the pair iterators on Version = 0.
    // The tests above pin the narrowed case; these pin the case W3 could not
    // express, because expressing it needs a slot that has actually been reused.

    [Fact]
    public void RecycledIndex_SpanYieldsTheLiveGeneration_NotTheStaleOne()
    {
        using var world = new NativeWorld();

        // Mint, destroy, flush — the flush is what returns the index to the
        // free list, so the next create recycles it.
        EntityId stale = world.CreateEntity();
        world.AddComponent(stale, new RoundTripComponent { Kind = 1, Stamp = 1 });
        world.DestroyEntity(stale);
        world.FlushDestroyedEntities();

        EntityId live = world.CreateEntity();
        world.AddComponent(live, new RoundTripComponent { Kind = 2, Stamp = 2 });

        live.Index.Should().Be(stale.Index, "the new entity must have recycled the slot");
        live.Version.Should().BeGreaterThan(stale.Version,
            "the ABA law bumps the slot version before it can be reissued");

        EntityId fromSpan = default;
        using (SpanLease<RoundTripComponent> lease = world.AcquireSpan<RoundTripComponent>())
        {
            foreach ((EntityId id, RoundTripComponent _) in lease.Pairs)
                fromSpan = id;
        }

        fromSpan.Should().Be(live,
            "the span must name the entity that occupies the slot NOW — reconstructing " +
            "Version = 0 here names the destroyed entity instead");
        fromSpan.Should().NotBe(stale);
        world.IsAlive(fromSpan).Should().BeTrue();
        world.IsAlive(stale).Should().BeFalse("the stale id must stay dead forever");
    }

    [Fact]
    public void RecycledIndex_StaleKeyIsDropped_WhileTheSpanKeyLands()
    {
        using var world = new NativeWorld();

        EntityId stale = world.CreateEntity();
        world.AddComponent(stale, new RoundTripComponent { Kind = 1, Stamp = 1 });
        world.DestroyEntity(stale);
        world.FlushDestroyedEntities();

        EntityId live = world.CreateEntity();
        world.AddComponent(live, new RoundTripComponent { Kind = 2, Stamp = 2 });

        EntityId fromSpan = default;
        using (SpanLease<RoundTripComponent> lease = world.AcquireSpan<RoundTripComponent>())
        {
            foreach ((EntityId id, RoundTripComponent _) in lease.Pairs)
                fromSpan = id;
        }

        // Two writes, one batch: one keyed on the id the span handed back, one
        // keyed on the stale id that shares its index. Exactly one must land,
        // and the flush count says which.
        int applied;
        using (WriteBatch<RoundTripComponent> batch = world.BeginBatch<RoundTripComponent>())
        {
            batch.Update(fromSpan, new RoundTripComponent { Kind = 9, Stamp = 900 })
                 .Should().BeTrue("recording is not applying — this only means the command was queued");
            batch.Update(stale, new RoundTripComponent { Kind = 4, Stamp = 400 })
                 .Should().BeTrue("the stale command records too; the flush is where it dies");
            applied = batch.Flush();
        }

        applied.Should().Be(1,
            "the flush-time version check must apply the span-keyed write and drop the stale one");

        world.TryGetComponent(live, out RoundTripComponent after).Should().BeTrue();
        after.Stamp.Should().Be(900,
            "the write keyed on the span's id must PERSIST — this is the write that used to " +
            "vanish whenever an index had been recycled");
        after.Kind.Should().Be(9);
    }

    [Fact]
    public void SdkRecycledIndex_ModLoopStillPersists_ThroughSpanScope()
    {
        using var world = new NativeWorld();
        var view = new SystemContextView(new ModRegistry(), "test.mod", () => 0L);
        var ctx = new SystemExecutionContext(
            "T", SystemOrigin.Mod, "test.mod", new NullModFaultSink(), world);

        SystemExecutionContext.PushContext(ctx);
        try
        {
            EntityId doomed = view.CreateEntity();
            using (WriteScope<RoundTripComponent> add = view.BeginBatch<RoundTripComponent>())
            {
                add.Add(doomed, new RoundTripComponent { Kind = 1, Stamp = 1 });
            }
            view.DestroyEntity(doomed);
            world.FlushDestroyedEntities();

            EntityId minted = view.CreateEntity();
            minted.Index.Should().Be(doomed.Index, "the SDK mint must recycle the slot");
            using (WriteScope<RoundTripComponent> add = view.BeginBatch<RoundTripComponent>())
            {
                add.Add(minted, new RoundTripComponent { Kind = 2, Stamp = 2 });
            }

            // The mod-facing loop, over a recycled slot.
            EntityId fromSpan = default;
            using (SpanScope<RoundTripComponent> span = view.AcquireSpan<RoundTripComponent>())
            {
                foreach ((EntityId id, RoundTripComponent _) in span.Pairs)
                    fromSpan = id;
            }
            fromSpan.Should().Be(minted);

            using (WriteScope<RoundTripComponent> upd = view.BeginBatch<RoundTripComponent>())
            {
                upd.Update(fromSpan, new RoundTripComponent { Kind = 7, Stamp = 700 });
            }

            RoundTripComponent after = default;
            using (SpanScope<RoundTripComponent> span = view.AcquireSpan<RoundTripComponent>())
            {
                foreach ((EntityId _, RoundTripComponent c) in span.Pairs)
                    after = c;
            }
            after.Stamp.Should().Be(700,
                "a mod's read-span-then-write-batch loop must persist over a recycled index too");
        }
        finally
        {
            SystemExecutionContext.PopContext();
        }
    }
}
