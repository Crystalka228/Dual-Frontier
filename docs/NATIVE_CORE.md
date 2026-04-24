# Native Core Experiment

> Status: **experimental**. Lives only on branch `claude/cpp-core-experiment-cEsyH`.
> The managed `DualFrontier.Core` is unchanged — both cores coexist until the
> decision rule below is applied.

## 1. Why

`DualFrontier.Core` is hot-path code: every tick the `ParallelSystemScheduler`
walks component stores for dozens of systems. Before investing in deeper
managed micro-optimisations (struct-of-arrays, archetype, native-array
tricks), we wanted to know the upper bound: how much performance is still on
the table if the ECS spine is written in C++? This branch is a **one-day
feasibility probe**, not a rewrite. It ships a minimum-viable native
`World`/`SparseSet` and a C# P/Invoke wrapper so the two can be compared on
the same input.

## 2. Architectural boundary

```
┌──────────────────────────────────────────────────────────────────┐
│  DualFrontier.Presentation (Godot C#)                            │
├──────────────────────────────────────────────────────────────────┤
│  DualFrontier.Application (C#)                                   │
├──────────────────────────────────────────────────────────────────┤
│  DualFrontier.Systems (C#, SystemBase + [SystemAccess])          │
├──────────────────────────────────────────────────────────────────┤
│  DualFrontier.Core (C#, managed World/SparseSet, scheduler,      │
│                   SystemExecutionContext, EventBus)              │
│                                                                  │
│  DualFrontier.Core.Interop (C#, new)                             │
│    └─ NativeWorld ─── P/Invoke ───┐                              │
├──────────────────────────────────────┼──────────────────────────┤
│  DualFrontier.Core.Native (C++, new) ▼                           │
│    world.cpp, component_store.h, sparse_set.h, capi.cpp          │
│    Built by CMake, not part of the .sln.                         │
└──────────────────────────────────────────────────────────────────┘
```

### On the native side

| Ported to C++ | Reason |
|---|---|
| `SparseSet<T>` | Hot-path data structure. Template, header-only. |
| `RawComponentStore` | Type-erased byte-oriented mirror of `ComponentStore<T>`; allows a single native store to back any blittable C# component without per-type codegen. |
| `World` (subset) | Entity lifecycle, add/get/has/remove, deferred destruction via `flush_destroyed`. |

### Stays on C# — and why

| Kept on C# | Reason |
|---|---|
| `SystemBase` / derived systems | Systems are the DX surface of the project. Writing them in C++ would multiply the cost of every gameplay change without proportionate speedup — they spend most of their time reading components, which is the only part that benefits from native. |
| `SystemExecutionContext` | Pure DEBUG-time isolation guard. In Release the checks are elided, so there is nothing to speed up. |
| `DependencyGraph` / `ParallelSystemScheduler` | Ran out of scope for the one-day probe (see §6). Interesting port candidate once Step 5's baseline is green, not before. |
| `EventBus`, `GameServices` | Subscribers are closures over system state; C++ can't hold C# delegates without `GCHandle` churn that would dominate the cost. |
| `IComponent` (marker) | Contract surface, not a hot path. |
| Components (`HealthComponent` etc.) | Currently declared as `class`. See §5 for the constraint this imposes on the native store. |

## 3. P/Invoke boundary

The C API is defined in `native/DualFrontier.Core.Native/include/df_capi.h`
and wrapped by `src/DualFrontier.Core.Interop/NativeMethods.cs`. Mapping at a
glance:

| C# (managed) | C ABI | C++ (native) |
|---|---|---|
| `EntityId` (two `int`) | `uint64_t` packed (hi = Version, lo = Index) | `dualfrontier::EntityId` |
| Component `struct T : unmanaged` | `(uint32_t type_id, void* data, int32_t size)` | `RawComponentStore` keyed by `type_id` |
| `new NativeWorld()` | `df_world_create()` | `new dualfrontier::World` |
| `world.CreateEntity()` | `df_world_create_entity` | `World::create_entity` |
| `world.AddComponent<T>(id, v)` | `df_world_add_component` | `World::add_component` |
| `world.TryGetComponent<T>(id, out v)` | `df_world_get_component` | `World::get_component` |
| `world.HasComponent<T>(id)` | `df_world_has_component` | `World::has_component` |
| `world.RemoveComponent<T>(id)` | `df_world_remove_component` | `World::remove_component` |
| `world.Dispose()` | `df_world_destroy` | `delete world` |

### Component marshalling strategy — what we chose and why

Three options were on the table:

1. **Blittable struct pass-through (`unmanaged` generic constraint).**
   Zero allocations, a single `memcpy` per Add/Get. Requires that the
   component be a struct with no managed references.
2. **`GCHandle.Alloc(component, Pinned)`.**
   Supports reference-type components as-is, but each Add/Get walks the GC
   handle table, and the pinned handle blocks compaction. Overhead would
   dwarf the native speedup.
3. **Managed side retains objects; native side stores only handles/indices.**
   Roughly the "archetype with parallel C# arrays" design. More work than
   the one-day probe permits.

This experiment uses **option 1** and adds a benchmark-only
`BenchHealthComponent` struct to keep the comparison apples-to-apples. The
production `HealthComponent` is still a `class`; converting the full
component catalog to structs is a separate investigation that depends on
the outcome of this experiment.

