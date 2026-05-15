---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-NAMESPACE_CASCADE_RESEARCH_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-NAMESPACE_CASCADE_RESEARCH_BRIEF
---

# M/G/R Namespace Cascade Map — Research Brief for Claude Code

**Status**: AUTHORED 2026-05-15. EXECUTED 2026-05-15 (cascade map shipped at commit `e3fef89`, output `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`).
**Mode**: Read-only research. **No code changes. No document edits. No REGISTER mutations. No commits.** The single output is one report document.
**Subject**: Targeted inventory of every milestone ID reference across LOCKED docs, REGISTER, briefs, and code — input to a forthcoming ratification proposal for composite milestone namespace (M-K / M-G / M-R / R / G).
**Output**: One report at `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`, structured as reference tables with line-range citations.
**Why this brief**: The 2026-05-15 deliberation reached a decision to introduce **composite milestone IDs that encode substrate dependency in the name** (e.g. `M-G7` for vanilla content depending on G substrate). Before authoring the ratification proposal, the deliberation requires complete ground truth on **where milestone IDs are referenced** across the repository, so the rename cascade is sized and bounded explicitly. This brief produces that ground truth.
**Predecessor**: `ARCHITECTURE_RECON_REPORT.md` (2026-05-15) — full architecture audit. This brief is **narrower**: it does not audit architecture, only inventories milestone ID references.

-----

## ⚠ READ ME FIRST

Three operating facts dominate everything below:

1. **The composite namespace decision is fixed in deliberation; this brief is downstream of it.** The 2026-05-15 deliberation locked the following substrate buckets: `K` (kernel, K0..K9 closed), `A'` (refactor cycle, A'.0..A'.5 closed), `R` (runtime/Vulkan — currently named M9.0..M9.8 in `RUNTIME_ARCHITECTURE.md`), `G` (GPU compute substrate — currently G0..G6 + G9), and demonstration buckets `M-K` (vanilla content on K substrate — the 10 production systems on disk retroactively), `M-G` (vanilla on G substrate — currently G7, G8), `M-R` (vanilla on R substrate — currently TBD). This brief does not re-deliberate the namespace; it inventories references that the ratification will need to update.
2. **Lesson #7 — transcribe references, do not paraphrase them.** Every milestone ID reference in the report must carry: file path, line number, verbatim text snippet of the surrounding context (so the reader sees how the ID is used — as a status row, as a cross-reference, as a comment, etc.), and a proposed new ID where the rename is unambiguous (with `TBD` marker where it is not).
3. **Lesson #8 — this is one atomic deliverable.** No phased recon shape. Either the report ships complete, or recon halts cleanly with a HALT_REPORT. The expected size is 30-60 KB / ~400-800 lines — much smaller than the full architecture recon (146 KB), because the task is narrow.

These three facts are why this brief explicitly forbids Claude Code from making any code, document, REGISTER, or commit changes. The deliverable is one new report file. Anything else is scope creep.

-----

## ⚠ HALT TRIGGERS

Recon halts under any of the following. Author a HALT_REPORT in `docs/scratch/NAMESPACE_CASCADE/HALT_REPORT_<timestamp>.md`; do not improvise:

- **SC-1** — A document path listed in REGISTER.yaml does not exist on disk. Stop, surface.
- **SC-2** — A file referenced in the report's evidence chain cannot be located. Stop, surface.
- **SC-3** — REGISTER.yaml fails to parse or is internally inconsistent. Stop, surface.
- **SC-4** — A milestone ID appears in a LOCKED document but is **not registered** anywhere (e.g. `MIGRATION_PLAN.md` references `K10` but REGISTER has no `DOC-D-K10` and no closure record). This is a high-severity finding and a halt — it indicates either a stale-claim drift or an unrecorded milestone. Document, stop.
- **SC-5** — A circular reference is found where document A references a milestone defined in B, which references a milestone defined in A, in a way that blocks unambiguous rename ordering. Stop, surface.
- **SC-6** — The report exceeds 100 KB. The cascade map should be table-dense, not analytical. If it inflates above 100 KB, the task may have been misunderstood. Stop, surface for scope refinement.
- **SC-7** — Push-to-main classifier block (known memory finding). N/A — recon produces one file in a working tree, not a push.

