# K1 — Batching primitive (bulk Add/Get + Span<T> access)

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-07
**Status**: EXECUTED
**Reference docs**: `docs/KERNEL_ARCHITECTURE.md` Part 2 §K1, §1.6 (P/Invoke patterns), §1.7 (Span protocol), `docs/MIGRATION_PROGRESS.md` (K0 closure context)
**Predecessor**: K0 (`89a4b24`) — cherry-pick + cleanup, merged to main
**Target**: fresh feature branch `feat/k1-batching-primitive` от `main`
**Estimated time**: 3–5 days at hobby pace
**Estimated LOC delta**: +500–800

---

## Status: EXECUTED

**Date**: 2026-05-07
**Branch**: `feat/k1-batching-primitive`
**Closure SHA**: `e2c50b8` (last K1-substantive commit — `NativeBulkAddBenchmark`)

Full executable brief authored and executed. See git log on
`feat/k1-batching-primitive` for the atomic commit sequence
(`92a90de` native ABI → `e8aa42e` managed bridge → `74e51ef` selftest →
`e2c50b8` benchmark) and `docs/MIGRATION_PROGRESS.md` K1 entry for the
closure record (lessons learned, scope deltas).

---

## Goal

Расширить native C ABI batching-операциями для устранения per-entity P/Invoke overhead, который Discovery report зафиксировал как главное узкое место (NativeAdd10k ratio 1.83× **slower** than managed). K1 валидирует hypothesis что bulk operations + Span<T> access восстанавливают преимущество native через elimination of crossing overhead.

**В scope**:
- 4 новых функции в C ABI: `df_world_add_components_bulk`, `df_world_get_components_bulk`, `df_world_acquire_span`, `df_world_release_span`
- Native side: span lifetime tracking через `std::atomic<int> active_spans_` counter в `World`
- Native side: mutation rejection при active spans (throw `std::logic_error`, ABI returns failure code)
- Managed bridge: `NativeWorld.AddComponents<T>(ReadOnlySpan<EntityId>, ReadOnlySpan<T>)` bulk method
- Managed bridge: `NativeWorld.AcquireSpan<T>()` returning `SpanLease<T>` (skeleton — full IDisposable wrapper в K5)
- Selftest extension: 2 new scenarios (bulk add correctness, span access invariants)
- Benchmark extension: `NativeBulkAddBenchmark` validating speedup hypothesis

**НЕ в scope** (откладывается):
- Explicit `ComponentTypeRegistry` replacing FNV-1a — это **K2** (отдельный milestone)
- `WriteCommandBuffer` full implementation — это **K5** (только skeleton в K1, см. §1.8)
- Native bootstrap graph / thread pool — это **K3**
- `DualFrontier.Core.Interop.Tests` project — это **K2**
- Component struct refactor — это **K4**
- Никаких changes в managed Domain layer (Core / Components / Systems / Application)

---

## Architectural context (LOCKED, неизменно)

K1 не принимает архитектурных решений. Все они зафиксированы в `KERNEL_ARCHITECTURE.md`:

- **K-L7**: Span protocol = read-only spans + write command batching (mutation semantics explicit)
- **K-L8**: Component lifetime = native owns storage, managed holds opaque IntPtr
- **§1.7**: Span acquisition flow с atomic counter — точная спецификация native invariant
- **§1.6**: P/Invoke style — classic `[DllImport]` (matches branch); `[LibraryImport]` deferred к K7+ if profiling demands

K1 implementing **точно эти спецификации**. Любое отклонение в реализации требует amendment LOCKED docs, не tactical workaround в коде.

---

## Pre-flight checks

Применяя descriptive pre-flight принцип (`METHODOLOGY.md` v1.2 «Native layer methodology adjustments»):

### Hard gates (STOP-eligible)

Если ЛЮБАЯ из этих проверок не пройдёт — STOP, escalate to human, await guidance.

#### HG-1: Working tree clean
```powershell
cd D:\Colony_Simulator\Colony_Simulator
git status                                # должен быть clean
```

#### HG-2: K0 successfully merged to main
```powershell
git log main --oneline | Select-String "K0|cherry-pick|native" | Select-Object -First 5
# Ожидаем: видеть commits типа "docs(briefs): K0 brief skeleton marked EXECUTED",
# "docs(migration): K0 closure recorded", "docs(native): annotate SparseSet"
```

Если K0 commits не видны на main — STOP, K0 не merged, K1 не должен начинаться.

#### HG-3: Native source intact

```powershell
Test-Path native\DualFrontier.Core.Native\include\df_capi.h        # True
Test-Path native\DualFrontier.Core.Native\src\capi.cpp              # True
Test-Path native\DualFrontier.Core.Native\src\world.cpp             # True
Test-Path native\DualFrontier.Core.Native\test\selftest.cpp         # True
Test-Path native\DualFrontier.Core.Native\CMakeLists.txt            # True
```

Если хоть один False — STOP, K0 артефакты повреждены.

#### HG-4: Managed bridge intact

```powershell
Test-Path src\DualFrontier.Core.Interop\NativeWorld.cs              # True
Test-Path src\DualFrontier.Core.Interop\NativeMethods.cs            # True
Test-Path src\DualFrontier.Core.Interop\DualFrontier.Core.Interop.csproj  # True
```

