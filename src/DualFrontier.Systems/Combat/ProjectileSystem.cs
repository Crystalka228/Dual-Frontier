using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Straight-line projectile motion and collision registration.
/// On a hit, publishes <c>ProjectileHitEvent</c> on the
/// <c>Combat</c> bus — <c>DamageSystem</c> converts the hit into
/// damage.
///
/// Phase: 1 (parallel with CombatSystem).
/// Tick: REALTIME (1 frame) — visually correct flight.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PositionComponent) },
    writes: new[] { typeof(PositionComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.REALTIME)]
[BridgeImplementation(Phase = 5)]
public sealed class ProjectileSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to ProjectileSpawnedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 5 — advance projectiles by delta, look for collisions.
    }
}
