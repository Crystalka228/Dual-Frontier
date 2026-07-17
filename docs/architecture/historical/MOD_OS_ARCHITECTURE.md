---
register_id: DOC-A-MOD_OS
project: Dual Frontier
category: A
tier: 1
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.12.0
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: null
title: Mod OS Architecture — Dual Frontier (historical; superseded by authored rework)
superseded_by: DOC-A-MOD_OS_ARCHITECTURE
last_modified_commit: 6888246
review_cadence: on-change+annual
last_review_date: 2026-05-20
last_review_event: 'К10.3 v2 load-bearing commit 3/3 2026-05-20 — К-L18 amendments landed: §9.5 ALC unload chain extended 8-step → 9-step с Step 3.6 V resource cleanup placeholder (К10.3 v2 Item 42; managed wrapper VResourceCleanup returns vacuous success, native df_vulkan_unload_mod_resources lands V-cycle / К-extensions); §9.5.1 failure semantics extended с Step 3.6 inclusion + К-L18 quiescent state precondition rejection note; §9.7 «Hot reload К-L18 compliance» subsection added (SimulationStateController.PauseAsync + WaitForQuiescenceAsync + ResumeAsync pattern; mod management UI и hot reload tooling share enforcement). §11.2 ValidationErrorKind enum extended с 4 К10.3 v2 entries: QuiescentStatePreconditionViolated (К-L18; mod operation attempted while sim не paused либо pipeline in-flight); PipelineQuiescenceTimeout (К-L18; quiescence wait timeout); LayerCapabilityMismatch (К-L17; layer attribute/Type mismatch); VulkanModResourceCleanupFailed (К-L18 V scope; Step 3.6 placeholder; full implementation V-cycle). Predecessor commits 1/3 (К-L7.1+К-L16 reconciliation) + 2/3 (К-L17 §3.2 capability syntax) intact.'
reviewer: Crystalka
risks_referenced:
- RISK-002
- RISK-004
- RISK-005
- RISK-006
capa_entries_referenced:
- CAPA-2026-05-09-K8.2-V2-REFRAMING
special_case_rationale: Superseded by DOC-A-MOD_OS_ARCHITECTURE per corpus rework EVT-2026-07-15-CORPUS_REWORK_R2_PLATFORM. Last-ratified reference preserved at docs/architecture/historical/MOD_OS_ARCHITECTURE.md; successor ratified LOCKED v1.0.0 2026-07-17 (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION) — authority-gap window closed.
---

---
title: Mod OS Architecture
nav_order: 25
---

# Mod OS Architecture — Dual Frontier

**Status:** LOCKED v1.12.0 — Phase 0 closed; the full amendment chronicle is in the version history below. Every architectural decision in this document is binding input to mod-system implementation work. Migration state authority is [docs/ROADMAP.md](../ROADMAP.md) (Mod-OS M-rows; see §11.1). Items marked **✓ LOCKED** reflect decisions taken during Phase 0 deliberation; deviation in implementation requires reopening this document, not improvisation in code.

**Version history:**

- v0.1 (initial draft) — initial specification of the mod-as-process model. Five strategic decisions locked; seven detail decisions (D-1 through D-7) collected in §12 as ⚠ DECISION pending human resolution.
- v1.0 — Phase 0 closed. All seven open decisions resolved and locked. Implementation phases M1–M10 may begin.
- v1.1 — non-semantic corrections from the first independent audit (M1–M3.1):
  - §4.1: `Log()` parameter type is `ModLogLevel`, not `LogLevel`. The original name collided with `Microsoft.Extensions.Logging.LogLevel`; the implementation correctly chose a kernel-namespaced enum, and the spec is brought in line.
  - §2.2: `dependencies[i].optional` (bool, default `false`) documented as a recognised optional flag on inter-mod dependencies. Discovered as `ModDependency.IsOptional` in the M1 implementation, kept on the strength of utility, and now ratified.
  - No semantic changes. No locked decision is altered. M1–M3.1 implementations continue to comply.
- v1.2 — non-semantic corrections from the M3 closure review:
  - §3.6: hybrid enforcement formulation. The v1.0/v1.1 wording described capability checks as load-time only and the hot path as "free of permission lookups." The M2 implementation (commits `35dc5b2`, `0d5b32f`) added a runtime per-call check inside `RestrictedModApi.EnforceCapability` — a hash-set lookup measured negligible on the hot path — which is exactly what §4.2 and §4.3 already specify. v1.2 brings §3.6 in line with §4.2/§4.3 and the implementation: enforcement is hybrid, load-time as primary gate plus runtime as second-layer defence.
  - §3.5 + §2.1: production components consumed by the §2.1 example manifest (`WeaponComponent`, `ArmorComponent`, `AmmoComponent`, `ShieldComponent`, `HealthComponent`) annotated with `[ModAccessible]` per D-1 LOCKED. The §2.1 example itself was expanded to include `kernel.read:AmmoComponent` and `kernel.read:ShieldComponent` — Vanilla.Combat requires both (ammo accounting per §11 of the original Phase 5 spec, shield damage routing per §6.4 of the GDD), but the v1.0/v1.1 example listed only three components as a sketch. v1.2 brings the example in line with what a real combat mod actually needs. Without these annotations the §2.1 example manifest would fail Phase C with `MissingCapability` — the spec example is now end-to-end loadable.
  - §11.1: M3.4 added as deferred milestone (CI Roslyn analyzer per D-2 hybrid completion). M3.1, M3.2, M3.3 closed by M3 closure review; M3.4 unblocked when the first external (non-vanilla) mod author appears — runtime `CapabilityViolationException` already catches dishonest `[ModCapabilities]` attributes, so the analyzer is developer-experience tooling for early feedback before publication, not a runtime safety boundary.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. M3 implementations continue to comply.
- v1.3 — non-semantic correction from the M4.3 implementation review:
  - §2.2: `entryAssembly` and `entryType` rows in the manifest field reference table reworded from "ignored for `kind=shared`" to "must be empty for `kind=shared`". The v1.0–v1.2 wording contradicted §5.2 step 1, which explicitly requires these fields to be empty for shared mods. The M4.3 implementation (`ContractValidator` Phase F, commit `e0151d8`) enforces §5.2 wording — non-empty `entryAssembly` or `entryType` on a shared mod manifest produces `ValidationErrorKind.SharedModWithEntryPoint`. v1.3 brings §2.2 in line with §5.2 and the implementation.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. M4 implementations continue to comply.
- v1.4 — non-semantic clarifications from the M7 pre-flight readiness review:
  - §9.5 step 7: explicit GC pump protocol added. Each iteration of the `WeakReference` spin loop performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` before re-checking `WeakReference.IsAlive`. The double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout (matches the §9.5 step 7 v1.0 wording). The cadence is implementation-tunable; the GC pump bracket is mandatory. Without this clarification the v1.0 wording «spins on `WeakReference`» admits flaky implementations, and the §11.4 stop condition («WeakReference unload tests are flaky — any failure rate above 0%») would trigger spuriously. v1.4 brings §9.5 step 7 in line with the only stable implementation pattern.
  - §9.5: new sub-section §9.5.1 «Failure semantics» added, locking the best-effort discipline already implicit in the chain. Steps 1–6 are sequential and best-effort: if any step throws, the loader logs the exception with `(modId, stepNumber)`, surfaces a non-blocking `ValidationWarning`, and continues to the next step. The `ModLoader.UnloadMod` swallowed `try/catch` around `mod.Instance.Unload()` (in place since M0) is consistent with this discipline. After step 6, if step 7 times out, the existing `ModUnloadTimeout` warning fires; the mod is removed from the active set regardless. There is no atomic-unload guarantee — `Unload` is conceptually irreversible (subscriptions removed cannot be re-attached without re-running `Subscribe`); the chain is structured so each step is a no-op if its predecessor failed (e.g. `RemoveSystems` on a mod with no registered systems is harmless). This formalises a discipline the M0–M6 implementation already follows; no new state is introduced to §9.1.
  - No semantic changes. No locked decision (D-1 through D-7) is altered. No state added to §9.1. M0–M6 implementations continue to comply.
- v1.5 — non-semantic correction from Audit Campaign Pass 2 (cumulative drift audit, 2026-05-01):
  - §11.2: baseline enumeration of `ValidationErrorKind` expanded to include `IncompatibleContractsVersion` and `WriteWriteConflict` alongside `MissingDependency` and `CyclicDependency`. The v1.0–v1.4 wording «The current enum has `MissingDependency` and `CyclicDependency`» is declarative without a non-exhaustive qualifier (no `e.g.`, no `among others`); on a strict reading it implies a complete 2-member baseline. The actual baseline at every v1.x snapshot is 4 members — `IncompatibleContractsVersion` is used by `ContractValidator` Phase A (`ValidateContractsVersions`) for `RequiresContractsVersion` failures, and `WriteWriteConflict` is used by Phase B (`ValidateWriteWriteConflicts`) for component write-collision detection per §10.1 («Mod registers a system that conflicts with another mod's system»). Both errors existed before any M-phase migration; the spec wording understated the baseline. v1.5 brings §11.2 in line with `src/DualFrontier.Application/Modding/ValidationError.cs:9–83` (11 enum members total: 4 baseline + 7 migration additions, matching the §11.2 «migration adds» list).
  - No semantic changes. No locked decision (D-1 through D-7) is altered. No state added to §9.1. M0–M7.3 implementations continue to comply.
- v1.6 — ratification of GPU compute integration capabilities, gating K9 (field storage abstraction) and G0–G9 (Vulkan compute integration) milestones per [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) v2.0 LOCKED:
  - §3.2: capability syntax extended with `field.*` and `pipeline.*` verbs. The `read`/`write` verbs apply only to entity-keyed components; field operations require dedicated verbs because the access pattern (point queries, dense spans, compute dispatch) is structurally distinct from entity component access. Pipeline registration is mod-side; pipeline dispatch is implicit through field dispatch and does not require a separate verb in v1.6 (deferred to v1.7 if cross-mod pipeline reuse becomes a feature).
  - §3.5: kernel-provided capability set extended via new `[FieldAccessible]` annotation. Field types ship in vanilla mod assemblies (not the kernel), so kernel-provided field capabilities are the *infrastructure verbs* (e.g. `kernel.field.acquire`, `kernel.field.dispatch`); concrete field tokens (e.g. `mod.dualfrontier.vanilla.magic.field.read:vanilla.magic.mana`) are mod-provided and resolve through the dependency chain established in §3.4.
  - §3.7: cross-check extended for `[FieldAccess]` and `[ComputePipelineAccess]` attributes on systems. The drift-prevention principle from §3.7 is preserved — manifest-vs-code mismatches surface as load-time errors, never silent.
  - §4.6 NEW: `IModApi` v3 surface — `Fields` and `ComputePipelines` sub-APIs. (Historical note v1.6 framed v3 as additive over v2 with v2 mods continuing to load. v3 was subsequently made strict in K8.3+K8.4 cutover — v2 IModApi deleted entirely, manifest parser rejects any version other than `"3"`. See v1.8 amendment below.)
  - §11.2: six new `ValidationErrorKind` entries (`FieldRegistrationConflict`, `InvalidFieldDimensions`, `FieldCapabilityMismatch`, `ComputePipelineCompilationFailed`, `ComputePipelineRegistrationConflict`, `ComputeUnsupportedWarning`). The first five are blocking errors; the sixth is a non-blocking warning when Vulkan 1.3 compute is unavailable and CPU fallback engages per [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) "Failure modes → CPU fallback".
  - §11.1: M3.5 added as deferred milestone — capability registry refresh for field types via `[FieldAccessible]` scan extension to `KernelCapabilityRegistry`. Unblocked at K9 in-progress.
  - No semantic changes to v1.5 decisions. No locked decision (D-1 through D-7) is altered. K9 + G0–G9 implementations begin against this surface.
- v1.7 — K-L3.1 bridge formalization applied (session 2026-05-10):
  - §«Modding с native ECS kernel» (lines 1149–1150 v1.6): rewritten to reflect K-L3.1 — Path α (`unmanaged struct` via `RegisterComponent<T>`) and Path β (managed `class` via `[ManagedStorage]` + `RegisterManagedComponent<T>`) as first-class peers; cross-mod direct managed-path access structurally impossible by ALC isolation; within-mod cross-path access via dual `SystemBase` API.
  - §3.5 D-1 LOCKED note: path orthogonality clarified — `[ModAccessible]` (already `Class | Struct` per K4 prerequisite) and capability strings function uniformly across paths.
  - §4.6 IModApi v3 surface: `RegisterManagedComponent<T> where T : class, IComponent` added alongside existing `RegisterComponent<T>` (Path α) and `Fields`/`ComputePipelines` v1.6 additions.
  - §11.1: M3.5 deferred milestone description extended (analyzer covers Path α/β consistency in addition to `[FieldAccessible]` scan extension); analyzer ships post-migration per Q5.b deferral.
  - No semantic changes to v1.6 decisions. No locked decision (D-1 through D-7) is altered. Authority: K-L3.1 amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`.
