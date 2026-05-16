---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-COMPOSITE_NAMESPACE_RATIFICATION_BRIEF
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-COMPOSITE_NAMESPACE_RATIFICATION_BRIEF
---
# Composite Milestone Namespace — Ratification Execution Brief

**Status**: AUTHORED 2026-05-15 evening.
**Mode**: Execution. Atomic commits per Q-target. `sync_register.ps1 --validate` gates each commit. No bypass.
**Subject**: Apply ratified composite milestone namespace decisions across LOCKED docs, briefs, REGISTER, READMEs, mod skeleton C# files. Performs documentation restructure + cascade amendments + Q-K-1 reconciliation in single pass.
**Ratification authority**: `COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` (Project files, finalized 2026-05-15). 9 of 10 Q explicitly locked, Q-K-1 instruction-only with retroactive lock mechanism.
**Predecessor context**: `DUAL_FRONTIER_CHRONOLOGY.md` (Project files); `docs/reports/M_G_R_NAMESPACE_CASCADE_MAP.md` (cascade map recon); `docs/reports/ARCHITECTURE_RECON_REPORT.md` (architecture audit).
**Navigation**: `docs/governance/REGISTER.yaml` (DOC-G level) + `REGISTER_RENDER.md` are single source of truth for file enumeration. Brief cites register IDs, not hardcoded file paths.

---

## ⚠ READ ME FIRST

Six load-bearing facts dominate everything below:

1. **All architectural decisions are ratified.** The deliberation state document encodes 9 explicit Q-locks (Q-G-1, Q-G-2, Q-V-2, Q-R-1, Q-R-2, Q-M-2, Q-M-1, Q-M-3, Q-V-1) plus Q-K-1 retroactive lock instructions. Execution applies ratified decisions; it does **not** revisit them. If execution encounters a case where ratified decision contradicts verbatim source content, halt and surface to deliberation.

2. **Lesson #7 governs every cascade target.** Transcribe verbatim. Do not paraphrase from synthesis. Every LOCKED-doc amendment must read source content verbatim before modification. Halt if synthesis from deliberation document contradicts verbatim source.

3. **Lesson #8 governs every commit.** Each atomic commit must leave the repository in a valid intermediate state. Tests green per relevant test suites. `sync_register.ps1 --validate` exit code 0. Build green where build affected.

4. **REGISTER.yaml is the navigation source.** Brief instructs Claude Code to query register entries by category (DOC-A architecture, DOC-D briefs, DOC-E reports, DOC-F module artifacts) for file enumeration. Hardcoded file lists in this brief are illustrative only — register is authoritative.

5. **Q-K-1 is retroactive.** Execution session reads KERNEL_ARCHITECTURE Part 2 + PHASE_A_PRIME_SEQUENCING §2 verbatim, applies principled reconciliation per recommendations, reports actual findings. Subsequent deliberation session ratifies actual resolution retroactively.

6. **Documentation drift principle (locked).** Restructure only where demonstrably drifts. Brief identifies specific drift targets per Q-target. Do not preemptively touch other documents.

These six facts are why this brief separates **lock principles section** (citing deliberation document) from **cascade work per Q-target** (citing register entries). The brief is **comprehensive** but **bounded** — explicit instructions, explicit scope, explicit halt triggers.

---

## ⚠ HALT TRIGGERS

Execution halts under any of the following. Author a HALT_REPORT in `docs/scratch/RATIFICATION_EXECUTION/HALT_REPORT_<timestamp>.md`; do not improvise:

- **SC-1** — Ratified decision contradicts verbatim source content. Halt, document verbatim citation, surface to deliberation for revisit. Examples: deliberation says "G1..G6 folded into V2" but `GPU_COMPUTE.md` §G6 describes content that does not fit folding pattern.
- **SC-2** — Q-K-1 recommendation conflicts с verbatim KERNEL Part 2 or PHASE_A_PRIME §2 content. Halt, document conflict, defer Q-K-1 reconciliation to follow-up deliberation. Other Q-targets proceed.
- **SC-3** — REGISTER.yaml fails to parse, or `sync_register.ps1 --validate` fails. Halt, document validation failure, surface для governance amendment.
- **SC-4** — Atomic commit boundary unclear: cascade work spans multiple LOCKED docs in a way that can't be split cleanly. Halt, request boundary clarification from deliberation. Example: MIGRATION_PLAN amendment and MOD_OS_ARCHITECTURE §11 both need same change, can they be one commit or must split?
- **SC-5** — Test suite or build fails on intermediate state. Halt, identify cascading impact, do not commit. Lesson #8 — intermediate states must be valid.
- **SC-6** — Brief reaches execution step requiring decision not covered by ratification document. Halt, surface to deliberation. Examples: «register entry naming convention for new unified V substrate doc» — not pre-decided, ask.
- **SC-7** — File enumeration via REGISTER.yaml returns unexpected count (e.g., expected ~12 docs touched by Q-G-1 cascade, actual returns 45). Halt, validate before proceeding. Possible REGISTER drift or scope misjudgment.

Halt is the right answer if a premise is wrong. Three K8.3+K8.4 v2.0 halts (precedent) protected the project from bad executions.

---

## ⚠ PROHIBITED ACTIONS

Claude Code MUST NOT, within this session:

- Re-deliberate ratified Q-locks. If verbatim source contradicts ratified decision → halt SC-1, not reinterpret.
- Make decisions on items marked **deferred** in deliberation document Section 4 (Vanilla.Combat, G9 eikonal, G5 projectile, per-vanilla-mod identifiers, V-side identifiers, hybrid coupling spec, threshold N).
- Rename Vanilla.X mod type names, assembly names, C# namespaces, folder structure (Q-V-1 explicitly forbids).
- Retroactively rename M0..M7 closed-record docs (Q-M-1 explicitly forbids — historical preservation).
- Touch any file in `docs/audit/`, `docs/prompts/` containing M0..M7 references (Q-M-1 scope).
- Commit without `sync_register.ps1 --validate` exit code 0.
- Push to remote within this session unless explicitly instructed in final step.
- Use any deletion that is not explicitly part of restructure scope (e.g., do not delete `GPU_COMPUTE.md` until unified V substrate doc is created and validated).

Permitted file operations:

- **Reading** any file in repository
- **Modifying** LOCKED docs per Q-target scope
- **Creating** new files explicitly required (e.g., unified V substrate doc replacing RUNTIME + GPU_COMPUTE)
- **Renaming/moving** files per ratified cascade
- **Creating** HALT_REPORT at `docs/scratch/RATIFICATION_EXECUTION/HALT_REPORT_<timestamp>.md` if halt triggers fire
- **Running** `sync_register.ps1 --validate` (and `--sync` if frontmatter regeneration needed)
- **Running** `dotnet build` and `dotnet test` where verification required by Lesson #8

---

## §0 — Brief structure overview

This brief is organized into ratified-locks section (§1, citing deliberation document) followed by cascade execution per Q-target (§2-§8). Final §9 handles Q-K-1 retroactive lock protocol. §10 handles commit discipline. §11 handles execution sign-off.

**Atomic commit sequence** (commits land on feature branch first, push protocol at final step):

1. **Commit 1**: Q-G-1 — RUNTIME + GPU_COMPUTE consolidation into unified V substrate doc
2. **Commit 2**: Q-V-2/Q-M-2/Q-M-3 — MIGRATION_PLAN amendment (bucket prefixes, deferred markers, multi-substrate marker convention)
3. **Commit 3**: Q-G-2 — GPU_COMPUTE roadmap section in new unified V doc (M-V demos cited per Q-R-1)
4. **Commit 4**: Q-M-1 — M0..M7 boundary documentation
5. **Commit 5**: Q-K-1 reconciliation per recommendations + verbatim validation
6. **Commit 6**: REGISTER amendments for renamed/restructured docs
7. **Commit 7**: Verification + sync_register --validate full pass

Each commit lands independently. Lesson #8 invariant: each intermediate state valid.

---

## §1 — Ratified locks (citing deliberation document)

This section restates lock summaries for execution reference. Full formulations in `COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` Section 3. **If discrepancy between brief summary and deliberation document, deliberation document authoritative.**

### §1.1 — Q-G-1: R + G merged into unified Vulkan substrate V

- Substrate bucket: **V** (single letter, symmetric с K)
- `RUNTIME_ARCHITECTURE.md` + `GPU_COMPUTE.md` → restructure into single unified V substrate doc
- Two use cases inside V: rendering + compute
- G0 plumbing = core V infrastructure (not separate milestone)

### §1.2 — Q-G-2: V substrate = two-layer topological scalar field model

**Substrate primitives:**
- **V0** — Vulkan compute pipeline plumbing
- **V1** — Scalar field type + diffusion shader (environmental)
- **V2** — Scalar field type + wave shader (routed, breakable, distance/direction as side product)

**Упразднено** (do not preserve in unified V doc):
- G3 storage cells → gameplay-level node config (dynamic spike)
- G6 flow field infrastructure → folded into V2
- G4 multi-field coexistence → V substrate close acceptance criteria
- G9 eikonal upgrade → possibly folded into V2 tunable parameters (deferred to amendment authoring — preserve note in V doc as TBD)

**Deferred** (preserve as TBD notes in V doc, do not resolve):
- G5 projectile Domain B — separate compute domain, substrate identity TBD

**M-V demonstrations** (cite in V doc roadmap):
- M-V1 mana (Vanilla.Magic)
- M-V2 electricity (Vanilla.Electricity)
- M-V7 movement (Vanilla.Movement)
- M-V8 local avoidance (Vanilla.Movement extension)
- Gaps M-V3, M-V4, M-V6, M-V9 reflect упразднения (no demonstration at those numbers)

**Navigation 3-mode dispatcher** (cite in V doc):
- Autonomous baseline — GPU persistent direction fields (hot loop)
- Small player command (≤10 pawns) — CPU per-pawn pathfinding
- Mass event (10+) — GPU wave shader dispatch с temporary destination field

### §1.3 — Q-V-2: Multi-substrate vanilla mods split per substrate

- K-side gets concrete `M-K{N}` id
- V-side gets reserved compound marker `M-K{N} / M-V` (slash separator)
- V-side concrete identifier deferred to V-side authoring
- Sequencing: K-side first, V-side after
- Affected: Vanilla.Magic, Vanilla.Electricity, Vanilla.Water, Vanilla.Movement
- K-only mods (single milestone): Vanilla.World, Vanilla.Pawn, Vanilla.Inventory, Vanilla.Core
- Pattern: FHE ratified-but-dormant precedent

