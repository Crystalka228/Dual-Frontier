# M7-cycle housekeeping — TICK display wiring + Phase 4/5 backlog establishment

## Context

M7.5.B.1 closed (commits `4956a13`, `94128be`, `b6b6d7e`). 415/415 tests passing. Working tree clean. F5 production launch verified clean by user 2026-05-02 — scene loads, ECS init successful, modding stack constructed silently, simulation ticks (verified empirically: NeedsSystem decays internal values from initial 0.1 toward 0 floor over SLOW ticks, MoodSystem reads decayed values, PawnStateReporterSystem publishes inverted display values to HUD, all consistent).

**Two pre-existing UI artifacts surfaced during F5 verification, neither caused by M7.5.B.1:**

1. **TICK: 0 frozen.** `GameHUD.SetTick(int)` defined since Phase 4 (`_colony.SetTick(tick)`) but never invoked. `RenderCommandDispatcher.Dispatch` handles only `PawnSpawned`, `PawnMoved`, `PawnDied`, `PawnState` — no tick-related command. The TickScheduler's monotonically incrementing `CurrentTick` value is never published to the bridge. Pure dead UI plumbing — small commit closes this.

2. **Needs at 100% green steady state.** `NeedsSystem` decays `needs.Hunger` toward 0 (initial 0.1 → 0 in ~50 SLOW ticks). `MoodSystem` formula `mood = 1 - avg(needs)` treats high `needs.X` as "deficit" (low = satisfied). `PawnStateReporterSystem` inverts before publish: `Hunger = 1f - needs.Hunger`. UI's `StatusColor` returns green for high values. Combined: a pawn with internal `Hunger = 0` (decayed floor) displays as "100% green Hunger" — semantically meaning "fully satiated, no deficit". This is **internally consistent design** at the level of NeedsSystem/MoodSystem/Reporter/UI — but the **decay direction is wrong** for actual gameplay (pawns should grow hungrier without food, not become spontaneously more satisfied). Real fix requires Phase 5 eat/drink/sleep job mechanics — without those, flipping the decay sign would have pawns starve to death immediately. **Defer to Phase 5 backlog.**

This housekeeping session: **TICK display fix only.** Document remaining Phase 3.5/4/5 carry-forward debts in a new ROADMAP "Backlog" section so they don't get lost. No M-cycle phase-naming; standalone bugfix commit pattern parallel to `f4b2cb8` (CODING_STANDARDS update).

## Out of scope (Phase 5 / future work — NOT in this session)