- v1.8 — IModApi v3 strict (K8.3+K8.4 cutover 2026-05-14, audit cleanup 2026-05-16):
  - §4.6 + §4.6.3: v3 reframed from additive-over-v2 to **strict v3-only**. Per `src/DualFrontier.Contracts/Modding/IModApi.cs` lines 16-27 (canonical): «v2 IModApi deleted entirely — no backward compatibility. Mods that registered class-shape components without `RegisterManagedComponent` fail to compile post-K8.3+K8.4; the manifest parser also rejects any `manifestVersion` other than `"3"`.» `RegisterComponent<T>` carries `where T : unmanaged, IComponent` (Path α — NativeWorld struct storage); `RegisterManagedComponent<T>` carries `where T : class, IComponent` (Path β — per-mod managed storage, requires `[ManagedStorage]`).
  - §4.6.3 «Backward compatibility» section: rewritten to remove v2-continues-to-load claim. v2 grace-period text deleted per §1.4 LOCK (no historical subsection in current Tier 1 doc).
  - No semantic changes to v1.7 decisions. No locked decision (D-1 through D-7) is altered. Authority: `IModApi.cs` verbatim + CLEANUP_CASCADE_BRIEF §3 Commit 11.
- v1.9 — К10.2 native bus closure amendments (2026-05-18, commit `a09aaba`; entry backfilled 2026-06-12 from git history — the v1.9 commit bumped the register version without updating this changelog):
  - §3.2: tier-prefixed bus capability verbs (`fast`/`normal`/`background` × `publish`/`subscribe`) per К-L15 + S-LOCK-4; `kernel.publish:`/`kernel.subscribe:` retained as Normal-tier aliases.
  - §9.5: step 3.5 added (`df_scheduler_unload_mod_native_state` native unload primitive); §9.5.1 extended to cover it.
  - §11.2: `FastTierContractViolation`, `BusTierMismatch`, `BackgroundCoalesceMissing` added (landed in `ValidationErrorKind`).
- v1.10 — К10.3 v2 К-L17 display composition amendment (2026-05-20, commit `9c660d9`; entry backfilled 2026-06-12 from git history):
  - §3.2: К-L17 layer capability verbs (`layer.intent`, `layer.combat_feedback`) + `[Layer(LayerType.…)]` registration pattern; `LayerCapabilityMismatch` named in §11.2 as reserved.
- v1.11 — К10.3 v2 К-L18 quiescent state amendment (2026-05-20, commit `f09055a`; entry backfilled 2026-06-12 from git history):
  - §9.5: step 3.5 extended with the pipeline-slot quiescence check; step 3.6 added (V resource cleanup placeholder); §9.5.1 К-L18 rejection semantics; §9.7 added (hot reload К-L18 compliance).
  - §11.2: `QuiescentStatePreconditionViolated`, `PipelineQuiescenceTimeout`, `VulkanModResourceCleanupFailed` named (reserved).
- v1.12.0 (2026-06-12) — sectional code-truth rewrite per ARCHITECTURE_TRUTH_CASCADE_BRIEF D1:
  - §0: OS-mapping matrix rewritten from target-state to realized state with on-disk evidence names (publish/subscribe, capability model, shared library, package-manager semantics, hot reload — all shipped M2–M7).
  - §2: retitled «Manifest v3». `manifestVersion: "3"` documented as a required field; strict-v3 parse gate (`ManifestParser`) replaces the «backward-compatible v2 extension» framing; §2.2 field table corrected against the parser (`apiVersion` optional with `requiresContractsVersion` fallback per `ModManifest.EffectiveApiVersion`); §2.3 split into parse-time vs batch-time validation with the capability grammar quoted from `ManifestCapabilities`.
  - §3.5: capability-set mechanism corrected to the startup reflection scan (`KernelCapabilityRegistry.BuildFromKernelAssemblies`) — the «generated at build time, embedded as a resource» wording never matched the implementation. §3.6 case 3: v1-grace-period residue replaced with the real empty-capability short-circuit behavior. §3.8: load-time mechanism aligned with D-2 LOCKED (attribute cross-check; no load-time Roslyn scan exists).
  - §4: retitled «IModApi» (single strict surface). §4.1 re-quoted from `IModApi.cs` (`RegisterComponent<T> where T : unmanaged, IComponent`; `RegisterSystem<T> where T : class`). §4.2 marker corrected to `[EventBus("…")]`. §4.4 intro aligned with D-3 LOCKED (structural barrier, no analyzer). §4.5 grace-period section replaced with the strict-v3 statement (DRIFT-011 residue removed). §4.6.1 sub-API surfaces re-quoted from the real contracts (`IModFieldApi`/`IFieldHandle`/placeholder `IModComputePipelineApi`) with explicit nullability truth; §4.6.2 corrected (runtime gating only — the `[FieldAccess]`/`[ComputePipelineAccess]` load-time attributes do not exist); §4.6.4 example corrected to the real entry point and surface.
  - §10.1: threat table rewritten to the current fault model (`ModIsolationException` + `ModFaultHandler` deferred-unload path; runtime isolation-guard route removed at К8.3+К8.4). §10.4: WeakReference unload tests marked closed at M7.3 (`M73Phase2DebtTests`); test bullets updated to realized suites.
  - §11: deduplicated against docs/ROADMAP.md — the migration-phase table removed (ROADMAP M-rows are the migration state authority); §11.2 rewritten as the realized 15-member `ValidationErrorKind` registry with reserved-but-absent names called out explicitly; stop conditions renumbered §11.4 → §11.3.
  - See-also: ROADMAP link corrected to `../ROADMAP.md`; §9.4 Persistence README link depth fixed; VULKAN_SUBSTRATE version pins removed from living prose (citation form: stable identifiers + section topics).
  - No semantic changes. No locked decision (D-1 through D-7) is altered; no К-series amendment is altered. Drift-to-code corrections only.

---

## Preamble — How to use this document

**Authority.** This document is the single architectural authority for the mod system of Dual Frontier. During implementation, every interface, attribute, manifest field, and lifecycle step traces back to a section here. Disagreement with the specification is escalated to the human (via §12 open decisions) — never resolved by improvisation in code.

**Scope.** The specification governs:

- The structural relationship between the kernel (`DualFrontier.Core` + `DualFrontier.Contracts`) and a loaded mod.
- The manifest schema and the loader pipeline that consumes it.
- The `IModApi` surface and its semantic guarantees.
- Capability declaration, capability checking, and capability granularity.
- Type sharing across `AssemblyLoadContext` boundaries.
- Bridge replacement and conflict resolution.
- Versioning across three independent axes (kernel API, mod self, inter-mod).
- The mod lifecycle, including hot reload through the mod menu.
- The threat model, distinguishing what mods can and cannot do.

The specification does **not** govern:

- Specific gameplay content (weapons, recipes, biomes) — these are decided by the mod author within the architecture.
- Game-design questions (balance, narrative, pacing).
- Performance budgets for individual systems — covered by [PERFORMANCE](./PERFORMANCE.md).
- Methodology of the development pipeline — covered by [METHODOLOGY](/docs/methodology/METHODOLOGY.md).

**Strategic locked decisions.** Five top-level decisions taken during Phase 0; the seven detail decisions (D-1 through D-7) are listed in §12:

1. **✓ LOCKED.** Capability granularity is **per-event-type and per-component-type**.
2. **✓ LOCKED.** Bridge replacement is **explicit** via the manifest's `replaces` field.
3. **✓ LOCKED.** Hot reload is fully supported, but **only through the mod menu** with the simulation paused.
4. **✓ LOCKED.** Vanilla content ships as **multiple mods** mirroring the existing `DualFrontier.Systems.*` structure.
5. **✓ LOCKED.** Versioning is a **three-tier model**: kernel API SemVer (existing `ContractsVersion`), mod self SemVer, inter-mod dependency SemVer with caret-prefix support.

**The "stop, escalate, lock" rule.** When implementation encounters a design question not answered here, the response is "stop, document in §12, wait for the human to lock" — not "guess." The structural strength of the mod system depends on the specification being the only source of architectural truth.

---

## 0. Executive summary and OS mapping

The design treats Dual Frontier as a small operating system and each mod as a process running on top of it. The mapping below is not decorative — every row names the counterpart the engine provides and the on-disk artifact that realizes it.

| OS concept | Dual Frontier counterpart | Realized by |
|---|---|---|
| Kernel | Native ECS core (К-series) behind `DualFrontier.Core` + `DualFrontier.Contracts` | `NativeWorld` interop surface; see [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) |
| Process | A mod loaded into its own `AssemblyLoadContext` | `ModLoadContext` (collectible ALC per regular mod) |
| Process isolation (MMU) | Structural isolation: a mod ALC cannot resolve `DualFrontier.Core`/`DualFrontier.Application`; component storage is reachable only through `NativeWorld`; `[SystemAccess]` declarations feed scheduler edge-building | `ModLoadContext` resolution rules; `SystemExecutionContext` (per-system context carrier); `DependencyGraph` |
| Syscall | `IModApi` | `RestrictedModApi` (M2) — real `Publish`/`Subscribe` routed via `ModBusRouter` to the `IGameServices` domain buses |
| IPC between processes | `IModContract` via `IModContractStore` | `ModContractStore`; three contract levels (§6) |
| Device driver | A mod-supplied system registered via `RegisterSystem` | `ModRegistry` + scheduler graph rebuild |
| Register a new "syscall" or new device | Mod adding new event/contract types | Shared mods (M4) + `capabilities.provided` resolution (`ContractValidator` Phase C) |
| Shared library (`.so` / `.dll`) | A library-only mod with shared types, no entry point | `SharedModLoadContext` (M4) |
| `dlopen` / hot reload | Pause-aware reload via the mod menu | `ModIntegrationPipeline.Pause()`/`Apply()`/`Resume()` + §9.5 unload chain (M7) |
| Capability model | Manifest `capabilities` list + load-time and runtime checks | `KernelCapabilityRegistry`, `[ModAccessible]`, `ContractValidator` Phases C+D, `CapabilityViolationException` (M3) |
| Package manager dependency resolution | Manifest `dependencies` with SemVer constraints | `VersionConstraint`, `ContractValidator` Phases A+G, topological load order (M5) |

Sections 1–10 specify the architecture; every row above is shipped. Migration state (which milestones are closed, pending, or deferred) is tracked in [docs/ROADMAP.md](../ROADMAP.md), not here (§11.1).

---

## 1. Mod topology — three mod kinds

Mods are not uniform. The architecture distinguishes three categories, each with a different role in the load graph and different rules at the loader.

### 1.1 Regular mod

The default kind. Has an entry point (`IMod` implementation), runs `Initialize(api)` to register components, systems, and subscriptions, may publish and consume contracts.

- Lives in its own `AssemblyLoadContext`.
- May depend on shared mods.
- May not depend on regular mods directly — only on contracts they publish.
- Manifest: `kind: "regular"`. Default when omitted.

### 1.2 Shared mod (library)

Defines types — `record`s, `interface`s, `enum`s — that other mods reference at compile time. Has **no `IMod` implementation** and registers no systems. The loader places its assembly in a **shared `AssemblyLoadContext`** so dependent mods see the same `Type` instances.

