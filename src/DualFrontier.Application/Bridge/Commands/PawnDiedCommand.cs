using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: pawn <paramref name="PawnId"/> has died. The presentation layer
/// finds its visual node, plays the death animation, and removes the node
/// once the animation completes.
/// </summary>
/// <param name="PawnId">Identifier of the deceased pawn.</param>
public sealed record PawnDiedCommand(EntityId PawnId) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
