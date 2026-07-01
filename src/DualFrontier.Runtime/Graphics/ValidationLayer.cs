using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DualFrontier.Runtime.Diagnostic;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Vulkan debug messenger (VK_EXT_debug_utils). Captures validation layer messages
/// (WARNING + ERROR severities — per S-LOCK-4 V0.A discipline) into <see cref="ValidationLog"/>.
/// Single-instance design: static <c>_staticLog</c> shared с <c>[UnmanagedCallersOnly]</c>
/// callback (callback signature cannot capture instance fields). Multi-instance use would race —
/// V0.A scope does not exercise multi-ValidationLayer; future V0.B/V0.C may revisit if needed.
/// </summary>
public sealed class ValidationLayer : IDisposable
{
    private static ValidationLog? _staticLog;

    private readonly IntPtr _instance;
    private IntPtr _messenger;
    private bool _disposed;

    public ValidationLog Log { get; } = new();

    public ValidationLayer(VulkanInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        if (!instance.ValidationLayerEnabled)
        {
            throw new InvalidOperationException(
                "ValidationLayer requires VulkanInstance constructed с enableValidation: true.");
        }
        _instance = instance.Handle;
        _staticLog = Log;
        CreateMessenger();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe uint DebugCallback(
        VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageType,
        IntPtr pCallbackData,
        IntPtr pUserData)
    {
        try
        {
            string message = "<unknown>";
            if (pCallbackData != IntPtr.Zero)
            {
                var data = (VkDebugUtilsMessengerCallbackDataEXT*)pCallbackData;
                if (data->pMessage != null)
                {
                    message = Marshal.PtrToStringUTF8((IntPtr)data->pMessage) ?? "<unknown>";
                }
            }
            _staticLog?.Record(ConvertSeverity(messageSeverity), message);
        }
        catch
        {
            // Validation callback runs from Vulkan driver thread — must not propagate
            // managed exceptions across the native boundary. Swallow + continue.
        }

        // VK_FALSE — Vulkan continues the call that triggered the message.
        return 0;
    }

    private static ValidationSeverity ConvertSeverity(VkDebugUtilsMessageSeverityFlagsEXT severity)
    {
        if ((severity & VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT) != 0)
        {
            return ValidationSeverity.Error;
        }
        if ((severity & VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT) != 0)
        {
            return ValidationSeverity.Warning;
        }
        return ValidationSeverity.Info;
    }

    private unsafe void CreateMessenger()
    {
        IntPtr createFnPtr = VkApi.vkGetInstanceProcAddr(_instance, "vkCreateDebugUtilsMessengerEXT");
        if (createFnPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "vkCreateDebugUtilsMessengerEXT not available. " +
                "VK_EXT_debug_utils extension not loaded or validation layer absent.");
        }
        // DFK-WAIVER(DFK001): sanctioned Vulkan debug-messenger interop
        // (VK_EXT_debug_utils, К-L19) — function-pointer marshalling is intrinsic to
        // the Vulkan extension ABI; Runtime.Graphics is the Vulkan presentation layer.
#pragma warning disable DFK001
        var createFn = Marshal.GetDelegateForFunctionPointer<CreateDebugUtilsMessengerDelegate>(createFnPtr);
#pragma warning restore DFK001

        var createInfo = new VkDebugUtilsMessengerCreateInfoEXT
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
            pNext = IntPtr.Zero,
            flags = 0,
            messageSeverity =
                VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT
                | VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT,
            messageType =
                VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT
                | VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT
                | VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT,
            pfnUserCallback = (IntPtr)(delegate* unmanaged[Cdecl]<
                VkDebugUtilsMessageSeverityFlagsEXT,
                VkDebugUtilsMessageTypeFlagsEXT,
                IntPtr,
                IntPtr,
                uint>)&DebugCallback,
            pUserData = IntPtr.Zero,
        };

        VkResult result = createFn(_instance, in createInfo, IntPtr.Zero, out _messenger);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException(
                $"vkCreateDebugUtilsMessengerEXT failed: {result}");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_messenger != IntPtr.Zero && _instance != IntPtr.Zero)
        {
            IntPtr destroyFnPtr = VkApi.vkGetInstanceProcAddr(_instance, "vkDestroyDebugUtilsMessengerEXT");
            if (destroyFnPtr != IntPtr.Zero)
            {
                // DFK-WAIVER(DFK001): sanctioned Vulkan debug-messenger interop
                // (VK_EXT_debug_utils, К-L19) — function-pointer marshalling is
                // intrinsic to the Vulkan extension ABI.
#pragma warning disable DFK001
                var destroyFn = Marshal.GetDelegateForFunctionPointer<DestroyDebugUtilsMessengerDelegate>(destroyFnPtr);
#pragma warning restore DFK001
                destroyFn(_instance, _messenger, IntPtr.Zero);
            }
            _messenger = IntPtr.Zero;
        }
        _disposed = true;
    }
}