#### HG-5: Baseline tests passing

```powershell
dotnet test
# Ожидаем: 472 passing, 0 failed
```

Если число != 472 — STOP, baseline сместился, обновить ожидание перед продолжением.

#### HG-6: Native build clean

```powershell
cd native\DualFrontier.Core.Native
cmake -S . -B build -A x64 -DCMAKE_BUILD_TYPE=Release
cmake --build build --config Release
# Ожидаем: 0 errors, 0 warnings
```

#### HG-7: Native selftest passing

```powershell
.\build\Release\df_native_selftest.exe
# Ожидаем: "ALL PASSED", exit code 0
```

#### HG-8: Tooling

```powershell
cmake --version                           # >= 3.20
dotnet --version                          # 8.x or 10.x
```

### Informational checks (record-only, не STOP)

Эти проверки описывают environment для audit trail. Расхождение с ожидаемым — записать, продолжить.

#### INF-1: Current C ABI surface count

```powershell
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "DF_API" | Measure-Object
# Expected: 12 baseline functions (post-K0)
# Record actual count
```

#### INF-2: Selftest scenario count

```powershell
Select-String -Path native\DualFrontier.Core.Native\test\selftest.cpp -Pattern "scenario_" | Measure-Object
# Expected: 4 scenarios (basic_crud, deferred_destroy, sparse_set_swap_remove, throughput)
# K1 will add 2 more, target: 6
```

#### INF-3: Benchmark count

```powershell
Select-String -Path tests\DualFrontier.Core.Benchmarks\NativeVsManagedBenchmark.cs -Pattern "\[Benchmark\]" | Measure-Object
# Expected: 4 (ManagedSumCurrent, NativeSumCurrent, ManagedAdd10k, NativeAdd10k)
# K1 will add NativeBulkAddBenchmark (separate file)
```

#### INF-4: Recent commit history

```powershell
git log main --oneline -5
# Record HEAD SHA for K1 closure reference
```

---

## Step 1 — Branch setup

```powershell
git checkout main
git pull origin main
git checkout -b feat/k1-batching-primitive main
```

Branch name: **`feat/k1-batching-primitive`** (точно).

Verification:
```powershell
git log -1 --format="%H %s"
git status                                # clean tree
```

---

## Step 2 — Native C ABI extension

K1 расширяет `df_capi.h` четырьмя новыми функциями. Все следуют existing conventions (Cdecl, `df_*` prefix, `DF_API` visibility, numeric return codes).

### 2.1 — Edit `native/DualFrontier.Core.Native/include/df_capi.h`

Добавить **после** existing `df_world_component_count` declaration, **перед** closing `#ifdef __cplusplus / extern "C"` block:

```cpp
/*
 * K1 batching primitives (added 2026-05-07).
 *
 * Bulk operations eliminate per-entity P/Invoke overhead by transmitting
 * arrays of entities + components in a single crossing.
 *
 * Span access provides direct read-only view into native dense storage.
 * The span pointer is valid until df_world_release_span is called OR
 * until any mutation (add/remove/destroy) is attempted (which will fail
 * while spans are active).
 *
 * Span lifetime contract:
 *   1. Caller calls df_world_acquire_span -> receives dense ptr + indices ptr + count.
 *   2. Caller iterates without further P/Invokes.
 *   3. Caller MUST call df_world_release_span before any mutation.
 *   4. Multiple concurrent spans (different type_ids OR same type_id) allowed.
 *   5. Mutation attempt while any span is active throws (caught at boundary,
 *      function returns 0 / no-op).
 */

DF_API void            df_world_add_components_bulk(
                           df_world_handle world,
                           const uint64_t* entities,
                           uint32_t type_id,
                           const void* component_data,
                           int32_t component_size,
                           int32_t count);

DF_API int32_t         df_world_get_components_bulk(
                           df_world_handle world,
                           const uint64_t* entities,
                           uint32_t type_id,
                           void* out_data,
                           int32_t component_size,
                           int32_t count);

DF_API int32_t         df_world_acquire_span(
                           df_world_handle world,
                           uint32_t type_id,
                           const void** out_dense_ptr,
                           const int32_t** out_indices_ptr,
                           int32_t* out_count);

DF_API void            df_world_release_span(
                           df_world_handle world,
                           uint32_t type_id);
```

**Rationale для signatures**:

- `add_components_bulk` — array of entities + single contiguous component array. Caller responsible for matching length. `count` parameter avoids reading beyond bounds.
- `get_components_bulk` — returns count of successfully read components (0 = none, N = first N had component). Allows partial reads when some entities lack the component.
- `acquire_span` — out-parameters receive pointers into native dense storage. `int32_t` return = 0 on failure (invalid type_id, world disposed), 1 on success.
- `release_span` — void; idempotent on already-released (no-op, no error).

### 2.2 — Edit `native/DualFrontier.Core.Native/include/world.h`

Добавить в `World` class declaration:

**Private member** (after `pending_destroy_`):
```cpp
std::atomic<int32_t> active_spans_{0};
```

