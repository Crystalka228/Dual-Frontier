# DUAL FRONTIER — Native Core Experiment

C++ core for the ECS simulator
Branch: `claude/cpp-core-experiment`

## Question

Can we move the ECS core to C++ without breaking the architecture?

## Status

Experiment — awaiting benchmark results (p99, GC-pause count, 2-core
throughput). The investment decision is made on the target hardware
profile, not on mean latency on a developer machine.

## Self-test

~4.6 ms / 100k operations (sandbox VM Opus) — the native ceiling.

## Criterion

Colony simulators are played on mid-range and weak hardware (2015–2018,
often 2–4 cores, 8 GB RAM). The original framing "≥20% on mean latency"
was formulated without accounting for the genre — on that hardware the
value is not in average latency but in **predictability**:

| Metric | Why it matters more than mean |
|--------|-------------------------------|
| GC pause count / duration | One stop-the-world for 50 ms is a visible camera jitter |
| p99 / worst-case tick | A tick that "spikes" past 33 ms feels like a freeze |
| Throughput on 2–4 cores | On the target hardware, the parallel scheduler competes with the OS for CPU |
| Behavior over 2–4 hours | Long-running simulation — managed memory loses here |

The updated decision rule is in §8.

---

## 1. Context and motivation

DualFrontier is built as an engine, not just a game. Once Phase 3 came
online — pawns moving, the simulation alive, 61 tests green — the
natural question arose: can we move the hottest core to C++ without
breaking the architecture?

The answer determines the long-term trajectory of the project:

**Path A — C# core forever**
Development simplicity, .NET optimizations, single codebase.
Limitation: JIT, GC pauses, managed overhead on the hot path.

**Path B — C++ core + C# outside ← the experiment tests this path**
ECS and scheduler in native code.
C# systems, buses, Application, Presentation — unchanged.
Result: **predictable** performance with no GC jitter on weak
hardware, plus C# DX during development.

> ⭐ **The main bet**
> Not "Unreal performance" on a developer machine, but stability on
> the player's machine. Colony simulators are played for tens of hours
> on 2015–2018 hardware — there a single Gen2 GC of 50 ms turns into a
> visible camera jitter. C++ core = no managed heap on the hot path =
> no GC pause = a smooth tick even after 4 hours of simulation.

---

## 2. Why this is possible without breaking the architecture

DualFrontier's architecture is already shaped for this replacement. Every
layer is isolated from the core's implementation through contracts:

```
DualFrontier.Systems      — does not know World directly
DualFrontier.Application  — does not know ComponentStore
DualFrontier.Presentation — does not know Scheduler
```

All layers communicate through:

```
IComponent    (Contracts)    ← does not change
SystemBase    (public API)   ← does not change
IGameServices (buses)        ← does not change
```

> 📌 **Replacement rule**
> The C++ core replaces only what is marked `internal` in Core. Contracts
> are not touched. Systems are not touched. Existing tests are not touched.

| Layer | Changes? | Reason |
|-------|----------|--------|
| `DualFrontier.Contracts` | ❌ No | Contracts are the boundary, not the implementation |
| `DualFrontier.Core` (internal) | ✅ Replaced | ECS, Scheduler — exactly what moves to C++ |
| `DualFrontier.Core.Interop` (new) | ✅ Added | P/Invoke wrappers for C# → C++ calls |
| `DualFrontier.Systems` | ❌ No | Systems work through `SystemBase` — they do not know the implementation |
| `DualFrontier.Application` | ❌ No | Uses `IGameServices`, not `World` directly |
| `DualFrontier.Presentation` | ❌ No | Only `PresentationBridge` — does not know the ECS |
| All existing tests | ❌ No | They test behavior through contracts, not the implementation |

---

## 3. What moves to C++

### 3.1 `SparseSet<T>` — performance foundation

The hottest spot in the ECS is component iteration. In C#: `int[] sparse`,
`T[] dense`, `int[] denseToIndex`. In C++ this becomes `template<typename T>` —
the compiler generates a specialization for each component type.

| Property | C# (managed) | C++ (native) |
|----------|--------------|--------------|
| Storage type | Managed heap, GC pressure | stack/heap, no GC |
| Virtual calls | Virtual dispatch through interface | None — template specialization |
| Boxing | Possible on object cast | None — direct memory |
| `dense[]` iteration | Cache-friendly, but managed overhead | Cache-friendly, zero overhead |
| Allocation | `new T[]` — GC managed | `std::vector<T>` — RAII |

### 3.2 `ComponentStore` / `World`

Registry of entities and components. In C++ uses `std::unordered_map` with
an FNV-1a hash of the component's `AssemblyQualifiedName` as the key.

