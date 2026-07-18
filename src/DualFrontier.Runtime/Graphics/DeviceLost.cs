using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Device-loss classifier for the Vulkan wrapper call sites (M9 / D1). Internal because it takes the
/// internal <see cref="VkResult"/>; the thrown <see cref="DeviceLostException"/> is public. Each
/// site calls <see cref="ThrowIfLost"/> before its own generic-error handling -- VK_ERROR_DEVICE_LOST
/// is distinct from every other classified result, so the generic path is unchanged.
/// </summary>
internal static class DeviceLost
{
    /// <summary>
    /// Throws <see cref="DeviceLostException"/> iff <paramref name="result"/> is
    /// VK_ERROR_DEVICE_LOST; otherwise a no-op.
    /// </summary>
    public static void ThrowIfLost(VkResult result, in DeviceLostContext context)
    {
        if (result == VkResult.VK_ERROR_DEVICE_LOST)
        {
            throw new DeviceLostException(context);
        }
    }
}
