using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Mage's mana: current value, maximum, and regeneration rate (units/tick).
/// Spell costs and golem upkeep are deducted by ManaSystem.
/// Maximum and regeneration depend on the ether-perception level
/// (see <see cref="EtherComponent"/>, GDD 4.1 "Ether Perception Tiers").
/// </summary>
public struct ManaComponent : IComponent
{
    public float Current;
    public float Maximum;
    public float RegenerationRate;
}
