using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has appeared in the world at
/// position (<paramref name="X"/>, <paramref name="Y"/>). The presentation
/// layer creates the visual node and places it on the scene.
/// </summary>
/// <param name="PawnId">Identifier of the new pawn.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
