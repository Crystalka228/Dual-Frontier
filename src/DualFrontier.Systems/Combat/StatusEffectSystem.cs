using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Status effect ticks (burning, poison, fear, etc.): applies
/// damage to health and/or affects mood. Publishes
/// <c>StatusAppliedEvent</c> / <c>StatusExpiredEvent</c>.
///
/// Phase: 2.
/// Tick: FAST (3 frames).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(MindComponent), typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5, Replaceable = true)]
public sealed class StatusEffectSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to StatusAppliedEvent / StatusRemovedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 5 — advance all active effects, apply their damage/mood, remove expired ones.
    }
}
