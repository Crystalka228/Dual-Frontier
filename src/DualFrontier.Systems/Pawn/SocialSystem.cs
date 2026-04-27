using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Pawn;
using DualFrontier.Core.ECS;

namespace DualFrontier.Systems.Pawn;

/// <summary>
/// Evolution of the social graph: friendships, rivalries, kinship.
/// Influences <c>MindComponent</c> by publishing
/// <c>SocialAffectEvent</c> (does not write mind directly).
///
/// Phase: 3 (pawns).
/// Tick: RARE (3600 frames) — social ties change slowly.
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(MindComponent) },
    writes: new[] { typeof(SocialComponent) },
    bus:    nameof(IGameServices.Pawns)
)]
[TickRate(TickRates.RARE)]
[BridgeImplementation(Phase = 3)]
public sealed class SocialSystem : SystemBase
{
    /// <summary>
    /// Bridge: pending Phase 3 expansion — will subscribe to ConversationEvent /
    /// GiftEvent.
    /// </summary>
    protected override void OnInitialize() { }

    public override void Update(float delta)
    {
        // TODO: Phase 3 — update the social graph.
    }
}
