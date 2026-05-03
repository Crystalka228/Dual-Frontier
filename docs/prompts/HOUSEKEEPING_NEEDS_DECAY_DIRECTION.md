# Honest needs decay — flip direction (deficit accumulates without recovery)

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

User's strict application 2026-05-02:

> «Сейчас в проекте нет ни одного модуля и вещей закрываюших потребности, по этому они не могут закрываться сами собой.»

The current code:

```csharp
needs.Hunger = Math.Clamp(needs.Hunger - HungerDecayPerTick, 0f, 1f);
```

implicitly asserts that needs decrease over time. NeedsComponent's own XML doc says `0 = full, 1 = starving` — high is bad. So decay-toward-0 means becoming-satiated-over-time. But:

- No food entities exist in the world
- No EatSystem exists that consumes food and resets `Hunger`
- No DrinkSystem, no SleepSystem
- `JobSystem.PickJob` ASSIGNS `JobKind.Eat` when `Hunger >= CriticalThreshold` (already wired!) but no execution layer actually eats and resets the need

The current decay direction therefore describes a recovery mechanism that does not exist. By the user's principle this is a placeholder that lies about state. The fix is to flip the decay sign — needs accumulate as deficits over time, with no recovery, until Phase 5 introduces real food/water/sleep mechanics. Pawns will visibly degrade. That is the honest state of an incomplete simulation.

## What changes after the flip (predicted gameplay behaviour)

- Pawns spawn with initial Hunger=0.1, Thirst=0.1, Rest=0.1, Comfort=0
- NeedsSystem (SLOW = 60 ticks ≈ 2s at 30 TPS) increments each need by its decay constant per tick
- Hunger reaches 0.8 (CriticalThreshold) after `(0.8-0.1)/0.002 = 350` SLOW updates = ~12 min gameplay
- `NeedsCriticalEvent` fires (already implemented)
- `JobSystem.OnNeedsCritical` adds pawn to `_urgentPawns` (already implemented)
- `JobSystem.Update` calls `PickJob`, returns `JobKind.Eat`, sets `JobComponent.Current = JobKind.Eat` (already implemented)
- `PawnStateReporterSystem` publishes `JobLabel = "Foraging"`, `JobUrgent = true` (already implemented)
- UI shows "Foraging" job label with red urgent dot (already implemented)
- NeedsSystem keeps incrementing Hunger because no recovery exists. Hunger climbs from 0.8 toward 1.0
- Display "Hunger" bar drops from 20% (critical red) toward 0% (empty red)
- MoodSystem averages needs deficit, mood drops from 0.5 toward 0
- At mood < 0.3 (default break threshold), MoodBreakEvent fires (no handler — UI shows "On the brink" mood label)
- Pawn stays in `JobKind.Eat` indefinitely; nothing resets the deficit

This is honest "incomplete state": the simulation runs, needs grow, jobs trigger correctly, but the recovery loop is missing. Phase 5 is what completes this.

## Audit context

`docs/audit/UI_REVIEW_PRE_M75B2.md` (created 2026-05-02) catalogued this issue as Phase 5 territory; the prior `HOUSEKEEPING_REAL_PAWN_DATA` brief explicitly listed it as "Phase 5 — gameplay completeness" pending. User's clarification reframes it as a placeholder lie that must be removed now even though the recovery half remains Phase 5.

## Out of scope

- No simulation systems modified except `NeedsSystem` (and only the four decay lines + class-level XML doc).
- No `MoodSystem` change (formula `1 - avg(needs)` still correct; mood will now fall as intended instead of rising spuriously).
- No new components or events.
- No food / water / bed entities — those land in Phase 5 along with EatSystem / DrinkSystem / SleepSystem.
- No JobSystem changes — it already does the right thing on `NeedsCriticalEvent`; just needs the event to actually fire, which the flipped decay enables.
- No `MoodBreakEvent` handler — Phase 5.
- No UI changes — current bars + thresholds already render decay correctly when display values fall (they were just never falling before).
- No change to `src/DualFrontier.Core` or `src/DualFrontier.Contracts`. M-phase boundary preserved.

