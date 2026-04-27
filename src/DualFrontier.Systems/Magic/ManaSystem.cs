using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Magic;
using DualFrontier.Systems.Magic.Internal;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Mana regeneration and consumption. Handles <c>ManaIntent</c>
/// (discrete drain via the two-step model) and
/// <see cref="ManaLeaseOpenRequest"/> (continuous lease per §12.2):
/// opens leases through <see cref="ManaLeaseRegistry"/>, drains every
/// active lease each tick, and publishes <see cref="ManaLeaseClosed"/>
/// once the duration or resource is exhausted.
///
/// Phase: 1 (parallel with CombatSystem, WeatherSystem).
/// Tick: NORMAL (15 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ManaLeaseOpenRequest) },
    writes: new[] { typeof(ManaComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 6)]
public sealed class ManaSystem : SystemBase
{
    // TODO: Phase 5 — replace with a field injected via the DI container,
    // so other Magic systems can obtain a reference to the same registry.
    private readonly ManaLeaseRegistry _registry = new();

    /// <summary>
    /// Bridge: Phase 6 will subscribe to ManaIntent / ManaLeaseOpenRequest /
    /// internal CloseRequest.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 1 — regenerate mana for every pawn based on school and stats.
        // TODO: Phase 5 — call DrainActiveLeases() to drain active leases.
    }

    /// <summary>
    /// Handler for a discrete mana drain (step 1 of the two-step model).
    /// Publishes <c>ManaGranted</c> if there is enough mana, otherwise
    /// <c>ManaRefused</c>.
    /// </summary>
    /// <param name="intent">Intent to spend mana.</param>
    public void OnManaIntent(ManaIntent intent)
    {
        throw new NotImplementedException("TODO: Phase 1 — validate and drain mana per ManaIntent");
    }

    /// <summary>
    /// Handler for ManaSystem's own <see cref="ManaGranted"/> response (when
    /// ManaSystem uses the two-step model as a client, e.g. for sustaining
    /// a golem).
    /// </summary>
    /// <param name="evt">Mana drain confirmation.</param>
    public void OnManaGranted(ManaGranted evt)
    {
        throw new NotImplementedException("TODO: Phase 1 — react to ManaGranted");
    }

    /// <summary>
    /// Reads <c>Mana.Current</c> (<c>N-1</c> snapshot — see §12.3); when the
    /// resource is sufficient, opens a lease via
    /// <see cref="ManaLeaseRegistry.Open"/> and publishes
    /// <see cref="ManaLeaseOpened"/>; otherwise — <see cref="ManaLeaseRefused"/>
    /// with the appropriate <see cref="RefusalReason"/>.
    /// </summary>
    /// <param name="req">Request to open a continuous mana lease.</param>
    public void OnManaLeaseOpenRequest(ManaLeaseOpenRequest req)
    {
        throw new NotImplementedException("TODO: Phase 5 — open the mana lease and publish ManaLeaseOpened/Refused");
    }

    /// <summary>
    /// Called every tick from <see cref="Update"/>: delegates to
    /// <see cref="ManaLeaseRegistry.DrainTick"/>, receives the list of
    /// expired leases, and publishes <see cref="ManaLeaseClosed"/> for each.
    /// </summary>
    public void DrainActiveLeases()
    {
        throw new NotImplementedException("TODO: Phase 5 — per-tick drain of active leases and publication of expired ones");
    }

    /// <summary>
    /// Handler for an explicit lease close (e.g. spell aborted externally
    /// or golem deactivated). Closes the entry via
    /// <see cref="ManaLeaseRegistry.Close"/> and publishes
    /// <see cref="ManaLeaseClosed"/> with the supplied reason.
    /// </summary>
    /// <param name="id">Identifier of the lease to close.</param>
    /// <param name="reason">Close reason.</param>
    public void OnManaLeaseCloseRequest(LeaseId id, CloseReason reason)
    {
        throw new NotImplementedException("TODO: Phase 5 — explicit mana lease close on request");
    }
}
