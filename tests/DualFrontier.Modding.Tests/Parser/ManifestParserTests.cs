using System;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Parser;

/// <summary>
/// Acceptance coverage for <see cref="ManifestParser"/> against the v2 schema
/// in MOD_OS_ARCHITECTURE §3 plus v1 backward-compatibility rules.
/// </summary>
public sealed class ManifestParserTests
{
    // --- v2 full ------------------------------------------------------------

    [Fact]
    public void Parse_v2_full_manifest_populates_all_fields()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.combat",
          "name": "Combat",
          "version": "1.0.0",
          "kind": "regular",
          "apiVersion": "^1.0.0",
          "hotReload": true,
          "dependencies": [
            { "id": "com.example.core", "version": "^1.0.0" },
            { "id": "com.example.util", "version": "^2.0.0", "optional": true }
          ],
          "replaces": ["A.B.CombatSystem"],
          "capabilities": {
            "required": ["kernel.publish:A.B.DamageEvent"],
            "provided": ["mod.com.example.combat.publish:A.B.WeaponDef"]
          }
        }
        """;

        ModManifest manifest = ManifestParser.Parse(json, "test");

        manifest.Id.Should().Be("com.example.combat");
        manifest.Name.Should().Be("Combat");
        manifest.Version.Should().Be("1.0.0");
        manifest.Kind.Should().Be(ModKind.Regular);

        manifest.ApiVersion.Should().NotBeNull();
        manifest.ApiVersion!.Value.Floor.Should().Be(new ContractsVersion(1, 0, 0));
        manifest.ApiVersion!.Value.Kind.Should().Be(VersionConstraintKind.Caret);

        manifest.HotReload.Should().BeTrue();

        manifest.Dependencies.Should().HaveCount(2);
        manifest.Dependencies[0].Should().Be(
            new ModDependency("com.example.core", VersionConstraint.Parse("^1.0.0"), IsOptional: false));
        manifest.Dependencies[1].ModId.Should().Be("com.example.util");
        manifest.Dependencies[1].Version.Should().Be(VersionConstraint.Parse("^2.0.0"));
        manifest.Dependencies[1].IsOptional.Should().BeTrue();

        manifest.Replaces.Should().HaveCount(1);
        manifest.Replaces[0].Should().Be("A.B.CombatSystem");

        manifest.Capabilities.Required.Should().Contain("kernel.publish:A.B.DamageEvent");
        manifest.Capabilities.Provided.Should().Contain("mod.com.example.combat.publish:A.B.WeaponDef");
    }

    // --- v1 compat ----------------------------------------------------------

    [Fact]
    public void Parse_v1_legacy_manifest_with_string_dependencies()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.legacy",
          "name": "Legacy",
          "version": "1.0.0",
          "requiresContractsVersion": "1.0.0",
          "dependencies": ["com.example.dep"]
        }
        """;

        ModManifest manifest = ManifestParser.Parse(json, "test");

        manifest.Kind.Should().Be(ModKind.Regular);
        manifest.ApiVersion.Should().BeNull();
        manifest.HotReload.Should().BeFalse();
        manifest.Replaces.Should().BeEmpty();
        manifest.Capabilities.Should().Be(ManifestCapabilities.Empty);

        manifest.Dependencies.Should().HaveCount(1);
        manifest.Dependencies[0].Should().Be(
            new ModDependency("com.example.dep", null, IsOptional: false));

        manifest.RequiresContractsVersion.Should().Be("1.0.0");
    }

    // --- defaults -----------------------------------------------------------

    [Fact]
    public void Missing_kind_defaults_to_Regular()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.Kind.Should().Be(ModKind.Regular);
    }

    [Fact]
    public void Missing_apiVersion_returns_null()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.ApiVersion.Should().BeNull();
    }

    [Fact]
    public void Missing_hotReload_defaults_to_false()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.HotReload.Should().BeFalse();
    }

    [Fact]
    public void Missing_dependencies_returns_empty_list()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.Dependencies.Should().BeEmpty();
    }

    [Fact]
    public void Missing_replaces_returns_empty_list()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.Replaces.Should().BeEmpty();
    }

    [Fact]
    public void Missing_capabilities_returns_Empty()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.Capabilities.Should().Be(ManifestCapabilities.Empty);
    }

    [Fact]
    public void Missing_author_defaults_to_empty_string()
    {
        ModManifest manifest = ManifestParser.Parse(MinimalJson(), "test");
        manifest.Author.Should().Be(string.Empty);
    }

    // --- error cases --------------------------------------------------------

    [Fact]
    public void Missing_id_throws_InvalidOperationException()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "name": "Test",
          "version": "1.0.0"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*'id'*");
    }

    [Fact]
    public void Empty_id_throws_InvalidOperationException()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "",
          "name": "Test",
          "version": "1.0.0"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*'id'*");
    }

    [Fact]
    public void Missing_name_throws()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "version": "1.0.0"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*'name'*");
    }

    [Fact]
    public void Missing_version_throws()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*'version'*");
    }

    [Fact]
    public void Unknown_kind_throws()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test",
          "version": "1.0.0",
          "kind": "unknown"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*kind*unknown*");
    }

    [Fact]
    public void Tilde_apiVersion_throws_wrapping_FormatException()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test",
          "version": "1.0.0",
          "apiVersion": "~1.0.0"
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*apiVersion*")
            .WithInnerException<FormatException>();
    }

    [Fact]
    public void Invalid_capability_token_throws_wrapping_ArgumentException()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test",
          "version": "1.0.0",
          "capabilities": {
            "required": ["bad-token"]
          }
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*capability*")
            .WithInnerException<ArgumentException>();
    }

    [Fact]
    public void Mixed_dependencies_array_throws()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test",
          "version": "1.0.0",
          "dependencies": [
            "com.example.first",
            { "id": "com.example.second" }
          ]
        }
        """;

        Action act = () => ManifestParser.Parse(json, "test");
        act.Should().Throw<InvalidOperationException>().WithMessage("*dependencies*");
    }

    // --- misc ---------------------------------------------------------------

    [Fact]
    public void Property_names_are_case_insensitive()
    {
        const string json = """
        {
          "manifestVersion": "3",
          "ID": "com.example.test",
          "NAME": "Test",
          "Version": "1.0.0"
        }
        """;

        ModManifest manifest = ManifestParser.Parse(json, "test");

        manifest.Id.Should().Be("com.example.test");
        manifest.Name.Should().Be("Test");
        manifest.Version.Should().Be("1.0.0");
    }

    [Fact]
    public void Comments_and_trailing_commas_are_tolerated()
    {
        const string json = """
        {
          "manifestVersion": "3",
          // header comment
          "id": "com.example.test",
          "name": "Test", // inline comment
          "version": "1.0.0",
        }
        """;

        ModManifest manifest = ManifestParser.Parse(json, "test");

        manifest.Id.Should().Be("com.example.test");
    }

    // --- helpers ------------------------------------------------------------

    private static string MinimalJson() =>
        """
        {
          "manifestVersion": "3",
          "id": "com.example.x",
          "name": "Test",
          "version": "1.0.0"
        }
        """;
}
