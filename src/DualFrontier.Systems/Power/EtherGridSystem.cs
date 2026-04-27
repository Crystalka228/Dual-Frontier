using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Building;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Power;

/// <summary>
/// Ether grid: nodes are connected by spatial radius and ether
/// density flows between neighbouring nodes. Feeds
/// <c>ConverterSystem</c> and magical buildings.
///
/// Phase: 2.
/// Tick: NORMAL (15 frames).
/// </summary>
// TODO: when another agent adds EtherNodeComponent to Components.Building/World,
// replace EtherComponent here with EtherNodeComponent.
[SystemAccess(
    reads:  new[] { typeof(EtherComponent), typeof(PowerConsumerComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 6)]
public sealed class EtherGridSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to EtherNodeChangedEvent / EtherDrainedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 2 — advance density flow and distribute to consumers.
    }
}
