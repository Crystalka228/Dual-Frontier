using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// The authoritative component-type identity: an owner plus the type's stable
/// CLR name. Identity is owner-scoped by design — two mods that both define a
/// <c>Weather.StateComponent</c> are two DISTINCT component types with distinct
/// ids, exactly as two same-named programs are distinct processes under an
/// operating system. The type NAME alone is not an identity; the owner is the
/// directory it lives in.
///
/// <para>
/// <paramref name="Owner"/> follows the ledger convention shared with
/// <c>KernelCapabilityRegistry</c> (MOD_OS_ARCHITECTURE §3.3 (owner strings)):
/// <c>kernel</c> for engine surface, <c>mod.&lt;modId&gt;</c> for mods.
/// </para>
/// </summary>
/// <param name="Owner">Owning namespace — <c>kernel</c> or <c>mod.&lt;modId&gt;</c>.</param>
/// <param name="TypeFullName">The CLR <see cref="Type.FullName"/> of the component type.</param>
public readonly record struct ComponentIdentity(string Owner, string TypeFullName)
{
    /// <summary>Diagnostic form: <c>owner/Namespace.TypeName</c>.</summary>
    public override string ToString() => $"{Owner}/{TypeFullName}";
}

/// <summary>
/// Explicit per-NativeWorld registry mapping <see cref="ComponentIdentity"/> to
/// sequential <c>uint</c> type ids.
///
/// Replaces FNV-1a hash-based implicit identification (K0 inheritance) with an
/// auditable deterministic mapping. Key K-L4 invariants:
///   * Ids are sequential (1, 2, 3, ...). 0 is reserved for invalid.
///   * Registration is idempotent: re-registering the same identity returns the
///     existing id without allocating a new one.
///   * Mod load order matters for id stability across runs. ModLoader must
///     process mods deterministically (alphabetical OR explicit ordering
///     manifest) — concern of K6, not K2.
///
/// <para>
/// <b>Identity is the owner-scoped NAME, never the <see cref="Type"/> object</b>
/// (F-60 closure, ID-A). The authoritative state below holds no <see cref="Type"/>
/// reference at all; <see cref="Type"/>-keyed resolution is a
/// <see cref="ConditionalWeakTable{TKey,TValue}"/> cache whose entries are held
/// weakly, so a mod's entry dies with its collectible AssemblyLoadContext. That
/// is what lets a component-defining mod's ALC be reclaimed at unload. The
/// identity ROW deliberately survives unload: when the mod is loaded again its
/// new <see cref="Type"/> object re-adopts the same identity, therefore the same
/// id, therefore the native store its previous incarnation was writing into —
/// which is how a reloaded mod resumes its state instead of re-seeding it.
/// </para>
///
/// Instance-per-NativeWorld (not static) — different worlds have independent
/// type-id spaces. Different game sessions have independent registries.
/// </summary>
public sealed class ComponentTypeRegistry
{
    /// <summary>
    /// Owner string for engine-owned component surface, per the ledger
    /// convention (MOD_OS_ARCHITECTURE §3.3 (owner strings)).
    /// </summary>
    public const string KernelOwner = "kernel";

    /// <summary>
    /// A live <see cref="Type"/>'s resolved id together with the identity it was
    /// registered under. Carrying both lets the cache-hit path verify the owner
    /// without touching the locked authoritative maps.
    /// </summary>
    private sealed class Binding
    {
        internal Binding(uint id, ComponentIdentity identity)
        {
            Id = id;
            Identity = identity;
        }

        internal uint Id { get; }
        internal ComponentIdentity Identity { get; }
    }

    // Authoritative state — no Type references, so nothing here roots an ALC.
    private readonly Dictionary<ComponentIdentity, uint> _idByIdentity = new();
    private readonly Dictionary<uint, ComponentIdentity> _identityById = new();

    // Resolution cache. ConditionalWeakTable holds its KEYS weakly: a mod ALC's
    // Type entry is collected with the ALC. This is the F-60 leak fix.
    private readonly ConditionalWeakTable<Type, Binding> _bindings = new();

    // Guards the authoritative maps and _nextId. Registration is cold — bootstrap
    // and mod Apply — while the hot path (a cache hit) never takes it, so a plain
    // lock is the whole answer; dispatch runs system bodies through Parallel.ForEach
    // (THREADING §2 (dispatch parallelism)) and first-use resolution is reachable
    // from inside a system body.
    private readonly object _gate = new();

    private readonly IntPtr _worldHandle;
    private uint _nextId = 1;  // 0 reserved for invalid.

    // The single generic registration entry point, reached reflectively by the
    // runtime-Type overload so that Unsafe.SizeOf<T> stays the ONE size
    // computation in this class.
    private static readonly MethodInfo GenericRegisterWithOwner =
        typeof(ComponentTypeRegistry).GetMethod(
            nameof(Register),
            genericParameterCount: 1,
            types: new[] { typeof(string) })
        ?? throw new InvalidOperationException(
            "ComponentTypeRegistry.Register<T>(string) not found by reflection.");

