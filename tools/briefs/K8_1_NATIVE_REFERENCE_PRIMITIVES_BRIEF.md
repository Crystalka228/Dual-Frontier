# K8.1 — Native-side reference handling primitives

**Brief version**: 1.0 (full, executable)
**Authored**: 2026-05-09
**Status**: EXECUTED (2026-05-09, branch `feat/k8-1-native-reference-primitives`, closure `a62c1f3..812df98`). See `docs/MIGRATION_PROGRESS.md` § "K8.1 — Native-side reference handling primitives" for closure record.
**Reference docs**: `docs/architecture/KERNEL_ARCHITECTURE.md` v1.2 LOCKED (K-L11 production storage backbone, K-L3 implication extended), `docs/MIGRATION_PROGRESS.md` (K8.0 closure + Solution A roadmap), `docs/CODING_STANDARDS.md`, native module docs (`native/DualFrontier.Core.Native/MODULE.md`, `native/DualFrontier.Core.Native/include/MODULE.md`, `native/DualFrontier.Core.Native/src/MODULE.md`)
**Companion**: `docs/MIGRATION_PROGRESS.md` (live tracker — K8.1 row promotes from NOT STARTED → DONE on closure)
**Methodology lineage**: `tools/briefs/K8_0_SOLUTION_A_RECORDING_BRIEF.md` (architectural decision brief precedent), `tools/briefs/K7_PERFORMANCE_MEASUREMENT_BRIEF.md` (read-first/brief-second/execute-third pivot), `tools/briefs/MOD_OS_V16_AMENDMENT_CLOSURE.md` (Anthropic `Edit` literal-mode semantics)
**Predecessor**: K8.0 (`9f9dc05..28498f9`) — Solution A architectural commitment recorded
**Target**: fresh feature branch `feat/k8-1-native-reference-primitives` from `main` after K8.0 closure
**Estimated time**: 8-14 hours auto-mode (1-2 weeks at hobby pace ~1h/day)
**Estimated LOC delta**: ~+1200/-30 (substantial native + bridge work; foundation для K8.2 component redesigns)

---

## Goal

Implement four native-side reference handling primitives that allow currently class-based components (`MovementComponent`, `IdentityComponent`, `SkillsComponent`, `SocialComponent`, `StorageComponent`, `WorkbenchComponent`, `FactionComponent`) to be redesigned as `unmanaged` structs in K8.2. After K8.1 closure, K-L3 «unmanaged structs only» can be fully realized без exception в K8.2.

The four primitives:

1. **String interning** (`StringPool`): native-side string pool, generational with per-mod scopes. Managed `string` fields в components become `InternedString` (wraps `uint32_t` ID + `uint32_t` pool-generation tag).
2. **Keyed map** (`KeyedMap`): native-side dictionary primitive с sorted-by-key iteration. Replaces `Dictionary<TKey, TValue>` patterns в Skills/Social/Storage components.
3. **Composite component** (`Composite`): native-side variable-length data array attached к parent component via composite-id. Replaces `List<T>` patterns в Movement (path waypoints), Storage (item list).
4. **Set primitive** (`SetPrimitive`): native-side keyed set с sorted-by-element iteration. Replaces `HashSet<T>` patterns в Storage (reservation set).

Each primitive backed by:
- C++ implementation в `native/DualFrontier.Core.Native/src/<primitive>.cpp` + `native/DualFrontier.Core.Native/include/<primitive>.h`
- C ABI surface в `df_capi.h` / `capi.cpp`
- Managed bridge type в `DualFrontier.Core.Interop.Marshalling`
- Selftest scenarios (round-trip / collision / iterate / mod-scope clear) в `native/DualFrontier.Core.Native/test/selftest.cpp`
- Bridge equivalence tests в `tests/DualFrontier.Core.Interop.Tests/Marshalling/`

K8.1 is **a foundation milestone**. It does not migrate any components yet — that's K8.2. It does not migrate any production systems — that's K8.3. К8.1's deliverable is the primitive infrastructure on which K8.2 builds.

---

## Phase 0 — Pre-flight verification

### 0.1 — Working tree clean

```
git status
```

**Expected**: `nothing to commit, working tree clean` on branch `main`.

**Halt condition**: any uncommitted modifications. Resolution: stash via `git stash push -m "pre-K8-1-WIP"` and re-verify.

### 0.2 — Prerequisite milestone closed

```
git log --oneline -25
```

**Expected**: K8.0 closure commits visible (most recent: `28498f9` per K8.0 closure record). K8.0 row in MIGRATION_PROGRESS.md status DONE; K8.1 row status NOT STARTED.

**Halt condition**: K8.0 not closed. K8.1 builds atop K-L11 LOCKED commitment; without K8.0, the architectural foundation is unrecorded.

### 0.3 — Prerequisite documents at expected versions

```
head -10 docs/architecture/KERNEL_ARCHITECTURE.md
head -10 docs/MIGRATION_PROGRESS.md
ls docs/PERFORMANCE_REPORT_K7.md
```

**Expected**:
- `KERNEL_ARCHITECTURE.md` Status: AUTHORITATIVE LOCKED v1.2 (post-K8.0 amendment)
- `MIGRATION_PROGRESS.md` Last updated: 2026-05-09 (K8.0 closure)
- `PERFORMANCE_REPORT_K7.md` exists, status FINAL

**Halt condition**: any spec at unexpected version. К8.1 implements against K-L11 verbatim; mismatch means spec contract has shifted.

### 0.4 — Code state inventory

```
ls native/DualFrontier.Core.Native/include/
ls native/DualFrontier.Core.Native/src/
ls native/DualFrontier.Core.Native/test/
ls src/DualFrontier.Core.Interop/Marshalling/
ls tests/DualFrontier.Core.Interop.Tests/Marshalling/
```

**Expected** existing public headers in `include/`: `bootstrap_graph.h`, `component_store.h`, `df_capi.h`, `entity_id.h`, `MODULE.md`, `sparse_set.h`, `thread_pool.h`, `world.h`.

**Expected** existing source files in `src/`: `bootstrap_graph.cpp`, `capi.cpp`, `component_store.cpp`, `MODULE.md`, `thread_pool.cpp`, `world.cpp`.

**Expected** existing test files in `test/`: `MODULE.md`, `selftest.cpp`.

**Expected** existing managed bridge files: `RawComponentStore.cs`, `WriteBatch.cs`, `SpanLease.cs`, `ComponentTypeRegistry.cs`, possibly more.

**K8.1 will add** to `include/`: `string_pool.h`, `keyed_map.h`, `composite.h`, `set_primitive.h`.

**K8.1 will add** to `src/`: `string_pool.cpp`, `keyed_map.cpp`, `composite.cpp`, `set_primitive.cpp`.

**K8.1 will add** managed bridge files: `InternedString.cs`, `NativeMap.cs`, `NativeComposite.cs`, `NativeSet.cs`.

**K8.1 will extend** existing files: `df_capi.h` (add C ABI sections), `capi.cpp` (add implementations), `world.h`/`world.cpp` (add primitive ownership и mod-scope tracking), `selftest.cpp` (add scenarios), `CMakeLists.txt` (add new sources).

**Halt condition**: any expected file missing. К8.1 assumes K0-K7 + K8.0 deliverables present.

### 0.5 — Native build clean

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
```

**Expected**: build succeeds without warnings or errors. Selftest passes:

```
build/Release/df_native_selftest.exe
```

**Halt condition**: native build failure or selftest failure on baseline. К8.1 starts from a known-good native state.

### 0.6 — Managed build + test baseline

```
cd ../..
dotnet build
dotnet test
```

**Expected**: build clean, **553 tests passing** (post-K8.0 baseline; K8.0 added no source changes).

**Halt condition**: any regression. К8.1 must not regress baseline; any regression indicates either an environment issue or an upstream regression that should be addressed before K8.1 work.

---

## Phase 1 — Architectural design (LOCKED — read-only, no edits)

This phase is the **architectural foundation** for K8.1. The executor reads this section as the design contract; decisions here are LOCKED by Crystalka's stated commitment («без костылей и сокращений») and the K8.0 architectural commitment (К-L11).

### 1.1 — Per-primitive file organization (LOCKED)

Each of the four primitives lives в its own header/source pair:

- `string_pool.h` / `string_pool.cpp` — `StringPool` class
- `keyed_map.h` / `keyed_map.cpp` — `KeyedMap` class
- `composite.h` / `composite.cpp` — `Composite` class
- `set_primitive.h` / `set_primitive.cpp` — `SetPrimitive` class

**Rationale**:
- Single responsibility per file. Each primitive has its own invariants, its own selftest section, its own C ABI section, its own managed bridge.
- Future K8.1.x patches can target one primitive without trampling others.
- Aligns с established native module structure (`component_store.h`, `sparse_set.h`, `thread_pool.h` already follow this pattern).
- `world.cpp` includes all four headers and orchestrates ownership; primitives don't depend on each other.

### 1.2 — Generational string pool с per-mod scope (LOCKED)

**Architecture**:

`StringPool` class owns:
- `std::vector<std::string> contents_` — flat array of unique strings, indexed by ID.
- `std::unordered_map<std::string, uint32_t> lookup_` — content → ID for dedup on intern.
- `std::vector<uint32_t> generation_per_id_` — generation tag per ID, parallel to `contents_`.
- `std::unordered_map<std::string, std::vector<uint32_t>> ids_by_mod_` — mod-scope tracking: which IDs were interned by which mod.
- Current generation counter `uint32_t current_generation_` — incremented on each per-mod clear.

**Lifecycle**:

```
StringPool pool;

