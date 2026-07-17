---
register_id: DOC-D-DRAFTS_RATIFICATION_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-17'
last_modified: '2026-07-17'
content_language: en
next_review_due: 'null'
last_review_date: '2026-07-17'
last_review_event: 'DRAFTS_RATIFICATION executed 2026-07-17 (EVT-2026-07-17-DRAFTS_RATIFICATION): H3 exclusion-rider 48983c4; C1 c3e77a8 (enroll); HALT-1 en-bloc ratification; Phase B C2 b4bcfa4 / C3 d6f1e9a / C4 b38d95a / C5 fde5072; Phase C C6 bd199da (six flips + PSC hold + qualifier sweep) + C7 closure (this commit) + C8 F-39 rider. Deviations recorded in the closure report.'
title: DRAFTS_RATIFICATION execution brief (item [6] widened; first cascade under register schema 2.0)
---

# DRAFTS_RATIFICATION -- Execution Brief

This cascade executes EVT-R4 ratification-checklist item [6], widened per D1 (ORC-3): the seven
2026-07-15 cross-cutting AUTHORED drafts are re-verified claim-by-claim against code at current
HEAD, retargeted onto the post-rework corpus, and SIX are ratified AUTHORED -> LOCKED v1.0.0 in
the recommended order -- EXECUTION_AUTHORITY_MATRIX; ENGINE_LIFECYCLE_AND_TRANSACTIONS +
RESOURCE_OWNERSHIP_AND_LIFETIME + CONCURRENCY_AND_MEMORY_MODEL; TIME_AND_CONSISTENCY_MODEL +
IDENTITY_AND_ABI_CONTRACT -- while PERSISTENCE_SNAPSHOT_CONTRACT is retargeted but HELD AUTHORED
until the save milestone (the standing item-[6] recommendation). Two mechanical staleness classes
land in the same cascade: the candidate-banner class across the 14 LOCKED successors, and the
"(AUTHORED draft)" qualifier class that the flips themselves make stale. Done means: the cutover
gates, shutdown law, identity law, and transaction vocabulary are LOCKED law; the corpus carries
zero stale candidate banners; armed validate exit 0.

**This is the FIRST cascade under register schema 2.0.** Every governance mutation is a
frontmatter edit in the document itself followed by
`dotnet run --project tools/DualFrontier.Governance -- sync`; the gate is `validate --armed`
exit 0; the closure EVT is the FIRST append to `docs/governance/AUDIT_TRAIL.yaml`;
`REGISTER.yaml` and `CURRENT_AUTHORITY_SURFACE.yaml` are DERIVED -- never hand-edited (FRAMEWORK
section 14.1). The PENDING-* vocabulary is outlawed (14.3).

**Executor**: Claude Code (Fable 5), LOCAL on the operator's machine.
**Repository**: `D:\Colony_Simulator\Colony_Simulator`, branch off `main` (= `2309f9b` at
authoring).
**Zero code changes**: this cascade edits documents and governance only. The ratified drafts
become the WORK ORDERS for the engineering cascades that follow (shutdown implementation, the
identity versions surface, the GATE-S family, the F-35..F-40 wiring/flake queue) -- seeding that
forward queue in ROADMAP is a closure deliverable, implementing it is out of scope (H6).

**Brief-integration notice**: standing law is cited by anchor, never restated. Brief vs standing
doc conflict = the brief is wrong -- halt. Brief vs live code conflict = the code is true --
record and proceed.

## 1. Mission [CORE]

| # | Deliverable | Action |
|---|---|---|
| D1 | `docs/reports/DRAFTS_RATIFICATION_REVIEW_REPORT.md` | Phase A per-draft re-verification at current HEAD + banner-class census |
| D2 | Retarget corrections to the 7 drafts | Phase B, HALT-1-ratified list only |
| D3 | Six drafts LOCKED v1.0.0; PSC held AUTHORED with refreshed verification note | Phase C flips (frontmatter + sync) |
| D4 | Two mechanical staleness classes cleared | banner class (14 successors) + "(AUTHORED draft)" qualifier class |
| D5 | Closure EVT as the first `AUDIT_TRAIL.yaml` append + ROADMAP forward-queue seeding | Phase C closure |

## 2. Established facts [CORE]

Re-verify (R) at Phase 0; mismatch -> HALT H1.

**Base state.** (R) `main` = `2309f9b`, clean tree. (R) Register derived, schema 2.0;
`CURRENT_AUTHORITY_SURFACE.yaml` 128 entries (104 Live + 24 LOCKED). (R)
`dotnet run --project tools/DualFrontier.Governance -- validate --armed` exit 0. Baseline test
state 1166 pass / 0 fail / 5 skip; known cross-suite flake F-40
(`UnloadModNativeState_VacuousUnload_Succeeds`, isolated-pass) -- a single-occurrence recurrence
of exactly that test in a full run is a RECORDED KNOWN FLAKE, not an H2 regression; anything else
is H2.

