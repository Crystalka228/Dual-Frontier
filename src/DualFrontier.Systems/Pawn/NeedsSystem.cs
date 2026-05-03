using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn
{
    /// <summary>
    /// Per-pawn wellness depletion. Each SLOW tick (~2s at 30 TPS), every
    /// need depletes by its constant rate, floored at 0.0. The semantic per
    /// <see cref="NeedsComponent"/> is "0 = starving / dehydrated /
    /// exhausted / miserable, 1 = full / hydrated / rested / comfortable" —
    /// low values indicate urgent need. <c>NeedsCriticalEvent</c> fires
    /// once per false→true transition across
    /// <see cref="NeedsComponent.CriticalThreshold"/>; <c>JobSystem</c>
    /// subscribes and reassigns the pawn to a recovery job
    /// (<c>JobKind.Eat</c>, <c>JobKind.Sleep</c>) when urgent.
    ///
    /// No recovery mechanism exists yet — neither food entities, nor an
    /// EatSystem that consumes food and restores
    /// <see cref="NeedsComponent.Satiety"/>, nor parallel systems for
    /// hydration / energy / comfort. Pawns therefore deplete indefinitely
    /// once spawned. Phase 5 introduces those systems; until then, the
    /// displayed need bars truthfully reflect ungrounded depletion. By the
    /// project's operating principle: state either exists or it does not —
    /// we do not fake recovery via inverted depletion.
    /// </summary>
    [SystemAccess(
        reads:  new Type[0],
        writes: new[] { typeof(NeedsComponent) },
        bus:    nameof(IGameServices.Pawns)
    )]
    [TickRate(TickRates.SLOW)]
    public sealed class NeedsSystem : SystemBase
    {
        // Depletion rates per SLOW tick.
        private const float SatietyDepletionPerTick   = 0.002f;
        private const float HydrationDepletionPerTick = 0.0015f;
        private const float EnergyDepletionPerTick    = 0.001f;
        private const float ComfortDepletionPerTick   = 0.0005f;

        // Per-entity edge state: remembers which needs were critical on the
        // previous tick so we publish NeedsCriticalEvent exactly once per
        // crossing rather than on every tick the value stays at or below threshold.
        private readonly Dictionary<(EntityId Entity, NeedKind Kind), bool> _critical
            = new();

        protected override void OnInitialize() { }

        public override void Update(float delta)
        {
            foreach (var entity in Query<NeedsComponent>())
            {
                var needs = GetComponent<NeedsComponent>(entity);
                needs.Satiety   = Math.Clamp(needs.Satiety   - SatietyDepletionPerTick,   0f, 1f);
                needs.Hydration = Math.Clamp(needs.Hydration - HydrationDepletionPerTick, 0f, 1f);
                needs.Energy    = Math.Clamp(needs.Energy    - EnergyDepletionPerTick,    0f, 1f);
                needs.Comfort   = Math.Clamp(needs.Comfort   - ComfortDepletionPerTick,   0f, 1f);
                SetComponent(entity, needs);

                CheckCritical(entity, NeedKind.Satiety,   needs.Satiety);
                CheckCritical(entity, NeedKind.Hydration, needs.Hydration);
                CheckCritical(entity, NeedKind.Energy,    needs.Energy);
                CheckCritical(entity, NeedKind.Comfort,   needs.Comfort);
            }
        }

        private void CheckCritical(EntityId entity, NeedKind kind, float value)
        {
            bool isCritical = value <= NeedsComponent.CriticalThreshold;
            var key = (entity, kind);
            _critical.TryGetValue(key, out bool wasCritical);

            if (!wasCritical && isCritical)
            {
                Services.Pawns.Publish(new NeedsCriticalEvent
                {
                    PawnId = entity,
                    Need   = kind,
                    Value  = value
                });
            }

            _critical[key] = isCritical;
        }
    }
}
