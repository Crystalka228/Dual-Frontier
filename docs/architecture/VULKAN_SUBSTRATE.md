---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-VULKAN_SUBSTRATE
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.1.1"
next_review_due: 2027-05-16
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-VULKAN_SUBSTRATE
---
# Vulkan Substrate (V) — Dual Frontier

**Status:** AUTHORITATIVE LOCKED v1.0 — unified Vulkan substrate spec covering rendering + compute use cases.

**Supersedes:** prior `RUNTIME_ARCHITECTURE.md` v1.0 LOCKED (rendering layer spec) + `GPU_COMPUTE.md` v2.0 LOCKED (compute layer spec). Both source documents were physically describing **one** Vulkan device with two use cases; the documentation drift introduced separate substrate identities for what is one physical layer. Per Q-G-1 LOCK in `docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §3.1, R (runtime) and G (GPU compute) substrate buckets merged into unified Vulkan substrate **V**.

**Companion documents:** [METHODOLOGY](/docs/methodology/METHODOLOGY.md), [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [ARCHITECTURE](./ARCHITECTURE.md), [THREADING](./THREADING.md), [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md), [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md), [FIELDS](./FIELDS.md), [ROADMAP](/docs/ROADMAP.md).

**Scope:** Full architectural specification for the Vulkan substrate (V) — single `VkInstance` / `VkDevice` / `vulkan-1.dll` linkage serving both 2D rendering and compute. Defines substrate primitives V0/V1/V2 (compute-side), rendering use case implementation, compute use case implementation, threading model, asset pipeline, shader strategy, mod-driven compute pipeline registration, mathematical models for field-based gameplay mechanics, failure modes, and the unified roadmap toward full architectural foundation. The Domain layer ([ARCHITECTURE §Domain](./ARCHITECTURE.md), [ECS](./ECS.md), [EVENT_BUS](./EVENT_BUS.md), [ISOLATION](./ISOLATION.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) is preserved verbatim by this layer — see L10 in §0.

**Version history:**

- **v1.1.1 (2026-05-23, this version)** — К-extensions cascade #2 closure patch bump per Q-G-12 LOCKED. Substantive content unchanged; cleanup-only patch records Godot deprecation + Launcher formalization status. R.8 «Migration cutover (delete Godot)» step in §6 marked complete (К-extensions cascade #2 closure: tracked Godot files removed, DualFrontier.Launcher production renderer formalized per Q-G-6 (b1) infrastructure-only). Migration narrative throughout §4 retained as historical record of pre-cascade-#2 state — readers should treat "parallel Godot+Vulkan" framing as past-tense reference к the pre-cascade migration approach.

- **v1.1 (2026-05-20)** — К10.3 v2 load-bearing commit 1/3 reconciliation per S-LOCK-14. Consolidates: (a) К-L19 deferred V0.B amendments (§0 L1 Vulkan 1.3 + async compute queue mandate notation; §0 L7 К-L19 hardware tier baseline note; §3.4 К-L19 mandate documentation); (b) К10.3 v2 К-L7.1/L16 amendments (§2 pipeline depth architecture subsection; §2.3 threading model pipeline depth + queue family roles subsection; §7.2 pipeline drain semantics; §7.3 pipeline slot tail read pattern). К-L17 + К-L18 amendments land в subsequent К10.3 v2 load-bearing commits.

- **v1.0 (2026-05-16)** — unified V substrate spec authored as cascade output of composite namespace ratification (Q-G-1 + Q-G-2 LOCK). Consolidates verbatim content from `RUNTIME_ARCHITECTURE.md` v1.0 (foundation decisions L1–L10, rendering migration sequencing, module structure, threading, asset pipeline, shader strategy) and `GPU_COMPUTE.md` v2.0 (two compute domains, field abstraction, mathematical models, flow field pathfinding, failure modes). Restructured per Q-G-2 LOCK: substrate primitives reduced from G0..G6+G9 (seven items) to V0/V1/V2 (three items) via six successive reductions (storage as gameplay metadata, wave shader as gameplay-driven, two-layer model, distribution+navigation unified, CPU/GPU policy decoupled, autonomous=GPU persistent). Упразднено: G3 storage cells (now gameplay-level node config), G6 flow field infrastructure (folded into V2 wave shader side products), G4 multi-field coexistence (now substrate close acceptance criterion, not separate primitive). Deferred: G5 projectile Domain B (substrate identity TBD), G9 eikonal upgrade (folded into V2 tunable or separate primitive — evidence-gated). M-V demonstrations cited per Q-R-1 format (M-V1 mana, M-V2 electricity, M-V5 projectile (deferred), M-V7 movement, M-V8 local avoidance; gaps M-V3/M-V4/M-V6/M-V9 reflect Q-G-2 reductions).

---

## Preamble — How to use this document

**Authority.** This document is the single architectural authority for the Dual Frontier Vulkan substrate (V) — window management, GPU rendering, input, sprite batching, text, asset loading, diagnostic tooling, compute pipeline plumbing, field-based compute mechanics. During implementation of substrate primitives V0/V1/V2 and the rendering use case migration, every interface, P/Invoke declaration, struct layout, and lifecycle step traces back to a section here. Disagreement with the specification is escalated to the human (via §4 open decisions) — never resolved by improvisation in code, mirroring the discipline established for the modding layer in [MOD_OS_ARCHITECTURE Preamble](./MOD_OS_ARCHITECTURE.md).

**Scope.** The specification governs:

- The structural relationship between the V substrate and the existing four layers ([ARCHITECTURE](./ARCHITECTURE.md)).
- The Win32 + Vulkan P/Invoke surface (functions, structs, enums, callbacks).
- The Vulkan resource lifetimes (instance, device, swapchain, pipeline, buffer, image, memory, compute pipeline, descriptor sets).
- The sprite + text + atlas batching model (rendering use case).
- The PNG decoder and shader compilation pipeline.
- The threading model on top of [THREADING](./THREADING.md) (window+render thread merged with simulation thread preserved).
- The compute pipeline plumbing (V0 substrate primitive) for both Domain A (field updates) and Domain B (entity-keyed bulk computation).
- Scalar field primitives V1 (diffusion shader) and V2 (wave shader) — substrate-level abstractions consumed by vanilla mods as gameplay mechanics.
- The migration sequencing from the current dual-backend Godot+Silk.NET state ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)) toward the locked Vulkan target — historically tracked as M9.0..M9.8 runtime milestones, now unified within V substrate (Q-R-2 LOCK).

The specification does **not** govern:

- Domain content — Domain is preserved verbatim, see [ARCHITECTURE](./ARCHITECTURE.md), [ECS](./ECS.md), [CONTRACTS](./CONTRACTS.md), [EVENT_BUS](./EVENT_BUS.md).
- The mod system — covered by [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [MODDING](./MODDING.md), [MOD_PIPELINE](./MOD_PIPELINE.md). V substrate exposes presentation + compute primitives only; mods cannot reach the substrate layer directly except through `IModApi` v3 surface (`Fields`, `ComputePipelines` sub-APIs).
- Game-design questions (balance, narrative, pacing).
- Field storage data layout — owned by [FIELDS](./FIELDS.md) and [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9 (`RawTileField<T>`). V substrate consumes the field storage primitives; it does not define them.
- Methodology of the development pipeline — covered by [METHODOLOGY](/docs/methodology/METHODOLOGY.md).

**The "stop, escalate, lock" rule.** When implementation encounters a design question not answered here, the response is "stop, document in §4, wait for the human to lock" — not "guess." Same discipline as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) Preamble.

---

## Executive summary

Dual Frontier's Vulkan substrate (V) is a **unified Vulkan 1.3 layer** serving two use cases on one `VkInstance` / `VkDevice` / `vulkan-1.dll` linkage:

- **Rendering use case** — Win32 window, swapchain, sprite batching, bitmap text, atlas-based 2D rendering. Migration target replacing Godot 4 + C# Presentation layer ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)).
- **Compute use case** — field-based GPU compute (Domain A) + entity-keyed bulk computation (Domain B). Substrate-level abstraction for diffusion / wave / flow field gameplay mechanics. Mod-driven shader registration.

The Domain layer ([ARCHITECTURE](./ARCHITECTURE.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [THREADING](./THREADING.md), [ISOLATION](./ISOLATION.md)) is preserved verbatim — zero touch by V substrate work. Only the Presentation layer ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)) is rewritten on the new foundation.

**Foundation philosophy** — «без компромиссов»:

- Pure P/Invoke to `vulkan-1.dll` (no third-party C# binding library).
- Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`).
- BCL only for math (`System.Numerics`) and compression (`System.IO.Compression.DeflateStream`).
- Manual PNG decoder (DEFLATE through BCL, chunk parsing manual).
- Build-time GLSL → SPIR-V via `glslangValidator.exe`.
- Production binary depends only on `vulkan-1.dll` (GPU driver) and pre-compiled `.spv` shader files.

Total ownership: every line above OS API surface is project's own code.

**Architectural insight (Q-G-1 + Q-G-2):** Rendering and compute share one Vulkan device. Treating them as separate substrates (R-bucket and G-bucket) was documentation drift. Compute substrate primitives reduce to three items (V0 plumbing, V1 diffusion, V2 wave) once gameplay mechanics are recognized as **configurations** of physical primitives rather than substrate primitives in their own right (cf. Lesson #12 candidate, deliberation document §6.3). Distribution networks (mana, electricity, water, heat), navigation (flow-field pathfinding), and crowd behavior all reduce to V1+V2 configuration — substrate stays small, gameplay stays expressive.

**Estimated scope:** Rendering use case migration to full M8.x parity on Vulkan — 4–7 weeks at hobby pace (~1h/day). V substrate compute primitives V0/V1/V2 — 3–5 weeks at hobby pace. M-V demonstration mods (M-V1 mana, M-V2 electricity, M-V5 projectile, M-V7 movement, M-V8 local avoidance) — additional 3–5 weeks combined.

---

## 0. Locked foundational decisions

The following decisions are committed as architectural foundation. Departures require an explicit re-architecture milestone, not spec-level adjustments mid-implementation. The locked-decision protocol mirrors [MOD_OS_ARCHITECTURE §0](./MOD_OS_ARCHITECTURE.md).

| #   | Decision               | Choice                                                                           | Rationale                                                            |
| --- | ---------------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------------------- |
| L1  | GPU API                | Vulkan 1.3 + async compute queue family mandate (К-L19 V0.B; К-L16 К10.3 v2)     | Future-proof, total control, modern GPU pipeline (rendering + compute + async dispatch per К-L16) |
| L2  | Vulkan bindings        | Pure P/Invoke to `vulkan-1.dll`                                                  | Zero third-party C# in production binary                              |
| L3  | Window/OS surface      | Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`)                               | Same — zero third-party for OS surface                                |
| L4  | Math                   | `System.Numerics` (BCL only)                                                     | BCL is .NET runtime surface, not third-party                          |
| L5  | PNG loading            | Manual decoder + `System.IO.Compression.DeflateStream` (BCL)                     | DEFLATE is BCL, chunk parsing manual ~500 lines                       |
| L6  | Shader strategy        | Build-time GLSL → SPIR-V via `glslangValidator.exe` (both graphics + compute)    | Production binary has no shader compiler dependency                   |
| L7  | Initial platform       | Windows-only (matches К-L19 hardware tier baseline — Vulkan 1.3 + async compute) | Cross-platform deferred — adds SDL2/GLFW dep or manual X11/Cocoa      |
| L8  | Threading              | Window+Render thread merged + Simulation thread (existing GameLoop preserved)    | Minimal change to domain; see [THREADING](./THREADING.md)             |
| L9  | Migration approach     | Parallel — kept Godot Presentation functional until rendering cutover (cutover completed К-extensions cascade #2, 2026-05-23) | Honest state always available                                         |
| L10 | Domain layer treatment | Preserved verbatim — zero modification                                           | Mature ECS proven; not throwing away tests + simulation work          |

**Implication of L7.** Project becomes Windows-only until explicit cross-platform milestone. macOS/Linux support deferred indefinitely (or through SDL2 layer accepted as «pragmatic compromise» if needed). See §4 open decision «Cross-platform support».

**Implication of L1 (К-L19 V0.B + К-L16 К10.3 v2).** Async compute queue family mandatory at startup per К-L19 hardware tier commitment (V0.B closure 2026-05-18). К10.3 v2 К-L16 pipeline depth (D=1-3, default 2) consumes the async compute queue для pipeline-managed dispatches (Phase.Compute scheduler integration per native phase_compute.h). К-L7.1 sub-invariant binds pipeline-managed FieldStorageSnapshot к slot tail — sim-thread reads see one-tick lag (К-L7 atomic-from-observer preserved within slot boundary). К-L9 «Vanilla = mods» — opt-in per field; V1 К-L7 sync dispatch_compute_field path preserved для existing consumers.

**Implication of L10.** All existing namespaces under `DualFrontier.Core`, `DualFrontier.Contracts`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Systems`, `DualFrontier.Application`, `DualFrontier.Modding`, `DualFrontier.Persistence` are untouched. The existing test suite passes throughout migration ([TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md)). Mod system contracts ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) remain unchanged — V substrate is not visible from a mod's `AssemblyLoadContext` directly; mods reach Vulkan compute via `IModApi.ComputePipelines` and field storage via `IModApi.Fields` (v3 surface, K8.4 closure).

