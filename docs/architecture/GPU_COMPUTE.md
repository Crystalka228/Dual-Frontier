# GPU Compute

Authoritative reference for GPU compute strategy in Dual Frontier. Supersedes the Phase 3 `ProjectileSystem`-only formulation. Reflects the architectural pivot toward native ECS kernel + Vulkan rendering layer (see [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) and [RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md)), and the field-based simulation model that elevates GPU compute from a deferred per-system optimization to a foundational architectural capability.

## Status

**LOCKED v2.0.** Departures require explicit re-architecture milestone.

This document supersedes the previous Phase 3 `GPU_COMPUTE.md`, which scoped GPU compute as a deferred backend swap for `ProjectileSystem` at the 500-projectile threshold. That formulation remains valid as a special case (entity-keyed bulk computation crossing CPU saturation), but the broader architectural commitment is now field-based GPU compute as a foundational layer integrated into the K9 + G-series roadmap.

## Context shift from v1.0

Phase 3 framed GPU compute as a single-system optimization concern: CPU handles `ProjectileSystem` at typical loads, GPU pays off at ~500 projectiles, dispatch/readback overhead determines the threshold. That framing assumed:

- Managed C# runtime with marshalling overhead between CPU and GPU
- GPU compute as a foreign capability invoked through DI-injected backend
- Single-system scope (`ProjectileSystem` only)
- Entity-keyed data layout requiring conversion to GPU-friendly format

The pivots committed in [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) v1.0 and [RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md) v1.0 invalidate every assumption above:

- Native C++ kernel owns component memory in SoA layout already SSBO-friendly
- Vulkan 1.3 already loaded for rendering — compute API is the same surface
- Native kernel can issue compute dispatch directly via `vulkan-1.dll` linkage, no managed marshalling per-dispatch
- Field abstraction (introduced in K9) provides dense 2D grid storage that is the natural GPU compute substrate

The combined effect: GPU compute is no longer a deferred optimization for one system. It is a structural capability of the architecture, with concrete grounding in field-based gameplay mechanics (mana, electricity, water, heat, sound, scent, future fields).

## Two compute domains

GPU compute serves two architecturally distinct workload categories. Both are LOCKED as supported domains.

### Domain A — Field updates (primary, foundational)

Spatial scalar/vector fields stored as dense 2D grids. Each field cell updates per tick (or sub-tick) via diffusion, propagation, decay, or similar local-stencil operations. Embarrassingly parallel; one compute shader invocation per cell.

Examples: mana density, electricity field, water pressure, temperature distribution, sound pressure, scent concentration, pollution, radiation, future modder-defined fields.

Storage: `RawTileField<T>` in native kernel (introduced K9). Per-field conductivity map enables anisotropic diffusion (wires, pipes). Per-field storage flags enable temporal capacitance (batteries, tanks, thermal mass).

Compute pattern: 4-neighbor stencil, optional anisotropy weighting, optional storage cell handling, optional cliff-threshold consumer effectiveness. Ping-pong between two image/buffer resources, 5–10 iterations per dispatch to reach near-equilibrium.

This is the primary motivator for elevating GPU compute to foundational status. Field-based mechanics are not a single optimization target; they are a generative architectural pattern that absorbs new gameplay mechanics through additive registration.

### Domain B — Entity-keyed bulk computation (secondary, opportunistic)

Per-entity calculations that scale poorly on CPU at high entity counts. Original `ProjectileSystem` case falls here. Workloads suitable when:

- Per-entity work is uniform (no branchy decision logic per entity)
- Entity count is high (typically 500+ for projectiles, similar magnitude for other domains)
- Output can tolerate one-tick lag (asynchronous readback)

Examples: projectile position/collision, parallel pathfinding flow fields, large-scale particle effects, bulk physics simulation, AI cohort updates.

Storage: existing `RawComponentStore` (sparse-set, dense byte buffers). Native kernel exposes component spans directly as SSBO content; no marshalling.

Compute pattern: one work group per entity batch, read component spans, write results to output buffer, fence-based sync to managed read.

Domain B retains the Phase 3 architectural pattern (interface-injected backend, threshold-driven swap) but no longer requires the architectural compromises of the original v1.0 formulation. Native kernel makes the integration cost negligible.

## Why now (vs. Phase 3 deferral rationale)

### v1.0 deferral rationale — superseded

Phase 3 reasoning: CPU→GPU→CPU roundtrip costs 0.5–2 ms per frame in a managed runtime, so dispatch overhead exceeds saved CPU work below ~500 projectiles. Threshold pinned experimentally to "Battle of the Gods" stress test.

This reasoning was correct for the v1.0 architecture (managed runtime, no native kernel, no Vulkan rendering layer). It does not apply to the post-pivot architecture:

- Dispatch path is native → `vulkan-1.dll`, no managed crossings
- Component data already in SoA layout, no marshalling
- Vulkan instance/device already live for rendering, no setup cost amortization concern
- Compute and graphics share the same `VkDevice` and `VkQueue` family (or async compute queue)

The 0.5–2 ms estimate from v1.0 reflected managed runtime overhead, not the actual dispatch cost on bare Vulkan. Native dispatch overhead is microseconds, not milliseconds.

### v2.0 rationale — field mechanics force the issue

Field-based gameplay mechanics (mana, electricity, water, etc.) are not optional optimizations for some future scale. They are first-class gameplay systems in Dual Frontier's design. Their natural implementation is dense 2D grid storage with cellular-automaton-style updates, which fits GPU compute architecturally.

The choice is not "CPU now, GPU later when scale demands." The choice is "field math on CPU forever (with associated scaling cap) vs. field math on GPU (free at any scale)." Given hardware baseline (Steam Hardware Survey median GPU class is RTX 3060/4060 territory; even budget GPUs handle our workload trivially) and architectural fit (Vulkan compute already in scope), the GPU path is structurally preferable.

