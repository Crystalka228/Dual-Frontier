---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-KERNEL
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "2.5"
next_review_due: 2027-05-21
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-KERNEL
---
# DualFrontier Kernel — Architecture & Roadmap

**Version**: 2.5 (A'.8 К-series formal closure — 8 К-L LOCK batch + К-L14 abbreviated cross-reference)
**Date**: 2026-05-23
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3); K8.2 v2 closure of K-L3 selective per-component application via K8.1 primitives (v1.4, header bump deferred to v1.5); K-L3.1 bridge formalization — Path β (managed-class, mod-side storage) as first-class peer to Path α (`unmanaged struct`, kernel-side NativeWorld) per session 2026-05-10 (v1.5); A'.5 K8.3+K8.4 combined milestone closure 2026-05-14 — managed `World` retired from production as `ManagedTestWorld` (test-project fixture only); Power subsystem deleted (electricity deferred to a separate GPU-compute brief); 10 production systems migrated to NativeWorld AcquireSpan/BeginBatch; isolation enforced at compile time (runtime guard removed) (v1.6); K10.1 closure 2026-05-18 — K-L6 «Game tick scheduler: Managed» SUPERSEDED by K-L12 «Full native kernel scheduling»; K-L13 «On-demand system activation (5 wake types)» and K-L14 «Performance derives from architectural cleanliness» AUTHORED. Native scheduler graph + wake registry + scheduling policies + write-through filter + batched callback ABI + observability + intrinsics landed (v2.0); K10.2 closure 2026-05-18 — K-L15 «Native bus authority + three-tier event dispatch» AUTHORED (v2.1); V0.B closure 2026-05-18 — K-L19 «Hardware tier commitment» (Vulkan 1.3 + async compute queue family mandate) LOCKED with full implementation backing (HardwareCapabilityCheck.Verify fail-fast at Runtime.Create; AMD RX 7600S verified К-L19 hardware baseline on test machine) (v2.2); **К10.3 v2 load-bearing commit 1/3 2026-05-20 — K-L7.1 «GPU compute pipeline slot binding» (sub-invariant к K-L7, opt-in coexistence с V1 sync default per S-LOCK-10) + K-L16 «Simulation tick pipeline depth» (D=1-3 configurable, default 2; governs pipeline-managed dispatches per S-LOCK-13) AUTHORED. Native pipeline_slot state machine + Phase.Compute scheduler integration + drain/refill protocols + filter primitive integration с slot transitions landed**; **К10.3 v2 load-bearing commit 2/3 2026-05-20 — K-L17 «Display composition multi-layer» (sim state + intent overlay + combat feedback layers с independent latency contracts; composition order: sim state first, overlays composited on top; lives в `src/DualFrontier.Application/Display/` per S-LOCK-11) AUTHORED. Application/Display/ composition framework + IntentOverlayLayer + CombatFeedbackLayer + Contracts/Display/LayerAttribute + KernelCapabilityRegistry layer-token scan landed**; **К10.3 v2 load-bearing commit 3/3 2026-05-20 — K-L18 «Mod lifecycle quiescent state» (sim paused + pipeline slots quiescent precondition для mod load/unload; UI helper layer enforces precondition per S-LOCK-12 helpers-only scope) AUTHORED. Native mod_unload primitive extended с pipeline quiescence check (К10.3 v2 Item 41); managed SimulationStateController + Step 3.6 V resource cleanup placeholder (К10.3 v2 Item 42). MOD_OS_ARCHITECTURE.md v1.10 amended: §9.5 chain extended 8-step → 9-step (Step 3.6 V cleanup placeholder), §9.7 «Hot reload К-L18 compliance» subsection added, §11.2 ValidationErrorKind enum extended с 4 К-L17/L18 entries (QuiescentStatePreconditionViolated, PipelineQuiescenceTimeout, LayerCapabilityMismatch, VulkanModResourceCleanupFailed). VULKAN_SUBSTRATE.md v1.1 final amendment: §3.4 df_vulkan_unload_mod_resources C ABI placeholder. К10.3 v2 cascade complete: 4 К-L invariants AUTHORED (К-L7.1 sub-invariant + К-L16/L17/L18); cumulative К-Lxx series total 20 invariants post-К10.3 v2 (К-L1..L11 + К-L6 SUPERSEDED + К-L3.1/L7.1 subs + К-L12..L19).**; **A'.7.x BUS_ARCHITECTURE_AMENDMENT closure 2026-05-21 — К-extensions cascade #0 (chronologically pre-A'.8 closure, architecturally K-extension). К-L15.1 «Three-tier independence» (2-layer sub-invariant к К-L15: per-tier state isolation — FastTierState/NormalTierState/BackgroundTierState с separate mutex + sequence counter + subscriber map; runtime layer — tier-bit-disambiguated subscription IDs + fixed-order df_bus_clear acquisition + cross-tier re-entrant safe publish) AUTHORED → LOCKED within the cascade per К-L7.1 precedent (sub-invariant LOCKS while parent stays AUTHORED candidate until A'.8). Cumulative К-Lxx series 21 invariants post-A'.7.x. Source split (bus_native.cpp → 4-file bus_fast/normal/background/common.cpp) deferred к A'.7.5 sub-milestone per Q-N-7X-2 Option C — К-L15.1 в 2-layer form does not require compile-time isolation as a 3rd layer. +45% bus throughput verified empirically; S10 cross-tier re-entrancy probe PASS; Bug #1 (BusFacade.Publish coalesce_key drop) + Bug #2 (df_background_queue_dispatch_idle_slot orphan in src/) + Bug #3 (O(N²) coalesce) ALL closed. К10.3 v2 verification #7 for К-L14 thesis annotated soft-halted (14 Modding "fails" later identified as transient fixture-copy build state, not regression) + retroactively closed by A'.7.x = К-L14 verification #8 clean cascade.**; **A'.8 К-series formal closure 2026-05-23 — 8 К-L candidates LOCKED batch (К-L7.1 + К-L12-L18) per Q-N-8-1 LOCKED Session 2 Day 2. Cumulative К-Lxx series 21 invariants finalized (К-L15.1 sub-invariant already LOCKED А'.7.x preserved; К-L20 reserved post-Mod API lock excluded from main count). К-L14 thesis canonical text resides in K_CLOSURE_REPORT.md §1.2 per Q-N-8-2 LOCKED (hybrid Q2 (c) policy — KERNEL_ARCHITECTURE.md keeps abbreviated row + cross-reference; K_CLOSURE_REPORT.md §1.2 carries canonical text). К-L14 evidence baseline at closure: 9 verifications (1 soft-halt annotated honestly, retroactively closed). К-extensions designation operationalized (cascades #0 А'.7.x + #1 А'.7.5 closed pre-closure architecturally as К-extensions; #2 Godot removal deferred к post-closure per Q-N-8-6 LOCKED). К-closure event boundary = K_CLOSURE_REPORT.md AUTHORED ratification commit (per Q-N-8-4 LOCKED amendment к Meta-Q1 Session 1 — initial AUTHORED, not LOCKED; LOCKED transition deferred к downstream review). K_CLOSURE_REPORT.md authored Tier 1 **AUTHORED** Category A reference document.**
**Companion documents**: `METHODOLOGY.md`, `CODING_STANDARDS.md`, `MOD_OS_ARCHITECTURE.md`, `VULKAN_SUBSTRATE.md`
**Scope**: Full architectural specification + milestone roadmap для native ECS kernel (C++ via pure P/Invoke). Companion к `VULKAN_SUBSTRATE.md` (Vulkan rendering layer) — together describing complete native foundation under managed Application layer.

---

## Executive summary

DualFrontier ECS kernel migrates от managed C# к pure C++ via P/Invoke. Domain layer abstractions (Components, Events, Systems) preserved verbatim в managed; only storage и primitive operations move к native. All systems remain managed (because all systems are mods loaded via AssemblyLoadContext — vanilla mods и third-party mods uniformly).

