using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic.Internal;

/// <summary>
/// Internal registry of active mana leases. Accessible only to ManaSystem
/// and related Magic systems. Not exported outside the assembly — the type
/// is marked <c>internal</c> and the file lives in the <c>Internal/</c>
/// subdirectory, whose boundaries are enforced by project conventions
/// (see <c>Magic/Internal/README.md</c>).
///
/// The registry stores the list of open leases, issues <see cref="LeaseId"/>
/// values, drains mana on each tick, and locates expired leases.
/// </summary>
internal sealed class ManaLeaseRegistry
{
    // TODO: Phase 5 — private readonly Dictionary<LeaseId, ManaLease> _active = new();
    // TODO: Phase 5 — track leases per caster (for ActiveCountForCaster).

    /// <summary>
    /// Opens a new mana lease for the caster. The real implementation must
    /// validate invariants (<paramref name="drainPerTick"/> &gt; 0,
    /// <paramref name="min"/> not greater than <paramref name="max"/>),
    /// issue a new <see cref="LeaseId"/> via <see cref="LeaseId.New"/>,
    /// record the <see cref="ManaLease"/> entry in the internal collection,
    /// and return the identifier.
    /// </summary>
    /// <param name="caster">Caster mage from whom mana will be drained.</param>
    /// <param name="drainPerTick">Mana drain per tick.</param>
    /// <param name="min">Minimum lease duration in ticks.</param>
    /// <param name="max">Maximum lease duration in ticks.</param>
    public LeaseId Open(EntityId caster, float drainPerTick, int min, int max)
    {
        throw new NotImplementedException("TODO: Phase 5 — register a new mana lease and issue a LeaseId");
    }

    /// <summary>
    /// Closes a lease by identifier. Removes the entry from the internal
    /// collection and returns the total mana drained
    /// (<see cref="ManaLease.TotalDrained"/>), which <c>ManaSystem</c> uses
    /// when publishing <c>ManaLeaseClosed</c>.
    /// </summary>
    /// <param name="id">Identifier of the lease to close.</param>
    /// <param name="reason">Close reason — propagated outward into the
    /// corresponding event.</param>
    public float Close(LeaseId id, CloseReason reason)
    {
        throw new NotImplementedException("TODO: Phase 5 — remove the lease and return TotalDrained");
    }

    /// <summary>
    /// Drains <c>DrainPerTick</c> from every active lease for one tick.
    /// Returns the list of identifiers of leases that have expired this
    /// tick (reached <c>MaxDurationTicks</c> or the caster ran out of mana)
    /// — <c>ManaSystem</c> publishes <c>ManaLeaseClosed</c> for each of them.
    /// </summary>
    public IReadOnlyList<LeaseId> DrainTick()
    {
        throw new NotImplementedException("TODO: Phase 5 — iterate every active lease, drain mana, surface expired ones");
    }

    /// <summary>
    /// Tries to retrieve a lease record by identifier. Returns
    /// <c>true</c> if a lease with the given <paramref name="id"/> is open;
    /// otherwise <c>false</c> and <paramref name="lease"/> = <c>null</c>.
    /// </summary>
    /// <param name="id">Identifier of the lease to find.</param>
    /// <param name="lease">Lease record, if found.</param>
    public bool TryGet(LeaseId id, out ManaLease? lease)
    {
        throw new NotImplementedException("TODO: Phase 5 — look up the lease in the internal collection");
    }

    /// <summary>
    /// Returns the number of active leases for the given caster. Used by
    /// <c>ManaSystem</c> to enforce the per-caster limit
    /// (<c>RefusalReason.LeaseCapExceeded</c>).
    /// </summary>
    /// <param name="caster">Caster mage.</param>
    public int ActiveCountForCaster(EntityId caster)
    {
        throw new NotImplementedException("TODO: Phase 5 — count open leases for the caster");
    }
}
