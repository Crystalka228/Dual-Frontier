using System;
using System.Buffers;
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

    /// <summary>
    /// Adopts a handle previously obtained from <c>df_engine_bootstrap</c>.
    /// Used by <see cref="Bootstrap.Run"/> — not for public consumption.
    /// </summary>
    internal static NativeWorld AdoptBootstrappedHandle(IntPtr handle,
                                                       ComponentTypeRegistry? registry)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Cannot adopt null handle", nameof(handle));
        }
        return new NativeWorld(handle, registry);
    }

    private NativeWorld(IntPtr existingHandle, ComponentTypeRegistry? registry)
    {
        _handle = existingHandle;
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
        // for large ones, rent from ArrayPool to avoid GC pressure (the
        // `new ulong[]` fallback was the source of an 80 KB allocation per
        // tick observed in K3 PERFORMANCE_REPORT Measurement 2).
        ulong[]? rentedBuffer = null;
        try
        {
            scoped Span<ulong> packed;
            if (entities.Length <= 256)
            {
                packed = stackalloc ulong[entities.Length];
            }
            else
            {
                rentedBuffer = ArrayPool<ulong>.Shared.Rent(entities.Length);
                packed = rentedBuffer.AsSpan(0, entities.Length);
            }
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
        finally
        {
            if (rentedBuffer != null)
            {
                ArrayPool<ulong>.Shared.Return(rentedBuffer);
            }
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

        // Mirrors AddComponents — see that method for the ArrayPool rationale.
        ulong[]? rentedBuffer = null;
        try
        {
            scoped Span<ulong> packed;
            if (entities.Length <= 256)
            {
                packed = stackalloc ulong[entities.Length];
            }
            else
            {
                rentedBuffer = ArrayPool<ulong>.Shared.Rent(entities.Length);
                packed = rentedBuffer.AsSpan(0, entities.Length);
            }
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
        finally
        {
            if (rentedBuffer != null)
            {
                ArrayPool<ulong>.Shared.Return(rentedBuffer);
            }
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
    /// Provides <see cref="SpanLease{T}.Span"/>, <see cref="SpanLease{T}.Indices"/>,
    /// and <see cref="SpanLease{T}.Pairs"/> (added in K5). Lease pooling
    /// remains deferred — K7 measurements determine if it is needed.
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

    /// <summary>
    /// Open a write batch for component type <typeparamref name="T"/>.
    /// Recorded commands are applied atomically when the batch is
    /// <see cref="WriteBatch{T}.Flush"/>ed (or auto-flushed on Dispose).
    ///
    /// While any batch is active on this world, direct mutations
    /// (Add/Remove/Destroy/FlushDestroyed/AddComponents) are silently
    /// rejected by the native side. Multiple concurrent batches are
    /// allowed (different OR same component type).
    /// </summary>
    public WriteBatch<T> BeginBatch<T>() where T : unmanaged
    {
        ThrowIfDisposed();

        uint typeId = ResolveTypeId<T>();
        int size = ResolveTypeSize<T>();

        IntPtr batchHandle = NativeMethods.df_world_begin_batch(_handle, typeId, size);
        if (batchHandle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"Failed to begin batch for component type {typeof(T).Name}");
        }

        return new WriteBatch<T>(this, typeId, batchHandle);
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

    // ---- K8.1 reference primitives ----------------------------------------

    /// <summary>
    /// Interns <paramref name="content"/> in this world's string pool. The
    /// returned <see cref="InternedString"/> captures the issued id and
    /// the current generation tag; resolve back to content via
    /// <see cref="InternedString.Resolve"/> or <see cref="ResolveInternedString"/>.
    /// Empty input is mapped to the empty sentinel without allocating.
    /// </summary>
    public unsafe InternedString InternString(string content)
    {
        ThrowIfDisposed();
        if (content is null) throw new ArgumentNullException(nameof(content));
        if (content.Length == 0)
        {
            return new InternedString(0, 0);
        }

        int byteCount = System.Text.Encoding.UTF8.GetByteCount(content);
        Span<byte> buffer = byteCount <= 256
            ? stackalloc byte[byteCount]
            : new byte[byteCount];
        System.Text.Encoding.UTF8.GetBytes(content, buffer);

        uint id;
        fixed (byte* ptr = buffer)
        {
            id = NativeMethods.df_world_intern_string(_handle, ptr, byteCount);
        }
        if (id == 0)
        {
            return new InternedString(0, 0);
        }
        uint generation = NativeMethods.df_world_string_generation(_handle, id);
        return new InternedString(id, generation);
    }

    /// <summary>
    /// Resolves an <see cref="InternedString"/> back to its UTF-8 content.
    /// Returns <c>null</c> if the id is the empty sentinel, the generation
    /// tag is stale, or the id was issued by a different world.
    /// </summary>
    public unsafe string? ResolveInternedString(InternedString interned)
    {
        ThrowIfDisposed();
        if (interned.IsEmpty) return null;

        // Two-pass: first call sizes the buffer, second copies. Most strings
        // fit in a 256-byte stack buffer; growth path remains zero-cost on
        // the common case.
        const int InitialCapacity = 256;
        byte[]? rented = null;
        Span<byte> buffer = stackalloc byte[InitialCapacity];

        int written;
        while (true)
        {
            fixed (byte* ptr = buffer)
            {
                written = NativeMethods.df_world_resolve_string(
                    _handle, interned.Id, interned.Generation,
                    ptr, buffer.Length);
            }
            if (written == 0)
            {
                if (rented is not null) System.Buffers.ArrayPool<byte>.Shared.Return(rented);
                return null;
            }
            if (written < buffer.Length)
            {
                break;
            }
            // The native side fills exactly buffer.Length bytes when the
            // string is at least that long; we can't tell whether the
            // content was truncated. Grow and retry.
            if (rented is not null)
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(rented);
            }
            rented = System.Buffers.ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
            buffer = rented;
        }

        string result = System.Text.Encoding.UTF8.GetString(buffer.Slice(0, written));
        if (rented is not null) System.Buffers.ArrayPool<byte>.Shared.Return(rented);
        return result;
    }

    /// <summary>
    /// Returns a managed wrapper for the keyed map identified by
    /// <paramref name="mapId"/>. Allocates the map on first use; subsequent
    /// calls with the same id return a wrapper over the same backing
    /// storage.
    /// </summary>
    public NativeMap<TKey, TValue> GetKeyedMap<TKey, TValue>(uint mapId)
        where TKey : unmanaged, IComparable<TKey>
        where TValue : unmanaged
    {
        ThrowIfDisposed();
        if (mapId == 0) throw new ArgumentOutOfRangeException(nameof(mapId), "Map id 0 reserved as empty sentinel.");

        IntPtr handle = NativeMethods.df_world_get_keyed_map(
            _handle, mapId,
            Unsafe.SizeOf<TKey>(), Unsafe.SizeOf<TValue>());
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"df_world_get_keyed_map returned null for map_id={mapId}, " +
                $"key_size={Unsafe.SizeOf<TKey>()}, value_size={Unsafe.SizeOf<TValue>()}.");
        }
        return new NativeMap<TKey, TValue>(this, mapId, handle);
    }

    public NativeComposite<T> GetComposite<T>(uint compositeId) where T : unmanaged
    {
        ThrowIfDisposed();
        if (compositeId == 0) throw new ArgumentOutOfRangeException(nameof(compositeId), "Composite id 0 reserved as empty sentinel.");

        IntPtr handle = NativeMethods.df_world_get_composite(
            _handle, compositeId, Unsafe.SizeOf<T>());
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"df_world_get_composite returned null for composite_id={compositeId}, " +
                $"element_size={Unsafe.SizeOf<T>()}.");
        }
        return new NativeComposite<T>(this, compositeId, handle);
    }

    public NativeSet<T> GetSet<T>(uint setId) where T : unmanaged, IComparable<T>
    {
        ThrowIfDisposed();
        if (setId == 0) throw new ArgumentOutOfRangeException(nameof(setId), "Set id 0 reserved as empty sentinel.");

        IntPtr handle = NativeMethods.df_world_get_set(
            _handle, setId, Unsafe.SizeOf<T>());
        if (handle == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                $"df_world_get_set returned null for set_id={setId}, element_size={Unsafe.SizeOf<T>()}.");
        }
        return new NativeSet<T>(this, setId, handle);
    }

    public unsafe void BeginModScope(string modId)
    {
        ThrowIfDisposed();
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        WithUtf8(modId, ptr => NativeMethods.df_world_begin_mod_scope(_handle, ptr));
    }

    public unsafe void EndModScope(string modId)
    {
        ThrowIfDisposed();
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        WithUtf8(modId, ptr => NativeMethods.df_world_end_mod_scope(_handle, ptr));
    }

    public unsafe void ClearModScope(string modId)
    {
        ThrowIfDisposed();
        if (modId is null) throw new ArgumentNullException(nameof(modId));
        WithUtf8(modId, ptr => NativeMethods.df_world_clear_mod_scope(_handle, ptr));
    }

    /// <summary>Number of currently-allocated string ids in the pool.</summary>
    public int StringPoolCount => NativeMethods.df_world_string_pool_count(_handle);

    /// <summary>Current pool generation tag — bumps on each id reclaim.</summary>
    public uint StringPoolCurrentGeneration =>
        NativeMethods.df_world_string_pool_current_generation(_handle);

    private unsafe delegate void Utf8Action(byte* ptr);

    private unsafe void WithUtf8(string text, Utf8Action action)
    {
        // C ABI expects a null-terminated UTF-8 string. The encoded payload
        // is small (mod ids are short identifiers); stackalloc is the right
        // shape here.
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(text);
        Span<byte> buffer = byteCount + 1 <= 256
            ? stackalloc byte[byteCount + 1]
            : new byte[byteCount + 1];
        System.Text.Encoding.UTF8.GetBytes(text, buffer.Slice(0, byteCount));
        buffer[byteCount] = 0;
        fixed (byte* ptr = buffer)
        {
            action(ptr);
        }
    }
}
