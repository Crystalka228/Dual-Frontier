# REGISTER_INVERSION Cascade A — Measure Report

*The dry-run measure. Cascade A builds the inverted-register instrument and changes
no governed state; this report is the **work order for Cascade B (F-34)**. Every
finding below is a REPORT-ONLY gate result over a full scratch-copy migration of
the live corpus, classified into a Cascade-B work class: **align-edit**,
**enroll**, **exclude**, or **architect-ruling**. Deployment discipline is
measure → align → arm (FRAMEWORK 14.8): B aligns the corpus to these findings,
then arms the gates.*

- Authored: 2026-07-15, Cascade A (the instrument).
- Instrument: `tools/DualFrontier.Governance` (`migrate --target <scratch>` +
  `validate`), net10.0.
- Corpus: a `%TEMP%` scratch copy of the repo at `HEAD` (the live corpus was not
  mutated).

---

## 1. Round-trip reconciliation (the C5 proof)

The migration round-trips the corpus with **zero unreconciled deltas**:

| Property | Result |
|---|---|
| documents migrated (.md frontmatter injected) | **285** |
| non-.md carried to the provisional supplement | **2** (the Launcher `.cs`) |
| DOC-G-REGISTER deterministic self-entry | 1 |
| **total derived documents** | **288** (= the old register's 288) |
| reconciliation deltas beyond the ratified drops | **0** (zero lost, zero invented) |
| globals extracted (REQ / RISK / CAPA / EVT) | **41 / 14 / 17 / 47** (exact) |
| second `sync` byte-identical (idempotency) | **true** |
| `sync` exit on the migrated corpus | **0** (structurally clean) |
| required-field backfills | `project` 285, `first_authored` 276 |

Ratified drops applied per document: `register_view_url` (always);
`last_modified_commit` where it held a `PENDING-*` placeholder. `id` → `register_id`
rename; `path` re-derived from the file location. The next_review_due sentinel
`'null'` and bare YAML `null` are treated as the same value (FRAMEWORK 14.4).

---

## 2. Report-only gate findings (the semantic measure)

Seven semantic-gate findings on the migrated corpus. All REPORT-ONLY in Cascade A;
each is classified for Cascade B.

| # | Gate | Document | Finding | Class |
|---|---|---|---|---|
| 1 | G-TERMINAL | `DOC-A-GODOT_INTEGRATION` | SUPERSEDED, `next_review_due: 2027-05-12` (must be `'null'`) | **align-edit** → `'null'` |
| 2 | G-TERMINAL | `DOC-A-VISUAL_ENGINE` | SUPERSEDED, `next_review_due: 2027-05-12` (must be `'null'`) | **align-edit** → `'null'` |
| 3 | G-TERMINAL | `DOC-A-A_PRIME_9_RECONNAISSANCE_REPORT` | EXECUTED, `next_review_due: 'post-A'.9.1 closure'` (must be `'null'`) | **architect-ruling** |
| 4 | G-SENTINEL | `DOC-E-DOC_DRIFT_REFACTOR_PROGRESS` | `next_review_due: 'on-refactor-cascade-execution'` (not sanctioned) | **align-edit** |
| 5 | G-SENTINEL | `DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT` | `next_review_due: 'on-refactor-cascade-execution'` (not sanctioned) | **align-edit** |
| 6 | G-SENTINEL | `DOC-F-SRC-LAUNCHER` | `next_review_due: 'TBD — after Vanilla mods … cascade'` (not sanctioned) | **architect-ruling** |
| 7 | G-XREF | `DOC-D-K8_2_V2` | supersedes `DOC-D-K8_2_V1_DEPRECATED`, which has no resolving `superseded_by` | **align-edit** |

**Notes on the architect-ruling findings:**
- **#3** — an EXECUTED report carrying a `post-<event> closure` review. The
  finding is correct *given* that FRAMEWORK 14.4 lists EXECUTED among the terminal
  lifecycles. The ruling: either align the value to `'null'`, OR narrow the
  terminal set so an EXECUTED report may schedule a post-closure review. This tests
  the terminal-set definition landed in D1.
- **#6** — a `'TBD — after <event>'` form that no sanctioned sentinel covers. The
  sentinel set (FRAMEWORK 14.4) is CLOSED; the ruling: sanction a `'TBD — <event>'`
  form via an amendment, OR align the value to `'null'` / a `post-<event> closure`.

The other four (#1, #2, #4, #5, #7) are mechanical corpus edits.

---

## 3. The rationale-ratio monitor — FRAMEWORK 10 #5 falsification instrument

> **G-RATIO: `special_case_rationale` overrides = 94 / 285 = 33.0%.**
> The FRAMEWORK section-10 falsification #5 threshold is **20%**. This reading is
> **over threshold** and is the first real measurement of that instrument.

FRAMEWORK 10 #5 states the framework is falsified if "real workflows require
transitions that the allowed-combinations matrix forbids, accumulating
`special_case_rationale` overrides into 20%+ of register entries (signaling the
matrix is wrong, not just edge-case-tolerant)." A third of the corpus carries an
override. **Class: architect-ruling** — a standing question for the operator: is
the §3.4 allowed-combinations matrix too strict for the real corpus (amend the
matrix), or is the override rate genuinely warranted (and #5's threshold itself
needs re-examination)? This is out of Cascade B's mechanical scope; it is a
framework-level deliberation the measure has surfaced, not resolved.

---

## 4. Orphan triage (the ratified R4.2 verdicts)

The migration excluded 27 in-scope orphans on the scratch copy so `sync` could
isolate the 288 round-trip (25 pre-existing orphans + this cascade's brief + recon,
which enroll at C8). The 25 pre-existing orphans carry the ratified triage:

| Verdict | Count | Members | Cascade-B action |
|---|---|---|---|
| **enroll** (MODULE.md, category F / tier 4) | 12 | the `src/DualFrontier.Runtime/**/MODULE.md` set + `tests/DualFrontier.Runtime.Tests/Vulkan/MODULE.md` | author F/4 frontmatter |
| **enroll** (UNCLEAR — category/tier TBD) | 5 | 4 `tools/briefs/*` (BRIEF_SKELETON_FRAMEWORK, K10_3_EXECUTION, K10_AMENDMENTS_APPLICATION, K8_5_DEFERRAL_CASCADE) + `docs/prompts/PHASE_BETA_PREP_EXECUTION_PROMPT.md` | **architect-ruling** on category/tier, then enroll |
| **exclude** | 8 | 2 `tools/DualFrontier.Analyzers/AnalyzerReleases.*.md` + 6 `docs/scratch/**` (4 HALT_REPORT + 2 manual-verification protocols) | add to `SCOPE_EXCLUSIONS.yaml` |

---

## 5. Non-.md enrolled artifacts (architect-ruling)

Two enrolled Category-F entries point at `.cs` source files and cannot carry YAML
frontmatter under the inversion:

- `DOC-F-SRC-LAUNCHER-PROCEDURAL-ATLAS` → `src/DualFrontier.Launcher/LauncherProceduralAtlas.cs`
- `DOC-F-SRC-LAUNCHER-SCENE-STATE` → `src/DualFrontier.Launcher/SceneState.cs`

Cascade A carries them verbatim into the provisional `REGISTER_SUPPLEMENT.yaml`
(merged by `sync`) so the round-trip loses nothing. **Class: architect-ruling** —
Cascade B decides: exclude them (they are source, not documents); relocate their
enrollment into the Launcher `MODULE.md` frontmatter; or formalize the supplement
as a standing non-.md-artifact SoT file. (`DOC-G-REGISTER` itself is not in this
list — it is regenerated as the deterministic self-entry.)

---

## 6. Cascade B sizing (the work order)

| Work class | Items | Notes |
|---|---|---|
| **align-edit** | ~5 | 5 report-only gate fixes (#1,#2,#4,#5,#7): 4 `next_review_due` corrections + 1 `superseded_by` pointer |
| **enroll** | 12 (+5) | 12 MODULE.md as F/4; 5 UNCLEAR after the category/tier ruling |
| **exclude** | 8 | 2 AnalyzerReleases + 6 scratch → `SCOPE_EXCLUSIONS.yaml` |
| **architect-ruling** | 5 classes | (a) 2 `.cs` non-.md artifacts; (b) the 5 UNCLEAR category/tier; (c) finding #3 EXECUTED-terminal; (d) finding #6 `'TBD —'` sentinel; (e) the 33% override-matrix question (§10 #5) |
| **arm** | 1 | flip `SemanticGatesEnforcing` after align; prove each armed gate red-once-then-green on the live corpus |
| **retire** | 2 | `sync_register.ps1` + `render_register.ps1` (the forward-regime writers); F-2 dissolution |

**Cascade B is bounded**: the mechanical surface (align-edit + enroll + exclude) is
~25 document touches; the architect-ruling surface is 5 discrete decisions; then
arm + retire. The 33% override reading (#5) is the one item that may spill into a
separate framework deliberation rather than Cascade B's alignment.

---

**End of REGISTER_INVERSION_A_MEASURE_REPORT.md — 2026-07-15. Cascade A (the instrument) CLOSED at C6; this report is Cascade B's (F-34) work order.**