- **NeedsSystem decay direction** — wrong sign for real gameplay; flip requires Phase 5 eat/drink/sleep job logic to be wired so pawns can recover. Backlog entry only.
- **`needs.Hunger` field name vs display semantic** — HUD label "Hunger 100%" alongside internal `needs.Hunger = 0` is confusing. Cosmetic; rename `NeedsComponent.Hunger → NeedsComponent.Satiety` (and Thirst → Hydration, Rest → Energy) eventually. Ripples to NeedsSystem, MoodSystem, PawnStateReporterSystem, possibly save format, mod contracts. Defer.
- **`BuildMenu.cs` stub** — `// TODO: Phase 3 — derive from Godot.Control once GodotSharp is wired in.`. Empty class waiting for Phase 5 build mode. Backlog only.
- **M7.5.B.2 Godot UI scene** — separate session immediately following this housekeeping closure.
- **Modifications to `DualFrontier.Core` or `DualFrontier.Contracts`** — M-phase boundary discipline preserved through M3–M7.5.B.1. The bug fix lives entirely in Application + Presentation. Verified via `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returning empty.

## Approved architectural decisions

1. **Mechanism: `TickAdvancedCommand` published to bridge per simulation tick.** Matches existing render-command pattern (PawnSpawned, PawnMoved, PawnDied, PawnState). Single sealed record `(int Tick) : IRenderCommand`. `GameLoop.RunLoop` publishes after each `_scheduler.ExecuteTick(FixedDelta)`. `RenderCommandDispatcher.Dispatch` routes to `_hud?.SetTick(c.Tick)`. Thread safety handled by the existing `PresentationBridge` concurrent queue — no need for atomic reads or locks.

2. **`GameLoop` gains `TickScheduler` reference for tick-value access.** Bootstrap creates `TickScheduler` once (existing) and now passes it to `GameLoop` constructor in addition to scheduler + bridge. GameLoop holds it as `_ticks` field and reads `_ticks.CurrentTick` for the publish payload. Both are `internal` types within `DualFrontier.Application` access scope (TickScheduler in Core, but Core has `[InternalsVisibleTo("DualFrontier.Application")]` already).

3. **Publish frequency: every tick.** At `TargetTps = 30f`, this adds ~30 commands/sec to the bridge. Negligible vs existing PawnState publishing per pawn per SLOW tick. UI updates at Godot frame rate (~60 FPS) drain commands; tick label updates whenever a TickAdvancedCommand arrives. No throttling, no tick-skipping.

4. **`int Tick` payload, not `long`.** UI label `_tickLabel.Text = $"TICK: {tick}"` accepts `int`. `TickScheduler.CurrentTick` is `long` but realistic game sessions won't reach `int.MaxValue` (over 800 days of continuous play at 30 TPS). Cast at publish point. If ever needed, widen the payload — backwards-incompatible change handled via standard ROADMAP entry, not now.

5. **No new test infrastructure.** Existing `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` from M7.5.B.1 is the natural home for one new test verifying TickAdvancedCommand publishing. No separate Bridge.Tests project. No mocking of Godot HUD — that's manually verified post-commit via F5.

6. **ROADMAP "Backlog" section** introduced as new top-level section after M-phase status block. Holds Phase 3.5/4/5 carry-forward items not on M-cycle path: needs decay direction, display semantic rename, BuildMenu stub. Each entry has a one-line description + which Phase will properly address. Future housekeeping commits add to this section as new debts surface.

7. **Commit style: standalone bugfix.** No "M7.X" phase numbering. Three commits parallel to commit `f4b2cb8` (CODING_STANDARDS) pattern: `fix(presentation):` for the TICK wiring, `test(presentation):` for the test, `docs(roadmap):` for the backlog section establishment.

8. **METHODOLOGY §2.4 atomic phase review** — implementation, tests, ROADMAP update all in one session. Three commits per §7.3.

## Required reading

1. `docs/MOD_OS_ARCHITECTURE.md` LOCKED v1.5 — not directly relevant to this housekeeping fix; bug is in presentation layer, not modding spec. Read only if any §-anchored question surfaces during execution.
2. `docs/ROADMAP.md` — locate the M7 sub-phase status block. The new "Backlog" section will sit after the M-phase status block and before any closure-review citations. Examine the document structure for where best to insert.
3. `docs/METHODOLOGY.md` — §2.4 atomic phase review, §7.3 three-commit invariant.
4. `docs/CODING_STANDARDS.md` — full document. Especially: one class per file, English-only comments, member order, `_camelCase` private fields.
5. Code (full files):
   - `src/DualFrontier.Application/Bridge/Commands/PawnStateCommand.cs` — pattern reference for the new `TickAdvancedCommand` record (sealed record, `IRenderCommand` interface, `Execute(object)` placeholder).
   - `src/DualFrontier.Application/Bridge/IRenderCommand.cs` — interface to confirm shape.
   - `src/DualFrontier.Application/Loop/GameLoop.cs` — `RunLoop` is the publish site. Read full body — the `_scheduler.ExecuteTick(FixedDelta)` call inside the accumulator loop is where TickAdvancedCommand publish belongs.
   - `src/DualFrontier.Application/Loop/GameBootstrap.cs` — the `var ticks = new TickScheduler();` line + `new GameLoop(scheduler, bridge)` line; constructor call needs the new `ticks` parameter.
   - `src/DualFrontier.Core/Scheduling/TickScheduler.cs` — `public long CurrentTick` getter is the read source. Note `internal sealed class` — accessible only via existing `InternalsVisibleTo` from Core to Application.
   - `src/DualFrontier.Presentation/Rendering/RenderCommandDispatcher.cs` — `Dispatch(IRenderCommand)` switch needs new case for `TickAdvancedCommand`.
   - `src/DualFrontier.Presentation/UI/GameHUD.cs` — `public void SetTick(int tick) => _colony.SetTick(tick);` — the orphaned method that becomes the dispatcher case target.
   - `src/DualFrontier.Presentation/UI/ColonyPanel.cs` — `public void SetTick(int tick) => _tickLabel.Text = $"TICK: {tick}";` — UI label sink.
6. M7.5.B.1 test pattern:
   - `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — the new test joins this file as one additional `[Fact]`.

