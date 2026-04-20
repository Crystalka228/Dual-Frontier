using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина мирового домена. События: изменение эфирных узлов, смена погоды,
/// приближающийся рейд, изменение биома.
/// Пишут: <c>BiomeSystem</c>, <c>WeatherSystem</c>.
/// Читают: <c>EtherGridSystem</c>, <c>RaidSystem</c>.
/// </summary>
public interface IWorldBus : IEventBus
{
}
