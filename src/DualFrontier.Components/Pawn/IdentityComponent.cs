namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Core;

/// <summary>
/// Pure POCO carrying a pawn's identity data. Currently a single field —
/// <see cref="Name"/> — populated by <c>RandomPawnFactory</c> (or any
/// future scenario loader). Pawns lacking this component carry no name;
/// <c>PawnStateReporterSystem</c> publishes empty <c>Name</c> in that
/// case. The UI displays empty name verbatim — no fabricated fallback.
/// Future fields (rank, biography, portrait id) can be added later.
/// </summary>
public sealed class IdentityComponent : IComponent
{
    /// <summary>
    /// Display name. Empty string by default. Set at spawn time by the
    /// scenario loader; not mutated thereafter except by future rename
    /// systems.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
