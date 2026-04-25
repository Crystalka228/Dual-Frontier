using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Стриминг регионов карты: загрузка тайлов в зоне интереса,
/// выгрузка далёких. Публикует <c>MapRegionLoadedEvent</c>.
///
/// Фаза: 0 (раньше всех — остальные читают карту).
/// Тик: RARE (3600 фреймов).
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
        // TODO: Фаза 0 — определить зону интереса, подгрузить/выгрузить регионы.
    }
}
