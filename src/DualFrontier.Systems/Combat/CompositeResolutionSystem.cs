using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Combat;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Handles <see cref="CompoundShotIntent"/>: collects responses from the
/// Inventory and Magic buses keyed by <see cref="TransactionId"/> and
/// publishes either <see cref="ShootGranted"/> or <see cref="ShootRefused"/>.
///
/// Two-phase commit: a single <c>CompoundShotIntent</c> must receive both
/// partial responses — <see cref="AmmoGranted"/>/<see cref="AmmoRefused"/>
/// from the Inventory bus and <see cref="ManaGranted"/>/<see cref="ManaRefused"/>
/// from the Magic bus. Only once both have arrived does the system make a
/// decision and publish the final <c>ShootGranted</c>/<c>ShootRefused</c>
/// on the Combat bus.
///
/// Phase: 2 (after CombatSystem, ManaSystem, InventorySystem).
/// Tick: FAST (3 frames) — combat responsiveness.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(AmmoGranted), typeof(AmmoRefused), typeof(ManaGranted), typeof(ManaRefused) },
    writes: new[] { typeof(ShootGranted), typeof(ShootRefused) },
    buses:  new[] { nameof(IGameServices.Combat), nameof(IGameServices.Inventory), nameof(IGameServices.Magic) }
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5)]
public sealed class CompositeResolutionSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to CompoundShotIntent / AmmoGranted /
    /// AmmoRefused / ManaGranted / ManaRefused.
    /// </summary>
    protected override void OnInitialize() { }

    /// <summary>
    /// Main tick: resolves accumulated unfinished transactions. Most of the
    /// work happens reactively in the handlers below; this method remains
    /// for periodic timeout checks (see §12.4).
    /// </summary>
    public override void Update(float delta)
    {
        // TODO: Phase 4 — clean up expired pending transactions, publish ShootRefused(TimedOut).
    }

    /// <summary>
    /// Begins the two-phase commit: stores <paramref name="intent"/> in the
    /// pending table by its <see cref="TransactionId"/> and dispatches partial
    /// requests on the Inventory and Magic buses.
    /// </summary>
    /// <param name="intent">Compound shot intent received from CombatSystem.</param>
    public void OnCompoundShotIntent(CompoundShotIntent intent)
    {
        throw new NotImplementedException("TODO: Phase 4 — initiate two-phase commit for CompoundShotIntent");
    }

    /// <summary>
    /// Partial "ammo granted" response keyed by <see cref="TransactionId"/>.
    /// Stored in the pending table; if the second partial response is already
    /// present, calls <see cref="TryResolve"/>.
    /// </summary>
    /// <param name="evt">Ammo issuance confirmation from InventoryBus.</param>
    public void OnAmmoGranted(AmmoGranted evt)
    {
        throw new NotImplementedException("TODO: Phase 4 — register partial AmmoGranted by TxId");
    }

    /// <summary>
    /// Caches the ammo refusal — the final transaction outcome will be
    /// <see cref="ShootRefused"/> with reason <see cref="ShotRefusalReason.NoAmmo"/>.
    /// </summary>
    /// <param name="evt">Refusal from InventoryBus.</param>
    public void OnAmmoRefused(AmmoRefused evt)
    {
        throw new NotImplementedException("TODO: Phase 4 — register partial AmmoRefused by TxId");
    }

    /// <summary>
    /// Second partial response — mana drain confirmed. Like
    /// <see cref="OnAmmoGranted"/>, when both responses are present the
    /// transaction is resolved via <see cref="TryResolve"/>.
    /// </summary>
    /// <param name="evt">Mana drain confirmation from MagicBus.</param>
    public void OnManaGranted(ManaGranted evt)
    {
        throw new NotImplementedException("TODO: Phase 4 — register partial ManaGranted by TxId");
    }

    /// <summary>
    /// Caches the mana refusal — the final transaction outcome will be
    /// <see cref="ShootRefused"/> with reason <see cref="ShotRefusalReason.NoMana"/>.
    /// </summary>
    /// <param name="evt">Refusal from MagicBus.</param>
    public void OnManaRefused(ManaRefused evt)
    {
        throw new NotImplementedException("TODO: Phase 4 — register partial ManaRefused by TxId");
    }

    /// <summary>
    /// Tries to resolve a transaction by <paramref name="id"/>: if both
    /// partial responses have arrived, publishes <see cref="ShootGranted"/>
    /// (both Granted) or <see cref="ShootRefused"/> (any Refused) and removes
    /// the entry from the pending table. If both responses are not yet in,
    /// it is a no-op.
    /// </summary>
    /// <param name="id">Transaction identifier to resolve.</param>
    private void TryResolve(TransactionId id)
    {
        throw new NotImplementedException("TODO: Phase 4 — combine Ammo* and Mana* responses into the final ShootGranted/ShootRefused");
    }
}