---

## 1. V substrate primitives (Q-G-2 LOCK)

The V substrate is built from three primitives, in correct order. Each primitive is a **substrate-building milestone**; closure of all three plus multi-field-coexistence acceptance criterion (the former G4, now subsumed) constitutes V substrate close.

### 1.1 V0 — Vulkan substrate foundation

**Scope:** All Vulkan plumbing shared by rendering and compute use cases. Single `VkInstance`/`VkDevice` linkage to `vulkan-1.dll`. Validation layer setup. SPIR-V toolchain. Memory allocator. Compute pipeline plumbing. Render pipeline plumbing. Win32 window + surface + swapchain. Threading model (window+render thread merged, simulation thread preserved).

**Historical context:** Pre-Q-G-1, this scope was split across `RUNTIME_ARCHITECTURE.md` (rendering side: Win32, swapchain, sprite/text/atlas plumbing) and `GPU_COMPUTE.md` G0 (compute-pipeline-only plumbing). Q-G-1 LOCK recognizes that both sides share one `VkInstance`/`VkDevice` physically; the substrate identity is one (V0), the use cases are two (rendering, compute).

**Rendering side deliverables** (former M9.0..M9.7 work, now V0 rendering use case implementation phases — see §6 Roadmap):

- Win32 window + surface (`vkCreateWin32SurfaceKHR`)
- Vulkan instance + physical/logical device + queue family selection
- Validation layer (`VK_LAYER_KHRONOS_validation`) enabled in DEBUG
- Swapchain + recreation on `WM_SIZE`
- Render pass + graphics pipeline + command pool/buffer
- Vertex buffer + image + memory allocator (bumper allocator initially)
- PNG decoder + asset manager
- Sprite atlas + sprite batcher + `Camera2D` orthographic projection
- Bitmap font + text renderer (sprite-pipeline-based)
- Input event queue (Win32 messages → typed events)
- Debug overlay + frame timer + validation log

**Compute side deliverables** (former G0 work, now V0 compute use case implementation):

- Compute pipeline (`VkPipeline` with `VK_PIPELINE_BIND_POINT_COMPUTE`)
- Compute descriptor set layout + descriptor pool + pipeline layout
- Storage buffer / storage image creation for fields marked compute-managed
- Compute pipeline registration C ABI (`df_world_register_compute_pipeline`)
- Compute dispatch C ABI (`df_world_field_dispatch_compute`)
- Fence-based sync between CPU writes (conductivity updates) and GPU dispatch
- Build-time compute shader compilation extending §3.2 shader strategy (same `glslangValidator.exe` toolchain)
- Tests: empty dispatch (no-op pipeline) executes without error; pipeline registration round-trip

**Exit criteria:**

- Window opens (Win32), Vulkan instance + device live, validation layer reports zero errors.
- Clear color rendered at 60+ FPS.
- Compute pipeline registration round-trip works; empty dispatch executes without error.
- All existing Domain tests pass (Domain layer untouched).
- Clean shutdown (no leaked Vulkan handles per validation).

**Estimated:** 4–6 weeks at hobby pace combining rendering + compute foundation work.

### 1.2 V1 — Scalar field + diffusion shader (environmental layer)

**Scope:** First substrate-level compute primitive. Scalar field type backed by `RawTileField<T>` (K9 storage) with **isotropic diffusion shader** describing environmental distribution.

**Mathematical model** (isotropic diffusion, ~30 LOC GLSL):

```
∂P/∂t = D · ∇²P + S(x,y) - K · P
```

4-neighbor stencil, single diffusion coefficient `D`, optional source map `S(x,y)`, optional decay coefficient `K`. Ping-pong between two image/buffer resources, 5–10 iterations per dispatch to reach near-equilibrium.

**Anisotropic variant** (electricity / water / pipes / wires):

```
∂P/∂t = ∇·(D(x,y) · ∇P) + S(x,y) - C(x,y) · effectiveness(P)
```

Per-cell `D` varies. Wire/pipe tiles have `D ≈ 10.0`; off-path tiles have `D ≈ 0.1`; insulators have `D = 0.0`. The asymmetric flow `min(D_self, D_neighbor)` between each tile pair guarantees flow blocked when either tile is non-conductor. Wire path channels propagation automatically; "narrow wave" is emergent, not coded.

