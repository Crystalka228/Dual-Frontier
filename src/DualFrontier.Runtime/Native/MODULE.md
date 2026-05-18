# DualFrontier.Runtime.Native

**Purpose:** Pure P/Invoke surfaces for OS + GPU APIs. Two sub-modules:

- `Native/Win32/` — Win32 P/Invoke (`user32.dll`, `kernel32.dll`)
- `Native/Vulkan/` — Vulkan P/Invoke (`vulkan-1.dll`)

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.5.

## Discipline

- **Pure P/Invoke к OS/GPU DLLs.** Zero third-party C# bindings (per VULKAN_SUBSTRATE §0 L2 + L3).
- **`internal` accessibility.** P/Invoke surface does not leak past Runtime project (Rule 5).
- **Source-generated marshalling preferred.** `[LibraryImport]` (.NET 7+) where source generator
  can handle the signature. Fall back к `[DllImport]` only where struct fields have non-blittable
  types (strings) that source generator cannot marshal in-struct.
- **Canonical struct + enum naming.** `WNDCLASSEX` (Win32 docs), `VkInstanceCreateInfo` (Vulkan
  1.3 spec). Pascal case wrapper classes (`Win32Api`, `VkApi`) per CODING_STANDARDS.

## V0.A subset

- Win32: 14 functions для window creation + message pump (per V0.A brief §3 Commit 3)
- Vulkan: ~12-15 functions для instance + device + queue family enumeration (per V0.A brief
  §3 Commit 4). Swapchain/surface/command-pool/memory functions deferred к V0.B.
