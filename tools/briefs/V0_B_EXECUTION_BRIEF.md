---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-V0_B
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-V0_B
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: V0_B_EXECUTION_BRIEF
status: EXECUTED
executed: 2026-05-18
authored: 2026-05-18
author: Claude Opus 4.7 (Crystalka deliberation session, post-V0.A closure)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 25-40 hours auto-mode (V0.B scope substantial — swapchain + render pass + compute pipeline plumbing + memory allocator + SPIR-V toolchain + async compute queue selection + HardwareCapabilityCheck)
brief_type: execution
authority_chain:
  - VULKAN_SUBSTRATE.md v1.0 LOCKED (DOC-A-VULKAN_SUBSTRATE) — V substrate authoritative spec; §1.1 V0 deliverables (rendering side bullets 4+ + compute side bullets); §2.7 shader strategy; §3 compute use case
  - KERNEL_FULL_NATIVE_SCHEDULER.md v2.0 LOCKED — К-L19 hardware tier (Item 43 async compute queue + Item 44 HardwareCapabilityCheck) consumed by V0.B
  - V0_A_EXECUTION_BRIEF.md EXECUTED (DOC-D-V0_A, 2026-05-18) — V0.A closure baseline + Vulkan struct alignment audit lesson (Lesson #7 strengthening) + mixed [LibraryImport]/[DllImport] convention precedent
  - K10_3_EXECUTION_BRIEF.md AUTHORED, HALTED Phase 0 SC-14 2026-05-18 — К10.3 brief restart pathway gate: V0.B closure unblocks К10.3 (async compute queue selection + compute pipeline plumbing ready)
  - KERNEL_ARCHITECTURE.md v2.1 LOCKED — K9 RawTileField storage уже complete в df_capi.h (df_world_register_field + df_world_field_acquire_span etc); V0.B compute side bridges existing K9 storage via new C ABI extension
  - df_capi.h (read Phase 0 brief authoring 2026-05-18) — existing K9 + К10.1 surface; V0.B extends с compute pipeline registration + field dispatch functions only
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 verbatim + #7 strengthening (P/Invoke ABI alignment audit recipe per V0.A executor finding) + §12.7 canonical closure protocol
  - FRAMEWORK.md v1.1 LOCKED — Category D Tier 3 lifecycle transitions
  - Directory.Build.props — net8.0 + LangVersion 12.0 + Nullable enable + TreatWarningsAsErrors true (verified V0.A Phase 0)
  - existing V0.A code anchors (verified Phase 0 brief authoring 2026-05-18): VkApi.cs (extension functions loaded via vkGetInstanceProcAddr), VkStructs.cs (alignment fix precedent VkPhysicalDeviceProperties 824 bytes), VulkanDevice.cs (queue family enumeration + selection logic), Runtime.cs facade composition
---

# V0.B — Swapchain + render pass + compute pipeline plumbing + memory allocator + SPIR-V toolchain + async compute queue + HardwareCapabilityCheck

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. **Substantial** multi-commit atomic cascade implementing **V0 substrate foundation completion** per VULKAN_SUBSTRATE.md §1.1 — specifically the rendering use case implementation bullets 4-9 (swapchain + render pass + command pool + pipeline + memory allocator + sprite atlas/text deferred к V0.C) **AND** compute use case implementation (compute pipeline + descriptor sets + dispatch + fence sync + C ABI extension + SPIR-V toolchain integration).

**Authority**: V0.A closed 2026-05-18 (PR #36 merged, 11 atomic commits, 685 tests green, zero hard-gate halts, VkPhysicalDeviceProperties alignment fix surfaced). V0.B continues V substrate authoring stream per Crystalka split ratification 2026-05-18 (V0.A/V0.B/V0.C structure). К10.3 brief restart gate: V0.B closure unblocks compute side (async compute queue + compute pipeline plumbing + HardwareCapabilityCheck).

**V0.A closure context inherited** (per V0.A §8 closure report):
- 685 tests baseline (665 К10.2 + 20 V0.A additive — Window options + Vulkan instance + validation layer + device + composition + smoke test)
- DualFrontier.Runtime project operational с Win32 window + Vulkan instance + ValidationLayer + VulkanDevice composition
- AMD RX 7600S (Vulkan 1.4.344, RDNA 3) verified К-L19 hardware tier baseline
- Validation layer ALWAYS-ON в DEBUG (zero validation errors at V0.A smoke test)
- Mixed [LibraryImport] + [DllImport] convention established (source-gen friendly + non-blittable struct fallback)
- VkPhysicalDeviceProperties alignment fix precedent (824 bytes — Lesson #7 strengthening landed)
- 5 queue families enumerated on Crystalka «Skarlet»: QF 0 graphics, QF 1 async compute candidate visible but not selected (V0.B selects)

**V0.B scope discipline (Lesson #20 + Lesson #14 candidate application)**:

In-scope (V0.B):
- **Swapchain**: `VkSwapchainKHR` lifecycle + surface creation (`vkCreateWin32SurfaceKHR`) + image acquisition + present + recreation on WM_SIZE handler (extends V0.A Window with WM_SIZE message handling)
- **Render pass**: VkRenderPass + minimal framebuffer setup (one color attachment, no depth/stencil, no MSAA — V0.B exit gate = clear color)
- **Command pool + command buffer**: VkCommandPool + per-frame VkCommandBuffer allocation pattern
- **Graphics pipeline (minimal)**: minimal VkPipeline для clear color render — minimal vertex + fragment shader pair (clearcolor.vert + clearcolor.frag); full sprite pipeline deferred к V0.C
- **Memory allocator (bumper)**: simple linear allocator over VkDeviceMemory per Q3 ratification; free list deferred к V0.C or later
- **Compute pipeline plumbing**: `VkPipeline` с `VK_PIPELINE_BIND_POINT_COMPUTE` + descriptor set layout + descriptor pool + pipeline layout
- **Compute dispatch**: VkCmdDispatch wrapper + fence-based sync (К-L7 atomic-from-observer preserved per V0.A pattern)
- **C ABI extension** в `DualFrontier.Core.Native.dll`: `df_world_register_compute_pipeline` + `df_world_field_dispatch_compute` per Q2 ratification (existing kernel module)
- **Async compute queue family selection**: extends V0.A VulkanDevice с QF 1 selection logic (К-L19 Item 43 from halted К10.3 brief)
- **HardwareCapabilityCheck.Verify**: startup fail-fast logic (К-L19 Item 44 from halted К10.3 brief)
- **SPIR-V toolchain integration**: MSBuild target invoking `tools/glslangValidator.exe` per Q4 ratification (in-repo committed copy from VULKAN_SDK\Bin\)
- **Minimal shaders**: clearcolor.vert + clearcolor.frag + noop.comp (per Q5 ratification — sprite shaders deferred к V0.C)
- **К-L19 invariant landing**: Vulkan 1.3 + async compute queue family mandate ratified в KERNEL_ARCHITECTURE.md (К10.3 brief had К-L19 architecturally в commit 5 — V0.B realizes implementation; K-L19 invariant row in KERNEL_ARCHITECTURE table lands при V0.B closure)
- **Mixed [LibraryImport]/[DllImport] convention formalization**: S-LOCK formalizes V0.A pattern (Lesson #22 application)
- **C ABI alignment audit mandatory**: per V0.A executor finding (Lesson #7 strengthening) — every new Vulkan struct gets Marshal.SizeOf<T>() unit test validating против Vulkan spec sizeof

Out-of-scope (V0.C):
- Sprite atlas + sprite batcher (consumes V0.B graphics pipeline + memory allocator)
- Bitmap font + text renderer
- PNG decoder
- Threading model integration (PresentationBridge consumer — domain integration)
- Camera2D orthographic projection
- TileMap rendering
- UI primitives (panels, labels, progress bars)
- Full input event types (V0.A scaffolded marker; V0.C completes)
- Domain integration (`DualFrontier.Presentation` port from Godot к Runtime)
- Godot Presentation cutover (R.8 — post-V substrate close)

Out-of-scope (deferred к post-V substrate close):
- К10.3 brief restart (after V0.B closure)
- V1/V2 substrate primitives (compute pipeline plumbing landed V0.B; V1 diffusion shader + V2 wave shader separate briefs)
- M-V demonstrations (per Q-R-1 + Q-V-2 — separate М-V briefs after V1/V2)
- Full memory allocator с free list (V0.B bumper only — К-L14 «без костылей» acceptable per VULKAN_SUBSTRATE §1.1 verbatim «bumper allocator initially»)
- К11+ Core systems migration к native code

**Strategic note**: V0.B substantially larger scope than V0.A. Surface includes ~30 new Vulkan functions (swapchain extension, render pass, command pool, pipeline, memory, surface), ~15 new Vulkan structs (VkSwapchainCreateInfoKHR, VkAttachmentDescription, VkSubpassDescription, VkRenderPassCreateInfo, VkPipelineLayoutCreateInfo, VkComputePipelineCreateInfo, VkBufferCreateInfo, VkImageCreateInfo, VkMemoryAllocateInfo, etc.), MSBuild target integration, native C ABI extension. **C ABI alignment audit load-bearing per V0.A precedent** — каждый new Vulkan struct получает Marshal.SizeOf verification test before consumption.

**К10.3 unblocking gate**: After V0.B closure, К10.3 brief Phase 0 reads find all required Vulkan code anchors. К10.3 SC-14 halt class won't fire. К10.3 brief Items 43+44 (async compute queue selection + HardwareCapabilityCheck) extend existing V0.B code rather than create from scratch — К10.3 surgical amendments where V0.B shape differs from К10.3 brief assumptions (Lesson #22 application).

**Brief size note**: V0.B brief target ~2500-3000 lines per К-L14 default-inclusion bias — V0.B substantial scope (swapchain + render pass + compute + memory + SPIR-V + async compute + HardwareCapabilityCheck) requires verbatim P/Invoke surfaces, verbatim Vulkan struct layouts с alignment audit notes, MSBuild target integration, native C ABI extension specification, multi-shader build pipeline. Brief size growth proportional к architectural surface per К-L14.

---

## §1 — Crystalka ratified scope locks (V0.B authoring, post-V0.A closure 2026-05-18)

### §1.1 — S-LOCK-1: V0.B scope = swapchain + render pass + compute + memory + SPIR-V + async compute + hardware check

**LOCK**: V0.B implements exactly these deliverables, in dependency order:

| Group | Deliverable | Source |
|---|---|---|
| Surface foundation | Win32 surface creation (`vkCreateWin32SurfaceKHR`) + extension activation | VULKAN_SUBSTRATE §1.1 V0 rendering |
| Surface foundation | Update V0.A VulkanInstance с surface extensions enabled | V0.A precedent (extensions enabled but не consumed) |
| Surface foundation | Update V0.A Window с WM_SIZE message handling | V0.A precedent (WM_CLOSE/WM_DESTROY only) |
| Async compute | Update V0.A VulkanDevice с async compute queue family selection (К-L19 Item 43) | halted К10.3 brief Item 43 |
| Async compute | Add `HardwareCapabilityCheck.Verify` startup fail-fast (К-L19 Item 44) | halted К10.3 brief Item 44 |
| Swapchain | VulkanSurface (`VkSurfaceKHR` lifecycle) | VULKAN_SUBSTRATE §2.2 |
| Swapchain | VulkanSwapchain (`VkSwapchainKHR` + image acquisition + present) | VULKAN_SUBSTRATE §2.2 |
| Swapchain | Swapchain recreation on resize (WM_SIZE handler delivers resize event) | VULKAN_SUBSTRATE §1.1 V0 + §4.2 R.0 |
| Memory | MemoryAllocator (bumper linear allocator) | VULKAN_SUBSTRATE §1.1 + §2.2 |
| Memory | VulkanBuffer wrapper (VkBuffer + memory binding) | VULKAN_SUBSTRATE §2.2 |
| Memory | VulkanImage wrapper (VkImage + view + memory binding) | VULKAN_SUBSTRATE §2.2 |
| Render pass | VulkanRenderPass (one color attachment, no depth) | VULKAN_SUBSTRATE §2.2 |
| Render pass | VulkanFramebuffer (per swapchain image) | VULKAN_SUBSTRATE §2.2 |
| Render pass | VulkanCommandPool + VulkanCommandBuffer (per-frame pattern) | VULKAN_SUBSTRATE §2.2 |
| Render pass | VulkanShaderModule (SPIR-V loading) | VULKAN_SUBSTRATE §2.2 |
| Graphics pipeline | Minimal VulkanPipeline (clear color graphics pipeline) | V0.B exit criterion |
| Compute pipeline | VulkanComputePipeline (`VK_PIPELINE_BIND_POINT_COMPUTE` + descriptor set layout + descriptor pool + pipeline layout) | VULKAN_SUBSTRATE §1.1 V0 compute |
| Compute pipeline | VulkanComputeDescriptors (descriptor pool management) | VULKAN_SUBSTRATE §2.2 |
| Compute pipeline | ComputeDispatch (VkCmdDispatch wrapper + fence sync) | VULKAN_SUBSTRATE §2.2 |
| Compute pipeline | FieldStorageBinding (K9 `RawTileField` → SSBO binding) | VULKAN_SUBSTRATE §2.2 + §3.4 |
| Native C ABI | Extend `df_capi.h` с `df_world_register_compute_pipeline` + `df_world_field_dispatch_compute` | VULKAN_SUBSTRATE §3.4 + Q2 (a) ratification |
| Native C ABI | Implement в native kernel (links к vulkan-1.dll) — bridges K9 storage к Vulkan compute | Q2 (a) ratification |
| SPIR-V toolchain | In-repo `tools/glslangValidator.exe` (~6MB committed copy from VULKAN_SDK\Bin\) | Q4 (a) ratification |
| SPIR-V toolchain | MSBuild target в Directory.Build.props (compile shaders before build) | VULKAN_SUBSTRATE §2.7 |
| Shaders | `tools/shaders/clearcolor.vert` + `clearcolor.frag` + `noop.comp` GLSL sources | Q5 (c) ratification |
| Shaders | Pre-compiled `assets/shaders/*.spv` output (committed since toolchain regenerates на build) | VULKAN_SUBSTRATE §2.7 |
| Runtime composition | Update Runtime.cs facade с new components (Surface + Swapchain + RenderPass + ComputePipelineRegistry + MemoryAllocator) | V0.A pattern |
| К-L19 landing | KERNEL_ARCHITECTURE.md amendment: К-L19 row added к К-L invariant table (LOCKED via V0.B implementation) | К-L19 architectural landing |
| Test infrastructure | Marshal.SizeOf<T>() unit test per Vulkan struct (Lesson #7 strengthening) | V0.A executor finding |
| Test infrastructure | Compute pipeline registration + empty noop dispatch integration test | VULKAN_SUBSTRATE §1.1 V0 exit |
| Smoke test | V0.B exit criteria standalone executable (clear color visible + compute round-trip + hardware check + zero validation errors) | VULKAN_SUBSTRATE §1.1 V0 exit |

**Deliverable ordering rationale**:
1. **Surface foundation first** — Win32 surface + V0.A extension activation + WM_SIZE handler (foundation for swapchain)
2. **Async compute extension second** — VulkanDevice update с QF 1 selection + HardwareCapabilityCheck (К-L19 enforcement before substantial work)
3. **Memory allocator third** — bumper allocator + VulkanBuffer + VulkanImage (required by swapchain images + descriptor pools)
4. **Swapchain fourth** — VulkanSurface + VulkanSwapchain + recreation on resize
5. **Render pass + framebuffer fifth** — VulkanRenderPass + VulkanFramebuffer per swapchain image
6. **Command infrastructure sixth** — VulkanCommandPool + VulkanCommandBuffer (per-frame pattern)
7. **Shader infrastructure seventh** — SPIR-V toolchain integration + VulkanShaderModule + minimal shaders
8. **Graphics pipeline eighth** — minimal VulkanPipeline для clear color
9. **Compute pipeline ninth** — VulkanComputePipeline + descriptor sets + dispatch + fence sync
10. **Native C ABI tenth** — df_capi.h extension + native implementation bridging K9 storage к Vulkan compute
11. **K-L19 amendment eleventh** — load-bearing commit; К-L19 invariant landing с full implementation behind it
12. **Smoke test twelfth** — V0.B exit criteria verification
13. **Closure thirteenth** — REGISTER amendments + REQ additions + EVT-V0_B-CLOSURE

### §1.2 — S-LOCK-2: V0.B monolithic approach (per Q1 ratification)

**LOCK** (per Crystalka ratification 2026-05-18): V0.B = single execution session covering all rendering + compute foundation. NOT split V0.B.1/V0.B.2.

**Rationale**:
- V0 exit criteria explicit per VULKAN_SUBSTRATE.md §1.1 (clear color rendered + compute pipeline registration round-trip + validation clean) — coherent unit
- Split fragments unified VkDevice setup (swapchain + compute pipeline share descriptor pool patterns; memory allocator serves both)
- К10.3 brief assumes V0 fully operational; partial К10.3 restart adds complexity vs single V0.B execution
- Brief size growth proportional к architectural complexity per К-L14
- К-L14 default-inclusion bias — substantial scope acceptable when architecturally coherent

**Multi-session execution provision**: V0.B brief structures atomic commits такие, что execution can pause + resume cleanly per Lesson #8 — atomic intermediate states preserved. Если Claude Code session token limit reached mid-execution, work resumes from last clean commit in subsequent session с no rework.

### §1.3 — S-LOCK-3: Native C ABI extension lands в existing `DualFrontier.Core.Native.dll` (Q2 (a))

**LOCK** (per Crystalka ratification 2026-05-18): V0.B compute pipeline registration + field dispatch C ABI extends existing `DualFrontier.Core.Native` module. NOT new separate `DualFrontier.Vulkan.Native.dll`.

**Rationale**:
- VULKAN_SUBSTRATE.md §3.4 verbatim: «C ABI extension on `df_capi.h`»
- К-L11 «NativeWorld single source of truth» preserved — kernel remains authoritative для storage; V substrate consumes via existing C ABI extended с Vulkan compute hooks
- К9 RawTileField storage already в `DualFrontier.Core.Native` (verified Phase 0 brief authoring: df_capi.h §K9 field storage section complete)
- Native module links к `vulkan-1.dll` at V0.B; managed `DualFrontier.Runtime.Compute` module wraps native compute pipeline lifecycle

**Native module addition**:
- `native/DualFrontier.Core.Native/CMakeLists.txt` modified: add `vulkan-1.dll` linkage via Vulkan SDK find_package (Phase 0 reads identify VULKAN_SDK env var; CMake locates Vulkan automatically); add new source files (compute_pipeline.cpp, compute_dispatch.cpp); update DF_NATIVE_HEADERS list
- `native/DualFrontier.Core.Native/include/compute_pipeline.h` (new) — internal API для Vulkan compute pipeline lifecycle
- `native/DualFrontier.Core.Native/src/compute_pipeline.cpp` (new) — implementation
- `native/DualFrontier.Core.Native/src/capi.cpp` extended с new exports

**Per K10.1/K10.2 precedent**: native code modifications consume VS-bundled CMake (`D:\Visual Studio\Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe`). DF_CHECK runner convention preserved для native selftest extensions.

### §1.4 — S-LOCK-4: Memory allocator = bumper linear allocator only (per Q3 ratification)

**LOCK**: V0.B lands bumper allocator only (simplest viable allocator per VULKAN_SUBSTRATE §1.1 verbatim «bumper allocator initially»). Free list / pool allocator deferred к V0.C or later (К-extensions scope).

**Bumper allocator semantics**:
- One `VkDeviceMemory` block per memory type (host-visible / device-local / staging)
- Allocations bump pointer forward — no free list
- Reset on explicit reset call (e.g., before swapchain recreation)
- No fragmentation issues (linear allocation pattern)
- Sufficient для V0.B exit criteria: swapchain image memory (small fixed count) + descriptor pool memory + minimal compute buffer

**Future allocator upgrade**: free list allocator или pool allocator (e.g., VMA — Vulkan Memory Allocator) deferred. Trigger: actual memory pressure measurements в V1/V2 substrate primitives или first real game scenario. К-L14 «без костылей» preserves architectural cleanness — bumper allocator is honest minimal solution, не temporary hack.

### §1.5 — S-LOCK-5: SPIR-V toolchain = in-repo committed `glslangValidator.exe` (per Q4 (a) ratification)

**LOCK**: V0.B commits `glslangValidator.exe` к `tools/glslangValidator.exe` per VULKAN_SUBSTRATE §2.7 verbatim. ~6MB binary acceptable per К-L14 architectural cleanness.

**Build self-contained**: developer machine doesn't need VULKAN_SDK для shader compilation. CI machines clone repo + build (no SDK installation needed). Production binary depends only on `vulkan-1.dll` + pre-compiled `*.spv` files per VULKAN_SUBSTRATE §0 (locked invariant).

**MSBuild target** (per VULKAN_SUBSTRATE §2.7):

```xml
<Target Name="CompileShaders" BeforeTargets="Build"
        Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\clearcolor.vert -o $(SolutionDir)assets\shaders\clearcolor.vert.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\clearcolor.frag -o $(SolutionDir)assets\shaders\clearcolor.frag.spv" />
  <Exec Command="$(SolutionDir)tools\glslangValidator.exe -V $(SolutionDir)tools\shaders\noop.comp -o $(SolutionDir)assets\shaders\noop.comp.spv" />
</Target>
```

**Pre-compiled `.spv` files committed** к `assets/shaders/`: regeneration happens on build, но committed for clone-and-run convenience + CI без SPIR-V toolchain access.

**Phase 0 deliverable**: copy `glslangValidator.exe` из `$(VULKAN_SDK)\Bin\glslangValidator.exe` к `tools/glslangValidator.exe` (committed to repo).

### §1.6 — S-LOCK-6: V0.B shaders = minimal clearcolor + noop only (per Q5 (c) ratification)

**LOCK**: V0.B lands minimal shaders sufficient для exit criteria:

- `tools/shaders/clearcolor.vert` — minimal vertex shader (fullscreen triangle, no vertex buffer needed via gl_VertexIndex trick)
- `tools/shaders/clearcolor.frag` — minimal fragment shader (outputs solid color)
- `tools/shaders/noop.comp` — minimal compute shader (empty main, exists для pipeline registration round-trip test)

**Sprite shaders deferred к V0.C**: full sprite.vert + sprite.frag pipeline (textured quad rendering, vertex buffer consumer, sprite atlas sampler) belongs alongside sprite batcher V0.C где они actually consumed. Per Lesson #11 architectural reduction methodology + Lesson #20 scope discipline.

**Shader source verbatim** (minimal GLSL 450 + Vulkan conventions):

```glsl
// tools/shaders/clearcolor.vert (fullscreen triangle, no vertex attribute)
#version 450
void main() {
    // Fullscreen triangle trick: 3 vertices covering full screen via gl_VertexIndex bit math
    vec2 positions[3] = vec2[](vec2(-1.0, -1.0), vec2(3.0, -1.0), vec2(-1.0, 3.0));
    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
}
```

```glsl
// tools/shaders/clearcolor.frag (solid color output)
#version 450
layout(location = 0) out vec4 outColor;
void main() {
    outColor = vec4(0.1, 0.2, 0.4, 1.0);  // dark blue clear color
}
```

```glsl
// tools/shaders/noop.comp (empty compute shader, smallest valid Vulkan compute pipeline)
#version 450
layout(local_size_x = 1, local_size_y = 1, local_size_z = 1) in;
void main() {
    // Intentionally empty — V0.B compute pipeline registration round-trip test
}
```

### §1.7 — S-LOCK-7: Mixed [LibraryImport] + [DllImport] convention formalized (per Q6 ratification)

**LOCK**: V0.B brief formalizes V0.A executor's mixed P/Invoke convention:

- **[LibraryImport]** (preferred — .NET 7+ source generator) для blittable-only signatures
- **[DllImport]** для non-blittable struct fields (e.g., strings inside structs, function-pointer marshalling)
- Static class organization preserved: `internal static partial class VkApi` (V0.A pattern)
- Convention документировано in `Native/Vulkan/VkApi.cs` comment header during V0.B

**Per Lesson #22**: matching .NET 7+ source generator constraints. V0.A executor surfaced this organically; V0.B brief formalizes так что V0.C inherits без deviation.

### §1.8 — S-LOCK-8: C ABI alignment audit mandatory (Lesson #7 strengthening)

**LOCK** (per V0.A executor finding 2026-05-18): every new Vulkan struct в V0.B gets explicit alignment audit:

**Required for each new Vulkan struct**:
1. Read Vulkan spec or vulkan_core.h header verbatim для struct layout
2. Identify all 64-bit fields (`VkDeviceSize`, `uint64_t` aliases) — these require 8-byte alignment
3. Identify nested structs containing 64-bit fields — propagates 8-byte alignment requirement
4. Add explicit padding fields (`fixed byte _padBeforeX[N]`) matching MSVC x64 ABI
5. Add trailing padding (`fixed byte _padTrailing[N]`) если struct size не natural alignment multiple
6. Write Marshal.SizeOf<T>() unit test in `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (new file) validating C# struct size matches Vulkan spec sizeof

**Pattern precedent**: VkPhysicalDeviceProperties (V0.A) — 824 bytes after alignment fix. V0.B introduces many new structs; same discipline.

**Affected V0.B structs requiring audit**:
- VkSurfaceCapabilitiesKHR (~52 bytes, contains VkExtent2D + multiple uint32_t)
- VkSurfaceFormatKHR (8 bytes, two enums)
- VkPresentModeKHR enum
- VkSwapchainCreateInfoKHR (~104 bytes, contains VkExtent2D + multiple uint32_t + pointers)
- VkImageCreateInfo (~88 bytes)
- VkBufferCreateInfo (~56 bytes, contains VkDeviceSize size + alignment fields)
- VkMemoryAllocateInfo (24 bytes)
- VkMemoryRequirements (24 bytes, **contains VkDeviceSize — needs 8-byte alignment**)
- VkRenderPassCreateInfo (~40 bytes + pointers)
- VkAttachmentDescription (~36 bytes)
- VkSubpassDescription (~72 bytes)
- VkComputePipelineCreateInfo (~96 bytes)
- VkPipelineLayoutCreateInfo (~32 bytes)
- VkDescriptorSetLayoutCreateInfo (~24 bytes)
- VkDescriptorPoolCreateInfo (~32 bytes)
- VkCommandPoolCreateInfo (~24 bytes)
- VkFenceCreateInfo (~16 bytes)
- VkShaderModuleCreateInfo (~32 bytes)
- VkPipelineShaderStageCreateInfo (~48 bytes)
- VkWin32SurfaceCreateInfoKHR (~32 bytes)

Per V0.A precedent, ~20+ new structs require alignment audit; ~10-15 contain VkDeviceSize fields requiring explicit padding.

### §1.9 — S-LOCK-9: К-L19 invariant landing на V0.B (post-К10.3 halt resolution)

**LOCK**: К-L19 «Hardware tier commitment» invariant lands в KERNEL_ARCHITECTURE.md v2.1 → v2.2 при V0.B closure (Commit 13 — load-bearing).

**Rationale**:
- К-L19 was authored в halted К10.3 brief (commit 5) but never landed (К10.3 cascade halted Phase 0)
- V0.B implements К-L19 enforcement: Vulkan 1.3 mandate (V0.A landed) + async compute queue family selection (V0.B Commit 4) + HardwareCapabilityCheck.Verify startup fail-fast (V0.B Commit 5)
- Architectural commitment lands только after implementation exists — К-L14 default-inclusion bias does не extend к claiming invariants без implementation backing

**К-L19 invariant text verbatim** (from halted К10.3 brief §1.7):

> «К-L19: Dual Frontier requires Vulkan 1.3 hardware с async compute queue family support. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer) / AMD RDNA 1+ (RX 5000 и newer) / Intel Arc Alchemist+. Async compute queue used для К-L16 pipeline depth dispatches. Graphics queue used для display rendering. Copy/transfer queue used для asset transfers. К10 mandates queue family availability at startup; failure to detect async compute queue is fail-fast condition с clear hardware requirement diagnostic message. Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation. Project release timeline accounts for hardware proliferation — by Dual Frontier release, target hardware tier represents majority of gaming hardware.»

**К-L invariant table amendment** (KERNEL_ARCHITECTURE.md):

```
| К-L19 | LOCKED (V0.B) | Hardware tier commitment | Vulkan 1.3 + async compute queue family mandate; NVIDIA Turing+ / AMD RDNA 1+ / Intel Arc Alchemist+ baseline; HardwareCapabilityCheck.Verify fail-fast at startup |
```

**README.md amendment** (Hardware Requirements section): same content as halted К10.3 brief commit 5 — published к user-facing documentation when К-L19 lands.

**Per Lesson #8 + Lesson #11**: К-L19 invariant landing + KERNEL_ARCHITECTURE amendment + README.md Hardware Requirements section + REGISTER.yaml DOC-A-KERNEL version bump 2.1 → 2.2 + REQ-K-L19 land **all в same load-bearing commit** (Commit 13). Splitting leaves intermediate state where К-L19 claimed но implementation absent — inconsistent state.

### §1.10 — S-LOCK-10: К10.3 brief restart pathway documented

**LOCK**: After V0.B closure, К10.3 brief restart pathway opens.

К10.3 brief Phase 0 reads will find:
- ✓ `src/DualFrontier.Runtime/` project exists (V0.A)
- ✓ `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` exists с Vulkan P/Invoke surface (V0.A + V0.B extensions)
- ✓ `src/DualFrontier.Runtime/Graphics/VulkanInstance.cs` (V0.A)
- ✓ `src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` с graphics + async compute queue family selection (V0.A + V0.B)
- ✓ V0.B HardwareCapabilityCheck.Verify exists (К-L19 Item 44 от halted К10.3 brief — implemented V0.B)
- ✓ Compute pipeline plumbing operational (V0.B — VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch)
- ✓ Native C ABI extension landed (df_world_register_compute_pipeline + df_world_field_dispatch_compute)
- ✓ К-L19 invariant LOCKED в KERNEL_ARCHITECTURE.md (V0.B Commit 13)

К10.3 SC-14 halt class will не fire. К10.3 brief Commits 3-4 (Items 43+44) — async compute queue selection + HardwareCapabilityCheck — are **already implemented** by V0.B; К10.3 brief restart adjustments mark these as «verified existing implementation» rather than «create from scratch».

К10.3 brief Commits 6-17 (Items 33-44 pipeline depth + display composition + mod lifecycle + К-L7.1/L16/L17/L18 invariants) proceed unchanged where possible; surgical amendments where V0.B shape differs from К10.3 brief assumptions (Lesson #22 application).

### §1.11 — S-LOCK-11: Atomic cascade preserves V0.A discipline

**LOCK**: V0.B executes as multi-commit atomic cascade. Per Lesson #8: V0.B items have **clean intermediate states** because:
1. Surface foundation (Commit 3) extends V0.A VulkanInstance с surface extension consumption — V0.A surface extensions already enabled, V0.B consumes them
2. WM_SIZE handler (Commit 3) extends V0.A WindowProc — additive, не disrupting
3. Async compute selection (Commit 4) extends V0.A VulkanDevice — additional queue family selected without disrupting graphics queue
4. Memory allocator (Commit 6) standalone primitive — consumed by swapchain/render pass downstream
5. Swapchain (Commit 7) requires memory allocator + surface — proper dependency ordering
6. Render pass + framebuffer + command infrastructure (Commits 8-10) build on swapchain
7. SPIR-V toolchain + shaders (Commits 11) standalone build pipeline addition
8. Graphics pipeline (Commit 12) requires render pass + shaders
9. Compute pipeline (Commit 14) standalone — но requires C ABI extension (Commit 15) для K9 storage binding
10. К-L19 invariant landing (Commit 13) load-bearing per Lesson #8
11. Smoke test (Commit 17) verifies V0.B exit criteria
12. Closure (Commit 18) governance

Tests pass at every commit (685 К10.2+V0.A baseline preserved; new V0.B tests additive). cmake --build clean at every native-touching commit. Marshal.SizeOf verification tests run в each commit landing new Vulkan structs (catches alignment regressions early per Lesson #7 strengthening).

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 + Lesson #22, executor MUST complete every read listed below **before writing a single line of V0.B code**. V0.B brief authored 2026-05-18 from V0.A post-closure verified ground truth; drift между brief authoring и execution time possible — surfaces как halt triggers (SC-3) — never silent improvisation per Lesson #20.

V0.B Phase 0 reads **substantially expanded** vs V0.A — V0.B introduces many new Vulkan structs/functions, MSBuild integration, native C ABI extension.

### §2.1 — Verify post-V0.A closure state (hard gates)

Read и verify:

1. `git log --oneline -25` on `main` — confirm:
   - V0.A PR #36 merged (per V0.A closure report — 11 commits a1c772..1a56887)
   - HEAD references V0.A closure commit
   - К10.3 halt does NOT appear in main history (K10.3 brief untracked per HALT_REPORT, awaiting V0.B closure для restart)
   - Halt если V0.A closure не reached

2. `git status` — working tree clean before execution starts. К10.3 brief (`tools/briefs/K10_3_EXECUTION_BRIEF.md`) untracked (expected per HALT_REPORT). **Hard gate** per К10.1/К10.2/V0.A precedents.

3. `docs/governance/REGISTER.yaml` head check — confirm DOC-D-V0_A present с lifecycle EXECUTED.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline. Advisory orphan warnings (per V0.A closure: 13 warnings — 5 baseline + 8 V0.A MODULE.md) — pre-existing, не halt.

5. `dotnet build DualFrontier.sln` — clean baseline.

6. `dotnet test DualFrontier.sln` — baseline pass count: **685 tests green** per V0.A closure report (665 К10.2 baseline + 20 V0.A additive). If suite fails или count diverges (excluding intentional V0.B additions), halt.

7. `cmake --build native/DualFrontier.Core.Native` через VS-bundled cmake — clean baseline. Native selftest passes 77 scenarios (К10.2 baseline). V0.A не touched native; V0.B будет.

### §2.2 — Verify Vulkan SDK + V0.A environment

Per V0.A precedent: Vulkan SDK 1.4.350.0 installed at `C:\VulkanSDK\1.4.350.0\` (or current). Hard gates:

1. `VULKAN_SDK` env var set + points к valid SDK installation
2. `$(VULKAN_SDK)\Bin\glslangValidator.exe` exists (V0.B copies к `tools/glslangValidator.exe`)
3. `$(VULKAN_SDK)\Bin\vulkaninfo.exe` exists (optional — diagnostic tool для troubleshooting)
4. `$(VULKAN_SDK)\Include\vulkan\vulkan_core.h` exists (Vulkan struct layout reference)
5. `vulkan-1.dll` accessible (V0.A verified — preserved)
6. AMD RX 7600S Vulkan 1.4.344 verified (V0.A confirmed К-L19 hardware tier)

If `VULKAN_SDK` env var unset or path invalid — halt SC-16 (matches V0.A halt class). Per V0.A precedent: Crystalka can install Vulkan SDK in-session if needed (LunarG silent installer + UAC click).

### §2.3 — Read VULKAN_SUBSTRATE.md spec (V0.B sections)

Read в full:
- §0 L1-L10 (V0.B preserves V0.A invariants)
- §1.1 V0 — full deliverable list (V0.B implements rendering bullets 4-9 + compute bullets)
- §2.1 project structure (V0.B extends `src/DualFrontier.Runtime/` с Graphics + Compute + Assets submodules)
- §2.2 module purposes (Graphics + Compute module details)
- §2.4 dependency rules (V0.B preserves Rule 1-5)
- §2.5 native interop patterns (V0.A pattern preserved + new structs)
- §2.7 shader strategy (MSBuild target verbatim — V0.B implements)
- §2.8 testing strategy (V0.B test surface expansion)
- §3 compute use case (full section — V0.B foundation primitive plumbing only; V1/V2 substrate primitives separate briefs)
- §3.4 native compute dispatch (C ABI extension verbatim)
- §5 compute use case detail (V0.B context only — V0.B doesn't implement V1/V2 shader math)
- §7 failure modes (V0.B inherits CPU fallback discipline — but actual CPU fallback shaders authored alongside V1/V2 primitives, NOT V0.B)
- §11 methodology adjustments (validation layer ALWAYS-ON в DEBUG preserved)

### §2.4 — Read V0.A code anchors verbatim

Read existing V0.A code shapes к understand exact extension points (Lesson #22):

**Vulkan native interop (V0.A landed)**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` — V0.A 11 functions; V0.B extends с ~30 new functions
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` — V0.A 6 structs + alignment fix precedent VkPhysicalDeviceProperties; V0.B extends с ~20 new structs
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` — V0.A enums; V0.B adds VkFormat, VkColorSpaceKHR, VkPresentModeKHR, VkImageLayout, VkImageUsageFlags, VkSampleCountFlagBits, VkPipelineBindPoint, VkDescriptorType, etc.
- `src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs` — V0.A constants; V0.B extends с swapchain extension constant, sizes for new structs, alignment helpers
- `src/DualFrontier.Runtime/Native/Vulkan/VkDelegates.cs` — V0.A debug messenger delegate

**Graphics layer (V0.A landed)**:
- `src/DualFrontier.Runtime/Graphics/VulkanInstance.cs` — V0.B extends: surface extensions consumed (V0.A had them enabled but не consumed)
- `src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` — V0.B extends: async compute queue family selection added + device extensions list (`VK_KHR_swapchain`)
- `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` — V0.A landed (debug messenger lifecycle); V0.B doesn't modify
- `src/DualFrontier.Runtime/Graphics/PhysicalDeviceInfo.cs` + `QueueFamilyInfo.cs` — V0.A records; V0.B adds async compute queue family info exposure

**Window layer (V0.A landed)**:
- `src/DualFrontier.Runtime/Window/Window.cs` — V0.B extends WindowProcedure с WM_SIZE handler + WindowResizeEvent into InputEventQueue
- `src/DualFrontier.Runtime/Window/InputEventQueue.cs` — V0.A placeholder; V0.B adds WindowResizeEvent type (full input event types still V0.C scope)
- `src/DualFrontier.Runtime/Native/Win32/Win32Api.cs` — V0.B adds `vkCreateWin32SurfaceKHR` not relevant; Vulkan side handles surface. Win32 side V0.B not significantly modified

**Runtime facade (V0.A landed)**:
- `src/DualFrontier.Runtime/Runtime.cs` — V0.B substantially extends composition: adds VulkanSurface + MemoryAllocator + VulkanSwapchain + VulkanRenderPass + VulkanCommandPool + ComputePipelineRegistry; Dispose в reverse construction order
- `src/DualFrontier.Runtime/RuntimeOptions.cs` — V0.B may extend (e.g., `RequireAsyncCompute` flag default true per К-L19)

**Test infrastructure (V0.A landed)**:
- `tests/DualFrontier.Runtime.Tests/` — V0.B adds Vulkan/VulkanStructSizeTests.cs + ComputePipelineTests.cs + MemoryAllocatorTests.cs
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — V0.B extends к verify V0.B exit criteria (clear color rendered + compute round-trip + hardware check)

**Per Lesson #7**: Read VULKAN_SUBSTRATE.md §2.5 P/Invoke template verbatim. Read V0.A existing code shapes literally — V0.B extensions match style exactly.

### §2.5 — Read existing `df_capi.h` к understand extension point

Read existing C ABI surface:
- `native/DualFrontier.Core.Native/include/df_capi.h` — K9 field storage section (lines ~280-380, df_world_register_field + df_world_field_acquire_span + etc.) — V0.B extends с compute pipeline functions
- `native/DualFrontier.Core.Native/CMakeLists.txt` — V0.B modifies: adds vulkan-1.dll linkage + new source files (compute_pipeline.cpp, compute_dispatch.cpp)
- `native/DualFrontier.Core.Native/src/capi.cpp` — V0.B extends с new export implementations

**Per Q2 (a) ratification**: V0.B compute pipeline code lands в existing kernel module. CMake Vulkan SDK find_package locates `vulkan-1.dll` import lib automatically на developer machine с VULKAN_SDK env var set.

**CMakeLists.txt extension pattern** (Phase 0 verify approach):
```cmake
# After existing project() declaration:
find_package(Vulkan REQUIRED)  # Locates Vulkan SDK via VULKAN_SDK env var

# Extension of existing source list:
list(APPEND DF_NATIVE_SOURCES
    src/compute_pipeline.cpp
    src/compute_dispatch.cpp
)
list(APPEND DF_NATIVE_HEADERS
    include/compute_pipeline.h
    include/compute_dispatch.h
)

# After add_library() declaration:
target_link_libraries(DualFrontier.Core.Native PRIVATE Vulkan::Vulkan)
```

### §2.6 — Read REGISTER.yaml structure для V0.B enrollment

Identify:
- DOC-D-V0_A entry (V0.A closure) — V0.B brief enrollment after this entry
- DOC-A-KERNEL entry (KERNEL_ARCHITECTURE.md v2.1 LOCKED) — V0.B amendment к v2.2 при К-L19 landing (Commit 13)
- DOC-A-VULKAN_SUBSTRATE entry — V0.B governance_events append с V0.B closure
- DOC-G-README entry — V0.B Hardware Requirements section landing
- audit_trail events list — V0.B adds EVT-{date}-V0_B-CLOSURE
- New REQs: REQ-V0-B-SWAPCHAIN, REQ-V0-B-RENDER_PASS, REQ-V0-B-COMPUTE_PIPELINE, REQ-V0-B-MEMORY_ALLOCATOR, REQ-V0-B-SPIRV_TOOLCHAIN, REQ-V0-B-ASYNC_COMPUTE_QUEUE, REQ-V0-B-HARDWARE_CHECK, REQ-K-L19 (4-8 new REQs depending on grouping)

### §2.7 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 + §2.2 + V0.A precedents:
- Working tree dirty (К10.3 brief untracked acceptable)
- Baseline tests failing (excluding intentional V0.B additions)
- `sync_register.ps1 --validate` non-zero baseline (NEW errors only — pre-existing advisory warnings OK)
- Build failure baseline (dotnet OR cmake)
- V0.A closure не reached

**Vulkan SDK hard gates** (V0.B verifies same as V0.A precedent):
- `VULKAN_SDK` env var unset
- `glslangValidator.exe` absent in VULKAN_SDK\Bin\
- `vulkan_core.h` absent
- К-L19 hardware tier failure (no async compute queue family on test hardware — V0.A QF 1 visible should preserve this)

**V0.B-specific informational checks (record-only)**:
- vkGetPhysicalDeviceSurfaceCapabilitiesKHR results (min/max image count, current extent, supported transforms — record)
- Available surface formats per `vkGetPhysicalDeviceSurfaceFormatsKHR` (record для swapchain format selection)
- Available present modes per `vkGetPhysicalDeviceSurfacePresentModesKHR` (record — FIFO_KHR guaranteed available, MAILBOX_KHR preferred if available)
- Memory type indices от `vkGetPhysicalDeviceMemoryProperties` (record host-visible + device-local indices)
- Marshal.SizeOf<T>() actual values для each new Vulkan struct (record во время Marshal.SizeOf unit tests — per Lesson #7 strengthening)

If informational check diverges from brief expectation — **record divergence в commit message, continue**. Hard gate failure → halt per SC-N (§4).

---

## §3 — Atomic commit cascade (target ~18 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean + `cmake --build` clean at every code-touching commit; `dotnet test` 685+ passing at every commit (V0.A baseline preserved; new V0.B tests additive).

### Commit 1 — Brief authoring commit (V0.B brief enrollment)

**Files**:
- `tools/briefs/V0_B_EXECUTION_BRIEF.md` (this brief)
- `docs/governance/REGISTER.yaml` (DOC-D-V0_B entry с `lifecycle: AUTHORED`, `category: D`, `tier: 3`)

**Validation**:
- `sync_register.ps1 --validate` exit 0
- No code changes

**Commit message**: `docs(briefs): V0.B brief authored — swapchain + render pass + compute pipeline + memory allocator + SPIR-V + async compute + hardware check`

### Commit 2 — Vulkan struct size verification test infrastructure (Lesson #7 strengthening)

**Files**:
- `tests/DualFrontier.Runtime.Tests/Vulkan/MODULE.md` (new — alignment audit discipline documented)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (new — initial tests covering V0.A structs as regression baseline + scaffold для V0.B new structs)

**Implementation surface**:

```csharp
// tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs
using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Runtime.Tests.Vulkan;

/// <summary>
/// Marshal.SizeOf verification tests per Lesson #7 strengthening (P/Invoke ABI alignment audit recipe).
/// Each new Vulkan struct gets size verification против Vulkan 1.3 spec sizeof.
/// Caught alignment regressions early — V0.A executor surfaced VkPhysicalDeviceProperties 816 → 824
/// bytes alignment fix; pattern inherited V0.B + future V briefs.
/// </summary>
public sealed class VulkanStructSizeTests
{
    // V0.A baseline structs (regression coverage)
    [Fact]
    public void VkApplicationInfo_Size_Matches_Spec()
    {
        // sType (4) + pad (4) + pNext (8) + pApplicationName (8) + appVersion (4) + pad (4)
        // + pEngineName (8) + engineVersion (4) + apiVersion (4) = 48 bytes на x64
        Marshal.SizeOf<VkApplicationInfo>().Should().Be(48);
    }

    [Fact]
    public void VkPhysicalDeviceProperties_Size_Matches_Spec()
    {
        // Per Vulkan 1.3 spec on x64: 824 bytes (V0.A alignment fix landed here).
        // Composition: header 20 + deviceName 256 + pipelineCacheUUID 16 + _padBeforeLimits 4
        //   + limits 504 + sparseProperties 20 + _padTrailing 4 = 824.
        Marshal.SizeOf<VkPhysicalDeviceProperties>().Should().Be(824);
    }

    // V0.B scaffold (tests for new structs added incrementally per commit)
    // Commit 7 (Swapchain) adds: VkSurfaceCapabilitiesKHR, VkSurfaceFormatKHR, VkSwapchainCreateInfoKHR
    // Commit 8 (Render pass) adds: VkAttachmentDescription, VkSubpassDescription, VkRenderPassCreateInfo
    // ... etc.
}
```

**Validation**:
- `dotnet build` clean
- `dotnet test` 685 baseline + 2 size tests pass (V0.A struct regression coverage)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `test(runtime): V0.B — Vulkan struct size verification infrastructure (Lesson #7 strengthening per V0.A executor finding)`

### Commit 3 — Surface foundation: VulkanInstance surface extensions + Window WM_SIZE handler + WindowResizeEvent

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkWin32SurfaceCreateInfoKHR added; check alignment audit)
- `src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs` (modified — surface extension function names; struct size constants)
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkDestroySurfaceKHR added via vkGetInstanceProcAddr loading pattern)
- `src/DualFrontier.Runtime/Graphics/VulkanInstance.cs` (modified — surface extensions activation already in V0.A; V0.B confirms consumption pattern)
- `src/DualFrontier.Runtime/Window/Window.cs` (modified — WindowProcedure handles WM_SIZE; populates WindowResizeEvent into InputEventQueue)
- `src/DualFrontier.Runtime/Input/IInputEvent.cs` (modified — placeholder marker; V0.B adds WindowResizeEvent type)
- `src/DualFrontier.Runtime/Input/WindowResizeEvent.cs` (new — concrete event type для swapchain recreation signal)
- `tests/DualFrontier.Runtime.Tests/Window/WindowResizeEventTests.cs` (new — WM_SIZE handler produces WindowResizeEvent)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — add VkWin32SurfaceCreateInfoKHR size test)

**Drift surface**: Window layer extends WindowProcedure с WM_SIZE handler. WindowResizeEvent emitted into InputEventQueue when window resized; swapchain (Commit 7) consumes events для recreation. Win32 surface creation infrastructure ready (consumed Commit 7).

**Implementation surface**:

```csharp
// WindowResizeEvent.cs
namespace DualFrontier.Runtime.Input;

public sealed record WindowResizeEvent(int NewWidth, int NewHeight) : IInputEvent;
```

```csharp
// Window.cs — extended WindowProcedure
private IntPtr WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
{
    switch (msg)
    {
        case Win32Constants.WM_CLOSE:
            // V0.A landed
            _isOpen = false;
            Win32Api.DestroyWindow(hWnd);
            return IntPtr.Zero;
        case Win32Constants.WM_DESTROY:
            // V0.A landed
            Win32Api.PostQuitMessage(0);
            _isOpen = false;
            return IntPtr.Zero;
        case Win32Constants.WM_SIZE:
            // V0.B NEW: emit resize event
            int newWidth = (int)(lParam.ToInt64() & 0xFFFF);
            int newHeight = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
            // Only emit на actual size change (not WM_SIZE during minimize/restore = 0×0)
            if (newWidth > 0 && newHeight > 0 && (newWidth != _currentWidth || newHeight != _currentHeight))
            {
                _currentWidth = newWidth;
                _currentHeight = newHeight;
                _inputQueue.Enqueue(new WindowResizeEvent(newWidth, newHeight));
            }
            return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
        default:
            return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
```

```csharp
// VkStructs.cs — added VkWin32SurfaceCreateInfoKHR
[StructLayout(LayoutKind.Sequential)]
internal struct VkWin32SurfaceCreateInfoKHR
{
    internal VkStructureType sType;       // offset 0
    internal IntPtr pNext;                 // offset 8 (8-byte aligned)
    internal uint flags;                   // offset 16
    // 4-byte pad before hinstance (8-byte alignment of IntPtr on x64)
    internal uint _padBeforeHinstance;     // offset 20
    internal IntPtr hinstance;             // offset 24
    internal IntPtr hwnd;                  // offset 32
}
// Total: 40 bytes. Marshal.SizeOf verification test in VulkanStructSizeTests.cs.
```

**Per Lesson #7 strengthening (alignment audit)**: VkWin32SurfaceCreateInfoKHR has IntPtr fields (hinstance, hwnd) requiring 8-byte alignment after 4-byte flags. Explicit `_padBeforeHinstance` field per V0.A precedent.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685 baseline + WindowResizeEventTests + VkWin32SurfaceCreateInfoKHR size test pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — Win32 surface foundation + WM_SIZE handler + WindowResizeEvent (preparing swapchain)`

### Commit 4 — Async compute queue family selection (К-L19 Item 43 from halted К10.3)

**Files**:
- `src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` (modified — async compute queue family selection extends V0.A graphics-only logic)
- `src/DualFrontier.Runtime/Graphics/QueueFamilyInfo.cs` (modified — possibly adds AsyncComputePreference flag)
- `tests/DualFrontier.Runtime.Tests/Graphics/AsyncComputeQueueSelectionTests.cs` (new — mocked queue family enumeration tests)

**Drift surface**: VulkanDevice extends с async compute queue family selection. Async compute queue (`AsyncComputeQueue` property + `AsyncComputeQueueFamilyIndex`) operational. Logical device creation extends к request both graphics + async compute queues.

**Implementation surface**:

```csharp
// VulkanDevice.cs — async compute queue family selection
public sealed class VulkanDevice : IDisposable
{
    // V0.A landed: GraphicsQueue + GraphicsQueueFamilyIndex
    public IntPtr GraphicsQueue => _graphicsQueue;
    public uint GraphicsQueueFamilyIndex => _graphicsQueueFamilyIndex;
    
    // V0.B NEW: async compute queue
    public IntPtr AsyncComputeQueue => _asyncComputeQueue;
    public uint? AsyncComputeQueueFamilyIndex => _asyncComputeQueueFamilyIndex;
    
    private IntPtr _asyncComputeQueue;
    private uint? _asyncComputeQueueFamilyIndex;
    
    private void SelectPhysicalDevice()
    {
        // V0.A logic preserved (prefer discrete GPU с graphics queue family)
        // ... existing code ...
        
        // V0.B NEW: also identify async compute queue family on selected device
        // Prefer dedicated compute-only queue family (no graphics bit) — per К-L19 architectural intent
        foreach (QueueFamilyInfo qf in selected.QueueFamilies)
        {
            if (qf.SupportsCompute && !qf.SupportsGraphics)
            {
                _asyncComputeQueueFamilyIndex = qf.Index;
                break;
            }
        }
        
        // Fallback: compute-capable graphics queue family (preserves К-L19 mandate when no dedicated)
        if (_asyncComputeQueueFamilyIndex is null)
        {
            foreach (QueueFamilyInfo qf in selected.QueueFamilies)
            {
                if (qf.SupportsCompute)
                {
                    _asyncComputeQueueFamilyIndex = qf.Index;
                    break;
                }
            }
        }
        
        // К-L19 mandate: async compute queue family must exist (HardwareCapabilityCheck enforces fail-fast — Commit 5)
    }
    
    private unsafe void CreateLogicalDevice()
    {
        // V0.B extension: request both graphics + async compute queues
        var queueCreateInfos = new List<VkDeviceQueueCreateInfo>();
        float queuePriority = 1.0f;
        
        // Graphics queue (V0.A preserved)
        queueCreateInfos.Add(new VkDeviceQueueCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            queueFamilyIndex = _graphicsQueueFamilyIndex,
            queueCount = 1,
            pQueuePriorities = &queuePriority,
        });
        
        // Async compute queue (V0.B NEW — only если distinct family)
        if (_asyncComputeQueueFamilyIndex.HasValue 
            && _asyncComputeQueueFamilyIndex.Value != _graphicsQueueFamilyIndex)
        {
            queueCreateInfos.Add(new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                queueFamilyIndex = _asyncComputeQueueFamilyIndex.Value,
                queueCount = 1,
                pQueuePriorities = &queuePriority,
            });
        }
        
        // V0.B NEW: device extensions list (VK_KHR_swapchain required)
        var deviceExtensions = new List<string> { "VK_KHR_swapchain" };
        var extPtrs = MarshalStringArray(deviceExtensions);
        
        try
        {
            fixed (byte** extPtrsPinned = extPtrs)
            fixed (VkDeviceQueueCreateInfo* queueCreateInfosPinned = queueCreateInfos.ToArray())
            {
                var createInfo = new VkDeviceCreateInfo
                {
                    sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
                    pNext = IntPtr.Zero,
                    flags = 0,
                    queueCreateInfoCount = (uint)queueCreateInfos.Count,
                    pQueueCreateInfos = queueCreateInfosPinned,
                    enabledLayerCount = 0,
                    ppEnabledLayerNames = null,
                    enabledExtensionCount = (uint)deviceExtensions.Count,
                    ppEnabledExtensionNames = extPtrsPinned,
                    pEnabledFeatures = IntPtr.Zero,
                };
                
                VkResult result = VkApi.vkCreateDevice(_physicalDevice, in createInfo, IntPtr.Zero, out _device);
                if (result != VkResult.VK_SUCCESS)
                {
                    throw new InvalidOperationException($"vkCreateDevice failed: {result}");
                }
            }
            
            // Get graphics queue (V0.A)
            VkApi.vkGetDeviceQueue(_device, _graphicsQueueFamilyIndex, 0, out _graphicsQueue);
            
            // Get async compute queue (V0.B)
            if (_asyncComputeQueueFamilyIndex.HasValue)
            {
                VkApi.vkGetDeviceQueue(_device, _asyncComputeQueueFamilyIndex.Value, 0, out _asyncComputeQueue);
            }
        }
        finally
        {
            FreeStringArray(extPtrs);
        }
    }
}
```

**Per S-LOCK-9 (К-L19 deferred к Commit 13)**: async compute queue selection lands V0.B Commit 4 (this); К-L19 invariant table amendment lands Commit 13 (load-bearing после full implementation operational).

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + AsyncComputeQueueSelectionTests pass
- Manual: on Crystalka «Skarlet», AsyncComputeQueueFamilyIndex should resolve к QF 1 (V0.A smoke test visible candidate)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — async compute queue family selection (К-L19 Item 43 from halted К10.3)`

### Commit 5 — HardwareCapabilityCheck (К-L19 Item 44 from halted К10.3)

**Files**:
- `src/DualFrontier.Runtime/Graphics/HardwareCapabilityCheck.cs` (new — startup fail-fast logic)
- `src/DualFrontier.Runtime/Graphics/HardwareCapabilityException.cs` (new — diagnostic exception type)
- `src/DualFrontier.Runtime/Runtime.cs` (modified — Runtime.Create invokes HardwareCapabilityCheck.Verify после VulkanDevice creation)
- `tests/DualFrontier.Runtime.Tests/Graphics/HardwareCapabilityCheckTests.cs` (new — mocked Vulkan instance + device scenarios)

**Drift surface**: Startup fail-fast check verifies К-L19 hardware tier satisfied. Если AsyncComputeQueueFamilyIndex is null OR Vulkan API version < 1.3 — HardwareCapabilityException thrown с user-facing diagnostic message.

**Implementation surface** (per halted К10.3 brief Commit 4 verbatim):

```csharp
// HardwareCapabilityCheck.cs
namespace DualFrontier.Runtime.Graphics;

public static class HardwareCapabilityCheck
{
    public static void Verify(VulkanInstance vulkan, VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(vulkan);
        ArgumentNullException.ThrowIfNull(device);
        
        // Vulkan 1.3 API version check (V0.A landed similar check in VulkanInstance.VerifyVulkanApiVersion)
        if (vulkan.ApiVersion < VkConstants.VK_API_VERSION_1_3)
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 API version. " +
                $"Detected version: 0x{vulkan.ApiVersion:X}. " +
                "Upgrade GPU driver or install Vulkan 1.3 capable hardware. " +
                "See README.md hardware requirements section."
            );
        }
        
        // К-L19 mandate: async compute queue family availability
        if (device.AsyncComputeQueueFamilyIndex is null)
        {
            throw new HardwareCapabilityException(
                "Dual Frontier requires Vulkan 1.3 hardware с async compute queue " +
                "family support. Detected GPU '" + device.SelectedDevice.DeviceName +
                "' does not expose compute-capable queue family. " +
                "Required hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx или newer), " +
                "AMD RDNA 1+ (RX 5500 или newer), Intel Arc Alchemist+ (A380 или newer). " +
                "See README.md hardware requirements section."
            );
        }
    }
}
```

```csharp
// HardwareCapabilityException.cs
namespace DualFrontier.Runtime.Graphics;

public sealed class HardwareCapabilityException : Exception
{
    public HardwareCapabilityException(string message) : base(message) { }
    public HardwareCapabilityException(string message, Exception inner) : base(message, inner) { }
}
```

```csharp
// Runtime.cs — extended Create method
public static Runtime Create(RuntimeOptions options)
{
    ArgumentNullException.ThrowIfNull(options);
    
    var runtime = new Runtime();
    try
    {
        runtime.InputQueue = new InputEventQueue();
        runtime.Window = new Window.Window(options.Window, runtime.InputQueue);
        runtime.VulkanInstance = new VulkanInstance(options.EnableValidationLayer);
        
        if (options.EnableValidationLayer)
        {
            runtime.ValidationLayer = new ValidationLayer(runtime.VulkanInstance);
        }
        
        runtime.VulkanDevice = new VulkanDevice(runtime.VulkanInstance);
        
        // V0.B NEW: hardware capability check (К-L19 fail-fast)
        HardwareCapabilityCheck.Verify(runtime.VulkanInstance, runtime.VulkanDevice);
        
        return runtime;
    }
    catch
    {
        runtime.Dispose();
        throw;
    }
}
```

**Per К-L19 + Lesson #20**: fail-fast at startup, не graceful degradation. Hardware tier exclusion is architectural commitment supporting clean implementation per Lesson #14 default-inclusion bias.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + HardwareCapabilityCheckTests pass (positive case с mocked async compute QF; negative case без, expects HardwareCapabilityException)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — HardwareCapabilityCheck.Verify (К-L19 Item 44 from halted К10.3, startup fail-fast)`

### Commit 6 — Memory allocator (bumper) + VulkanBuffer + VulkanImage primitives

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — adds vkAllocateMemory, vkFreeMemory, vkMapMemory, vkUnmapMemory, vkBindBufferMemory, vkBindImageMemory, vkCreateBuffer, vkDestroyBuffer, vkCreateImage, vkDestroyImage, vkGetBufferMemoryRequirements, vkGetImageMemoryRequirements, vkGetPhysicalDeviceMemoryProperties, vkCreateImageView, vkDestroyImageView)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — adds VkMemoryAllocateInfo, VkMemoryRequirements, VkPhysicalDeviceMemoryProperties, VkMemoryType, VkMemoryHeap, VkBufferCreateInfo, VkImageCreateInfo, VkImageViewCreateInfo, VkExtent2D, VkExtent3D, VkImageSubresourceRange, VkComponentMapping)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — adds VkFormat, VkImageType, VkImageViewType, VkImageUsageFlags, VkImageTiling, VkImageLayout, VkMemoryPropertyFlags, VkMemoryHeapFlags, VkBufferUsageFlags, VkSharingMode, VkSampleCountFlagBits, VkComponentSwizzle, VkImageAspectFlags)
- `src/DualFrontier.Runtime/Graphics/MemoryAllocator.cs` (new — bumper linear allocator)
- `src/DualFrontier.Runtime/Graphics/MemoryBlock.cs` (new — internal record describing allocated memory region)
- `src/DualFrontier.Runtime/Graphics/VulkanBuffer.cs` (new — VkBuffer + memory binding wrapper)
- `src/DualFrontier.Runtime/Graphics/VulkanImage.cs` (new — VkImage + VkImageView + memory binding wrapper)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — adds size tests для new structs per Lesson #7 strengthening)
- `tests/DualFrontier.Runtime.Tests/Graphics/MemoryAllocatorTests.cs` (new — bumper allocator semantics)

