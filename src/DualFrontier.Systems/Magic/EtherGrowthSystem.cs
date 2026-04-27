using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Growth of ether nodes in the world, driven by the dominant
/// magic school in the area. Publishes <c>EtherSurgeEvent</c>
/// when a density threshold is crossed.
///
/// Phase: 2 (after MapSystem).
/// Tick: SLOW (60 frames) — ether grows slowly.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(EtherComponent), typeof(SchoolComponent) },
    writes: new[] { typeof(EtherComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.SLOW)]
[BridgeImplementation(Phase = 6)]
public sealed class EtherGrowthSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to EtherDrainedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 2 — diffuse/grow ether across neighbouring nodes, check thresholds.
    }
}
