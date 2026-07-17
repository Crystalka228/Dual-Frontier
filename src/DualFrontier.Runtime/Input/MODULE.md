---
register_id: DOC-F-SRC-RUNTIME-INPUT
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
title: DualFrontier.Runtime.Input — module doc
last_modified_commit: b2ba32d
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime.Input

**Purpose:** Typed input events. V0.A scope is marker interface only — concrete event types
(`KeyPressedEvent`, `MouseMovedEvent`, etc.) и actual Win32 message → typed event translation
land V0.C.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Input module.

**Dependencies:** `System` (BCL).

## V0.A surface

- `IInputEvent` — marker interface для future event types

## V0.C surface (deferred)

- Concrete events: `KeyPressedEvent`, `KeyReleasedEvent`, `MouseMovedEvent`,
  `MouseButtonEvent`, `MouseWheelEvent`, `WindowResizedEvent`, `WindowFocusEvent`
- Enums: `Key`, `MouseButton`
- Window.cs WindowProcedure dispatch: WM_KEYDOWN → KeyPressedEvent.Enqueue(InputEventQueue)
