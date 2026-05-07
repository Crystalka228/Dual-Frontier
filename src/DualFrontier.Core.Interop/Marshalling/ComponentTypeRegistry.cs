using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// Explicit per-NativeWorld registry mapping <see cref="Type"/> to sequential
/// <c>uint</c> type ids.
///
/// Replaces FNV-1a hash-based implicit identification (K0 inheritance) with an
/// auditable deterministic mapping. Key K-L4 invariants:
///   * Ids are sequential (1, 2, 3, ...). 0 is reserved for invalid.
///   * Registration is idempotent: re-registering the same type returns the
///     existing id without touching the native side again.
///   * Mod load order matters for id stability across runs. ModLoader must
///     process mods deterministically (alphabetical OR explicit ordering
///     manifest) — concern of K6, not K2.
///
/// Instance-per-NativeWorld (not static) — different worlds have independent
/// type-id spaces. Different game sessions have independent registries.
/// </summary>
public sealed class ComponentTypeRegistry
{
    private readonly Dictionary<Type, uint> _typeToId = new();
    private readonly Dictionary<uint, Type> _idToType = new();
    private readonly IntPtr _worldHandle;
    private uint _nextId = 1;  // 0 reserved for invalid.

    /// <summary>
    /// Creates a registry bound to the specified native world. The handle is
    /// captured for the registry's lifetime — caller must ensure the world is
    /// not disposed while the registry is in use.
    /// </summary>
    internal ComponentTypeRegistry(IntPtr worldHandle)
    {
        if (worldHandle == IntPtr.Zero)
        {
            throw new ArgumentException(
                "Cannot bind ComponentTypeRegistry to a null world handle.",
                nameof(worldHandle));
        }
        _worldHandle = worldHandle;
    }

    /// <summary>
    /// Registers component type <typeparamref name="T"/>. Idempotent —
    /// re-registering returns the existing id without re-calling native
    /// registration.
    /// </summary>
    /// <typeparam name="T">Unmanaged component type.</typeparam>
    /// <returns>The deterministic id assigned to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">If native registration fails.</exception>
    public uint Register<T>() where T : unmanaged
    {
        Type type = typeof(T);
        if (_typeToId.TryGetValue(type, out uint existing))
        {
            return existing;
        }

        uint id = _nextId++;
        int size = Unsafe.SizeOf<T>();
        int result = NativeMethods.df_world_register_component_type(
            _worldHandle, id, size);
        if (result == 0)
        {
            // Native registration failed. Roll back the id assignment so the
            // next attempt does not skip a number.
            _nextId--;
            throw new InvalidOperationException(
                $"Native registration failed for component type {type.Name} " +
                $"(id={id}, size={size}).");
        }

        _typeToId[type] = id;
        _idToType[id] = type;
        return id;
    }

    /// <summary>
    /// Gets the id for a previously-registered type <typeparamref name="T"/>.
    /// Throws if <typeparamref name="T"/> is not registered.
    /// </summary>
    /// <exception cref="InvalidOperationException">If <typeparamref name="T"/> was never registered.</exception>
    public uint GetId<T>() where T : unmanaged
    {
        if (!_typeToId.TryGetValue(typeof(T), out uint id))
        {
            throw new InvalidOperationException(
                $"Component type {typeof(T).Name} not registered. " +
                $"Call Register<{typeof(T).Name}>() first.");
        }
        return id;
    }

    /// <summary>
    /// Tries to get the id for type <typeparamref name="T"/> without throwing.
    /// </summary>
    public bool TryGetId<T>(out uint id) where T : unmanaged
    {
        return _typeToId.TryGetValue(typeof(T), out id);
    }

    /// <summary>
    /// Reverse lookup: get the <see cref="Type"/> registered against the given
    /// id. Returns null if the id has not been assigned.
    /// </summary>
    public Type? Lookup(uint id)
    {
        return _idToType.TryGetValue(id, out Type? type) ? type : null;
    }

    /// <summary>Number of types registered.</summary>
    public int Count => _typeToId.Count;

    /// <summary>Returns true if <typeparamref name="T"/> has been registered.</summary>
    public bool IsRegistered<T>() where T : unmanaged
    {
        return _typeToId.ContainsKey(typeof(T));
    }
}
