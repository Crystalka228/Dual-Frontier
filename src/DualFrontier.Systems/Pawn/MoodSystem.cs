using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Interop;
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
            // K8.3+K8.4 Phase 4 — single batch wraps all MindComponent writes
            // for this Update (one P/Invoke at dispose); legacy SetComponent
            // mirrors for dual-write transition (removed Phase 5 commit 21).
            using var batch = NativeWorld.BeginBatch<MindComponent>();

            foreach (var entity in Query<NeedsComponent, MindComponent>())
            {
                var needs = GetComponent<NeedsComponent>(entity);
                var mind  = GetComponent<MindComponent>(entity);

                // Mood formula: average of wellness needs (0 = bad, 1 = good)
                float mood =
                    (needs.Satiety + needs.Hydration + needs.Sleep + needs.Comfort)
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

                batch.Update(entity, mind);
                SetComponent(entity, mind);
            }
        }
    }
}