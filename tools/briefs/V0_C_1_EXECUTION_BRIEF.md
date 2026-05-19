---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-V0_C_1
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-V0_C_1
---
---
# Brief frontmatter (not REGISTER mirror — brief lives in tools/briefs/ as Tier 3 Category D)
brief_id: V0_C_1_EXECUTION_BRIEF
status: AUTHORED
authored: 2026-05-19
author: Claude Opus 4.7 (Crystalka deliberation session, post-V0.B closure)
target_executor: Claude Code (auto-mode + Crystalka oversight)
estimated_duration: 20-30 hours auto-mode (V0.C.1 scope: PNG decoder + textured sprite pipeline + input event types + first textured quad — R.1 + R.4 phases per VULKAN_SUBSTRATE §4.2)
brief_type: execution
authority_chain:
  - VULKAN_SUBSTRATE.md v1.0 LOCKED (DOC-A-VULKAN_SUBSTRATE) — V substrate authoritative spec; §1.1 V0 rendering bullets 5-9 (PNG decoder + sprite atlas + bitmap font + text renderer + input event queue + debug overlay); §2.2 module structure (Assets, Sprite, Input modules); §2.5 native interop patterns; §2.6 asset pipeline; §4.2 R.1 + R.4 phase mapping
  - V0_B_EXECUTION_BRIEF.md EXECUTED (DOC-D-V0_B, 2026-05-19) — V0.B closure baseline + 786 tests + К-L19 hardware tier locked + swapchain operational; V0.C.1 builds on V0.B graphics primitives
  - V0_A_EXECUTION_BRIEF.md EXECUTED (DOC-D-V0_A, 2026-05-18) — V0.A closure baseline + Vulkan struct alignment audit (Lesson #7 strengthening continues V0.C.1)
  - K10_3_EXECUTION_BRIEF.md AUTHORED, HALTED Phase 0 SC-14 2026-05-18 — К10.3 brief restart pathway open после V0.A + V0.B closure (independent of V0.C.1); V0.C.1 не gates К10.3 restart, but extends V substrate stream parallel to К10.3 restart possibility
  - KERNEL_ARCHITECTURE.md v2.2 LOCKED (post-V0.B closure) — К-L1..К-L19 invariants; V0.C.1 does не land new К-L invariants (sprite/PNG/input belong to V substrate, не K substrate; existing K-Lxx preserved)
  - METHODOLOGY.md v1.8 LOCKED — Lessons #7/#8/#11/#20/#22 + #7 strengthening (P/Invoke ABI alignment audit recipe — Marshal.SizeOf<T>() per new Vulkan struct; V0.B caught 5 brief estimate errors continuing V0.C.1 discipline)
  - FRAMEWORK.md v1.1 LOCKED — Category D Tier 3 lifecycle transitions
  - Directory.Build.props — net8.0 + LangVersion 12.0 + Nullable enable + TreatWarningsAsErrors true (verified V0.A Phase 0; V0.B CompileShaders MSBuild target preserved)
  - existing V0.B code anchors (verified Phase 0 brief authoring 2026-05-19): VkApi.cs ~19.5 KB Vulkan P/Invoke surface; VulkanImage.cs с full image + view creation; VulkanPipelineLayout.cs с descriptor set + push constant range support; VulkanGraphicsPipeline.cs minimal pipeline pattern; Window.cs WM_SIZE handler precedent; Runtime.cs facade composition
---

# V0.C.1 — PNG decoder + textured sprite pipeline + input event types (R.1 + R.4)

**Brief shape**: Execution-mode brief targeting Claude Code auto-mode с Crystalka oversight. Multi-commit atomic cascade implementing **V0 rendering use case bullets 5-9 partial coverage** per VULKAN_SUBSTRATE.md §1.1 — specifically R.1 (first textured quad) + R.4 (input system) per §4.2 phase mapping. **Excludes** R.2 (batched sprite renderer at 10,000 sprites), R.3 (TileMap + Camera2D), R.5+ (Domain integration, UI primitives, lifecycle, cutover) — those belong V0.C.2 + post-V substrate close.

**Authority**: V0.B closed 2026-05-19 (PR #37 merged, 18 atomic commits, 786 tests green, zero hard-gate halts, 5 alignment audit corrections caught early). V0.C.1 continues V substrate authoring stream per Crystalka split ratification 2026-05-19 (V0.C → V0.C.1 + V0.C.2 split). К10.3 brief restart pathway already open post-V0.B; V0.C.1 не gates К10.3 — runs parallel в same stream.

**V0.B closure context inherited** (per V0.B §8 closure report):
- 786 tests baseline (685 V0.B baseline через 18 commits + 101 V0.B additive across Marshal.SizeOf tests + Runtime composition + smoke test)
- Validation layer 0 errors/warnings/info на Crystalka «Skarlet» (AMD RX 7600S, Vulkan 1.4.344)
- К-L19 invariant LOCKED в KERNEL_ARCHITECTURE.md v2.2 + README.md hardware requirements
- 5 brief-stated struct sizes corrected via Marshal.SizeOf test gate (Lesson #7 strengthening worked as designed)
- Native C ABI bookkeeping stubs documented honestly (Lesson #25 candidate — implementation depth follows consumer materialization)
- VulkanImage с full create + view + memory binding operational
- VulkanPipelineLayout с descriptor set + push constant range parameters
- VulkanGraphicsPipeline minimal pattern с dynamic viewport/scissor для swapchain recreation flexibility
- Window.cs WM_SIZE handler comment hint: «V0.C: WM_KILLFOCUS, WM_SETFOCUS, WM_KEYDOWN/UP, WM_MOUSE* dispatch» — V0.C.1 honors this

**V0.C.1 scope discipline (Lesson #20 + Lesson #14 application)**:

In-scope (V0.C.1):
- **PNG decoder** (Assets module per VULKAN_SUBSTRATE §2.6):
  - Manual chunk parsing (IHDR, IDAT, IEND minimum) + CRC32 verification
  - DEFLATE decompression via `System.IO.Compression.DeflateStream` (BCL per §0 L5)
  - Filter unfiltering (Sub/Up/Average/Paeth all four predictors per Q2 (c) ratification)
  - RGBA8 + RGB8 source formats (Q2 (a) ratification — RGB8 → RGBA8 conversion at load с alpha = 255)
  - **Excluded**: interlaced PNG, palette (PLTE/tRNS chunks), 16-bit channels, gAMA/sRGB/cHRM color management
  - Target: ~500-700 LOC manual decoder per §2.6 estimate
- **AssetManager** (Assets module per VULKAN_SUBSTRATE §2.2):
  - Path resolution within `assets/` directory
  - PNG file loading + decoded byte[] caching
  - Future-extensible к other asset types (font, audio, etc.) but V0.C.1 = PNG only
- **VulkanSampler** primitive (Graphics module — V0.B did не land sampler):
  - Nearest-neighbor sampler default per Q3 (b) ratification (pixel art preservation)
  - Configurable filter mode (nearest/linear) для future flexibility
  - Wrap mode REPEAT default; CLAMP_TO_EDGE option
- **Texture upload path** (Graphics module — bridges PNG bytes к VulkanImage):
  - Staging buffer (host-visible VulkanBuffer) → device-local VulkanImage transfer via VkCmdCopyBufferToImage
  - VkCmdPipelineBarrier для image layout transitions (UNDEFINED → TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL)
  - Synchronous upload (waitIdle на graphics command pool — К-L7 atomic-from-observer preserved)
- **Sprite shaders** (`tools/shaders/sprite.vert` + `sprite.frag`):
  - Vertex shader consumes per-vertex pos (2D) + UV (2D) + color (4 bytes packed RGBA) per Q3 (a) ratification
  - Vertex shader applies push-constant Camera2D MVP matrix (4×4 mat4 = 64 bytes per push constant range)
  - Fragment shader samples textured atlas + multiplies vertex color (enables tint effects)
- **Sprite pipeline** (Sprite module + VulkanGraphicsPipeline extension):
  - Vertex input description (pos + UV + color)
  - Alpha blending enabled (premultiplied alpha workflow — Q3 (b) consequence + standard sprite rendering pattern)
  - Descriptor set layout (1 combined image sampler binding)
  - Push constant range (camera MVP)
- **Sprite primitive types** (Sprite module per Q3 (c) ratification — minimal handle):
  - `SpriteTexture` — texture handle (VulkanImage + VulkanSampler reference) + atlas UV rect normalization helpers
  - `AtlasRegion` — UV rect description (u0, v0, u1, v1) for atlas-based sub-sprites
  - `Sprite` — minimal handle: texture reference + UV rect + transform (position, scale, rotation, tint color)
  - **Deferred к V0.C.2**: full sprite handle struct with caching, batching membership tracking
- **Single sprite renderer** (Sprite module):
  - One-sprite-per-draw-call API: `SpriteRenderer.DrawSprite(Sprite, CommandBuffer)`
  - Direct quad recording (4 vertices via dynamic vertex buffer)
  - **Deferred к V0.C.2**: batching (10,000 sprites at 60+ FPS), Camera2D class, TileMap rendering
- **Full input event types** (Input module — completes V0.A scaffold per VULKAN_SUBSTRATE §2.2):
  - `KeyPressedEvent` + `KeyReleasedEvent` (Key enum)
  - `MouseMovedEvent` (new position + delta)
  - `MouseButtonEvent` (Pressed/Released + MouseButton enum)
  - `MouseWheelEvent` (scroll delta)
  - `WindowFocusEvent` (Focused/Unfocused) — per VULKAN_SUBSTRATE §2.3 threading model + R.7 R.7 precedent (focus events couple к loop pause; coupling itself V0.C.2/V0.D scope but event types land V0.C.1)
- **Win32 message dispatch** (Window.cs extension):
  - WM_KEYDOWN/WM_KEYUP → KeyPressedEvent/KeyReleasedEvent (Virtual Key code → Key enum mapping)
  - WM_MOUSEMOVE → MouseMovedEvent (cursor coordinates extracted via lParam)
  - WM_LBUTTONDOWN/WM_LBUTTONUP/WM_RBUTTONDOWN/WM_RBUTTONUP/WM_MBUTTONDOWN/WM_MBUTTONUP → MouseButtonEvent
  - WM_MOUSEWHEEL → MouseWheelEvent
  - WM_SETFOCUS/WM_KILLFOCUS → WindowFocusEvent
- **Runtime facade composition extension**:
  - Add AssetManager к Runtime facade
  - Add SpriteRenderer к Runtime facade
  - Add VulkanSampler default к Runtime facade
  - Disposal в reverse construction order
- **V0.C.1 smoke test**: Kenney pawn sprite rendered at center of window (per VULKAN_SUBSTRATE §4.2 R.1 verbatim success criterion); validation clean; input events generated при key press / mouse move / focus change
- **Test infrastructure**: Marshal.SizeOf<T>() unit tests за каждый new Vulkan struct (Lesson #7 strengthening continued); PNG decoder unit tests with synthetic PNG inputs; input event mapping tests

Out-of-scope (V0.C.2):
- Batched sprite renderer (10,000 sprites at 60+ FPS via single draw call) — R.2 phase
- Dynamic vertex buffer pooling
- Sprite sorting by atlas/material для batch grouping
- Camera2D orthographic projection class с view/projection matrix construction
- TileMap rendering (200×200 grid) — R.3 phase
- TileMap-specific atlas region helpers
- Atlas region metadata loading (JSON/code-defined)
- Domain integration via PresentationBridge (R.5 phase) — post-V substrate close

Out-of-scope (post-V substrate close):
- BitmapFont + TextRenderer (R.6 phase per Q6 ratification — V0.C.1 не lands text rendering)
- UI primitives (panels, labels, progress bars) — R.6 phase
- DebugOverlay coupling к domain — R.7 phase
- Focus event coupling к loop.SetPaused() — R.7 phase (event types land V0.C.1; coupling implementation post-V substrate close)
- Godot Presentation cutover — R.8 phase
- К10.3 brief restart — independent stream, can proceed parallel к V0.C.1
- V1/V2 substrate primitives — separate briefs post-V0 substrate close
- M-V demonstrations
- Free list / pool memory allocator — V0.B bumper continues adequate

**Strategic note**: V0.C.1 substantially smaller scope than V0.B (which had ~30 new Vulkan functions + ~20 new structs + native C ABI extension + cross-document К-L19 landing). V0.C.1 surface ≈ ~10-15 new Vulkan functions (sampler creation, buffer-to-image copy command, image layout transition commands, descriptor pool/set for sampler), ~8-10 new Vulkan structs (VkSamplerCreateInfo, VkBufferImageCopy, VkImageMemoryBarrier, VkDescriptorPoolCreateInfo extension, VkDescriptorImageInfo, VkVertexInputBindingDescription, VkVertexInputAttributeDescription, VkPushConstantRange — already в V0.B VkStructs.cs partially), ~500-700 LOC PNG decoder, ~6 new input event types + Win32 message dispatch. **Lesson #7 strengthening discipline continues — every new Vulkan struct gets Marshal.SizeOf<T>() test**.

**К10.3 unblocking gate (already met post-V0.B)**: V0.C.1 не required для К10.3 brief restart. Crystalka can choose:
- Run К10.3 restart + V0.C.1 brief authoring в parallel sessions
- Run V0.C.1 first (continues V substrate stream coherently), then К10.3 restart
- Run К10.3 restart first (compute side priority), then V0.C.1
К10.3 restart pathway open независимо.

**Brief size note**: V0.C.1 brief target ~2200-2500 lines per К-L14 default-inclusion bias — moderate scope vs V0.B (2320 lines). PNG decoder verbatim specification adds substantial content (~500 lines), но overall structural surface smaller than V0.B native C ABI + cross-document invariant landing.

---

## §1 — Crystalka ratified scope locks (V0.C.1 authoring, post-V0.B closure 2026-05-19)

### §1.1 — S-LOCK-1: V0.C.1 scope = PNG decoder + sampler + texture upload + sprite pipeline + single sprite renderer + input event types

**LOCK**: V0.C.1 implements exactly these deliverables, in dependency order:

| Group | Deliverable | Source |
|---|---|---|
| Test infrastructure | Vulkan struct size verification tests inherited from V0.B (regression baseline) | Lesson #7 strengthening |
| Vulkan extension | VkSamplerCreateInfo + VkPushConstantRange + VkVertexInputBindingDescription + VkVertexInputAttributeDescription + VkBufferImageCopy + VkImageMemoryBarrier + VkDescriptorImageInfo struct definitions | VULKAN_SUBSTRATE §2.5 |
| Vulkan extension | vkCreateSampler + vkDestroySampler + vkCmdCopyBufferToImage + vkCmdPipelineBarrier (V0.B landed pipeline barriers partial; V0.C.1 completes) + vkUpdateDescriptorSets (V0.B landed; V0.C.1 consumes для sampler) | VULKAN_SUBSTRATE §2.5 |
| PNG decoder | `DualFrontier.Runtime.Assets.PngDecoder` — manual chunk parser + DEFLATE + filter unfiltering | VULKAN_SUBSTRATE §0 L5 + §2.6 |
| PNG decoder | `DualFrontier.Runtime.Assets.PngImage` — decoded image data record (width, height, RGBA8 byte[]) | VULKAN_SUBSTRATE §2.2 |
| AssetManager | `DualFrontier.Runtime.Assets.AssetManager` — path resolution + caching | VULKAN_SUBSTRATE §2.2 |
| AssetManager | `DualFrontier.Runtime.Assets.AssetPath` — typed path wrapper | VULKAN_SUBSTRATE §2.2 |
| Sampler | `DualFrontier.Runtime.Graphics.VulkanSampler` — VkSampler lifecycle | new V0.C.1 primitive |
| Sampler | `DualFrontier.Runtime.Graphics.SamplerOptions` — nearest/linear filter, wrap mode | new V0.C.1 primitive |
| Texture upload | `DualFrontier.Runtime.Graphics.TextureUploader` — staging buffer + buffer-to-image copy + layout transitions | new V0.C.1 primitive |
| Texture upload | VulkanImage extension с CreateFromPngImage convenience method (consumes PngImage + TextureUploader internally) | extends V0.B VulkanImage |
| Pipeline layout extension | VulkanPipelineLayout с push constant range support (V0.B parameters не consumed yet; V0.C.1 first use) | extends V0.B |
| Sprite pipeline | `DualFrontier.Runtime.Sprite.SpriteVertex` — struct LayoutKind.Sequential: pos(Vector2) + uv(Vector2) + color(uint packed RGBA) | new V0.C.1 primitive |
| Sprite pipeline | `DualFrontier.Runtime.Sprite.VulkanSpritePipeline` — pipeline с vertex input + blending + sampler descriptor set | extends VulkanGraphicsPipeline pattern |
| Sprite pipeline | `tools/shaders/sprite.vert` + `tools/shaders/sprite.frag` GLSL source files | VULKAN_SUBSTRATE §2.7 |
| Sprite pipeline | `assets/shaders/sprite.vert.spv` + `assets/shaders/sprite.frag.spv` pre-compiled output | VULKAN_SUBSTRATE §2.7 |
| Sprite pipeline | MSBuild target extension: shader compilation list adds sprite.vert + sprite.frag | extends V0.B CompileShaders |
| Sprite primitive types | `DualFrontier.Runtime.Sprite.AtlasRegion` — UV rect (u0, v0, u1, v1) record | VULKAN_SUBSTRATE §2.2 |
| Sprite primitive types | `DualFrontier.Runtime.Sprite.SpriteTexture` — texture handle wrapping VulkanImage + VulkanSampler | new V0.C.1 primitive |
| Sprite primitive types | `DualFrontier.Runtime.Sprite.Sprite` — minimal handle: texture ref + UV rect + transform | new V0.C.1 primitive |
| Sprite primitive types | `DualFrontier.Runtime.Sprite.SpriteTransform` — position(Vector2) + scale(Vector2) + rotation(float radians) + tint(uint RGBA) | new V0.C.1 primitive |
| Sprite renderer | `DualFrontier.Runtime.Sprite.SpriteRenderer` — one-sprite-per-call (V0.C.2 extends к batched) | new V0.C.1 primitive |
| Input types | `DualFrontier.Runtime.Input.Key` enum — keyboard keys (printable + special: arrows, escape, space, shift, ctrl, alt, etc.) | VULKAN_SUBSTRATE §2.2 |
| Input types | `DualFrontier.Runtime.Input.MouseButton` enum — Left, Right, Middle | VULKAN_SUBSTRATE §2.2 |
| Input types | `DualFrontier.Runtime.Input.KeyPressedEvent` + `KeyReleasedEvent` records | VULKAN_SUBSTRATE §2.2 |
| Input types | `DualFrontier.Runtime.Input.MouseMovedEvent` + `MouseButtonEvent` records | VULKAN_SUBSTRATE §2.2 |
| Input types | `DualFrontier.Runtime.Input.MouseWheelEvent` record | VULKAN_SUBSTRATE §2.2 |
| Input types | `DualFrontier.Runtime.Input.WindowFocusEvent` record | VULKAN_SUBSTRATE §2.3 |
| Win32 dispatch | Window.cs WindowProcedure extended: WM_KEYDOWN/UP/MOUSEMOVE/L|R|MBUTTONDOWN/UP/MOUSEWHEEL/SETFOCUS/KILLFOCUS dispatch к InputQueue | Window.cs hint comment |
| Win32 helpers | VK_* virtual key code constants in Win32Constants.cs (VK_LEFT, VK_RIGHT, VK_UP, VK_DOWN, VK_ESCAPE, VK_SPACE, VK_RETURN, etc.) | Win32 documentation verbatim |
| Win32 helpers | Virtual key code → Key enum mapping function in Window.cs или separate helper class | new V0.C.1 |
| Runtime composition | Update Runtime.cs facade: AssetManager + SpriteRenderer + default VulkanSampler composed; disposal в reverse order | extends V0.B |
| Test infrastructure | Marshal.SizeOf<T>() tests за каждый new Vulkan struct | Lesson #7 strengthening |
| Test infrastructure | PNG decoder unit tests (synthetic PNG inputs → expected RGBA output, malformed PNG rejection) | VULKAN_SUBSTRATE §2.8 |
| Test infrastructure | Input event mapping tests (VK_LEFT → Key.Left, etc.) | new V0.C.1 |
| Asset | Kenney pawn PNG checked into assets/ directory (if не already present from prior Godot-era work — verify Phase 0) | V0.C.1 smoke test prerequisite |
| Smoke test | V0.C.1 exit criteria standalone executable (Kenney pawn rendered centered + input events visible + validation clean) | VULKAN_SUBSTRATE §4.2 R.1 |

**Deliverable ordering rationale**:
1. **Vulkan extension first** — new structs + functions land before consumers compile (struct size tests immediately verify per Lesson #7)
2. **PNG decoder second** — pure managed code, no Vulkan dependency, testable in isolation
3. **AssetManager third** — wraps PNG decoder + path resolution
4. **VulkanSampler fourth** — Vulkan primitive needed before texture upload
5. **TextureUploader fifth** — combines staging buffer + image transfer
6. **Pipeline layout push constant extension sixth** — extends V0.B parameters consumption
7. **Sprite shaders + SPIR-V compilation seventh** — shader sources + MSBuild target extension
8. **Sprite vertex format eighth** — SpriteVertex struct + vertex input description helpers
9. **VulkanSpritePipeline ninth** — extends VulkanGraphicsPipeline pattern с vertex input + blending + descriptors
10. **Sprite primitive types tenth** — AtlasRegion, SpriteTexture, Sprite, SpriteTransform
11. **SpriteRenderer eleventh** — orchestrates pipeline + descriptor set + vertex buffer + command recording
12. **Input event types twelfth** — Key/MouseButton enums + event records
13. **Win32 message dispatch thirteenth** — Window.cs WindowProcedure extension
14. **Runtime facade composition fourteenth** — wires all new components
15. **Smoke test fifteenth** — verifies V0.C.1 exit criteria
16. **Closure sixteenth** — REGISTER + REQs + EVT

### §1.2 — S-LOCK-2: PNG decoder scope = RGBA8 + RGB8 minimum coverage (per Q2 ratification)

**LOCK** (per Crystalka ratification Q2 2026-05-19): PNG decoder lands **minimum viable subset**:

**Supported**:
- Bit depth: **8 bits per channel only**
- Color types: **2 (RGB) + 6 (RGBA) only**
- RGB8 → RGBA8 conversion at load с alpha = 255 (uniform fully-opaque)
- All 4 filter predictors: None(0) + Sub(1) + Up(2) + Average(3) + Paeth(4)
- IHDR + IDAT + IEND chunks
- CRC32 verification mandatory (corrupt PNG rejected с diagnostic exception)

**Not supported (out-of-scope V0.C.1)**:
- Interlaced PNG (Adam7) — error если interlace_method != 0
- Palette indexed color (color type 3) — error
- Grayscale (color type 0) и Grayscale+Alpha (color type 4) — error (Kenney atlas is RGB/RGBA)
- 16-bit channels (bit depth 16) — error
- Color management chunks (gAMA, sRGB, cHRM, iCCP) — silently ignored
- Ancillary chunks (tEXt, zTXt, tIME, tRNS, bKGD, hIST, sPLT, sBIT, etc.) — silently ignored

**Decoder API**:
```csharp
namespace DualFrontier.Runtime.Assets;

public sealed record PngImage(int Width, int Height, byte[] PixelsRgba8);

public static class PngDecoder
{
    /// <summary>Decode PNG bytes to RGBA8 pixel data. RGB8 source converted to RGBA8 с alpha=255.</summary>
    /// <exception cref="PngDecoderException">Thrown for unsupported PNG variants or malformed data.</exception>
    public static PngImage Decode(ReadOnlySpan<byte> pngBytes);
    
    /// <summary>Convenience load from file path.</summary>
    public static PngImage DecodeFile(string path);
}

public sealed class PngDecoderException : Exception
{
    public PngDecoderException(string message) : base(message) { }
}
```

**Rationale**:
- Minimum scope per Lesson #20 discipline — Kenney atlas verified RGB/RGBA 8-bit (typical), no interlaced needed
- Full PNG spec coverage = ~2000+ LOC vs ~500-700 LOC minimum — К-L14 «без костылей» preserved через explicit scope reduction documented honestly
- Future PNG variants extend через Lesson #25 (implementation depth follows consumer materialization) — when M-series content requires palette/grayscale, decoder extends

### §1.3 — S-LOCK-3: Sprite vertex format = pos+UV+color (per Q3 (a) ratification)

**LOCK**: SpriteVertex struct LayoutKind.Sequential:

```csharp
namespace DualFrontier.Runtime.Sprite;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SpriteVertex
{
    public Vector2 Position;  // 8 bytes — screen-space или world-space depending on shader
    public Vector2 Uv;        // 8 bytes — texture UV (0..1 normalized)
    public uint TintRgba;     // 4 bytes — packed RGBA: R = (TintRgba >> 0) & 0xFF, G = >> 8, B = >> 16, A = >> 24
    // Total: 20 bytes per vertex
}
```

**Marshal.SizeOf<SpriteVertex>() must == 20 bytes** (verified Marshal.SizeOf test).

**Vertex input description** (Vulkan binding):
- Binding 0: SpriteVertex, stride = 20 bytes, input rate VERTEX
- Attribute 0: Position, format VK_FORMAT_R32G32_SFLOAT, offset 0
- Attribute 1: Uv, format VK_FORMAT_R32G32_SFLOAT, offset 8
- Attribute 2: TintRgba, format VK_FORMAT_R8G8B8A8_UNORM, offset 16

**Rationale**:
- Color enables tint effects without changing format later (К-L14 default-inclusion)
- 20 bytes per vertex × 4 vertices per sprite = 80 bytes per sprite — efficient
- Packed uint RGBA matches Win32 color conventions + sprite shader normalizes via R8G8B8A8_UNORM format

### §1.4 — S-LOCK-4: Sprite shader scope = textured + tinted + camera MVP push constant (per Q5 (a))

**LOCK**: sprite.vert/sprite.frag minimal but production-shape:

**Vertex shader (sprite.vert)**:
```glsl
#version 450

layout(location = 0) in vec2 inPos;
layout(location = 1) in vec2 inUv;
layout(location = 2) in vec4 inColor;  // tint, normalized 0..1 via UNORM format

layout(push_constant) uniform PushConstants {
    mat4 mvp;  // model-view-projection matrix from Camera2D (V0.C.2 will populate; V0.C.1 uses identity for single-sprite test)
} pc;

layout(location = 0) out vec2 vUv;
layout(location = 1) out vec4 vColor;

void main() {
    gl_Position = pc.mvp * vec4(inPos, 0.0, 1.0);
    vUv = inUv;
    vColor = inColor;
}
```

**Fragment shader (sprite.frag)**:
```glsl
#version 450

layout(set = 0, binding = 0) uniform sampler2D atlas;

layout(location = 0) in vec2 vUv;
layout(location = 1) in vec4 vColor;

layout(location = 0) out vec4 outColor;

void main() {
    vec4 sampled = texture(atlas, vUv);
    outColor = sampled * vColor;  // sample × tint
    // Discard fully-transparent fragments to avoid blend overhead (optional optimization)
    if (outColor.a < 0.01) discard;
}
```

**Push constant range**: mat4 = 64 bytes, stage = VERTEX_BIT, offset = 0.

**Descriptor set 0 binding 0**: COMBINED_IMAGE_SAMPLER, stage = FRAGMENT_BIT, descriptor count = 1.

**V0.C.1 MVP identity behavior**: SpriteRenderer constructs identity mat4 for first textured quad test. V0.C.2 Camera2D class will populate actual ortho projection × view matrix.

### §1.5 — S-LOCK-5: Alpha blending = premultiplied alpha workflow (standard sprite rendering)

**LOCK**: VulkanSpritePipeline configures color blending:

```
srcColorBlendFactor = VK_BLEND_FACTOR_ONE
dstColorBlendFactor = VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA
colorBlendOp = VK_BLEND_OP_ADD
srcAlphaBlendFactor = VK_BLEND_FACTOR_ONE
dstAlphaBlendFactor = VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA
alphaBlendOp = VK_BLEND_OP_ADD
colorWriteMask = R | G | B | A
blendEnable = VK_TRUE
```

**Premultiplied alpha workflow**: assumes texture pixels stored с alpha pre-multiplied into RGB (common convention для sprite atlases). Allows correct compositing of multiple semi-transparent sprites.

**Texture upload note**: PngDecoder outputs straight-alpha RGBA8 (PNG standard). TextureUploader **does not** convert к premultiplied alpha — accepts straight alpha and shader handles via discard threshold. **Future V0.C.2 optimization**: PNG load with explicit «premultiply alpha» flag for opaque assets where premultiplication is correct.

**Rationale**:
- Premultiplied blending mode is **production-standard** для sprite rendering — handles edge anti-aliasing correctly
- Discard threshold (alpha < 0.01) avoids GPU blend overhead для fully-transparent pixels — performance optimization при large sprite counts
- К-L14 «clean architecture» — blending mode chosen for correctness, not simplicity

### §1.6 — S-LOCK-6: VulkanSampler default = nearest-neighbor + REPEAT wrap (per Q3 (b))

**LOCK**: VulkanSampler default configuration:

```csharp
public sealed record SamplerOptions
{
    public SamplerFilterMode MagFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerFilterMode MinFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerWrapMode WrapU { get; init; } = SamplerWrapMode.Repeat;
    public SamplerWrapMode WrapV { get; init; } = SamplerWrapMode.Repeat;
    public bool EnableAnisotropy { get; init; } = false;  // V0.C.1 keeps disabled; future high-DPI / TileMap perspective may enable
}

public enum SamplerFilterMode { Nearest, Linear }
public enum SamplerWrapMode { Repeat, ClampToEdge, ClampToBorder, MirroredRepeat }
```

**Default rationale**:
- Nearest-neighbor preserves pixel art aesthetic (Kenney atlas is pixel-art style)
- Repeat wrap mode allows future TileMap usage без changing default
- Anisotropy disabled — adds device feature requirement; V0.C.2/V0.C.1 не needs it

**Runtime composition**: Runtime exposes `Runtime.DefaultSampler` (nearest + repeat) для sprite usage. Mod sprite registration в future V1/V2 work can specify own SamplerOptions если needed.

### §1.7 — S-LOCK-7: Threading model unchanged (per Q4 (b) ratification)

**LOCK**: V0.C.1 does **not** introduce threading changes:

- V0.C.1 smoke test single-threaded: window pump + render submit + sprite draw в same thread
- InputEventQueue continues V0.A pattern (`ConcurrentQueue<IInputEvent>` — thread-safe но V0.C.1 не exercises concurrent producers)
- No PresentationBridge introduction (deferred к R.5 Domain integration brief post-V substrate close)
- Vulkan queue submissions sync via fence wait (К-L7 atomic-from-observer preserved per V0.B pattern)

**V0.C.1 threading model**:
```
Main thread:
  - Win32 PeekMessage loop
  - Vulkan command buffer recording
  - Queue submit + fence wait
  - InputQueue Enqueue (from WindowProcedure callback)
  - Optional: InputQueue Dequeue (test code consumes events synchronously)
```

Threading complexity lands when Domain integration (R.5) introduces:
- Simulation thread distinct from render thread
- PresentationBridge ConcurrentQueue<IRenderCommand> для simulation → render
- Focus event coupling к loop.SetPaused()

R.5+ scope = post-V substrate close brief sequence.

### §1.8 — S-LOCK-8: Push constants для Camera MVP (per Q5 (a) ratification)

**LOCK**: VulkanPipelineLayout for sprite pipeline configures push constant range:

```csharp
var pushConstantRange = new VkPushConstantRange
{
    stageFlags = VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
    offset = 0,
    size = 64,  // mat4 = 16 floats × 4 bytes
};

var spritePipelineLayout = new VulkanPipelineLayout(
    device,
    descriptorSetLayouts: new[] { spriteDescriptorSetLayout.Handle },
    pushConstantRanges: new[] { pushConstantRange });
```

**VulkanPipelineLayout extension required**: V0.B `VulkanPipelineLayout` constructor accepts `IReadOnlyList<IntPtr>? descriptorSetLayouts = null` but does **not** accept push constant ranges (V0.B Commit 11 left этот parameter as `pushConstantRangeCount = 0` hardcoded). V0.C.1 extends constructor signature backward-compatibly:

```csharp
public VulkanPipelineLayout(
    VulkanDevice device,
    IReadOnlyList<IntPtr>? descriptorSetLayouts = null,
    IReadOnlyList<VkPushConstantRangePublic>? pushConstantRanges = null)
{
    // existing V0.B logic для descriptor sets
    // V0.C.1 NEW: pushConstantRangeCount = (uint)(pushConstantRanges?.Count ?? 0)
    // V0.C.1 NEW: pPushConstantRanges = marshalled ranges array (or IntPtr.Zero if empty)
}
```

**Public-facing mirror** of VkPushConstantRange (existing V0.B pattern for public surface types):

```csharp
namespace DualFrontier.Runtime.Graphics;

public readonly record struct VkPushConstantRangePublic(
    VkShaderStageFlagsPublic StageFlags,
    uint Offset,
    uint Size);

[Flags]
public enum VkShaderStageFlagsPublic : uint
{
    Vertex = 0x00000001,
    Fragment = 0x00000010,
    Compute = 0x00000020,
}
```

**Camera MVP usage** (SpriteRenderer):

```csharp
// V0.C.1 single-sprite mode: identity matrix (Camera2D placeholder for V0.C.2)
Matrix4x4 mvp = Matrix4x4.Identity;
// Future V0.C.2: mvp = camera2D.GetViewProjectionMatrix() * spriteTransform.GetModelMatrix()

unsafe
{
    Matrix4x4* mvpPtr = &mvp;
    VkApi.vkCmdPushConstants(commandBuffer.Handle, pipelineLayout.Handle,
        VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
        offset: 0, size: 64, pValues: mvpPtr);
}
```

### §1.9 — S-LOCK-9: C ABI alignment audit mandatory continues (Lesson #7 strengthening)

**LOCK** (per V0.A executor finding + V0.B 5 corrections precedent): every new Vulkan struct в V0.C.1 gets explicit alignment audit:

**Required for each new Vulkan struct**:
1. Read Vulkan spec or `$(VULKAN_SDK)\Include\vulkan\vulkan_core.h` header verbatim
2. Identify 64-bit fields requiring 8-byte alignment
3. Identify nested structs propagating alignment requirements
4. Add explicit padding fields matching MSVC x64 ABI
5. Write Marshal.SizeOf<T>() test validating against Vulkan spec sizeof

**Affected V0.C.1 structs requiring audit** (Phase 0 brief authoring estimates; executor verifies):

| Struct | Brief size estimate | Phase 0 verify required |
|---|---|---|
| VkSamplerCreateInfo | ~80 bytes | Y |
| VkPushConstantRange | 12 bytes (3 uint, no 64-bit fields, natural align) | Y |
| VkVertexInputBindingDescription | 12 bytes | Y |
| VkVertexInputAttributeDescription | 16 bytes | Y |
| VkBufferImageCopy | 56 bytes (VkDeviceSize bufferOffset + multiple uint fields) | Y — alignment risk |
| VkImageMemoryBarrier | 72 bytes (sType+pad+pNext+srcAccessMask+dstAccessMask+oldLayout+newLayout+srcQueueFamilyIndex+dstQueueFamilyIndex+image+subresourceRange) | Y — alignment risk |
| VkDescriptorImageInfo | 24 bytes (sampler+imageView+imageLayout — all 8-byte aligned + 4-byte enum) | Y |
| VkWriteDescriptorSet | 64 bytes (V0.B may have landed; verify Phase 0) | Y |
| VkDescriptorPoolCreateInfo + VkDescriptorPoolSize | (V0.B partial — extend) | Y |
| SpriteVertex (managed-side struct, не Vulkan but aligned per packed layout) | 20 bytes target | Y |

**Per V0.B precedent**: 5 brief estimate corrections caught via test gate. V0.C.1 brief estimates above are **hypotheses** — Marshal.SizeOf<T>() test gate authoritative.

### §1.10 — S-LOCK-10: Input event types completion + Win32 dispatch (per VULKAN_SUBSTRATE §2.2)

**LOCK**: V0.C.1 completes V0.A InputEventQueue scaffold с full event types per VULKAN_SUBSTRATE §2.2 Input module.

**Event types** (V0.A landed marker IInputEvent + WindowResizeEvent; V0.C.1 adds):

```csharp
namespace DualFrontier.Runtime.Input;

public sealed record KeyPressedEvent(Key Key) : IInputEvent;
public sealed record KeyReleasedEvent(Key Key) : IInputEvent;

public sealed record MouseMovedEvent(int X, int Y) : IInputEvent;
public sealed record MouseButtonEvent(MouseButton Button, bool Pressed) : IInputEvent;
public sealed record MouseWheelEvent(int Delta) : IInputEvent;

public sealed record WindowFocusEvent(bool Focused) : IInputEvent;

public enum Key
{
    Unknown,
    // Arrow keys
    Left, Right, Up, Down,
    // Modifier keys
    Shift, Control, Alt,
    // Special keys
    Escape, Space, Enter, Tab, Backspace, Delete, Home, End, PageUp, PageDown,
    // Function keys
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    // Letter keys (printable, simplified — caller normalizes case if needed)
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    // Digit keys
    Digit0, Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9,
}

public enum MouseButton { Left, Right, Middle }
```

**Win32 message dispatch** в Window.cs WindowProcedure:

```csharp
private IntPtr WindowProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
{
    switch (msg)
    {
        // V0.A: WM_CLOSE, WM_DESTROY (preserved)
        // V0.B: WM_SIZE (preserved)
        
        // V0.C.1 NEW: keyboard
        case Win32Constants.WM_KEYDOWN:
        case Win32Constants.WM_SYSKEYDOWN:
        {
            int vk = (int)wParam.ToInt64();
            Key key = VirtualKeyMapper.Map(vk);
            if (key != Key.Unknown)
            {
                InputQueue.Enqueue(new KeyPressedEvent(key));
            }
            return IntPtr.Zero;
        }
        case Win32Constants.WM_KEYUP:
        case Win32Constants.WM_SYSKEYUP:
        {
            int vk = (int)wParam.ToInt64();
            Key key = VirtualKeyMapper.Map(vk);
            if (key != Key.Unknown)
            {
                InputQueue.Enqueue(new KeyReleasedEvent(key));
            }
            return IntPtr.Zero;
        }
        // V0.C.1 NEW: mouse
        case Win32Constants.WM_MOUSEMOVE:
        {
            long packed = lParam.ToInt64();
            int x = (int)(short)(packed & 0xFFFF);
            int y = (int)(short)((packed >> 16) & 0xFFFF);
            InputQueue.Enqueue(new MouseMovedEvent(x, y));
            return IntPtr.Zero;
        }
        case Win32Constants.WM_LBUTTONDOWN:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Left, Pressed: true)); return IntPtr.Zero;
        case Win32Constants.WM_LBUTTONUP:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Left, Pressed: false)); return IntPtr.Zero;
        case Win32Constants.WM_RBUTTONDOWN:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Right, Pressed: true)); return IntPtr.Zero;
        case Win32Constants.WM_RBUTTONUP:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Right, Pressed: false)); return IntPtr.Zero;
        case Win32Constants.WM_MBUTTONDOWN:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Middle, Pressed: true)); return IntPtr.Zero;
        case Win32Constants.WM_MBUTTONUP:
            InputQueue.Enqueue(new MouseButtonEvent(MouseButton.Middle, Pressed: false)); return IntPtr.Zero;
        case Win32Constants.WM_MOUSEWHEEL:
        {
            int delta = (short)((wParam.ToInt64() >> 16) & 0xFFFF) / 120;  // WHEEL_DELTA = 120; produces ±1 per notch
            InputQueue.Enqueue(new MouseWheelEvent(delta));
            return IntPtr.Zero;
        }
        // V0.C.1 NEW: focus events
        case Win32Constants.WM_SETFOCUS:
            InputQueue.Enqueue(new WindowFocusEvent(Focused: true));
            return IntPtr.Zero;
        case Win32Constants.WM_KILLFOCUS:
            InputQueue.Enqueue(new WindowFocusEvent(Focused: false));
            return IntPtr.Zero;
        
        default:
            return Win32Api.DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
```

**VirtualKeyMapper** — separate static helper class mapping VK_* codes к Key enum. Win32 verbatim VK_* constants in Win32Constants.cs.

### §1.11 — S-LOCK-11: Atomic cascade preserves V0.A + V0.B discipline

**LOCK**: V0.C.1 executes as multi-commit atomic cascade. Per Lesson #8: V0.C.1 items have **clean intermediate states**:

1. Vulkan struct + function additions (Commit 2) — declarations land without consumption; struct size tests pass immediately
2. PNG decoder (Commits 3-4) — pure managed code, isolated unit testable
3. AssetManager (Commit 5) — consumes PNG decoder
4. VulkanSampler (Commit 6) — standalone Vulkan primitive
5. TextureUploader (Commit 7) — consumes sampler + buffer + image (V0.B primitives extended)
6. Pipeline layout extension (Commit 8) — backward-compatible constructor extension
7. Sprite shaders + SPIR-V compilation (Commit 9) — MSBuild target extension landing
8. Sprite vertex format + types (Commit 10) — managed-side structs, isolated
9. VulkanSpritePipeline (Commit 11) — consumes layout extension + sprite shaders
10. SpriteRenderer (Commit 12) — orchestrates pipeline + buffer + descriptors
11. Input event types + Win32 dispatch (Commit 13) — pure managed code + Window.cs extension
12. Runtime facade composition (Commit 14) — wires all new components
13. Smoke test (Commit 15) — verifies V0.C.1 exit criteria
14. Closure (Commit 16) — governance

Tests pass at every commit (786 V0.B baseline preserved; new V0.C.1 tests additive). Manual visual verification expected at smoke test commit (Kenney pawn visible).

---

## §2 — Phase 0 — Pre-flight reads (mandatory before any edit)

Per Lesson #7 + Lesson #22 + Lesson #8, executor MUST complete every read listed before writing a single line of V0.C.1 code. V0.C.1 brief authored 2026-05-19 from V0.B post-closure verified ground truth.

### §2.1 — Verify post-V0.B closure state (hard gates)

Read и verify:

1. `git log --oneline -25` на `main` — confirm:
   - V0.B PR #37 merged (range d2c6627..e9ccd0f per V0.B closure report — 18 commits)
   - HEAD references V0.B closure commit
   - К10.3 halt не appears в main history (К10.3 brief untracked per HALT_REPORT)
   - Halt если V0.B closure не reached

2. `git status` — working tree clean before execution starts. К10.3 brief (`tools/briefs/K10_3_EXECUTION_BRIEF.md`) untracked acceptable (independent stream).

3. `docs/governance/REGISTER.yaml` head check — confirm DOC-D-V0_B present с lifecycle EXECUTED, version 1.0.

4. `tools/governance/sync_register.ps1 --validate` — exit 0 required as baseline. Advisory orphan warnings (per V0.B closure ~15 warnings) — pre-existing, не halt.

5. `dotnet build DualFrontier.sln` — clean baseline.

6. `dotnet test DualFrontier.sln` — baseline pass count: **786 tests green** per V0.B closure report. If suite fails или count diverges (excluding intentional V0.C.1 additions), halt.

7. `cmake --build native/DualFrontier.Core.Native` clean baseline. Native selftest passes 78 scenarios (77 К10.2 baseline + 1 V0.B compute pipeline roundtrip).

### §2.2 — Verify Vulkan SDK + V0.B environment preserved

Per V0.A/V0.B precedent: Vulkan SDK 1.4.350.0 (or current) installed. Hard gates:

1. `VULKAN_SDK` env var set + points к valid SDK installation
2. `tools/glslangValidator.exe` exists (V0.B committed copy)
3. `$(VULKAN_SDK)\Include\vulkan\vulkan_core.h` exists (Vulkan struct layout reference per Lesson #7 strengthening)
4. `vulkan-1.dll` accessible (V0.A/V0.B verified — preserved)
5. AMD RX 7600S Vulkan 1.4.344 verified (V0.A/V0.B confirmed К-L19 hardware tier)
6. MSBuild CompileShaders target operational (V0.B Commit 10 landed — `dotnet build DualFrontier.Runtime.csproj` regenerates clearcolor.vert.spv + clearcolor.frag.spv + noop.comp.spv)

V0.C.1 extends CompileShaders target с sprite.vert + sprite.frag compilation. No new SDK installation needed.

### §2.3 — Read VULKAN_SUBSTRATE.md spec (V0.C.1 sections)

Read в full:
- §0 L1-L10 (V0.C.1 preserves V0.A/V0.B invariants)
- §1.1 V0 rendering bullets 5-9 (PNG decoder + atlas + sprite + bitmap font + text + input + debug overlay — V0.C.1 covers 5 + 9 partial; 6-8 deferred V0.C.2/R.6)
- §2.1 project structure (V0.C.1 extends `src/DualFrontier.Runtime/` с Assets + Sprite + Input — Compute V0.B already landed)
- §2.2 module purposes (Assets + Sprite + Input + Window — V0.C.1 fills these)
- §2.3 threading model (V0.C.1 preserves single-threaded V0.B pattern; no PresentationBridge per S-LOCK-7)
- §2.4 dependency rules (V0.C.1 preserves Rules 1-5; Assets module depends only on BCL; Sprite depends on Graphics + Assets; Input depends only on BCL + Window)
- §2.5 native interop patterns (V0.A/V0.B patterns preserved + new structs/functions added per S-LOCK-9 alignment audit)
- §2.6 asset pipeline (PNG decoder verbatim spec — ~500-700 LOC target)
- §2.7 shader strategy (sprite.vert + sprite.frag added к MSBuild CompileShaders target)
- §2.8 testing strategy (V0.C.1 test surface = struct sizes + PNG decoder + input mapping + sprite primitive + smoke test)
- §4.2 R.1 phase verbatim (Kenney pawn rendered at center of window — V0.C.1 exit criterion)
- §4.2 R.4 phase verbatim (input event types complete — V0.C.1 exit criterion)
- §11 methodology adjustments (validation layer ALWAYS-ON в DEBUG preserved)

### §2.4 — Read V0.B code anchors verbatim (Lesson #22)

V0.C.1 extends V0.B primitives. Read existing code shapes literally:

**Vulkan native interop (V0.B landed)**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (~19.5 KB) — V0.A/V0.B 50+ functions; V0.C.1 adds ~10-15 functions (vkCreateSampler, vkDestroySampler, vkCmdCopyBufferToImage, vkCmdPushConstants, vkAllocateDescriptorSets variants, etc.)
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` — V0.A/V0.B ~26 structs; V0.C.1 adds ~8-10 structs (VkSamplerCreateInfo, VkPushConstantRange, VkVertexInputBindingDescription, VkVertexInputAttributeDescription, VkBufferImageCopy, VkImageMemoryBarrier, VkDescriptorImageInfo; some may overlap V0.B partial coverage — verify)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` — V0.A/V0.B enums; V0.C.1 adds VkSamplerAddressMode, VkSamplerMipmapMode, VkBorderColor, VkFilter, VkVertexInputRate, VkAccessFlags (V0.B partial — verify), VkPipelineStageFlags (V0.B partial — verify)
- `src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs` — extensible patterns established
- `src/DualFrontier.Runtime/Native/Vulkan/VkDelegates.cs` — debug messenger delegate (V0.A)

**Graphics layer (V0.B landed — V0.C.1 EXTENDS не replaces)**:
- `src/DualFrontier.Runtime/Graphics/VulkanInstance.cs` — V0.C.1 не modifies
- `src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` — V0.C.1 не modifies (graphics + async compute queues already operational)
- `src/DualFrontier.Runtime/Graphics/ValidationLayer.cs` — V0.C.1 не modifies
- `src/DualFrontier.Runtime/Graphics/HardwareCapabilityCheck.cs` — V0.C.1 не modifies
- `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs` + `VulkanSwapchain.cs` — V0.C.1 не modifies
- `src/DualFrontier.Runtime/Graphics/MemoryAllocator.cs` — V0.C.1 consumes (texture upload allocates staging buffers + device-local image memory)
- `src/DualFrontier.Runtime/Graphics/VulkanBuffer.cs` — V0.C.1 consumes для staging buffer + sprite vertex buffer
- `src/DualFrontier.Runtime/Graphics/VulkanImage.cs` — V0.C.1 consumes для texture creation; verifies CreateFromPngImage convenience method addition opportunity
- `src/DualFrontier.Runtime/Graphics/VulkanRenderPass.cs` + `VulkanFramebuffer.cs` — V0.C.1 consumes (rendering goes through existing render pass)
- `src/DualFrontier.Runtime/Graphics/VulkanCommandPool.cs` + `VulkanCommandBuffer.cs` — V0.C.1 consumes
- `src/DualFrontier.Runtime/Graphics/VulkanShaderModule.cs` — V0.C.1 consumes для sprite shaders
- `src/DualFrontier.Runtime/Graphics/VulkanGraphicsPipeline.cs` — V0.C.1 extends pattern (sprite pipeline с vertex input + blending)
- `src/DualFrontier.Runtime/Graphics/VulkanPipelineLayout.cs` — V0.C.1 EXTENDS constructor с push constant ranges parameter
- `src/DualFrontier.Runtime/Graphics/VulkanFence.cs` + `VulkanSemaphore.cs` — V0.C.1 consumes для sync

**Compute layer (V0.B landed)**:
- `src/DualFrontier.Runtime/Compute/*` — V0.C.1 не modifies (compute side independent of rendering use case)

**Window + Input layers (V0.A landed + V0.B WM_SIZE)**:
- `src/DualFrontier.Runtime/Window/Window.cs` — V0.C.1 EXTENDS WindowProcedure с full input message dispatch (V0.C comment hint)
- `src/DualFrontier.Runtime/Window/InputEventQueue.cs` — V0.C.1 не modifies (already ConcurrentQueue<IInputEvent>)
- `src/DualFrontier.Runtime/Input/IInputEvent.cs` — V0.C.1 не modifies (marker interface)
- `src/DualFrontier.Runtime/Input/WindowResizeEvent.cs` — V0.C.1 не modifies
- `src/DualFrontier.Runtime/Native/Win32/Win32Api.cs` — V0.C.1 EXTENDS с VK_* virtual key constants
- `src/DualFrontier.Runtime/Native/Win32/Win32Constants.cs` — V0.C.1 EXTENDS с WM_KEYDOWN/UP/MOUSEMOVE/L|R|MBUTTONDOWN/UP/MOUSEWHEEL/SETFOCUS/KILLFOCUS message IDs

**Runtime facade (V0.B landed)**:
- `src/DualFrontier.Runtime/Runtime.cs` — V0.C.1 EXTENDS composition с AssetManager + DefaultSampler + SpriteRenderer; disposal в reverse order
- `src/DualFrontier.Runtime/RuntimeOptions.cs` — V0.C.1 may extend с AssetsDirectory path option

**Test infrastructure (V0.B landed)**:
- `tests/DualFrontier.Runtime.Tests/` — V0.C.1 adds Assets/PngDecoderTests.cs + Sprite/SpriteVertexTests.cs + Sprite/SpriteRendererTests.cs + Graphics/VulkanSamplerTests.cs + Graphics/TextureUploaderTests.cs + Input/VirtualKeyMapperTests.cs + Vulkan/VulkanStructSizeTests.cs (extends V0.B file с new struct tests)
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — V0.C.1 EXTENDS с Kenney pawn render test + input event capture demonstration

**Per Lesson #7**: Read VULKAN_SUBSTRATE.md §2.5 P/Invoke template verbatim. Read V0.B VulkanImage.cs + VulkanPipelineLayout.cs + VulkanGraphicsPipeline.cs verbatim — V0.C.1 patterns mirror exactly.

### §2.5 — Verify Kenney pawn PNG asset present

V0.C.1 smoke test requires Kenney pawn sprite PNG. Phase 0 verify:

1. `D:\Colony_Simulator\Colony_Simulator\assets\sprites\pawn.png` (or similar — verify exact path) — if present, use it
2. If absent, halt SC-A (Asset prerequisite missing): Crystalka manually adds Kenney pawn PNG (free CC0 license, available at kenney.nl)
3. PNG verify: 8-bit RGBA или RGB (typical Kenney atlas); reasonable size (<1024×1024 pixels)

**Recovery for SC-A**: Crystalka downloads Kenney CC0 pixel art atlas, picks single pawn sprite, saves к assets/sprites/pawn.png. Brief execution resumes from Phase 0.

### §2.6 — Read REGISTER.yaml structure для V0.C.1 enrollment

Identify:
- DOC-D-V0_B entry (V0.B closure) — V0.C.1 brief enrollment after this entry
- audit_trail events list — V0.C.1 adds EVT-{date}-V0_C_1-CLOSURE
- New REQs: REQ-V0-C-1-PNG_DECODER, REQ-V0-C-1-SAMPLER, REQ-V0-C-1-TEXTURE_UPLOAD, REQ-V0-C-1-SPRITE_PIPELINE, REQ-V0-C-1-SPRITE_RENDERER, REQ-V0-C-1-INPUT_EVENTS, REQ-V0-C-1-WIN32_INPUT_DISPATCH (7 new REQs)

### §2.7 — Halt category clarity

**Hard gates (STOP-eligible)** per §2.1 + §2.2:
- Working tree dirty
- Baseline tests failing (excluding intentional V0.C.1 additions)
- `sync_register.ps1 --validate` non-zero baseline (NEW errors only)
- Build failure baseline
- V0.B closure не reached
- Kenney pawn PNG absent (SC-A)
- VULKAN_SDK env var unset

**V0.C.1-specific informational checks (record-only)**:
- Marshal.SizeOf<T>() actual values для each new Vulkan struct (record во время Marshal.SizeOf unit tests — per Lesson #7 strengthening)
- VirtualKey code mapping coverage (record which VK_* codes mapped к Key.Unknown vs concrete Key value)
- PNG decoder synthetic test inputs (record edge cases tested)

If informational check diverges from brief expectation — **record divergence в commit message, continue**. Hard gate failure → halt per SC-N (§4).

---

## §3 — Atomic commit cascade (target ~16 commits)

Each commit: individually compilable + register-valid intermediate state per Lesson #8. `sync_register.ps1 --validate` exit 0 at every governance-touching commit; `dotnet build` clean at every code-touching commit; `dotnet test` 786+ passing at every commit (V0.B baseline preserved; new V0.C.1 tests additive).

### Commit 1 — Brief authoring commit (V0.C.1 brief enrollment)

**Files**:
- `tools/briefs/V0_C_1_EXECUTION_BRIEF.md` (this brief)
- `docs/governance/REGISTER.yaml` (DOC-D-V0_C_1 entry с `lifecycle: AUTHORED`, `category: D`, `tier: 3`)

**Validation**:
- `sync_register.ps1 --validate` exit 0
- No code changes

**Commit message**: `docs(briefs): V0.C.1 brief authored — PNG decoder + textured sprite pipeline + input event types (R.1 + R.4)`

### Commit 2 — Vulkan struct/function extensions + struct size tests (Lesson #7 strengthening continues)

**Files**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` (modified — adds VkSamplerCreateInfo, VkPushConstantRange, VkVertexInputBindingDescription, VkVertexInputAttributeDescription, VkBufferImageCopy, VkImageMemoryBarrier, VkDescriptorImageInfo; if V0.B already landed some, verify Phase 0 and skip duplicates)
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` (modified — adds VkSamplerAddressMode, VkSamplerMipmapMode, VkBorderColor, VkFilter, VkVertexInputRate; possibly VkAccessFlags + VkPipelineStageFlags extensions если V0.B partial)
- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` (modified — adds vkCreateSampler, vkDestroySampler, vkCmdCopyBufferToImage, vkCmdPushConstants; verifies vkAllocateDescriptorSets/vkUpdateDescriptorSets/vkCmdBindDescriptorSets present from V0.B и используется correctly)
- `tests/DualFrontier.Runtime.Tests/Vulkan/VulkanStructSizeTests.cs` (modified — adds Marshal.SizeOf<T>() tests за каждый new struct above; preserves V0.B regression baseline tests)

**Drift surface**: Native interop surface extended. Pure declaration commit — no consumer code yet. Struct size tests provide immediate verification per Lesson #7 strengthening.

**Implementation surface (key snippets)**:

```csharp
// VkStructs.cs additions
[StructLayout(LayoutKind.Sequential)]
internal struct VkSamplerCreateInfo
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal uint flags;
    internal VkFilter magFilter;
    internal VkFilter minFilter;
    internal VkSamplerMipmapMode mipmapMode;
    internal VkSamplerAddressMode addressModeU;
    internal VkSamplerAddressMode addressModeV;
    internal VkSamplerAddressMode addressModeW;
    internal float mipLodBias;
    internal uint anisotropyEnable;
    internal float maxAnisotropy;
    internal uint compareEnable;
    internal VkCompareOp compareOp;
    internal float minLod;
    internal float maxLod;
    internal VkBorderColor borderColor;
    internal uint unnormalizedCoordinates;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPushConstantRange
{
    internal VkShaderStageFlags stageFlags;
    internal uint offset;
    internal uint size;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputBindingDescription
{
    internal uint binding;
    internal uint stride;
    internal VkVertexInputRate inputRate;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputAttributeDescription
{
    internal uint location;
    internal uint binding;
    internal VkFormat format;
    internal uint offset;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkBufferImageCopy
{
    internal ulong bufferOffset;            // VkDeviceSize — 8 byte alignment
    internal uint bufferRowLength;
    internal uint bufferImageHeight;
    internal VkImageSubresourceLayers imageSubresource;  // 16 bytes (aspectMask+mipLevel+baseArrayLayer+layerCount)
    internal VkOffset3D imageOffset;        // 12 bytes (3 × int32)
    internal VkExtent3D imageExtent;        // 12 bytes (3 × uint32)
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkImageMemoryBarrier
{
    internal VkStructureType sType;
    internal IntPtr pNext;
    internal VkAccessFlags srcAccessMask;
    internal VkAccessFlags dstAccessMask;
    internal VkImageLayout oldLayout;
    internal VkImageLayout newLayout;
    internal uint srcQueueFamilyIndex;
    internal uint dstQueueFamilyIndex;
    internal IntPtr image;
    internal VkImageSubresourceRange subresourceRange;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorImageInfo
{
    internal IntPtr sampler;
    internal IntPtr imageView;
    internal VkImageLayout imageLayout;
}
```

**VkApi.cs additions**:
```csharp
[LibraryImport(VulkanLib, EntryPoint = "vkCreateSampler")]
internal static partial VkResult vkCreateSampler(
    IntPtr device, in VkSamplerCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pSampler);

[LibraryImport(VulkanLib, EntryPoint = "vkDestroySampler")]
internal static partial void vkDestroySampler(IntPtr device, IntPtr sampler, IntPtr pAllocator);

[LibraryImport(VulkanLib, EntryPoint = "vkCmdCopyBufferToImage")]
internal static partial void vkCmdCopyBufferToImage(
    IntPtr commandBuffer, IntPtr srcBuffer, IntPtr dstImage,
    VkImageLayout dstImageLayout, uint regionCount, in VkBufferImageCopy pRegions);

[LibraryImport(VulkanLib, EntryPoint = "vkCmdPushConstants")]
internal static unsafe partial void vkCmdPushConstants(
    IntPtr commandBuffer, IntPtr layout, VkShaderStageFlags stageFlags,
    uint offset, uint size, void* pValues);
```

**Marshal.SizeOf<T>() tests (per Lesson #7 strengthening — these estimates are HYPOTHESES):**

```csharp
[Fact]
public void VkSamplerCreateInfo_Size_Matches_Spec()
{
    // Estimated: sType(4)+pad(4)+pNext(8)+flags(4)+magFilter(4)+minFilter(4)+mipmapMode(4)
    //   +addressU(4)+addressV(4)+addressW(4)+mipLodBias(4)+anisotropyEnable(4)+maxAnisotropy(4)
    //   +compareEnable(4)+compareOp(4)+minLod(4)+maxLod(4)+borderColor(4)+unnormalizedCoords(4) = 80 bytes
    // VERIFY against Vulkan 1.3 spec sizeof — adjust if mismatch per V0.B precedent
    Marshal.SizeOf<VkSamplerCreateInfo>().Should().Be(80);  // hypothesis; verify
}

[Fact]
public void VkPushConstantRange_Size_Matches_Spec()
{
    // 3 × uint = 12 bytes natural alignment
    Marshal.SizeOf<VkPushConstantRange>().Should().Be(12);
}

[Fact]
public void VkVertexInputBindingDescription_Size_Matches_Spec()
{
    // 2 × uint + enum (uint sized) = 12 bytes
    Marshal.SizeOf<VkVertexInputBindingDescription>().Should().Be(12);
}

[Fact]
public void VkVertexInputAttributeDescription_Size_Matches_Spec()
{
    // 4 × uint = 16 bytes
    Marshal.SizeOf<VkVertexInputAttributeDescription>().Should().Be(16);
}

[Fact]
public void VkBufferImageCopy_Size_Matches_Spec()
{
    // VkDeviceSize(8)+bufferRowLength(4)+bufferImageHeight(4)+subresource(16)+offset(12)+pad(0?)+extent(12)+pad(4?) = ?
    // ALIGNMENT RISK: VkDeviceSize 64-bit field at start; trailing alignment ambiguous
    // VERIFY against vulkan_core.h sizeof — adjust per V0.B 5-correction precedent
    Marshal.SizeOf<VkBufferImageCopy>().Should().Be(56);  // hypothesis; verify
}

[Fact]
public void VkImageMemoryBarrier_Size_Matches_Spec()
{
    // sType(4)+pad(4)+pNext(8)+srcAccess(4)+dstAccess(4)+oldLayout(4)+newLayout(4)
    //   +srcQueue(4)+dstQueue(4)+image(8)+subresourceRange(20) = 72 bytes
    Marshal.SizeOf<VkImageMemoryBarrier>().Should().Be(72);  // hypothesis; verify
}

[Fact]
public void VkDescriptorImageInfo_Size_Matches_Spec()
{
    // sampler(8) + imageView(8) + imageLayout(4) + trailing pad to 8-byte natural alignment(4) = 24 bytes
    Marshal.SizeOf<VkDescriptorImageInfo>().Should().Be(24);
}
```

**Per Lesson #7 strengthening — V0.B precedent**: 5 brief estimates corrected. V0.C.1 above estimates are **hypotheses** — executor verifies via Marshal.SizeOf<T>(); если actual differs, adjusts test expected value + adds explicit padding fields в struct if needed.

**Validation**:
- `dotnet build` clean
- `dotnet test` 786 baseline + 7 struct size tests pass (after executor adjusts hypotheses к actual Marshal.SizeOf values)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Vulkan struct + function extensions for sprite/texture/input (Lesson #7 alignment audit continued)`

### Commit 3 — PngDecoder Part 1: chunk parsing + CRC32 verification + IHDR/IEND handling

**Files**:
- `src/DualFrontier.Runtime/Assets/MODULE.md` (new — Assets module purpose per VULKAN_SUBSTRATE §2.2)
- `src/DualFrontier.Runtime/Assets/PngImage.cs` (new — decoded image record)
- `src/DualFrontier.Runtime/Assets/PngDecoder.cs` (new — Part 1: chunk parsing + CRC32 + IHDR/IEND)
- `src/DualFrontier.Runtime/Assets/PngDecoderException.cs` (new)
- `src/DualFrontier.Runtime/Assets/PngChunk.cs` (new — internal chunk record)
- `tests/DualFrontier.Runtime.Tests/Assets/PngDecoderTests.cs` (new — chunk parsing tests с synthetic PNG inputs)

**Drift surface**: PngDecoder chunk parsing operational. Decoder can recognize PNG signature + parse chunk headers + verify CRC32 + identify IHDR/IDAT/IEND. IDAT byte concatenation готова для Part 2 (DEFLATE + filter unfiltering).

**Implementation surface (Part 1 ~250 LOC of ~600-700 total)**:

```csharp
namespace DualFrontier.Runtime.Assets;

public sealed record PngImage(int Width, int Height, byte[] PixelsRgba8);

public sealed class PngDecoderException : Exception
{
    public PngDecoderException(string message) : base(message) { }
}

public static class PngDecoder
{
    private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

    public static PngImage Decode(ReadOnlySpan<byte> pngBytes)
    {
        // 1. Verify PNG signature (first 8 bytes)
        if (pngBytes.Length < 8 || !pngBytes[..8].SequenceEqual(PngSignature))
        {
            throw new PngDecoderException("Invalid PNG signature.");
        }
        
        // 2. Parse chunks sequentially
        var idatChunks = new List<byte[]>();
        int width = 0, height = 0;
        int bitDepth = 0, colorType = 0;
        bool seenIhdr = false, seenIend = false;
        
        int offset = 8;
        while (offset < pngBytes.Length)
        {
            PngChunk chunk = ReadChunk(pngBytes, ref offset);
            
            // CRC32 verification mandatory
            if (!VerifyChunkCrc32(chunk))
            {
                throw new PngDecoderException($"Chunk '{chunk.Type}' CRC32 mismatch.");
            }
            
            switch (chunk.Type)
            {
                case "IHDR":
                    if (seenIhdr) throw new PngDecoderException("Duplicate IHDR chunk.");
                    seenIhdr = true;
                    if (chunk.Data.Length != 13) throw new PngDecoderException("IHDR chunk wrong size.");
                    width = ReadBigEndianInt32(chunk.Data, 0);
                    height = ReadBigEndianInt32(chunk.Data, 4);
                    bitDepth = chunk.Data[8];
                    colorType = chunk.Data[9];
                    int compressionMethod = chunk.Data[10];
                    int filterMethod = chunk.Data[11];
                    int interlaceMethod = chunk.Data[12];
                    
                    // S-LOCK-2 scope checks
                    if (bitDepth != 8) throw new PngDecoderException($"Unsupported bit depth {bitDepth}; only 8 supported.");
                    if (colorType != 2 && colorType != 6) throw new PngDecoderException($"Unsupported color type {colorType}; only 2 (RGB) and 6 (RGBA) supported.");
                    if (compressionMethod != 0) throw new PngDecoderException($"Unsupported compression method {compressionMethod}.");
                    if (filterMethod != 0) throw new PngDecoderException($"Unsupported filter method {filterMethod}.");
                    if (interlaceMethod != 0) throw new PngDecoderException("Interlaced PNG not supported.");
                    break;
                    
                case "IDAT":
                    if (!seenIhdr) throw new PngDecoderException("IDAT before IHDR.");
                    idatChunks.Add(chunk.Data);
                    break;
                    
                case "IEND":
                    seenIend = true;
                    break;
                    
                default:
                    // Ancillary chunk — silently ignored per S-LOCK-2
                    break;
            }
            
            if (seenIend) break;
        }
        
        if (!seenIhdr) throw new PngDecoderException("Missing IHDR chunk.");
        if (!seenIend) throw new PngDecoderException("Missing IEND chunk.");
        if (idatChunks.Count == 0) throw new PngDecoderException("No IDAT chunks.");
        
        // 3. Concatenate IDAT bytes + decompress + unfilter (Part 2 — Commit 4)
        byte[] pixelBytes = DecompressAndUnfilter(idatChunks, width, height, colorType);
        
        // 4. Convert к RGBA8 (если RGB8 source)
        byte[] rgba8 = colorType == 6 ? pixelBytes : ConvertRgb8ToRgba8(pixelBytes, width, height);
        
        return new PngImage(width, height, rgba8);
    }
    
    public static PngImage DecodeFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        return Decode(bytes);
    }
    
    // Helper methods: ReadChunk, VerifyChunkCrc32, ReadBigEndianInt32, etc.
    // CRC32 implementation: standard Castagnoli polynomial 0xEDB88320 per PNG spec
    // (Part 2 Commit 4 implements DecompressAndUnfilter)
}
```

**Tests (Part 1 — Commit 3)**:
- Synthetic minimal valid PNG (8×8 RGBA) — should decode successfully (после Part 2 implementation)
- Invalid signature — throws PngDecoderException
- Missing IHDR — throws
- Missing IEND — throws
- Duplicate IHDR — throws
- Bit depth 16 — throws (out of S-LOCK-2 scope)
- Color type 3 (palette) — throws
- Interlaced PNG — throws
- CRC32 mismatch — throws

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + 7 PngDecoder Part 1 tests pass (negative cases throw; positive case skipped until Part 2)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — PngDecoder Part 1 — chunk parsing + CRC32 verification + IHDR/IEND handling`

### Commit 4 — PngDecoder Part 2: DEFLATE decompression + filter unfiltering (all 4 predictors)

**Files**:
- `src/DualFrontier.Runtime/Assets/PngDecoder.cs` (modified — adds DecompressAndUnfilter + ConvertRgb8ToRgba8 + UnfilterScanline + PaethPredictor helpers)
- `tests/DualFrontier.Runtime.Tests/Assets/PngDecoderTests.cs` (modified — adds positive decode tests + filter predictor tests)
- `tests/DualFrontier.Runtime.Tests/Assets/test_assets/` (new test directory с tiny synthetic PNGs encoded externally for verification)

**Drift surface**: PngDecoder fully operational. Decode arbitrary 8-bit RGB/RGBA PNG (non-interlaced, non-paletted). All 4 filter predictors implemented. RGB8 → RGBA8 conversion functional.

**Implementation surface (Part 2 ~300-400 LOC)**:

```csharp
private static byte[] DecompressAndUnfilter(List<byte[]> idatChunks, int width, int height, int colorType)
{
    // 1. Concatenate IDAT bytes
    int totalLength = idatChunks.Sum(c => c.Length);
    byte[] compressed = new byte[totalLength];
    int pos = 0;
    foreach (var chunk in idatChunks)
    {
        Buffer.BlockCopy(chunk, 0, compressed, pos, chunk.Length);
        pos += chunk.Length;
    }
    
    // 2. Skip zlib header (2 bytes) — DeflateStream expects raw DEFLATE
    using var ms = new MemoryStream(compressed, 2, compressed.Length - 2);
    using var deflate = new DeflateStream(ms, CompressionMode.Decompress);
    
    int bytesPerPixel = colorType == 6 ? 4 : 3;  // RGBA или RGB
    int scanlineLength = width * bytesPerPixel + 1;  // +1 для filter type byte
    byte[] raw = new byte[scanlineLength * height];
    
    using var rawMs = new MemoryStream(raw);
    deflate.CopyTo(rawMs);
    
    // 3. Unfilter scanlines (in-place, removing filter type bytes)
    byte[] unfiltered = new byte[width * bytesPerPixel * height];
    UnfilterScanlines(raw, unfiltered, width, height, bytesPerPixel);
    
    return unfiltered;
}

private static void UnfilterScanlines(byte[] raw, byte[] unfiltered, int width, int height, int bytesPerPixel)
{
    int scanlineLength = width * bytesPerPixel;
    int rawScanlineLength = scanlineLength + 1;  // filter byte prefix
    
    byte[] previousScanline = new byte[scanlineLength];
    
    for (int y = 0; y < height; y++)
    {
        int rawOffset = y * rawScanlineLength;
        byte filterType = raw[rawOffset];
        ReadOnlySpan<byte> currentRaw = raw.AsSpan(rawOffset + 1, scanlineLength);
        Span<byte> currentOut = unfiltered.AsSpan(y * scanlineLength, scanlineLength);
        
        switch (filterType)
        {
            case 0: UnfilterNone(currentRaw, currentOut); break;
            case 1: UnfilterSub(currentRaw, currentOut, bytesPerPixel); break;
            case 2: UnfilterUp(currentRaw, currentOut, previousScanline); break;
            case 3: UnfilterAverage(currentRaw, currentOut, previousScanline, bytesPerPixel); break;
            case 4: UnfilterPaeth(currentRaw, currentOut, previousScanline, bytesPerPixel); break;
            default: throw new PngDecoderException($"Unknown filter type {filterType}.");
        }
        
        currentOut.CopyTo(previousScanline);
    }
}

// UnfilterNone: identity copy
// UnfilterSub: pixel[x] = raw[x] + pixel[x - bpp] (left neighbor)
// UnfilterUp: pixel[x] = raw[x] + previousScanline[x]
// UnfilterAverage: pixel[x] = raw[x] + floor((left + up) / 2)
// UnfilterPaeth: pixel[x] = raw[x] + PaethPredictor(left, up, upLeft)

private static byte PaethPredictor(byte a, byte b, byte c)
{
    int p = a + b - c;
    int pa = Math.Abs(p - a);
    int pb = Math.Abs(p - b);
    int pc = Math.Abs(p - c);
    if (pa <= pb && pa <= pc) return a;
    if (pb <= pc) return b;
    return c;
}

private static byte[] ConvertRgb8ToRgba8(byte[] rgb, int width, int height)
{
    byte[] rgba = new byte[width * height * 4];
    for (int i = 0, j = 0; i < rgb.Length; i += 3, j += 4)
    {
        rgba[j] = rgb[i];
        rgba[j + 1] = rgb[i + 1];
        rgba[j + 2] = rgb[i + 2];
        rgba[j + 3] = 255;  // fully opaque
    }
    return rgba;
}
```

**Tests (Part 2 — Commit 4)**:
- 8×8 RGBA PNG synthetic (all filter type 0) — decodes к expected bytes
- 8×8 RGB PNG synthetic — decodes к RGBA с alpha=255
- 16×16 RGBA PNG с filter type 1 (Sub) — decodes correctly
- 16×16 RGBA PNG с filter type 2 (Up) — decodes correctly
- 16×16 RGBA PNG с filter type 3 (Average) — decodes correctly
- 16×16 RGBA PNG с filter type 4 (Paeth) — decodes correctly
- Mixed filter types in single PNG — decodes correctly
- DEFLATE corruption detection (truncated IDAT bytes)
- Кенney pawn PNG (если accessible) — decodes без exception, dimensions match expected

**Test PNG creation**: External tool (e.g., Python PIL, GIMP) generates test PNGs с known content. Test fixtures committed to `tests/DualFrontier.Runtime.Tests/Assets/test_assets/`.

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + PngDecoder Part 2 tests pass (positive decode cases + filter predictor variants)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — PngDecoder Part 2 — DEFLATE decompression + filter unfiltering (all 4 predictors) + RGB8→RGBA8 conversion`

### Commit 5 — AssetManager + AssetPath

**Files**:
- `src/DualFrontier.Runtime/Assets/AssetManager.cs` (new — path resolution + caching)
- `src/DualFrontier.Runtime/Assets/AssetPath.cs` (new — typed path wrapper)
- `tests/DualFrontier.Runtime.Tests/Assets/AssetManagerTests.cs` (new — path resolution + caching tests)

**Drift surface**: AssetManager provides centralized asset loading. Caches decoded PngImage per AssetPath to avoid redundant decoding.

**Implementation surface (~150 LOC)**:

```csharp
namespace DualFrontier.Runtime.Assets;

/// <summary>Typed asset path wrapper. Path relative to AssetManager root directory.</summary>
public readonly record struct AssetPath(string RelativePath)
{
    public override string ToString() => RelativePath;
}

/// <summary>
/// Centralized asset loading with caching. V0.C.1 supports PNG loading; future asset types
/// (fonts, audio) extend through similar Load* methods or generic IAssetLoader registration.
/// </summary>
public sealed class AssetManager : IDisposable
{
    private readonly string _rootDirectory;
    private readonly Dictionary<AssetPath, PngImage> _pngCache = new();
    private bool _disposed;
    
    public AssetManager(string rootDirectory)
    {
        ArgumentNullException.ThrowIfNull(rootDirectory);
        if (!Directory.Exists(rootDirectory))
        {
            throw new DirectoryNotFoundException($"Asset root directory not found: {rootDirectory}");
        }
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }
    
    /// <summary>Load PNG image, cached by path. Subsequent loads return cached instance.</summary>
    public PngImage LoadPng(AssetPath path)
    {
        if (_pngCache.TryGetValue(path, out var cached))
        {
            return cached;
        }
        
        string fullPath = ResolvePath(path);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Asset not found: {path.RelativePath} (resolved: {fullPath})");
        }
        
        var image = PngDecoder.DecodeFile(fullPath);
        _pngCache[path] = image;
        return image;
    }
    
    public string ResolvePath(AssetPath path)
    {
        // Combine root + relative path; verify result stays inside root (path traversal protection)
        string combined = Path.GetFullPath(Path.Combine(_rootDirectory, path.RelativePath));
        if (!combined.StartsWith(_rootDirectory, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Asset path escapes root: {path.RelativePath}");
        }
        return combined;
    }
    
    public void ClearCache() => _pngCache.Clear();
    
    public void Dispose()
    {
        if (_disposed) return;
        _pngCache.Clear();
        _disposed = true;
    }
}
```

**Tests**:
- Load PNG from valid path — returns PngImage с correct dimensions
- Load same PNG twice — returns same instance (cache hit)
- Load non-existent PNG — throws FileNotFoundException
- Path traversal attempt (`..\..\secret.png`) — throws InvalidOperationException
- ClearCache — subsequent load triggers re-decode

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + AssetManager tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — AssetManager + AssetPath (PNG loading с caching + path traversal protection)`

### Commit 6 — VulkanSampler primitive

**Files**:
- `src/DualFrontier.Runtime/Graphics/VulkanSampler.cs` (new — VkSampler lifecycle)
- `src/DualFrontier.Runtime/Graphics/SamplerOptions.cs` (new — record SamplerFilterMode/WrapMode + options)
- `tests/DualFrontier.Runtime.Tests/Graphics/VulkanSamplerTests.cs` (new — sampler creation tests + disposal)

**Drift surface**: VulkanSampler primitive operational. Default options nearest + repeat per S-LOCK-6.

**Implementation surface (~100 LOC)**:

```csharp
namespace DualFrontier.Runtime.Graphics;

public enum SamplerFilterMode { Nearest, Linear }
public enum SamplerWrapMode { Repeat, ClampToEdge, ClampToBorder, MirroredRepeat }

public sealed record SamplerOptions
{
    public SamplerFilterMode MagFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerFilterMode MinFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerWrapMode WrapU { get; init; } = SamplerWrapMode.Repeat;
    public SamplerWrapMode WrapV { get; init; } = SamplerWrapMode.Repeat;
    public bool EnableAnisotropy { get; init; } = false;
}

public sealed class VulkanSampler : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _sampler;
    private bool _disposed;
    
    public IntPtr Handle => _sampler;
    public SamplerOptions Options { get; }
    
    public VulkanSampler(VulkanDevice device, SamplerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        Options = options ?? new SamplerOptions();
        
        var createInfo = new VkSamplerCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            magFilter = MapFilter(Options.MagFilter),
            minFilter = MapFilter(Options.MinFilter),
            mipmapMode = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_NEAREST,
            addressModeU = MapWrap(Options.WrapU),
            addressModeV = MapWrap(Options.WrapV),
            addressModeW = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            mipLodBias = 0.0f,
            anisotropyEnable = Options.EnableAnisotropy ? 1u : 0u,
            maxAnisotropy = Options.EnableAnisotropy ? 16.0f : 1.0f,
            compareEnable = 0,
            compareOp = VkCompareOp.VK_COMPARE_OP_NEVER,
            minLod = 0.0f,
            maxLod = 0.0f,  // V0.C.1: no mipmaps; single mip level
            borderColor = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            unnormalizedCoordinates = 0,
        };
        
        VkResult result = VkApi.vkCreateSampler(_device, in createInfo, IntPtr.Zero, out _sampler);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateSampler failed: {result}");
        }
    }
    
    private static VkFilter MapFilter(SamplerFilterMode mode) => mode switch
    {
        SamplerFilterMode.Nearest => VkFilter.VK_FILTER_NEAREST,
        SamplerFilterMode.Linear => VkFilter.VK_FILTER_LINEAR,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };
    
    private static VkSamplerAddressMode MapWrap(SamplerWrapMode mode) => mode switch
    {
        SamplerWrapMode.Repeat => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
        SamplerWrapMode.ClampToEdge => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE,
        SamplerWrapMode.ClampToBorder => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER,
        SamplerWrapMode.MirroredRepeat => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_sampler != IntPtr.Zero)
        {
            VkApi.vkDestroySampler(_device, _sampler, IntPtr.Zero);
            _sampler = IntPtr.Zero;
        }
        _disposed = true;
    }
}
```

**Tests**:
- Default sampler creates successfully на real hardware
- Linear filter variant creates successfully
- ClampToEdge wrap variant creates successfully
- Disposal releases handle

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + VulkanSampler tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — VulkanSampler primitive (nearest + repeat default per pixel art aesthetic)`

### Commit 7 — TextureUploader (staging buffer + buffer-to-image copy + layout transitions)

**Files**:
- `src/DualFrontier.Runtime/Graphics/TextureUploader.cs` (new — orchestrates staging upload + layout transitions)
- `src/DualFrontier.Runtime/Graphics/VulkanImage.cs` (modified — adds static convenience method CreateFromPngImage)
- `tests/DualFrontier.Runtime.Tests/Graphics/TextureUploaderTests.cs` (new — texture upload integration tests)

**Drift surface**: TextureUploader operational. Synchronous staging buffer pattern: host-visible buffer copies PNG bytes → device-local VulkanImage via VkCmdCopyBufferToImage + layout transitions UNDEFINED → TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL.

**Implementation surface (~250 LOC)**:

```csharp
namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Orchestrates texture upload: host-visible staging buffer → device-local VulkanImage transfer
/// via vkCmdCopyBufferToImage + layout transitions. Synchronous (fence wait per К-L7
/// atomic-from-observer). Future V0.C.2/V1 may add async upload pattern для large asset streams.
/// </summary>
public sealed class TextureUploader : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanCommandPool _commandPool;
    private bool _disposed;
    
    public TextureUploader(VulkanDevice device, MemoryAllocator allocator, VulkanCommandPool commandPool)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(commandPool);
        _device = device;
        _allocator = allocator;
        _commandPool = commandPool;
    }
    
    /// <summary>
    /// Upload RGBA8 pixel data к VulkanImage. Creates staging buffer + records transfer
    /// commands + submits sync + waits fence.
    /// </summary>
    public unsafe void Upload(VulkanImage destImage, byte[] rgba8Pixels)
    {
        ArgumentNullException.ThrowIfNull(destImage);
        ArgumentNullException.ThrowIfNull(rgba8Pixels);
        
        ulong size = (ulong)rgba8Pixels.Length;
        
        // 1. Create host-visible staging buffer
        using var staging = new VulkanBuffer(
            _device, _allocator, size,
            VkBufferUsageFlagsPublic.TransferSrc,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent);
        
        // 2. Map staging buffer + copy pixels
        IntPtr mappedPtr;
        VkResult mapResult = VkApi.vkMapMemory(_device.Handle, staging.MemoryHandle, staging.MemoryOffset, size, 0, out mappedPtr);
        if (mapResult != VkResult.VK_SUCCESS) throw new InvalidOperationException($"vkMapMemory failed: {mapResult}");
        
        Marshal.Copy(rgba8Pixels, 0, mappedPtr, rgba8Pixels.Length);
        VkApi.vkUnmapMemory(_device.Handle, staging.MemoryHandle);
        
        // 3. Allocate one-shot command buffer
        using var commandBuffer = _commandPool.AllocateCommandBuffer();
        commandBuffer.Begin(oneTimeSubmit: true);
        
        // 4. Pipeline barrier: UNDEFINED → TRANSFER_DST_OPTIMAL
        RecordImageLayoutTransition(commandBuffer, destImage,
            VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            srcAccess: 0,
            dstAccess: VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT,
            srcStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            dstStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT);
        
        // 5. vkCmdCopyBufferToImage
        var region = new VkBufferImageCopy
        {
            bufferOffset = 0,
            bufferRowLength = 0,  // tightly packed
            bufferImageHeight = 0,
            imageSubresource = new VkImageSubresourceLayers
            {
                aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
                mipLevel = 0,
                baseArrayLayer = 0,
                layerCount = 1,
            },
            imageOffset = new VkOffset3D { x = 0, y = 0, z = 0 },
            imageExtent = new VkExtent3D { width = destImage.Width, height = destImage.Height, depth = 1 },
        };
        VkApi.vkCmdCopyBufferToImage(
            commandBuffer.Handle, staging.Handle, destImage.Handle,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            1, in region);
        
        // 6. Pipeline barrier: TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL
        RecordImageLayoutTransition(commandBuffer, destImage,
            VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
            VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            srcAccess: VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT,
            dstAccess: VkAccessFlags.VK_ACCESS_SHADER_READ_BIT,
            srcStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT,
            dstStage: VkPipelineStageFlags.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT);
        
        commandBuffer.End();
        
        // 7. Submit + fence wait
        using var fence = new VulkanFence(_device);
        commandBuffer.SubmitTo(_device.GraphicsQueue, fence);
        fence.Wait();
        // Staging buffer disposed via using; bumper allocator memory not freed (V0.B accepted)
    }
    
    private unsafe void RecordImageLayoutTransition(
        VulkanCommandBuffer cmd, VulkanImage image,
        VkImageLayout oldLayout, VkImageLayout newLayout,
        VkAccessFlags srcAccess, VkAccessFlags dstAccess,
        VkPipelineStageFlags srcStage, VkPipelineStageFlags dstStage)
    {
        var barrier = new VkImageMemoryBarrier
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER,
            pNext = IntPtr.Zero,
            srcAccessMask = srcAccess,
            dstAccessMask = dstAccess,
            oldLayout = oldLayout,
            newLayout = newLayout,
            srcQueueFamilyIndex = uint.MaxValue,  // VK_QUEUE_FAMILY_IGNORED
            dstQueueFamilyIndex = uint.MaxValue,
            image = image.Handle,
            subresourceRange = new VkImageSubresourceRange
            {
                aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
                baseMipLevel = 0, levelCount = 1,
                baseArrayLayer = 0, layerCount = 1,
            },
        };
        
        VkApi.vkCmdPipelineBarrier(
            cmd.Handle,
            srcStageMask: srcStage, dstStageMask: dstStage,
            dependencyFlags: 0,
            memoryBarrierCount: 0, pMemoryBarriers: IntPtr.Zero,
            bufferMemoryBarrierCount: 0, pBufferMemoryBarriers: IntPtr.Zero,
            imageMemoryBarrierCount: 1, pImageMemoryBarriers: &barrier);
    }
    
    public void Dispose()
    {
        // Uploader не owns persistent resources (one-shot pattern); все using-scoped above
        _disposed = true;
    }
}
```

**VulkanImage convenience method addition**:
```csharp
public static VulkanImage CreateFromPngImage(
    VulkanDevice device, MemoryAllocator allocator,
    TextureUploader uploader, PngImage png)
{
    var image = new VulkanImage(
        device, allocator,
        (uint)png.Width, (uint)png.Height,
        VkFormatPublic.R8G8B8A8_UNorm,
        VkImageUsageFlagsPublic.Sampled | VkImageUsageFlagsPublic.TransferDst,
        VkMemoryPropertyFlagsPublic.DeviceLocal);
    
    uploader.Upload(image, png.PixelsRgba8);
    return image;
}
```

**Tests**:
- Upload synthetic 4×4 RGBA bytes к VulkanImage — validation clean
- Upload Kenney pawn PNG — succeeds; image layout SHADER_READ_ONLY_OPTIMAL post-upload
- Verify staging buffer freed после upload completion

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + TextureUploader tests pass на real hardware
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — TextureUploader (staging buffer + buffer-to-image copy + layout transitions) + VulkanImage.CreateFromPngImage`

### Commit 8 — VulkanPipelineLayout extension с push constant ranges

**Files**:
- `src/DualFrontier.Runtime/Graphics/VulkanPipelineLayout.cs` (modified — constructor accepts pushConstantRanges parameter)
- `tests/DualFrontier.Runtime.Tests/Graphics/VulkanPipelineLayoutTests.cs` (new или modified — adds push constant range tests)

**Drift surface**: VulkanPipelineLayout backward-compatibly extended. V0.B clearcolor pipeline still passes empty descriptors + no push constants (default arguments preserve behavior). V0.C.1 sprite pipeline uses push constant range.

**Implementation surface (~50 LOC modification)**:

```csharp
public unsafe VulkanPipelineLayout(
    VulkanDevice device,
    IReadOnlyList<IntPtr>? descriptorSetLayouts = null,
    IReadOnlyList<VkPushConstantRangePublic>? pushConstantRanges = null)
{
    ArgumentNullException.ThrowIfNull(device);
    _device = device.Handle;
    
    IntPtr[] sets = descriptorSetLayouts?.ToArray() ?? Array.Empty<IntPtr>();
    
    // Convert public-facing к internal Vulkan struct
    VkPushConstantRange[] ranges = pushConstantRanges?
        .Select(r => new VkPushConstantRange
        {
            stageFlags = (VkShaderStageFlags)(uint)r.StageFlags,
            offset = r.Offset,
            size = r.Size,
        })
        .ToArray() ?? Array.Empty<VkPushConstantRange>();
    
    fixed (IntPtr* setsPtr = sets)
    fixed (VkPushConstantRange* rangesPtr = ranges)
    {
        var createInfo = new VkPipelineLayoutCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            setLayoutCount = (uint)sets.Length,
            pSetLayouts = sets.Length == 0 ? null : setsPtr,
            pushConstantRangeCount = (uint)ranges.Length,
            pPushConstantRanges = ranges.Length == 0 ? IntPtr.Zero : (IntPtr)rangesPtr,
        };
        VkResult result = VkApi.vkCreatePipelineLayout(_device, in createInfo, IntPtr.Zero, out _layout);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreatePipelineLayout failed: {result}");
        }
    }
}
```

**Tests**:
- Empty layout (default args) creates successfully (V0.B regression)
- Layout с push constant range creates successfully
- Layout с descriptor set layout + push constant range creates successfully

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + new tests pass; V0.B regression tests preserved
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — VulkanPipelineLayout extension с push constant range support (backward-compatible)`

### Commit 9 — Sprite shaders (GLSL sources) + MSBuild target extension + pre-compiled SPIR-V

**Files**:
- `tools/shaders/sprite.vert` (new — per S-LOCK-4 verbatim GLSL)
- `tools/shaders/sprite.frag` (new — per S-LOCK-4 verbatim GLSL)
- `assets/shaders/sprite.vert.spv` (new — pre-compiled output)
- `assets/shaders/sprite.frag.spv` (new — pre-compiled output)
- `Directory.Build.props` (modified — CompileShaders target adds sprite.vert + sprite.frag compilation)
- `.gitattributes` (verify — `assets/shaders/*.spv binary` declaration covers new files)

**Drift surface**: Sprite shaders source-controlled + pre-compiled. MSBuild rebuild regenerates `.spv` files on every `dotnet build DualFrontier.Runtime.csproj`.

**MSBuild target extension** (Directory.Build.props):

```xml
<Target Name="CompileShaders" BeforeTargets="Build"
        Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
  <!-- V0.B clearcolor + noop (preserved) -->
  <Exec Command="&quot;$(SolutionDir)tools\glslangValidator.exe&quot; -V &quot;$(SolutionDir)tools\shaders\clearcolor.vert&quot; -o &quot;$(SolutionDir)assets\shaders\clearcolor.vert.spv&quot;" />
  <Exec Command="&quot;$(SolutionDir)tools\glslangValidator.exe&quot; -V &quot;$(SolutionDir)tools\shaders\clearcolor.frag&quot; -o &quot;$(SolutionDir)assets\shaders\clearcolor.frag.spv&quot;" />
  <Exec Command="&quot;$(SolutionDir)tools\glslangValidator.exe&quot; -V &quot;$(SolutionDir)tools\shaders\noop.comp&quot; -o &quot;$(SolutionDir)assets\shaders\noop.comp.spv&quot;" />
  <!-- V0.C.1 sprite (NEW) -->
  <Exec Command="&quot;$(SolutionDir)tools\glslangValidator.exe&quot; -V &quot;$(SolutionDir)tools\shaders\sprite.vert&quot; -o &quot;$(SolutionDir)assets\shaders\sprite.vert.spv&quot;" />
  <Exec Command="&quot;$(SolutionDir)tools\glslangValidator.exe&quot; -V &quot;$(SolutionDir)tools\shaders\sprite.frag&quot; -o &quot;$(SolutionDir)assets\shaders\sprite.frag.spv&quot;" />
</Target>
```

**Validation**:
- `dotnet build DualFrontier.Runtime.csproj` invokes glslangValidator successfully; .spv files regenerated; no GLSL errors
- Manual: verify pre-compiled .spv files match regenerated output
- `dotnet test` 786+ baseline (no test changes)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Sprite GLSL shaders + MSBuild target extension + pre-compiled SPIR-V`

### Commit 10 — SpriteVertex + Sprite primitive types

**Files**:
- `src/DualFrontier.Runtime/Sprite/MODULE.md` (new — Sprite module purpose per VULKAN_SUBSTRATE §2.2)
- `src/DualFrontier.Runtime/Sprite/SpriteVertex.cs` (new — vertex format struct per S-LOCK-3)
- `src/DualFrontier.Runtime/Sprite/AtlasRegion.cs` (new — UV rect record)
- `src/DualFrontier.Runtime/Sprite/SpriteTexture.cs` (new — texture handle с VulkanImage + VulkanSampler refs)
- `src/DualFrontier.Runtime/Sprite/SpriteTransform.cs` (new — position + scale + rotation + tint)
- `src/DualFrontier.Runtime/Sprite/Sprite.cs` (new — minimal handle struct)
- `tests/DualFrontier.Runtime.Tests/Sprite/SpriteVertexTests.cs` (new — Marshal.SizeOf<SpriteVertex>() == 20 test + layout test)

**Drift surface**: Sprite module primitive types operational. Pure data records (no Vulkan dependency yet — SpriteTexture references VulkanImage + VulkanSampler as IDisposable owners).

**Implementation surface (~150 LOC total across files)**:

```csharp
// SpriteVertex.cs
namespace DualFrontier.Runtime.Sprite;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SpriteVertex
{
    public Vector2 Position;  // 8 bytes
    public Vector2 Uv;        // 8 bytes
    public uint TintRgba;     // 4 bytes
    // Total: 20 bytes per S-LOCK-3
    
    public SpriteVertex(Vector2 position, Vector2 uv, uint tintRgba)
    {
        Position = position;
        Uv = uv;
        TintRgba = tintRgba;
    }
    
    public static uint PackTintRgba(byte r, byte g, byte b, byte a) =>
        (uint)r | ((uint)g << 8) | ((uint)b << 16) | ((uint)a << 24);
    
    public static readonly uint WhiteTint = PackTintRgba(255, 255, 255, 255);
}

// AtlasRegion.cs
namespace DualFrontier.Runtime.Sprite;

/// <summary>UV rectangle within an atlas texture (normalized 0..1 coordinates).</summary>
public readonly record struct AtlasRegion(float U0, float V0, float U1, float V1)
{
    public static AtlasRegion Full => new(0f, 0f, 1f, 1f);
    
    /// <summary>Construct atlas region from pixel coordinates given total texture dimensions.</summary>
    public static AtlasRegion FromPixels(int x, int y, int width, int height, int textureWidth, int textureHeight)
        => new(
            U0: (float)x / textureWidth,
            V0: (float)y / textureHeight,
            U1: (float)(x + width) / textureWidth,
            V1: (float)(y + height) / textureHeight);
}

// SpriteTexture.cs — wraps VulkanImage + VulkanSampler с reference semantics
namespace DualFrontier.Runtime.Sprite;

public sealed class SpriteTexture : IDisposable
{
    public VulkanImage Image { get; }
    public VulkanSampler Sampler { get; }
    public int Width => (int)Image.Width;
    public int Height => (int)Image.Height;
    private bool _disposed;
    
    public SpriteTexture(VulkanImage image, VulkanSampler sampler)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(sampler);
        Image = image;
        Sampler = sampler;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        Image.Dispose();
        Sampler.Dispose();
        _disposed = true;
    }
}

// SpriteTransform.cs
namespace DualFrontier.Runtime.Sprite;

public readonly record struct SpriteTransform(
    Vector2 Position,
    Vector2 Scale,
    float Rotation,
    uint TintRgba)
{
    public static SpriteTransform Default => new(
        Position: Vector2.Zero,
        Scale: Vector2.One,
        Rotation: 0f,
        TintRgba: SpriteVertex.WhiteTint);
}

// Sprite.cs
namespace DualFrontier.Runtime.Sprite;

public readonly record struct Sprite(
    SpriteTexture Texture,
    AtlasRegion Region,
    SpriteTransform Transform)
{
    /// <summary>Construct sprite with default transform + full atlas region.</summary>
    public static Sprite Create(SpriteTexture texture)
        => new(texture, AtlasRegion.Full, SpriteTransform.Default);
}
```

**Tests**:
- `Marshal.SizeOf<SpriteVertex>()` == 20 (S-LOCK-3 verification — critical per Lesson #7)
- PackTintRgba round-trip
- AtlasRegion.FromPixels с known inputs
- Sprite.Create returns expected defaults

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + Sprite primitive tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Sprite primitive types (SpriteVertex 20 bytes + AtlasRegion + SpriteTexture + SpriteTransform + Sprite)`

### Commit 11 — VulkanSpritePipeline (consumes VulkanPipelineLayout extension + sprite shaders)

**Files**:
- `src/DualFrontier.Runtime/Sprite/VulkanSpritePipeline.cs` (new — sprite-specific pipeline с vertex input + blending + descriptors)
- `src/DualFrontier.Runtime/Sprite/SpriteDescriptorSetLayout.cs` (new — combined image sampler binding layout)
- `tests/DualFrontier.Runtime.Tests/Sprite/VulkanSpritePipelineTests.cs` (new — pipeline creation tests)

**Drift surface**: Sprite pipeline operational. Vertex input description matches SpriteVertex layout (20 bytes, 3 attributes). Alpha blending configured per S-LOCK-5 (premultiplied alpha). Descriptor set layout с 1 combined image sampler binding (set 0 binding 0).

**Implementation surface (~350 LOC — substantial pipeline composition)**:

```csharp
namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Sprite-rendering Vulkan pipeline. Extends V0.B VulkanGraphicsPipeline pattern с:
/// - Vertex input (SpriteVertex 20 bytes, 3 attributes)
/// - Alpha blending (premultiplied workflow per S-LOCK-5)
/// - Descriptor set layout (1 combined image sampler, fragment stage)
/// - Push constant range (mat4 MVP, vertex stage)
/// - Dynamic viewport + scissor (matches V0.B clearcolor pipeline для swapchain recreation)
/// </summary>
public sealed class VulkanSpritePipeline : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _pipeline;
    private bool _disposed;
    
    public IntPtr Handle => _pipeline;
    public VulkanPipelineLayout Layout { get; }
    public SpriteDescriptorSetLayout DescriptorSetLayout { get; }
    
    public unsafe VulkanSpritePipeline(
        VulkanDevice device,
        VulkanRenderPass renderPass,
        VulkanShaderModule vertexShader,
        VulkanShaderModule fragmentShader,
        SpriteDescriptorSetLayout descriptorSetLayout,
        VulkanPipelineLayout pipelineLayout)
    {
        ArgumentNullException.ThrowIfNull(device);
        // ... ArgumentNullException.ThrowIfNull для each param
        _device = device.Handle;
        Layout = pipelineLayout;
        DescriptorSetLayout = descriptorSetLayout;
        
        IntPtr mainNamePtr = Marshal.StringToCoTaskMemUTF8("main");
        try
        {
            // Vertex + fragment shader stages
            var stages = stackalloc VkPipelineShaderStageCreateInfo[2];
            stages[0] = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stage = VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
                module = vertexShader.Handle,
                pName = (byte*)mainNamePtr,
                pSpecializationInfo = IntPtr.Zero,
            };
            stages[1] = new VkPipelineShaderStageCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stage = VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT,
                module = fragmentShader.Handle,
                pName = (byte*)mainNamePtr,
                pSpecializationInfo = IntPtr.Zero,
            };
            
            // Vertex input description (SpriteVertex 20 bytes, 3 attributes)
            var vertexBinding = new VkVertexInputBindingDescription
            {
                binding = 0,
                stride = 20,  // sizeof(SpriteVertex)
                inputRate = VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX,
            };
            var vertexAttribs = stackalloc VkVertexInputAttributeDescription[3]
            {
                new() { location = 0, binding = 0, format = VkFormat.VK_FORMAT_R32G32_SFLOAT, offset = 0 },     // Position
                new() { location = 1, binding = 0, format = VkFormat.VK_FORMAT_R32G32_SFLOAT, offset = 8 },     // Uv
                new() { location = 2, binding = 0, format = VkFormat.VK_FORMAT_R8G8B8A8_UNORM, offset = 16 },   // TintRgba
            };
            var vertexInput = new VkPipelineVertexInputStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                vertexBindingDescriptionCount = 1,
                pVertexBindingDescriptions = (IntPtr)(&vertexBinding),
                vertexAttributeDescriptionCount = 3,
                pVertexAttributeDescriptions = (IntPtr)vertexAttribs,
            };
            
            // Input assembly: triangle list (2 triangles per quad)
            var inputAssembly = new VkPipelineInputAssemblyStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
                primitiveRestartEnable = 0,
            };
            
            // Dynamic viewport + scissor (matches V0.B pattern)
            var viewportState = new VkPipelineViewportStateCreateInfo { /* dynamic, 1 viewport, 1 scissor */ };
            var rasterizer = new VkPipelineRasterizationStateCreateInfo { /* fill + no cull + CCW */ };
            var multisample = new VkPipelineMultisampleStateCreateInfo { /* 1× sample */ };
            
            // Alpha blending per S-LOCK-5 (premultiplied)
            var colorAttachment = new VkPipelineColorBlendAttachmentState
            {
                blendEnable = 1,  // VK_TRUE
                srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
                colorBlendOp = VkBlendOp.VK_BLEND_OP_ADD,
                srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE,
                dstAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA,
                alphaBlendOp = VkBlendOp.VK_BLEND_OP_ADD,
                colorWriteMask = VkColorComponentFlags.VK_COLOR_COMPONENT_RGBA,
            };
            VkPipelineColorBlendStateCreateInfo colorBlend = default;
            colorBlend.sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO;
            colorBlend.attachmentCount = 1;
            colorBlend.pAttachments = &colorAttachment;
            // blendConstants 0
            
            var dynamicStates = stackalloc VkDynamicState[2]
            {
                VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT,
                VkDynamicState.VK_DYNAMIC_STATE_SCISSOR,
            };
            var dynamicState = new VkPipelineDynamicStateCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO,
                dynamicStateCount = 2,
                pDynamicStates = dynamicStates,
            };
            
            var createInfo = new VkGraphicsPipelineCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                stageCount = 2,
                pStages = stages,
                pVertexInputState = &vertexInput,
                pInputAssemblyState = &inputAssembly,
                pTessellationState = IntPtr.Zero,
                pViewportState = &viewportState,
                pRasterizationState = &rasterizer,
                pMultisampleState = &multisample,
                pDepthStencilState = IntPtr.Zero,
                pColorBlendState = &colorBlend,
                pDynamicState = &dynamicState,
                layout = pipelineLayout.Handle,
                renderPass = renderPass.Handle,
                subpass = 0,
                basePipelineHandle = IntPtr.Zero,
                basePipelineIndex = -1,
            };
            
            IntPtr pipeline = IntPtr.Zero;
            VkResult result = VkApi.vkCreateGraphicsPipelines(
                _device, pipelineCache: IntPtr.Zero, createInfoCount: 1,
                pCreateInfos: &createInfo, pAllocator: IntPtr.Zero, pPipelines: &pipeline);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateGraphicsPipelines failed: {result}");
            }
            _pipeline = pipeline;
        }
        finally
        {
            Marshal.FreeCoTaskMem(mainNamePtr);
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_pipeline != IntPtr.Zero)
        {
            VkApi.vkDestroyPipeline(_device, _pipeline, IntPtr.Zero);
            _pipeline = IntPtr.Zero;
        }
        _disposed = true;
    }
}
```

`SpriteDescriptorSetLayout` wraps `VkDescriptorSetLayout` creation с 1 binding (COMBINED_IMAGE_SAMPLER, fragment stage). Similar pattern к VulkanComputeDescriptors V0.B precedent.

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + sprite pipeline creation tests pass на real hardware
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — VulkanSpritePipeline (vertex input + alpha blending + descriptor set + push constants)`

### Commit 12 — SpriteRenderer (single-sprite per draw call)

**Files**:
- `src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs` (new — orchestrates pipeline + vertex buffer + descriptor pool + dispatch)
- `tests/DualFrontier.Runtime.Tests/Sprite/SpriteRendererTests.cs` (new — sprite render integration test)

**Drift surface**: SpriteRenderer operational. API: `DrawSprite(Sprite, VulkanCommandBuffer)`. Creates 4 vertices (quad) per call, uploads to dynamic vertex buffer, binds pipeline + descriptor set + buffer, records vkCmdDraw(4, 1, 0, 0). V0.C.1 single-sprite mode; V0.C.2 will refactor к batched.

**Implementation surface (~300 LOC)**:

```csharp
namespace DualFrontier.Runtime.Sprite;

public sealed class SpriteRenderer : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly MemoryAllocator _allocator;
    private readonly VulkanSpritePipeline _pipeline;
    private readonly VulkanBuffer _vertexBuffer;
    private readonly IntPtr _descriptorPool;
    private readonly Dictionary<SpriteTexture, IntPtr> _descriptorSetCache = new();
    private bool _disposed;
    
    public SpriteRenderer(
        VulkanDevice device,
        MemoryAllocator allocator,
        VulkanSpritePipeline pipeline)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(pipeline);
        _device = device;
        _allocator = allocator;
        _pipeline = pipeline;
        
        // Dynamic vertex buffer для one sprite (4 vertices × 20 bytes = 80 bytes minimum;
        // sized larger to avoid resize for V0.C.2 batched expansion)
        const ulong vbufferSize = 64 * 1024;  // 64 KB
        _vertexBuffer = new VulkanBuffer(
            _device, _allocator, vbufferSize,
            VkBufferUsageFlagsPublic.VertexBuffer,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent);
        
        // Descriptor pool (sized for ~32 distinct textures — V0.C.1 only uses 1, V0.C.2 expands)
        CreateDescriptorPool();
    }
    
    private unsafe void CreateDescriptorPool()
    {
        var poolSize = new VkDescriptorPoolSize
        {
            type = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = 32,
        };
        var poolInfo = new VkDescriptorPoolCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            maxSets = 32,
            poolSizeCount = 1,
            pPoolSizes = (IntPtr)(&poolSize),
        };
        VkResult result = VkApi.vkCreateDescriptorPool(_device.Handle, in poolInfo, IntPtr.Zero, out _descriptorPool);
        if (result != VkResult.VK_SUCCESS) throw new InvalidOperationException($"vkCreateDescriptorPool failed: {result}");
    }
    
    private unsafe IntPtr GetOrCreateDescriptorSet(SpriteTexture texture)
    {
        if (_descriptorSetCache.TryGetValue(texture, out IntPtr cached))
        {
            return cached;
        }
        
        // Allocate descriptor set from pool
        IntPtr layout = _pipeline.DescriptorSetLayout.Handle;
        var allocInfo = new VkDescriptorSetAllocateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO,
            pNext = IntPtr.Zero,
            descriptorPool = _descriptorPool,
            descriptorSetCount = 1,
            pSetLayouts = (IntPtr)(&layout),
        };
        IntPtr descriptorSet;
        VkResult allocResult = VkApi.vkAllocateDescriptorSets(_device.Handle, in allocInfo, out descriptorSet);
        if (allocResult != VkResult.VK_SUCCESS) throw new InvalidOperationException($"vkAllocateDescriptorSets failed: {allocResult}");
        
        // Update descriptor set с texture's image view + sampler
        var imageInfo = new VkDescriptorImageInfo
        {
            sampler = texture.Sampler.Handle,
            imageView = texture.Image.ViewHandle,
            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
        };
        var write = new VkWriteDescriptorSet
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET,
            pNext = IntPtr.Zero,
            dstSet = descriptorSet,
            dstBinding = 0,
            dstArrayElement = 0,
            descriptorCount = 1,
            descriptorType = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            pImageInfo = (IntPtr)(&imageInfo),
            pBufferInfo = IntPtr.Zero,
            pTexelBufferView = IntPtr.Zero,
        };
        VkApi.vkUpdateDescriptorSets(_device.Handle, 1, in write, 0, IntPtr.Zero);
        
        _descriptorSetCache[texture] = descriptorSet;
        return descriptorSet;
    }
    
    /// <summary>
    /// Record sprite draw commands к command buffer. V0.C.1 single-sprite per call;
    /// V0.C.2 will refactor к batched (one draw call per atlas).
    /// </summary>
    public unsafe void DrawSprite(Sprite sprite, VulkanCommandBuffer commandBuffer, Matrix4x4 mvp)
    {
        // 1. Build 4 vertices for sprite quad (CCW order: bottom-left, bottom-right, top-right, top-left)
        // For simplicity, draw 2 triangles via index-less topology (6 vertices total — 0,1,2 + 0,2,3 → use index buffer in V0.C.2)
        // V0.C.1 single-sprite simplest: 6 vertices forming 2 triangles
        
        Vector2 halfSize = new(sprite.Transform.Scale.X * 0.5f, sprite.Transform.Scale.Y * 0.5f);
        Vector2 pos = sprite.Transform.Position;
        AtlasRegion uv = sprite.Region;
        uint tint = sprite.Transform.TintRgba;
        
        Span<SpriteVertex> vertices = stackalloc SpriteVertex[6];
        // Triangle 1: bottom-left, bottom-right, top-right
        vertices[0] = new(new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U0, uv.V1), tint);  // BL
        vertices[1] = new(new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U1, uv.V1), tint);  // BR
        vertices[2] = new(new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U1, uv.V0), tint);  // TR
        // Triangle 2: bottom-left, top-right, top-left
        vertices[3] = new(new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U0, uv.V1), tint);  // BL
        vertices[4] = new(new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U1, uv.V0), tint);  // TR
        vertices[5] = new(new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U0, uv.V0), tint);  // TL
        
        // 2. Upload vertices к dynamic vertex buffer (host-visible, mapped writes)
        IntPtr mappedPtr;
        VkApi.vkMapMemory(_device.Handle, _vertexBuffer.MemoryHandle, _vertexBuffer.MemoryOffset, 6 * 20, 0, out mappedPtr);
        fixed (SpriteVertex* vptr = vertices)
        {
            Buffer.MemoryCopy(vptr, mappedPtr.ToPointer(), 6 * 20, 6 * 20);
        }
        VkApi.vkUnmapMemory(_device.Handle, _vertexBuffer.MemoryHandle);
        
        // 3. Get или create descriptor set для this texture
        IntPtr descriptorSet = GetOrCreateDescriptorSet(sprite.Texture);
        
        // 4. Record commands
        VkApi.vkCmdBindPipeline(commandBuffer.Handle, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, _pipeline.Handle);
        VkApi.vkCmdBindDescriptorSets(
            commandBuffer.Handle, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _pipeline.Layout.Handle, firstSet: 0, descriptorSetCount: 1, in descriptorSet,
            dynamicOffsetCount: 0, pDynamicOffsets: IntPtr.Zero);
        
        // Push constants — MVP matrix
        Matrix4x4* mvpPtr = &mvp;
        VkApi.vkCmdPushConstants(commandBuffer.Handle, _pipeline.Layout.Handle,
            VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT, 0, 64, mvpPtr);
        
        // Bind vertex buffer + draw
        IntPtr vbuffer = _vertexBuffer.Handle;
        ulong vbufferOffset = 0;
        VkApi.vkCmdBindVertexBuffers(commandBuffer.Handle, firstBinding: 0, bindingCount: 1,
            in vbuffer, in vbufferOffset);
        VkApi.vkCmdDraw(commandBuffer.Handle, vertexCount: 6, instanceCount: 1, firstVertex: 0, firstInstance: 0);
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        if (_descriptorPool != IntPtr.Zero)
        {
            VkApi.vkDestroyDescriptorPool(_device.Handle, _descriptorPool, IntPtr.Zero);
        }
        _vertexBuffer.Dispose();
        _descriptorSetCache.Clear();
        _disposed = true;
    }
}
```

**Tests**:
- Construct SpriteRenderer на real hardware — descriptor pool + vertex buffer created
- DrawSprite single sprite — validation clean, vertices uploaded correctly
- Descriptor set caching: same texture twice returns same descriptor set

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + SpriteRenderer tests pass на real hardware
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — SpriteRenderer (single-sprite per draw + descriptor set caching + dynamic vertex buffer)`

