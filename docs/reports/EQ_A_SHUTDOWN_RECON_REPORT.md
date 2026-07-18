---
register_id: DOC-E-EQ_A_SHUTDOWN_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'EQ_A_SHUTDOWN RECON REPORT — 2026-07-18 (R1–R8) — pre-brief measurement recon for the EQ-a shutdown-law family: managed fault-path census (ExecutePhase no-catch; deferred/sync bus asymmetry), shutdown-path S1–S8 verdicts, ABI export inventory + landing zones, render/device teardown, test-surface placement law, anomalies A1–A9, per-member M1–M9 work-class estimate, cascade split A/B/C/D + the D1–D10 decisions halt'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-F29_NATIVE_SCHEDULER_RECON_REPORT, DOC-E-F10_TEST_ISOLATION_RECON_REPORT). Basis of DOC-D-EQ_A1_FAULT_SYMMETRY_BRIEF; its R2/R6/R7 measurements (the managed fault-path census, the test-placement law, and the cascade split) were the load-bearing brief inputs.'
---

# EQ_A_SHUTDOWN RECON REPORT -- 2026-07-18

Pre-brief reconnaissance for the EQ-a shutdown-law family (ROADMAP "Engineering queue" row EQ-a). Read-only session; one report, zero repository mutations. Law in force (cited, not restated): RESOURCE_OWNERSHIP_AND_LIFETIME (ROL) v1.0.0 LOCKED, CONCURRENCY_AND_MEMORY_MODEL (CMM) v1.0.0 LOCKED, ENGINE_LIFECYCLE_AND_TRANSACTIONS (ELT) v1.0.0 LOCKED, IDENTITY_AND_ABI_CONTRACT (IAC) v1.0.0 LOCKED. The contracts' own code anchors were authored at `6f39903` and re-verified at `48983c4`; this recon re-measures every load-bearing anchor at the current HEAD. Where the law and the code diverge, the code is recorded as today's truth and the distance is recorded, judged by nobody here.

---

## R1 Base state

- **Branch / HEAD:** `main` @ `da63a933606644eeb7d23ba1959f5ee4c9f77c08` (`da63a93`) — matches the expected HEAD; not moved.
- **`git status --porcelain`:** empty (clean working tree; zero WIP).
- **Derived registers (read directly; `sync`/`validate` NOT run):**
  - `docs/governance/REGISTER.yaml` (DERIVED ARCHIVE): `schema_version: 2.0`, `register_version: 2.0`; **346 documents** — `rg --count-matches '^- register_id:' docs/governance/REGISTER.yaml` → 346.
  - `docs/governance/CURRENT_AUTHORITY_SURFACE.yaml` (DERIVED boot subset): `schema_version: 2.0`; **134 entries** — `rg --count-matches '^- register_id:' docs/governance/CURRENT_AUTHORITY_SURFACE.yaml` → 134.
- **AUDIT_TRAIL:** **57 EVTs** — `rg --count-matches '^- id: EVT-' docs/governance/AUDIT_TRAIL.yaml` → 57.
- **Work-order anchor — the EQ-a row, verbatim (`docs/ROADMAP.md:943`):**

> | EQ-a | **Shutdown-law implementation** | ROL §4 (S1–S8) + §6.2 checked-destroy + CMM §6–§7 + ELT §2.6/§3/§4 — the N-19 gap family; adjacent to F-40's mod_unload-globals guard | `ExecutePhase` per-system catch + origin dispatch; `UnloadAll` first production caller; world-shutdown transaction (Join checked, deterministic `NativeWorld.Dispose`, teardown order S2→S8); deferred-handler catch symmetry + regression test (CMM's stated top priority); `df_world_active_span_count` export + `df_world_destroy_checked` ABI; `EngineSession : IDisposable` adoption (ROL §4.4 sketch); swapchain prepare-before-reclaim + failure-injection test (ELT §2.5); Degraded surface (ELT §4.1); production native teardown (`df_bus_clear` promotion); device-lost v1 decision (ELT OQ-3 / IAC §7-7) |

- **F-40 ledger row location:** `docs/ROADMAP.md:1022` (quoted in R3.7).

---

## R2 Managed fault paths (catch semantics + deferred asymmetry, verbatim quotes)

### R2.1 Phase-execution paths — census

`rg -n 'ExecutePhase|ExecuteTick' src` → 10 lines; exactly **one implementing class**: `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/ParallelSystemScheduler.cs:144` `ExecutePhase`, `:175` `ExecuteTick`). Sole production driver: `GameLoop.RunLoop` → `_scheduler.ExecuteTick(FixedDelta)` (`src/DualFrontier.Application/Loop/GameLoop.cs:115`). The second dispatch path is native-driven (R2.3).

### R2.2 `ExecutePhase` — exact current exception semantics

**No catch of any kind.** `ParallelSystemScheduler.cs:149-164`, verbatim:

```csharp
Parallel.ForEach(phase.Systems, _parallelOptions, system =>
{
    if (!_ticks.ShouldRun(system))
        return;

    SystemExecutionContext ctx = _contextCache[system];
    SystemExecutionContext.PushContext(ctx);
    try
    {
        system.Update(delta);
    }
    finally
    {
        SystemExecutionContext.PopContext();
    }
});
```

The only `try/finally` pops the isolation context — nothing is caught. The class doc states the policy verbatim (`ParallelSystemScheduler.cs:30-35`):

> Exceptions from `SystemBase.Update` are not caught here — they propagate through `Parallel.ForEach` as an `AggregateException`. This is deliberate: during development we want system faults to surface immediately. Mod-fault handling lives in the Application layer (realized — `ModFaultHandler`, fed via `ModLoader.HandleModFault`); Core-system exceptions propagate.

**What is left behind on a throw:** the `AggregateException` unwinds `ExecutePhase` → `ExecuteTick` (`:175-181` — `_ticks.Advance()` is skipped) → `GameLoop.RunLoop` (`GameLoop.cs:94-139`, **no catch**) → unhandled exception on the "SimulationLoop" thread → **process death**, mod and core origin alike. The deferred flush at the phase boundary (`:166-167` `flusher.FlushDeferred()`) is also skipped, stranding queued events.

**Origin-dispatch data already exists at the insertion point:** `BuildContext` reads `SystemOrigin origin` / `string? modId` from the metadata table (`ParallelSystemScheduler.cs:248-254`) and bakes them plus `_faultSink` into each `SystemExecutionContext` (`:256-264`). A per-system catch has origin, modId, and sink in scope with zero new plumbing.

**Fault-sink feed route is fully dormant:** `ModFaultHandler.ReportFault` (`src/DualFrontier.Application/Modding/ModFaultHandler.cs:65-72`) only adds to a set; its sole documented feeder `ModLoader.HandleModFault` (`src/DualFrontier.Application/Modding/ModLoader.cs:303-309`) has **zero invocation sites repo-wide** — `rg -n 'HandleModFault' src tests -g '*.cs'` → definition + doc-comment references only, no call site even in tests. The drain side works and is tested (`ModIntegrationPipeline.Apply` drains the faulted set, `ModIntegrationPipeline.cs:216-228`; `ModFaultHandlerTests` via the `GetFaultHandlerForTests` seam, `:948`).

### R2.3 Sibling path — native batched dispatch (dormant)

`ManagedSystemDispatcher.OnBatch` (`src/DualFrontier.Application/Scheduler/ManagedSystemDispatcher.cs:74-91`) is the `[UnmanagedCallersOnly]` reverse-P/Invoke entry; it **does** catch-all, verbatim (`:85-90`):

```csharp
catch (Exception ex)
{
    // Exceptions cannot cross к native — absorb at boundary.
    // К10.2 wiring routes к IModFaultSink; К10.1 traces к Debug.
    System.Diagnostics.Debug.WriteLine($"[K10.1] managed batch dispatch error: {ex}");
}
```