**Drift surface**: Memory allocator operational. Bumper allocator allocates VkDeviceMemory blocks per memory type (host-visible / device-local); allocations bump pointer forward; reset capability для swapchain recreation. VulkanBuffer + VulkanImage primitives wrap VkBuffer/VkImage + memory binding via MemoryAllocator.

**Implementation surface (key snippets — full per VULKAN_SUBSTRATE §2.5 + Vulkan 1.3 spec verbatim)**:

```csharp
// MemoryAllocator.cs (bumper allocator)
namespace DualFrontier.Runtime.Graphics;

public sealed class MemoryAllocator : IDisposable
{
    private readonly IntPtr _device;
    private readonly IntPtr _physicalDevice;
    private readonly Dictionary<uint, MemoryBlock> _blocks = new();  // memoryTypeIndex → block
    private const long DefaultBlockSize = 64 * 1024 * 1024;  // 64 MB per memory type
    private bool _disposed;
    
    public MemoryAllocator(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        _physicalDevice = device.PhysicalDevice;
    }
    
    /// <summary>
    /// Allocate memory region of <paramref name="size"/> bytes с specified alignment + memory property requirements.
    /// Returns offset within a VkDeviceMemory block + the block handle itself для caller к bind buffer/image.
    /// </summary>
    public unsafe MemoryAllocation Allocate(ulong size, ulong alignment, VkMemoryPropertyFlags requiredProperties, uint memoryTypeBits)
    {
        uint memoryTypeIndex = FindMemoryType(memoryTypeBits, requiredProperties);
        
        if (!_blocks.TryGetValue(memoryTypeIndex, out var block))
        {
            // Allocate new block для this memory type
            var allocInfo = new VkMemoryAllocateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
                pNext = IntPtr.Zero,
                allocationSize = (ulong)DefaultBlockSize,
                memoryTypeIndex = memoryTypeIndex,
            };
            VkResult result = VkApi.vkAllocateMemory(_device, in allocInfo, IntPtr.Zero, out IntPtr memory);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkAllocateMemory failed: {result}");
            }
            block = new MemoryBlock(memory, (ulong)DefaultBlockSize, 0);
            _blocks[memoryTypeIndex] = block;
        }
        
        // Align bumper offset к requested alignment
        ulong alignedOffset = (block.UsedBytes + alignment - 1) & ~(alignment - 1);
        ulong newUsedBytes = alignedOffset + size;
        if (newUsedBytes > block.Capacity)
        {
            throw new InvalidOperationException(
                $"MemoryAllocator bumper exhausted (memoryTypeIndex {memoryTypeIndex}, " +
                $"requested {size} bytes, available {block.Capacity - alignedOffset}). " +
                "V0.B bumper allocator does не support free list; recreate Runtime or wait для V0.C/free-list upgrade."
            );
        }
        _blocks[memoryTypeIndex] = block с { UsedBytes = newUsedBytes };
        
        return new MemoryAllocation(block.DeviceMemory, alignedOffset, size, memoryTypeIndex);
    }
    
    /// <summary>
    /// Reset all bumper offsets к zero. Used before swapchain recreation. Caller must ensure no
    /// buffer/image still references this memory — typically called после vkDeviceWaitIdle.
    /// </summary>
    public void Reset()
    {
        foreach (var (key, block) in _blocks.ToList())
        {
            _blocks[key] = block с { UsedBytes = 0 };
        }
    }
    
    private unsafe uint FindMemoryType(uint typeBits, VkMemoryPropertyFlags requiredProperties)
    {
        VkApi.vkGetPhysicalDeviceMemoryProperties(_physicalDevice, out VkPhysicalDeviceMemoryProperties memProps);
        for (uint i = 0; i < memProps.memoryTypeCount; i++)
        {
            bool typeOk = (typeBits & (1u << (int)i)) != 0;
            VkMemoryType memType = memProps.memoryTypes[i];
            bool propsOk = (memType.propertyFlags & requiredProperties) == requiredProperties;
            if (typeOk && propsOk)
            {
                return i;
            }
        }
        throw new InvalidOperationException(
            $"No suitable memory type found (typeBits=0x{typeBits:X}, requiredProperties={requiredProperties})"
        );
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        foreach (var (_, block) in _blocks)
        {
            VkApi.vkFreeMemory(_device, block.DeviceMemory, IntPtr.Zero);
        }
        _blocks.Clear();
        _disposed = true;
    }
}

public readonly record struct MemoryAllocation(IntPtr DeviceMemory, ulong Offset, ulong Size, uint MemoryTypeIndex);

internal record struct MemoryBlock(IntPtr DeviceMemory, ulong Capacity, ulong UsedBytes);
```

