using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkSampler wrapper. V0.C.1 default: nearest-neighbor filter + REPEAT wrap mode per S-LOCK-6
/// (pixel art aesthetic preservation per Q3 (b) ratification). Configurable via
/// <see cref="SamplerOptions"/> for future flexibility (linear filtering, clamp wraps).
///
/// Anisotropy disabled by default — enabling requires VkPhysicalDeviceFeatures.samplerAnisotropy
/// device feature opt-in (not currently requested by VulkanDevice). V0.C.1/V0.C.2 не needs it;
/// future TileMap perspective rendering can extend.
/// </summary>
public sealed class VulkanSampler : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _sampler;
    private bool _disposed;

    public IntPtr Handle => _sampler;
    public SamplerOptions Options { get; }

    public VulkanSampler(VulkanDevice device, SamplerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        _device = device.Handle;
        Options = options ?? new SamplerOptions();

        var createInfo = new VkSamplerCreateInfo
        {
            sType = VkStructureType.VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO,
            pNext = IntPtr.Zero,
            flags = 0,
            magFilter = MapFilter(Options.MagFilter),
            minFilter = MapFilter(Options.MinFilter),
            mipmapMode = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_NEAREST,
            addressModeU = MapWrap(Options.WrapU),
            addressModeV = MapWrap(Options.WrapV),
            addressModeW = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            mipLodBias = 0.0f,
            anisotropyEnable = Options.EnableAnisotropy ? 1u : 0u,
            maxAnisotropy = Options.EnableAnisotropy ? 16.0f : 1.0f,
            compareEnable = 0,
            compareOp = VkCompareOp.VK_COMPARE_OP_NEVER,
            minLod = 0.0f,
            maxLod = 0.0f,        // V0.C.1: no mipmaps; single mip level.
            borderColor = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            unnormalizedCoordinates = 0,
        };

        VkResult result = VkApi.vkCreateSampler(_device, in createInfo, IntPtr.Zero, out _sampler);
        if (result != VkResult.VK_SUCCESS)
        {
            throw new InvalidOperationException($"vkCreateSampler failed: {result}");
        }
    }

    private static VkFilter MapFilter(SamplerFilterMode mode) => mode switch
    {
        SamplerFilterMode.Nearest => VkFilter.VK_FILTER_NEAREST,
        SamplerFilterMode.Linear => VkFilter.VK_FILTER_LINEAR,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };

    private static VkSamplerAddressMode MapWrap(SamplerWrapMode mode) => mode switch
    {
        SamplerWrapMode.Repeat => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
        SamplerWrapMode.ClampToEdge => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE,
        SamplerWrapMode.ClampToBorder => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER,
        SamplerWrapMode.MirroredRepeat => VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_sampler != IntPtr.Zero)
        {
            VkApi.vkDestroySampler(_device, _sampler, IntPtr.Zero);
            _sampler = IntPtr.Zero;
        }
        _disposed = true;
    }
}

/// <summary>Sampler filter mode per <see cref="VulkanSampler"/> options.</summary>
public enum SamplerFilterMode { Nearest, Linear }

/// <summary>Sampler texture coordinate wrap mode per <see cref="VulkanSampler"/> options.</summary>
public enum SamplerWrapMode { Repeat, ClampToEdge, ClampToBorder, MirroredRepeat }

/// <summary>
/// VulkanSampler configuration. V0.C.1 defaults preserve pixel art aesthetic (nearest filter
/// per S-LOCK-6 + repeat wrap for atlas/tilemap compatibility).
/// </summary>
public sealed record SamplerOptions
{
    public SamplerFilterMode MagFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerFilterMode MinFilter { get; init; } = SamplerFilterMode.Nearest;
    public SamplerWrapMode WrapU { get; init; } = SamplerWrapMode.Repeat;
    public SamplerWrapMode WrapV { get; init; } = SamplerWrapMode.Repeat;
    public bool EnableAnisotropy { get; init; } = false;
}
