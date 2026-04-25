using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Shared;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Faction;

/// <summary>
/// Матрица отношений между фракциями [-100..100]: агрессия,
/// нейтральность, союз. Меняется от событий (убийства,
/// подарки, торговля) и просто времени.
///
/// Фаза: 7 (метаигра, после основного мира).
/// Тик: RARE (3600 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(FactionComponent), typeof(SocialComponent) },
    writes: new Type[0],
    bus:    nameof(IGameServices.World)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 7)]
public sealed class RelationSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 7 will subscribe to DeathEvent / TradeCompletedEvent /
    /// RaidIncomingEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 7 — применить накопленные изменения к матрице отношений.
    }
}