Domain B (entity-keyed bulk compute) retains a deferral story: still threshold-driven, still benchmarked against CPU baseline, still optional. Domain A (fields) does not.

## Architectural integration

### Native kernel ([KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9)

K9 introduces field storage as a parallel abstraction alongside `RawComponentStore`:

```cpp
class RawTileField {
    int32_t width_;
    int32_t height_;
    int32_t cell_size_;
    std::vector<uint8_t> data_;
    std::vector<uint8_t> back_buffer_;       // ping-pong
    std::vector<float> conductivity_map_;    // per-cell D values (anisotropy)
    std::vector<uint8_t> storage_flags_;     // per-cell capacitance markers

    // GPU resources (lazy — only allocated for compute-managed fields):
    VkBuffer gpu_buffer_;
    VkDeviceMemory gpu_memory_;
    VkDescriptorSet compute_descriptor_;
};

class World {
    std::unordered_map<uint32_t, std::unique_ptr<RawComponentStore>> stores_;
    std::unordered_map<uint32_t, std::unique_ptr<RawTileField>> fields_;
};
```

K9 ships CPU functional path first (no GPU). Field reads, writes, span access, and CPU-side update kernels work without Vulkan compute. This guarantees the abstraction is sound independent of the GPU layer.

### Native compute dispatch (G-series)

G-series introduces Vulkan compute integration. The native kernel gains a new linkage to `vulkan-1.dll` for compute pipeline operations. The existing rendering layer ([RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md)) shares the same `VkInstance`/`VkDevice`; compute integration reuses these handles.

C ABI extension on `df_capi.h`:

```c
DF_API void df_world_register_field(
    df_world_handle world, uint32_t field_id,
    int32_t width, int32_t height, int32_t cell_size);

DF_API int32_t df_world_field_read_cell(
    df_world_handle world, uint32_t field_id,
    int32_t x, int32_t y, void* out_value, int32_t size);

DF_API int32_t df_world_field_acquire_span(
    df_world_handle world, uint32_t field_id,
    void** out_data, int32_t* out_width, int32_t* out_height);

DF_API int32_t df_world_field_set_conductivity(
    df_world_handle world, uint32_t field_id,
    int32_t x, int32_t y, float value);

DF_API int32_t df_world_field_set_storage_flag(
    df_world_handle world, uint32_t field_id,
    int32_t x, int32_t y, int32_t enabled);

DF_API int32_t df_world_field_dispatch_compute(
    df_world_handle world, uint32_t field_id,
    uint32_t pipeline_id, void* push_constants, int32_t push_size,
    int32_t iterations);

DF_API int32_t df_world_register_compute_pipeline(
    df_world_handle world, uint32_t pipeline_id,
    const uint8_t* spirv_bytes, int32_t spirv_size);
```

Managed bridge (`DualFrontier.Core.Interop`) wraps these into typed APIs:

```csharp
public sealed class FieldRegistry
{
    public FieldHandle<T> RegisterField<T>(string name, int width, int height) where T : unmanaged;
    public ComputePipelineHandle RegisterComputePipeline(string name, byte[] spirvBytes);
}

public sealed class FieldHandle<T> where T : unmanaged
{
    public T ReadCell(int x, int y);
    public ReadOnlySpan<T> AcquireSpan(out int width, out int height);
    public void SetConductivity(int x, int y, float value);
    public void SetStorageFlag(int x, int y, bool enabled);
    public void DispatchCompute(ComputePipelineHandle pipeline, ReadOnlySpan<byte> pushConstants, int iterations);
}
```

### Mod-driven shader registration

Per K-L9 (vanilla = mods), compute shaders are owned by mods, not the engine. `Vanilla.Magic` ships `ManaField` + diffusion shader. `Vanilla.Electricity` ships `PowerField` + anisotropic diffusion shader + storage cell handling. Third-party mods extend through the same registration API.

Build pipeline ([RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md) §1.7) already compiles GLSL to SPIR-V via `glslangValidator.exe` for graphics shaders. Compute shaders use the same toolchain. Mod build process embeds compiled SPIR-V into mod assets.

Mod startup code:

```csharp
public class MagicMod : IMod
{
    public void Register(IModApi api)
    {
        var manaField = api.Fields.RegisterField<float>("vanilla.magic.mana", 200, 200);
        var diffusionPipeline = api.ComputePipelines.RegisterPipeline(
            "vanilla.magic.mana_diffusion",
            EmbeddedResource.Load("shaders/mana_diffusion.spv"));

        api.Systems.RegisterFieldUpdate(
            "ManaFieldUpdate",
            phase: SimulationPhase.PostPawn,
            interval: TickInterval.Every(30),  // mana diffuses slowly
            handler: (ctx) => manaField.DispatchCompute(diffusionPipeline, manaParams, iterations: 5));
    }
}
```

## Mathematical models

### Domain A primary kernels

**Isotropic diffusion** (mana, basic spread):

```
∂P/∂t = D · ∇²P + S(x,y) - K · P
```

Compute shader (~30 LOC GLSL): 4-neighbor stencil, single D coefficient, source map, decay coefficient.

**Anisotropic diffusion** (electricity, water — wires/pipes channel flow):

```
∂P/∂t = ∇·(D(x,y) · ∇P) + S(x,y) - C(x,y) · effectiveness(P)
```

Per-cell D varies. Wire/pipe tiles have D ≈ 10.0; off-path tiles have D ≈ 0.1; insulators have D = 0.0. The asymmetric flow `min(D_self, D_neighbor)` between each tile pair guarantees flow blocked when either tile is non-conductor. Wire path channels propagation automatically; "narrow wave" is emergent, not coded.

