---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K_LESSONS_BATCH
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K_LESSONS_BATCH
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-K_LESSONS_BATCH
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-K_LESSONS_BATCH
---
# K-Lessons Batch — formalize 4 pipeline-execution lessons surfaced through K8.0/K8.1/K8.1.1 closures

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09 (post-K8.1.1 closure)
**Status**: EXECUTED (2026-05-09, branch `feat/k-lessons-formalization`, closure `9df2709..071ae11`). See `docs/MIGRATION_PROGRESS.md` § "K-Lessons — Pipeline closure lessons formalization" for closure record.
**Reference docs**: `docs/methodology/METHODOLOGY.md` v1.4 (target for 3 lessons + version bump to v1.5), `docs/architecture/KERNEL_ARCHITECTURE.md` v1.2 LOCKED (target for 1 lesson + version bump to v1.3), `docs/MIGRATION_PROGRESS.md` (closure tracker — K-Lessons row authored as part of this brief)
**Companion**: closure records in `MIGRATION_PROGRESS.md` for K8.0 (`9f9dc05..28498f9`), K8.1 (`a62c1f3..059f712`), K8.1.1 (`fc4400d..63777ef`)
**Methodology lineage**: this brief is itself an instance of the brief-authoring discipline (`MOD_OS_V16_AMENDMENT_CLOSURE.md`) and is target-document-only — no source code or tests are touched
**Predecessor**: K8.1.1 (`fc4400d..63777ef`) — InternedString closure follow-up EXECUTED
**Target**: fresh feature branch `feat/k-lessons-formalization` from `main` after K8.1.1 closure
**Estimated time**: 30-60 minutes auto-mode (focused documentation work, ~+200/-20 LOC, no code or tests)
**Estimated LOC delta**: ~+220/-20 (mostly METHODOLOGY.md additions; smaller KERNEL_ARCHITECTURE.md addition)

---

## Goal

Four pipeline-execution lessons surfaced through K8.0, K8.1, and K8.1.1 closures. Each lesson currently lives only in chat-session memory and closure narratives; this brief formalizes them as referenceable institutional knowledge in the appropriate authoritative document.

| # | Lesson | Source closure | Target document | Section |
|---|---|---|---|---|
| 1 | Atomic commit = compilable unit, not file-count unit | K8.1 Phase 5 (5 files in one commit due to dependency cycle) | `METHODOLOGY.md` | New sub-section under "Native layer methodology adjustments" |
| 2 | Phase 0.4 inventory as hypothesis, not authority | K8.1 (`Marshalling/` subfolder mismatch surfaced and reconciled at execution time) | `METHODOLOGY.md` | Same new sub-section |
| 3 | Mod-scope test isolation rule | K8.1.1 Stop condition #3 fix (test re-intern between scope boundaries anchored id to core scope, prevented reclaim) | `METHODOLOGY.md` | Same new sub-section |
| 4 | Error semantics convention (Interop layer four-category rule) | K8.1 vs K9 brief style difference (sparse → bool, dense → throw, lifecycle → throw, construction → handle-or-throw) | `KERNEL_ARCHITECTURE.md` | New sub-section in Part 7 (Methodology adjustments for K-series) |

The grouping is **3+1 (α) split**: three lessons concern pipeline execution discipline (METHODOLOGY-level), one concerns architectural shape of the Interop layer (KERNEL-level). The split matches Crystalka's Q2=(α) decision in chat session 2026-05-09.

K-Lessons is **a documentation milestone** — no source code, no tests, no native build. It updates two living documents and bumps their versions. It is executable in a single Cloud Code session of 30-60 minutes auto-mode.

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition (hard gate)**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K-Lessons-WIP"` and re-verify.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -25
```

**Expected**: K8.1.1 closure visible. Most recent K8.1.1 commit per closure record: `63777ef`.

**Halt condition (hard gate)**: K8.1.1 not closed, or closure SHA does not match. K-Lessons builds on the post-K8.1.1 state of MIGRATION_PROGRESS.md and the lessons sourced from K8.1.1 itself.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/methodology/METHODOLOGY.md
head -10 docs/architecture/KERNEL_ARCHITECTURE.md
```

**Expected**:
- `METHODOLOGY.md` Version: 1.4 (2026-05-07). K-Lessons bumps to v1.5.
- `KERNEL_ARCHITECTURE.md` Version: 1.2 (Status: AUTHORITATIVE LOCKED — Solution A recorded). K-Lessons bumps to v1.3.

**Halt condition (hard gate)**: either document at unexpected version. K-Lessons amends specific text known to be present at v1.4 / v1.2; an unexpected version means the text may have moved or been edited and the brief's edit instructions may not apply cleanly.

### 0.4 — Code state inventory

```
ls docs/
```

**Expected** present:
- `docs/methodology/METHODOLOGY.md`
- `docs/architecture/KERNEL_ARCHITECTURE.md`
- `docs/MIGRATION_PROGRESS.md`

This is an **informational check**, not a hard gate — listing the directory documents the working state for the audit trail. The hard gate is on the version headers in 0.3.

### 0.5 — Native and managed builds — not required

K-Lessons touches no code, no tests, no native build, no managed build. The build/test gate is **explicitly skipped** for this brief; documentation-only milestones do not exercise it.

The closure verification phase (Phase 5.3 below) reverifies the existing 592-test baseline is unchanged — not as gate but as sanity check that no documentation edit accidentally affected source paths.

