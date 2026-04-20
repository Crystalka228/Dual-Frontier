using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Шаг 2 двухшаговой модели (TechArch 11.5): InventorySystem подтверждает,
/// что патрон выдан по ранее поступившему <see cref="AmmoIntent"/>.
/// CombatSystem, получив это событие, завершает выстрел.
/// </summary>
public sealed record AmmoGranted : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required AmmoType AmmoType { get; init; }
    // TODO: public required int Count { get; init; }
}
