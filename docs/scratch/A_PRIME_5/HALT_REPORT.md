---
register_id: DOC-E-A_PRIME_5_K8_3_HALT_INVESTIGATION
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-A_PRIME_5_K8_3_HALT_INVESTIGATION
---
# A'.5 K8.3 execution halt report

**Status**: HALT 2026-05-13 — K8.3 v2.0 brief execution halted at Phase 0.2 per METHODOLOGY §3 «stop, escalate, lock». No code commits. Working tree clean. Investigation captured here. **Resolution**: Crystalka ratified Option 2 (swap K8.3/K8.4 order) via patch brief at `tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md` 2026-05-13; this halt report sits alongside the patch as historical artifact of the «stop, escalate, lock» invocation.

**Authored**: 2026-05-13 by Cloud Code (Claude Opus 4.7, auto mode) during attempted K8.3 v2.0 brief execution session.

**Halt authority**: Crystalka direction 2026-05-13 — «Option 1 — Halt fully, re-deliberate. Brief v2.0 missed GameBootstrap.cs comment explicitly stating 'component storage stays on the managed World through K8.3; the registry-based path lights up at K8.4 when component storage moves.' This is brief authoring premise miss, not execution issue.»

**Brief under execution**: `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` v2.0 AUTHORED 2026-05-13. Remains AUTHORED — not transitioned to EXECUTED, not transitioned to DEPRECATED. Lifecycle stays AUTHORED pending re-deliberation outcome.

**Companion artifacts** (none yet authored; expected post-deliberation):
- K8.3 v2.1 amendment brief (if Option A — brief authoring repair), OR
- K8.3 v2.0 DEPRECATED + K8.3 v3 brief (if Option B — full re-author), OR
- K8.4 v1 brief authored next + K8.3 v2.0 re-executed post-K8.4 (if Option C — sequence swap), OR
- Other resolution from Crystalka + Opus deliberation

---

## §1 Halt rationale

K8.3 v2.0 brief §3.1 (Read pattern, K-L7 + K5 protocol) prescribes the per-system migration target as:

```csharp
public sealed class NeedsSystem : SystemBase
{
    public override void Update(SystemExecutionContext context)
    {
        using var lease = context.NativeWorld.AcquireSpan<NeedsComponent>();
        var writeBatch = new WriteBatch<NeedsComponent>();
        // … iterate lease.Span, write to batch …
        context.NativeWorld.FlushWrites(writeBatch);
    }
}
```

This pattern presupposes that **NativeWorld holds entity-component storage for the 12 production systems' read/write types** at K8.3 execution time. Phase 0.2 empirical verification falsifies this premise.

**Conflation surface**: K8.2 v2 closure achieved «K-L3 selective per-component application» — 6 class-shape components converted to `unmanaged struct` shape, 6 stub deletions, 12 verify-only annotations. «K-L3 «без exception»» framing was reframed per K-L3.1 amendment as «selective per-component». The brief author conflated:

- **K-L3 invariant (struct shape)** — components are `unmanaged struct` types in C# (achieved K8.2 v2, 2026-05-09)

with:

- **K-L11 implication / K-L8 implication (storage location)** — production component storage lives in `NativeWorld` rather than `World` (scheduled for K8.4 per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §1.3 + `KERNEL_ARCHITECTURE.md` Part 0 K-L8 implication)

These are **orthogonal concerns**. The brief's §1 «Why now» framing — «post-K8.2 v2 closure, all production components are unmanaged structs (Path α). Production systems still use legacy managed `World` access pattern. K8.3 closes the gap» — treats struct-shape closure as if it implied native-storage closure. It does not.

---

## §2 Phase 0 partial state

### §2.1 Phase 0.1 — mandatory authoritative reads (DONE)

All 7 mandatory pre-flight documents read end-to-end (large documents read in targeted section ranges):