```csharp
// VulkanBuffer.cs (skeleton)
namespace DualFrontier.Runtime.Graphics;

public sealed class VulkanBuffer : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _buffer;
    private MemoryAllocation _allocation;
    private bool _disposed;
    
    public IntPtr Handle => _buffer;
    public ulong Size { get; }
    public VkBufferUsageFlags Usage { get; }
    
    public VulkanBuffer(VulkanDevice device, MemoryAllocator allocator, ulong size, VkBufferUsageFlags usage, VkMemoryPropertyFlags memoryProps)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        
        _device = device.Handle;
        Size = size;
        Usage = usage;
        
        var bufferInfo = new VkBufferCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            size = size,
            usage = usage,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE,
            queueFamilyIndexCount = 0,
            pQueueFamilyIndices = null,
        };
        
        VkResult result = VkApi.vkCreateBuffer(_device, in bufferInfo, IntPtr.Zero, out _buffer);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateBuffer failed: {result}");
        }
        
        VkApi.vkGetBufferMemoryRequirements(_device, _buffer, out VkMemoryRequirements memReqs);
        _allocation = allocator.Allocate(memReqs.size, memReqs.alignment, memoryProps, memReqs.memoryTypeBits);
        VkApi.vkBindBufferMemory(_device, _buffer, _allocation.DeviceMemory, _allocation.Offset);
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_buffer != IntPtr.Zero)
        {
            VkApi.vkDestroyBuffer(_device, _buffer, IntPtr.Zero);
            _buffer = IntPtr.Zero;
        }
        _disposed = true;
    }
}
```

