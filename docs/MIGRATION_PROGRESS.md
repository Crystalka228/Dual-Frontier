# Native Migration — Progress Tracker

**Status**: LIVE document (не LOCKED) — обновляется при каждом milestone closure
**Created**: 2026-05-07
**Last updated**: 2026-05-09 (K6.1 closure)
**Scope**: Tracks combined K-series (kernel) + M9-series (runtime) migration progression
**Companion documents**: `KERNEL_ARCHITECTURE.md` (LOCKED v1.0), `RUNTIME_ARCHITECTURE.md` (LOCKED v1.0), `CPP_KERNEL_BRANCH_REPORT.md` (Discovery, reference), `GPU_COMPUTE.md` (Phase 5 research, Lvl 1 pattern applies — см. D3)

---

## Purpose

Этот документ — единая точка наблюдения за продвижением миграции на native foundation (C++ ECS kernel + Vulkan runtime). НЕ является архитектурным документом — архитектура зафиксирована в `KERNEL_ARCHITECTURE.md` и `RUNTIME_ARCHITECTURE.md` как LOCKED v1.0.

**Что фиксируется здесь**:
- Status каждого milestone (not started / in progress / done / decision-pending / blocked)
- Commit hash + дата закрытия
- Текущие блокеры (если есть)
- Решения, принятые в процессе исполнения (вне рамок LOCKED архитектуры)
- Open questions, которые проявились в ходе работы

**Что НЕ фиксируется здесь** (см. соответствующие документы):
- Архитектурные решения K-L1..K-L10, L1..L10 — `KERNEL_ARCHITECTURE.md` Part 0, `RUNTIME_ARCHITECTURE.md` Part 0
- Детальные milestone briefs — `tools/briefs/`
- Discovery findings от experimental branch — `CPP_KERNEL_BRANCH_REPORT.md`
- Methodology adjustments — `METHODOLOGY.md`

---

## Current state snapshot

| | Value |
|---|---|
| **Active phase** | K7 (planned) — performance measurement (tick-loop) |
| **Last completed milestone** | K6.1 (mod fault wiring end-to-end — closes K6 deferred scope) — 2026-05-09 |
| **Next milestone (recommended)** | K7 (TickLoopBenchmark + §8 metrics) |
| **Sequencing strategy** | β6 — kernel-first sequential (decided 2026-05-07 per K2 closure) |
| **Combined estimate** | 9-15 weeks (5-8 kernel + 4-7 runtime) |
| **Tests passing** | 553 (76 Core + 4 Persistence + 66 Interop + 38 Systems + 362 Modding + 7 Mod.ManifestRewriter) |

---

## Sequencing decision

**Status**: RESOLVED 2026-05-07 per K2 closure
**Decision**: β6 — kernel-first sequential (K0–K8 → M9.0–M9.8)

**Rationale**:
- K2 closure provides bridge-maturity evidence — `ComponentTypeRegistry` lands K-L4 verbatim, 39 Interop tests cover entity packing, registry semantics, NativeWorld CRUD, span lifetime, and bulk operations, native selftest extended to 7 scenarios. P/Invoke patterns validated end-to-end.
- K3–K8 kernel work has no dependencies on M9.x runtime (см. cross-series coupling table). Deferring runtime does not block any kernel milestone.
- Single architectural focus per period preserves cleanness — Crystalka philosophy «cleanness > expediency». Switching to runtime mid-kernel would split mental context between two native stacks.
- M9.0–M9.8 runtime sprint deferred к after K8 cutover decision; if K8 Outcome 1 (native + batching wins decisively), runtime starts directly against `NativeWorld`.

**Alternatives rejected**:
- **β5** (kernel fast-track) — would require context-switching mid-kernel for 4–7 weeks of runtime work before returning to K3–K8. Bridge maturity is already sufficient; the «validate bridge early» motivation is satisfied by K2 itself.
- **β3** (interleaved) — context-switching cost across two native stacks with no compensating speedup. Rejected per Crystalka philosophy.

**Original three-option matrix retained for historical reference**:

| Option | Sequence | Total time | Trade-off |
|---|---|---|---|
| **β5 — kernel fast-track** | K0–K2 → M9.0–M9.8 → K3–K8 | 10–16w | Validates bridge early, then visible runtime progress, kernel completion last |
| **β6 — kernel-first sequential** | K0–K8 → M9.0–M9.8 | 10–15w | Single architectural focus per period, max cleanness, no visible game progress for 5–8w |
| **β3 — interleaved** | K0 → M9.0–M9.5 → K1–K2 → M9.6–M9.8 → K3–K8 | 11–15w | Earlier visible progress, но context-switching между native стеками |

---

## K-series progress (kernel)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| K0 | Cherry-pick + cleanup от experimental branch | DONE | 1–2 days | `89a4b24` | 2026-05-07 |
| K1 | Batching primitive (bulk Add/Get + Span<T>) | DONE | 3–5 days | `e2c50b8` | 2026-05-07 |
| K2 | Type-id registry + bridge tests | DONE | 2–3 days | `129a0a0` | 2026-05-07 |
| K3 | Native bootstrap graph + thread pool | DONE | 5–7 days | `7629f57` | 2026-05-07 |
| K4 | Component struct refactor (Hybrid Path) | DONE | 3-5 hours auto-mode (3-4 days hobby pace) | `2fc59d1` | 2026-05-08 |
| K5 | Span<T> protocol + Command Buffer write batching | DONE | 6-8 hours auto-mode (2-3 weeks hobby pace) | `547c919` | 2026-05-08 |
| K6 | Second-graph rebuild on mod change | DONE | 1-2 days hobby pace (~3-5h auto-mode for the as-found closure-shaped path) | `cb3d6cf`..`af2b572` | 2026-05-09 |
| K6.1 | Mod fault wiring end-to-end | DONE | 3-5 days hobby pace (~3-5h auto-mode) | `fe03ed3`..`a642d65` | 2026-05-09 |
| K7 | Performance measurement (tick-loop) | NOT STARTED | 3–5 days | — | — |
| K8 | Decision step + production cutover | NOT STARTED | 1 week | — | — |