- Lives in the shared ALC, separate from regular mods.
- Cannot subscribe, publish, or register — it is a pure type vendor.
- Loaded before any dependent regular mod.
- Manifest: `kind: "shared"`. Required.

The shared mod is the solution to the type-sharing problem (§5). Without it, Mod A defining `record FireballCastEvent : IEvent` and Mod B subscribing to that event cannot interoperate — the two ALCs produce two distinct `Type` instances even when the assembly bytes are identical.

### 1.3 Vanilla mod

A regular mod authored by the engine team that ships with the base game. Architecturally identical to a third-party regular mod — same manifest schema, same `IModApi` surface, same isolation rules. The distinction is editorial: vanilla mods are the canonical reference implementations and the test polygon for the mod system itself.

- Manifest: `kind: "regular"` (vanilla is not a separate kind).
- Convention: id begins with `dualfrontier.vanilla.` (e.g. `dualfrontier.vanilla.combat`).
- Convention: shipped in `mods/` directory under `DualFrontier.Mod.Vanilla.<Slice>/`.

The split into multiple vanilla mods follows the existing `DualFrontier.Systems.*` decomposition. Initial set: `Vanilla.Combat`, `Vanilla.Magic`, `Vanilla.Inventory`, `Vanilla.Pawn`, `Vanilla.World`. Each may depend on `Vanilla.Core` (a shared mod with definition records used across slices).

### 1.4 Load graph

```
                 ┌────────────────────────────────┐
                 │  shared ALC                    │
                 │  ┌──────────────────────────┐  │
                 │  │ DualFrontier.Mod.        │  │
                 │  │   Vanilla.Core (shared)  │  │
                 │  │ DualFrontier.Mod.        │  │
                 │  │   MagicProtocol (shared) │  │
                 │  └──────────────────────────┘  │
                 └────────────────────────────────┘
                          ▲                ▲
                          │ references     │
   ┌──────────────────────┴┐              ┌┴───────────────────────┐
   │ regular ALC            │              │ regular ALC            │
   │ Vanilla.Combat (mod)   │              │ Vanilla.Magic (mod)    │
   └────────────────────────┘              └────────────────────────┘
                          ▲                ▲
                          │  depends-on    │
                          └────────┬───────┘
                                   │
                          ┌────────┴───────────┐
                          │ regular ALC        │
                          │ Vanilla.Inventory  │
                          └────────────────────┘
```

**Invariants:**

- Shared ALC is loaded once at game start and never unloaded during the session.
- Each regular mod has its own ALC with `IsCollectible = true`, allowing unload.
- A regular mod's ALC may resolve types from the shared ALC, but never from another regular ALC.
- Cycles between regular mods are forbidden; cycles in the shared ALC dependency graph are forbidden.

---

## 2. Manifest v3

The manifest schema is **strict v3**. Every `mod.manifest.json` must declare `"manifestVersion": "3"`; `ManifestParser` rejects a missing field or any other value at parse time (the К8.3+К8.4 cutover removed v1/v2 manifest acceptance together with the v2 `IModApi` — §4.6.3). There is no backward-compatible extension path and no grace period. The vanilla mod skeletons under `mods/DualFrontier.Mod.Vanilla.*/mod.manifest.json` are the on-disk reference instances of this schema.

### 2.1 Schema (v3)

```json
{
  "manifestVersion": "3",
  "id": "dualfrontier.vanilla.combat",
  "name": "Vanilla Combat",
  "version": "1.0.0",
  "author": "Dual Frontier Team",
  "kind": "regular",
  "apiVersion": "^1.0.0",
  "entryAssembly": "DualFrontier.Mod.Vanilla.Combat.dll",
  "entryType": "DualFrontier.Mod.Vanilla.Combat.CombatMod",
  "hotReload": true,

  "dependencies": [
    { "id": "dualfrontier.vanilla.core", "version": "^1.0.0" },
    { "id": "dualfrontier.vanilla.inventory", "version": "^1.0.0" }
  ],

  "replaces": [
    "DualFrontier.Systems.Combat.CombatSystem",
    "DualFrontier.Systems.Combat.DamageSystem",
    "DualFrontier.Systems.Combat.ProjectileSystem"
  ],

  "capabilities": {
    "required": [
      "kernel.publish:DualFrontier.Events.Combat.DamageEvent",
      "kernel.publish:DualFrontier.Events.Combat.DeathEvent",
      "kernel.subscribe:DualFrontier.Events.Combat.ShootGranted",
      "kernel.read:DualFrontier.Components.Combat.WeaponComponent",
      "kernel.read:DualFrontier.Components.Combat.ArmorComponent",
      "kernel.read:DualFrontier.Components.Combat.AmmoComponent",
      "kernel.read:DualFrontier.Components.Combat.ShieldComponent",
      "kernel.write:DualFrontier.Components.Shared.HealthComponent"
    ],
    "provided": [
      "mod.dualfrontier.vanilla.combat.publish:DualFrontier.Mod.Vanilla.Combat.WeaponDef"
    ]
  }
}
```

### 2.2 Field reference

| Field | Type | Required | Default | Notes |
|---|---|---|---|---|
| `manifestVersion` | string | yes | — | Must be exactly `"3"`. A missing field or any other value is rejected by `ManifestParser` at parse time. |
| `id` | string | yes | — | Reverse-domain. Must be globally unique. |
| `name` | string | yes | — | Human-readable. |
| `version` | string (SemVer) | yes | — | Mod self-version. Strict `MAJOR.MINOR.PATCH`. |
| `author` | string | no | `""` | Free-form. |
| `kind` | enum | no | `"regular"` | One of `regular`, `shared`. |
| `apiVersion` | string (SemVer with caret) | no | — | Compatibility against `ContractsVersion.Current`. When absent, the loader falls back to `requiresContractsVersion` via `ModManifest.EffectiveApiVersion`. |
| `requiresContractsVersion` | string (SemVer) | no | `"1.0.0"` | v1-era field retained as the `apiVersion` fallback consumed by `EffectiveApiVersion`. New manifests should declare `apiVersion`. |
| `entryAssembly` | string | conditional | `"{id}.dll"` | Required for `kind=regular`; **must be empty for `kind=shared`** (per §5.2 step 1). |
| `entryType` | string | conditional | scan-for-IMod | Required for `kind=regular`; **must be empty for `kind=shared`** (per §5.2 step 1). |
| `hotReload` | bool | no | `false` | When `false`, mod loads only at session start; menu refuses to reload it. |
| `dependencies` | array of `{id, version, optional}` | no | `[]` | Each `version` is a SemVer constraint (§8). The optional `optional` boolean (default `false`) marks a dependency that the loader may treat as soft: when the named mod is absent, an optional dependency emits a warning rather than a `MissingDependency` error. Required (default) dependencies still hard-fail. Entries may alternatively be plain strings (id only, no constraint); string and object entries cannot be mixed within one array. |
| `replaces` | array of string (FQN) | no | `[]` | Fully-qualified type names of systems this mod replaces. Only meaningful for `kind=regular`. |
| `capabilities.required` | array of string | no | `[]` | See §3 for capability syntax. |
| `capabilities.provided` | array of string | no | `[]` | See §3. |

### 2.3 Validation — parse time and batch time

**Parse time** (`ManifestParser.Parse`; a failure throws `InvalidOperationException` naming the manifest path):

1. `manifestVersion` present and exactly `"3"` (the strict-v3 gate; the rejection message carries the `ValidationErrorKind.IncompatibleContractsVersion` semantic).
2. `id`, `name`, `version` present and non-empty.
3. `apiVersion` (when present) and every object-form `dependencies[i].version` (when present) parses as a `VersionConstraint` — `[^]MAJOR.MINOR.PATCH`; tilde is rejected with a directive to use caret (§8.4).
4. `kind` (when present) is `regular` or `shared`.
5. `dependencies` entries are all strings or all objects (no mixed arrays); every entry has a non-empty id.
6. Every capability token matches the authoritative pattern (`ManifestCapabilities.Parse`; the offending token is named in the error):

   ```
   ^(kernel|mod\.[a-z0-9.]+)\.((?:fast|normal|background)\.(?:publish|subscribe)|publish|subscribe|read|write|field\.(read|write|acquire|conductivity|storage|dispatch)|pipeline\.register):[A-Za-z][A-Za-z0-9_.]+$
   ```

   Note: the К-L17 layer tokens (§3.2) are emitted into the kernel-provided set by `KernelCapabilityRegistry`, but `layer.*` is not part of this manifest grammar — a manifest cannot currently declare layer tokens in `capabilities.required`.

**Batch time** (`ContractValidator`, producing typed `ValidationError`s per §11.2): kernel API version compatibility (Phase A), write-write conflicts (Phase B), capability resolution against kernel + listed dependencies (Phase C), `[ModCapabilities]` cross-check (Phase D), contract-type placement (Phase E), shared-mod compliance incl. the §5.2 empty-entry-point rule (Phase F), inter-mod dependency versions (Phase G), and `replaces` validation incl. the cross-batch conflict check per §7 (Phase H).

---

## 3. Capability model

Capabilities are **named, declared, and statically checked permissions**. Every operation a mod performs that touches the kernel or another mod must be backed by an entry in `capabilities.required`. Capabilities that the mod itself adds to the system (new event types, new contracts) appear in `capabilities.provided`.

### 3.1 Granularity ✓ LOCKED

Granularity is **per-event-type and per-component-type**. A capability never applies to a category, namespace, or wildcard:

- Permitted: `kernel.publish:DualFrontier.Events.Combat.DamageEvent`
- Forbidden: `kernel.publish:DualFrontier.Events.Combat.*`
- Forbidden: `kernel.publish:combat`
- Forbidden: `kernel.publish:*`

The cost is verbose manifests for content-rich mods. The benefit is that a `git diff` of a manifest reveals exactly which new kernel surface the mod began touching, enabling reviewable security and change control.

### 3.2 Syntax

```
<provider>.<verb>:<fully-qualified-type-name>
```

- `provider`:
  - `kernel` — provided by `DualFrontier.Contracts` itself.
  - `mod.<modId>` — provided by another loaded mod (typically a shared mod publishing event types).
- `verb`: one of `publish`, `subscribe`, `read`, `write`, `field.read`, `field.write`, `field.acquire`, `field.conductivity`, `field.storage`, `field.dispatch`, `pipeline.register`, **(K10.2)** tier-prefixed bus verbs `fast.publish`, `fast.subscribe`, `normal.publish`, `normal.subscribe`, `background.publish`, `background.subscribe`, or **(К10.3 v2)** К-L17 display composition layer verbs `layer.intent`, `layer.combat_feedback`.
- `fully-qualified-type-name`: the C# FQN of the event or component type, or the namespaced field/pipeline id for the `field.*` and `pipeline.*` verbs.