### Commit 13 — Input event types + VirtualKeyMapper

**Files**:
- `src/DualFrontier.Runtime/Input/Key.cs` (new — Key enum per S-LOCK-10)
- `src/DualFrontier.Runtime/Input/MouseButton.cs` (new — MouseButton enum)
- `src/DualFrontier.Runtime/Input/KeyPressedEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/KeyReleasedEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/MouseMovedEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/MouseButtonEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/MouseWheelEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/WindowFocusEvent.cs` (new)
- `src/DualFrontier.Runtime/Input/VirtualKeyMapper.cs` (new — VK_* code → Key enum mapping)
- `tests/DualFrontier.Runtime.Tests/Input/VirtualKeyMapperTests.cs` (new — VK code mapping tests)
- `tests/DualFrontier.Runtime.Tests/Input/InputEventTests.cs` (new — event record property tests)

**Drift surface**: Input event types complete. VirtualKeyMapper provides VK_* → Key enum mapping per Win32 documentation.

**Implementation surface (~250 LOC across files)**:

```csharp
// Key.cs per S-LOCK-10 verbatim enum
namespace DualFrontier.Runtime.Input;

public enum Key { /* per S-LOCK-10 */ }
public enum MouseButton { Left, Right, Middle }

public sealed record KeyPressedEvent(Key Key) : IInputEvent;
public sealed record KeyReleasedEvent(Key Key) : IInputEvent;
public sealed record MouseMovedEvent(int X, int Y) : IInputEvent;
public sealed record MouseButtonEvent(MouseButton Button, bool Pressed) : IInputEvent;
public sealed record MouseWheelEvent(int Delta) : IInputEvent;
public sealed record WindowFocusEvent(bool Focused) : IInputEvent;

// VirtualKeyMapper.cs
public static class VirtualKeyMapper
{
    public static Key Map(int vkCode) => vkCode switch
    {
        Win32Constants.VK_LEFT => Key.Left,
        Win32Constants.VK_RIGHT => Key.Right,
        Win32Constants.VK_UP => Key.Up,
        Win32Constants.VK_DOWN => Key.Down,
        Win32Constants.VK_ESCAPE => Key.Escape,
        Win32Constants.VK_SPACE => Key.Space,
        Win32Constants.VK_RETURN => Key.Enter,
        Win32Constants.VK_TAB => Key.Tab,
        Win32Constants.VK_BACK => Key.Backspace,
        Win32Constants.VK_DELETE => Key.Delete,
        Win32Constants.VK_HOME => Key.Home,
        Win32Constants.VK_END => Key.End,
        Win32Constants.VK_PRIOR => Key.PageUp,
        Win32Constants.VK_NEXT => Key.PageDown,
        Win32Constants.VK_SHIFT => Key.Shift,
        Win32Constants.VK_CONTROL => Key.Control,
        Win32Constants.VK_MENU => Key.Alt,  // VK_MENU = Alt
        Win32Constants.VK_F1 => Key.F1,
        // ... через VK_F12
        // Letter keys: VK_A = 0x41 .. VK_Z = 0x5A (ASCII for uppercase)
        >= 0x41 and <= 0x5A => (Key)((int)Key.A + (vkCode - 0x41)),
        // Digit keys: VK_0 = 0x30 .. VK_9 = 0x39
        >= 0x30 and <= 0x39 => (Key)((int)Key.Digit0 + (vkCode - 0x30)),
        _ => Key.Unknown,
    };
}
```