// Mod A loads, registers itself
pool.begin_mod_scope("ModA");
uint32_t id1 = pool.intern("MyFaction");      // id=1, gen=1, owned by ModA
uint32_t id2 = pool.intern("OtherFaction");   // id=2, gen=1, owned by ModA
pool.end_mod_scope("ModA");

// Mod B loads
pool.begin_mod_scope("ModB");
uint32_t id3 = pool.intern("MyFaction");      // id=1 returned (dedup), tracked also by ModB
uint32_t id4 = pool.intern("ModBExclusive");  // id=3, gen=1, owned by ModB
pool.end_mod_scope("ModB");

// Mod A unloads
pool.clear_mod_scope("ModA");
// id=1 still alive (still referenced by ModB)
// id=2 reclaimed, generation incremented (id=2 will be reused)
// id=3 still alive

uint32_t id5 = pool.intern("NewString");      // id=2 reused, gen=2 (incremented)
```

**Generation tag prevents stale-ID reads**: if a component was saved с InternedString{id=2, gen=1}, after the clear-and-reuse cycle it becomes InternedString{id=2, gen=2}. Reading the saved `{id=2, gen=1}` against the current pool detects mismatch on generation; either:
- Returns sentinel «unresolved» (caller decides how to recover)
- Triggers a re-intern: caller passes the original string content → gets a fresh `{id, gen}` pair

**Save/load semantics** (LOCKED): saves serialize **string content, not IDs**. On load, content is re-interned, fresh `{id, gen}` issued. No ID mapping table needed. This is the cleanest path; the alternative (serialize IDs and map on load) requires bookkeeping that adds permanent complexity.

**InternedString equality semantics** (LOCKED):
- Same-pool comparison: `id` equality (fast path; common case during runtime within one process).
- Cross-pool comparison: content equality (correctness path; rare, mostly during save/load reconciliation if it ever happens — current Solution A says it doesn't).
- The managed `InternedString` struct exposes both `Id` and a `ResolveContent()` method for cases where content equality is genuinely needed (e.g., debug output, save serialization).

**Pool-per-process** (LOCKED): one `StringPool` per `World`. Multiple Worlds (e.g., parallel scenarios in tests) each have independent pools. The pool is not a global singleton.

**No reference counting**: per K8.1's «without compromise» direction, we don't add per-ID refcount logic. Mod-scope ownership tracking serves as a coarse ownership model (entire mod's strings reclaimed on unload), которое sufficient для the production case.

### 1.3 — Sorted-by-key iteration in KeyedMap (LOCKED)

**Architecture**:

`KeyedMap` owns:
- `std::vector<KeyValuePair> entries_` — flat sorted array of (key, value) pairs, sorted by key.
- Insertion: binary search for key position; if found, update value; if not, insert at correct position (memmove if needed).
- Lookup: binary search on sorted keys.
- Iteration: linear scan через `entries_`, encounters keys в sorted order.

**Trade-off accepted**: O(log n) lookup + O(n) insertion (for memmove). For typical map sizes в game components (Skills: ~10 entries; Social: ~20-50; Storage: ~30-100), this is acceptable. Hash-table performance is not necessary at these sizes; determinism is.

**Determinism guarantees**:
- Save/load roundtrip: serialization writes entries в iteration order (sorted); load reads them; resulting map identical.
- Cross-platform: sort order is `<` on the key type, deterministic across compilers / architectures (assuming key type defines stable ordering, which all our key types do — `uint32_t`, `EntityId`, enum types).
- Cross-mod: when one mod's system iterates a map populated by another mod, iteration order matches sort order, не insertion order, не hash order. No mod gets surprise behavior.

**KeyedMap is type-erased like RawComponentStore**: stores raw `std::vector<uint8_t>` for keys и for values, sized at construction by caller-provided `key_size` и `value_size`. The C ABI passes pointers to bytes. Comparison is `memcmp(key_a, key_b, key_size)` (byte-wise; works для blittable POD key types per K-L3).

**Memcmp ordering caveat**: integers и enums work directly. Composite key types (e.g., `struct {uint32_t a; uint32_t b;}`) work if endianness consistent across save/load (it is — same machine). For cross-machine save sharing (post-game-launch concern, not K8.1), an explicit endianness convention may be needed; for now, memcmp ordering is acceptable.

### 1.4 — Detach-and-replace reallocation в Composite (LOCKED)

**Architecture**:

`Composite` represents variable-length data attached to a parent entity:
- Each composite-id maps to a `(parent_entity_id, count, std::vector<uint8_t>)` tuple.
- Element size is fixed at composite-id registration time (e.g., MovementComponent's path uses `sizeof(GridVector)` per element).
- Operations: `add_at_end`, `remove_at`, `get_count`, `get_at`, `clear`.

**Reallocation strategy** (LOCKED): detach-and-replace.
- On `add_at_end`, if `data_.size() < (count+1) * element_size`, allocate new vector с doubled capacity, copy existing content, free old. (`std::vector::push_back` already does this; we use it directly.)
- On `remove_at`, swap-with-last + pop. No realloc.
- No in-place grow (would require pre-allocating max capacity, wasteful for typical movement-path sizes).

**Trade-off accepted**: amortized O(1) push, O(log n) on removal-with-shifting alternative (we use swap-with-last for O(1) removal but loses ordering). Movement waypoints don't need order preservation (path is consumed front-to-back, not arbitrary-index access). Storage item list also doesn't need order preservation.

**Iteration order**: insertion order по умолчанию. `Composite` does not sort entries (unlike `KeyedMap`). For ordered iteration, caller can sort manually after `get_at` reads. Most use cases (movement path consumption, storage browsing) treat composite as an ordered list, not an ordered set.

### 1.5 — Sorted-by-element iteration в SetPrimitive (LOCKED)

**Architecture**:

`SetPrimitive` mirrors `KeyedMap` but без values:
- `std::vector<uint8_t> elements_` — flat sorted array of elements (each `element_size` bytes).
- `add`: binary search + insert if absent.
- `contains`: binary search.
- `remove`: binary search + erase + memmove to fill gap.
- `iterate`: linear scan, encounters elements в sorted order.

**Same determinism rationale as KeyedMap**: sorted iteration provides stable save/load roundtrip + cross-mod determinism.

**Use case**: Storage component has a `HashSet<EntityId>` for reservation tracking — which entities have «claim» on this storage. Replacing с `NativeSet<EntityId>` preserves semantic intent (membership test + iterate) с deterministic iteration order.

### 1.6 — World integration (LOCKED)

`World` becomes the orchestrator для primitive ownership:

```cpp
class World {
public:
    // ... existing K0-K6 methods ...

    // K8.1 — primitive accessors
    StringPool& string_pool() noexcept { return string_pool_; }
    KeyedMap*  get_or_create_keyed_map(uint32_t map_id, int32_t key_size, int32_t value_size);
    Composite* get_or_create_composite(uint32_t composite_id, int32_t element_size);
    SetPrimitive* get_or_create_set(uint32_t set_id, int32_t element_size);

    // K8.1 — mod scope orchestration
    void begin_mod_scope(const std::string& mod_id);
    void end_mod_scope(const std::string& mod_id);
    void clear_mod_scope(const std::string& mod_id);

    // ... existing methods ...

private:
    // ... existing fields ...

