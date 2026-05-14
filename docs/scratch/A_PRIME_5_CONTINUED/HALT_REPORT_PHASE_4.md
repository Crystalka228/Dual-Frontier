# A'.5 K8.3+K8.4 combined milestone — Phase 4 halt report

**Status**: HALT 2026-05-14 — Phase 4 per-system migration paused before Commit 9 (ConsumeSystem) per METHODOLOGY §3 «stop, escalate, lock» + combined brief §8.3 AS-C1. Phases 0-3 closed cleanly (8 commits, HEAD `b903b91`, 671 tests passing). No source commits beyond Phase 3.

**Authored**: 2026-05-14 by Claude Opus 4.7 (1M context) Cloud Code session, auto mode, continuation of the same session that landed Commits 1-8.

**Halt authority**: combined brief §8.3 AS-C1 (Architectural Surprise — escalation), specifically the example «K-L7 SpanLease/WriteBatch contract incompatible with a specific system's read-modify-write pattern», generalized in this report to «K-L7 contract incompatible with the cross-system read-write pattern that the production scheduler depends on during the Phase 2-4 dual-write transition».

**Prior halts this continues**: `docs/scratch/A_PRIME_5/HALT_REPORT.md` (K8.3 v2.0, 2026-05-13, storage-location premise miss) → `docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT.md` (K8.3+K8.4 combined v1.0, 2026-05-14, API-surface premise miss, resolved by `tools/briefs/K8_34_COMBINED_BRIEF_REFRESH_PATCH.md`) → **this halt** (Phase 4 dual-write drift, surfaced during in-progress execution of Phase 4 Commit 9 preparation).

---

## §1 Halt rationale

The combined brief §3.3 «Per-system migration patterns» and §6.5 «Phase 4: per-system migration (Commits 9-20)» prescribe each of the 12 production systems to migrate one-by-one. Each migration switches that system's reads to `NativeWorld.AcquireSpan<T>` and its writes to `NativeWorld.BeginBatch<T>` (via `using` scope, auto-flush on dispose). Brief §3.3 generic template:

```csharp
public sealed class NeedsSystem : SystemBase
{
    public override void Update(float delta)
    {
        using var lease = NativeWorld.AcquireSpan<NeedsComponent>();
        using var batch = NativeWorld.BeginBatch<NeedsComponent>();

        for (int i = 0; i < lease.Count; i++)
        {
            EntityId entity = new EntityId(lease.Indices[i], 0);
            ref readonly NeedsComponent needs = ref lease.Span[i];
            NeedsComponent updated = new NeedsComponent { /* decay */ };
            batch.Update(entity, updated);
        }
    }
}
```

The combined brief §6.3 Commit 3 «Critical invariant» requires every commit between Phase 2 Commit 3 and Phase 5 Commit 21 to «build cleanly AND pass all tests». The brief argues «dual-write is not a temporary stub — it is legitimate intermediate state because both writes serve real consumers (managed `World` serves unmigrated systems; `NativeWorld` serves migrated systems)».

**Phase 0 deep-reads during Phase 4 preparation surface a contradiction**: the dual-write at write-time only happens at FACTORY time (bootstrap). Once a system migrates to `BeginBatch<T>` writes, its writes go to `NativeWorld` only — managed `World` is NOT updated by that system thereafter. Conversely, unmigrated systems write only to managed `World` (legacy `SystemBase.SetComponent` path). The two stores immediately drift on every simulation tick.

### §1.1 — Empirical evidence of drift

Concrete failure case for the prescribed Phase 4 Commit 9 (ConsumeSystem migration), traced through:

1. **Bootstrap (Commits 1-8 state — current HEAD)**: factories dual-write 50 pawns + 255 items to both `world` and `nativeWorld`. Indices align (lockstep `CreateEntity`); component values match.

2. **Commit 9 (ConsumeSystem migrates)**:
   - ConsumeSystem reads `NeedsComponent` of each pawn via `NativeWorld.AcquireSpan<NeedsComponent>` (or `NativeWorld.GetComponent<NeedsComponent>(pawn)` per-entity).
   - ConsumeSystem writes `ConsumableComponent.Charges--` via `NativeWorld.BeginBatch<ConsumableComponent>()`. (Decrement on consume — preserved from current implementation.)
   - Other systems (JobSystem, NeedsSystem, MovementSystem, PawnStateReporter) STILL write to managed `World` via the legacy `SystemBase.SetComponent` path.

