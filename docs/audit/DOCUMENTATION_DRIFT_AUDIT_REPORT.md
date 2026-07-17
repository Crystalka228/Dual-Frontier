---
register_id: DOC-E-DOCUMENTATION_DRIFT_AUDIT_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-16
last_modified: 2026-05-16
content_language: en
next_review_due: null
title: Documentation Drift Audit Report
review_cadence: on-status-transition
last_review_date: 2026-05-16
last_review_event: Audit-only documentation drift report authored 2026-05-16; no architecture/source documents modified
reviewer: Crystalka
---

# Documentation Drift Audit Report

## Status

Audit-only report. No architecture documents were modified by this pass.

## Cleanup status (added 2026-05-16, CLEANUP_CASCADE_BRIEF execution)

The cleanup cascade (CLEANUP_CASCADE_BRIEF.md, branch `claude/cleanup-cascade`) closed 18 of 19 findings in a 16-commit atomic cascade. Per-finding disposition:

| Finding | Severity | Status | Closure commit(s) |
|---|---|---|---|
| DRIFT-001 | S4 | ADDRESSED | Commit 13 (fa88f12) — MIGRATION_PROGRESS state sync |
| DRIFT-002 | S4 | ADDRESSED | Commit 3 (438de49) — README NativeWorld production rewrite |
| DRIFT-003 | S5 | ADDRESSED | Commit 3 (438de49) — README isolation safety rewrite |
| DRIFT-004 | S4 | ADDRESSED | Commit 7 (11bf1c1) — ECS NativeWorld production storage rewrite |
| DRIFT-005 | S5 | ADDRESSED | Commit 4 (f5e27f3) ISOLATION + Commit 5 (6cf1d77) THREADING + Commit 6 (613e5ef) PERFORMANCE + Commit 12 (f7fe134) MODDING |
| DRIFT-006 | S4 | ADDRESSED | Commit 8 (1a88ece) — CONTRACTS + EVENT_BUS IPowerBus removal |
| DRIFT-007 | S4 | ADDRESSED | Commit 9 (73c7bbf) — ROADMAP M10.B power deletion reconciliation |
| DRIFT-008 | S4 | ADDRESSED | Commit 9 (73c7bbf) — MIGRATION_PLAN internal conflict reconciled |
| DRIFT-009 | S4 | ADDRESSED | Commit 2 (253c7ab) VISUAL/GODOT historical move + Commit 10 (3aa3585) ARCHITECTURE Silk.NET supersession |
| DRIFT-010 | S4 | ADDRESSED | Commit 16 (PENDING) — REGISTER_RENDER regeneration via render_register.ps1 |
| DRIFT-011 | S4 | ADDRESSED | Commit 11 (c926382) — MOD_OS v3 strict (no v2 compat) |
| DRIFT-012 | S4 | ADDRESSED | Commit 12 (f7fe134) — MODDING + MOD_PIPELINE v3 + current §9.5 unload |
| DRIFT-013 | S3 | ADDRESSED | Commit 14 (08e4fde) — G→V namespace tail across FIELDS/KERNEL/MIGRATION_PLAN/MOD_OS |
| DRIFT-014 | S3 | ADDRESSED | Commit 1 (e68d799) — G-series briefs AUTHORED → SUPERSEDED + moved to historical/ |
| DRIFT-015 | S3 | ADDRESSED | Commit 14 (08e4fde) — ARCHITECTURE_RECON_REPORT pre-V-unification annotation |
| DRIFT-016 | S3 | **HALTED (SC-4)** | Q-K-1 reconciliation note in PHASE_A_PRIME_SEQUENCING.md (lines 55-63, added 2026-05-16 commit d303fb5) explicitly defers A'-cycle renumbering propagation to «subsequent deliberation when K8.5 brief authoring approaches». Cleanup brief §4 SC-4 halt trigger fired; surfaces to Crystalka for K8.5-brief-time decision. |
| DRIFT-017 | S3 | ADDRESSED | Commit 14 (08e4fde) — module-local READMEs (Core.Interop/MODULE + Components/Building + Contracts/Bus + Contracts/Modding) |
| DRIFT-018 | S2 | DEFERRED | Per audit own categorization (S2 minor wording); IDEAS_RESERVOIR game-as-product framing deferred to next docs pass. |
| DRIFT-019 | S1 | NO ACTION | Historical residue in closure/audit reports; audit explicitly recommended no-action when register lifecycle is obeyed. |

5 CAPAs opened+closed within same governance event (EVT-2026-05-16-CLEANUP-CASCADE in REGISTER.yaml audit_trail):
- CAPA-2026-05-16-ISOLATION-AUTHORITY-RESTORATION (DRIFT-003 + DRIFT-005)
- CAPA-2026-05-16-LIVE-STATE-CLOSURE-PROTOCOL-GAP (DRIFT-001 + DRIFT-010)
- CAPA-2026-05-16-POWER-DELETION-PROPAGATION (DRIFT-006 + DRIFT-007 + DRIFT-008)
- CAPA-2026-05-16-MOD-API-V3-AUTHORITY (DRIFT-011 + DRIFT-012)
- CAPA-2026-05-16-V-SUBSTRATE-SUPERSESSION (DRIFT-009 + DRIFT-013 + DRIFT-014 + DRIFT-015)

## Scope

Audited the document corpus through the governance register, with emphasis on:

