---
register_id: DOC-A-VULKAN_SUBSTRATE_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.2
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: Vulkan Substrate (V) (authored rework; string-id ABI corrected, device-lost fenced open)
supersedes:
- DOC-A-VULKAN_SUBSTRATE
last_modified_commit: 5d71a8e
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'Post-merge Codex-review PATCH (operator-sanctioned): the section-2.3 swapchain-transaction fence and the section-6.3 device-lost fence no longer gate on ELT ratification (ratified law, EVT-2026-07-17-DRAFTS_RATIFICATION) - remaining gates are the deferred substrate amendment and the OQ-3 decision. PATCH 1.0.1 to 1.0.2.'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Successor of DOC-A-VULKAN_SUBSTRATE per EVT-2026-07-15-CORPUS_REWORK_R3_SUBSTRATE; predecessor supersession chain (G-series/GODOT/VISUAL_ENGINE) untouched on the historical entry.
---

# Vulkan Substrate (V)

The architectural authority for Dual Frontier's unified Vulkan 1.3 layer — one `VkInstance` / `VkDevice` / `vulkan-1.dll` linkage serving two use cases (2D rendering and field compute) — covering device and queue policy, swapchain and presentation, compute pipelines, pipeline slots, synchronization, and the GPU-side failure-mode law.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/VULKAN_SUBSTRATE.md` (DOC-A-VULKAN_SUBSTRATE, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`.

