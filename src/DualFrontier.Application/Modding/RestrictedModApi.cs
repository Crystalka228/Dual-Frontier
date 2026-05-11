using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Implementation of <see cref="IModApi"/> that <see cref="ModLoader"/> hands
/// to each mod in <c>IMod.Initialize</c>. Proxies calls into the core
/// through <see cref="ModRegistry"/> and <see cref="IModContractStore"/>,
/// and routes <see cref="Publish{T}"/>/<see cref="Subscribe{T}"/> through
/// <see cref="ModBusRouter"/> to the correct domain bus.
///
/// A mod MUST NOT cast <see cref="IModApi"/> to this concrete type;
/// <see cref="ModLoader"/> detects such attempts and unloads the mod with a
/// <see cref="ModIsolationException"/>.
///
/// Capability gates are enforced from the v2 manifest: a mod must declare
/// <c>kernel.publish:&lt;FQN&gt;</c> / <c>kernel.subscribe:&lt;FQN&gt;</c> in
/// <see cref="ManifestCapabilities.Required"/> for every event type it
/// publishes or subscribes. v1 manifests (empty <see cref="ManifestCapabilities"/>)
/// bypass the gate with a deprecation warning to <see cref="Console"/>.
///
/// Subscriptions registered through <see cref="Subscribe{T}"/> are tracked
/// per instance so <see cref="UnsubscribeAll"/> can release them when the
/// mod is unloaded.
/// </summary>
internal sealed class RestrictedModApi : IModApi
{
    private readonly string _modId;
    private readonly ModManifest _manifest;
    private readonly ModRegistry _registry;
    private readonly IModContractStore _contractStore;
    private readonly IGameServices _services;
    private readonly KernelCapabilityRegistry _kernelCapabilities;
    private readonly List<(IEventBus Bus, Action Unsubscribe)> _subscriptions = new();

    /// <summary>
    /// Creates an API instance bound to the given mod id, manifest, backing
    /// registry and contract store. <paramref name="services"/> is the bus
    /// aggregator used by <see cref="Publish{T}"/>/<see cref="Subscribe{T}"/>.
    /// <paramref name="kernelCapabilities"/> is the catalogue returned to the
    /// mod by <see cref="GetKernelCapabilities"/> so it can inspect which
    /// kernel capabilities are available before declaring them.
    /// </summary>
    internal RestrictedModApi(
        string modId,
        ModManifest manifest,
        ModRegistry registry,
        IModContractStore contractStore,
        IGameServices services,
        KernelCapabilityRegistry kernelCapabilities)
    {
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
        _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _kernelCapabilities = kernelCapabilities ?? throw new ArgumentNullException(nameof(kernelCapabilities));
    }

    /// <summary>
    /// Returns the number of active subscriptions — intended for tests and
    /// for the unload chain to assert the cleanup invariant.
    /// </summary>
    internal int SubscriptionCount => _subscriptions.Count;

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

        EnforceCapability("publish", typeof(T));

        if (ModBusRouter.Resolve(typeof(T), _services) is IEventBus bus)
            bus.Publish(evt);
    }

    /// <inheritdoc />
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));

        EnforceCapability("subscribe", typeof(T));

        if (ModBusRouter.Resolve(typeof(T), _services) is not IEventBus bus)
            return;

        SystemExecutionContext? captured = SystemExecutionContext.Current;
        Action<T> wrapped = captured is null
            ? handler
            : evt =>
            {
                SystemExecutionContext.PushContext(captured);
                try { handler(evt); }
                finally { SystemExecutionContext.PopContext(); }
            };

        bus.Subscribe(wrapped);
        _subscriptions.Add((bus, () => bus.Unsubscribe(wrapped)));
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

    /// <inheritdoc />
    public IReadOnlySet<string> GetKernelCapabilities() => _kernelCapabilities.Capabilities;

    /// <inheritdoc />
    public ModManifest GetOwnManifest() => _manifest;

    /// <inheritdoc />
    public void Log(ModLogLevel level, string message)
        => Console.WriteLine($"[{level.ToString().ToUpperInvariant()}][{_modId}] {message}");

    /// <inheritdoc />
    public IModFieldApi? Fields => null;

    /// <inheritdoc />
    public IModComputePipelineApi? ComputePipelines => null;

    /// <summary>
    /// Removes every subscription the mod had registered through
    /// <see cref="Subscribe{T}"/> from the underlying buses, then clears the
    /// tracking list. Called from the unload chain.
    /// </summary>
    internal void UnsubscribeAll()
    {
        foreach ((IEventBus _, Action unsubscribe) in _subscriptions)
            unsubscribe();
        _subscriptions.Clear();
    }

    private void EnforceCapability(string verb, Type eventType)
    {
        string token = $"kernel.{verb}:{eventType.FullName}";

        if (_manifest.Capabilities.IsEmpty)
        {
            Console.WriteLine(
                $"[WARNING][{_modId}] v1 manifest {verb}ing '{eventType.FullName}' " +
                "without capability declaration (v1 manifests are accepted in a grace period; " +
                "v2 manifests must declare every kernel.publish/subscribe in capabilities.required).");
            return;
        }

        if (!_manifest.Capabilities.RequiresCapability(token))
            throw new CapabilityViolationException(
                $"Mod '{_modId}' attempted to {verb} '{eventType.FullName}' " +
                $"without declaring capability '{token}'.");
    }
}