**Capacitance / storage cells** (batteries, tanks, thermal mass):

```
storage[t+1] = α · storage[t] + (1-α) · field_local[t]
field_emit_when_demanded = β · storage[t]
```

Storage tile retains state across ticks while neighbors evolve. RC time constant analogy. α near 0.95–0.99 for slow decay, β controls release rate during droop.

**Cliff threshold consumer effectiveness** (electricity 60% rule):

```
effectiveness(local_P, demand) =
    1.0,                          if local_P ≥ demand
    local_P / demand,             if 0.6·demand ≤ local_P < demand
    0.0,                          if local_P < 0.6·demand
```

Computed in managed code at consumer system after field update. Below-threshold consumers pull 0 from field, freeing capacity for others; system self-stabilizes.

### Unified across mechanics

| Field       | Sources              | Sinks                    | Conductivity                  | Storage           | Notes                       |
|-------------|----------------------|--------------------------|-------------------------------|-------------------|-----------------------------|
| Mana        | Springs, ley lines   | Spell casts              | Uniform (no conduits)         | Magic accumulators| Slow decay                  |
| Electricity | Generators, solar    | Consumers (pits)         | Wires (high D)                | Batteries         | Fast spread, cliff threshold|
| Water       | Pumps, wells         | Drains, irrigation       | Pipes (high D)                | Tanks             | Optional gravity bias       |
| Heat        | Furnaces, sun        | Cold tiles, refrigerators| Air (medium), insulation (low)| Thermal mass walls| Slow propagation            |
| Sound       | Combat, machinery    | Decay-dominated          | Air, walls                    | None typically    | Fast, decay over distance   |
| Scent       | Food, blood, entities| Time-dominated decay     | Air, terrain                  | None typically    | Trail formation             |

One compute shader pattern (~50–80 LOC GLSL) handles all six with parameter variation. Modder-defined fields extend the same template.

### Flow field pathfinding (Domain A extension)

Pathfinding is mathematically isomorphic to electricity propagation: a target spike, an anisotropic propagation respecting walkable terrain, an agent reading the gradient. Same shader template, same field infrastructure, different gameplay interpretation. Adds pathfinding capability to Dual Frontier without expanding the architectural surface — a structural pattern unification between pathfinding and supply networks.

**Per-agent A\* (current DF approach via `PathfindingService` / `AStarPathfinding`):**

```
For each pawn:
  Run A* from pawn position to target
  Cache path
  Follow path step-by-step
Cost: O(N × M log M) where N = pawns, M = grid size
```

**Global flow field:**

```
Per target (each unique destination):
  Compute distance field once (one compute shader dispatch)
  Compute direction field once (gradient of distance)

Per pawn (every tick):
  Read direction at pawn's position (point query)
  Move in that direction
Cost: O(K · M) + O(N) where K = unique targets, N = pawns
```

Scaling:

- 50 pawns going to 5 work zones → A\*: 50 searches per tick worst case. Flow field: 5 fields shared, 50 cheap reads.
- 200 pawns going to 5 work zones → A\*: 200 searches (linear pain). Flow field: still 5 fields, 200 cheap reads.
- Cost decouples from pawn count.

#### Mathematical isomorphism with electricity

Identical pattern, different interpretation:

| Aspect                | Electricity                        | Flow field                                              |
|-----------------------|------------------------------------|---------------------------------------------------------|
| Spike source          | Generator tile (+P)                | Target tile (max value)                                 |
| Field equation        | Anisotropic diffusion + decay      | Eikonal equation OR simple diffusion                    |
| Conductivity map      | Wires (high D), insulators (low D) | Walkable terrain (high D), obstacles (low D, walls = 0) |
| What field represents | Power available                    | Distance to target (after gradient: direction to target)|
| Agent behavior        | Read field, compute effectiveness  | Read gradient, move down                                |
| GPU compute pattern   | Same                               | Same                                                    |

Same shader template, different parameters. This is the architectural compound effect — new pathfinding capability through existing infrastructure.

#### Distance field math

Two main approaches to computing the distance field on GPU.

**Option A — Eikonal equation** (geodesic-accurate distances):

```
‖∇D(x,y)‖ = 1 / speed(x,y)
```

Where `D` is distance, `speed(x,y)` is local traversal speed (1.0 for open ground, 0.5 for difficult terrain, 0 for walls).

GPU implementation: Fast Sweeping Method (FSM) or Fast Marching Method (FMM) — established parallel algorithms. Multiple sweeps converge to correct geodesic distance respecting obstacles.

```glsl
// Simplified eikonal sweep (one direction):
void main() {
    ivec2 p = ivec2(gl_GlobalInvocationID.xy);
    float speed = imageLoad(speed_map, p).r;
    if (speed <= 0.0) return;  // wall

    float current = imageLoad(distance_in, p).r;
    float n = imageLoad(distance_in, p + ivec2(0, -1)).r;
    float s = imageLoad(distance_in, p + ivec2(0,  1)).r;
    float e = imageLoad(distance_in, p + ivec2( 1, 0)).r;
    float w = imageLoad(distance_in, p + ivec2(-1, 0)).r;

    float min_h = min(n, s);
    float min_v = min(e, w);

    // Eikonal solver (Godunov upwind):
    float h = 1.0 / speed;
    float new_d;
    if (abs(min_h - min_v) >= h) {
        new_d = min(min_h, min_v) + h;
    } else {
        // 2D update
        new_d = (min_h + min_v + sqrt(2.0*h*h - (min_h-min_v)*(min_h-min_v))) * 0.5;
    }

    imageStore(distance_out, p, vec4(min(current, new_d), 0, 0, 0));
}
```

5–10 sweep passes converge for 200×200 grid. Microseconds on mid-range GPU.

**Option B — Simple diffusion with decay** (gameplay-acceptable approximation):

