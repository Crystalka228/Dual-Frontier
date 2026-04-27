using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: mage <paramref name="CasterId"/> cast spell
/// <paramref name="SpellId"/> at point (<paramref name="X"/>,<paramref name="Y"/>).
/// Presentation displays the VFX based on the magic school.
/// </summary>
/// <param name="CasterId">Identifier of the casting mage pawn.</param>
/// <param name="SpellId">String identifier of the spell/school.</param>
/// <param name="X">Target X coordinate.</param>
/// <param name="Y">Target Y coordinate.</param>
public sealed record SpellCastCommand(
    EntityId CasterId,
    string SpellId,
    int X,
    int Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* TODO Phase 5 — apply via active IRenderer backend. */
    }
}
