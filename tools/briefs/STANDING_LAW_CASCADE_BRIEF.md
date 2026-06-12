---
register_id: DOC-D-STANDING_LAW_CASCADE_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: Draft (→ LOCKED on Crystalka ratification → EXECUTED at cascade closure)
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-06-11'
content_language: en
authored_by: Claude Opus (deliberation session, Standing-Law Cascade prep)
basis: STANDING-LAW RECON REPORT 2026-06-11 (R1–R8)
---

# STANDING-LAW CASCADE — Execution Brief

Single-pass execution. Executor: **Claude Code, Fable 5, LOCAL session on Skarlet** (Windows 11, PowerShell 5.1 only — no pwsh). Multi-agent topology per §4. Repository: `D:\Colony_Simulator\Colony_Simulator`.

---

## §1 Mission

Convert the project's methodology layer from drifted v1.0 sketches into **standing law**: documents that briefs cite instead of restating, that carry zero roadmap load, and whose every normative claim names an on-disk enforcer. Four rewrites, one new document, one METHODOLOGY amendment, one findings ledger — adapted from the proven News Intelligence Hub governance corpus (read-only reference) onto Dual Frontier's real state.

Deliverables:

| # | Artifact | Action | Version |
|---|---|---|---|
| D1 | `docs/methodology/RESERVED_SURFACE_MUTABILITY.md` | NEW | 1.0 (Category B, Tier 2, LOCKED) |
| D2 | `docs/methodology/CODING_STANDARDS.md` | rewrite | 1.0 → 2.0.0 |
| D3 | `docs/methodology/TESTING_STRATEGY.md` | rewrite | 1.0 → 2.0.0 |
| D4 | `docs/methodology/DEVELOPMENT_HYGIENE.md` | rewrite | 1.0 → 2.0.0 |
| D5 | `docs/architecture/ANALYZER_RULES.md` | restructure (roadmap-load extraction + stub-truth) | 0.1 → 0.2.0 |
| D6 | `docs/ROADMAP.md` | extend (Analyzer track section + Findings ledger F-series) | Live doc |
| D7 | `docs/methodology/METHODOLOGY.md` | amendment | 1.12.1 → 1.13.0 |
| D8 | REGISTER cascade + validation + render regeneration | governance closure | register 2.15 → 2.16 |

Why this precedes A'.9.1 Phase β: Phase β triage (fix / suppress / refine across analyzer violations) requires the suppression law (D2 §marker registry) and the mutability license (D1) to exist, or it reproduces Phase α's halt traffic at larger scale.

---

## §2 Inputs and authorities

### 2.1 Established facts (from RECON 2026-06-11 — re-verify in Phase 0, halt on mismatch)

- `main` HEAD = `f94bb84` ("Add Phase β-prep execution prompt"); zero commits after it.
- Drift branch `claude/doc-drift-reconnaissance-503aH` HEAD = `954f590`; merge-base = `f94bb84` → **fast-forward merge is possible and is the only sanctioned merge form**.
- Working tree clean; both branches fully pushed to origin.
- REGISTER: main 2.10 / drift 2.15; 270 documents, 38 EVT on drift; 124 `PENDING-COMMIT` placeholders (systemic, see F-2).
- `pwsh` ABSENT; Windows PowerShell **5.1** present; `powershell-yaml` 0.4.12 installed.
- `sync_register.ps1 -Validate` **writes** `docs/governance/VALIDATION_REPORT.md` unconditionally (timestamped, tracked). Every validate run therefore belongs inside a commit (§10 protocol).
- `REGISTER_RENDER.md` self-declares register v2.0 — 15 versions stale.
- Target docs (both sides identical for D2–D4; all at `docs/methodology/`): CODING_STANDARDS 1.0/LOCKED/265 ln; TESTING_STRATEGY 1.0/LOCKED/215 ln; DEVELOPMENT_HYGIENE 1.0/LOCKED/185 ln — all untouched since 2026-05-12, pre-dating Godot removal closure, A'.9.1, and the analyzer substrate. ANALYZER_RULES 0.1/AUTHORED-SKELETON/347 ln. METHODOLOGY 1.12.1/LOCKED/1078 ln (body changelog tops at 1.12 — F-3; highest lesson #N17; #N15/#N16 absent — F-7).
- `docs/ROADMAP.md` EXISTS on main (DOC-C-ROADMAP, Tier 2, Live, 634 ln; 862 ln on drift after `61ee43e` added «Native foundation tracks»). It is the canonical relocation target — not a new artifact.
- KERNEL_ARCHITECTURE: 2.5.3 main / 2.6.0 drift; Part 0 carries 21 invariant identifiers; **doc-stated composition "21 final" excludes К-L6 (SUPERSEDED), includes prose-only К-L3.1**; table-row IDs are Latin "K-L", prose Cyrillic «К-L» (F-4 — any grep must match both scripts).
- Analyzer state on main: 17 stubs (Architecture 9: DFK003, DFK003_1, DFK004, DFK005, DFK007, DFK011, DFK013, DFK016, DFK017; Discipline 3: DF999, DFL025_A, DFL025_B; NativeBoundary 5: DFK001, DFK002, DFK007_1, DFK015_1, DFK019_A); AnalyzerReleases.Shipped/Unshipped present; `src/Directory.Build.props` wires the analyzer to all **12** src projects (AI, Application, Components, Contracts, Core, Core.Interop, Crypto.Future, Events, Launcher, Persistence, Runtime, Systems); `tests/DualFrontier.Analyzers.Tests` = placeholder (1 test file).
- Marker census (drift tip, `src/`): `ReservedStub` 78 occurrences / 15 files **including the attribute definition file (×8)**; `throw new InvalidOperationException` 165/57 (dominated by legitimate guard clauses — ManifestParser ×32 etc.; NOT a stub signal); `NotSupportedException` 1; doc-tag `stub` 18, `deferred` 77, `TODO` 19, `Phase 6` 12, `not yet` 6; `#pragma warning disable DFK|DFL` **0 in code** (4 documentation-text occurrences only).
- `project.godot` still tracked (F-5). A'.9.1 Phase β-prep staged on main, unexecuted.