**Gameplay configurations** (mod-level concern, NOT substrate primitive — cf. Lesson #12 candidate): mana density, electricity field, water pressure, heat distribution, sound pressure, scent concentration, pollution, radiation, modder-defined environmental fields. Each gameplay configuration is one `RawTileField<T>` + diffusion shader instance with tuned `D` and conductivity map; substrate primitive V1 is the **mechanism**, not the specific application.

**Storage interaction:** V1 consumes `RawTileField<T>` from K9 (see [FIELDS](./FIELDS.md), [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9 section). Per-field conductivity map enables anisotropic variants. Per-field storage flags (former G3 storage cell feature) handled at gameplay-level node configuration — V1 substrate primitive does not include shader-level capacitance.

**M-V1 demonstration:** Vanilla.Magic mod (`ManaField`) — see §6 Roadmap and Q-R-1 format. Mana diffusion is the first production-shaped V1 use case.

**Exit criteria:**

- Mana sources spread spatially per shader; CPU reference (for shader equivalence testing) and GPU output match within tolerance.
- Spell-casting systems can read local mana via point query.
- Anisotropic variant (electricity) propagates along wire paths; off-wire decay matches expectation.
- Conductivity update API exercised (player build action triggers wire layout change).

**Estimated:** 1–2 weeks at hobby pace (substrate primitive itself; per-mod gameplay configurations are separate M-V milestones).

### 1.3 V2 — Scalar field + wave shader (routed layer)

**Scope:** Second substrate-level compute primitive. Scalar field type with **wave-propagation shader** through discrete topology overlay. Routed (not isotropic). **Breakable** — wave propagation respects walls / cliffs / closed pipes / cut cables. Distance and direction fields produced as **side products** of wave propagation; no separate flow-field-infrastructure primitive needed (former G6 folded into V2).

**Mathematical model — distance field via diffusion (Option B, baseline)**:

```
∂D/∂t = ∇²D - K·D + spike_at_target
```

Does not give geodesic-accurate distances but correctly produces a gradient pointing toward the target. Cheaper to compute, less mathematical machinery, ~99% gameplay-equivalent for colony sim. For Dual Frontier this quality is enough — pawns don't optimize paths, players don't notice 5% suboptimal routing.

**Mathematical model — distance field via eikonal equation (Option A, deferred — see §1.3.1)**:

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

**Direction field (gradient extraction)** — after distance field stable, single pass:

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

Stored as `vec2` per cell (8 bytes per tile × 200×200 = 320 KB per field). Trivial memory cost.

**Gameplay configurations** (mod-level concern): routed flow fields for pathfinding ("go to work zone X", "go to escape exit", "go to dining hall"), broken-cable / broken-pipe propagation in supply networks (water in pipes that respects pipe break, electricity that respects cable cut). Each gameplay configuration is one V2 instance with tuned parameters; substrate primitive V2 is the mechanism.

**Hybrid coupling note** (TBD — deferred to amendment authoring): how diffusion (V1) picks up from a broken wave node (V2). Example: water in pipes propagates via V2 wave shader respecting pipe topology, but on break the water diffuses ambient via V1. Coupling spec lives in V substrate authoring at amendment time.

**M-V2 demonstration:** Vanilla.Electricity mod — see §6 Roadmap and Q-R-1 format. Wave through cables with breakable propagation.

**M-V7 demonstration:** Vanilla.Movement pathfinding via V2 routed flow field — see §6 Roadmap.

**Exit criteria:**

- Distance field converges to correct gradient on representative grids.
- Direction field extraction produces walkable gradient toward target.
- Wave propagation respects walls / closed-pipe / cut-cable barriers (verified in synthetic obstacle scenarios).
- M-V2 (electricity through cables) and M-V7 (pawn navigation via flow field) demonstrate V2 use cases.

**Estimated:** 2–3 weeks at hobby pace (substrate primitive itself plus distance/direction shader integration).

#### 1.3.1 G9 eikonal upgrade (deferred TBD)

The eikonal solver (Option A) yields geodesic-accurate distances; Option B (simple diffusion) does not. Deliberation §3.2 deferred the question of whether eikonal is V2 tunable parameter (compile-time / dispatch-parameter selection between diffusion and eikonal) or a separate V-primitive. Resolution gated on measurement evidence: if Option B baseline shows gameplay-relevant suboptimality, the upgrade to Option A is justified; otherwise diffusion baseline remains. Brief authoring at amendment time captures the evidence.

#### 1.3.2 G5 projectile Domain B (deferred TBD)

Per-entity bulk computation (the original `ProjectileSystem` GPU path, Domain B below in §5) is a separate compute domain from V1/V2 field updates. Deliberation §3.2 deferred whether this stays within V substrate (as a third primitive or as part of V0 compute plumbing), becomes own substrate, or stays consumer-level. M-V5 namespace reserved (per Q-R-1) but identifier and substrate disposition deferred to future deliberation post-substrate work.

### 1.4 V substrate close acceptance criteria

V substrate is **closed** when:

- V0 (foundation) — rendering use case at full M8.x parity on Vulkan; compute use case empty-dispatch + pipeline registration round-trip working.
- V1 (diffusion) — M-V1 mana diffusion demonstration shipping; anisotropic variant (M-V2 electricity wire propagation) shipping.
- V2 (wave) — M-V7 routed flow field pathfinding demonstration shipping.
- **Multi-field coexistence** (former G4, now acceptance criterion) — V1 mana + V1-anisotropic electricity + V2 routed-water-pressure all active simultaneously without interference, within performance budget (~1 ms/tick per active field on mid-range GPU).

V substrate close gates Phase B M-cycle vanilla content mass migration. Future V-N primitives (V3+) reserved for post-substrate compute needs (G5 projectile resolution, G9 eikonal upgrade if evidence justifies, modder-driven primitives).

---

## 2. Architecture

### 2.0 Pipeline depth architecture (К-L16, К10.3 v2 amendment)

The V substrate supports simulation tick pipeline depth D=2 (default, configurable 1-3) per К-L16 для pipeline-managed dispatches. Simulation thread runs D ticks ahead of display thread для these dispatches; cross-layer async operations (GPU compute pipeline-managed, network, disk I/O) have full pipeline-depth window к complete без blocking simulation thread.

Pipeline slot data model (verbatim from `KERNEL_FULL_NATIVE_SCHEDULER.md` §3.10 Item 33; implementation в `native/DualFrontier.Core.Native/include/pipeline_slot.h`):

```c
typedef enum {
    SlotState_Empty = 0,
    SlotState_Dispatched = 1,
    SlotState_FenceCompleted = 2,
    SlotState_ReadableAsTail = 3
} SlotState;

typedef struct {
    uint64_t sim_tick;
    void* world_snapshot_ptr;
    void* fields_snapshot_ptr;   // К-L7.1 binding subject
    void* compute_fence_handle;  // VkFence opaque
    int32_t state;
} PipelineSlot;
```

Sim-thread reads see slot tail state для pipeline-managed fields (К-L7.1 sub-invariant): sim tick T+D reads dispatched-at-(T+D-1) state. One-tick lag bounded и deterministic.

Display thread reads from CurrentSimTick - D для pipeline-managed display state — display latency invariant established по К-L16.

**К-L7 sync coexistence (S-LOCK-10 + S-LOCK-13)**: V1's existing `V1DiffusionPipeline.ExecuteIteration` synchronous dispatch path (К-L7 atomic-from-observer per `compute_dispatch.h`) remains operational orthogonal к К-L16. К-L7.1 is opt-in для new pipeline-managed consumers; К-L7 is default для existing V1 consumers. К-L9 «Vanilla = mods» preserved — author choice per field.

### 2.1 Project structure (post-migration target)

```
src/
  // ====== Domain layer (preserved verbatim — zero touch) ======
  DualFrontier.Core/
  DualFrontier.Contracts/
  DualFrontier.Components/
  DualFrontier.Events/
  DualFrontier.Systems/
  DualFrontier.Application/
  DualFrontier.Modding/
  DualFrontier.Persistence/

  // ====== V substrate (NEW — Vulkan + Win32 foundation) ======
  DualFrontier.Runtime/
    DualFrontier.Runtime.csproj
    MODULE.md                                 // module purpose + dependencies
    Runtime.cs                                // top-level facade — entry point

    Native/
      MODULE.md
      Win32/
        MODULE.md
        Win32Api.cs                           // [LibraryImport] declarations
        Win32Constants.cs                     // WM_*, WS_*, CS_* constants
        Win32Structs.cs                       // WNDCLASSEX, MSG, RECT, POINT
        WindowProc.cs                         // window procedure callback
      Vulkan/
        MODULE.md
        VkApi.cs                              // [LibraryImport] vk* functions
        VkEnums.cs                            // VkResult, VkFormat, VkImageLayout
        VkStructs.cs                          // VkInstanceCreateInfo, etc.
        VkConstants.cs                        // VK_API_VERSION_1_3, VK_NULL_HANDLE
        VkDelegates.cs                        // function pointer signatures

    Window/
      MODULE.md
      IWindow.cs                              // interface — window contract
      Window.cs                               // Win32 implementation
      WindowOptions.cs                        // creation parameters
      InputEventQueue.cs                      // raw OS events → typed events

    Input/
      MODULE.md
      IInputEvent.cs                          // marker interface
      KeyPressedEvent.cs / KeyReleasedEvent.cs
      MouseMovedEvent.cs / MouseButtonEvent.cs / MouseWheelEvent.cs
      WindowResizedEvent.cs / WindowFocusEvent.cs
      Key.cs / MouseButton.cs                 // enums

    Graphics/
      MODULE.md
      VulkanInstance.cs                       // VkInstance lifecycle (shared with compute)
      VulkanDevice.cs                         // physical/logical device (shared with compute)
      VulkanSurface.cs                        // VkSurfaceKHR (Win32) — rendering use case
      VulkanSwapchain.cs                      // swapchain + recreation
      VulkanCommandPool.cs                    // command pool/buffer mgmt
      VulkanRenderPass.cs                     // render pass abstraction
      VulkanPipeline.cs                       // graphics pipeline state
      VulkanBuffer.cs                         // VkBuffer + memory allocation
      VulkanImage.cs                          // VkImage + memory + view
      VulkanShaderModule.cs                   // SPIR-V shader loading (graphics + compute)
      ValidationLayer.cs                      // debug messenger setup
      MemoryAllocator.cs                      // VkDeviceMemory bumper allocator

    Compute/
      MODULE.md
      VulkanComputePipeline.cs                // VkPipeline (compute bind point)
      VulkanComputeDescriptors.cs             // descriptor set layout + pool + pipeline layout
      ComputeDispatch.cs                      // dispatch command recording + fence sync
      FieldStorageBinding.cs                  // K9 RawTileField → SSBO binding

    Sprite/
      MODULE.md
      Sprite.cs                               // sprite handle struct
      SpriteAtlas.cs                          // atlas region definitions
      SpriteBatcher.cs                        // dynamic VBO, single-batch draw
      Camera2D.cs                             // orthographic projection
      AtlasRegion.cs                          // rect + atlas reference

    Text/
      MODULE.md
      BitmapFont.cs                           // pre-rendered glyph atlas
      Glyph.cs                                // glyph metrics struct
      TextRenderer.cs                         // text → glyph quad batch

    Assets/
      MODULE.md
      PngDecoder.cs                           // manual PNG decoder
      AssetManager.cs                         // path resolution + caching
      AssetPath.cs                            // typed path wrapper

    Diagnostic/
      MODULE.md
      FrameTimer.cs                           // FPS measurement
      DebugOverlay.cs                         // FPS/tick/queue overlay
      ValidationLog.cs                        // captures validation messages

  // ====== Adapter (REWRITE from Godot to Runtime) ======
  DualFrontier.Presentation/
    DualFrontier.Presentation.csproj
    MODULE.md
    Program.cs                                // Main() entry point
    GameRoot.cs                               // bootstrap, replaces Godot scene

    Bridge/
      MODULE.md
      RenderCommandDispatcher.cs              // existing pattern, retargeted

    Visuals/
      MODULE.md
      PawnVisual.cs / ItemVisual.cs / TileMapVisual.cs
      VisualRegistry.cs                       // EntityId → Visual lookup

    UI/
      MODULE.md
      ColonyPanel.cs / PawnDetail.cs
      DebugOverlayAdapter.cs                  // bridges Runtime DebugOverlay

tests/
  DualFrontier.Tests/                         // existing tests (unchanged)
  DualFrontier.Runtime.Tests/                 // new — non-GPU runtime tests
    MODULE.md
    Assets/PngDecoderTests.cs
    Sprite/{Camera2DTests.cs, SpriteBatcherTests.cs}
    Input/InputEventQueueTests.cs

tools/
  shaders/                                    // GLSL source files (graphics + compute)
    sprite.vert / sprite.frag / text.vert / text.frag
    diffusion.comp / wave.comp                // V1 + V2 substrate shaders
  glslangValidator.exe                        // Khronos shader compiler (build-time)
  scaffold-runtime.ps1                        // generator (committed)

assets/
  kenney/                                     // existing PNG atlas (preserved)
    terrain/roguelikeSheet_transparent.png
  shaders/                                    // pre-compiled SPIR-V output
    sprite.vert.spv / sprite.frag.spv / text.vert.spv / text.frag.spv
    diffusion.comp.spv / wave.comp.spv        // V1 + V2 substrate shaders
  fonts/
    default.png / default.fnt                 // glyph metrics file

mods/                                         // unchanged — see MOD_OS_ARCHITECTURE
  Vanilla.{Core,Combat,Magic,Inventory,Pawn,World,Electricity,Water,Movement}/

docs/
  architecture/
    VULKAN_SUBSTRATE.md                       // THIS DOCUMENT (V substrate spec)
    KERNEL_ARCHITECTURE.md / MOD_OS_ARCHITECTURE.md / FIELDS.md
    ARCHITECTURE.md / THREADING.md / historical/VISUAL_ENGINE.md / historical/GODOT_INTEGRATION.md
  methodology/
    METHODOLOGY.md / CODING_STANDARDS.md / ...
  ROADMAP.md
```

The scaffolding in `tools/scaffold-runtime.ps1` materializes the rendering hierarchy mechanically; commit `81fea13`. The `Compute/` submodule under `DualFrontier.Runtime/` is a Q-G-1 cascade addition (V0 compute side); scaffolding generator extends correspondingly when V0 compute primitives ship.

### 2.2 Module purposes

#### `DualFrontier.Runtime` (top-level)

**Purpose:** Generic 2D Vulkan substrate — window management, Vulkan instance/device/queues, rendering primitives, sprite batching, texture loading, input events, UI primitives, compute pipeline plumbing, scalar field compute primitives. Knows nothing of Domain. Could be open-sourced separately.

**Public API surface:** `Runtime.cs` facade exposes:

- Window creation (rendering use case)
- Sprite registration (rendering use case)
- Frame submission (rendering use case)
- Input event polling (rendering use case)
- Compute pipeline registration (compute use case, called by `IModApi.ComputePipelines`)
- Compute dispatch (compute use case, called by `IModApi.Fields.DispatchCompute`)

**Dependencies:** `System` (BCL), `System.Numerics`, `System.IO.Compression`. No third-party. Compare to the strict layering rules of [ARCHITECTURE](./ARCHITECTURE.md).

#### `DualFrontier.Runtime.Native.Win32`

**Purpose:** Pure P/Invoke to Win32 API. `[LibraryImport]` declarations for window management, message pump, input handling.

**Public API surface:** `internal` to Runtime. Win32 surface does not leak beyond Runtime project.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Native.Vulkan`

**Purpose:** Pure P/Invoke to Vulkan API. `[LibraryImport]` declarations + struct definitions + enums per Vulkan 1.3 spec. Covers both graphics and compute Vulkan surfaces (one `vulkan-1.dll` linkage, two pipeline bind points).

**Public API surface:** `internal` to Runtime. Vulkan surface does not leak beyond Runtime project.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Window`

**Purpose:** High-level window abstraction. Hides Win32 details. Provides lifecycle (create/show/destroy), event subscription, input event delivery.

**Public API surface:** `IWindow`, `Window`, `WindowOptions`. Replaces the [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) `IRenderer` initialization path on the new foundation.

**Dependencies:** `Native.Win32`, `Input`.

#### `DualFrontier.Runtime.Input`

**Purpose:** Typed input events + event queue. Events posted by Window, consumed by clients via polling. Supersedes the Godot `IInputSource` adapter ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) §Contracts) once rendering cutover lands.

