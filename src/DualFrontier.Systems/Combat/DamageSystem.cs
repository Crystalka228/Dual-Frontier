using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Combat;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Damage application: reads DamageEvent from the <c>Combat</c>
/// bus, accounts for armour and remaining shields, and writes
/// the result into <see cref="HealthComponent"/>. When HP reaches
/// zero, publishes <c>DeathEvent</c>.
///
/// Phase: 2 (after CombatSystem and ProjectileSystem).
/// Tick: FAST (3 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ArmorComponent), typeof(ShieldComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5)]
public sealed class DamageSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to DamageEvent / ProjectileHitEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 2 — process accumulated DamageEvent, apply the formula, publish DeathEvent.
    }
}