**Public methods** (after `component_count`):
```cpp
void add_components_bulk(const EntityId* entities, uint32_t type_id,
                         const void* component_data, int32_t component_size,
                         int32_t count);

int32_t get_components_bulk(const EntityId* entities, uint32_t type_id,
                            void* out_data, int32_t component_size,
                            int32_t count) const noexcept;

bool acquire_span(uint32_t type_id, const void** out_dense_ptr,
                  const int32_t** out_indices_ptr, int32_t* out_count) noexcept;

void release_span(uint32_t type_id) noexcept;

[[nodiscard]] int32_t active_spans_count() const noexcept {
    return active_spans_.load(std::memory_order_acquire);
}
```

**`<atomic>` include** — добавить в includes block если ещё не там:
```cpp
#include <atomic>
```

### 2.3 — Edit `native/DualFrontier.Core.Native/src/world.cpp`

#### Modify existing `add_component` для span-mutation rejection

Замените существующую `add_component` implementation на:

```cpp
void World::add_component(EntityId id, uint32_t type_id, const void* data,
                          int32_t size) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        // K1: span lifetime contract — no mutation while spans active.
        // Throw is caught at C ABI boundary (capi.cpp).
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    RawComponentStore* store = get_or_create_store(type_id, size);
    store->add(id.index, data, size);
}
```

Аналогично — `remove_component` и `destroy_entity`:

```cpp
void World::remove_component(EntityId id, uint32_t type_id) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    auto it = stores_.find(type_id);
    if (it == stores_.end()) return;
    it->second->remove(id.index);
}

void World::destroy_entity(EntityId id) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!is_alive(id)) return;
    ++versions_[id.index];
    --live_count_;
    pending_destroy_.push_back(id);
}
```

`flush_destroyed` тоже mutating — добавить same check:
```cpp
void World::flush_destroyed() {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    // ... existing implementation ...
}
```

#### Add new methods

В конец `World` implementation:

```cpp
void World::add_components_bulk(const EntityId* entities, uint32_t type_id,
                                const void* component_data, int32_t component_size,
                                int32_t count) {
    if (active_spans_.load(std::memory_order_acquire) > 0) {
        throw std::logic_error("Cannot mutate while spans are active");
    }
    if (!entities || !component_data || component_size <= 0 || count <= 0) return;

    RawComponentStore* store = get_or_create_store(type_id, component_size);
    const uint8_t* data_bytes = static_cast<const uint8_t*>(component_data);

    for (int32_t i = 0; i < count; ++i) {
        if (!is_alive(entities[i])) continue;  // skip dead entities silently
        store->add(entities[i].index,
                   data_bytes + static_cast<std::size_t>(i) * component_size,
                   component_size);
    }
}

int32_t World::get_components_bulk(const EntityId* entities, uint32_t type_id,
                                   void* out_data, int32_t component_size,
                                   int32_t count) const noexcept {
    if (!entities || !out_data || component_size <= 0 || count <= 0) return 0;

    const RawComponentStore* store = find_store(type_id);
    if (!store) return 0;

    uint8_t* out_bytes = static_cast<uint8_t*>(out_data);
    int32_t successful = 0;

    for (int32_t i = 0; i < count; ++i) {
        if (!is_alive(entities[i])) {
            // Zero-fill slot for absent entity (deterministic output)
            std::memset(out_bytes + static_cast<std::size_t>(i) * component_size,
                        0, component_size);
            continue;
        }
        if (store->get(entities[i].index,
                       out_bytes + static_cast<std::size_t>(i) * component_size,
                       component_size)) {
            ++successful;
        } else {
            std::memset(out_bytes + static_cast<std::size_t>(i) * component_size,
                        0, component_size);
        }
    }
    return successful;
}

bool World::acquire_span(uint32_t type_id, const void** out_dense_ptr,
                         const int32_t** out_indices_ptr,
                         int32_t* out_count) noexcept {
    if (!out_dense_ptr || !out_indices_ptr || !out_count) return false;

    const RawComponentStore* store = find_store(type_id);
    if (!store || store->count() == 0) {
        *out_dense_ptr = nullptr;
        *out_indices_ptr = nullptr;
        *out_count = 0;
        // Increment counter even for empty span (release must match)
        active_spans_.fetch_add(1, std::memory_order_acquire);
        return true;
    }

    // Note: RawComponentStore needs a public accessor for dense_bytes_.data()
    // and dense_to_index_.data() — see §2.4 below for the required addition.
    *out_dense_ptr = store->dense_data();
    *out_indices_ptr = store->dense_indices().data();
    *out_count = store->count();

    active_spans_.fetch_add(1, std::memory_order_acquire);
    return true;
}

void World::release_span(uint32_t type_id) noexcept {
    (void)type_id;  // currently ignored — counter is global, not per-type.
                    // Per-type tracking added in K5 if mutation granularity needed.
    int32_t prev = active_spans_.fetch_sub(1, std::memory_order_release);
    // Underflow guard: if released more than acquired, clamp to 0
    if (prev <= 0) {
        active_spans_.store(0, std::memory_order_release);
    }
}
```

### 2.4 — Edit `native/DualFrontier.Core.Native/include/component_store.h`

`acquire_span` requires public accessor for dense bytes pointer. Add к `RawComponentStore` public section (next to `count()`):

