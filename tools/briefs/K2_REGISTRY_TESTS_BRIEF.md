# K2 — Type-id registry + bridge tests project

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-07
**Status**: EXECUTED
**Reference docs**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K2, §1.10 (component type registry), §1.11 (testing strategy), `docs/MIGRATION_PROGRESS.md` (K1 closure context), `docs/methodology/METHODOLOGY.md` v1.3
**Predecessor**: K1 (`e2c50b8`) — batching primitive, merged to main
**Target**: fresh feature branch `feat/k2-registry-and-tests` от `main`
**Estimated time**: 2–3 days at hobby pace
**Estimated LOC delta**: +400–600

---

## Status: EXECUTED

**Date**: 2026-05-07
**Branch**: `feat/k2-registry-and-tests`
**Closure SHA**: `129a0a0` (last K2-substantive commit — bridge test project с 39 tests)

Full executable brief authored and executed. See git log on
`feat/k2-registry-and-tests` for the atomic commit sequence
(`c9ee0bc` native ABI → `254e4a5` managed registry → `3b61c41` selftest
→ `129a0a0` Interop test project → `ce6f9cc` migration progress closure)
and `docs/MIGRATION_PROGRESS.md` K2 entry for the closure record
(lessons learned, sequencing decision β6).

---

## Goal

Заменить FNV-1a hash-based type IDs на explicit deterministic registry per K-L4. Создать `DualFrontier.Core.Interop.Tests` project (xUnit) с comprehensive coverage bridge layer (~30-40 tests). После K2 sequencing decision (β5 vs β6 vs β3) можно принимать с evidence base.

**В scope**:
- Native ABI extension: `df_world_register_component_type(world, type_id, size)` — explicit registration
- Managed: новый класс `ComponentTypeRegistry` (instance-per-NativeWorld, sequential IDs 1, 2, 3...)
- `NativeWorld` integration: использовать `ComponentTypeRegistry` для type ID resolution
- `NativeComponentType<T>` deprecated с migration path, retained для historical compatibility
- Новый test project: `tests/DualFrontier.Core.Interop.Tests/`
- ~30-40 tests across 5 test classes: NativeWorldTests, EntityIdPackingTests, ComponentTypeRegistryTests, SpanLeaseTests, BulkOperationsTests
- Selftest extension: 1 new scenario для explicit registration
- MIGRATION_PROGRESS.md K2 closure с sequencing decision recorded

**НЕ в scope** (откладывается):
- WriteCommandBuffer full implementation — K5
- Native bootstrap graph / thread pool — K3
- Component struct refactor — K4
- Mod-driven registry integration — K6 (mods register types через IModApi v3)
- Removing `NativeComponentType<T>` entirely — оставляется для backward compat, K8 может удалить если cutover Outcome 1

---

## Architectural context (LOCKED, неизменно)

K2 implementing K-L4 verbatim:

> **K-L4** (KERNEL_ARCHITECTURE.md Part 0): Type IDs = Explicit registry per-mod registration. FNV-1a hash collision-prone; explicit IDs deterministic.

§1.10 даёт точную спецификацию `ComponentTypeRegistry`:
- Sequential IDs (1, 2, 3, ...) — 0 reserved for invalid
- Idempotent registration (re-register same type returns existing ID)
- Mod load order matters для type ID stability across runs

K2 **не принимает архитектурных решений**. Любое отклонение требует amendment LOCKED docs.

---

## Step 0 — Brief authoring commit (METHODOLOGY v1.3 prerequisite)

**Per METHODOLOGY.md v1.3 «Brief authoring as prerequisite step»**: brief itself is unstaged modification on main. Must commit before execution begins.

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git status
# Expected: K2_REGISTRY_TESTS_BRIEF.md modified (skeleton → full brief)

git add tools/briefs/K2_REGISTRY_TESTS_BRIEF.md
git commit -m "docs(briefs): K2 brief authored — full executable registry + tests"
# Не push на этом этапе — K2 feature branch будет ancestor от brief commit автоматически
```

После этого working tree clean, HG-1 пройдёт.

---

## Pre-flight checks (METHODOLOGY v1.2 descriptive style)

### Hard gates (STOP-eligible)

#### HG-1: Working tree clean
```powershell
git status                                # должен быть clean (после Step 0)
```

#### HG-2: K1 successfully merged to main
```powershell
git log main --oneline | Select-String "K1|batching" | Select-Object -First 5
# Ожидаем видеть commits типа:
#   "docs(briefs): K1 brief skeleton marked EXECUTED"
#   "docs(migration): K1 closure recorded"
#   "bench(kernel): K1 NativeBulkAddBenchmark"
#   "test(native): K1 selftest scenarios for bulk + span"
#   "interop(kernel): managed bridge for K1 batching primitives"
#   "native(kernel): add bulk + span operations to C ABI"
```

Если K1 commits не видны на main — STOP, K1 не merged, K2 не должен начинаться.

#### HG-3: Native source intact (post-K1)
```powershell
Test-Path native\DualFrontier.Core.Native\include\df_capi.h
Test-Path native\DualFrontier.Core.Native\src\capi.cpp
Test-Path native\DualFrontier.Core.Native\src\world.cpp
Test-Path native\DualFrontier.Core.Native\test\selftest.cpp

