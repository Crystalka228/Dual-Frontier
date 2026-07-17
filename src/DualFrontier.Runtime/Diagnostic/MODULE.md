---
register_id: DOC-F-SRC-RUNTIME-DIAGNOSTIC
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
title: DualFrontier.Runtime.Diagnostic — module doc
last_modified_commit: b7cfea0
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime.Diagnostic

**Purpose:** Performance + debug tooling — validation log capture (V0.A), FPS measurement
+ debug overlay (V0.C).

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Diagnostic module.

**Dependencies:** `System` (BCL) only at V0.A. V0.C extensions reach Sprite/Text для overlay.

## V0.A surface

- `ValidationLog` — thread-safe ring buffer для Vulkan validation messages. Consumed by
  `ValidationLayer.DebugCallback` (Graphics module Commit 7). 1024 message capacity;
  oldest evicted on overflow.
- `ValidationMessage` — readonly record struct (Severity + Message + TimestampUtc).
- `ValidationSeverity` — Info / Warning / Error.

## V0.C surface (deferred)

- `FrameTimer` — FPS measurement (ema-smoothed)
- `DebugOverlay` — FPS/tick/queue overlay (consumes Sprite + Text)
