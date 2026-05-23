using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has appeared в the world at
/// position (<paramref name="X"/>, <paramref name="Y"/>). The presentation
/// layer (Launcher's <c>RenderCommandDispatcher</c>) creates the visual node
/// and places it on the scene. Per К-extensions cascade #2 (2026-05-23):
/// commands are pure data records — dispatch handled centrally by Launcher,
/// не via per-command Execute() method (Lesson #25 refined applied).
/// </summary>
/// <param name="PawnId">Identifier of the new pawn.</param>
/// <param name="X">X coordinate (tile-grid units).</param>
/// <param name="Y">Y coordinate (tile-grid units).</param>
public sealed record PawnSpawnedCommand(EntityId PawnId, float X, float Y) : IRenderCommand;
