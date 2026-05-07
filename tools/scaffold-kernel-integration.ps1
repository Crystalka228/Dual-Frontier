# DualFrontier KERNEL_ARCHITECTURE.md integration scaffolding
# Generated: 2026-05-07
# References: docs/KERNEL_ARCHITECTURE.md, docs/RUNTIME_ARCHITECTURE.md
# Run from: D:\Colony_Simulator\Colony_Simulator\
#
# Generates:
#   1. docs/KERNEL_ARCHITECTURE.md (full v1.0 spec)
#   2. Additive cross-references in RUNTIME / METHODOLOGY / MOD_OS / README
#   3. Kernel migration folder skeleton (src/, native/, tests/, tools/briefs/)
#   4. MODULE.md per kernel folder
#   5. K0-K8 brief skeletons in tools/briefs/
#
# Idempotent: safe to re-run. Folder creation uses -Force; cross-reference
# insertions are guarded by content checks; doc files written unconditionally.
# NO IMPLEMENTATION CODE generated — scaffolding and documentation only.

$ErrorActionPreference = 'Stop'

# === Section 1: Verify working directory =====================================
if (-not (Test-Path "DualFrontier.sln")) {
    Write-Error "Must run from D:\Colony_Simulator\Colony_Simulator\ (DualFrontier.sln not found)"
    exit 1
}

Write-Host "DualFrontier KERNEL_ARCHITECTURE.md integration starting..." -ForegroundColor Cyan

