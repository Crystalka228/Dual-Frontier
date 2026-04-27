using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Terminal event in the mana lease lifecycle. Marked with
/// <see cref="DeferredAttribute"/>: subscribers may mutate state
/// (remove effects, drop bindings) — that should happen on the
/// next tick to avoid breaking invariants of the current phase.
/// </summary>
/// <param name="Id">Identifier of the lease being closed.</param>
/// <param name="Caster">The mage caster.</param>
/// <param name="Reason">Close reason — normal or abnormal.</param>
/// <param name="TotalManaDrained">Total amount of mana drained
/// over the entire lease lifetime (for statistics/balance).</param>
[Deferred]
public sealed record ManaLeaseClosed(
    LeaseId Id,
    EntityId Caster,
    CloseReason Reason,
    float TotalManaDrained) : IEvent;
