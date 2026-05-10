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
/// <para>
/// <b>Value-type wrapper (K8.2 v2).</b> Refactored from <c>sealed unsafe class</c>
/// to <c>readonly unsafe struct</c> so component structs can carry
/// <see cref="NativeComposite{T}"/> fields without breaking K-L3 «unmanaged».
/// Lifetime: the native composite is owned by the <see cref="NativeWorld"/>;
/// this wrapper is a thin facade. Construct via
/// <see cref="NativeWorld.CreateComposite{T}"/> (allocates fresh id; one
/// composite per component instance) or <see cref="NativeWorld.GetComposite{T}(uint)"/>
/// (re-binds to explicit id, e.g., for cross-entity shared lists).
/// </para>
/// </summary>
public readonly unsafe struct NativeComposite<T> where T : unmanaged
{
    private readonly uint _compositeId;
    private readonly IntPtr _handle;

    internal NativeComposite(uint compositeId, IntPtr handle)
    {
        _compositeId = compositeId;
        _handle = handle;
    }

    public uint CompositeId => _compositeId;

    /// <summary>
    /// True when the wrapper refers to a real native composite. False for
    /// <c>default(NativeComposite&lt;T&gt;)</c> — the invalid sentinel.
    /// </summary>
    public bool IsValid => _compositeId != 0 && _handle != IntPtr.Zero;

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
