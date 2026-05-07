using DualFrontier.Contracts.Core;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: item <paramref name="ItemId"/> appeared in the world at position
/// (<paramref name="X"/>, <paramref name="Y"/>) of kind
/// <paramref name="Kind"/>. Presentation creates an <c>ItemVisual</c> node
/// selecting the sprite atlas region based on <paramref name="Kind"/>.
/// </summary>
/// <param name="ItemId">Identifier of the spawned item entity.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
/// <param name="Kind">Presentation hint selecting atlas region.</param>
public sealed record ItemSpawnedCommand(
    EntityId ItemId,
    float X,
    float Y,
    ItemKind Kind) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* Routed by RenderCommandDispatcher — Execute body unused for Phase 5. */
    }
}
