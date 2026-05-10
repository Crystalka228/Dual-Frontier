using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Faction relations matrix in [-100..100]: aggression,
/// neutrality, alliance. Changes through events (kills, gifts,
/// trade) and the simple passage of time.
///
/// Phase: 7 (meta-game, after the core world).
/// Tick: RARE (3600 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(FactionComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class RelationSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to DeathEvent / TradeCompletedEvent /
    /// RaidIncomingEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 7 — apply accumulated changes to the relations matrix.
    }
}
