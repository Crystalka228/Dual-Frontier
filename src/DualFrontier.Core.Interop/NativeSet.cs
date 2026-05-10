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
/// <para>
/// <b>Value-type wrapper (K8.2 v2).</b> Refactored from <c>sealed unsafe class</c>
/// to <c>readonly unsafe struct</c> so component structs can carry
/// <see cref="NativeSet{T}"/> fields without breaking K-L3 «unmanaged».
/// Lifetime: the native set is owned by the <see cref="NativeWorld"/>;
/// this wrapper is a thin facade. Construct via
/// <see cref="NativeWorld.CreateSet{T}"/> (allocates fresh id) or
/// <see cref="NativeWorld.GetSet{T}(uint)"/> (re-binds to explicit id).
/// </para>
/// </summary>
public readonly unsafe struct NativeSet<T> where T : unmanaged
{
    private readonly uint _setId;
    private readonly IntPtr _handle;

    internal NativeSet(uint setId, IntPtr handle)
    {
        _setId = setId;
        _handle = handle;
    }

    public uint SetId => _setId;

    /// <summary>
    /// True when the wrapper refers to a real native set. False for
    /// <c>default(NativeSet&lt;T&gt;)</c> — the invalid sentinel.
    /// </summary>
    public bool IsValid => _setId != 0 && _handle != IntPtr.Zero;

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
