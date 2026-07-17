---
register_id: DOC-A-VULKAN_SUBSTRATE
project: Dual Frontier
category: A
tier: 1
lifecycle: SUPERSEDED
owner: Crystalka
version: 1.2.0
first_authored: 2026-07-15
last_modified: 2026-07-15
content_language: en
next_review_due: null
title: Vulkan Substrate (V) — Dual Frontier (historical; superseded by authored rework)
superseded_by: DOC-A-VULKAN_SUBSTRATE_V2
supersedes:
- DOC-D-G0
- DOC-D-G1
- DOC-D-G2
- DOC-D-G3
- DOC-D-G4
- DOC-D-G5
- DOC-D-G6
- DOC-D-G7
- DOC-D-G8
- DOC-D-G9
- DOC-A-VISUAL_ENGINE
- DOC-A-GODOT_INTEGRATION
last_modified_commit: fcbfe5b
review_cadence: on-change+annual
last_review_date: 2026-05-20
last_review_event: 'К10.3 v2 load-bearing commit 3/3 2026-05-20 — К-L18 final amendment landed: §3.4.1 «df_vulkan_unload_mod_resources C ABI primitive (К-L18, К10.3 v2 placeholder)» subsection (struct shape + signature + К10.3 v2 placeholder behavior + full implementation reservation для V-cycle / К-extensions). К10.3 v2 cascade complete: §0 L1/L7 К-L19 cleanup + §2.0 pipeline depth + §2.3 threading model К-L7.1/L16 + §3.4 К-L19 mandate cleanup + §3.4.1 df_vulkan_unload_mod_resources placeholder + §4.0 display composition К-L17 + §5.5 Mode C visibility К-L17 + §7.2 drain semantics К-L16 + §7.3 К-L7.1 slot tail read pattern.'
reviewer: Crystalka
risks_referenced:
- RISK-004
- RISK-013
special_case_rationale: 'Superseded by DOC-A-VULKAN_SUBSTRATE_V2 per corpus rework EVT-2026-07-15-CORPUS_REWORK_R3_SUBSTRATE. Last-ratified reference preserved at docs/architecture/historical/VULKAN_SUBSTRATE.md; successor ratified LOCKED v1.0.0 2026-07-17 (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION) — authority-gap window closed. Prior rationale: Unified V substrate per Q-G-1 LOCK (COMPOSITE_NAMESPACE_DELIBERATION_STATE.md §3.1). Supersedes prior DOC-A-RUNTIME (RUNTIME_ARCHITECTURE.md) + DOC-A-GPU_COMPUTE (GPU_COMPUTE.md); single Vulkan substrate covers rendering + compute use cases. Additionally supersedes G-series briefs DOC-D-G0..G9 per Q-G-2 LOCK + CLEANUP_CASCADE_BRIEF §1.1 (Crystalka lock 2026-05-16). Additionally supersedes DOC-A-VISUAL_ENGINE + DOC-A-GODOT_INTEGRATION (visual-runtime authority moved to docs/architecture/historical/) per CLEANUP_CASCADE_BRIEF §1.3 (Crystalka lock 2026-05-16). Bidirectional integrity per FRAMEWORK §3.3.2.'
---

# Vulkan Substrate (V) — Dual Frontier

**Status:** AUTHORITATIVE LOCKED — unified Vulkan substrate spec covering rendering + compute use cases. Version and lifecycle are owned by the document register (frontmatter mirror above).

**Supersedes:** prior `RUNTIME_ARCHITECTURE.md` v1.0 LOCKED (rendering layer spec) + `GPU_COMPUTE.md` v2.0 LOCKED (compute layer spec). Both source documents were physically describing **one** Vulkan device with two use cases; the documentation drift introduced separate substrate identities for what is one physical layer. Per Q-G-1 LOCK in `docs/architecture/COMPOSITE_NAMESPACE_DELIBERATION_STATE.md` §3.1, R (runtime) and G (GPU compute) substrate buckets merged into unified Vulkan substrate **V**.

**Companion documents:** [METHODOLOGY](../methodology/METHODOLOGY.md), [CODING_STANDARDS](../methodology/CODING_STANDARDS.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [ARCHITECTURE](./ARCHITECTURE.md), [THREADING](./THREADING.md), [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) (historical), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md) (historical), [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md), [FIELDS](./FIELDS.md), [ROADMAP](../ROADMAP.md).