**Cumulative estimate**: 5–8 weeks at hobby pace (~1h/day).

### K0 — Cherry-pick + cleanup от experimental branch

- **Status**: DONE (`89a4b24`, 2026-05-07)
- **Brief**: `tools/briefs/K0_CHERRY_PICK_BRIEF.md` (FULL EXECUTED)
- **Source branch**: `claude/cpp-core-experiment-cEsyH` (HEAD `e2bc2d9`)
- **Cherry-picks completed**: 7 commits — `7b5cf78`, `a8d235e`, `cf0eed3`, `6eac732`, `80178c2`, `f59492a`, `e2bc2d9`
- **Cleanup commits**: 3 — vscode portable path (`1236827`), NATIVE_CORE_EXPERIMENT superseded marker (`adc640e`), SparseSet retention annotation (`89a4b24`)
- **Native selftest**: ALL PASSED (Release x64 build)
- **Managed tests**: 472 passing (preserved baseline: 7 + 4 + 38 + 76 + 347)
- **Lessons learned**:
  - Brief's pre-flight check «verify disjoint histories» was incorrect: experimental branch was rooted off main at `3e9a001` («Phase 3 complete»). Histories share a base, but cherry-pick algorithm is unaffected by merge-base relationship — procedure works identically. Future briefs should not gate on disjoint-history assumption.
  - Brief expected `.sln` + `Core.csproj` conflicts to arise on cherry-pick 4 (Interop project), but they actually arose on cherry-pick 5 (NativeVsManaged benchmark) where the wiring was added. End-state identical, but documentation granularity was off-by-one.
  - Brief expected `entity_id.h` to arrive in cherry-pick 3, but it actually came in cherry-pick 2 (SparseSet template). Minor documentation drift, no procedural impact.
  - `BenchmarkDotNet.Artifacts` files committed in pick 7 had to be deleted via second amend after pick 6 amend already removed them — modify/delete conflict resolved trivially in favor of delete (gitignored).
  - `NATIVE_CORE.md` was absent on this branch (only `NATIVE_CORE_EXPERIMENT.md` existed); brief's defensive Test-Path check correctly handled this.
- **Blockers (resolved)**: none unresolved

### K1 — Batching primitive (bulk Add/Get + Span<T>)

- **Status**: DONE (`e2c50b8`, 2026-05-07)
- **Brief**: `tools/briefs/K1_BATCHING_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 4 new functions — `df_world_add_components_bulk`, `df_world_get_components_bulk`, `df_world_acquire_span`, `df_world_release_span` (12 → 16 total)
- **Native side**: `active_spans_` atomic counter в `World`; mutation rejection при active spans (`std::logic_error` thrown from `add_component` / `remove_component` / `destroy_entity` / `flush_destroyed`, caught at C ABI boundary so ABI stays noexcept-equivalent); `dense_data()` accessor в `RawComponentStore`
- **Managed bridge**: `NativeWorld.AddComponents` / `GetComponents` / `AcquireSpan`; `SpanLease<T>` skeleton с `ReadOnlySpan<T>` + `ReadOnlySpan<int>` access
- **Selftest scenarios**: 4 → 6 (added `scenario_bulk_operations`, `scenario_span_lifetime`)
- **Benchmark**: `NativeBulkAddBenchmark` added (execution deferred к K7)
- **Managed tests**: 472 passing (preserved baseline)
- **Lessons learned**:
  - Brief Step 2.5 specified try/catch only on the new bulk/span ABI functions, but the new throw paths added to `add_component` / `remove_component` / `destroy_entity` / `flush_destroyed` propagate through their existing capi wrappers. The wrappers without try/catch would have leaked C++ exceptions across the DLL boundary (UB). Wrapped them defensively — completeness требование, что brief implicitly предполагал но not stated.
  - Pre-flight HG-1 (working tree clean) failed because the brief itself had been authored as an unstaged modification on `main` (skeleton → 1223-line full brief). Resolved by committing «brief authoring» on `main` as a prerequisite step (`8fee2b1`) before creating the K1 branch. Future briefs in similar self-bootstrapping scenarios should call out this pattern explicitly.

### K2 — Type-id registry + bridge tests project

- **Status**: DONE (`129a0a0`, 2026-05-07)
- **Brief**: `tools/briefs/K2_REGISTRY_TESTS_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 1 new function — `df_world_register_component_type` (16 → 17 total)
- **Native side**: `World::register_component_type` с idempotent + size-conflict handling (throws `std::invalid_argument` on type_id 0, non-positive size, or size mismatch with existing registration; caught at C ABI boundary so the wrapper returns 0)
- **Managed bridge**: `ComponentTypeRegistry` instance-per-NativeWorld с sequential ids (1, 2, 3, ...); `NativeWorld(ComponentTypeRegistry?)` overload для opt-in explicit registration; internal `ResolveTypeId<T>()` centralizes the registry-vs-FNV-fallback dispatch (auto-registers on first use in registry mode)
- **Legacy compat**: `NativeComponentType<T>` + `NativeComponentTypeRegistry` marked `[Obsolete]` (warning, not error) — retained для backward compat. Will be removed at K8 cutover if Outcome 1 (native + batching wins decisively) materializes.
- **Selftest scenarios**: 6 → 7 (added `scenario_explicit_registration` covering first-registration, idempotent re-registration, size conflict rejection, type_id 0 reservation, and round-trip Add/Get against a pre-registered type)
- **Test project**: NEW `DualFrontier.Core.Interop.Tests` (xUnit + FluentAssertions) с 5 test classes / 39 tests:
  - `EntityIdPackingTests` (4 facts + 4 theory rows = 8 cases): bit-level pack/unpack invariants, capi.h spec match
  - `ComponentTypeRegistryTests` (9): sequential ids, idempotency, lookup, Count/IsRegistered semantics
  - `NativeWorldTests` (10): CRUD round-trip, multi-component coexistence, deferred-destroy semantics, Dispose throws, EntityCount tracking
  - `SpanLeaseTests` (7): acquisition/release lifetime, span access, mutation rejection, multiple concurrent leases
  - `BulkOperationsTests` (6): bulk add/get correctness, length validation, empty spans, bulk-then-span consistency
