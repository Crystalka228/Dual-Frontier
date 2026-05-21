using System.Collections.Generic;
using System.Threading.Tasks;
using DualFrontier.Application.Display;
using DualFrontier.Contracts.Display;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Application.Tests.Display;

/// <summary>
/// К10.3 v2 Item 40 — К-L17 combat feedback layer tests.
/// Verifies Fast tier subscriber sink (EnqueueFeedback) + drain-on-render
/// semantics + thread safety of the queue.
/// </summary>
public sealed class CombatFeedbackLayerTests
{
    [Fact]
    public void Type_is_CombatFeedback()
    {
        var layer = new CombatFeedbackLayer();
        layer.Type.Should().Be(LayerType.CombatFeedback);
    }

    [Fact]
    public void Default_fqn_matches_constant()
    {
        var layer = new CombatFeedbackLayer();
        layer.Fqn.Should().Be(CombatFeedbackLayer.DefaultFqn);
    }

    [Fact]
    public void Custom_fqn_overrides_default()
    {
        var layer = new CombatFeedbackLayer(fqn: "Mod.Custom.CombatLayer");
        layer.Fqn.Should().Be("Mod.Custom.CombatLayer");
    }

    [Fact]
    public void EnqueueFeedback_increments_pending_count()
    {
        var layer = new CombatFeedbackLayer();

        layer.PendingFeedbackCount.Should().Be(0);
        layer.EnqueueFeedback("hit_event_1");
        layer.EnqueueFeedback("hit_event_2");

        layer.PendingFeedbackCount.Should().Be(2);
    }

    [Fact]
    public void EnqueueFeedback_null_throws()
    {
        var layer = new CombatFeedbackLayer();
        layer.Invoking(l => l.EnqueueFeedback(null!)).Should().Throw<System.ArgumentNullException>();
    }

    [Fact]
    public void Render_drains_queue_and_invokes_delegate_with_snapshot()
    {
        IReadOnlyList<object>? capturedSnapshot = null;
        var layer = new CombatFeedbackLayer(renderDelegate: (_, snapshot) =>
        {
            capturedSnapshot = snapshot;
        });

        layer.EnqueueFeedback("first");
        layer.EnqueueFeedback("second");
        layer.EnqueueFeedback("third");

        layer.Render(new StubRenderContext(0.016, 1));

        capturedSnapshot.Should().NotBeNull();
        capturedSnapshot!.Should().HaveCount(3);
        capturedSnapshot![0].Should().Be("first");
        capturedSnapshot![1].Should().Be("second");
        capturedSnapshot![2].Should().Be("third");
        layer.PendingFeedbackCount.Should().Be(0);
    }

    [Fact]
    public void Render_with_empty_queue_does_not_invoke_delegate()
    {
        int callCount = 0;
        var layer = new CombatFeedbackLayer(renderDelegate: (_, _) => callCount++);

        layer.Render(new StubRenderContext(0.016, 1));

        callCount.Should().Be(0);
    }

    [Fact]
    public void Render_no_delegate_just_drains_queue()
    {
        var layer = new CombatFeedbackLayer();  // null delegate

        layer.EnqueueFeedback("event");
        layer.PendingFeedbackCount.Should().Be(1);

        layer.Render(new StubRenderContext(0.016, 1));

        layer.PendingFeedbackCount.Should().Be(0);
    }

    [Fact]
    public void OnUnregistered_clears_pending_queue()
    {
        var layer = new CombatFeedbackLayer();
        layer.EnqueueFeedback("a");
        layer.EnqueueFeedback("b");
        layer.PendingFeedbackCount.Should().Be(2);

        layer.OnUnregistered();

        layer.PendingFeedbackCount.Should().Be(0);
    }

    [Fact]
    public async Task EnqueueFeedback_is_thread_safe_under_concurrent_writes()
    {
        var layer = new CombatFeedbackLayer();
        const int writerCount = 8;
        const int eventsPerWriter = 250;

        var tasks = new Task[writerCount];
        for (int i = 0; i < writerCount; i++)
        {
            int writerId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < eventsPerWriter; j++)
                {
                    layer.EnqueueFeedback($"w{writerId}_e{j}");
                }
            });
        }

        await Task.WhenAll(tasks);

        layer.PendingFeedbackCount.Should().Be(writerCount * eventsPerWriter);
    }

    [Fact]
    public void Renders_after_Intent_in_composition_order()
    {
        var framework = new CompositionFramework();
        var simState = new SimStateLayer();
        var intent = new IntentOverlayLayer();
        var combat = new CombatFeedbackLayer();

        framework.RegisterLayer(combat);
        framework.RegisterLayer(simState);
        framework.RegisterLayer(intent);

        var ordered = framework.GetLayersInCompositionOrder();
        ordered[0].Type.Should().Be(LayerType.SimState);
        ordered[1].Type.Should().Be(LayerType.Intent);
        ordered[2].Type.Should().Be(LayerType.CombatFeedback);
    }

    private sealed class StubRenderContext : ILayerRenderContext
    {
        public StubRenderContext(double deltaSeconds, uint frameIndex)
        {
            DeltaSeconds = deltaSeconds;
            FrameIndex = frameIndex;
        }
        public double DeltaSeconds { get; }
        public uint FrameIndex { get; }
    }
}
