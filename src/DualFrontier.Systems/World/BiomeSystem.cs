using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Slow biome shifts: under the influence of ether storms and
/// prolonged weather, regions transition from one biome to
/// another. Publishes <c>BiomeShiftEvent</c>.
///
/// Phase: 1.
/// Tick: RARE (3600 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(BiomeComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class BiomeSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to WeatherChangedEvent / EtherSurgeEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 1 — walk the zones, decide whether the biome shifted, publish BiomeShiftEvent.
    }
}
