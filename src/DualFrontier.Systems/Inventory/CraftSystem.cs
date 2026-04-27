using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Building;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Inventory;

/// <summary>
/// Execution of recipes at workbenches: checks ingredients,
/// asks <c>InventorySystem</c>, and on completion publishes
/// <c>CraftCompletedEvent</c>.
///
/// Phase: 6.
/// Tick: NORMAL (15 frames).
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
        // TODO: Phase 6 — advance active recipes by progress.
    }
}
