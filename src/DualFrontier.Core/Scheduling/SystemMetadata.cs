using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Per-system metadata the scheduler needs to construct a
/// <see cref="DualFrontier.Core.ECS.SystemExecutionContext"/> with the
/// correct <see cref="DualFrontier.Core.ECS.SystemOrigin"/> and modId, and to
/// decide the system's tick cadence without touching its <see cref="System.Type"/>.
/// Application-side <c>SystemRegistration</c> projects to this record at
/// bootstrap time; the scheduler stays in Core and does not depend on the
/// modding layer.
///
/// <para>
/// F-60(a): this table is keyed by <see cref="SystemBase"/> INSTANCE and is
/// rebuilt and swapped wholesale at every load boundary (bootstrap, Apply,
/// UnloadMod, rollback/UnloadAll), so anything carried here is evicted at
/// unload for free. That is why the cadence lives here rather than in a
/// <see cref="System.Type"/>-keyed cache on the tick path: a strong
/// <see cref="System.Type"/> key populated during a tick outlives the mod that
/// produced it and roots its collectible <c>AssemblyLoadContext</c> for the
/// rest of the session.
/// </para>
/// </summary>
/// <param name="Origin">Provenance of the system, drives fault routing.</param>
/// <param name="ModId">Owning mod id when <paramref name="Origin"/> is <see cref="SystemOrigin.Mod"/>; otherwise null.</param>
/// <param name="TicksPerUpdate">
/// Resolved <c>[TickRate]</c> cadence, already normalised at construction: always
/// positive, defaulting to <see cref="DualFrontier.Contracts.Attributes.TickRates.REALTIME"/>
/// when the system declares no rate or a non-positive one. Deliberately has no
/// default value — every construction site states the cadence it means, so a
/// forgotten argument is a compile error rather than a silent every-tick system.
/// </param>
internal sealed record SystemMetadata(SystemOrigin Origin, string? ModId, int TicksPerUpdate);