### 0.6 — Brief itself committed

If this brief is on disk in the working tree but uncommitted at the start of execution, commit it first on `main`:

```
git add tools/briefs/K_LESSONS_BATCH_BRIEF.md
git commit -m "docs(briefs): author K-Lessons batch brief (4 pipeline lessons formalization)"
```

This is the first atomic commit of the milestone, performed on `main` before the feature branch is created (per the K8.1 / K8.1.1 precedent and METHODOLOGY.md "Brief authoring as prerequisite step" principle).

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the architectural foundation for K-Lessons. The executor reads this section as the design contract; decisions here are LOCKED by Crystalka's commitment («сложная архитектура без костылей») and the Q2=(α) decision recorded in chat session 2026-05-09.

### 1.1 — Lesson formalization shape (LOCKED)

Each lesson is formalized as a markdown sub-section following the **established failure-mode pattern** of the existing "Native layer methodology adjustments" entries (Pre-flight checks, ABI boundary exception completeness, Brief authoring as prerequisite step, Calibrated time estimates). The pattern is:

1. **Sub-section heading** — verb-noun phrasing, parallel to existing entries.
2. **One-paragraph framing** — what the lesson concerns, why it matters now (a sentence or two).
3. **Failure mode (observed at <closure SHA range>)** — concrete narrative of what went wrong, including the closure milestone, the deviation, and how it was caught or worked around. Include the failing pattern in code or commit-shape form when relevant.
4. **Principle** — one-sentence rule, bolded.
5. **Rationale** — 1-3 paragraphs of why the rule holds, what it prevents structurally, and the architectural cost of departing from it.
6. **Brief authoring requirement (mandatory checklist item)** — markdown checklist of how brief authors apply the rule going forward.
7. **Falsifiable claim** — what the lesson predicts and what would falsify it.

This shape is non-negotiable per Phase 1 LOCKED. It matches the existing METHODOLOGY.md "Native layer methodology adjustments" entries verbatim and integrates the new lessons without stylistic drift. Lessons that depart from the shape (e.g., omit the falsifiable claim, lack the failure-mode narrative) reduce to opinion notes and lose institutional value.

### 1.2 — METHODOLOGY.md target placement (LOCKED)

Three lessons (Atomic commit, Phase 0.4 inventory, Mod-scope test isolation) integrate into a **new sub-section** within the existing "Native layer methodology adjustments" appendix, placed **after** the existing "Calibrated time estimates" sub-section and **before** the existing "Reference: K0 lessons learned" sub-section. The new sub-section is titled:

```markdown
### Pipeline closure lessons (K-series, post-K8.1)
```

The three lessons appear under this heading as `####`-level entries (one nesting deeper than the surrounding `###` entries, since they are sub-categories of the new section). Each `####` entry follows the LOCKED shape above.

This placement preserves chronological readability of the appendix: K0 lessons (top), K1-K3 lessons (middle), post-K8 lessons (new section, bottom). Future K8.2+ lessons land as additional `####` entries in this same section.

**Why a new section (not append to existing)**: existing "Native layer methodology adjustments" entries (Pre-flight checks, ABI boundary exceptions, Brief authoring step, Calibrated estimates) are K0-K3 era lessons learned during foundation milestones. The K-Lessons batch concerns post-K8 mature-pipeline-era issues that have a different character: dependency cycles in code requiring commit shape adaptation, architectural inventory drift between brief authoring time and execution time, test isolation in mature multi-mod fixtures. These are different failure modes from the K0-K3 era and deserve their own section header for future readers tracing methodology evolution.

### 1.3 — KERNEL_ARCHITECTURE.md target placement (LOCKED)

One lesson (Error semantics convention) integrates into a **new sub-section** within Part 7 (Methodology adjustments для K-series). The new sub-section is titled:

```markdown
### Error semantics convention for Interop layer
```

Placed after the existing "Operating principle continues" paragraph and before the "AD numbering continues" paragraph. The convention is the architectural authority for how new Interop wrappers (NativeMap, NativeSet, NativeComposite, FieldHandle, future primitives) choose between exception-throwing and bool-returning semantics on the managed bridge side.

**Why Part 7 (not Part 4 Decisions log, not Part 1 Architecture)**: Part 4 entries (LOCKED K-Lxx decisions) are foundational architectural commitments with explicit reversal triggers (e.g., K-L11). Error semantics convention is a methodology-level rule for **how to design** new primitives, not a foundational commitment that all primitives obey simultaneously. Part 1 §1.6 covers low-level interop patterns (P/Invoke shape, naming, [DllImport] vs [LibraryImport]); error semantics sits one level higher — it's about API surface design choices on the managed wrapper layer. Part 7 (Methodology adjustments для K-series) is the right level: rules for K-series brief authors and primitive designers.

### 1.4 — Version bump shape (LOCKED)

Both documents bump versions atomically with the lesson additions:

- `METHODOLOGY.md`: v1.4 (2026-05-07) → **v1.5 (2026-05-09)**. Change history entry added at the bottom of §10 with description: "Added 'Pipeline closure lessons (K-series, post-K8.1)' sub-section under 'Native layer methodology adjustments' with three lessons formalized from K8.1 and K8.1.1 closures: atomic commit as compilable unit, Phase 0.4 inventory as hypothesis, mod-scope test isolation."

