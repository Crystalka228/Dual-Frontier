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
}