3. **Tick 1**: NeedsSystem (unmigrated) decays `NeedsComponent.Satiety` from 0.9 → 0.899 on managed `world` only. `nativeWorld.NeedsComponent.Satiety` stays at 0.9 (the bootstrap value).

4. **Tick 100**: NeedsSystem has decayed 100 ticks of Satiety on managed `world`. Pawn's `world.NeedsComponent.Satiety` ≈ 0.8 (still above CriticalThreshold = 0.2). `nativeWorld.NeedsComponent.Satiety` ≈ 0.9 (bootstrap state, never updated).

5. **Tick 800**: NeedsSystem has decayed Satiety to ~0.0 on managed `world`. `nativeWorld.NeedsComponent.Satiety` still 0.9.

6. **ConsumeSystem at tick 800**:
   - Reads `NativeWorld.GetComponent<NeedsComponent>(pawn)` → returns 0.9
   - Branch `if (needs.Satiety <= CriticalThreshold)` → 0.9 > 0.2 → false
   - ConsumeSystem decides «no pawn needs to consume»
   - **But on managed `world`, pawns are starving (Satiety ≈ 0.0)**

7. **Other unmigrated systems** (JobSystem deciding to assign Eat job based on managed-world NeedsComponent values) DO see starvation. They assign Eat jobs via managed-world writes. But ConsumeSystem (migrated, native-world reads) sees zero pawns with Eat job because `nativeWorld.JobComponent` was last written at bootstrap (no jobs).

8. **Test failure mode**: integration tests that tick the simulation for many ticks (e.g. `CreateLoop_With...` ticking 1000+ frames) would observe pawn behavior regression — pawns starve to death because the migrated ConsumeSystem and unmigrated NeedsSystem/JobSystem disagree on world state.

### §1.2 — Why the brief's «critical invariant» can't hold under the prescribed pattern

Brief §6.3 Commit 3 says «every commit between Phase 2 commit 3 and Phase 5 commit 21 must build cleanly AND pass all tests». The drift described above means:
- Commit 9 (ConsumeSystem only): tests may pass if they don't tick the simulation enough to trigger starvation; they fail under sustained simulation.
- Commits 10-19 (incremental migrations): the drift accumulates — each commit adds a system that reads from native (stale) and writes to native (invisible to managed). By Commit 19, half the simulation is on native and half on managed; they have completely different state.
- Commit 20 (last system migrated): both worlds may finally converge if every system reads + writes consistently, but the intermediate Commits 9-19 will fail tests that exercise the simulation.

The brief's transition strategy assumes either:
- (a) Tests don't exercise the simulation deeply enough to surface drift (Commit-9-passes-because-100-ticks-isn't-enough-to-starve), OR
- (b) Every migrated system dual-writes to both worlds (not what the prescribed pattern in §3.3 shows), OR
- (c) The scheduler synchronizes state between worlds at phase boundaries (no such infrastructure exists).

None of (a)/(b)/(c) is surfaced by the brief or refresh patch.

### §1.3 — Brief refresh patch §4.4 acknowledgement (incomplete)

Refresh patch §4.4 says «Phase 5 commit 21 — dual-write removal (combined brief §2.3 + §6.6 — clarification): the **two-phase structure stays** at commit 21 — only the «Dual-write to managed World» loop is deleted.»

This addresses the FACTORY-side dual-write removal at Phase 5. It does NOT address the SYSTEM-side dual-write during Phase 4. The patch implicitly assumes systems behave like factories during Phase 4 transition (read managed, write native) — but that's not what the brief's §3.3 system template prescribes (read native, write native).

The Phase 4 migration template explicitly switches reads to nativeWorld via `AcquireSpan<T>`. Once any system migrates, that system reads stale native data unless every other system that produces data of those types has also migrated. The brief's incremental Tier 1 → Tier 5 ordering doesn't avoid drift; it accumulates it.

---

## §2 Halt scope + state

**Working tree at halt**:
- `M .claude/settings.local.json` (pre-existing, benign)
- `?? docs/scratch/A_PRIME_5_CONTINUED/HALT_REPORT_PHASE_4.md` (this file)
- No source changes pending.

**Commits 1-8 status**: clean, tests passing, build green. No revert needed.

**Phase 4 status**: 0 of 12 system migrations attempted. HALT before Commit 9.