- **Managed tests**: 472 → 511 passing (+39 new)
- **Sequencing decision**: β6 chosen (см. «Sequencing decision» section). D2 in Decisions log marked RESOLVED.
- **Lessons learned**:
  - `TreatWarningsAsErrors=true` solution-wide (Directory.Build.props) caught CS0649 «field never assigned» on the placeholder test types in `ComponentTypeRegistryTests` (TypeA/B/C are used by reflection/identity only, never read). Resolved with a localized `#pragma warning disable CS0649` block. A future test-utilities pattern could centralize «empty-marker struct» helpers.

### K3 — Native bootstrap graph + thread pool

- **Status**: DONE (`7629f57`, 2026-05-07)
- **Brief**: `tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 1 new function — `df_engine_bootstrap` (17 → 18 total)
- **Native files added**: `bootstrap_graph.h/cpp` (~210 LOC), `thread_pool.h/cpp` (~150 LOC)
- **Architectural decisions implemented** (per 2026-05-07 K3 design discussion):
  - Q1 — **Thread pool scope**: Minimal (internal-only, NOT exposed via C ABI). Pool destroyed immediately after bootstrap completes; future native artifacts that need a pool create their own (D3 Lvl 1 pattern).
  - Q2 — **Tasks inventory**: 4 tasks (no placeholders) — `AllocateMemoryPools` → (`InitWorldStructure` ‖ `InitThreadPool`) → `SignalEngineReady`. `AllocateMemoryPools` is currently a no-op reserved for K7 measurements; kept as a graph node so the diamond shape exercises Kahn's parallel branches.
  - Q3 — **Topological sort**: Full Kahn's algorithm (cycle detection via processed-count comparison; generic mechanism, no hand-coded ordering).
  - Q4 — **Failure handling**: All-or-nothing with deterministic rollback (per-task cleanup invoked in reverse completion order via `BootstrapGraph::rollback`). First failure wins via atomic `compare_exchange_strong`; subsequent tasks at the same level skip their work but still flip their promise to keep the executor unblocked.
- **Throw inventory** (METHODOLOGY v1.3): 7 throw sites in C++; one new ABI function (`df_engine_bootstrap`) catches everything broadly and returns `nullptr`. RAII via `unique_ptr` in `df_engine_bootstrap` cleans up partial state on any exception path. `BootstrapGraph::run` itself catches the cycle-detection `std::logic_error` internally and reports via `last_failure()` instead of propagating.
- **Selftest scenarios**: 7 → 12 (added `scenario_bootstrap_basic`, `scenario_bootstrap_double_rejected`, `scenario_bootstrap_graph_topological`, `scenario_bootstrap_graph_parallel`, `scenario_bootstrap_rollback_on_failure`)
- **Selftest build adjustment**: switched `df_native_selftest` from "link against the DLL" to "compile sources directly into the executable" so non-`DF_API`-exported C++ classes (`BootstrapGraph`, `ThreadPool`) resolve. The DLL build target itself is unchanged.
- **Bridge tests**: 39 → 45 (+6 K3 BootstrapTests)
- **Benchmark**: `BootstrapTimeBenchmark` added (execution deferred to K7)
- **Managed bridge naming**: brief proposed `Bootstrap.Bootstrap()` but C# rejects a static method with the same name as its enclosing static class (CS0542). Renamed to `Bootstrap.Run(registry)` — semantically reads as "run bootstrap".
- **Lessons learned**:
  - Brief's expected `dotnet test` baseline was 511 but actual baseline was 466 (76 + 4 + 39 + 347). Brief author miscounted; actual K3 closure numbers used in this entry.
  - Brief's selftest scenario code used `dualfrontier::BootstrapGraph` and `dualfrontier::ThreadPool` directly. These are intentionally NOT `__declspec(dllexport)`-marked per Q1, so linking the selftest against the DLL fails on MSVC. Resolved by changing the selftest target to compile the source files directly (see CMakeLists update). A future option would be to introduce a `DF_API_INTERNAL` macro that exports under `DF_NATIVE_BUILDING_DLL` only, but the source-compile approach is simpler and matches the test's standalone nature.
  - Pre-flight HG-1 (working tree clean) failed because the K3 brief itself was an unstaged modification on `main` (skeleton → 1700-line full brief). Resolved by committing "brief authoring" on `main` as Step 0 (`3b18cb0`). Methodology v1.3 already calls this out — Step 0 worked as designed.

### K4 — Component struct refactor (Hybrid Path)

- **Status**: DONE (`2fc59d1`, 2026-05-08)
- **Brief**: `tools/briefs/K4_STRUCT_REFACTOR_BRIEF.md` (FULL EXECUTED)
- **Architectural decisions implemented** (per 2026-05-07 K4 design discussion):
  - Q1 — **Hybrid Path**: Trivial POCO components → struct (native batching path), components с reference types (Dictionary/List/HashSet/string) stay as class (managed path). Cleanness > expediency.
  - Q2 — **Per-component atomic commits**: 24 components = 24 commits across 7 batches. Bisect-friendly.
  - Q3 — **Explicit registration at Application bootstrap**: VanillaComponentRegistration.RegisterAll(registry) called once. Sequential type IDs 1..24.
  - Q4 — **Smoke + tricky-case tests**: 7 new tests. Existing tests cover behavior через consumer systems.
- **Components converted** (24 in Категория A):
  - Shared (3): Health, Position, Race
  - Pawn trivial (3): Needs, Mind, Job
  - Items (5): Bed, Consumable, DecorativeAura, Reservation, WaterSource
  - World (3): Tile, Biome, EtherNode
  - Magic (4): Ether, GolemBond, Mana, School
  - Combat (4): Ammo, Armor, Shield, Weapon
  - Building trivial (2): PowerConsumer, PowerProducer
- **Components staying as class** (7 in Категории B+C):
  - Collections (5): Movement (List), Skills/Social (Dictionary), Storage (Dictionary+HashSet), Workbench (string?)
  - Strings (2): Identity, Faction
- **VanillaComponentRegistration.cs**: Application/Bootstrap helper, registers all 24 на Application init
- **Test count**: 517 → 524 (+7 K4 roundtrip tests)
- **System code changes**: ONE — ElectricGridSystem foreach with tuple deconstruction needed a mutable local copy (CS1654). All other systems' get/modify/set patterns worked unchanged со struct copy semantics.
- **Lessons learned**:
  - C# CS8983: structs with field initializers require an explicit parameterless constructor. The brief's «keep default initializers» guidance had to be qualified — keep them only где default value differs from `default(T)` (e.g. MindComponent.Mood = 0.5f); drop them где the initializer matches the type's natural default (e.g. RaceKind.Human = 0, JobKind.Idle = 0, OwnershipMode.Bonded = 0). Saves an explicit ctor on most components.
  - C# CS1654: tuple-deconstructed foreach variables (`foreach (var (e, c) in pairs)`) are read-only locals; mutating struct members fails. Workaround: iterate with single variable (`foreach (var pair in pairs)`) then copy to a mutable local. Caught by ElectricGridSystem build failure after PowerConsumerComponent → struct.
  - `[ModAccessible]` attribute had `AttributeTargets.Class` only. Widened к `Class | Struct` as a K4 prerequisite commit before component conversions.
  - Brief expected baseline 472 tests (45 outdated count) but actual was 517 — extra tests added between K3 and K4. Final delta +7 still matched exactly.
  - DualFrontier.Application project did not previously reference DualFrontier.Core.Interop (registry symbol owner). Reference added as part of the VanillaComponentRegistration commit.

### K5 — Span<T> protocol + Command Buffer write batching

- **Status**: DONE (`547c919`, 2026-05-08)
- **Brief**: `tools/briefs/K5_SPAN_PROTOCOL_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 6 new functions (18 → 24 total): `df_world_begin_batch`, `df_batch_record_update`, `df_batch_record_add`, `df_batch_record_remove`, `df_batch_flush`, `df_batch_cancel`, `df_batch_destroy`
- **Native files extended**: `world.h/cpp` — added WriteBatch class, CommandKind enum, WriteCommand struct, `World::active_batches_` atomic counter; private `add_component_unchecked` / `remove_component_unchecked` (friend access from WriteBatch) for in-flush mutations that skip the active_batches gate but still honour active_spans.
- **Architectural decisions implemented** (per 2026-05-08 K5 design discussion):
  - Q1 — **ArrayPool fix scope**: Both `AddComponents` and `GetComponents` fixed (`System.Buffers.ArrayPool<ulong>.Shared.Rent/Return` for batches > 256). Eliminates 80 KB heap allocation observed in K3 PERFORMANCE_REPORT Measurement 2. Stackalloc path for ≤ 256 unchanged.
  - Q2 — **Command Buffer pattern**: System code never directly mutates native memory. Mutations recorded as commands (Update / Add / Remove), validated native-side (entity liveness via stored version), applied atomically at flush time. Preserves native sovereignty + managed safety + mod safety + audit observability.
  - Q3 — **Scope additions**: `SpanLease<T>.Pairs` iteration helper resolves K1 skeleton's deferred 'paired iteration helpers' comment. NOT included: lease pooling, WriteBatch pooling, SIMD command application — defer to K7 evidence.
  - Q4 — **Comprehensive tests**: 14 new tests (11 WriteBatch + 2 ArrayPool + 1 Pairs).
