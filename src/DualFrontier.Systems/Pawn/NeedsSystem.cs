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
    /// </summary>
    [SystemAccess(
        reads:  new Type[0],
        writes: new[] { typeof(NeedsComponent) },
        bus:    nameof(IGameServices.Pawns)
    )]
    [TickRate(TickRates.SLOW)]
    public sealed class NeedsSystem : SystemBase
    {
        private const float SatietyDepletionPerTick   = 0.002f;
        private const float HydrationDepletionPerTick = 0.0015f;
        private const float SleepDepletionPerTick     = 0.001f;
        private const float ComfortDepletionPerTick   = 0.0005f;

        private readonly Dictionary<(EntityId Entity, NeedKind Kind), bool> _critical
            = new();

        protected override void OnInitialize()
        {
            // M8.5 — ConsumeSystem publishes NeedsRestoredEvent when a pawn
            // finishes consuming food / drinking water. The handler runs at
            // the phase boundary with this system's captured context, which
            // owns the single declared write to NeedsComponent.
            Services.Pawns.Subscribe<NeedsRestoredEvent>(OnNeedsRestored);
        }

        private void OnNeedsRestored(NeedsRestoredEvent evt)
        {
            if (!NativeWorld.TryGetComponent<NeedsComponent>(evt.PawnId, out NeedsComponent needs))
                return;
            switch (evt.Need)
            {
                case NeedKind.Satiety:
                    needs.Satiety   = Math.Clamp(needs.Satiety   + evt.Amount, 0f, 1f);
                    break;
                case NeedKind.Hydration:
                    needs.Hydration = Math.Clamp(needs.Hydration + evt.Amount, 0f, 1f);
                    break;
                case NeedKind.Sleep:
                    needs.Sleep     = Math.Clamp(needs.Sleep     + evt.Amount, 0f, 1f);
                    break;
                case NeedKind.Comfort:
                    needs.Comfort   = Math.Clamp(needs.Comfort   + evt.Amount, 0f, 1f);
                    break;
            }
            using WriteBatch<NeedsComponent> batch = NativeWorld.BeginBatch<NeedsComponent>();
            batch.Update(evt.PawnId, needs);
        }

        public override void Update(float delta)
        {
            // Snapshot pass: read every NeedsComponent from a span lease,
            // collect (entity, modifiedNeeds) into a buffer, then dispose
            // the lease before opening the BeginBatch. The native side
            // forbids batch creation while a span lease is active on the
            // same world, but reads via TryGetComponent are fine alongside
            // the batch — see WriteBatch / SpanLease lifetime contracts.
            var entities = new List<EntityId>();
            var updated = new List<NeedsComponent>();
            using (SpanLease<NeedsComponent> needs = NativeWorld.AcquireSpan<NeedsComponent>())
            {
                ReadOnlySpan<NeedsComponent> needsSpan = needs.Span;
                ReadOnlySpan<int> indices = needs.Indices;
                for (int i = 0; i < needs.Count; i++)
                {
                    NeedsComponent n = needsSpan[i];
                    n.Satiety   = Math.Clamp(n.Satiety   - SatietyDepletionPerTick,   0f, 1f);
                    n.Hydration = Math.Clamp(n.Hydration - HydrationDepletionPerTick, 0f, 1f);
                    n.Sleep     = Math.Clamp(n.Sleep     - SleepDepletionPerTick,     0f, 1f);
                    n.Comfort   = Math.Clamp(n.Comfort   - ComfortDepletionPerTick,   0f, 1f);
                    entities.Add(new EntityId(indices[i], 0));
                    updated.Add(n);
                }
            }

            using (WriteBatch<NeedsComponent> batch = NativeWorld.BeginBatch<NeedsComponent>())
            {
                for (int i = 0; i < entities.Count; i++)
                    batch.Update(entities[i], updated[i]);
            }

            // Critical-edge tracking can use the post-update snapshot we just
            // built — the values are identical to what just landed on disk.
            for (int i = 0; i < entities.Count; i++)
            {
                EntityId e = entities[i];
                NeedsComponent n = updated[i];
                CheckCritical(e, NeedKind.Satiety,   n.Satiety);
                CheckCritical(e, NeedKind.Hydration, n.Hydration);
                CheckCritical(e, NeedKind.Sleep,     n.Sleep);
                CheckCritical(e, NeedKind.Comfort,   n.Comfort);
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
