using System;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed wrapper over <c>dualfrontier::SetPrimitive</c>. Value-less
/// mirror of <see cref="NativeMap{TKey,TValue}"/>; iteration returns
/// elements in sorted (memcmp) order.
///
/// Use case (per K8.2 plan): replaces <c>HashSet&lt;EntityId&gt;</c>
/// reservation tracking on storage components with deterministic
/// iteration.
///
/// Lifetime: the native set is owned by the <see cref="NativeWorld"/>;
/// this wrapper is a thin facade.
/// </summary>
public sealed unsafe class NativeSet<T> where T : unmanaged, IComparable<T>
{
    private readonly NativeWorld _world;
    private readonly uint _setId;
    private readonly IntPtr _handle;

    internal NativeSet(NativeWorld world, uint setId, IntPtr handle)
    {
        _world = world;
        _setId = setId;
        _handle = handle;
    }

    public uint SetId => _setId;

    public int Count => NativeMethods.df_set_count(_handle);

    /// <summary>
    /// Adds <paramref name="element"/>. Returns <c>true</c> if newly
    /// inserted, <c>false</c> if already present.
    /// </summary>
    public bool Add(T element) =>
        NativeMethods.df_set_add(_handle, &element) == 1;

    public bool Contains(T element) =>
        NativeMethods.df_set_contains(_handle, &element) == 1;

    public bool Remove(T element) =>
        NativeMethods.df_set_remove(_handle, &element) == 1;

    /// <summary>
    /// Writes up to <c>outBuffer.Length</c> elements into the provided
    /// span in sorted (memcmp) order. Returns the count actually written.
    /// </summary>
    public int Iterate(Span<T> outBuffer)
    {
        if (outBuffer.IsEmpty) return 0;
        fixed (T* ptr = outBuffer)
        {
            return NativeMethods.df_set_iterate(_handle, ptr, outBuffer.Length);
        }
    }
}
