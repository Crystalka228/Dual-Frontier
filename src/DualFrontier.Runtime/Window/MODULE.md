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

**Purpose:** Platform-neutral window abstraction with one backend per windowing system.
Lifecycle (create/show/hide/destroy), event pump, and Vulkan surface creation. Hides every
OS P/Invoke detail behind `IWindow`.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Window module.

**Dependencies:** `Native.Win32` (Windows backend), `Native.Xcb` (Linux backend),
`Native.Vulkan` (surface creation), `Input` (event types + key mapping).

## Public API

- `IWindow` — interface (dimensions, IsOpen, Show/Hide/PumpMessages, CreateVulkanSurface)
- `PlatformWindow` — factory; picks the backend for the running host
- `Win32Window` — Windows backend (`user32`/`kernel32`, WndProc message pump)
- `XcbWindow` — Linux backend (`libxcb`, poll loop; runs over XWayland on a Wayland session)
- `WindowEventDecode` — pure resize decode shared by both backends
- `WindowOptions` — record с Title, Width, Height, Resizable
- `InputEventQueue` — `ConcurrentQueue<IInputEvent>` (drained and discarded by the Launcher today)

## The seam (LINUX_PRESENT_1, 2026-08-26)

`IWindow` carries **no platform-tagged handle**. A Vulkan surface needs different inputs per
window system — Win32 an HINSTANCE plus an HWND, XCB a connection pointer plus a window id —
so a single `IntPtr Handle` could only ever carry one of them and every new backend would have
had to widen the contract. Surface creation therefore lives BEHIND the window
(`IWindow.CreateVulkanSurface`), where the implementation already holds whatever its platform
needs; `VulkanSurface` keeps ownership of the resulting handle and its `vkDestroySurfaceKHR`.

The instance-level counterpart is in `Graphics/VulkanInstance.cs`, which selects
`VK_KHR_win32_surface` or `VK_KHR_xcb_surface` by the same platform predicate the factory uses.
This is design intent recorded here, not an analyzer-enforced rule.

## Scope

A window can:
- Open + show + hide + destroy a native window on Windows or Linux
- Pump events once per frame (Win32: PeekMessage/TranslateMessage/DispatchMessage;
  XCB: `xcb_poll_for_event` drain) — the sole writer of Width/Height/IsOpen in steady state
- Handle close cleanly (Win32 WM_CLOSE/WM_DESTROY; XCB WM_DELETE_WINDOW client message)
- Create a VkSurfaceKHR for itself
- Carry an InputEventQueue handle (placeholder — input event enqueue deferred к V0.C
  when WM_KEYDOWN/WM_MOUSEMOVE/etc. handlers added)

Out of scope V0.A: input event enqueue (V0.C), resize handling (V0.B alongside swapchain
recreation), focus event coupling (V0.C alongside SetPaused integration).

## Marshalling discipline

**Win32.** `WindowProc` instance method wrapped в delegate; `GCHandle.Alloc` pins delegate
during window lifetime to prevent GC collection of the function pointer thunk that Win32 holds.

**XCB.** No callback and therefore no pinning — the X protocol is a poll loop. Each event
buffer `xcb_poll_for_event` returns is malloc'd and freed by the caller. Both backends carry
the same partial-construction rollback discipline (F08): a throwing constructor never reaches
`Dispose`, so every resource acquired so far is released on the way out, with the connection
or class registration released last.
