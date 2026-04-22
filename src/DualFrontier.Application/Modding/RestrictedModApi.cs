using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Implementation of <see cref="IModApi"/> that <see cref="ModLoader"/> hands
/// to each mod in <c>IMod.Initialize</c>. Proxies calls into the core
/// through <see cref="ModRegistry"/> and <see cref="IModContractStore"/>.
///
/// A mod MUST NOT cast <see cref="IModApi"/> to this concrete type;
/// <see cref="ModLoader"/> detects such attempts and unloads the mod with a
/// <see cref="ModIsolationException"/>.
///
/// Subscriptions registered through <see cref="Subscribe{T}"/> are tracked
/// per instance so <see cref="UnsubscribeAll"/> can release them when the
/// mod is unloaded. Publish/Subscribe routing between concrete domain buses
/// is deferred to a later phase — this class currently records subscriptions
/// without touching live buses, which is sufficient for registration and
/// contract flows.
/// </summary>
internal sealed class RestrictedModApi : IModApi
{
    private readonly string _modId;
    private readonly ModRegistry _registry;
    private readonly IModContractStore _contractStore;
    private readonly IGameServices _services;
    private readonly List<(Type EventType, Delegate Handler)> _subscriptions = new();

    /// <summary>
    /// Creates an API instance bound to the given mod id, backing registry
    /// and contract store. <paramref name="services"/> is kept for the later
    /// wire-up of <see cref="Publish{T}"/>/<see cref="Subscribe{T}"/> into
    /// concrete domain buses.
    /// </summary>
    internal RestrictedModApi(
        string modId,
        ModRegistry registry,
        IModContractStore contractStore,
        IGameServices services)
    {
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    /// <inheritdoc />
    public void RegisterComponent<T>() where T : IComponent
        => _registry.RegisterComponent(_modId, typeof(T));

    /// <inheritdoc />
    public void RegisterSystem<T>() where T : class
        => _registry.RegisterSystem(_modId, typeof(T));

    /// <inheritdoc />
    public void Publish<T>(T evt) where T : IEvent
    {
        if (evt is null) throw new ArgumentNullException(nameof(evt));
        // Маршрутизация событий по шинам вводится в следующей подфазе.
        // До неё Publish остаётся легальным no-op, чтобы моды могли тестироваться
        // без реального диспатчера.
        _ = _services;
    }

    /// <inheritdoc />
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        _subscriptions.Add((typeof(T), handler));
    }

    /// <inheritdoc />
    public void PublishContract<T>(T contract) where T : IModContract
    {
        if (contract is null) throw new ArgumentNullException(nameof(contract));
        _contractStore.Publish<T>(_modId, contract);
    }

    /// <inheritdoc />
    public bool TryGetContract<T>(out T? contract) where T : class, IModContract
        => _contractStore.TryGet<T>(out contract);

    /// <summary>
    /// Removes every subscription the mod had registered through
    /// <see cref="Subscribe{T}"/>. Called from the unload chain.
    /// </summary>
    internal void UnsubscribeAll()
    {
        _subscriptions.Clear();
    }

    /// <summary>
    /// Returns the number of active subscriptions — intended for tests and
    /// for the unload chain to assert the cleanup invariant.
    /// </summary>
    internal int SubscriptionCount => _subscriptions.Count;
}
