# V0.C.2 Manual Visual Verification Protocol

Per Lesson #27 candidate + S-LOCK-9 manual gate. This document defines the manual
verification steps Crystalka performs on «Skarlet» (ASUS TUF Gaming A16, AMD RX 7600S)
prior to PR review + merge.

## Pre-commit gate (before Commit 17 closure)

Crystalka runs `dotnet run --project tests/DualFrontier.Runtime.SmokeTest` on «Skarlet».
The smoke test executes three scenes sequentially:

1. V0.C.1 single-sprite regression (Kenney pawn or procedural sprite at NDC center)
2. V0.C.2 R.2 10K sprite stress (single atlas, randomized positions + tints)
3. V0.C.2 R.3 200×200 TileMap (40K tiles, 4-cycle multi-pass, WASD camera pan)

## Acceptance criteria

### V0.C.1 single-sprite scene (regression check)

- Sprite (Kenney pawn or procedural diamond) visible centered in window
- Validation log: 0 errors, 0 warnings, 0 info messages
- FPS ≥ 60 sustained (V0.C.1 baseline: 164 FPS on «Skarlet»)
- Window opens/closes cleanly; resize handler emits WindowResizeEvent + framebuffers
  recreated without leaked handles

### V0.C.2 R.2 10K stress test scene

- 10,000 sprites visible с randomized positions across viewport
- Each sprite shows distinct tint (randomly mixed RGB) + procedural atlas tile pattern
  (solid / checker / gradient / ring varieties)
- FPS ≥ 60 sustained for 5 seconds (≥ 300 frames)
- Validation: 0 errors, 0 warnings
- **RenderDoc inspection (S-LOCK-7a manual gate)**: single vkCmdDrawIndexed call per
  frame visible (single atlas → single atlas group → single draw call)
- Console reports `[PASS] R.2 success criterion met (60+ FPS sustained)`

### V0.C.2 R.3 200×200 TileMap scene

- 200×200 tile grid visible (partial viewport region; world coords [0..3200] in both X+Y)
- WASD keys pan camera smoothly без stutter
- FPS ≥ 60 sustained for 5 seconds (≥ 300 frames)
- Validation: 0 errors, 0 warnings
- **RenderDoc inspection**: 4 vkCmdDrawIndexed calls per frame visible (per S-LOCK-5a
  multi-cycle, single atlas across 4 cycles = 4 atlas group draws)
- Console reports `[PASS] R.3 success criterion met (60+ FPS sustained on 40K tiles)`

### Regression checks (V0.A + V0.B + V0.C.1 invariants preserved)

- HardwareCapabilityCheck passes (K-L19 preserved)
- Swapchain recreation works on window resize (V0.C.1 latent bug fix preserved)
- Per-image renderFinished semaphore pattern preserved (V0.C.1 latent bug fix preserved)
- Clean shutdown (no leaked Vulkan handles per validation)
- Mixed [LibraryImport]/[DllImport] convention preserved (Lesson #22)
- Sprite shaders SPIR-V (sprite.vert.spv + sprite.frag.spv from V0.C.1) loaded successfully

## Halt triggers (per V0.C.2 brief §4 SC-N taxonomy)

If any visual verification fails → HALT, surface к Crystalka, do NOT push к remote.
Specific failures map к brief halt classes:

- 10K stress FPS < 60 → SC-8 (investigate ring buffer overhead, descriptor caching, validation cost)
- 200×200 TileMap FPS < 60 → SC-9 (investigate multi-cycle overhead, per-cycle rebinding)
- WASD pan broken → SC-10 (verify Win32 dispatch, event types, InputEventQueue drain)
- Validation error/warning → S-LOCK-10 violation (investigate vkApi call patterns,
  framebuffer staleness, descriptor lifecycle)

## Hardware baseline

«Skarlet» = ASUS TUF Gaming A16. AMD Radeon RX 7600S (verified K-L19 hardware tier baseline).
V0.C.1 baseline: 164 FPS for single sprite over 820 frames.
V0.C.2 expected: 10K sprites at 60-120 FPS, 200×200 TileMap at 60-100 FPS.

## Validation log capture

Smoke test prints validation message counts at end of each scene. Expected: zero errors
+ zero warnings across all three scenes. Info messages tolerated (typically zero on
properly-cleaned validation layer setup).

## RenderDoc capture procedure (optional)

1. Launch RenderDoc 1.31+
2. Launch Application → Executable Path: `tests/DualFrontier.Runtime.SmokeTest/bin/Debug/net8.0/DualFrontier.Runtime.SmokeTest.exe`
3. Capture frame during 10K stress scene → verify single vkCmdDrawIndexed call
4. Capture frame during TileMap scene → verify 4 vkCmdDrawIndexed calls
5. Inspect vertex/index buffer contents — verify uint16 index pattern (0,1,2,2,3,0) per quad
6. Inspect descriptor set — verify VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER binding 0
   references the procedural atlas image

## К-L14 thesis evidence accumulation

V0.A → V0.B → V0.C.1 → V0.C.2 = four consecutive zero-hard-gate-halt cascades если
V0.C.2 closes per pattern. К-L14 «performance derives from clean complex architecture»
empirically validated on V substrate matching K substrate K0..K10 development.