- **Throw inventory** (METHODOLOGY v1.3): 5 new throw points в WriteBatch (ctor invalid_argument на null world / type_id 0 / non-positive size, flush logic_error на double-flush or post-cancel flush, record bad_alloc on vector growth). All caught at C ABI boundary returning sentinel values (nullptr / 0 / -1).
- **Mutation rejection extended**: `World::add_component` / `remove_component` / `destroy_entity` / `flush_destroyed` / `add_components_bulk` now also reject if `active_batches_ > 0` (parallel to existing `active_spans_` check).
- **Multi-batch concurrency**: brief's flush() counter-manipulation pattern would deadlock with two concurrent batches (each batch sees the other's contribution). Replaced with `friend class WriteBatch` + private `*_unchecked` mutation paths — flush bypasses the `active_batches_` gate entirely while still honouring the `active_spans_` contract. Counter now adjusted only by ctor/dtor.
- **Selftest scenarios**: 12 → 17 (+5: batch_basic, batch_mixed_commands, batch_cancel, batch_dead_entity_skipped, batch_mutation_rejection)
- **Bridge tests**: 524 → 538 (+14 K5)
- **System code changes**: NONE — K5 is purely infrastructure. K8 cutover will migrate systems to WriteBatch usage.
- **Lessons learned**:
  - The brief's flush() design temporarily decremented `active_batches_` before applying commands and re-incremented after. With a single batch this works, но в "MultipleConcurrentBatches" test the counter stays > 0 after a batch decrements its own contribution (the peer batch's contribution remains), so the in-flush mutation still throws. Replaced with friend-access internal `*_unchecked` methods — far cleaner и safer.
  - `Span<ulong>` variable that may be either `stackalloc` или a pool-rented array fails C# escape analysis (CS9081). Adding the `scoped` modifier на the local resolves it; the same issue would trip up any future "either-stack-or-rent" pattern in this codebase.
  - `SpanLease<T>.Pairs` returns `EntityId` with `Version=1` placeholder — flagged in code/doc comments as a K7 follow-up. Acceptable for K5 since flush validates entity version при apply (stale ids silently skipped).

### K6 — Second-graph rebuild on mod change

