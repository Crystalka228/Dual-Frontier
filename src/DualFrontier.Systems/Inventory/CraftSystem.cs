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
[BridgeImplementation(Phase = 6)]
public sealed class CraftSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to CraftRequestEvent / CraftAbortedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 6 — продвижение активных рецептов по прогрессу.
    }
}