- Category A / Tier 1 architecture documents under `docs/architecture/`.
- Category B / Tier 1 methodology documents where they define pipeline authority.
- Category C / Tier 2 live trackers: `docs/MIGRATION_PROGRESS.md`, `docs/ROADMAP.md`, `docs/IDEAS_RESERVOIR.md`.
- Category G / Tier 2 navigation documents: `README.md`, `docs/README.md`, `docs/governance/REGISTER_RENDER.md`.
- Category E / Tier 3 reports and audit artifacts when referenced as current evidence.
- Selected Category F / Tier 4 module README/MODULE files where keyword searches showed executor-facing current-state claims.

Primary paths inspected included `docs/governance/REGISTER.yaml`, `docs/governance/FRAMEWORK.md`, `README.md`, `docs/README.md`, `docs/MIGRATION_PROGRESS.md`, `docs/ROADMAP.md`, Tier 1 architecture documents, current reports, and light code anchors in `src/DualFrontier.Application/Loop/GameBootstrap.cs`, `src/DualFrontier.Contracts/Bus/IGameServices.cs`, `src/DualFrontier.Contracts/Modding/IModApi.cs`, `src/DualFrontier.Core/ECS/SystemBase.cs`, and `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`.

## Method

`REGISTER.yaml` was used as the metadata source of truth for document IDs, category, tier, lifecycle, and authority classification. `FRAMEWORK.md` was used to interpret Category x Tier x Lifecycle semantics: Tier 1 as architecture authority, Tier 2 as operational live state, Tier 3 as milestone/report artifacts, and Tier 4 as module-local just-in-time context.

The audit used two passes:

- Register-guided pass: read Tier 1 architecture and methodology entries, Tier 2 live documents, rendered register output, and current reports listed in the register.
- Keyword-guided pass: searched for runtime stack terms, power terms, NativeWorld/managed World terms, IModApi v2/v3 terms, isolation/analyzer terms, G/V namespace terms, and game-framing terms.

Code-state checks were deliberately light and used only to confirm current anchors, not to perform a code review.

## Current authority anchors

The comparison baseline used by this audit:

- `REGISTER.yaml` is authoritative for document metadata; frontmatter and `REGISTER_RENDER.md` are generated derivatives.
- NativeWorld is the sole production component-storage path after A'.5 / K8.3+K8.4. Managed `World` is retired from production and remains only as test/reference fixture where applicable.
- Production systems now use `NativeWorld`, `SpanLease<T>`, and `WriteBatch<T>`; `GameBootstrap.CreateLoop` constructs a single `NativeWorld` through `Bootstrap.Run`.
- The power CPU subsystem was deleted during A'.5: `ElectricGridSystem`, `ConverterSystem`, `IPowerBus`, `PowerProducerComponent`, `PowerConsumerComponent`, and related power events are not current production surfaces. Electricity-like mechanics move toward V substrate field/compute work.
- `VULKAN_SUBSTRATE.md` is the unified Vulkan substrate authority for rendering plus compute. It supersedes prior runtime/GPU-compute documents and reduces old G-series primitive framing to V0/V1/V2 plus M-V demonstrations.
- `IModApi` is v3 in code: `RegisterComponent<T>` is constrained to unmanaged Path alpha, `RegisterManagedComponent<T>` supports Path beta, `Fields` is present, `ComputePipelines` is reserved/nullable, and manifest version is strict v3.
- Runtime component-access isolation guard methods are removed in current code; safety now relies on `[SystemAccess]` declarations consumed by dependency graph construction, compile-time discipline, and future analyzer work. `SystemExecutionContext` still carries scheduler context, services, NativeWorld, mod provenance, and Path beta resolver state.
- README and ROADMAP correctly frame the game content as a workload/stress test for the methodology and engine rather than a conventional game-release-first project.

## Summary table

