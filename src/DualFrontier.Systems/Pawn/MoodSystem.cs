using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
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
            // Snapshot the (entity, updated MindComponent, was/is breaking)
            // tuples first by reading both spans, then issue all writes via
            // a single batch to satisfy the «no live span lease while batch
            // is being committed» contract.
            var entities = new List<EntityId>();
            var updated = new List<MindComponent>();
            var transitions = new List<(EntityId Entity, float Mood)>();

            using (SpanLease<MindComponent> minds = NativeWorld.AcquireSpan<MindComponent>())
            {
                ReadOnlySpan<MindComponent> mindSpan = minds.Span;
                ReadOnlySpan<int> mindIndices = minds.Indices;

                for (int i = 0; i < minds.Count; i++)
                {
                    var entity = new EntityId(mindIndices[i], 0);
                    if (!NativeWorld.TryGetComponent<NeedsComponent>(entity, out NeedsComponent needs))
                        continue;

                    MindComponent mind = mindSpan[i];

                    float mood =
                        (needs.Satiety + needs.Hydration + needs.Sleep + needs.Comfort)
                        / 4f;

                    bool wasBreaking = mind.Mood < mind.MoodBreakThreshold;
                    mind.Mood = Math.Clamp(mood, 0f, 1f);
                    bool isBreaking = mind.Mood < mind.MoodBreakThreshold;

                    if (!wasBreaking && isBreaking)
                        transitions.Add((entity, mind.Mood));

                    entities.Add(entity);
                    updated.Add(mind);
                }
            }

            using (WriteBatch<MindComponent> batch = NativeWorld.BeginBatch<MindComponent>())
            {
                for (int i = 0; i < entities.Count; i++)
                    batch.Update(entities[i], updated[i]);
            }

            foreach ((EntityId entity, float moodValue) in transitions)
            {
                Services.Pawns.Publish(new MoodBreakEvent
                {
                    PawnId    = entity,
                    MoodValue = moodValue
                });
            }
        }
    }
}
