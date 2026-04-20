using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Шаг 2 двухшаговой модели (TechArch 11.5): InventorySystem отказывает
/// в выдаче патронов — на складе пусто или резерв занят другим запросом.
/// CombatSystem должна отменить выстрел и, возможно, сменить тактику.
/// </summary>
public sealed record AmmoRefused : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required AmmoType AmmoType { get; init; }
    // TODO: public string Reason { get; init; } = string.Empty;  // для отладки/лога
}
