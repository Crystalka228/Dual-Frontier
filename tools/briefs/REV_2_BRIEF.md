---
register_id: DOC-D-REV_2_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-11-22'
last_modified: '2026-11-22'
content_language: en
next_review_due: null
title: 'REV_2 -- the governance FRAMEWORK says what the tool does: the rationale field splits (G-RATIO counts only real exemptions), the retired PowerShell regime leaves the law, the gate catalog is test-pinned to the tool, the F-ledger gets a row-form gate, schema 2.1 -- the third cascade of CORPUS_REVISION_1'
authored_by: Claude Opus (architect-orchestrator session, CORPUS_REVISION_1)
last_review_event: 'REV_2 EXECUTED 2026-11-22 (EVT-2026-11-22-REV_2) on claude/rev-2, C1-C6, 15 review-repair commits, 3 repair-over-repair commits and the C7 closure. Operator-ratified inputs: G-RATIO option A, REV_2 on Sonnet; the architect rulings A-1..A-10 are PENDING OPERATOR REVIEW at the PR. Lifecycle Draft -> EXECUTED. Wording corrected at closure: the displaced ledger row is F-13 (sections 2 and 7.3), halt H7 concerns code files (section 13).'
basis: 'REV_2 recon (/mnt/Work/Temporal/rev2-run/recon/R1.md, Sonnet, read-only at 3fa781c) + operator rulings 2026-11-21 (G-RATIO option A; REV_2 on Sonnet; self-merge grant through REV_3) + architect rulings A-1..A-10 (/mnt/Work/Temporal/rev2-run/RULINGS.md, pending operator review) + the revision synthesis'
---

# REV_2 -- Execution Brief

The governance FRAMEWORK still writes the retired PowerShell regime as law, names
instruments the .NET tool does not have, and counts provenance notes as exemptions:
G-RATIO reads 44.7% against a 20% threshold it never compares, while only 16 documents
carry a genuine exemption. The F-ledger has no form check and three malformed rows.
This cascade makes the law, the tool and the corpus agree. Done = FRAMEWORK 2.1.0
describes exactly the tool; schema 2.1 with `provenance_note`; G-RATIO a real gate
reading about 4%; a test pins the gate catalog to FRAMEWORK; G-ROWFORM armed on
structure; the text residue of REV_1 paid; gates green.

Executed by the architect-orchestrator through Sonnet subagents (operator ruling for
this cascade), integrator on `/mnt/Work/_wt/df-rev-2`, branch `claude/rev-2`.
Brief-integration notice: commit law CODING_STANDARDS section 8; per-commit duty by
tier DEVELOPMENT_HYGIENE section 3; test invocation TESTING_STRATEGY section 8.
Conflict with a standing document this cascade does not amend -> halt. Conflict with
live code -> the code is the truth, recorded.

## 1. Mission [CORE]

| D | Artifact | Version | Owner |
| - | -------- | ------- | ----- |
| D1 | this brief | enroll D/3/Draft | integrator C1 |
| D2 | `docs/governance/FRAMEWORK.md` | 2.0.2 -> 2.1.0 (MINOR) | U-FW |
| D3 | `docs/ROADMAP.md` ledger: four row-form repairs; seed F-95 | -- | integrator C3 (seam) |
| D4 | frontmatter migration: 151 tracked documents + `REGISTER_SUPPLEMENT.yaml` | -- | U-MIG |
| D5 | RISKS, REQUIREMENTS, ANALYZER_RULES 1.1.2, TESTING_STRATEGY 2.3.1, CODING_STANDARDS 4.0.1, BYPASS_LOG, SYNTHESIS_RATIONALE, tools/governance (MODULE.md, SCOPE_EXCLUSIONS.yaml, query_register.ps1 retired) | as listed | U-TEXT |
| D6 | `tools/DualFrontier.Governance/**` + `tests/DualFrontier.Governance.Tests/**` | schema 2.1 | U-TOOL |
| D7 | CAPA entry, EVT, brief | closure | integrator C7 |

## 2. Established facts [CORE]

(RV) re-verified at Phase 0; mismatch -> H1. Anchors at `3fa781c`.

1. (RV) main `3fa781c`; build 0W/0E; suite 1441/0/6 (10 TRX); selftest ALL PASSED; dev
   smoke 124; `validate --armed` exit 0 with G-RATIO monitor 169/378 on the checkout
   (167/375 on a clean worktree -- the two untracked DIST_1 reports carry rationales).