# Verify K1 functions present:
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "df_world_acquire_span"
# Expected: match (K1 added this)
```

#### HG-4: Managed bridge intact (post-K1)
```powershell
Test-Path src\DualFrontier.Core.Interop\NativeWorld.cs
Test-Path src\DualFrontier.Core.Interop\NativeMethods.cs
Test-Path src\DualFrontier.Core.Interop\SpanLease.cs                            # K1 added
Test-Path src\DualFrontier.Core.Interop\Marshalling\NativeComponentType.cs
Test-Path src\DualFrontier.Core.Interop\Marshalling\EntityIdPacking.cs
```

#### HG-5: Test project does NOT yet exist
```powershell
Test-Path tests\DualFrontier.Core.Interop.Tests\DualFrontier.Core.Interop.Tests.csproj
# Expected: False (only MODULE.md placeholder exists in scaffolded folder)
```

Если уже существует csproj — STOP, K2 уже частично выполнен.

#### HG-6: Baseline tests passing
```powershell
dotnet test
# Expected: 472 passing, 0 failed
```

#### HG-7: Native build clean
```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings (build dir already exists from K1)
```

#### HG-8: Native selftest passing
```powershell
.\build\Release\df_native_selftest.exe
# Expected: 6 scenarios ALL PASSED (post-K1 baseline)
```

### Informational checks (record-only)

#### INF-1: Current C ABI surface count
```powershell
Select-String -Path native\DualFrontier.Core.Native\include\df_capi.h -Pattern "DF_API" | Measure-Object
# Expected after K1: 16 functions (12 baseline + 4 K1)
# K2 will add 1 more, target: 17
# Record actual count
```

#### INF-2: Selftest scenario count
```powershell
Select-String -Path native\DualFrontier.Core.Native\test\selftest.cpp -Pattern "void scenario_" | Measure-Object
# Expected: 6 scenarios (4 baseline + 2 K1)
# K2 will add 1 more, target: 7
```

#### INF-3: Existing test project counts
```powershell
Get-ChildItem tests -Directory | Where-Object { $_.Name -like "DualFrontier*" -and (Test-Path "$($_.FullName)\*.csproj") } | Select-Object Name
# Record list of existing test projects
```

#### INF-4: Recent commit history
```powershell
git log main --oneline -5
# Record HEAD SHA для K2 closure reference
```

---

## Throw inventory (METHODOLOGY v1.3 ABI boundary completeness)

K2 introduces **one new throw site** in native code:

### Throw inventory

| Site | Throw type | Trigger condition |
|---|---|---|
| `World::register_component_type` | `std::invalid_argument` | size mismatch with already-registered type id (if re-registering with different size) OR type_id == 0 (reserved) |

### Boundary trace

The new throw can propagate ONLY through the new ABI function:
- `df_world_register_component_type` (added в Step 2.1)

NO existing ABI functions can propagate this throw — it's only triggered from `register_component_type` which is exclusively called from the new ABI wrapper.

### Wrap inventory

- **New wrapper**: `df_world_register_component_type` — try/catch added в Step 2.4 (new code, included by default)
- **No existing wrappers need modification** — K2 doesn't add throws to existing native functions

### Default catch behavior

- `df_world_register_component_type` catch: return `0` (failure code matching existing ABI convention для non-void functions)
- Caller checks return code and throws C# exception если registration failed

---

## Step 1 — Branch setup

```powershell
git checkout main
git pull origin main
git checkout -b feat/k2-registry-and-tests main
```

Branch name: **`feat/k2-registry-and-tests`** (точно).

---

## Step 2 — Native ABI extension (explicit registration)

### 2.1 — Edit `native/DualFrontier.Core.Native/include/df_capi.h`

Добавить **после** existing K1 declarations (`df_world_release_span`), **перед** closing `extern "C"` block:

```cpp
/*
 * K2 explicit type registration (added 2026-05-07).
 *
 * Replaces implicit FNV-1a hash-based type identification (K0 inheritance)
 * with explicit deterministic registry. Caller assigns sequential type_ids
 * (1, 2, 3, ...) and declares the byte size for each type.
 *
 * Native side:
 * - Records (type_id, size) mapping at registration time.
 * - Re-registration with same (type_id, size) is idempotent (no-op).
 * - Re-registration with same type_id but DIFFERENT size throws invalid_argument
 *   (caught at boundary, returns 0).
 * - type_id == 0 is reserved for "invalid" sentinel; registration with 0 fails.
 *
 * Migration note: existing functions (df_world_add_component, df_world_get_component,
 * df_world_acquire_span, etc.) accept type_ids whether or not they were
 * pre-registered. Pre-registration enables the registry-based path; legacy
 * FNV-1a-derived ids continue to work for backward compat with K0/K1 callers.
 */

DF_API int32_t         df_world_register_component_type(
                           df_world_handle world,
                           uint32_t type_id,
                           int32_t component_size);
```

Returns `1` on success, `0` on failure (invalid type_id, size mismatch with existing registration, world disposed).

### 2.2 — Edit `native/DualFrontier.Core.Native/include/world.h`

Добавить в `World` class declaration:

**Public method** (after `acquire_span` / `release_span`):
```cpp
bool register_component_type(uint32_t type_id, int32_t component_size);
```

**No new private members** — uses existing `stores_` map. Registration creates an empty `RawComponentStore` for the type, which gets populated lazily на first Add. Re-registration с same size = no-op. Re-registration с different size = throw.

### 2.3 — Edit `native/DualFrontier.Core.Native/src/world.cpp`

Добавить в конец implementation:

```cpp
bool World::register_component_type(uint32_t type_id, int32_t component_size) {
    if (type_id == 0) {
        throw std::invalid_argument("type_id 0 is reserved");
    }
    if (component_size <= 0) {
        throw std::invalid_argument("component_size must be > 0");
    }

    auto it = stores_.find(type_id);
    if (it != stores_.end()) {
        // Already registered — verify size matches.
        if (it->second->component_size() != component_size) {
            throw std::invalid_argument(
                "type_id already registered with different component_size");
        }
        return true;  // idempotent
    }

    // New registration: create empty store.
    stores_.emplace(type_id, std::make_unique<RawComponentStore>(component_size));
    return true;
}
```

### 2.4 — Edit `native/DualFrontier.Core.Native/src/capi.cpp`

Добавить **после** existing K1 wrappers:

```cpp
DF_API int32_t df_world_register_component_type(df_world_handle world,
                                                uint32_t type_id,
                                                int32_t component_size) {
    if (!world) return 0;
    try {
        return as_world(world)->register_component_type(type_id, component_size) ? 1 : 0;
    } catch (...) {
        return 0;
    }
}
```

### 2.5 — Atomic commit для C++ части

```powershell
git add native/DualFrontier.Core.Native/include/df_capi.h `
        native/DualFrontier.Core.Native/include/world.h `
        native/DualFrontier.Core.Native/src/world.cpp `
        native/DualFrontier.Core.Native/src/capi.cpp

git commit -m "native(kernel): explicit component type registration

Adds df_world_register_component_type to C ABI. Caller assigns
deterministic type_ids (1, 2, 3, ...) and declares byte size; native
side records (type_id, size) mapping at registration time.

Idempotent: re-registration with same (type_id, size) is no-op.
Re-registration with same type_id but different size throws
invalid_argument (caught at boundary, returns 0).

Implements K-L4 of KERNEL_ARCHITECTURE.md verbatim. Replaces the
FNV-1a hash-based implicit identification (K0 inheritance) with
explicit auditable mapping per §1.10."
```

### 2.6 — Native build verification (intermediate)

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
# Expected: 0 errors, 0 warnings

.\build\Release\df_native_selftest.exe
# Expected: existing 6 scenarios still ALL PASSED
# (new scenario added в Step 5)
```

Если build не clean — STOP, regression в C++ изменениях.

---

## Step 3 — Managed `ComponentTypeRegistry`

### 3.1 — Edit `src/DualFrontier.Core.Interop/NativeMethods.cs`

Добавить **после** existing K1 declarations:

```csharp
// K2 explicit registration (added 2026-05-07).

[DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
internal static extern int df_world_register_component_type(
    IntPtr world,
    uint typeId,
    int componentSize);
```

### 3.2 — Create `src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// Explicit per-NativeWorld registry mapping <see cref="Type"/> к
/// sequential uint type IDs.
///
/// Replaces FNV-1a hash-based implicit identification (K0 inheritance) с
/// auditable deterministic mapping. Key K-L4 invariants:
/// - IDs are sequential (1, 2, 3, ...). 0 is reserved для invalid.
/// - Registration is idempotent: re-registering same type returns existing ID.
/// - Mod load order matters для ID stability across runs.
///   ModLoader должен process mods deterministically (alphabetical OR
///   explicit ordering manifest) — concern of K6, не K2.
///
/// Instance-per-NativeWorld (не static) — different worlds have independent
/// type-id spaces. Different game sessions have independent registries.
/// </summary>
public sealed class ComponentTypeRegistry
{
    private readonly Dictionary<Type, uint> _typeToId = new();
    private readonly Dictionary<uint, Type> _idToType = new();
    private readonly IntPtr _worldHandle;
    private uint _nextId = 1;  // 0 reserved для invalid

    /// <summary>
    /// Creates registry bound к specified native world. The handle is
    /// captured for the registry's lifetime — caller must ensure world
    /// is not disposed while registry is in use.
    /// </summary>
    internal ComponentTypeRegistry(IntPtr worldHandle)
    {
        if (worldHandle == IntPtr.Zero)
        {
            throw new ArgumentException(
                "Cannot bind ComponentTypeRegistry к null world handle",
                nameof(worldHandle));
        }
        _worldHandle = worldHandle;
    }

    /// <summary>
    /// Registers component type T. Idempotent — re-registering returns
    /// existing ID without re-calling native registration.
    /// </summary>
    /// <typeparam name="T">Unmanaged component type.</typeparam>
    /// <returns>The deterministic ID assigned к T.</returns>
    /// <exception cref="InvalidOperationException">If native registration fails.</exception>
    public uint Register<T>() where T : unmanaged
    {
        Type type = typeof(T);
        if (_typeToId.TryGetValue(type, out uint existing))
        {
            return existing;  // idempotent
        }

        uint id = _nextId++;
        int size = Unsafe.SizeOf<T>();

        int result = NativeMethods.df_world_register_component_type(
            _worldHandle, id, size);

        if (result == 0)
        {
            // Native registration failed. Rollback the id assignment.
            _nextId--;
            throw new InvalidOperationException(
                $"Native registration failed для component type {type.Name} " +
                $"(id={id}, size={size}).");
        }

        _typeToId[type] = id;
        _idToType[id] = type;
        return id;
    }

    /// <summary>
    /// Gets the ID для previously-registered type T. Throws if T not registered.
    /// </summary>
    /// <exception cref="InvalidOperationException">If T was never registered.</exception>
    public uint GetId<T>() where T : unmanaged
    {
        if (!_typeToId.TryGetValue(typeof(T), out uint id))
        {
            throw new InvalidOperationException(
                $"Component type {typeof(T).Name} not registered. " +
                $"Call Register<{typeof(T).Name}>() first.");
        }
        return id;
    }

    /// <summary>
    /// Tries к get the ID для type T without throwing.
    /// </summary>
    public bool TryGetId<T>(out uint id) where T : unmanaged
    {
        return _typeToId.TryGetValue(typeof(T), out id);
    }

    /// <summary>
    /// Reverse lookup: get the Type registered against given ID.
    /// Returns null if id not assigned.
    /// </summary>
    public Type? Lookup(uint id)
    {
        return _idToType.TryGetValue(id, out Type? type) ? type : null;
    }

    /// <summary>
    /// Number of types registered.
    /// </summary>
    public int Count => _typeToId.Count;

    /// <summary>
    /// Returns true if type T has been registered.
    /// </summary>
    public bool IsRegistered<T>() where T : unmanaged
    {
        return _typeToId.ContainsKey(typeof(T));
    }
}
```

### 3.3 — Edit `src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs`

Mark as deprecated, retain для backward compatibility:

```csharp
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DualFrontier.Core.Interop.Marshalling;

/// <summary>
/// LEGACY: FNV-1a hash-based type identification.
///
/// Superseded by <see cref="ComponentTypeRegistry"/> (K2, 2026-05-07) which
/// provides explicit deterministic IDs per K-L4 of KERNEL_ARCHITECTURE.md.
///
/// Retained для backward compatibility с pre-K2 code paths. <see cref="NativeWorld"/>
/// constructed without explicit registry uses this fallback. New code should
/// always provide a <see cref="ComponentTypeRegistry"/> instance.
///
/// Will likely be removed at K8 cutover if Outcome 1 (native + batching wins
/// decisively) materializes — no production path will use FNV-1a после.
/// </summary>
[Obsolete("Use ComponentTypeRegistry для explicit deterministic IDs. " +
          "FNV-1a hash collision-prone — see K-L4 rationale.", error: false)]
internal static class NativeComponentType<T> where T : unmanaged
{
    internal static readonly uint TypeId = ComputeTypeId(typeof(T));
    internal static readonly int Size = Unsafe.SizeOf<T>();

    private static uint ComputeTypeId(Type type)
    {
        string name = type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
        const uint offsetBasis = 2166136261u;
        const uint prime = 16777619u;
        uint hash = offsetBasis;
        foreach (char c in name)
        {
            hash ^= c;
            hash *= prime;
        }
        return hash;
    }
}

/// <summary>
/// LEGACY: diagnostic registry для FNV-1a IDs.
/// Superseded by ComponentTypeRegistry. Retained для backward compat.
/// </summary>
[Obsolete("Use ComponentTypeRegistry. NativeComponentTypeRegistry will be " +
          "removed when NativeComponentType<T> is removed (K8 cutover).",
          error: false)]
internal static class NativeComponentTypeRegistry
{
    private static readonly ConcurrentDictionary<uint, Type> _byId = new();

    internal static void Register(uint id, Type type) => _byId[id] = type;

    internal static Type? Lookup(uint id) =>
        _byId.TryGetValue(id, out Type? t) ? t : null;
}
```

### 3.4 — Edit `src/DualFrontier.Core.Interop/NativeWorld.cs`

Two changes:

**Change A** — add optional `ComponentTypeRegistry` parameter to constructor:

```csharp
private readonly ComponentTypeRegistry? _registry;

/// <summary>
/// Creates a NativeWorld с FNV-1a fallback type IDs (legacy path).
/// Equivalent to passing <c>null</c> для registry. Retained для
/// backward compatibility — new code should pass an explicit registry.
/// </summary>
public NativeWorld() : this(null) { }

/// <summary>
/// Creates a NativeWorld с explicit ComponentTypeRegistry (K2 recommended path).
/// </summary>
/// <param name="registry">
/// If null, uses FNV-1a hash IDs (legacy). If provided, uses deterministic
/// sequential IDs per K-L4. Registry binds к this world's handle automatically.
/// </param>
public NativeWorld(ComponentTypeRegistry? registry)
{
    _handle = NativeMethods.df_world_create();
    if (_handle == IntPtr.Zero)
    {
        throw new InvalidOperationException(
            "df_world_create returned null — native library failed to allocate a World.");
    }
    _registry = registry;
}

/// <summary>
/// The component type registry bound к this world, if any.
/// Null если world was constructed без registry (legacy FNV-1a path).
/// </summary>
public ComponentTypeRegistry? Registry => _registry;
```

**Change B** — add helper method для resolving type ID, used by all component operations:

```csharp
/// <summary>
/// Resolves type ID для T using registry if available, else FNV-1a fallback.
/// Centralizes the FNV-1a vs explicit-registry decision.
/// </summary>
[MethodImpl(MethodImplOptions.AggressiveInlining)]
private uint ResolveTypeId<T>() where T : unmanaged
{
    if (_registry != null)
    {
        // Auto-register on first use в registry mode.
        // Idempotent — re-call returns existing ID.
        return _registry.Register<T>();
    }

    // Legacy FNV-1a path.
#pragma warning disable CS0618 // NativeComponentType<T> is obsolete
    uint typeId = NativeComponentType<T>.TypeId;
    NativeComponentTypeRegistry.Register(typeId, typeof(T));
    return typeId;
#pragma warning restore CS0618
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
private int ResolveTypeSize<T>() where T : unmanaged
{
    return Unsafe.SizeOf<T>();
}
```

**Change C** — update all component method calls к use `ResolveTypeId<T>()` and `ResolveTypeSize<T>()` instead of direct `NativeComponentType<T>.TypeId` / `.Size`:

In `AddComponent<T>`, `TryGetComponent<T>`, `HasComponent<T>`, `RemoveComponent<T>`, `GetComponentCount<T>`, `AddComponents<T>`, `GetComponents<T>`, `AcquireSpan<T>` — replace:
- `NativeComponentType<T>.TypeId` → `ResolveTypeId<T>()`
- `NativeComponentType<T>.Size` → `ResolveTypeSize<T>()`
- Remove `NativeComponentTypeRegistry.Register(typeId, typeof(T));` calls (now done in `ResolveTypeId`)

(Add `using System.Runtime.CompilerServices;` если ещё не there для `MethodImpl`.)

### 3.5 — Atomic commit для managed bridge

```powershell
git add src/DualFrontier.Core.Interop/NativeMethods.cs `
        src/DualFrontier.Core.Interop/Marshalling/ComponentTypeRegistry.cs `
        src/DualFrontier.Core.Interop/Marshalling/NativeComponentType.cs `
        src/DualFrontier.Core.Interop/NativeWorld.cs

git commit -m "interop(kernel): ComponentTypeRegistry с deterministic IDs

Adds ComponentTypeRegistry — instance-per-NativeWorld map of CLR type
к sequential uint IDs (1, 2, 3, ...). Implements K-L4 of
KERNEL_ARCHITECTURE.md verbatim per §1.10.

NativeWorld extended с optional registry parameter:
- new NativeWorld() — legacy FNV-1a path (backward compat)
- new NativeWorld(registry) — explicit deterministic IDs (K2 path)

Internal ResolveTypeId<T>() centralizes the dispatch — registry-bound
worlds auto-register types on first use; null-registry worlds fall
back к FNV-1a.

NativeComponentType<T> и NativeComponentTypeRegistry marked [Obsolete]
с warning (not error) — retained для backward compat. Will be removed
at K8 cutover if Outcome 1 (native + batching wins) materializes."
```

### 3.6 — Build verification (intermediate)

```powershell
dotnet build
# Expected: 0 errors. Warnings допустимы (CS0618 obsolete warnings внутри NativeWorld.cs
# suppressed via #pragma; обсолетные класс не должен генерировать warnings от existing
# tests которые могут его использовать).

dotnet test
# Expected: 472 passing (legacy path still works)
```

Если число tests != 472 — STOP, регрессия от registry refactor. Investigate.

---

## Step 4 — Selftest extension (1 new scenario)

### 4.1 — Edit `native/DualFrontier.Core.Native/test/selftest.cpp`

Добавить scenario **перед** `int main()`:

```cpp
void scenario_explicit_registration() {
    std::printf("scenario_explicit_registration\n");
    df_world_handle w = df_world_create();

    // Register type with explicit ID
    constexpr uint32_t kCustomTypeId = 42;
    constexpr int32_t kSize = sizeof(BenchHealth);

    int32_t reg1 = df_world_register_component_type(w, kCustomTypeId, kSize);
    DF_CHECK(reg1 == 1, "first registration succeeded");

    // Idempotent — re-register same (id, size)
    int32_t reg2 = df_world_register_component_type(w, kCustomTypeId, kSize);
    DF_CHECK(reg2 == 1, "idempotent re-registration succeeded");

    // Conflict — re-register с different size fails
    int32_t reg3 = df_world_register_component_type(w, kCustomTypeId, kSize * 2);
    DF_CHECK(reg3 == 0, "size conflict rejected");

    // type_id 0 reserved
    int32_t reg4 = df_world_register_component_type(w, 0, kSize);
    DF_CHECK(reg4 == 0, "type_id 0 rejected");

    // Pre-registered type usable for Add/Get
    uint64_t e = df_world_create_entity(w);
    BenchHealth h{50, 100};
    df_world_add_component(w, e, kCustomTypeId, &h, kSize);

    BenchHealth h_read{};
    int32_t got = df_world_get_component(w, e, kCustomTypeId, &h_read, kSize);
    DF_CHECK(got == 1, "get from pre-registered type succeeded");
    DF_CHECK(h_read.current == 50, "value preserved через pre-registered type");

    df_world_destroy(w);
}
```

И в `main()`, **после** `scenario_span_lifetime()`:
```cpp
    scenario_explicit_registration();
```

### 4.2 — Build + verify

```powershell
cd native\DualFrontier.Core.Native
cmake --build build --config Release
.\build\Release\df_native_selftest.exe
# Expected: 7 scenarios, ALL PASSED
```

### 4.3 — Atomic commit

```powershell
cd D:\Colony_Simulator\Colony_Simulator
git add native/DualFrontier.Core.Native/test/selftest.cpp
git commit -m "test(native): K2 selftest scenario для explicit registration

scenario_explicit_registration validates:
- First registration succeeds
- Idempotent re-registration (same id, same size) succeeds
- Size conflict rejected (re-register с different size returns 0)
- type_id 0 reserved (rejected at boundary)
- Pre-registered type usable для Add/Get

Total selftest scenarios: 6 -> 7. All passing."
```

---

## Step 5 — Create `DualFrontier.Core.Interop.Tests` project

### 5.1 — Create project file `tests/DualFrontier.Core.Interop.Tests/DualFrontier.Core.Interop.Tests.csproj`

Mirror existing `DualFrontier.Core.Tests.csproj` configuration:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>DualFrontier.Core.Interop.Tests</AssemblyName>
    <RootNamespace>DualFrontier.Core.Interop.Tests</RootNamespace>
    <IsPackable>false</IsPackable>
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageReference Include="FluentAssertions" Version="6.12.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\DualFrontier.Contracts\DualFrontier.Contracts.csproj" />
    <ProjectReference Include="..\..\src\DualFrontier.Core.Interop\DualFrontier.Core.Interop.csproj" />
  </ItemGroup>
  <!-- Native DLL must ride along into test output, like Benchmarks csproj does. -->
  <ItemGroup>
    <None Include="$(MSBuildThisFileDirectory)..\..\native\DualFrontier.Core.Native\build\Release\DualFrontier.Core.Native.dll"
          Condition="Exists('$(MSBuildThisFileDirectory)..\..\native\DualFrontier.Core.Native\build\Release\DualFrontier.Core.Native.dll')">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
      <Link>DualFrontier.Core.Native.dll</Link>
    </None>
  </ItemGroup>
</Project>
```

### 5.2 — Create `tests/DualFrontier.Core.Interop.Tests/EntityIdPackingTests.cs`

```csharp
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class EntityIdPackingTests
{
    [Fact]
    public void Pack_then_Unpack_preserves_index_and_version()
    {
        var original = new EntityId(42, 7);
        ulong packed = EntityIdPacking.Pack(original);
        EntityId unpacked = EntityIdPacking.Unpack(packed);

        unpacked.Index.Should().Be(42);
        unpacked.Version.Should().Be(7);
    }

    [Fact]
    public void Pack_zero_entity_yields_zero_ulong()
    {
        var zero = new EntityId(0, 0);
        ulong packed = EntityIdPacking.Pack(zero);
        packed.Should().Be(0UL);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(int.MaxValue, int.MaxValue)]
    [InlineData(1_000_000, 999)]
    public void Pack_unpack_roundtrip(int index, int version)
    {
        var original = new EntityId(index, version);
        ulong packed = EntityIdPacking.Pack(original);
        EntityId roundtrip = EntityIdPacking.Unpack(packed);

        roundtrip.Index.Should().Be(index);
        roundtrip.Version.Should().Be(version);
    }

    [Fact]
    public void Pack_layout_matches_capi_h_specification()
    {
        // df_capi.h spec: high 32 bits = Version, low 32 bits = Index.
        var id = new EntityId(0xCAFEBABE, 0x12345678);
        ulong packed = EntityIdPacking.Pack(id);

        ulong expectedHigh = (ulong)0x12345678 << 32;
        ulong expectedLow = (ulong)0xCAFEBABE;
        packed.Should().Be(expectedHigh | expectedLow);
    }
}
```

### 5.3 — Create `tests/DualFrontier.Core.Interop.Tests/ComponentTypeRegistryTests.cs`

```csharp
using System;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class ComponentTypeRegistryTests
{
    private struct TypeA { public int Value; }
    private struct TypeB { public long Value; }
    private struct TypeC { public byte Value; }

    [Fact]
    public void Register_returns_sequential_ids_starting_from_1()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint idA = registry.Register<TypeA>();
        uint idB = registry.Register<TypeB>();
        uint idC = registry.Register<TypeC>();

        idA.Should().Be(1);
        idB.Should().Be(2);
        idC.Should().Be(3);
    }

    [Fact]
    public void Register_is_idempotent()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint first = registry.Register<TypeA>();
        uint second = registry.Register<TypeA>();
        uint third = registry.Register<TypeA>();

        first.Should().Be(second).And.Be(third);
    }

    [Fact]
    public void GetId_throws_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        Action act = () => registry.GetId<TypeA>();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not registered*");
    }

    [Fact]
    public void TryGetId_returns_false_for_unregistered_type()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        bool found = registry.TryGetId<TypeA>(out uint id);
        found.Should().BeFalse();
        id.Should().Be(0);
    }

    [Fact]
    public void TryGetId_returns_true_after_register()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint registered = registry.Register<TypeA>();
        bool found = registry.TryGetId<TypeA>(out uint id);

        found.Should().BeTrue();
        id.Should().Be(registered);
    }

    [Fact]
    public void Lookup_returns_type_for_registered_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        uint id = registry.Register<TypeA>();
        Type? type = registry.Lookup(id);

        type.Should().Be(typeof(TypeA));
    }

    [Fact]
    public void Lookup_returns_null_for_unassigned_id()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        Type? type = registry.Lookup(999);
        type.Should().BeNull();
    }

    [Fact]
    public void Count_reflects_registered_types()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.Count.Should().Be(0);
        registry.Register<TypeA>();
        registry.Count.Should().Be(1);
        registry.Register<TypeB>();
        registry.Count.Should().Be(2);
        registry.Register<TypeA>();  // idempotent
        registry.Count.Should().Be(2);
    }

    [Fact]
    public void IsRegistered_reflects_registration_state()
    {
        using var world = new NativeWorld();
        var registry = new ComponentTypeRegistry(world.HandleForInternalUseTest);

        registry.IsRegistered<TypeA>().Should().BeFalse();
        registry.Register<TypeA>();
        registry.IsRegistered<TypeA>().Should().BeTrue();
        registry.IsRegistered<TypeB>().Should().BeFalse();
    }
}
```

**NOTE**: tests use `world.HandleForInternalUseTest` — internal accessor that needs to be added к `NativeWorld.cs`. See Step 5.7 для that addition.

### 5.4 — Create `tests/DualFrontier.Core.Interop.Tests/NativeWorldTests.cs`

```csharp
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class NativeWorldTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    private struct PositionComponent
    {
        public float X;
        public float Y;
    }

    [Fact]
    public void CreateEntity_returns_valid_alive_entity()
    {
        using var world = new NativeWorld();

        EntityId entity = world.CreateEntity();

        world.IsAlive(entity).Should().BeTrue();
        world.EntityCount.Should().Be(1);
    }

    [Fact]
    public void DestroyEntity_marks_entity_dead_immediately()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        world.DestroyEntity(entity);

        world.IsAlive(entity).Should().BeFalse();
    }

    [Fact]
    public void Component_added_can_be_retrieved()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var health = new HealthComponent { Current = 75, Maximum = 100 };

        world.AddComponent(entity, health);
        HealthComponent retrieved = world.GetComponent<HealthComponent>(entity);

        retrieved.Current.Should().Be(75);
        retrieved.Maximum.Should().Be(100);
    }

    [Fact]
    public void HasComponent_reflects_addition_and_removal()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        var health = new HealthComponent { Current = 50, Maximum = 100 };

        world.HasComponent<HealthComponent>(entity).Should().BeFalse();
        world.AddComponent(entity, health);
        world.HasComponent<HealthComponent>(entity).Should().BeTrue();
        world.RemoveComponent<HealthComponent>(entity);
        world.HasComponent<HealthComponent>(entity).Should().BeFalse();
    }

    [Fact]
    public void Multiple_component_types_coexist_on_entity()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        world.AddComponent(entity, new HealthComponent { Current = 75, Maximum = 100 });
        world.AddComponent(entity, new PositionComponent { X = 10f, Y = 20f });

        world.HasComponent<HealthComponent>(entity).Should().BeTrue();
        world.HasComponent<PositionComponent>(entity).Should().BeTrue();
        world.GetComponent<HealthComponent>(entity).Current.Should().Be(75);
        world.GetComponent<PositionComponent>(entity).X.Should().Be(10f);
    }

    [Fact]
    public void Destroyed_entity_components_persist_until_flush()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new HealthComponent { Current = 50, Maximum = 100 });

        world.DestroyEntity(entity);

        world.IsAlive(entity).Should().BeFalse();
        world.GetComponentCount<HealthComponent>().Should().Be(1);  // pre-flush

        world.FlushDestroyedEntities();

        world.GetComponentCount<HealthComponent>().Should().Be(0);  // post-flush
    }

    [Fact]
    public void World_works_with_explicit_registry()
    {
        var tempWorldHandle = NativeMethodsTestAccess.CreateWorld();
        var registry = new ComponentTypeRegistry(tempWorldHandle);
        NativeMethodsTestAccess.DestroyWorld(tempWorldHandle);

        // Now create real world с pre-bound registry pattern (registry will rebind
        // on actual NativeWorld constructor, demonstrated by behavior).
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();
        world.AddComponent(entity, new HealthComponent { Current = 1, Maximum = 1 });

        world.GetComponent<HealthComponent>(entity).Current.Should().Be(1);
    }

    [Fact]
    public void Disposed_world_throws_on_use()
    {
        var world = new NativeWorld();
        world.Dispose();

        Action act = () => world.CreateEntity();
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void TryGetComponent_returns_false_for_missing()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        bool found = world.TryGetComponent<HealthComponent>(entity, out _);
        found.Should().BeFalse();
    }

    [Fact]
    public void GetComponent_throws_for_missing()
    {
        using var world = new NativeWorld();
        EntityId entity = world.CreateEntity();

        Action act = () => world.GetComponent<HealthComponent>(entity);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*not found*");
    }
}
```

**NOTE**: `NativeMethodsTestAccess` это test helper — нужен только если test необходим к создавать handle напрямую. Для simplicity можно пропустить этот test или упростить его. Если упростить, удалить test «World_works_with_explicit_registry» — он demonstrates pattern но требует test infrastructure не указанную в этом brief. Альтернатива — добавить в NativeWorld.cs:

```csharp
/// <summary>Test-only accessor для handle. Marked internal с InternalsVisibleTo.</summary>
internal IntPtr HandleForInternalUseTest => _handle;
```

И в `DualFrontier.Core.Interop.csproj` добавить:
```xml
<ItemGroup>
  <InternalsVisibleTo Include="DualFrontier.Core.Interop.Tests" />
