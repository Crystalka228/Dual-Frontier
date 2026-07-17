---
register_id: DOC-E-DOC_DRIFT_REFACTOR_PROGRESS
project: Dual Frontier
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-06-02
last_modified: 2026-06-02
content_language: en
next_review_due: post-doc-drift-refactor-cascade closure
title: Documentation Dual-Load Drift — Refactor Progress Report
review_cadence: on-refactor-cascade-execution
last_review_date: 2026-06-02
last_review_event: "Refactor progress record for the Documentation Dual-Load Drift Reconnaissance refactor. Captures all work executed autonomously 2026-06-02 (DD-2 KERNEL roadmap relocation + 6-substrate-doc fencing; DD-1 increment 1 code-truth banners on 6 stale docs; REGISTER cascade rv 2.10→2.14) and itemises remaining work with recommendations (deep DD-1 body rewrites, DD-3 reclassification + doc_role schema, DF→DFK orphan cleanup, ANALYZER_RULES post-A'.9.1-Phase-β, count/version desyncs, pwsh validation + REGISTER_RENDER regen, project.godot decision). Companion к DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT. К-L impact: zero."
reviewer: Crystalka
risks_referenced: []
capa_entries_referenced: []
special_case_rationale: Live progress report (continuously updated as refactor increments land) tracking the multi-cascade execution of the spec/roadmap separation. Category E (docs/reports/), Tier 2 (governance-leverage, companion к the Tier-2 reconnaissance report). Referenced by the DD-1 code-truth banners placed in ARCHITECTURE/THREADING/EVENT_BUS/VULKAN_SUBSTRATE as the remaining-work tracker. pwsh unavailable — frontmatter mirror authored manually; sync_register.ps1 -Validate + render deferred к Crystalka environment.
---

---
register_id: DOC-E-DOC_DRIFT_REFACTOR_PROGRESS
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
last_modified: "2026-06-02"
last_modified_commit: "PENDING-COMMIT-DOC_DRIFT_RECON-PROGRESS"
content_language: en
review_cadence: on-refactor-cascade-execution
next_review_due: on-refactor-cascade-execution
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-DOC_DRIFT_REFACTOR_PROGRESS
source_report: DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT
---
# Documentation Dual-Load Drift — Refactor Progress Report

**Date**: 2026-06-02
**Branch**: `claude/doc-drift-reconnaissance-503aH`
**Executed by**: Claude Code (Opus), autonomously per Crystalka direction («да можешь продолжить выполнить полностью, отчёт о всей работе положишь в документы, я потом прочитаю и проверю»)
**Source plan**: [`DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md`](./DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md) (the reconnaissance that produced the refactor plan)
**Purpose**: a single record of everything the refactor session changed, and everything that remains — for Crystalka's review and verification.

> **Read this first.** This is a *progress* report, not a closure. The session executed the **safe, code-grounded** portion of the spec/roadmap separation. It deliberately **did not** attempt risky deep rewrites of LOCKED Tier-1 specs or REGISTER schema changes while operating autonomously — those are itemised in §3 as remaining work with recommendations. Every change is committed and pushed; nothing is staged or hidden.

---

## §1 — Executive summary

The Documentation Dual-Load Drift Reconnaissance (report enrolled `DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT`) recommended **Option ε** (REGISTER `doc_role` metadata + fenced NON-NORMATIVE sections, anchored on the existing `docs/ROADMAP.md`) and a **3-cascade** decomposition: **DD-1** spec-truth restoration, **DD-2** spec/roadmap structural separation, **DD-3** misclassification + orphan + desync hygiene.

This session executed:
- **DD-2 fully** for every architecture doc carrying a genuine roadmap component (KERNEL relocation + 6 substrate docs fenced).
- **DD-1 increment 1** — code-truth banners + surgical fixes on the 6 stale architecture docs (no risky body rewrites).
- **REGISTER kept in lockstep** throughout (`register_version` 2.10 → 2.14, 5 audit_trail EVTs, all version bumps + frontmatter mirrors synced, YAML validated at each step).

**Discipline held**: zero production code touched, zero К-L invariant text changed (count stays 21), KERNEL Part 0 untouched (halt-6). The one operative principle for autonomous work was **accuracy over completeness** — where the correct replacement was genuinely uncertain (the managed-facade/native-authoritative coexistence), the session flagged with a banner pointing to the authoritative source rather than fabricating spec.

---

## §2 — Work completed (commit ledger)

