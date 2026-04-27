using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Magic-domain bus. Events: mana request, spell cast, golem activation,
/// ether surge (EtherSurge), mage level-up.
/// Writers: <c>SpellSystem</c>, <c>GolemSystem</c>.
/// Readers: <c>ManaSystem</c>, <c>EtherGrowthSystem</c>.
/// </summary>
public interface IMagicBus : IEventBus
{
}