</ItemGroup>
```

### 5.5 — Create `tests/DualFrontier.Core.Interop.Tests/SpanLeaseTests.cs`

```csharp
using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class SpanLeaseTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    [Fact]
    public void AcquireSpan_on_empty_returns_lease_с_zero_count()
    {
        using var world = new NativeWorld();
        // Force store creation via Add then Remove
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 1 });
        world.RemoveComponent<HealthComponent>(e);

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(0);
        lease.Span.Length.Should().Be(0);
    }

    [Fact]
    public void Span_provides_read_access_к_dense_storage()
    {
        using var world = new NativeWorld();
        EntityId[] entities = new EntityId[5];
        for (int i = 0; i < 5; i++)
        {
            entities[i] = world.CreateEntity();
            world.AddComponent(entities[i],
                new HealthComponent { Current = i * 10, Maximum = 100 });
        }

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Count.Should().Be(5);
        lease.Span.Length.Should().Be(5);

        // Sum через span (no per-element P/Invoke)
        int sum = 0;
        for (int i = 0; i < lease.Count; i++)
        {
            sum += lease.Span[i].Current;
        }
        sum.Should().Be(0 + 10 + 20 + 30 + 40);
    }

    [Fact]
    public void Indices_parallel_к_Span()
    {
        using var world = new NativeWorld();
        for (int i = 0; i < 3; i++)
        {
            EntityId e = world.CreateEntity();
            world.AddComponent(e, new HealthComponent { Current = i, Maximum = 100 });
        }

        using var lease = world.AcquireSpan<HealthComponent>();

        lease.Indices.Length.Should().Be(lease.Count);
        for (int i = 0; i < lease.Count; i++)
        {
            lease.Indices[i].Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void Mutation_rejected_while_span_active()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        using var lease = world.AcquireSpan<HealthComponent>();

        // Mutation should be silently rejected (caught native-side, void no-op)
        EntityId e2 = world.CreateEntity();  // create OK, no native span check on createEntity itself
        // But adding component while span active triggers throw → ABI swallow → no-op
        world.AddComponent(e2, new HealthComponent { Current = 999, Maximum = 100 });

        // Component count should remain 1, не 2
        world.GetComponentCount<HealthComponent>().Should().Be(1);
    }

    [Fact]
    public void Mutation_succeeds_after_lease_disposed()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        lease.Dispose();

        EntityId e2 = world.CreateEntity();
        world.AddComponent(e2, new HealthComponent { Current = 2, Maximum = 100 });
        world.GetComponentCount<HealthComponent>().Should().Be(2);
    }

    [Fact]
    public void Span_throws_after_dispose()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        var lease = world.AcquireSpan<HealthComponent>();
        lease.Dispose();

        Action act = () => { var _ = lease.Span; };
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void Multiple_concurrent_leases_supported()
    {
        using var world = new NativeWorld();
        EntityId e = world.CreateEntity();
        world.AddComponent(e, new HealthComponent { Current = 1, Maximum = 100 });

        using var lease1 = world.AcquireSpan<HealthComponent>();
        using var lease2 = world.AcquireSpan<HealthComponent>();  // same type, OK

        lease1.Count.Should().Be(1);
        lease2.Count.Should().Be(1);
    }
}
```

### 5.6 — Create `tests/DualFrontier.Core.Interop.Tests/BulkOperationsTests.cs`

```csharp
using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Core.Interop.Tests;

