# Field Storage

Dual Frontier carries two orthogonal data systems under the same managed Application layer. The Entity Component System ([ECS](./ECS.md)) stores per-entity state as sparse component arrays; the Field Storage system stores spatial scalar/vector state as dense 2D grids. Neither subsumes the other. A pawn is an entity with components; a mana density is a field with cells. Code that reads pawn health does not touch fields; code that reads local mana does not touch components. The two systems share the native kernel ([KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md)) as the storage owner and `IModApi` ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §4.6) as the registration surface, but their access patterns, lifecycle rules, and capability verbs are distinct.

## Status

**Draft.** Populated incrementally during K9 implementation. Sections marked **TBD** are placeholders awaiting concrete native code; their architectural intent is fixed (per [GPU_COMPUTE](./GPU_COMPUTE.md) v2.0 LOCKED), but the matching managed-side surface only exists once the K9 milestone closes. Sections without **TBD** are contract-grounded and stable.

## Why two systems, not one

A single «World» entity model that absorbed both per-entity components and per-cell field values would force a representation choice that loses to both:

- A field encoded as «one entity per cell» bloats the entity table by `width × height` per field (200×200 = 40 000 entities per field, ten fields = 400 000 entities — orders of magnitude over typical pawn count) and forces sparse-set machinery to walk a dense 2D grid.
- An entity encoded as «a row in some grid» loses the per-entity state model where a pawn carries unrelated components (Health, Position, Job, Skills) without grid alignment.

The two systems target different access shapes:

| Aspect           | ECS                                     | Field Storage                                  |
|------------------|-----------------------------------------|------------------------------------------------|
| Identity         | `EntityId` (index + version)            | `(field_id, x, y)` cell coordinate             |
| Storage layout   | SparseSet — sparse indices, dense data  | Dense 2D array — every cell present            |
| Allocation       | Per-component-type store, grows on Add  | Per-field, fixed at registration               |
| Per-element work | Iterate dense entities matching a query | Iterate cells, optionally with stencil         |
| Mutation         | `WriteCommandBuffer` records intent     | Direct point write or compute dispatch         |
| Lifetime         | Entity destroyed → version bumped       | Field destroyed → buffer released              |
| GPU mapping      | Span exposed as SSBO (Domain B)         | Buffer exposed as SSBO or storage image (Domain A) |
| Capability verbs | `read`, `write`                         | `field.read`, `field.write`, `field.acquire`, `field.conductivity`, `field.storage`, `field.dispatch` |

The two systems remain decoupled at every layer above storage: managed bridges live in different files, capability verbs are disjoint, registration paths are disjoint, lifetimes are managed by separate kernel structures.

## Scope

This document specifies the **storage contract** for fields. It is the substrate that the GPU compute roadmap ([GPU_COMPUTE](./GPU_COMPUTE.md) G0–G9) sits on top of. The storage path is required to function on CPU alone — every shader has a CPU reference implementation that operates on the same `RawTileField<T>` instances, and the `IModApi.Fields` surface is identical regardless of GPU availability.

Three scopes are out of band for this document:

- **Mathematical models** (diffusion, anisotropy, capacitance, cliff thresholds, eikonal). These live in [GPU_COMPUTE](./GPU_COMPUTE.md) as the spec layer; the storage layer carries the data the math operates on, not the math itself.
- **Compute pipeline registration** (`IModApi.ComputePipelines`). Pipelines are registered alongside fields but are a separate API; only field-side verbs (`field.dispatch`) reach pipelines transitively.
- **Mod-specific gameplay decisions** (which fields ship in which vanilla mod, which decay rates, which colour ramps). These live in mod assemblies, not architectural docs.

## Field — the unit of storage

A field is a typed dense 2D array bound to a registered string id. Every field has the same structural shape regardless of element type:

- A primary buffer (`width × height × sizeof(T)` bytes) holding the current cell values.
- A back buffer of identical layout used for ping-pong updates by compute kernels (CPU or GPU) that read from one buffer and write to the other.
- A conductivity map (`width × height × sizeof(float)` bytes) carrying per-cell diffusion coefficients. Wires, pipes, walls, insulators all express themselves through this map. Default value is 1.0 (uniform isotropic diffusion); mods that don't need anisotropy never touch it.
- A storage flag bitmap (`width × height` bits, byte-aligned) marking cells that retain capacitance state across ticks. Batteries, tanks, thermal mass cells set their flag; ordinary cells leave it clear.

