namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// The SDK system contract — what a simulation system CAN be, authored against
/// <c>DualFrontier.Contracts</c> alone. A mod (and, after the W5 slice move,
/// vanilla) implements this instead of deriving from the engine's
/// <c>SystemBase</c>, which cannot live in Contracts (audit A4: a
/// <c>Contracts → Core.Interop → Contracts</c> cycle). The engine wraps an
/// implementation onto its executor through an internal adapter; the concrete
/// world stays kernel-internal.
///
/// <para>
/// <b>Lifecycle.</b> <see cref="Initialize"/> runs once (bus subscriptions,
/// one-time setup); <see cref="Tick"/> runs per the system's <c>[TickRate]</c>
/// cadence; <see cref="OnDispose"/> runs once at teardown. Each receives the
/// per-tick <see cref="ISystemContext"/> — the ONLY route to the world. Holding
/// the context, or anything obtained from it, across ticks is forbidden
/// (ECS.md §8).
/// </para>
///
/// <para>
/// <b>Why <see cref="OnDispose"/> exists despite zero current usage.</b> The
/// measured harness has no system that overrides a dispose hook — yet the
/// contract keeps one, because law mandates it: mod disable / fault / hot-reload
/// must remove the mod's resources COMPLETELY
/// (GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md §6, Definition of Done item 7),
/// and the mod unload chain is single and monotonic
/// (MOD_OS_ARCHITECTURE.md §9.4). Usage cannot veto a law-mandated teardown
/// hook; the hook is the system's place to release what it acquired.
/// </para>
///
/// <para>
/// <b>No temporal delta.</b> There is deliberately no <c>float delta</c> on this
/// contract (the measured harness reads none). Simulation time arrives via
/// <see cref="ISystemContext.CurrentTick"/> (TIME_AND_CONSISTENCY_MODEL §1).
/// </para>
/// </summary>
public interface ISimulationSystem
{
    /// <summary>
    /// One-time setup: subscribe to events, prepare state. Called once, before
    /// the first <see cref="Tick"/>, within an active engine context.
    /// </summary>
    void Initialize(ISystemContext context);

    /// <summary>
    /// Per-tick work. Called per the system's <c>[TickRate]</c> cadence. All
    /// world access is through <paramref name="context"/>; nothing from it may
    /// be retained beyond this call.
    /// </summary>
    void Tick(ISystemContext context);

    /// <summary>
    /// One-time teardown: unsubscribe, release acquired resources. Called once
    /// at disable / fault / unload. Law-mandated (see the type remarks); a
    /// system with nothing to release simply leaves it empty.
    /// </summary>
    void OnDispose();
}