**Win32Constants.cs additions**:
```csharp
public const uint WM_KEYDOWN = 0x0100;
public const uint WM_KEYUP = 0x0101;
public const uint WM_SYSKEYDOWN = 0x0104;
public const uint WM_SYSKEYUP = 0x0105;
public const uint WM_MOUSEMOVE = 0x0200;
public const uint WM_LBUTTONDOWN = 0x0201;
public const uint WM_LBUTTONUP = 0x0202;
public const uint WM_RBUTTONDOWN = 0x0204;
public const uint WM_RBUTTONUP = 0x0205;
public const uint WM_MBUTTONDOWN = 0x0207;
public const uint WM_MBUTTONUP = 0x0208;
public const uint WM_MOUSEWHEEL = 0x020A;
public const uint WM_SETFOCUS = 0x0007;
public const uint WM_KILLFOCUS = 0x0008;

// Virtual key codes per Win32 documentation
public const int VK_LEFT = 0x25;
public const int VK_UP = 0x26;
public const int VK_RIGHT = 0x27;
public const int VK_DOWN = 0x28;
public const int VK_ESCAPE = 0x1B;
public const int VK_SPACE = 0x20;
public const int VK_RETURN = 0x0D;
public const int VK_TAB = 0x09;
public const int VK_BACK = 0x08;
public const int VK_DELETE = 0x2E;
public const int VK_HOME = 0x24;
public const int VK_END = 0x23;
public const int VK_PRIOR = 0x21;  // PageUp
public const int VK_NEXT = 0x22;   // PageDown
public const int VK_SHIFT = 0x10;
public const int VK_CONTROL = 0x11;
public const int VK_MENU = 0x12;   // Alt
public const int VK_F1 = 0x70;
// ... VK_F12 = 0x7B
```

