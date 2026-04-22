using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Смена погоды: марковская цепь состояний (Clear, Rain,
/// EtherStorm, Snow, …). Публикует <c>WeatherChangedEvent</c>;
/// <c>BiomeSystem</c> реагирует, обновляя биомы.
///
/// Фаза: 1 (параллельно с CombatSystem, ManaSystem).
/// Тик: RARE (3600 фреймов).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(BiomeComponent) },
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
public sealed class WeatherSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на RitualCompletedEvent (ритуалы могут
    /// вызывать погоду) и NewDayEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 1 — подписка на события погоды");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 1 — прошагать марковскую цепь, при смене состояния опубликовать WeatherChangedEvent.
    }
}
