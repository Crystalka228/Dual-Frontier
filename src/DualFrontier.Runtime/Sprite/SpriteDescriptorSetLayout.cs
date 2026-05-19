using DualFrontier.Runtime.Graphics;
using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Sprite;

/// <summary>
/// Descriptor set layout for sprite pipeline: 1 binding (combined image sampler, fragment stage).
/// Matches sprite.frag layout(set = 0, binding = 0) uniform sampler2D atlas.
/// </summary>
public sealed class SpriteDescriptorSetLayout : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _layout;
    private bool _disposed;

    public IntPtr Handle => _layout;

    public unsafe SpriteDescriptorSetLayout(VulkanDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;

        var binding = new VkDescriptorSetLayoutBinding
        {
            binding = 0,
            descriptorType = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = 1,
            stageFlags = VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT,
            pImmutableSamplers = IntPtr.Zero,
        };

        var createInfo = new VkDescriptorSetLayoutCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            bindingCount = 1,
            pBindings = &binding,
        };

        VkResult result = VkApi.vkCreateDescriptorSetLayout(_device, in createInfo, IntPtr.Zero, out _layout);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateDescriptorSetLayout failed: {result}");
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
            VkApi.vkDestroyDescriptorSetLayout(_device, _layout, IntPtr.Zero);
            _layout = IntPtr.Zero;
        }
        _disposed = true;
    }
}
