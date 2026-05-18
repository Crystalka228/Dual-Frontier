namespace DualFrontier.Runtime.Graphics;

/// <summary>
/// Thrown by <see cref="HardwareCapabilityCheck"/> when the host hardware fails К-L19 mandate
/// (Vulkan 1.3 + async compute queue family). Carries a user-facing diagnostic message с
/// pointer к README.md hardware requirements section.
/// </summary>
public sealed class HardwareCapabilityException : Exception
{
    public HardwareCapabilityException(string message) : base(message) { }
    public HardwareCapabilityException(string message, Exception inner) : base(message, inner) { }
}
