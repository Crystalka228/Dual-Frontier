using System;
using System.Runtime.InteropServices;

namespace DualFrontier.Core.Interop;

/// <summary>
/// V0.B compute pipeline + Vulkan attachment P/Invoke surface. Mirrors
/// the V0.B section in <c>native/DualFrontier.Core.Native/include/df_capi.h</c>.
/// </summary>
internal static partial class NativeMethods
{
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_attach_vulkan(
        IntPtr world,
        IntPtr vkInstance,
        IntPtr vkPhysicalDevice,
        IntPtr vkDevice,
        IntPtr vkAsyncComputeQueue,
        uint asyncComputeQueueFamilyIndex);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe uint df_world_register_compute_pipeline(
        IntPtr world,
        byte* pipelineName,
        byte* spirvBytecode,
        int spirvSize,
        uint descriptorBindingCount);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe int df_world_field_dispatch_compute(
        IntPtr world,
        byte* fieldName,
        uint pipelineId,
        uint dispatchX,
        uint dispatchY,
        uint dispatchZ);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int df_world_compute_pipeline_count(IntPtr world);
}
