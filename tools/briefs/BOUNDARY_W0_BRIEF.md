---
register_id: DOC-D-BOUNDARY_W0_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
title: BOUNDARY_W0 execution brief -- boundary-axis enrollment + ratification flips + vanilla-mods solution enrollment + the engine-to-game reference ratchet (Wave 0 of VANILLA_SEPARATION_MIGRATION_PLAN)
---

# BOUNDARY_W0 -- Execution Brief

Wave 0 of the vanilla-separation program: enroll the boundary-axis documents, execute the
operator's ratification flips, make the six vanilla mods real build participants, and ship
the mechanical ratchet that freezes the engine-to-game reference census. Zero gameplay code
is touched. Anti-pattern rule: a conflict between this brief and any standing doc means THE
BRIEF IS WRONG -- halt and quote both.

## 1. Established facts (from GAME_ENGINE_BOUNDARY_AUDIT_REPORT.md @ 4c58942; re-verify at Phase 0)

- F1. Three untracked files exist: `docs/reports/GAME_ENGINE_BOUNDARY_AUDIT_REPORT.md`
  (proposed frontmatter in its appendix, head is body-only);
  `docs/governance/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` and
  `docs/VANILLA_SEPARATION_MIGRATION_PLAN.md` (operator-placed; BOTH move to
  `docs/architecture/` before any commit -- category-A home, EAM/predecessor precedent).
- F2. Engine->game census at 4c58942: exactly 4 ProjectReference edges, all in
  `src/DualFrontier.Application/DualFrontier.Application.csproj` (Components, Events,
  Systems, AI); exactly 1 IVT `src/DualFrontier.Core/DualFrontier.Core.csproj` -> Systems.
  Game-assembly set: DualFrontier.Components / .Events / .Systems / .AI.
- F3. The 6 vanilla mods (`mods/DualFrontier.Mod.Vanilla.{Core,World,Pawn,Inventory,
  Combat,Magic}`) are absent from `DualFrontier.sln`; only Mod.Example is enrolled. Each
  references ONLY Contracts; no TargetFramework of their own (inherit net10.0 from root
  props). `mods/Directory.Build.targets` runs the ManifestRewriter dll via Exec after
  Release builds of IsVanillaMod projects -- the dll must exist when that target fires.
- F4. Test baseline: 1177 pass / 0 fail / 5 skip (EQ_A1 closure shape; F-40 single-flake
  carve-out: one recurrence of exactly `UnloadModNativeState_VacuousUnload_Succeeds` in a
  full run is KNOWN, not a regression). DFK-WAIVER pin 2 = 2 HARD.
- F5. `src/Directory.Build.props` carries a stale "net8.0" scope comment (audit A6) --
  comment-only fix pre-authorized here.
- F6. Register: 349 docs / AUDIT_TRAIL 58 EVTs at 4c58942. Schema 2.0 discipline: mutation
  = frontmatter edit + `dotnet run --project tools/DualFrontier.Governance -- sync` folded
  into the SAME commit; gate = `validate --armed` exit 0; AUDIT_TRAIL append-only;
  PENDING-* outlawed.

## 2. Ratified decisions this brief executes

- R1. Placement: both boundary docs live in `docs/architecture/` (operator-ratified in chat
  2026-07-18; executor performs the move -- files are untracked, a filesystem move).
- R2. Ratification flips (operator chat act 2026-07-18, pre-authorized for C3):
  GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY AUTHORED -> LOCKED 1.0.0;
  VANILLA_SEPARATION_MIGRATION_PLAN Draft -> Live 1.0.0. If the operator has NOT confirmed
  the flips at dispatch, HALT H-RAT before C3 and ask.
- R3. B-2 (no new gameplay nouns in src/) becomes ACTIVE at the C3 flip -- the closure EVT
  records the activation date.

## 3. Phase 0 (preconditions; stop on any failure)

1. `main` HEAD = `4c58942` or a descendant with clean tree; record the hash. If HEAD moved,
   re-verify F1-F3 against it before proceeding.
2. Baseline gates: full-solution build 0W/0E (Debug + Release) and test run matches F4
   shape; `dotnet run --project tools/DualFrontier.Governance -- validate --armed` exit 0.
   Record all as the regression anchor.
3. Read the three untracked files IN FULL (you are committing documents you did not
   author). Confirm the audit report's appendix frontmatter matches the FRAMEWORK 14.3
   shape used by a live DOC-E exemplar (open one recent report's frontmatter and compare
   field-for-field).
4. Confirm `docs/architecture/` contains no name collisions for the two incoming files.
5. Mandatory reads: FRAMEWORK.md section 14 (armed law), the two boundary docs themselves,
   EXECUTION_AUTHORITY_MATRIX.md section 3.0 (the ratchet is a transitional enforcer and
   carries a deletion trigger).

## 4. Commit plan (atomic; sync folded into every frontmatter-touching commit)

- **C1 -- enroll the boundary axis.** Move both docs to `docs/architecture/`; lift the
  audit report's appendix frontmatter to its head (active YAML) and delete the appendix
  section; `git add` all three; sync; validate --armed exit 0. Register grows 349 -> 352.
  Subject: `governance(boundary): enroll boundary law (AUTHORED), migration plan (Draft),
  audit report (DOC-E)`.
