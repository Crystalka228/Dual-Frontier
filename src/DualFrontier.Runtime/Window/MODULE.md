---
register_id: DOC-F-SRC-RUNTIME-WINDOW
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
title: DualFrontier.Runtime.Window — module doc
last_modified_commit: b2ba32d
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime.Window

**Purpose:** High-level Win32 window abstraction. Hides Win32 P/Invoke details. Lifecycle
(create/show/hide/destroy), message pump.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Window module.

**Dependencies:** `Native.Win32`, `Input` (forward reference; V0.A surface
takes InputEventQueue placeholder, V0.C wires input events).

## Public API

- `IWindow` — interface (Handle, dimensions, IsOpen, Show/Hide/PumpMessages)
- `Window` — Win32 implementation
- `WindowOptions` — record с Title, Width, Height, Resizable
- `InputEventQueue` — `ConcurrentQueue<IInputEvent>` placeholder (consumed V0.C)

## V0.A scope

Window can:
- Open + show + hide + destroy a Win32 window
- Pump messages (PeekMessage / TranslateMessage / DispatchMessage loop)
- Handle WM_CLOSE / WM_DESTROY cleanly (sets IsOpen = false)
- Carry an InputEventQueue handle (placeholder — input event enqueue deferred к V0.C
  when WM_KEYDOWN/WM_MOUSEMOVE/etc. handlers added)

Out of scope V0.A: input event enqueue (V0.C), resize handling (V0.B alongside swapchain
recreation), focus event coupling (V0.C alongside SetPaused integration).

## Marshalling discipline

`WindowProc` instance method wrapped в delegate; `GCHandle.Alloc` pins delegate during
window lifetime to prevent GC collection of the function pointer thunk that Win32 holds.
