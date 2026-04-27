using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Streams map regions: loads tiles in the area of interest and
/// unloads distant ones. Publishes <c>MapRegionLoadedEvent</c>.
///
/// Phase: 0 (earliest — other systems read the map).
/// Tick: RARE (3600 frames).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(TileComponent) },
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class MapSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to CameraMovedEvent / RegionRequestedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 0 — determine the area of interest, load/unload regions.
    }
}
