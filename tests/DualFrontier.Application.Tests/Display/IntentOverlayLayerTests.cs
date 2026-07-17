using DualFrontier.Application.Display;
using DualFrontier.Contracts.Display;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Application.Tests.Display;

/// <summary>
/// К10.3 v2 Item 39 — К-L17 intent overlay layer tests.
/// Verifies layer slot tier identity + render delegate invocation pattern.
/// </summary>
public sealed class IntentOverlayLayerTests
{
    [Fact]
    public void Type_is_Intent()
    {
        var layer = new IntentOverlayLayer();
        layer.Type.Should().Be(LayerType.Intent);
    }

    [Fact]
    public void Default_fqn_matches_constant()
    {
        var layer = new IntentOverlayLayer();
        layer.Fqn.Should().Be(IntentOverlayLayer.DefaultFqn);
    }

    [Fact]
    public void Custom_fqn_overrides_default()
    {
        var layer = new IntentOverlayLayer(fqn: "Mod.Custom.IntentLayer");
        layer.Fqn.Should().Be("Mod.Custom.IntentLayer");
    }

    [Fact]
    public void Default_render_delegate_is_no_op()
    {
        var layer = new IntentOverlayLayer();
        var context = new StubRenderContext(0.016, 1);

        layer.Invoking(l => l.Render(context)).Should().NotThrow();
    }

    [Fact]
    public void Render_invokes_provided_delegate_with_context()
    {
        int callCount = 0;
        uint capturedFrame = 0;
        var layer = new IntentOverlayLayer(renderDelegate: ctx =>
        {
            callCount++;
            capturedFrame = ctx.FrameIndex;
        });

        layer.Render(new StubRenderContext(0.016, 42));

        callCount.Should().Be(1);
        capturedFrame.Should().Be(42);
    }

    [Fact]
    public void Render_with_null_context_throws()
    {
        var layer = new IntentOverlayLayer();
        layer.Invoking(l => l.Render(null!)).Should().Throw<System.ArgumentNullException>();
    }

    [Fact]
    public void CompositionOrder_defaults_to_zero()
    {
        var layer = new IntentOverlayLayer();
        layer.CompositionOrder.Should().Be(0);
    }

    [Fact]
    public void CompositionOrder_is_configurable()
    {
        var layer = new IntentOverlayLayer(compositionOrder: 5);
        layer.CompositionOrder.Should().Be(5);
    }

    [Fact]
    public void Renders_after_SimState_in_composition_order()
    {
        var framework = new CompositionFramework();
        var simState = new SimStateLayer();
        var intent = new IntentOverlayLayer();

        framework.RegisterLayer(intent);
        framework.RegisterLayer(simState);

        var ordered = framework.GetLayersInCompositionOrder();
        ordered[0].Type.Should().Be(LayerType.SimState);
        ordered[1].Type.Should().Be(LayerType.Intent);
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