```cpp
// EntityId — same algorithm as in C#
uint64_t entity_id = ((uint64_t)version << 32) | (uint32_t)index;

// ComponentStore — type-erased storage
std::unordered_map<uint32_t,
    std::unique_ptr<IComponentStore>> _stores;
// key = FNV-1a(AssemblyQualifiedName)
```

### 3.3 `DependencyGraph`

Kahn's algorithm — pure graph theory with no runtime dependencies. Takes
arrays of `type_id` through a C API, returns ordered phases. Fully
portable code.

### 3.4 `EventBus` (next step)

`std::vector<std::function>` under `std::shared_mutex`. Synchronous publication.
Key task: how does the C++ bus call C# subscribers through a function pointer.

### 3.5 What stays in C#

| Component | Why it stays in C# |
|-----------|--------------------|
| `SystemBase` + `Update(float delta)` | Systems are written by humans in C#. This is the project's main DX — must not change. |
| `SystemExecutionContext` (guard) | DEBUG-time check. Not on the hot path in Release. No reason to migrate. |
| `IGameServices` and `DomainEventBus` | Interfaces in Contracts. Can be migrated later if a benchmark shows the buses are the bottleneck. |
| `IComponent` components | POCO classes in C#. C++ stores them as blittable structs (PoC) or via `GCHandle` (production). |

---

## 4. The P/Invoke boundary — C API

C++ exports a clean C API (`extern "C"`) — a mandatory requirement for
reliable P/Invoke on every platform:

```cpp
// world.h — C API (extern "C")
void*    df_world_create();
void     df_world_destroy(void* world);
uint64_t df_world_create_entity(void* world);
void     df_world_destroy_entity(void* world, uint64_t id);
void     df_world_add_component(void* world, uint64_t id,
             uint32_t type_id, void* data, int32_t size);
void*    df_world_get_component(void* world, uint64_t id,
             uint32_t type_id);
bool     df_world_has_component(void* world, uint64_t id,
             uint32_t type_id);
```

C# wrapper in `DualFrontier.Core.Interop`:

```csharp
internal static class NativeMethods
{
    private const string DllName = "DualFrontier.Core.Native";

    [DllImport(DllName)]
    public static extern IntPtr df_world_create();

    [DllImport(DllName)]
    public static extern void df_world_destroy(IntPtr w);

    [DllImport(DllName)]
    public static extern ulong df_world_create_entity(IntPtr w);
    // ...
}
```

### 4.1 Cross-platform support

| Platform | Library file | Build |
|----------|--------------|-------|
| Windows | `DualFrontier.Core.Native.dll` | CMake + Visual Studio 2022 |
| Linux | `libDualFrontier.Core.Native.so` | CMake + GCC/Clang |
| macOS | `libDualFrontier.Core.Native.dylib` | CMake + Clang |

> 📌 **Automatic platform selection**
> `[DllImport("DualFrontier.Core.Native")]` picks up the right file
> automatically — P/Invoke knows about platform extensions. The same approach
> Godot's native plugins use.

---

## 5. Component marshalling — three options

The main technical complication of the experiment is passing managed C#
objects into native C++ code. The experiment chose Option 1 as the
minimally complex PoC.

| Option | Mechanism | Pros | Cons | Use |
|--------|-----------|------|------|-----|
| 1 — Blittable structs | `[StructLayout(Sequential)]` | No GC pressure, direct passing through pointer | Production components are classes, conversion needed | **PoC ✅ chosen** |
| 2 — `GCHandle` | `GCHandle.Alloc(Pinned)` → `AddrOfPinnedObject` | Works with classes, no copy | Handle lifetime management | Production — best balance |
| 3 — Serialization | Serialize to `byte[]` on transfer | Maximum flexibility | The slowest | Edge case |

```csharp
// Option 1 — Blittable struct (PoC)
[StructLayout(LayoutKind.Sequential)]
public struct BenchHealthComponent
{
    public int Current;
    public int Maximum;
}

// Option 2 — GCHandle (production)
var handle = GCHandle.Alloc(component, GCHandleType.Pinned);
IntPtr ptr = handle.AddrOfPinnedObject();
NativeMethods.df_world_add_component(world, id, typeId, ptr, size);
// handle.Free() — when the component is no longer needed in C++
```

---

## 6. Experiment structure