The per-type `uint32_t` id is derived with FNV-1a over
`Type.AssemblyQualifiedName` in `NativeComponentType<T>`. It is cached in a
`static readonly` field so each component type pays the hash cost exactly
once. A production implementation should replace this with an explicit
registry (see §7).

## 4. How to build and run

```bash
# 1. Build the native library.
cd native/DualFrontier.Core.Native
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j

# 2. Run the native self-test (CRUD, deferred destroy, swap-with-last).
./build/df_native_selftest       # Linux / macOS
.\build\Release\df_native_selftest.exe   # Windows

# 3. Copy the artifact next to the managed benchmark output.
#    See native/DualFrontier.Core.Native/build.md for the exact recipe.

# 4. Build and run the comparison.
dotnet build DualFrontier.sln -c Release
dotnet run --project tests/DualFrontier.Core.Benchmarks -c Release
dotnet run --project tests/DualFrontier.Core.Benchmarks -c Release -- --full
```

`--full` hands off to BenchmarkDotNet and produces the numbers used in §6.
Without `--full` a lightweight `Stopwatch` smoke run is emitted, which is
enough to catch wiring regressions and verify the two worlds store
equivalent data (the harness compares checksums before reporting timings).

### Native self-test numbers on the probe machine

The `df_native_selftest` binary includes a throughput scenario. For
reference (Linux, GCC 13, `-O3`, sandbox VM):

```
create 100000 entities:   1.023 ms
add    100000 components: 2.695 ms
get    100000 components: 0.870 ms
```

These are pure native numbers — no P/Invoke overhead. They set the ceiling
against which the managed+interop stack will be measured; the gap between
this ceiling and the BenchmarkDotNet numbers is the marshalling tax.

## 5. Risks and open questions

- **Marshalling cost.** Each `AddComponent`/`GetComponent` pays for the
  P/Invoke transition, type-id lookup, and a `memcpy`. On a tight 10k-entity
  loop this can exceed the native-side speedup entirely. The smoke benchmark
  in `Program.cs` is intentionally arranged to reveal this.
- **Reference-type components.** The current component catalog uses
  `class`-based `IComponent`. Porting them to `struct` is a prerequisite for
  the native path to cover more than the benchmark; it is a non-trivial
  refactor touching every system that reads/writes those components.
- **Cross-platform build.** CMake + GCC/Clang/MSVC is table-stakes, but
  bundling three `DualFrontier.Core.Native.{dll,so,dylib}` variants into a
  single `DualFrontier.Core.Interop` NuGet-style layout (`runtimes/…/native`)
  is unsolved in the experiment — the CMake target outputs to `build/` and
  the consumer is expected to copy manually. Godot's shipping pipeline will
  need a tighter integration before this can land in `main`.
- **Debug story.** A native crash inside `df_world_*` produces a native
  stack trace that will not show up in a standard .NET debugger attached in
  managed-only mode. Mixed-mode debugging works on Windows but is awkward
  on Linux/macOS.
- **Version drift between C# and C++.** The EntityId packing layout and the
  type-id convention are duplicated between `df_capi.h` and
  `NativeComponentType<T>` / `EntityIdPacking`. A mismatch would silently
  produce wrong reads rather than a crash. If the experiment continues,
  generate both sides from a single source of truth (e.g. a JSON manifest
  or a C header consumed by a T4 template).

## 6. Decision rule

| Native vs managed (BenchmarkDotNet, `SumCurrent` × 10k) | Decision |
|---|---|
| ≥ 20% faster on Get and Add combined | Continue: port `DependencyGraph`, then the parallel scheduler. |
| 0–20% faster | Park. Revisit after a managed-side struct conversion: if the C# side gets ~50% faster by switching `HealthComponent` and friends to structs, the marginal case for C++ disappears. |
| Not faster / slower | Close the experiment. Marshalling overhead eats the C++ win; the simpler payoff is on the managed side (structs, Span, ref returns). |

Results will be filled in after BenchmarkDotNet is run on a real workstation
(the probe sandbox does not have a .NET SDK available).

> **Build-status on probe machine.** The managed solution (`DualFrontier.sln`)
> was not re-verified here because `dotnet` is not installed in the probe
> environment. No managed source files were modified — the only touch to
> existing C# code is a single additional `InternalsVisibleTo` line in
> `DualFrontier.Core.csproj` (visibility only, no behavior). The standing
> 61/61 managed tests on `main` should remain green; verify before landing.

## 7. Follow-ups if the experiment continues

1. Replace the FNV-1a type-id with an explicit registry (`NativeComponentRegistry`
   built at app startup) so collisions are impossible and ids are auditable.
2. Convert the component catalog to `struct`-based blittable payloads.
3. Port `DependencyGraph` (Kahn's algorithm) — pure graph theory, no CLR
   dependencies, and the managed version already has a tested contract.
4. Port `ParallelSystemScheduler` — the node pool and wait-for-phase logic
   are straightforward in C++; the hard part is calling back into C# to
   execute `SystemBase.Update`, which means reverse P/Invoke and careful
   thread-attach/detach.
5. Add a `runtimes/{win-x64,linux-x64,osx-x64}/native` layout to
   `DualFrontier.Core.Interop` so the library ships like a standard
   managed package.
6. Add a CI job that builds the native library on all three platforms and
   runs `df_native_selftest` + the BenchmarkDotNet suite.
