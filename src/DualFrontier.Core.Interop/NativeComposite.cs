using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed wrapper over <c>dualfrontier::Composite</c>. Each parent
/// <see cref="EntityId"/> owns its own variable-length list of
/// <typeparamref name="T"/> elements; element size is fixed at
/// construction time (<c>sizeof(T)</c>).
///
/// Insertion order is preserved for non-removed elements. After a
/// <see cref="RemoveAt(EntityId,int)"/>, the element at that index
/// is replaced with whatever was at the end (swap-with-last); ordering
/// across removes is NOT preserved. This semantics matches the K8.1
/// brief LOCKED design (§1.4): movement waypoints and storage item
/// lists are the targets, neither needs stable index order.
///
/// Lifetime: the native composite is owned by the
/// <see cref="NativeWorld"/>; this wrapper is a thin facade.
/// </summary>
public sealed unsafe class NativeComposite<T> where T : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _compositeId;
    private readonly IntPtr _handle;

    internal NativeComposite(NativeWorld world, uint compositeId, IntPtr handle)
    {
        _world = world;
        _compositeId = compositeId;
        _handle = handle;
    }

    public uint CompositeId => _compositeId;

    public int CountFor(EntityId parent)
    {
        ulong packed = EntityIdPacking.Pack(parent);
        return NativeMethods.df_composite_get_count(_handle, packed);
    }

    public bool Add(EntityId parent, T element)
    {
        ulong packed = EntityIdPacking.Pack(parent);
        return NativeMethods.df_composite_add(_handle, packed, &element) == 1;
    }

    public bool TryGetAt(EntityId parent, int index, out T element)
    {
        ulong packed = EntityIdPacking.Pack(parent);
        T local = default;
        int rc = NativeMethods.df_composite_get_at(_handle, packed, index, &local);
        element = local;
        return rc == 1;
    }

    /// <summary>
    /// Removes the element at <paramref name="index"/> via swap-with-last.
    /// Order is NOT preserved across this operation.
    /// </summary>
    public bool RemoveAt(EntityId parent, int index)
    {
        ulong packed = EntityIdPacking.Pack(parent);
        return NativeMethods.df_composite_remove_at(_handle, packed, index) == 1;
    }

    public bool ClearFor(EntityId parent)
    {
        ulong packed = EntityIdPacking.Pack(parent);
        return NativeMethods.df_composite_clear_for(_handle, packed) == 1;
    }

    /// <summary>
    /// Writes up to <c>outBuffer.Length</c> elements for
    /// <paramref name="parent"/> into the provided span. Returns the
    /// count actually written. Insertion order (modulo any earlier
    /// swap-with-last removals).
    /// </summary>
    public int Iterate(EntityId parent, Span<T> outBuffer)
    {
        if (outBuffer.IsEmpty) return 0;
        ulong packed = EntityIdPacking.Pack(parent);
        fixed (T* ptr = outBuffer)
        {
            return NativeMethods.df_composite_iterate(_handle, packed, ptr, outBuffer.Length);
        }
    }
}
