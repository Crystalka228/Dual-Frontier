using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Шаг 1 двухшаговой модели (TechArch 11.5): CombatSystem публикует
/// намерение получить патрон. InventorySystem собирает все AmmoIntent
/// в фазе сбора и разрешает батчем в следующей фазе.
///
/// НЕ путать с блокирующим Request — этот event не ждёт ответа.
/// Ответ придёт в виде <see cref="AmmoGranted"/> или <see cref="AmmoRefused"/> позже.
/// </summary>
public sealed record AmmoIntent : IEvent
{
    // TODO: public required EntityId RequesterId { get; init; }
    // TODO: public required AmmoType AmmoType { get; init; }   // см. Components/Combat/AmmoComponent
    // TODO: public required GridVector Position { get; init; } // см. Components/Shared/PositionComponent
}
