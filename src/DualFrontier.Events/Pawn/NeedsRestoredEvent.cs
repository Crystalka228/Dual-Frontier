using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Pawn;

/// <summary>
/// Published by <c>ConsumeSystem</c> when a pawn finishes consuming food /
/// drinking water. Carries the amount and target need; the actual write to
/// <see cref="NeedsComponent"/> happens inside <c>NeedsSystem</c>'s
/// subscriber so that NeedsComponent has a single static writer (per the
/// dependency-graph constraint that forbids two systems with overlapping
/// <c>[SystemAccess]</c> writes).
/// <para>
/// Marked <see cref="DeferredAttribute"/>: dispatched at the phase boundary
/// after the publisher's <c>Update</c> returns. The handler is invoked with
/// the subscriber's captured <c>SystemExecutionContext</c>, which is what
/// allows it to write <see cref="NeedsComponent"/> even though the
/// publisher's context cannot.
/// </para>
/// </summary>
[Deferred]
public sealed record NeedsRestoredEvent : IEvent
{
    /// <summary>The pawn whose need was restored.</summary>
    public required EntityId PawnId { get; init; }

    /// <summary>Which need was restored (Satiety / Hydration).</summary>
    public required NeedKind Need { get; init; }

    /// <summary>How much to add to the need, clamped to 1.0 by the handler.</summary>
    public required float Amount { get; init; }
}
