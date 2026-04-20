using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Эволюция социального графа: дружбы, вражды, родственные связи.
/// Влияет на <c>MindComponent</c> через публикацию
/// <c>SocialAffectEvent</c> (не пишет mind напрямую).
///
/// Фаза: 3 (пешки).
/// Тик: RARE (3600 фреймов) — социалка меняется медленно.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(MindComponent) },
    writes: new[] { typeof(SocialComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.RARE)]
public sealed class SocialSystem : SystemBase
{
    /// <summary>
    /// TODO: Подписаться на ConversationEvent, GiftEvent.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 3 — подписка на социальные события");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 3 — обновление графа социальных связей.
    }
}
