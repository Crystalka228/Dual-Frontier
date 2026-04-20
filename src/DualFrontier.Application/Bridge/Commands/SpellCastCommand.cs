using DualFrontier.Contracts.Core;

namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Команда: маг <paramref name="CasterId"/> применил заклинание
/// <paramref name="SpellId"/> в точке (<paramref name="X"/>,<paramref name="Y"/>).
/// Presentation показывает VFX в зависимости от школы магии.
/// </summary>
/// <param name="CasterId">Идентификатор пешки-мага.</param>
/// <param name="SpellId">Строковый идентификатор заклинания/школы.</param>
/// <param name="X">Координата цели X.</param>
/// <param name="Y">Координата цели Y.</param>
public sealed record SpellCastCommand(
    EntityId CasterId,
    string SpellId,
    int X,
    int Y) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object godotScene)
    {
        /* TODO Фаза 5 — проиграть VFX заклинания на сцене. */
    }
}
