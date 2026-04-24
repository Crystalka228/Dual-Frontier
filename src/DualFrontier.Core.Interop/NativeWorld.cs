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
