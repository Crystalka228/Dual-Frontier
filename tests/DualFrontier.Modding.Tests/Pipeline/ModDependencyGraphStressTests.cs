using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Modding;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// Stress suite for the managed mod-load dependency graph
/// (<see cref="ModIntegrationPipeline.TopoSortRegularMods"/> +
/// <see cref="ModIntegrationPipeline.CheckDependencyPresence"/>). Hammers
/// the Kahn's-algorithm implementation with 5000-mod random DAGs from many
/// threads in parallel — the function is documented as pure, so identical
/// inputs must yield topologically-valid outputs regardless of caller
/// concurrency.
///
/// Mirrors the test-categorisation pattern of
/// <c>DualFrontier.Core.Tests.Scheduling.SchedulerStressTests</c>: tagged
/// <c>[Trait("Category","Stress")]</c> so CI can opt-out.
/// </summary>
[Trait("Category", "Stress")]
public sealed class ModDependencyGraphStressTests
{
    [Fact]
    public void TopoSortRegularMods_FiveThousandModRandomDag_ProducesValidOrdering()
    {
        const int ModCount = 5000;
        var (mods, byId) = BuildRandomAcyclicModBatch(ModCount, seed: 0xDF_BA7CH);

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(mods, byId);

        cycleErrors.Should().BeEmpty();
        sorted.Count.Should().Be(mods.Count);
        AssertTopologicallyValid(sorted, byId);
    }

    [Fact]
    public void TopoSortRegularMods_CalledInParallelFromManyThreads_ProducesConsistentValidOrdering()
    {
        // Pure function contract — concurrent callers must observe valid
        // toposorts regardless of permutation of the input list.
        const int ModCount = 2000;
        const int ParallelThreads = 64;
        const int IterationsPerThread = 50;

        var (mods, byId) = BuildRandomAcyclicModBatch(ModCount, seed: 0xDF_C0_DE);
        var failures = new ConcurrentBag<string>();

        Parallel.For(0, ParallelThreads, threadIdx =>
        {
            var rng = new Random(threadIdx * 104729 + 1);
            for (int iter = 0; iter < IterationsPerThread; iter++)
            {
                // Permute the input order before each call; topology must
                // still come out valid.
                var permuted = mods.ToArray();
                ShuffleInPlace(permuted, rng);

                (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> errors) =
                    ModIntegrationPipeline.TopoSortRegularMods(permuted, byId);

                if (errors.Count != 0)
                {
                    failures.Add($"thread {threadIdx} iter {iter}: cycle errors on acyclic input");
                    continue;
                }
                if (sorted.Count != permuted.Length)
                {
                    failures.Add($"thread {threadIdx} iter {iter}: sort dropped mods");
                    continue;
                }
                string? topoFailure = FindTopologyViolation(sorted, byId);
                if (topoFailure is not null)
                {
                    failures.Add($"thread {threadIdx} iter {iter}: {topoFailure}");
                }
            }
        });

        failures.Should().BeEmpty();
    }

    [Fact]
    public void TopoSortRegularMods_FiveThousandModsWithCycles_ReportsAllParticipants()
    {
        // Build an acyclic batch first, then sprinkle in 100 two-mod cycles
        // (each contributing 2 cyclic participants -> 200 expected errors).
        const int AcyclicCount = 4800;
        const int CyclicPairs = 100;
        var (mods, byId) = BuildRandomAcyclicModBatch(AcyclicCount, seed: 0xCAFE_F00D);
        var allMods = new List<ModManifest>(mods);
        var allByIdMutable = new Dictionary<string, ModManifest>(byId, StringComparer.Ordinal);

        for (int p = 0; p < CyclicPairs; p++)
        {
            string idA = $"stress.cyclic.{p}.a";
            string idB = $"stress.cyclic.{p}.b";
            var a = new ModManifest
            {
                Id = idA,
                Kind = ModKind.Regular,
                Dependencies = new[] { ModDependency.Required(idB) },
            };
            var b = new ModManifest
            {
                Id = idB,
                Kind = ModKind.Regular,
                Dependencies = new[] { ModDependency.Required(idA) },
            };
            allMods.Add(a);
            allMods.Add(b);
            allByIdMutable[idA] = a;
            allByIdMutable[idB] = b;
        }

        (IReadOnlyList<ModManifest> sorted, IReadOnlyList<ValidationError> cycleErrors) =
            ModIntegrationPipeline.TopoSortRegularMods(allMods, allByIdMutable);

        sorted.Count.Should().Be(AcyclicCount,
            "acyclic mods must still sort; only cycle participants are excluded");
        cycleErrors.Count.Should().Be(CyclicPairs * 2,
            "every participant in every cycle must be reported");
        cycleErrors.Should().OnlyContain(e =>
            e.Kind == ValidationErrorKind.CyclicDependency);
    }