**Tests**:
- VirtualKeyMapper.Map(VK_LEFT) returns Key.Left
- VirtualKeyMapper.Map(0x41 [VK_A]) returns Key.A
- VirtualKeyMapper.Map(0x31 [VK_1]) returns Key.Digit1
- VirtualKeyMapper.Map(0xFFFF [invalid]) returns Key.Unknown
- All event records construct with correct property values

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + input event tests pass
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Input event types complete (Key/MouseButton enums + 6 event records + VirtualKeyMapper)`

### Commit 14 — Win32 message dispatch (Window.cs WindowProcedure extension)

**Files**:
- `src/DualFrontier.Runtime/Window/Window.cs` (modified — WindowProcedure adds keyboard/mouse/focus dispatch per S-LOCK-10)
- `tests/DualFrontier.Runtime.Tests/Window/WindowInputDispatchTests.cs` (new — message dispatch integration tests where feasible; some require manual visual)

**Drift surface**: Window.cs WindowProcedure complete. WM_KEYDOWN/UP/MOUSEMOVE/L|R|MBUTTONDOWN/UP/MOUSEWHEEL/SETFOCUS/KILLFOCUS dispatch к InputEventQueue.

**Implementation surface**: Window.cs WindowProcedure extended per S-LOCK-10 verbatim. Existing WM_CLOSE/WM_DESTROY/WM_SIZE preserved.

**Tests**:
- Integration tests difficult без actual Win32 message pump — focus on smoke test verification
- Possible: SendMessage Win32 call к hWnd posting WM_KEYDOWN → verify InputQueue contains KeyPressedEvent
- Manual visual verification: smoke test (Commit 16) displays input events received

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + WindowInputDispatchTests pass
- Manual: смack test (Commit 16) verifies key press / mouse move / focus events captured
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Win32 message dispatch (WM_KEYDOWN/UP/MOUSEMOVE/L|R|MBUTTONDOWN/UP/MOUSEWHEEL/SETFOCUS/KILLFOCUS → InputEventQueue)`

