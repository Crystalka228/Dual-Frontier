using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Pawn;

namespace DualFrontier.Systems.Pawn
{
    [SystemAccess(
        reads:  new Type[0],
        writes: new[] { typeof(NeedsComponent) },
        bus:    nameof(IGameServices.Pawns)
    )]
    [TickRate(TickRates.SLOW)]
    public sealed class NeedsSystem : SystemBase
    {
        // Decay rates per second (tuned for 30Hz * 60 frames = SLOW tick)
        private const float HungerDecayPerTick  = 0.002f;
        private const float SleepDecayPerTick   = 0.001f;
        private const float ComfortDecayPerTick = 0.0005f;

        protected override void OnInitialize() { }

        public override void Update(float delta)
        {
            foreach (var entity in Query<NeedsComponent>())
            {
                var needs = GetComponent<NeedsComponent>(entity);
                needs.Hunger  = Math.Clamp(needs.Hunger  - HungerDecayPerTick,  0f, 1f);
                needs.Thirst  = Math.Clamp(needs.Thirst  - 0.0015f,             0f, 1f);
                needs.Rest = Math.Clamp(needs.Rest - SleepDecayPerTick, 0f, 1f);
                needs.Comfort = Math.Clamp(needs.Comfort - ComfortDecayPerTick, 0f, 1f);
                SetComponent(entity, needs);
            }
        }
    }
}