```
∂D/∂t = ∇²D - K·D + spike_at_target
```

Does not give geodesic-accurate distances but correctly produces a gradient pointing toward the target. Cheaper to compute, less mathematical machinery, ~99% gameplay-equivalent for colony sim.

For Dual Frontier this quality is enough — pawns don't optimize paths, players don't notice 5% suboptimal routing. Eikonal can be added later if specific gameplay demands accuracy (e.g. precise speedrun-style mechanics).

Recommendation: Option B initially; upgrade to Option A only if measurement shows gameplay degradation (G9 milestone below).

#### Direction field (gradient extraction)

After distance field stable:

```glsl
void main() {
    ivec2 p = ivec2(gl_GlobalInvocationID.xy);
    float c = imageLoad(distance_in, p).r;
    float n = imageLoad(distance_in, p + ivec2(0, -1)).r;
    float s = imageLoad(distance_in, p + ivec2(0,  1)).r;
    float e = imageLoad(distance_in, p + ivec2( 1, 0)).r;
    float w = imageLoad(distance_in, p + ivec2(-1, 0)).r;

    // Negative gradient = direction to smaller distance = toward target
    vec2 dir = normalize(vec2(w - e, n - s));

    imageStore(direction_field, p, vec4(dir, 0, 0));
}
```

Single pass after distance field converges. Stored as `vec2` per cell (8 bytes per tile × 200×200 = 320 KB per field). Trivial memory cost.

#### Pawn movement system reads direction field

```csharp
public class FlowFieldMovementSystem : ISystem
{
    public void Update(SpanLease<PositionComponent> positions,
                       SpanLease<MovementIntentComponent> intents,
                       SpanLease<VelocityComponent> velocities,
                       WriteCommandBuffer writes)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            ref readonly var pos = ref positions.Span[i];
            ref readonly var intent = ref intents.Span[i];

            // Get direction from appropriate flow field for this pawn's target
            var fieldId = GetFlowFieldForTarget(intent.TargetId);
            var direction = nativeWorld.ReadField<Vector2>(fieldId, (int)pos.X, (int)pos.Y);

            writes.Update(positions.Entity[i], new VelocityComponent
            {
                Vector = direction * intent.Speed
            });
        }
    }
}
```

Per-pawn cost: 1 P/Invoke field read + arithmetic. No A\* per pawn. 200 pawns → 200 P/Invokes/tick → trivial.

#### When flow field wins, when A\* still needed

Flow field excellent for:

- Many pawns → same/few targets (work zones, mining areas, dining halls, escape routes)
- Static or slow-changing terrain (buildings, walls)
- Crowd behavior desirable (natural flow patterns emerge)
- Long-running pathing (pawns spend most time on established routes)

A\* still better for:

- Few pawns → many unique targets (medic to specific patient, builder to specific construction)
- Highly dynamic environment (constant re-routing not amortized over field lifetime)
- Strict path constraints per agent (different rules per agent type — flow field shared)

Hybrid is the right answer for Dual Frontier:

```
Flow fields:
  - "go to work zone X" (shared by all workers in zone)
  - "go to escape exit" (shared during emergencies)
  - "go to dining hall" (shared during meal time)
  - Common destinations (~5-20 fields active)

A*:
  - Specific entity targets (this medic to that wounded pawn)
  - Edge cases (rare unique paths)
  - Validation (verify flow field gradient leads to reachable target)
```

Architectural pattern: flow field default, fall back to A\* only when a specific destination is not covered by an active field. Most pawn movement covered by flow fields; A\* a fraction.

#### Engine vs mod placement

**Engine provides infrastructure:**

- Flow field as a field type (just `Vector2`-typed `RawTileField`)
- Standard gradient extraction shader (computes ∇ from distance field)
- Standard FSM / diffusion shaders for distance field computation

**Mods provide gameplay:**

- `Vanilla.Movement` defines pawns following flow fields
- `Vanilla.Movement` registers flow field types per target category
- `Vanilla.Movement` decides when to recompute fields (target changes, terrain changes, periodic refresh)

This preserves "vanilla = mods" (K-L9). Engine doesn't know about pathfinding semantics; it just provides the field abstraction + compute primitives.

Third-party mods can:

- Replace `Vanilla.Movement` with an alternative pathfinding strategy
- Add specialty fields (e.g. enemy avoidance field, pheromone trails)
- Compose multiple fields (combine work-zone + danger-avoidance)

**Field lifecycle.** Flow fields are per-target ephemeral resources:

- Created when first pawn requests path to destination
- Updated when terrain changes affect distance field
- Destroyed when no pawns using them (LRU eviction)
- Pool size capped (e.g. max 32 active flow fields)

Lifecycle managed by `Vanilla.Movement`, not engine. Same pattern as other mod-managed resources.

#### Open considerations

**Multiple flow fields compete for GPU memory.** 200×200 × `Vector2` (8 bytes) × 32 fields = 10 MB. Trivial. Larger map (1000×1000) × 32 fields = 256 MB. Still acceptable on modern GPUs. If memory becomes a constraint: hierarchical flow fields (room-level fine, world-level coarse) reduce memory. Future optimization, not blocker.

**Field refresh frequency.** Static terrain: compute flow field once per target, persist until terrain changes. Cheap. Dynamic terrain (combat damage, building destroyed): mark affected cells dirty, re-compute affected fields. More complex but bounded — only cells in radius of change need recompute. Per-tick refresh not required. Most fields stable for hundreds of ticks.

**Local avoidance is not free.** Flow field gives global direction, doesn't handle agent-agent collision. Pawns following identical gradient may cluster, jam, oscillate. Solutions:

- RVO (Reciprocal Velocity Obstacles) — established technique
- Simple separation force (boids-style) added to flow direction
- Spatial hashing for near-pawn awareness