- **C2 -- solution enrollment.** Add the 6 vanilla mod projects to `DualFrontier.sln`
  (solution folder `mods` if the sln uses folders; match Mod.Example's placement
  convention). Build Debug AND Release full-solution. If the Release ManifestRewriter Exec
  fires before the rewriter dll exists, add the sln-level build dependency (mods depend on
  the rewriter project) -- proper wiring, in-scope; record it in the commit body. Subject:
  `build(mods): enroll the six vanilla mods into DualFrontier.sln (audit A1)`.
- **C3 -- ratification flips.** Frontmatter: law lifecycle AUTHORED -> LOCKED, version
  1.0.0, last_review_event records the operator ratification + this EVT id; plan Draft ->
  Live, 1.0.0, same form. Sync fold; validate. The law now appears on
  CURRENT_AUTHORITY_SURFACE (14.7 predicate: LOCKED tier-1); the plan appears as Live.
  Subject: `governance(boundary): ratify boundary law -> LOCKED 1.0.0, migration plan ->
  Live 1.0.0`.
- **C4 -- the ratchet.** New test class `BoundaryRatchetTests` in
  `tests/DualFrontier.Governance.Tests/` (governance = law enforcement; no new project).
  Behavior: parse every `src/*/*.csproj`; assert (a) the engine->game ProjectReference edge
  set equals EXACTLY the F2 baseline (the 4 Application edges), (b) the engine->game
  InternalsVisibleTo set equals EXACTLY {Core -> Systems}, (c) any engine csproj
  referencing a game assembly outside the baseline FAILS with a message citing
  GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md B-1. Baseline lists are explicit constants
  with a comment: shrinkage = update the constant + census-delta note in the commit body;
  growth = forbidden. FAILABILITY PROOF (red-once-then-green): demonstrate the test fails
  under a transient uncommitted edge addition, record the observed failure text in the
  commit body, revert the transient edge, commit green. The test carries its deletion
  trigger in its class doc: superseded by the B-6 analyzer rule when one ships (ROADMAP).
  Subject: `test(governance): engine-to-game reference ratchet (boundary law B-1/B-6
  interim enforcer)`.
- **C5 -- comment fix (pre-authorized micro).** `src/Directory.Build.props` stale "net8.0"
  scope comment corrected to net10.0 (audit A6). Comment-only; zero build-output change.
  Subject: `chore(build): fix stale net8.0 scope comment (audit A6)`.
- **C6 -- closure.** ROADMAP: new section "Vanilla-separation track (W0-W8)" citing the
  law + plan by stable id, wave table with W0 = DONE (hashes) and W1-W8 = OPEN with their
  BD gates; B-2 activation recorded; audit anomalies not otherwise consumed (A5, A9) noted
  as plan inputs, NOT new F-entries (the plan owns them). Plan doc: W0 row -> DONE with
  hashes (Live-doc update per its own protocol; PATCH bump). Closure EVT appended to
  AUDIT_TRAIL.yaml (EVT #59; prior entries byte-unchanged) listing lifecycle transitions
  (law LOCKED, plan Live, brief EXECUTED, audit report enrolled). Brief frontmatter ->
  EXECUTED. Sync fold; validate --armed exit 0. Closure report
  `docs/reports/BOUNDARY_W0_CLOSURE_REPORT.md` enrolled. Subject: `governance(closure):
  BOUNDARY_W0 EVT append + ROADMAP separation track + plan W0 DONE`.

## 5. Halt catalog

- H-RAT: C3 reached without operator flip confirmation -> stop, ask.
- H-SLN: any of the 6 mods fails to build after enrollment for a reason a sln build
  dependency cannot fix (source-level error) -> stop, report verbatim diagnostics; do NOT
  patch mod source in this cascade.
- H-RATCHET: the ratchet's Phase-0 census read disagrees with F2 (an edge appeared or
  vanished since 4c58942) -> stop, report the delta; the baseline constant must match
  measured reality, never the brief.
- H-GATE: validate --armed nonzero, build/test regression vs the Phase-0 anchor (F-40
  carve-out applies), or DFK-WAIVER pin moves -> stop.
- Standing rails: never push; no history rewrite; AUDIT_TRAIL prior entries byte-unchanged;
  no hand-edits to derived artifacts; docs/architecture/historical/ read-only.

## 6. Closure report schema

HEAD before/after; per-commit hashes + one-line delta; the ratchet failure text (red
proof) verbatim; final gates (build both configs, test totals vs F4, validate exit 0,
DFK-WAIVER 2=2); register/EVT counts before/after; surface delta (law appears LOCKED);
any Skeleton revisions; attestation (no push, sync run per-commit, derived headers intact).

## 7. Out of scope (fenced)

W1+ work (SDK abstraction, example mod, bus/capability changes); any gameplay code or mod
source edits; the B-6 analyzer rule itself (ratchet only); amendments to ARCHITECTURE.md /
MOD_OS_ARCHITECTURE.md current-truth text (per-wave, later); EQ-a Cascade B members;
Fixture.RegularMod_ReplacesCombat -> Core leak (a mods-axis item for W1's contract work,
recorded in the plan, untouched here).