Element type `T` is constrained to `unmanaged` (matches K-L3 from the kernel — blittable layout, no GC reachability, direct memory copy). Common types are `float` (most scalar fields), `Vector2` (flow field directions), `int32` (discrete labels). Modder-defined types are accepted as long as they are blittable.

### Why ping-pong from day one, not «add later»

A diffusion update reads four neighbours and writes the current cell. If the read and write target the same buffer, the update sees partially-applied results from earlier cells in the same pass — Gauss-Seidel rather than Jacobi semantics. The two converge to the same equilibrium but with different transient behaviour, and the choice is not free: GPU compute pipelines parallelise across cells, which forces Jacobi (no order guarantee), and the CPU reference implementation that serves as the shader equivalence oracle ([GPU_COMPUTE](./GPU_COMPUTE.md) «Failure modes → CPU fallback») must produce bit-equivalent output.

Ping-pong is therefore not an optimisation; it's the only update model that survives the CPU↔GPU equivalence requirement. The back buffer is allocated at field registration time, not lazily on first dispatch — there is no «field without back buffer» state.

### Why conductivity map and storage flags from day one

K9 ships isotropic mana diffusion (`Vanilla.Magic` per [GPU_COMPUTE](./GPU_COMPUTE.md) G1) where a uniform conductivity = 1.0 suffices. Adding anisotropy retroactively (G2 — `Vanilla.Electricity` wires) would force a storage layout migration: the existing buffer must grow by `width × height × 4` bytes, the C ABI surface gains `set_conductivity`, the managed bridge gains `SetConductivity(x, y, value)`, and every field-aware test must be updated to expect the new layout.

Same logic for storage flags (G3 — batteries). The cost of including both from K9 is fixed memory (`width × height × 5` bytes per field, ~200 KB for a 200×200 field) and a few extra C ABI functions; the cost of adding them later is a flag day across native, bridge, tests, and mod code. The «no compromises» rule resolves this trivially — pay the cost once at K9.

### Identity and namespacing

Field id is a string of the form `<mod-namespace>.<field-name>`, e.g. `vanilla.magic.mana`, `vanilla.electricity.power`, `vanilla.water.pressure`. The `<mod-namespace>` segment must match the registering mod's manifest id (per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.3 — reserved namespace rule). Two mods cannot register the same id; the second registration fails with `FieldRegistrationConflict` ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §11.2).

Other mods reach a field by id through `IModApi.Fields.GetField<T>(id)`. The accessing mod must declare the appropriate `field.*` capability against the foreign id in its manifest, and must list the owning mod in `dependencies` — the same chain that governs cross-mod contract access ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.4).

## Native layer — `RawTileField<T>`

**TBD** — concrete C++ class layout lands at K9 implementation. The contract:

```cpp
// native/DualFrontier.Core.Native/include/tile_field.h (K9 deliverable)

class RawTileField {
public:
    int32_t width() const noexcept;
    int32_t height() const noexcept;
    int32_t cell_size() const noexcept;

    // Point access (validation: bounds check; out-of-bounds returns 0).
    int32_t read_cell(int32_t x, int32_t y, void* out, int32_t size) const;
    int32_t write_cell(int32_t x, int32_t y, const void* in, int32_t size);

    // Span access (atomic counter; mutations rejected while spans active).
    int32_t acquire_span(const void** out_data, int32_t* out_width, int32_t* out_height);
    void    release_span();

    // Conductivity map (per-cell float coefficient).
    int32_t set_conductivity(int32_t x, int32_t y, float value);
    float   get_conductivity(int32_t x, int32_t y) const;

    // Storage flag (per-cell bit).
    int32_t set_storage_flag(int32_t x, int32_t y, int32_t enabled);
    int32_t get_storage_flag(int32_t x, int32_t y) const;

    // Ping-pong buffer swap (called by update kernels).
    void    swap_buffers() noexcept;

private:
    int32_t width_;
    int32_t height_;
    int32_t cell_size_;
    std::vector<uint8_t>  data_;            // primary buffer
    std::vector<uint8_t>  back_buffer_;     // ping-pong target
    std::vector<float>    conductivity_;    // per-cell D
    std::vector<uint8_t>  storage_flags_;   // per-cell bit (byte-packed)
    std::atomic<int32_t>  active_spans_{0};

    // GPU resources lazily allocated by G-series; null on K9.
    void* gpu_buffer_   = nullptr;
    void* gpu_descriptor_ = nullptr;
};
```

