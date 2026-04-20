using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Пересчёт баланса электросети: сумма генерации против
/// потребителей по связным компонентам (союзному графу
/// кабелей). Публикует <c>PowerOnlineEvent</c> /
/// <c>PowerOutageEvent</c> в <c>World</c> шину при смене статуса.
///
/// Фаза: 2 (после построек).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PowerConsumerComponent), typeof(PowerProducerComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ElectricGridSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на BuildingPlacedEvent, BuildingDestroyedEvent, CablePlacedEvent.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 2 — подписка на события построек/кабелей");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 2 — обход подсетей union-find, баланс, публикация статуса.
    }
}
