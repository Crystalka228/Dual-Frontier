using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Заклинание успешно произнесено. Публикуется SpellCastSystem после
/// получения <see cref="ManaGranted"/>. Подписчики: визуал (эффекты),
/// DamageSystem (боевые заклинания), StatusSystem (наложение эффектов),
/// SkillSystem (прокачка школы).
/// </summary>
public sealed record SpellCastEvent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public EntityId? TargetId { get; init; }
    // TODO: public required MagicSchool School { get; init; }    // enum в Components.Magic
    // TODO: public required int SpellTier { get; init; }
    // TODO: public GridVector? TargetPosition { get; init; }     // для AoE-заклинаний
}