    // The runtime's own recursive unmanagedness answer. The C# `unmanaged`
    // constraint is enforced by the COMPILER; the CLR's constraint check behind
    // MakeGenericMethod does not reproduce it, so a reflective caller can reach
    // Register<T> with a struct that holds managed references. Measured, not
    // assumed: `struct Bad { public string Value; }` was accepted and given a
    // native raw store before this probe was added.
    private static readonly MethodInfo ContainsReferencesProbe =
        typeof(RuntimeHelpers).GetMethod(nameof(RuntimeHelpers.IsReferenceOrContainsReferences))
        ?? throw new InvalidOperationException(
            "RuntimeHelpers.IsReferenceOrContainsReferences<T>() not found by reflection.");

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
    /// Registers component type <typeparamref name="T"/> as engine surface
    /// (owner <see cref="KernelOwner"/>). Idempotent — re-registering returns
    /// the existing id.
    /// </summary>
    /// <typeparam name="T">Unmanaged component type.</typeparam>
    /// <returns>The deterministic id assigned to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">If native registration fails.</exception>
    public uint Register<T>() where T : unmanaged => Register<T>(KernelOwner);

    /// <summary>
    /// Registers component type <typeparamref name="T"/> under
    /// <paramref name="owner"/>. Idempotent per identity: a second call with the
    /// same owner returns the existing id.
    ///
    /// <para>
    /// When a DIFFERENT <see cref="Type"/> object claims an identity that already
    /// has an id — the reload case, where a fresh collectible ALC minted a new
    /// <see cref="Type"/> for the same component — the existing id is re-adopted
    /// and re-asserted against native. Native registration is idempotent for the
    /// same size and REJECTS the same id at a different size, which is precisely
    /// how a component whose layout changed between mod versions is caught rather
    /// than silently reinterpreted over the surviving store.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Unmanaged component type.</typeparam>
    /// <param name="owner"><c>kernel</c> or <c>mod.&lt;modId&gt;</c>.</param>
    /// <exception cref="InvalidOperationException">
    /// If native registration fails, or if <typeparamref name="T"/> is already
    /// bound to a different owner.
    /// </exception>
    public uint Register<T>(string owner) where T : unmanaged
    {
        if (owner is null) throw new ArgumentNullException(nameof(owner));

        Type type = typeof(T);
        if (_bindings.TryGetValue(type, out Binding? cached))
        {
            RequireSameOwner(type, cached, owner);
            return cached.Id;
        }

        return RegisterCore(type, owner, Unsafe.SizeOf<T>());
    }

