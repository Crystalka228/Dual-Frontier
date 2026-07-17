---
register_id: DOC-D-CORPUS_CLOSURE_INVERSION_B_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-07-16
last_modified: 2026-07-17
content_language: en
next_review_due: null
title: 'CORPUS_CLOSURE_INVERSION_B -- Execution Brief (closes the 2026-07-15 corpus rework: full-corpus Wave-R review D1, HALT-1-ratified corrections, 14 A-successors AUTHORED -> LOCKED 1.0.0, register 2.31 -> 2.32; then REGISTER_INVERSION Cascade B (F-34) schema-2.0 migration on post-merge main)'
last_modified_commit: 98906ea
review_cadence: on-cascade-execution
last_review_date: 2026-07-17
last_review_event: "CORPUS_CLOSURE_INVERSION_B executed 2026-07-16/17 by Claude Code (Fable 5, LOCAL Skarlet) per the operator-ratified brief (plan approval = ratification signal 2026-07-16). Phase 0: (R) facts exact; baseline build 0W/0E + native selftest ALL PASSED + full-sln tests 1166/0/5; validate exit 0 (25 baseline orphans); governance-tool smoke = expected pre-migration §14.2 shape (1249 exit-affecting errors on the unmigrated corpus; instrument proven, suite 64/64). Phase A: Wave R (R1-R4, 4 read-only agents, ~1014 anchors, zero NOT-READY) -> D1 at C1 0380dbf. HALT-1 (operator, in-chat 2026-07-17): SEED-1 variant (b) recompose-keeping-21 (closes F-9); Annex A A-1..A-4 APPROVED; MIGRATION_PLAN scalar ACKNOWLEDGED; fix list EN BLOC FIX-NOW; R2-20 FRAMEWORK-§7 banner ratified as established shorthand (zero edits); next_review_due '2027-Q3'. Phase B: C2 d8f1db3 (SEED-1, census-delta 0) + C3 8896d32 (SEED-2) + 17 per-doc correction commits f5c5e97..4016c62; C-B gate build 0W/0E, tests 1166/0/5 (one cross-suite native-state flake isolated-run-refuted 202/202), DFK-WAIVER pin 2. Phase C: EVT-54 rider 717641b; ratification cascade this commit (14 flips, 16 gap-window closures, REQ-K-L transfer, items [3][4][5] closed); Phase D (F-34 inversion) follows on post-merge main per brief §7."
reviewer: Crystalka
special_case_rationale: Category D Tier 3 -- the clean (non-forbidden) Category-D combo, per the design/decision-tier brief precedent (the K-series + F-10 + F-29 + CODEX DOC-D Tier-3 briefs). Standalone execution brief carrying this cascade's authority; enrolled at the design tier per the D-brief convention. Entry id follows the brief's own frontmatter register_id; the brief FILE frontmatter is hand-authored, not synced. The PENDING-COMMIT self-reference is the single sanctioned new placeholder of this cascade (brief §12), backfilled at the render commit.
---

---
register_id: DOC-D-CORPUS_CLOSURE_INVERSION_B_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED (Draft -> ratified via plan approval 2026-07-16 -> EXECUTED at the Phase C ratification cascade 2026-07-17; Phase D rides post-merge main per section 7)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-16'
content_language: en
authored_by: Claude Opus (deliberation session, CORPUS_CLOSURE_INVERSION_B prep)
basis: ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715 + architect in-chat R1/R2 verification (9 docs, 6/6 code spot-checks) + REGISTER_INVERSION_A_MEASURE_REPORT + FRAMEWORK.md section 14 (v2.0.0)
---

# CORPUS_CLOSURE_INVERSION_B -- Execution Brief

