using System;
using System.Collections.Generic;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Managed wrapper over <c>dualfrontier::KeyedMap</c>, presenting a
/// dictionary-like API with sorted-by-key iteration semantics.
///
/// Both <typeparamref name="TKey"/> and <typeparamref name="TValue"/>
/// must be <c>unmanaged</c> — values are copied across the C ABI
/// boundary as raw bytes. The native side uses <c>memcmp</c> for key
/// ordering; <see cref="IComparable{T}"/> on the managed side is
/// surfaced for managed-only use (e.g. to validate ordering in tests),
/// not for native lookup.
///
/// Lifetime: the underlying native map is owned by the
/// <see cref="NativeWorld"/>. <see cref="NativeMap{TKey,TValue}"/>
/// is a thin facade — copies share the same backing storage.
/// </summary>
public sealed unsafe class NativeMap<TKey, TValue>
    where TKey : unmanaged, IComparable<TKey>
    where TValue : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _mapId;
    private readonly IntPtr _handle;

    internal NativeMap(NativeWorld world, uint mapId, IntPtr handle)
    {
        _world = world;
        _mapId = mapId;
        _handle = handle;
    }

    /// <summary>Stable id assigned by the caller at <see cref="NativeWorld.GetKeyedMap"/> time.</summary>
    public uint MapId => _mapId;

    /// <summary>Number of entries currently stored.</summary>
    public int Count => NativeMethods.df_keyed_map_count(_handle);

    /// <summary>
    /// Inserts or updates the entry for <paramref name="key"/>. Returns
    /// <c>true</c> if a new entry was inserted, <c>false</c> if an
    /// existing entry's value was overwritten.
    /// </summary>
    public bool Set(TKey key, TValue value)
    {
        return NativeMethods.df_keyed_map_set(_handle, &key, &value) == 1;
    }

    /// <summary>
    /// Looks up <paramref name="key"/>. Returns <c>true</c> on hit
    /// (writing the stored value into <paramref name="value"/>),
    /// <c>false</c> on miss.
    /// </summary>
    public bool TryGet(TKey key, out TValue value)
    {
        TValue local = default;
        int rc = NativeMethods.df_keyed_map_get(_handle, &key, &local);
        value = local;
        return rc == 1;
    }

    /// <summary>
    /// Removes the entry for <paramref name="key"/>. Returns <c>true</c>
    /// if an entry was removed, <c>false</c> if the key was absent.
    /// </summary>
    public bool Remove(TKey key)
    {
        return NativeMethods.df_keyed_map_remove(_handle, &key) == 1;
    }

    /// <summary>Removes all entries; returns the previous count.</summary>
    public int Clear() => NativeMethods.df_keyed_map_clear(_handle);

    /// <summary>
    /// Snapshots all entries in sorted-by-key (memcmp) order. Allocates;
    /// hot paths should use <see cref="Iterate"/> with caller-supplied
    /// spans instead.
    /// </summary>
    public IReadOnlyList<KeyValuePair<TKey, TValue>> ToList()
    {
        int count = Count;
        if (count == 0)
        {
            return Array.Empty<KeyValuePair<TKey, TValue>>();
        }

        var keys = new TKey[count];
        var values = new TValue[count];
        Iterate(keys, values);

        var result = new KeyValuePair<TKey, TValue>[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }
        return result;
    }

    /// <summary>
    /// Writes up to <c>min(outKeys.Length, outValues.Length)</c> entries
    /// into the provided spans in sorted-by-key order. Returns the count
    /// actually written. Zero allocations.
    /// </summary>
    public int Iterate(Span<TKey> outKeys, Span<TValue> outValues)
    {
        int capacity = outKeys.Length < outValues.Length ? outKeys.Length : outValues.Length;
        if (capacity == 0) return 0;

        fixed (TKey* keysPtr = outKeys)
        fixed (TValue* valuesPtr = outValues)
        {
            return NativeMethods.df_keyed_map_iterate(_handle, keysPtr, valuesPtr, capacity);
        }
    }
}
