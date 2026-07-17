---
register_id: DOC-F-SRC-RUNTIME-COMPUTE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-18
last_modified: 2026-05-19
content_language: mixed
next_review_due: 2026-Q4
title: DualFrontier.Runtime / Compute — module doc
last_modified_commit: 6758298
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime / Compute

V0.B compute pipeline plumbing per VULKAN_SUBSTRATE.md §1.1 V0 compute use case +
§3 (compute use case detail). V1 substrate primitive extension per §1.2 + §5.1.

## Surface

- `VulkanComputePipeline` — VkPipeline с VK_PIPELINE_BIND_POINT_COMPUTE, owns
  the pipeline lifetime (caller passes shader module + descriptor set layouts +
  pipeline layout).
- `VulkanComputeDescriptors` — descriptor set layout + descriptor pool + allocated
  descriptor sets bundle.
- `ComputeDispatch` — wraps command buffer recording + queue submit + fence wait.
  V0.B synchronous (К-L7 atomic-from-observer); V1+ may add async variant.
- `ComputePipelineRegistry` — named pipeline lookup, owns pipelines, disposed
  alongside Runtime facade.
- `FieldStorageBinding` — managed wrapper bridging K9 RawTileField storage к
  the Vulkan compute pipeline registration on the native side. Provides
  `Attach`/`Register`/`DispatchField`/`PipelineCount` surface.

## V1 diffusion substrate primitive (per VULKAN_SUBSTRATE §1.2 + §5.1)

`V1DiffusionPipeline` — managed wrapper for the V1 isotropic + anisotropic
diffusion compute shader (`tools/shaders/diffusion.comp`). Wires а single GLSL
shader template handling both variants:

  - Isotropic: uniform conductivity D, asymmetric flow rule collapses к
    `D · (P_neighbour - P_self)` per neighbour, summed → standard 4-neighbour
    Laplacian stencil.
  - Anisotropic: per-cell conductivity from а storage buffer. Asymmetric flow
    rule `min(D_self, D_neighbour) · (P_n - P_s)` makes high-D paths (wires)
    channel propagation automatically, since flow blocks whenever either side
    has zero conductivity (insulator).

### Push constants (16 bytes)

`DiffusionPushConstants`:

| Offset | Field              | Type   | Meaning                                |
|-------:|--------------------|--------|----------------------------------------|
|     0  | DecayCoefficient   | float  | K в `∂P/∂t = D·∇²P - K·P`              |
|     4  | DeltaTime          | float  | dt; integrate explicit-Euler step      |
|     8  | Width              | uint32 | Grid width (cells)                     |
|    12  | Height             | uint32 | Grid height (cells)                    |

### Descriptor bindings (set 0)

| Binding | Buffer            | Access      | Type     |
|--------:|-------------------|-------------|----------|
|       0 | input field       | readonly    | float[]  |
|       1 | output field      | writeonly   | float[]  |
|       2 | conductivity map  | readonly    | float[]  |

### Stability bound (CFL)

The explicit-Euler stencil is stable while `4·D_max·dt < 1`. At `4·D·dt = 1`
the system sits on the oscillation boundary (center ↔ neighbours flip-flop
every iteration); above 1 the system blows up. Choose `dt` so that `4·D_max·dt`
is comfortably below 1 (≤ 0.5 в practice).

### Ping-pong contract

Consumers needing multi-iteration evolution call
`FieldHandle<T>.SwapBuffers()` between dispatches so the next iteration reads
the previous output. Native-side `df_world_field_dispatch_compute` already
implements per-field shadow VkBuffers + barriers; consumers only manage CPU
buffer roles (primary ↔ scratch).

### Iteration economics

Typical gameplay configurations dispatch 5–10 iterations per tick per active
field (one tick × N iterations × ~1 ms/iter ≈ 5–10 ms budget for one field).
With per-frame dispatch budget ~16 ms (60 FPS), 1–3 simultaneous fields fit
comfortably; V substrate close criterion (per VULKAN_SUBSTRATE §1.4) is multi-field
coexistence (V1 mana + V1 electricity + V2 distance + V2 direction) at 60+ FPS.

## Runtime composition (V1-14 factories)

Application/mod code obtains а V1 diffusion pipeline through Runtime factories
без open-coding SPIR-V loading:

```csharp
using var world = new NativeWorld();
var binding = runtime.CreateFieldStorageBinding(world);
var pipeline = runtime.CreateV1DiffusionPipeline(binding, "vanilla.magic.mana");

var pc = new DiffusionPushConstants {
    DecayCoefficient = 0.01f, DeltaTime = 0.5f, Width = 200, Height = 200,
};
pipeline.ExecuteIteration("vanilla.magic.mana", pc, dispatchX: 25, dispatchY: 25);
```

World ownership stays с the caller; Runtime supplies only the Vulkan handles
and caches the SPIR-V bytecode on first call (subsequent pipelines reuse the
cached bytes).

## CPU reference oracles (per VULKAN_SUBSTRATE §11)

Every V1 compute shader has а CPU reference implementation in
`src/DualFrontier.Core.Interop/CpuKernels/`:

  - `IsotropicDiffusionKernel` — 4-neighbour Laplacian с uniform D, reflective
    boundary; matches the shader when the conductivity buffer is uniform.
  - `AnisotropicDiffusionKernel` — asymmetric `min(D_self, D_neighbour)` flow
    rule; matches the shader on per-cell varying conductivity.

Equivalence tests in `tests/DualFrontier.Runtime.Tests/Compute/V1DiffusionEquivalenceTests.cs`
verify CPU ≡ GPU within 0.001f per-cell tolerance across edge cases (corner
source, decay-only, decay + diffusion, multi-iteration counts, anisotropic
wire path, anisotropic wall с/without gap, 50-iteration mass conservation).

## Out of scope (V1)

- V2 wave shader + direction extraction — V2 brief portion, separate PR.
- M-V demonstration mods (Vanilla.Magic mana, Vanilla.Electricity power,
  Vanilla.Movement flow-field pathfinding) — post-V substrate close.
- Eikonal upgrade (per VULKAN_SUBSTRATE §1.3.1) — deferred TBD, evidence-gated
  к M-V7 pathfinding measurement.
