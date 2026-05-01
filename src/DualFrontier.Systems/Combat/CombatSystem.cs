using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Combat;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Initiates combat actions. In the TechArch v0.2 §12.4 model it delegates
/// resource checking through <see cref="CompoundShotIntent"/> —
/// CompositeResolutionSystem queries the Inventory and Magic buses and
/// responds with <see cref="ShootGranted"/>/<see cref="ShootRefused"/>.
/// CombatSystem no longer publishes <c>AmmoIntent</c> directly.
///
/// Operates simultaneously on the Combat and Magic buses (the shooter's
/// mana is part of the compound shot).
///
/// Phase: 1 (parallel with ManaSystem, WeatherSystem).
/// Tick: FAST (3 frames) — combat responsiveness.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent), typeof(ManaComponent) },
    writes: new[] { typeof(HealthComponent) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5, Replaceable = true)]
public sealed class CombatSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to ShootAttemptEvent /
    /// ShootGranted / ShootRefused.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: process active combat actions
    }

    /// <summary>
    /// Publishes <see cref="CompoundShotIntent"/> for the two-phase commit
    /// (instead of the direct <c>AmmoIntent</c> used in v0.1). The response
    /// arrives from <c>CompositeResolutionSystem</c> as
    /// <see cref="ShootGranted"/> or <see cref="ShootRefused"/>.
    /// TODO: Phase 4 — build the intent from the current weapon/target and
    /// publish it on the Combat bus.
    /// </summary>
    /// <param name="intent">Ready intent to publish (in the future — shot
    /// parameters from which the intent is built internally).</param>
    public void OnCompoundShotIntent(CompoundShotIntent intent)
    {
        throw new NotImplementedException("TODO: Phase 4 — delegate the shot via CompoundShotIntent");
    }
}
