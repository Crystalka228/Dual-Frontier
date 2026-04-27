using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Manages the owner-golem bond: sustains the active bond through the
/// owner's mana and breaks the bond when mana is insufficient.
/// Publishes <c>GolemActivatedEvent</c> / <c>GolemBondBrokenEvent</c>
/// and handles <see cref="GolemOwnershipTransferRequest"/> (owner change)
/// with the resulting deferred <see cref="GolemOwnershipChanged"/>.
///
/// Phase: 4 (after ManaSystem).
/// Tick: NORMAL (15 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(GolemBondComponent), typeof(ManaComponent) },
    writes: new[] { typeof(GolemBondComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 6)]
public sealed class GolemSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to GolemCommandEvent /
    /// GolemOwnershipTransferRequest / ManaGranted / ManaRefused.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 4 — check active bonds, drain maintenance, break on shortage.
    }

    /// <summary>
    /// Handles a golem owner change: validates the target mode
    /// (<c>OwnershipMode</c>), updates <see cref="GolemBondComponent"/>,
    /// and publishes <see cref="GolemOwnershipChanged"/>. The event is
    /// marked <c>[Deferred]</c>, so subscribers' actual mutations happen
    /// on the next tick.
    /// </summary>
    /// <param name="req">Request to transfer/change golem ownership.</param>
    public void OnGolemOwnershipTransferRequest(GolemOwnershipTransferRequest req)
    {
        throw new NotImplementedException("TODO: Phase 6 — handle golem owner change and publish GolemOwnershipChanged");
    }

    /// <summary>
    /// To avoid a feedback loop (v02 §12.3): GolemSystem reads the mana
    /// state of the PREVIOUS tick rather than the current one. The real
    /// implementation is a <c>Mana[N-1]</c> snapshot (buffered state) so
    /// that ManaSystem running concurrently does not see changes made in
    /// the current tick by GolemSystem, and vice versa.
    /// </summary>
    /// <param name="mage">Owner mage of the golem whose mana state is read.</param>
    private float ReadPreviousTickManaState(EntityId mage)
    {
        throw new NotImplementedException("TODO: Phase 6 — read Mana[N-1] snapshot to prevent the feedback loop");
    }
}
