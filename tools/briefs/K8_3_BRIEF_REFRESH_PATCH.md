---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K8_3_BRIEF_REFRESH_PATCH
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K8_3_BRIEF_REFRESH_PATCH
---
# K8.3 v2 Brief Refresh Patch — Companion to K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md v2.0

**Status**: AUTHORED 2026-05-13 — companion patch to `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` v2.0 (authored 2026-05-13 earlier same session).
**Scope**: Corrects critical storage premise miss discovered during Claude Code execution attempt 2026-05-13. K8.3 v2.0 brief presupposed component storage already moved to NativeWorld at K8.3 time; reality is K8.4 ships the storage move.
**Authority**: This patch overrides specific Phase 0/§3/§4/§6 statements in K8.3 v2.0 brief. K8.3 v2.0 architectural intent (per-system migration to SpanLease/WriteBatch pattern) preserved; **execution prerequisite re-ordered**: K8.4 storage move must land before K8.3 system migrations begin.
**Milestone**: A'.5 (K8.3 v2 + this patch + K8.4 v2 forthcoming, bundled per scope correction).
**CAPA opened**: CAPA-2026-05-13-K8.3-PREMISE-MISS — brief authoring missed explicit GameBootstrap.cs comment denying K8.3 storage premise. RISK-008 reproduction (amendment plan completeness gap); caught structurally via «stop, escalate, lock» METHODOLOGY §3 at execution attempt.

---

## ⚠ READ ORDER (critical — read this section first)

You are the executor of the Claude Code session opened to perform Milestone A'.5 (K8.3 — Production System Migration to SpanLease/WriteBatch). Two briefs are/were attached to this session:

1. **`tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md`** — this document
2. **`tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md`** — the main K8.3 v2.0 brief (authored 2026-05-13 earlier today)

**Read THIS document FIRST.** Then re-read K8.3 v2.0 brief with overrides applied mentally.

