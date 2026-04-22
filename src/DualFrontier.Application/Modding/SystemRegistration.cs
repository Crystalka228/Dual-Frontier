using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Entry in <see cref="ModRegistry"/>: a concrete system instance plus its
/// provenance (core vs mod) and the mod identifier when applicable.
/// Consumed by <see cref="ModIntegrationPipeline"/> to feed the dependency
/// graph in the correct order (core-first, then mods by registration).
/// </summary>
/// <param name="Instance">System instance to register with the scheduler.</param>
/// <param name="Origin">Core or Mod provenance, drives fault routing.</param>
/// <param name="ModId">Owning mod id when <paramref name="Origin"/> is <see cref="SystemOrigin.Mod"/>; otherwise null.</param>
internal sealed record SystemRegistration(
    SystemBase Instance,
    SystemOrigin Origin,
    string? ModId);
