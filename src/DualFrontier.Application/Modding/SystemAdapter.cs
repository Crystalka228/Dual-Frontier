using System;
using System.Reflection;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Engine-internal bridge that maps an SDK <see cref="ISimulationSystem"/> onto
/// the executor's <see cref="SystemBase"/> (W1 BD-1). Because it IS a
/// <see cref="SystemBase"/>, a fault thrown by the wrapped <see cref="ISimulationSystem.Tick"/>
/// routes through the scheduler's EXISTING D2 origin-asymmetric catch
/// (<c>ParallelSystemScheduler</c> → <c>SystemExecutionContext.RouteFault</c>)
/// with no additional fault code: a mod-origin fault is contained and its mod
/// quarantined, exactly as for a native <see cref="SystemBase"/> mod system.
///
/// <para>
/// Generic in the wrapped type so each distinct mod system yields a distinct
/// <see cref="SystemBase"/> type — keeping the executor's type-keyed logic
/// correct (duplicate detection in <c>DependencyGraph</c>, the per-type
/// tick-rate cache). The adapter carries no <c>[SystemAccess]</c>/<c>[TickRate]</c>
/// of its own; it FORWARDS the wrapped system's declarations via the
/// <see cref="SystemBase.AccessDeclaration"/>/<see cref="SystemBase.TickRateDeclaration"/>
/// hooks, so it is transparent to the executor's reflection.
/// </para>
///
/// <para>
/// BRIDGE — deletion trigger (recorded at creation per
/// GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md §4 + EXECUTION_AUTHORITY_MATRIX.md
/// §3.0): this adapter and the <see cref="SystemBase"/> authoring path retire
/// together at W5, when the last <c>src/</c> harness system migrates to
/// <see cref="ISimulationSystem"/> and nothing derives from <see cref="SystemBase"/>
/// any longer.
/// </para>
/// </summary>
/// <typeparam name="TSystem">The wrapped SDK system type.</typeparam>
// [TickRate] here is a BRIDGE PLACEHOLDER satisfying the DFK013 (К-L13) static
// wake-discipline check, which cannot see through the generic forwarding. It is
// read by nobody at runtime: the tick scheduler resolves the EFFECTIVE cadence
// through the TickRateDeclaration hook below, which returns the wrapped system's
// own [TickRate]. REALTIME is the safe default (a missed override just ticks
// every frame, never skips).
[TickRate(TickRates.REALTIME)]
internal sealed class SystemAdapter<TSystem> : SystemBase where TSystem : class, ISimulationSystem
{
    // Read once per wrapped type (static per generic instantiation).
    private static readonly SystemAccessAttribute? CachedAccess =
        typeof(TSystem).GetCustomAttribute<SystemAccessAttribute>(inherit: false);

    private static readonly TickRateAttribute? CachedTickRate =
        typeof(TSystem).GetCustomAttribute<TickRateAttribute>(inherit: false);

    private readonly TSystem _system;
    private readonly SystemContextView _view;

    // Public so Activator.CreateInstance (used on the reflective MakeGenericType
    // path in ModRegistry) binds it; the class itself is internal.
    public SystemAdapter(TSystem system, ModRegistry registry, string modId, Func<long> tickSource)
    {
        _system = system ?? throw new ArgumentNullException(nameof(system));
        _view = new SystemContextView(registry, modId, tickSource);
    }

    internal override SystemAccessAttribute? AccessDeclaration => CachedAccess;

    internal override TickRateAttribute? TickRateDeclaration => CachedTickRate;

    protected override void OnInitialize() => _system.Initialize(_view);

    public override void Update(float delta) => _system.Tick(_view);

    protected override void OnDispose() => _system.OnDispose();
}
