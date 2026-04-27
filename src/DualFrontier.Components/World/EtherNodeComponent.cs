using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.World;

/// <summary>
/// An ether node — source of boosted mana regeneration within <c>Radius</c>.
/// See GDD 4 "Magical Colony": meditating at a node ×2–3 mana regen
/// (GDD 4.2, 5.3). <c>Tier</c> — node tier (1..N), affects strength.
/// Property changes publish <c>EtherNodeChangedEvent</c>.
/// </summary>
public sealed class EtherNodeComponent : IComponent
{
    // TODO: public int Tier;
    // TODO: public float Radius;
}