### 2.2 Reference corpus (READ-ONLY, FROZEN — never modified, never built, never linted)

`D:\Colony_Simulator\News Intelligence Hub\docs\`:

| Reference | Lines | Consumed by |
|---|---|---|
| `governance/PHASE_0_SKELETON_MUTABILITY.md` | 69 | W3 (D1 adaptation source) |
| `methodology/CODING_STANDARDS.md` | 647 | W1 |
| `methodology/TESTING_STRATEGY.md` | 1853 | W2 (focus §8 Brief integration pattern, §9 marker/invariant audits) |
| `governance/FRAMEWORK.md` | 453 | Orchestrator + W4 (citation-form §9.2; session closure §11 → D7) |
| `docs/ROADMAP.md` | 447 | Orchestrator (findings-ledger §5 shape → D6) |

Adaptation law: transfer **mechanisms**, not stack specifics. NestJS/Vitest/ESLint content does not migrate; the structural pattern under it does. Where NIH and DF practice conflict, DF empirical practice wins and the pattern adapts.

### 2.3 DF inputs (read in Phase 0 / by assigned agents)

- `docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md` — ANALYZER_RULES findings + DD-1 set context (W4, orchestrator).
- `docs/governance/REGISTER.yaml` — schema enum vocabulary read EMPIRICALLY before any mutation template is written (Lesson #N14).
- `docs/governance/PROJECT_AXIOMS.md`, `docs/methodology/METHODOLOGY.md`, `docs/architecture/KERNEL_ARCHITECTURE.md` Part 0 — immutable-catalog sources for D1.
- The four target docs (current text), `tools/briefs/A_PRIME_9_1_ANALYZER_INFRASTRUCTURE_BRIEF.md` (suppression-law context, Q-L decisions).

---

## §3 Phase 0 — Preconditions, merge, validation checkpoint

Run serially by the orchestrator before any agent spawns.

1. **Verify recon facts**: current branch, both HEADs, clean tree, zero commits `f94bb84..main`. Any mismatch → **HALT H1**.
2. **Merge**: `git checkout main` → `git merge --ff-only claude/doc-drift-reconnaissance-503aH`. If ff-only fails → **HALT H2**. (No merge commit exists after this — main HEAD becomes `954f590`.)
3. **Validation checkpoint**: run

   ```
   powershell -NoProfile -ExecutionPolicy Bypass -File tools\governance\sync_register.ps1 -Validate
   ```

   Exit code ≠ 0 → **HALT H3** (report validator output verbatim; do not attempt repair without ratification). Exit 0 → the rewritten `VALIDATION_REPORT.md` is a tracked change; it lands inside commit **C1** (§10).
4. **Schema vocabulary read** (Lesson #N14): extract from REGISTER.yaml the empirically-used enum values for `category`, `tier`, `lifecycle`, `review_cadence`, and the exact shape of a `DOC-` entry and an `EVT-` entry. These verbatim shapes are the only sanctioned templates for D8.
5. **Mandatory reads**: §2.3 list. Confirm in the session log that each was read before Wave R spawns.

NEVER run `sync_register.ps1 -Sync`. `render_register.ps1` runs exactly once, at C10. Executor NEVER pushes to origin — all pushes are Crystalka's manual step after closure.

---

## §4 Session topology

```
Orchestrator (this session, Fable 5)
 ├─ Phase 0 (serial, §3)
 ├─ Wave R — 4 read-only survey agents, parallel (§5)   [zero writes, zero commits]
 ├─ Checkpoint C-R (serial, §6)
 ├─ Wave W — 4 writer agents, parallel, disjoint files (§7)   [write drafts ONLY; zero commits]
 ├─ Checkpoint C-W (serial, §6)
 └─ Serial closure (orchestrator only): commits C1–C10 (§10), REGISTER cascade (§11), closure report (§14)
