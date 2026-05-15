---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-NAMESPACE_CASCADE_MAP
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: "1.0"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-NAMESPACE_CASCADE_MAP
---

# M / G / R Namespace Cascade Map

**Authored**: 2026-05-15 (Claude Opus 4.7, read-only recon session)
**Branch**: `claude/milestone-cascade-map-akumN`
**Repo HEAD at recon start**: `b9aeb6f` (merge of architecture reconnaissance audit)
**Source brief**: `DOC-D-NAMESPACE_CASCADE_RESEARCH_BRIEF` (2026-05-15)
**Predecessor recon**: `docs/reports/ARCHITECTURE_RECON_REPORT.md` (broad architecture audit)
**Scope**: Read-only mechanical inventory of every milestone ID reference across LOCKED/Live docs, briefs, REGISTER, module READMEs, and code comments — input to a forthcoming ratification proposal for composite milestone namespace (M-K / M-G / M-R / R / G).

---

## 1 — Read Me First

This document is **mechanical inventory, not architectural analysis**. Each row records: where a milestone ID appears, the verbatim context, and the candidate rename only where the 2026-05-15 deliberation has already locked the case. Where a rename is **ambiguous**, the row is marked `TBD: requires deliberation` and the ambiguity is surfaced in §3 for the ratification proposal to resolve.

Three operating constraints from the source brief that the reader must hold:

- **The composite namespace decision is fixed in deliberation; this brief is downstream of it.** The 2026-05-15 deliberation locked the substrate buckets (`K`, `A'`, `R`, `G`) and the demonstration buckets (`M-K`, `M-G`, `M-R`). It explicitly named two cascading renames (`G7 → M-G7` for `Vanilla.Movement`, `G8 → M-G8` for `Local Avoidance`) and a retroactive bucket assignment (10 current production systems → `M-K`).
- **Closed audit reports / closure reviews for M3..M7 are historical record.** They are inventoried for awareness but the ratification proposal should decide whether to back-rename historical M-IDs (M0..M7) or leave them as record-of-what-shipped. The default view of this map is: the M0..M7 sequence is a closed mod-OS migration nomenclature; the rename target is the **pending** M-cycle (M8.x, M9, M10.x — the vanilla content) and the **pending** runtime `M9.0..M9.8` (which collides with the M-cycle M9).
- **The M-cycle / M-runtime collision is the largest single source of ambiguity.** The current ROADMAP carries an explicit numeric collision: `M9 — Vanilla.Combat` (mod-cycle, ROADMAP.md:44) and `M9.0–M9.8 — Vulkan + Win32 runtime` (runtime, ROADMAP.md:49). The deliberation's introduction of `R` as the runtime bucket resolves this; how it resolves is open (§3 Q-R-1).

---

## 2 — Substrate Bucket Definitions (locked 2026-05-15)

The buckets below are the deliberation result restated verbatim from the source brief §0.1, §0.3, and the brief's «⚠ READ ME FIRST» bullet 1. They are **not** re-deliberated here; they bound the inventory.