Absorbed to `Debug.WriteLine` only; the `IModFaultSink` routing is a comment, not code. The path is dormant in production: `rg -n 'SchedulerAdapter\.(Register|ClearCallback)' src tests` → **0 production callers** (`Register` only at `tests/DualFrontier.Core.Tests/Scheduler/BatchedCallbackTests.cs:30`; `ClearCallback` only at `:35`,`:72`).

### R2.4 Deferred-handler dispatch — the CMM §7 asymmetry, verified verbatim

**Dispatch chain (single flush site):** `ExecutePhase` → `if (_services is IDeferredFlush flusher) flusher.FlushDeferred();` (`ParallelSystemScheduler.cs:166-167`) → `GameServices.FlushDeferred` fans out to the five buses (`src/DualFrontier.Core/Bus/GameServices.cs:55-62`) → `DomainEventBus.FlushDeferred` (`src/DualFrontier.Core/Bus/DomainEventBus.cs:115-135`) → `InvokeDeferred` per subscriber.

**Sync/Immediate delivery — per-subscriber catch present** (`DomainEventBus.cs:156-166`, verbatim):

```csharp
foreach (Subscription sub in snapshot)
{
    try
    {
        sub.Invoker(evt);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error publishing event {eventType.Name}: {ex.Message}");
    }
}
```

**Deferred delivery — no catch whatsoever** (`DomainEventBus.cs:169-187`, verbatim):

```csharp
private static void InvokeDeferred(Subscription sub, IEvent evt)
{
    SystemExecutionContext? ctx = sub.CapturedContext;
    bool pushed = false;
    if (ctx is not null)
    {
        SystemExecutionContext.PushContext(ctx);
        pushed = true;
    }
    try
    {
        sub.Invoker(evt);
    }
    finally
    {
        if (pushed)
            SystemExecutionContext.PopContext();
    }
}
```

The asymmetry is exactly as CMM §7 states: a faulting deferred handler unwinds `FlushDeferred` → `ExecutePhase` → `RunLoop` → process death; the same handler faulting on the sync path is swallowed-with-console-line and delivery continues. The sync-path catch logs to `Console.WriteLine`, not to `IModFaultSink` (the "even the good mode under-reports" clause) — `rg --count-matches 'Console\.WriteLine' src -g '*.cs'` → 3 sites total in `src/` (`DomainEventBus.cs` ×1, `RestrictedModApi.cs` ×2). The catch-insertion point has the captured `SystemExecutionContext` in hand (origin/modId/sink), same as R2.2.

### R2.5 Fault-taxonomy surface (where ELT classes 1–7 could attach today)

- **Exception types in `src/`** — `rg -n 'class \w+Exception' src -g '*.cs'` → **8**: `SceneLoadException`, `CapabilityViolationException`, `BootstrapFailedException` (`Bootstrap.cs:90`), `FieldOperationFailedException`, `HardwareCapabilityException`, `PngDecoderException`, `PipelineQuiescenceTimeoutException` (`SimulationStateController.cs:142`), `ModIsolationException`.
- **Degraded surface:** **none** — `rg -in 'degraded' src` → 0 hits (ELT §3.1/§4.1 confirmed: no representation of any kind).
- Per-class attach points measured: class 1 → the absent `ExecutePhase` catch (R2.2); class 2 → the shipped out-of-date swapchain retry (R5.2) — no declared retry policy object anywhere; class 3 → `ModFaultHandler`/`IModFaultSink` exist, feed dormant (R2.2); class 4 → nothing; class 5 → `df_scheduler_panic_halt`/`df_scheduler_is_panic` exports exist (R4 inventory); class 6 → `VK_ERROR_DEVICE_LOST` defined and unhandled (R5.4); class 7 → `df_scheduler_policies_quota_exceeded`/`_quota_violations` exports exist, no configured handler.
- Logging shapes available today: `ValidationWarning` lists (pipeline), `Console.WriteLine` (3 sites), `Debug.WriteLine` (OnBatch), warning strings (step-7 pump). No structured lifecycle event stream (ELT §5) exists.

### R2.6 Existing tests on these fault paths

- Bus suite = 4 files (`rg --files tests/DualFrontier.Core.Tests/Bus`): `DeferredEventDeliveryTests`, `ImmediateEventDeliveryTests`, `MethodGroupSubscriptionTests`, `ManagedBusBridgeTests`.
- **No test throws from a handler in either delivery mode.** `rg -c 'Throw' tests/DualFrontier.Core.Tests/Bus/*.cs` → `DeferredEventDeliveryTests.cs:1` (a `NotThrow` assertion about captured-context mutation rights, `:146-149`), `ManagedBusBridgeTests.cs:5` (bridge/arg checks); `ImmediateEventDeliveryTests` → 0. The sync-path swallow-and-continue behavior and the deferred-path escape are both **unasserted**.
- **No test exercises a system throwing inside `ExecutePhase`** (no catch seam exists to test — ELT §5 row "Mod fault (a) missing" confirmed).
- What DOES exist nearby (ELT §5 anchors re-verified live): `Pipeline_build_failure_leaves_old_scheduler_intact` (`tests/DualFrontier.Modding.Tests/Pipeline/ModIntegrationPipelineTests.cs:88`), `Pipeline_unload_removes_mod_systems_from_scheduler` (`:107`), quiesce-refusal guards (`M71PauseResumeTests.cs:166,:180`; throw sites live at `ModIntegrationPipeline.cs:212-214`, `:564-566`, `:782-784`), step-throw seams in `M72UnloadChainTests` (R6.3).

---

## R3 Shutdown paths (call graphs + S1-S8/6.2 verdict table + F-40 shape)

### R3.1 Entry point A — application exit, complete live call chain

1. `runtime.Window.IsOpen` false → T1 leaves the frame loop — `src/DualFrontier.Launcher/Program.cs:70`.
2. `gameContext.Loop.Stop()` — `Program.cs:95` → `GameLoop.Stop` = `_cts.Cancel(); _thread?.Join(2000);` — **`Join` result ignored** (`GameLoop.cs:73-77`); thread was created `IsBackground = true` (`:64-68`); on timeout T2 is abandoned possibly inside `ExecuteTick` (`:115`) / Background drain (`:120-128`).
3. `renderer.Shutdown()` — `Program.cs:96` → `LauncherRenderer.Shutdown` (`LauncherRenderer.cs:199-218`): `VulkanDevice.WaitIdle()` (`:205`) → per-image semaphores (`:206-212`) → frame fence (`:213`) → `_imageAvailable` (`:214`) → command buffer (`:215`).
4. `return 0` — `Program.cs:97` (exit code unconditional) → `using` unwind in reverse declaration order: `renderer` (`:61`, idempotent re-`Shutdown` via `Dispose`, `LauncherRenderer.cs:222`) → `atlasTexture` (`:52`) → `runtime` (`:43`) → `Runtime.Dispose` (`Runtime.cs:438-479`): `WaitIdle` (`:446`) → sprite stack (`:449-457`) → compute pipelines/allocator/command pools (`:460-463`) → framebuffers (`:464-468`) → render pass (`:469`) → swapchain (`:470`) → surface (`:471`) → device (`:474`) → validation layer (`:475`) → instance (`:476`) → **window last** (`:477`).
5. Whatever survives is reclaimed by process teardown; `~NativeWorld` (T5) may run `df_world_destroy` at any unordered point (R3.4).

Nothing in this chain touches the `NativeWorld`, the mods, the native scheduler graph/wake registry, the native bus, or any subscription `GCHandle`. `gameContext` is a plain local (`Program.cs:55`); `GameContext` is `internal sealed record GameContext(GameLoop Loop, ModMenuController Controller)` (`GameContext.cs:19`) — not `IDisposable`, does not carry the world. `GameLoop.Dispose()` (= `Stop()`, `GameLoop.cs:141`) has **zero callers**: `rg -n 'Loop\.Dispose\(\)|loop\.Dispose\(\)' src tests -g '*.cs'` → 0; the `_cts` (`GameLoop.cs:42`) is never disposed.

