---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-HOUSEKEEPING_MENU_PAUSES_SIMULATION
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-HOUSEKEEPING_MENU_PAUSES_SIMULATION
---
# Housekeeping — Menu actually pauses simulation (§9.2 step 1 wiring)

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

Apply here: the menu currently CLAIMS to pause the simulation per §9.2 (`ModMenuController.BeginEditing()` calls `_pipeline.Pause()`) but the simulation visibly keeps ticking with the menu open (TICK counter advances, pawns keep wandering — confirmed via F5 screenshots, TICK 491 → 757 between two screenshots taken with the menu open the entire time). State that is claimed but does not exist is a placeholder lie. We fix by wiring the missing layer, not by removing the claim.

## Diagnosis

There are two independent pause flags in the codebase, and they are not connected to each other:

1. **`GameLoop._paused`** (in `src/DualFrontier.Application/Loop/GameLoop.cs`) — `volatile bool` gating tick advance in the background simulation thread. When `true`, the run loop sleeps 16 ms and skips `_scheduler.ExecuteTick`. Mutated only via `GameLoop.SetPaused(bool)`.

2. **`ModIntegrationPipeline._isRunning`** (in `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs`) — gates `Apply()` and `UnloadMod()` against being called mid-tick. Default `false` ("paused"). `Pause()` sets it to `false`, `Resume()` sets to `true`. Documented per the field's XML doc as the §9.3 "scheduler run flag" surface.

`ModMenuController.BeginEditing()` calls `_pipeline.Pause()` — sets `_isRunning = false`, which prevents Apply from throwing. **It does NOT call `_loop.SetPaused(true)`** — the simulation thread does not know the menu is open.

Per MOD_OS_ARCHITECTURE v1.5 §9.2 step 1: «menu pauses the scheduler». The spec wording is one entity ("the scheduler"), and the implementation has two distinct pause surfaces. This is a v1.6 ratification candidate — see hypothesis-falsification clause below.

The fix wires the second surface so the user-visible behavior matches the spec's intent: opening the menu visibly stops simulation; closing it (Apply success / Cancel) resumes.

## Out of scope

- ANY change to `src/DualFrontier.Core` or `src/DualFrontier.Contracts`. M-phase boundary preserved.
- ANY change to `ModMenuController` itself — its contract calls `_pipeline.Pause()` per its mandate; the controller does not (and should not) know about `GameLoop`. The wiring belongs at the orchestration layer (the bootstrap that owns both `GameLoop` and the controller).
- ANY change to `ModIntegrationPipeline._isRunning` semantics or to `Pause()`/`Resume()` methods on the pipeline. The Apply-mutation guard remains its own concern.
- ANY change to `GameLoop._paused` semantics or to `GameLoop.SetPaused`. The simulation-thread tick gate remains its own concern.
- Refactoring the two pause flags into one. That belongs to a v1.6 ratification cycle if we conclude §9.2 should specify a single unified pause surface; for this housekeeping pass we wire what exists.
- ANY change to UI widgets (`ModMenuPanel`, `ColonyPanel`, `PawnDetail`, `GameHUD`).
- The Cinzel + Kenney UI redesign brief — separate larger brief, decisions still pending.
- The `.sln` build verification gap — separate small housekeeping brief, deferred.
- M7-closure session — deferred.

## Approved architectural decisions

1. **Wiring location: `GameContext`.** The current `GameContext` record carries `(GameLoop Loop, ModMenuController Controller)`. We extend the controller's lifecycle hooks via callbacks set on the controller from inside `GameBootstrap.CreateLoop` so the wiring lives at the orchestration layer that owns both objects. The controller stays unaware of `GameLoop`.