    // K8.1 — primitives
    StringPool string_pool_;
    std::unordered_map<uint32_t, std::unique_ptr<KeyedMap>> keyed_maps_;
    std::unordered_map<uint32_t, std::unique_ptr<Composite>> composites_;
    std::unordered_map<uint32_t, std::unique_ptr<SetPrimitive>> sets_;
    std::string current_mod_scope_;  // empty string = "core scope" (vanilla)
};
```

**Mod scope** is propagated through `string_pool_.begin_mod_scope` / `end_mod_scope` / `clear_mod_scope`. Future К8.x work may extend this к other primitives if needed (e.g., per-mod KeyedMaps), but K8.1's scope is string-specific because strings are the primitive с the strongest mod-ownership signal (mod-defined faction names, recipe names, etc.).

**`KeyedMap`/`Composite`/`SetPrimitive` ownership** is global to the World — they're keyed by stable `map_id`/`composite_id`/`set_id` (uint32_t), allocated by Caller (managed side via C ABI). This mirrors the `register_component_type` pattern already established в K2.

### 1.7 — C ABI surface (LOCKED)

New functions added to `df_capi.h`:

**String pool** (group of 8):
```c
DF_API uint32_t df_world_intern_string(df_world_handle, const char* utf8_data, int32_t utf8_length);
DF_API int32_t  df_world_resolve_string(df_world_handle, uint32_t string_id, uint32_t generation, char* out_buffer, int32_t out_buffer_size);
DF_API uint32_t df_world_string_generation(df_world_handle, uint32_t string_id);
DF_API void     df_world_begin_mod_scope(df_world_handle, const char* mod_id);
DF_API void     df_world_end_mod_scope(df_world_handle, const char* mod_id);
DF_API void     df_world_clear_mod_scope(df_world_handle, const char* mod_id);
DF_API int32_t  df_world_string_pool_count(df_world_handle);
DF_API uint32_t df_world_string_pool_current_generation(df_world_handle);
```

**Keyed map** (group of 7):
```c
DF_API df_keyed_map_handle df_world_get_keyed_map(df_world_handle, uint32_t map_id, int32_t key_size, int32_t value_size);
DF_API int32_t df_keyed_map_set(df_keyed_map_handle, const void* key, const void* value);
DF_API int32_t df_keyed_map_get(df_keyed_map_handle, const void* key, void* out_value);
DF_API int32_t df_keyed_map_remove(df_keyed_map_handle, const void* key);
DF_API int32_t df_keyed_map_count(df_keyed_map_handle);
DF_API int32_t df_keyed_map_iterate(df_keyed_map_handle, void* out_keys_buffer, void* out_values_buffer, int32_t buffer_capacity);
DF_API int32_t df_keyed_map_clear(df_keyed_map_handle);
```

`df_keyed_map_handle` is `void*` opaque, returned by `df_world_get_keyed_map`. Underlying lifetime owned by `World` — caller does not destroy.

**Composite** (group of 7):
```c
DF_API df_composite_handle df_world_get_composite(df_world_handle, uint32_t composite_id, int32_t element_size);
DF_API int32_t df_composite_add(df_composite_handle, uint64_t parent_entity, const void* element);
DF_API int32_t df_composite_get_count(df_composite_handle, uint64_t parent_entity);
DF_API int32_t df_composite_get_at(df_composite_handle, uint64_t parent_entity, int32_t index, void* out_element);
DF_API int32_t df_composite_remove_at(df_composite_handle, uint64_t parent_entity, int32_t index);
DF_API int32_t df_composite_clear_for(df_composite_handle, uint64_t parent_entity);
DF_API int32_t df_composite_iterate(df_composite_handle, uint64_t parent_entity, void* out_elements_buffer, int32_t buffer_capacity);
```

**Set primitive** (group of 6):
```c
DF_API df_set_handle df_world_get_set(df_world_handle, uint32_t set_id, int32_t element_size);
DF_API int32_t df_set_add(df_set_handle, const void* element);
DF_API int32_t df_set_contains(df_set_handle, const void* element);
DF_API int32_t df_set_remove(df_set_handle, const void* element);
DF_API int32_t df_set_count(df_set_handle);
DF_API int32_t df_set_iterate(df_set_handle, void* out_elements_buffer, int32_t buffer_capacity);
```

Total new C ABI functions: 28.

All follow existing conventions:
- `0` = failure / not found, `1` = success / present (consistent с existing functions)
- `int32_t` for counts and signed quantities
- `uint32_t` for IDs
- `uint64_t` for `EntityId` packed format (matches existing convention)
- Iteration functions return count of elements written (clipped to `buffer_capacity`)

### 1.8 — Managed bridge surface (LOCKED)

Four new public types в `DualFrontier.Core.Interop.Marshalling` namespace:

**`InternedString`** struct (in `InternedString.cs`):
```csharp
public readonly struct InternedString : IEquatable<InternedString>
{
    public uint Id { get; }
    public uint Generation { get; }

    internal InternedString(uint id, uint generation);

    /// <summary>True if this is the empty/uninitialized sentinel.</summary>
    public bool IsEmpty => Id == 0;

    /// <summary>Resolves to the actual string content. Returns null if the
    /// generation is stale (string evicted in a mod scope clear).</summary>
    public string? Resolve(NativeWorld world);

    public bool Equals(InternedString other) => Id == other.Id && Generation == other.Generation;
    public override bool Equals(object? obj) => obj is InternedString other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Id, Generation);
    public static bool operator ==(InternedString a, InternedString b) => a.Equals(b);
    public static bool operator !=(InternedString a, InternedString b) => !a.Equals(b);
}
```

Construction site: `NativeWorld.InternString(string content)` returns `InternedString`. The world tracks current mod scope and applies it.

**`NativeMap<TKey, TValue>`** wrapper (in `NativeMap.cs`):
```csharp
public sealed class NativeMap<TKey, TValue>
    where TKey : unmanaged, IComparable<TKey>
    where TValue : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _mapId;

    internal NativeMap(NativeWorld world, uint mapId);

    public int Count { get; }

    public bool TrySet(TKey key, TValue value);
    public bool TryGet(TKey key, out TValue value);
    public bool TryRemove(TKey key);
    public void Clear();

    /// <summary>Snapshots all entries in sorted-by-key iteration order.
    /// Allocates; for hot paths use <see cref="TryIterate"/>.</summary>
    public IReadOnlyList<KeyValuePair<TKey, TValue>> ToList();

    /// <summary>Writes up to <paramref name="bufferCapacity"/> entries into
    /// caller-provided buffers in sorted-by-key order. Returns count
    /// written (clipped). Zero allocations.</summary>
    public int TryIterate(Span<TKey> outKeys, Span<TValue> outValues);
}
```

The `IComparable<TKey>` constraint is for managed-side comparisons in tests / debug; native side uses memcmp.

Construction site: `NativeWorld.GetKeyedMap<TKey, TValue>(uint mapId)` — reuses K2 type registration pattern (ID assigned by caller, sized by struct sizeof).

**`NativeComposite<T>`** wrapper (in `NativeComposite.cs`):
```csharp
public sealed class NativeComposite<T> where T : unmanaged
{
    private readonly NativeWorld _world;
    private readonly uint _compositeId;

    internal NativeComposite(NativeWorld world, uint compositeId);

    public int CountFor(EntityId parent);
    public bool TryAdd(EntityId parent, T element);
    public bool TryGetAt(EntityId parent, int index, out T element);
    public bool TryRemoveAt(EntityId parent, int index);
    public void ClearFor(EntityId parent);
    public int TryIterate(EntityId parent, Span<T> outBuffer);
}
```

Construction: `NativeWorld.GetComposite<T>(uint compositeId)`.

**`NativeSet<T>`** wrapper (in `NativeSet.cs`):
```csharp
public sealed class NativeSet<T>
    where T : unmanaged, IComparable<T>
{
    private readonly NativeWorld _world;
    private readonly uint _setId;

    internal NativeSet(NativeWorld world, uint setId);

    public int Count { get; }

    public bool TryAdd(T element);
    public bool Contains(T element);
    public bool TryRemove(T element);
    public int TryIterate(Span<T> outBuffer);
}
```

Construction: `NativeWorld.GetSet<T>(uint setId)`.

### 1.9 — Selftest scenarios (LOCKED)

Each primitive has its selftest section в `selftest.cpp` with these scenarios:

**StringPool selftest** (5 scenarios):
1. Round-trip: intern «Foo», resolve by ID + generation → returns «Foo».
2. Dedup: intern «Foo» twice → same ID returned.
3. Cross-mod sharing: ModA interns «Foo», ModB interns «Foo» → same ID, both mods own it.
4. Mod scope clear с retained reference: ModA's «Foo» referenced by ModB → after clear ModA, «Foo» still resolvable, generation unchanged.
5. Mod scope clear с unique string: ModA's «UniqueA» not referenced by anyone else → after clear ModA, ID reclaimable for new string, generation incremented.

**KeyedMap selftest** (4 scenarios):
1. Round-trip: set (1, 100), set (2, 200), get(1) → 100, get(2) → 200.
2. Sorted iteration: set (5, ...), set (1, ...), set (3, ...) — iterate returns в order [1, 3, 5].
3. Update: set (1, 100), set (1, 200), get(1) → 200, count = 1.
4. Remove + iterate: set (1,...), (2,...), (3,...), remove(2), iterate → [1, 3].

**Composite selftest** (4 scenarios):
1. Round-trip: add element to entity 42, get_at(42, 0) returns element.
2. Multi-element insertion order: add A, add B, add C → iterate returns [A, B, C].
3. Remove-at swaps-with-last: add A, B, C; remove_at(0); count=2; iterate → [C, B] (insertion order preserved для non-removed).
4. Multi-entity isolation: entity 42's composite independent of entity 99's composite.

**SetPrimitive selftest** (4 scenarios):
1. Round-trip: add 5, add 3, contains(5) → 1, contains(3) → 1, contains(7) → 0.
2. Dedup: add 5, add 5 → count = 1.
3. Sorted iteration: add 5, add 1, add 3 → iterate returns [1, 3, 5].
4. Remove + iterate: add 1, 2, 3, 4; remove(2); iterate → [1, 3, 4].

### 1.10 — Bridge equivalence tests (LOCKED)

Each managed bridge wrapper has tests verifying behavioral parity с corresponding managed BCL type:

**`InternedStringTests.cs`** — round-trip + equality semantics + null content sentinel
**`NativeMapTests.cs`** — equivalence vs `Dictionary<TKey, TValue>` (set/get/remove/count); plus iteration order test specifically calling out что NativeMap iterates sorted, Dictionary doesn't (but K8.1 `NativeMap.ToList()` returns sorted, vs Dictionary returns hash-order)
**`NativeCompositeTests.cs`** — equivalence vs `List<T>` for add/iterate; semantic difference acknowledged (NativeComposite doesn't preserve order across removes — uses swap-with-last; tests verify this explicitly)
**`NativeSetTests.cs`** — equivalence vs `HashSet<T>` for add/contains/remove/count; plus iteration order test (NativeSet iterates sorted)

Total new bridge tests: ~30 tests across 4 files.

### 1.11 — CMakeLists.txt integration (LOCKED)

The current `CMakeLists.txt` uses globs или explicit listing для sources — read the file at Phase 0.5 and adapt accordingly:
- If glob-based: K8.1 source files automatically picked up; no edit needed.
- If explicit: K8.1 adds the four new `.cpp` files в both targets (`DualFrontier.Core.Native` shared lib и `df_native_selftest` executable).

### 1.12 — Test count expectation

K8.1 baseline: 553 tests (post-K8.0).

К8.1 adds:
- Native selftest scenarios: 17 (counted в §1.9). These run via `df_native_selftest.exe` not via `dotnet test`; not counted в test count.
- Managed bridge tests: ~30 (per §1.10).

Expected post-K8.1 test count: **583** (553 + 30 = 583, ±2 for organic variance).

---

## Phase 2 — Native primitive implementation

### 2.1 — `string_pool.h` + `string_pool.cpp`

**File**: `native/DualFrontier.Core.Native/include/string_pool.h` (NEW)

**Content**:

```cpp
#pragma once

