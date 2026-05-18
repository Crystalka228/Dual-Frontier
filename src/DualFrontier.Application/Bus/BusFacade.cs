using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

namespace DualFrontier.Application.Bus;

/// <summary>
/// К10.2 Item 27 — Managed bus facade routing к native bus (К-L15).
///
/// Preserves <c>IModApi.Publish&lt;T&gt;</c> / <c>IModApi.Subscribe&lt;T&gt;</c>
/// signatures (К-L9 «Vanilla = mods» uniformity) while routing dispatch
/// through the native three-tier bus.
///
/// К10.2 strategy (managed-facade-preserved per К10.1 precedent):
/// <list type="bullet">
///   <item>K10.2 default: <see cref="UseNativeBusForDispatch"/> = <see langword="false"/>.
///   Managed bus continues authoritative dispatch path. Native bus runs as
///   parallel verification infrastructure (subscribers registered with both;
///   no dispatch through native unless explicitly enabled).</item>
///   <item>Sovereign authority switch (set the flag to true, retire managed
///   dispatch path) deferred к К10.4 closure or К-closure report (А'.8)
///   when full К10 infrastructure complete + integration tests verified.</item>
/// </list>
///
/// <para>
/// Event type → numeric type id mapping: K10.2 uses a stable FNV-1a hash of
/// the type's full name (assembly-independent so the same FQN от kernel
/// и от mod assemblies map к the same native type id). К-L4 (explicit
/// type ID registry) was for component types; events use a different id
/// space.
/// </para>
/// </summary>
public sealed class BusFacade
{
    private readonly ManagedBusBridge _bridge;
    private readonly ConcurrentDictionary<Type, uint> _typeIdCache = new();
    private readonly ConcurrentDictionary<Type, BusTier> _tierCache = new();

    /// <summary>
    /// К10.2 default: <see langword="false"/>. When <see langword="true"/>,
    /// publish routes through native bus instead of managed bus (К-L15
    /// sovereign authority — deferred к К10.4 closure / А'.8 К-closure).
    /// </summary>
    public bool UseNativeBusForDispatch { get; set; } = false;

    public BusFacade(ManagedBusBridge bridge)
    {
        _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
    }

    /// <summary>
    /// Stable type id for an event type. Hash of FQN (FNV-1a, 32-bit) —
    /// assembly-independent so kernel + mod assemblies declaring the same
    /// event type get the same native type id.
    /// </summary>
    public uint GetOrAssignTypeId<T>() where T : IEvent
        => _typeIdCache.GetOrAdd(typeof(T), static t => Fnv1a32(t.FullName ?? t.Name));

    /// <summary>
    /// Reads the dispatch tier from <see cref="EventTierAttribute"/>;
    /// defaults к <see cref="BusTier.Normal"/> per S-LOCK-4 backward
    /// compatibility.
    /// </summary>
    public BusTier GetTier<T>() where T : IEvent
        => _tierCache.GetOrAdd(typeof(T), static t =>
            t.GetCustomAttribute<EventTierAttribute>()?.Tier ?? BusTier.Normal);

    /// <summary>
    /// Registers the event type in the native event type registry (Item 28)
    /// и records the assignment в <see cref="EventTypeRegistryInterop"/>.
    /// Idempotent: re-registering с identical metadata is a no-op.
    /// </summary>
    public bool RegisterEventType<T>(IntPtr coalesceFnPtr = default) where T : IEvent
    {
        uint typeId = GetOrAssignTypeId<T>();
        BusTier tier = GetTier<T>();
        int payloadSize = MarshallableSizeOrDefault<T>();
        bool ok = EventTypeRegistryInterop.Register(
            typeId, tier, payloadSize, typeof(T).FullName ?? typeof(T).Name, coalesceFnPtr);
        return ok;
    }

    /// <summary>
    /// Publishes an event с tier-aware dispatch when
    /// <see cref="UseNativeBusForDispatch"/> is set. Otherwise no-op (managed
    /// bus authoritative path stays the caller's responsibility per К10.2
    /// managed-facade-preserved strategy).
    /// </summary>
    /// <returns>
    /// Count of subscribers invoked (Fast tier); 1 если queued (Normal/
    /// Background tier); 0 if dispatch disabled or no subscribers.
    /// </returns>
    public int Publish<T>(T evt) where T : unmanaged, IEvent
    {
        if (!UseNativeBusForDispatch) return 0;
        uint typeId = GetOrAssignTypeId<T>();
        BusTier tier = GetTier<T>();
        unsafe
        {
            T local = evt;
            return _bridge.PublishViaNative(typeId, tier, (IntPtr)(&local), (uint)sizeof(T));
        }
    }

    /// <summary>
    /// Drains Normal tier batched dispatch (called by scheduler at phase
    /// boundary). К10.2 default: caller (managed adapter) explicitly invokes
    /// this if <see cref="UseNativeBusForDispatch"/> = true; otherwise managed
    /// bus path handles dispatch independently.
    /// </summary>
    public int DrainNormalBatch() => _bridge.DrainNormalBatch();

    /// <summary>
    /// Diagnostic: subscriber count в the native bus for the given event
    /// type's tier. Always reflects native registry (even when dispatch
    /// disabled).
    /// </summary>
    public int NativeSubscriberCount<T>() where T : IEvent
    {
        uint typeId = GetOrAssignTypeId<T>();
        BusTier tier = GetTier<T>();
        return _bridge.SubscriberCount(tier, typeId);
    }

    private static int MarshallableSizeOrDefault<T>() where T : IEvent
    {
        try { return Marshal.SizeOf<T>(); }
        catch { return 0; }
    }

    private static uint Fnv1a32(string s)
    {
        const uint FNV_OFFSET = 2166136261u;
        const uint FNV_PRIME  = 16777619u;
        uint h = FNV_OFFSET;
        foreach (char ch in s)
        {
            h ^= ch;
            h *= FNV_PRIME;
        }
        return h;
    }
}