1. ✅ `docs/governance/FRAMEWORK.md` v1.0 LOCKED — full document (724 lines)
2. ✅ `docs/methodology/METHODOLOGY.md` v1.7 — §12 Register integration (lines 444-547) + Pipeline closure lessons (lines 713-822); §12.7 canonical closure protocol confirmed (8-step shape per A'.4.5)
3. ✅ `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.1 LOCKED — §0.3 LOCKED architectural decisions (lines 94-130) including decision #9 K-L3.1 bridge formalization; §1.2 K8.3 reformulated scope (lines 176-208); §1.3 K8.4 (lines 210-228) confirming managed World retirement scheduled at K8.4
4. ✅ `docs/architecture/KERNEL_ARCHITECTURE.md` v1.5 LOCKED — Part 0 K-L1..K-L11 + K-L3 implication post-K-L3.1 (lines 44-79); §1.7 Span<T> protocol (lines 358-417); §1.8 Write command batching (lines 418-453); Part 2 Master plan K8.3 row (line 603)
5. ✅ `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` Live — full document (336 lines); confirmed Phase A'.5 = K8.3 skeleton execution, executor Cloud Code
6. ✅ `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md` v2.0 EXECUTED — full document (1013 lines); K8.1 wrapper refactor + 6 conversions + 6 deletions delivered 2026-05-09
7. ✅ `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` v1.0 EXECUTED — full document (546 lines); Q1-Q6 locks recorded; Q3.i dual API; Q4.b managed-path runtime-only; Q5.a passive metrics

### §2.2 Phase 0.2 — production state verification (DONE — surfaced halt condition)

**§2.2.1 — coreSystems inventory (PASS)**:

`src/DualFrontier.Application/Loop/GameBootstrap.cs:157-171` array `coreSystems` matches brief §0.1 inventory exactly:

| # | System | Slice | Match |
|---|---|---|---|
| 1 | NeedsSystem | Pawn | ✅ |
| 2 | MoodSystem | Pawn | ✅ |
| 3 | JobSystem | Pawn | ✅ |
| 4 | ConsumeSystem | Pawn | ✅ |
| 5 | SleepSystem | Pawn | ✅ |
| 6 | ComfortAuraSystem | Pawn | ✅ |
| 7 | MovementSystem | Pawn | ✅ |
| 8 | PawnStateReporterSystem | Pawn | ✅ |
| 9 | InventorySystem | Inventory | ✅ |
| 10 | HaulSystem | Inventory | ✅ |
| 11 | ElectricGridSystem | Power | ✅ |
| 12 | ConverterSystem | Power | ✅ |

Q-K8.3-1 scope LOCK confirmed empirically.

**§2.2.2 — `[ManagedStorage]` attribute grep (PASS)**:

```powershell
Get-ChildItem -Path src/DualFrontier.Components -Recurse -Filter *.cs |
  Select-String -Pattern '\[ManagedStorage\]' -SimpleMatch
```

Zero matches. Q-K8.3-2 LOCK precondition confirmed — no production Path β components at K8.3 brief authoring time. K8.4 ships Path β plumbing as separate milestone scope.

**§2.2.3 — orphan .uid files (DEVIATION +1)**:

Brief §0.1 + §2.3 expected 3 orphan `.uid` files. Phase 0.2 grep against actual `src/DualFrontier.Systems/**/*.uid` vs `**/*.cs` found **4 orphans**:

1. `src/DualFrontier.Systems/Combat/ShieldSystem.cs.uid` ✅ (expected)
2. `src/DualFrontier.Systems/Pawn/SocialSystem.cs.uid` ✅ (expected)
3. `src/DualFrontier.Systems/World/BiomeSystem.cs.uid` ✅ (expected)
4. `src/DualFrontier.Systems/Pawn/SkillSystem.cs.uid` ⚠️ (NEW — not in brief)

Per K-Lessons «Phase 0.4 inventory as hypothesis, not authority»: a +1 orphan vs brief expectation is a recordable deviation, not a halt-eligible structural surprise. Would have been included in Phase 1 cleanup if execution proceeded. METHODOLOGY §7.1 11-th invocation applies — `SkillSystem.cs` does not exist; the `.uid` is meta-level placeholder lie surviving prior system deletion.

**§2.2.4 — premise verification (HALT — STRUCTURAL SURPRISE)**:

This sub-check was not in the brief's Phase 0 enumeration. It was triggered by the bootstrap-file comment surfacing during Phase 0.2 inspection. Empirical reads:

**Evidence A — bootstrap comment, [src/DualFrontier.Application/Loop/GameBootstrap.cs:85-90](src/DualFrontier.Application/Loop/GameBootstrap.cs:85)**:

```csharp
var world        = new World();
// K8.2 v2 — NativeWorld owned alongside the managed World until K8.4
// retires the latter. Production uses NativeWorld only for K8.1
// primitives (string interning, native maps/sets/composites bound to
// struct components); component storage stays on the managed World
// through K8.3. Bootstrap path uses FNV-1a fallback type ids; the
// registry-based path lights up at K8.4 when component storage moves.
var nativeWorld  = new NativeWorld();
```

This comment is **explicit** about K8.3 scope: NativeWorld holds K8.1 primitives only; entity-component storage stays on managed World through K8.3.

**Evidence B — production component allocation, [src/DualFrontier.Application/Scenario/RandomPawnFactory.cs:117-164](src/DualFrontier.Application/Scenario/RandomPawnFactory.cs:117)**:

```csharp
private EntityId SpawnOne(World world, GameServices services, GridVector pos)
{
    EntityId id = world.CreateEntity();                                                       // managed World creates entity

    world.AddComponent(id, new PositionComponent { Position = pos });                         // managed AddComponent
    world.AddComponent(id, new IdentityComponent { Name = _nativeWorld.InternString(...) });  // IdentityComponent → managed; field VALUE (InternedString) lives in native
    NativeMap<SkillKind, int> levels = _nativeWorld.CreateMap<SkillKind, int>();              // K8.1 primitive → native
    // … populate levels …
    world.AddComponent(id, new SkillsComponent { Levels = levels, ... });                     // SkillsComponent itself → managed; Levels FIELD references native NativeMap
    world.AddComponent(id, new NeedsComponent { ... });                                       // NeedsComponent → managed (no K8.1 primitive fields)
    world.AddComponent(id, new MindComponent());                                              // managed
    world.AddComponent(id, new JobComponent { Current = JobKind.Idle });                      // managed
    world.AddComponent(id, new MovementComponent { Path = _nativeWorld.CreateComposite<GridVector>() }); // MovementComponent → managed; Path FIELD references native NativeComposite

    services.Pawns.Publish(new PawnSpawnedEvent { … });
    return id;
}
```

**Diagnostic**: every entity-component pair lands in managed `World`. `_nativeWorld` is invoked only for K8.1 primitives (`InternString`, `CreateMap`, `CreateComposite`) — the *field values* inside components, not the component records themselves. NativeWorld at production runtime has zero entity-component records.

**Evidence C — existing system access pattern, [src/DualFrontier.Systems/Pawn/NeedsSystem.cs:82-98](src/DualFrontier.Systems/Pawn/NeedsSystem.cs:82)**:

```csharp
public override void Update(float delta)
{
    foreach (var entity in Query<NeedsComponent>())                                  // → ctx.Query<T>() → _world.GetEntitiesWith<T>() — MANAGED
    {
        var needs = GetComponent<NeedsComponent>(entity);                            // → ctx.GetComponent<T>() → _world.GetComponentUnsafe<T>() — MANAGED
        needs.Satiety   = Math.Clamp(needs.Satiety   - SatietyDepletionPerTick,   0f, 1f);
        // … other needs decay …
        SetComponent(entity, needs);                                                 // → ctx.SetComponent<T>() → _world.SetComponent<T>() — MANAGED
        // … critical-threshold edge detection …
    }
}
```

Current K7-era pattern. Components live in managed World; isolation guard (`SystemExecutionContext`) routes through `_world.GetComponentUnsafe<T>` / `_world.SetComponent<T>` ([SystemExecutionContext.cs:172,191](src/DualFrontier.Core/ECS/SystemExecutionContext.cs:172)).

**Evidence D — SystemExecutionContext native-world coupling, [src/DualFrontier.Core/ECS/SystemExecutionContext.cs:50,118](src/DualFrontier.Core/ECS/SystemExecutionContext.cs:50)**:

```csharp
private readonly World _world;
// …
private readonly NativeWorld? _nativeWorld;

internal NativeWorld? NativeWorld => _nativeWorld;
```

`_nativeWorld` is exposed via `Context.NativeWorld` for K8.1 primitive access (intern strings, resolve NativeMap fields). It is **not** plumbed for entity-component CRUD — `GetComponent`/`SetComponent`/`Query` go through `_world` (managed).

**Consequence at K8.3 execution time**:

Calling `context.NativeWorld.AcquireSpan<NeedsComponent>()` (per brief §3.1) would:
1. Resolve `NeedsComponent` typeId via FNV-1a (no registry binding at production NativeWorld construction per [GameBootstrap.cs:91](src/DualFrontier.Application/Loop/GameBootstrap.cs:91))
2. P/Invoke `df_world_acquire_span` for that typeId
3. Return a span with `Count = 0` because no `df_world_add_component<NeedsComponent>` was ever called for any entity

The migration target — read 50 pawns' NeedsComponent values via SpanLease, decay them, batch-write — would iterate zero entities. NeedsSystem would silently stop ticking. Same for the other 11 systems. F5 verification at Phase 4.1 would surface pawn behavior frozen (no need decay → no critical events → no job assignments → no movement → no consume/sleep) but `dotnet test` for individual systems (if authored as brief §0.3 specified) would not catch this if test fixtures use the same broken assumption.

---

## §3 The architectural diagnosis

### §3.1 Why brief authoring missed this

Possibilities for forensic record (deliberation session decides which apply):

1. **Closure-framing inheritance** — K8.2 v2 closure framed «K-L3 «без exception» state achieved» (later corrected by K-L3.1 to «selective per-component»). The «без exception» framing left rhetorical residue suggesting completeness. The brief author may have read «struct shape achieved» as «native storage achieved».

2. **Bootstrap comment not in mandatory reads** — Brief §5.1 lists 7 mandatory authoritative reads; `src/DualFrontier.Application/Loop/GameBootstrap.cs` is not among them. The comment at line 85-90 documents the K8.3-vs-K8.4 scope split clearly, but the brief authoring path didn't surface it. **Lesson candidate**: future brief authoring for milestones touching the system layer should mandate reading `GameBootstrap.cs` (and equivalent runtime-wiring files) as Phase 0 inputs — these contain operational ground truth that documentation may lag.

3. **«Path α default» ambiguity** — K-L3 implication post-K-L3.1 (`KERNEL_ARCHITECTURE.md` Part 0): «default storage path for components is `unmanaged struct`». «Storage path» reads as both «shape category» and «storage location». Brief author may have read the former; ground truth is the latter is still managed at K8.3.

4. **K-L3.1 amendment surface gap** — K-L3.1 amendment plan landed at A'.1.K (commit `0789bd4`); migration plan §1.2 K8.3 row was updated with «Dual access pattern (post-K-L3.1)» wording. The wording at MIGRATION_PLAN §1.2 line 204 mentions «SystemBase.NativeWorld.AcquireSpan<T>()» as the K8.3 target access — same wording as the brief. Plan and brief are mutually consistent but both diverge from the actual production state. This suggests the K-L3.1 amendment to MIGRATION_PLAN §1.2 also missed the K8.3-vs-K8.4 storage split — possibly the original conflation source.

### §3.2 Scope of the gap across LOCKED documents

| Document | Section | Wording status |
|---|---|---|
| K8.3 v2.0 brief | §3.1 read pattern + §3.2 write pattern + §3.5 dual-path API | Premise broken at brief authoring 2026-05-13 |
| MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.1 | §1.2 K8.3 scope (line 204) | Mentions «SystemBase.NativeWorld.AcquireSpan<T>()» as K8.3 target — same conflation |
| KERNEL_ARCHITECTURE.md v1.5 | Part 0 K-L3 implication | Wording «default storage path is unmanaged struct» — ambiguous, doesn't clarify storage location |
| KERNEL_ARCHITECTURE.md v1.5 | Part 0 K-L8 implication | «Post-K8.4 closure, NativeWorld is the only production storage path» — CORRECT (locates storage migration at K8.4) |
| KERNEL_ARCHITECTURE.md v1.5 | Part 2 K8.3 row (line 603) | «12 vanilla systems migrated to SpanLease/WriteBatch» — silent on storage location |
| GameBootstrap.cs:85-90 | inline comment | CORRECT operational ground truth |

K-L8 implication and the bootstrap comment are mutually consistent and correct. K-L3 implication (Part 0) + MIGRATION_PLAN §1.2 + K8.3 brief share the conflation. The K-L3.1 amendment plan likely propagated the wording inheritance from K-L3 implication into MIGRATION_PLAN §1.2 and into the brief without empirical verification against GameBootstrap.cs.

---

## §4 What was NOT done (clean halt)

Per Crystalka direction «Do not commit code changes. Halt cleanly.»:

- ❌ No Phase 1 (orphan .uid cleanup) execution
- ❌ No Phase 2 (NativeWorldTestFixture authoring) execution
- ❌ No Phase 3.x per-system migrations (zero of 12)
- ❌ No Phase 4 verification
- ❌ No Phase 5 closure documentation amendments
- ❌ No REGISTER.yaml mutations
- ❌ No audit_trail entry append
- ❌ No CAPA closure (CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT remains OPEN)
- ❌ No git commits
- ❌ No git stash; no branch creation
- ❌ No edits to any tracked file in `src/`, `tests/`, `mods/`, `native/`
- ❌ No edits to LOCKED documents (MIGRATION_PLAN, KERNEL_ARCHITECTURE, METHODOLOGY)
- ❌ No edits to K8.3 brief frontmatter (lifecycle stays AUTHORED)

**Working tree state at halt**: this artifact file (`docs/scratch/A_PRIME_5/HALT_REPORT.md`) plus the resolution patch (`tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md`) are the new untracked artifacts. No other diff vs branch HEAD `f7e6d52`. The accompanying `.patch` file (`K8_3_HALT_INVESTIGATION.patch`) is not authored — this Claude Code session's investigation produced zero `src/`/`tests/` working changes (clean reads only), so `git diff` would be empty. The investigation surfaced through reads + analysis is captured directly in this report.

---

## §5 Resolution path (deliberation session input)

Crystalka locked Option 1 (full halt, re-deliberate) per direction 2026-05-13. Subsequent deliberation session resolves K8.3/K8.4 ordering and brief disposition.

### §5.1 Candidate dispositions (deliberation surface)

The four resolution paths surfaced to Crystalka during the halt-or-proceed AskUserQuestion:

**Disposition A — Full halt, re-deliberate** (selected initially; superseded same-day by Disposition B via patch brief):
- K8.3 v2.0 brief lifecycle stays AUTHORED for now
- New deliberation session (Opus + Crystalka) authors corrective brief or amendment plan
- Possible outcomes: v2.1 amendment, full v3 re-author, K8.3+K8.4 merge, sequence swap

**Disposition B — Swap K8.3 and K8.4 order** (LOCKED via `tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md` 2026-05-13; supersedes Disposition A; halt session terminates after register update commits):
- K8.4 (managed World retirement + Mod API v3 + component storage migration) executes first
- K8.3 system migration runs against post-K8.4 state where SpanLease/WriteBatch on NativeWorld is the only path
- K8.3 v2.0 brief becomes correct post-K8.4 — possibly re-executable as-is with minor amendment
- Sequence: A'.5 = K8.4, A'.6 = K8.3; reverse current PHASE_A_PRIME_SEQUENCING

**Disposition C — Re-scope K8.3 in-flight** (one outcome of deliberation):
- K8.3 absorbs storage migration: per-system commit moves components into NativeWorld AND migrates access pattern
- Roughly doubles scope; tier ordering becomes harder; failure surface increases per atomic commit
- K8.3 v2.0 brief requires substantial v3 rewrite

**Disposition D — K8.3 managed-only protocol** (one outcome of deliberation):
- K8.3 migrates systems to bulk-managed-access (e.g., `World.GetComponents<T>()` if exists, or new managed batch protocol) without involving NativeWorld
- K8.4 separately moves to native
- K8.3 v2.0 brief requires substantial v3 rewrite (K-L7 reformulation for managed-bulk path)

### §5.2 Recommended deliberation questions

Surfaced as candidates for the resolution session (not pre-locked here):

- **Q-K8.3-Premise-1**: which disposition (A/B/C/D) best preserves K-L11 «без компромиссов» commitment + decade-horizon discipline?
- **Q-K8.3-Premise-2**: does K8.4 brief authoring (if Disposition B chosen) inherit the K-L3.1 dual-path Q3.i lock unchanged, or does the path β plumbing need re-deliberation?
- **Q-K8.3-Premise-3**: should K-L3 implication wording in `KERNEL_ARCHITECTURE.md` Part 0 be amended to explicitly distinguish «struct shape» from «storage location» to prevent recurrence?
- **Q-K8.3-Premise-4**: does MIGRATION_PLAN §1.2 K8.3 row wording «Dual access pattern (post-K-L3.1)» require amendment to surface the K8.4 storage prerequisite explicitly?
- **Q-K8.3-Premise-5**: brief authoring methodology amendment — should future briefs include a «§5.X production runtime ground-truth verification» Phase 0 sub-step (read `GameBootstrap.cs` + equivalent wiring files) as mandatory, distinct from architecture-document reads?

### §5.3 RISK-008 reproduction

This halt reproduces a brief-authoring premise miss pattern. Per A'.4.5 register `risks` collection, RISK-008 covers brief-authoring failure modes (specific entry text TBD against actual REGISTER.yaml content). The «stop, escalate, lock» protocol caught the miss structurally before harmful commits — register protocol operating as designed per METHODOLOGY §3.

Future entry to `audit_trail` for this halt: EVT-2026-05-13-K8.3-EXECUTION-HALT, governance_impact = «brief authoring premise miss surfaced during Phase 0.2 pre-flight; stop-escalate-lock pattern caught structurally; deliberation session pending».

CAPA entry candidate post-deliberation: CAPA-2026-05-13-K8.3-PREMISE-MISS, trigger = «K8.3 v2.0 brief authored against incorrect storage-location premise», corrective_action = TBD per deliberation outcome, effectiveness_verification = «K8.3 v2.1/v3 brief executes cleanly through Phase 0.2 premise-check sub-step».

---

## §6 Phase 0 read inventory (for deliberation session pre-flight)

For the deliberation session, the seven mandatory K8.3 reads remain valid. Additionally recommended:

1. This artifact (`docs/scratch/A_PRIME_5/HALT_REPORT.md`)
2. `src/DualFrontier.Application/Loop/GameBootstrap.cs` (full file; particularly lines 85-90, 91, 119, 157-171)
3. `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs` (full file; particularly lines 117-164 — component allocation)
4. `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` (full file; particularly lines 50, 118, 172, 191)
5. `src/DualFrontier.Core/ECS/SystemBase.cs` (full file; particularly NativeWorld accessor lines 163-188)
6. `src/DualFrontier.Core.Interop/NativeWorld.cs` lines 27-345 (constructors, AcquireSpan, BeginBatch — verify what AcquireSpan would actually return when no components registered)
7. `src/DualFrontier.Systems/Pawn/NeedsSystem.cs` (representative current-pattern system, full file)
8. `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` lines 1-100 (scheduler holds both `_world` and `_nativeWorld`)

These eight files are the operational ground truth. Reading them before deliberation prevents recurrence of the present premise miss.

---

## §7 Methodology lesson candidate (for METHODOLOGY v1.8 if Crystalka chooses to add)

**Title**: Production runtime ground-truth verification as Phase 0 mandatory step

**Body draft** (subject to Crystalka edit + deliberation):

> Briefs that prescribe a per-system migration pattern (e.g., K8.3-style read/write API changes) must include an empirical verification of the actual production wiring before Phase 1 begins. Architecture documents describe target state; bootstrap files (`GameBootstrap.cs`, factory classes, scheduler construction) describe current state. The two can diverge silently when amendment plans propagate target-state wording without retroactive verification.
>
> **Brief authoring requirement**: any brief touching the system layer or component access path must list the relevant production wiring files in Phase 0.1 (or equivalent), reading them as authoritative ground truth alongside architecture-document target state. Discrepancy = halt-eligible.
>
> **Failure mode (observed at K8.3 v2.0 execution halt, 2026-05-13)**: K8.3 v2.0 brief prescribed migration to `NativeWorld.AcquireSpan<TComponent>()` based on `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §1.2 + `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication wording. Both architecture documents and the brief shared a conflation of «struct shape» (K-L3) with «native storage» (K-L8 / K-L11 implication, scheduled K8.4). `GameBootstrap.cs:85-90` inline comment correctly recorded the K8.3-vs-K8.4 storage split but was not in the brief's Phase 0 reads. Halt-escalate-lock caught the conflation structurally before 12-20 hours of broken-migration execution.
>
> **Principle**: architecture docs say what should be true; production wiring says what is true. Phase 0 reads both. Discrepancy is data for halt-escalate-lock.
>
> **Falsifiable claim**: future briefs that include production-wiring reads in Phase 0 will encounter fewer premise-miss halts at execution time. Counter-example would force re-examination of read scope.

---

## §8 Provenance

- **Authored**: 2026-05-13, Claude Opus 4.7 (1M context) Cloud Code session, auto mode
- **Triggering event**: K8.3 v2.0 brief execution session, Phase 0.2 sub-check §2.2.4
- **Halt direction**: Crystalka 2026-05-13 «Option 1 — Halt fully, re-deliberate»
- **Working tree state at halt**: clean except this new file; HEAD = `f7e6d52` (K8.3 v2.0 brief authoring commit)
- **K8.3 v2.0 brief authoring lineage**: see brief §11 — authored 2026-05-13 Opus deliberation post-K-L3.1 + post-A'.4.5
- **Related governance artifacts** (to author post-deliberation):
  - EVT-2026-05-13-K8.3-EXECUTION-HALT (audit_trail)
  - CAPA-2026-05-13-K8.3-PREMISE-MISS (capa_entries)
  - REGISTER.yaml entry for this artifact (DOC-E-A_PRIME_5_K8_3_HALT_INVESTIGATION)
  - METHODOLOGY v1.8 lesson if Crystalka adopts §7 above

---

**Artifact end. Deliberation session input ready. Brief v2.0 lifecycle remains AUTHORED pending Crystalka + Opus resolution.**