| ID | Severity | Document | Register ID | Drift surface | Recommended action |
|---|---|---|---|---|---|
| DRIFT-001 | S4 | `docs/MIGRATION_PROGRESS.md` | DOC-C-MIGRATION_PROGRESS | Live tracker still pre-A'.5 | Update live tracker before next executor brief |
| DRIFT-002 | S4 | `README.md` | DOC-G-README | Native core framed as experimental, not production | Update wording to NativeWorld production backbone |
| DRIFT-003 | S5 | `README.md` | DOC-G-README | Runtime isolation guard still advertised | Update safety model immediately |
| DRIFT-004 | S4 | `docs/architecture/ECS.md` | DOC-A-ECS | Managed `World` and old SystemBase API | Amend or supersede ECS doc |
| DRIFT-005 | S5 | `docs/architecture/ISOLATION.md`; `docs/architecture/THREADING.md`; `docs/architecture/PERFORMANCE.md` | DOC-A-ISOLATION; DOC-A-THREADING; DOC-A-PERFORMANCE | Runtime guard / crash semantics obsolete | Block isolation-related executor work until cleaned |
| DRIFT-006 | S4 | `docs/architecture/CONTRACTS.md`; `docs/architecture/EVENT_BUS.md` | DOC-A-CONTRACTS; DOC-A-EVENT_BUS | `IPowerBus` still described as current | Update or mark historical |
| DRIFT-007 | S4 | `docs/ROADMAP.md` | DOC-C-ROADMAP | Future M10.B still migrates deleted power systems | Update Phase B/M10 wording |
| DRIFT-008 | S4 | `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md` | DOC-A-MIGRATION_PLAN | Same document contains power-deleted note plus stale power migration slices | Reconcile internal plan sections |
| DRIFT-009 | S4 | `docs/architecture/ARCHITECTURE.md`; `VISUAL_ENGINE.md`; `GODOT_INTEGRATION.md` | DOC-A-ARCHITECTURE; DOC-A-VISUAL_ENGINE; DOC-A-GODOT_INTEGRATION | Silk.NET/OpenGL production runtime vs Vulkan substrate | Supersede/amend visual-runtime docs |
| DRIFT-010 | S4 | `docs/governance/REGISTER_RENDER.md` | DOC-G-REGISTER_RENDER | Rendered register stale vs `REGISTER.yaml` | Regenerate from register |
| DRIFT-011 | S4 | `docs/architecture/MOD_OS_ARCHITECTURE.md` | DOC-A-MOD_OS | IModApi v3 compatibility claims conflict with code | Amend v3/backward-compat sections |
| DRIFT-012 | S4 | `docs/architecture/MODDING.md`; `MOD_PIPELINE.md` | DOC-A-MODDING; DOC-A-MOD_PIPELINE | Mod author/pipeline docs show old API and unload flow | Rewrite against Mod API v3 |
| DRIFT-013 | S3 | `docs/architecture/FIELDS.md`; `KERNEL_ARCHITECTURE.md`; `MIGRATION_PLAN_KERNEL_TO_VANILLA.md`; `MOD_OS_ARCHITECTURE.md` | DOC-A-FIELDS; DOC-A-KERNEL; DOC-A-MIGRATION_PLAN; DOC-A-MOD_OS | G-series names persist after V unification | Namespace cleanup pass |
| DRIFT-014 | S3 | `docs/governance/REGISTER.yaml` G0-G9 brief entries | DOC-D-G0..DOC-D-G9 | `AUTHORED` lifecycle overstates execution readiness | Reclassify lifecycle or add explicit historical state |
| DRIFT-015 | S3 | `docs/reports/ARCHITECTURE_RECON_REPORT.md` | DOC-E-ARCHITECTURE_RECON_REPORT | Live report still treats GPU_COMPUTE/RUNTIME as primary | Annotate as pre-V-unification evidence |
| DRIFT-016 | S3 | `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` | DOC-A-PHASE_A_PRIME_SEQUENCING | Self-admitted partial renumbering | Clean subordinate live sequencer |
| DRIFT-017 | S3 | module-local README/MODULE files | DOC-F-SRC-CORE-INTEROP; DOC-F-SRC-COMPONENTS-BUILDING; DOC-F-SRC-CONTRACTS-BUS; DOC-F-SRC-CONTRACTS-MODDING | Tier 4 module notes stale after A'.5 | Update in module-doc sweep |
| DRIFT-018 | S2 | `docs/IDEAS_RESERVOIR.md` | DOC-C-IDEAS_RESERVOIR | Game shipped/post-release wording | Minor wording cleanup |
| DRIFT-019 | S1 | Historical audit/closure reports | Various Category E | Old terms in retrospective context | No required action |

## Findings

### DRIFT-001 - `MIGRATION_PROGRESS.md` still reports the pre-A'.5 live state

- Severity: S4 - Authority conflict.
- Document: `docs/MIGRATION_PROGRESS.md`
- Register ID: DOC-C-MIGRATION_PROGRESS
- Category / Tier / Lifecycle: C / 2 / Live
- Drift surface: NativeWorld production state; K8.3/K8.4 status.
- Evidence:
  - `docs/MIGRATION_PROGRESS.md:47` says last completed milestone is A'.4.5 and next milestone is A'.5 K8.3.
  - `docs/MIGRATION_PROGRESS.md:99-101` still lists K8.3, K8.4, and K8.5 as `NOT STARTED`.
  - Current authority: `docs/architecture/KERNEL_ARCHITECTURE.md:17` and `:38` record A'.5 closed, managed `World` retired, power deleted, and 10 systems migrated to NativeWorld.
  - Code anchor: `src/DualFrontier.Application/Loop/GameBootstrap.cs:34-39` states NativeWorld is the sole production backend and power systems are deleted.
- Why it matters: This is a primary Tier 2 live tracker and a primary navigation input. An executor starting here would plan against a milestone that has already closed.
- Recommended action: Update the current snapshot and K-series table before using `MIGRATION_PROGRESS.md` as executor input. Create CAPA if the closure protocol failed to update this tracker at A'.5.

### DRIFT-002 - Root README still calls the native core experimental

- Severity: S4 - Authority conflict.
- Document: `README.md`
- Register ID: DOC-G-README
- Category / Tier / Lifecycle: G / 2 / Live
- Drift surface: NativeWorld / managed World production state.
- Evidence:
  - `README.md:74-78` says the native core is a replaceable experimental boundary rather than load-bearing.
  - Current authority: `docs/architecture/KERNEL_ARCHITECTURE.md:17` says NativeWorld is the sole production storage path after A'.5.
  - Code anchor: `GameBootstrap.CreateLoop` constructs and wires `NativeWorld` as production storage at `src/DualFrontier.Application/Loop/GameBootstrap.cs:57-64`.
- Why it matters: The README is a primary orientation document. This wording can cause an executor to treat NativeWorld as optional research instead of production architecture.
- Recommended action: Update README wording to distinguish the old experimental C++ branch from the current production NativeWorld backbone.

### DRIFT-003 - Root README advertises a runtime isolation guard that no longer exists

- Severity: S5 - Safety-critical drift.
- Document: `README.md`
- Register ID: DOC-G-README
- Category / Tier / Lifecycle: G / 2 / Live
- Drift surface: Isolation enforcement.
- Evidence:
  - `README.md:68-70` says a runtime isolation guard crashes immediately on any access violation.
  - Current code anchor: `src/DualFrontier.Core/ECS/SystemExecutionContext.cs:20-27` says the old runtime guard methods were deleted and enforcement is compile-time plus future analyzer.
  - Current architecture anchor: `docs/architecture/KERNEL_ARCHITECTURE.md:17` says isolation is enforced at compile time and runtime guard removed.
