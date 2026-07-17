---
register_id: DOC-F-SRC-RUNTIME
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-18
last_modified: 2026-05-18
content_language: mixed
next_review_due: 2026-Q4
title: DualFrontier.Runtime — module doc
last_modified_commit: 5c6a064
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: 'Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (REGISTER_INVERSION_A_MEASURE_REPORT §4, verdict: enroll F/4); real git provenance.'
---

# DualFrontier.Runtime

**Purpose:** Generic 2D Vulkan substrate — window management, Vulkan instance/device/queues,
rendering primitives, sprite batching, texture loading, input events, UI primitives, compute
pipeline plumbing, scalar field compute primitives. Knows nothing of Domain. Could be
open-sourced separately.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../docs/architecture/VULKAN_SUBSTRATE.md) v1.0 LOCKED §2.2.

## V0.A scope (this commit cascade)

Foundation layer only:

- Win32 window lifecycle (`Window/`)
- Vulkan instance + physical/logical device + queue family enumeration (`Graphics/`)
- Validation layer (`VK_LAYER_KHRONOS_validation`) ALWAYS-ON в DEBUG (`Graphics/ValidationLayer.cs`)
- Pure P/Invoke surfaces (`Native/Win32/`, `Native/Vulkan/`) — minimal V0.A subset

Subsequent V0.B + V0.C add swapchain, compute pipeline plumbing, sprite/text/atlas batching,
PNG decoder, threading model integration, clear color → first textured quad.

## Dependency rules (per VULKAN_SUBSTRATE.md §2.4)

- **Rule 1:** Runtime references **nothing from Domain**. No `ProjectReference` to
  `DualFrontier.Contracts`, `.Core`, `.Components`, `.Events`, `.Systems`, `.AI`,
  `.Application`, `.Modding`, `.Persistence`.
- **Rule 3:** Internal layering — `Native.{Win32,Vulkan}` (lowest) → `Window/Input` →
  `Graphics` → `Compute` (V0.B) → `Sprite/Text` (V0.C) → `Diagnostic` → `Runtime.cs`
  facade (top).
- **Rule 4:** No layer skipping (e.g. Diagnostic does not import Native.Vulkan directly).
- **Rule 5:** Minimal public API; implementation details `internal`.

## Conventions

- Vulkan struct types: canonical `VkInstanceCreateInfo` naming (matches Vulkan 1.3 spec).
- Win32 struct types: canonical `WNDCLASSEX` naming (matches Win32 docs).
- C# wrapper classes: Pascal case (`VulkanInstance`, `Window`).
- P/Invoke surfaces: `internal` accessibility (do not leak past Runtime project boundary).
- Pure P/Invoke к `vulkan-1.dll` + `user32.dll` + `kernel32.dll` (S-LOCK-6 per V0.A brief).
