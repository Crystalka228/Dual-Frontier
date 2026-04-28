using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Manifest;

/// <summary>
/// Acceptance coverage for the ModManifest v2 schema extension:
/// <see cref="ModKind"/>, default values, v2 construction,
/// <see cref="ModManifest.EffectiveApiVersion"/>, and the
/// <see cref="BridgeImplementationAttribute.Replaceable"/> flag.
/// </summary>
public sealed class ModManifestV2Tests
{
    // --- ModKind ------------------------------------------------------------

    [Fact]
    public void ModKind_Regular_is_default_value_zero()
    {
        ((int)ModKind.Regular).Should().Be(0);
    }

    [Fact]
    public void ModKind_Shared_is_defined()
    {
        Enum.IsDefined(typeof(ModKind), ModKind.Shared).Should().BeTrue();
    }

    // --- Default values -----------------------------------------------------

    [Fact]
    public void Default_manifest_Kind_is_Regular()
    {
        new ModManifest().Kind.Should().Be(ModKind.Regular);
    }

    [Fact]
    public void Default_manifest_ApiVersion_is_null()
    {
        new ModManifest().ApiVersion.Should().BeNull();
    }

    [Fact]
    public void Default_manifest_HotReload_is_false()
    {
        new ModManifest().HotReload.Should().BeFalse();
    }

    [Fact]
    public void Default_manifest_Replaces_is_empty()
    {
        new ModManifest().Replaces.Should().BeEmpty();
    }

    [Fact]
    public void Default_manifest_Capabilities_is_Empty()
    {
        new ModManifest().Capabilities.Should().Be(ManifestCapabilities.Empty);
    }

    [Fact]
    public void Default_manifest_Dependencies_is_empty_ModDependency_list()
    {
        ModManifest manifest = new();

        manifest.Dependencies.Should().BeAssignableTo<IReadOnlyList<ModDependency>>();
        manifest.Dependencies.Should().BeEmpty();
    }

    // --- v2 construction ----------------------------------------------------

    [Fact]
    public void v2_construction_with_ApiVersion_sets_field()
    {
        ModManifest manifest = new()
        {
            Id = "x",
            Name = "y",
            Version = "1.0.0",
            ApiVersion = VersionConstraint.Parse("^1.0.0"),
        };

        manifest.ApiVersion.Should().NotBeNull();
        manifest.ApiVersion!.Value.ToString().Should().Be("^1.0.0");
    }

    [Fact]
    public void v1_compatible_construction_compiles_with_no_v2_fields_set()
    {
        ModManifest manifest = new() { Id = "x", Name = "y", Version = "1.0.0" };

        manifest.Kind.Should().Be(ModKind.Regular);
        manifest.ApiVersion.Should().BeNull();
        manifest.HotReload.Should().BeFalse();
        manifest.Replaces.Should().BeEmpty();
        manifest.Capabilities.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void v2_construction_assigns_all_new_fields()
    {
        ModDependency dep = ModDependency.Required(
            "com.example.foo",
            VersionConstraint.Parse("^1.0.0"));

        ManifestCapabilities caps = ManifestCapabilities.Parse(
            new[] { "kernel.publish:Foo" },
            new[] { "mod.com.example.bar.subscribe:Bar" });

        ModManifest manifest = new()
        {
            Id = "com.example.test",
            Name = "Test Mod",
            Version = "1.2.3",
            Kind = ModKind.Shared,
            ApiVersion = VersionConstraint.Parse("^1.0.0"),
            HotReload = true,
            Dependencies = new[] { dep },
            Replaces = new[] { "Some.Bridge.System" },
            Capabilities = caps,
        };

        manifest.Kind.Should().Be(ModKind.Shared);
        manifest.HotReload.Should().BeTrue();
        manifest.Dependencies.Should().ContainSingle().Which.Should().Be(dep);
        manifest.Replaces.Should().ContainSingle().Which.Should().Be("Some.Bridge.System");
        manifest.Capabilities.Should().Be(caps);
    }

    // --- EffectiveApiVersion ------------------------------------------------

    [Fact]
    public void EffectiveApiVersion_returns_ApiVersion_when_set()
    {
        VersionConstraint apiVersion = VersionConstraint.Parse("^2.1.0");
        ModManifest manifest = new() { ApiVersion = apiVersion };

        manifest.EffectiveApiVersion.Should().Be(apiVersion);
    }

    [Fact]
    public void EffectiveApiVersion_falls_back_to_RequiresContractsVersion_when_ApiVersion_null()
    {
        ModManifest manifest = new()
        {
            ApiVersion = null,
            RequiresContractsVersion = "2.3.4",
        };

        manifest.EffectiveApiVersion.Floor.Should().Be(new ContractsVersion(2, 3, 4));
        manifest.EffectiveApiVersion.Kind.Should().Be(VersionConstraintKind.Exact);
    }

    [Fact]
    public void Default_manifest_EffectiveApiVersion_floor_is_1_0_0()
    {
        ModManifest manifest = new();

        manifest.EffectiveApiVersion.Floor.Should().Be(new ContractsVersion(1, 0, 0));
    }

    // --- BridgeImplementationAttribute --------------------------------------

    [Fact]
    public void BridgeImplementation_Phase_only_compiles_with_Replaceable_default_false()
    {
        BridgeImplementationAttribute attr = new() { Phase = 5 };

        attr.Replaceable.Should().BeFalse();
    }

    [Fact]
    public void BridgeImplementation_Phase_and_Replaceable_true_compiles_and_sets_field()
    {
        BridgeImplementationAttribute attr = new() { Phase = 5, Replaceable = true };

        attr.Phase.Should().Be(5);
        attr.Replaceable.Should().BeTrue();
    }

    [Fact]
    public void BridgeImplementation_default_Replaceable_is_false()
    {
        new BridgeImplementationAttribute { Phase = 3 }.Replaceable.Should().BeFalse();
    }

    [Fact]
    public void BridgeImplementation_explicit_Replaceable_true_returns_true()
    {
        new BridgeImplementationAttribute { Phase = 3, Replaceable = true }
            .Replaceable.Should().BeTrue();
    }
}