```cpp
[[nodiscard]] const void* dense_data() const noexcept {
    return dense_bytes_.data();
}
```

(Method `dense_indices()` already exists per Discovery report §5.5.)

### 2.5 — Edit `native/DualFrontier.Core.Native/src/capi.cpp`

Добавить **после** existing `df_world_component_count` implementation:

```cpp
DF_API void df_world_add_components_bulk(df_world_handle world,
                                         const uint64_t* entities,
                                         uint32_t type_id,
                                         const void* component_data,
                                         int32_t component_size,
                                         int32_t count) {
    if (!world || !entities || !component_data) return;
    try {
        // Unpack ulong array to EntityId array on stack/heap.
        // For typical counts (10-1000), heap allocation acceptable;
        // for hot-path tick loops, K5 WriteCommandBuffer eliminates this.
        std::vector<EntityId> ids(static_cast<std::size_t>(count));
        for (int32_t i = 0; i < count; ++i) {
            ids[i] = unpack_entity(entities[i]);
        }
        as_world(world)->add_components_bulk(
            ids.data(), type_id, component_data, component_size, count);
    } catch (...) {
        // Swallow — bulk add is void on the C# side.
    }
}

DF_API int32_t df_world_get_components_bulk(df_world_handle world,
                                            const uint64_t* entities,
                                            uint32_t type_id,
                                            void* out_data,
                                            int32_t component_size,
                                            int32_t count) {
    if (!world || !entities || !out_data) return 0;
    try {
        std::vector<EntityId> ids(static_cast<std::size_t>(count));
        for (int32_t i = 0; i < count; ++i) {
            ids[i] = unpack_entity(entities[i]);
        }
        return as_world(world)->get_components_bulk(
            ids.data(), type_id, out_data, component_size, count);
    } catch (...) {
        return 0;
    }
}

DF_API int32_t df_world_acquire_span(df_world_handle world,
                                     uint32_t type_id,
                                     const void** out_dense_ptr,
                                     const int32_t** out_indices_ptr,
                                     int32_t* out_count) {
    if (!world) return 0;
    try {
        return as_world(world)->acquire_span(type_id, out_dense_ptr,
                                             out_indices_ptr, out_count) ? 1 : 0;
    } catch (...) {
        return 0;
    }
}

DF_API void df_world_release_span(df_world_handle world, uint32_t type_id) {
    if (!world) return;
    as_world(world)->release_span(type_id);
}
```

### 2.6 — Atomic commit для C++ части

```powershell
git add native/DualFrontier.Core.Native/include/df_capi.h `
        native/DualFrontier.Core.Native/include/world.h `
        native/DualFrontier.Core.Native/include/component_store.h `
        native/DualFrontier.Core.Native/src/world.cpp `
        native/DualFrontier.Core.Native/src/capi.cpp

git commit -m "native(kernel): add bulk + span operations to C ABI

Adds 4 new extern \"C\" functions to df_capi.h:
- df_world_add_components_bulk: bulk Add transmitting array in single P/Invoke
- df_world_get_components_bulk: bulk Get with deterministic output for absent entities
- df_world_acquire_span: read-only view into native dense storage
- df_world_release_span: span lifetime termination

Native side adds active_spans_ atomic counter to World. All existing
mutation paths (add_component, remove_component, destroy_entity,
flush_destroyed) check counter and throw if spans are active.

Implements §1.6 + §1.7 of KERNEL_ARCHITECTURE.md verbatim. No
architectural decisions taken in this commit.

Selftest extension follows in next commit."
```

---

## Step 3 — Native build verification (intermediate)

