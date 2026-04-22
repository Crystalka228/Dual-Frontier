using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Обработчик запросов склада: пул <c>AmmoIntent</c>,
/// <c>CraftRequest</c> и т. д. Кэш + батчинг per TechArch 11.5.
///
/// Фаза: 6 (после всего, что могло породить запросы склада).
/// Тик: FAST (3 фрейма) — быстрый отклик на патроны/крафт.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(StorageComponent) },
    writes: new[] { typeof(StorageComponent) },
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.FAST)]
public sealed class InventorySystem : SystemBase
{
    // Кэш + батчинг per TechArch 11.5: накапливаем все Intent'ы за тик,
    // решаем одним проходом, один раз пересчитываем свободное место.

    /// <summary>
    /// TODO: Подписаться на AmmoIntent, CraftRequest, ItemDeposit, ItemWithdraw.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 6 — подписка на запросы склада");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 6 — батчевая обработка накопленных запросов, публикация *Granted/*Refused.
    }
}
