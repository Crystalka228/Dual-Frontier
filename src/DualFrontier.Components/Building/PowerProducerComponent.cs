using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Building;

/// <summary>
/// Постройка производит энергию заданного типа. Выработка добавляется
/// в баланс сети PowerSystem каждый тик.
/// </summary>
public sealed class PowerProducerComponent : IComponent
{
    // TODO: public PowerType Type;  // см. PowerConsumerComponent
    // TODO: public float Output;    // ед. энергии / тик
}
