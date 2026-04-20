using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Мана мага: текущее значение, максимум и скорость регенерации (ед./тик).
/// Стоимость заклинаний и содержания големов вычитается ManaSystem.
/// Максимум и регенерация зависят от уровня восприятия эфира
/// (см. <see cref="EtherComponent"/>, GDD 4.1 «Уровни Восприятия Эфира»).
/// </summary>
public sealed class ManaComponent : IComponent
{
    // TODO: public float Current;
    // TODO: public float Maximum;
    // TODO: public float RegenerationRate;
}
