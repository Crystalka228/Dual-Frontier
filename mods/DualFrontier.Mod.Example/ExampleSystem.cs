using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference SDK system — authored against <c>DualFrontier.Contracts</c> alone
/// (<see cref="ISimulationSystem"/>, NOT the engine's <c>SystemBase</c>). It
/// exercises the full per-tick surface a third-party mod has: a span-read and a
/// batch-write over its own component, the simulation tick, and an event
/// publish. The engine wraps it onto the executor through an internal adapter;
/// this mod never names an engine assembly.
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(ExampleComponent) },
    bus:    nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class ExampleSystem : ISimulationSystem
{
    /// <summary>One-time setup: subscribe to the example event.</summary>
    public void Initialize(ISystemContext context)
    {
        context.Subscribe<ExampleEvent>(OnExample);
    }

    /// <summary>
    /// Per-tick: advance every <see cref="ExampleComponent"/> by one via a
    /// batched write (recording while snapshotting the span, then flushing on
    /// scope exit), and publish the tick. The span releases before the batch
    /// flushes.
    /// </summary>
    public void Tick(ISystemContext context)
    {
        using (WriteScope<ExampleComponent> batch = context.BeginBatch<ExampleComponent>())
        {
            using SpanScope<ExampleComponent> span = context.AcquireSpan<ExampleComponent>();
            foreach ((EntityId entity, ExampleComponent component) in span.Pairs)
            {
                ExampleComponent advanced = component;
                advanced.Ticks++;
                batch.Update(entity, advanced);
            }
        }

        context.Publish(new ExampleEvent(context.CurrentTick));
    }

    /// <summary>
    /// Teardown. The system's subscription is released by the mod-unload chain
    /// (RestrictedModApi.UnsubscribeAll), so there is nothing to release here;
    /// the hook exists because the boundary law mandates it (DoD item 7).
    /// </summary>
    public void OnDispose()
    {
    }

    private static void OnExample(ExampleEvent evt)
    {
    }
}