## Implementation

### 1. New `TickAdvancedCommand` record

`src/DualFrontier.Application/Bridge/Commands/TickAdvancedCommand.cs`:

```csharp
namespace DualFrontier.Application.Bridge.Commands;

/// <summary>
/// Command: the simulation tick counter has advanced to <paramref name="Tick"/>.
/// <see cref="GameLoop"/> publishes one of these onto the
/// <see cref="PresentationBridge"/> after every successful
/// <c>ParallelSystemScheduler.ExecuteTick</c> call inside its fixed-step
/// accumulator loop. The presentation layer drains the command on the
/// Godot main thread and forwards <paramref name="Tick"/> to the HUD's
/// tick label.
/// </summary>
/// <param name="Tick">
/// Current value of <c>TickScheduler.CurrentTick</c> at publish time.
/// Cast from <c>long</c> to <c>int</c> at the publish site — UI labels
/// accept <c>int</c> and realistic sessions stay well below
/// <c>int.MaxValue</c>.
/// </param>
public sealed record TickAdvancedCommand(int Tick) : IRenderCommand
{
    /// <inheritdoc />
    public void Execute(object renderContext)
    {
        /* No-op — same Phase-5 IRenderer placeholder pattern as the other
         * commands. Actual routing happens through
         * RenderCommandDispatcher.Dispatch's switch arm. */
    }
}
```

### 2. `GameLoop` accepts `TickScheduler` and publishes per tick

Modify `src/DualFrontier.Application/Loop/GameLoop.cs`:

```csharp
private readonly ParallelSystemScheduler _scheduler;
private readonly TickScheduler _ticks;        // new
private readonly PresentationBridge _bridge;
// ... existing fields ...

public GameLoop(ParallelSystemScheduler scheduler,
                TickScheduler ticks,           // new parameter
                PresentationBridge bridge)
{
    _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    _ticks = ticks ?? throw new ArgumentNullException(nameof(ticks));
    _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
}
```

Inside `RunLoop`, in the accumulator inner loop:

```csharp
while (accumulator >= FixedDelta)
{
    _scheduler.ExecuteTick(FixedDelta);
    _bridge.Enqueue(new TickAdvancedCommand((int)_ticks.CurrentTick));  // new
    accumulator -= FixedDelta;
}
```

Add `using DualFrontier.Application.Bridge.Commands;` at the top of the file.

XML doc on `GameLoop` class updated to mention TickAdvancedCommand publishing as part of the bridge contract.

### 3. `GameBootstrap` passes `ticks` to GameLoop

Modify `src/DualFrontier.Application/Loop/GameBootstrap.cs` — single line change in `CreateLoop`:

```csharp
// before
var loop = new GameLoop(scheduler, bridge);

// after
var loop = new GameLoop(scheduler, ticks, bridge);
```

XML doc on `CreateLoop` mentions that the `ticks` reference is now threaded into `GameLoop` so the loop can publish `TickAdvancedCommand` per fixed-step tick.

### 4. `RenderCommandDispatcher` routes `TickAdvancedCommand` to HUD

Modify `src/DualFrontier.Presentation/Rendering/RenderCommandDispatcher.cs`:

```csharp
public void Dispatch(IRenderCommand command)
{
    switch (command)
    {
        case PawnSpawnedCommand c:
            _pawnLayer.SpawnPawn(c.PawnId, c.X, c.Y);
            break;
        case PawnMovedCommand c:
            _pawnLayer.MovePawn(c.PawnId, c.X, c.Y);
            break;
        case PawnDiedCommand c:
            _pawnLayer.RemovePawn(c.PawnId);
            break;
        case PawnStateCommand c:
            _hud?.UpdatePawn(c);
            break;
        case TickAdvancedCommand c:    // new
            _hud?.SetTick(c.Tick);
            break;
    }
}
```

No other Presentation-layer changes — `GameHUD.SetTick` already exists, `ColonyPanel.SetTick` already exists.

## Tests

### `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — one new test

Add to existing test class. Mark with `[Trait("Category", "Integration")]` since it requires the loop to actually run for a small interval to observe ticks publish.

