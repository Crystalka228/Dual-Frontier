using System.Collections.Generic;
using DualFrontier.Application.Display;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Application.Tests.Display;

/// <summary>
/// К10.3 v2 Item 38 — К-L17 display composition framework tests.
/// Verifies layer registration, composition order (LayerType primary,
/// CompositionOrder secondary, FQN tertiary), and RenderFrame ordering.
/// </summary>
public sealed class CompositionFrameworkTests
{
    [Fact]
    public void RegisterLayer_adds_to_registry()
    {
        var framework = new CompositionFramework();
        var layer = new SimStateLayer();

        framework.RegisterLayer(layer);

        framework.LayerCount.Should().Be(1);
        framework.GetLayer(SimStateLayer.DefaultFqn).Should().BeSameAs(layer);
    }

    [Fact]
    public void RegisterLayer_with_duplicate_fqn_throws()
    {
        var framework = new CompositionFramework();
        framework.RegisterLayer(new SimStateLayer());

        Action duplicate = () => framework.RegisterLayer(new SimStateLayer());

        duplicate.Should().Throw<System.InvalidOperationException>()
            .WithMessage("*already registered*");
    }

    [Fact]
    public void RegisterLayer_with_empty_fqn_throws()
    {
        var framework = new CompositionFramework();

        Action empty = () => framework.RegisterLayer(new SimStateLayer(fqn: ""));

        empty.Should().Throw<System.ArgumentException>()
            .WithMessage("*non-empty*");
    }

    [Fact]
    public void UnregisterLayer_removes_and_invokes_callback()
    {
        var framework = new CompositionFramework();
        var layer = new CallbackTrackingLayer(LayerType.SimState, "Mod.TestLayer");
        framework.RegisterLayer(layer);

        bool removed = framework.UnregisterLayer("Mod.TestLayer");

        removed.Should().BeTrue();
        framework.LayerCount.Should().Be(0);
        layer.UnregisteredCount.Should().Be(1);
    }

    [Fact]
    public void UnregisterLayer_returns_false_when_not_registered()
    {
        var framework = new CompositionFramework();

        framework.UnregisterLayer("NotRegistered.Fqn").Should().BeFalse();
    }

    [Fact]
    public void Composition_order_primary_by_LayerType()
    {
        var framework = new CompositionFramework();
        var staticLayer = new CallbackTrackingLayer(LayerType.Static, "L.Static");
        var simState = new CallbackTrackingLayer(LayerType.SimState, "L.SimState");
        var combat = new CallbackTrackingLayer(LayerType.CombatFeedback, "L.Combat");
        var intent = new CallbackTrackingLayer(LayerType.Intent, "L.Intent");

        framework.RegisterLayer(staticLayer);
        framework.RegisterLayer(intent);
        framework.RegisterLayer(combat);
        framework.RegisterLayer(simState);

        IReadOnlyList<Layer> ordered = framework.GetLayersInCompositionOrder();

        ordered.Should().HaveCount(4);
        ordered[0].Type.Should().Be(LayerType.SimState);
        ordered[1].Type.Should().Be(LayerType.Intent);
        ordered[2].Type.Should().Be(LayerType.CombatFeedback);
        ordered[3].Type.Should().Be(LayerType.Static);
    }

    [Fact]
    public void Composition_order_secondary_by_CompositionOrder()
    {
        var framework = new CompositionFramework();
        var simHigh = new CallbackTrackingLayer(LayerType.SimState, "L.SimHigh", compositionOrder: 10);
        var simLow = new CallbackTrackingLayer(LayerType.SimState, "L.SimLow", compositionOrder: 0);
        var simMid = new CallbackTrackingLayer(LayerType.SimState, "L.SimMid", compositionOrder: 5);

        framework.RegisterLayer(simHigh);
        framework.RegisterLayer(simLow);
        framework.RegisterLayer(simMid);

        IReadOnlyList<Layer> ordered = framework.GetLayersInCompositionOrder();

        ordered[0].Fqn.Should().Be("L.SimLow");
        ordered[1].Fqn.Should().Be("L.SimMid");
        ordered[2].Fqn.Should().Be("L.SimHigh");
    }

