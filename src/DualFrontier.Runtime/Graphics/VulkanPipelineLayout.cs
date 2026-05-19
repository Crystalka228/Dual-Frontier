using System.Runtime.InteropServices;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkPipelineLayout wrapper. V0.B clearcolor pipeline uses empty layout (no descriptor sets,
/// no push constants — fullscreen triangle vertex shader needs nothing). V0.C textures extend
/// с descriptor set layouts. V0.C.1 Commit 8: backward-compatible push constant range parameter
/// added per S-LOCK-8 (Camera MVP push constant for sprite pipeline vertex shader).
/// </summary>
public sealed class VulkanPipelineLayout : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _layout;
    private bool _disposed;

    public IntPtr Handle => _layout;

    public unsafe VulkanPipelineLayout(
        VulkanDevice device,
        IReadOnlyList<IntPtr>? descriptorSetLayouts = null,
        IReadOnlyList<VkPushConstantRangePublic>? pushConstantRanges = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;

        IntPtr[] sets = descriptorSetLayouts?.ToArray() ?? Array.Empty<IntPtr>();
        VkPushConstantRange[] ranges = pushConstantRanges?
            .Select(r => new VkPushConstantRange
            {
                stageFlags = (VkShaderStageFlags)(uint)r.StageFlags,
                offset = r.Offset,
                size = r.Size,
            })
            .ToArray() ?? Array.Empty<VkPushConstantRange>();

        fixed (IntPtr* setsPtr = sets)
        fixed (VkPushConstantRange* rangesPtr = ranges)
        {
            var createInfo = new VkPipelineLayoutCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                setLayoutCount = (uint)sets.Length,
                pSetLayouts = sets.Length == 0 ? null : setsPtr,
                pushConstantRangeCount = (uint)ranges.Length,
                pPushConstantRanges = ranges.Length == 0 ? IntPtr.Zero : (IntPtr)rangesPtr,
            };
            VkResult result = VkApi.vkCreatePipelineLayout(_device, in createInfo, IntPtr.Zero, out _layout);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreatePipelineLayout failed: {result}");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_layout != IntPtr.Zero)
        {
            VkApi.vkDestroyPipelineLayout(_device, _layout, IntPtr.Zero);
            _layout = IntPtr.Zero;
        }
        _disposed = true;
    }
}

/// <summary>
/// Public-facing mirror of VkPushConstantRange. Cast at construction boundary inside
/// <see cref="VulkanPipelineLayout"/>.
/// </summary>
public readonly record struct VkPushConstantRangePublic(
    VkShaderStageFlagsPublic StageFlags,
    uint Offset,
    uint Size);

/// <summary>
/// Public-facing mirror of VkShaderStageFlags. Mirrors values bit-for-bit so callers don't
/// need internal Vulkan enums.
/// </summary>
[Flags]
public enum VkShaderStageFlagsPublic : uint
{
    Vertex = 0x00000001,
    Fragment = 0x00000010,
    Compute = 0x00000020,
}