| Status field | Value |
|---|---|
| **Role** | normative (ratified successor) |
| **Successor of** | `docs/architecture/historical/VULKAN_SUBSTRATE.md` (DOC-A-VULKAN_SUBSTRATE, LOCKED v1.2.0) |
| **Scope** | The V substrate: Vulkan instance/device/queue policy (К-L19 hardware mandate), Win32 window + input event surface, swapchain + recreation, render pipeline + batched sprite path, compute pipeline plumbing + the V1 diffusion primitive, pipeline slots (К-L16/К-L7.1 GPU side), GPU synchronization and visibility law, mod-facing GPU surface status, GPU failure modes, GPU-side save-boundary behavior. Everything in §§0–7 is current truth unless inside a FENCED block. |
| **Non-goals** | Field storage layout and span law (FIELDS.md); К-L invariant canon text (KERNEL_ARCHITECTURE.md Part 0); the process-wide threading model (THREADING.md); mod lifecycle, manifests, capabilities (MOD_OS_ARCHITECTURE.md); scheduler phases and dispatch authority (SCHEDULER_ARCHITECTURE.md); game-design content; forward sequencing (docs/ROADMAP.md). |
| **Authority domains** | **gpu** — GPU device selection and queue policy, swapchain lifetime, graphics/compute pipeline objects, shader toolchain, GPU memory objects, pipeline-slot GPU machinery, GPU sync primitives, GPU failure-mode policy. |
| **Defers to** | [FIELDS.md](./FIELDS.md) → storage/memory (field layout, span lease, mutation rejection, string-id law) · [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) Part 0 → invariant canon (К-L7/К-L7.1, К-L9, К-L16, К-L17, К-L18, К-L19) · [THREADING.md](./THREADING.md) → threading (thread census, scheduler execution, happens-before) · [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) → mod-lifecycle (unload chain, capability grammar) · [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md) → scheduling (Phase.Compute's place in the native tick) · [ROADMAP](../ROADMAP.md) → all forward state. |

---

## §0 Hardware mandate and foundation locks

### 0.1 К-L19: Vulkan 1.3 + async-compute queue family, fail-fast

The V substrate runs on exactly one hardware tier and refuses to start on anything below it. Per К-L19 (KERNEL_ARCHITECTURE.md Part 0), the mandate is **Vulkan 1.3 API + a compute-capable queue family**, and the enforcement is **fail-fast at startup, not graceful degradation**. Two shipped enforcement sites, both verified at HEAD `35364c2`:

1. **Instance-level version gate** — `VulkanInstance` verifies `vkEnumerateInstanceVersion` ≥ `VK_API_VERSION_1_3` before creating the instance and throws with a driver-upgrade diagnostic otherwise (`src/DualFrontier.Runtime/Graphics/VulkanInstance.cs:26-40`).
2. **Composition-level capability gate** — `Runtime.Create` invokes `HardwareCapabilityCheck.Verify` immediately after `VulkanDevice` construction (`src/DualFrontier.Runtime/Runtime.cs:82-83`); it re-checks the 1.3 version (`src/DualFrontier.Runtime/Graphics/HardwareCapabilityCheck.cs:23-33`) and requires a resolved async-compute queue family index (`HardwareCapabilityCheck.cs:36-47`), throwing `HardwareCapabilityException` with the named hardware tier (NVIDIA Turing+, AMD RDNA 1+, Intel Arc Alchemist+) when absent. No further substrate composition occurs after a rejection.

The hardware-tier exclusion is an accepted architectural choice, not an oversight: the corresponding CPU execution story is an equivalence *oracle*, never a runtime fallback (§6). Revisiting the tier floor is an audience-driven decision tracked in [ROADMAP §Hardware tier expansion cascade](../ROADMAP.md); nothing in this document creates a fallback obligation.

One additional startup gate ships beyond К-L19's letter: after surface creation, `Runtime.Create` validates that the selected graphics queue family can actually present to the window surface (`vkGetPhysicalDeviceSurfaceSupportKHR`) and fails fast with a diagnostic if not (`Runtime.cs:95-111`, recorded as F06). Dual Frontier currently requires a graphics family that also presents; separate present-family *selection* is a recorded follow-up (§8).

### 0.2 Foundation locks (L1–L10)

The following decisions are committed foundation. Departure requires an explicit re-architecture milestone, not spec-level adjustment mid-implementation.

| # | Decision | Choice | Status note (2026-07-15) |
|---|---|---|---|
| L1 | GPU API | Vulkan 1.3 + async-compute queue family mandate (К-L19; consumed by К-L16 pipeline depth) | Enforced at two sites — §0.1 |
| L2 | Vulkan bindings | Pure P/Invoke to `vulkan-1.dll` (`[LibraryImport]`), no third-party C# binding library | Shipped — 86 `LibraryImport` declarations in `Native/Vulkan/VkApi.cs` |
| L3 | Window/OS surface | Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`) | Shipped — 15 `LibraryImport` declarations in `Native/Win32/Win32Api.cs` |
| L4 | Math | `System.Numerics` (BCL only) | Shipped |
| L5 | PNG loading | Manual decoder + `System.IO.Compression.DeflateStream` (BCL) | Shipped — `Assets/PngDecoder.cs` |
| L6 | Shader strategy | Build-time GLSL → SPIR-V via committed `tools/glslangValidator.exe`, both graphics and compute | Shipped — §1.4 |
| L7 | Initial platform | Windows-only (matches the К-L19 tier baseline) | In force; cross-platform is an open decision (§8) |
| L8 | Threading | Window+render thread merged; simulation thread preserved | In force — §2.4 |
| L9 | Migration approach | Parallel dual-backend until rendering cutover | **Completed and fully retired**: cutover 2026-05-23 (К-extensions cascade #2); the remaining inert Godot file surface (including the root `project.godot`) was deleted in the Godot Eradication Cascade, 2026-06-29/30 (F-5 CLOSED). No Godot artifact exists at HEAD. |
| L10 | Domain layer treatment | Preserved verbatim — zero modification by substrate work | Held; §1.3 Rule 1 is the mechanical check |

**Philosophy.** The substrate is total-ownership («без компромиссов»): every line above the OS API surface is project code. The production binary depends on `vulkan-1.dll` (GPU driver) and pre-compiled `.spv` files — no shader compiler, no binding library, no windowing library. The counterweight discipline is *features only on demand*: the Vulkan API surface is enormous, and every substrate feature must trace to a specific Domain requirement or gameplay mechanic; the substrate is not an engine-building exercise.

**Consolidation rationale (carried).** Rendering and compute share one physical Vulkan device; treating them as two substrates was documentation drift, resolved by the Q-G-1/Q-G-2 ratifications that produced the predecessor. The compute side reduces to three primitives — V0 plumbing, V1 diffusion, V2 wave — once gameplay mechanics are recognized as *configurations* of physical primitives rather than primitives in their own right. That reduction remains the load-bearing insight of this layer: distribution networks, navigation, and crowd behavior are all V1/V2 configurations (§4), so the substrate stays small while the gameplay surface stays expressive.

---

## §1 Shipped surface census

Everything in this section is evidence-marked current truth at HEAD `35364c2`. Forward state, sequencing, and gating live in [ROADMAP §Native foundation tracks](../ROADMAP.md) — the single forward-state authority. Verification-evidence rows for К-L-bound claims are tracked in the К-L14 evidence dashboard, not here.

### 1.1 Primitive census

| Primitive | Title | State — evidence |
|---|---|---|
| **V0** | Vulkan substrate foundation (rendering + compute plumbing) | **Shipped.** Sub-closures V0.A/V0.B 2026-05-18, V0.C.1/V0.C.2 2026-05-19; V0 substrate close 2026-05-19 per Q8 ratification (chronicle: `docs/MIGRATION_PROGRESS.md`, V-series table). Text/overlay scope was explicitly excluded from V0 — §1.6. |
| **V1** | Scalar field + diffusion shader (isotropic + anisotropic in one template) | **Shipped.** Closure 2026-05-19 (PR #40, recorded merge `88aebf2`). On disk: `tools/shaders/diffusion.comp` + `assets/shaders/diffusion.comp.spv`, `src/DualFrontier.Runtime/Compute/V1DiffusionPipeline.cs`, `DiffusionPushConstants.cs`, `Runtime.CreateFieldStorageBinding` / `Runtime.CreateV1DiffusionPipeline` factories (`Runtime.cs:392,416`), CPU oracle kernels (§6.2). |
| **V2** | Scalar field + wave shader (routed, breakable, distance/direction side products) | **Not on disk.** `tools/shaders/` contains no `wave.comp`; `assets/shaders/` contains no `wave.comp.spv`. Design rationale preserved in §4.4 (FENCED). |
| V close | Multi-field coexistence acceptance criterion | Pending — gated on V2; criteria tracked in ROADMAP. |

Two orthogonal truths complete the census honestly:

- **V1 has no production consumer today.** The only callers of the V1 factories are the smoke-test executable and the Runtime test suite (`tests/DualFrontier.Runtime.SmokeTest/Program.cs:709-710,815-816`; `tests/DualFrontier.Runtime.Tests/Compute/`). No Launcher/Application/mod code constructs a field binding or dispatches diffusion. The consumer wiring arrives with the demonstration mods (§3.1, FENCED; ROADMAP).
- **The rendering use case is in production every frame.** `DualFrontier.Launcher` is the shipping host: window, swapchain, batched sprite path, camera — §2.

### 1.2 On-disk tree (verified at `35364c2`)

```
src/
  // ====== Domain layer (preserved verbatim — zero substrate touch, L10) ======
  DualFrontier.AI/                     DualFrontier.Application/   (incl. Bridge/, Display/, Loop/, Modding/)
  DualFrontier.Components/             DualFrontier.Contracts/     (incl. Contracts/Modding, Contracts/Display)
  DualFrontier.Core/                   DualFrontier.Core.Interop/  (NativeWorld, FieldRegistry, FieldHandle<T>,
  DualFrontier.Crypto.Future/                                       PipelineSlotInterop, CpuKernels/)
  DualFrontier.Events/                 DualFrontier.Persistence/
  DualFrontier.Systems/

  // ====== V substrate (Vulkan + Win32 foundation) ======
  DualFrontier.Runtime/
    DualFrontier.Runtime.csproj        // exactly ONE project reference: DualFrontier.Core.Interop (csproj:8)
    Runtime.cs                         // top-level facade: composition + frame recording + compute factories
    RuntimeOptions.cs                  // window options, assets dir, validation default (#if DEBUG true / else false)
    MODULE.md                          // 11 MODULE.md files across the module directories

    Native/
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
      HardwareCapabilityCheck.cs / HardwareCapabilityException.cs        // К-L19 fail-fast (§0.1)
      PhysicalDeviceInfo.cs / PhysicalDeviceType.cs / QueueFamilyInfo.cs

    Compute/                           // V0 compute side + V1 primitive
      VulkanComputePipeline.cs / VulkanComputeDescriptors.cs / ComputeDispatch.cs
      ComputePipelineRegistry.cs / FieldStorageBinding.cs
      V1DiffusionPipeline.cs / DiffusionPushConstants.cs

    Sprite/
      Sprite.cs / AtlasRegion.cs / SpriteVertex.cs / SpriteTransform.cs / SpriteTexture.cs
      SpriteIndexBuffer.cs / VertexBufferRing.cs / SpriteDescriptorSetLayout.cs
      VulkanSpritePipeline.cs / SpriteRenderer.cs                        // batched Begin/Submit/End
      TileMap.cs / Camera2D.cs

    Assets/  PngDecoder.cs / PngImage.cs / PngChunk.cs / PngDecoderException.cs
             AssetManager.cs / AssetPath.cs
    Diagnostic/  ValidationLog.cs      // the ONLY diagnostic type; no FrameTimer/DebugOverlay (§1.6)

  // ====== Presentation host ======
  DualFrontier.Launcher/
    Program.cs                         // Main(): Runtime.Create + GameBootstrap + main loop (§2.2, §2.4)
    LauncherRenderer.cs                // IRenderer impl: bridge drain → dispatch → RecordSpritesFrame (§2.3, §5.3)
    RenderCommandDispatcher.cs / SceneState.cs / LauncherProceduralAtlas.cs / PawnSpriteEntry.cs

tests/
  DualFrontier.Runtime.Tests/          // §1.5
  DualFrontier.Runtime.SmokeTest/      // manual GPU smoke executable (Program.cs + ProceduralAtlas.cs)

tools/
  shaders/                             // complete GLSL source set (§1.4):
    clearcolor.vert / clearcolor.frag  //   V0 foundation clear pass
    sprite.vert / sprite.frag          //   sprite pipeline
    noop.comp                          //   V0 empty-dispatch test shader
    diffusion.comp                     //   V1 substrate shader   (no wave.comp — V2 not on disk)
  glslangValidator.exe                 // committed Khronos compiler (build-time)
  scaffold-runtime.ps1                 // idempotent runtime-directory materializer

assets/
  shaders/                             // compiled SPIR-V mirror (six .spv files)
  kenney/                              // art packs
  sprites/  scenes/                    // production sprite assets + scene samples
                                       // (the cinzel/ font pack recorded by the predecessor no longer exists —
                                       //  removed with the unshipped text scope; no font assets at HEAD)

mods/                                  // strict-v3 skeletons + example (§3.2)
  DualFrontier.Mod.Example/
  DualFrontier.Mod.Vanilla.{Combat,Core,Inventory,Magic,Pawn,World}/
  Directory.Build.targets              // manifest copy + hotReload Release rewrite — NO shader builds (§1.4)

native/
  DualFrontier.Core.Native/            // C++ kernel (CMake)
    include/  df_capi.h / compute_dispatch.h / pipeline_slot.h / phase_compute.h / tile_field.h / …
    src/      compute_dispatch.cpp / compute_pipeline.cpp / pipeline_slot.cpp / phase_compute.cpp /
              tile_field.cpp / world.cpp / mod_unload.cpp / system_graph.cpp / …
    test/     selftest.cpp
```

The scaffolding generator `tools/scaffold-runtime.ps1` remains committed and idempotent; running it materializes the rendering hierarchy without touching Domain. Repository-level census authority is the methodology layer (DEVELOPMENT_HYGIENE), not this document.

### 1.3 Module map and dependency rules

**`DualFrontier.Runtime` (top-level facade).** Generic 2D Vulkan substrate — window management, instance/device/queues, rendering primitives, batched sprite rendering, texture loading, input events, compute pipeline plumbing, scalar-field compute primitives. Knows nothing of Domain gameplay. Public surface via `Runtime.cs`: `Create(RuntimeOptions)` (full composition with fail-safe disposal + the К-L19 and F06 gates), `Window`/`InputQueue`, frame recording (`RecordSpritesFrame` batched Camera2D-driven production path; `BeginRenderPassForSprites`/`EndSpriteRenderPass` multi-cycle; `RecordSpriteFrame` single-sprite backward-compat shim), `RecreateFramebuffersForSwapchain`, compute (`ComputePipelines` registry property, `CreateFieldStorageBinding(NativeWorld)`, `CreateV1DiffusionPipeline(...)`).

**Sub-modules.**

- `Native.Win32`, `Native.Vulkan` — pure P/Invoke declaration layers; `internal` to Runtime; neither OS surface leaks beyond the Runtime project.
- `Window` — Win32 window lifecycle + message pump; owns `InputEventQueue` (the type lives in `Window/`).
- `Input` — typed input events + `VirtualKeyMapper`; consumed by polling.
- `Graphics` — Vulkan rendering primitives with idiomatic `IDisposable` lifetimes. **Consumed on disk by `Sprite`, `Compute`, and `Diagnostic`** (the predecessor's "used by Sprite, Text, Diagnostic" listed a `Text` module that has never existed — corrected to disk truth; §1.6).
- `Compute` — compute pipeline, descriptors, dispatch wrapper, native pipeline registration (`FieldStorageBinding`), the shipped V1 primitive. Shares the one `VkInstance`/`VkDevice` with `Graphics`.
- `Sprite` — atlas-based batched 2D rendering; `SpriteRenderer` (`BeginFrame`/`Submit`/`EndFrame`), `TileMap`, `Camera2D`.
- `Assets` — manual PNG decoder + asset path resolution (§2.7).
- `Diagnostic` — `ValidationLog` only: a thread-safe ring buffer (1024-message cap) receiving validation-layer callbacks from any driver thread; its `ErrorCount == 0` is the smoke-test exit criterion (`Diagnostic/ValidationLog.cs:1-16`).
- `DualFrontier.Launcher` — production presentation host; composes Domain (via `GameBootstrap`) with Runtime; owns "what to draw" (`SceneState` + `RenderCommandDispatcher`); §2.2–§2.4.

**Dependency rules (locked invariants, mechanically verifiable via the project-reference graph):**

- **Rule 1.** `DualFrontier.Runtime` MUST compile and pass tests without any reference to Domain gameplay projects. Its only permitted project reference is `DualFrontier.Core.Interop` (the native kernel bridge, required for K9 field binding). Verified: the csproj carries exactly that one reference (`src/DualFrontier.Runtime/DualFrontier.Runtime.csproj:8`).
- **Rule 2.** Domain ↔ Runtime communication happens ONLY through the `DualFrontier.Launcher` host (consumer chain `PresentationBridge` drain → `RenderCommandDispatcher` → `SceneState` → `Runtime.RecordSpritesFrame`) and through the field/compute bridge (`DualFrontier.Core.Interop`, plus the `IModApi` v3 surface for mods when wired — §3). Domain knows nothing of Runtime; Runtime knows nothing of Domain gameplay.
- **Rule 3.** Within Runtime, dependency direction respects layering: `Native.Win32`/`Native.Vulkan` → `Window`/`Input`/`Assets` → `Graphics` → `Compute` → `Sprite` → `Diagnostic` → `Runtime.cs` facade.
- **Rule 4.** No layer skipping (`Diagnostic` does not import `Native.Vulkan` directly; it goes through `Graphics`).
- **Rule 5.** Runtime exposes a minimal public API; implementation details are `internal`. Vulkan/Win32 struct names keep canonical spec naming (`VkInstanceCreateInfo`, `WNDCLASSEX`); C# wrappers are Pascal-case per the coding-standards law.

### 1.4 Shader set and build pipeline

**Build-time SPIR-V compilation** runs as the `CompileShaders` MSBuild target in the root `Directory.Build.props` (`Directory.Build.props:22-38`): `BeforeTargets="Build"`, conditioned on the `DualFrontier.Runtime` project, executing the committed `tools/glslangValidator.exe` so developer machines need no Vulkan SDK install. One toolchain compiles both graphics and compute shaders. The complete compilation set at HEAD — six shaders, sources in `tools/shaders/`, `.spv` outputs in `assets/shaders/`:

| Shader | Role |
|---|---|
| `clearcolor.vert` / `clearcolor.frag` | V0 foundation clear pass |
| `sprite.vert` / `sprite.frag` | Production sprite pipeline |
| `noop.comp` | V0 empty-dispatch test shader |
| `diffusion.comp` | V1 substrate shader (one template serving isotropic + anisotropic — §4.3) |

`mods/Directory.Build.targets` performs manifest copy plus the Release `hotReload` rewrite only — **no mod build compiles shaders today** (`mods/Directory.Build.targets:23-26` manifest copy, `:36-45` Release rewrite). The production binary depends on `vulkan-1.dll` + pre-compiled `.spv` files; no shader-compiler dependency ships.

> **FENCED (target / planned — not current truth):** Mod-side compute shader compilation per К-L9 «vanilla = mods»: each vanilla mod compiles its compute shader during the mod build with the same `glslangValidator.exe` toolchain, embeds the `.spv` bytes into mod assets, and registers through the `IModApi.ComputePipelines` surface at startup (§3.1). A V2 `wave.comp` line joins the `CompileShaders` target when V2 ships. Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

### 1.5 Test surface

Domain tests are untouched by substrate work (L10). The substrate's own suites, verified on disk in `tests/DualFrontier.Runtime.Tests/`:

- **Assets:** `PngDecoderTests`, `AssetManagerTests` — synthetic PNG → RGBA expectations; path smart-resolve.
- **Sprite:** `SpriteRendererTests`, `SpriteVertexTests`, `SpriteIndexBufferTests`, `AtlasRegionTests`, `Camera2DTests`.
- **Input:** `VirtualKeyMapperTests`.
- **Graphics:** marshalling/layout suites (`VulkanInstanceMarshallingTests`, `VulkanDeviceMarshallingTests`, `VulkanPipelineLayoutTests`, …), `HardwareCapabilityCheckTests`, `AsyncComputeQueueSelectionTests`, `ShaderCompilationTests`, `VulkanSwapchainTests`, `TextureUploaderTests`, `MemoryAllocatorTests`, per-wrapper lifecycle suites.
- **Compute:** `ComputePipelineRegistrationTests` (register/dispatch round-trip), `FieldStorageBindingTests`, `DiffusionPushConstantsTests`, `V1DiffusionFactoryTests`, `V1DiffusionIntegrationTests`, and `V1DiffusionEquivalenceTests` — CPU oracle vs GPU output on synthetic grids, tolerance-bounded (§6.2). A V2 equivalence suite is bound to land with V2 (§6.4 law).

**GPU-dependent verification** runs through `tests/DualFrontier.Runtime.SmokeTest` (stress scene, 200×200 tile-map scene, V1 isotropic and anisotropic wire-path field scenes) plus the committed manual visual-verification protocols. Validation-layer output captured to the `ValidationLog`; **clean validation output is the success criterion and a pre-commit check owned by this document** (§6.4). The native kernel side is exercised by `native/DualFrontier.Core.Native/test/selftest.cpp` (includes the V0.B compute registration round-trip scenario).

### 1.6 Explicit absence census

Things a reader of the predecessor might believe exist, stated plainly as absent at HEAD `35364c2`, with the section that owns each:

| Absent surface | Truth | Owner section |
|---|---|---|
| `Text/` module, bitmap font renderer, font assets | Never shipped; no `Text/` directory, no font asset pack on disk (the predecessor's tree still listed `assets/cinzel/` — never committed: git-ignored via `.gitignore` `/assets/Cinzel/`; no font asset is tracked at HEAD, though ignored local copies may persist in working directories) | §8 (font/UI open decisions); ROADMAP |
| `DebugOverlay`, `FrameTimer` | Never shipped; FPS measurement lives in the smoke-test executable | ROADMAP |
| `wave.comp` (V2) | Not on disk | §4.4 FENCED |
| Input forwarding into Domain | Does not exist — the Launcher drains and discards (§2.2) | §2.2; §8 |
| Focus → pause coupling | Not wired (§2.2) | §2.2 FENCED; §8 |
| Mod compute-pipeline registration | Contract placeholder; production returns `null` (§3.2) | §3 |
| Native `df_vulkan_unload_mod_resources` | No native symbol on disk; managed placeholder only (§3.3) | §3.3 |
| Runtime CPU-fallback dispatcher | Design option only, never built (§6.1) | §6.1 FENCED |
| `VK_ERROR_DEVICE_LOST` handling | Zero handlers repo-wide (§6.3) | §6.3 FENCED open question |
| Field save/serialization path | No field save path exists; `SaveSystem` is a stub (§7) | §7 |
| Godot residue | None — fully eradicated 2026-06-29/30 (F-5 CLOSED) | §0.2 L9 |

---

## §2 Architecture — device, presentation, slots

### 2.1 Instance, device, and queues

**Instance.** `VulkanInstance` owns `VkInstance` lifecycle: the 1.3 version gate (§0.1), extension activation (`VK_KHR_surface`, `VK_KHR_win32_surface`; `VK_EXT_debug_utils` when validating), and validation-layer activation (`VK_LAYER_KHRONOS_validation`). Validation defaults are build-flavored: `RuntimeOptions.EnableValidationLayer` is `true` under `#if DEBUG`, `false` in RELEASE (`src/DualFrontier.Runtime/RuntimeOptions.cs:18-23`) — validation ALWAYS-ON in development, zero third-party surface in production.

**Device and queue selection.** `VulkanDevice` selects the physical device and resolves two queue handles:

- **Graphics queue** — `GraphicsQueue` / `GraphicsQueueFamilyIndex` (`Graphics/VulkanDevice.cs:27-28`). Drives the render path and — via `TextureUploader` — staging uploads (`Graphics/TextureUploader.cs:121` submits to the graphics queue).
- **Async compute queue** — `AsyncComputeQueue` / `AsyncComputeQueueFamilyIndex` (`VulkanDevice.cs:36,42`). Selection prefers a **dedicated compute-only family** and falls back to any compute-capable family (`VulkanDevice.cs:183-202`). On hardware without a dedicated compute-only family, the async compute queue **aliases the graphics queue handle** — a single queue create-info is used and Vulkan returns the same `VkQueue` (`VulkanDevice.cs:222,285`).

Two corrections to the predecessor's queue story, to disk truth: (a) there is **no separate copy/transfer queue** — the predecessor's §2.3.1 listed a third "Copy/transfer queue" for asset transfers; on disk, transfers ride the graphics queue through `TextureUploader`; (b) the predecessor's "Graphics queue not used for compute" holds only when a dedicated compute-only family exists — the К-L19 *check* requires a compute-capable family to be resolvable (`HardwareCapabilityCheck.cs:36-47`), and the selection logic accepts the graphics family as that family when nothing dedicated exists. What К-L19 guarantees is therefore *an addressable compute queue*, with dedicated-family async execution as the preferred topology, not a universal hardware fact.

**Composition and teardown.** `Runtime.Create` composes, in order: `InputEventQueue` → `Window` → `VulkanInstance` (+ `ValidationLayer`) → `VulkanDevice` → **К-L19 gate** → `VulkanSurface` → **F06 present-support gate** → `VulkanSwapchain` → `VulkanRenderPass` + per-image `VulkanFramebuffer`s → graphics + compute `VulkanCommandPool`s → `MemoryAllocator` (bump allocator) → `ComputePipelineRegistry` → asset/sampler/uploader → sprite shaders + descriptor-set layout + pipeline layout + pipeline → batched `SpriteRenderer` (`maxSpritesPerFrame: 10_000`) → `Camera2D` (`Runtime.cs:63-186`). Any construction failure disposes every partially-constructed component before rethrowing — the «no leaked Vulkan handles» exit criterion. `Runtime.Dispose` waits the device idle, then tears down in strict reverse construction order (`Runtime.cs:438-479`).

**P/Invoke pattern.** All Vulkan and Win32 entry points are hand-written `[LibraryImport]` declarations (source-generated marshalling), e.g.:

```csharp
[LibraryImport("vulkan-1.dll", EntryPoint = "vkCreateInstance")]
internal static partial VkResult vkCreateInstance(
    in VkInstanceCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pInstance);
```

Direct-`LibraryImport` dispatch (rather than `vkGetInstanceProcAddr` procedure-address loading) is the shipped choice; migrating post-foundation if profiling demands remains an open decision (§8). The full V0 foundation shipped on hand-written bindings without a binding library — the predecessor's R1 binding-tedium risk is resolved moot.

### 2.2 Window and input truth

**Window.** Pure Win32: `Window` registers the class, creates the window, and pumps messages (`Window.PumpMessages`) on the thread that owns it — in production, the Launcher main thread. Window lifecycle events (close, resize, focus) and raw input messages are translated in the window procedure into typed events and enqueued.

**Input event surface (shipped).** `InputEventQueue` is an **unbounded `ConcurrentQueue<IInputEvent>`** channel (`src/DualFrontier.Runtime/Window/InputEventQueue.cs:13`) with `Enqueue`/`TryDequeue`/`Count`. Event types cover keyboard, mouse button/move/wheel, window resize, and window focus; `WM_SETFOCUS`/`WM_KILLFOCUS` are translated and enqueued as `WindowFocusEvent(Focused: true/false)` (`Window/Window.cs:253-257`).

**Input consumption truth (stated plainly).** The input → simulation path **does not exist**. The production Launcher drains the queue every frame and discards every event:

```csharp
// 2. Drain InputQueue → forward к Application.
//    Future cascade — InputBridge wiring TBD; events discarded for now.
while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
{
    // Future cascade will forward к Application input bridge here.
}
```

(`src/DualFrontier.Launcher/Program.cs:79-84`.) Every shipped input event — including the focus events that the designed pause coupling needs — dead-ends at this drain. No contract document governs input routing yet; the gap is recorded as a session finding (N-18) and carried as an open question (§8).

**Presentation bridge (shipped, one-way).** The Domain → render channel is `PresentationBridge`: an **unbounded `ConcurrentQueue<IRenderCommand>`** (`src/DualFrontier.Application/Bridge/PresentationBridge.cs:21`), enqueued from any domain thread, drained only by the render backend's main thread via `DrainCommands` (`PresentationBridge.cs:45-52`). The link is strictly one-way; `QueueDepth` is the only diagnostic surface. There is no backpressure mechanism on either queue — both are unbounded by construction; the sole consumer discipline is the per-frame drain.

> **FENCED (target / planned — not current truth):** Input forwarding — an input bridge carries drained events from the Launcher loop into Application (the "Future cascade" of `Program.cs:80`), giving Domain systems a typed input stream. Focus → pause coupling — the main thread reacts to `WindowFocusEvent(Focused: false)` by calling `loop.SetPaused(true)` (the pattern proven on the retired Godot path); all ingredients ship today except the wiring. Both Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md); the input-routing *contract* (ownership, ordering, bounding) is an open architecture question (§8).

