using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

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

    /// <summary>
    /// What an identity owns: its id, and a fingerprint of the memory layout the
    /// type had when the id was allocated. The fingerprint is the re-adoption
    /// gate — see <see cref="ComputeLayoutFingerprint"/>.
    /// </summary>
    private readonly struct Registration
    {
        internal Registration(uint id, string layout)
        {
            Id = id;
            Layout = layout;
        }

        internal uint Id { get; }
        internal string Layout { get; }
    }

    // Authoritative state — no Type references, so nothing here roots an ALC.
    private readonly Dictionary<ComponentIdentity, Registration> _byIdentity = new();
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
            if (_byIdentity.TryGetValue(new ComponentIdentity(KernelOwner, fullName), out Registration reg))
            {
                id = reg.Id;
                return true;
            }

            id = 0;
            return false;
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
                return _byIdentity.Count;
            }
        }
    }

    /// <summary>Returns true if <typeparamref name="T"/> has been registered.</summary>
    public bool IsRegistered<T>() where T : unmanaged => TryGetId<T>(out _);

    /// <summary>
    /// Returns true if an identity is already registered for
    /// <paramref name="owner"/> + <paramref name="typeFullName"/>. The mod pipeline
    /// asks BEFORE registering so it knows which rows a failed load must unwind and
    /// which predate it — a re-adopted row belongs to an earlier, successful load and
    /// must survive.
    /// </summary>
    public bool IsIdentityRegistered(string owner, string typeFullName)
    {
        if (owner is null) throw new ArgumentNullException(nameof(owner));
        if (typeFullName is null) throw new ArgumentNullException(nameof(typeFullName));

        lock (_gate)
        {
            return _byIdentity.ContainsKey(new ComponentIdentity(owner, typeFullName));
        }
    }

    /// <summary>
    /// Removes the identity row created for <paramref name="componentType"/> under
    /// <paramref name="owner"/>, together with its cache binding. Used by the mod
    /// pipeline to unwind component ids allocated during an <c>Apply</c> that then
    /// failed, so a batch that never committed leaves no trace in the identity space.
    /// Rows the caller did not create must not be passed here.
    ///
    /// <para>
    /// <b><c>_nextId</c> is deliberately NOT rewound.</b> There is no native
    /// store-removal export, so the store allocated under the withdrawn id outlives it.
    /// Reissuing that id to a DIFFERENT component would hand the newcomer the failed
    /// mod's bytes — at an equal size native registration is idempotent and would accept
    /// it silently, which is a corruption, not a leak. Letting the counter advance costs
    /// only id density within a run, and run-local ids are never persisted
    /// (IDENTITY_AND_ABI_CONTRACT §1 note 2). The store itself remains as inert residue,
    /// which is the F-58 reclamation gap, not this method's to close.
    /// </para>
    /// </summary>
    public void RollbackRegistration(Type componentType, string owner)
    {
        if (componentType is null) throw new ArgumentNullException(nameof(componentType));
        if (owner is null) throw new ArgumentNullException(nameof(owner));

        string? fullName = componentType.FullName;
        if (fullName is null) return;

        lock (_gate)
        {
            var identity = new ComponentIdentity(owner, fullName);
            if (!_byIdentity.TryGetValue(identity, out Registration registration))
            {
                return;
            }

            _byIdentity.Remove(identity);
            _identityById.Remove(registration.Id);
            _bindings.Remove(componentType);
        }
    }

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
        string layout = ComputeLayoutFingerprint(type, size);

        lock (_gate)
        {
            // Re-probe: another thread may have registered this Type while we waited.
            if (_bindings.TryGetValue(type, out Binding? raced))
            {
                RequireSameOwner(type, raced, owner);
                return raced.Id;
            }

            if (_byIdentity.TryGetValue(identity, out Registration existing))
            {
                // Re-adoption. The identity row outlived the Type object that first
                // claimed it — a reloaded mod — so the surviving native store is about
                // to be reinterpreted through the incoming type. Equal byte size is NOT
                // proof that this is safe: swapping two int fields, or replacing an int
                // with a float, leaves the size untouched and would silently reinterpret
                // every stored value. The layout fingerprint is what actually gates it.
                if (!string.Equals(existing.Layout, layout, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Component identity {identity} is already registered as id {existing.Id} " +
                        "with a different layout, so its surviving store cannot be re-adopted. " +
                        $"Registered layout: {existing.Layout}; incoming layout: {layout}. " +
                        "A component type's memory layout must not change while its store is live.");
                }

                // Re-assert against native as well. The fingerprint already implies the
                // size matches, so this is the ABI-level second opinion, not the primary
                // check — and it is what re-attaches this id on the native side.
                if (NativeMethods.df_world_register_component_type(_worldHandle, existing.Id, size) == 0)
                {
                    throw new InvalidOperationException(
                        $"Native registration rejected re-adoption of component identity {identity} " +
                        $"as id {existing.Id} at size {size}.");
                }

                _bindings.AddOrUpdate(type, new Binding(existing.Id, identity));
                return existing.Id;
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

            _byIdentity[identity] = new Registration(id, layout);
            _identityById[id] = identity;
            _bindings.AddOrUpdate(type, new Binding(id, identity));
            return id;
        }
    }

    /// <summary>
    /// A structural fingerprint of <paramref name="type"/>'s memory layout: its total
    /// size, then its instance field TYPES in declaration order, recursing through
    /// nested structs. Two different <see cref="Type"/> objects — a mod's component
    /// before and after a reload — produce equal fingerprints exactly when their bytes
    /// mean the same thing.
    ///
    /// <para>
    /// Field NAMES are deliberately excluded. This gates re-adoption of a surviving
    /// native store, and a rename does not move a byte; refusing it would fail loads
    /// that are perfectly safe. What it does catch is what actually corrupts data:
    /// reordering (<c>int,float</c> becoming <c>float,int</c>), retyping
    /// (<c>int</c> becoming <c>float</c>), and any size change. Enum fields reduce to
    /// their underlying type for the same reason — the storage is what matters.
    /// </para>
    ///
    /// <para>
    /// Comparison is by string rather than by hash, so there is no collision to reason
    /// about: a false match here would silently reinterpret live data, which is exactly
    /// the class of failure this exists to prevent, and the registry holds few enough
    /// rows that the strings are free. The fingerprint does not model
    /// <c>StructLayout</c>/<c>FieldOffset</c> attributes; explicit-layout components
    /// are outside what it claims to verify.
    /// </para>
    /// </summary>
    private static string ComputeLayoutFingerprint(Type type, int size)
    {
        var builder = new StringBuilder();
        builder.Append(size).Append(':');
        AppendLayout(builder, type, depth: 0);
        return builder.ToString();
    }

    private const int MaxLayoutDepth = 16;

    private static void AppendLayout(StringBuilder builder, Type type, int depth)
    {
        if (depth > MaxLayoutDepth)
        {
            throw new InvalidOperationException(
                $"Component type {type.FullName} nests structs deeper than {MaxLayoutDepth} " +
                "levels; its layout cannot be fingerprinted.");
        }

        builder.Append('(');
        foreach (FieldInfo field in type.GetFields(
                     BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            Type fieldType = field.FieldType;
            if (fieldType.IsEnum)
            {
                fieldType = Enum.GetUnderlyingType(fieldType);
            }

            builder.Append(fieldType.FullName ?? fieldType.Name);

            // Recurse into nested structs; primitives and pointers are leaves.
            if (fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsPointer && !fieldType.IsEnum)
            {
                AppendLayout(builder, fieldType, depth + 1);
            }

            builder.Append(',');
        }
        builder.Append(')');
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
