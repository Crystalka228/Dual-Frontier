using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Шаг 1 двухшаговой модели (TechArch 11.5): система публикует намерение
/// потратить ману (каст заклинания, содержание голема, ритуал).
/// ManaSystem отвечает <see cref="ManaGranted"/> или <see cref="ManaRefused"/>.
/// </summary>
public sealed record ManaIntent : IEvent
{
    // TODO: public required EntityId CasterId { get; init; }
    // TODO: public required float Amount { get; init; }
    // TODO: public string Purpose { get; init; } = string.Empty;  // для отладки/аналитики
}
