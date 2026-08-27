---
register_id: DOC-E-LINUX_PRESENT_1_RECON_REPORT
project: Dual Frontier
category: E
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-08-26'
last_modified: '2026-08-26'
content_language: en
next_review_due: null
review_cadence: none-historical-record
title: 'LINUX_PRESENT_1 RECON REPORT — 2026-08-26 (A-H) — Linux presentation-path measurement at HEAD 683b343 by a read-only Opus subagent: FOUR blockers firing in order (kernel32 GetModuleHandle at Window.cs:46 PLUS an independent second site VulkanSurface.cs:25; VkApi.VulkanLib hard-coded "vulkan-1.dll" feeding ~100 LibraryImports with NO SetDllImportResolver anywhere while the host loader is libvulkan.so.1; VK_KHR_win32_surface asserted unconditionally at VulkanInstance.cs:51 with no enumeration; the Launcher csproj never received the Linux .so copy arm the 8 test projects got at e30375c); coupling set is FIVE production files (Native/Win32 leaf, Window.cs, VulkanSurface.cs, VulkanInstance.cs, Input/VirtualKeyMapper.cs — the last pinned by the UNGATED VirtualKeyMapperTests green on Linux today); production consumes exactly 6 IWindow members and Handle has ONE reader; device/queue selection is surface-independent; resize truth = WindowResizeEvent is decorative, Width/Height are the load-bearing pump outputs read by the out-of-date recreate path, and the currentExtent clamp would mask a missing update; the 111-skip gate is a pure IsOSPlatform(Windows) check splitting 71 loader-only / 40 display-needing tests; SendMessage P/Invoke is test-only and non-portable; DFK002 (Error) requires new P/Invokes under DualFrontier.Runtime.Native.*; host session is Wayland + XWayland + DISPLAY=:0 with libxcb.so.1 and libxkbcommon-x11 present and libxcb-keysyms/icccm ABSENT; dead surface enumerated (RECT, WM_CREATE, WindowOptions.Resizable, IWindow.Hide); doc drift (MODULE.md 14 vs VULKAN_SUBSTRATE 15 LibraryImports vs actual 14 LI + 1 DllImport)'
special_case_rationale: 'Durable-report recon enrolled DOC-E Tier 3 per the docs/reports/ convention (precedents: DOC-E-F60A_TICK_PATH_RECON_REPORT, DOC-E-F60A_TICK_PATH_VERIFICATION_RECON). Operator-requested subagent recon (2026-08-26) for the LINUX_PRESENT_1 charter, executed by a fresh read-only Opus subagent at HEAD 683b343; complements the architect-seat scope measurement of 2026-08-26 (the Launcher DllNotFoundException repro, recorded in the ledger and the charter memory). Read-only: zero repository mutations besides this file, sync never run, zero builds/tests by the subagent. UNTRACKED at authoring — enrolls at the charter cascade C1.'
---

# LINUX_PRESENT_1 — READ-ONLY RECON REPORT

**HEAD verified:** `683b343d03974f952e304ae4a18c692f1b1f13de` — matches CONTEXT (`683b343`). Working tree at recon time: only `.claude/settings.local.json` modified (see DISCREPANCIES for the drift vs the session-start snapshot).

**Discipline observed:** reads + non-mutating commands only. No edits, no builds, no `dotnet`, no `cmake`, no git state changes.

---

## A. WIN32 COUPLING CENSUS (complete)

### A1. `src/DualFrontier.Runtime/Native/Win32/` — every P/Invoke, constant, struct

Directory contents, `wc -l`:

| File | Lines |
|---|---|
| `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Api.cs` | 72 |
| `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Constants.cs` | 83 |
| `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Structs.cs` | 50 |
| `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/WindowProc.cs` | 6 |
| `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/MODULE.md` | 52 |

**Count command + result:**
```
grep -c "user32.dll" .../Win32Api.cs   → 13
grep -c "kernel32.dll" .../Win32Api.cs → 2
```
Total **15 P/Invoke declarations** (13 user32 + 2 kernel32). Of these **14 are `[LibraryImport]`** and **1 is `[DllImport]`** (`RegisterClassEx`, line 9 — the raw `grep -c` values of 15/2 each include one comment line at `Win32Api.cs:7` that names both attributes).

Full enumeration, `Win32Api.cs`:

| Line | Attribute | DLL | Signature | Used FOR |
|---|---|---|---|---|
| 9-10 | `[DllImport]` `RegisterClassExW`, `CharSet.Unicode`, `SetLastError` | user32 | `ushort RegisterClassEx(in WNDCLASSEX lpwcx)` | window-class registration (`Window.cs:81`) |
| 12-14 | `[LibraryImport]` `UnregisterClassW`, Utf16 | user32 | `int UnregisterClass(string, IntPtr)` | class teardown (`Window.cs:130`, `:292`) |
| 16-30 | `[LibraryImport]` `CreateWindowExW`, Utf16 | user32 | `IntPtr CreateWindowEx(uint,string,string,uint,int,int,int,int,IntPtr,IntPtr,IntPtr,IntPtr)` | HWND creation (`Window.cs:87`) |
| 32-34 | `[LibraryImport]` `DestroyWindow` | user32 | `bool DestroyWindow(IntPtr)` | `Window.cs:125`, `:147`, `:287` |
| 36-38 | `[LibraryImport]` `ShowWindow` | user32 | `bool ShowWindow(IntPtr,int)` | `Show()`/`Hide()` (`Window.cs:265`, `:267`) |
| 40-42 | `[LibraryImport]` `PeekMessageW` | user32 | `bool PeekMessage(out MSG,IntPtr,uint,uint,uint)` | pump (`Window.cs:271`) |
| 44-46 | `[LibraryImport]` `TranslateMessage` | user32 | `bool TranslateMessage(in MSG)` | pump (`Window.cs:278`) |
| 48-49 | `[LibraryImport]` `DispatchMessageW` | user32 | `IntPtr DispatchMessage(in MSG)` | pump (`Window.cs:279`) |
| 51-52 | `[LibraryImport]` `DefWindowProcW` | user32 | `IntPtr DefWindowProc(IntPtr,uint,IntPtr,IntPtr)` | default handling (`Window.cs:166,202,212,261`) |
| 54-55 | `[LibraryImport]` `SendMessageW` | user32 | `IntPtr SendMessage(IntPtr,uint,IntPtr,IntPtr)` | **TEST-ONLY** — sole callers `WindowResizeEventTests.cs:30,49,64`. Zero production call sites. |
| 57-58 | `[LibraryImport]` `PostQuitMessage` | user32 | `void PostQuitMessage(int)` | `Window.cs:150` (WM_DESTROY) |
| 60-61 | `[LibraryImport]` `LoadCursorW` | user32 | `IntPtr LoadCursor(IntPtr,IntPtr)` | `Window.cs:74` (WNDCLASSEX.hCursor) |
| 63-64 | `[LibraryImport]` `LoadIconW` | user32 | `IntPtr LoadIcon(IntPtr,IntPtr)` | `Window.cs:73` (WNDCLASSEX.hIcon) |
| 66-68 | `[LibraryImport]` `GetModuleHandleW`, `SetLastError`, Utf16 | **kernel32** | `IntPtr GetModuleHandle(string?)` | **THE DEATH SITE** — `Window.cs:46` (first call in `InitializeWin32`) AND `VulkanSurface.cs:25` |
| 70-71 | `[LibraryImport]` `GetLastError` | **kernel32** | `uint GetLastError()` | error text in 3 throw sites |

**Call-site distribution of `Win32Api.` (command: `grep -rn "Win32Api\." src/ tests/ --include="*.cs" \| cut -d: -f1 \| sort \| uniq -c`):**
```
23  src/DualFrontier.Runtime/Window/Window.cs
 2  src/DualFrontier.Runtime/Graphics/VulkanSurface.cs
 3  tests/DualFrontier.Runtime.Tests/Window/WindowResizeEventTests.cs
```
Only **two** production files touch `Win32Api` at all.

**`Win32Constants.cs` — constant families (5):**
- Window messages, 19 members: `WM_CREATE 0x0001`, `WM_DESTROY 0x0002`, `WM_SIZE 0x0005`, `WM_SETFOCUS 0x0007`, `WM_KILLFOCUS 0x0008`, `WM_CLOSE 0x0010`, `WM_QUIT 0x0012`, `WM_KEYDOWN/UP 0x0100/0101`, `WM_SYSKEYDOWN/UP 0x0104/0105`, `WM_MOUSEMOVE 0x0200`, `WM_{L,R,M}BUTTON{DOWN,UP}`, `WM_MOUSEWHEEL 0x020A` (`:6-24`). `WM_CREATE` is declared but never switched on.
- Virtual key codes, 29 members `VK_BACK`..`VK_F12` (`:27-55`) — **consumed by `Input/VirtualKeyMapper.cs`, not by `Window.cs`.**
- `WHEEL_DELTA = 120` (`:58`).
- Window styles `WS_OVERLAPPEDWINDOW 0x00CF0000`, `WS_VISIBLE` (`:61-62`); class styles `CS_OWNDC|CS_HREDRAW|CS_VREDRAW` (`:65-67`).
- Resource/show/peek: `IDC_ARROW`, `IDI_APPLICATION`, `SW_HIDE 0`, `SW_SHOW 5`, `PM_NOREMOVE`, `PM_REMOVE 1`, `CW_USEDEFAULT` (`:70-82`).

