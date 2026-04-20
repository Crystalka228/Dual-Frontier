using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: заспавнился снаряд <paramref name="ProjectileId"/> из точки
/// (<paramref name="FromX"/>,<paramref name="FromY"/>) в точку
/// (<paramref name="ToX"/>,<paramref name="ToY"/>). Presentation
/// создаёт визуал и запускает анимацию полёта.
/// </summary>
/// <param name="ProjectileId">Идентификатор entity снаряда в домене.</param>
/// <param name="FromX">Стартовая координата X.</param>
/// <param name="FromY">Стартовая координата Y.</param>
/// <param name="ToX">Целевая координата X.</param>
/// <param name="ToY">Целевая координата Y.</param>
public sealed record ProjectileSpawnedCommand(
    EntityId ProjectileId,
    int FromX,
    int FromY,
    int ToX,
    int ToY) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object godotScene)
    {
        /* TODO Фаза 5 — создать визуал снаряда и анимировать траекторию. */
    }
}
