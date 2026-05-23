using DualFrontier.Contracts.Core;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: item <paramref name="ItemId"/> appeared в the world at position
/// (<paramref name="X"/>, <paramref name="Y"/>) of kind
/// <paramref name="Kind"/>. The presentation layer (Launcher's
/// <c>RenderCommandDispatcher</c>) creates an item visual selecting the
/// sprite atlas region based on <paramref name="Kind"/>. Per К-extensions
/// cascade #2 (2026-05-23): dispatch handled centrally by Launcher.
/// </summary>
/// <param name="ItemId">Identifier of the spawned item entity.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
/// <param name="Kind">Presentation hint selecting atlas region.</param>
public sealed record ItemSpawnedCommand(
    EntityId ItemId,
    float X,
    float Y,
    ItemKind Kind) : IRenderCommand;