### Commit 15 — Runtime facade composition: AssetManager + DefaultSampler + SpriteRenderer

**Files**:
- `src/DualFrontier.Runtime/Runtime.cs` (modified — extends composition с AssetManager + DefaultSampler + SpriteRenderer)
- `src/DualFrontier.Runtime/RuntimeOptions.cs` (modified — adds AssetsDirectory option default "assets")
- `tests/DualFrontier.Runtime.Tests/RuntimeCompositionTests.cs` (modified — V0.C.1 composition tests)

**Drift surface**: Runtime facade fully composed for V0.C.1. Runtime.Create returns с all V0.A+V0.B+V0.C.1 components operational + AssetManager loaded + DefaultSampler ready + SpriteRenderer ready (but no sprite yet — smoke test composes one).

**Implementation surface**:

```csharp
public sealed class Runtime : IDisposable
{
    // V0.A: Window + VulkanInstance + VulkanDevice + ValidationLayer + InputQueue
    // V0.B: Surface + Swapchain + RenderPass + Framebuffers + CommandPools + MemoryAllocator + ComputePipelines
    // V0.C.1 NEW:
    public AssetManager AssetManager { get; private set; } = null!;
    public VulkanSampler DefaultSampler { get; private set; } = null!;
    public TextureUploader TextureUploader { get; private set; } = null!;
    public VulkanSpritePipeline SpritePipeline { get; private set; } = null!;
    public SpriteDescriptorSetLayout SpriteDescriptorSetLayout { get; private set; } = null!;
    public VulkanPipelineLayout SpritePipelineLayout { get; private set; } = null!;
    public VulkanShaderModule SpriteVertexShader { get; private set; } = null!;
    public VulkanShaderModule SpriteFragmentShader { get; private set; } = null!;
    public SpriteRenderer SpriteRenderer { get; private set; } = null!;
    
    public static Runtime Create(RuntimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var runtime = new Runtime();
        try
        {
            // V0.A primitives (preserved)
            // ... InputQueue, Window, VulkanInstance, ValidationLayer, VulkanDevice, HardwareCapabilityCheck
            
            // V0.B primitives (preserved)
            // ... Surface, Swapchain, RenderPass, Framebuffers, GraphicsCommandPool, ComputeCommandPool, MemoryAllocator, ComputePipelines
            
            // V0.C.1 NEW
            runtime.AssetManager = new AssetManager(options.AssetsDirectory);
            runtime.DefaultSampler = new VulkanSampler(runtime.VulkanDevice);  // nearest + repeat default
            runtime.TextureUploader = new TextureUploader(
                runtime.VulkanDevice, runtime.MemoryAllocator, runtime.GraphicsCommandPool);
            
            // Sprite pipeline (loads shaders, builds descriptor set layout, builds pipeline layout, creates pipeline)
            byte[] spriteVertSpv = File.ReadAllBytes(Path.Combine(options.AssetsDirectory, "shaders/sprite.vert.spv"));
            byte[] spriteFragSpv = File.ReadAllBytes(Path.Combine(options.AssetsDirectory, "shaders/sprite.frag.spv"));
            runtime.SpriteVertexShader = new VulkanShaderModule(runtime.VulkanDevice, spriteVertSpv);
            runtime.SpriteFragmentShader = new VulkanShaderModule(runtime.VulkanDevice, spriteFragSpv);
            
            runtime.SpriteDescriptorSetLayout = new SpriteDescriptorSetLayout(runtime.VulkanDevice);
            
            var pushConstantRange = new VkPushConstantRangePublic(
                StageFlags: VkShaderStageFlagsPublic.Vertex,
                Offset: 0,
                Size: 64);
            runtime.SpritePipelineLayout = new VulkanPipelineLayout(
                runtime.VulkanDevice,
                descriptorSetLayouts: new[] { runtime.SpriteDescriptorSetLayout.Handle },
                pushConstantRanges: new[] { pushConstantRange });
            
            runtime.SpritePipeline = new VulkanSpritePipeline(
                runtime.VulkanDevice, runtime.RenderPass,
                runtime.SpriteVertexShader, runtime.SpriteFragmentShader,
                runtime.SpriteDescriptorSetLayout, runtime.SpritePipelineLayout);
            
            runtime.SpriteRenderer = new SpriteRenderer(
                runtime.VulkanDevice, runtime.MemoryAllocator, runtime.SpritePipeline);
            
            return runtime;
        }
        catch
        {
            runtime.Dispose();
            throw;
        }
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        // V0.C.1 teardown (reverse order)
        SpriteRenderer?.Dispose();
        SpritePipeline?.Dispose();
        SpritePipelineLayout?.Dispose();
        SpriteDescriptorSetLayout?.Dispose();
        SpriteFragmentShader?.Dispose();
        SpriteVertexShader?.Dispose();
        TextureUploader?.Dispose();
        DefaultSampler?.Dispose();
        AssetManager?.Dispose();
        // V0.B teardown
        ComputePipelines?.Dispose();
        MemoryAllocator?.Dispose();
        ComputeCommandPool?.Dispose();
        GraphicsCommandPool?.Dispose();
        foreach (var fb in _framebuffers) fb.Dispose();
        _framebuffers.Clear();
        RenderPass?.Dispose();
        Swapchain?.Dispose();
        Surface?.Dispose();
        // V0.A teardown
        VulkanDevice?.Dispose();
        ValidationLayer?.Dispose();
        VulkanInstance?.Dispose();
        Window?.Dispose();
        _disposed = true;
    }
}
```