**The seven drafts** (all `docs/architecture/`, all Tier 1 AUTHORED, verified at `35364c2`-era
HEAD `6f39903`, which is exactly the problem): EXECUTION_AUTHORITY_MATRIX (EAM),
CONCURRENCY_AND_MEMORY_MODEL (CMM), RESOURCE_OWNERSHIP_AND_LIFETIME (ROL),
ENGINE_LIFECYCLE_AND_TRANSACTIONS (ELT), TIME_AND_CONSISTENCY_MODEL (TCM),
IDENTITY_AND_ABI_CONTRACT (IAC), PERSISTENCE_SNAPSHOT_CONTRACT (PSC).

**Ratification order (EVT-R4 item [6], operator-confirmed 2026-07-17)**: EAM first; then
ELT + ROL + **CMM** (CMM is absent from the EVT order list -- architect ruling: it rides with the
lifecycle/shutdown family it co-owns per THREADING section 10); then TCM + IAC; **PSC held
AUTHORED** until the save milestone. Preamble wording updates at ratification are part of the
item ("LOCKED docs prevail" phrasing predates the corpus rework).

**Seeded findings (D1 of CORPUS_CLOSURE_INVERSION_B, R4-7..R4-13 -- carry verbatim into the
Phase A report):** TCM sections 5.1/1/7.5 cites resolve only against `historical/` (R4-7); CMM
sections 4/9 attribute the cycle/snapshot rule to FEEDBACK_LOOPS while THREADING_V2 section 7 is
the normative home (R4-8); CMM section 8 claims "17 rule stubs, all non-detecting" -- stale
(R4-9); TCM "only production RNG" vs 3 more seeded `Random` sites, PSC carries the accurate
version (R4-10); ELT section 4 cites `OWNERSHIP_TRANSITION.md:43` unqualified -- text now at
`historical/` (R4-11); CMM version bookkeeping three ways (R4-12); ALL SEVEN pin "today" to
`6f39903` and line-cite files the rework replaced in place -- sharpest: EAM:183 cites
"ANALYZER_RULES.md line 271", the successor is 206 lines (R4-13).

**Mechanical staleness class 1 -- candidate banners (architect finding, post-closure).** All 14
LOCKED successors still carry: the "Document class: authored-rework (current-truth candidate)...
Becomes the LOCKED authority upon Crystalka ratification... predecessor... prevails" banner; the
unchecked "Ratification checklist:" line; `| Role | normative-current-candidate |`; and (at least
ARCHITECTURE + CONTRACTS) an Amendment-protocol sentence "Tier 1, AUTHORED pending ratification"
-- all contradicting their own LOCKED frontmatter (the SoT). Census expression:
`rg -l "normative-current-candidate" docs/architecture/` (expect 14 before, 0 after).

**Mechanical staleness class 2 -- "(AUTHORED draft)" qualifiers.** LOCKED successors qualify
every draft citation with "(AUTHORED draft)" / "advisory until ratified" (e.g. SCHEDULER section
3.3; THREADING section 10; ECS section 5). The Phase C flips make those qualifiers stale for the
six ratified drafts; references to PSC KEEP the draft qualifier. Census expression:
`rg -n "AUTHORED draft" docs/architecture/*.md` -- record before/after; after Phase C every
remaining hit must reference PSC only.

**Regime facts.** FRAMEWORK section 14.3 required frontmatter fields; sentinel forms (14.4)
CLOSED set -- the PSC hold uses the sanctioned 'post-<event> closure' form (lean:
'post-persistence-milestone closure'); G-CATLIFE forbids tier-3 + LOCKED, so THIS BRIEF's
register lifecycle is Draft -> EXECUTED (chat ratification is not a register state; do not
attempt to persist LOCKED on a D/3 entry -- it would fail the armed gate by law, H5-adjacent).

## 3. Phase 0 -- preconditions [CORE]

1. Verify every (R) fact. Mismatch -> H1.
2. Baseline gates: full build + full test run per `docs/methodology/DEVELOPMENT_HYGIENE.md`;
   record as regression anchor (F-40 flake shape stated above). Governance suite green.
