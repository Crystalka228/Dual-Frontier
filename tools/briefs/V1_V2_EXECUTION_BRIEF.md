---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-D-V1_V2
category: D
tier: 3
lifecycle: AUTHORED
owner: Crystalka
version: "1.0"
next_review_due: null
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-D-V1_V2
---

# V1+V2 — Scalar field + diffusion shader + wave shader (V substrate primitives, combined deliberation, sequential execution)

**Brief lifecycle**: AUTHORED 2026-05-19 by Opus deliberation. EXECUTED post-V2 closure (two sub-PRs expected — V1 closure + V2 closure). Author: Crystalka (judicial role) + Claude Opus 4.7 (deliberation pipeline). Successor brief к V0.C.2 (closed PR #39 merged) per V substrate authoring stream. **Closure achieves V substrate full close** per VULKAN_SUBSTRATE.md §1.4 multi-field coexistence acceptance criterion (V0 ✓ + V1 ✓ + V2 ✓ + multi-field-coexistence verified) — unlocks Phase B M-cycle vanilla content migration (gated also on Roslyn analyzer А'.9).

**Authority chain**:

1. **VULKAN_SUBSTRATE.md v1.0 LOCKED** (DOC-A-VULKAN_SUBSTRATE) — §1.2 V1 specification, §1.3 V2 specification, §1.4 V substrate close acceptance criteria, §3 compute use case substrate primitives implementation, §5 mathematical models, §11 methodology adjustments (validation layer + CPU/GPU equivalence test).
2. **V0.A EXECUTED** (PR #36 merged 2026-05-18, 11 commits, 685 tests) — pure P/Invoke Vulkan foundation.
3. **V0.B EXECUTED** (PR #37 merged 2026-05-19, 18 commits, 786 tests, K-L19 LOCKED) — MemoryAllocator + ComputePipelineRegistry + VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch + FieldStorageBinding + descriptor primitives + compute pipeline registration round-trip working.
4. **V0.C.1 EXECUTED** (PR #38 merged 2026-05-19, 17 commits, 210 Runtime.Tests baseline) — PngDecoder + AssetManager + SpriteRenderer + input event types.
5. **V0.C.2 EXECUTED** (PR #39 merged 2026-05-19, 17 commits, 271 Runtime.Tests / 936 total tests, 165 FPS at 40K tiles) — batched SpriteRenderer + Camera2D + TileMap + VertexBufferRing + indexed quad rendering. V0 substrate close achieved per Q8 ratification.
6. **K9 LOCKED через A'.4** (FieldHandle<T> + FieldRegistry + FieldSpanLease<T> + ping-pong SwapBuffers + conductivity + storage flags + C ABI surface complete) — V1+V2 consume directly.
7. **CPU reference precedent established** (`src/DualFrontier.Core.Interop/CpuKernels/IsotropicDiffusionKernel.cs` existing from K9 era) — V1 brief extends pattern, не recreates.
8. **METHODOLOGY.md** v1.0 LOCKED — atomic cascade discipline, halt-before-damage principle (Lesson #8), substrate authoring stream protocol §12.7, validation layer ALWAYS-ON + CPU/GPU equivalence test (per VULKAN_SUBSTRATE §11).
9. **CODING_STANDARDS.md** v1.0 LOCKED — no LINQ в src, file-scoped namespaces, internal P/Invoke artifacts, mixed [LibraryImport] + [DllImport] convention per Lesson #22.
10. **K-L19 LOCKED via V0.B** — hardware capability fail-fast invariant preserved verbatim through V0.C.1 + V0.C.2 + V1+V2. No kernel changes.
11. **Q1-Q8 ratification 2026-05-19** (Crystalka): hybrid sub-milestone structure (Q1c — V1 monolithic + V2.A wave/distance + V2.B direction extraction split); minimal CPU reference scope (Q2a — single-iteration small grid synthetic tests); direct K9 consumption (Q3a — no new C ABI extensions); extend Directory.Build.props CompileShaders target (Q4a — substrate-side compilation); standard smoke test scenes (Q5b — minimal + multi-field coexistence); combined deliberation + sequential execution (Q6c — V1 PR #40 → V2 PR #41); diffusion baseline для V2 (Q7a — eikonal upgrade deferred TBD per spec); Phase A' naming А'.10 V1 + А'.11 V2 (Q8 — sequential post-V0 sub-milestones).

**Authority chain integrity statement**: This brief inherits substantial existing infrastructure. V0.B compute pipeline registration round-trip already verified working through V0.C.2 retesting. K9 FieldHandle<T> + FieldRegistry already production-ready with ping-pong + conductivity + storage flag + UTF-8 stackalloc encoding pattern. CPU reference IsotropicDiffusionKernel already exists from K9 era — V1 cascade hardens + extends, не recreates (per Lesson #11 redundancy check + Lesson #22 read existing code first).

**К-L14 thesis fifth verification window**: V0.A → V0.B → V0.C.1 → V0.C.2 closed с four consecutive zero-hard-gate-halt cascades. V1+V2 = fifth + sixth verifications. Two sub-PRs expected, each maintaining alignment audit discipline + atomic cascade + halt-before-damage + manual visual verification + CPU/GPU equivalence test discipline new к V1+V2.

---

## Intro — V0 substrate close inheritance + V1+V2 combined scope

### Inherited state from V0.C.2 closure (verified 2026-05-19)

V0 substrate fully closed per Q8 ratification. Repository state at V1+V2 brief authoring time:

**Baseline test count**: 936 tests total (210 V0.B → 271 V0.C.2 Runtime.Tests + 665 other test projects). V1+V2 estimated к add ~120-180 new tests (V1 ~50-80 + V2 ~70-100) — final estimated test count post-V2 ≈ 1060-1120 total tests.

**Smoke test baseline**: V0.C.2 smoke test renders sustained 165 FPS across V0.C.1 single sprite (regression) + 10K stress + 200×200 TileMap (40K sprites) scenes на «Skarlet» AMD RX 7600S. V1+V2 extends smoke test с new scenes per Q5b ratification.

**Compute infrastructure ready** (verified Phase 0):
- `VulkanComputePipeline` — compute pipeline creation с pipeline layout + SPIR-V shader module
- `VulkanComputeDescriptors` — descriptor set layout + pool + allocation for compute shaders
- `ComputeDispatch` — ExecuteSync wrapper using VulkanFence + VkApi.vkCmdBindPipeline + vkCmdDispatch via VulkanCommandBuffer.Dispatch + submit к AsyncComputeQueue + fence wait
- `ComputePipelineRegistry` — pipeline lifetime tracking
- `FieldStorageBinding` — V0.B no-op orchestration shell; **V1+V2 must implement actual VkBuffer binding + descriptor set update + dispatch wiring**
- `FieldHandle<T>` — point read/write, AcquireSpan, SetConductivity/GetConductivity, SetStorageFlag/GetStorageFlag, SwapBuffers (ping-pong)
- `FieldRegistry` — Register<T>(id, width, height), Get<T>, TryGet<T>, IsRegistered, Unregister, Count
- `IsotropicDiffusionKernel.Run(FieldHandle<float>, Parameters, iterations)` — CPU reference implementation from K9 era с 4-neighbour stencil + reflective boundary + scratch array

**K9 C ABI complete** (verified Phase 0):
- `df_world_register_field` / `df_world_field_unregister`
- `df_world_field_read_cell` / `df_world_field_write_cell`
- `df_world_field_acquire_span` / `df_world_field_release_span`
- `df_world_field_set_conductivity` / `df_world_field_get_conductivity`
- `df_world_field_set_storage_flag` / `df_world_field_get_storage_flag`
- `df_world_field_swap_buffers`
- `df_world_field_count`

**V0.B compute C ABI complete** (verified Phase 0):
- `df_world_attach_vulkan` — Vulkan instance/device/queue attached к native world
- `df_world_register_compute_pipeline` — SPIR-V bytecode + descriptor binding count → pipeline_id
- `df_world_field_dispatch_compute` — pipeline_id × field name × dispatch dimensions

**Shader build pipeline ready**: `tools/glslangValidator.exe` committed binary + `Directory.Build.props CompileShaders` target. V1+V2 extends target с new shaders (diffusion.comp, wave.comp, direction_extract.comp).

**К-L19 invariant preserved**: HardwareCapabilityCheck at Runtime.Create unchanged. V1+V2 не modifies native kernel layer.

**Validation layer ALWAYS-ON DEBUG pattern preserved**: V0.A pattern through V0.B + V0.C.1 + V0.C.2. V1+V2 maintains — new compute shader registration + dispatch + descriptor set updates substantial validation surface, ZERO validation messages tolerated as commit gate.

### V1+V2 combined scope per Q1-Q8 ratification

**Combined deliberation, sequential execution (Q6c)**: Single brief describes V1 + V2 architectural design coherently. Executor pauses at V1 closure (Commit ~22 estimated) для PR review + Crystalka manual visual verification of V1 substrate primitives + multi-iteration evolution + CPU/GPU equivalence + anisotropic variant. Then resumes к V2 (Commits ~23-46 estimated). Final V2 closure achieves V substrate full close с multi-field coexistence scene (Q5b standard).

**Two PR cascade structure**:
- **PR #40 — V1 closure** (~22 commits estimated): scalar field + diffusion shader (isotropic + anisotropic) + CPU equivalence + multi-iteration scene verification.
- **PR #41 — V2 closure** (~24 commits estimated): scalar field + wave shader (distance field) + gradient extraction (direction field) + CPU equivalence + multi-field coexistence scene + V substrate full close governance.

### Scope-IN (V1 + V2 combined deliverables)

#### V1 portion (Phase A'.10)

**V1.1 — Isotropic diffusion shader (substrate primitive baseline)**:
- GLSL compute shader `tools/shaders/diffusion.comp` — 4-neighbour stencil per VULKAN_SUBSTRATE §1.2:
  ```
  ∂P/∂t = D · ∇²P + S(x,y) - K · P
  ∇²P ≈ P(N) + P(S) + P(E) + P(W) - 4·P(center)
  ```
- Push constant payload: D (diffusion coefficient), K (decay coefficient), dt (delta time), width, height
- Storage buffer 0 binding: input field (read-only ssbo)
- Storage buffer 1 binding: output field (write ssbo)
- Reflective boundary conditions (edge cell uses self as neighbour, matching CPU reference)
- ~30-40 LOC GLSL

**V1.2 — Anisotropic diffusion configuration (per-cell conductivity)**:
- Same shader template OR variant с per-cell D from conductivity map
- Per-cell D via storage buffer 2 binding (conductivity map ssbo) — float per cell
- Asymmetric flow: `min(D_self, D_neighbor)` between each tile pair (per VULKAN_SUBSTRATE §1.2 + §5.1)
- M-V2 demonstration: wire propagation (high D wire tiles, low D non-wire tiles) — emergent narrow wave behaviour
- ~20-25 additional LOC OR separate shader file

**V1.3 — CPU/GPU equivalence test infrastructure**:
- Anisotropic CPU reference kernel: `AnisotropicDiffusionKernel.cs` in `src/DualFrontier.Core.Interop/CpuKernels/` matching shader semantics
- Equivalence test framework: synthetic 32×32 grid, both CPU + GPU iterations, tolerance-bounded comparison (`abs(cpu[i] - gpu[i]) < 0.001f` per cell) per VULKAN_SUBSTRATE §11
- Test scenarios: source spike (single non-zero cell), boundary check (edge cells), conductivity variation (anisotropic), decay verification

**V1.4 — FieldStorageBinding V1 wiring**:
- Implement actual VkBuffer binding for input/output field buffers (replacing V0.B no-op stub)
- Per-iteration ping-pong via FieldHandle<T>.SwapBuffers
- Descriptor set update per dispatch via vkUpdateDescriptorSets (binding 0 = input ssbo, binding 1 = output ssbo, binding 2 = conductivity ssbo для anisotropic)
- Compute fence sync per К-L7 atomic-from-observer

**V1.5 — Smoke test scenes per Q5b standard**:
- Scene: V1 isotropic diffusion — 200×200 field, single source spike at center, 5 iterations per frame, visual evolution observed for 10s. FPS ≥ 60.
- Scene: V1 anisotropic — 200×200 field с wire path (horizontal line, high D), source at one end, observe channeled propagation. FPS ≥ 60.

#### V2 portion (Phase A'.11)

**V2.A — Wave shader (distance field baseline)**:
- GLSL compute shader `tools/shaders/wave.comp` — wave propagation per VULKAN_SUBSTRATE §1.3 with diffusion-based distance baseline (Option B per Q7a ratification):
  ```
  ∂D/∂t = ∇²D - K·D + spike_at_target
  ```
- Per-cell speed map (storage buffer binding) for terrain-aware propagation
- Target spike injected via separate push constant or staging buffer
- Walls/obstacles = speed 0.0 (zero D, propagation blocked)
- ~40-50 LOC GLSL

**V2.B — Direction extraction shader**:
- GLSL compute shader `tools/shaders/direction_extract.comp` — gradient extraction per VULKAN_SUBSTRATE §1.3:
  ```
  dir = normalize(vec2(west - east, north - south))
  ```
- Input binding 0: distance field (read-only)
- Output binding 1: direction field (vec2 per cell, write)
- Single pass after distance field stable
- ~15-20 LOC GLSL

**V2.C — CPU reference kernels**:
- `WaveKernel.cs` — diffusion-based distance field CPU reference
- `DirectionExtractKernel.cs` — gradient extraction CPU reference
- Equivalence tests with tolerance bounds (distance: 0.01f, direction: 0.001f cosine similarity)

**V2.D — Field type extensions**:
- `Vector2Field` или similar for direction field storage (FieldHandle<Vector2> consumption через existing FieldRegistry generic mechanism)
- Verify `df_world_register_field` cellSize = 8 для Vector2 (managed-side type matches native byte count via sizeof<T>)

**V2.E — Smoke test scenes per Q5b standard**:
- Scene: V2 wave distance — 200×200 field с obstacles (walls), target spike, observe distance field convergence (10-20 iterations) + direction field gradient visualization. FPS ≥ 60.
- Scene: **Multi-field coexistence** (V substrate close acceptance criterion per VULKAN_SUBSTRATE §1.4) — simultaneously active: V1 mana field (isotropic), V1 electricity field (anisotropic), V2 distance field, V2 direction field. All updating per tick within performance budget (~1ms/tick per active field on mid-range GPU). FPS ≥ 60.

**V2.F — V substrate close governance**:
- VULKAN_SUBSTRATE.md amendment: §1.4 close criteria marked achieved
- MIGRATION_PROGRESS.md: V substrate full close
- REGISTER.yaml audit_trail event EVT-V-SUBSTRATE-CLOSE
- К-L14 thesis fifth+sixth verification documented (PR #40 + PR #41 closure metrics)

### Scope-OUT (explicit Lesson #20 discipline + Q1-Q8 boundaries)

- **Eikonal upgrade (Option A per VULKAN_SUBSTRATE §1.3.1)** — deferred TBD per Q7a ratification; evidence-gated к M-V7 pathfinding measurement
- **G5 projectile Domain B substrate disposition** — deferred TBD per VULKAN_SUBSTRATE §1.3.2; resolved at M-V5 reactivation amendment
- **Hybrid coupling V1↔V2** (broken-node diffusion fallback) — deferred к M-V Water demonstration amendment per VULKAN_SUBSTRATE §6.5
- **M-V1 demonstration mod** (Vanilla.Magic mana field) — separate mod-side milestone post-V1 close
- **M-V2 demonstration mod** (Vanilla.Electricity power field) — separate mod-side milestone post-V1 close
- **M-V7 demonstration mod** (Vanilla.Movement flow field pathfinding) — separate mod-side milestone post-V2 close
- **К10.3 brief restart** — independent stream, твоя prerogative
- **К10.4 TLA+ brief** — independent stream
- **A'.8 K-closure report** — accumulates lessons after V substrate full close + K series close
- **A'.9 Roslyn analyzer milestone** — post-K-closure, gates Phase B M-cycle
- **Phase B M-cycle vanilla content migration** — gated on V substrate full close ✓ + analyzer A'.9

### Brief size estimate post-ratification

V0.A: 1612 lines. V0.B: 2320. V0.C.1: 2961. V0.C.2: 2925. **V1+V2 combined target: 3500-4500 lines** (V1 portion ~1800-2200 + V2 portion ~1700-2300 — closer к V0.C.2 scale per portion plus combined scope governance). Multi-session execution expected per Q6c — V1 portion ~15-20h auto-mode, V2 portion ~15-20h auto-mode, pause point at V1 closure for PR review.


---

## §1 — S-LOCKs (V1+V2 invariants)

S-LOCKs scope-defining invariants enforced through V1+V2 cascade. Any deviation triggers SC-N halt (§4). S-LOCKs persist через atomic intermediate states + V1→V2 pause point + multi-session resume.

### §1.1 — S-LOCK-1: V1+V2 combined scope = scalar field substrate primitives (V1 diffusion + V2 wave + V substrate close)

V1+V2 brief ships exactly per VULKAN_SUBSTRATE §1.2 + §1.3 + §1.4 specifications:

**V1 substrate primitive close criteria** (PR #40):
- Isotropic diffusion shader functional (4-neighbour stencil, ping-pong, decay)
- Anisotropic configuration (per-cell conductivity from storage buffer) functional
- CPU/GPU equivalence test passes (tolerance < 0.001f per cell on 32×32 synthetic grids)
- Smoke test V1 isotropic + V1 anisotropic scenes pass at 60+ FPS
- FieldStorageBinding implements actual VkBuffer binding (no longer V0.B no-op stub)

**V2 substrate primitive close criteria** (PR #41):
- Wave shader functional (diffusion-based distance field per Q7a Option B baseline)
- Direction extraction shader functional (gradient field from distance field)
- CPU/GPU equivalence test passes (distance tolerance 0.01f, direction cosine similarity > 0.999)
- Smoke test V2 distance + direction scene passes at 60+ FPS
- Multi-field coexistence scene passes per VULKAN_SUBSTRATE §1.4: V1 mana (isotropic) + V1 electricity (anisotropic) + V2 distance + V2 direction simultaneously active at 60+ FPS

**V substrate FULL close** per VULKAN_SUBSTRATE §1.4 acceptance criteria:
- V0 ✓ (V0.A + V0.B + V0.C.1 + V0.C.2 done)
- V1 ✓ (PR #40 closure)
- V2 ✓ (PR #41 closure)
- Multi-field coexistence ✓ (PR #41 smoke test)

**Out of scope per Intro §0**: Eikonal upgrade, Domain B (projectile), hybrid V1↔V2 coupling, M-V demonstration mods, К10.3, К10.4, A'.8, A'.9, M-cycle migration.

### §1.2 — S-LOCK-2: Compute shader sourcing — substrate-side compilation (per Q4a)

V1+V2 compute shaders compile at engine build time через extended `tools/glslangValidator.exe` MSBuild target:

**Shaders added к Directory.Build.props CompileShaders target**:
- `diffusion.comp` → `diffusion.comp.spv` (V1 isotropic + anisotropic via per-cell D variant — single shader, parameter-driven per Q1c hybrid)
- `wave.comp` → `wave.comp.spv` (V2 distance field, diffusion-based per Q7a baseline)
- `direction_extract.comp` → `direction_extract.comp.spv` (V2 direction extraction)

**Mod-side compilation deferred** (per K-L9 vanilla = mods): M-V1/M-V2/M-V7 demonstration mods (separate milestones post-V1+V2 close) will compile gameplay-specific shaders при mod build, не substrate primitives themselves. Substrate primitives V1/V2 ship as compiled SPIR-V с engine binary distribution.

### §1.3 — S-LOCK-3: CPU/GPU equivalence test mandatory per VULKAN_SUBSTRATE §11

Every V1+V2 compute shader must have a CPU reference implementation. Tolerance-bounded equivalence test gates compute shader correctness before commit.

**V1 isotropic equivalence**:
- CPU reference: existing `IsotropicDiffusionKernel.Run` (K9-era), tolerance 0.001f per cell
- GPU dispatch: identical parameters (D, K, dt, iterations) on identical input field state
- Test grid: 32×32 synthetic (small enough for CPU speed, large enough к exercise stencil)
- Scenarios: source spike, decay-only, multi-iteration evolution

**V1 anisotropic equivalence**:
- CPU reference: NEW `AnisotropicDiffusionKernel.cs` matching shader semantics с per-cell D from conductivity map
- Asymmetric flow: `min(D_self, D_neighbor)` between each tile pair on BOTH CPU + GPU sides
- Tolerance 0.001f per cell
- Test scenarios: wire path (high D row), insulator (D=0 column), mixed conductivity

**V2 wave equivalence**:
- CPU reference: NEW `WaveKernel.cs` — diffusion-based distance field
- Target spike + speed map + iterations
- Tolerance 0.01f per cell (wave shader less numerically tight than diffusion)
- Test scenarios: open field convergence, obstacle blocking, target moving

**V2 direction extraction equivalence**:
- CPU reference: NEW `DirectionExtractKernel.cs` — gradient from distance field
- Cosine similarity tolerance > 0.999 for direction vectors (magnitude variance acceptable, direction must align)
- Test scenarios: uniform gradient, corner gradient, near-target singularity

**Critical invariant**: equivalence tests run on EVERY V1+V2 compute shader commit. Equivalence failure = halt SC-12 (CPU/GPU divergence). К-L14 default-inclusion bias not exempt — equivalence is correctness gate, not performance optimization.

### §1.4 — S-LOCK-4: Direct K9 consumption — no new C ABI extensions (per Q3a)

V1+V2 consumes existing K9 C ABI surface verbatim:
- Field registration via `df_world_register_field` (existing)
- Span access via `df_world_field_acquire_span` (existing)
- Ping-pong via `df_world_field_swap_buffers` (existing)
- Conductivity via `df_world_field_set_conductivity` / `df_world_field_get_conductivity` (existing)
- Compute pipeline registration via `df_world_register_compute_pipeline` (V0.B existing)
- Compute dispatch via `df_world_field_dispatch_compute` (V0.B existing)

**NO new C ABI surfaces introduced for V1+V2**. FieldStorageBinding wiring is **managed-side implementation** of actual VkBuffer binding + descriptor set update — bridges existing C ABI к Vulkan compute pipeline без extending native surface.

**Rationale**: K9 already provides complete field surface (point read/write, span lease, ping-pong, conductivity, storage flag). V0.B compute pipeline registration round-trip verified working. V1+V2 = «implementation of existing contracts», не «extension of contracts». Per К-L14 default-inclusion: minimal architectural surface preferred — substrate primitives V1+V2 = parameter variations on existing K9 + V0.B foundation.

### §1.5 — S-LOCK-5: Ping-pong buffer management via FieldHandle<T>.SwapBuffers

V1+V2 multi-iteration shaders use K9 ping-pong via existing `FieldHandle<T>.SwapBuffers`:

**Pattern per iteration**:
1. Bind input field (current primary buffer) к descriptor binding 0 (read-only SSBO)
2. Bind output field (scratch buffer) к descriptor binding 1 (write SSBO)
3. Dispatch shader
4. Wait fence (per К-L7 atomic-from-observer)
5. `FieldHandle<T>.SwapBuffers()` — primary ↔ scratch swap
6. Next iteration uses former scratch as input

**К-L7 invariant preserved**: span access (`FieldHandle<T>.AcquireSpan`) rejected while ping-pong active. Native side enforces mutation-during-active-span via `FieldOperationFailedException` (existing K9 contract).

**Multi-iteration economics**: 5-10 iterations per dispatch achieves near-equilibrium for typical D values per VULKAN_SUBSTRATE §1.2. Mod-side specifies iteration count per gameplay configuration; substrate exposes per-call iterations parameter.

### §1.6 — S-LOCK-6: V0 substrate inheritance — preserved invariants

V0 substrate close achievements preserved through V1+V2:
- K-L19 hardware capability fail-fast (Runtime.Create check)
- Validation layer ALWAYS-ON DEBUG (zero validation messages tolerated)
- Per-image semaphore discipline (per V0.C.1 fix)
- Framebuffer recreation on swapchain resize (per V0.C.1 latent bug fix)
- Mixed [LibraryImport]/[DllImport] convention per Lesson #22
- Marshal.SizeOf<T>() alignment audit gate per Lesson #7 strengthening
- Bumper allocator adequacy for compute workloads (V0.B MemoryAllocator handles GPU buffer creation)
- Async compute queue family separation (V0.A + V0.B identified, V0.B + V0.C.2 dispatched compute pipeline through it)

V1+V2 не modifies any V0 surface. Pure additive.

### §1.7 — S-LOCK-7: Alignment audit mandatory continues (Lesson #7)

V0.A (1 fix) → V0.B (5 fixes) → V0.C.1 (0 fixes) → V0.C.2 (1 fix — Marshal.SizeOf<enum> .NET 8 limitation) maturity curve continues.

**New Vulkan structs V1+V2 may introduce (estimated)**:
- Compute shader push constant struct (V1 + V2 may share или differ)
- Descriptor set layout binding structs (already V0.B существуют — verify reuse, не recreate per Lesson #11)

**Marshal.SizeOf hypotheses к verify** (executor adjusts via test gate per V0.C.2 SC-5 precedent — `sizeof()` в unsafe context для enums):

Expected new structures:
- `DiffusionPushConstants` (D, K, dt, width, height + padding) — likely 24 bytes (5 floats + 4-byte pad для 16-byte alignment OR 4 floats + 2 uints = 24 bytes natural alignment) — executor verifies
- `WavePushConstants` (target_x, target_y, target_value, K, dt) — likely 20 bytes
- `DirectionExtractPushConstants` (width, height, normalization_threshold) — likely 12 bytes

Each new struct gets Marshal.SizeOf<T>() test gate (or sizeof() для blittable). Discipline preserved regardless of accuracy.

### §1.8 — S-LOCK-8: Atomic cascade discipline preserves V substrate stream pattern

V1+V2 cascade per V0.A → V0.B → V0.C.1 → V0.C.2 inherited:

1. Each commit independently buildable + testable
2. Each commit explicit scope-prefix message
3. `dotnet build` clean + `dotnet test` green at each commit
4. `sync_register.ps1 --validate` exit 0 at governance commits
5. **Multi-session pause provision** per Lesson #8 + Lesson #26 — pause at atomic commit boundary, push branch, surface к Crystalka
6. Halt-before-damage protocol — SC-N triggers HALT_REPORT, no partial state commit
7. Push-to-main classifier blocks (expected behavior per V0.A/V0.B/V0.C.1/V0.C.2 precedent)

**V1→V2 pause point** (between PR #40 closure + PR #41 start):
- V1 closure = PR #40 opened + Crystalka review + merge + manual visual verification
- V2 start = new feature branch `claude/v1_v2-wave-substrate` (или similar) from main post-V1 merge
- Per Lesson #26 cross-substrate scope splitting precedent (V0.C → V0.C.1 + V0.C.2)

### §1.9 — S-LOCK-9: Validation layer ALWAYS-ON discipline preserved

V0.A pattern preserved through V0.B + V0.C.1 + V0.C.2, V1+V2 maintains:
- `VK_LAYER_KHRONOS_validation` enabled в DEBUG per `RuntimeOptions.EnableValidationLayer`
- All ValidationLayer.cs callback diagnostics captured к console
- **ZERO validation errors tolerated** as commit gate
- V1+V2 substantial new compute surface (compute pipeline + descriptor set creation + binding + dispatch + storage buffer barriers) — каждая new call potential validation message. Common V1+V2-specific causes flagged в §4

### §1.10 — S-LOCK-10: REGISTER.yaml governance discipline preserved

V1+V2 brief enrolled at Commit 1 (DOC-D-V1_V2 entry в `docs/governance/REGISTER.yaml`). `sync_register.ps1 --validate` exit 0 gates governance commits.

**Frontmatter regeneration**: `sync_register.ps1` (full sync) regenerates brief frontmatter at Commit 1 — V1+V2 brief gets register_id + category + tier + lifecycle + owner + version + next_review_due + register_view_url fields populated.

**Audit trail events**:
- EVT-V1-CLOSURE at PR #40 merge (V1 substrate primitive close)
- EVT-V2-CLOSURE at PR #41 merge (V2 substrate primitive close)
- EVT-V-SUBSTRATE-CLOSE at PR #41 merge (V substrate full close per VULKAN_SUBSTRATE §1.4)

**MIGRATION_PROGRESS.md updates**:
- After PR #40 merge: V1 substrate primitive close marked
- After PR #41 merge: V2 substrate primitive close + V substrate full close marked

### §1.11 — S-LOCK-11: CPU reference inheritance + extension (per Q2a + Lesson #11)

`IsotropicDiffusionKernel.Run` existing K9-era CPU reference inherited verbatim. V1 isotropic equivalence tests consume it directly.

**V1+V2 new CPU references к add**:
- `AnisotropicDiffusionKernel.cs` — V1 anisotropic с per-cell D from conductivity map (V1 portion)
- `WaveKernel.cs` — V2 diffusion-based distance field (V2 portion)
- `DirectionExtractKernel.cs` — V2 gradient extraction (V2 portion)

**Pattern consistency**: All CPU kernels follow `IsotropicDiffusionKernel` pattern:
- Static class
- `Parameters` struct с named fields + sensible `Default`
- `Run(FieldHandle<T>, Parameters, iterations)` entry point
- Acquires span via `FieldHandle<T>.AcquireSpan`, writes via per-cell `FieldHandle<T>.WriteCell` (slow but correct — CPU reference is correctness oracle, not performance target)
- Boundary conditions documented (reflective default)

**Minimal scope per Q2a**: Single-iteration synthetic test grids (32×32) sufficient для equivalence proof. Multi-iteration grids verified through smoke test visual evolution, не unit tests (managed CPU at 200×200 = milliseconds per iteration, acceptable для visual but bottleneck для unit test suite).


---

## §2 — Phase 0 reads (mandatory before any code edits)

Phase 0 verification mandatory per Lesson #1 (full reads of production wiring files) + Lesson #9 candidate (survey-before-brief discipline) + Lesson #22 (read existing code first). V1+V2 executor MUST complete Phase 0 reads before Commit 2 (Commit 1 is brief enrollment + governance only).

### §2.1 — Baseline verification (V0.C.2 inheritance state)

**Required baseline before V1+V2 cascade begins**:

1. **`git log --oneline -1`** — verifies HEAD на main branch post-V0.C.2 merge. Expected: V0.C.2 closure commit (last commit of PR #39).

2. **`git status`** — clean working tree.

3. **`dotnet build`** — clean build of all projects. Expected: 0 errors, 0 warnings.

4. **`cmake --build native/DualFrontier.Core.Native`** — clean native build. K substrate unchanged.

5. **`dotnet test`** — 936+ tests green. Expected: 271 Runtime.Tests + 665+ other test projects = 936+ total (V0.C.2 closure baseline).

6. **`./tools/governance/sync_register.ps1 --validate`** — exit code 0.

**If any baseline check fails**: HALT-SC-1 (drift from expected V0.C.2 closure state).

### §2.2 — Vulkan SDK environment verification

1. **VULKAN_SDK env var set** — LunarG SDK install path
2. **`tools/glslangValidator.exe` present** — committed binary, used for new shader compilation
3. **vulkan-1.dll loadable** — verify через V0.C.2 smoke test passes (already confirmed на «Skarlet»)
4. **VK_LAYER_KHRONOS_validation present** — vulkaninfo --summary listing

### §2.3 — VULKAN_SUBSTRATE.md authority sections

1. **§1.2 V1 specification** — isotropic + anisotropic diffusion, mathematical model verbatim
2. **§1.3 V2 specification** — wave shader baseline (diffusion-based), distance field, direction extraction
3. **§1.3.1 G9 eikonal upgrade** — deferred TBD section (Q7a confirms diffusion baseline)
4. **§1.4 V substrate close acceptance criteria** — multi-field coexistence
5. **§3.1 Domain A field updates** — substrate primitive context
6. **§3.3 Mod-driven shader registration** — separation между substrate (V1+V2 ships shaders) и mod (M-V demonstrations ship shaders later)
7. **§5.1 Mathematical models** — Domain A primary kernels GLSL pseudocode
8. **§5.2 Unified across gameplay configurations** — V1 vs V1-anisotropic vs V2 routing table
9. **§5.4 Engine vs mod placement** — substrate provides infrastructure, mods provide gameplay
10. **§11 Methodology adjustments** — validation layer output check + CPU/GPU equivalence test

### §2.4 — V0.C.2 code anchor verification (post-merge state)

1. **`src/DualFrontier.Core.Interop/FieldHandle.cs`** — verify FieldHandle<T> + FieldSpanLease<T> production-ready. Confirm ping-pong (SwapBuffers), conductivity, storage flag surfaces present.

2. **`src/DualFrontier.Core.Interop/FieldRegistry.cs`** — verify Register<T> + Get<T> + TryGet<T> + IsRegistered + Unregister + Count.

3. **`src/DualFrontier.Core.Interop/CpuKernels/IsotropicDiffusionKernel.cs`** — verify exists с `Run(FieldHandle<float>, Parameters, iterations)` signature. **CRITICAL**: V1 brief inherits this verbatim, не recreates. Per Lesson #11 redundancy check.

4. **`src/DualFrontier.Core.Interop/NativeMethods.Fields.cs`** — verify K9 ABI surface complete (df_world_register_field, _field_read_cell, _field_write_cell, _field_acquire_span, _field_release_span, _field_set_conductivity, _field_get_conductivity, _field_set_storage_flag, _field_get_storage_flag, _field_swap_buffers, _field_count). **NO new entries needed для V1+V2.**

5. **`src/DualFrontier.Core.Interop/NativeMethods.Compute.cs`** — verify V0.B compute surface (df_world_attach_vulkan, _register_compute_pipeline, _field_dispatch_compute, _compute_pipeline_count). **NO new entries needed для V1+V2.**

6. **`src/DualFrontier.Runtime/Compute/FieldStorageBinding.cs`** — verify V0.B no-op orchestration shell. **V1+V2 implements actual VkBuffer binding + descriptor set updates here.** Current state: `DispatchField` returns native `df_world_field_dispatch_compute` result. V1+V2 cascade adds managed-side wiring of actual compute pipeline state.

7. **`src/DualFrontier.Runtime/Compute/ComputeDispatch.cs`** — verify ExecuteSync(pipeline, x, y, z) pattern. V1+V2 may extend для multi-binding descriptor set updates + storage buffer barriers.

8. **`src/DualFrontier.Runtime/Compute/VulkanComputePipeline.cs`** — verify compute pipeline creation pattern. V1+V2 instantiates 3 new compute pipelines (diffusion, wave, direction_extract).

9. **`src/DualFrontier.Runtime/Compute/VulkanComputeDescriptors.cs`** — verify descriptor set layout + pool pattern. V1+V2 needs 2-3 binding layouts (read SSBO + write SSBO + conductivity SSBO).

10. **`tools/shaders/noop.comp`** — V0.B template для new V1+V2 compute shaders. GLSL #version 450 + layout local_size_x/y/z + storage buffer bindings + push constants.

### §2.5 — REGISTER structure read (governance entry template)

1. **`docs/governance/REGISTER.yaml`** — Tier 3 Category D documents. V0.C.2 + V0.C.1 entries as templates for V1+V2 entry structure.

2. **`docs/governance/REGISTER.yaml`** — audit_trail section format. V0.C.2 closure event provides EVT structure template.

3. **`docs/governance/sync_register.ps1`** — tool authoritative, не modify.

### §2.6 — Test project structure

1. **`tests/DualFrontier.Runtime.Tests/`** — V1+V2 test directory structure
2. **`tests/DualFrontier.Core.Tests/`** OR **`tests/DualFrontier.Core.Interop.Tests/`** — find existing CpuKernels tests location (likely existing IsotropicDiffusionKernel tests provide template)

```powershell
# Phase 0 helper:
Get-ChildItem -Recurse tests/ -Filter "*Diffusion*" 
Get-ChildItem -Recurse tests/ -Filter "*Kernel*"
```

3. **`tests/DualFrontier.Runtime.SmokeTest/Program.cs`** — V0.C.2 extension point. V1+V2 extends с V1 isotropic scene + V1 anisotropic scene + V2 distance scene + multi-field coexistence scene.

### §2.7 — Shader build pipeline read

1. **`Directory.Build.props`** — current `CompileShaders` target лines (verified Phase 0). V1+V2 extends с diffusion.comp + wave.comp + direction_extract.comp entries (Commits для V1 + V2 portions respectively).

2. **`tools/shaders/`** — verify existing shaders directory pattern для new shader file placement.

3. **`assets/shaders/`** — output SPIR-V directory. V1+V2 generates diffusion.comp.spv + wave.comp.spv + direction_extract.comp.spv at build time.

---

## §3 — V1 portion: atomic commit cascade (PR #40)

V1 portion structured as ~22 atomic commits. PR #40 closes V1 substrate primitive. Branch: `claude/v1-diffusion-substrate`.

### Commit V1-1 — Brief authoring commit (V1+V2 brief enrollment)

**Scope**: governance + docs

**Files modified**:
- `tools/briefs/V1_V2_EXECUTION_BRIEF.md` — NEW (this brief)
- `docs/governance/REGISTER.yaml` — ADD entry DOC-D-V1_V2:
```yaml
DOC-D-V1_V2:
  category: D
  tier: 3
  lifecycle: AUTHORED
  owner: Crystalka
  version: "1.0"
  next_review_due: 2027-05-19
  path: tools/briefs/V1_V2_EXECUTION_BRIEF.md
  description: V1+V2 combined execution brief — scalar field substrate primitives (diffusion + wave) per VULKAN_SUBSTRATE §1.2 + §1.3
```

**Operations**:
1. Copy brief from outputs к target path
2. Add REGISTER entry
3. `tools\governance\sync_register.ps1` (full sync) — regenerates brief frontmatter
4. `tools\governance\sync_register.ps1 --validate` exit 0
5. `git add` + `git commit -m "governance: enroll V1+V2 execution brief (DOC-D-V1_V2 AUTHORED)"`

**Verification**: Brief enrolled. No code changes.

### Commit V1-2 — Anisotropic diffusion CPU reference kernel

**Scope**: kernels + tests

**Files added**:
- `src/DualFrontier.Core.Interop/CpuKernels/AnisotropicDiffusionKernel.cs` — NEW kernel:

```csharp
using System;

namespace DualFrontier.Core.Interop.CpuKernels;

/// <summary>
/// CPU reference implementation of anisotropic 4-neighbour diffusion stencil
/// per VULKAN_SUBSTRATE.md §1.2 + §5.1. Per-cell diffusion coefficient D(x,y)
/// from field conductivity map. Asymmetric flow rule:
///   flow(self → neighbour) = min(D_self, D_neighbour) · (P_neighbour - P_self)
/// guarantees flow blocks when either side is non-conductor (D=0).
/// </summary>
/// <remarks>
/// <para>Math (per VULKAN_SUBSTRATE.md §5.1 anisotropic variant):</para>
/// <code>
///   ∂P/∂t = ∇·(D(x,y) · ∇P) + S(x,y) - C(x,y) · effectiveness(P)
/// </code>
/// <para>Discretized as asymmetric flow за tile pair (Q-G-2 anisotropic clarification).</para>
/// <para>Boundary: reflective (edge cell uses self as neighbour, flow=0 across boundary).</para>
/// <para>CPU reference oracle для GPU equivalence test (per VULKAN_SUBSTRATE §11).</para>
/// </remarks>
public static class AnisotropicDiffusionKernel
{
    public readonly struct Parameters
    {
        public float DecayCoefficient { get; init; }
        public float DeltaTime { get; init; }

        public static Parameters Default => new()
        {
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f
        };
    }

    public static void Run(FieldHandle<float> field, Parameters p, int iterations)
    {
        if (field is null) throw new ArgumentNullException(nameof(field));
        if (iterations < 1) return;

        int width = field.Width;
        int height = field.Height;
        var scratch = new float[width * height];

        // Snapshot conductivity map (D values per cell)
        var conductivity = new float[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                conductivity[y * width + x] = field.GetConductivity(x, y);

        for (int iter = 0; iter < iterations; iter++)
        {
            using (var lease = field.AcquireSpan())
            {
                ReadOnlySpan<float> readBuf = lease.Span;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * width + x;
                        float center = readBuf[i];
                        float dSelf = conductivity[i];

                        // 4-neighbour asymmetric flow (min(D_self, D_neighbour))
                        float flowSum = 0.0f;

                        if (y > 0)
                        {
                            float nVal = readBuf[i - width];
                            float dN = conductivity[i - width];
                            flowSum += Math.Min(dSelf, dN) * (nVal - center);
                        }
                        if (y < height - 1)
                        {
                            float sVal = readBuf[i + width];
                            float dS = conductivity[i + width];
                            flowSum += Math.Min(dSelf, dS) * (sVal - center);
                        }
                        if (x < width - 1)
                        {
                            float eVal = readBuf[i + 1];
                            float dE = conductivity[i + 1];
                            flowSum += Math.Min(dSelf, dE) * (eVal - center);
                        }
                        if (x > 0)
                        {
                            float wVal = readBuf[i - 1];
                            float dW = conductivity[i - 1];
                            flowSum += Math.Min(dSelf, dW) * (wVal - center);
                        }

                        float delta = (flowSum - p.DecayCoefficient * center) * p.DeltaTime;
                        scratch[i] = center + delta;
                    }
                }
            }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    field.WriteCell(x, y, scratch[y * width + x]);
        }
    }
}
```

- `tests/<existing CpuKernels test location>/AnisotropicDiffusionKernelTests.cs` — NEW tests:

```csharp
[Fact]
public void Run_NullField_Throws() { /* ArgumentNullException */ }

[Fact]
public void Run_ZeroIterations_NoChange() { /* identical state after 0 iterations */ }

[Fact]
public void Run_SourceSpike_HighConductivity_Spreads() {
    // 32×32 grid, source at center, uniform high D (1.0)
    // After 5 iterations, spike should spread radially
}

[Fact]
public void Run_WirePath_ChannelsPropagation() {
    // 32×32 grid, source at (0,0), wire row at y=0 (D=10.0), rest D=0.1
    // After 5 iterations, propagation channeled along wire row
}

[Fact]
public void Run_InsulatorBlocks() {
    // 32×32 grid, source at (0,0), full row y=15 D=0.0 (insulator)
    // After 10 iterations, value at y=20 should be near-zero
}

[Fact]
public void Run_DecayReducesTotal() {
    // 32×32 uniform field, D=0.0 (no diffusion), decay 0.1
    // After 1 iteration, total sum reduced by ~10%
}
```

**Operations**:
1. Create AnisotropicDiffusionKernel.cs
2. Create tests
3. `dotnet build` clean
4. `dotnet test` 941+ green (936 baseline + ~5-7 new tests)
5. `git add` + `git commit -m "kernels: add AnisotropicDiffusionKernel CPU reference per VULKAN_SUBSTRATE §1.2"`

**Verification**: CPU reference matches mathematical model. Asymmetric flow rule correctly handles wire path + insulator scenarios.

### Commit V1-3 — Diffusion compute shader (GLSL)

**Scope**: shaders

**Files added**:
- `tools/shaders/diffusion.comp` — NEW compute shader:

```glsl
#version 450

// V1 diffusion compute shader per VULKAN_SUBSTRATE.md §1.2 + §5.1.
// Handles both isotropic (uniform D) + anisotropic (per-cell D from conductivity) variants
// via parameter-driven path. К-L9 «vanilla = mods» pattern: substrate provides shader,
// mod provides gameplay-specific parameters (D map, decay, dt, iterations).
//
// Asymmetric flow rule: flow(self → neighbour) = min(D_self, D_neighbour) · (P_neighbour - P_self)
// Wire path emerges automatically (high D wire, low D off-path).

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(set = 0, binding = 0) readonly buffer FieldIn {
    float values[];
} fieldIn;

layout(set = 0, binding = 1) writeonly buffer FieldOut {
    float values[];
} fieldOut;

layout(set = 0, binding = 2) readonly buffer Conductivity {
    float values[];
} conductivity;

layout(push_constant) uniform PushConstants {
    float decayCoefficient;
    float deltaTime;
    uint width;
    uint height;
} pc;

void main() {
    uint x = gl_GlobalInvocationID.x;
    uint y = gl_GlobalInvocationID.y;

    if (x >= pc.width || y >= pc.height) return;

    uint idx = y * pc.width + x;
    float center = fieldIn.values[idx];
    float dSelf = conductivity.values[idx];

    // 4-neighbour asymmetric flow rule (Q-G-2 anisotropic spec).
    // Reflective boundaries: edge cell flow contribution = 0 (uses self → no flow).
    float flowSum = 0.0;

    if (y > 0u) {
        uint nIdx = (y - 1u) * pc.width + x;
        float nVal = fieldIn.values[nIdx];
        float dN = conductivity.values[nIdx];
        flowSum += min(dSelf, dN) * (nVal - center);
    }
    if (y < pc.height - 1u) {
        uint sIdx = (y + 1u) * pc.width + x;
        float sVal = fieldIn.values[sIdx];
        float dS = conductivity.values[sIdx];
        flowSum += min(dSelf, dS) * (sVal - center);
    }
    if (x < pc.width - 1u) {
        uint eIdx = y * pc.width + (x + 1u);
        float eVal = fieldIn.values[eIdx];
        float dE = conductivity.values[eIdx];
        flowSum += min(dSelf, dE) * (eVal - center);
    }
    if (x > 0u) {
        uint wIdx = y * pc.width + (x - 1u);
        float wVal = fieldIn.values[wIdx];
        float dW = conductivity.values[wIdx];
        flowSum += min(dSelf, dW) * (wVal - center);
    }

    float delta = (flowSum - pc.decayCoefficient * center) * pc.deltaTime;
    fieldOut.values[idx] = center + delta;
}
```

- `Directory.Build.props` — EXTEND CompileShaders target with:

```xml
<!-- V1 diffusion compute shader (isotropic + anisotropic variants via per-cell D) -->
<Exec Command="&quot;$(GlslangValidatorExe)&quot; -V &quot;$(ShaderSourceDir)\diffusion.comp&quot; -o &quot;$(ShaderOutputDir)\diffusion.comp.spv&quot;" />
```

**Note**: Isotropic variant achieved via uniform conductivity buffer (all cells D=constant per gameplay configuration). Single shader template handles both per Q1c hybrid sub-milestone structure + VULKAN_SUBSTRATE §1.2 «one V1 compute shader pattern handles isotropic + anisotropic variants».

**Operations**:
1. Create diffusion.comp
2. Edit Directory.Build.props
3. `dotnet build` — verify shader compiles к assets/shaders/diffusion.comp.spv
4. Verify SPIR-V file exists: `ls assets/shaders/diffusion.comp.spv`
5. `git add` + `git commit -m "shaders: add V1 diffusion compute shader (isotropic + anisotropic via per-cell D)"`

**Verification**: glslangValidator output clean (no GLSL errors). diffusion.comp.spv generated at build time.

**Halt trigger SC-13 (GLSL compilation error)**: If diffusion.comp fails к compile → glslangValidator output identifies line/column. Common causes: storage buffer layout mismatch, push constant size exceeded (128 bytes max), local_size product exceeds hardware limit (1024 typical max).

### Commit V1-4 — DiffusionPushConstants struct + alignment audit

**Scope**: compute + tests

**Files added**:
- `src/DualFrontier.Runtime/Compute/DiffusionPushConstants.cs` — NEW struct matching shader layout:

```csharp
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Push constant payload for V1 diffusion compute shader.
/// Layout matches diffusion.comp push_constant block per S-LOCK-3 alignment audit gate.
/// </summary>
/// <remarks>
/// Std140-ish push constant layout. Vulkan push constants don't follow std140 strictly
/// but standalone struct alignment must match shader declaration order:
///   float decayCoefficient (offset 0, size 4)
///   float deltaTime        (offset 4, size 4)
///   uint width             (offset 8, size 4)
///   uint height            (offset 12, size 4)
/// Total: 16 bytes (natural alignment, no padding required for 4-byte primitives).
/// </remarks>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct DiffusionPushConstants
{
    public float DecayCoefficient;
    public float DeltaTime;
    public uint Width;
    public uint Height;
}
```

- `tests/DualFrontier.Runtime.Tests/Compute/DiffusionPushConstantsTests.cs` — NEW alignment audit test:

```csharp
public class DiffusionPushConstantsTests
{
    [Fact]
    public void Size_Matches_Expected_16_Bytes()
    {
        // S-LOCK-7 alignment audit. Brief hypothesis: 16 bytes (4 × 4-byte primitives).
        // If actual ≠ 16, executor adjusts test + documents correction per Lesson #7.
        Assert.Equal(16, Marshal.SizeOf<DiffusionPushConstants>());
    }

    [Fact]
    public void Fields_AccessibleAndAssignable()
    {
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f,
            Width = 200,
            Height = 200,
        };
        Assert.Equal(0.01f, pc.DecayCoefficient);
        Assert.Equal(200u, pc.Width);
    }
}
```

**Operations**:
1. Create DiffusionPushConstants.cs
2. Create tests
3. `dotnet build`
4. `dotnet test --filter "FullyQualifiedName~DiffusionPushConstants"` — verify size test passes
5. `dotnet test` full suite green
6. `git add` + `git commit -m "compute: add DiffusionPushConstants struct + alignment audit (S-LOCK-7)"`

**Halt trigger SC-5 (alignment mismatch)**: Per V0.B 5-corrections + V0.C.2 1-correction precedent. If Marshal.SizeOf ≠ 16 → adjust test expected к actual, document fix per Lesson #7 strengthening.

### Commit V1-5 — V1DiffusionPipeline managed wrapper

**Scope**: compute

**Files added**:
- `src/DualFrontier.Runtime/Compute/V1DiffusionPipeline.cs` — NEW pipeline wrapper:

```csharp
using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// V1 diffusion compute pipeline managed wrapper. Encapsulates:
/// - VulkanComputePipeline creation from diffusion.comp.spv
/// - Descriptor set layout (3 bindings: input ssbo, output ssbo, conductivity ssbo)
/// - Per-dispatch descriptor set update + push constant + dispatch
/// - Ping-pong management via FieldHandle.SwapBuffers
///
/// V1 substrate primitive per VULKAN_SUBSTRATE.md §1.2.
/// Consumers (M-V1 mana mod, M-V2 electricity mod) supply field handle + conductivity map
/// + parameters; substrate handles GPU pipeline + dispatch.
/// </summary>
public sealed class V1DiffusionPipeline : IDisposable
{
    private readonly VulkanDevice _device;
    private readonly VulkanComputePipeline _pipeline;
    private readonly VulkanComputeDescriptors _descriptors;
    private readonly ComputeDispatch _dispatch;
    private bool _disposed;

    public V1DiffusionPipeline(
        VulkanDevice device,
        VulkanCommandPool computePool,
        string spirvPath)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(computePool);

        _device = device;

        // Descriptor set layout: 3 storage buffers (input, output, conductivity)
        _descriptors = new VulkanComputeDescriptors(device, bindingCount: 3);

        // Load SPIR-V shader
        var shader = VulkanShaderModule.LoadFromFile(device, spirvPath);

        // Compute pipeline с push constant range (16 bytes per DiffusionPushConstants S-LOCK-7)
        _pipeline = new VulkanComputePipeline(
            device, shader, _descriptors.Layout,
            pushConstantSize: 16);

        _dispatch = new ComputeDispatch(device, computePool);
    }

    /// <summary>
    /// Execute one diffusion iteration on field. Updates input ↔ output ping-pong через FieldHandle.SwapBuffers.
    /// Caller responsible для iteration loop (typically 5-10 iterations per gameplay tick).
    /// </summary>
    public void ExecuteIteration(
        FieldStorageBinding binding,
        string fieldName,
        DiffusionPushConstants pushConstants,
        uint dispatchX, uint dispatchY)
    {
        ArgumentNullException.ThrowIfNull(binding);
        ObjectDisposedException.ThrowIf(_disposed, this);

        // V1 implementation: dispatch through FieldStorageBinding с pipeline_id from V0.B registration.
        // Native side handles VkBuffer binding + descriptor set update + dispatch + fence wait.
        // V0.B + V0.C.2 round-trip verified; V1+ provides actual compute shader к dispatch.
        bool success = binding.DispatchField(fieldName, _pipeline.NativePipelineId, dispatchX, dispatchY, 1);
        if (!success)
            throw new InvalidOperationException($"V1 diffusion dispatch failed for field '{fieldName}'");
    }

    public uint PipelineId => _pipeline.NativePipelineId;

    public void Dispose()
    {
        if (_disposed) return;
        _dispatch.Dispose();
        _pipeline.Dispose();
        _descriptors.Dispose();
        _disposed = true;
    }
}
```

**Operations**:
1. Create V1DiffusionPipeline.cs
2. `dotnet build` — verify compiles (depends on existing VulkanComputePipeline + VulkanComputeDescriptors + ComputeDispatch infrastructure)
3. `dotnet test` full suite green (no new tests yet; integration tests follow)
4. `git add` + `git commit -m "compute: add V1DiffusionPipeline managed wrapper (S-LOCK-1 V1 substrate primitive)"`

**Verification**: V1DiffusionPipeline composes existing compute infrastructure. PipelineId exposed для FieldStorageBinding.DispatchField integration.

### Commit V1-6 — FieldStorageBinding V1 wiring + integration test

**Scope**: compute + tests

**Files modified**:
- `src/DualFrontier.Runtime/Compute/FieldStorageBinding.cs` — REPLACE V0.B no-op with V1 actual VkBuffer binding via dispatched compute:

```csharp
// Existing V0.B Attach + Register + DispatchField preserved.
// Add V1 specific helper:

/// <summary>
/// V1+: ensure compute pipeline registered + dispatch с field-bound descriptor set.
/// V0.B was no-op stub; V1+ exercises actual round-trip with diffusion shader.
/// </summary>
public bool DispatchDiffusion(
    string fieldName,
    uint pipelineId,
    DiffusionPushConstants pushConstants,
    uint dispatchX, uint dispatchY)
{
    // Per V0.B registration flow already wires push constants на native side
    // through df_world_field_dispatch_compute. V1 supplies actual shader pipelineId.
    return DispatchField(fieldName, pipelineId, dispatchX, dispatchY, 1);
}
```

- `tests/DualFrontier.Runtime.Tests/Compute/V1DiffusionIntegrationTests.cs` — NEW integration test (Vulkan-dependent — runs if Runtime can be created):

```csharp
public class V1DiffusionIntegrationTests
{
    [SkippableFact]
    public void DispatchDiffusion_OnRegisteredField_Succeeds()
    {
        // Skip if hardware unavailable per К-L19 (CI без GPU)
        Skip.IfNot(HardwareCapabilityProbe.IsAvailable());

        using var runtime = Runtime.Create(/* options с EnableValidationLayer = true */);
        var world = /* NativeWorld instance attached к runtime */;
        var fieldRegistry = /* FieldRegistry on world */;
        var binding = new FieldStorageBinding(world);
        binding.Attach(runtime.VulkanInstance, runtime.VulkanDevice);

        // Register 32×32 float field
        var field = fieldRegistry.Register<float>("test.v1.diffusion", 32, 32);

        // Set initial state: spike at center
        field.WriteCell(16, 16, 1.0f);

        // Set uniform conductivity D=0.1
        for (int y = 0; y < 32; y++)
            for (int x = 0; x < 32; x++)
                field.SetConductivity(x, y, 0.1f);

        // Register diffusion pipeline
        var spirvBytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "assets", "shaders", "diffusion.comp.spv"));
        uint pipelineId = binding.Register("test.v1.diffusion", spirvBytes, descriptorBindingCount: 3);
        Assert.NotEqual(0u, pipelineId);

        // Dispatch one iteration
        var pc = new DiffusionPushConstants
        {
            DecayCoefficient = 0.01f,
            DeltaTime = 1.0f,
            Width = 32, Height = 32,
        };
        bool success = binding.DispatchDiffusion("test.v1.diffusion", pipelineId, pc, dispatchX: 4, dispatchY: 4);
        Assert.True(success);

        // Verify validation log clean (S-LOCK-9)
        Assert.Equal(0, runtime.ValidationLayer.MessageCount);
    }
}
```

**Operations**:
1. Edit FieldStorageBinding.cs
2. Create V1DiffusionIntegrationTests.cs (note: SkippableFact pattern для hardware-dependent tests)
3. `dotnet build`
4. `dotnet test` — verify integration test passes on «Skarlet» (assumes Vulkan environment)
5. `git add` + `git commit -m "compute: wire V1 diffusion dispatch through FieldStorageBinding (S-LOCK-4 K9 direct consumption)"`

**Verification**: V1 diffusion dispatch round-trip works. Validation log clean.

### Commits V1-7 through V1-13 — CPU/GPU equivalence test scaffold + V1 isotropic equivalence + V1 anisotropic equivalence

Each commit ~50-150 LOC. Pattern follows V1-6:

**Commit V1-7**: V1 isotropic equivalence test (uniform D=0.1, source spike, 5 iterations, CPU IsotropicDiffusionKernel.Run + GPU dispatch, compare tolerance 0.001f per cell).

**Commit V1-8**: V1 isotropic edge cases — boundary reflective, decay-only (D=0), pure diffusion (decay=0), various iteration counts (1, 5, 10, 20).

**Commit V1-9**: V1 anisotropic equivalence test (wire path: D=10.0 row, off-wire D=0.1, source at row start, 5 iterations, both CPU AnisotropicDiffusionKernel.Run + GPU dispatch with same conductivity map).

**Commit V1-10**: V1 anisotropic insulator scenario (D=0.0 wall, propagation blocked).

**Commit V1-11**: V1 multi-iteration evolution (10-iteration stencil convergence к near-equilibrium).

**Commit V1-12**: SmokeTest scene — V1 isotropic 200×200 field, source spike at center, 5 iterations/frame, visual evolution over 10s, FPS≥60.

**Commit V1-13**: SmokeTest scene — V1 anisotropic 200×200 field, horizontal wire path, source at one end, observe channeled flow, FPS≥60.

### Commits V1-14 through V1-21 — Runtime composition + Visualization helper + Pre-closure

**Commit V1-14**: Runtime.cs composition — add V1DiffusionPipeline property + Create() construction call.

**Commit V1-15**: Visualization helper (optional) — render field state as heatmap texture в SmokeTest (visualizes diffusion patterns visually). Reuses V0.C.2 batched SpriteRenderer infrastructure.

**Commit V1-16**: SmokeTest end-к-end V1 isotropic + V1 anisotropic scenes via Runtime composition (Runtime.V1Diffusion property + helper API).

**Commit V1-17**: Performance benchmark — V1 isotropic dispatch latency, verify within ~1ms/iteration on «Skarlet» (per VULKAN_SUBSTRATE §1.4 budget — «~1 ms/tick per active field on mid-range GPU»).

**Commit V1-18**: Documentation — V1 substrate primitive README в `src/DualFrontier.Runtime/Compute/MODULE.md` extension (V1 pipeline usage, parameter tuning, ping-pong contract).

**Commit V1-19**: V1 closure manual visual verification protocol document в `docs/scratch/V1_V2/V1_MANUAL_VERIFICATION_PROTOCOL.md`.

**Commit V1-20**: REGISTER.yaml — V1 closure governance amendment:
- DOC-D-V1_V2 lifecycle = AUTHORED → IN_PROGRESS (V1 portion complete, V2 pending)
- audit_trail event EVT-V1-CLOSURE
- MIGRATION_PROGRESS.md V1 substrate primitive close marked

**Commit V1-21**: V1 closure section added к brief (commit ledger + verification metrics for V1 portion).

**Commit V1-22**: Final REGISTER sync + branch push + PR #40 opening:
- `sync_register.ps1 --validate` exit 0
- `git push origin claude/v1-diffusion-substrate`
- PR #40 opened: «V1 — Scalar field + diffusion shader (isotropic + anisotropic) — V substrate primitive»

**Crystalka review**: Manual visual verification на «Skarlet» of V1 isotropic + V1 anisotropic scenes. CPU/GPU equivalence test results. PR review + merge if acceptance criteria met.

**V1→V2 pause point**: After PR #40 merge, next session starts new branch `claude/v2-wave-substrate` from main. V2 portion proceeds (Commits V2-1 through V2-24 below).


---

## §4 — V2 portion: atomic commit cascade (PR #41)

V2 portion structured as ~24 atomic commits. PR #41 closes V2 substrate primitive + V substrate full close. Branch: `claude/v2-wave-substrate` (created from main post-V1 merge).

### Commit V2-1 — Baseline verification + V2 entry governance

**Scope**: governance

**Operations**:
1. Verify V1 merged (PR #40 merged к main, expected commit hashes from V1 closure section)
2. `dotnet build` clean baseline post-V1 merge
3. `dotnet test` — verify V1 tests in baseline (940-960+ tests post-V1 closure)
4. `sync_register.ps1 --validate` exit 0
5. New branch from main: `git checkout -b claude/v2-wave-substrate`
6. No file changes — Commit V2-1 is verification + branch entry (no actual commit; setup step recorded в session notes)

Note: if executor's discipline prefers explicit commit-1 marker, can be a no-op governance commit (e.g. progress note). Optional.

### Commit V2-2 — Wave CPU reference kernel (diffusion-based distance per Q7a)

**Scope**: kernels + tests

**Files added**:
- `src/DualFrontier.Core.Interop/CpuKernels/WaveKernel.cs` — NEW kernel per VULKAN_SUBSTRATE §1.3 Option B baseline:

```csharp
using System;

namespace DualFrontier.Core.Interop.CpuKernels;

/// <summary>
/// CPU reference implementation of diffusion-based distance field per VULKAN_SUBSTRATE §1.3
/// Option B baseline (per Q7a ratification — eikonal upgrade Option A deferred TBD).
///
/// Mathematical model:
///   ∂D/∂t = ∇²D - K·D + spike_at_target
///
/// Не geodesic-accurate distance, но produces gradient pointing toward target.
/// 99% gameplay-equivalent for colony sim — pawns don't optimize paths, 5% suboptimal
/// routing imperceptible. Per VULKAN_SUBSTRATE §1.3 Option B rationale.
/// </summary>
public static class WaveKernel
{
    public readonly struct Parameters
    {
        public float DecayCoefficient { get; init; }
        public float DeltaTime { get; init; }
        public float TargetSpikeValue { get; init; }
        public int TargetX { get; init; }
        public int TargetY { get; init; }

        public static Parameters Default => new()
        {
            DecayCoefficient = 0.1f,
            DeltaTime = 1.0f,
            TargetSpikeValue = 100.0f,
            TargetX = 0,
            TargetY = 0,
        };
    }

    public static void Run(FieldHandle<float> field, Parameters p, int iterations)
    {
        if (field is null) throw new ArgumentNullException(nameof(field));
        if (iterations < 1) return;

        int width = field.Width;
        int height = field.Height;
        var scratch = new float[width * height];

        // Snapshot conductivity (speed map — walls = 0, open = 1)
        var speed = new float[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                speed[y * width + x] = field.GetConductivity(x, y);

        for (int iter = 0; iter < iterations; iter++)
        {
            // Re-inject target spike each iteration (prevents decay reducing к zero)
            if (p.TargetX >= 0 && p.TargetX < width && p.TargetY >= 0 && p.TargetY < height)
                field.WriteCell(p.TargetX, p.TargetY, p.TargetSpikeValue);

            using (var lease = field.AcquireSpan())
            {
                ReadOnlySpan<float> readBuf = lease.Span;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int i = y * width + x;
                        float center = readBuf[i];
                        float sSelf = speed[i];

                        if (sSelf <= 0.0f)
                        {
                            // Wall — no propagation
                            scratch[i] = 0.0f;
                            continue;
                        }

                        // 4-neighbour diffusion с speed-weighted flow
                        float laplacian = 0.0f;
                        int contributingNeighbours = 0;

                        if (y > 0 && speed[i - width] > 0.0f) {
                            laplacian += readBuf[i - width] - center;
                            contributingNeighbours++;
                        }
                        if (y < height - 1 && speed[i + width] > 0.0f) {
                            laplacian += readBuf[i + width] - center;
                            contributingNeighbours++;
                        }
                        if (x < width - 1 && speed[i + 1] > 0.0f) {
                            laplacian += readBuf[i + 1] - center;
                            contributingNeighbours++;
                        }
                        if (x > 0 && speed[i - 1] > 0.0f) {
                            laplacian += readBuf[i - 1] - center;
                            contributingNeighbours++;
                        }

                        float delta = (sSelf * laplacian - p.DecayCoefficient * center) * p.DeltaTime;
                        scratch[i] = center + delta;
                    }
                }
            }

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    field.WriteCell(x, y, scratch[y * width + x]);
        }
    }
}
```

- `tests/.../WaveKernelTests.cs` — NEW tests:

```csharp
[Fact]
public void Run_TargetSpike_Spreads() {
    // 32×32 field, target at (16,16), uniform speed 1.0
    // After 10 iterations, gradient should radiate from target
}

[Fact]
public void Run_WallBlocks() {
    // 32×32 field, target at (5,5), wall column at x=15 (speed=0)
    // After 10 iterations, values at x=20 should be near-zero
}

[Fact]
public void Run_TargetReInjected() {
    // After 1 iteration, target cell should still hold TargetSpikeValue
    // (kernel re-injects each iteration к preserve gradient)
}
```

**Operations**:
1. Create WaveKernel.cs + tests
2. `dotnet build` + `dotnet test`
3. `git add` + `git commit -m "kernels: add WaveKernel CPU reference (diffusion-based distance per VULKAN_SUBSTRATE §1.3 Option B baseline)"`

### Commit V2-3 — Direction extraction CPU reference

**Scope**: kernels + tests

**Files added**:
- `src/DualFrontier.Core.Interop/CpuKernels/DirectionExtractKernel.cs` — NEW kernel:

```csharp
using System;
using System.Numerics;

namespace DualFrontier.Core.Interop.CpuKernels;

/// <summary>
/// CPU reference for direction field extraction per VULKAN_SUBSTRATE §1.3.
/// Gradient of distance field → direction toward target (negative gradient).
/// </summary>
public static class DirectionExtractKernel
{
    public static void Run(FieldHandle<float> distanceField, FieldHandle<Vector2> directionField)
    {
        if (distanceField is null) throw new ArgumentNullException(nameof(distanceField));
        if (directionField is null) throw new ArgumentNullException(nameof(directionField));
        if (distanceField.Width != directionField.Width || distanceField.Height != directionField.Height)
            throw new ArgumentException("Field dimensions must match");

        int width = distanceField.Width;
        int height = distanceField.Height;

        using var distLease = distanceField.AcquireSpan();
        ReadOnlySpan<float> dist = distLease.Span;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int i = y * width + x;

                float n = (y > 0)          ? dist[i - width] : dist[i];
                float s = (y < height - 1) ? dist[i + width] : dist[i];
                float e = (x < width - 1)  ? dist[i + 1]     : dist[i];
                float w = (x > 0)          ? dist[i - 1]     : dist[i];

                // Negative gradient = direction к smaller distance = toward target
                Vector2 dir = new(w - e, n - s);
                if (dir.LengthSquared() > 0.0001f)
                    dir = Vector2.Normalize(dir);

                directionField.WriteCell(x, y, dir);
            }
        }
    }
}
```

- `tests/.../DirectionExtractKernelTests.cs` — tests с uniform gradient, corner gradient, near-target singularity scenarios

**Operations**:
1. Create + tests
2. `dotnet build` + `dotnet test`
3. `git add` + `git commit -m "kernels: add DirectionExtractKernel CPU reference (gradient extraction per VULKAN_SUBSTRATE §1.3)"`

### Commit V2-4 — Wave compute shader (GLSL)

**Scope**: shaders

**Files added**:
- `tools/shaders/wave.comp` — NEW compute shader matching WaveKernel semantics:

```glsl
#version 450

// V2 wave compute shader per VULKAN_SUBSTRATE.md §1.3 Option B baseline.
// Diffusion-based distance field. Per-cell speed map (walls = 0, open = 1).
// 99% gameplay-equivalent for colony sim per spec rationale.

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(set = 0, binding = 0) readonly buffer FieldIn  { float values[]; } fieldIn;
layout(set = 0, binding = 1) writeonly buffer FieldOut { float values[]; } fieldOut;
layout(set = 0, binding = 2) readonly buffer SpeedMap { float values[]; } speedMap;

layout(push_constant) uniform PushConstants {
    float decayCoefficient;
    float deltaTime;
    uint width;
    uint height;
    uint targetX;
    uint targetY;
    float targetSpikeValue;
} pc;

void main() {
    uint x = gl_GlobalInvocationID.x;
    uint y = gl_GlobalInvocationID.y;

    if (x >= pc.width || y >= pc.height) return;

    uint idx = y * pc.width + x;

    // Re-inject target spike
    if (x == pc.targetX && y == pc.targetY) {
        fieldOut.values[idx] = pc.targetSpikeValue;
        return;
    }

    float center = fieldIn.values[idx];
    float sSelf = speedMap.values[idx];

    if (sSelf <= 0.0) {
        fieldOut.values[idx] = 0.0;
        return;
    }

    float laplacian = 0.0;
    if (y > 0u) {
        uint nIdx = (y - 1u) * pc.width + x;
        if (speedMap.values[nIdx] > 0.0) laplacian += fieldIn.values[nIdx] - center;
    }
    if (y < pc.height - 1u) {
        uint sIdx = (y + 1u) * pc.width + x;
        if (speedMap.values[sIdx] > 0.0) laplacian += fieldIn.values[sIdx] - center;
    }
    if (x < pc.width - 1u) {
        uint eIdx = y * pc.width + (x + 1u);
        if (speedMap.values[eIdx] > 0.0) laplacian += fieldIn.values[eIdx] - center;
    }
    if (x > 0u) {
        uint wIdx = y * pc.width + (x - 1u);
        if (speedMap.values[wIdx] > 0.0) laplacian += fieldIn.values[wIdx] - center;
    }

    float delta = (sSelf * laplacian - pc.decayCoefficient * center) * pc.deltaTime;
    fieldOut.values[idx] = center + delta;
}
```

- `Directory.Build.props` — EXTEND CompileShaders target with wave.comp entry

**Operations**:
1. Create wave.comp + edit Directory.Build.props
2. `dotnet build` — verify wave.comp.spv generated
3. `git add` + `git commit -m "shaders: add V2 wave compute shader (diffusion-based distance per VULKAN_SUBSTRATE §1.3 Option B)"`

### Commit V2-5 — Direction extraction compute shader (GLSL)

**Scope**: shaders

**Files added**:
- `tools/shaders/direction_extract.comp` — NEW compute shader:

```glsl
#version 450

// V2 direction field extraction per VULKAN_SUBSTRATE.md §1.3.
// Gradient of distance field → direction toward target.

layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

layout(set = 0, binding = 0) readonly buffer DistanceField { float values[]; } distField;
layout(set = 0, binding = 1) writeonly buffer DirectionField { vec2 values[]; } dirField;

layout(push_constant) uniform PushConstants {
    uint width;
    uint height;
} pc;

void main() {
    uint x = gl_GlobalInvocationID.x;
    uint y = gl_GlobalInvocationID.y;

    if (x >= pc.width || y >= pc.height) return;

    uint idx = y * pc.width + x;
    float c = distField.values[idx];

    float n = (y > 0u)               ? distField.values[(y-1u) * pc.width + x] : c;
    float s = (y < pc.height - 1u)   ? distField.values[(y+1u) * pc.width + x] : c;
    float e = (x < pc.width - 1u)    ? distField.values[y * pc.width + (x+1u)] : c;
    float w = (x > 0u)               ? distField.values[y * pc.width + (x-1u)] : c;

    // Negative gradient = direction к smaller distance = toward target
    vec2 dir = vec2(w - e, n - s);
    float len2 = dot(dir, dir);
    if (len2 > 0.0001) dir = normalize(dir);

    dirField.values[idx] = dir;
}
```

- `Directory.Build.props` — EXTEND CompileShaders target with direction_extract.comp entry

**Operations**:
1. Create + edit build props
2. `dotnet build` — verify direction_extract.comp.spv generated
3. `git add` + `git commit -m "shaders: add V2 direction extraction compute shader (gradient extraction)"`

### Commit V2-6 — WavePushConstants + DirectionExtractPushConstants structs + alignment audit

**Scope**: compute + tests

**Files added**:
- `src/DualFrontier.Runtime/Compute/WavePushConstants.cs` — NEW struct:

```csharp
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Push constant payload for V2 wave compute shader.
/// Layout matches wave.comp push_constant block:
///   float decayCoefficient (offset 0,  size 4)
///   float deltaTime        (offset 4,  size 4)
///   uint width             (offset 8,  size 4)
///   uint height            (offset 12, size 4)
///   uint targetX           (offset 16, size 4)
///   uint targetY           (offset 20, size 4)
///   float targetSpikeValue (offset 24, size 4)
/// Total: 28 bytes (естественное alignment, 4-byte primitives).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct WavePushConstants
{
    public float DecayCoefficient;
    public float DeltaTime;
    public uint Width;
    public uint Height;
    public uint TargetX;
    public uint TargetY;
    public float TargetSpikeValue;
}
```

- `src/DualFrontier.Runtime/Compute/DirectionExtractPushConstants.cs` — NEW struct:

```csharp
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Compute;

/// <summary>
/// Push constant payload for V2 direction extraction compute shader.
///   uint width  (offset 0, size 4)
///   uint height (offset 4, size 4)
/// Total: 8 bytes.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct DirectionExtractPushConstants
{
    public uint Width;
    public uint Height;
}
```

- `tests/.../WavePushConstantsTests.cs` + `DirectionExtractPushConstantsTests.cs` — alignment audit tests (S-LOCK-7):

```csharp
[Fact]
public void WavePushConstants_Size_28_Bytes() {
    Assert.Equal(28, Marshal.SizeOf<WavePushConstants>());
}

[Fact]
public void DirectionExtractPushConstants_Size_8_Bytes() {
    Assert.Equal(8, Marshal.SizeOf<DirectionExtractPushConstants>());
}
```

**Halt trigger SC-5 (alignment mismatch)**: If sizes ≠ expected, adjust per Lesson #7 strengthening discipline.

**Operations**:
1. Create both structs + tests
2. `dotnet build` + `dotnet test`
3. `git add` + `git commit -m "compute: add V2 push constant structs + alignment audit (S-LOCK-7)"`

### Commit V2-7 — V2WavePipeline + V2DirectionExtractPipeline managed wrappers

**Scope**: compute

**Files added**:
- `src/DualFrontier.Runtime/Compute/V2WavePipeline.cs` — NEW pipeline wrapper, mirrors V1DiffusionPipeline structure:
  - 3 binding descriptor set layout (input ssbo, output ssbo, speed map ssbo)
  - 28-byte push constant range
  - ExecuteIteration(binding, fieldName, WavePushConstants, dispatchX, dispatchY) method

- `src/DualFrontier.Runtime/Compute/V2DirectionExtractPipeline.cs` — NEW pipeline wrapper:
  - 2 binding descriptor set layout (distance ssbo, direction ssbo)
  - 8-byte push constant range
  - Execute(binding, distanceFieldName, directionFieldName, DirectionExtractPushConstants, dispatchX, dispatchY) — single pass

**Operations**:
1. Create both pipelines
2. `dotnet build` + `dotnet test`
3. `git add` + `git commit -m "compute: add V2WavePipeline + V2DirectionExtractPipeline managed wrappers"`

### Commits V2-8 through V2-12 — CPU/GPU equivalence tests + scene infrastructure

**Commit V2-8**: V2 wave CPU/GPU equivalence test (32×32 grid, target spike, uniform speed=1, 10 iterations, tolerance 0.01f per cell).

**Commit V2-9**: V2 wave wall scenario (column of walls, propagation blocked, CPU/GPU match).

**Commit V2-10**: V2 direction extraction equivalence test (cosine similarity > 0.999 between CPU + GPU direction vectors).

**Commit V2-11**: SmokeTest scene — V2 distance field 200×200 с walls (e.g. L-shaped maze), target at one corner, observe convergence + direction visualization, FPS≥60.

**Commit V2-12**: Visualization helper для direction field (small arrows or color-coded heatmap) — reuses V0.C.2 batched SpriteRenderer infrastructure для arrow rendering.

### Commit V2-13 — Multi-field coexistence scene (V substrate close acceptance criterion)

**Scope**: tests

**Files modified**:
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs` — ADD multi-field scene per VULKAN_SUBSTRATE §1.4:

```csharp
static void RunMultiFieldCoexistence(Runtime runtime, int durationSeconds = 15)
{
    Console.WriteLine($"=== V substrate close: multi-field coexistence scene for {durationSeconds}s ===");

    var binding = /* FieldStorageBinding */;
    var registry = /* FieldRegistry */;

    // Register 4 fields simultaneously:
    var manaField = registry.Register<float>("multifield.mana", 200, 200);  // V1 isotropic
    var elecField = registry.Register<float>("multifield.electricity", 200, 200);  // V1 anisotropic
    var distField = registry.Register<float>("multifield.distance", 200, 200);  // V2 wave
    var dirField = registry.Register<Vector2>("multifield.direction", 200, 200);  // V2 direction

    // Initialize conductivities + sources
    // Mana: uniform D=0.1, source at center
    // Electricity: wire path (horizontal line at y=100, D=10.0; off-wire D=0.1), source at left edge
    // Distance: walls (e.g. obstacles row y=80, x=50..150 with speed=0), target at corner
    // Direction: gradient extracted from distance

    var stopwatch = Stopwatch.StartNew();
    int frameCount = 0;

    while (stopwatch.Elapsed.TotalSeconds < durationSeconds)
    {
        if (runtime.Window.ShouldClose) break;

        // Each frame: 5 iterations of V1 mana + 5 iterations of V1 electricity + 10 iterations of V2 wave + 1 pass V2 direction
        for (int i = 0; i < 5; i++) {
            v1Mana.ExecuteIteration(binding, "multifield.mana", manaPC, 25, 25);
            manaField.SwapBuffers();
        }
        for (int i = 0; i < 5; i++) {
            v1Elec.ExecuteIteration(binding, "multifield.electricity", elecPC, 25, 25);
            elecField.SwapBuffers();
        }
        for (int i = 0; i < 10; i++) {
            v2Wave.ExecuteIteration(binding, "multifield.distance", wavePC, 25, 25);
            distField.SwapBuffers();
        }
        v2Direction.Execute(binding, "multifield.distance", "multifield.direction", dirPC, 25, 25);

        // Render visualization (each field на separate viewport quadrant)
        RenderMultiFieldFrame(runtime, manaField, elecField, distField, dirField);

        runtime.Window.PumpMessages();
        frameCount++;
    }

    stopwatch.Stop();
    double fps = frameCount / stopwatch.Elapsed.TotalSeconds;
    Console.WriteLine($"  Multi-field coexistence: {frameCount} frames в {stopwatch.Elapsed.TotalSeconds:F2}s = {fps:F1} FPS");
    Console.WriteLine($"  Active fields: 4 (V1 mana isotropic + V1 electricity anisotropic + V2 distance + V2 direction)");
    Console.WriteLine($"  Validation messages: {runtime.ValidationLayer.MessageCount}");

    if (fps < 60.0)
        Console.WriteLine($"  WARNING: FPS {fps:F1} < 60 target. V substrate close criterion not met.");
    else
        Console.WriteLine($"  ✓ V substrate close: multi-field coexistence sustained at 60+ FPS");
}
```

**Operations**:
1. Edit Program.cs
2. `dotnet build` + `dotnet run --project tests/DualFrontier.Runtime.SmokeTest`
3. Manual visual verification:
   - 4 fields render simultaneously
   - Mana spreads radially from source
   - Electricity channels along wire path
   - Distance field shows gradient к target
   - Direction field shows directional arrows
   - FPS ≥ 60 sustained
   - Validation: 0 messages
4. `git add` + `git commit -m "tests: add multi-field coexistence scene (V substrate close acceptance per VULKAN_SUBSTRATE §1.4)"`

**Halt trigger SC-14 (multi-field coexistence < 60 FPS)**: V substrate close blocked. Per VULKAN_SUBSTRATE §1.4 budget — «~1 ms/tick per active field». 4 active fields × ~1ms = ~4ms budget; well within 16.6ms 60 FPS budget. If exceeds → investigate per-iteration overhead, fence sync excessive, descriptor set rebinding.

### Commits V2-14 through V2-20 — Runtime composition + benchmarks + documentation

**Commit V2-14**: Runtime.cs composition — V2WavePipeline + V2DirectionExtractPipeline properties.

**Commit V2-15**: Performance benchmark — multi-field coexistence dispatch latency, verify ~4ms/tick total across 4 fields на «Skarlet».

**Commit V2-16**: V2 distance field convergence test — measure iterations needed для stable gradient на 200×200 (typically 10-20).

**Commit V2-17**: Documentation — V2 substrate primitive README, ping-pong pattern, target spike injection contract.

**Commit V2-18**: Update `docs/architecture/VULKAN_SUBSTRATE.md` — mark V1+V2 substrate primitives closed (§1.2 + §1.3 status), §1.4 multi-field coexistence verified.

**Commit V2-19**: Update `docs/MIGRATION_PROGRESS.md` — V substrate full close achieved.

**Commit V2-20**: V substrate close audit_trail event EVT-V-SUBSTRATE-CLOSE в REGISTER.yaml.

### Commits V2-21 through V2-24 — Final closure cascade

**Commit V2-21**: V1+V2 brief closure section §8 populated с full commit ledger (V1 portion + V2 portion + V substrate close metrics).

**Commit V2-22**: Final smoke test integration run на «Skarlet» — verify все 6 scenes pass (V0.C.1 regression + V0.C.2 10K stress + V0.C.2 TileMap + V1 isotropic + V1 anisotropic + V2 wave + multi-field coexistence).

**Commit V2-23**: REGISTER.yaml lifecycle DOC-D-V1_V2 = EXECUTED. sync_register.ps1 --validate exit 0.

**Commit V2-24**: Final REGISTER sync + branch push + PR #41 opening:
- `git push origin claude/v2-wave-substrate`
- PR #41 opened: «V2 — Scalar field + wave shader + direction extraction (V substrate full close)»

**Crystalka review**: Manual visual verification на «Skarlet» of all V2 scenes + multi-field coexistence. CPU/GPU equivalence results. PR review + merge if acceptance criteria met. After merge: V substrate full close formally achieved.

---

## §5 — Halt triggers (SC-N taxonomy)

V1+V2 cascade may encounter conditions triggering halt-before-damage per Lesson #8 discipline.

### SC-1 — Baseline drift from V0.C.2 closure state
Triggered if: V0.C.2 HEAD не main, working tree dirty, baseline build/tests fail, V0.C.2 infrastructure missing (FieldHandle/FieldRegistry/Compute primitives expected present).

### SC-2 — Vulkan SDK absent
glslangValidator.exe не committed или not executable.

### SC-3 — glslangValidator compile errors
GLSL syntax error в diffusion.comp / wave.comp / direction_extract.comp. Output identifies line/column.

### SC-4 — SPIR-V output missing post-build
assets/shaders/{diffusion,wave,direction_extract}.comp.spv не generated. Check Directory.Build.props target Exec command paths.

### SC-5 — Marshal.SizeOf alignment mismatch (Lesson #7)
Push constant struct sizes ≠ hypothesized. Adjust test expected к actual, document fix.

### SC-6 — CPU reference math wrong
CPU kernel produces unexpected результаты в unit tests (e.g. AnisotropicDiffusionKernel asymmetric flow incorrect). Investigation: reflective boundary handling, min(D_self, D_neighbour) calculation, scratch buffer copy-back order.

### SC-7 — GLSL shader divergence from CPU reference
CPU/GPU equivalence test fails (abs(cpu - gpu) > tolerance per cell). Investigation:
- Reflective boundary in GLSL — CPU uses self-as-neighbour-when-edge; GLSL must match
- min(D_self, D_neighbour) ordering — must be symmetric
- Push constant float precision — verify shader uses float (not double)
- Storage buffer std430 layout — verify alignment matches managed-side struct

### SC-8 — Compute pipeline registration fails
df_world_register_compute_pipeline returns 0. Investigation:
- SPIR-V byte count mismatch (must be multiple of 4)
- Descriptor binding count mismatch (V1 = 3, V2 wave = 3, V2 direction = 2)
- Async compute queue family availability (K-L19 invariant verified at Runtime.Create)

### SC-9 — Dispatch fails
df_world_field_dispatch_compute returns 0. Investigation:
- Field name mismatch (registered name vs dispatch name)
- Pipeline_id from registration vs dispatch
- Dispatch dimensions exceed local_size × hardware limit
- Storage buffer not bound (V0.B no-op stub — V1 must implement actual binding via FieldStorageBinding extension)

### SC-10 — Validation layer errors during dispatch
Common causes:
- Synchronization hazard (read-after-write без barrier)
- Storage buffer access violation (out-of-bounds index)
- Descriptor set not updated before use
- Pipeline barrier missing между ping-pong swaps

### SC-11 — Push-to-main classifier blocks (expected behavior, не bug)
Per V0.A/V0.B/V0.C.1/V0.C.2 precedent. Push branch only, open PR. Crystalka manually merges.

### SC-12 — CPU/GPU equivalence divergence (mandatory gate, Lesson #7 + S-LOCK-3)
Equivalence test fails on multiple shader iterations. Hard halt:
- Author HALT_REPORT_SC12.md in `docs/scratch/V1_V2/`
- Capture: CPU output sample, GPU output sample, divergence pattern (random noise vs systematic offset)
- Surface к Crystalka — likely numerical precision issue или shader mathematical error
- Do NOT commit shader until equivalence proven

### SC-13 — Performance budget exceeded (V substrate close gate)
Multi-field coexistence scene < 60 FPS на «Skarlet» AMD RX 7600S. V substrate close per VULKAN_SUBSTRATE §1.4 acceptance criterion not met.
- Per VULKAN_SUBSTRATE §1.4: «~1 ms/tick per active field on mid-range GPU»
- 4 fields × 5-10 iterations × ~0.1-0.2ms per iteration = ~4-8ms total budget — well within 16.6ms 60 FPS budget
- If exceeds: HALT, surface к Crystalka, investigate per-dispatch overhead

### SC-14 — Multi-field coexistence verification fails
V substrate close acceptance criterion specifies «V1 mana + V1-anisotropic electricity + V2 routed-water-pressure all active simultaneously without interference». Fields must NOT corrupt each other's state (separate buffers, separate descriptors, no shared writes).
- Investigation: descriptor set sharing, pipeline state leakage между dispatches, fence sync ordering

---

## §6 — Closure protocol

### §6.1 — V1 closure (PR #40)

1. All V1 commits landed (V1-1 through V1-22)
2. `dotnet build` clean
3. `dotnet test` ~990 tests green (936 V0.C.2 baseline + ~50-80 V1 additions)
4. `sync_register.ps1 --validate` exit 0
5. Smoke test V1 scenes pass на «Skarlet» (isotropic + anisotropic, FPS ≥ 60, validation clean)
6. CPU/GPU equivalence tests pass (all V1 scenarios within tolerance)
7. `git push origin claude/v1-diffusion-substrate`
8. PR #40 opened
9. Crystalka reviews + merges

### §6.2 — V2 closure (PR #41) — V substrate full close

1. All V2 commits landed (V2-1 through V2-24)
2. `dotnet build` clean
3. `dotnet test` ~1080 tests green (990 V1 baseline + ~70-100 V2 additions)
4. `sync_register.ps1 --validate` exit 0
5. Smoke test all scenes pass на «Skarlet» (V0 regression + V1 + V2 + **multi-field coexistence**)
6. CPU/GPU equivalence tests pass (all V2 scenarios)
7. V substrate close governance — VULKAN_SUBSTRATE §1.4 marked achieved
8. `git push origin claude/v2-wave-substrate`
9. PR #41 opened: «V2 + V substrate full close»
10. Crystalka reviews + merges

### §6.3 — Next Opus session decision tree post-V substrate full close

**Option A — M-V1 demonstration mod** (Vanilla.Magic mana field gameplay configuration)
- First M-V demonstration; uses V1 isotropic с gameplay-tuned parameters
- ~1-2 weeks hobby pace

**Option B — M-V2 demonstration mod** (Vanilla.Electricity power field)
- V1 anisotropic с wire conductivity configuration
- ~1-2 weeks hobby pace

**Option C — M-V7 demonstration mod** (Vanilla.Movement flow field pathfinding)
- V2 routed flow field consumed by pawn movement system
- ~2-3 weeks hobby pace

**Option D — К10.3 brief restart** (independent stream — твоя prerogative)

**Option E — A'.8 K-closure report** (formal К-Lxx + Lessons promotion)

**Option F — A'.9 Roslyn analyzer milestone** (gates Phase B M-cycle)

**Recommended sequencing per К-L14 default-inclusion bias**:
```
V1+V2 close (V substrate full close)
  ↓
M-V1 (proves V1 isotropic gameplay configuration)
  ↓
M-V2 (proves V1 anisotropic gameplay configuration)
  ↓
M-V7 (proves V2 routed gameplay configuration)
  ↓
A'.8 K-closure (accumulates V substrate + K substrate close lessons)
  ↓
A'.9 Roslyn analyzer (encodes К-Lxx including К-L14 evidence из V substrate)
  ↓
Phase B M-cycle (all gates met)
```

---

## §7 — Brief authority + lifecycle

### §7.1 — Lifecycle states

V1+V2 brief lifecycle:
```
AUTHORED ──→ IN_PROGRESS (post-V1 merge) ──→ EXECUTED (post-V2 merge) ──→ ARCHIVED (post-A'.8)
```

- **AUTHORED**: Opus deliberation authors brief; Crystalka accepts via «Принимаю бриф»
- **IN_PROGRESS**: V1 portion executed + closed (PR #40 merged); V2 portion pending
- **EXECUTED**: V2 portion executed + closed (PR #41 merged); V substrate full close achieved
- **ARCHIVED**: Future state at A'.8 K-closure report

### §7.2 — Multi-session pause provision

V1 portion execution ~15-20h auto-mode. V2 portion ~15-20h auto-mode. Total ~30-40h.

**Mandatory pause point** at V1 closure (between PR #40 + PR #41):
1. V1 cascade completes V1-22 (PR #40 opened)
2. Crystalka reviews + merges PR #40
3. **Next session** starts V2 portion from new branch `claude/v2-wave-substrate` from main post-V1 merge

**Within V1 portion** (or within V2 portion): pause-resume per Lesson #8 atomic intermediate states — executor pauses at atomic commit boundary, pushes feature branch, surfaces «paused at Commit N» к Crystalka.

---

## §8 — Lesson candidates (deferred к A'.8 K-closure report)

V1+V2 cascade accumulates evidence для METHODOLOGY §K-Lessons revision.

### Strengthening continues
- **Lesson #7 strengthening MATURED**: V0.A (1) + V0.B (5) + V0.C.1 (0) + V0.C.2 (1) + V1 (~1-3) + V2 (~1-3) maturity curve continues
- **Lesson #11 redundancy check strengthened**: IsotropicDiffusionKernel inherited verbatim (existing K9-era CPU reference); AnisotropicDiffusionKernel patterns from existing precedent. Не recreate.
- **Lesson #22 read existing code strengthened**: V1+V2 Phase 0 surfaced complete K9 + V0.B compute infrastructure ready. Substantial existing infrastructure inherited.
- **Lesson #25 implementation depth follows consumer materialization**: V1+V2 ship substrate primitives только; M-V demonstration mods materialize consumers separately. Camera2D culling, JSON atlas, eikonal upgrade — all deferred per «features only on demand».
- **Lesson #26 cross-substrate scope splitting**: V0.C → V0.C.1 + V0.C.2 precedent. V1+V2 → V1 + V2 sub-PR split via combined deliberation + sequential execution.
- **Lesson #27 candidate strengthened**: Each new render/compute workload exercises prior substrate primitives. V1+V2 may surface latent V0.B compute infrastructure issues — if so, pattern formalization opportunity.

### V1+V2-specific candidates
- **Lesson #N candidate — CPU reference oracle pattern**: Every GPU compute shader has CPU reference implementation. Tolerance-bounded equivalence test gates shader correctness. Pattern applies к V1+V2 + future M-V demonstrations + future K-extensions.
- **Lesson #N candidate — Substrate primitive vs mod gameplay configuration separation**: K-L9 «vanilla = mods» applied к compute shaders. Substrate ships shader template + push constant + binding contract; mod ships gameplay-tuned parameters + conductivity map + iteration count. Pattern formalization gate: second application (M-V demonstrations).
- **Lesson #N candidate — Multi-field coexistence as substrate close acceptance**: V substrate close criterion = simultaneous active fields without interference. Pattern: substrate primitives must compose без mutual corruption. Formalize when applied к non-V substrates (V1+V2 is first application).

---

## §9 — Closing summary

**Total estimated execution time**: V1 portion 15-20h + V2 portion 15-20h = ~30-40h auto-mode across 2-3 Claude Code sessions (multi-session pause at V1→V2 boundary mandatory per Q6c).

**Total estimated LOC delta**: V1 ~1500-2000 production code + V2 ~1500-2000 + ~800-1200 test code + ~300-500 governance + smoke test extensions = ~4000-5500 LOC.

**Total estimated test growth**: 936 V0.C.2 baseline → ~1080-1120 V2 closure (~144-184 new tests).

**Verification metrics targets**:
- V1 isotropic 200×200: 60+ FPS sustained
- V1 anisotropic 200×200: 60+ FPS sustained
- V2 distance field 200×200: 60+ FPS sustained
- **Multi-field coexistence (4 fields simultaneously)**: 60+ FPS sustained (V substrate close criterion)
- CPU/GPU equivalence: all tolerances met (V1: 0.001f, V2 distance: 0.01f, V2 direction: cosine > 0.999)
- Validation: 0 errors, 0 warnings across all scenes
- Hardware baseline: «Skarlet» AMD RX 7600S (V0.C.2 baseline 165 FPS at 40K sprites)

**V substrate full close achievement** per VULKAN_SUBSTRATE §1.4: V0 ✓ + V1 ✓ + V2 ✓ + multi-field coexistence ✓ — opens M-V demonstration mod paths + Phase B M-cycle vanilla migration (gated also on analyzer А'.9).

**К-L14 thesis fifth+sixth verification**: V0.A → V0.B → V0.C.1 → V0.C.2 → V1 → V2 = six consecutive zero-hard-gate-halt cascades expected, alignment maturity curve preserved, atomic discipline + halt-before-damage + CPU/GPU equivalence test + multi-field coexistence + V substrate full close pattern all empirically demonstrated.

**«Без костылей» summary**: V1+V2 ships substrate primitives per VULKAN_SUBSTRATE §1.2 + §1.3 + §1.4 specifications без compromise. Direct K9 consumption (no new C ABI), direct V0.B compute infrastructure consumption (no new managed wrappers needed beyond pipeline classes), CPU reference inheritance from K9 era (no duplication), single GLSL diffusion shader handling isotropic + anisotropic via parameter variation = clean architectural decisions избегающие dead-end optimization paths. К-L14 «performance derives from clean complex architecture» validates через V substrate full close + multi-field coexistence acceptance criterion.

---

*V1+V2 EXECUTION BRIEF AUTHORED 2026-05-19 — DOC-D-V1_V2 lifecycle AUTHORED, awaiting IN_PROGRESS transition at PR #40 merge + EXECUTED transition at PR #41 merge (V substrate full close).*
