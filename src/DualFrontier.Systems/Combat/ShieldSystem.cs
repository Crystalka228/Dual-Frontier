using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Combat;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Combat;

/// <summary>
/// Magical and technological shields: regeneration and damage
/// absorption (together with <c>DamageSystem</c>). Publishes
/// <c>ShieldBrokenEvent</c> when broken.
///
/// Phase: 2.
/// Tick: FAST (3 frames).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ShieldComponent) },
    writes: new[] { typeof(ShieldComponent) },
    bus:    nameof(IGameServices.Combat)
)]
[TickRate(TickRates.FAST)]
[BridgeImplementation(Phase = 5)]
public sealed class ShieldSystem : SystemBase
{
    /// <summary>
    /// Bridge: Phase 5 will subscribe to ShieldHitEvent / ShieldRechargeEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 5 — per-tick shield regeneration and absorption of incoming damage.
    }
}