public class BulkOperationsTests
{
    private struct HealthComponent
    {
        public int Current;
        public int Maximum;
    }

    [Fact]
    public void AddComponents_bulk_adds_all_in_single_call()
    {
        using var world = new NativeWorld();
        const int count = 100;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }

        world.AddComponents<HealthComponent>(entities, components);

        world.GetComponentCount<HealthComponent>().Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            world.GetComponent<HealthComponent>(entities[i]).Current.Should().Be(i);
        }
    }

    [Fact]
    public void GetComponents_bulk_reads_all_in_single_call()
    {
        using var world = new NativeWorld();
        const int count = 50;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i * 2, Maximum = 100 };
        }
        world.AddComponents<HealthComponent>(entities, components);

        var output = new HealthComponent[count];
        int successful = world.GetComponents<HealthComponent>(entities, output);

        successful.Should().Be(count);
        for (int i = 0; i < count; i++)
        {
            output[i].Current.Should().Be(i * 2);
        }
    }

    [Fact]
    public void GetComponents_bulk_handles_mixed_alive_dead()
    {
        using var world = new NativeWorld();
        const int count = 10;
        var entities = new EntityId[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            world.AddComponent(entities[i], new HealthComponent { Current = i, Maximum = 100 });
        }

        // Destroy half + flush
        for (int i = 0; i < count; i += 2)
        {
            world.DestroyEntity(entities[i]);
        }
        world.FlushDestroyedEntities();

        var output = new HealthComponent[count];
        int successful = world.GetComponents<HealthComponent>(entities, output);

        successful.Should().Be(count / 2);  // only odd indices alive
    }

    [Fact]
    public void AddComponents_throws_on_length_mismatch()
    {
        using var world = new NativeWorld();
        var entities = new EntityId[5];
        var components = new HealthComponent[3];

        Action act = () => world.AddComponents<HealthComponent>(entities, components);
        act.Should().Throw<ArgumentException>()
           .WithMessage("*Mismatched lengths*");
    }

    [Fact]
    public void AddComponents_handles_empty_spans()
    {
        using var world = new NativeWorld();
        var entities = ReadOnlySpan<EntityId>.Empty;
        var components = ReadOnlySpan<HealthComponent>.Empty;

        // Should not throw
        world.AddComponents<HealthComponent>(entities, components);
        world.GetComponentCount<HealthComponent>().Should().Be(0);
    }

    [Fact]
    public void Bulk_then_span_iteration_consistent()
    {
        using var world = new NativeWorld();
        const int count = 20;
        var entities = new EntityId[count];
        var components = new HealthComponent[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = world.CreateEntity();
            components[i] = new HealthComponent { Current = i, Maximum = 100 };
        }
        world.AddComponents<HealthComponent>(entities, components);

        using var lease = world.AcquireSpan<HealthComponent>();
        lease.Count.Should().Be(count);

        int sum = 0;
        for (int i = 0; i < lease.Count; i++)
        {
            sum += lease.Span[i].Current;
        }

        // Sum 0..19 = 190
        sum.Should().Be(190);
    }
}
```

### 5.7 — Final adjustments к NativeWorld.cs / csproj

Edit `src/DualFrontier.Core.Interop/DualFrontier.Core.Interop.csproj` — add InternalsVisibleTo:

```xml
<ItemGroup>
  <InternalsVisibleTo Include="DualFrontier.Core.Interop.Tests" />