**Phases 5-6 status**: 0 of 4 closure commits attempted. Cannot proceed without Phase 4.

**Architectural state preserved**: post-Commits 1-8, the kernel + Mod API + manifest infrastructure are fully in place. NativeWorld is active with deterministic ids; factories dual-write at bootstrap; Mod API v3 surface + Path β bridge are functional end-to-end. Only the SYSTEM-side migration remains.

---

## §3 The architectural diagnosis

### §3.1 — Why the brief author missed this

The brief's §3.3 prescribed migration pattern is correct in isolation (a single system, reads + writes a narrow component set on `NativeWorld`). The pattern is also correct at end-state (Phase 5 commit 21+, when all systems are migrated and managed `World` is retired).

The miss is at INTERMEDIATE state, during Phases 2-4 transition. The brief explicitly calls dual-write at factory time «legitimate intermediate state» (§6.3 Commit 3), but does not extend that reasoning to per-system writes during Phase 4. The implicit assumption is that systems can flip from managed-read/managed-write to native-read/native-write atomically per commit, and the cross-system data flow continues to work — but that requires the data sources (other systems) to also have flipped.

The brief's Tier 1 → Tier 5 ordering rationale (§3.4) does not address cross-tier data dependencies. Tier 1 systems (ConsumeSystem, SleepSystem, ComfortAuraSystem) READ NeedsComponent which is written by Tier 3 NeedsSystem (commit 16). Tier 1 migrates first; Tier 3 migrates 7 commits later. During those 7 commits, the Tier 1 systems read stale native NeedsComponent data because Tier 3's NeedsSystem still writes to managed `World`.

### §3.2 — Why each of the prior 8 commits was unaffected by this miss

The drift only manifests when:
1. A system migrates to read from nativeWorld (Commits 9-20), AND
2. The simulation is ticked enough for the writes (still on managed world from unmigrated systems) to diverge from nativeWorld bootstrap values.

Commits 1-8 don't trigger condition (1) — they refactor infrastructure (Bootstrap.Run, RegisterAll, factories, IModApi, Path β bridge, manifest parser) but don't migrate any system. Tests at Commits 1-8 still pass because every system continues to read+write managed world; nativeWorld is populated at bootstrap but not consulted by any system thereafter.

The trigger for the drift is Commit 9. Phase 0 hindsight: the drift could have been surfaced earlier by simulating Commit 9 mentally during brief authoring. The combined brief's claim of «structurally prevents premise miss class» (§1) doesn't extend to per-system transition drift.

### §3.3 — Connection to prior halts

| Halt | Date | Class | Resolution |
|---|---|---|---|
| K8.3 v2.0 | 2026-05-13 | Storage-location premise miss | Combined brief absorbed K8.3+K8.4 |
| Combined v1.0 | 2026-05-14 | API-surface premise miss | Refresh patch resolved 5 findings |
| **Phase 4 dual-write drift** | **2026-05-14** | **Transition-state semantic miss** | **Pending: requires brief amendment for per-system dual-write or scheduler sync** |

All three halts share a root: brief author worked from architecture documents (target state) without simulating intermediate state under realistic transition. K8.3 v2.0 missed bootstrap comment; combined v1.0 missed API surface visibility; Phase 4 missed cross-system dependencies during transition.

---

## §4 Candidate resolutions (deliberation surface)

**Disposition A — Per-system dual-write**:
- Modify the brief's §3.3 generic migration template to require dual-write per migrated system: `BeginBatch<T>` on nativeWorld AND `world.SetComponent<T>` (or equivalent) on managed world during the Phase 4 transition.
- Reads still come from nativeWorld for the migrated system (single read source per system).
- Other systems (unmigrated) continue reading from managed world; their reads get fresh data from the dual-write.
- Phase 5 commit 21 removes the managed-side dual-write, same as the factory pattern.
- Effort per system: ~10 minutes additional code per migration to add the dual-write loop.
- Pro: minimal deviation from brief structure; per-system tests continue to pass.
- Con: brief deviation; requires deliberation lock; adds Phase 4 LOC.