**Public API surface:** `IInputEvent` + concrete event types + `InputEventQueue`.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Graphics`

**Purpose:** Vulkan rendering primitives — instance, device, swapchain, render pass, pipeline, buffer, image, memory allocator. Direct wrappers around Vulkan API with idiomatic C# lifetimes (`IDisposable` patterns). Shares `VkInstance`/`VkDevice` with `Compute/`.

**Public API surface:** `VulkanInstance`, `VulkanDevice`, etc. — used by `Sprite`, `Text`, `Diagnostic`, and by `Compute/`.

**Dependencies:** `Native.Vulkan`, `Window` (for surface creation).

#### `DualFrontier.Runtime.Compute` (V0 compute side, V1, V2)

**Purpose:** Vulkan compute primitives — compute pipeline, descriptor sets, dispatch, fence sync, K9 field storage binding. Substrate primitives V1 (diffusion) and V2 (wave) shaders managed here. Mod-driven shader registration flows through this module via `IModApi.ComputePipelines`.

**Public API surface:** `VulkanComputePipeline`, `VulkanComputeDescriptors`, `ComputeDispatch`, `FieldStorageBinding`.

**Dependencies:** `Graphics` (shared instance/device), `Native.Vulkan`.

#### `DualFrontier.Runtime.Sprite`

**Purpose:** 2D sprite rendering — atlas-based, batched draw calls. `Camera2D` for orthographic projection.

**Public API surface:** `Sprite`, `SpriteAtlas`, `SpriteBatcher`, `Camera2D`.

**Dependencies:** `Graphics`, `Assets`, `System.Numerics`.

#### `DualFrontier.Runtime.Text`

**Purpose:** Bitmap font text rendering. Reuses sprite pipeline (text = textured quads from glyph atlas).

**Public API surface:** `BitmapFont`, `TextRenderer`.

**Dependencies:** `Sprite`, `Assets`.

#### `DualFrontier.Runtime.Assets`

**Purpose:** Asset loading — manual PNG decoder, asset path resolution.

**Public API surface:** `PngDecoder`, `AssetManager`.

**Dependencies:** `System` (BCL), `System.IO.Compression`.

#### `DualFrontier.Runtime.Diagnostic`

**Purpose:** Performance + debug tooling — FPS measurement, debug overlay, validation log capture. Targets and budgets governed by [PERFORMANCE](./PERFORMANCE.md).

**Public API surface:** `FrameTimer`, `DebugOverlay`, `ValidationLog`.

**Dependencies:** `Sprite`, `Text`, `Graphics`.

#### `DualFrontier.Presentation` (rewritten adapter)

**Purpose:** Bridge from Domain to Runtime. Translates `RenderCommands` (from existing `PresentationBridge`, see [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)) to Runtime API calls. Owns «what to draw» logic.

**Public API surface:** `Program.Main()`, `GameRoot`. Internal classes wire bridge to Runtime.

**Dependencies:** `DualFrontier.Runtime`, `DualFrontier.Application`, `DualFrontier.Events`, `DualFrontier.Components`.

### 2.3 Threading model

The substrate extends [THREADING](./THREADING.md) — domain `ParallelSystemScheduler` and tick rate are unchanged; the substrate contributes a single render thread merged with the OS message pump (per L8). Compute dispatches happen on the simulation thread (managed-side `IModApi.Fields.DispatchCompute` call), executed asynchronously by the GPU; fence-based sync ensures next-tick reads see consistent state without blocking the simulation thread.

#### 2.3.1 Pipeline depth and queue family roles (К-L16/L19/L7.1, К10.3 v2 amendment)

Sim thread coordinates с three Vulkan queues:
- **Graphics queue** — display rendering (existing — preserved verbatim).
- **Async compute queue** (К-L19 V0.B amendment) — К-L16 pipeline depth dispatches per Phase.Compute (К10.3 v2 Item 35 — see `native/include/phase_compute.h`); V1 sync `dispatch_compute_field` also uses this queue.
- **Copy/transfer queue** (К-L19 V0.B amendment) — asset transfers (existing semantics).

Pipeline depth D=2 default (К-L16): sim thread allocates new slot at start of pipeline-managed tick; Phase.Compute dispatches к async compute queue; fence orchestration tracks slot transitions Empty→Dispatched→FenceCompleted→ReadableAsTail. K-L13 wake registry extended с slot transition counter (К10.3 v2 Item 37 — `WakeOnSlotTransitionAttribute` consumer surface, full subscriber registry integration deferred к К-extensions).

Sim-thread reads от slot tail (К-L7.1) для pipeline-managed fields — `df_pipeline_read_slot_tail(slot_offset = -1)` returns sim_tick - 1 results. К-L7 atomic-from-observer preserved within slot boundary; cross-slot reads see different snapshots.

V1's `dispatch_compute_field` sync path (К-L7 baseline): preserved unchanged; consumer call returns after fence signals. Orthogonal к pipeline depth per S-LOCK-13 coexistence.

```
┌──────────────────────────────────────────────────────────────┐
│ Process Threads                                              │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Main Thread (Window + Render — MERGED)                      │
│  ┌────────────────────────────────────────────────────┐      │
│  │ Win32 message pump (PeekMessage / DispatchMessage) │      │
│  │   → input event queue                              │      │
│  │   → window lifecycle (close, resize, focus)        │      │
│  │ Focus events → loop.SetPaused()                    │      │
│  │ Vulkan rendering                                   │      │
│  │   → drain PresentationBridge (existing pattern)    │      │
│  │   → record command buffer                          │      │
│  │   → submit + present                               │      │
│  │ DebugOverlay update                                │      │
│  │ ~60 FPS target                                     │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  Simulation Thread (GameLoop — preserved verbatim)           │
│  ┌────────────────────────────────────────────────────┐      │
│  │ ParallelSystemScheduler.ExecuteTick()              │      │
│  │   30 TPS fixed step (THREADING §Phases)            │      │
│  │   ↓ writes to PresentationBridge.Enqueue()         │      │
│  │   ↑ reads input events from InputEventQueue        │      │
│  │   ↻ dispatches compute (V1/V2) via IModApi.Fields  │      │
│  │     — fence-based async sync, no block             │      │
│  │ SetPaused() coupling from Main thread              │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  GPU (asynchronous to CPU)                                   │
│  ┌────────────────────────────────────────────────────┐      │
│  │ Compute queue executes dispatched shaders          │      │
│  │ Graphics queue executes recorded command buffers   │      │
│  │ Fence signals back to CPU on completion            │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  Worker Threads (existing — used by ParallelSystemScheduler) │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

**Cross-thread synchronization:**

- `PresentationBridge` (existing `ConcurrentQueue<IRenderCommand>`, see [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)) preserved as primary domain → render channel.
- New: `InputEventQueue` (`ConcurrentQueue<IInputEvent>`) for render → domain input events.
- Pause coupling: Main thread detects focus loss, calls `loop.SetPaused(true)`. Domain thread sleeps. Pattern proven from M8.10 (focus notifications) — extended Vulkan way (Win32 `WM_KILLFOCUS`/`WM_SETFOCUS` messages — clean semantics, no `tree.paused` surprises).
- Compute dispatch: simulation thread calls `FieldHandle<T>.DispatchCompute(pipeline, params, iterations)` → native kernel records command buffer, submits to compute queue, returns immediately. Fence-based sync ensures subsequent `ReadCell` sees consistent state; one-tick lag acceptable for continuous field values (see §5.5 async sync hazards).

### 2.4 Dependency rules (locked invariants)

These rules are mechanically verifiable; the build fails if any is violated. They mirror the dependency-direction discipline of [ARCHITECTURE §Four layers](./ARCHITECTURE.md).

**Rule 1.** `DualFrontier.Runtime` MUST compile and tests pass without any reference to Domain projects. Verify via project reference graph.

**Rule 2.** Domain ↔ Runtime communication ONLY through `DualFrontier.Presentation` adapter and through `IModApi` (compute pipelines + field dispatch). Domain knows nothing of Runtime; Runtime knows nothing of Domain.

**Rule 3.** Within Runtime, dependency direction respects layering:

```
Native.Win32 / Native.Vulkan  (lowest)
    ↓
Window / Input / Assets
    ↓
Graphics
    ↓
Compute  (shares Graphics's VkInstance/VkDevice)
    ↓
Sprite / Text
    ↓
Diagnostic
    ↓
Runtime.cs (facade — top)
```

**Rule 4.** No layer skipping (Diagnostic does not import Native.Vulkan directly; goes through Graphics).

**Rule 5.** Runtime exposes minimal public API. Internal implementation details `internal`. Naming follows [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md).

### 2.5 Native interop patterns

**Win32 P/Invoke template:**

```csharp
namespace DualFrontier.Runtime.Native.Win32;

internal static partial class Win32Api
{
    [LibraryImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
    internal static partial ushort RegisterClassEx(in WNDCLASSEX lpwcx);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr CreateWindowEx(
        uint dwExStyle, string lpClassName, string lpWindowName,
        uint dwStyle, int X, int Y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);
}
```

**Vulkan P/Invoke template (Option A: Direct LibraryImport):**

```csharp
namespace DualFrontier.Runtime.Native.Vulkan;

internal static unsafe partial class VkApi
{
    [LibraryImport("vulkan-1.dll", EntryPoint = "vkCreateInstance")]
    internal static partial VkResult vkCreateInstance(
        in VkInstanceCreateInfo pCreateInfo,
        IntPtr pAllocator,
        out IntPtr pInstance);
}
```

**Recommendation:** start with Option A (direct `LibraryImport`) for V0 foundation work. Migrate to canonical procedure-address loading (`vkGetInstanceProcAddr` dispatch) post-foundation if profiling demands. See §4 «Vulkan dispatch».

### 2.6 Asset pipeline

**PNG loading flow:**

```
disk PNG file
  ↓ FileStream.Read
PNG bytes (compressed)
  ↓ PngDecoder.Decode (DualFrontier.Runtime.Assets)
RGBA byte[] + width + height
  ↓ VulkanImage.CreateFromBytes
VkImage + VkImageView + VkDescriptorSet
  ↓ SpriteAtlas.RegisterRegion(rect)
sprite handle (used by Visuals)
```

**Manual PNG decoder scope** (~500–700 lines):

- IHDR, IDAT, IEND chunks parsing
- DEFLATE decompression via `System.IO.Compression.DeflateStream` (BCL)
- Filter unfiltering (Sub/Up/Average/Paeth predictors)
- CRC32 verification
- RGBA8 output (other formats deferred)

### 2.7 Shader strategy

**Build-time SPIR-V compilation** via MSBuild target in `Directory.Build.props`. Same toolchain compiles both graphics shaders (vertex, fragment) and compute shaders (V1 diffusion, V2 wave) — single `glslangValidator.exe` invocation set per project build.

```xml
<Target Name="CompileShaders" BeforeTargets="Build"
        Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\sprite.vert -o $(SolutionDir)assets\shaders\sprite.vert.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\sprite.frag -o $(SolutionDir)assets\shaders\sprite.frag.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\diffusion.comp -o $(SolutionDir)assets\shaders\diffusion.comp.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\wave.comp -o $(SolutionDir)assets\shaders\wave.comp.spv" />
</Target>
```

**Mod-side compute shader compilation** (per K-L9 vanilla = mods): each vanilla mod compiles its compute shader during the mod build process, embeds the resulting `.spv` bytes into mod assets, registers via `IModApi.ComputePipelines.RegisterPipeline(name, spirvBytes)` at mod startup. Same toolchain (extends §2.7 build target into mod projects). See §3.3 mod-driven shader registration.

**Production binary depends on:** `vulkan-1.dll` + pre-compiled `*.spv` files. No shader compiler dependency.

### 2.8 Testing strategy

The existing Domain tests are preserved verbatim — Domain layer is untouched (L10). [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) governs the test pyramid; substrate additions slot in as new unit-test categories without altering the pyramid shape.

**New tests in `DualFrontier.Runtime.Tests`** (non-GPU, JIT-runnable):

- `PngDecoder` tests (~20–30): synthetic PNG inputs → expected RGBA output.
- `SpriteBatcher` logic tests (~10): batch sorting, atlas grouping, vertex math.
- `Camera2D` math tests (~10): orthographic projection, screen ↔ world conversion.
- `InputEventQueue` tests (~5): cross-thread enqueue/dequeue semantics.
- Compute pipeline registration tests (~5): round-trip register/dispatch/release.
- CPU reference vs GPU equivalence tests (V1 diffusion, V2 wave) — synthetic grids, tolerance-bounded comparison.

**GPU-dependent tests:** F5 manual visual verification per established M8.8/M8.9 protocol. Validation layer output captured to console — clean output is success criterion. The protocol mirrors the [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) red-flag checklist for visual changes.

### 2.9 Naming conventions

Continued from [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md):

- All identifiers English (Russian glossary unchanged — see [TRANSLATION_GLOSSARY](./TRANSLATION_GLOSSARY.md)).
- Vulkan struct types: keep canonical `VkInstanceCreateInfo` naming (matches Vulkan spec).
- Win32 struct types: keep canonical `WNDCLASSEX` naming (matches Win32 docs).
- Pascal case for C# wrapper classes: `VulkanInstance`, `Win32Window`, `VulkanComputePipeline`.
- Internal P/Invoke artifacts: `internal` access modifier.
- Public Runtime API: `public` access modifier, idiomatic C# names.

---

## 3. Compute use case (substrate primitives V0/V1/V2 implementation)

### 3.1 Two compute domains

V substrate compute use case serves two architecturally distinct workload categories. Both are LOCKED as supported domains.

#### Domain A — Field updates (primary, foundational; V1 + V2 substrate primitives)

Spatial scalar/vector fields stored as dense 2D grids. Each field cell updates per tick (or sub-tick) via diffusion (V1), wave propagation (V2), decay, or similar local-stencil operations. Embarrassingly parallel; one compute shader invocation per cell.

Examples (gameplay configurations of V1/V2 primitives): mana density, electricity field, water pressure, temperature distribution, sound pressure, scent concentration, pollution, radiation, future modder-defined fields.

Storage: `RawTileField<T>` in native kernel (introduced K9; see [FIELDS](./FIELDS.md)). Per-field conductivity map enables anisotropic diffusion (wires, pipes). Storage flag handling moved to gameplay-level node config (former G3 «storage cells / capacitance» feature absorbed into gameplay configurations of V1; not substrate-primitive concern — Q-G-2 reduction).

