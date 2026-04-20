using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Modding;

/// <summary>
/// API, предоставляемый моду ядром. Единственная точка, через которую
/// мод может влиять на игру. Реальная реализация (<c>RestrictedModApi</c>)
/// живёт в <c>DualFrontier.Application</c> и проксирует вызовы в ядро
/// с дополнительными проверками (права доступа, квоты, логирование).
///
/// Мод НЕ имеет права кастить этот интерфейс к конкретному типу —
/// ModLoader обнаружит попытку и выгрузит мод с ошибкой.
/// </summary>
public interface IModApi
{
    /// <summary>
    /// TODO: Фаза 2 — Регистрирует новый тип компонента в ECS.
    /// После регистрации компонент можно добавлять на entity и
    /// декларировать в <c>[SystemAccess]</c> систем мода.
    /// </summary>
    void RegisterComponent<T>() where T : IComponent;

    /// <summary>
    /// TODO: Фаза 2 — Регистрирует новую систему.
    /// Система должна быть уже сконфигурирована через <c>[SystemAccess]</c>
    /// и <c>[TickRate]</c>. Планировщик перестроит граф фаз.
    /// </summary>
    void RegisterSystem<T>() where T : class;

    /// <summary>
    /// TODO: Фаза 2 — Публикует событие в соответствующую доменную шину.
    /// ModApi определяет шину по типу события (по маркерам/атрибутам).
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// TODO: Фаза 2 — Подписка на события типа T.
    /// ModApi отслеживает все подписки мода и снимает их при Unload.
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    /// <summary>
    /// TODO: Фаза 2 — Публикует контракт для других модов.
    /// Другие моды могут получить его через <see cref="TryGetContract{T}"/>.
    /// Это единственный способ коммуникации между модами.
    /// </summary>
    void PublishContract<T>(T contract) where T : IModContract;

    /// <summary>
    /// TODO: Фаза 2 — Попытка получить контракт другого мода.
    /// Возвращает <c>true</c> если контракт зарегистрирован. Если мод,
    /// публикующий контракт, не загружен — возвращает <c>false</c>:
    /// зависящий мод должен gracefully degrade.
    /// </summary>
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;
}
