using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.World;

/// <summary>
/// Параметры эфирного узла изменились (уровень, радиус, разрушен).
/// ManaSystem пересчитывает регенерацию маны у пешек в зоне влияния,
/// UI обновляет мини-карту.
/// </summary>
public sealed record EtherNodeChangedEvent : IEvent
{
    // TODO: public required EntityId NodeId { get; init; }
    // TODO: public int NewTier { get; init; }
    // TODO: public float NewRadius { get; init; }
    // TODO: public bool Destroyed { get; init; }
}