## Approved architectural decisions

1. **Single-file fix scope: `src/DualFrontier.Systems/Pawn/NeedsSystem.cs`.** Four decay-line modifications (one per need), plus class-level XML doc rewrite to reflect the corrected semantics. No other Systems file touched.

2. **Mechanism: flip the sign on every decay assignment.** `needs.Hunger - HungerDecayPerTick` becomes `needs.Hunger + HungerDecayPerTick`, same for Thirst, Rest, Comfort. The `Math.Clamp(value, 0f, 1f)` ceiling clamps deficit at 1.0, matching the field's semantic range.

3. **Initial spawn values unchanged.** RandomPawnFactory and the existing GameBootstrap test fixtures continue to seed Hunger=0.1, Thirst=0.1, Rest=0.1, Comfort=0. After the flip, those starting values represent "freshly satiated, deficit will grow over time" — which is the natural starting state for a colonist.

4. **`CriticalThreshold = 0.8f` and `BreakThreshold = 0.95f` constants unchanged.** They already encode "high deficit triggers urgency" semantics — the flip realigns reality with their semantic meaning.

5. **`_critical` tracking dictionary in NeedsSystem unchanged.** Edge-detect logic (publish event only on the false→true transition) remains correct for accumulating-deficit semantics.

6. **No public API changes.** `Update`, `OnInitialize` signatures and visibility identical. NeedsComponent / MoodSystem / JobSystem / PawnStateReporterSystem / UI bars / display thresholds all interact through the existing field, which now finally moves in the direction its semantic name implies.

7. **`MoodSystem` formula stays `mood = 1 - avg(needs)`.** Comment in MoodSystem already says `0 = bad, 1 = good`. With the flip: deficit rises → average rises → `1 - average` falls → mood falls. Now consistent.

8. **`PawnStateReporterSystem` inversion stays `Hunger = 1f - needs.Hunger`.** Display semantic remains "100% green = no deficit". With the flip: deficit rises → display value falls → bar shrinks → color transitions through Neutral → Bad → Critical thresholds (`StatusColor` in PawnDetail handles this already).

9. **One regression-guard test added to `tests/DualFrontier.Systems.Tests/Pawn/`** asserting decay direction is positive (deficit grows). Locks the contract so a future refactor cannot silently revert.

10. **Existing `NeedsJobIntegrationTests.Starving_pawn_receives_Eat_job_after_NeedsCritical_fires_on_the_Pawns_bus`** continues to pass without modification. The test seeds Hunger=0.9 (already above CriticalThreshold). Both old and new decay directions keep it above threshold across 5 ticks; `NeedsCriticalEvent` fires, JobSystem assigns `JobKind.Eat`, assertion holds.

11. **Manual F5 verification deferred to user.** Predicted observation: over the first ~12 minutes of a fresh F5 launch, all 10 pawns' need bars (Hunger, Thirst, Rest, Comfort) will visibly drop from initial high % toward low %, mood values will fall from ~95 toward lower numbers, eventually pawns transition to `JobLabel = "Foraging"` with red urgent dot. None of these are currently observable.

12. **METHODOLOGY §2.4 atomic phase review** — implementation, test, ROADMAP closure all in one session. Three commits per §7.3 (`fix → test → docs`).

## Required reading

