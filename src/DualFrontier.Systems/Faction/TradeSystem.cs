using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Arrival of trade caravans: spawning guests with goods and
/// publishing <c>TradeCaravanEvent</c>. Prices and goods tables
/// depend on relations from <c>RelationSystem</c>.
///
/// Phase: 7.
/// Tick: RARE (3600 frames).
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
        // TODO: Phase 7 — check the caravan schedule and spawn guests.
    }
}