#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

namespace dualfrontier {

// String pool with generational mod-scoped interning.
//
// IDs are uint32_t. Generation tags are uint32_t. ID 0 reserved for
// "empty" sentinel. Generation 0 reserved for "uninitialized".
//
// Mod scope semantics:
//   - begin_mod_scope("ModX") before any intern() calls from ModX's code path.
//   - end_mod_scope("ModX") closes the scope (intern calls outside scope land
//     in "core" scope, mod_id = "").
//   - clear_mod_scope("ModX") releases all IDs uniquely owned by ModX. IDs
//     also referenced by other mods stay alive. Reclaimed IDs get their
//     generation incremented; future interns may reuse the slot.
//
// Save/load: pool itself is NOT serialized. Components serialize string
// CONTENT, not IDs. Load re-interns content; fresh ID/generation pair
// returned. Generation tag protects against stale-ID reads if a save
// tries to use an ID that's been reclaimed.
class StringPool {
public:
    static constexpr uint32_t kEmptyId = 0;
    static constexpr uint32_t kUninitializedGeneration = 0;

    StringPool();

    // Intern a string. If already present, returns existing ID with current
    // generation. If new, allocates new ID. The string is recorded as
    // owned by current_mod_scope_.
    [[nodiscard]] uint32_t intern(const std::string& content);

    // Resolve an ID to its content. Returns nullptr if generation mismatch
    // (stale reference) or ID out of range.
    [[nodiscard]] const std::string* resolve(uint32_t id, uint32_t expected_generation) const noexcept;

    // Returns the current generation tag for the given ID. Used by callers
    // who hold an ID and want to learn its current generation (e.g., a
    // managed-side snapshot rebuild after a mod unload).
    [[nodiscard]] uint32_t generation_for(uint32_t id) const noexcept;

    // Mod scope orchestration.
    void begin_mod_scope(const std::string& mod_id);
    void end_mod_scope(const std::string& mod_id);
    void clear_mod_scope(const std::string& mod_id);

    [[nodiscard]] int32_t count() const noexcept;
    [[nodiscard]] uint32_t current_generation() const noexcept { return current_generation_; }
    [[nodiscard]] const std::string& current_mod_scope() const noexcept { return current_mod_scope_; }

private:
    // contents_[id] is the string for that ID; index 0 reserved (empty).
    std::vector<std::string> contents_;
    // generation_per_id_[id] is the current generation tag for that ID.
    std::vector<uint32_t> generation_per_id_;
    // lookup_[content] -> id for dedup.
    std::unordered_map<std::string, uint32_t> lookup_;
    // ids_by_mod_[mod_id] -> vector of IDs interned while that mod's scope
    // was active.
    std::unordered_map<std::string, std::vector<uint32_t>> ids_by_mod_;
    // free_ids_ — IDs whose contents were reclaimed and can be reused.
    std::vector<uint32_t> free_ids_;

    std::string current_mod_scope_;
    uint32_t current_generation_;

    // Helper: reclaim an ID, increment its generation, push to free_ids_.
    void reclaim_id(uint32_t id);
};

} // namespace dualfrontier
```

**File**: `native/DualFrontier.Core.Native/src/string_pool.cpp` (NEW)

**Content**: implementation of `StringPool` per the header. Key methods:

```cpp
#include "string_pool.h"
#include <stdexcept>

namespace dualfrontier {

StringPool::StringPool()
    : contents_(1, std::string{})  // index 0 reserved
    , generation_per_id_(1, kUninitializedGeneration)
    , current_generation_(1)
{
}

uint32_t StringPool::intern(const std::string& content) {
    if (content.empty()) {
        return kEmptyId;
    }

    auto it = lookup_.find(content);
    if (it != lookup_.end()) {
        // Already interned. Track ownership in current mod scope (idempotent insert).
        uint32_t id = it->second;
        auto& mod_ids = ids_by_mod_[current_mod_scope_];
        if (std::find(mod_ids.begin(), mod_ids.end(), id) == mod_ids.end()) {
            mod_ids.push_back(id);
        }
        return id;
    }

    // New string. Reuse a freed ID if available.
    uint32_t id;
    if (!free_ids_.empty()) {
        id = free_ids_.back();
        free_ids_.pop_back();
        contents_[id] = content;
    } else {
        id = static_cast<uint32_t>(contents_.size());
        contents_.push_back(content);
        generation_per_id_.push_back(current_generation_);
    }
    generation_per_id_[id] = current_generation_;
    lookup_[content] = id;
    ids_by_mod_[current_mod_scope_].push_back(id);
    return id;
}

const std::string* StringPool::resolve(uint32_t id, uint32_t expected_generation) const noexcept {
    if (id == kEmptyId || id >= contents_.size()) {
        return nullptr;
    }
    if (generation_per_id_[id] != expected_generation) {
        return nullptr;  // stale reference
    }
    return &contents_[id];
}

uint32_t StringPool::generation_for(uint32_t id) const noexcept {
    if (id == kEmptyId || id >= generation_per_id_.size()) {
        return kUninitializedGeneration;
    }
    return generation_per_id_[id];
}

void StringPool::begin_mod_scope(const std::string& mod_id) {
    current_mod_scope_ = mod_id;
    // Ensure entry exists для предсказуемых iterations.
    ids_by_mod_.try_emplace(mod_id);
}

void StringPool::end_mod_scope(const std::string& mod_id) {
    if (current_mod_scope_ != mod_id) {
        throw std::logic_error("end_mod_scope called for mod_id != current scope");
    }
    current_mod_scope_.clear();  // back to "core" scope
}

void StringPool::clear_mod_scope(const std::string& mod_id) {
    auto it = ids_by_mod_.find(mod_id);
    if (it == ids_by_mod_.end()) return;

    for (uint32_t id : it->second) {
        // Check if any other mod still references this ID.
        bool referenced_elsewhere = false;
        for (const auto& [other_mod, other_ids] : ids_by_mod_) {
            if (other_mod == mod_id) continue;
            if (std::find(other_ids.begin(), other_ids.end(), id) != other_ids.end()) {
                referenced_elsewhere = true;
                break;
            }
        }
        if (!referenced_elsewhere) {
            reclaim_id(id);
        }
    }
    ids_by_mod_.erase(it);
}

void StringPool::reclaim_id(uint32_t id) {
    if (id == kEmptyId || id >= contents_.size()) return;

    lookup_.erase(contents_[id]);
    contents_[id].clear();
    current_generation_++;
    generation_per_id_[id] = current_generation_;
    free_ids_.push_back(id);
}

int32_t StringPool::count() const noexcept {
    return static_cast<int32_t>(contents_.size() - 1 - free_ids_.size());
}

} // namespace dualfrontier
```

**Atomic commit**:
```
feat(native): add StringPool with generational mod-scoped interning
```

### 2.2 — `keyed_map.h` + `keyed_map.cpp`

**File**: `native/DualFrontier.Core.Native/include/keyed_map.h` (NEW)

**Content**:

```cpp
#pragma once

