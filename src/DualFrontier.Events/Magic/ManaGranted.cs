using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Шаг 2 двухшаговой модели: ManaSystem подтверждает списание маны.
/// Получатель <see cref="ManaIntent"/> (например SpellCastSystem) завершает
/// действие — публикует <c>SpellCastEvent</c>, активирует голема и т. п.
/// </summary>
public sealed record ManaGranted : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Amount { get; init; }
}