```

Hard rules: **only the orchestrator runs `git add`/`git commit`** — atomic-commit discipline is incompatible with parallel committers. Writer agents write target files in place (working tree) but never stage. File ownership is disjoint per §7; any agent needing to touch a file outside its grant → escalate to orchestrator, do not improvise. No agent touches the News Intelligence Hub tree, ever (**H8**).

---

## §5 Wave R — survey agents (read-only)

Each agent returns a structured inventory to the orchestrator (chat/scratch output, fixed schema below). Inventories are the **code-truth substrate** for Wave W: a writer may not state an enforcement claim absent from its inventory.

### R1 — Enforcement reality for CODING_STANDARDS

Survey: root `Directory.Build.props` + `src/Directory.Build.props` (all `<PropertyGroup>` switches: Nullable, LangVersion, TreatWarningsAsErrors, ImplicitUsings, analyzers config); `.editorconfig` (exists? operative rules?); `Directory.Packages.props` (CPM state, package list); any CI workflow files (`.github/workflows/`, expected: verify presence/absence — do not assume); C# style as practiced (sample 10 representative files across Core/Systems/Contracts: naming, file-scoped namespaces, nullability annotations, XML-doc citation habits — does code cite К-L/PA/Q IDs?); C++ native kernel style as practiced (sample 5 files: naming, headers, error propagation across the interop boundary); existing commit-message practice (sample `git log --format=full -20`: scope prefixes in use, body structure, Co-Authored-By presence).

Schema: `R1-INV: {msbuild_switches[], editorconfig: present/absent+rules, cpm: state, ci: present/absent, cs_style_observed[], cpp_style_observed[], xmldoc_citation_practice, commit_practice{scopes_seen[], body_sections_seen[], trailers_seen[]}}`.

### R2 — Test landscape for TESTING_STRATEGY

Survey: every test project repo-wide (`**/*Tests*.csproj`, `tests/` tree) — name, framework (xunit/nunit/mstest?), test-file count, real-vs-placeholder; any native-side tests; how tests are run today (solution-level `dotnet test`? per-project?); any existing meta-/census-style tests (grep-based asserts); KERNEL/brief-declared S-LOCKs that claim test coverage — does the claimed test exist? (List claim → artifact → VERDICT exists/missing.)

Schema: `R2-INV: {test_projects[{name, framework, files, real_tests}], run_command_truth, meta_tests_existing[], slock_coverage_audit[{claim, artifact, verdict}]}`.

### R3 — Operational truth for DEVELOPMENT_HYGIENE

Survey: full project set (12 src + tools + tests + anything else solution-bound; the `.sln` contents vs disk truth); build commands that actually work (record exact invocations: managed build, native kernel build — CMake? MSBuild vcxproj? —, full solution); scripts inventory (`tools/` tree: every script, its language, its side effects — explicitly re-confirm sync_register.ps1's `-Validate` write at line ~380 and render_register.ps1's outputs); PowerShell 5.1 invocation forms that work; repo layout map (top-2-level); branch census (44 stale locals from recon — list candidates for future pruning, do not prune); `project.godot` references repo-wide.

Schema: `R3-INV: {sln_truth[], build_commands_verified[], scripts[{path, lang, side_effects}], ps51_invocation_forms[], layout_map, stale_branches[], godot_remnants[]}`.

### R4 — Reserved-surface census (the pin source)

Measure precisely, on post-merge main:

1. **Canonical census**: `[ReservedStub` attribute **application sites** in `src/` — EXCLUDING the attribute definition file (`Contracts/Analyzer/ReservedStubAttribute.cs`). Report: exact count, per-file breakdown, and the composition rule as one sentence (this becomes the D3 census pin).
2. Doc-tag families in `src/` XML-docs: per-pattern exact counts (`stub`, `deferred`, `TODO`, `Phase 6`, `not yet`) with the grep expressions used (these become the registered family definitions in D2 — the patterns are recorded verbatim so future censuses are reproducible).
3. `#pragma warning disable DFK|DFL` in code: expected 0 — verify (the DFK-WAIVER law starts from a clean baseline).
4. Cross-check vs recon R5 numbers; explain every delta (merge added nothing to src/, so deltas indicate measurement-method differences — resolve them and record the canonical method).

Schema: `R4-INV: {census_pin{count, rule_sentence, per_file[]}, doc_tag_families[{name, pattern_verbatim, count}], waiver_baseline: 0_confirmed, deltas_explained[]}`.

---

## §6 Checkpoints

**C-R (after Wave R):** orchestrator reconciles R1–R4 against §2.1. Material contradiction with recon (beyond explained measurement-method deltas) → **HALT H4**. Compile the consolidated inventory and hand the relevant slices to each writer.

**C-W (after Wave W):** orchestrator reviews all drafts against:
1. **Truth law (§8)** — sample-audit every enforcement claim against R-inventories;
2. **Zero roadmap load** — no future-tense normative content outside explicit `Planned — see ROADMAP.md §…` pointers;
3. Cross-citation integrity (D2↔D1, D5→D2, D3→D1/D2 anchors resolve);
4. NIH stack residue (no NestJS/Vitest/ESLint/pnpm vocabulary surviving as DF law).

Violations → return to the owning writer with a defect list (one iteration); unresolvable without an architectural decision → **HALT H6**.

---

## §7 Wave W — writer specifications

Global laws for all writers: §8 (truth), §9 (mutability license). Each writer reads: its NIH reference (§2.2), the current DF doc it replaces, its R-inventory slice, and this brief's section. Frontmatter on every produced doc follows the REGISTER mirror shape captured in Phase 0 step 4. All docs English. Each rewrite ends with an **Amendment protocol** section (adapted from NIH CODING_STANDARDS §Amendment protocol: surface→rationale→semver→register update→cross-doc propagation) and a **Change history** opening at its new version.

### W1 — D2 `CODING_STANDARDS.md` v2.0.0 (target ≈ 450–650 lines)

Section plan:

1. **Authority & scope.** Governs: C# managed layer, C++20 native kernel, tooling scripts, commit discipline. Does not govern: architecture (KERNEL/ANALYZER_RULES et al.), brief content (METHODOLOGY), test methodology (TESTING_STRATEGY — cite, don't restate). State the stop-escalate-lock rule for unanswered style questions (amend this doc, don't improvise).
2. **C# conventions** — from R1 observed practice, codified (naming, file-scoped namespaces, nullability, member order if a consistent order is observed; do NOT import Google-TS or invent rules the codebase doesn't follow — codify the real norm, flag observed inconsistencies as a short non-normative note).
3. **C++ native kernel conventions** — from R1 observed practice; include the interop-boundary error-propagation pattern actually used.
4. **Project & build organization** — 12 src projects, Directory.Build.props chain (quote the real analyzer-wiring block), CPM per Directory.Packages.props truth.
5. **Marker family registry** (the NIH graft, DF-instantiated). For each family: form, semantics, introduction rule, closure semantics, census method (verbatim grep from R4):
   - `[ReservedStub]` attribute — structurally-present, functionally-inert surface; closure = per-feature realization commits; census = R4 canonical pin (cite TESTING_STRATEGY §census for the meta-test).
   - Doc-tag families (`deferred`, `TODO`, `Phase 6`, `not yet`, `stub` in XML-docs) — registered AS-IS with R4 counts as the baseline; forward rule: new deferral markers name their closing phase. **No retroactive obligation is imposed on the 100+ existing sites** — they are baseline-registered, normalized opportunistically.
   - **`DFK-WAIVER` — the suppression law (NEW, load-bearing for Phase β).** Form:

     ```csharp
     // DFK-WAIVER(DFK013): <one-line reason citing authority — Q-L-#, К-L#, F-#, or "false positive pending rule refinement, see ROADMAP Analyzer track">
     #pragma warning disable DFK013
     ...minimal scope...
     #pragma warning restore DFK013
     ```

     Rules: (a) waiver comment immediately precedes the disable; (b) narrowest possible scope, always paired with restore; (c) allowed reason classes — false-positive-pending-refinement (must reference the refinement entry), sanctioned-architectural-exception (must cite the locked decision ID), generated/interop-mandated code; (d) file-scope or project-scope `<NoWarn>` for DFK/DFL is FORBIDDEN; (e) every waiver is census-tracked (TESTING_STRATEGY meta-test) and baseline = 0 as of this cascade; (f) a waiver without a resolvable authority citation fails review.
6. **Comments & documentation language.** English everywhere in artifacts; comments explain WHY and cite internal law by ID (К-L#, PA-00#, Q-L-#, F-#, doc §) — adapt NIH's "authority chain stays unbroken" rule; no narration-of-what.
7. **Exception & error discipline.** Codify the observed practice (guard-clause `InvalidOperationException` with diagnostic messages — ManifestParser as exemplar); when native error codes cross interop, the managed wrapper pattern per R1. Distinguish guard throws from reserved-stub throws (the latter carry `[ReservedStub]` — this distinction is what makes the census clean).
8. **Atomic commit discipline.** DF-adapted 7-section body: `Authority / Scope / Rationale / Scope boundaries / Skeleton revisions (when applicable, per RESERVED_SURFACE_MUTABILITY) / Verification / References` + `Co-Authored-By` trailer. Scope-prefix subject format from R1's observed scopes (codify the real list; new scopes via governance commit). Hard prohibitions: force-push, history rewrite on pushed branches, squash. **DF-specific rationale: in a falsifiable research framework the commit history IS the dataset — defect rate, pipeline economics, and architectural-integrity claims are measured from it; history integrity is research-data integrity.** Executor-never-pushes-main rule (pushes are the architect's act).
9. **Verification gates.** Only gates that exist per R1/R3 (managed build, native build, validate script with its VALIDATION_REPORT side-effect noted, analyzer baseline). CI: state truthfully per R1 (if absent: one `Planned — see ROADMAP.md` pointer, nothing more).
10. Amendment protocol + Change history (v2.0.0 entry: full rewrite to code-truth + tournament-pattern adaptation, MAJOR; cite this brief).

### W2 — D3 `TESTING_STRATEGY.md` v2.0.0 (target ≈ 400–550 lines)

Section plan:

1. **Authority & why** (test law; briefs cite, never restate).
2. **Test landscape truth** — per R2: what exists today, named honestly (placeholder suites called placeholders).
3. **Layer taxonomy, DF-adapted**: managed unit / analyzer tests (Roslyn `Microsoft.CodeAnalysis.Testing` pattern — the standing convention Phase β instantiates; present-tense only for what exists, the convention itself is law, its population is Phase β scope) / native tests (per R2 truth) / integration / **meta** (repo-discipline tests).
4. **Meta-test & census patterns** (the §9-graft, core of this rewrite):
   - **Reserved-surface census pin**: the R4 canonical count, composition rule verbatim, and the meta-test contract — census asserts the EXACT pin; any commit changing the count updates the pin in the same commit with a `Skeleton revisions`/census-delta record. (Monotonicity is NOT asserted — DF stubs close per-feature and may legitimately grow with new reserved surface; exactness, not direction, is the invariant.)
   - **Marker-family censuses** for each D2-registered family (pattern verbatim, baseline count).
   - **DFK-WAIVER census**: baseline 0; every increase requires the D2 §5 authority citation.
   - **S-LOCK → meta-test obligation**: every S-LOCK declared by an active brief names its verifying artifact (test, grep-gate, or build property); R2's slock_coverage_audit gaps become F-ledger entries, not silent debt.
5. **Validation-criteria mapping**: one testable criterion → one named test; К-L invariants map where machine-checkable (analyzer rule, meta-test) and are explicitly listed as architect-audited where not. No fake coverage claims.
6. **Brief integration pattern** (the §8-graft, near-verbatim mechanism): what a brief carries (named test list in `it/Fact` form, layer assignment, coverage anchors, 1–2 representative bodies, count delta) vs what it cites (isolation patterns, harness construction, mocking/waiver discipline — by section anchor). **Boundary rule**: reusable-across-phases → this document; phase-specific → brief; doubt → refactor into this document. **Anti-pattern rule**: a brief contradicting this document is wrong by default — either the brief is corrected or this document is amended BEFORE the brief locks.
7. **Closure-audit obligation**: every cascade's closure includes census table (all pins), S-LOCK coverage check, waiver count.
8. Amendment protocol + Change history.

### W3 — D1 `RESERVED_SURFACE_MUTABILITY.md` v1.0 (NEW, ≈ 150–220 lines) + D4 `DEVELOPMENT_HYGIENE.md` v2.0.0 (≈ 250–350 lines)

**D1** — adaptation of NIH PHASE_0_SKELETON_MUTABILITY onto DF:

1. **Why** — Phase α produced 5 mid-cascade deltas; ≥3 were surface-naming class requiring halts; briefs compensate with defensive literal transcription (the Lesson #N18 bloat root). This document pre-authorizes the trivial class so halts remain for genuinely architectural forks and briefs may name intended forms.
2. **The principle**: a property is **mutable** iff changing it changes no answer in any Tier-1 LOCKED authority (KERNEL Part 0, PROJECT_AXIOMS, locked Q-series, S-LOCKs, METHODOLOGY law). Mutable surface may be revised by any cascade commit with a commit-body record; immutable structure may not.
3. **Mutable catalog** (DF-instantiated, explicitly non-exhaustive): symbol names of reserved stubs and analyzer rule classes; file/directory layout below project level (project-to-layer assignment NOT mutable); annotation surface forms (attribute argument shapes, doc-tag wording); csproj wiring lists in briefs (the on-disk set is truth; brief lists are intent); grep patterns and census expressions (the canonical method lives in TESTING_STRATEGY and updates by census-delta record); doc section numbering in citations (cite by anchor+topic, renumber freely).
4. **Immutable catalog**: the 21 К-L invariants **per KERNEL Part 0's own stated composition ("21 final": excludes К-L6 SUPERSEDED, includes К-L3.1)** — pin the composition sentence to neutralize F-4/F-9 ambiguity; PA-001..004; all locked Q-series decisions; declared S-LOCKs; `register_id` permanence; lifecycle state machine; atomic-commit discipline; REGISTER-derived governance state; the additive-semver Mod API law (К-L20).
5. **Commit-body form**: `Skeleton revisions:` section (placement per D2 §8), entries `<from> → <to> (one-line rationale)`; one conceptual change = one entry regardless of site count.
6. **Retrospective note**: Phase α deltas 1–5 are retroactively classified (which were mutable-surface vs genuinely architectural — classify honestly; CPM-count and commit-7-scope were surface; ProjectReference-drop was architectural and correctly halted).
7. **Out of scope**: not a license for architectural change in disguise; anything not cataloged mutable is immutable-or-adjudicate.

**D4** — operational truth, post-Godot:

1. Repository map (R3 layout, top-2-level, annotated).
2. Project set truth (sln vs disk; 12 src + tools + tests).
3. Build commands — only R3-verified invocations, exact strings (managed, native, solution).
4. **Tooling reality**: PowerShell 5.1 (NOT pwsh) with working invocation forms; `powershell-yaml` dependency; **sync_register.ps1 semantics including the `-Validate` VALIDATION_REPORT.md write side-effect and the resulting commit-folding protocol**; render_register.ps1 usage and the staleness discipline; the `-Sync` prohibition outside ratified register cascades.
5. Branch & push policy: executor never pushes main; ff-only merges for linear cascades; stale-branch census from R3 listed as F-ledger pruning candidate (no pruning in this cascade).
6. Environment notes (Skarlet specifics relevant to builds; Windows paths vs MCP access patterns).
7. Godot status: fully deprecated (cascade #2), historical docs quarantined; `project.godot` fate = F-5 (Crystalka-owned, not touched here).
8. Amendment protocol + Change history.

### W4 — D5 `ANALYZER_RULES.md` v0.2.0 restructure + D6 ROADMAP «Analyzer track» section

Inputs: current D5 text, drift report's ANALYZER_RULES findings (bidirectional dual-load specimen), recon R4 stub truth, A'.9.1 brief Q-L decisions.

1. **Extract roadmap load**: relocate the future-work sections identified by the drift report (the §5/§6/§10.5/§11-class content: rule pipeline plans, deferred-rule family DF009/DF012/DF015/DF018/DF020 → К-L20 LOCK cascade, DFK016 Option γ assessment, PublicApiAnalyzers deferral, phase plans) into `docs/ROADMAP.md` under a new **«Analyzer track»** section, structured consistently with the existing «Native foundation tracks» section (match the live doc's idiom). Leave one-line `Planned — see ROADMAP.md §Analyzer track` pointers at each extraction site.
2. **§4 to stub-truth**: the rule inventory states exactly the 17 stubs by ID/category/title with `status: stub — detection logic lands at Phase β cleanup-phase`. No "is checked/enforced" phrasing anywhere until detection exists. S-LOCK-4 phrasing reconciled to the on-disk 17 (16 + conditional DFK016 → state the actual composition).
3. **Dangling reference resolution**: `MOD_API_CONTRACT` reference → re-point to the ROADMAP deferral entry (Mod-OS-coupled rules, К-L20 LOCK cascade) — the target document does not exist and must not be phantom-cited.
4. **Suppression**: add a short normative section that DEFERS to D2's DFK-WAIVER law by anchor citation (ANALYZER_RULES does not own the marker law — CODING_STANDARDS does).
5. Lifecycle stays AUTHORED-SKELETON (Phase β will mutate §4 again); version 0.2.0; change-history entry.

---

## §8 Truth law (binding on every writer)

**Existence test**: every normative claim of enforcement, automation, or coverage must name the on-disk artifact that realizes it (file path, script, MSBuild property, test). If the artifact does not exist, the claim is not law — it is roadmap, and it moves to `docs/ROADMAP.md` with a pointer. Forbidden verb forms without an existing enforcer: "is checked", "is enforced", "CI rejects", "the analyzer reports", "tests verify". This is the precise failure mode the drift audit caught (ANALYZER_RULES §4 overclaim); this cascade must not reproduce it in the act of curing it.

Corollary: writers state present-tense truth from R-inventories, convention-law in normative mood ("new code does X"), and future capability only inside ROADMAP pointers.

---

## §9 Mutability pre-authorization (inline license until D1 lands)

Until commit C2 lands D1, this brief grants the identical license: names, paths, section numbers, and counts stated in this brief are **intended forms**; the executor verifies empirically and adapts, recording every deviation as a `Skeleton revisions` commit-body entry. Halt is reserved for deviations that would change an answer in a Tier-1 authority (§7-W3-4 immutable catalog). After C2, D1 itself is the authority.

Known intended-vs-actual risks pre-cleared: exact doc paths (verify via `git ls-files`), ROADMAP section naming/idiom (match the live doc), REGISTER enum vocabulary (Phase 0 step 4 governs), census numbers (R4 governs over recon).

---

## §10 Commit plan (orchestrator only, serial, ~10 atomic commits)

Every commit: D2 §8 body structure (Authority cites this brief §-anchors + standing docs; Verification lists the gates actually run). Validate-gate protocol: when a commit's verification includes the validate run, the refreshed `VALIDATION_REPORT.md` is staged INTO that same commit.

| # | Subject | Content |
|---|---|---|
| C1 | `governance(standing-law): enroll Standing-Law Cascade brief + post-merge validation checkpoint` | `tools/briefs/STANDING_LAW_CASCADE_BRIEF.md` (this file) + VALIDATION_REPORT.md from Phase 0 step 3. (Merge itself is ff — no commit.) |
| C2 | `docs(methodology): author RESERVED_SURFACE_MUTABILITY v1.0` | D1 |
| C3 | `docs(methodology): rewrite CODING_STANDARDS to v2.0.0 — code-truth + marker registry + DFK-WAIVER law` | D2 |
| C4 | `docs(methodology): rewrite TESTING_STRATEGY to v2.0.0 — census pins + brief integration pattern` | D3 |
| C5 | `docs(methodology): rewrite DEVELOPMENT_HYGIENE to v2.0.0 — post-Godot operational truth` | D4 |
| C6 | `docs(analyzer): restructure ANALYZER_RULES to v0.2.0 — roadmap extraction + stub-truth` | D5 + D6 «Analyzer track» (one cohesive move: content leaves one file and lands in the other in the same commit) |
| C7 | `governance(methodology): amend METHODOLOGY to v1.13.0 — brief-integration rule, SYNTH-2 item, session closure protocol` | D7 (§12) |
| C8 | `governance(roadmap): seed Findings ledger F-series` | D6 ledger section (§13) |
| C9 | `governance(register): Standing-Law Cascade REGISTER closure (2.15 → 2.16)` | D8 mutations (§11) + validate run folded in (refreshed VALIDATION_REPORT.md) |
| C10 | `governance(register): regenerate REGISTER_RENDER + backfill closure commit hash` | render_register.ps1 output + Option-B backfill of C9's hash into the register header (closes the self-reference without amend; per established amendment-integrity practice) |

Commit count is intended-form: a writer-defect iteration or an extraction needing a split may add a commit — record the deviation in the closure report, do not compress history to match the table.

## §11 REGISTER cascade (C9 content)

Using ONLY the Phase 0 step 4 verbatim shapes:

- **Enroll**: DOC-D-STANDING_LAW_CASCADE_BRIEF (lifecycle LOCKED→EXECUTED at closure per register convention observed empirically), DOC-B-RESERVED_SURFACE_MUTABILITY (Tier 2, LOCKED). Documents 270 → 272.
- **Version bumps**: D2/D3/D4 → 2.0.0; D5 → 0.2.0; D7 → 1.13.0; D6 entry touched per its Live-doc convention (inspect how DOC-C-ROADMAP records updates and follow it).
- **EVT**: one audit-trail entry `EVT-STANDING_LAW-CLOSURE` (38 → 39) summarizing the cascade with C1–C8 **real hashes** (C9 lands after them — no new PENDING-COMMIT placeholders are created anywhere in this cascade except the single unavoidable header self-reference, which C10 backfills).
- `register_version` 2.15 → 2.16; `last_modified` = execution date.
- Validate exit 0 mandatory; nonzero → HALT H3 (fix only within the empirical schema vocabulary; never guess enums).

## §12 D7 — METHODOLOGY v1.13.0 amendment content

1. **New section — Brief-integration boundary rule**: briefs cite standing documents by anchor instead of restating their content; what a brief carries vs cites (mirror D3 §6 in one paragraph + cross-cite); the boundary rule (reusable → standing doc; phase-specific → brief; doubt → refactor into standing doc); the anti-pattern rule (a brief contradicting a standing doc is wrong by default — correct the brief or ratify the doc amendment BEFORE the brief locks).
2. **Cascade-impact checklist addition (SYNTH-2 item)**: append to the existing impact/closure checklist the question — *"Does this change alter behavior described in the prose of any Tier-1/standing document? If yes, that document receives its PATCH in the same cascade."* (Place it inside the existing checklist structure; if no checklist section exists, add it to the closure-protocol section being created in item 3.)
3. **New section — Session closure protocol**: every execution session's closure sequence: (a) tracker write-back (ROADMAP/brief lifecycle flips, DONE marks with hashes); (b) REGISTER mutations + validate (report folded into the commit); (c) render regeneration or an explicit recorded staleness deferral; (d) findings → F-ledger entries (never chat-only); (e) closure report per the active brief's schema. Propagation is a protocol step, not a later audit.
4. **Housekeeping**: backfill the missing v1.12.1 changelog line (F-3) — one sentence, dated from register data; add the v1.13.0 changelog entry (MINOR: two new sections + one checklist item; no existing rule inverted).
5. **NOT included**: Lesson #N18 formalization (timing-locked to A'.9.1 Phase δ); lesson numbering gap N15/N16 (F-7, architect-owned).

## §13 D6 — Findings ledger seed (C8)

New ROADMAP section **«Findings ledger (F-series)»** — shape adapted from NIH ROADMAP §5: `F-# | finding | severity (S0–S3/N) | state (OPEN/CLOSED/ACCEPTED-NO-ACTION) | resolution/owner`. Seed (states as of C8; C9/C10 close F-1 in-cascade — record the closing hash at closure):

- **F-1** REGISTER_RENDER 15 versions stale → CLOSED by C10.
- **F-2** 124 PENDING-COMMIT placeholders in REGISTER; backfill discipline systemically open → OPEN, owner: future register-tooling cascade (hybrid reverse-register, Q-T-2).
- **F-3** METHODOLOGY 1.12.1 lacked body changelog entry → CLOSED by C7.
- **F-4** KERNEL Part 0 ID script inconsistency (Latin "K-L" rows vs Cyrillic «К-L» prose) — any census greps both scripts → OPEN, owner: KERNEL amendment (architect).
- **F-5** `project.godot` tracked post-deprecation → OPEN, owner: Crystalka.
- **F-6** Tooling reality: pwsh absent, PS 5.1 + powershell-yaml is the actual substrate; `-Validate` writes VALIDATION_REPORT.md → CLOSED by C5 (documented in D4).
- **F-7** Lesson numbering gap #N15/#N16 → OPEN, owner: architect (assign or declare intentional).
- **F-8** S-LOCK coverage gaps from R2's audit (if any) → enroll each as its own F-entry, OPEN.
- **F-9** Part 0 counting-convention divergence (table composition vs doc-stated "21 final") → mitigated by D1 §4 composition pin; full reconciliation OPEN, owner: KERNEL amendment (merge with F-4 if the architect prefers).

## §14 Closure protocol & report

Execute D7 §3's protocol as authored (eat the dogfood): tracker write-back (this brief's register entry → EXECUTED; ROADMAP Analyzer-track cross-link verified), C9 register + validate, C10 render + backfill, ledger final states.

Closure report (chat) schema: commits table (hash | subject | files); versions table (before → after for D1–D7, register 2.15→2.16); **census pins recorded** (ReservedStub pin + composition sentence, all doc-tag baselines, waiver baseline 0); F-ledger final state table; Skeleton-revisions consolidated list (every deviation from this brief's intended forms); gates table (validate exits, build checks if run); self-attestation (NIH untouched; no pushes performed; no -Sync outside C9; no history rewrites); **Crystalka manual checklist**: push main to origin; ratify D1–D7 lifecycle states; decide F-5/F-7.

## §15 Halt conditions

H1 precondition mismatch (§3.1) · H2 non-ff merge (§3.2) · H3 validate nonzero (§3.3, §11) · H4 Wave-R/recon material contradiction (§6) · H5 REGISTER enum needed that Phase 0 vocabulary lacks → escalate, never invent · H6 truth-law unsatisfiable without architectural decision (§6) · H7 census composition ambiguity R4 cannot resolve to one sentence · H8 ANY write/touch toward News Intelligence Hub — immediate halt, no exceptions. On halt: stop, report state verbatim, await Crystalka. Resolution + re-confirmation in-session before resuming (auto-mode re-confirmation is expected behavior, not a fault).

## §16 Out of scope (explicit)

A'.9.1 Phase β cleanup-phase (next cascade — this one clears its runway) · deep DD-1 substrate rewrites (ARCHITECTURE, THREADING, EVENT_BUS, VULKAN bodies) · DD-3 hygiene · doc_role schema field (rides the future register-tooling cascade) · hybrid reverse-register (Q-T-2) · К-L20 LOCK cascade · KERNEL amendments (F-4/F-9) · branch pruning · `project.godot` removal · any push to origin · anything in `D:\Colony_Simulator\News Intelligence Hub`.

---

*Authored 2026-06-11 from STANDING-LAW RECON (R1–R8). Ratification: Crystalka. Без костылей.*
