using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Engine-side implementation of the W1 SDK per-tick context
/// (<see cref="ISystemContext"/>) and the internal
/// <see cref="IWriteBatchCapability"/> its write scope delegates to. One instance
/// per adapted system, REUSED across ticks (no per-tick allocation): world data
/// is read LIVE from the active <see cref="SystemExecutionContext"/> each call,
/// so the context is always current without minting a new object per tick.
///
/// <para>
/// Reaches the world exactly as <see cref="SystemBase"/> does — through
/// <see cref="SystemExecutionContext.Current"/> — so out-of-context access fails
/// loudly. Events route through the owning mod's <see cref="RestrictedModApi"/>
/// (resolved by mod id), inheriting the live capability gate and unload-time
/// unsubscribe tracking (W1 decision: wire the existing gate, do not duplicate).
/// The <c>Core.Interop</c> objects (lease/batch/interned string/composite) are
/// created here and handed out only through the Contracts-safe wrappers.
/// </para>
/// </summary>
internal sealed class SystemContextView : ISystemContext, IWriteBatchCapability
{
    private readonly ModRegistry _registry;
    private readonly string _modId;
    private readonly Func<long> _currentTick;

    internal SystemContextView(ModRegistry registry, string modId, Func<long> currentTick)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _modId = modId ?? throw new ArgumentNullException(nameof(modId));
        _currentTick = currentTick ?? throw new ArgumentNullException(nameof(currentTick));
    }

    private static NativeWorld World
        => (SystemExecutionContext.Current
            ?? throw new InvalidOperationException(
                "ISystemContext world access outside an active scheduler context."))
            .NativeWorld;

    public long CurrentTick => _currentTick();

    // ---- Component access: per-id ----

    public bool TryGetComponent<T>(EntityId id, out T value) where T : unmanaged, IComponent
        => World.TryGetComponent<T>(id, out value);

    public bool HasComponent<T>(EntityId id) where T : unmanaged, IComponent
        => World.HasComponent<T>(id);

    public T GetComponent<T>(EntityId id) where T : unmanaged, IComponent
        => World.GetComponent<T>(id);

    // ---- Component access: bulk (allocation-free scopes) ----

    public SpanScope<T> AcquireSpan<T>() where T : unmanaged, IComponent
    {
        SpanLease<T> lease = World.AcquireSpan<T>();
        return new SpanScope<T>(lease, lease.Span, lease.Indices);
    }

    public WriteScope<T> BeginBatch<T>() where T : unmanaged, IComponent
        => new WriteScope<T>(this, World.BeginBatch<T>());

    // ---- String interning ----

    public StringHandle InternString(string content)
    {
        InternedString interned = World.InternString(content);
        return new StringHandle(interned.Id, interned.Generation);
    }

    public string? Resolve(StringHandle handle)
        => new InternedString(handle.Id, handle.Generation).Resolve(World);

    // ---- Composites ----

    public CompositeHandle<T> CreateComposite<T>() where T : unmanaged
        => new CompositeHandle<T>(World.CreateComposite<T>().CompositeId);

    public bool CompositeAdd<T>(CompositeHandle<T> composite, EntityId entity, T value) where T : unmanaged
        => World.GetComposite<T>(composite.CompositeId).Add(entity, value);

    public bool CompositeTryGetAt<T>(CompositeHandle<T> composite, EntityId entity, int index, out T value) where T : unmanaged
        => World.GetComposite<T>(composite.CompositeId).TryGetAt(entity, index, out value);

    public int CompositeCountFor<T>(CompositeHandle<T> composite, EntityId entity) where T : unmanaged
        => World.GetComposite<T>(composite.CompositeId).CountFor(entity);

    public bool CompositeClearFor<T>(CompositeHandle<T> composite, EntityId entity) where T : unmanaged
        => World.GetComposite<T>(composite.CompositeId).ClearFor(entity);

    // ---- Events (routed through the live capability gate) ----

    public void Publish<T>(T evt) where T : IEvent => RequireApi().Publish(evt);

    public void Subscribe<T>(Action<T> handler) where T : IEvent => RequireApi().Subscribe(handler);

    private RestrictedModApi RequireApi()
        => _registry.GetModApi(_modId)
           ?? throw new InvalidOperationException(
               $"ISystemContext events require the mod API for '{_modId}', which is not registered.");

    // ---- IWriteBatchCapability: the engine seam WriteScope delegates to ----

    bool IWriteBatchCapability.Update<T>(object batch, EntityId entity, T value)
        => ((WriteBatch<T>)batch).Update(entity, value);

    bool IWriteBatchCapability.Add<T>(object batch, EntityId entity, T value)
        => ((WriteBatch<T>)batch).Add(entity, value);

    bool IWriteBatchCapability.Remove<T>(object batch, EntityId entity)
        => ((WriteBatch<T>)batch).Remove(entity);

    int IWriteBatchCapability.Flush<T>(object batch)
        => ((WriteBatch<T>)batch).Flush();

    void IWriteBatchCapability.Cancel<T>(object batch)
        => ((WriteBatch<T>)batch).Cancel();
}
