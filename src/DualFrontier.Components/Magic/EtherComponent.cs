using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Уровень восприятия эфира. См. GDD 4.1 «Уровни Восприятия Эфира» —
/// 5 дискретных ступеней (1 — чувствует эфир, до 5 — архимаг).
/// Определяет максимум маны, доступные школы и сложность заклинаний.
/// Повышение — через <c>EtherLevelUpEvent</c> (deferred, медитация / опыт).
/// </summary>
public sealed class EtherComponent : IComponent
{
    // TODO: public int Level;  // диапазон 1..5 — см. GDD 4.1
}
