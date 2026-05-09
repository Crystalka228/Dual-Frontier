using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Per-system metadata the scheduler needs to construct a
/// <see cref="DualFrontier.Core.ECS.SystemExecutionContext"/> with the
/// correct <see cref="DualFrontier.Core.ECS.SystemOrigin"/> and modId.
/// Application-side <c>SystemRegistration</c> projects to this record at
/// bootstrap time; the scheduler stays in Core and does not depend on the
/// modding layer.
/// </summary>
/// <param name="Origin">Provenance of the system, drives fault routing.</param>
/// <param name="ModId">Owning mod id when <paramref name="Origin"/> is <see cref="SystemOrigin.Mod"/>; otherwise null.</param>
internal sealed record SystemMetadata(SystemOrigin Origin, string? ModId);