The `read` and `write` verbs apply to entity-keyed components (`IComponent`). The `publish` and `subscribe` verbs apply to events (`IEvent`). The `field.*` verbs apply to spatial fields (`RawTileField<T>` per [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9 — dense 2D grids with conductivity map and storage flags). The `pipeline.register` verb applies to mod-owned compute pipelines registered via `IModApi.ComputePipelines` (§4.6).

Field verb semantics:

- `field.read:<id>` — point query (`ReadCell(x, y)`) returning a single cell value.
- `field.write:<id>` — single cell mutation (`WriteCell(x, y, value)`).
- `field.acquire:<id>` — dense span access (`AcquireSpan()`) for bulk reads or batch updates.
- `field.conductivity:<id>` — modify the conductivity map (per-cell diffusion coefficient `D`); used by mods that own the field to wire physical infrastructure (wires, pipes).
- `field.storage:<id>` — modify the per-cell storage flag (capacitance marker); used to designate batteries, tanks, thermal mass cells.
- `field.dispatch:<id>` — issue a compute dispatch on the field via a registered pipeline. The pipeline itself does not require a separate `pipeline.dispatch` capability — the field-side dispatch verb covers it. Pipelines registered by other mods are reachable through the dependency chain (§3.4).

Bus tier verb semantics (K10.2, per K-L15 + S-LOCK-4):

- `kernel.fast.publish:<FQN>` / `kernel.fast.subscribe:<FQN>` — Fast tier (≤1ms latency target). Synchronous bypass dispatch; bounded execution contract enforced via runtime monitor (`FastTierContractMonitor`).
- `kernel.normal.publish:<FQN>` / `kernel.normal.subscribe:<FQN>` — Normal tier (batched callback per-phase). К-L7 atomic-from-observer preserved within batch.
- `kernel.background.publish:<FQN>` / `kernel.background.subscribe:<FQN>` — Background tier (coalesce + idle-slot dispatch). Background tier event types require coalesce function declaration via `[EventTier(BusTier.Background, CoalesceFunctionTypeName = "...")]`; missing declaration emits `BackgroundCoalesceMissing` validation error (§11.2).

**Backward-compatible aliases** (S-LOCK-4): `kernel.publish:<FQN>` / `kernel.subscribe:<FQN>` continue к function for Normal-tier events. Mods authored prior к K10.2 require no manifest changes; tier-explicit tokens are opt-in for Fast/Background tier semantics.

Tier mismatch (manifest declares tier-specific capability но event type's `[EventTier]` attribute names a different tier) is caught at load time via `BusTierMismatch` (§11.2).

**К-L17 display composition layer capabilities** (К10.3 v2 Items 39+40, per S3-Q5 + S8-Q3 granular FQN pattern):

- `kernel.layer.intent:<FQN>` — mod registers an intent overlay layer (sub-pipeline-latency input surface; ≤16ms render latency contract).
- `kernel.layer.combat_feedback:<FQN>` — mod registers a combat feedback layer (К-L15 Fast tier consumer; ≤17ms event-к-visible latency contract).

Layer registration uses the `[Layer(LayerType.Intent | CombatFeedback)]` attribute on a concrete `DualFrontier.Application.Display.Layer` subclass; `KernelCapabilityRegistry` scans loaded assemblies and emits the corresponding capability tokens. Granular per FQN per tier — same uniformity as К10.2 bus tier tokens.

SimState and Static layer tiers use existing renderer-level capabilities (V substrate primitives) and do не surface layer capability tokens here. Vanilla layers register through the same attribute + capability pattern per К-L9 «Vanilla = mods».

Layer mismatch (attribute declares one `LayerType` но runtime layer instance reports a different `Type`) is reserved для validation error `LayerCapabilityMismatch` (§11.2 К10.3 v2 amendment, finalized в К-L18 load-bearing commit).

### 3.3 Reserved namespaces

- `kernel.*` is reserved for capabilities the kernel itself provides. Only the kernel may list these in `capabilities.provided` (as part of its own internal manifest — see §3.5).
- `mod.<modId>.*` is reserved for the mod with that exact `id`. Mods cannot claim to provide capabilities under another mod's namespace; the loader rejects such manifests.

### 3.4 Static check at load time

When a mod loads, the loader validates:

- For every `capabilities.required` entry of the mod, an entry with the same string exists in either:
  - The kernel's own provided set (§3.5), **or**
  - The `capabilities.provided` set of an already-loaded shared mod or regular mod listed in this mod's `dependencies`.
- A required capability cannot be satisfied by a mod *not* listed in `dependencies`. This is a hard rule — implicit dependency through shared capability is forbidden.

Failure produces a `ValidationError` of new kind `MissingCapability`, listing each unsatisfied capability and the mod that needs it.

### 3.5 Kernel-provided capability set

The kernel exposes a fixed list of capabilities derived from public types in the kernel-surface assemblies. The list is built once at startup by `KernelCapabilityRegistry.BuildFromKernelAssemblies()` — a reflection scan over `DualFrontier.Contracts`, `DualFrontier.Components`, and `DualFrontier.Events`: public concrete `IEvent` implementers yield `publish`/`subscribe` tokens per their `[EventTier]` (Normal when unattributed, with the legacy un-prefixed aliases per §3.2); `IComponent` implementers annotated `[ModAccessible]` yield `read`/`write` tokens; `[Layer]`-annotated classes yield the К-L17 layer tokens. Generic and nested types are skipped. Mods read the resulting set through `IModApi.GetKernelCapabilities()`.

> **✓ LOCKED (D-1).** `read` and `write` capabilities apply only to a **curated, opt-in subset** of public components. A component is reachable from a mod only when annotated with `[ModAccessible(Read = true, Write = false)]`. The component author actively decides what mods can touch; everything else is invisible to the capability resolver and produces a `MissingCapability` error if requested. Aligns with the project's structural-isolation philosophy: tighter blast radius, falsifiable surface.
>
> **Path orthogonality (K-L3.1, 2026-05-10).** `[ModAccessible]` annotation applies uniformly across Path α (`unmanaged struct`) and Path β (managed `class` via `[ManagedStorage]`). The attribute's `AttributeUsage` already targets `Class | Struct` (widened in K4 prerequisite commit). Capability strings carry verb + FQN (e.g. `kernel.read:Vanilla.Pawn.JobQueueComponent` or `mod.dualfrontier.vanilla.pawn.read:JobQueueComponent`) and are path-orthogonal — provider namespace prefix differs by ownership (kernel vs mod), not by storage path. The capability resolver dispatches internally to `NativeWorld` span access (Path α) or per-mod `ManagedStore<T>` lookup (Path β) per-T. See `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1 for the storage-path decision criterion.

### 3.6 Hybrid enforcement — load-time + runtime

Capability enforcement operates on two layers.

**Load-time** (primary gate, before the mod reaches `Active` state §9.1):

- §3.4 — every `capabilities.required` token must be provided by the kernel or by a listed dependency. Failure: `MissingCapability`.
- §3.7 — every registered system's `[SystemAccess]` declarations must be a subset of the mod's `capabilities.required`.
- §3.8 / D-2 — every registered system's `[ModCapabilities]` tokens must appear in the manifest's `capabilities.required`.

**Runtime** (second-layer defence inside `RestrictedModApi`):

- §4.2 — `Publish<T>` checks the per-mod required set; mismatch raises `CapabilityViolationException`. Hash-set lookup, `O(1)`, measured negligible on the hot path.
- §4.3 — `Subscribe<T>` checks the same set at subscribe time. Same exception, same cost.

Enforcement on isolated component access (`SystemExecutionContext` reads/writes via `[SystemAccess]`) and on bus delivery (subscribers receive only events for types they declared) operates independently of the capability layer and continues to function as in v1.

The runtime layer covers three cases the load-time gate cannot reach:

1. Reflection-based bypass of `[ModCapabilities]` declarations (deliberate violation rather than accident).
2. Event types constructed at runtime via generics or reflection.
3. Empty-capability manifests — when a manifest declares no capabilities at all (`capabilities` block absent, or both lists empty), `RestrictedModApi.EnforceCapability` short-circuits with a console warning instead of throwing. This leniency keeps capability-less skeleton mods (the vanilla skeletons on disk ship empty `required`/`provided` lists) loadable and publishing; a manifest that declares at least one capability is fully enforced on every `Publish`/`Subscribe` call.

### 3.7 Cross-check with `[SystemAccess]`

A mod's `capabilities.required` must be a **superset** of every `[SystemAccess]` declaration on every system the mod registers. The loader performs this cross-check after `Initialize(api)` returns:

```
for each system S registered by the mod:
    for each component C in S.[SystemAccess].Reads:
        require "kernel.read:<FQN of C>" or "mod.<provider>.read:<FQN of C>"
        in mod.capabilities.required
    for each component C in S.[SystemAccess].Writes:
        require "kernel.write:<FQN of C>" or "mod.<provider>.write:<FQN of C>"
    for each bus B in S.[SystemAccess].Buses:
        require capabilities consistent with what the system actually publishes (§3.8)
```

A drift between the manifest and the code is a load-time error, never silent.

### 3.8 Bus capability mapping

`[SystemAccess]` declares buses by name, not by event type. Capabilities are by event type. The loader cross-references via the per-system `[ModCapabilities(…)]` attribute (D-2 below): a system declaring `buses: ["Combat"]` and publishing `DamageEvent` must carry `publish:DamageEvent` in its attribute, and the attribute tokens must appear in the manifest's `capabilities.required` (`ContractValidator` Phase D). No load-time static analysis of call sites exists; the CI-side Roslyn honesty scan is the deferred D-2 completion — Planned, see [docs/ROADMAP.md](../ROADMAP.md) §Mod-OS Migration (M3.4).

> **✓ LOCKED (D-2).** Hybrid enforcement. Each mod-supplied system carries a `[ModCapabilities("publish:DamageEvent", "subscribe:ShootGranted")]` attribute; the loader cross-checks this attribute against the manifest at load time (cheap, no Roslyn dependency). A separate static-analysis pass runs in CI before mod publication, scanning `Services.X.Publish<T>` call sites and verifying the attribute is honest. Load-time check stays fast; CI catches drift between attribute and code reality.

---

## 4. IModApi

`IModApi` is a single, strict surface — version 3 (§4.6.3); a manifest's `manifestVersion: "3"` declares compatibility with exactly this surface. The interface below is condensed from `src/DualFrontier.Contracts/Modding/IModApi.cs` (the source of truth); signatures are verbatim.

### 4.1 Surface

```csharp
public interface IModApi
{
    // ── Component registration — Path α / Path β (K-L3.1) ─────────────────
    // Path α: NativeWorld-backed struct storage; unmanaged so the type
    // crosses the P/Invoke boundary without GCHandle pinning.
    void RegisterComponent<T>() where T : unmanaged, IComponent;
    // Path β: per-mod managed-class storage; T must carry [ManagedStorage],
    // absence raises ValidationErrorKind.MissingManagedStorageAttribute.
    void RegisterManagedComponent<T>() where T : class, IComponent;

    // ── System registration ────────────────────────────────────────────────
    void RegisterSystem<T>() where T : class;

    // ── Bus operations (capability-gated; §4.2/§4.3) ───────────────────────
    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;

    // ── Inter-mod contracts (§6) ───────────────────────────────────────────
    void PublishContract<T>(T contract) where T : IModContract;
    bool TryGetContract<T>(out T? contract) where T : class, IModContract;

    // ── Introspection and logging ──────────────────────────────────────────
    IReadOnlySet<string> GetKernelCapabilities();
    ModManifest GetOwnManifest();
    void Log(ModLogLevel level, string message);

    // ── Sub-APIs (§4.6) — nullable; mods check for null and degrade ───────
    IModFieldApi? Fields { get; }
    IModComputePipelineApi? ComputePipelines { get; }
}
```

### 4.2 `Publish<T>` semantics

- The implementation routes the event to the bus named by the event type's `[EventBus("…")]` marker (`EventBusAttribute.BusName`), resolved by `ModBusRouter` against the matching `IGameServices` bus property. An event type without the marker, or naming an unknown bus, resolves to no bus and the publish is a no-op.
- If the mod did not declare `publish:<FQN of T>` in its manifest, the call throws `CapabilityViolationException`. This is enforced via the per-mod capability set held inside `RestrictedModApi` (`EnforceCapability`). Exception: a manifest with an entirely empty capability block short-circuits with a warning — §3.6 runtime case 3.
- Delivery semantics (deferred vs immediate dispatch, bus tiers) are specified in [EVENT_BUS](./EVENT_BUS.md).

### 4.3 `Subscribe<T>` semantics

- The handler is wrapped to capture the calling thread's `SystemExecutionContext` at subscribe time (when one is current); the wrapper pushes that context around handler invocation, so the handler observes the same per-system context as the code that subscribed.
- A subscription is bound to the mod's lifetime: `RestrictedModApi.UnsubscribeAll` (called from the unload chain) removes every wrapper from the bus dispatcher.
- A capability check on `subscribe:<FQN of T>` runs at subscribe time. Without the capability, the call throws `CapabilityViolationException` (same §3.6 case-3 empty-manifest exception as `Publish`).
- Multiple subscriptions to the same `T` from the same mod are permitted; each handler runs.

### 4.4 The cast-prevention rule

A mod is forbidden from casting `IModApi` to a concrete type. The `RestrictedModApi` class is `internal sealed` with internal construction, and a regular mod's `AssemblyLoadContext` cannot resolve `DualFrontier.Application.*` types — the concrete type is structurally unreachable from a mod's compilation unit. No Roslyn analyzer and no runtime cast check exist for this rule; the structural barrier is the defense, per D-3:

> **✓ LOCKED (D-3).** `RestrictedModApi` is `internal sealed` with `internal` constructors. Combined with the rule that a regular mod's `AssemblyLoadContext` cannot resolve `DualFrontier.Application.*` types (the assembly is loaded only into the kernel's default ALC), the type is structurally unreachable from a mod's compilation unit. No Roslyn analyzer or runtime check is required in v1. If a real bypass attempt is observed in the wild, a defensive analyzer is added as v1.x amendment — but the structural barrier is the primary defense.

### 4.5 No backward compatibility — strict v3

There is no grace period and no backward compatibility with earlier `IModApi` versions. The v2 surface was deleted entirely at the К8.3+К8.4 cutover (2026-05-14), and `ManifestParser` rejects any `manifestVersion` other than `"3"` at parse time (§2). §4.6.3 carries the canonical statement.

### 4.6 IModApi v3 — Fields and Compute Pipelines (NEW in v1.6, made strict in v1.8)

v3 carries two sub-APIs: `Fields` (K9 field storage abstraction — contracts and registry shipped; see nullability truth in §4.6.1) and `ComputePipelines` (compute substrate — placeholder interface, no implementation) per [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) (architectural integration; Q-G-1 + Q-G-2). The sub-APIs are part of strict v3 — see §4.6.3 for the K8.3+K8.4 cutover that deleted v2 IModApi entirely (no backward compatibility).

### 4.6.1 Surface

The v3 members on `IModApi` itself are quoted in §4.1 (`RegisterManagedComponent<T>`, `Fields`, `ComputePipelines`). The sub-API contracts, quoted from `src/DualFrontier.Contracts/Modding/` (signatures verbatim; comments condensed):

```csharp
public interface IModFieldApi
{
    // Registers a new field in the calling mod's namespace. T must be
    // unmanaged (blittable). The id must start with "<modId>." — registering
    // outside the mod's own namespace throws CapabilityViolationException,
    // as does a missing "mod.<modId>.field.write:<id>" manifest token.
    IFieldHandle RegisterField<T>(string id, int width, int height) where T : unmanaged;

    // Retrieves a previously registered field. Own-namespace access requires
    // "mod.<modId>.field.read:<id>"; cross-mod access requires the read token
    // against the foreign mod's namespace.
    IFieldHandle GetField<T>(string id) where T : unmanaged;

    // True if a field with the given id is registered. No capability required.
    bool IsRegistered(string id);
}

public interface IModComputePipelineApi
{
    // Placeholder surface — no implementation exists in the codebase.
    string Name { get; }
}

public interface IFieldHandle
{
    string Id { get; }
    int Width { get; }
    int Height { get; }
    Type ElementType { get; }
}
```

Returns are typed as the type-erased `IFieldHandle` because the concrete `FieldHandle<T>` lives in `DualFrontier.Core.Interop`, which already references Contracts (the reverse reference would invert the dependency direction); mods downcast at the call site. The concrete `FieldHandle<T>` surface: `ReadCell(x, y)` / `WriteCell(x, y, value)` point access; `AcquireSpan()` returning a `FieldSpanLease<T>` (zero-copy span lease with indexer and `Dispose`); `SetConductivity`/`GetConductivity`; `SetStorageFlag`/`GetStorageFlag`; `SwapBuffers()`. Field operation failures raise `FieldOperationFailedException` (`DualFrontier.Core.Interop`); capability misses raise `CapabilityViolationException` from the `RestrictedFieldApi` wrapper.

**Nullability (code truth).** `Fields` is non-null only when the loader supplies a `FieldRegistry` to `RestrictedModApi` at construction. The production `ModIntegrationPipeline` currently constructs the API without one, so mods loaded through the pipeline observe `Fields == null` and must degrade gracefully; the field stack (`FieldRegistry`, `RestrictedFieldApi`, `FieldHandle<T>`) is exercised by tests. `ComputePipelines` is unconditionally `null` — `RestrictedModApi.ComputePipelines` is hardwired to return `null`, and `IModComputePipelineApi` is a placeholder with no implementing type. The real compute-pipeline surface (SPIR-V registration, dispatch) is Planned — see [docs/ROADMAP.md](../ROADMAP.md) §V substrate.

### 4.6.2 Capability cross-check

Every acquisition operation on `IModFieldApi` is gated by a manifest capability declaration per §3.2: `RegisterField` requires the mod's own-namespace `field.write` token, `GetField` requires the applicable `field.read` token, and a mismatch raises `CapabilityViolationException` (hash-set lookup inside `RestrictedFieldApi`). Per-cell traffic through an acquired handle is not re-checked — the handle is capability-gated at acquisition, mirroring how `RestrictedModApi` gates event traffic at the publish/subscribe boundary rather than per event. For field access this enforcement is runtime-only: no `[FieldAccess]`-style load-time attribute cross-check exists (the load-time attribute layer covers `[ModCapabilities]` bus tokens per §3.8/D-2).

### 4.6.3 Backward compatibility

**Strict v3 only — no backward compatibility.** Per [src/DualFrontier.Contracts/Modding/IModApi.cs](../../src/DualFrontier.Contracts/Modding/IModApi.cs) lines 16-27 (canonical statement): v2 `IModApi` was deleted entirely in K8.3+K8.4 cutover (2026-05-14). Mods compiled against v2 fail to load — the `RegisterComponent<T>` surface now carries `where T : unmanaged, IComponent` (was unconstrained in v2), and class-shape components must use `RegisterManagedComponent<T>` (Path β, K-L3.1 bridge) with the `[ManagedStorage]` attribute. The manifest parser rejects any `manifestVersion` other than `"3"`.

The `Fields` and `ComputePipelines` properties are nullable by contract — mods that opt into those sub-APIs check for null and degrade gracefully (nullability truth in §4.6.1; startup example in §4.6.4). This is forward-compatibility within v3, not backward-compatibility with prior IModApi versions.

### 4.6.4 Mod startup example

```csharp
public class MagicMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.Fields is null)
        {
            // No field registry wired on this build — degrade gracefully.
            api.Log(ModLogLevel.Warning, "Field API unavailable; mana mechanics disabled");
            return;
        }

        var manaField = (FieldHandle<float>)api.Fields.RegisterField<float>(
            "vanilla.magic.mana", 200, 200);

        api.RegisterSystem<ManaFieldUpdateSystem>();
    }
}
```

(`ComputePipelines` is checked the same way once a compute implementation exists; today it is always `null` — §4.6.1.) The full motivation for the field abstraction, Domain A vs Domain B distinction, and shader registration model is in [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) (architectural integration; mod-driven shader registration).

---

## 5. Type-sharing protocol

Without a shared `AssemblyLoadContext`, two mods that both reference a third assembly load that assembly twice and obtain two distinct `Type` instances. `typeof(FireballCastEvent)` in mod A and mod B refers to two different runtime types, and a subscription registered with the bus in one mod cannot be matched against an event published from another. Every interesting cross-mod scenario breaks at this boundary.

### 5.1 The shared ALC

The kernel creates a single `AssemblyLoadContext` named `"shared"` at startup. Its `IsCollectible = false` (the shared ALC never unloads while the game runs). Its `Resolving` event delegates to the kernel's own context for `DualFrontier.*` references.

```csharp
internal sealed class SharedModLoadContext : AssemblyLoadContext
{
    public SharedModLoadContext() : base("shared", isCollectible: false) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 1. DualFrontier.Contracts and friends — resolve via Default ALC.
        // 2. Other shared-mod assemblies — resolve from the shared cache.
        // 3. Otherwise — return null and let the runtime pick.
        ...
    }
}
```

### 5.2 Loader rules for shared mods

When the loader encounters a manifest with `kind: "shared"`:

1. Validate manifest as in §2.3, plus: `entryAssembly` and `entryType` must be empty.
2. Load the assembly **into the shared ALC**, not a new one.
3. Verify the assembly contains no `IMod` implementation. A shared mod with an entry point is a manifest-code mismatch and is rejected.
4. Run a reflection pass to enumerate public exported types. Cache by FQN.
5. Add the mod's `capabilities.provided` to the global capability resolver.
6. The shared mod is now loaded; dependent regular mods can begin loading.

### 5.3 Loader rules for regular mods that depend on shared mods

When loading a regular mod whose `dependencies` include a shared mod:

1. Verify all listed shared dependencies are already loaded.
2. Create a new collectible `AssemblyLoadContext` for the mod.
3. Configure the context's `Resolving` event to delegate to the shared ALC for any FQN that begins with the prefix of a depended-on shared assembly.
4. Load the mod's entry assembly.
5. Continue with the standard `IMod.Initialize` flow.

### 5.4 Restrictions on shared mods

- A shared mod must export only types: `record`, `class`, `interface`, `enum`, `struct`. Public methods are permitted only as members of these types.
- A shared mod cannot reference `DualFrontier.Core` or `DualFrontier.Application` (the same isolation rule as regular mods). It can reference only `DualFrontier.Contracts` and other shared mods.
- A shared mod cannot have a static constructor that touches mutable state. Loaders cannot guarantee initialization order across shared mods.
- A shared mod cannot read environment variables, files, or network resources at type-load time. Detection is best-effort; violations are documented in the threat model (§10).

### 5.5 Naming convention

Shared mods follow the convention `id: "<base>.protocol"` or `id: "<base>.types"`. Examples:

- `dualfrontier.vanilla.core` — shared types used by every vanilla slice (e.g., `WeaponDef`, `RecipeDef`).
- `dualfrontier.magic.protocol` — shared types for a magic-system protocol used by multiple magic mods.

The convention is enforced by the loader as a warning, not an error.

---

## 6. Three-level contracts

Mods communicate with the kernel and with each other through three distinct contract levels. Mixing levels (e.g. using a data contract for service dispatch) is a design error caught at review.

### 6.1 Level 1: Data contracts

A pure record describing a thing. No methods, no behavior, no inheritance other than `IModContract`.

```csharp
// In a shared mod (e.g. dualfrontier.vanilla.core):
public sealed record WeaponDef(
    string Id,
    int BaseDamage,
    DamageKind Kind,
    float Range,
    int AmmoPerShot
) : IModContract;
```

Use cases: weapon definitions, recipe definitions, biome parameters, faction templates. Mods publish instances via `IModApi.PublishContract`. The kernel and other mods read them via `TryGetContract`, typically iterating over the contract store at registration time.

### 6.2 Level 2: Service contracts

An interface a mod provides as a callable service. Implementations live in a regular mod; the interface lives in a shared mod so other mods can reference it at compile time.

```csharp
// In a shared mod (dualfrontier.cooking.protocol):
public interface ICookingService : IModContract
{
    bool TryCook(EntityId chef, RecipeId recipe, out CookResult result);
}

// In a regular mod (dualfrontier.vanilla.cooking):
internal sealed class VanillaCookingService : ICookingService
{
    public bool TryCook(EntityId chef, RecipeId recipe, out CookResult result) { /* ... */ }
}

// Registered at Initialize:
api.PublishContract<ICookingService>(new VanillaCookingService());
```

Use cases: pluggable behaviors where exactly one provider answers (cooking, smithing, lockpicking). Rule: the service interface is in a shared mod; the implementation is in a regular mod. A consumer fetches via `TryGetContract<ICookingService>(out var svc)` and gracefully degrades when `svc` is null.

### 6.3 Level 3: Protocol contracts

A new `IEvent` type defined by a mod, with publish/subscribe semantics. The event lives in a shared mod; publishers and subscribers are regular mods.

```csharp
// In a shared mod (dualfrontier.magic.protocol):
[Combat]
[Deferred]
public sealed record FireballCastEvent(EntityId Caster, GridVector Target, int ManaCost) : IEvent;

// Publisher (regular mod, dualfrontier.vanilla.magic):
api.Publish(new FireballCastEvent(caster, target, 25));

// Subscriber (another regular mod, dualfrontier.community.fireshield):
api.Subscribe<FireballCastEvent>(OnFireballCast);
```

Use cases: cross-mod gameplay protocols. A magic mod publishes spell events; defensive mods subscribe to add countermeasures.

### 6.4 Level matrix

| Level | Where the type lives | Where the implementation lives | How a consumer reaches it |
|---|---|---|---|
| Data | shared mod | publishing regular mod | `TryGetContract<T>` returns instance |
| Service | shared mod | publishing regular mod | `TryGetContract<T>` returns implementation |
| Protocol | shared mod | publisher and subscribers in regular mods | bus dispatch via `Publish` / `Subscribe` |

### 6.5 Anti-pattern: type in regular mod

A type used in any contract level **must** live in a shared mod. A `WeaponDef` defined in a regular mod's assembly is unreachable from another regular mod (different ALC, different `Type`). The loader rejects mods that declare contract types in non-shared assemblies.

> **✓ LOCKED (D-4).** The loader actively scans every regular-mod assembly via reflection for types implementing `IModContract` or `IEvent`. Detection rejects the mod with `ValidationErrorKind.ContractTypeInRegularMod`, naming the offending type and directing the author to the shared-mod pattern (§5). The cost (one reflection pass per load) is negligible compared to the architectural signal: contracts in regular mods break cross-mod interoperability silently, and silent breakage is what this architecture exists to prevent.

---

## 7. Bridge replacement ✓ LOCKED

Phase 5 systems (`CombatSystem`, `DamageSystem`, `ProjectileSystem`) currently exist in the kernel as `[BridgeImplementation(Phase = 5)]` stubs with empty `Update()` bodies. The bridge mechanism keeps the dependency graph valid for downstream phases that reference the system identity. When a vanilla mod ships with a real implementation, the bridge must step aside.

### 7.1 Mechanism: explicit `replaces` ✓ LOCKED

A mod listing a fully-qualified type name in `replaces`:

```json
"replaces": ["DualFrontier.Systems.Combat.CombatSystem"]
```

declares that **its** registered system supersedes the named kernel system. The loader, when applying the mod set:

1. Reads every `replaces` entry from every mod in the batch.
2. Builds the `replacedSystems` set.
3. When the kernel's bootstrap system list is being added to the dependency graph, every entry in `replacedSystems` is **skipped**. The bridge stays compiled, but never registered.
4. The mod's replacement system is registered in its place.

### 7.2 Conflict resolution

If two mods in the same load batch both list the same FQN in `replaces`, the loader rejects the batch with `ValidationError` of new kind `BridgeReplacementConflict`. The user is presented with the conflict in the mod menu and asked to disable one of the conflicting mods.

There is no automatic priority. Two combat mods cannot coexist if both replace `CombatSystem`. This is intentional: silent precedence is a debugging nightmare; explicit user choice is the architectural answer.

### 7.3 Rationale

Implicit replacement (silently letting mod systems shadow kernel systems) was rejected during Phase 0 deliberation because:

- A user investigating "why is combat acting strangely" must be able to discover, from manifests alone, which mod is responsible.
- A mod author adding combat mechanics deliberately (rather than accidentally) signals intent through `replaces`.
- The `ContractValidator` already detects write-write conflicts on components; explicit `replaces` extends the same philosophy ("conflicts are surfaced, never resolved silently") to system identity.

### 7.4 Bridge metadata

The existing `[BridgeImplementation(Phase = N)]` attribute is extended with a `Replaceable` flag:

```csharp
[BridgeImplementation(Phase = 5, Replaceable = true)]
public sealed class CombatSystem : SystemBase { /* ... */ }
```

A bridge with `Replaceable = false` cannot be replaced. The loader rejects mods that list it in `replaces`. This is the kernel's escape hatch for systems that must remain authoritative (e.g. `SystemExecutionContext` itself if it were a registered system).

### 7.5 Tests

The integration test set extends with:

- "Mod replaces a `Replaceable = true` bridge — bridge skipped, mod system runs."
- "Two mods replace same bridge — batch rejected with `BridgeReplacementConflict`."
- "Mod replaces `Replaceable = false` system — mod rejected with `ProtectedSystemReplacement`."
- "Mod replaces non-existent FQN — mod rejected with `UnknownSystemReplacement`."
- "Mod is unloaded — replacement skip is reverted, kernel bridge re-registers, dependency graph rebuilds."

---

## 8. Versioning ✓ LOCKED

Three independent SemVer axes govern the mod system. Each axis answers a distinct compatibility question.

### 8.1 Axis 1 — Kernel API version

The version of the `DualFrontier.Contracts` assembly. Already exists as `ContractsVersion.Current`; currently `1.0.0`. Bumped manually:

- **Major** — breaking change to `IModApi`, manifest schema, attribute set, or any public type signature in `Contracts`.
- **Minor** — additive change (new method on `IModApi` with a default fallback, new optional manifest field, new attribute).
- **Patch** — bug fix that does not touch the public surface.

The mod's manifest declares the required kernel API version in `apiVersion`. The loader uses `ContractsVersion.IsCompatible(required, ContractsVersion.Current)`. Caret prefix support is added (§8.4).

### 8.2 Axis 2 — Mod self version

The mod's own `version` field. Used for:

- Hot-reload lineage. When the menu reloads a mod, the loader compares the new manifest's `version` against the loaded one. A lower version triggers a warning ("you are downgrading"); equal versions are permitted (re-apply); higher versions proceed.
- User-visible identity in the mod menu.
- Save-game compatibility: each save records the set of `(modId, modVersion)` it used. Loading a save with a mod at a major version below what the save expects warns the user.

### 8.3 Axis 3 — Inter-mod dependency version

Each entry in `dependencies` is `{id, version}`, where `version` is a SemVer constraint. The loader verifies that for every dependency, the loaded mod with that id satisfies the constraint.

### 8.4 Constraint syntax: caret subset ✓ LOCKED

Three syntaxes are supported, all are subsets of npm/Cargo conventions:

- **Exact**: `"1.2.3"` — matches `1.2.3` only.
- **Caret**: `"^1.2.3"` — matches any version `>= 1.2.3` and `< 2.0.0`. The major number is pinned; minor and patch may be higher.
- **Tilde** (rejected): `"~1.2.3"` — explicitly **not supported** in v1. Reserved syntax; the parser rejects with a clear error message pointing to caret.

Range syntaxes (`">=1.0 <2.0"`) and OR (`"1.x || 2.x"`) are not supported. If a mod author needs a non-caret constraint, the design escalates to §12 for case-by-case handling.

### 8.5 Parser

`ContractsVersion.Parse` already handles strict `MAJOR.MINOR.PATCH` and silently strips a leading caret/tilde. The v2 parser:

1. Detects the prefix (`^`, exact, or `~`).
2. For `~`, throws `FormatException` with a directive to use caret instead.
3. Returns a new `VersionConstraint` struct: `{ ContractsVersion Version, ConstraintKind Kind }`.
4. `VersionConstraint.IsSatisfiedBy(ContractsVersion candidate)` evaluates per kind.

```csharp
public readonly struct VersionConstraint
{
    public ContractsVersion Version { get; }
    public ConstraintKind Kind { get; } // Exact, Caret

    public bool IsSatisfiedBy(ContractsVersion candidate) => Kind switch
    {
        ConstraintKind.Exact => candidate == Version,
        ConstraintKind.Caret =>
            candidate.Major == Version.Major
            && (candidate.Minor > Version.Minor
                || (candidate.Minor == Version.Minor && candidate.Patch >= Version.Patch)),
        _ => throw new InvalidOperationException()
    };
}
```

### 8.6 Where each axis applies

| Field | Axis | Constraint kinds allowed |
|---|---|---|
| `apiVersion` | 1 (Kernel API) | Exact or Caret |
| `version` | 2 (Mod self) | Exact only (it's a single value, not a constraint) |
| `dependencies[i].version` | 3 (Inter-mod) | Exact or Caret |

### 8.7 Resolution algorithm

Given a load batch (a list of mod manifests):

1. Build the dependency graph.
2. Topologically sort. A cycle is `ValidationError.CyclicDependency`.
3. In topological order, for each mod:
   a. Verify `apiVersion` against `ContractsVersion.Current`.
   b. For each dependency, verify the loaded version satisfies the constraint.
   c. If any check fails, the mod is added to the failed set; mods that depend on it cascade-fail.
4. The failed set is presented to the user; the success set proceeds to load.

There is no version-resolution backtracking ("find a combination that works"). The loader takes manifests at face value. A cycle or unsatisfied constraint is a user-resolvable error, not a solver problem.

---

## 9. Lifecycle

The mod lifecycle has six well-defined states. Transitions between states are atomic; failure mid-transition rolls back to the previous state.

### 9.1 States

```
        ┌──────────┐
        │ Disabled │  ← user toggled off in menu, or never enabled
        └────┬─────┘
             │ user enables
             ▼
        ┌──────────┐
        │ Pending  │  ← manifest read, not yet validated
        └────┬─────┘
             │ validate
             ▼
        ┌──────────┐
        │  Loaded  │  ← assembly in ALC, IMod.Initialize ran
        └────┬─────┘
             │ scheduler.Rebuild
             ▼
        ┌──────────┐
        │  Active  │  ← system is in the dependency graph, ticking
        └────┬─────┘
             │ user disables (or HotReload)
             ▼
        ┌──────────┐
        │ Stopping │  ← graph rebuild excludes this mod
        └────┬─────┘
             │ ALC.Unload
             ▼
        ┌──────────┐
        │ Disabled │
        └──────────┘
```

### 9.2 Hot reload through the menu ✓ LOCKED

Hot reload is supported only via the mod menu, with the simulation paused. The user flow:

1. User opens the mod menu. The menu calls `ModIntegrationPipeline.Pause()` which sets the scheduler's run flag to false.
2. User toggles mods, edits versions, clicks "Apply."
3. The menu invokes `ModIntegrationPipeline.Apply(newModSet)`. This call:
   - Builds the new graph in a local variable (existing behavior).
   - On success, calls `_scheduler.Rebuild(newPhases)`.
   - Calls `ALC.Unload()` on every mod in the previous set that is not in the new set.
4. The menu calls `ModIntegrationPipeline.Resume()` and the simulation continues from the current world state.

Reloading a mod (same id, possibly different version) follows the same flow: old version unloads, new version loads, replacement systems re-register if listed in `replaces`.

### 9.3 No live-tick reload ✓ LOCKED

The architecture explicitly forbids reloading a mod during a tick. Attempts to call `Apply` while the scheduler is running throw `InvalidOperationException("Pause the scheduler before applying mods")`. This is enforced by `ModIntegrationPipeline` checking the scheduler's run flag.

### 9.4 Save-game implications

A save records `(modId, modVersion)` for every active mod at the time of save. On load:

- If a recorded mod is missing → user warned, save loads with that mod absent (entities with components from that mod are removed; this is destructive but explicit).
- If a recorded mod is at a higher major version → user warned, save may behave incorrectly.
- If a recorded mod is at a lower or equal version → load proceeds.

The fine-grained handling of component data from missing mods is delegated to the persistence layer ([PERSISTENCE](../../src/DualFrontier.Persistence/README.md)) and is out of scope for this document.

### 9.5 ALC unload protocol

`AssemblyLoadContext.Unload()` is asynchronous; the runtime waits for all references to the assembly to be released. The unload chain:

1. `RestrictedModApi.UnsubscribeAll()` — drops bus subscriptions.
2. `IModContractStore.RevokeAll(modId)` — drops contract registrations.
3. `ModRegistry.RemoveSystems(modId)` — drops system instances.
3.5. **(К10.2)** `df_scheduler_unload_mod_native_state(modId)` — native primitive encapsulating T0-T7 internal sequence: clears native scheduler state (subscriber registries per tier, capability registrations, wake registry subscriptions, shared memory registrations); returns `ModUnloadResult` с per-tier metrics. Best-effort sequential per §9.5.1; native primitive internal critical section atomicity (per S3-Q1 L3 layering, single primitive contract per S3-Q6). К10.3 v2 Item 41 extends с pipeline slot quiescence check (К-L18 invariant) — primitive rejects unload if any pipeline slot is `Dispatched` или `FenceCompleted`. Also tears down per-mod `ModSubScheduler` (К10.2 Item 21).
3.6. **(К10.3 v2 Item 42)** V (Vulkan) resource cleanup — wraps `df_vulkan_unload_mod_resources(mod_id)` C ABI primitive per VULKAN_SUBSTRATE.md §3.4 К10.3 v2 amendment. К10.3 v2 lands the managed wrapper (`DualFrontier.Application.Bridge.VResourceCleanup`) returning vacuous success when no pipeline-managed mod resources are registered; full native implementation (`VkDestroyPipeline` / `VkFreeDescriptorSets` / `vkDestroyBuffer` / `vkDestroyImage` для mod-registered Vulkan handles) lands V-cycle или К-extensions per managed-facade-preserved strategy. К-L18 quiescent state precondition already satisfied (Step 3.5 verified sim paused + pipeline quiescent per К-L18 invariant). Best-effort sequential per §9.5.1.
4. The dependency graph is rebuilt without this mod's systems.
5. The scheduler swaps to the new phase list.
6. `ALC.Unload()` is called.
7. The loader spins on `WeakReference.IsAlive`, polling each iteration. Before every poll the loader performs `GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect()` — the double-collect bracket is required because `WaitForPendingFinalizers` can resurrect finalizable graph nodes the first collect would have removed; the second collect picks those up, restoring monotonic progress. Default cadence: 100 iterations × 100 ms = 10 s timeout. On timeout, a `ModUnloadTimeout` warning fires; the mod is marked as a leaked reference and the user is advised to restart.

WeakReference-based unload tests are mandatory for every regular mod (§10.4).

### 9.5.1 Failure semantics

Steps 1–6 (including К10.2 Step 3.5 — `df_scheduler_unload_mod_native_state` — и К10.3 v2 Step 3.6 — V resource cleanup placeholder) of the unload protocol (§9.5) are sequential and best-effort. If any step throws, the loader logs the exception with `(modId, stepNumber)`, surfaces a non-blocking `ValidationWarning`, and continues to the next step. After step 6, if step 7 times out, the `ModUnloadTimeout` warning per §9.5 fires; the mod is removed from the active set regardless of whether the assembly actually unloaded.

Step 3.5 may reject the entire unload if К-L18 quiescent state precondition is not satisfied (sim не paused, либо pipeline slots в `Dispatched`/`FenceCompleted` state). In that case the native primitive returns failure + error message; Steps 3.6 onward proceed best-effort consuming the same warning pipeline, но the upstream К-L18 violation surfaces as `QuiescentStatePreconditionViolated` (§11.2 К10.3 v2 amendment) carrying the rejection reason.

There is no atomic-unload guarantee. `Unload` is conceptually irreversible: subscriptions removed in step 1 cannot be re-attached without re-running `Subscribe`. The chain is structured so each step is a no-op if its predecessor failed (e.g. `RemoveSystems` on a mod with no registered systems is harmless), making best-effort progression safe. The `ModLoader.UnloadMod` swallowed `try/catch` around `mod.Instance.Unload()` is the canonical example of this discipline, in place since M0.

### 9.6 Hot-reload disabled mods

A mod with `"hotReload": false` cannot be reloaded mid-session. The menu disables the reload button for that mod and presents a tooltip. To change such a mod, the user must restart the game.

### 9.7 Hot reload К-L18 compliance (К10.3 v2 amendment)

Hot reload preserves game state through managed dependency graph swap (§9.2/§9.5). К-L18 «mod lifecycle quiescent state» mandates the simulation be paused и pipeline slots quiescent before unload/reload operations proceed:

- `DualFrontier.Application.Loop.SimulationStateController.PauseAsync()` sets the К-L18 sim-paused flag (consumed by native `df_scheduler_unload_mod_native_state` Step 3.5 precondition check per К10.2 + К10.3 v2 Item 41 extension).
- `WaitForQuiescenceAsync(timeout)` polls pipeline slot state (К10.3 v2 Item 33 — all slots `Empty` или `ReadableAsTail`) until quiescent или timeout. Timeout surfaces as `PipelineQuiescenceTimeout` validation error (§11.2 К10.3 v2 amendment).
- `ResumeAsync()` clears the sim-paused flag после the mod operation completes.

Mod management UI и hot reload tooling share this enforcement pattern. Per S-LOCK-12 К10.3 v2 scope: helpers only land; full settings menu / preferences UI deferred к V-cycle или К-extensions.

---

## 10. Threat model

The mod system is not a sandbox. A mod runs in-process with full .NET access. Isolation is **structural and architectural**, not security-grade. The threat model documents what the architecture catches and what it does not.

### 10.1 Architectural threats: caught

| Threat | Mechanism that catches it |
|---|---|
| Mod reaches for component storage outside its `[SystemAccess]` declaration | Structural: post-К8.3+К8.4 there is no managed component-access surface to misuse — systems reach storage only through `NativeWorld`, and `[SystemAccess]` declarations drive scheduler edge-building (`DependencyGraph`). The former runtime guard (`SystemExecutionContext` throwing on undeclared access) was deleted at К8.3+К8.4; analyzer-grade detection is Planned — see [docs/ROADMAP.md](../ROADMAP.md) §Analyzer track. |
| Mod system misbehaves at runtime (isolation breach reported as a fault) | `ModLoader.HandleModFault(modId, ModIsolationException)` → `ModFaultHandler.ReportFault` (the `IModFaultSink` implementation); the mod is queued and unloaded via the §9.5 chain at the next menu open (`ModIntegrationPipeline.Apply` drains the faulted set) — the core does not crash. |
| Mod calls `GetSystem<T>()` directly | Surface removed at К8.3+К8.4 — no `GetSystem` exists on any mod-reachable type. |
| Mod casts `IModApi` to `RestrictedModApi` | Structural unreachability (§4.4, D-3): `internal sealed`, internal construction, `DualFrontier.Application` not resolvable from a mod ALC. `ModIsolationException` is the fault type carried through `ModLoader.HandleModFault` when an isolation breach is reported. |
| Mod registers a system that conflicts with another mod's system | `ContractValidator` write-write check (Phase B) |
| Mod replaces a system also replaced by another mod | `BridgeReplacementConflict` (Phase H) |
| Mod requires a capability not provided by kernel or dependencies | `MissingCapability` (Phase C) |
| Mod publishes or subscribes without the declared capability | `CapabilityViolationException` from `RestrictedModApi.EnforceCapability` (empty-capability manifests short-circuit with a warning — §3.6 case 3) |

### 10.2 Architectural threats: not caught

These are explicitly out of scope and documented:

- **Mod calls `Process.Kill(0)` or `Environment.Exit(1)`.** The .NET runtime gives mods full process access. We do not sandbox via AppDomain (deprecated in .NET 8) or process isolation (would break the in-process performance assumptions).
- **Mod opens network sockets, reads arbitrary files, executes shell commands.** Same reason.
- **Mod consumes unbounded memory or CPU.** Performance budgets are advisory, not enforced.
- **Mod mutates `IComponent` instances obtained via `GetComponent` after the call returns.** Component records are returned by reference for performance. A mod ignoring the [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md) immutability convention can corrupt state. This is caught at code review, not at runtime.
- **Mod uses reflection to access internal types.** A mod that calls `Type.GetType("DualFrontier.Core.ECS.World")` and casts to it bypasses the contract surface. The `ALC.Resolving` event refuses to load `DualFrontier.Core` into the mod's context, but a determined mod can still find loaded instances via static lookups in shared types. This is the cost of in-process execution.

### 10.3 The contract: best-effort structural isolation

Dual Frontier's mod system promises:

- A well-behaved mod (one not deliberately attempting to subvert the architecture) cannot accidentally crash the engine, corrupt state, or break other mods.
- A misbehaving mod can be detected, named, and unloaded with high probability.
- A malicious mod can break the game; the user accepts this risk by installing the mod.

This is the same contract operating systems offer to processes, scaled down: ring 3 is enforced; ring 0 is reachable through deliberate effort.

### 10.4 Required tests

The mod system test set (`tests/DualFrontier.Modding.Tests/`) covers:

- **Capability tests.** `KernelCapabilityRegistryTests` + `CapabilityValidationTests` (`Capability/`) — registry token emission and the violation path (`CapabilityViolationException` on undeclared publish/subscribe).
- **Bridge replacement tests.** `PhaseHBridgeReplacementTests` (`Validator/`) — the §7.5 scenarios.
- **Type-sharing tests.** `CrossAlcTypeIdentityTests` (`Sharing/`) — a type loaded through the shared ALC resolves to the same `Type` instance across regular-mod ALCs.
- **WeakReference unload tests.** Closed at M7.3: `M73Step7Tests` + `M73Phase2DebtTests` (`Pipeline/`) assert `WeakReference.IsAlive == false` within the timeout for real-mod fixtures, with `ModUnloadAssertions` mirroring the production GC-pump spin pattern (§9.5 step 7) for reuse by later fixtures. The historical Phase 2 backlog item this closed is recorded in docs/ROADMAP.md (M7 row).
- **Cross-mod cycle tests.** `RegularModTopologicalSortTests` (`Pipeline/`) — a load batch with `A → B → A` is rejected with `CyclicDependency`.
- **Version constraint tests.** `PhaseGInterModVersionTests` (`Validator/`) — caret satisfaction and major-mismatch rejection per §8.

The isolation-guard runtime suite of the Phase 2 era was retired together with the runtime guard at К8.3+К8.4 (§10.1 row 1); isolation is exercised structurally through the loader/ALC suites above.

---

## 11. Migration plan

### 11.1 Migration state

Migration state authority is **[docs/ROADMAP.md](../ROADMAP.md)** — the Mod-OS Migration M-rows (M0–M10, including the deferred M3.x analyzer milestones) and the «Native foundation tracks» section. This document does not track which milestone is open, closed, or deferred; it specifies the architecture those milestones implement. The standing sequencing rule the migration runs under — the engine remains buildable and the test suite green at every step — continues to bind any remaining M-row work.

### 11.2 `ValidationErrorKind` registry

`ValidationErrorKind` (`src/DualFrontier.Application/Modding/ValidationError.cs`) has fifteen members. Each entry names the milestone that introduced it:

- `IncompatibleContractsVersion` (baseline) — `RequiresContractsVersion`/`apiVersion` not satisfiable by the current build (`ContractValidator` Phase A); the strict-v3 manifest gate reuses this semantic in its parse-time rejection message (§2.3).
- `WriteWriteConflict` (baseline) — two systems declare writes to the same component type (Phase B).
- `CyclicDependency` (baseline) — dependency cycle in the mod graph (§8.7).
- `MissingDependency` (baseline) — required mod id absent from the load set; optional dependencies downgrade to a warning (§2.2).
- `IncompatibleVersion` (M5) — `apiVersion` or `dependencies[i].version` constraint not satisfied (Phases A+G).
- `MissingCapability` (M3) — a `capabilities.required` token provided by neither the kernel nor a listed dependency (Phase C).
- `SharedModWithEntryPoint` (M4) — `kind: "shared"` with a non-empty entry point or an `IMod` implementation (Phase F).
- `ContractTypeInRegularMod` (M4) — a regular mod exports `IModContract`/`IEvent` types (Phase E, D-4).
- `BridgeReplacementConflict` (M6) — two mods in one batch replace the same FQN (Phase H, §7.2).
- `ProtectedSystemReplacement` (M6) — `replaces` names a system not marked `Replaceable = true` (§7.4).
- `UnknownSystemReplacement` (M6) — `replaces` names an FQN found in no loaded assembly.
- `MissingManagedStorageAttribute` (К8.3+К8.4, K-L3.1) — `RegisterManagedComponent<T>` called for a class without `[ManagedStorage]`; caught at registration time before the per-mod `ManagedStore<T>` is created.
- `FastTierContractViolation` (К10.2) — Fast tier subscriber violates the bounded-execution contract (К-L15 ≤1ms); runtime monitoring via `FastTierContractMonitor`.
- `BusTierMismatch` (К10.2) — manifest declares a tier-specific capability while the event type's `[EventTier]` names a different tier (§3.2).
- `BackgroundCoalesceMissing` (К10.2) — Background tier event type missing its coalesce-function declaration (§3.2).

Not every violation in the mod system is a `ValidationErrorKind`. Runtime capability misses at `Publish`/`Subscribe` and at field acquisition surface as `CapabilityViolationException` (§3.6 runtime layer, §4.6.2); field operation failures surface as `FieldOperationFailedException` (`DualFrontier.Core.Interop`).

**Documented-but-reserved names.** The К10.3 v2 amendments (chronicled at v1.10/v1.11) reserved `QuiescentStatePreconditionViolated` (§9.5.1), `PipelineQuiescenceTimeout` (§9.7), `LayerCapabilityMismatch` (§3.2), and `VulkanModResourceCleanupFailed` (the §9.5 step 3.6 cleanup primitive's failure kind); the K9/compute era reserved `FieldRegistrationConflict`, `InvalidFieldDimensions`, `FieldCapabilityMismatch`, `ComputePipelineCompilationFailed`, `ComputePipelineRegistrationConflict`, and `ComputeUnsupportedWarning`. None of these is a `ValidationErrorKind` member today. `PipelineQuiescenceTimeout` exists as `PipelineQuiescenceTimeoutException` (`DualFrontier.Application.Loop.SimulationStateController`); the others have no on-disk artifact — their enum entries land with the implementations that need them (compute substrate and К-L17/К-L18 surfacing work — see [docs/ROADMAP.md](../ROADMAP.md) §V substrate / §Native foundation tracks).

### 11.3 Stop conditions

Mod-system work halts and escalates to the human if:

- The cost of the capability cross-check (§3.7) exceeds 5 seconds per mod load.
- WeakReference unload tests are flaky (any failure rate above 0%).
- A mod author successfully bypasses capability enforcement using documented .NET features.

Each stop is a Phase 0 re-entry: the architecture is amended in this document before code resumes. (Renumbered from §11.4 at v1.12.0; pre-v1.12.0 change-history entries reference these conditions as §11.4.)

---

## 12. Locked decisions

These items were unresolved in v0.1 and were locked during Phase 0 closure (v1.0). Each is referenced from the section that introduced it. The full Question/Options/Locked-resolution structure is preserved verbatim — Options are kept for traceability, so future re-opens of any decision can read the alternatives that were considered.

### D-1. `read`/`write` capability scope

**Question.** Does a mod's `kernel.read:<Component>` capability apply to *every* public component in `DualFrontier.Components`, or only to components opted in via `[ModAccessible(Read = true)]`?

**Options.**
- **a) Blanket.** Every public component is reachable; capability is a reservation, not a gate. Simpler manifest semantics, broader access surface.
- **b) Curated, opt-in.** Only components annotated `[ModAccessible(Read = true, Write = false)]` can be requested by mods. Tighter security, requires every component author to opt in.
- **c) Curated, opt-out.** Every public component is accessible *except* those marked `[ModRestricted]`. Middle ground.