- Why it matters: Safety-layer wording can drive an executor either to rely on a missing runtime check or to reintroduce/bypass the wrong safety mechanism.
- Recommended action: Update README safety statement before any isolation, scheduler, or mod-safety executor brief uses it as context.

### DRIFT-004 - ECS architecture still describes managed `World` as the core storage surface

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/ECS.md`
- Register ID: DOC-A-ECS
- Category / Tier / Lifecycle: A / 1 / LOCKED
- Drift surface: Managed `World` vs NativeWorld.
- Evidence:
  - `docs/architecture/ECS.md:21` says `World` is the registry for all entities and is created in `GameLoop`.
  - `docs/architecture/ECS.md:111-117` documents `SystemBase.GetComponent`, `SetComponent`, `Query`, and `GetSystem`.
  - Current code anchor: `src/DualFrontier.Core/ECS/SystemBase.cs:9-14` says the managed-World access surface was removed; systems use NativeWorld span/batch APIs.
  - Current code anchor: `SystemBase.NativeWorld` is the production access surface at `src/DualFrontier.Core/ECS/SystemBase.cs:73-90`.
- Why it matters: A Tier 1 ECS doc still teaches the old storage and system-access API. This could cause wrong implementation choices in any ECS or system migration brief.
- Recommended action: Amend ECS.md to describe NativeWorld production storage and explicitly preserve old managed World material as historical or test-fixture context.

### DRIFT-005 - Isolation and threading docs still define the removed runtime guard

- Severity: S5 - Safety-critical drift.
- Document: `docs/architecture/ISOLATION.md`; `docs/architecture/THREADING.md`; `docs/architecture/PERFORMANCE.md`
- Register ID: DOC-A-ISOLATION; DOC-A-THREADING; DOC-A-PERFORMANCE
- Category / Tier / Lifecycle: A / 1 / LOCKED for all three
- Drift surface: Runtime isolation guard, crash semantics, DEBUG/RELEASE split.
- Evidence:
  - `docs/architecture/ISOLATION.md:15` says every component access passes through `SystemExecutionContext` and throws immediately on undeclared access.
  - `docs/architecture/ISOLATION.md:106-122` defines DEBUG vs RELEASE runtime guard modes.
  - `docs/architecture/THREADING.md:96` says the isolation guard access check is O(1) via `HashSet`.
  - `docs/architecture/THREADING.md:128` says DEBUG catches async/await through stack analysis.
  - `docs/architecture/PERFORMANCE.md:81-83` describes `SystemExecutionContext.GetComponent` DEBUG check overhead.
  - Current code anchor: `src/DualFrontier.Core/ECS/SystemExecutionContext.cs:20-27` says runtime guard methods that threw `IsolationViolationException` were deleted.
- Why it matters: This is safety-critical architecture. The docs now describe an enforcement layer that is not in code and contradict the new compile-time/analyzer direction.
- Recommended action: Block isolation/scheduler/mod-safety executor work until this cluster is reconciled. Recommended CAPA: "Isolation authority restoration".

### DRIFT-006 - Contract and event-bus specs still expose `IPowerBus`

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/CONTRACTS.md`; `docs/architecture/EVENT_BUS.md`
- Register ID: DOC-A-CONTRACTS; DOC-A-EVENT_BUS
- Category / Tier / Lifecycle: A / 1 / LOCKED for both
- Drift surface: PowerBus / ElectricGrid / Converter.
- Evidence:
  - `docs/architecture/CONTRACTS.md:38-70` documents six domain buses and `IPowerBus Power`.
  - `docs/architecture/EVENT_BUS.md:24-32` repeats the `IPowerBus Power` property.
  - Current code anchor: `src/DualFrontier.Contracts/Bus/IGameServices.cs:13-52` has Combat, Inventory, Magic, Pawns, and World only.
  - Current authority: `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:196` says IPowerBus and the power components/events were removed.
- Why it matters: These Tier 1 contracts can cause an executor to reintroduce a deleted bus or route electricity work through the old CPU subsystem.
- Recommended action: Update Tier 1 contract/event-bus specs or add a clearly historical subsection for v0.3 Phase 4 power.

### DRIFT-007 - ROADMAP future work still migrates deleted power systems

- Severity: S4 - Authority conflict.
- Document: `docs/ROADMAP.md`
- Register ID: DOC-C-ROADMAP
- Category / Tier / Lifecycle: C / 2 / Live
- Drift surface: PowerBus / ElectricGrid / Converter; Phase B sequencing.
- Evidence:
  - `docs/ROADMAP.md:523` says M10.B migrates `ElectricGridSystem` and `ConverterSystem`, and that the kernel keeps bus contracts and deferred event types.
  - Current authority: `docs/architecture/KERNEL_ARCHITECTURE.md:17` says the power subsystem was deleted and electricity deferred to GPU compute.
  - Current authority: `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:196` says the systems, components, events, and IPowerBus were deleted.
- Why it matters: ROADMAP is Tier 2 live state. This could send a vanilla-mod executor to migrate systems that no longer exist.
- Recommended action: Update M10/M10.B roadmap text to route electricity toward V substrate field/compute work and remove the old kernel-keeps-power-bus claim.

