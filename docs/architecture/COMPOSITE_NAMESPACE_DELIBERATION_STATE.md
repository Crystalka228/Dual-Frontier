---
register_id: DOC-A-COMPOSITE_NAMESPACE_DELIBERATION_STATE
project: Dual Frontier
category: A
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-15
last_modified: 2026-05-15
content_language: mixed
next_review_due: null
title: Composite Milestone Namespace — Deliberation Final State
review_cadence: on-status-transition
last_review_date: 2026-05-15
last_review_event: Deliberation closed 2026-05-15 — 9 of 10 Q explicitly locked, Q-K-1 instruction-only with retroactive lock mechanism; ratified ratification proposal brief execution
reviewer: Crystalka
special_case_rationale: Deliberation state document behaves like amendment plan (EXECUTED post-deliberation, not LOCKED); Category A + Tier 3 + EXECUTED override per Pass 2 §1.3 precedent (cf. DOC-A-K_L3_1_AMENDMENT_PLAN, DOC-A-A_PRIME_0_7_AMENDMENT_PLAN)
---

# Composite Milestone Namespace — Deliberation Final State

**Authored**: 2026-05-15 evening (Claude Opus 4.7, deliberation session)
**Status**: CLOSED — 9 of 10 Q explicitly locked, 1 (Q-K-1) instruction-only with retroactive lock mechanism
**Replaces**: Previous revision of this document (7 of 10 Q locked, in-progress state)
**Purpose**: Persistent record of completed deliberation. Input to subsequent ratification proposal authoring session.
**Sister document**: `DUAL_FRONTIER_CHRONOLOGY.md` (project chronology, separate)
**Source recon**: `docs/reports/M_G_R_NAMESPACE_CASCADE_MAP.md` (cascade map, 2026-05-15)

---

## 1 — Context

Делиберация по cascade map'у, который surfaced 10 open questions about composite milestone namespace. Cascade map = mechanical inventory of milestone ID references; deliberation = architectural decisions resolving each Q.

**Project nature**: Dual Frontier — research framework, not game (per README.md DOC-G-README LOCKED Live). Claim under test: solo dev as contract architect builds non-trivial systems-software via LLM pipeline. M-series vanilla content = demonstration surface на substrate'е.