1. `src/DualFrontier.Systems/Pawn/NeedsSystem.cs` — full file. The fix target.
2. `src/DualFrontier.Components/Pawn/NeedsComponent.cs` — confirms field semantic (`0 = full, 1 = starving`).
3. `src/DualFrontier.Systems/Pawn/JobSystem.cs` — confirms NeedsCriticalEvent handling and `PickJob` already wired.
4. `src/DualFrontier.Systems/Pawn/MoodSystem.cs` — confirms `mood = 1 - avg(needs)` formula compatible with flip.
5. `tests/DualFrontier.Systems.Tests/Pawn/NeedsJobIntegrationTests.cs` — confirm it still passes (no fixture change needed).
6. `tests/DualFrontier.Systems.Tests/` — confirm directory structure for adding the new file. Match existing project conventions (xUnit, FluentAssertions if present elsewhere).
7. `docs/ROADMAP.md` Backlog section — locate the "NeedsSystem decay direction" entry under "Phase 5 — gameplay completeness" subsection. Move to "Resolved" subsection on commit 3.
8. `docs/METHODOLOGY.md` §2.4 atomic phase review, §7.3 three-commit invariant.
9. `docs/CODING_STANDARDS.md` — English-only comments, member order.

Pre-flight grep checks before commit 1:
- `grep -rn "HungerDecayPerTick\|ThirstDecayPerTick\|SleepDecayPerTick\|ComfortDecayPerTick" src/ tests/` — should return references in NeedsSystem.cs only. Any other reference would suggest external dependency on these constants.
- `grep -rn "needs\.Hunger\s*-=\|needs\.Hunger\s*-\s*Hunger" src/ tests/` — find any other code that assumes decay direction. Should be only NeedsSystem.

If any test asserts `needs.Hunger < initialValue` after ticks (i.e., asserts decay-toward-0 specifically), it must be updated to match the new direction. If no such test exists, no test changes outside the new file.

## Implementation

### `src/DualFrontier.Systems/Pawn/NeedsSystem.cs` — modify

**Change 1: flip the four decay assignments inside `Update`:**

```csharp
// before
needs.Hunger  = Math.Clamp(needs.Hunger  - HungerDecayPerTick,  0f, 1f);
needs.Thirst  = Math.Clamp(needs.Thirst  - ThirstDecayPerTick,  0f, 1f);
needs.Rest    = Math.Clamp(needs.Rest    - SleepDecayPerTick,   0f, 1f);
needs.Comfort = Math.Clamp(needs.Comfort - ComfortDecayPerTick, 0f, 1f);

// after
needs.Hunger  = Math.Clamp(needs.Hunger  + HungerDecayPerTick,  0f, 1f);
needs.Thirst  = Math.Clamp(needs.Thirst  + ThirstDecayPerTick,  0f, 1f);
needs.Rest    = Math.Clamp(needs.Rest    + SleepDecayPerTick,   0f, 1f);
needs.Comfort = Math.Clamp(needs.Comfort + ComfortDecayPerTick, 0f, 1f);
```

**Change 2: rewrite class-level XML doc** to reflect honest semantics. Replace existing comment-block above `public sealed class NeedsSystem` with:

```csharp
/// <summary>
/// Per-pawn deficit accumulator. Each SLOW tick (~2s at 30 TPS), every need
/// grows by its constant decay rate, capped at 1.0. The semantic per
/// <see cref="NeedsComponent"/> is "0 = full / comfortable, 1 = starving /
/// dehydrated / exhausted / miserable" — high values indicate urgent
/// deficit. <c>NeedsCriticalEvent</c> fires once per false→true transition
/// across <see cref="NeedsComponent.CriticalThreshold"/>; <c>JobSystem</c>
/// subscribes and reassigns the pawn to a recovery job
/// (<c>JobKind.Eat</c>, <c>JobKind.Sleep</c>) when urgent.
///
/// No recovery mechanism exists yet — neither food entities, nor an
/// EatSystem that consumes food and resets <see cref="NeedsComponent.Hunger"/>,
/// nor parallel systems for thirst / sleep / comfort. Pawns therefore
/// degrade indefinitely once spawned. Phase 5 introduces those systems;
/// until then, the displayed need bars truthfully reflect ungrounded
/// accumulation. By the project's operating principle: state either
/// exists or it does not — we do not fake recovery via inverted decay.
/// </summary>
```

The four constants `HungerDecayPerTick = 0.002f`, `ThirstDecayPerTick = 0.0015f`, `SleepDecayPerTick = 0.001f`, `ComfortDecayPerTick = 0.0005f` stay unchanged — they were already calibrated for "deficit accumulation" pacing semantically; the wrong-direction sign was the only bug.