```csharp
[Fact]
[Trait("Category", "Integration")]
public void CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge()
{
    // Construct the production context.
    var bridge = new PresentationBridge();
    GameContext context = GameBootstrap.CreateLoop(bridge);
    
    try
    {
        context.Loop.Start();
        // Run for ~200 ms — at 30 TPS that's ~6 fixed-step ticks. Allow
        // generous slop on slow CI: assert >= 2 ticks observed (still
        // strictly proves the wiring; not a timing assertion).
        Thread.Sleep(250);
    }
    finally
    {
        context.Loop.Stop();
    }
    
    int tickCommandCount = 0;
    int lastTickValue = -1;
    bridge.DrainCommands(cmd =>
    {
        if (cmd is TickAdvancedCommand tac)
        {
            tickCommandCount++;
            lastTickValue = tac.Tick;
        }
    });
    
    Assert.True(tickCommandCount >= 2,
        $"Expected at least 2 TickAdvancedCommand publishes, observed {tickCommandCount}");
    Assert.True(lastTickValue >= tickCommandCount - 1,
        $"Expected monotonic tick values, last={lastTickValue}, count={tickCommandCount}");
}
```

The second assertion locks the monotonicity contract — `_ticks.CurrentTick` advances exactly once per `ExecuteTick`, so the last published value must be at least `count - 1` (allowing for tick 0 to be the first publish).

### Out-of-scope tests

- Manual F5 verification of the actual TICK label updating in the running game — user does this post-commit. Brief explicitly requests the user run F5 and confirm `TICK: N` increments visibly in the bottom-left of `ColonyPanel`.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors.
2. `dotnet test` — 415 existing pass; 1 new pass. Expected total: **416/416**.
3. New file `src/DualFrontier.Application/Bridge/Commands/TickAdvancedCommand.cs` exists.
4. `GameLoop` constructor signature: `(ParallelSystemScheduler scheduler, TickScheduler ticks, PresentationBridge bridge)`. The `ticks` parameter is non-null-validated.
5. `GameBootstrap.CreateLoop` passes `ticks` to the GameLoop constructor.
6. `RenderCommandDispatcher.Dispatch` has a `case TickAdvancedCommand c: _hud?.SetTick(c.Tick); break;` arm.
7. `GameLoop.RunLoop` publishes `TickAdvancedCommand((int)_ticks.CurrentTick)` after each `_scheduler.ExecuteTick(FixedDelta)` call inside the accumulator loop.
8. New ROADMAP "Backlog" section exists with three entries:
   - **NeedsSystem decay direction** — Phase 5 (eat/drink/sleep job logic must land first; without it, flipping the sign starves pawns)
   - **`NeedsComponent` field semantic rename** (`Hunger → Satiety`, `Thirst → Hydration`, `Rest → Energy`) — Phase 5 (ripples to MoodSystem, PawnStateReporterSystem, save format, mod contracts; cosmetic naming fix worth doing alongside Phase 5 needs work)
   - **`BuildMenu.cs` stub** — Phase 5 (build mode UI; class is empty and waiting since Phase 3.5)
9. M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
10. `dotnet sln list` unchanged from M7.5.B.1 baseline.
11. **Manual F5 verification documented in commit 1 message** — agent CANNOT run F5 (no Godot binary in terminal), but the integration test in commit 2 exercises the same publishing path. Commit 1 message notes that manual F5 sign-off is the user's responsibility post-commit and lists the specific visual: `TICK: N` label in `ColonyPanel` bottom-left increments visibly during gameplay. M7.5.B.2 pre-flight will start with user confirmation that TICK label works.

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `fix(presentation): wire TickAdvancedCommand to GameHUD tick counter`

- New file `src/DualFrontier.Application/Bridge/Commands/TickAdvancedCommand.cs`.
- Modify `src/DualFrontier.Application/Loop/GameLoop.cs`: new `_ticks` field, signature change, publish in RunLoop accumulator loop, new using.
- Modify `src/DualFrontier.Application/Loop/GameBootstrap.cs`: pass `ticks` to GameLoop constructor.
- Modify `src/DualFrontier.Presentation/Rendering/RenderCommandDispatcher.cs`: new switch arm for `TickAdvancedCommand`.
- Verify existing M7.x suites still pass via `dotnet test --filter "FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~ManifestRewriter|FullyQualifiedName~GameBootstrapIntegration"`.
- Commit message body notes: «Manual F5 verification deferred to user — the integration test added in commit 2 exercises the same publishing path through the bridge. User to confirm `TICK: N` label in `ColonyPanel` bottom-left visibly increments during gameplay before opening M7.5.B.2.»

