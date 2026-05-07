---
title: Runtime Architecture
nav_order: 26
---

# Runtime Architecture — Dual Frontier

**Status:** AUTHORITATIVE LOCKED v1.0 — operational reference document. Every architectural decision in this document is final input to all subsequent migration milestones (M9.0–M9.8, see §2). Items marked **✓ LOCKED** reflect decisions taken during foundation deliberation; deviation in implementation requires reopening this document, not improvisation in code.

**Companion documents:** [METHODOLOGY](./METHODOLOGY.md), [CODING_STANDARDS](./CODING_STANDARDS.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [ARCHITECTURE](./ARCHITECTURE.md), [THREADING](./THREADING.md), [VISUAL_ENGINE](./VISUAL_ENGINE.md), [GODOT_INTEGRATION](./GODOT_INTEGRATION.md), [ROADMAP](./ROADMAP.md).

**Scope:** Full architectural specification + milestone roadmap для path от Godot Presentation к pure Vulkan-based 2D runtime. The Domain layer ([ARCHITECTURE §Domain](./ARCHITECTURE.md), [ECS](./ECS.md), [EVENT_BUS](./EVENT_BUS.md), [ISOLATION](./ISOLATION.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) is preserved verbatim by this migration — see L10 in §0.

**Version history:**

- v1.0 (2026-05-07, this version) — initial specification. Ten foundation decisions (L1–L10) locked. Nine implementation milestones (M9.0–M9.8) sequenced. Six open decisions (§4) deferred with stated triggers. Six risks (§5) registered with mitigations. Companion scaffolding generator committed at `tools/scaffold-runtime.ps1` materializes the folder hierarchy + contracts described in §1.

---

## Preamble — How to use this document

**Authority.** This document is the single architectural authority for the Dual Frontier runtime layer (window management, GPU rendering, input, sprite batching, text, asset loading, diagnostic tooling). During implementation of milestones M9.0 through M9.8, every interface, P/Invoke declaration, struct layout, and lifecycle step traces back to a section here. Disagreement with the specification is escalated to the human (via §4 open decisions) — never resolved by improvisation in code, mirroring the discipline established for the modding layer in [MOD_OS_ARCHITECTURE Preamble](./MOD_OS_ARCHITECTURE.md).

**Scope.** The specification governs:

- The structural relationship between the runtime layer and the existing four layers ([ARCHITECTURE](./ARCHITECTURE.md)).
- The Win32 + Vulkan P/Invoke surface (functions, structs, enums, callbacks).
- The Vulkan resource lifetimes (instance, device, swapchain, pipeline, buffer, image, memory).
- The sprite + text + atlas batching model.
- The PNG decoder and shader compilation pipeline.
- The threading model on top of [THREADING](./THREADING.md) (window+render thread merged with simulation thread preserved).
- The migration sequencing from the current dual-backend Godot+Silk.NET state ([VISUAL_ENGINE](./VISUAL_ENGINE.md), [GODOT_INTEGRATION](./GODOT_INTEGRATION.md)) к the locked Vulkan target.

The specification does **not** govern:

- Domain content — Domain is preserved verbatim, see [ARCHITECTURE](./ARCHITECTURE.md), [ECS](./ECS.md), [CONTRACTS](./CONTRACTS.md), [EVENT_BUS](./EVENT_BUS.md).
- The mod system — covered by [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [MODDING](./MODDING.md), [MOD_PIPELINE](./MOD_PIPELINE.md). Runtime exposes presentation primitives only; mods cannot reach the runtime layer directly.
- Game-design questions (balance, narrative, pacing).
- Methodology of the development pipeline — covered by [METHODOLOGY](./METHODOLOGY.md), with M9.x adjustments noted in §7.

**The "stop, escalate, lock" rule.** When implementation encounters a design question not answered here, the response is "stop, document in §4, wait for the human to lock" — not "guess." Same discipline as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) Preamble.

---

## Executive summary

Dual Frontier migrates от Godot 4 + C# к custom Vulkan-based 2D runtime. The Domain layer ([ARCHITECTURE](./ARCHITECTURE.md), [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md), [THREADING](./THREADING.md), [ISOLATION](./ISOLATION.md)) is preserved verbatim — zero touch by migration. Only the Presentation layer ([VISUAL_ENGINE](./VISUAL_ENGINE.md), [GODOT_INTEGRATION](./GODOT_INTEGRATION.md)) is rewritten on the new foundation.

**Foundation philosophy** — «без компромиссов»:

- Pure P/Invoke к `vulkan-1.dll` (no third-party C# binding library).
- Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`).
- BCL only for math (`System.Numerics`) и compression (`System.IO.Compression.DeflateStream`).
- Manual PNG decoder (DEFLATE through BCL, chunk parsing manual).
- Build-time GLSL → SPIR-V via `glslangValidator.exe`.
- Production binary depends only on `vulkan-1.dll` (GPU driver) и pre-compiled `.spv` shader files.

Total ownership: every line above OS API surface is project's own code.

**Estimated scope:** 4–7 weeks at hobby pace (~1h/day) к full M8.x parity на Vulkan ([ROADMAP](./ROADMAP.md) M8 closure → M9 entry).

---

## 0. Locked foundational decisions

The following decisions are committed как architectural foundation. Departures require an explicit re-architecture milestone, not spec-level adjustments mid-implementation. The locked-decision protocol mirrors [MOD_OS_ARCHITECTURE §0](./MOD_OS_ARCHITECTURE.md).

| #   | Decision               | Choice                                                                           | Rationale                                                            |
| --- | ---------------------- | -------------------------------------------------------------------------------- | -------------------------------------------------------------------- |
| L1  | GPU API                | Vulkan 1.3                                                                       | Future-proof, total control, modern GPU pipeline understanding       |
| L2  | Vulkan bindings        | Pure P/Invoke к `vulkan-1.dll`                                                   | Zero third-party C# в production binary                              |
| L3  | Window/OS surface      | Pure Win32 P/Invoke (`user32.dll`, `kernel32.dll`)                               | Same — zero third-party for OS surface                               |
| L4  | Math                   | `System.Numerics` (BCL only)                                                     | BCL is .NET runtime surface, not third-party                         |
| L5  | PNG loading            | Manual decoder + `System.IO.Compression.DeflateStream` (BCL)                     | DEFLATE is BCL, chunk parsing manual ~500 lines                      |
| L6  | Shader strategy        | Build-time GLSL → SPIR-V via `glslangValidator.exe`                              | Production binary has no shader compiler dependency                  |
| L7  | Initial platform       | Windows-only                                                                     | Cross-platform deferred — adds SDL2/GLFW dep or manual X11/Cocoa     |
| L8  | Threading              | Window+Render thread merged + Simulation thread (existing GameLoop preserved)    | Minimal change to domain; see [THREADING](./THREADING.md)            |
| L9  | Migration approach     | Parallel — keep Godot Presentation functional до M9.5 cutover                    | Honest state always available                                        |
| L10 | Domain layer treatment | Preserved verbatim — zero modification                                           | Mature ECS proven, не throwing away 472 tests + simulation work      |

**Implication of L7.** Project becomes Windows-only до явного cross-platform milestone. macOS/Linux support deferred indefinitely (или through SDL2 layer accepted as «pragmatic compromise» if needed). See §4 open decision «Cross-platform support».

**Implication of L10.** All existing namespaces under `DualFrontier.Core`, `DualFrontier.Contracts`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Systems`, `DualFrontier.Application`, `DualFrontier.Modding`, `DualFrontier.Persistence` are untouched. The 472 existing tests pass throughout migration ([TESTING_STRATEGY](./TESTING_STRATEGY.md)). Mod system contracts ([MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)) remain unchanged — runtime is not visible from a mod's `AssemblyLoadContext`.

---

## 1. Architecture

### 1.1 Project structure (post-migration target)

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

  // ====== Runtime stack (NEW — Vulkan + Win32 foundation) ======
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
      VulkanInstance.cs                       // VkInstance lifecycle
      VulkanDevice.cs                         // physical/logical device
      VulkanSurface.cs                        // VkSurfaceKHR (Win32)
      VulkanSwapchain.cs                      // swapchain + recreation
      VulkanCommandPool.cs                    // command pool/buffer mgmt
      VulkanRenderPass.cs                     // render pass abstraction
      VulkanPipeline.cs                       // graphics pipeline state
      VulkanBuffer.cs                         // VkBuffer + memory allocation
      VulkanImage.cs                          // VkImage + memory + view
      VulkanShaderModule.cs                   // SPIR-V shader loading
      ValidationLayer.cs                      // debug messenger setup
      MemoryAllocator.cs                      // VkDeviceMemory bumper allocator

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

  // ====== Adapter (REWRITE от Godot к Runtime) ======
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
  DualFrontier.Tests/                         // existing 472 tests (unchanged)
  DualFrontier.Runtime.Tests/                 // new — non-GPU runtime tests
    MODULE.md
    Assets/PngDecoderTests.cs
    Sprite/{Camera2DTests.cs, SpriteBatcherTests.cs}
    Input/InputEventQueueTests.cs

tools/
  shaders/                                    // GLSL source files
    sprite.vert / sprite.frag / text.vert / text.frag
  glslangValidator.exe                        // Khronos shader compiler (build-time)
  scaffold-runtime.ps1                        // generator (committed; v1.0 of this doc)

assets/
  kenney/                                     // existing PNG atlas (preserved)
    terrain/roguelikeSheet_transparent.png
  shaders/                                    // pre-compiled SPIR-V output
    sprite.vert.spv / sprite.frag.spv / text.vert.spv / text.frag.spv
  fonts/
    default.png / default.fnt                 // glyph metrics file

mods/                                         // unchanged — see MOD_OS_ARCHITECTURE
  Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}/

docs/
  METHODOLOGY.md / CODING_STANDARDS.md / ARCHITECTURE.md
  THREADING.md / VISUAL_ENGINE.md / GODOT_INTEGRATION.md
  MOD_OS_ARCHITECTURE.md / ROADMAP.md
  RUNTIME_ARCHITECTURE.md                     // THIS DOCUMENT
```

The scaffolding in `tools/scaffold-runtime.ps1` materializes this hierarchy mechanically; see commit `81fea13`.

### 1.2 Module purposes

#### `DualFrontier.Runtime` (top-level)

**Purpose:** Generic 2D runtime — window management, Vulkan rendering primitives, sprite batching, texture loading, input events, UI primitives. Knows nothing of Domain. Could be open-sourced separately.

**Public API surface:** `Runtime.cs` facade exposes:

- Window creation
- Sprite registration
- Frame submission
- Input event polling

**Dependencies:** `System` (BCL), `System.Numerics`, `System.IO.Compression`. No third-party. Compare к the strict layering rules of [ARCHITECTURE](./ARCHITECTURE.md).

#### `DualFrontier.Runtime.Native.Win32`

**Purpose:** Pure P/Invoke к Win32 API. `[LibraryImport]` declarations для window management, message pump, input handling.

**Public API surface:** `internal` к Runtime. Win32 surface не leaks beyond Runtime project.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Native.Vulkan`

**Purpose:** Pure P/Invoke к Vulkan API. `[LibraryImport]` declarations + struct definitions + enums per Vulkan 1.3 spec.

**Public API surface:** `internal` к Runtime. Vulkan surface не leaks beyond Runtime project.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Window`

**Purpose:** High-level window abstraction. Hides Win32 details. Provides lifecycle (create/show/destroy), event subscription, input event delivery.

**Public API surface:** `IWindow`, `Window`, `WindowOptions`. Replaces the [VISUAL_ENGINE](./VISUAL_ENGINE.md) `IRenderer` initialization path on the new foundation.

**Dependencies:** `Native.Win32`, `Input`.

#### `DualFrontier.Runtime.Input`

**Purpose:** Typed input events + event queue. Events posted by Window, consumed by clients via polling. Supersedes the Godot `IInputSource` adapter ([VISUAL_ENGINE](./VISUAL_ENGINE.md) §Contracts) once M9.5 cutover lands.

**Public API surface:** `IInputEvent` + concrete event types + `InputEventQueue`.

**Dependencies:** `System` (BCL).

#### `DualFrontier.Runtime.Graphics`

**Purpose:** Vulkan rendering primitives — instance, device, swapchain, render pass, pipeline, buffer, image, memory allocator. Direct wrappers around Vulkan API c idiomatic C# lifetimes (`IDisposable` patterns).

**Public API surface:** `VulkanInstance`, `VulkanDevice`, etc. — used by `Sprite`, `Text`, `Diagnostic`.

**Dependencies:** `Native.Vulkan`, `Window` (для surface creation).

#### `DualFrontier.Runtime.Sprite`

**Purpose:** 2D sprite rendering — atlas-based, batched draw calls. `Camera2D` для orthographic projection.

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

**Purpose:** Performance + debug tooling — FPS measurement, debug overlay, validation log capture. Targets и budgets are governed by [PERFORMANCE](./PERFORMANCE.md).

**Public API surface:** `FrameTimer`, `DebugOverlay`, `ValidationLog`.

**Dependencies:** `Sprite`, `Text`, `Graphics`.

#### `DualFrontier.Presentation` (rewritten adapter)

**Purpose:** Bridge от Domain к Runtime. Translates `RenderCommands` (от existing `PresentationBridge`, see [GODOT_INTEGRATION](./GODOT_INTEGRATION.md)) к Runtime API calls. Owns «what to draw» logic.

**Public API surface:** `Program.Main()`, `GameRoot`. Internal classes wire bridge к Runtime.

**Dependencies:** `DualFrontier.Runtime`, `DualFrontier.Application`, `DualFrontier.Events`, `DualFrontier.Components`.

### 1.3 Threading model

The runtime extends [THREADING](./THREADING.md) — domain `ParallelSystemScheduler` and tick rate are unchanged; the runtime contributes a single render thread merged with the OS message pump (per L8).

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
│  │   ↓ writes к PresentationBridge.Enqueue()          │      │
│  │   ↑ reads input events from InputEventQueue        │      │
│  │ SetPaused() coupling от Main thread                │      │
│  └────────────────────────────────────────────────────┘      │
│                                                              │
│  Worker Threads (existing — used by ParallelSystemScheduler) │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

**Cross-thread synchronization:**

- `PresentationBridge` (existing `ConcurrentQueue<IRenderCommand>`, see [GODOT_INTEGRATION](./GODOT_INTEGRATION.md)) preserved as primary domain → render channel.
- New: `InputEventQueue` (`ConcurrentQueue<IInputEvent>`) for render → domain input events.
- Pause coupling: Main thread обнаруживает focus loss, calls `loop.SetPaused(true)`. Domain thread sleeps. Pattern proven from M8.10 (focus notifications) — extended Vulkan way (Win32 `WM_KILLFOCUS`/`WM_SETFOCUS` messages — clean semantics, no `tree.paused` surprises).

### 1.4 Dependency rules (locked invariants)

These rules are mechanically verifiable; the build fails if any is violated. They mirror the dependency-direction discipline of [ARCHITECTURE §Four layers](./ARCHITECTURE.md).

**Rule 1.** `DualFrontier.Runtime` MUST compile и tests pass without any reference к Domain projects. Verify via project reference graph.

**Rule 2.** Domain ↔ Runtime communication ONLY through `DualFrontier.Presentation` adapter. Domain knows nothing of Runtime; Runtime knows nothing of Domain.

**Rule 3.** Within Runtime, dependency direction respects layering:

```
Native.Win32 / Native.Vulkan  (lowest)
    ↓
Window / Input / Assets
    ↓
Graphics
    ↓
Sprite / Text
    ↓
Diagnostic
    ↓
Runtime.cs (facade — top)
```

**Rule 4.** No layer skipping (Diagnostic не imports Native.Vulkan directly; goes через Graphics).

**Rule 5.** Runtime exposes minimal public API. Internal implementation details `internal`. Naming follows [CODING_STANDARDS](./CODING_STANDARDS.md).

### 1.5 Native interop patterns

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

**Recommendation:** start с Option A (direct `LibraryImport`) для M9.0–M9.4. Migrate к canonical procedure-address loading (`vkGetInstanceProcAddr` dispatch) в M9.5+ if profiling demands. See §4 «Vulkan dispatch».

### 1.6 Asset pipeline

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

### 1.7 Shader strategy

**Build-time SPIR-V compilation** via MSBuild target в `Directory.Build.props`:

```xml
<Target Name="CompileShaders" BeforeTargets="Build"
        Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\sprite.vert -o $(SolutionDir)assets\shaders\sprite.vert.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\sprite.frag -o $(SolutionDir)assets\shaders\sprite.frag.spv" />
</Target>
```

**Production binary depends on:** `vulkan-1.dll` + pre-compiled `*.spv` files. No shader compiler dependency.

### 1.8 Testing strategy

The existing 472 tests are preserved verbatim — Domain layer is untouched (L10). [TESTING_STRATEGY](./TESTING_STRATEGY.md) governs the test pyramid; Runtime additions slot in as new unit-test categories without altering the pyramid shape.

**New tests in `DualFrontier.Runtime.Tests`** (non-GPU, JIT-runnable):

- `PngDecoder` tests (~20–30): synthetic PNG inputs → expected RGBA output.
- `SpriteBatcher` logic tests (~10): batch sorting, atlas grouping, vertex math.
- `Camera2D` math tests (~10): orthographic projection, screen ↔ world conversion.
- `InputEventQueue` tests (~5): cross-thread enqueue/dequeue semantics.

**GPU-dependent tests:** F5 manual visual verification per established M8.8/M8.9 protocol. Validation layer output captured к console — clean output is success criterion. The protocol mirrors the [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) red-flag checklist for visual changes.

**Goal post-M9.8:** ~520 total tests (472 + new).

### 1.9 Naming conventions

Continued from [CODING_STANDARDS](./CODING_STANDARDS.md):

- All identifiers English (Russian glossary unchanged — see [TRANSLATION_GLOSSARY](./TRANSLATION_GLOSSARY.md)).
- Vulkan struct types: keep canonical `VkInstanceCreateInfo` naming (matches Vulkan spec).
- Win32 struct types: keep canonical `WNDCLASSEX` naming (matches Win32 docs).
- Pascal case для C# wrapper classes: `VulkanInstance`, `Win32Window`.
- Internal P/Invoke artifacts: `internal` access modifier.
- Public Runtime API: `public` access modifier, idiomatic C# names.

---

## 2. Roadmap

### 2.1 Master plan

The milestones extend [ROADMAP](./ROADMAP.md). The numeric range M9.0–M9.8 is reserved for the Vulkan migration; phases above M10 remain reserved for the post-migration topics in §4.

| Milestone | Title                                                | Estimated time | LOC delta            |
| --------- | ---------------------------------------------------- | -------------- | -------------------- |
| M9.0      | Foundation: Win32 window + Vulkan clear color        | 4–5h           | +800–1000            |
| M9.1      | First textured quad: PNG → VkImage → sprite render   | 5–7h           | +1000–1500           |
| M9.2      | Batched sprite renderer                              | 4–6h           | +500–700             |
| M9.3      | TileMap parity + Camera2D                            | 3–4h           | +400–600             |
| M9.4      | Input system (Win32 → InputEventQueue)               | 3–4h           | +400–500             |
| M9.5      | Domain integration (Presentation port)               | 6–8h           | +800–1200            |
| M9.6      | UI primitives (text + panels)                        | 8–12h          | +1000–1500           |
| M9.7      | Coupled lifecycle + DebugOverlay                     | 2–3h           | +200–300             |
| M9.8      | Migration cutover (delete Godot)                     | 2–3h           | -2000+ (deletion)    |

**Cumulative:** ~37–52 hours = **4–7 weeks at hobby pace** (~1h/day).

### 2.2 M9.0 — Foundation: Win32 window + Vulkan clear color

**Goal:** empty window opens, Vulkan initializes correctly, clear color renders каждый frame, no validation errors.

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

**Success criteria:**

- Window opens, sizable, closeable.
- Clear color rendered every frame.
- FPS measurable.
- Validation layers report zero errors in DEBUG mode.
- Clean shutdown (no leaked Vulkan handles per validation).

**Time:** 4–5 hours. **LOC:** ~800–1000.

### 2.3 M9.1 — First textured quad

**Goal:** Kenney pawn sprite rendered at center of window.

**Deliverables:** `PngDecoder` full implementation + `VulkanBuffer`/`VulkanImage` abstractions + Pipeline + RenderPass + sprite shaders compiled к SPIR-V + single sprite render.

**Success criteria:** Kenney pawn sprite displayed centered. PNG decoder tests passing.

**Time:** 5–7 hours. **LOC:** ~1000–1500.

### 2.4 M9.2 — Batched sprite renderer

**Goal:** 10 000 sprites rendered at 60+ FPS via single draw call.

**Deliverables:** Dynamic vertex buffer + per-sprite vertex data + atlas-shared batching + stress test.

**Success criteria:** 10 000 sprites at 60+ FPS. Single draw call в RenderDoc. [PERFORMANCE](./PERFORMANCE.md) budget for sprite pass adopted.

**Time:** 4–6 hours. **LOC:** ~500–700.

### 2.5 M9.3 — TileMap parity + Camera2D

**Goal:** 200 × 200 tile grid rendered, camera pannable, full M8.8 visual parity.

**Deliverables:** `TileMapBatch` + `Camera2D` + atlas regions для terrain.

**Success criteria:** 200 × 200 tile map visible. 60+ FPS sustained (was 17 на Godot).

**Time:** 3–4 hours. **LOC:** ~400–600.

### 2.6 M9.4 — Input system

**Goal:** keyboard и mouse events from Win32 delivered к domain.

**Deliverables:** `InputEventQueue` + event types + Win32 message handler dispatching. Replaces the Godot `InputRouter` ([VISUAL_ENGINE](./VISUAL_ENGINE.md)) for new code path; Godot path stays alive до M9.8.

**Success criteria:** smooth camera pan, key bindings work.

**Time:** 3–4 hours. **LOC:** ~400–500.

### 2.7 M9.5 — Domain integration (Presentation port)

**Goal:** full M8.9 visual parity на Vulkan stack.

**Deliverables:** Rewrite Presentation layer к target Runtime API. `PawnVisual` / `ItemVisual` / `TileMapVisual` на sprite handles. `RenderCommandDispatcher` retargeted (existing pattern from [GODOT_INTEGRATION](./GODOT_INTEGRATION.md)).

**Success criteria:** 50 pawns + 255 items + terrain. 60+ FPS. 472 domain tests passing ([TESTING_STRATEGY](./TESTING_STRATEGY.md) gate).

**Time:** 6–8 hours. **LOC:** ~800–1200 (rewrite).

### 2.8 M9.6 — UI primitives

**Goal:** full HUD parity — `ColonyPanel` + `PawnDetail` + `DebugOverlay` на Runtime UI.

**Deliverables:** `BitmapFont` + `TextRenderer` + Panel/Label/ProgressBar primitives + `ColonyPanel` + `PawnDetail` port.

**Success criteria:** HUD matches M8.9 visual.

**Time:** 8–12 hours. **LOC:** ~1000–1500.

### 2.9 M9.7 — Coupled lifecycle + DebugOverlay

**Goal:** parity с M8.10 — focus events couple к loop pause, no desync drift possible.

**Deliverables:** `WM_KILLFOCUS`/`WM_SETFOCUS` hooks + `WM_SIZE` swapchain recreation + `DebugOverlay`.

**Success criteria:** alt-tab pauses simulation. Window resize works. No 298-tick desync.

**Time:** 2–3 hours. **LOC:** ~200–300.

### 2.10 M9.8 — Migration cutover

**Goal:** Godot completely removed. Only Vulkan stack running.

**Deliverables:** Delete `.godot/`, `project.godot`, `*.import`, Godot-specific gitignore entries. Update `tools/build-all.ps1`. Update `README.md`.

**Success criteria:** `dotnet build` clean без Godot. `grep -r godot` returns empty. `dotnet run` launches game. [GODOT_INTEGRATION](./GODOT_INTEGRATION.md) marked deprecated; superseded by §1.2 of this document.

**Time:** 2–3 hours. **LOC:** -2000+ (net deletion).

---

## 3. Migration strategy

**Approach: parallel development.**

Keep `DualFrontier.Presentation` (Godot) functional через М9.5. New Runtime project develops в parallel. M9.5 cutover migrates Presentation к Runtime. M9.8 deletes Godot remnants.

**Operating principle:** «honest state always available». Не blind period где game не runs. The same discipline as the parallel mod-system migration in [MOD_OS_ARCHITECTURE §11](./MOD_OS_ARCHITECTURE.md) — every phase ends with a runnable build.

---

## 4. Decisions log

### Resolved (locked — see §0)

L1–L10 above.

### Open (deferred)

The «stop, escalate, lock» rule applies; opening any item below requires a brief and an amendment к §0. Same protocol as [MOD_OS_ARCHITECTURE §12](./MOD_OS_ARCHITECTURE.md).

| Decision                                                  | Trigger to resolve                          |
| --------------------------------------------------------- | ------------------------------------------- |
| Editor scope                                              | Post-M9.8 evaluation                        |
| Cross-platform support                                    | If/when needed                              |
| Font system (bitmap vs TrueType)                          | M9.6 brief authoring                        |
| UI architecture (retained vs immediate-mode)              | M9.6 brief authoring                        |
| Vulkan dispatch (`LibraryImport` vs `vkGetInstanceProcAddr`) | M9.5+ if profiling demands                |
| Atlas metadata format (code vs JSON/TOML)                 | M11+ when atlas grows                       |

---

## 5. Risk register

**R1 — Pure P/Invoke binding tedium exceeds tolerance.**

- Probability: Medium.
- Mitigation: if grinding feels unbearable after ~1000 lines, switch к Vortice.Vulkan (lateral move). Decision recorded as a §4 amendment.

**R2 — Vulkan complexity bugs (synchronization, layout transitions).**

- Probability: High.
- Mitigation: validation layers ALWAYS on в development. RenderDoc для visual debugging. Validation-clean output added to the [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) checklist.

**R3 — PNG decoder edge cases.**

- Probability: Medium.
- Mitigation: extensive test suite с synthetic + real PNG inputs ([TESTING_STRATEGY](./TESTING_STRATEGY.md) §unit).

**R4 — Bitmap font tooling bottleneck.**

- Probability: Medium.
- Mitigation: external tool (BMFont) once for default font.

**R5 — Scope creep into building «engine» instead of game.**

- Probability: High.
- Mitigation: «features only on demand» principle. Each Runtime feature must trace к specific Domain requirement. Same discipline as [MOD_OS_ARCHITECTURE Preamble](./MOD_OS_ARCHITECTURE.md) — architectural strength depends on the spec being the only source of truth.

**R6 — Vulkan learning curve disrupts pace.**

- Probability: Low–Medium.
- Mitigation: Vulkan tutorial materials extensive (Vulkan Tutorial, Sascha Willems samples).

---

## 6. Operational considerations

**Required tooling — install before M9.0:**

- Vulkan SDK (LunarG, current 1.3.x).
- RenderDoc (graphics debugger).
- Visual Studio 2022 17.8+ (for `[LibraryImport]` source generators).

**Optional:**

- BMFont — bitmap font generator (M9.6).
- AssetForge / TexturePacker — atlas tooling (future).

The scaffolding generator `tools/scaffold-runtime.ps1` is committed and idempotent; running it materializes the §1.1 hierarchy without touching Domain.

---

## 7. Methodology adjustments для M9.x

The existing methodology ([METHODOLOGY](./METHODOLOGY.md)) carries forward с the following adjustments. None of these adjustments alter the four-agent pipeline shape; they extend its pre-flight + verification stages для the Vulkan-specific failure modes.

**Pre-flight checks adapted:**

- Write-conflict table — applies к Domain commits, не Runtime.
- Project reference direction sanity check — extended: Runtime may not reference Domain (§1.4 Rule 1).
- New: **Validation layer output check** — clean validation output mandatory before commit. Added to [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) checklist.

**Brief structure:**

- M-phase boundary check expanded: Runtime / Domain / Mods / Tests boundaries (compare к the boundaries defined in [ARCHITECTURE](./ARCHITECTURE.md) и [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md)).
- Visual verification protocol primary (precedent от M8.8 / M8.9 extended).
- Falsification clauses include Vulkan-specific edge cases (layout transitions, swapchain re-creation, validation messages).

**Operating principle continues:**

- «Data exists or it doesn't» applies к Vulkan resources.
- New corollary: «State exists или driver crashes» — Vulkan demands explicit state.
- AD numbering continues sequence.

---

## Closing notes

3 weeks к current Dual Frontier state demonstrates high learning velocity, architectural rigor, methodology effectiveness. Vulkan from-scratch within 4–7 weeks is comparable к existing pace.

**«Features only on demand»:** Vulkan API surface is enormous. Resist temptation к build «complete» renderer. Each feature must trace к specific Domain requirement.

This document is **v1.0**, authoritative until amended via explicit decision. Amendments require a commit с rationale, в the same style as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) version-history block.

---

## See also

- [METHODOLOGY](./METHODOLOGY.md) — the development pipeline; the M9.x adjustments in §7 keep this architecture inside the same methodology.
- [CODING_STANDARDS](./CODING_STANDARDS.md) — naming, file-scoped namespaces, nullable, member order; Runtime adheres verbatim.
- [ARCHITECTURE](./ARCHITECTURE.md) — the four layers; Runtime extends the Presentation layer без touching the others.
- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — companion architectural authority for the modding subsystem; version-history protocol and «stop, escalate, lock» rule follow that document.
- [THREADING](./THREADING.md) — domain `ParallelSystemScheduler`; the Window+Render thread merge in §1.3 is the only addition.
- [VISUAL_ENGINE](./VISUAL_ENGINE.md) — current dual-backend (Godot DevKit + Silk.NET Native); superseded for production by this document at M9.8.
- [GODOT_INTEGRATION](./GODOT_INTEGRATION.md) — current `PresentationBridge` and Godot-specific glue; deprecated at M9.8.
- [ROADMAP](./ROADMAP.md) — phase ordering; §2 of this document is the authoritative sequence for the runtime work.
- [TESTING_STRATEGY](./TESTING_STRATEGY.md) — test pyramid; §1.8 slots Runtime tests into the existing structure.
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — pre-commit checklist; §7 adds the validation-layer-clean check.
- [PERFORMANCE](./PERFORMANCE.md) — target metrics; sprite/tile budgets adopted in M9.2 / M9.3.

## Part 9: Relationship к KERNEL_ARCHITECTURE.md

RUNTIME_ARCHITECTURE.md (this) и KERNEL_ARCHITECTURE.md describe two halves of single architectural vision: native foundation под managed Application layer.

**Symmetric architecture**:
- This document (RUNTIME): Vulkan rendering layer, M-series milestones (M9.0-M9.8)
- KERNEL_ARCHITECTURE.md: ECS kernel layer, K-series milestones (K0-K8)
- Both: pure P/Invoke к native (vulkan-1.dll | DualFrontier.Core.Native.dll)
- Both: managed thin adapter layer
- Both: single ownership boundary, direction-disciplined (managed → native)

**Independent layers**: rendering знает nothing about ECS storage; ECS kernel знает nothing about Vulkan. Both reachable от managed Application layer through respective bridges.

**Combined timeline**: see KERNEL_ARCHITECTURE.md Part 8 для sequencing options (β3, β5, β6) и recommended approach.

**Cross-document invariants**: «без компромиссов», operating principle (data exists/doesn't), single ownership boundary, direction-discipline, long-horizon planning. See KERNEL_ARCHITECTURE.md Part 8 для full invariant list.
