using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Реализация <see cref="IModApi"/>, которую <see cref="ModLoader"/> передаёт
/// каждому моду в <c>IMod.Initialize</c>. Проксирует вызовы в ядро с
/// дополнительными проверками (права доступа, квоты, трассировка).
///
/// Мод НЕ должен кастить <see cref="IModApi"/> к этому типу; <see cref="ModLoader"/>
/// обнаруживает такие попытки и выгружает мод с <see cref="ModIsolationException"/>.
/// </summary>
internal sealed class RestrictedModApi : IModApi
{
    /// <inheritdoc />
    public void RegisterComponent<T>() where T : IComponent
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }

    /// <inheritdoc />
    public void RegisterSystem<T>() where T : class
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }

    /// <inheritdoc />
    public void Publish<T>(T evt) where T : IEvent
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }

    /// <inheritdoc />
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }

    /// <inheritdoc />
    public void PublishContract<T>(T contract) where T : IModContract
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }

    /// <inheritdoc />
    public bool TryGetContract<T>(out T? contract) where T : class, IModContract
    {
        throw new NotImplementedException("TODO: Фаза 2 — проксирование через IModApi");
    }
}
