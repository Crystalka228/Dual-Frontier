using DualFrontier.Runtime.Native.Vulkan;

namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// VkShaderModule wrapper. Loads SPIR-V bytecode from file/byte array, creates module,
/// disposed via vkDestroyShaderModule. Module можно destroy после graphics/compute pipeline
/// creation completes (driver retains compiled state).
/// </summary>
public sealed class VulkanShaderModule : IDisposable
{
    private readonly IntPtr _device;
    private IntPtr _module;
    private bool _disposed;

    public IntPtr Handle => _module;

    public static VulkanShaderModule LoadFromFile(VulkanDevice device, string spirvPath)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(spirvPath);
        if (!File.Exists(spirvPath))
        {
            throw new FileNotFoundException($"SPIR-V file not found: {spirvPath}", spirvPath);
        }
        byte[] bytes = File.ReadAllBytes(spirvPath);
        return new VulkanShaderModule(device, bytes);
    }

    public unsafe VulkanShaderModule(VulkanDevice device, byte[] spirvBytecode)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(spirvBytecode);
        if (spirvBytecode.Length == 0 || spirvBytecode.Length % 4 != 0)
        {
            throw new ArgumentException(
                $"SPIR-V bytecode length must be a non-zero multiple of 4 (got {spirvBytecode.Length})",
                nameof(spirvBytecode));
        }
        _device = device.Handle;

        fixed (byte* codePtr = spirvBytecode)
        {
            var createInfo = new VkShaderModuleCreateInfo
            {
                sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO,
                pNext = IntPtr.Zero,
                flags = 0,
                codeSize = (nuint)spirvBytecode.Length,
                pCode = (uint*)codePtr,
            };
            VkResult result = VkApi.vkCreateShaderModule(_device, in createInfo, IntPtr.Zero, out _module);
            if (result != VkResult.VK_SUCCESS)
            {
                throw new InvalidOperationException($"vkCreateShaderModule failed: {result}");
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        if (_module != IntPtr.Zero)
        {
            VkApi.vkDestroyShaderModule(_device, _module, IntPtr.Zero);
            _module = IntPtr.Zero;
        }
        _disposed = true;
    }
}
