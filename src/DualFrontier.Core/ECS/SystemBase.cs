using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Базовый класс для всех игровых систем. Наследники обязаны:
///
/// 1. Быть помечены <c>[SystemAccess(reads, writes, bus)]</c> — без атрибута
///    планировщик считает систему "пишет во всё" и полностью сериализует.
/// 2. Декларировать частоту через <c>[TickRate(TickRates.X)]</c>.
/// 3. Обращаться к компонентам строго через <see cref="GetComponent{T}"/> —
///    прямой доступ к <see cref="World"/> или другим системам = краш сторожа.
/// 4. Не запускать async/await в <see cref="Update"/>: это сбрасывает
///    ThreadLocal контекст сторожа и ломает проверки изоляции.
/// </summary>
public abstract class SystemBase
{
    /// <summary>
    /// TODO: Фаза 1 — вызывается один раз при регистрации системы.
    /// Здесь система подписывается на события своей шины через
    /// SystemExecutionContext. По умолчанию — no-op.
    /// </summary>
    protected virtual void Subscribe()
    {
    }

    /// <summary>
    /// TODO: Фаза 1 — основной метод: игровая логика на один тик.
    /// Вызывается планировщиком с частотой согласно <c>[TickRate]</c>.
    /// </summary>
    /// <param name="delta">Сколько секунд игрового времени прошло с предыдущего Update.</param>
    public abstract void Update(float delta);

    /// <summary>
    /// TODO: Фаза 1 — типобезопасный доступ к компоненту через сторожа изоляции.
    /// В DEBUG — проверяет, что <typeparamref name="T"/> задекларирован в
    /// <c>[SystemAccess(reads/writes)]</c>. Нарушение = <see cref="IsolationViolationException"/>.
    /// </summary>
    protected T GetComponent<T>(EntityId id) where T : IComponent
    {
        // TODO: Фаза 1 — делегировать в SystemExecutionContext.Current.GetComponent<T>(id).
        throw new NotImplementedException("TODO: Фаза 1 — через SystemExecutionContext");
    }
}