### 2.3 Swapchain and recreation

**Shipped mechanics.** `VulkanSwapchain` owns the swapchain and its image views: `AcquireNextImage(signalSemaphore, signalFence, out bool outOfDate)` surfaces `VK_ERROR_OUT_OF_DATE_KHR` as a flag while treating `VK_SUBOPTIMAL_KHR` as success (`Graphics/VulkanSwapchain.cs:59-64`); `Present(queue, waitSemaphore, imageIndex)` returns an out-of-date flag likewise (`VulkanSwapchain.cs:93`); `Recreate(width, height)` rebuilds the swapchain (`VulkanSwapchain.cs:43`). `Runtime.RecreateFramebuffersForSwapchain` rebuilds the framebuffer list to match the recreated swapchain (`Runtime.cs:193-206`), and its doc comment carries the only protocol statement that exists: caller must invoke it after `Swapchain.Recreate` + `VulkanDevice.WaitIdle` (`Runtime.cs:188-192`).

**Production recreation flow.** `LauncherRenderer.RenderFrame` handles both out-of-date sites — acquire (`LauncherRenderer.cs:123-139`) and present (`LauncherRenderer.cs:172-196`) — with the same inline sequence: skip if the window is closing (shutdown-race guard), `VulkanDevice.WaitIdle()`, `Swapchain.Recreate(w,h)`, `RecreateFramebuffersForSwapchain()`, then re-fit `Camera2D` viewport/zoom to the new extent.

**Honest limitation.** Swapchain recreation is shipped **without a transactional protocol**: the shipped stage order is quiesce → *reclaim → prepare* → implicit commit — `RecreateFramebuffersForSwapchain` disposes every old framebuffer and clears the list *before* constructing the new ones (`Runtime.cs:195-205`), so a constructor failure mid-rebuild leaves the runtime with zero framebuffers and no rollback. This document records the mechanism as current truth and does not claim transaction safety for it.