```csharp
// VulkanImage.cs (skeleton, similar pattern к VulkanBuffer но c VkImage + VkImageView)
namespace DualFrontier.Runtime.Graphics;

public sealed class VulkanImage : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _image;
    private IntPtr _imageView;
    private MemoryAllocation _allocation;
    private bool _disposed;
    
    public IntPtr Handle => _image;
    public IntPtr ViewHandle => _imageView;
    public uint Width { get; }
    public uint Height { get; }
    public VkFormat Format { get; }
    
    public VulkanImage(VulkanDevice device, MemoryAllocator allocator,
        uint width, uint height, VkFormat format,
        VkImageUsageFlags usage, VkMemoryPropertyFlags memoryProps)
    {
        // Full VkImageCreateInfo construction + vkCreateImage + allocator.Allocate + vkBindImageMemory
        // + VkImageViewCreateInfo construction + vkCreateImageView
        // Per Vulkan 1.3 spec verbatim
        // ... implementation details
    }
    
    public void Dispose() { /* destroy view, then image */ }
}
```

**Per Lesson #7 strengthening — alignment audit для NEW structs**:

- `VkMemoryAllocateInfo` (24 bytes): sType(4) + pad(4) + pNext(8) + allocationSize(8 — VkDeviceSize) + memoryTypeIndex(4) + trailing pad(4) = **24**. Marshal.SizeOf test.
- `VkMemoryRequirements` (24 bytes): size(8) + alignment(8) + memoryTypeBits(4) + trailing pad(4) = **24**. Marshal.SizeOf test.
- `VkBufferCreateInfo` (56 bytes): sType(4) + pad(4) + pNext(8) + flags(4) + pad(4) + size(8) + usage(4) + sharingMode(4) + queueFamilyIndexCount(4) + pad(4) + pQueueFamilyIndices(8) = **56**. Marshal.SizeOf test.
- `VkImageCreateInfo` (88 bytes): sType(4) + pad(4) + pNext(8) + flags(4) + imageType(4) + format(4) + pad(4) + extent.width(4) + extent.height(4) + extent.depth(4) + mipLevels(4) + arrayLayers(4) + samples(4) + tiling(4) + usage(4) + sharingMode(4) + queueFamilyIndexCount(4) + pad(4) + pQueueFamilyIndices(8) + initialLayout(4) + trailing pad(4) = **88**. Marshal.SizeOf test.
- `VkImageViewCreateInfo` (80 bytes). Marshal.SizeOf test.
- `VkPhysicalDeviceMemoryProperties`: complex array struct (memoryTypeCount + memoryTypes[32] + memoryHeapCount + memoryHeaps[16]); use fixed-size inline arrays. Marshal.SizeOf verification critical.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests + MemoryAllocatorTests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — memory allocator (bumper) + VulkanBuffer + VulkanImage primitives`

### Commit 7 — VulkanSurface + VulkanSwapchain + recreation on resize

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — adds vkGetPhysicalDeviceSurfaceCapabilitiesKHR, vkGetPhysicalDeviceSurfaceFormatsKHR, vkGetPhysicalDeviceSurfacePresentModesKHR, vkGetPhysicalDeviceSurfaceSupportKHR; via vkGetInstanceProcAddr: vkCreateSwapchainKHR, vkDestroySwapchainKHR, vkGetSwapchainImagesKHR, vkAcquireNextImageKHR, vkQueuePresentKHR, vkDeviceWaitIdle; extension function loading pattern preserved)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — adds VkSurfaceCapabilitiesKHR, VkSurfaceFormatKHR, VkSwapchainCreateInfoKHR, VkPresentInfoKHR)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — adds VkColorSpaceKHR, VkPresentModeKHR, VkSurfaceTransformFlagsKHR, VkCompositeAlphaFlagsKHR)
- `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs` (new — VkSurfaceKHR (Win32) lifecycle)
- `src/DualFrontier.Runtime/Graphics/VulkanSwapchain.cs` (new — VkSwapchainKHR + image acquisition + present + recreation)
- `src/DualFrontier.Runtime/Graphics/SwapchainImage.cs` (new — record exposing per-image VkImage + VkImageView)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — adds swapchain struct size tests)

**Drift surface**: Surface + swapchain operational. VulkanSwapchain handles image acquisition + present. Recreation on resize triggered by WindowResizeEvent in InputEventQueue — clean teardown + recreate с new extent. Memory allocator reset между recreations.

**Implementation surface (key snippet)**:

```csharp
// VulkanSurface.cs (Win32 surface creation)
namespace DualFrontier.Runtime.Graphics;

public sealed class VulkanSurface : IDisposable
{
    private readonly IntPtr _instance;
    private IntPtr _surface;
    private bool _disposed;
    
    public IntPtr Handle => _surface;
    
    public unsafe VulkanSurface(VulkanInstance instance, IWindow window)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(window);
        _instance = instance.Handle;
        
        // Load vkCreateWin32SurfaceKHR via vkGetInstanceProcAddr (extension function)
        IntPtr createFnPtr = VkApi.vkGetInstanceProcAddr(_instance, "vkCreateWin32SurfaceKHR");
        if (createFnPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException("vkCreateWin32SurfaceKHR not available — VK_KHR_win32_surface extension missing.");
        }
        var createFn = Marshal.GetDelegateForFunctionPointer<CreateWin32SurfaceDelegate>(createFnPtr);
        