## Tests

### New file: `tests/DualFrontier.Systems.Tests/Pawn/NeedsAccumulationTests.cs`

```csharp
using DualFrontier.Components.Pawn;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Systems.Pawn;
using FluentAssertions;
using Xunit;

namespace DualFrontier.IntegrationTests.PawnDomain;

/// <summary>
/// Locks the post-honesty-pass decay-direction contract: needs grow
/// over time, representing accumulating deficit. The previous (incorrect)
/// direction was decay-toward-0, which falsely implied automatic
/// recovery. See HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md for context.
/// </summary>
public sealed class NeedsAccumulationTests
{
    [Fact]
    public void Hunger_GrowsOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, hunger: 0.5f);

        // Run several SLOW ticks. Each tick, NeedsSystem should
        // increment Hunger by HungerDecayPerTick (0.002).
        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().BeGreaterThan(0.5f,
            "deficit must accumulate without recovery — no module closes needs yet");
    }

    [Fact]
    public void AllFourNeeds_GrowOverTime_WhenNoRecoveryExists()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(
            world, hunger: 0.3f, thirst: 0.3f, rest: 0.3f, comfort: 0.3f);

        for (int i = 0; i < 10; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().BeGreaterThan(0.3f);
        needs.Thirst.Should().BeGreaterThan(0.3f);
        needs.Rest.Should().BeGreaterThan(0.3f);
        needs.Comfort.Should().BeGreaterThan(0.3f);
    }

    [Fact]
    public void Hunger_ClampsAt1_WhenAlreadyAtCeiling()
    {
        var (world, scheduler) = BuildOneNeedsSystem();
        EntityId pawn = SpawnIdleNeedsPawn(world, hunger: 1.0f);

        for (int i = 0; i < 5; i++)
            scheduler.ExecuteTick(1f / 30f);

        world.TryGetComponent<NeedsComponent>(pawn, out NeedsComponent needs)
             .Should().BeTrue();
        needs.Hunger.Should().Be(1.0f, "ceiling clamp must hold at 1.0");
    }

    private static (World world, ParallelSystemScheduler scheduler) BuildOneNeedsSystem()
    {
        var world    = new World();
        var services = new GameServices();
        var ticks    = new TickScheduler();

        var graph = new DependencyGraph();
        graph.AddSystem(new NeedsSystem());
        graph.Build();

        var scheduler = new ParallelSystemScheduler(
            graph.GetPhases(), ticks, world, faultSink: null, services: services);

        return (world, scheduler);
    }

    private static EntityId SpawnIdleNeedsPawn(
        World world,
        float hunger  = 0f,
        float thirst  = 0f,
        float rest    = 0f,
        float comfort = 0f)
    {
        EntityId id = world.CreateEntity();
        world.AddComponent(id, new NeedsComponent
        {
            Hunger = hunger, Thirst = thirst, Rest = rest, Comfort = comfort,
        });
        return id;
    }
}
```

**Implementation notes for the agent:**

- If `World.TryGetComponent<T>` does not exist with that exact signature, adapt to whatever the existing API is. The pre-existing `NeedsJobIntegrationTests.cs` uses `world.TryGetComponent<JobComponent>(pawn, out JobComponent job)` — same shape, should be fine.
- If `NeedsAccumulationTests` namespace conflicts with anything, follow the existing namespace pattern from `NeedsJobIntegrationTests.cs`: `namespace DualFrontier.IntegrationTests.PawnDomain;`.
- The test runs only NeedsSystem in isolation (no JobSystem) — focus is purely on decay direction.
- 10 SLOW ticks at delta=1/30s each. Hunger initial 0.5 → expected 0.5 + 10*0.002 = 0.520. Test asserts `> 0.5` — robust to small math drift.

### Existing test verification

