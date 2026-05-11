namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Compute-pipeline sub-API placeholder per <c>MOD_OS_ARCHITECTURE.md</c>
/// v1.7 §4.6. Ships at G0 alongside Vulkan compute plumbing; on K9-only
/// builds <see cref="IModApi.ComputePipelines"/> returns <c>null</c> and
/// mods degrade gracefully.
/// </summary>
public interface IModComputePipelineApi
{
    /// <summary>
    /// Identifier for the implementation. K9 ships no implementation;
    /// G0 wires the real surface.
    /// </summary>
    string Name { get; }
}