**`Win32Structs.cs` — 4 structs:** `WNDCLASSEX` (`:6-22`, `LayoutKind.Sequential`, `CharSet.Unicode`, two `[MarshalAs(LPWStr)]` string fields — this is exactly why `RegisterClassEx` cannot use `[LibraryImport]`), `MSG` (`:25-34`), `POINT` (`:37-41`), `RECT` (`:44-50`). **`RECT` has zero references anywhere** — dead.

**`WindowProc.cs`:** one delegate — `[UnmanagedFunctionPointer(CallingConvention.StdCall)] internal delegate IntPtr WindowProc(IntPtr,uint,IntPtr,IntPtr)` (`:5-6`). `StdCall` is a Windows-only calling convention token; the seam replacement has no analogue (xcb is a poll loop, not a callback).

### A2. `Window.cs` (302 lines) — full anatomy

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/Window.cs`, `public sealed class Window : IWindow` (`:13`).

**State:** `_options`, `_hwnd`, `_hinstance`, `_className`, `_isOpen`, `_wndProcHandle` (`GCHandle`), `_wndProc`, `_currentWidth`, `_currentHeight` (`:15-23`). `InputQueue` is `internal` (`:30`) — not on `IWindow`.

**Construction sequence** (`:32-42` → `:44-114`):
1. ctor null-guards options + queue, seeds `_currentWidth/_currentHeight` from `WindowOptions`, stores the queue, calls `InitializeWin32()`.
2. `InitializeWin32` line 46: **`Win32Api.GetModuleHandle(null)` — the very first native call, into `kernel32.dll`. This is where the Launcher dies on Linux.** Zero-check throws `InvalidOperationException` (`:47-51`).
3. `_className = $"DualFrontierWindow_{Guid.NewGuid():N}"` (`:53`) — a **per-instance unique class name**, so repeated Window construction never collides. Any seam replacement should preserve this property.
4. `_wndProc = WindowProcedure; _wndProcHandle = GCHandle.Alloc(_wndProc)` (`:56-57`) — delegate pinned so Win32's function pointer stays valid. Explicit F08 comment at `:59-62` explains that everything after the `GCHandle.Alloc` is inside `try`/`catch → RollbackWin32Initialization()` because a throwing ctor never reaches `Dispose`.
5. `WNDCLASSEX` populated (`:65-79`): `CS_OWNDC|CS_HREDRAW|CS_VREDRAW`, `Marshal.GetFunctionPointerForDelegate(_wndProc)`, `LoadIcon`/`LoadCursor`, `hbrBackground = IntPtr.Zero`, `lpszClassName = _className`.
6. `RegisterClassEx` → 0 throws (`:81-85`).
7. `CreateWindowEx` with `dwStyle: WS_OVERLAPPEDWINDOW`, `X/Y: CW_USEDEFAULT`, `nWidth/nHeight: _options.Width/Height` (`:87-99`); zero HWND throws (`:101-105`).
8. `_isOpen = true` (`:107`).

**`RollbackWin32Initialization`** (`:121-139`): DestroyWindow → UnregisterClass → free `GCHandle` → null `_wndProc` → `_isOpen = false`. Byte-for-byte the same body as `Dispose` (`:283-301`) — the two are duplicated, not shared.

**Message pump** (`PumpMessages`, `:269-281`):
```csharp
while (Win32Api.PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, Win32Constants.PM_REMOVE))
{
    if (msg.message == Win32Constants.WM_QUIT) { _isOpen = false; break; }
    Win32Api.TranslateMessage(in msg);
    Win32Api.DispatchMessage(in msg);
}
```
Note `hWnd: IntPtr.Zero` — it drains **all messages for the calling thread**, not just this window's. Non-blocking drain-until-empty, called once per frame.

**`WindowProcedure` (`:141-263`) — every handled message:**

| Message | Action | Return |
|---|---|---|
| `WM_CLOSE` `:145-148` | `_isOpen = false`; `DestroyWindow(hWnd)` | `IntPtr.Zero` |
| `WM_DESTROY` `:149-152` | `PostQuitMessage(0)`; `_isOpen = false` | `IntPtr.Zero` |
| `WM_SIZE` `:153-166` | unpack LOWORD/HIWORD of lParam; **skip if 0×0 (minimize) or unchanged**; else set `_currentWidth/_currentHeight` and enqueue `WindowResizeEvent(w,h)` | `DefWindowProc` |
| `WM_KEYDOWN` `:169-178` | `VirtualKeyMapper.Map(wParam)`; if != `Key.Unknown` enqueue `KeyPressedEvent(key)` | `IntPtr.Zero` |
| `WM_KEYUP` `:179-188` | same → `KeyReleasedEvent(key)` | `IntPtr.Zero` |
| `WM_SYSKEYDOWN` `:194-203` | enqueue `KeyPressedEvent` **then fall through** | `DefWindowProc` (F05 comment `:190-193`: swallowing broke Alt+F4 / Alt+Space / F10) |
| `WM_SYSKEYUP` `:204-213` | enqueue `KeyReleasedEvent`, fall through | `DefWindowProc` |
| `WM_MOUSEMOVE` `:216-224` | signed short unpack of lParam → `MouseMovedEvent(x,y)` | `IntPtr.Zero` |
| `WM_LBUTTONDOWN/UP` `:225-230` | `MouseButtonEvent(Left, true/false)` | `IntPtr.Zero` |
| `WM_RBUTTONDOWN/UP` `:231-236` | `MouseButtonEvent(Right, …)` | `IntPtr.Zero` |
| `WM_MBUTTONDOWN/UP` `:237-242` | `MouseButtonEvent(Middle, …)` | `IntPtr.Zero` |
| `WM_MOUSEWHEEL` `:243-250` | signed HIWORD of wParam ÷ `WHEEL_DELTA` → `MouseWheelEvent(delta)` | `IntPtr.Zero` |
| `WM_SETFOCUS` `:253-255` | `WindowFocusEvent(true)` | `IntPtr.Zero` |
| `WM_KILLFOCUS` `:256-258` | `WindowFocusEvent(false)` | `IntPtr.Zero` |
| default `:260-261` | — | `DefWindowProc` |

**Published event types (7 total):** `WindowResizeEvent`, `KeyPressedEvent`, `KeyReleasedEvent`, `MouseMovedEvent`, `MouseButtonEvent`, `MouseWheelEvent`, `WindowFocusEvent`.

**Width/Height:** updated ONLY in the `WM_SIZE` arm (`:162-163`). No other writer.

**`IsOpen` transitions:** `true` at `:107` (successful init). `false` at `:138` (rollback), `:146` (WM_CLOSE), `:151` (WM_DESTROY), `:275` (WM_QUIT seen in pump), `:300` (Dispose). Note WM_CLOSE→DestroyWindow→WM_DESTROY→PostQuitMessage→WM_QUIT gives **three redundant paths** to the same flag.

**Show/Hide:** one-liners `:265`/`:267` calling `ShowWindow(SW_SHOW/SW_HIDE)`. **`Hide()` has zero callers repo-wide.**

**Thread affinity:** none enforced in code. `IWindow.cs:22` documents "Call once per frame from message pump thread"; the `GCHandle` pin is the only marshalling ceremony. Win32 requires the pump run on the thread that created the HWND — undeclared and unasserted in code.

### A3. `IWindow` contract + every consumer

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/IWindow.cs` (24 lines): `public interface IWindow : IDisposable` with 7 members — `IntPtr Handle` (`:10`, doc'd "Win32 HWND for surface creation (vkCreateWin32SurfaceKHR)"), `int Width` (`:12`), `int Height` (`:13`), `bool IsOpen` (`:16`), `void Show()` (`:18`), `void Hide()` (`:19`), `void PumpMessages()` (`:23`).

`WindowOptions` is at `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/WindowOptions.cs` (13 lines): `sealed record` with `Title = "Dual Frontier"`, `Width = 1280`, `Height = 720`, `Resizable = true`. **`Resizable` is never read by `Window.cs`** — `dwStyle` is hard-coded `WS_OVERLAPPEDWINDOW` (`:91`). Dead property.

Consumer sites, member by member:

**`Handle`** — 1 production, 4 test:
- `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:38` — `hwnd = window.Handle`
- `tests/DualFrontier.Runtime.Tests/RuntimeCompositionTests.cs:17`
- `tests/DualFrontier.Runtime.Tests/Window/WindowResizeEventTests.cs:30, 49, 64`

**`PumpMessages`** — 1 production, 5 smoke:
- `src/DualFrontier.Launcher/Program.cs:82`
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs:68, 130, 444, 574` (+ scene loops)

**`IsOpen`** — 4 production, 1 test, 9 smoke:
- `src/DualFrontier.Launcher/Program.cs:75` (main loop gate)
- `src/DualFrontier.Launcher/LauncherRenderer.cs:135, 205, 239`
- `tests/DualFrontier.Runtime.Tests/RuntimeCompositionTests.cs:50`
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs:66, 128, 156, 442, 449, 572, 596, 742, 746, 847, 851`

**`Width`/`Height`** — 2 production, 6 smoke (all feeding `Swapchain.Recreate`):
- `src/DualFrontier.Launcher/LauncherRenderer.cs:137, 207`
- `tests/DualFrontier.Runtime.SmokeTest/Program.cs:151, 166, 195, 455, 480, 602, 625`
- (`Runtime.cs:115` and `:176` read `options.Window.Width/Height` — the **options record**, not `IWindow`.)

**`Show()`** — `src/DualFrontier.Launcher/Program.cs:65`; `tests/DualFrontier.Runtime.SmokeTest/Program.cs:61`.

**`Hide()`** — **zero consumers.**

**`Dispose()`** — `src/DualFrontier.Runtime/Runtime.cs:488` (last in teardown).

Design consequence: **the `IWindow` surface consumed in production is exactly `Handle`, `Width`, `Height`, `IsOpen`, `Show`, `PumpMessages`, `Dispose`** — six live members plus Dispose. `Hide` is free to keep or drop. `Handle` is the only member whose *meaning* is platform-bound, and it has exactly one production reader.

### A4. Full residual census

**Command:** `grep -rni "win32\|kernel32\|user32\|hwnd\|hinstance\|GetModuleHandle" src/ --include="*.cs" | wc -l` → **170 hits across 18 files**.

Hits **outside** `Native/Win32/` and `Window/Window.cs`, classified:

**CODE (must change):**
| Site | Nature |
|---|---|
| `src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs:14` | `VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface"` |
| `src/DualFrontier.Runtime/Graphics/VulkanInstance.cs:51` | extension list adds the above **unconditionally** |
| `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:2` | `using DualFrontier.Runtime.Native.Win32;` |
| `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:25, 29` | `Win32Api.GetModuleHandle(null)` + `GetLastError()` — **a second kernel32 death site independent of Window.cs** |
| `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:32-41` | `VkWin32SurfaceCreateInfoKHR` + `vkCreateWin32SurfaceKHR` |
| `src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs:159-162` | `[LibraryImport] vkCreateWin32SurfaceKHR` |
| `src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs:86` | `VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR = 1000009000` |
| `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs:133-139` | `VkWin32SurfaceCreateInfoKHR` layout (`hinstance`, `hwnd`) |
| **`src/DualFrontier.Runtime/Input/VirtualKeyMapper.cs:1, 14-42`** | **`using …Native.Win32;` + 29 `Win32Constants.VK_*` switch arms — a 4th coupled production file the CONTEXT did not name** |

**COMMENT / DOC ONLY (no behaviour):**
`src/DualFrontier.Application/Input/IInputSource.cs:6`; `src/DualFrontier.Application/Display/IntentOverlayLayer.cs:21`; `src/DualFrontier.Runtime/Input/IInputEvent.cs:4`; `src/DualFrontier.Runtime/Input/Key.cs:5`; `src/DualFrontier.Runtime/Input/MouseWheelEvent.cs:5`; `src/DualFrontier.Runtime/Input/VirtualKeyMapper.cs:6`; `src/DualFrontier.Runtime/Window/IWindow.cs:9, 21`; `src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs:125-131`; `src/DualFrontier.Runtime/Native/Win32/Win32Constants.cs:5, 60`; `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:8`; `src/DualFrontier.Runtime/Graphics/MODULE.md:41`.

**Verdict on scope:** the CONTEXT's "15 P/Invokes in one file + surface" is accurate as far as it goes, but the true code-change set is **five production files, not three**: `Win32Api/Constants/Structs/WindowProc` (the Native/Win32 leaf), `Window.cs`, `VulkanSurface.cs`, `VulkanInstance.cs`, and **`Input/VirtualKeyMapper.cs`**.

---

## B. VULKAN SURFACE + INSTANCE + DEVICE COUPLING

### B1. `VulkanSurface.cs` (63 lines)

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanSurface.cs`, `public sealed class VulkanSurface : IDisposable`.

Ctor `(VulkanInstance instance, IWindow window)` (`:19`):
- `:25` `IntPtr hinstance = Win32Api.GetModuleHandle(null);` — throws `InvalidOperationException` on zero (`:26-30`). **This is the second kernel32 dependency, structurally independent of `Window.cs`.**
- `:32-39` builds `VkWin32SurfaceCreateInfoKHR { sType = VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR, pNext = 0, flags = 0, hinstance, hwnd = window.Handle }`.
- `:41` `VkApi.vkCreateWin32SurfaceKHR(_instance, in createInfo, IntPtr.Zero, out _surface)`; non-`VK_SUCCESS` throws naming the extension (`:43-47`).
- `Dispose` (`:50-62`) → `vkDestroySurfaceKHR` — **platform-neutral already**.

**What it needs from `IWindow`: exactly `window.Handle` (one IntPtr).** Everything else is process-scoped. An XCB surface needs `(xcb_connection_t*, xcb_window_t)` — two values — so the seam must widen `Handle` into a platform-tagged pair, or move surface creation behind the window implementation (the window knows both). The latter keeps `IWindow` narrower.

`VkApi.cs:153` carries a comment already flagging the assumption: "KHR functions exported by vulkan-1.dll loader trampoline **on Windows**".

### B2. `VulkanInstance.cs` (148 lines) — extension construction

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanInstance.cs`.

```
:48-52   var extensionNames = new List<string> {
             VkConstants.VK_KHR_SURFACE_EXTENSION_NAME,
             VkConstants.VK_KHR_WIN32_SURFACE_EXTENSION_NAME,   // ← hard-coded, unconditional
         };
:53      var layerNames = new List<string>();
:55-59   if (ValidationLayerEnabled) { ext += VK_EXT_debug_utils; layers += VK_LAYER_KHRONOS_validation; }
```

**No availability query.** There is no `vkEnumerateInstanceExtensionProperties` call anywhere in the file — the list is asserted, not negotiated. On Linux the loader will return `VK_ERROR_EXTENSION_NOT_PRESENT` from `vkCreateInstance` (`:101-109`). So even with windowing fixed, instance creation is the **second** hard failure.

Marshalling (`:61-73`, `:86-99`, `:114-132`): `Marshal.StringToCoTaskMemUTF8` per string into `IntPtr[]`, `fixed`-pinned into `byte**`, freed in `finally`. Adding a platform-selected surface extension is a one-line list change inside this existing pattern — no marshalling rework.

Version gate `VerifyVulkanApiVersion` (`:26-41`): requires ≥ `VK_API_VERSION_1_3`. Host reports 1.4.335, passes.

`VkConstants` extension names live at `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs:11-15` — `VK_LAYER_KHRONOS_VALIDATION`, `VK_EXT_DEBUG_UTILS_EXTENSION_NAME`, `VK_KHR_SURFACE_EXTENSION_NAME`, **`VK_KHR_WIN32_SURFACE_EXTENSION_NAME` (`:14`)**, `VK_KHR_SWAPCHAIN_EXTENSION_NAME`. `VK_KHR_xcb_surface` / `VK_KHR_wayland_surface` are absent — the cascade adds them here.

### B3. Device / queue selection + present support

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` (322 lines), read in full.

Ctor `:47-54`: `EnumeratePhysicalDevices()` → `SelectPhysicalDevice()` → `CreateLogicalDevice()`.

- `EnumeratePhysicalDevices` (`:56-124`): two-pass count/fill; per device reads `vkGetPhysicalDeviceProperties` + `vkGetPhysicalDeviceQueueFamilyProperties`, materialising `PhysicalDeviceInfo` with a `QueueFamilyInfo` list carrying graphics/compute/transfer/sparse bits. **No surface parameter anywhere.**
- `SelectPhysicalDevice` (`:126-176`): prefers a `DiscreteGpu` with a graphics family (`:131-139`), falls back to any device with a graphics family (`:142-152`), throws if none (`:154-158`). Picks the first graphics-capable family index (`:163-170`). Then `FindAsyncComputeQueueFamilyIndex` (`:183-202`) — prefers a compute-only family, falls back to any compute-capable one.
- `CreateLogicalDevice` (`:216-293`): 1 or 2 `VkDeviceQueueCreateInfo` via `stackalloc`, exactly one device extension (`VK_KHR_swapchain`, `:250-252, :268`).

**Answer to the recon question: device selection does NOT depend on the surface existing.** No `vkGetPhysicalDeviceWin32PresentationSupportKHR` exists in the repo at all.

**Present support is queried post-hoc, in `Runtime.cs`:**
```
grep -rn "PresentationSupport\|SurfaceSupport" src/ --include="*.cs"
  → src/DualFrontier.Runtime/Runtime.cs:95, :103
  → src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs:169-170
```
`Runtime.cs:88-111` — after `Surface` is built (`:86`), it calls `vkGetPhysicalDeviceSurfaceSupportKHR(physicalDevice, graphicsQueueFamilyIndex, surface, out support)` and throws if `support == 0`. The comment at `:88-94` is explicit that this is a fail-fast assertion, not a selection input, and names full present-family selection as follow-up **F06**.

**Design implication (favourable):** the surface is a leaf. Swapping Win32→XCB touches surface creation and the instance extension list; **device/queue selection needs zero change**, and the present-support assertion continues to work unmodified because it is surface-generic.

### B4. Swapchain

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanSwapchain.cs` (345 lines).

- Capability query `PrepareSwapchain:140-145` — `vkGetPhysicalDeviceSurfaceCapabilitiesKHR`.
- Extent `:149-152` — honours `caps.currentExtent` when != `uint.MaxValue`, else clamps the caller's width/height to min/max. **On X11/XCB the compositor reports a real `currentExtent`, so the caller-supplied extent is normally ignored** — the caller's `Window.Width/Height` matters far less than on some drivers.
- Image count `:155-159` — `minImageCount + 1`, capped.
- Format `:162-178` — prefers `B8G8R8A8_SRGB` + `SRGB_NONLINEAR`, else `formats[0]`.
- Present mode `:181-196` — prefers `MAILBOX`, falls back `FIFO`.
- `VkSwapchainCreateInfoKHR` `:198-218` — `EXCLUSIVE` sharing, `caps.currentTransform`, `OPAQUE` composite alpha, `clipped = 1`, `oldSwapchain` hint.
- `Recreate(uint,uint)` `:47-67` — prepare-before-reclaim (ELT §2.5).

**Who triggers recreation, and how it learns the new extent — this is load-bearing and contradicts the natural reading of A2:**

Recreation is **NOT** driven by `WindowResizeEvent`. It is driven by Vulkan out-of-date results:
- `LauncherRenderer.cs:130-147` — `AcquireNextImage(..., out bool outOfDate)`; if out-of-date → `WaitIdle()` → `Swapchain.Recreate((uint)_runtime.Window.Width, (uint)_runtime.Window.Height)` → `RecreateFramebuffersForSwapchain()` → recompute camera zoom → `return`.
- `LauncherRenderer.cs:191-215` — same on `Present(...)` returning true (`VK_ERROR_OUT_OF_DATE_KHR` / `VK_SUBOPTIMAL_KHR`, `VulkanSwapchain.cs:119-122`).

So the new extent comes from `IWindow.Width/Height` — which `Window.cs` only ever updates **inside the `WM_SIZE` arm** (`:162-163`). The `WindowResizeEvent` it also enqueues is drained and discarded (see E). **Consequence for the seam: the XCB backend must keep `Width`/`Height` current from `XCB_CONFIGURE_NOTIFY`, or resize silently feeds stale dimensions into `Recreate`.** The `currentExtent` clamp at `:149-152` will mask this on most X11 drivers — which is exactly the kind of accidental correctness worth pinning with a test rather than relying on.

### B5. All other platform-specific Vulkan usage

**Command:** `grep -rn "Win32\|KHR_win32" src/DualFrontier.Runtime/Graphics/ src/DualFrontier.Runtime/Compute/` → **8 hits, all in `VulkanSurface.cs` (7) + `Graphics/MODULE.md:41` (1). `Compute/` is CLEAN — zero hits.**

Extension names are defined in `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs:11-15` (see B2).

---

## C. COMPOSITION + LOOP

### `Runtime.cs` (491 lines)

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Runtime.cs`. `Runtime.Create(RuntimeOptions)` (`:63-186`), private ctor (`:55`), whole body wrapped in `try { … } catch { runtime.Dispose(); throw; }` (`:68`, `:181-185`).

Order (V0.A → V0.C.2):
```
:71  InputQueue    = new InputEventQueue()
:72  Window        = new Window.Window(options.Window, runtime.InputQueue)   ← UNCONDITIONAL
:73  VulkanInstance= new VulkanInstance(options.EnableValidationLayer)
:75-78 ValidationLayer (DEBUG only)
:80  VulkanDevice  = new VulkanDevice(runtime.VulkanInstance)
:83  HardwareCapabilityCheck.Verify(...)                        (К-L19 fail-fast)
:86  Surface       = new VulkanSurface(instance, runtime.Window) ← UNCONDITIONAL
:95-111 vkGetPhysicalDeviceSurfaceSupportKHR present-support assertion (F06)
:113 Swapchain     = new VulkanSwapchain(device, surface, options.Window.Width, options.Window.Height)
:116 RenderPass; :117-122 Framebuffers per swapchain image
:123-126 Graphics + Compute command pools
:127 MemoryAllocator; :128 ComputePipelines
:131-134 AssetManager, DefaultSampler, TextureUploader
:138-141 sprite shader SPIR-V load
:144-159 SpriteDescriptorSetLayout, SpritePipelineLayout, SpritePipeline
:163-168 SpriteRenderer (batched, maxSpritesPerFrame 10_000)
:171-177 Camera2D (ViewportSize from options.Window.Width/Height)
```

**Confirmed: `Runtime.Create` unconditionally constructs Window (`:72`) then Surface (`:86`). There is no flag, no overload, no branch.** `RuntimeOptions` (`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/RuntimeOptions.cs`, 31 lines) carries only `Window`, `EnableValidationLayer`, `AssetsDirectory` — no headless switch.

`Dispose` (`:449-490`): `VulkanDevice.WaitIdle()` first (`:457`), then strict reverse-construction order — sprite stack → V0.B (`ComputePipelines`, `MemoryAllocator`, pools, framebuffers, `RenderPass`, `Swapchain`, `Surface`) → V0.A (`VulkanDevice`, `ValidationLayer`, `VulkanInstance`, **`Window` last at `:488`**).

### Launcher main loop

`/mnt/Work/Colony_Simulator/src/DualFrontier.Launcher/Program.cs` (110 lines), `Main` `:28-109`:
- `:31-42` `RuntimeOptions` (1280×720, `AssetsDirectory = "assets"`).
- `:43` `using var runtime = Runtime.Runtime.Create(runtimeOptions);` ← **process dies here on Linux, at `Window.cs:46`, before any Vulkan call.**
- `:48-52` procedural atlas → `VulkanImage` → `SpriteTexture`.
- `:54-55` `PresentationBridge` + `GameBootstrap.CreateSession(bridge)`.
- `:59-61` `SceneState` / `RenderCommandDispatcher` / `LauncherRenderer`.
- `:64-66` `renderer.Initialize()` → `runtime.Window.Show()` → `session.Loop.Start()` (sim self-ticks on a background thread).
- `:72` `DeviceLossBoundary`.
- **Loop `:75-97`:** `while (runtime.Window.IsOpen)` → compute `deltaSeconds` from `DateTime.UtcNow` → **(1)** `runtime.Window.PumpMessages()` `:82` → **(2)** drain `InputQueue` into a discarded `_` `:86-89` → **(3)** no sim tick (background) → **(4)** `deviceLoss.RunGuarded(frameIndex++, () => renderer.RenderFrame(deltaSeconds))` `:96`.
- **Frame pacing:** none. No sleep, no vsync wait beyond the swapchain present mode (MAILBOX preferred, FIFO fallback) and the `_frameFence.Wait()` inside `RenderFrame` (`LauncherRenderer.cs:197`). The loop spins as fast as present allows.
- `:106-107` shutdown transaction: `session.Dispose()` (sim fence + engine teardown) **then** `renderer.Shutdown()` (GPU), then the `using`-unwind (renderer/atlasTexture/runtime → device → window).

**`PumpMessages` is called exactly once per frame, before render, and is the only writer of `IsOpen` during steady state.** Any XCB backend must preserve that single-call-per-frame drain contract.

### Headless / offscreen arm

**Command:** `grep -rni "headless\|offscreen" src/ --include="*.cs"` → **1 hit**, and it is a comment: `src/DualFrontier.Application/Modding/ModRegistry.cs:62`. **Confirmed: no headless path exists anywhere.**

---

## D. TEST GATE ANATOMY

### The gate type

**There is no type named `PlatformFacts`.** `PlatformFacts.cs` is the *file*; it declares two attributes.

`/mnt/Work/Colony_Simulator/tests/DualFrontier.Runtime.Tests/TestInfrastructure/PlatformFacts.cs` (38 lines):
```csharp
public sealed class WindowsOnlyFactAttribute : FactAttribute        // :14
{
    public WindowsOnlyFactAttribute()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))   // :18
            Skip = "Requires Windows (Win32 + Vulkan runtime); skipped on non-Windows host.";
    }
}
public sealed class WindowsOnlyTheoryAttribute : TheoryAttribute    // :29  (same body, :33-35)
```

**What it checks: `RuntimeInformation.IsOSPlatform(OSPlatform.Windows)` — the OS, and nothing else.** It does not probe the Vulkan loader, a GPU, a display, or `/dev/dri`. `Skip` is set at *discovery* time, so xunit never constructs the test class (documented `:11-12`) — that is why these report as clean skips rather than F09 errors.

The single documented failure mode it exists to prevent is at `docs/reports/LINUX_SANDBOX_ENV_BASELINE_REPORT.md:49`: three `WindowResizeEventTests` were ungated `[Fact]` → `DllNotFoundException: kernel32.dll`, gated at commit `b9af14a`.

### Counts (commands + results)

```
grep -rn "\[WindowsOnlyFact\]"   tests/ --include="*.cs" | grep -v "/obj/\|/bin/" | wc -l   → 108
grep -rn "\[WindowsOnlyTheory\]" tests/ --include="*.cs" | grep -v "/obj/\|/bin/" | wc -l   →   3
```
**108 + 3 = 111 — exactly the CONTEXT's "111 PlatformFacts skips". The arithmetic reconciles.** (A `Theory` whose `Skip` is set at discovery reports as one skipped case, not one per `InlineData`; the 3 theories carry 10 `InlineData` rows between them, which do **not** inflate the count.)

**Project scope:** `grep -rn "\[WindowsOnly" tests/ | cut -d/ -f2 | sort -u` → **`DualFrontier.Runtime.Tests` only.** No other suite carries the gate.

### Per-suite breakdown and DISPLAY-vs-loader classification

Determined by what each file actually constructs (`grep -o "new VulkanInstance\|new VulkanDevice\|Runtime\.Create\|new VulkanSurface\|new VulkanSwapchain\|new NativeWorld"`).

**Class A — needs only the Vulkan LOADER + a GPU (no surface, no display), 14 files / 71 gated tests.** These construct `VulkanInstance` + `VulkanDevice` and stop:

| Count | File |
|---|---|
| 12 | `tests/DualFrontier.Runtime.Tests/Sprite/TileMapTests.cs` |
| 10 | `tests/DualFrontier.Runtime.Tests/Sprite/VertexBufferRingTests.cs` |
| 9 | `tests/DualFrontier.Runtime.Tests/Compute/V1DiffusionEquivalenceTests.cs` |
| 7 | `tests/DualFrontier.Runtime.Tests/Sprite/SpriteIndexBufferTests.cs` |
| 6 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanSamplerTests.cs` |
| 6 | `tests/DualFrontier.Runtime.Tests/Graphics/MemoryAllocatorTests.cs` |
| 5 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanCommandBufferTests.cs` |
| 5 | `tests/DualFrontier.Runtime.Tests/Graphics/HardwareCapabilityCheckTests.cs` |
| 5 | `tests/DualFrontier.Runtime.Tests/Graphics/AsyncComputeQueueSelectionTests.cs` |
| 4 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanPipelineLayoutTests.cs` |
| 4 | `tests/DualFrontier.Runtime.Tests/Graphics/TextureUploaderTests.cs` |
| 3 | `tests/DualFrontier.Runtime.Tests/Compute/ComputePipelineRegistrationTests.cs` |
| 2 | `tests/DualFrontier.Runtime.Tests/Graphics/ShaderCompilationTests.cs` |
| 2 | `tests/DualFrontier.Runtime.Tests/Compute/V1DiffusionIntegrationTests.cs` |