После C++ commits — обязательная проверка (не commit, но gate перед C# work):

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings

.\build\Release\df_native_selftest.exe
# Expected: existing 4 scenarios still ALL PASSED
# (new scenarios will be added in Step 5)
```

Если build не clean или selftest fails — STOP, regression в C++ изменениях. Investigate before proceeding to C#.

---

## Step 4 — Managed bridge extension

### 4.1 — Edit `src/DualFrontier.Core.Interop/NativeMethods.cs`

Добавить **после** existing `df_world_component_count` declaration:

```csharp
// K1 batching primitives (added 2026-05-07).

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern unsafe void df_world_add_components_bulk(
    IntPtr world,
    ulong* entities,
    uint typeId,
    void* componentData,
    int componentSize,
    int count);

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern unsafe int df_world_get_components_bulk(
    IntPtr world,
    ulong* entities,
    uint typeId,
    void* outData,
    int componentSize,
    int count);

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern unsafe int df_world_acquire_span(
    IntPtr world,
    uint typeId,
    void** outDensePtr,
    int** outIndicesPtr,
    int* outCount);

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern void df_world_release_span(IntPtr world, uint typeId);
```

### 4.2 — Create `src/DualFrontier.Core.Interop/SpanLease.cs`

K1 ships **skeleton SpanLease** — minimal IDisposable wrapper. Full implementation в K5.

```csharp
using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Lease of read-only access to a native component storage span.
/// Disposed lease releases the span back to native side, allowing mutation again.
/// 
/// K1 SKELETON: provides span pointer + count, no Span&lt;T&gt; wrapper yet.
/// K5 will add proper Span&lt;T&gt; / ReadOnlySpan&lt;T&gt; access methods.
/// </summary>
public sealed unsafe class SpanLease<T> : IDisposable where T : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _typeId;
    private readonly void* _densePtr;
    private readonly int* _indicesPtr;
    private readonly int _count;
    private bool _released;

    internal SpanLease(NativeWorld world, uint typeId,
                       void* densePtr, int* indicesPtr, int count)
    {
        _world = world;
        _typeId = typeId;
        _densePtr = densePtr;
        _indicesPtr = indicesPtr;
        _count = count;
        _released = false;
    }

    /// <summary>Number of components в span.</summary>
    public int Count => _count;

    /// <summary>
    /// Read-only span over the dense component data.
    /// Valid until Dispose() is called.
    /// </summary>
    public ReadOnlySpan<T> Span
    {
        get
        {
            if (_released) throw new ObjectDisposedException(nameof(SpanLease<T>));
            return new ReadOnlySpan<T>(_densePtr, _count);
        }
    }

    /// <summary>
    /// Read-only span over entity indices (parallel to Span).
    /// indices[i] is the entity index for span[i].
    /// </summary>
    public ReadOnlySpan<int> Indices
    {
        get
        {
            if (_released) throw new ObjectDisposedException(nameof(SpanLease<T>));
            return new ReadOnlySpan<int>(_indicesPtr, _count);
        }
    }

    public void Dispose()
    {
        if (_released) return;
        NativeMethods.df_world_release_span(_world.HandleForInternalUse, _typeId);
        _released = true;
    }
}
```

### 4.3 — Edit `src/DualFrontier.Core.Interop/NativeWorld.cs`

Добавить **internal handle accessor** для use в `SpanLease`:

```csharp
/// <summary>
/// Internal handle access for SpanLease lifetime management.
/// NOT for public consumption — use AcquireSpan instead.
/// </summary>
internal IntPtr HandleForInternalUse => _handle;
```

Добавить **public methods** (after existing `RemoveComponent<T>`):

```csharp
/// <summary>
/// Bulk add: transmits array of entities + components in a single P/Invoke crossing.
/// Eliminates per-entity overhead for batch initialization scenarios.
/// </summary>
/// <typeparam name="T">Unmanaged component type.</typeparam>
/// <param name="entities">Entities to add components to. Length must match components.</param>
/// <param name="components">Component values. Length must match entities.</param>
/// <exception cref="ArgumentException">If lengths mismatch.</exception>
/// <exception cref="InvalidOperationException">If any span is currently active.</exception>
public unsafe void AddComponents<T>(ReadOnlySpan<EntityId> entities,
                                     ReadOnlySpan<T> components) where T : unmanaged
{
    ThrowIfDisposed();
    if (entities.Length != components.Length)
        throw new ArgumentException(
            $"Mismatched lengths: {entities.Length} entities, {components.Length} components");
    if (entities.Length == 0) return;

    uint typeId = NativeComponentType<T>.TypeId;
    int size = NativeComponentType<T>.Size;

    // Pack EntityId span to ulong span on stack (or stackalloc heap fallback).
    Span<ulong> packed = entities.Length <= 256
        ? stackalloc ulong[entities.Length]
        : new ulong[entities.Length];
    for (int i = 0; i < entities.Length; i++)
    {
        packed[i] = EntityIdPacking.Pack(entities[i]);
    }

    fixed (ulong* entitiesPtr = packed)
    fixed (T* componentsPtr = components)
    {
        NativeMethods.df_world_add_components_bulk(
            _handle, entitiesPtr, typeId, componentsPtr, size, entities.Length);
    }
}

/// <summary>
/// Bulk get: reads components for an array of entities in a single P/Invoke.
/// Returns count of successfully read components. Absent entities/components
/// produce zero-filled output slots.
/// </summary>
public unsafe int GetComponents<T>(ReadOnlySpan<EntityId> entities,
                                    Span<T> output) where T : unmanaged
{
    ThrowIfDisposed();
    if (entities.Length != output.Length)
        throw new ArgumentException(
            $"Mismatched lengths: {entities.Length} entities, {output.Length} output");
    if (entities.Length == 0) return 0;

    uint typeId = NativeComponentType<T>.TypeId;
    int size = NativeComponentType<T>.Size;

    Span<ulong> packed = entities.Length <= 256
        ? stackalloc ulong[entities.Length]
        : new ulong[entities.Length];
    for (int i = 0; i < entities.Length; i++)
    {
        packed[i] = EntityIdPacking.Pack(entities[i]);
    }

    fixed (ulong* entitiesPtr = packed)
    fixed (T* outputPtr = output)
    {
        return NativeMethods.df_world_get_components_bulk(
            _handle, entitiesPtr, typeId, outputPtr, size, entities.Length);
    }
}

