---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-TESTING_STRATEGY
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "2.2.0"
next_review_due: 2027-06-11
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-TESTING_STRATEGY
---

# Testing strategy — Dual Frontier

## §1 — Authority and why this document exists

Testing in Dual Frontier is an architectural discipline, not a collection of conventions. Tests exist so that the architectural guarantees — native ECS storage ownership, the ALC mod-isolation boundary, bus delivery semantics, the К-Lxx invariant series — are verified at every cascade closure rather than merely documented. This document is the project's standing test LAW: the single source of truth for test layers, census contracts, brief integration, and invocation reality.

**Briefs cite this document; they do not restate it.** From this cascade onward, every brief that specifies tests references `TESTING_STRATEGY.md §N` by section anchor for any pattern codified here. A brief that re-specifies a pattern inline duplicates law and is defective per §6.4.

### §1.1 — What this document governs

- The test-layer taxonomy (§3) and the layer assignment of every test in the repository.
- The census contracts for reserved surface, marker families, and waivers (§4) — what is counted, by which exact expression, and what a count change obligates.
- The mapping discipline between validation criteria and named tests (§5), including the prohibition on testing lying stubs.
- The division of labor between briefs and this document (§6).
- The closure-audit obligations every cascade owes (§7).
- The invocation commands that actually run the suites (§8).

### §1.2 — What this document does not govern

- Per-cascade test specifications — those live in the owning brief (Category D), in the §6.1 carry format.
- Source-code style, marker syntax, and commit discipline — `CODING_STANDARDS.md` (cited where load-bearing).
- The closure protocol itself — `METHODOLOGY.md §12.7` is canonical; §7 here extends it with test-law obligations and does not duplicate it.
- Architecture content — `KERNEL_ARCHITECTURE.md`, `ANALYZER_RULES.md`, and peers define what is true; this document defines how truth is verified.

When execution encounters a testing question this document does not answer, the response is: stop, propose an amendment per §9.1, obtain ratification — not improvise per file.

### §1.3 — Stack truth