2. (RV) The 16 genuine rationales (forbidden combinations) and the 151 tracked movable
   ones are listed in `/mnt/Work/Temporal/rev2-run/recon-scratch/R1_genuine_16.tsv` and
   `R1_move_153.tsv` (the two UNTRACKED rows are out of scope here). All values are
   single-line scalars; `REGISTER_RENDER.md:19-20` has a duplicate key; body hazards at
   `tools/briefs/A_PRIME_9_0_RECONNAISSANCE_BRIEF.md:633` and
   `docs/architecture/A_PRIME_9_0_AMENDMENTS_LOG.md:550` (R1 item 11).
3. (RV) G-RATIO code: `Validators.cs:279-286`; predicate `Frontmatter.cs:100` shared
   with G-CATLIFE (`Validators.cs:147`); the 20% is printed, never compared. Unknown
   frontmatter keys are already accepted (R1 item 8).
4. (RV) Gate names match 10:10 between FRAMEWORK 14.5 and the tool; no registry, no
   parity test (R1 item 15); clause-level gaps R1 item 16.
5. (RV) Ledger `docs/ROADMAP.md:1069-1164`, 94 rows F-1..F-94; structural defects F-55,
   F-56, F-59 (:1125, :1126, :1129) and F-13 out of order (it sat at :1072, before F-2); vocabulary defects 14
   severity + 8 strict-state (R1 item 22). Only `Monitor` is exit-neutral under
   `--armed` (`RegisterSync.cs:131-132`).
6. The rulings A-1..A-10 (`/mnt/Work/Temporal/rev2-run/RULINGS.md`) are binding; R1's
   measured facts are the substrate; units draft replacement text themselves (R1 gives
   facts, not prose).

## 3. Phase 0 [CORE]

Verify section 2 (RV); native copied (native/ unchanged); baseline: build 0W/0E, full
suite 1441/0/6 with 10 TRX, smoke 124, `validate --armed` exit 0; frontmatter-shape
read (FRAMEWORK 14.3/14.4, a CAPA entry, EVT-2026-11-21-REV_1); mandatory reads: this
brief, RULINGS.md, R1 sections for the unit.

## 4. Topology [CORE]

Integrator (sonnet/high) the only committer; four units in parallel, DISJOINT sets,
all sonnet/high: U-FW (FRAMEWORK.md), U-MIG (the 151 migration files +
REGISTER_SUPPLEMENT.yaml), U-TEXT (D5 files; ANALYZER_RULES's own rationale migration
is U-TEXT's, not U-MIG's), U-TOOL (tool + its tests). Seams: ROADMAP, AUDIT_TRAIL,
CAPA, the brief, derived registers.

## 5. Wave R

None (R1 at 3fa781c; Phase 0 re-verifies).

## 6. Checkpoints [CORE]

- C-green after C1; C-stage after C6 (full chain once): build 0W/0E; full suite with
  the TRX count equal to the assemblies (U-TOOL adds tests: the new total is the base
  plus its added cases, named); `validate --armed` exit 0 with G-RATIO about 4.3% and
  G-ROWFORM structural findings 0; the vocabulary findings printed as monitors; smoke 124.

## 7. Writer specifications [CORE]

**7.1 C1** enroll the brief.

**7.2 U-FW (C2).** FRAMEWORK 2.1.0 per A-2, A-4 (document side), A-5, A-6 (the 14.5 table
gains G-ROWFORM and states the gate registry is test-pinned), A-7 (the G-ROWFORM row and
its armed/monitor split), and the 14.3 field list gains `provenance_note` (optional, free
text, never an exemption). Every R1 item-12 site resolved; R1 item-16 clauses narrowed;
the 2.0.1 change row restored; change history 2.1.0 entry; frontmatter bumped. Write
F-95's text into the report (the integrator seeds it).

**7.3 C3 (integrator seam).** In `docs/ROADMAP.md`: repair F-55 and F-56 (add the
trailing pipe), F-59 (escape the stray pipe at its char ~2909, add the trailing pipe),
move F-13 (the displaced row; F-2 stays) to its numeric place; seed F-95 (U-FW's text: the FRAMEWORK clauses narrowed to
the tool -- the unbuilt checks listed; S3, OPEN, owner architect; F-93 row form). Change
nothing else in the ledger (vocabulary is REV_3).

**7.4 U-MIG (C4).** For every file of `R1_move_153.tsv` flagged tracked (151) and
`docs/governance/REGISTER_SUPPLEMENT.yaml:59`: rename the frontmatter key
`special_case_rationale` -> `provenance_note`, value byte-identical, inside the
frontmatter block only (README-class: the end-of-file block). `REGISTER_RENDER.md`: one
`provenance_note` with the second (archive-visible) value. ANALYZER_RULES is excluded
(U-TEXT). Prove with a script: per file, the frontmatter diff is exactly one key line;
the 16 genuine files untouched; the two body hazards untouched.

