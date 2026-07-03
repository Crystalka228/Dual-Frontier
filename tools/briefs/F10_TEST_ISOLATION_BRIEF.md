---
register_id: DOC-D-F10_TEST_ISOLATION_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: LOCKED (ratified by Crystalka 2026-07-03; -> EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-02'
content_language: en
authored_by: Claude Opus (deliberation session, F10_TEST_ISOLATION prep)
basis: F10_TEST_ISOLATION RECON REPORT 2026-07-02 (R1-R9); architect-verified against live repo
---

# F10_TEST_ISOLATION -- Execution Brief

This cascade resolves finding **F-10** by isolating and dispositioning every object the recon
proved lies behind it, and codifies the newly root-caused invocation hazard as standing law. It
is FIX + RECLASSIFY/QUARANTINE + CODIFY, and it SEEDS the two native-scheduler product defects the
recon surfaced (it does not fix them here). Concretely: it makes the `RunningLoop` integration
family deterministic (closing F-10 #2); adds the 13 disk-only `Fixture.RegularMod_*` projects to
`DualFrontier.sln` so a solution Release build stages all fixtures, and installs a fail-fast
fixture-deployment guard; Skip-quarantines the three Extreme scale non-completers (S1/S2/S7);
reclassifies the Stress load-crash under a serial isolation pass; and amends `TESTING_STRATEGY.md`
with the invocation-safety (no-pipe) protocol, the corrected fast-sweep filter, the refreshed
honesty register, and the F-28(a) meta-test correction. It touches only `tests/`, `docs/`, and
`DualFrontier.sln` -- **no `src/` (production) code changes**.

**Done** = F-10 CLOSED in the ROADMAP F-ledger; the `RunningLoop` family green across >=20
full-Modding runs; a solution Release build stages 18 fixtures and Modding runs 399/399 under
`-c Release --no-build`; S1/S2/S7 report as Skipped (never hang); `TESTING_STRATEGY.md` at 2.1.0
with the four amendments; F-29 seeded, F-28(a) folded; the analyzer + census suites unchanged
green; REGISTER at 2.22 with validate exit 0.

**Executor**: Claude Code (flagship model), LOCAL on the operator's machine `SKARLET`.
**Repository**: `D:\Colony_Simulator\Colony_Simulator` (`Crystalka228/Dual-Frontier`).

**Brief-integration notice.** This brief CITES standing law by anchor and does not restate it:
test layers / census contracts / invocation law -> `TESTING_STRATEGY.md` (the very document this
cascade amends -- edit the sections named in §8, cite the rest); commit-body structure, marker
registry, DFK-WAIVER law, citation form -> `CODING_STANDARDS.md`; mutability license and
`Skeleton revisions` form -> `RESERVED_SURFACE_MUTABILITY.md`; session closure protocol ->
`METHODOLOGY.md §12.7`. **Anti-pattern rule: a conflict between this brief and any standing doc
means THE BRIEF IS WRONG -- halt (H6/H(gov)) and surface, do not improvise.** Where this brief and
the live code differ, the code wins and the conflict is recorded.

---

## 1. Mission [CORE]

Resolve F-10 to a clean CLOSED state, harden the two test-infra correctness gaps the recon proved,
and lift the invocation hazard into standing law -- on a green analyzer/census baseline, leaving
the two native product defects seeded for their own cascade.

| # | Deliverable | Artifact | Action | Version |
|---|---|---|---|---|
| D1 | RunningLoop determinism (closes F-10 #2) | `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` | edit (poll-until-condition + serial collection) | n/a (test) |
| D2 | Fixture Release-staging fix | `DualFrontier.sln` | add 13 `Fixture.RegularMod_*` projects | n/a (build) |
| D3 | Fixture-deployment fail-fast guard | `tests/DualFrontier.Modding.Tests/Sharing/FixtureDeploymentTests.cs` | create | n/a (test) |
| D4 (rider) | DRY hoist of the fixture-copy target | `tests/Directory.Build.targets` (create) + 19 `Fixture.*.csproj` | precondition-gated refactor | n/a (build) |
| D5 | Extreme scale-non-completer quarantine | `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs` | `[Fact(Skip=..)]` on S1/S2/S7 + S1 name truth-up | n/a (test) |
| D6 | Test-law amendments | `docs/methodology/TESTING_STRATEGY.md` | §2.6 + §3.8 + §8 + §9.2 (exact text §8 below) | 2.0.2 -> 2.1.0 |
| D7 | F-ledger closure | `docs/ROADMAP.md` | F-10 CLOSED, F-28(a) folded, F-29 seeded | n/a |
| D8 | Governance closure | `docs/governance/REGISTER.yaml` (+ render) | enroll brief + report; TESTING_STRATEGY bump; EVT; register bump | 2.21 -> 2.22 |

This cascade precedes the register-tooling cascade and the K-L20 LOCK family (the next candidates):
it removes the "known failure shape" noise from every subsequent closure gate.

## 2. Established facts [CORE]

Measured by the recon (`docs/reports/F10_TEST_ISOLATION_RECON_REPORT.md`, 2026-07-02) and
architect-verified against the live repo by line-read. `[RV]` marks a fact the executor MUST
re-verify at Phase 0, halting on mismatch (H1).

- `[RV]` HEAD = `main` = `origin/main` = `e44c27b`; tree clean; REGISTER 2.21 (`8ec64c5`),
  282 docs / 44 EVT. (Local refs only; no fetch.)
- **Nine dotnet-test projects; 19 `Fixture.*`; benchmarks + smoke are non-test** (recon R1, TESTING_STRATEGY §2.1-§2.3).
- **F-10 has zero deterministic failures.** In per-project isolation the green baseline is
  **1098 / 1101** (recon R2). The three non-verdicts are Extreme S1/S2/S7 (do-not-complete under a
  120-180 s watchdog).
- **D1 object -- RunningLoop family (3 tests, `GameBootstrapIntegrationTests.cs`, all `[Trait("Category","Integration")]`):**
  `CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` (F-10 #2), `...CarriesTopSkills`,
  `...PublishesTickAdvancedCommandsThroughBridge`. Each does `Loop.Start(); Thread.Sleep(500|500|250); Loop.Stop();`
  then asserts commands were published. **5/5 green isolated; ~80% flake under full-Modding parallel load;
  the failing member rotates** (recon R4). Root cause: xUnit parallelizes across test classes in the assembly,
  starving the GameLoop thread inside the fixed sleep window. Verified in code (architect read).
- **D2 object -- fixture Release gap (root-caused, architect-verified end-to-end):** `Modding.Tests.csproj`
  references 18 fixtures with `<ReferenceOutputAssembly>false</ReferenceOutputAssembly>` (5 in-solution
  + 13 `Fixture.RegularMod_*`). Each `Fixture.*.csproj` carries an identical `DeployToTestFixtures`
  target (`AfterTargets="Build"`) copying `$(TargetPath)` + `mod.manifest.json` to
  `..\DualFrontier.Modding.Tests\bin\$(Configuration)\$(TargetFramework)\Fixtures\$(AssemblyName)\`
  (`$(Configuration)` is parametrized, NOT hardcoded). The 13 `Fixture.RegularMod_*` are **NOT members
  of `DualFrontier.sln`** (verified against the `.sln` Project() list). Consequence: a solution-scoped
  `-c Release` build resolves their ProjectReferences to their default (Debug) output, so their
  `DeployToTestFixtures` fires with `$(Configuration)=Debug` and never stages into `bin\Release\...\Fixtures`.
  Measured: solution Release stages **5/18** -> Modding `-c Release --no-build` = **384 / 399 (15 failed:
  14 fixture-load + 1 coincidental flake)**; per-project `-c Release` build stages **18/18** -> **399/399**
  (recon R3/R7). Consumer path: `Sharing/TestModPaths.cs` = `AppContext.BaseDirectory/Fixtures/<name>/`.
- **D5 object -- Extreme scale non-completers (native scale pathology):** `SchedulerExtremeTests`
  (class-level `[Trait("Category","Extreme")]`, `[Collection("ExtremeSerial")]`) -- `S1` (body
  `SystemCount=50_000` x `TickIterations=3_000`, method name says "HundredThousand"/"TwoThousand" --
  a truth-up target), `S2` (200k ticks), `S7` (250k systems) do not complete in 120-180 s
  (compute-bound; the class's own comment hypothesizes O(N^2) register-conflict scan / native mutex
  above ~90k). The other 8 Extreme tests pass (recon R4/R6; architect-verified S1 body + comment).
- **The wedge is a stdout-pipe deadlock, not a test hang (recon R6, accepted-by-report -- the A/B is
  irreproducible from the architect seat but internally coherent and corroborated by the verified statics).**
  Identical Modding suite: file-redirect exits clean at 50.9 s; `| Add-Content` wedges at 150 s with an
  idle testhost (22 CPU-s) holding the inherited pipe. The green result is in the TRX, not the lost
  piped console. This is the invocation-safety law codified in D6/§8.
- **`src/**/*.cs` census pins are OUT of this cascade's edit surface** (all edits are `tests/`, `docs/`,
  `.sln`) -- reserved-surface 34/13, the five marker families, DFK-WAIVER 2 are UNCHANGED and are the
  regression anchor (`CensusMetaTests` must stay green). See §10.

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially by the orchestrator before any edit.

1. **Verify recon facts** (the `[RV]` set in §2). Any mismatch -> HALT H1.
2. **Baseline gates -- record the regression anchor, using the SAFE harness.** Build managed +
   native (per `DEVELOPMENT_HYGIENE`), run each of the nine test projects per-suite. **You MUST run
   tests with the no-pipe harness (Start-Process + file redirection + a WaitForExit watchdog +
   orphan-testhost sweep; the harness the recon used and that D6/§8 codifies) -- NEVER pipe
   `dotnet test` into a shell consumer, or you will reproduce the ~50-minute wedge on yourself.**
   Exclude Extreme from the default sweep. Record the KNOWN pre-existing shape so it is NOT mistaken
   for a halt: RunningLoop flakes ~80% under full-Modding load; Stress passes green in isolation;
   Extreme S1/S2/S7 do-not-complete (do not run them un-Skipped -- run `Category=Extreme&FullyQualifiedName!~S1&...!~S2&...!~S7` or simply skip the Extreme run until D5 lands). Closure must
   match-or-improve -> HALT H2 on any NEW regression (a previously-green test going red).
3. **Branch prep**: none -- work on `main` (single-orchestrator serial cascade). No merge.
4. **Validation checkpoint**:
   `powershell -NoProfile -ExecutionPolicy Bypass -File tools\governance\sync_register.ps1 -Validate`.
   Exit code != 0 -> HALT H3. The refreshed `VALIDATION_REPORT.md` lands inside C1.
5. **REGISTER enum read (Lesson #N14)**: extract from `REGISTER.yaml` the verbatim `category` /
   `tier` / `lifecycle` enum values actually in use and the exact `DOC-` and `EVT-` entry shapes,
   including the shape of an existing `docs/reports/` recon-report enrollment (pattern the report's
   entry on it) and a Category-D Tier-3 brief entry. These verbatim shapes are the ONLY sanctioned
   templates for §12. Needing an enum value the vocabulary lacks -> HALT H5 (never invent).
6. **Mandatory reads (confirm before any edit)**: the recon report (full); `GameBootstrapIntegrationTests.cs`;
   `SchedulerExtremeTests.cs` (S1/S2/S7); `Modding.Tests.csproj` + one `Fixture.RegularMod_*.csproj`
   + `Sharing/TestModPaths.cs`; `DualFrontier.sln` (Project() + config blocks -- the pattern to
   replicate for the 13 additions); `TESTING_STRATEGY.md` §2.1/§2.3/§2.6/§3.4/§3.7/§3.8/§8/§9;
   the ROADMAP F-ledger (F-10, F-28, format); `METHODOLOGY.md §12.7`.

NEVER run `-Sync` outside §12's REGISTER closure. `render_register.ps1` runs exactly once, at the
render commit (Cn). The executor NEVER pushes -- push is the operator's manual step after closure.

## 4. Topology [CORE]

**Single orchestrator, no wave.** The edit set is small (six files + REGISTER + ROADMAP + render),
the changes are interdependent (D2 gates D3's Release-green; D5 gates the safe Extreme run; D6/D7/D8
depend on the code commits' hashes), and there is no survey front that parallel read-only agents
would accelerate. The orchestrator alone runs `git add`/`commit` (atomic discipline). `ROADMAP.md`
and `REGISTER.yaml` are orchestrator-only single-writer files. No reference tree is touched.

## 6. Checkpoints [CORE]

Serial self-audit the orchestrator runs before the closure commits: (a) **truth-law** -- every
enforcement/coverage claim added to `TESTING_STRATEGY.md` names an on-disk artifact (the guard test,
`CensusMetaTests`, the `.editorconfig`/Shipped surfaces already cited) -- no bare "is enforced"
without an enforcer; (b) **citation-form** -- no living-doc version pins, no URL anchors in the new
text; (c) **no standing-law residue** -- the amended sections carry no leftover stale claim; (d)
**census unchanged** -- `CensusMetaTests` green, `src/` pins unmoved (§10). Any failure returns to
the owning edit once; unresolvable without an architectural decision -> HALT H6.

## 7. Execution / writer specifications [CORE]

Global laws by reference: truth law (no enforcement verb without an on-disk enforcer); citation-form
(anchor + stable ID, no version pins, no URL anchors); **code is the truth** -- verify against the
live file before editing; the recon report is the work order, the code is the arbiter. Test additions
follow the `TESTING_STRATEGY.md §6.1` carry format (path, named tests, layer, coverage anchor, one
representative body, count delta).

### 7.1 -- D1: RunningLoop determinism (closes F-10 #2)

The fix is entirely test-side; production code is correct (green in isolation). Two changes to
`GameBootstrapIntegrationTests.cs`:

1. **Replace fixed-sleep windows with poll-until-condition.** For each of the three `RunningLoop`
   tests, replace `Loop.Start(); Thread.Sleep(500|250); Loop.Stop();` with: start the loop; poll on
   a short interval (e.g. 25 ms), draining `bridge.DrainCommands` into an accumulator each poll,
   until the asserted condition is satisfied (`>=1 PawnStateCommand` for the two PawnState tests;
   `>=2 TickAdvancedCommand` for the tick test) OR a generous overall timeout elapses (>= 5 s); then
   `Loop.Stop()` and assert on the accumulator. The generous wall budget makes the test robust to
   CPU starvation without a fixed window. (Because `DrainCommands` drains, poll into an accumulator
   -- do not re-drain-and-discard.)
2. **Serialize the loop tests against the rest of the assembly.** Place the three loop tests in an
   xUnit collection marked `[CollectionDefinition("GameLoopSerial", DisableParallelization = true)]`
   so they do not run concurrently with the CPU-heavy Modding classes. Putting the whole
   `GameBootstrapIntegrationTests` class in the collection is acceptable (simplest); extracting the
   three loop tests into a dedicated class in the collection is also acceptable if you prefer a
   smaller serialization footprint. The synchronous (non-loop) tests are unaffected either way.

**Acceptance (D1):** >= 20 consecutive full-`DualFrontier.Modding.Tests` runs (`-c Debug --no-build`,
via the safe harness) all green, no rotating RunningLoop failure. If the poll change alone clears the
flake at this bar, the collection is belt-and-suspenders; land both regardless (the determinism
guarantee is the point).

**Test plan** (`GameBootstrapIntegrationTests.cs` -- §3.3 integration, 3 modified):
1. `CreateLoop_RunningLoop_PawnStateCommandCarriesRealName` (poll for `>=1 PawnStateCommand`)
2. `CreateLoop_RunningLoop_PawnStateCommandCarriesTopSkills` (poll for `>=1 PawnStateCommand`)
3. `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge` (poll for `>=2 TickAdvancedCommand`, monotonic Tick)

Representative body -- `CreateLoop_RunningLoop_PawnStateCommandCarriesRealName`:

```csharp
[Fact]
[Trait("Category", "Integration")]
public void CreateLoop_RunningLoop_PawnStateCommandCarriesRealName()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    var stateCommands = new List<PawnStateCommand>();

    try
    {
        context.Loop.Start();
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        while (DateTime.UtcNow < deadline && stateCommands.Count == 0)
        {
            bridge.DrainCommands(c => { if (c is PawnStateCommand ps) stateCommands.Add(ps); });
            if (stateCommands.Count == 0) Thread.Sleep(25);
        }
    }
    finally
    {
        context.Loop.Stop();
    }
    // final drain to catch anything published between last poll and Stop
    bridge.DrainCommands(c => { if (c is PawnStateCommand ps) stateCommands.Add(ps); });

    stateCommands.Should().NotBeEmpty(
        "the loop must publish at least one PawnStateCommand within the 5s budget");
    foreach (var ps in stateCommands)
    {
        ps.Name.Should().NotBeNullOrWhiteSpace();
        ps.Name.Should().Contain(" ", "names follow forename + surname format");
    }
}
```

**Coverage anchors:** F-10 #2 (closes); TESTING_STRATEGY §5.1 mapping (the criterion "bootstrap pawn
naming reaches the render command stream" moves from OPEN-known-failure to a passing deterministic test).
**Test count delta:** +0 (3 modified); Modding total unchanged (399).

### 7.2 -- D2: fixture Release-staging fix

Add the 13 disk-only `Fixture.RegularMod_*` projects to `DualFrontier.sln` under the `tests` solution
folder, using the clean mechanical route:

```
dotnet sln DualFrontier.sln add tests\Fixture.RegularMod_BadApiVersion\Fixture.RegularMod_BadApiVersion.csproj --solution-folder tests
... (repeat for all 13: BadApiVersion, CyclicA, CyclicB, DependedOn, DependsOnAnother,
    DependsOnBadApi, DepsBadVersion, MissingOptional, MissingRequired, ReplacesCombat,
    ReplacesCombat_Alt, ReplacesProtected, ReplacesUnknown)
```

Then verify the resulting `.sln` gives each new project the same 6-row `ProjectConfigurationPlatforms`
block as the existing fixtures (Debug/Release x Any CPU/x64/x86, x64/x86 mapping to Any CPU per the
existing pattern) and a `tests` NestedProjects entry. If `dotnet sln add` does not produce the x64/x86
`ActiveCfg` rows that peers have, add them to match (the invariant is that a solution Release build
now builds these projects in Release).

**Acceptance (D2):** `dotnet build DualFrontier.sln -c Release` builds all 13 in Release (no
`-> ...\bin\Debug\...` lines for them in the log); `bin\Release\...\Fixtures\` contains 18 fixture
dirs; `dotnet test tests\DualFrontier.Modding.Tests\ -c Release --no-build` = 399/399 (via the safe
harness). Record as a `Skeleton revisions` note in the commit body (the `.sln` membership set is
mutable build surface).

### 7.3 -- D3: fixture-deployment fail-fast guard

Create `tests/DualFrontier.Modding.Tests/Sharing/FixtureDeploymentTests.cs` -- one meta/deployment
guard that asserts every fixture the Modding suite consumes is present under `Fixtures/`, converting a
missing-fixture deployment gap into ONE clear failure instead of many cryptic assembly-load errors.

**Test plan** (`FixtureDeploymentTests.cs` -- §3.8 meta (deployment-shape guard), 1 created):
1. `AllReferencedFixtures_DeployedUnderFixturesRoot`

Representative body:

```csharp
public sealed class FixtureDeploymentTests
{
    // The fixtures the Modding suite loads through the ALC boundary; each must be
    // staged by its DeployToTestFixtures target into Fixtures/<name>/ (TESTING_STRATEGY §2.3).
    private static readonly string[] ExpectedFixtures =
    {
        "tests.shared.events", "Fixture.PublisherMod", "Fixture.SubscriberMod",
        "Fixture.BadRegularMod", "tests.bad-shared-imod",
        "Fixture.RegularMod_DependsOnAnother", "Fixture.RegularMod_DependedOn",
        "Fixture.RegularMod_CyclicA", "Fixture.RegularMod_CyclicB",
        "Fixture.RegularMod_MissingRequired", "Fixture.RegularMod_MissingOptional",
        "Fixture.RegularMod_BadApiVersion", "Fixture.RegularMod_DepsBadVersion",
        "Fixture.RegularMod_DependsOnBadApi", "Fixture.RegularMod_ReplacesCombat",
        "Fixture.RegularMod_ReplacesCombat_Alt", "Fixture.RegularMod_ReplacesProtected",
        "Fixture.RegularMod_ReplacesUnknown",
    };

    [Fact]
    public void AllReferencedFixtures_DeployedUnderFixturesRoot()
    {
        string root = Path.Combine(AppContext.BaseDirectory, "Fixtures");
        var missing = ExpectedFixtures
            .Where(n => !Directory.Exists(Path.Combine(root, n)))
            .ToList();

        missing.Should().BeEmpty(
            "every referenced fixture must be staged under Fixtures/. A missing fixture usually "
            + "means it is not a DualFrontier.sln member, so a solution build stages it in the wrong "
            + "configuration (TESTING_STRATEGY §2.3). Missing: " + string.Join(", ", missing));
    }
}
```

(The expected set is the 18 fixtures `Modding.Tests.csproj` references. If the two name-vs-folder
cases -- `tests.shared.events` for `Fixture.SharedEvents`, `tests.bad-shared-imod` for
`Fixture.BadSharedMod_WithIMod` -- differ from the deployed folder names on disk, use the deployed
folder name, which equals each fixture's `AssemblyName`; confirm against the actual `Fixtures/`
listing at Phase 0.)

**Coverage anchors:** the fixture-copy completeness invariant (D2); §2.3 cross-ALC projection integrity.
**Test count delta:** +1; Modding total 399 -> 400.

### 7.4 -- D4 (rider): DRY hoist of the fixture-copy target

**Precondition gate (HALT H(gov) if unmet):** confirm the `DeployToTestFixtures` target is byte-identical
across ALL 19 `Fixture.*.csproj` before removing any. If any differs, STOP -- do NOT hoist; land D2+D3
only and record D4 as deferred (the per-fixture targets stay). Only on confirmed identity:

1. Create `tests/Directory.Build.targets` carrying a single `DeployToTestFixtures` target with the same
   body (`$(TargetPath)` + `$(MSBuildProjectDirectory)\mod.manifest.json` ->
   `...\DualFrontier.Modding.Tests\bin\$(Configuration)\$(TargetFramework)\Fixtures\$(AssemblyName)\`),
   conditioned so it applies only to the `Fixture.*` projects (e.g. `Condition` on
   `$(MSBuildProjectName)` starting with `Fixture.`, or an opt-in property the fixtures set) -- it must
   NOT fire for the nine test projects or other `tests/` projects.
2. Remove the now-redundant per-project `DeployToTestFixtures` target from all 19 `Fixture.*.csproj`.

**Acceptance (D4):** identical staging outcome to D2 -- `bin\Debug\...\Fixtures\` and
`bin\Release\...\Fixtures\` each contain the same 18 dirs as before; Modding 400/400 under both
configs (per-project) and under solution Release `--no-build`. If any fixture stops staging, revert D4
(H(gov)) -- the per-fixture targets return; D2+D3 stand.

### 7.5 -- D5: Extreme scale-non-completer quarantine

In `SchedulerExtremeTests.cs`, mark S1, S2, S7 with an unconditional static Skip and truth-up the S1
name. (The class comment "xUnit Skip is not used (it requires throwing)" refers to DYNAMIC/conditional
skip; the static `[Fact(Skip="..")]` attribute is unconditional and does not throw -- it is the correct
tool for an unconditional quarantine.)

- `S1`: rename the method to reflect its body (`SystemCount=50_000`, `TickIterations=3_000`) -- e.g.
  `S1_NativeGraph_FiftyThousandSystems_ThreeThousandTicks_HoldsWithoutError` -- and add
  `[Fact(Skip = "F-29(b): ExtremeScale non-completer -- native scheduler scale pathology; does not complete within CI budget. See docs/ROADMAP.md F-29.")]`.
- `S2`, `S7`: add the same `[Fact(Skip = "F-29(b): ...")]` (no rename needed unless a name-vs-body
  mismatch exists -- check and truth-up if so).

Leave the other 8 Extreme tests untouched. Do NOT alter the class-level `[Trait("Category","Extreme")]`
or `[Collection("ExtremeSerial")]` -- with S1/S2/S7 Skipped, an explicit `Category=Extreme` run no
longer hangs (the 3 hangers are skipped; the 8 pass).

**Acceptance (D5):** `dotnet test tests\DualFrontier.Core.Tests\ -c Debug --no-build --filter "Category=Extreme"`
(safe harness) completes with 8 passed / 3 skipped, no hang.

### 7.6 -- D6/D7 (code-truth for the doc + ledger)

D6 exact text is in §8. D7 (ROADMAP F-ledger) writes: F-10 -> CLOSED with the disposition note;
F-28 resolution updated (a folded, b remains OPEN); F-29 NEW OPEN. Exact ledger rows in §8 as well.
Both are orchestrator-only single-writer edits.

## 8. Governance machinery -- exact text [KIND: governance]

Ground the target's CURRENT live version before the bump: `TESTING_STRATEGY.md` frontmatter reads
`version: "2.0.2"` (verified). Bump to **2.1.0** -- MINOR per §9.1 (a new invocation-safety convention
is additive law; the §2.6/§3.8 refreshes ride along). The frontmatter mirror is auto-generated from
REGISTER by sync -- edit the BODY (below) and the §9.2 change-history; the frontmatter `version` and the
REGISTER entry move together in §12's closure + render.

> Replacement text below is ASCII-normalized per deliverable hygiene. The doc's house style uses
> Cyrillic-Ka "K-L" and guillemet quotes; the executor MAY match house glyphs when applying, but ASCII
> is acceptable and preferred. Apply the content verbatim.

### D6.1 -- REPLACE §2.6 in full

Current §2.6 opens "Two tests fail on `main` as of the 2026-06-11 survey..." and lists the two F-10
tests. Replace the entire §2.6 body with:

```
### §2.6 -- Honesty register: known-failing and quarantined tests

As of the F-10 isolation cascade (2026-07-02) the F-10 pre-existing failures are resolved; this
register states the current honest state. No test fails deterministically in per-project isolation.
Two categories are declared here so no cascade silently absorbs them (F-ledger: docs/ROADMAP.md
Findings ledger).

1. Quarantined -- do not run (native scale pathology, F-29(b)). Three SchedulerExtremeTests scenarios
   do not complete within any CI budget on the reference machine: S1 (50,000 systems x 3,000 ticks),
   S2 (200,000 ticks), S7 (250,000 systems). They are compute-bound in the native/managed scheduler at
   scale (the class's own diagnostic hypothesizes an O(N^2) register-conflict scan or a native mutex
   above ~90k entries). They carry [Fact(Skip = "...F-29...")] and are run by no sweep until F-29(b) is
   resolved; the fixing cascade removes the Skip and brings the completion evidence. Recorded, not
   absorbed.

2. Load-sensitive (F-29(a)). SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_ComputesAndTicksWithoutError
   (Category=Stress) passes green in per-project isolation but has produced a native TickBegin crash
   (testhost crash) under concurrent build/test load. That crash is a native concurrency signature
   (F-29(a)), not a deterministic assertion failure; the test stays under Category=Stress and runs only
   in the dedicated serial no-load Stress pass (§8). Declaring an unqualified "all Stress pass" while
   F-29(a) is open is a truth-law violation; the honest form is "Stress passes in the serial isolation
   pass; the F-29(a) load-crash is re-verified as pre-existing".

The now-resolved F-10 members are in git history and the F-ledger (F-10 CLOSED): the three
GameBootstrapIntegrationTests.CreateLoop_RunningLoop_* tests were flaky under xUnit intra-suite
parallelism and are fixed (serial GameLoopSerial collection + poll-until-condition, replacing fixed
Thread.Sleep windows); the shutdown "wedge" was root-caused as a stdout-pipe deadlock -- an invocation
hazard, not a test hang -- and is codified in §8.
```

### D6.2 -- REPLACE the body of §3.8

Current §3.8 declares "Current state: NONE exist." Replace the section body (keep the §3.8 header) with:

```
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
```

### D6.3 -- AMEND §8: fix the fast-sweep filter + add the invocation-safety law

(i) In §8, the "Stress exclusion (fast default sweep)" block currently filters `Category!=Stress`.
Replace that block with a heavy-test exclusion that excludes BOTH Stress and Extreme:

```
Heavy-test exclusion (fast default sweep) and inclusion:

    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category!=Stress&Category!=Extreme"
    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category=Stress"
    dotnet test tests/DualFrontier.Core.Tests/ -c Release --filter "Category=Extreme"

The fast default sweep excludes both Stress and Extreme (both are heavy). The Extreme scale
non-completers S1/S2/S7 are additionally Skip-guarded (§2.6), so even an explicit Category=Extreme run
completes (8 pass / 3 skipped) rather than hanging. Trait values available for filtering are exactly
those of §3.7: Stress, Extreme, Integration.
```

(ii) Add a NEW subsection to §8 (immediately after the "Testhost file-lock caveat" note):

```
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
```

### D6.4 -- ADD the §9.2 change-history entry (at the top of the change history)

```
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
```

### D7 -- ROADMAP F-ledger rows (exact)

REPLACE the F-10 row's State + Resolution cells:

```
| F-10 | (finding text unchanged) | S1 | CLOSED | F-10 isolation cascade 2026-07-02: recon proved zero deterministic failures. #2 (RunningLoop) FIXED (serial GameLoopSerial collection + poll-until-condition replacing fixed sleeps) at C2 <hash>. #1 (SchedulerStressTests) is green in isolation; the native TickBegin crash-under-load reassigned to F-29(a); kept Category=Stress under the serial isolation pass. The "zombie testhost wedge" root-caused as a stdout-pipe deadlock (not a test hang) and codified in TESTING_STRATEGY §8 (C7 <hash>). Extreme scale non-completers S1/S2/S7 reassigned to F-29(b), Skip-quarantined (C6 <hash>). |
```

UPDATE the F-28 Resolution cell (State stays OPEN for (b)):

```
| F-28 | (finding text unchanged) | S3 | OPEN | (a) TESTING_STRATEGY §3.8 "NONE exist" CLOSED -- corrected to the realized meta layer (CensusMetaTests + FixtureDeploymentTests) at the F-10 cascade C7 <hash>. (b) the 17 analyzer-rule XML <remarks> "Phase beta-prep stub" sweep remains OPEN -- next hygiene pass (comment-only analyzer-file class). |
```

APPEND the new F-29 row:

```
| F-29 | Native/managed scheduler pathology surfaced by the F-10 recon (2026-07-02): (a) SchedulerStressTests.NativeGraph_FiveThousandSystems_RandomDag_... produces a native TickBegin crash (testhost crash) under concurrent build/test load -- green in per-project isolation, so a contention/concurrency defect in the native SystemGraphInterop/bus path, not a deterministic assertion failure; (b) SchedulerExtremeTests S1 (50k systems x 3k ticks), S2 (200k ticks), S7 (250k systems) do not complete within a 120-180s watchdog -- compute-bound at scale; the class's own comment hypothesizes an O(N^2) register-conflict scan or a native mutex above ~90k entries. Mitigated this cascade: (a) kept Category=Stress under the serial isolation pass; (b) Skip-quarantined so no sweep hangs. | S2 | OPEN | Architect / dedicated native-scheduler cascade -- investigate concurrency (a) and scale (b); may share a root cause in SystemGraphInterop. The fixing cascade removes the S1/S2/S7 Skip and brings completion evidence. |
```

Also record, in the ROADMAP closure note, a **Lesson candidate** (not formalized here): the
stdout-pipe / TRX-is-truth invocation law -- an executor-level lesson analogous to the #N-series,
sourced from the F-10 recon; formalize at the next methodology cascade (alongside any other pending
candidates).

## 9. S-LOCK invariants [CORE]

**One additive structural lock.** S-LOCK (fixture-deployment completeness): every fixture the Modding
suite references must be staged under `Fixtures/` -- verifying artifact = `FixtureDeploymentTests`
(D3). No K-L invariant or PA axiom is added or altered. The RunningLoop determinism and the
no-pipe invocation law are disciplines (test-side and doc-codified), not structurally-enforceable
locks; they are not S-LOCKs.

## 10. Census discipline [CORE]

**The `src/**/*.cs` census pins are UNTOUCHED and are the regression anchor.** All edits are in
`tests/`, `docs/`, and `DualFrontier.sln`; none is a `src/` file. Therefore:

- reserved-surface pin `[ReservedStub` (34/13, HARD), the five marker families (§4.2 SOFT), and
  DFK-WAIVER (2, HARD) do NOT move. `CensusMetaTests` MUST remain green end-to-end -- a census-pin
  change would signal an out-of-scope `src/` edit (H2/H6).
- The `.sln` fixture-membership set is mutable build surface: D2's additions are recorded as a
  `Skeleton revisions` note in the commit body (`RESERVED_SURFACE_MUTABILITY.md`), not a finding.
- New Skip attributes / a poll helper / a guard test are `tests/`-only and touch no registered
  census expression.

## 11. Commit plan [CORE]

Atomic, dependency order; each passes the safe-harness gates. Commit count is intended-form -- a
needed split or a writer-defect iteration may add one; record the deviation in the closure report,
do not compress history to match.

| #  | Subject | Content |
| -- | ------- | ------- |
| C1 | `governance(testing): enroll F10_TEST_ISOLATION brief + recon report + validation checkpoint` | brief + `docs/reports/F10_TEST_ISOLATION_RECON_REPORT.md` enrolled; VALIDATION_REPORT folded |
| C2 | `test(modding): serialize + poll-stabilize RunningLoop integration family` | D1 (closes F-10 #2) |
| C3 | `build(sln): add 13 Fixture.RegularMod_* to DualFrontier.sln` | D2 + Skeleton revisions note |
| C4 | `test(modding): fixture-deployment fail-fast guard` | D3 |
| C5 | `build(tests): hoist DeployToTestFixtures into tests/Directory.Build.targets` | D4 rider (precondition-gated; omit if not identical) |
| C6 | `test(scheduler): Skip-quarantine Extreme scale non-completers S1/S2/S7` | D5 + S1 name truth-up |
| C7 | `docs(testing): TESTING_STRATEGY 2.1.0 -- invocation-safety law + fast-sweep filter + honesty register + F-28(a)` | D6 (§2.6/§3.8/§8/§9.2 exact text) |
| C8 | `docs(roadmap): F10_TEST_ISOLATION closure -- F-10 CLOSED, F-29 seeded, F-28(a) folded` | D7 (F-ledger + lesson candidate note) |
| C9 | `governance(register): F10_TEST_ISOLATION REGISTER closure (2.21 -> 2.22)` | REGISTER mutations + validate folded |
| C10 | `governance(register): render regeneration + header backfill` | render + header self-reference backfill |

## 12. REGISTER cascade [CORE]

Using ONLY the Phase 0 verbatim enum shapes (H5 on any missing value):

- **Enroll** (C1): the brief `DOC-D-F10_TEST_ISOLATION_BRIEF` (category D, tier 3, lifecycle per the
  brief-enrollment convention) and the recon report at `docs/reports/F10_TEST_ISOLATION_RECON_REPORT.md`
  (pattern its entry on an existing `docs/reports/` recon-report enrollment read at Phase 0).
- **Version bump** (C9): `DOC-B-TESTING_STRATEGY` 2.0.2 -> 2.1.0 (frontmatter mirror syncs).
- **Lifecycle** (C9): brief Draft -> EXECUTED at closure per convention; the recon report to its
  consumed-recon terminal lifecycle per the pattern read at Phase 0.
- **EVT** (C9): one audit-trail event `EVT-2026-07-02-F10_TEST_ISOLATION-CLOSURE` (exact shape per
  Phase 0) carrying the real hashes of C2-C8.
- **`register_version`** 2.21 -> 2.22; **document-count delta +2** (brief + report).
- Validate exit 0 mandatory (H3); fix only within the empirical enum vocabulary (H5).

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition mismatch (§3.1).
- **H2** build/test regression vs the Phase 0 baseline -- a previously-green test going red, or a
  census-pin move (signals an out-of-scope `src/` edit).
- **H3** validate nonzero.
- **H5** a REGISTER enum value is needed that the Phase 0 vocabulary lacks -- escalate, never invent.
- **H6** truth-law unsatisfiable without an architectural decision; or a §6 checkpoint failure that
  cannot be resolved by returning to the owning edit.
- **H(gov)** any semantic change to `TESTING_STRATEGY.md` beyond the D6 ratified scope; OR the D4
  hoist precondition unmet (targets not byte-identical) -- land D2+D3 only, defer D4.
- Standing rails: **no pushes to origin**; **no `-Sync` outside §12**; no history rewrite /
  force-push / squash; single-writer files (`ROADMAP.md`, `REGISTER.yaml`) orchestrator-only.

On halt: stop, report state verbatim, await the operator. In-session re-confirmation before resuming
is expected behavior (the auto-mode push-to-main block is expected, not a fault).

## 14. Closure protocol and report [CORE]

Execute the `METHODOLOGY.md §12.7` closure protocol: (a) tracker write-back -- ROADMAP F-ledger (D7)
+ any forward-state; (b) REGISTER + validate folded (C9); (c) render + header backfill (C10); (d)
findings -> F-ledger entries done in D7 (F-10 CLOSED, F-28(a) folded, F-29 seeded -- never chat-only);
(e) the closure report (chat).

The closure report carries: the commits table (hash | subject); the versions table (before -> after,
incl. register 2.21 -> 2.22 and TESTING_STRATEGY 2.0.2 -> 2.1.0); the census pins recorded (reserved
34/13 + families + waiver 2 -- all UNCHANGED-exact, the regression proof); the F-ledger final-state
table (F-10 CLOSED, F-28 OPEN-for-(b), F-29 NEW); the consolidated `Skeleton revisions` list (the
`.sln` membership additions; any D4 target relocation; the S1 rename; any deviation from this brief's
intended forms); the gates table (baseline vs closure, via the safe harness -- RunningLoop now green
x>=20, Extreme 8/3-skipped, Modding 400/400 both configs incl. solution Release, all other suites
match-or-better, `CensusMetaTests` green); self-attestation (no pushes; single render run; no `-Sync`
outside §12; no history rewrite; no `src/` edit; no reference tree touched); and the operator manual
checklist (push; ratify the brief/report lifecycle transitions; the standing F-queue items that remain
operator/architect-owned -- notably F-29, F-28(b)).

## 15. Out of scope [CORE]

- **The native scheduler product defects themselves (F-29 (a) crash-under-load, (b) scale
  non-completion).** This cascade quarantines and seeds them; it does NOT diagnose or fix the native
  `SystemGraphInterop`/bus code. That is a dedicated architect-owned cascade.
- **F-28(b)** -- the 17 analyzer-rule XML `<remarks>` "Phase beta-prep stub" sweep (comment-only
  analyzer-file class) -- next hygiene pass, not here.
- **Any `src/` (production) edit.** If a fix appears to require one, HALT H6 and surface -- it is a
  different cascade.
- Other open findings (F-2, F-4/F-9, F-11, F-13, F-20, F-21, F-22, F-23, F-24) -- untouched.
- Pushing to origin; `-Sync` outside §12; snapshots / EXECUTED-doc content; the News Intelligence Hub
  reference tree.

---

**End of F10_TEST_ISOLATION_BRIEF.md v1.0**
