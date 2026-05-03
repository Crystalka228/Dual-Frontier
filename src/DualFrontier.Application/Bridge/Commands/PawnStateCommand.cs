using System.Collections.Generic;
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Periodic snapshot of one pawn's HUD-relevant state. Published by
/// GameBootstrap in response to a domain PawnStateChangedEvent.
/// All need values are wellness 0..1 (1 = best), passed through directly
/// from <c>NeedsComponent</c>, so the HUD can render bars without
/// further translation.
/// <see cref="TopSkills"/> carries up to 3 highest skill levels for
/// PawnDetail's SKILLS section; empty if the pawn has no skill data.
/// </summary>
public sealed record PawnStateCommand(
    EntityId PawnId,
    string Name,
    float Satiety,
    float Hydration,
    float Sleep,
    float Comfort,
    float Mood,
    string JobLabel,
    bool JobUrgent,
    IReadOnlyList<(SkillKind Kind, int Level)> TopSkills
) : IRenderCommand
{
    public void Execute(object renderContext)
    {
        /* Dispatched by RenderCommandDispatcher, not via Execute. */
    }
}