- `KERNEL_ARCHITECTURE.md`: v1.2 (Solution A LOCKED) → **v1.3 (2026-05-09)**. The Status line is preserved as "AUTHORITATIVE LOCKED" — adding a methodology adjustment does not unlock K-L11 or any other LOCKED decision. The Date is bumped. Part 4 Decisions log gets no new entry (this is not a new architectural decision; it's a formalization of an existing convention practiced through K8.1 and K9 brief authoring).

The status of `KERNEL_ARCHITECTURE.md` v1.3 reads:

```markdown
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3)
```

### 1.5 — Atomic commit shape (LOCKED)

K-Lessons lands as **5 atomic commits**:

1. `docs(briefs): author K-Lessons batch brief (4 pipeline lessons formalization)` — Phase 0.6, on main before feature branch.
2. `docs(methodology): formalize atomic-commit-as-compilable-unit lesson` — Phase 2.1.
3. `docs(methodology): formalize Phase 0.4 inventory-as-hypothesis lesson` — Phase 2.2.
4. `docs(methodology): formalize mod-scope test isolation lesson` — Phase 2.3.
5. `docs(kernel): formalize Interop error semantics convention; bump to v1.3` — Phase 3.

Phase 4 (METHODOLOGY.md version bump) is bundled into commit 4 — it's part of the same logical edit (final lesson + version bump constitute "the v1.5 amendment"). Closure documentation (Phase 5) goes in:

6. `docs(progress): record K-Lessons closure (4 pipeline lessons formalized)` — Phase 5.1 + 5.2 combined.

**Rationale for split**: each METHODOLOGY lesson can land as its own commit because each `####` sub-section is self-contained (no cross-references between lessons inside the new section). Cloud Code may bundle commits 2-4 if execution discovers cross-textual dependencies between the lessons that surface only at edit time — but the default is per-lesson commits, matching the K8.1.1 precedent of one-commit-per-lesson where independence permits.

### 1.6 — Scope limits (LOCKED)

K-Lessons does **not**:

- Modify any source code, tests, native build, or CMake files.
- Add, remove, or rename any K-Lxx LOCKED foundational decision in KERNEL_ARCHITECTURE.md Part 0.
- Introduce new methodology principles beyond formalizing four already-observed practices.
- Modify CODING_STANDARDS.md (the error semantics convention is architecture-level, not language-level).
- Touch other documents (MOD_OS_ARCHITECTURE.md, RUNTIME_ARCHITECTURE.md, GPU_COMPUTE.md, FIELDS.md).

K-Lessons **does**:

- Append four `####` lesson entries (3 in METHODOLOGY.md, 1 in KERNEL_ARCHITECTURE.md).
- Bump both documents' version numbers and date stamps.
- Add change history entry to METHODOLOGY.md §10.
- Update KERNEL_ARCHITECTURE.md Status line to mention the v1.3 amendment.
- Update MIGRATION_PROGRESS.md with K-Lessons closure section.

---

## Phase 2 — METHODOLOGY.md additions

### 2.1 — Lesson 1: Atomic commit as compilable unit

**Target file**: `docs/methodology/METHODOLOGY.md`

**Insertion point**: after the existing `### Calibrated time estimates` sub-section (which ends with the "Caveat — what auto-mode does NOT speed up" paragraph and the "The pattern is: think slowly, decide carefully, execute fast" line) and before the existing `### Reference: K0 lessons learned` sub-section.

**Content to insert** (creates the new section header on first lesson; lessons 2 and 3 append to the same section):

```markdown
### Pipeline closure lessons (K-series, post-K8.1)

Pipeline behaviour matures across closures. Lessons surfaced from K8.1 onward concern dependency-cycle commit shapes, drift between brief authoring time and execution time, and test isolation in mature multi-mod fixtures. They differ in character from the K0-K3 era foundation lessons recorded above (descriptive pre-flight, ABI boundary catches, brief-as-step, calibrated estimates) and are recorded here as a distinct section so future readers can trace methodology evolution chronologically.

#### Atomic commit as compilable unit

The pipeline's atomic-commit discipline is normally read as "small commits with focused scope." The intuitive proxy for "small" is "few files." This proxy works in the common case; it fails when types in different files form a dependency cycle that compiles only together.

**Failure mode (observed at K8.1 closure, 2026-05-09).** K8.1 brief Phase 5 specified five separate commits, one per managed bridge wrapper file (`InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs`, plus the wiring change in `NativeWorld.cs`). At execution time, Cloud Code discovered that `InternedString.Resolve` calls `NativeWorld.ResolveInternedString`, which in turn requires `InternedString` to be already defined. The wrappers and the World wiring formed a small dependency cycle that did not factor into independently-buildable per-file commits — any subset of the five would either fail to compile or require stub methods that would be deleted in the next commit (and stub-and-delete is the textbook structural костыль).

The brief's five-commit shape was abandoned in favour of a single atomic commit bundling all five files. The deviation was recorded in the K8.1 closure report and ratified post-hoc — splitting would have produced either broken-build commits or temporary stubs, both worse than bundling.

**Principle: atomic = minimum unit that compiles and passes tests, not minimum unit by file count.**

The pipeline's atomic-commit discipline is structural, not aesthetic. Each commit should leave the project in a working state — compilable, tests passing — so that any later `git checkout` lands on a coherent codebase. When type definitions cross files in a way that requires them to land together, the file-count proxy lies: "five separate commits" produces five broken intermediate states. The compilable-unit definition is robust against this; the file-count proxy is not.

In practice, most commits remain single-file or two-file because most type definitions don't form cycles. The compilable-unit rule is a reformulation that preserves the common case and handles the cycle case without temporary scaffolding.

**Brief authoring requirement** (mandatory checklist item for any brief introducing new types across multiple files):

- [ ] **Cycle inventory**: identify any cross-file type dependencies (calls, returns, generic parameters) introduced by the brief.
- [ ] **Commit-shape decision**: for cycles, specify the bundled commit explicitly rather than mandating per-file commits the executor must override.
- [ ] **Stub-and-delete prohibition**: never specify a per-file commit shape that requires temporary stub methods later removed; that is a structural костыль regardless of how clean each commit looks in isolation.

**Falsifiable claim**: from K8.2 onward, briefs that include the cycle inventory checklist will encounter zero "executor bundled commits the brief specified to split" deviations on milestones introducing new cross-file types. A counter-example would force the rule to be re-examined for missing cycle classes.
```

**Atomic commit**: `docs(methodology): formalize atomic-commit-as-compilable-unit lesson`

### 2.2 — Lesson 2: Phase 0.4 inventory as hypothesis

**Target file**: `docs/methodology/METHODOLOGY.md`

**Insertion point**: at the end of the new `### Pipeline closure lessons (K-series, post-K8.1)` section (i.e., after the lesson 1 content from §2.1 above, appended to the same section).

**Content to insert**:

```markdown
#### Phase 0.4 inventory as hypothesis, not authority

Briefs include Phase 0.4 inventory listing files and directories the brief expects to find on disk. The intuitive reading is "expected layout — verify and STOP if wrong." Closer inspection shows the inventory is brief-authoring-time hypothesis, not execution-time authority: code layout drifts between brief authoring and execution, and the executor sees the truth on disk that the brief author saw second-hand or remembered from earlier sessions.

**Failure mode (observed at K8.1 closure, 2026-05-09).** K8.1 brief Phase 0.4 expected the managed bridge wrappers to live under `src/DualFrontier.Core.Interop/Marshalling/`. At execution time, Cloud Code listed the actual `Marshalling/` directory and found it contained ID/registry helpers (`ComponentTypeRegistry.cs`, `EntityIdPacking.cs`, `NativeComponentType.cs`) — not primary handle wrappers. The actual project convention placed primary handles top-level (`NativeWorld.cs`, `SpanLease.cs`, `WriteBatch.cs`).

A strict reading of Phase 0.4 as authority would have triggered a STOP. Cloud Code instead recognized the inventory as hypothesis, recorded the divergence, and placed the new wrappers (`InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs`) top-level matching the convention. The decision was recorded in the K8.1 closure report and ratified.

This pattern recurs across milestones. Briefs author against a model of the codebase that the brief author held in their head at authoring time; the codebase moves between then and execution. The inventory documents what the brief author thought was true, not what is true.

**Principle: Phase 0.4 inventory is a hypothesis under test, not a hard gate.**

Hard gates from Phase 0 (working tree clean, baseline tests passing, prerequisite milestone closed) protect the executor from corrupting workspace state — those remain STOP-eligible. The inventory is a separate category: it documents the brief author's expectation for the audit trail. Mismatches between expectation and reality are recorded as deviations, not stops.

This separation is consistent with the descriptive-pre-flight principle established at K0 closure (see "Pre-flight checks: descriptive over prescriptive" above) and extends it: descriptive principle handles informational *checks*; this lesson extends the same logic to informational *inventories*.

**Brief authoring requirement** (mandatory checklist item for any brief introducing new files):

- [ ] **Inventory marked as hypothesis**: Phase 0.4 wording uses "Expected layout (brief-authoring-time hypothesis; record divergences and proceed)" rather than "Expected layout (STOP if mismatched)."
- [ ] **Convention reference**: where the brief proposes a placement, cite the convention being followed (e.g., "matches NativeWorld.cs / SpanLease.cs top-level convention") rather than asserting placement absolutely.
- [ ] **Halt category clarity**: separate hard gates (workspace state) from informational inventories (file layout) explicitly in the brief's Phase 0 sub-section ordering.

**Falsifiable claim**: from K8.2 onward, briefs that mark Phase 0.4 as hypothesis will encounter fewer false-stop interruptions on layout mismatches than briefs that mark it as authority. The measurement: count execution sessions where the executor halts on Phase 0.4 layout divergence vs. proceeds with recorded deviation. Target: zero false stops on layout-only divergences from K8.2 onward.
```

**Atomic commit**: `docs(methodology): formalize Phase 0.4 inventory-as-hypothesis lesson`

### 2.3 — Lesson 3: Mod-scope test isolation

**Target file**: `docs/methodology/METHODOLOGY.md`

**Insertion point**: at the end of the `### Pipeline closure lessons (K-series, post-K8.1)` section (after the lesson 2 content from §2.2 above).

**Content to insert**:

```markdown
#### Mod-scope test isolation

Tests that exercise per-mod resource reclaim semantics (string-pool clear, ALC unload, mod-scoped registry teardown) must isolate every reference to the resource within the scope under test. Any reference taken outside the scope — including reads that look like read-only assertions — anchors the resource to a co-owning scope and prevents the test from observing reclaim.

**Failure mode (observed at K8.1.1 closure, 2026-05-09).** K8.1.1 brief Phase 5 included a test `EqualsByContent_StaleGeneration_ReturnsFalse` whose specified setup interned content under `BeginModScope("ModA") / EndModScope("ModA")`, then re-interned the same content from outside any scope as a "fresh-lookup sanity check," then called `ClearModScope("ModA")` and asserted the original handle resolved to null.

At execution, the test failed: the original handle still resolved successfully after `ClearModScope`. Per K8.1.1 brief Stop condition #3, Cloud Code read `string_pool.cpp::clear_mod_scope` and found that the brief-author-induced re-intern outside the scope had added the id to `ids_by_mod_[""]` (the empty/core scope's ownership list). `clear_mod_scope("ModA")` correctly skipped reclaim because the id was co-owned by core. The K8.1 implementation was correct; the brief's test setup had inadvertently anchored the id to a second scope.

The fix moved the fresh-lookup re-intern **inside** the `BeginModScope / EndModScope` pair, keeping the id uniquely owned by `ModA`. The test then observed the expected reclaim. The deviation was recorded in the K8.1.1 closure report.

This is a Stop-condition #3 success: the methodology caught the test-shape error before it shipped. But the underlying lesson is general — any test that asserts per-mod reclaim must keep all referencing inside the scope under test. The pattern recurs across mod-scoped resources beyond string interning (component-type registries, system-graph entries, ALC-loaded assemblies).

**Principle: tests asserting per-mod resource reclaim must hold all references to that resource inside the scope under test, including read-side references taken for assertion purposes.**

The architectural reason is structural: per-mod cleanup is implemented as "reclaim resources owned only by the scope being torn down." A reference taken from outside that scope creates co-ownership, which the cleanup correctly preserves. The test that intends to observe reclaim must therefore avoid creating that co-ownership. Read-side ergonomics (taking a handle outside a scope to "check" something inside the scope) silently changes ownership and breaks the test's intent.

This generalizes beyond string interning. K8.2 component conversion will produce tests that assert mod-scoped component-type teardown; M7-era ALC tests assert assembly unload; future mod-OS work will assert similar reclaim. All are subject to this rule.

**Brief authoring requirement** (mandatory checklist item for any brief authoring tests on per-mod reclaim):

- [ ] **Reclaim test isolation**: tests that assert reclaim of mod-owned resources must take all references — both for setup and for assertion — inside the `BeginModScope / EndModScope` pair (or equivalent for non-string mod-scoped resources).
- [ ] **Anchor warning**: the brief explicitly notes the anchoring failure mode for the test author, with a reference to this lesson.
- [ ] **Scope-leak proof obligation**: if the test must take a reference outside the scope (rare, but possible for cross-scope semantics), the test asserts that reclaim does not occur, and that intent is the test's documented purpose.

**Falsifiable claim**: from K8.2 onward, tests that follow the reclaim-test-isolation rule will assert reclaim correctly on the first build, without the executor needing to invoke Stop condition #3 to debug per-mod reclaim semantics. Counter-examples — Stop condition #3 invocations on reclaim assertions where the test setup was the cause — would force re-examination of the rule's coverage.
```

**Atomic commit**: `docs(methodology): formalize mod-scope test isolation lesson`

### 2.4 — METHODOLOGY.md version bump

After lesson 3 commits, bump METHODOLOGY.md version. The version line at the top of the document currently reads:

```markdown
*Version: 1.4 (2026-05-07). The document describes the methodology in its state after Phase 4 closure, with the M7 operating-principle elevation appended in §7 and the post-K0 / post-K1 / post-K3 native-layer adjustments appended to the native-layer adjustments section (descriptive pre-flight checks, ABI boundary exception completeness, brief authoring as prerequisite step, calibrated time estimates).*
```

Replace with:

```markdown
*Version: 1.5 (2026-05-09). The document describes the methodology in its state after Phase 4 closure, with the M7 operating-principle elevation appended in §7, the post-K0 / post-K1 / post-K3 native-layer adjustments appended to the native-layer adjustments section (descriptive pre-flight checks, ABI boundary exception completeness, brief authoring as prerequisite step, calibrated time estimates), and the post-K8.1 / post-K8.1.1 pipeline closure lessons sub-section (atomic commit as compilable unit, Phase 0.4 inventory as hypothesis, mod-scope test isolation).*
```

Then add the change history entry. Locate the change history table in §10. The current bottom row reads:

```markdown
| 1.4 | 2026-05-07 | Post-K3 calibration lesson added к «Native layer methodology adjustments»: brief time estimates from architectural docs assume hobby pace (~1h/day manual typing); auto-mode execution actual time is 5-10x faster. Future briefs must state both hobby-pace и auto-mode estimates explicitly. K0-K3 measured data: 11-17 days hobby estimate vs ~6 hours actual auto-mode. |
```

Append a new row immediately below it:

```markdown
| 1.5 | 2026-05-09 | Added "Pipeline closure lessons (K-series, post-K8.1)" sub-section under "Native layer methodology adjustments" with three lessons formalized from K8.1 and K8.1.1 closures: atomic commit as compilable unit (per K8.1 Phase 5 dependency-cycle bundling, `a62c1f3..059f712`), Phase 0.4 inventory as hypothesis (per K8.1 `Marshalling/` layout reconciliation), mod-scope test isolation (per K8.1.1 Stop condition #3 fix on `EqualsByContent_StaleGeneration_ReturnsFalse`, `fc4400d..63777ef`). |
```

The version bump and the change history entry are part of the **same commit as lesson 3** (per Phase 1.5 atomic commit shape decision). The version bump is the closing edit of the METHODOLOGY.md amendment series.

**Atomic commit**: included in `docs(methodology): formalize mod-scope test isolation lesson` (Phase 2.3 commit). Cloud Code may split into a separate "version bump" commit if it judges the lesson 3 commit too large; document the choice in closure.

---

## Phase 3 — KERNEL_ARCHITECTURE.md additions

### 3.1 — Lesson 4: Error semantics convention for Interop layer

**Target file**: `docs/architecture/KERNEL_ARCHITECTURE.md`

**Insertion point**: in Part 7 (Methodology adjustments для K-series), after the paragraph beginning "**Operating principle continues**:" and before the paragraph beginning "**AD numbering continues**:". The existing structure is:

```
**Pre-flight checks adapted**:
... (existing content)

**Brief structure**:
... (existing content)

**Operating principle continues**:
... (existing content)

**AD numbering continues**:
... (existing content)
```

Insert the new sub-section between "Operating principle continues" and "AD numbering continues":

**Content to insert**:

```markdown
### Error semantics convention for Interop layer

The Interop layer has two surfaces: the C ABI (C-level functions in `df_capi.h` / `capi.cpp`) and the managed bridge wrappers (C# classes in `DualFrontier.Core.Interop`). The C ABI surface follows a single convention; the managed bridge surface follows a four-category rule.

**C ABI surface (immutable)**: every `extern "C"` function returns a status code (or sentinel — null pointer, zero count, default value) and swallows all exceptions via `catch (...)` at the boundary. The ABI never propagates C++ exceptions across the DLL boundary; the managed side never relies on native exception propagation. This convention is established through K0-K8.1 and is non-negotiable for cross-DLL safety: uncaught C++ exceptions across the boundary are undefined behaviour (process termination, silent corruption, or platform-specific miscompiles depending on toolchain).

**Managed bridge surface (four-category rule)**: error semantics on the managed wrapper depends on the nature of the abstraction the wrapper exposes. Four categories:

1. **Sparse abstractions** (lookup, contains, search; e.g. `NativeMap<K,V>.TryGet`, `NativeSet<T>.Contains`): return `bool` or `bool TryX(...)` patterns. Rationale: the caller normally branches on whether the lookup found a value; "not found" is an expected runtime case, not an exception. Throwing on miss would be unergonomic for the common iteration shape.

2. **Dense abstractions** (indexed access, position-bound, capacity-bound; e.g. K9 `FieldHandle<T>.ReadCell`, future `RawTileField` access): throw on failure. Rationale: out-of-bounds access on a dense indexed structure is a programmer error (the caller asserted a valid index), not an expected miss. Returning `bool` would force a `TryReadCell` boilerplate at every call site, which silently degrades performance-critical iteration.

3. **Lifecycle operations** (Begin/End, Acquire/Release; e.g. `NativeWorld.AcquireSpan`, `WriteBatch` lifecycle): throw on misuse. Rationale: lifecycle violations (acquire after dispose, release without acquire, double-release) are always programmer errors. Recovery is impossible from the caller's perspective; the throw signals the bug rather than silently masking it.

4. **Construction operations** (Register, Create; e.g. `NativeWorld(registry)`, `NativeWorld.GetKeyedMap`): return the constructed handle, or throw `InvalidOperationException` if construction fails. Rationale: failure to construct is irrecoverable for the caller — a `null` handle would propagate through every subsequent method call as a NullReferenceException at the wrong level. Throwing at construction time fails fast with the right diagnostic.

**Failure mode the rule prevents (observed during K9 brief authoring, 2026-05-09).** K9 brief Phase 5.2 specified `FieldHandle<T>.ReadCell` to throw `FieldOperationFailedException` on out-of-bounds. K8.1 wrappers (`NativeMap<K,V>.TryGet`, `NativeSet<T>.Remove`) return `bool`. A naive reading of the difference is "K9 disagrees with K8.1." Closer inspection shows the difference is intentional — `NativeMap` is sparse (lookup miss expected), `FieldHandle` is dense (out-of-bounds is bug). Without an explicit convention, future primitive designers (G-series, K10/K11, third-party mod authors) face guesswork: should `FoobarHandle.GetX` throw or return `bool`? The convention removes the guesswork by tying the choice to the abstraction's nature.

**Brief authoring requirement** (mandatory checklist item for any brief introducing new managed Interop wrappers):

- [ ] **Category classification**: each new wrapper method is classified as sparse / dense / lifecycle / construction in the brief's Phase 1 architectural design section.
- [ ] **Convention compliance**: the method's error semantics (throw or bool return) matches the category.
- [ ] **Deviation rationale**: any departure from the convention requires explicit rationale in the brief, recorded as a milestone-specific architectural decision (not a silent override).

**Falsifiable claim**: from K9 onward, briefs that classify new Interop wrappers by category will produce wrappers whose error semantics is consistent with the convention. The measurement: count wrapper additions across K9, K10, K11, G-series that depart from the four-category rule without explicit rationale. Target: zero unexplained departures. A counter-example — a wrapper whose semantics fits no category cleanly — would force the convention to grow a fifth category or be re-examined.

**Cross-reference**: the convention applies to all Interop layer additions from K9 onward. K8.1 wrappers (`NativeMap`, `NativeSet`, `NativeComposite`) are already convention-compliant (sparse). K9 brief drift findings note `FieldHandle<T>` as convention-compliant (dense) but recommend the brief surface this categorization explicitly during K9 execution.
```

**Atomic commit**: `docs(kernel): formalize Interop error semantics convention; bump to v1.3`

### 3.2 — KERNEL_ARCHITECTURE.md version bump

After lesson 4 content is inserted, update the version block at the top of the document.

Currently:

```markdown
**Version**: 1.2
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added, K-L3/K-L8 implications extended)
```

Replace with:

```markdown
**Version**: 1.3
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3)
```

The version bump is part of the **same commit as lesson 4** (per Phase 1.5 atomic commit shape decision). Part 4 Decisions log gets no new entry — this is a methodology formalization of an existing convention practiced through K8.1 and K9 brief authoring, not a new architectural decision.

**Atomic commit**: included in `docs(kernel): formalize Interop error semantics convention; bump to v1.3` (Phase 3.1 commit).

---

## Phase 4 — (Empty — version bumps merged into lesson commits)

Phase 4 is intentionally empty. METHODOLOGY.md version bump is bundled into Phase 2.3 commit (last METHODOLOGY edit); KERNEL_ARCHITECTURE.md version bump is bundled into Phase 3.1 commit (the kernel edit itself). This matches Phase 1.5 LOCKED commit shape and avoids a "tax" commit for version-only changes.

If the executor judges the version bump separable from the lesson 3 / lesson 4 content (e.g., because the bump involves multi-paragraph editing), it may split into two commits per document. Document the choice in closure.

---

## Phase 5 — Closure verification

### 5.1 — Update `MIGRATION_PROGRESS.md`

Locate the K8.1.1 closure section (the most recent closure section in the document). Immediately after it, insert the K-Lessons closure section:

```markdown
### K-Lessons — Pipeline closure lessons formalization (post-K8.1.1, methodology batch)

**Status**: DONE
**Closure**: `<phase-2-1-sha>..<phase-3-sha>` on `feat/k-lessons-formalization` (fast-forward merged to main)
**Brief**: `tools/briefs/K_LESSONS_BATCH_BRIEF.md`
**Test count**: 592 (unchanged — documentation-only milestone)

**Deliverables**:

- `METHODOLOGY.md` v1.4 → v1.5: new sub-section "Pipeline closure lessons (K-series, post-K8.1)" under "Native layer methodology adjustments" with three lessons:
  - Atomic commit as compilable unit, not file-count unit (sourced from K8.1 Phase 5 bundled commit)
  - Phase 0.4 inventory as hypothesis, not authority (sourced from K8.1 `Marshalling/` reconciliation)
  - Mod-scope test isolation (sourced from K8.1.1 Stop condition #3 fix)
- `KERNEL_ARCHITECTURE.md` v1.2 → v1.3: new sub-section "Error semantics convention for Interop layer" in Part 7, codifying the four-category rule (sparse / dense / lifecycle / construction) practiced through K8.1 and K9 brief authoring.
- Change history entries in METHODOLOGY.md §10 and Status line update in KERNEL_ARCHITECTURE.md.

**Brief deviations**: record here per session findings. Expected zero structural deviations — Phase 1 design is implemented as written. Documentation-only milestone; no test count delta, no source code changes.

**Architectural decisions LOCKED in this milestone**: none new. The lessons formalize existing practice and existing convention; no foundational decision is added or modified. K-L1..K-L11 in `KERNEL_ARCHITECTURE.md` Part 0 unchanged.

**Cross-cutting impact**:

- K8.2 brief authoring (next milestone) cites the four lessons by name in its own Phase 1 design section.
- K9 brief drift findings (deferred Stage 4 work) cite the error semantics convention as the resolution of Drift #4.
- Future Cloud Code execution sessions on K-series milestones can reference the four lessons by section heading rather than by closure-narrative recall.
```

Replace `<phase-2-1-sha>..<phase-3-sha>` with the actual commit range after merge. Update the live tracker / current state snapshot section at the top of `MIGRATION_PROGRESS.md` to reflect:

- K-Lessons status DONE
- Test count 592 (unchanged)
- Most-recent closure: K-Lessons

**Atomic commit**: `docs(progress): record K-Lessons closure (4 pipeline lessons formalized)`

### 5.2 — Mark brief EXECUTED

Edit the front matter of this brief (`tools/briefs/K_LESSONS_BATCH_BRIEF.md`):

```markdown
**Status**: EXECUTED (2026-MM-DD, branch `feat/k-lessons-formalization`, closure `<phase-2-1-sha>..<phase-3-sha>`). See `docs/MIGRATION_PROGRESS.md` § "K-Lessons — Pipeline closure lessons formalization" for closure record.
```

Replace `2026-MM-DD` with the actual execution date and the SHA range with the actual commits.

The brief EXECUTED edit is part of the same commit as the MIGRATION_PROGRESS.md closure entry (Phase 5.1 commit).

### 5.3 — Final verification

Documentation-only milestone has minimal verification surface. Run:

```
git status
git log --oneline -8
dotnet test
```

**Expected**:
- Working tree clean post-merge.
- Commit log shows 6 K-Lessons commits (1 brief authoring on main + 4 lesson commits + 1 closure commit).
- `dotnet test` reports **592 tests passing** (unchanged from K8.1.1 baseline; no source code touched).

**Sanity grep on touched documents**:

```
grep -nE "TODO|FIXME|XXX|HACK" docs/methodology/METHODOLOGY.md docs/architecture/KERNEL_ARCHITECTURE.md
```

**Expected**: zero new debt markers introduced by K-Lessons. Pre-existing markers (if any) are out of scope; diff against baseline if needed.

### 5.4 — Merge to main

Branch is N commits ahead of `origin/main`. Fast-forward merge:

```
git checkout main
git merge --ff-only feat/k-lessons-formalization
```

If a non-fast-forward situation arises (main moved during execution), halt and report. K-Lessons expects to land cleanly on top of K8.1.1 closure SHA `63777ef` with no intervening commits.

Do **not** push to origin. Per established auto-mode convention, `git push` is a Crystalka decision after closure report.

---

## Stop conditions

Halt and surface the situation to Crystalka if any of the following occur:

1. **Phase 0 baseline failure**: K8.1.1 closure SHA does not match `63777ef`, baseline test count is not 592, or either target document is at unexpected version. K-Lessons cannot proceed against an unknown predecessor state.

2. **Existing structure drift in METHODOLOGY.md or KERNEL_ARCHITECTURE.md**: the target insertion points (described in Phase 2 and Phase 3) have moved or been edited since the brief was authored. The brief's textual references (e.g., "after the existing `### Calibrated time estimates` sub-section") may not match. Read the document, locate the analogous insertion point, and either proceed with adjusted insertion (recording the divergence as deviation) or halt if the section structure has changed substantively.

3. **Style mismatch with existing failure-mode pattern**: when reading existing entries (Pre-flight checks, ABI boundary exception completeness, etc.), the executor finds the brief's lesson shape (Phase 1.1 LOCKED) does not match the existing entries verbatim. Adjust the brief's content text to match the existing pattern's exact phrasing conventions (e.g., section header prefixes, paragraph ordering, falsifiable-claim phrasing). Document any adjustment in closure.

4. **Cross-reference failure**: any K-Lxx decision, milestone SHA, commit range, or document section referenced in the new lesson content does not match the actual document state. Stop and reconcile — fabricated cross-references are worse than missing them, and grep-checking is fast.

5. **Surprise during edit**: any observation that a lesson's principle conflicts with an existing METHODOLOGY or KERNEL_ARCHITECTURE rule. The brief assumes orthogonality (the four lessons add to existing rules without contradicting them); a conflict would mean either the lesson is mis-formulated or the existing rule needs updating, both of which warrant halt-and-prompt rather than silent reconciliation.

---

## Atomic commit log expected

| # | Phase | Message |
|---|---|---|
| 1 | 0.6 | `docs(briefs): author K-Lessons batch brief (4 pipeline lessons formalization)` |
| 2 | 2.1 | `docs(methodology): formalize atomic-commit-as-compilable-unit lesson` |
| 3 | 2.2 | `docs(methodology): formalize Phase 0.4 inventory-as-hypothesis lesson` |
| 4 | 2.3 + 2.4 | `docs(methodology): formalize mod-scope test isolation lesson` (includes version bump v1.4 → v1.5 and change history entry) |
| 5 | 3.1 + 3.2 | `docs(kernel): formalize Interop error semantics convention; bump to v1.3` |
| 6 | 5.1 + 5.2 | `docs(progress): record K-Lessons closure (4 pipeline lessons formalized)` |

Total: **6 atomic commits**. Splits between commits 4 and 5 (separating version bumps from lesson content) are permitted if the executor judges the bumps separable; document any split in closure.

---

## Cross-cutting design constraints

- **No source code, tests, or native build touched**: K-Lessons is pure documentation. Any incidental code edit is out of scope and signals brief drift; halt and surface.

- **No new K-Lxx LOCKED decisions added**: lessons formalize existing convention, not new architectural commitments. Part 0 of `KERNEL_ARCHITECTURE.md` is preserved verbatim. Part 4 Decisions log gets no new entry.

- **Version bumps atomically with content**: METHODOLOGY.md v1.5 and KERNEL_ARCHITECTURE.md v1.3 each bump in the same commit as their final lesson edit. Version-only commits are wasteful and break the atomic-commit-as-compilable-unit principle (which generalizes to documentation: each commit should leave the document in a coherent reviewable state).

- **Style consistency over speed**: when in doubt, match existing entry phrasing exactly. The lessons live alongside K0-K3 entries in the same appendix; stylistic drift (different bolding conventions, different falsifiable-claim formats, different sub-section depths) reduces document readability. Cloud Code reads existing entries before writing new ones.

- **No forward references to K8.2 or K9**: lessons document past closures (K8.0, K8.1, K8.1.1), not future milestones. K8.2 brief authoring will reference the lessons by their now-stable section headings; the lessons themselves do not anticipate K8.2's specific content.

---

## Execution closure

When all phases complete and all gates pass, K-Lessons is **DONE**.

Closure report to Crystalka should include:

- Commit range merged to main
- Confirmation that 592 test count is unchanged (sanity for documentation-only milestone)
- Both document version bumps confirmed (METHODOLOGY v1.4 → v1.5, KERNEL v1.2 → v1.3)
- Any deviations from this brief recorded under "Brief deviations" in the MIGRATION_PROGRESS.md K-Lessons closure section
- Any architectural surprises that should inform K8.2 brief authoring or K9 drift findings

After closure report, Crystalka decides:

- Whether to push `main` to `origin`
- Whether K8.2 brief authoring proceeds immediately, or K9 drift findings document is drafted first
- Whether any lesson surfaced an inconsistency with existing rules that requires a follow-up amendment

End of K-Lessons batch brief.
