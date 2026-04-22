using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Исполнение рецептов на верстаках: проверяет ингредиенты,
/// спрашивает <c>InventorySystem</c>, при готовности публикует
/// <c>CraftCompletedEvent</c>.
///
/// Фаза: 6.
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(WorkbenchComponent), typeof(SkillsComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Inventory)
)]
[TickRate(TickRates.NORMAL)]
public sealed class CraftSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на CraftRequestEvent, CraftAbortedEvent.
    /// </summary>
    protected override void OnInitialize()
    {
        throw new NotImplementedException("TODO: Фаза 6 — подписка на события крафта");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 6 — продвижение активных рецептов по прогрессу.
    }
}