    /// <summary>
    /// Runtime-<see cref="Type"/> form of <see cref="Register{T}(string)"/>, for
    /// callers holding a <see cref="Type"/> rather than a generic parameter — the
    /// mod pipeline, which learns a mod's claimed component types only at load
    /// time. Dispatches into the generic path reflectively so the component size
    /// is computed by the same <c>Unsafe.SizeOf&lt;T&gt;</c> expression as every
    /// other registration.
    /// </summary>
    /// <param name="componentType">A concrete unmanaged component type.</param>
    /// <param name="owner"><c>kernel</c> or <c>mod.&lt;modId&gt;</c>.</param>
    public uint Register(Type componentType, string owner)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));
        if (owner is null) throw new ArgumentNullException(nameof(owner));
        if (!componentType.IsValueType)
        {
            throw new ArgumentException(
                $"Component type {componentType.FullName} is not a value type. Only Path α " +
                "(unmanaged struct) components consume a native type id; Path β managed " +
                "components are held in per-mod managed stores.",
                nameof(componentType));
        }

        // IsValueType alone does NOT establish the unmanaged invariant this overload
        // promises: a struct holding a managed reference is a value type, and native
        // raw storage would copy its bytes as if they were plain data. The generic
        // path gets this from the compiler; the reflective path has to ask the runtime.
        if ((bool)ContainsReferencesProbe.MakeGenericMethod(componentType).Invoke(null, null)!)
        {
            throw new ArgumentException(
                $"Component type {componentType.FullName} is a value type but contains managed " +
                "references, so it has no native memory layout and cannot back a raw component " +
                "store. Path α components must be unmanaged all the way down.",
                nameof(componentType));
        }

        try
        {
            return (uint)GenericRegisterWithOwner
                .MakeGenericMethod(componentType)
                .Invoke(this, new object[] { owner })!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            // Surface the real failure — a size rejection from native, or an owner
            // conflict — rather than the reflection wrapper the caller cannot act on.
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;  // unreachable; satisfies definite-assignment analysis.
        }
    }

    /// <summary>
    /// Gets the id for a previously-registered type <typeparamref name="T"/>.
    /// Throws if <typeparamref name="T"/> is not registered.
    /// </summary>
    /// <exception cref="InvalidOperationException">If <typeparamref name="T"/> was never registered.</exception>
    public uint GetId<T>() where T : unmanaged
    {
        if (TryGetId<T>(out uint id))
        {
            return id;
        }

        throw new InvalidOperationException(
            $"Component type {typeof(T).Name} not registered. " +
            $"Call Register<{typeof(T).Name}>() first.");
    }

    /// <summary>
    /// Tries to get the id for type <typeparamref name="T"/> without throwing.
    /// Resolves through the cache; a cache miss falls through to the
    /// authoritative map under the <see cref="KernelOwner"/> identity, which is
    /// where engine surface lives. A mod-owned type that is absent from the cache
    /// resolves false — its identity is namespaced under its mod, not the kernel.
    /// </summary>
    public bool TryGetId<T>(out uint id) where T : unmanaged
    {
        Type type = typeof(T);
        if (_bindings.TryGetValue(type, out Binding? cached))
        {
            id = cached.Id;
            return true;
        }

        string? fullName = type.FullName;
        if (fullName is null)
        {
            id = 0;
            return false;
        }

        lock (_gate)
        {
            return _idByIdentity.TryGetValue(new ComponentIdentity(KernelOwner, fullName), out id);
        }
    }

    /// <summary>
    /// Reverse lookup: get the <see cref="ComponentIdentity"/> registered against
    /// the given id. Returns null if the id has not been assigned. The reverse map
    /// answers with the identity rather than a <see cref="Type"/> so that no
    /// registry structure holds a reference into a mod's collectible ALC.
    /// </summary>
    public ComponentIdentity? Lookup(uint id)
    {
        lock (_gate)
        {
            return _identityById.TryGetValue(id, out ComponentIdentity identity) ? identity : null;
        }
    }

    /// <summary>Number of identities registered.</summary>
    public int Count
    {
        get
        {
            lock (_gate)
            {
                return _idByIdentity.Count;
            }
        }
    }

    /// <summary>Returns true if <typeparamref name="T"/> has been registered.</summary>
    public bool IsRegistered<T>() where T : unmanaged => TryGetId<T>(out _);

    /// <summary>
    /// Returns true if <paramref name="type"/> currently resolves through the
    /// cache, i.e. this exact <see cref="Type"/> object is bound to an id.
    /// The mod pipeline uses it to report what a mod's registration actually
    /// produced; it never triggers registration.
    /// </summary>
    public bool TryGetCachedId(Type type, out uint id)
    {
        if (type is not null && _bindings.TryGetValue(type, out Binding? cached))
        {
            id = cached.Id;
            return true;
        }

        id = 0;
        return false;
    }

    private uint RegisterCore(Type type, string owner, int size)
    {
        string fullName = type.FullName
            ?? throw new InvalidOperationException(
                $"Component type {type} has no FullName and cannot carry a stable identity.");

        var identity = new ComponentIdentity(owner, fullName);

        lock (_gate)
        {
            // Re-probe: another thread may have registered this Type while we waited.
            if (_bindings.TryGetValue(type, out Binding? raced))
            {
                RequireSameOwner(type, raced, owner);
                return raced.Id;
            }

            if (_idByIdentity.TryGetValue(identity, out uint existing))
            {
                // Re-adoption. The identity row outlived the Type object that first
                // claimed it — a reloaded mod. Re-assert against native: idempotent
                // at the same size, rejected at a different one.
                if (NativeMethods.df_world_register_component_type(_worldHandle, existing, size) == 0)
                {
                    throw new InvalidOperationException(
                        $"Component identity {identity} is already registered as id {existing} " +
                        $"with a different layout; native registration rejected size {size}. " +
                        "A component type's layout must not change while its store is live.");
                }

                _bindings.AddOrUpdate(type, new Binding(existing, identity));
                return existing;
            }

            uint id = _nextId++;
            if (NativeMethods.df_world_register_component_type(_worldHandle, id, size) == 0)
            {
                // Native registration failed. Roll back the id assignment so the
                // next attempt does not skip a number.
                _nextId--;
                throw new InvalidOperationException(
                    $"Native registration failed for component type {type.Name} " +
                    $"(id={id}, size={size}, identity={identity}).");
            }

            _idByIdentity[identity] = id;
            _identityById[id] = identity;
            _bindings.AddOrUpdate(type, new Binding(id, identity));
            return id;
        }
    }

    private static void RequireSameOwner(Type type, Binding bound, string owner)
    {
        if (!string.Equals(bound.Identity.Owner, owner, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Component type {type.FullName} is already registered under owner " +
                $"'{bound.Identity.Owner}'; '{owner}' cannot claim it. One CLR type belongs " +
                "to exactly one owner — a second owner means a second type.");
        }
    }
}
