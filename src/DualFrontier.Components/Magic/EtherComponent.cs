using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Ether-perception level. See GDD 4.1 "Ether Perception Tiers" —
/// 5 discrete tiers (1 — senses the ether, up to 5 — archmage).
/// Determines mana cap, accessible schools, and spell complexity.
/// Raised via <c>EtherLevelUpEvent</c> (deferred, meditation / experience).
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct EtherComponent : IComponent
{
    /// <summary>Ether-perception tier (1..5). See GDD 4.1.</summary>
    public int Level;
}
