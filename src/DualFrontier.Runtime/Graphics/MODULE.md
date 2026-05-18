# DualFrontier.Runtime.Graphics

**Purpose:** Vulkan rendering primitives — instance, physical/logical device, queue family
selection, validation layer setup. Direct wrappers around Vulkan API with idiomatic C#
lifetimes (`IDisposable`). Shares `VkInstance` / `VkDevice` с `Compute/` (V0.B addition).

**Spec authority:** [VULKAN_SUBSTRATE.md](../../../docs/architecture/VULKAN_SUBSTRATE.md) §2.2 Graphics module.

**Dependencies:** `Native.Vulkan`, `Window` (for surface creation — V0.B), `Diagnostic` (for
validation log — Commit 7).

## V0.A surface

- `VulkanInstance` — VkInstance lifecycle с Vulkan 1.3 API version enforcement (К-L19 surface)
  and validation layer + extension activation in DEBUG mode
- `ValidationLayer` (Commit 7) — debug messenger setup, validation message capture
- `VulkanDevice` (Commit 8) — physical device enumeration + logical device + graphics queue
  family selection

## V0.B surface (deferred)

- `VulkanSurface` (VkSurfaceKHR Win32)
- `VulkanSwapchain` + WM_SIZE-triggered recreation
- `VulkanCommandPool`, `VulkanRenderPass`, `VulkanPipeline`
- `VulkanBuffer`, `VulkanImage`, `MemoryAllocator` (bumper)
- Async compute queue family selection (К10.3 brief Item 43 — К-L19 hardware tier)

## К-L19 enforcement scope (V0.A)

`VulkanInstance` requires Vulkan 1.3 at instance creation time — `vkEnumerateInstanceVersion`
queried; if reported version < `VK_API_VERSION_1_3` throws с diagnostic message referencing
KERNEL_FULL_NATIVE_SCHEDULER.md К-L19 architectural rationale. Hardware capability check at
startup (К10.3 brief Item 44 — `HardwareCapabilityCheck.Verify`) deferred к V0.B alongside
async compute queue selection.
