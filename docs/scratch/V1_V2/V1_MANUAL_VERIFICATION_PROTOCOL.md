# V1 Manual Visual Verification Protocol

Per S-LOCK-9 manual gate + V1+V2 brief §6.1 closure protocol. This document defines
the manual verification steps Crystalka performs on «Skarlet» (ASUS TUF Gaming A16,
AMD RX 7600S) prior к PR #40 (V1 closure) review + merge.

## Pre-merge gate (before PR #40)

Crystalka runs `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` on «Skarlet».
The smoke test executes V0 regression scenes followed by V1 substrate primitive scenes:

1. V0.C.1 single-sprite regression (Kenney pawn or procedural sprite at NDC center)
2. V0.C.2 R.2 10K sprite stress
3. V0.C.2 R.3 200×200 TileMap (multi-cycle, WASD camera pan)
4. **V1-12 V1 isotropic diffusion 200×200** (new)
5. **V1-13 V1 anisotropic diffusion wire path 200×200** (new)
6. **V1-17 V1 isotropic dispatch latency benchmark** (new)

## Acceptance criteria

### V0 regression preservation

- V0.C.1 single sprite: visible at viewport center, FPS ≥ 60, validation 0/0/0
- V0.C.2 10K stress: 10,000 sprites, single vkCmdDrawIndexed, FPS ≥ 60
- V0.C.2 200×200 TileMap: WASD pan smooth, 4 multi-cycle draws, FPS ≥ 60

If any V0 scene regresses → HALT-SC-1 (V0 substrate inheritance violation).

### V1-12 V1 isotropic diffusion 200×200 scene

- Compute round-trip succeeds (0 dispatches reported FAIL)
- Console reports:
  - `Field 200×200: D=0.1, K=0, dt=0.5, 5 iter/frame`
  - Non-zero `center` value (substantially below the initial 1000 because mass spreads)
  - Non-zero `adjacent` value (≫ 0 — flow reached neighbours)
  - `total` value approximately equal к initial mass 1000 (no decay → mass conservation)
  - `[PASS] V1-12 success criterion met (60+ FPS sustained)`
- Validation: 0 errors, 0 warnings new since V0.C.2 baseline

### V1-13 V1 anisotropic diffusion wire path scene

- Compute round-trip succeeds
- Console reports:
  - `Wire(x=0)` value substantially below initial 1000 (source loses к neighbours)
  - `wire(x=W/4)` value > 0 (propagation along high-D row reached far cells)
  - `wire(x=W/2)` value > 0 (further along wire still received some flow)
  - `Off-wire(y-1)` value much smaller than wire value at same x (channeling factor visible)
  - `Off-wire(y-10)` near zero (far off-wire = essentially insulated)
  - `Wire ratio` ≥ 2× (wire-channelled flow is at least 2× any nearby off-wire flow)
  - `[PASS] V1-13 success criterion met (60+ FPS sustained)`
- Validation: 0 errors, 0 warnings new since V0.C.2 baseline

### V1-17 dispatch latency benchmark

- Console reports:
  - `Field 200×200, 200 single-iter dispatches (warmup=10)`
  - `mean: <X.XXX> ms/iter`
  - `[PASS] V1-17 latency within ~1 ms/iter budget` OR `[WARN] ... < 2 ms` (acceptable)
  - **HALT-SC-13** if latency > 2 ms/iter (investigate dispatch overhead, fence sync,
    descriptor rebinding per VULKAN_SUBSTRATE.md §1.4 budget)

### Aggregate

- Sum of all scene FPS ≥ 60 thresholds met
- Validation totals after V1 scenes: errors = 0, warnings unchanged from V0 baseline
- Smoke test exits cleanly с return code 0
- Console reports `V0.C.1 smoke test PASS`

## Regression checks (V0 substrate inheritance preserved)

- HardwareCapabilityCheck passes (K-L19 preserved)
- Swapchain recreation works on window resize (V0.C.1 latent bug fix preserved)
- Per-image renderFinished semaphore pattern preserved
- Clean shutdown (no leaked Vulkan handles per validation)
- Mixed [LibraryImport]/[DllImport] convention preserved (Lesson #22)
- Native VkPipeline + VkCmdDispatch path (V1-5a..d) functional
- Per-field shadow VkBuffer binding works (V1-5c implementation)

## Halt triggers (per V1+V2 brief §5 SC-N taxonomy)

If any visual verification fails → HALT, surface к Crystalka, do NOT push к remote.

| Failure | Halt class | Investigation hint |
|---------|-----------|--------------------|
| V0 scene regresses | SC-1 | Verify baseline drift from V0.C.2 closure HEAD |
| V1 isotropic dispatch fails | SC-8 / SC-9 | Pipeline registration или dispatch round-trip — check pipeline_id != 0, descriptor binding count = 3, push constant size = 16 |
| V1 anisotropic shows isotropic-like pattern | SC-6 | CPU AnisotropicDiffusionKernel math, asymmetric `min(D_self, D_neighbour)` rule, conductivity buffer binding 2 not bound |
| V1 fps < 60 | SC-13 | Per-iter compute overhead, fence sync excessive, descriptor set rebinding на every dispatch |
| Validation error | S-LOCK-9 | Shader storage buffer barriers, sync hazard, out-of-bounds index, missing descriptor set update before use |
| Latency > 2 ms/iter | SC-13 | Investigate dispatch overhead, fence sync, descriptor rebinding |

## Hardware baseline

«Skarlet» = ASUS TUF Gaming A16. AMD Radeon RX 7600S (verified K-L19 hardware tier baseline).

V0.C.2 baseline: 165 FPS at 40K sprites.
V1 expected (per V1+V2 brief §1.1): 60+ FPS on 200×200 fields с 5 iter/frame.
V1-17 latency target: ~1 ms/iter (per VULKAN_SUBSTRATE.md §1.4 budget).

## Validation log capture

Smoke test prints validation message counts at end of run. Expected: zero errors,
zero new warnings beyond V0.C.2 baseline. V1 substrate adds substantial new compute
surface (3 storage buffer bindings + push constants + per-iteration dispatch) — каждая
new call is а potential validation message source. Zero tolerance for new errors.

## RenderDoc capture procedure (optional, recommended for first V1 PR)

1. Launch RenderDoc 1.31+
2. Launch Application → Executable Path:
   `tests/DualFrontier.Runtime.SmokeTest/bin/Debug/net8.0/DualFrontier.Runtime.SmokeTest.exe`
3. Capture frame during V1 isotropic scene → verify single vkCmdDispatch call per
   iteration (5 dispatches per smoke frame)
4. Inspect descriptor set bindings — verify 3 storage buffers (input/output/conductivity)
5. Inspect push constant range — verify 16-byte block с DecayCoefficient + DeltaTime +
   Width + Height fields
6. Inspect SPIR-V disassembly — verify diffusion.comp shader entry point

## К-L14 thesis evidence accumulation

V0.A → V0.B → V0.C.1 → V0.C.2 → V1 = five consecutive zero-hard-gate-halt cascades
если V1 closes per pattern. К-L14 «performance derives from clean complex architecture»
extends к compute pipeline surface: substrate primitive + CPU reference + tolerance-bounded
equivalence test + Runtime composition factories = clean architecture pattern, not а
single-shader hack.