`NeedsJobIntegrationTests.Starving_pawn_receives_Eat_job_after_NeedsCritical_fires_on_the_Pawns_bus` — fixture seeds `Hunger = 0.9f` (above CriticalThreshold 0.8). Both old and new decay directions keep Hunger ≥ 0.8 across the 5 test ticks. NeedsCriticalEvent fires on first SLOW tick, JobSystem assigns Eat. **Test passes unchanged.**

If any test in `tests/` asserts that Hunger / Thirst / Rest / Comfort DECREASES over time (i.e., asserts the now-wrong direction), it must be updated as part of commit 1. Run pre-flight grep `grep -rn "BeLessThan\|< initialValue\|< 0\." tests/DualFrontier.Systems.Tests/Pawn/` to find any such assertion.

## Acceptance criteria

1. `dotnet build` clean — 0 warnings, 0 errors.
2. `dotnet test` count: **428 + 3 new = 431/431** passing. Or report actual count if pre-existing tests had to be updated.
3. `NeedsSystem.Update` body has four `+ DecayPerTick` lines (Hunger, Thirst, Rest, Comfort), no `-` operators in the decay arithmetic.
4. NeedsSystem class-level XML doc rewritten to describe deficit-accumulation semantics with the no-recovery caveat.
5. New test file `tests/DualFrontier.Systems.Tests/Pawn/NeedsAccumulationTests.cs` exists with 3 facts.
6. Existing `NeedsJobIntegrationTests` still passes without modification.
7. M-phase boundary: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` returns empty.
8. M7.x + previous housekeeping suites all green: M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, ManifestRewriter, ModMenuController, DefaultModDiscoverer, PipelineGetActiveMods, GameBootstrapIntegration, TickSchedulerThreadSafety, RandomPawnFactory.
9. `dotnet sln list` count unchanged from prior baseline.
10. **Manual F5 verification deferred to user.** Predicted observation:
    - Over first ~12 minutes of fresh F5 launch, all 10 pawns' need bars visibly drop from initial high % toward critical low %.
    - Mood scores fall from ~95 toward lower values as deficit averages rise.
    - Eventually pawns transition `JobLabel` from "Idle" to "Foraging" with red urgent dot.
    - None of these were observable before this fix.

## Финал

Atomic commits in order. Each commit individually must pass `dotnet build && dotnet test`:

**1.** `fix(systems): NeedsSystem decay direction — deficit accumulates without recovery`

- Modify `src/DualFrontier.Systems/Pawn/NeedsSystem.cs`:
  - Flip the four decay-line signs (Hunger, Thirst, Rest, Comfort).
  - Rewrite class-level XML doc per §Implementation above.
- Verify existing M-cycle and Systems suites still pass via `dotnet test --filter "FullyQualifiedName~NeedsJobIntegration|FullyQualifiedName~M71|FullyQualifiedName~M72|FullyQualifiedName~M73|FullyQualifiedName~M74|FullyQualifiedName~ModMenuController|FullyQualifiedName~DefaultModDiscoverer|FullyQualifiedName~PipelineGetActiveMods|FullyQualifiedName~ManifestRewriter|FullyQualifiedName~GameBootstrapIntegration|FullyQualifiedName~TickScheduler|FullyQualifiedName~RandomPawnFactory"` — all green.
- Commit message body documents the user's framing: «No module currently closes needs in the simulation; therefore needs cannot decrease on their own. Decay-toward-0 was a placeholder lie about non-existent recovery. Flipped to deficit accumulation; pawns now degrade visibly until Phase 5 lands food/water/sleep mechanics.»

**2.** `test(systems): regression guard for needs-accumulation direction`

- New file `tests/DualFrontier.Systems.Tests/Pawn/NeedsAccumulationTests.cs` with 3 facts.
- Run full suite. Confirm 431/431.

**3.** `docs(roadmap): close needs decay direction; update Phase 5 backlog`

- `docs/ROADMAP.md`:
  - Header status line: `*Updated: YYYY-MM-DD (housekeeping — needs decay direction flipped to honest accumulation; M7.5.B.2 + M7-closure pending; Phase 5 recovery loop still pending).*`
  - Engine snapshot: 428 → 431 tests.
  - Backlog: move "NeedsSystem decay direction" entry from "Phase 5 — gameplay completeness" subsection to "Resolved" subsection. Reword the resolved entry:
    > **NeedsSystem decay direction** — was decay-toward-0 (falsely implying automatic recovery); flipped to deficit accumulation in housekeeping commit `<sha-1>`. Recovery loop (food / water / bed entities + Eat/Drink/Sleep job execution) remains pending in Phase 5 — until then pawns visibly degrade indefinitely, which is the honest behaviour.
  - Add new entry under "Phase 5 — gameplay completeness" subsection:
    > **Recovery loop for accumulating needs** — food / water / bed entities, EatSystem / DrinkSystem / SleepSystem that consume them and reset corresponding `NeedsComponent` field. JobSystem already assigns `JobKind.Eat` / `Sleep` on `NeedsCriticalEvent`; missing layer is execution. After this lands, NeedsAccumulationTests will need adjustment (or supplemental tests) to cover the recovery side.
  - Add new entry under same subsection:
    > **MoodBreakEvent handler** — currently fires from MoodSystem when `mind.Mood < MindComponent.DefaultBreakThreshold (0.3)` and has no subscriber. Phase 5 introduces breakdown behaviour (run away, fight, idle-stupor, etc.).

**Special verification preamble:**

After commit 1: full `dotnet test` — must pass at 428/428 (no new tests yet). If any test breaks, STOP — likely an existing fixture asserts decreasing values; investigate.

After commit 2: `dotnet test --filter "FullyQualifiedName~NeedsAccumulation"` — 3 new tests green. Full suite at 431/431.

After commit 3: ROADMAP renders cleanly; resolved entry moved correctly; new Phase 5 entries appended.

If during execution an architectural fork is encountered not foreseen here — STOP, ask, document choice.

**Hypothesis-falsification clause:**

Housekeeping commit, not an M-cycle phase. Datapoint sequence (10 consecutive zeros post-M4) unaffected. No `MOD_OS_ARCHITECTURE` ratification candidate is plausible — this is gameplay-systems work below the spec layer.

Plausible non-spec frictions worth flagging:

(a) **Hidden test asserting decreasing values**: pre-flight grep should catch this; if found, update inline within commit 1 with rationale in commit body.

(b) **`NeedsSystem` constants inadvertently scaled assuming decay-toward-0**: empirically, the constants `0.002`, `0.0015`, `0.001`, `0.0005` were calibrated for "deficit accumulation pace" semantically — same pace either direction. Should not be an issue.

(c) **`MoodSystem.IsAtRisk` definition** (`Mood <= MoodBreakThreshold + 0.15f`) — works correctly with falling mood in either direction; no change needed.

(d) **Save-game format**: there is no active save system (verified earlier). Not an issue.

## Report-back format

- 3 commit SHAs (full hex).
- Final `dotnet test` count (428 + 3 = 431 expected, or actual).
- 3 new tests by name (`Hunger_GrowsOverTime_WhenNoRecoveryExists`, `AllFourNeeds_GrowOverTime_WhenNoRecoveryExists`, `Hunger_ClampsAt1_WhenAlreadyAtCeiling`) — all green.
- Existing `NeedsJobIntegrationTests.Starving_pawn_receives_Eat_job_after_NeedsCritical_fires_on_the_Pawns_bus` — confirmed still green (no fixture change needed).
- M7.x + housekeeping regression — all suites green.
- Working tree state: clean.
- M-phase boundary: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
- Solution file: `dotnet sln list` count unchanged.
- ROADMAP backlog: confirm "NeedsSystem decay direction" moved to Resolved, two new Phase 5 entries added.
- Manual F5 verification: deferred to user with predicted-observation checklist.
- Any test that had to be updated due to assuming decreasing values — list with rationale.
- Any unexpected findings.
