---
register_id: DOC-A-CONCURRENCY_AND_MEMORY_MODEL
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.2
first_authored: 2026-07-15
last_modified: 2026-07-18
content_language: en
next_review_due: 2027-Q3
title: Concurrency & Memory Model — owner threads, happens-before catalog, lock order, shutdown semantics (the A1 contract)
review_cadence: on-change+annual
last_review_date: 2026-07-18
last_review_event: 'EQ_A2_SHUTDOWN_TRANSACTION Cascade B — v1.0.1 → v1.0.2 PATCH: §6.2 gains a cross-reference to the abnormal-exit (fence-abort) contract now owned by ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.6 (D10). EVT-2026-07-18-EQ_A2_SHUTDOWN_TRANSACTION. Prior: EQ_A1 D2 fault-symmetry (v1.0.0 → v1.0.1); DRAFTS_RATIFICATION Phase C (AUTHORED → LOCKED v1.0.0).'
reviewer: Crystalka
special_case_rationale: 'Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-DRAFTS_RATIFICATION (item [6]). The A1 concurrency/memory model — thread census, owner-thread table, resource×operation matrix, 12-edge happens-before catalog, lock-order law, shutdown quiesce law, fault-crossing symmetry; §9.1/§9.3 conflicts already resolved in-corpus; the deferred-catch asymmetry and shutdown-fence items are the seeded engineering work orders.'
---

# Concurrency and Memory Model (the A1 contract)

> **Document class: RATIFIED-IN-FORCE forward contract (EVT-2026-07-17-DRAFTS_RATIFICATION).** The A1 concurrency/memory model, produced by the Architecture Decomposition & Contracts session 2026-07-15 ([docs/reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)). Authored against HEAD `6f39903`; code-truth re-verified at HEAD `48983c4`, 2026-07-17 (src/native unchanged). If an edge is not in the §4 catalog, cross-thread visibility is not guaranteed — that rule is now law. Where a specific point still conflicts with a LOCKED doc, the carve-out below governs.

**Forward amendment queue** (recorded in docs/ROADMAP.md; folds NOT executed by this cascade):

| Section | Deferred destination |
|---|---|
| §3 Resource × operation matrix, §4 Happens-before catalog | [THREADING.md](./THREADING.md) — the concurrency authority gains its missing formal core (MAJOR) — note §4 edge 4 is re-homed to THREADING §7, already the rule's normative owner |
| §2 Vulkan external-synchronization rows, §6 fence/teardown steps | [VULKAN_SUBSTRATE.md](./VULKAN_SUBSTRATE.md) — queue thread-affinity and shutdown-fence law (MINOR: additive) |
| §1 census, §2 owner table (non-Vulkan), §5 lock order, §6-§9 | Remain standalone here |

**Conflict carve-out (re-verified at ratification):** §9.1 (KERNEL §1.4) and §9.3 (FIELDS "non-blocking") are RESOLVED — the LOCKED successors already retired/corrected those texts. §9.4's dangling snapshot anchor is re-homed to THREADING §7 (mechanism still unbuilt). Genuinely open — the named LOCKED texts govern until amended: §9.2 dual-executor (К-L12 cutover), §9.6 bounded-queue ownership, §9.7 C-ABI per-entry thread affinity (A6), §9.8 production native teardown (a build item).

Why A1 exists: the corpus has **no** formal concurrency/memory model — zero corpus-wide hits for happens-before / memory model / lock order / data race vocabulary at HEAD `6f39903`. What exists is fragments: the implicit phase barrier and the N−2 rule (THREADING §3 (phase barrier)), the native fixed mutex order (EVENT_BUS §3 (К-L15.1 lock order)), the span mutation-rejection counters (KERNEL_ARCHITECTURE §1.7; FIELDS §5 (span-lifetime law)), the declaration-based no-concurrent-writes rule (K-L11), and a pending TLA+ item (historical/KERNEL_FULL_NATIVE_SCHEDULER.md Item 18). This document composes those fragments into one model and names every edge the fragments leave implicit. Invariant wording for K-L7/K-L7.1, K-L11, K-L12, K-L15/K-L15.1, K-L16, K-L18 remains owned by [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) Part 0.

---

## §1 Thread population census (current truth)

Every thread the production process runs today, verified against code:

| # | Thread(s) | Created by | Role |
|---|-----------|------------|------|
| T1 | **Main thread — which IS the render thread** | OS entry / `Program.Main` | Composition root; Win32 message pump; input drain (drained and **discarded** today — `Program.cs:81-84`); per-frame bridge drain + Vulkan record/present; shutdown sequencing |
| T2 | **"SimulationLoop"** — one dedicated background thread | `GameLoop.Start` | Fixed-step 30 Hz driver: `ExecuteTick`, bridge enqueue, Background-tier drain within the remaining tick budget |
| T3 | **TPL workers** — `MaxDegreeOfParallelism = max(1, ProcessorCount − 2)` | `Parallel.ForEach` per phase | System `Update` bodies inside a phase; the blocking join is the phase barrier |
| T4 | **Native kernel pool** — `hardware_concurrency()` threads, fallback 4 | native bootstrap graph | Bootstrap task execution; `Scheduler` lifecycle mode exists but production dispatch does not route through it — **mostly idle in production** |
| T5 | **GC / finalizer threads** | CLR | The only production path that destroys the native world today (`~NativeWorld`) |
| T6 | Transient `Task` pool threads | `SimulationStateController` | `PauseAsync` / `WaitForQuiescenceAsync` / `ResumeAsync` (K-L18 helpers) — lifecycle-only, never per-tick |

Anchors: T1 — `src/DualFrontier.Launcher/Program.cs:70-92` (main loop), VULKAN_SUBSTRATE §(render-thread merge) ("a single render thread merged with the OS message pump — in production this is the `DualFrontier.Launcher` main thread"). T2 — `src/DualFrontier.Application/Loop/GameLoop.cs:64-69` (thread creation, `Name = "SimulationLoop"`, `IsBackground = true`), `:29` (`TargetTps = 30`), `:112-131` (fixed-step inner loop). T3 — `src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:88-91` (MaxDoP), `:149-164` (per-phase ForEach); THREADING §3 (phase barrier). T4 — `native/DualFrontier.Core.Native/src/thread_pool.cpp:7-11`; THREADING §2 (native pool). T5 — `src/DualFrontier.Core.Interop/NativeWorld.cs:496-503`. T6 — `src/DualFrontier.Application/Loop/SimulationStateController.cs:71,82,110`.

