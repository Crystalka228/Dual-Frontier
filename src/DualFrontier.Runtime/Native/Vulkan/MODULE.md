# DualFrontier.Runtime.Native.Vulkan

**Purpose:** Pure P/Invoke to Vulkan API. `[LibraryImport]` (source-generated marshalling) +
`unsafe partial class` для function declarations + struct definitions + enums per Vulkan 1.3 spec.

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../../docs/architecture/VULKAN_SUBSTRATE.md)
§2.5 Vulkan template + Vulkan 1.3 specification verbatim.

**Dependencies:** `System` (BCL), `System.Runtime.InteropServices`.

## Marshalling discipline (S-LOCK-6 + Lesson #22)

- **Pure P/Invoke к `vulkan-1.dll`.** Zero third-party C# bindings (Silk.NET, Vortice.Vulkan не используются).
- **`[LibraryImport]` для core API** — vkCreateInstance, vkDestroyInstance, etc. Functions
  с simple parameters + struct pass-by-ref work cleanly under source generator.
- **Function pointer loading** — Extension functions (`vkCreateDebugUtilsMessengerEXT`) loaded
  at runtime via `vkGetInstanceProcAddr`, не declared as direct P/Invoke (extensions не exported
  from vulkan-1.dll). Pattern used by ValidationLayer (Commit 7).
- **UTF-8 strings.** Vulkan uses UTF-8 for layer/extension names, application strings. Marshal
  via `Marshal.StringToCoTaskMemUTF8` (consumer-side) — не StringMarshalling.Utf16 which is for
  Win32.

## V0.A function surface (~12 functions)

### Instance lifecycle
- `vkCreateInstance` / `vkDestroyInstance`
- `vkEnumerateInstanceVersion`

### Physical device + queue family discovery
- `vkEnumeratePhysicalDevices`
- `vkGetPhysicalDeviceProperties`
- `vkGetPhysicalDeviceQueueFamilyProperties`

### Logical device + queue
- `vkCreateDevice` / `vkDestroyDevice`
- `vkGetDeviceQueue`

### Extension function loading
- `vkGetInstanceProcAddr`

### Extension functions (loaded at runtime via vkGetInstanceProcAddr, не direct exports)
- `vkCreateDebugUtilsMessengerEXT` / `vkDestroyDebugUtilsMessengerEXT`

Pre-V0.B addition (deferred): swapchain, surface, command pool, fence/semaphore, memory
allocation, render pass, pipeline functions.

## V0.A struct surface

- `VkApplicationInfo`, `VkInstanceCreateInfo`
- `VkPhysicalDeviceProperties` (read-only — V0.A reads deviceName + deviceType + apiVersion;
  full layout includes opaque `limits` + `sparseProperties` blocks per Vulkan 1.3 spec)
- `VkQueueFamilyProperties`
- `VkDeviceQueueCreateInfo`, `VkDeviceCreateInfo`
- `VkDebugUtilsMessengerCreateInfoEXT`, `VkDebugUtilsMessengerCallbackDataEXT` (Commit 7)

## VkPhysicalDeviceProperties size validation

Per Vulkan 1.3 spec on x64: 5×uint32 + 256(deviceName) + 16(pipelineCacheUUID) + 504(limits)
+ 20(sparseProperties) = **816 bytes**. Verified at runtime via VulkanDeviceMarshallingTests
(Commit 8) — если sizeof mismatch (cross-version drift), test halts cascade per SC-2.