> **FENCED (target / planned — not current truth):** A prepare-before-reclaim recreation transaction (build new swapchain/views/framebuffers alongside via Vulkan's `oldSwapchain`, fence-quiesce, commit by single-assignment swap, then best-effort reclaim of the old set) is specified in [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) §2.5, using its seven-stage lifecycle vocabulary — ratified law as of EVT-2026-07-17-DRAFTS_RATIFICATION. Adopting it here is the deferred substrate amendment (ROADMAP forward queue).

### 2.4 Threading model

[THREADING.md](./THREADING.md) is the concurrency authority — thread census, scheduler execution truth, happens-before rules. The substrate's contribution to the process model is deliberately minimal (L8): **one render thread merged with the OS message pump** — in production, the `DualFrontier.Launcher` main thread — beside the existing self-ticking simulation thread. Domain tick scheduling is untouched by substrate work; per К-L12 (KERNEL_ARCHITECTURE.md Part 0) scheduling authority is native, and THREADING.md owns the current-vs-target wiring statement for it.

```
┌──────────────────────────────────────────────────────────────────┐
│ Process threads (production composition — Launcher)              │
├──────────────────────────────────────────────────────────────────┤
│  Main thread (Launcher: window + render — MERGED, L8)            │
│    Win32 message pump (Window.PumpMessages)                      │
│      → InputEventQueue (typed events incl. focus)                │
│    InputQueue drain — events DISCARDED (no forwarding — §2.2)    │
│    LauncherRenderer.RenderFrame:                                 │
│      drain PresentationBridge → RenderCommandDispatcher          │
│        → SceneState → Runtime.RecordSpritesFrame                 │
│      acquire → record → submit → present                         │
│      per-image renderFinished semaphores + shared imageAvailable │
│      + frame fence Wait/Reset each frame (§5.3)                  │
│                                                                  │
│  Simulation thread (GameLoop — self-ticking background thread,   │
│  30 TPS fixed step; GameLoop.cs:29)                              │
│    system execution per THREADING.md (native graph authority     │
│    per К-L12; managed executor as current wiring)                │
│    → PresentationBridge.Enqueue (any-thread producer)            │
│    [designed home of V1 compute dispatch — no production         │
│     dispatch site exists today; §1.1, §4.3]                      │
│                                                                  │
│  GPU (asynchronous to CPU)                                       │
│    async compute queue — compute dispatches (may alias graphics  │
│      queue on non-dedicated hardware — §2.1)                     │
│    graphics queue — recorded command buffers + staging uploads   │
│    fences signal completion back to CPU                          │
│                                                                  │
│  Worker threads — native kernel thread pool + managed phase      │
│  parallelism (THREADING.md owns this model)                      │
└──────────────────────────────────────────────────────────────────┘
```

**Cross-thread channels.** Exactly two substrate-adjacent queues cross threads: `PresentationBridge` (simulation → render, drained per frame) and `InputEventQueue` (window → consumer; today drained and discarded on the same thread that fills it — §2.2). Compute dispatch, when a consumer exists, is a *synchronous* call on the dispatching thread: `V1DiffusionPipeline.ExecuteIteration` → native `dispatch_compute_field` → returns after the fence signals (§5.1) — so a subsequent `FieldHandle<T>.ReadCell` on that thread sees the dispatched result. The opt-in К-L7.1 pipeline-managed alternative (bounded one-tick slot-tail lag) is §2.5/§5.2. Thread-ownership rows for every GPU-adjacent object are tabulated in [CONCURRENCY_AND_MEMORY_MODEL.md](./CONCURRENCY_AND_MEMORY_MODEL.md) §2.3.

### 2.5 Pipeline depth and slots (К-L16, К-L7.1)

Per К-L16 (KERNEL_ARCHITECTURE.md Part 0), the substrate supports simulation-tick pipeline depth **D = 2 by default, configurable 1–3**, for *pipeline-managed* dispatches: the simulation thread may run D ticks ahead of the display thread for those dispatches, giving cross-layer async operations a full pipeline-depth window to complete without blocking simulation. Per К-L7.1, pipeline-managed field reads bind to the **slot tail**: sim-thread reads of a pipeline-managed field see the slot-tail state (sim_tick − 1) without a per-read fence query; К-L7 atomic-from-observer is preserved *within* a slot boundary, and cross-slot reads see different snapshots. Display reads take `CurrentSimTick − D` for pipeline-managed display state.

**Slot data model — preserved verbatim from the shipped header** (`native/DualFrontier.Core.Native/include/pipeline_slot.h:45-64`):

```c
// Slot state machine per spec §3.10 Item 33 verbatim.
typedef enum {
    SlotState_Empty = 0,           // Initial state, no sim_tick assigned
    SlotState_Dispatched = 1,      // Sim thread dispatched compute work к GPU
    SlotState_FenceCompleted = 2,  // GPU finished, results available
    SlotState_ReadableAsTail = 3   // Display/sim thread reads from here
} SlotState;

// PipelineSlot struct verbatim from spec §3.10 Item 33.
//
// fields_snapshot_ptr: К-L7.1 binding subject — pipeline-managed FieldStorageSnapshot.
// compute_fence_handle: VkFence opaque (cast к/от VkFence в integration code).
// world_snapshot_ptr: NativeWorld snapshot pointer (К-L7.1 binding subject).
typedef struct {
    uint64_t sim_tick;
    void* world_snapshot_ptr;
    void* fields_snapshot_ptr;
    void* compute_fence_handle;
    int32_t state;  // SlotState enum value (int32 для C ABI portability)
} PipelineSlot;
```

**Fence orchestration** tracks slot transitions `Empty → Dispatched → FenceCompleted → ReadableAsTail`. The shipped C ABI (`pipeline_slot.h`): `df_pipeline_init(depth)` / `df_pipeline_reset` / `df_pipeline_get_depth`; `df_pipeline_allocate_slot(sim_tick, out_slot)` (cycles through D slots; returns null when all D slots are in flight — the К-L16 backpressure point); `df_pipeline_get_slot(slot_offset, out_slot)` with `0` = current, `-1` = previous (the К-L7.1 sim-thread tail read), `-2..-D` = display tail; `df_pipeline_set_fence(slot, vk_fence)`; `df_pipeline_check_fences(out_transitioned)`; `df_pipeline_force_fence_completed(slot)`; `df_pipeline_transition_to_tail(slot)` (fires the slot-transition wake hook); `df_pipeline_is_quiescent(out)` — the К-L18 precondition primitive: quiescent means every slot is `Empty` or `ReadableAsTail` (no in-flight compute), consumed by the mod-unload chain before mod operations. Save/load slot-metadata serialization primitives also ship (§7.1).

**Wiring truth, stated honestly.** The slot state machine, its C ABI, and the managed mirror (`src/DualFrontier.Core.Interop/PipelineSlotInterop.cs`) are on disk and test-exercised (7 dedicated interop tests on disk — `PipelineSlotInteropTests.cs`; 35 interop tests + 14 native selftest pipeline scenarios recorded at К10.3 v2 closure). Two integration edges are explicitly *not* live:

- `df_pipeline_check_fences` is a **recorded stub**: it returns "zero slots transitioned" without polling — actual `vkGetFenceStatus` integration was deferred to the Phase.Compute commit that has a `VulkanAttachment` context, and callers are directed to `df_pipeline_force_fence_completed` (test path) meanwhile (`native/DualFrontier.Core.Native/src/pipeline_slot.cpp:153-163`).
- **No production consumer opts in.** Phase.Compute infrastructure (`phase_compute.h/.cpp` — named `Update/Compute/Display` phases, per-tick dispatch registry, single-`VkQueueSubmit` batching of up to 256 dispatches) is scaffold; actual pipeline-managed compute consumers were scoped out of К10.3 v2 and have not arrived since. The `FenceCompleted → ReadableAsTail` wake hook has an observable fire counter, and the `[WakeOnSlotTransition]` consumer attribute exists (`src/DualFrontier.Contracts/Scheduling/WakeOnSlotTransitionAttribute.cs`), but subscriber-registry integration for it is Planned — THREADING.md states the same.

**Coexistence law (current truth).** The V1 synchronous dispatch path (К-L7 — §5.1) is the default and remains fully operational, orthogonal to the slot machinery; К-L7.1 pipeline management is opt-in per field, an author choice per К-L9 «Vanilla = mods». Nothing on disk forces any field onto the slot path today, and nothing routes through it in production.

### 2.6 Display composition (К-L17)

Per К-L17 (KERNEL_ARCHITECTURE.md Part 0), display output is composed from layers with independent latency contracts. The framework lives one architectural layer **above** the substrate — `src/DualFrontier.Application/Display/` (`CompositionFramework.cs`, `SimStateLayer.cs`, `IntentOverlayLayer.cs`, `CombatFeedbackLayer.cs`, `Layer.cs`, `ILayerRenderContext.cs`) with the mod-facing attribute surface in `src/DualFrontier.Contracts/Display/` (`LayerAttribute.cs`, `LayerType.cs`). The substrate's role under К-L17 is fixed: it exposes rendering primitives (`SpriteRenderer`, `Camera2D`, `TileMap`); layer composition operates above them and above the preserved `IRenderer` interface, not by extending them.

**Wiring truth.** The composition framework is on disk and test-exercised (`tests/DualFrontier.Application.Tests/Display/`), but the production render path does **not** route through it: `LauncherRenderer` drains the bridge and records sprites directly (§2.2), and `CompositionFramework` has zero production consumers at HEAD. К-L17's three-layer model is therefore framework-shipped, composition-pending.

> **FENCED (target / planned — not current truth):** The К-L17 composed pipeline — (1) **SimStateLayer**: the V substrate render path as the default layer slot, reading slot tail for pipeline-managed display state (К-L16 `D × tick_period` latency) or current state for К-L7 sync fields; (2) **IntentOverlayLayer**: current input state read from `InputEventQueue` at display-tick time, ≤ 16 ms contract (blocked today on the same missing input forwarding as §2.2); (3) **CombatFeedbackLayer**: Fast-tier event consumers per К-L15 (KERNEL_ARCHITECTURE.md Part 0) rendering damage numbers/hit feedback, ≈ ≤ 17 ms event-to-visible. Composition order: SimState first, intent + combat overlays on top, static layers last. Mod-registered layers use `[Layer(LayerType.Intent | CombatFeedback)]` plus registry-emitted `kernel.layer.intent:{FQN}` / `kernel.layer.combat_feedback:{FQN}` tokens (observable via `GetKernelCapabilities()`, not manifest-declarable — MOD_OS_ARCHITECTURE.md §3.2) — vanilla layers register through the identical pattern per К-L9. Wiring the framework into the Launcher path is Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md).

### 2.7 Asset pipeline

PNG loading flow, all substrate-owned:

```
disk PNG file
  → PngDecoder.Decode                 (manual chunk parsing: IHDR/IDAT/IEND; DEFLATE via BCL
                                       DeflateStream; Sub/Up/Average/Paeth unfiltering; CRC32;
                                       RGBA8 output — other formats deferred)
  → PngImage (RGBA bytes + dimensions)
  → VulkanImage.CreateFromPngImage    (staging upload via TextureUploader on the graphics queue,
                                       with layout transitions)
  → SpriteTexture (image + sampler) + AtlasRegion rects
  → Sprite instances → SpriteRenderer.Submit   (consumed by the Launcher's SceneState)
```

`AssetManager` owns root-directory smart-resolve; `Runtime.Create` loads sprite shader modules through the resolved root (`Runtime.cs:136-141`). The production atlas today is procedurally generated in the Launcher (`LauncherProceduralAtlas`) and uploaded once at composition (`Program.cs:48-52`); atlas metadata format (code vs data file) remains an open decision (§8).

### 2.8 Relationship to the K substrate

The V substrate (this document) and the native ECS kernel are two independent native layers under the managed Application layer, symmetric by design: pure P/Invoke each (`vulkan-1.dll` for V; `DualFrontier.Core.Native.dll` for K), thin managed adapters, single ownership boundary, direction-disciplined. Rendering knows nothing of ECS storage; the kernel knows nothing of Vulkan. The single integration point is field compute: **storage is K's** (`RawTileField` — [FIELDS.md](./FIELDS.md)), **compute is V's**, and the binding is `FieldStorageBinding` handing Vulkan handles to the kernel via `df_world_attach_vulkan` (§4.3). Native-side C ABI conventions (status codes, exception swallowing at the boundary) are the kernel document's law and apply unchanged to the compute entry points.

---

## §3 Mod GPU surface

### 3.1 Design allocation (К-L9)

Per К-L9 «vanilla = mods» (KERNEL_ARCHITECTURE.md Part 0), compute shaders belong to mods, not the engine. The substrate ships *mechanisms* (V1 diffusion template, V2 wave template when authored, dispatch + fence + point-query plumbing); mods own every gameplay field and its parameters. Third-party mods extend through the same registration API as vanilla.

> **FENCED (target / planned — not current truth):** The designed allocation: `Vanilla.Magic` owns the mana field + isotropic V1 configuration; `Vanilla.Electricity` owns the power field + anisotropic V1 configuration; `Vanilla.Movement` owns routed flow-field configurations on V2. Mod startup registers fields and pipelines through the `IModApi` v3 sub-APIs — illustrative design sketch (string ids throughout, matching the shipped ABI; the `ComputePipelines.RegisterPipeline` and `Systems.RegisterFieldUpdate` calls shown **do not exist yet**):
>
> ```csharp
> public class MagicMod : IMod
> {
>     public void Initialize(IModApi api)
>     {
>         var manaField = api.Fields.RegisterField<float>("vanilla.magic.mana", 200, 200);
>         var diffusion = api.ComputePipelines.RegisterPipeline(
>             "vanilla.magic.mana_diffusion",
>             EmbeddedResource.Load("shaders/mana_diffusion.spv"));
>
>         api.Systems.RegisterFieldUpdate(
>             "ManaFieldUpdate",
>             phase: SimulationPhase.PostPawn,
>             interval: TickInterval.Every(30),          // mana diffuses slowly
>             handler: ctx => manaField.DispatchCompute(diffusion, manaParams, iterations: 5));
>     }
> }
> ```
>
> The registration surface, per-mod resource tracking (feeding §3.3), and the demonstration mods (mana, electricity, movement — the Q-R-1 M-V series) are Planned — see [ROADMAP §Native foundation tracks](../ROADMAP.md). Exact API law at landing time is [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md)'s to ratify.

### 3.2 On-disk truth

- **`IModApi.Fields` is wired.** Production constructs `RestrictedFieldApi` over the kernel field registry when native field storage is present (`src/DualFrontier.Application/Modding/RestrictedModApi.cs:85,213`). The contract (`src/DualFrontier.Contracts/Modding/IModFieldApi.cs`) exposes `RegisterField<T>(string id, int width, int height)`, `GetField<T>(string id)`, `IsRegistered(string id)` — string field ids namespaced by mod id (`"vanilla.magic.mana"` for mod `vanilla.magic`), capability-gated at acquisition. Storage semantics defer to [FIELDS.md](./FIELDS.md).
- **`IModApi.ComputePipelines` is a placeholder returning `null`.** The production property is hardwired: `public IModComputePipelineApi? ComputePipelines => null;` (`RestrictedModApi.cs:216`). The contract itself (`src/DualFrontier.Contracts/Modding/IModComputePipelineApi.cs`) carries only a `Name` property and a self-describing placeholder comment — there is **no registration or dispatch member in the contract today**, and no `DispatchCompute` symbol exists anywhere in `src/` (grep-verified). Mods must null-check and degrade gracefully.
- **The vanilla mods are strict-v3 skeletons.** `Vanilla.Magic`'s `Initialize` is an empty TODO body (`mods/DualFrontier.Mod.Vanilla.Magic/MagicMod.cs:19-22`); `Vanilla.Electricity`, `Vanilla.Water`, and `Vanilla.Movement` do not exist as projects (`mods/` census: Example + Vanilla.{Combat,Core,Inventory,Magic,Pawn,World}).
- **The V1 pipeline is engine-composed.** Where V1 runs at all (smoke test, tests), pipelines are constructed via the `Runtime` factories (§4.3), not via any mod surface.

### 3.3 Mod unload — V resource cleanup (Step 3.6)

The mod-unload chain (MOD_OS_ARCHITECTURE.md §9-series law) includes a V-substrate cleanup step. Its on-disk truth at HEAD, precisely:

- **Managed placeholder shipped.** `VResourceCleanup.UnloadModResources(modId)` exists and is invoked from the unload pipeline; it **returns `Success = true` with zero counts** — pipelines/descriptor-sets/buffers/images destroyed all `0` — because no pipeline-managed mod resources can be registered yet (`src/DualFrontier.Application/Bridge/VResourceCleanup.cs:52-68`). Per the MOD_OS best-effort law, Step 3.6 failures would surface as validation warnings, not halt the chain.
- **No native primitive exists.** `df_vulkan_unload_mod_resources` has **no symbol in the native tree** (grep over `native/` headers and sources: zero hits) — the predecessor presented the C signature and `VulkanModUnloadResult` struct as a landed ABI; on disk they are design-only, mirrored in shape by the managed `Result` class. This document records the C-level surface as design, not shipped.
- **Precondition already real.** The К-L18 quiescent-state check that must precede Step 3.6 *is* shipped — `df_pipeline_is_quiescent` (§2.5) — so the ordering contract holds even while the cleanup body is vacuous.

> **FENCED (target / planned — not current truth):** Full implementation lands when mod-registered Vulkan resources exist to clean: the native `df_vulkan_unload_mod_resources(const char* mod_id, VulkanModUnloadResult* out)` primitive performing `vkDestroyPipeline` / `vkFreeDescriptorSets` / `vkDestroyBuffer` / `vkDestroyImage` over per-mod-tracked handles, best-effort sequential, error messages surfaced in the result struct; the managed wrapper switches from placeholder to P/Invoke translation. Per-mod resource tracking parallels the pipeline-registration bookkeeping of §4.3. Sequenced with the mod-facing registration surface (§3.1) — see [ROADMAP §Native foundation tracks](../ROADMAP.md); resource-ownership rows for these handles belong to [RESOURCE_OWNERSHIP_AND_LIFETIME.md](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) at its ratification.

---

## §4 Compute path

### 4.1 Two compute domains

The compute use case serves two architecturally distinct workload categories; both remain LOCKED as supported domains.

**Domain A — field updates (primary, foundational; V1 + V2 primitives).** Spatial scalar/vector fields stored as dense 2D grids; each cell updates per tick (or sub-tick) via local-stencil operations — diffusion (V1), wave propagation (V2), decay. Embarrassingly parallel; one shader invocation per cell. Storage is the kernel's `RawTileField` (K9; layout, conductivity map, storage flags, span law all per [FIELDS.md](./FIELDS.md)). Field-based mechanics are not an optimization target; they are a generative architectural pattern that absorbs new gameplay mechanics through additive registration — the reason compute is foundational in this substrate.

**Domain B — entity-keyed bulk computation (secondary, deferred disposition).** Per-entity uniform calculations that scale poorly on CPU at high entity counts (the original projectile case). Suitable when per-entity work is branch-free, counts are high (≳ 500), and output tolerates one-tick asynchronous readback. Storage would expose native component spans directly as SSBO content, no marshalling. **Substrate disposition is deferred**: whether Domain B becomes a V3+ primitive, its own substrate, or stays consumer-level is decided at the amendment that reactivates projectile bulk-compute work (§8; sketch preserved in §4.7 FENCED).

### 4.2 Why GPU compute is foundational here (rationale, carried)

The pre-pivot deferral rationale (managed-runtime dispatch overhead of 0.5–2 ms per roundtrip, breakeven ≈ 500 projectiles) was correct for the managed-era architecture and **does not apply** post-pivot: the dispatch path is native → `vulkan-1.dll` with no managed crossings, component/field data is already SoA, and the Vulkan device is already live for rendering. Native dispatch overhead is microseconds. For Domain A the choice was never "CPU now, GPU when scale demands" — it is "field math on CPU forever (with a scaling cap) vs. field math on GPU (free at any scale)"; given the К-L19 hardware baseline and the architectural fit, the GPU path is structurally preferable and is the shipped law. Domain B retains the threshold-driven deferral story.

### 4.3 V1 diffusion — the shipped primitive

**Mathematical model.** One shader template serves both variants:

- Isotropic (mana, basic spread): `∂P/∂t = D·∇²P + S(x,y) − K·P` — uniform D.
- Anisotropic (electricity/water — wires/pipes channel flow): `∂P/∂t = ∇·(D(x,y)·∇P) + S(x,y) − C(x,y)·effectiveness(P)` — per-cell D; wire/pipe tiles D ≈ 10.0, off-path D ≈ 0.1, insulators D = 0.0.

**Shipped shader truth** (`tools/shaders/diffusion.comp`, verified): a single ~65-line GLSL compute shader, `local_size 8×8`, three storage-buffer bindings — `FieldIn` (readonly), `FieldOut` (writeonly), `Conductivity` (readonly) — and a 16-byte push-constant block `{ float decayCoefficient; float deltaTime; uint width; uint height; }`. The per-cell D always comes from the conductivity buffer: a uniform-valued buffer *is* the isotropic case; a per-cell map *is* the anisotropic case — one template, parameter-selected, exactly as designed. The flow rule is the asymmetric `flow(self → neighbour) = min(D_self, D_neighbour) · (P_n − P_self)`, which guarantees blocking when either tile is a non-conductor — "narrow wave" along a wire is emergent, not coded. Boundary handling: a missing neighbor contributes zero flow (equivalent to a reflective boundary), matching the CPU reference convention. Two precision notes against the older prose: the **source term `S(x,y)` is not in the shipped shader** — sources are applied managed-side (`FieldHandle<T>.WriteCell`) between dispatches; and the **diffusion coefficient has no dedicated push constant** — conductivity carries it entirely.