**Disposition B — Scheduler-level world sync**:
- At end of each scheduler phase (or end of each tick), synchronize managed ↔ native worlds bidirectionally.
- A new helper `WorldSync.SyncManagedToNative(world, nativeWorld)` (and reverse) called by the scheduler.
- Eliminates per-system concerns; the worlds always have current data on both sides during transition.
- Effort: ~1-2 hours infrastructure work; once.
- Pro: minimal per-commit risk; brief migration template unchanged at system level.
- Con: scheduler-level change is more invasive; performance cost per tick (full snapshot copy); raises new questions about which world is authoritative for conflicting writes.

**Disposition C — Big-bang Phase 4**:
- Migrate all 12 systems in a SINGLE commit. The drift never has a chance to manifest because no intermediate state exists — either all systems on managed (pre-Commit 9) or all on native (post-Commit 9).
- Effort: ~6-10 hours for one massive commit covering 12 system rewrites.
- Pro: drift class doesn't surface (no intermediate state).
- Con: violates K8.1 «atomic compilable unit» (each migration is normally a separate commit); large blast radius; harder to debug if any system migration introduces a regression.

**Disposition D — Reorder per-tier dependency graph**:
- Migrate systems in REVERSE dependency order: producers (NeedsSystem, JobSystem, MovementSystem) FIRST, consumers (ConsumeSystem, etc.) LAST.
- Brief's Tier 1 → Tier 5 ordering is roughly «narrow-component first»; need to invert to «produces-data-for-others first».
- Effort: brief amendment (renumbered Tier 1-5 ordering); per-system code unchanged.
- Pro: drift class minimized — by the time a consumer migrates, its producers already are on native.
- Con: doesn't fully eliminate drift (some systems both read and write shared components); still needs partial dual-write or sync; brief ordering is established and changing it is a significant amendment.

**Disposition E — Halt + ship Phase 3 closure**:
- Accept Phases 0-3 as the deliverable for this milestone.
- Document Phase 4 as «deferred» to a future milestone.
- This violates the brief's combined-milestone atomicity but reflects pragmatic risk: Phase 4 has unsurfaced complexity; better to ship clean Phase 3 than risk regressions in Phase 4.
- Effort: minimal — just close out the 8 commits with a partial-milestone closure entry.
- Pro: safest; no further regressions risked.
- Con: combined milestone goal not achieved; K-L7 «SpanLease/WriteBatch in production» not realized.

---

## §5 Recommended deliberation questions

- **Q-K8.34-P4-1**: Which Disposition (A/B/C/D/E) best preserves the combined-milestone atomicity property AND the K8.1 «atomic compilable unit» property?
- **Q-K8.34-P4-2**: For Disposition A, what's the exact dual-write pattern? Does the migrated system call `SystemBase.SetComponent<T>` (legacy path) AFTER `BeginBatch<T>.Update`? Or does it bypass the isolation guard and write directly via a new `Context.LegacySetComponent<T>` method?
- **Q-K8.34-P4-3**: For Disposition B, is the per-tick sync acceptable performance-wise? Native World has 50 pawns × 7 components ≈ 350 round-trips per sync per direction. At 60 ticks/sec, that's 21,000 round-trips/sec. Probably tolerable, but unmeasured.
- **Q-K8.34-P4-4**: Methodology — should brief authoring discipline (CAPA-2026-05-14-K8.34-API-SURFACE-MISS) extend to «simulating Phase 4 mid-state mentally during brief authoring»? Lesson #8 candidate: «briefs that prescribe per-system migration must mentally simulate the cross-system read-write transition during authoring to surface drift».
- **Q-K8.34-P4-5**: Pragmatic — given that Phases 0-3 already deliver substantial architectural value (NativeWorld active, factories dual-write, Mod API v3 + Path β shipped), is Disposition E (close at Phase 3) acceptable as a milestone outcome with Phase 4 deferred?

---

## §6 CAPA candidate

**CAPA-2026-05-14-K8.34-PHASE-4-TRANSITION-DRIFT** (new):
- **Trigger**: Combined K8.3+K8.4 brief v1.0 + refresh patch §3.3 prescribe per-system migration to `NativeWorld.AcquireSpan/BeginBatch` without addressing the cross-system data flow during the Phase 4 incremental transition. First-system migration (ConsumeSystem at Commit 9) would create stale-read divergence between migrated and unmigrated systems for sustained simulation.
- **Affected documents**: combined brief, refresh patch.
- **Root cause**: same class as CAPA-2026-05-13/2026-05-14 (brief from architecture docs without simulating realistic execution paths). Specifically: per-system transition mid-state not simulated.
- **Corrective action**: TBD per deliberation — one of Disposition A-E above.
- **Effectiveness verification**: TBD — depends on chosen disposition. For A: per-system tests + full integration tick test passes at each Phase 4 commit. For B: world sync infrastructure tests + integration test. For C: single-commit integration test. For D: per-system tests + integration test passes at each renumbered tier. For E: partial-milestone closure entry recorded.

