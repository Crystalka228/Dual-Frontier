using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.World;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.World;

/// <summary>
/// Weather transitions: Markov chain of states (Clear, Rain,
/// EtherStorm, Snow, …). Publishes <c>WeatherChangedEvent</c>;
/// <c>BiomeSystem</c> reacts and updates biomes accordingly.
///
/// Phase: 1 (parallel with CombatSystem, ManaSystem).
/// Tick: RARE (3600 frames).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class WeatherSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to RitualCompletedEvent / NewDayEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 1 — step the Markov chain; on a state change publish WeatherChangedEvent.
    }
}
