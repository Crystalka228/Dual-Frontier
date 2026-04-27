using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Pawn skill growth from accumulated experience. Publishes
/// <c>SkillGainEvent</c> on the <c>Pawns</c> bus when a level rises.
///
/// Phase: 3 (pawns).
/// Tick: NORMAL (15 frames).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(SkillsComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 3)]
public sealed class SkillSystem : SystemBase
{
    /// <summary>
    /// Bridge: pending Phase 3 expansion — will subscribe to SkillXpGainedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 3 — apply accumulated XP, raise levels, publish SkillGainEvent.
    }
}