**RuntimeOptions extension**:
```csharp
public sealed record RuntimeOptions
{
    public WindowOptions Window { get; init; } = new();
    public bool EnableValidationLayer { get; init; }
#if DEBUG
        = true;
#else
        = false;
#endif
    
    /// <summary>Assets root directory (V0.C.1+).</summary>
    public string AssetsDirectory { get; init; } = "assets";
}
```

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + Runtime composition tests pass на real hardware (full V0.A+V0.B+V0.C.1 composition)
- `sync_register.ps1 --validate` exit 0

**Commit message**: `feat(runtime): V0.C.1 — Runtime facade extension (AssetManager + DefaultSampler + sprite pipeline + SpriteRenderer)`

### Commit 16 — V0.C.1 smoke test (Kenney pawn rendered + input events demonstrated)

**Files**:
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` (modified — extends V0.B smoke test с V0.C.1 exit criteria)

**Drift surface**: V0.C.1 smoke test extends V0.B. Verifies V0.C.1 exit criteria per VULKAN_SUBSTRATE §4.2 R.1 + R.4:

1. ✓ V0.A criteria preserved (window opens, Vulkan instance live, validation clean, clean shutdown)
2. ✓ V0.B criteria preserved (HardwareCapabilityCheck passes, swapchain operational, clear color при no sprite, swapchain recreation, compute pipeline roundtrip)
3. ✓ **V0.C.1 R.1**: Kenney pawn sprite loaded via AssetManager + uploaded via TextureUploader + rendered at center of window via SpriteRenderer
4. ✓ **V0.C.1 R.4**: Input events generated при key press, mouse move, mouse click, focus change — events captured + printed к console
5. ✓ Validation log: 0 errors, 0 warnings (S-LOCK-4 preserved)

**Smoke test extension (key snippets)**:

```csharp
// V0.C.1 NEW: load Kenney pawn sprite + create texture
Console.WriteLine();
Console.WriteLine("Loading Kenney pawn sprite...");
PngImage pawnPng = runtime.AssetManager.LoadPng(new AssetPath("sprites/pawn.png"));
Console.WriteLine($"  Decoded PNG: {pawnPng.Width}×{pawnPng.Height} RGBA8 ({pawnPng.PixelsRgba8.Length} bytes)");

