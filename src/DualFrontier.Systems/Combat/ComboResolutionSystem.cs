using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Merges multiple <see cref="DamageIntent"/> from different systems
/// (Physical, Magic, Status) into a unified damage application order
/// within a single tick. Guarantees a deterministic sort by
/// <c>(EntityId, DamageKind ordinal)</c>, then publishes
/// <see cref="DamageEvent"/> sequentially to <c>DamageSystem</c>.
///
/// Determinism is critical for multiplayer/replays: the same set of
/// intents within a tick must produce the same damage application
/// order regardless of publication order (TechArch v0.2 §12.4).
///
/// Phase: 2 (after CombatSystem, SpellSystem, StatusEffectSystem).
/// Tick: NORMAL (15 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(DamageIntent) },
    writes: new[] { typeof(DamageEvent) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 5, Replaceable = true)]
public sealed class ComboResolutionSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to DamageIntent on Combat and Magic buses.
    /// </summary>
    protected override void OnInitialize() { }

    /// <summary>
    /// Main tick: after all <see cref="DamageIntent"/> for the current phase
    /// have been collected, calls <see cref="ResolvePending"/>, which sorts
    /// and applies the damage deterministically.
    /// </summary>
    public override void Update(float delta)
    {
        // TODO: Phase 4 — call ResolvePending() once per tick.
    }

    /// <summary>
    /// Queues <paramref name="intent"/> for the current tick. Application
    /// happens in <see cref="ResolvePending"/> once all intents have been
    /// collected.
    /// </summary>
    /// <param name="intent">Intent to deal damage, from any source system.</param>
    public void OnDamageIntent(DamageIntent intent)
    {
        throw new NotImplementedException("TODO: Phase 4 — queue DamageIntent for the tick");
    }

    /// <summary>
    /// Sorts the accumulated <see cref="DamageIntent"/> entries by
    /// <c>(EntityId, DamageKind ordinal)</c>, applies them in that order,
    /// and publishes the resulting <see cref="DamageEvent"/> to
    /// <c>DamageSystem</c>. The current tick's queue is cleared after the call.
    /// </summary>
    public void ResolvePending()
    {
        throw new NotImplementedException("TODO: Phase 4 — deterministic sort and publication of DamageEvent");
    }
}