#include <cstdint>
#include <vector>

namespace dualfrontier {

// Type-erased keyed map с sorted-by-key iteration.
//
// Sorting is byte-wise (memcmp on key_size bytes). Works deterministically
// for blittable POD key types (uint32_t, EntityId, enum types). Composite
// key types работают if endianness consistent (single-machine assumption
// holds for current scope).
//
// Operations:
//   - set(key, value): O(log n) lookup + O(n) memmove if insert is needed.
//   - get(key): O(log n).
//   - remove(key): O(log n) + O(n) memmove.
//   - iterate: linear scan, encounters keys в sorted order.
class KeyedMap {
public:
    KeyedMap(int32_t key_size, int32_t value_size);

    // Returns 1 if newly inserted, 0 if updated (key already present).
    int32_t set(const void* key, const void* value);

    // Returns 1 if found, 0 otherwise. On success copies value into out_value.
    int32_t get(const void* key, void* out_value) const noexcept;

    // Returns 1 if removed, 0 if not present.
    int32_t remove(const void* key);

    // Returns count of entries.
    [[nodiscard]] int32_t count() const noexcept { return count_; }

    // Writes up to buffer_capacity entries into out_keys и out_values.
    // Iteration is sorted-by-key. Returns count written.
    int32_t iterate(void* out_keys, void* out_values, int32_t buffer_capacity) const noexcept;

    // Removes all entries.
    int32_t clear() noexcept;

private:
    int32_t key_size_;
    int32_t value_size_;
    int32_t count_;
    std::vector<uint8_t> keys_;     // count_ * key_size_ bytes, sorted
    std::vector<uint8_t> values_;   // count_ * value_size_ bytes, parallel

    // Returns index where key is found, or -(insert_position + 1) if absent.
    int32_t binary_search(const void* key) const noexcept;
};

} // namespace dualfrontier
```

**File**: `native/DualFrontier.Core.Native/src/keyed_map.cpp` (NEW)

**Content**: implementation per the header. Key methods:

```cpp
#include "keyed_map.h"
#include <cstring>

namespace dualfrontier {

KeyedMap::KeyedMap(int32_t key_size, int32_t value_size)
    : key_size_(key_size), value_size_(value_size), count_(0) {}

int32_t KeyedMap::binary_search(const void* key) const noexcept {
    int32_t lo = 0, hi = count_;
    while (lo < hi) {
        int32_t mid = (lo + hi) / 2;
        int cmp = std::memcmp(keys_.data() + mid * key_size_, key, key_size_);
        if (cmp == 0) return mid;
        if (cmp < 0) lo = mid + 1;
        else hi = mid;
    }
    return -(lo + 1);
}

int32_t KeyedMap::set(const void* key, const void* value) {
    int32_t idx = binary_search(key);
    if (idx >= 0) {
        // Update existing.
        std::memcpy(values_.data() + idx * value_size_, value, value_size_);
        return 0;
    }
    int32_t insert_pos = -(idx + 1);
    // Insert. Grow vectors first.
    keys_.resize((count_ + 1) * key_size_);
    values_.resize((count_ + 1) * value_size_);
    // Memmove existing entries to make room.
    if (insert_pos < count_) {
        std::memmove(keys_.data() + (insert_pos + 1) * key_size_,
                     keys_.data() + insert_pos * key_size_,
                     (count_ - insert_pos) * key_size_);
        std::memmove(values_.data() + (insert_pos + 1) * value_size_,
                     values_.data() + insert_pos * value_size_,
                     (count_ - insert_pos) * value_size_);
    }
    std::memcpy(keys_.data() + insert_pos * key_size_, key, key_size_);
    std::memcpy(values_.data() + insert_pos * value_size_, value, value_size_);
    count_++;
    return 1;
}

int32_t KeyedMap::get(const void* key, void* out_value) const noexcept {
    int32_t idx = binary_search(key);
    if (idx < 0) return 0;
    std::memcpy(out_value, values_.data() + idx * value_size_, value_size_);
    return 1;
}

int32_t KeyedMap::remove(const void* key) {
    int32_t idx = binary_search(key);
    if (idx < 0) return 0;
    if (idx < count_ - 1) {
        std::memmove(keys_.data() + idx * key_size_,
                     keys_.data() + (idx + 1) * key_size_,
                     (count_ - idx - 1) * key_size_);
        std::memmove(values_.data() + idx * value_size_,
                     values_.data() + (idx + 1) * value_size_,
                     (count_ - idx - 1) * value_size_);
    }
    count_--;
    keys_.resize(count_ * key_size_);
    values_.resize(count_ * value_size_);
    return 1;
}

int32_t KeyedMap::iterate(void* out_keys, void* out_values, int32_t buffer_capacity) const noexcept {
    int32_t to_write = (count_ < buffer_capacity) ? count_ : buffer_capacity;
    if (to_write > 0) {
        std::memcpy(out_keys, keys_.data(), to_write * key_size_);
        std::memcpy(out_values, values_.data(), to_write * value_size_);
    }
    return to_write;
}

int32_t KeyedMap::clear() noexcept {
    int32_t prev = count_;
    count_ = 0;
    keys_.clear();
    values_.clear();
    return prev;
}

} // namespace dualfrontier
```

**Atomic commit**:
```
feat(native): add KeyedMap with sorted-by-key iteration
```

### 2.3 — `composite.h` + `composite.cpp`

**File**: `native/DualFrontier.Core.Native/include/composite.h` (NEW)

**Content**:

```cpp
#pragma once

#include <cstdint>
#include <unordered_map>
#include <vector>

#include "entity_id.h"

namespace dualfrontier {

// Variable-length data attached to a parent entity. Each parent's data is
// stored in a separate vector keyed by entity (encoded as uint64_t).
//
// Element size fixed at construction time. Operations are per-parent-entity:
//   - add: append element to parent's vector (amortized O(1)).
//   - remove_at: swap-with-last + pop O(1). Order NOT preserved across removes.
//   - get_at: O(1) random access.
//   - iterate: O(count) linear over parent's vector.
//   - clear_for: O(1) (drops the parent's vector).
//
// Insertion order preserved для non-removed elements. After remove_at(i),
// the element at index i is replaced with what was at the end. This is
// the "swap-with-last" pattern; suitable for cases где order doesn't matter
// (movement waypoints consumed front-to-back, storage item lists иterated
// без stable-order requirement).
class Composite {
public:
    explicit Composite(int32_t element_size);

    int32_t add(uint64_t parent, const void* element);
    int32_t get_count(uint64_t parent) const noexcept;
    int32_t get_at(uint64_t parent, int32_t index, void* out_element) const noexcept;
    int32_t remove_at(uint64_t parent, int32_t index);
    int32_t clear_for(uint64_t parent);
    int32_t iterate(uint64_t parent, void* out_buffer, int32_t buffer_capacity) const noexcept;

private:
    int32_t element_size_;
    std::unordered_map<uint64_t, std::vector<uint8_t>> data_per_parent_;
};

} // namespace dualfrontier
```

**File**: `native/DualFrontier.Core.Native/src/composite.cpp` (NEW)

**Content**: implementation per header. Key methods:

```cpp
#include "composite.h"
#include <cstring>

namespace dualfrontier {

Composite::Composite(int32_t element_size) : element_size_(element_size) {}

int32_t Composite::add(uint64_t parent, const void* element) {
    auto& vec = data_per_parent_[parent];
    size_t old_size = vec.size();
    vec.resize(old_size + element_size_);
    std::memcpy(vec.data() + old_size, element, element_size_);
    return 1;
}

int32_t Composite::get_count(uint64_t parent) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) return 0;
    return static_cast<int32_t>(it->second.size() / element_size_);
}

int32_t Composite::get_at(uint64_t parent, int32_t index, void* out_element) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) return 0;
    int32_t count = static_cast<int32_t>(it->second.size() / element_size_);
    if (index < 0 || index >= count) return 0;
    std::memcpy(out_element, it->second.data() + index * element_size_, element_size_);
    return 1;
}

int32_t Composite::remove_at(uint64_t parent, int32_t index) {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) return 0;
    int32_t count = static_cast<int32_t>(it->second.size() / element_size_);
    if (index < 0 || index >= count) return 0;
    int32_t last_index = count - 1;
    if (index != last_index) {
        std::memcpy(it->second.data() + index * element_size_,
                    it->second.data() + last_index * element_size_,
                    element_size_);
    }
    it->second.resize(last_index * element_size_);
    return 1;
}

int32_t Composite::clear_for(uint64_t parent) {
    return data_per_parent_.erase(parent) > 0 ? 1 : 0;
}