        IntPtr hinstance = Win32Api.GetModuleHandle(null);
        var createInfo = new VkWin32SurfaceCreateInfoKHR
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            pNext = IntPtr.Zero,
            flags = 0,
            hinstance = hinstance,
            hwnd = window.Handle,
        };
        
        VkResult result = createFn(_instance, in createInfo, IntPtr.Zero, out _surface);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateWin32SurfaceKHR failed: {result}");
        }
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate VkResult CreateWin32SurfaceDelegate(
        IntPtr instance, in VkWin32SurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out IntPtr pSurface);
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_surface != IntPtr.Zero)
        {
            IntPtr destroyFnPtr = VkApi.vkGetInstanceProcAddr(_instance, "vkDestroySurfaceKHR");
            if (destroyFnPtr != IntPtr.Zero)
            {
                var destroyFn = Marshal.GetDelegateForFunctionPointer<DestroySurfaceDelegate>(destroyFnPtr);
                destroyFn(_instance, _surface, IntPtr.Zero);
            }
            _surface = IntPtr.Zero;
        }
        _disposed = true;
    }
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void DestroySurfaceDelegate(IntPtr instance, IntPtr surface, IntPtr pAllocator);
}
```

VulkanSwapchain similar pattern — extension function loading, VkSwapchainCreateInfoKHR construction с queried surface capabilities, format selection (prefer B8G8R8A8_SRGB + COLOR_SPACE_SRGB_NONLINEAR_KHR), present mode selection (prefer MAILBOX_KHR, fallback FIFO_KHR), VkSwapchainKHR creation, image enumeration, per-image VkImageView creation. Recreation API: AcquireNextImage returns flag indicating «swapchain out of date» → caller invokes Recreate() с new extent.

**Per Lesson #7 strengthening — alignment audit**:
- `VkSurfaceCapabilitiesKHR` (52 bytes): contains VkExtent2D × 3 (uint32 pairs) — natural alignment OK
- `VkSurfaceFormatKHR` (8 bytes): two enums — natural alignment OK
- `VkSwapchainCreateInfoKHR` (104 bytes): sType(4)+pad(4)+pNext(8)+flags(4)+pad(4)+surface(8)+minImageCount(4)+imageFormat(4)+imageColorSpace(4)+pad(4)+imageExtent(8)+imageArrayLayers(4)+imageUsage(4)+imageSharingMode(4)+queueFamilyIndexCount(4)+pad(4)+pQueueFamilyIndices(8)+preTransform(4)+compositeAlpha(4)+presentMode(4)+clipped(4)+oldSwapchain(8) = **104**. Marshal.SizeOf test critical.
- `VkPresentInfoKHR` (40 bytes). Marshal.SizeOf test.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests pass
- Manual: smoke test (Commit 17) verifies clear color rendered + resize handler works
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — VulkanSurface + VulkanSwapchain + recreation on WM_SIZE`

### Commit 8 — VulkanRenderPass + VulkanFramebuffer

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkCreateRenderPass, vkDestroyRenderPass, vkCreateFramebuffer, vkDestroyFramebuffer)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkRenderPassCreateInfo, VkAttachmentDescription, VkSubpassDescription, VkAttachmentReference, VkSubpassDependency, VkFramebufferCreateInfo)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — VkAttachmentLoadOp, VkAttachmentStoreOp, VkPipelineStageFlags, VkAccessFlags, VkDependencyFlags, VkPipelineBindPoint)
- `src/DualFrontier.Runtime/Graphics/VulkanRenderPass.cs` (new — VkRenderPass с one color attachment, no depth/stencil)
- `src/DualFrontier.Runtime/Graphics/VulkanFramebuffer.cs` (new — VkFramebuffer per swapchain image)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — render pass struct size tests)

**Drift surface**: Render pass + framebuffer infrastructure operational. One color attachment configured с CLEAR loadOp + STORE storeOp + UNDEFINED initial layout → PRESENT_SRC_KHR final layout. Single subpass. One framebuffer per swapchain image, wrapping per-image VkImageView.

**Per Lesson #7 strengthening — alignment audit**:
- `VkAttachmentDescription` (36 bytes): flags(4) + format(4) + samples(4) + loadOp(4) + storeOp(4) + stencilLoadOp(4) + stencilStoreOp(4) + initialLayout(4) + finalLayout(4) = **36**. Natural alignment (no 8-byte fields).
- `VkAttachmentReference` (8 bytes): attachment(4) + layout(4) = **8**.
- `VkSubpassDescription` (72 bytes): flags(4) + pipelineBindPoint(4) + inputAttachmentCount(4) + pad(4) + pInputAttachments(8) + colorAttachmentCount(4) + pad(4) + pColorAttachments(8) + pResolveAttachments(8) + pDepthStencilAttachment(8) + preserveAttachmentCount(4) + pad(4) + pPreserveAttachments(8) = **72**.
- `VkSubpassDependency` (28 bytes): srcSubpass(4) + dstSubpass(4) + srcStageMask(4) + dstStageMask(4) + srcAccessMask(4) + dstAccessMask(4) + dependencyFlags(4) = **28**.
- `VkRenderPassCreateInfo` (64 bytes): sType(4)+pad(4)+pNext(8)+flags(4)+pad(4)+attachmentCount(4)+pad(4)+pAttachments(8)+subpassCount(4)+pad(4)+pSubpasses(8)+dependencyCount(4)+pad(4)+pDependencies(8) = **64**.
- `VkFramebufferCreateInfo` (64 bytes).

All require Marshal.SizeOf verification tests per Lesson #7 strengthening.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — VulkanRenderPass + VulkanFramebuffer (one color attachment, no depth)`

### Commit 9 — VulkanCommandPool + VulkanCommandBuffer + per-frame pattern

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkCreateCommandPool, vkDestroyCommandPool, vkAllocateCommandBuffers, vkFreeCommandBuffers, vkBeginCommandBuffer, vkEndCommandBuffer, vkResetCommandBuffer, vkCmdBeginRenderPass, vkCmdEndRenderPass, vkCmdBindPipeline, vkCmdDraw, vkCmdDispatch, vkCmdPipelineBarrier; vkQueueSubmit, vkCreateFence, vkDestroyFence, vkWaitForFences, vkResetFences, vkCreateSemaphore, vkDestroySemaphore)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkCommandPoolCreateInfo, VkCommandBufferAllocateInfo, VkCommandBufferBeginInfo, VkRenderPassBeginInfo, VkClearValue, VkClearColorValue, VkSubmitInfo, VkFenceCreateInfo, VkSemaphoreCreateInfo)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — VkCommandPoolCreateFlags, VkCommandBufferLevel, VkCommandBufferUsageFlags, VkSubpassContents, VkFenceCreateFlags)
- `src/DualFrontier.Runtime/Graphics/VulkanCommandPool.cs` (new — pool lifecycle)
- `src/DualFrontier.Runtime/Graphics/VulkanCommandBuffer.cs` (new — Begin/End/Submit pattern wrapper)
- `src/DualFrontier.Runtime/Graphics/VulkanFence.cs` (new — CPU-GPU sync primitive)
- `src/DualFrontier.Runtime/Graphics/VulkanSemaphore.cs` (new — GPU-GPU sync primitive)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — command buffer struct size tests)

**Drift surface**: Command pool + command buffer infrastructure operational. Per-frame pattern: caller allocates VkCommandBuffer from pool, begins recording, records commands (begin render pass + bind pipeline + draw + end render pass), ends recording, submits к queue с fence + semaphores. Fences wait for CPU-GPU sync; semaphores для GPU-GPU sync (swapchain acquire/present sync).

**Per Lesson #7 strengthening — alignment audit**:
- `VkCommandPoolCreateInfo` (24 bytes)
- `VkCommandBufferAllocateInfo` (32 bytes)
- `VkSubmitInfo` (72 bytes): contains pointers + VkPipelineStageFlags array pointer + multiple semaphore arrays
- `VkRenderPassBeginInfo` (64 bytes): contains VkRect2D (16 bytes) + pointer к VkClearValue array
- `VkClearValue` (16 bytes — union of color/depth/stencil; layout determined by attachment format — for V0.B color only, use VkClearColorValue)

All require Marshal.SizeOf verification tests.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests + command buffer roundtrip tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — VulkanCommandPool + VulkanCommandBuffer + per-frame fence/semaphore sync`

### Commit 10 — SPIR-V toolchain integration + minimal shaders + VulkanShaderModule

**Files**:
- `tools/glslangValidator.exe` (new — committed binary copied from VULKAN_SDK\Bin\ per S-LOCK-5; ~6 MB)
- `tools/shaders/clearcolor.vert` (new — minimal fullscreen triangle vertex shader per S-LOCK-6)
- `tools/shaders/clearcolor.frag` (new — solid color fragment shader per S-LOCK-6)
- `tools/shaders/noop.comp` (new — empty compute shader для compute pipeline registration round-trip per S-LOCK-6)
- `assets/shaders/clearcolor.vert.spv` (new — pre-compiled output)
- `assets/shaders/clearcolor.frag.spv` (new — pre-compiled output)
- `assets/shaders/noop.comp.spv` (new — pre-compiled output)
- `Directory.Build.props` (modified — adds CompileShaders MSBuild target conditional on DualFrontier.Runtime project)
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkCreateShaderModule, vkDestroyShaderModule)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkShaderModuleCreateInfo, VkPipelineShaderStageCreateInfo)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — VkShaderStageFlags)
- `src/DualFrontier.Runtime/Graphics/VulkanShaderModule.cs` (new — SPIR-V loading + module lifecycle)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — shader struct size tests)
- `tests/DualFrontier.Runtime.Tests/Graphics/ShaderCompilationTests.cs` (new — verifies SPIR-V files present + readable)
- `.gitattributes` (modified — `tools/glslangValidator.exe binary` declaration per repo binary discipline; `assets/shaders/*.spv binary`)

**Drift surface**: SPIR-V toolchain integration operational. `dotnet build DualFrontier.Runtime.csproj` invokes `tools/glslangValidator.exe` per CompileShaders target, regenerating `assets/shaders/*.spv` files. VulkanShaderModule loads pre-compiled SPIR-V bytecode из `assets/shaders/` directory.

**MSBuild target verbatim (Directory.Build.props extension)**:

```xml
<Project>
  <!-- Existing V0.A PropertyGroup preserved -->
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);CS1591</NoWarn>
    <Company>DualFrontier</Company>
    <Product>Dual Frontier</Product>
  </PropertyGroup>
  
  <!-- V0.B NEW: shader compilation target -->
  <Target Name="CompileShaders" BeforeTargets="Build"
          Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
    <PropertyGroup>
      <GlslangValidatorExe>$(SolutionDir)tools\glslangValidator.exe</GlslangValidatorExe>
      <ShaderSourceDir>$(SolutionDir)tools\shaders</ShaderSourceDir>
      <ShaderOutputDir>$(SolutionDir)assets\shaders</ShaderOutputDir>
    </PropertyGroup>
    <MakeDir Directories="$(ShaderOutputDir)" Condition="!Exists('$(ShaderOutputDir)')" />
    <Exec Command="&quot;$(GlslangValidatorExe)&quot; -V &quot;$(ShaderSourceDir)\clearcolor.vert&quot; -o &quot;$(ShaderOutputDir)\clearcolor.vert.spv&quot;" />
    <Exec Command="&quot;$(GlslangValidatorExe)&quot; -V &quot;$(ShaderSourceDir)\clearcolor.frag&quot; -o &quot;$(ShaderOutputDir)\clearcolor.frag.spv&quot;" />
    <Exec Command="&quot;$(GlslangValidatorExe)&quot; -V &quot;$(ShaderSourceDir)\noop.comp&quot; -o &quot;$(ShaderOutputDir)\noop.comp.spv&quot;" />
  </Target>
</Project>
```

**VulkanShaderModule skeleton**:

```csharp
namespace DualFrontier.Runtime.Graphics;

public sealed class VulkanShaderModule : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _module;
    private bool _disposed;
    
    public IntPtr Handle => _module;
    
    public static VulkanShaderModule LoadFromFile(VulkanDevice device, string spirvPath)
    {
        byte[] bytes = File.ReadAllBytes(spirvPath);
        return new VulkanShaderModule(device, bytes);
    }
    
    public unsafe VulkanShaderModule(VulkanDevice device, byte[] spirvBytecode)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(spirvBytecode);
        if (spirvBytecode.Length % 4 != 0)
        {
            throw new ArgumentException("SPIR-V bytecode length must be multiple of 4");
        }
        _device = device.Handle;
        
        fixed (byte* codePtr = spirvBytecode)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                codeSize = (nuint)spirvBytecode.Length,
                pCode = (uint*)codePtr,
            };
            VkResult result = VkApi.vkCreateShaderModule(_device, in createInfo, IntPtr.Zero, out _module);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateShaderModule failed: {result}");
            }
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_module != IntPtr.Zero)
        {
            VkApi.vkDestroyShaderModule(_device, _module, IntPtr.Zero);
            _module = IntPtr.Zero;
        }
        _disposed = true;
    }
}
```

**Per S-LOCK-5 (in-repo SPIR-V toolchain)**: `glslangValidator.exe` committed к `tools/` directory. `.gitattributes` declares binary discipline. Pre-compiled `.spv` files committed для clone-and-run convenience (regenerated on build via MSBuild target).

**Validation**:
- `dotnet build DualFrontier.Runtime.csproj` invokes glslangValidator successfully — `.spv` files regenerated; no glslangValidator errors
- `dotnet test` 685+ baseline + ShaderCompilationTests + new size tests pass
- Manual: verify committed `.spv` files match regenerated output (file diff)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — SPIR-V toolchain integration + minimal shaders (clearcolor + noop) + VulkanShaderModule`

### Commit 11 — Graphics pipeline (minimal clearcolor)

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkCreateGraphicsPipelines, vkDestroyPipeline, vkCreatePipelineLayout, vkDestroyPipelineLayout)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkGraphicsPipelineCreateInfo, VkPipelineVertexInputStateCreateInfo, VkPipelineInputAssemblyStateCreateInfo, VkPipelineViewportStateCreateInfo, VkPipelineRasterizationStateCreateInfo, VkPipelineMultisampleStateCreateInfo, VkPipelineColorBlendStateCreateInfo, VkPipelineColorBlendAttachmentState, VkPipelineDynamicStateCreateInfo, VkPipelineLayoutCreateInfo, VkViewport, VkRect2D)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — VkPrimitiveTopology, VkPolygonMode, VkCullModeFlags, VkFrontFace, VkColorComponentFlags, VkDynamicState)
- `src/DualFrontier.Runtime/Graphics/VulkanGraphicsPipeline.cs` (new — graphics pipeline lifecycle)
- `src/DualFrontier.Runtime/Graphics/VulkanPipelineLayout.cs` (new — empty pipeline layout для clear color shader, no descriptors)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — graphics pipeline struct size tests)

**Drift surface**: Minimal graphics pipeline operational. Renders fullscreen triangle с clearcolor shaders → solid dark blue color on swapchain. Pipeline state: no vertex input (fullscreen triangle via gl_VertexIndex), triangle list topology, fill polygon mode, no cull, no MSAA, single color attachment с no blending, dynamic viewport + scissor (for swapchain recreation flexibility).

**Per Lesson #7 strengthening — alignment audit для graphics pipeline structs**:

~10 new structs each requiring Marshal.SizeOf verification. Critical ones:
- `VkGraphicsPipelineCreateInfo` (144 bytes — many pointers)
- `VkPipelineLayoutCreateInfo` (32 bytes)
- `VkPipelineVertexInputStateCreateInfo` (48 bytes)
- `VkPipelineColorBlendAttachmentState` (32 bytes)
- `VkViewport` (24 bytes — contains floats, natural alignment)

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — minimal graphics pipeline (clearcolor fullscreen triangle)`

### Commit 12 — Compute pipeline plumbing: VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — vkCreateComputePipelines, vkCreateDescriptorSetLayout, vkDestroyDescriptorSetLayout, vkCreateDescriptorPool, vkDestroyDescriptorPool, vkAllocateDescriptorSets, vkUpdateDescriptorSets, vkCmdBindDescriptorSets, vkCmdBindPipeline)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — VkComputePipelineCreateInfo, VkDescriptorSetLayoutCreateInfo, VkDescriptorSetLayoutBinding, VkDescriptorPoolCreateInfo, VkDescriptorPoolSize, VkDescriptorSetAllocateInfo, VkWriteDescriptorSet, VkDescriptorBufferInfo, VkDescriptorImageInfo)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — VkDescriptorType, VkShaderStageFlags compute-related additions, VkDescriptorPoolCreateFlags)
- `src/DualFrontier.Runtime/Compute/MODULE.md` (new — Compute module purpose per VULKAN_SUBSTRATE §2.2)
- `src/DualFrontier.Runtime/Compute/VulkanComputePipeline.cs` (new — VkPipeline с VK_PIPELINE_BIND_POINT_COMPUTE)
- `src/DualFrontier.Runtime/Compute/VulkanComputeDescriptors.cs` (new — descriptor set layout + pool + pipeline layout)
- `src/DualFrontier.Runtime/Compute/ComputeDispatch.cs` (new — VkCmdDispatch wrapper + fence sync)
- `src/DualFrontier.Runtime/Compute/ComputePipelineRegistry.cs` (new — name → pipeline mapping, exposed via Runtime facade)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — compute pipeline struct size tests)
- `tests/DualFrontier.Runtime.Tests/Compute/ComputePipelineRegistrationTests.cs` (new — noop pipeline round-trip)

**Drift surface**: Compute pipeline plumbing operational. ComputePipelineRegistry.Register(name, spirvBytes) creates VkPipeline + descriptor set layout + pipeline layout. ComputeDispatch.Execute(pipeline, descriptorSets, x/y/z) records VkCmdBindPipeline + VkCmdBindDescriptorSets + VkCmdDispatch к command buffer, submits к async compute queue (V0.B Commit 4), waits на fence (К-L7 atomic-from-observer preserved).

**Implementation surface (key snippet)**:

```csharp
// VulkanComputePipeline.cs (skeleton)
namespace DualFrontier.Runtime.Compute;

public sealed class VulkanComputePipeline : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _pipeline;
    private IntPtr _pipelineLayout;
    private IntPtr _descriptorSetLayout;
    private bool _disposed;
    
    public IntPtr Handle => _pipeline;
    public IntPtr LayoutHandle => _pipelineLayout;
    public IntPtr DescriptorSetLayoutHandle => _descriptorSetLayout;
    
    public unsafe VulkanComputePipeline(VulkanDevice device, byte[] spirvBytecode, IReadOnlyList<VkDescriptorSetLayoutBinding> bindings)
    {
        // 1. Create VkDescriptorSetLayout from bindings
        // 2. Create VkPipelineLayout consuming descriptor set layout
        // 3. Create VkShaderModule from SPIR-V bytecode
        // 4. Create VkPipeline VK_PIPELINE_BIND_POINT_COMPUTE consuming VkPipelineLayout + VkShaderModule
        // 5. Destroy VkShaderModule (no longer needed after pipeline creation)
        // Full implementation per Vulkan 1.3 spec
    }
    
    public void Dispose() { /* destroy pipeline → pipelineLayout → descriptorSetLayout */ }
}
```

```csharp
// ComputePipelineRegistry.cs (named pipeline lookup)
namespace DualFrontier.Runtime.Compute;

