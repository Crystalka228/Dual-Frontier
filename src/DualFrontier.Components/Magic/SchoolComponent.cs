using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Уровни магии по школам. См. GDD 4.3 «Школы Магии» —
/// маг углубляется в школу, а не изучает всё подряд (глубина важнее ширины).
/// GDD 4.4 «Мультишкольность» описывает штрафы при распылении.
/// </summary>
public sealed class SchoolComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Magic.MagicSchool enum
    //       (Fire, Ice, Storm, Earth, Wind, Water, Dark, Light, Mind, Void) — GDD 4.3, Фаза 6.
    // TODO: public Dictionary<MagicSchool, int> SchoolLevels = new();
}
