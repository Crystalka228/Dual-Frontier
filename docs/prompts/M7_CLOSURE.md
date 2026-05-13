---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-M7_CLOSURE
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-M7_CLOSURE
---
# M7 closure verification — brief for closure session

## Purpose

Standalone brief for executing the M7 closure verification session. Produces `docs/audit/M7_CLOSURE_REVIEW.md` parallel to existing `M3_CLOSURE_REVIEW.md`...`M6_CLOSURE_REVIEW.md`. Marks M7 row → ✅ Closed in `ROADMAP.md`. **Crucially**: documents the first formal hypothesis falsification in the M-cycle datapoint sequence (§9.2 menu-pause gap surfaced post-M7.5.B.2 via F5 manual verification) and registers the §9.2 v1.6 ratification candidate explicitly.

This brief is specifically NOT a code-change session. It is a **verification-only** closure following METHODOLOGY §2.4 atomic phase review and §7.3 process discipline. Deviations from M3-M6 closure structure are minimal and explicitly justified where they exist (multi-sub-phase tally, housekeeping commits documented as separate sequences from M-cycle commits, hypothesis falsification formalized).

## Operating principle (load-bearing)

> «Будем работать без заглушек которые обманывают состояние, оно либо есть, либо его нет вообще.»

Apply here: M7 closure documents what M7 actually shipped, what it did NOT ship, and what was discovered during F5 manual verification post-closure. The §9.2 menu-pause gap is the strongest possible falsifiable signal — the empirical contradiction-rate hypothesis registered in M5/M6 closure reviews predicted "≤ 1 contradiction at M7"; the verification cycle delivered exactly 1 (one §9.2 wording-vs-implementation gap). Hypothesis preserved, datapoint sequence honest. We do not invent additional contradictions to inflate the number, and we do not hide the surfaced one to preserve the streak.

## Out of scope

- ANY code change to `src/`. Closure is verification. The §9.2 v1.6 ratification candidate is **registered for future cycle**, not addressed in this session.
- ANY change to `docs/architecture/MOD_OS_ARCHITECTURE.md`. Spec wording stays at v1.5 LOCKED through M7. v1.6 ratification belongs to a follow-up cycle informed by the §9.2 candidate. **Surgical fixes** to typos / broken cross-references / clearly-wrong facts in newly-added M7 documentation are allowed per §9 of M3-M6 closure precedent — but only those.
- ANY change to test fixtures or assertions. The 437/437 test count is the closure baseline.
- M8 (Vanilla skeletons) preparation work. M7 closes; M8 is a separate session.
- UI redesign (Kenney + Cinzel) brief — separate larger work, decisions still pending.
- `.sln` build verification gap — separate small housekeeping brief; the M7 closure documents the gap as a finding under §10.
- Phase 5 backlog work.

## Approved approach