**Registration and dispatch plumbing (all string-addressed).** The shipped C ABI on `df_capi.h` — corrected here to the on-disk signatures; the predecessor's §3.4 sketch showed `uint32_t field_id` parameters, contradicting both its own shipped bridge and the string-id law of [FIELDS.md](./FIELDS.md), and that sketch is superseded by the following (the one-line correction the corpus rework owes this section). Key signatures exact; mid-block field accessors are compressed with elided parameters marked `…`:

```c
/* Field surface (storage law: FIELDS.md; all functions string-addressed) */
DF_API int32_t  df_world_register_field(df_world_handle, const char* field_id,
                                        int32_t width, int32_t height, int32_t cell_size);
DF_API int32_t  df_world_field_unregister(df_world_handle, const char* field_id);
DF_API int32_t  df_world_field_read_cell / _write_cell(…, const char* field_id, …);
DF_API int32_t  df_world_field_acquire_span / _release_span(…, const char* field_id, …);
DF_API int32_t  df_world_field_set_conductivity(…, const char* field_id, …);
DF_API float    df_world_field_get_conductivity(…, const char* field_id, …);
DF_API int32_t  df_world_field_set_storage_flag / _get_storage_flag(…, const char* field_id, …);
DF_API int32_t  df_world_field_swap_buffers(df_world_handle, const char* field_id);
DF_API int32_t  df_world_field_count(df_world_handle);

/* Compute surface (V0.B; df_capi.h:551-577) */
DF_API int32_t  df_world_attach_vulkan(df_world_handle,
                    void* vk_instance, void* vk_physical_device, void* vk_device,
                    void* vk_async_compute_queue, uint32_t async_compute_queue_family_index);
DF_API uint32_t df_world_register_compute_pipeline(df_world_handle,
                    const char* pipeline_name, const uint8_t* spirv_bytecode, int32_t spirv_size,
                    uint32_t descriptor_binding_count, uint32_t push_constant_size);
DF_API int32_t  df_world_field_dispatch_compute(df_world_handle,
                    const char* field_name, uint32_t pipeline_id,
                    const uint8_t* push_constant_data, int32_t push_constant_size,
                    uint32_t dispatch_x, uint32_t dispatch_y, uint32_t dispatch_z);
DF_API int32_t  df_world_compute_pipeline_count(df_world_handle);
```

Identity law as shipped: **fields are addressed by string id everywhere**; a compute *pipeline* registers under a string name and receives a non-zero numeric `pipeline_id` (monotonic from 1) used for dispatch — 0 signals registration failure (duplicate name, empty/misaligned SPIR-V, Vulkan object creation failure).

**Managed layers above the ABI.**

- `DualFrontier.Core.Interop`: `NativeWorld.Fields` (`NativeWorld.cs:118`) → `FieldRegistry` (`Register<T>(string id, int, int)` at `FieldRegistry.cs:33`, `Get<T>`/`TryGet<T>`/`IsRegistered`/`Unregister`/`Count`) → `FieldHandle<T>` (`ReadCell`/`WriteCell` at `FieldHandle.cs:43,63`, `AcquireSpan()` returning the ref-struct `FieldSpanLease<T>` at `:82`, `SetConductivity`/`GetConductivity` at `:103,120`, `SetStorageFlag`/`GetStorageFlag` at `:134,152`, `SwapBuffers` at `:166`). Semantics (mutation rejection while spans are active, span lifetime, ping-pong law) are FIELDS.md's.
- `DualFrontier.Runtime.Compute`: `FieldStorageBinding` wraps world attachment and pipeline registration — `Attach(VulkanInstance, VulkanDevice)` (`FieldStorageBinding.cs:31`) hands the instance/physical-device/device/async-compute-queue handles to the kernel via `df_world_attach_vulkan`; `Register(name, spirv, bindingCount, pushConstantSize)` (`:54`) wraps pipeline registration; `DispatchField(...)` (`:66`) wraps dispatch. `V1DiffusionPipeline` composes on top: construction registers the diffusion pipeline (3 bindings, 16-byte push range — `V1DiffusionPipeline.cs:42-45`) and `ExecuteIteration(fieldName, pushConstants, dispatchX, dispatchY)` performs one synchronous iteration (`:67-77`); the consumer owns the iteration loop (typical gameplay use 5–10 iterations per dispatch tick) and any `SwapBuffers` ping-pong across iterations.
- `Runtime` facade factories: `CreateFieldStorageBinding(NativeWorld)` (`Runtime.cs:392` — world ownership stays with the caller; validates attachment) and `CreateV1DiffusionPipeline(binding, pipelineName)` (`Runtime.cs:416` — loads `diffusion.comp.spv` once via the AssetManager root and caches the bytecode for subsequent registrations).

**Native dispatch flow (К-L7 sync path).** `dispatch_compute_field` maintains per-field shadow `VkBuffer`s (input + output + conductivity), uploads CPU field state, records and submits the dispatch to the attached async-compute queue, **blocks on the dispatch `VkFence`**, then reads the GPU result back into the field's primary CPU buffer (`native/DualFrontier.Core.Native/include/compute_dispatch.h:1-11`; fence wait at `src/compute_dispatch.cpp:326-332`). Two architectural consequences: the **CPU-side `RawTileField` remains the canonical state at every instant the call is not executing** (the GPU is a transient executor, which is what makes the CPU oracle and the future save story coherent — §6.2, §7), and dispatch is a blocking simulation-thread operation by design (§5.1). Failure modes returned as `false`/0: unknown pipeline id, unregistered field, Vulkan operation failure, push-constant overflow, world not Vulkan-attached.

**Consumers today.** The registration round-trip and dispatch are exercised by `ComputePipelineRegistrationTests`, the V1 suites, and the smoke-test field scenes (isotropic 200×200; anisotropic wire-path). No production dispatch site exists (§1.1). The separate `ComputePipelineRegistry` on the `Runtime` facade (`Compute/ComputePipelineRegistry.cs`) is a substrate-side named registry owning `VulkanComputePipeline` + `VulkanComputeDescriptors` objects for runtime-composed consumers; it is composed at startup and disposed with the facade — distinct from the kernel-side registration used by the V1 field path.

**Managed-side gameplay patterns (design allocation, mod-level; carried).** Storage cells (batteries/tanks/thermal mass) are gameplay-level dynamic spikes on the topology, not a shader feature: the owning mod modulates cells between dispatches (`storage[t+1] = α·storage[t] + (1−α)·field_local[t]`, release `β·storage[t]` on demand; α ≈ 0.95–0.99). Cliff-threshold consumer effectiveness (the electricity 60% rule: full effect at `P ≥ demand`, proportional in `[0.6·demand, demand)`, zero below) is computed managed-side after the field update — below-threshold consumers pull nothing, freeing capacity, and the system self-stabilizes. Both belong to consumer mods per К-L9; the substrate shader stays simple.

### 4.4 V2 wave — design rationale (not on disk)

> **FENCED (target / planned — not current truth):** **V2 — scalar field + wave-propagation shader through a discrete topology overlay.** Routed (not isotropic), **breakable** — propagation respects walls / cliffs / closed pipes / cut cables. Distance and direction fields fall out as **side products** of propagation, which is why no separate flow-field-infrastructure primitive exists (folded here by the Q-G-2 reductions).
>
> **Baseline math (Option B — distance-ish field via diffusion):** `∂D/∂t = ∇²D − K·D + spike_at_target`. Not geodesic-accurate, but produces a correct gradient toward the target; cheaper, less machinery, ~99% gameplay-equivalent for a colony sim (pawns don't optimize paths; players don't notice ~5% suboptimal routing).
>
> **Deferred math (Option A — eikonal):** `‖∇D(x,y)‖ = 1/speed(x,y)` with `speed` 1.0 open ground / 0.5 difficult / 0 walls; GPU implementation via Fast Sweeping or Fast Marching, Godunov-upwind update per sweep, 5–10 sweeps converging on a 200×200 grid in microseconds on the mandated hardware tier. Whether eikonal is a V2 tunable (compile-/dispatch-time selection) or a separate primitive is **evidence-gated**: if the Option-B baseline shows gameplay-relevant suboptimality, upgrade; otherwise diffusion stays (§8).
>
> **Direction field extraction:** after the distance field stabilizes, one pass computes the normalized negative gradient per cell (`dir = normalize(vec2(w−e, n−s))`), stored as `vec2` per cell — 320 KB per 200×200 field, trivial.
>
> **Exit criteria (design contract, to be evidenced at V2 closure):** distance field converges to a correct gradient on representative grids; direction extraction yields a walkable gradient; propagation respects wall/closed-pipe/cut-cable barriers in synthetic obstacle scenarios; the electricity-through-cables and pawn-navigation demonstration mods exercise both use cases. The V2 equivalence suite and a CPU reference kernel are mandatory at landing (§6.4).
>
> **Hybrid V1↔V2 coupling (TBD):** how V1 diffusion picks up from a broken V2 wave node (water propagates in pipes via V2; on pipe break it diffuses ambient via V1). The coupling spec is deliberately deferred to a V-substrate amendment at the first integration point (the water demonstration) — held open in §8.

### 4.5 Gameplay configurations of V1/V2 (design allocation)

> **FENCED (target / planned — not current truth):** The configuration table — every row is a mod-owned configuration of a substrate primitive, never a new primitive:
>
> | Field | Sources | Sinks | Conductivity | Storage (gameplay-config) | Primitive |
> |---|---|---|---|---|---|
> | Mana | springs, ley lines | spell casts | uniform | magic accumulators | V1 |
> | Electricity | generators, solar | consumers (cliff rule) | wires (high D) | batteries | V1 |
> | Water | pumps, wells | drains, irrigation | pipes (high D) | tanks | V1 + V2 (wave in pipes; ambient diffusion on break — coupling TBD) |
> | Heat | furnaces, sun | cold tiles, refrigerators | air medium / insulation low | thermal-mass walls | V1 |
> | Sound | combat, machinery | decay-dominated | air, walls | — | V1 |
> | Scent | food, blood, entities | time decay | air, terrain | — | V1 |
> | Routed navigation | target tile (spike) | (gradient) | walkable terrain | — | V2 (distance + direction side products) |
>
> Modder-defined fields extend the same templates. Engine-vs-mod placement: the substrate provides field binding (shipped), the V1 template (shipped), the V2 template (pending), dispatch + fence + point queries (shipped); mods provide every field definition, parameterization, recompute policy, and lifecycle (flow fields as per-target ephemeral resources with LRU eviction and a capped pool — mod-managed, not substrate).

### 4.6 Flow-field navigation (design allocation)