A halt is the right answer if a premise is wrong.

-----

## ⚠ PROHIBITED ACTIONS

Claude Code MUST NOT, within this session:

- Create, modify, or delete any source file under `src/` or `tests/`
- Create, modify, or delete any document under `docs/architecture/`, `docs/methodology/`, `docs/governance/`, or `docs/roadmap*`
- Create, modify, or delete any brief in `tools/briefs/` — including the G-series skeletons (which are part of the **subject** of recon, not its output)
- Edit `REGISTER.yaml`, `FRAMEWORK.md`, `SYNTHESIS_RATIONALE.md`, or any governance artifact
- Run `sync_register.ps1` or any governance tooling
- Run `dotnet build`, `dotnet test`, or any build/test command
- Make any commit; push to any remote
- **Propose specific new milestone IDs beyond what is already deliberation-locked.** The brief is mechanical: where the rename is unambiguous from the deliberation decision (G0..G6 → some R/G substrate name TBD by proposal; G7/G8 → M-G; M9.0..M9.4 runtime → R), record the candidate new ID. Where it is ambiguous (e.g. should G0 become `R0`, `G0` retained as G-substrate, or something else?), record as `TBD: requires deliberation` — do not pick.

The only permitted file operations:

- **Reading** any file in the repository
- **Creating** the report at `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`
- **Creating** a HALT_REPORT at `docs/scratch/NAMESPACE_CASCADE/HALT_REPORT_<timestamp>.md` if a halt trigger fires

-----

## §0 — Goals and non-goals

### §0.1 — Goal

Produce a structured inventory of every milestone ID reference in the repository, so that a forthcoming ratification proposal for composite milestone namespace can size and bound the rename cascade explicitly.

A milestone ID is any of:

- **K-series**: K0, K1, K2, K3, K4, K5, K6, K7, K8 (and subindexes: K8.0, K8.1, K8.2, K8.3, K8.4, K8.5), K9, K-L3.1, K-L1..K-L11 (kernel lessons)
- **A'-series**: A'.0, A'.0.5, A'.0.7, A'.1, A'.2, A'.3, A'.4, A'.4.5, A'.5, A'.6, A'.7, A'.8, A'.9 (and any future)
- **M-series (current)**: M0..M10 and subindexes like M9.0..M9.8
- **G-series (current)**: G0..G9

Plus references to **"vanilla mods"** which carry implicit milestone affiliation (Vanilla.Combat = M-cycle, Vanilla.Movement = G7, etc.).

The report enables the ratification proposal author to know **exactly** which lines of which documents will change when the namespace is ratified.

### §0.2 — Non-goals

- Not an architectural audit — that was the prior recon (`ARCHITECTURE_RECON_REPORT.md`).
- Not the ratification proposal itself — only its input.
- Not architectural recommendations.
- Not effort estimates.
- Not deciding the new IDs where the rename is ambiguous. Where uncertain, mark `TBD: requires deliberation`.
- Not validating whether the namespace decision was correct. The decision is fixed in deliberation as of 2026-05-15.

### §0.3 — What "unambiguous" means for ID renames

The 2026-05-15 deliberation locked these substrate buckets but did **not** lock the specific renames inside them. Examples:

**Unambiguous (rename is clear):**

- `G7 Vanilla.Movement` (G-skeleton) → `M-G7` (vanilla content on G substrate, the deliberation explicitly named this case)
- `G8 Local Avoidance` → `M-G8` (deliberation named this)
- 10 current production systems retroactively belong to `M-K` category (deliberation explicitly named this)

**Ambiguous (proposal must decide):**

