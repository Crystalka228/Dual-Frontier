using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Mage's mana: current value, maximum, and regeneration rate (units/tick).
/// Spell costs and golem upkeep are deducted by ManaSystem.
/// Maximum and regeneration depend on the ether-perception level
/// (see <see cref="EtherComponent"/>, GDD 4.1 "Ether Perception Tiers").
/// </summary>
public sealed class ManaComponent : IComponent
{
    // TODO: public float Current;
    // TODO: public float Maximum;
    // TODO: public float RegenerationRate;
}
