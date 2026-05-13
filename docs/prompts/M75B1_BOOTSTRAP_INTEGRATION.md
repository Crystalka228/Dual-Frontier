---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M75B1_BOOTSTRAP_INTEGRATION
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M75B1_BOOTSTRAP_INTEGRATION
---
# M7.5.B.1 — Production bootstrap integration for ModMenuController

## Context

M7.5.A closed (commits `9c895fe`, `4c648c6`, `198d948`). 408/408 tests passing. Working tree clean. `MOD_OS_ARCHITECTURE.md` LOCKED v1.5.

`ModMenuController` exists in `DualFrontier.Application/Modding/` as `internal sealed class`, fully unit-tested (22 tests). `IModDiscoverer` + `DefaultModDiscoverer` exist as the disk-discovery abstraction. `Pipeline.GetActiveMods()` extends the public read-API. **Production currently does NOT create any of these** — `GameBootstrap.CreateLoop` builds the simulation graph + scheduler + GameLoop with no `ModIntegrationPipeline` instance whatsoever. Modding work has been tests-only since M0.

M7.5 was decomposed during pre-flight into M7.5.A (controller logic, ✅ closed), **M7.5.B.1** ← this session (production bootstrap integration), and M7.5.B.2 (Godot UI scene). The decomposition was driven by the discovery that bootstrap-side wiring is substantial non-UI surface — pipeline construction, controller wiring, discovery root configuration, GameRoot signature adjustment — and deserves its own falsifiable closure separate from taste-driven Godot scene work.

M7.5.B.1 scope: extend `GameBootstrap` to construct the full modding stack (loader, registry with core systems, validator, contract store, pipeline, default discoverer, controller), expose the controller alongside the existing GameLoop via a new `GameContext` record, and adjust the single caller `GameRoot._Ready` to consume the new return shape. Smoke tests verify the production wiring produces a working controller. No Godot UI work, no actual mod toggling end-to-end through a scene — that lands in M7.5.B.2.

## Out of scope (M7.5.B.2 / M7-closure / M8 will do — NOT in this session)