> **FENCED (target / planned — not current truth):** Pathfinding is mathematically isomorphic to wave propagation — target spike ≙ generator, walkability ≙ conductivity, distance field ≙ power field, gradient read ≙ effectiveness read — so V2 adds navigation without new architectural surface. Cost model: per-target field computation `O(K·M)` once plus `O(N)` per-pawn point reads, versus per-pawn A* `O(N·M log M)`; cost decouples from pawn count. Pawn movement is a **three-mode hybrid** (mode selection is mod-level tuning): **Mode A** — autonomous baseline: persistent GPU direction fields per recurring destination type, all autonomous pawns read them (the hot loop, pawn-count-independent); **Mode B** — small player command (≤ ~10 pawns): CPU per-pawn A* via the existing `IPathfindingService`/`AStarPathfinding` (cheaper than spinning up a temporary dispatch; threshold N is gameplay tuning, §8); **Mode C** — mass event: one temporary V2 dispatch creates a destination field all affected pawns read, destroyed after resolution. Local avoidance stays a separate mod-level concern (steering on top of the field direction; pure managed CPU). Mode-C visibility latency is governed by the К-L17 composition framework (§2.6) uniformly — no special-case mechanism.

### 4.7 Domain B kernel sketch (deferred)

> **FENCED (target / planned — not current truth):** The preserved projectile-style Domain B pattern: one invocation per entity (`local_size_x = 64`), read-only entity input buffer + output buffer + obstacle buffer, `{dt, count}` push constants; integrate position, brute-force collision test, write result; dispatched once per tick with one-tick asynchronous readback. Threshold for adoption is experimental (CPU degradation point), and post-pivot integration cost is negligible relative to the original framing, so the threshold may shift downward. Disposition (V3+ primitive vs own substrate vs consumer-level) is decided at the reactivation amendment (§8).

### 4.8 Memory budget