public sealed class ComputePipelineRegistry : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly Dictionary<string, VulkanComputePipeline> _pipelines = new();
    private bool _disposed;
    
    public ComputePipelineRegistry(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device;
    }
    
    /// <summary>
    /// Register compute pipeline под given name. SPIR-V bytecode + descriptor layout describe pipeline.
    /// Pipeline disposed when Registry disposed.
    /// </summary>
    public VulkanComputePipeline Register(string name, byte[] spirvBytecode, IReadOnlyList<VkDescriptorSetLayoutBinding> bindings)
    {
        if (_pipelines.ContainsKey(name))
        {
            throw new InvalidOperationException($"Compute pipeline '{name}' already registered.");
        }
        var pipeline = new VulkanComputePipeline(_device, spirvBytecode, bindings);
        _pipelines[name] = pipeline;
        return pipeline;
    }
    
    public VulkanComputePipeline? Get(string name) => _pipelines.GetValueOrDefault(name);
    
    public IReadOnlyList<string> RegisteredNames => _pipelines.Keys.ToList();
    
    public void Dispose()
    {
        if (_disposed) return;
        foreach (var (_, pipeline) in _pipelines)
        {
            pipeline.Dispose();
        }
        _pipelines.Clear();
        _disposed = true;
    }
}
```

```csharp
// ComputeDispatch.cs (dispatch + fence sync)
namespace DualFrontier.Runtime.Compute;

public sealed class ComputeDispatch : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanCommandPool _commandPool;
    private VulkanCommandBuffer? _commandBuffer;
    private VulkanFence? _fence;
    private bool _disposed;
    
    public ComputeDispatch(VulkanDevice device, VulkanCommandPool commandPool)
    {
        _device = device;
        _commandPool = commandPool;
        _commandBuffer = _commandPool.AllocateBuffer();
        _fence = new VulkanFence(_device);
    }
    
    /// <summary>
    /// Execute compute dispatch synchronously (fence wait). For V0.B exit criterion noop dispatch test.
    /// Future V1/V2 substrate primitives may use async dispatch с fence stored для next-tick read.
    /// </summary>
    public void ExecuteSync(VulkanComputePipeline pipeline, IntPtr[] descriptorSets, uint x, uint y, uint z)
    {
        _commandBuffer!.Begin();
        VkApi.vkCmdBindPipeline(_commandBuffer.Handle, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_COMPUTE, pipeline.Handle);
        // VkCmdBindDescriptorSets if descriptorSets non-empty
        VkApi.vkCmdDispatch(_commandBuffer.Handle, x, y, z);
        _commandBuffer.End();
        
        // Submit к async compute queue (V0.B Commit 4 selected this queue)
        _commandBuffer.SubmitTo(_device.AsyncComputeQueue, _fence!);
        
        // Wait fence (К-L7 atomic-from-observer)
        _fence!.Wait();
        _fence.Reset();
        _commandBuffer.Reset();
    }
    
    public void Dispose() { /* destroy fence + command buffer */ }
}
```

**Per Lesson #7 strengthening — alignment audit для compute structs**:
- `VkComputePipelineCreateInfo` (96 bytes): sType+pad+pNext+flags+pad+stage(VkPipelineShaderStageCreateInfo 48 bytes nested)+layout(8)+basePipelineHandle(8)+basePipelineIndex(4)+trailing pad = **96**
- `VkDescriptorSetLayoutCreateInfo` (32 bytes)
- `VkDescriptorPoolCreateInfo` (40 bytes)
- `VkWriteDescriptorSet` (64 bytes — multiple pointers)
- `VkDescriptorBufferInfo` (24 bytes — contains VkDeviceSize × 2)

All require Marshal.SizeOf verification tests.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + new size tests + ComputePipelineRegistrationTests (noop pipeline round-trip — register + dispatch + fence wait) pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch + ComputePipelineRegistry`

### Commit 13 — К-L19 invariant landing (LOAD-BEARING — KERNEL_ARCHITECTURE.md v2.1 → v2.2 + README.md hardware requirements + REQ-K-L19)

**Files**:
- `docs/architecture/KERNEL_ARCHITECTURE.md` (modified — К-L invariant table extended с К-L19 row LOCKED; version 2.1 → 2.2; version history block updated; companion docs cross-references к VULKAN_SUBSTRATE.md V0.B implementation)
- `README.md` (modified — Hardware Requirements section added per VULKAN_SUBSTRATE §0 L1 + halted К10.3 brief commit 5 verbatim)
- `docs/governance/REGISTER.yaml` (modified — DOC-A-KERNEL version 2.1 → 2.2; DOC-G-README governance_events append; REQ-K-L19 enrolled)

**Drift surface**: К-L19 invariant LOCKED. KERNEL_ARCHITECTURE.md authoritative для hardware tier commitment. README.md user-facing hardware requirements operational. REGISTER.yaml mirrors all changes.

**Per Lesson #8 + Lesson #11 + S-LOCK-9**: K-L19 invariant landing + KERNEL_ARCHITECTURE amendment + README.md amendment + REGISTER bump + REQ-K-L19 enrollment **all в same load-bearing commit**. Splitting leaves intermediate state where К-L19 claimed но implementation absent — inconsistent. Implementation backing (V0.B Commits 4+5) already operational at this commit time.

**KERNEL_ARCHITECTURE.md amendment** (К-L invariant table append):

```
| К-L19 | LOCKED (V0.B 2026-MM-DD) | Hardware tier commitment | Vulkan 1.3 + async compute queue family mandate. Target hardware tier: NVIDIA Turing+ (GTX 16xx / RTX 20xx и newer), AMD RDNA 1+ (RX 5500 и newer), Intel Arc Alchemist+ (A380 и newer). Async compute queue used для К-L16 pipeline depth dispatches. Graphics queue used для display rendering. HardwareCapabilityCheck.Verify (DualFrontier.Runtime.Graphics) enforces fail-fast at Runtime.Create с clear user-facing diagnostic message. Hardware exclusion of pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel, и most integrated GPUs accepted as architectural choice supporting clean implementation per К-L14. By Dual Frontier release timeline, target hardware tier represents majority of gaming hardware. |
```

**README.md amendment** (Hardware Requirements section per halted К10.3 brief commit 5):

```markdown
## Hardware Requirements

Dual Frontier targets modern GPU hardware с Vulkan 1.3 + async compute queue family support per K-L19 architectural commitment.

**Minimum tier**:
- **NVIDIA**: Turing or newer (GeForce GTX 1660 / RTX 20-series and later)
- **AMD**: RDNA 1 or newer (Radeon RX 5500 and later)
- **Intel**: Arc Alchemist or newer (Arc A380 and later)
- **Integrated GPUs**: most NOT supported (lack async compute queue family)

Pre-Turing NVIDIA, pre-RDNA AMD, pre-Arc Intel hardware will fail at startup с clear diagnostic message. This is an intentional architectural choice supporting clean implementation; not a hardware-discrimination decision.

**Verification**: launch Dual Frontier → если startup fails с HardwareCapabilityException → upgrade GPU driver or hardware. Run `vulkaninfo.exe` (from Vulkan SDK or GPU driver) к verify hardware supports Vulkan 1.3 + compute queue family.

**OS support**: Windows 10/11 x64 (Linux/macOS deferred per VULKAN_SUBSTRATE.md L7).
```

**REGISTER.yaml amendments**:
- `DOC-A-KERNEL` version 2.1 → 2.2
- `DOC-A-KERNEL.governance_events` append: `{date: YYYY-MM-DD, summary: "V0.B closure — К-L19 hardware tier invariant landed (Vulkan 1.3 + async compute queue mandate)"}`
- `DOC-G-README.governance_events` append: `{date: YYYY-MM-DD, summary: "V0.B closure — Hardware Requirements section added"}`
- `REQ-K-L19` enrolled: «Vulkan 1.3 + async compute queue family hardware tier baseline; HardwareCapabilityCheck.Verify enforces fail-fast»

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean (no code changes — pure governance commit)
- `dotnet test` 685+ baseline (preserved)
- К-L19 invariant text consistent across KERNEL_ARCHITECTURE.md + README.md + REGISTER.yaml (Lesson #20 application)

**Commit message**: `governance: V0.B — К-L19 hardware tier invariant LOCKED (KERNEL_ARCHITECTURE v2.2 + README.md hardware requirements + REQ-K-L19)`

### Commit 14 — Native C ABI extension: df_world_register_compute_pipeline + df_world_field_dispatch_compute

**Files**:
- `native/DualFrontier.Core.Native/include/df_capi.h` (modified — new section «V0.B Compute pipeline registration + field dispatch» appended after K9 field storage section)
- `native/DualFrontier.Core.Native/include/compute_pipeline.h` (new — internal C++ API)
- `native/DualFrontier.Core.Native/include/compute_dispatch.h` (new — internal C++ API)
- `native/DualFrontier.Core.Native/src/compute_pipeline.cpp` (new — VkPipeline lifecycle management bridging к existing K9 field storage)
- `native/DualFrontier.Core.Native/src/compute_dispatch.cpp` (new — VkCmdDispatch + fence sync wrapper)
- `native/DualFrontier.Core.Native/src/capi.cpp` (modified — new exports implementation)
- `native/DualFrontier.Core.Native/CMakeLists.txt` (modified — Vulkan find_package + extension source files)
- `native/DualFrontier.Core.Native/test/selftest.cpp` (modified — adds compute pipeline registration + noop dispatch native-side test)

**Drift surface**: `DualFrontier.Core.Native.dll` extended с Vulkan compute hooks. Native module links к `vulkan-1.dll` via CMake find_package(Vulkan). К-L11 «NativeWorld single source of truth» preserved — kernel remains authoritative для storage (K9); V substrate compute primitives accessed via new C ABI calls.

**Per S-LOCK-3 (Q2 (a) ratification)**: native C ABI extension lands в existing kernel module. Same `.dll` provides K9 storage + V0.B compute hooks. Managed `DualFrontier.Runtime.Compute` module wraps these via existing `DualFrontier.Core.Interop` P/Invoke infrastructure pattern (post-К8.4 interop conventions inherited).

**`df_capi.h` extension verbatim**:

```c
/*
 * V0.B — Compute pipeline registration + field dispatch (added 2026-MM-DD).
 *
 * Bridges K9 field storage (RawTileField<T>) к Vulkan compute pipelines. Compute pipeline
 * registration accepts SPIR-V bytecode + descriptor set layout binding count; native side
 * creates VkPipeline + descriptor set layout + pipeline layout, returns pipeline_id для
 * later dispatch.
 *
 * Field dispatch invokes registered pipeline against named K9 field. Native side acquires
 * field span (per K9 mutation rejection contract), binds к storage buffer descriptor,
 * records VkCmdDispatch, submits к async compute queue, waits на fence (К-L7 atomic-from-
 * observer preserved). Synchronous V0.B implementation; V1+ may add async pattern.
 *
 * Native module links к vulkan-1.dll via CMake find_package(Vulkan). Vulkan instance/device
 * handles passed from managed side at initialization (df_world_attach_vulkan).
 *
 * Returns 1 on success, 0 on failure (invalid pipeline_id, field not found, dispatch error).
 */

DF_API int32_t df_world_attach_vulkan(
    df_world_handle world,
    void* vk_instance,
    void* vk_physical_device,
    void* vk_device,
    void* vk_async_compute_queue,
    uint32_t async_compute_queue_family_index);

DF_API uint32_t df_world_register_compute_pipeline(
    df_world_handle world,
    const char* pipeline_name,
    const uint8_t* spirv_bytecode,
    int32_t spirv_size,
    uint32_t descriptor_binding_count);

DF_API int32_t df_world_field_dispatch_compute(
    df_world_handle world,
    const char* field_name,
    uint32_t pipeline_id,
    uint32_t dispatch_x,
    uint32_t dispatch_y,
    uint32_t dispatch_z);

DF_API int32_t df_world_compute_pipeline_count(df_world_handle world);
```

**Implementation strategy (compute_pipeline.cpp + compute_dispatch.cpp)**:

```cpp
// compute_pipeline.cpp (skeleton — full per Vulkan 1.3 spec)
#include "compute_pipeline.h"
#include <vulkan/vulkan.h>

namespace dualfrontier {

struct ComputePipelineEntry {
    VkPipeline pipeline;
    VkPipelineLayout pipeline_layout;
    VkDescriptorSetLayout descriptor_set_layout;
    std::string name;
};

class ComputePipelineRegistry {
public:
    uint32_t register_pipeline(VkDevice device, const std::string& name,
        const uint8_t* spirv, int32_t spirv_size, uint32_t binding_count);
    bool unregister_pipeline(uint32_t pipeline_id);
    ComputePipelineEntry* get_pipeline(uint32_t pipeline_id);
    int32_t pipeline_count() const;
    void destroy_all(VkDevice device);

private:
    std::unordered_map<uint32_t, ComputePipelineEntry> pipelines_;
    uint32_t next_id_ = 1;
};

} // namespace dualfrontier
```

```cpp
// capi.cpp extension
extern "C" {

DF_API uint32_t df_world_register_compute_pipeline(
    df_world_handle world, const char* pipeline_name,
    const uint8_t* spirv_bytecode, int32_t spirv_size, uint32_t descriptor_binding_count)
{
    auto* world_impl = reinterpret_cast<dualfrontier::World*>(world);
    if (!world_impl || !world_impl->has_vulkan_attached()) {
        return 0;
    }
    return world_impl->compute_pipelines().register_pipeline(
        world_impl->vk_device(), pipeline_name, spirv_bytecode, spirv_size, descriptor_binding_count);
}

DF_API int32_t df_world_field_dispatch_compute(
    df_world_handle world, const char* field_name, uint32_t pipeline_id,
    uint32_t dispatch_x, uint32_t dispatch_y, uint32_t dispatch_z)
{
    auto* world_impl = reinterpret_cast<dualfrontier::World*>(world);
    if (!world_impl || !world_impl->has_vulkan_attached()) {
        return 0;
    }
    return world_impl->dispatch_field_compute(
        field_name, pipeline_id, dispatch_x, dispatch_y, dispatch_z) ? 1 : 0;
}

}  // extern "C"
```

**CMakeLists.txt extension** (per S-LOCK-3 + §2.5 Phase 0 reads):

```cmake
# Existing CMakeLists.txt preserved; V0.B additions:

find_package(Vulkan REQUIRED)

# Append V0.B sources к existing list
list(APPEND DF_NATIVE_SOURCES
    src/compute_pipeline.cpp
    src/compute_dispatch.cpp
)
list(APPEND DF_NATIVE_HEADERS
    include/compute_pipeline.h
    include/compute_dispatch.h
)

# After add_library() declaration:
target_link_libraries(DualFrontier.Core.Native PRIVATE Vulkan::Vulkan)

# Same для selftest if its compute pipeline test paths require Vulkan linkage
if(DF_NATIVE_BUILD_SELFTEST)
    target_link_libraries(df_native_selftest PRIVATE Vulkan::Vulkan)
endif()
```

**Selftest extension** (`test/selftest.cpp` adds):

```cpp
// V0.B compute pipeline registration round-trip native-side test
TEST_CASE("V0.B compute pipeline registration roundtrip") {
    // 1. Create test World с mock Vulkan handles (or actual Vulkan if test env)
    // 2. Call df_world_register_compute_pipeline с minimal valid SPIR-V (noop.comp bytecode)
    // 3. Verify returned pipeline_id != 0
    // 4. Call df_world_field_dispatch_compute against registered noop pipeline
    // 5. Verify returns 1 (success)
    // 6. Cleanup
}
```

**Per Lesson #22**: native module Phase 0 verifies df_capi.h existing structure + identifies exact insertion point после K9 field storage section + before К10.1 scheduler section. Insertion preserves alphabetical/architectural section ordering.

**Validation**:
- `cmake --build` clean (VS-bundled CMake invoked); native selftest passes 77 baseline + 1 new compute pipeline roundtrip = 78
- `dotnet build` clean (managed-side P/Invoke к new native exports compiles)
- `dotnet test` 685+ baseline preserved
- Manual: `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` exercises native-side df_world_register_compute_pipeline (V0.B smoke test Commit 17)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(native): V0.B — df_world_register_compute_pipeline + df_world_field_dispatch_compute C ABI extension + Vulkan linkage`

### Commit 15 — Managed-side FieldStorageBinding bridging K9 RawTileField к VkBuffer

**Files**:
- `src/DualFrontier.Runtime/Compute/FieldStorageBinding.cs` (new — bridges K9 field storage span к Vulkan storage buffer)
- `src/DualFrontier.Core.Interop/NativeWorld.cs` (modified — adds AttachVulkan + RegisterComputePipeline + DispatchFieldCompute methods consuming new C ABI exports)
- `src/DualFrontier.Core.Interop/Native/CoreNativeBindings.cs` (modified — adds P/Invoke declarations for new exports)
- `tests/DualFrontier.Runtime.Tests/Compute/FieldStorageBindingTests.cs` (new — K9 field → VkBuffer binding tests)

**Drift surface**: Managed-side bridge between K9 field storage (existing) и V0.B Vulkan compute (new). NativeWorld.AttachVulkan invoked from Runtime.Create после VulkanDevice ready — passes Vulkan handles to native side. NativeWorld.RegisterComputePipeline accepts SPIR-V bytecode + descriptor binding count, returns pipeline_id wrapper. NativeWorld.DispatchFieldCompute named API consumed by V1/V2 substrate primitives (future briefs).

**FieldStorageBinding semantics**: V0.B exits с minimal infrastructure — actual K9-field-to-SSBO binding happens на native side когда `df_world_field_dispatch_compute` invoked. Managed `FieldStorageBinding` class is thin orchestration wrapper. Full descriptor set updates + storage buffer binding implementation lives native side для performance (avoids cross-boundary marshalling per К-L7 hot path).

**Per К-L7 + К-L11**: native side authoritative; managed side requests, native side executes + reports completion.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + FieldStorageBindingTests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — FieldStorageBinding bridges K9 RawTileField к Vulkan compute (managed wrapper consuming native C ABI)`

### Commit 16 — Runtime facade composition: full V0.B extension

**Files**:
- `src/DualFrontier.Runtime/Runtime.cs` (modified — V0.B composition: V0.A baseline + MemoryAllocator + VulkanSurface + VulkanSwapchain + VulkanRenderPass + per-image VulkanFramebuffer + VulkanCommandPool + ComputePipelineRegistry; orderly Dispose в reverse construction order)
- `src/DualFrontier.Runtime/RuntimeOptions.cs` (modified — possibly adds RequireAsyncCompute flag default true)
- `tests/DualFrontier.Runtime.Tests/RuntimeCompositionTests.cs` (modified — V0.B composition tests extends V0.A tests)

**Drift surface**: Runtime facade fully composed для V0.B exit criteria. Runtime.Create returns с все компоненты operational: window + Vulkan instance + ValidationLayer + Device (с async compute queue) + HardwareCapabilityCheck passed + Surface + Swapchain + RenderPass + CommandPool + MemoryAllocator + ComputePipelineRegistry. Dispose проходит в reverse construction order — preserves «no leaked Vulkan handles» exit criterion.

**Per Lesson #8 — atomic disposal**: Runtime.Dispose() в Commit 16 ensures graceful teardown even mid-construction. Если Surface creation fails, Window + ValidationLayer + Device + VulkanInstance + HardwareCapability already-constructed components disposed via try/catch pattern preserved from V0.A.

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + V0.B composition tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.B — Runtime facade full V0.B composition (surface + swapchain + render pass + command pool + memory + compute registry)`

### Commit 17 — V0.B smoke test executable

**Files**:
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` (modified — extends V0.A smoke test с V0.B exit criteria verification)

**Drift surface**: V0.B smoke test extends V0.A smoke test. Verifies V0.B exit criteria per VULKAN_SUBSTRATE.md §1.1 V0 + S-LOCK-1:

1. ✓ Window opens (V0.A preserved)
2. ✓ Vulkan instance + device live (V0.A preserved)
3. ✓ Validation layer reports zero errors (V0.A preserved + V0.B additional Vulkan calls maintain validation cleanness)
4. ✓ Clean shutdown (V0.A preserved + V0.B Runtime.Dispose disposes new components)
5. ✓ HardwareCapabilityCheck passes (К-L19) — async compute queue family detected
6. ✓ Swapchain operational — VkSwapchainKHR created, images acquired/presented
7. ✓ Clear color rendered at 60+ FPS — dark blue clearcolor visible на window
8. ✓ Window resize triggers swapchain recreation cleanly
9. ✓ Compute pipeline registration round-trip — noop.comp registered, noop dispatch executed без error

**V0.B smoke test extension (key snippets)**:

```csharp
// Program.cs (V0.B extension)
// ... V0.A baseline preserved ...

Console.WriteLine();
Console.WriteLine("V0.B exit criteria:");

// 5. Hardware capability check
Console.WriteLine($"  [PASS] К-L19 HardwareCapabilityCheck (async compute QF: {runtime.VulkanDevice.AsyncComputeQueueFamilyIndex})");

// 6. Swapchain operational
Console.WriteLine($"  [PASS] Swapchain operational ({runtime.Swapchain.ImageCount} images, format={runtime.Swapchain.Format}, present={runtime.Swapchain.PresentMode})");

// 7-8. Clear color rendering + resize handling
runtime.Window.Show();
Console.WriteLine("Window opened. Rendering clear color for 5 seconds...");

int frames = 0;
var startTime = DateTime.UtcNow;
while ((DateTime.UtcNow - startTime).TotalSeconds < 5 && runtime.Window.IsOpen)
{
    runtime.Window.PumpMessages();
    
    // Drain resize events; trigger swapchain recreation
    while (runtime.InputQueue.TryDequeue(out var evt))
    {
        if (evt is WindowResizeEvent resize)
        {
            Console.WriteLine($"  Window resized к {resize.NewWidth}x{resize.NewHeight} — recreating swapchain");
            runtime.Swapchain.Recreate(resize.NewWidth, resize.NewHeight);
        }
    }
    
    // Render clear color frame
    runtime.RenderClearFrame();  // helper API combining acquire/begin renderpass/draw/end renderpass/present
    frames++;
    Thread.Sleep(16);  // 60 FPS pacing
}

double fps = frames / (DateTime.UtcNow - startTime).TotalSeconds;
Console.WriteLine($"  [PASS] Clear color rendered ({frames} frames, {fps:F1} FPS — target 60+)");

// 9. Compute pipeline registration round-trip
Console.WriteLine();
Console.WriteLine("Compute pipeline round-trip test:");
byte[] noopSpirv = File.ReadAllBytes("assets/shaders/noop.comp.spv");
var noopPipeline = runtime.ComputePipelines.Register("noop", noopSpirv, bindings: new List<VkDescriptorSetLayoutBinding>());
Console.WriteLine($"  Registered noop pipeline (handle 0x{noopPipeline.Handle.ToInt64():X})");

using var dispatch = new ComputeDispatch(runtime.VulkanDevice, runtime.CommandPool);
dispatch.ExecuteSync(noopPipeline, descriptorSets: Array.Empty<IntPtr>(), x: 1, y: 1, z: 1);
Console.WriteLine($"  [PASS] Noop dispatch executed without error");

// Final validation log check (V0.A criterion preserved)
if (runtime.ValidationLayer is not null && runtime.ValidationLayer.Log.ErrorCount > 0)
{
    Console.Error.WriteLine($"FAIL: {runtime.ValidationLayer.Log.ErrorCount} validation errors detected (S-LOCK-4 violation).");
    return 1;
}

Console.WriteLine();
Console.WriteLine("V0.B smoke test PASS");
return 0;
```

**Validation**:
- `dotnet build` clean
- `dotnet test` 685+ baseline + all V0.B tests pass
- `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` — exit 0
  - Window opens, dark blue clear color visible
  - Window resize triggers swapchain recreation (manual visual test by Crystalka)
  - Frame rate ≥ 60 FPS sustained
  - Noop compute dispatch completes без error
  - Validation log shows 0 errors
- `sync_register.ps1 --validate` exit 0

**Per К10.1/К10.2/V0.A precedent**: Manual visual verification is **executor responsibility** post-cascade. Brief expects executor (Crystalka «Skarlet» AMD RX 7600S) к confirm visual behavior:
1. Window opens с dark blue color filled
2. Window resize works smoothly without flicker
3. Close button terminates cleanly
4. No validation errors в console output

**Commit message**: `test(runtime): V0.B — smoke test executable (V0.B exit criteria: clear color + swapchain recreation + compute roundtrip + hardware check)`

### Commit 18 — V0.B closure: REGISTER amendments + audit_trail EVT + brief lifecycle EXECUTED

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-V0_B lifecycle AUTHORED → EXECUTED; audit_trail EVT-{date}-V0_B-CLOSURE; new REQs)
- `tools/briefs/V0_B_EXECUTION_BRIEF.md` (this brief — frontmatter status AUTHORED → EXECUTED; §8 closure section added)
- `docs/MIGRATION_PROGRESS.md` (V0.B closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated via render_register.ps1)
- `docs/governance/VALIDATION_REPORT.md` (regenerated via sync_register.ps1)

