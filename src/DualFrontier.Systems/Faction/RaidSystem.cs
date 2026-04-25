using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Спавн рейдов: формула силы по богатству колонии и
/// отношениям с соседями. Публикует <c>RaidIncomingEvent</c> в
/// <c>World</c> шину — остальные системы готовятся.
///
/// Фаза: 7.
/// Тик: RARE (3600 фреймов).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class RaidSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to NewDayEvent / RelationBreakdownEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 7 — решить, спавнить ли рейд, опубликовать RaidIncomingEvent.
    }
}