Per 200×200 float field, from the shipped kernel layout (`native/DualFrontier.Core.Native/include/tile_field.h:18-23`): primary buffer 160 KB + ping-pong back buffer 160 KB + conductivity map 160 KB (floats) + storage flags **5 KB** (per-cell *bit*, byte-packed `(w·h+7)/8` — the predecessor's 40 KB assumed a byte per cell) ≈ **485 KB CPU-side per active field**. The V1 dispatch path additionally maintains GPU shadow buffers (input + output + conductivity ≈ 480 KB per dispatched field) for the lifetime of the attachment (§4.3). Ten simultaneous fields ≈ 5 MB CPU + 5 MB GPU — negligible against the hardware tier. A future 1000×1000 map scales to ≈ 12 MB per field CPU-side; still trivial.

---

## §5 Synchronization and visibility

### 5.1 К-L7 sync dispatch path (default; fence-block truth)

The default compute dispatch is **synchronous to the caller**: `V1DiffusionPipeline.ExecuteIteration` → `df_world_field_dispatch_compute` → native `dispatch_compute_field`, which submits to the async-compute queue and **blocks on the dispatch fence** — `vkResetFences` → `vkQueueSubmit(…, dispatch_fence)` → `vkWaitForFences(…, VK_TRUE, UINT64_MAX)` — before reading results back into CPU storage (`src/compute_dispatch.cpp:326-332`; sync model declared in `compute_dispatch.h:9-11`; managed doc-comment law "call returns after the compute fence signals", `V1DiffusionPipeline.cs:65-66`). Consequence: a `FieldHandle<T>.ReadCell` issued after the call sees the dispatched result, unconditionally. This is К-L7 atomic-from-observer in its simplest form.

Cross-document note for consumers: any description of field dispatches as "non-blocking" is wrong for this path — the sync path *blocks by contract* (the corpus rework corrects the FIELDS-side wording; N-24). Non-blocking behavior is exactly what the opt-in К-L7.1 path (§5.2) provides, at the price of one-tick lag.

### 5.2 К-L7.1 slot-tail reads (opt-in)

For pipeline-managed fields (§2.5), sim-thread reads bind to the slot tail: `df_pipeline_get_slot(slot_offset = -1)` returns the sim_tick − 1 snapshot without any per-read fence query (predicted ~30–50% `ReadCell` latency reduction on managed paths — a prediction, not yet a measurement, since no production consumer exists). К-L7 atomic-from-observer holds *within* a slot; cross-slot reads see different snapshots; the one-tick lag is bounded and deterministic. Coexistence is a standing law: К-L7 sync remains the default for every existing consumer; К-L7.1 is per-field author opt-in (К-L9).

### 5.3 Graphics-side frame synchronization

The production frame loop uses the smoke-test-proven pattern (`LauncherRenderer.cs:24-27,92-101`): one **shared `imageAvailable` semaphore**, **per-swapchain-image `renderFinished` semaphores** (a binary semaphore cannot be reused while still pending in present — the reason for per-image allocation), and a **frame fence** that the loop waits and resets every frame *before* any recreation decision (`LauncherRenderer.cs:175-179`) — guaranteeing GPU completion of the just-issued submission before the CPU proceeds (К-L7 atomic-from-observer, simple form). Submission waits `imageAvailable` at the color-attachment-output stage and signals the per-image semaphore consumed by present.

### 5.4 Device-wide waitIdle census

`vkDeviceWaitIdle` is deliberately rare. Every **production** call site at HEAD, exhaustively (the manual SmokeTest executable additionally device-idles between scenes): swapchain recreation, both triggers (`LauncherRenderer.cs:129,187`); renderer shutdown (`LauncherRenderer.cs:205`); runtime disposal (`Runtime.cs:446`). The tick path never device-idles — compute uses the per-dispatch fence (§5.1), graphics the per-frame fence (§5.3). The predecessor's phrasing "waitIdle … only used for save snapshots and shutdown" is corrected on both ends: **no save path exists to use it** (§7), and **recreation uses it** in addition to shutdown. The two shutdown-path calls (renderer, runtime) are, per the session shutdown audit (N-19), the only real quiesce waits in the entire process shutdown sequence — the process-wide shutdown gap is owned by [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) §2.6, not this document.

### 5.5 Why relaxed read visibility is acceptable (carried rationale)

For gameplay consumers on the sync path there is no in-flight window at all (§5.1). For future async/pipeline-managed consumers, a point read may see last-tick state — acceptable because field values are continuous and slow-changing; pawn systems read on per-tick cadence, so one-tick staleness is invisible; and the cliff-threshold rule (§4.3) is hysteresis-free, so brief inconsistencies don't cascade. Hard device-wide sync stays reserved for the §5.4 census sites.

---

## §6 Hardware policy and failure modes

### 6.1 Exclusion, not fallback

Not all hardware supports Vulkan 1.3 compute reliably, and pure software environments (CI, headless agents) may lack GPU access entirely. The shipped policy resolves this by **exclusion, not fallback**:

- **К-L19 fail-fast (shipped).** `Runtime.Create` runs `HardwareCapabilityCheck.Verify` at startup and throws `HardwareCapabilityException` if the Vulkan 1.3 + compute-queue tier is absent (§0.1). The tier exclusion is an accepted architectural choice — KERNEL_ARCHITECTURE.md Part 0, К-L19 row.
- **CPU reference kernels (shipped — test-oracle role, nothing else).** Each shipped compute shader has a managed CPU reference implementation, runnable without any GPU; this is how field logic is exercised on CI (§6.2).
- **Runtime CPU-fallback dispatch: design option, not wired.** No fallback dispatcher is on disk.

> **FENCED (target / planned — not current truth):** A config-selected per-tick CPU execution path for fields exists only as design. Reopening it (e.g., for a broader hardware audience) is a hardware-tier-*expansion* decision, not a bug fix — see [ROADMAP §Hardware tier expansion cascade](../ROADMAP.md). Nothing in the current substrate depends on its existence.

### 6.2 CPU reference kernels — the equivalence oracle

`IsotropicDiffusionKernel` and `AnisotropicDiffusionKernel` (`src/DualFrontier.Core.Interop/CpuKernels/`) are **the GPU equivalence oracle, not a performance target and not a fallback** — the framing is load-bearing and preserved verbatim from the code's own law: "this kernel exists as the GPU equivalence oracle, not as a performance target" (`IsotropicDiffusionKernel.cs:21-22`; same statement `AnisotropicDiffusionKernel.cs:27`). They serve three purposes and no others: (1) the V1 equivalence suites compare GPU output against them within tolerance on synthetic grids (`V1DiffusionEquivalenceTests`); (2) they exercise field logic in GPU-less environments; (3) they are the *designed* source of canonical field state for future save snapshots (§7.2 — that use is design, not implemented). They must not grow into a runtime dispatcher; that would be the §6.1 fenced decision by the back door.

### 6.3 Device-lost — unhandled, open

> **FENCED (open question — no current-truth handling exists):** **`VK_ERROR_DEVICE_LOST` is unspecified and unhandled.** The result code is defined (`src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs:14`) and there are **zero handlers repo-wide** — no code path, in substrate or Launcher, tests for or reacts to device loss; a lost device today surfaces as whatever `InvalidOperationException` the first failing wrapper throws, with undefined subsequent behavior. **Proposed v1 resolution (not ratified):** fail-fast with a user-facing diagnostic, consistent with the К-L19 startup posture — crash cleanly rather than render garbage — with device re-creation deferred as a separate epic. The fault-taxonomy row and the open question are owned by [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) §4 (class 6) and §6 (OQ-3) — ratified law as of EVT-2026-07-17-DRAFTS_RATIFICATION, with OQ-3 (fail-fast v1 vs device re-creation) still an open decision; deciding OQ-3, then amending this section to the decided behavior, closes the gap. Until then this substrate makes **no** claim about post-device-lost behavior.

### 6.4 Verification law and live risks

Two binding checks carried from the predecessor's methodology adjustments — they are substrate law, not optional process:

1. **Validation-clean output is a pre-commit check owned by this document.** Development runs with `VK_LAYER_KHRONOS_validation` on (DEBUG default — §2.1); the smoke-test protocols capture output through `ValidationLog`, and `ErrorCount == 0` is the exit criterion (§1.5).
2. **Every new compute shader ships with a CPU reference implementation and an equivalence test** on representative inputs within tolerance, before commit. Realized for V1; binds V2 and every subsequent shader.

Risk register, pruned to what is still live: **Vulkan complexity bugs** (synchronization, layout transitions, compute fence sync) remain high-probability and are mitigated by the two checks above plus RenderDoc/Nsight debugging; **scope creep into engine-building** remains the standing cultural risk, answered by the features-only-on-demand rule (§0.2); **compute determinism across hardware** is a live, accepted property with its save-time mitigation deliberately deferred (§7.2). Resolved-moot risks (binding tedium, PNG decoder edge cases, font tooling) are historical record. Required tooling in use since V0: LunarG Vulkan SDK for development/debugging (not for shader builds — §1.4), RenderDoc, VS 2022 17.8+ for `[LibraryImport]` source generators.

---

## §7 Save boundary and determinism

### 7.1 GPU-side save protocol (slot metadata — shipped; consumer — stub)

The GPU-side save law, per the shipped slot machinery (`pipeline_slot.h`, Item-34 block):

- **Snapshot boundary: display-tick state, `CurrentSimTick − D`.** The display thread already observes a coherent world at that tick, so **no pipeline drain is required at save time** — in-flight dispatches for newer ticks are simply not part of the snapshot. This is deliberately cheaper than drain-and-quiesce.
- **Pause protocol: natural convergence.** The sim thread completes its current tick and issues no new dispatch; pipeline depth absorbs already-dispatched work; the К-L18 quiescence check (`df_pipeline_is_quiescent` — §2.5) verifies the drained state before mod operations.
- **Load/resume: orderly refill** from the saved tick; display unblocks once D slots repopulate.
- **What serializes:** slot *metadata only* — depth header + per-slot `sim_tick` and `state` (4 + D×16 bytes max); the runtime pointers (`world_snapshot_ptr`, `fields_snapshot_ptr`, `compute_fence_handle`) are **never persisted** and regenerate on load via re-dispatch. `df_pipeline_serialize_display_state` / `df_pipeline_deserialize_display_state` ship on the C ABI.

**Consumer truth.** No production save exists to invoke any of this: managed persistence integration is deferred and `SaveSystem` is a stub (`src/DualFrontier.Core.Interop/PipelineSlotInterop.cs:202` records it verbatim). The field payload itself (which buffers serialize, resize policy, identity law) is [FIELDS.md](./FIELDS.md)'s domain; the cross-system snapshot boundary — including reconciling this section's "no drain at save" with any drain-at-save reading of К-L16 — is specified by [PERSISTENCE_SNAPSHOT_CONTRACT.md](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) §1, which this document defers to for everything above the GPU boundary.

### 7.2 Determinism posture

GPU compute results may vary across hardware/driver combinations (floating-point ordering, parallel reduction, driver optimization). The shipped posture: **realtime simulation does not require bit-exact determinism** (single-player, no replays); **save/load must produce reproducible state on load**; network multiplayer (unscoped) would require strict determinism and is not promised by this substrate. Determinism *classes* as corpus vocabulary are [TIME_AND_CONSISTENCY_MODEL.md](./TIME_AND_CONSISTENCY_MODEL.md)'s to define; this document only pins the substrate facts above.

> **FENCED (target / planned — not current truth):** **Canonical-CPU-state save mitigation — designed, explicitly not implemented** (no field save path exists at all): at save time, pause GPU dispatch and run one CPU-oracle iteration (§6.2) to produce canonical field state; serialize that; on load, restore fields from canonical state and resume GPU dispatch. This makes saves hardware-independent while leaving the realtime path free. Integration is sequenced with the persistence milestone — [PERSISTENCE_SNAPSHOT_CONTRACT.md](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) carries the invariant; [ROADMAP](../ROADMAP.md) carries the schedule. For hobby-scale single-player, slight cross-session non-determinism remains acceptable in the interim.

---

## §8 Open questions

The «stop, escalate, lock» rule applies: when implementation meets a design question not answered here, the response is *stop, record it here, wait for the lock* — never improvisation in code. Opening any item requires a brief and an amendment to §0/§1. Pruned to still-open at 2026-07-15:

| # | Decision | Trigger to resolve |
|---|---|---|
| OQ-V1 | Device-lost policy (fail-fast v1 vs device re-creation) — §6.3 | Ratification of the lifecycle draft's fault taxonomy (its OQ-3); then substrate amendment |
| OQ-V2 | Input routing contract (forwarding, ownership, ordering, queue bounding) + focus→pause wiring — §2.2 | Input-bridge cascade authoring |
| OQ-V3 | Swapchain recreation transaction (adopt prepare-before-reclaim protocol) — §2.3 | Ratification of the lifecycle draft §2.5; then substrate amendment |
| OQ-V4 | Present-queue-family selection (surface-aware device selection; today: require graphics-family present, F06) — §0.1 | First hardware report of a split graphics/present topology, or proactive hardening brief |
| OQ-V5 | Fence-poll integration (`df_pipeline_check_fences` stub → real `vkGetFenceStatus` wiring) — §2.5 | First pipeline-managed consumer (Phase.Compute activation) |
| OQ-V6 | Mod compute-pipeline registration surface (contract members, per-mod tracking, unload wiring) — §3 | M-V demonstration cascade; MOD_OS §4.3 amendment |
| OQ-V7 | Eikonal upgrade: V2 tunable vs separate primitive — §4.4 | Evidence-gated at V2 close (Option-B suboptimality measurement) |
| OQ-V8 | Domain B (entity-keyed bulk compute) substrate disposition — §4.1/§4.7 | Projectile-reactivation amendment authoring |
| OQ-V9 | Hybrid V1↔V2 broken-node coupling — §4.4 | Water demonstration amendment authoring |
| OQ-V10 | Navigation mode threshold N (~10 placeholder) — §4.6 | Movement-mod authoring (gameplay tuning) |
| OQ-V11 | Font system (bitmap vs TrueType) and UI architecture (retained vs immediate) — §1.6 | Text/UI brief authoring |
| OQ-V12 | Vulkan dispatch mechanism (`LibraryImport` vs `vkGetInstanceProcAddr`) — §2.1 | Post-foundation, if profiling demands |
| OQ-V13 | Atlas metadata format (code vs JSON/TOML) — §2.7 | When the atlas outgrows the procedural generator |
| OQ-V14 | Cross-platform support (SDL2 compromise vs manual X11/Cocoa) — §0.2 L7 | If/when an explicit cross-platform milestone opens |
| OQ-V15 | Editor scope | Post-rendering-completeness evaluation |

---

## Cross-references

| Document | Relation | Note |
|---|---|---|
| [KERNEL_ARCHITECTURE.md](./KERNEL_ARCHITECTURE.md) | defers-to | Part 0 invariant canon consumed here: К-L7/К-L7.1 (sync/slot-tail), К-L9 (vanilla = mods), К-L16 (pipeline depth), К-L17 (display composition), К-L18 (quiescence), К-L19 (hardware tier); kernel C ABI error conventions |
| [FIELDS.md](./FIELDS.md) | defers-to | Field storage layout, span lease + mutation rejection, string field-id law, field save payload; this doc owns only the GPU binding/dispatch of fields |
| [THREADING.md](./THREADING.md) | defers-to | Thread census and scheduler execution truth; §2.4 here adds only the merged window+render thread |
| [MOD_OS_ARCHITECTURE.md](./MOD_OS_ARCHITECTURE.md) | defers-to | Mod lifecycle and unload chain (Step 3.6 consumes §3.3), capability grammar, `IModApi` v3 law |
| [SCHEDULER_ARCHITECTURE.md](./SCHEDULER_ARCHITECTURE.md) | cites | Phase.Compute's position in the native tick lifecycle (scheduler-side view of §2.5) |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | cites | Layer map and project census; §1.3 Rule 1/2 mirror its dependency law |
| [EVENT_BUS.md](./EVENT_BUS.md) | cites | Fast-tier consumption by the fenced CombatFeedbackLayer (§2.6) |
| [PERFORMANCE.md](./PERFORMANCE.md) | cites | Sprite/tile and field-compute budgets |
| [ROADMAP](../ROADMAP.md) | defers-to (forward state) | Single authority for V2, M-V demonstrations, text/UI, input bridge, hardware-tier expansion, sequencing |
| [ENGINE_LIFECYCLE_AND_TRANSACTIONS.md](./ENGINE_LIFECYCLE_AND_TRANSACTIONS.md) | cites | Swapchain recreation transaction (§2.5 there ↔ §2.3 here); fault taxonomy incl. device-lost (§4/OQ-3 there ↔ §6.3 here); shutdown transaction (§2.6 there ↔ §5.4 note) |
| [PERSISTENCE_SNAPSHOT_CONTRACT.md](./PERSISTENCE_SNAPSHOT_CONTRACT.md) (AUTHORED draft) | cites | Cross-system snapshot boundary above the GPU line (§7) |
| [CONCURRENCY_AND_MEMORY_MODEL.md](./CONCURRENCY_AND_MEMORY_MODEL.md) | cites | Owner-thread rows for GPU/presentation objects; pipeline-slot operation matrix |
| [TIME_AND_CONSISTENCY_MODEL.md](./TIME_AND_CONSISTENCY_MODEL.md) | cites | Determinism-class vocabulary consumed by §7.2; visibility-table home for the §5 rules |
| [RESOURCE_OWNERSHIP_AND_LIFETIME.md](./RESOURCE_OWNERSHIP_AND_LIFETIME.md) | cites | Vulkan handle ownership/dispose-order rows (§2.1 teardown, §3.3 mod handles) |
| [EXECUTION_AUTHORITY_MATRIX.md](./EXECUTION_AUTHORITY_MATRIX.md) | cites | gpu-domain ownership row; cutover-gate discipline referenced for pipeline-managed activation (§2.5) |
| [FRAMEWORK.md](../governance/FRAMEWORK.md) | governance | Ratification path (§7), authority predicate (§14.7) |

Historical inputs (superseded; not linked per rework law): the predecessor `VULKAN_SUBSTRATE.md` v1.2.0 and, through it, `RUNTIME_ARCHITECTURE.md` v1.0 + `GPU_COMPUTE.md` v2.0 and the retired dual-backend-era specs. Q-G-1/Q-G-2 ratification provenance lives in the register and the historical corpus.

## Amendment protocol

Amendments follow the «stop, escalate, lock» discipline: open an §8 row (or cite an existing one), author a brief, obtain the lock, then amend the affected §§ in one commit with rationale. FENCED blocks convert to current-truth sections only with code on disk at the amending HEAD and re-verified `file:line` anchors. Version and lifecycle are owned by the document register; this file carries no self-pinned version claims.

## Change history

| Date | Change |
|---|---|
| 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R3-1..R3-8; register 0.1.0 → 0.1.1): slot-header anchor `:47-70`→`:45-64`; targets anchor split (`:23-26` copy + `:36-45` rewrite); compute-surface range →`:551-577`; §2.5 test-count claim re-anchored to current disk truth (7 dedicated interop tests; 35/14 = К10.3 v2 closure record); §5.4 waitIdle census scoped to production call sites; OQ-V6 pointer §4.6→§4.3 (successor map); cinzel wording → never-committed/git-ignored truth; §2.6 layer tokens corrected to registry-emitted observables (not manifest-declarable, MOD_OS §3.2). |
| 2026-07-15 | Authored as successor of DOC-A-VULKAN_SUBSTRATE (v1.2.0) per the corpus rework: current-truth/target separation via FENCED blocks; §3.4 field-id sketch corrected to the shipped string-id ABI; device-lost, input-discard, swapchain-protocol, queue-topology, waitIdle-census, unload-placeholder, and memory-budget truths pinned to HEAD `35364c2`; closure-evidence tables and migration narrative retired to the historical predecessor. |