| Commit | Cascade | Summary |
|---|---|---|
| `90b9a1b` | recon | Reconnaissance report authored (`docs/reports/DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT.md`) |
| `1d2e682` | enrollment | Enrolled report + source brief in REGISTER (rv 2.10→2.11) |
| `61ee43e` | DD-2 | **KERNEL_ARCHITECTURE** Part 2/3 roadmap relocated → `docs/ROADMAP.md` (rv →2.12) |
| `512c171` | DD-2 | **6 substrate docs** code-grounded roadmap fencing (rv →2.13) |
| `6480df1` | DD-1 | **6 stale docs** code-truth banners + surgical fixes (rv →2.14) |
| _(this)_ | report | This progress report + enrollment |

### §2.1 — Enrollment (`1d2e682`)
- `DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT` (Tier 2 AUTHORED) and `DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF` (Tier 4 EXECUTED) enrolled. The brief was authored into `tools/briefs/` so the report's `source_brief` pointer resolves (avoiding the exact orphan-reference this exercise targets).

### §2.2 — DD-2: KERNEL roadmap relocation (`61ee43e`)
`KERNEL_ARCHITECTURE.md` (v2.5.3 → **v2.6.0**), the #1 dual-load specimen (self-titled "Architecture **&** Roadmap"):
- **Part 2 (Roadmap K-series) + Part 3 (Migration strategy) relocated verbatim** → `docs/ROADMAP.md` § "Native foundation tracks → K-series — Native ECS kernel — CLOSED" (headers demoted to nest).
- Replaced in KERNEL with a **fenced NON-NORMATIVE pointer**; title → "Architecture"; scope "+ milestone roadmap" dropped.
- **Part 0 К-L invariants untouched** (S-LOCK-1 / halt-6). 1123 → 912 lines.
- ROADMAP status-table K-series + K9 rows retired ⏭ Pending → ✅ Closed (these were the report's load-bearing C7 staleness). 634 → 862 lines.
- *Versioning note*: MINOR bump chosen (content relocated, not deleted; zero substrate/invariant impact). KERNEL's own convention says "structural reorganization = major" → **Crystalka may escalate to v3.0.0** if preferred.

### §2.3 — DD-2: substrate roadmap fencing (`512c171`)
Per your instruction («сначала ревью на коде чтобы было понято что уже готово»), **every forward claim was verified against code first**, then corrected (if realized) or fenced NON-NORMATIVE (if genuinely pending). Code-grounding results:

| Doc | v | Code-truth finding | Action |
|---|---|---|---|
| FIELDS | 0.1→0.1.1 | K9 **shipped** (`tile_field.*`/`FieldRegistry.cs`/`FieldHandle.cs`) — "TBD lands at K9" was stale | 3× "TBD" → "Shipped at K9"; Save/load = only genuine pending |
| ISOLATION | 1.1→1.1.1 | analyzer infra **shipped** (17 stubs), detection **pending Phase β** (zero `ReportDiagnostic`) | "A'.9 planned" threads re-grounded |
| PERFORMANCE | 1.1→1.1.1 | managed `ComponentStore<T>` hot-path dead (native authoritative); diffusion compute shipped, flow-field pending | stale flag + 3 fences |
| COMBO_RESOLUTION | 0.2→0.2.1 | `ComboResolutionSystem` = `NotImplementedException` stub; `DamageKind.cs`/tests absent | design-status banner |
| COMPOSITE_REQUESTS | 1.0→1.0.1 | `CompositeResolutionSystem` stub; named types absent | design-status banner |
| OWNERSHIP_TRANSITION | 1.0→1.0.1 | golem system **shipped**; `GolemOwnershipRefused` + `GolemBondStrength` genuinely pending | shipped banner + 2 inline fences |

### §2.4 — DD-1: code-truth banners (`6480df1`)
The four large SPEC-STALE docs describe the pre-К10 managed-ECS / pre-Godot world, but the real state is a **managed-facade/native-authoritative coexistence** that is genuinely nuanced — `ParallelSystemScheduler.cs` and `DomainEventBus.cs` still exist as facades, and even their own source comments are stale (e.g. "one core for Godot's main thread"). Rather than risk new errors, each got a **code-truth banner** pointing to the authoritative native source + `KERNEL_ARCHITECTURE` Part 0/1, enumerating known divergences. Two small high-confidence clauses were surgically fixed.

| Doc | v | Treatment |
|---|---|---|
| ARCHITECTURE | 0.4→0.4.1 | banner (partial: Presentation section already Godot-clean; Infrastructure/Core still managed-era) |
| THREADING | 1.1→1.1.1 | banner (native `system_graph` authoritative К10.1; managed scheduler = adapter facade) |
| EVENT_BUS | 1.1→1.1.1 | banner (native tiered bus authoritative К10.2/К-L15; `SetComponent` removed К8.3+К8.4) |
| ECS | 1.1→1.1.1 | **surgical** (scheduler-facade clause clarified; doc otherwise current) |
| FEEDBACK_LOOPS | 0.2→0.2.1 | code-truth note (registration-time `IsolationViolationException` flagged for verification) |
| VULKAN_SUBSTRATE | 1.1.1→1.1.2 | banner (Godot retired cascade #2; body Godot framing flagged; **`project.godot` residual flagged**) |

---

## §3 — What remains (with recommendations)

### §3.1 — DD-1 deep body rewrites (the larger, riskier work)
`ARCHITECTURE`, `THREADING`, `EVENT_BUS`, `VULKAN_SUBSTRATE` carry banners but their **bodies still describe the managed-era / Godot mechanics**. A faithful rewrite requires deciding and documenting the **managed-facade ↔ native-authoritative relationship** precisely (the source comments are themselves stale, so this needs dedicated code study + your architectural confirmation). **Recommendation**: a focused follow-up cascade, one doc at a time, each verified against `native/DualFrontier.Core.Native/` + `SchedulerAdapter`/`BusFacade`/`SystemGraphInterop`, with you confirming the intended managed-vs-native framing. `ARCHITECTURE.md` specifically may be cleaner to **supersede + redirect** to `KERNEL_ARCHITECTURE` than to rewrite.

### §3.2 — DD-3 misclassification reclassification (7 docs)
The report's structural meta-finding: roadmap/forward-contract/deliberation artifacts registered as Tier-1 descriptive Category-A specs. **Recommended `doc_role`** (Option ε δ-backbone):

| Document | Current | Recommended `doc_role` |
|---|---|---|
| `PHASE_A_PRIME_SEQUENCING.md` | A / Tier-2 / Live | ROADMAP |
| `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | A / Tier-1 / LOCKED | ROADMAP |
| `FHE_INTEGRATION_CONTRACT.md` | A / Tier-1 / LOCKED | RESERVED-CONTRACT |
| `ARCHITECTURE_TYPE_SYSTEM.md` | A / Tier-1 / Draft | FORWARD-SPEC |
| `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` | A / Tier-1 / Draft | ACTIVATION-BRIEF |
| `KERNEL_FULL_NATIVE_SCHEDULER.md` | A / Tier-1 / LOCKED | DELIBERATION-SNAPSHOT |
| `DEVELOPMENT_HYGIENE.md` | B / Tier-1 / LOCKED | (SPEC-STALE — near-rewrite, see §3.1) |

**Why not done autonomously**: adding a `doc_role` field is a **REGISTER schema extension** (FRAMEWORK schema_version change) that should be ratified, not introduced ad-hoc — especially since `sync_register.ps1 -Validate` can't run here to confirm it (§3.6). **Recommendation**: ratify the `doc_role` schema field (FRAMEWORK amendment), then apply it in one DD-3 REGISTER cascade.

### §3.3 — DF### → DFK### orphan-reference cleanup
Living-spec cross-refs still use the pre-rename `DF###` IDs: `K_CLOSURE_REPORT.md` §7 (DF001..DF020), `KERNEL_ARCHITECTURE.md`, `K_EXTENSIONS_LEDGER.md`, `K_L14_EVIDENCE_DASHBOARD.md`, `PHASE_A_PRIME_SEQUENCING.md` (snapshot briefs are exempt). **Not done autonomously** because the rename is non-uniform (`DFK###` vs `DFL###` vs `DF999` stays) and `K_CLOSURE_REPORT` §7 enumerates a full rule table that needs careful mapping. **Recommendation**: a scripted, reviewed find-map pass (not blind sed) in a dedicated DD-3 step.

### §3.4 — ANALYZER_RULES.md (deferred by design)
The prime specimen, but it is the active **A'.9.1 Phase β** surface (report Q-DD-8). Untouched this session. **Recommendation** (report's): let A'.9.1 Phase β land first (its detection-logic implementation naturally fixes the §4 "stub→active" status), then extract §5/§6/§10.5/§11 roadmap. You have not yet ratified Q-DD-8 sequencing.

### §3.5 — Count/version desyncs (DD-3)
Verifiable, mostly small, but several touch LOCKED governance text: `REGISTER_RENDER.md` (5 versions stale — needs regen, §3.6), `FRAMEWORK.md` §8.1 "4 vs 5 meta-entries" (LOCKED protocol — flag, don't auto-edit), `METHODOLOGY.md` frontmatter v1.12.1 with no changelog backing + absent Lesson #N18, `MIGRATION_PLAN` body-v1.3 vs frontmatter-v1.4. **Recommendation**: bundle into the DD-3 cascade alongside §3.2.

### §3.6 — Tooling gap (blocks two items)
**`pwsh` is unavailable in this environment.** Therefore `sync_register.ps1 -Validate` (the canonical gate) and `render_register.ps1` (regenerates `REGISTER_RENDER.md`) **could not be run**. All REGISTER edits + frontmatter mirrors were authored manually and validated structurally with a Python YAML parse (269 docs, parses clean at every step). **Action required**: run `tools/governance/sync_register.ps1 -Validate` (expect exit 0) and `render_register.ps1` in an environment with PowerShell, and commit the regenerated `REGISTER_RENDER.md`.

### §3.7 — `project.godot` residual (your decision)
You stated Godot is removed, but `project.godot` still exists at the repo root, and `godot` still appears in a few `.cs` files (`PresentationBridge`, `SimStateLayer`, `ParallelSystemScheduler` comment, `SystemExecutionContext`, `SystemBase`). This contradicts `VULKAN_SUBSTRATE` §6 R.8 ("grep godot empty"). **Not deleted autonomously** — `project.godot` is a root project artifact and removing it (plus scrubbing the `.cs` references) is a code change, not a doc edit. **Recommendation**: confirm whether `project.godot` + the residual `.cs` references should be removed; if yes, that's a small code-cleanup commit (separate from the doc refactor).

---

## §4 — Verification checklist (for Crystalka)

1. `git log --oneline` shows commits `90b9a1b → 6480df1` (+ this report) on `claude/doc-drift-reconnaissance-503aH`. ☐
2. `KERNEL_ARCHITECTURE.md` is now pure "Architecture" (no Part 2/3); the K-series detail lives in `docs/ROADMAP.md` "Native foundation tracks". Part 0 invariants byte-identical. ☐
3. The 6 substrate docs carry code-grounded fences; spot-check FIELDS ("Shipped at K9") and OWNERSHIP (shipped + 2 pending) against `src/`. ☐
4. The 6 DD-1 banners point to authoritative sources and are accurate. ☐
5. `REGISTER.yaml` `register_version: 2.14`; run `sync_register.ps1 -Validate` (expect exit 0) — **§3.6**. ☐
6. Decide: KERNEL major-vs-minor (§2.2), `doc_role` schema (§3.2), Q-DD-8 sequencing (§3.4), `project.godot` (§3.7). ☐
7. Regenerate `REGISTER_RENDER.md` (§3.6). ☐

---

## §5 — REGISTER state summary

- `register_version`: 2.10 → **2.14** (4 increments).
- New enrollments: `DOC-E-DOCUMENTATION_DUAL_LOAD_DRIFT_REPORT`, `DOC-D-DOC_DRIFT_RECONNAISSANCE_BRIEF`, + this `DOC-E-DOC_DRIFT_REFACTOR_PROGRESS`.
- audit_trail EVTs appended (5): `…DOC_DRIFT_RECONNAISSANCE-ENROLLMENT`, `…DD2-KERNEL-ROADMAP-EXTRACTION`, `…DD2-SUBSTRATE-ROADMAP-FENCING`, `…DD1-SPEC_TRUTH-BANNERS`, `…PROGRESS` (this).
- Docs version-bumped: KERNEL (2.6.0) + 12 architecture docs (patch) + frontmatter mirrors synced.
- **Pending tool action**: `sync_register.ps1 -Validate` + `render_register.ps1` (no pwsh here).

---

**К-L14 thesis preserved.** Zero production code, zero substrate, zero К-L invariant change (count 21); KERNEL Part 0 untouched. Architectural-integrity restoration: the #1 dual-load specimen is resolved, the substrate docs carry code-grounded forward labels, and the stale docs no longer read as current truth. PA-001/PA-002/PA-003/PA-004 anchored. Без костылей.

**Forward**: Crystalka reviews → ratifies the §4 decisions → DD-3 cascade (reclassification + orphan + desync + REGISTER_RENDER regen) + deeper DD-1 body rewrites + ANALYZER_RULES after A'.9.1 Phase β.