Cost: per-pawn local steering (~100 µs for 200 pawns). Still much cheaper than per-pawn A\*. Lands as G8 below.

**Path quality validation.** Gradient-following gives locally optimal paths. Edge cases:

- Local minima (impossible with proper distance field, possible with naive diffusion)
- Tight passages (gradient may oscillate)
- Long detours (gradient may follow distance, not actual best route)

Mitigations: use eikonal (no local minima); smooth the gradient field (post-process); player tolerance is high for colony sim. For Dual Frontier gameplay this is not critical; minor path quirks are acceptable.

### Domain B kernel (Phase 3 carryover)

Original `ProjectileSystem` GPU implementation pattern preserved:

```glsl
layout(local_size_x = 64) in;
layout(binding = 0) readonly buffer ProjectileIn  { Projectile projectiles_in[]; };
layout(binding = 1) buffer       ProjectileOut { Projectile projectiles_out[]; };
layout(binding = 2) readonly buffer Obstacles  { Obstacle obstacles[]; };
layout(push_constant) uniform PC { float dt; uint count; } pc;

void main() {
    uint i = gl_GlobalInvocationID.x;
    if (i >= pc.count) return;

    Projectile p = projectiles_in[i];
    p.position += p.velocity * pc.dt;

    // Collision check against obstacle list
    for (uint j = 0; j < obstacles.length(); ++j) {
        if (intersects(p, obstacles[j])) {
            p.alive = 0;
            p.collision_target = obstacles[j].entity_id;
            break;
        }
    }

    projectiles_out[i] = p;
}
```

Dispatched once per tick. One-tick lag for visual representation (asynchronous readback). Threshold for adopting this path remains experimental — projectile count where CPU degrades — but the integration cost is now negligible compared to v1.0, so the threshold may shift downward in practice.

## Performance characteristics

### Hardware baseline assumption

Steam Hardware Survey median GPU class is RTX 3060/4060 territory as of authoring. Field math workload (200×200 grid × 10 iterations × 4 bytes per cell = 1.6 MB per field update) is bandwidth-trivial on any GPU shipped after 2018. Compute throughput requirements are noise-level relative to modern hardware.

This is a structural assumption: Dual Frontier targets users with compute-capable GPUs (essentially all gaming hardware of the past 6 years). A pure-CPU fallback is provided (see Failure modes below) but is not the optimization target.

### Domain A timing budget

Per-tick GPU compute work for typical scenario (3 active fields: mana + electricity + water):

- Field dispatch: 3 fields × 5–10 iterations × ~30 µs per dispatch = 0.45–0.9 ms
- Storage cell handling: included in shader, no separate dispatch
- Anisotropy lookup: included in shader, no separate dispatch
- Total: well under 1 ms per tick on mid-range GPU

CPU equivalent for same workload: 200×200 cells × 10 iterations × 3 fields × ~10 ops per cell = 1.2M ops per tick, sequentially executed = several milliseconds. Cumulative budget pressure on CPU as more fields added.

The CPU-vs-GPU comparison strongly favors GPU even at minimum scale. There is no crossover threshold; field workload is GPU-suitable from the first cell.

### Domain B timing budget

Threshold-driven (Phase 3 pattern preserved):

- Below ~500 entities: CPU implementation faster (no dispatch amortization)
- 500–5000 entities: GPU starts winning, lag tradeoff acceptable
- 5000+ entities: GPU dominates (CPU implementation visibly degrades)

Exact thresholds determined by per-system benchmarks ([PERFORMANCE](./PERFORMANCE.md) tracks measurements as systems gain GPU implementations).

### Cross-stack pipelining benefit

Architecture-level performance property (KERNEL + RUNTIME independence): tick N+1 simulating on CPU, tick N rendering recording, tick N-1 GPU executing all proceed concurrently. Field compute dispatches issued at end of tick run in background while next tick's CPU work proceeds.

GPU compute does not block CPU tick loop. Fence-based sync ensures next read sees consistent state without serializing execution.

## Architectural integration with existing systems

### Two-phase model fit ([KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §1.4)

**Phase A — Native bootstrap**: GPU compute pipelines registered during bootstrap. Vulkan instance/device acquired (shared with rendering layer). Compute pipelines compiled and cached. Storage buffers allocated for fields marked compute-managed.

**Phase B — Managed game tick**: managed scheduler invokes field update systems at appropriate phases. Each field update system makes a single P/Invoke to `df_world_field_dispatch_compute`. Native kernel records command buffer, submits to compute queue, returns. Managed code does not block on GPU; subsequent tick reads field via point queries (`ReadCell`) which see latest available state.

**Mod replacement** ([KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) §1.10): managed second graph rebuilds via `AssemblyLoadContext.Unload` + reload. Native kernel's compute pipeline registry persists. Pipelines registered by unloaded mods are deregistered explicitly during mod teardown (lifecycle hook).

### Direction-discipline (KERNEL K-L7)

Managed → native always. Field reads and writes flow through the managed bridge to native C ABI. Native kernel issues GPU compute to `vulkan-1.dll`. No reverse P/Invoke. No GPU-to-managed callbacks. No managed code in the GPU execution path.

This preserves the architectural property that contracts define every cross-layer transition explicitly. No hidden coupling. No race conditions across layers (each layer owns its data; cross-layer access is transactional via P/Invoke calls).

### Mod parity (KERNEL K-L9)

Vanilla mods register compute pipelines through the same `IModApi` as third-party mods. There is no engine-special compute pipeline path. `Vanilla.Magic`, `Vanilla.Electricity`, `Vanilla.Water` all use the same `api.ComputePipelines.RegisterPipeline(...)` call as a hypothetical `Vanilla.Climate` or third-party `PollutionMod`.