**Foundation philosophy** — «без компромиссов»:
- Pure P/Invoke к `DualFrontier.Core.Native.dll` (no third-party C# binding library, mirrors VULKAN_SUBSTRATE.md L2)
- BCL only for managed bridge (`System.Runtime.InteropServices`, `System.Numerics`)
- Manual memory management в C++ (std::vector + std::unordered_map only, no third-party libs)
- Component storage: Path α (`unmanaged struct`, kernel-side NativeWorld) default; Path β (managed `class` via `[ManagedStorage]`, mod-side store) per opt-in (K-L3.1 bridge formalization, 2026-05-10)
- Two-phase model: native bootstrap → managed game tick

**Source of truth для existing experimental work**: `claude/cpp-core-experiment-cEsyH` branch + `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` (Discovery report). 11 substantive C++ commits + 1637 LOC delta + clean self-test passing.

**Estimated scope**: 5-8 weeks at hobby pace (~1h/day) для full kernel completion (K0-K8) + **1-2 weeks** для К9 (field storage abstraction).

**Status snapshot** (live, обновляется по closure milestone): K0–K8.2 v2 closed (cumulative `547c919..7527d00`, 2026-05-07 through 2026-05-09); K-L3.1 bridge formalization recorded 2026-05-10; A'.4 K9 closed (RawTileField field storage + IModApi v3 Fields wiring) 2026-05-10; A'.5 K8.3+K8.4 combined closed 2026-05-14 (commits `24e5f56..54c6658` — managed World retired, Power subsystem deleted, 10 production systems on NativeWorld). См. `MIGRATION_PROGRESS.md` для current state.

**Combined с VULKAN_SUBSTRATE.md (M9.0-M9.8) + VULKAN_SUBSTRATE.md (К9 + G0–G9)**: **15-25 weeks** для full architectural foundation. См. `ROADMAP.md` "Native foundation tracks" section для master sequencing.

---

## Part 0: LOCKED foundational decisions

The following decisions are committed как architectural foundation. Departures require explicit re-architecture milestone, not spec-level adjustments mid-implementation.

| # | Decision | Choice | Rationale |
|---|---|---|---|
| K-L1 | Native language | C++20, MSVC/GCC/Clang | Compiled native, modern features, no third-party deps |
| K-L2 | Bindings | Pure P/Invoke к `DualFrontier.Core.Native.dll` | Zero third-party C# в production binary (mirrors L2) |
| K-L3 | Component storage paths | Path α (`unmanaged struct`, NativeWorld) default; Path β (managed `class` via `[ManagedStorage]`, mod-side store) per opt-in | Two first-class peer paths; per-component author choice based on architectural fit; native-path retains blittable-layout invariant, managed-path is mod-private and runtime-only |
| K-L4 | Type IDs | Explicit registry per-mod registration | FNV-1a hash collision-prone; explicit IDs deterministic |
| K-L5 | Bootstrap orchestration | Declarative graph, native, parallel where deps allow | Symmetric к runtime second graph; explicit dependencies |
| K-L6 | Game tick scheduler | **SUPERSEDED by K-L12** (see KERNEL_FULL_NATIVE_SCHEDULER.md) | Original rationale «Vanilla = mods» preserved as K-L9; execution layer concern factored out to K-L12 at K10.1 closure (2026-05-18) |
| K-L7 (LOCKED К0) + К-L7.1 (LOCKED A'.8) | Span protocol (К-L7) + GPU compute pipeline slot binding (К-L7.1 opt-in coexistence) | Read-only spans + write command batching (К-L7); GPU compute writes к RawTileField storage bound к pipeline slot (К-L7.1) — sim-thread reads of pipeline-managed fields see slot-tail state (sim tick T+D reads dispatched-at-(T+D-1) state). One-tick lag bounded и deterministic. К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots. | Mutation semantics explicit; race conditions structurally impossible (К-L7); К-L7.1 sub-invariant К10.3 v2 — mirrors К-L3.1 precedent pattern (opt-in per field per К-L9 «Vanilla = mods» author choice; V1 К-L7 sync default preserved для existing FieldHandle consumers); **К-L7.1 LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L8 | Component lifetime | Native owns storage, managed holds opaque `IntPtr` | Single ownership boundary; managed holds handle only |
| K-L9 | Mod parity | Vanilla mods register components и systems through same IModApi as third-party | «Vanilla = mods» enforced at architecture level |
| K-L10 | Decision rule | §8 metrics (GC pause / p99 / long-run drift on weak hardware) | §6 «20% mean speed» superseded; §8 captures actual project value |
| K-L11 | Production storage backbone | NativeWorld single source of truth (Solution A); ManagedWorld retained as test fixture and research artifact only | K7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + «no compromises» commitment; single ownership boundary, single mental model |
| K-L12 (LOCKED A'.8) | Native kernel scheduling | Sovereign per-tick scheduling for kernel-space systems (Core) native; managed scheduler scope reduced к user-space (mod) systems within mod ALCs. Kernel scheduling decisions are made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement. Cross-layer communication uses C ABI with batched callbacks. | OS-faithful kernel/user split; K-L6 supersession; cross-layer ABI bridge per K10.1 Item 15 (managed adapter receives batched managed-system dispatches from kernel scheduler) — K10 deliberation arc closure 2026-05-17, K10.1 execution closure 2026-05-18; **LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L13 (LOCKED A'.8) | On-demand system activation | Five wake types (Timer / Event / StateChange / Init / Explicit); only runnable systems enter phase dispatch; per-tick dependency graph computed on runnable subset | Real-OS process-blocking model; sparse-world efficiency; cache locality improvement — K10 deliberation arc closure 2026-05-17, K10.1 execution closure 2026-05-18; **LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L14 | Performance derives from cleanness (LOCKED A'.8) | **Abbreviated form** (per Q-N-8-2 LOCKED hybrid Q2 (c)): Architectural completeness causes performance; tactical heuristics inapplicable в research framework; default-inclusion bias for architectural items. **Canonical text** (full thesis + Causality + Empirical + Falsifiability + Default-inclusion bias + Burden of proof sub-clauses): see [K_CLOSURE_REPORT.md §1.2](K_CLOSURE_REPORT.md#12--к-l14-canonical-text-q-n-8-2-locked-2026-05-23-verbatim) | Project framing (Crystalka 2026-05-16); decade-horizon rent of compromise; Linux/seL4 empirical evidence — K10 deliberation arc closure 2026-05-17, K10.1 execution closure 2026-05-18; LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch + Q-N-8-2 canonical text adoption |
| K-L15 (LOCKED A'.8 parent) + К-L15.1 (LOCKED А'.7.x sub; compile-time layer materialized А'.7.5) | Native bus authority + three-tier event dispatch (К-L15) + Three-tier independence (К-L15.1) | К-L15: Native kernel owns sovereign event routing for kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority. Bus supports three-tier dispatch (fast / normal / background) with tier declared per event type. Managed bus facade preserved as IModApi-exposed surface (K-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations. Cross-layer event delivery uses batched callback ABI (normal/background tiers) or immediate synchronous callback (fast tier). **К-L15.1 (2-layer sub-invariant, А'.7.x К-extensions cascade #0, LOCKED 2026-05-21):** каждый tier owns architectural isolation at two structural layers — (1) State layer: per-tier state struct (FastTierState/NormalTierState/BackgroundTierState) с separate std::mutex, separate next_seq counter, separate subscriber map, separate pending queue where applicable; no shared mutable state between tiers. (2) Runtime layer: subscription ID space uses high 8 bits = tier identifier + low 56 bits = per-tier sequential counter (cross-tier collisions structurally impossible); df_bus_unsubscribe dispatches via tier-bit; df_bus_clear acquires three tier mutexes in fixed fast→normal→background order для deadlock safety. Cross-tier publish semantics: Fast subscriber callback MAY publish events к any tier — re-entrant safe through mutex isolation (post-А'.7.x); pre-А'.7.x single shared mutex prevented re-entrant publish и was a structural deadlock hazard. Falsifiability: К-L15.1 falsified if subsequent cascade demonstrates tier dependence at state level (shared mutex reintroduced), tier dependence at runtime level (shared subscription ID counter, tier-bit disambiguation broken), or cross-tier re-entrancy deadlock (S10 probe regresses). K-L15 invariants preserved: single DLL (К-L2 unchanged), single C ABI surface (bus_native.h 16 functions unchanged), native bus authority, three-tier dispatch semantics. | OS-faithful event routing; cross-layer delivery via batched callback ABI; K-L9 «Vanilla = mods» preserved through facade (К-L15) — K10 deliberation arc closure 2026-05-17, K10.2 execution closure 2026-05-18; **К-L15.1 sub-invariant А'.7.x К-extensions cascade #0** — mirrors К-L7.1 sub-invariant precedent (К-L15.1 LOCKED at A'.7.x closure; К-L15 parent LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch). Empirical evidence: per-tier state split + per-tier mutex closed 48-way contention observed at 15M events / 16 producers × 3 tiers stress (Bug #4); cross-tier re-entrancy hazard closed (S10 cross-tier re-entrancy probe в SchedulerExtremeTests.cs:1007 PASS post-split, was deadlock-prone pre-split); O(N²) → O(N) background coalesce closed Bug #3 (5M pending events → multi-minute hang pre-fix → ~14 ms 1000 events post-fix linearly); BusFacade.Publish<T>(T, uint coalesceKey) overload closed Bug #1 (managed boundary tier validation); ManagedBusBridge.DrainBackgroundBatch + GameLoop tick-end integration closed Bug #2 (df_background_queue_dispatch_idle_slot had 0 src/ call sites). +45% bus throughput verified, S10 PASS ≤ 100ms, F5 verification clean. Source split (bus_native.cpp → 4-file bus_fast/normal/background/common.cpp) deferred к A'.7.5 sub-milestone (separate cascade, no К-L impact) per Q-N-7X-2 Option C hybrid — К-L15.1 в 2-layer form does not require compile-time isolation as a 3rd layer. |
| K-L16 (LOCKED A'.8) | Simulation tick pipeline depth | D ≥ 1 (configurable 1-3, default 2). Simulation thread runs D ticks ahead of display thread for pipeline-managed dispatches. Cross-layer async operations (GPU compute pipeline-managed, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread. Display thread reads results from simulation tick (CurrentSimTick - D). Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume. К-L16 establishes display latency invariant (D × tick_period); К-L15 fast tier latency invariant (subscriber response ≤1ms) preserved independently. | К10 deliberation arc closure 2026-05-17, К10.3 v2 execution closure 2026-05-20 (S-LOCK-13 coexistence: governs pipeline-managed dispatches; V1 К-L7 sync дисpatch_compute_field path orthogonal и preserved для existing consumers per К-L9 «Vanilla = mods» author choice per field); **LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L17 (LOCKED A'.8) | Display composition multi-layer | Display tick T composites multi-layer state where layers carry independent latency contracts: SimState layer (К-L16 pipeline tail for pipeline-managed, либо current sim state for К-L7 sync) → Intent overlay (current input state, ≤16ms latency) → CombatFeedback layer (К-L15 Fast tier consumers, ≤17ms event-к-visible) → Static layer (loaded assets). Composition order: SimState rendered first, intent + combat overlays composited on top, static last. Framework lives в `src/DualFrontier.Application/Display/` (Application layer above Rendering/IRenderer abstraction per S-LOCK-11; renderer interfaces preserved unchanged). Mod-registered layers declare via `[Layer(LayerType.Intent | CombatFeedback)]` attribute (Contracts/Display) + capability tokens `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}` (granular per FQN per tier per S3-Q5 + S8-Q3 pattern). Per К-L9 «Vanilla = mods», vanilla layers register through same attribute + capability pattern. | К10 deliberation arc closure 2026-05-17, К10.3 v2 execution closure 2026-05-20 (S-LOCK-11 location: Application/Display/ above renderer; S-LOCK-12 К-L18 UI hookup deferred к К-L18 load-bearing commit); **LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L18 (LOCKED A'.8) | Mod lifecycle quiescent state | Mod load/unload operations require simulation paused state + pipeline slots quiescent (all fences completed; no concurrent compute dispatches in-flight). Precondition enforced at native unload primitive (К10.2 sim-paused stub + К10.3 v2 Item 41 pipeline quiescence check via `df_pipeline_is_quiescent`). UI helper layer (`DualFrontier.Application.Loop.SimulationStateController` + existing `ModMenuController` pause hook) provides programmatic API: `PauseAsync` → `WaitForQuiescenceAsync(timeout)` → mod operation → `ResumeAsync`. К10.3 v2 §9.5 unload chain extended 8-step → 9-step с Step 3.6 V (Vulkan) resource cleanup placeholder (`df_vulkan_unload_mod_resources` C ABI primitive; К10.3 v2 lands managed wrapper returning vacuous success; full implementation V-cycle / К-extensions). Per S-LOCK-12 helpers-only scope: full settings menu / preferences UI deferred к V-cycle / К-extensions. | К10 deliberation arc closure 2026-05-17, К10.3 v2 execution closure 2026-05-20 (S-LOCK-12 helpers-only; settings menu deferred); **LOCKED at A'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch** |
| K-L19 | Hardware tier commitment | Vulkan 1.3 + async compute queue family mandate. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20-series и newer), AMD RDNA 1+ (Radeon RX 5500 и newer), Intel Arc Alchemist+ (Arc A380 и newer). Async compute queue family used для compute-side pipeline depth dispatches (К-L16 К10.3 v2); graphics queue for display rendering; copy/transfer queue for asset transfers. Кernel mandates queue family availability at startup; failure to detect async compute queue is fail-fast condition с user-facing diagnostic message pointing к README hardware requirements section. Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation. By Dual Frontier release timeline, target hardware tier represents majority of gaming hardware. | К10 deliberation arc closure 2026-05-17; landed V0.B closure 2026-05-18 — full implementation backing operational (async compute queue family selection in VulkanDevice; HardwareCapabilityCheck.Verify in Runtime.Create; AMD RX 7600S verified hardware baseline; К-L17/L18 numbering reserved для subsequent К10.3 v2 load-bearing commits) |

К10.3 v2 cascade complete 2026-05-20: К-L7.1 (sub-invariant), К-L16, К-L17, К-L18 all AUTHORED (LOCKED at А'.8 closure 2026-05-23 per Q-N-8-1 LOCK batch). К-L16 landed K10.3 v2 load-bearing commit 1/3 (К-L7.1 + К-L16 grouped per Approach C — pipeline depth + sub-invariant share physical reality «GPU pipeline slots»). К-L17 landed K10.3 v2 load-bearing commit 2/3 (display composition framework + intent/combat feedback layer infrastructure per Items 38-40). К-L18 landed K10.3 v2 load-bearing commit 3/3 (mod lifecycle quiescent state precondition + UI helper layer wiring per Items 41-42; settings menu deferred per S-LOCK-12). К-L19 landed V0.B per cross-stream prerequisite resolution. Cumulative К-Lxx series total post-К10.3 v2: 20 invariants (К-L1..L11 + К-L6 SUPERSEDED + К-L3.1/L7.1 subs + К-L12..L19). А'.7.x К-extensions cascade #0 (2026-05-21) added К-L15.1 sub-invariant LOCKED 2-layer (state + runtime) — cumulative 21 invariants post-А'.7.x. А'.7.5 К-extensions cascade #1 (2026-05-22) added К-L15.1 compile-time layer materialization (3-layer manifestation complete; К-Lxx series stays 21 invariants — no LOCK transition at А'.7.5). А'.8 К-series formal closure (2026-05-23) LOCKED 8 К-L candidates (К-L7.1 + К-L12-L18) per Q-N-8-1 LOCK batch; К-L20 reserved post-Mod API lock. К-L14 canonical text adopted verbatim per Q-N-8-2 LOCKED — resides at [K_CLOSURE_REPORT.md §1.2](K_CLOSURE_REPORT.md#12--к-l14-canonical-text-q-n-8-2-locked-2026-05-23-verbatim) per Q2 hybrid (c) policy (KERNEL keeps abbreviated row + cross-reference). К-L14 evidence baseline at A'.8 closure: 9 verifications (8 clean + 1 honest soft-halt annotation verification #7 per Q-N-8-3 LOCKED). К-extensions designation operationalized per Q-N-8-10 LOCKED (cascade #0 + #1 closed; cascade #2 Godot removal deferred к post-closure).

**Implication of K-L3 (post-K-L3.1, 2026-05-10)**: Components are first-class via either Path α (`unmanaged struct`, kernel-side `NativeWorld` storage) or Path β (managed `class`, mod-side per-mod ManagedStore). Path α is the default — author silence + struct shape implies native registration via existing `IModApi.RegisterComponent<T> where T : IComponent`. Path β is per-component opt-in via `[ManagedStorage]` attribute on a `class : IComponent` type, registered through `IModApi.RegisterManagedComponent<T> where T : class, IComponent` (Mod API v3 surface, ships at K8.4 closure). Decision criterion is per-component architectural fit: Path α applies when conversion to `unmanaged struct` is justified (performance, locality, blittable layout, K8.1 primitive coverage); Path β applies when conversion forces structural compromise (managed-only references not expressible as K8.1 primitives, lazy state graphs, runtime-only computed handles, complex object graphs not blittable).

Path β components are runtime-only (Q4.b lock) — not persisted by save system; managed-storage lives per-mod (mod assembly's `RestrictedModApi` instance), reclaimed deterministically on `AssemblyLoadContext.Unload` per `MOD_OS_ARCHITECTURE.md` §9.5 unload chain. Cross-mod managed-path direct access is structurally impossible by ALC isolation; cross-mod data flow uses event/intent contracts per `MOD_OS_ARCHITECTURE.md` §6 three-level contracts. Within-mod cross-path access (one system reads native + managed components on same entity) is supported via dual `SystemBase` API (Q3.i lock): `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α, `SystemBase.ManagedStore<T>()` for Path β; performance characteristics are visible per-call (no opaque dispatch).

**K8.2 v2 closure (`MIGRATION_PROGRESS.md` K8.2 v2 entry, 2026-05-09)** delivered the Path α kernel-side completion: K8.1 wrapper value-type refactor (`NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` from `sealed unsafe class` to `readonly unsafe struct`; `InternedString.IComparable`; `NativeWorld.Allocate*Id` counters + `Create*` factory methods); 6 class→struct conversions using K8.1 primitives (Identity/Workbench/Faction via InternedString, Skills via NativeMap×2, Storage via NativeMap+NativeSet, Movement via NativeComposite+`HasTarget`+`PathStepIndex`); 6 empty TODO stub deletions per METHODOLOGY §7.1 «data exists or it doesn't» (Combat: Ammo/Shield/Weapon; Magic: School; Pawn: Social; World: Biome). The 6 deletions reflect §7.1 application — empty placeholder components removed because their data did not exist; this is selective per-component judgment (delete per §7.1, convert per K8.1 primitives, leave verify-only struct annotations on already-struct), not universal Path α mandate.

**K4's "Hybrid Path" framing superseded by K-L3.1**: post-amendment, both paths are first-class peers, not «α default plus β tolerated as exception». Author choice is recorded explicitly via `[ManagedStorage]` opt-in; absence implies Path α. The K8.2 v2 closure framing «K-L3 «без exception» state achieved» is corrected — closure delivered selective per-component application of K-L3 to `src/DualFrontier.Components/`, not universal mandate. Capability model is path-orthogonal (Q6.a lock) — `[ModAccessible]` attribute and capability strings (`kernel.read:` / `mod.<id>.read:`) function uniformly across paths; the resolver dispatches internally to NativeWorld span access or ManagedStore lookup per-T.

**Performance contract (Q5.a lock)**: native-path publishes specific guarantees (zero-allocation reads via `SpanLease<T>`, structure-of-arrays layout, batched writes via `WriteBatch<T>`). Managed-path provides Dictionary-shaped lookup with no zero-allocation guarantee. The contract difference is visible per-call via dual `SystemBase` API (Q3.i). Performance metrics are tagged per-path in `KernelCapabilityRegistry` (Q5.a passive); active analyzer enforcement (Q5.b) deferred to post-migration analyzer milestone per Crystalka 2026-05-10 «после миграции нужен будет анализатор... но это потом».

**Implication of K-L6 (pre-K10.1, retained для historical context)**: K-L6's «no native game-tick scheduler» framing held через K0..K9 and the K8.3+K8.4 cutover. Post-K10.1 closure (2026-05-18), K-L6 is SUPERSEDED by K-L12 — native scheduler now owns sovereign per-tick scheduling decisions (dependency graph, runqueue, wake dispatch, phase composition, parallelism, priority arbitration, resource quotas). Managed adapter facade preserves external behavior unchanged from observer perspective; Core system execution bodies remain managed (К11+ optional native migration deferred per K10.1 Item 14).

**Implication of K-L9**: No vanilla privilege. Vanilla.{Core,Combat,Magic,Inventory,Pawn,World} mods register components и systems via same API as third-party would.

**Implication of K-L8 in production**: Post-K8.4 closure, NativeWorld is the only production storage path. World class retained as test fixture and research reference (per K-L11). Production code constructs NativeWorld via Bootstrap two-phase model; no production code path constructs World directly.

**Implication of K-L11**: All production storage is NativeWorld. After K8.4 closure, no production code path constructs `World` directly. `World` class is retained for tests and research reference only. K8.1-K8.5 sub-milestones execute the migration; see Part 2 §K8.

---

## Part 1: Architecture

### 1.1 — Project structure (post-K8 target)

````
src/
  // ====== Domain layer (preserved verbatim — zero touch) ======
  DualFrontier.Core/                          # managed wrappers, World facade
  DualFrontier.Contracts/                     # IComponent, EntityId, etc.
  DualFrontier.Components/                    # vanilla components: Path α (struct, default) or Path β (class via [ManagedStorage], per K-L3.1)
  DualFrontier.Events/                        # event types
  DualFrontier.Systems/                       # system implementations (managed)
  DualFrontier.Application/                   # bootstrap, scheduler, coordinator
  DualFrontier.Modding/                       # mod loader, IModApi
  DualFrontier.Persistence/                   # save/load

  // ====== Bridge layer (от branch + extended) ======
  DualFrontier.Core.Interop/
    DualFrontier.Core.Interop.csproj
    MODULE.md
    Native/
      NativeMethods.cs                        # P/Invoke declarations
      NativeMethods.Bootstrap.cs              # bootstrap-specific
      NativeMethods.Storage.cs                # storage primitives
      NativeMethods.Span.cs                   # span acquisition/release
      NativeMethods.Batch.cs                  # write command flushing
    NativeWorld.cs                            # managed handle wrapper
    Marshalling/
      EntityIdPacking.cs                      # ulong ↔ EntityId
      ComponentTypeRegistry.cs                # explicit type-id registry
      WriteCommandBuffer.cs                   # mod mutation accumulator
      SpanLease.cs                            # IDisposable span lifetime

  // ====== Runtime stack ======
  DualFrontier.Runtime/                       # see VULKAN_SUBSTRATE.md
  DualFrontier.Presentation/                  # see VULKAN_SUBSTRATE.md

native/
  DualFrontier.Core.Native/
    CMakeLists.txt
    build.md
    include/
      df_capi.h                               # public C ABI (extended от branch)
      entity_id.h                             # EntityId POD
      sparse_set.h                            # template (or removed if unused — K0.2)
      component_store.h                       # type-erased store
      world.h                                 # World declaration
      bootstrap_graph.h                       # NEW K3: startup task graph
      thread_pool.h                           # NEW K3: native thread pool
      write_command_buffer.h                  # NEW K5: mutation batch
    src/
      world.cpp
      capi.cpp                                # extended С span/batch endpoints
      bootstrap_graph.cpp                     # NEW K3
      thread_pool.cpp                         # NEW K3
      write_command_buffer.cpp                # NEW K5
    test/
      selftest.cpp                            # extended с new scenarios

tests/
  DualFrontier.Tests/                         # existing 472 tests (struct refactor in K7)
  DualFrontier.Core.Interop.Tests/            # NEW K2: bridge equivalence tests
  DualFrontier.Core.Benchmarks/               # extended с tick-loop benchmark

docs/
  METHODOLOGY.md
  CODING_STANDARDS.md
  MOD_OS_ARCHITECTURE.md
  VULKAN_SUBSTRATE.md                     # Vulkan layer companion
  KERNEL_ARCHITECTURE.md                      # THIS DOCUMENT
  CPP_KERNEL_BRANCH_REPORT.md                 # Discovery report (input to К0)
  NATIVE_CORE.md                              # superseded by this doc, retained для history
  NATIVE_CORE_EXPERIMENT.md                   # superseded, retained
````

### 1.2 — Module purposes

#### `DualFrontier.Core.Native` (C++)

**Purpose**: ECS kernel storage + bootstrap orchestration + thread pool. Knows nothing of game domain. Could be open-sourced separately as «sparse-set ECS in C++ с C ABI».

**Public API surface**: 12 baseline functions (from branch) + ~6-8 new functions для span access, batch flush, bootstrap graph, type registration. Total ~18-20 extern «C» functions.

**Dependencies**: C++20 stdlib only (`<vector>`, `<unordered_map>`, `<memory>`, `<thread>`, `<mutex>`, `<atomic>`, `<chrono>`, `<cstring>`).

#### `DualFrontier.Core.Interop` (managed bridge)

**Purpose**: P/Invoke declarations + managed handle wrapper + marshalling helpers. Bridge между managed Application и native kernel.

**Public API surface**: `NativeWorld`, `WriteCommandBuffer`, `SpanLease<T>`, `ComponentTypeRegistry`. All marked `internal` к Application или `public` for mod authors.

**Dependencies**: BCL only (`System.Runtime.InteropServices`, `System.Numerics`).

#### `DualFrontier.Application` (managed orchestrator)

**Purpose**: Bootstrap orchestrates two-phase model (Native → Managed). PhaseCoordinator drives game tick. Bridge между Domain и Native via Interop.

**Public API surface**: `Bootstrap.Bootstrap()`, `IGameServices`, `PhaseCoordinator`, `ManagedScheduler`.

**Dependencies**: `DualFrontier.Core`, `DualFrontier.Core.Interop`, `DualFrontier.Modding`, `DualFrontier.Events`.

### 1.3 — Two-phase model

#### Phase A: Native bootstrap (one-time, native scheduler + native graph)

````
Application startup → df_engine_bootstrap() → returns world handle

Internally (native side):
  1. Build bootstrap dependency graph (declarative — kStartupTasks array)
  2. Topological sort (Kahn's algorithm)
  3. Spawn worker threads (std::thread × N cores)
  4. Execute tasks in parallel where graph permits

Example tasks (full inventory in §1.5 of K3 brief):
  - AllocateMemoryPools (no deps)
  - InitWorldStructure (deps: AllocateMemoryPools)
  - InitThreadPool (deps: AllocateMemoryPools) — parallel с InitWorldStructure
  - RegisterBuiltinTypes (deps: InitWorldStructure)
  - ValidateConfiguration (deps: RegisterBuiltinTypes)
  - SignalEngineReady (deps: all above)

Returns: opaque IntPtr к World handle.
Failure mode: returns IntPtr.Zero → managed throws BootstrapFailedException.
````

«Без конфликтов» = startup phase has no mod loading concurrency, no ECS tick races. Free parallelization.

#### Phase B: Managed bootstrap (mods + second graph)

````
Application receives world handle → loads mods → builds second graph

Steps:
  1. ModLoader.LoadAll(modsDirectory)
     - AssemblyLoadContext per mod
     - Vanilla mods + third-party mods uniformly
  2. ModRegistry collects:
     - Component type registrations → calls df_world_register_component_type
     - System registrations (with phase + dependencies)
  3. SystemGraph.Build(modRegistry.Systems) (Kahn topological sort)
  4. PhaseCoordinator.Initialize(worldHandle, systemGraph)
  5. Engine ready для game ticks

Mod replacement (occasional):
  - Pause tick loop
  - AssemblyLoadContext.Unload(modContext)
  - Reload mod assemblies
  - Re-register component types и systems
  - REBUILD second graph
  - Resume tick loop

Native side untouched throughout mod replacement.
Native graph stays static (startup-only, never rebuilds).
````

#### Phase C: Game tick (per-frame, managed-driven)

````
PhaseCoordinator.RunTick():
  for phase in [Input, NeedDecay, JobAssign, Movement, JobExec, Cleanup, Reporting]:

    systemsForPhase = secondGraph.GetSystemsForPhase(phase)

    for system in systemsForPhase (parallel where graph permits):

      // STEP 1: acquire spans (single P/Invoke per type)
      using var lease = nativeWorld.AcquireSpans(system.ReadTypes);

      // STEP 2: managed system execution (zero P/Invokes during)
      var writeBuffer = new WriteCommandBuffer();
      system.Update(lease.Spans, writeBuffer);

      // STEP 3: flush writes (single P/Invoke)
      nativeWorld.FlushWrites(writeBuffer);

    // Per-system flush ↑, или per-phase flush at end (К6 design choice)

Crossings per tick estimate:
  ~7 phases × ~5-10 systems × 2-3 P/Invokes = ~150-200 crossings/tick
  At 30 TPS: ~4500-6000 crossings/sec
  Each ~10-50 ns: ~50-300 μs/sec total overhead = negligible
````

### 1.4 — Threading model

````
┌──────────────────────────────────────────────────────────────┐
│ Native side (during bootstrap):                              │
│  - Thread pool (std::thread × N cores)                       │
│  - Worker threads execute startup tasks parallel             │
│  - Threads idle after bootstrap complete (но pool persists)  │
│                                                               │
│ Native side (during game runtime):                           │
│  - Thread pool idle (managed scheduler doesn't use it)       │
│  - Storage operations serialized (single-threaded contract)  │
│  - Span acquisition uses atomic counter for safety           │
└──────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────┐
│ Managed side (during bootstrap):                             │
│  - Main thread orchestrates                                  │
│                                                               │
│ Managed side (during game runtime):                          │
│  - Simulation Thread (existing GameLoop, 30 TPS)             │
│  - Worker threads (existing ParallelSystemScheduler)         │
│  - Window/Render Thread (VULKAN_SUBSTRATE.md)            │
│                                                               │
│ Communication:                                                │
│  - PresentationBridge (existing, domain → render)            │
│  - WriteCommandBuffer (NEW K5, system → native)              │
│  - SpanLease (NEW K5, native → system read access)           │
└──────────────────────────────────────────────────────────────┘
````

**Storage thread-safety contract**: Native World single-threaded для writes. Managed scheduler ensures only one phase active at a time → no concurrent native writes. Reads (spans) safe concurrent through atomic active-spans counter.

### 1.5 — Dependency rules (LOCKED invariants)

**Rule 1**: `DualFrontier.Core.Native` (C++) MUST not include any project-specific business logic. Pure ECS storage + bootstrap. Could compile standalone.

**Rule 2**: `DualFrontier.Core.Interop` MUST be only managed code calling `DualFrontier.Core.Native.dll`. Other projects MUST go через Interop, not direct P/Invoke.

**Rule 3**: `DualFrontier.Application` orchestrates через Interop. Other Domain projects (Core, Components, Events, Systems) don't know about Native existence — they see only `IGameContext`.

**Rule 4**: Mods reach native через `IGameContext` API surface (managed). No mod has direct P/Invoke к `DualFrontier.Core.Native.dll`.

**Rule 5**: Native side has no callbacks к managed. Direction unidirectional (managed → native always).

### 1.6 — Native interop patterns

**P/Invoke declarations** (Interop layer):

````csharp
namespace DualFrontier.Core.Interop.Native;

internal static partial class NativeMethods
{
    private const string DllName = "DualFrontier.Core.Native";

    // From branch (12 functions):
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_world_create();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_destroy(IntPtr world);

    // ... 10 more from branch ...

    // K1 NEW (batching):
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_add_components_bulk(
        IntPtr world, ulong* entities, uint typeId,
        void* componentData, int componentSize, int count);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_acquire_span(
        IntPtr world, uint typeId, void** outDensePtr,
        int** outIndicesPtr, int* outCount);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void df_world_release_span(IntPtr world, uint typeId);

    // K3 NEW (bootstrap):
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr df_engine_bootstrap();

    // K5 NEW (write batching):
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe void df_world_flush_write_batch(
        IntPtr world, void* commandBuffer, int byteSize);
}
````

**Style**: classic `[DllImport]` (matches branch convention). К consider migrating к `[LibraryImport]` (source-generated) at K7+ if profiling demands. Не blocking for K0-K2.

### 1.7 — Span<T> protocol

**Acquisition flow**:

````csharp
// Managed side acquires span:
using var lease = nativeWorld.AcquireSpan<HealthComponent>();
// Internally:
//   1. P/Invoke к df_world_acquire_span(world, HealthComponentTypeId, &ptr, &indices, &count)
//   2. Native side: increments active_spans_ counter
//   3. Native side: returns pointer к dense bytes + indices array + count
//   4. Managed wraps в SpanLease<T> (IDisposable)

// Iteration (zero P/Invokes):
foreach (ref readonly var health in lease.Span)
{
    // read-only access
    int currentHp = health.Current;
}

// Or paired access:
for (int i = 0; i < lease.Count; i++)
{
    EntityId entity = new EntityId(lease.Indices[i], 0); // version not exposed via span
    ref readonly HealthComponent h = ref lease.Span[i];
}

// Release on Dispose:
//   5. P/Invoke к df_world_release_span(world, HealthComponentTypeId)
//   6. Native side: decrements active_spans_ counter
````

**Native side invariant**:

````cpp
class World {
    std::atomic<int> active_spans_{0};

    void* acquire_span(uint32_t type_id, ...) {
        active_spans_.fetch_add(1, std::memory_order_acquire);
        // ... return pointer ...
    }

    void release_span(uint32_t type_id) {
        active_spans_.fetch_sub(1, std::memory_order_release);
    }

    void add_component(...) {
        if (active_spans_.load(std::memory_order_acquire) > 0) {
            throw std::logic_error("Cannot mutate during active span");
        }
        // ... actual add ...
    }
};
````

**Mutation safety**: any attempt к Add/Remove/Modify during active span throws. Caller must release all spans before mutating. Write command buffer accumulates mutations during span lifetime; flushes after release.

**Read concurrency**: multiple spans from different component types can be active simultaneously. Single-type span re-acquisition while one already active also safe (counter increments). Native side allows multiple concurrent readers.

### 1.8 — Write command batching

````csharp
public sealed class WriteCommandBuffer
{
    // Accumulates mutations during managed system execution

    public void Add<T>(EntityId entity, T component) where T : unmanaged;
    public void Remove<T>(EntityId entity) where T : unmanaged;
    public void Destroy(EntityId entity);
    public void Spawn(out EntityId entity);  // queues spawn, ID assigned at flush

    // Internal: serialized к single byte buffer for P/Invoke
    internal Span<byte> SerializeToBuffer();
}
````

**Flush mechanism**:
````csharp
public sealed class NativeWorld
{
    public unsafe void FlushWrites(WriteCommandBuffer buffer)
    {
        ReadOnlySpan<byte> data = buffer.SerializeToBuffer();
        fixed (byte* p = data)
        {
            NativeMethods.df_world_flush_write_batch(_handle, p, data.Length);
        }
    }
}
````

**Native side parses** command buffer, applies operations in order. Single P/Invoke transmits entire batch. Mutations applied только after all spans released for affected types.

**Buffer reuse**: `WriteCommandBuffer` pooled per phase к avoid allocation. Phase coordinator owns buffer pool.

### 1.9 — Mod system registration

````csharp
namespace DualFrontier.Modding;

public interface IModApi  // v3 — extends v2
{
    void RegisterComponentType<T>() where T : unmanaged;
    void RegisterSystem<T>(SystemRegistration registration) where T : ISystem, new();
    // ... other мode hooks ...
}

public sealed record SystemRegistration(
    Phase Phase,                          // which phase к run в
    IReadOnlyList<Type> ReadComponents,   // components system reads
    IReadOnlyList<Type> WriteComponents,  // components system writes
    IReadOnlyList<string> Dependencies,   // ordered after these system IDs
    string SystemId);                     // unique identifier для graph node
````

**Vanilla mod example** (Vanilla.Pawn):
````csharp
public class VanillaPawnMod : ModBase
{
    public override void RegisterComponentTypes(IModApi api)
    {
        api.RegisterComponentType<HealthComponent>();   // unmanaged struct (post-K7)
        api.RegisterComponentType<PositionComponent>();
        api.RegisterComponentType<NeedsComponent>();
        // ...
    }

    public override void RegisterSystems(IModApi api)
    {
        api.RegisterSystem<NeedsSystem>(new SystemRegistration(
            Phase: Phase.NeedDecay,
            ReadComponents: new[] { typeof(HealthComponent), typeof(NeedsComponent) },
            WriteComponents: new[] { typeof(NeedsComponent) },
            Dependencies: Array.Empty<string>(),
            SystemId: "vanilla.pawn.needs_system"));
        // ... more systems ...
    }
}
````

**Third-party mod example**: identical pattern. No vanilla privilege.

### 1.10 — Component type registry

````csharp
namespace DualFrontier.Core.Interop.Marshalling;

public sealed class ComponentTypeRegistry
{
    private readonly Dictionary<Type, uint> _typeToId = new();
    private uint _nextId = 1;  // 0 reserved for invalid

    public uint Register<T>() where T : unmanaged
    {
        if (_typeToId.TryGetValue(typeof(T), out uint existing))
            return existing;  // idempotent

        uint id = _nextId++;
        _typeToId[typeof(T)] = id;

        unsafe
        {
            NativeMethods.df_world_register_component_type(
                _worldHandle, id, sizeof(T));
        }

        return id;
    }

    public uint GetId<T>() where T : unmanaged
    {
        if (!_typeToId.TryGetValue(typeof(T), out uint id))
            throw new InvalidOperationException($"Component {typeof(T).Name} not registered");
        return id;
    }
}
````

**Sequential IDs** (1, 2, 3, ...) deterministic per mod load order. Auditable. No collision possibility (vs FNV-1a hash from branch).

**Mod load order matters** для type ID stability across runs. ModLoader должен process mods deterministically (alphabetical или explicit ordering manifest).

### 1.11 — Testing strategy

**Existing 472 tests preserved**. Component struct refactor (K7) updates field access patterns но behavior preserved.

**New tests**:

`DualFrontier.Core.Interop.Tests` (K2 NEW):
- `NativeWorldTests` — equivalence с managed `World` (CRUD round-trip)
- `EntityIdPackingTests` — bit-level pack/unpack invariants
- `ComponentTypeRegistryTests` — registration, GetId, idempotency
- `SpanLeaseTests` — acquisition/release/mutation-rejection invariants
- `WriteCommandBufferTests` — serialization correctness, flush semantics
- ~30-40 tests

Native `selftest.cpp` (K0/K1/K3 EXTEND):
- Existing 4 scenarios preserved
- K1 add: bulk add benchmark, span acquisition
- K3 add: bootstrap graph topological sort
- K5 add: write command buffer parsing
- ~10-12 scenarios total

`DualFrontier.Core.Benchmarks` (K1 EXTEND):
- Existing `NativeVsManagedBenchmark` preserved
- K1 add: `NativeBulkAddBenchmark` (validates batching speedup hypothesis)
- K3 add: `BootstrapTimeBenchmark` (parallel vs sequential startup)
- K7 add: `TickLoopBenchmark` (full simulation tick на representative load)
- K10 add: `LongRunDriftBenchmark` (10k+ tick run, p99 + GC + memory)

**Goal post-K8**: ~520 + ~60 = ~580 total tests, plus extended native selftest, plus extended benchmarks.

### 1.12 — Naming conventions

Continued от existing `CODING_STANDARDS.md`:
- All identifiers English
- C++ namespace: `dualfrontier`, lowercase
- C ABI prefix: `df_*` (matches branch)
- C++ class names: PascalCase
- C struct names: `df_*` snake_case (matches POD convention)
- C# wrapper classes: PascalCase (`NativeWorld`, `WriteCommandBuffer`)
- P/Invoke methods: keep C names (`df_world_create`) — no idiomatic translation

Mirrors VULKAN_SUBSTRATE.md §1.9 для cross-document consistency.

---

## Part 2: Roadmap (K-series)

### Master plan

| Milestone | Title | Estimated time | LOC delta |
|---|---|---|---|
| K0 | Cherry-pick + cleanup от branch | 1-2 days | +1637 (existing) + cleanup |
| K1 | Batching primitive (bulk Add/Get + Span<T>) | 3-5 days | +500-800 |
| K2 | Type-id registry + bridge tests | 2-3 days | +400-600 |
| K3 | Native bootstrap graph + thread pool | 5-7 days | +600-900 |
| K4 | Component struct refactor (Path α) | 2-3 weeks | +/- (mostly conversion) |
| K5 | Span<T> protocol + write command batching | 1 week | +500-700 |
| K6 | Second-graph rebuild on mod change | 3-5 days | +200-400 |
| K7 | Performance measurement (tick-loop) | 3-5 days | +200-400 |
| K8.0 | Architectural decision recording (Solution A) | 1-2 days | +/- (docs only) |
| K8.1 | Native-side reference handling primitives | 1-2 weeks | +600-1000 |
| K8.2 | K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation) + 6 class→struct conversions on Path α via K8.1 primitives (Identity/Workbench/Faction/Skills/Storage/Movement) + 6 empty TODO stub deletions per METHODOLOGY §7.1 (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series, authored on appropriate path per K-L3.1) + 12 ModAccessible annotation completeness pass on already-struct components | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
| K8.3 | **CLOSED 2026-05-14** (combined w/ K8.4 in A'.5): 10 vanilla systems migrated to SpanLease/WriteBatch (Power 2 deleted as disposable CPU systems — electricity moves to GPU compute) | combined w/ K8.4 | actual -4481/+1211 |
| K8.4 | **CLOSED 2026-05-14** (combined w/ K8.3 in A'.5): ManagedWorld retired from production as `ManagedTestWorld` (test-project fixture); Mod API v3 ships (Path β bridge: `RegisterManagedComponent<T>` + `ManagedStore<T>`); compile-time isolation enforcement (runtime guard removed) | combined w/ K8.3 | (see K8.3 row) |
| K8.5 | Mod ecosystem migration prep | 3-5 days | +500 (docs) |
| K9 | Field storage abstraction (`RawTileField<T>`) | 1-2 weeks | +600-900 |
| K10 | Native kernel scheduler (К-L6 SUPERSEDED + К-L12/L13/L14 AUTHORED in K10.1; K-L15 AUTHORED in K10.2; К-L19 AUTHORED V0.B inheritance; К-L7.1/L16/L17/L18 AUTHORED in K10.3 v2) | 4 sub-milestones (K10.1 = kernel scheduler core CLOSED 2026-05-18; K10.2 = native bus + mod ALC lifecycle CLOSED 2026-05-18; K10.3 v2 = pipeline + display composition + quiescent CLOSED 2026-05-20; K10.4 = TLA+ formal verification pending) | K10.1 ~16 commits +3000-4000 (kernel core); K10.2 ~14 commits +4000-5000 (native bus + ALC); К10.3 v2 ~14 commits (pipeline + display + quiescent); К10.4 future brief |

**Cumulative K0-K8**: 5-8 weeks at hobby pace.
**Cumulative K0-K9**: 6-10 weeks at hobby pace (K9 prerequisite for V substrate primitives per Q-G-2 LOCK).
**Cumulative K0-K10.1**: K10.1 execution closure 2026-05-18 — native scheduler core landed (17 of 46 K10 items). K10 master closure waits for K10.4 (TLA+).
**Cumulative K0-K10.2**: K10.2 execution closure 2026-05-18 — native bus three-tier dispatch + mod ALC lifecycle native primitives landed (25 of 46 K10 items cumulative). K-L15 AUTHORED architecturally; sovereign authority switch deferred к K10.4 / А'.8 per managed-facade-preserved strategy.

**Combined с VULKAN_SUBSTRATE.md R.0-R.8 rendering migration + V0/V1/V2 substrate primitives**: 16-25 weeks total для full architectural foundation. K-series gates K9; K9 gates V substrate primitives. See [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) Roadmap for V0/V1/V2 detail и combined timeline (per Q-G-1/Q-G-2 LOCKs unifying former R-bucket + G-bucket into V substrate).

### K0 — Cherry-pick + cleanup от branch

**Goal**: experimental branch contents preserved on current main as fresh feature branch. Hygiene fixes applied.

**Source**: `claude/cpp-core-experiment-cEsyH` per `docs/reports/CPP_KERNEL_BRANCH_REPORT.md` Section 11.6.

**Cherry-pick sequence** (7 substantive commits, skipping 4 doc commits):
````
7b5cf78 — CMake scaffold
a8d235e — SparseSet template
cf0eed3 — World + C API
6eac732 — Interop project
80178c2 — Benchmark
f59492a — build files (cleaned)
e2bc2d9 — DLL loading fix
````

**Cleanup commits после cherry-pick**:
- `.gitignore` widening (exclude `native/*/out/`, `BenchmarkDotNet.Artifacts/`, committed `.dll`)
- Dead code removal (`SparseSet<T>` unused — delete OR wire к `RawComponentStore`)
- `.vscode/settings.json` → relative paths или remove
- `NATIVE_CORE.md` superseded marker + reference to KERNEL_ARCHITECTURE.md
- `NATIVE_CORE_EXPERIMENT.md` superseded marker

**Time**: 1-2 days. **LOC**: net +1500 (cherry-pick) - 50 (dead code) = +1450.

### K1 — Batching primitive

**Goal**: bulk operations + Span<T> access. Validates batching hypothesis quantitatively.

**Deliverables**:
- C ABI extension: `df_world_add_components_bulk`, `df_world_acquire_span`, `df_world_release_span`, `df_world_get_components_bulk`
- Managed bridge: `AddComponents<T>(ReadOnlySpan<EntityId>, ReadOnlySpan<T>)`, `AcquireSpan<T>()`, `WriteCommandBuffer` skeleton
- Native side: span acquisition с atomic counter
- Selftest extension: bulk add scenario, span access scenario
- Benchmark: `NativeBulkAddBenchmark` (target ≤200μs vs current 400μs unbatched, vs managed 218μs)

**Success criteria**:
- Bulk add 10k components в single P/Invoke
- Span<T> iteration zero P/Invoke per element
- Mutation rejection during active span verified
- Validation: zero memory leaks (selftest)

**Time**: 3-5 days. **LOC**: +500-800.

### K2 — Type-id registry + bridge tests

**Goal**: replace FNV-1a hash с explicit deterministic registry. Comprehensive bridge test coverage.

**Deliverables**:
- `ComponentTypeRegistry` class (sequential IDs, idempotent registration)
- `df_world_register_component_type(type_id, size)` C ABI function
- `DualFrontier.Core.Interop.Tests` project (xUnit, ~30-40 tests)
- Tests cover: NativeWorld CRUD equivalence, packing roundtrip, registry idempotency, span lease invariants, write buffer serialization

**Success criteria**:
- All bridge tests passing
- Hash collision risk eliminated (deterministic IDs)
- 472 + ~30 = ~500 tests passing

**Time**: 2-3 days. **LOC**: +400-600.

### K3 — Native bootstrap graph + thread pool

**Goal**: declarative startup task graph executed parallel где deps allow. Native scheduler used ONLY for bootstrap.

**Deliverables**:
- `bootstrap_graph.h/cpp` — declarative `kStartupTasks` array, Kahn topological sort
- `thread_pool.h/cpp` — std::thread pool (N cores), work-stealing OR fixed-partitioned
- `df_engine_bootstrap()` C ABI entry point (replaces direct `df_world_create`)
- Selftest extension: bootstrap graph correctness
- Benchmark: `BootstrapTimeBenchmark` (parallel vs sequential)

**Success criteria**:
- Engine bootstraps в ~5-15ms typical hardware
- Parallel tasks demonstrably parallel (e.g. AllocateMemoryPools then InitWorld + InitThreadPool в parallel)
- All bootstrap tasks complete before SignalEngineReady
- Validation clean

**Time**: 5-7 days. **LOC**: +600-900.

### K4 — Component struct refactor (Path α)

**Goal**: convert all class-based components к `unmanaged` structs.

**Scope**: ~50-80 components в `DualFrontier.Components/`. Each conversion:
- `class XComponent : IComponent` → `struct XComponent : IComponent`
- Update field access patterns
- Update systems that mutate (struct semantics — must use `ref` для mutation)
- Update tests

**Some components may need refactor**:
- Components с complex behavior (methods) — split к pure data + separate behavior class
- Components с reference fields — replace с EntityId references или separate storage

**Time**: 2-3 weeks (substantial scope, mostly mechanical).
**LOC**: +/- (conversion, не net additive).

**Success criteria**:
- All components are `unmanaged` structs
- All systems compile and tests pass
- Allocation profile: zero managed heap allocations during component access
- Existing 472 tests still passing

### K5 — Span<T> protocol + write command batching

**Goal**: production-grade span lifetime + write batching infrastructure.

**Deliverables**:
- `SpanLease<T>` (`IDisposable`) wrapping native span pointer
- `WriteCommandBuffer` full implementation (Add/Remove/Destroy/Spawn commands)
- Native side: command buffer parser в C++ (parses byte stream from managed)
- `df_world_flush_write_batch(world, buffer, size)` C ABI function
- Native side: mutation rejection during active spans (atomic counter)
- Tests: span lifetime, write batch round-trip, mutation rejection

**Time**: 1 week. **LOC**: +500-700.

### K6 — Second-graph rebuild on mod change

**Goal**: managed dependency graph rebuilds when mods load/unload. AssemblyLoadContext integration.

**Deliverables** (v1.1 — reconciled with M7-era implementation; pre-M7 wording kept in git history under v1.0):
- Graph rebuild primitive: `DependencyGraph.Reset() + AddSystem + Build()` invoked from `ModIntegrationPipeline.UnloadMod` step 4 and `Apply` steps [5-7]
- `ModLoader.UnloadMod(modId)` per `MOD_OS_ARCHITECTURE.md` §9.5 step 6; reload composition: `Pause + UnloadMod + Apply([newPath]) + Resume`
- Pause-rebuild-resume pattern composed across `GameLoop.SetPaused` and `ModIntegrationPipeline.Pause/Resume/Apply`; gate via `ModIntegrationPipeline.IsRunning` per `MOD_OS_ARCHITECTURE.md` §9.3
- Tests: `M71PauseResumeTests`, `M72UnloadChainTests`, `M73Step7Tests`, `M73Phase2DebtTests`, `RegularModTopologicalSortTests`, plus `M51`/`M52`/`M62` integration tests
- Adjacent debt closed during K6: `ModFaultHandler` implementing `IModFaultSink` (Application-side), wired through `ModLoader.HandleModFault` and `ModIntegrationPipeline` deferred drain

**Time**: 3-5 days. **LOC**: +200-400.

### K7 — Performance measurement (tick-loop)

**Goal**: representative-load benchmark applying §8 metrics rule.

**Deliverables**:
- `TickLoopBenchmark` — 50 pawns × full component set × 10k ticks
- Variants: managed-current, managed-with-structs (validates К7 conversion value), native-with-batching
- Metrics: p50/p95/p99 tick time, GC pause count + duration, total allocations, drift over time
- Run on weak hardware (Docker cpu-limit container OR secondary machine)
- Report file `docs/reports/PERFORMANCE_REPORT_K7.md` documenting findings

**Time**: 3-5 days. **LOC**: +200-400 (mostly benchmark code).

### K8 — Production storage cutover (Solution A: NativeWorld backbone)

**Decision** (recorded by K8.0 closure, 2026-05-09; see `docs/MIGRATION_PROGRESS.md` K8.0 closure section): Solution A — single NativeWorld backbone (per K-L11). Choice rationale in Part 4 Decisions log.

**Sub-milestone series**:
- **K8.0** — Architectural decision recording (this milestone; LOCKED v1.2)
- **K8.1** — Native-side reference handling primitives (string interning, keyed maps, composite components)
- **K8.2** — Per-component redesign + K8.1 wrapper value-type refactor + empty TODO stub deletions; K-L3 selective per-component closure achieved (v2 brief, single milestone; post-K-L3.1 reframing)
- **K8.3** — Production system migration (12 vanilla systems → SpanLease/WriteBatch)
- **K8.4** — ManagedWorld retired as production; Mod API v3 ships
- **K8.5** — Mod ecosystem migration prep (documentation + migration guide)

**Cumulative time**: 4-8 weeks at hobby pace.

**LOC delta**: substantial — K8.1 adds ~600-1000 LOC (native + bridge); K8.2 modifies 7 component files plus their consumers; K8.3 modifies 12 system files plus tests; K8.4 deletes managed World production path; K8.5 adds documentation.

### K9 — Field storage abstraction

**Goal**: native `RawTileField<T>` storage as a parallel abstraction alongside `RawComponentStore`. Prerequisite for the V substrate primitives roadmap ([VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) v1.0 LOCKED — V0/V1/V2 per Q-G-2). Ships CPU functional path first; no Vulkan compute dependency.

**Authoritative spec**: [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) "Architectural integration → Native kernel (KERNEL_ARCHITECTURE.md K9)" + "Roadmap → K9 — Field storage abstraction".

**Deliverables**:
- `RawTileField<T>` C++ class (data + back buffer + conductivity map + storage flags)
- C ABI: `df_world_register_field`, `df_world_field_read_cell`, `df_world_field_acquire_span`, `df_world_field_set_conductivity`, `df_world_field_set_storage_flag`
- Managed bridge: `FieldRegistry`, `FieldHandle<T>` в `DualFrontier.Core.Interop`
- CPU-side reference implementation of basic diffusion (also serves as G1+ shader equivalence oracle and as CPU fallback per [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) "Failure modes → CPU fallback")
- Selftest: round-trip, span access, mutation, conductivity update, storage flag toggle

**Success criteria**:
- Any field type registrable / readable / writeable from managed
- CPU diffusion produces correct results on test grids
- No GPU dependency (G-series can take over later without API churn)

**Time**: 1-2 weeks. **LOC**: +600-900.

---

## Part 3: Migration strategy

**Approach: parallel development through K7; LOCKED commitment from K8.0 onward.**

Managed `World` stayed functional throughout K0-K7. K8.0 closure (2026-05-09) recorded the architectural decision per K-L11: **Solution A — single NativeWorld backbone**. Migration executes via the K8.1-K8.5 sub-milestone series:
- K8.1 — native-side reference handling primitives (string interning, keyed maps, composite components)
- K8.2 — K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor + 6 class→struct conversions using K8.1 primitives + 6 empty TODO stub deletions per METHODOLOGY §7.1
- K8.3 — 12 vanilla systems migrated to `SpanLease<T>` reads + `WriteBatch<T>` writes
- K8.4 — managed `World` retired as production path; Mod API v3 ships
- K8.5 — mod ecosystem migration prep (documentation + migration guide)

**Operating principle**: «honest state always available» — managed World stayed working through K0-K7 so the K7 evidence base could be collected; the K-L11 commitment then settles the decision permanently. Reversal trigger documented in `docs/MIGRATION_PROGRESS.md` D5 (Solution A rationale and reversal trigger).

Mirrors VULKAN_SUBSTRATE.md migration approach (parallel Godot + Vulkan until M9.5 cutover).

---

## Part 4: Decisions log

### Resolved (LOCKED — see Part 0)

K-L1 through K-L10 above.

### Resolved (additional context)

**§6 vs §8 decision rule reconciliation**:
- §6 (English NATIVE_CORE.md): «≥20% combined Get+Add → continue» — naive baseline, optimizes wrong metric
- §8 (Russian NATIVE_CORE_EXPERIMENT.md): «GC pause / p99 / drift on weak hardware» — captures actual project value
- **§8 authoritative**, §6 superseded и retained для historical context only

**Path α vs Path β для components (resolved 2026-05-10 per K-L3.1)**:
- Path α: `unmanaged struct` components in kernel-side `NativeWorld` (existing K-L3 default; K8.2 v2 closure delivered for `src/DualFrontier.Components/`)
- Path β (original rejection): GCHandle marshalling on kernel-side managed component store — rejected, defeats GC pressure reduction goal
- Path β (K-L3.1 reformulation): managed `class` components annotated with `[ManagedStorage]`, stored mod-side in per-mod `RestrictedModApi.ManagedStore<T>` instance (Q2.β-i lock); kernel-side has no managed component store; ALC isolation provides ownership boundary; reclaim is deterministic on `AssemblyLoadContext.Unload`
- **Both paths chosen as first-class peers per K-L3.1 (2026-05-10)**: kernel-side native storage (Path α) preserves K-L11 «NativeWorld single source of truth» for native data; managed-storage decentralization-by-mod is consistent with K-L9 «vanilla = mods» + ALC isolation. Performance characteristics visible per-call via dual `SystemBase` API (Q3.i). Capability model path-orthogonal (Q6.a). Save system out of scope (Q4.b runtime-only managed-path). Amendment authority: K-L3.1 amendment plan at `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` + bridge formalization brief at `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md`.

**Q1 — Bootstrap graph: declarative**
- Imperative simpler но dependencies implicit
- Declarative explicit, symmetric с runtime second graph (K-L5)
- Cleanness > simplicity-of-implementation per Crystalka philosophy

**Q2 — Span<T> protocol: read-only spans + write batching**
- Read-write spans expose race conditions
- Read-only + batch flush makes mutations explicit (K-L7)
- Cleanness wins over micro-optimization

**Q3 — Mod-defined component types: fully supported**
- Vanilla mods register types via same API as third-party (K-L9)
- «Vanilla = mods» enforced architecturally

**Q4 — Two-phase entry point: clean separation**
- Native bootstrap + managed bootstrap distinct phases
- Single entry/exit per phase
- Boundaries explicit

**Solution A — single NativeWorld backbone (resolved 2026-05-09 per K8.0)**:
- K7 evidence: V3 (NativeWorld) dominates V2 (managed-with-structs) by 4× mean tick / 32× p99 / 27× total allocation / 0 vs 13 GC collections across 10k ticks
- Two alternatives considered:
  - Solution B (storage abstraction `IComponentStore` with managed and native impls): rejected — adds permanent runtime polymorphism layer, defers a decision the project is now committed to making, "structural костыль"
  - Solution C (explicit hybrid: struct components on Native, class components on Managed): rejected — bifurcated storage, permanent mental overhead for every mod author, cross-storage queries become friction
- **Solution A chosen**: single source of truth, K-L3 fully realized via K8.1 native-side reference primitives, K-L11 codifies commitment
- Crystalka commitment per chat session (2026-05-09): «игра это стресс тест, тут всё чистая инженирия и исследование, так что можно развивать максимально сложную архитектуру которая будет работать десятилетиями без костылей»
- Migration roadmap: K8.0 (decision) → K8.1 (primitives) → K8.2 (components) → K8.3 (systems) → K8.4 (retire managed) → K8.5 (mod ecosystem)

### Open (deferred)

| Decision | Trigger to resolve |
|---|---|
| Cross-platform support | If/when needed (parallels VULKAN_SUBSTRATE.md L7) |
| Vulkan dispatch (LibraryImport vs vkGetInstanceProcAddr) — applies к Native too | K7+ if profiling demands |
| Save/load of Native World | Persistence integration milestone (M-Persistence?) |
| Native event bus (если scheduler ever moves к native) | Currently не planned (К-L6 keeps systems managed) |

---

## Part 5: Risk register

**R1 — Component struct refactor scope underestimated**
- Probability: Medium-High
- Impact: K4 may extend от 2-3w к 4-5w
- Mitigation: incremental conversion (5-10 components per commit), tests verify each commit

**R2 — Path α components с complex behavior need redesign**
- Probability: Medium
- Impact: Some «components» may not survive struct conversion as-is
- Mitigation: case-by-case redesign, possibly split component into pure-data struct + behavior class kept managed

**R3 — Native scheduler complexity (К3)**
- Probability: Low (scope limited к bootstrap, не game tick)
- Impact: Bootstrap graph implementation more complex than estimated
- Mitigation: limited task count (~5-10 startup tasks), simple Kahn's algorithm

**R4 — Mod hot reload edge cases (К6)**
- Probability: Medium
- Impact: AssemblyLoadContext unload may leak refs, GC.Collect may не promptly release
- Mitigation: extensive testing, accept some leakage в development workflow

**R5 — Performance regression на weak hardware**
- Probability: Low (allocation reduction structural; refuted by K7 evidence on Skarlet hardware — V3 dominates V2 by 4-32× across §8 metrics)
- Impact: hypothetical K7+ measurement on weaker hardware shows native не faster than managed-with-structs
- Mitigation: K-L11 Solution A LOCKED (per K8.0 closure 2026-05-09) commits to NativeWorld production backbone. If a future weaker-hardware measurement surfaces a regression post-K8.4, K-L11 reversal trigger applies (re-open Solution C as fallback per `MIGRATION_PROGRESS.md` D5; explicit re-architecture milestone required).

**R6 — Cross-document drift (KERNEL ↔ RUNTIME)**
- Probability: Medium (two evolving docs)
- Impact: Decisions in one doc invalidate decisions in other
- Mitigation: §8 explicit cross-references, semantic version both docs together

---

## Part 6: Operational considerations

**Required tooling — install before K0**:
- CMake 3.20+ (already required by branch)
- Visual Studio 2022 17.8+ (для C++20 + [LibraryImport])
- C++ compiler: MSVC (Windows default) or Clang/GCC (cross-platform)

**Optional**:
- RenderDoc (graphics) — irrelevant к kernel
- Heaptrack/Valgrind — для memory leak detection в native code
- BenchmarkDotNet — already в branch

**Build pipeline**:
- Native build via CMake (independent of .sln)
- Post-build: copy `DualFrontier.Core.Native.dll` к Interop output directory
- Already configured в branch's BenchmarkDotNet csproj `<None Include>` pattern
- Future: `runtimes/{rid}/native/` packaging для cross-platform (deferred)

**Daily development flow**:
- Edit C++ → save → CMake rebuild → CTest selftest pass
- Edit C# → save → `dotnet build` → `dotnet test`
- Mixed-mode debugging (C++ + C#) requires Visual Studio Pro+ (deferred decision)

**Pipeline gating**:
- Before each commit: `dotnet build` clean, `dotnet test` 472+ passing, native selftest passing
- New convention: «kernel commits с C++ changes MUST include selftest run output в commit description»

---

## Part 7: Methodology adjustments для K-series

Existing methodology (METHODOLOGY.md) carries forward с adjustments:

**Pre-flight checks adapted**:
- Write-conflict table — still applies к managed Domain commits
- Project reference direction sanity check — extended: Interop reaches Native, no Domain reaches Native directly
- New: **C++ build verification** — `cmake --build` clean + selftest passing mandatory before commit
- New: **P/Invoke marshalling check** — every new `[DllImport]` declaration verified против native signature

**Brief structure**:
- M-phase boundary check expanded: Native / Interop / Domain / Mods boundaries
- Cross-language commits acceptable when C++ + C# changes coupled (e.g. K1 adds bulk function in both)
- Falsification clauses include native-specific edge cases (memory leaks, atomic memory ordering, span lifetime violations)

**Operating principle continues**:
- «Data exists or it doesn't» applies к component stores и span availability
- New corollary: «Native owns or managed holds opaque IntPtr — no in-between» — single ownership boundary
- Mirrors VULKAN_SUBSTRATE.md §1.9 «State exists или driver crashes»

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

**AD numbering continues**:
- M-series ADs от VULKAN_SUBSTRATE.md continue
- K-series ADs new sequence
- Direct Opus → Claude Code routing pattern continues

---

## Part 8: Relationship к VULKAN_SUBSTRATE.md

KERNEL_ARCHITECTURE.md (this) и VULKAN_SUBSTRATE.md describe **two halves of single architectural vision**: native foundation under managed Application layer.

### Symmetric architecture diagram

````
┌──────────────────────────────────────────────────────────────┐
│ DualFrontier.Application + Systems + Modding (managed)       │
│   - PhaseCoordinator (this doc + RUNTIME_ARCH §1.3)          │
│   - ManagedScheduler (mod systems, second graph)             │
│   - PresentationBridge (RUNTIME_ARCH §1.4)                   │
│   - Mod loader, scenarios, persistence                        │
└────────────┬─────────────────────────────────┬───────────────┘
             │                                 │
             ▼                                 ▼
┌──────────────────────────────────┐  ┌──────────────────────────────┐
│ DualFrontier.Core.Interop        │  │ DualFrontier.Runtime         │
│ (managed bridge layer)           │  │ (managed Vulkan adapter)     │
│  ↓ P/Invoke (this doc)           │  │  ↓ P/Invoke (RUNTIME_ARCH)   │
└──────────────────────────────────┘  └──────────────────────────────┘
             │                                 │
             ▼                                 ▼
┌──────────────────────────────────┐  ┌──────────────────────────────┐
│ DualFrontier.Core.Native.dll     │  │ vulkan-1.dll +               │
│ (built by us, C++20)             │  │ user32.dll + kernel32.dll    │
│  - World, component storage      │  │  (provided by OS / GPU       │
│  - Bootstrap graph + thread pool │  │   driver)                    │
│  - C ABI ~20 functions           │  │                              │
│  - Pure stdlib, zero deps        │  │                              │
└──────────────────────────────────┘  └──────────────────────────────┘
````

### Independent layers

**Kernel knows nothing of rendering**: `DualFrontier.Core.Native` doesn't include `vulkan-1.dll`, doesn't depend on VULKAN_SUBSTRATE.md decisions. Could be open-sourced separately as «sparse-set ECS kernel».

**Runtime knows nothing of ECS**: `DualFrontier.Runtime` doesn't include `DualFrontier.Core.Native.dll`, doesn't depend on KERNEL_ARCHITECTURE.md decisions. Could be open-sourced separately as «2D Vulkan runtime».

**Application orchestrates both** (managed C# layer): PhaseCoordinator drives game tick, calling Interop для ECS access и calling PresentationBridge для render commands. Both bridges are managed thin layers.

### Combined timeline

**Sequencing options** (per Crystalka philosophy «cleanness > expediency, long horizon»):

**Option β5 — kernel-fast-track**:
````
K0-K2 (~1-2w preservation + bridge maturity)
  → M9.0-M9.8 (5-7w Vulkan complete)
  → K3-K8 (4-7w kernel completion)
Total: 10-16 weeks
````

**Option β6 — kernel-first sequential**:
````
K0-K8 (5-8w kernel complete)
  → M9.0-M9.8 (5-7w Vulkan complete)
Total: 10-15 weeks
````

**Option β3 — interleaved** (earlier visible progress):
````
K0 (1-2d preservation)
  → M9.0-M9.5 (4-5w Vulkan parity)
  → K1-K2 (1w batching + tests)
  → M9.6-M9.8 (1-2w Vulkan finish)
  → K3-K8 (4-7w kernel completion)
Total: 11-15 weeks
````

**Recommendation** (per Crystalka philosophy): **β5 или β6 over β3** — single architectural focus per period preserves cleanness. Decision deferred к after K2 measurement (evidence-based choice). **Sequencing decision RESOLVED 2026-05-07** per K2 closure: **β6 selected**. См. `MIGRATION_PROGRESS.md` "Sequencing decision" section.

### Sequencing options including К9 + G-series

After K8 closure, two prerequisites unlock G-series:

- К9 (field storage CPU functional first, 1-2 weeks)
- M9.0–M9.4 (Vulkan instance/device live, 2-3 weeks within M-series)

Three valid sequencing options для post-K8 work:

**Option β6+G-sequential** (recommended baseline — single architectural focus per period):
````
K0-K8 (5-8w) → К9 (1-2w) → M9.0-M9.8 (4-7w) → G0-G9 (5-8w)
Total: 15-25 weeks
````

**Option β6+G-overlap** (К9 + early G-series concurrent с runtime, if hobby pace permits):
````
K0-K8 (5-8w) → split: { K9 + G0-G5 } parallel { M9.0-M9.8 } → G6-G9
Total: 13-22 weeks
````

**Option β6+G-runtime-first** (М9 ready first, then К9 + G-series sequentially):
````
K0-K8 (5-8w) → M9.0-M9.8 (4-7w) → К9 (1-2w) → G0-G9 (5-8w)
Total: 15-25 weeks
````

**Recommendation**: **β6+G-sequential** aligns с «cleanness > expediency» philosophy. Decision deferred к after K8 closure (evidence-based choice based on K8 metrics).

### Cross-document invariants

Both documents commit к following invariants:
- **«Без компромиссов»**: pure P/Invoke к OS/native APIs, no third-party C# binding libraries
- **Operating principle**: «data exists or it doesn't», honest state always
- **Single ownership boundary**: native owns native data, managed owns managed data, no shared mutability across boundary
- **Direction-discipline**: managed → native always, never reverse P/Invoke
- **Long-horizon planning**: cleanness over expediency, hard right over easy wrong

---

## Closing notes

3 weeks к current Dual Frontier state demonstrates high learning velocity, architectural rigor, methodology effectiveness. Combined kernel + runtime native vision within 9-15 weeks comparable к existing pace.

**«Features only on demand»** (continuing principle от VULKAN_SUBSTRATE.md): kernel API surface stays minimal. ~20 C ABI functions sufficient для full DF gameplay. Resist temptation к build «complete» ECS engine — every function must trace к specific Domain requirement.

This document is **v1.5** (current), authoritative until amended via explicit decision. Amendments require commit с rationale (similar к how MOD_OS_ARCHITECTURE.md evolved). Version history: v1.0 initial; v1.1 K6 reconciliation; v1.2 K-L11 + Solution A; v1.3 Interop error semantics convention; v1.4 K8.2 v2 closure (header bump deferred); v1.5 K-L3.1 bridge formalization.

Next document update expected при K8 closure (decision step results recorded), then per K-milestone (decisions log + risk register updates).

**Document end. Companion: METHODOLOGY.md, CODING_STANDARDS.md, MOD_OS_ARCHITECTURE.md, VULKAN_SUBSTRATE.md, [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) (v2.0 LOCKED — K9 field storage + G-series Vulkan compute).**