**✓ LOCKED.** (b) Curated, opt-in via `[ModAccessible(Read = true, Write = false)]`. Aligns with the project's structural-isolation philosophy. A component author actively decides what mods can touch; everything else is unreachable.

**Blocking phase.** M3 (unblocked).

### D-2. `[SystemAccess]` ↔ capability cross-check enforcement

**Question.** How does the loader verify that a registered mod system's `[SystemAccess]` declarations are a subset of the mod's manifest capabilities?

**Options.**
- **a) Static analysis at load time.** Roslyn-based scan of every `Services.X.Publish<T>` call site in the mod assembly. Heavyweight (multi-second on large mods), authoritative, no drift.
- **b) Per-system attribute.** `[ModCapabilities("publish:DamageEvent")]` on each system, manually maintained by the mod author. Lightweight, drift-prone.
- **c) Hybrid.** Attribute at load time, static analysis only as a CI check before publication.

**✓ LOCKED.** (c) Hybrid. Per-system `[ModCapabilities(...)]` attribute checked at load time; CI static analysis verifies the attribute matches actual `Publish`/`Subscribe` call sites in the assembly. Load-time stays fast; CI catches drift before release.

**Blocking phase.** M3 (unblocked).

### D-3. Cast-prevention enforcement

**Question.** How is the rule "a mod cannot cast `IModApi` to a concrete type" enforced?