Compute pattern: 4-neighbor stencil (V1 diffusion), wave propagation respecting topology overlay (V2 wave), optional anisotropy weighting, optional cliff-threshold consumer effectiveness (managed-side). Ping-pong between two image/buffer resources, 5–10 iterations per dispatch to reach near-equilibrium.

This is the primary motivator for elevating V substrate compute to foundational status. Field-based mechanics are not a single optimization target; they are a generative architectural pattern that absorbs new gameplay mechanics through additive registration.

#### Domain B — Entity-keyed bulk computation (secondary, opportunistic; deferred substrate disposition)

Per-entity calculations that scale poorly on CPU at high entity counts. Original `ProjectileSystem` case falls here. Workloads suitable when:

- Per-entity work is uniform (no branchy decision logic per entity)
- Entity count is high (typically 500+ for projectiles, similar magnitude for other domains)
- Output can tolerate one-tick lag (asynchronous readback)

Examples: projectile position/collision, parallel pathfinding flow fields (now folded into V2 wave shader side product per Q-G-2), large-scale particle effects, bulk physics simulation, AI cohort updates.

Storage: existing `RawComponentStore` (sparse-set, dense byte buffers). Native kernel exposes component spans directly as SSBO content; no marshalling.

Compute pattern: one work group per entity batch, read component spans, write results to output buffer, fence-based sync to managed read.

**Substrate disposition deferred (Q-G-2 §3.2):** Domain B is a separate compute domain from V1/V2 field updates. Whether it stays in V substrate (as additional primitive V3+), becomes own substrate, or stays consumer-level is deferred to amendment authoring at the time the first M-V5 projectile reactivation work begins.

### 3.2 Why V substrate compute (vs. v1.0 deferral rationale)

#### Prior `GPU_COMPUTE.md` v1.0 deferral rationale — superseded

Phase 3 reasoning: CPU→GPU→CPU roundtrip costs 0.5–2 ms per frame in a managed runtime, so dispatch overhead exceeds saved CPU work below ~500 projectiles. Threshold pinned experimentally to "Battle of the Gods" stress test.

This reasoning was correct for the pre-pivot architecture (managed runtime, no native kernel, no Vulkan rendering layer). It does not apply to the post-pivot architecture:

- Dispatch path is native → `vulkan-1.dll`, no managed crossings
- Component data already in SoA layout, no marshalling
- Vulkan instance/device already live for rendering, no setup cost amortization concern
- Compute and graphics share the same `VkDevice` and `VkQueue` family (or async compute queue)

The 0.5–2 ms estimate from v1.0 reflected managed runtime overhead, not the actual dispatch cost on bare Vulkan. Native dispatch overhead is microseconds, not milliseconds.

#### Current rationale — field mechanics force the issue

Field-based gameplay mechanics (mana, electricity, water, etc.) are not optional optimizations for some future scale. They are first-class gameplay systems in Dual Frontier's design. Their natural implementation is dense 2D grid storage with cellular-automaton-style updates, which fits GPU compute architecturally.

The choice is not "CPU now, GPU later when scale demands." The choice is "field math on CPU forever (with associated scaling cap) vs. field math on GPU (free at any scale)." Given hardware baseline (Steam Hardware Survey median GPU class is RTX 3060/4060 territory; even budget GPUs handle our workload trivially) and architectural fit (Vulkan compute already in scope), the GPU path is structurally preferable.

Domain B (entity-keyed bulk compute) retains a deferral story: still threshold-driven, still benchmarked against CPU baseline, still optional. Domain A (V1/V2 fields) does not.

### 3.3 Mod-driven shader registration

Per K-L9 (vanilla = mods), compute shaders are owned by mods, not the engine. `Vanilla.Magic` ships `ManaField` + diffusion shader (V1 configuration). `Vanilla.Electricity` ships `PowerField` + anisotropic diffusion shader (V1 configuration). `Vanilla.Movement` ships flow field configurations on V2 (routed paths to destinations). Third-party mods extend through the same registration API.

Build pipeline (§2.7) already compiles GLSL to SPIR-V via `glslangValidator.exe` for graphics shaders. Compute shaders use the same toolchain. Mod build process embeds compiled SPIR-V into mod assets.

Mod startup code (illustrative — exact API per `IModApi` v3 surface in [MOD_OS_ARCHITECTURE §4.6](./MOD_OS_ARCHITECTURE.md)):

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

### 3.4 Native compute dispatch (V0 substrate primitive)

V0 introduces Vulkan compute integration. The native kernel gains a new linkage to `vulkan-1.dll` for compute pipeline operations (shared with the rendering linkage — one `vulkan-1.dll`, one `VkInstance`, one `VkDevice`).

#### 3.4.0 К-L19 async compute queue mandate (К10.3 v2 documentation cleanup от V0.B landing)

Native compute dispatch к dedicated async compute queue family mandated per К-L19 (V0.B implementation backing): `df_world_attach_vulkan` populates `VulkanAttachment.async_compute_queue` от `VulkanDevice.AsyncComputeQueueFamilyIndex`. V1's `dispatch_compute_field` и К10.3 v2's Phase.Compute (Item 35) both submit к this queue. Graphics queue не used для compute. К-L19 hardware tier exclusion accepted as architectural choice per `KERNEL_ARCHITECTURE.md` К-L19 row (Vulkan 1.3 + async compute queue family — NVIDIA Turing+/AMD RDNA 1+/Intel Arc Alchemist+).

К10.3 v2 К-L16 pipeline depth (D=2 default) operates атоп of К-L19's async compute queue infrastructure — pipeline-managed dispatches batched per tick into single VkQueueSubmit (Phase.Compute scheduler integration; Prediction 12 ~50-100μs savings at ~10 active dispatch systems). S-LOCK-13 coexistence: V1's `dispatch_compute_field` synchronous path orthogonal к pipeline-managed Phase.Compute path.

C ABI extension on `df_capi.h` (per [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9 + V0 integration):

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

### 3.4.1 `df_vulkan_unload_mod_resources` C ABI primitive (К-L18, К10.3 v2 placeholder per S-LOCK-12 spec scope)

К10.3 v2 lands the C ABI signature + managed wrapper placeholder for Step 3.6 of the mod unload chain (MOD_OS_ARCHITECTURE §9.5 К10.3 v2 amendment); native implementation lands as V-cycle work либо К-extensions per managed-facade-preserved strategy. К-L18 quiescent state precondition is already satisfied before this primitive is invoked (Step 3.5 К10.2 native primitive verified sim paused + pipeline quiescent per К-L18 invariant).

```c
typedef struct {
    int32_t success;
    int32_t pipelines_destroyed;
    int32_t descriptor_sets_destroyed;
    int32_t buffers_destroyed;
    int32_t images_destroyed;
    char    error_messages[8][256];
    int32_t error_count;
} VulkanModUnloadResult;

DF_API int32_t df_vulkan_unload_mod_resources(
    const char*             mod_id,
    VulkanModUnloadResult*  out_result);
```

К10.3 v2 placeholder behavior: returns `success = 1` + zero counts (no pipeline-managed mod resources yet registered). Full implementation: `VkDestroyPipeline` / `VkFreeDescriptorSets` / `vkDestroyBuffer` / `vkDestroyImage` operations for mod-registered resources, paralleling the existing per-mod tracking conventions established for compute pipeline registration (§3.4) и field storage. Best-effort sequential per MOD_OS §9.5.1.

Managed wrapper lives at `src/DualFrontier.Application/Bridge/VResourceCleanup.cs` (К10.3 v2 Item 42). К10.3 v2 cascade lands the managed surface; native side wires up when consumer code begins registering Vulkan handles per mod (after V-cycle / К-extensions surface lands).

---

## 4. Rendering use case (V0 rendering side)

The rendering use case rebuilds presentation functionality from Godot 4 onto the V substrate. Existing Godot path runs in parallel until cutover; substrate rendering side reaches full M8.x parity before Godot deletion.

### 4.0 Display composition (К-L17, К10.3 v2 amendment)

V substrate rendering use case is consumed by display composition framework per К-L17 (lives в `src/DualFrontier.Application/Display/`, not в V substrate). Three-layer composition с independent latency contracts:

1. **SimStateLayer** — V substrate render path (existing V0.C.2 batched sprite + Camera2D — preserved verbatim, wrapped as default layer slot). Reads от pipeline slot tail (К-L16) for pipeline-managed display state; reads current state for К-L7 sync default (V1 path). Latency `D × tick_period` or sub-tick.
2. **IntentOverlayLayer** (К10.3 v2 Item 39) — current input state surface. Reads from InputEventQueue at display tick time. Latency ≤16ms (60 FPS) per К-L17 contract.
3. **CombatFeedbackLayer** (К10.3 v2 Item 40) — К-L15 Fast tier event consumers. Subscribes к Fast tier events; renders damage numbers, hit sparks, weapon glints. Latency ≤1ms К-L15 + ≤16ms display ≈ ≤17ms event-к-visible per Prediction 15.

Composition order (К-L17 mandate): SimState layers rendered first, intent + combat overlays composited on top, static layers (loaded assets) last.

Mod-registered layers use `[Layer(LayerType.Intent | CombatFeedback)]` attribute (`DualFrontier.Contracts.Display.LayerAttribute`) + capability declaration:

- `kernel.layer.intent:{FQN}` — sub-pipeline-latency input overlay.
- `kernel.layer.combat_feedback:{FQN}` — К-L15 Fast tier consumer.

Per К-L9 «Vanilla = mods», vanilla layers (built-in intent cursor, combat hit feedback) register through same attribute + capability pattern as third-party mods.

V substrate exposes rendering primitives (`SpriteRenderer`, `Camera2D`, `TileMap`); layer composition lives one architectural layer above per S-LOCK-11. Existing `IRenderer`/`IDevKitRenderer` interfaces в `DualFrontier.Application.Rendering` preserved unchanged — composition framework operates above them, не extending.

### 4.1 Migration approach

**Strategy: parallel development.**

Keep `DualFrontier.Presentation` (Godot) functional through the rendering cutover phase (formerly M9.5). New Runtime project develops in parallel. Cutover phase migrates Presentation to Runtime. Final Godot deletion phase deletes remnants.

**Operating principle:** «honest state always available». No blind period where game does not run. The same discipline as the parallel mod-system migration in [MOD_OS_ARCHITECTURE §11](./MOD_OS_ARCHITECTURE.md) — every phase ends with a runnable build.

### 4.2 Rendering use case implementation phases

The rendering side is implemented as a sequence of sub-phases within V substrate work. Pre-Q-G-1 these were labeled M9.0..M9.8 runtime milestones; post-Q-G-1 they are V substrate rendering use case implementation phases (V0 rendering side). Phase IDs preserved historically in commits and `MIGRATION_PROGRESS.md` closure entries; reference convention in new artifacts uses descriptive names rather than numerical M9.x labels.

Estimated cumulative: ~37–52 hours = **4–7 weeks at hobby pace** (~1h/day).

| Phase | Title                                                | Estimated time | LOC delta            |
| ----- | ---------------------------------------------------- | -------------- | -------------------- |
| R.0   | Foundation: Win32 window + Vulkan clear color        | 4–5h           | +800–1000            |
| R.1   | First textured quad: PNG → VkImage → sprite render   | 5–7h           | +1000–1500           |
| R.2   | Batched sprite renderer                              | 4–6h           | +500–700             |
| R.3   | TileMap parity + Camera2D                            | 3–4h           | +400–600             |
| R.4   | Input system (Win32 → InputEventQueue)               | 3–4h           | +400–500             |
| R.5   | Domain integration (Presentation port)               | 6–8h           | +800–1200            |
| R.6   | UI primitives (text + panels)                        | 8–12h          | +1000–1500           |
| R.7   | Coupled lifecycle + DebugOverlay                     | 2–3h           | +200–300             |
| R.8   | Migration cutover (delete Godot)                     | 2–3h           | -2000+ (deletion)    |

These R.0..R.8 phases are V0 rendering side implementation sequence; they do not introduce additional V-numbered substrate primitives (V0/V1/V2 numbering covers substrate primitives, not implementation sub-phases). The R.x labels are descriptive identifiers within V0 work — convenient for commit messages and progress tracking — not formal namespace bucket assignments.

#### R.0 — Foundation: Win32 window + Vulkan clear color

**Goal:** empty window opens, Vulkan initializes correctly, clear color renders every frame, no validation errors.

**Deliverables:**

- New project `src/DualFrontier.Runtime/`
- New project `tests/DualFrontier.Runtime.Tests/`
- `Native/Win32/` — minimal P/Invoke set (~14 functions)
- `Native/Vulkan/` — minimal P/Invoke set (~30 functions)
- `Window/Window.cs` — wraps `WNDCLASSEX` + `CreateWindowEx` + message pump
- `Graphics/{VulkanInstance,VulkanDevice,VulkanSwapchain,VulkanCommandPool}.cs`
- `Graphics/ValidationLayer.cs` — `VK_LAYER_KHRONOS_validation` enabled in DEBUG
- `Runtime.cs` — top-level facade
- Standalone test executable

**Success criteria:** Window opens; sizable, closeable. Clear color rendered every frame. FPS measurable. Validation layers report zero errors in DEBUG mode. Clean shutdown (no leaked Vulkan handles per validation).

**Time:** 4–5 hours. **LOC:** ~800–1000.

#### R.1 — First textured quad

**Goal:** Kenney pawn sprite rendered at center of window.

**Deliverables:** `PngDecoder` full implementation + `VulkanBuffer`/`VulkanImage` abstractions + Pipeline + RenderPass + sprite shaders compiled to SPIR-V + single sprite render.

**Success criteria:** Kenney pawn sprite displayed centered. PNG decoder tests passing.

**Time:** 5–7 hours. **LOC:** ~1000–1500.

#### R.2 — Batched sprite renderer

**Goal:** 10 000 sprites rendered at 60+ FPS via single draw call.

**Deliverables:** Dynamic vertex buffer + per-sprite vertex data + atlas-shared batching + stress test.

**Success criteria:** 10 000 sprites at 60+ FPS. Single draw call in RenderDoc. [PERFORMANCE](./PERFORMANCE.md) budget for sprite pass adopted.

**Time:** 4–6 hours. **LOC:** ~500–700.

#### R.3 — TileMap parity + Camera2D

**Goal:** 200 × 200 tile grid rendered, camera pannable, full M8.8 visual parity.

**Deliverables:** `TileMapBatch` + `Camera2D` + atlas regions for terrain.

**Success criteria:** 200 × 200 tile map visible. 60+ FPS sustained (was 17 on Godot).

**Time:** 3–4 hours. **LOC:** ~400–600.

#### R.4 — Input system

**Goal:** keyboard and mouse events from Win32 delivered to domain.

**Deliverables:** `InputEventQueue` + event types + Win32 message handler dispatching. Replaces the Godot `InputRouter` ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md)) for new code path; Godot path stays alive until cutover phase R.8.