    [Fact]
    public void CheckDependencyPresence_LargeBatchWithMissingDeps_ReportsErrorsAndWarnings()
    {
        // Build a base batch, then add 1000 mods with missing required deps
        // and 1000 with missing optional deps. Expectation: 1000 errors,
        // 1000 warnings.
        const int BaseCount = 3000;
        const int MissingRequired = 1000;
        const int MissingOptional = 1000;
        var (mods, byId) = BuildRandomAcyclicModBatch(BaseCount, seed: 0xBEEF_0042);
        var mutableBatch = new Dictionary<string, ModManifest>(byId, StringComparer.Ordinal);

        for (int i = 0; i < MissingRequired; i++)
        {
            string id = $"stress.missing.req.{i}";
            mutableBatch[id] = new ModManifest
            {
                Id = id,
                Kind = ModKind.Regular,
                Dependencies = new[] { ModDependency.Required($"does.not.exist.req.{i}") },
            };
        }
        for (int i = 0; i < MissingOptional; i++)
        {
            string id = $"stress.missing.opt.{i}";
            mutableBatch[id] = new ModManifest
            {
                Id = id,
                Kind = ModKind.Regular,
                Dependencies = new[] { ModDependency.Optional($"does.not.exist.opt.{i}") },
            };
        }

        (IReadOnlyList<ValidationError> errors, IReadOnlyList<ValidationWarning> warnings) =
            ModIntegrationPipeline.CheckDependencyPresence(mutableBatch);

        errors.Count.Should().Be(MissingRequired);
        warnings.Count.Should().Be(MissingOptional);
        errors.Should().OnlyContain(e => e.Kind == ValidationErrorKind.MissingDependency);
    }

    // ════════════════════════════════════════════════════════════════════════
    // Helpers
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Builds a random acyclic dependency batch. Each mod with index i has
    /// 0–3 required dependencies, all chosen from strictly lower indices.
    /// Forward-only edges guarantee acyclicity; deterministic by seed.
    /// </summary>
    private static (IReadOnlyList<ModManifest> Mods, IReadOnlyDictionary<string, ModManifest> ById)
        BuildRandomAcyclicModBatch(int count, int seed)
    {
        var rng = new Random(seed);
        var mods = new List<ModManifest>(count);
        var byId = new Dictionary<string, ModManifest>(count, StringComparer.Ordinal);

        for (int i = 0; i < count; i++)
        {
            string id = $"stress.mod.{i:D5}";
            int depCount = i == 0 ? 0 : rng.Next(0, Math.Min(4, i + 1));
            var deps = new ModDependency[depCount];
            // Track chosen indices so we don't pick the same dep twice.
            var picked = new HashSet<int>();
            for (int d = 0; d < depCount; d++)
            {
                int target;
                int safety = 0;
                do
                {
                    target = rng.Next(0, i);
                    safety++;
                } while (!picked.Add(target) && safety < 16);
                deps[d] = ModDependency.Required($"stress.mod.{target:D5}");
            }
            var manifest = new ModManifest
            {
                Id = id,
                Kind = ModKind.Regular,
                Dependencies = deps,
            };
            mods.Add(manifest);
            byId[id] = manifest;
        }

        return (mods, byId);
    }

    private static void ShuffleInPlace<T>(T[] arr, Random rng)
    {
        for (int i = arr.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }

    private static void AssertTopologicallyValid(
        IReadOnlyList<ModManifest> sorted,
        IReadOnlyDictionary<string, ModManifest> byId)
    {
        string? violation = FindTopologyViolation(sorted, byId);
        violation.Should().BeNull();
    }

    /// <summary>
    /// Walks the sorted list and confirms that for every required regular-mod
    /// dependency the dependency precedes the dependent. Returns a diagnostic
    /// string on the first violation, or null if the ordering is valid.
    /// </summary>
    private static string? FindTopologyViolation(
        IReadOnlyList<ModManifest> sorted,
        IReadOnlyDictionary<string, ModManifest> byId)
    {
        var index = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < sorted.Count; i++)
            index[sorted[i].Id] = i;

        for (int i = 0; i < sorted.Count; i++)
        {
            ModManifest mod = sorted[i];
            foreach (ModDependency dep in mod.Dependencies)
            {
                if (!byId.TryGetValue(dep.ModId, out ModManifest? target))
                    continue;
                if (target.Kind != ModKind.Regular)
                    continue;
                if (!index.TryGetValue(dep.ModId, out int depIdx))
                    continue;
                if (depIdx >= i)
                {
                    return $"mod '{mod.Id}' at index {i} depends on '{dep.ModId}' " +
                           $"at index {depIdx} (must precede)";
                }
            }
        }
        return null;
    }
}