**Options.**
- **a) Roslyn analyzer at load time.** Scans for `(RestrictedModApi)api` and similar patterns. Slow, requires shipping the analyzer.
- **b) Runtime check on first use.** Cheaper but allows the cast to compile and run before being detected.
- **c) Make `RestrictedModApi` `internal sealed` and rely on the type being unreachable from a mod's ALC.** This is structurally true today.

**✓ LOCKED.** (c) Structural barrier only in v1. `RestrictedModApi` is `internal sealed`; its containing assembly (`DualFrontier.Application`) is not resolvable from a mod's ALC. The cast cannot compile against the kernel's actual type. (a) is held in reserve for v1.x if a real bypass attempt is observed.

**Blocking phase.** M2 (unblocked).

### D-4. Loader scan for `IModContract`/`IEvent` in regular mods

**Question.** Does the loader actively scan a regular mod's exported types for `IModContract` or `IEvent` implementations and reject the mod if found?

**Options.**
- **a) Active scan, reject.** Strong signal to mod authors that contracts belong in shared mods.
- **b) Passive: warn but load.** Mods with mistakes still work, with a runtime warning.
- **c) Documentation-only.** Rely on review and code style.

**✓ LOCKED.** (a) Active scan and reject. The reflection cost is negligible; the architectural signal — that contract types belong in shared mods, period — is large enough to justify load-time enforcement.

