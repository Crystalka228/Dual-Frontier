using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// Assigns a stable <c>uint</c> type id to each blittable component type on
/// the managed side so the native world can key its type-erased stores
/// without knowing anything about CLR metadata.
///
/// The id is derived from a hash of the type's assembly-qualified name. For
/// the PoC this is fine: collisions across the small number of benchmark
/// types are vanishingly unlikely and a collision would be caught by the
/// native Add contract (size mismatch on first insert into an existing
/// store). A production port should replace this with an explicit registry
/// populated at game startup to make the mapping auditable.
/// </summary>
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
/// Diagnostic registry so a benchmark or test can inspect which ids have
/// been minted. Not on the hot path — only used from debug tooling.
/// </summary>
internal static class NativeComponentTypeRegistry
{
    private static readonly ConcurrentDictionary<uint, Type> _byId = new();

    internal static void Register(uint id, Type type) => _byId[id] = type;

    internal static Type? Lookup(uint id) =>
        _byId.TryGetValue(id, out Type? t) ? t : null;
}