| Bucket | Nature | Current state | Locked rename examples |
|---|---|---|---|
| **K** | Kernel substrate | K0..K9 closed (K8.5 deferred → A'.7) | Unchanged. K-series stays K. |
| **A'** | Refactor cycle on top of K | A'.0..A'.5 closed, A'.6..A'.9 pending | Unchanged. A'-series stays A'. |
| **R** | Runtime / Vulkan substrate | Currently named **M9.0..M9.8** in `RUNTIME_ARCHITECTURE.md` v1.0 | All M9.x runtime → R namespace. Specific format TBD (§3 Q-R-1). |
| **G** | GPU compute substrate | G0..G6, G9 (`GPU_COMPUTE.md` v2.0) | G0..G6, G9 stay G *unless* re-bucketed to R (§3 Q-G-1). |
| **M-K** | Vanilla content on K substrate (demonstration) | 10 current production systems retroactively | Locked: 10 systems on `NativeWorld` (post-A'.5) belong to M-K. Specific per-system IDs TBD. |
| **M-G** | Vanilla content on G substrate (demonstration) | Currently G7, G8 in G-skeleton briefs | **G7 → M-G7** (Vanilla.Movement). **G8 → M-G8** (Local Avoidance). |
| **M-R** | Vanilla content on R substrate (demonstration) | Currently TBD | No rename target yet — no vanilla content authored against R substrate. |

**Locked rename catalogue (only items deliberation explicitly named):**

1. `G7 Vanilla.Movement` → `M-G7` (vanilla content on G substrate)
2. `G8 Local Avoidance` → `M-G8` (vanilla content on G substrate)
3. 10 current production systems on `NativeWorld` → bucketed under `M-K` (specific per-system IDs not locked)

Everything else is **TBD** — the ratification proposal must decide format and per-ID disposition. See §3.

---

## 3 — Open Naming Questions (surfaced for ratification proposal)

These ambiguities surfaced during inventory. Recon **does not answer**; it only registers them so the proposal author has them in front of them.

| ID | Question | Current state | Why ambiguous |
|---|---|---|---|
| **Q-R-1** | Format for `M9.0..M9.8` → `R` rename | `M9.0..M9.8` defined in `RUNTIME_ARCHITECTURE.md` §2.1 v1.0 LOCKED | Three candidate formats: (a) `R0..R8` flat, (b) `R-9.0..R-9.8` preserving sub-index, (c) `R-Runtime0..R-Runtime8`. Deliberation locked the *bucket* (R), not the *format*. The proposal must pick one. |
| **Q-R-2** | M-cycle `M9 — Vanilla.Combat` (ROADMAP.md:44) collision with runtime `M9.0..M9.8` | Two distinct meanings of `M9` co-exist in the active ROADMAP | The M-cycle `M9` (Vanilla.Combat content) becomes `M-K9` (or analog) under composite namespace, freeing `M9` of overload — but the proposal must lock the exact M-cycle rename. |
| **Q-G-1** | G0 substrate identity — `G0` retained or `R0` | `G0` defined as "Vulkan compute pipeline plumbing" (`GPU_COMPUTE.md` §G0); shares Vulkan plumbing with runtime | If R bucket subsumes Vulkan plumbing entirely, G0 may migrate to R. If G remains a distinct substrate for *compute*-only-Vulkan, G0 stays. The deliberation did not lock this. |
| **Q-G-2** | G0..G6 + G9 disposition under composite namespace | G0..G6 are infrastructure (plumbing, mana/electricity/storage/multi-field/projectile/flow-field-infra); G9 is optional eikonal upgrade | All seven are G-substrate-buildup, not vanilla content. Are they retained as G0..G6 + G9 (substrate items, mirroring K0..K9), or do they take a different form? The deliberation locked the *bucket* (G), not the per-item disposition. |
| **Q-M-1** | M-cycle M0..M7 (closed) retroactive rename | M0..M7 closed; documented in ROADMAP, closure reviews, M-cycle audit reports | Two options: (a) retroactive `M-K0..M-K7` for record consistency, (b) leave M0..M7 as historical record (closed phases keep their closure-time names). The proposal must decide. |
| **Q-M-2** | M-cycle M8.x / M9 / M10.x (pending vanilla content) rename | M8.4 (Vanilla.World), M8.5-M8.7 (Vanilla.Pawn), M9 (Vanilla.Combat), M10 (Inventory), M10.B (Magic), M10 incremental (Core) — all pending | These are vanilla content **on K substrate** (per Migration Plan §6.2). Under composite namespace they belong in `M-K`. Specific per-milestone format TBD (e.g., is M8.4 → `M-K8.4` or `M-K-World`?). |
| **Q-M-3** | M3.5 deferred — capability registry refresh | Documented in `MOD_OS_ARCHITECTURE.md` §11.1 v1.6 (deferred from K9; unblocks at K9 in-progress) | Closed phases (M0..M7) get one answer; explicitly deferred phases (M3.4 deferred, M3.5 deferred) may get another. The proposal must decide how deferred-but-named milestones cascade. |
| **Q-K-1** | K8.5 vs A'.6 / A'.7 — overlapping identity | `KERNEL_ARCHITECTURE.md` Part 2 row says **K8.5** "Mod ecosystem migration prep"; `PHASE_A_PRIME_SEQUENCING.md` §2 says **A'.6** = K8.5 (re-numbered after K8.3+K8.4 combination); also `A'.7 — K8.5 skeleton execution` | Same milestone carries two IDs across two LOCKED/Live docs. Not a namespace-rename question per se; a register-of-truth question. Surface for proposal author awareness. |
| **Q-V-1** | `Vanilla.X` mod-name affiliation when M-cycle renames | `Vanilla.Combat → M9`, `Vanilla.Movement → G7`, etc. | The names `Vanilla.Combat`, `Vanilla.Movement` are **type/namespace names** in C# assemblies (`mods/DualFrontier.Mod.Vanilla.Combat/`). They are not milestone IDs; they carry milestone affiliation in doc-comments. The proposal must clarify: does it just rename milestone refs in doc-comments, or also reconsider the type-name → milestone-bucket mapping? |
| **Q-V-2** | Vanilla-content with multiple substrate dependencies (e.g. Combat with both K-storage and G-compute systems) | Combat will likely depend on K (NativeWorld for combat components), and may also use G (compute for projectile pathing per G5) | Composite ID `M-K9` (Vanilla.Combat) or `M-G5` (Domain B projectile reactivation) — which bucket wins when a single vanilla mod spans substrates? The deliberation example (G7 → M-G7) is a single-substrate case. Multi-substrate mapping not locked. |

---

## 4 — Reference Inventory by Series

Five subsections — one per ID series. Each table presents references with line citation, verbatim context (excerpted to the relevant snippet), and proposed new ID where unambiguous.

### 4.1 — K-series references (unchanged for rename — summary only)

K-series IDs (`K0..K9`, `K8.0..K8.5`, `K-L1..K-L11`, `K-L3.1`) **stay as K** under the deliberation's composite namespace. Inventory is here for completeness — the rename cascade does not modify these IDs. Counts only; no per-line citation needed since no rename target.

| File | K-series refs (approx) | Rename status |
|---|---|---|
| `docs/architecture/KERNEL_ARCHITECTURE.md` | ~140 | Unchanged. K-series is the kernel substrate, retained. |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | ~110 | Unchanged. |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | ~40 | Unchanged. |
| `docs/architecture/RUNTIME_ARCHITECTURE.md` | ~15 | Unchanged (mostly K0..K8 callouts). |
| `docs/architecture/GPU_COMPUTE.md` | ~25 (mostly K9 references) | Unchanged. |
| `docs/architecture/FIELDS.md` | ~30 (mostly K9 + K-L3 references) | Unchanged. |
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | ~60 | Unchanged. |
| `docs/methodology/METHODOLOGY.md` | ~50 (K-Lessons, K-L1..K-L11) | Unchanged. |
| `docs/ROADMAP.md` | ~20 | Unchanged. |
| `docs/governance/REGISTER.yaml` | 31 DOC-D-K* entries + cross-refs | Unchanged. |
| `tools/briefs/K*_BRIEF.md` (31 K-series briefs) | ~30-100 refs each, ~1500 total | Unchanged. |
| `src/`, `tests/`, `native/` code comments | 188 K-series comment refs | Unchanged. |
| `mods/Vanilla.*/README.md` | 0 K-series refs | Unchanged. |

**Anchor references** (sampled to demonstrate K-series stays anchored):

- `docs/architecture/KERNEL_ARCHITECTURE.md:60` — K-L11 lock: «Production storage backbone | NativeWorld single source of truth (Solution A); ManagedWorld retained as test fixture and research artifact only».
- `docs/architecture/KERNEL_ARCHITECTURE.md:600` — K8.0 row: «Architectural decision recording (Solution A) | 1-2 days | +/- (docs only)».
- `docs/architecture/KERNEL_ARCHITECTURE.md:606` — K9 row: «Field storage abstraction (`RawTileField<T>`) | 1-2 weeks | +600-900».

### 4.2 — A'-series references (unchanged for rename — summary only)

A'-series IDs (`A'.0`, `A'.0.5`, `A'.0.7`, `A'.1`, `A'.2`, `A'.3`, `A'.4`, `A'.4.5`, `A'.5`, `A'.6`, `A'.7`, `A'.8`, `A'.9`) **stay as A'** under the deliberation's composite namespace. Summary only.

| File | A'-series refs (approx) | Rename status |
|---|---|---|
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | ~110 | Unchanged. Canonical authority on A'-series. |
| `docs/architecture/KERNEL_ARCHITECTURE.md` | ~15 | Unchanged. |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | ~30 | Unchanged. |
| `docs/methodology/METHODOLOGY.md` | ~25 | Unchanged. |
| `docs/governance/REGISTER.yaml` | 12 DOC-D-A_PRIME* entries + EVT-* + REQ-* cross-refs | Unchanged. |
| `tools/briefs/A_PRIME_*_BRIEF.md` (10 A'-series briefs) | ~50-150 refs each | Unchanged. |
| `src/`, `tests/` code comments | 4 A'-series refs | Unchanged. |

**Anchor references** (sampled):

- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:53` — sequence diagram opens: «[K8.2 v2 closure — DONE 2026-05-09, commits 7527d00 on main]».
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:109` — A'.5 closure: «Phase A'.5 — K8.3+K8.4 combined storage cutover — DONE 2026-05-14».
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:121` — «Phase A'.7 — K8.5 skeleton execution».

### 4.3 — M-series references (the largest cascade target)

M-series is split into three sub-categories by deliberation intent:

- **M-current (closed)**: M0..M7 — historical, closed phases. Default `TBD` per Q-M-1.
- **M-current (pending vanilla content)**: M8.4, M8.5-M8.7, M9, M10.x, M10.B, M3.5 (deferred). Belong to `M-K` per Q-M-2.
- **M9.x runtime (M9.0..M9.8)**: Vulkan runtime. Belong to `R` per Q-R-1; **collides with M-cycle M9** per Q-R-2.

#### 4.3.1 — M9.0..M9.8 runtime references (98 raw refs; LOCKED bucket → R)

Per-file count of `M9.x` runtime references:

| File | M9.x refs | Notes |
|---|---|---|
| `docs/architecture/RUNTIME_ARCHITECTURE.md` | 38 | Canonical M9.x definition site (v1.0 LOCKED). |
| `docs/MIGRATION_PROGRESS.md` | 21 | Progress tracker. |
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` | 12 | Recent audit; historical. |
| `docs/architecture/KERNEL_ARCHITECTURE.md` | 11 | Cross-references to runtime layer. |
| `docs/ROADMAP.md` | 5 | Public roadmap. |
| `tools/briefs/K2_REGISTRY_TESTS_BRIEF.md` | 2 | Cross-ref. |
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md` | 2 | Prereq citation: `M9.0–M9.4`. |
| `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` | 1 | |
| `tools/briefs/MODULE.md` | 1 | |
| `tools/briefs/K9_BRIEF_REFRESH_PATCH.md` | 1 | |
| `docs/architecture/GPU_COMPUTE.md` | 1 | |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | 1 | |
| `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` | 1 | |
| `docs/README.md` | 1 | |

**Critical verbatim citations (rename target = R, format TBD per Q-R-1):**

| File:line | Verbatim context | Proposed new ID |
|---|---|---|
| `docs/architecture/RUNTIME_ARCHITECTURE.md:20` | "Every architectural decision in this document is final input to all subsequent migration milestones (M9.0–M9.8, see §2)." | M9.0–M9.8 → TBD: R0–R8 or R-9.0–R-9.8 (Q-R-1) |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:28` | "Nine implementation milestones (M9.0–M9.8) sequenced." | M9.0–M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:34` | "During implementation of milestones M9.0 through M9.8, every interface, P/Invoke declaration, struct layout, and lifecycle step traces back to a section here." | M9.0..M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:51` | "Methodology of the development pipeline — covered by [METHODOLOGY](/docs/methodology/METHODOLOGY.md), with M9.x adjustments noted in §7." | M9.x → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:72` | "**Estimated scope:** 4–7 weeks at hobby pace (~1h/day) к full M8.x parity на Vulkan ([ROADMAP](./ROADMAP.md) M8 closure → M9 entry)." | Mixed: M8.x stays as M-K8.x candidate; M9 entry → TBD (Q-R-2) |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:90` | "L9 | Migration approach | Parallel — keep Godot Presentation functional до M9.5 cutover" | M9.5 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:289` | "Supersedes the Godot `IInputSource` adapter ([VISUAL_ENGINE](./VISUAL_ENGINE.md) §Contracts) once M9.5 cutover lands." | M9.5 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:450` | "**Recommendation:** start с Option A (direct `LibraryImport`) для M9.0–M9.4. Migrate к canonical procedure-address loading (`vkGetInstanceProcAddr` dispatch) в M9.5+ if profiling demands." | M9.0–M9.4 / M9.5+ → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:503` | "**Goal post-M9.8:** ~520 total tests (472 + new)." | M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:522` | "The milestones extend [ROADMAP](./ROADMAP.md). The numeric range M9.0–M9.8 is reserved for the Vulkan migration; phases above M10 remain reserved for the post-migration topics in §4." | M9.0–M9.8 → TBD; also calls out M10 (M-cycle) |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:526-534` | Roadmap table rows: "M9.0 \| Foundation: Win32 window + Vulkan clear color" … through "M9.8 \| Migration cutover (delete Godot)" | Eight rows; each M9.x → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:538` | "### 2.2 M9.0 — Foundation: Win32 window + Vulkan clear color" | Subsection header; M9.0 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:564, 574, 584, 594, 604, 614, 624, 634` | Sub-section headers `### 2.3 M9.1`, `### 2.4 M9.2`, ... through `### 2.10 M9.8` | Each M9.x → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:650` | "Keep `DualFrontier.Presentation` (Godot) functional через М9.5. New Runtime project develops в parallel. M9.5 cutover migrates Presentation к Runtime. M9.8 deletes Godot remnants." | M9.5 / M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:668` | "Editor scope \| Post-M9.8 evaluation" | M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:670-672` | Open decisions table rows referencing M9.6, M9.5+ | M9.x → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:713` | "**Required tooling — install before M9.0:**" | M9.0 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:721` | "BMFont — bitmap font generator (M9.6)." | M9.6 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:728` | "## 7. Methodology adjustments для M9.x" | M9.x → TBD (section header) |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:769-770` | "superseded for production by this document at M9.8 ... deprecated at M9.8." | M9.8 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:774` | "[PERFORMANCE](./PERFORMANCE.md) — target metrics; sprite/tile budgets adopted in M9.2 / M9.3." | M9.2 / M9.3 → TBD |
| `docs/architecture/RUNTIME_ARCHITECTURE.md:782` | "This document (RUNTIME): Vulkan rendering layer, M-series milestones (M9.0-M9.8)" | M9.0-M9.8 → TBD; note phrase "M-series" needs corrective rename to "R-series" |
| `docs/architecture/KERNEL_ARCHITECTURE.md:40` | "**Combined с RUNTIME_ARCHITECTURE.md (M9.0-M9.8) + GPU_COMPUTE.md (К9 + G0–G9)**: **15-25 weeks** для full architectural foundation." | M9.0-M9.8 → TBD |
| `docs/architecture/KERNEL_ARCHITECTURE.md:611` | "**Combined с RUNTIME_ARCHITECTURE.md M9.0-M9.8 + GPU_COMPUTE.md G0-G9**: 16-25 weeks total для full architectural foundation." | M9.0-M9.8 → TBD |
| `docs/architecture/KERNEL_ARCHITECTURE.md:808` | "Mirrors RUNTIME_ARCHITECTURE.md migration approach (parallel Godot + Vulkan until M9.5 cutover)." | M9.5 → TBD |
| `docs/architecture/KERNEL_ARCHITECTURE.md:1036, 1044, 1051, 1053` | Combined-timeline diagrams: "→ M9.0-M9.8 (5-7w Vulkan complete)" / "→ M9.0-M9.5 (4-5w Vulkan parity)" / "→ M9.6-M9.8 (1-2w Vulkan finish)" | M9.x → TBD |
| `docs/architecture/KERNEL_ARCHITECTURE.md:1065, 1071, 1077, 1083` | Sequencing-option formulas referencing M9.0-M9.8 / M9.0-M9.4 | M9.x → TBD |
| `docs/ROADMAP.md:49` | "\| **M9.0–M9.8 — Vulkan + Win32 runtime** \| ⏭ Pending \| — \| Per [RUNTIME_ARCHITECTURE] v1.0 LOCKED, parallel-development cutover at M9.5, Godot deletion at M9.8 \|" | M9.0–M9.8 / M9.5 / M9.8 → TBD |
| `docs/ROADMAP.md:50` | "\| Phase 9 — Native Runtime \| ⏭ Post-launch \| — \| Separate large project (now decomposed into K-series + M9.x runtime above) \|" | M9.x → TBD |
| `docs/README.md:29` | "Nine migration milestones (M9.0–M9.8), parallel-development cutover at M9.5, Godot deletion at M9.8." | M9.0–M9.8 / M9.5 / M9.8 → TBD |
| `docs/architecture/GPU_COMPUTE.md:753` | "K9 + G0–G5 ≈ 6–9 weeks for foundational fields and Domain B reactivation. G6–G9 (flow field pathfinding overhaul) adds ~3–4 weeks. Combined with kernel (K0–K8) and runtime (M9.0–M9.8) pivots, the full architectural vision is 16–25 weeks." | M9.0–M9.8 → TBD |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:29` | "After this document is locked, individual milestone briefs (K8.2, K8.3, K8.4, K8.5, M8.4, M8.5-M8.7, M9, M10.x) are authored against its constraints." | Mixed; M-cycle M9 (Q-R-2 collision); M9 → TBD; M8.4 / M8.5-M8.7 / M10.x → M-K bucket per Q-M-2 |
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md:17` | "**Prerequisites**: K9 closed; M9.0–M9.4 closed (Vulkan instance/device live)" | M9.0–M9.4 → TBD |
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md:44` | "Awaiting K9 closed; M9.0–M9.4 closed." | M9.0–M9.4 → TBD |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:1149-1150` (referenced indirectly) | Lines referenced by K-L3.1 amendment plan; not direct M9.x ref | n/a |

#### 4.3.2 — M-cycle (M0..M10.x) references — mod-OS migration sequence

Per-file count of M-cycle references (M0..M10.x; excludes M9.x runtime which is §4.3.1):

| File | M-cycle refs (approx) | Status of refs | Rename intent |
|---|---|---|---|
| `docs/audit/M7_CLOSURE_REVIEW.md` | 252 | M7 closure review — historical record | Q-M-1 deliberation: leave or back-rename |
| `docs/audit/M6_CLOSURE_REVIEW.md` | 173 | Historical | Q-M-1 |
| `docs/prompts/M7_CLOSURE.md` | 145 | Historical brief | Q-M-1 |
| `docs/audit/M5_CLOSURE_REVIEW.md` | 131 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_3_ROADMAP_REALITY.md` | 129 | Historical audit | Q-M-1 |
| `docs/ROADMAP.md` | 116 | Active ROADMAP — M0..M10 sequence | Mixed: M0..M7 historical (Q-M-1); M8.x / M9 / M10.x pending content (Q-M-2 → M-K) |
| `docs/audit/M4_CLOSURE_REVIEW.md` | 91 | Historical | Q-M-1 |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | 72 | Live LOCKED v1.2 — central M-cycle source | Most refs to M8.4 / M8.5-M8.7 / M9 / M10.x → M-K candidates (Q-M-2) |
| `docs/audit/AUDIT_PASS_3_PROMPT.md` | 64 | Historical | Q-M-1 |
| `docs/audit/M3_CLOSURE_REVIEW.md` | 63 | Historical | Q-M-1 |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | ~50 (excluding M9.x runtime) | LOCKED v1.7 — version history and §11 phases table | Mixed: M0..M7 closed (Q-M-1); M3.5 deferred (Q-M-3); M9, M10 vanilla (Q-M-2) |
| `docs/prompts/M75B1_BOOTSTRAP_INTEGRATION.md` | 47 | Historical brief | Q-M-1 |
| `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | 46 | Historical | Q-M-1 |
| `docs/prompts/M75A_MOD_MENU_CONTROLLER.md` | 45 | Historical brief | Q-M-1 |
| `docs/audit/AUDIT_PASS_1_INVENTORY.md` | 45 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_2_SPEC_CODE.md` | 41 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_5_TRIAGE.md` | 39 | Historical | Q-M-1 |
| `docs/MIGRATION_PROGRESS.md` | 38 (excl M9.x) | Active tracker | Q-M-2 for pending; Q-M-1 for closed |
| `docs/prompts/M75B2_GODOT_UI_SCENE.md` | 36 | Historical | Q-M-1 |
| `tools/briefs/K6_MOD_REBUILD_BRIEF.md` | 31 | K6 brief; cross-refs M-cycle | Q-M-2 for pending refs |
| `docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md` | 26 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_2_PROMPT.md` | 26 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_5_PROMPT.md` | 24 | Historical | Q-M-1 |
| `docs/prompts/HOUSEKEEPING_MENU_PAUSES_SIMULATION.md` | 22 | Historical | Q-M-1 |
| `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` | 21 | Live amendment plan | Q-M-2 |
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` | 20 (excl M9.x) | Recent audit | Records as-is |
| `docs/audit/AUDIT_PASS_2_RESUMPTION_PROMPT.md` | 20 | Historical | Q-M-1 |
| `docs/prompts/M7_HOUSEKEEPING_TICK_DISPLAY.md` | 19 | Historical | Q-M-1 |
| `docs/methodology/PIPELINE_METRICS.md` | 19 | Historical metrics | Q-M-1 |
| `docs/audit/AUDIT_PASS_1_PROMPT.md` | 19 | Historical | Q-M-1 |
| `docs/prompts/HOUSEKEEPING_MENU_POSITION_AND_ASSETS_GITIGNORE.md` | 18 | Historical | Q-M-1 |
| `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` | 16 | K8.3 brief | Records as-is; Q-M-2 for pending |
| `docs/audit/UI_REVIEW_PRE_M75B2.md` | 15 | Historical | Q-M-1 |
| `docs/audit/AUDIT_PASS_4_CROSSDOC_TRANSLATION.md` | 15 | Historical | Q-M-1 |
| `docs/prompts/TD1_SONNET_BRIEF.md` | 14 | Historical | Q-M-1 |
| `docs/architecture/KERNEL_ARCHITECTURE.md` | 13 (excl M9.x) | LOCKED — refs M3.5, M-cycle | Mixed |
| `docs/audit/AUDIT_REPORT.md` | 12 | Historical | Q-M-1 |
| `docs/prompts/M73_CODING_STANDARDS_UPDATE.md` | 11 | Historical | Q-M-1 |
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | 11 | Live reference doc | Mixed: M3.4 deferred, M-cycle pending |
| Other docs/audit/, docs/prompts/, etc. | < 10 each, ~150 cumulative | Mostly historical | Q-M-1 |

**Critical verbatim citations — M-cycle pending vanilla content (rename → M-K bucket per Q-M-2):**

| File:line | Verbatim context | Proposed new ID |
|---|---|---|
| `docs/ROADMAP.md:43` | "\| M8 — Vanilla skeletons \| ⏭ Pending \| — \| Five empty mod assemblies \|" | M8 → TBD: M-K8 candidate |
| `docs/ROADMAP.md:44` | "\| M9 — Vanilla.Combat \| ⏭ Pending \| — \| Absorbs original Phase 5 scope \|" | M9 (M-cycle, **not runtime**) → TBD: M-K9 candidate. Resolves Q-R-2 collision. |
| `docs/ROADMAP.md:45` | "\| M10 — Remaining vanilla \| ⏭ Pending \| — \| Magic, Inventory, Pawn, World — incremental \|" | M10 → TBD: M-K10 candidate |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:62` | "  M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x" (sequence diagram) | All M-cycle nodes → M-K bucket; per-format TBD |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:98` | "**Sequence LOCKED**: K8.2 → K8.3 → K8.4 → K8.5 → M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x." | Mixed: K-series stays; M-cycle → M-K |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:110` | "**M-series unchanged structurally LOCKED**: M8.4, M8.5-M8.7, M9, M10.x sequence preserves `MOD_OS_ARCHITECTURE.md` §11." | All five M-cycle IDs → M-K (Q-M-2) |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:111-116` | Vanilla→milestone mapping: "- Vanilla.World → **M8.4** / - Vanilla.Pawn → **M8.5-M8.7** / - Vanilla.Combat → **M9** / - Vanilla.Inventory → **M10** / - Vanilla.Magic → **M10.B** / - Vanilla.Core → **M10 incremental**" | Six per-mod assignments; all → M-K candidates |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:282` | "Section 1 closed Phase A. M-series begins from the resulting `src/` state." | M-series → M-K series |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:284` | "### 2.1 — Inputs to M8.4 from K8.5" | M8.4 → M-K candidate |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:286` | "**Codebase state at K8.5 close** (M8.4 brief authoring time):" | M8.4 → M-K candidate |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:331` | "### 3.1 — M8.4: Vanilla.World" | M8.4 → M-K candidate |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:337` | "**Authority**: vanilla mod doc-comment (`WorldMod.cs`): «content lands in M8.4 (Item factory + 4 entity types)»." | M8.4 → M-K candidate (also doc-comment in `mods/.../WorldMod.cs`) |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:343` | "### 3.2 — M8.5-M8.7: Vanilla.Pawn (3 sub-milestones)" | M8.5-M8.7 → M-K candidates |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:349` | "**Authority**: vanilla mod doc-comment (`PawnMod.cs`): «content lands in M8.5–M8.7 (ConsumeSystem / SleepSystem / ComfortAuraSystem)»." | M8.5–M8.7 → M-K candidates |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:351` | "**Sub-milestone split** (this plan locks the structure; M8.5 brief authors specifics):" | M8.5 → M-K candidate |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:230` | "**`KernelCapabilityRegistry`** finalized (per M3.5 deferred from MOD_OS_ARCHITECTURE §11.1, unblocked at K9 in-progress per v1.6)" | M3.5 (deferred) → Q-M-3 |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:252` | "**`MIGRATION_PROGRESS.md` Phase B preparation section**: skeleton tracking entries for M8.4, M8.5-M8.7, M9, M10.x." | All four M-cycle IDs → M-K candidates |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:20` | "Every architectural decision in this document is final input to all subsequent migration phases (M1–M10, K9, G0–G9, see §11)." | Mixed series; M1–M10 → some M-K, some Q-M-1 (closed) |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:45` | "v1.6 — ratification of GPU compute integration capabilities, gating K9 (field storage abstraction) and G0–G9 (Vulkan compute integration) milestones..." | n/a (G0-G9 stays G per Q-G-2; K9 stays K) |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:51` | "§11.1: M3.5 added as deferred milestone — capability registry refresh for field types..." | M3.5 → Q-M-3 |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:563` | "The v3 properties default to null on builds that do not include K9/G-series support; v2 mods never observe them. v1 mods (no-op `Publish`/`Subscribe`) continue under the §4.5 grace period." | n/a (K9, G-series stay) |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:1032` | "**M3.5** *(deferred)* \| Capability registry refresh for field types via `[FieldAccessible]` scan extension (per v1.6 §3.5)..." | M3.5 → Q-M-3 |
| `docs/architecture/MOD_OS_ARCHITECTURE.md:1053-1058` | ValidationErrorKind entries: "`FieldRegistrationConflict` (K9) — ... `InvalidFieldDimensions` (K9) ... `FieldCapabilityMismatch` (K9) ... `ComputePipelineCompilationFailed` (G0) ... `ComputePipelineRegistrationConflict` (G0) ... `ComputeUnsupportedWarning` (G0)" | K9 stays; G0 → TBD per Q-G-1 |

**M-cycle closed-phase refs (M0..M7) — Q-M-1 ratification decision required:**

The closure reviews under `docs/audit/M[3-7]_CLOSURE_REVIEW.md` (total ~710 refs) and historical prompts under `docs/prompts/M*.md` (total ~360 refs) document already-shipped milestones. The proposal must decide:

- (a) Leave M0..M7 closure documents at their closure-time names. Rationale: historical record of what shipped; the names are pinned by closure commits and changing them retroactively rewrites history.
- (b) Back-rename to `M-K0..M-K7`. Rationale: consistency with the composite namespace; the 10 production systems on `NativeWorld` retroactively belong to M-K per deliberation, suggesting the closed M-cycle also bucketizes as M-K.

This map records the references; the proposal answers.

#### 4.3.3 — Vanilla.X mod-name references (mod-cycle affiliation carrier)

`Vanilla.X` names (`Vanilla.Combat`, `Vanilla.Core`, `Vanilla.Inventory`, `Vanilla.Magic`, `Vanilla.Movement`, `Vanilla.Pawn`, `Vanilla.World`, `Vanilla.Electricity`, `Vanilla.Water`) carry implicit milestone affiliation. The names themselves are **type/namespace names** in the codebase (`mods/DualFrontier.Mod.Vanilla.Combat/`, etc.) and are not milestone IDs per se — they map to milestones via doc-comments and architectural docs. Per Q-V-1, the ratification proposal must clarify whether the rename touches these mod-name references or only their adjacent milestone-ID labels.

Per-file count of `Vanilla.X` mentions:

| File | Vanilla.X refs | Cascade impact |
|---|---|---|
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | 35 | Central mod→milestone assignment site |
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` | 26 | Recent recon; records as-is |
| `docs/ROADMAP.md` | 26 | Public mod→milestone view |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | 14 | §2.1 example manifest, §3.5 capability surface |
| `docs/architecture/GPU_COMPUTE.md` | 11 | G-series targets: Vanilla.Magic, Vanilla.Electricity, Vanilla.Water, Vanilla.Movement |
| `docs/audit/M7_CLOSURE_REVIEW.md` | 8 | Historical |
| `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF.md` | 6 | K8.3+K8.4 cutover targets |
| `docs/governance/REGISTER_RENDER.md` | 6 | Register rendered view |
| `docs/governance/REGISTER.yaml` | 6 | Register entries (DOC-F-MODS-VANILLA-*) |
| `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` | 4 | K-L3.1 references mod boundaries |
| `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` | 3 | Migration brief |
| `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | 2 | K9 references Vanilla.Magic |
| `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md` | 2 | G7 brief — own ID |
| `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md` | 2 | G2 → Vanilla.Electricity |
| `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md` | 2 | G1 → Vanilla.Magic |
| `docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md` | 2 | Historical |
| Other files | < 2 each, ~30 cumulative | |
| **Code** (`src/`, `tests/`, `mods/`) | ~14 `Vanilla.X` C# refs + 5 `DualFrontier.Mod.Vanilla.*` namespace refs | Mostly type names, not milestone IDs |
| **Mod READMEs** (`mods/.../README.md`) | 6 README files, 4-5 milestone refs each | doc-comments tie mod to M-cycle milestone |

**Critical verbatim citations — Vanilla.X mod-name → milestone mapping:**

| File:line | Verbatim context | Mapping note |
|---|---|---|
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:111-116` | "- Vanilla.World → **M8.4** / Vanilla.Pawn → **M8.5-M8.7** / Vanilla.Combat → **M9** / Vanilla.Inventory → **M10** / Vanilla.Magic → **M10.B** / Vanilla.Core → **M10 incremental**" | Six M-cycle slots; all → M-K candidates |
| `docs/architecture/GPU_COMPUTE.md:613-735` (G0..G9 roadmap section) | "G1 — First field compute shader (mana diffusion) — `Vanilla.Magic` mod registers..." / "G2 — Anisotropic diffusion (electricity) — `Vanilla.Electricity` mod..." / "G4 — Multi-field coexistence — `Vanilla.Magic` + `Vanilla.Electricity` + `Vanilla.Water` all active..." / "G7 — `Vanilla.Movement` integration" | Maps G-series substrate to vanilla content; G7 → M-G7 locked; others remain G substrate (Q-G-2) |
| `mods/DualFrontier.Mod.Vanilla.Combat/README.md:4` | "Vanilla Combat mod — empty M8 skeleton. Establishes the assembly," | M8 → M-K8 candidate |
| `mods/DualFrontier.Mod.Vanilla.Combat/README.md:7` | "place. Content migration into this mod happens in M9 Combat (Faction" | M9 (M-cycle) → M-K9 candidate |
| `mods/DualFrontier.Mod.Vanilla.Combat/README.md:17, 20` | "`CombatMod.cs` — empty IMod implementation; content lands in M9." / "M8.1 skeleton — content empty. Migration target: M9 Combat." | M9 → M-K9 candidate |
| `mods/DualFrontier.Mod.Vanilla.Core/README.md:4, 18` | "Vanilla shared mod — empty M8 skeleton..." / "Migration target: M10 incremental shared types." | M8, M10 → M-K candidates |
| `mods/DualFrontier.Mod.Vanilla.Inventory/README.md:4, 20` | "Vanilla Inventory mod — empty M8 skeleton..." / "Migration target: M10 Inventory." | M8, M10 → M-K candidates |
| `mods/DualFrontier.Mod.Vanilla.Magic/README.md:4, 19` | "Vanilla Magic mod — empty M8 skeleton..." / "Migration target: M10.B Magic." | M8, M10.B → M-K candidates |
| `mods/DualFrontier.Mod.Vanilla.Pawn/README.md:4, 7, 17, 20` | "Vanilla Pawn mod — empty M8 skeleton..." / "Content migration into this mod happens across M8.5–M8.7" / "content lands in M8.5–M8.7." / "Migration target: M8.5–M8.7" | M8, M8.5–M8.7 → M-K candidates |
| `mods/DualFrontier.Mod.Vanilla.World/README.md:4, 7, 17, 20` | "Vanilla World mod — empty M8 skeleton..." / "Content migration into this mod happens in M8.4 (Item factory + 4" / "content lands in M8.4." / "Migration target: M8.4 Item factory + 4" | M8, M8.4 → M-K candidates |
| `mods/DualFrontier.Mod.Vanilla.Inventory/InventoryMod.cs:6-18` | "/// Vanilla Inventory mod skeleton. Currently empty — content lands in M10" / "/// in M10." / "/// registered in the M8 skeleton. Content lands in M10." | M8, M10 in code-doc comments |
| `mods/DualFrontier.Mod.Vanilla.Combat/CombatMod.cs:6-18` | "/// Vanilla Combat mod skeleton. Currently empty — content lands in M9 Combat" / "/// happens in M9." / "/// registered in the M8 skeleton. Content lands in M9." | M8, M9 in code-doc comments |
| `mods/DualFrontier.Mod.Vanilla.Magic/MagicMod.cs:6-17` | "/// Vanilla Magic mod skeleton... content lands in M10.B Magic" / "/// in M10.B." / "/// registered in the M8 skeleton. Content lands in M10.B." | M8, M10.B in code-doc comments |
| `mods/DualFrontier.Mod.Vanilla.Pawn/PawnMod.cs:7` | "/// M8.5–M8.7 (ConsumeSystem / SleepSystem / ComfortAuraSystem) per the" | M8.5–M8.7 in code-doc comment |

### 4.4 — G-series references (G7/G8 LOCKED rename → M-G; others Q-G-1/Q-G-2)

G-series IDs (`G0..G9`) appear 280 times across 32 files. Two cases are **locked** by deliberation; the rest are **TBD** pending Q-G-1 and Q-G-2.

**Locked G-rename cases (per 2026-05-15 deliberation):**

| Current ID | Locked rename | File defining ID | Verbatim definition |
|---|---|---|---|
| **G7** | **M-G7** | `docs/architecture/GPU_COMPUTE.md:708` | "### G7 — `Vanilla.Movement` integration (~1 week)" |
| **G7** (brief) | **M-G7** | `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md:13` | "# G7 — `Vanilla.Movement` integration" |
| **G8** | **M-G8** | `docs/architecture/GPU_COMPUTE.md:722` | "### G8 — Local avoidance layer (~3–5 days)" |
| **G8** (brief) | **M-G8** | `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md:13` | "# G8 — Local avoidance layer" |

**TBD G-series cases (Q-G-1 for G0; Q-G-2 for G1..G6, G9):**

| Current ID | File defining ID | Verbatim definition | Proposed |
|---|---|---|---|
| G0 | `docs/architecture/GPU_COMPUTE.md:613` | "### G0 — Vulkan compute pipeline plumbing (~1 week)" | TBD per Q-G-1 (G0 vs R0) |
| G0 (brief) | `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md:13` | "# G0 — Vulkan compute pipeline plumbing" | TBD per Q-G-1 |
| G1 | `docs/architecture/GPU_COMPUTE.md:629` | "### G1 — First field compute shader (mana diffusion) (~1 week)" | TBD per Q-G-2 |
| G1 (brief) | `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md:13` | "# G1 — First field compute shader (mana diffusion)" | TBD per Q-G-2 |
| G2 | `docs/architecture/GPU_COMPUTE.md:642` | "### G2 — Anisotropic diffusion (electricity) (~1 week)" | TBD per Q-G-2 |
| G2 (brief) | `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md:13` | "# G2 — Anisotropic diffusion (electricity)" | TBD per Q-G-2 |
| G3 | `docs/architecture/GPU_COMPUTE.md:656` | "### G3 — Storage cells / capacitance (batteries, tanks) (~3–5 days)" | TBD per Q-G-2 |
| G3 (brief) | `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md:13` | "# G3 — Storage cells / capacitance (batteries, tanks)" | TBD per Q-G-2 |
| G4 | `docs/architecture/GPU_COMPUTE.md:668` | "### G4 — Multi-field coexistence (~3–5 days)" | TBD per Q-G-2 |
| G4 (brief) | `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md:13` | "# G4 — Multi-field coexistence" | TBD per Q-G-2 |
| G5 | `docs/architecture/GPU_COMPUTE.md:681` | "### G5+ — Domain B integration (`ProjectileSystem` reactivation) (~1 week)" | TBD per Q-G-2 |
| G5 (brief) | `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md:13` | "# G5 — Domain B integration (`ProjectileSystem` reactivation)" | TBD per Q-G-2 |
| G6 | `docs/architecture/GPU_COMPUTE.md:695` | "### G6 — Flow field infrastructure (~3–5 days)" | TBD per Q-G-2 |
| G6 (brief) | `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md:13` | "# G6 — Flow field infrastructure" | TBD per Q-G-2 |
| G9 | `docs/architecture/GPU_COMPUTE.md:735` | "### G9 — Eikonal upgrade (optional, ~1 week, evidence-gated)" | TBD per Q-G-2 |
| G9 (brief) | `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md:13` | "# G9 — Eikonal upgrade (optional, evidence-gated)" | TBD per Q-G-2 |

**Per-file G-series reference count (all 280 raw refs):**

| File | G-series refs | Notes |
|---|---|---|
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` | 107 | Recent recon — most refs are historical; records as-is |
| `docs/governance/REGISTER.yaml` | 22 | 10 DOC-D-G* entries + cross-refs |
| `docs/architecture/GPU_COMPUTE.md` | 15 | LOCKED v2.0 — canonical G-series source |
| `docs/ROADMAP.md` | 13 | Public roadmap §G-series row |
| `docs/governance/REGISTER_RENDER.md` | 10 | Register rendered view |
| `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | 8 | K9 references G-series prerequisites |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | 8 | §4.6 IModApi v3, §11.2 ValidationErrorKind G0 errors |
| `docs/architecture/FIELDS.md` | 7 | K9 + G-series infrastructure |
| `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md` | 6 | Own brief (LOCKED → M-G8) |
| `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md` | 6 | Own brief (LOCKED → M-G7) |
| `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md` | 6 | Own brief |
| `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md` | 6 | Own brief |
| `docs/architecture/KERNEL_ARCHITECTURE.md` | 6 | Cross-refs (e.g. K9 gates G-series) |
| `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` | 5 | Cross-refs |
| `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` | 4 | Amendment closure |
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md` | 4 | Own brief |
| `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md` | 3 | K8.0 ref to G-series |
| `docs/architecture/PERFORMANCE.md` | 3 | |
| `tools/briefs/K9_BRIEF_REFRESH_PATCH.md` | 2 | |
| `tools/briefs/A_PRIME_4_5_PASS_5_PRODUCTION_ENTRIES.md` | 2 | |
| `tools/briefs/MODULE.md`, `tools/briefs/K6_MOD_REBUILD_BRIEF.md`, `tools/briefs/A_PRIME_4_5_PASS_2_CLASSIFICATION_MODEL_LOCK.md`, `docs/methodology/METHODOLOGY.md`, `docs/governance/FRAMEWORK.md`, `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`, `docs/README.md` | 1 each | Minor refs |

**Cross-references — G-series referenced from non-G briefs (rename ordering input):**

| Source brief | G-series ref | Verbatim |
|---|---|---|
| `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` | G-series gating | K9 is gate for G-series; brief explicitly notes G0..G9 dependence |
| `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md` | G-series timing | Notes G-series follows K8 closure |
| `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md:17` | Prerequisite: G0 | "Prerequisites: G0 closed" |
| `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md:17` | Prerequisite: G1 | "Prerequisites: G1 closed" |
| `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md:17` | Prerequisite: G2 | "Prerequisites: G2 closed" |
| `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md` | Prerequisite: G3 | "Prerequisites: G3 closed" |
| `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md` | Prerequisite: G4 | "Prerequisites: G4 closed" |
| `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md` | Prerequisite: G5 (or G4) | "Prerequisites: G5 closed (or G4 closed if scheduling permits early start)" |
| `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md:17` | Prerequisite: G6 | "Prerequisites: G6 closed" |
| `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md:17` | Prerequisite: G7 | "Prerequisites: G7 closed" |
| `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md:17` | Prerequisite: G7 + measurement | "Prerequisites: G7 closed; measurement of Option B suboptimality" |
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md:17` | Cross-substrate: K9 + M9.0-M9.4 | "Prerequisites: K9 closed; M9.0–M9.4 closed (Vulkan instance/device live)" |

### 4.5 — Vanilla mod-name references (Q-V-1 / Q-V-2)

Already inventoried under §4.3.3 (mod-cycle affiliation). Total ~173 `Vanilla.X` references across docs/briefs + ~14 in code (C# namespaces) + ~5 mod-assembly project names + 6 mod-folder README files.

The vanilla mod-name map (mod → currently-assigned milestone):

| Mod (C# namespace) | Assigned milestone (current) | Bucket under composite namespace |
|---|---|---|
| `DualFrontier.Mod.Vanilla.World` | M8.4 | M-K (per Q-M-2) |
| `DualFrontier.Mod.Vanilla.Pawn` | M8.5-M8.7 | M-K (per Q-M-2) |
| `DualFrontier.Mod.Vanilla.Combat` | M9 (M-cycle, not runtime) | M-K (per Q-M-2); resolves Q-R-2 collision |
| `DualFrontier.Mod.Vanilla.Inventory` | M10 | M-K (per Q-M-2) |
| `DualFrontier.Mod.Vanilla.Magic` | M10.B (M-cycle); also G1 (mana diffusion compute, G substrate) | Multi-substrate per Q-V-2 |
| `DualFrontier.Mod.Vanilla.Core` | M10 incremental | M-K (per Q-M-2) |
| `Vanilla.Movement` (not yet authored) | G7 | M-G7 (LOCKED) |
| `Vanilla.Electricity` (not yet authored) | G2 | TBD per Q-G-2 (substrate G) — but content vanilla → M-G2 candidate per Q-V-2 |
| `Vanilla.Water` (not yet authored) | G4 (multi-field) | TBD per Q-G-2 / Q-V-2 |

---

## 5 — Reference Inventory by Artifact (orthogonal view)

Per-document summary of cascade impact. Counts are approximate per-series; see §4 for detailed line-citation tables.

| Document | Lifecycle | K | A' | M-cycle (M0..M10) | M9.x runtime | G | Vanilla.X | Cascade impact summary |
|---|---|---|---|---|---|---|---|---|
| `docs/architecture/KERNEL_ARCHITECTURE.md` | LOCKED v1.6 | ~140 | ~15 | ~13 | 11 | 6 | 1 | High — Part 2 status table is the central K/M/G status registry |
| `docs/architecture/MOD_OS_ARCHITECTURE.md` | LOCKED v1.7 | ~40 | ~5 | ~50 | 0 | 8 | 14 | High — §11 migration phases, §4.6 IModApi v3, version history exhaustively cites M0..M10 cascades |
| `docs/architecture/RUNTIME_ARCHITECTURE.md` | LOCKED v1.0 | ~15 | 0 | ~10 (M8/M10 cross-refs) | 38 | 0 | 0 | **Highest M9.x density** — canonical M9.0..M9.8 → R rename site |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | LOCKED v1.2 | ~110 | ~30 | 72 | 1 | 1 | 35 | **Highest M-cycle density** — central M-cycle nomenclature; primary cascade impact site |
| `docs/architecture/GPU_COMPUTE.md` | LOCKED v2.0 | ~25 | 0 | 1 | 1 | 15 | 11 | High — canonical G-series source, K9 + G0–G9 roadmap |
| `docs/architecture/FIELDS.md` | Live v0.1 | ~30 | 0 | ~5 | 0 | 7 | 2 | Medium — K9 + G-series storage contract |
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | Live | ~60 | ~110 | 11 | 0 | 0 | 1 | High A' density — canonical A'-series source |
| `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` | LOCKED | ~25 | ~10 | 21 | 0 | 5 | 2 | Medium |
| `docs/architecture/CONTRACTS.md`, `ECS.md`, `ISOLATION.md`, `THREADING.md`, `MOD_PIPELINE.md`, `EVENT_BUS.md`, `MODDING.md`, `ARCHITECTURE.md`, `PERFORMANCE.md`, `GODOT_INTEGRATION.md`, `VISUAL_ENGINE.md`, `FHE_INTEGRATION_CONTRACT.md`, `ARCHITECTURE_TYPE_SYSTEM.md`, `COMBO_RESOLUTION.md`, `COMPOSITE_REQUESTS.md`, `RESOURCE_MODELS.md`, `FEEDBACK_LOOPS.md`, `OWNERSHIP_TRANSITION.md`, `MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` | LOCKED / Live | 0-10 each | 0-2 each | 0-3 each | 0-3 each | 0-3 each | 0-2 each | Low — peripheral architecture docs, few milestone refs |
| `docs/methodology/METHODOLOGY.md` | LOCKED | ~50 | ~25 | 1 | 0 | 1 | 0 | High K-Lessons density (K-L1..K-L11) — unchanged; minimal M/G touch |
| `docs/methodology/CODING_STANDARDS.md`, `DEVELOPMENT_HYGIENE.md`, `MAXIMUM_ENGINEERING_REFACTOR.md`, `PIPELINE_METRICS.md`, `TESTING_STRATEGY.md` | LOCKED | varies | varies | varies | 0 | 0 | 0 | Low cascade impact |
| `docs/governance/FRAMEWORK.md` | LOCKED v1.0 | low | low | low | 0 | 1 | 0 | Low — schema spec |
| `docs/governance/SYNTHESIS_RATIONALE.md` | LOCKED v1.0 | low | low | low | 0 | 0 | 0 | Low |
| `docs/governance/REGISTER.yaml` | Live (Tier 2) | 31 DOC-D-K* entries + many cross-refs (~250 cumulative) | 12 DOC-D-A_PRIME* entries + EVT-*/REQ-* | ~25 (incl. M3.4/M3.5 deferred, M-prompts under DOC-E-*) | 0 | 22 (10 DOC-D-G* + cross-refs) | 6 (DOC-F-MODS-VANILLA-*) | High — register entries cascade rename. See §7. |
| `docs/governance/REGISTER_RENDER.md` | auto-gen | mirror of REGISTER | mirror | mirror | mirror | 10 | 6 | High — rendered view from REGISTER.yaml |
| `docs/ROADMAP.md` | Live | ~20 | low | 116 | 5 | 13 | 26 | **Public-facing density** — M0..M10 sequence table |
| `docs/IDEAS_RESERVOIR.md` | Live | 0 | 0 | M0–M10 callouts (~4) | 0 | 0 | 0 | Low — reservoir is post-shipping |
| `docs/MIGRATION_PROGRESS.md` | Live | many | many | 38 | 21 | low | 1 | High M9.x density |
| `docs/audit/M[3-7]_CLOSURE_REVIEW.md` (5 docs) | Closed records | low | 0 | ~710 cumulative | low | low | ~13 cumulative | Q-M-1 (historical record decision) |
| `docs/audit/AUDIT_PASS_*` (multiple) | Closed records | varies | varies | ~250 cumulative | varies | low | low | Q-M-1 |
| `docs/prompts/M*` (multiple) | Closed records | low | 0 | ~300 cumulative | 0 | 0 | low | Q-M-1 |
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` | Recent recon | high | high | 20 | 12 | 107 | 26 | Records as-is; informational |
| `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` | Live | many | 0 | low | 1 | 0 | 0 | Low |
| `docs/reports/NORMALIZATION_REPORT.md`, `NATIVE_CORE_EXPERIMENT.md`, `PERFORMANCE_REPORT_K3.md`, `PERFORMANCE_REPORT_K7.md` | Closed | varies | low | low | 0 | 0 | 0 | Low |
| `tools/briefs/G[0-9]_*_BRIEF.md` (10 G-series skeletons) | AUTHORED | 1-8 each (K9 prereq) | 0 | 0-2 each | 0-2 each (G0 only) | 4-8 each (own + prereqs) | 0-2 each | **G7 + G8 LOCKED renames; G0..G6/G9 TBD per Q-G-1/Q-G-2** |
| `tools/briefs/K[0-9]*_BRIEF.md` (31 K-series briefs) | varies | varies | varies | varies | 0-2 each | 0-3 each | 0-3 each | Cross-refs to M/G visible in K8.0, K8.34, K9 briefs |
| `tools/briefs/A_PRIME_*_BRIEF.md` (10 A'-series briefs) | varies | varies | varies | varies | 0 | 0 | 0 | A' refs internal |
| `tools/briefs/K_L3_1_*` (2 briefs) | AUTHORED | high | medium | ~25 | 0 | 5 | 4 | Medium |
| `tools/briefs/MOD_OS_V16_AMENDMENT_*` (2 briefs) | AUTHORED | medium | low | medium | 1 | 4 | 1 | Medium |
| `tools/briefs/MODULE.md` | F | low | 0 | 1 | 1 | 1 | 1 | Low |
| `mods/DualFrontier.Mod.Vanilla.*/README.md` (6 README files) | F (mod folders) | 0 | 0 | 4-5 each (M8/M9/M10/M10.B/M8.4/M8.5-M8.7) | 0 | 0 | own mod name | Medium — each ties mod assembly to current M-cycle ID |
| `mods/DualFrontier.Mod.Vanilla.*/*.cs` (6 mod skeleton files) | F (code) | 0 | 0 | 3-4 each in doc-comments | 0 | 0 | own type name | Low — but renamed if M-cycle migration rename touches doc-comments |
| `src/`, `tests/` code comments | F (code) | 188 | 4 | 173 | 0 | 5 | ~14 type-name refs | High volume; mostly historical (M0..M7) |
| `native/` code comments | F (code) | ~30 K-series | 0 | 0 | 0 | 0 | 0 | Low |

---

## 6 — Code Comment Inventory

Code comments in `src/`, `tests/`, `native/`, and `mods/` carry 509 grep matches against the milestone-ID pattern (`src/`+`tests/` C# only — `native/` adds ~30 K-series). Comments are sample-quoted grouped by ID, not exhaustively per line.

**Per-series totals (across `src/` + `tests/`):**

| Series pattern | Total comment refs | Action required by rename cascade |
|---|---|---|
| K-series (`K0..K9`, `K-L*`, `K8.x`) | 188 | None — K-series unchanged |
| M-series (`M0..M10`, `M3.x`, `M5.x`, `M6.x`, `M7.x`, `M8.x`) | 173 | Most are historical M0..M7 closure-shape refs (Q-M-1); some M8.x pending (Q-M-2) |
| M9.x runtime | 0 (no runtime code yet) | n/a until R substrate work begins |
| G-series | 5 | TBD per Q-G-1/Q-G-2 |
| A'-series | 4 | Unchanged |
| Phase N | 121 | Mixed — historical Phase 3/4/5 refs (closed); Phase 5 / Phase 9 future placeholders |

**Per-ID code comment counts (top counts):**

M-series:

| ID | Count | Notes |
|---|---|---|
| M7.5 | 32 | M7.5.A / M7.5.B / etc. — hot reload sub-phases |
| M5.1 | 20 | Pipeline regular-mod toposort |
| M7.3 | 18 | ALC step 7 + GC pump |
| M72 | 17 | M7.2 short-form |
| M73 | 16 | M7.3 short-form |
| M7.2 | 16 | ALC unload chain steps 1-6 |
| M7.1 | 12 | Pause/Resume |
| M6.2 | 9 | Bridge replacement integration tests |
| M71 | 8 | M7.1 short-form |
| M5.2 | 7 | Phase A/G modernization |
| M6, M62, M0, M8, M8.5, M8.8, M4.3, M5, M7.4 | 4-6 each | Various |
| M8.4, M8.2, M8.10, M6.1, M3, M2, M7, M51, M8.6, M74 | 2-3 each | Various |
| M9 | 1 | M-cycle Vanilla.Combat ref (Q-R-2 collision case) |

K-series (unchanged, listed for completeness):

| ID | Count |
|---|---|
| K8.4 | 58 |
| K8.3 | 58 |
| K7 | 26 |
| K8.2 | 22 |
| K4 | 16 |
| K9 | 14 |
| K6 | 10 |
| K6.1 | 9 |
| K5, K1 | 8 |
| K8.1, K3, K2 | 7 |
| K8, K0 | 3-4 |

**Highest-density files (≥10 milestone-mentioning comments):**

| File | Count | Notes |
|---|---|---|
| `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` | 29 | M-series (M5/M6) + K-series cascades |
| `tests/DualFrontier.Modding.Tests/Bootstrap/GameBootstrapIntegrationTests.cs` | 15 | M7.5 hot-reload tests |
| `tests/DualFrontier.Modding.Tests/Pipeline/M72UnloadChainTests.cs` | 14 | M7.2 ALC chain |
| `src/DualFrontier.Core.Interop/NativeWorld.cs` | 14 | K-series + K9 field registry |
| `tests/DualFrontier.Core.Benchmarks/TickLoop/V3NativeBatchedScenario.cs` | 13 | K-series scenario tags |
| `src/DualFrontier.Application/Bootstrap/VanillaComponentRegistration.cs` | 13 | K8.2 + M-series + Vanilla.* |
| `tests/DualFrontier.Modding.Tests/Pipeline/ModFaultHandlerTests.cs` | 10 | M6.1/M7 |
| `tests/DualFrontier.Core.Interop.Tests/VanillaComponentRoundTripTests.cs` | 10 | K8.x + Vanilla.* |
| `tests/DualFrontier.Core.Benchmarks/TickLoop/V1ManagedCurrentScenario.cs` | 10 | K-series |
| `src/DualFrontier.Core.Interop/Bootstrap.cs` | 10 | K-series |

**Sample verbatim code comments (grouped by ID):**

| File:line | Comment text |
|---|---|
| `src/DualFrontier.Core.Interop/NativeWorld.cs:112` | `/// K9 field registry. Field storage is orthogonal to entity-component` |
| `native/DualFrontier.Core.Native/src/world.cpp:78` | `// K5 — internal path used by WriteBatch::flush(). The active_batches_` |
| `native/DualFrontier.Core.Native/src/world.cpp:461` | `// ---- K8.1 reference primitives ---------------------------------------------` |
| `native/DualFrontier.Core.Native/include/sparse_set.h:3` | `// NOTE (K0 cleanup, 2026-05-07): This template is currently used only for` |
| `native/DualFrontier.Core.Native/src/capi.cpp:66` | `// K1: destroy_entity now throws if spans are active. Swallow to keep` |
| `mods/DualFrontier.Mod.Vanilla.Combat/CombatMod.cs:6` | `/// Vanilla Combat mod skeleton. Currently empty — content lands in M9 Combat` |
| `mods/DualFrontier.Mod.Vanilla.Pawn/PawnMod.cs:7` | `/// M8.5–M8.7 (ConsumeSystem / SleepSystem / ComfortAuraSystem) per the` |
| `mods/DualFrontier.Mod.Vanilla.Inventory/InventoryMod.cs:6` | `/// Vanilla Inventory mod skeleton. Currently empty — content lands in M10` |
| `mods/DualFrontier.Mod.Vanilla.Magic/MagicMod.cs:6` | `/// Vanilla Magic mod skeleton. Currently empty — content lands in M10.B Magic` |

Code comment renames are **downstream of the document rename**. The ratification proposal's cascade execution step (separate milestone) updates code comments after the doc cascade lands.

---

## 7 — REGISTER Inventory

Every `REGISTER.yaml` entry whose `id` contains M/G/R/M9 patterns. Format: `register_id | path | lifecycle | rename impact`.

### 7.1 — Category-D milestone briefs

| register_id | Path | Lifecycle | Rename impact |
|---|---|---|---|
| `DOC-D-K0` through `DOC-D-K9` + `DOC-D-K8_0`..`K8_5`, `K8_34*`, `K8_DECISION`, `K_L3_1_*`, `K_LESSONS_BATCH` | `tools/briefs/K*_BRIEF.md` (31 total) | varies (EXECUTED / AUTHORED / CLOSED) | Unchanged. K-series stays. |
| `DOC-D-A_PRIME_0_5_REORG_REFRESH` through `DOC-D-A_PRIME_4_5_PASS_5` (12 entries) | `tools/briefs/A_PRIME_*_BRIEF.md` | varies | Unchanged. A'-series stays. |
| `DOC-D-G0` | `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md` | AUTHORED | TBD per Q-G-1 (G0 substrate identity). If G stays → register entry unchanged. If G0 → R0 → register entry renamed. |
| `DOC-D-G1` | `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G2` | `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G3` | `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G4` | `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G5` | `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G6` | `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md` | AUTHORED | TBD per Q-G-2 |
| `DOC-D-G7` | `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md` | AUTHORED | **LOCKED**: `DOC-D-G7` → `DOC-D-M_G7` (or similar — register entry renamed per LOCKED rename G7 → M-G7) |
| `DOC-D-G8` | `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md` | AUTHORED | **LOCKED**: `DOC-D-G8` → `DOC-D-M_G8` (or similar) |
| `DOC-D-G9` | `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md` | AUTHORED | TBD per Q-G-2 |

### 7.2 — Category-E closure reports & audit prompts containing M-series

| register_id | Path | Lifecycle | Rename impact |
|---|---|---|---|
| `DOC-E-M3_CLOSURE_REVIEW` | `docs/audit/M3_CLOSURE_REVIEW.md` | Closed | Q-M-1 (historical) |
| `DOC-E-M4_CLOSURE_REVIEW` | `docs/audit/M4_CLOSURE_REVIEW.md` | Closed | Q-M-1 |
| `DOC-E-M5_CLOSURE_REVIEW` | `docs/audit/M5_CLOSURE_REVIEW.md` | Closed | Q-M-1 |
| `DOC-E-M6_CLOSURE_REVIEW` | `docs/audit/M6_CLOSURE_REVIEW.md` | Closed | Q-M-1 |
| `DOC-E-M7_CLOSURE_REVIEW` | `docs/audit/M7_CLOSURE_REVIEW.md` | Closed | Q-M-1 |
| `DOC-E-M7_CLOSURE` | `docs/prompts/M7_CLOSURE.md` | Closed | Q-M-1 |
| `DOC-E-M7_HOUSEKEEPING_TICK_DISPLAY` | `docs/prompts/M7_HOUSEKEEPING_TICK_DISPLAY.md` | Closed | Q-M-1 |
| `DOC-E-M73_CODING_STANDARDS_UPDATE` | `docs/prompts/M73_CODING_STANDARDS_UPDATE.md` | Closed | Q-M-1 |
| `DOC-E-M74_BUILD_PIPELINE_OVERRIDE` | `docs/prompts/M74_BUILD_PIPELINE_OVERRIDE.md` | Closed | Q-M-1 |
| `DOC-E-M75A_MOD_MENU_CONTROLLER` | `docs/prompts/M75A_MOD_MENU_CONTROLLER.md` | Closed | Q-M-1 |
| `DOC-E-M75B1_BOOTSTRAP_INTEGRATION` | `docs/prompts/M75B1_BOOTSTRAP_INTEGRATION.md` | Closed | Q-M-1 |
| `DOC-E-M75B2_GODOT_UI_SCENE` | `docs/prompts/M75B2_GODOT_UI_SCENE.md` | Closed | Q-M-1 |
| `DOC-E-UI_REVIEW_PRE_M75B2` | `docs/audit/UI_REVIEW_PRE_M75B2.md` | Closed | Q-M-1 |
| `DOC-E-SESSION_PHASE_4_CLOSURE_REVIEW` | (closed phase 4 review) | Closed | Q-M-1 |
| `DOC-E-AUDIT_PASS_*` (multiple) | `docs/audit/AUDIT_PASS_*` | Closed | Q-M-1 (historical M-cycle references) |
| `DOC-E-AUDIT_REPORT` | `docs/audit/AUDIT_REPORT.md` | Closed | Q-M-1 |
| `DOC-E-AUDIT_CAMPAIGN_PLAN` | `docs/audit/AUDIT_CAMPAIGN_PLAN.md` | Closed | Q-M-1 |

### 7.3 — Category-A LOCKED docs (cascade target via doc bodies, not IDs)

| register_id | Path | Lifecycle | Rename impact on register entry |
|---|---|---|---|
| `DOC-A-KERNEL` | `docs/architecture/KERNEL_ARCHITECTURE.md` | LOCKED v1.6 | Register id unchanged; doc body cascade (see §4) |
| `DOC-A-MOD_OS` | `docs/architecture/MOD_OS_ARCHITECTURE.md` | LOCKED v1.7 | Register id unchanged; doc body cascade |
| `DOC-A-RUNTIME` | `docs/architecture/RUNTIME_ARCHITECTURE.md` | LOCKED v1.0 | Register id unchanged; doc body cascade (heaviest M9.x → R rename load) |
| `DOC-A-MIGRATION_PLAN` | `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | LOCKED v1.2 | Register id unchanged; doc title contains "K-series → M-series" — may need title rename to "K-series → M-K series" or analog |
| `DOC-A-GPU_COMPUTE` | `docs/architecture/GPU_COMPUTE.md` | LOCKED v2.0 | Register id unchanged; doc body cascade for G-series renames (G7/G8 LOCKED) |
| `DOC-A-FIELDS` | `docs/architecture/FIELDS.md` | Live v0.1 | Register id unchanged; doc body cascade |
| `DOC-A-PHASE_A_PRIME_SEQUENCING` | `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | Live | Register id unchanged; doc body cascade for M-cycle refs |

### 7.4 — Category-C live planning

| register_id | Path | Lifecycle | Rename impact |
|---|---|---|---|
| `DOC-C-ROADMAP` | `docs/ROADMAP.md` | Live | High cascade — public M-cycle table |
| `DOC-C-MIGRATION_PROGRESS` | `docs/MIGRATION_PROGRESS.md` | Live | High cascade — pending closure entries reference M-cycle |
| `DOC-C-IDEAS_RESERVOIR` | `docs/IDEAS_RESERVOIR.md` | Live v0.1 | Low cascade — few M-cycle callouts |

### 7.5 — Category-F module READMEs

| register_id | Path | Rename impact |
|---|---|---|
| `DOC-F-MODS-VANILLA-COMBAT` | `mods/DualFrontier.Mod.Vanilla.Combat/README.md` | M-cycle refs (M8, M9) — Q-M-2 |
| `DOC-F-MODS-VANILLA-CORE` | `mods/DualFrontier.Mod.Vanilla.Core/README.md` | M-cycle refs (M8, M10) — Q-M-2 |
| `DOC-F-MODS-VANILLA-INVENTORY` | `mods/DualFrontier.Mod.Vanilla.Inventory/README.md` | M-cycle refs (M8, M10) — Q-M-2 |
| `DOC-F-MODS-VANILLA-MAGIC` | `mods/DualFrontier.Mod.Vanilla.Magic/README.md` | M-cycle refs (M8, M10.B) — Q-M-2 |
| `DOC-F-MODS-VANILLA-PAWN` | `mods/DualFrontier.Mod.Vanilla.Pawn/README.md` | M-cycle refs (M8, M8.5-M8.7) — Q-M-2 |
| `DOC-F-MODS-VANILLA-WORLD` | `mods/DualFrontier.Mod.Vanilla.World/README.md` | M-cycle refs (M8, M8.4) — Q-M-2 |

---

## 8 — Cross-Reference Graph (high-level)

Directed reference flow surfaces tight clusters where rename ordering matters.

**Cluster A — Substrate definition (must rename before downstream cascades)**

```
RUNTIME_ARCHITECTURE.md (M9.0..M9.8 definition site, LOCKED v1.0)
  ↓ referenced by
KERNEL_ARCHITECTURE.md       — combined-timeline diagrams
GPU_COMPUTE.md                — K9 + G-series → runtime cross-ref
G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md  — prerequisite M9.0–M9.4
ROADMAP.md                    — public M9.0–M9.8 row
docs/README.md                — top-level pointer
MIGRATION_PROGRESS.md         — Phase B tracker
```

Rename ordering: `RUNTIME_ARCHITECTURE.md` (Q-R-1 format-lock) → all downstream M9.x refs cascade together.

**Cluster B — M-cycle definition (must rename before downstream cascades)**

```
MIGRATION_PLAN_KERNEL_TO_VANILLA.md (M-cycle nomenclature LOCKED v1.2)
  ↓ referenced by
ROADMAP.md                       — M0..M10 sequence table
MOD_OS_ARCHITECTURE.md §11       — phases table
MIGRATION_PROGRESS.md            — Phase B preparation section
mods/.../README.md (×6)           — per-mod milestone target
mods/.../*Mod.cs (×5)             — per-mod doc-comments
```

Rename ordering: `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (Q-M-2 format-lock) → ROADMAP / MOD_OS §11 / mod READMEs cascade together. Code doc-comments are downstream cascade-execution step (separate milestone).

**Cluster C — G-series definition (G7/G8 LOCKED; others TBD)**

```
GPU_COMPUTE.md §G-series Roadmap (LOCKED v2.0)
  ↓ referenced by
G0..G9 skeleton briefs (10 briefs)  — each cites own ID + prereq chain
KERNEL_ARCHITECTURE.md              — K9 gates G-series
MOD_OS_ARCHITECTURE.md §4.6/§11.2   — IModApi v3, G0 ValidationErrorKind
FIELDS.md                            — K9 + G-series storage contract
ROADMAP.md                           — public G-series row
```

Rename ordering: G-series brief renames (G7 → M-G7, G8 → M-G8 LOCKED; others Q-G-1/Q-G-2 TBD) → `GPU_COMPUTE.md` cascade → all downstream cross-refs together.

**Cluster D — Vanilla.X mod-name affiliation (Q-V-1/Q-V-2)**

```
mods/DualFrontier.Mod.Vanilla.*/  (6 mod assemblies, 6 README files, 6 Mod.cs files)
  ↔ MIGRATION_PLAN_KERNEL_TO_VANILLA.md §6.2 vanilla→milestone mapping
  ↔ MOD_OS_ARCHITECTURE.md §2.1 example manifest
  ↔ GPU_COMPUTE.md §G-series (Vanilla.Magic, Vanilla.Electricity, Vanilla.Water, Vanilla.Movement)
  ↔ ROADMAP.md M-cycle table
```

Per Q-V-1 / Q-V-2, the ratification proposal must decide:
- Does the rename touch only milestone-ID labels in doc-comments, or also `Vanilla.X` mod-names (likely not — they are type names)?
- For multi-substrate mods (Vanilla.Magic = M10.B M-cycle content + G1 mana compute), which bucket assignment dominates the doc-comment ref?

---

## 9 — Cascade Volume Summary

**Total milestone references across all artifacts (raw grep counts on indexed source set):**

| Series | docs + briefs + register | code comments (src+tests+native+mods) | mod READMEs | Total |
|---|---|---|---|---|
| K-series (K0..K9, K8.x, K-L*) | ~821 | 188 (C#) + ~30 (C++) = ~218 | 0 | ~1039 |
| A'-series (A'.0..A'.9, A'.0.5, A'.4.5) | ~521 | 4 | 0 | ~525 |
| M-cycle (M0..M10.x, M3.5, M5.x, M6.x, M7.x, M8.x) | ~371 (incl ~270 in cascade-relevant docs/briefs; rest in closed audit reports) | 173 | ~25 (across 6 mod READMEs + 5 Mod.cs) | ~569 |
| M9.x runtime (M9.0..M9.8) | 98 | 0 | 0 | 98 |
| G-series (G0..G9) | 280 | 5 | 0 | 285 |
| Vanilla.X mod-name | 173 (docs/briefs/register) | 14 (C# type-name refs) + 5 (namespace) | 6 (own mod names) | ~198 |
| **Cascade-affected total** | **~1443** | **~415** | **~31** | **~1889** |

**References-per-bucket (excluding K-series and A'-series which are unchanged):**

| Bucket → rename target | Affected refs (locked + TBD) |
|---|---|
| M9.0..M9.8 → R (Q-R-1 format TBD) | 98 (docs+briefs) + 0 (code) = 98 |
| G7, G8 → M-G7, M-G8 (LOCKED) | G7: ~6 doc-refs + own brief (44 lines); G8: ~6 doc-refs + own brief (42 lines) — total ~100 line-edits |
| G0..G6, G9 → TBD (Q-G-1/Q-G-2) | ~280 - ~12 (G7+G8 above) ≈ ~268 |
| M-cycle pending (M8.x, M9, M10.x, M3.5) → M-K (Q-M-2/Q-M-3 format TBD) | ~150-200 (cascade-relevant docs); ~25 in mod READMEs + code-doc comments |
| M-cycle closed (M0..M7) → Q-M-1 | ~370 (in closure reviews / audit prompts / historical PROMPTS) — proposal decides whether to back-rename |
| Vanilla.X mod-names → Q-V-1/Q-V-2 | ~198 total references; rename touches milestone-ID labels adjacent to mod-names, possibly type-name doc-comments |

**Document count touched by rename:**

| Scope | Count |
|---|---|
| LOCKED Tier-1 architecture docs requiring body cascade | 7 (KERNEL, MOD_OS, RUNTIME, MIGRATION_PLAN, GPU_COMPUTE, FIELDS, PHASE_A_PRIME_SEQUENCING) |
| Live Tier-2 planning docs requiring body cascade | 3 (ROADMAP, MIGRATION_PROGRESS, IDEAS_RESERVOIR) |
| Tier-3 briefs requiring body cascade (active G-series) | 10 G-skeleton briefs (G7/G8 LOCKED rename; G0..G6/G9 TBD) |
| Tier-3 briefs cross-referencing pending M-cycle (K briefs) | ~8 (K8.0, K8.3, K8.4, K8.5, K8.34, K9, K_L3_1_*, K-Lessons batch) |
| Tier-2 live methodology / governance docs | 4 (METHODOLOGY, FRAMEWORK, SYNTHESIS_RATIONALE, REGISTER.yaml) — mostly K/A' refs unchanged; few M/G refs |
| Closed audit/closure reports (Q-M-1 deliberation) | ~25 (M3..M7 closure reviews, audit passes, prompts) |
| Module READMEs (Vanilla mods) | 6 |
| Mod-skeleton C# files (doc-comments) | 5-6 (CombatMod.cs, InventoryMod.cs, MagicMod.cs, PawnMod.cs, WorldMod.cs, possibly CoreMod) |
| Code files (`src/`+`tests/`+`native/`) with milestone-mentioning comments | ~80 unique files |
| **Total document/file scope** | **~150 unique files** |

**Estimated proposal scope (volume only, not architectural judgment):**

The rename cascade is **medium-large** on a pure-volume basis:

- **Heavy cascade**: ~98 M9.x refs across 6 docs (RUNTIME_ARCHITECTURE.md alone carries 38) — concentrated in two documents (RUNTIME and KERNEL).
- **Heavy cascade**: ~270 M-cycle refs across cascade-relevant docs (MIGRATION_PLAN, ROADMAP, MOD_OS §11, MIGRATION_PROGRESS, K8.x briefs) — concentrated in five-six documents.
- **Heavy cascade**: ~280 G-series refs, two locked (G7/G8 → M-G7/M-G8), 268 TBD pending Q-G-1/Q-G-2.
- **Medium cascade**: 6 mod READMEs + 5-6 mod-skeleton C# files = ~30 line-edits.
- **Lower-priority cascade**: ~370 historical M0..M7 refs in closed reviews/audits — Q-M-1 deliberation may bypass or back-rename.
- **Downstream cascade execution step**: ~415 code-comment refs in `src/`/`tests/`/`native/` — separate milestone, not part of the doc cascade.

Roughly: **15-25 LOCKED/Live docs require body cascade edits**; **10 G-series briefs require rename + content cascade**; **6 mod READMEs and ~6 mod-skeleton files require doc-comment cascade**; **~80 code files require comment cascade in a separate downstream milestone**; **~25 closed-record docs require Q-M-1 deliberation** (preserve or back-rename).

---

## 10 — Open Questions (restated)

Restating from §3 for proposal-author convenience. Each ambiguity is context-anchored to where in the codebase it appears and what input the ratification proposal needs.

| ID | Need from ratification proposal |
|---|---|
| **Q-R-1** | Lock the format for the M9.0..M9.8 → R rename. Candidates: (a) `R0..R8` flat, (b) `R-9.0..R-9.8`, (c) `R-Runtime0..R-Runtime8`. The proposal must pick one; downstream is 98 refs across 14 files. |
| **Q-R-2** | Resolve the M9 collision: M-cycle `M9 — Vanilla.Combat` (ROADMAP.md:44) and runtime `M9.0..M9.8` both currently use M9. Once Q-R-1 locks the R format, propose the M-cycle M9 rename (e.g. `M-K9` for Vanilla.Combat). |
| **Q-G-1** | Decide whether G0 (Vulkan compute pipeline plumbing) stays as G-substrate G0 or migrates to R0 (since it shares Vulkan plumbing with the runtime). Single-item decision; cascades to ~8 refs. |
| **Q-G-2** | Decide format for G1..G6 + G9 substrate items. Default candidate: retained as G1..G6, G9 (mirrors K0..K9 substrate). The proposal must confirm or pick alternative. Cascades to ~250 refs. |
| **Q-M-1** | Decide whether to back-rename M0..M7 closed phases (closure reviews, audit reports, prompts) to M-K0..M-K7 or leave as historical record. ~370 refs in ~25 closed-record docs. |
| **Q-M-2** | Lock format for pending M-cycle (M8.x, M9, M10.x, M3.5 deferred) → M-K rename. E.g. is M8.4 → `M-K8.4` or `M-K-World`? Cascades to ~270 refs across ~12 docs. |
| **Q-M-3** | Decide how deferred-but-named milestones (M3.4 deferred, M3.5 deferred) cascade. They are not yet implemented but live in `MOD_OS_ARCHITECTURE.md` §11.1. |
| **Q-K-1** | Reconcile the K8.5 / A'.6 / A'.7 identity overlap surfaced in `PHASE_A_PRIME_SEQUENCING.md` §2 vs `KERNEL_ARCHITECTURE.md` Part 2. Not a namespace question, but a register-of-truth question the proposal should note. |
| **Q-V-1** | Clarify scope: does the rename touch `Vanilla.X` mod-name doc-comments (type names in `mods/.../README.md` and `mods/.../*Mod.cs`), or only adjacent milestone-ID labels? The default reading is "only milestone-ID labels"; the proposal should confirm. |
| **Q-V-2** | Decide bucket assignment for multi-substrate vanilla mods (e.g. Vanilla.Magic = M10.B M-cycle + G1 mana compute substrate). Which composite ID dominates the doc-comment ref? |

---

## 11 — Halt Status

**No halt triggers fired.** All documents listed in REGISTER.yaml exist on disk; the REGISTER parses; all referenced files were located; no circular reference blocking unambiguous ordering surfaced; the report stays well under the 100 KB SC-6 ceiling; no LOCKED-document milestone-ID reference was found that the REGISTER does not enroll.

Specifically:

- **SC-1** (REGISTER doc path missing on disk): not triggered.
- **SC-2** (referenced file not located): not triggered.
- **SC-3** (REGISTER.yaml parse failure): not triggered.
- **SC-4** (milestone ID in LOCKED doc not registered anywhere): not triggered. All milestone IDs encountered (K-series, A'-series, M-cycle, M9.x runtime, G-series) are enrolled either as substrate buckets in `PHASE_A_PRIME_SEQUENCING.md` / `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` or as individual Category-D briefs in REGISTER.yaml.
- **SC-5** (circular reference blocking rename ordering): not triggered. The cross-reference graph in §8 shows directed clusters; rename ordering is sequenceable per cluster (RUNTIME first → KERNEL cross-refs / ROADMAP → public; MIGRATION_PLAN first → ROADMAP + MOD_OS §11 + mod READMEs; GPU_COMPUTE + G-skeleton briefs together).
- **SC-6** (report exceeds 100 KB): not triggered. Final size below.
- **SC-7** (push-to-main classifier block): N/A — this is one new research file in a working tree.

---

## 12 — Report Metadata

| Field | Value |
|---|---|
| Generation date | 2026-05-15 |
| Branch | `claude/milestone-cascade-map-akumN` |
| Model | claude-opus-4-7[1m] |
| Repo HEAD at recon start | `b9aeb6f` |
| REGISTER schema version | 1.0 |
| REGISTER version | 1.1 |
| REGISTER last_modified | 2026-05-13 |
| Documents read (full or partial) | REGISTER.yaml (header + index); KERNEL_ARCHITECTURE.md (Part 0–Part 2 + Part 9); MOD_OS_ARCHITECTURE.md (header–§4.6 + §11.x sample); RUNTIME_ARCHITECTURE.md (header–§7 + roadmap); MIGRATION_PLAN_KERNEL_TO_VANILLA.md (full body grep + Sections 0-3 read); GPU_COMPUTE.md (header + roadmap §G0–G9); FIELDS.md (header + §«Field» section); PHASE_A_PRIME_SEQUENCING.md (full); ROADMAP.md (full grep); IDEAS_RESERVOIR.md (full); METHODOLOGY.md (cross-reference grep only); 10 G-skeleton briefs (G0..G9 — full reads of G0/G1/G7/G8/G9 + headers of G2-G6); module READMEs under `mods/` (6 read); spot reads of `mods/.../*Mod.cs` doc-comments; code-comment grep across `src/`, `tests/`, `native/`, `mods/` (509 hits inventoried). |
| Source files searched | docs/ (entire tree, except scratch); tools/briefs/ (53 files); src/, tests/, native/, mods/ — grep for milestone ID patterns and `Vanilla.X` patterns; REGISTER.yaml header + index extraction. |
| Table row counts | §3 open-questions: 10 rows. §4 series tables (K/A'/M-current/M-runtime/G/Vanilla): K-summary 13 rows + sample anchors; A'-summary 7 rows + sample anchors; M9.x verbatim 25+ rows; M-cycle per-file ~40 rows + verbatim ~20 rows; G-series verbatim 16 rows + per-file 26 rows + cross-ref 12 rows; Vanilla per-file 16 rows + verbatim 11 rows. §5 by-artifact: ~30 rows. §6 code-comment per-ID: 16 M-rows + 12 K-rows. §7 REGISTER inventory: ~50 rows. §8 cross-ref graph: 4 directed clusters. §9 volume summary: 6 rows + 8 rows. §10 open questions restated: 10 rows. |
| Halt status | No halts. |
| Report size (target / actual) | Target 30-60 KB, ceiling 100 KB. Actual: see file size at commit time. |

---

**Brief end. Cascade map is mechanical inventory; ratification proposal author resolves §3 / §10 ambiguities and authors the rename plan.**
