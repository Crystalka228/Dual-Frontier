using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has died. The presentation layer
/// (Launcher's <c>RenderCommandDispatcher</c>) finds its visual node, plays
/// the death animation, and removes the node once the animation completes.
/// Per К-extensions cascade #2 (2026-05-23): dispatch handled centrally
/// by Launcher.
/// </summary>
/// <param name="PawnId">Identifier of the deceased pawn.</param>
public sealed record PawnDiedCommand(EntityId PawnId) : IRenderCommand;