Reason: K8.3 v2.0 brief Phase 0 + §3 + §4 + §6 are authored under premise that component storage already lives in NativeWorld at K8.3 execution time. **This premise is false at HEAD = 056579f (A'.4.5 closure state)**. Component storage moves to NativeWorld at K8.4, not K8.2 v2.

You correctly halted execution at Phase 0 / Phase 1 surfacing this discrepancy. This patch resolves the discrepancy by **re-ordering K8.3 vs K8.4 execution** — K8.4 ships storage move first, then K8.3 v2.0 brief applies cleanly against post-K8.4 state.

---

## ⚠ EXECUTION ORDER RESOLUTION (the load-bearing decision this patch carries)

**Lock**: Q-K8.3-RESCOPING ratified by Crystalka 2026-05-13 («Да давай писать дополнение как и раньше было отправить в ту сессию K8.3v2 как патч»).

**Resolution**: **Option 2 from Claude Code's halt prompt — swap K8.3 and K8.4 order**. Internal re-scoping:

- **K8.4 (executes first, becomes A'.5)**: storage migration + Mod API v3 ships (`RegisterComponent<T>` direct registration into NativeWorld; `Fields`; `ComputePipelines`; `RegisterManagedComponent<T>`). **Managed World retained as fallback during transition** — not retired in K8.4.
- **K8.3 (executes second, becomes A'.6)**: 12 production systems migrate to SpanLease/WriteBatch against already-moved NativeWorld storage. K8.3 v2.0 brief applies verbatim post-K8.4 — premise restored.
- **K8.5 (executes third, becomes A'.7)**: managed World retirement (was K8.4 scope) + ecosystem migration prep + `[ModAccessible]` annotation completeness (was K8.4 scope).

**Phase A' sequencing update** (per Q-A45-X5 post-session protocol, applied at K8.4 closure):
```
Phase A'.4 [DONE 2026-05-11] — K9 field storage execution
Phase A'.4.5 [DONE 2026-05-12] — Document Control Register operational
Phase A'.5 [REVISED] — K8.4 storage migration + Mod API v3 ships (was: K8.3)
Phase A'.6 [REVISED] — K8.3 system migration to SpanLease/WriteBatch (was: K8.4)
Phase A'.7 [REVISED] — K8.5 World retirement + ecosystem prep (was: K8.5; scope unchanged conceptually but absorbs World retirement from old K8.4)
Phase A'.8 [unchanged] — K-closure report
Phase A'.9 [unchanged] — Architectural analyzer milestone
```

**Decision #1 «Sequence LOCKED» verification**: Migration Plan v1.1 §0.3 Decision #1 reads «K8.2 → K8.3 → K8.4 → K8.5 → M8.4 → ... No interleaving. K-track closes before M-track begins.» This patch verifies: «No interleaving» refers to K-track vs M-track interleaving; **internal K8.x ordering not strictly load-bearing** beyond «K8.2 v2 first» (already done). Decision #1 not violated by K8.4-before-K8.3 internal swap. M-track still gated behind K-track closure.

**Lock authority**: K-L11 (NativeWorld single source of truth) implementation order is a tactical choice; the architectural goal (Phase A closure = K-completed `src/` codebase ready for M-series relocation) preserved.

---

## ⚠ COMMIT ORDERING (critical — affects current Claude Code session continuation)

Your current Claude Code session (per uploaded screenshot 2026-05-13) holds **+869/-34 working changes**. These changes were investigation/exploration of K8.3 v2.0 execution against false premise. They must NOT be committed as K8.3 execution.

**Resolution sequence**:

### Step 1 — Preserve investigation work as deliberation artifact

```powershell
# In current Claude Code session:
git diff > /tmp/k8_3_investigation_workproduct.patch
# Verify patch size: ~869 lines additions
```

Save patch file to `docs/scratch/A_PRIME_5/K8_3_HALT_INVESTIGATION.patch` AND author summary `docs/scratch/A_PRIME_5/HALT_REPORT.md` documenting:
- What Claude Code session attempted
- What halt surfaced (storage premise mismatch)
- What working changes contain (investigation/scoping/alternative-explored work)
- Pointer to this patch as resolution

### Step 2 — Reset working tree

```powershell
git checkout .
git clean -fd
# Verify clean state
git status
```

### Step 3 — Author halt artifacts atomically

```powershell
# Commit #1 on main (NOT on feat/k8-3-... branch):
git add docs/scratch/A_PRIME_5/K8_3_HALT_INVESTIGATION.patch
git add docs/scratch/A_PRIME_5/HALT_REPORT.md
git commit -m "docs(scratch): A'.5 K8.3 v2.0 execution halt — storage premise mismatch surfaced (METHODOLOGY §3 stop-escalate-lock)"
```

### Step 4 — Commit this patch on main

```powershell
# Commit #2 on main:
git add tools/briefs/K8_3_BRIEF_REFRESH_PATCH.md
git commit -m "docs(briefs): K8.3 v2 brief refresh patch — corrects storage premise, swaps K8.3/K8.4 order"
```

### Step 5 — Update REGISTER for halt event

Per Q-A45-X5 post-session protocol, halt event itself is governance event. Update REGISTER.yaml:

```yaml
audit_trail:
  - id: EVT-2026-05-13-K8.3-V2-HALT
    date: "2026-05-13"
    event: "K8.3 v2.0 execution halt — storage premise mismatch"
    event_type: governance_event  # stop-escalate-lock invocation
    documents_affected:
      - DOC-D-K8_3                    # brief still AUTHORED; revised lifecycle PENDING new patch
      - DOC-D-K8_3_BRIEF_REFRESH_PATCH  # NEW entry — this patch
    commits:
      range: "TBD"
      key_commits:
        - hash: "<halt-investigation-commit>"
          summary: "Halt investigation preserved as deliberation artifact"
        - hash: "<this-patch-commit>"
          summary: "K8.3 v2 brief refresh patch authored"
    governance_impact: |
      First post-A'.4.5 «stop, escalate, lock» invocation under register governance.
      Brief authoring premise miss caught at execution attempt before harmful commits landed.
      RISK-008 (amendment plan completeness gap) reproduced; structurally caught.
      CAPA-2026-05-13-K8.3-PREMISE-MISS opened; effectiveness verification at A'.7 K8.5 closure.
    cross_references:
      capa_entries:
        - CAPA-2026-05-13-K8.3-PREMISE-MISS
```

CAPA entry:

```yaml
capa_entries:
  - id: CAPA-2026-05-13-K8.3-PREMISE-MISS
    opened_date: "2026-05-13"
    closure_status: OPEN
    trigger: |
      K8.3 v2.0 brief authoring (2026-05-13) presupposed component storage already
      in NativeWorld at K8.3 execution time. Claude Code execution attempt surfaced
      that GameBootstrap.cs comment explicitly states «component storage stays on
      the managed World through K8.3; registry-based path lights up at K8.4».
      Brief authoring missed this comment despite reading GameBootstrap.cs file
      during Q-K8.3-1 verification.
    affected_documents:
      - DOC-D-K8_3                       # K8.3 v2.0 brief
      - DOC-D-K8_3_BRIEF_REFRESH_PATCH  # this patch
      - DOC-A-MIGRATION_PLAN             # internal K-series ordering implication
      - DOC-A-KERNEL                     # Part 2 K8.x rows
    root_cause: |
      Brief authoring read GameBootstrap.cs primarily for Q-K8.3-1 coreSystems
      verification (count + names). Comment block about storage path activation
      (lines 91-96 of GameBootstrap.cs) was not surfaced during scope verification.
      Consistency review focused on K-L3.1 reframing + A'.4.5 governance integration;
      did not deep-read K8.2 v2 closure state to verify storage premise.
    immediate_action: |
      Halt K8.3 v2.0 execution at Claude Code session 2026-05-13.
      Surface 4 resolution options via «stop, escalate, lock» chat session.
      Crystalka selects Option 2 (swap K8.3/K8.4 order) via patch brief direction.
    corrective_action: |
      (1) This patch brief authored as resolution artifact.
      (2) K8.4 v2 brief to be authored (currently skeleton state) — captures
          storage migration scope with Mod API v3 ship.
      (3) K8.5 v2 brief to be authored — absorbs managed World retirement from
          old K8.4 scope.
      (4) PHASE_A_PRIME_SEQUENCING.md amended per «Phase A' sequencing update»
          table in this patch (A'.5 K8.4, A'.6 K8.3, A'.7 K8.5).
      (5) Future brief authoring protocol enhancement: deep-read entry-point
          files (GameBootstrap.cs, Bootstrap.cs) for embedded architectural
          comments documenting current-state assumptions. Lesson added to
          METHODOLOGY §K-Lessons at next revision.
    effectiveness_verification:
      method: "K8.4 + K8.3 + K8.5 close cleanly in revised order; no further premise miss"
      date_verified: null
      verification_commit: null
      verification_outcome: null
      verification_pending: "A'.7 K8.5 closure"
    lessons_learned_reference: DOC-B-METHODOLOGY  # next revision absorbs the lesson
```

### Step 6 — Sync register

```powershell
.\tools\governance\sync_register.ps1 --validate
.\tools\governance\sync_register.ps1 --sync
.\tools\governance\render_register.ps1
```

### Step 7 — Push to origin

```powershell
git push origin main
```

### Step 8 — Halt cleanly, await next session

Current Claude Code session **terminates here**. A'.5 milestone now means «K8.4 storage migration» — separate Claude Code session opens against K8.4 v2 brief (forthcoming, authored by Opus next session).

---

## Overview — what this patch changes

K8.3 v2.0 brief is preserved in full. It applies **after K8.4 closure**, not against current state. The single load-bearing correction is execution ordering: K8.4 first, then K8.3, then K8.5.

| K8.3 v2.0 brief section | Status | Override per this patch |
|---|---|---|
| §0.1 Q-K8.3-1 (scope = 12) | UNCHANGED | 12 systems verified; lock holds |
| §0.2 Q-K8.3-2 (Path β = 0) | UNCHANGED | zero Path β production components; lock holds |
| §0.3 Q-K8.3-3 (per-system test class) | UNCHANGED | strategy holds |
| §0.4 Q-K8.3-4 (rule-based ordering) | UNCHANGED | tier classification holds |
| §0.5 Q-K8.3-5 (closure protocol exercise) | UNCHANGED conceptually | Verification milestone shifts: K8.3 now A'.6, not A'.5; CAPA-2026-05-12 effectiveness verification milestone shifts to **K8.4 closure** (A'.5) — first post-register milestone is K8.4, not K8.3 |
| §1 Goal | UNCHANGED | post-K8.4 state restores premise |
| §2 Scope inventory | UNCHANGED | 12 systems in scope |
| §3.1-§3.6 Architectural design constraints | UNCHANGED | SpanLease/WriteBatch patterns valid against post-K8.4 storage |
| §4 Per-system migration design | UNCHANGED | per-system patterns valid |
| §5 Phase 0 pre-flight | **OVERRIDDEN** — see §1 of this patch |
| §6.1 Phase 1 (.uid cleanup) | **MOVES TO K8.4** — see §2 of this patch |
| §6.2 Phase 2 (test fixture) | UNCHANGED | applies in K8.3 (= A'.6) execution |
| §6.3 Phase 3 (per-system migration) | UNCHANGED | applies in K8.3 (= A'.6) execution |
| §6.4 Phase 4 (verification) | UNCHANGED | applies in K8.3 (= A'.6) execution |
| §6.5 Phase 5 (closure) | OVERRIDDEN — references shift per §3 of this patch |
| §8 Stop conditions | UNCHANGED structurally | adds: §8.7 «storage premise verification fails at Phase 0» (this is what triggered the halt) |
| §10 Estimated commit log | **OVERRIDDEN** — see §4 of this patch |

---

## §1 K8.3 v2.0 §5 Phase 0 pre-flight — REVISED

**Critical addition**: K8.3 execution Phase 0 includes new verification step that storage migration has landed.

**§5.5 (NEW) — Verify K8.4 closure landed**:

```powershell
# Confirm K8.4 closure entry in MIGRATION_PROGRESS.md
Select-String -Path docs/MIGRATION_PROGRESS.md -Pattern "K8.4.*DONE" -Context 0,3
# Expected: K8.4 closure entry present with commits range
```

```powershell
# Confirm GameBootstrap.cs no longer carries «storage stays on managed World» comment
Select-String -Path src/DualFrontier.Application/Loop/GameBootstrap.cs -Pattern "storage stays on the managed World" -SimpleMatch
# Expected: zero matches (K8.4 closure deletes/updates this comment)
```

```powershell
# Confirm NativeWorld is the active storage path
Select-String -Path src/DualFrontier.Application/Loop/GameBootstrap.cs -Pattern "RegisterComponent" -SimpleMatch
# Expected: registry-based component registration calls present
```

```powershell
# Confirm `World.GetComponent` call sites still exist in systems (pre-K8.3) but
# managed `World` is no longer authoritative storage backbone
Select-String -Path src/DualFrontier.Systems -Pattern "World\.GetComponent" -SimpleMatch -Recurse
# Expected: 12+ matches (systems still using legacy access pattern); K8.3 migration replaces these
```

If any verification fails:
- K8.4 not yet closed: halt; await K8.4 closure
- K8.4 closure partial (storage moved but `world.GetComponent` removed from systems prematurely): halt; surface as K8.4 closure gap

**This is the storage premise verification step missing from K8.3 v2.0 brief.** Adding it explicitly prevents future re-discovery of the same gap.

---

## §2 Phase 1 .uid cleanup — MOVES TO K8.4

**Rationale**: 3 orphan `.uid` files (ShieldSystem, SocialSystem, BiomeSystem) are independent of K8.3 v2.0 scope. They are K8.2 v2 closure artifacts (deleted .cs but .uid survived). Since K8.4 lands first in revised order, cleanup naturally fits there.

**K8.4 brief (forthcoming) absorbs**:
- Storage migration (component storage moves to NativeWorld)
- Mod API v3 ship (`Fields`, `ComputePipelines`, `RegisterManagedComponent<T>`)
- **NEW**: orphan .uid cleanup (was K8.3 Phase 1)
- **NOT in K8.4**: managed World retirement (deferred to K8.5)

**K8.3 v2.0 Phase 1 (.uid cleanup) becomes no-op** at K8.3 (= A'.6) execution time because K8.4 already cleaned. K8.3 v2.0 brief §6.1 reads as: «Phase 1 verifies cleanup complete (already done by K8.4); zero-action commit OR skipped entirely; reduce commit count by 1.»

---

## §3 Phase 5 closure references — REVISED

K8.3 v2.0 §6.5 Phase 5 closure documents amendments. Two references change:

### §3.1 — Phase 5.1 amendment target

**Original (K8.3 v2.0 §6.5 Phase 5.1)**: «Amend MIGRATION_PLAN v1.1 → v1.2 (non-semantic correction)».

**Override**: Migration Plan version at K8.3 (= A'.6) closure depends on K8.4 (= A'.5) closure deliverables:
- If K8.4 closure amends Migration Plan v1.1 → v1.2 (storage migration scope ratified), then K8.3 amends v1.2 → v1.3.
- If K8.4 closure leaves Migration Plan unchanged, K8.3 amends v1.1 → v1.2 per original K8.3 v2.0 Phase 5.1.

**Recommended**: K8.4 brief authoring (forthcoming) absorbs the K8.3/K8.4 ordering swap as part of its own Phase 5.1 amendment. K8.3 closure then refines further if needed. K8.4 closure produces Migration Plan v1.2 (storage migration scope correct + ordering swap recorded); K8.3 closure produces Migration Plan v1.3 (K8.3 scope reformulated to 12 + post-K8.4 dependency stated explicitly).

### §3.2 — Phase 5.5 REGISTER.yaml updates

**Original (K8.3 v2.0 §6.5 Phase 5.5)**: «DOC-D-K8_3 lifecycle: AUTHORED → EXECUTED». 

**Override**: K8.3 v2.0 brief lifecycle: AUTHORED → EXECUTED at A'.6 closure (not A'.5). This patch (DOC-D-K8_3_BRIEF_REFRESH_PATCH) lifecycle: AUTHORED → EXECUTED at A'.6 closure as well (consumed during K8.3 v2.0 execution + halt-resolution-mechanism artifact retained).

**CAPA-2026-05-13-K8.3-PREMISE-MISS effectiveness verification milestone**: K8.5 closure (A'.7). The CAPA closes when K8.4 → K8.3 → K8.5 sequence completes cleanly without further premise miss surfacing. If K8.4 execution surfaces additional brief authoring premise misses, CAPA remains OPEN; cascading patches may be needed.

### §3.3 — Phase 5.5 CAPA-2026-05-12 effectiveness verification

**Original (K8.3 v2.0 §6.5)**: «CAPA-2026-05-12-A_PRIME_0_5-COUNT-DRIFT closure with effectiveness verification if validation passes clean» (at K8.3 closure as first post-register milestone).

**Override**: «First post-register milestone» role shifts to **K8.4 closure (A'.5)**. CAPA-2026-05-12 effectiveness verification migrates to K8.4 closure. K8.3 closure (A'.6) becomes **second** post-register milestone — additional empirical evidence for RISK-010 mitigation status.

This is positive — register protocol gets two independent verification opportunities (K8.4 + K8.3) instead of one. More falsification surface area.

---

## §4 Estimated atomic commit log — REVISED

K8.3 v2.0 §10 estimated 16 commits. **K8.3 v2.0 brief commit count reduces by 1** (Phase 1 .uid cleanup moves to K8.4):

**Revised K8.3 (= A'.6) execution commit log**:
```
01. feat(tests): NativeWorldTestFixture shared fixture for K8.3 system tests
02. feat(systems): migrate NeedsSystem to SpanLease/WriteBatch (K8.3 1/12)
03. feat(systems): migrate MoodSystem to SpanLease/WriteBatch (K8.3 2/12)
04. feat(systems): migrate JobSystem to SpanLease/WriteBatch (K8.3 3/12)
05. feat(systems): migrate PawnStateReporterSystem to SpanLease/WriteBatch (K8.3 4/12)
06. feat(systems): migrate ConverterSystem to SpanLease/WriteBatch (K8.3 5/12)
07. feat(systems): migrate ConsumeSystem to SpanLease/WriteBatch (K8.3 6/12)
08. feat(systems): migrate HaulSystem to SpanLease/WriteBatch (K8.3 7/12)
09. feat(systems): migrate ElectricGridSystem to SpanLease/WriteBatch (K8.3 8/12)
10. feat(systems): migrate SleepSystem to SpanLease/WriteBatch (K8.3 9/12)
11. feat(systems): migrate MovementSystem to SpanLease/WriteBatch (K8.3 10/12)
12. feat(systems): migrate ComfortAuraSystem to SpanLease/WriteBatch (K8.3 11/12)
13. feat(systems): migrate InventorySystem to SpanLease/WriteBatch (K8.3 12/12)
14. test(systems): K8.3 post-migration verification — performance sanity + KCR tags
15. docs(closure,governance): A'.6 K8.3 closure — 12 production systems migrated

Total: 15 atomic commits at K8.3 (= A'.6) execution time
```

**This patch atomic commit log at HALT time (current session)**:
```
01. docs(scratch): A'.5 K8.3 v2.0 execution halt — storage premise mismatch surfaced
02. docs(briefs): K8.3 v2 brief refresh patch — corrects storage premise, swaps K8.3/K8.4 order
03. docs(governance): REGISTER halt event + CAPA-2026-05-13 entry recorded

Total: 3 commits at halt resolution (this session)
```

---

## §5 What this patch does NOT do

- **Does not author K8.4 v2 brief.** K8.4 currently skeleton state. Forthcoming Opus session authors K8.4 v2 brief with storage migration scope. K8.4 brief absorbs orphan .uid cleanup + ordering swap rationale + storage move execution shape.
- **Does not author K8.5 v2 brief.** K8.5 currently skeleton state. Forthcoming Opus session authors K8.5 v2 brief absorbing managed World retirement (from old K8.4 scope) + ecosystem migration prep + analyzer milestone preparation.
- **Does not retroactively amend K8.3 v2.0 brief on disk.** K8.3 v2.0 brief is preserved as authored. This patch sits alongside as override layer (K9 brief refresh patch precedent). The pair (K8.3 v2.0 brief + this patch) becomes input to K8.3 (= A'.6) execution session, post-K8.4 closure.
- **Does not predict K8.4 internal execution.** K8.4 v2 brief authoring is separate Opus session. This patch only specifies that K8.4 closure must happen before K8.3 execution begins.

---

## §6 Storage premise — explicit statement

The premise miss is recorded here explicitly to prevent future repetition:

**False premise (K8.3 v2.0 brief)**: «Post-K8.2 v2 closure, all production components are unmanaged structs in NativeWorld. K8.3 systems read/write via SpanLease/WriteBatch against NativeWorld storage.»

**Correct state at HEAD = 056579f (A'.4.5 closure)**: «Post-K8.2 v2 closure, all production components are unmanaged structs IN MANAGED WORLD. K8.1 native primitives (InternedString, NativeMap, NativeSet, NativeComposite) live in NativeWorld. Component storage backbone is still managed World. K8.4 moves storage to NativeWorld via registry-based component registration. K8.3 migrates 12 production systems to SpanLease/WriteBatch against POST-K8.4 NativeWorld storage.»

**Authority for correction**: `src/DualFrontier.Application/Loop/GameBootstrap.cs` lines 91-96 verbatim comment:

```csharp
// K8.2 v2 — NativeWorld owned alongside the managed World until K8.4
// retires the latter. Production uses NativeWorld only for K8.1
// primitives (string interning, native maps/sets/composites bound to
// struct components); component storage stays on the managed World
// through K8.3. Bootstrap path uses FNV-1a fallback type ids; the
// registry-based path lights up at K8.4 when component storage moves.
```

This comment was present at K8.3 v2.0 brief authoring time. Brief authoring read GameBootstrap.cs for Q-K8.3-1 coreSystems verification but did not surface this comment block. Lesson recorded in CAPA-2026-05-13 corrective action #5.

---

## §7 Lesson encoded for future brief authoring

When authoring a milestone brief that depends on architectural state delivered by **previous** milestones:

1. **Read entry-point files in full**, not just for specific scope verification (Q-K8.3-1 mode). GameBootstrap.cs, Bootstrap.cs, Application/Program.cs, and similar orchestration files often carry **embedded comments documenting transitional state assumptions** that are load-bearing for downstream milestones.

2. **Verify storage / lifecycle premise explicitly** in Phase 0 pre-flight, not just inventory premise. Phase 0 of every brief should include verification step: «X has happened» before relying on X-already-happened-implicit-premise. This is the structural fix this patch encodes.

3. **Cross-reference K-Lxx implication paragraphs**, not just K-Lxx headlines. KERNEL_ARCHITECTURE Part 0 K-L11 «Implication» paragraph reads «Post-K8.4 closure, NativeWorld is the only production storage path». This *implies* pre-K8.4, NativeWorld is NOT the only storage path. K8.3 v2.0 brief authoring referenced K-L11 but did not parse the «Post-K8.4» qualifier.

4. **«Stop, escalate, lock» is operational, not exceptional.** Claude Code session 2026-05-13 invocation of stop-escalate-lock at K8.3 execution attempt is precisely how METHODOLOGY §3 should operate. Halt was not failure; it was **structurally correct behavior catching brief authoring premise miss** before harmful commits landed. This is the methodology working as designed.

5. **Patch briefs are first-class artifacts**, not workarounds. K9_BRIEF_REFRESH_PATCH precedent (2026-05-10) demonstrates the pattern. This K8.3 v2 patch follows the same shape. Future briefs should expect occasional patch companions; this is not exceptional.

---

## §8 Brief authoring lineage

- **2026-05-13 morning**: K8.3 v2.0 full brief authored (Opus deliberation session); 5 Q-K8.3 locks ratified; brief uploaded to Claude Code session
- **2026-05-13 afternoon**: Claude Code session attempted K8.3 v2.0 execution; halted at Phase 0/1 surfacing storage premise mismatch (working tree +869/-34); 4 resolution options surfaced to chat
- **2026-05-13 afternoon**: Crystalka selected patch brief approach («Да давай писать дополнение как и раньше было отправить в ту сессию K8.3v2 как патч»); Option 2 (swap K8.3/K8.4 order) chosen as resolution
- **2026-05-13 afternoon**: This patch authored as resolution artifact
- **(TBD)** — Claude Code session commits halt artifacts + this patch + REGISTER updates; session terminates
- **(TBD)** — Opus session authors K8.4 v2 brief absorbing storage migration scope + ordering swap rationale + orphan .uid cleanup
- **(TBD)** — Claude Code session executes K8.4 v2 brief; A'.5 closure (revised: K8.4, was K8.3)
- **(TBD)** — Opus session authors K8.5 v2 brief absorbing World retirement
- **(TBD)** — Claude Code session executes K8.3 v2.0 brief + this patch as override; A'.6 closure (revised: K8.3, was K8.4)
- **(TBD)** — Claude Code session executes K8.5 v2 brief; A'.7 closure (revised K8.5 absorbs World retirement)
- **(TBD)** — CAPA-2026-05-13-K8.3-PREMISE-MISS closes at A'.7 if K8.4 → K8.3 → K8.5 cascade clean

---

**Patch end. Awaits halt artifact commits + register updates + termination of current Claude Code session.**