The `World` class (already extant per K0–K5) gains a parallel registry alongside `stores_`:

```cpp
class World {
    std::unordered_map<uint32_t, std::unique_ptr<RawComponentStore>> stores_;
    std::unordered_map<std::string, std::unique_ptr<RawTileField>>   fields_;  // K9 NEW
    // ...
};
```

Field registration is keyed by string id (not numeric type id) because field identity is namespaced per-mod and must remain stable across mod reload cycles, while component type ids are kernel-local and assigned sequentially. Numeric field ids are not stable across mod reload (mod A reloaded twice could see its field assigned id 7 then id 9); string ids are.

### Mutation rejection contract

A field follows the same atomic-counter pattern as `RawComponentStore`: while any span is acquired (`active_spans_ > 0`), point writes (`write_cell`), conductivity updates (`set_conductivity`), storage flag updates (`set_storage_flag`), and ping-pong swap (`swap_buffers`) all throw `std::logic_error` caught at the C ABI boundary and reported as a 0 return code. The contract aligns with K-L7 (read-only spans + write batching) — a span is a read view, mutations are explicit, races are structurally impossible.

Compute dispatches are not gated by `active_spans_` but by their own dispatch synchronisation (GPU fence-based; CPU reference implementation is single-threaded). A dispatch in flight does not increment `active_spans_` because the dispatch has its own buffer ownership model — fence wait before subsequent reads guarantees consistency.

### Span lifetime

`acquire_span` returns a pointer to the primary buffer's contiguous bytes, valid until `release_span` is called or any mutating operation throws (which fails before any state change). The returned pointer is `const` — spans are read-only. Bulk writes go through compute dispatch, not span mutation. This matches the «mutations are explicit, batched, validated» discipline established by K5 (`WriteCommandBuffer` for entity component writes).

## C ABI extension — `df_world_field_*`

**TBD** — exact signatures land at K9 implementation. The contract per [GPU_COMPUTE](./GPU_COMPUTE.md) «Architectural integration → Native compute dispatch (G-series)»:

```c
// native/DualFrontier.Core.Native/include/df_capi.h (K9 extension)

DF_API int32_t df_world_register_field(
    df_world_handle world,
    const char* field_id,
    int32_t width,
    int32_t height,
    int32_t cell_size);

DF_API int32_t df_world_field_read_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    void* out_value,
    int32_t size);

DF_API int32_t df_world_field_write_cell(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    const void* value,
    int32_t size);

DF_API int32_t df_world_field_acquire_span(
    df_world_handle world,
    const char* field_id,
    const void** out_data,
    int32_t* out_width,
    int32_t* out_height);

DF_API int32_t df_world_field_release_span(
    df_world_handle world,
    const char* field_id);

DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    float value);

DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world,
    const char* field_id,
    int32_t x,
    int32_t y,
    int32_t enabled);
```

Compute dispatch (`df_world_field_dispatch_compute`) and pipeline registration (`df_world_register_compute_pipeline`) are **not** part of K9 — they land in G0 with the Vulkan compute plumbing. K9 ships a CPU-only path; mods that want field updates run a CPU reference kernel registered as an ordinary system (no compute API needed).

Naming convention (`df_world_field_*` — three-token prefix) follows the existing kernel pattern (`df_world_*` for world-wide, `df_batch_*` for batch-handle-bound). It distinguishes field-bound calls from component-bound calls (`df_world_*_component`) at a glance.

## Managed bridge — `FieldRegistry` and `FieldHandle<T>`

**TBD** — concrete classes land at K9 implementation. The contract per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §4.6:

```csharp
// src/DualFrontier.Core.Interop/FieldRegistry.cs (K9 deliverable)

public sealed class FieldRegistry
{
    private readonly NativeWorld _world;
    private readonly Dictionary<string, IFieldHandle> _handles = new();

    public FieldHandle<T> Register<T>(string id, int width, int height) where T : unmanaged;
    public FieldHandle<T> Get<T>(string id) where T : unmanaged;
    public bool IsRegistered(string id);
}

public sealed class FieldHandle<T> where T : unmanaged
{
    public string Id { get; }
    public int Width { get; }
    public int Height { get; }

    public T ReadCell(int x, int y);
    public void WriteCell(int x, int y, T value);

    public FieldSpanLease<T> AcquireSpan();   // IDisposable, calls release_span on Dispose

    public void SetConductivity(int x, int y, float value);
    public float GetConductivity(int x, int y);

    public void SetStorageFlag(int x, int y, bool enabled);
    public bool GetStorageFlag(int x, int y);
}

public readonly ref struct FieldSpanLease<T> where T : unmanaged
{
    public ReadOnlySpan<T> Span { get; }
    public int Width { get; }
    public int Height { get; }
    public void Dispose();
}
```