3. `validate --armed` exit 0 -> else H3.
4. Frontmatter-shape read (Lesson #N14, 2.0 edition): read FRAMEWORK 14.3 + the live frontmatter
   of one LOCKED successor (KERNEL_ARCHITECTURE.md) and one AUTHORED draft -- these verbatim
   shapes are the only templates for Phase C frontmatter edits. Read the AUDIT_TRAIL.yaml head
   comment + one existing EVT entry as the append template.
5. Mandatory reads: this brief; D1 of the previous cascade
   (`docs/reports/CORPUS_REWORK_REVIEW_REPORT.md`, R4 + ORC sections); the seven drafts in full;
   FRAMEWORK section 14; METHODOLOGY closure protocol.

The executor NEVER pushes; the PR merge is the operator's. No history rewrite. `historical/` is
read-only (symmetric preservation).

## 4. Topology [CORE]

Orchestrator + Wave R only in Phase A: four parallel read-only agents -- A1: EAM; A2:
ELT + ROL + CMM; A3: TCM + IAC + PSC; A4: the two mechanical-class censuses across the 14
successors (exact per-file occurrence lists with proposed replacement text). Only the
orchestrator commits; ROADMAP.md and AUDIT_TRAIL.yaml are orchestrator-only; Phases B/C serial.

## 5. Wave R -- review agents [KIND: audit within a combined cascade]

Per draft, fixed schema: (1) exhaustive anchor verification at current HEAD -- every file:line /
file:range claim verdict-ed EXACT / DRIFTED / WRONG; (2) truth-law audit (enforcement verbs vs
on-disk enforcers; FENCED discipline); (3) retarget list -- every citation of a replaced document
re-pointed onto the successor corpus with exact proposed text (the R4-7..R4-13 seeds resolved
concretely); (4) LAW-READINESS verdict: is the draft's normative content ratifiable as-is
(READY / READY-WITH-FIX / NOT-READY with reason) -- for EAM specifically, re-verify each
GATE-S/GATE-B condition's open/closed status against code at HEAD so the gates ratify TRUE;
(5) preamble replacement text per item [6] (the post-rework wording), drafted for HALT-1.

A4 returns the two censuses of section 2 with per-site replacement text (banner -> one-line
ratified-successor note carrying the EVT id; checklist line -> removed; Role ->
`normative (ratified successor)`; amendment-protocol sentence -> LOCKED wording; "(AUTHORED
draft)" -> "(LOCKED)" or bare title for the six, untouched for PSC).

Report schema: R0 base state; R1-R3 per-cluster tables; R4 mechanical censuses; R5 consolidated
fix register with exact texts; R6 self-attestation (zero writes except the report; zero git
mutations; every census expression verbatim).

## 6. Checkpoints [CORE]

**HALT-1 (operator).** Report presented in chat. The operator: ratifies per-draft (en bloc
sanctioned when verdicts are clean); confirms six-now / PSC-held; ratifies the fix list
(FIX-NOW / DEFER / REJECT per finding) and both mechanical-class replacement forms; decides the
optional F-39 rider (section 8). A NOT-READY draft is withheld from Phase C (partial ratification
legal, recorded); its law-gap goes to the F-ledger.

**C-B.** Orchestrator self-audit: every fix matches ratified text; nothing outside the list;
build/tests match baseline; sync run after every frontmatter-touching commit; validate --armed
exit 0.

**C-C (closure).** Class-1 census = 0; class-2 census = PSC-only; the six drafts LOCKED 1.0.0 on
the regenerated CURRENT_AUTHORITY_SURFACE (spot-check: all six present; PSC absent); AUDIT_TRAIL
append parses and merges into the derived register; ROADMAP forward-queue seeded.

## 7. Execution specifications [CORE]

**Phase B (fixes).** One commit per cluster (A1/A2/A3 scopes) + one commit for the banner class +
one for the qualifier class (the qualifier commit may land in Phase C after the flips if the
executor judges ordering cleaner -- record the choice). Every edited document: body fix + its own
frontmatter maintenance (version PATCH bump, `last_modified`, `last_review_event` naming this
cascade and the ratified finding ids, real `last_modified_commit` where retained) + `sync` in the
same commit; `validate --armed` exit 0 folded.

**Phase C (flips).** For each of the six, in the ratified order: frontmatter `lifecycle: LOCKED`,
`version: 1.0.0`, `next_review_due: 2027-Q3` (successor convention), `last_review_event` carrying
the ratification + EVT id; preamble replaced with the HALT-1-ratified post-rework wording
(authored-proposal disclaimer retires; the document states its law-in-force status). PSC:
verification note refreshed (re-verified at HEAD, held AUTHORED), `next_review_due:
'post-persistence-milestone closure'`. Closure: the EVT appended to AUDIT_TRAIL.yaml
(`EVT-2026-07-<dd>-DRAFTS_RATIFICATION`, real hashes, per-document lifecycle_transitions);
`sync`; ROADMAP write-back -- item-[6] queue entry closed, and the post-ratification ENGINEERING
QUEUE seeded as forward state (the operator's mandate: the ratified drafts are the work orders):
shutdown-law implementation family (ROL section 4 + CMM sections 6-7 -> the N-19 gap, F-40's
mod_unload-globals guard); identity versions surface (IAC section 2 -> the ECS section 5 defect);
GATE-S1..S4 / GATE-B preparatory work per EAM section 3; the F-35..F-38 wiring/comment queue.
Findings from HALT-1 marked DEFER get F-ledger entries -- nothing chat-only.

## 8. Optional rider -- F-39 [KIND: governance]

If ratified at HALT-1: the two out-of-corpus dangling "METHODOLOGY section 7.1 determinism"
citations (`IDEAS_RESERVOIR.md`, `MAXIMUM_ENGINEERING_REFACTOR.md`) receive the same re-anchor
treatment the FHE instances got (contract-local commitment + TCM-draft vocabulary; the MAX_ENG
touch is a PATCH-class LOCKED edit). One commit; F-39 closes. If not ratified, F-39 stays open --
zero touches.

## 9. S-LOCK invariants [CORE]

None added. The two census expressions of section 2 become closure gates for this cascade only
(class-1 -> 0; class-2 -> PSC-only), not standing pins.

## 10. Census discipline [CORE]

No reserved-stub or pragma surface touched; DFK-WAIVER HARD pin = 2 unchanged at closure
(`rg -n "DFK-WAIVER" src/` = 2). Both mechanical-class censuses recorded before/after with
verbatim expressions.

## 11. Commit plan [CORE]

| # | Subject | Content |
|---|---|---|
| C1 | `governance(review): enroll DRAFTS_RATIFICATION brief + review report` | brief + D1' frontmatter-enrolled + sync |
| C2 | `docs(eam): HALT-1-ratified retargets + gate-status re-verification` | A1 fixes |
| C3 | `docs(lifecycle-law): ELT/ROL/CMM retargets (R4-8/9/11/12 family)` | A2 fixes |
| C4 | `docs(time-identity): TCM/IAC/PSC retargets (R4-7/10 family)` | A3 fixes |
| C5 | `docs(architecture): retire candidate banners across the 14 ratified successors` | class 1 |
| C6 | `governance(ratification): six drafts AUTHORED -> LOCKED v1.0.0; PSC held; qualifier sweep` | flips + class 2 + preambles |
| C7 | `governance(closure): DRAFTS_RATIFICATION EVT append + ROADMAP forward-queue` | AUDIT_TRAIL + ROADMAP + validate |
| C8 | *(rider, if ratified)* `docs(governance): F-39 dangling-citation re-anchors` | F-39 |

Intended form; deviations recorded, never compressed.

## 12. REGISTER cascade [CORE]

Schema-2.0 discipline throughout: enrollment and every mutation = frontmatter in the document +
`sync`; derived artifacts never hand-edited; `validate --armed` exit 0 per
governance-touching commit; this brief enrolls D/3/Draft at C1 and flips to EXECUTED in C7's
frontmatter edit. The closure EVT is the first AUDIT_TRAIL.yaml append -- treat the existing
entry shape as the verbatim template; the file is append-only law (FRAMEWORK 14.6).

## 13. Halt conditions [CORE]

- **H1** precondition mismatch. **H2** build/test regression beyond the recorded F-40 flake
  shape, or governance-suite failure. **H3** validate --armed nonzero. **H4** Wave-R material
  contradiction with section 2. **H5** a frontmatter/sentinel value needed outside the 14.3/14.4
  closed vocabulary -- escalate, never invent. **H6** any need to change `src/`/`native/` code,
  or law-semantics beyond the ratified retarget scope. **HALT-1** always stops.
- Standing rails: no pushes; no history rewrite; `historical/` untouched; derived registers never
  hand-edited; AUDIT_TRAIL entries never modified (append-only).

## 14. Closure protocol and report [CORE]

METHODOLOGY closure protocol. Report (chat): commits table; versions table (7 drafts + touched
successors + register state); both census before/after; gates baseline vs closure; F-ledger
final states (F-39 if ridden; new DEFER entries); skeleton revisions; self-attestation (no
pushes; sync after every frontmatter commit; single AUDIT_TRAIL append; historical/ untouched;
zero code changes); operator manual checklist (push + merge; the engineering-queue deliberation
now unblocked -- next architect act: charter the first engineering cascade off the ratified
work orders; G-RATIO matrix deliberation still pending separately).

## 15. Out of scope [CORE]

Implementing ANY draft law in code (the engineering cascades that follow); PSC ratification;
G-RATIO matrix decision; F-41 tool PATCH; F-30/F-31/F-32; TESTING_STRATEGY 2.6; the NIH tree;
pushes/merges.

---

**End of DRAFTS_RATIFICATION_BRIEF.md v1.0**
