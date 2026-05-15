using System;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Passive ambient Comfort emitter. Each SLOW tick (~2s at 30 TPS), for
/// each entity carrying <see cref="DecorativeAuraComponent"/>, scans pawns
/// within Chebyshev distance <see cref="DecorativeAuraComponent.Radius"/>
/// and publishes <see cref="NeedsRestoredEvent"/> with
/// <see cref="DecorativeAuraComponent.ComfortPerTick"/> amount.
///
/// Pure publisher: writes no components. NeedsComponent restoration
/// happens in NeedsSystem's captured-context handler (M8.5 pattern,
/// extended in M8.6 for Sleep + Comfort cases).
///
/// Nested O(N×M) loop: 25 decorations × 50 pawns at current scale = 1250
/// distance checks per SLOW tick. SpatialGrid integration deferred to
/// backlog when entity counts grow (formal kernel system needed for
/// lifecycle).
///
/// Primary Comfort path. Secondary path = bed-sleep proportional formula
/// (ΔComfort = ΔSleep × 0.3) implemented in SleepSystem (M8.6, master plan
/// AD-3). Combined paths give pawns autonomous Comfort recovery.
/// </summary>
[SystemAccess(
    reads:  new[]
    {
        typeof(DecorativeAuraComponent),
        typeof(PositionComponent),
        typeof(NeedsComponent),
    },
    writes: new Type[0],
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.SLOW)]
public sealed class ComfortAuraSystem : SystemBase
{
    public override void Update(float delta)
    {
        // Acquire spans once per tick. Pawn span is iterated for each
        // decoration; both are released at scope end (using-disposal).
        using SpanLease<DecorativeAuraComponent> auras = NativeWorld.AcquireSpan<DecorativeAuraComponent>();
        using SpanLease<NeedsComponent> needs = NativeWorld.AcquireSpan<NeedsComponent>();

        ReadOnlySpan<DecorativeAuraComponent> auraSpan = auras.Span;
        ReadOnlySpan<int> auraIndices = auras.Indices;
        ReadOnlySpan<int> needsIndices = needs.Indices;

        for (int a = 0; a < auras.Count; a++)
        {
            var auraEntity = new EntityId(auraIndices[a], 0);
            if (!NativeWorld.HasComponent<PositionComponent>(auraEntity)) continue;

            DecorativeAuraComponent aura = auraSpan[a];
            PositionComponent decoPos = NativeWorld.GetComponent<PositionComponent>(auraEntity);

            for (int p = 0; p < needs.Count; p++)
            {
                var pawnEntity = new EntityId(needsIndices[p], 0);
                if (!NativeWorld.HasComponent<PositionComponent>(pawnEntity)) continue;

                PositionComponent pawnPos = NativeWorld.GetComponent<PositionComponent>(pawnEntity);
                int dist = ChebyshevDistance(decoPos.Position, pawnPos.Position);
                if (dist > aura.Radius) continue;

                Services.Pawns.Publish(new NeedsRestoredEvent
                {
                    PawnId = pawnEntity,
                    Need   = NeedKind.Comfort,
                    Amount = aura.ComfortPerTick,
                });
            }
        }
    }

    private static int ChebyshevDistance(GridVector a, GridVector b)
    {
        int dx = Math.Abs(a.X - b.X);
        int dy = Math.Abs(a.Y - b.Y);
        return Math.Max(dx, dy);
    }
}
