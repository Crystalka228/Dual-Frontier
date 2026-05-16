---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-ARCHITECTURE_RECON_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-ARCHITECTURE_RECON_BRIEF
---
# Architecture Reconnaissance — Research Brief for Claude Code

**Status**: AUTHORED 2026-05-15.
**Mode**: Read-only reconnaissance. **No code changes. No document edits. No REGISTER mutations. No commits.** The single output is one report document.
**Subject**: Full DualFrontier architecture-vs-game-assumptions audit + technical-debt inventory + G/K-series interlock map, with deliberate analytical focus on **navigation as load-bearing foundation for the post-release horizon**.
**Output**: One report at `docs/reports/ARCHITECTURE_RECON_REPORT.md`, formatted in the precedent established by `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` (v Live, 2026-05-07).
**Why this brief**: Three recent halts on the K8.3+K8.4 combined milestone (CAPA-2026-05-13 storage-location, CAPA-2026-05-14 API-surface, CAPA-2026-05-14 mid-transition-drift) made it clear that briefs authored against unknown current state stop at execution. Before the **next** architectural milestone is written, the deliberation requires honest ground truth of what is on disk now — including the **G-series skeleton briefs** (`G0_VULKAN_COMPUTE_PLUMBING` through `G9_EIKONAL_UPGRADE`) that record forward architectural intent for GPU compute and movement, but have not been authored to execution-ready state. This brief produces that ground truth.

---

## ⚠ READ ME FIRST

Three operating facts dominate everything below:

1. **G-series briefs are skeletons, not authored execution briefs.** They exist on disk in `tools/briefs/G0_*..G9_*` and they record architectural intent for GPU compute, flow fields, vanilla movement, local avoidance, and Eikonal upgrade. They are **not** LOCKED specs; they are **not** in REGISTER as EXECUTED. They are the cleanest record of where the navigation/compute thinking is heading. Recon **treats them as primary architectural intent** and maps current code + LOCKED docs against them. Recon does **not** attempt to author them, advance them, or critique them — only to map them.

2. **Lesson #7 — transcribe APIs, do not paraphrase them.** v2.0 of the K8.3+K8.4 brief was authored from a fresh read of system / scheduler / test code, and it passed execution with zero halts after three prior versions halted on paraphrased premises. Recon applies the same rule: the report quotes signatures, comments, constants, and skeleton excerpts verbatim. Paraphrase only when explicitly signposted as synthesis (one section, clearly marked — see Section 14 of the precedent for the discipline).

3. **Lesson #8 — prove each intermediate state is valid; if you cannot, the atomic unit is larger than one step.** This brief authors one document end-to-end in one session. There is no "Phase 0 / Phase 1 / Phase 2" within recon — recon is read, take notes, write report, halt. A partial recon report is not useful; an intermediate state of "half the documents read" is not a deliverable. Either the report is delivered complete, or recon halts cleanly with a HALT_REPORT.

These three facts are why this brief explicitly forbids Claude Code from making any code, document, REGISTER, or commit changes. The deliverable is one new report file plus the work of having read everything required. Anything else is scope creep, and scope creep on a recon brief that becomes input to a ratification proposal is exactly the kind of cross-contamination that produced the three halts.

---

## ⚠ HALT TRIGGERS

Recon halts under any of the following. Author a HALT_REPORT in `docs/scratch/ARCHITECTURE_RECON/HALT_REPORT_<timestamp>.md`; do not improvise:

- **SC-1** — A document path listed in REGISTER.yaml does not exist on disk. Stop, surface, do not "infer what it would have said."
- **SC-2** — A file referenced in the report's evidence chain (e.g. a constant in NavGrid.cs at a specific line) cannot be located. Stop, surface.
- **SC-3** — REGISTER.yaml itself fails to parse or is in an internally inconsistent state. Stop, surface — REGISTER is the recon's entry point.
- **SC-4** — A LOCKED document version on disk differs from the version REGISTER asserts. This is a finding **and** a halt — recon's facts are no longer trustworthy until reconciled.
- **SC-5** — A G-series skeleton brief contradicts a LOCKED document (e.g. G7 vanilla movement assumes a kernel surface that K-series LOCKED docs do not provide and have not promised). This is a high-severity finding **and** a halt — it indicates an architectural conflict that the recon report alone cannot resolve, and the user must rule on it before recon can usefully continue. Document the conflict, stop.
- **SC-6** — The report exceeds 200 KB. Recon scope was misjudged. Stop, surface for scope refinement.
- **SC-7** — Push-to-main classifier block (known memory finding). Not a halt — recon produces one file in a working tree, not a push. The user pushes when ready.

A halt is not failure. Three halts protected the K8.3+K8.4 milestone from bad commits. A recon halt protects the next deliberation from bad ground truth.

---

## ⚠ PROHIBITED ACTIONS

Claude Code MUST NOT, within this session:

- Create, modify, or delete any source file under `src/`
- Create, modify, or delete any test file under `tests/`
- Create, modify, or delete any document under `docs/architecture/`, `docs/methodology/`, `docs/governance/`, or `docs/roadmap*` — these are LOCKED or live and outside recon scope
- Create, modify, or delete any brief in `tools/briefs/` — including any of the G-series skeletons (which are part of the **subject** of recon, not its output)
- Edit `REGISTER.yaml`, `FRAMEWORK.md`, `SYNTHESIS_RATIONALE.md`, or any governance artifact
- Run `sync_register.ps1` or any governance tooling
- Run `dotnet build`, `dotnet test`, or any build/test command
- Make any commit; push to any remote