- Godot UI scene (`ModMenuPanel : Control`), button widgets, layout, copy, hot-reload disabled tooltip rendering — M7.5.B.2.
- End-to-end mod-toggling integration test through a Godot scene + user click sequence — M7.5.B.2 (manual verification only there).
- M7-closure session — separate post-M7.5.B.2 session with full M7 verification report.
- Vanilla mod skeletons that the menu would actually toggle in production — M8.
- Modifications to `DualFrontier.Core` or `DualFrontier.Contracts` — M-phase boundary discipline preserved through M3–M7.5.A. Verified via `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.
- Refactoring of existing `GameBootstrap` ECS-system construction. The hard-coded systems (NeedsSystem, MoodSystem, etc.) stay exactly as they are; M7.5.B.1 only adds modding-stack construction after the existing scheduler creation.

## Approved architectural decisions

1. **Bootstrap return signature: `CreateLoop` returns new `GameContext` record `(GameLoop Loop, ModMenuController Controller)`.** Backward-incompatible refactor of the single caller `GameRoot._Ready`. Atomic change. Per pre-flight ratification this is the cleanest of three options (vs adding parallel `CreateContext` method, vs `GameLoop.Controller` property — both create coupling or duplication).

2. **`GameContext` is `internal sealed record`** in `DualFrontier.Application/Loop/`. Carries `GameLoop` (currently internal — verify) and `ModMenuController` (internal). Both `DualFrontier.Modding.Tests` and `DualFrontier.Presentation` see it via existing `InternalsVisibleTo` declarations on `DualFrontier.Application.csproj`. A `public` record carrying an `internal` member would not compile (C# accessibility rules); internal record is the only valid option without downgrading the controller.

3. **Discovery root configuration: parameter to `CreateLoop`, default `"mods"`.** `CreateLoop(PresentationBridge bridge, string modsRoot = "mods")`. Production Godot launches with cwd = project root, so `"mods"` resolves to `<project>/mods/`. Tests override the parameter with a temp path or fixture path. The string is passed through to `DefaultModDiscoverer` unchanged — discoverer's existing contract handles non-existent paths by returning an empty list (no exception).

4. **Pipeline construction order in bootstrap: AFTER scheduler construction.** The pipeline takes `scheduler` as a constructor argument. The existing bootstrap flow builds the graph and creates the scheduler — pipeline construction inserts immediately after, before `GameLoop` instantiation. The pipeline does not invoke `Apply` from bootstrap — it stays inert until the menu commits an editing session.

5. **Core systems passed to `ModRegistry.SetCoreSystems` are the same instances added to the dependency graph.** Bootstrap holds the array of core SystemBase instances locally, registers them with the graph (existing behaviour) AND with the modding registry. This matches the M7.2 test pattern (`registry.SetCoreSystems(coreSystems)` after building the same array). Future bridge replacement (M9+) will let mods supersede these via `replaces`.

6. **Controller's `GameServices` parameter: the same `services` instance the loop consumes.** Pipeline routes mod-published events through this services aggregator (existing M2 wiring). One services instance per session, shared between kernel systems and mod systems — verified by every M-phase test using single `IGameServices` per pipeline.

7. **No call to `Pipeline.Pause()` in bootstrap.** The pipeline's default state is paused (M7.1's load-bearing default). The first `BeginEditing` from the menu side calls Pause again (idempotent per M7.1 AD). Bootstrap leaves the pipeline in default state, GameLoop starts ticking the scheduler with kernel-only systems.

8. **Test surface: smoke-only.** ~5 tests verifying production wiring produces a working controller. Heavier end-to-end mod loading through bootstrap (manual `dotnet run` against `mods/DualFrontier.Mod.Example/`) is a manual verification step in commit 1 message; the full UI loop verification belongs to M7.5.B.2 manual sign-off.

9. **Atomic phase review per METHODOLOGY §2.4** — implementation, tests, ROADMAP closure all in one session. Three-commit invariant per §7.3.

## Required reading

1. `docs/architecture/MOD_OS_ARCHITECTURE.md` LOCKED v1.5 — §9.1 (lifecycle states), §9.2 (menu flow — bootstrap exposes the controller the menu drives), §1.4 (load graph — bootstrap's discovery root resolves to where mods live).
2. `docs/ROADMAP.md` — M7 sub-phase status block, M7.5.A closure entry pattern as reference for M7.5.B.1's closure entry shape.
3. `docs/methodology/METHODOLOGY.md` — §2.4 atomic phase review, §7.3 three-commit invariant.
4. `docs/methodology/CODING_STANDARDS.md` — full document. Especially: one class per file, English-only comments, member order, `_camelCase` private fields, **Stack-frame retention** section (added in commit `f4b2cb8`).
5. Code (full files):
   - `src/DualFrontier.Application/Loop/GameBootstrap.cs` — current shape; the refactor target. Read the whole `CreateLoop` body to understand existing graph construction order.
   - `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — public surface (constructor signature, IsRunning, GetActiveMods, Apply, UnloadMod, Pause/Resume).
   - `src/DualFrontier.Application/Modding/ModMenuController.cs` — controller class (M7.5.A, internal sealed) — verify constructor signature `(ModIntegrationPipeline pipeline, IModDiscoverer discoverer)`.
   - `src/DualFrontier.Application/Modding/ModRegistry.cs` — `SetCoreSystems` method shape.
   - `src/DualFrontier.Application/Modding/DefaultModDiscoverer.cs` — discoverer constructor `(string rootPath)`.
   - `src/DualFrontier.Presentation/Nodes/GameRoot.cs` — single caller of `GameBootstrap.CreateLoop`. Refactor target for the consumer side.
   - `src/DualFrontier.Application/DualFrontier.Application.csproj` — confirm `InternalsVisibleTo Include="DualFrontier.Presentation"` and `InternalsVisibleTo Include="DualFrontier.Modding.Tests"` are both present.
