using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Медленные сдвиги биомов: под действием эфирных бурь и
/// длительной погоды участки переходят из одного биома в
/// другой. Публикует <c>BiomeShiftEvent</c>.
///
/// Фаза: 1.
/// Тик: RARE (3600 фреймов).
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
        // TODO: Фаза 1 — пройти по зонам, решить, сдвинулся ли биом, опубликовать BiomeShiftEvent.
    }
}
