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

**Purpose:** Pure P/Invoke to Win32 API. `[LibraryImport]` (source-generated marshalling) +
selective `[DllImport]` (legacy runtime marshalling) declarations for window management,
message pump, input handling.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.5 Win32 template.

**Dependencies:** `System` (BCL), `System.Runtime.InteropServices`.

## Files

- `Win32Api.cs` — P/Invoke function declarations (user32.dll + kernel32.dll)
- `Win32Constants.cs` — WM_*, WS_*, CS_*, IDC_*, IDI_*, SW_*, PM_* constants
- `Win32Structs.cs` — WNDCLASSEX, MSG, POINT, RECT struct layouts
- `WindowProc.cs` — window procedure callback delegate

## Marshalling note

`RegisterClassEx` uses `[DllImport]` because `WNDCLASSEX` struct contains string fields
(`lpszClassName`, `lpszMenuName`) which the `[LibraryImport]` source generator cannot
marshal in-struct. All other functions с simple parameters use `[LibraryImport]` with
`StringMarshalling.Utf16` for direct string params.

## V0.A surface (14 functions)

- Lifecycle: `GetModuleHandle`, `RegisterClassEx`, `UnregisterClass`, `CreateWindowEx`,
  `DestroyWindow`, `ShowWindow`
- Message pump: `PeekMessage`, `TranslateMessage`, `DispatchMessage`, `DefWindowProc`,
  `PostQuitMessage`
- Resource loading: `LoadCursor`, `LoadIcon`
- Diagnostic: `GetLastError`
