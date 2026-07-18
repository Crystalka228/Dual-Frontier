using System;
using System.Collections.Generic;

namespace DualFrontier.Application.Loop;

/// <summary>Origin of a Degraded reason (ENGINE_LIFECYCLE_AND_TRANSACTIONS.md §4.1 structured field).</summary>
internal enum DegradedSource
{
    Mod,
    Core,
}

/// <summary>
/// A structured Degraded reason (ENGINE_LIFECYCLE_AND_TRANSACTIONS.md §4.1:
/// <c>(source, faultClass, tickId, advice)</c>). <see cref="ModId"/> is populated for
/// mod-origin reasons so the exit path can remove a specific mod's reason.
/// </summary>
internal sealed record DegradedReason(
    DegradedSource Source,
    string FaultClass,
    long TickId,
    string Advice,
    string? ModId = null)
{
    /// <summary>
    /// The reason a quarantined mod contributes (EQ_A2 / M7): source = Mod, advised
    /// action = reload-or-remove-then-restart. The scheduler supplies the tick on
    /// which the mod was first quarantined.
    /// </summary>
    public static DegradedReason ForQuarantinedMod(string modId, long tickId) =>
        new(
            DegradedSource.Mod,
            FaultClass: "mod-system-fault (quarantined)",
            TickId: tickId,
            Advice: $"reload or remove mod '{modId}', then restart the session",
            ModId: modId);
}

/// <summary>Health kind: an annotation, NOT a sixth session/mod state (ELT §4.1).</summary>
internal enum EngineHealthKind
{
    Normal,
    Degraded,
}

/// <summary>
/// A queryable snapshot of session health (ELT §4.1: "a session-scoped EngineHealth
/// value: Normal | Degraded(reasons)"). Produced by <see cref="EngineSession.Health"/>.
/// </summary>
internal sealed record EngineHealth(EngineHealthKind Kind, IReadOnlyList<DegradedReason> Reasons)
{
    /// <summary>The Normal (no-reasons) health value.</summary>
    public static readonly EngineHealth Normal =
        new(EngineHealthKind.Normal, Array.Empty<DegradedReason>());
}

/// <summary>
/// Lifecycle event raised on every Degraded entry/exit (ELT §4.1: "every entry/exit
/// emits a §5 lifecycle event"). <see cref="Entered"/> is true when the transition
/// crossed Normal -&gt; Degraded (first reason) and false on Degraded -&gt; Normal
/// (last reason cleared); <see cref="Reason"/> is the reason added or removed.
/// </summary>
internal sealed record EngineHealthChanged(bool Entered, DegradedReason Reason, EngineHealthKind ResultingKind);