**Caveat that makes Class A larger than it looks:** every one of these files also does `var queue = new InputEventQueue();` — but that is a bare queue, not a Window. However `VulkanInstance` itself requests `VK_KHR_win32_surface` unconditionally (B2), so **Class A cannot flip on the extension fix alone** — it needs the platform-selected extension list too. They do NOT need a display.

**Class B — needs a real surface, i.e. a window and therefore a DISPLAY, 9 files / 40 gated tests:**

| Count | File | Constructs |
|---|---|---|
| 5 | `tests/DualFrontier.Runtime.Tests/Compute/V1DiffusionFactoryTests.cs` | `Runtime.Create` ×5 |
| 5 | `tests/DualFrontier.Runtime.Tests/RuntimeCompositionTests.cs` | `Runtime.Create` ×3 |
| 5 | `tests/DualFrontier.Runtime.Tests/Sprite/SpriteRendererTests.cs` | Surface + Swapchain |
| 4 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanSwapchainTests.cs` | Surface + Swapchain ×3 |
| 3 | `tests/DualFrontier.Runtime.Tests/Window/WindowResizeEventTests.cs` | `new Window(...)` + `Win32Api.SendMessage` |
| 3 | `tests/DualFrontier.Runtime.Tests/Compute/FieldStorageBindingTests.cs` | `Runtime.Create` |
| 2 | `tests/DualFrontier.Runtime.Tests/Sprite/VulkanSpritePipelineTests.cs` | Surface + Swapchain |
| 2 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanRenderPassTests.cs` | Surface + Swapchain |
| 2 | `tests/DualFrontier.Runtime.Tests/Graphics/VulkanGraphicsPipelineTests.cs` | Surface + Swapchain |

