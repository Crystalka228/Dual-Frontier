using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Periodic snapshot of one entity's HUD-relevant state.
///
/// <para>
/// <b>Nothing emits this today.</b> The engine used to translate a domain
/// PawnStateChangedEvent into this command from inside its composition root, which is one of the
/// reasons the engine had to name the game's component and event assemblies. That translation is
/// gone with the composition root; the reporter system still publishes its domain event, and the
/// mod that registers it does not turn it into a render command.
/// </para>
///
/// <para>
/// The record is kept rather than deleted because it is the only description the codebase has of
/// what a pawn-detail HUD needs, and the UI program that will consume it is fenced out of this
/// wave. Deleting it would decide the HUD's payload by omission; inventing an engine-generic HUD
/// contract here would decide it by invention. Both are the UI program's call.
/// </para>
///
/// <para>
/// The one change made here is the removal of the game vocabulary: the skill identity was a game
/// enum, and an engine assembly may not name one (boundary law B-2). It is an opaque int, which
/// is what a renderer can do with it anyway.
/// </para>
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
    IReadOnlyList<(int Kind, int Level)> TopSkills
) : IRenderCommand;