This is structural, not stylistic. The mod-OS architecture ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) defines vanilla as mods uniformly. GPU compute capability is part of the `IModApi` surface, not a privileged engine feature.

## Roadmap

### K9 — Field storage abstraction (1–2 weeks)

Prerequisite for all GPU compute work. Builds CPU functional path first.

Scope:

- `RawTileField<T>` C++ class with conductivity map and storage flags
- C ABI: register/read/write/acquire-span/set-conductivity/set-storage-flag
- Managed bridge: `FieldRegistry`, `FieldHandle<T>`
- CPU-side reference implementation of basic diffusion (for shader equivalence testing)
- Tests: round-trip, span access, mutation, conductivity update, storage flag toggle

Exit criteria: any field type registrable, readable, writeable from managed; CPU diffusion produces correct results on test grids; no GPU dependency yet.

### G0 — Vulkan compute pipeline plumbing (~1 week)

Establishes compute path; no game-visible behavior yet.

Scope:

- `VkBuffer` / storage image creation for fields marked compute-managed
- `VkDescriptorSetLayout`, `VkDescriptorPool`, `VkPipelineLayout` for compute
- Compute pipeline registration C ABI (`df_world_register_compute_pipeline`)
- Dispatch C ABI (`df_world_field_dispatch_compute`)
- Fence-based sync between CPU writes (conductivity updates) and GPU dispatch
- Build-time compute shader compilation in mod build process (extends [RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md) §1.7)
- Tests: empty dispatch (no-op pipeline) executes without error; pipeline registration round-trip

Exit criteria: managed code can register a compute pipeline and trigger empty dispatch; framework operational without specific gameplay use yet.

### G1 — First field compute shader (mana diffusion) (~1 week)

First production-shaped use case.

Scope:

- `Vanilla.Magic` mod registers `ManaField` (200×200, float32)
- Mana diffusion compute shader (isotropic, decay, source map)
- Mod startup wires field + pipeline + scheduled dispatch
- Test: mana sources spread spatially per shader; CPU reference and GPU output match within tolerance

Exit criteria: `ManaField` visibly diffuses across grid in-game; spell-casting systems can read local mana via point query; CPU/GPU equivalence verified on reference scenarios.

### G2 — Anisotropic diffusion (electricity) (~1 week)

Adds conductivity map handling.

Scope:

- `Vanilla.Electricity` mod registers `PowerField` + conductivity map
- Anisotropic diffusion shader with `min(D_self, D_neighbor)` flow gating
- Cliff threshold consumer effectiveness in managed code
- Wire placement updates conductivity map (player build action)
- Tests: power propagates along wire paths; off-wire decay matches expectation; cliff threshold offlines underpowered consumers

Exit criteria: electricity behaves as designed (sources, wires, consumers, brownouts) in stress test scenarios.

### G3 — Storage cells / capacitance (batteries, tanks) (~3–5 days)

Adds temporal capacitance to field shaders.

Scope:

- Storage flag handling in shader
- Battery placement updates storage flags + retention parameters
- Tests: battery accumulates excess during low demand; battery discharges during droop; load-smoothing visible in benchmark scenarios

Exit criteria: power and water systems both demonstrate storage cell behavior.

### G4 — Multi-field coexistence (~3–5 days)

Validates architecture with multiple simultaneous fields.

Scope:

- `Vanilla.Magic` + `Vanilla.Electricity` + `Vanilla.Water` all active simultaneously
- Independent dispatch scheduling per field (different update intervals)
- GPU memory budget verification (worst-case all-fields-active scenario)
- Cross-field interaction tests (none expected; verify isolation)

Exit criteria: three fields run concurrently without interference, within performance budget.

### G5+ — Domain B integration (`ProjectileSystem` reactivation) (~1 week)

Re-implements original Phase 3 `ProjectileSystem` GPU path on the new architecture.

Scope:

- `ProjectileSystem` registers a compute pipeline for projectile update + collision
- Native kernel exposes projectile component span as SSBO directly
- Asynchronous readback with one-tick lag (Phase 3 pattern)
- Threshold detection: managed code chooses CPU vs GPU based on entity count
- Performance comparison vs CPU baseline; threshold pinned per measurement

Exit criteria: `ProjectileSystem` on GPU validates Domain B pattern; threshold for swap measured; falls back to CPU below threshold.

### G6 — Flow field infrastructure (~3–5 days)

Engine-side flow field primitives. No gameplay binding yet.

Scope:

- Distance field compute shader (Option B simple diffusion variant initially)
- Direction field compute shader (gradient extraction)
- `Vector2` field type support in kernel (K9 extension)
- Tests: distance field converges; gradient points correctly toward target on representative grids

Exit criteria: distance field + direction field shaders run end-to-end on a synthetic obstacle grid; engine exposes `Vector2`-typed `RawTileField` cleanly.

### G7 — `Vanilla.Movement` integration (~1 week)

Pathfinding ships through a vanilla mod, replacing per-pawn A\* on common destinations.

Scope:

- Flow field lifecycle management (creation, refresh, eviction)
- Per-target field assignment logic
- Movement system reads direction field, applies to velocity
- Hybrid: A\* fallback for unique destinations (preserves [PERFORMANCE](./PERFORMANCE.md) `PathfindingService` for the residual case)
- Tests: pawn navigates to work zone via flow field; pawn navigates to specific entity via A\*; correct handoff between modes

Exit criteria: 50+ pawns navigating shared work zones consume O(K) field updates regardless of pawn count; A\* exercised only on unique-destination paths.

### G8 — Local avoidance layer (~3–5 days)

Local steering on top of flow field direction.

Scope:

- RVO-like or simple boids approach
- Combines flow field global direction + local agent collision avoidance
- Pure managed CPU code (per-pawn, but simple math, parallelizable)
- Tests: pawns following identical gradient do not cluster / jam / oscillate beyond tolerance

