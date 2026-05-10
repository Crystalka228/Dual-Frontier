# DualFrontier Kernel — Architecture & Roadmap

**Version**: 1.3
**Date**: 2026-05-09
**Status**: AUTHORITATIVE LOCKED — operational reference document, Solution A architectural commitment recorded (K-L11 added in v1.2, K-L3/K-L8 implications extended); Interop error semantics convention formalized in Part 7 (v1.3)
**Companion documents**: `METHODOLOGY.md`, `CODING_STANDARDS.md`, `MOD_OS_ARCHITECTURE.md`, `RUNTIME_ARCHITECTURE.md`
**Scope**: Full architectural specification + milestone roadmap для native ECS kernel (C++ via pure P/Invoke). Companion к `RUNTIME_ARCHITECTURE.md` (Vulkan rendering layer) — together describing complete native foundation under managed Application layer.

---

## Executive summary

DualFrontier ECS kernel migrates от managed C# к pure C++ via P/Invoke. Domain layer abstractions (Components, Events, Systems) preserved verbatim в managed; only storage и primitive operations move к native. All systems remain managed (because all systems are mods loaded via AssemblyLoadContext — vanilla mods и third-party mods uniformly).

**Foundation philosophy** — «без компромиссов»:
- Pure P/Invoke к `DualFrontier.Core.Native.dll` (no third-party C# binding library, mirrors RUNTIME_ARCHITECTURE.md L2)
- BCL only for managed bridge (`System.Runtime.InteropServices`, `System.Numerics`)
- Manual memory management в C++ (std::vector + std::unordered_map only, no third-party libs)
- Component constraint: `unmanaged` structs only (Path α — class-based components prohibited)
- Two-phase model: native bootstrap → managed game tick

**Source of truth для existing experimental work**: `claude/cpp-core-experiment-cEsyH` branch + `docs/CPP_KERNEL_BRANCH_REPORT.md` (Discovery report). 11 substantive C++ commits + 1637 LOC delta + clean self-test passing.

**Estimated scope**: 5-8 weeks at hobby pace (~1h/day) для full kernel completion (K0-K8) + **1-2 weeks** для К9 (field storage abstraction).

**Status snapshot** (live, обновляется по closure milestone): K0–K5 closed (`547c919`, 2026-05-08); 538 tests passing; K6 next per β6 sequencing. См. `MIGRATION_PROGRESS.md` для current state.

**Combined с RUNTIME_ARCHITECTURE.md (M9.0-M9.8) + GPU_COMPUTE.md (К9 + G0–G9)**: **15-25 weeks** для full architectural foundation. См. `ROADMAP.md` "Native foundation tracks" section для master sequencing.

---

## Part 0: LOCKED foundational decisions

The following decisions are committed как architectural foundation. Departures require explicit re-architecture milestone, not spec-level adjustments mid-implementation.

| # | Decision | Choice | Rationale |
|---|---|---|---|
| K-L1 | Native language | C++20, MSVC/GCC/Clang | Compiled native, modern features, no third-party deps |
| K-L2 | Bindings | Pure P/Invoke к `DualFrontier.Core.Native.dll` | Zero third-party C# в production binary (mirrors L2) |
| K-L3 | Component constraint | Unmanaged structs only (Path α) | Storage requires blittable layout; class-based prohibited |
| K-L4 | Type IDs | Explicit registry per-mod registration | FNV-1a hash collision-prone; explicit IDs deterministic |
| K-L5 | Bootstrap orchestration | Declarative graph, native, parallel where deps allow | Symmetric к runtime second graph; explicit dependencies |
| K-L6 | Game tick scheduler | Managed (because all systems are mods) | «Vanilla = mods» principle; AssemblyLoadContext mandates managed code path для systems |
| K-L7 | Span protocol | Read-only spans + write command batching | Mutation semantics explicit; race conditions structurally impossible |
| K-L8 | Component lifetime | Native owns storage, managed holds opaque `IntPtr` | Single ownership boundary; managed holds handle only |
| K-L9 | Mod parity | Vanilla mods register components и systems through same IModApi as third-party | «Vanilla = mods» enforced at architecture level |
| K-L10 | Decision rule | §8 metrics (GC pause / p99 / long-run drift on weak hardware) | §6 «20% mean speed» superseded; §8 captures actual project value |
| K-L11 | Production storage backbone | NativeWorld single source of truth (Solution A); ManagedWorld retained as test fixture and research artifact only | K7 evidence (V3 dominates V2 by 4-32× across §8 metrics) + «no compromises» commitment; single ownership boundary, single mental model |

**Implication of K-L3**: All managed components must be unmanaged structs. **K8.2 v2 (`MIGRATION_PROGRESS.md` K8.2 closure entry, 2026-05-09) achieved K-L3 «без exception» state** for `src/DualFrontier.Components/`. Three deliverables in one milestone: (1) K8.1 wrapper value-type refactor — `NativeMap<K,V>`, `NativeSet<T>`, `NativeComposite<T>` from `sealed unsafe class` to `readonly unsafe struct` so component structs can carry these as fields without violating the `unmanaged` constraint; `InternedString` gained `IComparable<InternedString>`; `NativeWorld` gained `Allocate*Id` counters and `Create*` factory methods. (2) 6 class→struct conversions using K8.1 primitives — Identity/Workbench/Faction (InternedString), Skills (NativeMap×2), Storage (NativeMap+NativeSet), Movement (NativeComposite + bool HasTarget + PathStepIndex). (3) 6 empty TODO stub deletions per METHODOLOGY §7.1 «data exists or it doesn't» — Combat (Ammo/Shield/Weapon), Magic (School), Pawn (Social), World (Biome). Real game-mechanics components for these slices are authored fresh as `unmanaged struct` in the M-series vanilla mod content milestones (M9 Combat, M10.B Magic, M-series Pawn social, M-series World biome). Mod components subject to same constraint. **K4's "Hybrid Path" softening retired** — after K8.2 v2 closure, K-L3 holds without exception across vanilla and mod components alike.

**Implication of K-L6**: There is NO «native scheduler» для game tick. Native scheduler exists only для bootstrap orchestration. All system code (vanilla mods + third-party mods) executes managed.

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
  DualFrontier.Components/                    # struct components (Path α post-K7)
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
  DualFrontier.Runtime/                       # see RUNTIME_ARCHITECTURE.md
  DualFrontier.Presentation/                  # see RUNTIME_ARCHITECTURE.md

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
  RUNTIME_ARCHITECTURE.md                     # Vulkan layer companion
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
│  - Window/Render Thread (RUNTIME_ARCHITECTURE.md)            │
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

Mirrors RUNTIME_ARCHITECTURE.md §1.9 для cross-document consistency.

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
| K8.2 | K-L3 «без exception» closure: K8.1 wrapper value-type refactor (NativeMap/NativeSet/NativeComposite to readonly struct + IComparable<InternedString> + per-instance id allocation) + 6 class→struct conversions (Identity/Workbench/Faction/Skills/Storage/Movement) + 6 empty TODO stub deletions (Ammo/Shield/Weapon/School/Social/Biome — content deferred to M-series) + 12 ModAccessible annotation completeness pass | 6-12 hours auto-mode (3-5 days hobby) | +800/-1500 |
| K8.3 | 12 vanilla systems migrated to SpanLease/WriteBatch | 2-3 weeks | -400/+600 |
| K8.4 | ManagedWorld retired; Mod API v3 ships | 1 week | -2000/+200 |
| K8.5 | Mod ecosystem migration prep | 3-5 days | +500 (docs) |
| K9 | Field storage abstraction (`RawTileField<T>`) | 1-2 weeks | +600-900 |

**Cumulative K0-K8**: 5-8 weeks at hobby pace.
**Cumulative K0-K9**: 6-10 weeks at hobby pace (K9 prerequisite for G-series GPU compute).

**Combined с RUNTIME_ARCHITECTURE.md M9.0-M9.8 + GPU_COMPUTE.md G0-G9**: 16-25 weeks total для full architectural foundation. K-series gates K9; K9 gates G-series. See [GPU_COMPUTE](./GPU_COMPUTE.md) Roadmap for G0-G9 detail и combined timeline.

### K0 — Cherry-pick + cleanup от branch

**Goal**: experimental branch contents preserved on current main as fresh feature branch. Hygiene fixes applied.

**Source**: `claude/cpp-core-experiment-cEsyH` per `docs/CPP_KERNEL_BRANCH_REPORT.md` Section 11.6.

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
- Report file `docs/PERFORMANCE_REPORT_K7.md` documenting findings

**Time**: 3-5 days. **LOC**: +200-400 (mostly benchmark code).

### K8 — Production storage cutover (Solution A: NativeWorld backbone)

**Decision** (recorded by K8.0 closure, 2026-05-09; see `docs/MIGRATION_PROGRESS.md` K8.0 closure section): Solution A — single NativeWorld backbone (per K-L11). Choice rationale in Part 4 Decisions log.

**Sub-milestone series**:
- **K8.0** — Architectural decision recording (this milestone; LOCKED v1.2)
- **K8.1** — Native-side reference handling primitives (string interning, keyed maps, composite components)
- **K8.2** — Per-component redesign + K8.1 wrapper value-type refactor + empty TODO stub deletions; K-L3 «без exception» achieved (v2 brief, single milestone)
- **K8.3** — Production system migration (12 vanilla systems → SpanLease/WriteBatch)
- **K8.4** — ManagedWorld retired as production; Mod API v3 ships
- **K8.5** — Mod ecosystem migration prep (documentation + migration guide)

**Cumulative time**: 4-8 weeks at hobby pace.

**LOC delta**: substantial — K8.1 adds ~600-1000 LOC (native + bridge); K8.2 modifies 7 component files plus their consumers; K8.3 modifies 12 system files plus tests; K8.4 deletes managed World production path; K8.5 adds documentation.

### K9 — Field storage abstraction

**Goal**: native `RawTileField<T>` storage as a parallel abstraction alongside `RawComponentStore`. Prerequisite for the G-series GPU compute roadmap ([GPU_COMPUTE](./GPU_COMPUTE.md) v2.0). Ships CPU functional path first; no Vulkan compute dependency.

**Authoritative spec**: [GPU_COMPUTE.md](./GPU_COMPUTE.md) "Architectural integration → Native kernel (KERNEL_ARCHITECTURE.md K9)" + "Roadmap → K9 — Field storage abstraction".

**Deliverables**:
- `RawTileField<T>` C++ class (data + back buffer + conductivity map + storage flags)
- C ABI: `df_world_register_field`, `df_world_field_read_cell`, `df_world_field_acquire_span`, `df_world_field_set_conductivity`, `df_world_field_set_storage_flag`
- Managed bridge: `FieldRegistry`, `FieldHandle<T>` в `DualFrontier.Core.Interop`
- CPU-side reference implementation of basic diffusion (also serves as G1+ shader equivalence oracle and as CPU fallback per [GPU_COMPUTE](./GPU_COMPUTE.md) "Failure modes → CPU fallback")
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
- K8.2 — K-L3 «без exception» closure: K8.1 wrapper value-type refactor + 6 class→struct conversions using K8.1 primitives + 6 empty TODO stub deletions per METHODOLOGY §7.1
- K8.3 — 12 vanilla systems migrated to `SpanLease<T>` reads + `WriteBatch<T>` writes
- K8.4 — managed `World` retired as production path; Mod API v3 ships
- K8.5 — mod ecosystem migration prep (documentation + migration guide)

**Operating principle**: «honest state always available» — managed World stayed working through K0-K7 so the K7 evidence base could be collected; the K-L11 commitment then settles the decision permanently. Reversal trigger documented in `docs/MIGRATION_PROGRESS.md` D5 (Solution A rationale and reversal trigger).

Mirrors RUNTIME_ARCHITECTURE.md migration approach (parallel Godot + Vulkan until M9.5 cutover).

---

## Part 4: Decisions log

### Resolved (LOCKED — see Part 0)

K-L1 through K-L10 above.

### Resolved (additional context)

**§6 vs §8 decision rule reconciliation**:
- §6 (English NATIVE_CORE.md): «≥20% combined Get+Add → continue» — naive baseline, optimizes wrong metric
- §8 (Russian NATIVE_CORE_EXPERIMENT.md): «GC pause / p99 / drift on weak hardware» — captures actual project value
- **§8 authoritative**, §6 superseded и retained для historical context only

**Path α vs Path β для components**:
- Path α: convert components к structs (this document, K-L3)
- Path β: GCHandle marshalling for class components (rejected — defeats GC pressure reduction goal)
- **Path α chosen** — aligns с modern ECS conventions, eliminates GC pressure structurally

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
| Cross-platform support | If/when needed (parallels RUNTIME_ARCHITECTURE.md L7) |
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
- Mirrors RUNTIME_ARCHITECTURE.md §1.9 «State exists или driver crashes»

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
- M-series ADs от RUNTIME_ARCHITECTURE.md continue
- K-series ADs new sequence
- Direct Opus → Claude Code routing pattern continues

---

## Part 8: Relationship к RUNTIME_ARCHITECTURE.md

KERNEL_ARCHITECTURE.md (this) и RUNTIME_ARCHITECTURE.md describe **two halves of single architectural vision**: native foundation under managed Application layer.

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

**Kernel knows nothing of rendering**: `DualFrontier.Core.Native` doesn't include `vulkan-1.dll`, doesn't depend on RUNTIME_ARCHITECTURE.md decisions. Could be open-sourced separately as «sparse-set ECS kernel».

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

**«Features only on demand»** (continuing principle от RUNTIME_ARCHITECTURE.md): kernel API surface stays minimal. ~20 C ABI functions sufficient для full DF gameplay. Resist temptation к build «complete» ECS engine — every function must trace к specific Domain requirement.

This document is **v1.0**, authoritative until amended via explicit decision. Amendments require commit с rationale (similar к how MOD_OS_ARCHITECTURE.md evolved).

Next document update expected при K8 closure (decision step results recorded), then per K-milestone (decisions log + risk register updates).

**Document end. Companion: METHODOLOGY.md, CODING_STANDARDS.md, MOD_OS_ARCHITECTURE.md, RUNTIME_ARCHITECTURE.md, [GPU_COMPUTE.md](./GPU_COMPUTE.md) (v2.0 LOCKED — K9 field storage + G-series Vulkan compute).**