Census implications, stated once:

- The simulation **never** runs on the main thread; the main thread **is** the render thread. No prior document states both halves in one sentence.
- Peak concurrency touching engine state = T2 + (N−2) T3 workers + T1 draining the bridge, while T4 idles and T5 may run `df_world_destroy` at any point after the last managed reference drops (§6).
- If the K-L12 cutover ever activates T4 for per-tick dispatch while TPL stays, the process oversubscribes: (N−2) + N + T1 + T2 threads on N cores (§9.2).

### 1.1 Context vocabulary

The tables below use *contexts*, not raw thread ids, because T3 workers are fungible:

- **Phase-legal context** — a T3 worker executing a system whose `[SystemAccess]` declaration covers the touched component/field/bus (THREADING §4 (SystemAccess declaration law)), with its `SystemExecutionContext` pushed. The declaration graph, not a lock, is what makes two simultaneous phase-legal writers safe (K-L11: they can never target the same component type).
- **Driver context** — T2 between phases and between ticks: the only context allowed to run `FlushDeferred`, `TickScheduler.Advance`, the Background drain, and slot acquisition.
- **Menu path** — T1 while the loop is paused via the `ModMenuController` hooks (`GameBootstrap.cs:216-217`): the only context allowed to run mod load/unload and `Rebuild`.
- **Render context** — T1 inside the frame loop: bridge drain, `SceneState`, all graphics-queue Vulkan calls.

## §2 Owner-thread table

"Owner" = the only context that may mutate the object without further synchronization. "Others" = every additional permitted toucher plus the primitive that legalizes the touch. Rows marked ⚠ carry a current-truth defect inventoried in §3/§6/§9.

### 2.1 Kernel-side objects (native)

| Object | Owner | Who else may touch it, and how |
|---|---|---|
| `dualfrontier::World` (entity index, component stores, string pool, keyed maps/sets/composites, field registry) | T2 during play — created on T1 in `GameBootstrap.CreateLoop` (`GameBootstrap.cs:70-220`) **before** `Loop.Start()`; ownership transfers at `Start` | T3 workers within a phase, legalized by the declaration-based graph — K-L11: no two systems write the same component type concurrently (К-L11, KERNEL_ARCHITECTURE.md Part 0; §1.4) — plus the span/batch atomic gates (§3.1). ⚠ T5 finalizer may destroy it concurrently (§6). The managed wrapper self-declares "Not thread-safe" (`NativeWorld.cs:22-24`) |
| Native system graph + wake registry (process singletons) | T1 at bootstrap (`GameBootstrap.cs:160-181`: `Clear`, `RegisterSystem`, `SubscribeTimer(i,1)`, `ComputeStaticGraph`) | Registration changes only at mod load/unload under K-L18 quiescence; per-tick reads by whoever drives dispatch (today: exercised by selftests, not production — THREADING §3 (dual registration; managed dispatch facade)) |
| Native bus tiers Fast / Normal / Background | Kernel (K-L15) | Any thread may publish/subscribe under the owning tier's `std::mutex`; tiers share **no** mutable state (K-L15.1 state layer — `bus_native_internal.h:57-74`); Background tier drained by T2 each tick (`GameLoop.cs:120-128`) |
| Background queue policy (size cap, strategy, saturation counter) | — | Own config mutex + relaxed atomic counter (`background_queue.cpp:118-137,199-201`) |
| Pipeline slots (K-L16, depth D=2 default) | T2 acquires and dispatches | GPU signals the fence; fence polling transitions `Dispatched → FenceCompleted`; display/sim read the `ReadableAsTail` slot. State machine `pipeline_slot.h:47-50`; acquire returns null when all D slots are in flight (backpressure, `:78-79`) |
| Per-mod native state (bus subscriptions, wake entries, quotas) | Kernel | Torn down in one critical section T0-T7 by `df_scheduler_unload_mod_native_state` (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 32), only under `sim_state == Paused && pipeline_slots_quiescent()` (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 41) |

### 2.2 Managed simulation objects

| Object | Owner | Who else may touch it, and how |
|---|---|---|
| `SpanLease<T>` | The acquiring system's T3 worker | Nobody. The pointer is valid only until `Dispose`; the lease must not escape its phase (KERNEL_ARCHITECTURE §1.7; FIELDS §5 states the same window for field spans). ⚠ Pair iteration synthesizes `Version = 1` (`SpanLease.cs:76-84,112`) — §9.5 |
| `WriteBatch<T>` | The opening system's T3 worker | Nobody; record and `Flush` on the owner thread only. Managed guards throw on misuse (`WriteBatch.cs:185-189`) |
| `DependencyGraph` / installed phase list | T1 at bootstrap (`GameBootstrap.cs:145-148`); menu path for `Rebuild` | T2 reads `_phases` every tick (`ParallelSystemScheduler.cs:177`). `Rebuild` (`:191-222`) is legal **only** while the loop is paused (`ModMenuController` hooks — `GameBootstrap.cs:216-217`). ⚠ Convention only; no runtime check |
| `TickScheduler` | T2 (`Advance` after the last phase, `:180`) | T3 workers call `ShouldRun` inside phases (read-only, memoised lookup, `:151`) |
| `GameServices` + five `DomainEventBus` instances | Constructed on T1; no single runtime owner | Subscriber lists guarded by `lock (list)` (`DomainEventBus.cs:46,71,129,153`); `Publish` from any in-phase T3 worker; `FlushDeferred` from T2 only, at the phase boundary (`ParallelSystemScheduler.cs:166-167` → `GameServices.cs:55-62`) |
| Deferred queues (one per bus) | — (MPSC by convention) | `ConcurrentQueue<DeferredItem>` (`DomainEventBus.cs:28`): many T3 producers, single T2 consumer. ⚠ Unbounded |
| Static `ModeCache` (delivery-mode per event type) | — | `ConcurrentDictionary`, read-mostly (`DomainEventBus.cs:25`) |
| `SystemExecutionContext` current-context slot | The thread it is pushed on | `ThreadLocal<T>` per scheduler thread (THREADING §(execution context)); push/pop strictly scoped in `finally` (`ParallelSystemScheduler.cs:154-163`). The async ban exists because `await` breaks this affinity (THREADING §8 (async ban)) |
| `IntentBatcher` caches | Resolver system's context | Enqueue during a phase, flush as a typed snapshot at the boundary (EVENT_BUS §(IntentBatcher)) |
| `GameLoop` control flags (`_paused`, `_speedMultiplier`) | — (any controller context writes: menu hooks, pause coupling) | `volatile` fields (`GameLoop.cs:44-45`); T2 reads them each iteration (`:103,109`). Visibility-only guarantee — a write is *eventually* seen at the next loop iteration; no synchronization with the tick body (§4.11) |
| `GameContext` (record of `Loop` + `Controller`) | T1 | Immutable aggregate handle (`GameContext.cs`); note it does **not** carry `NativeWorld` — the world has no owner on the shutdown path (§6.1) |

