using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Spell successfully cast. Published by SpellCastSystem after
/// receiving <see cref="ManaGranted"/>. Subscribers: visuals (effects),
/// DamageSystem (combat spells), StatusSystem (applying effects),
/// SkillSystem (school progression).
/// </summary>
public sealed record SpellCastEvent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public EntityId? TargetId { get; init; }
    // TODO: public required MagicSchool School { get; init; }    // enum in Components.Magic
    // TODO: public required int SpellTier { get; init; }
    // TODO: public GridVector? TargetPosition { get; init; }     // for AoE spells
}