/// <summary>
/// Acquires a read-only span lease over native dense component storage for type T.
/// 
/// Lifetime contract:
/// - While ANY SpanLease is active, mutations (Add/Remove/Destroy) throw.
/// - Caller MUST Dispose the lease before mutating.
/// - Multiple concurrent leases allowed (different OR same type).
/// 
/// K1 SKELETON: provides Span&lt;T&gt; access via SpanLease.
/// K5 will add advanced patterns (paired iteration helpers, lease pooling).
/// </summary>
public unsafe SpanLease<T> AcquireSpan<T>() where T : unmanaged
{
    ThrowIfDisposed();

    uint typeId = NativeComponentType<T>.TypeId;
    void* densePtr;
    int* indicesPtr;
    int count;

    int result = NativeMethods.df_world_acquire_span(
        _handle, typeId, &densePtr, &indicesPtr, &count);

    if (result == 0)
    {
        throw new InvalidOperationException(
            $"Failed to acquire span for component type {typeof(T).Name}");
    }

    return new SpanLease<T>(this, typeId, densePtr, indicesPtr, count);
}
```

### 4.4 — Atomic commit для managed bridge

```powershell
git add src/DualFrontier.Core.Interop/NativeMethods.cs `
        src/DualFrontier.Core.Interop/NativeWorld.cs `
        src/DualFrontier.Core.Interop/SpanLease.cs

git commit -m "interop(kernel): managed bridge for K1 batching primitives

Adds to NativeMethods.cs: 4 new P/Invoke declarations matching
df_capi.h K1 additions (Cdecl convention, unsafe pointers).

Adds to NativeWorld.cs:
- AddComponents<T>(ReadOnlySpan<EntityId>, ReadOnlySpan<T>) — bulk add
- GetComponents<T>(ReadOnlySpan<EntityId>, Span<T>) — bulk get
- AcquireSpan<T>() returning SpanLease<T> — read-only span lifetime

Adds SpanLease.cs as IDisposable wrapper. K1 SKELETON — provides
ReadOnlySpan<T> + ReadOnlySpan<int> indices access. K5 will extend
with advanced patterns (lease pooling, paired iteration helpers).

Implements §1.6 + §1.7 of KERNEL_ARCHITECTURE.md verbatim."
```

---

## Step 5 — Selftest extension

### 5.1 — Edit `native/DualFrontier.Core.Native/test/selftest.cpp`

Добавить **2 новых scenarios** перед `int main()`:

```cpp
void scenario_bulk_operations() {
    std::printf("scenario_bulk_operations\n");
    df_world_handle w = df_world_create();

    // Create 100 entities
    constexpr int kCount = 100;
    uint64_t entities[kCount];
    BenchHealth components[kCount];
    for (int i = 0; i < kCount; ++i) {
        entities[i] = df_world_create_entity(w);
        components[i] = BenchHealth{i * 10, 100};
    }

    // Bulk add — single C ABI call
    df_world_add_components_bulk(w, entities, kHealthTypeId,
                                 components, sizeof(BenchHealth), kCount);

    DF_CHECK(df_world_component_count(w, kHealthTypeId) == kCount,
             "all bulk-added components present");

    // Bulk get
    BenchHealth read_back[kCount];
    int32_t successful = df_world_get_components_bulk(
        w, entities, kHealthTypeId, read_back, sizeof(BenchHealth), kCount);

    DF_CHECK(successful == kCount,
             "bulk get returned all components");

    for (int i = 0; i < kCount; ++i) {
        DF_CHECK(read_back[i].current == i * 10,
                 "bulk get value matches bulk add");
    }

    // Bulk get with mixed alive/dead — destroy half, flush, get all
    for (int i = 0; i < kCount; i += 2) {
        df_world_destroy_entity(w, entities[i]);
    }
    df_world_flush_destroyed(w);

    int32_t partial = df_world_get_components_bulk(
        w, entities, kHealthTypeId, read_back, sizeof(BenchHealth), kCount);

    DF_CHECK(partial == kCount / 2,
             "bulk get on mixed alive/dead returns alive count");

    df_world_destroy(w);
}

void scenario_span_lifetime() {
    std::printf("scenario_span_lifetime\n");
    df_world_handle w = df_world_create();

    // Setup: 10 entities with components
    uint64_t entities[10];
    for (int i = 0; i < 10; ++i) {
        entities[i] = df_world_create_entity(w);
        BenchHealth h{i, 100};
        df_world_add_component(w, entities[i], kHealthTypeId, &h, sizeof(h));
    }

    // Acquire span
    const void* dense_ptr = nullptr;
    const int32_t* indices_ptr = nullptr;
    int32_t span_count = 0;

    int32_t acq = df_world_acquire_span(
        w, kHealthTypeId, &dense_ptr, &indices_ptr, &span_count);

    DF_CHECK(acq == 1, "span acquisition succeeded");
    DF_CHECK(span_count == 10, "span count matches entity count");
    DF_CHECK(dense_ptr != nullptr, "dense pointer is non-null");
    DF_CHECK(indices_ptr != nullptr, "indices pointer is non-null");

    // Read через span (no further P/Invokes, direct memory access)
    const BenchHealth* dense = static_cast<const BenchHealth*>(dense_ptr);
    int sum = 0;
    for (int i = 0; i < span_count; ++i) {
        sum += dense[i].current;
    }
    DF_CHECK(sum == 45, "span iteration sum matches expected (0+1+...+9)");

    // Mutation while span active — must fail (silently, returns code)
    BenchHealth h_extra{999, 100};
    uint64_t e_extra = df_world_create_entity(w);  // ok, this is allowed
    df_world_add_component(w, e_extra, kHealthTypeId, &h_extra, sizeof(h_extra));
    // ^ This should be silently rejected (capi catches exception)

    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 10,
             "mutation rejected while span active");

    // Release span
    df_world_release_span(w, kHealthTypeId);

    // Now mutation should succeed
    df_world_add_component(w, e_extra, kHealthTypeId, &h_extra, sizeof(h_extra));
    DF_CHECK(df_world_component_count(w, kHealthTypeId) == 11,
             "mutation succeeds after span released");

    df_world_destroy(w);
}
```

И в `main()`, **после** `scenario_throughput()`:
```cpp
    scenario_bulk_operations();
    scenario_span_lifetime();
