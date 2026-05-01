using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Helper-level coverage for the M6.2 skip-set construction
/// (<see cref="ModIntegrationPipeline.CollectReplacedFqnsForTests"/>).
/// The production helper is private static; these tests reach it through
/// the internal seam exposed via <c>InternalsVisibleTo</c> per
/// <c>docs/ISOLATION.md</c>. They exercise boundary cases of FQN
/// collection without going through the full
/// <see cref="ModIntegrationPipeline.Apply"/> orchestration — that is
/// covered by the M6.2 integration tests.
///
/// Phase H pre-conditions are deliberately bypassed here: a same-FQN
/// duplicate across two mods would normally fail Phase H and never reach
/// the helper, but the helper itself must still be order- and
/// duplicate-safe so a future skip path that legitimately produces
/// duplicates (e.g. internal pipeline retries) does not corrupt the set.
/// </summary>
public sealed class CollectReplacedFqnsTests
{
    [Fact]
    public void EmptyLoadedList_ReturnsEmptySet()
    {
        // Boundary: pipeline applied with no mods (e.g. cold-start).
        // Helper must yield an empty, allocated set rather than null.
        HashSet<string> result =
            ModIntegrationPipeline.CollectReplacedFqnsForTests(Array.Empty<LoadedMod>());

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void SingleModWithMultipleReplaces_AllCollected()
    {
        // Single mod replaces three distinct FQNs. All three must surface
        // in the resulting set — the helper does not prune by validity
        // (Phase H already vetted them) nor by count.
        LoadedMod mod = MakeMod("com.example.multi",
            "DualFrontier.Systems.Combat.CombatSystem",
            "DualFrontier.Systems.Combat.DamageSystem",
            "DualFrontier.Systems.Combat.ProjectileSystem");

        HashSet<string> result =
            ModIntegrationPipeline.CollectReplacedFqnsForTests(new[] { mod });

        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(
            "DualFrontier.Systems.Combat.CombatSystem",
            "DualFrontier.Systems.Combat.DamageSystem",
            "DualFrontier.Systems.Combat.ProjectileSystem");
    }

    [Fact]
    public void MultipleModsWithDistinctReplaces_AllCollected()
    {
        // Three mods each replace one disjoint FQN. The helper unions the
        // entries across mods — the result must contain every FQN from
        // every mod regardless of mod order.
        LoadedMod a = MakeMod("com.example.a", "Some.Namespace.SystemA");
        LoadedMod b = MakeMod("com.example.b", "Some.Namespace.SystemB");
        LoadedMod c = MakeMod("com.example.c", "Some.Namespace.SystemC");

        HashSet<string> result =
            ModIntegrationPipeline.CollectReplacedFqnsForTests(new[] { a, b, c });

        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(
            "Some.Namespace.SystemA",
            "Some.Namespace.SystemB",
            "Some.Namespace.SystemC");
    }

    [Fact]
    public void MultipleModsWithSameFqn_DeduplicatedToSingle()
    {
        // Two mods declare the same FQN. Phase H would normally reject
        // this batch with BridgeReplacementConflict before the helper
        // runs, but the helper itself is dedup-safe — its
        // StringComparer.Ordinal-keyed HashSet collapses the duplicate.
        // Documents the order-and-uniqueness invariant the pipeline
        // relies on for the skip loop.
        LoadedMod a = MakeMod("com.example.a", "Same.Conflicting.System");
        LoadedMod b = MakeMod("com.example.b", "Same.Conflicting.System");

        HashSet<string> result =
            ModIntegrationPipeline.CollectReplacedFqnsForTests(new[] { a, b });

        result.Should().HaveCount(1);
        result.Should().Contain("Same.Conflicting.System");
    }

    [Fact]
    public void ModWithEmptyReplaces_ContributesNothing()
    {
        // A mod with an empty Manifest.Replaces (the common case for
        // non-overriding mods) must not appear in the skip set — the
        // helper iterates the inner string list, and an empty list yields
        // zero contributions. Mixing it with a mod that does declare
        // replacements proves the empty case is silently no-op rather
        // than an error.
        LoadedMod empty = MakeMod("com.example.empty");
        LoadedMod active = MakeMod(
            "com.example.active", "DualFrontier.Systems.Combat.CombatSystem");

        HashSet<string> result =
            ModIntegrationPipeline.CollectReplacedFqnsForTests(new[] { empty, active });

        result.Should().HaveCount(1);
        result.Should().Contain("DualFrontier.Systems.Combat.CombatSystem");
    }

    private static LoadedMod MakeMod(string modId, params string[] replaces)
    {
        var manifest = new ModManifest
        {
            Id = modId,
            Name = modId,
            Version = "1.0.0",
            Author = "Test",
            Replaces = replaces,
        };
        var context = new ModLoadContext(modId);
        return new LoadedMod(modId, manifest, new StubMod(), context, Array.Empty<Type>());
    }

    private sealed class StubMod : IMod
    {
        public void Initialize(IModApi api) { }
        public void Unload() { }
    }
}
