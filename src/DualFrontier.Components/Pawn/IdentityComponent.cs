namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

/// <summary>
/// Pure POCO carrying a pawn's identity data. Currently a single field —
/// <see cref="Name"/> — populated by <c>RandomPawnFactory</c> (or any
/// future scenario loader). Pawns lacking this component carry no name;
/// <c>PawnStateReporterSystem</c> publishes empty <c>Name</c> in that
/// case. The UI displays empty name verbatim — no fabricated fallback.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct IdentityComponent : IComponent
{
    /// <summary>
    /// Display name handle. Default = empty interned-string sentinel
    /// (<see cref="InternedString.Empty"/>). Set at spawn time by the
    /// scenario loader via <c>NativeWorld.InternString</c>; not mutated
    /// thereafter except by future rename systems. Resolve at the
    /// presentation site via <c>InternedString.Resolve(NativeWorld)</c>
    /// or <c>NativeWorld.ResolveInternedString</c>; the empty sentinel
    /// resolves to <c>null</c> per the «no fabricated fallback» rule —
    /// callers convert <c>null</c> to empty string at the display
    /// boundary.
    /// </summary>
    public InternedString Name;
}
