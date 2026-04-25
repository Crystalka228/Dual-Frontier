using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Periodic snapshot of one pawn's HUD-relevant state. Published by
/// GameBootstrap in response to a domain PawnStateChangedEvent.
/// All need values are normalised "wellness" 0..1 (1 = best), already
/// inverted from <c>NeedsComponent</c> deficit semantics by the reporter
/// system, so the HUD can render bars without further translation.
/// </summary>
public sealed record PawnStateCommand(
    EntityId PawnId,
    string Name,
    float Hunger,
    float Thirst,
    float Rest,
    float Comfort,
    float Mood,
    string JobLabel,
    bool JobUrgent
) : IRenderCommand
{
    public void Execute(object renderContext)
    {
        /* Dispatched by RenderCommandDispatcher, not via Execute. */
    }
}