**71 + 40 = 111.** ✔

**What a Linux arm needs to flip them:**
1. **Both classes:** the instance extension list must stop asserting `VK_KHR_win32_surface` (B2) — otherwise `new VulkanInstance` throws before anything else.
2. **Both classes:** the Vulkan library name must resolve on Linux — see SURPRISES #1. This is the hard blocker for Class A, which otherwise needs nothing else.
3. **Class B only:** an `IWindow` implementation that opens on the host + a matching `VkSurfaceKHR` path.
4. **`WindowResizeEventTests` specifically is not portable as written** — it synthesises `WM_SIZE` via `Win32Api.SendMessage` (`:30, :49, :64`), which is the *only* consumer of that P/Invoke. The XCB analogue is a synthesised `XCB_CONFIGURE_NOTIFY`, so these three tests need rewriting, not re-gating.
5. The gate attribute itself is a pure `IsOSPlatform(Windows)` check; a Linux arm means either a new capability-probing attribute (loader-present / display-present as separate predicates — the Class A / Class B split above is exactly that boundary) or removal of the gate where it no longer applies.

**Two ungated tests already run on Linux today** and constrain the seam: `tests/DualFrontier.Runtime.Tests/Window/WindowOptionsTests.cs` (42 lines — pins `WindowOptions` defaults 1280/720/"Dual Frontier"/`Resizable = true`) and `tests/DualFrontier.Runtime.Tests/Input/VirtualKeyMapperTests.cs` (100 lines — `[Theory]` pinning raw VK hex → `Key`, e.g. `[InlineData(0x25, Key.Left)]`). **The latter pins `VirtualKeyMapper` to Win32 VK numerics from an ungated test — so touching `VirtualKeyMapper` for an xkbcommon keysym path will break a currently-green Linux test.**