The managed test stack is **xUnit 2.9.2 + FluentAssertions 6.12.1 + Microsoft.NET.Test.Sdk 17.11.1 + xunit.runner.visualstudio 2.8.2**, all version-resolved through Central Package Management (`Directory.Packages.props` at repo root). The analyzer-test project additionally references **Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit 1.1.2** plus an explicit **Microsoft.CodeAnalysis.CSharp.Workspaces 5.3.0** override (the override forces the CPM-pinned Workspaces to win over the testing package's transitive 1.0.1 — without it MEF composition fails at first run; recorded in `tests/DualFrontier.Analyzers.Tests/DualFrontier.Analyzers.Tests.csproj`). The native test stack is a standalone C++ selftest executable built by CMake (§2.5).

There is **no CI pipeline**. Every gate named in this document is executed locally — by the executing agent during a cascade and at closure per §7. Any future CI claim must name its on-disk workflow artifact before it may appear here.

### §1.4 — Authority chain

This document derives from and interlocks with:

- `docs/methodology/METHODOLOGY.md` — §12.7 closure protocol (canonical); §12.8 brief-integration boundary (cross-document form of §6.3/§6.4).
- `docs/methodology/CODING_STANDARDS.md` — §5 «Marker family registry» (census expressions registered there), §5.3 DFK-WAIVER law, §8 atomic commit discipline.
- `docs/methodology/RESERVED_SURFACE_MUTABILITY.md` — the census-delta / `Skeleton revisions` commit-body law that makes §4 pin updates auditable.
- `docs/architecture/ANALYZER_RULES.md` — rule specifications the §3.5 analyzer-test convention will verify.
- `docs/ROADMAP.md` — §«Analyzer track» (Phase β scope pointers) and §«Findings ledger (F-series)» (open defects, including §2.6).

## §2 — Test landscape truth

Surveyed 2026-06-11 against `main` at `d90b522`. Every count below was measured by `rg`/`dotnet test` against the working tree; this section states what exists, not what is intended.

### §2.1 — dotnet-test-invocable projects (9)

All nine live under `tests/`, are members of `DualFrontier.sln`, and target the §1.3 stack.

| Project | .cs files | `[Fact]`/`[Theory]` sites | 2026-06-11 Release run | Verdict |
|---|---:|---:|---|---|
| `DualFrontier.Core.Tests` | 18 | 96 | 83 passed / 1 failed / 84 reported | Real. Managed ECS, bus, scheduler, math. Run truncated by testhost crash after `SchedulerStressTests` (§2.6). |
| `DualFrontier.Core.Interop.Tests` | 26 | 199 | 202 / 202 | Real. P/Invoke boundary suite (§3.2). |
| `DualFrontier.Modding.Tests` | 45 | 382 | 398 / 399 | Real. Largest suite; ALC isolation + fixture projection (§3.4). 1 timing-sensitive failure (§2.6). |
| `DualFrontier.Runtime.Tests` | 36 | 251 | 292 / 292 | Real. Runtime substrate suite. |
| `DualFrontier.Application.Tests` | 5 | 45 | 45 / 45 | Real. Bootstrap/loop application layer. |
| `DualFrontier.Mod.ManifestRewriter.Tests` | 1 | 7 | 7 / 7 | Real. Tooling suite for the manifest rewriter. |
| `DualFrontier.Persistence.Tests` | 4 | 4 | 4 / 4 | Nascent. Real but thin; persistence scaffold scale. |
| `DualFrontier.Systems.Tests` | 2 | 2 | 2 / 2 | Thin real. `SmokeTests` + `NativeWorldTestFixture`. |
| `DualFrontier.Analyzers.Tests` | 1 | 1 | 1 / 1 | **PLACEHOLDER.** Single assembly-identity `[Fact]` in `PlaceholderTests.cs` confirming `DualFrontier.Analyzers` loads in the Roslyn test host. Per A'.9.1 Phase α design; per-rule verifier tests land at Phase β (its own XML doc says so). |

Run-reported totals exceed attribute-site counts where `[Theory]` rows expand (Modding, Runtime, Core.Interop); `Core.Tests` reports fewer than its 96 sites because the 2026-06-11 run crashed before full reporting (§2.6).

### §2.2 — Non-test executables under tests/ (2)

Two projects live under `tests/` but are **not dotnet-test-invocable** (`OutputType=Exe`, no test SDK wiring):

- `tests/DualFrontier.Core.Benchmarks` — BenchmarkDotNet 0.13.12 console executable. Run via `dotnet run -c Release`, never via `dotnet test`. Benchmarks measure; they do not gate — no regression-gate artifact exists (any future gate appears first as `Planned — see docs/ROADMAP.md`).
- `tests/DualFrontier.Runtime.SmokeTest` — standalone smoke executable for manual runtime verification.

A closure report that says «all tests pass» refers to the nine §2.1 projects plus the native selftest (§2.5); it makes no claim about these two executables unless it ran them and says so.

### §2.3 — Fixture mod projects (19)

Nineteen `tests/Fixture.*` projects feed the Modding suite:

- **6 in the solution**: `Fixture.SharedEvents`, `Fixture.PublisherMod`, `Fixture.SubscriberMod`, `Fixture.BadRegularMod`, `Fixture.BadSharedMod_WithIMod`, `Fixture.VanillaMod_HotReloadOverride`.
- **13 disk-only** (`Fixture.RegularMod_*`, pulled transitively when `DualFrontier.Modding.Tests` builds): `BadApiVersion`, `CyclicA`, `CyclicB`, `DependedOn`, `DependsOnAnother`, `DependsOnBadApi`, `DepsBadVersion`, `MissingOptional`, `MissingRequired`, `ReplacesCombat`, `ReplacesCombat_Alt`, `ReplacesProtected`, `ReplacesUnknown`.

The wiring is the **cross-ALC projection pattern**: `DualFrontier.Modding.Tests.csproj` references fixtures with `<ReferenceOutputAssembly>false</ReferenceOutputAssembly>`, so fixture binaries are built and copied for the suite to load through real `AssemblyLoadContext` machinery while their types stay out of the test assembly's compile-time graph. This is what makes the isolation tests honest — the test project cannot accidentally hold compile-time references to what the ALC boundary is supposed to block.

### §2.4 — Mod projects (7)

`mods/` holds 7 mod projects: `DualFrontier.Mod.Example` (in solution) and 6 disk-only `DualFrontier.Mod.Vanilla.*` (Combat, Core, Inventory, Magic, Pawn, World). They are production mod surface, not test fixtures; they appear here because Phase β analyzer sweeps include them (brief A'.9.1 §7.2 violation inventory counts `mod_violations` separately).

### §2.5 — Native selftest

`df_native_selftest.exe` — a CMake target in `native/DualFrontier.Core.Native`, source `native/DualFrontier.Core.Native/test/selftest.cpp`. It is a standalone executable running `DF_CHECK`-macro scenario functions; it prints `ALL PASSED` and exits 0 on success, or prints the failure count and exits 1. It is **not under `dotnet test`** and is invoked as an exe (§8). Binaries exist on disk for both configurations: `native/DualFrontier.Core.Native/build/Debug/df_native_selftest.exe` and `.../build/Release/df_native_selftest.exe`.

### §2.6 -- Honesty register: known-failing and quarantined tests

As of the F-29 native-scheduler cascade (2026-07-04) the F-29 defects are resolved and this register
states the current honest state. No test fails deterministically in per-project isolation, and the full
Core.Tests run is green + stable. Two quarantine categories remain, declared here so no cascade silently
absorbs them (F-ledger: docs/ROADMAP.md Findings ledger).

1. Quarantined -- do not run (managed marathon, F-30). One SchedulerExtremeTests scenario: S2
   (S2_ParallelSystemScheduler_..., 200,000-tick TPL steady-state over a pre-built 80-system phase
   list). It is a managed ParallelSystemScheduler marathon, NOT a native-scale scenario -- the native
   graph is trivial and built once; its non-completion is the 200k-tick wall-clock, unrelated to the
   native scheduler. Carries [Fact(Skip = "F-30: ...")]; disposition (tick-trim vs opt-in marathon
   excluded from the default sweep) is architect-owned. Recorded, not absorbed.

2. Quarantined -- do not run (extreme-bus-load runtime-stress artifact, F-31). Four SchedulerExtremeTests
   bus ceiling-probes: S3 (5M events / 3 tiers), S4 (12.8M events x 64 producer threads), S5a/S5b
   (~1.6M latency samples). Co-resident with SchedulerStressTests in one testhost they cumulatively
   over-stress the .NET runtime (thread-pool / GC) into a managed-heap corruption that crashes the test
   host -- root-caused at the F-29 cascade as NOT a project memory-safety bug (native bus/scheduler paths
   and the managed-output-buffer interops audit clean; an AddressSanitizer-instrumented native DLL finds
   no per-op overrun; it is a cumulative-load threshold, each probe passing in isolation and alongside
   the lighter bus tests). Carry [Fact(Skip = "F-31: ...")]; re-tuning (down-scale, or a process-isolated
   harness) is architect-owned. Recorded, not absorbed.

RESOLVED at the F-29 cascade (both were recorded here at 2.1.0). (a) The F-29(b) scale non-completers S1
(50k x 3k) and S7 (250k) now complete and are un-quarantined -- the O(N^2) graph rebuild is O(N+E)
index-keyed (system_graph.cpp); the "native mutex above ~90k" hypothesis was REFUTED (there is no lock
on the compute/tick path; the wall was pure compute). (b) The F-29(a) load-sensitive crash was a data
race across the lock-free scheduler-graph and wake-registry process-global singletons (NOT a "bus
TickBegin path"), closed by the shared-native-singleton xUnit collection (§2.8, serialising the
singleton-touching test classes) plus a native fail-loud concurrency detector on those two singletons;
SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_... passes.

The now-resolved F-10 members are in git history and the F-ledger (F-10 CLOSED): the three
GameBootstrapIntegrationTests.CreateLoop_RunningLoop_* tests were flaky under xUnit intra-suite
parallelism and are fixed (serial GameLoopSerial collection + poll-until-condition, replacing fixed
Thread.Sleep windows); the shutdown "wedge" was root-caused as a stdout-pipe deadlock -- an invocation
hazard, not a test hang -- and is codified in §8.

### §2.7 — Grand total at survey

**1036 tests reported / 1034 passing** across the nine §2.1 projects (2026-06-11, Release, per-project invocations). This number is a survey snapshot, not a pin: test counts move with every cascade and are tracked per-brief via the §6.1 count delta.

### §2.8 — Shared-native-singleton test isolation

The native kernel exposes several process-global singletons — the system-scheduler graph, the wake
registry, the scheduling-policies registry, the event-type registry, and the native event bus. Their
design contract is single-threaded registration and compute; concurrent reads of already-computed,
immutable state are safe, but concurrent mutation or compute is undefined behaviour. Any test class that
mutates one of these singletons (including calling its Clear / reset for isolation) MUST join the single
shared xUnit collection reserved for them (`SharedNativeSingletonCollection`, name `"SharedNativeSingleton"`),
so that no two such classes ever run in parallel. An xUnit `[Collection]` serialises only its own members;
placing two singleton-touching classes in different collections lets them run concurrently and corrupt
shared native state — this was the F-29(a) race.

The law is enforced structurally on two on-disk fronts:

1. **The shared collection** prevents the parallel schedule. It additionally carries
   `DisableParallelization = true` so the collection — the project's heaviest suites — runs isolated from
   every other collection (the `GameLoopSerial` precedent), which also removes the thread-pool contention
   that otherwise failed the scheduler fan-out assertions.
2. **A native fail-loud concurrency detector** on the two genuinely lock-free singletons — the scheduler
   graph and the wake registry — returns a distinct violation code
   (`SystemGraphInterop.ComputeResult.ConcurrencyViolation`) rather than corrupting memory if concurrent
   entry ever occurs, so an incomplete collection surfaces as a loud, localised failure naming the
   offending path, not a silent heap corruption.

The native event bus is already internally synchronised (per-tier mutexes) and is designed for concurrent
multi-producer publish, so it is not detector-guarded (an acquire-or-fail detector there would reject
legitimate publishers); its cross-test disruption was a semantic issue fully closed by the shared
collection. The scheduling-policies and event-type registries are covered by the collection requirement;
extending the native detector to them is a candidate future hardening (the K-L20 analyzer-rule family —
see docs/ROADMAP.md).

## §3 — Layer taxonomy (DF-adapted)

Eight layers. Layer assignment is structural, not stylistic: the layer determines isolation requirements, invocation route, and which closure gate exercises the test. Every test a brief names carries a layer assignment from this list (§6.1).

### §3.1 — Managed unit

Pure managed logic in isolation: ECS value types, bus delivery semantics, dependency-graph math, spatial math, serialization. No native handles, no ALC loading, no disk beyond fixture input. Exemplars: `tests/DualFrontier.Core.Tests/ECS/EntityIdTests.cs`, `.../Math/SpatialGridTests.cs`. The default layer — anything that can be a unit test must be.

Forbidden in managed unit: P/Invoke or native handle acquisition (that is §3.2), `AssemblyLoadContext` loading (that is §3.4), booting the production composition (that is §3.3), filesystem writes. Reading a fixture file as parser input is permitted — the file is input, not I/O under test. A test whose setup needs any forbidden item is not a unit test and moves layer; relabeling instead of restructuring is the defect this rule exists to catch.

### §3.2 — Interop boundary

Tests that exercise the P/Invoke surface against the real native DLL: marshalling shape, handle lifetime, error-code propagation, span protocol. The suite is `DualFrontier.Core.Interop.Tests` (199 attribute sites across 26 files) — the canonical pattern for any new `[DllImport]` surface. A new interop entry point without a boundary test in this suite is a review defect.

### §3.3 — Integration

Several real components composed through the real bus/bootstrap, no mocks of project-owned surfaces. Exemplar: `GameBootstrapIntegrationTests` (`tests/DualFrontier.Modding.Tests/Bootstrap/`) — boots the real `GameBootstrap` composition and asserts cross-component behavior. Integration tests carry `[Trait("Category", "Integration")]` (§3.7 lists the measured trait census). The composition under test must be the production composition; a test passing against a wiring that production does not use verifies nothing.

### §3.4 — Modding-ALC isolation

The project's strongest suite: `DualFrontier.Modding.Tests` (382 attribute sites across 45 files) plus the 19-fixture projection of §2.3. Verifies that `AssemblyLoadContext` boundaries actually hold: load/unload lifecycles, capability enforcement, dependency-graph resolution, hot reload, manifest validation, shared-ALC semantics. Internal harness: `tests/DualFrontier.Modding.Tests/Fixtures/SchedulerTestFixture.cs` (an internal static helper — note for history: the public builder-style `SchedulerFixture` API described by v1.0 of this document never existed). New mod-facing surface gets its isolation tests here, against real fixture mods, never against in-assembly stand-ins.

### §3.5 — Analyzer tests

**The convention is standing law; its population landed at A'.9.1 Phase β (the 54-test suite — §2.1's one-Fact placeholder row is the dated 2026-06-11 survey record).**

Convention: every active analyzer rule (`DFK###`/`DFL###`/`DF999`, specified in `docs/architecture/ANALYZER_RULES.md`) receives, at the cascade that lands its detection logic, a per-rule verifier class in `DualFrontier.Analyzers.Tests` built on the `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit` harness, with at minimum:

- one **positive case** — a minimal code sample that MUST trigger the diagnostic, asserting ID, severity, and location;
- one **negative case** — the closest compliant sample that MUST NOT trigger it.

Rules with sub-cases (the underscore descriptor variants `DFK003_1`, `DFL025_A` — the dotted/hyphen forms were superseded by the Phase β descriptor-ID adjudication) get positive+negative pairs per sub-case. A Phase β commit that lands detection logic without the paired verifier tests is incomplete by definition of this section.

Convention shape (realized at Phase β across the 17-rule suite; illustrative sample):

```csharp
// tests/DualFrontier.Analyzers.Tests/Rules/Architecture/Dfk003StorageOwnershipVerifierTests.cs
// Harness: Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit 1.1.2 (§1.3).
public sealed class Dfk003StorageOwnershipVerifierTests
{
    [Fact]
    public async Task Positive_MinimalViolatingSample_ReportsDfk003()
    {
        // Minimal source that MUST trigger DFK003; asserts diagnostic ID,
        // severity, and span via the harness's expected-diagnostic markup.
    }

    [Fact]
    public async Task Negative_ClosestCompliantSample_ReportsNothing()
    {
        // The nearest-compliant variant of the same source; zero diagnostics.
    }
}
```

Current truth: all 17 rules under `tools/DualFrontier.Analyzers/Rules/` carry detection logic (§5.2), enforced at their shipped severities since Phase γ, and `DualFrontier.Analyzers.Tests` carries the 54-test Phase β suite — per-rule positive/negative verifiers + harness + census meta-tests. The verifier classes are severity-transparent: expected severities derive from the descriptors, so a severity promotion is test-neutral by construction.

### §3.6 — Native selftest

`DF_CHECK` scenario functions in `native/DualFrontier.Core.Native/test/selftest.cpp`, compiled into the standalone `df_native_selftest.exe` (§2.5). Native kernel invariants (storage ownership, pipeline state machine, mod-unload quiescence, etc.) are tested here, at the layer that owns them. A native-touching cascade adds its scenarios here and runs the exe in both configurations at closure (METHODOLOGY §12.7 step 1).

### §3.7 — Stress and extreme

Heavy tests are partitioned by xUnit traits so default runs stay fast and closure runs stay complete. Measured trait census (2026-06-11):

| Trait value | Sites | Files |
|---|---:|---|
| `Stress` | 4 | `SchedulerStressTests.cs` (×2), `ModDependencyGraphStressTests.cs` (×2) |
| `Extreme` | 1 | `SchedulerExtremeTests.cs` |
| `Integration` | 6 | `GameBootstrapIntegrationTests.cs` (×3), `M74BuildPipelineTests.cs` (×3) |

These three are the only `[Trait("Category", …)]` values in the repository. Inclusion/exclusion is via `--filter` (§8). New heavy tests MUST carry `Stress` or `Extreme`; untagged heavy tests that slow the default sweep are review defects. New trait values are introduced only by amending this table (§9.1).

### §3.8 -- Meta (repo-discipline)

Tests whose subject is the repository itself: census pins, marker-family counts, waiver counts,
structural invariants (§4), and deployment shape. They read the source tree or the build output and
assert shape, not behavior.

Current state (2026-07-02): realized. CensusMetaTests (tests/DualFrontier.Analyzers.Tests/CensusMetaTests.cs,
landed at A'.9.1 Phase beta) asserts the §4 census pins -- reserved-surface (§4.1), the five marker
families (§4.2), and the DFK-WAIVER count (§4.3) -- from inside the compiled suite at every run; the
registered rg expressions remain the closure-audit cross-check (§7). The fixture-deployment guard
(FixtureDeploymentTests, tests/DualFrontier.Modding.Tests/Sharing/, F-10 cascade) asserts that every
fixture the Modding suite consumes is staged under Fixtures/ -- a fail-fast structural check that turns
a missing-fixture deployment gap into one clear failure instead of many cryptic assembly-load errors
(§2.3). (Supersedes the dated "NONE exist" record -- the pre-beta survey state, closed as F-28(a).)

## §4 — Meta-test and census patterns

The core graft of v2.0.0. Each census below is a CONTRACT: an exact pin, a registered measuring expression, and a same-commit update obligation. Censuses keep the repository's declared debt honest — the point is not to prevent reserved surface, but to make its every change deliberate and recorded.

### §4.1 — Reserved-surface census pin

**Composition rule (canonical, quoted by briefs verbatim):** The reserved-surface census counts `[ReservedStub` attribute application sites in `src/**/*.cs` (rg --type cs), excluding the attribute definition file `src/DualFrontier.Contracts/Analyzer/ReservedStubAttribute.cs`, matching rg pattern `\[ReservedStub`; current pin: **34 application sites across 13 files**.

(Measured 2026-06-11; the definition file contributes 0 matches to the pattern, so the exclusion is currently vacuous but remains part of the rule.)

**The meta-test contract:**

1. A census meta-test asserts the **EXACT** pin — `34/13` today — not a bound.
2. Any commit that changes the count updates the pin **in the same commit**, with a `Skeleton revisions` / census-delta record in the commit body, per `RESERVED_SURFACE_MUTABILITY.md` and `CODING_STANDARDS.md §8`.
3. **Monotonicity is NOT asserted.** Reserved stubs close per-feature as consumers materialize (Lesson #25), and reserved surface may legitimately grow when a cascade ships new structural-ahead-of-implementation surface (Lesson #N12). EXACTNESS, not direction, is the invariant — a census that only forbids growth would incentivize hiding stubs; a census that pins exactly forces every delta through a recorded decision.

Contract shape of the compiled meta-test (realized at Phase β — `CensusMetaTests.ReservedSurfaceCensus_MatchesExactPin`; §3.8):

```csharp
[Fact]
public void ReservedSurfaceCensus_MatchesExactPin()
{
    // Count [ReservedStub application sites under src/**/*.cs per the §4.1
    // composition rule (definition file excluded).
    // Assert sites == 34 AND files == 13 — EXACT equality, not a bound.
    // Failure message instructs: update the pin AND record the census delta
    // (Skeleton revisions) in the same commit — per RESERVED_SURFACE_MUTABILITY.md.
}
```

**Implementation state (truth):** the compiled meta-test (`CensusMetaTests`, landed at Phase β) asserts the pin from inside the suite at every run; the rg expression above (registered in `CODING_STANDARDS.md §5`) remains the closure-audit cross-check (§7).

### §4.2 — Marker-family censuses

One census per doc-tag family registered in `CODING_STANDARDS.md §5.2`. Patterns verbatim; baselines registered 2026-06-11, `stub`/`deferred` refreshed 2026-07-01 (the F-25 owed fold — drift from the 2026-06-12 comment-citation pass; the compiled meta-tests have carried the live values since Phase β):

| Family | Census expression | Baseline (matches / files) |
|---|---|---|
| `stub` | `rg --count-matches -i '\bstub\b' src/ --type cs` | 51 / 20 |
| `deferred` | `rg --count-matches -i '\bdeferred\b' src/ --type cs` | 82 / 51 |
| `TODO` (case-sensitive) | `rg --count-matches '\bTODO\b' src/ --type cs` | 136 / 53 |
| `Phase 6` (literal) | `rg --count-matches 'Phase 6' src/ --type cs` | 23 / 11 |
| `not yet` | `rg --count-matches -i 'not yet' src/ --type cs` | 8 / 7 |

Same exactness-pin contract as §4.1: a commit that moves a family's count records the delta in its commit body (`CODING_STANDARDS.md §8`); the closure audit re-measures all five (§7). The baselines impose **no retroactive cleanup obligation** — existing sites are baseline-registered and normalized opportunistically; the census exists so that drift is visible, not to manufacture work. Implementation state: compiled meta-tests assert all five families since Phase β; the rg expressions remain the closure-audit cross-check (§7).

### §4.3 — DFK-WAIVER census

**Baseline: 0** (2026-06-11). **Current pin: 2** since the A'.9.1 Phase β triage — the two DFK001 waivers in `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` (К-L19-sanctioned Vulkan debug-messenger interop), asserted by `CensusMetaTests.DfkWaiverCensus_MatchesPin`. The 2026-06-11 verification record:

- `rg '#pragma warning disable (DFK|DFL|DF9)'` over `.cs` → 0 matches;
- `[SuppressMessage]` over `src/` and `tests/` → 0 matches;
- no `GlobalSuppressions.cs` exists anywhere in the repository.

(Since Phase β the `tests/` tree contains `SuppressMessage` as string literals inside analyzer-test fixtures; attribute usage in `src/` remains 0 and no `GlobalSuppressions.cs` exists — the compiled meta-tests assert both.)

Every increase requires the full waiver form and authority citation of `CODING_STANDARDS.md §5.3` (waiver comment with a resolvable authority — Q-L-#, К-L#, F-#, or refinement reference — immediately preceding a minimally-scoped disable/restore pair). The census runs at every cascade closure (§7); a waiver appearing without its §5.3 citation fails the closure audit. Phase β surfaced real diagnostics and moved the census exactly as designed — 0 → 2, both waivers carrying their §5.3 citations (the recorded-decision discipline held).

### §4.4 — S-LOCK → verification obligation

Every S-LOCK declared by an active brief names its **verifying artifact**: a test, a grep-gate (registered expression), or a build property. An S-LOCK whose verifying artifact does not exist on disk is a gap; gaps become F-ledger entries (`docs/ROADMAP.md §Findings ledger (F-series)`) at the closure that detects them — never silent debt.

**2026-06-11 audit result (recorded):** the A'.9.1 S-LOCK set was audited against disk. All artifact-checkable locks verified: S-LOCK-4 — 17 rule files present under `tools/DualFrontier.Analyzers/Rules/`; S-LOCK-10 — no DFK010 file exists; S-LOCK-6/7/8/9/11/12 — required absences/presences confirmed on disk. No hard gaps. The `DualFrontier.Analyzers.Tests` gate is placeholder-level pending Phase β — **by design** (Phase α scoped it to assembly identity; §3.5 governs its population), so it is recorded here as known state, not as a gap.

## §5 — Validation-criteria mapping

### §5.1 — One criterion → one named test

Every testable validation criterion a brief declares maps to **one named test** (a `Fact`/`Theory` whose name appears in the brief's §6.1 test list, or a named native selftest scenario). The mapping is mechanical: the closure audit can walk criterion → test name → green run. A criterion mapped to «covered by the suite generally» is not mapped.

Worked examples from the surveyed tree (real artifacts, both verified on disk 2026-06-11):

| Criterion | Named test | Layer (§3) |
|---|---|---|
| К-L18 — mod unload refused while pipeline holds a dispatched slot | `scenario_mod_unload_fails_when_pipeline_has_dispatched_slot` (`native/DualFrontier.Core.Native/test/selftest.cpp`) | §3.6 native selftest |
| К-L18 — unload succeeds once pipeline is quiescent after tail transition | `scenario_mod_unload_succeeds_when_pipeline_quiescent_after_tail_transition` (same file) | §3.6 native selftest |
| Bootstrap pawn naming reaches the render command stream | `GameBootstrapIntegrationTests.CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` | §3.3 integration — currently an F-ledger known failure (§2.6); the mapping stands, the criterion is honestly OPEN |

### §5.2 — К-L invariants: machine-checkable vs architect-audited

К-Lxx invariants split by verification mode, and the split is stated explicitly — no fake coverage claims:

- **Machine-checkable (live since A'.9.1 Phase β/γ):** invariants with a specified analyzer rule in `docs/architecture/ANALYZER_RULES.md` §4.1 (rule registry). Current truth: the 17 rules detect (Phase β) and are **enforced at their shipped severities** (Phase γ, Release 1.0) — 11 Error + 5 Warning fail the build under `TreatWarningsAsErrors`; DFL025_B is IDE-only (descriptor Info / `.editorconfig` `suggestion`). `AnalyzerReleases.Shipped.md` records Release 1.0; the root `.editorconfig` restates the same severities; per-rule positive/negative verifier tests run in `tests/DualFrontier.Analyzers.Tests` (§3.5, severity-transparent). Meta-test census contracts (§4) are the second machine-checkable channel — the compiled census meta-tests assert the pins from inside the suite.
- **Architect-audited (now):** invariants not machine-checkable are verified by explicit architect audit with recorded evidence. The exemplar is `docs/architecture/K_L14_EVIDENCE_DASHBOARD.md` — К-L14 holds because each verification event is individually recorded with its outcome (including honest soft-halts), not because a test claims it. A document that says an invariant «is enforced» must name either the artifact (rule + verifier test, meta-test, build property) or the audit record; otherwise the claim is removed.

### §5.3 — The lying-stub law (Lesson #25 / Lesson #N12)

**Never test lying stubs.** A `[ReservedStub]` surface has no real behavior; a test that exercises it and passes is a lie that inflates coverage and masks the stub's eventual realization. Per METHODOLOGY.md Provisional Lessons #25 (implementation depth follows consumer materialization) and #N12 (defensive reserved-stub pattern, sub-patterns A/B):

1. **Default: do not test.** A reserved stub with no observable behavior carries the honest DO-NOT-TEST documentation pattern at the stub site («DO NOT TEST — stub has no observable behavior; tests would lie by passing trivially»), and no test touches it. Sub-pattern A stubs (test-only invocation paths, defensive `NotImplementedException` throw) need no passing test either — the throw is the guard.
2. **When a test class must touch reserved surface** (e.g. a composition test that transits a stub), the touching test carries `[Trait("Category", "ReservedStub")]` so the contact is declared and filterable. This is the **DFL025_A convention**; its analyzer detection (`DFL025_A`) landed at A'.9.1 Phase β and binds at Warning — build-breaking under `TreatWarningsAsErrors` — since Phase γ (Release 1.0). The trait census is part of the §7 audit when such tests first appear. Current truth (re-verified 2026-07-02): zero tests carry this trait — the sole textual occurrence is fixture source inside the DFL025_A verifier tests, not a live trait.
3. When a stub gains real behavior, its realization commit brings the real tests and drops the trait — the census delta (§4.1) and the test-count delta (§6.1) record the closure together.

## §6 — Brief integration pattern

The mechanism by which briefs consume this law without duplicating it.

### §6.1 — What a brief CARRIES

For each commit that adds or modifies tests, the owning brief specifies:

1. **Test files created/modified** — explicit paths.
2. **Named test list** — every test as the `Fact`/`Theory` method name it will bear, one line each (native scenarios likewise by function name).
3. **Layer assignment per §3** — explicit, e.g. «4 interop boundary + 2 modding-ALC + 1 native selftest scenario». A test whose name implies one layer but is listed under another is a halt-grade brief defect.
4. **Coverage anchors** — which validation criteria, К-L invariants, S-LOCKs, or F-ledger entries the commit's tests cover (§5.1 mapping, stated per test or per group).
5. **1–2 representative test bodies** — full code for the trickiest case(s) only; the executor expands the rest from the named list using this document's layer patterns.
6. **Count delta** — «+N tests this commit; suite total before → after», making the closure count audit mechanical.

Mandatory carry format (per-commit, inside the brief's test section):

````markdown
**Test plan** (`<test file path>` — <§3 layer>, N tests):

1. <MethodName_Scenario_ExpectedOutcome>
2. <MethodName_Scenario_ExpectedOutcome>
   ... N. <MethodName_Scenario_ExpectedOutcome>

Representative test body — <name of the one critical test>:

```csharp
// full code of the trickiest case only; the executor expands the rest
// from the named list using the §3 layer patterns.
```

**Coverage anchors:** <К-L# / S-LOCK-# / Q-series criterion / F-# per test or group>
**Test count delta:** +N this commit; <project> total <before> → <after>.
````

The format makes test scope auditable per commit and makes the §7 count reconciliation mechanical. A brief whose test section cannot be read in this shape is returned for correction before lock.

### §6.2 — What a brief CITES (never restates)

- Layer definitions and isolation requirements — `§3`.
- Fixture/ALC harness construction and the cross-ALC projection pattern — `§2.3`, `§3.4`.
- The analyzer-test convention — `§3.5`.
- Census contracts and pin-update obligations — `§4` (and `CODING_STANDARDS.md §5` for the expressions).
- Waiver discipline — `§4.3` / `CODING_STANDARDS.md §5.3`.
- Invocation commands and filters — `§8`.
- Closure obligations — `§7` / `METHODOLOGY.md §12.7`.

Citation is by section anchor (`TESTING_STRATEGY.md §4.1`), so renumber-stable wording («the reserved-surface census pin») accompanies the anchor.

### §6.3 — Boundary rule

**Reusable-across-cascades → this document. Cascade-specific → the brief. Doubt → refactor into this document.** A pattern that two briefs would both need is law and belongs here before the second brief is authored; patterns spreading across briefs without codification here is the failure mode this section exists to prevent.

### §6.4 — Anti-pattern rule

A brief that contradicts this document is **wrong by default**. Resolution: either the brief is corrected to comply, or this document is amended per §9.1 — **before the brief locks**. A locked brief executing a contradiction is a halt condition, not a precedent. `METHODOLOGY.md §12.8` carries the cross-document form of this rule (brief-vs-standing-law boundary); this section is its test-law instantiation.

## §7 — Closure-audit obligation

Every cascade closure includes, in its closure report, in addition to — extending, not replacing — the canonical `METHODOLOGY.md §12.7` protocol:

1. **Census table** — all §4 pins re-measured by their registered expressions: reserved-surface pin (§4.1), the five marker families (§4.2), waiver count (§4.3). Each row: expression, expected pin, measured value, delta disposition (unchanged | updated-in-commit-X with census-delta record).
2. **S-LOCK coverage check** (§4.4) — every S-LOCK active in the cascade's brief mapped to its verifying artifact, gaps filed as F-ledger entries.
3. **Waiver audit** (§4.3) — count plus, for any non-zero delta, the per-waiver authority citations.
4. **Test-suite runs** — per `METHODOLOGY.md §12.7` step 1 (cited, not restated here: per-suite Release runs, native selftest, the Modding-suite mandatory-run rule). Known-failing F-ledger tests are re-verified and reported per §2.6's honest-statement form.
5. **Count reconciliation** — the brief's cumulative §6.1 count deltas against the measured suite totals; unexplained drift is a closure defect.

## §8 — Test invocation truth

Commands verified working as of 2026-06-11. These exact strings are the law of invocation; briefs cite, sessions copy-paste.

**Full solution:**

```
dotnet test DualFrontier.sln -c Release --logger "console;verbosity=minimal"
```

Testhost file-lock caveat: a stale `testhost.exe` (left by an IDE test runner or a concurrent run) holds `DualFrontier.Modding.Tests` bin files and produces MSB3026/MSB3027 retry warnings or copy failures — close other runners before solution-wide runs.

Invocation safety -- never pipe dotnet test (the shutdown-pipe-deadlock law).

Under load a testhost can linger in shutdown holding the write end of an inherited stdout pipe; a
`dotnet test ... | <consumer>` pipeline then blocks forever on an EOF that never arrives (observed as
a ~50-minute shell wedge on an all-passing suite -- no crash or hang required, F-10 recon 2026-07-02).
The durable result is the TRX, written independently of stdout: on a killed run the pass/fail verdict
is in the TRX, not the lost piped console.

Standing rule: never pipe dotnet test into a shell consumer (| Add-Content, | Tee-Object, | Out-File
via pipe). Run it under a process launcher with file redirection and a watchdog that snapshots and
kills lingering testhosts on timeout, and read results from --logger "trx". Verified-working harness
shape (PowerShell 5.1):

    $p = Start-Process dotnet -ArgumentList @('test', <proj>, '-c', <cfg>, '--no-build',
             '--logger', 'trx;LogFileName=<label>.trx', '--results-directory', <dir>) `
             -RedirectStandardOutput <out.log> -RedirectStandardError <err.log> `
             -NoNewWindow -PassThru
    if (-not $p.WaitForExit(<timeoutMs>)) {
        # timeout: kill the tree, then sweep orphans
        Stop-Process -Id $p.Id -Force
        Get-Process testhost*, vstest* -ErrorAction SilentlyContinue | Stop-Process -Force
    }
    # verdict comes from <dir>\<label>.trx, not from the console

This is invocation law: briefs cite §8, sessions copy the harness. It composes with the file-lock
caveat above (close other runners before solution-wide runs).

**Per-suite** (the closure-protocol route, per `METHODOLOGY.md §12.7` step 1):

```
dotnet test tests/DualFrontier.Core.Tests/ -c Release
dotnet test tests/DualFrontier.Modding.Tests/ -c Release
# ... one invocation per §2.1 project as the closure scope requires
```

Heavy-test exclusion (fast default sweep) and inclusion:

    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category!=Stress&Category!=Extreme"
    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category=Stress"
    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category=Extreme"

The fast default sweep excludes both Stress and Extreme (both are heavy). Five Extreme scenarios are
additionally Skip-guarded (§2.6) -- S2 under F-30 (managed marathon) and the S3/S4/S5 bus ceiling-probes
under F-31 (runtime-stress artifact) -- so even an explicit Category=Extreme run completes (6 pass /
5 skipped) rather than hanging or crashing the host. Trait values available for filtering are exactly
those of §3.7: Stress, Extreme, Integration.

**Native selftest** (standalone exe, not dotnet test):

```
native/DualFrontier.Core.Native/build/Release/df_native_selftest.exe
native/DualFrontier.Core.Native/build/Debug/df_native_selftest.exe
```

Success criterion: prints `ALL PASSED`, exit code 0 (§2.5).

**Benchmarks** (not a test run; §2.2): `dotnet run -c Release --project tests/DualFrontier.Core.Benchmarks`.

## §9 — Amendment protocol and change history

### §9.1 — Amendment protocol

Same shape as `CODING_STANDARDS.md §10`. Every amendment states:

1. **Surface** — which §/pin/table changes (census pin updates under their same-commit contract of §4 are the routine case; taxonomy or contract changes are the deliberative case).
2. **Rationale** — the driving decision, citing its authority (Q-series, К-L, F-entry, lesson, brief).
3. **Semver** — PATCH (pin/baseline refresh, cross-reference fix), MINOR (new layer, new census family, new convention — additive), MAJOR (contract change: pin semantics, boundary rule, closure obligations).
4. **REGISTER update** — `docs/governance/REGISTER.yaml` version bump + audit-trail event; frontmatter here follows by sync.
5. **Propagation** — citing documents checked and updated. Known citers to check on amendment: active briefs (§6 consumers), `CODING_STANDARDS.md` (§5 census expression registry), `RESERVED_SURFACE_MUTABILITY.md` (census-delta law), `METHODOLOGY.md` (§12.7/§12.8 interlock), `ANALYZER_RULES.md` (§3.5 convention consumer), `docs/ROADMAP.md` (Analyzer track + F-ledger).

Census pins (§4.1–§4.3 values) are mutable surface under `RESERVED_SURFACE_MUTABILITY.md`: any cascade commit may update them with the same-commit census-delta record, PATCH-level, no separate ratification. Everything else in §4's contracts is immutable-or-adjudicate.

### §9.2 — Change history

**v2.2.0 -- 2026-07-04 -- F-29 native-scheduler cascade (MINOR).** Per tools/briefs/F29_NATIVE_SCHEDULER_BRIEF.md.
New shared-native-singleton test-isolation law (§2.8): singleton-touching classes join one shared xUnit
collection (DisableParallelization = true), enforced by a native fail-loud concurrency detector on the
lock-free scheduler graph + wake registry (the bus is mutex-synchronised, not detector-guarded). Honesty
register (§2.6) refreshed to the post-F-29 state: S1/S7 un-quarantined (F-29(b) O(N^2)->O(N+E) index-keyed
rebuild resolved), S2 re-pointed to F-30 (managed ParallelSystemScheduler marathon), the refuted "native
mutex above ~90k" hypothesis removed, F-29(a) closed (a scheduler + wake singleton race, not a bus
TickBegin path), and F-31 seeded (extreme-bus-load runtime-stress artifact -- S3/S4/S5 quarantined). §8
Category=Extreme count corrected (8 pass / 3 skipped -> 6 pass / 5 skipped). Additive law + honesty
refresh; no taxonomy, contract, or pin-semantics change.

**v2.1.0 -- 2026-07-02 -- F-10 isolation cascade (MINOR).** Per tools/briefs/F10_TEST_ISOLATION_BRIEF.md.
Added the invocation-safety (no-pipe / TRX-is-truth) law to §8 -- the stdout-pipe shutdown deadlock the
F-10 recon root-caused (a ~50-minute wedge on an all-passing suite; the "zombie testhost wedge" was an
invocation hazard, not a test hang) -- and the verified Start-Process + file-redirect + watchdog harness
(new invocation convention -> MINOR). §8 fast default sweep corrected to exclude both Stress and Extreme
(was Stress only; it would otherwise run the heavy Extreme suite, including the scale non-completers). §2.6
honesty register refreshed to the post-F-10 state: the RunningLoop family fixed (serial collection +
poll-until-condition), the Stress load-crash reclassified under F-29(a), the Extreme scale non-completers
S1/S2/S7 Skip-quarantined under F-29(b). §3.8 corrected from "NONE exist" to the realized meta layer
(CensusMetaTests since Phase beta + the new FixtureDeploymentTests) -- closes F-28(a). No taxonomy,
contract, or pin-semantics change.

**v2.0.2 — 2026-07-02 — A'.9.1 Phase δ rider (F-27(b) PATCH).** §5.3 item 2's stale forward-claim («analyzer detection is Phase β scope — today the convention binds by review») replaced with the realized tail: DFL025_A detection landed at Phase β and binds at Warning (build-breaking under `TreatWarningsAsErrors`) since Phase γ Release 1.0; rule-ID tokens underscore-normalized on the touched sentence; the zero-trait census re-verified 2026-07-02 (sole textual occurrence = DFL025_A verifier fixture source). The §3.8-consistency parenthetical dropped — §3.8's own «NONE exist» claim is superseded by the Phase β census meta-tests and is ledgered separately at this cascade's F-sweep (ROADMAP Findings ledger). No taxonomy, contract, or pin-semantics change.

**v2.0.1 — 2026-07-01 — A'.9.1 Phase γ propagation PATCH.** METHODOLOGY §12.7 step 9 (SYNTH-2) + the F-25 owed fold, per §9.1 PATCH class (pin/baseline refresh + cross-reference truth): §5.2 machine-checkable channel «non-detecting stubs / zero enforcement» → live Release 1.0 enforcement (11 Error + 5 Warning build-breaking under `TreatWarningsAsErrors`, DFL025_B IDE-only; `Shipped.md` + `.editorconfig` agreeing severity surfaces); §4.1/§4.2 «Planned — Phase β» compiled-meta-test wording → realized (`CensusMetaTests`, Phase β); §3.5 analyzer-test layer «placeholder / no such test exists» state claims → the realized 54-test suite (severity-transparent verifiers), sub-case ID examples → underscore forms per the Phase β adjudication; §4.2 `stub`/`deferred` baselines 48/18 → 51/20, 79/48 → 82/51 (F-25 — the ledger's 81/51 was the 2026-06-12 measure, 82/51 is the Phase β meta-test value); §4.3 waiver census current pin 2 (0-baseline kept as dated history; `SuppressMessage` string-literal precision note). No taxonomy, contract, or pin-semantics change.

**v2.0.0 — 2026-06-11 — Full rewrite to standing test law (MAJOR).** Per `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md` (W2 deliverable). Replaced the v1.0 aspirational text with code-truth: the nine-project landscape with measured counts and verdicts (§2), the placeholder suite named as such, the known-failing-test honesty register (§2.6), the eight-layer DF taxonomy (§3), census pin contracts with 2026-06-11 baselines — reserved-surface 34/13, marker families 48/18 · 79/48 · 136/53 · 23/11 · 8/7, waivers 0 (§4), the S-LOCK verification obligation with the recorded A'.9.1 audit (§4.4), the validation-mapping and lying-stub law (§5), the brief-integration boundary (§6), the closure-audit extension of METHODOLOGY §12.7 (§7), and verified invocation commands (§8). Removed v1.0 phantom surfaces that never existed on disk: the builder-style `SchedulerFixture` API, `EvilMod`/`GoodMod` fixtures, `PerformanceGates.cs`, CI gates, and the four-project test layout (nine exist). Tier 1 LOCKED; owner Crystalka.

**v1.0 — 2026-05-12 — Initial authoring (historical).** Aspirational test strategy: xUnit + FluentAssertions stack statement, intended unit/integration/isolation/modding layers, intended CI gating. Superseded in full by v2.0.0; retained in git history.
