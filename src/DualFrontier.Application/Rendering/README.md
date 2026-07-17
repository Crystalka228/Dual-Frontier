# Rendering — Render-backend contract

## Purpose
`IRenderer` is the single entry point for the visual backend. Application holds
only the contract; the concrete implementation lives in `DualFrontier.Launcher`
(Vulkan-native via `DualFrontier.Runtime` substrate). Reserved DevKit-tier
extension (`IDevKitRenderer`, dormant per К-extensions cascade #2 2026-05-23)
для future first-party DevKit work над Vulkan substrate.

## Dependencies
- `DualFrontier.Contracts` — base types (через `PresentationBridge`).

## Contents
- `IRenderer.cs` — `Initialize` / `RenderFrame` / `Shutdown` / `IsRunning`.
- `IDevKitRenderer.cs` — dormant DevKit-tier extension (debug gizmos,
  profiler overlay, entity highlighting); reserved для future first-party
  DevKit над Vulkan substrate.

## Rules
- No `using Godot;` or `using Silk.NET;` (paths retired per К-extensions cascade
  #2) — only the abstract contract.
- The implementation MUST drain `PresentationBridge` inside its `RenderFrame`.
- Every GPU resource is released in `Shutdown` — leaks are unacceptable.

## TODO
- [x] К-extensions cascade #2 — `LauncherRenderer` infrastructure scaffold
      (δ phase 2026-05-23 — defensive throws per Lesson #N12).
- [ ] К-extensions cascade #3 — `LauncherRenderer` real visual implementation
      (SpriteCatalog + scene state + Vulkan sprite recording).
- [ ] Future К-extensions / V-cycle — first-party DevKit materialization
      implementing `IDevKitRenderer` над Vulkan substrate.

## See also
- [VISUAL_ENGINE.md](/docs/architecture/historical/VISUAL_ENGINE.md) — historical
  (superseded) overview of pre-Vulkan DevKit vs Native strategy.
- [VULKAN_SUBSTRATE.md](/docs/architecture/VULKAN_SUBSTRATE.md) — current
  authoritative substrate spec.

---
register_id: DOC-F-SRC-APPLICATION-RENDERING
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: Application Rendering submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