**7.5 U-TEXT (C5).** A-8, A-9, A-10 exactly; ANALYZER_RULES also migrates its own rationale
(`:22`) to `provenance_note`. Each amended standing document gets its version bump,
`last_modified`, change-history row in its own format.

**7.6 U-TOOL (C6).** `Frontmatter` gains `ProvenanceNote` (parsed, carried to the archive,
never an exemption); G-RATIO per A-3; schema `2.1` per A-4 (all writers, the test pin);
a gate registry (one static list of gate ids) and a parity test reading FRAMEWORK 14.5
(A-6); G-ROWFORM per A-7 as an I/O-backed corpus gate merged where the globals gate is
(`RegisterSync.CollectFindings`), its ledger path from one config value (the
`tools/governance/` config precedent of `ExclusionConfig`), inert when the file is absent;
tests: positive and negative fixtures for every structural and vocabulary rule (copy the
real row shapes R1 item 24 names), a G-RATIO threshold test both sides of 20%, the parity
test, each new gate watched red once (record both readings). `Validators.cs:414` comment
re-pointed (A-9). Code-tier gate: build + governance tests.

**7.7 C7 (closure).** ONE EVT `EVT-2026-11-22-REV_2` (FRAMEWORK 7.2 schema amendment
2.0 -> 2.1 recorded in it; rulings A-1..A-10 listed as architect rulings pending operator
review); a CAPA entry for the schema amendment per FRAMEWORK 7.2 (shape of an existing
entry); brief EXECUTED; sync; validate exit 0. Note in
`/mnt/Work/Temporal/dist1-run/PAUSE_2026-11-21.md`: the two DIST_1 reports migrate their
rationale to `provenance_note` at DIST_1 C1.

## 8. Kind-specific [KIND: governance]

Landing order is C1, C2 (FW), C3 (ROADMAP), C4 (MIG), C5 (TEXT), C6 (TOOL): the tool's
armed G-RATIO and G-ROWFORM and its parity test are green only after the law, the
ledger repairs and the migration are on the branch.

## 9. S-LOCK [CORE]

None touched.

## 10. Census [CORE]

U-TOOL edits `tools/` only (outside the census scope); no `src/` change; no pin moves.

## 11. Commit plan [CORE]

| # | Subject |
| - | ------- |
| C1 | `governance(enroll): REV_2 brief enrolled` |
| C2 | `docs(framework): 2.1.0 -- the law describes the tool; the retired regime is history; provenance_note joins the schema` |
| C3 | `governance(ledger): four rows take the ledger's own form; F-95 seeded` |
| C4 | `governance(register): provenance notes leave special_case_rationale -- only forbidden combinations keep an exemption` |
| C5 | `docs(governance): the operational residue of REV_1 is paid -- risks, analyzer rules, census claim, amendment scope` |
| C6 | `feat(governance): schema 2.1 -- G-RATIO is a real gate, the gate catalog is test-pinned, G-ROWFORM guards the ledger` |
| C7 | `governance(closure): REV_2 EVT -- schema 2.1; the governance law is the tool's` |

## 12. REGISTER cascade [CORE]

sync in every frontmatter-touching landing; derived files never hand-edited;
AUDIT_TRAIL and CAPA append-only; validate exit 0 at C1, C2, C3, C4, C5 (the old tool
still reads the old field -- after C4 the G-RATIO monitor drops to 16/375) and C6/C7.

## 13. Halts [CORE]

H1 Phase 0 mismatch; H2 gate regression; H3 validate nonzero; H5 vocabulary gap; H6 a
change needs semantics beyond A-1..A-10 and the operator rulings; H7 any code file under `src/`,
`mods/`, `native/` (the frontmatter-only edits of `*.md` files that 7.4 requires there are not one). Standing rails as REV_1.

## 14. Closure [CORE]

Commits, versions, gates baseline vs closure (suite delta = U-TOOL's named tests),
G-RATIO before/after, G-ROWFORM findings before/after (structural 5 -> 0; vocabulary
monitors listed), migration proof, friction table, self-attestation; push once; PR body
lists A-1..A-10 for the operator's review.

## 15. Out of scope [CORE]

Ledger split, state/severity normalization (REV_3); METHODOLOGY (REV_4); the unbuilt
checks of F-95; the DIST_1 reports' migration (at DIST_1).

---

**End of REV_2_BRIEF.md v1.0**