This cascade closes the 2026-07-15 corpus rework (PR #43) and executes REGISTER_INVERSION
Cascade B (F-34) in one session, four phases with two operator gates. Phase A reviews every
rework successor claim-by-claim against code (read-only). Phase B applies only the
operator-ratified corrections. Phase C ratifies: AUTHORED -> LOCKED v1.0.0 for the 14
A-successors under the pre-2.0 register regime. Phase D, after the operator merges PR #43,
migrates the corpus to register schema 2.0: frontmatter becomes the source of truth,
`REGISTER.yaml` becomes the derived archive, `CURRENT_AUTHORITY_SURFACE.yaml` becomes the
derived boot subset, gates are armed, PowerShell writers retire. Done means: PR #43 merged
with all successors LOCKED, and the corpus running schema 2.0 with armed gates and validate
exit 0.

**Executor**: Claude Code (Fable 5), LOCAL on the operator's machine.
**Repository**: `D:\Colony_Simulator\Colony_Simulator`.
**Reference tree (FROZEN -- read, never modify)**: `D:\Start-up_Projects\news-intelligence-hub`
(the NIH governance model this program's target law was ported from).

**Brief-integration notice**: this brief cites standing law by anchor and never restates it.
A conflict between this brief and any standing document means THE BRIEF IS WRONG -- halt and
escalate. A conflict between this brief and live code means THE CODE IS TRUE -- record and
proceed per the recon-classification law.

## 1. Mission [CORE]

| # | Deliverable | Action | Version / state |
|---|---|---|---|
| D1 | `docs/reports/CORPUS_REWORK_REVIEW_REPORT.md` | Phase A full-corpus review findings (new, untracked until C1) | enrolled E/3/EXECUTED at C1 |
| D2 | Ratified corrections to successor documents | Phase B edits on the PR branch | PATCH bumps per doc |
| D3 | 14 A-successors ratified | Phase C register closure (pre-2.0 regime) | AUTHORED -> LOCKED, 0.1.0 -> 1.0.0 |
| D4 | Register schema 2.0 live | Phase D: migrate + align + arm + retire PS writers | REGISTER.yaml + CURRENT_AUTHORITY_SURFACE.yaml derived; 4 globals SoT files |
| D5 | `docs/reports/G_RATIO_PER_RULE_BREAKDOWN.md` | Phase D measure deliverable (ruling e; feeds the architect matrix deliberation) | enrolled |
| D6 | Closure report | chat, per section 14 schema | -- |

Why this precedes everything queued behind it: Cascade B must migrate the CLEAN corpus
(ratified 2026-07-15 sequencing lean), and the authority-surface predicate
(FRAMEWORK section 14.7: Live OR LOCKED tier 1/2) only captures the 19 successors after the
Phase C flips -- so review -> fix -> flip -> migrate is the only order that does not produce
a boot subset missing the current-truth corpus.

## 2. Established facts [CORE]

Re-verify at Phase 0 the facts marked (R). Any mismatch -> HALT H1.

**Branch / PR state.** (R) Checked-out branch `claude/architecture-decomposition-review-squ1fy`
at HEAD `05fcb9e`; PR #43 (open, unmerged) carries the 10-commit corpus rework
(`8960085..05fcb9e`); last verified `main` HEAD `6f39903`. (R) `register_version: "2.31"`,
`last_modified_commit: "1b420e0"`, 319 documents, EVT count 54, schema_version "1.0"
(pre-2.0 regime operative per FRAMEWORK section 14.9).

**Review corpus (Phase A scope).** 14 Category A AUTHORED successors (register ids exactly as
EVT-2026-07-15-CORPUS_REWORK_R4_MECHANICS checklist item [1] lists them): ARCHITECTURE_V2,
KERNEL_ARCHITECTURE, SCHEDULER_ARCHITECTURE, THREADING_V2, ECS_V2, MOD_OS_ARCHITECTURE,
MODDING_V2, CONTRACTS_V2, ANALYZER_RULES_V2, VULKAN_SUBSTRATE_V2, FIELDS_V2, EVENT_BUS_V2,
PERFORMANCE_V2, FHE_INTEGRATION_CONTRACT_V2 -- all in `docs/architecture/`, all verified by
their authors against code at `35364c2`. Plus 5 Category J Tier-1 Drafts in `docs/mechanics/`
(COMBO_RESOLUTION, COMPOSITE_REQUESTS, RESOURCE_MODELS, GOLEM_OWNERSHIP, FEEDBACK_LOOPS),
`docs/mechanics/README.md`, and the 7 cross-cutting AUTHORED drafts of the 2026-07-15 set
(EXECUTION_AUTHORITY_MATRIX, CONCURRENCY_AND_MEMORY_MODEL, RESOURCE_OWNERSHIP_AND_LIFETIME,
ENGINE_LIFECYCLE_AND_TRANSACTIONS, TIME_AND_CONSISTENCY_MODEL, IDENTITY_AND_ABI_CONTRACT,
PERSISTENCE_SNAPSHOT_CONTRACT) reviewed for internal consistency only (their ratification is
out of scope, section 15).

**Architect pre-verification (chat, 2026-07-16).** 9 successors already line-read in full;
6/6 code spot-checks exact (GameBootstrap dual registration incl. the 10-system census;
EntityId.IsValid; DependencyGraph BuildCycleException as plain InvalidOperationException;
ValidationErrorKind = exactly 15 members; capability-grammar regex verbatim). Two seeded
findings the Phase A report MUST carry:

- **SEED-1 (KERNEL_ARCHITECTURE, blocks its flip until resolved).** Part 0 line ~37 states
  "Series state: 21 invariants -- К-L1..К-L19 (К-L6 SUPERSEDED counted) + sub-invariants
  К-L3.1/К-L7.1/К-L15.1". Arithmetic: 19 + 3 = 22, and the Part 0 table itself carries 22
  invariant rows (К-L20 reserved separately, excluded). Sharpened form of ROADMAP finding
  F-9. Resolution is an operator decision at HALT-1: (a) correct the headline to 22 and
  update every "21 final" citation, or (b) recompose the counting convention keeping 21.
- **SEED-2 (CONTRACTS_V2, one line).** Cross-references table row "MODDING.md | cites |
  author guide, ALC refusal list" cites a mechanism the same rework retires as fictional
  (MODDING_V2 section 5; MOD_OS_ARCHITECTURE section 1.4). Reword the note (lean:
  "ALC resolution truth (no refusal list) + author guide", matching ARCHITECTURE_V2's row).

**Ratification law.** The 7-item CORPUS REWORK CLOSURE ratification checklist is carried
verbatim in EVT-2026-07-15-CORPUS_REWORK_R4_MECHANICS `governance_impact` -- it is the
Phase C work order. Item [3]: the 5 J-docs STAY Draft (mechanics unshipped); Phase C only
confirms their `constrains` lists and naming note. Item [6] (7-drafts ratification) is out
of scope. Until each flip lands, historical SUPERSEDED predecessors prevail on conflict
(item [7]).

**Inversion law and instrument.** FRAMEWORK.md v2.0.0 section 14 is the complete target law:
14.1 inversion core (frontmatter SoT; derived archive + derived boot subset; 4 globals SoT
files REQUIREMENTS/RISKS/CAPA/AUDIT_TRAIL.yaml; byte-reproducibility invariant), 14.2
tool-law relation + failability, 14.3 schema v2 fields (PENDING-* outlawed;
last_modified_commit optional), 14.4 sentinel law (closed set incl. 'null' for terminals),
14.5 gate catalog (G-SCHEMA/UNIQUE/XREF/CATLIFE/TERMINAL/SENTINEL/PATH/NAMESPACE/GLOBALS +
G-RATIO monitor), 14.7 authority-surface predicate (tunable law), 14.8 measure->align->arm,
14.9 transition status. The instrument is `tools/DualFrontier.Governance` (validate | sync |
migrate; `--armed`; migrate dry-runs against a scratch `--target`, live mutation only behind
`--i-understand-this-mutates-the-corpus`). Cascade A measure baseline
(`docs/reports/REGISTER_INVERSION_A_MEASURE_REPORT.md`): 288 derived docs, idempotent, 7
report-only findings, G-RATIO 94/285 = 33.0%. (R) The corpus has since grown to 319 -- the
Phase D re-measure supersedes every stale count.

**The five ratified Cascade-B rulings (operator, 2026-07-15 -- binding verbatim).**
(a) formalize `REGISTER_SUPPLEMENT.yaml` as the standing non-.md SoT; (b) the 5 UNCLEAR ->
4 briefs D/3/EXECUTED, the prompt E/3/EXECUTED; (c) measure finding #3 -> align to 'null',
EXECUTED stays terminal; (d) measure finding #6 -> align 'TBD --' values to the
'post-<event> closure' form; (e) G-RATIO matrix question DEFERRED to its own architect
deliberation WITH the per-rule breakdown (D5) -- does not block Phase D mechanics.

**F-ledger anchors.** F-34 = Cascade B work order (closes at D). F-2 = 123 PENDING-COMMIT
placeholders -- structurally dissolved by section 14.3 (placeholder vocabulary outlawed;
field optional; real hashes MAY be retained where real): the sanctioned migration path is
DROP the PENDING-* values, close F-2 as dissolved-by-inversion (hashes remain recoverable
from git history). F-9 = the К-L count ambiguity -- resolved by SEED-1's HALT-1 decision.
F-13 = render-script defect -- mooted if the render writer retires at Phase D (section 8).

## 3. Phase 0 -- preconditions and checkpoint [CORE]

Run serially before anything else.

1. **Verify the (R) facts** of section 2 (branch, HEAD, main HEAD, register_version/doc/EVT
   counts read directly from `REGISTER.yaml` -- do not run validate yet). Mismatch -> HALT H1.
2. **Baseline gates**: full managed + native build and full test run, commands per
   `docs/methodology/DEVELOPMENT_HYGIENE.md`. Record result as the regression anchor and
   state the pre-existing-failure shape verbatim in the report. Closure must match-or-improve
   -> HALT H2 on regression.
3. **Validation checkpoint** (pre-2.0 regime):
   `powershell -NoProfile -ExecutionPolicy Bypass -File tools\governance\sync_register.ps1 -Validate`
   -- exit != 0 -> HALT H3. The refreshed `VALIDATION_REPORT.md` lands inside C1
   (validate-fold protocol; every later register-touching commit folds its own refresh).
4. **REGISTER enum read** (Lesson #N14): extract the empirically-used category / tier /
   lifecycle values and the exact `DOC-` and `EVT-` entry shapes from `REGISTER.yaml`.
   These verbatim shapes are the only sanctioned templates for the Phase C cascade.
5. **New-tool smoke**: `dotnet run --project tools/DualFrontier.Governance -- validate`
   (report-only regime) runs and exits per FRAMEWORK section 14.2 expectations; governance
   test suite green. Failure -> HALT H2 (the Phase D instrument must be proven before
   Phase A spends the session).
6. **Mandatory reads**: this brief in full; `docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md`;
   `docs/reports/REGISTER_INVERSION_A_MEASURE_REPORT.md`; FRAMEWORK.md section 14 in full;
   the EVT-R4 ratification checklist; `docs/methodology/METHODOLOGY.md` closure protocol.

NEVER run `-Sync` outside the Phase C cascade. `render_register.ps1` runs at most once, at
the Phase C render commit. The executor NEVER pushes -- pushes and the PR #43 merge are the
operator's manual steps (the auto-mode push block is expected behavior, not a fault).

## 4. Topology [CORE]

Single orchestrator throughout; **Wave R only in Phase A** -- four parallel read-only review
agents (R1 kernel cluster: ARCHITECTURE_V2/KERNEL_ARCHITECTURE/SCHEDULER_ARCHITECTURE/
THREADING_V2/ECS_V2; R2 platform: CONTRACTS_V2/MOD_OS_ARCHITECTURE/MODDING_V2/
ANALYZER_RULES_V2; R3 substrate: VULKAN_SUBSTRATE_V2/FIELDS_V2/EVENT_BUS_V2/PERFORMANCE_V2/
FHE_INTEGRATION_CONTRACT_V2; R4 mechanics + drafts: the 5 J-docs, mechanics README, 7
drafts). HARD RULES: only the orchestrator runs git add/commit; `docs/ROADMAP.md` and
`REGISTER.yaml` are orchestrator-only; no agent touches the NIH reference tree; Phases B/C/D
are strictly serial (register mutation and migration are incompatible with parallel writers).

## 5. Wave R -- Phase A review agents [KIND: audit within a combined cascade]

Phase A is read-only: its ONLY filesystem output is D1. Each agent returns, per document, a
fixed-schema inventory:

1. **Anchor verification, exhaustive.** Every `file:line` / `file:range` claim in the
   document is opened and verdict-ed: EXACT | DRIFTED (content there, lines moved) |
   WRONG (content absent or contradicting). The rework verified at `35364c2`; the PR head
   `05fcb9e` added no `src/`/`native/` changes -- (R) confirm via
   `git diff --stat 35364c2..05fcb9e -- src native tools`, and if code DID change, verify
   against `05fcb9e` and record the delta.
2. **Enforcement-verb truth audit** per the standing truth law: no "is checked / analyzer
   reports / tests verify" claim without an on-disk enforcer; future capability only behind
   FENCED / `Planned -- see ROADMAP.md`.
3. **Internal-consistency sweep**: cross-references between same-rework documents (the
   SEED-2 class); FENCED discipline; Non-goals vs content leakage.
4. **Ratification-readiness verdict**: READY | READY-WITH-FIX (list) | NOT-READY (reason).

The report schema (D1): R0 base state; R1..R4 per-cluster tables (one row per document:
anchors checked / EXACT / DRIFTED / WRONG, truth-law verdict, findings, readiness); R5
consolidated findings register (SEED-1, SEED-2, plus new findings F-A-1..n, each with exact
proposed fix text where mechanical); R6 PR review-checklist mapping (which PR body checkboxes
this report substantiates); R7 self-attestation (zero writes except D1; zero git mutations in
Phase A; every count with its verbatim expression).

## 6. Checkpoints [CORE]

**C-A (HALT-1, operator gate -- end of Phase A).** Orchestrator presents D1 in chat and
HALTS. The operator: (1) decides SEED-1 (22 vs 21-recomposed) -- this is also the F-9
disposition; (2) approves KERNEL Annex A rows A-1..A-4 (checklist item [2]); (3) acknowledges
the MIGRATION_PLAN superseded_by: DOC-C-ROADMAP scalar (item [4]); (4) ratifies the fix list
(every F-A finding marked FIX-NOW / DEFER-to-F-ledger / REJECT). No Phase B edit outside the
ratified list. A NOT-READY verdict the operator does not resolve -> that document's flip is
withheld and recorded (partial-flip is legal; item [7] closes per-document).

**C-B (end of Phase B).** Orchestrator self-audit: every applied fix matches its ratified
text; no edit outside the list; build+tests match baseline; truth-law spot-recheck on every
edited section.

**C-C (HALT-2, operator gate -- end of Phase C).** Validate exit 0, render regenerated,
closure state presented. The operator merges PR #43 into `main` and confirms in-session.
The orchestrator then re-reads `main` HEAD (H1 re-verify) and branches for Phase D.

**C-D (end of Phase D).** The armed validate exits 0; second sync is byte-identical
(derived-register integrity, section 14.1); failability suite green (every gate proven
red-on-fixture; section 14.8); globals files parse and merge; `CURRENT_AUTHORITY_SURFACE.yaml`
contains exactly the section 14.7 predicate set -- spot-check that all 14 flipped successors
plus FRAMEWORK itself appear and that no AUTHORED/Draft/SUPERSEDED document does.

## 7. Execution specifications [CORE]

**Phase B (corrections, PR branch).** One commit per document touched (atomic law). SEED-2
and every mechanical FIX-NOW: apply the ratified text exactly. SEED-1 per the HALT-1
decision: variant (a) edits the KERNEL Part 0 headline to 22 AND greps the corpus for every
"21 invariant" / "21 final" citation (`rg -n "21 (invariant|final)"` both scripts) updating
each in the same commit; variant (b) applies the operator's recomposition text verbatim.
Each edited successor bumps PATCH (0.1.0 -> 0.1.1) in its register entry at Phase C (the
pre-2.0 regime keeps frontmatter mirrors -- do NOT hand-edit mirrors; they refresh at the
Phase C sync).

**Phase C (ratification cascade, pre-2.0 regime, PR branch).** Using ONLY Phase 0 enum
shapes: (1) the 14 A-successors (minus any withheld at HALT-1): lifecycle AUTHORED -> LOCKED,
version -> 1.0.0, `next_review_due` -> the operator-chosen sanctioned form (default lean:
'2027-Q3'), `special_case_rationale` updated (the AUTHORED-override wording retires with the
flip); (2) predecessor rationales: the "until successor ratification (deliberate
authority-gap window)" clauses close per-document; (3) REQ-K-L1..K-L19 transfer from
DOC-A-KERNEL to DOC-A-KERNEL_ARCHITECTURE + K_CLOSURE_REPORT rationale update (item [2]);
(4) DOC-C-ROADMAP gains a real `next_review_due` (item [5]); (5) MIGRATION_PLAN rationale
gains the operator-acknowledged note (item [4]); (6) J-doc `constrains` confirmation recorded
(item [3]); (7) D1 enrolled E/3/EXECUTED and this brief -> EXECUTED; (8) one EVT
(`EVT-2026-07-<dd>-CORPUS_CLOSURE_RATIFICATION`) carrying real hashes; register_version
2.31 -> 2.32 (or next per any interim state); validate exit 0 folded; render + header
backfill as the final PR-branch commit.

**Phase D (inversion, on `main` post-merge, new branch).** Strictly per FRAMEWORK section
14.8 measure -> align -> arm:

1. **Measure**: dry-run `migrate --target <scratch copy>` of the post-merge corpus; write
   `docs/reports/REGISTER_INVERSION_B_MEASURE_REPORT.md` (supersedes stale counts) + D5
   (G-RATIO per-rule breakdown). New UNCLEAR classes beyond the five ratified rulings ->
   HALT H-D1.
2. **Align**: apply rulings (a)-(d); drop PENDING-* values (F-2 dissolution); align sentinel
   forms (14.4); resolve every measure finding within the ratified vocabulary.
3. **Migrate live**: frontmatter injection as SoT across the corpus; `REGISTER.yaml`
   regenerated as the derived archive; `CURRENT_AUTHORITY_SURFACE.yaml` generated; globals
   split to the four hand-edited SoT files (AUDIT_TRAIL.yaml append-only). The live flag is
   typed once, deliberately, and recorded in the commit body.
4. **Arm + retire**: gates armed; PowerShell writers retired per section 14.9 and section 8
   below; meta-entry roles update (DOC-G-REGISTER -> derived-archive role;
   CURRENT_AUTHORITY_SURFACE joins as meta-entry); FRAMEWORK section 14.9 transition-status
   text updated to record completion (PATCH-class, pre-authorized by the operator's
   ratification of this brief); F-34/F-2 closed in ROADMAP with hashes.

## 8. Governance machinery [KIND: governance]

**PS-writer retirement (exact scope).** Retire = delete from the tree in the arm commit:
`tools/governance/sync_register.ps1` (the frontmatter-mirror writer) and
`tools/governance/render_register.ps1`. `REGISTER_RENDER.md` transitions to historical
status (its generator retires; F-13 closes as mooted). Default lean for the mirror-era
`register_view_url` frontmatter field: dropped at migration (not in the section 14.3 field
set). If the Migrator's actual behavior differs from these leans -> record as a skeleton
revision if surface-level, HALT H-D2 if it would lose information (round-trip-loses-nothing
directive).

**Exact-text law**: Phase C rationale/EVT texts and the Phase D FRAMEWORK 14.9 completion
note are drafted by the executor in the established register voice, presented at their
commit point in the closure flow -- they are mechanical state-recording, not new law
(the one FRAMEWORK edit is pre-authorized above and PATCH-only; any wording that would
change law semantics -> HALT H6).

## 9. S-LOCK invariants [CORE]

None added. Preserved structurally: the derived-register integrity invariant (byte-identical
re-sync, section 14.1) and gate failability (section 14.8) are enforced by the governance
test suite this cascade arms -- they are Cascade A's locks becoming live, not new ones.

## 10. Census discipline [CORE]

No reserved-stub or pragma surface is touched; the analyzer waiver HARD pin (exactly 2
DFK-WAIVER sites) must be unchanged at closure -- `rg -n "DFK-WAIVER" src/` = 2. SOFT:
the "21 invariant" citation census of Phase B (expression recorded in section 7) moves by
design under SEED-1 variant (a); record as census-delta, not a finding.

## 11. Commit plan [CORE]

Intended form; a defect iteration may add a commit -- record the deviation, never compress.

| # | Branch | Subject | Content |
|---|---|---|---|
| C1 | PR | `governance(review): enroll CORPUS_CLOSURE_INVERSION_B brief + review report + validation checkpoint` | brief, D1, VALIDATION_REPORT |
| C2 | PR | `docs(kernel): resolve К-L series-count per HALT-1 decision (F-9)` | SEED-1 fix + citation census |
| C3 | PR | `docs(contracts): correct MODDING cross-reference row (refusal-list retirement)` | SEED-2 |
| C4..Ck | PR | `docs(<scope>): <ratified fix>` | one per HALT-1 FIX-NOW item |
| Ck+1 | PR | `governance(register): corpus rework ratification closure (2.31 -> 2.32)` | flips, REQ transfer, rationales, EVT, validate folded |
| Ck+2 | PR | `governance(register): render regeneration + header backfill` | final pre-2.0 render |
| -- | -- | HALT-2: operator merges PR #43 | -- |
| CD1 | main | `governance(inversion): Cascade B measure report + G-RATIO per-rule breakdown` | reports enrolled per 2.0 discipline |
| CD2 | main | `governance(inversion): corpus align -- rulings a-d, PENDING-* dissolution, sentinel forms` | align edits |
| CD3 | main | `governance(inversion): live migration -- frontmatter SoT, derived archive + authority surface, globals split` | the switch |
| CD4 | main | `governance(inversion): arm gates, retire PS writers, meta-roles, 14.9 completion, F-34/F-2 closure` | arm + closure |

## 12. REGISTER cascade [CORE]

Phase C is the LAST cascade under the pre-2.0 regime: mutations only in the Phase 0 verbatim
enum shapes; the single new PENDING-COMMIT is this brief's unavoidable header self-reference,
backfilled at Ck+2. Phase D thereafter operates under section 14: enrollment = frontmatter in
the document itself; the archive and surface regenerate by tool; the closure EVT of Phase D
is an `AUDIT_TRAIL.yaml` append (its first). Armed validate exit 0 is the closure gate for
both regimes' final commits.

## 13. Halt conditions (H-series) [CORE]

- **H1** precondition/(R) mismatch, incl. post-merge main-HEAD re-verify.
- **H2** build/test regression vs baseline, or governance-tool/test smoke failure at Phase 0.
- **H3** validate nonzero (either regime's tool at its own gate).
- **H4** Wave-R material contradiction with section 2 beyond explained method deltas.
- **H5** a register enum/sentinel value needed that the empirical vocabulary lacks --
  escalate, never invent (both regimes; section 14.4 set is CLOSED).
- **H6** truth-law or law-semantics unsatisfiable without an architectural decision.
- **HALT-1 / HALT-2** the operator gates of section 6 -- always stop, never assume.
- **H-D1** measure surfaces an UNCLEAR class outside the five ratified rulings.
- **H-D2** migration would drop information (round-trip-loses-nothing violated).
- **H-D3** post-migration derived artifacts not byte-reproducible, or the authority surface
  violates the 14.7 predicate on spot-check.
- Standing rails: no pushes; no `-Sync` outside Phase C; no history rewrite / force-push /
  squash; single-writer files honored; NIH tree untouched; `docs/architecture/historical/`
  is read-only in every phase (symmetric preservation -- altering a HISTORICAL predecessor
  is halt-class).

On halt: stop, report state verbatim, await the operator.

## 14. Closure protocol and report [CORE]

Execute the METHODOLOGY session closure protocol. ROADMAP write-back: F-34 CLOSED (hash),
F-2 CLOSED dissolved-by-inversion (hash), F-9 CLOSED per HALT-1 (hash), F-13 disposition,
forward-state refresh (next: the 7-drafts ratification deliberation; the G-RATIO matrix
deliberation now unblocked by D5). Every DEFER-to-F-ledger finding from HALT-1 gets its
F-entry -- nothing chat-only. Closure report (chat): commits table (hash | branch | subject);
versions table incl. register 2.31 -> 2.32 -> schema-2.0 state; gates table baseline vs
closure; census pins (waiver HARD = 2; SEED-1 SOFT delta); F-ledger final states; skeleton
revisions consolidated; self-attestation (no pushes; single render run; no -Sync outside
Phase C; no history rewrite; NIH + historical/ untouched; live-migrate flag typed exactly
once); operator manual checklist (push Phase D branch; merge; the standing operator-owned
queue: 7-drafts ratification order, G-RATIO matrix deliberation, TESTING_STRATEGY 2.6
micro-patch, F-30/F-31/F-32).

## 15. Out of scope [CORE]

The 7 cross-cutting drafts' ratification and preamble rewrites (checklist item [6] --
architect deliberation); the G-RATIO threshold/matrix decision itself (ruling e -- D5 only
feeds it); TESTING_STRATEGY section 2.6 ASan wording; `set_phase_barrier` SingletonGuard
hardening; F-30/F-31/F-32; any `src/`/`native/` code change beyond zero (this cascade edits
documents and governance only -- a needed code change is H6); the NIH reference tree; pushes
and the PR merge (operator); mods/vanilla content.

---

**End of CORPUS_CLOSURE_INVERSION_B_BRIEF.md v1.0**