### R3.2 Entry point B — `UnloadAll` / mod unload, complete call chain

`ModIntegrationPipeline.UnloadAll` (`src/DualFrontier.Application/Modding/ModIntegrationPipeline.cs:780`; `_isRunning` guard `:782-784`) → `SnapshotActiveModIds` (`:837`, `NoInlining`) → per mod `UnloadMod` (`:561`; guard `:564-566`) → `RunUnloadSteps1Through6AndCaptureAlc` (`:612`, `NoInlining`):

- Step 1 `mod.Api?.UnsubscribeAll()` (`:630-633`); Step 2 `_contractStore.RevokeAll(modId)` (`:637-640`); Step 3 `_registry.RemoveMod(modId)` (`:646-649`);
- Step 3.5 (`:665-691`): `ModUnloadInterop.HashModId` → `ModUnloadInterop.UnloadModNativeState` (`src/DualFrontier.Core.Interop/ModUnloadInterop.cs:112-116`) → `df_scheduler_unload_mod_native_state` (`native/.../src/mod_unload.cpp:45-121`); plus `_registry.RemoveSubScheduler(modId)` (`:690`);
- Step 3.6 V-resource placeholder (`:705-717`); Steps 4+5 local graph rebuild + `_scheduler.Rebuild` (`:724-737`); Step 6 `_loader.UnloadMod(modId)` (`:744-747`);
- WR capture (`:752`) + `_activeMods.Remove(mod)` (`:757`) → step 7 GC pump `TryStep7AlcVerification` (`:913-935`; 100 × 100 ms per `:118-120`; `ModUnloadTimeout` warning `:929-934`).

Each step is wrapped best-effort by `TryUnloadStep` (`:853-859+`). Empty-active-set path still rebuilds the kernel-only graph (`:809-821`).

**Production callers of `UnloadAll`: zero.** `rg -n 'UnloadAll\(\)' -g '*.cs'` → 1 definition (`ModIntegrationPipeline.cs:780`) + 6 test sites (`M71PauseResumeTests.cs:166,:180`; `M72UnloadChainTests.cs:260,:279,:297`; `ModIntegrationPipelineTests.cs:116`); 0 sites in `src/` outside the definition.

### R3.3 S1–S8 verdict table (ROL §4.1 vs the code at `da63a93`)

| Step | Verdict | Evidence |
|---|---|---|
| S1 stop intake | **PARTIAL** | Input drained-and-discarded every frame (`Program.cs:81-84`) — vacuous, not a shutdown gate; mod ops gated only by `_isRunning` throws (`ModIntegrationPipeline.cs:212-214`, `:564-566`, `:782-784`). No shutdown-specific intake stop exists. |
| S2 finish tick, bounded join, checked | **PARTIAL** | Bounded join exists; check absent: `_cts.Cancel(); _thread?.Join(2000);` result ignored (`GameLoop.cs:73-77`); no fault on timeout; teardown proceeds under a possibly live mutator. No quiesce point Q1 is ever established. |
| S3 flush-or-drop, counted per queue | **ABSENT** | Drop primitives exist uncounted: `DomainEventBus.Clear` (`DomainEventBus.cs:141-145`, "Used by tests and scene reloads"), `GameServices.Clear` (`GameServices.cs:41-48`); never invoked at exit; `PresentationBridge` leftovers die with the process, unstated; native Background pending count never recorded. |
| S4 unload mods | **ABSENT at exit** | The full chain EXISTS and is tested (R3.2) but `UnloadAll` has zero production callers — the chain is unreachable from any production path. |
| S5 native scheduler/bus teardown + free GCHandles | **ABSENT at exit** | `SystemGraphInterop.Clear()`/`WakeRegistryInterop.Clear()` run only at next-bootstrap start (`GameBootstrap.cs:160-161`; census `rg -n 'SystemGraphInterop\.Clear\(\)|WakeRegistryInterop\.Clear\(\)' src tests` → sole `src/` site is GameBootstrap; 6 test files); `SchedulerAdapter.ClearCallback` test-only (R2.3; native `df_scheduler_clear_managed_callback` `capi.cpp:1338`, header `df_capi.h:826`, binding `NativeMethods.Scheduler.cs:229`); `df_bus_clear` test-only (R3.6); GCHandle freeing exists only in `Unsubscribe` (`ManagedBusBridge.cs:94-100`) and `ClearForTesting` (`:129-137`). |
| S6 GPU waitIdle + Vulkan dispose | **EXISTS** | `LauncherRenderer.Shutdown` (`:199-218`) + `Runtime.Dispose` reverse-order (`:438-479`) — the model citizens, unchanged. Pipeline-slot drain to `Empty`/`ReadableAsTail` before device teardown: ABSENT (quiescence is checked only by the mod-unload primitive, `mod_unload.cpp:58-72`). |
| S7 deterministic `NativeWorld.Dispose` | **ABSENT** | `rg -n 'World\.Dispose|world\.Dispose\(\)' src` → 0 hits in `src/`; tests: 10 sites in 10 files (`rg --count-matches 'world\.Dispose\(\)' tests -g '*.cs'`). Destruction happens only via the finalizer (R3.4). |
| S8 window last + pending-reclamation exit log | **PARTIAL** | Window disposal is last inside `Runtime.Dispose` (`Runtime.cs:477`); exit log of pending reclamations ABSENT; exit code is a constant `0` (`Program.cs:97`). |

**ROL §6.2 checked-destroy: ABSENT in all clauses.** `df_world_destroy` is an unconditional `delete as_world(world)` — void return, no span check, no liveness check, no catch (`native/.../src/capi.cpp:60-62`, verbatim: `DF_API void df_world_destroy(df_world_handle world) { delete as_world(world); }`). No Q1 assert; no wait-for-zero (`active_spans_` counter exists native-side: `world.h:167`, accessor `World::active_spans_count()` `world.h:76-78` — **unexported**, R4.2); release underflow silently clamps (`world.cpp:257-261`); per-field counters unwired to any teardown; the finalizer is the destroy path rather than a leak reporter.

### R3.4 `NativeWorld` disposal reality

`src/DualFrontier.Core.Interop/NativeWorld.cs:486-503`, verbatim:

```csharp
public void Dispose()
{
    if (_handle != IntPtr.Zero)
    {
        NativeMethods.df_world_destroy(_handle);
        _handle = IntPtr.Zero;
    }
    GC.SuppressFinalize(this);
}

~NativeWorld()
{
    if (_handle != IntPtr.Zero)
    {
        NativeMethods.df_world_destroy(_handle);
        _handle = IntPtr.Zero;
    }
}
```