### §1.4 — Q-R-1: V naming format

- Bucket: `V` single letter
- Substrate primitives: `V0..V2` flat numbering (future V3+ open)
- M-V demonstrations: `M-V{original G number}` (M-V1, M-V2, M-V7, M-V8)
- Compound marker: `M-K{N} / M-V` with slash separator

### §1.5 — Q-R-2: M9 collision resolved by prior locks

- Runtime side: M9.0..M9.8 → V0..V2 + future V-N (via Q-G-1+Q-G-2)
- Combat side: deferred (memory #25, Vanilla.Combat excluded from cascade map)
- M9 namespace freed
- Textual cleanup of dual-meaning M9 refs in MIGRATION_PLAN amendment

### §1.6 — Q-M-2: All pending vanilla → M-K bucket, specific renames deferred

- Format pattern: prefix insertion M{X} → M-K{X}
- All specific renames deferred to MIGRATION_PLAN amendment author (FHE-style symmetrically across all vanilla mods)
- Affects World, Pawn (3 sub-milestones), Inventory, Core, Magic K-side, Electricity K-side, Water K-side, Movement K-side
- Combat excluded entirely

### §1.7 — Q-M-1: M0..M7 closed phases preserved as historical

- M0..M7 closure reviews, audit passes, historical prompts NOT renamed
- Composite namespace applies M8.x forward only
- Boundary marker added explicitly

### §1.8 — Q-M-3: Deferred-but-named milestones treated as pending under Q-M-2

- M3.4, M3.5 → bucket M-K, identifier deferred
- Deferral notes preserved in references
- Not Q-M-1 scope (not closed phases)

### §1.9 — Q-V-1: Composite namespace touches milestone labels, NOT Vanilla.X type names

- In scope: milestone-ID labels в MIGRATION_PLAN mappings, architectural docs, READMEs, mod skeleton doc-comments
- NOT in scope: assembly names, C# namespaces, type names, folder structure, Mod-OS manifests
- Mod identity (Vanilla.X) stable per Mod-OS ALC isolation requirements

### §1.10 — Q-K-1: Retroactive lock via execution discovery

- Read KERNEL Part 2 + PHASE_A_PRIME §2 verbatim during execution
- Apply principled resolution per recommendations (NOT ratified principles, validate)
- Halt if recommendation contradicts verbatim (SC-2)
- Report actual findings for deliberation retroactive lock
- See §9 for detailed protocol

---

## §2 — Commit 1: Q-G-1 RUNTIME + GPU_COMPUTE consolidation

### §2.1 — Scope

Consolidate `RUNTIME_ARCHITECTURE.md` (LOCKED v1.0, DOC-A-RUNTIME register) и `GPU_COMPUTE.md` (LOCKED v2.0, DOC-A-GPU_COMPUTE register) в один unified doc describing V substrate.

**Target file name**: `docs/architecture/VULKAN_SUBSTRATE.md` (proposed)
**Target register ID**: `DOC-A-VULKAN_SUBSTRATE`
**Lifecycle**: LOCKED v1.0 (Live initial during transition, lock when content stable)

**SC-6 escalation**: if doc name `VULKAN_SUBSTRATE.md` или register ID conflicts с existing conventions, halt и surface — naming не pre-ratified here, ask.

### §2.2 — Source content extraction

Read **both** source docs verbatim:
- `docs/architecture/RUNTIME_ARCHITECTURE.md` (всё)
- `docs/architecture/GPU_COMPUTE.md` (всё)

Extract:
- Vulkan instance/device lifecycle (RUNTIME §1 + GPU_COMPUTE Architectural Integration)
- SPIR-V toolchain (RUNTIME §1.7)
- Validation layer (RUNTIME)
- Memory allocator (RUNTIME)
- Threading model (RUNTIME §L8)
- Compute pipeline plumbing (GPU_COMPUTE §G0 content)
- Field types и shader primitives (GPU_COMPUTE §G1, §G2 content — restructured per Q-G-2)

### §2.3 — Unified V substrate doc structure

Proposed sections (adjust as content dictates, halt SC-6 if uncertain):

1. **Overview** — V substrate = Vulkan layer obслуживающий rendering и compute
2. **Module structure** — DualFrontier.Runtime layout, shared VkInstance/VkDevice principle
3. **V substrate primitives** (per Q-G-2):
   - V0 — Vulkan compute pipeline plumbing
   - V1 — Scalar field type + diffusion shader
   - V2 — Scalar field type + wave shader
4. **Rendering use case** — render pipelines, swapchain, presentation (from RUNTIME)
5. **Compute use case** — compute pipelines, SPIR-V upload, dispatch, fence sync (from GPU_COMPUTE)
6. **Threading** (from RUNTIME §L8)
7. **Storage** — K9 field storage interaction (from GPU_COMPUTE)
8. **Navigation 3-mode dispatcher** (per Q-G-2 — moved here as demonstration usage pattern note)
9. **Roadmap** — V0/V1/V2 substrate sequencing; M-V demonstrations cited (M-V1, M-V2, M-V7, M-V8); gaps M-V3/4/6/9 noted as упразднения; G5/G9 deferred items as TBD
10. **Cross-references** — to KERNEL, MOD_OS, MIGRATION_PLAN, FIELDS

### §2.4 — Content restructure rules

- Preserve verbatim content where doctrine matches новой substrate identity (V is Vulkan unified)
- Restructure where current docs describe G и R as separate (Q-G-1 drift target)
- Remove G3 storage shader feature content (упразднён per Q-G-2)
- Fold G6 flow field content into V2 wave shader section (distance/direction as side products)
- Remove G4 multi-field coexistence as separate milestone (becomes V substrate close acceptance criteria — add to roadmap as criterion)
- Preserve G9 eikonal upgrade as deferred TBD note in V2 section
- Preserve G5 projectile Domain B as deferred note in roadmap

### §2.5 — Old docs disposition

**After** unified V substrate doc validates:
- Delete `docs/architecture/RUNTIME_ARCHITECTURE.md`
- Delete `docs/architecture/GPU_COMPUTE.md`
- REGISTER amendment (in Commit 6) removes DOC-A-RUNTIME and DOC-A-GPU_COMPUTE, adds DOC-A-VULKAN_SUBSTRATE

**Before deletion**:
- Validate unified doc content covers all material from both source docs (no information loss)
- Validate cross-references in other docs (KERNEL, MOD_OS, MIGRATION_PLAN, FIELDS) — they will be updated to cite new doc (in subsequent commits) but should not break in this commit

**Atomic commit boundary**: this commit lands new unified V doc + deletes two old docs in one commit. Repository state valid at end (no orphan references because subsequent commits update cite references).

### §2.6 — Commit 1 acceptance criteria

- `docs/architecture/VULKAN_SUBSTRATE.md` exists with proper frontmatter
- `docs/architecture/RUNTIME_ARCHITECTURE.md` deleted
- `docs/architecture/GPU_COMPUTE.md` deleted
- Unified doc content covers all material from sources
- V substrate primitives V0/V1/V2 explicit per Q-G-2
- Navigation 3-mode dispatcher described
- G3 упразднено, G6 folded, G4 as acceptance criteria, G9/G5 as TBD deferred
- M-V demonstrations cited (M-V1, M-V2, M-V7, M-V8) с gaps noted
- `sync_register.ps1 --validate` exit code 0
- Build green (no source code depends on these docs)

### §2.7 — Commit 1 message

```
A'.6/composite-ns: Q-G-1 RUNTIME + GPU_COMPUTE → unified VULKAN_SUBSTRATE

Ratification of Q-G-1 LOCK per COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §3.1:
R и G substrate buckets merged into unified Vulkan substrate V.
RUNTIME_ARCHITECTURE.md + GPU_COMPUTE.md consolidated.

V substrate primitives (per Q-G-2):
- V0 — Vulkan compute pipeline plumbing (бывший G0)
- V1 — Scalar field type + diffusion shader (environmental)
- V2 — Scalar field type + wave shader (routed, breakable)

Упразднено per Q-G-2: G3, G6 (folded into V2), G4 (acceptance criteria).
Deferred: G5 projectile Domain B, G9 eikonal upgrade.

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md (Project files).
```

---

## §3 — Commit 2: Q-V-2/Q-M-2/Q-M-3 MIGRATION_PLAN amendment

### §3.1 — Scope

Amend `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (LOCKED v1.1, DOC-A-MIGRATION_PLAN register) to encode:

- Composite namespace declared (M-K bucket для vanilla content на K substrate)
- Pending vanilla content milestones get **bucket prefix M-K**, **specific identifiers deferred**
- Multi-substrate mods get `M-K{N} / M-V` compound marker, V-side identifier deferred (Q-V-2 + Q-R-1)
- Deferred-but-named M3.4/M3.5 treated as pending (Q-M-3)
- Closed M0..M7 phases preserved as historical (Q-M-1 — explicit boundary)
- M9 dual-meaning cleanup (Q-R-2)
- Vanilla.Combat excluded entirely (deliberation memory #25)

**Cross-doc cite update**: any references to RUNTIME_ARCHITECTURE.md or GPU_COMPUTE.md updated to cite VULKAN_SUBSTRATE.md (created in Commit 1).

### §3.2 — Content amendments

**Section 6.2 vanilla → milestone mapping** restructure:

Current state (pre-amendment):
```
- Vanilla.World → M8.4
- Vanilla.Pawn → M8.5-M8.7
- Vanilla.Combat → M9
- Vanilla.Inventory → M10
- Vanilla.Magic → M10.B
- Vanilla.Core → M10 incremental
```

Post-amendment:
```
**Bucket assignments (composite namespace, M-K bucket):**
- Vanilla.World → M-K bucket (specific identifier deferred to authoring; currently M8.4 placeholder)
- Vanilla.Pawn → M-K bucket (3 sub-milestones, deferred; currently M8.5-M8.7 placeholder)
- Vanilla.Inventory → M-K bucket (deferred; currently M10 placeholder)
- Vanilla.Core → M-K bucket (deferred; currently M10 incremental placeholder)

**Multi-substrate mods (compound marker per Q-V-2):**
- Vanilla.Magic → M-K{N} / M-V (K-side deferred; currently M10.B K-side placeholder; V-side reserved, identifier appears when V substrate ready)
- Vanilla.Electricity → M-K{N} / M-V (K-side slot не yet assigned, deferred to amendment author; V-side reserved)
- Vanilla.Water → M-K{N} / M-V (K-side slot не yet assigned, deferred; V-side reserved)
- Vanilla.Movement → M-K{N} / M-V (K-side slot не yet assigned, deferred; V-side reserved)

**Deferred-but-named (Q-M-3):**
- M3.4 → M-K3.4 bucket assignment (deferral note preserved: trigger met at K9 close, awaiting authoring approach)
- M3.5 → M-K3.5 bucket assignment (deferral note preserved: capability registry refresh, K9 trigger met)

**Excluded from cascade map scope (separate post-substrate deliberation):**
- Vanilla.Combat — consumer mod, identity discussion deferred until V substrate ready
```

**Section M-cycle sequencing** amend:

Current state references `K8.2 → K8.3 → K8.4 → K8.5 → M8.4 → M8.5 → M8.6 → M8.7 → M9 → M10.x`.

Post-amendment: К-series stays, M-cycle pending parts get M-K bucket prefixes (specific identifiers deferred per Q-M-2 FHE pattern). M9 dual-meaning removed.

**Section M9.x runtime references** amend:

Any reference to M9.0..M9.8 runtime milestones → V0..V2 + future V-N (per Q-G-1 + Q-G-2 unification).

Example transformation:
- Before: «Runtime M9.0..M9.8 sequence per RUNTIME_ARCHITECTURE.md»
- After: «V substrate sequence V0/V1/V2 per VULKAN_SUBSTRATE.md (per Q-G-1 unification of M9.x runtime and G-series compute into V substrate)»

**Section pre-namespace boundary** add (Q-M-1):

New subsection added documenting boundary:
> «Composite namespace applies M8.x forward. Closed M-cycle phases M0..M7 preserved as historical record under pre-namespace convention (Q-M-1 LOCK). Closure reviews under `docs/audit/M*_CLOSURE_REVIEW.md` retain original M-series names as shipped. Audit passes и historical prompts likewise preserved.»

### §3.3 — Cross-references update

- All references to `RUNTIME_ARCHITECTURE.md` → `VULKAN_SUBSTRATE.md`
- All references to `GPU_COMPUTE.md` → `VULKAN_SUBSTRATE.md`
- M9.0..M9.8 numeric references → cite V substrate per Q-G-1

### §3.4 — Commit 2 acceptance criteria

- `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` amended per §3.2
- Cross-references updated per §3.3
- Pre-namespace boundary explicit (Q-M-1)
- Vanilla.Combat excluded note explicit
- M3.4/M3.5 deferred-but-named treated under Q-M-3 protocol
- `sync_register.ps1 --validate` exit code 0
- No cross-doc broken references

### §3.5 — Commit 2 message

```
A'.6/composite-ns: Q-V-2/Q-M-2/Q-M-3 MIGRATION_PLAN amendment

Ratification of Q-V-2, Q-M-2, Q-M-3 LOCKS per COMPOSITE_NAMESPACE_DELIBERATION_STATE.md:
- Vanilla mods → M-K bucket (Q-M-2)
- Multi-substrate mods compound marker M-K{N} / M-V (Q-V-2, FHE-pattern reserved)
- Specific renames deferred to per-mod authoring time
- Deferred-but-named M3.4/M3.5 treated as pending под Q-M-2 (Q-M-3)
- Combat excluded entirely
- Pre-namespace boundary M0..M7 explicit (Q-M-1 prep)
- M9 dual-meaning cleanup (Q-R-2)
- Cross-refs RUNTIME/GPU_COMPUTE → VULKAN_SUBSTRATE

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md (Project files).
```

---

## §4 — Commit 3: Q-G-2 GPU_COMPUTE roadmap section (in unified V doc)

### §4.1 — Scope

This commit refines the **roadmap section** of `VULKAN_SUBSTRATE.md` (created in Commit 1) to encode Q-G-2 demonstration content references precisely.

If Commit 1 already included roadmap section correctly, this commit can be folded или become refinement only. **Halt SC-4** if boundary unclear (might combine with Commit 1).

### §4.2 — Content amendments

In V substrate roadmap section:

**Substrate sequencing** (per Q-G-2 reductions):
- V0 — Vulkan compute pipeline plumbing
- V1 — Scalar diffusion shader (environmental layer)
- V2 — Scalar wave shader (routed layer)
- V substrate close criteria: multi-field coexistence demonstration works (was G4)
- V N+ future expansions open

**M-V demonstrations** (per Q-R-1 format):
- M-V1 — Vanilla.Magic mana field (configures V1 ± V2 для channeled mana)
- M-V2 — Vanilla.Electricity (configures V2 wave through cables)
- ~~M-V3~~ — gap (G3 storage упразднён as gameplay-level node config)
- ~~M-V4~~ — gap (G4 multi-field as substrate close criteria)
- M-V5 — Vanilla.Combat projectile Domain B (**deferred** — TBD whether stays in V substrate)
- ~~M-V6~~ — gap (G6 flow field folded into V2)
- M-V7 — Vanilla.Movement (3-mode dispatcher described, hybrid CPU+GPU)
- M-V8 — Vanilla.Movement local avoidance extension (mod-level concern)
- ~~M-V9~~ — gap (G9 eikonal as deferred V2 tunable parameter)

**Hybrid coupling** TBD note (for amendment authoring): how diffusion picks up from broken wave node (water in pipes + diffusion on break).

### §4.3 — Commit 3 acceptance criteria

- V substrate roadmap section in VULKAN_SUBSTRATE.md encodes Q-G-2 decisions
- M-V demonstrations cited per Q-R-1 format
- Gaps explicit (M-V3, M-V4, M-V6, M-V9)
- Deferred items noted (G5/M-V5 projectile, G9/V2 eikonal)
- Hybrid coupling spec TBD note for future
- `sync_register.ps1 --validate` exit code 0

### §4.4 — Commit 3 message

```
A'.6/composite-ns: Q-G-2 V substrate roadmap refinement

Refines VULKAN_SUBSTRATE.md roadmap section per Q-G-2 LOCK:
- V substrate primitives sequenced (V0/V1/V2)
- M-V demonstrations cited per Q-R-1 (M-V1, M-V2, M-V5 deferred, M-V7, M-V8)
- Gaps documented (M-V3/4/6/9 reflect упразднения)
- Hybrid coupling spec TBD for amendment authoring

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §3.2.
```

---

## §5 — Commit 4: Q-M-1 M0..M7 boundary documentation

### §5.1 — Scope

Per Q-M-1 LOCK, M0..M7 closed phases retain historical names. This commit adds **explicit boundary documentation** in artifacts that document the M-cycle, without modifying any closure documents themselves.

**Touched** (boundary notes added):
- `docs/ROADMAP.md` (DOC-C-ROADMAP) — pre-namespace boundary note in M-cycle sequence
- `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` (already amended in Commit 2 §3.2 added boundary note)

**Not touched** (Q-M-1 explicit preservation):
- `docs/audit/M3_CLOSURE_REVIEW.md` through `M7_CLOSURE_REVIEW.md`
- `docs/audit/AUDIT_PASS_*` files
- `docs/prompts/M7_CLOSURE.md` и historical prompt docs

Query REGISTER.yaml для full enumeration of closed-record docs containing M0..M7 refs — they are explicitly preserved.

### §5.2 — Content amendments

**ROADMAP.md** add boundary note:

> «**Pre-namespace M-cycle (M0..M7)** — closed phases preserved under pre-composite-namespace naming convention. Closure reviews `docs/audit/M*_CLOSURE_REVIEW.md` retain original names as shipped (Q-M-1 LOCK per `COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §3.7).
> 
> **Composite namespace (M8.x forward)** — pending and future vanilla content uses M-K bucket prefix (Q-M-2). Specific renames deferred to per-mod authoring per FHE-style reserved pattern.»

