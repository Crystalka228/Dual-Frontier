using System;
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

    /// <summary>
    /// Internal handle access for <see cref="SpanLease{T}"/> lifetime
    /// management. Not for public consumption — use
    /// <see cref="AcquireSpan{T}"/> instead.
    /// </summary>
    internal IntPtr HandleForInternalUse => _handle;

    public NativeWorld()
    {
        _handle = NativeMethods.df_world_create();
        if (_handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "df_world_create returned null — native library failed to allocate a World.");
        }
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
        uint typeId = NativeComponentType<T>.TypeId;
        NativeComponentTypeRegistry.Register(typeId, typeof(T));
        NativeMethods.df_world_add_component(
            _handle,
            EntityIdPacking.Pack(id),
            typeId,
            &value,
            NativeComponentType<T>.Size);
    }

    public unsafe bool TryGetComponent<T>(EntityId id, out T value) where T : unmanaged
    {
        ThrowIfDisposed();
        T tmp = default;
        int ok = NativeMethods.df_world_get_component(
            _handle,
            EntityIdPacking.Pack(id),
            NativeComponentType<T>.TypeId,
            &tmp,
            NativeComponentType<T>.Size);
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
            NativeComponentType<T>.TypeId) != 0;
    }

    public void RemoveComponent<T>(EntityId id) where T : unmanaged
    {
        ThrowIfDisposed();
        NativeMethods.df_world_remove_component(
            _handle,
            EntityIdPacking.Pack(id),
            NativeComponentType<T>.TypeId);
    }

    public int GetComponentCount<T>() where T : unmanaged
    {
        ThrowIfDisposed();
        return NativeMethods.df_world_component_count(
            _handle,
            NativeComponentType<T>.TypeId);
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

        uint typeId = NativeComponentType<T>.TypeId;
        NativeComponentTypeRegistry.Register(typeId, typeof(T));
        int size = NativeComponentType<T>.Size;

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

        uint typeId = NativeComponentType<T>.TypeId;
        int size = NativeComponentType<T>.Size;

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

        uint typeId = NativeComponentType<T>.TypeId;
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
}
