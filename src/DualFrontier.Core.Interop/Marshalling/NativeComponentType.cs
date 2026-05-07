using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// LEGACY: FNV-1a hash-based type identification.
///
/// Superseded by <see cref="ComponentTypeRegistry"/> (K2, 2026-05-07) which
/// provides explicit deterministic ids per K-L4 of KERNEL_ARCHITECTURE.md.
///
/// Retained for backward compatibility with pre-K2 code paths. A
/// <see cref="NativeWorld"/> constructed without an explicit registry uses
/// this fallback. New code should always provide a
/// <see cref="ComponentTypeRegistry"/> instance.
///
/// Will likely be removed at K8 cutover if Outcome 1 (native + batching wins
/// decisively) materializes — no production path will use FNV-1a after that.
/// </summary>
[Obsolete("Use ComponentTypeRegistry for explicit deterministic ids. " +
          "FNV-1a hash collision-prone — see K-L4 rationale.", error: false)]
internal static class NativeComponentType<T> where T : unmanaged
{
    internal static readonly uint TypeId = ComputeTypeId(typeof(T));

    internal static readonly int Size = Unsafe.SizeOf<T>();

    private static uint ComputeTypeId(Type type)
    {
        string name = type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
        // FNV-1a 32-bit — deterministic and allocation-free after the string.
        const uint offsetBasis = 2166136261u;
        const uint prime = 16777619u;
        uint hash = offsetBasis;
        foreach (char c in name)
        {
            hash ^= c;
            hash *= prime;
        }
        return hash;
    }
}

/// <summary>
/// LEGACY: diagnostic registry for FNV-1a ids.
/// Superseded by <see cref="ComponentTypeRegistry"/>. Retained for backward
/// compat with pre-K2 callers.
/// </summary>
[Obsolete("Use ComponentTypeRegistry. NativeComponentTypeRegistry will be " +
          "removed when NativeComponentType<T> is removed (K8 cutover).",
          error: false)]
internal static class NativeComponentTypeRegistry
{
    private static readonly ConcurrentDictionary<uint, Type> _byId = new();

    internal static void Register(uint id, Type type) => _byId[id] = type;

    internal static Type? Lookup(uint id) =>
        _byId.TryGetValue(id, out Type? t) ? t : null;
}