```

### 5.2 — Build + verify

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
.\build\Release\df_native_selftest.exe
# Expected: 6 scenarios, ALL PASSED
```

### 5.3 — Atomic commit

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git add native/DualFrontier.Core.Native/test/selftest.cpp
git commit -m "test(native): K1 selftest scenarios for bulk + span

Adds 2 new selftest scenarios:
- scenario_bulk_operations: validates bulk add/get correctness,
  including mixed alive/dead entity handling.
- scenario_span_lifetime: validates span acquisition, direct memory
  access, mutation rejection while active, mutation re-enabled
  after release.

Total selftest scenarios: 4 -> 6. All passing."
```

---

## Step 6 — Benchmark extension

### 6.1 — Create `tests/DualFrontier.Core.Benchmarks/NativeBulkAddBenchmark.cs`

```csharp
using BenchmarkDotNet.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// K1 benchmark: validates bulk-add hypothesis.
/// 
/// Baseline (from Discovery report):
/// - ManagedAdd10k: 218 μs (with 655 KB allocations)
/// - NativeAdd10k:  399 μs (with 24 B allocations) — per-entity P/Invoke overhead
/// 
/// Target for K1:
/// - NativeBulkAdd10k: ≤200 μs (single P/Invoke for entire batch)
/// 
/// If achieved: bulk transmission validates the §8 hypothesis quantitatively
/// — native eliminates GC pressure AND restores throughput parity.
/// </summary>
[MemoryDiagnoser]
public class NativeBulkAddBenchmark
{
    private const int EntityCount = 10_000;
    private NativeWorld _world = null!;
    private EntityId[] _entities = null!;
    private BenchHealthComponent[] _components = null!;