The only permitted file operations are:

- **Reading** any file in the repository
- **Creating** the report file at `docs/reports/ARCHITECTURE_RECON_REPORT.md` (this is the deliverable)
- **Creating** a HALT_REPORT at `docs/scratch/ARCHITECTURE_RECON/HALT_REPORT_<timestamp>.md` if a halt trigger fires

If recon finds itself wanting to fix something in passing — record it as a finding in Section 13 of the report (the technical-debt inventory). Do not patch. Recon is recon.

---

## §0 — Goals and non-goals

### §0.1 — Three analytical layers

Recon produces **three interlocking deliverables in one report**:

**Layer A — Architecture-vs-game-assumptions map.** For every load-bearing engine subsystem on disk, identify:
- Where it is correctly abstracted (engine-side, scale-independent, game-agnostic)
- Where it is prematurely specialized to current game assumptions (200×200 map, 50 pawns, fixed `coreSystems`, vanilla mechanics specifics)
- Where the gap between architectural promise (LOCKED docs) and code reality has drifted

The output of Layer A is the input to a future ratification proposal — the user will decide, **based on this layer**, whether the architecture needs broad rework, targeted updates, or is fundamentally sound with only documentation drift.

**Layer B — Technical debt inventory.** A structured catalog of cleanup work accumulated across recent milestones (A'.4.5 enrollment, K8.3+K8.4 cutover, prior halts and reverts). Each entry has: file path, evidence (what is there now, what it should be, why), severity, and proposed cleanup window (dedicated cleanup milestone vs piggyback on next relevant milestone). Known seeds: orphan `.uid` files post-cutover, `SpanLease<T>.Pairs` Version=1 vs native Version=0 mismatch noted in K8.3+K8.4 commit 2, stale comments in production wiring referencing pre-K8 state, the per-system test coverage gap from the K8.3+K8.4 closure.

**Layer C — G/K-series interlock map.** Mapping of **all** authored briefs (G-series skeletons + K-series briefs at every lifecycle stage + A'-series milestones) against:
- The current on-disk code state (where code is already moving in the brief's direction; where code is pinned to pre-brief assumptions)
- LOCKED architecture documents (concordance vs contradiction)
- IDEAS_RESERVOIR.md (which reservoir items depend on which series milestones)
- Inter-series dependencies (which K must close before which G is authorable; which A' phase carries which K)

The three layers interlock. Technical debt is a signal of architectural pressure; architectural drift sometimes hides under debt patches; and G-series skeletons reveal where the project's forward intent sits relative to both the LOCKED state and the code. The recon must produce all three because each illuminates the others.

### §0.2 — Analytical priority: navigation as load-bearing foundation

The user has specified that navigation (NavGrid, AStarPathfinding, MovementSystem, all spatial structure) is the **most consequential architectural question on the post-cutover horizon**, because:

- It must accommodate **dynamic map expansion** (the only item from `IDEAS_RESERVOIR.md` already in active backlog — colony grows, map grows from 200×200 toward 250×250 and beyond during simulation)
- It must accommodate **hybrid pathfinding** (the user's current architectural intent: ad-hoc CPU paths for ordinary pawns, GPU compute for declared mass events like raids and caravans, possibly terrain-shader fields for emergent movement)
- It must accommodate **mod-extensible navigation** through Mod API v3 (custom terrain types, custom pathfinders) without requiring breaking changes
- It is currently prototyped against a fixed 200×200 grid in code that was written before any of the above requirements were known
- **The G-series skeletons (G6 flow field infrastructure, G7 vanilla movement, G8 local avoidance, G9 Eikonal upgrade) record the project's forward intent for this exact domain.** Recon must read them as primary architectural sources.

Navigation gets **dedicated analytical depth** in the report (its own section with line-level evidence from code AND verbatim excerpts from G-series skeletons). Other subsystems get **comparable rigor where they intersect navigation** (scheduler concurrency model, NativeWorld K8.1 primitives, Mod API v3 surface, GPU compute infrastructure per `GPU_COMPUTE.md` v2.0) and **structural rigor where they don't** (factory layer, event bus, modding pipeline — observed and characterized, not exhaustively quoted).

### §0.3 — Non-goals

This recon explicitly does **not** produce:

- Architectural recommendations or proposed solutions. The report records observations and open questions only. Solutions are deliberation work, after the report.
- Any ratification proposal. The recon is **input** to a potential ratification, not the ratification itself.
- **Any authorship advancement of G-series skeletons.** Recon maps them; it does not advance them from skeleton to authored-ready. That is deliberation work after recon, conducted with full knowledge of the recon's findings.
- Estimates of effort for any future work. The precedent report's Section 14.4 ("Effort estimate") is **specific to that branch's cherry-pick scenario** and is not the appropriate analog for an architecture-wide recon.
- Performance measurements, benchmarks, profiling. No `dotnet build/test` is run; no measurements are taken; nothing is observed under load. The recon reads source, not behavior. If a committed benchmark artifact exists on disk, read it; if it doesn't, note its absence and move on.
- Speculative architectural risk grading. Risks are documented where the **evidence shows risk**; the recon does not invent risks for completeness.

---

## §1 — Entry point: REGISTER.yaml

The DualFrontier project maintains a governance register at `docs/governance/REGISTER.yaml` (231+ documents enrolled per A'.4.5 closure, schema `FRAMEWORK.md` v1.0). This register is the **mandatory first read** of recon — it is the project's authoritative document map.

### §1.1 — REGISTER reading procedure

Do the following in order:

1. Read `docs/governance/REGISTER.yaml` end to end. Build a mental index by category (A through E) and by lifecycle (LOCKED, AUTHORED, EXECUTED, SUPERSEDED, Live).
2. Read `docs/governance/FRAMEWORK.md` for schema specification — what each field means, what the lifecycle transitions mean, what category/tier conventions mean.
3. Read `docs/governance/SYNTHESIS_RATIONALE.md` for the source-standard provenance (DO-178C / ISO 9001 / ISO 26262 / IEC 61508 / FDA 21 CFR Part 11 synthesis), so recon understands the discipline frame.

The report's Section 3 documents what the register contains, organized by category and by relevance to recon goals.

### §1.2 — Known-relevant document IDs (starting set, not exhaustive)

The following are confirmed present in REGISTER as of 2026-05-15 and are **starting reads**, not the full set. Claude Code expands the set by following REGISTER:

**Tier 1 LOCKED architecture (Category A) — read every one:**
- `DOC-A-KERNEL` → `docs/architecture/KERNEL_ARCHITECTURE.md` (v1.5 or current — verify on disk; A'.5 closure may have bumped to v1.6)
- `DOC-A-MOD_OS` → `docs/architecture/MOD_OS_ARCHITECTURE.md` (v1.7 or current)
- `DOC-A-RUNTIME` → `docs/architecture/RUNTIME_ARCHITECTURE.md` v1.0 (surfaced by CPP_KERNEL_BRANCH_REPORT — read for runtime-vs-kernel boundary)
- `DOC-A-MIGRATION_PLAN` → `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (v1.1 or current; A'.5 may have bumped to v1.2)
- `DOC-A-GPU_COMPUTE` → `docs/architecture/GPU_COMPUTE.md` v2.0 (**critical** — GPU compute is the substrate for hybrid pathfinding and electricity; recon must understand its current API and constraints)
- `DOC-A-FHE_INTEGRATION` → `docs/architecture/FHE_INTEGRATION_CONTRACT.md` v1.0 (precedent for "ratified contract for dormant capability")

**Methodology and governance (Category B):**
- `DOC-B-METHODOLOGY` → `docs/methodology/METHODOLOGY.md` (verify version — A'.5 closure added Lessons #7+#8)
- `DOC-B-FRAMEWORK` → `docs/governance/FRAMEWORK.md` v1.0
- `DOC-B-SYNTHESIS_RATIONALE` → `docs/governance/SYNTHESIS_RATIONALE.md` v1.0

**Live planning documents (Category C):**
- `DOC-C-ROADMAP` → verify exact path via REGISTER
- `DOC-C-PHASE_A_PRIME_SEQUENCING` → `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`
- `DOC-C-IDEAS_RESERVOIR` → `docs/architecture/IDEAS_RESERVOIR.md` (per deliberation upload, verify path on disk)

**Reports (Category E):**
- `DOC-E-CPP_KERNEL_BRANCH_REPORT` → `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` Live (**precedent for recon's report format** — read in full early)

**Closure briefs and prior milestones (Category D, EXECUTED or SUPERSEDED — useful for deltas):**
- `DOC-D-K8_34_COMBINED_V2` → `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md` (the brief whose execution produced the current state — read for ground truth on what was supposed to ship vs what's on disk)
- Any K8.3 / K8.4 / K-L3.1 closure reports the register references
- Any `K_CLOSURE_REPORT*` documents

REGISTER will surface additional documents. Pay particular attention to:
- Anything with `nav`, `path`, `movement`, `grid`, `spatial`, `map`, `expansion`, `terrain` in title or path
- Anything with `compute`, `gpu`, `kernel`, `runtime` in title or path
- Anything with `lifecycle: Live` (the "growing" documents — IDEAS_RESERVOIR, ROADMAP, K-RAW-NOTES, etc.)
- Anything with `lifecycle: AUTHORED` but not EXECUTED (pending work that informs current architectural posture)

### §1.3 — Skeleton briefs in REGISTER vs on disk

A specific sweep: cross-reference the **on-disk** contents of `tools/briefs/` against REGISTER. The G-series skeletons (G0..G9) may or may not be enrolled in REGISTER; report the registration state of each. Mismatches are Layer B findings:
- Skeleton on disk but not in REGISTER → unregistered brief (governance gap)
- Skeleton in REGISTER as AUTHORED → recon treats as authored-but-unexecuted, not skeleton (this contradicts the user's "all skeletons" statement and is a SC-4 halt trigger)
- Skeleton in REGISTER as a non-D category (e.g. C/live or E/report) → record as anomaly

---

## §2 — Required reads: documents (LOCKED, governance, planning)

Read every document below in full. The report references specific sections, line ranges, and quotes where the evidence demands it.

### §2.1 — LOCKED architecture (Category A, Tier 1)

| Document | Why recon reads it | What to extract |
|---|---|---|
| `KERNEL_ARCHITECTURE.md` | Defines K-L1 through K-L11 architectural invariants. **K-L11** (NativeWorld single source of truth) is the load-bearing post-cutover invariant. | All K-L invariants, particularly any that touch spatial / scheduling / determinism. Any mention of map size, world size, scale. |
| `MOD_OS_ARCHITECTURE.md` | Defines the OS-as-modding model: capabilities, ALC isolation, capability validation, Mod API. **Critical for**: can a mod replace navigation? | Mod API v3 surface, capability inventory, capability check points, registration mechanisms. Quote `IModApi` v3 exactly. |
| `RUNTIME_ARCHITECTURE.md` | Defines the runtime layer — render, presentation bridge, frame-budget allocation. **Critical because**: the relationship between simulation runtime and kernel scheduler determines what concurrency model navigation must respect. | Frame budget, tick-vs-render relationship, thread mapping, cross-runtime/kernel synchronization contracts. |
| `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | The K-series → M-series progression. | Current K-series status post-K8.4. Any explicit mention of dynamic map expansion. Any explicit navigation milestone. M-cycle milestone definitions. |
| `GPU_COMPUTE.md` v2.0 | **Highest priority for navigation recon.** Defines GPU compute substrate, what the kernel exposes to compute, how compute is registered (presumably through `IModComputePipelineApi` per K9), determinism contract for compute. | Full compute API surface. Determinism model. How compute reads/writes ECS data. Any worked example of a compute pipeline. Any explicit roadmap item for pathfinding-on-compute, electricity-on-compute, terrain-on-compute. **Specifically: cross-reference against G-series skeletons (§2.4).** |
| `FHE_INTEGRATION_CONTRACT.md` | **Format precedent** for ratified-but-dormant capability. Studied as template for "how does this project ratify a capability whose implementation comes years later?" | Contract structure. Conditional-activation pattern (§D1). Cross-references to other LOCKED docs. |

### §2.2 — Methodology and governance

| Document | Why recon reads it | What to extract |
|---|---|---|
| `METHODOLOGY.md` | Process discipline. K-Lessons #1–#8 are recon's quality bar. | All K-Lessons. The §3 "stop, escalate, lock" rule. §7.1 determinism invariant. §12.7 closure protocol. |
| `FRAMEWORK.md` | REGISTER schema. | Schema fields, lifecycle states, category/tier conventions, audit-calendar mechanics. |
| `SYNTHESIS_RATIONALE.md` | Why the discipline is what it is. | Source-standard mapping. Principle behind each adopted requirement. |

### §2.3 — Live planning documents (Category C)

| Document | Why recon reads it | What to extract |
|---|---|---|
| `ROADMAP.md` | Active surface for development. Defines M-cycle milestones (M0–M10 per IDEAS_RESERVOIR's reference to "Phase 7 closure"). Any explicit mention of "dynamic map expansion" lives here. | Full milestone progression. Phase boundaries. Items in active backlog vs deferred. Any spatial/nav/GPU items. **Reconcile against G-series skeletons (§2.4): does ROADMAP reference G-series? Are the G milestones in the M-cycle?** |
| `PHASE_A_PRIME_SEQUENCING.md` | A'-cycle (kernel refactor sequence) post-A'.5 closure. | Current A'-cycle plan. Any item touching navigation, scheduler, or compute. |
| `IDEAS_RESERVOIR.md` | Post-release backlog — 9 reservoir items. | The 9 items, the §3 cross-feature combinations, §4 out-of-scope rejections. Specifically: where each reservoir item touches navigation or GPU compute. |

### §2.4 — G-series skeleton briefs (architectural intent, **primary source**)

These are **skeleton-state** architectural intents for the project's GPU compute and movement directions. They are not LOCKED specs; they are not authored-ready execution briefs. They are **the cleanest record of where the project's thinking is heading** in this domain, and recon must treat them as primary architectural sources alongside (not subordinate to) LOCKED documents.

Read every G-skeleton in full. The report's §9 maps them.

| Path | Recon reads for |
|---|---|
| `tools/briefs/G0_VULKAN_COMPUTE_PLUMBING_BRIEF.md` | The base architectural intent for Vulkan compute as the project's GPU substrate. What it sees as the kernel/compute boundary; what it commits to about determinism; how it relates to `GPU_COMPUTE.md` v2.0 (concordance? extension? supersession-in-intent?). |
| `tools/briefs/G1_MANA_DIFFUSION_BRIEF.md` | First compute-domain skeleton — mana diffusion as field problem. What it establishes as the field-compute pattern (read input field, write output field, determinism). |
| `tools/briefs/G2_ELECTRICITY_ANISOTROPIC_BRIEF.md` | **Direct dependency for the user's "electricity on GPU compute" decision** in K8.3+K8.4 v2.0 closure. The skeleton that defines what the electricity rework defers to. Anisotropic field model. |
| `tools/briefs/G3_STORAGE_CAPACITANCE_BRIEF.md` | Third compute-domain skeleton — storage as capacitance model. What it establishes about resource accounting on compute. |
| `tools/briefs/G4_MULTI_FIELD_COEXISTENCE_BRIEF.md` | **Critical** — how multiple compute fields coexist on the same GPU substrate. Defines the multi-field memory model, scheduling model, determinism model. |
| `tools/briefs/G5_PROJECTILE_DOMAIN_B_BRIEF.md` | "Projectile domain B" — recon discovers what this means by reading. Likely the second domain class (after fields), for discrete entity simulation on GPU. |
| `tools/briefs/G6_FLOW_FIELD_INFRASTRUCTURE_BRIEF.md` | **Critical for navigation recon.** Flow field as compute infrastructure. The skeleton that defines how flow fields are computed, stored, queried, invalidated, and consumed by movement. |
| `tools/briefs/G7_VANILLA_MOVEMENT_BRIEF.md` | **Critical for navigation recon.** Vanilla movement as a game-side contract — what movement the game expects, how it consumes flow fields and/or other navigation outputs. What the player-visible movement contract is. |
| `tools/briefs/G8_LOCAL_AVOIDANCE_BRIEF.md` | **Critical for navigation recon.** Local-scale avoidance — the "ad-hoc CPU pathfinding" axis of the hybrid model. What avoidance algorithms are committed to; what they read from the flow field; what they write per-pawn. |
| `tools/briefs/G9_EIKONAL_UPGRADE_BRIEF.md` | **Critical for navigation recon.** Eikonal solver as upgrade to base flow field — the "mass movement through GPU compute" axis. What an Eikonal upgrade adds; what triggers the upgrade; how it integrates with G6 base infrastructure. |

For each G-skeleton, recon extracts (in the report's §9):
- The skeleton's architectural intent in one paragraph (verbatim where possible)
- The on-disk state of the skeleton (line count, sections present, sections missing per skeleton-vs-authored-brief comparison with a precedent like `K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md`)
- Dependencies on K-series (which K must close before this G is authorable)
- Dependencies on LOCKED docs (which LOCKED doc would need amendment when this G is authored)
- The recon's reading of skeleton-vs-current-code gap

### §2.5 — K-series briefs (kernel refactor — recently executed lineage)

| Path | Why |
|---|---|
| `tools/briefs/K8_34_COMBINED_KERNEL_CUTOVER_BRIEF_V2.md` | The brief that produced current code state. Read for Q-COMBINED locks 1–8, §4 cutover design, §9 closure protocol. |
| `tools/briefs/K8_*` (every K8 brief) | The K8 lineage. Read each for what it added, what it deferred, what it superseded. Particularly K8.5_MOD_ECOSYSTEM_MIGRATION_PREP — if it's the next K-milestone, it's high-relevance. |
| `tools/briefs/K9_*` | K9 field storage + refresh patch. Compute substrate prerequisite. Critical for understanding what `IModComputePipelineApi` actually exposes. |
| Earlier K-briefs (K0..K7) | Read structurally — what they established, what survived to current code. Quote only where evidence demands. |

### §2.6 — A'-series briefs (refactor phases)

| Path | Why |
|---|---|
| `tools/briefs/A_PRIME_*` (every A' brief) | The A'-cycle — what each phase did. Particularly A'.4.5 (governance register enrollment), and the A'.5 = K8.3+K8.4 just-closed milestone. Recon understands what each A' phase accomplished and what comes next (A'.6+). |

### §2.7 — Halt reports

| Path | Why |
|---|---|
| `docs/scratch/A_PRIME_5/HALT_REPORT.md`, `A_PRIME_5_CONTINUED/HALT_REPORT.md`, `A_PRIME_5_CONTINUED/HALT_REPORT_PHASE_4.md` | The three K8.3+K8.4 halts. Each carries findings about premise drift that recon internalizes. | 

### §2.8 — Precedent

| Document | Why |
|---|---|
| `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` | **Format precedent for this recon's report.** Read in full early. Mirror its structure: numbered sections, "Read me first" framing, fact-vs-synthesis separation (Section 14 is the only synthesis section, clearly marked), severity-and-table style, terminal "Recommendation summary" as separate synthesis layer. |

---

## §3 — Required reads: code

The report's evidence chain quotes code by file and line range — not paraphrased descriptions.

### §3.1 — Navigation primary (dedicated analytical depth)

| Path | Why |
|---|---|
| `src/DualFrontier.AI/Pathfinding/NavGrid.cs` | The grid itself. Transcribe constructor, `SetTile`, constants. Identify every hardcoded dimension. |
| `src/DualFrontier.AI/Pathfinding/AStarPathfinding.cs` | The pathfinding algorithm. Identify dependencies on grid size, tile representation, cost model. |
| `src/DualFrontier.AI/Pathfinding/IPathfindingService.cs` (if exists) | The current abstraction boundary between systems and pathfinding implementation. |
| `src/DualFrontier.AI/` (full directory listing, then targeted reads) | The complete AI surface. What else lives there beyond pathfinding? |
| `src/DualFrontier.Systems/Pawn/MovementSystem.cs` | The primary navigation consumer. Post-cutover state. Transcribe `Update` body and event handlers. Note how `move.Path` (NativeComposite primitive) is used. |
| `src/DualFrontier.Components/Pawn/MovementComponent.cs` | The component that carries movement state. Identify what data is per-pawn vs what could be shared, what is grid-coordinate-relative vs world-relative. |
| `src/DualFrontier.Components/Shared/PositionComponent.cs` | Position representation. Coordinate type, range, signedness, embedded world-boundary assumptions. |
| `src/DualFrontier.Contracts/Math/GridVector.cs` | Grid coordinate primitive. Shape — `int X, Y`? Signed range? World-bound assumptions? |

### §3.2 — Scheduler and ECS kernel

| Path | Why |
|---|---|
| `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs` | Concurrency model. Post-cutover ctor signature. How systems are sequenced. Whether navigation can run on separate phase or thread. |
| `src/DualFrontier.Core/Scheduling/DependencyGraph.cs` | How systems declare data dependencies via `[SystemAccess]`. Whether navigation can declare reads on virtual "tile space" or only on components. |
| `src/DualFrontier.Core/ECS/SystemBase.cs` | Post-cutover surface. What primitives are exposed to systems. |
| `src/DualFrontier.Core/ECS/SystemExecutionContext.cs` | Post-cutover. The single channel between system and kernel. |
| `src/DualFrontier.Contracts/Attributes/SystemAccessAttribute.cs` | Read/write/bus declaration. Whether it can express "reads tile (x,y)" or only "reads component T". |

### §3.3 — Native kernel surface (GPU compute boundary)

| Path | Why |
|---|---|
| `src/DualFrontier.Core.Interop/NativeWorld.cs` | The managed-side surface to the C++ kernel. Post-cutover signatures for `AcquireSpan`, `BeginBatch`, `CreateMap`, `CreateComposite`, `InternString`. |
| `src/DualFrontier.Core.Interop/Bootstrap.cs` | Kernel bootstrap path. |
| `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs` | Type-id registry. Determinism contract per K-L4. |
| All files under `src/DualFrontier.Core.Interop/` (full directory listing then targeted reads) | Complete managed/native bridge surface. **Critical for GPU compute**: identify how compute would access ECS data. |
| All headers under `native/DualFrontier.Core.Native/include/` | C ABI. What the kernel exposes; what it could expose to compute. |
| `native/DualFrontier.Core.Native/src/` (structurally — directory listing + targeted reads) | The C++ implementation. Recon does not need every line, but does need to characterize what's there. |

### §3.4 — Mod API v3 and modding pipeline

| Path | Why |
|---|---|
| `src/DualFrontier.Contracts/Modding/IModApi.cs` (or equivalent location) | v3 surface. Whether a mod can register custom navigation. |
| `src/DualFrontier.Contracts/Modding/IManagedStorageResolver.cs` | Path β. Whether navigation could be mod-provided service. |
| `src/DualFrontier.Contracts/Modding/IModComputePipelineApi.cs` (or equivalent) | **Critical** — the compute-pipeline surface mods register against. Quote in full. |
| `src/DualFrontier.Application/Modding/ModRegistry.cs` | How mods are registered. Capability-check points. |
| `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs` | The ratification pipeline. Scheduler-rebuild path. |

### §3.5 — Application bootstrap and factories

| Path | Why |
|---|---|
| `src/DualFrontier.Application/Loop/GameBootstrap.cs` | Where world is bootstrapped. Where map dimensions, pawn count, item counts, factory seeds are decided. **The single densest concentration of "game assumptions" in the codebase.** |
| `src/DualFrontier.Application/Scenario/RandomPawnFactory.cs` | Factory for initial pawns. Post-cutover. How it interacts with NavGrid. |
| `src/DualFrontier.Application/Scenario/ItemFactory.cs` | Factory for initial items. How it uses excluded-positions and NavGrid. |
| `src/DualFrontier.Application/Loop/GameLoop.cs` | Main loop. Tick rate, pause/resume gating. |

### §3.6 — All ten production systems

Read every file under `src/DualFrontier.Systems/` post-cutover. Recon does **not** quote each in full — but identifies:
- Which systems read `PositionComponent`
- Which systems read `MovementComponent`
- Which systems publish events that influence navigation
- Which systems would be affected by a navigation-foundation rework
- Any system that hardcodes assumptions about map coordinates, walking distance, or spatial reach

### §3.7 — Tests (structural — coverage map, not evidence-quoted)

Read the directory structure of `tests/`. Note:
- Which tests exist for navigation specifically
- The K8.3+K8.4 closure gap: `Systems.Tests` was deleted and re-authored as smoke tests only
- Which tests would need re-authoring if navigation foundation lands

Do not read every test file in full. Identify structure and gaps. Section 13 (technical debt) records the coverage gap.

---

## §4 — Required reads: configuration

| Path | Why |
|---|---|
| `DualFrontier.sln` | Project graph. Every assembly and its references. |
| Each `*.csproj` for assemblies named in §3 | Per-project dependencies, `InternalsVisibleTo` grants, target frameworks, build conditions. |
| `.gitignore` | What is excluded from version control. CPP report identified gaps (`out/` not covered); recon does same sweep. |
| `tools/governance/sync_register.ps1` and companion scripts | Governance tooling. Recon notes existence, tests, documentation. |
| `tools/briefs/` directory listing | Historical brief inventory. Cross-reference against REGISTER category D entries. |

---

## §5 — Search sweeps

### §5.1 — Game-assumption sweep (Layer A input)

Grep entire `src/` tree for:
- `MapWidth`, `MapHeight`, `200`, `MAP_SIZE`, `WORLD_SIZE`, `WIDTH`, `HEIGHT` (as identifiers, not in comments)
- `InitialPawnCount`, `MaxPawns`, `50`, `100`, `200` (pawn-count contexts)
- `coreSystems`, `CoreSystemCount`, `12`, `10` (system-count contexts post-cutover)
- `passable`, `walkable`, `Tile`, `Terrain` (every spatial-aware code site)
- `InitialFoodCount`, `InitialWaterCount`, `InitialBedCount`, `InitialDecorationCount`, `ObstacleCount` (factory contexts)
- `Seed`, `FactorySeed`, `42`, `43` (every hardcoded RNG seed)
- `mapsize`, `worldsize`, `boundsize` (case-insensitive)
- Strings: "200x200", "200×200", "fixed-size", "fixed size"

### §5.2 — Drift sweep (documentation vs code)

For each LOCKED architecture document, sample 3 specific implementation claims and verify against current code. Record drift instances. **Sample-verify enough to characterize drift rate; do not exhaustively verify every claim.**

### §5.3 — G-skeleton vs code alignment sweep

For each G-skeleton (G0..G9), identify any concept it introduces (e.g. "flow field cell", "Eikonal solver", "anisotropic field", "local avoidance step") and grep `src/` for related identifiers. Findings:
- Skeleton concept already partially implemented in code → record where, characterize maturity
- Skeleton concept absent from code (expected — they're skeletons) → confirm absence
- Skeleton concept implemented under a different name or shape than skeleton suggests → record as **drift between skeleton intent and code intent** (high-value finding)

### §5.4 — Orphan and stale sweep (Layer B input)

- Every `.uid` file under `src/` and `tests/` without matching `.cs`. Known seeds post-K8.3+K8.4: `World.cs.uid`, `ComponentStore.cs.uid`, `IsolationViolationException.cs.uid`.
- Every `// TODO`, `// FIXME`, `// HACK`, `// XXX`, `// K8`, `// K9`, `// Phase 4`, `// Phase 5`, `// deferred`, `// stale`, `// remove`, `// TEMP` in `src/` and `tests/`. Cluster by theme.
- Every reference to deleted-post-cutover entities: `World` (the type), `ComponentStore`, `IsolationViolationException`, `IsolationDiagnostics`, `IsolationGuardException`, `[Allowed]`, `ElectricGridSystem`, `ConverterSystem`, `EtherGridSystem`, `PowerConsumerComponent`, `PowerProducerComponent`, `IPowerBus`. Any straggler — comment, XML doc, README, doc — is a finding.
- Every site where `SpanLease`, `WriteBatch`, `EntityId`, `Version` appear together (known finding: `SpanLease<T>.Pairs` hardcodes Version=1 but native init sets Version=0). Record workaround pattern across sites.

### §5.5 — REGISTER vs disk sweep

- Every REGISTER document with `path` field — verify file exists on disk. Missing paths = Layer B findings.
- Every document file under `docs/architecture/`, `docs/methodology/`, `docs/governance/`, `docs/reports/`, `docs/research/` — verify REGISTER entry. Unregistered files = findings.
- Every `tools/briefs/*.md` — verify REGISTER presence. **Particular attention**: G-series skeletons (per §1.3).

### §5.6 — Coverage sweep

Per-system test coverage map: for each of 10 production systems, find every test file referencing it. Identify systems with **no** behavioral test (smoke tests don't count). Informational, not evidence-quoted. Section 13 records gap with estimated re-author scope.

---

## §6 — Output specification

### §6.1 — File location

`docs/reports/ARCHITECTURE_RECON_REPORT.md`. New file. The `docs/reports/` directory exists per CPP_KERNEL_BRANCH_REPORT precedent — verify.

### §6.2 — Frontmatter

```yaml
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-ARCHITECTURE_RECON_REPORT
category: E
tier: 2
lifecycle: Live
owner: Crystalka
version: "1.0"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-ARCHITECTURE_RECON_REPORT
---
```

The report file is created with this frontmatter; REGISTER.yaml itself is **not** edited in this brief — a separate governance action the user takes later.

### §6.3 — Section structure

The report follows `CPP_KERNEL_BRANCH_REPORT.md`'s structure adapted for architecture-wide subject:

1. **Read Me First** — 3–5 load-bearing facts that dominate everything below. Example seeds: "Managed `World` is a test fixture only post-K8.3+K8.4. G-series briefs are skeletons, not authored-ready. Recon is read-only, the report does not commit work." Author whichever facts the actual evidence requires.
2. **Project Overview** — repo HEAD, current branch, REGISTER schema/register version, document count, total LOC, milestone summary. Tables.
3. **REGISTER inventory** — by category (A–E) and lifecycle. Document count per category. Critical document IDs and versions. REGISTER inconsistencies.
4. **LOCKED architecture map** — per-document summary of what each LOCKED doc commits to, invariants locked, current version. One subsection per Category-A document. The project's architectural promises.
5. **Navigation deep-dive** — **analytical heart of the report**. Subsections:
   - 5A.1 Current `NavGrid` implementation (verbatim constructor, constants, methods, line ranges)
   - 5A.2 Current `AStarPathfinding`
   - 5A.3 `MovementSystem` post-cutover
   - 5A.4 Component layer (`PositionComponent`, `MovementComponent`, `GridVector` — coordinate semantics)
   - 5A.5 Hardcoded assumptions surfaced (table: file, line, constant, what it ties down)
   - 5B.1 G6 flow field infrastructure — skeleton intent verbatim
   - 5B.2 G7 vanilla movement — skeleton intent verbatim
   - 5B.3 G8 local avoidance — skeleton intent verbatim
   - 5B.4 G9 Eikonal upgrade — skeleton intent verbatim
   - 5B.5 How G6/G7/G8/G9 interlock with each other (per the skeletons themselves)
   - 5C.1 Code-vs-skeleton interlock map (where code is moving in skeleton direction, where it's pinned to pre-skeleton assumptions)
   - 5C.2 The dynamic-map-expansion requirement: what would change in each subsystem
   - 5C.3 The GPU-compute boundary: what `GPU_COMPUTE.md` v2.0 enables, what `IModComputePipelineApi` exposes, what navigation would need from compute, what G0/G4 commit to
   - 5C.4 Mod-extensibility of navigation: what Mod API v3 allows, what would need extension
6. **Scheduler and ECS kernel** — concurrency model, dependency-graph mechanics, navigation's place in the schedule, constraints scheduler imposes on navigation.
7. **Native kernel and compute boundary** — current surface (post-K8.4 cutover), C ABI exposure, headroom for compute-pipeline expansion.
8. **Mod API v3 surface** — current surface, Path α vs Path β, capabilities, what's reserved. **Cross-reference G-series**: do skeletons assume Mod API surface that doesn't exist yet?
9. **G/K/A' series brief inventory and interlock** — **dedicated mapping section**. Subsections:
   - 9.1 G-series (G0..G9) — per-skeleton summary (intent, file size, sections present, dependencies on K/LOCKED, gaps)
   - 9.2 K-series — every K brief, lifecycle state, what it ships/shipped
   - 9.3 A'-series — every A' brief, what each phase did
   - 9.4 Series-to-series dependency graph (which K closures unblock which G authorings; which A' phase carries which K)
   - 9.5 G-skeleton vs code drift findings (where skeletons would need updating before authoring; where code has moved past skeleton implicitly)
10. **Production systems map** — all 10 surviving systems, what each writes/reads, dependency-graph picture, pressure points.
11. **Game-assumption inventory** (Layer A) — every place architecture specializes to current game assumptions. Tables. Severity. Incidental vs load-bearing.
12. **Drift inventory** — documented behavior vs actual, by document. Sampled, not exhaustive.
13. **IDEAS_RESERVOIR through architectural lens** — for each of 9 reservoir items, what current architecture supports, what would require engine extension, what would require LOCKED amendment. The answer to "is the architecture ready for these post-release ideas?"
14. **Technical debt inventory** (Layer B) — orphans, stale comments, deleted-but-referenced types, REGISTER/disk mismatches, `SpanLease.Pairs` Version mismatch, per-system test coverage gap, `.uid` orphans. Tables. Severity. Proposed cleanup window.
15. **Synthesis** — *this section is the only synthesis section, clearly marked* (per precedent). Recon's reading of what the facts mean for the next architectural decision. **No specific architectural designs.** Only: what questions does the user now have evidence to answer; what questions remain open; what should next deliberation prioritize.

### §6.4 — Report metadata (closing section)

Final section, table format, matching CPP report's "Report metadata":
- Generation date
- Generating session / branch / model
- Repo HEAD when generated
- REGISTER schema version
- Total documents read
- Total source files read
- Sweep result counts (game-assumption hits, drift hits, orphans, debt items, G-skeleton-vs-code-drift hits)
- Report size (KB)
- Halt status (no halts / halt at SC-X)

### §6.5 — Format conventions

- Tables for fact catalogs. Prose for interpretation.
- Quote code verbatim with line ranges (`file.cs:42–58`).
- Quote document excerpts verbatim with section IDs.
- Quote G-skeleton excerpts verbatim with `## §N` references.
- Severity labels: **High** / **Medium** / **Low** — use sparingly, only where severity is decision-relevant.
- Decimal-numbered subsections (§5.1, §5A.2, §5B.3). Match precedent.
- Language: English. Match precedent. No mid-document language switch.
- Tone: factual, sparse, dry. Avoid "interesting", "elegant", "concerning". Precedent does this well — calibrate against it.
- No emoji. No decorative formatting. Two horizontal rules per section transition, table headers — that's the visual vocabulary.

### §6.6 — Length expectations

Precedent (`CPP_KERNEL_BRANCH_REPORT.md`) is 101 KB. Recon's subject is **broader** (whole architecture + G-series + technical debt vs one experimental branch) but **less LOC-dense per fact** (much analysis is document/skeleton/surface, not source-line-dense). Expected size: **120–200 KB**. SC-6 halt at 200 KB is hard ceiling.

---

## §7 — What this recon explicitly does NOT do

- Does not propose architectural designs.
- Does not write briefs for future milestones.
- Does not advance G-series skeletons to authored-ready state.
- Does not patch any finding it surfaces.
- Does not run builds, tests, or benchmarks.
- Does not edit REGISTER, LOCKED docs, methodology, governance, or any brief.
- Does not edit any source file.
- Does not estimate effort for specific cleanup or rework.
- Does not commit or push.
- Does not invent risk grades for completeness.
- Does not include performance numbers unless committed artifacts already exist on disk.

---

## §8 — Provenance and lineage

```
2026-05-13  K8.3 v2.0 brief authored — halt 1 (storage-location)
2026-05-14  combined K8.3+K8.4 brief v1.0 — halt 2 (API-surface) → patch v1
            patch v1 execution — halt 3 (mid-transition drift)
            v2.0 brief authored from code, executed cleanly (4 commits, b903b91 → fc8ecb6)
2026-05-15  deliberation: architecture-vs-game-assumptions question raised;
            G-series skeletons discovered as primary architectural intent record;
            full reconnaissance scoped to include 3 layers (A game-assumptions,
            B technical debt, C G/K-series interlock); CPP_KERNEL_BRANCH_REPORT.md
            identified as format precedent
2026-05-15  this brief authored
2026-XX-XX  recon execution — one Claude Code session — one report produced
2026-XX-XX  deliberation phase resumes with recon report as ground truth;
            user decides scope of ratification proposal / future milestones
```

---

## §9 — Reading sign-off

Before executing, Claude Code confirms:

1. **Read-only reconnaissance.** No code changes. No document edits. No commits. Single deliverable: `docs/reports/ARCHITECTURE_RECON_REPORT.md`.
2. **REGISTER.yaml is entry point.** Read first. Build document index. Follow REGISTER to discover documents not in §1.2.
3. **G-series briefs are skeletons.** Treat as primary architectural intent. Read in full. Map but do not advance.
4. **Precedent is `CPP_KERNEL_BRANCH_REPORT.md`.** Read early. Match structure, tone, fact/synthesis discipline.
5. **Navigation gets deepest analytical treatment.** Other subsystems characterized to depth needed for context.
6. **Three analytical layers in one report.** Layer A (architecture-vs-game-assumptions, §11), Layer B (technical debt, §14), Layer C (G/K-series interlock, §9) all produced. They interlock; not independent.
7. **Halt is the right answer if a premise is wrong.** Three halts protected K8.3+K8.4. A recon halt protects the next deliberation.
8. **Section 15 is the only synthesis section.** Everything else is fact. Synthesis is one place, clearly marked.

**Brief end. Execution begins at §1 (REGISTER.yaml as entry point).**