### 2.3 Presentation & GPU objects

| Object | Owner | Who else may touch it, and how |
|---|---|---|
| `PresentationBridge` queue | — (MPSC by design: "many writers, one reader") | `ConcurrentQueue<IRenderCommand>` (`PresentationBridge.cs:21`); producers = T2 today (any domain thread by contract, `:27-31`); sole consumer = T1 per frame (`:45-52`); `QueueDepth` is an eventually-consistent diagnostic (`:62`). ⚠ Unbounded |
| `SceneState` | T1 exclusively | Written by `RenderCommandDispatcher` during drain, read by `LauncherRenderer` in the same frame — both on T1 by construction (`Program.cs:59-61`, S-LOCK-10) |
| `InputQueue` | T1 (pump + drain, `Program.cs:76-84`) | Nobody else today; forwarding to the Domain is an unwired future cascade |
| Vulkan objects: instance, device, swapchain, graphics queue, per-frame semaphores/fences, sprite/atlas resources | T1 | **Vulkan external synchronization**: a `VkQueue` and most `Vk*` handles admit one owning thread at a time. Graphics queue = T1 only. **Async compute queue = native side, driven from T2** via `df_world_field_dispatch_compute` (VULKAN_SUBSTRATE §5.1 (async-compute affinity)). The two queues must never swap threads without an explicit handoff fence. `vkDeviceWaitIdle` policy per VULKAN_SUBSTRATE §5.4 (the successor's census: shutdown-path calls plus swapchain recreation; no save path exists yet) (`LauncherRenderer.cs:199-205`) |
| Compute pipeline registry + Vulkan attachment inside `World` | T2 (attach, register, dispatch) | Read by the native dispatch path; gated by `has_vulkan_attached` (`world.cpp:41-43`) |

### 2.4 Mod & lifecycle objects

| Object | Owner | Who else may touch it, and how |
|---|---|---|
| ALCs (shared + per-mod collectible) | Menu path on T1 | Load/unload only under K-L18 quiescence (К-L18); unload chain per MOD_OS_ARCHITECTURE §9.4 (`ModIntegrationPipeline.cs:60-72`); step 7 observes finalization via a `WeakReference` spin (`:115-120`) |
| `ModRegistry`, `ModSubScheduler`, recorded mod subscriptions | Menu path on T1 | Read by scheduler/buses during play; mutation while un-paused is a violation (same rule as `Rebuild`) |
| `ManagedBusBridge` (GCHandles, reverse-P/Invoke callback registrations) | T1 at construction (`GameBootstrap.cs:212`) | `DrainBackgroundBatch` called from T2 (`GameLoop.cs:127`); ⚠ `df_bus_clear` reachable only through `ClearForTesting` (`ManagedBusBridge.cs:129-131`) — no production teardown (§9.8) |

## §3 Resource × operation matrix

Format: resource | operation | allowed context | synchronization primitive | on violation **today** (throw / UB / silent). The violation column is current truth; the normative rule follows the tables.

### 3.1 World storage: spans, batches, structural mutation

| Resource | Operation | Allowed context | Primitive | On violation today |
|---|---|---|---|---|
| Component stores | `add/remove_component`, `destroy_entity`, `flush_destroyed`, `add_components_bulk` | One phase-legal writer (K-L11 declaration graph); **no** active spans or batches | `active_spans_` / `active_batches_` atomics, acq/rel (`world.h:167-168`) | Native `std::logic_error` (`world.cpp:84-119,150-159,177-183`) → swallowed at the C ABI (`catch (...)`, KERNEL:759) → status 0 → managed `void` wrapper drops the status → **silent no-op** (`NativeWorld.cs:337` documents "silently rejected") |
| Component stores | `acquire_span` / `release_span` | Phase-legal reader; multiple concurrent readers explicitly allowed (KERNEL §1.7:426) | Counter `fetch_add(acquire)` / `fetch_sub(release)` (`world.cpp:231-262`) | Release underflow silently clamped to 0 (`world.cpp:258-261`) — double-release is masked |
| `WriteBatch<T>` | record Add/Remove/Destroy | Owner thread, batch open | Single-owner handle | Managed throw: `ObjectDisposedException` / `InvalidOperationException` (`WriteBatch.cs:185-189`) |
| `WriteBatch<T>` | `Flush` (or dispose auto-flush) | Owner thread; **no active spans** — the flusher's own batch is exempt from the batch gate | `active_spans_` gate inside `add/remove_component_unchecked` (`world.cpp:121-133,161-170`) | Native throw → ABI 0 → managed `InvalidOperationException` (`WriteBatch.cs:129-130`) — the one honest lifecycle path |
| Entity index | `create_entity` | Same single-writer discipline as mutation (not span-gated) | None beyond phase legality | Concurrent create from two writers = **data race / UB** (`world.cpp:57-72` — unguarded `free_slots_`/`versions_`) |
| All const accessors | `is_alive`, `get/has_component`, bulk get, counts | Any context | Generation check gates every accessor (`world.cpp:74-78,89-218`) | Dead or stale id → `false` / zero-fill — silent **by design** (sparse-category semantics, KERNEL Part 7:763) |
| String pool / keyed maps / sets / composites | intern, resolve, get-or-create, element ops | Same single-logical-writer discipline as the owning `World` (K8.1 primitives are world-owned; wrapper id counters are `Interlocked` "for future thread-safety", `NativeWorld.cs:33-42`) | None native-side beyond phase legality | Concurrent structural mutation = **data race / UB**; stale-generation resolve → `null` (honest, `NativeWorld.cs:578-582`) |

### 3.2 Field storage (K9)

| Resource | Operation | Allowed context | Primitive | On violation today |
|---|---|---|---|---|
| Tile field | `write_cell`, `set_conductivity`, `set_storage_flag`, `swap_buffers` | Phase-legal writer; no active field spans | Per-field `active_spans_` atomic (FIELDS §5 (span-lifetime law)) | `std::logic_error` caught at the ABI, reported as return 0 (FIELDS §5) — visible only if the caller checks |
| Tile field | `acquire_span` (read) | Phase-legal reader | Same counter; pointer valid until release or a failed mutation (FIELDS §5) | — |
| Tile field | `dispatch_compute` | T2 only | GPU fence — **not** `active_spans_` (FIELDS §10 (CPU/GPU exclusion)); the K-L7 sync path blocks until the fence signals | Dispatch from any other thread races the async compute queue — **UB** (Vulkan external synchronization) |

### 3.3 Event buses

| Resource | Operation | Allowed context | Primitive | On violation today |
|---|---|---|---|---|
| Managed bus | `Publish` (sync / `[Immediate]`) | Any in-phase system context | `lock (list)` snapshot, handlers invoked outside the lock (`DomainEventBus.cs:152-154`) | Handler fault caught + logged per subscriber, delivery continues (`:156-166`) |
| Managed bus | `Publish` (`[Deferred]`) | Any in-phase system context | `ConcurrentQueue.Enqueue` (`:96-99`) | None — queue unbounded, no backpressure |
| Managed bus | `FlushDeferred` | T2 at the phase boundary **only** | Convention (sole call site `ParallelSystemScheduler.cs:166-167`) | No check. ⚠ Handler fault **escapes** — §7 |
| Managed bus | `Subscribe` / `Unsubscribe` | Any context (systems: `OnInitialize`/`OnDispose` — EVENT_BUS §(subscription lifecycle)) | `lock (list)`, delegate value-equality dedupe (`DomainEventBus.cs:46-57`) | — |
| Native bus | `df_bus_publish_fast` | Any thread | Fast mutex for subscriber snapshot only; callbacks fire on the publisher's thread outside any mutex (EVENT_BUS §3 (fast-tier delivery)) | ≤1 ms subscriber budget unenforced by the bus; `FastTierContractMonitor` is advisory |
| Native bus | publish normal / background | Any thread | Owning tier mutex (EVENT_BUS §3) | — |
| Native bus | `df_bus_drain_normal_batch`, `df_background_queue_dispatch_idle_slot` | The driving thread (T2 today) | Queue swap + subscriber snapshot under the tier mutex, dispatch outside it (`background_queue.cpp:145-152`) | Budget exhaustion → remainder correctly requeued (`:162-170`) |
| Background queue | publish path size control | — | ⚠ **The 10 MB drop-oldest cap is applied only inside `force_coalesce` (`background_queue.cpp:312-332`), never on publish or idle-slot dispatch**; BACKPRESSURE / EXPAND strategies are accepted as parameters but unimplemented (`:124-130`) | Silent unbounded growth until someone calls `force_coalesce` |
| Background queue | save serialize / deserialize (Item 31 wire format) | Save path — must run under quiescence (no concurrent publish is *excluded* only by the tier mutex per call, not across the size+write pair) | Tier mutex per ABI call (`background_queue.cpp:229-310`) | `compute_save_size` and `serialize` are two lock acquisitions — a publish between them makes `serialize` return 0 (honest failure, caller must retry under quiescence) |

### 3.4 Pipeline slots and presentation

| Resource | Operation | Allowed context | Primitive | On violation today |
|---|---|---|---|---|
| Pipeline slot | acquire (`Empty/ReadableAsTail → Dispatched`) | T2 at pipeline-managed tick start | Slot state machine (`pipeline_slot.h:47-50,78-79`) | All D slots in flight → null (K-L16 backpressure — correct refusal, not error) |
| Pipeline slot | `check_fences` (`Dispatched → FenceCompleted`), tail publish (`→ ReadableAsTail`) | Fence-polling driver | VkFence + transition wake hook (THREADING §2 (fence-transition wake)) | Out-of-order transitions unrepresentable by construction |
| `PresentationBridge` | `Enqueue` | Any domain thread (T2 today) | `ConcurrentQueue` | Null command → `ArgumentNullException` (`PresentationBridge.cs:29`); no depth cap |
| `PresentationBridge` | `DrainCommands` | T1 only ("Called ONLY from the main thread of the active render backend", `:34-39`) | Convention | No check — a second drainer would silently interleave |
| Scheduler | `Rebuild` phase-list swap | Menu path, loop paused | Convention + K-L18 for the mod path | Undetected race on `_phases` if violated |

**Normative rule for the silent rows.** Every "silent" cell above is a latent-bug factory. Target semantics on ratification: the C ABI keeps status codes and `catch (...)` — that convention is immutable (KERNEL_ARCHITECTURE §5 (C ABI conventions)) — but **every managed wrapper must surface a failed status as a throw** (lifecycle category per the four-category rule, KERNEL_ARCHITECTURE §5), never drop it. `NativeWorld.AddComponent`/`RemoveComponent`/`DestroyEntity`/`FlushDestroyedEntities` returning `void` over a dropped status is the first amendment target.

## §4 Happens-before catalog (normative edges)

Notation: *A ➜hb B, established by M*. If an edge is not in this catalog, cross-thread visibility is **not guaranteed** and no code or document may assume it.

1. **Phase barrier.** Every system `Update` in phase P ➜hb everything sequenced after P — established by the `Parallel.ForEach` join blocking T2 (`ParallelSystemScheduler.cs:149-164`; THREADING §3 "the blocking semantics of `Parallel.ForEach` form the implicit phase barrier"). Native mirror: per-phase barrier semantics, `Full` by default (THREADING §2 (barrier types)).
2. **Deferred flush after the barrier.** A `[Deferred]` publish during phase P ➜hb its handler invocation — established by edge 1 plus `FlushDeferred` running on T2 strictly after the join (`ParallelSystemScheduler.cs:166-167`); the `ConcurrentQueue` enqueue/dequeue pair supplies the memory edge. Handlers run under the subscriber's captured context (EVENT_BUS §2 (captured-context law)); events published from inside a drain are delivered on the **next** flush (snapshot re-entrancy — `DomainEventBus.cs:111-113`).
3. **Batch flush → next-phase visibility.** Commands recorded in a `WriteBatch` ➜hb component reads in the next phase — established by `Flush` on the owner thread before the phase ends (`WriteBatch.cs:122`), then edge 1. Within the *same* phase the batch is invisible: "batch commands do NOT show up until after `Flush`" (`WriteBatch.cs:196`).
4. **Tick finalization → snapshot copy → next-tick read.** Tick-N writes to a cycle-closing component ➜hb tick-N+1 reads of its snapshot — established by the previous-tick snapshot mechanism defined normatively in THREADING §7 (feedback cycles and previous-tick snapshots) — "any read that closes a cycle must go through a `_Previous` snapshot of the previous tick" — plus edge 1. Gameplay applications only: FEEDBACK_LOOPS (Category J, mechanics) §2/§6. ⚠ The establishing copy pass is unbuilt — §9.4.
5. **Fence signal → span read of GPU results.** A compute dispatch's device writes ➜hb subsequent CPU reads — established on the K-L7 sync path by blocking until the fence signals (VULKAN_SUBSTRATE §5.1: "the К-L7 sync path returns after the fence signals, so a subsequent `FieldHandle<T>.ReadCell` sees the dispatched result"); on the opt-in K-L7.1 pipeline path by the `FenceCompleted → ReadableAsTail` slot transition, yielding slot-tail state with a bounded one-tick lag (К-L7.1/К-L16; `pipeline_slot.h:47-50`). Cross-slot reads see different snapshots — the atomic-from-observer invariant holds only within a slot boundary (K-L7.1).
6. **Bridge enqueue → render drain.** `PresentationBridge.Enqueue` on T2 ➜hb that command's dispatch inside T1's `DrainCommands` — established by `ConcurrentQueue` (`PresentationBridge.cs:27-52`). Composed with program order in `RunLoop` (`GameLoop.cs:115-116`: `ExecuteTick` then `Enqueue(TickAdvancedCommand)`), tick N's completed simulation state ➜hb the render thread observing `TickAdvancedCommand(N)`.
7. **Subscription add → next publish (snapshot semantics).** `Subscribe` ➜hb the first `Publish`/`FlushDeferred` whose subscriber snapshot is taken after the subscribe — established by `lock (list)` in both `Subscribe` and the snapshot copies (`DomainEventBus.cs:46,129,153`). A publish racing a subscribe may legitimately miss the new subscriber. **No stronger guarantee is offered, and none may be assumed.**
8. **ALC unload → finalization.** All per-mod detachment — recorded managed unsubscribes, `ModSubScheduler` removal, native per-mod teardown (T0-T7 single critical section) — ➜hb `AssemblyLoadContext.Unload` ➜hb collection/finalization of mod state — established by the §9.4 chain order (`ModIntegrationPipeline.cs:60-72`; MOD_OS_ARCHITECTURE §9.4) executing under the K-L18 precondition `sim_state == Paused && pipeline_slots_quiescent()` (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 41), with step 7's `WeakReference` poll as the finalization observation (`ModIntegrationPipeline.cs:115-120`).
9. **Native tier publish → drain callback.** A tier publish ➜hb its subscriber callback — established by the tier mutex around enqueue and the drain's locked queue swap (EVENT_BUS §3; `background_queue.cpp:145-152`).
10. **Bootstrap publication.** Everything constructed in `GameBootstrap.CreateLoop` on T1 — world population, graph, subscriptions made in `InitializeAllSystems` (`ParallelSystemScheduler.cs:104,115-133`) — ➜hb T2's first tick — established by `Thread.Start` (`GameLoop.cs:69`), which is a full publication edge in the .NET memory model. Corollary: post-`Start` construction on T1 has **no** edge to T2; the composition root must not mutate simulation state after `Loop.Start()` (`Program.cs:66`).
11. **Tick counter.** `TickScheduler.Advance` on T2 after the last phase (`ParallelSystemScheduler.cs:180`) ➜hb every `ShouldRun` in the next tick's phases — established by the `Parallel.ForEach` fork. The counter needs no interlocking *because* this is its only writer and the fork/join brackets every reader.
12. **Control flags (visibility-only, no ordering).** `SetPaused`/`SetSpeed` writes ➜visible-to T2's next loop-head read via `volatile` (`GameLoop.cs:44-45,80,91-92`) — but this is *not* a happens-before with the tick body: a pause request during `ExecuteTick` takes effect only at the next iteration. Anything needing tick-boundary certainty must use the K-L18 quiescence wait, not the flag (`SimulationStateController.WaitForQuiescenceAsync`).

## §5 Lock-order law

### 5.1 Lock inventory and global order

A thread holding a level-*n* lock may acquire only locks of level > *n*. Never the reverse, never sideways within a level (except the fixed tier order).

| Level | Lock | Module | Notes |
|---|---|---|---|
| 1 (leaf, managed) | `DomainEventBus` subscriber-list locks (`lock (list)`) | `DomainEventBus.cs:46,71,129,153` | **Leaf**: nothing may be acquired under them; no user code may run under them |
| 2 (native) | Tier mutexes, fixed order **fast → normal → background** when multiple are needed | К-L15.1 (KERNEL_ARCHITECTURE.md Part 0); `df_bus_clear` per EVENT_BUS §3 (К-L15.1 lock order) | Single-tier operations take only their own mutex |
| 3 (native) | Background policy config mutex | `background_queue.cpp:315-326` nests tier → config | Also legal standalone (`:132`) |
| — | Native scheduler critical section (T0-T7 unload) | `historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 32 | Entered only under K-L18 quiescence; by construction no bus/world lock is held around it |

Lock-free primitives sit **outside** the order and may be touched at any level: the `ConcurrentQueue`s (deferred, bridge), `ConcurrentDictionary` caches, the span/batch atomics, the saturation counter, and the `volatile` loop flags. They grant only the §4 edges explicitly listed for them — being "thread-safe" does not make the surrounding protocol safe (the §3.3 cap hole and §4.12 are both lock-free and both defective/limited).

### 5.2 Laws

- **No callback while holding a lock.** Both live delivery paths already conform — managed: snapshot under `lock`, invoke outside (`DomainEventBus.cs:152-154`); native fast tier: snapshot under mutex, invoke on the publisher's thread outside it (EVENT_BUS §3 (fast-tier delivery)). This graduates from implementation detail to law: any future dispatch path that invokes a subscriber under a bus lock is a violation, whatever its tier.
- **Forbidden: publishing while holding a subscriber-list lock.** Same-list re-entry survives only via `Monitor` reentrancy; cross-bus publish chains under a held list lock are a deadlock topology.
- **Forbidden: P/Invoke into the kernel while holding any managed lock that a native→managed callback can retake.** The batched callback ABI (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 15; `ManagedSystemDispatcher.OnBatch`, THREADING §3 (managed dispatch facade)) re-enters managed code from native frames. A managed lock held across `df_bus_drain_normal_batch`, idle-slot dispatch, or future scheduler dispatch is a cross-boundary lock inversion that will only manifest after the K-L12 cutover.
- **Cross-tier re-entrancy stays legal** (a Fast subscriber may publish to any tier — EVENT_BUS §3 (cross-tier re-entrancy)) precisely *because* of the no-callback-under-lock law; the pre-А'.7.x single shared mutex is the recorded counterexample.

## §6 Cancellation & shutdown semantics

### 6.1 What `Stop` guarantees today (current truth — documented as violation)

`GameLoop.Stop` = `_cts.Cancel()` then `_thread?.Join(2000)` (`GameLoop.cs:73-77`). The `Join` return value is ignored. On timeout, T2 is **abandoned** — possibly inside `ExecuteTick` with live spans, mid `WriteBatch`, or mid Background drain, all touching `NativeWorld`. `Program.Main` then continues: `gameContext.Loop.Stop(); renderer.Shutdown();` (`Program.cs:94-97`). No production code disposes `NativeWorld` (no `Dispose` call in GameBootstrap or the Launcher); destruction runs only in the finalizer (`NativeWorld.cs:496-503`). Consequence: T5 can execute `df_world_destroy` **concurrently with an abandoned T2 inside `df_world_*`** — undefined behavior; `is_alive`'s generation gate (`world.cpp:74-78`) does not protect against a freed `World`. There is no sim/native quiesce fence at shutdown, no native graph/wake teardown, and `df_bus_clear` is reachable only via `ManagedBusBridge.ClearForTesting` (`ManagedBusBridge.cs:129-131`). The full observed exit sequence, for the record:

1. `runtime.Window.IsOpen` turns false → T1 leaves the frame loop (`Program.cs:70`).
2. `gameContext.Loop.Stop()` → cancel + bounded join, result ignored (`GameLoop.cs:73-77`).
3. `renderer.Shutdown()` → `VulkanDevice.WaitIdle()` + resource disposal on T1 (`LauncherRenderer.cs:199-205`) — the GPU is fenced, but possibly **while an abandoned T2 is still dispatching compute** (step 2's timeout case).
4. Process exit; `IsBackground = true` means a still-running T2 is killed mid-instruction; otherwise T5 finalizes `NativeWorld` at some unordered point.

In short: today `Stop` guarantees **nothing** past the 2-second join — the rest of the exit path is ordered luck.

### 6.2 The law (normative target): quiesce → fence → teardown

1. **Quiesce.** Stop request → T2 finishes the in-flight `ExecuteTick` *including* the deferred flush and the Background drain, then exits `RunLoop`. The join is unbounded after cancellation. If a bounded join is ever justified, its expiry is a **fail-fast process abort with diagnostics** — never silent continuation past a live simulation thread. (The abnormal-exit contract — fail-fast WITHOUT native teardown, leak-on-abort — is owned by ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.6, D10.)
2. **Fence the GPU.** Pipeline slots quiescent — every slot `Empty` or `ReadableAsTail`, the same predicate K-L18 uses (`df_pipeline_is_quiescent`; К-L18; `pipeline_slot.h:24-25`) — then `vkDeviceWaitIdle`, per the VULKAN_SUBSTRATE §5.4 policy census.
3. **Teardown order.** Mods (`UnloadAll` under K-L18) → managed scheduler/graph release → native system graph + wake registry clear → native bus clear (`df_bus_clear`, promoted out of test-only) → `NativeWorld.Dispose()` executed **deterministically on the thread that observed quiescence** → renderer/device last.
4. **Abandoned-thread prohibition.** No thread that can reach `NativeWorld`, field storage, or a native queue may exist past step 3's dispose. The finalizer is a leak detector, not a teardown path; a finalizer-run `df_world_destroy` in a healthy shutdown is itself a defect.

`SimulationStateController.PauseAsync → WaitForQuiescenceAsync → ResumeAsync` (`SimulationStateController.cs:71,82,110`) is already the K-L18 template for mod operations; shutdown is the same protocol with teardown substituted for resume.

## §7 Exception & fault crossing rules

- **Native → managed (exists, keep).** No C++ exception crosses the DLL boundary: every `extern "C"` returns a status code or sentinel and swallows via `catch (...)` (KERNEL_ARCHITECTURE §5 (C ABI conventions) — "immutable"; `capi.cpp` throughout, including the targeted `catch (const std::logic_error&)` at `capi.cpp:392`).
- **Managed → native (normative, symmetric).** No managed exception may unwind through a reverse-P/Invoke frame: every `[UnmanagedCallersOnly]` entry point (`ManagedSystemDispatcher.OnBatch`, bus drain callbacks) must catch-all and convert to status before returning to native. UB otherwise.
- **Subscriber fault isolation MUST be symmetric across delivery modes — today it is asymmetric.** Sync delivery wraps each subscriber in try/catch and continues to the remaining subscribers (`DomainEventBus.cs:156-166`). Deferred delivery — `InvokeDeferred` — has **no catch whatsoever** (`DomainEventBus.cs:169-187`): a faulting deferred handler unwinds `FlushDeferred` → `ExecutePhase` → `RunLoop`, killing T2, and an unhandled exception on a background thread is process death. This bypasses the origin split that routes mod faults to `ModFaultHandler` (THREADING.md:120) — a mod's `[Deferred]` handler fault today crashes the game that its sync fault could not. Law: per-subscriber isolation in **both** modes; mod-origin faults route to the `IModFaultSink` in both modes; Core-origin fault policy is the recorded decision **D2** (ratified 2026-07-18; `EVT-2026-07-18-EQ_A1_FAULT_SYMMETRY`): catch, record with full context (origin, system/subscriber identity, exception) on the existing logging surface — no new sink abstraction — then **rethrow**; fail-fast preserved. A half-executed core handler leaves undefined sim state, so masking it is forbidden. This is one explicit decision, not an accident of the `[Deferred]` attribute.
- **Even the "good" mode under-reports.** The sync-path catch logs to `Console.WriteLine` (`DomainEventBus.cs:164`) — not to the `IModFaultSink` — so a caught subscriber fault is invisible to the mod fault lifecycle and to diagnostics (the managed bus keeps no counters, EVENT_BUS §7 (observability)). The symmetric-isolation law above includes the sink, not just the catch.
- **Native-side subscriber callbacks** are user code on publisher/kernel threads: batched-callback adapters isolate per batch and report status; the Fast tier's ≤1 ms budget stays advisory-monitored (`FastTierContractMonitor`, EVENT_BUS §3/§7) until a fault-handler consumer lands.
- **Realized — EQ_A1 / Cascade A (2026-07-18).** The symmetric-isolation law above is implemented at all three managed fault-crossing points through the single `SystemExecutionContext.RouteFault` D2 definition: `DomainEventBus.DeliverSync` and `InvokeDeferred` (M4), and `ParallelSystemScheduler.ExecutePhase` (M1, which additionally commits a faulted mod to the ENGINE_LIFECYCLE_AND_TRANSACTIONS §2.3 quarantine skip-set). Mod faults route to the `IModFaultSink`; core faults record-then-rethrow. The "today it is asymmetric" state above is the recon-time (pre-`da63a93`) motivation, now closed.

## §8 Verification obligations

Which rows of this model get which instrument, and what exists today:

| Model element | Instrument | Today |
|---|---|---|
| §3.1 span/batch vs mutation exclusion; §3.4/§4.5 slot state machine; §5 tier independence; §6 K-L18/shutdown quiescence precondition; scheduler deadlock/starvation/priority inversion | **TLA+** — `historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 18: 12 invariants (K-L3/4/5/7/7.1/8/11/12/13/15/16/18), safety CI gate (~1-2 h) + liveness models for K-L7.1 fence completion, K-L12 progress, K-L16 drain | **Pending** (K10.4 per ROADMAP; `docs/formal/` does not exist) |
| §1 context affinity (async ban); §2 span/batch escape past phase or thread; §5.2 publish-under-lock, callback-under-lock, P/Invoke-under-managed-lock; missing `Dispose` on leases/batches | **Roslyn analyzer rules** | Today: the analyzer is live, not stubbed — `DualFrontier.Analyzers` ships **17 detecting rules** (9 Architecture / 5 NativeBoundary / 3 Discipline), enforced at Release-1.0 severities since А'.9.1 Phase β/γ (2026-07-01): 11 Error + 5 Warning (build-breaking) + `DFL025_B` Info/IDE-only; waiver census pin = 2 (THREADING §8 (analyzer enforcement); ANALYZER_RULES §1.1 registry, §1.4 waiver census). **Gap:** none of the 17 covers async-context-affinity, span/batch escape, or missing-Dispose — those model elements stay unenforced (a coverage gap, not a non-detecting-stub state) |
| §5.2 cross-tier re-entrancy | **Stress test** | Exists — S10 probe, `tests/DualFrontier.Core.Tests/Scheduling/SchedulerExtremeTests.cs` (EVENT_BUS §3) |
| §6.1 shutdown abandonment (Stop during a long tick); §7 deferred-handler fault; §3.3 Background publish-path growth past the cap; §2.3 bridge/deferred queue growth under render stall | **Stress tests** | **Missing entirely** |
| All of the above | **CI** | **No CI exists at all** (no `.github/`); every obligation above is currently manual |

Priority on ratification: (1) the deferred-catch asymmetry fix + its regression test — smallest diff, largest fault-radius reduction; (2) shutdown quiesce fence — the only UB reachable by simply closing the window; (3) TLA+ Item 18 kickoff with §3 rows as the invariant source.

## §9 Open questions & conflicts with LOCKED texts

1. **KERNEL §1.4 staleness — RESOLVED.** KERNEL_ARCHITECTURE §1.4 no longer carries the pool-idle / single-threaded-writes claims: the LOCKED successor retired them as pre-К10.1 text (KERNEL_ARCHITECTURE §6 (threading summary)) and delegates thread-safety to THREADING. §1-§2 here match the successor's truth; no live conflict remains.
2. **Dual-executor ambiguity (Item 15).** In-batch execution is specified as "via TPL … Or sequential within batch if managed-side parallelism better dispatched at native level" (`historical/KERNEL_FULL_NATIVE_SCHEDULER.md` Item 15 (dual-executor)). Unresolved: after the K-L12 cutover, who owns parallelism — TPL at N−2 or the native pool at hardware_concurrency? Both live = oversubscription (§1). This document cannot resolve it; it makes the collision countable.
3. **FIELDS vs VULKAN visibility model — RESOLVED.** The FIELDS successor §10 (CPU/GPU exclusion — sync-dispatch fence semantics) corrected "non-blocking" to "Sync dispatch blocks; it does not return early", matching the K-L7 sync path (VULKAN_SUBSTRATE §5.1); §4.5 and the successor agree. The predecessor wording survives only at `historical/FIELDS.md`.
4. **Dangling snapshot anchor (re-homed).** The predecessor's "Phase 5 — Feedback snapshot" anchor is retired; THREADING §7 now owns the rule and confirms no `*Snapshot` component and no scheduler copy-pass exist (`ParallelSystemScheduler.ExecuteTick`, `.cs:175-181`). Edge §4.4 is normative-only until the copy pass is built or the rule re-anchored — THREADING §7 independently flags the same dangling state, citing this draft §9 item 4.
5. **Version-0/1 idiom weakens generation gating.** `SpanLease` pairs synthesize `Version = 1` (`SpanLease.cs:112`), systems construct `EntityId(idx, 0)`, while `is_alive` compares exact versions (`world.cpp:74-78`) — the staleness protection in §3.1 degrades to "this index was never reused". Owned by the ECS/KERNEL §1.7 discussion (KERNEL:391 "version not exposed via span").
6. **Backpressure is vocabulary, not behavior.** Bridge and deferred queues are unbounded (§2.2-2.3); the Background 10 MB cap fires only on `force_coalesce` (§3.3); BACKPRESSURE/EXPAND are accepted-but-unimplemented (`background_queue.cpp:124-130`). A bounded-queue law needs an owner (K-extensions, per the K10.2 scope note in the same file).
7. **Per-entry-point thread affinity of the C ABI is undeclared** (the A6 gap): nothing today stops `AcquireSpan` or `write_cell` from the render thread. §2-§3 propose the affinities; the A6 ABI contract must stamp them per function.
8. **No production native teardown exists to ratify against.** `df_bus_clear` is test-only (`ManagedBusBridge.cs:129-131`); graph/wake `Clear` runs only on the *next* bootstrap (`GameBootstrap.cs:160-161`); `NativeWorld.Dispose` has zero production call sites. §6.2 step 3 is therefore a build item, not a documentation item.

## See also

- [THREADING](./THREADING.md) — phase barrier, N−2 rule, async ban, fault origin split (§4/§5/§7 fold target).
- [EVENT_BUS](./EVENT_BUS.md) — managed delivery modes, native tiers, K-L15.1 lock order.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — Part 0 invariants, §1.4 threading model, §1.7 span protocol, Part 7 error semantics.
- [KERNEL_FULL_NATIVE_SCHEDULER](./historical/KERNEL_FULL_NATIVE_SCHEDULER.md) — Items 15 (batched callback ABI), 18 (TLA+), 32 (T0-T7 teardown).
- [VULKAN_SUBSTRATE](./VULKAN_SUBSTRATE.md) — render-thread merge, fence sync, `waitIdle` policy (§2/§6 fold target).
- [FIELDS](./FIELDS.md) · [FEEDBACK_LOOPS](../mechanics/FEEDBACK_LOOPS.md) · [ISOLATION](./historical/ISOLATION.md) · [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.4 · [EXECUTION_AUTHORITY_MATRIX](./EXECUTION_AUTHORITY_MATRIX.md) (the A0 contract).

## Change history

| Version | Date | Change |
|---|---|---|
| 1.0.1 | 2026-07-18 | EQ_A1_FAULT_SYMMETRY (Cascade A): §7 records the ratified D2 core-origin policy (catch → record with full context on the existing logging surface → rethrow, fail-fast preserved; `EVT-2026-07-18-EQ_A1_FAULT_SYMMETRY`) and adds the "Realized" note — the symmetric-isolation law is now implemented at all three managed fault-crossing points (M1 + M4). **PATCH** on the LOCKED 1.0.0 baseline (the 0.1.4 content was ratified LOCKED → 1.0.0 on 2026-07-17 per `EVT-2026-07-17-DRAFTS_RATIFICATION`, a lifecycle bump carrying no separate content row). |
| 0.1.4 | 2026-07-17 | DRAFTS_RATIFICATION Phase B (C3): R4-8 re-homing (snapshot rule → THREADING §7), R4-9 analyzer-truth correction, §9.1/§9.3 re-marked RESOLVED, §6.1 citation reform, historical/ KFNS prefixes. **PATCH.** |
| 0.1.3 | 2026-07-15 | R4 mechanics corpus rework (`0145f1b`): see-also link `FEEDBACK_LOOPS` → `../mechanics/` (Category J reclassification). Citation-only; no normative change. **PATCH.** (Row backfilled at DRAFTS_RATIFICATION per R4-12.) |
| 0.1.2 | 2026-07-15 | R2 platform corpus rework (`6888246`): see-also link `ISOLATION` → `./historical/` (MOD_OS merge retired it). Citation-only. **PATCH.** (Backfilled per R4-12.) |
| 0.1.1 | 2026-07-15 | R1 kernel-core corpus rework (`fe27fbe`): see-also link `KERNEL_FULL_NATIVE_SCHEDULER` → `./historical/` (retired to SUPERSEDED). Citation-only. **PATCH.** (Backfilled per R4-12.) |
| 0.1.0-draft | 2026-07-15 | Initial authored proposal (the A1 contract): thread census, owner-thread table, resource × operation matrix, happens-before catalog (12 edges), lock-order law, shutdown quiesce law, fault-crossing symmetry rule, verification obligations, LOCKED-conflict inventory. Produced by the Architecture Decomposition & Contracts session; **unratified** — see the preamble for the ratification path. |