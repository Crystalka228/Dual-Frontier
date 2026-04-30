using System;
using System.Reflection;
using System.Runtime.Loader;
using DualFrontier.Application.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Sharing;

/// <summary>
/// Supporting coverage for <see cref="SharedModLoadContext"/> and
/// <see cref="ModLoadContext"/>'s shared-ALC delegation. The cross-ALC
/// pub/sub roundtrip lives in
/// <see cref="CrossAlcTypeIdentityTests"/>; these cases pin the resolution
/// rules and the duplicate-load guard.
/// </summary>
public sealed class SharedAssemblyResolutionTests
{
    [Fact]
    public void Loading_same_shared_mod_twice_throws()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        Action act = () => loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already loaded*");
    }

    [Fact]
    public void ModLoadContext_without_shared_alc_returns_null_for_any_name()
    {
        var ctx = new ModLoadContext("regular-no-shared", sharedAlc: null);

        Assembly? result = InvokeLoad(ctx, new AssemblyName("Fixture.SharedEvents"));

        result.Should().BeNull(
            "with no shared ALC supplied, the override must defer all resolution to the default context");
    }

    [Fact]
    public void ModLoadContext_with_shared_alc_resolves_cached_assembly()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        LoadedSharedMod shared = loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        var regularCtx = new ModLoadContext("regular-with-shared", sharedAlc);

        Assembly? cached = InvokeLoad(regularCtx, new AssemblyName(shared.Assembly.GetName().Name!));

        cached.Should().BeSameAs(shared.Assembly,
            "ModLoadContext.Load must return the shared ALC's assembly so cross-mod " +
            "type references resolve to the same Type instance");
    }

    [Fact]
    public void ModLoadContext_with_shared_alc_returns_null_for_unknown_name()
    {
        var loader = new ModLoader();
        var sharedAlc = new SharedModLoadContext();
        loader.LoadSharedMod(TestModPaths.SharedEvents, sharedAlc);

        var regularCtx = new ModLoadContext("regular-with-shared", sharedAlc);

        Assembly? result = InvokeLoad(regularCtx, new AssemblyName("DualFrontier.Contracts"));

        result.Should().BeNull(
            "DualFrontier.Contracts is not owned by the shared ALC; the override " +
            "must return null so the default context handles it");
    }

    private static Assembly? InvokeLoad(AssemblyLoadContext ctx, AssemblyName name)
    {
        MethodInfo method = typeof(AssemblyLoadContext)
            .GetMethod("Load", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException(
                "AssemblyLoadContext.Load is no longer accessible via reflection.");
        return (Assembly?)method.Invoke(ctx, new object?[] { name });
    }
}
