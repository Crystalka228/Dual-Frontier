using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Прилёт/прибытие торговых караванов: спавн гостей с товаром,
/// публикация <c>TradeCaravanEvent</c>. Цены и таблицы товаров
/// зависят от отношений из <c>RelationSystem</c>.
///
/// Фаза: 7.
/// Тик: RARE (3600 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(FactionComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class TradeSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to NewDayEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 7 — проверка расписания караванов, спавн гостей.
    }
}