---

## E. INPUT PATH

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/InputEventQueue.cs` (20 lines): `public sealed class InputEventQueue` wrapping `ConcurrentQueue<IInputEvent>` — `Enqueue` (`:15`), `TryDequeue` (`:17`), `Count` (`:19`). Platform-neutral already; nothing to change.

**Event types** (`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Input/`):

| Type | File | Shape |
|---|---|---|
| `IInputEvent` | `IInputEvent.cs` (9) | marker interface |
| `KeyPressedEvent` | `KeyPressedEvent.cs` (4) | `record(Key Key)` |
| `KeyReleasedEvent` | `KeyReleasedEvent.cs` (4) | `record(Key Key)` |
| `MouseMovedEvent` | `MouseMovedEvent.cs` (4) | `record(int X, int Y)` |
| `MouseButtonEvent` | `MouseButtonEvent.cs` (7) | `record(MouseButton Button, bool Pressed)` |
| `MouseWheelEvent` | `MouseWheelEvent.cs` (7) | `record(int Delta)` — normalised ±1 per notch |
| `WindowFocusEvent` | `WindowFocusEvent.cs` (8) | `record(bool Focused)` |
| `WindowResizeEvent` | `WindowResizeEvent.cs` (7) | `record(int NewWidth, int NewHeight)` |
| `Key` | `Key.cs` (30) | enum: Unknown, 4 arrows, 3 modifiers, 10 special, F1-F12, A-Z, Digit0-9 |
| `MouseButton` | `MouseButton.cs` (4) | enum Left/Right/Middle |
| `VirtualKeyMapper` | `VirtualKeyMapper.cs` (49) | **`Win32Constants.VK_*` → `Key`; ranges `0x41-0x5A`→A-Z, `0x30-0x39`→Digit0-9** |

**Producers — every `InputQueue.Enqueue` call, all in `Window.cs`:**
`:164` `WindowResizeEvent` (WM_SIZE) · `:175` `KeyPressedEvent` (WM_KEYDOWN) · `:185` `KeyReleasedEvent` (WM_KEYUP) · `:200` `KeyPressedEvent` (WM_SYSKEYDOWN) · `:210` `KeyReleasedEvent` (WM_SYSKEYUP) · `:222` `MouseMovedEvent` (WM_MOUSEMOVE) · `:226/:229` `MouseButtonEvent(Left, true/false)` · `:232/:235` `(Right, …)` · `:238/:241` `(Middle, …)` · `:248` `MouseWheelEvent` (WM_MOUSEWHEEL) · `:254` `WindowFocusEvent(true)` (WM_SETFOCUS) · `:257` `WindowFocusEvent(false)` (WM_KILLFOCUS).

**14 enqueue sites, 7 distinct event types. `Window.cs` is the sole producer in the repo.**

**Consumers — the drain-and-drop claim is CONFIRMED:**

`src/DualFrontier.Launcher/Program.cs:84-89`:
```csharp
// 2. Drain InputQueue → forward к Application.
//    Future cascade — InputBridge wiring TBD; events discarded for now.
while (runtime.InputQueue.TryDequeue(out IInputEvent? _))
{
    // Future cascade will forward к Application input bridge here.
}
```
The discard is a `_` in the `out` position. **Zero production consumers of any event type.**

Corroborated in the governed corpus: `docs/architecture/VULKAN_SUBSTRATE.md:354` — "`InputEventQueue` (window → consumer; today drained and discarded on the same thread that fills it)".

Other drain sites, all diagnostic: `tests/DualFrontier.Runtime.SmokeTest/Program.cs:69, 134, 447, 577, 745, 850` (`:321` prints `[PASS] V0.C.1 R.4 input events`). Application-side `IInputSource.cs:6` and `IntentOverlayLayer.cs:21` reference it **in comments only** — no code path.

**Consequence for the design:** because `WindowResizeEvent` is discarded, the resize path runs entirely through B4's out-of-date mechanism reading `IWindow.Width/Height`. The XCB backend can therefore ship correct resize behaviour by keeping `Width`/`Height` current from `XCB_CONFIGURE_NOTIFY`, without any consumer for the event — but the event should still be published to preserve parity with the pinned WM_SIZE semantics (skip 0×0, skip unchanged).

---

## F. HOST ENVIRONMENT (all non-mutating queries)

```
$XDG_SESSION_TYPE = wayland
$WAYLAND_DISPLAY  = wayland-0
$DISPLAY          = :0
```

`ldconfig -p | grep -E "libxcb\.|libxcb-keysyms|libX11|libwayland-client|libxkbcommon"`:

| Library | Present | soname / path |
|---|---|---|
| `libxcb` | **YES** | `libxcb.so.1` → `/usr/lib/x86_64-linux-gnu/libxcb.so.1` (also `libxcb.so` dev symlink) |
| `libxcb-keysyms` | **NO** | not in cache |
| `libxcb-icccm` | **NO** | not in cache (checked in the same command) |
| `libX11` | YES | `libX11.so.6` |
| `libX11-xcb` | YES | `libX11-xcb.so.1` |
| `libwayland-client` | YES | `libwayland-client.so.0` |
| `libxkbcommon` | YES | `libxkbcommon.so.0` |
| `libxkbcommon-x11` | YES | `libxkbcommon-x11.so.0` |

`ldconfig -p | grep vulkan` — loader **`libvulkan.so.1`** → `/usr/lib/x86_64-linux-gnu/libvulkan.so.1` (plus `libvulkan.so` dev symlink and the Mesa ICDs `libvulkan_radeon.so`, `libvulkan_intel.so`, `libvulkan_lvp.so`, `libvulkan_virtio.so`, `libvulkan_nouveau.so`, `libvulkan_asahi.so`, `libvulkan_intel_hasvk.so`).

Headers (reference only — P/Invoke needs the `.so`): `/usr/include/xcb/xcb.h` (22326 b) and `/usr/include/X11/Xlib.h` (99801 b) both present.

XWayland: `which Xwayland` → `/usr/bin/Xwayland`; `ls -la` → `-rwxr-xr-x root root 2410256 Apr 14 10:16`.

GPU: `ls /dev/dri/` → `by-path/`, **`card1` (crw-rw---- root video 226,1)**, **`renderD128` (crw-rw---- root render 226,128)**. Consistent with the CONTEXT's RADV.

**PLAIN ANSWER: YES — an xcb window can be created in THIS session.** `DISPLAY=:0` is set, XWayland is installed at `/usr/bin/Xwayland`, and `libxcb.so.1` is in the loader cache. The session is natively Wayland, so X11 clients run through XWayland; that is a fully functional path for `VK_KHR_xcb_surface` (Mesa/RADV supports XCB surfaces over XWayland). A DRI render node is present for the GPU.

**Two caveats the design should absorb:**
1. **`libxcb-keysyms` and `libxcb-icccm` are NOT installed.** Do not design against them. Core `libxcb.so.1` covers everything needed: `xcb_intern_atom` handles `WM_PROTOCOLS`/`WM_DELETE_WINDOW` (the close-button contract) without icccm, and `libxkbcommon-x11.so.0` (present) is the modern keycode→keysym path, superior to xcb-keysyms anyway. Choosing the helper libs would add an install-time dependency this host does not satisfy.
2. Because the session is Wayland-native, a future `VK_KHR_wayland_surface` backend is also viable here (`libwayland-client.so.0` present) — but XCB-first is the right call, since it needs one connection object and no protocol-object bookkeeping.

---

## G. SDK / ANALYZER CONSTRAINTS

### The binding constraint: DFK002 is an ERROR and it polices P/Invoke placement

`/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/DFK002PInvokeBindingsAnalyzer.cs`:
- `defaultSeverity: DiagnosticSeverity.Error` (`:56`), `isEnabledByDefault: true` (`:57`).
- `RegisterSymbolAction(AnalyzePInvokeMethod, SymbolKind.Method)` (`:73`); flags any method carrying `DllImportAttribute` or `LibraryImportAttribute` (`:76-92`) whose containing namespace fails `SanctionedInteropSurface.IsSanctioned` (`:94`).

`/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/SanctionedInteropSurface.cs:30-34`:
```csharp
private static readonly string[] SanctionedRoots =
{
    "DualFrontier.Core.Interop",
    "DualFrontier.Runtime.Native",
};
```
Match is exact-or-nested (`:50`).

**Hard requirement for the cascade: every new xcb/xkb P/Invoke MUST be declared under `DualFrontier.Runtime.Native.*` (e.g. `DualFrontier.Runtime.Native.Xcb`). Anything else is a build-breaking `error DFK002`.** The `SanctionedInteropSurface` XML doc (`:17-21`) already anticipates this shape, naming `Runtime.Native.Win32` as "Launcher OS surface".

### Analyzer wiring scope

`/mnt/Work/Colony_Simulator/src/Directory.Build.props` wires `tools/DualFrontier.Analyzers` as `OutputItemType="Analyzer"` for **all `src/` projects** (`:34-38`), and re-imports the root `Directory.Build.props` (`:31`). `tests/` and `tools/` are explicitly out of scope (comment `:22`). **There is no `tests/Directory.Build.props`** — confirmed by `ls`.

Root `/mnt/Work/Colony_Simulator/Directory.Build.props`: `net10.0`, `LangVersion 14.0`, `Nullable enable`, `ImplicitUsings enable`, **`TreatWarningsAsErrors true`**, `GenerateDocumentationFile true`, `NoWarn += CS1591`. A `CompileShaders` target runs `BeforeTargets="Build"` **only when `'$(OS)' == 'Windows_NT'`** and only for `DualFrontier.Runtime` — on Linux the committed `assets/shaders/*.spv` are the artifacts.

`TreatWarningsAsErrors = true` means **DFK019_A (Warning severity) is effectively an Error for `src/`.** It only fires on `using` directives whose root namespace is one of `OpenGL, OpenTK, DirectX, Direct3D, SharpDX, Vortice, Metal` (`/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/DFK019_AStaticVulkanApiAnalyzer.cs:87-90, :92-118`). **`Xcb`, `X11`, `Wayland` are not on that list — no conflict.** DFK017 (К-L17 display composition, Error, `Rules/Architecture/DFK017DisplayCompositionAnalyzer.cs:35-56`) concerns multi-layer draw discipline, not windowing.

### No-LINQ / no-async law

**Command:** `grep -rn "System.Linq\|IsAsync\|AsyncKeyword" tools/DualFrontier.Analyzers/Rules/ --include="*.cs"` → **zero hits.** There is **no analyzer enforcing no-LINQ or no-async**. It is doctrinal law (CLAUDE.md; ROADMAP F-47 records it as "LINQ convention documentary-not-enforced, `SpatialGrid.cs:3`"). `src/DualFrontier.Runtime` is subject to the law by convention, not by machine — and the existing Runtime code honours it (`VulkanInstance`/`VulkanDevice` use `foreach` + `List<T>` throughout, never `.Where`/`.Select`).

### `[SupportedOSPlatform]`

**Command:** `grep -rn "SupportedOSPlatform" src/ --include="*.cs" | wc -l` → **0.** No platform attributes anywhere in `src/`. No `RuntimeIdentifier` or `TargetPlatform` properties either. The Runtime csproj (`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/DualFrontier.Runtime.csproj`) declares only `AssemblyName`, `RootNamespace`, `AllowUnsafeBlocks=true`, one `ProjectReference` to `Core.Interop`, and `InternalsVisibleTo("DualFrontier.Runtime.Tests")`.

**Design note:** the absence of `[SupportedOSPlatform("windows")]` is why the Win32 P/Invokes compile cleanly today and fail only at runtime. If the seam introduces per-platform implementation classes, adding the attribute would move some of that to compile time — but it would also require annotating or suppressing every consumer, and `TreatWarningsAsErrors` makes CA1416 a build break. That is a design decision, not a given.

### `InternalsVisibleTo` chain (affects where a seam can live)

- `DualFrontier.Runtime.csproj` → `DualFrontier.Runtime.Tests`. This is why `WindowResizeEventTests` can call `internal Win32Api.SendMessage`.
- `DualFrontier.Launcher.csproj` → `DualFrontier.Runtime.Tests` (for `RenderCommandDispatcher`, W3/G2).

---

## H. SIZE / SHAPE FOR THE BRIEF

### Every file read, with `wc -l`

**Production — the change set:**
| Lines | Path |
|---|---|
| 72 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Api.cs` |
| 83 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Constants.cs` |
| 50 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/Win32Structs.cs` |
| 6 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/WindowProc.cs` |
| 52 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Win32/MODULE.md` |
| **302** | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/Window.cs` |
| 24 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/IWindow.cs` |
| 13 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/WindowOptions.cs` |
| 20 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/InputEventQueue.cs` |
| 53 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Window/MODULE.md` |
| 63 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanSurface.cs` |
| 148 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanInstance.cs` |
| 49 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Input/VirtualKeyMapper.cs` |
| 28 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs` |

**Production — read for context, unlikely to change:**
| Lines | Path |
|---|---|
| 322 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanDevice.cs` |
| 345 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Graphics/VulkanSwapchain.cs` |
| 491 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Runtime.cs` |
| 31 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/RuntimeOptions.cs` |
| 545 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs` |
| 557 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs` |
| 1012 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs` |
| 110 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Launcher/Program.cs` |
| 242 | `/mnt/Work/Colony_Simulator/src/DualFrontier.Launcher/LauncherRenderer.cs` |
| 9/30/4/4/4/7/4/7/8/7 | the ten `src/DualFrontier.Runtime/Input/*.cs` event/enum files |

**Tooling / tests read:**
| Lines | Path |
|---|---|
| 38 | `/mnt/Work/Colony_Simulator/tests/DualFrontier.Runtime.Tests/TestInfrastructure/PlatformFacts.cs` |
| 68 | `/mnt/Work/Colony_Simulator/tests/DualFrontier.Runtime.Tests/Window/WindowResizeEventTests.cs` |
| 109 | `/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/DFK002PInvokeBindingsAnalyzer.cs` |
| 58 | `/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/SanctionedInteropSurface.cs` |
| 119 | `/mnt/Work/Colony_Simulator/tools/DualFrontier.Analyzers/Rules/NativeBoundary/DFK019_AStaticVulkanApiAnalyzer.cs` |
| 991 | `/mnt/Work/Colony_Simulator/tests/DualFrontier.Runtime.SmokeTest/Program.cs` (grepped, not read in full) |

### Test csproj files referencing the gate

**Only one:** `/mnt/Work/Colony_Simulator/tests/DualFrontier.Runtime.Tests/DualFrontier.Runtime.Tests.csproj`. `PlatformFacts.cs` lives inside that project (`TestInfrastructure/`), so no csproj *references* it — it is compiled in. Its project refs are `src/DualFrontier.Runtime` and `src/DualFrontier.Launcher`; it carries **both** the Windows `.dll` arm (`:28-32`) and the **Linux `.so` arm** (`:40-44`, added by commit `e30375c`).

### `tests/DualFrontier.Runtime.Tests/` — every file, one-line purpose

| Lines | File | Purpose |
|---|---|---|
| 79 | `AmbientTintDispatchTests.cs` | W3/G2 Launcher ambient-tint dispatch arm (via `InternalsVisibleTo`) |
| 249 | `Assets/AssetManagerTests.cs` | asset root resolution + load |
| 454 | `Assets/PngDecoderTests.cs` | PNG decode correctness |
| 87 | `Compute/ComputePipelineRegistrationTests.cs` | compute pipeline registry (**3 gated**, loader-only) |
| 38 | `Compute/DiffusionPushConstantsTests.cs` | push-constant struct layout (ungated, pure) |
| 105 | `Compute/FieldStorageBindingTests.cs` | `FieldStorageBinding` attach via `Runtime.Create` (**3 gated**, display) |
| 592 | `Compute/V1DiffusionEquivalenceTests.cs` | GPU vs CPU diffusion equivalence (**9 gated**, loader-only) |
| 119 | `Compute/V1DiffusionFactoryTests.cs` | `Runtime.CreateV1DiffusionPipeline` factory (**5 gated**, display) |
| 126 | `Compute/V1DiffusionIntegrationTests.cs` | end-to-end diffusion dispatch (**2 gated**, loader-only) |
| 67 | `Diagnostic/ValidationLogTests.cs` | validation-layer message capture |
| 75 | `Graphics/AsyncComputeQueueSelectionTests.cs` | К-L19 Item 43 queue-family selection (**5 gated**) |
| 116 | `Graphics/DeviceLostTests.cs` | M9 device-lost classification (ungated) |
| 59 | `Graphics/HardwareCapabilityCheckTests.cs` | К-L19 fail-fast (**5 gated**) |
| 109 | `Graphics/MemoryAllocatorTests.cs` | VkDeviceMemory allocation (**6 gated**) |
| 99 | `Graphics/PrepareBeforeReclaimTests.cs` | ELT §2.5 all-or-nothing build primitive (ungated) |
| 76 | `Graphics/ShaderCompilationTests.cs` | SPIR-V module load (**2 gated**) |
| 108 | `Graphics/TextureUploaderTests.cs` | staging→device image upload (**4 gated**) |
| 83 | `Graphics/VulkanCommandBufferTests.cs` | command buffer record/submit (**5 gated**) |
| 41 | `Graphics/VulkanDeviceMarshallingTests.cs` | device struct marshalling (ungated) |
| 75 | `Graphics/VulkanGraphicsPipelineTests.cs` | graphics pipeline creation (**2 gated**, display) |
| 49 | `Graphics/VulkanInstanceMarshallingTests.cs` | instance createinfo marshalling (ungated) |
| 76 | `Graphics/VulkanPipelineLayoutTests.cs` | pipeline layout + push constants (**4 gated**) |
| 76 | `Graphics/VulkanRenderPassTests.cs` | render pass creation (**2 gated**, display) |
| 95 | `Graphics/VulkanSamplerTests.cs` | sampler creation (**6 gated**) |
| 83 | `Graphics/VulkanSwapchainTests.cs` | swapchain create + recreate (**4 gated**, display) |
| 100 | `Input/VirtualKeyMapperTests.cs` | **VK hex → `Key` mapping — UNGATED, runs on Linux today** |
| 17 | `PlaceholderTests.cs` | project scaffold placeholder |
| 92 | `RuntimeCompositionTests.cs` | `Runtime.Create` composition + Dispose (**5 gated**, display) |
| 83 | `Sprite/AtlasRegionTests.cs` | atlas UV math (ungated) |
| 130 | `Sprite/Camera2DTests.cs` | view-projection matrix (ungated) |
| 92 | `Sprite/SpriteIndexBufferTests.cs` | index buffer (**7 gated**) |
| 145 | `Sprite/SpriteRendererTests.cs` | batched sprite renderer (**5 gated**, display) |
| 78 | `Sprite/SpriteVertexTests.cs` | vertex packing + `PackTintRgba` (ungated) |
| 158 | `Sprite/TileMapTests.cs` | tile map sprite generation (**12 gated**) |
| 162 | `Sprite/VertexBufferRingTests.cs` | per-image vertex ring (**10 gated**) |
| 89 | `Sprite/VulkanSpritePipelineTests.cs` | sprite pipeline (**2 gated**, display) |
| 38 | `TestInfrastructure/PlatformFacts.cs` | **the gate attributes** |
| 321 | `Vulkan/VulkanStructSizeTests.cs` | Vk struct size/offset pins (ungated) |
| 42 | `Window/WindowOptionsTests.cs` | **`WindowOptions` defaults — UNGATED, runs on Linux today** |
| 68 | `Window/WindowResizeEventTests.cs` | WM_SIZE → `WindowResizeEvent` (**3 gated**; uses `SendMessage`) |

**Total: 4751 lines across 40 files.**

---

## DISCREPANCIES vs the CONTEXT

1. **"15 Win32 P/Invokes in the single file `Win32Api.cs`" — count correct, "single file" understates the coupling.** The count is exactly 15 (13 user32 + 2 kernel32). But `Win32Constants.VK_*` is consumed by **`src/DualFrontier.Runtime/Input/VirtualKeyMapper.cs:1, 14-42`** — a fifth production file outside `Native/Win32/`, `Window/`, and `Graphics/`, which the CONTEXT does not name. It is also pinned by an **ungated, currently-green Linux test** (`Input/VirtualKeyMapperTests.cs`), so touching it has immediate suite consequences.

2. **"dies in `Window.InitializeWin32()` on `kernel32.dll` before Vulkan" — correct, but there is a SECOND independent kernel32 site.** `src/DualFrontier.Runtime/Graphics/VulkanSurface.cs:25` calls `Win32Api.GetModuleHandle(null)` on its own. Fixing `Window.cs` alone would move the death 40 lines later in `Runtime.Create` (`:72` → `:86`), not eliminate it.

3. **"The blocker is narrow: windowing + VK surface only" — this is the material discrepancy.** Two further blockers sit behind it, both outside the stated scope. See SURPRISES #1 and #2. Neither can be deferred: #1 blocks *every* Vulkan call, #2 blocks `vkCreateInstance` before a surface is ever requested.

4. **"`vkCreateWin32SurfaceKHR` / `VK_KHR_win32_surface` in `VulkanSurface.cs` and `VulkanInstance.cs`" — incomplete file list.** The same coupling also lives in `Native/Vulkan/VkApi.cs:159-162` (the P/Invoke), `VkStructs.cs:133-139` (`VkWin32SurfaceCreateInfoKHR`), `VkEnums.cs:86` (the sType), and `VkConstants.cs:14` (the name). Five files, not two.

5. **"Vulkan itself is healthy here (RADV, API 1.4.335)" — true of the SYSTEM, not of the managed binding.** The host loader is `libvulkan.so.1` and a render node exists; the C# side cannot reach either (SURPRISES #1).

6. **Working-tree drift vs the session-start git snapshot.** The snapshot showed `M tools/governance/SCOPE_EXCLUSIONS.yaml` and `?? CLAUDE.md`. At recon time `git status --short` reports **only** `M .claude/settings.local.json`; `git diff tools/governance/SCOPE_EXCLUSIONS.yaml` is empty, and **`/mnt/Work/Colony_Simulator/CLAUDE.md` does not exist on disk** (`ls` → No such file). Not a defect, but the Phase 0 preconditions should re-measure rather than trust the snapshot. Relevant because governance scans the filesystem: had `CLAUDE.md` still been present un-enrolled and un-excluded (it is **not** in `tools/governance/SCOPE_EXCLUSIONS.yaml` — grep returns nothing), it would be a G-PATH block under `validate --armed`.

---

## SURPRISES (load-bearing, not anticipated by the CONTEXT)

### 1. `VkApi.VulkanLib = "vulkan-1.dll"` — hard-coded, with NO import resolver anywhere in the repo

`/mnt/Work/Colony_Simulator/src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs:7`:
```csharp
private const string VulkanLib = "vulkan-1.dll";
```
This name feeds **every one of the ~100 `[LibraryImport]` declarations** in that 545-line file.

**Command:** `grep -rn "SetDllImportResolver\|NativeLibrary\.\|DllImportResolver" --include="*.cs" . | grep -v "/obj/\|/bin/"` → **2 hits, both in analyzer rule text and its test** (`DFK001NativeLanguageAnalyzer.cs:73`, `DFK001NativeLanguageTests.cs:9`). **There is no `NativeLibrary.SetDllImportResolver` call anywhere in the repository.** `find . -name "runtimeconfig.template.json" -o -name "*.dll.config"` → none.

The .NET Unix probing sequence for a name `N` tries `N.so`, `libN.so`, `N`, `libN` — i.e. `vulkan-1.dll.so`, `libvulkan-1.dll.so`, `vulkan-1.dll`, `libvulkan-1.dll`. The host's actual loader soname is **`libvulkan.so.1`**, which none of those match. **Every Vulkan P/Invoke will throw `DllNotFoundException` on Linux.**

`VkApi.cs:153` already carries the tell in a comment: "KHR functions exported by vulkan-1.dll loader trampoline **on Windows**".

Why this was never caught: `docs/reports/LINUX_SANDBOX_ENV_BASELINE_REPORT.md:54` (D7) records "Runtime GPU paths are WindowsOnly-skipped anyway" — the 111-skip gate has hidden this since the first Linux session. **This is the true first blocker on the Vulkan side and it is entirely outside the CONTEXT's stated scope.** It is also cheap: one `[ModuleInitializer]` + `NativeLibrary.SetDllImportResolver` mapping `"vulkan-1.dll"` → `libvulkan.so.1`, placed under `DualFrontier.Runtime.Native.*` to satisfy DFK002. It should be its own commit with a falsification proof (the F-60 discipline: watch a Vulkan call fail, apply the resolver, watch it succeed).

### 2. `VulkanInstance` asserts `VK_KHR_win32_surface` with no availability query — it fails before any surface exists

`VulkanInstance.cs:48-52` builds the extension list as a literal and never calls `vkEnumerateInstanceExtensionProperties` (zero occurrences in the file). On Linux `vkCreateInstance` returns `VK_ERROR_EXTENSION_NOT_PRESENT` and throws at `:101-109`. This is a **second, independent** blocker that fires at `Runtime.cs:73` — *before* `VulkanSurface` is even constructed at `:86` — and it is what keeps the 71 "loader-only" gated tests (Class A in §D) red even though they never touch a window.

### 3. The Launcher csproj never got the Linux native arm that all 8 test projects received

`/mnt/Work/Colony_Simulator/src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` copies the native kernel from `native/DualFrontier.Core.Native/build/**Release**/DualFrontier.Core.Native.dll` under `Condition="Exists(...)"` — **Windows path only.**

**Command:** `grep -c "Native.so" src/DualFrontier.Launcher/DualFrontier.Launcher.csproj` → **0.**

Every test project got the sibling `.so` arm at commit `e30375c` — verified present in `tests/DualFrontier.Runtime.Tests` (`:40-44`), `tests/DualFrontier.Runtime.SmokeTest`, `Core.Interop.Tests`, `Application.Tests`, `Core.Tests`, `Modding.Tests`. On disk the artifact exists: `ls native/DualFrontier.Core.Native/build/*.so` → **`DualFrontier.Core.Native.so`** (note: no `lib` prefix). Because the copy is `Condition="Exists(...)"`, the Launcher build stays silently green and then `GameBootstrap.CreateSession` (`Program.cs:55` → `NativeWorld`) fails at runtime for want of the kernel.

**Consequence: even a perfect XCB window + XCB surface will not produce a running Launcher.** This is a one-item csproj addition (copy the ratified test-project comment block verbatim) but it must be in the cascade or the live Launcher smoke — the debt W3 has owed since 2026-08-20 — still cannot be paid.

### 4. `SendMessage` exists solely for a test, and that test is not portable

`Win32Api.cs:54-55` declares `SendMessageW`. Its only callers repo-wide are `WindowResizeEventTests.cs:30, 49, 64`. It has **zero production call sites**. This explains the three-way count disagreement between sources: `Native/Win32/MODULE.md:44` says "V0.A surface (14 functions)" and omits `SendMessage`; `docs/architecture/VULKAN_SUBSTRATE.md:61` says "15 `LibraryImport` declarations" (also wrong — one is `[DllImport]`); the file has 15 P/Invokes, 14 `[LibraryImport]` + 1 `[DllImport]`. Both docs need correcting in the cascade's doc pass regardless of the seam design.

The portability point: those three tests inject a synthetic `WM_SIZE` into the window procedure. There is no XCB equivalent of "call the window proc directly" — the analogue is crafting an `xcb_configure_notify_event_t` and pushing it through `xcb_send_event`, or refactoring so the message-decode step is a pure function testable without a live window. **The latter is the better design and worth stating in the brief:** it would move the WM_SIZE/CONFIGURE_NOTIFY decode logic out from behind a live OS object and let all three tests run ungated on every platform.

### 5. The resize path does NOT flow through `WindowResizeEvent` — the event is dead

The Launcher drains and discards (`Program.cs:86-89`), and recreation is instead driven by `VK_ERROR_OUT_OF_DATE_KHR` / `VK_SUBOPTIMAL_KHR` reading `IWindow.Width`/`Height` (`LauncherRenderer.cs:130-147`, `:191-215`). So `Width`/`Height` are the load-bearing outputs of the message pump, and the event is decorative. **An XCB backend that publishes `WindowResizeEvent` but forgets to update `Width`/`Height` would look correct in tests and break resize in production** — and `VulkanSwapchain.cs:149-152` (`caps.currentExtent` preference) would mask it on most X11 drivers. Worth an explicit pin.

### 6. Two structural conveniences that will make the seam cheaper than it looks

- **Device selection is surface-independent** (§B3). `VulkanDevice` needs zero change; the present-support assertion at `Runtime.cs:95-111` is already surface-generic and keeps working.
- **`_className` is per-instance GUID-unique** (`Window.cs:53`), and the `RollbackWin32Initialization`/`Dispose` pair (`:121-139` / `:283-301`) is an already-solved partial-construction rollback with an explicit F08 rationale. The XCB backend should carry both properties across rather than re-derive them.

### 7. Dead surface the cascade can retire for free

`Win32Structs.cs:44-50` `RECT` — zero references repo-wide. `Win32Constants.WM_CREATE` (`:6`) — declared, never switched on. `WindowOptions.Resizable` (`WindowOptions.cs:12`) — never read by `Window.cs`, yet **pinned by the ungated `WindowOptionsTests.cs:17`**, so it cannot simply be deleted without touching a green Linux test. `IWindow.Hide()` — zero consumers.

---

## SUMMARY: the design leans this recon supports

1. **The seam is narrower than the file count suggests.** Production consumes exactly six `IWindow` members, and `Handle` — the only platform-bound one — has a single production reader (`VulkanSurface.cs:38`). Moving surface creation behind the window implementation (which already knows both the connection and the window id) keeps `IWindow` free of platform-tagged handles entirely.
2. **XCB-first is well-supported on this host** and needs only `libxcb.so.1` + `libxkbcommon-x11.so.0`, both present. Avoid `libxcb-keysyms` / `libxcb-icccm` — **not installed here**.
3. **New P/Invokes must live under `DualFrontier.Runtime.Native.*`** or DFK002 fails the build as an Error.
4. **Three blockers, not one, and they fire in this order:** `kernel32` at `Window.cs:46` → `vulkan-1.dll` name resolution (SURPRISE #1) → `VK_KHR_win32_surface` assertion at `VulkanInstance.cs:51` (SURPRISE #2). Plus a fourth, non-fatal-at-build but fatal-at-run: the Launcher's missing native `.so` copy arm (SURPRISE #3).
5. **The test story splits cleanly at a real boundary:** 71 gated tests need only the loader + extension fix (no display); 40 need a live window. That split is a better gate predicate than `IsOSPlatform(Windows)` and can be shipped as two capability-probing attributes.
6. **Prove the instrument fires.** Per the project's own standing lesson: the resolver fix, the extension-list fix, and the `Width`/`Height` update path should each be broken once deliberately and restored, because all three have a masking mechanism (probing fallbacks, `currentExtent` clamp, `Condition="Exists"`) that can make a broken state look green.