2. **Mechanism: `Action? OnEditingBegan` and `Action? OnEditingEnded` hooks on `ModMenuController`.** Two nullable `Action` fields with public setters (or `Set` methods if setter accessibility friction surfaces). Default null — pre-existing tests construct controllers without setting them; the `null` path is a no-op. `BeginEditing()` raises `OnEditingBegan` after the existing `_pipeline.Pause()` call (i.e., on the path that actually transitions; idempotent re-entry stays a no-op). `Cancel()` and the success-path branch of `Commit()` raise `OnEditingEnded` after the `_pipeline.Resume()` call. The failure-path branch of `Commit()` does NOT raise `OnEditingEnded` (per AD #4 of M7.5.A — failed commit leaves the session open, simulation paused).

3. **Hook semantics: fire-after-pipeline.** Hooks fire AFTER the pipeline state transition, not before. Justification: if a hook callback throws, the pipeline state has already advanced, so a re-entry of `BeginEditing` will correctly take the "already editing" idempotent no-op path. Reverse order would corrupt the session state on a callback exception. Hooks are wrapped in `try/catch` inside the controller — if a hook throws, the controller swallows and continues, since hook failures must not prevent the menu lifecycle from completing. Swallowing matches §9.5.1's best-effort discipline philosophically (we don't apply that section verbatim — it's about unload step failures — but the discipline of "lifecycle cannot be derailed by sideband observers" is consistent).

4. **Wiring in `GameBootstrap.CreateLoop`.** After both `loop` and `controller` are constructed, set:
    ```csharp
    controller.OnEditingBegan = () => loop.SetPaused(true);
    controller.OnEditingEnded = () => loop.SetPaused(false);
    ```
    Captured `loop` is the local variable already in scope; no closure-lifetime concern (the loop is held by `GameContext.Loop` for the program's lifetime).

5. **No changes to `ModIntegrationPipeline.Pause`/`Resume`.** The pipeline keeps its existing semantics. The hooks operate IN PARALLEL to the existing pipeline-pause call, not as a replacement. The two flags remain independent on purpose — Apply-mutation safety and tick-advance gating are different concerns at the implementation level even though §9.2 conflates them at the spec level.

6. **Test coverage.** Three new integration tests in `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs`:
    - `MenuFlow_BeginEditing_PausesGameLoop` — `BeginEditing` causes `loop.IsPaused` to be true (or equivalent observable: tick count does not advance after a brief delay).
    - `MenuFlow_Cancel_ResumesGameLoop` — `Cancel` after `BeginEditing` causes `loop.IsPaused` to be false (tick count advances after a brief delay).
    - `MenuFlow_CommitSuccess_ResumesGameLoop` — `Commit()` returning success after `BeginEditing` causes `loop.IsPaused` to be false. (Failed-commit-stays-paused covered indirectly: the existing M7.5.A test `Commit_ValidationFailure_LeavesSessionOpen` plus this brief's AD #2 wording is sufficient — adding a fourth test would be parallel to the M7.5.A coverage which we don't want to duplicate.)

7. **`GameLoop` API surface for tests.** `GameLoop` currently exposes `SetPaused(bool)` but not a getter. Tests need to observe paused state. Add an `internal bool IsPaused` property (or `bool GetPaused()` if property style mismatches the file convention) — pure read accessor, no behavioral change. The property reads `_paused` (the existing volatile field). This is a small enabling edit at the same Application-layer boundary.

8. **Public-vs-internal hook accessibility.** `ModMenuController` is `internal sealed`. Callers in `DualFrontier.Modding.Tests` and `DualFrontier.Presentation` access via `InternalsVisibleTo`. The new fields are typed `Action?` (System.Action) — System.Action is `public`, so the fields can be `internal` safely. Use `internal` accessibility on the new fields, matching the surrounding class accessibility.

9. **No changes to existing tests.** The 3 prior `MenuFlow_*` tests in `GameBootstrapIntegrationTests.cs` (added in M7.5.B.2 commit 2) exercise controller logic without observing `GameLoop` state — they continue to pass unchanged. The new tests are additional, not replacements.

10. **METHODOLOGY §2.4 atomic phase review** — implementation, tests, ROADMAP closure all in one session. Three commits per §7.3 (`fix → test → docs`).

## Required reading

1. `src/DualFrontier.Application/Loop/GameLoop.cs` — full file. Confirms `_paused`, `SetPaused`, run loop check at line ~80. The fix needs to add an `IsPaused` getter here.
2. `src/DualFrontier.Application/Loop/GameBootstrap.cs` — full file. Locate `CreateLoop` method and find the lines after `loop` and `controller` are both in scope; this is where the hook wiring goes.
3. `src/DualFrontier.Application/Modding/ModMenuController.cs` — full file. Locate `BeginEditing`, `Cancel`, `Commit` methods; identify exact insertion points for hook invocations relative to `_pipeline.Pause()` / `_pipeline.Resume()` calls.
4. `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` — XML doc on `_isRunning` field for context that the two flags are independent (already understood, but verify nothing changed since baseline `0b11d1f`).
5. `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — full file. Confirm the existing 3 `MenuFlow_*` tests; identify the test class style (FluentAssertions per M7.5.B.2 closure report). New tests follow the same style.
6. `docs/architecture/MOD_OS_ARCHITECTURE.md` §9.2 wording — quote the exact spec line in the ROADMAP closure entry. Confirms whether the two-flag implementation is a §9.2 ambiguity worth flagging as v1.6 candidate.
7. `docs/ROADMAP.md` — locate the M7.5.B.2 entry and its follow-up housekeeping note (added in commit `0b11d1f`); this brief appends a second follow-up note. Locate the §9 ratification candidates section if one exists; if not, the closure entry text covers it.
8. `docs/methodology/METHODOLOGY.md` §2.4, §7.3.

Pre-flight verification commands:

```
# Confirm baseline state.
git log --oneline -1                                # expect 0b11d1f
dotnet test                                         # expect 434/434 passing

# Confirm current behavior (will be fixed by this commit).
# Look at the relevant code lines.
grep -n "OnEditingBegan\|OnEditingEnded" src/        # expect zero matches (fields don't exist yet)
grep -n "_paused\|SetPaused" src/DualFrontier.Application/Loop/GameLoop.cs
grep -n "_isRunning\|Pause\(\)\|Resume\(\)" src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs
```

## Implementation

### Commit 1 — `fix(application): menu actually pauses simulation via lifecycle hooks`

#### Edit 1.1: `src/DualFrontier.Application/Modding/ModMenuController.cs`

Add two new fields right after the existing private fields (`_pipeline`, `_discoverer`, `_isEditing`, etc.):

```csharp
/// <summary>
/// Optional hook fired by <see cref="BeginEditing"/> AFTER the pipeline
/// is paused, on the transition from "not editing" to "editing".
/// Idempotent re-entry (a second BeginEditing call while already
/// editing) does NOT re-fire the hook — the no-op early return path
/// skips both the pipeline call and the hook.
///
/// Wired by <see cref="GameBootstrap.CreateLoop"/> to also pause the
/// background simulation thread (<c>GameLoop.SetPaused(true)</c>) per
/// MOD_OS_ARCHITECTURE §9.2 step 1, since
/// <see cref="ModIntegrationPipeline.Pause"/> only gates the
/// Apply-mutation safety flag and does not affect tick advance.
///
/// Hook exceptions are caught and swallowed inside the controller —
/// hook failures must not prevent the menu lifecycle from completing.
/// Tests not exercising the simulation pause leave the field
/// <see langword="null"/>; the null path is a no-op.
/// </summary>
internal Action? OnEditingBegan { get; set; }

/// <summary>
/// Optional hook fired by <see cref="Cancel"/> and by the
/// success-path branch of <see cref="Commit"/>, AFTER the pipeline is
/// resumed, on the transition from "editing" to "not editing". The
/// failure-path branch of <see cref="Commit"/> does NOT fire this
/// hook — failed commit leaves the session open per AD #4 of M7.5.A,
/// and the simulation must stay paused so the user can fix the
/// pending state and retry.
///
/// Symmetric counterpart to <see cref="OnEditingBegan"/>; same
/// swallow-exceptions discipline.
/// </summary>
internal Action? OnEditingEnded { get; set; }
```

In `BeginEditing()`, after the line `_isEditing = true;` (which is the last line of the method), add:

```csharp
RaiseHook(OnEditingBegan);
```

In `Cancel()`, after the line `_pipeline.Resume();` (last line of method), add:

```csharp
RaiseHook(OnEditingEnded);
```

In `Commit()`, in the success-path branch, after the line `_pipeline.Resume();` and BEFORE the line `return new CommitResult(...);`, add:

```csharp
RaiseHook(OnEditingEnded);
```

The failure-path branch (the final `return` in the method, after `// Failure path (AD #4)` comment) gets NO hook fire.

Add the helper at the bottom of the class (immediately before the closing brace):

```csharp
private static void RaiseHook(Action? hook)
{
    if (hook is null) return;
    try { hook(); }
    catch { /* §9.5.1-style swallow: lifecycle cannot be derailed by hook callbacks. */ }
}
```

#### Edit 1.2: `src/DualFrontier.Application/Loop/GameLoop.cs`

Add a property right after the existing public methods (`Start`, `Stop`, `SetPaused`, `SetSpeed`):

```csharp
/// <summary>
/// True iff the simulation thread is currently sleeping the tick
/// advance per <see cref="SetPaused"/>. Read-only observation surface
/// for tests and diagnostics; production code mutates state via
/// <see cref="SetPaused"/>.
/// </summary>
public bool IsPaused => _paused;
```

#### Edit 1.3: `src/DualFrontier.Application/Loop/GameBootstrap.cs`

Locate `CreateLoop` method. After the lines that construct `loop` and `controller`, and BEFORE the `return new GameContext(loop, controller);` line, add the wiring:

```csharp
// MOD_OS_ARCHITECTURE §9.2 step 1 — menu opens => simulation pauses.
// Pipeline.Pause() (already called by controller.BeginEditing) only
// gates the Apply-mutation safety flag; the tick-advance gate is
// GameLoop._paused. Wire both surfaces here at the orchestration
// layer that owns both the loop and the controller.
controller.OnEditingBegan = () => loop.SetPaused(true);
controller.OnEditingEnded = () => loop.SetPaused(false);
```

If `loop` and `controller` are not in the same lexical scope at any point in `CreateLoop` — which would be unusual given they end up in the same `GameContext` record — find the smallest scope where both are visible and place the wiring there. Verify by reading the full method.

#### Build + test verification (after Edit 1.1, 1.2, 1.3)

```
dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj
dotnet test
```

Expected: 434/434 passing (no new tests yet). The existing 3 `MenuFlow_*` tests do not observe `GameLoop` state, so they pass unchanged.

Commit message body:

> M7.5.B.2 manual F5 verification surfaced that the simulation kept ticking with the mod menu open: TICK counter advanced ~250 ticks across two screenshots, pawns kept wandering. Per MOD_OS_ARCHITECTURE v1.5 §9.2 step 1 the menu must pause the scheduler — but the implementation had two independent pause flags (ModIntegrationPipeline._isRunning gating Apply mutation safety, GameLoop._paused gating tick advance) and ModMenuController only toggled the first. Add OnEditingBegan/OnEditingEnded hooks on the controller (default null; tests not observing pause state leave them null), wire them in GameBootstrap.CreateLoop to also call GameLoop.SetPaused. Add GameLoop.IsPaused getter for test observation. Failed-commit stays paused per M7.5.A AD #4. The two-flag separation is a §9.2 wording-vs-implementation gap; ROADMAP closure flags it as a v1.6 ratification candidate.

### Commit 2 — `test(bootstrap): menu lifecycle pauses and resumes simulation`

#### Edit 2.1: `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs`

Add 3 new `[Fact]` methods. Match the existing FluentAssertions style of the surrounding class:

```csharp
[Fact]
public void MenuFlow_BeginEditing_PausesGameLoop()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);

    context.Loop.IsPaused.Should().BeFalse(
        "loop is unpaused at construction");

    context.Controller.BeginEditing();

    context.Loop.IsPaused.Should().BeTrue(
        "BeginEditing fires OnEditingBegan which calls loop.SetPaused(true) " +
        "per MOD_OS_ARCHITECTURE §9.2 step 1");
}

[Fact]
public void MenuFlow_Cancel_ResumesGameLoop()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    context.Controller.BeginEditing();
    context.Loop.IsPaused.Should().BeTrue(
        "loop should be paused after BeginEditing (precondition for this test)");

    context.Controller.Cancel();

    context.Loop.IsPaused.Should().BeFalse(
        "Cancel fires OnEditingEnded which calls loop.SetPaused(false)");
}

[Fact]
public void MenuFlow_CommitSuccess_ResumesGameLoop()
{
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    context.Controller.BeginEditing();
    context.Loop.IsPaused.Should().BeTrue(
        "loop should be paused after BeginEditing (precondition for this test)");

    CommitResult result = context.Controller.Commit();

    result.Success.Should().BeTrue(
        "no toggles applied — Commit on a clean session is a no-op success " +
        "per M7.5.A test 16");
    context.Loop.IsPaused.Should().BeFalse(
        "successful Commit fires OnEditingEnded which calls loop.SetPaused(false)");
}
```

If the existing test file has different `using` directives required for `CommitResult`, ensure they're present. The class already imports `DualFrontier.Application.Loop`, `DualFrontier.Application.Modding`, `DualFrontier.Application.Bridge`, and `FluentAssertions` per M7.5.B.2 work — verify before adding.

#### Build + test verification

```
dotnet test
```

Expected: 437/437 passing. The 3 new tests pass; existing tests unchanged.

Commit message body:

> Three integration tests at GameContext level verifying that menu open pauses the simulation thread, that Cancel resumes it, and that successful Commit resumes it. Failed-commit-stays-paused implicitly covered by AD #2 of the implementation (failed-path Commit doesn't fire OnEditingEnded) plus M7.5.A's existing Commit_ValidationFailure_LeavesSessionOpen — adding a fourth test for that path here would parallel that M7.5.A coverage. Tests use FluentAssertions matching the surrounding test class style.

### Commit 3 — `docs(roadmap): close menu-pause wiring; flag §9.2 v1.6 candidate`

#### Edit 3.1: `docs/ROADMAP.md`

Update header status line:

```
*Updated: 2026-05-02 (housekeeping — menu lifecycle now actually pauses simulation per §9.2 step 1; ModMenuPanel position fix landed; UI redesign with Kenney+Cinzel pending; M7-closure pending; .sln gap fix pending; Phase 5 backlog tracked).*
```

Engine snapshot tests count: 434 → 437.

Append to the M7.5.B.2 closure entry (after the existing follow-up housekeeping note added in `0b11d1f`):

```
- **Second follow-up housekeeping** (commits `<sha-1>`, `<sha-2>`, `<sha-3>`):
  - Menu open now actually pauses the background simulation thread.
    Surface gap diagnosed during F5 verification: TICK counter advanced
    ~250 ticks while menu held open across two screenshots; root cause
    was two independent pause flags (`ModIntegrationPipeline._isRunning`
    for Apply-mutation safety, `GameLoop._paused` for tick advance) where
    `ModMenuController.BeginEditing` only toggled the former. Fix wires
    `OnEditingBegan` / `OnEditingEnded` hooks on the controller from
    `GameBootstrap.CreateLoop` so the orchestration layer calls
    `GameLoop.SetPaused` in lockstep with `pipeline.Pause`/`Resume`.
    Failed-commit stays paused per M7.5.A AD #4. Three new integration
    tests lock the contract.
  - **§9.2 v1.6 ratification candidate flagged.** Spec wording «menu
    pauses the scheduler» reads as a single entity; implementation has
    two distinct pause surfaces. The orchestration-layer wiring fix is
    correct given current architecture, but the spec section should
    either explicitly enumerate the two surfaces or refactor toward a
    unified pause contract — to be addressed in M7-closure or its own
    ratification cycle.
```

If the ROADMAP has a dedicated «§9 ratification candidates» list, add the §9.2 entry there as well.

Commit message body:

> Closure for the second M7.5.B.2 follow-up. Surface diagnosis recorded so future readers don't waste time rediscovering the two-pause-flag situation. The §9.2 v1.6 candidate flag is the explicit hypothesis-falsification result: the M-cycle hypothesis predicted M7.5.B.2 closure would be the eleventh consecutive zero post-M4 (no spec contradiction); F5 manual verification falsified that — §9.2 has a real wording-vs-implementation gap. M-closure session resolves the v1.6 cycle.

## Acceptance criteria

1. `dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj` clean — 0 warnings, 0 errors.
2. `dotnet test` count: **434 → 437/437 passing**.
3. `ModMenuController` has `OnEditingBegan` and `OnEditingEnded` `internal Action?` properties; both are fired (via `RaiseHook`) at the documented points; `RaiseHook` swallows callback exceptions.
4. `GameLoop` has an `IsPaused` getter returning `_paused`.
5. `GameBootstrap.CreateLoop` wires both hooks to call `loop.SetPaused(true/false)` in lockstep with the controller's lifecycle. Wiring lines reference §9.2 in a comment.
6. M-phase boundary preserved: `git diff 0b11d1f..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
7. M7.x + previous housekeeping suites all green (ManifestRewriter 7, Persistence 4, Systems 19, Core 61, Modding 343 + 3 new = 346).
8. `dotnet sln list` count unchanged.
9. **Manual F5 verification deferred to user.** Predicted observations:
    - F5 launches as before (10 pawns, TICK counter, mood/needs UI all rendering).
    - Pressing **F10** opens the centered modal AND simulation freezes — TICK counter stops advancing, pawn sprites stop moving, needs bars stop changing.
    - Pressing **F10 again** (or Cancel) closes the modal AND simulation resumes — TICK advances, pawns resume motion, needs continue evolving.
    - Pressing **Apply** on an empty/unchanged session closes the modal AND simulation resumes (success path).
    - **ESC** continues to quit the game (`InputRouter` unchanged).

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj && dotnet test`:

**1.** `fix(application): menu actually pauses simulation via lifecycle hooks`

- Edit 1.1: `ModMenuController.cs` — add `OnEditingBegan`, `OnEditingEnded`, `RaiseHook`, fire at documented points.
- Edit 1.2: `GameLoop.cs` — add `IsPaused` getter.
- Edit 1.3: `GameBootstrap.cs` — wire hooks in `CreateLoop`.
- Build + test verification.
- Commit message body per §Commit 1.

**2.** `test(bootstrap): menu lifecycle pauses and resumes simulation`

- Edit 2.1: 3 new `[Fact]` methods in `GameBootstrapIntegrationTests.cs`.
- Build + test verification (expect 437/437).
- Commit message body per §Commit 2.

**3.** `docs(roadmap): close menu-pause wiring; flag §9.2 v1.6 candidate`

- Edit 3.1: `ROADMAP.md` header, snapshot count, follow-up entry, §9.2 v1.6 candidate flag.
- Build + test verification (no code change but sanity check).
- Commit message body per §Commit 3.

**Special verification preamble:**

After commit 1: `dotnet test` — must pass at 434/434 (no new tests yet). If any of the existing 3 `MenuFlow_*` tests breaks, the controller-level hook firing logic has a regression in the existing API surface; STOP and investigate. The hook fields default null and the existing tests don't set them, so the existing tests should continue to pass.

After commit 2: `dotnet test --filter "FullyQualifiedName~MenuFlow"` — 6 tests green (3 existing + 3 new). Full suite at 437/437.

After commit 3: ROADMAP renders cleanly; second follow-up note appended; §9.2 v1.6 candidate flagged.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice.

**Hypothesis-falsification clause:**

This brief explicitly **falsifies** the M-cycle hypothesis at M7.5.B.2 closure. The closure report from M7.5.B.2 claimed «§9 contradiction status: zero» — F5 manual verification has surfaced a §9.2 wording-vs-implementation gap that the closure missed because it was a presentation-layer issue invisible to controller-level integration tests.

**Updated datapoint sequence (post-this-commit):**
- M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0, M7.4=0, M7.5.A=0, M7.5.B.1=0, M7.5.B.2=**1** (was 0; revised after F5).

The hypothesis "10 consecutive zeros post-M4" → **revised** to "10 consecutive zeros post-M4 then a 1 at M7.5.B.2". The ratification candidate is §9.2 (wording vs. implementation: spec says one entity, implementation has two). M7-closure session formalizes the v1.6 cycle.

Plausible non-spec frictions for this brief:

(a) **Scope of `OnEditingBegan` field setter accessibility.** If `internal { get; set; }` causes accessibility-mismatch warnings against callers in `DualFrontier.Modding.Tests` or `DualFrontier.Presentation`, switch to `Set` methods or to `internal Action? OnEditingBegan` field-with-no-property. The `InternalsVisibleTo` chain on `DualFrontier.Application.csproj` should make this fine.

(b) **Order of pipeline call vs. hook fire in `Commit` failure path.** If the failure-path branch is restructured during this commit such that `RaiseHook(OnEditingEnded)` could accidentally fire on failure, the regression would be silent (failed commit would resume simulation despite session staying open). Verify the success-path-only placement carefully.

(c) **Multiple subscribers to `OnEditingBegan`.** The `Action?` field-style API supports `+=` accumulation, but the brief documents single-assignment usage. If a future test or production caller adds a second handler, both fire. Defensive: not a concern at M7.5.B.2 scope, but worth a note. The `RaiseHook` helper handles a multicast Action correctly anyway (delegate invocation walks the invocation list).

(d) **Test ordering / shared state.** The 3 new tests each construct their own `PresentationBridge` and `GameContext` — no shared state. But `GameBootstrap.CreateLoop` may have side effects (e.g., on the `ModRegistry` singleton if one exists). Verify by reading. The existing 3 `MenuFlow_*` tests are the precedent — if they isolated correctly, these will too.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (434 → 437 expected, or actual).
- 3 new tests by name (`MenuFlow_BeginEditing_PausesGameLoop`, `MenuFlow_Cancel_ResumesGameLoop`, `MenuFlow_CommitSuccess_ResumesGameLoop`) — all green.
- Existing 3 `MenuFlow_*` tests confirmed still green without modification.
- M7.x + housekeeping regression — all suites green.
- Working tree state: clean.
- M-phase boundary: `git diff 0b11d1f..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- Solution file: `dotnet sln list` count unchanged.
- ROADMAP: confirm second follow-up note appended; §9.2 v1.6 candidate flagged.
- Manual F5 verification: deferred to user with predicted observations (TICK freezes when menu open, resumes on close).
- Any API adaptations from brief: flag if `Action?` field accessibility needed adjustment, if hook wiring location moved within `CreateLoop`, etc.
- §9 contradiction status: **one (§9.2 wording vs. implementation)** — flagged for v1.6 ratification.
- Any unexpected findings.
