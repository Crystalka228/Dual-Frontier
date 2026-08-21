using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class ComponentTypeRegistryTests
{
    // Placeholder types exercise registration semantics by identity only —
    // the fields are not read in any test.
#pragma warning disable CS0649
    private struct TypeA { public int Value; }
    private struct TypeB { public long Value; }
    private struct TypeC { public byte Value; }
#pragma warning restore CS0649

    [Fact]
    public void Register_returns_sequential_ids_starting_from_1()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint idA = registry.Register<TypeA>();
        uint idB = registry.Register<TypeB>();
        uint idC = registry.Register<TypeC>();

        idA.Should().Be(1);
        idB.Should().Be(2);
        idC.Should().Be(3);
    }

    [Fact]
    public void Register_is_idempotent()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint first = registry.Register<TypeA>();
        uint second = registry.Register<TypeA>();
        uint third = registry.Register<TypeA>();

        first.Should().Be(second).And.Be(third);
    }

    [Fact]
    public void GetId_throws_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        Action act = () => registry.GetId<TypeA>();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not registered*");
    }

    [Fact]
    public void TryGetId_returns_false_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        bool found = registry.TryGetId<TypeA>(out uint id);

        found.Should().BeFalse();
        id.Should().Be(0);
    }

    [Fact]
    public void TryGetId_returns_true_after_register()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint registered = registry.Register<TypeA>();
        bool found = registry.TryGetId<TypeA>(out uint id);

        found.Should().BeTrue();
        id.Should().Be(registered);
    }

    [Fact]
    public void Lookup_returns_identity_for_registered_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint id = registry.Register<TypeA>();
        ComponentIdentity? identity = registry.Lookup(id);

        // The reverse map answers with the owner-scoped identity, not a Type: holding a
        // Type here is exactly what rooted a mod's collectible ALC before ID-A.
        identity.Should().NotBeNull();
        identity!.Value.Owner.Should().Be(ComponentTypeRegistry.KernelOwner);
        identity.Value.TypeFullName.Should().Be(typeof(TypeA).FullName);
    }

    [Fact]
    public void Lookup_returns_null_for_unassigned_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        ComponentIdentity? identity = registry.Lookup(999);

        identity.Should().BeNull();
    }

    [Fact]
    public void Count_reflects_registered_types()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.Count.Should().Be(0);
        registry.Register<TypeA>();
        registry.Count.Should().Be(1);
        registry.Register<TypeB>();
        registry.Count.Should().Be(2);
        registry.Register<TypeA>();  // idempotent
        registry.Count.Should().Be(2);
    }

    [Fact]
    public void IsRegistered_reflects_registration_state()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.IsRegistered<TypeA>().Should().BeFalse();
        registry.Register<TypeA>();
        registry.IsRegistered<TypeA>().Should().BeTrue();
        registry.IsRegistered<TypeB>().Should().BeFalse();
    }

    // ---- ID-A: owner-scoped identity ------------------------------------
    //
    // These exercise the property the re-key is named for: identity is
    // (owner, type FullName), never the Type object. Two CLR types sharing a
    // FullName are what a mod reload -- and two mods choosing the same component
    // name -- actually produce, so these build that exact shape with
    // Reflection.Emit rather than approximating it with two unrelated types.

    /// <summary>
    /// Emits a value type with the requested FullName into its own dynamic assembly.
    /// Two calls with the same <paramref name="typeFullName"/> under different
    /// <paramref name="assemblyName"/> values yield two DISTINCT Type objects that
    /// agree on FullName -- the shape a reloaded mod presents to the registry.
    /// </summary>
    private static Type EmitComponentType(string assemblyName, string typeFullName, int intFields)
    {
        var fields = new Type[intFields];
        for (int i = 0; i < intFields; i++)
            fields[i] = typeof(int);
        return EmitComponentType(assemblyName, typeFullName, fields);
    }

    /// <summary>
    /// Emit form taking explicit field types, so a test can hold the FullName and the
    /// total size fixed while changing only the field ORDER or the field TYPES — the
    /// two mutations that keep `Unsafe.SizeOf` constant and still change what the
    /// stored bytes mean.
    /// </summary>
    private static Type EmitComponentType(string assemblyName, string typeFullName, params Type[] fieldTypes)
    {
        AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
        ModuleBuilder module = assembly.DefineDynamicModule(assemblyName);
        TypeBuilder type = module.DefineType(
            typeFullName,
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.SequentialLayout,
            typeof(ValueType));

        for (int i = 0; i < fieldTypes.Length; i++)
            type.DefineField("Field" + i, fieldTypes[i], FieldAttributes.Public);

        return type.CreateType()!;
    }

    [Fact]
    public void SameFullName_TwoOwners_DistinctIds()
    {
        // The OS model as a test: the type NAME is not the identity. Two mods that
        // both ship a "Weather.StateComponent" are two distinct component types with
        // distinct stores, exactly as two same-named programs are distinct processes.
        // If this ever yields one id, mods have begun sharing storage by an accident
        // of naming.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string SharedName = "Rival.Weather.StateComponent";
        Type fromModA = EmitComponentType("IdA.Rival.ModA", SharedName, intFields: 1);
        Type fromModB = EmitComponentType("IdA.Rival.ModB", SharedName, intFields: 1);

        fromModA.Should().NotBeSameAs(fromModB);
        fromModA.FullName.Should().Be(fromModB.FullName, "the point is that the names collide");

        uint idA = registry.Register(fromModA, "mod.alpha");
        uint idB = registry.Register(fromModB, "mod.beta");

        idB.Should().NotBe(idA, "distinct owners means distinct identities means distinct ids");
        registry.Lookup(idA)!.Value.Owner.Should().Be("mod.alpha");
        registry.Lookup(idB)!.Value.Owner.Should().Be("mod.beta");
        registry.Lookup(idA)!.Value.TypeFullName.Should().Be(SharedName);
        registry.Count.Should().Be(2);
    }

    [Fact]
    public void SameIdentity_Reregistration_ReturnsExistingId()
    {
        // Two halves of one guarantee. Re-registering the same Type is idempotent, as
        // it always was; re-registering a DIFFERENT Type carrying the same identity --
        // precisely what a reloaded mod's fresh AssemblyLoadContext produces -- RE-ADOPTS
        // the id instead of allocating a new one. The second half is why a reloaded mod
        // finds its own surviving native store rather than an empty one.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Reload.StateComponent";
        Type firstIncarnation = EmitComponentType("IdA.Reload.Gen1", Name, intFields: 2);
        Type secondIncarnation = EmitComponentType("IdA.Reload.Gen2", Name, intFields: 2);

        uint original = registry.Register(firstIncarnation, "mod.reloadable");
        registry.Register(firstIncarnation, "mod.reloadable").Should().Be(original,
            "re-registering the same Type is idempotent");

        uint afterReload = registry.Register(secondIncarnation, "mod.reloadable");

        afterReload.Should().Be(original,
            "a new Type object with the same identity re-adopts the id, which is what re-attaches " +
            "a reloaded mod to the store its previous incarnation was writing into");
        registry.Count.Should().Be(1, "re-adoption must not mint a second identity row");
    }

    [Fact]
    public void SizeMismatch_OnReload_FailsLoadCleanly()
    {
        // A mod version whose component grew a field. The identity is unchanged, so the
        // registry offers the surviving id back to native -- and native refuses the same
        // id at a different size. That refusal must surface as a clean typed failure,
        // never as a silent reinterpretation of the old store's bytes under a new layout.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Grown.StateComponent";
        Type v1 = EmitComponentType("IdA.Grown.V1", Name, intFields: 1);
        Type v2 = EmitComponentType("IdA.Grown.V2", Name, intFields: 4);

        registry.Register(v1, "mod.grown");

        Action reloadWithNewLayout = () => registry.Register(v2, "mod.grown");

        reloadWithNewLayout.Should().Throw<InvalidOperationException>()
            .WithMessage("*different layout*",
                "the diagnostic must say what went wrong, not merely that native returned 0");
        registry.Count.Should().Be(1, "the refused registration leaves no partial row behind");
    }

    [Fact]
    public void SameSizeDifferentFieldOrder_OnReload_IsRefused()
    {
        // Codex P1 on PR #50. Re-adoption hands a reloaded mod the SURVIVING native store,
        // so equal byte size is not proof that the bytes still mean the same thing.
        // Swapping an int and a float keeps Unsafe.SizeOf at 8 and inverts every stored
        // value's interpretation -- the native size check cannot see it, and before the
        // layout fingerprint this reload succeeded silently.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Reordered.StateComponent";
        Type v1 = EmitComponentType("IdA.Reordered.V1", Name, typeof(int), typeof(float));
        Type v2 = EmitComponentType("IdA.Reordered.V2", Name, typeof(float), typeof(int));

        registry.Register(v1, "mod.reordered");

        Action reload = () => registry.Register(v2, "mod.reordered");

        reload.Should().Throw<InvalidOperationException>()
            .WithMessage("*different layout*",
                "the sizes are identical, so only a layout comparison can catch this");
        registry.Count.Should().Be(1, "the refused re-adoption leaves no second row");
    }

    [Fact]
    public void SameSizeDifferentFieldType_OnReload_IsRefused()
    {
        // The other same-size mutation: int -> float in place. Same width, different
        // meaning for every byte already in the store.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Retyped.StateComponent";
        Type v1 = EmitComponentType("IdA.Retyped.V1", Name, typeof(int));
        Type v2 = EmitComponentType("IdA.Retyped.V2", Name, typeof(float));

        registry.Register(v1, "mod.retyped");

        Action reload = () => registry.Register(v2, "mod.retyped");

        reload.Should().Throw<InvalidOperationException>().WithMessage("*different layout*");
    }

    [Fact]
    public void IdenticalLayout_OnReload_StillReAdopts()
    {
        // The control. The fingerprint must not become a reason that legitimate reloads
        // start failing: an unchanged component re-adopts its id exactly as before, which
        // is the whole resume mechanism.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Stable.StateComponent";
        Type v1 = EmitComponentType("IdA.Stable.V1", Name, typeof(int), typeof(float));
        Type v2 = EmitComponentType("IdA.Stable.V2", Name, typeof(int), typeof(float));

        uint original = registry.Register(v1, "mod.stable");
        uint afterReload = registry.Register(v2, "mod.stable");

        afterReload.Should().Be(original, "an unchanged layout re-adopts, as it must");
        registry.Count.Should().Be(1);
    }

    [Fact]
    public void RollbackRegistration_WithdrawsTheRow_ButDoesNotReissueTheId()
    {
        // Codex P2 on PR #50. A batch that fails after registering a component must not
        // leave its identity behind, or a corrected retry whose layout also changed is
        // refused as a mismatch against a version that was never active.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const string Name = "Withdrawn.StateComponent";
        Type attempt = EmitComponentType("IdA.Withdrawn.V1", Name, typeof(int));
        uint firstId = registry.Register(attempt, "mod.withdrawn");
        registry.Count.Should().Be(1);

        registry.RollbackRegistration(attempt, "mod.withdrawn");

        registry.Count.Should().Be(0, "the withdrawn row must leave no trace in the identity space");
        registry.IsIdentityRegistered("mod.withdrawn", Name).Should().BeFalse();

        // The retry carries a CHANGED layout -- the case that used to be refused.
        Type retry = EmitComponentType("IdA.Withdrawn.V2", Name, typeof(int), typeof(int));
        uint retryId = registry.Register(retry, "mod.withdrawn");

        retryId.Should().NotBe(firstId,
            "the id is NOT reissued: no native store-removal export exists, so handing the " +
            "withdrawn id to a different component would hand it the failed attempt's bytes");
        registry.Count.Should().Be(1);
    }

    [Fact]
    public void ConcurrentRegistration_IsThreadSafe()
    {
        // Dispatch runs system bodies through Parallel.ForEach and resolution is reachable
        // from inside a system body, so registration can be entered concurrently. Distinct
        // identities must each get exactly one id, and repeated registration of one identity
        // from many threads must converge on a single id rather than racing the counter.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        const int Distinct = 24;
        var types = new Type[Distinct];
        for (int i = 0; i < Distinct; i++)
            types[i] = EmitComponentType("IdA.Storm.Asm" + i, "Storm.Component" + i, intFields: 1);

        var observed = new ConcurrentBag<uint>();

        // Each identity is registered four times, from arbitrary threads.
        Parallel.For(0, Distinct * 4, i =>
        {
            Type t = types[i % Distinct];
            observed.Add(registry.Register(t, "mod.storm"));
        });

        registry.Count.Should().Be(Distinct,
            "one identity is one row no matter how many threads asked for it");

        var ids = new HashSet<uint>(observed);
        ids.Count.Should().Be(Distinct, "each identity resolved to exactly one id");
        ids.Should().NotContain(0u, "0 is the reserved invalid id");

        // Sequential allocation survives the storm: 24 identities occupy ids 1..24 with no
        // gap, so the counter was never double-advanced nor left rolled back under a race.
        for (uint expected = 1; expected <= Distinct; expected++)
            ids.Should().Contain(expected);
    }

    // A value type that is NOT unmanaged. The C# `unmanaged` constraint would reject it
    // at compile time; the reflective path cannot, which is what this pins.