- **Status**: DONE (`cb3d6cf`..`af2b572`, 2026-05-09)
- **Brief**: `tools/briefs/K6_MOD_REBUILD_BRIEF.md` (FULL EXECUTED as closure-shaped implementation brief — third brief type alongside «implementation» and «skeleton»)
- **Closure shape**: Most K6 deliverables were already fulfilled by parallel MOD_OS migration M7.1–M7.3 work (pause/resume primitives `0606c43`, full §9.5 unload chain `c3f5251`, ALC verification + Phase 2 carried debt `1d43858`). The K6 brief executed five additional phases:
  1. **Phase 0 pre-flight + 0.4 inventory** — every K6 deliverable verified present on disk, one expected gap (ModFaultHandler) confirmed.
  2. **Phase 1 verification** of M7-era code against K6 contract — log at `tools/briefs/K6_VERIFICATION_LOG.md` (commit `62ff956`).
  3. **Phase 2 drift reconciliation (Option C)** — KERNEL_ARCHITECTURE.md amended to v1.1 (`cb3d6cf` status bump, `ab581cb` §K6 wording reconciliation) AND D4 decision-log entry added here (`30b982b`) for the audit trail.
  4. **Phase 3 adjacent debt fill** — `ModFaultHandler` implementation closing the «Phase 2 part 2» TODO in `ModLoader.HandleModFault`. Files: `ModFaultHandler.cs` new (`a6664cf`); `ModLoader.HandleModFault` rewired (`208e9e7`); `ModIntegrationPipeline` ctor + Apply drain (`4999926`); `ModFaultHandlerTests.cs` 9 tests (`af2b572`).
  5. **Phase 4 coverage audit** — verified existing `Pipeline_build_failure_leaves_old_scheduler_intact` covers the cyclic-graph-build rollback path; `UnloadMod_OnNonActiveMod_ReturnsEmptyWarnings_NoThrow` and `UnloadAll_OnEmptyActiveSet_RebuildsKernelOnlyScheduler` cover the other Phase 4 candidates. No gap-fill commit needed.
- **Test count**: 538 baseline → **547 passing** (+9 ModFaultHandler tests). Distribution: 76 Core + 4 Persistence + 66 Interop + 38 Systems + 356 Modding + 7 Mod.ManifestRewriter.
- **Out of K6 scope (deferred → resolved by K6.1)**: Full `ParallelSystemScheduler`/`SystemExecutionContext` rewiring to install the real `ModFaultHandler` in place of the `NullModFaultSink` default. The K6 brief Phase 3.3 example only wired `ModLoader.HandleModFault` (a defensive entry-point for callers holding a `ModLoader` ref but not a `ModFaultHandler` ref); the actual fault routing path (`SystemExecutionContext.RouteAndThrow` → `_faultSink.ReportFault`) still uses the null sink, so a real isolation violation surfaces only via `IsolationViolationException` and does not yet drive the deferred-unload queue. Full wiring requires construction-order changes (handler must exist before the scheduler that needs it) and is left for a future ticket — flagged here so the gap is visible. **Resolved by K6.1 (`fe03ed3`..`a642d65`, 2026-05-09) — see K6.1 closure section below.**
- **Lessons learned**:
  - M-series (mod migration) and K-series (kernel migration) have meaningful overlap. Future skeleton briefs should cross-check overlapping migration phases before being authored as full implementation briefs — the K6 case shows that a deliverable nominally in the kernel track may already be fulfilled by mod-track work.
  - «Closure-shaped implementation brief» is a third brief type alongside «implementation» and «skeleton». Used when the milestone's deliverables exist but verification, drift reconciliation, and adjacent debt are needed for closure. The format is documented in K6_MOD_REBUILD_BRIEF.md «Methodology note on closure-shaped briefs».
  - `IModFaultSink` interface (Core-side) was authored during M3-era work but the Application-side `ModFaultHandler` was deferred. K6 closure exposed the deferred work as a real gap (mod isolation violations would crash with `NotImplementedException` if `HandleModFault` was ever reached). The fix lands as part of K6 because K6 closure semantically requires the fault → unload path infrastructure to exist; the upstream wiring to make it active end-to-end remains as a future ticket per the «out of K6 scope» note above.
  - Pre-flight verification revealed only one substantive drift (`ParallelSystemScheduler.Rebuild` is `internal` not `public` per spec wording — class itself is `internal sealed`, so the access modifier is the right visibility for the rebuild surface). All other M7-era deliverables matched the K6 contract verbatim. The closure-shaped brief format with explicit verification log is what surfaced this — a non-closure-shaped brief might have re-implemented existing infrastructure from scratch.

### K6.1 — Mod fault wiring end-to-end

