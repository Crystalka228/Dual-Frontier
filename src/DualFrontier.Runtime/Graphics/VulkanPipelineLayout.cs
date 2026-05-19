using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkPipelineLayout wrapper. V0.B clearcolor pipeline uses empty layout (no descriptor sets,
/// no push constants — fullscreen triangle vertex shader needs nothing). V0.C textures extend
/// с descriptor set layouts.
/// </summary>
public sealed class VulkanPipelineLayout : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _layout;
    private bool _disposed;

    public IntPtr Handle => _layout;

    public unsafe VulkanPipelineLayout(VulkanDevice device, IReadOnlyList<IntPtr>? descriptorSetLayouts = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;

        IntPtr[] sets = descriptorSetLayouts?.ToArray() ?? Array.Empty<IntPtr>();
        fixed (IntPtr* setsPtr = sets)
        {
            var createInfo = new VkPipelineLayoutCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                setLayoutCount = (uint)sets.Length,
                pSetLayouts = sets.Length == 0 ? null : setsPtr,
                pushConstantRangeCount = 0,
                pPushConstantRanges = IntPtr.Zero,
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