**Success criteria:** smooth camera pan, key bindings work.

**Time:** 3–4 hours. **LOC:** ~400–500.

#### R.5 — Domain integration (Presentation port)

**Goal:** full M8.9 visual parity on Vulkan stack.

**Deliverables:** Rewrite Presentation layer to target Runtime API. `PawnVisual` / `ItemVisual` / `TileMapVisual` on sprite handles. `RenderCommandDispatcher` retargeted (existing pattern from [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md)).

**Success criteria:** 50 pawns + 255 items + terrain. 60+ FPS. Domain tests passing ([TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) gate).

**Time:** 6–8 hours. **LOC:** ~800–1200 (rewrite).

#### R.6 — UI primitives

**Goal:** full HUD parity — `ColonyPanel` + `PawnDetail` + `DebugOverlay` on Runtime UI.

**Deliverables:** `BitmapFont` + `TextRenderer` + Panel/Label/ProgressBar primitives + `ColonyPanel` + `PawnDetail` port.

**Success criteria:** HUD matches M8.9 visual.

**Time:** 8–12 hours. **LOC:** ~1000–1500.

#### R.7 — Coupled lifecycle + DebugOverlay

**Goal:** parity with M8.10 — focus events couple to loop pause, no desync drift possible.

**Deliverables:** `WM_KILLFOCUS`/`WM_SETFOCUS` hooks + `WM_SIZE` swapchain recreation + `DebugOverlay`.

**Success criteria:** alt-tab pauses simulation. Window resize works. No 298-tick desync.

**Time:** 2–3 hours. **LOC:** ~200–300.

#### R.8 — Migration cutover

**Goal:** Godot completely removed. Only Vulkan stack running.

**Deliverables:** Delete `.godot/`, `project.godot`, `*.import`, Godot-specific gitignore entries. Update `tools/build-all.ps1`. Update `README.md`.

**Success criteria:** `dotnet build` clean without Godot. `grep -r godot` returns empty. `dotnet run` launches game. [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md) marked deprecated; superseded by §2.2 of this document.

**Time:** 2–3 hours. **LOC:** -2000+ (net deletion).

---

## 5. Compute use case (V1 + V2 detail)

### 5.1 Mathematical models (Domain A primary kernels)

**Isotropic diffusion** (V1 baseline — mana, basic spread):

```
∂P/∂t = D · ∇²P + S(x,y) - K · P
```

Compute shader (~30 LOC GLSL): 4-neighbor stencil, single D coefficient, source map, decay coefficient.

**Anisotropic diffusion** (V1 variant — electricity, water — wires/pipes channel flow):

```
∂P/∂t = ∇·(D(x,y) · ∇P) + S(x,y) - C(x,y) · effectiveness(P)
```

Per-cell D varies. Wire/pipe tiles have D ≈ 10.0; off-path tiles have D ≈ 0.1; insulators have D = 0.0. The asymmetric flow `min(D_self, D_neighbor)` between each tile pair guarantees flow blocked when either tile is non-conductor. Wire path channels propagation automatically; "narrow wave" is emergent, not coded.

**Capacitance / storage cells** — gameplay-level node config, NOT substrate-primitive concern (Q-G-2 reduction of former G3):

Storage cells (batteries, tanks, thermal mass) are gameplay-level dynamic spikes on the topology, not a shader feature. Per K-L9 (vanilla = mods), the mod owns the storage configuration: it modulates `RawTileField<T>` cells directly between dispatches via `FieldHandle<T>.SetCell` or via separate "storage layer" managed-side logic that tops up specific cells based on `[t-1]` field values. The shader stays simple.

Conceptual semantic (managed-side, not shader):

```
storage[t+1] = α · storage[t] + (1-α) · field_local[t]
field_emit_when_demanded = β · storage[t]
```

Storage tile retains state across ticks while neighbors evolve. RC time constant analogy. α near 0.95–0.99 for slow decay, β controls release rate during droop.

**Cliff threshold consumer effectiveness** (electricity 60% rule — managed-side, NOT shader):

```
effectiveness(local_P, demand) =
    1.0,                          if local_P ≥ demand
    local_P / demand,             if 0.6·demand ≤ local_P < demand
    0.0,                          if local_P < 0.6·demand
```

Computed in managed code at consumer system after field update. Below-threshold consumers pull 0 from field, freeing capacity for others; system self-stabilizes.

### 5.2 Unified across gameplay configurations (V1 + V2 variants)

| Field       | Sources              | Sinks                    | Conductivity (V1 anisotropy)  | Storage (gameplay-config) | V1 / V2 | Notes                       |
|-------------|----------------------|--------------------------|-------------------------------|---------------------------|---------|-----------------------------|
| Mana        | Springs, ley lines   | Spell casts              | Uniform (no conduits)         | Magic accumulators        | V1      | Slow decay                  |
| Electricity | Generators, solar    | Consumers (pits)         | Wires (high D)                | Batteries                 | V1      | Fast spread, cliff threshold|
| Water       | Pumps, wells         | Drains, irrigation       | Pipes (high D)                | Tanks                     | V1 + V2 | Wave via pipe topology (V2); diffuses ambient on break (V1) — hybrid coupling TBD |
| Heat        | Furnaces, sun        | Cold tiles, refrigerators| Air (medium), insulation (low)| Thermal mass walls        | V1      | Slow propagation            |
| Sound       | Combat, machinery    | Decay-dominated          | Air, walls                    | None typically            | V1      | Fast, decay over distance   |
| Scent       | Food, blood, entities| Time-dominated decay     | Air, terrain                  | None typically            | V1      | Trail formation             |
| Routed nav  | Target tile (sink)   | (gradient)               | Walkable terrain (D)          | n/a                       | V2      | Distance + direction as side product (former G6) |

One V1 compute shader pattern (~50–80 LOC GLSL) handles isotropic + anisotropic variants with parameter variation; one V2 wave shader pattern handles routed propagation + distance/direction extraction. Modder-defined fields extend the same template.

### 5.3 Flow field pathfinding (V2 routed primitive — former G6 folded)

Pathfinding is mathematically isomorphic to wave propagation: a target spike, an anisotropic propagation respecting walkable terrain, an agent reading the gradient. Same V2 wave shader template, same field infrastructure, different gameplay interpretation. Adds pathfinding capability to Dual Frontier without expanding the architectural surface — a structural pattern unification between pathfinding and supply networks. Former G6 flow field infrastructure folded into V2 wave shader side products per Q-G-2 LOCK.

**Per-agent A* (current DF approach via `PathfindingService` / `AStarPathfinding`):**

```
For each pawn:
  Run A* from pawn position to target
  Cache path
  Follow path step-by-step
Cost: O(N × M log M) where N = pawns, M = grid size
```

**Global flow field (V2 routed):**

```
Per target (each unique destination):
  Compute distance field once (one V2 wave dispatch)
  Compute direction field once (gradient of distance — side product of same dispatch)

Per pawn (every tick):
  Read direction at pawn's position (point query)
  Move in that direction
Cost: O(K · M) + O(N) where K = unique targets, N = pawns
```

Scaling:

- 50 pawns going to 5 work zones → A*: 50 searches per tick worst case. Flow field: 5 fields shared, 50 cheap reads.
- 200 pawns going to 5 work zones → A*: 200 searches (linear pain). Flow field: still 5 fields, 200 cheap reads.
- Cost decouples from pawn count.

#### Mathematical isomorphism with electricity

Identical pattern, different interpretation:

| Aspect                | Electricity (V1 anisotropic)        | Flow field (V2 routed)                                   |
|-----------------------|--------------------------------------|---------------------------------------------------------|
| Spike source          | Generator tile (+P)                  | Target tile (max value)                                 |
| Field equation        | Anisotropic diffusion + decay        | Eikonal equation OR simple diffusion                    |
| Conductivity map      | Wires (high D), insulators (low D)   | Walkable terrain (high D), obstacles (low D, walls = 0) |
| What field represents | Power available                      | Distance to target (after gradient: direction to target)|
| Agent behavior        | Read field, compute effectiveness    | Read gradient, move down                                |
| GPU compute pattern   | V1 shader template                   | V2 shader template (similar structure)                  |

Same shader templates with different parameters. This is the architectural compound effect — new pathfinding capability through existing infrastructure.

### 5.4 Engine vs mod placement

**V substrate provides infrastructure:**

- Field types as `RawTileField<T>` (K9) bound to V0 compute pipeline
- V1 diffusion shader template (isotropic + anisotropic via per-cell D)
- V2 wave shader template (routed + distance/direction extraction)
- Compute dispatch + fence sync + read-cell point queries

**Mods provide gameplay:**

- `Vanilla.Movement` defines pawns following V2 flow fields (M-V7)
- `Vanilla.Movement` registers flow field types per target category
- `Vanilla.Movement` decides when to recompute fields (target changes, terrain changes, periodic refresh)
- `Vanilla.Magic` defines mana field (M-V1 — V1 isotropic configuration)
- `Vanilla.Electricity` defines power field (M-V2 — V1 anisotropic configuration)
- `Vanilla.Water` defines water pressure field (V1 + V2 hybrid — wave via pipes, diffuses on break)

This preserves "vanilla = mods" (K-L9). Substrate doesn't know about pathfinding semantics; it just provides V0 plumbing + V1/V2 shader primitives + field abstraction.

Third-party mods can:

- Replace `Vanilla.Movement` with an alternative pathfinding strategy
- Add specialty fields (e.g. enemy avoidance field, pheromone trails) as V1 or V2 configurations
- Compose multiple fields (combine work-zone + danger-avoidance)

