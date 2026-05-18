namespace DualFrontier.Runtime;

/// <summary>
/// Top-level facade for the Vulkan substrate. V0.A scope: window + Vulkan instance + device +
/// validation layer. V0.B + V0.C extend с swapchain, compute pipeline, sprite/text rendering.
/// </summary>
public sealed class Runtime
{
    // V0.A: placeholder facade. Composition lands at Commit 9 (Runtime facade composition).
    // Earlier commits add the underlying primitives (Window, VulkanInstance, ValidationLayer,
    // VulkanDevice) which Runtime.Create wires together at Commit 9.
}