int32_t Composite::iterate(uint64_t parent, void* out_buffer, int32_t buffer_capacity) const noexcept {
    auto it = data_per_parent_.find(parent);
    if (it == data_per_parent_.end()) return 0;
    int32_t count = static_cast<int32_t>(it->second.size() / element_size_);
    int32_t to_write = (count < buffer_capacity) ? count : buffer_capacity;
    if (to_write > 0) {
        std::memcpy(out_buffer, it->second.data(), to_write * element_size_);
    }
    return to_write;
}

} // namespace dualfrontier
```

**Atomic commit**:
```
feat(native): add Composite for per-entity variable-length data
```

### 2.4 — `set_primitive.h` + `set_primitive.cpp`

**File**: `native/DualFrontier.Core.Native/include/set_primitive.h` (NEW)

**Content**:

```cpp
#pragma once

#include <cstdint>
#include <vector>

namespace dualfrontier {

// Type-erased keyed set с sorted-by-element iteration.
//
// Sorting is byte-wise (memcmp on element_size bytes). Mirror of KeyedMap
// without value half. Operations:
//   - add: O(log n) lookup + O(n) memmove if insert.
//   - contains: O(log n).
//   - remove: O(log n) + O(n) memmove.
//   - iterate: linear scan, encounters elements в sorted order.
class SetPrimitive {
public:
    explicit SetPrimitive(int32_t element_size);

    int32_t add(const void* element);
    int32_t contains(const void* element) const noexcept;
    int32_t remove(const void* element);
    [[nodiscard]] int32_t count() const noexcept { return count_; }
    int32_t iterate(void* out_buffer, int32_t buffer_capacity) const noexcept;

private:
    int32_t element_size_;
    int32_t count_;
    std::vector<uint8_t> elements_;

    int32_t binary_search(const void* element) const noexcept;
};

} // namespace dualfrontier
```

**File**: `native/DualFrontier.Core.Native/src/set_primitive.cpp` (NEW)

**Content**: implementation analogous to KeyedMap but без value array. Methods follow the same memmove pattern.

**Atomic commit**:
```
feat(native): add SetPrimitive with sorted-by-element iteration
```

### 2.5 — Native build verification

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
```

Verify all four primitives compile cleanly. Selftest scenarios will be added in Phase 4 (not yet — current selftest still passes на baseline).

**Halt condition**: any compilation error. The `unordered_map<>` and `vector<>` patterns are well-trodden; failures likely indicate a typo or include issue.

---

## Phase 3 — World integration

### 3.1 — Add primitives to `World` declaration

**File**: `native/DualFrontier.Core.Native/include/world.h`

**Edit**: add includes:

```cpp
#include "string_pool.h"
#include "keyed_map.h"
#include "composite.h"
#include "set_primitive.h"
```

**Edit**: in the `World` class public section, after the existing K6 / K2 methods, before the `private:` line, add the K8.1 primitive accessors per §1.6 design contract:

```cpp
    // K8.1 — primitive accessors and mod scope orchestration
    StringPool& string_pool() noexcept { return string_pool_; }
    const StringPool& string_pool() const noexcept { return string_pool_; }
    KeyedMap*  get_or_create_keyed_map(uint32_t map_id, int32_t key_size, int32_t value_size);
    Composite* get_or_create_composite(uint32_t composite_id, int32_t element_size);
    SetPrimitive* get_or_create_set(uint32_t set_id, int32_t element_size);
    void begin_mod_scope(const std::string& mod_id);
    void end_mod_scope(const std::string& mod_id);
    void clear_mod_scope(const std::string& mod_id);
```

**Edit**: in `private:` section, add fields:

```cpp
    // K8.1 — primitives
    StringPool string_pool_;
    std::unordered_map<uint32_t, std::unique_ptr<KeyedMap>> keyed_maps_;
    std::unordered_map<uint32_t, std::unique_ptr<Composite>> composites_;
    std::unordered_map<uint32_t, std::unique_ptr<SetPrimitive>> sets_;
```

### 3.2 — Implement primitive accessors в `world.cpp`

**File**: `native/DualFrontier.Core.Native/src/world.cpp`

**Edit**: add includes if not present (`#include "string_pool.h"` etc.).

**Edit**: append implementations at end of file:

```cpp
KeyedMap* World::get_or_create_keyed_map(uint32_t map_id, int32_t key_size, int32_t value_size) {
    if (map_id == 0) return nullptr;
    auto it = keyed_maps_.find(map_id);
    if (it != keyed_maps_.end()) {
        return it->second.get();
    }
    auto [inserted, _] = keyed_maps_.emplace(map_id, std::make_unique<KeyedMap>(key_size, value_size));
    return inserted->second.get();
}

Composite* World::get_or_create_composite(uint32_t composite_id, int32_t element_size) {
    if (composite_id == 0) return nullptr;
    auto it = composites_.find(composite_id);
    if (it != composites_.end()) {
        return it->second.get();
    }
    auto [inserted, _] = composites_.emplace(composite_id, std::make_unique<Composite>(element_size));
    return inserted->second.get();
}

SetPrimitive* World::get_or_create_set(uint32_t set_id, int32_t element_size) {
    if (set_id == 0) return nullptr;
    auto it = sets_.find(set_id);
    if (it != sets_.end()) {
        return it->second.get();
    }
    auto [inserted, _] = sets_.emplace(set_id, std::make_unique<SetPrimitive>(element_size));
    return inserted->second.get();
}

void World::begin_mod_scope(const std::string& mod_id) {
    string_pool_.begin_mod_scope(mod_id);
}

void World::end_mod_scope(const std::string& mod_id) {
    string_pool_.end_mod_scope(mod_id);
}

void World::clear_mod_scope(const std::string& mod_id) {
    string_pool_.clear_mod_scope(mod_id);
}
```

### 3.3 — Native build verification

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
```

Verify all changes compile cleanly. Selftest still passes на baseline (selftest scenarios for primitives will be added в Phase 4).

**Atomic commit** (Phase 3 bundled — single commit since edits are tightly coupled):
```
feat(native): integrate K8.1 primitives into World with mod scope orchestration
```

---

## Phase 4 — C ABI surface

### 4.1 — Extend `df_capi.h` with K8.1 sections

**File**: `native/DualFrontier.Core.Native/include/df_capi.h`

**Edit**: at end of file, before `#ifdef __cplusplus` closing brace, add three sections per §1.7 design contract.

For each section: include a documentation block similar to existing K1/K5 styles (Lifecycle, Returns, etc.), then declare the functions.

The full text is per §1.7 with full signatures. Place the StringPool group first, then KeyedMap, then Composite, then SetPrimitive. Each function declaration follows existing `DF_API` convention.

(For brevity, the executor reads §1.7 and transcribes the signatures into header. Documentation blocks are written by the executor to match the Lifecycle / Returns style of existing K5 documentation block.)

### 4.2 — Implement C ABI in `capi.cpp`

**File**: `native/DualFrontier.Core.Native/src/capi.cpp`

**Edit**: at end of file, before any closing `extern "C"` brace, add implementations for each function declared in §4.1. Pattern для each function:

1. Cast `df_world_handle` → `World*`.
2. If null check fails, return 0.
3. Wrap call in try-catch (catch `std::exception` → return 0; ABI must not unwind through C boundary).
4. Forward to corresponding World/primitive method.

Example (for `df_world_intern_string`):

```cpp
DF_API uint32_t df_world_intern_string(df_world_handle world,
                                        const char* utf8_data,
                                        int32_t utf8_length) {
    if (!world || !utf8_data || utf8_length < 0) return 0;
    try {
        World* w = static_cast<World*>(world);
        std::string content(utf8_data, utf8_length);
        return w->string_pool().intern(content);
    } catch (...) {
        return 0;
    }
}
```

(Pattern repeats for all 28 functions; brief leaves boilerplate to executor.)

**Atomic commit** (Phase 4 bundled — C ABI is single logical unit):
```
feat(native): add K8.1 C ABI surface (28 functions for 4 primitives)
```