**Field lifecycle.** Flow fields are per-target ephemeral resources:

- Created when first pawn requests path to destination
- Updated when terrain changes affect distance field
- Destroyed when no pawns using them (LRU eviction)
- Pool size capped (e.g. max 32 active flow fields)

Lifecycle managed by `Vanilla.Movement`, not V substrate. Same pattern as other mod-managed resources.

### 5.5 Navigation 3-mode hybrid dispatcher (Q-G-2 LOCK)

Per Q-G-2 deliberation §3.2, pawn movement is **three-mode hybrid** — not a single algorithm. Mode selection is gameplay-tuning concern (mod-level, not substrate); substrate provides the primitives all three modes consume.

#### Mode A — Autonomous baseline (GPU persistent direction fields)

For recurring destination types (work zone X, dining hall, escape route): GPU persistent direction fields on V2. All autonomous pawns read these fields. No per-pawn pathfinding. Scaling **O(destination types)**, pawn-count-independent. **This is the hot loop.**

```
Per autonomous pawn (every tick):
  destination_type = pawn.intent.destinationType
  field = active_flow_fields[destination_type]
  direction = field.ReadCell(pawn.position)
  pawn.velocity = direction * speed
```

Cost: per-pawn = single field read. 200 autonomous pawns × 1 read = trivial.

#### Mode B — Small player command (≤10 pawns, CPU per-pawn pathfinding)

For one-off player commands to specific destinations: CPU per-pawn A*. Threshold N ≈ 10 (gameplay tuning, not substrate spec). For 1–10 pawns following a unique command, per-pawn A* is cheaper than spinning up a temporary V2 dispatch.

```
Per commanded pawn:
  Run A* from pawn.position to specific target
  Cache path, follow step-by-step
```

#### Mode C — Mass event (10+ pawns to one-off destination, GPU wave dispatch)

For mass events (raid alert, fire evacuation, mass migration to specific tile): one V2 wave dispatch creates a temporary destination field; all pawns read it.

```
On mass event trigger:
  Spawn temporary flow field at event target
  All affected pawns read direction field
  Field destroyed after event resolution
```

Cost: O(1 dispatch) + O(N reads) where N = affected pawn count. Vastly cheaper than O(N) A* searches.

#### Mode selection logic (gameplay-level, mod-owned)

```csharp
// Vanilla.Movement (illustrative)
foreach (var pawn in pawns)
{
    if (pawn.Intent.IsAutonomous && IsRecurringDestination(pawn.Intent.Target))
    {
        // Mode A — GPU persistent field
        pawn.Velocity = ReadActiveField(pawn.Intent.Target).At(pawn.Position) * pawn.Speed;
    }
    else if (commandedPawns.Count <= 10)
    {
        // Mode B — CPU A*
        pawn.Velocity = AStarPath(pawn.Position, pawn.Intent.Target).Next() * pawn.Speed;
    }
    else
    {
        // Mode C — GPU mass dispatch
        var massField = SpawnTemporaryFlowField(pawn.Intent.Target);
        pawn.Velocity = massField.At(pawn.Position) * pawn.Speed;
    }
}
```

**Local avoidance** is a separate concern (mod-level, NOT substrate primitive). Local steering on top of flow field direction — RVO-like or simple boids approach, combines flow field global direction + local agent collision avoidance. Pure managed CPU code (per-pawn, but simple math, parallelizable). M-V8 demonstration.

#### Mode C visibility latency (К-L17, К10.3 v2 amendment)

Mode C navigation visibility latency is governed по К-L17 composition framework (§4.0). Player commands → IntentOverlayLayer (≤16ms render latency, sub-pipeline-latency input surface); pawn responses → SimStateLayer (pipeline-managed К-L16 D=2 lag для async dispatches, либо К-L7 sync для V1 path); combat feedback на encounter → CombatFeedbackLayer (К-L15 Fast tier ≤1ms + display ≤16ms ≈ ≤17ms event-к-visible per Prediction 15).

No special-case visibility mechanism — К-L17 composition framework handles latency separation uniformly across навигation modes.

### 5.6 Domain B kernel (deferred)

Original `ProjectileSystem` GPU implementation pattern preserved as deferred consideration:

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

Dispatched once per tick. One-tick lag for visual representation (asynchronous readback). Threshold for adopting this path remains experimental — projectile count where CPU degrades — but the integration cost is now negligible compared to v1.0 prior framing, so the threshold may shift downward in practice. **Substrate disposition deferred (Q-G-2 §3.2)**: whether Domain B becomes V3 primitive, separate substrate, or stays consumer-level decided at M-V5 reactivation amendment time.

---

## 6. Roadmap

### 6.1 V substrate primitive sequencing

| Primitive | Title                                                                | Estimated time | Status |
| --------- | -------------------------------------------------------------------- | -------------- | ------ |
| V0        | Vulkan substrate foundation (rendering + compute plumbing)           | 4–6 weeks      | Pending |
| V1        | Scalar field + diffusion shader (isotropic + anisotropic)            | 1–2 weeks      | Pending (gates on V0) |
| V2        | Scalar field + wave shader (routed, breakable, distance/direction)   | 2–3 weeks      | Pending (gates on V0) |
| V close   | Multi-field coexistence acceptance criterion (former G4)             | included       | (V substrate close gate) |

Future V-N primitives reserved for post-substrate compute needs (G5 Domain B disposition, G9 eikonal upgrade if evidence justifies, modder-driven primitives).

### 6.2 Rendering use case implementation phases (R.0..R.8)

Listed in §4.2 above. Sub-phases of V0 work; do not introduce additional V-numbered primitives.

### 6.3 M-V demonstrations (per Q-R-1 format)

Per Q-R-1 LOCK and Q-V-2 LOCK, mods that demonstrate V substrate primitives carry `M-V{original G number}` identifiers preserving traceability to G-skeleton briefs. Mods with multi-substrate dependencies (K-side + V-side) carry compound marker `M-K{N} / M-V` with V-side identifier deferred to V-side authoring time per FHE-style reserved pattern.

| Demo identifier | Title                                                  | Substrate primitive | Status |
|-----------------|---------------------------------------------------------|---------------------|--------|
| M-V1            | Vanilla.Magic mana diffusion (V1 isotropic)             | V1                  | Pending (gates on V1) |
| M-V2            | Vanilla.Electricity power field (V1 anisotropic)        | V1                  | Pending (gates on V1) |
| ~~M-V3~~        | (gap — former G3 storage cells, reduced to gameplay)    | n/a                 | Reduced |
| ~~M-V4~~        | (gap — former G4 multi-field, now V close criterion)    | n/a                 | Reduced |
| M-V5            | Vanilla.Combat projectile Domain B reactivation         | TBD (deferred)      | Deferred — substrate disposition TBD |
| ~~M-V6~~        | (gap — former G6 flow field infrastructure, folded V2)  | n/a                 | Folded |
| M-V7            | Vanilla.Movement integration (V2 routed flow field)     | V2                  | Pending (gates on V2) |
| M-V8            | Vanilla.Movement local avoidance extension              | mod-level (not substrate) | Pending (gates on M-V7) |
| ~~M-V9~~        | (gap — former G9 eikonal, deferred as V2 tunable TBD)   | V2 tunable or V3    | Deferred — evidence-gated |

Gaps M-V3, M-V4, M-V6, M-V9 reflect Q-G-2 reductions; identifier slots reserved but unused.

### 6.4 Multi-substrate vanilla mods (Q-V-2 LOCK)

Mods that span both K substrate and V substrate carry compound markers per Q-V-2 LOCK:

| Mod                     | K-side identifier (bucket M-K) | V-side identifier (reserved) | Substrate primitive consumed |
|-------------------------|--------------------------------|------------------------------|------------------------------|
| Vanilla.Magic           | M-K{N} (deferred to author)    | M-V1                         | V1 isotropic diffusion        |
| Vanilla.Electricity     | M-K{N} (deferred)              | M-V2                         | V1 anisotropic diffusion      |
| Vanilla.Water           | M-K{N} (deferred)              | M-V (deferred — V1+V2 hybrid) | V1 + V2 hybrid                |
| Vanilla.Movement        | M-K{N} (deferred)              | M-V7 + M-V8                  | V2 routed flow field           |

K-side milestone authored first; V-side authored after V substrate ready. V-side concrete identifier within the M-V bucket appears at V-side authoring time per FHE-style reserved pattern (Q-V-2 precedent, IHomomorphicComputeProvider model).

K-only mods (single milestone) — no compound marker:

- Vanilla.World (M-K bucket, identifier deferred)
- Vanilla.Pawn (M-K bucket, 3 sub-milestones, identifiers deferred)
- Vanilla.Inventory (M-K bucket, deferred)
- Vanilla.Core (M-K bucket, deferred)

### 6.5 Hybrid coupling spec (deferred TBD)

How V1 diffusion picks up from a broken V2 wave node — example: water in pipes propagates via V2 wave respecting pipe topology; on pipe break, water diffuses ambient via V1. Coupling spec deferred to V substrate amendment authoring; deliberation §3.2 surfaced this as deferred-to-amendment item. M-V Water demonstration is the first integration point requiring resolution.

### 6.6 Combined timeline

