using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Рост навыков пешек по накопленному опыту. Публикует
/// <c>SkillGainEvent</c> в <c>Pawns</c> шину при повышении уровня.
///
/// Фаза: 3 (пешки).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(SkillsComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.NORMAL)]
[BridgeImplementation(Phase = 3)]
public sealed class SkillSystem : SystemBase
{
    /// <summary>
    /// Bridge: pending Phase 3 expansion — will subscribe to SkillXpGainedEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 3 — применить накопленный XP, повысить уровни, опубликовать SkillGainEvent.
    }
}
