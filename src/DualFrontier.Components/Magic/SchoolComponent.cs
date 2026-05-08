using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Magic levels per school. See GDD 4.3 "Magic Schools" — a mage goes deep
/// into a school rather than sampling all of them (depth beats breadth).
/// GDD 4.4 "Multi-school" describes the penalties for spreading thin.
/// </summary>
public struct SchoolComponent : IComponent
{
    // TODO: introduce DualFrontier.Components.Magic.MagicSchool enum
    //       (Fire, Ice, Storm, Earth, Wind, Water, Dark, Light, Mind, Void) — GDD 4.3, Phase 6.
    // TODO: public Dictionary<MagicSchool, int> SchoolLevels = new();
}