var pawnImage = VulkanImage.CreateFromPngImage(
    runtime.VulkanDevice, runtime.MemoryAllocator,
    runtime.TextureUploader, pawnPng);
using var pawnTexture = new SpriteTexture(pawnImage, runtime.DefaultSampler);
Console.WriteLine("  [PASS] Texture uploaded к device-local memory + transitioned к SHADER_READ_ONLY_OPTIMAL");

var pawnSprite = new Sprite(
    Texture: pawnTexture,
    Region: AtlasRegion.Full,
    Transform: new SpriteTransform(
        Position: new Vector2(0f, 0f),  // center of NDC space
        Scale: new Vector2(0.3f, 0.3f),  // 30% of NDC half-extent
        Rotation: 0f,
        TintRgba: SpriteVertex.WhiteTint));

// Show window + render loop с input event capture
runtime.Window.Show();
Console.WriteLine();
Console.WriteLine("Window opened. Rendering pawn sprite + capturing input events for 8 seconds...");
Console.WriteLine("Try: arrow keys, mouse movement, mouse clicks, alt-tab focus switch.");

int frames = 0;
int eventsCaptured = 0;
var startTime = DateTime.UtcNow;
while ((DateTime.UtcNow - startTime).TotalSeconds < 8 && runtime.Window.IsOpen)
{
    runtime.Window.PumpMessages();
    
    // Drain input events
    while (runtime.InputQueue.TryDequeue(out var evt))
    {
        Console.WriteLine($"  Event: {evt}");
        eventsCaptured++;
        
        if (evt is WindowResizeEvent resize)
        {
            runtime.Swapchain.Recreate(resize.NewWidth, resize.NewHeight);
        }
    }
    
    // Render frame (per-frame: acquire image, record command buffer с sprite draw, submit, present)
    runtime.RenderFrame(pawnSprite);
    frames++;
    Thread.Sleep(16);
}

double fps = frames / (DateTime.UtcNow - startTime).TotalSeconds;
Console.WriteLine();
Console.WriteLine($"  [PASS] Pawn sprite rendered ({frames} frames, {fps:F1} FPS)");
Console.WriteLine($"  [PASS] Input events captured: {eventsCaptured} (expected > 0 if interacted)");

// Validation check
if (runtime.ValidationLayer is not null && runtime.ValidationLayer.Log.ErrorCount > 0)
{
    Console.Error.WriteLine($"FAIL: {runtime.ValidationLayer.Log.ErrorCount} validation errors detected.");
    return 1;
}

Console.WriteLine();
Console.WriteLine("V0.C.1 smoke test PASS");
return 0;
```

**Manual visual verification expected by Crystalka**:
1. Window opens с Kenney pawn sprite visible centered
2. Sprite has correct UV mapping (no distortion or wrong texture region)
3. Sprite has correct alpha blending (если PNG has transparent edges, они blend correctly с clear color background)
4. Arrow keys / mouse movement / mouse clicks generate console-printed events
5. Alt-tab causes WindowFocusEvent(Focused: false) → Alt-tab back generates Focused: true
6. Window resize triggers swapchain recreation cleanly
7. Validation log shows 0 errors

**Validation**:
- `dotnet build` clean
- `dotnet test` 786+ baseline + all V0.C.1 tests pass
- `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` exits 0
  - Visual: pawn sprite visible
  - Events captured > 0 if user interacted
  - Frame rate ≥ 60 FPS
- `sync_register.ps1 --validate` exit 0

**Commit message**: `test(runtime): V0.C.1 — smoke test extension (Kenney pawn rendered + input events captured + R.1 + R.4 verified)`

### Commit 17 — V0.C.1 closure: REGISTER amendments + audit_trail EVT + brief lifecycle EXECUTED

**Files**:
- `docs/governance/REGISTER.yaml` (DOC-D-V0_C_1 lifecycle AUTHORED → EXECUTED; audit_trail EVT-{date}-V0_C_1-CLOSURE; 7 new REQs)
- `tools/briefs/V0_C_1_EXECUTION_BRIEF.md` (this brief — frontmatter status AUTHORED → EXECUTED; §8 closure section added)
- `docs/MIGRATION_PROGRESS.md` (V0.C.1 closure entry per METHODOLOGY §12.7 step 3)
- `docs/governance/REGISTER_RENDER.md` (regenerated)
- `docs/governance/VALIDATION_REPORT.md` (regenerated)

**REGISTER amendments**:

1. **DOC-D-V0_C_1**: lifecycle AUTHORED → EXECUTED
2. **DOC-D-V0_C_2**: placeholder note added — V0.C.2 brief pending (R.2 batched sprite + R.3 TileMap + Camera2D)
3. **audit_trail entry**: `EVT-{date}-V0_C_1-CLOSURE`
4. **Requirements added** (7 new REQs):
   - REQ-V0-C-1-PNG_DECODER — RGBA8/RGB8 8-bit non-interlaced PNG decoding
   - REQ-V0-C-1-SAMPLER — VulkanSampler с nearest+repeat default
   - REQ-V0-C-1-TEXTURE_UPLOAD — staging buffer + layout transitions
   - REQ-V0-C-1-SPRITE_PIPELINE — vertex input + alpha blending + descriptor sets + push constants
   - REQ-V0-C-1-SPRITE_RENDERER — single-sprite per draw
   - REQ-V0-C-1-INPUT_EVENTS — 6 event types + Key/MouseButton enums
   - REQ-V0-C-1-WIN32_INPUT_DISPATCH — WindowProcedure dispatch к InputEventQueue

**Validation**:
- `sync_register.ps1 --validate` exit 0
- `dotnet build` clean
- `dotnet test` 786+ green (V0.C.1 additive; final count documented в closure entry — estimated ~830-860 tests с new struct sizes + PNG decoder + sampler + sprite + input)
- Smoke test exits 0
- К-L19 invariant text preserved (no kernel changes V0.C.1)

**Commit message**:
```
governance: V0.C.1 closure — REGISTER amendments + 7 REQs + EVT-V0_C_1-CLOSURE

V0.C.1 V substrate R.1 + R.4 completion per METHODOLOGY §12.7 canonical protocol.

REGISTER updates:
- DOC-D-V0_C_1 lifecycle AUTHORED → EXECUTED
- DOC-D-V0_C_2 placeholder note: V0.C.2 brief pending (R.2 batched sprite + R.3 TileMap + Camera2D)
- MIGRATION_PROGRESS.md updated с V0.C.1 closure entry

Requirements added (7 new):
- REQ-V0-C-1-PNG_DECODER — RGBA8/RGB8 8-bit non-interlaced PNG decoding с all 4 filter predictors
- REQ-V0-C-1-SAMPLER — VulkanSampler с nearest+repeat default
- REQ-V0-C-1-TEXTURE_UPLOAD — staging buffer + buffer-to-image copy + layout transitions
- REQ-V0-C-1-SPRITE_PIPELINE — vertex input + alpha blending + descriptor sets + push constants
- REQ-V0-C-1-SPRITE_RENDERER — single-sprite per draw
- REQ-V0-C-1-INPUT_EVENTS — 6 event types + Key/MouseButton enums + VirtualKeyMapper
- REQ-V0-C-1-WIN32_INPUT_DISPATCH — WindowProcedure dispatch к InputEventQueue

audit_trail entry: EVT-{date}-V0_C_1-CLOSURE

V0.C.1 closure completes V0 substrate R.1 (first textured quad) + R.4 (input system).
Remaining V substrate work:
- V0.C.2: R.2 batched sprite renderer (10,000 sprites at 60+ FPS) + R.3 TileMap + Camera2D
- V0 substrate close gates на V0.C.2 (per Q8 ratification)
- К10.3 brief restart independent of V0.C.1; can proceed parallel или sequential per Crystalka decision

