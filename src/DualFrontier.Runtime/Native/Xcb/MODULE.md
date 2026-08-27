---
register_id: DOC-F-SRC-RUNTIME-NATIVE-XCB
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-08-26
last_modified: 2026-08-26
content_language: mixed
next_review_due: 2026-Q4
title: DualFrontier.Runtime.Native.Xcb — module doc
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Module doc for the Linux windowing leaf authored at LINUX_PRESENT_1 (2026-08-26), mirroring the enrolled DOC-F-SRC-RUNTIME-NATIVE-WIN32 form for its sibling backend (enroll F/4 per the CORPUS_CLOSURE_INVERSION_B CD2 orphan-triage convention).
---

# DualFrontier.Runtime.Native.Xcb

**Purpose:** Pure P/Invoke to libxcb and libxkbcommon — the LINUX windowing backend's OS
surface. The Windows equivalent leaf is [`Native/Win32`](../Win32/MODULE.md); neither
references the other, and the platform choice is made once in `Window/PlatformWindow.cs`.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../../docs/architecture/VULKAN_SUBSTRATE.md)
§2.2 Window and input truth.

**Dependencies:** `System` (BCL), `System.Runtime.InteropServices`.

## Files

- `XcbApi.cs` — libxcb P/Invokes + libc `free`
- `XkbApi.cs` — libxkbcommon + libxkbcommon-x11 P/Invokes
- `XcbConstants.cs` — event codes, event masks, CW/config value bits, predefined atoms
- `XcbStructs.cs` — X11 wire layouts (screen, iterator, cookies, reply, event structs)

## Surface — 31 P/Invokes

Measured at LINUX_PRESENT_1 (2026-08-26):

| Library | Count | What for |
|---|---|---|
| `libxcb.so.1` | 18 | connection, setup/screen, window lifecycle, atoms, properties, events |
| `libc.so.6` | 1 | `free` — libxcb hands back malloc'd event and reply buffers |
| `libxkbcommon.so.0` | 8 | context/keymap/state lifetime, keysym lookup, modifier state, layout + level-0 identity |
| `libxkbcommon-x11.so.0` | 4 | XKB extension setup, core keyboard device, keymap/state from device |

## Deliberately core libxcb only

`libxcb-icccm` and `libxcb-keysyms` are NOT used, and are not installed on the reference host.
Neither is needed: `xcb_intern_atom` covers the WM_PROTOCOLS / WM_DELETE_WINDOW close contract
that icccm would wrap, and libxkbcommon-x11 is the modern keycode path — strictly better than
xcb-keysyms, because it tracks modifier state across press/release instead of making the caller
re-derive it. Choosing the helper libraries would have added an install-time dependency the
host does not satisfy.

## Marshalling notes

X11 wire layouts are fixed by the protocol, so the byte offsets in `XcbStructs.cs` are stable
across libxcb versions. Every event arrives in a 32-byte buffer; the smaller event structs
simply leave the tail unread. `xcb_generic_event_t.response_type` carries a high bit flagging a
`SendEvent`-synthesised event — WM_DELETE_WINDOW arrives that way — so it MUST be masked with
`XCB_RESPONSE_TYPE_MASK` before comparing.

Cookie types (`xcb_void_cookie_t`, `xcb_intern_atom_cookie_t`) are single-`uint` structs passed
and returned by value; they are declared as distinct types so the two cannot be swapped.

## Thread contract

The `xcb_connection_t` is used only from the thread that created it — the same law the Win32
backend's HWND imposes. libxcb is itself thread-safe; the backend's window state is not.

## Analyzer boundary

Every declaration here sits under `DualFrontier.Runtime.Native.Xcb`, a namespace rooted in a
sanctioned interop surface. DFK002 (Error) rejects any P/Invoke outside those roots, and DFK001
(Error) rejects dynamic native binding outside them.