### DRIFT-008 - Migration plan contains both correct A'.5 closure note and stale power migration slices

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`
- Register ID: DOC-A-MIGRATION_PLAN
- Category / Tier / Lifecycle: A / 1 / LOCKED
- Drift surface: PowerBus / ElectricGrid / Converter; Phase B migration.
- Evidence:
  - `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:196` correctly says the power subsystem was deleted.
  - `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:211` still lists a `Power` system slice containing Converter/ElectricGrid/EtherGrid.
  - `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:461` still maps `Systems/Power` and power components to Vanilla.Inventory.
  - `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:698` still says Phase B includes `M10 (Vanilla.Inventory + Power)`.
- Why it matters: This is a Tier 1 migration plan with an internal conflict. The closure note is correct, but later sections remain executor-facing.
- Recommended action: Reconcile the plan so preserved historical K8.3 authoring context is clearly fenced and active Phase B sequencing no longer includes the deleted power slice.

### DRIFT-009 - Visual-runtime docs still name Silk.NET/OpenGL as production runtime

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/ARCHITECTURE.md`; `docs/architecture/VISUAL_ENGINE.md`; `docs/architecture/GODOT_INTEGRATION.md`
- Register ID: DOC-A-ARCHITECTURE; DOC-A-VISUAL_ENGINE; DOC-A-GODOT_INTEGRATION
- Category / Tier / Lifecycle: A / 1 / LOCKED for all three
- Drift surface: Runtime / presentation stack.
- Evidence:
  - `docs/architecture/ARCHITECTURE.md:65` calls `DualFrontier.Presentation.Native` the Silk.NET production runtime.
  - `docs/architecture/ARCHITECTURE.md:90` says `Presentation.Native` depends on Silk.NET.
  - `docs/architecture/VISUAL_ENGINE.md:15-17` says the shipped runtime is Silk.NET + OpenGL.
  - `docs/architecture/VISUAL_ENGINE.md:129-136` lists Silk.NET.Windowing/Input/OpenGL.
  - `docs/architecture/GODOT_INTEGRATION.md:19-22` repeats Godot-as-DevKit with Silk.NET + OpenGL final game.
  - Current authority: `docs/architecture/VULKAN_SUBSTRATE.md:15-17` says VULKAN_SUBSTRATE supersedes prior runtime/GPU docs; `:31` says it is the single authority for Vulkan substrate work.
- Why it matters: These are Tier 1 docs. They point executors toward Silk.NET/OpenGL rather than the locked Vulkan substrate direction.
- Recommended action: Supersede or amend the visual-runtime doc family. At minimum add prominent "pre-V substrate; superseded for production direction" notices.

### DRIFT-010 - Rendered register is stale relative to `REGISTER.yaml`

- Severity: S4 - Authority conflict.
- Document: `docs/governance/REGISTER_RENDER.md`
- Register ID: DOC-G-REGISTER_RENDER
- Category / Tier / Lifecycle: G / 2 / Live
- Drift surface: Governance navigation.
- Evidence:
  - `docs/governance/REGISTER_RENDER.md:165-170` lists `DOC-A-GPU_COMPUTE` as Tier 1 LOCKED.
  - `docs/governance/REGISTER_RENDER.md:282-287` lists `DOC-A-RUNTIME` as Tier 1 LOCKED.
  - `docs/governance/REGISTER.yaml:74-89` instead registers `DOC-A-VULKAN_SUBSTRATE` and says it supersedes prior RUNTIME/GPU_COMPUTE docs.
  - Filesystem check: `docs/architecture/GPU_COMPUTE.md` and `docs/architecture/RUNTIME_ARCHITECTURE.md` are not present.
- Why it matters: The rendered register is a Tier 2 navigation surface. It contradicts the register SoT and points to non-existent Tier 1 authorities.
- Recommended action: Regenerate `REGISTER_RENDER.md` from `REGISTER.yaml` as part of governance sync. If this recurs, create CAPA for generated-derivative staleness.

