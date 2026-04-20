using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building;

/// <summary>
/// Постройка потребляет энергию заданного типа (электричество или эфир).
/// PowerSystem списывает <c>WattsPerTick</c> из соответствующей сети,
/// при нехватке публикует <c>GridOverloadEvent</c>.
/// </summary>
public sealed class PowerConsumerComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Building.PowerType enum (Electric, Ether) — Фаза 2.
    // TODO: public PowerType Type;
    // TODO: public float WattsPerTick;
}
