using System;
using System.Runtime.CompilerServices;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed handle over <c>dualfrontier::World</c>.
///
/// Mirrors the subset of <c>DualFrontier.Core.ECS.World</c> exercised by
/// the proof-of-concept benchmark: entity lifecycle, add/get/has/remove of
/// blittable components, and the deferred-destroy flush.
///
/// Constraints:
///   * Component type <c>T</c> must be <c>unmanaged</c>. Reference-type
///     components cannot cross the P/Invoke boundary without GCHandle
///     pinning, which was deliberately deferred — see
///     <c>docs/NATIVE_CORE.md</c> for the rationale.
///   * Not thread-safe. The native world matches the managed one's API but
///     drops the <c>ConcurrentDictionary</c> guard; do not share an instance
///     between threads for the PoC.
///   * Disposal is mandatory. Dropping a <see cref="NativeWorld"/> without
///     calling <see cref="Dispose"/> leaks the underlying C++ world.
/// </summary>
public sealed class NativeWorld : IDisposable
{
    private IntPtr _handle;
    private readonly ComponentTypeRegistry? _registry;

    /// <summary>
    /// Internal handle access for <see cref="SpanLease{T}"/> lifetime
    /// management. Not for public consumption — use
    /// <see cref="AcquireSpan{T}"/> instead.
    /// </summary>
    internal IntPtr HandleForInternalUse => _handle;

    /// <summary>
    /// Test-only accessor (visible via InternalsVisibleTo to the test project).
    /// Same handle as <see cref="HandleForInternalUse"/>; named distinctly so
    /// the intended consumer (cross-assembly tests) is obvious at the call site.
    /// </summary>
    internal IntPtr HandleForInternalUseTest => _handle;

    /// <summary>
    /// The component type registry bound to this world, if any. Null when the
    /// world was constructed without a registry (legacy FNV-1a path).
    /// </summary>
    public ComponentTypeRegistry? Registry => _registry;

    /// <summary>
    /// Creates a NativeWorld with FNV-1a fallback type ids (legacy path).
    /// Equivalent to passing <c>null</c> for the registry. Retained for
    /// backward compatibility — new code should pass an explicit registry.
    /// </summary>
    public NativeWorld() : this(null) { }