> **Presentation reality.** Godot is fully retired (К-extensions cascade #2, 2026-05-23): the `DualFrontier.Presentation*`
> projects were deleted and `DualFrontier.Launcher` (pure Win32 + Vulkan P/Invoke through `DualFrontier.Runtime`) is the
> production render host. Authoritative on-disk state: `src/DualFrontier.Launcher/` + `src/DualFrontier.Runtime/` +
> `native/DualFrontier.Core.Native/`. **Residual:** a `project.godot` file still exists at the repo root
> (Crystalka-owned, not deleted autonomously — [DEVELOPMENT_HYGIENE §7 (Godot status)](../methodology/DEVELOPMENT_HYGIENE.md);
> tracked as Findings-ledger item F-5 in [ROADMAP](../ROADMAP.md)).

**Scope:** Full architectural specification for the Vulkan substrate (V) — single `VkInstance` / `VkDevice` / `vulkan-1.dll` linkage serving both 2D rendering and compute. Defines substrate primitives V0/V1/V2 (compute-side), rendering use case implementation, compute use case implementation, threading model, asset pipeline, shader strategy, mod-driven compute pipeline registration, mathematical models for field-based gameplay mechanics, and failure modes. This document answers what the substrate **is**; substrate status and sequencing live in [ROADMAP §Native foundation tracks](../ROADMAP.md). The Domain layer ([ARCHITECTURE §Domain](./ARCHITECTURE.md), [ECS](./ECS.md), [EVENT_BUS](./EVENT_BUS.md), [ISOLATION](./ISOLATION.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) is preserved verbatim by this layer — see L10 in §0.

**Version history:**

- **v1.2.0 (2026-06-12, this version)** — Architecture Truth Cascade body scrub (D2 per `tools/briefs/ARCHITECTURE_TRUTH_CASCADE_BRIEF.md` §5-W2). Body rewritten to realized state: V0 + V1 substrate primitives recorded as shipped with evidence (V0 close 2026-05-19 per Q8 ratification; V1 close PR #40 merge `88aebf2`); V2 reduced to design rationale with pending status pointed at ROADMAP; §2.1 tree replaced with the verified on-disk tree (Launcher in, Presentation out, real shader set, real mod set); §2.2–§2.4 Presentation/scheduler residues fixed to Launcher/native-scheduler truth; §4.1–§4.2 migration narrative converted to completed record; §6 forward tables converted to evidence-marked status records with ROADMAP as single status authority; §7.1 CPU-fallback claim corrected to К-L19 fail-fast truth; phantom `tools/build-all.ps1` corrected to the `CompileShaders` MSBuild target; closing-note and Part 12 self-version pins removed; stale companion labels marked historical. Forward state authority: `docs/ROADMAP.md`.

- **v1.1.2 (2026-06-02)** — DD-1 code-truth banner + surgical fixes (commit `6480df1`, Documentation Dual-Load Drift Reconnaissance batch across 6 architecture docs). Banner superseded by the v1.2.0 body scrub.

- **v1.1.1 (2026-05-23)** — К-extensions cascade #2 closure patch bump per Q-G-12 LOCKED. Substantive content unchanged; cleanup-only patch records Godot deprecation + Launcher formalization status. R.8 «Migration cutover (delete Godot)» step in §6 marked complete (К-extensions cascade #2 closure: tracked Godot files removed, DualFrontier.Launcher production renderer formalized per Q-G-6 (b1) infrastructure-only). Migration narrative throughout §4 retained as historical record of pre-cascade-#2 state — readers should treat "parallel Godot+Vulkan" framing as past-tense reference к the pre-cascade migration approach.

- **v1.1 (2026-05-20)** — К10.3 v2 load-bearing commit 1/3 reconciliation per S-LOCK-14. Consolidates: (a) К-L19 deferred V0.B amendments (§0 L1 Vulkan 1.3 + async compute queue mandate notation; §0 L7 К-L19 hardware tier baseline note; §3.4 К-L19 mandate documentation); (b) К10.3 v2 К-L7.1/L16 amendments (§2 pipeline depth architecture subsection; §2.3 threading model pipeline depth + queue family roles subsection; §7.2 pipeline drain semantics; §7.3 pipeline slot tail read pattern). К-L17 + К-L18 amendments land в subsequent К10.3 v2 load-bearing commits.

- **v1.0 (2026-05-16)** — unified V substrate spec authored as cascade output of composite namespace ratification (Q-G-1 + Q-G-2 LOCK). Consolidates verbatim content from `RUNTIME_ARCHITECTURE.md` v1.0 (foundation decisions L1–L10, rendering migration sequencing, module structure, threading, asset pipeline, shader strategy) and `GPU_COMPUTE.md` v2.0 (two compute domains, field abstraction, mathematical models, flow field pathfinding, failure modes). Restructured per Q-G-2 LOCK: substrate primitives reduced from G0..G6+G9 (seven items) to V0/V1/V2 (three items) via six successive reductions (storage as gameplay metadata, wave shader as gameplay-driven, two-layer model, distribution+navigation unified, CPU/GPU policy decoupled, autonomous=GPU persistent). Упразднено: G3 storage cells (now gameplay-level node config), G6 flow field infrastructure (folded into V2 wave shader side products), G4 multi-field coexistence (now substrate close acceptance criterion, not separate primitive). Deferred: G5 projectile Domain B (substrate identity TBD), G9 eikonal upgrade (folded into V2 tunable or separate primitive — evidence-gated). M-V demonstrations cited per Q-R-1 format (M-V1 mana, M-V2 electricity, M-V5 projectile (deferred), M-V7 movement, M-V8 local avoidance; gaps M-V3/M-V4/M-V6/M-V9 reflect Q-G-2 reductions).

---

## Preamble — How to use this document

**Authority.** This document is the single architectural authority for the Dual Frontier Vulkan substrate (V) — window management, GPU rendering, input, sprite batching, text, asset loading, diagnostic tooling, compute pipeline plumbing, field-based compute mechanics. Every shipped interface, P/Invoke declaration, struct layout, and lifecycle step in the substrate traces back to a section here; the same rule binds the remaining substrate work (V2). Disagreement with the specification is escalated to the human (via §8.2 open decisions) — never resolved by improvisation in code, mirroring the discipline established for the modding layer in [MOD_OS_ARCHITECTURE Preamble](./MOD_OS_ARCHITECTURE.md).

**Scope.** The specification governs:

- The structural relationship between the V substrate and the existing four layers ([ARCHITECTURE](./ARCHITECTURE.md)).
- The Win32 + Vulkan P/Invoke surface (functions, structs, enums, callbacks).
- The Vulkan resource lifetimes (instance, device, swapchain, pipeline, buffer, image, memory, compute pipeline, descriptor sets).
- The sprite + text + atlas batching model (rendering use case).
- The PNG decoder and shader compilation pipeline.
- The threading model on top of [THREADING](./THREADING.md) (window+render thread merged with simulation thread preserved).
- The compute pipeline plumbing (V0 substrate primitive) for both Domain A (field updates) and Domain B (entity-keyed bulk computation).
- Scalar field primitives V1 (diffusion shader, shipped) and V2 (wave shader, pending) — substrate-level abstractions designed for consumption by vanilla mods as gameplay mechanics.
- The completed migration from the dual-backend Godot+Silk.NET state ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md), both historical) to the Vulkan-only target — historically tracked as M9.0..M9.8 runtime milestones, unified within V substrate (Q-R-2 LOCK), cutover completed at К-extensions cascade #2 (2026-05-23). §4 records this as a realized migration.

The specification does **not** govern:

- Domain content — Domain is preserved verbatim, see [ARCHITECTURE](./ARCHITECTURE.md), [ECS](./ECS.md), [CONTRACTS](./CONTRACTS.md), [EVENT_BUS](./EVENT_BUS.md).
- The mod system — covered by [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [MODDING](./MODDING.md), [MOD_PIPELINE](./MOD_PIPELINE.md). V substrate exposes presentation + compute primitives only; mods cannot reach the substrate layer directly except through `IModApi` v3 surface (`Fields`, `ComputePipelines` sub-APIs).
- Game-design questions (balance, narrative, pacing).
- Field storage data layout — owned by [FIELDS](./FIELDS.md) and [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) K9 (`RawTileField<T>`). V substrate consumes the field storage primitives; it does not define them.
- Methodology of the development pipeline — covered by [METHODOLOGY](../methodology/METHODOLOGY.md).

**The "stop, escalate, lock" rule.** When implementation encounters a design question not answered here, the response is "stop, document in §8.2, wait for the human to lock" — not "guess." Same discipline as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) Preamble.

---

## Executive summary

Dual Frontier's Vulkan substrate (V) is a **unified Vulkan 1.3 layer** serving two use cases on one `VkInstance` / `VkDevice` / `vulkan-1.dll` linkage:

- **Rendering use case** — Win32 window, swapchain, batched sprite rendering, atlas-based 2D rendering. Shipped as `DualFrontier.Runtime`, consumed by the production host `DualFrontier.Launcher`. It replaced the Godot 4 + C# Presentation layer ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md), [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md), both historical) — Godot was physically removed at К-extensions cascade #2 (2026-05-23). Text rendering and UI primitives have not shipped yet (Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md)).
- **Compute use case** — field-based GPU compute (Domain A) + entity-keyed bulk computation (Domain B). Substrate-level abstraction for diffusion / wave / flow field gameplay mechanics. V0 compute plumbing and the V1 diffusion primitive are shipped (`src/DualFrontier.Runtime/Compute/`); mod-driven shader registration is a designed contract, not yet wired to mods (§3.3).

The Domain layer ([ARCHITECTURE](./ARCHITECTURE.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [THREADING](./THREADING.md), [ISOLATION](./ISOLATION.md)) was preserved verbatim — zero touch by V substrate work. Only the presentation layer was rebuilt on the new foundation: the Godot-era `DualFrontier.Presentation` projects were deleted and `DualFrontier.Launcher` now hosts the game on `DualFrontier.Runtime`.

**Foundation philosophy** — «без компромиссов»:

- Pure P/Invoke to `vulkan-1.dll` (no third-party C# binding library).
- Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`).
- BCL only for math (`System.Numerics`) and compression (`System.IO.Compression.DeflateStream`).
- Manual PNG decoder (DEFLATE through BCL, chunk parsing manual).
- Build-time GLSL → SPIR-V via `glslangValidator.exe`.
- Production binary depends only on `vulkan-1.dll` (GPU driver) and pre-compiled `.spv` shader files.

Total ownership: every line above OS API surface is project's own code.

**Architectural insight (Q-G-1 + Q-G-2):** Rendering and compute share one Vulkan device. Treating them as separate substrates (R-bucket and G-bucket) was documentation drift. Compute substrate primitives reduce to three items (V0 plumbing, V1 diffusion, V2 wave) once gameplay mechanics are recognized as **configurations** of physical primitives rather than substrate primitives in their own right (cf. Lesson #12 candidate, deliberation document §6.3). Distribution networks (mana, electricity, water, heat), navigation (flow-field pathfinding), and crowd behavior all reduce to V1+V2 configuration — substrate stays small, gameplay stays expressive.

**Realized state (evidence record):** V0 foundation closed 2026-05-19 (Q8 ratification; `docs/MIGRATION_PROGRESS.md` V-series table). V1 diffusion closed 2026-05-19 (PR #40, merge `88aebf2`). Rendering cutover (Godot deletion) completed at К-extensions cascade #2, 2026-05-23. V2 wave, the M-V demonstration mods, and the remaining rendering scope (text/UI) are pending — status authority: [ROADMAP §Native foundation tracks](../ROADMAP.md).

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

**Implication of L10.** All existing Domain namespaces — `DualFrontier.Core`, `DualFrontier.Contracts` (including `DualFrontier.Contracts.Modding`), `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Systems`, `DualFrontier.Application` (including `DualFrontier.Application.Modding`), `DualFrontier.Persistence`, `DualFrontier.AI` — are untouched by substrate work. The existing test suite passed throughout the migration ([TESTING_STRATEGY](../methodology/TESTING_STRATEGY.md) owns the census). Mod system contracts ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) remain unchanged — V substrate is not visible from a mod's `AssemblyLoadContext` directly. The mod-facing compute surface (v3, K8.4 closure): `IModApi.Fields` (`IModFieldApi`, wired in production via `RestrictedFieldApi` when native field storage is present) and `IModApi.ComputePipelines` (`IModComputePipelineApi` contract placeholder — the production implementation currently returns `null`; the real registration surface is Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md)).

---

## 1. V substrate primitives (Q-G-2 LOCK)

The V substrate is built from three primitives, in dependency order. Each primitive is a **substrate-building milestone**; closure of all three plus the multi-field-coexistence acceptance criterion (the former G4, now subsumed) constitutes V substrate close. Realized so far: **V0 and V1 are shipped** (closure evidence in the respective sections below); **V2 is pending** (no wave shader on disk). Status authority: [ROADMAP §Native foundation tracks](../ROADMAP.md).

### 1.1 V0 — Vulkan substrate foundation (shipped)

**Scope:** All Vulkan plumbing shared by rendering and compute use cases. Single `VkInstance`/`VkDevice` linkage to `vulkan-1.dll`. Validation layer setup. SPIR-V toolchain. Memory allocator. Compute pipeline plumbing. Render pipeline plumbing. Win32 window + surface + swapchain. Threading model (window+render thread merged, simulation thread preserved).

**Historical context:** Pre-Q-G-1, this scope was split across `RUNTIME_ARCHITECTURE.md` (rendering side: Win32, swapchain, sprite/text/atlas plumbing) and `GPU_COMPUTE.md` G0 (compute-pipeline-only plumbing). Q-G-1 LOCK recognizes that both sides share one `VkInstance`/`VkDevice` physically; the substrate identity is one (V0), the use cases are two (rendering, compute).

**Rendering side deliverables** (former M9.0..M9.7 work, executed as V0 sub-milestones V0.A/V0.B/V0.C.1/V0.C.2 — realized record):

- Win32 window + surface (`vkCreateWin32SurfaceKHR`) — shipped (`Window/Window.cs`, `Graphics/VulkanSurface.cs`).
- Vulkan instance + physical/logical device + queue family selection — shipped (`Graphics/VulkanInstance.cs`, `Graphics/VulkanDevice.cs`, `Graphics/QueueFamilyInfo.cs`).
- Validation layer (`VK_LAYER_KHRONOS_validation`) enabled in DEBUG — shipped (`Graphics/ValidationLayer.cs`; default per `RuntimeOptions.EnableValidationLayer`).
- Swapchain + recreation — shipped (`Graphics/VulkanSwapchain.cs`; `Runtime.RecreateFramebuffersForSwapchain`).
- Render pass + graphics pipeline + command pool/buffer — shipped (`Graphics/VulkanRenderPass.cs`, `Graphics/VulkanGraphicsPipeline.cs`, `Graphics/VulkanCommandPool.cs`, `Graphics/VulkanCommandBuffer.cs`).
- Vertex buffer + image + memory allocator (bumper allocator) — shipped (`Graphics/VulkanBuffer.cs`, `Graphics/VulkanImage.cs`, `Graphics/MemoryAllocator.cs`).
- PNG decoder + asset manager — shipped (`Assets/PngDecoder.cs`, `Assets/AssetManager.cs`).
- Batched sprite renderer + `Camera2D` orthographic projection + `TileMap` — shipped (`Sprite/SpriteRenderer.cs` batched BeginFrame/Submit/EndFrame API, `Sprite/Camera2D.cs`, `Sprite/TileMap.cs`).
- Input event queue (Win32 messages → typed events) — shipped (`Window/InputEventQueue.cs`, `Input/` event types, `Input/VirtualKeyMapper.cs`). Forwarding of drained events into Domain is not yet wired in the Launcher (Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md)).
- Validation log — shipped (`Diagnostic/ValidationLog.cs`).
- **Not shipped within V0:** bitmap font + text renderer, debug overlay + frame timer (no `Text/` module, no `DebugOverlay`/`FrameTimer` types on disk). These remain rendering-scope work — Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).

**Compute side deliverables** (former G0 work, executed within V0.B — realized record):

- Compute pipeline (`VkPipeline` with `VK_PIPELINE_BIND_POINT_COMPUTE`) — shipped (`Compute/VulkanComputePipeline.cs`; native `compute_pipeline.cpp`).
- Compute descriptor set layout + descriptor pool + pipeline layout — shipped (`Compute/VulkanComputeDescriptors.cs`).
- Storage buffer binding for fields — shipped (`Compute/FieldStorageBinding.cs` — K9 `RawTileField<T>` → SSBO).
- Compute pipeline registration C ABI (`df_world_register_compute_pipeline`) — shipped (`native/DualFrontier.Core.Native/include/df_capi.h`).
- Compute dispatch C ABI (`df_world_field_dispatch_compute`) — shipped (same header; `compute_dispatch.cpp`).
- Fence-based sync between CPU writes (conductivity updates) and GPU dispatch — shipped (К-L7 sync path; `Graphics/VulkanFence.cs`).
- Build-time compute shader compilation — shipped (`CompileShaders` MSBuild target in root `Directory.Build.props`; `noop.comp` + `diffusion.comp` compiled via `tools/glslangValidator.exe`).
- Tests — shipped (`tests/DualFrontier.Runtime.Tests/Compute/ComputePipelineRegistrationTests.cs`: registration round-trip + no-op dispatch).

**Exit criteria — met (closure record, V0 close 2026-05-19 per Q8 ratification; chronicle: `docs/MIGRATION_PROGRESS.md` V-series table):**

- Window opens (Win32), Vulkan instance + device live, validation layer reports zero errors — V0.A closure 2026-05-18.
- Sprite/tile rendering at 60+ FPS — V0.C.1 smoke run: 820 frames at 164 FPS (AMD RX 7600S, 2026-05-19); V0.C.2 added the 10K-sprite stress scene and the 200×200 TileMap scene.
- Compute pipeline registration round-trip + empty dispatch without error — V0.B closure 2026-05-18 (native selftest compute roundtrip scenario).
- All existing Domain tests pass (Domain layer untouched) — held at every V0 sub-closure (875 tests at V0.C.1).
- Clean shutdown (no leaked Vulkan handles per validation) — `Runtime.Create` disposes partially-constructed components on failure; validation-clean exit verified in the closure smoke runs.

### 1.2 V1 — Scalar field + diffusion shader (environmental layer, shipped)

**Scope:** First substrate-level compute primitive. Scalar field type backed by `RawTileField<T>` (K9 storage) with **isotropic diffusion shader** describing environmental distribution. Shipped 2026-05-19 (PR #40 V1-series, merge `88aebf2`): `tools/shaders/diffusion.comp` (+ compiled `assets/shaders/diffusion.comp.spv`), `src/DualFrontier.Runtime/Compute/V1DiffusionPipeline.cs` + `DiffusionPushConstants.cs`, `Runtime.CreateFieldStorageBinding` / `Runtime.CreateV1DiffusionPipeline` factories, CPU oracle kernels `IsotropicDiffusionKernel` + `AnisotropicDiffusionKernel` (`src/DualFrontier.Core.Interop/CpuKernels/`).

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

**M-V1 demonstration:** Vanilla.Magic mod (`ManaField`) — the first production-shaped V1 use case. Pending: `mods/DualFrontier.Mod.Vanilla.Magic` is a strict-v3 skeleton with an empty `Initialize`. Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

**Exit criteria — substrate-level criteria met (closure record, V1 close 2026-05-19; commits `7ad0560` closure + `88aebf2` merge):**

- Sources spread spatially per shader; CPU oracle and GPU output match within tolerance — `V1DiffusionEquivalenceTests` (isotropic vs `IsotropicDiffusionKernel`, anisotropic vs `AnisotropicDiffusionKernel`), plus isotropic and anisotropic-wire-path 200×200 smoke scenes (commits `34d85e0`, `2f00a8f`).
- Local point query reads — `FieldHandle<T>.ReadCell` (shipped, `src/DualFrontier.Core.Interop/FieldHandle.cs`); consumed by the equivalence and smoke tests.
- Anisotropic variant propagates along wire paths; insulator blocking holds — anisotropic insulator-equivalence scenarios (commit `59dfc72`) + wire-path smoke scene.
- Conductivity update API — `FieldHandle<T>.SetConductivity` shipped and exercised by tests; the *player-build-action* trigger is a mod-level (M-V2) concern, pending with the demonstration mods.

Mod-level demonstration criteria (mana spell-casting reads, player wire-building) belong to M-V1/M-V2 — Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).

### 1.3 V2 — Scalar field + wave shader (routed layer, pending — design rationale)

**Status:** Not implemented. No wave shader exists on disk (`tools/shaders/` contains no `wave.comp`; `assets/shaders/` contains no `wave.comp.spv`). This section is the authored design rationale; scheduling and status live in [ROADMAP §Native foundation tracks (V substrate)](../ROADMAP.md).

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

**M-V2 / M-V7 demonstrations:** Vanilla.Electricity (wave through cables with breakable propagation) and Vanilla.Movement (pathfinding via V2 routed flow field). Neither mod exists on disk yet. Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

**Exit criteria (design contract — to be evidenced at V2 closure):**

- Distance field converges to correct gradient on representative grids.
- Direction field extraction produces walkable gradient toward target.
- Wave propagation respects walls / closed-pipe / cut-cable barriers (verified in synthetic obstacle scenarios).
- M-V2 (electricity through cables) and M-V7 (pawn navigation via flow field) demonstrate V2 use cases.

#### 1.3.1 G9 eikonal upgrade (deferred TBD)

The eikonal solver (Option A) yields geodesic-accurate distances; Option B (simple diffusion) does not. Deliberation §3.2 deferred the question of whether eikonal is V2 tunable parameter (compile-time / dispatch-parameter selection between diffusion and eikonal) or a separate V-primitive. Resolution gated on measurement evidence: if Option B baseline shows gameplay-relevant suboptimality, the upgrade to Option A is justified; otherwise diffusion baseline remains. Brief authoring at amendment time captures the evidence.

#### 1.3.2 G5 projectile Domain B (deferred TBD)

Per-entity bulk computation (the original `ProjectileSystem` GPU path, Domain B below in §5) is a separate compute domain from V1/V2 field updates. Deliberation §3.2 deferred whether this stays within V substrate (as a third primitive or as part of V0 compute plumbing), becomes own substrate, or stays consumer-level. M-V5 namespace reserved (per Q-R-1) but identifier and substrate disposition deferred to future deliberation post-substrate work.

### 1.4 V substrate close acceptance criteria

V substrate close criteria, gating, and current standing are tracked in [ROADMAP §Native foundation tracks (V substrate)](../ROADMAP.md) — the single forward-state authority. The multi-field-coexistence acceptance criterion (former G4) closes the substrate together with V0/V1/V2. Future V-N primitive identifiers (V3+) remain reserved for post-substrate compute needs (G5 projectile resolution per §1.3.2, G9 eikonal upgrade per §1.3.1 if evidence justifies, modder-driven primitives).

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

### 2.1 Project structure (on-disk, verified 2026-06-12)

```
src/
  // ====== Domain layer (preserved verbatim — zero substrate touch) ======
  DualFrontier.AI/                            // pathfinding (AStarPathfinding, IPathfindingService)
  DualFrontier.Application/                   // bootstrap, GameLoop, PresentationBridge, Display/, Modding/
  DualFrontier.Components/
  DualFrontier.Contracts/                     // incl. Contracts/Modding (IModApi v3) + Contracts/Display
  DualFrontier.Core/                          // ECS, Bus, Scheduling (managed side)
  DualFrontier.Core.Interop/                  // native kernel bridge: NativeWorld, FieldRegistry,
                                              //   FieldHandle<T>, NativeMethods.{Compute,Fields,…},
                                              //   CpuKernels/{Isotropic,Anisotropic}DiffusionKernel
  DualFrontier.Crypto.Future/
  DualFrontier.Events/
  DualFrontier.Persistence/
  DualFrontier.Systems/

  // ====== V substrate (Vulkan + Win32 foundation) ======
  DualFrontier.Runtime/
    DualFrontier.Runtime.csproj               // references exactly one project: DualFrontier.Core.Interop
    MODULE.md
    Runtime.cs                                // top-level facade (composition + frame + compute factories)
    RuntimeOptions.cs                         // window options, assets dir, validation-layer default

    Native/                                   // MODULE.md per directory (here and below)
      Win32/   Win32Api.cs / Win32Constants.cs / Win32Structs.cs / WindowProc.cs
      Vulkan/  VkApi.cs / VkEnums.cs / VkStructs.cs / VkConstants.cs / VkDelegates.cs

    Window/  IWindow.cs / Window.cs / WindowOptions.cs / InputEventQueue.cs
    Input/   IInputEvent.cs / KeyPressedEvent.cs / KeyReleasedEvent.cs / MouseMovedEvent.cs
             MouseButtonEvent.cs / MouseWheelEvent.cs / WindowResizeEvent.cs / WindowFocusEvent.cs
             Key.cs / MouseButton.cs / VirtualKeyMapper.cs

    Graphics/
      VulkanInstance.cs / VulkanDevice.cs / VulkanSurface.cs / VulkanSwapchain.cs / SwapchainImage.cs
      VulkanRenderPass.cs / VulkanFramebuffer.cs / VulkanGraphicsPipeline.cs / VulkanPipelineLayout.cs
      VulkanCommandPool.cs / VulkanCommandBuffer.cs / VulkanShaderModule.cs
      VulkanBuffer.cs / VulkanImage.cs / VulkanSampler.cs / VulkanSemaphore.cs / VulkanFence.cs
      TextureUploader.cs / MemoryAllocator.cs / ValidationLayer.cs
      HardwareCapabilityCheck.cs / HardwareCapabilityException.cs        // К-L19 fail-fast
      PhysicalDeviceInfo.cs / PhysicalDeviceType.cs / QueueFamilyInfo.cs

    Compute/                                  // V0 compute side + V1 primitive
      VulkanComputePipeline.cs / VulkanComputeDescriptors.cs / ComputeDispatch.cs
      ComputePipelineRegistry.cs / FieldStorageBinding.cs               // K9 RawTileField → SSBO
      V1DiffusionPipeline.cs / DiffusionPushConstants.cs                // V1 primitive (shipped)

    Sprite/
      Sprite.cs / AtlasRegion.cs / SpriteVertex.cs / SpriteTransform.cs / SpriteTexture.cs
      SpriteIndexBuffer.cs / VertexBufferRing.cs / SpriteDescriptorSetLayout.cs
      VulkanSpritePipeline.cs / SpriteRenderer.cs                       // batched Begin/Submit/End
      TileMap.cs / Camera2D.cs

    Assets/  PngDecoder.cs / PngImage.cs / PngChunk.cs / PngDecoderException.cs
             AssetManager.cs / AssetPath.cs
    Diagnostic/  ValidationLog.cs             // FrameTimer/DebugOverlay never shipped — see §1.1

  // ====== Presentation host (replaced the deleted DualFrontier.Presentation) ======
  DualFrontier.Launcher/
    DualFrontier.Launcher.csproj              // references Application + Runtime; copies Native.dll
    Program.cs                                // Main(): Runtime.Create + GameBootstrap + main loop
    LauncherRenderer.cs                       // IRenderer impl: drains PresentationBridge, records frame
    RenderCommandDispatcher.cs                // bridge commands → SceneState
    SceneState.cs                             // composition-root scene model
    LauncherProceduralAtlas.cs / PawnSpriteEntry.cs

tests/                                        // full census: DEVELOPMENT_HYGIENE §2
  DualFrontier.Runtime.Tests/                 // non-GPU + GPU-gated runtime tests (see §2.8)
  DualFrontier.Runtime.SmokeTest/             // manual GPU smoke executable (Program.cs + ProceduralAtlas.cs)

tools/
  shaders/                                    // GLSL sources — the complete current set:
    clearcolor.vert / clearcolor.frag         //   V0 foundation clear pass
    sprite.vert / sprite.frag                 //   sprite pipeline
    noop.comp                                 //   V0 empty-dispatch test shader
    diffusion.comp                            //   V1 substrate shader
                                              //   (wave.comp does not exist — V2 pending, see §1.3)
  glslangValidator.exe                        // committed Khronos compiler (build-time)
  scaffold-runtime.ps1                        // idempotent runtime-directory materializer

assets/
  shaders/                                    // compiled SPIR-V mirror of tools/shaders/ (six .spv files)
  kenney/ …                                   // art packs incl. kenney/terrain/roguelikeSheet_transparent.png
  cinzel/                                     // font asset pack (no runtime text renderer consumes it yet)

mods/                                         // strict-v3 skeletons + example — see MOD_OS_ARCHITECTURE
  DualFrontier.Mod.Example/
  DualFrontier.Mod.Vanilla.{Combat,Core,Inventory,Magic,Pawn,World}/
  Directory.Build.targets                     // manifest copy + hotReload Release rewrite (no shader builds)

native/
  DualFrontier.Core.Native/                   // C++ kernel (CMake) — includes compute_pipeline.cpp,
                                              //   compute_dispatch.cpp, tile_field.cpp, system_graph.cpp
```

Differences from the original post-migration plan, recorded for traceability: `SpriteBatcher`/`SpriteAtlas` materialized as `SpriteRenderer` + `SpriteTexture`/`AtlasRegion`; the `Text/` module, `fonts/` assets, `FrameTimer`, and `DebugOverlay` never shipped (Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md)); the adapter landed as `DualFrontier.Launcher`, not as a rewritten `DualFrontier.Presentation`; the planned `Vanilla.{Electricity,Water,Movement}` mods do not exist yet. The repository-level census authority is [DEVELOPMENT_HYGIENE §1–§2](../methodology/DEVELOPMENT_HYGIENE.md).

The scaffolding in `tools/scaffold-runtime.ps1` materialized the rendering hierarchy mechanically (commit `81fea13`); the `Compute/` submodule under `DualFrontier.Runtime/` is a Q-G-1 cascade addition (V0 compute side), filled in by the V0.B and V1 cascades.

### 2.2 Module purposes

#### `DualFrontier.Runtime` (top-level)

**Purpose:** Generic 2D Vulkan substrate — window management, Vulkan instance/device/queues, rendering primitives, batched sprite rendering, texture loading, input events, compute pipeline plumbing, scalar field compute primitives. Knows nothing of Domain gameplay.

**Public API surface:** `Runtime.cs` facade exposes (verified against `src/DualFrontier.Runtime/Runtime.cs`):

- `Runtime.Create(RuntimeOptions)` — full V0.A/B/C composition with fail-safe disposal and the К-L19 `HardwareCapabilityCheck.Verify` fail-fast.
- Window + input: `Window` (`IWindow`, message pump), `InputQueue` (`InputEventQueue`).
- Frame recording: `RecordSpritesFrame` (batched, `Camera2D`-driven — the production path), `BeginRenderPassForSprites`/`EndSpriteRenderPass` (multi-cycle), `RecordSpriteFrame` (single-sprite V0.C.1 backward-compat shim), `RecreateFramebuffersForSwapchain`.
- Compute: `ComputePipelines` (`ComputePipelineRegistry`), `CreateFieldStorageBinding(NativeWorld)` and `CreateV1DiffusionPipeline(...)` factories (V1-14).

**Dependencies:** `System` (BCL), `System.Numerics`, `System.IO.Compression`, plus exactly one project reference — `DualFrontier.Core.Interop` (native kernel bridge for `NativeWorld`/field compute binding; a V1 addition). No Domain gameplay projects, no third-party packages. Compare to the strict layering rules of [ARCHITECTURE](./ARCHITECTURE.md).

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

**Purpose:** Typed input events + event queue. Events posted by Window, consumed by clients via polling. Superseded the Godot `IInputSource` adapter ([VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) §Contracts, historical) at the rendering cutover. Note: the Launcher currently drains the queue without forwarding events into Domain — forwarding is Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).

**Public API surface:** `IInputEvent` + concrete event types + `VirtualKeyMapper` + `InputEventQueue` (the queue type lives in `Window/`).

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Graphics`

**Purpose:** Vulkan rendering primitives — instance, device, swapchain, render pass, pipeline, buffer, image, memory allocator. Direct wrappers around Vulkan API with idiomatic C# lifetimes (`IDisposable` patterns). Shares `VkInstance`/`VkDevice` with `Compute/`.

**Public API surface:** `VulkanInstance`, `VulkanDevice`, etc. — used by `Sprite`, `Text`, `Diagnostic`, and by `Compute/`.

**Dependencies:** `Native.Vulkan`, `Window` (for surface creation).

#### `DualFrontier.Runtime.Compute` (V0 compute side + V1; V2 pending)

**Purpose:** Vulkan compute primitives — compute pipeline, descriptor sets, dispatch, fence sync, K9 field storage binding. The shipped V1 diffusion primitive lives here; the V2 wave primitive lands here when authored (§1.3). Mod-driven shader registration is designed to flow through this module via `IModApi.ComputePipelines` (contract placeholder today — §3.3).

**Public API surface:** `VulkanComputePipeline`, `VulkanComputeDescriptors`, `ComputeDispatch`, `ComputePipelineRegistry`, `FieldStorageBinding`, `V1DiffusionPipeline`, `DiffusionPushConstants`.

**Dependencies:** `Graphics` (shared instance/device), `Native.Vulkan`, `DualFrontier.Core.Interop` (`NativeWorld` field storage).

#### `DualFrontier.Runtime.Sprite`

**Purpose:** 2D sprite rendering — atlas-based, batched draw calls. `Camera2D` for orthographic projection; `TileMap` for grid rendering.

**Public API surface:** `Sprite`, `AtlasRegion`, `SpriteTexture`, `SpriteRenderer` (batched `BeginFrame`/`Submit`/`EndFrame`), `TileMap`, `Camera2D`.

**Dependencies:** `Graphics`, `Assets`, `System.Numerics`.

#### Text rendering — not shipped

No `Text/` module exists on disk. Bitmap-font text rendering (design: glyph-atlas quads reusing the sprite pipeline) is rendering-scope work that did not ship with V0. Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md); the font-system design decision is held open in §8.2.

#### `DualFrontier.Runtime.Assets`

**Purpose:** Asset loading — manual PNG decoder, asset path resolution.

**Public API surface:** `PngDecoder`, `AssetManager`.

**Dependencies:** `System` (BCL), `System.IO.Compression`.

#### `DualFrontier.Runtime.Diagnostic`

**Purpose:** Debug tooling — validation log capture. Targets and budgets governed by [PERFORMANCE](./PERFORMANCE.md). `FrameTimer`/`DebugOverlay` never shipped (Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md)); FPS measurement currently lives in the smoke-test executable.

**Public API surface:** `ValidationLog`.

**Dependencies:** `Graphics`.

#### `DualFrontier.Launcher` (presentation host)

**Purpose:** Production entry point bridging Domain to Runtime. `Program.Main()` composes `Runtime.Create` + `GameBootstrap.CreateLoop(PresentationBridge)` + `SceneState` + `RenderCommandDispatcher` + `LauncherRenderer`, then drives the main loop (message pump → input drain → `LauncherRenderer.RenderFrame`). `LauncherRenderer` implements the Domain-side `IRenderer` contract (`DualFrontier.Application.Rendering`): it drains `PresentationBridge` per frame, dispatches commands through `RenderCommandDispatcher` into `SceneState`, and records the Vulkan frame via `Runtime.RecordSpritesFrame`. Owns «what to draw» logic. Formalized at К-extensions cascade #2; pawn visuals landed at К-extensions cascade #3 (commit `97f4573`).

**Public API surface:** `Program.Main()`; all other classes `internal`.

**Dependencies:** `DualFrontier.Application`, `DualFrontier.Runtime` (project references; the csproj also copies `DualFrontier.Core.Native.dll` from the canonical native build tree — [DEVELOPMENT_HYGIENE §3](../methodology/DEVELOPMENT_HYGIENE.md)).

### 2.3 Threading model

The substrate extends [THREADING](./THREADING.md) (the concurrency authority) — Domain tick scheduling is untouched by substrate work: the native kernel owns the scheduling architecture per К-L12 (`native/DualFrontier.Core.Native/src/system_graph.cpp`, with К-L13 runnable-subset wake semantics), and the managed `ParallelSystemScheduler` (`src/DualFrontier.Core/Scheduling/`) executes managed systems as the adapter facade above it. The substrate contributes a single render thread merged with the OS message pump (per L8) — in production this is the `DualFrontier.Launcher` main thread. Compute dispatches happen on the simulation thread through the substrate's typed dispatch surface (`V1DiffusionPipeline.ExecuteIteration` → native `df_world_field_dispatch_compute`), executed by the GPU on the async compute queue; the К-L7 sync path returns after the fence signals, so subsequent reads see consistent state.

#### 2.3.1 Pipeline depth and queue family roles (К-L16/L19/L7.1, К10.3 v2 amendment)

Sim thread coordinates с three Vulkan queues:
- **Graphics queue** — display rendering (existing — preserved verbatim).
- **Async compute queue** (К-L19 V0.B amendment) — К-L16 pipeline depth dispatches per Phase.Compute (К10.3 v2 Item 35 — see `native/DualFrontier.Core.Native/include/phase_compute.h`); V1 sync `dispatch_compute_field` also uses this queue.
- **Copy/transfer queue** (К-L19 V0.B amendment) — asset transfers (existing semantics).

Pipeline depth D=2 default (К-L16): sim thread allocates new slot at start of pipeline-managed tick; Phase.Compute dispatches к async compute queue; fence orchestration tracks slot transitions Empty→Dispatched→FenceCompleted→ReadableAsTail. K-L13 wake registry extended с slot transition counter (К10.3 v2 Item 37 — `WakeOnSlotTransitionAttribute` consumer surface, full subscriber registry integration deferred к К-extensions).

Sim-thread reads от slot tail (К-L7.1) для pipeline-managed fields — `df_pipeline_read_slot_tail(slot_offset = -1)` returns sim_tick - 1 results. К-L7 atomic-from-observer preserved within slot boundary; cross-slot reads see different snapshots.

V1's `dispatch_compute_field` sync path (К-L7 baseline): preserved unchanged; consumer call returns after fence signals. Orthogonal к pipeline depth per S-LOCK-13 coexistence.

```
┌──────────────────────────────────────────────────────────────┐
│ Process Threads (production composition — Launcher)          │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Main Thread (Launcher: Window + Render — MERGED)            │
│  ┌────────────────────────────────────────────────────┐      │
│  │ Win32 message pump (Window.PumpMessages)           │      │
│  │   → input event queue                              │      │
│  │   → window lifecycle (close, resize, focus)        │      │
│  │ InputQueue drain (Domain forwarding not yet wired) │      │
│  │ LauncherRenderer.RenderFrame                       │      │
│  │   → drain PresentationBridge                       │      │
│  │   → RenderCommandDispatcher → SceneState           │      │
│  │   → Runtime.RecordSpritesFrame (record + submit    │      │
│  │     + present; per-image semaphores + frame fence) │      │
│  │ ~60 FPS target                                     │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  Simulation Thread (GameLoop — self-ticking background       │
│  thread per Q-G-7 (d) hybrid orchestration)                  │
│  ┌────────────────────────────────────────────────────┐      │
│  │ GameLoop tick — 30 TPS fixed step                  │      │
│  │   scheduling: native system_graph per К-L12/К-L13; │      │
│  │   managed ParallelSystemScheduler = adapter facade │      │
│  │   (THREADING owns the full model)                  │      │
│  │   ↓ writes to PresentationBridge.Enqueue()         │      │
│  │   ↻ V1 compute dispatch (К-L7 sync path —          │      │
│  │     returns after fence signal)                    │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  GPU (asynchronous to CPU)                                   │
│  ┌────────────────────────────────────────────────────┐      │
│  │ Async compute queue executes dispatched shaders    │      │
│  │ Graphics queue executes recorded command buffers   │      │
│  │ Fences signal back to CPU on completion            │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  Worker Threads (native kernel thread pool + managed         │
│  phase parallelism — THREADING)                              │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

**Cross-thread synchronization:**

- `PresentationBridge` (`ConcurrentQueue`-backed command bridge, `src/DualFrontier.Application/Bridge/PresentationBridge.cs`) — the primary domain → render channel, preserved from the pre-cutover architecture.
- `InputEventQueue` (`Window/InputEventQueue.cs`) for render → domain input events. The Launcher drains it each frame; forwarding into Domain (input bridge) is not yet wired — Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).
- Pause coupling (design, not yet wired on the Vulkan path): main thread detects focus loss via Win32 `WM_KILLFOCUS`/`WM_SETFOCUS` and calls `loop.SetPaused(true)`. The pattern was proven on the Godot path at M8.10; `WindowFocusEvent` exists in `Input/`, but the Launcher does not couple it to the loop yet — Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).
- Compute dispatch: simulation-side code calls `V1DiffusionPipeline.ExecuteIteration(...)` → native `df_world_field_dispatch_compute` records and submits to the async compute queue; the К-L7 sync path returns after the fence signals, so a subsequent `FieldHandle<T>.ReadCell` sees the dispatched result. The opt-in К-L7.1 pipeline-managed path (bounded one-tick slot-tail lag) is specified in §2.3.1 and §7.3.0.

### 2.4 Dependency rules (locked invariants)

These rules are mechanically verifiable via the project reference graph. They mirror the dependency-direction discipline of [ARCHITECTURE §Four layers](./ARCHITECTURE.md).

**Rule 1.** `DualFrontier.Runtime` MUST compile and tests pass without any reference to Domain gameplay projects (`Core`, `Components`, `Systems`, `Events`, `Application`, …). Its only permitted project reference is `DualFrontier.Core.Interop` (the native kernel bridge — required for K9 field storage binding). Verified state: the csproj carries exactly that one reference.

**Rule 2.** Domain ↔ Runtime communication ONLY through the `DualFrontier.Launcher` presentation host (consumer chain: `PresentationBridge` drain → `RenderCommandDispatcher` → `SceneState` → `Runtime.RecordSpritesFrame`) and through the field/compute bridge (`DualFrontier.Core.Interop` + the `IModApi` v3 surface for mods). Domain knows nothing of Runtime; Runtime knows nothing of Domain gameplay.

**Rule 3.** Within Runtime, dependency direction respects layering:

```
Native.Win32 / Native.Vulkan  (lowest)
    ↓
Window / Input / Assets
    ↓
Graphics
    ↓
Compute  (shares Graphics's VkInstance/VkDevice; binds Core.Interop fields)
    ↓
Sprite
    ↓
Diagnostic
    ↓
Runtime.cs (facade — top)
```

(The planned `Text` layer slots between `Sprite` and `Diagnostic` when it ships — §2.2.)

**Rule 4.** No layer skipping (Diagnostic does not import Native.Vulkan directly; goes through Graphics).

**Rule 5.** Runtime exposes minimal public API. Internal implementation details `internal`. Naming follows [CODING_STANDARDS](../methodology/CODING_STANDARDS.md).

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
PngImage (RGBA bytes + width + height)
  ↓ VulkanImage.CreateFromPngImage (staging upload via TextureUploader)
VkImage + VkImageView
  ↓ SpriteTexture (image + sampler) + AtlasRegion rects
Sprite instances → SpriteRenderer.Submit (consumed by the Launcher's SceneState)
```

**Manual PNG decoder scope** (~500–700 lines):

- IHDR, IDAT, IEND chunks parsing
- DEFLATE decompression via `System.IO.Compression.DeflateStream` (BCL)
- Filter unfiltering (Sub/Up/Average/Paeth predictors)
- CRC32 verification
- RGBA8 output (other formats deferred)

### 2.7 Shader strategy

**Build-time SPIR-V compilation** via the `CompileShaders` MSBuild target in the root `Directory.Build.props` (runs `BeforeTargets="Build"`, conditioned on the `DualFrontier.Runtime` project; uses the committed `tools/glslangValidator.exe` so developer machines need no Vulkan SDK for shader compilation — [DEVELOPMENT_HYGIENE §4](../methodology/DEVELOPMENT_HYGIENE.md)). One toolchain compiles both graphics and compute shaders. Current compilation set (verified against `Directory.Build.props`): `clearcolor.vert`/`clearcolor.frag`, `noop.comp`, `sprite.vert`/`sprite.frag`, `diffusion.comp` — sources in `tools/shaders/`, `.spv` outputs in `assets/shaders/`. The V2 `wave.comp` line is added to this target when V2 ships (§1.3).

**Mod-side compute shader compilation** (design per К-L9 «vanilla = mods», not yet realized): each vanilla mod compiles its compute shader during the mod build, embeds the resulting `.spv` bytes into mod assets, and registers via the `IModApi.ComputePipelines` registration surface at mod startup — same toolchain, extended into mod projects. Today `mods/Directory.Build.targets` performs only manifest copy + Release `hotReload` rewrite (no shader builds), and the registration surface is a contract placeholder (§3.3). Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

**Production binary depends on:** `vulkan-1.dll` + pre-compiled `*.spv` files. No shader compiler dependency.

### 2.8 Testing strategy

The existing Domain tests are preserved verbatim — Domain layer is untouched (L10). [TESTING_STRATEGY](../methodology/TESTING_STRATEGY.md) governs test law and the census method; substrate additions slot in as test categories without altering the suite structure.

**Tests in `tests/DualFrontier.Runtime.Tests`** (realized census, verified 2026-06-12):

- Assets: `PngDecoderTests`, `AssetManagerTests` — synthetic PNG inputs → expected RGBA output; path smart-resolve.
- Sprite: `SpriteRendererTests`, `SpriteVertexTests`, `SpriteIndexBufferTests`, `AtlasRegionTests`, `Camera2DTests` — batching, vertex math, orthographic projection, screen ↔ world conversion.
- Input: `VirtualKeyMapperTests`.
- Graphics: marshalling/layout suites (`VulkanInstanceMarshallingTests`, `VulkanDeviceMarshallingTests`, …), `HardwareCapabilityCheckTests`, `AsyncComputeQueueSelectionTests`, `ShaderCompilationTests`, plus per-wrapper lifecycle suites.
- Compute: `ComputePipelineRegistrationTests` (round-trip register/dispatch), `FieldStorageBindingTests`, `DiffusionPushConstantsTests`, `V1DiffusionFactoryTests`, `V1DiffusionIntegrationTests`, and `V1DiffusionEquivalenceTests` — CPU oracle (`IsotropicDiffusionKernel`/`AnisotropicDiffusionKernel` in `DualFrontier.Core.Interop.CpuKernels`) vs GPU output on synthetic grids, tolerance-bounded. The V2 equivalence suite lands with V2 (§1.3).
- `RuntimeCompositionTests` — facade composition.

**GPU-dependent verification:** the `tests/DualFrontier.Runtime.SmokeTest` executable (stress + tile-map scenes) plus manual visual verification per the committed protocols (V0.C.2 protocol, commit `f6ff03b`; V1 protocol, commit `94335eb`) — continuing the M8.8/M8.9 visual-verification practice. Validation layer output captured to console — clean output is the success criterion (§11 pre-flight check; this document owns that check).

### 2.9 Naming conventions

Continued from [CODING_STANDARDS](../methodology/CODING_STANDARDS.md):

- All identifiers English (Russian glossary unchanged — see [TRANSLATION_GLOSSARY](../TRANSLATION_GLOSSARY.md)).
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

Per К-L9 (vanilla = mods), compute shaders are owned by mods, not the engine — this is the design allocation: `Vanilla.Magic` owns `ManaField` + diffusion shader (V1 configuration), `Vanilla.Electricity` owns `PowerField` + anisotropic diffusion shader (V1 configuration), `Vanilla.Movement` owns flow field configurations on V2 (routed paths to destinations). Third-party mods extend through the same registration API.

**Status:** design contract, not yet wired. On disk today: `Vanilla.Magic` is a strict-v3 skeleton with an empty `Initialize`; `Vanilla.Electricity`/`Vanilla.Movement` do not exist; the production `IModApi.ComputePipelines` implementation returns `null` (placeholder per `IModComputePipelineApi`); the V1 diffusion pipeline is constructed engine-side via the `Runtime` factories (§2.2). Mod-facing wiring is Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

Build pipeline (§2.7) already compiles GLSL to SPIR-V via `glslangValidator.exe` for engine shaders; mod builds extend the same toolchain when the registration surface lands.

Mod startup code (illustrative design sketch — exact API per `IModApi` v3 surface in [MOD_OS_ARCHITECTURE §4.6](./MOD_OS_ARCHITECTURE.md); the `ComputePipelines`/`Systems.RegisterFieldUpdate` calls shown do not exist yet):

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

Managed bridge (`DualFrontier.Core.Interop`) wraps these into typed APIs — shipped shape (verified against `FieldRegistry.cs` / `FieldHandle.cs`):

```csharp
public sealed class FieldRegistry   // reached via NativeWorld.Fields
{
    public FieldHandle<T> Register<T>(string id, int width, int height) where T : unmanaged;
    public FieldHandle<T> Get<T>(string id) where T : unmanaged;
    public bool TryGet<T>(string id, out FieldHandle<T>? handle) where T : unmanaged;
    public bool IsRegistered(string id);
    public void Unregister(string id);
}

public sealed class FieldHandle<T> : IFieldHandle where T : unmanaged
{
    public T ReadCell(int x, int y);
    public void WriteCell(int x, int y, T value);
    public FieldSpanLease<T> AcquireSpan();          // ref-struct lease over the dense span
    public void SetConductivity(int x, int y, float value);
    public float GetConductivity(int x, int y);
    public void SetStorageFlag(int x, int y, bool enabled);
    public bool GetStorageFlag(int x, int y);
    public void SwapBuffers();                       // ping-pong advance
}
```

Compute pipeline registration and dispatch sit one layer up, in the substrate (`DualFrontier.Runtime.Compute`): `FieldStorageBinding.Register` wraps `df_world_register_compute_pipeline`, and `V1DiffusionPipeline.ExecuteIteration` wraps `df_world_field_dispatch_compute` with a typed `DiffusionPushConstants` payload (§2.2 Runtime factories).

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

## 4. Rendering use case (V0 rendering side — realized record)

The rendering use case rebuilt presentation functionality from Godot 4 onto the V substrate. The Godot path ran in parallel until cutover; Godot was deleted at К-extensions cascade #2 (2026-05-22/23) with the Launcher formalized as the production renderer (infrastructure-only per Q-G-6 (b1)), and real pawn visuals landed at К-extensions cascade #3 (2026-05-23, commit `97f4573`). Full M8.x UI parity (text, HUD panels) was **not** reached before deletion and remains pending — Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md).

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

### 4.1 Migration approach (completed)

**Strategy: parallel development — executed as designed.**

`DualFrontier.Presentation` (Godot) was kept functional through the rendering cutover phase (formerly M9.5). The Runtime project developed in parallel; the cutover replaced Presentation with the `DualFrontier.Launcher` host on Runtime; the final deletion phase removed the Godot remnants (К-extensions cascade #2, commit `2ba8130`).

**Operating principle:** «honest state always available». No blind period where the game did not run — every phase ended with a runnable build, the same discipline as the parallel mod-system migration ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) migration discipline).

### 4.2 Rendering use case implementation phases (realized record)

The rendering side was designed as a sequence of sub-phases within V substrate work. Pre-Q-G-1 these were labeled M9.0..M9.8 runtime milestones; post-Q-G-1 they are V substrate rendering use case implementation phases (V0 rendering side). Phase IDs are preserved historically in commits and `docs/MIGRATION_PROGRESS.md` closure entries; in practice the work shipped as the V0.A/V0.B/V0.C.1/V0.C.2 sub-milestones plus the К-extensions cascades #2/#3. The R.x labels are descriptive identifiers within V0 work — not formal namespace bucket assignments; they introduce no additional V-numbered substrate primitives.

Design decomposition and realized outcome (status detail authority: [ROADMAP §Native foundation tracks](../ROADMAP.md)):

| Phase | Design goal                                          | Realized — evidence |
| ----- | ---------------------------------------------------- | ------------------- |
| R.0   | Foundation: Win32 window + Vulkan clear color        | ✅ V0.A closure 2026-05-18 (+ V0.B infrastructure 2026-05-18) — `clearcolor.*` shaders, `Runtime.Create` V0.A/V0.B composition |
| R.1   | First textured quad: PNG → VkImage → sprite render   | ✅ V0.C.1 closure 2026-05-19 — `PngDecoder` + `TextureUploader` + `VulkanSpritePipeline`; smoke 820 frames @ 164 FPS |
| R.2   | Batched sprite renderer (10K sprites, 60+ FPS)       | ✅ V0.C.2 closure 2026-05-19 — batched `SpriteRenderer` (commit `18e6f8e`), 10K stress scene (commit `01d9c1c`) |
| R.3   | TileMap parity + Camera2D                            | ✅ V0.C.2 closure 2026-05-19 — `TileMap` (commit `b72cd7e`), `Camera2D` (commit `bd2c8eb`), 200×200 smoke scene (commit `655e6c0`) |
| R.4   | Input system (Win32 → InputEventQueue)               | ◐ Infrastructure shipped at V0.C.1 (event types + queue + Win32 dispatch); Domain forwarding not wired — Launcher drains and discards. Planned — ROADMAP |
| R.5   | Domain integration (Presentation port)               | ✅ Realized in altered form: not a Presentation rewrite but the `DualFrontier.Launcher` host — formalized at К-ext #2, pawn visuals at К-ext #3 (commit `97f4573`; pawn-3 dispatch arms real, 3 arms deferred silent stubs) |
| R.6   | UI primitives (text + panels)                        | ✗ Not shipped — no `Text/` module, no panel primitives on disk. Planned — ROADMAP |
| R.7   | Coupled lifecycle + DebugOverlay                     | ◐ Partial: swapchain/framebuffer recreation support shipped (`Runtime.RecreateFramebuffersForSwapchain`); focus→pause coupling and `DebugOverlay` not wired/shipped. Planned — ROADMAP |
| R.8   | Migration cutover (delete Godot)                     | ✅ К-extensions cascade #2 (commit `2ba8130`, 2026-05-22; cascade closed 2026-05-23) — tracked Godot files removed, pure Vulkan/.NET stack. Residual: root `project.godot` kept deliberately (Crystalka-owned — [DEVELOPMENT_HYGIENE §7 (Godot status)](../methodology/DEVELOPMENT_HYGIENE.md); ROADMAP F-5) |

Corrections to the original phase definitions, recorded for traceability:

- The R.8 deliverable list cited a `tools/build-all.ps1` script that never existed; the real build wiring is the `CompileShaders` MSBuild target in the root `Directory.Build.props` plus the verified build commands in [DEVELOPMENT_HYGIENE §3](../methodology/DEVELOPMENT_HYGIENE.md).
- The R.8 «`grep -r godot` returns empty» success criterion is not met literally: the root `project.godot` file and `*.import` residues under `assets/` survive deliberately (Crystalka-owned residual; see above).
- R.3's «full M8.8 visual parity» and R.5's «full M8.9 visual parity» were met at the sprite/tile-map level; HUD-level parity belongs to the unshipped R.6 scope.

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

**Per-agent A* (current Dual Frontier approach via `IPathfindingService` / `AStarPathfinding` in `DualFrontier.AI`):**

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

### 5.4 Engine vs mod placement (design allocation)

**V substrate provides infrastructure:**

- Field types as `RawTileField<T>` (K9) bound to V0 compute pipeline — shipped
- V1 diffusion shader template (isotropic + anisotropic via per-cell D) — shipped
- V2 wave shader template (routed + distance/direction extraction) — pending (§1.3)
- Compute dispatch + fence sync + read-cell point queries — shipped

**Mods provide gameplay** (design allocation — none of these mods carry content yet; Planned, see [ROADMAP §Native foundation tracks](../ROADMAP.md)):

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

No special-case visibility mechanism — К-L17 composition framework handles latency separation uniformly across navigation modes.

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

## 6. Status record (realized / pending)

Forward state, sequencing, and gating for everything below are owned by [ROADMAP §Native foundation tracks (V substrate)](../ROADMAP.md) — the single status authority. This section is an evidence-marked record, not a roadmap.

### 6.1 V substrate primitive record

| Primitive | Title                                                                | State — evidence |
| --------- | -------------------------------------------------------------------- | ---------------- |
| V0        | Vulkan substrate foundation (rendering + compute plumbing)           | ✅ Realized — V0.A/V0.B closures 2026-05-18, V0.C.1/V0.C.2 closures 2026-05-19; V0 substrate close 2026-05-19 per Q8 ratification (`docs/MIGRATION_PROGRESS.md` V-series table). Text/overlay scope excluded — §1.1 |
| V1        | Scalar field + diffusion shader (isotropic + anisotropic)            | ✅ Realized — closure 2026-05-19, PR #40 merge `88aebf2`; `V1DiffusionPipeline.cs` + `diffusion.comp(.spv)` on disk — §1.2 |
| V2        | Scalar field + wave shader (routed, breakable, distance/direction)   | ⏭ Pending — no `wave.comp` on disk; design rationale in §1.3 |
| V close   | Multi-field coexistence acceptance criterion (former G4)             | ⏭ Pending — gated on V2; criteria tracked in ROADMAP (§1.4) |

Future V-N primitive identifiers remain reserved (G5 Domain B disposition, G9 eikonal upgrade if evidence justifies, modder-driven primitives) — §1.3.1/§1.3.2.

### 6.2 Rendering use case implementation phases (R.0..R.8)

Realized record in §4.2. Sub-phases of V0 work; they introduce no additional V-numbered primitives.

### 6.3 M-V demonstrations record (per Q-R-1 format)

Per Q-R-1 LOCK and Q-V-2 LOCK, mods that demonstrate V substrate primitives carry `M-V{original G number}` identifiers preserving traceability to G-skeleton briefs; multi-substrate mods carry the compound `M-K{N} / M-V` marker with the V-side identifier assigned at V-side authoring time (FHE-style reserved pattern).

All M-V demonstrations are **pending** — the vanilla mods on disk are strict-v3 skeletons with empty `Initialize` bodies, and `Vanilla.Electricity`/`Vanilla.Water`/`Vanilla.Movement` do not exist yet. One line per item (evidence = current disk state; status detail in ROADMAP):

- M-V1 Vanilla.Magic mana diffusion (V1 isotropic) — pending; `mods/DualFrontier.Mod.Vanilla.Magic` skeleton only.
- M-V2 Vanilla.Electricity power field (V1 anisotropic) — pending; mod absent.
- M-V5 Vanilla.Combat projectile Domain B reactivation — deferred; substrate disposition TBD (§1.3.2).
- M-V7 Vanilla.Movement routed flow field (V2) — pending; mod absent, gates on V2.
- M-V8 Vanilla.Movement local avoidance (mod-level, not substrate) — pending; gates on M-V7.
- M-V3 / M-V4 / M-V6 / M-V9 — identifier gaps from the Q-G-2 reductions; slots reserved, unused.

### 6.4 Multi-substrate vanilla mod markers (Q-V-2 LOCK)

Compound `M-K{N} / M-V` marker assignments and the K-only mod list are tracked in [ROADMAP §Mod-OS Migration and §Native foundation tracks](../ROADMAP.md). The design rule recorded here: K-side milestone authored first; V-side authored after the V substrate is ready; the concrete V-side identifier appears at V-side authoring time (Q-V-2 precedent, `IHomomorphicComputeProvider` model).

### 6.5 Hybrid coupling spec (deferred TBD)

How V1 diffusion picks up from a broken V2 wave node — example: water in pipes propagates via V2 wave respecting pipe topology; on pipe break, water diffuses ambient via V1. Coupling spec deferred to V substrate amendment authoring (deliberation §3.2 deferred-to-amendment item); the M-V Water demonstration is the first integration point requiring resolution. Held in §8.2 open decisions.

### 6.6 Sequencing

Owned by [ROADMAP §Native foundation tracks](../ROADMAP.md) (combined K/V timeline and cross-track gating).

---

## 7. Failure modes and fallbacks

### 7.1 Hardware capability policy and CPU reference kernels

Not all hardware supports Vulkan 1.3 compute reliably; pure software environments (CI, headless build agents) may lack GPU access entirely. The shipped policy resolves this by **exclusion, not fallback**:

- **К-L19 fail-fast (shipped):** `Runtime.Create` runs `HardwareCapabilityCheck.Verify` at startup and throws `HardwareCapabilityException` if the Vulkan 1.3 + async-compute-queue hardware tier is absent. The hardware-tier exclusion is an accepted architectural choice — `KERNEL_ARCHITECTURE Part 0 (К-L invariants)`, К-L19 row.
- **CPU reference kernels (shipped, test-oracle role):** each shipped compute shader has a managed CPU reference implementation — `IsotropicDiffusionKernel` + `AnisotropicDiffusionKernel` in `src/DualFrontier.Core.Interop/CpuKernels/` — used by the V1 equivalence suites (§2.8) and runnable without any GPU (this is how field logic is exercised on CI).
- **Runtime CPU-fallback dispatch (design option, not wired):** a config-selected per-tick CPU execution path for fields exists only as design; no fallback dispatcher is on disk. Reopening it (e.g. for a broader hardware audience) is a hardware-tier-expansion concern — see [ROADMAP §Hardware tier expansion cascade](../ROADMAP.md).

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

Design mitigation (not implemented — no field save path exists yet): the CPU reference kernels (§7.1) produce canonical state for save snapshots — save pauses GPU dispatch, runs one CPU iteration to produce canonical field state, serializes that; on load, fields restore from canonical state and GPU dispatch resumes. Integration with the persistence layer is Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

For hobby-scale single-player, slight non-determinism between sessions is acceptable.

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

**R1 — Pure P/Invoke binding tedium exceeds tolerance.** *Resolved moot post-V0:* the full V0 foundation shipped on hand-written `[LibraryImport]` bindings (`Native/Vulkan/VkApi.cs`, `Native/Win32/Win32Api.cs`) without switching to a binding library; the Vortice.Vulkan lateral-move option was never needed.

**R2 — Vulkan complexity bugs (synchronization, layout transitions, compute fence sync).**

- Probability: High.
- Mitigation: validation layers ALWAYS on in development (DEBUG default per `RuntimeOptions`). RenderDoc for visual debugging. Validation-clean output is a pre-commit check owned by §11 of this document. Compute fence-sync invariants explicitly tested with controlled dispatch sequences.

**R3 — PNG decoder edge cases.**

- Probability: Medium.
- Mitigation: extensive test suite with synthetic + real PNG inputs ([TESTING_STRATEGY](../methodology/TESTING_STRATEGY.md) unit tier; realized — `PngDecoderTests`).

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

**Required tooling (in use since V0):**

- Vulkan SDK (LunarG, 1.3.x) — for development/debugging; shader compilation itself uses the committed `tools/glslangValidator.exe` and needs no SDK install ([DEVELOPMENT_HYGIENE §4](../methodology/DEVELOPMENT_HYGIENE.md)).
- RenderDoc (graphics debugger; compute debugging via NVIDIA Nsight or similar).
- Visual Studio 2022 17.8+ (for `[LibraryImport]` source generators).

**Optional:**

- BMFont — bitmap font generator (R.6).
- AssetForge / TexturePacker — atlas tooling (future).

The scaffolding generator `tools/scaffold-runtime.ps1` is committed and idempotent; running it materializes the §2.1 hierarchy without touching Domain. The `Compute/` subdirectory extension is a Q-G-1 cascade addition.

---

## 11. Methodology adjustments for V substrate work

The existing methodology ([METHODOLOGY](../methodology/METHODOLOGY.md)) carries forward with the following adjustments. None of these adjustments alter the pipeline shape; they extend its pre-flight + verification stages for the Vulkan-specific failure modes (both rendering and compute).

**Pre-flight checks adapted:**

- Write-conflict table — applies to Domain commits, not Runtime.
- Project reference direction sanity check — extended: Runtime may not reference Domain gameplay projects (§2.4 Rule 1).
- **Validation layer output check** — clean validation output mandatory before commit. This document owns the check; it is exercised through the smoke-test runs and the committed manual verification protocols (§2.8).
- **CPU/GPU equivalence test** — every new compute shader must have a CPU reference implementation; before commit, verify equivalence on representative inputs within tolerance. Realized for V1 (`V1DiffusionEquivalenceTests` + `CpuKernels`); binds V2 when it ships.

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

This document is authoritative until amended via explicit decision; its current version is owned by the document register (frontmatter mirror). Amendments require a commit with rationale, recorded in the version-history block, in the same style as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md).