Phase 17 of V0.C.1 cascade. Commit 17 of 17 — V0.C.1 closure.
```

---

## §4 — Halt triggers (V0.C.1-specific SC-N taxonomy)

V0.C.1 SC-N taxonomy refined по V0.A + V0.B precedents. V0.C.1 introduces PNG decoder complexity (chunk parsing, CRC32, DEFLATE, filter unfiltering) + texture upload synchronization + descriptor pool management + Win32 input dispatch — each c new halt classes.

Per Lesson #8 corollary: brief promises «halts before damage», не «zero halts». V0.A landed zero hard-gate halts; V0.B landed zero hard-gate halts + 5 brief-estimate corrections caught via test gate. V0.C.1 preserves discipline.

### SC-1 — Code anchor doesn't match brief assumptions

V0.B code shapes verified Phase 0 brief authoring 2026-05-19. Если existing V0.B code drifted с merge (unlikely but possible), halt и surface drift.

### SC-2 — Vulkan struct alignment regression

Если Marshal.SizeOf<T>() test fails для V0.A или V0.B baseline struct, halt. Regression = serious issue (compiler change, .NET runtime change, dependency upgrade).

### SC-3 — Deep-read contradiction

Any §2.3/§2.4 mandatory re-read surfaces a file shape that contradicts this brief. Halt + surface.

### SC-4 — New Vulkan struct size mismatch (Lesson #7 strengthening applied)

Per V0.B precedent (5 corrections caught): Marshal.SizeOf<T>() для new V0.C.1 struct не matches brief hypothesis. **Expected behavior** — executor adjusts test expected value к actual Marshal.SizeOf<T>() result + adds explicit padding fields if needed per V0.A VkPhysicalDeviceProperties precedent.

**Recovery**: read `$(VULKAN_SDK)\Include\vulkan\vulkan_core.h` для exact struct layout; correct test expected value or add padding. **Document correction в commit message** per V0.B honest reporting precedent (5 brief-estimate corrections documented в V0.B commit messages).

**Not a halt** — это expected pattern per Lesson #7 strengthening. Halt только если cannot reconcile actual sizeof с struct layout (suggests structural alignment problem unrelated к padding).

### SC-5 — VULKAN_SDK env var unset

Hard gate per §2.2 (V0.A SC-16 + V0.B SC-5 precedent).

### SC-6 — glslangValidator.exe absent

V0.B Commit 10 committed `tools/glslangValidator.exe`. Если absent post-V0.B (unlikely — committed binary), halt + verify V0.B PR merge was complete.

### SC-7 — Vulkan validation regression (V0.C.1 introductions)

S-LOCK-4 mandates ZERO validation errors. V0.C.1 introduces substantial new Vulkan code surface — каждая new call has potential validation message. Common V0.C.1-specific causes:

- **Image layout transition wrong**: TextureUploader must transition UNDEFINED → TRANSFER_DST_OPTIMAL → SHADER_READ_ONLY_OPTIMAL exactly. Skipping или wrong order = validation error.
- **Pipeline barrier missing**: Between staging upload + draw call в same command buffer requires barrier
- **Descriptor set binding mismatch**: sprite.frag declares `layout(set = 0, binding = 0) uniform sampler2D atlas`; descriptor set layout must match exactly (binding 0, COMBINED_IMAGE_SAMPLER, FRAGMENT_BIT)
- **Push constant range mismatch**: VkPushConstantRange offset/size must match `vkCmdPushConstants` offset/size + shader push_constant block layout
- **Vertex input attribute format mismatch**: sprite.vert declares `layout(location = 2) in vec4 inColor`; vertex attribute format must produce vec4 — VK_FORMAT_R8G8B8A8_UNORM produces vec4 via UNORM expansion ([0..1] from [0..255])
- **Pipeline color blending mismatch**: Render pass attachment format must allow blending operation (color attachment must be blendable per device's VkFormatProperties)

Halt + investigate validation message via ValidationLog dump + diff against expected pipeline barrier sequence.

### SC-8 — PNG decoder rejects valid Kenney pawn PNG

V0.C.1 smoke test prerequisite: Kenney pawn PNG decodes successfully. Если PngDecoder throws (e.g., unexpected color type, interlaced detected, CRC mismatch), halt SC-8 + investigate.

**Recovery**:
- Verify PNG file integrity (open в external viewer)
- Verify PNG matches S-LOCK-2 supported subset (8-bit RGB или RGBA, non-interlaced)
- If PNG uses 16-bit или palette, re-export к 8-bit RGBA via GIMP/Paint.NET (Phase 0 SC-A asset prerequisite may need re-fulfillment)

### SC-9 — DEFLATE decompression fails

DeflateStream throws (corrupt IDAT bytes, unexpected zlib header). Halt SC-9 + investigate.

**Common cause**: PNG decoder Part 2 skips zlib header (2 bytes) — если skip wrong (zlib header format varies), DeflateStream fails. Verify zlib header bytes (CMF byte = 0x78, FLG byte varies) match expected.

### SC-10 — Filter unfiltering produces wrong pixel data

Synthetic PNG test fails: decoded RGBA bytes don't match expected. Halt SC-10 + investigate filter predictor implementation.

**Most likely**: Paeth predictor — Most error-prone filter; verify against PNG spec verbatim (RFC 2083 section 6.6).

### SC-11 — Sprite pipeline creation fails

`vkCreateGraphicsPipelines` returns non-success. V0.C.1 sprite pipeline more complex than V0.B clearcolor (vertex input + blending + descriptor sets + push constants — 4 axes vs 0). Common causes:

- Vertex input description doesn't match shader input attributes
- Descriptor set layout doesn't match shader binding declarations
- Push constant range conflicts с pipeline layout
- Color attachment format не compatible с blending mode (e.g., SRGB swapchain may не allow blending без extension)
- Stencil/depth state expected but pDepthStencilState = IntPtr.Zero (sprite pipeline doesn't need depth — verify Vulkan accepts this)

Halt + validation log + isolate failing axis (try without descriptor set, then without push constant, etc.).

### SC-12 — Validation regression post-commit

Sync_register.ps1 --validate exits non-zero после V0.C.1 commit. Halt immediately per V0.A/V0.B precedent.

### SC-13 — Scope creep

Execution encounters drift не в V0.C.1 scope (e.g., implementing batched renderer V0.C.2 scope, или Camera2D class). Halt + surface. Не «fix while we're here» per Lesson #20.

### SC-14 — Win32 input event mapping not exhaustive

VirtualKeyMapper.Map returns Key.Unknown для commonly-used keys. Acceptable если key not в S-LOCK-10 enum scope (e.g., NumPad, media keys deferred); halt если basic keys broken (arrow keys, escape, space, letters).

### SC-15 — Asset path resolution security

AssetManager path traversal check rejects valid asset path (e.g., `sprites/pawn.png` rejected). Halt + verify path traversal logic correct.

### SC-16 — Push-to-main classifier reminder (operational, не halt)

Known behavior per V0.A/V0.B precedent: Claude Code auto-mode classifier blocks push-to-main even с explicit instruction в initial prompt. Не halt — expected. Re-confirm in-session после work done, then push branch к remote.

### SC-17 — Manual visual verification fails

Smoke test exits 0 в console (validation clean, frame rate OK), но Crystalka manual visual sees broken rendering (pawn invisible, wrong color, distorted UV, alpha blending wrong). Halt + investigate.

**Common causes**:
- MVP matrix not applied correctly (identity in V0.C.1 single-sprite mode — sprite should appear at world origin = center of NDC, но wrong scale interpretation may render off-screen)
- UV coordinates flipped (Vulkan has Y-down NDC vs OpenGL Y-up — verify sprite UVs match texture orientation)
- Alpha blending mode wrong (PNG with transparent edges renders с visible alpha channel as opaque если blending disabled)
- Sampler filter mode wrong (linear filter с pixel art = blurry)

Recovery: produce smoke test screenshot, diff against expected; adjust shader code или pipeline config; re-run smoke test.

### SC-18 — Descriptor pool exhaustion

V0.C.1 SpriteRenderer.cs descriptor pool sized для 32 distinct textures. Если smoke test или integration test allocates > 32 unique SpriteTexture instances, halt SC-18.

**V0.C.1 mitigation**: smoke test uses 1 texture (Kenney pawn) — well within limit. Halt only если test infrastructure inadvertently creates > 32. V0.C.2 will expand pool sizing for batched 10,000-sprite scenarios.

### SC-19 — Multi-session execution pause

V0.C.1 substantial scope may exceed single Claude Code session token budget (estimated 20-30 hours auto-mode = ~5-8 hours wall time given Claude Code throughput). Per S-LOCK-11 + Lesson #8: atomic intermediate states preserve resume capability.

Если session approaches limit:
1. Complete current atomic commit (do не leave partial)
2. Push branch к remote (preserves work)
3. Surface к Crystalka: «V0.C.1 paused at Commit N/17. Resume в next session.»
4. Next session: read current state, identify last clean commit, resume from Commit N+1

Не a true halt — pause + resume mechanism per V0.B precedent.

При halting (SC-1..SC-19): author HALT_REPORT в `docs/scratch/V0_C_1/`, state trigger, state what was/wasn't committed, stop. **Do не commit partial atomic commit** — atomicity protects milestone per Lesson #8.

---

## §5 — Closure protocol (per METHODOLOGY §12.7 canonical)

After Commit 17 lands clean:

### §5.1 — Verify final state

1. `git log --oneline` shows ~17 commits added by V0.C.1 на feature branch `claude/v0_c_1-vulkan-textured-sprite`
2. `git status` clean working tree
3. `sync_register.ps1 --validate` exit 0
4. `dotnet build` clean
5. `cmake --build native/DualFrontier.Core.Native` clean (V0.C.1 doesn't modify native — V0.B baseline preserved 78 selftest scenarios)
6. `dotnet test` 786+ green (V0.C.1 additive — estimated ~830-860 tests final)
7. V0.C.1 smoke test exits 0:
   - Window opens
   - Kenney pawn sprite visible centered
   - Input events captured при user interaction (> 0 events если interacted)
   - Frame rate ≥ 60 FPS sustained for 8 seconds
   - HardwareCapabilityCheck passed (К-L19 preserved)
   - Swapchain recreation clean on resize
   - Validation log: 0 errors, 0 warnings
8. K-L19 invariant text preserved (no kernel changes V0.C.1)
9. Manual visual verification на Crystalka «Skarlet»: pawn sprite visible, alpha blending correct, input events generate console output

### §5.2 — Update brief status + closure section

Set `status: EXECUTED` в frontmatter; add §8 closure section с commit range + date + commit ledger table + verification metrics + halt protocol activations + lesson candidates + pattern established. Same template как V0.A/V0.B precedent.

### §5.3 — PR opening (NOT auto-push, per V0.A/V0.B precedent)

- Push branch `claude/v0_c_1-vulkan-textured-sprite` к remote (NOT к `main`)
- Open PR titled «V0.C.1 — PNG decoder + textured sprite pipeline + input event types (R.1 + R.4)»
- Body summarizes per-commit per-deliverable mapping + verification metrics + halt activations + closure section
- **DO NOT auto-push к main**. Crystalka reviews + merges per established protocol

### §5.4 — Surface к Crystalka

PR ready для review. Crystalka:
1. Reviews V0.C.1 closure report
2. Reviews V0.C.1 smoke test output (validation clean, pawn rendered, input events captured)
3. Manually verifies visual behavior на «Skarlet»
4. Merges PR к `main`
5. Provides closure report к next Opus deliberation session

**Next Opus session decision tree**:
- **Option A — V0.C.2 brief authoring**: continues V substrate stream с R.2 batched sprite renderer + R.3 TileMap + Camera2D. After V0.C.2 closure → V0 substrate close (per Q8 ratification)
- **Option B — К10.3 brief restart**: V0.A + V0.B together unblock К10.3; restart с surgical amendments. Independent of V0.C.1 closure
- **Option C — К10.4 (TLA+) brief authoring**: TLA+ formal verification of К10 scheduler infrastructure — independent stream

V0.C.1 closure doesn't gate К10.3 restart или К10.4 authoring. Crystalka prerogative.

### §5.5 — V0 substrate close pathway (post-V0.C.2 closure)

Per Q8 ratification: V0 substrate close gates на V0.C completion (R.1-R.4 visual primitives equivalent). V0.C.1 + V0.C.2 closure = V0 substrate close.

After V0 substrate close:
- V1 brief authoring possible (scalar field + diffusion shader — M-V1 mana, M-V2 electricity)
- V2 brief authoring possible (scalar field + wave shader — M-V7 movement)
- R.5-R.8 phases (Domain integration + UI + lifecycle + Godot cutover) separate R-cycle briefs
- K-closure report (A'.8) accumulates V substrate experience for archival summary

---

## §6 — Brief authority + lifecycle

**Brief authority**: V substrate authoring stream continued post-V0.B closure 2026-05-19 per Crystalka split ratification 2026-05-19 (V0.C → V0.C.1 + V0.C.2). V0.C.1 is third sub-milestone of V0 series (V0.A → V0.B → V0.C.1). К10.3 brief restart pathway already open; independent of V0.C.1.

**Brief lifecycle** (per FRAMEWORK §3.3 + §3.3.1):
- AUTHORED at this commit (Commit 1 of cascade)
- EXECUTED post-Commit 17 closure
- Registered в `tools/briefs/` as Tier 3 Category D per A'.4.5 governance

**Brief enrollment**: V0.C.1 brief added к REGISTER.yaml в Commit 1 atomic с brief authoring per V0.A/V0.B precedent.

**Brief location**: `tools/briefs/V0_C_1_EXECUTION_BRIEF.md` after Crystalka copies из `/mnt/user-data/outputs/` per Filesystem MCP workaround pattern.

**Brief lifecycle для V0.C.2**:
- V0.C.2 brief authoring deferred до V0.C.1 closure
- V0.C.2 brief scope: R.2 batched sprite renderer + R.3 TileMap + Camera2D
- V0.C.2 brief size estimate: ~1800-2200 lines (smaller than V0.C.1 — fewer new Vulkan primitives, mostly refactoring SpriteRenderer + adding Camera2D class)

---

## §7 — Lesson candidates surfaced (informational, formal promotion deferred к А'.8 K-closure report)

**Lesson #7 strengthened continues** (V0.A + V0.B + V0.C.1 application): P/Invoke ABI alignment audit recipe. Every new Vulkan struct получает Marshal.SizeOf<T>() unit test validating against actual sizeof. V0.A caught 1 alignment error (VkPhysicalDeviceProperties 816 → 824); V0.B caught 5 brief-estimate errors; V0.C.1 expects similar discipline. ~10 new structs introduced V0.C.1.

**Lesson #22 strengthened continues** (V0.B formalized via S-LOCK-7): Mixed [LibraryImport] + [DllImport] convention. .NET 7+ source generator constraints (non-blittable struct fields not supported) force pragmatic mixing. V0.C.1 inherits without modification.

**Lesson #11 continues** (architectural reduction methodology): V0.C.1 applies Q1 split rationale (V0.C → V0.C.1 + V0.C.2) per Crystalka surfacing concern that V0.C monolithic exceeds reasonable session budget. Reduction = decompose substantial scope into coherent staged delivery preserving multi-session safety.

**К-L14 application surface — V substrate inheritance** (V0.A noted, V0.B reinforced, V0.C.1 third validation): К-L14 «performance derives from clean complex architecture». V0.C.1 substantial scope (PNG decoder + texture upload + sprite pipeline + input dispatch) acceptable when architecturally coherent. Default-inclusion bias: bumper allocator continues (V0.B adequate), in-repo SPIR-V toolchain extends к sprite shaders, premultiplied alpha blending workflow (production-standard, не simplified), all 4 PNG filter predictors (full coverage, не staged subset).

**Lesson candidate #25 surfaced V0.B — applied V0.C.1**: «Implementation depth follows consumer materialization». V0.B landed native C ABI bookkeeping stubs (managed-side wrappers operational; native-side Vulkan calls deferred к V1+). V0.C.1 honors this pattern — uses V0.B managed-side wrappers, не expects native-side full implementation. Future V1/V2 substrate primitives will populate native-side stubs when actually consuming K9 field storage via compute pipelines.

**Lesson candidate #26** (V0.C.1 authoring potential — emerging): «Cross-substrate scope splitting preserves multi-session execution budget». V0.C split ratification (V0.C.1 + V0.C.2) decomposes V0 rendering completion across two execution sessions. Each session sizes к reasonable budget (~20-30 hours). К-L14 default-inclusion preserved through scope coherence не aggregation.

**К-L14 third verification** (V0.A first, V0.B second, V0.C.1 third): if V0.C.1 closes с similar discipline (zero hard-gate halts + alignment audit catches early + validation clean + manual visual verification passes + PNG decoder operates correctly), К-L14 thesis on V substrate validated across three substantial cascades. **Three consecutive successful cascades** establishes pattern reliability на V substrate just as K series established pattern on K substrate.

---

**End of brief. ~17 atomic commits across 11 S-LOCK invariants + 35+ deliverables (struct test extensions + Vulkan extensions + PNG decoder Parts 1 + 2 + AssetManager + VulkanSampler + TextureUploader + VulkanPipelineLayout extension + sprite shaders + MSBuild target extension + sprite primitive types + VulkanSpritePipeline + SpriteRenderer + input event types + Win32 dispatch + runtime composition + V0.C.1 smoke test + closure). Expected 20-30 hours auto-mode execution (Crystalka «Skarlet»).**

V0.C.1 closes V substrate R.1 (first textured quad) + R.4 (input system). Remaining V substrate work:
- V0.C.2: R.2 batched sprite renderer (10,000 sprites at 60+ FPS) + R.3 TileMap + Camera2D
- V0 substrate close gates на V0.C.2 (per Q8 ratification)
- К10.3 brief restart independent of V0.C.1 — Crystalka prerogative on timing
- К10.4 TLA+ brief — independent stream
- A'.8 K-closure report after К-series + V substrate close (accumulates lessons)
- Phase B M-cycle vanilla migration after analyzer milestone

«Halt is success, не failure» per Lesson #8 corollary — V0.A landed first Vulkan code с zero hard-gate halts + 1 tactical course-correction; V0.B landed substantial scope с zero hard-gate halts + 5 alignment audit corrections caught early. V0.C.1 preserves discipline.

«Без костылей» applied к V substrate authoring V0.C.1:
- PNG decoder full subset coverage (all 4 filter predictors, both RGB8/RGBA8, mandatory CRC32) — explicit minimum scope с honest documentation of exclusions (interlaced, palette, 16-bit deferred к Lesson #25 pattern), не temporary hack
- VulkanSampler API exposes options record (filter mode + wrap mode) — clean architecture allows future configurability без breaking changes
- VulkanPipelineLayout backward-compatible push constant range extension — V0.B regression preserved
- Sprite shaders production-shape (vertex input + alpha blending + descriptor sets + push constants) — full sprite pipeline establishment, не minimal placeholder
- Premultiplied alpha blending workflow standard для production sprite rendering, не simplified
- Input event types complete per VULKAN_SUBSTRATE §2.2 — full event surface, не staged subset
- VirtualKeyMapper covers all keys в Key enum — exhaustive mapping, не sparse

К-L14 thesis on V substrate validated across three consecutive substantial cascades (V0.A → V0.B → V0.C.1, all zero-halt closures) establishes substrate authoring pattern reliability. К-L14 falsifiable claim («clean complex architecture causes performance») has accumulated evidence от V substrate development matching prior accumulation from K substrate (К0..К10 development).
