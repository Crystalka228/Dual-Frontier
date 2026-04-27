using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has moved to tile
/// (<paramref name="X"/>, <paramref name="Y"/>). The presentation layer
/// updates the position of the visual node.
/// </summary>
/// <param name="PawnId">Identifier of the pawn that moved.</param>
/// <param name="X">New X coordinate (tile-grid units).</param>
/// <param name="Y">New Y coordinate (tile-grid units).</param>
public sealed record PawnMovedCommand(EntityId PawnId, float X, float Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