---

## See also

- [METHODOLOGY](../methodology/METHODOLOGY.md) — the development pipeline; the V substrate adjustments in §11 keep this architecture inside the same methodology.
- [CODING_STANDARDS](../methodology/CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, member order; V substrate adheres verbatim.
- [ARCHITECTURE](./ARCHITECTURE.md) — the layer overview; V substrate is the presentation/compute foundation consumed by `DualFrontier.Launcher`, with compute primitives reachable from Domain via the `Core.Interop` bridge and (when wired) `IModApi`.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — companion architectural authority for the modding subsystem; `IModApi.Fields` + `IModApi.ComputePipelines` are the designed compute consumption surface (§3.3 status).
- [KERNEL_ARCHITECTURE](./KERNEL_ARCHITECTURE.md) — native ECS kernel; K9 `RawTileField<T>` is the storage primitive V substrate compute consumes; Part 0 owns the К-L invariants cited here (К-L7/L7.1, К-L9, К-L12/L13, К-L15–К-L19).
- [FIELDS](./FIELDS.md) — field storage contract; consumed by V substrate compute (V1 binds `RawTileField<T>` as SSBO; V2 follows the same binding when authored).
- [THREADING](./THREADING.md) — concurrency authority: native scheduler (К-L12/К-L13) + managed adapter facade; the Window+Render thread merge in §2.3 is the substrate's only addition. V1 compute dispatch is К-L7 sync on the simulation thread.
- [VISUAL_ENGINE](./historical/VISUAL_ENGINE.md) — **historical**: the retired dual-backend (Godot DevKit + Silk.NET Native) spec; superseded for production by this document at the rendering cutover.
- [GODOT_INTEGRATION](./historical/GODOT_INTEGRATION.md) — **historical**: the retired Godot glue spec (the `PresentationBridge` pattern it described lives on in `DualFrontier.Application.Bridge`); deprecated at the R.8 cutover.
- [MIGRATION_PLAN_KERNEL_TO_VANILLA](./MIGRATION_PLAN_KERNEL_TO_VANILLA.md) — planning record for Phase A K-series + Phase B M-cycle sequencing; forward state lives in ROADMAP.
- [ROADMAP](../ROADMAP.md) — **the single forward-state authority**: V substrate status, sequencing, gating, and the Findings ledger.
- [TESTING_STRATEGY](../methodology/TESTING_STRATEGY.md) — test law and census method; §2.8 slots V substrate tests into the existing structure.
- [DEVELOPMENT_HYGIENE](../methodology/DEVELOPMENT_HYGIENE.md) — operational truth: repository map, verified build commands (incl. the native kernel build the Launcher depends on), tooling reality, Godot end-state.
- [PERFORMANCE](./PERFORMANCE.md) — target metrics; sprite/tile budgets adopted with the batched renderer and TileMap work; field compute budgets adopted in V1 (V2 pending).
- [COMPOSITE_NAMESPACE_DELIBERATION_STATE](./COMPOSITE_NAMESPACE_DELIBERATION_STATE.md) — ratification authority for Q-G-1 + Q-G-2 LOCK (substrate consolidation and primitive reductions).
- [CPP_KERNEL_BRANCH_REPORT](../reports/CPP_KERNEL_BRANCH_REPORT.md) — Discovery report establishing K0 cherry-pick scope.

## Part 12: Relationship to KERNEL_ARCHITECTURE.md

VULKAN_SUBSTRATE.md (this) and KERNEL_ARCHITECTURE.md describe two halves of a single architectural vision: native foundation under managed Application layer. K substrate (kernel) and V substrate (Vulkan) are independent layers reachable from managed Application layer through respective bridges; the two-substrate symmetry was implicit in the prior RUNTIME + GPU_COMPUTE split and is made explicit by the Q-G-1 unification.

**Symmetric architecture**:
- This document (V substrate): Vulkan rendering + compute layer; substrate primitives V0/V1/V2; rendering use case implementation R.0..R.8; M-V demonstrations
- KERNEL_ARCHITECTURE.md: native ECS kernel layer; substrate K0..K9; M-K demonstrations (vanilla mods on K substrate)
- Both: pure P/Invoke to native (`vulkan-1.dll` for V; `DualFrontier.Core.Native.dll` for K)
- Both: managed thin adapter layer
- Both: single ownership boundary, direction-disciplined (managed → native)

**Independent layers**: rendering knows nothing about ECS storage; ECS kernel knows nothing about Vulkan. Both reachable from managed Application layer through respective bridges. Compute use case of V bridges to K through `RawTileField<T>` (K9 storage primitive) — the storage is in K, the compute is in V, the binding is `FieldStorageBinding.cs` in V0.

**Combined sequencing**: K-series and V-substrate sequencing both live in [ROADMAP §Native foundation tracks](../ROADMAP.md) (the former KERNEL_ARCHITECTURE Part 2 roadmap content was relocated there); this document §6 is the V-side evidence record. The parallel-tracks design held in practice: V work proceeded alongside the К-series, with the integration point at K9 + V0 (field storage abstraction + Vulkan compute plumbing) — realized as `FieldStorageBinding` over `RawTileField<T>`.

**Cross-document invariants**: «без компромиссов», operating principle (data exists/doesn't), single ownership boundary, direction-discipline, long-horizon planning. See `KERNEL_ARCHITECTURE Part 8 (cross-document invariants)` for the full list.

**LOCKED** — supersedes prior `RUNTIME_ARCHITECTURE.md` v1.0 + `GPU_COMPUTE.md` v2.0 (supersession record; current version owned by the register). Departures require an explicit re-architecture milestone and updates to dependent K9/V-series briefs.