using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn
{
    [SystemAccess(
        reads:  new[] { typeof(NeedsComponent) },
        writes: new[] { typeof(MindComponent) },
        bus:    nameof(IGameServices.Pawns)
    )]
    [TickRate(TickRates.SLOW)]
    public sealed class MoodSystem : SystemBase
    {
        protected override void OnInitialize() { }

        public override void Update(float delta)
        {
            foreach (var entity in Query<NeedsComponent, MindComponent>())
            {
                var needs = GetComponent<NeedsComponent>(entity);
                var mind  = GetComponent<MindComponent>(entity);

                // Mood formula: average of inverted needs (0 = bad, 1 = good)
                float mood = 1f
                    - (needs.Hunger + needs.Thirst + needs.Rest + needs.Comfort)
                    / 4f;

                bool wasBreaking = mind.Mood < mind.MoodBreakThreshold;
                mind.Mood = Math.Clamp(mood, 0f, 1f);
                bool isBreaking = mind.Mood < mind.MoodBreakThreshold;

                // Publish MoodBreakEvent on transition into break state
                if (!wasBreaking && isBreaking)
                {
                    Services.Pawns.Publish(new MoodBreakEvent
                    {
                        PawnId    = entity,
                        MoodValue = mind.Mood
                    });
                }

                SetComponent(entity, mind);
            }
        }
    }
}