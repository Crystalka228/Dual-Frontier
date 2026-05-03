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
    /// Per-pawn deficit accumulator. Each SLOW tick (~2s at 30 TPS), every need
    /// grows by its constant decay rate, capped at 1.0. The semantic per
    /// <see cref="NeedsComponent"/> is "0 = full / comfortable, 1 = starving /
    /// dehydrated / exhausted / miserable" — high values indicate urgent
    /// deficit. <c>NeedsCriticalEvent</c> fires once per false→true transition
    /// across <see cref="NeedsComponent.CriticalThreshold"/>; <c>JobSystem</c>
    /// subscribes and reassigns the pawn to a recovery job
    /// (<c>JobKind.Eat</c>, <c>JobKind.Sleep</c>) when urgent.
    ///
    /// No recovery mechanism exists yet — neither food entities, nor an
    /// EatSystem that consumes food and resets <see cref="NeedsComponent.Hunger"/>,
    /// nor parallel systems for thirst / sleep / comfort. Pawns therefore
    /// degrade indefinitely once spawned. Phase 5 introduces those systems;
    /// until then, the displayed need bars truthfully reflect ungrounded
    /// accumulation. By the project's operating principle: state either
    /// exists or it does not — we do not fake recovery via inverted decay.
    /// </summary>
    [SystemAccess(
        reads:  new Type[0],
        writes: new[] { typeof(NeedsComponent) },
        bus:    nameof(IGameServices.Pawns)
    )]
    [TickRate(TickRates.SLOW)]
    public sealed class NeedsSystem : SystemBase
    {
        // Decay rates per SLOW tick.
        private const float HungerDecayPerTick  = 0.002f;
        private const float ThirstDecayPerTick  = 0.0015f;
        private const float SleepDecayPerTick   = 0.001f;
        private const float ComfortDecayPerTick = 0.0005f;

        // Per-entity edge state: remembers which needs were critical on the
        // previous tick so we publish NeedsCriticalEvent exactly once per
        // crossing rather than on every tick the value stays above threshold.
        private readonly Dictionary<(EntityId Entity, NeedKind Kind), bool> _critical
            = new();

        protected override void OnInitialize() { }

        public override void Update(float delta)
        {
            foreach (var entity in Query<NeedsComponent>())
            {
                var needs = GetComponent<NeedsComponent>(entity);
                needs.Hunger  = Math.Clamp(needs.Hunger  + HungerDecayPerTick,  0f, 1f);
                needs.Thirst  = Math.Clamp(needs.Thirst  + ThirstDecayPerTick,  0f, 1f);
                needs.Rest    = Math.Clamp(needs.Rest    + SleepDecayPerTick,   0f, 1f);
                needs.Comfort = Math.Clamp(needs.Comfort + ComfortDecayPerTick, 0f, 1f);
                SetComponent(entity, needs);

                CheckCritical(entity, NeedKind.Hunger,  needs.Hunger);
                CheckCritical(entity, NeedKind.Thirst,  needs.Thirst);
                CheckCritical(entity, NeedKind.Rest,    needs.Rest);
                CheckCritical(entity, NeedKind.Comfort, needs.Comfort);
            }
        }

        private void CheckCritical(EntityId entity, NeedKind kind, float value)
        {
            bool isCritical = value >= NeedsComponent.CriticalThreshold;
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