- `G0 Vulkan Compute Plumbing` → is this `R0` (runtime substrate, since Vulkan plumbing is shared with rendering) or `G0` retained (G-substrate, distinct from R)? Deliberation has not decided.
- `M9.0..M9.4 runtime` → `R0..R4`? `R-Runtime0..R-Runtime4`? Format not locked.
- `K8.5 Mod Ecosystem Prep` (current A'.6) → stays as `K8.5` because K-series is done as a label? Or migrates to `A'-K8.5` for clarity? Deliberation has not decided.

For unambiguous cases: record the candidate new ID in the cascade map.
For ambiguous cases: record `TBD: requires deliberation` and surface the ambiguity in §3 (Open questions) of the report.

-----

## §1 — Required reads

Read every document below in full **for the purpose of milestone ID reference extraction only**. Do not perform architectural analysis. Do not summarize content. Find references, record them.

### §1.1 — REGISTER.yaml (entry point)

`docs/governance/REGISTER.yaml` — read end to end. Extract:

- Every `DOC-D-K*` entry (K-series briefs)
- Every `DOC-D-A_PRIME*` entry (A'-series briefs)
- Every `DOC-D-M*` entry (M-series briefs)
- Every `DOC-D-G*` entry (G-series briefs)
- Every `DOC-A-*` entry that lists M/G/R/K milestones in its frontmatter or notes
- Every `DOC-C-*` entry referencing roadmap progression
- Every `DOC-E-*` closure report referencing milestone IDs

### §1.2 — LOCKED Category-A architecture documents

| Document                                               | Why this brief reads it                                                             |
|-------------------------------------------------------|------------------------------------------------------------------------------------|
| `docs/architecture/KERNEL_ARCHITECTURE.md`             | Part 2 status table is the central K/M/G status registry                            |
| `docs/architecture/MOD_OS_ARCHITECTURE.md`             | References K9 (fields) and G0 (compute) in v1.7 §4.6 surface                        |
| `docs/architecture/RUNTIME_ARCHITECTURE.md`            | Defines M9.0..M9.8 runtime milestones (rename candidates → R)                       |
| `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`| Central M-cycle nomenclature; **most dense** reference site                         |
| `docs/architecture/GPU_COMPUTE.md` v2.0                | Inlines the G-series roadmap (G0..G9); references K9 and M9.0..M9.4 as prerequisites|
| `docs/architecture/FIELDS.md`                          | Live v0.1 — may reference K9, G-series                                              |
| `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`        | Defines A'.0..A'.9 (the A' namespace); references K-series and forthcoming M/G work |
| `docs/architecture/FHE_INTEGRATION_CONTRACT.md`        | If it references any milestone (likely M-cycle or post-G)                           |
| `docs/architecture/ECS.md`                             | If it references K-series or M-series                                               |
| `docs/architecture/ISOLATION.md`                       | If it references K-series                                                           |
| `docs/architecture/THREADING.md`                       | If it references K-series                                                           |
| `docs/architecture/MOD_PIPELINE.md`                    | If it references M-cycle or mod-related milestones                                  |
| `docs/architecture/EVENT_BUS.md`                       | If it references K-series or M-cycle                                                |
| `docs/architecture/CONTRACTS.md`                       | If it references K-series                                                           |
| `docs/architecture/MODDING.md`                         | If it references mod-related milestones                                             |
| `docs/architecture/ARCHITECTURE.md`                    | Top-level — may have milestone references                                           |
| `docs/architecture/PERFORMANCE.md`                     | May reference milestones                                                            |
| `docs/architecture/GODOT_INTEGRATION.md`               | May reference M-cycle                                                               |
| `docs/architecture/VISUAL_ENGINE.md`                   | May reference M9.x runtime                                                          |

### §1.3 — Methodology and governance

| Document                                | Why                                            |
|----------------------------------------|-----------------------------------------------|
| `docs/methodology/METHODOLOGY.md`       | Lessons reference milestones; K-Lessons history|
| `docs/governance/FRAMEWORK.md`          | Schema may define milestone-ID conventions     |
| `docs/governance/SYNTHESIS_RATIONALE.md`| May reference milestone provenance             |

### §1.4 — Live planning documents

| Document                 | Why                                                   |
|-------------------------|------------------------------------------------------|
| `docs/ROADMAP.md`        | M-cycle phase status; the public-facing milestone view|
| `docs/IDEAS_RESERVOIR.md`| May reference M-cycle phases ("post-Phase-7")         |

### §1.5 — G-skeleton briefs (the subject of the rename)

All 10 G-series skeletons in `tools/briefs/`. For each, extract:

- Frontmatter `register_id`
- Body references to: own ID, dependent G-IDs, K-IDs, M-IDs, R-IDs
- Status header (`Status: NOT STARTED (prerequisite: ...)`)
- Goal/Deliverables sections — any embedded milestone refs

| Path                                                |
|----------------------------------------------------|
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md`  |
| `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md`           |
| `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md`  |
| `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md`      |
| `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md`  |
| `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md`      |
| `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md`|
| `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md`         |
| `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md`          |
| `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md`          |

### §1.6 — K/A'-series briefs (cross-reference targets)

Read structurally — the rename does not change K/A' IDs themselves (those stay K0..K9, A'.0..A'.9), but K/A' briefs may reference G/M/R milestones. Capture every G/M/R reference in K/A' briefs.

| Pattern  | Path                                                    |
|---------|--------------------------------------------------------|
| K-series | `tools/briefs/K*_BRIEF.md` (all variants)               |
| A'-series| `tools/briefs/A_PRIME_*_BRIEF.md` (all variants)        |
| K-L3.1   | `tools/briefs/K_L3_1_*.md`                              |
| K8.34 v2 | `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md`|

### §1.7 — Reports (Category E)

| Document                                                                                        |
|------------------------------------------------------------------------------------------------|
| `docs/reports/ARCHITECTURE_RECON_REPORT.md` (2026-05-15 recon — references all milestone series)|
| `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` (Live)                                               |
| `docs/reports/NORMALIZATION_REPORT.md`                                                          |
| `docs/reports/NATIVE_CORE_EXPERIMENT.md`                                                        |
| Any other report under `docs/reports/`                                                          |

### §1.8 — Code references (comments only)

Code itself does not need rename when milestone IDs change — but **comments** in code do. Grep `src/` and `tests/` for milestone ID patterns in comments. Specifically:

- `// K[0-9]`, `// K8\.`, `// K-L`, `// K9`
- `// A'\.`, `// A'.5`, `// A_PRIME`
- `// M[0-9]`, `// M9\.`, `// Phase [4-9]`
- `// G[0-9]`
- `// vanilla`, `// Vanilla\.` (case-insensitive — Vanilla.Combat, Vanilla.Movement, etc.)

For each hit, record: file path, line, the comment line verbatim, the surrounding 1-2 lines for context.

**Code comments are the largest reference surface** — there will be many. The report tables this comprehensively but does not propose comment edits (those are a separate downstream cascade execution).

### §1.9 — Module READMEs (Category F)

Module-local READMEs under `src/*/README.md` may reference milestone IDs in their "current status" sections. Read each, extract milestone refs.

### §1.10 — Configuration / project structure

| Path                             | Why                                                                       |
|---------------------------------|--------------------------------------------------------------------------|
| `DualFrontier.sln`               | Solution structure — may not have milestone refs but read for completeness|
| Project `*.csproj` files         | Build configurations occasionally reference milestone IDs in comments     |
| `.gitignore`                     | May reference milestone-named directories                                 |
| `tools/briefs/` directory listing| Cross-reference against REGISTER for completeness                         |

-----

## §2 — Output specification

### §2.1 — File location

`docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`. New file. `docs/research/` is the canonical home per `IDEAS_RESERVOIR.md`'s pattern for "specifications of activating reservoir ideas" — this brief stretches that pattern to "specifications of architectural namespace ratifications", which is appropriate.

If `docs/research/` does not exist on disk, create the directory.

### §2.2 — Frontmatter

```yaml
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
```

REGISTER.yaml itself is **not edited** by this brief — a separate governance action enrolls the document. The frontmatter is a placeholder for the eventual auto-generation.

### §2.3 — Section structure

The report is **table-dense, not prose-dense**. Sections:

1. **Read Me First** — load-bearing facts (2-3 sentences each). Examples: "Cascade map is mechanical inventory, not architectural analysis." "Renames marked TBD require deliberation; do not pick." "Ambiguous cases are surfaced in §3, not resolved."
2. **Substrate Bucket Definitions (locked)** — verbatim restatement of the 2026-05-15 deliberation decision. K, A', R, G as substrate buckets. M-K, M-G, M-R as demonstration buckets. Locked items: G7→M-G7, G8→M-G8, retroactive M-K for 10 production systems.
3. **Open Naming Questions (for ratification proposal)** — list every ambiguity uncovered during the inventory. Examples: "Should G0 become R0 or stay G0?", "Should K8.5 retain K-prefix or move to A'?", "Format for M9.0..M9.4 runtime — R0..R4? R-9.0..R-9.4?". Recon **does not answer**, only surfaces.
4. **Reference Inventory by Series** — five subsections (K, A', M-current, G-current, vanilla mod names):
   - **§4.1 K-series references** — table of every K0..K9 + K8.x + K-L* mention across documents/briefs/code. Format: `Reference ID | File | Line | Verbatim context | Rename status (unchanged for K)`.
   - **§4.2 A'-series references** — similar table.
   - **§4.3 M-series references (current)** — every M0..M10 + M9.0..M9.8 reference. Format includes `Proposed new ID` column. For M-K retroactive: where do current 10 production systems get referenced as part of M-K?
   - **§4.4 G-series references (current)** — every G0..G9 reference. Format includes `Proposed new ID` column with G7→M-G7, G8→M-G8 unambiguous, others TBD.
   - **§4.5 Vanilla mod name references** — every "Vanilla.X" mention (Vanilla.Movement, Vanilla.Combat, Vanilla.Magic, etc.). These carry implicit milestone affiliation — record which milestone bucket they map to.
5. **Reference Inventory by Artifact** — orthogonal view: per-document summary of how many milestone references it carries, what categories. For example: "MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.1 — 47 milestone references, primary cascade impact site." Provides at-a-glance volume estimate for the rename cascade.
6. **Code Comment Inventory** — table of every milestone-mentioning comment in `src/` and `tests/`. Larger table; sample-quoted comments grouped by milestone ID rather than by file. Total count by ID.
7. **REGISTER Inventory** — every REGISTER entry whose `register_id` contains M/G/R/M9 patterns. Format: `register_id | Path | Current lifecycle | Notes`.
8. **Cross-Reference Graph (high-level)** — directed: document A → milestone X → document B. Surfaces tight reference clusters where rename ordering matters (e.g. if X is renamed, A's reference to X must be updated before B's link to A is followed).
9. **Cascade Volume Summary** — final table:
   - Total milestone references across all artifacts (sum)
   - References per bucket (K / A' / M / G / "Vanilla.*")
   - Document count touched by rename
   - Estimated proposal scope (small/medium/large) — based purely on volume, not architectural judgment
10. **Open Questions** — restated explicitly. Each TBD case from §3 with its context, why it's ambiguous, what input the ratification proposal needs.
11. **Halt Status** — any SC-* triggers, or "no halts".
12. **Report Metadata** — generation date, branch, model, repo HEAD, REGISTER schema version, documents read, source files read, table row counts, report size, halt status.

### §2.4 — Format conventions

- **Tables for everything.** Prose only in section intros (1-2 sentences max). Lesson #7 applied to format: the reader should see references, not narrative.
- **Quote verbatim with line ranges** (`file.md:42-45`). No paraphrase.
- **No severity labels** for milestone references — they are facts, not findings. Severity applies to halts only.
- **Decimal-numbered subsections** (§4.1, §4.2).
- **Language**: English. Match `ARCHITECTURE_RECON_REPORT.md` and `CPP_KERNEL_BRANCH_REPORT.md`. No mid-document language switch.
- **Tone**: factual, sparse, dry.
- **No emoji.** No decorative formatting.

### §2.5 — Length expectations

`ARCHITECTURE_RECON_REPORT.md` was 146 KB / 1769 lines because it included three analytical layers (game-assumptions, technical debt, series interlock). This cascade map is **one mechanical inventory** — much smaller. Expected: **30-60 KB / 400-800 lines**. SC-6 halt at **100 KB**.

If the report approaches 100 KB and the inventory is incomplete, halt — the scope was misjudged. Do not truncate quietly.

-----

## §3 — What this recon explicitly does NOT do

- Does not propose specific new milestone IDs beyond what 2026-05-15 deliberation already locked (G7→M-G7, G8→M-G8, 10 production systems retroactively → M-K).
- Does not propose namespace conventions for ambiguous cases — only surfaces ambiguities.
- Does not patch any reference (no rename done, only inventory).
- Does not audit architecture — that was the prior recon.
- Does not edit REGISTER, LOCKED docs, methodology, governance, or any brief.
- Does not edit any source file.
- Does not estimate effort.
- Does not commit or push.
- Does not run builds, tests, or benchmarks.

-----

## §4 — Provenance and lineage

```
2026-05-13  K8.3 v2.0 brief authored — halt 1
2026-05-14  combined K8.3+K8.4 brief v1.0 — halts 2 and 3 → patch v1
            v2.0 brief authored from code, executed cleanly (b903b91 → fc8ecb6)
2026-05-15  Architecture recon — full audit (ARCHITECTURE_RECON_REPORT.md)
2026-05-15  deliberation: project re-framed as research framework
            (README.md DOC-G-README LOCKED Live, clarified mid-session);
            M-series sequencing decision: substrate before vanilla content;
            composite milestone IDs decided (K/A'/R/G/M-K/M-G/M-R) — minimal,
            existing-only, future buckets added only when needed
2026-05-15  this brief authored
2026-05-15  recon executed — one Claude Code session — cascade map produced
            (commit e3fef89; output docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md;
            no halts; 73.7 KB / 807 lines within SC-6 100 KB ceiling)
2026-XX-XX  ratification proposal authored using cascade map as input
2026-XX-XX  ratification by Crystalka; cascade execution as separate milestone
```

-----

## §5 — Reading sign-off

Before executing, Claude Code confirms:

1. **Read-only research.** No code/document/REGISTER/commit changes. Single deliverable: `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md`.
2. **Mechanical inventory, not analysis.** Find references, record them with verbatim context. Do not summarize, interpret, or recommend.
3. **Composite namespace is locked in deliberation.** K, A', R, G substrate buckets; M-K, M-G, M-R demonstration buckets. Renames marked TBD only where deliberation has not locked the specific case.
4. **Halt is the right answer if a premise is wrong.** Three K8.3+K8.4 halts and one mid-recon-pause already protected the project from bad authorings. Halts on this recon would do the same.
5. **Table-dense format.** Prose minimal; references comprehensive.
6. **Output size 30-60 KB expected, 100 KB halt ceiling.**

**Brief end. Execution begins at §1.1 (REGISTER.yaml as entry point).**

-----

## Execution closure (2026-05-15)

Executed by Claude Opus 4.7 session on branch `claude/milestone-cascade-map-akumN`.

- **Output**: `docs/research/M_G_R_NAMESPACE_CASCADE_MAP.md` (73.7 KB / 807 lines).
- **Cascade map commit**: `e3fef89` — `docs(research): M/G/R namespace cascade map — milestone ID reference inventory`.
- **Halts**: none. SC-1..SC-6 all clear.
- **Locked renames captured**: 2 (G7 → M-G7, G8 → M-G8).
- **Open ambiguities surfaced**: 10 (Q-R-1, Q-R-2, Q-G-1, Q-G-2, Q-M-1, Q-M-2, Q-M-3, Q-K-1, Q-V-1, Q-V-2).
- **Cascade-affected refs inventoried**: ~1,889 across ~150 unique files.
- **Next**: ratification proposal authoring (separate milestone, not this brief's scope).