- **Status**: DONE (`fe03ed3`..`a642d65`, 2026-05-09)
- **Brief**: `tools/briefs/K6_1_FAULT_WIRING_BRIEF.md` (FULL EXECUTED)
- **Goal**: close the wiring gap explicitly flagged in K6 closure ("Out of K6 scope (deferred)"). After K6, `ModFaultHandler` existed as infrastructure but `ParallelSystemScheduler` defaulted to `NullModFaultSink` and `BuildContext` hardcoded `SystemOrigin.Core`/`modId: null` for every system. K6.1 makes the fault path active end-to-end: a mod system that commits an isolation violation routes through `RouteAndThrow` → real `ModFaultHandler.ReportFault`, and the next `ModIntegrationPipeline.Apply` drains the faulted mod from the active set.
- **Architectural change**: ownership of `ModFaultHandler` inverted from `ModIntegrationPipeline` to `GameBootstrap`. Handler created before scheduler; passed as immutable reference into scheduler ctor; pipeline receives reference for query at Apply time; loader wired via `SetFaultHandler` directly by the bootstrap. This eliminates the circular construction dependency that K6 worked around with a setter-after-construction pattern (cross-cutting design constraint #1 of the K6.1 brief).
- **Per-system origin propagation**: introduced `SystemMetadata` record (Core-side) and `SystemMetadataBuilder` (Application-side projection from `ModRegistry.GetAllSystems()`). Scheduler accepts metadata dictionary at construction and at every `Rebuild` call; `BuildContext` reads each system's actual origin/modId from the table instead of hardcoding `Core`/`null`. Layer discipline preserved per Option C of the K6.1 brief §1.3 — Core gets a minimal projection, `SystemRegistration` stays in Application.
- **Non-optional faultSink**: scheduler ctor's `faultSink` parameter is now required (no silent `?? new NullModFaultSink()` default). Tests that don't exercise faults pass `new NullModFaultSink()` explicitly, so the silent-default bug K6.1 fixes can never re-emerge unnoticed.
- **Deliverables**:
  - `SystemMetadata.cs` new (Core)
  - `ParallelSystemScheduler` ctor + Rebuild + BuildContext rewritten to propagate metadata + non-optional fault sink
  - `ModFaultHandler` ctor parameter removed (no more pipeline dependency); handler is now a self-contained fault accumulator
  - `SystemMetadataBuilder.cs` new (Application) — single projection helper called from both bootstrap and pipeline
  - `GameBootstrap.CreateLoop` bootstrap order restructured (handler before scheduler; loader wired before any mods can fault)
  - `ModIntegrationPipeline` ctor + Apply [step 8] + UnloadMod [step 4] + UnloadAll empty-set path updated to propagate metadata
  - `K6_1_AFFECTED_TESTS.md` (operational artifact, lists test files touched)
  - `SchedulerTestFixture.cs` helper for non-fault test scenarios (Modding.Tests-side; Core/Systems tests use inline construction)
  - `K6_1_FaultRoutingEndToEndTests.cs` 6 new end-to-end fault tests covering: mod-isolation-violation reports fault, next Apply drains, core violations don't route, multiple mods recorded, only-faulting-mod recorded, fault during Initialize captured before ctor throws
  - K6 `ModFaultHandlerTests.BuildPipeline` updated for new ctor signatures with shared handler across loader/scheduler/pipeline (folded into the Modding.Tests update commit since the helper sits inline with the rest of the file's pipeline construction)
  - 11 affected non-fault tests in Modding.Tests + 3 in Core.Tests + 9 in Systems.Tests updated for the new scheduler ctor signature
- **Test count**: 547 baseline → **553 passing** (+6 new K6.1 end-to-end tests, no regressions). Distribution: 76 Core + 4 Persistence + 66 Interop + 38 Systems + 362 Modding + 7 Mod.ManifestRewriter.
- **Lessons learned**:
  - Setter-after-construction was the K6 workaround; K6.1 confirms restructuring ownership at the orchestrator layer is the right shape. The cost (touching `GameBootstrap` + ctor signatures across ~24 test files) is bounded; the benefit (immutable scheduler, no temporal coupling around "is the sink wired yet?") is structural and persists for the lifetime of the project.
  - `SystemRegistration` already carried `Origin` + `ModId` since K4-era work, but the data was unused at the scheduler boundary because the scheduler is in Core (no access to `SystemRegistration` which is in Application). The Core-side `SystemMetadata` projection bridges this without the layer violation that putting `SystemRegistration` itself in Core would cause. Cross-layer projection via a minimal record is the correct shape; an `ISystemMetadata` interface for two fields would be over-engineered (rejected per brief §1.3).
  - The K6 closure-shaped brief format successfully flagged the wiring gap in MIGRATION_PROGRESS.md. K6.1 is the validation: an explicitly-flagged deferred gap turned into a focused follow-up milestone with bounded scope. The pattern — closure-shaped brief surfaces a deferred gap → focused follow-up milestone closes it — is now an established methodology, not a one-off.
  - Test design subtlety: `Parallel.ForEach` may stop dispatching new partitions when one item throws, so a test that expects two parallel mod systems to both fault in a single phase is sensitive to host parallelism. K6.1's `MultipleModSystems_BothFault_AllIdsRecorded` was originally written with both systems in one phase and was flaky on low-core machines; rewritten to use one phase per system + `ExecutePhase` per phase with a per-phase try/catch for deterministic coverage of the fault-routing path.
  - `SystemBase.Context` is vestigial — never assigned by the scheduler. The K6.1 test fixtures originally tried to use `Context.SetComponent(...)` and would have NPE'd; the working pattern is `SystemExecutionContext.Current!.SetComponent(...)` (the thread-local is what the scheduler actually pushes). The vestigial `Context` property is a latent footgun that should probably be removed in a future cleanup commit, but K6.1 stayed scope-disciplined and only documented the issue here.

---

## M9-series progress (runtime)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| M9.0 | Win32 window + Vulkan instance + clear color | NOT STARTED | 4–5 days | — | — |
| M9.1 | Textured quad pipeline (single sprite) | NOT STARTED | 3–4 days | — | — |
| M9.2 | Batched sprite renderer (instancing) | NOT STARTED | 4–5 days | — | — |
| M9.3 | TileMap parity (M8.9 visual reproduction) | NOT STARTED | 4–6 days | — | — |
| M9.4 | Input layer (Win32 message pump → IInputAdapter) | NOT STARTED | 3–4 days | — | — |
| M9.5 | Domain integration (M8.9 full parity, switchable backend) | NOT STARTED | 1 week | — | — |
| M9.6 | UI primitives (text rendering, panels) | NOT STARTED | 1–2 weeks | — | — |
| M9.7 | Coupled lifecycle (pause/focus parity с Godot M8.10) | NOT STARTED | 2–3 days | — | — |
| M9.8 | Godot deletion (point of no return для runtime) | NOT STARTED | 2–3 days | — | — |

**Cumulative estimate**: 4–7 weeks at hobby pace.

### M9.0 — M9.8

Detailed entries будут добавлены при подходе к каждому milestone.

---

## Cross-series coupling

Зависимости и взаимовлияния между K и M series, которые могут проявиться в ходе исполнения:

| K milestone | M milestone | Potential coupling |
|---|---|---|
| K4 (struct refactor) | M9.5 (Domain integration) | Если M9.5 идёт раньше K4, Domain integration работает с class-based components. После K4 — adjustment может потребоваться. |
| K5 (span protocol) | None direct | Self-contained в kernel layer |
| K8 (cutover) | M9.5 (Domain integration) | Если K8 Outcome 1 (native wins) до M9.5 — Domain integration сразу нацеливается на NativeWorld. Иначе — на managed World с возможным позднейшим switchover. |
| K7 (perf measurement) | None direct | Информирует K8 decision, не блокирует runtime |
| Все K | M9.8 (Godot deletion) | Godot deletion это точка невозврата ТОЛЬКО для runtime. Kernel decision (K8) независим — managed World остаётся fallback. |

**Invariant**: Kernel и runtime — два независимых стека под managed Application layer. Координация требуется только при cutover-точках (K8, M9.5, M9.8).

---

## Decisions log (operational, не архитектурные)

Решения, принятые в ходе исполнения миграции. Архитектурные LOCKED-решения см. в `KERNEL_ARCHITECTURE.md` Part 0 и `RUNTIME_ARCHITECTURE.md` Part 0.

### D1 — Single progress tracker для K и M
- **Date**: 2026-05-07
- **Decision**: Один файл `MIGRATION_PROGRESS.md`, не разделение на KERNEL_PROGRESS + RUNTIME_PROGRESS
- **Rationale**: sequencing β3/β5/β6 — это решение про взаимосвязь стеков. Раздельные файлы потеряют это видение. Cross-series coupling table требует обоих списков рядом.
- **Reversal trigger**: если документ превысит ~50KB или K и M прогресс начнут существенно расходиться по темпу — рассмотреть split.

### D2 — Sequencing decision deferred к after K2 (RESOLVED 2026-05-07)
- **Original date**: 2026-05-07
- **Original decision**: β3/β5/β6 не выбирается заранее. K0–K2 выполняются first как preservation + bridge maturity (общий шаг для всех вариантов).
- **Resolution date**: 2026-05-07 (per K2 closure)
- **Resolved decision**: **β6 — kernel-first sequential** (K0–K8 → M9.0–M9.8). См. «Sequencing decision» section выше для full rationale + rejected alternatives.
- **Rationale (resolved)**: K2 closure provided sufficient bridge-maturity evidence (39 tests + selftest 7/7 + verified P/Invoke patterns). Single architectural focus per period preserves cleanness; K3–K8 kernel work has no runtime dependencies, so deferring M9.x is non-blocking.

### D3 — Native organicity Lvl 1 как foundational pattern для всего native слоя
- **Date**: 2026-05-07
- **Scope**: Не разовое решение для kernel + runtime, а **архитектурный паттерн проекта** для любого native артефакта — текущего или будущего.

#### Decision

Каждый native артефакт в Dual Frontier — **независимый проект**:
- Отдельный `.dll` (свой build target)
- Отдельный CMake build
- Отдельный узкий C ABI
- Отдельный Interop bridge на managed стороне
- Свой selftest, свои тесты
- Не знает о существовании других native артефактов

**Никакого shared native code между артефактами**: общий thread pool, общий allocator, общий logger, единый монолитный DLL — всё это **отвергается** как foundational rule.

**Координация артефактов** происходит **только** на managed стороне — через interfaces (`IProjectileCompute`, `INativeWorld`, `IRenderer` и т.д.) с DI-регистрацией. Domain не знает какой backend активен.

#### Артефакты, к которым применяется паттерн

**Подтверждённые** (зафиксированы в LOCKED architectural docs):
- `DualFrontier.Core.Native.dll` — ECS kernel (KERNEL_ARCHITECTURE.md)
- `DualFrontier.Runtime.Native.dll` — Vulkan rendering (RUNTIME_ARCHITECTURE.md)

**Запланированные** (зафиксированы в research/roadmap docs):
- GPU Compute pipeline для `ProjectileSystem` — Phase 5 «Battle of the Gods» threshold (GPU_COMPUTE.md). Уже спроектирован через `IProjectileCompute` interface — готовый к Lvl 1 паттерну без переделки.

**Потенциальные** (могут возникнуть в будущем):
- Audio engine, если решит уйти от managed
- AI inference (нейросети для пешек/животных), если потребует native
- Networking layer для multiplayer
- Physics engine, если custom потребуется
- Любой другой compute-heavy domain

Каждый из них автоматически получает Lvl 1 контракт — без необходимости заново открывать архитектурную дискуссию.

#### Rationale

1. **Consistency с операционным принципом проекта «никто никуда не лезет»**:
   - Mods isolated через AssemblyLoadContext + IModApi
   - Domain isolated от Presentation через PresentationBridge
   - Systems isolated через SystemExecutionContext (crash в DEBUG при undeclared access)
   - Native↔Managed isolated через single ownership boundary
   - Native↔Native теперь следует тому же правилу — паттерн распространяется тотально

2. **Open-source-separately property сохраняется per-артефакт**: kernel может стать standalone «sparse-set ECS in C++». Runtime — standalone «2D Vulkan runtime». Compute — standalone «Vulkan compute pipeline для projectile-style workloads». Каждый artifact имеет ценность независимо.

3. **Independent failure domains**: bug в compute pipeline не затрагивает kernel storage. Crash в Vulkan не убивает ECS. Отладка локализована per-артефакт.

4. **Independent build/test cycles**: при работе над kernel не нужно компилировать runtime. CTest selftest каждого артефакта standalone. Pipeline metrics локальны.

5. **Mod-friendly extensibility**: моды могут зарегистрировать свой `IProjectileCompute` или другую compute backend через тот же DI mechanism. Lvl 2/Lvl 3 это бы заблокировали — мод не может встроиться в shared internals.

6. **Use-case flexibility**:
   - Headless dedicated server: kernel + compute, без runtime → работает (Lvl 3 невозможно)
   - Migration period: kernel + Godot, без native runtime → работает (Lvl 2 ломает)
   - Compute disabled на слабом железе: kernel + runtime, без compute → работает
   - Любая комбинация артефактов валидна

#### Rejected alternatives

- **Lvl 2** (shared native infrastructure — общий thread pool, allocator, logger между артефактами): premature optimization. Артефакты имеют фундаментально разные work characteristics (kernel thread pool idle после bootstrap, runtime Vulkan thread per-frame, compute thread в момент dispatch). Shared infrastructure создаст coupling без выигрыша.
- **Lvl 3** (один монолитный native DLL): ломает open-source-separately, ломает headless server use-case, ломает mod extensibility, нарушает conceptual integrity (ECS storage + Vulkan rendering + compute pipeline не имеют общего concern).

#### Reversal trigger

**Per-артефакт основание требуется**, не глобальное. Конкретный pair артефактов (например, runtime + compute) может быть рассмотрен на shared infrastructure **только при наличии всех трёх условий**:

1. Profiling на weak hardware показывает измеримую боль (thread oversubscription, allocator fragmentation, etc.)
2. Боль не решается оптимизацией внутри артефакта
3. Прошло минимум 12 месяцев с production использования обоих артефактов (evidence base достаточен)

Decision на shared infrastructure требует **отдельного architectural milestone** с amendment к LOCKED docs обоих артефактов и явным трейдоффом против выгод Lvl 1 паттерна.

#### Implication для cross-series coupling table

Остаётся как есть — координация K-серии и M-серии только в cutover-точках (K8, M9.5, M9.8). Никаких shared native artifacts. При появлении третьего артефакта (например, GPU Compute) — добавляется свой столбец без изменения существующих coupling правил.

#### Implication для open questions

OQ3 («Cross-document drift между KERNEL_ARCHITECTURE и RUNTIME_ARCHITECTURE») расширяется: при добавлении третьего native артефакта (например, COMPUTE_ARCHITECTURE.md) — drift-prevention требуется тройной cross-reference. Currently не активно (compute artifact не существует), но при подходе к Phase 5 — переоткрыть.

### D4 — K6 deliverables fulfilled differently than KERNEL §K6 v1.0 wording suggested
- **Date**: 2026-05-09
- **Decision**: KERNEL_ARCHITECTURE.md §K6 v1.0 wording is acknowledged as speculative pre-implementation; the M7-era realization is authoritative going forward. Recorded as Option C of K6 brief Phase 2 reconciliation: KERNEL spec wording was amended (commits `cb3d6cf` v1.1 status bump, `ab581cb` §K6 reconciliation) AND this decision-log entry captures the rationale for the audit trail.
- **Drift inventory** (per `tools/briefs/K6_MOD_REBUILD_BRIEF.md` §2.1):
  - `SystemGraph.Rebuild(modRegistry)` (skeleton wording) → `DependencyGraph.Reset() + AddSystem + Build()` inline in `ModIntegrationPipeline.UnloadMod` step 4 and `Apply` steps [5-7]. No standalone Rebuild method exists; the operation is inline and atomic per call site.
  - `ModLoader.UnloadMod + ReloadMod` (skeleton wording) → `ModLoader.UnloadMod(modId)` exists; `ReloadMod` is composed at the pipeline level as `Pause + UnloadMod + Apply([newPath]) + Resume` rather than offered as a single primitive.
  - `PhaseCoordinator.OnModChanged()` event handler (skeleton wording) → no `PhaseCoordinator` class exists. The pause-rebuild-resume contract is composed across `GameLoop.SetPaused(true)` + `ModIntegrationPipeline.Apply(...)` + `GameLoop.SetPaused(false)`, gated by `ModIntegrationPipeline.IsRunning` per MOD_OS_ARCHITECTURE §9.3.
- **Rationale**: Functional contract met everywhere — pause-then-rebuild-then-resume is operational, ALC unload chain is per §9.5, graph rebuilds correctly. The drift is purely about wording and decomposition. Pre-implementation specs cannot accurately predict the realized API surface; the closure-shaped K6 brief format (verify-then-fill rather than build-from-scratch) exists precisely because parallel migration tracks (M-series) had already produced the deliverables.
- **Why both spec amendment AND decision log** (Option C of brief Phase 2): the spec amendment fixes the immediate reading; this entry preserves *why* the wording changed for future audit. Spec-only would lose the rationale; log-only would leave readers of KERNEL §K6 alone with stale wording.
- **No future trigger**: this drift is closed. Future K-series milestones may exhibit similar overlap with M-series work; the closure-shaped brief methodology (per K6 brief Methodology note §) is the established response, not a new D-entry per occurrence.

---

## Open questions

Вопросы, которые проявились в процессе планирования но решение которых отложено к моменту, когда появятся данные.

### OQ1 — Branch strategy для миграции
- **Question**: Один long-lived `feature/native-kernel` branch для всей K-series, или per-K feature branches с merge на main?
- **Trigger to resolve**: до K0 execution (требуется в K0 brief)
- **Lean**: per-K feature branches + atomic merge на main per closure (matches existing M8.x pattern)

### OQ2 — Rollback policy для K4 (struct refactor)
- **Question**: K4 это 2-3 weeks scope с ~50-80 component conversions. Если в середине обнаружится фундаментальная проблема (например, behavior class split не работает для X компонентов) — есть ли клин-rollback?
- **Trigger to resolve**: при K4 brief authoring
- **Lean**: incremental merge per 5-10 components (each commit independent), revert simple

### OQ3 — Cross-document drift между KERNEL_ARCHITECTURE и RUNTIME_ARCHITECTURE
- **Question**: Два LOCKED v1.0 документа эволюционируют параллельно. Как избежать дрейфа decisions (например, K-L4 explicit registry vs L-аналог в RUNTIME)?
- **Trigger to resolve**: при первом обнаружении конфликта
- **Lean**: при amendment одного документа — обязательная проверка cross-references в другом

### OQ4 — Что если K7 показал, что native не нужен?
- **Question**: K8 Outcome 2/3 означают что native kernel parking. Что делать с уже выполненными K0-K7 (особенно K3 native scheduler, K5 span protocol)?
- **Trigger to resolve**: после K7 measurements
- **Lean**: K8 brief предусматривает три outcome-варианта; recording rationale + lessons learned обязательны

---

## Closure protocol

Когда milestone закрывается:

1. **Run final verification**: `dotnet build`, `dotnet test`, native selftest (для K-series), F5 verification (для M-series)
2. **Atomic commit** с scope prefix (`feat`/`fix`/`refactor`/`test`/`docs`/`native`/`interop`)
3. **Update этот документ**:
   - Status: NOT STARTED → DONE
   - Commit hash + date filled
   - Add «Lessons learned» entry если что-то нетривиальное обнаружилось
   - Update `Current state snapshot` table
   - Если milestone разрешил OQ — переместить из «Open questions» в «Decisions log»
4. **Update brief**: mark brief as EXECUTED + add link к commit в `tools/briefs/K{N}_*.md`

**Pre-flight для самого update**: pre-flight grep на «NOT STARTED» / «IN PROGRESS» — убедиться что только один milestone в IN PROGRESS одновременно.

---

## Document end

Single point of view на 9-15 weeks of work. Обновляется per milestone closure. Архитектурные решения остаются LOCKED в companion documents.
