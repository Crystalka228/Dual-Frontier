using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Long collective rituals: several pawns stand in a circle,
/// accumulate a shared mana pool, and at the end produce an
/// effect (summon, buff, node transformation). Publishes
/// <c>RitualCompletedEvent</c>.
///
/// Phase: 4.
/// Tick: RARE (3600 frames) — ritual steps are long.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(SchoolComponent) },
    writes: new[] { typeof(ManaComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 6)]
public sealed class RitualSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to RitualStartEvent / RitualAbortEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 4 — advance ritual steps, collect mana from participants.
    }
}
