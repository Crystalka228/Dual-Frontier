using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: an item entity appeared in the world at (<paramref name="X"/>, <paramref name="Y"/>),
/// with <paramref name="Kind"/> as the renderer's hint for which atlas region to use.
///
/// <para>
/// <b>Nothing emits this today</b>, and its handler has always been empty, so no item has
/// ever been drawn. The engine used to publish one per starting item from its composition root;
/// that composition root is gone, and the mod which now seeds the colony reports colonists
/// through the generic presentation surface and items not at all, preserving exactly what
/// shipped.
/// </para>
///
/// <para>
/// <paramref name="Kind"/> was a game enum, which an engine assembly may not name (boundary law
/// B-2). It is now an opaque int: whoever revives item visuals owns the meaning of the value, and
/// the engine carries it without interpreting it.
/// </para>
/// </summary>
/// <param name="ItemId">Identifier of the spawned item entity.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
/// <param name="Kind">Opaque presentation hint selecting an atlas region.</param>
public sealed record ItemSpawnedCommand(
    EntityId ItemId,
    float X,
    float Y,
    int Kind) : IRenderCommand;