6. M7.5.A test patterns:
   - `tests/DualFrontier.Modding.Tests/Menu/ModMenuControllerTests.cs` — controller-level tests for behavioural reference. M7.5.B.1 tests are higher-level (production wiring) and don't duplicate M7.5.A's behavioural coverage.

## Implementation

### 1. New `GameContext` record

`src/DualFrontier.Application/Loop/GameContext.cs`:

```csharp
namespace DualFrontier.Application.Loop;

/// <summary>
/// Aggregate handle returned by <see cref="GameBootstrap.CreateLoop"/>.
/// Carries the simulation loop and the mod-menu controller wired to the
/// same scheduler / services / pipeline state. The caller (GameRoot in
/// production, integration tests in DualFrontier.Modding.Tests) uses
/// <see cref="Loop"/> to start/stop the simulation and
/// <see cref="Controller"/> to drive the menu-side editing session.
///
/// Internal because it carries an internal ModMenuController; reachable
/// from DualFrontier.Modding.Tests and DualFrontier.Presentation via
/// the existing InternalsVisibleTo declarations on the Application
/// project.
/// </summary>
internal sealed record GameContext(GameLoop Loop, ModMenuController Controller);
```

Add `using DualFrontier.Application.Modding;` so the record can name `ModMenuController`.

### 2. Refactor `GameBootstrap.CreateLoop`

`src/DualFrontier.Application/Loop/GameBootstrap.cs` — modify `CreateLoop` signature and body. Add `using DualFrontier.Application.Modding;`.

New signature:

```csharp
public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")
```

Body changes (after existing scheduler construction, before `return new GameLoop(...)`):

```csharp
// ... existing graph construction with hard-coded core systems ...
var coreSystems = new SystemBase[]
{
    new NeedsSystem(),
    new MoodSystem(),
    new JobSystem(),
    new MovementSystem(pathfinding),
    new PawnStateReporterSystem(),
    new InventorySystem(),
    new HaulSystem(),
    new ElectricGridSystem(),
    new ConverterSystem(),
};

var graph = new DependencyGraph();
foreach (SystemBase s in coreSystems)
    graph.AddSystem(s);
graph.Build();

var scheduler = new ParallelSystemScheduler(
    graph.GetPhases(),
    ticks,
    world,
    faultSink: null,
    services: services);

// M7.5.B.1 — modding stack construction. Pipeline stays in default
// paused state; the menu (M7.5.B.2) drives Pause/Apply/Resume from the
// controller surface returned in the GameContext.
var modLoader = new ModLoader();
var modRegistry = new ModRegistry();
modRegistry.SetCoreSystems(coreSystems);
var modValidator = new ContractValidator();
var modContractStore = new ModContractStore();
var pipeline = new ModIntegrationPipeline(
    modLoader, modRegistry, modValidator, modContractStore, services, scheduler);
var discoverer = new DefaultModDiscoverer(modsRoot);
var controller = new ModMenuController(pipeline, discoverer);

var loop = new GameLoop(scheduler, bridge);
return new GameContext(loop, controller);
```

**Critical**: `coreSystems` is now an explicit local array (existing code adds them inline `graph.AddSystem(new NeedsSystem())` etc.). The refactor replaces the inline `new` with an array first, then iterates. Same instances flow into both `graph.AddSystem` and `modRegistry.SetCoreSystems`.

XML doc on `CreateLoop` updated to mention the modding stack construction and the `modsRoot` parameter, noting that the controller-driven menu flow lives in M7.5.B.2.

### 3. Adjust `GameRoot._Ready`

`src/DualFrontier.Presentation/Nodes/GameRoot.cs` — single caller. Replace:

```csharp
_loop = GameBootstrap.CreateLoop(_bridge);
```

with:

```csharp
GameContext context = GameBootstrap.CreateLoop(_bridge);
_loop = context.Loop;
_modMenuController = context.Controller;
```

Add `private ModMenuController _modMenuController = null!;` field. Add `using DualFrontier.Application.Modding;` and `using DualFrontier.Application.Loop;` (verify the latter is already present — it is, since `using DualFrontier.Application.Loop;` is the existing import for `GameLoop`).

The controller is held as a field so M7.5.B.2 can wire it to the Godot UI scene without re-constructing the full bootstrap context. M7.5.B.1 itself does NOT consume the controller from GameRoot — the field exists but stays unused until M7.5.B.2 binds it.

XML doc on the `_modMenuController` field notes the M7.5.B.2 forward dependency.

### 4. Project references

No new project references needed. `DualFrontier.Presentation.csproj` already references `DualFrontier.Application` (verify by inspection). `DualFrontier.Modding.Tests.csproj` already references it.

## Tests

### `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs`

New test class. New subdirectory `Bootstrap/` parallels existing `Pipeline/`, `Menu/`, `Validator/`, etc.

The harness for these tests is `GameBootstrap.CreateLoop` itself — production wiring is the unit under test, no separate harness needed. Use `PresentationBridge` test instance and a temp directory for `modsRoot` where appropriate.

1. **`CreateLoop_ReturnsContextWithLoopAndController`** — call `CreateLoop(new PresentationBridge())`. Assert returned `GameContext` is non-null, `Loop` is non-null, `Controller` is non-null. Smoke-test of the production constructor chain.

2. **`CreateLoop_ReturnedController_BeginEditingSucceedsAndPauses`** — call `CreateLoop` with the default `mods` root. The pipeline starts paused (M7.1 default). Call `context.Controller.BeginEditing()`. Assert `context.Controller.IsEditing == true`. The pipeline's IsRunning is hidden behind the controller — but if accessible via test seam (none currently exists; assert via `BeginEditing()` not throwing is sufficient).

3. **`CreateLoop_WithEmptyModsRoot_GetEditableStateReturnsEmpty`** — pass `modsRoot: <temp dir>` where temp dir exists but is empty. Call `BeginEditing` on controller. Call `GetEditableState()`. Assert returned list is empty (no active mods, no discovered mods).

4. **`CreateLoop_WithModsRootContainingFixture_GetEditableStateReturnsFixture`** — pass `modsRoot` pointing at a temp dir containing a single fixture mod (manifest + minimal contents — reuse the M7.5.A discoverer test fixture pattern, OR point at `mods/DualFrontier.Mod.Example/` directly via assembly-relative path). Call `BeginEditing` then `GetEditableState`. Assert list contains exactly 1 entry with `IsCurrentlyActive=false`, `IsPendingActive=false`, `CanToggle=true`. Locks the production wiring through the discoverer.

5. **`CreateLoop_WithNonExistentModsRoot_GetEditableStateReturnsEmpty_NoThrow`** — pass `modsRoot: "definitely/not/a/real/path"`. CreateLoop succeeds (DefaultModDiscoverer.Discover handles non-existent paths gracefully per M7.5.A). BeginEditing succeeds. GetEditableState returns empty list. Locks the first-launch-with-no-mods-directory scenario.

6. **`CreateLoop_DefaultModsRoot_IsLiteralStringMods`** — verify the default parameter value is `"mods"` (not `"./mods"`, not absolute). Use reflection on `MethodInfo.GetParameters()[1].DefaultValue`. Locks AD #3 at the API surface so an accidental refactor doesn't silently change the default.

(Optional 7th test, if 5+6 surface no friction:) **`CreateLoop_ReturnedLoop_StartStopRoundTripsCleanly`** — call `context.Loop.Start()`, immediately call `context.Loop.Stop()`. No exception. Verifies the loop returned by the new context shape behaves identically to the previous direct-return shape (zero functional regression on the simulation side).

### Out-of-scope tests (M7.5.B.2 / future will add)

