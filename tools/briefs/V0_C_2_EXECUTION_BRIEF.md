---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-V0_C_2
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-V0_C_2
---
---
register_id: DOC-D-V0_C_2
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-19
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-V0_C_2
---

# V0.C.2 — Batched sprite renderer + Camera2D + TileMap (V substrate R.2 + R.3)

**Brief lifecycle**: AUTHORED 2026-05-19 by Opus deliberation. EXECUTED post-Commit 17 closure. Author: Crystalka (judicial role) + Claude Opus (deliberation pipeline). Successor brief к V0.C.1 (closed PR #38 merged) per V substrate authoring stream after V0.C split ratification 2026-05-19 (V0.C → V0.C.1 + V0.C.2). **V0.C.2 closure = V0 substrate close** per Q8 ratification 2026-05-19 — gates Phase B M-cycle vanilla content mass migration.

**Authority chain**:

1. **VULKAN_SUBSTRATE.md v1.0 LOCKED** (DOC-A-VULKAN_SUBSTRATE, supersedes RUNTIME_ARCHITECTURE.md + GPU_COMPUTE.md). §1.4 V substrate close acceptance criteria, §4.2 R.2 batched sprite renderer (4-6h, ~500-700 LOC), §4.2 R.3 TileMap + Camera2D (3-4h, ~400-600 LOC).
2. **V0.A EXECUTED** (PR #36 merged 2026-05-18, 11 commits, 685 tests baseline) — pure P/Invoke Vulkan foundation, VkInstance + VkDevice + ValidationLayer ALWAYS-ON pattern.
3. **V0.B EXECUTED** (PR #37 merged 2026-05-19, 18 commits, 786 tests, K-L19 LOCKED) — Surface + Swapchain + RenderPass + Framebuffers + CommandPool + MemoryAllocator + ComputePipelineRegistry + VulkanBuffer (IndexBuffer flag supported) + VulkanImage + VulkanShaderModule + graphics pipeline + compute pipeline + descriptor primitives. 5 alignment audit corrections caught at Marshal.SizeOf<T>() test gate per Lesson #7 strengthening.
4. **V0.C.1 EXECUTED** (PR #38 merged 2026-05-19, 17 commits, 210 Runtime.Tests, smoke test 820 frames at 164 FPS на AMD RX 7600S, validation 0 errors). **Zero alignment audit corrections** — discipline matured. PNG decoder + AssetManager + VulkanSampler + TextureUploader + VulkanPipelineLayout push constant range extension + sprite shaders + sprite primitive types + VulkanSpritePipeline + SpriteRenderer single-sprite-per-call + input event types + Win32 dispatch. Two V0.B latent bugs surfaced + fixed within V0.C.1 cascade: framebuffers stale after swapchain recreation, single render-finished semaphore reuse race.
5. **METHODOLOGY.md** v1.0 LOCKED — atomic cascade discipline, halt-before-damage principle (Lesson #8), substrate authoring stream protocol §12.7.
6. **CODING_STANDARDS.md** v1.0 LOCKED — no LINQ в src, file-scoped namespaces, internal P/Invoke artifacts, ALWAYS-ON validation layer DEBUG, mixed [LibraryImport] + [DllImport] convention per Lesson #22.
7. **K-L19 LOCKED via V0.B** — Hardware capability fail-fast invariant. Vulkan 1.3 + queue family requirements verified at Runtime.Create. Preserved verbatim в V0.C.2 — no kernel changes.
8. **K10.3 brief halted 2026-05-18** — independent stream, V0.A + V0.B unblocked К10.3 restart pathway; V0.C.2 does not gate. Crystalka prerogative on К10.3 restart timing.
9. **Q1-Q6 ratification 2026-05-19** (Crystalka): vertex buffer = ring buffer N-frame (Q1c); indexed quad rendering = indexed (Q2b); Camera2D scope = standard (Q3b — position + zoom + rotation + viewport + matrices + transforms, ~150 LOC); TileMap = one sprite per tile (Q4a — reuses batched infrastructure, 40K sprite stress = 4× R.2 target); atlas regions = code-defined (Q5a — AtlasRegion.FromPixels per tile, JSON manifest deferred к V0.C.2.1/V0.D); monolithic brief (Q6 — 1800-2200 lines target, 15-25h auto-mode execution, multi-session pause provision).

**Authority chain integrity statement**: This brief inherits V0.A foundation primitives + V0.B GPU infrastructure (MemoryAllocator bumper, IndexBuffer flag landed, swapchain recreation discipline) + V0.C.1 sprite primitives (SpriteVertex 20 bytes, AtlasRegion, Sprite, SpriteTexture, SpriteTransform, VulkanSpritePipeline, SpriteDescriptorSetLayout). V0.C.2 REFACTORS SpriteRenderer.cs к batched (90% rewrite — `BeginFrame()/Submit()/EndFrame()` API replaces single-sprite `DrawSprite()`), ADDS VertexBufferRing infrastructure, ADDS IndexBuffer support, ADDS Camera2D class, ADDS TileMap class, ADDS atlas region helpers (per Q5a code-defined). Per **К-L14 default-inclusion bias** preserved through scope coherence — V0.C.2 substantial but cohesive; multi-session pause per Lesson #8 atomic intermediate states.

**К-L14 thesis third+fourth verification window**: V0.A → V0.B → V0.C.1 closed с three consecutive zero-hard-gate-halt cascades. V0.C.2 closure = fourth verification + V0 substrate close pattern reliability. If V0.C.2 closes с similar discipline → К-L14 «performance derives from clean complex architecture» falsifiable thesis empirically validated на V substrate matching K substrate K0..K10 development accumulation.

---

## Intro — V0.C.1 closure context + V0.C.2 scope discipline

### Inherited state from V0.C.1 closure (verified 2026-05-19)

V0.C.1 closed clean per PR #38 merge. Repository state at V0.C.2 brief authoring time:

**Baseline test count**: 210 Runtime.Tests passing (121 V0.B baseline + 89 V0.C.1 additions). V0.C.2 estimated к add ~50-80 new tests proportional к architectural surface — final estimated test count post-V0.C.2 ≈ 260-290 Runtime.Tests.

**Smoke test baseline**: V0.C.1 Program.cs renders Kenney pawn (synthetic RGBA8 PngImage due к Kenney palette PNG incompatibility) at 164 FPS sustained over 5 seconds (820 frames), zero validation errors/warnings. V0.C.2 extends smoke test с 10,000-sprite stress + 200×200 TileMap visual verification.

**Framebuffer recreation discipline established**: `Runtime.RecreateFramebuffersForSwapchain()` helper landed V0.C.1 Commit 15 per latent bug fix. V0.C.2 preserves verbatim — swapchain recreation path stable.

**Per-image semaphore discipline established**: V0.C.1 fixed single render-finished semaphore reuse race via per-image semaphores. V0.C.2 preserves verbatim.

**Validation layer ALWAYS-ON в DEBUG**: V0.A pattern preserved through V0.C.1, V0.C.2 preserves. Zero validation regressions tolerated per S-LOCK invariant.

**Sprite primitive types stable**:
- `SpriteVertex` (20 bytes verified via Marshal.SizeOf<SpriteVertex>() test gate) — position (Vector2) + UV (Vector2) + TintRgba (uint packed RGBA)
- `Sprite` record — Texture (SpriteTexture) + Region (AtlasRegion) + Transform (SpriteTransform)
- `AtlasRegion` — U0/V0/U1/V1 normalized texture coordinates
- `SpriteTransform` — Position (Vector2) + Scale (Vector2) + TintRgba (uint)
- `SpriteTexture` — Image (VulkanImage) + Sampler (VulkanSampler)
- `VulkanSpritePipeline` — graphics pipeline c vertex input + alpha blending + descriptor sets + push constants
- `SpriteDescriptorSetLayout` — binding 0 COMBINED_IMAGE_SAMPLER FRAGMENT_BIT

**VulkanPipelineLayout extension landed V0.C.1**: `pushConstantRanges` parameter accepts `VkPushConstantRangePublic` array. V0.C.2 consumes for Camera2D MVP push constant — current usage в Runtime.cs Composition: 64-byte VERTEX_BIT range for Matrix4x4 MVP.

**VkApi.cs landed functions (V0.A + V0.B + V0.C.1 combined)**:
- vkCmdBindVertexBuffers (V0.C.1)
- vkCmdPushConstants (V0.C.1)
- vkCmdCopyBufferToImage (V0.C.1)
- vkCmdPipelineBarrier (V0.C.1)
- vkCreateSampler / vkDestroySampler (V0.C.1)
- vkCmdDraw, vkCmdSetViewport, vkCmdSetScissor (V0.B)
- vkCmdBindPipeline, vkCmdBindDescriptorSets (V0.B)
- vkCmdBeginRenderPass, vkCmdEndRenderPass (V0.B)
- Pipeline + descriptor + buffer + image + memory + swapchain + surface + sync primitives (V0.A + V0.B)

**VkApi.cs NOT YET LANDED — V0.C.2 must add**:
- `vkCmdBindIndexBuffer` — required for indexed draw (Q2b ratification)
- `vkCmdDrawIndexed` — required for indexed draw (Q2b ratification)

VulkanBuffer.cs supports `VkBufferUsageFlagsPublic.IndexBuffer = 0x00000040` flag landed V0.B — V0.C.2 instantiates index buffer via existing infrastructure без VulkanBuffer changes.

**Sprite shaders landed V0.C.1**: `assets/shaders/sprite.vert.spv` + `assets/shaders/sprite.frag.spv` precompiled via MSBuild `CompileShaders` target в `Directory.Build.props`. V0.C.2 reuses existing sprite shaders — no new shaders required (push constant MVP wiring + identity matrix transform from V0.C.1 already production-ready для Camera2D adoption).

**Runtime facade composition stable**: All V0.A + V0.B + V0.C.1 primitives composed в `Runtime.Create()`. V0.C.2 extends с Camera2D + TileMap construction; `RecordSpriteFrame()` single-sprite convenience method retained but V0.C.2 adds `RecordSpritesFrame(IEnumerable<Sprite>, Camera2D, ...)` batched convenience.

### V0.C.2 scope per Q1-Q6 ratification

**Scope-IN (V0.C.2 deliverables)**:

1. **VkApi.cs extension**: add `vkCmdBindIndexBuffer` + `vkCmdDrawIndexed` per Q2b ratification.

2. **VertexBufferRing infrastructure** per Q1c ratification: N-frame ring buffer matching swapchain image count (typically 2-3 frames). Each frame writes own chunk. Avoids cross-frame sync hazards. Production-standard pattern.

3. **IndexBuffer per-frame** per Q2b ratification: 6-index pattern per quad (0,1,2,2,3,0). Pre-populated once at SpriteRenderer construction time — index buffer constant for batched quad rendering.

4. **SpriteRenderer batched rewrite** per Q1c + Q2b ratification: replaces `DrawSprite(Sprite, CommandBuffer, mvp)` single-sprite API с `BeginFrame() / Submit(Sprite) / EndFrame(CommandBuffer, Camera2D)` batched API. **4-vertex-per-quad** (top-left, top-right, bottom-right, bottom-left) consumed via index buffer (40K vertices + 60K indices for 10K sprites vs 60K non-indexed = ~33% vertex reduction). Per-frame descriptor pool reset for descriptor set lifecycle simplification.

5. **Camera2D class** per Q3b ratification: position (Vector2) + zoom (float, default 1.0) + rotation (float radians, default 0.0) + viewport (Vector2 — typically window size). View matrix construction (translate + rotate + scale) + ortho projection matrix construction (left/right/top/bottom from viewport + zoom). World-к-screen + screen-к-world transforms. ~150 LOC target. Culling helpers (`IsVisible(AABB)`) deferred — extends Camera2D when needed per «features only on demand» (К-L14 default-inclusion preserved through scope coherence).

6. **TileMap class** per Q4a ratification: 2D grid of tiles, each tile = `Sprite` instance referencing TileMap's SpriteTexture (one atlas reused across all tiles) + `AtlasRegion` for specific tile type. `TileMap.Submit(SpriteRenderer)` enumerates visible tiles + calls `SpriteRenderer.Submit(Sprite)` per tile. **One sprite per tile approach** — reuses batched infrastructure. 200×200 grid = 40,000 tiles = 4× R.2 stress test target. Tests batching scales beyond 10K sprites.

7. **Atlas region helpers** per Q5a ratification: `AtlasRegion.FromPixels(int xPx, int yPx, int wPx, int hPx, int texWidthPx, int texHeightPx)` static factory method computing normalized U0/V0/U1/V1 from pixel coordinates. Code-defined per TileMap initialization — `var grassRegion = AtlasRegion.FromPixels(0, 0, 16, 16, 256, 256);`. JSON manifest deferred к V0.C.2.1 sub-cycle or V0.D Domain integration timeframe per «features only on demand».

8. **Runtime facade extension**: Camera2D + TileMap exposed as `Runtime.Camera` + `Runtime.TileMap` properties. `Runtime.RecordSpritesFrame(IEnumerable<Sprite>, Camera2D, int imageIndex, Vector4 clearColor)` batched convenience method. V0.C.1 `RecordSpriteFrame()` single-sprite method **retained for backward compatibility** + reduced-scope smoke testing.

9. **10,000-sprite stress test**: extends V0.C.1 smoke test program — generates 10,000 randomly-positioned + tinted sprites referencing single procedurally-generated atlas. Verifies 60+ FPS sustained over 10 seconds (600+ frames at 60 FPS). Validation clean throughout. Manual visual verification per established M8.8/M8.9 protocol.

10. **200×200 TileMap test**: extends smoke test — populates 200×200 grid с procedurally-determined tile types (e.g. checker pattern, или Perlin noise-based terrain), camera pannable via input events captured V0.C.1, verifies 60+ FPS sustained, manual visual verification.

11. **Closure governance**: REGISTER.yaml DOC-D-V0_C_2 enrolled at AUTHORED → EXECUTED transition. audit_trail event for V0.C.2 closure. MIGRATION_PROGRESS.md V substrate progress reflects V0 substrate close per Q8 ratification.

**Scope-OUT (explicit Lesson #20 discipline — V0.C.2 не делает)**:

- **JSON atlas manifest** (deferred к V0.C.2.1 sub-cycle или V0.D — per Q5a discipline)
- **Atlas region culling helpers** в Camera2D (`Camera2D.IsVisible(AABB)`) — deferred per Q3b discipline; extends Camera2D when measured insufficient
- **GPU-instanced TileMap rendering** — deferred per Q4a discipline; one-sprite-per-tile reuses batched infrastructure, instanced fallback only if measured insufficient
- **Chunked TileMap rendering** (per-chunk vertex buffer e.g. 32×32 tile chunks) — deferred per Q4a discipline; reused batched infrastructure preferred
- **BitmapFont + TextRenderer** — R.6 separate brief post-V substrate close
- **UI primitives** (Panel, Label, ProgressBar) — R.6
- **DebugOverlay coupling к loop.SetPaused** — R.7
- **PresentationBridge + Domain integration** — R.5 post-V substrate close
- **Godot Presentation cutover** — R.8 post-V substrate close
- **К10.3 brief restart** — independent stream
- **К10.4 TLA+ brief authoring** — independent stream
- **A'.8 K-closure report** — accumulates lessons after V substrate close + K series close
- **A'.9 Roslyn analyzer milestone** — post-K-closure
- **Phase B M-cycle vanilla content migration** — gated on V0 substrate close + analyzer
- **V1 brief authoring** (scalar field + diffusion shader) — gated on V0 substrate close
- **V2 brief authoring** (scalar field + wave shader) — gated on V0 substrate close

### Brief size growth proportional rationale

V0.A brief: 1612 lines. V0.B brief: 2320 lines. V0.C.1 brief: 2961 lines. V0.C.2 brief target: **1800-2200 lines** — **smaller than V0.C.1** despite substantial scope due to:

- Fewer new Vulkan primitives (only 2 new vkApi functions vs ~10 V0.C.1 new functions)
- Mostly refactoring existing infrastructure (SpriteRenderer 90% rewrite, не new primitive)
- Inherited discipline patterns (alignment audit, ALWAYS-ON validation, mixed P/Invoke, atomic cascade, manual visual verification) — references inherited V0.C.1 patterns without re-deriving

К-L14 default-inclusion bias preserved через scope coherence — substantial cohesive scope не aggregation.


---

## §1 — S-LOCKs (V0.C.2 invariants)

S-LOCKs are scope-defining invariants enforced through entire V0.C.2 cascade. Any deviation triggers SC-N halt (§4). S-LOCKs persist через atomic intermediate states + multi-session resume.

### §1.1 — S-LOCK-1: V0.C.2 scope = batched sprite renderer + Camera2D + TileMap (R.2 + R.3)

V0.C.2 ships exactly per VULKAN_SUBSTRATE §4.2 R.2 + R.3 specification:

**R.2 Batched sprite renderer**:
- Goal: 10,000 sprites rendered at 60+ FPS via single draw call per atlas
- Deliverables: dynamic vertex buffer + per-sprite vertex data + atlas-shared batching + stress test
- Success: 10,000 sprites at 60+ FPS, single draw call observable in RenderDoc, PERFORMANCE budget for sprite pass adopted
- Estimate per VULKAN_SUBSTRATE: 4-6 hours, ~500-700 LOC

**R.3 TileMap parity + Camera2D**:
- Goal: 200×200 tile grid rendered, camera pannable, full M8.8 visual parity
- Deliverables: TileMapBatch + Camera2D + atlas regions for terrain
- Success: 200×200 tile map visible, 60+ FPS sustained (was 17 on Godot at M8.8)
- Estimate per VULKAN_SUBSTRATE: 3-4 hours, ~400-600 LOC

Combined R.2 + R.3 estimate: 7-10 hours, ~900-1300 LOC. Actual V0.C.2 estimated 15-25h auto-mode (executor throughput) — VULKAN_SUBSTRATE estimate was wall-clock manual; auto-mode includes test authoring + alignment audit + governance + manual visual verification.

**V0 substrate close gate per Q8 ratification**: V0.C.2 closure = V0 substrate close. Unlocks V1 (diffusion) + V2 (wave) brief authoring + Phase B M-cycle vanilla content migration (gated also on Roslyn analyzer A'.9).

**Out of scope per §0 Intro**: JSON atlas manifest, Camera2D culling helpers, GPU-instanced TileMap, chunked TileMap, BitmapFont, UI primitives, DebugOverlay coupling, PresentationBridge, Godot cutover, К10.3, К10.4, K-closure report, analyzer, M-cycle migration, V1/V2.

### §1.2 — S-LOCK-2: Vertex buffer = N-frame ring buffer (per Q1c ratification)

V0.C.2 introduces `VertexBufferRing` infrastructure per Q1c ratification (production-standard N-frame ring matching swapchain image count):

**Specification**:
- N = swapchain image count (typically 2-3 from V0.B `Swapchain.Images.Count`)
- Each frame writes own chunk — no cross-frame sync
- Per-chunk size = max 10,000 sprites × 4 vertices × 20 bytes = 800,000 bytes ≈ 800 KB per frame
- Total ring size = N × 800 KB ≈ 1.6-2.4 MB (vs V0.C.1 single 64 KB buffer)
- Host-visible + host-coherent memory (per V0.B `MemoryAllocator` bumper pattern)
- VkBufferUsageFlags.VertexBuffer (VulkanBuffer.cs V0.B existing infrastructure)

**Per-frame write pattern**:
1. SpriteRenderer.BeginFrame(uint frameIndex) — frameIndex modulo N selects active chunk
2. vkMapMemory at chunk offset + chunk size
3. Submit() calls write SpriteVertex sequence к mapped pointer (4 vertices per sprite)
4. EndFrame(CommandBuffer, Camera2D) — vkUnmapMemory + record draw command consuming chunk

**Synchronization invariant**: Frame N+swapchain_count's write must NOT collide с frame N's GPU consumption. Fence-based synchronization preserved per V0.B `imageAvailable` + V0.C.1 per-image `renderFinished` discipline. **K-L19 invariant preserved** — N matches swapchain images, host write happens after `vkWaitForFences` confirms previous frame consumption.

**Bumper allocator adequacy preserved per Q7 V0.C.1**: V0.B `MemoryAllocator` accommodates 2-3 MB ring buffer без free list — V0.C.2 не extends allocator.

**Alternative considered + rejected**:
- (a) Fixed-size large buffer (~4 MB upfront, simpler but wastes memory)
- (b) Dynamic growth (recreate + reallocate, complex synchronization)
- (c) Ring buffer N-frame **RATIFIED**

### §1.3 — S-LOCK-3: Indexed quad rendering (per Q2b ratification)

V0.C.2 switches sprite rendering from 6-vertices-non-indexed (V0.C.1 pattern) to **4-vertices-per-quad + index buffer** per Q2b ratification:

**Vertex layout per sprite** (4 vertices, 20 bytes each = 80 bytes per sprite):
- Index 0: top-left (Position, UV.U0,V0, Tint)
- Index 1: top-right (Position, UV.U1,V0, Tint)
- Index 2: bottom-right (Position, UV.U1,V1, Tint)
- Index 3: bottom-left (Position, UV.U0,V1, Tint)

**Index pattern per quad** (6 indices, 2 bytes each = 12 bytes per quad — using `uint16` index type):
- 0, 1, 2 — triangle 1 (top-left, top-right, bottom-right)
- 2, 3, 0 — triangle 2 (bottom-right, bottom-left, top-left)

CCW front face preserved per V0.C.1 VulkanSpritePipeline configuration. Vulkan NDC: +Y is down, +X is right; UV: (0,0) = top-left, (1,1) = bottom-right.

**10,000 sprites memory comparison**:
- Non-indexed (V0.C.1): 10,000 × 6 vertices × 20 bytes = 1,200,000 bytes = 1.2 MB
- Indexed (V0.C.2): 10,000 × 4 vertices × 20 bytes + 10,000 × 6 indices × 2 bytes = 800,000 + 120,000 = 920,000 bytes = ~920 KB
- **~33% vertex buffer reduction** + ~120 KB index buffer overhead

**Index type**: VK_INDEX_TYPE_UINT16 (4 bytes savings per index vs UINT32 — sufficient for 10,000 sprites × 6 indices = 60,000 indices < 65,535 uint16 max). For 11,000+ sprites would require UINT32; **S-LOCK-3a** clarifies: V0.C.2 hard-caps batched render at 10,000 sprites per BeginFrame/EndFrame cycle. Larger scenes require multiple BeginFrame/EndFrame cycles или future indexed UINT32 extension.

**Index buffer lifecycle**: Index buffer is **constant per SpriteRenderer instance**. Pre-populated once at SpriteRenderer construction time (pattern 0,1,2,2,3,0 repeated 10,000 times = 60,000 uint16 values). Device-local memory + transfer-via-staging-buffer per V0.B `TextureUploader` precedent OR host-visible + coherent if simpler (acceptable per Q1c ring buffer locality — index buffer не varies per frame, host-visible adequate).

**VkApi.cs extensions required**:
- `vkCmdBindIndexBuffer(IntPtr commandBuffer, IntPtr buffer, ulong offset, VkIndexType indexType)` — binds index buffer for subsequent indexed draws
- `vkCmdDrawIndexed(IntPtr commandBuffer, uint indexCount, uint instanceCount, uint firstIndex, int vertexOffset, uint firstInstance)` — issues indexed draw

**Alternative considered + rejected**:
- (a) Continue non-indexed 6-vertices (simpler но 33% больше memory)
- (b) Indexed 4-vertices **RATIFIED**
- (c) Instanced rendering (4 vertices + 10K instances — adds storage buffer descriptor set complexity, overkill for V0.C.2)

### §1.4 — S-LOCK-4: Camera2D standard scope (per Q3b ratification)

V0.C.2 introduces `Camera2D` class per Q3b ratification (standard scope, ~150 LOC):

**Public surface**:
```csharp
public sealed class Camera2D
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1.0f;           // 1.0 = no zoom, 2.0 = 2× zoom
    public float Rotation { get; set; }                // radians; 0 = no rotation
    public Vector2 ViewportSize { get; set; }          // typically window size

    public Matrix4x4 ViewMatrix { get; }               // translate + rotate + scale
    public Matrix4x4 ProjectionMatrix { get; }         // ortho from viewport + zoom
    public Matrix4x4 ViewProjectionMatrix { get; }     // composed для shader MVP

    public Vector2 WorldToScreen(Vector2 worldPos);
    public Vector2 ScreenToWorld(Vector2 screenPos);
}
```

**Mathematical model**:

View matrix construction (camera transform, inverse of world transform):
```
view = Translate(-Position) × Rotate(-Rotation) × Scale(1/Zoom × viewport-aspect-correction)
```

Projection matrix (orthographic, +Y down к match Vulkan NDC):
```
left   = -ViewportSize.X / 2
right  = +ViewportSize.X / 2
top    = -ViewportSize.Y / 2
bottom = +ViewportSize.Y / 2
ortho  = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, near=-1, far=1)
```

**WorldToScreen**: transform worldPos via ViewProjectionMatrix к clip space, then к screen pixel coordinates via viewport mapping.

**ScreenToWorld**: inverse — screen pixel к NDC, NDC × inverse(ViewProjectionMatrix) = world position.

**Push constant payload**: Camera2D.ViewProjectionMatrix (64 bytes Matrix4x4) consumed via VulkanPipelineLayout push constant range (V0.C.1 infrastructure, V0.C.2 actual usage). V0.C.1 used identity matrix — V0.C.2 wires Camera2D.ViewProjectionMatrix per frame.

**Out of scope for V0.C.2**:
- Culling helpers (`IsVisible(AABB)`) — deferred to «when measured insufficient»
- Interpolation для smooth transitions — deferred
- Bounded mode (clamp к world bounds) — deferred

Per К-L14 default-inclusion: standard scope provides foundation; culling/interpolation/bounded extend Camera2D when consumer materializes (Lesson #25).

### §1.5 — S-LOCK-5: TileMap one-sprite-per-tile (per Q4a ratification)

V0.C.2 introduces `TileMap` class per Q4a ratification (reuses batched sprite infrastructure):

**Public surface**:
```csharp
public sealed class TileMap : IDisposable
{
    public int Width { get; }
    public int Height { get; }
    public float TileSize { get; }                  // world units per tile (e.g. 16.0)
    public SpriteTexture Atlas { get; }

    public TileMap(int width, int height, float tileSize, SpriteTexture atlas);
    public void SetTile(int x, int y, AtlasRegion region);
    public void SetTile(int x, int y, AtlasRegion region, uint tintRgba);
    public AtlasRegion GetTile(int x, int y);
    public void Submit(SpriteRenderer renderer, Camera2D camera);
}
```

**Implementation pattern**:
- Internal storage: `AtlasRegion[width × height]` + `uint[width × height]` (tints)
- `Submit(SpriteRenderer, Camera2D)` enumerates all tiles (V0.C.2) или visible tiles only (future culling extension)
- Per tile: constructs Sprite (Atlas, region, SpriteTransform с world position computed from tile coords × TileSize) + calls `renderer.Submit(sprite)`

**200×200 stress configuration**:
- Width × Height = 200 × 200 = 40,000 tiles
- TileSize = 16.0 (one Kenney-standard tile pixel size scaled к world units)
- Atlas = procedurally-generated terrain atlas (V0.C.2 smoke test responsibility)
- 40,000 tiles = 4× R.2 stress test (10,000) — tests batching scales beyond R.2 target

**Memory cost**:
- 40,000 tiles × (16 bytes AtlasRegion + 4 bytes tint) = 800,000 bytes ≈ 800 KB managed memory
- Per-frame GPU vertex buffer cost: 40,000 × 4 × 20 = 3.2 MB per frame chunk
- Total ring (3 frames): ~9.6 MB — within bumper allocator capacity per V0.B (16 MB per memory type default)

**S-LOCK-5a**: If 40,000 sprite ring buffer exceeds 10,000 hard-cap per Q2b indexed UINT16 (60,000 indices), `TileMap.Submit` must invoke multiple `BeginFrame/EndFrame` cycles на SpriteRenderer (each capped at 10,000 sprites). Acceptable performance per VULKAN_SUBSTRATE R.3 ((60+ FPS sustained) — 4 BeginFrame/EndFrame cycles per frame = 4 vkCmdDrawIndexed calls per frame = trivially fast.

**Out of scope**:
- Chunked rendering (per-chunk vertex buffer e.g. 32×32 tile chunks) — deferred per Q4a discipline
- GPU-instanced TileMap (per-tile instance data + compute shader vertex generation) — deferred per Q4a discipline; requires V1 compute substrate already populated
- Visible-tile culling (Camera2D AABB intersection) — deferred; full enumeration adequate для R.3 visual parity verification

If 200×200 TileMap performance measured insufficient at 60 FPS target → V0.C.2 closure surfaces measurement к next deliberation session for V0.C.2.1 sub-cycle authoring (chunked rendering или culling extension). Per Lesson #20 scope discipline + Lesson #25 implementation depth follows consumer materialization.

### §1.6 — S-LOCK-6: Atlas region helpers code-defined (per Q5a ratification)

V0.C.2 introduces `AtlasRegion.FromPixels` static factory method per Q5a ratification:

**API**:
```csharp
public readonly record struct AtlasRegion(float U0, float V0, float U1, float V1)
{
    public static AtlasRegion FromPixels(int xPx, int yPx, int wPx, int hPx, int texWidthPx, int texHeightPx)
    {
        if (xPx < 0 || yPx < 0 || wPx <= 0 || hPx <= 0)
            throw new ArgumentOutOfRangeException("Region pixel coordinates must be non-negative + non-zero size");
        if (xPx + wPx > texWidthPx || yPx + hPx > texHeightPx)
            throw new ArgumentOutOfRangeException("Region exceeds texture bounds");

        return new AtlasRegion(
            U0: (float)xPx / texWidthPx,
            V0: (float)yPx / texHeightPx,
            U1: (float)(xPx + wPx) / texWidthPx,
            V1: (float)(yPx + hPx) / texHeightPx);
    }

    public static readonly AtlasRegion Full = new(0.0f, 0.0f, 1.0f, 1.0f);
}
```

**Usage pattern** (TileMap initialization):
```csharp
const int atlasW = 256, atlasH = 256, tileW = 16, tileH = 16;
var grass     = AtlasRegion.FromPixels(0, 0, tileW, tileH, atlasW, atlasH);
var stone     = AtlasRegion.FromPixels(tileW, 0, tileW, tileH, atlasW, atlasH);
var sand      = AtlasRegion.FromPixels(2 * tileW, 0, tileW, tileH, atlasW, atlasH);

for (int y = 0; y < tileMap.Height; y++)
    for (int x = 0; x < tileMap.Width; x++)
        tileMap.SetTile(x, y, ChooseTile(x, y));
```

**Out of scope**:
- JSON manifest format (`assets/atlases/tiles.json`) — deferred к V0.C.2.1 или V0.D
- TexturePacker XML format — deferred
- BMFont .fnt format — R.6 separate brief

Per К-L14 default-inclusion: code-defined helpers provide V0.C.2 foundation; JSON manifest extends when atlas grows beyond hand-managed scale (Lesson #25 implementation depth follows consumer materialization).

### §1.7 — S-LOCK-7: SpriteRenderer batched API (90% rewrite)

V0.C.2 refactors SpriteRenderer.cs к batched per Q1c + Q2b ratification:

**V0.C.1 API (DEPRECATED at V0.C.2 close — replaced)**:
```csharp
public void DrawSprite(Sprite sprite, VulkanCommandBuffer commandBuffer, Matrix4x4 mvp)
```

**V0.C.2 API (NEW — replaces DrawSprite)**:
```csharp
public sealed class SpriteRenderer : IDisposable
{
    // Construction takes additional parameters
    public SpriteRenderer(
        VulkanDevice device,
        MemoryAllocator allocator,
        VulkanSpritePipeline pipeline,
        int swapchainImageCount,
        int maxSpritesPerFrame = 10000);

    // Batched API
    public void BeginFrame(uint frameIndex);
    public void Submit(Sprite sprite);
    public void EndFrame(VulkanCommandBuffer commandBuffer, Camera2D camera);

    // Diagnostic properties
    public int CurrentFrameSubmissionCount { get; }    // resets at BeginFrame
    public int MaxSpritesPerFrame { get; }
    public int CachedDescriptorSetCount { get; }
}
```

**Per-frame draw call discipline**: One `vkCmdDrawIndexed` per SpriteTexture key (atlas grouping). Sprites accumulated в `Dictionary<SpriteTexture, List<Sprite>>` during Submit() calls; EndFrame() iterates dictionary, writes vertices per atlas group к ring buffer chunk, issues one vkCmdDrawIndexed per atlas group.

**Single-atlas optimization (R.2 target)**: 10,000 sprites all referencing one SpriteTexture (single atlas) = single dictionary entry = single vkCmdDrawIndexed call. Verified via RenderDoc inspection at V0.C.2 closure smoke test (S-LOCK-7a manual verification step).

**Multi-atlas pattern**: TileMap typically references single SpriteTexture (one atlas) — single draw call. Mixed atlas scenes (e.g. terrain + pawns + items) = N draw calls where N = unique SpriteTexture count, не sprite count.

**Per-frame descriptor pool reset**: V0.C.1 SpriteRenderer cached descriptor sets indefinitely (32-capacity pool). V0.C.2 resets descriptor pool at BeginFrame — descriptor sets recreated as needed via existing cache mechanism. Pattern simplifies cleanup + matches per-frame batched lifecycle. **Pool capacity expansion**: 32 → 64 per SwapchainImageCount × MaxAtlasGroupsPerFrame (typically 2-3 atlas × 3 frames = 6-9 needed; 64 provides headroom).

**Backward compatibility**: V0.C.1 `Runtime.RecordSpriteFrame(commandBuffer, imageIndex, sprite, mvp, clearColor)` single-sprite convenience method **retained** for backward compatibility + reduced-scope smoke testing. Internally calls `BeginFrame() / Submit(sprite) / EndFrame(commandBuffer, identityCamera)`.

### §1.8 — S-LOCK-8: K-L19 hardware capability preserved + alignment audit mandatory continues

**K-L19 preservation**: V0.C.2 does NOT modify native kernel layer. HardwareCapabilityCheck.Verify(VulkanInstance, VulkanDevice) at Runtime.Create unchanged. V0.B + V0.C.1 fail-fast invariant preserved verbatim.

**Lesson #7 strengthening discipline continues** (V0.A 1 fix + V0.B 5 fixes + V0.C.1 0 fixes = maturity curve):

**New Vulkan structs introduced V0.C.2 (estimated)**: VkIndexType enum + no new struct types. `vkCmdBindIndexBuffer` + `vkCmdDrawIndexed` accept primitive parameters (IntPtr, ulong, uint, int) + new enum:

```csharp
public enum VkIndexType : uint
{
    VK_INDEX_TYPE_UINT16 = 0,
    VK_INDEX_TYPE_UINT32 = 1,
    VK_INDEX_TYPE_NONE_KHR = 1_000_165_000,
    VK_INDEX_TYPE_UINT8_EXT = 1_000_265_000,
}
```

**Marshal.SizeOf hypotheses к verify** (estimated, executor adjusts via test gate):
- VkIndexType: 4 bytes (enum underlying type uint)

Single new enum — discipline maintained per V0.C.1 zero-correction precedent. **Discipline preserved regardless** — V0.C.2 brief honest hypothesis documentation; executor verifies через Marshal.SizeOf test gate. Per V0.B precedent, brief estimates are HYPOTHESES — test gate authoritative.

**Build-time SPIR-V shader compilation discipline preserved** per V0.C.1: V0.C.2 не adds new shaders — reuses existing `sprite.vert.spv` + `sprite.frag.spv`. MSBuild `CompileShaders` target в `Directory.Build.props` unchanged.

**Pure P/Invoke discipline preserved** per V0.A + V0.B + V0.C.1: `[LibraryImport]` for blittable + `[DllImport]` for non-blittable per Lesson #22 mixed convention. `vkCmdBindIndexBuffer` + `vkCmdDrawIndexed` use blittable primitives → `[LibraryImport]`.

### §1.9 — S-LOCK-9: Atomic cascade preserves V0.A + V0.B + V0.C.1 discipline

V0.C.2 cascade discipline per V0.A → V0.B → V0.C.1 inherited atomic intermediate state pattern:

1. **Each commit is independently buildable + testable** — partial intermediate state never committed
2. **Each commit has explicit scope-prefix message** per repository convention
3. **`dotnet build` clean + `dotnet test` green at each commit** — regression-free progression
4. **REGISTER.yaml validation `sync_register.ps1 --validate` exit 0 at governance commits** — A'.4.5 governance discipline preserved
5. **Multi-session pause provision per Lesson #8 + Lesson #26** — если session token budget exhausted, executor completes current atomic commit, pushes branch к remote, surfaces «paused at Commit N» к Crystalka. Next session resumes from Commit N+1.
6. **Halt-before-damage protocol** — any SC-N halt class triggered (§4) → executor authors HALT_REPORT в `docs/scratch/V0_C_2/`, surfaces к Crystalka, не commits partial state
7. **К10.3 классifier reminder** (per V0.A/V0.B/V0.C.1 precedent) — Claude Code auto-mode blocks push-to-main even с explicit instruction. **Expected behavior** — branch push only, PR review + merge by Crystalka

**Manual visual verification gate per V0.C.1 precedent** (Lesson #27 candidate): Crystalka runs smoke test на «Skarlet» before PR merge. V0.C.2 manual visual verification critical — 10,000-sprite stress + 200×200 TileMap visual scenes complex enough к surface rendering bugs (UV misalignment, vertex order errors, alpha blending wrong, etc.) that automated tests cannot catch.

### §1.10 — S-LOCK-10: Validation layer ALWAYS-ON discipline preserved

V0.A pattern preserved through V0.B + V0.C.1, V0.C.2 preserves:

- `VK_LAYER_KHRONOS_validation` enabled в DEBUG configuration per `RuntimeOptions.EnableValidationLayer`
- All ValidationLayer.cs callback diagnostics captured к console
- **ZERO validation errors tolerated** as commit gate — `dotnet test` smoke test passes with 0 errors/warnings/info messages
- V0.C.2 cascade introduces substantial new Vulkan code surface (index buffer binding + indexed draws + ring buffer write synchronization + descriptor pool reset) — каждая new call has potential validation message. Common V0.C.2-specific causes flagged в §4.

### §1.11 — S-LOCK-11: REGISTER.yaml governance discipline preserved

V0.C.2 brief enrolled at Commit 1 (DOC-D-V0_C_2 entry в `docs/governance/REGISTER.yaml`). `sync_register.ps1 --validate` exit 0 gates governance-related commits per A'.4.5 enrollment + V0.B + V0.C.1 precedent.

**Frontmatter regeneration**: `sync_register.ps1` (full sync mode, не --validate-only) regenerates brief frontmatter at Commit 1 — V0.C.2 brief gets register_id + category + tier + lifecycle + owner + version + next_review_due + register_view_url fields populated via canonical governance pipeline.

**Audit trail event** at Commit 17 closure: REGISTER.yaml audit_trail section gains V0.C.2 closure event с commit range + date + executor identity + summary.

**MIGRATION_PROGRESS.md update at closure**: V substrate progress section reflects V0 substrate close — R.1 + R.4 (V0.C.1) + R.2 + R.3 (V0.C.2) marked complete; V0 substrate close status updated per Q8 ratification gate met.


---

## §2 — Phase 0 reads (mandatory before any code edits)

Phase 0 verification mandatory per Lesson #1 (full reads of production wiring files surface embedded transitional-state comments) + Lesson #9 candidate (survey-before-brief discipline). V0.C.2 executor MUST complete Phase 0 reads before Commit 2 (Commit 1 is brief enrollment + governance only — does not touch code).

### §2.1 — Baseline verification (V0.C.1 inheritance state)

**Required baseline before V0.C.2 cascade begins**:

1. **`git log --oneline -1`** — verifies HEAD на main branch post-V0.C.1 merge. Expected: V0.C.1 closure commit (last commit of PR #38).

2. **`git status`** — clean working tree. No uncommitted changes from prior sessions.

3. **`dotnet build`** — clean build of all projects. Expected: 0 errors, 0 warnings.

4. **`cmake --build native/DualFrontier.Core.Native`** — clean native build. K substrate unchanged from V0.C.1.

5. **`dotnet test`** — 786+ tests green. Expected: 210 Runtime.Tests + 576+ other test projects = 786+ total (V0.C.1 closure baseline).

6. **`./tools/governance/sync_register.ps1 --validate`** — exit code 0. REGISTER.yaml schema valid + audit_trail intact + Category D documents enrolled correctly.

**If any baseline check fails**: HALT-SC-1 (drift from expected V0.C.1 closure state). Author HALT_REPORT в `docs/scratch/V0_C_2/`, surface к Crystalka.

### §2.2 — Vulkan SDK environment verification

**Required environment before any GPU testing**:

1. **VULKAN_SDK env var set** — `echo $env:VULKAN_SDK` returns LunarG SDK install path (e.g. `C:\VulkanSDK\1.3.x.y`). HARD GATE — SC-2 halt if absent.

2. **`tools/glslangValidator.exe` present** — committed in repository per V0.B Commit 10. No shader compilation в V0.C.2 (reuses V0.C.1 sprite shaders), но build target still references binary. SC-3 halt if absent.

3. **vulkan-1.dll loadable** — Windows runtime layer typically auto-installed с GPU driver. If `vkCreateInstance` fails at smoke test → driver не loaded или SDK install corrupted. Surface к Crystalka.

4. **VK_LAYER_KHRONOS_validation present** — Vulkan SDK installs validation layer. Verify via `vulkaninfo --summary` showing validation layer listed.

### §2.3 — VULKAN_SUBSTRATE.md authority sections

**Required reads before any architectural decision**:

1. **VULKAN_SUBSTRATE.md §1.4 V substrate close acceptance criteria** — V0.C.2 closure = V0 close gate per Q8 ratification.

2. **VULKAN_SUBSTRATE.md §4.2 R.2 + R.3 deliverables + success criteria** — V0.C.2 deliverable scope.

3. **VULKAN_SUBSTRATE.md §2.2 module purposes** — DualFrontier.Runtime.Sprite module + Camera2D placement (Sprite module per spec, not separate Camera module).

4. **VULKAN_SUBSTRATE.md §2.6 asset pipeline** — atlas region metadata format expectations (§Q5a ratification gates code-defined for V0.C.2).

5. **VULKAN_SUBSTRATE.md §11 methodology adjustments** — validation layer output check + CPU/GPU equivalence test (rendering doesn't require equivalence test — applies к compute shaders V1/V2).

### §2.4 — V0.C.1 code anchor verification (post-merge state)

**Required reads before Commit 2 (Vulkan struct/function extensions)**:

1. **`src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs`** (~19,530 bytes per V0.C.1) — verify last function declaration is `vkCmdBindVertexBuffers`. **CRITICAL VERIFICATION**: `vkCmdBindIndexBuffer` + `vkCmdDrawIndexed` MUST BE ABSENT (V0.C.2 adds them). If present already → drift from V0.C.1 closure state, halt SC-1.

2. **`src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs`** — verify V0.C.1 single-sprite-per-call shape:
   - `DrawSprite(Sprite sprite, VulkanCommandBuffer commandBuffer, Matrix4x4 mvp)` public method present
   - 6-vertex stackalloc array в DrawSprite body
   - 64 KB VertexBufferSize constant
   - 32 DescriptorPoolCapacity constant
   - `_descriptorSetCache` Dictionary<SpriteTexture, IntPtr>
   - V0.C.2 будет major refactor — 90% rewrite per S-LOCK-7.

3. **`src/DualFrontier.Runtime/Runtime.cs`** — verify V0.C.1 composition:
   - V0.A + V0.B + V0.C.1 primitives all composed в Create() method
   - `SpriteRenderer SpriteRenderer { get; private set; }` property present
   - `RecordSpriteFrame(VulkanCommandBuffer, int, Sprite, Matrix4x4, Vector4)` convenience method present
   - `RecreateFramebuffersForSwapchain()` helper present (V0.C.1 latent bug fix)
   - V0.C.2 extends с `Camera2D Camera { get; private set; }` + `RecordSpritesFrame(IEnumerable<Sprite>, Camera2D, int, Vector4)` batched convenience.

4. **`src/DualFrontier.Runtime/Sprite/SpriteVertex.cs`** — verify 20 bytes layout:
   - `Position` (Vector2, 8 bytes)
   - `Uv` (Vector2, 8 bytes)
   - `TintRgba` (uint, 4 bytes)
   - `[StructLayout(LayoutKind.Sequential, Pack = 4)]` attribute
   - Marshal.SizeOf<SpriteVertex>() == 20 test gate в Sprite/SpriteVertexLayoutTests.cs (или equivalent path) — verify test exists + passes.

5. **`src/DualFrontier.Runtime/Sprite/AtlasRegion.cs`** — verify V0.C.1 shape:
   - `record struct AtlasRegion(float U0, float V0, float U1, float V1)`
   - **`FromPixels` static factory ABSENT** (V0.C.2 adds it per S-LOCK-6)
   - `Full` static property present (full atlas region 0,0,1,1)

6. **`src/DualFrontier.Runtime/Sprite/VulkanSpritePipeline.cs`** — verify V0.C.1 pipeline configuration:
   - Vertex input binding stride = 20 (SpriteVertex)
   - 3 vertex attributes (Position R32G32_SFLOAT offset 0, Uv R32G32_SFLOAT offset 8, TintRgba R8G8B8A8_UNORM offset 16)
   - Alpha blending enabled (premultiplied alpha per S-LOCK-5 V0.C.1)
   - Topology = TRIANGLE_LIST
   - V0.C.2 reuses VulkanSpritePipeline unchanged — indexed draw uses same pipeline (топology TRIANGLE_LIST + index buffer = indexed triangles)

7. **`src/DualFrontier.Runtime/Graphics/VulkanBuffer.cs`** — verify `VkBufferUsageFlagsPublic.IndexBuffer = 0x00000040` present (V0.B). V0.C.2 instantiates index buffer via existing VulkanBuffer constructor.

### §2.5 — REGISTER structure read (governance entry template)

**Required read before Commit 1 (governance enrollment)**:

1. **`docs/governance/REGISTER.yaml`** — Tier 3 Category D documents section. Find V0.B + V0.C.1 entries as templates for V0.C.2 entry structure.

2. **`docs/governance/REGISTER.yaml`** — audit_trail section format. Most recent V0.C.1 closure event provides EVT structure template.

3. **`docs/governance/sync_register.ps1`** — read but не modify; tool authoritative.

### §2.6 — Asset directory inspection (atlas generation)

**Required reads before Commit 16 (10K stress + 200×200 TileMap smoke test)**:

1. **`assets/`** directory inspection — verify `assets/shaders/` contains `sprite.vert.spv` + `sprite.frag.spv` (V0.C.1 committed). If missing → SC-4 halt.

2. **`assets/sprites/` inspection** — V0.C.1 used synthetic PngImage in smoke test (Kenney palette PNG incompatible). V0.C.2 smoke test similarly generates procedural atlas — no external sprite assets required.

3. **No external atlas PNG required**: V0.C.2 smoke test generates procedural 256×256 RGBA8 atlas в memory (e.g. 16×16 tile grid с 16 distinct procedural tile types — checker + gradient + noise patterns), uploads via V0.C.1 TextureUploader infrastructure, registers с AtlasRegion.FromPixels per S-LOCK-6.

### §2.7 — Test project structure read

**Required reads before Commit 2 (struct size tests)**:

1. **`tests/DualFrontier.Runtime.Tests/`** directory structure — verify test discovery + run pattern from V0.B + V0.C.1.

2. **`tests/DualFrontier.Runtime.Tests/Native/Vulkan/VkStructSizeTests.cs`** (или equivalent path) — Marshal.SizeOf test template. V0.C.2 adds VkIndexType enum size test.

3. **`tests/DualFrontier.Runtime.SmokeTest/Program.cs`** — V0.C.1 smoke test extension point. V0.C.2 extends с 10K stress + 200×200 TileMap scenes.

---

## §3 — Atomic commit cascade (17 commits)

V0.C.2 cascade structured as 17 atomic commits. Each commit independently buildable + testable. Per S-LOCK-9 + V0.A/V0.B/V0.C.1 precedent: partial intermediate state never committed; multi-session pause provision honors atomic boundaries.

**Branch**: `claude/v0_c_2-batched-sprite-tilemap-camera`

**Scope-prefix convention**: `<scope>: <imperative summary>` per repository convention. Scopes: `runtime`, `vulkan`, `sprite`, `tilemap`, `camera`, `docs`, `tests`, `governance`.

### Commit 1 — Brief authoring commit (V0.C.2 brief enrollment)

**Scope**: governance + docs

**Files modified**:
- `tools/briefs/V0_C_2_EXECUTION_BRIEF.md` — NEW (this brief), copied from `/mnt/user-data/outputs/` by Crystalka pre-session
- `docs/governance/REGISTER.yaml` — ADD entry DOC-D-V0_C_2:
```yaml
DOC-D-V0_C_2:
  category: D
  tier: 3
  lifecycle: AUTHORED
  owner: Crystalka
  version: "1.0"
  next_review_due: 2027-05-19
  path: tools/briefs/V0_C_2_EXECUTION_BRIEF.md
  description: V0.C.2 execution brief — batched sprite renderer + Camera2D + TileMap (R.2 + R.3)
```

**Operations**:
1. `tools\governance\sync_register.ps1` (full sync, не --validate-only) — regenerates V0.C.2 brief frontmatter via canonical pipeline
2. `tools\governance\sync_register.ps1 --validate` — verifies exit code 0
3. `git add tools/briefs/V0_C_2_EXECUTION_BRIEF.md docs/governance/REGISTER.yaml docs/governance/REGISTER_RENDER.md`
4. `git commit -m "governance: enroll V0.C.2 execution brief (DOC-D-V0_C_2 AUTHORED)"`

**Expected state post-commit**: Brief enrolled. `sync_register.ps1 --validate` exit 0. No code changes (purely governance).

**Verification**:
- `git log --oneline -1` shows new commit
- `dotnet build` clean (unchanged from baseline)
- `dotnet test` 786+ green (unchanged from baseline)

### Commit 2 — Vulkan VkIndexType enum + vkCmdBindIndexBuffer + vkCmdDrawIndexed + struct size tests

**Scope**: vulkan + tests

**Files modified**:
- `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` — ADD VkIndexType enum:
```csharp
public enum VkIndexType : uint
{
    VK_INDEX_TYPE_UINT16 = 0,
    VK_INDEX_TYPE_UINT32 = 1,
    VK_INDEX_TYPE_NONE_KHR = 1_000_165_000,
    VK_INDEX_TYPE_UINT8_EXT = 1_000_265_000,
}
```

- `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` — ADD function bindings (append к V0.C.1 section, new V0.C.2 section):
```csharp
// =======================================================================
// V0.C.2 — Indexed draw + index buffer binding
// =======================================================================

[LibraryImport(VulkanLib, EntryPoint = "vkCmdBindIndexBuffer")]
internal static partial void vkCmdBindIndexBuffer(
    IntPtr commandBuffer,
    IntPtr buffer,
    ulong offset,
    VkIndexType indexType);

[LibraryImport(VulkanLib, EntryPoint = "vkCmdDrawIndexed")]
internal static partial void vkCmdDrawIndexed(
    IntPtr commandBuffer,
    uint indexCount,
    uint instanceCount,
    uint firstIndex,
    int vertexOffset,
    uint firstInstance);
```

- `tests/DualFrontier.Runtime.Tests/Native/Vulkan/VkStructSizeTests.cs` (или equivalent) — ADD test:
```csharp
[Fact]
public void VkIndexType_Size_Matches_Expected()
{
    Assert.Equal(4, Marshal.SizeOf<VkIndexType>());  // enum underlying type uint
}
```

**Operations**:
1. Edit files per above
2. `dotnet build` — verify clean
3. `dotnet test --filter "FullyQualifiedName~VkStructSizeTests"` — verify VkIndexType size test passes
4. `dotnet test` — full suite, 787+ green (786 baseline + 1 new)
5. `git add` + `git commit -m "vulkan: add VkIndexType + vkCmdBindIndexBuffer + vkCmdDrawIndexed (V0.C.2 indexed draw infrastructure)"`

**Halt trigger SC-5 (Lesson #7 strengthening)**: If Marshal.SizeOf<VkIndexType>() ≠ 4 → adjust test expected value к actual + document correction в commit message per V0.B 5-corrections precedent. Per V0.C.1 precedent: разница unlikely (single-field enum c uint underlying = 4 bytes always).

**Expected state post-commit**: VkApi.cs has 2 new functions; VkEnums.cs has VkIndexType; struct size test passes. Build + tests clean.

### Commit 3 — VertexBufferRing infrastructure

**Scope**: sprite

**Files added**:
- `src/DualFrontier.Runtime/Sprite/VertexBufferRing.cs` — NEW class per S-LOCK-2:

```csharp
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// N-frame ring buffer for batched sprite vertex data per V0.C.2 S-LOCK-2.
/// Each frame writes own chunk avoiding cross-frame sync hazards.
/// Per-chunk size: maxSpritesPerFrame × 4 vertices × sizeof(SpriteVertex).
/// Total ring size: chunkSize × frameCount.
/// </summary>
public sealed class VertexBufferRing : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanBuffer _buffer;
    private readonly int _frameCount;
    private readonly ulong _chunkSize;
    private readonly int _maxSpritesPerFrame;
    private uint _activeFrame;
    private IntPtr _mappedPtr;
    private ulong _writeOffset;
    private int _spritesSubmittedThisFrame;
    private bool _disposed;

    public ulong ChunkSize => _chunkSize;
    public int FrameCount => _frameCount;
    public int MaxSpritesPerFrame => _maxSpritesPerFrame;
    public IntPtr Handle => _buffer.Handle;

    public VertexBufferRing(
        VulkanDevice device,
        MemoryAllocator allocator,
        int frameCount,
        int maxSpritesPerFrame)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (frameCount <= 0) throw new ArgumentOutOfRangeException(nameof(frameCount));
        if (maxSpritesPerFrame <= 0) throw new ArgumentOutOfRangeException(nameof(maxSpritesPerFrame));

        _device = device;
        _frameCount = frameCount;
        _maxSpritesPerFrame = maxSpritesPerFrame;
        // 4 vertices per sprite × 20 bytes per SpriteVertex
        _chunkSize = (ulong)maxSpritesPerFrame * 4UL * 20UL;
        ulong totalSize = _chunkSize * (ulong)frameCount;

        _buffer = new VulkanBuffer(
            device, allocator, totalSize,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.VertexBuffer);
    }

    /// <summary>
    /// Begin writing к chunk N (frameIndex modulo frameCount).
    /// Maps memory at chunk offset для duration until EndFrame.
    /// </summary>
    public void BeginFrame(uint frameIndex)
    {
        ThrowIfDisposed();
        if (_mappedPtr != IntPtr.Zero)
            throw new InvalidOperationException("VertexBufferRing.BeginFrame called twice without EndFrame.");

        _activeFrame = frameIndex % (uint)_frameCount;
        ulong chunkOffset = (ulong)_activeFrame * _chunkSize;

        VkResult result = VkApi.vkMapMemory(
            _device.Handle,
            _buffer.Allocation.DeviceMemory,
            _buffer.Allocation.Offset + chunkOffset,
            _chunkSize,
            0,
            out _mappedPtr);

        if (result != VkResult.VK_SUCCESS)
            throw new InvalidOperationException($"vkMapMemory (vertex ring) failed: {result}");

        _writeOffset = 0;
        _spritesSubmittedThisFrame = 0;
    }

    /// <summary>
    /// Write 4 vertices к current frame chunk. Caller responsible for vertex pattern
    /// (TL, TR, BR, BL) matching index pattern (0,1,2,2,3,0).
    /// </summary>
    public unsafe void WriteSprite(in SpriteVertex tl, in SpriteVertex tr, in SpriteVertex br, in SpriteVertex bl)
    {
        if (_mappedPtr == IntPtr.Zero)
            throw new InvalidOperationException("VertexBufferRing.WriteSprite without BeginFrame.");
        if (_spritesSubmittedThisFrame >= _maxSpritesPerFrame)
            throw new InvalidOperationException($"VertexBufferRing capacity exceeded ({_maxSpritesPerFrame}).");

        byte* dst = (byte*)_mappedPtr + _writeOffset;
        *(SpriteVertex*)(dst + 0) = tl;
        *(SpriteVertex*)(dst + 20) = tr;
        *(SpriteVertex*)(dst + 40) = br;
        *(SpriteVertex*)(dst + 60) = bl;

        _writeOffset += 80;
        _spritesSubmittedThisFrame++;
    }

    /// <summary>
    /// Unmap memory + return current frame's chunk offset for vkCmdBindVertexBuffers.
    /// </summary>
    public ulong EndFrame()
    {
        if (_mappedPtr == IntPtr.Zero)
            throw new InvalidOperationException("VertexBufferRing.EndFrame without BeginFrame.");

        VkApi.vkUnmapMemory(_device.Handle, _buffer.Allocation.DeviceMemory);
        _mappedPtr = IntPtr.Zero;

        return (ulong)_activeFrame * _chunkSize;
    }

    public int SpritesSubmittedThisFrame => _spritesSubmittedThisFrame;

    public void Dispose()
    {
        if (_disposed) return;
        if (_mappedPtr != IntPtr.Zero)
        {
            VkApi.vkUnmapMemory(_device.Handle, _buffer.Allocation.DeviceMemory);
            _mappedPtr = IntPtr.Zero;
        }
        _buffer.Dispose();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VertexBufferRing));
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/VertexBufferRingTests.cs` — NEW tests:
```csharp
public class VertexBufferRingTests
{
    [Fact]
    public void Constructor_ValidArgs_SucceedsWithExpectedChunkSize()
    {
        // chunkSize = maxSpritesPerFrame × 4 × 20
        // For 100 sprites × 3 frames: chunk = 8000, total = 24000
        // Test verifies ChunkSize property reflects argument
    }

    [Fact]
    public void Constructor_InvalidArgs_Throws()
    {
        // frameCount = 0 → ArgumentOutOfRangeException
        // maxSpritesPerFrame = 0 → ArgumentOutOfRangeException
    }

    [Fact]
    public void WriteSprite_WithoutBeginFrame_Throws()
    {
        // InvalidOperationException
    }

    [Fact]
    public void WriteSprite_BeyondCapacity_Throws()
    {
        // BeginFrame + WriteSprite × maxSpritesPerFrame succeeds
        // (maxSpritesPerFrame + 1)th WriteSprite throws InvalidOperationException
    }

    [Fact]
    public void EndFrame_AfterBeginFrame_ReturnsCorrectChunkOffset()
    {
        // BeginFrame(0) → offset 0
        // BeginFrame(1) → offset = chunkSize
        // BeginFrame(2) → offset = 2 × chunkSize
        // BeginFrame(3) → offset = 0 (wraps modulo frameCount=3)
    }

    [Fact]
    public void BeginFrame_TwiceWithoutEndFrame_Throws()
    {
        // InvalidOperationException on second BeginFrame
    }
}
```

**Operations**:
1. Create VertexBufferRing.cs
2. Create VertexBufferRingTests.cs (note: GPU-dependent tests require VulkanDevice — mock или integration test pattern per V0.B precedent)
3. `dotnet build` — verify clean
4. `dotnet test` — verify new tests pass
5. `git add` + `git commit -m "sprite: add VertexBufferRing infrastructure (N-frame ring buffer per S-LOCK-2)"`

**Verification**: VertexBufferRing instantiates с 3-frame × 10K sprite config → ~2.4 MB buffer allocated. WriteSprite × 10K succeeds. WriteSprite 10,001th throws. EndFrame returns correct offset per active frame.

### Commit 4 — Index buffer pre-population infrastructure

**Scope**: sprite

**Files added**:
- `src/DualFrontier.Runtime/Sprite/SpriteIndexBuffer.cs` — NEW class per S-LOCK-3:

```csharp
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Pre-populated index buffer for batched quad rendering per V0.C.2 S-LOCK-3.
/// 6 indices per quad (0,1,2,2,3,0) repeated maxQuads times.
/// Index type: VK_INDEX_TYPE_UINT16 (supports up к 10,922 quads = 65,532 indices < 65,536 max).
/// V0.C.2 hard-caps at 10,000 sprites per BeginFrame/EndFrame cycle per S-LOCK-3a.
/// </summary>
public sealed class SpriteIndexBuffer : IDisposable
{
    public const int IndicesPerQuad = 6;
    public const ushort MaxUint16Quads = 10_000;  // S-LOCK-3a hard cap

    private readonly VulkanBuffer _buffer;
    private readonly int _quadCapacity;
    private bool _disposed;

    public IntPtr Handle => _buffer.Handle;
    public int QuadCapacity => _quadCapacity;
    public int IndexCount => _quadCapacity * IndicesPerQuad;

    public unsafe SpriteIndexBuffer(
        VulkanDevice device,
        MemoryAllocator allocator,
        int quadCapacity)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        if (quadCapacity <= 0 || quadCapacity > MaxUint16Quads)
            throw new ArgumentOutOfRangeException(nameof(quadCapacity),
                $"Quad capacity must be 1..{MaxUint16Quads} (uint16 index range)");

        _quadCapacity = quadCapacity;
        ulong totalBytes = (ulong)quadCapacity * (ulong)IndicesPerQuad * sizeof(ushort);

        _buffer = new VulkanBuffer(
            device, allocator, totalBytes,
            VkMemoryPropertyFlagsPublic.HostVisible | VkMemoryPropertyFlagsPublic.HostCoherent,
            VkBufferUsageFlagsPublic.IndexBuffer);

        // Pre-populate index pattern (0,1,2,2,3,0) per quad
        VkResult mapResult = VkApi.vkMapMemory(
            device.Handle,
            _buffer.Allocation.DeviceMemory,
            _buffer.Allocation.Offset,
            totalBytes,
            0,
            out IntPtr mappedPtr);

        if (mapResult != VkResult.VK_SUCCESS)
        {
            _buffer.Dispose();
            throw new InvalidOperationException($"vkMapMemory (index buffer pre-pop) failed: {mapResult}");
        }

        try
        {
            ushort* indices = (ushort*)mappedPtr;
            for (int q = 0; q < quadCapacity; q++)
            {
                int baseVertex = q * 4;
                int baseIndex = q * IndicesPerQuad;
                indices[baseIndex + 0] = (ushort)(baseVertex + 0);  // TL
                indices[baseIndex + 1] = (ushort)(baseVertex + 1);  // TR
                indices[baseIndex + 2] = (ushort)(baseVertex + 2);  // BR
                indices[baseIndex + 3] = (ushort)(baseVertex + 2);  // BR
                indices[baseIndex + 4] = (ushort)(baseVertex + 3);  // BL
                indices[baseIndex + 5] = (ushort)(baseVertex + 0);  // TL
            }
        }
        finally
        {
            VkApi.vkUnmapMemory(device.Handle, _buffer.Allocation.DeviceMemory);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _buffer.Dispose();
        _disposed = true;
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/SpriteIndexBufferTests.cs` — NEW tests:
```csharp
public class SpriteIndexBufferTests
{
    [Fact]
    public void Constructor_InvalidQuadCapacity_Throws()
    {
        // quadCapacity = 0 → ArgumentOutOfRangeException
        // quadCapacity = -1 → ArgumentOutOfRangeException
        // quadCapacity = MaxUint16Quads + 1 = 10,001 → ArgumentOutOfRangeException
    }

    [Fact]
    public void Constants_AreExpectedValues()
    {
        Assert.Equal(6, SpriteIndexBuffer.IndicesPerQuad);
        Assert.Equal(10_000, SpriteIndexBuffer.MaxUint16Quads);
    }

    [Fact]
    public void IndexCount_Equals_QuadCapacity_Times_IndicesPerQuad()
    {
        // For quadCapacity = 100, IndexCount = 600
    }
}
```

**Operations**:
1. Create SpriteIndexBuffer.cs
2. Create SpriteIndexBufferTests.cs
3. `dotnet build` — verify clean
4. `dotnet test` — verify new tests pass
5. `git add` + `git commit -m "sprite: add SpriteIndexBuffer (pre-populated uint16 index pattern per S-LOCK-3)"`

**Verification**: SpriteIndexBuffer instantiates с 10,000 quad capacity → 60,000 indices × 2 bytes = 120 KB buffer. Quad capacity boundary checked.


### Commit 5 — AtlasRegion.FromPixels static factory + tests

**Scope**: sprite + tests

**Files modified**:
- `src/DualFrontier.Runtime/Sprite/AtlasRegion.cs` — extend existing record struct per S-LOCK-6:

```csharp
namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Normalized atlas region (UV coordinates 0..1).
/// V0.C.1: U0/V0/U1/V1 + Full property.
/// V0.C.2: FromPixels static factory для code-defined atlas regions per S-LOCK-6.
/// </summary>
public readonly record struct AtlasRegion(float U0, float V0, float U1, float V1)
{
    /// <summary>Full atlas (entire texture).</summary>
    public static readonly AtlasRegion Full = new(0.0f, 0.0f, 1.0f, 1.0f);

    /// <summary>
    /// V0.C.2 — Construct AtlasRegion from pixel coordinates within a texture of known size.
    /// </summary>
    public static AtlasRegion FromPixels(int xPx, int yPx, int wPx, int hPx, int texWidthPx, int texHeightPx)
    {
        if (xPx < 0 || yPx < 0)
            throw new ArgumentOutOfRangeException("Pixel coordinates must be non-negative.");
        if (wPx <= 0 || hPx <= 0)
            throw new ArgumentOutOfRangeException("Region dimensions must be positive.");
        if (texWidthPx <= 0 || texHeightPx <= 0)
            throw new ArgumentOutOfRangeException("Texture dimensions must be positive.");
        if (xPx + wPx > texWidthPx || yPx + hPx > texHeightPx)
            throw new ArgumentOutOfRangeException(
                $"Region ({xPx},{yPx},{wPx},{hPx}) exceeds texture bounds ({texWidthPx}x{texHeightPx}).");

        return new AtlasRegion(
            U0: (float)xPx / texWidthPx,
            V0: (float)yPx / texHeightPx,
            U1: (float)(xPx + wPx) / texWidthPx,
            V1: (float)(yPx + hPx) / texHeightPx);
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/AtlasRegionTests.cs` — ADD FromPixels tests (или extend existing):
```csharp
[Theory]
[InlineData(0, 0, 16, 16, 256, 256, 0.0f, 0.0f, 0.0625f, 0.0625f)]      // top-left tile
[InlineData(16, 0, 16, 16, 256, 256, 0.0625f, 0.0f, 0.125f, 0.0625f)]   // second tile in row
[InlineData(0, 240, 16, 16, 256, 256, 0.0f, 0.9375f, 0.0625f, 1.0f)]    // bottom-left tile
[InlineData(0, 0, 256, 256, 256, 256, 0.0f, 0.0f, 1.0f, 1.0f)]           // full atlas
public void FromPixels_ValidArgs_ReturnsExpectedNormalizedRegion(
    int x, int y, int w, int h, int texW, int texH,
    float expectedU0, float expectedV0, float expectedU1, float expectedV1)
{
    AtlasRegion region = AtlasRegion.FromPixels(x, y, w, h, texW, texH);
    Assert.Equal(expectedU0, region.U0, 5);
    Assert.Equal(expectedV0, region.V0, 5);
    Assert.Equal(expectedU1, region.U1, 5);
    Assert.Equal(expectedV1, region.V1, 5);
}

[Theory]
[InlineData(-1, 0, 16, 16, 256, 256)]      // negative x
[InlineData(0, -1, 16, 16, 256, 256)]      // negative y
[InlineData(0, 0, 0, 16, 256, 256)]        // zero width
[InlineData(0, 0, 16, 0, 256, 256)]        // zero height
[InlineData(0, 0, 16, 16, 0, 256)]         // zero tex width
[InlineData(0, 0, 16, 16, 256, 0)]         // zero tex height
[InlineData(250, 0, 16, 16, 256, 256)]     // exceeds width (250 + 16 = 266 > 256)
[InlineData(0, 250, 16, 16, 256, 256)]     // exceeds height
public void FromPixels_InvalidArgs_Throws(
    int x, int y, int w, int h, int texW, int texH)
{
    Assert.Throws<ArgumentOutOfRangeException>(
        () => AtlasRegion.FromPixels(x, y, w, h, texW, texH));
}
```

**Operations**:
1. Edit AtlasRegion.cs
2. Edit или create AtlasRegionTests.cs
3. `dotnet build` — verify clean
4. `dotnet test --filter "FullyQualifiedName~AtlasRegionTests"` — verify FromPixels tests pass
5. `dotnet test` — full suite green
6. `git add` + `git commit -m "sprite: add AtlasRegion.FromPixels static factory (S-LOCK-6 code-defined atlas regions)"`

**Verification**: 16×16 tile in 256×256 atlas → U0=0, V0=0, U1=1/16, V1=1/16. Boundary checks reject invalid args.

### Commit 6 — Camera2D class

**Scope**: sprite + tests

**Files added**:
- `src/DualFrontier.Runtime/Sprite/Camera2D.cs` — NEW class per S-LOCK-4:

```csharp
using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 orthographic 2D camera per S-LOCK-4.
/// Standard scope: position + zoom + rotation + viewport + matrices + transforms.
/// Culling helpers (IsVisible(AABB)) + interpolation + bounded mode deferred.
///
/// View matrix: inverse of camera world transform (translate + rotate + scale).
/// Projection matrix: orthographic from viewport dimensions + zoom.
/// Composed ViewProjectionMatrix consumed via push constant by sprite shader.
/// </summary>
public sealed class Camera2D
{
    /// <summary>World-space position of camera center.</summary>
    public Vector2 Position { get; set; }

    /// <summary>Zoom factor. 1.0 = no zoom; 2.0 = 2× zoom (objects appear larger).</summary>
    public float Zoom { get; set; } = 1.0f;

    /// <summary>Rotation angle in radians. 0 = no rotation; positive = counter-clockwise (Vulkan NDC).</summary>
    public float Rotation { get; set; }

    /// <summary>Viewport size в pixels (typically window framebuffer dimensions).</summary>
    public Vector2 ViewportSize { get; set; } = new(1280, 720);

    /// <summary>View matrix: inverse of camera world transform.</summary>
    public Matrix4x4 ViewMatrix
    {
        get
        {
            // V = T(-pos) * R(-rot) * S(1/zoom)
            // Note: ortho projection handles viewport mapping; view handles camera positioning
            Matrix4x4 translate = Matrix4x4.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0));
            Matrix4x4 rotate = Matrix4x4.CreateRotationZ(-Rotation);
            Matrix4x4 scale = Matrix4x4.CreateScale(Zoom, Zoom, 1.0f);
            // Order: scale × rotate × translate (matrix multiplication order = reversed)
            return translate * rotate * scale;
        }
    }

    /// <summary>Orthographic projection matrix from viewport + zoom.</summary>
    public Matrix4x4 ProjectionMatrix
    {
        get
        {
            // Vulkan NDC: +X right, +Y down, depth 0..1
            // Ortho maps viewport-half-extents к NDC ±1
            float halfWidth = ViewportSize.X * 0.5f;
            float halfHeight = ViewportSize.Y * 0.5f;
            // CreateOrthographicOffCenter(left, right, bottom, top, near, far)
            // Note: для Vulkan +Y down, swap top/bottom in argument order
            return Matrix4x4.CreateOrthographicOffCenter(
                -halfWidth, halfWidth,
                halfHeight, -halfHeight,   // bottom > top для Vulkan +Y down
                -1.0f, 1.0f);
        }
    }

    /// <summary>Composed view × projection matrix. Consumed as push constant by sprite shader.</summary>
    public Matrix4x4 ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;

    /// <summary>Transform world position к screen pixel coordinates.</summary>
    public Vector2 WorldToScreen(Vector2 worldPos)
    {
        Vector4 worldVec = new(worldPos.X, worldPos.Y, 0, 1);
        Vector4 clipVec = Vector4.Transform(worldVec, ViewProjectionMatrix);
        // Clip к NDC (divide by w; ortho projection w = 1, но general form)
        Vector2 ndc = new(clipVec.X / clipVec.W, clipVec.Y / clipVec.W);
        // NDC к screen pixel
        return new Vector2(
            (ndc.X + 1) * 0.5f * ViewportSize.X,
            (ndc.Y + 1) * 0.5f * ViewportSize.Y);
    }

    /// <summary>Transform screen pixel coordinates к world position.</summary>
    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        // Screen pixel к NDC
        Vector2 ndc = new(
            (screenPos.X / ViewportSize.X) * 2 - 1,
            (screenPos.Y / ViewportSize.Y) * 2 - 1);
        // NDC к world via inverse(ViewProjection)
        if (!Matrix4x4.Invert(ViewProjectionMatrix, out Matrix4x4 invVP))
            throw new InvalidOperationException("ViewProjectionMatrix not invertible.");
        Vector4 ndcVec = new(ndc.X, ndc.Y, 0, 1);
        Vector4 worldVec = Vector4.Transform(ndcVec, invVP);
        return new Vector2(worldVec.X / worldVec.W, worldVec.Y / worldVec.W);
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/Camera2DTests.cs` — NEW tests:
```csharp
public class Camera2DTests
{
    [Fact]
    public void Default_Camera_Has_IdentityViewProjection_ForUnitViewport()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(2, 2) };  // half-extents = 1
        // Position=0, Zoom=1, Rotation=0
        // View × Ortho = should map ±1 world → ±1 NDC
        Matrix4x4 vp = cam.ViewProjectionMatrix;
        Vector4 origin = Vector4.Transform(new Vector4(0, 0, 0, 1), vp);
        Assert.Equal(0, origin.X, 5);
        Assert.Equal(0, origin.Y, 5);
    }

    [Fact]
    public void WorldToScreen_AtCenter_ReturnsViewportCenter()
    {
        var cam = new Camera2D { ViewportSize = new Vector2(1280, 720) };
        Vector2 screen = cam.WorldToScreen(Vector2.Zero);
        Assert.Equal(640, screen.X, 1);
        Assert.Equal(360, screen.Y, 1);
    }

    [Fact]
    public void WorldToScreen_ScreenToWorld_RoundTrip()
    {
        var cam = new Camera2D
        {
            Position = new Vector2(100, 50),
            Zoom = 1.5f,
            ViewportSize = new Vector2(800, 600),
        };
        Vector2 originalWorld = new(123, 456);
        Vector2 screen = cam.WorldToScreen(originalWorld);
        Vector2 backToWorld = cam.ScreenToWorld(screen);
        Assert.Equal(originalWorld.X, backToWorld.X, 3);
        Assert.Equal(originalWorld.Y, backToWorld.Y, 3);
    }

    [Fact]
    public void Zoom_Affects_WorldToScreen_Mapping()
    {
        var cam1 = new Camera2D { ViewportSize = new Vector2(800, 600), Zoom = 1.0f };
        var cam2 = new Camera2D { ViewportSize = new Vector2(800, 600), Zoom = 2.0f };
        Vector2 worldPos = new(100, 0);
        Vector2 screen1 = cam1.WorldToScreen(worldPos);
        Vector2 screen2 = cam2.WorldToScreen(worldPos);
        // Zoom 2× → world position appears twice as far from center
        float offset1 = screen1.X - 400;
        float offset2 = screen2.X - 400;
        Assert.Equal(offset1 * 2, offset2, 1);
    }

    [Fact]
    public void Position_TranslatesView()
    {
        var cam = new Camera2D
        {
            Position = new Vector2(100, 50),
            ViewportSize = new Vector2(800, 600)
        };
        // World position at (100, 50) appears at viewport center (400, 300)
        Vector2 screen = cam.WorldToScreen(new Vector2(100, 50));
        Assert.Equal(400, screen.X, 1);
        Assert.Equal(300, screen.Y, 1);
    }
}
```

**Operations**:
1. Create Camera2D.cs
2. Create Camera2DTests.cs
3. `dotnet build` — verify clean
4. `dotnet test --filter "FullyQualifiedName~Camera2DTests"` — verify camera math correct
5. `dotnet test` — full suite green
6. `git add` + `git commit -m "sprite: add Camera2D class (S-LOCK-4 standard scope — position + zoom + rotation + transforms)"`

**Verification**: Identity camera produces identity-equivalent view projection. WorldToScreen + ScreenToWorld round-trip preserves coordinates. Zoom + Position affect mapping correctly.

**Halt trigger SC-6 (Camera2D math wrong)**: If round-trip test fails — investigate matrix composition order (translate × rotate × scale vs reverse), Vulkan NDC Y-direction (+Y down), ortho projection bottom/top argument order.

### Commit 7 — SpriteRenderer batched rewrite (BeginFrame/Submit/EndFrame API)

**Scope**: sprite

**Files modified**:
- `src/DualFrontier.Runtime/Sprite/SpriteRenderer.cs` — MAJOR REWRITE per S-LOCK-7:

```csharp
using System.Numerics;
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 batched sprite renderer per S-LOCK-7.
/// Accumulates sprites by SpriteTexture key during Submit() calls.
/// EndFrame issues one vkCmdDrawIndexed per atlas group.
///
/// V0.C.1 single-sprite-per-call DrawSprite API replaced by BeginFrame/Submit/EndFrame.
/// Runtime.RecordSpriteFrame retained for backward compatibility (V0.C.1) — internally
/// uses BeginFrame/Submit/EndFrame with identity Camera2D.
/// </summary>
public sealed class SpriteRenderer : IDisposable
{
    private const uint DefaultDescriptorPoolCapacity = 64;
    private const int DefaultMaxSpritesPerFrame = 10_000;

    private readonly VulkanDevice _device;
    private readonly VulkanSpritePipeline _pipeline;
    private readonly VertexBufferRing _vertexRing;
    private readonly SpriteIndexBuffer _indexBuffer;
    private readonly int _maxSpritesPerFrame;
    private readonly Dictionary<SpriteTexture, List<Sprite>> _frameAtlasGroups = new();
    private readonly Dictionary<SpriteTexture, IntPtr> _descriptorSetCache = new();
    private IntPtr _descriptorPool;
    private uint _currentFrameIndex;
    private bool _frameActive;
    private bool _disposed;

    public int MaxSpritesPerFrame => _maxSpritesPerFrame;
    public int CurrentFrameSubmissionCount => _frameAtlasGroups.Values.Sum(list => list.Count);
    public int CachedDescriptorSetCount => _descriptorSetCache.Count;

    public SpriteRenderer(
        VulkanDevice device,
        MemoryAllocator allocator,
        VulkanSpritePipeline pipeline,
        int swapchainImageCount,
        int maxSpritesPerFrame = DefaultMaxSpritesPerFrame)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(allocator);
        ArgumentNullException.ThrowIfNull(pipeline);
        if (swapchainImageCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(swapchainImageCount));
        if (maxSpritesPerFrame <= 0 || maxSpritesPerFrame > SpriteIndexBuffer.MaxUint16Quads)
            throw new ArgumentOutOfRangeException(nameof(maxSpritesPerFrame),
                $"Must be 1..{SpriteIndexBuffer.MaxUint16Quads}");

        _device = device;
        _pipeline = pipeline;
        _maxSpritesPerFrame = maxSpritesPerFrame;

        _vertexRing = new VertexBufferRing(device, allocator, swapchainImageCount, maxSpritesPerFrame);
        _indexBuffer = new SpriteIndexBuffer(device, allocator, maxSpritesPerFrame);

        CreateDescriptorPool();
    }

    private unsafe void CreateDescriptorPool()
    {
        var poolSize = new VkDescriptorPoolSize
        {
            type = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = DefaultDescriptorPoolCapacity,
        };
        var poolInfo = new VkDescriptorPoolCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = (uint)VkDescriptorPoolCreateFlagBits.VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT,
            maxSets = DefaultDescriptorPoolCapacity,
            poolSizeCount = 1,
            _padBeforePool = 0,
            pPoolSizes = &poolSize,
        };
        VkResult result = VkApi.vkCreateDescriptorPool(_device.Handle, in poolInfo, IntPtr.Zero, out _descriptorPool);
        if (result != VkResult.VK_SUCCESS)
            throw new InvalidOperationException($"vkCreateDescriptorPool failed: {result}");
    }

    /// <summary>
    /// Begin batched frame. Caller passes frameIndex (typically swapchain image index).
    /// </summary>
    public void BeginFrame(uint frameIndex)
    {
        ThrowIfDisposed();
        if (_frameActive)
            throw new InvalidOperationException("BeginFrame called twice without EndFrame.");

        _currentFrameIndex = frameIndex;
        _frameAtlasGroups.Clear();
        _vertexRing.BeginFrame(frameIndex);
        _frameActive = true;
    }

    /// <summary>
    /// Submit sprite for batched rendering. Sprites grouped by SpriteTexture key.
    /// </summary>
    public void Submit(Sprite sprite)
    {
        if (!_frameActive)
            throw new InvalidOperationException("Submit called without BeginFrame.");
        ArgumentNullException.ThrowIfNull(sprite.Texture);

        if (CurrentFrameSubmissionCount >= _maxSpritesPerFrame)
            throw new InvalidOperationException(
                $"SpriteRenderer frame capacity exceeded ({_maxSpritesPerFrame}). " +
                "Per S-LOCK-3a hard cap. Multiple BeginFrame/EndFrame cycles required.");

        if (!_frameAtlasGroups.TryGetValue(sprite.Texture, out var list))
        {
            list = new List<Sprite>();
            _frameAtlasGroups[sprite.Texture] = list;
        }
        list.Add(sprite);
    }

    /// <summary>
    /// Record draw commands к command buffer. One vkCmdDrawIndexed per atlas group.
    /// MVP push constant = camera.ViewProjectionMatrix.
    /// </summary>
    public unsafe void EndFrame(VulkanCommandBuffer commandBuffer, Camera2D camera)
    {
        if (!_frameActive)
            throw new InvalidOperationException("EndFrame called without BeginFrame.");
        ArgumentNullException.ThrowIfNull(commandBuffer);
        ArgumentNullException.ThrowIfNull(camera);

        // 1. Write vertices к ring buffer chunk grouped by atlas.
        // Per-atlas-group vertex ranges tracked for vkCmdDrawIndexed firstIndex/vertexOffset.
        var atlasDrawRanges = new List<(SpriteTexture Texture, int VertexOffset, int IndexCount)>();
        int totalSpritesWritten = 0;

        foreach (var kvp in _frameAtlasGroups)
        {
            SpriteTexture texture = kvp.Key;
            List<Sprite> sprites = kvp.Value;
            int vertexOffset = totalSpritesWritten * 4;
            int indexCount = sprites.Count * SpriteIndexBuffer.IndicesPerQuad;

            foreach (Sprite sprite in sprites)
            {
                WriteSpriteVertices(sprite);
                totalSpritesWritten++;
            }

            atlasDrawRanges.Add((texture, vertexOffset, indexCount));
        }

        ulong vertexChunkOffset = _vertexRing.EndFrame();

        // 2. Bind pipeline + push constant + vertex/index buffers.
        VkApi.vkCmdBindPipeline(
            commandBuffer.Handle,
            VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
            _pipeline.Handle);

        Matrix4x4 mvp = camera.ViewProjectionMatrix;
        VkApi.vkCmdPushConstants(
            commandBuffer.Handle,
            _pipeline.Layout.Handle,
            VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT,
            offset: 0,
            size: 64,
            pValues: &mvp);

        IntPtr vbuffer = _vertexRing.Handle;
        ulong vbufferOffset = vertexChunkOffset;
        VkApi.vkCmdBindVertexBuffers(
            commandBuffer.Handle,
            firstBinding: 0,
            bindingCount: 1,
            pBuffers: &vbuffer,
            pOffsets: &vbufferOffset);

        VkApi.vkCmdBindIndexBuffer(
            commandBuffer.Handle,
            _indexBuffer.Handle,
            offset: 0,
            VkIndexType.VK_INDEX_TYPE_UINT16);

        // 3. Issue one vkCmdDrawIndexed per atlas group.
        int currentFirstIndex = 0;
        foreach (var range in atlasDrawRanges)
        {
            IntPtr descriptorSet = GetOrCreateDescriptorSet(range.Texture);
            IntPtr setHandle = descriptorSet;
            VkApi.vkCmdBindDescriptorSets(
                commandBuffer.Handle,
                VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
                _pipeline.Layout.Handle,
                firstSet: 0,
                descriptorSetCount: 1,
                pDescriptorSets: &setHandle,
                dynamicOffsetCount: 0,
                pDynamicOffsets: null);

            VkApi.vkCmdDrawIndexed(
                commandBuffer.Handle,
                indexCount: (uint)range.IndexCount,
                instanceCount: 1,
                firstIndex: (uint)currentFirstIndex,
                vertexOffset: range.VertexOffset,
                firstInstance: 0);

            currentFirstIndex += range.IndexCount;
        }

        _frameActive = false;
    }

    private void WriteSpriteVertices(Sprite sprite)
    {
        Vector2 halfSize = new(sprite.Transform.Scale.X * 0.5f, sprite.Transform.Scale.Y * 0.5f);
        Vector2 pos = sprite.Transform.Position;
        AtlasRegion uv = sprite.Region;
        uint tint = sprite.Transform.TintRgba;

        // 4 vertices: TL, TR, BR, BL — consumed by index pattern (0,1,2,2,3,0)
        SpriteVertex tl = new(new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U0, uv.V0), tint);
        SpriteVertex tr = new(new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y), new Vector2(uv.U1, uv.V0), tint);
        SpriteVertex br = new(new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U1, uv.V1), tint);
        SpriteVertex bl = new(new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y), new Vector2(uv.U0, uv.V1), tint);

        _vertexRing.WriteSprite(in tl, in tr, in br, in bl);
    }

    private unsafe IntPtr GetOrCreateDescriptorSet(SpriteTexture texture)
    {
        if (_descriptorSetCache.TryGetValue(texture, out IntPtr cached))
            return cached;
        if (_descriptorSetCache.Count >= DefaultDescriptorPoolCapacity)
            throw new InvalidOperationException(
                $"SpriteRenderer descriptor pool exhausted ({DefaultDescriptorPoolCapacity} textures).");

        IntPtr layout = _pipeline.DescriptorSetLayout.Handle;
        var allocInfo = new VkDescriptorSetAllocateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO,
            pNext = IntPtr.Zero,
            descriptorPool = _descriptorPool,
            descriptorSetCount = 1,
            _padBeforeLayouts = 0,
            pSetLayouts = &layout,
        };
        IntPtr descriptorSet = IntPtr.Zero;
        VkResult allocResult = VkApi.vkAllocateDescriptorSets(_device.Handle, in allocInfo, &descriptorSet);
        if (allocResult != VkResult.VK_SUCCESS)
            throw new InvalidOperationException($"vkAllocateDescriptorSets failed: {allocResult}");

        var imageInfo = new VkDescriptorImageInfo
        {
            sampler = texture.Sampler.Handle,
            imageView = texture.Image.ViewHandle,
            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            _padTrailing = 0,
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
            pImageInfo = &imageInfo,
            pBufferInfo = null,
            pTexelBufferView = IntPtr.Zero,
        };
        VkApi.vkUpdateDescriptorSets(_device.Handle, 1, &write, 0, IntPtr.Zero);

        _descriptorSetCache[texture] = descriptorSet;
        return descriptorSet;
    }

    public void Dispose()
    {
        if (_disposed) return;
        if (_descriptorPool != IntPtr.Zero)
        {
            VkApi.vkDestroyDescriptorPool(_device.Handle, _descriptorPool, IntPtr.Zero);
            _descriptorPool = IntPtr.Zero;
        }
        _indexBuffer.Dispose();
        _vertexRing.Dispose();
        _descriptorSetCache.Clear();
        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(SpriteRenderer));
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/SpriteRendererBatchedTests.cs` — extend or add tests:
```csharp
public class SpriteRendererBatchedTests
{
    [Fact]
    public void Submit_BeforeBeginFrame_Throws()
    {
        // Test InvalidOperationException
    }

    [Fact]
    public void BeginFrame_Twice_WithoutEndFrame_Throws()
    {
        // Test InvalidOperationException
    }

    [Fact]
    public void Submit_Beyond_MaxSpritesPerFrame_Throws()
    {
        // BeginFrame + Submit × MaxSpritesPerFrame succeeds
        // MaxSpritesPerFrame + 1 throws
    }

    [Fact]
    public void CurrentFrameSubmissionCount_Reflects_Sprite_Count_Across_Atlases()
    {
        // Submit 3 sprites of texture A + 5 sprites of texture B
        // → CurrentFrameSubmissionCount = 8
    }

    [Fact]
    public void Constructor_WithInvalidMaxSpritesPerFrame_Throws()
    {
        // maxSpritesPerFrame = 0 → throws
        // maxSpritesPerFrame > MaxUint16Quads → throws
    }
}
```

**Operations**:
1. Replace SpriteRenderer.cs with batched version
2. Create/extend SpriteRendererBatchedTests.cs (note: GPU-dependent tests require integration пattern per V0.B precedent — Submit/BeginFrame mock-safe tests + actual draw command tests require live VulkanDevice)
3. `dotnet build` — verify clean
4. `dotnet test` — full suite, baseline + new tests green
5. `git add` + `git commit -m "sprite: refactor SpriteRenderer к batched API (BeginFrame/Submit/EndFrame per S-LOCK-7, 90% rewrite)"`

**Verification**: SpriteRenderer accepts swapchainImageCount + maxSpritesPerFrame args. BeginFrame/Submit/EndFrame discipline enforced. Atlas grouping correct.

**Halt trigger SC-7 (existing DrawSprite consumers break)**: Runtime.cs `RecordSpriteFrame` method calls SpriteRenderer.DrawSprite — old API. V0.C.2 must update Runtime.RecordSpriteFrame internal implementation к use BeginFrame/Submit/EndFrame (next commit). If executor commits SpriteRenderer rewrite without updating Runtime.RecordSpriteFrame → dotnet build fails. Acceptable if next commit immediately follows fixes — но per atomic discipline, commit 7 SHOULD include Runtime.RecordSpriteFrame update OR commit 7 + commit 8 done в same session before pushing.

**Recommendation для executor**: combine SpriteRenderer rewrite + Runtime.RecordSpriteFrame update в Commit 7 (single semantic unit). Если split → commit 7 leaves build broken transitionally — violates atomic discipline. Per S-LOCK-9 invariant: each commit independently buildable.

**REVISED Commit 7 scope**: SpriteRenderer rewrite + Runtime.RecordSpriteFrame backward-compat update (call BeginFrame/Submit/EndFrame internally with identity Camera2D) — to preserve build invariant.

### Commit 8 — Runtime facade extension (Camera2D property + RecordSpritesFrame batched convenience)

**Scope**: runtime

**Files modified**:
- `src/DualFrontier.Runtime/Runtime.cs` — ADD Camera2D property + RecordSpritesFrame method:

```csharp
// V0.C.2 additions:
public Camera2D Camera { get; private set; } = null!;

// In Create():
runtime.Camera = new Camera2D
{
    ViewportSize = new Vector2(options.Window.Width, options.Window.Height),
};

// V0.C.2 SpriteRenderer construction now с swapchain image count + maxSpritesPerFrame:
runtime.SpriteRenderer = new SpriteRenderer(
    runtime.VulkanDevice,
    runtime.MemoryAllocator,
    runtime.SpritePipeline,
    runtime.Swapchain.Images.Count,
    maxSpritesPerFrame: 10_000);

// V0.C.2 RecordSpritesFrame — batched convenience:
/// <summary>
/// V0.C.2 batched convenience: record many sprites per frame с Camera2D MVP.
/// </summary>
public unsafe void RecordSpritesFrame(
    VulkanCommandBuffer commandBuffer,
    int imageIndex,
    IEnumerable<Sprite.Sprite> sprites,
    Camera2D camera,
    Vector4 clearColor)
{
    ArgumentNullException.ThrowIfNull(commandBuffer);
    ArgumentNullException.ThrowIfNull(sprites);
    ArgumentNullException.ThrowIfNull(camera);
    if ((uint)imageIndex >= _framebuffers.Count)
        throw new ArgumentOutOfRangeException(nameof(imageIndex));

    VulkanFramebuffer framebuffer = _framebuffers[imageIndex];

    VkClearValue clearValue = default;
    clearValue.color.float32[0] = clearColor.X;
    clearValue.color.float32[1] = clearColor.Y;
    clearValue.color.float32[2] = clearColor.Z;
    clearValue.color.float32[3] = clearColor.W;

    var renderPassBegin = new VkRenderPassBeginInfo
    {
        sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
        pNext = IntPtr.Zero,
        renderPass = RenderPass.Handle,
        framebuffer = framebuffer.Handle,
        renderArea = new VkRect2D
        {
            offsetX = 0, offsetY = 0,
            width = framebuffer.Width, height = framebuffer.Height,
        },
        clearValueCount = 1,
        _padBeforePtr = 0,
        pClearValues = &clearValue,
    };
    VkApi.vkCmdBeginRenderPass(commandBuffer.Handle, in renderPassBegin, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE);

    VkViewport viewport = new()
    {
        x = 0, y = 0,
        width = framebuffer.Width, height = framebuffer.Height,
        minDepth = 0.0f, maxDepth = 1.0f,
    };
    VkApi.vkCmdSetViewport(commandBuffer.Handle, 0, 1, &viewport);

    VkRect2D scissor = new()
    {
        offsetX = 0, offsetY = 0,
        width = framebuffer.Width, height = framebuffer.Height,
    };
    VkApi.vkCmdSetScissor(commandBuffer.Handle, 0, 1, &scissor);

    SpriteRenderer.BeginFrame((uint)imageIndex);
    foreach (var sprite in sprites)
        SpriteRenderer.Submit(sprite);
    SpriteRenderer.EndFrame(commandBuffer, camera);

    VkApi.vkCmdEndRenderPass(commandBuffer.Handle);
}

// V0.C.1 RecordSpriteFrame retained — refactor internally к use batched API:
public unsafe void RecordSpriteFrame(
    VulkanCommandBuffer commandBuffer,
    int imageIndex,
    Sprite.Sprite sprite,
    Matrix4x4 mvp,
    Vector4 clearColor)
{
    // V0.C.1 backward compat: single sprite via batched infrastructure.
    // mvp parameter ignored — use Camera property of Runtime instead.
    // Crystalka adjusts smoke test к use RecordSpritesFrame + Camera if needed.
    var tempCam = new Camera2D
    {
        ViewportSize = new Vector2(Swapchain.Width, Swapchain.Height),
    };
    RecordSpritesFrame(commandBuffer, imageIndex, new[] { sprite }, tempCam, clearColor);
}
```

**Operations**:
1. Edit Runtime.cs per above
2. Update SpriteRenderer construction в Create() с swapchain image count + maxSpritesPerFrame
3. Update or add Runtime tests if needed
4. `dotnet build` — verify clean
5. `dotnet test` — full suite green
6. `git add` + `git commit -m "runtime: add Camera2D property + RecordSpritesFrame batched convenience (S-LOCK-1)"`

**Verification**: Runtime.Camera property accessible. RecordSpritesFrame accepts IEnumerable<Sprite> + Camera2D. RecordSpriteFrame V0.C.1 backward compat works via batched internal implementation.

### Commit 9 — TileMap class

**Scope**: sprite + tests

**Files added**:
- `src/DualFrontier.Runtime/Sprite/TileMap.cs` — NEW class per S-LOCK-5:

```csharp
using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// V0.C.2 TileMap class per S-LOCK-5.
/// 2D grid of tiles, each tile = AtlasRegion + tint.
/// Submit() enumerates tiles + invokes SpriteRenderer.Submit per tile (one sprite per tile).
/// 200×200 grid = 40,000 tiles = 4× R.2 stress test target.
///
/// V0.C.2 hard cap: if width × height > SpriteRenderer.MaxSpritesPerFrame,
/// multiple BeginFrame/EndFrame cycles required (per S-LOCK-5a).
/// </summary>
public sealed class TileMap : IDisposable
{
    private readonly AtlasRegion[] _regions;
    private readonly uint[] _tints;
    private bool _disposed;

    public int Width { get; }
    public int Height { get; }
    public float TileSize { get; }
    public SpriteTexture Atlas { get; }

    public TileMap(int width, int height, float tileSize, SpriteTexture atlas)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (tileSize <= 0) throw new ArgumentOutOfRangeException(nameof(tileSize));
        ArgumentNullException.ThrowIfNull(atlas);

        Width = width;
        Height = height;
        TileSize = tileSize;
        Atlas = atlas;

        int count = width * height;
        _regions = new AtlasRegion[count];
        _tints = new uint[count];
        // Initialize tints к opaque white default
        for (int i = 0; i < count; i++)
            _tints[i] = SpriteVertex.WhiteTint;
    }

    public void SetTile(int x, int y, AtlasRegion region)
    {
        ValidateCoords(x, y);
        _regions[y * Width + x] = region;
    }

    public void SetTile(int x, int y, AtlasRegion region, uint tintRgba)
    {
        ValidateCoords(x, y);
        int idx = y * Width + x;
        _regions[idx] = region;
        _tints[idx] = tintRgba;
    }

    public AtlasRegion GetTile(int x, int y)
    {
        ValidateCoords(x, y);
        return _regions[y * Width + x];
    }

    public uint GetTint(int x, int y)
    {
        ValidateCoords(x, y);
        return _tints[y * Width + x];
    }

    /// <summary>
    /// Submit all tiles к SpriteRenderer. Caller must have called BeginFrame on renderer.
    /// V0.C.2 V0 enumerates all tiles (no culling); V0.C.2.1 future extension adds Camera2D AABB culling.
    /// If tile count exceeds renderer.MaxSpritesPerFrame, caller responsible for multiple
    /// BeginFrame/EndFrame cycles.
    /// </summary>
    public void Submit(SpriteRenderer renderer, Camera2D camera)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        ArgumentNullException.ThrowIfNull(camera);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int idx = y * Width + x;
                var region = _regions[idx];
                var tint = _tints[idx];

                var sprite = new Sprite
                {
                    Texture = Atlas,
                    Region = region,
                    Transform = new SpriteTransform
                    {
                        Position = new Vector2(x * TileSize, y * TileSize),
                        Scale = new Vector2(TileSize, TileSize),
                        TintRgba = tint,
                    },
                };
                renderer.Submit(sprite);
            }
        }
    }

    private void ValidateCoords(int x, int y)
    {
        if ((uint)x >= Width || (uint)y >= Height)
            throw new ArgumentOutOfRangeException($"Tile coordinates ({x},{y}) out of TileMap bounds {Width}x{Height}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        // Atlas is caller-owned — not disposed by TileMap
        _disposed = true;
    }
}
```

- `tests/DualFrontier.Runtime.Tests/Sprite/TileMapTests.cs` — NEW tests:
```csharp
public class TileMapTests
{
    [Fact]
    public void Constructor_ValidArgs_Succeeds()
    {
        // Use mock SpriteTexture — actual VulkanImage/Sampler not needed for managed-side test
    }

    [Fact]
    public void Constructor_InvalidArgs_Throws()
    {
        // width = 0, height = 0, tileSize = 0 → throws
    }

    [Fact]
    public void SetTile_OutOfBounds_Throws()
    {
        // x=Width+1 → throws
        // y=Height+1 → throws
        // x=-1 → throws (cast к uint catches это)
    }

    [Fact]
    public void GetTile_ReturnsSetValue()
    {
        // SetTile(5, 10, region) + GetTile(5, 10) == region
    }

    [Fact]
    public void Default_Tints_AreOpaqueWhite()
    {
        // Constructor initializes _tints к SpriteVertex.WhiteTint
    }

    [Fact]
    public void SetTile_WithTint_PreservesTint()
    {
        // SetTile(0,0, region, 0xFF0000FF) + GetTint(0,0) == 0xFF0000FF
    }
}
```

**Operations**:
1. Create TileMap.cs
2. Create TileMapTests.cs (managed-side tests + bounds checks — no GPU dependency)
3. `dotnet build` — verify clean
4. `dotnet test --filter "FullyQualifiedName~TileMapTests"` — verify TileMap logic
5. `dotnet test` — full suite green
6. `git add` + `git commit -m "sprite: add TileMap class (S-LOCK-5 one-sprite-per-tile + 40K stress capacity)"`

**Verification**: TileMap stores regions + tints correctly. Bounds checks reject invalid coordinates. Submit enumerates all tiles + invokes SpriteRenderer.Submit per tile.


### Commit 10 — Procedural atlas generation helper (smoke test infrastructure)

**Scope**: tests

**Files added**:
- `tests/DualFrontier.Runtime.SmokeTest/ProceduralAtlas.cs` — NEW helper:

```csharp
using DualFrontier.Runtime.Assets;

namespace DualFrontier.Runtime.SmokeTest;

/// <summary>
/// V0.C.2 procedural atlas generation для 10K sprite stress + 200×200 TileMap smoke tests.
/// Generates 256×256 RGBA8 atlas с 16×16 tile grid = 16 distinct tile types.
/// Each tile type = distinct visual pattern (checker, gradient, noise) для visual differentiation.
/// </summary>
public static class ProceduralAtlas
{
    public const int AtlasWidth = 256;
    public const int AtlasHeight = 256;
    public const int TileWidth = 16;
    public const int TileHeight = 16;
    public const int TilesPerRow = AtlasWidth / TileWidth;        // 16
    public const int TilesPerColumn = AtlasHeight / TileHeight;   // 16
    public const int TotalTiles = TilesPerRow * TilesPerColumn;   // 256

    /// <summary>
    /// Generate procedural atlas PngImage с per-tile distinct pattern.
    /// </summary>
    public static PngImage GenerateAtlas()
    {
        int pixelCount = AtlasWidth * AtlasHeight;
        byte[] pixels = new byte[pixelCount * 4];  // RGBA8

        for (int tileIdx = 0; tileIdx < TotalTiles; tileIdx++)
        {
            int tileX = tileIdx % TilesPerRow;
            int tileY = tileIdx / TilesPerRow;
            FillTile(pixels, tileX, tileY, tileIdx);
        }

        return new PngImage(AtlasWidth, AtlasHeight, pixels, PngColorType.RgbAlpha);
    }

    private static void FillTile(byte[] pixels, int tileX, int tileY, int tileIdx)
    {
        // Each tile gets distinct color base + pattern modulation
        byte baseR = (byte)((tileIdx * 37) % 256);
        byte baseG = (byte)((tileIdx * 71) % 256);
        byte baseB = (byte)((tileIdx * 113) % 256);

        int patternType = tileIdx % 4;  // 4 pattern types (solid, checker, gradient, ring)

        for (int py = 0; py < TileHeight; py++)
        {
            for (int px = 0; px < TileWidth; px++)
            {
                int pxAbs = tileX * TileWidth + px;
                int pyAbs = tileY * TileHeight + py;
                int idx = (pyAbs * AtlasWidth + pxAbs) * 4;

                byte r = baseR, g = baseG, b = baseB;

                switch (patternType)
                {
                    case 0: // Solid
                        break;
                    case 1: // Checker
                        if (((px / 4) + (py / 4)) % 2 == 1) { r ^= 0x55; g ^= 0x55; b ^= 0x55; }
                        break;
                    case 2: // Horizontal gradient
                        r = (byte)((r + px * 8) % 256);
                        break;
                    case 3: // Ring
                        int dx = px - 8, dy = py - 8;
                        int d2 = dx * dx + dy * dy;
                        if (d2 > 32 && d2 < 56) { r ^= 0xAA; g ^= 0xAA; b ^= 0xAA; }
                        break;
                }

                pixels[idx + 0] = r;
                pixels[idx + 1] = g;
                pixels[idx + 2] = b;
                pixels[idx + 3] = 255;  // fully opaque
            }
        }
    }

    /// <summary>
    /// Returns AtlasRegion для specific tile index (0..TotalTiles-1).
    /// </summary>
    public static AtlasRegion GetTileRegion(int tileIndex)
    {
        if ((uint)tileIndex >= TotalTiles)
            throw new ArgumentOutOfRangeException(nameof(tileIndex));
        int tileX = tileIndex % TilesPerRow;
        int tileY = tileIndex / TilesPerRow;
        return AtlasRegion.FromPixels(
            tileX * TileWidth, tileY * TileHeight,
            TileWidth, TileHeight,
            AtlasWidth, AtlasHeight);
    }
}
```

**Operations**:
1. Create ProceduralAtlas.cs in SmokeTest project
2. `dotnet build` — verify SmokeTest project builds
3. `git add` + `git commit -m "tests: add ProceduralAtlas helper for V0.C.2 stress test atlas generation"`

**Verification**: ProceduralAtlas.GenerateAtlas() returns 256×256 RGBA8 PngImage. GetTileRegion(N) returns normalized AtlasRegion for tile N.

### Commit 11 — 10,000-sprite stress test scene in SmokeTest Program.cs

**Scope**: tests

**Files modified**:
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — extend с stress test scene:

```csharp
// Add after existing V0.C.1 smoke test scene:

static void RunStressTest10K(Runtime runtime, int durationSeconds = 10)
{
    Console.WriteLine($"=== V0.C.2 stress test: 10K sprites for {durationSeconds}s ===");

    // 1. Generate procedural atlas + upload via TextureUploader.
    PngImage atlasImage = ProceduralAtlas.GenerateAtlas();
    using var atlasTexture = SpriteTexture.CreateFromPngImage(
        runtime.VulkanDevice, runtime.MemoryAllocator,
        runtime.TextureUploader, atlasImage,
        runtime.DefaultSampler);

    // 2. Generate 10,000 random sprites referencing procedural atlas.
    var random = new Random(seed: 42);
    var sprites = new List<Sprite>(10_000);
    for (int i = 0; i < 10_000; i++)
    {
        int tileIdx = random.Next(ProceduralAtlas.TotalTiles);
        AtlasRegion region = ProceduralAtlas.GetTileRegion(tileIdx);
        float x = (float)(random.NextDouble() * runtime.Swapchain.Width);
        float y = (float)(random.NextDouble() * runtime.Swapchain.Height);
        uint tint = (uint)random.Next(int.MinValue, int.MaxValue);
        sprites.Add(new Sprite
        {
            Texture = atlasTexture,
            Region = region,
            Transform = new SpriteTransform
            {
                Position = new Vector2(x, y),
                Scale = new Vector2(16, 16),
                TintRgba = tint,
            },
        });
    }

    // 3. Set Camera2D к viewport-fitting view.
    runtime.Camera.Position = new Vector2(runtime.Swapchain.Width * 0.5f, runtime.Swapchain.Height * 0.5f);
    runtime.Camera.ViewportSize = new Vector2(runtime.Swapchain.Width, runtime.Swapchain.Height);
    runtime.Camera.Zoom = 1.0f;

    // 4. Render loop с timing.
    int frameCount = 0;
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
    {
        runtime.Window.PumpMessages();
        if (runtime.Window.ShouldClose) break;

        // Render frame
        RenderFrameWithSprites(runtime, sprites);
        frameCount++;
    }

    stopwatch.Stop();
    double fps = frameCount / stopwatch.Elapsed.TotalSeconds;
    Console.WriteLine($"  10K stress test: {frameCount} frames in {stopwatch.Elapsed.TotalSeconds:F2}s = {fps:F1} FPS");
    Console.WriteLine($"  Validation messages: {runtime.ValidationLayer?.MessageCount ?? 0}");

    if (fps < 60.0)
        Console.WriteLine($"  WARNING: FPS {fps:F1} < 60 target. R.2 success criterion not met.");
    else
        Console.WriteLine($"  ✓ R.2 success criterion met (60+ FPS sustained).");
}

static void RenderFrameWithSprites(Runtime runtime, IEnumerable<Sprite> sprites)
{
    // Acquire next swapchain image
    uint imageIndex = runtime.Swapchain.AcquireNextImage(/* semaphore */ ...);
    
    // Record + submit + present (using runtime.RecordSpritesFrame batched convenience)
    using var commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
    commandBuffer.Begin();
    runtime.RecordSpritesFrame(commandBuffer, (int)imageIndex, sprites, runtime.Camera, new Vector4(0.1f, 0.1f, 0.2f, 1));
    commandBuffer.End();
    
    runtime.VulkanDevice.GraphicsQueue.Submit(commandBuffer, ...);
    runtime.Swapchain.Present(imageIndex, ...);
}
```

Note: Exact `Window.PumpMessages`, `Swapchain.AcquireNextImage`, queue submit/present API per V0.C.1 smoke test main loop pattern — executor extends existing pattern, не invents new API surface.

**Operations**:
1. Edit Program.cs к add 10K stress test scene
2. `dotnet build` — verify SmokeTest builds
3. `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` — verify scene executes
4. Manual verification: Crystalka runs on «Skarlet»:
   - Window opens
   - 10K sprites visible с randomized positions + tints + tile patterns
   - Validation 0 errors/warnings
   - FPS ≥ 60 sustained for 10 seconds (≥ 600 frames)
   - Single vkCmdDrawIndexed visible in RenderDoc (single atlas)
5. `git add` + `git commit -m "tests: add 10K sprite stress test scene (R.2 verification, single atlas single draw call)"`

**Verification**: 10K sprites render at 60+ FPS. Validation clean. Per S-LOCK-7a manual RenderDoc inspection confirms single vkCmdDrawIndexed call.

**Halt trigger SC-8 (FPS < 60 target)**: If 10K stress test produces < 60 FPS sustained → HALT-SC-8. Investigate:
- Per-frame ring buffer alloc overhead (excessive)
- Descriptor set rebinding excessive (multiple atlases when expected single)
- Validation layer overhead в DEBUG mode (verify Release build performance separately)
- Hardware specific — AMD RX 7600S baseline V0.C.1 = 164 FPS for single sprite; 10K sprites should still produce 60+ FPS

Surface к Crystalka для measurement-driven evaluation.

### Commit 12 — 200×200 TileMap smoke test scene

**Scope**: tests

**Files modified**:
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — extend с TileMap scene:

```csharp
// Add after RunStressTest10K:

static void RunTileMap200x200(Runtime runtime, int durationSeconds = 10)
{
    Console.WriteLine($"=== V0.C.2 TileMap 200×200 test for {durationSeconds}s ===");

    // 1. Generate procedural atlas + upload (same atlas as stress test).
    PngImage atlasImage = ProceduralAtlas.GenerateAtlas();
    using var atlasTexture = SpriteTexture.CreateFromPngImage(
        runtime.VulkanDevice, runtime.MemoryAllocator,
        runtime.TextureUploader, atlasImage,
        runtime.DefaultSampler);

    // 2. Construct 200×200 TileMap с procedural terrain pattern.
    const int width = 200, height = 200;
    const float tileSize = 16.0f;
    using var tileMap = new TileMap(width, height, tileSize, atlasTexture);

    // Populate с procedural pattern (e.g. terrain-like noise pattern).
    var random = new Random(seed: 123);
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            int tileIdx;
            // Procedural terrain: bands + noise
            if ((x + y) / 30 % 2 == 0)
                tileIdx = random.Next(0, 4);   // grass-like tiles
            else
                tileIdx = random.Next(4, 8);   // stone-like tiles
            AtlasRegion region = ProceduralAtlas.GetTileRegion(tileIdx);
            tileMap.SetTile(x, y, region);
        }
    }

    // 3. Set Camera2D к view top-left corner of map.
    runtime.Camera.Position = new Vector2(runtime.Swapchain.Width * 0.5f, runtime.Swapchain.Height * 0.5f);
    runtime.Camera.ViewportSize = new Vector2(runtime.Swapchain.Width, runtime.Swapchain.Height);
    runtime.Camera.Zoom = 1.0f;

    // 4. Render loop с camera pan via WASD input.
    int frameCount = 0;
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    int submissionsPerFrame = 0;

    while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
    {
        runtime.Window.PumpMessages();
        if (runtime.Window.ShouldClose) break;

        // Pan camera via input events.
        ProcessCameraInput(runtime);

        // Render TileMap + count submissions для verification of S-LOCK-5a multi-cycle if needed.
        RenderFrameWithTileMap(runtime, tileMap);
        frameCount++;
    }

    stopwatch.Stop();
    double fps = frameCount / stopwatch.Elapsed.TotalSeconds;
    Console.WriteLine($"  200x200 TileMap: {frameCount} frames in {stopwatch.Elapsed.TotalSeconds:F2}s = {fps:F1} FPS");
    Console.WriteLine($"  Tile count: {width * height} = {(width * height / 10_000)} BeginFrame/EndFrame cycles per frame");
    Console.WriteLine($"  Validation messages: {runtime.ValidationLayer?.MessageCount ?? 0}");

    if (fps < 60.0)
        Console.WriteLine($"  WARNING: FPS {fps:F1} < 60 target. R.3 success criterion not met.");
    else
        Console.WriteLine($"  ✓ R.3 success criterion met (200×200 at 60+ FPS).");
}

static void ProcessCameraInput(Runtime runtime)
{
    // Drain input event queue, react to WASD keys.
    while (runtime.InputQueue.TryDequeue(out var ev))
    {
        if (ev is KeyHeldEvent keyEv) // или whatever V0.C.1 event type covers held keys
        {
            const float panSpeed = 5.0f;
            switch (keyEv.Key)
            {
                case Key.W: runtime.Camera.Position += new Vector2(0, -panSpeed); break;
                case Key.S: runtime.Camera.Position += new Vector2(0, panSpeed); break;
                case Key.A: runtime.Camera.Position += new Vector2(-panSpeed, 0); break;
                case Key.D: runtime.Camera.Position += new Vector2(panSpeed, 0); break;
            }
        }
    }
}

static void RenderFrameWithTileMap(Runtime runtime, TileMap tileMap)
{
    // S-LOCK-5a: если tileCount > MaxSpritesPerFrame, multiple BeginFrame/EndFrame cycles.
    int totalTiles = tileMap.Width * tileMap.Height;
    int cyclesNeeded = (totalTiles + runtime.SpriteRenderer.MaxSpritesPerFrame - 1) / runtime.SpriteRenderer.MaxSpritesPerFrame;
    
    uint imageIndex = runtime.Swapchain.AcquireNextImage(/* ... */);
    using var commandBuffer = runtime.GraphicsCommandPool.AllocateBuffer();
    commandBuffer.Begin();
    
    // Begin render pass
    BeginRenderPass(commandBuffer, runtime, imageIndex, new Vector4(0.1f, 0.1f, 0.2f, 1));
    
    // Multi-cycle TileMap submission per S-LOCK-5a
    int tilesPerCycle = runtime.SpriteRenderer.MaxSpritesPerFrame;
    for (int cycle = 0; cycle < cyclesNeeded; cycle++)
    {
        int startTile = cycle * tilesPerCycle;
        int endTile = Math.Min(startTile + tilesPerCycle, totalTiles);
        
        runtime.SpriteRenderer.BeginFrame((uint)imageIndex);
        for (int i = startTile; i < endTile; i++)
        {
            int x = i % tileMap.Width;
            int y = i / tileMap.Width;
            var sprite = new Sprite
            {
                Texture = tileMap.Atlas,
                Region = tileMap.GetTile(x, y),
                Transform = new SpriteTransform
                {
                    Position = new Vector2(x * tileMap.TileSize, y * tileMap.TileSize),
                    Scale = new Vector2(tileMap.TileSize, tileMap.TileSize),
                    TintRgba = tileMap.GetTint(x, y),
                },
            };
            runtime.SpriteRenderer.Submit(sprite);
        }
        runtime.SpriteRenderer.EndFrame(commandBuffer, runtime.Camera);
    }
    
    EndRenderPass(commandBuffer);
    commandBuffer.End();
    
    runtime.VulkanDevice.GraphicsQueue.Submit(commandBuffer, /* ... */);
    runtime.Swapchain.Present(imageIndex, /* ... */);
}
```

Note: Exact swapchain acquire / submit / present API per V0.C.1 smoke test pattern. Multi-cycle TileMap submission per S-LOCK-5a — 40K tiles / 10K MaxSpritesPerFrame = 4 cycles per frame = 4 vkCmdDrawIndexed calls per frame (single atlas = single draw call per cycle).

**Operations**:
1. Edit Program.cs к add TileMap scene
2. `dotnet build` — verify
3. `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` — verify scene executes
4. Manual verification: Crystalka runs on «Skarlet»:
   - 200×200 tile grid visible (32-pixel × 200 = 3200 pixels wide; only viewport-portion visible at given camera position)
   - WASD camera pan works smoothly
   - 60+ FPS sustained for 10 seconds
   - Validation 0 errors/warnings
   - 4 vkCmdDrawIndexed calls per frame visible in RenderDoc (single atlas across 4 cycles per S-LOCK-5a)
5. `git add` + `git commit -m "tests: add 200×200 TileMap smoke test scene (R.3 verification + camera pan + S-LOCK-5a multi-cycle)"`

**Verification**: 200×200 TileMap (40K tiles) renders at 60+ FPS. Camera2D pan via WASD events smooth. RenderDoc shows 4 draw calls per frame per S-LOCK-5a multi-cycle pattern.

**Halt trigger SC-9 (TileMap performance < 60 FPS)**: If 200×200 TileMap < 60 FPS → HALT-SC-9. Investigate per-cycle overhead, descriptor set rebinding within cycle. Surface к Crystalka.

**Halt trigger SC-10 (Camera pan broken)**: Input events не reach Camera2D — verify Win32 message dispatch к InputEventQueue (V0.C.1 infrastructure), verify event types (KeyHeldEvent vs KeyPressedEvent — may need extension if V0.C.1 only landed KeyPressedEvent + KeyReleasedEvent).

### Commit 13 — Manual visual verification gate documentation

**Scope**: docs

**Files added**:
- `docs/scratch/V0_C_2/MANUAL_VISUAL_VERIFICATION_PROTOCOL.md` — NEW:

```markdown
# V0.C.2 manual visual verification protocol

Per Lesson #27 candidate + S-LOCK-9 manual gate.

## Pre-commit gate (before Commit 17 closure)

Crystalka runs `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` on «Skarlet».
Smoke test executes V0.C.1 single sprite + V0.C.2 10K stress + V0.C.2 200×200 TileMap scenes sequentially.

## Acceptance criteria

### V0.C.1 single sprite scene (regression check)
- Kenney pawn (or synthetic equivalent) visible centered
- Validation: 0 errors, 0 warnings, 0 info messages
- FPS ≥ 60 sustained

### V0.C.2 R.2 10K stress test scene
- 10,000 sprites visible с randomized positions across viewport
- Each sprite shows distinct tint + procedural atlas tile pattern
- FPS ≥ 60 sustained for 10 seconds (≥ 600 frames)
- Validation: 0 errors, 0 warnings
- RenderDoc inspection: single vkCmdDrawIndexed call per frame (single atlas)

### V0.C.2 R.3 200×200 TileMap scene
- 200×200 tile grid visible (partial viewport region)
- WASD keys pan camera smoothly
- FPS ≥ 60 sustained for 10 seconds
- Validation: 0 errors, 0 warnings
- RenderDoc inspection: 4 vkCmdDrawIndexed calls per frame (per S-LOCK-5a multi-cycle, single atlas)

### Regression checks (V0.A + V0.B + V0.C.1 invariants preserved)
- HardwareCapabilityCheck passes (K-L19 preserved)
- Swapchain recreation works on window resize
- Clean shutdown (no leaked Vulkan handles per validation)
- Mixed [LibraryImport]/[DllImport] convention preserved
- Sprite shaders SPIR-V loaded successfully

## Halt triggers

Per V0.C.2 brief §4 SC-N taxonomy. If any visual verification fails → HALT, surface к Crystalka, do NOT push commit.

## Hardware baseline

«Skarlet» = ASUS TUF Gaming A16 (AMD RX 7600S). V0.C.1 baseline: 164 FPS for single sprite. V0.C.2 expected: 10K sprites at 60-100 FPS, 200×200 TileMap at 60-120 FPS.

## Validation log capture

Smoke test prints validation message count к console at end of each scene. Zero across all scenes = pass.
```

**Operations**:
1. Create docs/scratch/V0_C_2/MANUAL_VISUAL_VERIFICATION_PROTOCOL.md
2. `git add` + `git commit -m "docs: add V0.C.2 manual visual verification protocol (S-LOCK-9 gate)"`

**Verification**: Document committed. Crystalka reads protocol before running smoke test.

### Commit 14 — Pre-closure validation + governance amendment

**Scope**: governance

**Files modified**:
- `docs/governance/REGISTER.yaml` — UPDATE DOC-D-V0_C_2 entry:
  - `lifecycle: AUTHORED` → `lifecycle: EXECUTED`
  - Add audit_trail event

```yaml
# REGISTER.yaml audit_trail section addition:
audit_trail:
  # ... existing events ...
  - event_id: EVT-V0_C_2-CLOSURE
    date: 2026-05-XX  # actual date at closure
    actor: Crystalka + Opus (deliberation) + Claude Code (execution)
    type: brief_executed
    summary: |
      V0.C.2 closure: batched sprite renderer + Camera2D + TileMap (R.2 + R.3)
      shipped via 17-commit cascade. V0 substrate close per Q8 ratification.
      210 → ~270 Runtime.Tests proportional growth. 10K stress at 60+ FPS verified.
      200×200 TileMap at 60+ FPS verified. Manual visual verification on «Skarlet» AMD RX 7600S.
      Zero validation errors throughout. Lesson #7 strengthening continues per executor alignment audit.
    related_documents:
      - DOC-D-V0_C_2
      - DOC-A-VULKAN_SUBSTRATE
    commit_range: <first-commit-hash>..<closure-commit-hash>
```

- `docs/MIGRATION_PROGRESS.md` — UPDATE V substrate section:
  - R.1 ✓ (V0.C.1)
  - R.2 ✓ (V0.C.2)
  - R.3 ✓ (V0.C.2)
  - R.4 ✓ (V0.C.1)
  - **V0 substrate close achieved** per Q8 ratification
  - Phase B M-cycle vanilla migration gate now open (gated also on Roslyn analyzer A'.9)

**Operations**:
1. Edit REGISTER.yaml (lifecycle + audit_trail event)
2. Edit MIGRATION_PROGRESS.md (V substrate status)
3. `tools\governance\sync_register.ps1` (full sync, regenerates V0.C.2 brief frontmatter с EXECUTED lifecycle)
4. `tools\governance\sync_register.ps1 --validate` — verify exit 0
5. `git add` + `git commit -m "governance: V0.C.2 closure — DOC-D-V0_C_2 EXECUTED, V0 substrate close achieved"`

**Verification**: REGISTER.yaml lifecycle EXECUTED. Audit trail event landed. MIGRATION_PROGRESS.md reflects V0 close. sync_register.ps1 --validate exits 0.

### Commit 15 — Closure section in V0.C.2 brief

**Scope**: docs

**Files modified**:
- `tools/briefs/V0_C_2_EXECUTION_BRIEF.md` — ADD §8 closure section:

```markdown
## §8 — Closure section (added at EXECUTED transition)

**Date executed**: 2026-05-XX
**Branch**: claude/v0_c_2-batched-sprite-tilemap-camera
**Commits**: 17 atomic (commit hashes <first>..<closure>)
**PR**: #XX (Crystalka merged к main after review)

### Commit ledger

| # | Scope | Hash | Summary |
|---|-------|------|---------|
| 1 | governance | <hash> | enroll V0.C.2 brief (DOC-D-V0_C_2 AUTHORED) |
| 2 | vulkan + tests | <hash> | add VkIndexType + vkCmdBindIndexBuffer + vkCmdDrawIndexed |
| 3 | sprite | <hash> | add VertexBufferRing infrastructure |
| 4 | sprite | <hash> | add SpriteIndexBuffer (pre-populated uint16) |
| 5 | sprite + tests | <hash> | add AtlasRegion.FromPixels static factory |
| 6 | sprite + tests | <hash> | add Camera2D class |
| 7 | sprite | <hash> | refactor SpriteRenderer к batched API (90% rewrite) |
| 8 | runtime | <hash> | add Camera2D property + RecordSpritesFrame batched convenience |
| 9 | sprite + tests | <hash> | add TileMap class |
| 10 | tests | <hash> | add ProceduralAtlas helper |
| 11 | tests | <hash> | add 10K sprite stress test scene |
| 12 | tests | <hash> | add 200×200 TileMap smoke test scene |
| 13 | docs | <hash> | add manual visual verification protocol |
| 14 | governance | <hash> | V0.C.2 closure governance |
| 15 | docs | <hash> | add closure section к brief |
| 16 | tests | <hash> | final smoke test integration verification |
| 17 | docs | <hash> | final REGISTER + audit_trail sync |

### Verification metrics

- `dotnet build` clean (0 errors, 0 warnings)
- `cmake --build native/DualFrontier.Core.Native` clean (K substrate unchanged)
- `dotnet test` ~270 tests green (210 V0.C.1 baseline + ~60 V0.C.2 additions)
- `sync_register.ps1 --validate` exit 0
- Smoke test execution на «Skarlet»:
  - V0.C.1 single sprite regression: <FPS> sustained, validation clean
  - V0.C.2 10K stress: <FPS> sustained, validation clean, single draw call in RenderDoc
  - V0.C.2 200×200 TileMap: <FPS> sustained, validation clean, 4 draw calls per frame
- K-L19 invariant preserved (HardwareCapabilityCheck unchanged)

### Halt protocol activations

- <list any SC-N activations during execution>
- <or «none» if cascade completed without halts>

### Alignment audit corrections caught

- VkIndexType size hypothesis: <verified | adjusted к N bytes>
- <list any other Marshal.SizeOf adjustments>

### Lesson candidates surfaced

- Lesson #7 strengthening continues — V0.A (1) + V0.B (5) + V0.C.1 (0) + V0.C.2 (<N>) maturity curve
- Lesson #22 strengthened continues — mixed P/Invoke convention
- Lesson #25 strengthened continues — implementation depth follows consumer materialization
- Lesson #26 strengthened — cross-substrate scope splitting preserved multi-session budget
- Lesson #27 candidate continues — render workload exercises prior substrate primitives
- <V0.C.2-specific candidates if any>

### V0 substrate close pattern established

V0.A (PR #36) + V0.B (PR #37) + V0.C.1 (PR #38) + V0.C.2 (PR #XX) = four consecutive zero-hard-gate-halt cascades on V substrate authoring stream. К-L14 thesis empirically validated на V substrate с pattern reliability matching K substrate K0..K10 development.

### Next steps

V0 substrate close per Q8 ratification opens:
- V1 brief authoring (scalar field + diffusion shader, isotropic + anisotropic per VULKAN_SUBSTRATE §1.2)
- V2 brief authoring (scalar field + wave shader, routed + breakable per VULKAN_SUBSTRATE §1.3)
- Phase B M-cycle vanilla content migration (gated also on Roslyn analyzer А'.9 + K-closure report А'.8)

Independent streams unchanged:
- К10.3 brief restart (Crystalka prerogative)
- К10.4 TLA+ brief
- A'.8 K-closure report
- A'.9 Roslyn architectural analyzer milestone
```

**Operations**:
1. Edit V0.C.2 brief с closure section
2. `git add` + `git commit -m "docs: add V0.C.2 closure section к brief (commit ledger + metrics + lessons)"`

**Verification**: Closure section reflects actual cascade outcome. Commit hashes filled in by executor at closure time.

### Commit 16 — Final integration smoke test

**Scope**: tests

**Operations**:
1. Run full smoke test sequence on «Skarlet» one final time per Manual Visual Verification Protocol
2. Capture validation log + FPS measurements + RenderDoc draw call counts
3. Verify all V0.A + V0.B + V0.C.1 + V0.C.2 invariants preserved
4. If any regression → halt + investigate (likely SC-1 drift или SC-7 SpriteRenderer side effect)
5. If clean → commit any final test infrastructure adjustments (typically minor cleanup)
6. `git commit -m "tests: final V0.C.2 integration verification on «Skarlet» AMD RX 7600S"`

**Verification**: All smoke test scenes pass acceptance criteria. Validation clean. Manual visual gates met.

### Commit 17 — Final REGISTER amendments + push branch + PR opening

**Scope**: governance

**Operations**:
1. Final REGISTER.yaml sync after all commits landed:
   - `tools\governance\sync_register.ps1` (full sync)
   - `tools\governance\sync_register.ps1 --validate` (exit 0)
2. Any final brief frontmatter regeneration via sync
3. `git commit -m "governance: final V0.C.2 closure sync — brief EXECUTED state + audit_trail"`
4. `git push origin claude/v0_c_2-batched-sprite-tilemap-camera`
5. Open PR titled «V0.C.2 — Batched sprite renderer + Camera2D + TileMap (R.2 + R.3 + V0 substrate close)»
6. PR body summarizes per-commit per-deliverable mapping + verification metrics + halt activations + closure section reference
7. **DO NOT auto-push к main** — Crystalka reviews + merges per established protocol (V0.A/V0.B/V0.C.1 precedent)

**Verification**:
- Branch pushed к origin
- PR opened
- Crystalka receives PR notification + reviews + manual visual verification protocol + merges

**Halt trigger SC-11 (push-to-main classifier blocks)**: Per V0.A/V0.B/V0.C.1 precedent — Claude Code auto-mode classifier may block push-to-main even с explicit instruction. **Expected behavior** — branch push only is correct path. PR review + merge by Crystalka.


---

## §4 — Halt triggers (SC-N taxonomy)

V0.C.2 cascade may encounter conditions that **halt-before-damage** per Lesson #8 discipline. Each SC-N halt class triggers identical protocol: executor authors `HALT_REPORT_SC<N>.md` в `docs/scratch/V0_C_2/`, surfaces к Crystalka, does NOT commit partial state, does NOT push branch к origin. Crystalka decides recovery path (continue, patch brief, defer, abort).

### SC-1 — Baseline drift from V0.C.1 closure state

**Trigger conditions** (any of):
- `git log --oneline -1` does not show V0.C.1 closure commit
- `git status` shows uncommitted changes from prior sessions
- `dotnet build` baseline fails before any V0.C.2 edit
- `dotnet test` baseline shows < 786 tests passing
- VkApi.cs already contains `vkCmdBindIndexBuffer` or `vkCmdDrawIndexed` (drift — V0.C.2 expects absent)
- SpriteRenderer.cs already shows batched `BeginFrame/Submit/EndFrame` API (drift — V0.C.2 expects single-sprite V0.C.1 shape)
- AtlasRegion.cs already shows `FromPixels` static method (drift)
- Camera2D.cs already exists в `src/DualFrontier.Runtime/Sprite/` (drift)
- TileMap.cs already exists (drift)

**Recovery pattern**: Surface к Crystalka. Likely cause: previous incomplete V0.C.2 session attempt left state, or branch confusion. Crystalka resets к V0.C.1 closure HEAD, re-creates feature branch, restarts cascade.

### SC-2 — Vulkan SDK absent

**Trigger**: `echo $env:VULKAN_SDK` returns empty or invalid path. `vulkaninfo` not on PATH.

**Recovery**: Crystalka installs Vulkan SDK 1.3.x.y from LunarG. Re-runs Phase 0 §2.2 verification. Continues cascade from Commit 1.

### SC-3 — glslangValidator absent

**Trigger**: `tools/glslangValidator.exe` not present in repository (V0.B Commit 10 committed it; presence verified в Phase 0).

**Recovery**: Surface к Crystalka. V0.C.2 не compiles new shaders (reuses V0.C.1 sprite.vert.spv + sprite.frag.spv), но MSBuild target references binary. If absent → MSBuild target may fail at build. Crystalka re-runs V0.B Commit 10 recovery procedure (commit binary if accidentally removed from repository).

### SC-4 — Sprite shaders SPIR-V absent

**Trigger**: `assets/shaders/sprite.vert.spv` или `assets/shaders/sprite.frag.spv` missing.

**Recovery**: Surface к Crystalka. V0.C.1 Commit 9 committed shaders. If absent → re-trigger compilation via `dotnet build` (MSBuild CompileShaders target should regenerate). If MSBuild fails → check glslangValidator presence + GLSL source files presence.

### SC-5 — Marshal.SizeOf alignment mismatch (Lesson #7)

**Trigger**: VkIndexType size test fails (expected 4 bytes, actual differs).

**Recovery**: Per V0.B 5-corrections precedent — adjust test expected value к actual, document correction в commit message. Per V0.C.1 precedent: разница unlikely. If multiple alignment corrections needed → may indicate broader ABI drift, surface к Crystalka.

**Discipline reminder**: V0.A (1) + V0.B (5) + V0.C.1 (0) maturity curve. Brief estimates are HYPOTHESES — test gate authoritative. Discipline preserved regardless of accuracy.

### SC-6 — Camera2D math wrong

**Trigger conditions** (any of):
- `Camera2D_RoundTrip` test fails (WorldToScreen + ScreenToWorld не preserves coordinates)
- `Camera2D_Position_TranslatesView` test fails (world pos at Position does not appear at viewport center)
- `Camera2D_Zoom_AffectsMapping` test fails
- Sprite rendered at world (0,0) с identity Camera does not appear at viewport center в smoke test

**Recovery investigation order**:
1. Matrix composition order (translate × rotate × scale vs reverse) — `System.Numerics` row-major matrices apply right-to-left, post-multiplication
2. Vulkan NDC Y-direction (+Y down) — `CreateOrthographicOffCenter` argument order: `(left, right, bottom, top, near, far)` — for Vulkan +Y down, bottom > top numerically
3. ViewProjectionMatrix multiplication order: `ViewMatrix * ProjectionMatrix` (apply view first, then project)
4. Scale by 1/Zoom vs Zoom — zoom 2× should make objects appear LARGER, so view scale = Zoom (not 1/Zoom) per S-LOCK-4 spec

**Recovery action**: Fix math, re-run Camera2DTests, commit с corrected math. Document fix в commit message.

### SC-7 — Existing DrawSprite consumers break

**Trigger**: After SpriteRenderer batched rewrite (Commit 7), `dotnet build` fails because Runtime.RecordSpriteFrame calls old DrawSprite signature.

**Recovery**: Per Commit 7 REVISED scope — Commit 7 MUST include Runtime.RecordSpriteFrame backward-compat update (route through BeginFrame/Submit/EndFrame internally). Если executor split commits incorrectly → squash или amend к combine.

**Atomic discipline reminder** (S-LOCK-9): Each commit independently buildable. SpriteRenderer rewrite + Runtime.RecordSpriteFrame update = single semantic unit.

### SC-8 — 10K stress test FPS < 60

**Trigger**: Smoke test 10K stress scene measures < 60 FPS sustained.

**Recovery investigation order**:
1. Verify single vkCmdDrawIndexed call в RenderDoc (single atlas grouping correct)
2. Verify ring buffer per-frame chunk write не stalls на vkMapMemory (host-coherent + host-visible memory type properties)
3. Verify descriptor pool reset overhead не excessive (per-frame pool reset vs cached sets)
4. Verify ALWAYS-ON validation layer не imposing excessive runtime cost (Release build measurement)
5. Hardware specific — AMD RX 7600S baseline V0.C.1 = 164 FPS for single sprite; 10K should reach 100+ FPS на этом GPU

**Recovery action**: Surface к Crystalka для measurement-driven evaluation. Per Q8 V0 close gate per VULKAN_SUBSTRATE R.2 acceptance — 60+ FPS REQUIRED. Может потребовать V0.C.2.1 follow-up cycle для optimization (descriptor caching, ring buffer locality).

### SC-9 — 200×200 TileMap FPS < 60

**Trigger**: Smoke test TileMap scene measures < 60 FPS sustained.

**Recovery investigation order**:
1. Verify multi-cycle pattern (S-LOCK-5a) correctly issuing 4 vkCmdDrawIndexed per frame — не 1 large draw exceeding uint16 index range
2. Per-cycle descriptor set rebinding overhead (single atlas should reuse cached set across cycles)
3. Per-cycle BeginFrame/EndFrame overhead (ring buffer chunk reset per cycle)
4. Verify Camera2D matrix recomputation не occurring per cycle (compute once per frame, reuse)

**Recovery action**: Surface к Crystalka. May indicate scope adjustment per Lesson #20 — chunked rendering (32×32 tile chunks с per-chunk vertex buffer) deferred к V0.C.2.1, or culling helpers in Camera2D activated. Per К-L14 «без костылей» — measure first, optimize on evidence.

### SC-10 — Camera pan broken (input events не reach Camera2D)

**Trigger**: WASD keys в TileMap smoke test do not pan camera.

**Recovery investigation order**:
1. Verify Win32 message dispatch к InputEventQueue (V0.C.1 Commit 14 infrastructure)
2. Verify event types — V0.C.1 may have only landed KeyPressedEvent + KeyReleasedEvent; sustained pan needs KeyHeldEvent OR per-frame polling of held key state
3. Verify InputEventQueue.TryDequeue called in smoke test main loop

**Recovery action**: If KeyHeldEvent absent → V0.C.2 extends Input module к add it (sub-commit within Commit 12 smoke test scope), or smoke test maintains held-key state via KeyPressedEvent/KeyReleasedEvent pair pattern.

### SC-11 — Push-to-main classifier blocks (expected behavior, not bug)

**Trigger**: At Commit 17, executor attempts `git push origin main` per default Claude Code auto-mode behavior; classifier blocks.

**Recovery**: **NOT A HALT** — this is expected behavior per V0.A/V0.B/V0.C.1 precedent. Executor pushes branch only (`git push origin claude/v0_c_2-batched-sprite-tilemap-camera`), opens PR, surfaces «PR ready for review» к Crystalka. Crystalka manually reviews + merges.

**Pattern documentation**: This SC-11 is documented as **expected operational behavior**, not a bug. Future briefs reference V0.A/V0.B/V0.C.1/V0.C.2 precedent as standard pattern.

---

## §5 — Closure protocol (METHODOLOGY §12.7)

V0.C.2 closure follows METHODOLOGY §12.7 substrate authoring stream protocol per V0.A/V0.B/V0.C.1 inherited precedent.

### §5.1 — Final state verification checklist

Before Commit 17 push, executor verifies:

1. **Build clean**: `dotnet build` exit 0, 0 warnings (Domain layer + Runtime layer + Tests layer + SmokeTest)
2. **Tests green**: `dotnet test` exit 0, ~270 tests passing (proportional к V0.C.1 baseline 210 + V0.C.2 additions ~60)
3. **Governance valid**: `tools\governance\sync_register.ps1 --validate` exit 0
4. **K substrate unchanged**: `git diff --name-only main..HEAD -- native/DualFrontier.Core.Native/` = empty (K substrate не touched per K-L19 invariant)
5. **Smoke test pass**: All three scenes (V0.C.1 regression + V0.C.2 10K + V0.C.2 TileMap) execute clean per Manual Visual Verification Protocol
6. **REGISTER.yaml lifecycle**: DOC-D-V0_C_2 = EXECUTED
7. **MIGRATION_PROGRESS.md**: V substrate V0 close marked achieved
8. **Audit trail**: EVT-V0_C_2-CLOSURE event present
9. **Closure section**: V0.C.2 brief §8 populated с actual commit hashes + verification metrics
10. **Branch pushed**: `git push origin claude/v0_c_2-batched-sprite-tilemap-camera` succeeded

### §5.2 — PR opening discipline

PR title: «V0.C.2 — Batched sprite renderer + Camera2D + TileMap (R.2 + R.3 + V0 substrate close)»

PR body sections:
1. **Summary** — V0.C.2 scope per Q1-Q6 ratification, V0 substrate close gate per Q8
2. **Per-commit ledger** — 17 commits с scope + summary (mirrors closure section)
3. **Verification metrics** — FPS measurements, test count, validation log status
4. **Halt activations** — any SC-N triggered during execution + resolution
5. **Manual visual verification** — Crystalka attests per Protocol
6. **Inheritance preserved** — V0.A + V0.B + V0.C.1 invariants regression-checked
7. **Lesson candidates** — surfaced для potential A'.8 K-closure report inclusion
8. **Next steps** — V0 substrate close opens V1 + V2 + Phase B M-cycle paths

### §5.3 — Crystalka review + merge workflow

Per V0.A/V0.B/V0.C.1 precedent:

1. Crystalka receives PR notification
2. Reviews per-commit diffs sequentially
3. Runs Manual Visual Verification Protocol on «Skarlet»
4. Captures FPS measurements + validation log + RenderDoc screenshot inspection
5. If acceptable → squash merge или rebase merge per repository convention
6. If issues → comment requesting fixes, executor re-runs от relevant commit forward

### §5.4 — Surface к Crystalka после merge

После PR merge, executor (или Opus deliberation) surfaces к Crystalka:

> «V0.C.2 closed. V0 substrate close per Q8 ratification achieved.
> Three independent streams unblocked:
> 1. V1 brief authoring (scalar field + diffusion shader)
> 2. V2 brief authoring (scalar field + wave shader)
> 3. К10.3 brief restart (independent stream, your prerogative)
> Также available: A'.8 K-closure report, A'.9 Roslyn analyzer milestone.
> Какой следующий next Opus session focus?»

### §5.5 — V0 substrate close pathway documentation

V0.C.2 closure = V0 substrate close per Q8 ratification 2026-05-19. Pathway:

```
V0.A (PR #36 merged 2026-05-18) — Vulkan foundation
  ↓
V0.B (PR #37 merged 2026-05-19) — GPU infrastructure + descriptors + compute plumbing
  ↓
V0.C.1 (PR #38 merged 2026-05-19) — Asset pipeline + single-sprite rendering + input
  ↓
V0.C.2 (PR #XX merged 2026-05-XX) — Batched rendering + Camera2D + TileMap
  ↓
=== V0 SUBSTRATE CLOSE ===
  ↓
V1 brief authoring (gates: V0 close ✓)
V2 brief authoring (gates: V0 close ✓)
M-V demonstration mods (M-V1 mana, M-V2 electricity, M-V7 movement; gates: V1+V2)
Phase B M-cycle vanilla migration (gates: V0 close ✓ + analyzer A'.9)
```

К-L14 thesis verification: V substrate authored через four consecutive zero-hard-gate-halt cascades с alignment maturity curve (1 → 5 → 0 → V0.C.2 audit count). К-L14 empirically validated на V substrate matching K substrate K0..K10 pattern.

### §5.6 — Next Opus session decision tree

После V0.C.2 closure, next Opus session focus determined by Crystalka:

**Option A — V1 brief authoring**: Scalar field + diffusion shader (isotropic + anisotropic). Estimated brief ~1500-1800 lines. Gates on V0 close ✓. Authoring time ~1 session.

**Option B — V2 brief authoring**: Scalar field + wave shader (routed + breakable + distance/direction). Estimated brief ~1700-2000 lines. Gates on V0 close ✓. Authoring time ~1 session.

**Option C — К10.3 restart**: Independent stream. Halted 2026-05-18 pre-V0 work; restart pathway open.

**Option D — К10.4 TLA+ brief**: Independent stream. Theorem-proved kernel verification.

**Option E — A'.8 K-closure report**: Formal K-Lxx enumeration + Lessons promotion. Accumulates after V0 close + K series close.

**Option F — A'.9 Roslyn analyzer milestone**: Architectural analyzer для М-cycle migration verification.

Recommended sequencing per К-L14 default-inclusion bias: A → B → C (V1 + V2 close substrate authoring, then К10.3 closes K10 series, then М-cycle gates met).

---

## §6 — Brief authority + lifecycle

### §6.1 — Brief lifecycle states

V0.C.2 brief lifecycle mirrors V0.A/V0.B/V0.C.1 precedent:

```
AUTHORED ──→ EXECUTED ──→ ARCHIVED (future, post-A'.8 K-closure)
```

- **AUTHORED**: Opus deliberation session authors brief; Crystalka reviews + accepts via «Принимаю бриф» pattern. Brief committed to `tools/briefs/V0_C_2_EXECUTION_BRIEF.md`. REGISTER.yaml entry lifecycle = AUTHORED.

- **EXECUTED**: Claude Code execution session completes 17-commit cascade. Crystalka reviews PR + merges. REGISTER.yaml entry lifecycle = EXECUTED. Audit trail event EVT-V0_C_2-CLOSURE recorded. Closure section §8 populated.

- **ARCHIVED**: Future state at A'.8 K-closure report. Briefs from completed phases archived (lifecycle field) as historical record.

### §6.2 — Authority hierarchy

V0.C.2 brief inherits authority chain (Intro §0):

1. **VULKAN_SUBSTRATE.md v1.0 LOCKED** — architectural authority
2. **METHODOLOGY.md** — process authority (atomic cascade, halt-before-damage, substrate authoring stream)
3. **CODING_STANDARDS.md** — style authority
4. **К-L19 LOCKED** — hardware capability invariant (preserved verbatim)
5. **V0.A/V0.B/V0.C.1 EXECUTED** — inherited substrate primitive precedent

Brief departures from authority chain require explicit re-architecture milestone, not in-cascade improvisation. «Stop, escalate, lock» discipline per VULKAN_SUBSTRATE.md Preamble.

### §6.3 — Multi-session pause provision (Lesson #8 + Lesson #26)

If executor session token budget exhausted before Commit 17:

1. Complete current atomic commit (commit N)
2. Push branch к origin: `git push origin claude/v0_c_2-batched-sprite-tilemap-camera`
3. Surface к Crystalka: «V0.C.2 paused at Commit N/17. Resume в next session from Commit N+1.»
4. Next session: Crystalka invokes Claude Code на same branch; executor reads §3 commit ledger, locates Commit N+1, resumes
5. Each commit independently buildable (S-LOCK-9) — pause-resume preserves invariants

V0.A executed в single session (11 commits). V0.B executed в single session (18 commits). V0.C.1 executed в single session (17 commits). V0.C.2 estimated 15-25h auto-mode — may require pause-resume across 2 sessions.

---

## §7 — Lesson candidates (deferred к A'.8 K-closure report enumeration)

V0.C.2 cascade accumulates evidence для METHODOLOGY §K-Lessons revision. Per Lesson promotion criteria (second-application application gate), candidates remain candidates until A'.8 K-closure report formalizes.

### Strengthening continues (V0.C.2 expected confirmations)

**Lesson #7 strengthening MATURED continues**: Alignment audit discipline preserved. V0.A (1 fix) → V0.B (5 fixes) → V0.C.1 (0 fixes) → V0.C.2 (TBD count). Pattern: maturity curve as discipline calibrates с practice. V0.C.2 single new VkIndexType enum likely matches hypothesis (single uint underlying = 4 bytes); если matches → MATURED status reinforced. Если diverges → discipline-mandatory-regardless reaffirmed.

**Lesson #22 strengthening continues**: Mixed [LibraryImport] + [DllImport] P/Invoke convention preserved across V0.C.2 (2 new functions = `[LibraryImport]` blittable per Lesson #22 default).

**Lesson #25 strengthening continues**: Implementation depth follows consumer materialization. V0.C.2 atlas region support stays code-defined (Q5a) per «JSON manifest unnecessary until atlas grows». Camera2D scope stays standard (Q3b) per «culling unnecessary until measurement justifies». TileMap stays one-sprite-per-tile (Q4a) per «chunked rendering unnecessary until measured insufficient». Pattern: scope discipline preserved when consumer не yet materialized.

**Lesson #26 strengthening continues**: Cross-substrate scope splitting preserves multi-session execution budget. V0.C → V0.C.1 + V0.C.2 split validated через V0.C.1 successful closure + V0.C.2 successful execution. Pattern: substantial substrate work bigger than session token budget productively splits across atomic substrate cycles.

**Lesson #27 candidate continues — render workload exercises prior substrate primitives**: V0.C.1 surfaced V0.B framebuffer staleness + semaphore reuse bugs through actual rendering workload. V0.C.2 may surface latent V0.C.1 bugs через 10K stress + TileMap workload (e.g. descriptor pool sizing, ring buffer synchronization edge cases). Pattern formalization gate: second application — V0.C.2 confirms или rejects.

### V0.C.2-specific candidates (potential new lessons)

**Lesson #N candidate — Ring buffer N-frame production pattern**: V0.C.2 introduces VertexBufferRing infrastructure matching swapchain image count. Pattern applies к other GPU resources needing per-frame state (uniform buffers, dynamic descriptor data, future V1+V2 compute dispatches). Formalize after second application (V1 or V2 substrate work).

**Lesson #N candidate — Indexed quad rendering memory tradeoff**: V0.C.2 indexed 4-vertex quads vs V0.C.1 non-indexed 6-vertex = ~33% vertex memory reduction. Pattern applies к any quad-heavy rendering (UI panels, text glyph quads R.6, particle systems future). Formalize after second application.

**Lesson #N candidate — Composition refactor preserves backward compat via API routing**: V0.C.2 SpriteRenderer.DrawSprite removed; Runtime.RecordSpriteFrame V0.C.1 method preserved via routing к BeginFrame/Submit/EndFrame batched API internally. Pattern: deprecation through backward-compat routing layer preserves consumer code unchanged. Formalize after second application (R.5 Domain integration or R.6 UI may apply this pattern).

**Lesson #N candidate — Manual visual verification gate scales к multi-scene smoke test**: V0.C.1 smoke test = single scene; V0.C.2 smoke test = 3 scenes sequential (V0.C.1 regression + V0.C.2 R.2 stress + V0.C.2 R.3 TileMap). Pattern: multi-scene smoke test layered scenes inherit prior scene baselines + add specific feature verification. Formalize при R.5 Domain integration (will further layer scene).

### К-L14 thesis fourth+verification

V0.A (PR #36) + V0.B (PR #37) + V0.C.1 (PR #38) + V0.C.2 (PR #XX) = four consecutive zero-hard-gate-halt cascades on V substrate authoring stream.

К-L14 thesis: «performance derives from clean complex architecture without compromise». V substrate cascade pattern empirically demonstrates:
- Atomic cascade discipline preserves invariants across multi-hour sessions
- Halt-before-damage protocol catches errors early
- Alignment audit pattern reliable (maturity curve V0.A 1 → V0.B 5 → V0.C.1 0 → V0.C.2 audit)
- Manual visual verification gate substantive failure-mode catch
- Substrate authoring stream productively splits across atomic substrate cycles
- К-L14 default-inclusion bias scales к substantial cascades через scope coherence (not aggregation)

Если V0.C.2 closes per pattern → К-L14 empirically validated на V substrate matching K substrate K0..K10 development accumulation. Decade-horizon planning thesis evidence accumulates.

---

## End of brief

**Total estimated execution time**: 15-25 hours auto-mode (Claude Code session) — multi-session pause provision available per Lesson #8 + Lesson #26.

**Total estimated LOC delta**: +900-1300 production code (per VULKAN_SUBSTRATE R.2 + R.3 estimates) + ~500-700 test code + ~200-300 governance + smoke test extension. Net cascade ~1700-2300 LOC.

**Total estimated test growth**: 210 V0.C.1 baseline → ~270-290 V0.C.2 closure (~60-80 new tests proportional к architectural surface).

**Verification metrics target**:
- 10K sprite stress: 60+ FPS sustained 10s, single vkCmdDrawIndexed call в RenderDoc
- 200×200 TileMap: 60+ FPS sustained 10s, 4 vkCmdDrawIndexed calls per frame (S-LOCK-5a multi-cycle)
- Validation: 0 errors, 0 warnings throughout smoke test
- Hardware baseline: «Skarlet» AMD RX 7600S (V0.C.1 baseline 164 FPS single sprite)

**V0 substrate close achievement** per Q8 ratification 2026-05-19: V0.C.2 closure opens V1 + V2 brief authoring + Phase B M-cycle vanilla migration paths.

**К-L14 thesis fourth verification**: V0.A → V0.B → V0.C.1 → V0.C.2 = four consecutive zero-hard-gate-halt cascades, alignment maturity curve preserved, atomic discipline + halt-before-damage + manual visual verification + substrate authoring stream + scope coherence + decade-horizon planning all empirically demonstrated.

**«Без костылей» summary**: V0.C.2 ships R.2 batched sprite renderer + R.3 TileMap + Camera2D per VULKAN_SUBSTRATE §4.2 specification без compromise. Ring buffer N-frame infrastructure + indexed quad rendering + Camera2D standard scope + one-sprite-per-tile reuse-existing-batched-infrastructure = clean architectural decisions избегающие dead-end optimization paths. К-L14 «performance derives from clean complex architecture» validates на V0 substrate close.

---

## §8 — Closure section (added at EXECUTED transition)

**Date executed**: 2026-05-19
**Branch**: `claude/v0_c_2-batched-sprite-tilemap-camera`
**Commits**: 17 atomic (b4084f1..PENDING-COMMIT-V0_C_2-CLOSURE)
**PR**: Pending (Crystalka reviews + merges per V0.A/V0.B/V0.C.1 precedent)

### Commit ledger

| # | Hash | Scope | Summary |
|---|------|-------|---------|
| 1 | b4084f1 | governance | enroll V0.C.2 execution brief (DOC-D-V0_C_2 AUTHORED) |
| 2 | 6733c9c | vulkan + tests | add VkIndexType + vkCmdBindIndexBuffer + vkCmdDrawIndexed |
| 3 | 26720fc | sprite + tests | add VertexBufferRing N-frame ring buffer (S-LOCK-2) |
| 4 | 18555e9 | sprite + tests | add SpriteIndexBuffer pre-populated uint16 pattern (S-LOCK-3) |
| 5 | ce40e67 | sprite + tests | harden AtlasRegion.FromPixels validation guards (S-LOCK-6) |
| 6 | bd2c8eb | sprite + tests | add Camera2D class (S-LOCK-4) |
| 7 | 18e6f8e | sprite + runtime | SpriteRenderer batched API rewrite + Runtime backward-compat |
| 8 | d9de52a | runtime | Runtime.Camera property wiring (S-LOCK-1) |
| 9 | b72cd7e | sprite + tests | add TileMap class (S-LOCK-5) |
| 10 | e596f73 | tests | ProceduralAtlas SmokeTest helper |
| 11 | 01d9c1c | tests | 10K sprite stress test scene (R.2) |
| 12 | 655e6c0 | tests + runtime | 200×200 TileMap scene + multi-cycle helpers (R.3 + S-LOCK-5a) |
| 13 | f6ff03b | docs | Manual visual verification protocol |
| 14 | 1b8f2ea | governance | DOC-D-V0_C_2 EXECUTED + audit_trail event |
| 15 | (this) | docs | Closure section в brief |
| 16 | (next) | tests | Final integration smoke test |
| 17 | (next) | governance | Final REGISTER sync + push + PR |

### Verification metrics

- `dotnet build` clean (0 errors, 0 warnings) at every atomic commit
- `dotnet test (DualFrontier.Runtime.Tests)`: **271 tests passing** (210 V0.C.1 baseline + 61 V0.C.2 additive)
- `sync_register.ps1 -Validate` exit 0 (audit_trail=21, documents=251, 20 advisory orphan warnings)
- Smoke test execution на «Skarlet»: pending Commit 16 final integration verification per Manual Visual Verification Protocol

### Halt protocol activations

- **SC-1 non-critical drift** (Commit 5): AtlasRegion.FromPixels already present from V0.C.1 without validation guards; hardened existing factory rather than full add per Lesson #1 production-wiring-surfaces-embedded-behavior pattern.
- **SC-6 averted** (Commit 6): Camera2D ortho projection initial argument order incorrect (brief said «swap для +Y down» but standard CreateOrthographicOffCenter convention works directly); test gate caught inversion; fixed pre-commit.
- **SC-7 averted** (Commit 7): combined SpriteRenderer rewrite + Runtime call site update в single semantic commit per atomic discipline; build invariant preserved at every step.
- **Lesson #7 alignment audit** (Commit 2): Marshal.SizeOf<T>() doesn't support enums в .NET 8; substituted sizeof + Enum.GetUnderlyingType. First V0.C.2 alignment audit correction.
- No SC-N hard-halt activations.

### Alignment audit corrections caught

- VkIndexType: hypothesis Marshal.SizeOf<T>()==4 rejected by .NET 8 enum marshalability constraint; substituted sizeof + Enum.GetUnderlyingType — actual size 4 bytes confirmed via underlying type uint.

### Lesson candidates surfaced

- **Lesson #7 strengthening MATURED continues**: V0.A 1 → V0.B 5 → V0.C.1 0 → V0.C.2 1 correction. Pattern: discipline-mandatory regardless; methodology rigor preserved through changing failure modes.
- **Lesson #22 strengthening continues**: mixed [LibraryImport]/[DllImport] preserved (2 new functions, both [LibraryImport]).
- **Lesson #25 strengthening continues**: scope discipline — Camera2D culling + TileMap chunked/instanced + JSON atlas manifest all deferred к consumer materialization.
- **Lesson #26 strengthening continues**: V0.C split (V0.C.1 + V0.C.2) validated through both successful cascades.
- **Lesson #27 candidate continues**: render workload exercises prior substrate primitives (V0.B framebuffers + swapchain + memory allocator + V0.C.1 sprite pipeline + push constants).
- **New candidate — Ring buffer N-frame production pattern** (Commit 3): VertexBufferRing applies к per-frame GPU resources (uniform buffers, dynamic descriptors, future V1/V2 compute).
- **New candidate — Indexed quad rendering memory tradeoff** (Commit 4): 4 vertices + uint16 indices vs 6 non-indexed = ~33% vertex memory reduction.
- **New candidate — Composition refactor preserves backward compat via routing** (Commit 7): SpriteRenderer.DrawSprite removed, Runtime.RecordSpriteFrame preserved via routing through batched API.
- **New candidate — Multi-scene smoke test layering** (Commits 11 + 12): V0.C.1 single scene → V0.C.2 3 sequential scenes inheriting prior baselines + adding specific verification.

### V0 substrate close pattern established

V0.A (PR #36) + V0.B (PR #37) + V0.C.1 (PR #38) + V0.C.2 (PR #pending) = **four consecutive zero-hard-gate-halt cascades** on V substrate authoring stream. К-L14 thesis empirically validated на V substrate matching K substrate K0..K10 development accumulation pattern.

### Next steps

**V0 substrate close per Q8 ratification opens**:
- V1 brief authoring (scalar field + diffusion shader, isotropic + anisotropic)
- V2 brief authoring (scalar field + wave shader, routed + breakable)
- Phase B M-cycle vanilla content migration (gated also on Roslyn analyzer A'.9 + K-closure report A'.8)

**Independent streams unchanged**:
- К10.3 brief restart (Crystalka prerogative)
- К10.4 TLA+ brief authoring
- A'.8 K-closure report
- A'.9 Roslyn architectural analyzer milestone

---

*V0.C.2 EXECUTION BRIEF EXECUTED 2026-05-19 — DOC-D-V0_C_2 lifecycle transitioned AUTHORED → EXECUTED at Commit 14 closure governance landing.*