### §5.3 — Commit 4 acceptance criteria

- ROADMAP.md boundary note added
- No closure review document modified (Q-M-1 preservation invariant)
- Audit passes, historical prompts untouched
- `sync_register.ps1 --validate` exit code 0

### §5.4 — Commit 4 message

```
A'.6/composite-ns: Q-M-1 pre-namespace boundary documentation

Adds explicit boundary documentation in ROADMAP per Q-M-1 LOCK:
M0..M7 closed phases preserved as historical record;
composite namespace applies M8.x forward.

No closure review modified (preservation invariant).

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §3.7.
```

---

## §6 — Commit 5: Q-K-1 reconciliation per recommendations + verbatim validation

### §6.1 — Scope

Per Q-K-1 instruction-only protocol (deliberation document §5):
1. Read `KERNEL_ARCHITECTURE.md` Part 2 verbatim
2. Read `PHASE_A_PRIME_SEQUENCING.md` §2 verbatim
3. Apply principled reconciliation per recommendations
4. **Halt SC-2** if recommendation conflicts с verbatim content
5. Report actual findings и resolution applied для retroactive lock

### §6.2 — Verbatim read protocol

**Required reads**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 (status table including K8.5 entry)
- `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` §2 (sequencing diagrams и A' phase definitions)

**Extract**:
- K8.5 entry в KERNEL Part 2: what does it actually say?
- A'.6 = K8.5 statement в PHASE_A_PRIME §2: what context?
- A'.7 = K8.5 skeleton execution statement в PHASE_A_PRIME: what context?
- Cross-references: do other docs cite K8.5? A'.6? A'.7? Where?

### §6.3 — Recommendations (per deliberation document §5)

These are **recommendations**, not ratified principles. Validate against verbatim before applying:

- **Canonical identifier recommendation**: K8.5 is kernel-series milestone identifier (Part 2 origin)
- **A'.6 / A'.7 nature**: These are A'-cycle sequencing labels pointing TO K8.5, not alternate identities
- **Format suggestion**: «A'.7 phase: executes K8.5 brief»
- **Other docs cross-referencing**: align to K8.5 canonical

### §6.4 — Resolution application

If recommendations align с verbatim content (i.e., A'.6/A'.7 are demonstrably sequencing labels не identity aliases):

Apply consistent reconciliation:
- `KERNEL_ARCHITECTURE.md` Part 2: K8.5 entry stays canonical (already in this form per recommendation)
- `PHASE_A_PRIME_SEQUENCING.md` §2: clarify A'.6/A'.7 as «**sequencing pointers** к K8.5, not alternate identifiers»
- Any other doc references aligned to K8.5 canonical

If recommendations conflict с verbatim (i.e., docs actually treat A'.6 as separate identity, or other unexpected pattern):

**Halt SC-2**. Document conflict в HALT_REPORT с verbatim citations. Defer Q-K-1 reconciliation. Other Q-targets proceed normally; Commit 5 becomes Q-K-1 attempt только (this commit's other content moves to follow-up).

### §6.5 — Reporting (для retroactive lock)

Whether resolved or halted, report:
- Verbatim citations of K8.5 entries в both docs (lines numbers + actual text)
- Reconciliation applied (if applied) или conflict description (if halted)
- File modifications made
- Recommendation match level (full match / partial / conflict)

Report goes into commit message body **AND** into a structured execution report at `docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md`.

### §6.6 — Commit 5 acceptance criteria

- Verbatim reads completed
- Resolution applied OR halt SC-2 triggered с full conflict report
- Q-K-1 report authored at `docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md`
- If resolution: KERNEL Part 2 + PHASE_A_PRIME §2 + cross-refs amended for consistency
- `sync_register.ps1 --validate` exit code 0

### §6.7 — Commit 5 message

```
A'.6/composite-ns: Q-K-1 reconciliation per execution discovery

Per Q-K-1 retroactive lock protocol (COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §5):

Verbatim reads:
- KERNEL_ARCHITECTURE.md Part 2 K8.5 entry
- PHASE_A_PRIME_SEQUENCING.md §2 A'.6/A'.7 statements

Resolution applied: [describe actual resolution based on verbatim findings]

Report: docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md

Awaiting retroactive lock by subsequent deliberation session.
```

(If halted instead, commit message says «Halted SC-2, see HALT_REPORT» с verbatim conflict citations.)

---

## §7 — Commit 6: REGISTER amendments

### §7.1 — Scope

Update `docs/governance/REGISTER.yaml` to reflect restructured documents:

**Remove entries**:
- `DOC-A-RUNTIME` (deleted in Commit 1)
- `DOC-A-GPU_COMPUTE` (deleted в Commit 1)

**Add entries**:
- `DOC-A-VULKAN_SUBSTRATE` (created в Commit 1, lifecycle LOCKED v1.0 or Live)

**Amend entries** (cross-reference cleanup):
- Any DOC-A entries citing RUNTIME/GPU_COMPUTE in their description fields → cite VULKAN_SUBSTRATE
- Any DOC-D entries (briefs) с stale references → cite new doc

**G-series brief register entries** (10 skeleton briefs, DOC-D-G0..DOC-D-G9):
- Lifecycle remains AUTHORED (briefs still on disk, не deleted)
- Add note in REGISTER frontmatter «Architecture refactored per Q-G-2 LOCK — brief content superseded by VULKAN_SUBSTRATE.md V0/V1/V2 primitives. Briefs retained as historical record of pre-deliberation architectural intent.»

**M-cycle audit/closure register entries** (DOC-E-M3_CLOSURE_REVIEW..DOC-E-M7_CLOSURE_REVIEW, DOC-E-AUDIT_PASS_*, etc.):
- Unchanged (Q-M-1 preservation)

### §7.2 — Validation protocol

Run `sync_register.ps1 --validate` after each REGISTER edit. Exit code 0 required. If validation fails: halt SC-3.

If `--sync` needed для regenerating doc frontmatters, run it; commit includes regenerated frontmatter changes.

### §7.3 — Commit 6 acceptance criteria

- DOC-A-RUNTIME removed from REGISTER
- DOC-A-GPU_COMPUTE removed
- DOC-A-VULKAN_SUBSTRATE added
- Cross-references in other register entries updated
- G-series brief entries annotated with refactoring note
- M-series audit/closure entries unchanged
- `sync_register.ps1 --validate` exit code 0
- `sync_register.ps1 --sync` run if frontmatter regen needed

### §7.4 — Commit 6 message

```
A'.6/composite-ns: REGISTER amendments for restructured docs

Updates REGISTER.yaml per Q-G-1 + Q-G-2 + Q-M-1 + Q-V-2 + Q-M-2 + Q-M-3 cascade:
- Remove DOC-A-RUNTIME (consolidated into VULKAN_SUBSTRATE)
- Remove DOC-A-GPU_COMPUTE (consolidated)
- Add DOC-A-VULKAN_SUBSTRATE (unified V substrate doc)
- Cross-reference updates in dependent register entries
- G-series brief entries annotated с refactoring note (briefs preserved as historical)
- M-cycle audit/closure entries unchanged (Q-M-1 preservation)

sync_register.ps1 --validate exit code 0.

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md.
```

---

## §8 — Commit 7: Verification full pass

### §8.1 — Scope

Final verification commit. Runs全 full validation cycle:

1. `sync_register.ps1 --validate` full pass — exit code 0
2. `dotnet build` — green where build affected (mostly не affected, doc-only changes)
3. `dotnet test` — relevant test suites green
4. Cross-doc link validation (check that no broken cite references)

If any validation fails: halt SC-5. Roll back if needed before commit.

### §8.2 — Cross-doc link audit

Use grep или register-driven enumeration to find:
- Any remaining references to `RUNTIME_ARCHITECTURE.md` (should be none)
- Any remaining references to `GPU_COMPUTE.md` (should be none)
- Any references to M9.0..M9.8 runtime (should be migrated to V0..V2)
- Any pending vanilla mod references missing M-K bucket prefix where Q-M-2 expects it

Halt SC-1 if any unexpected reference remains.

### §8.3 — Commit 7 acceptance criteria

- `sync_register.ps1 --validate` exit code 0
- `dotnet build` green
- `dotnet test` green for affected suites
- No broken cite references
- All Q-targets applied per ratification document

### §8.4 — Commit 7 message

```
A'.6/composite-ns: Verification full pass

Full validation cycle after composite namespace ratification cascade:
- sync_register.ps1 --validate exit code 0
- dotnet build green
- dotnet test green for affected suites
- Cross-doc link audit: no broken references

Cascade complete. Q-K-1 retroactive lock pending follow-up deliberation.

Ratified by: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md.
```

---

## §9 — Q-K-1 retroactive lock protocol summary

For convenience, the Q-K-1 protocol consolidated в one place. Detailed scope in §6.

**Execution responsibility**: read verbatim, apply per recommendations, halt on conflict, report findings.

**Deliberation responsibility (post-execution)**: review Q-K-1 report, ratify actual resolution retroactively, update deliberation document.

**Format of Q-K-1 report** (`docs/scratch/RATIFICATION_EXECUTION/Q_K_1_REPORT.md`):

```markdown
# Q-K-1 Execution Report

**Date**: <timestamp>
**Authority**: COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §5

## Verbatim Read — KERNEL_ARCHITECTURE.md Part 2

K8.5 entry (line X-Y):
> [verbatim text]

## Verbatim Read — PHASE_A_PRIME_SEQUENCING.md §2

A'.6 = K8.5 statement (line A-B):
> [verbatim text]

A'.7 = K8.5 skeleton execution statement (line C-D):
> [verbatim text]

## Cross-References Found

[file:line list with surrounding context]

## Reconciliation Applied (or Conflict Reported)

[detailed description of resolution или conflict]

## Recommendation Match Level

[full / partial / conflict]
```

This report enables retroactive lock в follow-up deliberation session.

---

## §10 — Commit discipline summary

**Atomic commits per Q-target** (per K8.3+K8.4 v2.0 precedent):

1. Q-G-1 — RUNTIME + GPU_COMPUTE → VULKAN_SUBSTRATE (Commit 1)
2. Q-V-2/Q-M-2/Q-M-3 — MIGRATION_PLAN amendment (Commit 2)
3. Q-G-2 — V substrate roadmap refinement (Commit 3)
4. Q-M-1 — pre-namespace boundary documentation (Commit 4)
5. Q-K-1 — reconciliation per execution discovery (Commit 5)
6. REGISTER amendments (Commit 6)
7. Verification full pass (Commit 7)

**Per-commit invariants** (Lesson #8):
- Repository in valid intermediate state
- `sync_register.ps1 --validate` exit code 0
- Build green where build affected
- Tests green for affected suites
- No broken cite references

**Branch strategy**: feature branch first (e.g., `claude/composite-ns-ratification-<id>`). Push to main only after Crystalka explicit re-confirmation (per memory entry on Claude Code auto-mode push-block).

**Halt sequence**: if halt triggers fire, work commits go onto branch up to halt point, HALT_REPORT committed, push to main blocked, deliberation session reviews halt.

---

## §11 — Execution sign-off

Before executing, Claude Code confirms:

1. **Ratification authority understood** — `COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` in Project files is the single source of truth для 9 explicit Q-locks + Q-K-1 retroactive instructions.

2. **REGISTER.yaml is navigation source** — query register для file enumeration, не hardcode lists from brief examples.

3. **Atomic commits per Q-target** — 7 commits sequenced per §10. Each commit invariant per Lesson #8.

4. **Halt is the right answer** if any premise wrong. Three K8.3+K8.4 v2.0 halts (precedent) protected project from bad executions.

5. **Q-K-1 is retroactive** — execution discovers verbatim ground truth, deliberation ratifies post-execution.

6. **Vanilla.Combat excluded** from cascade entirely (deferred to post-substrate deliberation).

7. **M0..M7 closed phases preserved** — no closure review modified, no audit pass modified.

8. **Vanilla.X type names preserved** — Q-V-1 scope discipline strictly enforced.

9. **Push protocol**: feature branch first, push to main только после Crystalka re-confirmation.

**Provenance**:

```
2026-05-13  K8.3 v2.0 brief halts → patch v1 → K8.3+K8.4 v2.0 (b903b91→fc8ecb6)
2026-05-15  Architecture recon — ARCHITECTURE_RECON_REPORT.md
2026-05-15  Cascade map recon — M_G_R_NAMESPACE_CASCADE_MAP.md
2026-05-15  Composite namespace deliberation session — 9 Q locked, Q-K-1 instruction-only
2026-05-15  This brief authored (DOC-D-COMPOSITE_NAMESPACE_RATIFICATION_BRIEF)
2026-XX-XX  Execution session — atomic commits land on feature branch
2026-XX-XX  Retroactive Q-K-1 lock in subsequent deliberation
2026-XX-XX  Push to main (post-confirmation)
```

**Brief end. Execution begins at §2.1 (Commit 1: Q-G-1 RUNTIME + GPU_COMPUTE consolidation).**
