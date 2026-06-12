namespace DualFrontier.Contracts.Modding;

/// <summary>
/// Compute-pipeline sub-API placeholder per MOD_OS_ARCHITECTURE §4.6
/// (Fields and Compute Pipelines). No implementation exists:
/// <see cref="IModApi.ComputePipelines"/> returns <c>null</c> and mods
/// degrade gracefully. The former G0 milestone was superseded by the
/// V substrate per Q-G-2 — V0/V1 shipped the kernel-side compute path;
/// the mod-facing wiring is Planned — see docs/ROADMAP.md §V substrate
/// (M-V demonstrations).
/// </summary>
public interface IModComputePipelineApi
{
    /// <summary>
    /// Identifier for the implementation. No implementation ships today;
    /// the mod-facing compute surface is wired with the V substrate M-V
    /// work (Planned — see docs/ROADMAP.md §V substrate).
    /// </summary>
    string Name { get; }
}