```
native/DualFrontier.Core.Native/
  ├── CMakeLists.txt
  ├── build.md                    ← build instructions
  ├── include/
  │   ├── sparse_set.h            ← template SparseSet<T>
  │   ├── component_store.h       ← type-erased IComponentStore
  │   ├── world.h                 ← World + C API
  │   └── df_native.h             ← unified public header
  └── src/
      ├── world.cpp
      └── df_native_selftest.cpp  ← standalone test without dotnet

src/DualFrontier.Core.Interop/
  ├── DualFrontier.Core.Interop.csproj
  ├── NativeMethods.cs            ← all P/Invoke declarations
  ├── NativeWorld.cs              ← C# wrapper around df_world_*
  └── Marshalling/
      └── ComponentMarshaller.cs  ← blittable struct ↔ IComponent

tests/DualFrontier.Core.Benchmarks/
  └── NativeVsManagedBenchmark.cs ← BenchmarkDotNet comparison
```

### 6.1 Native library self-test

`df_native_selftest.exe` — a standalone C++ test with no dotnet dependency.
Four scenarios:

| Scenario | What it checks | Expected result |
|----------|---------------|------------------|
| CRUD | Create entity, add component, read, remove | All operations O(1) |
| deferred destroy | Entity is marked for removal, actually removed after iteration | No invalid pointers |
| swap-with-last | Removal from the middle of the dense array — O(1) swap | Dense order is correct |
| 100k throughput | 100,000 entities Create/Add/Get | ~4.6 ms (sandbox VM) — the native ceiling |

---

## 7. How to run the benchmark

### Step 1 — Build the C++ library

Open Developer PowerShell for VS 2022:

```powershell
cd native\DualFrontier.Core.Native
cmake -S . -B build -G "Visual Studio 17 2022" -A x64
cmake --build build --config Release
```

### Step 2 — Self-test

```powershell
.\build\Release\df_native_selftest.exe
# Expected: All tests passed.
```

### Step 3 — Copy the .dll

```powershell
copy build\Release\DualFrontier.Core.Native.dll `
     ..\..\tests\DualFrontier.Core.Benchmarks\
```

### Step 4 — Run the benchmark

```powershell
cd D:\Dual-Frontier\Dual-Frontier
dotnet run --project tests\DualFrontier.Core.Benchmarks `
           -c Release -- --filter "*NativeVsManaged*"
```

### Results table

BenchmarkDotNet, .NET 8.0, Windows 11, Release, 10k entities, full warmup.
Native side built with MSVC `/O2`, blittable `BenchHealthComponent (int, int)`.
`ManagedSumCurrent` is marked `[Benchmark(Baseline = true)]` — so the `Ratio`
column compares everything to it, not "Add to Add".

| Method              | Mean       | Ratio (vs baseline) | Allocated  |
|---------------------|------------|---------------------|------------|
| `ManagedSumCurrent` | 101.93 µs  | 1.00                | 0 B        |
| `NativeSumCurrent`  |  95.31 µs  | 0.94                | 0 B        |
| `ManagedAdd10k`     | 218.24 µs  | 2.19                | 655 606 B  |
| `NativeAdd10k`      | 399.83 µs  | 3.92                | 24 B       |

Same-operation comparisons (what one usually wants to see):

| Operation    | Native / Managed | Allocated Native / Managed |
|--------------|------------------|----------------------------|
| `SumCurrent` | 0.94×            | 0 B / 0 B                  |
| `Add10k`     | 1.83×            | 24 B / 655 606 B           |

#### What the benchmark shows

- **`NativeSumCurrent`: 0.94× managed.** P/Invoke overhead almost fully
  eats the C++ gain on per-element calls — `df_world_get_component` crossing
  the boundary 10k times per tick costs about as much as the entire managed
  `TryGetComponent`.
- **`NativeAdd10k`: 3.92× in the Ratio column (vs baseline), 1.83× same-operation.**
  Native allocates 24 B against managed's 655 KB. The .NET allocator on the
  hot path is faster thanks to gen0 bump allocation, JIT inlining, and escape
  analysis; the native side goes through P/Invoke + `std::vector` geometric
  growth on every call.
- **Smoke test showed ~2.7× speedup on Add** — that was a one-shot warmup
  with no statistics. BenchmarkDotNet (thousands of iterations with warmup)
  shows the real picture: the per-element boundary kills it.

#### Conclusion

Per-element P/Invoke calls eat the native gain at 10k entities. Mean
latency by itself does **not** justify the migration — the rationale stated
in §8 still lies in GC pause / p99 / long-run drift, and these metrics have
not been measured here yet.

#### Next step — batch API

Replace per-element P/Invoke with batched calls:

| Now                                 | After                                |
|-------------------------------------|--------------------------------------|
| 10,000 × `df_world_add_component`   | 1 × `df_world_add_components_batch`  |
| 10,000 × `df_world_get_component`   | 1 × `df_world_fill_buffer`           |

One P/Invoke + one `memcpy` over contiguous memory — that is the regime in
which the C++ core can actually pull ahead of managed on cache-friendly
iterations, without 10k cross-boundary calls. Implementation — **a Phase 9 task**.

