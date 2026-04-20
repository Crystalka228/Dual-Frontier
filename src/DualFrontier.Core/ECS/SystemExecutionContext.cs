using System;
using System.Threading;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Сторож изоляции систем. В момент выполнения конкретной системы
/// <see cref="Current"/> содержит её контекст: разрешённые READ/WRITE типы,
/// имя системы (для сообщений об ошибках), ссылку на <see cref="World"/>.
///
/// Контекст хранится в <see cref="ThreadLocal{T}"/> — у каждого потока
/// планировщика свой. Планировщик устанавливает контекст перед вызовом
/// <c>SystemBase.Update</c> и снимает после.
///
/// ВАЖНО: контекст ломается при async/await внутри системы — continuation
/// может попасть на другой поток, где <see cref="Current"/> пуст. Поэтому
/// async в системах категорически запрещён.
/// См. TechArch 11.7.
/// </summary>
internal sealed class SystemExecutionContext
{
    // TODO: Фаза 1 — private static readonly ThreadLocal<SystemExecutionContext?> _current = new();
    // TODO: Фаза 1 — private readonly HashSet<Type> _allowedReads;
    // TODO: Фаза 1 — private readonly HashSet<Type> _allowedWrites;
    // TODO: Фаза 1 — private readonly string _systemName;
    // TODO: Фаза 1 — private readonly World _world;

    /// <summary>
    /// TODO: Фаза 1 — Текущий контекст исполнения для вызывающего потока.
    /// Null если поток не принадлежит планировщику (например, main thread Godot).
    /// </summary>
    public static SystemExecutionContext? Current
    {
        get => throw new NotImplementedException("TODO: Фаза 1 — ThreadLocal<SystemExecutionContext>");
    }

    /// <summary>
    /// TODO: Фаза 1 — проверенный доступ к компоненту.
    /// В DEBUG: если <typeparamref name="T"/> не в _allowedReads/_allowedWrites —
    /// <see cref="IsolationViolationException"/> с подсказкой как исправить.
    /// В RELEASE: прямой вызов <c>World.GetComponentUnsafe</c>.
    /// </summary>
    public T GetComponent<T>(EntityId id) where T : IComponent
    {
        throw new NotImplementedException("TODO: Фаза 1 — проверка allowedReads/Writes + делегирование в World");
    }

    /// <summary>
    /// TODO: Фаза 1 — всегда бросает <see cref="IsolationViolationException"/>.
    /// Прямой доступ к другой системе запрещён — используй EventBus.
    /// Метод оставлен намеренно: если кто-то попытается вызвать —
    /// исключение с понятным сообщением вместо молчаливой ошибки.
    /// </summary>
    public TSystem GetSystem<TSystem>() where TSystem : SystemBase
    {
        throw new NotImplementedException("TODO: Фаза 1 — всегда бросать IsolationViolationException");
    }
}