#pragma warning disable CS0649
    private struct ReferenceCarryingStruct { public string Value; }
#pragma warning restore CS0649

    [Fact]
    public void Register_RuntimeForm_RefusesValueTypesContainingManagedReferences()
    {
        // Codex P1/P2 on PR #50, verified by probe before the fix: this exact type was
        // ACCEPTED and given native raw storage (id=1). Type.IsValueType is true for it,
        // and the CLR's constraint check behind MakeGenericMethod does not reproduce the
        // compiler's recursive `unmanaged` rule -- so the overload has to ask the runtime
        // itself rather than trust the constraint it declares.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        typeof(ReferenceCarryingStruct).IsValueType.Should().BeTrue(
            "precondition: the weaker IsValueType test passes, which is why it was insufficient");

        Action act = () => registry.Register(typeof(ReferenceCarryingStruct), "mod.sneaky");

        act.Should().Throw<ArgumentException>().WithMessage("*contains managed references*");
        registry.Count.Should().Be(0, "a refused type must not consume an id or create a store");
    }

    [Fact]
    public void Register_RuntimeForm_RefusesManagedComponentTypes()
    {
        // Path beta components are classes held in per-mod managed stores and consume no
        // native id. The runtime-Type entry point is the one the pipeline calls with
        // whatever a mod claimed, so it is where the two paths must be told apart.
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        Action act = () => registry.Register(typeof(string), "mod.confused");

        act.Should().Throw<ArgumentException>().WithMessage("*not a value type*");
    }
}
