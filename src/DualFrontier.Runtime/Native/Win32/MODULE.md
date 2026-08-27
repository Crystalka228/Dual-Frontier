---
register_id: DOC-F-SRC-RUNTIME-NATIVE-WIN32
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
title: DualFrontier.Runtime.Native.Win32 — module doc
last_modified_commit: 0cc72ca
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime.Native.Win32

**Purpose:** Pure P/Invoke to the Win32 API — the WINDOWS windowing backend's OS surface.
`[LibraryImport]` (source-generated marshalling) + selective `[DllImport]` (legacy runtime
marshalling) declarations for window management, message pump, input handling. The Linux
backend's equivalent leaf is [`Native/Xcb`](../Xcb/MODULE.md); neither references the other,
and the platform choice is made once in `Window/PlatformWindow.cs`.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.5 Win32 template.

**Dependencies:** `System` (BCL), `System.Runtime.InteropServices`.

## Files

- `Win32Api.cs` — P/Invoke function declarations (user32.dll + kernel32.dll)
- `Win32Constants.cs` — WM_*, VK_*, WS_*, CS_*, IDC_*, IDI_*, SW_*, PM_* constants
  (the `VK_*` virtual-key family is consumed by `Input/VirtualKeyMapper.cs`, not by the window)
- `Win32Structs.cs` — WNDCLASSEX, MSG, POINT struct layouts
- `WindowProc.cs` — window procedure callback delegate

## Marshalling note

`RegisterClassEx` uses `[DllImport]` because `WNDCLASSEX` struct contains string fields
(`lpszClassName`, `lpszMenuName`) which the `[LibraryImport]` source generator cannot
marshal in-struct. All other functions с simple parameters use `[LibraryImport]` with
`StringMarshalling.Utf16` for direct string params.

## Surface — 14 P/Invokes

Measured at LINUX_PRESENT_1 (2026-08-26): **14 declarations = 13 `[LibraryImport]` + 1
`[DllImport]`**, split 12 `user32.dll` + 2 `kernel32.dll`.

- Lifecycle: `GetModuleHandle`, `RegisterClassEx`, `UnregisterClass`, `CreateWindowEx`,
  `DestroyWindow`, `ShowWindow`
- Message pump: `PeekMessage`, `TranslateMessage`, `DispatchMessage`, `DefWindowProc`,
  `PostQuitMessage`
- Resource loading: `LoadCursor`, `LoadIcon`
- Diagnostic: `GetLastError`

`SendMessageW` was retired at LINUX_PRESENT_1 (15 → 14). It had zero production callers and
existed only so three tests could inject a synthetic `WM_SIZE` into the window procedure;
there is no portable analogue of calling a window procedure directly, so the resize decode was
extracted as a pure function (`Window/WindowEventDecode.cs`) and those tests now run ungated
on every platform. `RECT` (zero references) and `WM_CREATE` (never switched on) were trimmed
in the same pass.