V0 substrate foundation (rendering side + compute side) ≈ 4–6 weeks. V1 + V2 ≈ 3–5 weeks. M-V demonstrations (M-V1, M-V2, M-V7, M-V8) ≈ 3–5 weeks. Combined V substrate work ≈ 10–16 weeks. Combined with kernel work (K-series → A' bridge → M-cycle Phase B) per [MIGRATION_PLAN_KERNEL_TO_VANILLA](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md), the full architectural foundation is ~20–30 weeks at hobby pace.

---

## 7. Failure modes and fallbacks

### 7.1 CPU fallback for compute shaders

Not all hardware supports Vulkan 1.3 compute reliably. Some integrated GPUs and older laptops have driver issues. Pure software environments (CI, headless build agents) may lack GPU access entirely.

Each compute shader has a CPU reference implementation in managed code (originally written for shader equivalence testing during V1+). At startup, native kernel detects compute capability:

- Vulkan 1.3 + compute queue available: use GPU dispatch path
- Compute unavailable or disabled by config: managed scheduler invokes CPU reference implementation per tick

Performance on CPU fallback is significantly worse (orders of magnitude for large grids), but functionality is preserved. Game still runs; users see degraded performance rather than crashes.

CPU fallback is also mandatory for deterministic save snapshots (see below) if GPU determinism cannot be guaranteed across hardware.

### 7.2 Determinism considerations

#### 7.2.0 Pipeline drain semantics (К-L16, К10.3 v2 amendment)

Save protocol per S8-Q1.5: snapshot display tick state (CurrentSimTick - D). Display already sees coherent world; pipeline drain не required at save time. Faster save (no waiting для in-flight compute completion).

Pause protocol: natural convergence — sim thread completes current tick, no new dispatch. Pipeline depth naturally absorbs already-dispatched work. К-L18 quiescent state precondition (Item 41 К10.3 v2 — subsequent load-bearing commit) verifies pipeline quiesced before mod operations.

V1 sync dispatch path (К-L7 default): not affected by pipeline drain semantics (no slot machinery involvement). К-L7.1 pipeline-managed path opt-in per field per К-L9 «Vanilla = mods» author choice.

#### 7.2.1 Pre-К10.3 v2 determinism notes

GPU compute results may vary across hardware/driver combinations due to floating-point ordering, parallel reduction differences, and driver optimizations. For Dual Frontier:

- Realtime simulation does not require bit-exact determinism (single-player, no replays)
- Save/load must produce reproducible state on load
- Network multiplayer (not currently scoped) would require strict determinism

Mitigation: CPU reference implementation produces canonical state for save snapshots. Save process pauses GPU dispatch, runs one CPU iteration to produce canonical field state, serializes that. On load, fields restored from canonical state; GPU dispatch resumes.

For hobby-scale single-player, slight non-determinism between sessions is acceptable. The CPU canonical save path is implemented but minimally exercised.

### 7.3 Async sync hazards

#### 7.3.0 Pipeline slot tail read pattern (К-L7.1, К10.3 v2 amendment)

К10.3 v2 К-L7.1 introduces opt-in slot tail read pattern: sim-thread reads see slot tail state (sim_tick - 1) without per-read fence query для pipeline-managed fields. Predicted savings ~30-50% reduction в FieldHandle.ReadCell latency для pipeline-managed paths (Prediction 13).

К-L7 atomic-from-observer invariant preserved within pipeline slot boundary; cross-slot reads see different snapshots (К-L7.1).

**Coexistence (S-LOCK-10 + S-LOCK-13)**: V1's К-L7 sync semantics remain default; pipeline-managed К-L7.1 is opt-in per field. Mod authors choose per-field based на consumer pattern requirements. V1 К-L7 sync default note section follows below.

#### 7.3.1 Pre-К10.3 v2 К-L7 async sync hazards (V1 sync path baseline)

Field reads from managed code use `ReadCell` (point query). If a compute dispatch is in flight, the read may see stale data (last frame's state) or new data (if dispatch completed). Either is acceptable for gameplay because:

- Field values are continuous and slow-changing
- Pawn systems read on per-tick cadence; one-tick stale data is invisible
- Cliff thresholds (electricity effectiveness) hysteresis-free; brief inconsistencies don't cascade

Hard sync (`waitIdle`) is available but only used for save snapshots and shutdown. Game tick path uses fence-based async sync (see [THREADING](./THREADING.md)).

### 7.4 Memory budget

Each 200×200 float field = 160 KB per buffer × 2 (ping-pong) = 320 KB. Plus conductivity map (160 KB) and storage flags (40 KB). Total ~520 KB per active field.

Worst case 10 simultaneous fields: ~5.2 MB. Negligible on any modern GPU (typical 8 GB+ VRAM).

Field grid size scales with map size. 1000×1000 future map size: ~13 MB per field, 130 MB for 10 fields. Still trivial.

---

## 8. Decision log

### 8.1 Resolved (locked — see §0 + §1)

L1–L10 above. Substrate primitive reductions V0/V1/V2 (Q-G-2) above.

### 8.2 Open (deferred)

The «stop, escalate, lock» rule applies; opening any item below requires a brief and an amendment to §0/§1. Same protocol as [MOD_OS_ARCHITECTURE §12](./MOD_OS_ARCHITECTURE.md).

| Decision                                                  | Trigger to resolve                          |
| --------------------------------------------------------- | ------------------------------------------- |
| Editor scope                                              | Post-R.8 evaluation                         |
| Cross-platform support                                    | If/when needed                              |
| Font system (bitmap vs TrueType)                          | R.6 brief authoring                         |
| UI architecture (retained vs immediate-mode)              | R.6 brief authoring                         |
| Vulkan dispatch (`LibraryImport` vs `vkGetInstanceProcAddr`) | Post-foundation if profiling demands     |
| Atlas metadata format (code vs JSON/TOML)                 | When atlas grows                            |
| **G5 projectile Domain B substrate disposition**          | M-V5 reactivation amendment authoring        |
| **G9 eikonal upgrade (V2 tunable vs separate primitive)** | Evidence-gated; V2 close amendment authoring |
| **Hybrid coupling spec (V1↔V2 broken-node)**              | M-V Water demonstration amendment authoring  |
| **Navigation threshold N (~10 placeholder)**              | Vanilla.Movement authoring                   |

---

## 9. Risk register

**R1 — Pure P/Invoke binding tedium exceeds tolerance.**

- Probability: Medium.
- Mitigation: if grinding feels unbearable after ~1000 lines, switch to Vortice.Vulkan (lateral move). Decision recorded as a §8 amendment.

**R2 — Vulkan complexity bugs (synchronization, layout transitions, compute fence sync).**

- Probability: High.
- Mitigation: validation layers ALWAYS on in development. RenderDoc for visual debugging. Validation-clean output added to the [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) checklist. Compute fence-sync invariants explicitly tested with controlled dispatch sequences.

**R3 — PNG decoder edge cases.**

- Probability: Medium.
- Mitigation: extensive test suite with synthetic + real PNG inputs ([TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) §unit).

**R4 — Bitmap font tooling bottleneck.**

- Probability: Medium.
- Mitigation: external tool (BMFont) once for default font.

**R5 — Scope creep into building «engine» instead of game.**

- Probability: High.
- Mitigation: «features only on demand» principle. Each substrate feature must trace to specific Domain requirement. Same discipline as [MOD_OS_ARCHITECTURE Preamble](./MOD_OS_ARCHITECTURE.md) — architectural strength depends on the spec being the only source of truth.

**R6 — Vulkan learning curve disrupts pace.**

- Probability: Low–Medium.
- Mitigation: Vulkan tutorial materials extensive (Vulkan Tutorial, Sascha Willems samples, Khronos compute samples).

**R-V1 — Compute determinism across hardware.**

- Probability: Medium (single-player tolerable; save/load needs canonical).
- Mitigation: CPU reference path mandatory for save snapshots (§7.2).

---

## 10. Operational considerations

**Required tooling — install before V0:**

- Vulkan SDK (LunarG, current 1.3.x).
- RenderDoc (graphics debugger; compute debugging via NVIDIA Nsight or similar).
- Visual Studio 2022 17.8+ (for `[LibraryImport]` source generators).

**Optional:**

- BMFont — bitmap font generator (R.6).
- AssetForge / TexturePacker — atlas tooling (future).

The scaffolding generator `tools/scaffold-runtime.ps1` is committed and idempotent; running it materializes the §2.1 hierarchy without touching Domain. The `Compute/` subdirectory extension is a Q-G-1 cascade addition.

---

## 11. Methodology adjustments for V substrate work

The existing methodology ([METHODOLOGY](/docs/methodology/METHODOLOGY.md)) carries forward with the following adjustments. None of these adjustments alter the pipeline shape; they extend its pre-flight + verification stages for the Vulkan-specific failure modes (both rendering and compute).

**Pre-flight checks adapted:**

- Write-conflict table — applies to Domain commits, not Runtime.
- Project reference direction sanity check — extended: Runtime may not reference Domain (§2.4 Rule 1).
- New: **Validation layer output check** — clean validation output mandatory before commit. Added to [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) checklist.
- New: **CPU/GPU equivalence test** — every new compute shader must have a CPU reference implementation; before commit, verify equivalence on representative inputs within tolerance.

**Brief structure:**

- M-phase boundary check expanded: Runtime (V substrate) / Domain / Mods / Tests boundaries (compare to the boundaries defined in [ARCHITECTURE](./ARCHITECTURE.md) and [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)).
- Visual verification protocol primary (precedent from M8.8 / M8.9 extended).
- Falsification clauses include Vulkan-specific edge cases (layout transitions, swapchain re-creation, validation messages, compute fence sync, descriptor set lifetime).

**Operating principle continues:**

- «Data exists or it doesn't» applies to Vulkan resources.
- New corollary: «State exists or driver crashes» — Vulkan demands explicit state.
- AD numbering continues sequence.

---

## Closing notes

V substrate consolidates rendering + compute into a single Vulkan layer per Q-G-1 LOCK. Substrate primitive reductions to V0/V1/V2 per Q-G-2 LOCK preserve the gameplay surface (mana, electricity, water, heat, sound, scent, pathfinding, projectile bulk compute) while keeping the substrate small. The architectural compound effect — every new field mechanic is mod-level work (~half day) on the same V1 or V2 primitive — is the structural payoff for consolidating the substrate identity.

**«Features only on demand»:** Vulkan API surface is enormous. Resist temptation to build «complete» renderer or «complete» compute framework. Each feature must trace to specific Domain requirement or gameplay mechanic.

This document is **v1.0**, authoritative until amended via explicit decision. Amendments require a commit with rationale, in the same style as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) version-history block.

---

## See also

- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — the development pipeline; the V substrate adjustments in §11 keep this architecture inside the same methodology.
- [CODING_STANDARDS](/docs/methodology/CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, member order; V substrate adheres verbatim.
- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers; V substrate extends the Presentation layer (rendering use case) and provides compute primitives consumable from Domain via `IModApi` (compute use case).
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — companion architectural authority for the modding subsystem; `IModApi.Fields` + `IModApi.ComputePipelines` are the compute consumption surface.
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — native ECS kernel; K9 `RawTileField<T>` is the storage primitive V substrate compute consumes.
- [FIELDS](./FIELDS.md) — field storage contract; consumed by V substrate compute (V1/V2 primitives bind `RawTileField<T>` as SSBO/storage image).
- [THREADING](./THREADING.md) — domain `ParallelSystemScheduler`; the Window+Render thread merge in §2.3 is the only addition. Compute dispatch is fence-async on simulation thread.
- [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) — current dual-backend (Godot DevKit + Silk.NET Native); superseded for production by this document at rendering cutover R.8.
- [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md) — current `PresentationBridge` and Godot-specific glue; deprecated at R.8.
- [MIGRATION_PLAN_KERNEL_TO_VANILLA](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md) — Phase A K-series + Phase B M-cycle sequencing; V substrate work runs in parallel with K-series per β6 sequencing, gates Phase B M-V demonstrations.
- [ROADMAP](/docs/ROADMAP.md) — phase ordering; §6 of this document is the authoritative sequence for the V substrate work.
- [TESTING_STRATEGY](/docs/methodology/TESTING_STRATEGY.md) — test pyramid; §2.8 slots V substrate tests into the existing structure.
- [DEVELOPMENT_HYGIENE](/docs/methodology/DEVELOPMENT_HYGIENE.md) — pre-commit checklist; §11 adds the validation-layer-clean check + CPU/GPU equivalence check.
- [PERFORMANCE](./PERFORMANCE.md) — target metrics; sprite/tile budgets adopted in R.2 / R.3; field compute budgets adopted in V1/V2.
- [COMPOSITE_NAMESPACE_DELIBERATION_STATE](./COMPOSITE_NAMESPACE_DELIBERATION_STATE.md) — ratification authority for Q-G-1 + Q-G-2 LOCK (substrate consolidation and primitive reductions).
- [CPP_KERNEL_BRANCH_REPORT](/docs/reports/CPP_KERNEL_BRANCH_REPORT.md) — Discovery report establishing K0 cherry-pick scope.

## Part 12: Relationship to KERNEL_ARCHITECTURE.md

VULKAN_SUBSTRATE.md (this) and KERNEL_ARCHITECTURE.md describe two halves of a single architectural vision: native foundation under managed Application layer. K substrate (kernel) and V substrate (Vulkan) are independent layers reachable from managed Application layer through respective bridges; the two-substrate symmetry was implicit in the prior RUNTIME + GPU_COMPUTE split and is made explicit by the Q-G-1 unification.

**Symmetric architecture**:
- This document (V substrate): Vulkan rendering + compute layer; substrate primitives V0/V1/V2; rendering use case implementation R.0..R.8; M-V demonstrations
- KERNEL_ARCHITECTURE.md: native ECS kernel layer; substrate K0..K9; M-K demonstrations (vanilla mods on K substrate)
- Both: pure P/Invoke to native (`vulkan-1.dll` for V; `DualFrontier.Core.Native.dll` for K)
- Both: managed thin adapter layer
- Both: single ownership boundary, direction-disciplined (managed → native)

**Independent layers**: rendering knows nothing about ECS storage; ECS kernel knows nothing about Vulkan. Both reachable from managed Application layer through respective bridges. Compute use case of V bridges to K through `RawTileField<T>` (K9 storage primitive) — the storage is in K, the compute is in V, the binding is `FieldStorageBinding.cs` in V0.

**Combined timeline**: see KERNEL_ARCHITECTURE.md Part 2 for K-series sequencing; this document §6 for V substrate sequencing. V work can proceed in parallel with K-series (independent layers), with the integration point at K9 + V0 (field storage abstraction + Vulkan compute plumbing).

**Cross-document invariants**: «без компромиссов», operating principle (data exists/doesn't), single ownership boundary, direction-discipline, long-horizon planning. See KERNEL_ARCHITECTURE.md Part 8 for full invariant list.

**LOCKED v1.0** — supersedes prior `RUNTIME_ARCHITECTURE.md` v1.0 + `GPU_COMPUTE.md` v2.0. Departures require explicit re-architecture milestone and updates to dependent K9/V-series briefs.
