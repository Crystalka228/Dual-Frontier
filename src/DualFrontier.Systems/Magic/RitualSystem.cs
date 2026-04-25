using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Длинные коллективные ритуалы: несколько пешек стоят в
/// круге, накапливается общий пул маны, в конце выдают эффект
/// (призыв, бафф, превращение узла). Публикует
/// <c>RitualCompletedEvent</c>.
///
/// Фаза: 4.
/// Тик: RARE (3600 фреймов) — шаги ритуала длинные.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(SchoolComponent) },
    writes: new[] { typeof(ManaComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 6)]
public sealed class RitualSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 6 will subscribe to RitualStartEvent / RitualAbortEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Фаза 4 — продвижение шагов ритуала, сбор маны участников.
    }
}