**Composite namespace принцип**: IDs кодируют substrate dependency в имени. Substrate buckets (K, A', V). Demonstration buckets (M-K, M-V).

---

## 2 — Discussion mode rules (accepted 2026-05-15)

- One Q at a time
- Per-Q memory fixation when locked
- Q-by-Q ratification (not all-at-once)
- Return allowed if prior Q changes
- No file edits until all 10 Q done (deliberation mode only)

**Classification:**
- **Class A formal** — Q-R-1, Q-M-2
- **Class B architectural** — Q-G-1, Q-G-2, Q-V-2
- **Class C organizational** — Q-M-1, Q-M-3, Q-V-1, Q-K-1, Q-R-2

**Order followed:** architectural first (B), then formal (A), then organizational (C).

---

## 3 — Locked Q decisions (9 of 10)

### 3.1 — Q-G-1 LOCKED 2026-05-15

**Question:** G0 substrate identity — `G` retained or merge с runtime?

**Decision:** R and G merged into unified Vulkan substrate **V**.

**Implications:**
- RUNTIME_ARCHITECTURE.md + GPU_COMPUTE.md restructure into single Vulkan substrate doc с двумя use cases (rendering + compute).
- G0 plumbing = core V infrastructure (not separate milestone, it's the substrate foundation).
- G1..G6, G9 = potential demonstration content on V (final disposition resolved by Q-G-2).

**Rationale:** Both LOCKED docs (RUNTIME v1.0, GPU_COMPUTE v2.0) describe G и R as "shared VkInstance/VkDevice" — physically one Vulkan device. Treating их as separate substrates was a documentation drift. Documentation drift principle (см. раздел 6) applied: restructure только where demonstrably drift.

### 3.2 — Q-G-2 LOCKED 2026-05-15

**Question:** Disposition of G1..G6, G9 (former G-skeletons) under composite namespace.

**Decision:** V substrate = **two-layer topological scalar field model**.

**Substrate primitives** (substrate-building milestones, in correct order):
- **V0** — Vulkan compute pipeline plumbing (бывший G0)
- **V1** — Scalar field type + **diffusion shader** (environmental layer). Distribution by gradient + anisotropy. Heat, fire, ambient mana, weather, spillage.
- **V2** — Scalar field type + **wave shader** (routed layer). Propagates through discrete topology overlay. Breakable. Distance/direction field as side product. Persistent K9 storage.

**Demonstration usage patterns (mod-level concerns, NOT substrate):**
- **Distribution fields** (mana, electricity, water, heat, fire) — global field per type configured by vanilla mod.
- **Navigation** (Vanilla.Movement) — **three-mode hybrid dispatcher**:
  - **Autonomous baseline** — GPU persistent direction fields for recurring destination types. All autonomous pawns read these fields; no per-pawn pathfinding. Scaling O(destination types), pawn-count-independent. **Это hot loop.**
  - **Small player command (≤10 pawns)** — CPU per-pawn pathfinding. Threshold gameplay tuning.
  - **Mass event (10+)** — one GPU wave shader dispatch с temporary destination field.
- **Local avoidance** — mod concern, NOT substrate.

**Упразднено:**
- G3 storage cells / capacitance → gameplay-level node config (dynamic spike на топологии, не shader feature)
- G6 flow field infrastructure → folded into V2 (distance/direction = wave shader side products)
- G9 eikonal upgrade → possibly folded into V2 tunable parameters; deferred to amendment authoring
- G4 multi-field coexistence → V substrate close acceptance criteria, не отдельная milestone

**Deferred to amendment authoring:**
- G5 projectile Domain B — separate compute domain (не wave/diffusion); whether projectile stays in V or own substrate decided at amendment time
- G9 eikonal upgrade — folded into V2 tunable or separate primitive — evidence-gated

**Rationale chain (six successive reductions during deliberation):**
1. Storage = dynamic spike node на топологии (не отдельный shader feature)
2. Wave shader motivated by gameplay constraint (broken cable / pipe / road)
3. Two-layer model: diffusion (environment) + wave (routed)
4. Distribution + navigation unified (дорога = труба для пешек)
5. CPU/GPU policy decoupled from substrate primitives
6. Autonomous = GPU persistent fields (hot loop benefits from batching), small command = CPU exception

См. Lesson #11 candidate (раздел 6.3).

### 3.3 — Q-V-2 LOCKED 2026-05-15

**Question:** Multi-substrate vanilla mods naming (mod uses K + V).

**Decision:** Split per substrate dependency. C# assembly stays one, milestone split = work phasing.

- **K-side** gets concrete `M-K{N}` id
- **V-side** gets **reserved compound marker** in form `M-K{N} / M-V` (format Q-R-1)
- **V-side concrete identifier deferred** until V substrate ready and V-side mod authoring approached

**Sequencing:** K-side milestone authored first, V-side after.

**Affected multi-substrate mods:** Vanilla.Magic, Vanilla.Electricity, Vanilla.Water, Vanilla.Movement.

**K-only mods unchanged** (single milestone, K-side only): Vanilla.World, Vanilla.Pawn, Vanilla.Inventory, Vanilla.Core.

**Pattern precedent:** Follows `IHomomorphicComputeProvider.cs` (FHE_INTEGRATION_CONTRACT.md) ratified-but-dormant pattern. Interface ratified, concrete implementation appears when trigger conditions met. Applied here: K-side milestone ratified, V-side milestone reserved, identifier concretized when V substrate ready.

**MIGRATION_PLAN amendment** within Q-G-1 restructuring scope.

### 3.4 — Q-R-1 LOCKED 2026-05-15

**Question:** Naming format for V (после Q-G-1 merge R+G→V).

**Decision:**
1. **Substrate bucket name** = `V` (single letter, symmetric with K).
2. **Substrate primitives format** = `V0..V2` flat numbering (V0 plumbing, V1 diffusion, V2 wave; future extensions V3+).
3. **M-V demonstrations format** = `M-V{original G number}` preserving traceability к G-skeleton briefs (M-V1 mana, M-V2 electricity, M-V7 movement, M-V8 local avoidance; gaps M-V3/4/6/9 reflect Q-G-2 упразднения).
4. **Compound marker for multi-substrate mods** = `M-K{N} / M-V` with slash separator (V-side concrete identifier deferred to V-side authoring per Q-V-2).

### 3.5 — Q-R-2 LOCKED 2026-05-15

**Question:** M9 collision (M-cycle Vanilla.Combat vs M9.x runtime).

**Decision:** Collision resolved by prior locks, no separate decision needed.

- Runtime side: M9.0..M9.8 → V0..V2 + future V-N (per Q-G-1 + Q-G-2)
- Combat side: Vanilla.Combat deferred from cascade map scope (раздел 4)
- M9 namespace becomes free
- `M-K9` slot may be assigned later via Q-M-2 to some non-Combat pending vanilla mod
- Textual cleanup handled by MIGRATION_PLAN amendment within Q-G-1 restructuring scope

### 3.6 — Q-M-2 LOCKED 2026-05-15

**Question:** Pending M-cycle vanilla content (M8.x, M10.x, etc.) rename format.

**Decision:** All vanilla mods → M-K bucket. Format pattern = prefix insertion (M{X} → M-K{X}). **All specific renames deferred** to MIGRATION_PLAN amendment authoring time, when each mod approached.

Multi-substrate mods get K-side в M-K bucket + V-side reserved M-V marker per Q-V-2.

**Affected:** Vanilla.World, Vanilla.Pawn (3 sub-milestones), Vanilla.Inventory, Vanilla.Core, Vanilla.Magic K-side, Vanilla.Electricity K-side, Vanilla.Water K-side, Vanilla.Movement K-side. **Combat excluded** (раздел 4). **Closed M0..M7 handled by Q-M-1**.

**Pattern:** FHE-style reserved pattern (Q-V-2 precedent) applied symmetrically to all vanilla mods — они consumers, not substrate. Concrete identifier appears when authoring approached.

### 3.7 — Q-M-1 LOCKED 2026-05-15

**Question:** M-cycle M0..M7 closed phases retroactive rename or preserved historical.

**Decision:** Closed M0..M7 phases **retained as historical record**, NOT retroactively renamed. Closure reviews (M3-M7), audit passes, historical prompts keep original M-cycle names as shipped.

**Boundary:** Composite namespace applies M8.x forward only. Pre-namespace M-cycle (M0..M7) preserved. Ratification proposal documents this boundary explicitly.

**Rationale:**
- Historical record preservation principle (research framework documents what shipped under what convention)
- K-series precedent: K0..K9 closed phases retained under Q-G-1 LOCK — same applies M0..M7
- Cascade volume saved: ~370 refs across ~25 closed-record docs
- Natural boundary marker between pre-namespace и composite-namespace eras

### 3.8 — Q-M-3 LOCKED 2026-05-15

**Question:** Deferred-but-named milestones (M3.4, M3.5).

**Decision:** Treated as **pending under Q-M-2 scope**, not as Q-M-1 historical.

M3.5 trigger (K9 close) already met; both M3.4 and M3.5 are active pending awaiting authoring. Q-M-2 LOCK pattern applies:
- Bucket: M-K
- Specific identifier deferred to MIGRATION_PLAN amendment authoring
- References в MOD_OS_ARCHITECTURE §11.1 and MIGRATION_PLAN get mechanical bucket prefix insertion at amendment time
- Deferral notes preserved («M-K3.4 (deferred trigger met at K9 close, awaiting authoring approach)»)

**Not Q-M-1 scope** — these are not closed phases, they are pending with trigger conditions met.

### 3.9 — Q-V-1 LOCKED 2026-05-15

**Question:** Vanilla.X mod-name scope для rename.

**Decision:** Composite namespace ratification touches **milestone-ID labels only**, **not Vanilla.X mod type names**.

**In scope of rename:**
- Milestone-ID labels in `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §6.2 mappings
- Milestone-ID references в architectural docs (`GPU_COMPUTE.md`, `MOD_OS_ARCHITECTURE.md` §11, `ROADMAP.md`)
- Milestone-ID references в module READMEs (`mods/.../README.md`)
- Milestone-ID doc-comments в mod skeleton C# files (`mods/.../*Mod.cs`)

**NOT in scope of rename:**
- Assembly names (`DualFrontier.Mod.Vanilla.Combat.dll` etc.)
- C# namespaces (`namespace DualFrontier.Mod.Vanilla.Magic`)
- Type names (`CombatMod`, `MagicMod`, etc.)
- Folder structure (`mods/DualFrontier.Mod.Vanilla.X/` paths preserved)
- Mod-OS manifest entries pointing to assembly identities

**Rationale**: Mod identity (Vanilla.X) is **stable, persistent type identity** required by Mod-OS architecture (capability system, ALC isolation, manifest resolution). Milestone identity is **work-phasing identifier**, attached к mod work but separate from mod identity. Composite namespace operates on **work-phasing layer**, not type-identity layer.

FHE pattern consistency: encoding substrate dependency in type name would force premature commitment, violating reserved-but-dormant pattern (Q-V-2 precedent).

---

## 4 — Deferred items registry

| Item | Type | Reason | Trigger / future deliberation |
|---|---|---|---|
| **Vanilla.Combat** | Consumer mod | Combat это consumer of substrate, не identity question. Гейплейные механики не должны диктовать substrate. | Post-substrate deliberation when full V infrastructure ready and Combat requirements can be analyzed against готовую substrate |
| **G9 eikonal upgrade** | Substrate primitive (possibly) | Может folds into V2 wave shader tunable parameter, или отдельный primitive — evidence-gated | GPU_COMPUTE.md amendment authoring time |
| **G5 projectile Domain B** | Compute domain | Не wave/diffusion shader; separate compute domain. Может остаться в V (как третий primitive), может быть own substrate, может быть consumer-level | Future deliberation post-substrate |
| **Per-vanilla-mod K-side identifiers** | Specific milestone names | Per Q-M-2: all vanilla mods deferred symmetrically | MIGRATION_PLAN amendment authoring, when each mod approached |
| **V-side identifiers for multi-substrate mods** | Specific milestone names | Per Q-V-2: V-side reserved, concrete identifier appears when V substrate ready and mod approached | V-side authoring time (после V substrate primitives done) |
| **Hybrid coupling formal spec** | V substrate behavior | How diffusion picks up from broken wave node (вода в трубе → diffusion на разрыве) | GPU_COMPUTE.md amendment authoring |
| **Threshold N for navigation dispatcher** | Gameplay tuning | ~10 placeholder; final tuning gameplay-level | Vanilla.Movement authoring |

---

## 5 — Q-K-1 — instruction-only with retroactive lock mechanism

**Question:** K8.5 / A'.6 / A'.7 identity overlap.

**Surfaced by cascade map recon as register-of-truth conflict between LOCKED docs:**
- `KERNEL_ARCHITECTURE.md` Part 2: «K8.5 — Mod ecosystem migration prep»
- `PHASE_A_PRIME_SEQUENCING.md` §2: «A'.6 = K8.5» AND «A'.7 = K8.5 skeleton execution»

**Status:** NOT explicitly locked. Resolution path through **ratification proposal brief instructions** + **retroactive lock after execution**.

**Reasoning for retroactive approach:**

Deliberation в чате не deep-read'нула KERNEL_ARCHITECTURE Part 2 и PHASE_A_PRIME_SEQUENCING §2 verbatim. Synthesizing principle ('K8.5 canonical') без verbatim ground truth = potential premature commitment without verification. Future ratification proposal execution session anyway будет читать обе LOCKED docs verbatim для applying Q-G-1 restructuring cascade. Therefore Q-K-1 reconciliation **natural extension** of same execution pass.

**Ratification proposal brief instructions for Q-K-1:**

1. **Read both LOCKED docs verbatim** during execution session
2. **Apply principled resolution** based on these recommendations (NOT ratified principles, validate against verbatim content):
   - **Canonical identifier recommendation**: K8.5 is kernel-series milestone identifier (Part 2 origin)
   - **A'.6 / A'.7 nature**: These are A'-cycle sequencing labels pointing TO K8.5, not alternate identities
   - **Format suggestion**: «A'.7 phase: executes K8.5 brief»
   - **Other docs cross-referencing**: align to K8.5 canonical
3. **Halt if recommendation contradicts verbatim content** — return to deliberation if conflict
4. **Report what was actually found and what reconciliation was applied** in execution output

**Retroactive lock mechanism:**

After execution session completes and reports actual findings/resolution, **subsequent deliberation session** (after execution complete):
- Reviews execution report
- Ratifies actual resolution as Q-K-1 LOCK retroactively
- Updates this document с retroactive lock entry в Section 3
- Removes Q-K-1 from this Section 5

If execution surfaced conflict that contradicts recommendations → deliberation revisits Q-K-1 с verbatim ground truth.

**Aligns с Lesson #7 (transcribe verbatim) — extends it to deferred ratification.** Lock applies after ground truth read, not from synthesis interpretation.

---

## 6 — Surfaced principles + lesson candidates

These are principles that emerged during deliberation. Recorded here для historical record. **Formalization deferred** to next METHODOLOGY revision (METHODOLOGY currently v1.8 per `DUAL_FRONTIER_CHRONOLOGY.md`).

### 6.1 — Documentation drift principle (locked 2026-05-15, Q-G-1 deliberation)

Restructure documentation only where it **demonstrably drifts**: scattered descriptions of one reality, stale assumptions, divergent duplicates. Criterion = drift, not scope.

Applied в Q-G-1: RUNTIME + GPU_COMPUTE restructure into unified Vulkan substrate doc (currently described as if two independent systems, but physically one VkInstance/VkDevice).

Subsequently applied to KERNEL/PHASE_A_PRIME via Q-K-1 reconciliation в same pass — drift fixed where discovered, no carry-over debt.

### 6.2 — FHE-style reserved pattern (Q-V-2 application)

Architectural pattern: identity ratified, concrete details appear when trigger conditions met. Precedent: `IHomomorphicComputeProvider.cs` (FHE_INTEGRATION_CONTRACT.md) — interface ratified, method signatures appear when candidate library identified and evaluated.

Applied to namespace problem:
- K-side milestone ratified (M-K{N}), V-side milestone reserved (M-V marker), concrete V-identifier appears when V substrate ready (Q-V-2)
- Symmetrically applied to all vanilla mods in Q-M-2 — bucket assignment ratified, all specific renames deferred
- Extended to Q-K-1 as retroactive lock pattern — identity surfaced, concrete resolution after execution ground truth

### 6.3 — Lesson candidates (for METHODOLOGY revision)

**Lesson #9 candidate — Survey phase before brief authoring**

When "fully update LOCKED" arises, switch authoring → survey mode. Briefs are for known scope; surveys define scope. Architecture Recon 2026-05-15 first application.

Carried forward от pre-deliberation accumulation. Awaits second application для formalization.

**Lesson #10 candidate — Architecture audit + technical debt inventory in one pass**

Debt signals where compromise hid architectural truth. Architecture Recon 2026-05-15 first application (Layer B debt inventory alongside Layers A/C audit).

Carried forward от pre-deliberation accumulation. Awaits second application для formalization.

**Lesson #11 candidate — Architectural reduction methodology** (surfaced Q-G-2)

When deliberating substrate identity, **actively search for reductions** where complex setup hides simpler model. 

Q-G-2 had six successive reductions:
1. Storage as gameplay metadata (not shader feature)
2. Wave shader motivated by gameplay constraint (broken cable)
3. Two-layer model (diffusion + wave)
4. Distribution + navigation unified
5. CPU/GPU policy decoupled from substrate
6. Autonomous = GPU persistent (reversed naive intuition)

Each reduction made substrate **smaller**, gameplay **more expressive**. Final architecture: 3 substrate primitives (V0/V1/V2), все distribution/navigation/storage gameplay является **configuration** этих primitives.

**Pattern:** transcribe verbatim, never invert direction. Surface principle, не пересказывать.

**Lesson #12 candidate — Physical domain identity ≠ gameplay configuration identity** (surfaced mid-Q-M-2 when Crystalka corrected interpretation)

Substrate primitives должны быть named at **physical level**, не gameplay level. Electricity / mana / water / movement — это **gameplay configurations** одних и тех же physical primitives (scalar field + diffusion или wave shader), не отдельные substrate primitives.

**Pattern:** при определении substrate, поднять level abstraction — что общего в физическом поведении? Один shader = много gameplay domains.

Related к Lesson #11 (reduction methodology) but distinct: #11 = how to search, #12 = what to look for.

**Lesson #13 candidate — Retroactive lock pattern** (surfaced Q-K-1)

When deliberation cannot access ground truth without execution work, **instruct execution to discover and resolve**, **lock after execution report**. Avoids premature commitment based on synthesis without verbatim read.

Pattern preserves Lesson #7 (transcribe verbatim) discipline — extends it to deferred ratification when verbatim read requires execution.

Steps:
1. Surface question + recommendations during deliberation
2. Instruct future execution session to read verbatim + apply principled resolution
3. Execution reports actual findings/resolution
4. Subsequent deliberation ratifies as retroactive lock

Mechanism for когда synthesis может быть wrong but deep-read costly during deliberation.

---

## 7 — Cascade impact summary

**Из cascade map §9 + Q-locks:**

- **Total milestone references:** ~1889 across all artifacts
- **K/A' series unchanged:** ~1564 refs (Q-G-1 / Q-V-2 leave K, A' alone except Q-K-1 K8.5/A'.6/A'.7 reconciliation)
- **Cascade-zone refs:** ~325 (M-cycle pending + M9.x runtime + G-series + Vanilla.X)
- **Deferred per Q-M-2 (FHE-style):** ~80% of pending M-cycle volume — specific renames happen at amendment time, не at ratification time
- **MIGRATION_PLAN amendment scope** (Q-G-1 restructuring): RUNTIME_ARCHITECTURE.md + GPU_COMPUTE.md consolidation; MIGRATION_PLAN_KERNEL_TO_VANILLA.md bucket prefix updates; KERNEL/PHASE_A_PRIME Q-K-1 reconciliation; cleanups dual-meaning M9 refs

**Closed-record docs (M0..M7):** ~370 refs **preserved** (Q-M-1 LOCK), not touched

---

## 8 — Next steps

**Deliberation closed.** Next phase: ratification proposal authoring (separate session).

**Phases ahead:**

1. **Ratification proposal authoring** (separate deliberation session, likely):
   - Authors `RATIFICATION_PROPOSAL_COMPOSITE_NAMESPACE.md` document — formal proposal text
   - Encodes all 9 locks + Q-K-1 instructions as ratified architectural decisions
   - Identifies LOCKED-doc amendments needed
   - Crystalka ratifies proposal formally

2. **Ratification proposal execution brief authoring** (deliberation session):
   - Authors brief для Claude Code execution session
   - Brief instructs:
     - Q-G-1: RUNTIME + GPU_COMPUTE consolidation into unified V substrate doc
     - Q-V-2/Q-M-2/Q-M-3: MIGRATION_PLAN amendment cascade (bucket prefix insertions, deferred markers preserved)
     - Q-G-2: M-V demonstration references update в GPU_COMPUTE roadmap section
     - Q-M-1: M0..M7 closure docs explicitly preserved, boundary marker added
     - Q-V-1: scope discipline — milestone labels only, not type names
     - **Q-K-1 reconciliation**: read KERNEL Part 2 + PHASE_A_PRIME §2 verbatim, apply principled resolution per recommendations, halt if conflict, report actual findings
   - Brief uses staging path workaround for `create_file`

3. **Execution session** (Claude Code, autonomous tool-loop):
   - Applies cascade
   - Reports findings (especially Q-K-1 actual resolution)

4. **Retroactive Q-K-1 lock** (deliberation session post-execution):
   - Reviews execution report
   - Ratifies actual Q-K-1 resolution
   - Updates this document (Q-K-1 moves from Section 5 to Section 3 with retroactive lock entry)

5. **METHODOLOGY revision** (separate future work):
   - Formalizes lesson candidates (Lessons #9, #10, #11, #12, #13)
   - Triggered when accumulated changes warrant new METHODOLOGY version

**Memory management:**
- Q-M-3, Q-V-1 currently in temporary memory (entries #9, #10)
- After Crystalka confirms this document attached в Project files → memory cleanup (remove entries #9, #10)
- Lessons stay в memory как behavioral instructions для future Claude sessions

---

## 9 — Quick reference

**Substrate buckets locked:** K (kernel, K0..K9 closed), A' (refactor, A'.0..A'.5 closed), V (Vulkan unified — was R + G separate before Q-G-1 LOCK)

**Demonstration buckets locked:** M-K (vanilla on K), M-V (vanilla on V)

**V substrate primitives:** V0 (Vulkan plumbing), V1 (diffusion shader), V2 (wave shader)

**V demonstrations format:** M-V{original G number} preserving traceability (M-V1 mana, M-V2 electricity, M-V5 projectile deferred, M-V7 movement, M-V8 local avoidance; gaps M-V3/4/6/9 reflect Q-G-2 упразднения)

**Vanilla mods:**
- K-only (single M-K milestone): World, Pawn (3 sub-milestones), Inventory, Core
- K+V multi-substrate (split per Q-V-2, FHE-pattern reserved): Magic, Electricity, Water, Movement
- Deferred entirely: Combat (consumer, separate post-substrate deliberation)

**Pattern for multi-substrate mods:** `M-K{N} / M-V` with slash separator; V-side identifier deferred to V-side authoring time.

**Pre-namespace era preserved:** M0..M7 closed phases historical record (Q-M-1 LOCK), не renamed.

**Q-K-1 status:** instruction-only в ratification proposal brief; retroactive lock after execution report.

---

**End of final deliberation state document. 9 of 10 Q explicitly locked; Q-K-1 instruction-only with retroactive lock mechanism. Ready for ratification proposal authoring.**

**Recorded principles awaiting METHODOLOGY formalization:**
- Documentation drift principle (locked)
- FHE-style reserved pattern (extended pattern, locked)
- Lesson #9 candidate (survey phase, awaiting second application)
- Lesson #10 candidate (audit + debt inventory, awaiting second application)
- Lesson #11 candidate (architectural reduction methodology, surfaced Q-G-2)
- Lesson #12 candidate (physical domain ≠ gameplay configuration, surfaced Q-M-2)
- Lesson #13 candidate (retroactive lock pattern, surfaced Q-K-1)