    [GlobalSetup]
    public void Setup()
    {
        _world = new NativeWorld();
        _entities = new EntityId[EntityCount];
        _components = new BenchHealthComponent[EntityCount];

        for (int i = 0; i < EntityCount; i++)
        {
            _entities[i] = _world.CreateEntity();
            _components[i] = new BenchHealthComponent { Current = i, Maximum = 100 };
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _world.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void NativeAdd10k_PerEntity()
    {
        // Existing path: 10,000 P/Invoke crossings
        for (int i = 0; i < EntityCount; i++)
        {
            _world.AddComponent(_entities[i], _components[i]);
        }
    }

    [Benchmark]
    public void NativeBulkAdd10k_SinglePInvoke()
    {
        // K1 path: 1 P/Invoke crossing для всего batch
        _world.AddComponents<BenchHealthComponent>(_entities, _components);
    }
}
```

### 6.2 — Atomic commit

```powershell
git add tests/DualFrontier.Core.Benchmarks/NativeBulkAddBenchmark.cs
git commit -m "bench(kernel): K1 NativeBulkAddBenchmark

Validates bulk-transmission hypothesis quantitatively.

Baseline: NativeAdd10k_PerEntity = 10,000 P/Invoke crossings.
Target:   NativeBulkAdd10k_SinglePInvoke = 1 P/Invoke crossing.

Discovery report (CPP_KERNEL_BRANCH_REPORT.md §10.4) recorded:
- NativeAdd10k: 399 μs (per-entity overhead dominant)
- ManagedAdd10k: 218 μs (with 655 KB allocations)

K1 hypothesis: bulk path completes ≤200 μs.

Benchmark execution deferred к K7 (representative load measurement).
This commit ships the benchmark code only — execution + analysis is
a separate concern."
```

---

## Step 7 — Managed build + test verification

```powershell
dotnet build
# Expected: 0 errors, 0 warnings (or only pre-existing XML doc warnings)

dotnet test
# Expected: 472 passing, 0 failed
# K1 does NOT add tests to DualFrontier.Core.Interop — that's K2 scope.
# Existing 472 baseline must be preserved.
```

Если число тестов != 472 — STOP, K1 нарушил invariant. Investigate.

---

## Step 8 — Update MIGRATION_PROGRESS.md

Открыть `docs/MIGRATION_PROGRESS.md`. Найти таблицу K-series progress.

Заменить K1 row:
```markdown
| K1 | Batching primitive (bulk Add/Get + Span<T>) | DONE | 3–5 days | <last-K1-commit-sha> | 2026-MM-DD |
```

Обновить `Current state snapshot`:
```markdown
| **Active phase** | K2 (planned) — type-id registry + bridge tests |
| **Last completed milestone** | K1 (batching primitive) — `<sha>` 2026-MM-DD |
| **Next milestone (recommended)** | K2 (type-id registry + bridge tests) |
```

Обновить `Last updated`:
```markdown
**Last updated**: 2026-MM-DD (K1 closure)
```

Добавить в детальную секцию (после K0):
```markdown
### K1 — Batching primitive (bulk Add/Get + Span<T>)

- **Status**: DONE (`<commit-sha>`, 2026-MM-DD)
- **Brief**: `tools/briefs/K1_BATCHING_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 4 new functions — add_components_bulk, get_components_bulk,
  acquire_span, release_span (12 → 16 total)
- **Native side**: active_spans_ atomic counter в World; mutation rejection
  при active spans; dense_data() accessor в RawComponentStore
- **Managed bridge**: NativeWorld.AddComponents/GetComponents/AcquireSpan;
  SpanLease<T> skeleton с ReadOnlySpan<T>/ReadOnlySpan<int> access
- **Selftest scenarios**: 4 → 6 (added scenario_bulk_operations,
  scenario_span_lifetime)
- **Benchmark**: NativeBulkAddBenchmark added (execution deferred к K7)
- **Managed tests**: 472 passing (preserved baseline)
- **Lessons learned**: <fill if non-trivial issues encountered>
```

Атомарный commit:
```powershell
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(migration): K1 closure recorded"
```

---

## Step 9 — Update K1 brief skeleton

Open `tools/briefs/K1_BATCHING_BRIEF.md`, replace TODO list с:

```markdown
## Status: EXECUTED

**Date**: 2026-MM-DD
**Branch**: feat/k1-batching-primitive
**Final commit**: <sha>

Full executable brief authored and executed. See git log on
feat/k1-batching-primitive branch for atomic commit sequence.

See `MIGRATION_PROGRESS.md` for closure record.
```

```powershell
git add tools/briefs/K1_BATCHING_BRIEF.md
git commit -m "docs(briefs): K1 brief skeleton marked EXECUTED"
```

---

## Step 10 — Final verification & merge prep

### Branch state check

```powershell
git log --oneline main..HEAD
# Expected sequence (6-7 commits):
#   <sha> docs(briefs): K1 brief skeleton marked EXECUTED
#   <sha> docs(migration): K1 closure recorded
#   <sha> bench(kernel): K1 NativeBulkAddBenchmark
#   <sha> test(native): K1 selftest scenarios for bulk + span
#   <sha> interop(kernel): managed bridge for K1 batching primitives
#   <sha> native(kernel): add bulk + span operations to C ABI
```

### Final builds

```powershell
git status                                  # clean
cmake --build native\DualFrontier.Core.Native\build --config Release
.\native\DualFrontier.Core.Native\build\Release\df_native_selftest.exe
# Expected: 6 scenarios, ALL PASSED

dotnet build
dotnet test
# Expected: 472 passing
```

Все четыре должны пройти clean.

### Push

```powershell
git push -u origin feat/k1-batching-primitive
```

PR-ready для review/merge.

---

## Acceptance criteria

K1 закрыт когда ВСЕ выполнено:

- [ ] Branch `feat/k1-batching-primitive` создан от `main`
- [ ] Native commit: 4 новых extern "C" функций в `df_capi.h`
- [ ] Native commit: `World::active_spans_` counter + mutation rejection logic
- [ ] Native commit: `RawComponentStore::dense_data()` accessor
- [ ] Interop commit: 4 новых `[DllImport]` в NativeMethods.cs
- [ ] Interop commit: `NativeWorld.AddComponents/GetComponents/AcquireSpan` методы
- [ ] Interop commit: `SpanLease<T>` skeleton с ReadOnlySpan access
- [ ] Selftest commit: scenario_bulk_operations + scenario_span_lifetime
- [ ] Benchmark commit: NativeBulkAddBenchmark.cs
- [ ] `cmake --build` clean — 0 errors, 0 warnings
- [ ] Native selftest: **6 scenarios ALL PASSED**
- [ ] `dotnet build` clean
- [ ] `dotnet test` — **472 passing** (baseline preserved)
- [ ] MIGRATION_PROGRESS.md K1 row DONE с commit SHA
- [ ] tools/briefs/K1_BATCHING_BRIEF.md marked EXECUTED
- [ ] Branch pushed to origin
- [ ] No build artifacts committed

---

## Rollback procedure

K1 не делает destructive changes — main untouched until merge. Rollback:

```powershell
git cherry-pick --abort                     # if mid-cherry-pick (unlikely в K1)
git checkout main
git branch -D feat/k1-batching-primitive
# Origin remains untouched (push только в Step 10)
```

---

## Open issues / lessons learned (заполнить при closure)

<empty — заполнить если в процессе обнаружилось что-то нетривиальное>

---

## Pipeline metadata

- **Brief authored by**: Opus (architect)
- **Brief executed by**: Claude Code agent или human (на основании этого brief)
- **Final review**: Crystalka (architectural judgment + commit author)
- **Methodology compliance**: descriptive pre-flight (METHODOLOGY.md v1.2),
  atomic commits, scope prefixes per existing convention

**Brief end. Companion docs: KERNEL_ARCHITECTURE.md (§1.6, §1.7, §K1), MIGRATION_PROGRESS.md.**