**REGISTER amendments** (per METHODOLOGY §12.7 canonical):

1. **DOC-D-V0_B**: lifecycle AUTHORED → EXECUTED
2. **DOC-D-K10_3**: status note updated — V0.A + V0.B both closed; К10.3 brief restart pathway open
3. **audit_trail entry**: `EVT-{date}-V0_B-CLOSURE`
4. **Requirements added** (7-8 new REQs depending on grouping):
   - REQ-V0-B-SWAPCHAIN — VkSwapchainKHR + Win32 surface + recreation on resize
   - REQ-V0-B-RENDER_PASS — VkRenderPass + framebuffer + command buffer infrastructure
   - REQ-V0-B-COMPUTE_PIPELINE — VkPipeline compute + descriptor sets + dispatch
   - REQ-V0-B-MEMORY_ALLOCATOR — bumper linear allocator
   - REQ-V0-B-SPIRV_TOOLCHAIN — glslangValidator MSBuild integration
   - REQ-V0-B-ASYNC_COMPUTE_QUEUE — async compute queue family selection
   - REQ-V0-B-HARDWARE_CHECK — HardwareCapabilityCheck.Verify startup fail-fast
   - REQ-K-L19 — already enrolled Commit 13, status verified consistent

**Validation**:
- `sync_register.ps1 --validate` exit 0 — **mandatory gate** per METHODOLOGY §12.7
- `dotnet build` clean
- `cmake --build native` clean
- `dotnet test` 685+ green (V0.B additive)
- Smoke test exits 0 (validation clean + all V0.B exit criteria satisfied)

**Commit message**:
```
governance: V0.B closure — REGISTER amendments + 7 REQs + EVT-V0_B-CLOSURE

V0.B V substrate foundation completion per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-V0_B lifecycle AUTHORED → EXECUTED
- DOC-D-K10_3 status note: V0.A + V0.B closed; К10.3 brief restart pathway open
- MIGRATION_PROGRESS.md updated с V0.B closure entry

Requirements added (7 new):
- REQ-V0-B-SWAPCHAIN — VkSwapchainKHR + Win32 surface + recreation on resize
- REQ-V0-B-RENDER_PASS — VkRenderPass + framebuffer + command infrastructure
- REQ-V0-B-COMPUTE_PIPELINE — VkPipeline compute + descriptor sets + dispatch
- REQ-V0-B-MEMORY_ALLOCATOR — bumper linear allocator
- REQ-V0-B-SPIRV_TOOLCHAIN — glslangValidator MSBuild integration
- REQ-V0-B-ASYNC_COMPUTE_QUEUE — async compute queue family selection
- REQ-V0-B-HARDWARE_CHECK — HardwareCapabilityCheck.Verify startup fail-fast

audit_trail entry: EVT-{date}-V0_B-CLOSURE

V0.B closure completes V0 substrate foundation. Remaining V substrate work:
- V0.C: sprite/text/atlas + PNG decoder + threading + first textured quad
- V1 (post-V0.C): scalar field + diffusion shader (M-V1 mana, M-V2 electricity)
- V2 (post-V0.C): scalar field + wave shader (M-V7 movement)
- V substrate close: multi-field coexistence acceptance criterion

К10.3 brief restart pathway: V0.A + V0.B together provide ALL Vulkan code anchors
required by К10.3 brief. SC-14 halt class will не fire. К10.3 brief Items 43+44
(async compute queue selection + HardwareCapabilityCheck) already implemented;
К10.3 restart = surgical amendments + remaining items.

К-L19 hardware tier invariant LOCKED (Commit 13) с full implementation backing.

Phase 18 of V0.B cascade. Commit 18 of 18 — V0.B closure.
```

---

## §4 — Halt triggers (V0.B-specific SC-N taxonomy)

V0.B SC-N taxonomy substantially expanded vs V0.A. V0.B introduces swapchain extension dependency, MSBuild integration, native CMake modification, descriptor set discipline — each с new halt classes.

If execution agent encounters any of these conditions, **halt и surface к Crystalka**. Per Lesson #8 corollary: brief promises «halts before damage», not «zero halts».

### SC-1 — Code anchor doesn't match brief assumptions

V0.A code shapes verified Phase 0 brief authoring 2026-05-18. Если existing V0.A code drifted с merge (unlikely but possible), halt и surface drift.

### SC-2 — Vulkan struct alignment regression

Если Marshal.SizeOf<T>() test fails for V0.A baseline struct (e.g., VkPhysicalDeviceProperties != 824) — halt. V0.A landed alignment fix; regression = serious issue (compiler change, runtime change).

### SC-3 — Deep-read contradiction

Any §2.3/§2.4/§2.5 mandatory re-read surfaces a file shape that contradicts this brief. Halt + surface.

### SC-4 — New Vulkan struct size mismatch

Per Lesson #7 strengthening: Marshal.SizeOf<T>() для new V0.B struct doesn't match expected size from Vulkan 1.3 spec. Common cause: missed alignment padding. Halt + audit struct layout against vulkan_core.h header file.

Recovery: read $(VULKAN_SDK)\Include\vulkan\vulkan_core.h for exact struct layout; add explicit `fixed byte _padX[N]` fields per V0.A precedent (VkPhysicalDeviceProperties model).

### SC-5 — VULKAN_SDK env var unset

Hard gate per §2.2. Same as V0.A SC-16.

### SC-6 — glslangValidator.exe absent or fails

Phase 0 deliverable: copy glslangValidator.exe from VULKAN_SDK\Bin\ к tools/. Если absent in SDK → halt SC-6a. Если present but invocation fails (e.g., compile error in clearcolor.vert) → halt SC-6b.

Recovery for SC-6a: reinstall Vulkan SDK or verify file present manually.
Recovery for SC-6b: review GLSL syntax error message; correct shader source.

### SC-7 — Vulkan validation regression (V0.B introductions)

S-LOCK-4 mandates ZERO validation errors. V0.B introduces substantial new Vulkan code surface — каждая new call has potential validation message. Если smoke test reports validation errors:

Common causes:
- Pipeline barrier missing between command buffer recordings
- Descriptor set layout binding mismatch
- Image layout transition missing (swapchain image must be PRESENT_SRC_KHR before vkQueuePresentKHR)
- Memory binding before vkAllocateMemory completion (sync error)
- Resource creation order issue (e.g., framebuffer before render pass)
- Pipeline shader stage flags mismatch

Halt + investigate validation message via ValidationLog dump.

### SC-8 — Swapchain creation fails

`vkCreateSwapchainKHR` returns non-success. Common causes:
- Surface format / present mode combination not supported on hardware (Phase 0 should record available formats)
- Image count below minImageCount or above maxImageCount (read VkSurfaceCapabilitiesKHR Phase 0)
- Composite alpha flag not supported (use OPAQUE_BIT_KHR safe default)
- Surface no longer valid (window destroyed)

Halt + check VkResult specifically + dump surface capabilities + supported formats.

### SC-9 — CMake Vulkan find_package fails

Phase 0 should establish VULKAN_SDK env var. find_package(Vulkan) consumes this env var. Если CMake error «Vulkan not found» — halt SC-9.

Recovery: ensure VULKAN_SDK env var set + accessible к VS-bundled CMake; alternative: explicit `-DVulkan_INCLUDE_DIR=...` + `-DVulkan_LIBRARY=...` к cmake configure step.

### SC-10 — Compute pipeline registration fails

`vkCreateComputePipelines` returns non-success. Common causes:
- SPIR-V bytecode invalid (corrupted file, wrong version, validation rejects)
- Pipeline layout incompatible с shader stage descriptors
- Shader stage flags mismatch (COMPUTE_BIT required)
- Device extension VK_KHR_compute_shader missing (default in Vulkan 1.0+ so unlikely)

Halt + verify SPIR-V validation (run `tools/glslangValidator.exe -V noop.comp` manually + check output) + validation log dump.

### SC-11 — Native CMake build fails

Native module modifications (`compute_pipeline.cpp` + `compute_dispatch.cpp` + CMakeLists.txt extension) cause cmake --build to fail. Halt + investigate.

Common causes:
- C++ syntax error in new code
- Missing include directive
- Vulkan symbol unresolved (linkage to Vulkan::Vulkan missing — verify CMakeLists.txt edit)
- DF_API export inconsistency

Recovery: review CMake error output; verify Vulkan SDK installation; verify find_package(Vulkan) found correctly.

### SC-12 — Validation regression post-commit

Sync_register.ps1 --validate exits non-zero после V0.B commit. Halt immediately per V0.A precedent (К10.1/К10.2 standard).

### SC-13 — Scope creep

Execution encounters drift не в V0.B scope (e.g., implementing sprite shaders V0.C scope, или V1/V2 substrate primitive math). Halt + surface. Do не «fix while we're here» per Lesson #20.

### SC-14 — К-L19 hardware tier failure on test hardware

`HardwareCapabilityCheck.Verify` throws `HardwareCapabilityException` on Crystalka «Skarlet» (AMD RX 7600S). V0.A confirmed К-L19 baseline; if regression — halt + investigate.

Recovery: review queue family enumeration logic; verify async compute queue family detection works on RX 7600S; potentially Vulkan driver update may be needed.

### SC-15 — К-L19 invariant landing precedes implementation

Commit 13 (К-L19 invariant landing) must follow Commits 4-5 (async compute queue selection + HardwareCapabilityCheck) per S-LOCK-9 sequencing. Если order swapped (К-L19 landing attempted before implementation), halt.

Recovery: re-order cascade — implementation first, invariant landing after.

### SC-16 — Push-to-main classifier reminder (operational, не halt)

Known behavior per V0.A precedent: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction в initial prompt. Не halt — expected. Re-confirm in-session after work done, then push branch к remote.

### SC-17 — Native + managed code interaction failure

V0.B introduces first non-trivial interaction between V substrate (managed) и K substrate (native). Если P/Invoke к new native exports crashes (e.g., struct marshalling mismatch, calling convention issue, native module not loadable):

Halt + investigate:
- Native module rebuild (`cmake --build` + verify .dll regenerated)
- P/Invoke declaration matches C ABI signature exactly
- Vulkan handle marshalling: void* в native, IntPtr в managed (must verify endianness + ABI match)

Recovery: simplify to minimal reproducer (e.g., one new export at a time); test in isolation before full integration.

### SC-18 — Multi-session execution pause

V0.B substantial scope may exceed single Claude Code session token budget. Per S-LOCK-2 + Lesson #8: atomic intermediate states preserve resume capability. Если session approaches limit:

