using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Raid spawning: strength formula based on colony wealth and
/// relations with neighbours. Publishes <c>RaidIncomingEvent</c>
/// on the <c>World</c> bus — other systems prepare for it.
///
/// Phase: 7.
/// Tick: RARE (3600 frames).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class RaidSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to NewDayEvent / RelationBreakdownEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 7 — decide whether to spawn a raid, publish RaidIncomingEvent.
    }
}