    [Fact]
    public void Composition_order_tertiary_by_Fqn_for_stable_tie_break()
    {
        var framework = new CompositionFramework();
        // Same Type + CompositionOrder → ordered by Fqn.
        var aaa = new CallbackTrackingLayer(LayerType.Intent, "AAA");
        var zzz = new CallbackTrackingLayer(LayerType.Intent, "ZZZ");
        var mmm = new CallbackTrackingLayer(LayerType.Intent, "MMM");

        framework.RegisterLayer(zzz);
        framework.RegisterLayer(aaa);
        framework.RegisterLayer(mmm);

        IReadOnlyList<Layer> ordered = framework.GetLayersInCompositionOrder();

        ordered[0].Fqn.Should().Be("AAA");
        ordered[1].Fqn.Should().Be("MMM");
        ordered[2].Fqn.Should().Be("ZZZ");
    }

    [Fact]
    public void RenderFrame_invokes_layers_in_composition_order()
    {
        var framework = new CompositionFramework();
        var simState = new CallbackTrackingLayer(LayerType.SimState, "L.SimState");
        var intent = new CallbackTrackingLayer(LayerType.Intent, "L.Intent");
        var combat = new CallbackTrackingLayer(LayerType.CombatFeedback, "L.Combat");

        framework.RegisterLayer(combat);
        framework.RegisterLayer(simState);
        framework.RegisterLayer(intent);

        framework.RenderFrame(new TestRenderContext(deltaSeconds: 0.016, frameIndex: 42));

        // К-L17 order: SimState → Intent → CombatFeedback.
        simState.LastRenderFrameIndex.Should().Be(42);
        intent.LastRenderFrameIndex.Should().Be(42);
        combat.LastRenderFrameIndex.Should().Be(42);
        simState.RenderCallSequence.Should().BeLessThan(intent.RenderCallSequence);
        intent.RenderCallSequence.Should().BeLessThan(combat.RenderCallSequence);
    }

    [Fact]
    public void SimStateLayer_default_constructor_is_no_op()
    {
        var layer = new SimStateLayer();
        var context = new TestRenderContext(0.016, 1);

        // Render should not throw despite no delegate provided.
        layer.Invoking(l => l.Render(context)).Should().NotThrow();
        layer.Type.Should().Be(LayerType.SimState);
        layer.Fqn.Should().Be(SimStateLayer.DefaultFqn);
    }

    [Fact]
    public void SimStateLayer_invokes_render_delegate_when_provided()
    {
        int invokeCount = 0;
        uint capturedFrame = 0;
        var layer = new SimStateLayer(renderDelegate: ctx =>
        {
            invokeCount++;
            capturedFrame = ctx.FrameIndex;
        });

        layer.Render(new TestRenderContext(0.016, 7));

        invokeCount.Should().Be(1);
        capturedFrame.Should().Be(7);
    }

    [Fact]
    public void RenderFrame_with_null_context_throws()
    {
        var framework = new CompositionFramework();

        framework.Invoking(f => f.RenderFrame(null!)).Should().Throw<System.ArgumentNullException>();
    }

    private sealed class CallbackTrackingLayer : Layer
    {
        private static int s_globalSequence;
        private readonly LayerType _type;
        private readonly string _fqn;
        private readonly int _compositionOrder;

        public int UnregisteredCount { get; private set; }
        public uint LastRenderFrameIndex { get; private set; }
        public int RenderCallSequence { get; private set; } = -1;

        public CallbackTrackingLayer(LayerType type, string fqn, int compositionOrder = 0)
        {
            _type = type;
            _fqn = fqn;
            _compositionOrder = compositionOrder;
        }

        public override LayerType Type => _type;
        public override string Fqn => _fqn;
        public override int CompositionOrder => _compositionOrder;

        public override void Render(ILayerRenderContext context)
        {
            LastRenderFrameIndex = context.FrameIndex;
            RenderCallSequence = System.Threading.Interlocked.Increment(ref s_globalSequence);
        }

        public override void OnUnregistered() => UnregisteredCount++;
    }

    private sealed class TestRenderContext : ILayerRenderContext
    {
        public TestRenderContext(double deltaSeconds, uint frameIndex)
        {
            DeltaSeconds = deltaSeconds;
            FrameIndex = frameIndex;
        }

        public double DeltaSeconds { get; }
        public uint FrameIndex { get; }
    }
}