---

## §7 Methodology lesson candidate (for METHODOLOGY v1.9 if Crystalka chooses to add)

Extends the K8.3 v2.0 + combined v1.0 lessons (production runtime ground-truth + API surface verification) with a third dimension:

**Title**: Mid-transition state simulation as brief authoring discipline

**Body draft** (subject to Crystalka edit + deliberation):

> Briefs that prescribe incremental migration (per-system, per-component, per-feature) must include a mid-transition simulation step at authoring time. The simulation walks through a representative subset of the prescribed commits to verify the cross-cutting state (data flow, invariants, test pass rate) holds at each intermediate state, not just at start and end.
>
> K8.3 v2.0 lesson #1 added Phase 0 *runtime-state* verification (read entry-point files in full at authoring).
> Combined v1.0 lesson #2 added *API surface* verification at authoring (transcribe constructor signatures + helper existence into the brief).
> **Phase 4 lesson #3 adds *mid-transition state* simulation**: for any brief prescribing N>1 incremental commits where each commit changes a load-bearing invariant, mentally walk through commits 1, N/2, N to verify state remains consistent.
>
> **Failure mode (Phase 4 drift, 2026-05-14)**: combined brief §3.3 + refresh patch prescribed 12 per-system migrations switching reads + writes from managed world to nativeWorld atomically per system. Brief stipulated «every commit must pass tests» but did not simulate the mid-state where 1, 5, or 11 systems are migrated. In that mid-state, migrated systems read bootstrap-time nativeWorld values while unmigrated systems write to managed world only — the two stores diverge per tick, and the simulation behavior is incoherent until either all systems migrate (Disposition C) or both worlds stay in sync (Disposition A or B).
>
> **Principle**: a brief is a specification for a sequence of changes; the sequence's mid-states must be verified, not just the endpoints. Each intermediate state should preserve invariants that tests assert.
>
> **Operational consequence**: brief authoring includes a «mid-transition simulation» entry in provenance — author walks through commits, traces data flow, confirms tests should pass at each intermediate.

---

## §8 Provenance

- **Authored**: 2026-05-14 (same day as combined v1.0 halt + resolution + 8 commits)
- **Triggering event**: pre-Commit-9 (ConsumeSystem migration) analysis during Phase 4 preparation; user direction «Continue Phase 4 fully» followed by analytical surface of cross-system dependency drift
- **Halt direction**: user choice 2026-05-14 «Halt + author HALT_REPORT» from 4-option AskUserQuestion offering Halt / Empirical / Per-system dual-write / Stop-at-Phase-3
- **HEAD at halt**: `b903b91` (Phase 3 closure)
- **Tests at halt**: 671 passing, build green
- **No source changes pending** (this halt artifact is the only working-tree mutation)

### §8.1 Continuation context

Session has been long (8 commits across Phases 0-3). User explicitly chose Continue-auto-execute when the second halt resolved into the refresh patch. Continuing into Phase 4 surfaced the drift concern within the first system's analysis. Per protocol established by prior halts, escalating rather than improvising.

### §8.2 Related governance artifacts (to author post-deliberation)

- `EVT-2026-05-14-K8.34-PHASE-4-HALT` (audit_trail entry)
- `CAPA-2026-05-14-K8.34-PHASE-4-TRANSITION-DRIFT` (capa_entries)
- REGISTER.yaml entry for this artifact: `DOC-E-A_PRIME_5_CONTINUED_PHASE_4_HALT_INVESTIGATION` (category E scratch, tier 3, lifecycle EXECUTED at this artifact's authoring)
- METHODOLOGY v1.9 lesson if Crystalka adopts §7
- Brief amendment per chosen disposition (Disposition A, B, C, or D triggers brief amendment; E triggers partial-milestone closure)

---

**Artifact end. Phase 4 deliberation surface ready. Combined brief + refresh patch lifecycle remains AUTHORED pending Crystalka resolution. 8 commits clean at HEAD `b903b91`.**