### 4.3 — Native build verification + selftest

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
```

Verify build clean, baseline selftest still passes.

**Halt condition**: any compilation error or selftest regression.

---

## Phase 5 — Managed bridge

### 5.1 — `InternedString.cs`

**File**: `src/DualFrontier.Core.Interop/Marshalling/InternedString.cs` (NEW)

**Content**: per §1.8 design contract. Full struct implementation including `Resolve(NativeWorld)` method.

**Atomic commit**:
```
feat(interop): add InternedString managed bridge type
```

### 5.2 — `NativeMap.cs`

**File**: `src/DualFrontier.Core.Interop/Marshalling/NativeMap.cs` (NEW)

**Content**: per §1.8. Wrapper around `df_keyed_map_handle`.

**Atomic commit**:
```
feat(interop): add NativeMap<TKey, TValue> wrapper for KeyedMap
```

### 5.3 — `NativeComposite.cs`

**File**: `src/DualFrontier.Core.Interop/Marshalling/NativeComposite.cs` (NEW)

**Content**: per §1.8. Wrapper around `df_composite_handle`.

**Atomic commit**:
```
feat(interop): add NativeComposite<T> wrapper for Composite
```

### 5.4 — `NativeSet.cs`

**File**: `src/DualFrontier.Core.Interop/Marshalling/NativeSet.cs` (NEW)

**Content**: per §1.8. Wrapper around `df_set_handle`.

**Atomic commit**:
```
feat(interop): add NativeSet<T> wrapper for SetPrimitive
```

### 5.5 — Extend `NativeWorld` with primitive accessors

**File**: `src/DualFrontier.Core.Interop/NativeWorld.cs`

**Edit**: add public methods on `NativeWorld`:

```csharp
public InternedString InternString(string content) { /* P/Invoke df_world_intern_string */ }
public NativeMap<TKey, TValue> GetKeyedMap<TKey, TValue>(uint mapId)
    where TKey : unmanaged, IComparable<TKey>
    where TValue : unmanaged
{ /* P/Invoke df_world_get_keyed_map, return wrapper */ }
public NativeComposite<T> GetComposite<T>(uint compositeId) where T : unmanaged
{ /* P/Invoke df_world_get_composite, return wrapper */ }
public NativeSet<T> GetSet<T>(uint setId)
    where T : unmanaged, IComparable<T>
{ /* P/Invoke df_world_get_set, return wrapper */ }
public void BeginModScope(string modId) { /* P/Invoke */ }
public void EndModScope(string modId) { /* P/Invoke */ }
public void ClearModScope(string modId) { /* P/Invoke */ }
```

**Atomic commit**:
```
feat(interop): NativeWorld exposes K8.1 primitive accessors
```

### 5.6 — Build verification

```
dotnet build
```

Verify managed build clean. P/Invoke signatures match C ABI declared в Phase 4.

**Halt condition**: any compilation error. Likely cause: P/Invoke signature mismatch с C ABI; cross-check `df_capi.h` declaration vs `[DllImport]` attribute literal-string.

---

## Phase 6 — Native selftest scenarios

### 6.1 — Extend `selftest.cpp`

**File**: `native/DualFrontier.Core.Native/test/selftest.cpp`

**Edit**: at end of file, before `int main()` (or wherever existing test sections are), add four new test sections:

- `test_string_pool` — 5 scenarios per §1.9
- `test_keyed_map` — 4 scenarios
- `test_composite` — 4 scenarios
- `test_set_primitive` — 4 scenarios

Each scenario uses `assert(...)` (or whatever test macro the existing selftest uses — read selftest.cpp at Phase 0 to confirm). Each section prints `[K8.1] <primitive> selftest passed` to stdout on success.

**Atomic commit**:
```
test(native): add K8.1 primitive selftest scenarios (17 total)
```

### 6.2 — Run native selftest

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
```

**Expected output**: K0/K1/K2/K3/K5 selftest output preserved; new K8.1 sections all print «passed».

**Halt condition**: any selftest scenario fails. Diagnose against the corresponding primitive's implementation; do not weaken the test.

---

## Phase 7 — Bridge equivalence tests

### 7.1 — `InternedStringTests.cs`

**File**: `tests/DualFrontier.Core.Interop.Tests/Marshalling/InternedStringTests.cs` (NEW)

**Content**: ~7 tests:
- Round-trip: intern, resolve, content matches
- Equality: same ID + generation → equal
- Equality: different ID → not equal
- Empty sentinel: `default(InternedString).IsEmpty` true
- Stale resolution: clear mod scope, resolve old ID → null
- Cross-mod sharing: ModA interns, ModB interns same content → same ID
- Generation increment after reclaim

### 7.2 — `NativeMapTests.cs`

**File**: `tests/DualFrontier.Core.Interop.Tests/Marshalling/NativeMapTests.cs` (NEW)

**Content**: ~10 tests covering set/get/remove/count/iterate equivalence vs `Dictionary<TKey, TValue>` plus iteration order verification.

### 7.3 — `NativeCompositeTests.cs`

**File**: `tests/DualFrontier.Core.Interop.Tests/Marshalling/NativeCompositeTests.cs` (NEW)