`FieldHandle<T>` carries the bound type at compile time; element-typed `ReadCell` / `WriteCell` use `Unsafe.As` for the unmanaged conversion. `FieldSpanLease<T>` mirrors the existing `SpanLease<T>` from K5 — `ref struct` to keep it stack-bound, `IDisposable` semantics through `Dispose` (manual, since `ref struct` cannot implement `IDisposable`).

The registry is per-`NativeWorld`. Mod loader supplies the registry to `RestrictedModApi`'s `IModApi.Fields` accessor; the registry routes registrations to native and tracks managed-side handles for capability cross-check.

## IModApi v3 wiring

The capability cross-check happens at three points:

1. **Manifest parse** — every `field.*:<id>` capability string in `capabilities.required` is validated against the regex (per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §2.3 step 6).
2. **Load-time** — if the mod registers a system carrying `[FieldAccess(...)]` attributes, the loader verifies each declared field id appears in the manifest's `capabilities.required` (per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.7 cross-check).
3. **Runtime** — every call to `FieldHandle<T>.ReadCell` / `WriteCell` / `AcquireSpan` / `SetConductivity` / `SetStorageFlag` consults the calling mod's per-mod capability set (`HashSet<string>`); a missing entry raises `CapabilityViolationException` (per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.6 hybrid enforcement).

Layer 2 is conceptually new for K9 — the `[FieldAccess]` attribute on systems is added alongside `[SystemAccess]` to carry field id declarations. Layer 3 reuses the existing `RestrictedModApi.EnforceCapability` machinery (added in M3); the per-call cost is a hash-set lookup, measured negligible.

The full attribute scheme:

```csharp
[FieldAccess(
    reads: new[] { "vanilla.magic.mana" },
    writes: new[] { "vanilla.magic.mana" },
    conductivity: new[] { },
    storage: new[] { },
    dispatch: new[] { "vanilla.magic.mana" })]
public class ManaFieldUpdateSystem : SystemBase { /* ... */ }
```

The attribute is enforced by `SystemExecutionContext` in the same way it enforces `[SystemAccess]` for components (per [ECS](./ECS.md) and [ISOLATION](./ISOLATION.md)). A system that calls a field operation without declaring the verb-id pair throws `IsolationViolationException` even in Release builds.

## Lifecycle

Field lifecycle parallels mod lifecycle. A mod registers fields during `IMod.Initialize(api)`; the fields persist as long as the mod is loaded. On mod unload, the registry deregisters every field owned by the unloading mod and the native side releases the buffers. Mod reload (per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §9.2) re-registers the fields from scratch — content does not survive across reload, since the data is mod-managed and the new instance might disagree about cell semantics.

The kernel does not own any field. Vanilla = mods means there is no engine-special field; even the canonical mana, electricity, water fields belong to vanilla mods (`Vanilla.Magic`, `Vanilla.Electricity`, `Vanilla.Water`).

A field is destroyed by:

- The owning mod calling `FieldRegistry.Unregister(id)` explicitly (e.g. dynamic field lifetime — flow field eviction per [GPU_COMPUTE](./GPU_COMPUTE.md) G7).
- The owning mod being unloaded (loader sweeps mod-namespaced ids).
- World destruction (kernel shutdown sweeps all fields).

## Save / load

