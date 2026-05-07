using System;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Passive ambient Comfort emitter. Each SLOW tick (~2s at 30 TPS), для
/// each entity carrying <see cref="DecorativeAuraComponent"/>, scans pawns
/// within Chebyshev distance <see cref="DecorativeAuraComponent.Radius"/>
/// and publishes <see cref="NeedsRestoredEvent"/> с
/// <see cref="DecorativeAuraComponent.ComfortPerTick"/> amount.
///
/// Pure publisher: writes no components. NeedsComponent restoration
/// happens в NeedsSystem's captured-context handler (M8.5 pattern,
/// extended in M8.6 для Sleep + Comfort cases).
///
/// Nested O(N×M) loop: 25 decorations × 50 pawns at current scale = 1250
/// distance checks per SLOW tick. SpatialGrid integration deferred к
/// backlog когда entity counts grow (formal kernel system needed для
/// lifecycle).
///
/// Primary Comfort path. Secondary path = bed-sleep proportional formula
/// (ΔComfort = ΔSleep × 0.3) implemented в SleepSystem (M8.6, master plan
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
        // For each decoration, scan pawns в radius and publish
        // restoration events. Outer loop = decorations (smaller set);
        // inner loop = pawns. Order matters только если we add
        // per-pawn caps/dedup в future (currently each event is
        // additive through NeedsSystem).
        foreach (EntityId decoration in Query<DecorativeAuraComponent, PositionComponent>())
        {
            DecorativeAuraComponent aura = GetComponent<DecorativeAuraComponent>(decoration);
            PositionComponent decoPos = GetComponent<PositionComponent>(decoration);

            foreach (EntityId pawn in Query<NeedsComponent, PositionComponent>())
            {
                PositionComponent pawnPos = GetComponent<PositionComponent>(pawn);
                int dist = ChebyshevDistance(decoPos.Position, pawnPos.Position);
                if (dist > aura.Radius) continue;

                Services.Pawns.Publish(new NeedsRestoredEvent
                {
                    PawnId = pawn,
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