### DRIFT-011 - MOD_OS_ARCHITECTURE conflicts on IModApi v3 compatibility

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/MOD_OS_ARCHITECTURE.md`
- Register ID: DOC-A-MOD_OS
- Category / Tier / Lifecycle: A / 1 / LOCKED
- Drift surface: Mod OS API surface.
- Evidence:
  - `docs/architecture/MOD_OS_ARCHITECTURE.md:49` says v3 is additive and v2 mods continue unchanged.
  - `docs/architecture/MOD_OS_ARCHITECTURE.md:563` repeats that v2 mods continue to compile and load against v3.
  - Current code anchor: `src/DualFrontier.Contracts/Modding/IModApi.cs:16-27` says IModApi v3 deleted v2 entirely, with no backward compatibility and strict manifest version `"3"`.
  - `docs/architecture/MOD_OS_ARCHITECTURE.md:56` partially reflects the newer `RegisterManagedComponent<T>` surface, creating mixed old/new authority in one doc.
- Why it matters: A mod-pipeline executor could implement backward compatibility that the code and current cutover explicitly removed.
- Recommended action: Amend MOD_OS §4.5/§4.6 and migration rows to reflect strict v3-only parser and the final Path alpha/beta constraints.

### DRIFT-012 - Modding and mod-pipeline docs still teach old API and unload flow

- Severity: S4 - Authority conflict.
- Document: `docs/architecture/MODDING.md`; `docs/architecture/MOD_PIPELINE.md`
- Register ID: DOC-A-MODDING; DOC-A-MOD_PIPELINE
- Category / Tier / Lifecycle: A / 1 / LOCKED for both
- Drift surface: Mod OS API surface; hot reload/unload semantics.
- Evidence:
  - `docs/architecture/MODDING.md:38-58` shows `RegisterComponent<T>() where T : IComponent`, `Unsubscribe<T>`, and split `LogWarning`/`LogError` methods not present in current `IModApi`.
  - `docs/architecture/MODDING.md:76-78` says obtaining a system reference crashes via isolation guard.
  - `docs/architecture/MOD_PIPELINE.md:116-134` shows old `RestrictedModApi` with `RegisterComponent<T>() where T : IComponent`.
  - `docs/architecture/MOD_PIPELINE.md:187-200` shows an outdated unload chain that does not match the current best-effort §9.5 chain.
  - Current code anchor: `src/DualFrontier.Contracts/Modding/IModApi.cs:16-123` defines v3 constraints, `Fields`, and `ComputePipelines`.
  - Current code anchor: `src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:57-60` documents M7.2/M7.3 unload discipline.
- Why it matters: These are Tier 1 executor-facing API docs. They can produce wrong example mods, wrong loader behavior, and wrong safety assumptions.
- Recommended action: Rewrite MODDING and MOD_PIPELINE against Mod API v3, strict manifest v3, shared/regular/vanilla mod kind rules, and current unload semantics.

### DRIFT-013 - G-series names remain in current Tier 1/2 docs after V unification

- Severity: S3 - Executor-facing ambiguity.
- Document: `docs/architecture/FIELDS.md`; `docs/architecture/KERNEL_ARCHITECTURE.md`; `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md`; `docs/architecture/MOD_OS_ARCHITECTURE.md`; `docs/architecture/PERFORMANCE.md`
- Register ID: DOC-A-FIELDS; DOC-A-KERNEL; DOC-A-MIGRATION_PLAN; DOC-A-MOD_OS; DOC-A-PERFORMANCE
- Category / Tier / Lifecycle: A / Tier 1 / mixed LOCKED or Live
- Drift surface: G-series / V-series namespace.
- Evidence:
  - `docs/architecture/FIELDS.md:45` says `VULKAN_SUBSTRATE` G0-G9 sits on top of field storage.
  - `docs/architecture/KERNEL_ARCHITECTURE.md:609` says K9 is prerequisite for G-series GPU compute.
  - `docs/architecture/KERNEL_ARCHITECTURE.md:775` references `VULKAN_SUBSTRATE.md v2.0` and G-series roadmap.
  - `docs/architecture/MIGRATION_PLAN_KERNEL_TO_VANILLA.md:700` still says Phase C is G-series GPU compute.
  - `docs/architecture/MOD_OS_ARCHITECTURE.md:446` says v3 gates G0-G9 Vulkan compute integration.
  - Current authority: `docs/architecture/VULKAN_SUBSTRATE.md:1281-1283` defines V0/V1/V2 as the substrate primitive roadmap.
- Why it matters: Executors could sequence G0/G1 work instead of V0/V1/V2 substrate work, or cite a non-existent `GPU_COMPUTE.md` authority.
- Recommended action: Do a namespace cleanup pass across current Tier 1/2 docs. Use "former G..." only when explicitly historical.

### DRIFT-014 - G0-G9 brief lifecycle overstates execution readiness

- Severity: S3 - Executor-facing ambiguity.
- Document: `docs/governance/REGISTER.yaml` entries DOC-D-G0 through DOC-D-G9
- Register ID: DOC-D-G0..DOC-D-G9
- Category / Tier / Lifecycle: D / 3 / AUTHORED
- Drift surface: Milestone namespace and brief lifecycle.
- Evidence:
  - `docs/governance/REGISTER.yaml:1340-1489` registers G0-G9 with lifecycle `AUTHORED` while special-case rationales say the content was consolidated, reframed, reduced, or deferred under V substrate.
  - `docs/reports/ARCHITECTURE_RECON_REPORT.md:188` independently notes that `AUTHORED` overstates their execution readiness.
  - Current authority: `docs/architecture/VULKAN_SUBSTRATE.md:17` and `:1539` say VULKAN_SUBSTRATE supersedes prior runtime/GPU compute authority.
- Why it matters: In this framework, `AUTHORED` means a brief awaits execution. These entries are more accurately historical/superseded intent.
- Recommended action: Reclassify G-series briefs or add explicit lifecycle/state wording that prevents executor sessions from treating them as executable.

### DRIFT-015 - Live Architecture Recon report still points to pre-V authorities

- Severity: S3 - Executor-facing ambiguity.
- Document: `docs/reports/ARCHITECTURE_RECON_REPORT.md`
- Register ID: DOC-E-ARCHITECTURE_RECON_REPORT
- Category / Tier / Lifecycle: E / 3 / Live
- Drift surface: Runtime/Vulkan/G-series authority.
- Evidence:
  - `docs/reports/ARCHITECTURE_RECON_REPORT.md:196-198` lists `RUNTIME_ARCHITECTURE.md` and `GPU_COMPUTE.md` as LOCKED authority.
  - `docs/reports/ARCHITECTURE_RECON_REPORT.md:366-368` calls `GPU_COMPUTE.md v2.0` the single most consequential document for navigation.
  - Current authority: `docs/architecture/VULKAN_SUBSTRATE.md:15-17` supersedes both source documents.
- Why it matters: The report is registered as Live, so agents may treat it as current architecture reconnaissance even though its runtime/compute authority basis has been superseded.
- Recommended action: Annotate the report as pre-Q-G-1/V-unification evidence or transition its lifecycle if it should no longer be live authority.

### DRIFT-016 - Phase A' sequencing document has partial renumbering drift

- Severity: S3 - Executor-facing ambiguity.
- Document: `docs/architecture/PHASE_A_PRIME_SEQUENCING.md`
- Register ID: DOC-A-PHASE_A_PRIME_SEQUENCING
- Category / Tier / Lifecycle: A / 2 / Live
- Drift surface: K8.3/K8.4/K8.5 sequencing; analyzer milestone.
- Evidence:
  - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:59` explicitly says body sections and estimates retain pre-renumbering structure and duplicate K8.5 pointers.
  - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:121-124` correctly records A'.5 K8.3+K8.4 closure and A'.6/A'.7 renumbering.
  - `docs/architecture/PHASE_A_PRIME_SEQUENCING.md:269-270` still has checklist items for all skeleton briefs and managed World retirement as future work.
- Why it matters: The document is subordinate to the migration plan but is Tier 2 Live; executor sessions could follow the stale body instead of the correct closure note.
- Recommended action: Clean the live sequencer after the authoritative migration-plan update, or mark it superseded if the migration plan now absorbs it.

### DRIFT-017 - Module-local README/MODULE files lag A'.5 cutover

- Severity: S3 - Executor-facing ambiguity.
- Document: `src/DualFrontier.Core.Interop/MODULE.md`; `src/DualFrontier.Components/Building/README.md`; `src/DualFrontier.Contracts/Bus/README.md`; `src/DualFrontier.Contracts/Modding/README.md`
- Register ID: DOC-F-SRC-CORE-INTEROP; DOC-F-SRC-COMPONENTS-BUILDING; DOC-F-SRC-CONTRACTS-BUS; DOC-F-SRC-CONTRACTS-MODDING
- Category / Tier / Lifecycle: F / 4 / Live
- Drift surface: NativeWorld status; PowerBus deletion; Mod API v3.
- Evidence:
  - `src/DualFrontier.Core.Interop/MODULE.md:40-45` says K8.3, K8.4, K8.5, and K9 are pending.
  - `src/DualFrontier.Components/Building/README.md:11-20` lists deleted power components and overload events.
  - `src/DualFrontier.Contracts/Bus/README.md:19` lists `IPowerBus.cs`.
  - `src/DualFrontier.Contracts/Modding/README.md:14-24` and `:43` omit v3 fields/compute/managed registration and still say RestrictedModApi remains to be implemented.
- Why it matters: Tier 4 docs are just-in-time context for code executors. These notes could cause local work against deleted files or old APIs.
- Recommended action: Schedule a module-doc sweep after Tier 1/Tier 2 authority restoration.

### DRIFT-018 - Ideas reservoir still uses game-release framing

- Severity: S2 - Minor stale wording.
- Document: `docs/IDEAS_RESERVOIR.md`
- Register ID: DOC-C-IDEAS_RESERVOIR
- Category / Tier / Lifecycle: C / 2 / Live
- Drift surface: Game-as-product vs game-as-load-stand framing.
- Evidence:
  - `docs/IDEAS_RESERVOIR.md:23` says "After Phase 7 closure - game shipped..." and frames entries as post-release updates.
  - Current README authority: `README.md:3-16` frames Dual Frontier as a falsifiable methodology/engine workload, not a conventional game release.
  - Current ROADMAP anchor: `docs/ROADMAP.md:17` says game content is a test case for the hypothesis.
- Why it matters: Low architecture risk, but it can pull planning language toward conventional shipping pressure.
- Recommended action: During a nearby docs pass, reword "game shipped/post-release" to "after load-stand baseline" or similar methodology-aligned wording.

### DRIFT-019 - Historical reports retain old terms in retrospective context

- Severity: S1 - Historical residue.
- Document: Historical audit, prompt, and closure reports under `docs/audit/`, `docs/prompts/`, and `docs/reports/`
- Register ID: Various Category E entries
- Category / Tier / Lifecycle: E / 3 / mostly EXECUTED or historical Live reports
- Drift surface: Old PowerBus, G-series, managed World, Godot/Silk.NET wording.
- Evidence:
  - Power and `IPowerBus` terms appear in `docs/audit/SESSION_PHASE_4_CLOSURE_REVIEW.md`, `docs/audit/AUDIT_PASS_*`, and earlier closure reports.
  - These documents are closure/audit artifacts with retrospective context, not current architecture authority.
- Why it matters: Low risk when the register/lifecycle is obeyed. Risk rises only if agents ignore lifecycle and cite old reports as current authority.
- Recommended action: No required action. Optional annotation only if a historical report is repeatedly misread as current authority.

## Cross-document clusters

### NativeWorld / managed World

Findings: DRIFT-001, DRIFT-002, DRIFT-004, DRIFT-017.

The current production code and newer Tier 1 kernel docs agree on NativeWorld as the production storage backend. The drift is concentrated in the Tier 2 migration tracker, the root README, the Tier 1 ECS doc, and Tier 4 module notes.

### Runtime / Vulkan / Godot / Silk.NET

Findings: DRIFT-009, DRIFT-010, DRIFT-013, DRIFT-015.

`VULKAN_SUBSTRATE.md` is clear and current, but older Tier 1 visual/runtime docs and a stale rendered register still preserve Silk.NET/OpenGL and prior runtime/GPU-compute authority.

### PowerBus / ElectricGrid / fields

Findings: DRIFT-006, DRIFT-007, DRIFT-008, DRIFT-017, DRIFT-019.

The current code and A'.5 notes delete the CPU power subsystem. The stale wording is still executor-facing in Tier 1 contracts/event-bus specs, Tier 2 roadmap, migration plan Phase B sections, and module-local READMEs.

### Mod OS API surface

Findings: DRIFT-011, DRIFT-012, DRIFT-017.

Current code is strict v3. MOD_OS partially contains v3 updates but retains additive/v2 compatibility text. MODDING and MOD_PIPELINE are much older and need a full v3 rewrite.

### Isolation / analyzer / runtime guard

Findings: DRIFT-003, DRIFT-005, DRIFT-012.

This is the highest-risk cluster. The old runtime guard appears in user-facing and Tier 1 architecture docs, but current code removed guard methods and points toward compile-time discipline plus future Roslyn analyzer.

### Milestone namespace drift

Findings: DRIFT-010, DRIFT-013, DRIFT-014, DRIFT-015, DRIFT-016.

V unification is well specified in `VULKAN_SUBSTRATE.md`, `ROADMAP.md`, and `docs/README.md`, but G-series and prior runtime names remain in several current surfaces.

### Game-as-load-stand framing

Findings: DRIFT-018.

Primary framing is mostly healthy: README and ROADMAP align on methodology/engine as the main research result and game content as workload. The remaining drift is low-severity wording in the ideas reservoir.

### Other

The generated register derivative (`REGISTER_RENDER.md`) is a governance-process issue rather than an architecture concept issue, but it is high impact because agents use it for navigation.

## Cleanup priority

### Immediate cleanup candidates

- DRIFT-003 and DRIFT-005: isolation/runtime guard safety model. Block isolation, scheduler, or mod-safety executor work until this is corrected.
- DRIFT-001 and DRIFT-010: primary navigation surfaces (`MIGRATION_PROGRESS.md`, `REGISTER_RENDER.md`). These should be fixed before any future broad executor session.
- DRIFT-006, DRIFT-007, DRIFT-008: deleted power subsystem still appears in Tier 1/Tier 2 execution surfaces.
- DRIFT-004: ECS.md still teaches managed `World` and old SystemBase methods.
- DRIFT-009: visual/runtime architecture family still points to Silk.NET/OpenGL production.
- DRIFT-011 and DRIFT-012: Mod API v3 conflict and stale mod authoring docs.

### Next documentation pass

- DRIFT-013: G-to-V namespace cleanup across Tier 1 docs.
- DRIFT-014: G0-G9 lifecycle/classification update.
- DRIFT-015: annotate or reclassify Architecture Recon as pre-V-unification.
- DRIFT-016: clean or supersede Phase A' sequencing.
- DRIFT-017: module-local README/MODULE sweep.
- DRIFT-018: minor game-framing wording.

### Optional historical annotations

- DRIFT-019: no required action; optional annotations only for historical reports that are repeatedly cited incorrectly.

## CAPA candidates

- CAPA candidate: Isolation authority restoration. Covers DRIFT-003 and DRIFT-005.
- CAPA candidate: Live-state closure protocol gap. Covers DRIFT-001 and possibly DRIFT-010 if generated derivatives were not refreshed after register changes.
- CAPA candidate: Power subsystem deletion propagation. Covers DRIFT-006, DRIFT-007, DRIFT-008, and power portions of DRIFT-017.
- CAPA candidate: Mod API v3 authority restoration. Covers DRIFT-011 and DRIFT-012.
- CAPA candidate: V-substrate supersession propagation. Covers DRIFT-009, DRIFT-013, DRIFT-014, and DRIFT-015.

## Non-findings

- `docs/architecture/VULKAN_SUBSTRATE.md` is internally consistent with V0/V1/V2, V substrate unification, and Godot deletion at R.8.
- `docs/README.md` correctly identifies `VULKAN_SUBSTRATE.md` as superseding prior runtime and GPU-compute docs.
- `docs/ROADMAP.md:49` correctly summarizes V substrate unification and G-to-V reductions despite later power drift.
- `README.md` and `docs/ROADMAP.md:17` correctly frame the project as methodology/engine research with game content as workload.
- Current code confirms NativeWorld production state: `GameBootstrap.CreateLoop` constructs `NativeWorld`; `SystemBase` exposes NativeWorld; production systems in `src/DualFrontier.Systems/Pawn` use `AcquireSpan` / `BeginBatch`.
- Current code confirms IPowerBus deletion: `IGameServices` has no Power property and source search finds no `ElectricGridSystem` or `ConverterSystem` implementation.
- Current code confirms IModApi v3 surface: `RegisterManagedComponent`, `Fields`, `ComputePipelines`, and strict v3 comments are present in `IModApi.cs`.
- MOD_OS correctly states that the mod system is not a security sandbox at `docs/architecture/MOD_OS_ARCHITECTURE.md:970`.
- Old power, managed World, and G-series mentions in executed closure reports are generally S1 historical residue when the register lifecycle is obeyed.

## Open questions for Crystalka

- Should `VISUAL_ENGINE.md` and `GODOT_INTEGRATION.md` be amended in place, marked SUPERSEDED, or retained as historical Tier 1 specs with explicit VULKAN_SUBSTRATE supersession notices?
- Should the old runtime isolation guard design be preserved as a historical section, or removed from current Tier 1 authority entirely?
- Should G0-G9 brief entries transition from `AUTHORED` to `SUPERSEDED`/`DEPRECATED`, or does the governance framework need a separate "historical authored skeleton" convention?
- Should power-system history remain in `CONTRACTS.md` / `EVENT_BUS.md` as versioned history, or move wholly to historical reports while current contracts show only live buses?