# Helper: write file with UTF-8 encoding (CRLF preserved by PowerShell default on Windows)
function Write-File {
    param([string]$Path, [string]$Content)
    $dir = Split-Path -Parent $Path
    if ($dir -and -not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
    Set-Content -Path $Path -Value $Content -Encoding UTF8
}

# Helper: append section to existing file if marker not present
function Append-IfMissing {
    param([string]$Path, [string]$Marker, [string]$Content)
    if (-not (Test-Path $Path)) {
        Write-Host "  SKIP $Path (file does not exist)" -ForegroundColor DarkGray
        return
    }
    $existing = Get-Content -Path $Path -Raw
    if ($existing -match [regex]::Escape($Marker)) {
        Write-Host "  SKIP $Path (marker already present)" -ForegroundColor DarkGray
        return
    }
    $existing = $existing.TrimEnd("`r","`n")
    $combined = $existing + "`r`n`r`n" + $Content + "`r`n"
    Set-Content -Path $Path -Value $combined -Encoding UTF8
    Write-Host "  APPEND $Path" -ForegroundColor Green
}

# Helper: replace exact line if found and replacement marker not present
function Replace-Line {
    param([string]$Path, [string]$Pattern, [string]$Replacement, [string]$AlreadyDoneMarker)
    if (-not (Test-Path $Path)) { return }
    $existing = Get-Content -Path $Path -Raw
    if ($existing -match [regex]::Escape($AlreadyDoneMarker)) {
        Write-Host "  SKIP line replace in $Path (already updated)" -ForegroundColor DarkGray
        return
    }
    if ($existing -notmatch $Pattern) {
        Write-Host "  SKIP line replace in $Path (pattern not found)" -ForegroundColor DarkGray
        return
    }
    $updated = [regex]::Replace($existing, $Pattern, $Replacement)
    Set-Content -Path $Path -Value $updated -Encoding UTF8
    Write-Host "  REPLACE line in $Path" -ForegroundColor Green
}

# === Section 2: Save KERNEL_ARCHITECTURE.md ==================================
Write-Host "[2] Writing docs/KERNEL_ARCHITECTURE.md..." -ForegroundColor Cyan

$kernelArch = @'
# DualFrontier Kernel — Architecture & Roadmap

**Version**: 1.0
**Date**: 2026-05-07
**Status**: AUTHORITATIVE LOCKED — operational reference document
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

**Estimated scope**: 5-8 weeks at hobby pace (~1h/day) для full kernel completion (K0-K8).

**Combined с RUNTIME_ARCHITECTURE.md (M9.0-M9.8)**: **9-15 weeks** для both pivots fully shipped.

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

**Implication of K-L3**: All managed components must convert от class к struct (Phase 7 effort, 50-80 components). Mod components subject к same constraint.

**Implication of K-L6**: There is NO «native scheduler» для game tick. Native scheduler exists only для bootstrap orchestration. All system code (vanilla mods + third-party mods) executes managed.

**Implication of K-L9**: No vanilla privilege. Vanilla.{Core,Combat,Magic,Inventory,Pawn,World} mods register components и systems via same API as third-party would.

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
| K8 | Decision step + production cutover | 1 week | +/- (refactor) |

**Cumulative**: 5-8 weeks at hobby pace.

**Combined с RUNTIME_ARCHITECTURE.md M9.0-M9.8**: 9-15 weeks total для both pivots. See §8 для combined timeline visualization.

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

**Deliverables**:
- `SystemGraph.Rebuild(modRegistry)` method
- `ModLoader.UnloadMod(modId)` с ALC unload + GC.Collect
- `ModLoader.ReloadMod(modId)` reloads assembly + re-registers components/systems
- `PhaseCoordinator.OnModChanged()` event handler (pause tick, rebuild graph, resume)
- Tests: rebuild correctness, unload + reload cycle, graph topological invariants

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

### K8 — Decision step + production cutover

**Goal**: apply §8 rule к K7 numbers, decide path forward, execute decision.

**Three outcomes**:

**Outcome 1: Native + batching wins decisively**
- Replace managed `World` с `NativeWorld` в production tick loop
- Migrate Application bootstrap к use `Bootstrap.Bootstrap()` two-phase model
- Deprecate managed `World`, retain as test reference только
- Update modding API к v3 (sub-phase support, unified registration)

**Outcome 2: Managed-with-structs alone wins**
- Native kernel becomes optional optimization
- Convert components к structs (already done в K7)
- Keep native as research artifact, не production
- Document «native available но не required»

**Outcome 3: All paths equivalent**
- Native kernel solved problem (proven viable, not urgent)
- Park kernel work, continue с managed
- Document lessons learned

**Time**: 1 week (depending on outcome — production cutover most work).

---

## Part 3: Migration strategy

**Approach: parallel development.**

Keep managed `World` functional throughout K0-K7. K8 decision determines whether:
- Replace managed `World` с `NativeWorld` (Outcome 1)
- Keep both, native optional (Outcome 2)
- Park native (Outcome 3)

**Operating principle**: «honest state always available» — managed World stays working, native World matures alongside, decision made с evidence not speculation.

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
- Probability: Low (allocation reduction structural)
- Impact: K7 measurement shows native не faster than managed-with-structs
- Mitigation: К8 Outcome 2 already planned for this case (managed-with-structs valid path)

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

**Recommendation** (per Crystalka philosophy): **β5 или β6 over β3** — single architectural focus per period preserves cleanness. Decision deferred к after K2 measurement (evidence-based choice).

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

**Document end. Companion: METHODOLOGY.md, CODING_STANDARDS.md, MOD_OS_ARCHITECTURE.md, RUNTIME_ARCHITECTURE.md.**
'@
Write-File -Path "docs/KERNEL_ARCHITECTURE.md" -Content $kernelArch

# === Section 3: Cross-reference insertions ===================================
Write-Host "[3] Inserting cross-references..." -ForegroundColor Cyan

# 3.1 RUNTIME_ARCHITECTURE.md — Companion documents line + Master plan + Part 9
$runtimeCompanionPattern = '(?m)^\*\*Companion documents\*\*:[^\r\n]*$'
$runtimeCompanionReplacement = '**Companion documents**: `METHODOLOGY.md`, `CODING_STANDARDS.md`, `MOD_OS_ARCHITECTURE.md`, **`KERNEL_ARCHITECTURE.md`** (kernel layer companion)'
Replace-Line -Path "docs/RUNTIME_ARCHITECTURE.md" `
    -Pattern $runtimeCompanionPattern `
    -Replacement $runtimeCompanionReplacement `
    -AlreadyDoneMarker '**`KERNEL_ARCHITECTURE.md`** (kernel layer companion)'

# Master plan companion roadmap note (insert if missing)
$masterPlanMarker = '> **Companion roadmap**: K-series milestones'
$masterPlanBlock = @'
### Master plan

> **Companion roadmap**: K-series milestones для native ECS kernel migration documented в `KERNEL_ARCHITECTURE.md` Part 2. Combined timeline (M-series + K-series) detailed в `KERNEL_ARCHITECTURE.md` Part 8.
'@

if (Test-Path "docs/RUNTIME_ARCHITECTURE.md") {
    $runtimeContent = Get-Content -Path "docs/RUNTIME_ARCHITECTURE.md" -Raw
    if ($runtimeContent -notmatch [regex]::Escape($masterPlanMarker)) {
        # Try to insert before the first occurrence of "## Part 2" content (roadmap)
        if ($runtimeContent -match '(?m)^## Part 2[^\r\n]*\r?\n') {
            $insertion = $matches[0] + "`r`n" + $masterPlanBlock + "`r`n`r`n"
            $runtimeContent = $runtimeContent -replace [regex]::Escape($matches[0]), $insertion
            Set-Content -Path "docs/RUNTIME_ARCHITECTURE.md" -Value $runtimeContent -Encoding UTF8
            Write-Host "  INSERT Master plan note in docs/RUNTIME_ARCHITECTURE.md" -ForegroundColor Green
        } else {
            Write-Host "  SKIP Master plan note (Part 2 anchor not found)" -ForegroundColor DarkGray
        }
    } else {
        Write-Host "  SKIP Master plan note (already present)" -ForegroundColor DarkGray
    }
}

# Part 9 — Relationship к KERNEL_ARCHITECTURE.md
$runtimePart9 = @'
## Part 9: Relationship к KERNEL_ARCHITECTURE.md

RUNTIME_ARCHITECTURE.md (this) и KERNEL_ARCHITECTURE.md describe two halves of single architectural vision: native foundation под managed Application layer.

**Symmetric architecture**:
- This document (RUNTIME): Vulkan rendering layer, M-series milestones (M9.0-M9.8)
- KERNEL_ARCHITECTURE.md: ECS kernel layer, K-series milestones (K0-K8)
- Both: pure P/Invoke к native (vulkan-1.dll | DualFrontier.Core.Native.dll)
- Both: managed thin adapter layer
- Both: single ownership boundary, direction-disciplined (managed → native)

**Independent layers**: rendering знает nothing about ECS storage; ECS kernel знает nothing about Vulkan. Both reachable от managed Application layer through respective bridges.

**Combined timeline**: see KERNEL_ARCHITECTURE.md Part 8 для sequencing options (β3, β5, β6) и recommended approach.

**Cross-document invariants**: «без компромиссов», operating principle (data exists/doesn't), single ownership boundary, direction-discipline, long-horizon planning. See KERNEL_ARCHITECTURE.md Part 8 для full invariant list.
'@
Append-IfMissing -Path "docs/RUNTIME_ARCHITECTURE.md" -Marker "Part 9: Relationship к KERNEL_ARCHITECTURE.md" -Content $runtimePart9

# 3.2 METHODOLOGY.md — Native layer methodology adjustments
$methodologyAddendum = @'
## Native layer methodology adjustments

Kernel и runtime native layers introduce specific methodology adjustments documented в:
- `KERNEL_ARCHITECTURE.md` Part 7 (kernel-specific adjustments)
- `RUNTIME_ARCHITECTURE.md` Part 7 (runtime-specific adjustments)

Common adjustments (apply к both):
- C++ build verification mandatory pre-commit (CMake clean + selftest passing)
- P/Invoke marshalling check на every new `[DllImport]` declaration
- Cross-language commits acceptable when C++ + C# changes coupled
- Single ownership boundary preserved across managed/native communication
'@
Append-IfMissing -Path "docs/METHODOLOGY.md" -Marker "Native layer methodology adjustments" -Content $methodologyAddendum

# 3.3 MOD_OS_ARCHITECTURE.md — Modding с native ECS kernel
$modOsAddendum = @'
## Modding с native ECS kernel

When kernel migration к native completes (K-series, see `KERNEL_ARCHITECTURE.md`):
- Mod component types must be `unmanaged` structs (Path α)
- Class-based component storage prohibited (через ECS — mod state classes acceptable outside ECS)
- Vanilla mods register components и systems through same IModApi as third-party (vanilla = mods principle preserved)
- Mod replacement triggers second-graph rebuild (managed) — native side untouched

See `KERNEL_ARCHITECTURE.md` §1.9 (mod system registration) и §1.10 (component type registry) для full detail.
'@
Append-IfMissing -Path "docs/MOD_OS_ARCHITECTURE.md" -Marker "Modding с native ECS kernel" -Content $modOsAddendum

# 3.4 README.md — Architecture documents (if file exists)
$readmeAddendum = @'
### Architecture documents

- `docs/METHODOLOGY.md` — pipeline и methodology
- `docs/CODING_STANDARDS.md` — coding conventions
- `docs/MOD_OS_ARCHITECTURE.md` — modding architecture
- `docs/RUNTIME_ARCHITECTURE.md` — Vulkan rendering layer (M9.x)
- `docs/KERNEL_ARCHITECTURE.md` — native ECS kernel layer (K0-K8)
- `docs/CPP_KERNEL_BRANCH_REPORT.md` — Discovery report (experimental branch)
'@
if (Test-Path "README.md") {
    Append-IfMissing -Path "README.md" -Marker "docs/KERNEL_ARCHITECTURE.md" -Content $readmeAddendum
}

# === Section 4: Folder scaffolding ===========================================
Write-Host "[4] Creating folder structure..." -ForegroundColor Cyan

$folders = @(
    "src/DualFrontier.Core.Interop",
    "native/DualFrontier.Core.Native",
    "native/DualFrontier.Core.Native/include",
    "native/DualFrontier.Core.Native/src",
    "native/DualFrontier.Core.Native/test",
    "tests/DualFrontier.Core.Interop.Tests",
    "tools/briefs"
)
foreach ($f in $folders) {
    New-Item -ItemType Directory -Path $f -Force | Out-Null
    Write-Host "  MKDIR $f" -ForegroundColor Green
}

# === Section 5: MODULE.md generation =========================================
Write-Host "[5] Writing MODULE.md per kernel folder..." -ForegroundColor Cyan

$moduleInterop = @'
# DualFrontier.Core.Interop — Module Documentation

**Purpose**: P/Invoke bridge layer между managed Application и native `DualFrontier.Core.Native.dll`. Translates managed API calls к C ABI invocations с appropriate marshalling.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2

**Public API surface** (post-K8):
- `NativeWorld` — managed handle wrapper (`IDisposable`)
- `WriteCommandBuffer` — mod mutation accumulator
- `SpanLease<T>` — span lifetime guard (`IDisposable`)
- `ComponentTypeRegistry` — explicit type-id registration

**Dependencies**:
- BCL only (`System.Runtime.InteropServices`, `System.Numerics`)
- Project reference: `DualFrontier.Contracts`
- Native dependency: `DualFrontier.Core.Native.dll` (loaded at runtime)

**Layering**: bridge layer — Domain calls Interop, Interop calls Native. Domain не calls Native directly.

**TODO list**:
- K0: cherry-pick existing project from experimental branch
- K1: bulk operations + Span<T> primitives
- K2: ComponentTypeRegistry replaces FNV-1a hash
- K5: WriteCommandBuffer + SpanLease<T> production version

**Status**: scaffolding only. Implementation lives in cherry-picked branch contents (K0) и subsequent K-series milestones.
'@
Write-File -Path "src/DualFrontier.Core.Interop/MODULE.md" -Content $moduleInterop

$moduleNative = @'
# DualFrontier.Core.Native — Module Documentation

**Purpose**: ECS kernel storage + bootstrap orchestration + thread pool. C++20 implementation built independently от .NET solution via CMake.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2, §1.3, §1.4

**Public API surface** (post-K8):
- `df_capi.h` — extern «C» functions (~20 total)
- `df_world_*` — entity/component lifecycle
- `df_engine_bootstrap` — startup entry point (K3)
- `df_world_acquire_span` / `df_world_release_span` — span lifetime (K5)
- `df_world_flush_write_batch` — mutation flush (K5)
- `df_world_register_component_type` — type registration (K2)

**Dependencies**:
- C++20 stdlib only (`<vector>`, `<unordered_map>`, `<thread>`, `<atomic>`)
- No third-party libraries
- CMake 3.20+ build system

**Output artifact**: `DualFrontier.Core.Native.dll` (Windows) / `.so` (Linux) / `.dylib` (macOS)

**Layering**: lowest layer — knows nothing of game domain. Could be open-sourced separately as «sparse-set ECS in C++ с C ABI».

**TODO list**:
- K0: cherry-pick existing implementation from experimental branch
- K1: bulk operations + span access functions
- K2: component type registry function
- K3: bootstrap_graph.h/cpp + thread_pool.h/cpp
- K5: write_command_buffer.h/cpp

**Status**: scaffolding only. Implementation lives in cherry-picked branch contents (K0) и subsequent K-series milestones.
'@
Write-File -Path "native/DualFrontier.Core.Native/MODULE.md" -Content $moduleNative

$moduleNativeInclude = @'
# DualFrontier.Core.Native/include — Public Headers

**Purpose**: Public C ABI и C++ implementation headers.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2

**Files** (post-K8):
- `df_capi.h` — public C ABI (extern «C» functions, DllExport/DllImport macros)
- `entity_id.h` — EntityId POD + pack/unpack helpers
- `sparse_set.h` — header-only template (or removed K0.2 if unused)
- `component_store.h` — type-erased RawComponentStore
- `world.h` — World class declaration
- `bootstrap_graph.h` — startup task graph (K3 NEW)
- `thread_pool.h` — std::thread pool (K3 NEW)
- `write_command_buffer.h` — mutation batch parser (K5 NEW)

**Visibility rules**:
- C ABI in `df_capi.h` — exported via `DF_API` macro
- C++ classes — `internal` к library, not exported
- Implementation в corresponding .cpp файлах в `../src/`

**Status**: K0 cherry-picks existing headers; K3, K5 add new headers.
'@
Write-File -Path "native/DualFrontier.Core.Native/include/MODULE.md" -Content $moduleNativeInclude

$moduleNativeSrc = @'
# DualFrontier.Core.Native/src — Implementation Files

**Purpose**: C++ implementation files corresponding к headers в `../include/`.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.2

**Files** (post-K8):
- `world.cpp` — World methods
- `capi.cpp` — extern «C» wrappers
- `bootstrap_graph.cpp` — startup graph + Kahn's algorithm (K3 NEW)
- `thread_pool.cpp` — std::thread pool implementation (K3 NEW)
- `write_command_buffer.cpp` — mutation batch parser (K5 NEW)

**Compilation**:
- All files compiled into single shared library `DualFrontier.Core.Native.dll`
- CMakeLists.txt в parent directory orchestrates build

**Status**: K0 cherry-picks existing implementations; K3, K5 add new implementations.
'@
Write-File -Path "native/DualFrontier.Core.Native/src/MODULE.md" -Content $moduleNativeSrc

$moduleNativeTest = @'
# DualFrontier.Core.Native/test — Native Self-tests

**Purpose**: Standalone executable validating C ABI without requiring .NET runtime. Run after every native build.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.11

**Current scenarios** (К0 от branch):
- `scenario_basic_crud` — entity/component CRUD round-trip
- `scenario_deferred_destroy` — slot recycling, version invalidation
- `scenario_sparse_set_swap_remove` — swap-with-last correctness
- `scenario_throughput` — 100k entity stress test

**К-series additions**:
- К1: `scenario_bulk_add` — bulk operations correctness
- К1: `scenario_span_access` — span acquisition/release с atomic counter
- К3: `scenario_bootstrap_graph` — topological sort + parallel execution
- К5: `scenario_write_batch` — write command buffer parsing

**Run command** (post-build):
```bash
./build/df_native_selftest
# Expected output: ALL PASSED
```

**Status**: К0 имports existing 4 scenarios; subsequent milestones add scenarios.
'@
Write-File -Path "native/DualFrontier.Core.Native/test/MODULE.md" -Content $moduleNativeTest

$moduleInteropTests = @'
# DualFrontier.Core.Interop.Tests — Bridge Test Project

**Purpose**: xUnit-based equivalence tests verifying managed bridge correctly mirrors managed `World` semantics. Validates marshalling, span lifetime invariants, write batch serialization.

**Reference**: `docs/KERNEL_ARCHITECTURE.md` §1.11, K2 milestone

**Status**: scaffolding only. Tests added в K2 milestone (~30-40 tests).

**Test categories** (К2 target):
- `NativeWorldTests` — equivalence с managed World (CRUD round-trip)
- `EntityIdPackingTests` — bit-level pack/unpack invariants
- `ComponentTypeRegistryTests` — registration, GetId, idempotency
- `SpanLeaseTests` — acquisition/release lifecycle, mutation rejection
- `WriteCommandBufferTests` — serialization correctness, flush semantics

**Dependencies**:
- xUnit + xunit.runner.visualstudio + Microsoft.NET.Test.Sdk
- Project reference: `DualFrontier.Core.Interop`
- Project reference: `DualFrontier.Core` (для managed World comparison)
- Native dependency: `DualFrontier.Core.Native.dll` (copied к output via post-build target)

**Goal**: 472 + ~30 = ~500 total tests passing post-K2.
'@
Write-File -Path "tests/DualFrontier.Core.Interop.Tests/MODULE.md" -Content $moduleInteropTests

# === Section 6: K-series brief skeletons =====================================
Write-Host "[6] Writing K-series brief skeletons..." -ForegroundColor Cyan

$briefK0 = @'
# K0 — Cherry-pick + cleanup от experimental branch

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K0
**Source**: `claude/cpp-core-experiment-cEsyH` per `docs/CPP_KERNEL_BRANCH_REPORT.md` §11.6

## Goal

Cherry-pick 7 substantive C++ commits от experimental branch onto current main. Clean up hygiene issues. Reconcile decision rule conflict.

## Time estimate

1-2 days

## Deliverables (high-level)

- Cherry-pick sequence (7 commits)
- Cleanup commits (`.gitignore`, dead code, `.vscode`, doc reconciliation)
- Single atomic commit chain ending с «native: kernel branch resumption + cleanup»

## TODO

- [ ] Author full brief (similar к M8.5/M8.6 brief structure)
- [ ] Include cherry-pick command sequence
- [ ] Include cleanup commit specifications
- [ ] Include verification protocol
- [ ] Include acceptance criteria

**Brief authoring trigger**: when ready к execute K0 (likely tomorrow или next session).
'@
Write-File -Path "tools/briefs/K0_CHERRY_PICK_BRIEF.md" -Content $briefK0

$briefK1 = @'
# K1 — Batching primitive

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K1
**Prerequisite**: K0 complete

## Goal

Bulk Add/Get + Span<T> access via extended C ABI. Validates batching hypothesis quantitatively (target ≤200μs for bulk add 10k vs current 400μs unbatched).

## Time estimate

3-5 days

## Deliverables (high-level)

- C ABI extension (4 new functions: bulk add, bulk get, span acquire, span release)
- Managed bridge extension (`AddComponents<T>`, `AcquireSpan<T>`)
- Native span counter + mutation rejection
- Selftest scenarios (К1: bulk add, span access)
- Benchmark: `NativeBulkAddBenchmark`

## TODO

- [ ] Author full brief
- [ ] Include C++ implementation outlines
- [ ] Include C# bridge outlines
- [ ] Include benchmark target metrics
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K0 closure.
'@
Write-File -Path "tools/briefs/K1_BATCHING_BRIEF.md" -Content $briefK1

$briefK2 = @'
# K2 — Type-id registry + bridge tests

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K2
**Prerequisite**: K1 complete

## Goal

Replace FNV-1a hash с explicit deterministic registry. Comprehensive bridge test coverage (~30-40 tests).

## Time estimate

2-3 days

## Deliverables (high-level)

- `ComponentTypeRegistry` class (sequential IDs)
- C ABI: `df_world_register_component_type`
- New project: `DualFrontier.Core.Interop.Tests`
- Tests: 30-40 covering NativeWorld, packing, registry, spans, write buffer

## TODO

- [ ] Author full brief
- [ ] Include test scaffolding template
- [ ] Include test category breakdown
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K1 closure.
'@
Write-File -Path "tools/briefs/K2_REGISTRY_TESTS_BRIEF.md" -Content $briefK2

$briefK3 = @'
# K3 — Native bootstrap graph + thread pool

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K3
**Prerequisite**: K2 complete

## Goal

Declarative startup task graph executed parallel где deps allow. Native scheduler used ONLY для bootstrap (not game tick).

## Time estimate

5-7 days

## Deliverables (high-level)

- `bootstrap_graph.h/cpp` — declarative `kStartupTasks` array, Kahn topological sort
- `thread_pool.h/cpp` — std::thread pool (N cores)
- `df_engine_bootstrap()` C ABI entry point
- Selftest scenario: `scenario_bootstrap_graph`
- Benchmark: `BootstrapTimeBenchmark` (parallel vs sequential)

## TODO

- [ ] Author full brief
- [ ] Enumerate startup tasks с dependencies
- [ ] Include thread pool design (work-stealing vs fixed-partitioned)
- [ ] Include benchmark target metrics (≤15ms typical hardware)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K2 closure.
'@
Write-File -Path "tools/briefs/K3_BOOTSTRAP_GRAPH_BRIEF.md" -Content $briefK3

$briefK4 = @'
# K4 — Component struct refactor (Path α)

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K4
**Prerequisite**: K3 complete

## Goal

Convert all class-based components к `unmanaged` structs. ~50-80 components в `DualFrontier.Components/`. Eliminates GC pressure structurally.

## Time estimate

2-3 weeks (substantial scope, mostly mechanical)

## Deliverables (high-level)

- All components converted от `class X : IComponent` к `struct X : IComponent`
- Systems updated to use `ref` semantics для mutation
- Tests updated (existing 472 still passing)
- Components с complex behavior split: pure-data struct + behavior class

## TODO

- [ ] Author full brief
- [ ] Inventory all 50-80 components
- [ ] Identify components needing redesign (behavior, reference fields)
- [ ] Include conversion sequence (5-10 components per commit)
- [ ] Include allocation profile target (zero managed allocs in component access)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K3 closure.
'@
Write-File -Path "tools/briefs/K4_COMPONENT_STRUCT_REFACTOR_BRIEF.md" -Content $briefK4

$briefK5 = @'
# K5 — Span<T> protocol + write command batching

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K5
**Prerequisite**: K4 complete

## Goal

Production-grade span lifetime guard + write command batching infrastructure. Mutation rejection during active spans. Single P/Invoke per phase для writes.

## Time estimate

1 week

## Deliverables (high-level)

- `SpanLease<T>` (`IDisposable`) wrapping native span pointer
- `WriteCommandBuffer` full implementation (Add/Remove/Destroy/Spawn commands)
- C++ command buffer parser
- `df_world_flush_write_batch` C ABI
- Native atomic counter mutation rejection
- Tests: span lifetime, write batch round-trip, mutation rejection

## TODO

- [ ] Author full brief
- [ ] Include byte-stream serialization format spec
- [ ] Include native parser implementation outline
- [ ] Include test scenarios (lifetime, rejection, round-trip)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K4 closure.
'@
Write-File -Path "tools/briefs/K5_SPAN_PROTOCOL_BRIEF.md" -Content $briefK5

$briefK6 = @'
# K6 — Second-graph rebuild on mod change

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K6
**Prerequisite**: K5 complete

## Goal

Managed dependency graph rebuilds when mods load/unload. AssemblyLoadContext integration. Native side untouched throughout.

## Time estimate

3-5 days

## Deliverables (high-level)

- `SystemGraph.Rebuild(modRegistry)` method
- `ModLoader.UnloadMod(modId)` + `ReloadMod(modId)`
- `PhaseCoordinator.OnModChanged()` (pause/rebuild/resume tick)
- Tests: rebuild correctness, unload+reload cycle, topological invariants

## TODO

- [ ] Author full brief
- [ ] Include AssemblyLoadContext lifecycle handling
- [ ] Include GC.Collect coordination spec
- [ ] Include edge cases (leaked refs, partial unload)
- [ ] Include acceptance criteria

**Brief authoring trigger**: after K5 closure.
'@
Write-File -Path "tools/briefs/K6_MOD_REBUILD_BRIEF.md" -Content $briefK6

$briefK7 = @'
# K7 — Performance measurement (tick-loop)

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K7
**Prerequisite**: K6 complete

## Goal

Representative-load benchmark applying §8 metrics rule (GC pause / p99 / long-run drift on weak hardware). Evidence base для K8 decision step.

## Time estimate

3-5 days

## Deliverables (high-level)

- `TickLoopBenchmark` — 50 pawns × full component set × 10k ticks
- Three variants: managed-current, managed-with-structs, native-with-batching
- Metrics: p50/p95/p99, GC count + duration, total allocations, drift
- Run on weak hardware (Docker cpu-limit OR secondary machine)
- Report: `docs/PERFORMANCE_REPORT_K7.md`

## TODO

- [ ] Author full brief
- [ ] Include benchmark scenario definition
- [ ] Include weak-hardware target spec
- [ ] Include report template
- [ ] Include acceptance criteria (metrics threshold для §8 rule)

**Brief authoring trigger**: after K6 closure.
'@
Write-File -Path "tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md" -Content $briefK7

$briefK8 = @'
# K8 — Decision step + production cutover

**Status**: SKELETON
**Reference**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K8
**Prerequisite**: K7 complete

## Goal

Apply §8 rule к K7 numbers. Decide path forward (Outcome 1/2/3). Execute decision (cutover, optional, или park).

## Time estimate

1 week (depending on outcome)

## Deliverables (high-level)

**Outcome 1 — native wins**: replace managed `World` с `NativeWorld`, migrate Application bootstrap, deprecate managed World, update modding API к v3.

**Outcome 2 — managed-with-structs wins**: native kernel optional optimization, components remain structs, document «native available но не required».

**Outcome 3 — equivalent**: park kernel work, document lessons learned.

## TODO

- [ ] Author full brief (after K7 closes — outcome-dependent content)
- [ ] Include decision matrix (which metrics → which outcome)
- [ ] Include cutover protocol (Outcome 1)
- [ ] Include rollback protocol if cutover fails
- [ ] Include acceptance criteria per outcome

**Brief authoring trigger**: after K7 results available.
'@
Write-File -Path "tools/briefs/K8_DECISION_BRIEF.md" -Content $briefK8

$briefsModule = @'
# tools/briefs — Brief Inventory

**Purpose**: Brief skeletons и full briefs для milestone execution.

**Reference**: `docs/METHODOLOGY.md`, `docs/KERNEL_ARCHITECTURE.md`, `docs/RUNTIME_ARCHITECTURE.md`

**Convention**:
- `K{N}_TITLE_BRIEF.md` — kernel milestone brief
- `M{N.M}_TITLE_BRIEF.md` — runtime milestone brief

**Status spectrum**:
- SKELETON — placeholder с TODO list (created at scaffolding time)
- DRAFT — brief authoring в progress
- READY — brief reviewed и approved для execution
- EXECUTED — milestone complete, brief retained для reference

**Current inventory**:
- K-series (kernel): K0-K8 skeletons (this scaffolding)
- M-series (runtime): per RUNTIME_ARCHITECTURE.md Part 2 (М9.0-М9.8 — TODO when приступаем к runtime)

**Workflow**:
1. Skeleton created at scaffolding time (this prompt's output)
2. Full brief authored when ready к execute milestone
3. Crystalka pastes full brief к Claude Code session
4. Execution + atomic commit
5. Brief marked EXECUTED, retained for history

**Cross-reference**: every brief должен reference its parent architecture document section.
'@
Write-File -Path "tools/briefs/MODULE.md" -Content $briefsModule

# === Section 7: Closing summary ==============================================
Write-Host ""
Write-Host "Integration complete" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. git status                                # verify changes" -ForegroundColor Yellow
Write-Host "  2. git add docs/ src/ native/ tests/ tools/  # stage" -ForegroundColor Yellow
Write-Host "  3. git commit с appropriate message          # atomic commit" -ForegroundColor Yellow
Write-Host "  4. Author K0 brief when ready к execute" -ForegroundColor Yellow