**2.** `test(presentation): integration test for TickAdvancedCommand bridge publishing`

- Add one new `[Fact]` to `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` named `CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`.
- Marked `[Trait("Category", "Integration")]`.
- Run the full suite. Confirm 416/416.
- Commit message notes the timing-tolerant assertion (>=2 ticks in 250 ms; allows slow CI without flake).

**3.** `docs(roadmap): add Backlog section + housekeeping closure entry`

- `ROADMAP.md`:
   - New "Backlog" top-level section with the three entries listed in AC #8.
   - Header status line: `*Updated: YYYY-MM-DD (housekeeping commit — TICK display wired through TickAdvancedCommand; M7.5.B.2 + M7-closure pending; Phase 5 backlog established).*`
   - Engine snapshot: 415 → 416 tests.
   - Status overview table M7 row tests column extended with `+1 housekeeping` line (no new sub-phase number).
- No M-cycle phase numbering for this commit — it's a standalone bugfix parallel to the CODING_STANDARDS commit pattern (`f4b2cb8`).

**Special verification preamble for commits 1 + 2:**

- After commit 1: `dotnet build && dotnet test --filter "<full M7.x filter from above>"` — no regression.
- After commit 2: `dotnet test --filter "FullyQualifiedName~CreateLoop_RunningLoop"` — 1 new test green. Full suite at 416/416.
- After commit 3: ROADMAP renders cleanly; new Backlog section visible; M7-cycle progression unaffected.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice. Per spec preamble "stop, escalate, lock — never guess".

**Hypothesis-falsification clause:**

Datapoints (per [M6 closure review §10](../audit/M6_CLOSURE_REVIEW.md)): M3=1, M4=1, M5=0, M6=0, M7.1=0, M7.2=0, M7.3=0, M7.4=0, M7.5.A=0, M7.5.B.1=0. **This commit does not increment the datapoint sequence** — it's a Phase 4 housekeeping bugfix, not an M-cycle phase. The sequence resumes at M7.5.B.2.

This commit exercises only Application + Presentation layers (no spec-driven design). The `MOD_OS_ARCHITECTURE` spec doesn't speak to TICK display. No §-anchored ratification candidate is plausible. **If something §-anchored does surface, report immediately.**

The only plausible non-spec friction:

(a) **Test timing flakiness on slow CI.** 250 ms window targets 6 ticks at 30 TPS, asserts >=2. Should be robust, but if observed flake — increase window to 500 ms before declaring §-level issue.

(b) **`InternalsVisibleTo` chain for TickScheduler in test project.** `TickScheduler` is internal in Core; the test reads the scheduler indirectly through the bridge (no direct access needed) — should not be a problem. Flag if compile fails.

(c) **`int` cast overflow concern on `_ticks.CurrentTick`.** Realistic sessions stay well below `int.MaxValue` (800 days at 30 TPS); not an issue for production. Test runs only ~6 ticks; trivially safe.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (415 + 1 = 416 expected, or actual with discrepancy noted).
- Per-test confirmation: 1 new test (`CreateLoop_RunningLoop_PublishesTickAdvancedCommandsThroughBridge`) green; ran reliably across at least 3 consecutive `dotnet test` invocations to verify no timing flake.
- Regression confirmation: M7.1 (11) + M7.2 (13) + M7.3 (5+2) + M7.4 (9) + M7.5.A (30) + M7.5.B.1 (7) + remaining M0–M6 + Persistence + Systems + Core all still green.
- Working tree state: clean / dirty.
- **No spec contradiction status** (this commit doesn't engage with MOD_OS_ARCHITECTURE; report immediately if any §-anchored question arises during execution).
- **Manual verification of TICK label deferred** to user (cannot launch F5 from terminal); commit 1 message documents this.
- **M-phase boundary discipline**: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` = empty.
- **Solution file**: `dotnet sln list` count unchanged.
- **ROADMAP Backlog section**: confirm three entries present (NeedsSystem decay, field semantic rename, BuildMenu stub) with target Phase noted.
- Any unexpected findings, especially any timing flake on the new integration test, or any §-anchored question that surfaced.