1. **Closure document location: `docs/audit/M7_CLOSURE_REVIEW.md`.** Parallel to existing M3-M6 reviews. `nav_order: 99` (the next slot after M6's 98). Front-matter per existing convention (title + nav_order in YAML).

2. **Structure: 8-check format from M3-M6 precedent.** Identical eight numbered checks (§0 executive summary table → §1-§8 detailed verifications). Extensions (multi-sub-phase tally, housekeeping commits, hypothesis falsification) live within the existing sections rather than new top-level sections.

3. **Multi-sub-phase tally in §1.** M7 has 7 sub-phases (M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, M7.5.B.2) plus 5 housekeeping passes (TICK display, TickScheduler race, real pawn data, needs decay direction, ModMenuPanel position + assets gitignore, menu-pauses-simulation). The three-commit invariant table in §1 lists every M-cycle commit AND every housekeeping commit with explicit visual separation between the two sequences. Total commit count: 7 sub-phases × 3 commits + 6 housekeeping × 3 commits = 21 + 18 = 39 commits. Verify the actual count via `git log` during execution; report deviations.

4. **Housekeeping documented as auxiliary sequence.** M-cycle three-commit invariant applies per sub-phase (M7.1 = 3 commits, etc.); housekeeping commits also follow three-commit (fix/test/docs) but are explicitly NOT M-cycle commits. The §1 table separates them visually with a horizontal separator row labeled "── housekeeping ──" so the reader cannot confuse a housekeeping commit count delta with an M-sub-phase commit count delta.

5. **Hypothesis falsification formalized in §10.** The empirical contradiction-rate observation registered in M5_CLOSURE_REVIEW.md §10 and extended in M6_CLOSURE_REVIEW.md §10 receives its **fifth datapoint** from M7. The datapoint is **1** (one §9.2 menu-pause gap surfaced via F5 manual verification post-M7.5.B.2 closure). Sequence becomes: M3=1, M4=1, M5=0, M6=0, **M7=1**.

   **The hypothesis is NOT falsified by this datapoint.** The forward claim from M6_CLOSURE_REVIEW §10 was "M7 (which exercises §9 lifecycle + §10.4 WeakReference unload) discovers ≤ 1 latent contradiction". M7 produced exactly 1 → forward claim **holds**. The asymptotic-decrease hypothesis ("contradiction discovery rate decreases asymptotically with each major implementation pass") was already weakened by M7 producing nonzero, but the hypothesis was always falsifiable and now has its first nonzero post-M4 datapoint to test against.

   The §10 entry quotes the M5/M6 forward claims verbatim and reports their status:
   - M5_CLOSURE_REVIEW §10: "M7 alone produces spec-amendment count ≥ 2 OR cumulative count across M3-M10 exceeds 4" → falsifies. M7 produced 1 (not ≥ 2); cumulative through M7 is 3 (not > 4). **Hypothesis preserved.**
   - M6_CLOSURE_REVIEW §10: "M7 discovers ≤ 1 latent contradiction" → exactly 1. **Forward claim met.**
   - Forward claim for M8-M10: cumulative count across M3-M10 must remain ≤ 4. With 3 already used (M3, M4, M7), M8-M10 must collectively contribute ≤ 1 contradiction. Falsifiable forward target.

6. **§9.2 v1.6 ratification candidate registered in §10** with full diagnosis and forward path. The candidate is **registered**, not **resolved** — resolution belongs to a future v1.6 ratification cycle. Section text covers: the spec wording («menu pauses the scheduler»), the implementation (two pause flags), the orchestration-layer wiring (the M7-closure-followup-housekeeping fix), three resolution options for v1.6 (refactor toward unified contract / explicitly enumerate two surfaces / document orchestration as canonical), and the recommendation that resolution wait for M8+ load patterns to inform which option scales best.

7. **§7.5 fifth scenario closure verification.** M6 closure registered «§7.5 fifth scenario (mod-unloaded replacement-revert) is M6→M7 hand-off». M7 closure must verify this hand-off is met: the test surface that exercises mod unload AND post-unload kernel bridge re-registration. Run `grep -rn "ReplacementRevert\|UnloadRevert\|UnloadedMod_RestoresKernel" tests/` to find the exact test name. If the scenario was implicitly covered (e.g., M7.2 unload chain tests + M6.2 replacement skip tests run independently but no joint test exists), §7 documents this as a **carried hand-off to M8 or M9** rather than asserting closure. **Honest reporting required**: if M6→M7 hand-off was deferred without explicit registration, §7 surfaces the deferral, does not pretend it landed.

8. **Spec byte-identity verification in §3 and §5.** `git diff <M6-closure-baseline>..HEAD -- docs/architecture/MOD_OS_ARCHITECTURE.md`. M6 closure baseline was `e643011`. Expected: zero output if v1.5 is byte-identical to the v1.4-locked-at-M6-closure state. M7 introduced **at least one** spec change — the v1.4 → v1.5 transition documented in user memory occurred during M7. So zero-output is NOT expected; the diff IS expected to show v1.4 → v1.5 transition with documented changelog entry. §3 and §5 capture the actual diff and verify it matches the intended changelog (no silent ratifications).

9. **Cross-document consistency check in §3.** Same three documents as M6 closure: `MOD_OS_ARCHITECTURE.md` (v1.5 LOCKED expected), `ROADMAP.md` (header date + M7 row + engine snapshot), `docs/README.md` (v1.5 LOCKED expected). Plus auxiliary: every housekeeping prompt file in `docs/prompts/` referenced by the relevant ROADMAP closure entries.

10. **§4 stale-reference sweep extended for M7-specific patterns.** Forbidden in active-navigation context: `M7 in progress`, `M7.5.B.2 pending`, `🔨 Current` (M7 row should be ✅ Closed), test counts that pre-date M7 closure (`311`, `328`, `333`, `338`, `415`, `416`, `417`, `428`, `431`, `434` — all prior intermediate counts). Allowed in historical/audit-trail context (M3-M6 closure reviews, this document's own §1 three-commit table). The current-state count is **437**.

11. **§7 carried debts forward inventory.** Phase 5 backlog (now categorized into 3 subsections per the housekeeping commits) needs explicit listing in §7 with target M-phase per entry. Plus: M7 introduces specific carried items — the §9.2 v1.6 ratification candidate (target: future v1.6 cycle), the `.sln` build verification gap (target: small housekeeping post-closure or M8 prep), the UI redesign brief (target: Phase 5 / M10 polish).

12. **§8 ready-for-M8 readiness check.** Mirror M5-closes-readiness-for-M6 and M6-closes-readiness-for-M7 patterns. M8 needs: vanilla mod skeleton scaffolding (5 mods per the spec), build pipeline support (M7.4 already shipped — verify), mod loader path (M0-M2 + M4 + M7 chain — verify), menu integration so a user can toggle vanilla mods in the menu (M7.5 chain — verify, including post-fix menu-pauses-simulation behavior).

13. **§9 surgical fixes section reserved.** Same M3-M6 convention — "None" expected unless during execution a typo / broken xref / clearly-wrong fact surfaces in M7-introduced documentation. If any surgical fix is applied, it must be in its own commit separate from the closure-document commit. Closure document + surgical fix = up to 2 commits this session. If surgical fixes ≥ 2, that is itself a finding worth surfacing.

14. **§10 items requiring follow-up structured.** Three observation classes from M3-M6 precedent: empirical-rate datapoint, M-future architectural considerations, registered ratification candidates. M7 contributes the §9.2 candidate as registered candidate, the empirical-rate fifth datapoint, and any architectural considerations surfaced during closure work.

15. **METHODOLOGY §2.4 + §7.3 compliance for closure session itself.** Closure session is single-commit by convention (or closure + surgical fix = 2 commits). Three-commit invariant doesn't apply to docs-only closure work — verify the M3-M6 closures were single-commit and follow that precedent.

## Required reading

1. **Existing closure precedent** (full files):
    - `docs/audit/M3_CLOSURE_REVIEW.md` — origin of 8-check format. Read fully.
    - `docs/audit/M4_CLOSURE_REVIEW.md` — formalization of multi-document consistency check.
    - `docs/audit/M5_CLOSURE_REVIEW.md` — first contradiction-rate hypothesis registration; cascade-failure interpretation precedent.
    - `docs/audit/M6_CLOSURE_REVIEW.md` — most recent precedent; format and tone target. Specifically read §10 contradiction-rate observation for hypothesis-extension precedent.

2. **M7 implementation artifacts** (head only, full only if closure section requires deep verification):
    - `docs/prompts/M73_CODING_STANDARDS_UPDATE.md`
    - `docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md`
    - `docs/prompts/M75A_MOD_MENU_CONTROLLER.md`
    - `docs/prompts/M75B1_BOOTSTRAP_INTEGRATION.md`
    - `docs/prompts/M75B2_GODOT_UI_SCENE.md`
    - `docs/prompts/M7_HOUSEKEEPING_TICK_DISPLAY.md`
    - `docs/prompts/HOUSEKEEPING_TICKSCHEDULER_RACE.md`
    - `docs/prompts/HOUSEKEEPING_REAL_PAWN_DATA.md`
    - `docs/prompts/HOUSEKEEPING_NEEDS_DECAY_DIRECTION.md`
    - `docs/prompts/HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md`
    - `docs/prompts/HOUSEKEEPING_MENU_PAUSES_SIMULATION.md`
    - `docs/audit/UI_REVIEW_PRE_M75B2.md` — pre-implementation audit document, referenced from housekeeping briefs.

3. **Current state documents**:
    - `docs/architecture/MOD_OS_ARCHITECTURE.md` — verify v1.5 LOCKED status; identify v1.4→v1.5 changelog entry; locate §9.2 wording.
    - `docs/ROADMAP.md` — locate M7 row; verify all 7 sub-phases marked ✅ Closed; verify engine snapshot at 437/437.
    - `docs/README.md` — verify spec-version pointer.
    - `.gitignore` — verify Kenney + Cinzel patterns from `0b11d1f` housekeeping.

4. **Tests** (head only):
    - `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` — verify all 6 MenuFlow tests.
    - `tests/DualFrontier.Modding.Tests/M71PauseResumeTests.cs` and siblings — verify M7.1-M7.4 surfaces.

5. **Spec sections relevant to M7**:
    - §9.2 (menu flow Pause-Toggle-Apply-Resume) — the candidate's home section.
    - §9.3 (scheduler run flag enforcement) — context for the two-flag situation.
    - §9.5 + §9.5.1 (unload chain steps 1-7 + best-effort failure discipline) — M7.2, M7.3 territory.
    - §9.6 (hot-reload disabled per-mod) — M7.5.A interpretation home.
    - §11.1 M7 row — acceptance bullets.

6. **METHODOLOGY**:
    - §2.4 atomic phase review.
    - §7.3 process discipline.
    - §11.4 stop-condition discipline (relevant if surgical fixes scope creeps).

Pre-flight verification commands:

```
# Confirm baseline state.
git log --oneline -1                                # expect 110ad61 (M7.5.B.2 menu-pauses-simulation closure)
dotnet test                                         # expect 437/437 passing

# Confirm spec version.
grep -E "^.*v1\.[0-9]+ LOCKED" docs/architecture/MOD_OS_ARCHITECTURE.md | head -5
grep -E "^.*v1\.[0-9]+ LOCKED" docs/README.md | head -5

# Verify M7.5.B.2 sub-phase status in ROADMAP.
grep -n "M7.5.B.2" docs/ROADMAP.md | head -10

# Verify all M-cycle prompt files exist for §1 commit-tally cross-reference.
ls docs/prompts/M7*.md docs/prompts/HOUSEKEEPING*.md

# Identify the M6 closure baseline commit.
grep -E "^.*\(commits .*\.\..*," docs/audit/M6_CLOSURE_REVIEW.md | head -1
# Expected output: refers to e643011 as M6 closure HEAD; that's our M7-closure baseline.

# Verify §9.2 wording in spec.
grep -A 5 "## 9.2\|### 9.2\|^9.2 " docs/architecture/MOD_OS_ARCHITECTURE.md | head -30
```

## Implementation

### Single deliverable: `docs/audit/M7_CLOSURE_REVIEW.md`

Structure mirrors M6 review (verbatim section headers + spirit), with M7-specific content. Section-by-section guidance:

#### YAML front-matter

```yaml
---
title: M7 closure verification report
nav_order: 99
---
```

(M3-M6 used 95-98; 99 is the next slot.)

#### Title + frontmatter prose

```markdown
# M7 — Hot reload from menu closure verification report

**Date:** 2026-05-03
**Branch:** `feat/m4-shared-alc` (commits `<commit-A>..<commit-B>`,
N commits inclusive across 7 M-sub-phases and 6 housekeeping passes;
branch is M commits ahead of `origin/main` and not yet pushed)
**Scope:** Verification only. No new architectural work. Surgical fixes
applied only for typos, broken cross-references, or clearly-wrong facts
in the new documents. Any structural finding is recorded in §10 as a
follow-up item rather than remediated in this session.
```

Replace `<commit-A>..<commit-B>` with actual commit range from `git log`. Replace N with actual commit count, M with `git log origin/main..HEAD --oneline | wc -l`.

#### §0 Executive summary table

8 rows, one per check, mirroring M6 §0 table format. Status = PASSED for all 8 unless something surfaces. Notes column is the M7-specific interpretation. Closing line: "M7 closes; M8 (Vanilla skeletons) is unblocked." Plus separate paragraph noting:

- Hypothesis fifth datapoint = **1** (the §9.2 menu-pause gap from M7.5.B.2 follow-up). Forward claim from M6 («M7 ≤ 1 contradiction») holds. M5/M6 cumulative falsification target («cumulative across M3-M10 ≤ 4») still holds at 3.
- §9.2 v1.6 ratification candidate registered in §10. Resolution deferred to future v1.6 cycle.
- Two non-blocking items registered in §10: `.sln` build verification gap; UI redesign brief decisions pending.

#### §1 Build & test integrity

Build succeeded line. Test totals table (Persistence 4 / Systems 19 / Modding 346 / Core 61, ManifestRewriter 7 / Total 437; verify exact splits per the M7.5.B.2 closure report breakdown).

**Three-commit invariant table — extended format:**

```markdown
### Three-commit invariant (METHODOLOGY §7.3)

Each M-sub-phase comprises three atomic commits (`feat → test → docs`)
per METHODOLOGY §7.3. Each housekeeping pass also comprises three
atomic commits (`fix → test → docs` or `feat → test → docs` per its
nature). All commits in the M7 closure batch were checked out
independently against a clean working tree; build was 0/0 at every
checkout, tests passed at every checkout. Per-commit total test counts:

| # | Commit | Sub-phase / Housekeeping | Subject (truncated) | Build | Tests |
|---|---|---|---|---|---|
| — | `e643011` | (M6 closure) | docs(review): M6 closure verification report | 0 W / 0 E | **338** |
| 1 | `<sha>` | M7.1 | feat(modding): Pause/Resume + IsRunning … | 0 W / 0 E | **<count>** |
| ... | ... | ... | ... | ... | ... |
| ── | ── | ── housekeeping ── | ── | ── | ── |
| ... | ... | ... | ... | ... | ... |
```

Every commit listed; the `── housekeeping ──` separator row appears between M7.5.B.2's third commit and the first housekeeping commit, then again before each new housekeeping pass if needed. M-cycle progression flows top-to-bottom; housekeeping commits at their actual `git log` positions in the timeline. Final line: total at HEAD = 437.

#### §2 Spec ↔ code ↔ test triple consistency

Mirror M6 §2 table format. Acceptance bullets for M7 come from §11.1 M7 row plus the §9 LOCKED chapter (specifically §9.2-§9.6 + §10.4 WeakReference unload). Each row has: spec section, code file:line, test name. M7 has more acceptance bullets than M6 — expect 12-18 rows (vs M6's 9). Group by sub-phase if helpful; M3-M6 didn't need to but the sheer volume here may benefit. Format suggestion:

```markdown
| # | Acceptance | Sub-phase | Spec leg | Code leg | Test leg |
|---|---|---|---|---|---|
| 1 | Pause/Resume + IsRunning | M7.1 | §9.2, §9.3 | ... | ... |
| 2 | Apply paused-only guard | M7.1 | §9.3 | ... | ... |
...
```

The "Sub-phase" column is the M7 extension; M3-M6 didn't need it because their sub-phases (M5.1+M5.2 / M6.1+M6.2) had only 2 each. M7 has 7, so the extension is warranted.

**Special acceptance bullet: §7.5 fifth scenario (M6→M7 hand-off).**
Verify whether M7 closed the §7.5 fifth scenario (mod-unloaded ⇒
replacement-revert ⇒ kernel-bridge-reregister). Possible outcomes:

- **(A) Closed by joint test.** Run `grep -rn "ReplacementRevert\|UnloadedMod_RestoresKernel\|UnloadMod.*BridgeReregister" tests/`. If a test exists asserting this end-to-end behavior — record it as the test leg, mark closed.

- **(B) Implicitly covered.** M7.2 + M7.3 unload tests independent of M6.2 replacement skip tests, but the structural argument from M6_CLOSURE_REVIEW §8 ("re-Apply without unloaded mod" naturally rebuilds with kernel bridge re-registered) holds — record as **structural verification** with: "M7.2 unload chain removes mod from `_activeMods`; subsequent `Apply` invocations naturally exclude the mod's `Replaces` from `CollectReplacedFqns`'s aggregate set; the kernel bridge is re-registered via the unmodified Core-systems loop. No joint end-to-end test was added during M7 — the structural argument from M6_CLOSURE_REVIEW §8 holds and is verified by the disjoint tests M7.2 (`UnloadMod` removes from `_activeMods`) and M6.2 (`CollectReplacedFqns` is parameter-driven)."

- **(C) Carried hand-off (deferral)**. If neither test exists nor the structural argument holds — register as carried debt to M8 or M9. **Honest reporting**: do not pretend (A) or (B) if the truth is (C).

The actual outcome must be determined during execution by inspecting the test surface. The brief does NOT pre-commit to (A) / (B) / (C); it requires the executor to determine factually.

#### §3 Cross-document consistency

Mirror M6 §3 table format. Documents to verify:

- `MOD_OS_ARCHITECTURE.md` — v1.5 LOCKED status line. Compare byte-diff against v1.4 LOCKED state (M6 closure baseline `e643011`); expect a non-empty diff this time, since v1.4 → v1.5 happened during M7. Verify the diff matches the documented v1.5 changelog entry. If a silent ratification surfaces (diff content not matching changelog), surface as a finding under §10.
- `ROADMAP.md` header date (`Updated: 2026-05-03`), M7 row (`✅ Closed`), engine snapshot (`437/437`), all 7 M7 sub-phase status entries (each `✅ Closed` with commit SHAs), housekeeping section (each `✅` with commit SHAs).
- `docs/README.md` — v1.5 LOCKED reference.
- `.gitignore` — Kenney + Cinzel patterns present (per `0b11d1f` housekeeping).

#### §4 Stale-reference sweep

Mirror M6 §4 table format. Patterns specific to M7 closure:

- `M7 in progress` / `🔨 Current` (M7 row) / `M7.X.X pending` for any X — should return **zero hits in active-navigation context**.
- `Updated: 2026-04-30` / `Updated: 2026-05-01` / `Updated: 2026-05-02` — should return zero hits in `ROADMAP.md` header (the active date).
- Test counts that pre-date 437: `311`, `328`, `333`, `338` (M3-M6 closure counts), `415`, `416`, `417`, `428`, `431`, `434` (M7-intermediate counts). Each should match only in M3-M6 closure review files (frozen audit trail) or in ROADMAP commit-progression text describing intermediate states. Active-navigation count is **437**.
- `M8 ... ⏭ Pending` (positive: required pattern). Should appear at expected ROADMAP line.

#### §5 Methodology compliance

Mirror M6 §5 sub-sections:

- **§5.1** Commit scope prefixes table. List every M7 commit (39 expected) with prefix verification. M-cycle commits use `feat`/`test`/`docs` per §7.3; housekeeping commits use `fix`/`test`/`docs` or similar. All prefixes verified per METHODOLOGY §7.3.

- **§5.2** LOCKED decision sanctity. **DEVIATION FROM M3-M6 PRECEDENT**: M3-M6 closures all had byte-identical `MOD_OS_ARCHITECTURE.md` between baseline and HEAD. M7 has a non-empty diff because v1.4→v1.5 happened during M7. The §5.2 verdict: "v1.5 LOCKED is the supersede of v1.4. The diff between e643011 (M6 closure baseline) and HEAD shows N changes: <list>. Each change is documented in the v1.5 changelog entry at line <X> of `MOD_OS_ARCHITECTURE.md`. **D-1 through D-7 byte-identical**" (verify the §12 D-1..D-7 LOCKED decisions are byte-identical even though the document overall changed; this is the strongest possible signal that v1.5 is a non-strategic ratification).

- **§5.3** Deliberate-interpretation registrations. M7 has multiple interpretations registered in housekeeping ROADMAP entries:
  - M7.5.A AD #4 (failed Commit leaves session paused) — registered.
  - M7.5.A AD #5 (§9.6 first-load is not a reload) — registered.
  - M7.5.A AD #6 (idempotent BeginEditing/Cancel) — registered.
  - M7.3 AD #2 (capture WR → remove from active set → spin) — registered.
  - M7.3 AD #7 (step-7 runs after step-6 failure) — registered.
  - Menu-pauses-simulation housekeeping (orchestration-layer wiring of two pause flags) — registered, plus §9.2 v1.6 candidate flagged.

  All 6 interpretations explicitly registered in ROADMAP per "no improvisation" rule. List each in §5.3.

#### §6 Sub-phase acceptance criteria coverage

Mirror M6 §6 — but with 7 sub-phases instead of 2. Each sub-phase gets its own acceptance bullet table (M7.1, M7.2, M7.3, M7.4, M7.5.A, M7.5.B.1, M7.5.B.2). Each bullet → file:line + test name. The 5 housekeeping passes are NOT sub-phases but each gets a brief paragraph after the M7.5.B.2 table:

```markdown
### Housekeeping passes (post-M-cycle-commit closures)

5 housekeeping passes ran during M7's implementation cycle. Per the
project's operating principle ("data exists or it doesn't"), none of
them resolved an open M-cycle acceptance bullet — each fixed a separate
issue surfaced during F5 manual verification or audit work. Each pass
followed three-commit discipline (`fix/feat → test → docs`).

1. **TICK display wiring** (`1b16e9e`, `21921887`, `3d800d2`) — ...
2. **TickScheduler.ShouldRun race fix** (`e0b0ecf`, `52d6d3f`, `700cbc0`) — ...
3. **Real pawn data** (`9141bd6`, `659a64a`, `74d2eed`) — ...
4. **NeedsSystem decay direction** (`ee12108`, `7ea038c`, `9d7b7f6`) — ...
5. **ModMenuPanel position + assets gitignore** (`5f0b4f5`, `805b882`, `0b11d1f`) — ...
6. **Menu actually pauses simulation** (`9f87536`, `d8a448f`, `110ad61`) — ...

(Six total — re-count if anything was missed.)
```

#### §7 Carried debts forward

Mirror M6 §7 format. Categories:

- **Phase 2 carried** — WeakReference unload tests: NOW CLOSED by M7.3 (verify; the M6 closure had this at "tracked forward to M7 — hard requirement"). If closed, mark closed; if any sub-tests still pending, list them.
- **Phase 3 carried** — `SocialSystem`/`SkillSystem` `[BridgeImplementation(Phase = 3)]` stubs still tracked forward to M10.C. Unchanged through M7.
- **M3.4 — CI Roslyn analyzer (D-2 hybrid completion)** — still `⏸ Deferred`. Unchanged through M7.
- **§7.5 fifth scenario (M6→M7 hand-off)** — verify (A/B/C from §2 above). Whichever outcome holds, document explicitly here.

**New in-batch carried items:**

- **§9.2 v1.6 ratification candidate.** Full diagnosis, three resolution options. Target: future v1.6 cycle. Implementation today: orchestration-layer wiring at `GameBootstrap.CreateLoop` (commit `9f87536`). Recommendation: defer resolution until M8+ load patterns clarify which option scales.
- **`.sln` build verification gap.** Top-level `dotnet build DualFrontier.sln` does NOT compile `DualFrontier.Presentation` (Godot project, not in .sln). M7.5.B.2 closure report flagged this. Target: small housekeeping post-M7-closure or M8 prep.
- **UI redesign with Kenney UI pack + Cinzel font.** Phase 5 / M10 polish. Decisions pending (which Kenney pack as base, theme system breadth, font application rules). Target: separate larger brief.
- **Phase 5 backlog (extended)** — list all categorized entries from `ROADMAP.md` Backlog section per the housekeeping commits. Each entry has its target M-phase.

#### §8 Ready-for-M8 readiness

Mirror M6 §8 format. M8 needs:

| M8 dependency | M0-M7 surface | Status |
|---|---|---|
| Vanilla mod skeleton scaffolding (5 mods per spec) | M0 ModLoader, M2 IModApi, M4 SharedModLoadContext, M7.4 build pipeline | <verify> |
| Vanilla mod manifests with v2 schema | M1 + M5.2 v2 path | <verify> |
| Mod menu integration for vanilla mod toggle | M7.5.A controller + M7.5.B.1/2 production wiring + post-fix menu-pauses-simulation | <verify> |
| Build pipeline support for vanilla mods | M7.4 (`<IsVanillaMod>` MSBuild opt-in, `mods/Directory.Build.targets`, ManifestRewriter tool) | <verify> |
| Hot-reload disabled per-mod for vanilla | §9.6 + M7.4 + M7.5.A AD #5 (first-load is not a reload) | <verify> |
| Documentation surface for mod authors | MODDING.md, MOD_PIPELINE.md | <verify these exist + currency> |
| ContractValidator readiness | Eight-phase pipeline (no new phase needed for M8 unless vanilla mods exercise an unforeseen contract) | <verify> |

**Verdict:** Each row PASSED if the surface is in place; CARRIED if pending. List all carried items in §7.

#### §9 Surgical fixes applied

Reserved. Expected: "None." If any surgical fix is applied during this closure session, document each: file, line, change, justification. The closure session itself produces 1 commit (the M7_CLOSURE_REVIEW.md addition) plus optionally per-surgical-fix commits (each fix in its own commit per M3-M6 precedent).

#### §10 Items requiring follow-up

Mirror M6 §10 structure. Five sub-sections expected:

1. **Empirical observation — contradiction discovery rate, fifth datapoint.**
   - Quote the M6 forward claim verbatim.
   - Report M7 datapoint: 1 (§9.2 menu-pause gap surfaced via F5 manual verification post-M7.5.B.2 closure).
   - Sequence: M3=1, M4=1, M5=0, M6=0, M7=1.
   - Hypothesis status: **preserved** (forward claim "M7 ≤ 1" holds; cumulative target "≤ 4 across M3-M10" still at 3).
   - Forward claim for M8-M10: cumulative count must remain ≤ 4. With 3 used, M8-M10 must contribute ≤ 1 collectively.

2. **§9.2 v1.6 ratification candidate.**
   - Full diagnosis of two-pause-flag situation.
   - Three resolution options.
   - Recommendation: defer to v1.6 cycle informed by M8+ load patterns.

3. **`.sln` build verification gap.**
   - Diagnosis: `DualFrontier.Presentation` (Godot project) not in `.sln`.
   - Risk: top-level `dotnet build` skips Presentation; CI/automated checks miss Presentation regressions.
   - Resolution options: add to `.sln` (verify Godot SDK headless build cooperates); or document required dual-build invocation in CODING_STANDARDS / METHODOLOGY; or wrapper script.
   - Target: small housekeeping post-closure.

4. **UI redesign brief decisions pending.**
   - Kenney pack choice.
   - Theme system breadth.
   - Font application rules.
   - Pawn sprite replacement (Phase 5 dependency).
   - TileMapRenderer asset replacement (Phase 5 dependency).
   - Asset extraction & import workflow.
   - Target: separate larger brief informed by user vision/mockup.

5. **Operating principle extension.**
   - The «data exists or it doesn't» principle was applied 5 times during M7 housekeeping (real pawn data, needs decay direction, ModMenuPanel position fix, menu pauses simulation, gitignore for derived state).
   - Each application surfaced a placeholder lie that needed structural fix, not a removal-of-claim.
   - Forward claim: future cycles will continue surfacing applications. The principle has demonstrated value and should be promoted from "user-stated convention" to documented project value in `METHODOLOGY.md` or `docs/README.md`.
   - Target: small documentation commit, separate from this closure.

#### §11 Verification end-state

Mirror M6 closing format with M7-specific values:

- Build: 0/0.
- Tests: 437/437 passing across 5 test projects.
- Three-commit invariant: holds at every commit `<commit-A>..<commit-B>`. Per-commit progression: 338 (M6 baseline) → ... → 437 (HEAD).
- Spec ↔ code ↔ test triple consistency: <count of acceptance bullets> in-batch. CENTRAL <demonstration if applicable>.
- Cross-document consistency: `MOD_OS_ARCHITECTURE` v1.5 LOCKED ↔ `ROADMAP` `2026-05-03` `437/437` M7 ✅ ↔ `docs/README` v1.5 LOCKED.
- Stale-reference sweep: zero hits in active context.
- Methodology compliance: scope prefixes 39/39 (or actual count), all bodies substantive, D-1..D-7 byte-identical, **§9.2 wording-vs-implementation gap surfaced via F5 verification (registered as v1.6 candidate, not silent)**.
- Sub-phase acceptance: M7.1 through M7.5.B.2 all mapped; housekeeping closures all present.
- Carried debts forward: Phase 2 closed (WeakReference via M7.3); Phase 3 → M10.C; M3.4 → first external mod author; §7.5 fifth scenario <A/B/C outcome>; new items (§9.2 candidate, .sln gap, UI redesign).
- Ready-for-M8: <verdict per §8>.
- Surgical fixes applied this pass: 0 (or count if any).
- Items needing follow-up: <count> blocking, 5 observations registered.

M7 closes <cleanly | with one v1.6 candidate registered>. M8 (Vanilla skeletons) is unblocked.

#### §12 See also

```markdown
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) v1.5 LOCKED.
- [ROADMAP](../ROADMAP.md) — M7 closure status, 7 sub-phase entries,
  6 housekeeping closures.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — §2.4, §7.3, §11.4.
- [M3_CLOSURE_REVIEW](../audit/M3_CLOSURE_REVIEW.md) — closure-report
  format origin (eight-check structure).
- [M4_CLOSURE_REVIEW](../audit/M4_CLOSURE_REVIEW.md) — multi-document
  consistency precedent.
- [M5_CLOSURE_REVIEW](../audit/M5_CLOSURE_REVIEW.md) — empirical
  contradiction-rate hypothesis registration.
- [M6_CLOSURE_REVIEW](../audit/M6_CLOSURE_REVIEW.md) — most recent precedent;
  this document mirrors its eight-check structure and extends the
  empirical contradiction-discovery rate observation registered in
  M5/M6 closure reviews with its first nonzero post-M4 datapoint.
- [UI_REVIEW_PRE_M75B2](./UI_REVIEW_PRE_M75B2.md) — pre-M7.5.B.2 audit
  document referenced by M7.5.B.2 prompt.
```

### Optional surgical fixes (separate commits if any)

Per M3-M6 precedent: if any typo / broken cross-reference / clearly-wrong fact surfaces in M7-introduced docs (`docs/prompts/M7*.md`, `docs/prompts/HOUSEKEEPING_*.md`, `docs/audit/UI_REVIEW_PRE_M75B2.md`, `docs/ROADMAP.md` M7-related entries), apply each as its own commit with prefix `docs(audit-fix)` or similar. Document in §9 of the closure review.

If no surgical fixes are needed → §9 says "None" and the closure session is single-commit.

### Single closure commit

```
docs(review): M7 closure verification report
```

Body covers: 8 checks PASSED, hypothesis fifth datapoint = 1, §9.2 v1.6 candidate registered, .sln gap registered, M7 closes cleanly, M8 unblocked.

## Acceptance criteria

1. `docs/audit/M7_CLOSURE_REVIEW.md` exists with 8-check structure mirroring M6.
2. All 8 checks PASSED (or explicit failures documented with surgical-fix follow-up commits).
3. Three-commit invariant table in §1 lists every M7 commit (M-cycle + housekeeping) with explicit visual separation.
4. §2 verifies §7.5 fifth scenario M6→M7 hand-off with one of (A/B/C) outcome.
5. §3 verifies v1.4 → v1.5 spec transition matches documented changelog.
6. §4 stale-reference sweep returns zero active-navigation hits.
7. §5.2 verifies D-1..D-7 byte-identical despite v1.4→v1.5 (strongest signal v1.5 is non-strategic ratification).
8. §5.3 lists 6+ deliberate-interpretation registrations.
9. §6 covers all 7 sub-phases + 6 housekeeping passes.
10. §7 carried debts: Phase 2 marked CLOSED via M7.3; Phase 3 unchanged; §7.5 fifth scenario outcome documented; 3+ new carried items.
11. §8 ready-for-M8 verdict per dependency.
12. §9 surgical fixes: 0 expected, document any if applied.
13. §10 contains 5 observation sub-sections (empirical fifth datapoint, §9.2 v1.6 candidate, .sln gap, UI redesign, operating principle extension).
14. §11 verification end-state mirrors M6 format with M7-specific values.
15. ROADMAP.md M7 row updated to ✅ Closed.
16. M-phase boundary preserved: `git diff <baseline>..HEAD --stat -- src/DualFrontier.Core src/DualFrontier.Contracts` empty.
17. `dotnet test` count: **437/437 unchanged** (closure is verification, no test changes).
18. `dotnet sln list` count unchanged.

## Финал

Single closure commit (or +N surgical-fix commits if any):

**1.** `docs(review): M7 closure verification report`

- Add `docs/audit/M7_CLOSURE_REVIEW.md` per §Implementation above.
- Update `docs/ROADMAP.md`:
    - Header status line: `*Updated: 2026-05-03 (M7 closed; M8 (Vanilla skeletons) is the next phase; §9.2 v1.6 ratification candidate registered).*`
    - M7 row: `🔨 In progress` → `✅ Closed`. Tests column: confirm 437/437.
    - M7 sub-phase block: each sub-phase already marked ✅; verify and clean up. The block becomes historical record.
    - Add M7-closure note paragraph at the end of the M7 section parallel to M5/M6 closure notes: "M7 closes; §7.5 fifth scenario <outcome>; §9.2 v1.6 candidate registered."
    - Engine snapshot final line: 437/437.
- Verify no other ROADMAP edits creep in. Closure session is verification-only.
- Verify build + test still pass (sanity check; no code change expected).

**N.** (optional) Per surgical fix found:

- Each as its own commit with `docs(audit-fix): <subject>`.
- Document each in §9 of the closure review (the closure-review commit must be re-edited to add the §9 entries; in practice apply surgical fixes BEFORE writing §9).
- If only surgical fixes are typo-level, keep them minimal — closure session is verification, not cleanup. Defer non-blocking cleanup to a separate housekeeping pass.

**Special verification preamble:**

After commit 1: `dotnet test` — 437/437 unchanged. ROADMAP renders cleanly with M7 marked ✅ Closed. `docs/audit/M7_CLOSURE_REVIEW.md` exists and renders cleanly.

If surgical fixes were applied: each commit independently passes `dotnet build` + `dotnet test`. Closure review's §9 lists each fix.

If during execution an unforeseen architectural finding surfaces (e.g., a sub-phase wasn't actually closed despite ROADMAP saying so) — STOP, document the finding under §10 of the in-progress closure review, ask the user how to proceed (escalate to a remediation brief vs. defer to M8 prep vs. carry forward). Closure does NOT silently absorb remediation work.

**Hypothesis-falsification clause:**

This is the closure session that **explicitly handles** the hypothesis falsification surfaced post-M7.5.B.2. The §10 fifth-datapoint section records:

- M3=1 (§3.6 hybrid enforcement)
- M4=1 (§2.2 ignored-for-shared)
- M5=0 (§8 versioning, no contradictions)
- M6=0 (§7 bridge replacement, no contradictions)
- **M7=1 (§9.2 menu-pause gap, surfaced via F5 verification post-closure)**

Forward claim status:
- M5 forward claim ("M7 alone ≥ 2 OR cumulative > 4 falsifies") → **preserved**: M7 produced 1 (not ≥ 2); cumulative 3 (not > 4).
- M6 forward claim ("M7 ≤ 1 latent contradiction") → **exactly met**.
- New M7 forward claim: cumulative count across M3-M10 must stay ≤ 4. With 3 used, M8-M10 collectively must contribute ≤ 1.

Plausible non-spec frictions during execution:

(a) **§7.5 fifth scenario actually closed by tests.** If a joint end-to-end test exists (option A from §2), the closure is cleanest. Verify via grep in pre-flight.

(b) **§7.5 fifth scenario neither closed nor structurally argued.** If the executor finds the scenario is truly a deferred carry — register honestly under §7 and §10. Do NOT pretend closure.

(c) **v1.4 → v1.5 spec changelog missing or incomplete.** If the diff shows changes not documented in v1.5 changelog — surface as §10 finding requiring follow-up commit (could be a surgical fix to the changelog if minor, or a registered v1.6 ratification candidate if substantive).

(d) **An M7 sub-phase ROADMAP entry shows ⏭ instead of ✅ in current state.** If found, surface as §10 finding. Either it's an unfinished sub-phase (escalate) or a stale ROADMAP marker (surgical fix in §9).

(e) **Stale-reference sweep returns hits in active-navigation context.** Each is a surgical-fix candidate. Apply per §9 if minor; register as finding if substantive.

(f) **§5.3 deliberate-interpretation count > 6.** If more than 6 found during execution, list all. Methodology compliance: each interpretation must be registered in ROADMAP per "no improvisation" rule.

## Report-back format

- 1 closure commit SHA + N surgical-fix SHAs (if any).
- §0 executive summary verdict per check (PASSED/FAILED count).
- §1 three-commit invariant: confirmed at every commit in M7 batch (commit count, range).
- §2 outcome on §7.5 fifth scenario: A (joint test) / B (structural) / C (carried).
- §3 v1.4 → v1.5 diff: matches changelog YES/NO. If NO, surgical fix or §10 finding.
- §4 stale-reference sweep: zero hits / count of hits + dispositions.
- §5.2 D-1..D-7 byte-identity: confirmed YES/NO.
- §5.3 deliberate-interpretation count: actual count (expect 6+).
- §6 sub-phases + housekeeping: all 7 + 6 mapped: YES/NO.
- §7 Phase 2 closure: confirmed via M7.3 YES/NO. New carried items count.
- §8 ready-for-M8 verdict: PASSED / verbal explanation per dependency.
- §9 surgical fixes count.
- §10 observations: 5 sub-sections present.
- §11 verification end-state final values.
- ROADMAP M7 row: ✅ Closed confirmed.
- Final dotnet test count: 437/437 unchanged.
- M-phase boundary: empty diff against Core/Contracts.
- Hypothesis fifth datapoint: 1 (recorded), forward claim status (preserved).
- §9.2 v1.6 ratification candidate: registered in §10 with three resolution options.
- Any unexpected findings.
