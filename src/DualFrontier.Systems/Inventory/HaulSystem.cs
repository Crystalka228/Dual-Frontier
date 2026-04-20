using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Переноска стаков между хранилищами пешками. Публикует
/// <c>ItemPickupEvent</c> / <c>ItemDropEvent</c> — итоговые
/// изменения склада применяет <c>InventorySystem</c>.
///
/// Фаза: 3 (пешки).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(JobComponent), typeof(PositionComponent), typeof(StorageComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.NORMAL)]
public sealed class HaulSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на HaulRequestEvent, StorageFilledEvent.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 6 — подписка на события хаулов");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 6 — найти пешек с JobHaul, продвинуть шаг переноски.
    }
}
