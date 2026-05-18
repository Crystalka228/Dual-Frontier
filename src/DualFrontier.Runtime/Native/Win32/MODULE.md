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