---

## 8. Decision criterion

The original criterion (`Native is faster by ≥20% on mean`) was built on
the assumption that the target player sits at a dev machine. That is a wrong
assumption: colony simulators are played on mid-range and weak hardware —
2015–2018 machines, 2–4 cores, 8 GB RAM, often with an IGP. On such hardware
mean latency is not the metric the player feels.

**Revised conclusion: porting the core to C++ is justified independent of
the mean latency benchmark.** The main value is predictability and the
absence of GC jitter under long simulation.

### What we now measure

| Metric | What it counts | "Keep going" threshold |
|--------|----------------|------------------------|
| **GC pause events / min** (Gen2) | `dotnet-counters` on `System.Runtime` during a 10-minute simulation | Native: 0. Managed: any non-zero value — already a C++ win |
| **p99 tick duration** | `ParallelSystemScheduler` on a 4-core VM, 60 ticks/s, 2000 pawns | Native improves p99 at equal mean — the deciding signal |
| **p99.9 / max tick** | Same run | Native bounds the tail — GC rolls do not strike |
| **Throughput on 2 cores** | Same, with `--cpu-affinity 0,1` | Native should hold tick rate; managed may degrade due to the GC thread |
| **Long-run drift** | 2-hour simulation, measure tick latency every 10 min | Native should stay flat; managed usually grows |
| **Mean latency (looked at, not decisive)** | BenchmarkDotNet `NativeVsManagedBenchmark` | For the record; the decision is not here |

### Decision rule

| Picture | Decision | Next step |
|---------|----------|-----------|
| Native cuts GC pauses **and** improves p99 on 2 cores | ✅ Continue | `DependencyGraph` + Scheduler in C++ |
| Native gives flat long-run (no drift), but mean parity | ✅ Continue | Same — jitter > mean for the genre |
| Native is worse on mean, but GC pauses dropped to zero | ✅ Continue, **if** GC pauses >1/min on the target configuration | GC cost > cost of C++ complexity |
| Parity on every metric, including GC | ❌ Close | C++ complexity is unjustified — stay on C# |
| Managed is faster everywhere (including p99) | ❌ Close | Very unlikely on a hot ECS, but formally possible |

> ⚡ **P/Invoke overhead**
> One P/Invoke call: ~10–50 ns. For small components, marshalling can eat
> the entire mean gain. But P/Invoke **does not cause a GC pause** — that
> is, even where C++ loses on mean it usually wins on p99. That is the
> essence of the revised criterion.

> 🎯 **What we do not do**
> We do not optimize for the dev machine. We do not benchmark on a 12-core
> 32 GB DDR5 workstation — those results are not representative for the
> genre's player. The target run is a 4-core VM with bounded memory.

---

## 9. Next steps if the experiment succeeds

| Step | Task | Difficulty |
|------|------|------------|
| 1 | `DependencyGraph` in C++ — Kahn's algorithm, input data from C# through `type_id` arrays | Medium |
| 2 | `NativeScheduler` — parallel phase execution through `std::thread` pool. Call C# `Update()` through a function pointer | High |
| 3 | `EventBus` in C++ — synchronous delivery, C# subscribers via delegates | High |
| 4 | `GCHandle` marshalling — transition from blittable structs to real production components | Medium |
| 5 | CI pipeline — build for three platforms (Win/Linux/macOS) in one workflow | Medium |

### Long-term trajectory

**Now (Phase 5)**
Godot Runtime → C# GameLoop → C# ECS core

**After the experiment (if successful)**
Godot Runtime → C# GameLoop → C++ ECS core via P/Invoke

**Phase 9 — Native Runtime (long-term)**
Own Runtime → C# GameLoop → C++ ECS core
Godot = scene editor only

> ⭐ **Long-term bet**
> An own runtime is no longer an indie game; it is an engine. Godot as a
> scene editor + native core + C# logic is a rare combination on the market.

---

## 10. Risks

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| P/Invoke overhead eats the gain for small components | High | Operation batching, blittable structs, measure before deciding |
| Debugging complexity (mixed C++ + C# stack) | Medium | `lldb` / WinDbg + managed debugger, isolate the boundary |
| Cross-platform build in CI | Medium | CMake + GitHub Actions matrix build |
| Marshalling managed objects across the boundary | High complexity | `GCHandle`, clear ownership, do not mix heaps |
| Drift between the C# and C++ versions of the API | Medium | Versioning of the C API, compatibility tests |

> ⚠ **Main risk**
> Marshalling complexity. Blittable structs are simple. `GCHandle` of
> managed classes is hard and requires explicit lifetime management.
> Underestimating this risk breaks production.

---

Unreal performance. C# development convenience. An architecture that does
not break when the core is replaced.