Exit criteria: crowd traversal through choke points without deadlock at target pawn counts.

### G9 — Eikonal upgrade (optional, ~1 week, evidence-gated)

Replace simple diffusion with Fast Sweeping Method when measurement justifies it.

Scope:

- Geodesic-accurate distances via FSM/FMM compute shaders
- Reuse direction field extraction unchanged
- Side-by-side comparison with Option B output on shipped scenarios

Exit criteria: only shipped if Option B measurement shows gameplay-relevant suboptimality. Otherwise the milestone closes without code, with the measurement archived.

### Future G milestones

Each new field-based mechanic gets its own G milestone (small, focused). Climate, fire spread, sound, scent, pollution, radiation, alarm propagation, terrain affordances, custom modder fields. ~3–5 days each. Compositional with the established infrastructure.

### Combined timeline

K9 + G0–G5 ≈ 6–9 weeks for foundational fields and Domain B reactivation. G6–G9 (flow field pathfinding overhaul) adds ~3–4 weeks. Combined with kernel (K0–K8) and runtime (M9.0–M9.8) pivots, the full architectural vision is 16–25 weeks.

## Failure modes and fallbacks

### CPU fallback for compute shaders

Not all hardware supports Vulkan 1.3 compute reliably. Some integrated GPUs and older laptops have driver issues. Pure software environments (CI, headless build agents) may lack GPU access entirely.

Each compute shader has a CPU reference implementation in managed code (originally written for shader equivalence testing during G1+). At startup, native kernel detects compute capability:

- Vulkan 1.3 + compute queue available: use GPU dispatch path
- Compute unavailable or disabled by config: managed scheduler invokes CPU reference implementation per tick

Performance on CPU fallback is significantly worse (orders of magnitude for large grids), but functionality is preserved. Game still runs; users see degraded performance rather than crashes.

CPU fallback is also mandatory for deterministic save snapshots (see below) if GPU determinism cannot be guaranteed across hardware.

### Determinism considerations

GPU compute results may vary across hardware/driver combinations due to floating-point ordering, parallel reduction differences, and driver optimizations. For Dual Frontier:

- Realtime simulation does not require bit-exact determinism (single-player, no replays)
- Save/load must produce reproducible state on load
- Network multiplayer (not currently scoped) would require strict determinism

Mitigation: CPU reference implementation produces canonical state for save snapshots. Save process pauses GPU dispatch, runs one CPU iteration to produce canonical field state, serializes that. On load, fields restored from canonical state; GPU dispatch resumes.

For hobby-scale single-player, slight non-determinism between sessions is acceptable. The CPU canonical save path is implemented but minimally exercised.

### Async sync hazards