    /// <summary>
    /// Creates a NativeWorld with an explicit <see cref="ComponentTypeRegistry"/>
    /// (K2 recommended path).
    /// </summary>
    /// <param name="registry">
    /// If null, uses FNV-1a hash ids (legacy). If provided, uses deterministic
    /// sequential ids per K-L4. The registry binds to this world's handle.
    /// </param>
    public NativeWorld(ComponentTypeRegistry? registry)
    {
        _handle = NativeMethods.df_world_create();
        if (_handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "df_world_create returned null — native library failed to allocate a World.");
        }
        _registry = registry;
    }

    public EntityId CreateEntity()
    {
        ThrowIfDisposed();
        ulong packed = NativeMethods.df_world_create_entity(_handle);
        return EntityIdPacking.Unpack(packed);
    }

    public void DestroyEntity(EntityId id)
    {
        ThrowIfDisposed();
        NativeMethods.df_world_destroy_entity(_handle, EntityIdPacking.Pack(id));
    }

    public bool IsAlive(EntityId id)
    {
        ThrowIfDisposed();
        return NativeMethods.df_world_is_alive(_handle, EntityIdPacking.Pack(id)) != 0;
    }

    public int EntityCount
    {
        get
        {
            ThrowIfDisposed();
            return NativeMethods.df_world_entity_count(_handle);
        }
    }

    public void FlushDestroyedEntities()
    {
        ThrowIfDisposed();
        NativeMethods.df_world_flush_destroyed(_handle);
    }

    public unsafe void AddComponent<T>(EntityId id, T value) where T : unmanaged
    {
        ThrowIfDisposed();
        uint typeId = ResolveTypeId<T>();
        NativeMethods.df_world_add_component(
            _handle,
            EntityIdPacking.Pack(id),
            typeId,
            &value,
            ResolveTypeSize<T>());
    }

    public unsafe bool TryGetComponent<T>(EntityId id, out T value) where T : unmanaged
    {
        ThrowIfDisposed();
        T tmp = default;
        int ok = NativeMethods.df_world_get_component(
            _handle,
            EntityIdPacking.Pack(id),
            ResolveTypeId<T>(),
            &tmp,
            ResolveTypeSize<T>());
        value = tmp;
        return ok != 0;
    }

    public T GetComponent<T>(EntityId id) where T : unmanaged
    {
        if (!TryGetComponent(id, out T value))
        {
            throw new InvalidOperationException(
                $"Component {typeof(T).Name} not found for entity {id}.");
        }
        return value;
    }

    public bool HasComponent<T>(EntityId id) where T : unmanaged
    {
        ThrowIfDisposed();
        return NativeMethods.df_world_has_component(
            _handle,
            EntityIdPacking.Pack(id),
            ResolveTypeId<T>()) != 0;
    }

    public void RemoveComponent<T>(EntityId id) where T : unmanaged
    {
        ThrowIfDisposed();
        NativeMethods.df_world_remove_component(
            _handle,
            EntityIdPacking.Pack(id),
            ResolveTypeId<T>());
    }

    public int GetComponentCount<T>() where T : unmanaged
    {
        ThrowIfDisposed();
        return NativeMethods.df_world_component_count(
            _handle,
            ResolveTypeId<T>());
    }

    /// <summary>
    /// Bulk add: transmits an array of entities and components in a single
    /// P/Invoke crossing, eliminating per-entity overhead for batch
    /// initialization scenarios.
    /// </summary>
    /// <typeparam name="T">Unmanaged component type.</typeparam>
    /// <param name="entities">Entities to add components to. Length must match <paramref name="components"/>.</param>
    /// <param name="components">Component values. Length must match <paramref name="entities"/>.</param>
    /// <exception cref="ArgumentException">If the spans have different lengths.</exception>
    public unsafe void AddComponents<T>(ReadOnlySpan<EntityId> entities,
                                        ReadOnlySpan<T> components) where T : unmanaged
    {
        ThrowIfDisposed();
        if (entities.Length != components.Length)
        {
            throw new ArgumentException(
                $"Mismatched lengths: {entities.Length} entities, {components.Length} components");
        }
        if (entities.Length == 0) return;

        uint typeId = ResolveTypeId<T>();
        int size = ResolveTypeSize<T>();

        // Pack EntityId span to ulong span. Stack-allocate for small batches;
        // fall back to heap for large ones to avoid stack overflow risk.
        Span<ulong> packed = entities.Length <= 256
            ? stackalloc ulong[entities.Length]
            : new ulong[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            packed[i] = EntityIdPacking.Pack(entities[i]);
        }

        fixed (ulong* entitiesPtr = packed)
        fixed (T* componentsPtr = components)
        {
            NativeMethods.df_world_add_components_bulk(
                _handle, entitiesPtr, typeId, componentsPtr, size, entities.Length);
        }
    }

    /// <summary>
    /// Bulk get: reads components for an array of entities in a single
    /// P/Invoke crossing. Returns the count of successfully read components.
    /// Absent entities/components produce zero-filled output slots, so the
    /// output buffer is fully written regardless of return value.
    /// </summary>
    public unsafe int GetComponents<T>(ReadOnlySpan<EntityId> entities,
                                       Span<T> output) where T : unmanaged
    {
        ThrowIfDisposed();
        if (entities.Length != output.Length)
        {
            throw new ArgumentException(
                $"Mismatched lengths: {entities.Length} entities, {output.Length} output");
        }
        if (entities.Length == 0) return 0;

        uint typeId = ResolveTypeId<T>();
        int size = ResolveTypeSize<T>();

        Span<ulong> packed = entities.Length <= 256
            ? stackalloc ulong[entities.Length]
            : new ulong[entities.Length];
        for (int i = 0; i < entities.Length; i++)
        {
            packed[i] = EntityIdPacking.Pack(entities[i]);
        }

        fixed (ulong* entitiesPtr = packed)
        fixed (T* outputPtr = output)
        {
            return NativeMethods.df_world_get_components_bulk(
                _handle, entitiesPtr, typeId, outputPtr, size, entities.Length);
        }
    }

    /// <summary>
    /// Acquires a read-only span lease over native dense component storage
    /// for type <typeparamref name="T"/>.
    ///
    /// While ANY active <see cref="SpanLease{T}"/> exists, mutations
    /// (Add/Remove/Destroy/Flush) are silently rejected by the native side.
    /// The caller MUST <see cref="SpanLease{T}.Dispose"/> the lease before
    /// resuming mutations. Multiple concurrent leases are allowed.
    ///
    /// K1 SKELETON: provides <see cref="SpanLease{T}.Span"/> and
    /// <see cref="SpanLease{T}.Indices"/>. K5 will extend with paired
    /// iteration helpers and lease pooling.
    /// </summary>
    public unsafe SpanLease<T> AcquireSpan<T>() where T : unmanaged
    {
        ThrowIfDisposed();

        uint typeId = ResolveTypeId<T>();
        void* densePtr;
        int* indicesPtr;
        int count;

        int result = NativeMethods.df_world_acquire_span(
            _handle, typeId, &densePtr, &indicesPtr, &count);

        if (result == 0)
        {
            throw new InvalidOperationException(
                $"Failed to acquire span for component type {typeof(T).Name}");
        }

        return new SpanLease<T>(this, typeId, densePtr, indicesPtr, count);
    }

    public void Dispose()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMethods.df_world_destroy(_handle);
            _handle = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }

    ~NativeWorld()
    {
        if (_handle != IntPtr.Zero)
        {
            NativeMethods.df_world_destroy(_handle);
            _handle = IntPtr.Zero;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_handle == IntPtr.Zero)
        {
            throw new ObjectDisposedException(nameof(NativeWorld));
        }
    }

    /// <summary>
    /// Resolves the type id for <typeparamref name="T"/> using the registry if
    /// one is bound, else falling back to FNV-1a. Centralizes the
    /// registry-vs-fallback decision so component methods stay terse.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint ResolveTypeId<T>() where T : unmanaged
    {
        if (_registry != null)
        {
            // Auto-register on first use in registry mode. Idempotent — a
            // re-call returns the existing id.
            return _registry.Register<T>();
        }
#pragma warning disable CS0618 // NativeComponentType<T> is obsolete (legacy fallback path).
        uint typeId = NativeComponentType<T>.TypeId;
        NativeComponentTypeRegistry.Register(typeId, typeof(T));
        return typeId;
#pragma warning restore CS0618
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ResolveTypeSize<T>() where T : unmanaged
    {
        return Unsafe.SizeOf<T>();
    }
}
