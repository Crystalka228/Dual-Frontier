---
register_id: DOC-A-KERNEL_ARCHITECTURE
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.1
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: DualFrontier Kernel — Architecture (authored rework; sole К-L canon carrier)
supersedes:
- DOC-A-KERNEL
last_modified_commit: 6497ed5
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'DRAFTS_RATIFICATION MC-1 (C5): candidate-banner class retired - banner to ratified-successor note (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION carried), checklist line removed, Role to normative (ratified successor) where the candidate token was present, pending-amendment sentence to LOCKED form (ARCHITECTURE, CONTRACTS). Changelog status cells left as authored-session history per HALT-1 OD-2. PATCH 1.0.0 to 1.0.1.'
reviewer: Crystalka
requirements_authored:
- REQ-K-L1
- REQ-K-L2
- REQ-K-L3
- REQ-K-L4
- REQ-K-L5
- REQ-K-L6
- REQ-K-L7
- REQ-K-L7_1
- REQ-K-L8
- REQ-K-L9
- REQ-K-L10
- REQ-K-L11
- REQ-K-L12
- REQ-K-L13
- REQ-K-L14
- REQ-K-L15
- REQ-K-L15_1
- REQ-K-L16
- REQ-K-L17
- REQ-K-L18
- REQ-K-L19
special_case_rationale: 'Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist items [1]+[2]: Annex A approved; REQ-K-L1..K-L19 carried over from DOC-A-KERNEL at this cascade). Part 0 is the sole К-L canon carrier (consolidated verbatim from K_CLOSURE_REPORT §2, resolves session finding N-1; ratified К-L14 exception: full thesis stays in K_CLOSURE_REPORT §1.2 per Q-N-8-2). Successor of DOC-A-KERNEL per EVT-2026-07-15-CORPUS_REWORK_R1_KERNEL_CORE; predecessor preserved at docs/architecture/historical/.'
---

# DualFrontier Kernel — Architecture

The native ECS kernel contract: the К-L invariant canon (Part 0) plus the current-truth architecture of storage, identity, boundaries, and the C ABI between the C++20 kernel and the managed layers.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/KERNEL_ARCHITECTURE.md` (DOC-A-KERNEL, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`.

## Status

| Field | Value |
|---|---|
| Role | normative (ratified successor) |
| Successor of | `docs/architecture/historical/KERNEL_ARCHITECTURE.md` (DOC-A-KERNEL, LOCKED v2.6.2, now SUPERSEDED) |
| Scope | К-L invariant canon (Part 0, sole carrier); two-phase bootstrap; NativeWorld storage + span/batch protocol; type-id registry; dependency rules 1–5; C ABI error conventions; threading summary |
| Non-goals | Scheduling/bus/mod-lifecycle/Vulkan mechanism detail; roadmap history; per-rule analyzer specs; performance budgets |
| Authority domains | **storage/memory** (NativeWorld SSoT, span/batch, component lifetime) · **entity-identity** (component type ids; EntityId law shared with IDENTITY_AND_ABI_CONTRACT.md draft) · **boundary/layering** (dependency rules 1–5, P/Invoke surface) · **error-handling** (C ABI status-code law, four-category managed rule) |
| Defers to | [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md) → scheduling detail · [EVENT_BUS.md](./EVENT_BUS.md) → bus semantics · [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) → mod lifecycle · [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) → gpu · [THREADING.md](./THREADING.md) → threading |

---

## Part 0 — К-L invariant canon

**This Part is the sole canonical carrier of К-L texts** (consolidated from K_CLOSURE_REPORT §2, which remains the closure-evidence record). One ratified exception: the К-L14 full thesis resides in K_CLOSURE_REPORT.md §1.2 per Q-N-8-2 LOCKED; this Part carries К-L14's abbreviated form, as the predecessor did.

Series state: **21 invariants** — К-L1..К-L19 (К-L6 SUPERSEDED listed for traceability, excluded from the count) + sub-invariants К-L3.1/К-L7.1/К-L15.1; К-L20 reserved separately. (The Part 0 table carries 22 canonical-text rows — the 21 active plus the superseded К-L6, retained verbatim.) Departures require an explicit re-architecture milestone (К-extensions cascade or supersession event recorded in the register), never spec-level adjustment mid-implementation.

Per-invariant format: status/LOCK-history line → canonical text (verbatim blockquote) → falsifiability commitments → implementation artifacts corrected to the tree at HEAD `35364c2` → DF-rule row stating the shipped analyzer truth per [ANALYZER_RULES.md](./ANALYZER_RULES.md) §1.1 (no rule is claimed Active that does not exist on disk).

### К-L1 — Native language C++20

**Status / LOCK history**: LOCKED at К0 (2026-05-07; foundational — no candidate phase).

> Native language: C++20 (MSVC/GCC/Clang). Compiled native, modern features (concepts, ranges, modules where applicable), no third-party dependencies в production binary.

**Falsifiability**: production code regressing к pre-C++20 dialect; a third-party C++ library introduced без explicit deliberation; managed C# bypassing the kernel via an alternative native binding.

**Implementation artifacts**: `native/DualFrontier.Core.Native/CMakeLists.txt:4-5` (`CMAKE_CXX_STANDARD 20` + `CMAKE_CXX_STANDARD_REQUIRED ON`); the kernel tree `native/DualFrontier.Core.Native/{include,src}/` uses C++20 stdlib plus the Vulkan SDK link required by the V1 compute substrate (`CMakeLists.txt:12,99`); no other third-party dependency.

**DF rule**: **DFK001** — Error, enforcing (shipped Release 1.0). Suppression census: exactly 2 DFK-WAIVERs, both DFK001, `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` (К-L19-sanctioned VK_EXT_debug_utils interop), census-pinned by meta-test.

### К-L2 — Bindings via pure P/Invoke

**Status / LOCK history**: LOCKED at К0 (2026-05-07); verified through К0–К10 + А'.7.x.

> Bindings: pure P/Invoke к `DualFrontier.Core.Native.dll`. Zero third-party C# binding library в production binary. Mirrors Vulkan substrate L2 commitment (zero third-party Vulkan binding library — direct C ABI к Vulkan loader через P/Invoke).

**Falsifiability**: a third-party C# binding library in production; managed code bypassing the single-DLL P/Invoke boundary; `Core.Native.dll` splitting into multiple production DLLs (the А'.7.5 source split preserved the single binary).

