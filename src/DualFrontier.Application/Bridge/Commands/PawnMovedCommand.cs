using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has moved к tile
/// (<paramref name="X"/>, <paramref name="Y"/>). The presentation layer
/// (Launcher's <c>RenderCommandDispatcher</c>) updates the position of the
/// visual node. Per К-extensions cascade #2 (2026-05-23): dispatch handled
/// centrally by Launcher.
/// </summary>
/// <param name="PawnId">Identifier of the pawn that moved.</param>
/// <param name="X">New X coordinate (tile-grid units).</param>
/// <param name="Y">New Y coordinate (tile-grid units).</param>
public sealed record PawnMovedCommand(EntityId PawnId, float X, float Y) : IRenderCommand;