**Content**: ~7 tests covering add/iterate/remove/clear equivalence vs `List<T>` (plus explicit acknowledgement что `RemoveAt` doesn't preserve order).

### 7.4 — `NativeSetTests.cs`

**File**: `tests/DualFrontier.Core.Interop.Tests/Marshalling/NativeSetTests.cs` (NEW)

**Content**: ~6 tests covering add/contains/remove/count/iterate equivalence vs `HashSet<T>` plus iteration order verification.

**Atomic commit** (Phase 7 bundled per primitive):
```
test(interop): add K8.1 bridge equivalence tests for InternedString
test(interop): add K8.1 bridge equivalence tests for NativeMap
test(interop): add K8.1 bridge equivalence tests for NativeComposite
test(interop): add K8.1 bridge equivalence tests for NativeSet
```

### 7.5 — Build + test gate

```
dotnet build
dotnet test
```

**Expected**: 0 errors, 0 warnings, **583 tests passing** (553 baseline + ~30 new).

**Halt condition**: any test failure. Bridge-equivalence test failures are diagnostic — they highlight specific behavioral divergence which must be reconciled.

---

## Phase 8 — CMakeLists.txt integration

### 8.1 — Verify glob vs explicit listing

**File**: `native/DualFrontier.Core.Native/CMakeLists.txt`

**Action**: read the file. If sources are listed explicitly (e.g., `add_library(DualFrontier.Core.Native SHARED src/world.cpp src/capi.cpp ...)`), add the four new K8.1 source files. If glob-based (`file(GLOB SOURCES src/*.cpp)`), no edit needed; new files автоматически picked up.

Same check for `df_native_selftest` target.

**Atomic commit** (only if edit needed):
```
build(native): add K8.1 source files to CMakeLists.txt
```

If glob-based, no commit; phase is verification-only.

---

## Phase 9 — Closure documentation

### 9.1 — Update `MIGRATION_PROGRESS.md`

**File**: `docs/MIGRATION_PROGRESS.md`

**Edit 1**: K-series Overview table, K8.1 row:

`| K8.1 | Native-side reference handling primitives | NOT STARTED | 1-2 weeks | — | — |`

→

`| K8.1 | Native-side reference handling primitives | DONE | <commit SHA range> | <date> |`

**Edit 2**: Add K8.1 closure section after K8.0:

```markdown
### K8.1 — Native-side reference handling primitives

- **Status**: DONE (`<commit SHA range>`, <date>)
- **Brief**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md` (FULL EXECUTED)
- **Goal**: Foundation primitives для K8.2 component redesigns. Four reference primitives now available on the native side: `StringPool` (generational mod-scoped interning), `KeyedMap` (sorted-by-key map), `Composite` (per-entity variable-length data), `SetPrimitive` (sorted-by-element set).
- **Deliverables**:
  - `string_pool.h/cpp` (generational pool; per-mod scopes via begin/end/clear)
  - `keyed_map.h/cpp` (type-erased; binary-search ops; sorted iteration)
  - `composite.h/cpp` (per-entity data; swap-with-last removal)
  - `set_primitive.h/cpp` (type-erased; sorted iteration)
  - 28 new C ABI functions in `df_capi.h` / `capi.cpp`
  - Managed bridge: `InternedString`, `NativeMap<TKey, TValue>`, `NativeComposite<T>`, `NativeSet<T>` в `Marshalling`
  - 17 native selftest scenarios across 4 primitives
  - ~30 managed bridge equivalence tests
- **Test count**: 553 → <new count> (~583)
- **Lessons learned**:
  - <fill in based on actual execution surprises>
  - Generational mod-scope tracking handles the K-L9 «vanilla = mods» principle natively — strings owned by core stay alive across mod loads/unloads, but mod-specific strings reclaim deterministically.
  - Sorted-by-key iteration for KeyedMap and SetPrimitive provides save/load determinism out of the box; insertion-order or hash-order alternatives would have created roundtrip drift.
```

**Edit 3**: Update `Current state snapshot`:
- `Active phase`: K8.1 → K9 (per K8.0 sequencing decision Option c)
- `Last completed milestone`: K8.0 → K8.1
- `Tests passing`: 553 → <new count>
- `Last updated`: 2026-05-09 (K8.0 closure) → <new date> (K8.1 closure)

**Atomic commit**:
```
docs(migration): K8.1 closure recorded — native-side reference primitives ready
```

### 9.2 — Mark brief EXECUTED

**File**: `tools/briefs/K8_1_NATIVE_REFERENCE_PRIMITIVES_BRIEF.md`

Status line: `AUTHORED` → `EXECUTED (<date>, branch <branch>, closure <commit SHA range>)`. Add link to MIGRATION_PROGRESS.md K8.1 closure section.

**Atomic commit**:
```
docs(briefs): mark K8.1 brief as EXECUTED with closure refs
```

### 9.3 — Final verification

```
cd native/DualFrontier.Core.Native
cmake --build build --config Release
build/Release/df_native_selftest.exe
cd ../..
dotnet build
dotnet test
```

**Expected**: native build clean, selftest passes, managed build clean, all tests passing (553 + ~30 = ~583).

**Final pre-commit grep** (AD #4 discipline):

```
grep -rn "TODO\|FIXME\|XXX" native/DualFrontier.Core.Native/include/string_pool.h native/DualFrontier.Core.Native/include/keyed_map.h native/DualFrontier.Core.Native/include/composite.h native/DualFrontier.Core.Native/include/set_primitive.h native/DualFrontier.Core.Native/src/string_pool.cpp native/DualFrontier.Core.Native/src/keyed_map.cpp native/DualFrontier.Core.Native/src/composite.cpp native/DualFrontier.Core.Native/src/set_primitive.cpp src/DualFrontier.Core.Interop/Marshalling/InternedString.cs src/DualFrontier.Core.Interop/Marshalling/NativeMap.cs src/DualFrontier.Core.Interop/Marshalling/NativeComposite.cs src/DualFrontier.Core.Interop/Marshalling/NativeSet.cs
```

**Expected**: 0 matches (no leftover debt markers in K8.1-introduced files).

---

## Atomic commit log expected

Approximate commit count: **18-22**:

**Phase 2 (4 commits)**:
1. `feat(native): add StringPool with generational mod-scoped interning`
2. `feat(native): add KeyedMap with sorted-by-key iteration`
3. `feat(native): add Composite for per-entity variable-length data`
4. `feat(native): add SetPrimitive with sorted-by-element iteration`

**Phase 3 (1 commit)**:
5. `feat(native): integrate K8.1 primitives into World with mod scope orchestration`

**Phase 4 (1 commit)**:
6. `feat(native): add K8.1 C ABI surface (28 functions for 4 primitives)`

**Phase 5 (5 commits)**:
7. `feat(interop): add InternedString managed bridge type`
8. `feat(interop): add NativeMap<TKey, TValue> wrapper for KeyedMap`
9. `feat(interop): add NativeComposite<T> wrapper for Composite`
10. `feat(interop): add NativeSet<T> wrapper for SetPrimitive`
11. `feat(interop): NativeWorld exposes K8.1 primitive accessors`

**Phase 6 (1 commit)**:
12. `test(native): add K8.1 primitive selftest scenarios (17 total)`

**Phase 7 (4 commits)**:
13. `test(interop): add K8.1 bridge equivalence tests for InternedString`
14. `test(interop): add K8.1 bridge equivalence tests for NativeMap`
15. `test(interop): add K8.1 bridge equivalence tests for NativeComposite`
16. `test(interop): add K8.1 bridge equivalence tests for NativeSet`

**Phase 8 (0-1 commit)**:
17. `build(native): add K8.1 source files to CMakeLists.txt` (only if explicit-listing CMake)

**Phase 9 (2 commits)**:
18. `docs(migration): K8.1 closure recorded — native-side reference primitives ready`
19. `docs(briefs): mark K8.1 brief as EXECUTED with closure refs`

A merge commit on `main` is **not** в this list — fast-forward merge.

---

## Cross-cutting design constraints

1. **No production system changes**. К8.1 is foundation work. The 7 class components are NOT redesigned in K8.1 (that's K8.2). The 12 vanilla systems are NOT migrated в K8.1 (that's K8.3). К8.1 produces primitives only.

2. **K-L11 holds throughout**. NativeWorld is the production storage backbone. К8.1 primitives live в native only; managed bridge wrappers are thin facades, не parallel implementations.

3. **K-L7 span protocol unchanged**. К8.1 introduces new operations (intern, set, get, etc.) but не new span-style protocols. Future К8.x milestones may extend span semantics к primitives if needed; K8.1 does not.

4. **K-L9 mod parity preserved**. Vanilla mods и third-party mods both use the same K8.1 primitives через the same C ABI / managed bridge. No vanilla privilege.

5. **Atomic commits per logical change**. Each primitive: one feat commit для native impl. Bridge: one commit per wrapper. Tests: one commit per test file. Closure: separate commits per logical change.

6. **No regex metacharacters в `Edit` tool boundaries** (per `MOD_OS_V16_AMENDMENT_CLOSURE.md`).

7. **Pre-flight grep discipline (AD #4)**. Phase 0.4 inventory is structured grep verification. Phase 9.3 final pre-commit grep verifies no debt leaks.

8. **«Data exists or it doesn't»**. Each primitive's «not found» path returns 0 / nullptr deterministically; never returns stale данных or undefined behavior. This is enforced by the C ABI return-code conventions (0 = failure / not found).

9. **Single ownership boundary**. `World` owns all primitives. Primitives don't own each other. Mod scope state owned by `StringPool` (one data path); other primitives may extend mod-scope в future K8.x milestones if needed.

10. **Non-invasive integration with existing native infrastructure**. К8.1 doesn't modify `RawComponentStore`, `WriteBatch`, `SpanLease`, or other K0-K7 deliverables. Existing tests preserve their behavior; new tests cover К8.1 only.

---

## Stop conditions

The executor halts and escalates the brief authoring session if any of the following:

1. Phase 0 pre-flight check fails — working tree dirty, K8.0 not closed, specs at unexpected version, baseline build/test/selftest fails.

2. Phase 0.4 inventory diverges — any expected file missing, OR К8.1-target file already exists (indicates intervening work).

3. Phase 2 native compilation fails for any primitive. The patterns are well-trodden; failure indicates a typo or include issue. Diagnose locally; do not weaken design.

4. Phase 3 World integration fails — most likely cause is missing include or a mismatched method signature between header and source.

5. Phase 4 C ABI compilation fails — most likely cause is a try-catch boundary issue (must catch all exceptions, never let them propagate through C boundary) or signature mismatch between `df_capi.h` declaration and `capi.cpp` definition.

6. Phase 5 managed bridge compilation fails — most likely P/Invoke signature mismatch с C ABI. Cross-check `df_capi.h` declaration vs `[DllImport]` attribute char-by-char.

7. Phase 6 selftest scenario fails — diagnose against primitive impl; do not weaken test.

8. Phase 7 bridge equivalence test fails for behavioral divergence (e.g., `NativeMap.TryIterate` returns out-of-order). Halt; the divergence indicates either а P/Invoke marshalling bug or a primitive impl bug.

9. Phase 9.3 final grep finds TODO markers in K8.1-introduced files. Halt; clean up before closure.

10. Test count after Phase 7 less than 580 OR more than 590 (583 ± organic variance). Out-of-range indicates either missing tests or unexpected test additions; investigate.

The fallback in every halt case is `git stash push -m "k8-1-WIP-halt-$(date +%s)"` and report to the brief author.

---

## Brief authoring lineage

- **2026-05-09** — K8.0 closed (`9f9dc05..28498f9`). К8.1 brief authored same day per skeleton's «Brief authoring trigger: after K8.0 closure». Author: Opus architect session per «доки сначала, миграция потом» pivot continuation. К8.1 designed as foundation milestone — primitives only, не component redesigns, не system migrations.
- **(date TBD)** — Executed and closed at К8.1 milestone closure.

The brief was authored read-first / brief-second per the methodology pivot recorded in `MOD_OS_V16_AMENDMENT_CLOSURE.md`. Source documents read during authoring: `KERNEL_ARCHITECTURE.md` v1.2 LOCKED (К-L11, К-L3 implication, К-L8 implication), `MIGRATION_PROGRESS.md` (K8.0 closure section), existing native code (`world.h`, `df_capi.h`, native module structure), К8.1 skeleton recorded in K8.0 Phase 3.1.

Three architectural decisions LOCKED via Phase 1 (Crystalka chose «без костылей и упрощений» for all three):
- Per-primitive file organization (Option 2 in K8.1 design discussion)
- Generational mod-scope string interning (Option 3)
- Sorted-by-key iteration в KeyedMap and SetPrimitive (Option 3)

Three derived architectural decisions LOCKED by brief author («без компромиссов»):
- Save/load semantics: serialize content, not IDs; re-intern on load
- InternedString equality: ID-equality fast path, content-equality cross-pool
- Composite reallocation: detach-and-replace via std::vector::push_back; swap-with-last on remove

---

## Methodology note

К8.1 is the first **«foundation» implementation brief** in K8 series. Predecessor К8.0 was an architectural decision brief (fourth brief type); K8.1 returns к standard implementation brief format with extensive Phase 1 design contract.

The Phase 1 design contract is unusually detailed because К8.1's deliverables underpin К8.2-К8.5. Subsequent milestones consume К8.1 primitives without re-evaluating their design; getting К8.1's primitives right (type-erasure pattern, sorted iteration, generational pool, swap-with-last semantics) avoids retrofit work in К8.2-К8.5.

Future К8.x milestones may take similar shape if they introduce new primitives. Brief author should expect K8.2 to be a smaller brief (it consumes K8.1 primitives, applies them per-component) и K8.3 even smaller (it consumes both, applies to system code).

---

**Brief end.** Awaits Crystalka's review and feed to Claude Code session for execution.
