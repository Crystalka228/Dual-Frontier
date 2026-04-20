using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Неизменяемый идентификатор сущности.
/// Состоит из индекса <see cref="Id"/> в массиве компонентов ECS и
/// поля <see cref="Version"/>, которое инкрементируется при уничтожении
/// сущности.
///
/// TODO: Фаза 1 — Version используется для обнаружения "мёртвых" ссылок.
/// Если внешняя система сохранила EntityId, а затем entity была уничтожена
/// и новая entity переиспользовала тот же слот, версия у новой будет больше.
/// Сравнение версий позволяет отличить "эту же" entity от "новой в том же слоте".
/// </summary>
public readonly record struct EntityId(int Id, int Version)
{
    /// <summary>
    /// Специальное значение "нет сущности" — эквивалент null для reference-типов.
    /// Используется как возврат из методов вида <c>TryGet</c> когда результата нет.
    /// </summary>
    public static EntityId Invalid => default;
}