**Blocking phase.** M4 (unblocked).

### D-5. Shared-mod cycle detection

**Question.** Are cycles in the shared-mod dependency graph forbidden? The architecture in §1.4 says yes, but enforcement requires the loader to refuse such configurations explicitly.

**✓ LOCKED.** Forbidden. The loader rejects shared-mod cycles with `ValidationErrorKind.CyclicDependency`. Cycles break compilation order at the mod author's IDE before reaching the loader, but explicit runtime rejection is cheaper than implicit failure further downstream.

**Blocking phase.** M4 (unblocked).

### D-6. Save-game compatibility policy when a mod is missing

**Question.** When loading a save that recorded a mod no longer present, what happens to entity components owned by that mod?

**Options.**
- **a) Strip components, keep entities.** The entity continues to exist with reduced state.
- **b) Strip entities entirely.** The world loses everything tied to the absent mod.
- **c) Refuse to load.** Save is bound to its mod set; missing mods make it unloadable.

**✓ LOCKED.** (a) Default: strip components owned by the missing mod, keep entities; load with a clear warning naming each missing mod. (c) Available as a user-toggle "strict mod compatibility" setting; when enabled, the save refuses to load. Workshop reality is that mods get abandoned, and the community patches around them — strict-by-default would block too many real save loads.

