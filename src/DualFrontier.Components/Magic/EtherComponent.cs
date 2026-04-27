using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Ether-perception level. See GDD 4.1 "Ether Perception Tiers" —
/// 5 discrete tiers (1 — senses the ether, up to 5 — archmage).
/// Determines mana cap, accessible schools, and spell complexity.
/// Raised via <c>EtherLevelUpEvent</c> (deferred, meditation / experience).
/// </summary>
public sealed class EtherComponent : IComponent
{
    // TODO: public int Level;  // range 1..5 — see GDD 4.1
}