- Godot UI scene smoke tests — M7.5.B.2.
- Manual click-through verification of menu → controller → pipeline → scheduler — M7.5.B.2.
- Real mod-toggling end-to-end through `mods/DualFrontier.Mod.Example/` — M7.5.B.2 manual verification.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors.
2. `dotnet test` — 408 existing pass; 6 new pass (or 7 with optional). Expected total: **414/414** (or 415).
3. `GameBootstrap.CreateLoop` signature: `public static GameContext CreateLoop(PresentationBridge bridge, string modsRoot = "mods")`.
4. `GameContext` exists as `internal sealed record(GameLoop Loop, ModMenuController Controller)` in `src/DualFrontier.Application/Loop/`.
5. `GameRoot._Ready` consumes the new return shape; `_modMenuController` field exists and is populated.
6. Modding stack constructed in bootstrap: `ModLoader`, `ModRegistry` (with `SetCoreSystems(coreSystems)`), `ContractValidator`, `ModContractStore`, `ModIntegrationPipeline`, `DefaultModDiscoverer`, `ModMenuController`. Same `services` instance threaded through both kernel scheduler and pipeline.
7. Pipeline starts in default paused state — bootstrap does NOT call `Pause` or `Resume`.
8. M7.1 + M7.2 + M7.3 + M7.4 + M7.5.A regression guards still pass: every existing test in `M71PauseResumeTests`, `M72UnloadChainTests`, `M73Step7Tests`, `M73Phase2DebtTests`, `ManifestRewriterTests`, `M74BuildPipelineTests`, `ModMenuControllerTests`, `DefaultModDiscovererTests`, `PipelineGetActiveModsTests` is green.
9. `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty (M-phase boundary discipline preserved through M7.5.B.1).
10. `dotnet sln list` unchanged — no new projects.
11. **Manual verification** documented in commit 1 message: `dotnet run` (or Godot launch via F5 from project root) starts the simulation cleanly with the new bootstrap; logs show no exceptions during ECS initialization or pipeline construction. (Pure smoke check that production startup not broken.)

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `feat(bootstrap): wire ModIntegrationPipeline + ModMenuController into GameBootstrap`

- New file `src/DualFrontier.Application/Loop/GameContext.cs`.
- Modify `src/DualFrontier.Application/Loop/GameBootstrap.cs`: signature, body, XML doc, new usings.
- Modify `src/DualFrontier.Presentation/Nodes/GameRoot.cs`: consume `GameContext`, add `_modMenuController` field, new usings.
- No test changes — verify existing M7.x suites still pass via `dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~ManifestRewriter"`.
- Manual verification: launch Godot or `dotnet run` (whichever the existing dev workflow uses), confirm clean startup. Document in commit body.

**2.** `test(bootstrap): GameBootstrap.CreateLoop integration smoke tests`

- New file `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` with 6 (or 7) tests.
- Run full suite. Confirm 414/414 (or 415).

**3.** `docs(roadmap): close M7.5.B.1 — production bootstrap integration`

- `ROADMAP.md` M7 sub-phase status block: M7.5.B replaced with M7.5.B.1 + M7.5.B.2 (parallel to M7.5 → M7.5.A + M7.5.B split done in M7.5.A closure). M7.5.B.1 entry: ✅ Closed with commits 1+2 SHA + acceptance summary in M7.1/M7.2/M7.3/M7.4/M7.5.A entry pattern. M7.5.B.2 entry: ⏭ Pending, "Godot UI scene + ModMenuController binding; manual verification".
- Header status line: `*Updated: YYYY-MM-DD (M7.5.B.1 closed — production GameBootstrap wires ModIntegrationPipeline + ModMenuController + DefaultModDiscoverer into a new GameContext return type; M7.5.B.2 + M7-closure pending).*`
- Engine snapshot: 408 → 414 (or 415) tests. List the new test class (`GameBootstrapIntegrationTests`).
- Status overview table M7 row tests column extended with `M7.5.B.1 added`.

