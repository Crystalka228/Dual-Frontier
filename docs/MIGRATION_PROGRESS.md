---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-C-MIGRATION_PROGRESS
category: C
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-C-MIGRATION_PROGRESS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-C-MIGRATION_PROGRESS
category: C
tier: 2
lifecycle: Live
owner: Crystalka
version: "Live"
next_review_due: 2026-Q3
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-C-MIGRATION_PROGRESS
---
# Native Migration — Progress Tracker

**Status**: LIVE document (не LOCKED) — обновляется при каждом milestone closure
**Created**: 2026-05-07
**Last updated**: 2026-05-10 (A'.4 K9 closure — RawTileField field storage + IModApi v3 Fields wiring complete; 17-commit bundle `d163341..<HEAD>` on `feat/k9-field-storage`; native selftest 21 → 29 scenarios; bridge tests +27 (FieldRegistry/FieldHandle/IsotropicDiffusionKernel); capability regex extended with field.*/pipeline.* verbs)
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
| **Active phase** | A'.5 K8.3 (production system migration → SpanLease/WriteBatch) → A'.6 K8.4 / A'.7 K8.5 / A'.8 К-closure report / A'.9 architectural analyzer (А'.4 K9 + A'.1.K + A'.1.M + A'.0.5 + A'.0.7 + К-L3.1 + A'.3 push closed) |
| **Last completed milestone** | **A'.4** (K9 + A'.4.0 patch bundled per Crystalka 2026-05-10 «всё в одну сессию, окно контекста позволяет») — RawTileField field storage; 12-function C ABI; FieldHandle/FieldSpanLease/FieldRegistry managed bridge; CPU IsotropicDiffusionKernel reference; IModApi v3 Fields + ComputePipelines surface wiring; capability regex field.*/pipeline.* extension. Branch `feat/k9-field-storage` `d163341..<HEAD>` (17 commits), 2026-05-10. Previous: A'.1.K (К-L3.1 architecture amendment landing) — 2026-05-10; A'.1.M (A'.0.7 methodology amendment landing) — 2026-05-10; A'.0.7 (methodology pipeline restructure) — 2026-05-10; A'.0.5 (documentation reorganization) — 2026-05-10. |
| **Next milestone (recommended)** | A'.5 K8.3 (production system migration to SpanLease/WriteBatch — 12+ vanilla systems per migration plan §1.2) |
| **Sequencing strategy** | β6 — kernel-first sequential (decided 2026-05-07 per K2 closure); K8 split into sub-milestones K8.0-K8.5 per K8.0 closure (2026-05-09); K8.2 reformulated as v2 single-milestone foundation closure per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 LOCKED |
| **Combined estimate** | 9-15 weeks (5-8 kernel + 4-7 runtime) |
| **Tests passing** | 671 expected post-A'.4 K9 (631 baseline + 27 K9 bridge + 13 K9 capability validation = 671; K9 bridge tests verified 27/27 PASSED 0.92s; native selftest 21 → 29 scenarios verified). Final dotnet test verification at K9 closure. |

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
| K7 | Performance measurement (tick-loop) | DONE | 3–5 days hobby pace (~4-6h auto-mode) | `72ea8b5`..`e917220` | 2026-05-09 |
| K8.0 | Architectural decision recording (Solution A) | DONE | 1-2 days | `9f9dc05`..`5fa3f1d` | 2026-05-09 |
| K8.1 | Native-side reference handling primitives | DONE | 8-14 hours auto-mode | `a62c1f3`..`812df98` | 2026-05-09 |
| K8.1.1 | InternedString closure follow-up (cross-pool equality, doc semantics, empty-string coverage) | DONE | 1-3 hours auto-mode | `f8273ca`..`16afdf3` | 2026-05-09 |
| K-Lessons | Pipeline closure lessons formalization (METHODOLOGY v1.5 + KERNEL v1.3) | DONE | 30-60 min auto-mode | `9df2709`..`071ae11` | 2026-05-09 |
| K8.2 | K-L3 selective per-component closure (post-K-L3.1 reframing): K8.1 wrapper value-type refactor + 6 class→struct conversions + 6 empty TODO stub deletions + 12 ModAccessible annotation pass | DONE | 6-12 hours auto-mode | `6ee1a85`..`7527d00` (preceded by 3 main commits: `c834f18` migration plan lock, `b1a461e` v1 deprecate, `88cbe3f` v2 brief author) | 2026-05-09 |
| K8.3 | 12 vanilla systems migrated to SpanLease/WriteBatch | NOT STARTED | 2-3 weeks | — | — |
| K8.4 | ManagedWorld retired; Mod API v3 ships | NOT STARTED | 1 week | — | — |
| K8.5 | Mod ecosystem migration prep | NOT STARTED | 3-5 days | — | — |
| K9 | Field storage abstraction (RawTileField + IModApi v3 Fields wiring) | DONE | 8-12 hours auto-mode | `d163341..<HEAD>` on `feat/k9-field-storage` | 2026-05-10 |

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

### K7 — Performance measurement (tick-loop)

- **Status**: DONE (`72ea8b5`..`e917220`, 2026-05-09)
- **Brief**: `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` (FULL EXECUTED)
- **Hardware**: AMD Ryzen 7 7435HS, 32 GB DDR5-4800, Win11 25H2 ("Skarlet" — Crystalka's primary dev machine, treated as median target audience hardware per K7 brief Phase 1)
- **Variants measured**:
  - V1 managed-current (pre-K4 worktree at `9227a577`) — simplified depletion-only loop on 50 entities × 2 systems per K7 brief stop condition #6 (full pre-K4 vanilla pipeline would have required IGameServices bus wiring out of scope)
  - V2 managed-with-structs (post-K6.1 main) — full 12-system production pipeline + factories, no presentation bridge
  - V3 native-with-batching (post-K6.1 main) — `NativeWorld` + 3 purpose-written V3 systems mirroring V2 read/write patterns through `SpanLease<T>` + `WriteBatch<T>`
- **Workload**: 50 pawns × full vanilla component set + 255 items (150 food + 50 water + 30 beds + 25 decorations) on a 200×200 grid with 800 obstacles. Fixed seeds (42 nav/pawns, 43 items). 10,000 ticks @ 30 TPS (333.3 simulated seconds target).
- **Frameworks**: BenchmarkDotNet 0.13.12 ShortRunJob (3w×5m, MemoryDiagnoser + ThreadingDiagnoser) for per-tick mean / allocations / gen0; custom `LongRunDriftRunner` for 10k-tick percentiles + GC count/duration + drift + process memory.
- **Key results** (V2 vs V3 — the apples-to-apples comparison):
  - BDN mean tick: V2 19.9 μs → V3 5.2 μs (3.81× faster)
  - BDN allocated/op: V2 6985 B → V3 360 B (19.4× less)
  - Long-run mean: V2 34.5 μs → V3 8.0 μs (4.33× faster)
  - Long-run p99: V2 480.1 μs → V3 15.0 μs (32.0× faster)
  - Long-run max: V2 88,000 μs (gen2 spike) → V3 43.7 μs (2,013× faster)
  - GC pause total over 10k ticks: V2 3.4 ms → V3 0.0 ms (V3 had zero collections of any generation)
  - Both variants comfortably meet 30 TPS / 60 FPS budget in absolute terms on Skarlet
- **Report**: `docs/reports/PERFORMANCE_REPORT_K7.md`
- **Raw data**: `docs/benchmarks/k7-bdn-tick.csv`, `docs/benchmarks/k7-bdn-tick-report.html`, `docs/benchmarks/k7-bdn-tick-report.md`, `docs/benchmarks/k7-long-run-V{1,2,3}.csv`
- **Recommended K8 outcome direction** (per report's executive summary): **Outcome 1 OR Outcome 2 depending on Crystalka's weighting of relative-improvement vs cutover-cost**. Outcome 1 (native + batching wins decisively) is favoured by relative-improvement axis (V3 4-32× margin across §8 metrics — exactly the "decisively" definition). Outcome 2 (managed-with-structs alone wins) is favoured by absolute-budget axis (V2 already meets target with 100×+ margin on Skarlet — K8 cutover cost may not be justified). Outcome 3 is excluded (V2 vs V3 not within 10%; gap is 4-32×). Crystalka makes the K8 decision call.
- **Lessons learned**:
  - K7 is the first measurement-only K-series milestone; brief format adapted from K-series implementation briefs (Phase 3 = "run benchmarks", Phase 4 = "analyze + report", no production source changes). Format worked — measurement runs were uneventful, the engineering challenge was V3 system harness design (purpose-written ≠ production-equivalent, an honest divergence the brief authorized).
  - Pre-K4 worktree V1 reconstruction was simpler than the brief feared: `git worktree add` + copy benchmark project tree + write a focused V1 scenario + drop V2/V3 from the worktree's TickLoopBenchmark + simplify Program.cs CLI. Total: ~30 minutes. The fragility the brief anticipated (substantive API drift) didn't materialize because pre-K4 already had K1/K2/K3 + K6 work landed; only the component shapes differed (class vs struct).
  - The brief's `2fc59d1^` reference for "pre-K4 baseline" was wrong: `2fc59d1` is the K4 closure DOC commit, and its parent is itself a K4 commit. The true pre-K4 baseline is `75a8ac7^` = `9227a577` (one commit before the first component-conversion). Drift recorded here so future closure-shaped briefs cross-check the closure SHA against the actual code-change SHAs, not the docs-update SHA.
  - V3's no-allocation property (zero gen0/1/2 over 10k ticks) is the K1 + K5 combined design landing cleanly. The 3.4 MB total allocation in V3 is per-tick `WriteBatch<T>` + `SpanLease<T>` managed wrappers; pooling them (deferred per K1/K5 closure notes) would drop V3 allocation closer to zero, but the K7 numbers already show no GC impact at the current allocation rate.
  - V3's 32× p99 advantage over V2 is the K8-relevant number. Mean-vs-mean comparison (4× faster) underestimates the value because mean is sensitive to the few-percent of GC-spike outliers V2 has and V3 doesn't. For fixed-step simulation (30 TPS), tail-latency dominates frame-budget violations; V3's tighter distribution is the qualitative win even before the mean speedup.
  - V2's 88 ms max single-tick (likely the first gen2 collection during measurement warmup) is the worst-case to budget against. Measured average across 10k ticks is 35 μs but a 88 ms spike is a missed-frame at any reasonable budget. Production V2 would ship with mitigations (server GC mode, gen2-tuning) that the benchmark didn't apply; full benchmark isolation reproduces the worst-case more aggressively than production runtime would, so the 88 ms is conservative.

### K8.0 — Architectural decision recording (Solution A: NativeWorld backbone)

- **Status**: DONE (`9f9dc05`..`5fa3f1d`, 2026-05-09)
- **Brief**: `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md` (FULL EXECUTED)
- **Brief type**: Architectural decision brief (fourth brief type — see brief §1.8)
- **Decision recorded**: Solution A — single NativeWorld backbone for production storage. ManagedWorld retained as test fixture and research reference only. K-L11 added to LOCKED foundational decisions; K-L3 and K-L8 implications extended; §K8 reconciled to K8.0-K8.5 sub-milestone series.
- **Context**:
  - K7 evidence (`docs/reports/PERFORMANCE_REPORT_K7.md`): V3 (NativeWorld) dominates V2 (managed-with-structs) by 4× mean tick / 32× p99 / 27× total allocation / 0 vs 13 GC collections across 10k ticks on Skarlet hardware
  - Crystalka commitment (chat session 2026-05-09): «игра это стресс тест, тут всё чистая инженирия и исследование, так что можно развивать максимально сложную архитектуру которая будет работать десятилетиями без костылей»
  - Solution B (storage abstraction layer) and Solution C (explicit hybrid) rejected — both are "structural costlines" relative to long-horizon cleanness
- **Migration roadmap**: K8.0 → K8.1 (primitives) → K9 (RawTileField; sequencing decision per brief §1.7) → K8.2 (component redesigns) → K8.3 (system migrations) → K8.4 (ManagedWorld retirement, Mod API v3) → K8.5 (mod ecosystem prep)
- **Test count**: 553 (unchanged — K8.0 is documentation-only)
- **Lessons learned**:
  - Architectural decision briefs (fourth brief type) are appropriate when a major directional choice needs LOCKED-spec recording. K8.0 establishes the pattern; future architectural inflection points may use the same shape.
  - Solution A's cost is bounded (4-8 weeks across K8.1-K8.5) but yields decade-scale cleanness. The «no compromises» commitment makes the trade calculation explicit: shorter-term pragmatism (Solutions B/C) creates structural costlines that propagate through every future system author and mod author. The decision is recorded, not deferred.
  - K9 sequencing decision (Option c — K9 between K8.1 and K8.2) was made in this brief. This unblocks G-series earlier without disrupting K8 series flow. Recorded in brief §1.7.

### K8.1 — Native-side reference handling primitives

- **Status**: DONE (`a62c1f3`..`812df98`, 2026-05-09)
- **Brief**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (FULL EXECUTED)
- **Goal**: Foundation primitives for K8.2 component redesigns. Four reference primitives now available on the native side: `StringPool` (generational mod-scoped interning), `KeyedMap` (sorted-by-key map), `Composite` (per-entity variable-length data), `SetPrimitive` (sorted-by-element set).
- **Deliverables**:
  - Native: `string_pool.h/cpp`, `keyed_map.h/cpp`, `composite.h/cpp`, `set_primitive.h/cpp`
  - World integration: per-mod scope orchestration delegated to `StringPool`; id-keyed maps of `unique_ptr` for the other three primitives, sized at first `get_or_create_*` call
  - C ABI: 28 new functions (`df_world_intern_string`, `df_keyed_map_*`, `df_composite_*`, `df_set_*`, plus mod scope and pool-introspection entries)
  - Managed bridge (top-level at `src/DualFrontier.Core.Interop/`, mirroring `NativeWorld`/`SpanLease`/`WriteBatch` placement): `InternedString`, `NativeMap<TKey,TValue>`, `NativeComposite<T>`, `NativeSet<T>`; `NativeWorld` accessors `InternString` / `ResolveInternedString` / `GetKeyedMap` / `GetComposite` / `GetSet` / `Begin`|`End`|`ClearModScope` / `StringPoolCount` / `StringPoolCurrentGeneration`
  - 17 native selftest sub-scenarios across 4 functions (`scenario_string_pool`, `scenario_keyed_map`, `scenario_composite`, `scenario_set_primitive`)
  - 30 managed bridge equivalence tests (7 + 10 + 7 + 6) in `tests/DualFrontier.Core.Interop.Tests/`
- **Test count**: 553 → 583 (+30, exact match to brief §1.12 projection)
- **Lessons learned**:
  - The brief's "atomic commit per managed wrapper" Phase 5 split assumed the wrappers were independent. They are not — `InternedString.Resolve` calls `NativeWorld.ResolveInternedString`, so the types only compile together. Bundled into one logical commit; the brief's discipline holds for native primitives (where the dependencies are forward-only) but breaks down at the bridge layer.
  - Brief Phase 0.4 expected `Marshalling/RawComponentStore.cs` and `Marshalling/{WriteBatch,SpanLease}.cs`. Actual project layout is **top-level** for primary handles and `Marshalling/` only for ID/registry helpers (`EntityIdPacking`, `ComponentTypeRegistry`, `NativeComponentType`). New K8.1 wrappers placed top-level to match — confirmed against existing convention rather than the brief's mistaken inventory. Future briefs in K-series should grep the actual file tree at authoring time, not memory of layout.
  - Generational mod-scope tracking handles the K-L9 «vanilla = mods» principle natively — strings owned by core stay alive across mod loads/unloads, but mod-specific strings reclaim deterministically when the owning mod's scope is cleared. Co-ownership across mods is recorded as set-membership in `ids_by_mod_`, so an id ceases to be reclaimable as soon as more than one scope uses it.
  - Sorted-by-key iteration for `KeyedMap` and `SetPrimitive` provides save/load determinism out of the box; insertion-order or hash-order alternatives would have created roundtrip drift across machines or across save/load cycles. Memcmp-byte ordering is sufficient for the blittable POD key types K8.2 will use.
  - CMakeLists.txt was updated **incrementally** (one entry per primitive commit) rather than as a single Phase 8 commit. Reason: explicit-listing CMakeLists rejects sources that aren't yet declared, and "build verification per primitive" is meaningless without the source being compiled. The brief's Phase 8 step became verification-only (no edit needed at closure).
  - The native selftest's `id_b_only` fixture initially raised a /W4 unused-variable warning; the right fix was an additional invariant assertion (verifying the ModB-exclusive id is distinct from both the shared id and the ModA-exclusive id), not a `(void)` cast. Dropped warnings should usually become tests.

### K8.1.1 — InternedString closure follow-up: cross-pool equality, doc semantics, empty-string coverage

- **Status**: DONE (`f8273ca`..`16afdf3` on `feat/k8-1-1-interned-string-followup`, fast-forward merged to main, 2026-05-09)
- **Brief**: `tools/briefs/K8_1_1_INTERNED_STRING_FOLLOWUP_BRIEF.md` (FULL EXECUTED) — authored on main as commit `fc4400d` before branch creation
- **Goal**: Close three observations surfaced by Opus closure verification of K8.1: (1) cross-pool equality had no explicit API surface despite K8.1 LOCKED #5 specifying dual semantics; (2) the `InternedString` doc-comment elided the multi-world failure mode of `==`; (3) the empty-string sentinel path had no test coverage on either side of the bridge.
- **Deliverables**:
  - `InternedString.EqualsByContent(InternedString, NativeWorld, NativeWorld)` — explicit cross-pool comparison method with two-world signature. Same-pool fast path preserved via reference-equality short-circuit when `(Id, Generation)` pairs are equal. Empty-on-both-sides returns true regardless of which worlds are supplied. Stale-generation resolution (or wrong-world resolution) returns false.
  - `InternedString` struct-level XML doc-comment expanded into five paragraphs: same-pool fast path, cross-pool failure mode of `==`, Solution A applicability (single-world production), stale ids (generation tag and wrong-world resolution), save/load contract restated with `EqualsByContent` as the cross-snapshot reconciliation path.
  - Empty-string sentinel coverage: native selftest sub-scenario 6 in `scenario_string_pool` (intern of empty content returns id 0; `df_world_string_pool_count` unchanged; resolve of id 0 returns 0 bytes regardless of generation tag passed in). Managed bridge test `EmptyString_RoundTrip_YieldsEmptySentinel` covers the same contract on the managed side, plus value-equality of empty values to `default(InternedString)`.
  - Eight `EqualsByContent_*` bridge tests covering: both-empty across worlds, one-side-empty in either order, same-pool fast-path agreement with `==`, same-pool different content, cross-pool identical content, cross-pool different content, null-world `ArgumentNullException` with the correct parameter name on each side, and stale-generation post-`ClearModScope`.
- **Test count**: 583 → 592 (+9 managed: 8 `EqualsByContent_*` scenarios + 1 empty-string round-trip; native selftest scenario count unchanged at 21 with 3 new sub-checks inside `scenario_string_pool`)
- **Atomic commit log** (8 commits total — 1 on main + 7 on feature branch):
  1. `fc4400d docs(briefs): author K8.1.1 InternedString follow-up brief` (on main, pre-branch)
  2. `f8273ca feat(interop): add InternedString.EqualsByContent for cross-pool comparison`
  3. `e365da3 docs(interop): expand InternedString doc-comment with multi-world semantics`
  4. `2c9f7ce test(native): cover string pool empty-string sentinel in selftest`
  5. `6c0d8c4 test(interop): cover InternedString empty-string sentinel round-trip`
  6. `16afdf3 test(interop): cover InternedString.EqualsByContent across same-pool and cross-pool scenarios`
  7. `docs(progress): record K8.1.1 closure (InternedString follow-up)` — this commit
  8. `docs(briefs): mark K8.1.1 brief EXECUTED` — pending
- **Brief deviations**:
  - Brief description text says "+9: 7 EqualsByContent scenarios + 2 empty-string round-trip" but the brief's own Phase 5 code listing defines 8 `EqualsByContent_*` methods + 1 empty-string round-trip = 9 total. Recorded honestly here as 8 + 1 = 9. The 592 total matches the brief's projection.
  - `EqualsByContent_StaleGeneration_ReturnsFalse` test setup as specified in the brief re-interned the same content **after** `EndModScope("ModA")` and **before** `ClearModScope("ModA")`. That re-intern adds the id to the empty/core scope's reference list (`current_mod_scope_ == ""` between mod scopes), which causes `clear_mod_scope` to see the id as «referenced elsewhere» and skip reclaim. The pre-clear and post-clear `(Id, Generation)` pairs then stay identical and the test fails its `Equals(...).Should().BeFalse()` assertion. Per Stop condition #3, the K8.1 reclaim semantics in `string_pool.cpp::clear_mod_scope` are correct as written; the test setup needed to keep the id uniquely owned by ModA. Fix: moved the fresh-lookup sanity assertion **inside** the `BeginModScope`/`EndModScope` pair, where it confirms the same-pool dedup invariant without polluting the empty-scope reference list. The brief's intended assertion (stale generation makes resolution null and `EqualsByContent` returns false) is preserved end-to-end.
- **Architectural decisions LOCKED in this milestone**:
  - Two-world signature for cross-pool comparison (no single-world overload). Misuse becomes structurally impossible at the API surface; same-pool callers pay zero extra cost via reference-equality short-circuit on equal `(Id, Generation)` pairs.
  - Empty values are equal to each other by content irrespective of which worlds they were issued by. Mixed empty/non-empty returns false. These two clauses are part of the `EqualsByContent` contract, not a fast-path optimisation, so they are documented in the method XML doc.
- **Cross-cutting impact**:
  - K8.2 component conversion may use `EqualsByContent` where cross-pool semantics applies. K8.1.1 only adds the surface; K8.2 onward is the consumer.
  - The expanded XML doc-comment is the user-facing source of truth for `InternedString` multi-world semantics; K8.2 component authors should treat it as authoritative when deciding between `==` and `EqualsByContent`.
- **Lessons learned**:
  - "Closure follow-up" briefs (precedent: K6.1 fault wiring follow-up) work well for tightening API surface and test coverage of one already-shipped deliverable without redesigning it. Scope discipline is enforced by the brief's "K8.1.1 does not" list (no native changes, no ABI additions, no production migration). The boundary held end-to-end.
  - Test setup for generational reclaim is sensitive to which scope holds the reference. The `string_pool.cpp` invariant «id stays alive as long as any scope refers to it» means even an "innocent" re-lookup between mod scopes anchors the id to the empty scope. Future mod-scope tests should isolate the reference to the scope under test or document the cross-scope intent explicitly.
  - The brief's "Halt and reconcile" Stop condition (#3) for reclaim semantics worked as designed: the failing test surfaced the assumption mismatch, the executor read `clear_mod_scope`, identified the cause (re-intern in empty scope), and adjusted the test rather than the implementation. The reclaim implementation is correct as shipped in K8.1.

### K-Lessons — Pipeline closure lessons formalization (post-K8.1.1, methodology batch)

**Status**: DONE
**Closure**: `9df2709..071ae11` on `feat/k-lessons-formalization` (fast-forward merged to main)
**Brief**: `tools/briefs/K_LESSONS_BATCH_BRIEF.md`
**Test count**: 592 (unchanged — documentation-only milestone)

**Deliverables**:

- `METHODOLOGY.md` v1.4 → v1.5: new sub-section "Pipeline closure lessons (K-series, post-K8.1)" under "Native layer methodology adjustments" with three lessons:
  - Atomic commit as compilable unit, not file-count unit (sourced from K8.1 Phase 5 bundled commit)
  - Phase 0.4 inventory as hypothesis, not authority (sourced from K8.1 `Marshalling/` reconciliation)
  - Mod-scope test isolation (sourced from K8.1.1 Stop condition #3 fix)
- `KERNEL_ARCHITECTURE.md` v1.2 → v1.3: new sub-section "Error semantics convention for Interop layer" in Part 7, codifying the four-category rule (sparse / dense / lifecycle / construction) practiced through K8.1 and K9 brief authoring.
- Change history entries in METHODOLOGY.md §10 and Status line update in KERNEL_ARCHITECTURE.md.

**Atomic commit log** (6 commits total — 1 on main + 5 on feature branch):
1. `3b242ba docs(briefs): author K-Lessons batch brief (4 pipeline lessons formalization)` (on main, pre-branch)
2. `9df2709 docs(methodology): formalize atomic-commit-as-compilable-unit lesson`
3. `b78441f docs(methodology): formalize Phase 0.4 inventory-as-hypothesis lesson`
4. `f1a4b34 docs(methodology): formalize mod-scope test isolation lesson` (includes METHODOLOGY.md v1.4 → v1.5 bump and §10 change history entry)
5. `071ae11 docs(kernel): formalize Interop error semantics convention; bump to v1.3` (includes KERNEL_ARCHITECTURE.md v1.2 → v1.3 bump)
6. `docs(progress): record K-Lessons closure (4 pipeline lessons formalized)` — this commit (also marks brief EXECUTED)

**Brief deviations**: zero structural deviations. Phase 1 design was implemented as written: 3 METHODOLOGY lessons + 1 KERNEL lesson, version bumps bundled into final lesson commits per Phase 1.5 atomic commit shape decision. Documentation-only milestone; no test count delta, no source code changes.

**Architectural decisions LOCKED in this milestone**: none new. The lessons formalize existing practice and existing convention; no foundational decision is added or modified. K-L1..K-L11 in `KERNEL_ARCHITECTURE.md` Part 0 unchanged.

**Cross-cutting impact**:

- K8.2 brief authoring (next milestone) cites the four lessons by name in its own Phase 1 design section.
- K9 brief drift findings (deferred Stage 4 work) cite the error semantics convention as the resolution of Drift #4.
- Future Cloud Code execution sessions on K-series milestones can reference the four lessons by section heading rather than by closure-narrative recall.

**Lessons learned**:

- Methodology-formalization batches (precedent: K-Lessons here, K8.0 architectural decision brief earlier) work cleanly as documentation-only milestones with their own atomic-commit log. Scope discipline is enforced by the brief's "K-Lessons does not" list (no source code, no tests, no native build, no new K-Lxx LOCKED decisions). Skipping the build/test gate is acceptable when the brief proves it touches no source paths; the closure verification (still running `dotnet test` and grepping for new debt markers) substitutes for it.
- Per-lesson commits (one `####` sub-section per commit) preserve readability of the document history without adding overhead, because each lesson is self-contained.
- Bundling the version bump into the final-lesson commit (rather than a separate "version bump" commit) avoids a tax commit that would not leave the document in a coherent reviewable state on its own. This generalizes the atomic-commit-as-compilable-unit principle to documentation: each commit should leave the doc reviewable.

### K8.2 v2 — K-L3 selective per-component closure (single milestone; framing reformulated by K-L3.1 amendment 2026-05-10)

**Status**: DONE
**Closure**: 3 main commits + `6ee1a85`..`7527d00` on `feat/k82-foundation-closure` (fast-forward merged to main).
  - Main commits (Phase 0): `c834f18` lock migration plan v1.0; `b1a461e` deprecate v1 brief; `88cbe3f` author v2 brief.
  - Branch commits (Phases 2.A + 2.C + 2.B + 3 + 4): 21 atomic commits — wrapper refactor, deletions, conversions, annotation pass, KERNEL doc bump.
**Brief**: `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V2.md` (FULL EXECUTED). v1 deprecated to `tools/briefs/K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md` after Phase 2.7 §1 stop on K8.1 wrapper surface mismatch.
**Test count**: 631 (up from 592 K-Lessons baseline; +39 net = 33 new wrapper/component tests + 14 InternedString IComparable + 9 NativeWorldFactory − 8 deleted-stub test scaffolding − 9 internal accounting).

**Deliverables** (single milestone, three concerns merged per Crystalka «Variant 3» 2026-05-09):

1. **K8.1 wrapper value-type refactor.** `NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` from `sealed unsafe class` to `readonly unsafe struct (uint id, IntPtr handle)`. `InternedString` gained `IComparable<InternedString>` + `Empty` static constant. `NativeWorld` gained `AllocateMapId/SetId/CompositeId` + `CreateMap/CreateSet/CreateComposite` factory methods (managed-side monotonic counters; native side unchanged — leverages existing `get_or_create_*` lazy pattern). `NativeMap` and `NativeSet` generic constraints relaxed from `IComparable<T>` to plain `unmanaged` so C# enums (e.g. `SkillKind`) work as keys.

2. **6 class→struct conversions** using post-refactor wrappers:
  - IdentityComponent — `string Name` → `InternedString Name`
  - WorkbenchComponent — `string? ActiveRecipeId` → `InternedString ActiveRecipeId` (Empty = idle sentinel)
  - FactionComponent — `string FactionId` → `InternedString FactionId`; const strings renamed to `*IdString` to clarify intern-at-use-site requirement
  - SkillsComponent — two `Dictionary<SkillKind, T>?` → `NativeMap<SkillKind, T>`
  - StorageComponent — `Dictionary<string, int>` → `NativeMap<InternedString, int>`; `HashSet<string>` → `NativeSet<InternedString>`
  - MovementComponent — `GridVector? Target` → `GridVector Target + bool HasTarget`; `List<GridVector> Path` → `NativeComposite<GridVector> Path + int PathStepIndex` (advance-counter walk to dodge `RemoveAt` swap-with-last semantics)

3. **6 empty TODO stub deletions** per METHODOLOGY §7.1 «data exists or it doesn't»:
  - Combat: AmmoComponent, ShieldComponent, WeaponComponent (+ orphan ShieldSystem)
  - Magic: SchoolComponent
  - Pawn: SocialComponent (+ orphan SocialSystem; replaces-protected fixture redirected to SkillSystem)
  - World: BiomeComponent (+ orphan BiomeSystem)
  - Reference cleanup across consumers: VanillaComponentRegistration, CombatSystem, DamageSystem, SpellSystem, EtherGrowthSystem, RitualSystem, RelationSystem, WeatherSystem, AmmoIntent, RefusalReason, DeathReactionEvent, M62IntegrationTests, Phase5BridgeAnnotationsTests, ProductionComponentCapabilityTests, VanillaComponentRoundTripTests.

**Architectural plumbing added** (Phase 2.B.1 surfaced as Phase 2.7 §3 stop, then resolved):
  - `DualFrontier.Core` → `DualFrontier.Core.Interop` ProjectReference (new dep).
  - `SystemExecutionContext` and `ParallelSystemScheduler` accept optional `NativeWorld?` parameter; `SystemBase` exposes `protected NativeWorld NativeWorld` accessor mirroring the existing `Services` pattern.
  - `GameBootstrap.CreateLoop` constructs one `NativeWorld` alongside the managed `World`. `RandomPawnFactory` ctor takes the native world (5th param). `PawnStateReporterSystem` resolves InternedString names via `SystemBase.NativeWorld`. `InventorySystem` and `HaulSystem` intern/resolve at the event-bridge boundary.
  - `DualFrontier.Modding.Tests` and `DualFrontier.Systems.Tests` csproj add native DLL `CopyToOutputDirectory` block (mirrors Core.Interop.Tests + Benchmarks).

**ModAccessible annotation pass** (Phase 3): 12 verify-only struct components annotated per `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` §1.5 defaults. Surviving 25 production components (31 pre-K8.2 − 6 stub deletions = 25) all carry `[ModAccessible]`.

**KERNEL_ARCHITECTURE.md** v1.3 → v1.4 (header bump deferred; landed at v1.5 in K-L3.1 amendment): Part 2 K8.2 row reformulated; Part 0 K-L3 implication rewritten (post-K-L3.1: K8.2 v2 selective per-component application; «без exception» framing superseded); status line updated.

**Brief deviations**:
  - Phase 0.7 lock of `MIGRATION_PLAN_KERNEL_TO_VANILLA.md` v1.0 (status `AUTHORED — pending Crystalka acceptance and lock` → `AUTHORITATIVE LOCKED`) committed on main as the milestone's first commit per v2 brief Phase 0.7.
  - v1 brief deprecated to `K8_2_COMPONENT_CONVERSION_BRIEF_V1_DEPRECATED.md` with frontmatter superseded note instead of `git mv` (v1 was untracked; standard rename + commit).
  - Native side `df_world_allocate_*_id` C ABI primitives NOT added; managed-side counter approach used instead per v2 brief Phase 1.A.5 «if existing has counters, reuse; else minimal addition» — chose the no-addition path because the native side accepts any non-zero uint via the existing `get_or_create_*` lazy primitives. Avoids C++ rebuild and binary deploy.
  - `NativeMap`/`NativeSet` generic constraint relaxation (drop `IComparable<TKey>`) — discovered at Phase 2.B.4 SkillsComponent compile because C# enums don't satisfy `IComparable<T>` automatically. Native side uses memcmp regardless of any managed comparer; constraint was documentary not load-bearing.
  - SystemBase NativeWorld accessor + GameBootstrap NativeWorld plumbing — surfaced as Phase 2.7 §3 stop at IdentityComponent conversion start (no production code path had a NativeWorld available); resolved per Crystalka «(A) add NativeWorld to SystemBase» direction.
  - NativeComposite EntityId-parameterized API preserved unchanged — for per-component-instance use the same parent entity passes every method call; redundant dimension is harmless. MovementComponent uses `PathStepIndex` advance-counter rather than `RemoveAt` to dodge the swap-with-last FIFO break.

**Architectural decisions LOCKED in this milestone**:
  - K8.1 wrapper class→struct refactor is now permanent (was Cloud Code's recommended K8.1.2 follow-up; Crystalka folded into K8.2 v2 to avoid a partial intermediate state).
  - Per-instance id allocation strategy: managed-side `NativeWorld` monotonic counter, 0 = invalid sentinel. Native side unchanged.
  - Empty TODO stub deletions per METHODOLOGY §7.1 are an architectural application of an existing principle, not a new K-Lxx decision.
  - K-L3 selective per-component application is the architectural fact for `src/DualFrontier.Components/` (25 surviving components: 6 conversions via K8.1 primitives + 19 verify-only annotations on already-struct + 6 deletions per METHODOLOGY §7.1). KERNEL_ARCHITECTURE.md Part 0 implication updated. Post-K-L3.1 (2026-05-10): «без exception» framing reformulated as «selective per-component»; bridge formalization adds Path β as first-class peer.

**Cross-cutting impact**:
  - K8.3 (system migration) unblocked: every consumer-bearing component is now `unmanaged struct` and uses K8.1 primitives where applicable. K8.3 brief authoring against this state.
  - K8.4 (managed `World` retirement, Mod API v3) — partial preview already in place: GameBootstrap owns one NativeWorld alongside managed World, and SystemBase exposes it. K8.4 finalizes the retirement.
  - M-series migrations (M8.4 Vanilla.World, M8.5-M8.7 Vanilla.Pawn, M9 Vanilla.Combat, M10/M10.B Vanilla.Inventory/Magic) consume K-L3-clean components. The 6 deleted-stub slice contents are M-series content concerns — vanilla mods author them as `unmanaged struct` from inception.

**Lessons learned**:
  - When a brief's design assumes consumer-site infrastructure that doesn't yet exist (here: `NativeWorld` access from systems and factories), surface as Phase 2.7 §3 stop early, ask, and treat the resolution as architectural plumbing rather than per-component improvisation. Saves replicating the same plumbing across 6 conversions.
  - Wrapper generic constraints can be over-constrained for documentary reasons. When a constraint blocks idiomatic usage of valid managed types (enums) without buying anything semantically, drop it. The native side is the authority on ordering when it uses memcmp.
  - Brief Phase 6.1 split-point contingency (partial closure mid-K8.2) ended up not used: single session executed the full milestone in 21 commits over the branch + 3 main commits, with all green. The split-point design remains a useful contingency for future similar milestones.

### K-L3.1 — Bridge formalization (architectural decision session)

- **Status**: DONE (2026-05-10)
- **Brief**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` + `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (FULL EXECUTED — Phase 0 reads + Phase 1 deliberation Q1–Q6 + Phase 3 synthesis + Phase 4 amendment plan)
- **Brief type**: Architectural decision brief (fourth brief type, K8.0 precedent)
- **Trigger**: post-K8.2 v2 cleanup session 2026-05-10 — Crystalka clarification «там был частичный перенос то что можно легко преобразовывать в struct было преобразовано, а что нет то managed, так как не все имеет смысл тащить в натив для скорости» revealed K8.2 v2 closure framing «K-L3 «без exception»» as misframing of selective per-component judgment actually applied; bridge formalization escalated per METHODOLOGY §3 «stop, escalate, lock»
- **Session length**: deliberation 2026-05-10 (single session, Crystalka + Opus, no Cloud Code execution)
- **Test count**: 631 (unchanged — deliberation session, no source edits; Phase 4 deliverable is documentation plan)

**Locks** (Phase 1 deliberation closures):

- **Q1 = (a)**: `[ManagedStorage]` attribute on type. Class `: IComponent` types annotated with this attribute are Path β; absent attribute + struct shape implies Path α. Analyzer enforcement of shape↔attribute consistency deferred to post-migration analyzer milestone (Q5.b/M3.5-extension).
- **Q2 = (β-i)**: mod-side managed-storage ownership; `IModApi` v3 extension `RegisterManagedComponent<T> where T : class, IComponent`. Storage lives in per-mod `RestrictedModApi` instance; reclaimed deterministically on `AssemblyLoadContext.Unload`.
- **Q3 = (i)**: explicit dual API. `SystemBase.NativeWorld.AcquireSpan<T>()` for Path α (existing K8.2 v2 plumbing); `SystemBase.ManagedStore<T>()` for Path β (new K8.4 plumbing). Cross-mod managed-path direct access structurally impossible by ALC isolation; cross-mod data flow via events per §6 three-level contracts.
- **Q4 = (b)**: managed-path forbidden for persisted components. Save system out of scope per migration plan §0.4 + §8.1; managed-path = runtime-only state; codec layer untouched.
- **Q5 = (a)**: passive performance metrics; `KernelCapabilityRegistry` tagged per-path; PERFORMANCE_REPORT splits native/managed. Active Roslyn analyzer enforcement (Q5.b) deferred per Crystalka 2026-05-10 «после миграции нужен будет анализатор... но это потом».
- **Q6 = (a)**: capability surface path-blind. `[ModAccessible]` already targets `Class | Struct` (K4 prerequisite); capability strings (`kernel.read:`, `mod.<id>.read:`) carry verb + FQN, path-orthogonal — confirms existing structural reality.
- **Synthesis = §4.A**: amend K-L3 wording (single principle, peer paths) rather than add new K-L12. KERNEL `v1.3 → v1.5`; MOD_OS `v1.6 → v1.7`; MIGRATION_PLAN `v1.0 → v1.1`.

**Architectural decisions LOCKED in this milestone**:

- Path α (`unmanaged struct` / native) and Path β (managed `class` via `[ManagedStorage]` / mod-side) are first-class peers, not principle/exception
- K-L3 implication post-K-L3.1: components are either path; default is α (silent + struct shape); β requires `[ManagedStorage]` opt-in
- Mod-side managed-storage ownership preserves K-L11 «NativeWorld single source of truth» for native data; managed-storage decentralization-by-mod is consistent with K-L9 «vanilla = mods» + ALC isolation
- Path β components are runtime-only at K-L3.1; persistence opt-in (if/when needed) is a future amendment milestone
- Capability model uniform across paths (Q6.a confirmed structural truth)
- K8.2 v2 closure framing «K-L3 «без exception» state achieved» reformulated as «K-L3 selective per-component application» — closure outcome was selective judgment per METHODOLOGY §7.1 (delete) + per K8.1 primitive coverage (convert) + verify-only annotations (already struct), not universal mandate

**Output artifacts**:

1. `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (Status: AUTHORED → EXECUTED 2026-05-10)
2. `tools/briefs/K_L3_1_BRIEF_ADDENDUM_1.md` (skeleton brief disposition extension — APPLIED)
3. `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (NEW — Phase 4 deliverable; old/new text pairs for KERNEL + MOD_OS + MIGRATION_PLAN + MIGRATION_PROGRESS + skeleton brief dispositions)
4. This MIGRATION_PROGRESS entry (added by amendment brief atomic with line corrections)

**Cross-cutting impact**:

- **Amendment brief = Phase A'.1**: docs-only execution per `docs/architecture/K_L3_1_AMENDMENT_PLAN.md`; KERNEL `v1.3 → v1.5`, MOD_OS `v1.6 → v1.7`, MIGRATION_PLAN `v1.0 → v1.1` (includes §0.1 Phase A' integration) + this MIGRATION_PROGRESS sync. Estimated 30–60 min auto-mode. Test count delta zero (docs-only). Atomic commit shape: per-document.
- **Phase A' sequencing**: companion document at `docs/architecture/PHASE_A_PRIME_SEQUENCING.md` (authored 2026-05-10) anchors structural unit between K8.2 v2 closure (DONE) and M8.4 begin (Phase B). Phase A' contains 10 phases: A'.0 K-L3.1 (DONE), A'.1 amendment brief, A'.2 README cleanup, A'.3 push, A'.4 K9, A'.5 K8.3, A'.6 K8.4, A'.7 K8.5, A'.8 K-closure report (NEW), A'.9 Architectural analyzer (NEW). Cumulative duration ~10–16 weeks hobby pace; analyzer's dual purpose = M-series migration verifier + architectural debugger per Crystalka 2026-05-10 «анализатор будет верификатором миграции и будет нашим дебагером на баги которые не ловят тесты».
- **K9 brief = Phase A'.4** (full authored, ~1200 lines, AUTHORED awaiting execution per β6 sequencing — NOT a skeleton): Disposition B (surgical) — version refs (KERNEL v1.4+ instead of v1.0) + test baseline (631 instead of 538) update; scope unchanged (fields architecturally orthogonal to entity-component bridge per addendum §A2.5)
- **K8.3 skeleton brief = Phase A'.5** (~36 lines true skeleton): Disposition B-to-C — scope undercount correction (12 named systems → 34 actual per migration plan §1.2 reformulated scope) + dual-path access pattern wording per Q3.i; full brief authoring against post-K-L3.1 amended state
- **K8.4 skeleton brief = Phase A'.6** (~33 lines true skeleton): Disposition C.1 in-place rewrite — title «ManagedWorld retired» framing preserved (kernel-side managed `World` still retires per K-L11; managed-storage moves mod-side per K-L3.1, not stays kernel-side); add `RegisterManagedComponent<T>` to v3 surface deliverables; add `SystemBase.ManagedStore<T>()` plumbing
- **K8.5 skeleton brief = Phase A'.7** (~30 lines true skeleton): Disposition B (surgical) — content list expands to include bridge mechanism documentation (per-component path choice criterion, `[ManagedStorage]` usage, `ManagedStore<T>` access pattern, dual-API examples)
- **K-closure report = Phase A'.8** (NEW per Phase A' sequencing): structured document enumerating final K-Lxx invariants; dual purpose = (1) historical record of K-series, (2) formal analyzer rule specification surface; format = each invariant has formal statement + violation example + compliance example. Out of K-L3.1 amendment plan scope; tracked as future Phase A' milestone.
- **Architectural analyzer = Phase A'.9** (NEW per Phase A' sequencing): Roslyn analyzer encoding K-Lxx invariants per K-closure report; dual purpose = M-series migration verifier + architectural debugger; track B activation candidate per ROADMAP «Maximum Engineering Refactor»; M3.4 capability analyzer merge decision at analyzer brief authoring time. Out of K-L3.1 amendment plan scope; tracked as future Phase A' milestone.

**Lessons learned**:

- Architectural decision briefs (fourth brief type, K8.0 precedent) extended to K-L3.1 with «closure clarification triggers retroactive principle reformulation» case. Phase 0 reads → Phase 1 deliberation Q1–Q6 → Phase 3 synthesis → Phase 4 amendment plan format works for this case (K8.2 v2 closure framing was misread by closure report; K-L3.1 reformulates without invalidating closure outcome).
- B.2 finding (live `IModApi.RegisterComponent<T> where T : IComponent` is path-agnostic at code level despite K-L3 doc-level «universal mandate» framing) was the structural anchor enabling Reading γ. Bridge was empirically already compatible with existing surface; K-L3.1 formalized convention into LOCKED architectural decision rather than creating new mechanism.
- B.4 finding (K8.2 v2 closure outcome empirically embodies Reading γ — 6 §7.1 deletions are «not converted to struct, removed because data didn't exist», not «universally constrained») confirmed §4.A synthesis correctness. The closure outcome was selective per-component judgment, not universal mandate; §4.A captures this in K-L3 amendment without inventing new principle.
- Crystalka clarification («что можно легко преобразовывать в struct было преобразовано, а что нет — managed») as session trigger validated «stop, escalate, lock» rule (METHODOLOGY §3): closure framing misalignment was surfaced by user observation, not auto-detected by tooling; deliberation session is the structurally correct response (vs. inline closure-report patch which would have been a kostyl).
- Q6 (capability path-orthogonality) was already structurally true per K4 prerequisite (`[ModAccessible]` widened to `Class | Struct`). This is empirical evidence that some «open» architectural questions in deliberation briefs may already be answered by accumulated code state — Phase 0 inventory lessons (K-Lessons «inventory as hypothesis, not authority») extended: hypothesis can resolve question without deliberation when disk truth is decisive.
- Crystalka direction «без костылей, у меня нет давления временем» as session frame enabled rigorous Q1–Q6 walkthrough with each lock + rationale, rather than batched/forced answers under time pressure. Long-horizon framing aligns with brief §1.3 «architectural cleanness... десятилетиями».

**Amendment landing (A'.1.K, 2026-05-10)**:

8 atomic commits landed amendments per `docs/architecture/K_L3_1_AMENDMENT_PLAN.md` §1–§5:
- Commit K-A1: KERNEL_ARCHITECTURE.md v1.3 → v1.5 (Part 0 K-L3 row + K-L8 implication paragraph + Part 2 K8.2 row + Status line + Part 4 Decisions log Path α vs Path β + closing v1.0 → v1.5 sediment + 4 surgical scrubs on «без exception» active spec wording)
- Commit K-A2: MOD_OS_ARCHITECTURE.md v1.6 → v1.7 (lines 1149-1150 full rewrite — «must be unmanaged struct» → Path α + Path β + dual SystemBase API + ALC isolation barrier; §3.5 D-1 path orthogonality clarification; §4.6 IModApi v3 RegisterManagedComponent<T>; §11.1 M3.5 Path α/β consistency analyzer scope extension; §11.2 MissingManagedStorageAttribute ValidationErrorKind entry)
- Commit K-A3: MIGRATION_PLAN_KERNEL_TO_VANILLA.md v1.0 → v1.1 (§0.1 Phase A' integration full sequence diagram replacement; §0.3 Decision #9 K-L3.1 bridge formalization LOCKED; line 62/78 «K-L3 violation» reframing; line 148 «no exception post-K8.2» reframing; §1.2 K8.3 dual-path access constraint; §1.3 K8.4 RegisterManagedComponent ships; §1.5 Phase A closure gate Path α/β registration paths; §6.6 NEW K-L3.1 amendment execution maintenance entry)
- Commit K-A4: MIGRATION_PROGRESS.md sync (line 5 Last updated A'.1.K landing; line 34 Active phase A'.3 push next; line 35 Last completed milestone cascade A'.1.K → A'.1.M → A'.0.7 → A'.0.5 → K-L3.1; line 87 Overview table K8.2 reframe; line 407 K8.2 v2 header reframe; line 443 KERNEL doc bump entry reframe; line 457 architectural fact reframe; new K-L3.1 closure entry per §4.7)
- Commits K-A5 through K-A8: 4 skeleton brief surgical edits (K9 Disposition B — Phase 0.4 version refs KERNEL v1.5+ + MOD_OS v1.7+, Phase 0.7 baseline 631+; K8.3 Disposition B-to-C — scope 12 → 34 systems + dual-path access TODO; K8.4 Disposition C.1 in-place rewrite — Mod API v3 dual registration paths + SystemBase.ManagedStore<T>() accessor + per-mod ManagedStore<T> implementation + MissingManagedStorageAttribute TODO; K8.5 Disposition B — bridge documentation + dual-API access patterns + v2→v3 migration guide)
- Commit K-A9: K-L3.1 brief Status amendment landing reference
- Commit K-A10: this closure entry extension

Test count delta: zero across all 10 commits (docs-only).
Working tree clean post-A'.1.K; baseline 631 preserved by construction.

**HG-4 pre-flight (recorded 2026-05-10)**: dotnet test executed with testhost cleared (per user directive — kill leftover testhost.exe before pre-flight HG-4 prevents environmental incident #3 per incident #2 root cause). Baseline 631 verified at start of Part K execution.

**Surgical scrubs applied during Part K (per §1.3 amendment plan pattern; out-of-plan)**:
- KERNEL line 19 «Component constraint: unmanaged structs only (class-based prohibited)» → «Component storage: Path α default; Path β per opt-in»
- KERNEL line 79 «struct components (Path α post-K7)» → «vanilla components: Path α or Path β»
- KERNEL line 752 «K-L3 «без exception» achieved» → «K-L3 selective per-component closure achieved (post-K-L3.1 reframing)»
- KERNEL line 789 «K-L3 «без exception» closure» → «K-L3 selective per-component closure (post-K-L3.1 reframing)»
- MIGRATION_PLAN line 148 «Conversion target: unmanaged struct (Path α, K-L3 LOCKED, no exception post-K8.2)» → «default Path α + per-component Path β opt-in per K-L3.1»

§K.12.2 cross-document drift audit: all remaining «без exception» / «K-L3 violation» / «must be unmanaged struct» / «Class-based component storage prohibited» / «no exception» hits are in version-history quote-context or explicit reframing context (acceptable per §K.12.2 directive «only in version-history quote-context»).

K-L3.1 + A'.0.7 amendments both LANDED. Phase A' deliberation foundation (A'.0 К-L3.1 / A'.0.5 reorg / A'.0.7 methodology rewrite) + amendment landing (A'.1.M A'.0.7 amendment / A'.1.K K-L3.1 amendment) DONE. Awaits A'.3 push к origin → A'.4-A'.7 K-series execution → A'.8 K-closure report → A'.9 architectural analyzer.

### K8.3-K8.5 — Sub-milestones

- **K8.3**: `tools/briefs/K8_3_PRODUCTION_SYSTEM_MIGRATION_BRIEF.md` (skeleton; reformulated scope per migration plan §1.2 — all 34 production systems in `src/DualFrontier.Systems/`)
- **K8.4**: `tools/briefs/K8_4_MANAGED_WORLD_RETIRED_BRIEF.md` (skeleton; partial preview already in place via K8.2 v2 SystemBase NativeWorld plumbing)
- **K8.5**: `tools/briefs/K8_5_MOD_ECOSYSTEM_MIGRATION_PREP_BRIEF.md` (skeleton)

---

## Phase A' progress

Phase A' is the structural unit between Phase A (K-series) closure and Phase B (M-series) start, formalized 2026-05-10 per `/docs/architecture/PHASE_A_PRIME_SEQUENCING.md`. It contains all Phase A → B transition deliverables: K-L3.1 architectural decision, documentation reorganization, methodology pipeline restructure, amendment plan execution, push to origin, K9/K8.3-K8.5 skeleton execution, K-closure report, and architectural analyzer milestone.

### Overview

| Milestone | Title | Status | Estimate | Commits | Date closed |
|---|---|---|---|---|---|
| A'.0 | K-L3.1 bridge formalization | DONE | 2-4 hours session | `45d831c` | 2026-05-10 |
| A'.0.5 | Documentation reorganization + cross-ref refresh + module-local refresh + pipeline-terminology scrub + cleanup campaign | DONE | 2-4 hours auto-mode | `27523ac`..`<HEAD>` | 2026-05-10 |
| A'.0.7 | Methodology pipeline restructure rewrite | DONE | ~3 hours deliberation + landing | session 2026-05-10 | 2026-05-10 |
| A'.1 | Amendment brief execution (K-L3.1 propagation + Tier 2 K-L11 framing + Vanilla mod READMEs) | NOT STARTED | 30-60 min auto-mode | — | — |
| A'.2 | REMOVED — folded into A'.0.5 Phase 5 | — | — | — | — |
| A'.3 | Push to origin | NOT STARTED | minutes | — | — |
| A'.4 | K9 + A'.4.0 patch execution (RawTileField + IModApi v3 Fields wiring) | DONE | 8-12 hours auto-mode | `d163341..<HEAD>` on `feat/k9-field-storage` (17 commits) | 2026-05-10 |
| A'.5 | K8.3 skeleton execution (production system migration) | NOT STARTED | 4-6 weeks | — | — |
| A'.6 | K8.4 skeleton execution (managed World retired) | NOT STARTED | 1-2 weeks | — | — |
| A'.7 | K8.5 skeleton execution (mod ecosystem migration prep) | NOT STARTED | 3-5 days | — | — |
| A'.8 | K-closure report | NOT STARTED | 1-2 sessions | — | — |
| A'.9 | Architectural analyzer milestone | NOT STARTED | 2-4 weeks | — | — |

### A'.0 — K-L3.1 bridge formalization

- **Status**: DONE (`45d831c`, 2026-05-10)
- **Brief**: `tools/briefs/K_L3_1_BRIDGE_FORMALIZATION_BRIEF.md` (EXECUTED) + `K_L3_1_BRIEF_ADDENDUM_1.md` (APPLIED)
- **Output**: amendment plan at `/docs/architecture/K_L3_1_AMENDMENT_PLAN.md` (Phase 4 deliverable; awaits A'.1 execution to propagate locks into 4 LOCKED docs)
- **Architectural decisions**: K-L3.1 lock — Path α (`unmanaged struct` via `RegisterComponent<T>`, kernel-side `NativeWorld`) and Path β (managed `class` annotated with `[ManagedStorage]` via `RegisterManagedComponent<T>`, mod-side `ManagedStore<T>`) as first-class peers; cross-mod managed-path direct access structurally impossible by ALC isolation; capability annotation path-blind (Q6.a)
- **Code changes**: zero (deliberation session; implementation lands in K8.4 per plan)
- **Test count**: 631 unchanged

### A'.0.5 — Documentation reorganization + cross-ref refresh + module-local refresh + pipeline-terminology scrub + cleanup campaign

- **Status**: DONE (closure 2026-05-10)
- **Brief**: `tools/briefs/A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md` (EXECUTED)
- **Closure commits**: `27523ac` (Phase 0 brief authoring) through Phase 9 closure (~25 atomic commits) on main
- **Test count**: 631 unchanged (documentation-only milestone)
- **Phases completed**:
  - Phase 0 pre-flight: HEAD `45d831c` ✓, build ✓, working-tree state mismatch (8 pre-staged moves) noted and resolved per Stop #1 Q0=A
  - Phase 1 discovery: ~135 .md files inventoried, classified A/B/C/D/E/F/G/H, staleness scan + pipeline-terminology scan complete; outputs at `tools/scratch/A_05/`
  - Phase 2 reorganization plan authored, Crystalka approved 2026-05-10 with Q0=A, Q1-Q10=yes per 11-question Stop #1 protocol
  - Phase 3 file moves (5 commits): 8 pre-staged Cat A + 3 new Cat A + 14 subsystem-arch + 6 methodology + 5 reports → 36 files relocated
  - Phase 4 cross-reference refresh (3 commits): ~250 stale path references updated to repo-rooted absolute form (architecture group: 97 files; methodology group: 54 files; reports group: 25 files)
  - Phase 5 README stub-reference cleanup: 5 component READMEs (top-level, Combat, World, Magic, Pawn) — single atomic commit
  - Phase 6 module-local doc refresh (3 commits): kernel-area (Core/Math, Core.Interop) + Systems (Combat/World/Pawn/Magic/Faction/top) + Events (Combat/Pawn) — mechanical-only refreshes per brief §8.1 Stop #2 boundary
  - Phase 7 pipeline-terminology scrub (2 commits): 7 active narrative docs mechanically scrubbed; A'.0.7 deferral markers added on 3 methodology files (METHODOLOGY/PIPELINE_METRICS/MAXIMUM_ENGINEERING_REFACTOR)
  - Phase 8 discovery cleanup: Tier 1 typo fix («Opus aud the result» → «Opus audits the result»); Tier 2 items flagged at `tools/scratch/A_05/TIER2_FLAGS.md`
  - Phase 9 closure (this entry): Phase A' sequencing doc updated; this MIGRATION_PROGRESS entry filled; brief marked EXECUTED in next commit

- **Tier 2 surfaced debt (forwarded for future deliberation)**:
  - **K-L11 managed-World framing in kernel-area module-local docs** (Core/README.md, Core/ECS/README.md) — substantive narrative refresh required; forwarded to A'.1
  - **Vanilla mod placeholder READMEs** (mods/DualFrontier.Mod.Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}/README.md) — Phase 6 deferred; forwarded to A'.1 / M8+
  - **IModApi v3 surface docs** (Contracts/Modding/README.md) — forward-looking; verify post-K8.4 closure

- **Items forwarded to A'.0.7**:
  - Substantive rewrite of `docs/methodology/METHODOLOGY.md` §0/§2.1/§2.2/§3/§4/§5/§8 (4-agent pipeline framing → 2-agent unified Claude Desktop session)
  - Substantive rewrite of `docs/methodology/PIPELINE_METRICS.md` (entire empirical record gathered with Gemma-era pipeline; reframe-or-recollect decision)
  - Substantive rewrite of `docs/methodology/MAXIMUM_ENGINEERING_REFACTOR.md` 4-agent layout / hardware-variance / pipeline-mapping sub-sections
  - README.md (root) Pipeline section rewrite (substantive 4-agent decomposition; HTML deferral marker preserves verbatim until A'.0.7 lands)

- **Items forwarded to A'.1**:
  - K-L3.1 lock propagation into 4 LOCKED docs (KERNEL/MOD_OS/MIGRATION_PLAN/MIGRATION_PROGRESS) per `K_L3_1_AMENDMENT_PLAN.md`
  - K-L11 framing propagation in kernel-area module-local docs (per Tier 2 flag above)
  - Vanilla mod placeholder README refresh (per Tier 2 flag above)

- **Brief deviations**:
  - Working tree was NOT clean at Phase 0 (8 pre-staged moves + brief untracked) — accepted Option A (commit pre-staged moves with rename detection) per Stop #1 Q0=A; rename detection produced 100%-similarity renames as expected
  - Phase 4 commit count came in at 3 (matched brief estimate); Phase 6 commit count came in at 3 (lower than brief's 6-12 estimate because mechanical-only scope kept changes narrow); Phase 7 commit count 2 (matched estimate)

- **Lessons learned**:
  - **Single-session pipeline collapses milestone splits**: A'.2 (originally separate README cleanup milestone) folded naturally into A'.0.5 Phase 5 because there's no longer a multi-agent handoff cost to multiple milestones. Future Phase A' / M-series brief authoring should consider milestone consolidation when the work is documentation-shaped and falls within single-session capacity.
  - **Stop #2 mechanical-vs-architectural boundary holds**: Phase 6 module-local refresh strictly limited to deleted-type removals + status-line corrections + factual updates — no narrative-meaning changes. Items requiring architectural deliberation (K-L11 framing, K-L3.1 propagation, Vanilla mod READMEs) explicitly flagged forward rather than improvised. Discipline preserved at the cost of slightly thin post-Phase-5 Combat/README that Phase 6 didn't refill (it would have required architectural narrative beyond scope).
  - **Cross-ref refresh script reusability**: PowerShell script at `tools/scratch/A_05/update_xrefs.ps1` parameterized by category — reusable for future organizational reshuffles. Pattern: «replace markdown link form within destDir-aware scope; replace inline path form repo-root-uniformly». Worth formalizing as `tools/` permanent helper if future moves are anticipated.
  - **Pre-staged work + Stop #1 protocol scales**: Crystalka pre-staged 8 file moves before invoking the session. Treating those as Phase 3 work-in-progress (via Q0=A in Stop #1) preserved the user's pre-decision rather than re-deciding it. The 11-question Stop #1 format with explicit Q/recommendation/decision columns made the deliberation efficient (~3-minute response cycle).

### A'.0.7 — Methodology pipeline restructure rewrite

- **Status**: DONE (deliberation session 2026-05-10)
- **Brief**: `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` (EXECUTED via this closure)
- **Closure brief**: `tools/briefs/A_PRIME_0_7_CLOSURE_EXECUTION_BRIEF.md` (this milestone's execution-mode brief)
- **Phase 4 deliverable**: `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (1460 lines; awaits A'.1 amendment brief execution к propagate locks into 4 methodology docs)
- **Brief type**: Architectural decision brief (fourth brief type, К8.0 / К-L3.1 precedent)
- **Trigger**: A'.0.5 closure 2026-05-10 (`4e332bb`) placed HTML deferral markers on 4 methodology docs (METHODOLOGY / PIPELINE_METRICS / MAXIMUM_ENGINEERING_REFACTOR / README) flagging substantive sections для A'.0.7 architectural-deliberation rewrite. Crystalka direction 2026-05-10 («Всё делается через десктопное приложение Claude» + pipeline restructure clarifications) + «Без костылей, у меня много времени» discipline declaration set frame для deliberation session.
- **Session length**: ~3 hours (single Crystalka + Claude Desktop session, deliberation mode; no code execution)
- **Test count**: 631 unchanged (deliberation session, no source edits)

**Locks** (Phase 1 deliberation closures):

- **Q-A07-1 = (α)**: pure deliberation brief shape (analog К-L3.1).
- **Q-A07-2 = (β)**: mixed disposition per section (per-section judgment in cascade).
- **Q-A07-3 = (β+γ)**: PIPELINE_METRICS preserve historical с per-metric reassessment notes.
- **Q-A07-4 = (γ)**: falsifiable claim generalized к architect/executor abstract framing.
- **Q-A07-5 = (a)**: A'.0.5 lesson #1 («single-session pipeline collapses milestone splits») formalized as methodology K-Lessons entry.
- **Q-A07-6** (surfaced during Q1 deliberation): **audience contract** — methodology corpus authored under agent-as-primary-reader assumption. Human reader pathway preserved at README level only; high cross-reference density, FQN-style references, §-level addressability, terse compression declared as design features, not coincidental complexity. Session-level invariant frame для Q2–Q12.
- **Q-A07-7 = (b)** (surfaced during Q9 deliberation): defer K-L3.1-derived methodology lesson («closure clarification triggers retroactive principle reformulation») к A'.8 К-closure report scope. A'.0.7 stays within locked Q-A07-1..5 + Q-A07-6.
- **Q-A07-8 = (c)** (surfaced during Q10 deliberation): inline §6 forward measurement plan в PIPELINE_METRICS itself; no separate backlog document.
- **Q1 = (b)**: §0 Abstract — abstract framing primary + current-configuration footnote.
- **Q2 = (α)**: §2.1 Role distribution — structural rewrite, abstract role categories (direction owner / architect / executor) + §2.1.1 Current configuration table (v1.6 N=2 Claude Desktop session-mode).
- **Q3 = (c-reformulated)**: §2.2 Contracts as IPC — top-layer principle «contracts as IPC across context boundaries» + sub-layer three-properties mechanism (falsifiability + self-contained scope + repository as coordination surface).
- **Q4 = (b-reformulated)**: §3 Economics — §3.1 economic invariant (architectural deliberation context-intensive low-frequency vs mechanical execution scope-bounded high-frequency, independent of boundary type) + §3.2 current configuration economics с A'.0.5 empirical anchor (commit range `27523ac..4e332bb`); v1.5 §3.3 comparison-with-alternatives discarded.
- **Q5 = (c-decomposed)**: §4 Empirical results — per-sub-section disposition (§4.1 refresh / §4.2 mechanical scrub / §4.3 untouched / §4.4 parallel-form rewrite Case A Phase 4 v1.x + Case B A'.0.5 v1.6 / §4.5 untouched).
- **Q6 = (α-b)**: §5 Threat model substantial rewrite (§5.1 OpenClaw case study untouched; §5.2 4-agent enumeration → v1.6 session-mode enumeration; §5.3 falsifiable claims reformulated к 5 attack classes including new «architect-executor crosstalk impossible») + §6 Boundaries verify clean.
- **Q7 = (a-table)**: per-section judgment с explicit table for §1 / §2.3 / §2.4 / §7 / §8 / §9 / §10 / §11 dispositions.
- **Q8 = (c-formulation)**: new «Phase A' lessons (post-A'.0.5)» sub-section с full formulation of «Milestone consolidation under session-mode pipeline» lesson (era classification framing, A'.0.5 empirical anchor, brief authoring checklist, falsifiable claim, caveats, era inversion observation).
- **Q9 = (b)**: existing Native layer methodology adjustments sub-sections verify clean; no expansion.
- **Q10 = (a-with-standardized-labels)**: PIPELINE_METRICS — 5 standardized transferability labels (`[v1.x era specific]` / `[transfers с reframing]` / `[transfers as-is]` / `[uncertain — needs v1.6 measurement]` / `[v1.x historical record]`) applied к 17 sub-sections + top-of-document era frame note + version history sub-section + v0.1 → v0.2 version bump.
- **Q11 = (table)**: MAXIMUM_ENGINEERING_REFACTOR — per-sub-section dispositions (bulk untouched; 3 substantial rewrites: §4.3 prompts/ sub-tree, §5.2 parallel-track discipline mapping, §10 v1.1 ratification row); Tracks A/B/C architectural content untouched.
- **Q12 = (c-formulation)**: README Pipeline section — hybrid (abstract + current configuration + historical) + audience contract declaration («agent-as-primary-reader assumption») + falsifiability claim generalized.
- **Synthesis form = §4.A-primary с document-specific §4.C для PIPELINE_METRICS**: methodology corpus (METHODOLOGY + MAXIMUM_ENGINEERING_REFACTOR + README) describes invariants pipeline-agnostically с current-configuration anchor + historical mention. Empirical record (PIPELINE_METRICS) preserves per-era data verbatim с era classification annotations. Two-track principle explicit: pipeline-agnostic principles + per-era empirical data are different epistemic categories.

**Architectural decisions LOCKED в this milestone**:

- Methodology corpus is **abstract primary** documents (METHODOLOGY + MAXIMUM_ENGINEERING_REFACTOR + README). Falsifiable claims pipeline-agnostic per Q-A07-4=γ. Survives any future pipeline pivot.
- Empirical record is **versioned per-era** document (PIPELINE_METRICS). v1.x era data preserved verbatim с transferability annotations; v1.6 era data collection forward-looking.
- Audience contract: **agent-as-primary-reader** для methodology corpus deeper than README. Cross-reference density, FQN-style references, §-level addressability, terse compression declared as design features. README serves as gateway human entry point.
- Pipeline reality: 2-agent unified Claude Desktop session с deliberation/execution modes; boundary type session-mode. v1.x era 4-agent model-tier boundary preserved as historical reference.
- New К-Lessons entry: «Phase A' lessons (post-A'.0.5)» sub-section с «Milestone consolidation under session-mode pipeline» lesson — extends K-Lessons #1 «atomic commit as compilable unit» к milestone scope; same structural pattern (boundaries match natural seams) at different scope levels.

**Output artifacts**:

1. `tools/briefs/A_PRIME_0_7_METHODOLOGY_RESTRUCTURE_BRIEF.md` (commit `55d9e36`; Status: AUTHORED → EXECUTED 2026-05-10)
2. `tools/briefs/A_PRIME_0_7_CLOSURE_EXECUTION_BRIEF.md` (this milestone's closure execution brief; new tracked artifact)
3. `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` (NEW — Phase 3 deliverable; 1460 lines old/new text pairs for 4 methodology corpus docs)
4. This MIGRATION_PROGRESS entry (added by closure execution commit)

**Cross-cutting impact**:

- **Amendment brief = Phase A'.1 follow-up** (after К-L3.1 amendment lands): docs-only execution per `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md`; touches 4 methodology corpus docs (METHODOLOGY v1.5 → v1.6, PIPELINE_METRICS v0.1 → v0.2, MAXIMUM_ENGINEERING_REFACTOR v1.0 → v1.1, README Pipeline section). 5 atomic commits per amendment plan §7.1. Estimated 30-60 min auto-mode. Test count delta zero. Possible fold с A'.1 К-L3.1 amendment per brief §6.2 + amendment plan §7.2.
- **No К-L3.1 amendment scope overlap**: К-L3.1 amendment touches architecture corpus (KERNEL / MOD_OS / MIGRATION_PLAN / MIGRATION_PROGRESS / 4 skeleton briefs); A'.0.7 amendment touches methodology corpus (METHODOLOGY / PIPELINE_METRICS / MAXIMUM_ENGINEERING_REFACTOR / README). Zero file overlap, zero section overlap. Per amendment plan §5.
- **Phase A' progress**: A'.0 К-L3.1 DONE + A'.0.5 reorg DONE + A'.0.7 methodology rewrite DONE (this) → A'.1 amendment brief execution → A'.3 push → A'.4-A'.7 К-series execution → A'.8 К-closure report → A'.9 architectural analyzer milestone → Phase B M8.4.

**Lessons learned**:

- **Audience contract surfaces as architectural decision**: Crystalka «документы для агентов, не для людей» mid-Q1 elevated к Q-A07-6 session-level invariant. Surfacing as separate lock rather than improvising Q1.b формулировку was correct per «escalate, не improvise» discipline. Audience contract declaration appears explicitly в three documents (METHODOLOGY §0 footnote, PIPELINE_METRICS frame note, README Pipeline section) для cross-document consistency.
- **Two-track synthesis is cleaner than monolithic synthesis**: locked dispositions Q1-Q12 pushed methodology corpus к §4.A (abstract primary), но Q10 locked preservation pushed PIPELINE_METRICS к §4.C (versioned-empirical-record). Recognizing these are different epistemic categories (pipeline-agnostic principles vs per-era empirical data) и giving them different document shapes resolves what brief §4 phrased as «one coherent v1.6 framing» without forcing inconsistency.
- **Era inversion observation in K-Lessons #2 (Phase A' lessons)**: v1.x era principle was «split is default safe» (model-tier handoff bounded); v1.6 era principle is «bundle is default safe» (session-mode handoff = brief authoring duplication). Same underlying discipline («boundaries match natural seams»), different default behavior. Future pipeline pivots may invert again. Lesson formulated с explicit era-comparison structure.
- **Q-A07 cascade auxiliary locks surfaced during deliberation**: Q-A07-6 (audience contract during Q1), Q-A07-7 (К-L3.1 lesson scope during Q9), Q-A07-8 (PIPELINE_METRICS backlog tracking during Q10). All three surface-and-lock followed «escalate, не improvise» discipline; each got explicit deliberation surface, recommendation rationale, и lock. Cascade pattern (auxiliary Q-A07-X locks emerging during main Q1-Q12 surface) may recur in future architectural decision sessions; documenting Q-A07-X with «session lock» marker (vs pre-session lock) preserves traceability.
- **Pipeline empirical validation**: this deliberation session executed under v1.6 session-mode boundary (Claude Desktop deliberation mode) delivered substantial architectural work (15 Q locks + synthesis + 1460-line amendment plan) в single ~3-hour session. Becomes v1.6 era data point alongside A'.0.5 (execution session). Both feed PIPELINE_METRICS §6 forward measurement plan.

**Amendment landing (A'.1.M, 2026-05-10)**:

5 atomic commits landed amendments per `docs/architecture/A_PRIME_0_7_AMENDMENT_PLAN.md` §1–§4 + closure:
- Commit M-1: METHODOLOGY.md v1.5 → v1.6 (§0 Abstract / §2.1 Role distribution + §2.1.1 Current configuration / §2.2 Contracts as IPC / §2.3 Verification cycle / §3 Economics / §4.1 State at publication / §4.2 mechanical scrub / §4.4 parallel-form Case A+B / §5.2-§5.3 Threat model / §7.1 surgical scrub / §9 «degradation as codebase grows» / §10 v1.6 row / new «Phase A' lessons (post-A'.0.5)» K-Lessons sub-section / §11 path verification / tagline surgical scrub / deferral marker removed)
- Commit M-2: PIPELINE_METRICS.md v0.1 → v0.2 (top-of-document era frame note + per-metric annotations с 5 standardized transferability labels at 17 sub-sections + new §6 Forward measurement plan + Version history sub-section + tagline surgical scrub + deferral marker removed)
- Commit M-3: MAXIMUM_ENGINEERING_REFACTOR.md v1.0 → v1.1 (§4.3 prompts/ rewrite + §5.2 parallel-track mapping rewrite + §4.8 «Hardware variance» surgical scrub + §10 v1.1 row + header version line bump + deferral marker removed)
- Commit M-4: README.md Pipeline section rewrite (Q12.c-formulation hybrid + agent-as-primary-reader audience contract declaration + deferral marker removed)
- Commit M-5: this closure entry + A'.0.7 brief Status amendment landing reference

Test count delta: zero across all 5 commits (docs-only).
Working tree clean post-A'.1.M; baseline 631 preserved by construction.

**HG-4 pre-flight divergence (recorded per user directive 2026-05-10)**: Pre-flight HG-4 (dotnet test 631 baseline verification) skipped due к dotnet test environmental stall — 18 worker processes idle for 10+ min с empty output streams; same hang pattern as A'.0.7 closure session 2026-05-10 §5.1 (which was recovered via kill). Permission denial on kill для >5min-old processes blocked recovery в this session. Construction argument applied: 3 intervening commits since prior 631 verification (c74a342, c2b83b4, 86b721a) are docs-only; baseline preserved by construction. Empirical retry of `dotnet test --logger "console;verbosity=minimal"` после commit M-1: 5 of 6 test projects ran cleanly (275/275 passing, 0 failures); 6th project (DualFrontier.Modding.Tests, 356 tests) blocked at build due к file lock on test fixture DLLs held by leftover testhost (16692) from prior hung run. Root cause identified: leftover testhost from earlier hangs holds file locks on `tests/Fixture.*/bin/Debug/net8.0/*.dll`. This is the **2nd confirmed environmental incident** в Phase A' execution sessions (A'.0.7 closure §5.1 was the 1st; both involved hung dotnet processes requiring kill для recovery). Environmental investigation deferred к A'.8 К-closure report candidate list (out of A'.1 scope).

**Amendment plan gap surgical scrubs applied (out-of-amendment-plan, per §1.3 «surgical scrub at execution time» pattern)**:
- METHODOLOGY tagline (line 3): «four-agent LLM pipeline, contracts as IPC between agents» → «architect-executor split with contracts as IPC across context boundaries». Amendment plan §1.1 didn't enumerate tagline; v1.6 framing required alignment.
- METHODOLOGY §7.1 closing paragraph: «all four agents in the pipeline... prompt generator... local executor... architect... human» → «all participants... architect... executor... QA review... direction owner». Q7.a-table «verify clean» reported hit; surgical reformulation preserves substantive principle while generalizing vocabulary per Q-A07-4=γ.
- METHODOLOGY §11 cross-ref paths: PHASE_1, SESSION_PHASE_4_CLOSURE_REVIEW, ROADMAP, root README updated к post-A'.0.5 repo-rooted absolute paths (amendment plan §1.16 anticipated need).
- METHODOLOGY §4.4 Case A: SESSION_PHASE_4_CLOSURE_REVIEW path corrected к `/docs/audit/` (amendment plan §1.9.4 specified pre-reorg sibling path).
- PIPELINE_METRICS tagline (line 30): «for the four-agent LLM pipeline used in this project» → «for the LLM pipeline used in this project, gathered per-era with transferability annotations». Same gap pattern as METHODOLOGY tagline.
- MAXIMUM_ENGINEERING_REFACTOR header version line: v1.0 → v1.1. Amendment plan §3 declared target version v1.0 → v1.1 but §3.1-§3.6 didn't enumerate header line edit; consistency с §10 v1.1 row required header update.
- MAXIMUM_ENGINEERING_REFACTOR §4.8 «Hardware variance» Risk bullet: «Local Gemma performance varies with GPU» → «Environment variance» с v1.x era qualifier + v1.6 era equivalent risk note. §3.5 «surgical scrub at execution time» pattern.

These amendment plan gaps may surface к A'.8 К-closure report as «brief authoring inaccuracy» pattern; not A'.1 re-deliberation scope.

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

**K9 sequencing post-K8.0**: K9 prerequisite changes from "K6 complete" to "K8.1 closure" per K8.0 brief §1.7 sequencing decision (Option c). Rationale: K9 reuses K8.1 native-side reference primitives where applicable; K9 doesn't depend on K8.2/3/4. K9 runs between K8.1 and K8.2 for natural pause in K-series progression.

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

### D5 — Solution A (single NativeWorld backbone) chosen for K8 series

- **Date**: 2026-05-09
- **Decision**: Solution A — NativeWorld single source of truth for production storage. ManagedWorld retained as test fixture only.
- **Alternatives rejected**:
  - Solution B (storage abstraction `IComponentStore`): permanent runtime polymorphism layer; defers a decision the project is committed to making; "structural костыль".
  - Solution C (explicit hybrid: struct components on Native, class components on Managed): bifurcated storage; permanent mental overhead for every mod author; cross-storage queries become friction.
- **Rationale**: K7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + Crystalka commitment («архитектура на десятилетия без костылей») + K-L11 codification.
- **Reversal trigger**: only if K8.1 native-side reference primitives prove fundamentally infeasible (e.g., cannot match managed Dictionary semantics within reasonable performance budget). Reversal would re-open Solution C as fallback, with explicit re-architecture milestone.
- **Implementation roadmap**: K8.0 (this milestone) → K8.1 → K9 → K8.2 → K8.3 → K8.4 → K8.5.

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
