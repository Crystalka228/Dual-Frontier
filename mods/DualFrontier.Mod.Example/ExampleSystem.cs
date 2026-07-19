using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference SDK system — authored against <c>DualFrontier.Contracts</c> alone
/// (<see cref="ISimulationSystem"/>, NOT the engine's <c>SystemBase</c>). It
/// exercises the per-tick component surface a third-party mod has: a span-read
/// and a batch-write over its own component. The engine wraps it onto the
/// executor through an internal adapter; this mod never names an engine assembly.
///
/// <para>
/// <b>Events (W1 note).</b> A regular mod cannot publish its OWN event type — an
/// event type defined in a regular mod's collectible ALC is invisible to other
/// mods, so it must be vended by a shared mod (<c>ContractValidator</c> Phase E;
/// MOD_OS_ARCHITECTURE §5 / §6.5 D-4). The <see cref="ISystemContext"/>
/// Publish/Subscribe surface exists and is unit-tested, but the mod-authored event
/// story (shared-event ownership) lands at W2 (BD-3). So this reference system
/// demonstrates component access + the lifecycle, not events.
/// </para>
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(ExampleComponent) })]
[TickRate(TickRates.NORMAL)]
public sealed class ExampleSystem : ISimulationSystem
{
    /// <summary>One-time setup. Nothing to subscribe in W1 (see the type remarks).</summary>
    public void Initialize(ISystemContext context)
    {
    }

    /// <summary>
    /// Per-tick: advance every <see cref="ExampleComponent"/> by one via a batched
    /// write, reading the current set through a span (the span releases before the
    /// batch flushes on scope exit).
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
    }

    /// <summary>
    /// Teardown. Law-mandated hook (boundary-law DoD item 7); fires on
    /// disable / fault / unload. Nothing to release for this reference system.
    /// </summary>
    public void OnDispose()
    {
    }
}