**Special verification preamble for commits 1 + 2:**

- After commit 1: `dotnet build && dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~ManifestRewriter"` — no regression, all existing M7.x + M7.5.A tests green. Manual: launch the production entry point (Godot F5 or `dotnet run` against the Presentation project, whichever is the current dev workflow), confirm scene loads without exceptions in the log.
- After commit 2: `dotnet test --filter "FullyQualifiedName~GameBootstrapIntegration"` — 6 new tests green. Full suite at 414/414 (or 415).
- After commit 3: ROADMAP renders cleanly; M7.5.B.1 (closed) and M7.5.B.2 (pending) entries both present.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

Datapoints (per [M6 closure review §10](../audit/M6_CLOSURE_REVIEW.md)): M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0, M7.4=0, M7.5.A=0. M7.5.B.1 closure pending = potentially **tenth consecutive zero** post-M4.

M7.5.B.1 exercises §1.4 (load graph — discovery root resolves to where mods live) and §9.2 (the bootstrap produces the controller that drives the menu flow). The implementation surface — bootstrap-level wiring, record type, parameter default — sits on standard C# composition patterns and existing M-phase contracts. **If implementation surfaces a §1 or §9 contradiction requiring v1.6 ratification → hypothesis falsified. Report immediately.**

Plausible v1.6 candidates worth flagging if encountered:

(a) **§9 underspec on bootstrap-time pipeline state.** AD #7 (pipeline starts paused, bootstrap does not call Pause/Resume) follows M7.1's load-bearing default. If §9 wording demands explicit Pause-at-construction, ratification candidate.

(b) **`GameLoop` accessibility.** AD #2 assumes `GameLoop` is internal (or otherwise compatible with internal record member). If `GameLoop` is public, the GameContext record could potentially be public too — small change, not a contradiction; only flag if the C# accessibility rules surprise during compile.

(c) **Single-services threading vs separate services for mods.** AD #6 threads the same `IGameServices` instance through both scheduler and pipeline, matching all M-phase test patterns. If §9 or §6 wording demands an isolated services aggregator for mods, ratification candidate.

(d) **`ModRegistry.SetCoreSystems` semantic.** If passing the same SystemBase instances to `graph.AddSystem` and `registry.SetCoreSystems` causes any double-registration or conflict during a hypothetical Apply, this is an integration issue that would surface in test 7 (loop start/stop round trip) or in the manual verification step.

(e) **GameRoot field initialization timing.** `_modMenuController = null!;` field default with assignment in `_Ready` follows the existing `_loop = null!;` pattern. If Godot's lifecycle invokes anything before `_Ready` that touches the controller, NRE. Should not happen given M7.5.B.1 doesn't actually consume the controller anywhere yet, but flag if encountered.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (408 + 6 = 414 expected, or 415 with optional test 7, or actual with discrepancy noted).
- Per-test confirmation: 6 (or 7) new tests all green by name.
- Regression confirmation: M7.1 (11) + M7.2 (13) + M7.3 (5+2) + M7.4 (9) + M7.5.A (30) + remaining M0–M6 + Persistence + Systems + Core all still green.
- Working tree state: clean / dirty.
- **§9 / §1 contradiction status**: zero (or REPORT IMMEDIATELY with section reference + which of the 5 plausible categories above + proposed ratification candidate).
- **Manual verification of production startup**: confirmed clean (cite log excerpt or "no exceptions during ECS init / pipeline construction").
- **M-phase boundary discipline**: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` = empty (verify and report).
- **Solution file**: `dotnet sln list` count unchanged from M7.5.A baseline.
- Any unexpected findings, especially around `GameLoop` accessibility, `ModRegistry.SetCoreSystems` semantics with double-array passage, or Godot lifecycle interaction with the new `_modMenuController` field.
- **Special**: any §1 or §9 spec contradiction discovered (would be ratification candidate for v1.6 — flag immediately with category from list above).