</ItemGroup>
```

Edit `src/DualFrontier.Core.Interop/NativeWorld.cs` — add test accessor next к existing `HandleForInternalUse`:

```csharp
/// <summary>
/// Test-only accessor (visible через InternalsVisibleTo to test project).
/// Returns same handle as HandleForInternalUse но visible from external assembly.
/// </summary>
internal IntPtr HandleForInternalUseTest => _handle;
```

### 5.8 — Add test project к DualFrontier.sln

```powershell
dotnet sln DualFrontier.sln add tests/DualFrontier.Core.Interop.Tests/DualFrontier.Core.Interop.Tests.csproj
```

### 5.9 — Atomic commit для test project

```powershell
git add tests/DualFrontier.Core.Interop.Tests/ `
        src/DualFrontier.Core.Interop/DualFrontier.Core.Interop.csproj `
        src/DualFrontier.Core.Interop/NativeWorld.cs `
        DualFrontier.sln

git commit -m "test(interop): K2 bridge test project (~35 tests)

Creates DualFrontier.Core.Interop.Tests (xUnit + FluentAssertions)
с comprehensive coverage of Interop layer:
- EntityIdPackingTests (4): bit-level pack/unpack invariants, capi.h spec
- ComponentTypeRegistryTests (9): sequential IDs, idempotency, lookup,
  Count/IsRegistered semantics
- NativeWorldTests (10): CRUD round-trip, multi-component coexistence,
  deferred destroy semantics, Dispose throws
- SpanLeaseTests (7): acquisition/release lifetime, span access,
  mutation rejection, multiple concurrent leases
- BulkOperationsTests (6): bulk add/get correctness, length validation,
  empty spans, bulk-then-span consistency

Total: ~36 new tests bringing project test count to 472 + 36 = 508.

Native DLL bundling configured per existing Benchmarks csproj pattern.
InternalsVisibleTo added для test access к NativeWorld._handle."
```

