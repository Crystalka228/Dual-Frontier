using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Spell execution: checks resource/school, publishes
/// <c>SpellCastEvent</c> and target effects (DamageEvent,
/// StatusApplied) on the corresponding buses. DOES NOT WRITE
/// components — only reads and publishes events.
///
/// Phase: 4 (after ManaSystem).
/// Tick: FAST (3 frames) — spell responsiveness.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ManaComponent), typeof(SchoolComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 6)]
public sealed class SpellSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to SpellCastAttemptEvent /
    /// ManaGranted / ManaRefused.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 4 — advance active casts, publish SpellCastEvent.
    }
}