**TBD** — persistence integration lands as a separate milestone (currently «out of M0–M10 scope» per [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §12 D-6). The intended contract:

- Field data serialises as a single blob per field (raw bytes from primary buffer + conductivity map + storage flags). Back buffer is not serialised — restored to zero on load (any in-flight ping-pong state is lost).
- Save records `(field_id, owning_mod_id, width, height, cell_size, blob)`.
- On load, fields are re-registered by the owning mod's startup, then the loader streams the blob into the registered field's buffers.
- Width / height mismatch between save and current registration is a load error — fields cannot resize across sessions.

Save format and per-field opt-out (some fields might be ephemeral, e.g. flow fields per [GPU_COMPUTE](./GPU_COMPUTE.md) G7) are deferred к persistence-integration milestone.

## CPU and GPU paths share the same field

A field is a single allocation in native memory. The GPU path (G0+) lazily allocates a `VkBuffer` mirror of the primary buffer when the field is first dispatched; subsequent dispatches reuse the buffer. Read-back to CPU happens through `acquire_span` after a fence wait — the managed code never sees a partially-applied dispatch.

The CPU reference path operates directly on the native buffers. A CPU-only build (Vulkan unavailable, per [GPU_COMPUTE](./GPU_COMPUTE.md) «Failure modes → CPU fallback») runs the same shaders translated to managed C# — diffusion math is straightforward, the equivalence is testable cell-by-cell.

The mod author writes the same code regardless of backend:

```csharp
public class ManaFieldUpdateSystem : SystemBase
{
    [FieldAccess(reads: new[] { "vanilla.magic.mana" },
                 dispatch: new[] { "vanilla.magic.mana" })]
    public override void Update(float delta)
    {
        var mana = Fields.Get<float>("vanilla.magic.mana");
        var pipeline = Pipelines.Get("vanilla.magic.mana_diffusion");
        mana.DispatchCompute(pipeline, pushConstants, iterations: 5);
    }
}
```

The runtime decides whether `DispatchCompute` issues a Vulkan compute dispatch or runs the CPU reference kernel. The mod doesn't care.

## Anti-patterns

### A field per entity

A naive «one mana value per pawn» encoding stores `ManaComponent` on every entity that has mana, plus a separate field for ambient mana, plus arithmetic linking them. The field model dissolves this: there is exactly one mana field; a pawn casting a spell reads `mana.ReadCell(pawn.x, pawn.y)` to determine local availability and writes back the deficit through a write op or contributes to a sink map. No per-pawn component.

The same logic applies to electricity, water, heat, sound, scent. Per-entity components capture entity-specific state (a battery's stored charge, a generator's output level); the field captures the spatial distribution.

### A component holding a `FieldHandle<T>`

```csharp
// BAD — the handle outlives the component if the mod unregisters the field.
public struct ManaCasterComponent : IComponent
{
    public FieldHandle<float> ManaField;
}
```

A `FieldHandle<T>` is registry-bound; storing it in component state ties component memory to mod-managed lifetime. On mod reload, the handle becomes invalid but the component still holds it. Systems should fetch the handle through `IModApi.Fields.Get` per-tick (cheap — registry lookup is `O(1)`), not cache it in components.

### Direct point-write loop instead of compute dispatch

```csharp
// BAD — 40 000 P/Invoke crossings per tick on a 200×200 grid.
for (int x = 0; x < width; x++)
    for (int y = 0; y < height; y++)
        mana.WriteCell(x, y, decay(mana.ReadCell(x, y)));
```

Bulk updates go through compute dispatch (CPU reference or GPU). Point writes are for sparse mutations (a generator placed at one cell sets its source value; a battery stores per-tick excess at its location). The cost-difference is structural: a dispatch is one P/Invoke + one shader execution; a per-cell loop is `width × height` P/Invokes.

### A modding mod that doesn't list its fields

A mod registering a field without declaring `mod.<id>.field.read:<id>` (or equivalent) in `capabilities.provided` is invisible to other mods. The capability declaration is the only way another mod can resolve the field through the dependency chain. This is enforced by the loader's cross-check ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3.4).

## See also

- [ECS](./ECS.md) — the orthogonal system; entities, components, sparse-set storage. Same kernel, separate access model.
- [GPU_COMPUTE](./GPU_COMPUTE.md) — v2.0 LOCKED. Field math models (diffusion, anisotropy, capacitance, eikonal). The mathematics fields exist to carry.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — Part 2 §K9 (field storage abstraction milestone). The native-side spec.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — §3.2 (capability syntax for `field.*` verbs), §4.6 (`IModApi` v3 — `Fields` and `ComputePipelines` sub-APIs), §11.2 (validation error kinds for field operations).
- [THREADING](./THREADING.md) — fence-based GPU sync; field dispatches are non-blocking.
- [PERFORMANCE](./PERFORMANCE.md) — field memory budget, dispatch timing.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — pipeline patterns; «data exists or it doesn't» applies to field cells (out-of-bounds reads return zero, not undefined).
- `tools/briefs/K9_FIELD_STORAGE_BRIEF.md` — the executable contract for the K9 implementation milestone.

**Document end.** Updated incrementally as K9 lands. Sections marked **TBD** become concrete once their implementation commits exist.