**Implementation artifacts**: `src/DualFrontier.Core.Interop/NativeMethods.cs` + partials `NativeMethods.{Bus,Compute,Fields,PipelineSlot,Scheduler}.cs` and the wrapper-local declarations in `BackgroundQueueInterop.cs` / `EventTypeRegistryInterop.cs` / `ModUnloadInterop.cs` — the only kernel-DLL DllImport sites in the solution (verified census); `native/DualFrontier.Core.Native/` builds the single `DualFrontier.Core.Native.dll`. (Predecessor's `NativeMethods.{Bootstrap,Storage,Span,Batch}.cs` partial names never shipped.)

**DF rule**: **DFK002** — Error, enforcing. Federated interop model (A'.9.1 Phase β §8): sanctioned P/Invoke surface = `DualFrontier.Core.Interop` (kernel boundary) + `DualFrontier.Runtime.Native` (Vulkan К-L19 + Win32 Launcher surface), one `SanctionedInteropSurface` definition shared by DFK002 + DFK001.

### К-L3 — Component storage paths (Path α default, Path β opt-in)

**Status / LOCK history**: LOCKED at К0 (2026-05-07); amended by sub-invariant К-L3.1 (LOCKED А'.0, 2026-05-10).

> Component storage paths: **Path α** (`unmanaged struct` implementing IComponent, native-side `NativeWorld` storage, structure-of-arrays layout via NativeMap<K,V>/NativeSet<T>/NativeComposite<T> primitives) is default. **Path β** (managed `class : IComponent` with `[ManagedStorage]` opt-in, mod-side per-mod ManagedStore<T> storage, Dictionary-shaped lookup) is per-component opt-in.
>
> Decision criterion is **per-component architectural fit**: Path α applies when conversion to `unmanaged struct` is justified (performance, locality, blittable layout, К8.1 primitive coverage); Path β applies when conversion forces structural compromise (managed-only references not expressible as К8.1 primitives, lazy state graphs, runtime-only computed handles, complex object graphs not blittable).

**Falsifiability**: Path α universal mandate reintroduced; Path β components persisting through the save system (Q4.b runtime-only lock); cross-mod managed-path direct access enabled (ALC isolation broken).

**Implementation artifacts**: `src/DualFrontier.Contracts/Core/IComponent.cs`; `src/DualFrontier.Contracts/Modding/ManagedStorageAttribute.cs` (Path β opt-in marker); `src/DualFrontier.Core.Interop/NativeWorld.cs:345,376` (Path α `AcquireSpan<T>` + `BeginBatch<T>`); `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (Path β per-mod store instantiation). (No `src/DualFrontier.Modding/` project exists — the modding surface splits between Contracts and Application/Modding.)

**DF rule**: **DFK003** — Error, enforcing.

### К-L3.1 — Path β bridge formalization (sub-invariant of К-L3)

**Status / LOCK history**: AUTHORED and LOCKED at А'.0 (2026-05-10). First sub-invariant precedent («sub-invariant LOCKs while the parent stays candidate»), later applied to К-L7.1 and К-L15.1.

> Path β is **first-class peer** к Path α (not «α default plus β tolerated as exception» per superseded К4 «Hybrid Path» framing). Author choice is recorded explicitly via `[ManagedStorage]` opt-in; absence implies Path α.
>
> Per-component architectural fit (Path α for blittable-shape data; Path β for managed-only references, lazy state graphs, runtime-only handles, complex object graphs). Path β components are **runtime-only** (Q4.b lock — not persisted by save system); managed-storage lives per-mod (mod assembly's `RestrictedModApi` instance), reclaimed deterministically on `AssemblyLoadContext.Unload` per [MOD_OS_ARCHITECTURE.md §9.5 unload chain](MOD_OS_ARCHITECTURE.md).
>
> Within-mod cross-path access supported via **dual `SystemBase` API** (Q3.i lock): `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α, `SystemBase.ManagedStore<T>()` for Path β. Performance characteristics visible per-call (no opaque dispatch).
>
> Cross-mod managed-path direct access is **structurally impossible** by ALC isolation; cross-mod data flow uses event/intent contracts per [MOD_OS_ARCHITECTURE.md §6 three-level contracts](MOD_OS_ARCHITECTURE.md).

**Falsifiability**: a Path γ candidate superseding Path β (currently none); a cross-mod managed-path access mechanism; Path β persistence enabled.

**Implementation artifacts**: `src/DualFrontier.Contracts/Modding/ManagedStore.cs`; `src/DualFrontier.Application/Modding/RestrictedModApi.cs` (Path β registration surface); `src/DualFrontier.Core/ECS/SystemBase.cs:93` (dual API: `SystemBase.NativeWorld` guard + `ManagedStore<T>` accessor; predecessor's `Systems/SystemBase.cs` path corrected).

**DF rule**: **DFK003_1** — Error, enforcing (underscore descriptor form per the 2026-07-01 descriptor-ID adjudication; dotted `DFK003.1` is the superseded convention).

### К-L4 — Explicit type ID registry

**Status / LOCK history**: LOCKED at К0 (2026-05-07).

> Component type IDs: explicit per-mod registration. FNV-1a hash auto-generation prohibited (collision-prone). Per-mod registration ensures deterministic ID assignment + cross-mod isolation.

**Falsifiability**: hash-based type ID auto-generation introduced; a cross-mod type ID collision; compile-time id generation (loses per-mod registration determinism).

**Implementation artifacts**: `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs` (§3 below); `src/DualFrontier.Contracts/Modding/IModApi.cs:38` (`RegisterComponent<T>()`). Scope note: К-L4 governs **component** ids; event type ids are a separate FNV-1a-of-FQN space owned by the bus ([EVENT_BUS.md](./EVENT_BUS.md)) — a different id space, not a violation.

**DF rule**: **DFK004** — Error, enforcing.

### К-L5 — Declarative bootstrap graph

**Status / LOCK history**: LOCKED at К0 (2026-05-07).

> Bootstrap orchestration: declarative graph, native, parallel where deps allow. Symmetric к runtime second graph (К-L12 native scheduler dependency graph). Explicit dependencies — no implicit ordering, no startup-order anti-pattern.

**Falsifiability**: implicit startup ordering reintroduced; managed-side bootstrap fragments duplicating the native graph; circular dependencies allowed in bootstrap.

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/bootstrap_graph.cpp`; `src/DualFrontier.Core.Interop/Bootstrap.cs` (`Bootstrap.Run` → `df_engine_bootstrap`, atomic success-or-rollback); `src/DualFrontier.Application/Loop/GameBootstrap.cs` (managed orchestration entry; predecessor's `Bootstrap/GameBootstrap.cs` path corrected).

**DF rule**: **DFK005** — Error, enforcing.

### К-L6 — Game tick scheduler: managed [SUPERSEDED by К-L12]

**Status / LOCK history**: LOCKED at К0 → SUPERSEDED by К-L12 at К10.1 (2026-05-18). Original canonical text, preserved for traceability:

> Game tick scheduler: managed C#. Sequential dispatch (no native scheduler). Rationale: «Vanilla = mods» preserved via single managed dispatch path — К-L9 priority over implementation tier.

**Supersession rationale** (К10.1, 2026-05-18): the К10 arc ratified that К-L9 «Vanilla = mods» does not require a managed scheduler — the invariant survives through the facade pattern while native authority owns kernel scheduling decisions (К-L12 kernel/user split). К-L6 no longer manifests in production; К-L12 carries the invariant forward.

**DF rule**: **DFK006** — never activates; reserved historical entry, no enforcement (ANALYZER_RULES §4 realized decision record).

### К-L7 — Span protocol (parent invariant)

**Status / LOCK history**: LOCKED at К0 (2026-05-07); sub-invariant К-L7.1 below.

> Span protocol: read-only spans + write command batching. Mutation semantics explicit through API surface (AcquireSpan<T>() returns SpanLease<T>; BeginBatch<T>() returns WriteBatch<T>). Race conditions structurally impossible within tick boundary через single-writer per-tick discipline + read-span immutability while held.
>
> К-L7 atomic-from-observer invariant: within single tick T, all reads of component type T see consistent snapshot (no torn reads, no mid-tick mutation visibility leaks).

**Falsifiability**: torn reads within a tick boundary; mutation visibility leaking mid-tick (batch flushed before the boundary); single-writer per-tick discipline broken.

**Implementation artifacts**: `src/DualFrontier.Core.Interop/NativeWorld.cs:345,376` (`AcquireSpan<T>` / `BeginBatch<T>`); `SpanLease.cs` (read-span lifetime); `WriteBatch.cs` (batched writes); native protocol in `native/DualFrontier.Core.Native/src/world.cpp` + `capi.cpp` (span acquire/release + mutation-rejection counter; predecessor's `span_native.cpp` never existed).

**DF rule**: **DFK007** — Error, enforcing.

### К-L7.1 — GPU compute pipeline slot binding (sub-invariant of К-L7)

**Status / LOCK history**: AUTHORED at К10.3 v2 (2026-05-20) → LOCKED at A'.8 closure (2026-05-23, Q-N-8-1 batch).

> GPU compute writes к RawTileField storage bound к **pipeline slot** (К-L16 pipeline-managed dispatches). Sim-thread reads of pipeline-managed fields see **slot-tail state** (sim tick T+D reads dispatched-at-(T+D-1) state). One-tick lag (D=1 default per К-L16) bounded и deterministic.
>
> К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots (each pipeline slot maintains its own snapshot through GPU fence completion). One-tick lag is **opt-in coexistence с V1 sync default**: per-field author choice per К-L9 «Vanilla = mods». V1 К-L7 sync dispatch_compute_field path orthogonal и preserved для existing FieldHandle consumers.

**Falsifiability**: slot cross-contamination (slot T snapshot leaking into slot T+1 reads); GPU fence completion violated (sim reads dispatched-but-uncompleted state); К-L7 sync-default coexistence broken (V1 consumers force-migrated).

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (slot state machine); `compute_dispatch.cpp` (V1 К-L7 sync path — blocks until VkFence signals; preserved); `phase_compute.cpp` (Item 35 Phase.Compute scheduler integration); managed mirrors `NativeMethods.Compute.cs` + `PipelineSlotInterop.cs`. (Predecessor's `dispatch_compute_field*.cpp` / `Scheduler/Phase.Compute.cs` names never shipped.)

**DF rule**: **DFK007_1** — Error, enforcing.

### К-L8 — Component lifetime (native owns storage)

**Status / LOCK history**: LOCKED at К0 (2026-05-07).

> Component lifetime: native owns storage; managed holds opaque `IntPtr` через NativeWorld handle. Single ownership boundary; managed holds handle only. No managed-side component pool, no managed-side component lifetime tracking.
>
> Post-К-L11 production manifestation: NativeWorld is single source of truth для production storage; ManagedWorld retained as test fixture and research artifact only. К-L8 + К-L11 combined establish complete native authority over production component lifetime.

**Falsifiability**: a managed-side component pool reintroduced; dual ownership boundaries (a component living managed and native simultaneously); production construction of the managed world.

**Implementation artifacts**: `src/DualFrontier.Core.Interop/NativeWorld.cs` (opaque handle; `Dispose` :486, finalizer :496); `src/DualFrontier.Application/Loop/GameBootstrap.cs:76` (production construction); `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs` (fixture only). Known lifetime gap: deterministic disposal never runs in production — §1.

**DF rule**: **DFK008** — outside Roslyn scope; process-invariant with a git pre-commit hook designated as the alternative mechanism (ANALYZER_RULES §4). No Roslyn rule exists or is scheduled.

### К-L9 — Mod parity («Vanilla = mods»)

**Status / LOCK history**: LOCKED at К0 (2026-05-07); preserved through the К-L12 facade pattern at К10.1.

> Mod parity: vanilla mods register components and systems through the same IModApi as third-party mods. «Vanilla = mods» enforced at architecture level. No vanilla privilege: Vanilla.{Core,Combat,Magic,Inventory,Pawn,World} mods use identical registration surface as third-party.
>
> Post-К-L12 manifestation: К-L9 preserved через managed scheduler facade (IModApi-exposed surface) routing к native scheduler authority через C ABI batched callbacks. Mod system execution bodies remain managed; vanilla AND third-party uniformly. К-L9 invariant is preserved при К-L6 supersession by К-L12.

**Falsifiability**: vanilla privilege introduced (special registration surface, scheduler authority, or bus access); vanilla mods bypassing the К-L12 facade; «Vanilla = mods» purity failing the Phase B M-cycle verification.

**Implementation artifacts**: `src/DualFrontier.Contracts/Modding/IModApi.cs` (v3 unified surface); `mods/DualFrontier.Mod.Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}/` — vanilla mods compile against Contracts only, identically to `mods/DualFrontier.Mod.Example/` (csproj-verified; predecessor's `vanilla/Vanilla.*` layout corrected). No `ManagedSchedulerFacade.cs` exists — the facade role is carried by `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/`) plus `BusFacade` (`src/DualFrontier.Application/Bus/`).

**DF rule**: **DFK009** — DEFERRED to the К-L20 LOCK cascade; no rule on disk (predecessor "Error, Active" claim withdrawn — Annex A-4).

### К-L10 — Decision rule (§8 metrics authority)

**Status / LOCK history**: LOCKED at К0 (2026-05-07).

> Decision rule: §8 metrics (GC pause time / p99 frame latency / long-run drift on weak hardware) supersede §6 «20% mean speed» as primary decision criterion. §8 captures actual project value (responsiveness on weak hardware, long-haul stability) rather than synthetic peak-speed comparisons.
>
> §6 «20% mean speed» framing superseded due to research framework reframe: synthetic peak-speed metrics не correlate с player experience on target hardware tier. §8 metrics directly map к К-L19 hardware tier commitment.

**Falsifiability**: «20% mean speed» reintroduced for go/no-go decisions; synthetic benchmarks overriding §8 empirical observations; К-L14 evidence contradicting the §8 framing.

**Implementation artifacts**: `docs/methodology/PIPELINE_METRICS.md` (metrics dashboard); [PERFORMANCE.md](./PERFORMANCE.md) (decision-rule reference — note its enforcement layer is currently unimplemented; see the session report §3 C12).

**DF rule**: **DFK010** — PERMANENTLY DROPPED per Q-L-9 + PA-002 («без костылей»); enforcement is FRAMEWORK/METHODOLOGY documentation discipline only (predecessor "Error, Active" claim withdrawn — Annex A-4).

### К-L11 — Production storage backbone (NativeWorld single source of truth)

**Status / LOCK history**: AUTHORED at К7 (V3-dominance evidence) → LOCKED at К8.4 v2 closure (Solution A commitment).

> Production storage backbone: **NativeWorld single source of truth** (Solution A). ManagedWorld retained as test fixture (ManagedTestWorld) and research artifact only.
>
> К7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + «no compromises» commitment (К-L14 default-inclusion bias precedent) ratified Solution A choice. Single ownership boundary, single mental model, single storage authority.

**Falsifiability**: production construction of the managed world; dual storage backbones reintroduced; К-L7/К-L8/К-L9 breaking under the completed migration.

**Implementation artifacts**: `src/DualFrontier.Core.Interop/NativeWorld.cs` (production storage authority); `tests/DualFrontier.Core.Tests/Fixtures/ManagedTestWorld.cs` (fixture only); `GameBootstrap.cs:76` (sole production construction path).

**DF rule**: **DFK011** — Error, enforcing.

### К-L12 — Full native kernel scheduling

**Status / LOCK history**: AUTHORED at the К10 arc (2026-05-17) → AUTHORED at К10.1 closure (2026-05-18) → LOCKED at A'.8 (2026-05-23, Q-N-8-1 batch).

> Native kernel scheduling: sovereign per-tick scheduling for kernel-space systems (Core) native. Managed scheduler scope reduced к user-space (mod) systems within mod ALCs.
>
> Kernel scheduling decisions made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement.
>
> Cross-layer communication uses C ABI with batched callbacks (managed adapter receives batched managed-system dispatches from kernel scheduler). К-L6 SUPERSEDED by К-L12 — managed scheduler facade preserved for К-L9 «Vanilla = mods» uniformity.

**Current/target wiring (2026-07-15).** CURRENT — systems are registered twice at startup (`GameBootstrap.cs:145-181`): once with the managed `DependencyGraph` and once with the native `SystemGraph`, whose registration passes **empty read/write component-id sets** (`GameBootstrap.cs:170-176`) plus a TimerWake at rate 1 for every system (`GameBootstrap.cs:179`). Production phases are planned by the managed `DependencyGraph` and executed via `Parallel.ForEach` (`ParallelSystemScheduler.cs:149`); the batched-callback adapter is on disk and test-exercised only (`BatchedCallbackTests.cs:30`).

> **FENCED (target / planned — not current truth):** TARGET — sovereignty per the canonical text above: native per-tick graph over the runnable subset drives dispatch through the batched-callback ABI; the managed `DependencyGraph` is deleted. Cutover gates and the `DependencyGraph` deletion trigger per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft); sequencing per [docs/ROADMAP.md](../ROADMAP.md).

**Falsifiability**: managed scheduler reclaiming sovereign authority; cross-layer dispatch deviating from the batched callback ABI (synchronous per-system callbacks); mods reaching the native scheduler directly past the К-L9 facade. **Falsifiability clauses bind the post-cutover end state; the pre-cutover wiring above is the sanctioned migration state, not a falsification.**

**Implementation artifacts (on disk; dispatch switch pending)**: `native/DualFrontier.Core.Native/src/system_graph.cpp` (static + per-tick Kahn graphs), `wake_registry.cpp`, `scheduling_policies.cpp`, `state_change_filter.cpp` (write-through wake hook), `managed_callback.cpp` (batched-callback ABI), `scheduler_trace.cpp`, `scheduler_intrinsics.cpp`, `thread_pool.cpp`; managed adapter `src/DualFrontier.Application/Scheduler/SchedulerAdapter.cs:22` + `ManagedSystemDispatcher.cs:75` (`[UnmanagedCallersOnly]` `OnBatch`). (Predecessor's `scheduler_native.cpp` / `scheduler_graph.cpp` / `scheduler_runqueue.cpp` / `scheduler_wake.cpp` / `ManagedSchedulerFacade.cs` never existed.)

**DF rule**: **DFK012** — DEFERRED to the К-L20 LOCK cascade; no rule on disk (predecessor "Error, Active" claim withdrawn — Annex A-4). Mechanism detail defers to [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md).

### К-L13 — On-demand system activation (five wake types)

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.1 (2026-05-18) → LOCKED at A'.8 (Q-N-8-1).

> On-demand system activation: **five wake types** — Timer, Event, StateChange, Init, Explicit. Only runnable systems enter phase dispatch; per-tick dependency graph computed on runnable subset (NOT full system inventory).
>
> Real-OS process-blocking model: systems wait on wake conditions analogously к OS process blocking states. Sparse-world efficiency: empty colony regions skip system dispatch entirely (no useless work on dormant entities).
>
> Cache locality improvement: runnable subset is typically small (gameplay-state-dependent), improves CPU cache hit rates relative к full-inventory dispatch.

**Current-wiring note (2026-07-15)**: production registration wakes every system with `TimerWake` rate 1 (`GameBootstrap.cs:179`) — the every-tick degenerate case — until `[WakeOn*]` declarations are marshalled through; the runnable-subset machinery is native-side complete and selftest-exercised ([THREADING.md](./THREADING.md)).

**Falsifiability**: falsified if the wake type set expands without a principled К-extensions cascade; if full-inventory dispatch is reintroduced post-cutover; if cache locality degrades systematically (co-fires К-L14 criterion 1).

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/wake_registry.cpp` (subscription tables + runqueue); `src/DualFrontier.Core.Interop/WakeRegistryInterop.cs`; `src/DualFrontier.Contracts/Scheduling/WakeAttributes.cs` + `WakeOnSlotTransitionAttribute.cs`; `Contracts/Attributes/TickRateAttribute.cs` (Timer subsumes `[TickRate]`). (Predecessor's `scheduler_wake.cpp` and `SystemBase.RegisterWakeSource` never shipped.)

**DF rule**: **DFK013** — Warning, enforcing (efficiency-not-correctness class).

### К-L14 — Performance derives from cleanness (meta-invariant)

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.1 → LOCKED at A'.8 (Q-N-8-1); canonical text ratified per Q-N-8-2 LOCKED (2026-05-23).

> **Abbreviated form**: Performance derives from cleanness; architectural completeness causes performance; tactical heuristics inapplicable в research framework; default-inclusion bias for architectural items. See K_CLOSURE_REPORT.md §1.2 for canonical text.

Canonical-text location is the ratified Q-N-8-2 hybrid: the full thesis (Causality + Empirical + Falsifiability + Default-inclusion bias + Burden of proof sub-clauses) resides in [K_CLOSURE_REPORT.md §1.2](K_CLOSURE_REPORT.md) — the one К-L text this Part does not carry in full.

**Falsifiability** (tracked in [K_L14_EVIDENCE_DASHBOARD.md](K_L14_EVIDENCE_DASHBOARD.md), verification log through #14 at HEAD — 13 in active log, #10 vacated): six criteria — performance-ceiling decrease under К-extension cascades (NOT falsified; А'.7.x +45% confirming), systematic hard-halt trend (NOT falsified; 1 soft-halt honestly annotated, retroactively closed), alignment-maturity reversal (NOT falsified), atomic-discipline breakdown (NOT falsified), completeness cost exceeding long-horizon payoff (deferred — metric TBD post-Phase B), soft-halt rate over threshold (provisional, threshold TBD per Q-N-8-7).

**Implementation artifacts**: none direct (meta-invariant); manifests through the other invariants' surfaces and the evidence dashboard.

**DF rule**: **DFK014** — outside Roslyn scope; the evidence dashboard is the designated mechanism (ANALYZER_RULES §4). No Roslyn rule exists or is scheduled.

### К-L15 — Native bus authority + three-tier event dispatch (parent invariant)

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.2 (2026-05-18) → LOCKED at A'.8 (Q-N-8-1); sub-invariant К-L15.1 LOCKED earlier at А'.7.x.

> Native bus authority: native kernel owns sovereign event routing для kernel-space and cross-layer events. Bus implementation native: type registry, subscriber registry, payload dispatch, wake firing, tier-based delivery semantics all native authority.
>
> Three-tier dispatch (fast / normal / background) with tier declared per event type:
> - **Fast tier**: subscriber callback latency ≤ 1ms (immediate synchronous callback)
> - **Normal tier**: subscriber callback latency ≤ 1 tick (batched callback ABI, dispatched at tick boundary)
> - **Background tier**: subscriber callback latency ≤ N ticks (background queue + idle-slot dispatch; coalescable)
>
> Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity); implementation routes through C ABI bridge к native bus. Fast/background event publish/subscribe requires per-FQN per-tier capability declarations.

**Current/target wiring (2026-07-15).** CURRENT — production events travel the five managed `DomainEventBus` instances (`GameServices.cs:16-113`: Combat/Inventory/Magic/World/Pawn buses); `BusFacade.UseNativeBusForDispatch` defaults `false` and no production code constructs a `BusFacade` (`BusFacade.cs:49`). Live native touchpoints today: the Background-tier drain each tick (`GameLoop.cs:120-128` via `ManagedBusBridge.DrainBackgroundBatch`) and per-mod bulk unsubscribe at unload (vacuous — production registers no native subscribers).

> **FENCED (target / planned — not current truth):** TARGET — sovereign native routing per the canonical text above; the facade flag flips and the managed dispatch path retires. Gates per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft); sequencing per [docs/ROADMAP.md](../ROADMAP.md).

**Falsifiability**: managed bus reclaiming sovereign authority; tier latency contracts violated (fast > 1ms; normal > 1 tick; background beyond its N-tick bound); mods reaching the native bus without capability declaration; К-L15.1 violation. **Falsifiability clauses bind the post-cutover end state; the pre-cutover wiring above is the sanctioned migration state, not a falsification.**

**Implementation artifacts (on disk; dispatch switch pending)**: `native/DualFrontier.Core.Native/include/bus_native.h` (single C ABI surface, unchanged through А'.7.x + А'.7.5); `bus_fast.cpp`, `bus_normal.cpp`, `bus_background.cpp`, `bus_common.cpp` (+ `background_queue.cpp`, `event_type_registry.cpp`); `src/DualFrontier.Application/Bus/BusFacade.cs` + `ManagedBusBridge.cs`.

**DF rule**: **DFK015** — DEFERRED to the К-L20 LOCK cascade; no base rule on disk (predecessor "Error, Active" claim withdrawn — Annex A-4). Sub-rule DFK015_1 is shipped — see К-L15.1. Bus semantics defer to [EVENT_BUS.md](./EVENT_BUS.md).

### К-L15.1 — Three-tier independence (sub-invariant of К-L15)

**Status / LOCK history**: AUTHORED and LOCKED at А'.7.x cascade #0 γ4 load-bearing commit `08d0bba` (2026-05-21; sub-invariant LOCKs while the parent stays candidate); compile-time layer materialized at А'.7.5 (2026-05-22).

> Each tier owns architectural isolation at **three structural layers**:
>
> **Layer 1 — State layer** (per-tier state struct): FastTierState / NormalTierState / BackgroundTierState с separate `std::mutex`, separate `next_seq` counter, separate subscriber map, separate pending queue where applicable. No shared mutable state between tiers.
>
> **Layer 2 — Runtime layer** (subscription ID disambiguation): Subscription ID space uses **high 8 bits = tier identifier + low 56 bits = per-tier sequential counter**. Cross-tier collisions structurally impossible. `df_bus_unsubscribe` dispatches via tier-bit; `df_bus_clear` acquires three tier mutexes in fixed `fast → normal → background` order for deadlock safety.
>
> **Layer 3 — Compile-time layer** (А'.7.5 materialization, source split bus_native.cpp → 4-file): Source-level translation unit separation per tier. `bus_fast.cpp` / `bus_normal.cpp` / `bus_background.cpp` host tier-specific implementations; `bus_common.cpp` hosts shared helpers + dispatch entry points (single C ABI surface preserved per К-L2). Compile-time enforcement: tier implementation cannot accidentally reach into another tier's state struct (translation unit boundaries enforce isolation at compile level).
>
> **Cross-tier publish semantics**: Fast subscriber callback MAY publish events к any tier — re-entrant safe through mutex isolation (post-А'.7.x); pre-А'.7.x single shared mutex prevented re-entrant publish и was a structural deadlock hazard. К-L15.1 closes the deadlock hazard.

**Falsifiability**: falsified if tier dependence returns at the state level (shared mutex), the runtime level (shared subscription counter, broken tier-bit disambiguation), or via cross-tier re-entrancy deadlock (regression of the S10 probe — `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs:997-1012`); or if the compile-time layer boundary is crossed.

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/bus_fast.cpp` / `bus_normal.cpp` / `bus_background.cpp` / `bus_common.cpp`; internal header `src/bus_native_internal.h` (predecessor's `bus_internal.h` name corrected). Evidence at LOCK: 48-way contention closed (Bug #4); S10 PASS post-split; O(N²)→O(N) coalesce (Bug #3); +45% throughput.

**DF rule**: **DFK015_1** — Error, enforcing (three-tier mutex managed-facade discipline, NativeBoundary category).

### К-L16 — Simulation tick pipeline depth (D ≥ 1, configurable 1–3, default 2)

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.3 v2 (2026-05-20) → LOCKED at A'.8 (Q-N-8-1).

> Simulation tick pipeline depth: **D ≥ 1** (configurable 1-3, default 2). Simulation thread runs D ticks ahead of display thread для pipeline-managed dispatches.
>
> Cross-layer async operations (GPU compute pipeline-managed via К-L7.1, network, disk I/O) have **full pipeline-depth window** к complete без blocking simulation thread. Display thread reads results from simulation tick (CurrentSimTick - D).
>
> Pipeline drain orderly at save/pause; pipeline refill orderly at load/resume.
>
> К-L16 establishes **display latency invariant** (D × tick_period); К-L15 fast tier latency invariant (subscriber response ≤1ms) preserved independently (orthogonal latency contracts).

**Falsifiability**: D < 1 (display reading ahead of simulation); D > 3 proving empirically necessary; drain/refill ordering violated (save/pause observes mid-pipeline state); К-L15 fast-tier contract degraded by К-L16.

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/pipeline_slot.cpp` (D-slot state machine); `phase_compute.cpp` (per-tick dispatch coalescing to the async compute queue); `src/DualFrontier.Core.Interop/PipelineSlotInterop.cs:29,32,74` (`DefaultDepth = 2`, `MaxDepth = 3`, `Init(depth)`). Honesty note: the predecessor evidence record listed `SimulationLoop.cs` / `DisplayLoop.cs` / `PipelineDepthSetting.cs` — none exist. The managed loop today is the single fixed-step `GameLoop` sim thread plus the Launcher render thread ([THREADING.md](./THREADING.md)); the D-lookahead pair is carried by the slot machinery and depth constants, not dedicated loop classes.

**DF rule**: **DFK016** — Warning, enforcing (depth-constant discipline vs hardcoded D; Phase 0 «retain α» ratified per Q-L-16).

### К-L17 — Display composition multi-layer

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.3 v2 (2026-05-20) → LOCKED at A'.8 (Q-N-8-1).

> Display composition multi-layer: Display tick T composites multi-layer state where layers carry **independent latency contracts**:
> - **SimState layer**: К-L16 pipeline tail (for pipeline-managed) либо current sim state (for К-L7 sync)
> - **Intent overlay**: current input state, ≤ 16ms latency (one render frame at 60Hz)
> - **CombatFeedback layer**: К-L15 Fast tier consumers, ≤ 17ms event-к-visible (fast tier callback ≤ 1ms + render frame budget ≤ 16ms)
> - **Static layer**: loaded assets (no latency contract — static for tick duration)
>
> **Composition order**: SimState rendered first, intent + combat overlays composited on top, static last.
>
> **Framework location**: `src/DualFrontier.Application/Display/` (Application layer above Rendering/IRenderer abstraction per S-LOCK-11 К10.3 v2). Renderer interfaces preserved unchanged (К10.3 v2 did not touch renderer abstraction).
>
> **Layer registration**: Mod-registered layers declare via `[Layer(LayerType.Intent | CombatFeedback)]` attribute (Contracts/Display) + capability tokens `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}` (granular per FQN per tier per S3-Q5 + S8-Q3 pattern). Per К-L9 «Vanilla = mods», vanilla layers register through same attribute + capability pattern.

**Falsifiability**: composition order violated; layer latency contracts cross-coupled; vanilla layer privilege at registration; CombatFeedback rendering degrading the К-L15 fast-tier contract.

**Implementation artifacts**: `src/DualFrontier.Application/Display/{CompositionFramework,SimStateLayer,IntentOverlayLayer,CombatFeedbackLayer,Layer,ILayerRenderContext}.cs`; `src/DualFrontier.Contracts/Display/{LayerAttribute,LayerType}.cs`; layer-token scan `src/DualFrontier.Application/Modding/KernelCapabilityRegistry.cs:146-152` (predecessor's `Bootstrap/` path corrected).

**DF rule**: **DFK017** — Error, enforcing.

### К-L18 — Mod lifecycle quiescent state precondition

**Status / LOCK history**: AUTHORED at the К10 arc → AUTHORED at К10.3 v2 (2026-05-20) → LOCKED at A'.8 (Q-N-8-1).

> Mod lifecycle quiescent state precondition: Mod load/unload operations require:
> - **Simulation paused state** (К10.2 sim-paused stub)
> - **Pipeline slots quiescent** (all fences completed; no concurrent compute dispatches in-flight)
>
> Precondition enforced at native unload primitive (К10.3 v2 Item 41 pipeline quiescence check via `df_pipeline_is_quiescent`).
>
> UI helper layer provides programmatic API: `PauseAsync` → `WaitForQuiescenceAsync(timeout)` → mod operation → `ResumeAsync`. К10.3 v2 §9.5 unload chain extended 8-step → 9-step с Step 3.6 V (Vulkan) resource cleanup placeholder (`df_vulkan_unload_mod_resources` C ABI primitive; К10.3 v2 lands managed wrapper returning vacuous success; full implementation V-cycle / К-extensions).
>
> **Helpers-only scope** (S-LOCK-12 К10.3 v2): full settings menu / preferences UI deferred к V-cycle / К-extensions.

**Falsifiability**: mod load/unload without the precondition check; a precondition bypass (helper API succeeding without verification); the Item 42 V-cleanup placeholder regressing.

**Implementation artifacts**: `native/DualFrontier.Core.Native/src/mod_unload.cpp` (+ `include/mod_unload.h` — T0–T7 sequence, T0 = scheduler critical section); `include/pipeline_slot.h:117` (`df_pipeline_is_quiescent`); `src/DualFrontier.Application/Loop/SimulationStateController.cs` (helper API); `Application/Modding/ModMenuController.cs` (pause hook; predecessor's `Application/UI/` path corrected); `Application/Bridge/VResourceCleanup.cs` (Step 3.6 V placeholder — managed-side; predecessor's native `vulkan_mod_cleanup.cpp` does not exist). Chain detail defers to [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md).

**DF rule**: **DFK018** — DEFERRED to the К-L20 LOCK cascade; no rule on disk (predecessor "Error, Active" claim withdrawn — Annex A-4).

### К-L19 — Hardware tier commitment (Vulkan 1.3 + async compute queue)

**Status / LOCK history**: AUTHORED at the К10 arc → LOCKED at V0.B closure (2026-05-18, pre-A'.8; full implementation backing operational).

> Hardware tier commitment: **Vulkan 1.3 + async compute queue family mandate**.
>
> **Target hardware tier**:
> - NVIDIA Turing+ (GTX 16xx / RTX 20-series и newer)
> - AMD RDNA 1+ (Radeon RX 5500 и newer)
> - Intel Arc Alchemist+ (Arc A380 и newer)
>
> **Queue family usage**:
> - Async compute queue family: compute-side pipeline depth dispatches (К-L16 К10.3 v2)
> - Graphics queue: display rendering
> - Copy/transfer queue: asset transfers
>
> Kernel mandates queue family availability at startup; failure to detect async compute queue is **fail-fast condition** с user-facing diagnostic message pointing к README hardware requirements section.
>
> Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation. By Dual Frontier release timeline, target hardware tier represents majority of gaming hardware.

**Falsifiability**: hardware-tier expansion becoming empirically necessary; Vulkan 1.3 proving insufficient; the fail-fast condition bypassed (silent graphics-queue fallback).

**Implementation artifacts**: `src/DualFrontier.Runtime/Graphics/HardwareCapabilityCheck.cs:23` (`VerifyVulkanApiVersion`, fail-fast `HardwareCapabilityException`); `Graphics/VulkanDevice.cs` (async compute queue family selection); `VulkanInstance.cs:26-40`; `Runtime.cs` (`Runtime.Create` integration). Correction: the predecessor evidence record listed native `vulkan_device.cpp` / `hardware_capability_check.cpp` — device selection and capability checking are **managed** P/Invoke per the substrate L2 commitment; no device-ownership or rendering code exists in `native/`. The kernel does host the V1 compute-dispatch substrate (`compute_dispatch.cpp`, `compute_pipeline.{h,cpp}`, direct vk* calls; `CMakeLists.txt:12,99` links `Vulkan::Vulkan`) against an externally attached device (`VulkanAttachment`). Empirical baseline: AMD RX 7600S. Detail defers to [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md).

**DF rule**: **DFK019_A** — Warning, enforcing (static Vulkan API surface discipline). **DFK019.B** (hardware-tier runtime check) — DEFERRED to the hardware tier expansion cascade (audience-driven).

### К-L20 — Mod API forward-compatibility [RESERVED]

**Status / LOCK history**: RESERVED, pre-AUTHORED (target: Mod API lock milestone post-А'.9); text TBD at that deliberation (Bridge mechanism, manifest freeze, grace-period semantics, deprecation cadence).

> **FENCED (target / planned — not current truth):** post-LOCK manifestation: version-frozen `IModApi`, frozen manifest schema, a Bridge amendment mechanism, `MOD_API_CONTRACT.md` authored at that milestone (no such document exists today). The DFK020 family (~20 sub-rules) and DFK009/DFK012/DFK015/DFK018 activate in the same cascade — [docs/ROADMAP.md](../ROADMAP.md) «Analyzer track».

**DF rule**: **DFK020 family** — RESERVED; deferred to the К-L20 LOCK cascade.

### Part 0 summary table

| К-L | Title | Status (LOCK cascade) | DF rule (shipped truth) |
|---|---|---|---|
| К-L1 | Native language C++20 | LOCKED (К0) | DFK001 Error, enforcing |
| К-L2 | Pure P/Invoke bindings | LOCKED (К0) | DFK002 Error, enforcing |
| К-L3 | Storage paths (α default, β opt-in) | LOCKED (К0); sub К-L3.1 | DFK003 Error, enforcing |
| К-L3.1 | Path β bridge formalization | LOCKED (А'.0) | DFK003_1 Error, enforcing |
| К-L4 | Explicit type ID registry | LOCKED (К0) | DFK004 Error, enforcing |
| К-L5 | Declarative bootstrap graph | LOCKED (К0) | DFK005 Error, enforcing |
| К-L6 | Game tick scheduler (managed) | SUPERSEDED by К-L12 (К0 → К10.1) | DFK006 never activates |
| К-L7 | Span protocol | LOCKED (К0); sub К-L7.1 | DFK007 Error, enforcing |
| К-L7.1 | GPU pipeline slot binding | LOCKED (A'.8; AUTHORED К10.3 v2) | DFK007_1 Error, enforcing |
| К-L8 | Component lifetime | LOCKED (К0) | DFK008 outside Roslyn scope |
| К-L9 | Mod parity («Vanilla = mods») | LOCKED (К0) | DFK009 DEFERRED (К-L20 cascade) |
| К-L10 | Decision rule (§8 metrics) | LOCKED (К0) | DFK010 PERMANENTLY DROPPED |
| К-L11 | NativeWorld production backbone | LOCKED (К8.4 v2 / А'.5) | DFK011 Error, enforcing |
| К-L12 | Full native kernel scheduling | LOCKED (A'.8; AUTHORED К10.1) | DFK012 DEFERRED (К-L20 cascade) |
| К-L13 | On-demand activation (5 wake types) | LOCKED (A'.8; AUTHORED К10.1) | DFK013 Warning, enforcing |
| К-L14 | Performance derives from cleanness (meta) | LOCKED (A'.8; AUTHORED К10.1) | DFK014 outside Roslyn scope |
| К-L15 | Native bus authority + three tiers | LOCKED (A'.8; AUTHORED К10.2); sub К-L15.1 | DFK015 DEFERRED (К-L20 cascade) |
| К-L15.1 | Three-tier independence (3-layer) | LOCKED (А'.7.x; compile-time А'.7.5) | DFK015_1 Error, enforcing |
| К-L16 | Tick pipeline depth D≥1 (default 2) | LOCKED (A'.8; AUTHORED К10.3 v2) | DFK016 Warning, enforcing |
| К-L17 | Display composition multi-layer | LOCKED (A'.8; AUTHORED К10.3 v2) | DFK017 Error, enforcing |
| К-L18 | Mod lifecycle quiescent state | LOCKED (A'.8; AUTHORED К10.3 v2) | DFK018 DEFERRED (К-L20 cascade) |
| К-L19 | Hardware tier (Vulkan 1.3 + async compute) | LOCKED (V0.B) | DFK019_A Warning, enforcing; DFK019.B deferred |
| К-L20 | Mod API forward-compatibility | RESERVED (post-Mod API lock) | DFK020 family RESERVED |

Shipped analyzer census (2026-07-01, ANALYZER_RULES §1.1): **17 detecting rules** at Release 1.0 severities (11 Error + 5 Warning build-breaking under `TreatWarningsAsErrors`; DFL025_B IDE-only); the deferred families have **no implementation on disk**.

---

## §1 — Bootstrap flow (two-phase, per К-L5)

**Phase N — native bootstrap (one-shot, atomic).** `DualFrontier.Core.Interop.Bootstrap.Run(useRegistry: true)` (`src/DualFrontier.Core.Interop/Bootstrap.cs`) calls `df_engine_bootstrap`: the native side builds the declarative startup task graph (`bootstrap_graph.cpp`), Kahn-sorts it, and executes it in parallel on the thread pool's Bootstrap mode (`thread_pool.cpp`; its Scheduler mode is designed to arm after `SignalEngineReady` (`transition_to_scheduler_mode`, `thread_pool.cpp:63`) — the flip is selftest-exercised only today (`selftest.cpp:1699`); [THREADING.md](./THREADING.md)). Single-entry atomic: a fully initialized world handle returns, or the native side rolls back completely and returns null → `BootstrapFailedException`. `useRegistry: true` (production default) constructs the `ComponentTypeRegistry` against the returned handle (§3).

**Phase M — managed composition.** Production entry point: `GameBootstrap.CreateLoop(PresentationBridge bridge, string modsRoot = "mods")` (`src/DualFrontier.Application/Loop/GameBootstrap.cs:70`). Verified sequence:

1. `Bootstrap.Run` → `NativeWorld` (`:76`); `VanillaComponentRegistration.RegisterAll(nativeWorld.Registry!)` (`:77`).
2. `GameServices` + `TickScheduler` (`:79-80`); domain-event → render-command bridge subscriptions (`:82-93`).
3. World content: `NavGrid` + obstacles, `AStarPathfinding`, pawn/item factories, `PublishItemSpawnedEvents` (`:95-129`); sizes/seeds are hardcoded consts (`:58-68`) — no configuration contract exists (session report N-20).
4. Ten-element `coreSystems` array (`:131-143`).
5. **Dual scheduler registration** (`:145-181`): managed `DependencyGraph` build (`:145-148`); native `SystemGraphInterop.RegisterSystem` with empty read/write id sets, priority class 2, wake type 0 (`:160-176`); `SubscribeTimer(i, 1)` (`:179`); `ComputeStaticGraph()` (`:181`). See Part 0 К-L12 wiring.
6. Mod stack (`ModLoader`/`ModRegistry`/`ModFaultHandler`/`SystemMetadataBuilder`, `:183-190`); `ParallelSystemScheduler` (`:192-199`); `ModIntegrationPipeline` + `DefaultModDiscoverer` + `ModMenuController` (`:201-206`).
7. `ManagedBusBridge` (`:212`), `GameLoop` (`:214`), pause wiring, returned `GameContext` (`:219`).

Mod (re)load happens post-bootstrap through `ModIntegrationPipeline.Apply` under the К-L18 quiescence protocol ([MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md)); the native bootstrap graph is startup-only and never rebuilds.

**Known ownership gap (current truth).** The `NativeWorld` from `:76` remains a `CreateLoop` local: `GameContext` (`GameContext.cs:19`) does not carry it, `df_world_destroy` is never called from Application or Launcher, disposal happens only at GC finalization (`NativeWorld.cs:486-500`), and `GameLoop.Stop` = cancel + `Thread.Join(2000)` (`GameLoop.cs:73-77`) can abandon a mid-tick sim thread. Shutdown ordering law is proposed in RESOURCE_OWNERSHIP_AND_LIFETIME.md (AUTHORED draft); this paragraph records the gap as fact (session report N-19).

## §2 — Storage model: NativeWorld, spans, batches

**Single source of truth.** All production component storage is `NativeWorld` (К-L11): a managed handle wrapper (`src/DualFrontier.Core.Interop/NativeWorld.cs`) holding an opaque `IntPtr` per К-L8. Path α components live natively in structure-of-arrays stores with the K8.1 primitives (`NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>`, `InternedString`); Path β components live mod-side in per-mod `ManagedStore<T>` (К-L3/К-L3.1). The managed world survives only as the `ManagedTestWorld` fixture.

**Read protocol.** `NativeWorld.AcquireSpan<T>()` (`NativeWorld.cs:345`) is one P/Invoke returning a `SpanLease<T>` (`IDisposable`) exposing `Span` (dense data), `Indices` (entity **indices** — no versions), `Count`; iteration is zero-P/Invoke. Multiple spans — including re-acquisition of one type — may be held concurrently. While any span is held, native mutation of that store is rejected (active-span counter, `world.cpp`) — the mechanical half of К-L7 atomic-from-observer.

**Write protocol.** Mutations accumulate in `WriteBatch<T>` from `NativeWorld.BeginBatch<T>()` (`NativeWorld.cs:376`) and cross the boundary in one flush (auto-flush on dispose), applying only after affected spans release, in recorded order. Zero-allocation reads + batched writes are the published Path α performance contract; Path β publishes Dictionary-shaped lookup with no such guarantee — visible per-call via the dual `SystemBase` API (К-L3.1).

**Entity identity across the span surface — the corrected teaching.** `EntityId` is `readonly record struct (int Index, int Version)` (`EntityId.cs:21`); the native ABA guard is real — `is_alive` requires `id.version == versions_[id.index]`, `destroy_entity` bumps the version (`world.cpp:74-78,90`). Spans expose **indices without versions** today; there is no correct way to reconstruct a full `EntityId` from a span alone:

```csharp
using SpanLease<HealthComponent> lease = world.AcquireSpan<HealthComponent>();
for (int i = 0; i < lease.Count; i++)
{
    ref readonly HealthComponent h = ref lease.Span[i];  // data access: zero-cost
    int entityIndex = lease.Indices[i];                   // an INDEX, not an identity
    // Do NOT construct new EntityId(entityIndex, 0) — a fabricated version
    // nullifies the ABA guard (valid only until the slot recycles).
    // Carry EntityIds received from the world (CreateEntity, events, component
    // fields); answer aliveness through the world:
    //     world.IsAlive(id)               // NativeWorld.cs:133 — version-compared
    //     world.TryGetComponent<T>(id, …) // fails closed on stale versions
}
```

The predecessor §1.7 taught `new EntityId(lease.Indices[i], 0)` and production followed it (nine systems plus `GameBootstrap.cs:241`; `SpanLease.Pairs` fabricates `Version = 1` with an honest caveat, `SpanLease.cs:76-84,112`) — a defect, not a convention (session verdicts C10/N-22). The fix — a parallel read-only versions view (`df_world_acquire_versions`) under the same acquire/release discipline, plus the `EntityId.IsValid`/native `is_alive` alignment — is specified in IDENTITY_AND_ABI_CONTRACT.md (AUTHORED draft) §2. Until that ratifies, the rule stands: **managed code must not construct an `EntityId` whose `Version` it did not receive from the world.**

**Fields.** Tile-field storage (`RawTileField`, K9) is a separate dense surface with its own string-id registry and span discipline (`src/DualFrontier.Core.Interop/{FieldRegistry,FieldHandle}.cs`, native `tile_field.cpp`) — contract in [FIELDS.md](./FIELDS.md); GPU-side writes bind to pipeline slots per К-L7.1.

## §3 — Component type-id registry

`ComponentTypeRegistry` (`src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs`) is the К-L4 mechanism, one instance per `NativeWorld` (independent id spaces per world), constructed inside `Bootstrap.Run` and exposed as `NativeWorld.Registry` (`NativeWorld.cs:62`):

- **Sequential ids** — 1, 2, 3, …; `_nextId = 1`, **0 reserved for invalid** (`ComponentTypeRegistry.cs:28`).
- **Idempotent** — re-registering returns the existing id without touching native state; failed native registration rolls the counter back and throws `InvalidOperationException` (construction category, §5).
- **Deterministic per mod load order** — auditable, no collisions (vs the rejected FNV-1a derivation); load order matters for id stability across runs, so ModLoader must process mods deterministically.
- **Registration surfaces** — vanilla: `VanillaComponentRegistration.RegisterAll` at bootstrap; mods: `IModApi.RegisterComponent<T>()` (`IModApi.cs:38`); Path β classes go through `RegisterManagedComponent<T>` (`IModApi.cs:59`), which consumes no native id.

Load-order-dependent numeric ids **must not cross the save boundary** — the save-side law (persist an FQN→id map, translate at load) is proposed in IDENTITY_AND_ABI_CONTRACT.md (AUTHORED draft) §1 note 2, jointly with PERSISTENCE_SNAPSHOT_CONTRACT.md (AUTHORED draft). Event type ids are a distinct FNV-1a-of-FQN space owned by the bus; subscription ids are the tier-bit `uint64` space of К-L15.1 — [EVENT_BUS.md](./EVENT_BUS.md).

## §4 — Dependency rules (Rules 1–5)

Verified against csproj truth at HEAD; the table under Rule 3 is the enforceable statement.

**Rule 1 — kernel standalone.** `DualFrontier.Core.Native` (C++) contains no project-specific business logic: pure ECS storage, scheduler/bus/field primitives, bootstrap graph, thread pool; C++20 stdlib plus the Vulkan SDK link required by the V1 compute substrate (`CMakeLists.txt:12,99`) — no other third-party dependency. It could compile and ship standalone.

**Rule 2 — federated P/Invoke surface.** All P/Invoke to `DualFrontier.Core.Native.dll` lives in `DualFrontier.Core.Interop`; all P/Invoke to OS/driver DLLs (vulkan-1, user32, kernel32) lives in `DualFrontier.Runtime.Native` (`VkApi.cs`, `Win32Api.cs`). No other project declares `[DllImport]` (verified census). This is the DFK002 federated model (A'.9.1 Phase β §8); the predecessor's single-surface wording predates the Runtime split.

**Rule 3 — strictly-downward references.** The project graph is acyclic and layered; each project references only what the table lists (csproj-verified):

| Project | References |
|---|---|
| Contracts | — (bottom; the mod-visible surface) |
| Core.Interop | Contracts |
| Components | Contracts, Core.Interop |
| Core | Contracts, Core.Interop |
| Events | Contracts, Components |
| Systems | Contracts, Core, Components, Events, AI |
| AI | Contracts, Components |
| Persistence | Contracts, Components |
| Crypto.Future | — (reserved; no references) |
| Runtime | Core.Interop |
| Application | Contracts, Core, Components, Core.Interop, Events, Systems, AI |
| Launcher | Application, Runtime (+ copies the native DLL) |

Domain projects reach the kernel through Core.Interop wrapper types (`NativeWorld`, `SpanLease<T>`, `WriteBatch<T>`), never through raw `NativeMethods` (`internal`). The predecessor's "Domain projects don't know about Native existence — they see only `IGameContext`" is retired: no `IGameContext` type exists, and `Components → Core.Interop` is a legal, table-recorded edge (`InternedString`/`NativeMap` field types). Whether domain data structs *should* reference the interop assembly is an open design question (`Kernel.Abstractions` extraction, session report §8 AD-4), not a rule violation.

**Rule 4 — mods compile against Contracts only.** Every mod csproj (all vanilla + Example) references exactly `DualFrontier.Contracts` (verified); mods reach the kernel exclusively through `IModApi` (implementation: `RestrictedModApi`), and the mod ALC provides per-mod collectible isolation plus shared-ALC type identity — it does **not** police references (`ModLoadContext.cs:29-54`; no refusal list — MODDING.md §5); the Contracts-only surface is compile-time convention (ARCHITECTURE.md §2).

**Rule 5 — reverse-invocation discipline.** Native never invokes managed code except through the registered batched C ABI trampoline (`[UnmanagedCallersOnly]` `ManagedSystemDispatcher.OnBatch`, registered via `SchedulerAdapter.Register`) — the single sanctioned reverse path required by К-L12/К-L15 batched-callback dispatch. Ad-hoc reverse P/Invoke, synchronous per-system callbacks, and unregistered function pointers remain forbidden. (Replaces the predecessor's absolute "no callbacks к managed" Rule 5 and its Part 8 "never reverse P/Invoke" restatement, which contradicted Part 0 К-L12 with zero in-document reconciliation — Annex A row A-1. Anchors: `ManagedSystemDispatcher.cs:75`, `SchedulerAdapter.cs:22`.)

## §5 — C ABI conventions (error semantics)

The Interop layer has two surfaces: the C ABI (`df_capi.h`, `bus_native.h`, `event_type_registry.h`, `mod_unload.h`, `pipeline_slot.h` and peers — 205 `DF_API` declarations across the `include/` headers at HEAD) and the managed bridge wrappers.

**C ABI surface (immutable law).** Every `extern "C"` function returns a status code or sentinel — shipped convention: `0` = failure / not found, `1` = success / present, out-of-range inputs return 0 rather than crashing (`df_capi.h:38-41`) — and swallows all exceptions via `catch (...)` at the boundary (72 sites in `capi.cpp`). The ABI never propagates C++ exceptions across the DLL boundary; the managed side never relies on native exception propagation. Non-negotiable: an uncaught C++ exception across the boundary is undefined behaviour. Reverse-direction symmetry: `[UnmanagedCallersOnly]` callbacks absorb every managed exception before returning (`ManagedSystemDispatcher.OnBatch` catch-all).

**Managed bridge surface — the four-category rule.** Error semantics follows the abstraction's nature:

1. **Sparse** (lookup/contains — `NativeMap<K,V>.TryGet`, `NativeSet<T>.Contains`): return `bool` / `TryX`; a miss is an expected case, not an exception.
2. **Dense** (indexed, position-bound — `FieldHandle<T>.ReadCell`): throw; out-of-bounds on a dense structure is a programmer error, and `TryX` boilerplate would degrade hot iteration.
3. **Lifecycle** (Acquire/Release — `NativeWorld.AcquireSpan`, `WriteBatch`): throw on misuse (acquire-after-dispose, double-release) — always a bug; the throw signals it.
4. **Construction** (Register/Create — `NativeWorld`, registry `Register<T>`): return the handle or throw `InvalidOperationException`; a null handle would defer failure to the wrong level.

**Brief-authoring requirement** (any brief adding Interop wrappers): classify each new wrapper method sparse/dense/lifecycle/construction in its design section; match error semantics to the category; record any deviation as an explicit milestone decision. Falsifiable claim carried forward: zero unexplained departures; a wrapper fitting no category forces a fifth category or re-examination.

**Deeper protocol** — ABI version negotiation, blittable/layout law, buffer ownership, pointer validity windows, per-entry-point thread affinity, the `df_status` taxonomy (retryable / contract-violation / fatal-subsystem / fatal-process) — is specified in IDENTITY_AND_ABI_CONTRACT.md (AUTHORED draft) §3–§4, whose ratification expands this section; the shipped `1/0` convention stays frozen for shipped entry points either way.

## §6 — Threading summary

Kernel scheduling decisions (graph, runqueue, wakes, phase composition) are computed natively (`system_graph.cpp`, `wake_registry.cpp`, `thread_pool.cpp` — Bootstrap and Scheduler pool modes); production dispatch today runs on the managed facade — `ParallelSystemScheduler` per-phase `Parallel.ForEach`, `MaxDegreeOfParallelism = max(1, ProcessorCount − 2)` (`ParallelSystemScheduler.cs:90,149`), the blocking join forming the implicit phase barrier, driven by the 30 Hz `GameLoop` sim thread (`GameLoop.cs:29`). The batched-callback dispatch switch is pending per Part 0 К-L12. Everything deeper — thread census, execution contexts, barriers, storage thread-safety — is owned by [THREADING.md](./THREADING.md); the predecessor's §1.4 "pool idle / single-threaded writes / managed-scheduler-only" claims are retired as pre-К10.1 text.

---

## Annex A — «было → стало»

Ratification-critical change register for this rework: what the predecessor said (было), what this document says (стало), why, and where the code stands. Rows are the scaffold-ratified annex, embedded verbatim.

| # | Target | Was (было — verbatim, source) | Becomes (стало) | Trigger | Code anchors |
|---|---|---|---|---|---|
| A-1 | Rule 5 (§1.5 dependency rules) | "**Rule 5**: Native side has no callbacks к managed. Direction unidirectional (managed → native always)." (old KERNEL_ARCHITECTURE.md:319); Part 8: "**Direction-discipline**: managed → native always, never reverse P/Invoke" (old :897) | "**Rule 5**: Native never invokes managed code except through the registered batched C ABI trampoline (`[UnmanagedCallersOnly]` `ManagedSystemDispatcher.OnBatch`, registered via `SchedulerAdapter.Register`) — the single sanctioned reverse path required by К-L12/К-L15 batched-callback dispatch. Ad-hoc reverse P/Invoke, synchronous per-system callbacks, and unregistered function pointers remain forbidden." Part-8-equivalent restated to match. | Session report §3 C3: Rule 5 vs К-L12 batched callbacks, zero in-doc reconciliation | src/DualFrontier.Application/Scheduler/ManagedSystemDispatcher.cs:75; SchedulerAdapter.cs:22 |
| A-2 | К-L12 row (Part 0) | Canonical text K_CLOSURE_REPORT.md:497-501 (three blockquote paragraphs: "Native kernel scheduling: sovereign per-tick scheduling…"; "Kernel scheduling decisions made natively: dependency graph construction, runqueue maintenance, wake-up dispatch, phase composition, parallelism scheduling, priority arbitration, resource quota enforcement."; "Cross-layer communication uses C ABI with batched callbacks… К-L6 SUPERSEDED by К-L12 — managed scheduler facade preserved for К-L9…"); "Production manifestation" list naming scheduler_native.cpp/scheduler_graph.cpp/scheduler_runqueue.cpp/scheduler_wake.cpp/ManagedSchedulerFacade.cs with no unwired caveat (K_CLOSURE:508-514) | Canonical blockquote preserved VERBATIM. Appended subsection "**Current/target wiring (2026-07-15)**": CURRENT — systems registered twice (GameBootstrap.cs:145-181); native SystemGraph receives empty read/write sets + TimerWake rate 1; phases planned by managed DependencyGraph; executed via Parallel.ForEach (ParallelSystemScheduler.cs:149); batched-callback adapter on disk, test-exercised only. TARGET — sovereignty per canonical text; cutover gates + DependencyGraph deletion trigger per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft). Falsifiability clauses bind the post-cutover end state; the pre-cutover wiring above is the sanctioned migration state, not a falsification. Artifact list relabeled "Implementation artifacts (on disk; dispatch switch pending)" and corrected to the real tree (system_graph.cpp, wake_registry.cpp, scheduler_intrinsics.cpp, thread_pool.cpp; SchedulerAdapter.cs/ManagedSystemDispatcher.cs). | Session report §3 C1 + §6.1 N-1 (K_CLOSURE §2.14 "Production manifestation" overstatement; THREADING truth) | GameBootstrap.cs:145-181; ParallelSystemScheduler.cs:149; native/DualFrontier.Core.Native/src/system_graph.cpp |
| A-3 | К-L15 row (Part 0) | Canonical text K_CLOSURE_REPORT.md:577-584 (blockquote: "Native bus authority: native kernel owns sovereign event routing…"; three-tier fast/normal/background definitions; "Managed bus facade preserved as IModApi-exposed surface (К-L9 uniformity)…") + Production manifestation incl. BusFacade.cs (K_CLOSURE:592-600) | Canonical blockquote preserved VERBATIM. Appended subsection "**Current/target wiring (2026-07-15)**": CURRENT — production events travel the five managed DomainEventBus instances (GameServices.cs:16-113); `BusFacade.UseNativeBusForDispatch` defaults false and no production code constructs a BusFacade (BusFacade.cs:49); live native touchpoints today: Background-tier drain each tick (GameLoop.cs:120-128 via ManagedBusBridge.DrainBackgroundBatch) and per-mod bulk unsubscribe at unload (vacuous — production registers no native subscribers). TARGET — sovereign native routing per canonical text; gates per EXECUTION_AUTHORITY_MATRIX.md §3 (AUTHORED draft). Falsifiability clauses bind the post-cutover end state. | Session report §3 C2 (authority-vs-traffic split; EVENT_BUS honesty) | GameServices.cs:16-113; BusFacade.cs:49; GameLoop.cs:120-128 |
| A-4 | Part 0 metadata rows (non-invariant) | K_CLOSURE §2 per-invariant metadata claims "DF### rule: DF012 (Error, Active)" / "DF015 (Error, Active)" etc. | DF-rule status rows corrected to the shipped analyzer truth: 17 rules shipped per ANALYZER_RULES (DFK-namespace); DFK012/DFK015-base are DEFERRED (К-L20 LOCK cascade); Active claims removed where no rule exists on disk. Invariant texts untouched by this row. | Session report §6.4 N-26 (13 declared-not-implemented); analyzer registry census | tools/DualFrontier.Analyzers/Rules/ census; ANALYZER_RULES deferral tables |

---

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md) | defers-to | Scheduling mechanism + cutover detail; К-L12/К-L13 texts stay here |
| [EVENT_BUS.md](./EVENT_BUS.md) | defers-to | Bus mechanics, tiers, diagnostics; К-L15/К-L15.1 texts stay here |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | defers-to | Mod lifecycle chains, capabilities; К-L18 text stays here |
| [THREADING.md](./THREADING.md) | defers-to | Thread census, execution contexts, barriers |
| [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) | defers-to | GPU substrate — the other half of the native foundation: the kernel knows nothing of rendering (no device ownership or rendering in `native/`; the kernel's Vulkan surface is the К-L7.1 compute-dispatch substrate over an attached device), the Runtime knows nothing of ECS, and the managed Application layer orchestrates both bridges |
| [ECS.md](./ECS.md) | constrains | Span teaching surface must follow §2's corrected identity example |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | constrains | Umbrella layer map must match §4's csproj-verified table |
| [ANALYZER_RULES.md](./ANALYZER_RULES.md) | cites | Shipped rule registry backing every DF-rule row |
| [FIELDS.md](./FIELDS.md) | cites | Tile-field storage contract (К-L7.1 consumer surface) |
| [K_CLOSURE_REPORT.md](K_CLOSURE_REPORT.md) | cites | Closure-evidence record; Part 0 consolidation source; §1.2 remains the К-L14 carrier per Q-N-8-2 |
| IDENTITY_AND_ABI_CONTRACT.md (AUTHORED draft) | cites | Version-0 resolution + full ABI protocol/error taxonomy — ratification expands §2/§5 |
| EXECUTION_AUTHORITY_MATRIX.md (AUTHORED draft) | cites | Cutover gates + deletion triggers for К-L12/К-L15 target states |
| RESOURCE_OWNERSHIP_AND_LIFETIME.md (AUTHORED draft) | cites | Shutdown/dispose-order law for the §1 ownership gap |
| [docs/ROADMAP.md](../ROADMAP.md) | cites | All forward-scheduled work (K-series record, analyzer track, cutover sequencing) |

## Amendment protocol

Amendments per [FRAMEWORK.md](../governance/FRAMEWORK.md) §7.2 (Tier 1 protocol). К-L canonical texts change only through a К-extensions cascade or an explicit supersession event recorded in the register; current-architecture sections (§1–§6) re-verify against code at each amendment HEAD. Versioning: patch = cross-ref/correction, minor = new/superseded К-L or behavioral contract change, major = structural reorganization (Q-G-12 convention). No header chronicles — history lives in git and the register.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.2 | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R1-3..R1-10): К-L19/Rule-1/cross-ref Vulkan-in-native truth (kernel hosts the V1 compute-dispatch substrate; no device ownership/rendering); Rule 4 ALC-refusal claim retired (isolation + type identity, not reference policing); Rule 3 table corrected (AI → Contracts, Components) and completed (Persistence, Crypto.Future rows); 5 stale ANALYZER_RULES §-pointers retargeted (§4.1→§1.1, §7→§4); DF_API census 154→205 declarations; К-L14 log wording (13 active, #10 vacated); thread-pool Scheduler-mode arming qualified selftest-only; version-0 fabrication census ten→nine systems. |
| 0.1.1 | 2026-07-17 | SEED-1 / F-9 resolution per HALT-1 variant (b) (CORPUS_CLOSURE_INVERSION_B): Part 0 headline recomposed keeping the ratified count 21 — К-L6 SUPERSEDED listed for traceability, excluded from the count (restores K_CLOSURE_REPORT §Part-0-composition "−1 SUPERSEDED" convention the rework had inverted); 22-row table note added. Census "21 (invariant\|final)" delta: 0. |
| 0.1.0 | 2026-07-15 | Authored rework successor of DOC-A-KERNEL v2.6.2: Part 0 consolidated as sole К-L canonical carrier (from K_CLOSURE_REPORT §2) with corrected artifact lists and shipped DF-rule truth; §1–§6 rewritten to code truth at HEAD 35364c2; Annex A (было→стало) rows A-1..A-4 applied. |