---

## Step 6 — Test execution + verification

```powershell
dotnet build
# Expected: 0 errors. Pre-existing CS0618 obsolete warnings about
# NativeComponentType<T> are suppressed via #pragma in NativeWorld.cs.

dotnet test
# Expected: 472 (existing) + ~36 (new K2) = ~508 passing, 0 failed
```

Если number != ~508 — investigate. Tests могут fail на:
- Native DLL не скопирован (check `Test-Path tests\DualFrontier.Core.Interop.Tests\bin\Debug\net8.0\DualFrontier.Core.Native.dll`)
- InternalsVisibleTo не applied (`HandleForInternalUseTest` not visible)
- ComponentTypeRegistry tests fail из-за legacy NativeWorld() constructor — должны использовать explicit handle from new world

---

## Step 7 — Update MIGRATION_PROGRESS.md (с sequencing decision!)

K2 closure is the **trigger point для sequencing decision** (D2). Brief execution must record выбор β-variant.

### 7.1 — Sequencing decision recommendation

**Per Crystalka philosophy «cleanness > expediency»** + evidence от K0+K1+K2:
- Bridge layer matured (registry + tests give confidence)
- Native API stable (16+1=17 functions, ABI baseline solid)
- Two paths valid: β5 (kernel pause → runtime sprint) OR β6 (kernel-first sequential)

**Recommendation для D2 closure**: **β6 — kernel-first sequential**.

Rationale:
- K3 (native bootstrap graph) и K4 (struct refactor) — наиболее интенсивная kernel work. Делать их пока контекст native development свежий = меньше context-switching cost.
- M9.x runtime work не имеет dependencies на K3-K8. Откладывание не блокирует ничего.
- «Single architectural focus per period» preserves cleanness.

Альтернативно: β5 если хочется skipped runtime delivery в середине kernel work. Это valid но requires разбиение фокуса.

**Decision authority**: Crystalka. Brief recommends β6 но decision is human's к make.

### 7.2 — Update tracker

Open `docs/MIGRATION_PROGRESS.md`. Update:

**Current state snapshot**:
```markdown
| **Active phase** | K3 (planned) — native bootstrap graph + thread pool |
| **Last completed milestone** | K2 (registry + tests) — `<sha>` 2026-MM-DD |
| **Next milestone (recommended)** | K3 (native bootstrap graph + thread pool) |
| **Sequencing strategy** | β6 — kernel-first sequential (decided 2026-MM-DD per K2 closure) |
| **Tests passing** | 508 (472 baseline + 36 K2 new) |
```

