using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина магического домена. События: запрос маны, каст заклинания,
/// активация голема, срыв эфира (EtherSurge), рост уровня мага.
/// Пишут: <c>SpellSystem</c>, <c>GolemSystem</c>.
/// Читают: <c>ManaSystem</c>, <c>EtherGrowthSystem</c>.
/// </summary>
public interface IMagicBus : IEventBus
{
}
