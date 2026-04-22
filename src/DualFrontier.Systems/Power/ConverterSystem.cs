using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Конвертер эфир → электричество. КПД 30% per GDD 9:
/// на вход 10 единиц эфира — на выход 3 единицы тока.
///
/// Фаза: 2 (между EtherGridSystem и ElectricGridSystem в потоке данных).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ConverterSystem : SystemBase
{
    // КПД 30% per GDD 9.

    /// <summary>
    /// TODO: Подписаться на ConverterInputEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 2 — подписка на события конвертеров");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 2 — перевести входящий эфир в электричество с коэффициентом 0.3f (КПД 30% per GDD 9).
    }
}