**Blocking phase.** Out of M0–M10 scope. Tracked here for the persistence rework that will implement the component-stripping logic.

### D-7. `hotReload: false` semantics for vanilla mods

**Question.** Vanilla mods are the engine team's reference implementation. Does it make sense to allow them to be hot-reloaded? A hot-reload of `Vanilla.Combat` changes core combat math during a session.

**Options.**
- **a) Vanilla mods are hot-reloadable like any other.** Maximum testability of the mod system.
- **b) Vanilla mods set `hotReload: false` by default.** Stable session experience.

**✓ LOCKED.** Hybrid by build flavor. Vanilla mods declare `hotReload: true` in source; the shipping build pipeline rewrites this to `hotReload: false` in release manifests. Development gets free hot-reload testing of vanilla mechanics; shipped builds get stable session experience. Override is a single flag in the build script — no code branching needed.

**Blocking phase.** M7 (unblocked).

---

## See also

- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — the development pipeline; this architecture is the artifact of the same methodology applied to the engine's modding layer.
- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers; mods live above Domain through `IModApi`.
- [MODDING](./MODDING.md) — the mod-author guide («Writing mods»).
- [MOD_PIPELINE](./MOD_PIPELINE.md) — the pipeline implementation notes; extended through M2–M7.
- [CONTRACTS](./CONTRACTS.md) — bus and marker conventions; capability syntax mirrors bus naming.
- [ISOLATION](./ISOLATION.md) — `SystemExecutionContext` and the structural isolation rules.
- [ROADMAP](../ROADMAP.md) — migration state authority; the Mod-OS M-rows track what is closed, pending, or deferred (§11.1).
- `src/DualFrontier.Contracts/Modding/` — the source-of-truth surface for `IMod`, `IModApi`, `IModContract`, `ContractsVersion`, `ModManifest`.
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — field-based GPU compute as a foundational architectural capability. Per К-L9 mod parity, vanilla and third-party mods register fields — and, once the compute surface lands, pipelines — through the same `IModApi` (mod-driven shader registration).

## Modding с native ECS kernel

The kernel is native (К-series; see `KERNEL_ARCHITECTURE.md`). Consequences for modding:
- Mod component types are either Path α (`unmanaged struct`, kernel-side `NativeWorld` storage) or Path β (managed `class` annotated with `[ManagedStorage]`, mod-side `ManagedStore<T>` storage). Path α is default; Path β is per-component opt-in for shapes where unmanaged conversion forces structural compromise (managed-only references, lazy state graphs, complex object graphs not blittable). See `KERNEL_ARCHITECTURE.md` Part 0 K-L3 implication post-K-L3.1 for the decision criterion.
- Path α registers via `IModApi.RegisterComponent<T> where T : unmanaged, IComponent`. Path β registers via `IModApi.RegisterManagedComponent<T> where T : class, IComponent` (Mod API v3 surface, shipped at the К8.3+К8.4 cutover, 2026-05-14). Both registration entries are uniform across vanilla and third-party mods (K-L9 «vanilla = mods» preserved).
- Cross-mod managed-path direct access is structurally impossible by `AssemblyLoadContext` isolation: a regular mod's ALC cannot resolve types from another regular mod's ALC, so `Vanilla.Combat` cannot reference `Vanilla.Pawn.JobQueueComponent` at compile time. Cross-mod data flow uses event/intent contracts per §6 three-level contracts (publish/subscribe via shared protocol mods).
- Within-mod cross-path access (one system reads native + managed components on same entity) is supported via dual `SystemBase` API: `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α; `SystemBase.ManagedStore<T>()` for Path β. The accessor resolves to the system's owning mod's per-mod store via `SystemMetadata.ModId` (K6.1 plumbing). Performance characteristics visible per-call: native-path is zero-allocation span iteration; managed-path is Dictionary-shaped lookup.
- Mod replacement triggers second-graph rebuild (managed) — native side untouched. Per-mod `ManagedStore<T>` instances reclaim deterministically with the mod's `AssemblyLoadContext.Unload` per §9.5 unload chain.
- Vanilla mods register components, systems, fields, and compute pipelines through the same IModApi as third-party (vanilla = mods principle preserved per K-L9).
- Mod fields (`RawTileField<T>`) register through `api.Fields` (§4.6.1 — including its current production nullability); `api.ComputePipelines` is the reserved compute entry point, a placeholder until the V substrate lands its implementation. See [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) (mod-driven shader registration).
- `[ModAccessible]` annotation and capability strings (`kernel.read:`, `mod.<id>.read:`) function uniformly across Path α and Path β (Q6.a path-blind capability lock per K-L3.1). The capability resolver dispatches internally to `NativeWorld` span access or `ManagedStore` lookup per-T.

See `KERNEL_ARCHITECTURE.md` §1.9 (mod system registration) и §1.10 (component type registry) для full detail. See [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) (mod parity, К-L9) для GPU compute mod-parity invariant.