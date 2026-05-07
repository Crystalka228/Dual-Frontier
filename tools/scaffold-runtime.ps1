# DualFrontier Runtime scaffolding generator
# Generated: 2026-05-07
# Reference: docs/RUNTIME_ARCHITECTURE.md
# Run from: D:\Colony_Simulator\Colony_Simulator\
#
# Generates folder structure, MODULE.md docs, contract architecture
# (interfaces, abstract classes, struct/enum signatures, member declarations)
# для Vulkan-based 2D runtime. NO IMPLEMENTATION CODE — all bodies throw
# NotImplementedException("TODO: M9.x — ...").
#
# Idempotent: safe to re-run. Uses -Force on directory creation; files
# overwritten unconditionally (pure scaffolding).

$ErrorActionPreference = 'Stop'

# === Section 1: Verify working directory =====================================
if (-not (Test-Path "DualFrontier.sln")) {
    Write-Error "Must run from D:\Colony_Simulator\Colony_Simulator\ (DualFrontier.sln not found)"
    exit 1
}

Write-Host "DualFrontier Runtime scaffolding starting..." -ForegroundColor Cyan

# Helper: write file with UTF-8 encoding
function Write-File {
    param([string]$Path, [string]$Content)
    $dir = Split-Path -Parent $Path
    if ($dir -and -not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
    Set-Content -Path $Path -Value $Content -Encoding UTF8 -NoNewline:$false
}

# === Section 2: Create folder structure ======================================
$folders = @(
    "src/DualFrontier.Runtime",
    "src/DualFrontier.Runtime/Native",
    "src/DualFrontier.Runtime/Native/Win32",
    "src/DualFrontier.Runtime/Native/Vulkan",
    "src/DualFrontier.Runtime/Window",
    "src/DualFrontier.Runtime/Input",
    "src/DualFrontier.Runtime/Graphics",
    "src/DualFrontier.Runtime/Sprite",
    "src/DualFrontier.Runtime/Text",
    "src/DualFrontier.Runtime/Assets",
    "src/DualFrontier.Runtime/Diagnostic",
    "tests/DualFrontier.Runtime.Tests",
    "tests/DualFrontier.Runtime.Tests/Assets",
    "tests/DualFrontier.Runtime.Tests/Sprite",
    "tests/DualFrontier.Runtime.Tests/Input",
    "tools/shaders"
)
foreach ($f in $folders) {
    New-Item -ItemType Directory -Path $f -Force | Out-Null
}
Write-Host "Folders created" -ForegroundColor Green

# === Section 3: Generate files ===============================================

# -----------------------------------------------------------------------------
# DualFrontier.Runtime project
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/DualFrontier.Runtime.csproj" @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>DualFrontier.Runtime</RootNamespace>
    <AssemblyName>DualFrontier.Runtime</AssemblyName>
  </PropertyGroup>

</Project>
'@

Write-File "src/DualFrontier.Runtime/MODULE.md" @'
# DualFrontier.Runtime — Module Documentation

**Purpose**: Generic 2D runtime — window management, Vulkan rendering primitives,
sprite batching, texture loading, input events, UI primitives. Knows nothing of
Domain. Could be open-sourced separately.

See `docs/RUNTIME_ARCHITECTURE.md` §1.2 для authoritative description.

## Public API surface

`Runtime.cs` facade exposes:
- Window creation
- Sprite registration
- Frame submission
- Input event polling

## Dependency rules

- Zero references к Domain projects (Core, Contracts, Components, Events,
  Systems, Application, Modding, Persistence).
- BCL only: `System`, `System.Numerics`, `System.IO.Compression`.
- No third-party packages.

## Layer dependency direction

```
Native.Win32 / Native.Vulkan  (lowest)
    ↓
Window / Input / Assets
    ↓
Graphics
    ↓
Sprite / Text
    ↓
Diagnostic
    ↓
Runtime.cs (facade — top)
```

No layer skipping — Diagnostic не imports Native.Vulkan directly; goes через
Graphics.

## Milestones

- **M9.0** — Win32 window + Vulkan clear color (foundation)
- **M9.1** — First textured quad (PNG decoder, sprite shader)
- **M9.2** — Batched sprite renderer (10k sprites @ 60 FPS)
- **M9.3** — TileMap parity + Camera2D
- **M9.4** — Input system (Win32 → InputEventQueue)
- **M9.5** — Domain integration (Presentation port)
- **M9.6** — UI primitives (text + panels)
- **M9.7** — Coupled lifecycle + DebugOverlay
- **M9.8** — Migration cutover (delete Godot)

Reference: `docs/RUNTIME_ARCHITECTURE.md` Part 2 — Roadmap.
'@

Write-File "src/DualFrontier.Runtime/Runtime.cs" @'
namespace DualFrontier.Runtime;

/// <summary>
/// Top-level facade для DualFrontier Runtime. Encapsulates window lifecycle,
/// Vulkan device, sprite batcher, and frame submission.
/// </summary>
/// <remarks>
/// See docs/RUNTIME_ARCHITECTURE.md §1.2 для public API surface contract.
/// TODO: M9.0 — Runtime facade implementation.
/// </remarks>
public sealed class Runtime : IDisposable
{
    /// <summary>
    /// Initializes the runtime: creates window, Vulkan instance, device, swapchain.
    /// </summary>
    public void Initialize()
        => throw new NotImplementedException("TODO: M9.0 — Runtime facade implementation");

    /// <summary>
    /// Runs a single frame: pumps OS messages, drains command queue, submits.
    /// </summary>
    public void RunFrame()
        => throw new NotImplementedException("TODO: M9.0 — Runtime facade implementation");

    /// <summary>
    /// Performs orderly shutdown: waits for device idle, releases all resources.
    /// </summary>
    public void Shutdown()
        => throw new NotImplementedException("TODO: M9.0 — Runtime facade implementation");

    /// <inheritdoc />
    public void Dispose()
        => throw new NotImplementedException("TODO: M9.0 — Runtime facade implementation");
}
'@

# -----------------------------------------------------------------------------
# Native — top-level
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Native/MODULE.md" @'
# Native — Module Documentation

**Purpose**: Pure P/Invoke к OS APIs (Win32) и GPU loader (Vulkan). Internal к
Runtime project. No leakage beyond Native namespace.

Subdirectories:
- `Win32/` — user32.dll / kernel32.dll P/Invoke (window, message pump, input)
- `Vulkan/` — vulkan-1.dll P/Invoke (instance, device, swapchain, commands)

## Dependency rules

- Internal access modifier on all P/Invoke artifacts.
- No dependencies на other Runtime modules.
- Higher layers (Window, Graphics) wrap these in idiomatic C# abstractions.

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.5 — Native interop patterns.
'@

# -----------------------------------------------------------------------------
# Native/Win32
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Native/Win32/MODULE.md" @'
# Native.Win32 — Module Documentation

**Purpose**: Win32 P/Invoke declarations.

## Functions covered

- Window class registration / window creation / destruction
- Message pump (PeekMessage / GetMessage / DispatchMessage)
- Input message routing (WM_KEYDOWN/UP, WM_MOUSE*, WM_*BUTTON*)
- Focus events (WM_KILLFOCUS, WM_SETFOCUS)
- Window resize (WM_SIZE, GetClientRect)

## Pattern

`[LibraryImport]` (C# 11+) over `[DllImport]`. UTF-16 string marshalling для
Wide-API entries (`*W` suffix). `partial` static class enables source-generated
marshalling.

## TODO

- **M9.0** — implement P/Invoke signatures (initial set ~14 functions)
- **M9.4** — extend для raw input если standard message routing insufficient
- **M9.7** — extend для focus / sizing / minimize-restore lifecycle

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.5.
'@

Write-File "src/DualFrontier.Runtime/Native/Win32/Win32Api.cs" @'
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Win32;

/// <summary>
/// P/Invoke declarations для Win32 user32.dll and kernel32.dll.
/// TODO: M9.0 — implement P/Invoke signatures (verify entry points + marshalling).
/// </summary>
internal static partial class Win32Api
{
    private const string User32 = "user32.dll";
    private const string Kernel32 = "kernel32.dll";

    [LibraryImport(User32, EntryPoint = "RegisterClassExW", SetLastError = true)]
    internal static partial ushort RegisterClassEx(in WNDCLASSEX lpwcx);

    [LibraryImport(User32, EntryPoint = "CreateWindowExW", SetLastError = true,
        StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    [LibraryImport(User32, EntryPoint = "DestroyWindow", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyWindow(IntPtr hWnd);

    [LibraryImport(User32, EntryPoint = "ShowWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [LibraryImport(User32, EntryPoint = "UpdateWindow")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UpdateWindow(IntPtr hWnd);

    [LibraryImport(User32, EntryPoint = "PeekMessageW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PeekMessage(
        out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [LibraryImport(User32, EntryPoint = "GetMessageW")]
    internal static partial int GetMessage(
        out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport(User32, EntryPoint = "TranslateMessage")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in MSG lpMsg);

    [LibraryImport(User32, EntryPoint = "DispatchMessageW")]
    internal static partial IntPtr DispatchMessage(in MSG lpMsg);

    [LibraryImport(User32, EntryPoint = "DefWindowProcW")]
    internal static partial IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport(User32, EntryPoint = "PostQuitMessage")]
    internal static partial void PostQuitMessage(int nExitCode);

    [LibraryImport(Kernel32, EntryPoint = "GetModuleHandleW",
        StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    internal static partial IntPtr GetModuleHandle(string? lpModuleName);

    [LibraryImport(User32, EntryPoint = "LoadCursorW", SetLastError = true)]
    internal static partial IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

    [LibraryImport(User32, EntryPoint = "GetClientRect", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetClientRect(IntPtr hWnd, out RECT lpRect);
}
'@

Write-File "src/DualFrontier.Runtime/Native/Win32/Win32Constants.cs" @'
namespace DualFrontier.Runtime.Native.Win32;

/// <summary>
/// Win32 constant values (window messages, styles, classes).
/// TODO: M9.0 — verify constant values match Windows SDK headers.
/// </summary>
internal static class Win32Constants
{
    // Window messages -----------------------------------------------------
    internal const uint WM_NULL = 0x0000;
    internal const uint WM_CREATE = 0x0001;
    internal const uint WM_DESTROY = 0x0002;
    internal const uint WM_SIZE = 0x0005;
    internal const uint WM_SETFOCUS = 0x0007;
    internal const uint WM_KILLFOCUS = 0x0008;
    internal const uint WM_PAINT = 0x000F;
    internal const uint WM_CLOSE = 0x0010;
    internal const uint WM_QUIT = 0x0012;
    internal const uint WM_KEYDOWN = 0x0100;
    internal const uint WM_KEYUP = 0x0101;
    internal const uint WM_MOUSEMOVE = 0x0200;
    internal const uint WM_LBUTTONDOWN = 0x0201;
    internal const uint WM_LBUTTONUP = 0x0202;
    internal const uint WM_RBUTTONDOWN = 0x0204;
    internal const uint WM_RBUTTONUP = 0x0205;
    internal const uint WM_MBUTTONDOWN = 0x0207;
    internal const uint WM_MBUTTONUP = 0x0208;
    internal const uint WM_MOUSEWHEEL = 0x020A;

    // Window styles -------------------------------------------------------
    internal const uint WS_OVERLAPPEDWINDOW = 0x00CF0000;
    internal const uint WS_VISIBLE = 0x10000000;

    // Window class styles -------------------------------------------------
    internal const uint CS_VREDRAW = 0x0001;
    internal const uint CS_HREDRAW = 0x0002;
    internal const uint CS_OWNDC = 0x0020;

    // ShowWindow commands -------------------------------------------------
    internal const int SW_HIDE = 0;
    internal const int SW_SHOW = 5;

    // PeekMessage flags ---------------------------------------------------
    internal const uint PM_NOREMOVE = 0x0000;
    internal const uint PM_REMOVE = 0x0001;
}
'@

Write-File "src/DualFrontier.Runtime/Native/Win32/Win32Structs.cs" @'
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Win32;

/// <summary>
/// Win32 struct layouts.
/// TODO: M9.0 — verify struct field order, sizes match Windows SDK.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEX
{
    public uint cbSize;
    public uint style;
    public IntPtr lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    [MarshalAs(UnmanagedType.LPWStr)] public string? lpszMenuName;
    [MarshalAs(UnmanagedType.LPWStr)] public string? lpszClassName;
    public IntPtr hIconSm;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    public int left;
    public int top;
    public int right;
    public int bottom;
}
'@

Write-File "src/DualFrontier.Runtime/Native/Win32/WindowProc.cs" @'
namespace DualFrontier.Runtime.Native.Win32;

/// <summary>
/// Window procedure delegate signature matching Win32 WNDPROC.
/// </summary>
internal delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

/// <summary>
/// Default window procedure helpers.
/// TODO: M9.0 — implement message dispatching (WM_CLOSE → quit, WM_KEY* → input queue, WM_SIZE → resize).
/// </summary>
internal static class WindowProc
{
    /// <summary>
    /// Default message handler — currently a stub that returns IntPtr.Zero.
    /// Real implementation must dispatch к Win32Api.DefWindowProc для unhandled messages.
    /// </summary>
    internal static IntPtr DefaultProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        => throw new NotImplementedException("TODO: M9.0 — implement message dispatching");
}
'@

# -----------------------------------------------------------------------------
# Native/Vulkan
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Native/Vulkan/MODULE.md" @'
# Native.Vulkan — Module Documentation

**Purpose**: Vulkan 1.3 API P/Invoke declarations.

## Approach

Direct `[LibraryImport("vulkan-1.dll")]` (Option A per
`docs/RUNTIME_ARCHITECTURE.md` §1.5). May migrate к canonical
`vkGetInstanceProcAddr` dispatch в M9.5+ if profiling demands.

## Files

- `VkApi.cs` — function declarations (~70 entries)
- `VkEnums.cs` — enum types (VkResult, VkFormat, etc.)
- `VkStructs.cs` — struct layouts (~40 types)
- `VkConstants.cs` — well-known values + extension name strings
- `VkDelegates.cs` — function pointer signatures для callbacks

## TODO

- **M9.0** — instance + device + swapchain function set
- **M9.1** — buffer + image + memory + pipeline + render pass functions
- **M9.2** — draw command buffer functions, dynamic state
- **M9.4** — sampler + descriptor set functions for texturing extensions

All declarations: signatures only, no bodies. Validation against Vulkan 1.3
spec (https://registry.khronos.org/vulkan/specs/1.3/html/vkspec.html) before
implementation.

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.5.
'@

Write-File "src/DualFrontier.Runtime/Native/Vulkan/VkApi.cs" @'
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

/// <summary>
/// Vulkan 1.3 API P/Invoke declarations.
/// TODO: M9.0-M9.4 — implement P/Invoke signatures matching Vulkan 1.3 spec.
/// All function pointers loaded directly via vulkan-1.dll exports.
/// </summary>
internal static unsafe partial class VkApi
{
    private const string Vk = "vulkan-1.dll";

    // Instance ------------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateInstance(
        in VkInstanceCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pInstance);
    [LibraryImport(Vk)] internal static partial void vkDestroyInstance(
        IntPtr instance, IntPtr pAllocator);

    // Physical device ----------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkEnumeratePhysicalDevices(
        IntPtr instance, ref uint pPhysicalDeviceCount, IntPtr* pPhysicalDevices);
    [LibraryImport(Vk)] internal static partial void vkGetPhysicalDeviceProperties(
        IntPtr physicalDevice, out VkPhysicalDeviceProperties pProperties);
    [LibraryImport(Vk)] internal static partial void vkGetPhysicalDeviceQueueFamilyProperties(
        IntPtr physicalDevice, ref uint pQueueFamilyPropertyCount, IntPtr pQueueFamilyProperties);

    // Logical device -----------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateDevice(
        IntPtr physicalDevice, in VkDeviceCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pDevice);
    [LibraryImport(Vk)] internal static partial void vkDestroyDevice(
        IntPtr device, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial void vkGetDeviceQueue(
        IntPtr device, uint queueFamilyIndex, uint queueIndex, out IntPtr pQueue);

    // Command pool / buffers ---------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateCommandPool(
        IntPtr device, in VkCommandPoolCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pCommandPool);
    [LibraryImport(Vk)] internal static partial void vkDestroyCommandPool(
        IntPtr device, IntPtr commandPool, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkAllocateCommandBuffers(
        IntPtr device, in VkCommandBufferAllocateInfo pAllocateInfo, IntPtr* pCommandBuffers);
    [LibraryImport(Vk)] internal static partial void vkFreeCommandBuffers(
        IntPtr device, IntPtr commandPool, uint commandBufferCount, IntPtr* pCommandBuffers);
    [LibraryImport(Vk)] internal static partial VkResult vkBeginCommandBuffer(
        IntPtr commandBuffer, in VkCommandBufferBeginInfo pBeginInfo);
    [LibraryImport(Vk)] internal static partial VkResult vkEndCommandBuffer(IntPtr commandBuffer);

    // Queue submission ---------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkQueueSubmit(
        IntPtr queue, uint submitCount, in VkSubmitInfo pSubmits, IntPtr fence);
    [LibraryImport(Vk)] internal static partial VkResult vkQueueWaitIdle(IntPtr queue);
    [LibraryImport(Vk)] internal static partial VkResult vkDeviceWaitIdle(IntPtr device);

    // Swapchain ----------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateSwapchainKHR(
        IntPtr device, in VkSwapchainCreateInfoKHR pCreateInfo, IntPtr pAllocator, out IntPtr pSwapchain);
    [LibraryImport(Vk)] internal static partial void vkDestroySwapchainKHR(
        IntPtr device, IntPtr swapchain, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkGetSwapchainImagesKHR(
        IntPtr device, IntPtr swapchain, ref uint pSwapchainImageCount, IntPtr* pSwapchainImages);
    [LibraryImport(Vk)] internal static partial VkResult vkAcquireNextImageKHR(
        IntPtr device, IntPtr swapchain, ulong timeout, IntPtr semaphore, IntPtr fence, out uint pImageIndex);
    [LibraryImport(Vk)] internal static partial VkResult vkQueuePresentKHR(
        IntPtr queue, in VkPresentInfoKHR pPresentInfo);

    // Image / image view -------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateImage(
        IntPtr device, in VkImageCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pImage);
    [LibraryImport(Vk)] internal static partial void vkDestroyImage(
        IntPtr device, IntPtr image, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkBindImageMemory(
        IntPtr device, IntPtr image, IntPtr memory, ulong memoryOffset);
    [LibraryImport(Vk)] internal static partial void vkGetImageMemoryRequirements(
        IntPtr device, IntPtr image, IntPtr pMemoryRequirements);
    [LibraryImport(Vk)] internal static partial VkResult vkCreateImageView(
        IntPtr device, in VkImageViewCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pView);
    [LibraryImport(Vk)] internal static partial void vkDestroyImageView(
        IntPtr device, IntPtr imageView, IntPtr pAllocator);

    // Buffer -------------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateBuffer(
        IntPtr device, in VkBufferCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pBuffer);
    [LibraryImport(Vk)] internal static partial void vkDestroyBuffer(
        IntPtr device, IntPtr buffer, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkBindBufferMemory(
        IntPtr device, IntPtr buffer, IntPtr memory, ulong memoryOffset);
    [LibraryImport(Vk)] internal static partial void vkGetBufferMemoryRequirements(
        IntPtr device, IntPtr buffer, IntPtr pMemoryRequirements);

    // Memory -------------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkAllocateMemory(
        IntPtr device, in VkMemoryAllocateInfo pAllocateInfo, IntPtr pAllocator, out IntPtr pMemory);
    [LibraryImport(Vk)] internal static partial void vkFreeMemory(
        IntPtr device, IntPtr memory, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkMapMemory(
        IntPtr device, IntPtr memory, ulong offset, ulong size, uint flags, out IntPtr ppData);
    [LibraryImport(Vk)] internal static partial void vkUnmapMemory(
        IntPtr device, IntPtr memory);

    // Shader module ------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateShaderModule(
        IntPtr device, in VkShaderModuleCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pShaderModule);
    [LibraryImport(Vk)] internal static partial void vkDestroyShaderModule(
        IntPtr device, IntPtr shaderModule, IntPtr pAllocator);

    // Pipeline -----------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateGraphicsPipelines(
        IntPtr device, IntPtr pipelineCache, uint createInfoCount,
        in VkGraphicsPipelineCreateInfo pCreateInfos, IntPtr pAllocator, IntPtr* pPipelines);
    [LibraryImport(Vk)] internal static partial void vkDestroyPipeline(
        IntPtr device, IntPtr pipeline, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkCreatePipelineLayout(
        IntPtr device, in VkPipelineLayoutCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pPipelineLayout);
    [LibraryImport(Vk)] internal static partial void vkDestroyPipelineLayout(
        IntPtr device, IntPtr pipelineLayout, IntPtr pAllocator);

    // Descriptor sets ----------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateDescriptorSetLayout(
        IntPtr device, in VkDescriptorSetLayoutCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pSetLayout);
    [LibraryImport(Vk)] internal static partial void vkDestroyDescriptorSetLayout(
        IntPtr device, IntPtr descriptorSetLayout, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkCreateDescriptorPool(
        IntPtr device, in VkDescriptorPoolCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pDescriptorPool);
    [LibraryImport(Vk)] internal static partial void vkDestroyDescriptorPool(
        IntPtr device, IntPtr descriptorPool, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkAllocateDescriptorSets(
        IntPtr device, in VkDescriptorSetAllocateInfo pAllocateInfo, IntPtr* pDescriptorSets);
    [LibraryImport(Vk)] internal static partial void vkUpdateDescriptorSets(
        IntPtr device, uint descriptorWriteCount, in VkWriteDescriptorSet pDescriptorWrites,
        uint descriptorCopyCount, IntPtr pDescriptorCopies);

    // Render pass / framebuffer ------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateRenderPass(
        IntPtr device, in VkRenderPassCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pRenderPass);
    [LibraryImport(Vk)] internal static partial void vkDestroyRenderPass(
        IntPtr device, IntPtr renderPass, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkCreateFramebuffer(
        IntPtr device, in VkFramebufferCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pFramebuffer);
    [LibraryImport(Vk)] internal static partial void vkDestroyFramebuffer(
        IntPtr device, IntPtr framebuffer, IntPtr pAllocator);

    // Synchronization ----------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateSemaphore(
        IntPtr device, IntPtr pCreateInfo, IntPtr pAllocator, out IntPtr pSemaphore);
    [LibraryImport(Vk)] internal static partial void vkDestroySemaphore(
        IntPtr device, IntPtr semaphore, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkCreateFence(
        IntPtr device, IntPtr pCreateInfo, IntPtr pAllocator, out IntPtr pFence);
    [LibraryImport(Vk)] internal static partial void vkDestroyFence(
        IntPtr device, IntPtr fence, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkWaitForFences(
        IntPtr device, uint fenceCount, in IntPtr pFences,
        [MarshalAs(UnmanagedType.Bool)] bool waitAll, ulong timeout);
    [LibraryImport(Vk)] internal static partial VkResult vkResetFences(
        IntPtr device, uint fenceCount, in IntPtr pFences);

    // Command buffer recording -------------------------------------------
    [LibraryImport(Vk)] internal static partial void vkCmdBeginRenderPass(
        IntPtr commandBuffer, IntPtr pRenderPassBegin, uint contents);
    [LibraryImport(Vk)] internal static partial void vkCmdEndRenderPass(IntPtr commandBuffer);
    [LibraryImport(Vk)] internal static partial void vkCmdBindPipeline(
        IntPtr commandBuffer, uint pipelineBindPoint, IntPtr pipeline);
    [LibraryImport(Vk)] internal static partial void vkCmdBindDescriptorSets(
        IntPtr commandBuffer, uint pipelineBindPoint, IntPtr layout,
        uint firstSet, uint descriptorSetCount, in IntPtr pDescriptorSets,
        uint dynamicOffsetCount, IntPtr pDynamicOffsets);
    [LibraryImport(Vk)] internal static partial void vkCmdBindVertexBuffers(
        IntPtr commandBuffer, uint firstBinding, uint bindingCount,
        in IntPtr pBuffers, in ulong pOffsets);
    [LibraryImport(Vk)] internal static partial void vkCmdBindIndexBuffer(
        IntPtr commandBuffer, IntPtr buffer, ulong offset, uint indexType);
    [LibraryImport(Vk)] internal static partial void vkCmdDraw(
        IntPtr commandBuffer, uint vertexCount, uint instanceCount, uint firstVertex, uint firstInstance);
    [LibraryImport(Vk)] internal static partial void vkCmdDrawIndexed(
        IntPtr commandBuffer, uint indexCount, uint instanceCount,
        uint firstIndex, int vertexOffset, uint firstInstance);
    [LibraryImport(Vk)] internal static partial void vkCmdClearColorImage(
        IntPtr commandBuffer, IntPtr image, uint imageLayout,
        IntPtr pColor, uint rangeCount, IntPtr pRanges);
    [LibraryImport(Vk)] internal static partial void vkCmdPipelineBarrier(
        IntPtr commandBuffer, uint srcStageMask, uint dstStageMask, uint dependencyFlags,
        uint memoryBarrierCount, IntPtr pMemoryBarriers,
        uint bufferMemoryBarrierCount, IntPtr pBufferMemoryBarriers,
        uint imageMemoryBarrierCount, IntPtr pImageMemoryBarriers);
    [LibraryImport(Vk)] internal static partial void vkCmdSetViewport(
        IntPtr commandBuffer, uint firstViewport, uint viewportCount, in VkViewport pViewports);
    [LibraryImport(Vk)] internal static partial void vkCmdSetScissor(
        IntPtr commandBuffer, uint firstScissor, uint scissorCount, in VkRect2D pScissors);
    [LibraryImport(Vk)] internal static partial void vkCmdCopyBufferToImage(
        IntPtr commandBuffer, IntPtr srcBuffer, IntPtr dstImage,
        uint dstImageLayout, uint regionCount, IntPtr pRegions);
    [LibraryImport(Vk)] internal static partial void vkCmdCopyBuffer(
        IntPtr commandBuffer, IntPtr srcBuffer, IntPtr dstBuffer,
        uint regionCount, IntPtr pRegions);

    // Win32 surface (Windows-specific) -----------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateWin32SurfaceKHR(
        IntPtr instance, in VkWin32SurfaceCreateInfoKHR pCreateInfo, IntPtr pAllocator, out IntPtr pSurface);
    [LibraryImport(Vk)] internal static partial void vkDestroySurfaceKHR(
        IntPtr instance, IntPtr surface, IntPtr pAllocator);
    [LibraryImport(Vk)] internal static partial VkResult vkGetPhysicalDeviceSurfaceCapabilitiesKHR(
        IntPtr physicalDevice, IntPtr surface, out VkSurfaceCapabilitiesKHR pSurfaceCapabilities);
    [LibraryImport(Vk)] internal static partial VkResult vkGetPhysicalDeviceSurfaceFormatsKHR(
        IntPtr physicalDevice, IntPtr surface, ref uint pSurfaceFormatCount, IntPtr pSurfaceFormats);
    [LibraryImport(Vk)] internal static partial VkResult vkGetPhysicalDeviceSurfacePresentModesKHR(
        IntPtr physicalDevice, IntPtr surface, ref uint pPresentModeCount, IntPtr pPresentModes);
    [LibraryImport(Vk)] internal static partial VkResult vkGetPhysicalDeviceSurfaceSupportKHR(
        IntPtr physicalDevice, uint queueFamilyIndex, IntPtr surface,
        [MarshalAs(UnmanagedType.Bool)] out bool pSupported);

    // Sampler ------------------------------------------------------------
    [LibraryImport(Vk)] internal static partial VkResult vkCreateSampler(
        IntPtr device, in VkSamplerCreateInfo pCreateInfo, IntPtr pAllocator, out IntPtr pSampler);
    [LibraryImport(Vk)] internal static partial void vkDestroySampler(
        IntPtr device, IntPtr sampler, IntPtr pAllocator);

    // Debug utils (extension functions — must be loaded via vkGetInstanceProcAddr) -------
    // Stubs below kept for symmetry; production code must resolve via procedure address.
    [LibraryImport(Vk)] internal static partial VkResult vkCreateDebugUtilsMessengerEXT(
        IntPtr instance, in VkDebugUtilsMessengerCreateInfoEXT pCreateInfo, IntPtr pAllocator, out IntPtr pMessenger);
    [LibraryImport(Vk)] internal static partial void vkDestroyDebugUtilsMessengerEXT(
        IntPtr instance, IntPtr messenger, IntPtr pAllocator);
}
'@

Write-File "src/DualFrontier.Runtime/Native/Vulkan/VkEnums.cs" @'
namespace DualFrontier.Runtime.Native.Vulkan;

// TODO: M9.0-M9.4 — verify enum values match Vulkan 1.3 spec.
// Reference: https://registry.khronos.org/vulkan/specs/1.3/html/vkspec.html

internal enum VkResult
{
    VK_SUCCESS = 0,
    VK_NOT_READY = 1,
    VK_TIMEOUT = 2,
    VK_EVENT_SET = 3,
    VK_EVENT_RESET = 4,
    VK_INCOMPLETE = 5,
    VK_ERROR_OUT_OF_HOST_MEMORY = -1,
    VK_ERROR_OUT_OF_DEVICE_MEMORY = -2,
    VK_ERROR_INITIALIZATION_FAILED = -3,
    VK_ERROR_DEVICE_LOST = -4,
    VK_ERROR_MEMORY_MAP_FAILED = -5,
    VK_ERROR_LAYER_NOT_PRESENT = -6,
    VK_ERROR_EXTENSION_NOT_PRESENT = -7,
    VK_ERROR_FEATURE_NOT_PRESENT = -8,
    VK_ERROR_INCOMPATIBLE_DRIVER = -9,
    VK_ERROR_TOO_MANY_OBJECTS = -10,
    VK_ERROR_FORMAT_NOT_SUPPORTED = -11,
    VK_ERROR_FRAGMENTED_POOL = -12,
    VK_ERROR_UNKNOWN = -13,
    VK_ERROR_OUT_OF_DATE_KHR = -1000001004,
    VK_SUBOPTIMAL_KHR = 1000001003,
}

internal enum VkFormat
{
    VK_FORMAT_UNDEFINED = 0,
    VK_FORMAT_R8G8B8A8_UNORM = 37,
    VK_FORMAT_B8G8R8A8_UNORM = 44,
    VK_FORMAT_R8G8B8A8_SRGB = 43,
    VK_FORMAT_B8G8R8A8_SRGB = 50,
    VK_FORMAT_R32_SFLOAT = 100,
    VK_FORMAT_R32G32_SFLOAT = 103,
    VK_FORMAT_R32G32B32_SFLOAT = 106,
    VK_FORMAT_R32G32B32A32_SFLOAT = 109,
    VK_FORMAT_D32_SFLOAT = 126,
}

internal enum VkImageLayout
{
    VK_IMAGE_LAYOUT_UNDEFINED = 0,
    VK_IMAGE_LAYOUT_GENERAL = 1,
    VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL = 2,
    VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL = 3,
    VK_IMAGE_LAYOUT_DEPTH_STENCIL_READ_ONLY_OPTIMAL = 4,
    VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL = 5,
    VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL = 6,
    VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL = 7,
    VK_IMAGE_LAYOUT_PREINITIALIZED = 8,
    VK_IMAGE_LAYOUT_PRESENT_SRC_KHR = 1000001002,
}

internal enum VkImageType
{
    VK_IMAGE_TYPE_1D = 0,
    VK_IMAGE_TYPE_2D = 1,
    VK_IMAGE_TYPE_3D = 2,
}

internal enum VkImageTiling
{
    VK_IMAGE_TILING_OPTIMAL = 0,
    VK_IMAGE_TILING_LINEAR = 1,
}

[Flags]
internal enum VkImageUsageFlags : uint
{
    VK_IMAGE_USAGE_TRANSFER_SRC_BIT = 0x00000001,
    VK_IMAGE_USAGE_TRANSFER_DST_BIT = 0x00000002,
    VK_IMAGE_USAGE_SAMPLED_BIT = 0x00000004,
    VK_IMAGE_USAGE_STORAGE_BIT = 0x00000008,
    VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT = 0x00000010,
    VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 0x00000020,
}

[Flags]
internal enum VkImageAspectFlags : uint
{
    VK_IMAGE_ASPECT_COLOR_BIT = 0x00000001,
    VK_IMAGE_ASPECT_DEPTH_BIT = 0x00000002,
    VK_IMAGE_ASPECT_STENCIL_BIT = 0x00000004,
}

[Flags]
internal enum VkBufferUsageFlags : uint
{
    VK_BUFFER_USAGE_TRANSFER_SRC_BIT = 0x00000001,
    VK_BUFFER_USAGE_TRANSFER_DST_BIT = 0x00000002,
    VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT = 0x00000004,
    VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT = 0x00000008,
    VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT = 0x00000010,
    VK_BUFFER_USAGE_STORAGE_BUFFER_BIT = 0x00000020,
    VK_BUFFER_USAGE_INDEX_BUFFER_BIT = 0x00000040,
    VK_BUFFER_USAGE_VERTEX_BUFFER_BIT = 0x00000080,
    VK_BUFFER_USAGE_INDIRECT_BUFFER_BIT = 0x00000100,
}

internal enum VkSharingMode
{
    VK_SHARING_MODE_EXCLUSIVE = 0,
    VK_SHARING_MODE_CONCURRENT = 1,
}

[Flags]
internal enum VkPipelineStageFlags : uint
{
    VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT = 0x00000001,
    VK_PIPELINE_STAGE_VERTEX_INPUT_BIT = 0x00000008,
    VK_PIPELINE_STAGE_VERTEX_SHADER_BIT = 0x00000010,
    VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT = 0x00000080,
    VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT = 0x00000400,
    VK_PIPELINE_STAGE_TRANSFER_BIT = 0x00001000,
    VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT = 0x00002000,
    VK_PIPELINE_STAGE_ALL_GRAPHICS_BIT = 0x00008000,
    VK_PIPELINE_STAGE_ALL_COMMANDS_BIT = 0x00010000,
}

[Flags]
internal enum VkAccessFlags : uint
{
    VK_ACCESS_INDIRECT_COMMAND_READ_BIT = 0x00000001,
    VK_ACCESS_INDEX_READ_BIT = 0x00000002,
    VK_ACCESS_VERTEX_ATTRIBUTE_READ_BIT = 0x00000004,
    VK_ACCESS_UNIFORM_READ_BIT = 0x00000008,
    VK_ACCESS_SHADER_READ_BIT = 0x00000020,
    VK_ACCESS_SHADER_WRITE_BIT = 0x00000040,
    VK_ACCESS_COLOR_ATTACHMENT_READ_BIT = 0x00000080,
    VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT = 0x00000100,
    VK_ACCESS_TRANSFER_READ_BIT = 0x00000800,
    VK_ACCESS_TRANSFER_WRITE_BIT = 0x00001000,
}

[Flags]
internal enum VkMemoryPropertyFlags : uint
{
    VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT = 0x00000001,
    VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT = 0x00000002,
    VK_MEMORY_PROPERTY_HOST_COHERENT_BIT = 0x00000004,
    VK_MEMORY_PROPERTY_HOST_CACHED_BIT = 0x00000008,
}

internal enum VkPresentModeKHR
{
    VK_PRESENT_MODE_IMMEDIATE_KHR = 0,
    VK_PRESENT_MODE_MAILBOX_KHR = 1,
    VK_PRESENT_MODE_FIFO_KHR = 2,
    VK_PRESENT_MODE_FIFO_RELAXED_KHR = 3,
}

internal enum VkColorSpaceKHR
{
    VK_COLOR_SPACE_SRGB_NONLINEAR_KHR = 0,
}

internal enum VkPrimitiveTopology
{
    VK_PRIMITIVE_TOPOLOGY_POINT_LIST = 0,
    VK_PRIMITIVE_TOPOLOGY_LINE_LIST = 1,
    VK_PRIMITIVE_TOPOLOGY_LINE_STRIP = 2,
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST = 3,
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP = 4,
    VK_PRIMITIVE_TOPOLOGY_TRIANGLE_FAN = 5,
}

internal enum VkPolygonMode
{
    VK_POLYGON_MODE_FILL = 0,
    VK_POLYGON_MODE_LINE = 1,
    VK_POLYGON_MODE_POINT = 2,
}

[Flags]
internal enum VkCullModeFlags : uint
{
    VK_CULL_MODE_NONE = 0,
    VK_CULL_MODE_FRONT_BIT = 0x00000001,
    VK_CULL_MODE_BACK_BIT = 0x00000002,
    VK_CULL_MODE_FRONT_AND_BACK = 0x00000003,
}

internal enum VkFrontFace
{
    VK_FRONT_FACE_COUNTER_CLOCKWISE = 0,
    VK_FRONT_FACE_CLOCKWISE = 1,
}

[Flags]
internal enum VkSampleCountFlags : uint
{
    VK_SAMPLE_COUNT_1_BIT = 0x00000001,
    VK_SAMPLE_COUNT_2_BIT = 0x00000002,
    VK_SAMPLE_COUNT_4_BIT = 0x00000004,
    VK_SAMPLE_COUNT_8_BIT = 0x00000008,
}

internal enum VkBlendFactor
{
    VK_BLEND_FACTOR_ZERO = 0,
    VK_BLEND_FACTOR_ONE = 1,
    VK_BLEND_FACTOR_SRC_ALPHA = 6,
    VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA = 7,
    VK_BLEND_FACTOR_DST_ALPHA = 8,
    VK_BLEND_FACTOR_ONE_MINUS_DST_ALPHA = 9,
}

internal enum VkBlendOp
{
    VK_BLEND_OP_ADD = 0,
    VK_BLEND_OP_SUBTRACT = 1,
    VK_BLEND_OP_REVERSE_SUBTRACT = 2,
    VK_BLEND_OP_MIN = 3,
    VK_BLEND_OP_MAX = 4,
}

internal enum VkDescriptorType
{
    VK_DESCRIPTOR_TYPE_SAMPLER = 0,
    VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER = 1,
    VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE = 2,
    VK_DESCRIPTOR_TYPE_STORAGE_IMAGE = 3,
    VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER = 6,
    VK_DESCRIPTOR_TYPE_STORAGE_BUFFER = 7,
}

[Flags]
internal enum VkShaderStageFlags : uint
{
    VK_SHADER_STAGE_VERTEX_BIT = 0x00000001,
    VK_SHADER_STAGE_FRAGMENT_BIT = 0x00000010,
    VK_SHADER_STAGE_COMPUTE_BIT = 0x00000020,
    VK_SHADER_STAGE_ALL_GRAPHICS = 0x0000001F,
    VK_SHADER_STAGE_ALL = 0x7FFFFFFF,
}

internal enum VkFilter
{
    VK_FILTER_NEAREST = 0,
    VK_FILTER_LINEAR = 1,
}

internal enum VkSamplerAddressMode
{
    VK_SAMPLER_ADDRESS_MODE_REPEAT = 0,
    VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT = 1,
    VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE = 2,
    VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER = 3,
}
'@

Write-File "src/DualFrontier.Runtime/Native/Vulkan/VkStructs.cs" @'
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

// TODO: M9.x — verify field layouts match Vulkan 1.3 spec.
// All structs use [StructLayout(LayoutKind.Sequential)] для blittable interop.

[StructLayout(LayoutKind.Sequential)]
internal struct VkApplicationInfo
{
    public uint sType;
    public IntPtr pNext;
    public IntPtr pApplicationName;
    public uint applicationVersion;
    public IntPtr pEngineName;
    public uint engineVersion;
    public uint apiVersion;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkInstanceCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr pApplicationInfo;
    public uint enabledLayerCount;
    public IntPtr ppEnabledLayerNames;
    public uint enabledExtensionCount;
    public IntPtr ppEnabledExtensionNames;
}

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct VkPhysicalDeviceProperties
{
    public uint apiVersion;
    public uint driverVersion;
    public uint vendorID;
    public uint deviceID;
    public uint deviceType;
    public fixed byte deviceName[256];
    public fixed byte pipelineCacheUUID[16];
    // Note: limits/sparseProperties fields omitted for brevity; add when needed.
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPhysicalDeviceFeatures
{
    public uint robustBufferAccess;
    public uint fullDrawIndexUint32;
    public uint imageCubeArray;
    public uint independentBlend;
    public uint geometryShader;
    public uint tessellationShader;
    public uint sampleRateShading;
    // Truncated: full struct has 55 features. Add as needed.
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDeviceCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint queueCreateInfoCount;
    public IntPtr pQueueCreateInfos;
    public uint enabledLayerCount;
    public IntPtr ppEnabledLayerNames;
    public uint enabledExtensionCount;
    public IntPtr ppEnabledExtensionNames;
    public IntPtr pEnabledFeatures;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDeviceQueueCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint queueFamilyIndex;
    public uint queueCount;
    public IntPtr pQueuePriorities;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSurfaceCapabilitiesKHR
{
    public uint minImageCount;
    public uint maxImageCount;
    public VkExtent2D currentExtent;
    public VkExtent2D minImageExtent;
    public VkExtent2D maxImageExtent;
    public uint maxImageArrayLayers;
    public uint supportedTransforms;
    public uint currentTransform;
    public uint supportedCompositeAlpha;
    public uint supportedUsageFlags;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSurfaceFormatKHR
{
    public VkFormat format;
    public VkColorSpaceKHR colorSpace;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSwapchainCreateInfoKHR
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr surface;
    public uint minImageCount;
    public VkFormat imageFormat;
    public VkColorSpaceKHR imageColorSpace;
    public VkExtent2D imageExtent;
    public uint imageArrayLayers;
    public VkImageUsageFlags imageUsage;
    public VkSharingMode imageSharingMode;
    public uint queueFamilyIndexCount;
    public IntPtr pQueueFamilyIndices;
    public uint preTransform;
    public uint compositeAlpha;
    public VkPresentModeKHR presentMode;
    public uint clipped;
    public IntPtr oldSwapchain;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkImageCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public VkImageType imageType;
    public VkFormat format;
    public VkExtent3D extent;
    public uint mipLevels;
    public uint arrayLayers;
    public VkSampleCountFlags samples;
    public VkImageTiling tiling;
    public VkImageUsageFlags usage;
    public VkSharingMode sharingMode;
    public uint queueFamilyIndexCount;
    public IntPtr pQueueFamilyIndices;
    public VkImageLayout initialLayout;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkImageViewCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr image;
    public uint viewType;
    public VkFormat format;
    public uint componentsR;
    public uint componentsG;
    public uint componentsB;
    public uint componentsA;
    public VkImageAspectFlags subresourceRangeAspectMask;
    public uint subresourceRangeBaseMipLevel;
    public uint subresourceRangeLevelCount;
    public uint subresourceRangeBaseArrayLayer;
    public uint subresourceRangeLayerCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkBufferCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public ulong size;
    public VkBufferUsageFlags usage;
    public VkSharingMode sharingMode;
    public uint queueFamilyIndexCount;
    public IntPtr pQueueFamilyIndices;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkMemoryAllocateInfo
{
    public uint sType;
    public IntPtr pNext;
    public ulong allocationSize;
    public uint memoryTypeIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSubmitInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint waitSemaphoreCount;
    public IntPtr pWaitSemaphores;
    public IntPtr pWaitDstStageMask;
    public uint commandBufferCount;
    public IntPtr pCommandBuffers;
    public uint signalSemaphoreCount;
    public IntPtr pSignalSemaphores;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPresentInfoKHR
{
    public uint sType;
    public IntPtr pNext;
    public uint waitSemaphoreCount;
    public IntPtr pWaitSemaphores;
    public uint swapchainCount;
    public IntPtr pSwapchains;
    public IntPtr pImageIndices;
    public IntPtr pResults;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkRenderPassCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint attachmentCount;
    public IntPtr pAttachments;
    public uint subpassCount;
    public IntPtr pSubpasses;
    public uint dependencyCount;
    public IntPtr pDependencies;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkFramebufferCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr renderPass;
    public uint attachmentCount;
    public IntPtr pAttachments;
    public uint width;
    public uint height;
    public uint layers;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPipelineLayoutCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint setLayoutCount;
    public IntPtr pSetLayouts;
    public uint pushConstantRangeCount;
    public IntPtr pPushConstantRanges;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkGraphicsPipelineCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint stageCount;
    public IntPtr pStages;
    public IntPtr pVertexInputState;
    public IntPtr pInputAssemblyState;
    public IntPtr pTessellationState;
    public IntPtr pViewportState;
    public IntPtr pRasterizationState;
    public IntPtr pMultisampleState;
    public IntPtr pDepthStencilState;
    public IntPtr pColorBlendState;
    public IntPtr pDynamicState;
    public IntPtr layout;
    public IntPtr renderPass;
    public uint subpass;
    public IntPtr basePipelineHandle;
    public int basePipelineIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkShaderModuleCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public nuint codeSize;
    public IntPtr pCode;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorSetLayoutCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint bindingCount;
    public IntPtr pBindings;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorPoolCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint maxSets;
    public uint poolSizeCount;
    public IntPtr pPoolSizes;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDescriptorSetAllocateInfo
{
    public uint sType;
    public IntPtr pNext;
    public IntPtr descriptorPool;
    public uint descriptorSetCount;
    public IntPtr pSetLayouts;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkWriteDescriptorSet
{
    public uint sType;
    public IntPtr pNext;
    public IntPtr dstSet;
    public uint dstBinding;
    public uint dstArrayElement;
    public uint descriptorCount;
    public VkDescriptorType descriptorType;
    public IntPtr pImageInfo;
    public IntPtr pBufferInfo;
    public IntPtr pTexelBufferView;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkCommandPoolCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint queueFamilyIndex;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkCommandBufferAllocateInfo
{
    public uint sType;
    public IntPtr pNext;
    public IntPtr commandPool;
    public uint level;
    public uint commandBufferCount;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkCommandBufferBeginInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr pInheritanceInfo;
}

[StructLayout(LayoutKind.Explicit)]
internal struct VkClearValue
{
    [FieldOffset(0)] public float color0;
    [FieldOffset(4)] public float color1;
    [FieldOffset(8)] public float color2;
    [FieldOffset(12)] public float color3;
    [FieldOffset(0)] public float depth;
    [FieldOffset(4)] public uint stencil;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkRect2D
{
    public VkOffset2D offset;
    public VkExtent2D extent;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkOffset2D
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent2D
{
    public uint width;
    public uint height;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkExtent3D
{
    public uint width;
    public uint height;
    public uint depth;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkViewport
{
    public float x;
    public float y;
    public float width;
    public float height;
    public float minDepth;
    public float maxDepth;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputBindingDescription
{
    public uint binding;
    public uint stride;
    public uint inputRate;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkVertexInputAttributeDescription
{
    public uint location;
    public uint binding;
    public VkFormat format;
    public uint offset;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkPushConstantRange
{
    public VkShaderStageFlags stageFlags;
    public uint offset;
    public uint size;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkSamplerCreateInfo
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public VkFilter magFilter;
    public VkFilter minFilter;
    public uint mipmapMode;
    public VkSamplerAddressMode addressModeU;
    public VkSamplerAddressMode addressModeV;
    public VkSamplerAddressMode addressModeW;
    public float mipLodBias;
    public uint anisotropyEnable;
    public float maxAnisotropy;
    public uint compareEnable;
    public uint compareOp;
    public float minLod;
    public float maxLod;
    public uint borderColor;
    public uint unnormalizedCoordinates;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkWin32SurfaceCreateInfoKHR
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public IntPtr hinstance;
    public IntPtr hwnd;
}

[StructLayout(LayoutKind.Sequential)]
internal struct VkDebugUtilsMessengerCreateInfoEXT
{
    public uint sType;
    public IntPtr pNext;
    public uint flags;
    public uint messageSeverity;
    public uint messageType;
    public IntPtr pfnUserCallback;
    public IntPtr pUserData;
}
'@

Write-File "src/DualFrontier.Runtime/Native/Vulkan/VkConstants.cs" @'
namespace DualFrontier.Runtime.Native.Vulkan;

/// <summary>
/// Vulkan well-known constants and extension/layer name strings.
/// TODO: M9.0 — verify against vulkan_core.h for current SDK.
/// </summary>
internal static class VkConstants
{
    // Versioning ----------------------------------------------------------
    // VK_MAKE_API_VERSION(0, 1, 3, 0) = (1 << 22) | (3 << 12) | 0
    internal const uint VK_API_VERSION_1_3 = (1u << 22) | (3u << 12) | 0u;

    // Sentinels -----------------------------------------------------------
    internal static readonly IntPtr VK_NULL_HANDLE = IntPtr.Zero;
    internal const uint VK_TRUE = 1;
    internal const uint VK_FALSE = 0;
    internal const uint VK_QUEUE_FAMILY_IGNORED = ~0u;
    internal const uint VK_SUBPASS_EXTERNAL = ~0u;
    internal const ulong VK_WHOLE_SIZE = ~0ul;
    internal const uint VK_REMAINING_MIP_LEVELS = ~0u;
    internal const uint VK_REMAINING_ARRAY_LAYERS = ~0u;

    // Extension names -----------------------------------------------------
    internal const string VK_KHR_SURFACE_EXTENSION_NAME = "VK_KHR_surface";
    internal const string VK_KHR_WIN32_SURFACE_EXTENSION_NAME = "VK_KHR_win32_surface";
    internal const string VK_KHR_SWAPCHAIN_EXTENSION_NAME = "VK_KHR_swapchain";
    internal const string VK_EXT_DEBUG_UTILS_EXTENSION_NAME = "VK_EXT_debug_utils";

    // Layer names ---------------------------------------------------------
    internal const string VK_LAYER_KHRONOS_VALIDATION_NAME = "VK_LAYER_KHRONOS_validation";
}
'@

Write-File "src/DualFrontier.Runtime/Native/Vulkan/VkDelegates.cs" @'
using System.Runtime.InteropServices;

namespace DualFrontier.Runtime.Native.Vulkan;

/// <summary>
/// Function pointer signatures for Vulkan callbacks.
/// TODO: M9.0 — wire validation callback к ValidationLayer.cs.
/// </summary>
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal delegate uint VkDebugUtilsMessengerCallbackEXT(
    uint messageSeverity,
    uint messageTypes,
    IntPtr pCallbackData,
    IntPtr pUserData);
'@

# -----------------------------------------------------------------------------
# Window
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Window/MODULE.md" @'
# Window — Module Documentation

**Purpose**: High-level window abstraction. Hides Win32 details. Provides
lifecycle (create / show / destroy), event subscription, input event delivery.

## Public API surface

- `IWindow` — contract
- `Window` — Win32 implementation
- `WindowOptions` — creation parameters
- `InputEventQueue` — raw OS events → typed events bus

## Dependencies

- `Native.Win32` (P/Invoke layer)
- `Input` (typed event types)

## TODO

- **M9.0** — Window.cs: WNDCLASSEX registration, CreateWindowEx, message pump skeleton
- **M9.4** — full input event routing through InputEventQueue
- **M9.7** — focus events, WM_SIZE swapchain trigger, lifecycle coupling

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.2.
'@

Write-File "src/DualFrontier.Runtime/Window/IWindow.cs" @'
namespace DualFrontier.Runtime.Window;

/// <summary>
/// Contract for an OS-native window. Implementations wrap platform-specific
/// window handles and message pumps (e.g., Win32 HWND).
/// </summary>
public interface IWindow
{
    /// <summary>Current client width in pixels.</summary>
    int Width { get; }

    /// <summary>Current client height in pixels.</summary>
    int Height { get; }

    /// <summary>True if the window has been closed (WM_CLOSE / Close()).</summary>
    bool IsClosed { get; }

    /// <summary>Native OS handle (HWND on Windows). Required for Vulkan surface creation.</summary>
    IntPtr NativeHandle { get; }

    /// <summary>Makes the window visible.</summary>
    void Show();

    /// <summary>Requests window closure (posts WM_CLOSE on Windows).</summary>
    void Close();

    /// <summary>Drains pending OS messages once. Call once per frame on the main thread.</summary>
    void PumpMessages();

    /// <summary>Raised when client area is resized (new width, new height).</summary>
    event Action<int, int>? Resized;

    /// <summary>Raised when the user closes the window.</summary>
    event Action? Closed;

    /// <summary>Raised when focus state changes (true = focus gained, false = lost).</summary>
    event Action<bool>? FocusChanged;
}
'@

Write-File "src/DualFrontier.Runtime/Window/Window.cs" @'
namespace DualFrontier.Runtime.Window;

/// <summary>
/// Win32 implementation of <see cref="IWindow"/>.
/// TODO: M9.0 — Window Win32 implementation (WNDCLASSEX + CreateWindowEx + message pump).
/// </summary>
public sealed class Window : IWindow, IDisposable
{
    /// <summary>Creates a window using the supplied options. Window is not visible until <see cref="Show"/> is called.</summary>
    public Window(WindowOptions options)
        => throw new NotImplementedException("TODO: M9.0 — Window constructor (register class + CreateWindowEx)");

    /// <inheritdoc />
    public int Width
        => throw new NotImplementedException("TODO: M9.0 — track client width via WM_SIZE");

    /// <inheritdoc />
    public int Height
        => throw new NotImplementedException("TODO: M9.0 — track client height via WM_SIZE");

    /// <inheritdoc />
    public bool IsClosed
        => throw new NotImplementedException("TODO: M9.0 — track closed state via WM_CLOSE / WM_DESTROY");

    /// <inheritdoc />
    public IntPtr NativeHandle
        => throw new NotImplementedException("TODO: M9.0 — return HWND");

    /// <inheritdoc />
    public event Action<int, int>? Resized;

    /// <inheritdoc />
    public event Action? Closed;

    /// <inheritdoc />
    public event Action<bool>? FocusChanged;

    /// <inheritdoc />
    public void Show()
        => throw new NotImplementedException("TODO: M9.0 — ShowWindow(SW_SHOW) + UpdateWindow");

    /// <inheritdoc />
    public void Close()
        => throw new NotImplementedException("TODO: M9.0 — PostMessage(WM_CLOSE)");

    /// <inheritdoc />
    public void PumpMessages()
        => throw new NotImplementedException("TODO: M9.0 — PeekMessage/Translate/Dispatch loop");

    /// <inheritdoc />
    public void Dispose()
        => throw new NotImplementedException("TODO: M9.0 — DestroyWindow + UnregisterClass");

    private void RaiseResized(int w, int h) => Resized?.Invoke(w, h);
    private void RaiseClosed() => Closed?.Invoke();
    private void RaiseFocusChanged(bool focused) => FocusChanged?.Invoke(focused);
}
'@

Write-File "src/DualFrontier.Runtime/Window/WindowOptions.cs" @'
namespace DualFrontier.Runtime.Window;

/// <summary>
/// Window creation parameters.
/// </summary>
/// <param name="Title">Window title bar text.</param>
/// <param name="Width">Initial client area width in pixels.</param>
/// <param name="Height">Initial client area height in pixels.</param>
public sealed record WindowOptions(
    string Title = "Dual Frontier",
    int Width = 800,
    int Height = 600);
'@

Write-File "src/DualFrontier.Runtime/Window/InputEventQueue.cs" @'
using DualFrontier.Runtime.Input;

namespace DualFrontier.Runtime.Window;

/// <summary>
/// Thread-safe queue для cross-thread input event delivery. Window thread
/// enqueues; Domain thread polls via TryDequeue.
/// TODO: M9.4 — back с ConcurrentQueue&lt;IInputEvent&gt;.
/// </summary>
public sealed class InputEventQueue
{
    /// <summary>Number of events currently buffered.</summary>
    public int Count
        => throw new NotImplementedException("TODO: M9.4 — ConcurrentQueue.Count");

    /// <summary>Posts an event from the producer (window) thread.</summary>
    public void Enqueue(IInputEvent evt)
        => throw new NotImplementedException("TODO: M9.4 — ConcurrentQueue.Enqueue");

    /// <summary>Attempts to dequeue an event from the consumer thread.</summary>
    public bool TryDequeue(out IInputEvent? evt)
        => throw new NotImplementedException("TODO: M9.4 — ConcurrentQueue.TryDequeue");
}
'@

# -----------------------------------------------------------------------------
# Input
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Input/MODULE.md" @'
# Input — Module Documentation

**Purpose**: Typed input events + marker interface. Events are produced by the
Window layer (Win32 message routing) and consumed by Domain (через bridge in
Presentation).

## Event types

- `KeyPressedEvent`, `KeyReleasedEvent` — keyboard
- `MouseMovedEvent`, `MouseButtonEvent`, `MouseWheelEvent` — pointer
- `WindowResizedEvent`, `WindowFocusEvent` — lifecycle

## Enums

- `Key` — virtual key set (A-Z, D0-D9, F1-F12, navigation, modifiers)
- `MouseButton` — Left/Right/Middle/X1/X2

## TODO

- **M9.4** — full event implementation; map Win32 VK_* codes к Key enum.

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.3.
'@

Write-File "src/DualFrontier.Runtime/Input/IInputEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Marker interface for all input events.</summary>
public interface IInputEvent
{
}
'@

Write-File "src/DualFrontier.Runtime/Input/Key.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>
/// Logical keyboard key identifiers.
/// TODO: M9.4 — map к Win32 VK_* virtual key codes.
/// </summary>
public enum Key
{
    Unknown = 0,

    A, B, C, D, E, F, G, H, I, J, K, L, M,
    N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

    D0, D1, D2, D3, D4, D5, D6, D7, D8, D9,

    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

    Escape,
    Space,
    Enter,
    Tab,
    Backspace,

    LeftShift, RightShift,
    LeftCtrl, RightCtrl,
    LeftAlt, RightAlt,

    ArrowLeft, ArrowRight, ArrowUp, ArrowDown,
}
'@

Write-File "src/DualFrontier.Runtime/Input/MouseButton.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Mouse button identifiers.</summary>
public enum MouseButton
{
    Left,
    Right,
    Middle,
    X1,
    X2,
}
'@

Write-File "src/DualFrontier.Runtime/Input/KeyPressedEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when a key transitions from up to down.</summary>
public sealed record KeyPressedEvent(Key Key) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/KeyReleasedEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when a key transitions from down to up.</summary>
public sealed record KeyReleasedEvent(Key Key) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/MouseMovedEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when the mouse pointer moves. Coordinates are window-client space, pixels.</summary>
public sealed record MouseMovedEvent(int X, int Y) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/MouseButtonEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when a mouse button changes state.</summary>
public sealed record MouseButtonEvent(MouseButton Button, bool Pressed, int X, int Y) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/MouseWheelEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when the mouse wheel rotates. Delta in standard wheel notches (1.0f = one notch).</summary>
public sealed record MouseWheelEvent(float Delta, int X, int Y) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/WindowResizedEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when the window client area is resized.</summary>
public sealed record WindowResizedEvent(int Width, int Height) : IInputEvent;
'@

Write-File "src/DualFrontier.Runtime/Input/WindowFocusEvent.cs" @'
namespace DualFrontier.Runtime.Input;

/// <summary>Raised when the window gains or loses focus.</summary>
public sealed record WindowFocusEvent(bool HasFocus) : IInputEvent;
'@

# -----------------------------------------------------------------------------
# Graphics
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Graphics/MODULE.md" @'
# Graphics — Module Documentation

**Purpose**: Vulkan rendering primitives — instance, device, swapchain, render
pass, pipeline, buffer, image, memory allocator. Direct C# wrappers around
Vulkan API c idiomatic lifetimes (IDisposable patterns).

## Public API surface

- `VulkanInstance`, `VulkanDevice`, `VulkanSurface`, `VulkanSwapchain`
- `VulkanCommandPool`, `VulkanRenderPass`, `VulkanPipeline`
- `VulkanBuffer`, `VulkanImage`, `VulkanShaderModule`
- `MemoryAllocator`
- `ValidationLayer`

## Dependencies

- `Native.Vulkan` (P/Invoke layer)
- `Window` (для surface creation only — IntPtr НWND)

## TODO

- **M9.0** — Instance / Device / Surface / Swapchain / CommandPool / ValidationLayer
- **M9.1** — RenderPass / Pipeline / Buffer / Image / ShaderModule / MemoryAllocator

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.2.
'@

# Helper к generate Vulkan wrapper class skeletons --------------------------
function Get-VulkanWrapperClass {
    param([string]$Name, [string]$Milestone, [string]$Description)
    return @"
namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// $Description
/// TODO: $Milestone — implement.
/// </summary>
public sealed class $Name : IDisposable
{
    /// <summary>Creates the wrapper. Native handles allocated lazily or at construction depending on resource semantics.</summary>
    public $Name()
        => throw new NotImplementedException(`"TODO: $Milestone — $Name constructor`");

    /// <inheritdoc />
    public void Dispose()
        => throw new NotImplementedException(`"TODO: $Milestone — $Name disposal (free Vulkan handles)`");
}
"@
}

Write-File "src/DualFrontier.Runtime/Graphics/VulkanInstance.cs" (Get-VulkanWrapperClass "VulkanInstance" "M9.0" "Owns the VkInstance handle. Configures application info, enabled extensions, validation layers.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanDevice.cs" (Get-VulkanWrapperClass "VulkanDevice" "M9.0" "Owns the VkPhysicalDevice + VkDevice + queue handles. Selects suitable physical device, creates logical device.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanSurface.cs" (Get-VulkanWrapperClass "VulkanSurface" "M9.0" "Owns the VkSurfaceKHR (Win32). Bridges window HWND к Vulkan presentation pipeline.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanSwapchain.cs" (Get-VulkanWrapperClass "VulkanSwapchain" "M9.0" "Owns VkSwapchainKHR + swap images + image views. Recreates on WM_SIZE.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanCommandPool.cs" (Get-VulkanWrapperClass "VulkanCommandPool" "M9.0" "Owns a VkCommandPool plus per-frame command buffers. Resets before each frame.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanRenderPass.cs" (Get-VulkanWrapperClass "VulkanRenderPass" "M9.1" "Owns a VkRenderPass. Defines color attachment + subpass dependencies для sprite drawing.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanPipeline.cs" (Get-VulkanWrapperClass "VulkanPipeline" "M9.1" "Owns a VkPipeline + VkPipelineLayout. Encapsulates graphics pipeline state for sprite rendering.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanBuffer.cs" (Get-VulkanWrapperClass "VulkanBuffer" "M9.1" "Owns a VkBuffer + bound VkDeviceMemory. Used для vertex / index / uniform / staging buffers.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanImage.cs" (Get-VulkanWrapperClass "VulkanImage" "M9.1" "Owns a VkImage + VkImageView + bound VkDeviceMemory. Used для textures и depth buffer.")
Write-File "src/DualFrontier.Runtime/Graphics/VulkanShaderModule.cs" (Get-VulkanWrapperClass "VulkanShaderModule" "M9.1" "Owns a VkShaderModule loaded from compiled SPIR-V bytecode.")
Write-File "src/DualFrontier.Runtime/Graphics/ValidationLayer.cs" (Get-VulkanWrapperClass "ValidationLayer" "M9.0" "Owns the VK_EXT_debug_utils messenger. Captures validation messages к ValidationLog.")
Write-File "src/DualFrontier.Runtime/Graphics/MemoryAllocator.cs" (Get-VulkanWrapperClass "MemoryAllocator" "M9.1" "VkDeviceMemory allocator. Initial implementation: bumper allocator. May evolve к pool/sub-allocator.")

# -----------------------------------------------------------------------------
# Sprite
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Sprite/MODULE.md" @'
# Sprite — Module Documentation

**Purpose**: 2D sprite rendering — atlas-based, batched draw calls. Camera2D
для orthographic projection.

## Public API surface

- `Sprite` — handle struct (atlas id + region + transform)
- `SpriteAtlas` — atlas region registry
- `SpriteBatcher` — dynamic VBO, single-batch draw
- `Camera2D` — orthographic projection
- `AtlasRegion` — UV rect

## Batching strategy

Sprites sharing an atlas are coalesced into a single draw call. Vertex data is
written к dynamic VBO each frame (Begin → Submit* → End). Sort by atlas id для
minimize state changes.

## TODO

- **M9.1** — single-quad path through pipeline, AtlasRegion + Sprite handles
- **M9.2** — dynamic VBO, batched submission, 10k sprites @ 60 FPS
- **M9.3** — Camera2D orthographic projection, screen ↔ world transforms

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.2.
'@

Write-File "src/DualFrontier.Runtime/Sprite/Sprite.cs" @'
using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Sprite handle — references a region within an atlas plus a model transform.
/// Pure value type; cheap к pass by value.
/// </summary>
/// <param name="AtlasId">Atlas registry id (0 = invalid).</param>
/// <param name="Region">UV rect within the atlas.</param>
/// <param name="Transform">Model-space к world-space transform.</param>
public readonly record struct Sprite(int AtlasId, AtlasRegion Region, Matrix4x4 Transform);
'@

Write-File "src/DualFrontier.Runtime/Sprite/AtlasRegion.cs" @'
namespace DualFrontier.Runtime.Sprite;

/// <summary>Pixel-space rect within a texture atlas.</summary>
/// <param name="X">Left x in pixels.</param>
/// <param name="Y">Top y in pixels.</param>
/// <param name="Width">Width in pixels.</param>
/// <param name="Height">Height in pixels.</param>
public readonly record struct AtlasRegion(int X, int Y, int Width, int Height);
'@

Write-File "src/DualFrontier.Runtime/Sprite/SpriteAtlas.cs" @'
namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Registry of named regions within a single atlas texture.
/// TODO: M9.1 — region table + name lookup.
/// </summary>
public sealed class SpriteAtlas
{
    /// <summary>Registers a named region. Returns a stable id для use in <see cref="Sprite"/>.</summary>
    public int RegisterRegion(string name, AtlasRegion region)
        => throw new NotImplementedException("TODO: M9.1 — SpriteAtlas.RegisterRegion");

    /// <summary>Looks up a previously registered region by name.</summary>
    public AtlasRegion GetRegion(string name)
        => throw new NotImplementedException("TODO: M9.1 — SpriteAtlas.GetRegion");
}
'@

Write-File "src/DualFrontier.Runtime/Sprite/SpriteBatcher.cs" @'
namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Dynamic vertex buffer batcher — accumulates sprite quads per frame, emits
/// minimum draw calls (one per atlas).
/// TODO: M9.2 — dynamic VBO + single-atlas batching + sort by atlas id.
/// </summary>
public sealed class SpriteBatcher : IDisposable
{
    /// <summary>Begins a new batch. Must be called once per frame before any Submit.</summary>
    public void Begin(Camera2D camera)
        => throw new NotImplementedException("TODO: M9.2 — SpriteBatcher.Begin (clear queues, bind camera)");

    /// <summary>Submits a sprite for rendering this frame.</summary>
    public void Submit(in Sprite sprite)
        => throw new NotImplementedException("TODO: M9.2 — SpriteBatcher.Submit (append к queue)");

    /// <summary>Flushes pending batches к the command buffer.</summary>
    public void End()
        => throw new NotImplementedException("TODO: M9.2 — SpriteBatcher.End (sort, write VBO, draw indexed)");

    /// <inheritdoc />
    public void Dispose()
        => throw new NotImplementedException("TODO: M9.2 — SpriteBatcher.Dispose (free VBO/IBO)");
}
'@

Write-File "src/DualFrontier.Runtime/Sprite/Camera2D.cs" @'
using System.Numerics;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Orthographic 2D camera. Position represents the world-space center of the view.
/// TODO: M9.3 — implement projection math (screen ↔ world transforms).
/// </summary>
public sealed class Camera2D
{
    /// <summary>World-space center of the camera view.</summary>
    public Vector2 Position { get; set; }

    /// <summary>Zoom factor (1.0 = 1 world unit per screen pixel scaled by viewport).</summary>
    public float Zoom { get; set; } = 1.0f;

    /// <summary>Returns the combined view × projection matrix for the current state.</summary>
    public Matrix4x4 GetViewProjection()
        => throw new NotImplementedException("TODO: M9.3 — orthographic VP matrix from Position+Zoom+viewport");
}
'@

# -----------------------------------------------------------------------------
# Text
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Text/MODULE.md" @'
# Text — Module Documentation

**Purpose**: Bitmap font text rendering. Reuses sprite pipeline (text =
textured quads from glyph atlas).

## Strategy

Pre-rendered bitmap fonts (PNG atlas + .fnt metrics file). Generated externally
(BMFont или similar). Runtime loads PNG + parses .fnt → glyph table.
TextRenderer batches glyph quads через SpriteBatcher.

## Public API surface

- `BitmapFont` — atlas + glyph metrics
- `Glyph` — per-character rect + offsets + advance
- `TextRenderer` — string → glyph quad batch

## TODO

- **M9.6** — BitmapFont.LoadFromFiles (.png + .fnt parse)
- **M9.6** — Glyph metrics struct
- **M9.6** — TextRenderer.DrawText (kerning deferred)

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.2.
'@

Write-File "src/DualFrontier.Runtime/Text/Glyph.cs" @'
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Runtime.Text;

/// <summary>
/// Per-character glyph metrics + atlas region.
/// </summary>
/// <param name="Character">Unicode scalar value.</param>
/// <param name="Region">Glyph rect in the font atlas.</param>
/// <param name="OffsetX">Pen-relative X offset to glyph origin.</param>
/// <param name="OffsetY">Pen-relative Y offset to glyph origin.</param>
/// <param name="Advance">Pen advance after rendering.</param>
public readonly record struct Glyph(char Character, AtlasRegion Region, int OffsetX, int OffsetY, int Advance);
'@

Write-File "src/DualFrontier.Runtime/Text/BitmapFont.cs" @'
namespace DualFrontier.Runtime.Text;

/// <summary>
/// Pre-rendered bitmap font. Loaded from a PNG atlas + .fnt metrics file
/// (BMFont format).
/// TODO: M9.6 — BitmapFont implementation (atlas load, .fnt parse).
/// </summary>
public sealed class BitmapFont
{
    /// <summary>Loads a font from a PNG atlas and a BMFont .fnt metrics file.</summary>
    public static BitmapFont LoadFromFiles(string pngPath, string fntPath)
        => throw new NotImplementedException("TODO: M9.6 — BitmapFont.LoadFromFiles");

    /// <summary>Looks up a glyph by character. Returns null if not present in the font.</summary>
    public Glyph? GetGlyph(char ch)
        => throw new NotImplementedException("TODO: M9.6 — BitmapFont.GetGlyph");
}
'@

Write-File "src/DualFrontier.Runtime/Text/TextRenderer.cs" @'
using System.Numerics;

namespace DualFrontier.Runtime.Text;

/// <summary>
/// Renders text using a <see cref="BitmapFont"/> through the sprite pipeline.
/// TODO: M9.6 — DrawText: walk string, look up glyphs, emit quads через SpriteBatcher.
/// </summary>
public sealed class TextRenderer
{
    /// <summary>Renders a string at the given world-space origin using the supplied font.</summary>
    public void DrawText(BitmapFont font, string text, Vector2 origin)
        => throw new NotImplementedException("TODO: M9.6 — TextRenderer.DrawText");
}
'@

# -----------------------------------------------------------------------------
# Assets
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Assets/MODULE.md" @'
# Assets — Module Documentation

**Purpose**: Asset loading — manual PNG decoder, asset path resolution.

## Pipeline

```
disk PNG → FileStream → PngDecoder.Decode → byte[] RGBA
                                       ↓
                          VulkanImage.CreateFromBytes (Graphics layer)
                                       ↓
                          SpriteAtlas.RegisterRegion (Sprite layer)
```

## Public API surface

- `PngDecoder` — IHDR / IDAT / IEND parsing + DEFLATE через BCL + filter unfiltering
- `AssetManager` — path resolution + caching
- `AssetPath` — typed wrapper

## TODO

- **M9.1** — PngDecoder full implementation (~500-700 lines)
  - chunk parser (IHDR, IDAT, IEND, IPLT, tRNS)
  - DEFLATE decompression via System.IO.Compression.DeflateStream
  - filter unfiltering (Sub/Up/Average/Paeth)
  - CRC32 verification
  - RGBA8 output (truecolor + alpha; indexed/grayscale deferred)
- **M9.5+** — AssetManager + AssetPath (when caching matters)

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.6.
'@

Write-File "src/DualFrontier.Runtime/Assets/PngDecoder.cs" @'
namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Manual PNG decoder. DEFLATE через BCL; chunk parsing manual.
/// TODO: M9.1 — full implementation per spec (PNG 1.2 + APNG ignored).
/// </summary>
public static class PngDecoder
{
    /// <summary>
    /// Decodes PNG bytes к RGBA8 pixel data.
    /// </summary>
    /// <param name="data">Raw PNG file bytes (signature + chunks).</param>
    /// <param name="rgba">Output RGBA8 byte array (width × height × 4).</param>
    /// <param name="width">Decoded width in pixels.</param>
    /// <param name="height">Decoded height in pixels.</param>
    /// <returns>True if decoded successfully; false on malformed input.</returns>
    public static bool Decode(ReadOnlySpan<byte> data, out byte[] rgba, out int width, out int height)
        => throw new NotImplementedException("TODO: M9.1 — PngDecoder.Decode");
}
'@

Write-File "src/DualFrontier.Runtime/Assets/AssetManager.cs" @'
namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Resolves asset paths and caches loaded resources.
/// TODO: M9.5 — AssetManager (path roots, texture cache, ref-counting).
/// </summary>
public sealed class AssetManager
{
    /// <summary>Loads (or returns cached) RGBA8 texture data for the given asset path.</summary>
    public byte[] LoadTexture(AssetPath path)
        => throw new NotImplementedException("TODO: M9.5 — AssetManager.LoadTexture");
}
'@

Write-File "src/DualFrontier.Runtime/Assets/AssetPath.cs" @'
namespace DualFrontier.Runtime.Assets;

/// <summary>
/// Typed wrapper for asset paths relative к the asset root.
/// </summary>
/// <param name="RelativePath">Forward-slash separated path под asset root (e.g., "kenney/terrain/sheet.png").</param>
public readonly record struct AssetPath(string RelativePath);
'@

# -----------------------------------------------------------------------------
# Diagnostic
# -----------------------------------------------------------------------------

Write-File "src/DualFrontier.Runtime/Diagnostic/MODULE.md" @'
# Diagnostic — Module Documentation

**Purpose**: Performance + debug tooling — FPS measurement, debug overlay,
validation log capture.

## Public API surface

- `FrameTimer` — measures FPS / frame time
- `DebugOverlay` — renders FPS + tick + queue depth overlay
- `ValidationLog` — captures Vulkan validation messages

## Dependencies

- `Sprite`, `Text` (для overlay rendering)
- `Graphics` (для validation messenger setup, indirectly через ValidationLayer)

## TODO

- **M9.7** — FrameTimer (rolling avg over N frames)
- **M9.7** — DebugOverlay rendering panel + text
- **M9.7** — ValidationLog circular buffer

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.2.
'@

Write-File "src/DualFrontier.Runtime/Diagnostic/FrameTimer.cs" @'
namespace DualFrontier.Runtime.Diagnostic;

/// <summary>
/// Measures frame time and exposes FPS as a rolling average.
/// TODO: M9.7 — Stopwatch-backed implementation, configurable window.
/// </summary>
public sealed class FrameTimer
{
    /// <summary>Records a frame boundary. Call once per frame.</summary>
    public void Tick()
        => throw new NotImplementedException("TODO: M9.7 — FrameTimer.Tick");

    /// <summary>Frames per second based on the rolling window.</summary>
    public double FramesPerSecond
        => throw new NotImplementedException("TODO: M9.7 — FrameTimer.FramesPerSecond");
}
'@

Write-File "src/DualFrontier.Runtime/Diagnostic/DebugOverlay.cs" @'
namespace DualFrontier.Runtime.Diagnostic;

/// <summary>
/// On-screen debug overlay. Displays FPS, tick rate, queue depths, validation
/// message count.
/// TODO: M9.7 — render panel + text via TextRenderer.
/// </summary>
public sealed class DebugOverlay
{
    /// <summary>Updates the metrics for the next draw.</summary>
    public void UpdateMetrics(double framesPerSecond, double tickPerSecond, int queueDepth)
        => throw new NotImplementedException("TODO: M9.7 — DebugOverlay.UpdateMetrics");

    /// <summary>Renders the overlay для current frame.</summary>
    public void Draw()
        => throw new NotImplementedException("TODO: M9.7 — DebugOverlay.Draw");
}
'@

Write-File "src/DualFrontier.Runtime/Diagnostic/ValidationLog.cs" @'
namespace DualFrontier.Runtime.Diagnostic;

/// <summary>
/// Severity classification for captured validation messages.
/// </summary>
public enum ValidationSeverity
{
    Verbose,
    Info,
    Warning,
    Error,
}

/// <summary>
/// Circular buffer для Vulkan validation messages.
/// TODO: M9.7 — bounded ring buffer + thread-safe Capture.
/// </summary>
public sealed class ValidationLog
{
    /// <summary>Captures a validation message (called from the Vulkan debug callback).</summary>
    public void Capture(ValidationSeverity severity, string message)
        => throw new NotImplementedException("TODO: M9.7 — ValidationLog.Capture");

    /// <summary>Returns a snapshot of currently buffered messages.</summary>
    public IReadOnlyList<string> GetMessages()
        => throw new NotImplementedException("TODO: M9.7 — ValidationLog.GetMessages");
}
'@

# -----------------------------------------------------------------------------
# Tests project
# -----------------------------------------------------------------------------

Write-File "tests/DualFrontier.Runtime.Tests/DualFrontier.Runtime.Tests.csproj" @'
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>DualFrontier.Runtime.Tests</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\DualFrontier.Runtime\DualFrontier.Runtime.csproj" />
  </ItemGroup>

</Project>
'@

Write-File "tests/DualFrontier.Runtime.Tests/MODULE.md" @'
# DualFrontier.Runtime.Tests — Module Documentation

**Purpose**: Non-GPU runtime unit tests. JIT-runnable on any developer machine
без Vulkan device.

GPU-dependent paths are verified manually per F5 visual protocol (precedent от
M8.8 / M8.9). See `docs/RUNTIME_ARCHITECTURE.md` §1.8 для testing strategy.

## Test areas

- **Assets/** — PngDecoder unit tests (synthetic PNG inputs → expected RGBA)
- **Sprite/** — Camera2D math, SpriteBatcher batching logic
- **Input/** — InputEventQueue cross-thread semantics

## Goal

Post-M9.8: ~520 total tests project-wide (existing 472 + new ~50).

## TODO

- **M9.1** — PngDecoderTests (CRC, filter unfiltering, edge cases)
- **M9.2** — SpriteBatcherTests (atlas grouping, sort stability)
- **M9.3** — Camera2DTests (orthographic projection, screen ↔ world)
- **M9.4** — InputEventQueueTests (concurrent producers/consumers)

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.8.
'@

Write-File "tests/DualFrontier.Runtime.Tests/Assets/PngDecoderTests.cs" @'
using Xunit;
using DualFrontier.Runtime.Assets;

namespace DualFrontier.Runtime.Tests.Assets;

public class PngDecoderTests
{
    [Fact]
    public void TODO_M9_1_DecodeMinimalRgba8Png()
        => throw new NotImplementedException("TODO: M9.1 — synthetic 1×1 RGBA PNG → 4 bytes output");

    [Fact]
    public void TODO_M9_1_RejectMalformedSignature()
        => throw new NotImplementedException("TODO: M9.1 — PngDecoder.Decode returns false for bad signature");

    [Fact]
    public void TODO_M9_1_HandleAllFilterTypes()
        => throw new NotImplementedException("TODO: M9.1 — Sub/Up/Average/Paeth filters round-trip correctly");
}
'@

Write-File "tests/DualFrontier.Runtime.Tests/Sprite/Camera2DTests.cs" @'
using Xunit;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Runtime.Tests.Sprite;

public class Camera2DTests
{
    [Fact]
    public void TODO_M9_3_IdentityCameraProducesIdentityProjection()
        => throw new NotImplementedException("TODO: M9.3 — zero position + zoom 1 → orthographic identity");

    [Fact]
    public void TODO_M9_3_ZoomScalesViewProjection()
        => throw new NotImplementedException("TODO: M9.3 — zoom 2 halves visible world span");
}
'@

Write-File "tests/DualFrontier.Runtime.Tests/Sprite/SpriteBatcherTests.cs" @'
using Xunit;
using DualFrontier.Runtime.Sprite;

namespace DualFrontier.Runtime.Tests.Sprite;

public class SpriteBatcherTests
{
    [Fact]
    public void TODO_M9_2_BatchesSpritesSharingAtlas()
        => throw new NotImplementedException("TODO: M9.2 — same atlas → single draw call");

    [Fact]
    public void TODO_M9_2_SeparatesBatchesPerAtlas()
        => throw new NotImplementedException("TODO: M9.2 — N atlases → N draw calls");
}
'@

Write-File "tests/DualFrontier.Runtime.Tests/Input/InputEventQueueTests.cs" @'
using Xunit;
using DualFrontier.Runtime.Window;

namespace DualFrontier.Runtime.Tests.Input;

public class InputEventQueueTests
{
    [Fact]
    public void TODO_M9_4_EnqueueDequeueRoundTrip()
        => throw new NotImplementedException("TODO: M9.4 — single producer / single consumer round-trip");

    [Fact]
    public void TODO_M9_4_TryDequeueReturnsFalseWhenEmpty()
        => throw new NotImplementedException("TODO: M9.4 — empty queue → TryDequeue returns false");
}
'@

# -----------------------------------------------------------------------------
# Shader placeholders
# -----------------------------------------------------------------------------

Write-File "tools/shaders/MODULE.md" @'
# Shaders — Module Documentation

**Purpose**: GLSL source files. Compiled к SPIR-V via the `CompileShaders`
MSBuild target в `Directory.Build.props` (uses `tools/glslangValidator.exe`).

Output goes к `assets/shaders/*.spv`. Production binary depends only on
compiled `.spv` artifacts and `vulkan-1.dll`.

## TODO

- **M9.1** — `sprite.vert` / `sprite.frag` (textured quad с MVP + sample + alpha blend)
- **M9.6** — `text.vert` / `text.frag` (similar but UV from glyph atlas)

The placeholder bodies (`void main() {}`) ensure the build target has source
files на которых to operate. Real shader bodies arrive in M9.1 / M9.6.

Reference: `docs/RUNTIME_ARCHITECTURE.md` §1.7.
'@

Write-File "tools/shaders/sprite.vert" @'
#version 450
// TODO: M9.1 — sprite vertex shader (MVP transform + UV pass-through).
// Inputs: position (vec2), uv (vec2). Outputs: gl_Position, uv.
void main() {}
'@

Write-File "tools/shaders/sprite.frag" @'
#version 450
// TODO: M9.1 — sprite fragment shader (sample atlas, output color, premultiplied alpha).
// Inputs: uv. Bindings: sampler2D atlas. Output: color.
void main() {}
'@

Write-File "tools/shaders/text.vert" @'
#version 450
// TODO: M9.6 — text vertex shader (per-glyph quad transform + UV).
void main() {}
'@

Write-File "tools/shaders/text.frag" @'
#version 450
// TODO: M9.6 — text fragment shader (sample glyph atlas, alpha-only).
void main() {}
'@

Write-Host "Files generated" -ForegroundColor Green

# === Section 4: Update DualFrontier.sln ======================================

$slnPath = "DualFrontier.sln"
$sln = Get-Content $slnPath -Raw

# Project type GUID for C# SDK-style projects (Microsoft.NET.Sdk).
$csProjType = "{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"

# Generate stable GUIDs для new projects (fresh on each run is fine — sln still valid).
$runtimeGuid = "{" + ([guid]::NewGuid().ToString().ToUpper()) + "}"
$runtimeTestsGuid = "{" + ([guid]::NewGuid().ToString().ToUpper()) + "}"

# Only insert if not already present (idempotency).
if ($sln -notmatch [regex]::Escape("DualFrontier.Runtime.csproj")) {
    $newProjects = @"
Project("$csProjType") = "DualFrontier.Runtime", "src\DualFrontier.Runtime\DualFrontier.Runtime.csproj", "$runtimeGuid"
EndProject
Project("$csProjType") = "DualFrontier.Runtime.Tests", "tests\DualFrontier.Runtime.Tests\DualFrontier.Runtime.Tests.csproj", "$runtimeTestsGuid"
EndProject
"@

    # Insert before "Global" section.
    $sln = $sln -replace "(?m)^Global\b", ($newProjects + "`r`nGlobal")

    # Add к ProjectConfigurationPlatforms section.
    $configBlock = @"
        $runtimeGuid.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        $runtimeGuid.Debug|Any CPU.Build.0 = Debug|Any CPU
        $runtimeGuid.Release|Any CPU.ActiveCfg = Release|Any CPU
        $runtimeGuid.Release|Any CPU.Build.0 = Release|Any CPU
        $runtimeTestsGuid.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        $runtimeTestsGuid.Debug|Any CPU.Build.0 = Debug|Any CPU
        $runtimeTestsGuid.Release|Any CPU.ActiveCfg = Release|Any CPU
        $runtimeTestsGuid.Release|Any CPU.Build.0 = Release|Any CPU
"@
    $sln = $sln -replace "(?m)^(\s*EndGlobalSection\s*\r?\n\s*GlobalSection\(SolutionProperties\))", ($configBlock + "`r`n`$1")

    Set-Content -Path $slnPath -Value $sln -Encoding UTF8 -NoNewline:$false
    Write-Host "Solution file updated с new projects" -ForegroundColor Green
} else {
    Write-Host "Solution file already contains DualFrontier.Runtime — skipping" -ForegroundColor Yellow
}

# === Section 5: Update Directory.Build.props =================================

$propsPath = "Directory.Build.props"
$props = Get-Content $propsPath -Raw

if ($props -notmatch "CompileShaders") {
    $shaderTarget = @'

  <Target Name="CompileShaders" BeforeTargets="Build"
          Condition="'$(MSBuildProjectName)' == 'DualFrontier.Runtime'">
    <PropertyGroup>
      <GlslangPath>$(SolutionDir)tools\glslangValidator.exe</GlslangPath>
      <ShaderSrcDir>$(SolutionDir)tools\shaders</ShaderSrcDir>
      <ShaderOutDir>$(SolutionDir)assets\shaders</ShaderOutDir>
    </PropertyGroup>
    <MakeDir Directories="$(ShaderOutDir)" Condition="!Exists('$(ShaderOutDir)')" />
    <ItemGroup>
      <ShaderSource Include="$(ShaderSrcDir)\*.vert;$(ShaderSrcDir)\*.frag" />
    </ItemGroup>
    <Exec Command="&quot;$(GlslangPath)&quot; -V &quot;%(ShaderSource.FullPath)&quot; -o &quot;$(ShaderOutDir)\%(ShaderSource.Filename)%(ShaderSource.Extension).spv&quot;"
          Condition="Exists('$(GlslangPath)') AND '@(ShaderSource)' != ''" />
  </Target>

'@
    $props = $props -replace "</Project>", ($shaderTarget + "</Project>")
    Set-Content -Path $propsPath -Value $props -Encoding UTF8 -NoNewline:$false
    Write-Host "Directory.Build.props updated с CompileShaders target" -ForegroundColor Green
} else {
    Write-Host "Directory.Build.props already has CompileShaders target — skipping" -ForegroundColor Yellow
}

# === Done ====================================================================

Write-Host ""
Write-Host "Scaffolding complete" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. dotnet build DualFrontier.sln  # verify projects compile" -ForegroundColor Yellow
Write-Host "  2. dotnet test                     # verify test discovery works" -ForegroundColor Yellow
Write-Host "  3. Begin M9.0 brief                # implement Win32 + Vulkan foundation" -ForegroundColor Yellow