Field reads from managed code use `ReadCell` (point query). If a compute dispatch is in flight, the read may see stale data (last frame's state) or new data (if dispatch completed). Either is acceptable for gameplay because:

- Field values are continuous and slow-changing
- Pawn systems read on per-tick cadence; one-tick stale data is invisible
- Cliff thresholds (electricity effectiveness) hysteresis-free; brief inconsistencies don't cascade

Hard sync (`waitIdle`) is available but only used for save snapshots and shutdown. Game tick path uses fence-based async sync (see [THREADING](./THREADING.md)).

### Memory budget

Each 200×200 float field = 160 KB per buffer × 2 (ping-pong) = 320 KB. Plus conductivity map (160 KB) and storage flags (40 KB). Total ~520 KB per active field.

Worst case 10 simultaneous fields: ~5.2 MB. Negligible on any modern GPU (typical 8 GB+ VRAM).

Field grid size scales with map size. 1000×1000 future map size: ~13 MB per field, 130 MB for 10 fields. Still trivial.

## Decision log

### Why field abstraction in K9 vs deferring fields to G-series

Field storage in K9 (CPU-functional first) ensures the abstraction is sound independent of GPU layer. If field design has flaws, they surface in K9 testing, not after G-series Vulkan work is invested.

Field abstraction in K9 also unblocks systems that read fields (consumer effectiveness, mana availability, water pressure) for testing without requiring full G-series integration. Game design iteration happens on CPU path; G-series adds performance, not functionality.

### Why anisotropy and storage cells from the start

Adding anisotropy retroactively to an isotropic field implementation requires reworking shader signature, native storage layout, and conductivity update API. Doing this once after G1 ships would be invasive.

Adding it from K9 design means storage layout includes conductivity map; shader signature accommodates per-cell D; update API ships from day one. Cost is small (additional 160 KB per field, negligible shader complexity); benefit is no rework when electricity/water mechanics arrive.

Same logic for storage cells.

### Why cliff threshold instead of soft cutoff

The 60% cliff (consumer offline below threshold) is gameplay-driven, not technically motivated. Alternatives considered:

- **Soft cutoff**: consumer effectiveness scales smoothly to 0 — produces complex equilibrium dynamics, cascading partial failures, hard to reason about
- **Hard cutoff at 100%**: consumer either runs at full power or not at all — too binary, no brownout gameplay
- **60% cliff**: matches industrial design intuition (motors stall below ~60% rated voltage), provides clear failure threshold, self-stabilizing (offlined consumers free capacity for marginal others)

The cliff is a parameter per consumer type, exposed for modder tuning. Default 0.6 for most electrical consumers; magic and water may use different defaults.

### Why one shader per field type vs. parameterized super-shader

A single shader with all features (anisotropy, storage, decay, multiple sources) and runtime branches is possible but slower (GPU branch divergence), less maintainable (one shader handles all field types), and harder to specialize (mana doesn't need anisotropy; sound doesn't need storage).

Per-field shaders compile separately, each tuned for its mechanic. Compilation cost is amortized at startup. Runtime dispatch cost is the same regardless of how many shaders exist (pipeline binding is constant-cost).

Mods own their shaders; engine provides registration, dispatch, and resource management.

### Why GPU compute is not deferred (vs. v1.0 deferral)

V1.0 deferral was correct for managed runtime architecture. With native kernel + Vulkan rendering layer committed:

- Dispatch overhead is microseconds, not milliseconds
- Vulkan instance already live; no marginal setup cost
- Component data already in GPU-friendly layout
- Field abstraction makes compute the natural implementation, not a foreign capability

Deferring GPU compute now would mean implementing fields on CPU first, then refactoring to GPU later — invasive change to working code. Doing GPU from the start (after K9 abstraction stable) avoids the refactor and grounds the architecture in its target form.

Domain B (entity-keyed bulk compute) retains a deferral story: still threshold-driven, still benchmarked. `ProjectileSystem` can run on CPU until measurements justify swap. This preserves the v1.0 `ProjectileSystem` reasoning as a special case within the broader v2.0 framework.

## Architectural moat implications

GPU compute as foundational architectural capability, combined with field-based gameplay mechanics, creates a structural performance gap vs. CPU-bound competitors in the simulation genre.

Steam Hardware Survey median GPU is RTX 3060/4060 class (compute-capable). Major incumbents (RimWorld on Unity, Dwarf Fortress single-thread C++, Prison Architect, Oxygen Not Included, Factorio) are all CPU-bound architecturally and cannot retrofit GPU compute without major engine rewrites.

Dual Frontier's structural advantages:

- Vulkan compute already in scope (rendering uses Vulkan)
- Native kernel exposes SoA data layout (SSBO-friendly)
- Contract architecture absorbs new compute mechanics additively
- "Vanilla = mods" democratizes compute capability to mod authors
- Each new field mechanic is mod-level work (~half day), not engine work

Performance projection: 200+ pawns with multiple field mechanics affordable on mid-range hardware where competitors struggle with 100 pawns and zero field math. Gap widens as Dual Frontier matures and competitors hit architectural walls.

### Pathfinding scaling — secondary moat

Per-agent A\* in incumbents: RimWorld (Unity, performance wall at 100+ pawns), Dwarf Fortress (per-dwarf pathfinding contributing to "FPS death"), Oxygen Not Included (per-duplicant pathfinding limits scaling), Factorio (per-entity pathfinding pressure). All have linear-or-worse pathfinding overhead in pawn count.

Dual Frontier flow field approach (Domain A extension above):

- Pathfinding cost decouples from pawn count
- 50 pawns or 500 pawns: same K flow fields active, shared
- GPU computes flow fields in microseconds
- Per-pawn cost = single field read

| Workload                    | Per-pawn A\* (incumbents) | Flow field (DF)            |
|-----------------------------|---------------------------|----------------------------|
| 50 pawns, 5 work zones      | ~5 ms / tick              | ~200 µs / tick             |
| 500 pawns, 5 work zones     | ~50 ms / tick (unfeasible)| ~250 µs / tick             |
| 5000 pawns, 5 work zones    | structurally impossible   | ~400 µs / tick             |

Order-of-magnitude advantage at scale. Scaling property fundamentally different from competitors.

### Capability comparison

| Capability                                                      | Dual Frontier                          | RimWorld                  | Dwarf Fortress    | Comments                       |
|-----------------------------------------------------------------|----------------------------------------|---------------------------|-------------------|--------------------------------|
| Field-based supply networks (electricity / water / heat)        | ✓ GPU, additive                        | ✗ explicit graphs         | ✗ explicit graphs | Order of magnitude faster      |
| Pathfinding scaling                                             | ✓ flow field, decoupled from pawn count| ✗ A\* per pawn            | ✗ A\* per dwarf   | Structural advantage           |
| Crowd behavior emergence                                        | ✓ natural from flow gradient           | partial (utilities-based) | minimal           | Gameplay depth bonus           |
| Mod-extensible compute mechanics                                | ✓ uniform abstraction                  | ✗ engine-locked           | ✗ engine-locked   | Mod ecosystem advantage        |
| Performance ceiling                                             | ~500–1000 pawns realistic              | ~150 pawns ceiling        | similar           | 3–7× capacity                  |

Flow field substantively raises performance ceiling independently of electricity / water / heat speedup. Combined effect: Dual Frontier can run scenarios literally inaccessible on the same hardware to incumbents.

This is not a commercial positioning argument (project is hobby+polygon framing); it is an architectural validation argument. The pivots committed in [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) and [RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md) compound to enable simulation depth structurally inaccessible to incumbents. GPU compute is the lever that translates architecture into gameplay capability; field abstraction is the pattern that lets each new gameplay mechanic (supply networks, pathfinding, alarms, scent, climate) reuse the same infrastructure rather than inflate it.

## See also

- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — native ECS kernel, K9 field storage abstraction
- [RUNTIME_ARCHITECTURE](./RUNTIME_ARCHITECTURE.md) — Vulkan rendering layer, shared `VkInstance`/`VkDevice`
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — modding architecture, `IModApi`, vanilla = mods
- [PERFORMANCE](./PERFORMANCE.md) — benchmark tracking, threshold measurements per system
- [THREADING](./THREADING.md) — async sync patterns, fence-based GPU coordination
- [ROADMAP](./ROADMAP.md) — overall milestone sequencing
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — pipeline patterns, brief authoring conventions
- [CPP_KERNEL_BRANCH_REPORT](./CPP_KERNEL_BRANCH_REPORT.md) — Discovery report establishing K0 cherry-pick scope

**LOCKED v2.0** — supersedes Phase 3 `GPU_COMPUTE.md`. Departures require explicit re-architecture milestone and updates to dependent K9/G-series briefs.