- Idempotent handle-zeroing dispose + **a live finalizer that calls `df_world_destroy`**. Determinism: none in production (0 `src/` Dispose calls — the finalizer thread T5 is the only destroyer). Join semantics: none — nothing sequences destruction against the sim thread; after a `Join(2000)` timeout the finalizer can race an abandoned T2 inside `df_world_*` (CMM §6.1's UB scenario, structurally unchanged).
- Destruction order vs buses/leases/swapchain: **unordered** — the finalizer runs whenever GC decides after the last strong reference drops (the world is rooted via scheduler → loop → `gameContext` until `Main` returns), i.e. potentially after the Vulkan device is destroyed and while native bus/graph singletons still hold registrations.
- **Anomaly (recorded in R7):** the class doc at `NativeWorld.cs:25-26` still says "Dropping a `NativeWorld` without calling `Dispose` leaks the underlying C++ world", and ELT §2.6 asserts "there is no finalizer backstop" — both contradicted by the live finalizer at `:496-503`. ROL R1/§4.3-G1 and CMM §1-T5 carry the finalizer truth.

### R3.5 `EngineSession` — absent; who owns the would-be members today

`rg -n 'EngineSession' src tests` → **0 hits** (ROL §1.1 confirmed). Ownership of the four subtrees today:

| Would-be session member | Constructed at | Held by (today) | Reachable at shutdown? |
|---|---|---|---|
| `NativeWorld` | `Bootstrap.Run(useRegistry: true)` (`Bootstrap.cs:61-70`) at `GameBootstrap.cs:76` | local var; rooted transitively via systems/scheduler | no owner — finalizer only |
| `GameServices` (5 buses) + bootstrap bridge lambdas | `GameBootstrap.cs:79`; five `Subscribe` lambdas `:82-93` | scheduler ctor arg `:198`; subscriptions never unsubscribed | no |
| `TickScheduler` / `DependencyGraph` / `ParallelSystemScheduler` | `:80`, `:145-148`, `:192-199` | `GameLoop` ctor `:214` | via `Loop` only (no dispose path) |
| Native graph + wake registrations | loop `:160-181` (`Clear` at start `:160-161`, register `:170-176`, `SubscribeTimer` `:179`, `ComputeStaticGraph` `:181`) | process-global native singletons | no teardown at exit |
| Mod stack (`ModLoader`/`ModRegistry`/`ContractValidator`/`ModContractStore`/`ModIntegrationPipeline`/discoverer/controller) | `:183-206` | `GameContext.Controller` → pipeline | `UnloadAll` unreachable |
| `ManagedBusBridge` (+ GCHandle pool `_handles`, `ManagedBusBridge.cs:35`) | `:212` | `GameLoop` ctor `:214` (Background drain `GameLoop.cs:127`) | no (`ClearForTesting` only) |
| `Runtime` / atlas / `LauncherRenderer` / `PresentationBridge` / `SceneState` | `Program.cs:43-61` | `Program.Main` `using`s + locals | yes — the only owned subtree |
| Returned to the caller | `new GameContext(loop, controller)` (`:219`) | `Program.cs:55` plain local | Loop.Stop only |

### R3.6 `df_bus_clear` — current status

- Declared test-only at the ABI: `native/.../include/bus_native.h:155` verbatim: `DF_API void    df_bus_clear(void);  // test-only`.
- Implementation `native/.../src/bus_common.cpp:68-85`: fixed-order lock `fast → normal → background` (`:74-76`), clears subscribers + pending, resets each tier's `next_seq = 1` (`:77-84`).
- Callers (code): `rg -n 'df_bus_clear'` → managed production path **none**; sole `src/` caller `ManagedBusBridge.ClearForTesting` (`ManagedBusBridge.cs:131`; doc `:128` "Test-only: clears all native bus state. Releases GC handles too."); binding `NativeMethods.Bus.cs:60`; tests: `SchedulerStressTests.cs:78` (private direct P/Invoke declared `:87`); native selftest: **37 call sites** (`rg --count-matches 'df_bus_clear\(\);' native/DualFrontier.Core.Native/test/selftest.cpp` → 37).
- Promotion surface per ROL §7.1-3: the header comment, `ManagedBusBridge` (a production `Shutdown()` promoted from `ClearForTesting`), and the EVENT_BUS/К-L15 doc surface; open sub-decision — Fast-tier callbacks on the clearing thread (D6 in R7).

### R3.7 F-40 shape

**Ledger row, verbatim (`docs/ROADMAP.md:1022`):**

> | F-40 | Cross-suite native-state flake: `ModUnloadInteropTests.UnloadModNativeState_VacuousUnload_Succeeds` failed once in a full-sln run (C-B gate, 2026-07-17) and passed 202/202 in the isolated suite re-run; zero `src/`/`native/` delta vs the passing baseline run. Mod-unload native globals (`g_sim_paused`, quiescence preconditions) are process-wide state exposed to cross-suite ordering — the F-29/F-31 family (bus was mutex-excluded; mod_unload state was not under the F-29 SingletonGuard umbrella) | S3 | OPEN | Reproduce under controlled ordering; consider SingletonGuard/collection treatment for `mod_unload` globals per the F-29 D2/D3 pattern |

**Native globals inventory (mod_unload + what the unload primitive touches):**

| Global | Where | Mutated by |
|---|---|---|
| `std::atomic<int32_t> g_sim_paused{1}` (default **paused**) | `mod_unload.cpp:17` (anonymous namespace) | `df_scheduler_set_sim_paused` (`:36-39`) ← `ModUnloadInterop.SetSimPaused` ← `SimulationStateController.PauseAsync`/`ResumeAsync` (`SimulationStateController.cs:71-87`) and tests |
| Bus tier-state singletons (subscribers, pending, `next_seq`) | `bus_common.cpp:21-34` (function-local statics) | publish/subscribe/unsubscribe/clear from any thread; unload primitive T1–T3 (`mod_unload.cpp:83-93`) |
| Pipeline-slot state machine | `pipeline_slot.*` process-global | slot acquire/fence/reset paths; read by the unload precondition via `df_pipeline_is_quiescent` (`mod_unload.cpp:65-72`) |
| Scheduler graph / wake registry | `system_graph.cpp` / `wake_registry.cpp` | register/clear/compute (bootstrap, tests) |

**Preconditions the primitive enforces (`mod_unload.cpp:52-72`):** `g_sim_paused == 1`, else error "K-L18 quiescent state precondition violated: sim is not paused"; pipeline quiescent (all slots Empty/ReadableAsTail; depth==0 vacuously quiescent), else the second K-L18 error. T1–T3 are real per-tier operations; **T4–T7 are zeroed wire-up stubs** (`:108-117`). The header's T0 "single critical section" is no longer real: code comment `:74-79` states verbatim "The cross-tier «single critical section» concept is gone" after the 2026-05-21 bus refactor (per-tier mutexes; each unsubscribe locks only its tier).

**SingletonGuard coverage (what F-29 protects vs what it does not):** `rg --count-matches 'SingletonGuard guard\(' native/DualFrontier.Core.Native/src/system_graph.cpp native/DualFrontier.Core.Native/src/wake_registry.cpp` → **5 + 13 = 18 guarded entry points**, all in the scheduler graph and wake registry (`singleton_guard.h:31-51` — acquire-or-fail detector returning `kConcurrencyViolation = -3`, `:13`). NOT covered: `mod_unload`'s `g_sim_paused` (a bare atomic — data-race-safe but semantically shared), the bus (deliberately excluded — internally mutex-synchronized, TESTING_STRATEGY §2.8), pipeline slots, scheduling-policies and event-type registries (collection-covered only, §2.8).

**The failing test, exactly (`tests/DualFrontier.Core.Interop.Tests/ModUnloadInteropTests.cs:20-30`):** sets `SetSimPaused(true)` itself (`:22`), calls a vacuous unload for mod-hash `0xFFFFFFFF`, then asserts `Success == 1`, `Fast/Normal/BackgroundSubscriptionsCleared == 0` each, and `ErrorCount == 0`. Sibling `UnloadModNativeState_SimNotPaused_FailsWithError` (`:33-44`) flips `g_sim_paused` to 0 and restores it (`:43`) — same class, therefore serialized by xUnit.

**Why ordering matters — measured shape:** the assertions read process-global native state (pause flag, pipeline-slot quiescence, bus subscriber registries). `DualFrontier.Core.Interop.Tests` has **no serializing collection**: `rg -n '\[Collection\(' tests -g '*.cs'` → 4 attribute sites, all in `Core.Tests` (`SharedNativeSingleton` ×3) and `Modding.Tests` (`GameLoopSerial` ×1); none in `Interop.Tests`, and no `xunit.runner.json`/assembly-level parallelization override was found (`rg -n 'DisableParallelization|ParallelizeAssembly|maxParallelThreads' tests -g '*.cs' -g '*.json'` → only the two collection definitions). xUnit therefore runs `Interop.Tests` **classes** in parallel; `PipelineSlotInteropTests` (same suite) mutates the process-global pipeline slots that the vacuous-unload precondition polls, and any parallel `SetSimPaused(false)` window would fail precondition 1. One distance note for the architect: the ledger phrases the exposure as "cross-suite ordering," but `dotnet test` on the solution runs each suite in its own testhost process — native globals cannot cross processes; the measured exposure is **intra-suite cross-class parallelism** (recorded as anomaly A7, R7).

---

## R4 ABI surface (export inventory + landing zones)

### R4.1 Export inventory

Census (per header, expression verbatim): `rg --count-matches '^DF_API' <header>` →

| Header | Exports |
|---|---|
| `include/df_capi.h` | **151** |
| `include/bus_native.h` | **15** |
| `include/event_type_registry.h` | **5** |
| `include/mod_unload.h` | **3** |
| **Total** | **174** |

Name extraction (verbatim, count-verified 151/151 for df_capi.h): `rg -o '^DF_API\s+[^(]*?(df_\w+)\s*\(' -r '$1' native/DualFrontier.Core.Native/include/df_capi.h`.

**df_capi.h (151):** df_world_create, df_world_destroy, df_world_create_entity, df_world_destroy_entity, df_world_is_alive, df_world_entity_count, df_world_flush_destroyed, df_world_add_component, df_world_has_component, df_world_get_component, df_world_remove_component, df_world_component_count, df_world_add_components_bulk, df_world_get_components_bulk, df_world_acquire_span, df_world_release_span, df_world_register_component_type, df_engine_bootstrap, df_world_begin_batch, df_batch_record_update, df_batch_record_add, df_batch_record_remove, df_batch_flush, df_batch_cancel, df_batch_destroy, df_world_intern_string, df_world_resolve_string, df_world_string_generation, df_world_begin_mod_scope, df_world_end_mod_scope, df_world_clear_mod_scope, df_world_string_pool_count, df_world_string_pool_current_generation, df_world_get_keyed_map, df_keyed_map_set, df_keyed_map_get, df_keyed_map_remove, df_keyed_map_count, df_keyed_map_iterate, df_keyed_map_clear, df_world_get_composite, df_composite_add, df_composite_get_count, df_composite_get_at, df_composite_remove_at, df_composite_clear_for, df_composite_iterate, df_world_get_set, df_set_add, df_set_contains, df_set_remove, df_set_count, df_set_iterate, df_world_register_field, df_world_field_unregister, df_world_field_read_cell, df_world_field_write_cell, df_world_field_acquire_span, df_world_field_release_span, df_world_field_set_conductivity, df_world_field_get_conductivity, df_world_field_set_storage_flag, df_world_field_get_storage_flag, df_world_field_swap_buffers, df_world_field_count, df_world_attach_vulkan, df_world_register_compute_pipeline, df_world_field_dispatch_compute, df_world_compute_pipeline_count, df_scheduler_register_system, df_scheduler_unregister_system, df_scheduler_system_count, df_scheduler_clear, df_scheduler_compute_static_graph, df_scheduler_static_phase_count, df_scheduler_static_phase_size, df_scheduler_static_phase_systems, df_scheduler_compute_per_tick_graph, df_scheduler_per_tick_phase_count, df_scheduler_per_tick_phase_size, df_scheduler_per_tick_phase_systems, df_wake_registry_subscribe_timer, df_wake_registry_subscribe_event, df_wake_registry_subscribe_state, df_wake_registry_subscribe_init, df_wake_registry_subscribe_explicit, df_wake_registry_unsubscribe, df_wake_registry_fire_timer, df_wake_registry_fire_event, df_wake_registry_fire_state_change, df_wake_registry_fire_init, df_wake_registry_fire_explicit, df_wake_registry_runqueue_size, df_wake_registry_drain_runqueue, df_wake_registry_subscription_count, df_wake_registry_clear, df_scheduler_query_runnable, df_scheduler_query_wake_subscriptions, df_scheduler_policies_set, df_scheduler_policies_get_class, df_scheduler_policies_get_quota, df_scheduler_policies_record_execution, df_scheduler_policies_quota_exceeded, df_scheduler_policies_total_micros, df_scheduler_policies_quota_violations, df_scheduler_policies_reset_tick_stats, df_scheduler_policies_order_by_priority, df_scheduler_policies_clear, df_scheduler_policies_set_affinity, df_scheduler_policies_get_affinity, df_scheduler_work_stealing_enabled, df_scheduler_set_work_stealing_enabled, df_scheduler_set_phase_barrier, df_scheduler_get_phase_barrier, df_scheduler_trace_set_enabled, df_scheduler_trace_enabled, df_scheduler_trace_push, df_scheduler_trace_dump, df_scheduler_trace_event_count, df_scheduler_trace_clear, df_scheduler_suspend, df_scheduler_resume, df_scheduler_is_suspended, df_scheduler_panic_halt, df_scheduler_is_panic, df_scheduler_snapshot, df_scheduler_intrinsics_reset, df_state_filter_may_have_subscribers, df_state_filter_has_entity_specific_subscriber, df_state_filter_subscribe_type, df_state_filter_subscribe_entity, df_state_filter_unsubscribe_type, df_state_filter_unsubscribe_entity, df_state_filter_type_wide_subscriber_count, df_state_filter_entity_subscriber_count, df_state_filter_clear, df_native_world_commit_hook, df_scheduler_register_managed_callback, df_scheduler_dispatch_managed_batch, df_scheduler_managed_callback_registered, df_scheduler_clear_managed_callback, df_shm_create, df_shm_map, df_shm_size, df_shm_unmap, df_shm_destroy, df_shm_register_writer, df_shm_writer, df_shm_region_count, df_shm_clear, df_scheduler_tick_begin.

**bus_native.h (15):** df_bus_publish_fast, df_bus_publish_normal, df_bus_publish_background, df_bus_subscribe_fast, df_bus_subscribe_normal, df_bus_subscribe_background, df_bus_unsubscribe, df_bus_unsubscribe_fast_by_mod, df_bus_unsubscribe_normal_by_mod, df_bus_unsubscribe_background_by_mod, df_bus_drain_normal_batch, df_bus_subscriber_count_fast, df_bus_subscriber_count_normal, df_bus_subscriber_count_background, df_bus_clear.

**event_type_registry.h (5):** df_event_type_registry_register, df_event_type_registry_lookup, df_event_type_registry_get_tier, df_event_type_registry_count, df_event_type_registry_clear.

**mod_unload.h (3):** df_scheduler_set_sim_paused, df_scheduler_is_sim_paused, df_scheduler_unload_mod_native_state.

### R4.2 Landing zones for the two EQ-a exports

- **`df_world_active_span_count`** — census `rg -n 'active_span_count' native/DualFrontier.Core.Native/include src` → **0 hits** (unexported today). Native backing already exists: `World::active_spans_count()` (`world.h:76-78`) over `std::atomic<int32_t> active_spans_{0}` (`world.h:167`). Adjacent block to land in: the K1 span-protocol family, `df_capi.h:82-127` — directly after `df_world_release_span` (`:125-127`), whose contract comment block (`:88-99`) is the place the counter's read-only semantics attach. Release-side context the export makes observable: acquire increments even for empty spans (`world.cpp:241-243`), the counter is global not per-type (`world.cpp:254-256` comment), and release-underflow silently clamps to 0 (`world.cpp:257-261`) — ROL §3.4 item 4's DEBUG-assert candidate.
- **`df_world_destroy_checked`** — lands adjacent to `df_world_destroy` (`df_capi.h:45-46`); current destroy is the unconditional void `delete` (`capi.cpp:60-62`). Status-code convention to match: the shipped block comment `df_capi.h:37-41` ("0 — failure / not found; 1 — success / present. Out-of-range inputs return 0 rather than crashing") is grandfathered-immutable for shipped entries; **new** entry points take the IAC §4 `df_status` space (0 = DF_OK; negative = contract violation incl. `DF_ERR_MUTATION_LOCKED`/`DF_ERR_NOT_QUIESCENT`; positive 1–99 retryable incl. `DF_COND_PENDING`) — exact values assigned at the amendment milestone per IAC. `catch (...)` boundary discipline: 72 sites in `capi.cpp` (`rg --count-matches 'catch \(\.\.\.\)' src/capi.cpp` → 72, matching IAC §3.4's recount; also world.cpp 1, thread_pool.cpp 2, bootstrap_graph.cpp 2) — a new entry follows the same swallow-to-status form.

### R4.3 `NativeMethods` marshaling conventions (managed mirror)

`internal static partial class NativeMethods` across **6 partial files** (`rg --files src/DualFrontier.Core.Interop | grep NativeMethods`): `NativeMethods.cs` (world/batch/strings/primitives), `.Bus.cs`, `.Scheduler.cs`, `.Fields.cs`, `.Compute.cs`, `.PipelineSlot.cs`. Convention per member (`NativeMethods.cs:25-29` pattern): `[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]`, `DllName = "DualFrontier.Core.Native"` (`:23`), `IntPtr` for handles, `ulong` for packed entities, `int` for 1/0 booleans/status, `unsafe` pointers for buffers; no `SuppressGCTransition`, no custom marshalers (the `ModUnloadResult` mirror uses `LayoutKind.Sequential` + `fixed byte` for the 8×256 error block, `ModUnloadInterop.cs:15-39`, with a marshaling-shape regression test `ModUnloadInteropTests.cs:47-54` asserting size 2096). The two new exports mirror into `NativeMethods.cs` (world family) with these exact forms; the wrapper surface is `NativeWorld` (`Dispose` at `:486-494` gains the checked path; span count surfaces near `AcquireSpan`).

Not surveyed (out of scope per kickoff): the F-43 protocol surface (`df_abi_version`, affinity tags, DFK-abi rules) beyond noting IAC §3 is its ratified work order.

---

## R5 Render/device tail

### R5.1 Swapchain/device teardown order (live chain)

`LauncherRenderer.Shutdown` (`LauncherRenderer.cs:199-218`): `WaitIdle` → per-image semaphores → frame fence → acquire semaphore → command buffer; then `Runtime.Dispose` (`Runtime.cs:438-479`): `WaitIdle` (`:446`) → sprite stack → compute/allocator/pools → framebuffers (`:464-468`) → render pass (`:469`) → swapchain (`:470`) → surface (`:471`) → device (`:474`) → validation (`:475`) → instance (`:476`) → window (`:477`). Construction-failure precedent: `Runtime.Create` disposes the partial runtime on any failure (`Runtime.cs:181-185`, `runtime.Dispose(); throw;`).

### R5.2 Swapchain recreation — where prepare-before-reclaim would insert

Two trigger sites in `LauncherRenderer.RenderFrame`: acquire-out-of-date (`:125-139`) and present-out-of-date (`:181-196`), both `WaitIdle` → `Swapchain.Recreate(w,h)` → `Runtime.RecreateFramebuffersForSwapchain()` (+ shutdown-race guard `if (!_runtime.Window.IsOpen) return;` at `:128`/`:186`).

Measured stage order today, by object:

- **Swapchain handle — prepare-before-reclaim ALREADY IN PLACE:** `VulkanSwapchain.Recreate` (`src/DualFrontier.Runtime/Graphics/VulkanSwapchain.cs:43-53`) saves `oldSwapchain = _swapchain` (`:46`), passes it into `vkCreateSwapchainKHR` via `createInfo.oldSwapchain` (`:184`), and destroys the old handle **after** the new one exists (`:49-52`). (Refines ELT §2.5, which proposes the `oldSwapchain` mechanism as future work — recorded as anomaly A5.)
- **Image views — reclaim-before-prepare:** `DestroyImageViews()` runs before `CreateSwapchain` (`:47`); a `vkCreateImageView` failure mid-loop throws (`:230-234`) with old views already gone.
- **Framebuffers — reclaim-before-prepare:** `Runtime.RecreateFramebuffersForSwapchain` (`Runtime.cs:193-206`) disposes all old framebuffers and clears the list (`:195-199`) **then** constructs new ones (`:200-205`); a constructor throw leaves zero framebuffers — the ELT §2.5 counter-example, confirmed live. The doc-comment protocol statement is `Runtime.cs:188-192` ("Caller must invoke after `Swapchain.Recreate` + `VulkanDevice.WaitIdle`…"). Insertion point for the §2.5 protocol: candidate-list construction inside `:193-206` (framebuffers) and view-creation-before-view-destroy inside `VulkanSwapchain.CreateSwapchain`/`Recreate`.

### R5.3 Failure-injection seams in the render path

**None.** `rg -in 'inject|ForTest|Seam' src/DualFrontier.Runtime src/DualFrontier.Launcher -g '*.cs'` → 4 hits, all constructor-injection doc comments (`SceneState.cs:21`, `RenderCommandDispatcher.cs:25`, `Program.cs:23,:58`); no throw-on-demand or fault-injection surface exists anywhere in Runtime/Launcher. (Contrast: the modding layer has the decorator-seam precedent, R6.3.)

### R5.4 Device-lost — current handling, verified

- Definition: `VK_ERROR_DEVICE_LOST = -4` at `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs:14` — the **only** hit repo-wide: `rg -n -i "DEVICE_LOST|DeviceLost" src native tests` → 1 line. Zero handlers, zero mappings (ELT §4 class 6 / IAC §7 item 7 confirmed).
- What actually happens today on `VK_ERROR_DEVICE_LOST`: `VulkanSwapchain.AcquireNextImage` (`VulkanSwapchain.cs:59-69`) maps only `VK_ERROR_OUT_OF_DATE_KHR` to `outOfDate` and tolerates `VK_SUBOPTIMAL_KHR`; **every other result throws** `new InvalidOperationException($"vkAcquireNextImageKHR failed: {result}")` (`:64-67`). `Present` likewise (`:92-101`). So a lost device today = a generic `InvalidOperationException` from T1's `RenderFrame` → unwinds `Program.Main` (no catch) → process crash with a generic message — not silent, but unclassified (no ELT class-6 taxonomy, no diagnostic), and it bypasses even today's three-step shutdown (`Loop.Stop`/`renderer.Shutdown` never run; the `using` unwind still executes during exception propagation).
- OQ-3 (fail-fast v1 vs device re-creation) remains the architect's decision; if fail-fast is ratified, the classification point is exactly these two result checks.

---

## R6 Test surface + seams

### R6.1 Suite layout truth

Live non-fixture projects under `tests/` (`rg --files tests -g '*.csproj' | grep -v Fixture`) → **12**: ten dotnet-test-invocable suites (Analyzers, Application, Core.Interop, Core, Governance, Mod.ManifestRewriter, Modding, Persistence, Runtime, Systems) + two non-test executables (Core.Benchmarks, Runtime.SmokeTest — TESTING_STRATEGY §2.2). TESTING_STRATEGY §2.1 pins **nine** invocable projects as the dated 2026-06-11 survey; `DualFrontier.Governance.Tests` post-dates it (REGISTER_INVERSION) — live invocable count is **10** (anomaly A2; the closure-gate wording in the brief should count 10).

### R6.2 Suites/fixtures relevant to shutdown + fault injection

| Surface | Suite | Anchors |
|---|---|---|
| Quiesce refusal (pause guards) | Modding.Tests | `M71PauseResumeTests.cs:166,:180` (UnloadAll-while-running throws) |
| Unload chain, step-throw isolation | Modding.Tests | `M72UnloadChainTests`: `UnloadMod_WhenRunning_Throws…` `:49`; `UnloadMod_Step2Throws_StepsContinueAndWarningCollected` `:180`; `UnloadMod_StepThrows_ModRemovedFromActiveSet` `:209`; `UnloadAll_PreservesM71Guard_ThrowsWhenRunning` `:288` |
| Commit atomicity | Modding.Tests | `ModIntegrationPipelineTests.cs:88` (build-failure leaves old scheduler), `:107` (unload removes systems), `:116` (`UnloadAll`) |
| Loop/bootstrap integration | Modding.Tests | `GameBootstrapIntegrationTests` under `[Collection("GameLoopSerial")]` (`:53`; definition `:27`, `DisableParallelization = true`) |
| Batched-callback registration/teardown | Core.Tests | `BatchedCallbackTests.cs:30` (`Register`), `:35`/`:72` (`ClearCallback`) |
| Native unload primitive | Core.Interop.Tests | `ModUnloadInteropTests` (R3.7) + `PipelineSlotInteropTests`, `SpanLeaseTests`, `NativeWorldTests` |
| К-L18 helpers | Application.Tests | `SimulationStateControllerTests.cs` |
| Bus delivery semantics | Core.Tests/Bus | 4 files (R2.6) — no fault-path assertions |
| Native kernel scenarios | selftest | `test/selftest.cpp` (`DF_CHECK` harness; 37 `df_bus_clear` resets) |

### R6.3 Existing injection seams (the M7.2 precedent)

The repo's canonical failure-injection pattern is the **collaborator decorator**, not a hook: `M72UnloadChainTests` builds the pipeline with `new ThrowingRevokeAllContractStore(new ModContractStore())` (`M72UnloadChainTests.cs:337-341`) so step 2 throws inside the real chain, asserting best-effort continuation + warning collection (the "M7.2 step-2 throwing seam" the pipeline's own comments cite, `ModIntegrationPipeline.cs:578-580`). The scheduler has **no equivalent seam** (no catch to observe — ELT §5 "no catch seam exists to test", confirmed), and the render path has none (R5.3).

### R6.4 Where the new regression tests land (per TESTING_STRATEGY layer taxonomy)

- Deferred-catch symmetry + sync-path sink routing → `tests/DualFrontier.Core.Tests/Bus/` (managed unit, §3.1 — the existing 4-file family).
- `ExecutePhase` per-system catch + origin dispatch → `tests/DualFrontier.Core.Tests/Scheduling/` (§3.1/§3.7; a fault-injection system fixture is in-assembly, no ALC needed).
- The two new ABI exports → `tests/DualFrontier.Core.Interop.Tests/` — §3.2 law verbatim: "A new interop entry point without a boundary test in this suite is a review defect"; plus selftest scenarios (§3.6: "A native-touching cascade adds its scenarios here and runs the exe in both configurations at closure").
- Shutdown transaction / `EngineSession` → integration tests beside `GameBootstrapIntegrationTests` (Modding.Tests, `GameLoopSerial` collection) and/or Application.Tests; ROL §4.4's verification hooks (world handle zeroed, `ManagedBusBridge.SubscriberCount` zero, span count zero) name the assertions.
- Swapchain failure injection → `tests/DualFrontier.Runtime.Tests/` (a seam must be introduced first, R5.3).

### R6.5 Stress/collection interactions the new tests must respect

- §2.8 law: any test class that **mutates a process-global native singleton** (scheduler graph, wake registry, policies registry, event-type registry, native bus) MUST join `[Collection("SharedNativeSingleton")]` (`SharedNativeSingletonCollection.cs:41`, `DisableParallelization = true`; members today: `SchedulerStressTests`, `SchedulerExtremeTests`, `ManagedBusBridgeTests`). New shutdown tests that call `df_bus_clear`, graph/wake `Clear`, or `ClearCallback` from Core.Tests fall under this law.
- The §2.8 enumerated singleton list does **not** include `mod_unload`'s `g_sim_paused` or the pipeline slots, and `Core.Interop.Tests` has no serializing collection at all (R3.7) — extending either the list, the collection membership, or the SingletonGuard per the F-29 D2/D3 pattern is exactly F-40's owner line.
- Trait law §3.7: `Stress`/`Extreme`/`Integration` are the only `[Trait("Category", …)]` values (census 2026-06-11: 4/1/6 sites); new heavy tests MUST carry `Stress` or `Extreme`; new trait values only by amending the §3.7 table. Live spot-check: `SchedulerStressTests.cs:43`, `ModDependencyGraphStressTests.cs:26`.
- Invocation: per TESTING_STRATEGY §8 (invocation-safety, no-pipe law) — closure gates run suites per-project, stress chunk last.
- Bi-script К-L census for the quiescence family: `rg '[КK]-L18' src native -g '!*.md'` → **39** code-side citation lines (top carriers: `selftest.cpp` 8, `SimulationStateController.cs` 7, `mod_unload.cpp` 6, `pipeline_slot.h` 4) — the surface a К-L18-generalization amendment (D5) will touch.

---

## R7 Anomalies + scale estimate (split proposal + lead edge + decision list)

### R7.1 Anomalies — divergences from the work order's assumptions

- **A1 — finalizer truth split inside the law corpus.** ELT §2.6 states "there is no finalizer backstop", and the live class doc `NativeWorld.cs:25-26` still says dropping without `Dispose` "leaks" — but `~NativeWorld()` exists and calls `df_world_destroy` (`NativeWorld.cs:496-503`). ROL R1/G1 and CMM §1-T5 carry the correct finalizer-destroys truth. Code is truth; the cascade should fix the stale in-code doc comment, and the ELT slip belongs in the closure record (ELT is LOCKED — record, not improvise).
- **A2 — suite-count pin drift.** TESTING_STRATEGY §2.1 "(9)" is a dated survey; live invocable suites = 10 (`Governance.Tests` added post-survey). Baseline-gate wording in the execution brief must count 10.
- **A3 — T0 critical section no longer exists.** `mod_unload.cpp:74-79` verbatim: "The cross-tier «single critical section» concept is gone" (post-2026-05-21 per-tier-mutex refactor). KFNS Item 32's and CMM §2.1's "one critical section T0-T7" wording describes a shape the code left behind; the unload primitive is now a sequence of per-tier locked operations behind one entry point.
- **A4 — T4–T7 are zeroed stubs** (`mod_unload.cpp:108-117`): capabilities/shm/wake/access-declaration teardown all report 0 pending К10.3 wire-up. ROL §5's "same call — synchronous" row reclaims less than the T0-T7 label suggests.
- **A5 — swapchain `oldSwapchain` already wired.** `VulkanSwapchain.Recreate` already does prepare-before-reclaim for the swapchain handle itself (`:46-52`, `createInfo.oldSwapchain` `:184`); the ELT §2.5 gap is real only for image views and framebuffers. The §2.5 member shrinks accordingly.
- **A6 — device-lost is a generic crash, not silence.** `VK_ERROR_DEVICE_LOST` today produces `InvalidOperationException("vkAcquireNextImageKHR failed: …")` from the render loop (R5.4) — unclassified fail-crash, distinct from the "nothing checks it" reading; what is missing is classification + diagnostic, not a check per se.
- **A7 — F-40 "cross-suite" wording vs process isolation.** Suites run in separate testhost processes; native globals cannot leak between them. The measured exposure is intra-suite cross-CLASS parallelism in `Core.Interop.Tests` (no serializing collection; `PipelineSlotInteropTests` mutates the polled slot state). Repro-under-controlled-ordering should target class parallelism inside that suite.
- **A8 — `ModLoader.HandleModFault` has zero invocation sites repo-wide** (not merely "no runtime path" — no test calls it either). The documented class-3 fault route is fully dormant end-to-end; the `ExecutePhase` catch will be its first real feeder.
- **A9 — `df_bus_clear` resets `next_seq` to 1** (`bus_common.cpp:78-84`) — the clear path already conforms to IAC §1 note 5's sequences-start-at-1 proposal (initial-state value not verified here; out of scope).
- **WIP check:** none — porcelain clean (R1).

### R7.2 Per-member work-class estimate

| # | EQ-a member | Files touched (src / native) | New files | New tests (est.) | Class |
|---|---|---|---|---|---|
| M1 | `ExecutePhase` per-system catch + origin dispatch (+ ELT §2.3 quarantine commit) | 1–2 src (`ParallelSystemScheduler.cs`; possibly `SystemExecutionContext` accessor) / 0 native | 0–1 (skip-set if quarantine lands here) | 4–6 (Core.Tests/Scheduling) | **M** |
| M2 | `UnloadAll` first production caller | rides M3 (one call inside the session dispose) | 0 | covered by M3 | **S** (within M3) |
| M3 | World-shutdown transaction + `EngineSession : IDisposable` (ROL §4.4 steps 1–3; S2 join-checked, S3 counted drops, S5 teardown promotion) | 6–9 src (`GameBootstrap`, `GameContext` (replaced/absorbed), `Program`, `GameLoop`, `ManagedBusBridge` (+`Shutdown`), `DomainEventBus`/`GameServices` counted clear) / 0 native | 1–2 (`EngineSession.cs`, health type) | 5–8 (GameLoopSerial integration + ROL §4.4 leak test) | **L** |
| M4 | Deferred-handler catch symmetry + sync-path sink routing + regression tests | 1 src (`DomainEventBus.cs`) / 0 native | 0 | 3–5 (Core.Tests/Bus) | **S** |
| M5 | `df_world_active_span_count` + `df_world_destroy_checked` (+ §6.2 wait-for-zero in `NativeWorld.Dispose`) | 2–3 src (`NativeMethods.cs`, `NativeWorld.cs`) / 4 native (`df_capi.h`, `capi.cpp`, `world.h`/`world.cpp`, `selftest.cpp`) | 0 | 5–7 (Interop boundary + selftest scenarios) | **M** |
| M6 | Swapchain prepare-before-reclaim (views + framebuffers) + failure-injection seam + test | 2–3 src (`Runtime.cs`, `VulkanSwapchain.cs`) / 0 native | 0–1 (seam type) | 2–4 (Runtime.Tests) | **M** (shrunk by A5) |
| M7 | Degraded surface (ELT §4.1 `EngineHealth`) | 1–2 src, hangs off M3's session | 1 | 2–3 | **S–M** (sequenced after M3) |
| M8 | `df_bus_clear` promotion (production teardown surface) | 1 src (`ManagedBusBridge` production `Shutdown`) / 1 native comment (`bus_native.h:155`) + EVENT_BUS/К-L15 doc amendment | 0 | 1–2 | **S** + governance |
| M9 | Device-lost v1 | DECISION first (OQ-3); if fail-fast ratified: 1–2 src (`VulkanSwapchain` result classification + diagnostic) | 0 | 1–2 | **S** after D1 |

### R7.3 Cascade split proposal + lead edge

Proposed split (each independently landable, gates green between):

1. **Cascade A — fault-crossing symmetry (lead).** M4, then M1. Managed-only, zero ABI, zero ordering dependence on the session work; unit-layer testable. Decisions consumed: D2 (core-origin policy).
2. **Cascade B — shutdown transaction.** M3 + M2 + M7 (+ M8 riding the S5 commit). Decisions consumed: D3, D4, D6, D8; governance rider: D5 (К-L18 generalization), ELT/ROL forward-fold records.
3. **Cascade C — checked-destroy ABI.** M5. Per ROL §4.4 step 4 the native surface lands **last** — C can be B's tail or follow it; C is also independently landable before B's S7 commit if the architect prefers ABI-first. Decision consumed: D7 (mostly settled by IAC §4).
4. **Cascade D — render tail.** M6 + M9 after D1 (OQ-3).

**Lead-edge validation (the work order names deferred-catch symmetry as CMM's stated top priority, min diff / max fault-radius):** **VALIDATED by measurement.** The asymmetric site is one 19-line static method in one file (`DomainEventBus.cs:169-187`); the catch template sits 13 lines above it in the same file (`:156-166`); the captured `SystemExecutionContext` (origin/modId/sink) is already in hand at the catch point; the blast radius removed is "any mod's `[Deferred]` handler fault = process death". One refinement from measurement: CMM §7's full law also indicts the sync path's `Console.WriteLine`-only catch ("even the good mode under-reports"), so the minimal honest cascade is InvokeDeferred parity **plus** sink routing in both modes — still a single-file src diff. The only entangled decision is D2; everything else in M4 is mechanical.

### R7.4 DECISIONS (not code) — the architect's HALT set

- **D1 (known)** — ELT OQ-3 / IAC §7 item 7: device-lost v1 = fail-fast with diagnostic, or defer entirely. Blocks M9 only.
- **D2** — CMM §7: Core-origin deferred-handler fault policy (propagate vs log-and-continue) — "one explicit recorded decision, not an accident of the `[Deferred]` attribute". Blocks M4's final shape.
- **D3** — ROL §7.2: who owns `EngineSession` (Launcher composes vs Application owns; `InternalsVisibleTo` runs Application → Launcher); `GameContext` grows vs dies; `ManagedBusBridge` ownership (session-owned, loop-borrowed). Blocks M3.
- **D4** — ROL §7.2 + ELT OQ-5: S2 join deadline value (2 s magic vs tick-budget multiple) and timeout posture (fail-fast both configurations vs DEBUG/RELEASE split — must be justified under the ELT §4 law either way). Blocks M3's S2 commit.
- **D5** — ROL §7.1-2: К-L18 quiescence is mod-lifecycle-scoped; generalizing quiesce points to process shutdown needs a К-L18 amendment or a new К-L invariant (KERNEL Part 0 is immutable core → successor/amendment path, an operator ratification act). Governance gate for B's claim to be law-conformant rather than proposal-implementing.
- **D6** — ROL §7.1-3: promoting `df_bus_clear` out of test-only amends the EVENT_BUS/К-L15 surface, including whether teardown-time Fast-tier callbacks may still fire on the clearing thread.
- **D7** — ROL §7.1-4 / IAC §7 item 6: `df_world_destroy_checked` status form — IAC §4 already supplies the `df_status` space for new entries; confirm the exact constants at the amendment milestone (low-risk).
- **D8** — ROL §7.2 (last bullet): the five bootstrap bridge lambdas (`GameBootstrap.cs:82-93`) — unsubscribe at S5 vs explicit session-lifetime exemption. "Silence is the only wrong option."
- **D9** — F-40 treatment: extend TESTING_STRATEGY §2.8's singleton list + collection membership to `mod_unload`/pipeline-slot globals (incl. an Interop.Tests serialization decision), vs extend the native SingletonGuard, vs both (the ledger's owner line names the D2/D3 pattern). Also fixes A7's wording.
- **D10** — ROL §7.2: the abnormal-exit paragraph (crash-path policy) — one authoritative paragraph is owed somewhere; owner unassigned.

---

## R8 Self-attestation

- **Zero writes except the report file (sync/validate NOT run):** CONFIRMED — the only file created in the repository is `docs/reports/EQ_A_SHUTDOWN_RECON_REPORT.md`; no existing file edited or deleted; the governance tool was not invoked in any mode; derived registers and frontmatter were read directly. (Full disclosure: two throwaway scratch files were briefly created in the OS temp directory — outside the repository — during the export-name extraction cross-check and deleted immediately after.)
- **Report written to docs/reports/ AND presented in chat (uncommitted):** CONFIRMED — the file is new and untracked; no `git add`/`commit` performed; full text presented in the session chat.
- **Zero git mutations; zero builds; zero test runs:** CONFIRMED — git used read-only (`rev-parse`, `status`, `log`); no build, no `dotnet test`, no selftest execution; all test-surface facts are from reading sources and standing documents.
- **Every census expression recorded verbatim (bi-script К-L):** CONFIRMED — each count in R1–R7 carries its exact `rg` expression beside it; К-L citation censuses use the bi-script class `[КK]` (R6.5).