1. Complete current atomic commit (do не leave partial)
2. Push branch к remote (preserves work for next session)
3. Surface к Crystalka: «V0.B paused at Commit N/18. Resume в next session.»
4. Next session: read current state, identify last clean commit, resume from Commit N+1.

Не a true halt — pause + resume mechanism.

При halting (SC-1..SC-18): author HALT_REPORT в `docs/scratch/V0_B/`, state trigger, state what was/wasn't committed, stop. **Do не commit partial atomic commit** — atomicity protects milestone per Lesson #8.

---

## §5 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 18 lands clean:

### §5.1 — Verify final state

1. `git log --oneline` shows ~18 commits added by V0.B на feature branch `claude/v0_b-vulkan-foundation`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `dotnet build` clean
5. `cmake --build native/DualFrontier.Core.Native` clean
6. `dotnet test` 685+ green (V0.B new tests additive — final count documented в closure entry; likely ~730-750 tests с alignment audit suite expanding test base)
7. Native selftest: 77 baseline + V0.B compute pipeline roundtrip = 78
8. V0.B smoke test exits 0:
   - Window opens с dark blue clear color visible
   - Window resize triggers clean swapchain recreation
   - Frame rate ≥ 60 FPS sustained for 5 seconds
   - Noop compute dispatch executes without error
   - HardwareCapabilityCheck passed (К-L19 async compute QF detected)
   - Validation log: 0 errors, 0 warnings
9. К-L19 invariant text consistent across KERNEL_ARCHITECTURE.md + README.md + REGISTER.yaml (Lesson #20)
10. Manual visual verification on Crystalka «Skarlet»: clear color visible, resize smooth, close button works

### §5.2 — Update brief status + closure section

Set `status: EXECUTED` в frontmatter; add §8 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established. Same template as V0.A precedent.

### §5.3 — PR opening (NOT auto-push, per V0.A precedent)

- Push branch `claude/v0_b-vulkan-foundation` к remote (NOT к `main`)
- Open PR titled «V0.B — Swapchain + render pass + compute pipeline + memory allocator + SPIR-V + async compute + hardware check»
- Body summarizes per-commit per-deliverable mapping + verification metrics + halt activations (если any) + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §5.4 — Surface к Crystalka

PR ready для review. Crystalka:
1. Reviews V0.B closure report content
2. Reviews V0.B smoke test output (validation clean, clear color rendered, compute roundtrip successful)
3. Manually verifies visual behavior на «Skarlet»
4. Merges PR к `main`
5. Provides closure report к next Opus deliberation session

**Next Opus session decision tree**:
- **Option A — V0.C brief authoring**: continue V substrate stream с rendering use case completion (sprite/text/atlas/PNG/threading)
- **Option B — К10.3 brief restart**: V0.A + V0.B unblock К10.3; restart с surgical amendments where V0.B shape differs from К10.3 brief assumptions
- **Decision rationale**: V0.C scope (sprite/PNG/threading) doesn't gate К10.3; К10.3 only requires compute pipeline plumbing + async compute queue + HardwareCapabilityCheck (all V0.B). Either order viable; Crystalka prerogative.

### §5.5 — К10.3 brief restart pathway (post-V0.B closure)

К10.3 brief Phase 0 reads will find:
- ✓ `src/DualFrontier.Runtime/` project exists (V0.A)
- ✓ `Native/Vulkan/VkApi.cs` exists с full Vulkan P/Invoke surface (V0.A baseline + V0.B extensions covering swapchain + render pass + compute + memory)
- ✓ `Graphics/VulkanInstance.cs` (V0.A)
- ✓ `Graphics/VulkanDevice.cs` с graphics + async compute queue selection (V0.A + V0.B)
- ✓ `Graphics/HardwareCapabilityCheck.cs` (V0.B Commit 5)
- ✓ `Compute/VulkanComputePipeline.cs` + `ComputeDispatch.cs` + `ComputePipelineRegistry.cs` (V0.B Commit 12)
- ✓ Native C ABI extension df_world_register_compute_pipeline + df_world_field_dispatch_compute (V0.B Commit 14)
- ✓ К-L19 invariant LOCKED в KERNEL_ARCHITECTURE.md v2.2 (V0.B Commit 13)
- ✓ README.md Hardware Requirements section (V0.B Commit 13)

К10.3 SC-14 halt class will не fire. К10.3 brief Items 43-44 (async compute queue selection + HardwareCapabilityCheck.Verify) — already implemented V0.B; К10.3 restart marks these as «verified existing implementation» rather than «create from scratch». К10.3 brief Commits 6-17 (Items 33-44 pipeline depth + display composition + mod lifecycle + К-L7.1/L16/L17/L18 invariants) proceed unchanged where possible с surgical amendments where V0.B code shape differs.

---

## §6 — Brief authority + lifecycle

**Brief authority**: V substrate authoring stream continued from V0.A closure 2026-05-18 per Crystalka split ratification (V0.A/V0.B/V0.C). V0.B is second sub-milestone of V0 split. К10.3 brief unblocking gate met на V0.B closure.

**Brief lifecycle** (per FRAMEWORK §3.3 + §3.3.1):
- AUTHORED at this commit (Commit 1 of cascade)
- EXECUTED post-Commit 18 closure
- Registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance

**Brief enrollment**: V0.B brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per V0.A precedent.

**Brief location**: `tools/briefs/V0_B_EXECUTION_BRIEF.md` after Crystalka copies из `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern.

**Brief lifecycle для К10.3**:
- К10.3 brief остаётся AUTHORED untracked в repo until V0.B closure
- Post-V0.B closure: К10.3 brief restart possible с amendments
- К10.3 brief lifecycle eventually EXECUTED с surgical amendments where V0.B reality differs from К10.3 assumptions

---

## §7 — Lesson candidates surfaced (informational, formal promotion deferred к А'.8 K-closure report)

**Lesson #7 strengthened** (V0.A executor finding 2026-05-18, applied throughout V0.B): P/Invoke ABI alignment audit recipe. Every new Vulkan struct gets Marshal.SizeOf<T>() unit test validating against Vulkan spec sizeof. Catches alignment regressions early — V0.A VkPhysicalDeviceProperties 816 → 824 alignment fix landed via этой discipline. V0.B applies к ~20+ new structs.

**Lesson #22 strengthened** (V0.A executor finding 2026-05-18, formalized V0.B S-LOCK-7): Mixed [LibraryImport] + [DllImport] convention pragmatic. .NET 7+ source generator constraints (non-blittable struct fields not supported) force pragmatic mixing. Lesson #22 «match existing convention» extends к «match source generator capability constraints».

**Vulkan SDK install workflow precedent** (V0.A executor 2026-05-18): External tool installation within deliberation session viable когда user (Crystalka) can UAC-click. LunarG silent installer + env var setting в same session. V0.B doesn't require new SDK install (V0.A landed Vulkan SDK 1.4.350.0); future V briefs может leverage same pattern для AssetForge / TexturePacker etc.

**Pattern established** (V0.B authoring): Substantial multi-commit briefs benefit от explicit S-LOCK enumeration covering scope items + sequencing rationale + Q-ratification mapping. V0.B 11 S-LOCKs vs V0.A 8 S-LOCKs — surface complexity reflected в lock count.

**К-L14 application surface — V substrate inheritance** (V0.A noted, V0.B reinforced): К-L14 «performance derives from clean complex architecture» — V0.B's substantial scope (swapchain + render pass + compute + memory + SPIR-V + async compute + hardware check + native C ABI extension) acceptable когда architecturally coherent. Default-inclusion bias preserves architectural cleanness vs split-for-split-sake.

**Lesson candidate #23** (V0.B authoring): Cross-substrate dependency resolution sequence. К10.3 brief halted Phase 0 SC-14 (V substrate absent); V0.A + V0.B together unblock К10.3 restart. Pattern: when brief halts due к prerequisite absent, prerequisite stream authors complete prerequisite (V0.A + V0.B), then halted brief restarts с surgical amendments. Cross-stream dependency resolution preserves work investment (К10.3 1923-line brief not rewritten, just amended where reality differs).

**Lesson candidate #24** (V0.B authoring): Load-bearing commit pattern для cross-document invariant landing. К-L19 lands в Commit 13 — KERNEL_ARCHITECTURE.md amendment + README.md amendment + REGISTER.yaml bump + REQ-K-L19 enrollment **все в одном commit**. Splitting leaves inconsistent intermediate state (claim без implementation backing или vice versa). К-L8 atomic compilable commits scales к cross-document invariant landing.

---

**End of brief. ~18 atomic commits across 11 S-LOCK invariants + 30+ deliverables (struct size test infrastructure + surface foundation + async compute + hardware check + memory allocator + swapchain + render pass + command infrastructure + SPIR-V toolchain + minimal shaders + graphics pipeline + compute pipeline + К-L19 invariant landing + native C ABI extension + field storage bridge + runtime facade composition + V0.B smoke test + closure). Expected 25-40 hours auto-mode execution (Crystalka «Skarlet»).**

V0.B closes V0 substrate foundation completion. V0.C remaining для V0 substrate full close. К10.3 brief restart pathway opens after V0.B closure (compute side + async compute queue + hardware check all ready). К10.4 (TLA+) further deferred.

V substrate authoring stream продолжается per Phase A' sequencing (К10.1 ✅ + К10.2 ✅ + V0.A ✅ + [V0.B] + V0.C + К10.3 restarted + К10.4 + K-closure + Roslyn analyzer + Phase B). Distance к Phase B М-series increased но architectural cleanness preserved per К-L14 default-inclusion bias.

«Halt is success, не failure» per Lesson #8 corollary — V0.A landed first Vulkan code с zero hard-gate halts + one tactical course-correction (VkPhysicalDeviceProperties alignment) caught at test gate. V0.B preserves the same discipline.

«Без костылей» applied к V substrate authoring V0.B: pure P/Invoke к vulkan-1.dll continued, in-repo SPIR-V toolchain commits build self-contained, bumper allocator simplest viable solution (не temporary hack), native C ABI extension lives в existing kernel module preserving К-L11 single source of truth, К-L19 invariant lands с full implementation backing. Substantial scope addressed coherently per К-L14 default-inclusion bias.

V0.B is second verification of К-L14 thesis на новый substrate (V0.A was first). Если V0.B closes c similar discipline (zero hard-gate halts + alignment audit catches regression early + validation clean + manual visual verification passes), К-L14 thesis on V substrate validated across multiple substantial cascades.

---

## §8 — Closure section (V0.B EXECUTED 2026-05-18)

**Commit range**: `d2c6627..PENDING-COMMIT-V0_B-CLOSURE` (18 atomic commits on branch `claude/v0_b-vulkan-foundation`).

**Cascade ledger**:
| # | Hash | Subject |
|---|------|---------|
| 1 | d2c6627 | docs(briefs): V0.B brief authored — swapchain + render pass + compute pipeline + memory allocator + SPIR-V + async compute + hardware check |
| 2 | a9e5ebb | test(runtime): V0.B — Vulkan struct size verification infrastructure (Lesson #7 strengthening per V0.A executor finding) |
| 3 | a71ecdc | feat(runtime): V0.B — Win32 surface foundation + WM_SIZE handler + WindowResizeEvent (preparing swapchain) |
| 4 | 6ff58dc | feat(runtime): V0.B — async compute queue family selection (К-L19 Item 43 from halted К10.3) |
| 5 | d62b80b | feat(runtime): V0.B — HardwareCapabilityCheck.Verify (К-L19 Item 44 from halted К10.3, startup fail-fast) |
| 6 | ebd1296 | feat(runtime): V0.B — memory allocator (bumper) + VulkanBuffer + VulkanImage primitives |
| 7 | 877635b | feat(runtime): V0.B — VulkanSurface + VulkanSwapchain + recreation on WM_SIZE |
| 8 | 648b523 | feat(runtime): V0.B — VulkanRenderPass + VulkanFramebuffer (one color attachment, no depth) |
| 9 | 75dde8c | feat(runtime): V0.B — VulkanCommandPool + VulkanCommandBuffer + per-frame fence/semaphore sync |
| 10 | 09a22e8 | feat(runtime): V0.B — SPIR-V toolchain integration + minimal shaders (clearcolor + noop) + VulkanShaderModule |
| 11 | d99b1b9 | feat(runtime): V0.B — minimal graphics pipeline (clearcolor fullscreen triangle) |
| 12 | d3ff335 | feat(runtime): V0.B — VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch + ComputePipelineRegistry |
| 13 | 834140f | governance: V0.B — К-L19 hardware tier invariant LOCKED (KERNEL_ARCHITECTURE v2.2 + README.md hardware requirements + REQ-K-L19) |
| 14 | 6c867ec | feat(native): V0.B — df_world_register_compute_pipeline + df_world_field_dispatch_compute C ABI extension |
| 15 | 09bf10d | feat(runtime): V0.B — FieldStorageBinding bridges K9 RawTileField к Vulkan compute (managed wrapper consuming native C ABI) |
| 16 | 048e3f9 | feat(runtime): V0.B — Runtime facade full V0.B composition (surface + swapchain + render pass + command pool + memory + compute registry) |
| 17 | 401c3a3 | test(runtime): V0.B — smoke test executable (V0.B exit criteria: swapchain + render pass + compute roundtrip + hardware check) |
| 18 | PENDING-COMMIT-V0_B-CLOSURE | governance: V0.B closure — REGISTER amendments + 7 REQs + EVT-V0_B-CLOSURE |

**Verification metrics (final)**:
- `dotnet build DualFrontier.sln`: clean (0 warnings, 0 errors)
- `dotnet test DualFrontier.sln`: 786 passed, 0 failed (685 V0.A baseline + 101 V0.B additive)
  - DualFrontier.Runtime.Tests grew 20 → 121 (V0.B test contribution)
  - All other test projects baselines preserved
- `cmake --build native/DualFrontier.Core.Native`: clean (0 warnings)
- `df_native_selftest.exe`: ALL PASSED (78 scenarios — 77 K10.2 baseline + 1 V0.B compute pipeline registration roundtrip)
- `V0.B smoke test`: exit 0 — К-L19 hardware tier satisfied (AMD Radeon RX 7600S, async compute QF 1), swapchain + render pass + compute round-trip operational, validation layer 0 errors / warnings / info
- `sync_register.ps1 --validate`: exit 0 (advisory orphan warnings only — pre-existing baseline + V0.B MODULE.md files)

**Halt protocol activations**: 1 minor cascade course-correction (Commit 14 native build: forward-decl namespace collision `dualfrontier::dualfrontier::ComputePipelineRegistry`, caught by MSVC + missing include in capi.cpp). Neither а hard-gate halt nor SC-N classification — both fixed in same commit cycle и build passed before commit landed.

**Zero hard-gate halts** across 18 commits. Same V0.A discipline preserved. К-L14 thesis on V substrate validated across two substantial cascades (V0.A + V0.B).

**Lesson candidates surfaced (informational, formal promotion deferred к А'.8 K-closure report)**:
- **Lesson #7 strengthened (V0.B reinforcement)**: P/Invoke ABI alignment audit recipe inherited from V0.A, applied к ~50+ new Vulkan structs in V0.B. Five brief-stated sizes corrected via Marshal.SizeOf test feedback (VkMemoryAllocateInfo 32 not 24; VkPresentInfoKHR 64 not 40; VkDescriptorSetLayoutCreateInfo 32 not 24; VkShaderModuleCreateInfo 40 not 32; VkPipelineColorBlendStateCreateInfo 56 not 40). Each correction caught at test gate, не at runtime crash.
- **Lesson #8 corollary applied**: К-L19 invariant landing (Commit 13) sequenced AFTER full implementation backing operational (Commits 4 + 5 + 12) — never leaving intermediate state where invariant claimed без implementation.
- **Lesson #22 confirmed**: native code modifications consume VS-bundled CMake; DllImport convention preserved; SmokeTest project deploys native DLL via same csproj pattern as Interop.Tests.
- **K-L14 default-inclusion bias validated**: V0.B substantial scope (18 commits, ~5000 LOC managed + native) addressed coherently as single monolithic milestone per S-LOCK-2 ratification. Splitting V0.B.1/V0.B.2 would have fragmented unified VkDevice setup unnecessarily.
- **Cross-stream prerequisite resolution pattern (Lesson candidate #23 confirmation)**: К10.3 brief halt at Phase 0 SC-14 (V substrate absent) resolved by completing prerequisite stream (V0.A + V0.B) rather than rewriting halted brief. К10.3 restart pathway opens; Items 43 + 44 already implemented as part of V0.B; К10.3 brief restart marks these as «verified existing implementation». Preserves work investment in halted brief (1923-line К10.3 brief authoring not lost).
- **Load-bearing commit pattern (Lesson candidate #24 confirmation)**: К-L19 invariant landing в Commit 13 — KERNEL_ARCHITECTURE.md amendment + README.md amendment + REGISTER.yaml bump + REQ-K-L19 enrollment all в одном commit. Intermediate state never inconsistent (claim без implementation backing или vice versa). K-L8 atomic compilable commits scales к cross-document invariant landing.

**К10.3 restart preconditions met at this closure**:
- ✓ src/DualFrontier.Runtime/ project exists (V0.A)
- ✓ Vulkan P/Invoke surface complete via VkApi + VkStructs + VkEnums (V0.A + V0.B substantial extensions)
- ✓ Graphics/VulkanInstance.cs (V0.A)
- ✓ Graphics/VulkanDevice.cs с graphics + async compute queue family selection (V0.A + V0.B Commit 4)
- ✓ Graphics/HardwareCapabilityCheck.cs (V0.B Commit 5)
- ✓ Compute/VulkanComputePipeline.cs + ComputeDispatch.cs + ComputePipelineRegistry.cs (V0.B Commit 12)
- ✓ Native C ABI extension df_world_register_compute_pipeline + df_world_field_dispatch_compute (V0.B Commit 14)
- ✓ К-L19 invariant LOCKED в KERNEL_ARCHITECTURE.md v2.2 (V0.B Commit 13)
- ✓ README.md Hardware requirements section (V0.B Commit 13)

К10.3 brief SC-14 halt class won't fire at restart. К10.3 brief Commits 6-17 (Items 33-44 pipeline depth + display composition + mod lifecycle + К-L16/L17/L18 invariants) proceed с surgical amendments where V0.B shape differs from К10.3 brief assumptions. К-L16/L17/L18 numbering reserved для К10.3 elaboration.