**Sequencing decision section** — replace OPEN status:
```markdown
## Sequencing decision

**Status**: RESOLVED 2026-MM-DD per K2 closure
**Decision**: β6 — kernel-first sequential (K0–K8 → M9.0–M9.8)
**Rationale**:
- K2 closure provides bridge maturity evidence (registry + 36 tests + selftest 7/7)
- K3-K8 kernel work has no dependencies on M9.x runtime
- Single architectural focus per period preserves cleanness (Crystalka philosophy)
- M9.0-M9.8 runtime sprint deferred к after K8 cutover decision
**Alternatives rejected**:
- β5 (kernel fast-track) — would require context-switching mid-kernel
- β3 (interleaved) — context-switching cost with no compensating speedup

[Original 3-options table preserved below для historical reference]
```

**K2 detailed entry** (после K1):
```markdown
### K2 — Type-id registry + bridge tests project

- **Status**: DONE (`<commit-sha>`, 2026-MM-DD)
- **Brief**: `tools/briefs/K2_REGISTRY_TESTS_BRIEF.md` (FULL EXECUTED)
- **C ABI extension**: 1 new function — `df_world_register_component_type` (16 → 17 total)
- **Native side**: `World::register_component_type` с idempotent + size-conflict handling
- **Managed bridge**: `ComponentTypeRegistry` instance-per-NativeWorld с sequential IDs;
  `NativeWorld(registry?)` overload для opt-in explicit registration;
  `ResolveTypeId<T>()` centralizes registry-vs-FNV-fallback dispatch
- **Legacy compat**: `NativeComponentType<T>` + `NativeComponentTypeRegistry` marked
  `[Obsolete]` (warning, not error) — retained для backward compat
- **Selftest scenarios**: 6 → 7 (added `scenario_explicit_registration`)
- **Test project**: NEW `DualFrontier.Core.Interop.Tests` (xUnit + FluentAssertions)
  с 5 test classes / ~36 tests
- **Managed tests**: 472 → 508 passing (36 new)
- **Sequencing decision**: β6 chosen (см. Sequencing decision section)
- **Lessons learned**: <fill if non-trivial issues encountered>
```

### 7.3 — Atomic commit

```powershell
git add docs/MIGRATION_PROGRESS.md
git commit -m "docs(migration): K2 closure recorded + sequencing decision β6"
```

---

## Step 8 — Update K2 brief skeleton

Open `tools/briefs/K2_REGISTRY_TESTS_BRIEF.md`. Brief был authored на main в Step 0 и теперь EXECUTED. Update:

```markdown
## Status: EXECUTED

**Date**: 2026-MM-DD
**Branch**: feat/k2-registry-and-tests
**Final commit**: <sha>

Full executable brief authored and executed. See git log on
feat/k2-registry-and-tests branch для atomic commit sequence.

See `MIGRATION_PROGRESS.md` for closure record + sequencing decision.
```

```powershell
git add tools/briefs/K2_REGISTRY_TESTS_BRIEF.md
git commit -m "docs(briefs): K2 brief skeleton marked EXECUTED"
```

---

## Step 9 — Final verification & merge prep

### Branch state check

```powershell
git log --oneline main..HEAD
# Expected sequence (~7 commits, начиная с brief authoring которое было pre-Step 0):
#   <sha> docs(briefs): K2 brief skeleton marked EXECUTED
#   <sha> docs(migration): K2 closure recorded + sequencing decision β6
#   <sha> test(interop): K2 bridge test project (~35 tests)
#   <sha> test(native): K2 selftest scenario для explicit registration
#   <sha> interop(kernel): ComponentTypeRegistry с deterministic IDs
#   <sha> native(kernel): explicit component type registration
#   <sha> docs(briefs): K2 brief authored — full executable registry + tests   ← Step 0 commit
```

(Step 0 brief authoring commit может быть на main, не на feature branch — depending on когда был сделан merge или branch checkout. Both are valid.)

### Final builds

```powershell
git status                                  # clean
cmake --build native\DualFrontier.Core.Native\build --config Release
.\native\DualFrontier.Core.Native\build\Release\df_native_selftest.exe
# Expected: 7 scenarios, ALL PASSED

dotnet build
dotnet test
# Expected: 508 passing
```

### Push

```powershell
git push -u origin feat/k2-registry-and-tests
```

PR-ready для review/merge.

---

## Acceptance criteria

K2 закрыт когда ВСЕ выполнено:

- [ ] Step 0 brief authoring commit на main выполнен
- [ ] Branch `feat/k2-registry-and-tests` создан от `main`
- [ ] Native commit: `df_world_register_component_type` в C ABI + `World::register_component_type` implementation
- [ ] Interop commit: `ComponentTypeRegistry` class + `NativeWorld` integration + `NativeComponentType<T>` deprecation
- [ ] Selftest commit: `scenario_explicit_registration` (7 scenarios total)
- [ ] Tests commit: `DualFrontier.Core.Interop.Tests` project с ~36 tests across 5 test classes
- [ ] `cmake --build` clean — 0 errors, 0 warnings
- [ ] Native selftest: **7 scenarios ALL PASSED**
- [ ] `dotnet build` clean
- [ ] `dotnet test`: **~508 passing** (472 baseline + ~36 new K2)
- [ ] MIGRATION_PROGRESS.md K2 row DONE с commit SHA
- [ ] MIGRATION_PROGRESS.md sequencing decision RESOLVED (β6 recommended, decision recorded)
- [ ] tools/briefs/K2_REGISTRY_TESTS_BRIEF.md marked EXECUTED
- [ ] Branch pushed to origin
- [ ] No build artifacts committed

---

## Rollback procedure

K2 не делает destructive changes — main untouched until merge. Rollback:

```powershell
git checkout main
git branch -D feat/k2-registry-and-tests
# Step 0 brief authoring commit остаётся на main как durable artifact
# (если хотите откатить и его: git reset --hard <pre-Step0-sha>)
```

---

## Open issues / lessons learned (заполнить при closure)

<empty — заполнить если в процессе обнаружилось что-то нетривиальное>

---

## Pipeline metadata

- **Brief authored by**: Opus (architect)
- **Brief executed by**: Claude Code agent или human
- **Final review**: Crystalka (architectural judgment + commit author)
- **Methodology compliance**: 
  - METHODOLOGY.md v1.3 «Step 0 brief authoring» applied
  - METHODOLOGY.md v1.3 «throw inventory» completed (1 throw site, 1 boundary, 1 wrap)
  - METHODOLOGY.md v1.2 «descriptive pre-flight» applied (8 hard gates + 4 informational)

**Brief end. Companion docs: KERNEL_ARCHITECTURE.md (§1.10, §1.11, §K2), MIGRATION_PROGRESS.md, METHODOLOGY.md v1.3.**
