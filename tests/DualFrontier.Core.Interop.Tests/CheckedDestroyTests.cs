using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

/// <summary>
/// EQ_A3 — real-native coverage of the checked-destroy pair
/// (<c>df_world_active_span_count</c> / <c>df_world_destroy_checked</c>). Each
/// test owns its own <see cref="NativeWorld"/>, so these stay in the default
/// (parallel) collection. The EngineSession busy-&gt;Abort ROUTING is covered
/// separately in DualFrontier.Modding.Tests via the ShutdownTransactionHooks seam.
/// </summary>
public sealed class CheckedDestroyTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    [Fact]
    public void DisposeChecked_at_zero_destroys_and_reports_zero()
    {
        using var world = new NativeWorld();

        WorldTeardownResult r = world.DisposeChecked();

        r.Destroyed.Should().BeTrue("no borrows outstanding -> checked destroy succeeds");
        r.ActiveSpans.Should().Be(0);
        r.ActiveBatches.Should().Be(0);
    }

    [Fact]
    public void ActiveSpanCount_tracks_acquire_and_release()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 1 });

        world.ActiveSpanCount().Should().Be(0);

        var lease = world.AcquireSpan<HealthComponent>();
        world.ActiveSpanCount().Should().Be(1, "one span held");
        lease.Dispose();

        world.ActiveSpanCount().Should().Be(0, "span released");
    }

    [Fact]
    public void DisposeChecked_refuses_while_a_span_is_live_then_succeeds()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 1 });

        var lease = world.AcquireSpan<HealthComponent>();
        WorldTeardownResult busy = world.DisposeChecked();

        busy.Destroyed.Should().BeFalse("a live span must refuse checked destroy");
        busy.ActiveSpans.Should().Be(1, "the refusal reports the live span count");
        world.ActiveSpanCount().Should().Be(1, "the world is intact and usable after a refusal");

        lease.Dispose();
        world.DisposeChecked().Destroyed.Should().BeTrue("released span -> checked destroy succeeds");
        // The `using` Dispose() is now a no-op (handle retired by DisposeChecked).
    }

    [Fact]
    public void DisposeChecked_refuses_while_a_write_batch_is_live_then_succeeds()
    {
        using var world = new NativeWorld();

        WriteBatch<HealthComponent> batch = world.BeginBatch<HealthComponent>();
        WorldTeardownResult busy = world.DisposeChecked();

        busy.Destroyed.Should().BeFalse("a live write-batch must refuse checked destroy");
        busy.ActiveBatches.Should().Be(1, "the refusal reports the live batch count");

        batch.Dispose();
        world.DisposeChecked().Destroyed.Should().BeTrue("released batch -> checked destroy succeeds");
    }
}
