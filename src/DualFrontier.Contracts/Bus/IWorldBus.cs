using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// World-domain bus. Events: ether-node changes, weather changes, incoming
/// raid, biome change.
/// Writers: <c>BiomeSystem</c>, <c>WeatherSystem</c>.
/// Readers: <c>EtherGridSystem</c>, <c>RaidSystem</c>.
/// </summary>
public interface IWorldBus : IEventBus
{
}
